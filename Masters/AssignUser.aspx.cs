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
using System.Security.Cryptography;
using System.Text;
using BAL;
using System.Net.Mail;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class AssignUser : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        ContactBLL CBLL = new ContactBLL();
        SupplierBLL SPBLL = new SupplierBLL();
        CustomerBLL CMBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        AssignUserBLL AUBLL = new AssignUserBLL();
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
                    Response.Redirect("login.aspx", false);
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
                        //btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
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
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                BindDropDownList(ddlCtgry, null, (EMBLL.GetEnumTypesWithGuid(CommonBLL.FlagRegularDRP, CommonBLL.ContactType, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContactType)).Tables[0]);
                BindDropDownList(ddlDsgntn, null, (EMBLL.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Designation)).Tables[0]);
                BindDropDownList(ddlRole, null, (EMBLL.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Role)).Tables[0]);
                GetDataForGridView(AUBLL.GetAssignedUsersGrid(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                ddlRole.Visible = lblRoleType.Visible = true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to bind the roles and types accroding to selected values for contact Type DDL
        /// </summary>
        private void ContactTypeBind()
        {
            try
            {
                if (ddlCtgry.SelectedValue.ToString() != Guid.Empty.ToString())
                {
                    Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                    if (ddlCtgry.SelectedItem.Text == CommonBLL.TraffickerContactTypeText)
                    {
                        //lblRole.Visible = ddlRole.Visible = true;
                        LBCustomer.Visible = lblCustomer.Visible = false;
                        lblRoleType.Visible = ddlRole.Visible = true;
                        BindDropDownList(ddlRole, null, (EMBLL.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, CompanyID, CommonBLL.Role)).Tables[0]);
                    }
                    else if (ddlCtgry.SelectedItem.Text == CommonBLL.CustmrContactTypeText)
                    {
                        lblRoleType.Visible = ddlRole.Visible = false;
                        LBCustomer.Visible = lblCustomer.Visible = true;
                        LBCustomer.Enabled = false;
                        BindDropDownList(ddlRole, null, (EMBLL.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, CompanyID, CommonBLL.Role)).Tables[0]);
                        //ddlRole.SelectedItem.Text = "Customer";
                        BindDropDownList(null, LBCustomer, (CMBLL.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[0]));
                    }
                    else if (ddlCtgry.SelectedItem.Text == CommonBLL.SupplierCategory.ToString())
                    {
                        //lblCustomer.Text = "Company Name*:";
                        //lblRole.Text = "Type:";
                        LBCustomer.Visible = lblRoleType.Visible = false;
                        lblCustomer.Visible = ddlRole.Visible = true;
                        //BindDropDownList(ddlRole, (CMBLL.SelectCustomers(CommonBLL.FlagRegularDRP, 0)).Tables[0]);
                        BindDropDownList(ddlRole, null, (SPBLL.SelectSuppliersForBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);

                    }
                }
                else
                {
                    ddlRole.Items.Clear();
                    ddlRole.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
                    ddlRole.SelectedIndex = 0;
                    lblCustomer.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Assign User", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Bind GridView
        /// </summary>
        private void GetDataForGridView(DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    gvUsers.DataSource = ds;
                    gvUsers.DataBind();
                }
                //else if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                //    gvUsers.DataSource = ds;
                //    gvUsers.DataBind();
                //    int columncount = gvUsers.Rows[0].Cells.Count;
                //    gvUsers.Rows[0].Cells.Clear();
                //    gvUsers.Rows[0].Cells.Add(new TableCell());
                //    gvUsers.Rows[0].Cells[0].ColumnSpan = columncount;
                //    gvUsers.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
                //}
                //else
                //    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Save New Record And Update
        /// </summary>
        private void SaveRecord()
        {
            try
            {
                int res = 0;
                int res1 = 0; Filename = FileName();
                if (ddlCtgry.SelectedIndex != 0 && ddlUsers.SelectedIndex != 0 && txtPwd.Text.Trim() != "" && txtCfmPwd.Text.Trim() != "")
                {
                    if (txtPwd.Text == txtCfmPwd.Text)
                    {
                        Guid UserID = new Guid(ddlUsers.SelectedValue);
                        string MailID = ddlUsers.SelectedItem.Text;
                        string pwd = MD5(txtPwd.Text.Trim());
                        string contactType = ddlCtgry.SelectedItem.Text;
                        Guid desID = new Guid(ddlDsgntn.SelectedValue);
                        Guid RoleID = new Guid(ddlRole.SelectedValue);
                        bool changePwd = false;
                        if (ddlCtgry.SelectedItem.Text == CommonBLL.SuplrContactTypeText)
                            changePwd = true;
                        else
                            changePwd = Convert.ToBoolean(hdnfldCngPwd.Value);

                        if (btnSave.Text == "Save" && ViewState["EditID"] == null)
                        {
                            res = AUBLL.AssignedUsersInsert(CommonBLL.FlagNewInsert, Guid.Empty, UserID, MailID, pwd, Guid.Empty, "", Guid.Empty, "",
                                changePwd, new Guid(Session["UserID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                            // res1 = CBLL.ContacstUpdate(CommonBLL.FlagCSelect, Convert.ToInt32(ViewState["EditUserID"]), "", "", "",
                            // desID, 0, "", "", "", "", "", contactType, RoleID, true, Convert.ToInt32(Session["UserID"]), "", 0);
                        }
                        else if (btnSave.Text == "Update" && ViewState["EditID"] != null)
                        {
                            res = AUBLL.AssignedUsersInsert(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), UserID,
                                MailID, pwd, Guid.Empty, "", Guid.Empty, "", changePwd, Guid.Empty, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                            //res1 = CBLL.ContacstUpdate(CommonBLL.FlagCSelect, Convert.ToInt32(ViewState["EditUserID"]), "", "", "",
                            //desID, 0, "", "", "", "", "", contactType, RoleID, true, Convert.ToInt32(Session["UserID"]), "", 0);
                        }
                        if (res == 0 && res1 == 0 && btnSave.Text == "Save")
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            SendMail(txtPwd.Text.Trim());
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Contact Master",
                                "Data inserted successfully in Contact Master.");
                            ClearItems();
                        }
                        else if (res != 0 && res1 != 0 && btnSave.Text == "Save")
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", "Getting Error.");
                        }
                        else if (res == 0 && res1 == 0 && btnSave.Text == "Update")
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            SendMail(txtPwd.Text.Trim());
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Contact Master",
                                "Data Updated successfully in Contact Master.");
                            ViewState["EditID"] = null;
                            ViewState["EditUserID"] = null;

                        }
                        else if (res != 0 && res1 != 0 && btnSave.Text == "Update")
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Contact Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", "Getting Error while Updating.");
                        }
                        GetDataForGridView(AUBLL.GetAssignedUsersGrid(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Enter Details Properly.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This method is used to Edit record For a perticular ID
        /// </summary>
        /// <param name="ID"></param>
        private void EditUser(Guid ID)
        {
            try
            {

                DataSet ds = AUBLL.GetAssignedUsersEdit(CommonBLL.FlagModify, ID, Guid.Empty);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ViewState["EditUserID"] = ds.Tables[0].Rows[0]["userID"].ToString();
                    ddlCtgry.SelectedValue = ds.Tables[0].Rows[0]["ContactType"].ToString() == "" ? Guid.Empty.ToString() : ddlCtgry.Items.FindByText(ds.Tables[0].Rows[0]["ContactType"].ToString()).Value;
                    ContactTypeBind();
                    BindDropDownList(ddlUsers, null, (CBLL.GetusersOnCtype(CommonBLL.FlagLSelect,
                        ds.Tables[0].Rows[0]["ContactType"].ToString(), new Guid(Session["CompanyID"].ToString())).Tables[0]));
                    ddlUsers.SelectedValue = ds.Tables[0].Rows[0]["userID"].ToString().Trim().ToLower();
                    BindDropDownList(ddlDsgntn, null, (EMBLL.GetEnumTypesWithGuid(CommonBLL.FlagRegularDRP, CommonBLL.Designation, new Guid(Session["CompanyID"].ToString()), CommonBLL.Designation)).Tables[0]);
                    ddlDsgntn.SelectedValue = ds.Tables[0].Rows[0]["DesignationID"].ToString() == "" ? Guid.Empty.ToString() : ds.Tables[0].Rows[0]["DesignationID"].ToString();
                    BindDropDownList(ddlRole, null, (EMBLL.GetEnumTypes(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Role)).Tables[0]);
                    if (ddlCtgry.SelectedItem.Text == CommonBLL.TraffickerContactTypeText)
                    {
                        ddlRole.SelectedValue = ddlRole.Items.FindByText(ds.Tables[0].Rows[0]["AccessRole"].ToString()).Value;
                        lblRoleType.Visible = ddlRole.Visible = true;
                    }
                    hdnfldCngPwd.Value = ds.Tables[0].Rows[0]["IsPwdChanged"].ToString();
                    if (ddlCtgry.SelectedItem.Text == CommonBLL.CustmrContactTypeText)
                    {
                        string[] RoleIDs = (ds.Tables[0].Rows[0]["Custmr_SuplrID"].ToString()).Split(',');
                        foreach (ListItem item in LBCustomer.Items)
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
                        ddlRole.SelectedValue = ddlRole.Items.FindByText(ds.Tables[0].Rows[0]["AccessRole"].ToString()).Value;
                    }
                    if (Convert.ToBoolean(hdnfldCngPwd.Value))
                    {
                        txtPwd.Text = ds.Tables[0].Rows[0]["Password"].ToString();
                        txtCfmPwd.ToolTip = txtPwd.ToolTip = ds.Tables[0].Rows[0]["Password"].ToString();
                        txtCfmPwd.Text = ds.Tables[0].Rows[0]["Password"].ToString();
                        //txtPwd.Enabled = txtCfmPwd.Enabled = false;
                    }
                    else
                        txtPwd.Enabled = txtCfmPwd.Enabled = true;



                    lblDsgntn.Text = ddlDsgntn.SelectedItem.Text; ddlDsgntn.Visible = false;
                    //lblRole.Text = ddlRole.SelectedItem.Text; 
                    lblDsgntn.Visible = true;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Clear Controls
        /// </summary>
        private void ClearItems()
        {
            txtPwd.Text = txtCfmPwd.Text = lblDsgntn.Text = "";
            ddlCtgry.SelectedIndex = ddlRole.SelectedIndex = -1;
            ddlUsers.SelectedIndex = ddlDsgntn.SelectedIndex = -1;
            btnSave.Text = "Save";
            ViewState["EditID"] = null;
            ViewState["EditUserID"] = null;
            ddlRole.Visible = ddlDsgntn.Visible = true;
            lblDsgntn.Visible = lblCustomer.Visible = false;
            lblRoleType.Visible = true;
            LBCustomer.Items.Clear();
            LBCustomer.Visible = false;
        }

        /// <summary>
        /// This Method is used to Encrypr PWD
        /// </summary>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        public string MD5(string sPassword)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(sPassword);
                bs = x.ComputeHash(bs);

                foreach (byte b in bs)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
                return s.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// This Method is used to Get Users On the Basis of Contact Type
        /// </summary>
        /// <param name="contactTypeID"></param>
        private void GetusersOnCType(string contactTypeID, Guid CompanyID)
        {
            try
            {
                ddlUsers.Items.Clear();
                DataSet ds = new DataSet();
                ds = CBLL.GetusersOnCtype(CommonBLL.FlagRegularDRP, contactTypeID, CompanyID);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlUsers.Items.Clear();
                    ddlUsers.DataSource = ds;
                    ddlUsers.DataTextField = "PMail";
                    ddlUsers.DataValueField = "ID";
                    ddlUsers.DataBind();
                }
                if (contactTypeID == "Trafficker")
                    ddlUsers.Items.Insert(0, new ListItem("--Select UserMail-ID--", Guid.Empty.ToString()));
                else if (contactTypeID == "Customer")
                    ddlUsers.Items.Insert(0, new ListItem("--Select CustomerMail--", Guid.Empty.ToString()));
                else if (contactTypeID == "Supplier")
                    ddlUsers.Items.Insert(0, new ListItem("--Select SupplierMail--", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, ListBox lb, DataTable CommonDt)
        {
            try
            {

                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    if (ddl != null)
                    {
                        ddl.Items.Clear();
                        ddl.DataSource = CommonDt;
                        ddl.DataTextField = "Description";
                        ddl.DataValueField = "ID";
                        ddl.DataBind();
                    }
                    else
                    {
                        lb.Items.Clear();
                        lb.Items.Clear();
                        lb.DataSource = CommonDt;
                        lb.DataTextField = "Description";
                        lb.DataValueField = "ID";
                        lb.DataBind();
                    }
                }
                else
                {
                    ddl.DataSource = null;
                    ddl.DataBind();
                }
                if (ddl != null)
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ContactType");
                dt.Columns.Add("UserID");
                dt.Columns.Add("DesignationID");
                dt.Columns.Add("RoleID");

                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvUsers.DataSource = ds;
                gvUsers.DataBind();
                int columncount = gvUsers.Rows[0].Cells.Count;
                gvUsers.Rows[0].Cells.Clear();
                gvUsers.Rows[0].Cells.Add(new TableCell());
                gvUsers.Rows[0].Cells[0].ColumnSpan = columncount;
                gvUsers.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        private void SendMail(string PWD)
        {
            try
            {
                CommonBLL.SendMails(ddlUsers.SelectedItem.Text.Trim(), "", "Requested for Password Reset.", "Your New Reset Password is " + PWD);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }
        # endregion

        # region DDL selected Indexes

        /// <summary>
        /// Bind User Mail-ID's based on Category Selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCtgry.SelectedItem.Text != "")
                {
                    ddlUsers.Items.Clear();
                    ContactTypeBind();
                    if (ddlCtgry.SelectedItem.Text != "")
                        BindDropDownList(ddlUsers, null, (CBLL.GetusersOnCtype(CommonBLL.FlagRegularDRP, ddlCtgry.SelectedItem.Text, new Guid(Session["CompanyID"].ToString())).Tables[0]));
                    else
                        ddlUsers.Items.Clear();

                    GetDataForGridView(AUBLL.GetAssignedUsersGrid(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                    if (hdnfldCngPwd.Value == "")
                        hdnfldCngPwd.Value = "False";
                }
                else
                {
                    ClearItems();
                    lblCustomer.Visible = false;
                    lblRoleType.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to autoselect designation and role
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = CBLL.ContactsBindGrid(CommonBLL.FlagZSelect, new Guid(ddlUsers.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BindDropDownList(ddlDsgntn, null, (EMBLL.GetEnumTypesWithGuid(CommonBLL.FlagRegularDRP, CommonBLL.Designation, new Guid(Session["CompanyID"].ToString()), CommonBLL.Designation)).Tables[0]);
                    ddlDsgntn.SelectedValue = ds.Tables[0].Rows[0]["DesignationID"].ToString();
                    if ((ddlCtgry.SelectedItem.Text) == CommonBLL.TraffickerContactTypeText)
                    {
                        ddlRole.SelectedValue = ddlRole.Items.FindByText(ds.Tables[0].Rows[0]["AccessRole"].ToString()).Value;
                    }
                    else if ((ddlCtgry.SelectedItem.Text) == CommonBLL.SuplrContactTypeText)
                    {
                        ddlRole.SelectedValue = ddlRole.Items.FindByText(ds.Tables[0].Rows[0]["AccessRole"].ToString()).Value;
                    }
                    else if ((ddlCtgry.SelectedItem.Text) == CommonBLL.CustmrContactTypeText)
                    {
                        string[] RoleIDs = (ds.Tables[0].Rows[0]["Custmr_SuplrID"].ToString()).Split(',');
                        foreach (ListItem item in LBCustomer.Items)
                        {
                            item.Selected = false;
                            foreach (string s in RoleIDs)
                            {
                                if (item.Value == s)
                                    item.Selected = true;
                            }
                        }

                    }
                    else
                    {
                        LBCustomer.SelectedValue = ds.Tables[0].Rows[0]["Custmr_SuplrID"].ToString();
                    }
                    LBCustomer.Enabled = false;
                }
                else
                {
                    ddlDsgntn.SelectedValue = "0";
                    ddlRole.SelectedValue = "0";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }
        # endregion

        # region Grid View Events

        /// <summary>
        /// This Event IS used to Edit And Delete Users from GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvUsers.Rows[index];
                string lblId = ((Label)row.FindControl("lblID")).Text.ToString();
                string lblUsrId = ((Label)row.FindControl("lblUsrID")).Text.ToString();
                if (e.CommandName == "Modify")
                {
                    ViewState["EditID"] = lblId;
                    EditUser(new Guid(lblId));
                }
                else if (e.CommandName == "Delete")
                {
                    int res = 0;
                    res = AUBLL.DeleteAssignUser(CommonBLL.FlagDelete, new Guid(lblId), new Guid(lblUsrId));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully.');", true);
                        ClearItems();
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error While Deleting.');", true);
                    GetDataForGridView(AUBLL.GetAssignedUsersGrid(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used for Conformation and for some modification on databind
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string ID = ((Label)e.Row.FindControl("lblCtype")).Text;
                    if (ID == "1")
                        ((Label)e.Row.FindControl("lblCtype")).Text = "Trafficker";
                    else if (ID == "2")
                        ((Label)e.Row.FindControl("lblCtype")).Text = "Customer";
                    else if (ID == "3")
                        ((Label)e.Row.FindControl("lblCtype")).Text = "Supplier";
                }

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is not in use but it should be kept for not getting Error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        /// <summary>
        /// Grid View Pre-Rendering Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUsers_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvUsers.UseAccessibleHeader = false;
                gvUsers.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        # endregion

        # region ButtonClicks

        /// <summary>
        /// This Event is used to Save new record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnsave_Click(object sender, EventArgs e)
        {
            SaveRecord();
            ClearItems();
        }

        /// <summary>
        /// This Event is used to clear controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            ClearItems();
            GetDataForGridView(null);
        }

        # endregion
    }
}
