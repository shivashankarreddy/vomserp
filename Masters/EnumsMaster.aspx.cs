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
using System.Collections.Generic;
using System.Web.Services;
using System.Threading;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class EnumsMaster : System.Web.UI.Page
    {

        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL EMBL = new EnumMasterBLL();
        CommonBLL CommBll = new CommonBLL();
        Guid CreatdBY = Guid.Empty;
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

        #region Default Paga Load Event

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        /// <summary>
        /// Default Paga Load Event
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

                        DataSet ds = new DataSet();
                        ds = CommBll.GetAuthorisedUsersList(new Guid(Session["UserID"].ToString()), Request.Path);
                        bool test = CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path);
                        string[] testll = null;
                        if ((string[])Session["UsrPermissions"] != null)
                            testll = (string[])Session["UsrPermissions"];

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (!IsPostBack)
                            {
                                //ClearAllInputs();
                                //rbgnenmtp.Checked = true;
                                if (Session["AccessRole"].ToString() != CommonBLL.SuperAdminRole && Session["AccessRole"].ToString() != CommonBLL.AdminRole)
                                {
                                    rbgnenmtp.Enabled = false;
                                }
                                rbtnenmdescs.Checked = true;
                                GetData();


                                string EnmTyp = Request.QueryString["ID"] as string;
                                if (EnmTyp != null)
                                {
                                    DataTable table;
                                    if (EnmTyp == CommonBLL.State)
                                    {
                                        table = EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries).Tables[0];
                                        DataRow drow = table.NewRow();
                                        drow["ID"] = Guid.Empty; drow["Description"] = "-- Select --";
                                        table.Rows.InsertAt(drow, 0);
                                    }
                                    else if (EnmTyp == CommonBLL.City)
                                    {
                                        table = EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.State).Tables[0];
                                        DataRow drow = table.NewRow();
                                        drow["ID"] = Guid.Empty; drow["Description"] = "-- Select --";
                                        table.Rows.InsertAt(drow, 0);
                                    }
                                    else if (EnmTyp == CommonBLL.IncotrmsSpec)
                                    {
                                        table = EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.IncotrmsSpec).Tables[0];
                                        DataRow drow = table.NewRow();
                                        drow["ID"] = Guid.Empty; drow["Description"] = "-- Select --";
                                        table.Rows.InsertAt(drow, 0);
                                    }
                                    else
                                    {
                                        table = EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Parent).Tables[0];
                                        DataRow drow = table.NewRow();
                                        drow["ID"] = Guid.Empty; drow["Description"] = "-- Select --";
                                        table.Rows.InsertAt(drow, 0);
                                        ddlEnmPrnt.Visible = false;
                                    }
                                    string result = string.Empty;

                                    foreach (DataRow r in table.Rows)
                                    {
                                        result += r["Description"].ToString() + "," + r["ID"].ToString() + ";";
                                    }
                                    Response.Clear();
                                    Response.Write(result);
                                    Response.End();
                                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                                }
                                //--------
                            }

                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }

                    }
                    else
                    {
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                //You can ignore this 
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
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
                BindDropDownList(ddlENmTP, EMBL.EnumTypeBind());
                BindGridView(gvEnmTp, EMBL.EnumTypeBind());
                BindGridView(gvEnumMaster, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownLists
        /// </summary>
        /// <param name="Ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList Ddl, DataSet CommonDt)
        {
            try
            {
                DataRow dr = CommonDt.Tables[0].NewRow();
                dr["EnmID"] = Guid.Empty; dr["Name"] = "--Select--";
                CommonDt.Tables[0].Rows.InsertAt(dr, 0);
                Ddl.DataSource = CommonDt.Tables[0];
                Ddl.DataTextField = "Name";
                Ddl.DataValueField = "EnmID";
                Ddl.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master & Enum Type(DropDownList)", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GridViews of Enum Type and Enum Master
        /// </summary>
        /// <param name="Gv"></param>
        /// <param name="CommonDt"></param>
        protected void BindGridView(GridView Gv, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    if (Gv.Caption == "EnumType")
                    {
                        DataView dView = new DataView(CommonDt.Tables[0]);
                        //dView.Sort = "EnmID DESC";
                        Gv.DataSource = dView;
                    }
                    else
                    {
                        DataView dView = new DataView(CommonDt.Tables[0]);
                        //dView.Sort = "ID DESC";
                        Gv.DataSource = dView;
                    }
                    Gv.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master & Enum Type(GridView)", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear All Inputs
        /// </summary>
        protected void ClearAllInputs()
        {
            try
            {
                if (rbgnenmtp.Checked)
                {
                    txtEnmTp.Text = "";
                }
                else
                {
                    txtEnmDesrp.Text = "";
                    ddlENmTP.SelectedValue = Guid.Empty.ToString();
                    ddlEnmPrnt.SelectedValue = Guid.Empty.ToString();
                }
                ViewState["EnumTpID"] = null;
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
            }
        }
        #endregion

        #region GridView Events

        /// <summary>
        /// Enum Type Grie Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnmTp_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvEnmTp.UseAccessibleHeader = false;
                gvEnmTp.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// Enum Master Grie Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnumMaster_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvEnumMaster.UseAccessibleHeader = false;
                gvEnumMaster.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// Enum Master GridView Row Command Envents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnumMaster_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvEnumMaster.Rows[index];
                if (e.CommandName == "Modify")
                {
                    txtEnmDesrp.Text = ((Label)gvrow.FindControl("lblDescription")).Text;
                    ddlENmTP.SelectedValue = ((Label)gvrow.FindControl("lblEnmTpID")).Text;
                    string valu = ((Label)gvrow.FindControl("lblEnmPrntID")).Text;
                    if (ddlENmTP.SelectedItem.Text == "State" || ddlENmTP.SelectedItem.Text == "City" || ddlENmTP.SelectedItem.Text == "Incoterms Sepc")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "call me",
                            "CheckParent('ddlENmTP', 'enmPrnt', 'enmPrntddl');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "cal me",
                            "UpdateDropDownList('ddlENmTP', 'EnumsMaster.Aspx', 'ddlEnmPrnt', '" + valu + "');", true);
                    }
                    else
                        hiddenddlEnmPrnt.Value = Guid.Empty.ToString();
                    ViewState["EnumMstrID"] = ((Label)gvrow.FindControl("lblEnmMsterID")).Text;
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    Guid vl = new Guid(((Label)gvrow.FindControl("lblEnmMsterID")).Text);
                    if (ViewState["EnumMstrID"] != null && new Guid(ViewState["EnumMstrID"].ToString()) != vl)
                    {
                        string EnumDesc = txtEnmDesrp.Text.Trim();
                        Guid EnumTYpeID = new Guid(ddlENmTP.SelectedItem.Value.ToString());
                        res = EMBL.EnumMasterInsert(CommonBLL.FlagDelete, vl, EnumTYpeID, EnumDesc, Guid.Empty, Guid.Empty);
                        btnSave.Text = "Save";
                        BindGridView(gvEnumMaster, EMBL.EnumMasterSelectforDescription('S', Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
                        if (res == 0)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Deleted Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Master",
                                "Data Deleted successfully in Enum Master.");
                            ClearAllInputs();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Value is Used by Another Transaction');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master",
                                "Value is Used by Another Transaction " + vl + ".");
                        }
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('You cannot Delete, when the same row is Editing.');", true);
                }
                gvrow.FindControl("lblDescription").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Enum Type Grid View Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnmTp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvEnmTp.Rows[index];
                if (e.CommandName == "Modify")
                {
                    txtEnmTp.Text = ((Label)gvrow.FindControl("lblEnmTPNm")).Text;
                    ViewState["EnumTpID"] = ((Label)gvrow.FindControl("lblID")).Text;
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    Guid uid = new Guid(((Label)gvrow.FindControl("lblID")).Text);
                    if (ViewState["EnumTpID"] != null && new Guid(ViewState["EnumTpID"].ToString()) != uid)
                    {
                        res = EMBL.EnumTypeInsert(CommonBLL.FlagDelete, txtEnmTp.Text, uid, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), false);
                        btnSave.Text = "Save";
                        BindGridView(gvEnmTp, EMBL.EnumTypeBind());
                        if (res == 0)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Deleted Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Type Master",
                                "Data Deleted successfully in Enum Type Master.");
                            ClearAllInputs();
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Value is Used by Another Transaction');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Type Master",
                                "Value is Used by Another Transaction " + uid + ".");
                        }
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('You cannot Delete, when the same row is Editing.');", true);
                }
                gvrow.FindControl("lblEnmTPNm").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Enum Master Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnumMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            int lastCellIndex = e.Row.Cells.Count - 1;
            ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
        }

        /// <summary>
        /// Enum Type Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvEnmTp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            int lastCellIndex = e.Row.Cells.Count - 1;
            ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
            Guid EditID = new Guid(((Label)e.Row.FindControl("lblID")).Text);
            if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
                EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";

            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
                deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            //deleteButton.OnClientClick = "javascript:return deleteID('" + EditID + "')";
            else
                deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
        }

        #endregion

        # region Services

        /// <summary>
        /// This is web service for autocomplete
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<string> GetSubOrMailIDs(string SearchString)
        {
            List<string> result = new List<string>();
            try
            {
                DataSet ds = new DataSet();
                BAL.EnumMasterBLL EMBL = new EnumMasterBLL();
                ds = EMBL.SearchSuggestion(CommonBLL.FlagASelect, 0);
                if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        result.Add(ds.Tables[0].Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
            }
            return result;
        }

        # endregion

        #region Save, Update and Clear Button Click Events

        /// <summary>
        /// This is to save the EnumType In enumtype table using business login layer
        /// </summary>
        /// <param name="enumName">name of the enum</param>
        /// <param name="createdBy">id createdBy</param>
        /// 
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (txtEnmTp.Text.Length > 0 && txtEnmTp.Text.Trim() != "" && rbgnenmtp.Checked)
                {
                    if (btnSave.Text == "Save")
                    {
                        res = EMBL.EnumTypeInsert(CommonBLL.FlagNewInsert, txtEnmTp.Text, Guid.Empty, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), true);
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Type Master",
                                "Data Inserted successfully in Enum Type Master.");
                            ClearAllInputs();
                            GetData();
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error While Saving');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Type Master", "Error While Saving.");
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        res = EMBL.EnumTypeInsert(CommonBLL.FlagUpdate, txtEnmTp.Text, new Guid(ViewState["EnumTpID"].ToString()), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()), true);
                        btnSave.Text = "Save";
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Type Master",
                                "Data Updated successfully in Enum Type Master.");
                            ClearAllInputs();
                            GetData();
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error While Updating');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Type Master", "Error While Updating.");
                        }
                    }
                }
                else if (rbtnenmdescs.Checked)
                {
                    if (btnSave.Text == "Save")
                    {
                        Guid EnumTYpeID = new Guid(ddlENmTP.SelectedItem.Value);
                        string EnumDesc = txtEnmDesrp.Text.Trim();
                        string ParentID = hiddenddlEnmPrnt.Value == "0" ? Guid.Empty.ToString() : hiddenddlEnmPrnt.Value;
                        Guid ParentItem = new Guid(ParentID);
                        res = EMBL.EnumMasterInsert(CommonBLL.FlagNewInsert, Guid.Empty, EnumTYpeID, EnumDesc, ParentItem, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Master",
                                "Data Inserted successfully in Enum Master.");
                            ClearAllInputs();
                            GetData();
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error While Saving');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", "Error While Saving.");
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        Guid EnumTYpeID = new Guid(ddlENmTP.SelectedItem.Value.ToString());
                        string EnumDesc = txtEnmDesrp.Text.Trim();
                        Guid vl = new Guid(ViewState["EnumMstrID"].ToString());
                        Guid ParentItem = new Guid(hiddenddlEnmPrnt.Value);
                        res = EMBL.EnumMasterInsert(CommonBLL.FlagUpdate, vl, EnumTYpeID, EnumDesc, ParentItem, new Guid(Session["CompanyID"].ToString()));
                        btnSave.Text = "Save";
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ViewState["EnumTpID"] = null;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Enum Master",
                                "Data Updated successfully in Enum Master.");
                            ClearAllInputs();
                            GetData();
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Enum Type Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error While Updating');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", "Error While Updating.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enum Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Buttn Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAllInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"),
                    "Enum Master & Enum Type(ClearButtonClick)", ex.Message.ToString());
            }
        }

        #endregion
    }
}
