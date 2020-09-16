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
using System.Text;
using VOMS_ERP.Admin;
using System.IO;


namespace VOMS_ERP.Masters
{
    public partial class Contact : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        CommonBLL CmnBLL = new CommonBLL();
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
                if (Session["UserID"] == null || (Session["UserID"]).ToString() == "")
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
                            //savenew.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                        {
                            btnSave.Enabled = false;
                            btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                            //savenew.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
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

        private void GetData()
        {
            try
            {
                ClearItems();
                GetContactTypes();
                GetCustomerNames();
                GetDesignations();
                GetDepartments();
                GetRolesDRP();
                GetCompanyDRP();
                BindGridView();

                if (CommonBLL.SuperAdminRole == (Session["AccessRole"].ToString()))
                {
                    lblCmpnNm.Visible = true;
                    ddlCmpnyNm.Visible = true;
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        private void GetCompanyDRP()
        {
            try
            {
                DataSet ds = new DataSet();
                CompanyBLL CDBL = new CompanyBLL();
                ds = CDBL.SelectCompany(CommonBLL.FlagRegularDRP, Guid.Empty);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlCmpnyNm.DataSource = ds;
                    ddlCmpnyNm.DataTextField = "Description";
                    ddlCmpnyNm.DataValueField = "ID";
                    ddlCmpnyNm.DataBind();
                }
                ddlCmpnyNm.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
                ddlCmpnyNm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }

        private void GetCustomerNames()
        {
            try
            {
                lbcustomer.Items.Clear();
                CustomerBLL CBLL = new CustomerBLL();
                DataSet ds = new DataSet();
                ds = CBLL.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    lbcustomer.DataSource = ds.Tables[0];
                    lbcustomer.DataTextField = "Description";
                    lbcustomer.DataValueField = "ID";
                    lbcustomer.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Re-Set Data Table
        /// </summary>
        private void ResetDataTablesPlugin()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("<script type='text/javascript'>");
                builder.Append("fnClickRedraw();");
                builder.Append("</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "fnClickRedraw", builder.ToString(), false);

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Save Record
        /// </summary>
        private void SaveRecord()
        {
            try
            {
                Guid DesignationID = new Guid(ddlDesignationID.SelectedItem.Value);
                Guid DeptID = new Guid(ddldept.SelectedItem.Value);
                string ContactType = ddlcncttp.SelectedItem.Text;
                string Custmr_SuplrID = "", AccessType = "";
                ContactBLL ctbll = new ContactBLL();
                if (ddlcncttp.SelectedItem.Text.ToString() == CommonBLL.CustmrContactTypeText)
                    Custmr_SuplrID = String.Join(",", lbcustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray()); ;
                AccessType = ddlrole.SelectedItem.Text;
                if (ddlcncttp.SelectedItem.Text.ToString() == CommonBLL.SuplrContactTypeText)
                {
                    Custmr_SuplrID = ddlrole.SelectedValue;
                    AccessType = "Supplier";
                }
                if (ddlcncttp.SelectedItem.Text == CommonBLL.TraffickerContactTypeText)
                    AccessType = ddlrole.SelectedItem.Text;
                Guid CMPNYname = (CommonBLL.SuperAdminRole == Session["AccessRole"].ToString() ? new Guid(ddlCmpnyNm.SelectedValue) : new Guid(Session["CompanyId"].ToString()));
                int res = 0;
                if (btnSave.Text == "Save" && ViewState["EditID"] == null)
                    res = ctbll.ContacstInsert(CommonBLL.FlagNewInsert, Guid.Empty, txtfname.Text.Trim(), txtlname.Text.Trim(),
                        txtalsNm.Text.Trim(), DesignationID, DeptID, txtfx.Text.Trim(), txtmbl.Text.Trim(), txttpn.Text.Trim(),
                        txtpem.Text.Trim(), txtsem.Text.Trim(), ContactType, new Guid(Session["UserID"].ToString()),
                       CMPNYname, Custmr_SuplrID, AccessType);
                else if (btnSave.Text == "Update" && ViewState["EditID"] != null)
                    res = ctbll.ContacstUpdate(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtfname.Text.Trim(), txtlname.Text.Trim(), txtalsNm.Text.Trim(), DesignationID,
                        DeptID, txtfx.Text.Trim(), txtmbl.Text.Trim(), txttpn.Text.Trim(), txtpem.Text.Trim(),
                        txtsem.Text.Trim(), ContactType, true, new Guid(Session["UserID"].ToString()), new Guid(ddlCmpnyNm.SelectedValue), Custmr_SuplrID, AccessType);
                if (res == 0 && btnSave.Text == "Save")
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('Saved Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Contact Master",
                        "Data inserted successfully in Contact Master.");
                    ClearItems();
                }
                else if (res != 0 && btnSave.Text == "Save")
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", "Getting Error.");
                }
                else if (res == 0 && btnSave.Text == "Update")
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('Updated Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Contact Master",
                        "Data Updated successfully in Contact Master.");
                    ViewState["EditID"] = null;
                }
                else if (res != 0 && btnSave.Text == "Update")
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Updating.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", "Getting Error while Updating.");
                }
                BindGridView();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to Clear Text in TextBoxes and For DropDownLists selected Index as 0
        /// </summary>
        private void ClearItems()
        {
            try
            {
                txtfname.Text = "";
                txtlname.Text = "";
                txtalsNm.Text = "";
                txtfx.Text = "";
                txtmbl.Text = "";
                txttpn.Text = "";
                txtpem.Text = "";
                txtsem.Text = "";
                ddlDesignationID.SelectedIndex = 0;
                ddldept.SelectedIndex = 0;
                ddlcncttp.SelectedIndex = 0;
                ddlrole.SelectedIndex = 0;
                btnSave.Text = "Save";
                lblRoleType.Visible = true;
                lbcustomer.Items.Clear();
                lbcustomer.Visible = false;
                ddlrole.Enabled = true;
                ddlCmpnyNm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Meathod is used to Bind Designation of the user from EnumMasters Table
        /// </summary>
        private void GetDesignations()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL emb = new EnumMasterBLL();
                ds = emb.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Designation);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlDesignationID.DataSource = ds;
                    ddlDesignationID.DataTextField = "Description";
                    ddlDesignationID.DataValueField = "ID";
                    ddlDesignationID.DataBind();
                }
                ddlDesignationID.Items.Insert(0, new ListItem("--Select Designation--", Guid.Empty.ToString()));
                ddlDesignationID.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Meeathod is used to bind Departments from EnumsMaster Table
        /// </summary>
        private void GetDepartments()
        {
            try
            {
                //DataSet ds = CmnBLL.GetAllDepartments("ID", "Description", "EnumMaster");
                DataSet ds = new DataSet();
                EnumMasterBLL emb = new EnumMasterBLL();
                ds = emb.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddldept.DataSource = ds;
                    ddldept.DataTextField = "Description";
                    ddldept.DataValueField = "ID";
                    ddldept.DataBind();
                }
                ddldept.Items.Insert(0, new ListItem("--Select Department--", Guid.Empty.ToString()));
                ddldept.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind ContactType
        /// </summary>
        private void GetContactTypes()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL emb = new EnumMasterBLL();
                ds = emb.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContactType);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlcncttp.DataSource = ds;
                    ddlcncttp.DataTextField = "Description";
                    ddlcncttp.DataValueField = "ID";
                    ddlcncttp.DataBind();
                }
                ddlcncttp.Items.Insert(0, new ListItem("--Select ContactType--", Guid.Empty.ToString()));
                ddlcncttp.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
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
                ds = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContactType);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlcncttp.DataSource = ds;
                    ddlcncttp.DataTextField = "Description";
                    ddlcncttp.DataValueField = "ID";
                    ddlcncttp.DataBind();
                }
                ddlcncttp.Items.Insert(0, new ListItem("-- Select ContactType--", Guid.Empty.ToString()));
                ddlcncttp.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Meathod is used to Bind RoleType
        /// </summary>
        private void GetRoleType()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL emb = new EnumMasterBLL();
                ds = emb.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Role);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlrole.DataSource = ds;
                    ddlrole.DataTextField = "Description";
                    ddlrole.DataValueField = "ID";
                    ddlrole.DataBind();
                }
                ddlrole.Items.Insert(0, new ListItem("--Select Role--", Guid.Empty.ToString()));
                ddlrole.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// When GridView Edit Button is Pressed Control will be binded according to that ID
        /// </summary>
        /// <param name="ID"></param>
        private void GridEdit(Guid ID, Guid CompanyId)
        {
            try
            {
                DataSet dsg = new DataSet();
                ContactBLL cbll = new ContactBLL();
                dsg = cbll.ContactsBindGridWithGuid(CommonBLL.FlagModify, ID, CompanyId);
                if (dsg.Tables.Count > 0 && dsg.Tables[0].Rows.Count > 0)
                {
                    txtfname.Text = dsg.Tables[0].Rows[0]["FName"].ToString();
                    txtlname.Text = dsg.Tables[0].Rows[0]["LName"].ToString();
                    txtalsNm.Text = dsg.Tables[0].Rows[0]["AliasName"].ToString();
                    ddlDesignationID.SelectedValue = dsg.Tables[0].Rows[0]["DesignationID"].ToString() == "" ? Guid.Empty.ToString() : dsg.Tables[0].Rows[0]["DesignationID"].ToString();
                    ddldept.SelectedValue = dsg.Tables[0].Rows[0]["DepartmentID"].ToString() == "" ? Guid.Empty.ToString() : dsg.Tables[0].Rows[0]["DepartmentID"].ToString();
                    txtfx.Text = dsg.Tables[0].Rows[0]["FaxNo"].ToString();
                    txtmbl.Text = dsg.Tables[0].Rows[0]["MobileNo"].ToString();
                    txttpn.Text = dsg.Tables[0].Rows[0]["Phone"].ToString();
                    txtpem.Text = dsg.Tables[0].Rows[0]["PriEmail"].ToString();
                    txtsem.Text = dsg.Tables[0].Rows[0]["SecEmail"].ToString();
                    ddlcncttp.SelectedValue = (ddlcncttp.Items.FindByText(dsg.Tables[0].Rows[0]["ContactType"].ToString())).Value;
                    ContactTypeBind();
                    if (ddlcncttp.SelectedItem.Text == "Trafficker")
                    {
                        if (dsg.Tables[0].Rows[0]["AccessRole"].ToString() != "")
                            ddlrole.SelectedValue = (ddlrole.Items.FindByText(dsg.Tables[0].Rows[0]["AccessRole"].ToString())).Value;
                        ddlCmpnyNm.SelectedValue = dsg.Tables[0].Rows[0]["CompanyId"].ToString();
                    }
                    else if (ddlcncttp.SelectedItem.Text == "Supplier")
                    {
                        ddlrole.SelectedValue = dsg.Tables[0].Rows[0]["Custmr_SuplrID"].ToString();
                        ddlCmpnyNm.SelectedValue = dsg.Tables[0].Rows[0]["CompanyId"].ToString();
                    }
                    else if (ddlcncttp.SelectedItem.Text == "Customer")
                    {
                        GetRoleType();
                        ddlrole.SelectedValue = (ddlrole.Items.FindByText(dsg.Tables[0].Rows[0]["AccessRole"].ToString())).Value;
                        ddlCmpnyNm.SelectedValue = dsg.Tables[0].Rows[0]["CompanyId"].ToString();
                        DataSet dsd = new DataSet();
                        lbcustomer.Items.Clear();
                        lblCustomer.Visible = true;
                        lbcustomer.Visible = true;
                        CustomerBLL cbl = new CustomerBLL();
                        dsd = cbl.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlCmpnyNm.SelectedValue));
                        if (dsd.Tables.Count > 0 && dsd.Tables[0].Rows.Count > 0)
                        {
                            lbcustomer.DataSource = dsd;
                            lbcustomer.DataTextField = "Description";
                            lbcustomer.DataValueField = "ID";
                            lbcustomer.DataBind();
                        }
                        
                        lblRoleType.Visible = true;
                        string[] RoleIDs = (dsg.Tables[0].Rows[0]["Custmr_SuplrID"].ToString()).Split(',');
                        foreach (ListItem item in lbcustomer.Items)
                        {
                            foreach (string s in RoleIDs)
                            {
                                if (item.Value.Trim().ToLower() == s.Trim().ToLower())
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }

                    else
                    {
                        string[] RoleIDs = (dsg.Tables[0].Rows[0]["Custmr_SuplrID"].ToString()).Split(',');
                        foreach (ListItem item in lbcustomer.Items)
                        {
                            foreach (string s in RoleIDs)
                            {
                                if (item.Value == s)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }
                    ViewState["ID"] = ID;
                    btnSave.Text = "Update";
                    //savenew.Text = "Update & New";
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('invalid ID');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is use dto Bind Gridview when inserted / Updated / pageload
        /// </summary>
        protected void BindGridView()
        {
            try
            {
                DataSet ds = new DataSet();
                ContactBLL cbll = new ContactBLL();
                ds = cbll.ContactsBindGridWithGuid(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    gvcontact.DataSource = ds;
                    gvcontact.DataBind();
                }
                //else
                //    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind the roles and types accroding to selected values for contact Type DDL
        /// </summary>
        private void ContactTypeBind()
        {
            try
            {
                DataSet dsc = new DataSet();
                if (ddlcncttp.SelectedValue.ToString() != Guid.Empty.ToString())
                {
                    if (ddlcncttp.SelectedItem.Text == CommonBLL.TraffickerContactTypeText)
                    {
                        //lblRoleType.Visible = true;
                        GetRoleType();
                        lblRoleType.Visible = true;
                        ddlrole.Visible = true;
                        lbcustomer.Visible = false;
                        lblCustomer.Visible = false;
                        ddlrole.Enabled = true;
                        lblCompany.Visible = false;
                    }
                    else if (ddlcncttp.SelectedItem.Text == CommonBLL.CustmrContactTypeText)
                    {
                        GetCompanyDRP();
                        if (CommonBLL.SuperAdminRole != (Session["AccessRole"].ToString()))
                        {
                            lbcustomer.Items.Clear();
                            lblCustomer.Visible = true;
                            lbcustomer.Visible = true;
                            CustomerBLL cbll = new CustomerBLL();
                            dsc = cbll.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                            if (dsc.Tables.Count > 0 && dsc.Tables[0].Rows.Count > 0)
                            {
                                lbcustomer.DataSource = dsc;
                                lbcustomer.DataTextField = "Description";
                                lbcustomer.DataValueField = "ID";
                                lbcustomer.DataBind();
                            }
                        }
                        GetRoleType();
                        lblRoleType.Visible = true;
                        ddlrole.SelectedValue = (ddlrole.Items.FindByText("Customer")).Value; //"234";  //ddlcncttp.SelectedItem.Text;
                        ddlrole.Enabled = false;
                        ddlrole.Visible = true;
                        lblCompany.Visible = false;

                    }
                    else if (ddlcncttp.SelectedItem.Text == CommonBLL.SuplrContactTypeText)
                    {
                        //lblRoleType.Text = "Company Name*:";
                        lblCompany.Visible = true;
                        ddlrole.Items.Clear();
                        lblRoleType.Visible = false;
                        ddlrole.Visible = true;
                        ddlrole.Enabled = true;
                        lbcustomer.Visible = false;
                        lblCustomer.Visible = false;
                        SupplierBLL sbll = new SupplierBLL();
                        dsc = sbll.SelectSuppliersForBind(CommonBLL.FlagODRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (dsc.Tables.Count > 0 && dsc.Tables[0].Rows.Count > 0)
                        {
                            ddlrole.DataSource = dsc;
                            ddlrole.DataTextField = "Description";
                            ddlrole.DataValueField = "ID";
                            ddlrole.DataBind();
                        }
                        ddlrole.Items.Insert(0, new ListItem("-- Select Supplier --", Guid.Empty.ToString()));
                        ddlrole.SelectedIndex = 0;
                    }
                }
                else
                {
                    ddlrole.Items.Clear();
                    ddlrole.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
                    ddlrole.SelectedIndex = 0;
                    lblRoleType.Visible = true;
                    lbcustomer.Visible = false;
                    lblCustomer.Visible = false;
                    ddlCmpnyNm.SelectedValue = Guid.Empty.ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("FullName");
                dt.Columns.Add("Department");
                dt.Columns.Add("Contact");
                dt.Columns.Add("MailID");
                dt.Columns.Add("");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvcontact.DataSource = ds;
                gvcontact.DataBind();
                int columncount = gvcontact.Rows[0].Cells.Count;
                gvcontact.Rows[0].Cells.Clear();
                gvcontact.Rows[0].Cells.Add(new TableCell());
                gvcontact.Rows[0].Cells[0].ColumnSpan = columncount;
                gvcontact.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }
        # endregion

        # region GridView Events

        /// <summary>
        /// This event is used to Edit Nnd Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcontact_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvcontact.Rows[index];
                string lblId = ((Label)row.FindControl("lblSNO")).Text.ToString();
                if (e.CommandName.Equals("Modify"))
                {
                    ViewState["EditID"] = lblId;
                    GridEdit(new Guid(lblId), new Guid(Session["CompanyID"].ToString()));
                }
                else if (e.CommandName.Equals("Delete"))
                {
                    ContactBLL cbll = new ContactBLL();
                    int res = cbll.ContactsDeleteWithGuid(CommonBLL.FlagDelete, new Guid(lblId));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Record Deleted successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", "Deleted Successfully.");
                        ClearItems();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Error while Deleting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master",
                            "Getting Error while Deleting.");
                    }
                }
                BindGridView();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This not in use But IT should be mentioned for not getting Error While Deleting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcontact_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //try
            //{
            //    int index = Convert.ToInt32(e.RowIndex);
            //    GridViewRow row = gvcontact.Rows[index];
            //    string lblId = ((Label)row.FindControl("lblSNO")).Text.ToString();
            //    ContactBLL cbll = new ContactBLL();
            //    int res = cbll.ContactsDelete(Convert.ToChar("D"), Convert.ToInt32(lblId));
            //    if (res == 0)
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage();", true);
            //    else
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage();", true);
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //}
        }

        /// <summary>
        /// This GridView Event is used for Delete Conformation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcontact_RowDataBound(object sender, GridViewRowEventArgs e)
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

                //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                //deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcontact_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvcontact.UseAccessibleHeader = false;
                gvcontact.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }
        # endregion

        # region DDL Selected Index

        /// <summary>
        /// This Event is used to bind the roles and types accroding to selected values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcncttp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ContactTypeBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClick

        /// <summary>
        /// This Event is used to Save Recors Ones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if ((ddlrole.SelectedItem.Text == "Customer" || lbcustomer.Visible == true) && lbcustomer.GetSelectedIndices().Count() == 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select atleast One Customer.');", true);
                else
                {
                    SaveRecord();
                    ClearItems();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This event is used to clear the textboxes and dropdownlists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["IsEdit"] = "0";
                ClearItems();
                BindGridView();
                ResetDataTablesPlugin();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMeathods
        /// <summary>
        /// This is used to check Mial-ID
        /// </summary>
        /// <param name="MailID">mailID</param>
        /// <returns></returns>
        [WebMethod]
        public static string CheckMailID(string MailID)
        {
            string res = "";
            try
            {
                DataSet ds = new DataSet();
                CheckBLL CBLL = new CheckBLL();
                ds = CBLL.CheckMail(Convert.ToChar("M"), MailID, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString().ToLower() == MailID.ToLower())
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

        /// <summary>
        /// This is used to Check FirstName
        /// </summary>
        /// <param name="FirstName"></param>
        /// <returns></returns>
        [WebMethod]
        public static string CheckFirstName(string FirstName)
        {
            string res = "True";
            try
            {
                //DataSet ds = new DataSet();
                //CheckBLL CBLL = new CheckBLL();
                //ds = CBLL.CheckMail(Convert.ToChar("F"), FirstName);
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    if (ds.Tables[0].Rows[0][0].ToString().ToLower() == FirstName.ToLower())
                //        res = "False";                    
                //}
                //else
                return "True";
            }
            catch (Exception ex)
            {
                string Errmsg = ex.Message;
                res = "True";
            }
            return res;
        }

        /// <summary>
        /// This is used to check LastName
        /// </summary>
        /// <param name="LastName"></param>
        /// <returns></returns>
        [WebMethod]
        public static string CheckLastName(string LastName)
        {
            string res = "";
            try
            {
                DataSet ds = new DataSet();
                CheckBLL CBLL = new CheckBLL();
                ds = CBLL.CheckMail(CommonBLL.FlagLSelect, LastName, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString().ToLower() == LastName.ToLower())
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

        [WebMethod]
        public static string CheckAlias(string AliasName)
        {
            string res = "";
            try
            {
                DataSet ds = new DataSet();
                CheckBLL CBLL = new CheckBLL();
                ds = CBLL.CheckMail(CommonBLL.FlagDelete, AliasName, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString().ToLower() == AliasName.ToLower())
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

        protected void ddlCmpnyNm_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet d = new DataSet();
            lbcustomer.Items.Clear();
            lblCustomer.Visible = true;
            lbcustomer.Visible = true;
            CustomerBLL cbll = new CustomerBLL();
            d = cbll.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlCmpnyNm.SelectedValue));
            if (d.Tables.Count > 0 && d.Tables[0].Rows.Count > 0)
            {
                lbcustomer.DataSource = d;
                lbcustomer.DataTextField = "Description";
                lbcustomer.DataValueField = "ID";
                lbcustomer.DataBind();
            }
            GetRoleType();
            lblRoleType.Visible = true;
            ddlrole.SelectedValue = (ddlrole.Items.FindByText("Customer")).Value; //"234";  //ddlcncttp.SelectedItem.Text;
            ddlrole.Enabled = false;
            ddlrole.Visible = true;
            lblCompany.Visible = false;
        }
    }
}
