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
    public partial class ThirdPartyMaster : System.Web.UI.Page
    {
        # region Variables
        RqstInsptnPlnBLL IRBL = new RqstInsptnPlnBLL();
        ErrorLog ELog = new ErrorLog();
        ThirdPartyBLL TPBLL = new ThirdPartyBLL();
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
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../login.aspx", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    txtEndDate.Attributes.Add("readonly", "readonly");
                    txtStartDate.Attributes.Add("readonly", "readonly");
                    Ajax.Utility.RegisterTypeForAjax(typeof(ThirdPartyMaster));

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
                        InchargeShip();
                        ClearAll();
                        BindGridview();
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
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

        private void InchargeShip()
        {
            try
            {
                DataSet ds = IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagQSelect, Guid.Empty, CommonBLL.TraffickerContactTypeText, new Guid(Session["CompanyID"].ToString()));
                //DataSet ds = IRBL.SelectInsptnReport(CommonBLL.FlagQSelect, CommonBLL.TraffickerContactType, Convert.ToInt32(Session["CompanyID"]));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlInchargeShipTo.DataSource = ds.Tables[0];
                    ddlInchargeShipTo.DataTextField = "Description";
                    ddlInchargeShipTo.DataValueField = "ID";
                }
                ddlInchargeShipTo.DataBind();
                ddlInchargeShipTo.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        private void BindGridview()
        {
            try
            {
                DataSet ds = TPBLL.GetData(CommonBLL.FlagSelectAll, Guid.Empty, "", "", "", false, Guid.Empty, "", Guid.Empty, Guid.Empty,
                    Guid.Empty, DateTime.Now, DateTime.Now, 0, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now,
                    true, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView DView = new DataView(ds.Tables[0]);
                    //DView.Sort = "ID DESC";
                    gvThirdParty.DataSource = DView;
                }
                else
                    gvThirdParty.DataSource = null;
                gvThirdParty.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        private void ClearAll()
        {
            txtContactCode.Text = "";
            txtContactPerson.Text = "";
            txtContractValidRemainder.Text = "";
            txtEndDate.Text = "";
            txtInspection.Text = "";
            txtStartDate.Text = "";
            txtStreet.Text = "";
            ddlInchargeShipTo.SelectedValue = "0";
            ddlCountry.SelectedIndex = -1;
            ddlState.SelectedIndex = -1;
            ddlCountry.SelectedIndex = -1;
            CascadingDropDown3.SelectedValue = "0";
            CascadingDropDown2.SelectedValue = "0";
            CascadingDropDown1.SelectedValue = "0";
            // CascadingFirstDropDown.SelectedValue = " " (empty)
            ddlCity.SelectedIndex = ddlCountry.SelectedIndex = ddlState.SelectedIndex = -1;
            ViewState["EditID"] = null;
        }

        private void EditRecord(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = TPBLL.GetData(CommonBLL.FlagModify, ID, "", "", "", false, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty,
                    DateTime.Now, DateTime.Now, 0, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, Guid.Empty);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    txtInspection.Text = ds.Tables[0].Rows[0]["InspAgentname"].ToString();
                    txtContactPerson.Text = ds.Tables[0].Rows[0]["ContactPerson"].ToString();
                    txtContactCode.Text = ds.Tables[0].Rows[0]["ContactCode"].ToString();
                    if (Convert.ToBoolean(ds.Tables[0].Rows[0]["IsContractValied"].ToString()))
                    {
                        rbtnYes.Checked = true;
                        rbtnNo.Checked = false;
                    }
                    else
                    {
                        rbtnYes.Checked = false;
                        rbtnNo.Checked = true;
                    }
                    txtStreet.Text = ds.Tables[0].Rows[0]["StreetNo"].ToString();

                    ddlInchargeShipTo.SelectedValue = ds.Tables[0].Rows[0]["InchargeShipTo"].ToString();
                    CascadingDropDown1.SelectedValue = ds.Tables[0].Rows[0]["Country"].ToString();
                    CascadingDropDown2.SelectedValue = ds.Tables[0].Rows[0]["State"].ToString();
                    CascadingDropDown3.SelectedValue = ds.Tables[0].Rows[0]["City"].ToString();
                    txtStartDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDateTime"].ToString()));
                    txtEndDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDateTime"].ToString()));
                    txtContractValidRemainder.Text = ds.Tables[0].Rows[0]["RemainderWeeks"].ToString();
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        # endregion

        # region GridView Events

        protected void gvThirdParty_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvThirdParty.HeaderRow == null) return;
                gvThirdParty.UseAccessibleHeader = false;
                gvThirdParty.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        protected void gvThirdParty_RowDataBound(object sender, GridViewRowEventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        protected void gvThirdParty_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int res = 1;
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvThirdParty.Rows[index];
                Guid ID = new Guid(gvThirdParty.DataKeys[index].Values["ID"].ToString());
                if (e.CommandName == "Modify")
                {
                    ViewState["EditID"] = ID;
                    EditRecord(ID);

                }
                else if (e.CommandName == "Remove")
                {
                    res = TPBLL.InsertUpdateDelete(CommonBLL.FlagDelete, ID, "", "", "", false, Guid.Empty, "", Guid.Empty, Guid.Empty,
                        Guid.Empty, DateTime.Now, DateTime.Now, 0, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, Guid.Empty);
                    BindGridview();
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Customer Master",
                            "Data Deleted successfully in ThirdParty Master, ID = " + ID + ".");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        protected void gvThirdParty_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClicks

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int res = 1;
            try
            {
                Filename = FileName();
                bool ContractValid = false;
                if (rbtnYes.Checked)
                    ContractValid = true;
                if (btnSave.Text == "Save")
                {
                    res = TPBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, txtInspection.Text.Trim(), txtContactPerson.Text.Trim(),
                        txtContactCode.Text.Trim(), ContractValid, new Guid(ddlInchargeShipTo.SelectedValue), txtStreet.Text.Trim(),
                        new Guid(ddlCountry.SelectedValue), new Guid(ddlState.SelectedValue), new Guid(ddlCity.SelectedValue),
                        CommonBLL.DateInsert(txtStartDate.Text.Trim()), CommonBLL.DateInsert(txtEndDate.Text.Trim()),
                        Convert.ToInt32(txtContractValidRemainder.Text.Trim()), new Guid(Session["UserID"].ToString()),
                        DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Third Party Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridview();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                            "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ClearAll();
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Third Party Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                            "ShowAll", "ErrorMessage(" + CommonBLL.ErrorMsg + ");", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", "Error while Inserting.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = TPBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                        txtInspection.Text.Trim(), txtContactPerson.Text.Trim(), txtContactCode.Text.Trim(), ContractValid,
                        new Guid(ddlInchargeShipTo.SelectedValue), txtStreet.Text.Trim(),
                        new Guid(ddlCountry.SelectedValue), new Guid(ddlState.SelectedValue), new Guid(ddlCity.SelectedValue),
                        CommonBLL.DateInsert(txtStartDate.Text.Trim()), CommonBLL.DateInsert(txtEndDate.Text.Trim()),
                        Convert.ToInt32(txtContractValidRemainder.Text.Trim()), Guid.Empty, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                        new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {

                        BindGridview();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                            "ShowAll", "SuccessMessage(" + CommonBLL.SuccessMsgUpdate + ");", true);
                        btnSave.Text = "Save";
                        ClearAll();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                            "ShowAll", "ErrorMessage(" + CommonBLL.ErrorMsgUpdate + ");", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Masters/ThirdPartyMaster.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Third Party Master", ex.Message.ToString());
            }
        }

        # endregion
    }
}
