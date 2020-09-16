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
    public partial class Customer : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        ContactBLL CBLL = new ContactBLL();
        AutoComplete ACWM = new AutoComplete();
        CustomerBLL custbal = new CustomerBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        AuditLogs ALS = new AuditLogs();
        FieldAccessBLL FAB = new FieldAccessBLL();
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
                    Response.Redirect("../login.aspx", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(Customer));
                        if (!IsPostBack)
                            GetData();

                        #region Add/Update Permission Code
                        //To Check User can have the Add/Update permissions
                        if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                        {
                            //lbtnsavenew.Enabled = true;
                            btnSave.Enabled = true;
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            //lbtnsavenew.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                        {
                            //lbtnsavenew.Enabled = false;
                            btnSave.Enabled = false;
                            btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                            //lbtnsavenew.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        }
                        #endregion

                        string CountryID = Request.QueryString["ID"] as string;
                        if (CountryID != null)
                        {
                            if (CountryID == "0")
                            {
                                DataTable table = new DataTable();
                                table.Columns.Add("ID", typeof(System.Int64));
                                table.Columns.Add("Description", typeof(System.String));
                                DataRow drow = table.NewRow();
                                drow["ID"] = "0"; drow["Description"] = "-- Select --";
                                table.Rows.InsertAt(drow, 0);
                                string result = string.Empty;
                                foreach (DataRow r in table.Rows)
                                {
                                    result += r["Description"].ToString() + "," + r["ID"].ToString() + ";";
                                }
                                Response.Clear();
                                Response.Write(result);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                            }
                            else
                            {
                                DataTable table = (embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0];
                                DataRow drow = table.NewRow();
                                drow["ID"] = "0"; drow["Description"] = "-- Select --";
                                table.Rows.InsertAt(drow, 0);


                                string result = string.Empty;

                                foreach (DataRow r in table.Rows)
                                {
                                    result += r["Description"].ToString() + "," + r["ID"].ToString() + ";";
                                }

                                Response.Clear();
                                Response.Write(result);
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                            }
                        }
                        if (IsPostBack)
                        {
                            ddlblngSt.SelectedValue = Request.Form[ddlblngSt.UniqueID];
                            ddlBilngCty.SelectedValue = Request.Form[ddlBilngCty.UniqueID];
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
            }
        }
        #endregion

        #region Save/Save & New Events for Insert Customer
        /// <summary>
        /// Save/Save & new Button Click Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>       
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    int result = custbal.InsertUpdateCustomer(CommonBLL.FlagNewInsert, Guid.Empty, txtorgnm.Text, txtbsnm.Text,
                        txttpn.Text, txtmbl.Text, txtpem.Text, txtsem.Text,
                                txtfx.Text, txtfx2.Text, new Guid(ddlRegion.SelectedValue.ToString()), txtShpingPortNm.Text, txtblngstrt.Text,
                                ddlBilngCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlBilngCty.SelectedValue),
                                ddlblngSt.SelectedValue == "" ? Guid.Empty : new Guid(ddlblngSt.SelectedValue), txtblngpb.Text,
                                ddlblngCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlblngCntry.SelectedValue), txtshpngstrt.Text,
                                ddlshpngCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngCty.SelectedValue),
                                ddlshpngSt.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngSt.SelectedValue), txtshpngpb.Text,
                                ddlshpngCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngCntry.SelectedValue), txtKindAttn.Text,
                                ddlAsgnedUsr.SelectedValue == "0" ? Guid.Empty : new Guid(ddlAsgnedUsr.SelectedValue), new Guid(ddlCurrency.SelectedValue), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Customer Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(custbal.SelectCustomers(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Customer Master",
                            "Data Inserted successfully in Customer Master.");
                        ClearInputs();
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    Guid CustID = new Guid(hdfdCustmrID.Value);
                    int result1 = custbal.InsertUpdateCustomer(CommonBLL.FlagUpdate, CustID, txtorgnm.Text, txtbsnm.Text,
                        txttpn.Text, txtmbl.Text, txtpem.Text, txtsem.Text, txtfx.Text, txtfx2.Text, new Guid(ddlRegion.SelectedValue.ToString()), txtShpingPortNm.Text, txtblngstrt.Text,
                       ddlBilngCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlBilngCty.SelectedValue),
                                ddlblngSt.SelectedValue == "" ? Guid.Empty : new Guid(ddlblngSt.SelectedValue), txtblngpb.Text,
                                ddlblngCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlblngCntry.SelectedValue), txtshpngstrt.Text,
                                ddlshpngCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngCty.SelectedValue),
                                ddlshpngSt.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngSt.SelectedValue), txtshpngpb.Text,
                                ddlshpngCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlshpngCntry.SelectedValue), txtKindAttn.Text,
                                ddlAsgnedUsr.SelectedValue == "0" ? Guid.Empty : new Guid(ddlAsgnedUsr.SelectedValue), new Guid(ddlCurrency.SelectedValue), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (result1 == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Customer Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(custbal.SelectCustomers(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Customer Master",
                            "Data Updated successfully in Customer Master.");
                        ClearInputs();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            GetData();
        }
        #endregion

        #region Bind DropDownList and GirdView Controls

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
        /// Bind DropDownLists and GridView Controls
        /// </summary>       
        protected void GetData()
        {
            try
            {

                Guid CmpnyId = new Guid(Session["CompanyID"].ToString());
                ClearInputs();
                BindDropDownList(ddlAsgnedUsr, CBLL.TeamLeadBindDRP(CommonBLL.FlagESelect, new Guid(Session["CompanyID"].ToString())));
                BindGridData(custbal.SelectCustomersDtlsGUIDBind(CommonBLL.FlagSelectAll, Guid.Empty, CmpnyId));
                BindDropDownList(ddlblngCntry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                BindDropDownList(ddlshpngCntry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                BindDropDownList(ddlCurrency, FAB.GetFieldMaster(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty));
                BindDropDownList(ddlRegion, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
            }
        }

        protected void BindGridData(DataSet CusstDt)
        {
            try
            {
                gvcustomer.DataSource = CusstDt.Tables[0];
                gvcustomer.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
            }
        }

        protected string ShowNmbrs(object item1, object item2)
        {
            string nubr = ((!String.IsNullOrEmpty(item1.ToString())) && (!String.IsNullOrEmpty(item2.ToString()))) ?
                (item1.ToString() + "/" + item2.ToString()) :
                (((!String.IsNullOrEmpty(item1.ToString())) || (!String.IsNullOrEmpty(item2.ToString()))) ?
                ((!String.IsNullOrEmpty(item1.ToString())) ? item1.ToString() : item2.ToString()) : "");

            return nubr;
        }
        #endregion

        #region GridView RowCommnd for Modify/Delete and Row Data Bound event for Delete confirmation Message Customer

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcustomer_RowDataBound(object sender, GridViewRowEventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }

            //int lastCellIndex = e.Row.Cells.Count - 1;
            //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            //deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
        }

        /// <summary>
        /// Grid View Pre-Render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcustomer_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvcustomer.UseAccessibleHeader = false;
                gvcustomer.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcustomer_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvcustomer.Rows[index];
                if (e.CommandName == "Modify")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCustcd")).Text);
                    SetUpdateValues(custbal.SelectCustomersDtlsGUIDBind(CommonBLL.FlagModify, ID, new Guid(Session["CompanyID"].ToString())));
                    btnSave.Text = "Update"; //lbtnsavenew.Visible = false;
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCustcd")).Text);
                    //Convert.ToInt32(((Label)gvrow.FindControl("lblCmpnyId")).Text) == 0 : 0 ? Convert.ToInt32(((Label)gvrow.FindControl("lblCmpnyId")).Text);
                    //int CmpnyID = 0;
                    // if (Session["CompanyID"] != "")
                    Guid CmpnyID = new Guid(Session["CompanyID"].ToString());
                    res = custbal.DeleteCustomerWithGuid(CommonBLL.FlagDelete, ID);
                    BindGridData(custbal.SelectCustomersDtlsGUIDBind(CommonBLL.FlagSelectAll, Guid.Empty, CmpnyID));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Customer Master",
                            "Data Deleted successfully in Customer Master.");
                        ClearInputs();
                    }
                }
                gvrow.FindControl("lblCustcd").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        protected void SetUpdateValues(DataSet CustomerDt)
        {
            try
            {
                hdfdCustmrID.Value = CustomerDt.Tables[0].Rows[0]["ID"].ToString();
                txtorgnm.Text = CustomerDt.Tables[0].Rows[0]["OrgName"].ToString();
                txtbsnm.Text = CustomerDt.Tables[0].Rows[0]["BussName"].ToString();
                txttpn.Text = CustomerDt.Tables[0].Rows[0]["Phone"].ToString();
                txtmbl.Text = CustomerDt.Tables[0].Rows[0]["Mobile"].ToString();
                txtpem.Text = CustomerDt.Tables[0].Rows[0]["PriEmail"].ToString();
                txtsem.Text = CustomerDt.Tables[0].Rows[0]["SecEmail"].ToString();
                txtfx.Text = CustomerDt.Tables[0].Rows[0]["Fax1"].ToString();
                txtfx2.Text = CustomerDt.Tables[0].Rows[0]["Fax2"].ToString();
                txtShpingPortNm.Text = CustomerDt.Tables[0].Rows[0]["ShipngPort"].ToString();
                ddlRegion.SelectedValue = (CustomerDt.Tables[0].Rows[0]["Region"].ToString() == "" || CustomerDt.Tables[0].Rows[0]["Region"].ToString() == null) ? Guid.Empty.ToString()
                    : CustomerDt.Tables[0].Rows[0]["Region"].ToString();
                CascadingDropDown1.SelectedValue = CustomerDt.Tables[0].Rows[0]["BillCountry"].ToString();
                //BindDropDownList(ddlblngSt, embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(ddlblngCntry.SelectedValue)));
                CascadingDropDown2.SelectedValue = CustomerDt.Tables[0].Rows[0]["BillState"].ToString();
                //BindDropDownList(ddlBilngCty, embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(ddlblngSt.SelectedValue)));
                CascadingDropDown3.SelectedValue = CustomerDt.Tables[0].Rows[0]["BillCity"].ToString();
                txtblngstrt.Text = CustomerDt.Tables[0].Rows[0]["BillStreet"].ToString();
                txtblngpb.Text = CustomerDt.Tables[0].Rows[0]["BillPin"].ToString();

                CascadingDropDown4.SelectedValue = CustomerDt.Tables[0].Rows[0]["ShipCountry"].ToString() == "0" ?
                    CascadingDropDown4.SelectedValue = "" : CustomerDt.Tables[0].Rows[0]["ShipCountry"].ToString();
                //BindDropDownList(ddlshpngSt, embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(ddlshpngCntry.SelectedValue)));
                CascadingDropDown5.SelectedValue = CustomerDt.Tables[0].Rows[0]["ShipState"].ToString();
                //BindDropDownList(ddlshpngCty, embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(ddlshpngSt.SelectedValue)));
                CascadingDropDown6.SelectedValue = CustomerDt.Tables[0].Rows[0]["ShipCity"].ToString();
                txtshpngstrt.Text = CustomerDt.Tables[0].Rows[0]["ShipStreet"].ToString();
                txtshpngpb.Text = CustomerDt.Tables[0].Rows[0]["ShipPin"].ToString();
                txtKindAttn.Text = CustomerDt.Tables[0].Rows[0]["KindAttn"].ToString();
                //ddlAsgnedUsr.SelectedValue = CustomerDt.Tables[0].Rows[0]["AssignedTo"].ToString();
                ddlAsgnedUsr.SelectedValue = CustomerDt.Tables[0].Rows[0]["AssignedTo"].ToString() == Guid.Empty.ToString() ? Guid.Empty.ToString() : CustomerDt.Tables[0].Rows[0]["AssignedTo"].ToString();
                ddlCurrency.SelectedValue = CustomerDt.Tables[0].Rows[0]["Currency"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Clear Input Fields
        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearInputs()
        {
            try
            {
                hdfdCustmrID.Value = txtorgnm.Text = txtbsnm.Text = txttpn.Text = txtmbl.Text = txtShpingPortNm.Text = "";
                txtpem.Text = txtsem.Text = txtfx.Text = txtfx2.Text = txtblngstrt.Text = txtblngpb.Text = "";
                txtKindAttn.Text = "";
                ddlRegion.SelectedIndex = ddlblngCntry.SelectedIndex = ddlblngSt.SelectedIndex = ddlBilngCty.SelectedIndex = ddlAsgnedUsr.SelectedIndex = ddlCurrency.SelectedIndex = -1;

                ddlshpngCntry.SelectedIndex = ddlshpngSt.SelectedIndex = ddlshpngCty.SelectedIndex = -1;

                txtshpngstrt.Text = txtshpngpb.Text = ""; btnSave.Text = "Save"; //lbtnsavenew.Visible = true;
                //hdfdblngCty.Value = hdfdblngSt.Value = hdfdshpngCty.Value = hdfdShpngSt.Value = "0";
                CascadingDropDown1.SelectedValue = CascadingDropDown4.SelectedValue = "0";
                txtorgnm.Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool CheckCustomerName(string Name, string Type)
        {
            try
            {
                if (ACWM.CheckCustomerExistance(Name, Type) > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer", ex.Message.ToString());
                return false;
            }
        }

        #endregion

    }
}
