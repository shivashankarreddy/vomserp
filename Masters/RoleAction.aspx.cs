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
using Ajax;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class RoleAction : System.Web.UI.Page
    {
        # region variables
        int res = 1;
        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RoleActionBLL RABll = new RoleActionBLL();
        TeamMasterBLL TMBLL = new TeamMasterBLL();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(RoleAction));
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                        else
                            BingGridView();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    Page.LoadComplete += new EventHandler(Page_LoadComplete);
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Default Page Load Complete Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            if ((string[])Session["UsrPermissions"] == null || !((string[])Session["UsrPermissions"]).Contains("View"))
            {
                Session["NoPermission"] = "YES";
                Response.Redirect("../Masters/Home.aspx", false);
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
        /// To fill grid view
        /// </summary>
        private void GetData()
        {
            try
            {
                GetRolesDRP();
                GetTeamsDRP();
                BingGridView();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Bind Method
        /// </summary>
        private void BingGridView()
        {
            try
            {
                DataSet ds = RABll.GetScreenAndPermission();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    gvRlActn.DataSource = ds.Tables[0];
                    gvRlActn.DataBind();
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Roles for Drop Down List
        /// </summary>
        private void GetRolesDRP()
        {
            try
            {

                DataSet ds = EMBLL.GetEnumTypesWithGuid(CommonBLL.FlagRegularDRP, CommonBLL.ContactType, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContactType);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ddlTmMbrNm.DataSource = ds;
                    ddlTmMbrNm.DataTextField = "Description";
                    ddlTmMbrNm.DataValueField = "ID";
                    ddlTmMbrNm.DataBind();
                }
                ddlTmMbrNm.Items.Insert(0, new ListItem("-- Select ContactType--", Guid.Empty.ToString()));//Guid.Empty.ToString()
                ddlTmMbrNm.Items.Remove(ddlTmMbrNm.Items.FindByText("Trafficker"));
                ddlTmMbrNm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Team Names for Drop Down List
        /// </summary>
        private void GetTeamsDRP()
        {
            try
            {

                DataSet ds = TMBLL.TeamMasterSelect(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())); ;
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ddlTmNm.DataSource = ds;
                    ddlTmNm.DataTextField = "TeamName";
                    ddlTmNm.DataValueField = "TeamID";
                    ddlTmNm.DataBind();
                }
                ddlTmNm.Items.Insert(0, new ListItem("-- Select Team--", Guid.Empty.ToString()));//Guid.Empty.ToString()
                ddlTmNm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                ddlTmMbrNm.SelectedIndex = ddlTmNm.SelectedIndex = -1;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "JavaScript:ClearAllChkb()", true);
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        #endregion

        #region GridView RowDataBound Event

        /// <summary>
        /// To fill permission check boxes in row data bound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRlActn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    DataSet ds = RABll.GetScreenAndPermission();
                    if (ds != null && ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            CheckBox cb = new CheckBox();
                            cb.ID = ds.Tables[1].Rows[i]["ID"].ToString();
                            cb.Text = ds.Tables[1].Rows[i]["Description"].ToString();
                            e.Row.Cells[3].Controls.Add(cb);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRlActn_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvRlActn.UseAccessibleHeader = false;
                gvRlActn.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Save Button Event

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                DataSet dse = RABll.GetScreenAndPermission();
                if (hdfldbtnvalue.Value == "Save")
                {
                    foreach (GridViewRow grow in gvRlActn.Rows)
                    {

                        Label scrid = (Label)grow.FindControl("lblScreenID");
                        string permission = (((CheckBox)grow.FindControl(dse.Tables[1].Rows[0]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[0]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[1]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[1]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[2]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[2]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[3]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[3]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString());
                        res = RABll.InsertUpdateRoleActionMaster(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlTmMbrNm.SelectedValue),
                            new Guid(ddlTmNm.SelectedValue), new Guid(scrid.Text), permission, new Guid(Session["UserID"].ToString()));
                    }
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Role Action Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Role Action Master",
                            "Data Inserted successfully in Role Action Master.");
                        Response.Redirect(Request.RawUrl);
                        ClearAll();

                    }
                }
                else if (hdfldbtnvalue.Value == "Update")
                {
                    DataSet ds = new DataSet();
                    ds = (DataSet)Session["RoleAction"];
                    foreach (GridViewRow grow in gvRlActn.Rows)
                    {
                        Label scrid = (Label)grow.FindControl("lblScreenID");
                        HiddenField ID = (HiddenField)grow.FindControl("hdfldsrnid");
                        string permission = (((CheckBox)grow.FindControl(dse.Tables[1].Rows[0]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[0]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[1]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[1]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[2]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[2]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString()) + "," +
                                            (((CheckBox)grow.FindControl(dse.Tables[1].Rows[3]["ID"].ToString())).Checked ?
                                            ((CheckBox)grow.FindControl(dse.Tables[1].Rows[3]["ID"].ToString())).ID.ToString() : Guid.Empty.ToString());
                        string screenID = Guid.Empty.ToString();
                        DataRow[] result = ds.Tables[0].Select("ScreenID = '" + ID.Value.ToString() + "'");
                        foreach (DataRow row in result)
                            screenID = row[0].ToString();

                        if (screenID != Guid.Empty.ToString())
                            res = RABll.InsertUpdateRoleActionMaster(CommonBLL.FlagUpdate, new Guid(screenID),
                                new Guid(ddlTmMbrNm.SelectedValue), new Guid(ddlTmNm.SelectedValue), new Guid(scrid.Text), permission, new Guid(Session["UserID"].ToString()));
                        else
                            res = RABll.InsertUpdateRoleActionMaster(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlTmMbrNm.SelectedValue),
                                new Guid(ddlTmNm.SelectedValue), new Guid(scrid.Text), permission, new Guid(Session["UserID"].ToString()));
                    }
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Role Action Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        //Response.Redirect(Request.RawUrl);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Role Action Master",
                            "Data Inserted successfully in Role Action Master.");

                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Ajax Method for getting data for update

        /// <summary>
        /// Getting all Permissions to reset into gridview for updating
        /// </summary>
        /// <param name="selID"></param>
        /// <param name="RoleGroup"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public DataSet GetScreens(string selID, string RoleGroup)
        {
            try
            {
                if (new Guid(selID) != Guid.Empty)
                {
                    DataSet RAction = RoleGroup == "RoleID" ? RABll.SelectRAMasterEdit(CommonBLL.FlagASelect, new Guid(selID)) :
                        RABll.SelectRAMasterEdit(CommonBLL.FlagBSelect, new Guid(selID));
                    if (RAction.Tables.Count > 0)
                        if (RAction != null && RAction.Tables[0].Rows.Count > 0)
                        {
                            Session["RoleAction"] = RAction;
                            return RAction;
                        }
                        else
                            return null;
                    else
                        return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Role Action Master", ex.Message.ToString());
                return null;
            }
        }

        #endregion

    }
}
