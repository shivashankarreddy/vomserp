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
using System.Web.Services;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class TeamMaster : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        ContactBLL CBLL = new ContactBLL();
        TeamMasterBLL tmb = new TeamMasterBLL();
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
            # region NotInUse
            //DataTable dtbl = new DataTable();
            //DataColumn dcl = new DataColumn();
            //dtbl.Columns.Add("ID");
            //dtbl.Columns.Add("Team Name");
            //dtbl.Columns.Add("Team Lead");
            //dtbl.Columns.Add("Parent Team");

            //DataRow frst = dtbl.NewRow();
            //frst["ID"] = "1";
            //frst["Team Name"] = "Projects";
            //frst["Team Lead"] = "Hema Chand";
            //frst["Parent Team"] = " --- ";
            //dtbl.Rows.Add(frst);

            //DataRow second = dtbl.NewRow();
            //second["ID"] = "2";
            //second["Team Name"] = "Purchases";
            //second["Team Lead"] = "D K Rao";
            //second["Parent Team"] = " --- ";
            //dtbl.Rows.Add(second);
            //DataRow third = dtbl.NewRow();
            //third["ID"] = "3";
            //third["Team Name"] = "Purchase & Projects";
            //third["Team Lead"] = "Nethaji";
            //third["Parent Team"] = "Projects";
            //dtbl.Rows.Add(third);


            //gvTmMstr.DataSource = dtbl;
            //gvTmMstr.DataBind();
            # endregion

            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        #region Add/Update Permission Code
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
                            ViewState["EditID"] = null;
                            BindParentGroup();
                            TeamLeadBind(CBLL.GetusersOnCtype(CommonBLL.FlagBSelect, "", new Guid(Session["CompanyID"].ToString())));
                            BindGrid();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }
        #endregion

        # region ButtonClicks

        /// <summary>
        /// This event is used to save and update TeamMaster details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                Guid tmName = Guid.Empty;
                Guid tmLead = Guid.Empty;
                if (ddlTmNm.SelectedIndex == 0)
                    tmName = Guid.Empty;
                else
                    tmName = new Guid(ddlTmNm.SelectedItem.Value);
                if (ddlTmLead.SelectedIndex != 0 && txtTmNm.Text.Trim() != "")
                {
                    tmLead = new Guid(ddlTmLead.SelectedItem.Value);
                    TeamMasterBLL tmb = new TeamMasterBLL();
                    if (btnSave.Text == "Save")
                        res = tmb.TeamMasterInsert(CommonBLL.FlagNewInsert, txtTmNm.Text, tmLead, tmName, new Guid(Session["UserID"].ToString())
                            , new Guid(Session["CompanyID"].ToString()));
                    else if (btnSave.Text == "Update")
                        res = tmb.TeamMasterUpdate(CommonBLL.FlagUpdate, txtTmNm.Text, tmLead, tmName, new Guid(Session["UserID"].ToString()),
                            new Guid(ViewState["EditID"].ToString()));
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Team Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Team Master", "Data inserted successfully in Team Master.");
                        ClearControls();
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Team Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('ERROR while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", "Getting Error.");
                    }
                    else if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Team Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Team Master", "Data Updated successfully in Team Master.");
                        ViewState["EditID"] = null;
                        btnSave.Text = "Save";
                        ClearControls();
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Team Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('ERROR while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", "Getting Error while Updating.");
                    }
                    BindParentGroup();
                    BindGrid();
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('TeamName & TeamLead Details are Required.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to Clear The Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["EditID"] = null;
                btnSave.Text = "Save";
                ClearControls();
                BindGrid();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }
        # endregion

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

        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("S.No.");
                dt.Columns.Add("TeamID");
                dt.Columns.Add("TeamName");
                dt.Columns.Add("TeamLead");
                dt.Columns.Add("ParentTeamID");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvTmMstr.DataSource = ds;
                gvTmMstr.DataBind();

                int columncount = gvTmMstr.Rows[0].Cells.Count;
                gvTmMstr.Rows[0].Cells.Clear();
                gvTmMstr.Rows[0].Cells.Add(new TableCell());
                gvTmMstr.Rows[0].Cells[0].ColumnSpan = columncount;
                gvTmMstr.Rows[0].Cells[0].Text = "<center>No Items To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind gridview
        /// </summary>
        private void BindGrid()
        {
            try
            {
                DataSet ds = tmb.TeamMasterSelect(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    gvTmMstr.DataSource = ds;
                    gvTmMstr.DataBind();
                }
                else
                {
                    gvTmMstr.DataSource = null;
                    gvTmMstr.DataBind();
                    //NoTable();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This method is used to bind the parent Group
        /// </summary>
        private void BindParentGroup()
        {
            try
            {
                DataSet ds = tmb.TeamMasterSelect(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlTmNm.DataSource = ds;
                    ddlTmNm.DataTextField = "TeamName";
                    ddlTmNm.DataValueField = "TeamID";
                    ddlTmNm.DataBind();
                    ddlTmNm.Items.Insert(0, new ListItem("-- Select ParentName--", Guid.Empty.ToString()));
                    ddlTmNm.SelectedIndex = -1;
                }
                else
                {
                    ddlTmNm.Items.Insert(0, new ListItem("-- Select ParentName--", Guid.Empty.ToString()));
                    ddlTmNm.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind TeamLead and same meathod is used in teammemberMaster.aspx page
        /// </summary>
        private void TeamLeadBind(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables[0].Rows.Count > 0)
                {
                    ddlTmLead.DataSource = CommonDt.Tables[0];
                    ddlTmLead.DataTextField = "FullName";
                    ddlTmLead.DataValueField = "ID";
                    ddlTmLead.DataBind();
                    ddlTmLead.Items.Insert(0, new ListItem("-- Select TeamLead--", Guid.Empty.ToString()));
                    ddlTmLead.SelectedIndex = 0;
                }
                else
                {
                    ddlTmLead.Items.Insert(0, new ListItem("-- Select TeamLead--", Guid.Empty.ToString()));
                    ddlTmLead.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// While Editing This is used to bind the controls
        /// </summary>
        /// <param name="ID"></param>
        private void EditTeam(Guid ID)
        {
            try
            {
                ClearControls();
                DataSet ds = tmb.TeamMasterEdit(CommonBLL.FlagModify, ID);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    TeamLeadBind(CBLL.TeamLeadBindDRP(CommonBLL.FlagFSelect, new Guid(Session["CompanyID"].ToString())));

                    txtTmNm.Text = ds.Tables[0].Rows[0]["TeamName"].ToString();
                    ddlTmLead.SelectedValue = ds.Tables[0].Rows[0]["TeamLead"].ToString();
                    ddlTmNm.SelectedValue = (ds.Tables[0].Rows[0]["ParentTeamID"].ToString() == "" ? Guid.Empty.ToString() : ds.Tables[0].Rows[0]["ParentTeamID"].ToString());
                    ddlTmNm.Items.Remove(ddlTmNm.Items.FindByText(ds.Tables[0].Rows[0]["TeamName"].ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());

            }
        }

        private void ClearControls()
        {
            try
            {
                txtTmNm.Text = "";
                ddlTmLead.SelectedIndex = -1;
                ddlTmNm.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string ReeMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        # endregion

        # region GridViewCommands

        /// <summary>
        /// This GridView Commands are used for EDIT and DELETE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmMstr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvTmMstr.Rows[index];
                string lblId = ((Label)row.FindControl("lblSNO")).Text.ToString();
                if (e.CommandName == "Modify")
                {
                    btnSave.Text = "Update";
                    ViewState["EditID"] = lblId;
                    EditTeam(new Guid(lblId));
                }
                else if (e.CommandName == "Delete")
                {
                    int res = tmb.ContactsDelete(CommonBLL.FlagDelete, new Guid(lblId));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", "Deleted Successfully.");
                        BindGrid(); ClearControls();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('ERROR While Deleting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", "Getting Error while Deleting.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Team Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This event is not using But It should be kept.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmMstr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        /// <summary>
        /// This GridView Event IS used to delete Conformation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvTmMstr_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void gvTmMstr_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvTmMstr.UseAccessibleHeader = false;
                gvTmMstr.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
            }
        }
        # endregion

        # region WebMeathods

        [WebMethod]
        public static string CheckTeamName(string TeamName)
        {
            string res = "";
            try
            {
                DataSet ds = new DataSet();
                CheckBLL CBLL = new CheckBLL();
                ds = CBLL.CheckMail(CommonBLL.FlagCommonMstr, TeamName, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString().ToLower() == TeamName.ToLower())
                        res = "False";
                    else
                        res = "True";
                }
                else
                    return "True";
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
