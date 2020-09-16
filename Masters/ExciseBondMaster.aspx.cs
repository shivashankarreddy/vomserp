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
    public partial class ExciseBondMaster : System.Web.UI.Page
    {
        # region variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL embal = new EnumMasterBLL();
        FinancialYearBLL FNYBL = new FinancialYearBLL();
        ExciseBondMstrBLL EBMBL = new ExciseBondMstrBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load Event

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
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
                        if (!IsPostBack)
                        {
                            txtDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind GridView/DropDownList

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
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                BindGridData(EBMBL.SelectExBndMstrForBind(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlFnclYr, FNYBL.SelectFnclYr(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GirdView Data
        /// </summary>
        /// <param name="FnclYrDt"></param>
        protected void BindGridData(DataSet ExBndMster)
        {
            try
            {
                if (ExBndMster != null && ExBndMster.Tables.Count > 0 && ExBndMster.Tables[0].Rows.Count > 0)
                {
                    if (ExBndMster.Tables.Count > 0 && ExBndMster.Tables[0].Rows.Count > 0)
                    {
                        gvExBndMstr.DataSource = ExBndMster.Tables[0];
                        gvExBndMstr.DataBind();
                    }
                }
                else
                {
                    gvExBndMstr.DataSource = null;
                    gvExBndMstr.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownList
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
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }
        #endregion

        #region GridView Row Command for Update/Delete Tax Master Details

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvExBndMstr_RowDataBound(object sender, GridViewRowEventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvExBndMstr_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvExBndMstr.HeaderRow == null) return;
                gvExBndMstr.UseAccessibleHeader = false;
                gvExBndMstr.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvExBndMstr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvExBndMstr.Rows[index];
                if (e.CommandName == "Modify")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblExBndMaster")).Text);
                    SetUpdateValues(EBMBL.SelectExBndMstrForBind(CommonBLL.FlagModify, ID, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblExBndMaster")).Text);
                    res = EBMBL.InsertUpdateDeleteExBndMstrToInsert(CommonBLL.FlagDelete, ID, DateTime.Now, 0, Guid.Empty, "", Guid.Empty, new Guid(Session["UserID"].ToString()));
                    BindGridData(EBMBL.SelectExBndMstrForBind(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Excise Bond Master", "Deleted successfully.");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Values to inputs for Update
        /// </summary>
        /// <param name="ItmMstrDt"></param>
        protected void SetUpdateValues(DataSet ExBndMstrDt)
        {
            try
            {
                ddlFnclYr.SelectedValue = ExBndMstrDt.Tables[0].Rows[0]["FinancialYearID"].ToString();
                txtExValue.Text = ExBndMstrDt.Tables[0].Rows[0]["BondValue"].ToString();
                txtDt.Text = ExBndMstrDt.Tables[0].Rows[0]["BondDate"].ToString();
                txttowardsdescription.Text = ExBndMstrDt.Tables[0].Rows[0]["TwsDscptn"].ToString();
                hdfdExBndMasterID.Value = ExBndMstrDt.Tables[0].Rows[0]["ExciseBondID"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Insert Data into Table (Button Click Events)

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
                    res = EBMBL.InsertUpdateDeleteExBndMstrToInsert(CommonBLL.FlagNewInsert, Guid.Empty, CommonBLL.DateInsert(txtDt.Text),
                        Convert.ToDecimal(txtExValue.Text), new Guid(ddlFnclYr.SelectedValue), txttowardsdescription.Text, new Guid(Session["CompanyID"].ToString()),
                        new Guid(Session["UserID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Excise Bond Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(EBMBL.SelectExBndMstrForBind(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Excise Bond Master",
                            "Data Inserted successfully in Excise Bond Master.");
                        ClearAll();
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = EBMBL.InsertUpdateDeleteExBndMstrToInsert(CommonBLL.FlagUpdate, new Guid(hdfdExBndMasterID.Value),
                        CommonBLL.DateInsert(txtDt.Text), Convert.ToDecimal(txtExValue.Text), new Guid(ddlFnclYr.SelectedValue),
                        txttowardsdescription.Text, Guid.Empty, new Guid(Session["UserID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Excise Bond Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(EBMBL.SelectExBndMstrForBind(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Excise Bond Master",
                            "Updated successfully in Excise Bond Master.");
                        btnSave.Text = "Save";
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Clear Inputs

        /// <summary>
        /// Clear all input fields
        /// </summary>

        protected void ClearAll()
        {
            try
            {
                ddlFnclYr.SelectedValue = "0";
                txtExValue.Text = txtDt.Text = txttowardsdescription.Text = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Excise Bond Master", ex.Message.ToString());
            }
        }
        #endregion
    }
}
