using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using Ajax;
using System.Data.SqlClient;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class EmailBodyContent : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        CompanyBLL CBL = new CompanyBLL();
        EmailBodyBLL EBBLL = new EmailBodyBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";

        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(EmailBodyContent));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            Session.Remove("uploads");
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Body Content", ex.Message.ToString());
            }
        }


        #endregion

        #region Methods

        protected Boolean GetDuplicaterecords()
        {
            DataSet ds = new DataSet();
            try
            {
                if (ddlCompany.SelectedValue != null && ddlCompany.SelectedValue != Guid.Empty.ToString() && ddlPage.SelectedValue != null && ddlPage.SelectedValue != Guid.Empty.ToString())
                {
                    ds = EBBLL.GetEmBdyDetails(CommonBLL.FlagCSelect, Guid.Empty, new Guid(ddlCompany.SelectedValue), new Guid(ddlPage.SelectedValue), "", "", new Guid(Session["UserId"].ToString()), DateTime.Now
                                                      , Guid.Empty, DateTime.Now, true, "", new Guid(Session["CompanyId"].ToString()));
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                string ErMsg = ex.Message;
                int Lineno = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Body Content", ex.Message.ToString());
                return true;
            }
        }
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

        private void GetData()
        {
            try
            {
                BindDropDownList(ddlCompany, CBL.SelectCompany(CommonBLL.FlagRegularDRP, new Guid(Session["CompanyId"].ToString())).Tables[0]);
                //BindDropDownList(ddlPage, EBBLL.GetEmBdyDetails(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, "", "", new Guid(Session["UserID"].ToString()), DateTime.Now
                  // , Guid.Empty, DateTime.Now, false, "", new Guid(Session["CompanyId"].ToString())).Tables[0]);
               
                if (Request.QueryString != null && Request.QueryString["ID"] != "")
                {
                   
                    EditDetails(EBBLL.GetEmBdyDetails(CommonBLL.FlagVSelect, new Guid(Request.QueryString["ID"]), Guid.Empty, Guid.Empty, "", "", new Guid(Session["UserID"].ToString()), DateTime.Now
                    , Guid.Empty, DateTime.Now, false, "", new Guid(Session["CompanyId"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Body Content", ex.Message.ToString());
            }
        }

        protected void EditDetails(DataSet Ds)
        {
            try
            {
                BindDropDownList(ddlPage, EBBLL.GetEmBdyDetails(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(Ds.Tables[0].Rows[0]["ScreenId"].ToString()), "", "", new Guid(Session["UserID"].ToString()), DateTime.Now
                   , Guid.Empty, DateTime.Now, false, "", new Guid(Session["CompanyId"].ToString())).Tables[0]);
                //ddlCompany.Items.FindByValue(Ds.Tables[0].Rows[0]["ComapnyName"].ToString()).Selected = true;
                ddlCompany.SelectedValue = Ds.Tables[0].Rows[0]["ComapnyName"].ToString();
               // ddlPage.Items.FindByValue(Ds.Tables[0].Rows[0]["ScreenId"].ToString()).Selected = true;
                ddlPage.SelectedValue = Ds.Tables[0].Rows[0]["ScreenId"].ToString();
                txtMsgBdy.Text = Ds.Tables[0].Rows[0]["Message"].ToString();
                txtSbjct.Text = Ds.Tables[0].Rows[0]["Subject"].ToString();
                btnSave.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Body Content", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataTable commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        protected void Clear()
        {
            ddlCompany.SelectedIndex = ddlPage.SelectedIndex = -1;
            txtMsgBdy.Text = txtSbjct.Text = "";
        }

        #endregion

        #region DDL Selected Index Change Event

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCompany.SelectedValue != Guid.Empty.ToString())
            {
                //BindDropDownList(ddlPage, EBBLL.GetEmBdyDetails(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, "", "", new Guid(Session["UserID"].ToString()), DateTime.Now
                //   , Guid.Empty, DateTime.Now, false, "", new Guid(Session["CompanyId"].ToString())).Tables[0]);
                BindDropDownList(ddlPage, EBBLL.GetEmBdyDetails(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, "", "", new Guid(Session["UserID"].ToString()), DateTime.Now
                 , Guid.Empty, DateTime.Now, false, "", new Guid(ddlCompany.SelectedValue)).Tables[0]);
            }
            else
            {
                ddlPage.Items.Clear();
                txtSbjct.Text = txtMsgBdy.Text = "";
            }
        }

        #endregion

        #region Button Click
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int res = 0;
                bool dup_records = GetDuplicaterecords();
                if (btnSave.Text == "Save")
                {
                    if (dup_records == false)
                    {
                        res = EBBLL.InsertUpdatedDeltedEmBdyDetails(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCompany.SelectedValue),
                            new Guid(ddlPage.SelectedValue), txtSbjct.Text, txtMsgBdy.Text, new Guid(Session["UserId"].ToString()),
                            DateTime.Now, Guid.Empty, DateTime.Now, true, "", new Guid(Session["CompanyID"].ToString()));
                    }
                    else
                    {
                        res = -1;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('The record you are trying to Insert already exists. ');", true);
                        Clear();
                    }
                }
                else
                {
                    res = EBBLL.InsertUpdatedDeltedEmBdyDetails(CommonBLL.FlagUpdate, new Guid(Request.QueryString["ID"]), new Guid(ddlCompany.SelectedValue),
                        new Guid(ddlPage.SelectedValue), txtSbjct.Text, txtMsgBdy.Text, new Guid(Session["UserId"].ToString()),
                        DateTime.Now, Guid.Empty, DateTime.Now, true, "", new Guid(Session["CompanyID"].ToString()));
                }
                if (res == 0)
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Email Body Content", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('" + CommonBLL.SuccessMsg + "');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "New Email BodyContent", "");
                    Response.Redirect("~/Admin/EmailBodyContent.aspx");
                    Clear();
                }
                else
                {
                    if(res != -1)
                    {
                    ALS.AuditLog(res, btnSave.Text, "", "Email Body Content", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                          "ErrorMessage('Error while Updating.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "New Email BodyContent", "");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "New Email BodyContent", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            Response.Redirect("~/Admin/EmailBodyContent.aspx");
        }
        #endregion

        #region WebMethods
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID)
        {
            try
            {
                int res = 0;
                string result = "Success";
                #region Delete
                if (result == "Success")
                {

                    res = EBBLL.InsertUpdatedDeltedEmBdyDetails(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now
                        , false, "", new Guid(Session["CompanyID"].ToString()));
                }
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                    result = "Error::Cannot Delete this Record";

                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

       
    }
}