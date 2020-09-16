using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class CVMapping : System.Web.UI.Page
    {
        # region variables
        int res = -1;
        ErrorLog ELog = new ErrorLog();
        MappingBLL MBLL = new MappingBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserName"].ToString() == null)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    //if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    if (Session["UserName"].ToString() == CommonBLL.SuperAdminRole)
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(CVMapping));
                        if (!IsPostBack)
                        {
                            Session["PreviousCustomers"] = null;
                            GetData();
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                            {
                                EditMappingRecord(new Guid(Request.QueryString["ID"].ToString()));
                            }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

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

        protected void GetData()
        {
            try
            {
                VendorsCustomers(ListBoxVendor, MBLL.SelectVendor(CommonBLL.FlagSelectAll));
                VendorsCustomers(ListBoxCustomers, MBLL.SelectCustomer(CommonBLL.FlagESelect));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

        private void VendorsCustomers(ListBox cntrl, DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0)
                {
                    cntrl.DataSource = ds.Tables[0];
                    cntrl.DataTextField = "Description";
                    cntrl.DataValueField = "ID";
                    cntrl.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

        protected void ClearAll()
        {
            try
            {
                btnSave.Text = "Save";
                ListBoxCustomers.SelectedIndex = ListBoxVendor.SelectedIndex = -1;
                Session["PreviousCustomers"] = null;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

        private void EditMappingRecord(Guid ID)
        {
            try
            {
                DataSet EditDS = new DataSet();
                EditDS = MBLL.SelectCVMapping(CommonBLL.FlagASelect, ID);
                Dictionary<string, Guid> PrevCustomers = new Dictionary<string, Guid>(); 
                if (EditDS.Tables[0].Rows.Count > 0)
                {
                    VendorsCustomers(ListBoxVendor, MBLL.SelectVendor(CommonBLL.FlagBSelect));
                    ListBoxVendor.SelectedValue = EditDS.Tables[0].Rows[0]["VendorId"].ToString();
                    foreach (ListItem Customers in ListBoxCustomers.Items)
                    {
                        for (int i = 0; i < EditDS.Tables[0].Rows.Count; i++)
                        {
                            if (Customers.Value == EditDS.Tables[0].Rows[i]["CustomerId"].ToString())
                            {
                                // ListBoxCustomers.SelectedValue = EditDS.Tables[0].Rows[i]["CustomerId"].ToString();
                                Customers.Selected = true;
                                PrevCustomers.Add(Customers.Text, new Guid(Customers.Value));
                            }
                        }
                    }
                    Session["PreviousCustomers"] = PrevCustomers;
                    ViewState["EditID"] = ID;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

        #region Button Click Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                
                if (btnSave.Text == "Update")
                {
                    if (Session["PreviousCustomers"] != null)
                    {
                        string PrevCustomers = Session["PreviousCustomers"].ToString();
                    }
                    res = MBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Guid.Empty, new Guid(ViewState["EditID"].ToString()), Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "");
                    res = -1;
                }
                if (btnSave.Text == "Save")
                {
                    DataSet EditDS = new DataSet();
                    Guid Vendor1 = Guid.Empty;
                    Guid Customer1 = Guid.Empty;

                    foreach (ListItem lv in ListBoxVendor.Items)
                    {
                        if (lv.Selected)
                        {
                            Vendor1 = new Guid(lv.Value);
                            //Customer1 = new Guid(lc.Value);
                            EditDS = MBLL.SelectCVMapping(CommonBLL.FlagASelect, Vendor1);
                            if (EditDS.Tables[0].Rows.Count > 0)
                            {
                                res = 0;
                                ALS.AuditLog(res, btnSave.Text, "", "Mapping", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('One of the Selected Vendor Already Exists ');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", "Error while Saving.");
                                break;
                            }
                            else
                            {
                                res = -1;
                            }
                        }
                    }
                }

                if (res == -1)
                {
                    Guid Vendor = Guid.Empty;
                    Guid Customer = Guid.Empty;
                    string SelCustomers = String.Join(",", ListBoxCustomers.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    Dictionary<string, Guid> SelCust = (Dictionary<string, Guid>)Session["PreviousCustomers"];
                    string Ids = "";
                    if (SelCust != null)
                    {
                        foreach (KeyValuePair<string, Guid> item in SelCust)
                        {
                            if (!SelCustomers.Contains(item.Value.ToString()))
                            {
                                Ids += item.Value.ToString() + ",";
                            }
                        }
                    }
                    foreach (ListItem lv in ListBoxVendor.Items)
                    {
                        if (lv.Selected)
                        {
                            foreach (ListItem lc in ListBoxCustomers.Items)
                            {
                                if (lc.Selected)
                                {
                                    Vendor = new Guid(lv.Value);
                                    Customer = new Guid(lc.Value);
                                    if (btnSave.Text == "Save")
                                    {
                                        res = MBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, Vendor, Customer, new Guid(Session["UserID"].ToString()),
                                              DateTime.Now, Guid.Empty, DateTime.Now, "");
                                    }
                                    else if (btnSave.Text == "Update")
                                    {
                                        res = MBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, Guid.Empty, Vendor, Customer, Guid.Empty,
                                              DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, Ids.Trim());
                                    }
                                }
                            }
                        }
                    }
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Mapping", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Mapping",
                                "Data Inserted Successfully.");
                            ClearAll();
                            Response.Redirect("CVMappingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Mapping", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", "Error while Saving.");
                        }
                    }
                    if (res == 0 && btnSave.Text == "Update")
                    {
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Mapping", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Mapping",
                                "Data Inserted Successfully.");
                            ClearAll();
                            Response.Redirect("CVMappingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Mapping", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating Customer cannot be unselected as it is used by another transaction');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", "Error while Updating.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        #endregion
    }
}

