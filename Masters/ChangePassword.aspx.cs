using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.Services;
using BAL;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        # region variables

        AssignUserBLL AUBLL = new AssignUserBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

        #region Default Page Load Event


        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }


        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("login.aspx", false);
                else
                {
                    if (!IsPostBack)
                    {
                        btnChange.Attributes.Add("OnClick", "javascript:return Myvalidations()");

                        ddlUsrNm.Items.Clear();
                        ddlUsrNm.Items.Insert(0, Session["UserName"].ToString());
                        Session["UserMail"].ToString();
                        txtCrntPwd.Text = "";

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                Response.Redirect("../Login.aspx", false);
            }
        }
        #endregion

        #region Clear All Inputs
        /// <summary>
        /// Clear All Input Controls
        /// </summary>
        private void ClearControls()
        {
            txtCrntPwd.Text = "";
            txtNwPwd.Text = "";
            txtCfmPwd.Text = "";
        }
        #endregion

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
        /// This Method is used to Encrypr PWD
        /// </summary>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        public static string MD5(string sPassword)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(sPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
        # endregion

        # region ButtonClicks
        /// <summary>
        /// This buttonEvent is used to Change Password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if ((txtNwPwd.Text.Trim() == txtCfmPwd.Text.Trim()) && (txtNwPwd.Text.Trim() != "" && txtCfmPwd.Text.Trim() != ""))
                {
                    int res = 0;
                    DataSet CommonDt = AUBLL.GetAssignedUsersEdit(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["UserID"].ToString()));
                    if (CommonDt != null && CommonDt.Tables.Count > 0)
                    {
                        string OldPswd = CommonDt.Tables[0].Rows[0]["Password"].ToString();

                        if (OldPswd == MD5(txtCrntPwd.Text.Trim()))
                        {
                            string EncPWD = MD5(txtNwPwd.Text.Trim());
                            res = AUBLL.AssignedUsersInsert(CommonBLL.FlagCSelect, Guid.Empty, new Guid(Session["UserID"].ToString()), "", EncPWD, Guid.Empty, "",
                                Guid.Empty, "", false, Guid.Empty, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                            if (res == 0)
                            {
                                ALS.AuditLog(res, "Save", "", "Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Password Changed Successfully');", true);
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", 
                                //"SuccessMessage(Password Changed Successfully.);", true);
                                Session.Remove("UserDtls");
                                Session.Remove("UserID");
                                Response.Redirect("../Login.aspx?logout=yes");
                            }
                            else
                            {
                                ALS.AuditLog(res, "Save", "", "Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error While Changing Password.');", true);
                            }
                        }
                        else
                        {
                            ALS.AuditLog(res, "Save", "", "Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                   "ErrorMessage('Old Password is not Correct.');", true);
                        }
                    }
                    else
                    {
                        ALS.AuditLog(res, "Save", "", "Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                               "ErrorMessage('User does not exitst, please contact your admin.');", true);
                    }

                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Password Does Not Match.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }


        /// <summary>
        /// This is used to Clear controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
        }
        # endregion

        # region WebMeathods

        [WebMethod]
        public static string CheckPwd(string PWD)
        {
            string res = "";
            try
            {
                string EncryptedPWD = MD5(PWD);
                DataSet ds = new DataSet();
                CheckBLL CBLL = new CheckBLL();
                ds = CBLL.CheckMail(CommonBLL.FlagPSelectAll, HttpContext.Current.Session["UserID"].ToString(), new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString() == EncryptedPWD)
                        res = "True";
                    else
                        res = "False";
                }
                else
                    return "False";
            }
            catch (Exception ex)
            {
                string Errmsg = ex.Message;
                res = "";
            }
            return res;
        }

        # endregion
    }
}
