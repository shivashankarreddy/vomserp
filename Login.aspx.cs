using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BAL;
using System.Net.Mail;
using Ajax;
using System.IO;
using VOMS_ERP.Admin;

namespace VOMS_ERP
{
    public partial class Login : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        LoginBLL bll = new LoginBLL();
        CommonBLL CommBll = new CommonBLL();
        DataSet dsTeamMembers = new DataSet();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

        # region Page Events

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["logout"] != null && Request.QueryString["logout"] == "yes")
                {
                    Session.Remove("UserMail");
                    Session.Remove("UserID");
                    Session.Remove("UserName");
                    Session.Remove("TeamMembers");
                    Session.Remove("UsrPermissions");
                    Session.Remove("UserList");
                    Session.Remove("LoginType");
                    Session.Remove("UserDtls");
                    Session.Abandon();
                    Session["UserID"] = Session["UserName"] = Session["TeamMembers"] = CommonBLL.UserList = "";
                    Session["previous"] = Request.UrlReferrer.ToString();

                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
                }
                else if (Request.QueryString["logout"] != null && Request.QueryString["logout"] == "no")
                {
                    Session["UserMail"] = null;
                    Session["UserID"] = null;
                    Session["UserName"] = null;
                    Session["TeamMembers"] = null;
                    Session["UsrPermissions"] = null;
                    Session["UserList"] = null;
                    Session.Abandon();
                    Session["UserID"] = Session["UserName"] = Session["TeamMembers"] = CommonBLL.UserList = "";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "javascript:alert('You do't have permission, please contact your Administrator...');", true);
                }
                else
                {
                    if (!IsPostBack)
                    {
                        loginerrormsg.Visible = false;
                        this.Page.LoadComplete += new EventHandler(Page_LoadComplete);
                    }
                }

                Ajax.Utility.RegisterTypeForAjax(typeof(Login));
                btnReSet.Attributes.Add("OnClick", "javascript:return Myvalidations()");
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
            }
        }

        /// <summary>
        /// Default Page Load Complete Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Request.QueryString["logout"] != null && Request.QueryString["logout"] == "yes")
                Response.Redirect("Login.aspx", false);
            else if (Request.QueryString["logout"] != null && Request.QueryString["logout"] == "no")
                Response.Redirect("Login.aspx", false);
        }

        # endregion

        # region Methods

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <returns></returns>
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        /// <summary>
        /// This Meathod is used to check Captcha image
        /// </summary>
        /// <returns></returns>
        protected bool CImageCheck()
        {
            //if (this.txtimgcode.Text == this.Session["CaptchaImageText"].ToString())
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //    //lblmsg.Text = "image code is not valid.";
            //}
            return true;
        }
        # endregion

        # region ButtonClicks
        /// <summary>
        /// To check valid Authentication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (txtUser.Text.Trim() != "" && txtPass.Text.Trim() != "")
                {
                    if (bll.LogIn(Convert.ToChar("S"), txtUser.Text, txtPass.Text, true))
                    {
                        ArrayList al = bll.UserDetails();
                        Session["UserMail"] = txtUser.Text;
                        Session["UserID"] = al[1].ToString();
                        Session["UserName"] = al[0].ToString();
                        Session["AliasName"] = al[4].ToString();
                        Session["TLMailID"] = al[5].ToString(); Session["TLID"] = al[6].ToString();
                        Session["LoginType"] = al[7].ToString();
                        Session["Custmr_SuplrID"] = al[8].ToString();
                        Session["TeamID"] = al[9].ToString(); Session["DeptID"] = al[10].ToString();
                        Session["CompanyID"] = al[12].ToString();
                        Session["CurrencySymbol"] = al[13].ToString();
                        Session["AccessRole"] = al[14].ToString();
                        Session["IsUser"] = al[15].ToString(); Session["TeamName"] = al[16].ToString();
                        Session["UserType"] = al[17].ToString();
                        Session["UserDtls"] = al;

                        #region To Get Team Members List
                        if (Session["UserID"] != null)
                        {
                            if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
                                Session["TeamMembers"] = "All";
                            else if (Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                            { Session["TeamMembers"] = "All"; }
                            else
                            {
                                dsTeamMembers = CommBll.GetTeamMembers(new Guid(Session["UserID"].ToString()));
                                Session["TeamMembers"] = dsTeamMembers.Tables[0].Rows[0]["userslist"].ToString().Trim(',');
                                CommonBLL.UserList = dsTeamMembers.Tables[0].Rows[0]["userslist"].ToString().Trim(',');
                            }
                        }
                        #endregion

                        if (Convert.ToBoolean(al[2]))
                        {
                            if (Session["previous"] != null)
                                Response.Write(Session["previous"].ToString());
                            else
                            {
                                ALS.AuditLogForLogin(0, "Submit", "", Session["UserName"].ToString(), new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                if (Session["AccessRole"] != null && Session["AccessRole"].ToString() == "Customer")
                                {
                                    Response.Redirect("~/Masters/Home.aspx", false);
                                }
                                else
                                {
                                    Response.Redirect("~/Masters/Dashboard.aspx", false);
                                }
                                #region Old code before Customer Access

                                //if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
                                //{
                                //    Response.Redirect("~/Masters/CHome.aspx", false);
                                //}
                                //else
                                //{
                                //    Response.Redirect("~/Masters/Dashboard.aspx", false);
                                //}
                                #endregion
                            }
                        }
                        else
                        {
                            Session["AuId"] = al[3].ToString();
                            Response.Redirect("~/Masters/RequestForCngePwd.aspx", false);
                        }
                    }
                    else
                    {
                        loginerrormsg.Visible = true;
                        loginerrormsg.Text = "Invalid User Mail-ID OR Password. Please Try again.";
                        txtUser.Text = "";
                        txtPass.Text = "";
                    }
                }
                else
                {
                    loginerrormsg.Visible = true;
                    loginerrormsg.Text = "Please Enter User Mail-ID and Password.";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("/Logs/Others/ErrorLog"), "Login Failed", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Reset Password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReSet_Click(object sender, EventArgs e)
        {
            try
            {
                string PWD = Membership.GeneratePassword(8, 1);
                LoginBLL LBLL = new LoginBLL();
                int res = LBLL.ResetPWD(CommonBLL.FlagRegularDRP, txtUserMail.Text, PWD);
                if (res == 0)
                {
                    CommonBLL.SendMails(txtUserMail.Text.Trim(), "", "Requested for Password Reset.", "Your New Reset Password is " + PWD);

                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Reset Success, Please check Your Mail.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Reset Password", "Reset Password successfull in Login.");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('invalid E-MailID');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                //ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "ForGot Password", ex.Message.ToString());
            }
        }
        # endregion

        #region CheckEmail Validation for Reset Password

        /// <summary>
        /// Checking Email for Reset Password
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckEmail(string txtemail)
        {
            try
            {
                DataSet ChkMl = bll.CheckEmailID('A', txtemail);
                if (ChkMl.Tables.Count > 0)
                    return ChkMl.Tables[0].Rows.Count > 0 ? ChkMl.Tables[0].Rows[0]["ID"].ToString() + "#" +
                        ChkMl.Tables[0].Rows[0]["SecretAnswer1"].ToString() + "#" + ChkMl.Tables[0].Rows[0]["SecretAnswer2"].ToString() + "~" +
                        ChkMl.Tables[0].Rows[0]["Question1"].ToString() + "~" + ChkMl.Tables[0].Rows[0]["Question2"].ToString() : "";
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                return ErrMSg;
            }
        }

        #endregion
    }
}
