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
    public partial class TaxMater : System.Web.UI.Page
    {
        # region variables
        int res;
        FinancialYearBLL FNYBL = new FinancialYearBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
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
                        //btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            txtFrmDt.Attributes.Add("readonly", "readonly");
                            txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
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
                BindGridData(FNYBL.SelectTaxMaster(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlFnclYr, FNYBL.SelectFnclYr(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GirdView Data
        /// </summary>
        /// <param name="FnclYrDt"></param>
        protected void BindGridData(DataSet TxMster)
        {
            try
            {
                gvTxMstr.DataSource = TxMster.Tables[0];
                gvTxMstr.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
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
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }
        }
        #endregion

        #region GridView Row Command for Update/Delete Tax Master Details

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTxMstr_RowDataBound(object sender, GridViewRowEventArgs e)
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
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }



            //int lastCellIndex = e.Row.Cells.Count - 1;
            //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            //deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTxMstr_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvTxMstr.HeaderRow == null) return;
                gvTxMstr.UseAccessibleHeader = false;
                gvTxMstr.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTxMstr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvTxMstr.Rows[index];
                Guid ID = new Guid(((Label)gvrow.FindControl("lblTaxMaster")).Text);
                if (e.CommandName == "Modify")
                {
                    SetUpdateValues(FNYBL.SelectTaxMaster(CommonBLL.FlagModify, ID, new Guid(Session["CompanyID"].ToString())));
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    res = FNYBL.InsertUpdateDeleteTaxMaster(CommonBLL.FlagDelete, ID, Guid.Empty, 0, 0, 0, DateTime.Now, DateTime.Now,
                        Guid.Empty, new Guid(Session["UserID"].ToString()));
                    BindGridData(FNYBL.SelectTaxMaster(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Tax Master", "Deleted successfully.");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Values to inputs for Update
        /// </summary>
        /// <param name="ItmMstrDt"></param>
        protected void SetUpdateValues(DataSet TxMstrDt)
        {
            try
            {
                ddlFnclYr.SelectedValue = TxMstrDt.Tables[0].Rows[0]["FinancialYearID"].ToString();
                txtExdt.Text = TxMstrDt.Tables[0].Rows[0]["ExPercent"].ToString();
                txtSltx.Text = TxMstrDt.Tables[0].Rows[0]["Salestax"].ToString();
                txtDCRt.Text = TxMstrDt.Tables[0].Rows[0]["DCRate"].ToString();
                txtFrmDt.Text = TxMstrDt.Tables[0].Rows[0]["StartDate"].ToString();
                txtToDt.Text = TxMstrDt.Tables[0].Rows[0]["EndDate"].ToString();
                hdfdTaxMasterID.Value = TxMstrDt.Tables[0].Rows[0]["ExID"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
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
                    int result = FNYBL.InsertUpdateDeleteTaxMaster(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlFnclYr.SelectedValue),
                        Convert.ToDecimal(txtExdt.Text), Convert.ToDecimal(txtSltx.Text), Convert.ToDecimal(txtDCRt.Text),
                        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyID"].ToString()),
                        new Guid(Session["UserID"].ToString()));

                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Tax Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(FNYBL.SelectTaxMaster(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Tax Master", "Data Inserted successfully in Tax Master.");
                        //Response.Redirect("../Home.Aspx");
                        ClearAll();
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    int result = FNYBL.InsertUpdateDeleteTaxMaster(CommonBLL.FlagUpdate, new Guid(hdfdTaxMasterID.Value),
                        new Guid(ddlFnclYr.SelectedValue), Convert.ToDecimal(txtExdt.Text), Convert.ToDecimal(txtSltx.Text),
                        Convert.ToDecimal(txtDCRt.Text), CommonBLL.DateInsert(txtFrmDt.Text),
                        CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    //CommonBLL.UserID);
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Tax Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(FNYBL.SelectTaxMaster(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Tax Master", "Updated successfully in Tax Master.");
                        btnSave.Text = "Save";
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
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
                txtExdt.Text = txtSltx.Text = txtFrmDt.Text = txtToDt.Text = txtDCRt.Text = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Tax Master", ex.Message.ToString());
            }
        }
        #endregion
    }
}
