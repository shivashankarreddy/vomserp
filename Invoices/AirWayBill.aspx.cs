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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class AirWayBill : System.Web.UI.Page
    {
        # region Variables

        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        CustomerBLL CBLL = new CustomerBLL();
        InvoiceBLL INBLL = new InvoiceBLL();
        AirWayBillBLL AWBLL = new AirWayBillBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";


        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    Ajax.Utility.RegisterTypeForAjax(typeof(AirWayBill));
                    if (!IsPostBack)
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        //Ajax.Utility.RegisterTypeForAjax(typeof(AirWayBill));
                        txtExecDate.Attributes.Add("readonly", "readonly");
                        if (!IsPostBack)
                            GEtData();
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

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

        private void GEtData()
        {
            try
            {
                #region Add/Update Permission Code
               // txtFreight.Attributes.Add("readonly", "readonly");

                //BindDropDownList(ddlPod, EMBAL.EnumMasterSelect(CommonBLL.FlagRegularDRP, 0, CommonBLL.PortofDischarge, 0));
                BindCustomers();
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge));
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty,Guid.Empty,Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery));
               
                if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                    Request.QueryString["ID"] != null)
                {
                    if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                    {
                        ViewState["EditID"] = Request.QueryString["ID"];
                        EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                    }
                }
                else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save";
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                # endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        private void BindFreightAmount()//Binding the Freight amount field 
        {
            try
            {
                string PrformaInv = String.Join(",", lbPInv.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string RefShpngID = String.Join(",", lbShippingBill.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = INBLL.GetDataSet1(CommonBLL.FlagISelect1, RefShpngID, Guid.Empty, Guid.Empty, Guid.Empty, PrformaInv, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                decimal FreightAmt = 0;
                decimal NoOfPkg = 0;
                decimal GrossWeight = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                  
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            FreightAmt += Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                            NoOfPkg += Convert.ToDecimal(ds.Tables[1].Rows[i]["TotPkgs"].ToString());
                            GrossWeight += Convert.ToDecimal(ds.Tables[1].Rows[i]["GrossWeight"].ToString());
                        }
                    }
                    txtFreight.Text = FreightAmt.ToString();
                    ddlPrtDscrg.SelectedValue = ds.Tables[1].Rows[0]["PrtDischarge"].ToString();
                    txtGrossweight.Text = GrossWeight.ToString();
                    txtNoPkgs.Text = NoOfPkg.ToString();
                    //txtGrossweight.Text = ds.Tables[1].Rows[0]["GrossWeight"].ToString();
                    //txtNoPkgs.Text = ds.Tables[1].Rows[0]["TotPkgs"].ToString();
                }
      
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }
        protected void ddlShipBillNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindFreightAmount();
                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }

        private void BindCustomers()
        {
            try
            {
                DataSet ds = CBLL.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP,Guid.Empty ,new Guid(Session["CompanyId"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    BindListBox(lbCustomer, ds.Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        private void BindProFrmaIDs()
        {
            try
            {
                if (lbCustomer.SelectedValue != "")
                {
                    string CustIDs = String.Join(",", lbCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, Guid.Empty, CustIDs, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        BindListBox(lbPInv, ds.Tables[0]);
                    else
                    {
                        lbPInv.Items.Clear();
                        lbShippingBill.Items.Clear();
                    }
                }
                else
                {
                    lbPInv.Items.Clear();
                    lbShippingBill.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        private void BindShippingBillIDs()
        {
            try
            {
                string PrformaInv = String.Join(",", lbPInv.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, PrformaInv, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    BindListBox(lbShippingBill, ds.Tables[0]);
                else
                    lbShippingBill.Items.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
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
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        protected void BindListBox(ListBox LB, DataTable CommonDt)
        {
            try
            {
                LB.DataSource = CommonDt;
                LB.DataTextField = "Description";
                LB.DataValueField = "ID";
                LB.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        private void EditRecord(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = AWBLL.GetDataSet(CommonBLL.FlagModify, ID);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BindCustomers();
                    Dictionary<string, string> CustIDs = new Dictionary<string, string>();
                    Dictionary<string, string> ProformaInvIDs = new Dictionary<string, string>();
                    Dictionary<string, string> ShippingBillIDs = new Dictionary<string, string>();

                    CustIDs = ds.Tables[0].Rows[0]["CustomerIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (CustIDs != null && CustIDs.Count > 0)
                    {
                        for (int i = 0; i < lbCustomer.Items.Count; i++)
                        {
                            string cs = lbCustomer.Items[i].Value.ToString();
                            if (CustIDs.ContainsKey(cs.Trim()))
                                lbCustomer.Items[i].Selected = true;
                        }
                    }
                    lbCustomer.Enabled = false;
                    //BindProFrmaIDs();
                    BindListBox(lbPInv, ds.Tables[1]);
                    ProformaInvIDs = ds.Tables[0].Rows[0]["ProformaINVIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (ProformaInvIDs != null && ProformaInvIDs.Count > 0)
                    {
                        for (int j = 0; j < lbPInv.Items.Count; j++)
                        {
                            string pv = lbPInv.Items[j].Value.ToString();
                            if (ProformaInvIDs.ContainsKey(pv.Trim()))
                                lbPInv.Items[j].Selected = true;
                        }
                    }
                    lbPInv.Enabled = false;
                    BindShippingBillIDs();
                    ShippingBillIDs = ds.Tables[0].Rows[0]["ShippingBillIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (ShippingBillIDs != null && ShippingBillIDs.Count > 0)
                    {
                        for (int k = 0; k < lbShippingBill.Items.Count; k++)
                        {
                            string spb = lbShippingBill.Items[k].Value.ToString();
                            if (ShippingBillIDs.ContainsKey(spb.Trim()))
                                lbShippingBill.Items[k].Selected = true;
                        }
                    }
                    lbShippingBill.Enabled = false;
                    txtAwb.Text = ds.Tables[0].Rows[0]["AWBNumber"].ToString();
                    txtExecDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["ExecutableDT"].ToString()));
                    ddlPrtDscrg.SelectedValue = ds.Tables[0].Rows[0]["PODischarge"].ToString();
                    txtPlaceofReceipt.Text = ds.Tables[0].Rows[0]["PlcOfRcpt"].ToString();
                    ddlPlcDlry.SelectedValue = ds.Tables[0].Rows[0]["PlcOfDlvry"].ToString();
                    txtFreight.Text = ds.Tables[0].Rows[0]["Freight"].ToString();
                    //txtpaid.Text = ds.Tables[0].Rows[0]["PrePostpaid"].ToString();
                    txtTareweight.Text = ds.Tables[0].Rows[0]["TWeight"].ToString();
                    txtGrossweight.Text = ds.Tables[0].Rows[0]["GWeight"].ToString();
                    txtNoPkgs.Text = ds.Tables[0].Rows[0]["TotalPkgs"].ToString();
                    txtDimensions.Text = ds.Tables[0].Rows[0]["Dimenctions"].ToString();
                    txtTotalprepaid.Text = ds.Tables[0].Rows[0]["TotalPrePaid"].ToString();
                    ViewState["EditID"] = Request.QueryString["ID"];
                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        private void ClearAll()
        {
            Response.Redirect("~/Invoices/AirWayBill.aspx");
            //ddlPod.SelectedIndex = -1;
            //lbShippingBill.Items.Clear();
            //lbPInv.Items.Clear();
        }

        # endregion

        # region ListBox Events

        protected void ddlPod_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (ddlPod.SelectedValue != "0")
            //    {
            //        DataSet ds = CBLL.SelectCustomers(CommonBLL.FlagZSelect, Convert.ToInt32(ddlPod.SelectedValue));
            //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //        {
            //            BindListBox(lbCustomer, ds.Tables[0]);
            //            //lbCustomer.DataSource = ds.Tables[0];
            //            //lbCustomer.DataTextField = "Description";
            //            //lbCustomer.DataValueField = "ID";
            //            //lbCustomer.DataBind();
            //        }
            //    }
            //    else
            //        ClearAll();
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            //}
        }

        protected void lbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindProFrmaIDs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        protected void lbPInv_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindShippingBillIDs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClicks

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int res = 1;
                string CustIDs = String.Join(",", lbCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string ProformaInvIDs = String.Join(",", lbPInv.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string ShippingBillIDs = String.Join(",", lbShippingBill.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (btnSave.Text == "Save")
                {
                    res = AWBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert,Guid.Empty, CustIDs, ProformaInvIDs, ShippingBillIDs, txtAwb.Text.Trim(),
                        CommonBLL.DateInsert(txtExecDate.Text), new Guid(ddlPrtDscrg.SelectedValue), txtPlaceofReceipt.Text.Trim(),
                        new Guid(ddlPlcDlry.SelectedValue), txtFreight.Text, "", Convert.ToDecimal(txtTareweight.Text.Trim()),
                        Convert.ToDecimal(txtGrossweight.Text.Trim()), Convert.ToInt64(txtNoPkgs.Text), txtDimensions.Text.Trim(),
                        Convert.ToDecimal(txtTotalprepaid.Text), new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), Guid.Empty, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, "",new Guid(Session["CompanyId"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Air Way Bill", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("~/Invoices/AirWayBillStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Air Way Bill", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoicse/ErrorLog"), "Air Way Bill", "Error while Inserting.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = AWBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), CustIDs, ProformaInvIDs, ShippingBillIDs, txtAwb.Text.Trim(),
                        CommonBLL.DateInsert(txtExecDate.Text), new Guid(ddlPrtDscrg.SelectedValue), txtPlaceofReceipt.Text.Trim(),
                       new Guid(ddlPlcDlry.SelectedValue), txtFreight.Text, "", Convert.ToDecimal(txtTareweight.Text.Trim()),
                        Convert.ToDecimal(txtGrossweight.Text.Trim()), Convert.ToInt64(txtNoPkgs.Text), txtDimensions.Text.Trim(),
                        Convert.ToDecimal(txtTotalprepaid.Text), new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                        new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, txtComments.Text.Trim(), new Guid(Session["CompanyId"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Air Way Bill", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("~/Invoices/AirWayBillStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Air Way Bill", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Air Way Bill", ex.Message.ToString());
            }
        }

        protected void btnclear_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Invoices/AirWayBill.aspx");
        }

        # endregion

        # region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckAB(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckEnquiryNo('R', EnqNo,new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        #endregion
    }
}
