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
using Ajax;
using System.Net.Mail;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class RequestForCngePwd : System.Web.UI.Page
    {

        # region variables
        int res, ID;
        BAL.AssignUserBLL asnusr = new AssignUserBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        ErrorLog ELog = new ErrorLog();
        #endregion

        #region Default Page Load Evnet

        //void Page_PreInit(object sender, EventArgs e)
        //{
        //    if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
        //    {
        //        MasterPageFile = "~/CustomerMaster.master";
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || (Session["UserID"]).ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!IsPostBack)
                    {
                        GetData();
                    }

                    string Qstn1 = Request.QueryString["ID"] as string;
                    if (Qstn1 != null)
                    {
                        DataTable table = (embal.EnumMasterSelectforDescription(CommonBLL.FlagZSelect, new Guid(Qstn1), Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.SecurityQuestions)).Tables[0];
                        DataRow drow = table.NewRow();
                        drow["ID"] = Guid.Empty.ToString(); drow["Description"] = "-- Select Question --";
                        table.Rows.InsertAt(drow, 0);

                        string result = string.Empty;

                        foreach (DataRow r in table.Rows)
                        {
                            result += r["Description"].ToString() + "," + r["ID"].ToString() + ";";
                        }

                        Response.Clear();
                        Response.Write(result);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }

                    Ajax.Utility.RegisterTypeForAjax(typeof(RequestForCngePwd));
                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }
        }
        #endregion

        #region Save/Change Button Click Event
        /// <summary>
        /// Save/Change Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                res = asnusr.AssignedUsersInsert(CommonBLL.FlagUpdate, new Guid(Session["AuId"].ToString()), Guid.Empty,
                    lblUsrID.Text, CommonBLL.MD5(txtNwPwd.Text), new Guid(ddlSQtn1.SelectedValue),
                    txtAnsQ1.Text, new Guid(ddlSQtn2.SelectedValue), txtAnsQ2.Text, true, new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ALS.AuditLog(res, "Save", "", "Request for Chage Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    CommonBLL.SendMails(lblUsrID.Text.Trim(), "", "Requested for Password Reset.", "Your New Reset Password is "
                        + txtNwPwd.Text);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Request for Chage Password",
                        "Password Changed Successfully.");
                    Response.Redirect("../Masters/Home.Aspx", false);
                }
                else
                {
                    ALS.AuditLog(res, "Save", "", "Request for Chage Password", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password",
                        "Getting Error while Changing.");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind/Get Required Data
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
        /// Get User Data for Checking
        /// </summary>
        ///
        protected void GetData()
        {
            try
            {
                BindDropDownList(ddlSQtn1, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, BAL.CommonBLL.SecurityQuestions));
                BindDropDownList(ddlSQtn2, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, BAL.CommonBLL.SecurityQuestions));
                SetValues(asnusr.GetAssignedUsersEdit(CommonBLL.FlagModify, new Guid(Session["AuId"].ToString()), Guid.Empty));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }
        }
        protected void SetValues(DataSet UserDt)
        {
            try
            {
                if (UserDt.Tables.Count > 0)
                {
                    lblUsrID.Text = UserDt.Tables[0].Rows[0]["EmailID"].ToString();
                    hdnfldpwd.Value = UserDt.Tables[0].Rows[0]["Password"].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }
        }
        protected void BindDropDownList(DropDownList ddl, DataSet commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }

                ddl.Items.Insert(0, new ListItem("-- Select Question--", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier", ex.Message.ToString());
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool ComparePWd(string CrntPwdTxt, string CrntPwdDb)
        {
            return CommonBLL.MD5(CrntPwdTxt) == CrntPwdDb ? true : false;
        }

        #endregion

        #region Clear Button Click Event
        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                txtCrntPwd.Text = txtNwPwd.Text = txtCfmPwd.Text = txtAnsQ1.Text = txtAnsQ2.Text = "";
                ddlSQtn1.SelectedValue = ddlSQtn2.SelectedValue = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }

        }
        #endregion

        #region Exit Button Click Event
        /// <summary>
        /// Exit Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Abandon();
                Response.Redirect("../Login.Aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Request for Chage Password", ex.Message.ToString());
            }
        }
        #endregion
    }
}
