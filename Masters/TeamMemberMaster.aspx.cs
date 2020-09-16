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
    public partial class TeamMemberMaster : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

        #region Page Load Event
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
                            GetTeamMemberMastersAll();
                            GetTeamNames();
                            TeamLeadBind(Guid.Empty);
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }
        #endregion

        # region Meathods

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
        /// This meathod is used to bind gridview
        /// </summary>
        private void GetTeamMemberMastersAll()
        {
            try
            {
                TeamMemberMasterBLL tmmbll = new TeamMemberMasterBLL();
                DataSet ds = new DataSet();
                ds = tmmbll.GetTeamMemberMastersAll(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dView = new DataView(ds.Tables[0]);
                    //dView.Sort = "ID DESC";
                    gvdept.DataSource = dView;

                }
                else
                    gvdept = null;
                gvdept.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind TeamLead and same meathod is used in teammemberMaster.aspx page
        /// </summary>
        private void TeamLeadBind(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ContactBLL CBLL = new ContactBLL();
                if (ID == Guid.Empty)
                    ds = CBLL.TeamLeadBindDRP(CommonBLL.FlagASelect, new Guid(Session["CompanyID"].ToString()));
                else
                    ds = CBLL.TeamLeadBindDRP(CommonBLL.FlagXSelect, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlTmMbrNm.DataSource = ds;
                    ddlTmMbrNm.DataTextField = "FullName";
                    ddlTmMbrNm.DataValueField = "ID";
                    ddlTmMbrNm.DataBind();
                    ddlTmMbrNm.Items.Insert(0, new ListItem("-- Select TeamMember--", Guid.Empty.ToString()));
                    ddlTmMbrNm.SelectedIndex = 0;
                }
                else
                {
                    ddlTmMbrNm.Items.Insert(0, new ListItem("-- Select TeamMember--", Guid.Empty.ToString()));
                    ddlTmMbrNm.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Member Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to Bind TeamNames
        /// </summary>
        private void GetTeamNames()
        {
            try
            {
                DataSet ds = new DataSet();
                TeamMasterBLL tmb = new TeamMasterBLL();
                ds = tmb.TeamMasterSelect(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlTmNm.DataSource = ds;
                    ddlTmNm.DataTextField = "TeamName";
                    ddlTmNm.DataValueField = "TeamID";
                    ddlTmNm.DataBind();
                    ddlTmNm.Items.Insert(0, new ListItem("-- Select TeamName--", "0"));
                    ddlTmNm.SelectedIndex = 0;
                }
                else
                {
                    ddlTmNm.Items.Insert(0, new ListItem("-- Select TeamName--", "0"));
                    ddlTmNm.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// this MEathod is used to Featch a perticular Row From Teammembers table to bind Data to DDL's for  Editing
        /// </summary>
        /// <param name="ID">ID from TeamMembers Table</param>
        private void EditMember(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                TeamMemberMasterBLL tmmbll = new TeamMemberMasterBLL();
                ds = tmmbll.GetTeamMemberMastersEdit(CommonBLL.FlagModify, ID);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    TeamLeadBind(ID);
                    ddlTmNm.SelectedValue = ds.Tables[0].Rows[0]["TeamID"].ToString();
                    ddlTmMbrNm.SelectedValue = ds.Tables[0].Rows[0]["MemberID"].ToString();
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// this Meathod is used to Clear All Items and DropDownList's to Select Index 0
        /// </summary>
        private void ClearItems()
        {
            ddlTmMbrNm.SelectedIndex = 0;
            ddlTmNm.SelectedIndex = 0;
        }

        # endregion

        # region ButtonClicks

        /// <summary>
        /// This Button Event Is used to assign a Team Member to the Team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int res = 0;
                TeamMemberMasterBLL tmmbll = new TeamMemberMasterBLL();
                Guid MemberId = new Guid(ddlTmMbrNm.SelectedItem.Value);
                Guid teamID = new Guid(ddlTmNm.SelectedItem.Value);
                if (btnSave.Text == "Assign" && ViewState["EditID"] == null)
                {
                    res = tmmbll.TeamMemberMastersInsert(CommonBLL.FlagNewInsert, teamID, MemberId, new Guid(Session["UserID"].ToString()),
                        new Guid(Session["CompanyID"].ToString()));
                }
                else if (btnSave.Text == "Update" && ViewState["EditID"] != null)
                {
                    res = tmmbll.TeamMemberMastersUpdate(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), teamID, MemberId,
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                }
                if (res == 0 && btnSave.Text == "Assign")
                {
                    ALS.AuditLog(res, "Save", "", "Team Member Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Team Member Master",
                        "Data inserted successfully in Team Member Master.");
                    ClearItems();
                }
                else if (res != 0 && btnSave.Text == "Assign")
                {
                    ALS.AuditLog(res, "Save", "", "Team Member Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('ERROR while Saving.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Member Master", "Getting Error.");
                }
                else if (res == 0 && btnSave.Text == "Update")
                {
                    ALS.AuditLog(res, "Update", "", "Team Member Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Team Member Master",
                        "Data Updated successfully in Team Member Master.");
                    ViewState["EditID"] = null;
                    btnSave.Text = "Assign";
                    ClearItems();
                }
                else if (res != 0 && btnSave.Text == "Update")
                {
                    ALS.AuditLog(res, "Update", "", "Team Member Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('ERROR while Updating.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Member Master", "Getting Error while Updating.");
                }
                GetTeamMemberMastersAll();
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This ButtonClick Event IS used to Clear Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ViewState["EditID"] = null;
            btnSave.Text = "Assign";
            ClearItems();
            GetTeamMemberMastersAll();
        }
        # endregion

        # region GridView Commands
        /// <summary>
        /// This GridView Command Is used to EDIT and DELETE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvdept_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvdept.Rows[index];
                string lblId = ((Label)row.FindControl("lblID")).Text.ToString();
                if (e.CommandName.Equals("Modify"))
                {
                    ViewState["EditID"] = lblId;
                    EditMember(new Guid(lblId));
                }
                else if (e.CommandName.Equals("Delete"))
                {
                    TeamMemberMasterBLL tmmbll = new TeamMemberMasterBLL();
                    int res = tmmbll.GetTeamMemberMastersDelete(CommonBLL.FlagDelete, new Guid(lblId));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", "Deleted Successfully.");
                        GetTeamMemberMastersAll();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Error while Deleting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", "Getting Error while Deleting.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is not in use but it should be Declared for not Getting Error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvdept_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        /// <summary>
        /// This isused to Conform Either to Delete OR Not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvdept_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void gvdept_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvdept.UseAccessibleHeader = false;
                gvdept.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
        # endregion
    }
}
