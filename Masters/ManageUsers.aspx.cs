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

namespace VOMS_ERP.Masters
{
    public partial class ManageUsers : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

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
            try
            {
                if (Session["UserID"] == null || (Session["UserID"]).ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }
        #endregion

        # region GridView Events
        /// <summary>
        /// This GridView Event is used to assign user images, button Enable Disable ....
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsrLst_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string ID = ((Label)e.Row.FindControl("lblStatus")).Text;
                    if (ID == "True")
                    {
                        ((Image)e.Row.FindControl("imgSmbl")).ImageUrl = "~/images/Userstatus_Green.PNG";
                        LinkButton lbActive = (LinkButton)e.Row.Cells[5].Controls[0];
                        lbActive.Enabled = false;
                        LinkButton lbDactive = (LinkButton)e.Row.Cells[6].Controls[0];
                        lbDactive.Enabled = true;
                    }
                    else
                    {
                        ((Image)e.Row.FindControl("imgSmbl")).ImageUrl = "~/images/Userstatus_icon.PNG";
                        LinkButton lbDactive = (LinkButton)e.Row.Cells[5].Controls[0];
                        lbDactive.Enabled = true;
                        LinkButton lbActive = (LinkButton)e.Row.Cells[6].Controls[0];
                        lbActive.Enabled = false;
                    }

                    int lastCellIndex = e.Row.Cells.Count - 1;
                    LinkButton deleteButton = (LinkButton)e.Row.Cells[lastCellIndex].Controls[0];
                    LinkButton EditButton = (LinkButton)e.Row.Cells[lastCellIndex - 1].Controls[0];

                    if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
                        EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Activate.')) return false;";

                    if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
                        deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                    else
                        deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to DeActivate.')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This event is used to Active/Deactive users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsrLst_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvUsrLst.Rows[index];
                string lblId = ((Label)row.FindControl("lblID")).Text.ToString();
                int res = 0;
                if (Session["UserID"] == null)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "message",
                        "alert('Session Expaired Please Login Again.');location.href='../Login.Aspx?logout=yes';", true);
                }
                ContactBLL CBLL = new ContactBLL();
                if (e.CommandName.Equals("Active"))
                    res = CBLL.ContacstUpdate(CommonBLL.FlagBSelect, new Guid(lblId), "", "", "", Guid.Empty, Guid.Empty, "", "", "",
                        "", "", "", true, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), "", "");
                else if (e.CommandName.Equals("Deactive"))
                    res = CBLL.ContacstUpdate(CommonBLL.FlagBSelect, new Guid(lblId), "", "", "", Guid.Empty, Guid.Empty, "", "", "",
                        "", "", "", false, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), "", "");
                if (res == 0)
                {
                    if (e.CommandName.Equals("Active"))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Actived Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Manage Users Master",
                            "Data Updated successfully in Assign Users.");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('De-actived Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Manage Users Master",
                            "Data Updated successfully in Assign Users.");
                    }
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('ERROR while Updating.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users Master", "Getting Error.");
                }
                GetUsersByCID(ddlCtgry.SelectedItem.Text, new Guid(Session["CompanyID"].ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }

        protected void gvUsrLst_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvUsrLst.UseAccessibleHeader = false;
                gvUsrLst.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
        # endregion

        # region Meathods & DDL Event

        private void GetData()
        {
            try
            {
                GetRolesDRP();
                //ddlCtgry.SelectedIndex = 3;
                GetUsersByCID(ddlCtgry.SelectedItem.Text, new Guid(Session["CompanyID"].ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This method is used to bing Gridview based on selected contact type
        /// </summary>
        /// <param name="CID"></param>
        private void GetUsersByCID(string CID, Guid CompanyId)
        {
            try
            {
                DataSet ds = new DataSet();
                ContactBLL CBLL = new ContactBLL();
                ds = CBLL.ContactsBindGridByCID(CommonBLL.FlagPSelectAll, CID, CompanyId);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    gvUsrLst.DataSource = ds;
                    gvUsrLst.DataBind();
                }
                else
                {
                    gvUsrLst.DataSource = null;
                    gvUsrLst.DataBind();
                }

                //NoTables();
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Roles for Drop Down List
        /// </summary>
        private void GetRolesDRP()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.GetEnumTypesWithGuid(CommonBLL.FlagRegularDRP, CommonBLL.ContactType, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContactType);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlCtgry.DataSource = ds;
                    ddlCtgry.DataTextField = "Description";
                    ddlCtgry.DataValueField = "ID";
                    ddlCtgry.DataBind();
                }
                ddlCtgry.Items.Insert(0, new ListItem("-- Select ContactType--", Guid.Empty.ToString()));
                ddlCtgry.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This method is used when there are No tables
        /// </summary>
        private void NoTables()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("IsActive");
                dt.Columns.Add("ID");
                dt.Columns.Add("FullName");
                dt.Columns.Add("RoleName");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvUsrLst.DataSource = ds;
                gvUsrLst.DataBind();
                int columncount = gvUsrLst.Rows[0].Cells.Count;
                gvUsrLst.Rows[0].Cells.Clear();
                gvUsrLst.Rows[0].Cells.Add(new TableCell());
                gvUsrLst.Rows[0].Cells[0].ColumnSpan = columncount;
                gvUsrLst.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This event is used to bind gridview based on selected catagory / contact type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCtgry.SelectedValue != "0")
                    GetUsersByCID(ddlCtgry.SelectedItem.Text, new Guid(Session["CompanyID"].ToString()));
                else
                    NoTables();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Manage Users", ex.Message.ToString());
            }
        }

        # endregion
    }
}
