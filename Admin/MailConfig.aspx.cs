using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class MailConfig : System.Web.UI.Page
    {

        #region Variables

        ErrorLog ELog = new ErrorLog();
        EmailConfigBLL ECBLL = new EmailConfigBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || (Session["UserID"]).ToString() == "")
                Response.Redirect("login.aspx", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    if (!IsPostBack)
                        GetData();
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        #region Methods

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
            BindDropDownList(ddlCompanyID, ECBLL.GetDataSet(CommonBLL.FlagRegularDRP, Guid.Empty,new Guid(Session["CompanyID"].ToString())));
            GetDataForGridView(ECBLL.GetDataSet(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
        }

        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {

                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    ddl.Items.Clear();
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                else
                {
                    ddl.DataSource = null;
                    ddl.DataBind();
                }
                if (ddl != null)
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));

            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Config", ex.Message.ToString());
            }
        }

        private void GetDataForGridView(DataSet ds)
        {
            try
            {
                gvDomains.DataSource = ds;
                gvDomains.DataBind();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Config", ex.Message.ToString());
            }
        }

        private void ClearItems()
        {
            ddlCompanyID.SelectedIndex = -1;
            txtDomainName.Text = "";
            txtPortNo.Text = "";
            chkIsSSL.Checked = false;
            btnSave.Text = "Save";
            HFEditID.Value = "";
        }

        private void EditUser(Guid EditID)
        {
            try
            {
                HFEditID.Value = EditID.ToString();
                DataSet ds = ECBLL.GetDataSet(CommonBLL.FlagModify, EditID, Guid.Empty);
                BindDropDownList(ddlCompanyID, ECBLL.GetDataSet(CommonBLL.FlagBSelect, Guid.Empty, new Guid(ds.Tables[0].Rows[0]["CompanyId"].ToString())));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    txtDomainName.Text = ds.Tables[0].Rows[0]["DomainName"].ToString();
                    txtPortNo.Text = ds.Tables[0].Rows[0]["Port"].ToString();
                    ddlCompanyID.SelectedValue = ds.Tables[0].Rows[0]["CompanyId"].ToString();
                    chkIsSSL.Checked = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsSSL"].ToString());
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Config", ex.Message.ToString());
            }
        }

        #endregion

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int res = -1; Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    res = ECBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, txtDomainName.Text.Trim(), txtPortNo.Text.Trim(),
                        Convert.ToBoolean(chkIsSSL.Checked), new Guid(ddlCompanyID.SelectedValue), new Guid(Session["UserID"].ToString()),
                        DateTime.Now, Guid.Empty, DateTime.Now, true);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Not Saved Successfully.');", true);
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = ECBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(HFEditID.Value), txtDomainName.Text.Trim(), txtPortNo.Text.Trim(),
                        Convert.ToBoolean(chkIsSSL.Checked), new Guid(ddlCompanyID.SelectedValue), Guid.Empty, DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Not Updated Successfully.');", true);
                    }
                }
                GetData();
                ClearItems();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Email Config", ex.Message.ToString());
            }
        }

        protected void gvDomains_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvDomains.UseAccessibleHeader = false;
                gvDomains.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        protected void gvDomains_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvDomains.Rows[index];
                string lblId = ((Label)row.FindControl("lblID")).Text.ToString();
                if (e.CommandName == "Modify")
                {
                    ViewState["EditID"] = lblId;
                    EditUser(new Guid(lblId));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }
    }
}