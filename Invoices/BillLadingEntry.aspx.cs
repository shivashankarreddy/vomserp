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
using System.Text;
using BAL;
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class BillLadingEntry : System.Web.UI.Page
    {
        # region Variables

        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        CustomerBLL CBLL = new CustomerBLL();
        InvoiceBLL INBLL = new InvoiceBLL();
        BillOfLadingBLL BLBLL = new BillOfLadingBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        static string CustIDs = "";
        static string PrformaInv = "";
        static string ShippingBillIDs = "";

        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
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
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Ajax.Utility.RegisterTypeForAjax(typeof(BillLadingEntry));
                        if (!IsPostBack)
                        {
                            txtSOBDt.Attributes.Add("readonly", "readonly");
                            txtDate.Attributes.Add("readonly", "readonly");
                            txtDOIDt.Attributes.Add("readonly", "readonly");
                            //txtChqDt.Attributes.Add("readonly", "readonly");
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        #endregion

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

        private void GetData()
        {
            try
            {
                txtDate.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtSOBDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtDOIDt.Text = (txtDOIDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtDOIDt.Text);
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading));
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge));
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery));
                BindCustomers();

                #region Add/Update Permission Code
                if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                    Request.QueryString["ID"] != null)
                {
                    if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                    {
                        ViewState["EditID"] = Request.QueryString["ID"];
                        EditRecord(new Guid(Request.QueryString["ID"]));
                    }
                }
                else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                {
                    Session["ContainerDetails"] = CommonBLL.EmptyDTBillOfladingContainer();
                    dvContainerDetails.InnerHtml = FillContainerDetails();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        private void EditRecord(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = BLBLL.GetDataSet(CommonBLL.FlagModify, ID);
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    BindCustomers();
                    Dictionary<string, string> CustIDs = new Dictionary<string, string>();
                    Dictionary<string, string> ProformaInvIDs = new Dictionary<string, string>();
                    Dictionary<string, string> ShippingBillIDs = new Dictionary<string, string>();

                    CustIDs = ds.Tables[0].Rows[0]["CustomerIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (CustIDs != null && CustIDs.Count > 0)
                    {
                        for (int i = 0; i < lbCustomers.Items.Count; i++)
                        {
                            string cs = lbCustomers.Items[i].Value.ToString();
                            if (CustIDs.ContainsKey(cs.Trim()))
                                lbCustomers.Items[i].Selected = true;
                        }
                    }
                    lbCustomers.Enabled = false;
                    //BindProFrmaIDs();
                    BindListBox(lbPrfmaInvc, ds.Tables[2]);
                    ProformaInvIDs = ds.Tables[0].Rows[0]["ProformaInvIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (ProformaInvIDs != null && ProformaInvIDs.Count > 0)
                    {
                        for (int j = 0; j < lbPrfmaInvc.Items.Count; j++)
                        {
                            string pv = lbPrfmaInvc.Items[j].Value.ToString();
                            if (ProformaInvIDs.ContainsKey(pv.Trim()))
                                lbPrfmaInvc.Items[j].Selected = true;
                        }
                    }
                    lbPrfmaInvc.Enabled = false;
                    BindShippingBillIDs();
                    ShippingBillIDs = ds.Tables[0].Rows[0]["ShippingBillIDs"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    if (ShippingBillIDs != null && ShippingBillIDs.Count > 0)
                    {
                        for (int k = 0; k < lbShpngBil.Items.Count; k++)
                        {
                            string spb = lbShpngBil.Items[k].Value.ToString();
                            if (ShippingBillIDs.ContainsKey(spb.Trim()))
                                lbShpngBil.Items[k].Selected = true;
                        }
                    }
                    lbShpngBil.Enabled = false;
                    txtShippingLine.Text = ds.Tables[0].Rows[0]["ShippingLine"].ToString();
                    txtBookingNo.Text = ds.Tables[0].Rows[0]["BookingNo"].ToString();
                    txtbillLading.Text = ds.Tables[0].Rows[0]["BillofLadingNo"].ToString();
                    txtSOBDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["SOBDT"].ToString()));
                    txtVessel.Text = ds.Tables[0].Rows[0]["Vessel"].ToString();
                    txtVoyage.Text = ds.Tables[0].Rows[0]["Voyage"].ToString();
                    ddlPrtLdng.SelectedValue = ds.Tables[0].Rows[0]["PortOfLoading"].ToString();
                    ddlPrtDscrg.SelectedValue = ds.Tables[0].Rows[0]["PortOfDischarge"].ToString();
                    txtPlcRcpt.Text = ds.Tables[0].Rows[0]["PlaceOfRcpt"].ToString();
                    ddlPlcDlry.SelectedValue = ds.Tables[0].Rows[0]["PlaceOfDelivery"].ToString();
                    //txtDOIDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["DOIDT"].ToString()));
                    ///////////////////////////////
                    txtFreight.Text = ds.Tables[0].Rows[0]["Frieght"].ToString();

                    ///////////////////////////////
                    if (ds.Tables[0].Rows[0]["IDFNo"].ToString()!="")
                    {
                        txtIDFNo.Text = ds.Tables[0].Rows[0]["IDFNo"].ToString();
                        txtFERINo.Enabled = false;
                        txtECTNNo.Enabled = false;
                    }
                    if(ds.Tables[0].Rows[0]["FERINo"].ToString()!=""){
                        txtFERINo.Text = ds.Tables[0].Rows[0]["FERINo"].ToString();
                        txtIDFNo.Enabled = false;
                        txtECTNNo.Enabled = false;
                     }
                    if (ds.Tables[0].Rows[0]["ECTNNo"].ToString() != "")
                    {
                        txtECTNNo.Text = ds.Tables[0].Rows[0]["ECTNNo"].ToString();
                        txtIDFNo.Enabled = false;
                        txtFERINo.Enabled = false;
                    }

                   // txtIDFNo.Text = ds.Tables[0].Rows[0]["IDFNo"].ToString();
                    //txtFERINo.Text = ds.Tables[0].Rows[0]["FERINo"].ToString();
                    //txtECTNNo.Text = ds.Tables[0].Rows[0]["ECTNNo"].ToString();
                    txtDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"].ToString()));
                    txtTareWeight.Text = ds.Tables[0].Rows[0]["Tweight"].ToString();
                    txtGrWeight.Text = ds.Tables[0].Rows[0]["Gweight"].ToString();
                    txtTotalPkgs.Text = ds.Tables[0].Rows[0]["Totalpkgs"].ToString();
                    txtDOIDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["DOIDT"].ToString()));
                    Session["ContainerDetails"] = ds.Tables[1];
                    dvContainerDetails.InnerHtml = FillContainerDetails();
                    ViewState["EditID"] = Request.QueryString["ID"];
                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
                else
                    btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        private void BindCustomers()
        {
            try
            {
                DataSet ds = CBLL.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    BindListBox(lbCustomers, ds.Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        private void BindProFrmaIDs()
        {
            try
            {
                if (lbCustomers.SelectedValue != "")
                {
                    CustIDs = String.Join(",", lbCustomers.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                    DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, CustIDs, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        BindListBox(lbPrfmaInvc, ds.Tables[0]);
                    else
                    {
                        lbPrfmaInvc.Items.Clear();
                        lbShpngBil.Items.Clear();
                    }
                }
                else
                {
                    lbPrfmaInvc.Items.Clear();
                    lbShpngBil.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        private void BindFreightAmount()//Binding the Freight amount field 
        {
            try
            {
                string PrformaInvs = String.Join(",", lbShpngBil.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = INBLL.GetDataSet1(CommonBLL.FlagISelect1, PrformaInvs, Guid.Empty, Guid.Empty, Guid.Empty, PrformaInv, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //{
                //    txtFreight.Text = ds.Tables[0].Rows[0]["Amount"].ToString();
                //    ddlPrtLdng.SelectedValue = ds.Tables[1].Rows[0]["PrtLoading"].ToString();
                //    ddlPrtDscrg.SelectedValue = ds.Tables[1].Rows[0]["PrtDischarge"].ToString();
                //    txtGrWeight.Text = ds.Tables[1].Rows[0]["GrossWeight"].ToString();
                //    txtTotalPkgs.Text = ds.Tables[1].Rows[0]["TotPkgs"].ToString();
     

                //}
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
                    ddlPrtLdng.SelectedValue = ds.Tables[1].Rows[0]["PrtLoading"].ToString();
                    ddlPrtDscrg.SelectedValue = ds.Tables[1].Rows[0]["PrtDischarge"].ToString();
                    txtGrWeight.Text = GrossWeight.ToString();
                    txtTotalPkgs.Text = NoOfPkg.ToString();
                    
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }
        
        private void BindShippingBillIDs()
        {
            try
            {
                PrformaInv = String.Join(",", lbPrfmaInvc.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, PrformaInv, Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    BindListBox(lbShpngBil, ds.Tables[0]);
                else
                    lbShpngBil.Items.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Drop Down List
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Box
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="CommonDt"></param>
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Container Details
        /// </summary>
        /// <returns></returns>
        private string FillContainerDetails()
        {
            try
            {
                DataTable dt = (DataTable)Session["ContainerDetails"];
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                " id='tblPaymentTerms' align='center'><thead align='center'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Container Type</th><th>Container No</th><th>C Seal No.</th>" +
                    "<th>L Seal No.</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td>");
                        # region Bind-DDL
                        sb.Append("<select id='ddlCType" + SNo + "' onchange='FillItemGrid(" + SNo + ")' Class='bcAspdropdown' width='50px'>");
                        sb.Append("<option value='0'>-SELECT-</option>");

                        DataSet ds = new DataSet();
                        EnumMasterBLL EMBLL = new EnumMasterBLL();
                        ds = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContainerTypes);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (dt.Rows[i]["Ctype"].ToString() == row["ID"].ToString())
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                        }
                        sb.Append("</select>");
                        # endregion
                        sb.Append("</td>");

                        sb.Append("<td>&nbsp;<input type='text' name='txtCNO' value='" + dt.Rows[i]["CNO"].ToString() + "' maxlength='500' "
                            + " id='txtCNO" + SNo + "' onchange='FillItemGrid(" + SNo + ")' class='bcAsptextbox'/></td>");
                        sb.Append("<td align='right'><input type='text' name='txtCSealNo' onfocus='this.select()' "
                            + " onMouseUp='return false' value='" + dt.Rows[i]["CSealNo"].ToString() + "'  id='txtCSealNo" + SNo + "' maxlength='500' "
                            + " onkeypress='return onchange='FillItemGrid(" + SNo + ")' class='bcAsptextbox'/></td>");
                        sb.Append("<td><input type='text' name='txtLSealNo' value='" + dt.Rows[i]["LSealNo"].ToString() + "' maxlength='500' "
                            + " id='txtLSealNo" + SNo + "' onchange='FillItemGrid(" + SNo + ")' class='bcAsptextbox'/></td>");

                        sb.Append("<td valign='top'>");
                        if (dt.Rows.Count == 1)
                            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;<span class='gridactionicons'><a href='javascript:void(0)'" +
                        " onclick='AddNewRow(" + SNo + ")'" +
                        " class='icons additionalrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></span>");
                        else if (dt.Rows.Count - 1 == i)
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return doConfirm(" + SNo + ")' " +
                                " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span>&nbsp;&nbsp;" +
                                "<a href='javascript:void(0)' onclick='AddNewRow(" + SNo + ")' " +
                                " class='icons additionalrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a>");
                        else
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirm(" + SNo + ")' class='icons deleteicon' " +
                                " title='Delete' OnClientClick='javascript:return doConfirm();'>" +
                                " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='5' class='rounded-foot-right' " +
                    " align='left'><input id='HfMessage' type='hidden' name='HfMessage' value='"
                    + "" + "'/></th></tfoot>");
                }
                sb.Append("</tbody></table>");

                return sb.ToString();// FillItemGrid(0, dt, Codes, ds, 0, CatCodes, 0, UnitCodes);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
                return string.Empty;
            }
        }

        private void ClearAll()
        {
            Response.Redirect("~/Invoices/BillLadingEntry.aspx");
        }

        # endregion

        # region Selected Index Changed Events

        /// <summary>
        /// Place of Destination Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPlcDstnsn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //DataSet ds = CBLL.SelectCustomers(CommonBLL.FlagZSelect, Convert.ToInt32(ddlPlcDstnsn.SelectedValue));
                //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //{
                //    BindListBox(lbCustomers, ds.Tables[0]);
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box Customers Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (lbCustomers.SelectedValue != "")
                //{
                //    string CustIDs = String.Join(",", lbCustomers.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //    DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagZSelect, 0, 0, 0, CustIDs, 0);
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //        BindListBox(lbPrfmaInvc, ds.Tables[0]);
                //    else
                //    {
                //        lbPrfmaInvc.Items.Clear();
                //        lbShpngBil.Items.Clear();
                //    }
                //}
                //else
                //{
                //    lbPrfmaInvc.Items.Clear();
                //    lbShpngBil.Items.Clear();
                //}
                BindProFrmaIDs();
                dvContainerDetails.InnerHtml = FillContainerDetails();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box Proforma Invoice Selected Index Chaged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbPrfmaInvc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //string PrformaInv = String.Join(",", lbPrfmaInvc.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //DataSet ds = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagXSelect, 0, 0, 0, PrformaInv, 0);
                //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //{
                //    BindListBox(lbShpngBil, ds.Tables[0]);
                //}
                BindShippingBillIDs();
                dvContainerDetails.InnerHtml = FillContainerDetails();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }
        //geeting shiping bill no
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

        //protected void ddlFreight_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (ddlFreight.SelectedValue == "1")
        //        {
        //            ddlPmtPop.SelectedValue = "0";
        //            ModalPopupExtender1.Show();
        //        }
        //        else                
        //            ModalPopupExtender1.Hide();
        //        dvContainerDetails.InnerHtml = FillContainerDetails();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
        //    }
        //}

        # endregion

        # region Button Clicks

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int res = 1; Filename = FileName();
                ShippingBillIDs = String.Join(",", lbShpngBil.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                CustIDs = String.Join(",", lbCustomers.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataTable dt = (DataTable)Session["ContainerDetails"];

                //foreach (DataColumn dc in dt.Columns)
                //{
                //    if (dc.ColumnName != "Ctype" && dc.ColumnName != "CNO" && dc.ColumnName != "CSealNo" && dc.ColumnName != "LSealNo")
                //    {
                //        dt.Columns.Remove(dc);
                //    }
                //}

                for (int index = dt.Columns.Count - 1; index >= 0; index--)
                {
                    if (dt.Columns[index].ColumnName != "Ctype" && dt.Columns[index].ColumnName != "CNO" && dt.Columns[index].ColumnName != "CSealNo" && dt.Columns[index].ColumnName != "LSealNo")
                    {
                        dt.Columns.RemoveAt(index);
                    }
                }

                dt.AcceptChanges();
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["Ctype"].ToString() != "0")
                {

                    if (btnSave.Text == "Save")
                    {
                        res = BLBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, CustIDs, PrformaInv, ShippingBillIDs, txtShippingLine.Text.Trim(),
                        txtBookingNo.Text.Trim(), txtbillLading.Text.Trim(), CommonBLL.DateInsert(txtSOBDt.Text.Trim()), txtVessel.Text.Trim(),
                        txtVoyage.Text.Trim(), new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue), txtPlcRcpt.Text.Trim(),
                        new Guid(ddlPlcDlry.SelectedValue), Convert.ToDecimal(txtFreight.Text), 0, 0, "", CommonBLL.EndDate,
                        txtIDFNo.Text.Trim(), txtFERINo.Text.Trim(), txtECTNNo.Text.Trim(), CommonBLL.DateInsert(txtDate.Text.Trim()), Convert.ToDecimal(txtTareWeight.Text.Trim()),
                        Convert.ToDecimal(txtGrWeight.Text.Trim()), Convert.ToInt64(txtTotalPkgs.Text.Trim()), CommonBLL.DateInsert(txtDOIDt.Text), new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), Guid.Empty, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, dt, "", new Guid(Session["CompanyId"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill of Lading", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("~/Invoices/BillOfLadingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill of Lading", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoicse/ErrorLog"), "Bill of Lading", "Error while Inserting.");
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        res = BLBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), CustIDs, PrformaInv, ShippingBillIDs, txtShippingLine.Text.Trim(),
                       txtBookingNo.Text.Trim(), txtbillLading.Text.Trim(), CommonBLL.DateInsert(txtSOBDt.Text.Trim()), txtVessel.Text.Trim(),
                        txtVoyage.Text.Trim(), new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue), txtPlcRcpt.Text.Trim(),
                        new Guid(ddlPlcDlry.SelectedValue), Convert.ToDecimal(txtFreight.Text), 0, 0, "", CommonBLL.EndDate,
                        txtIDFNo.Text.Trim(),txtFERINo.Text.Trim(), txtECTNNo.Text.Trim(), CommonBLL.DateInsert(txtDate.Text.Trim()), Convert.ToDecimal(txtTareWeight.Text.Trim()),
                        Convert.ToDecimal(txtGrWeight.Text.Trim()), Convert.ToInt64(txtTotalPkgs.Text.Trim()), CommonBLL.DateInsert(txtDOIDt.Text), new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, dt, txtComments.Text.Trim(), new Guid(Session["CompanyId"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill of Lading", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("~/Invoices/BillOfLadingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill of Lading", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoicse/ErrorLog"), "Bill of Lading", "Error while updating.");
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No container details to save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //ModalPopupExtender1.Hide();
        //        //ddlFreight.SelectedValue = "0";
        //        //ddlPmtPop.SelectedValue = "0";
        //        //txtAmt.Text = "";
        //        //txtChqno.Text = "";
        //        //txtChqDt.Text = "";
        //        //txtRtgsCd.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
        //    }            
        //}

        //protected void btnDone_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ModalPopupExtender1.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
        //    }
        //}

        # endregion

        # region WebMethods Grid

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string AddNewRow()
        {
            DataTable dt = (DataTable)Session["ContainerDetails"];
            try
            {
                dt.Rows.Add(dt.NewRow());
                dt.AcceptChanges();
                Session["ContainerDetails"] = dt;
                return FillContainerDetails();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return FillContainerDetails();
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(string RowNo, string CType, string CNo, string CSealNo, string LSealNo)
        {
            DataTable dt = (DataTable)Session["ContainerDetails"];
            try
            {
                int RNo = Convert.ToInt32(RowNo) - 1;

                dt.Rows[RNo]["Ctype"] = CType;
                dt.Rows[RNo]["CNO"] = CNo;
                dt.Rows[RNo]["CSealNo"] = CSealNo;
                dt.Rows[RNo]["LSealNo"] = LSealNo;
                dt.AcceptChanges();
                Session["ContainerDetails"] = dt;
                return FillContainerDetails();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return FillContainerDetails();
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string DeleteRecord(string RNo)
        {
            DataTable dt = (DataTable)Session["ContainerDetails"];
            try
            {
                int RowNo = Convert.ToInt32(RNo) - 1;
                dt.Rows.RemoveAt(RowNo);
                dt.AcceptChanges();
                Session["ContainerDetails"] = dt;
                return FillContainerDetails();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return FillContainerDetails();
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool GetBOLN(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckEnquiryNo('S', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }
        # endregion
    }
}
