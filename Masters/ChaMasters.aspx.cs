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
    public partial class ChaMasters : System.Web.UI.Page
    {

        # region variables
        ErrorLog ELog = new ErrorLog();
        ChaMasterBLL CMBL = new ChaMasterBLL();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        #endregion

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

        /// <summary>
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                BindGridData((CMBL.SelectChaMaster1(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Data
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void BindGridData(DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    chamaster.DataSource = CommonDt;
                    chamaster.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Record
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditMaster(DataTable CommonDt)
        {
            try
            {
                txtcode.Text = CommonDt.Rows[0]["Code"].ToString();
                txtName.Text = CommonDt.Rows[0]["Name"].ToString();
                txtcontactperson.Text = CommonDt.Rows[0]["ContactPrsn"].ToString();
                txtblngstrt.Text = CommonDt.Rows[0]["Street"].ToString();
                txtblngpb.Text = CommonDt.Rows[0]["PBoxNo"].ToString();
                txtmbl.Text = CommonDt.Rows[0]["MobileNo"].ToString();
                txtAgentsLicence.Text = CommonDt.Rows[0]["AgentsLicenceNo"].ToString();
                CascadingDropDown1.SelectedValue = CommonDt.Rows[0]["Cntry"].ToString();
                CascadingDropDown2.SelectedValue = CommonDt.Rows[0]["State"].ToString();
                CascadingDropDown3.SelectedValue = CommonDt.Rows[0]["City"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear All Inputs
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                txtcode.Text = txtName.Text = txtcontactperson.Text = txtAgentsLicence.Text = "";
                txtblngstrt.Text = txtblngpb.Text = txtmbl.Text = "";
                //ddlblngCntry.SelectedIndex =0;
                //ddlblngSt.SelectedIndex = 0;
                //ddlBilngCty.SelectedIndex = 0;
                CascadingDropDown1.SelectedValue = CascadingDropDown2.SelectedValue = CascadingDropDown3.SelectedValue = "";

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int res; Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    res = CMBL.InsertUpdateDeleteChaMaster1(CommonBLL.FlagNewInsert, Guid.Empty, txtcode.Text, txtName.Text,
                        txtcontactperson.Text, new Guid(ddlblngCntry.SelectedValue), new Guid(ddlblngSt.SelectedValue),
                        new Guid(ddlBilngCty.SelectedValue), txtblngstrt.Text, txtblngpb.Text, txtmbl.Text, txtAgentsLicence.Text.Trim(),
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Cha Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData((CMBL.SelectChaMaster1(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Cha Master",
                            "Saved Successfully in Cha Master.");
                        ClearInputs();
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Cha Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = CMBL.InsertUpdateDeleteChaMaster1(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtcode.Text, txtName.Text, txtcontactperson.Text, new Guid(ddlblngCntry.SelectedValue),
                        new Guid(ddlblngSt.SelectedValue), new Guid(ddlBilngCty.SelectedValue),
                        txtblngstrt.Text, txtblngpb.Text, txtmbl.Text, txtAgentsLicence.Text.Trim(),
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Cha Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData((CMBL.SelectChaMaster(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Cha Master",
                            "Updated Successfully in Cha Master.");
                        btnSave.Text = "Save";
                        ClearInputs();
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Cha Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                    }
                }
                GetData();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chamaster_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = chamaster.Rows[index];
                Guid ID = new Guid(((Label)gvrow.FindControl("txtchaMstr")).Text);
                if (e.CommandName == "Modify")
                {
                    ViewState["ID"] = ID;
                    EditMaster((CMBL.SelectChaMaster1(CommonBLL.FlagModify, ID, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    int res = CMBL.InsertUpdateDeleteChaMaster1(CommonBLL.FlagDelete, ID, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", "",
                       new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    BindGridData((CMBL.SelectChaMaster1(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Cha Master",
                            "Deleted successfully in Cha Master.");
                        ClearInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chamaster_RowDataBound(object sender, GridViewRowEventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        protected void chamaster_PreRender(object sender, EventArgs e)
        {
            try
            {
                chamaster.UseAccessibleHeader = false;
                chamaster.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Cha Master", ex.Message.ToString());
            }
        }

        #endregion
    }
}
