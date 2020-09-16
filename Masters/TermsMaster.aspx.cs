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
using BAL;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class TermsMaster : System.Web.UI.Page
    {
        # region variables
        int res;
        BAL.TermsMasterBLL TrmsMstr = new TermsMasterBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load
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
            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
            if (!IsPostBack)
            {
                try
                {
                    if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                        Response.Redirect("../Login.aspx?logout=yes", false);
                    else
                    {
                        if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        {
                            #region Add/Update Permission Code
                            //To Check User can have the Add/Update permissions
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Enabled = true;
                                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            }
                            else
                            {
                                btnSave.Enabled = false;
                                btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                            }
                            #endregion
                            GetData();
                        }
                        else
                            Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
                catch (Exception ex)
                {
                    string ErrMsg = ex.Message;
                    int LineNo = ExceptionHelper.LineNumber(ex);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
                }
            }
            // btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
        }
        #endregion

        #region Button Click Events (Insert/Update into Terms Master)
        /// <summary>
        /// Save and Updte Button Click Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    int result = TrmsMstr.InsertUpdateTermsMasterForInsert(CommonBLL.FlagNewInsert, Guid.Empty, txtTermNm.Text,
                        txtTrmVlu.Text, "Text Box", ddlArea.SelectedValue == "" ? Guid.Empty : new Guid(ddlArea.SelectedValue),
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Terms Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(TrmsMstr.SelectTermsMasterForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Terms Master",
                            "Data Inserted successfully in Terms Master.");
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "MySuccessMessage(Terms Added Successfully.);", true);
                        ClearAll();
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    int result = TrmsMstr.InsertUpdateTermsMasterForInsert(CommonBLL.FlagUpdate, new Guid(hdfdTermMstrID.Value),
                        txtTermNm.Text, txtTrmVlu.Text, "Text Box", ddlArea.SelectedValue == "" ? Guid.Empty : new Guid(ddlArea.SelectedValue), Guid.Empty, new Guid(Session["UserID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Terms Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(TrmsMstr.SelectTermsMasterForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Terms Master", "Data Updated successfully in Terms Master.");
                        btnSave.Text = "Save";
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        #endregion

        #region GridView Row command Update/Delete Terms Master

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmsMstr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];

                if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
                    EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";

                if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                else
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";

                //int lastCellIndex = e.Row.Cells.Count - 1;
                //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                //deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmsMstr_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvTmsMstr.UseAccessibleHeader = false;
                gvTmsMstr.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView RowCommand Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmsMstr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvTmsMstr.Rows[index];
                if (e.CommandName == "Modify")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblTrmMstr")).Text);
                    SetUpdateValues(TrmsMstr.SelectTermsMasterForEdit(CommonBLL.FlagModify, ID, Guid.Empty));
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblTrmMstr")).Text);
                    res = TrmsMstr.DeleteTermsMasterToDelete(CommonBLL.FlagDelete, ID);
                    BindGridData(TrmsMstr.SelectTermsMasterForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Terms Master", "Data Deleted successfully in Terms Master.");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Values to inputs for Update
        /// </summary>
        /// <param name="ItmMstrDt"></param>
        protected void SetUpdateValues(DataSet TermsMstrDt)
        {
            try
            {
                txtTermNm.Text = TermsMstrDt.Tables[0].Rows[0]["Description"].ToString();
                hdnDsptn.Value = TermsMstrDt.Tables[0].Rows[0]["Description"].ToString();
                //ddlIpTP.SelectedItem.Text = TermsMstrDt.Tables[0].Rows[0]["InputType"].ToString().Trim();
                txtTrmVlu.Text = TermsMstrDt.Tables[0].Rows[0]["Value"].ToString();
                hdfdTermMstrID.Value = TermsMstrDt.Tables[0].Rows[0]["ID"].ToString();
                ddlArea.SelectedValue = TermsMstrDt.Tables[0].Rows[0]["TermArea"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Bind GridData

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
        /// Bind Data to GirdView
        /// </summary>

        protected void GetData()
        {
            try
            {
                BindGridData(TrmsMstr.SelectTermsMasterForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlArea, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.TermArea));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View
        /// </summary>
        /// <param name="TermsMsterDt"></param>
        protected void BindGridData(DataSet TermsMsterDt)
        {
            try
            {
                if (TermsMsterDt != null && TermsMsterDt.Tables.Count > 0)
                {
                    DataView DView = new DataView(TermsMsterDt.Tables[0]);
                    DView.Sort = "ID DESC";
                    gvTmsMstr.DataSource = DView;
                }
                else
                    gvTmsMstr.DataSource = null;
                gvTmsMstr.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind All Drop Down Lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Clear all Input Fields

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                txtTermNm.Text = txtTrmVlu.Text = "";
                //ddlIpTP.SelectedValue = "0";
                ddlArea.SelectedIndex = -1;
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms Master", ex.Message.ToString());
            }
        }

        #endregion


    }
}
