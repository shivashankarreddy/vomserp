using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Text;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class ShipmentPlanningDirect : System.Web.UI.Page
    {

        # region Variables

        static string FPOids = "";
        static string CustID = "";
        decimal TotalNetWeight = 0;
        decimal TotalGrWeight = 0;
        int TotalPkgs = 0;

        NewFPOBLL NFPOBLL = new NewFPOBLL();
        ErrorLog ELog = new ErrorLog();
        CustomerBLL CBLL = new CustomerBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        static DataTable dt = new DataTable();
        ShipmentPlanningDirectBLL SHPBLL = new ShipmentPlanningDirectBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";

        static Dictionary<string, string> FPOids1 = new Dictionary<string, string>();
        static Dictionary<string, string> CustID1 = new Dictionary<string, string>();
        string ShipMntMode = "";

        # endregion

        # region Page Load Events

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    txtTrmDlryPmnt.Attributes.Add("readonly", "readonly");
                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    Ajax.Utility.RegisterTypeForAjax(typeof(ShipmentPlanningDirect));
                    if (!IsPostBack)
                    {
                        GetData();

                        string TermsOfDeliveryPyment = "Routing of Payment:" + System.Environment.NewLine +
                            "Intermediary  Institution  : CITIBANK, NEW YORK," + System.Environment.NewLine +
                            "SWIFT CODE                 : CITIUS33" + System.Environment.NewLine +
                            "Account with Institution   : ANDHRA BANK, MUMBAI" + System.Environment.NewLine +
                            "Account Number             : 36153818" + System.Environment.NewLine +
                            "SWIFT CODE                 : ANDBINBB" + System.Environment.NewLine +
                            "BENEFICIARY NAME & ADDRESS : VOLTA IMPEX PRIVATE  LIMITED" + System.Environment.NewLine +
                            "Bank Name and address      : ANDHRA BANK, S R NAGAR BRANCH, HYDERABAD– 500038 INDIA" + System.Environment.NewLine +
                            "Account Number             : 052211011007863" + System.Environment.NewLine +
                            "IFSC CODE of Branch        : ANDB0000522" + System.Environment.NewLine +
                            "F.Ex: Dealer & Code        : ANDHRA BANK, OSB,PUNJAGUTTA, HYDERABAD 500 038" + System.Environment.NewLine +
                            "IFSC CODE of Branch        : ANDB0000667  " + System.Environment.NewLine +
                            "A   D Code No.             : 0340872 -8000009";
                        txtTrmDlryPmnt.Text = TermsOfDeliveryPyment;
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
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
        /// Get Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                ClaerAll();
                Customers();
                
                BindDropDownList(ddlNtfy, CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlPlcOrgGds, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofOrigin));
                BindDropDownList(ddlPlcFnlDstn, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofFinalDestination));
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading));
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge));
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery));
                BindDropDownList(ddlIncoTrm, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindRadioList(rbtnshpmnt, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode));
                GetCount();
                if (Request.QueryString["ID"] != null)
                    EditRecord(new Guid(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Count
        /// </summary>
        private string GetCount()
        {
            string Count = "1";
            try
            {
                DataSet ds = new DataSet();
                ds = CLBLL.SelectChekcListDtls(CommonBLL.FlagJSelect, new Guid(rbtnshpmnt.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0)
                    Count = ds.Tables[0].Rows[0]["cCount"].ToString();

                ShipMntMode = rbtnshpmnt.SelectedItem.Text == "By Sea" ? "Sea" : "Air";
                txtRefNo.Text = Session["AliasName"].ToString() + "/" + ShipMntMode + "/"
                                + Count + "/" + CommonBLL.FinacialYearShort;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
            return Session["AliasName"].ToString() + "/" + ShipMntMode + "/" + Count + "/" + CommonBLL.FinacialYearShort;
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        private void ClaerAll()
        {
            dt = null;
            CustID = "";
            txtRefNo.Text = "";
            CustID1 = null;
            TotalNetWeight = 0;
            TotalGrWeight = 0;
            TotalPkgs = 0;
            CustID1 = null;
            ViewState["LPOs"] = "";
            Session["SPDItems"] = null;
            //ListBoxCustomer.ClearSelection();
            //ListBoxFpos.Items.Clear();
        }

        /// <summary>
        /// Bind Customer List Box
        /// </summary>
        private void Customers()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ListBoxCustomer.DataSource = ds.Tables[0];
                    ListBoxCustomer.DataTextField = "Description";
                    ListBoxCustomer.DataValueField = "ID";
                    ListBoxCustomer.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind FPO List Box
        /// </summary>
        private void BindListboxFpo(ListBox ls, DataSet dt)
        {
            try
            {
                if (dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
                {
                    ListBoxCustomer.DataSource = dt.Tables[0];
                    ListBoxCustomer.DataTextField = "Description";
                    ListBoxCustomer.DataValueField = "ID";
                    ListBoxCustomer.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Radio List Buttons
        /// </summary>
        protected void BindRadioList(RadioButtonList rbl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    rbl.DataSource = CommonDt.Tables[0];
                    rbl.DataTextField = "Description";
                    rbl.DataValueField = "ID";
                    rbl.DataBind();
                    rbl.SelectedValue = (rbl.Items.FindByText("By Sea")).Value;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Get Check List Items
        /// </summary>
        /// <param name="GDNS"></param>
        /// <param name="GRNS"></param>
        private void GetCheckListItems(string GDNS, string GRNS)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = CLBLL.GetData(CommonBLL.FlagYSelect, Guid.Empty, "", GRNS, GDNS, Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty,
                    DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0)
                {
                    dt = null;
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    DataRow dr;
                    int PkgsCount = 1;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        if (ds.Tables[0].Rows[i]["ID"].ToString() != "")
                        {
                            long CstmrID = Convert.ToInt64(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                            dr["CustomerID"] = CstmrID;
                            dr["GDNID"] = Convert.ToInt64(ds.Tables[0].Rows[i]["ID"].ToString());
                            int AddGDNPkgs = (ds.Tables[0].Rows[i]["PackagesNo"].ToString() == "" ? 0
                                : Convert.ToInt32(ds.Tables[0].Rows[i]["PackagesNo"].ToString()));
                            if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                                dr["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                            else
                                dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);
                            PkgsCount += AddGDNPkgs;
                            dr["SupplierID"] = Convert.ToInt64(ds.Tables[0].Rows[i]["SupplierID"].ToString());
                            dr["SupplierNm"] = ds.Tables[0].Rows[i]["supplierNm"].ToString();
                            //(txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                            dr["NoOfPkgs"] = (ds.Tables[0].Rows[i]["PackagesNo"].ToString() == "" ? 0
                                : Convert.ToInt32(ds.Tables[0].Rows[i]["PackagesNo"].ToString()));
                            dr["FPONOs"] = ds.Tables[0].Rows[i]["FPONos"].ToString();
                            dr["FPOs"] = ds.Tables[0].Rows[i]["FPOs"].ToString();
                            dr["LR_GodownNo"] = ds.Tables[0].Rows[i]["GDNNo"].ToString(); // GodownNo
                            dr["IsARE1"] = true;//Convert.ToBoolean(ds.Tables[0].Rows[i]["GDN_No"].ToString());
                            dr["NetWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["NetWeight"].ToString());
                            dr["GrWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["GrossWeight"].ToString());
                            dr["aType"] = "GDN";
                        }
                        else
                            dr["GDNID"] = 0;
                        dr["Remarks"] = ds.Tables[0].Rows[i]["Remarks"].ToString();
                        dt.Rows.Add(dr);
                    }
                    for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                    {
                        dr = dt.NewRow();
                        if (ds.Tables[1].Rows[j]["ID"].ToString() != "")
                        {
                            dr["CustomerID"] = Convert.ToInt64(ds.Tables[1].Rows[j]["CustomerID"].ToString());
                            dr["GRNID"] = Convert.ToInt64(ds.Tables[1].Rows[j]["ID"].ToString());
                            int AddPkgs = Convert.ToInt32(ds.Tables[1].Rows[j]["PackagesNo"].ToString());
                            if (AddPkgs == 1 || AddPkgs == 0)
                                dr["PkgNos"] = (PkgsCount + AddPkgs - 1);
                            else
                                dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddPkgs - 1);
                            PkgsCount += AddPkgs;
                            dr["SupplierID"] = Convert.ToInt64(ds.Tables[1].Rows[j]["SupplierID"].ToString());
                            dr["SupplierNm"] = ds.Tables[1].Rows[j]["supplierNm"].ToString();
                            //dr["NoOfPkgs"] = Convert.ToInt32(ds.Tables[1].Rows[i]["GRN_PackagesNo"].ToString());
                            dr["NoOfPkgs"] = (ds.Tables[1].Rows[j]["PackagesNo"].ToString() == "" ? 0 : Convert.ToInt32(ds.Tables[1].Rows[j]["PackagesNo"].ToString()));
                            dr["FPONOs"] = ds.Tables[1].Rows[j]["FPONos"].ToString();
                            dr["FPOs"] = ds.Tables[1].Rows[j]["FPOs"].ToString();
                            dr["LR_GodownNo"] = ds.Tables[1].Rows[j]["GRNNo"].ToString();
                            dr["IsARE1"] = true;//Convert.ToBoolean(ds.Tables[1].Rows[i]["GRN_No"].ToString());
                            dr["NetWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[j]["NetWeight"].ToString());
                            dr["GrWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[j]["GrossWeight"].ToString());
                            dr["aType"] = "GRN";
                        }
                        else
                            dr["GRNID"] = 0;
                        dr["Remarks"] = ds.Tables[1].Rows[j]["Remarks"].ToString();
                        dt.Rows.Add(dr);
                    }
                    if (dt != null && dt.Rows.Count > 0) { }
                    //    gvCheckedList.DataSource = dt;
                    //else
                    //    gvCheckedList.DataSource = null;
                    //gvCheckedList.DataBind();


                    //DataSet dss = new DataSet();
                    //dss = CLBLL.GetData(CommonBLL.FlagKSelect, 0, "", GRNS, GDNS, 0, "", "", "", 0,
                    //    DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTCheckedList());
                    //if (dss.Tables.Count > 0)
                    //{
                    //    lbfpos.DataSource = dss;
                    //    lbfpos.DataTextField = "Description";
                    //    lbfpos.DataValueField = "ID";
                    //    lbfpos.DataBind();
                    //}
                    //else                    
                    //    lbfpos.Items.Clear();

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Check List Items Edit
        /// </summary>
        /// <param name="ds"></param>
        private void GetCheckListItemsEdit(DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    dt = null;
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    DataRow dr;
                    int PkgsCount = 1;
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        long CstmrID = Convert.ToInt64(ds.Tables[1].Rows[i]["CustomerID"].ToString());
                        dr["CustomerID"] = CstmrID;
                        int AddGDNPkgs = (ds.Tables[1].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[1].Rows[i]["NoOfPkgs"].ToString()));
                        if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                            dr["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                        else
                            dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);
                        PkgsCount += AddGDNPkgs;
                        dr["SupplierID"] = Convert.ToInt64(ds.Tables[1].Rows[i]["SupplierID"].ToString());
                        dr["SupplierNm"] = ds.Tables[1].Rows[i]["SupplierNm"].ToString();
                        dr["NoOfPkgs"] = (ds.Tables[1].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[1].Rows[i]["NoOfPkgs"].ToString()));
                        dr["FPONOs"] = ds.Tables[1].Rows[i]["FPONos"].ToString();
                        dr["FPOs"] = ds.Tables[1].Rows[i]["FPOs"].ToString();
                        dr["LR_GodownNo"] = ds.Tables[1].Rows[i]["LR_GodownNo"].ToString();
                        dr["IsARE1"] = Convert.ToBoolean(ds.Tables[1].Rows[i]["IsARE1"].ToString());
                        dr["NetWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[i]["NetWeight"].ToString());
                        dr["GrWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[i]["GrWeight"].ToString());
                        dr["aType"] = ds.Tables[1].Rows[i]["aType"].ToString();
                        dr["Remarks"] = ds.Tables[1].Rows[i]["Remarks"].ToString();

                        if (ds.Tables[1].Rows[i]["GDNID"].ToString() != "")
                            dr["GDNID"] = Convert.ToInt64(ds.Tables[1].Rows[i]["GDNID"].ToString());
                        else
                            dr["GDNID"] = 0;
                        if (ds.Tables[1].Rows[i]["GRNID"].ToString() != "")
                            dr["GRNID"] = Convert.ToInt64(ds.Tables[1].Rows[i]["GRNID"].ToString());
                        else
                            dr["GRNID"] = 0;
                        dt.Rows.Add(dr);
                    }

                    if (dt != null && dt.Rows.Count > 0) { }
                    //    gvCheckedList.DataSource = dt;
                    //else
                    //    gvCheckedList.DataSource = null;
                    //gvCheckedList.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
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
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to Convert GridView to DataTable
        /// </summary>
        /// <param name="gvCT1"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView gvCheckList)
        {
            int i = 0;
            foreach (GridViewRow row in gvCheckList.Rows)
            {
                dt.Rows[i]["Remarks"] = ((TextBox)row.FindControl("txtRemarks")).Text.Trim();
                i++;
            }
            dt.AcceptChanges();
            return dt;
        }

        /// <summary>
        /// Edit Record
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = SHPBLL.GetDataSet(CommonBLL.FlagModify, ID, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    rbtnshpmnt.ClearSelection();
                    Customers();
                    CustID = ds.Tables[0].Rows[0]["CustomerID"].ToString();
                    CustID1 = CustID.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    ListBoxCustmrSelected();
                    GetFPOs();
                    rbtnshpmnt.SelectedValue = ds.Tables[0].Rows[0]["ShipmentMode"].ToString();//.ToUpper();
                    txtImpInstructions.Text = ds.Tables[0].Rows[0]["ImpInstructions"].ToString();
                    txtRefNo.Text = ds.Tables[0].Rows[0]["ChkLstRefNo"].ToString();
                    txtSupInv.Text = ds.Tables[0].Rows[0]["SupINvoiceNos"].ToString();
                    GetCheckListItemsEdit(ds);

                    # region Header

                    //txtPrfmInvce.Text = ds.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    //txtPIDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[0]["PrfmInvcDt"].ToString()));
                    txtOtrRfs.Text = ds.Tables[0].Rows[0]["OtherRef"].ToString();
                    ddlNtfy.SelectedValue = ds.Tables[0].Rows[0]["Notify"].ToString();
                    ddlPlcOrgGds.SelectedValue = ds.Tables[0].Rows[0]["CntryOrgGds"].ToString();
                    ddlPlcFnlDstn.SelectedValue = ds.Tables[0].Rows[0]["CntryFnlDstn"].ToString();
                    ddlPrtLdng.SelectedValue = ds.Tables[0].Rows[0]["PrtLdng"].ToString();
                    ddlPrtDscrg.SelectedValue = ds.Tables[0].Rows[0]["PrtDschrg"].ToString();
                    ddlPlcDlry.SelectedValue = ds.Tables[0].Rows[0]["PrtDlry"].ToString();
                    txtPCrBy.Text = ds.Tables[0].Rows[0]["PreCrirBy"].ToString();
                    txtPlcRcptPCr.Text = ds.Tables[0].Rows[0]["PlcRcptPCrirBy"].ToString();
                    txtVslFlt.Text = ds.Tables[0].Rows[0]["VslFltNo"].ToString();
                    txtTrmDlryPmnt.Text = ds.Tables[0].Rows[0]["TrmsDlryPymnts"].ToString();
                    ddlIncoTrm.Text = ds.Tables[0].Rows[0]["IncTrm"].ToString();
                    txtPriceBasis.Text = ds.Tables[0].Rows[0]["IncTrmLctn"].ToString();

                    # endregion


                    string Custid = ds.Tables[0].Rows[0]["CustomerId"].ToString();
                    string[] Cust = Custid.Split(',');
                    foreach (ListItem item in ListBoxCustomer.Items)
                    {
                        foreach (string s in Cust)
                        {
                            if (item.Value == s)
                            {
                                item.Selected = true;
                            }
                        }
                    }

                    //ListBoxCustmrSelected();

                    ListBoxFpos.DataSource = ds.Tables[1];
                    ListBoxFpos.DataTextField = "Description";
                    ListBoxFpos.DataValueField = "ID";
                    ListBoxFpos.DataBind();



                    //string fpoid = ds.Tables[1].Rows[0]["FPOs"].ToString();
                    //string[] fponmbrs = fpoid.Split(',');
                    foreach (ListItem item in ListBoxFpos.Items)
                        item.Selected = true;

                    string FPNo = String.Join(",", ListBoxFpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    Session["SelectedFPOs"] = FPNo;
                    //BindDropDownList(ddlSupllier, SHPBLL.GetData(CommonBLL.FlagASelect, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "", 0, DateTime.Now, 0, DateTime.Now, false, FPNo));                    
                    //ddlSupllier.SelectedValue = ds.Tables[1].Rows[0]["SupplierID"].ToString();
                    //txtPkgNos.Text = ds.Tables[1].Rows[0]["PkgNos"].ToString();
                    ////txtSuppNamePlace.Text = ds.Tables[1].Rows[0]["Supplier"].ToString();
                    //txtNoofPackages.Text = ds.Tables[1].Rows[0]["NoOfPkgs"].ToString();
                    //txtGodownReceipt.Text = ds.Tables[1].Rows[0]["LR_GodownNo"].ToString();
                    //txtCoveredUnder.Checked = Convert.ToBoolean(ds.Tables[1].Rows[0]["ISARE1"].ToString());
                    //txtNetWeightKgs.Text = ds.Tables[1].Rows[0]["NetWeight"].ToString();
                    //txtGrWeightKgs.Text = ds.Tables[1].Rows[0]["GrWeight"].ToString();
                    //txtRemarks.Text = ds.Tables[1].Rows[0]["Remarks"].ToString();

                    DataTable dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    dt.AcceptChanges();

                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["FPONOs"] = ds.Tables[2].Rows[i]["FPONOs"].ToString();
                        dr["FPOs"] = ds.Tables[2].Rows[i]["FPOs"].ToString();
                        dr["SupplierID"] = ds.Tables[2].Rows[i]["SupplierID"].ToString();
                        dr["SupplierNm"] = ds.Tables[2].Rows[i]["SupNm"].ToString();
                        dr["CustomerID"] = ds.Tables[2].Rows[i]["CustomerID"].ToString();
                        dr["NoOfPkgs"] = ds.Tables[2].Rows[i]["NoOfPkgs"].ToString();
                        dr["NetWeight"] = ds.Tables[2].Rows[i]["NetWeight"].ToString();
                        dr["GrWeight"] = ds.Tables[2].Rows[i]["GrWeight"].ToString();
                        dr["Remarks"] = ds.Tables[2].Rows[i]["Remarks"].ToString();
                        dr["LR_GodownNo"] = ds.Tables[2].Rows[i]["LR_GodownNo"].ToString();
                        dr["IsARE1"] = Convert.ToBoolean(ds.Tables[2].Rows[i]["IsARE1"].ToString());
                        dt.Rows.Add(dr);
                    }
                    dt.AcceptChanges();
                    Session["SPDItems"] = dt;
                    divItems.InnerHtml = FillItemGrid(false);

                    ViewState["EditID"] = ID;
                    btnSave.Text = "Update";
                    DivComments.Visible = true;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box Selected Customers
        /// </summary>
        private void ListBoxCustmrSelected()
        {
            try
            {
                CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = new DataSet();
                if (Request.QueryString["ID"] == null)
                    ds = CLBLL.GetData(CommonBLL.FlagZSelect, Guid.Empty, CustID, "", "", Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                else
                    ds = CLBLL.GetData(CommonBLL.FlagPSelectAll, Guid.Empty, CustID, "", "", Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0 && ds.Tables[2].Columns.Contains("Description"))
                {
                    ListBoxFpos.DataSource = ds.Tables[2];
                    ListBoxFpos.DataTextField = "Description";
                    ListBoxFpos.DataValueField = "ID";
                    ListBoxFpos.DataBind();

                    //ViewState["LPOs"] = ViewState["LPOs"] + "," + string.Join(",", ds.Tables[1].AsEnumerable().Select(r => r.Field<string>("LPOs")).ToArray());

                    //ViewState["GRNInv"] = string.Join(",", ds.Tables[1].AsEnumerable().Select(r => r.Field<string>("GRNInv")).ToArray());
                }
                else
                    ListBoxFpos.Items.Clear();

                if (Request.QueryString["ID"] == null)
                {
                    CustID1 = CustID.Split(',').ToDictionary(key => key, value => value);
                    GetCheckListItems("", "");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        private void GetFPOs()
        {
            try
            {
                DataSet dss = new DataSet();
                dss = CLBLL.GetData(CommonBLL.FlagKSelect, Guid.Empty, "", "", "", Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty,
                    DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    ListBoxFpos.DataSource = dss;
                    ListBoxFpos.DataBind();
                    ListBoxFpos.DataTextField = "Description";
                    ListBoxFpos.DataValueField = "ID";

                }
                else
                    ListBoxFpos.Items.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        private string FillItemGrid(bool Add)
        {
            try
            {
                DataTable dtItems = (DataTable)Session["SPDItems"];
                if (dtItems == null)
                    dtItems = CommonBLL.EmptyDTCheckedList();
                StringBuilder sb = new StringBuilder();
                if (!Add)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' " + " id='tblSPDItems'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th>SNo</th><th>FPO No.</th><th>Supplier Name</th><th>No of Pkgs</th>" +
                        "<th>Pkg.Nos</th><th>Godown Receipt No.</th><th>Is ARE-1</th><th>Net Weight Kgs</th><th>Gr Weight Kgs</th><th>Remarks</th><th></th><th></th></tr>");
                    sb.Append("</thead><tbody>");
                }

                #region Body
                if (dtItems != null && dtItems.Rows.Count > 0 && dtItems.Rows[0]["FPONOs"].ToString() != "")
                {
                    int PkgsCount = 1;
                    for (int i = 0; i < dtItems.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["FPOs"] + " <input type='hidden' id='hfFPOID" + SNo + "' value='" + dtItems.Rows[i]["FPONOs"].ToString() + "'></td>");
                        sb.Append("<td>" + dtItems.Rows[i]["SupplierNm"] + " <input type='hidden' id='hfSUPID" + SNo + "' value='" + dtItems.Rows[i]["SupplierID"].ToString() + "'></td>");

                        int AddGDNPkgs = (dtItems.Rows[i]["NoOfPkgs"].ToString() == "" ? 0
                                : Convert.ToInt32(dtItems.Rows[i]["NoOfPkgs"].ToString()));
                        if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                            dtItems.Rows[i]["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                        else
                            dtItems.Rows[i]["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);
                        PkgsCount += AddGDNPkgs;

                        sb.Append("<td>" + dtItems.Rows[i]["NoOfPkgs"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["PkgNos"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["LR_GodownNo"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["IsARE1"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["NetWeight"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["GrWeight"] + "</td>");
                        sb.Append("<td>" + dtItems.Rows[i]["Remarks"] + "</td>");

                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnDel" + SNo + "' href='javascript:void(0)' " +
                            " onclick='javascript:return doConfirm(" + i.ToString() + ")' class='icons deleteicon' title='Delete'>" +
                            " <img src='../images/Delete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");

                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnEdit" + SNo + "' href='javascript:void(0)' " +
                            " onclick='javascript:return EditSelectedItem(" + i.ToString() + ")' class='icons deleteicon' title='Edit'>" +
                            " <img src='../images/Edit.jpeg' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");

                        sb.Append("</tr>");
                    }
                }
                #endregion

                if (!Add)
                {
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th><label id='lblEditID0'></label></th>");
                    sb.Append("<th>");
                    #region Fill FPOs Dropdown

                    DataSet dss = new DataSet();
                    string FPOs = Session["SelectedFPOs"].ToString();
                    if (FPOs != "")
                        dss = SHPBLL.GetDataSet(CommonBLL.FlagZSelect, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                            DateTime.Now, FPOs, new Guid(Session["CompanyID"].ToString()));

                    sb.Append("<select id='ddlFPO0' Class='bcAspdropdown' onchange='FillSuppliers(0)' style='width:85px;'>");
                    sb.Append("<option value='0'>-SELECT-</option>");
                    foreach (DataRow row in dss.Tables[0].Rows)
                        sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                    sb.Append("</select>");
                    #endregion
                    sb.Append("</th>");

                    sb.Append("<th>");
                    #region Fill Supplier Dropdown

                    sb.Append("<select id='ddlSup0' Class='bcAspdropdown' style='width:150px'>");
                    sb.Append("<option value='0'>-SELECT-</option>");
                    sb.Append("</select>");
                    sb.Append("<input type='hidden' id='hfCustID0' value=''>");

                    #endregion
                    sb.Append("</th>");

                    sb.Append("<th><input type='text' id='txtNoOfPkgs0' class='bcAsptextbox' onblur='extractNumber(this,2,false);' "
                        + " style='height:20px; width:50px; resize:none;'"
                        + " onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);'></th>");
                    sb.Append("<th></th>");
                    sb.Append("<th><input type='text' id='txtGodownRcptNo0' class='bcAsptextbox'></th>");
                    sb.Append("<th><input type='checkbox' id='ChkIsAreOne0' class='bcAsptextboxmulti_Footer' style='height:20px; width:50px; resize:none;'></th>");
                    sb.Append("<th><input type='text' id='txtNetWeight0' onblur='extractNumber(this,2,false);' style='height:20px; width:50px; resize:none;'"
                        + "onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' class='bcAsptextbox'></th>");
                    sb.Append("<th><input type='text' id='txtGrWeight0' onblur='extractNumber(this,2,false);' style='height:20px; width:50px; resize:none;'"
                        + "onkeyup='extractNumber(this,2,false);' onchange='return CheckWeights(event)' onkeypress='return blockNonNumbers(this, event, true, false);' class='bcAsptextbox'></th>");
                    sb.Append("<th><textarea id='txtRemarks0' class='bcAsptextbox' onfocus='ExpandTXT(this.id)' "
                                    + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;'></textarea></th>");

                    sb.Append("<th>");
                    sb.Append("<span class='gridactionicons'><a id='btnaddItem' href='javascript:void(0)'" +
                                        " onclick='AddItemRow(0)'class='icons additionalrow' title='Add Row' style='display:display;'>"
                                        + "<img src='../images/add.jpeg' style='border-style: none;'/></a></span>");
                    sb.Append("<span class='gridactionicons'><a id='btnEditItem' href='javascript:void(0)' " +
                                                " onclick='UpdateSelectedItem()' class='icons deleteicon' style='display:none;'" +
                                                " title='Edit selected item' >" +
                                                " <img src='../images/Edit.jpeg' style='border-style: none;'/></a></span>");//OnClientClick='javascript:return doConfirm();'
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("<span class='gridactionicons'><a id='btnCancel' href='javascript:void(0)' " +
                                        " onclick='CancelEdit()'class='icons additionalrow' title='Cancel'>"
                                        + "<img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                    sb.Append("</th></tr>");
                    sb.Append("</tfoot></table>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ex.Message;
            }
        }

        # endregion

        # region ListBox Events

        /// <summary>
        /// List Box Customers Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxCustmrSelected();
        }

        /// <summary>
        /// List Box FPO Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxFpo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string FPNo = String.Join(",", ListBoxFpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Session["SelectedFPOs"] = FPNo;
                divItems.InnerHtml = FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        # endregion

        # region RadioButton Change

        /// <summary>
        /// Shipment Radio Button Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbtnshpmnt_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCount();
        }

        # endregion

        # region ButtonClick

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int res = 1; Filename = FileName();
            string FPONOs = String.Join(",", ListBoxFpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            try
            {
                DataTable dt = (DataTable)Session["SPDItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (btnSave.Text == "Save")
                    {
                        string ChkLstNo = GetCount();
                        res = SHPBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, CustID, new Guid(rbtnshpmnt.SelectedValue),
                            txtImpInstructions.Text.Trim(), ChkLstNo, "", DateTime.Now, txtOtrRfs.Text, new Guid(ddlNtfy.SelectedValue),
                            new Guid(ddlPlcOrgGds.SelectedValue),
                            new Guid(ddlPlcFnlDstn.SelectedValue), new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue),
                            new Guid(ddlPlcDlry.SelectedValue), txtPCrBy.Text, txtPlcRcptPCr.Text, txtVslFlt.Text, txtTrmDlryPmnt.Text,
                            new Guid(ddlIncoTrm.Text), txtPriceBasis.Text, txtSupInv.Text.Trim(), "", Guid.Empty, 0, "", false, 0, 0, "", "",
                            new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, FPONOs, dt,
                            new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Shipment Planning for FPO", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("../Invoices/ShipmentPlanningDirectStatus.aspx", false);
                        }
                        else if (res != 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Shipment Planning for FPO", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", "Error while Inserting.");
                            ViewState["LPOs"] = "";
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        res = SHPBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(Request.QueryString["ID"]), CustID,
                            new Guid(rbtnshpmnt.SelectedValue), txtImpInstructions.Text.Trim(), txtRefNo.Text.Trim(),
                            "", DateTime.Now, txtOtrRfs.Text, new Guid(ddlNtfy.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue),
                            new Guid(ddlPlcFnlDstn.SelectedValue),
                            new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue), new Guid(ddlPlcDlry.SelectedValue),
                            txtPCrBy.Text, txtPlcRcptPCr.Text, txtVslFlt.Text, txtTrmDlryPmnt.Text, new Guid(ddlIncoTrm.Text), txtPriceBasis.Text,
                            txtSupInv.Text.Trim(), "", Guid.Empty, 0, "", false, 0, 0, "", "", new Guid(Session["UserID"].ToString()),
                            DateTime.Now, Guid.Empty, DateTime.Now, true, FPONOs, dt, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Shipment Planning for FPO", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("../Invoices/ShipmentPlanningDirectStatus.aspx", false);
                        }
                        else if (res != 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Shipment Planning for FPO", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", "Error while Inserting.");
                            ViewState["LPOs"] = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Invoices/ShipmentPlanningDirect.aspx", false);
        }

        # endregion

        # region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillSuppliers(string FPOID)
        {
            try
            {
                DataSet dss = SHPBLL.GetDataSet(CommonBLL.FlagYSelect, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, FPOID, new Guid(Session["CompanyID"].ToString()));
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                if (dss != null && dss.Tables.Count > 0)
                {
                    foreach (DataRow dr in dss.Tables[0].Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dss.Tables[0].Columns)
                            row.Add(col.ColumnName, dr[col]);
                        rows.Add(row);
                    }
                }

                string CustID = "";
                if (dss.Tables.Count > 1)
                {
                    CustID = dss.Tables[1].Rows[0]["CusmorId"].ToString();
                }
                return serializer.Serialize(rows) + "~^~" + CustID;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItem(string FPOID, string FPONo, string SupID, string SupNo, string CustID, string NoOfPkgs, string GodonRcptNo, string IsARE1, string Netweight, string GrWeight, string remarks)
        {
            try
            {
                DataTable dt = (DataTable)Session["SPDItems"];
                if (dt == null)
                {
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    dt.AcceptChanges();
                }
                DataRow dr = dt.NewRow();
                if (new Guid(FPOID) != Guid.Empty)
                {
                    dr["FPONOs"] = FPOID;
                    dr["FPOs"] = FPONo;
                }
                if (new Guid(SupID) != Guid.Empty)
                {
                    dr["SupplierID"] = SupID;
                    dr["SupplierNm"] = SupNo;
                }
                if (CustID != "")
                    dr["CustomerID"] = CustID;
                if (Convert.ToInt32(NoOfPkgs) != 0)
                    dr["NoOfPkgs"] = NoOfPkgs;
                if (Convert.ToDecimal(Netweight) > 0)
                    dr["NetWeight"] = Netweight;
                if (Convert.ToDecimal(GrWeight) > 0)
                    dr["GrWeight"] = GrWeight;
                if (remarks != "")
                    dr["Remarks"] = remarks;
                if (GodonRcptNo != "")
                    dr["LR_GodownNo"] = GodonRcptNo;
                dr["IsARE1"] = Convert.ToBoolean(IsARE1);
                dt.Rows.Add(dr);

                Session["SPDItems"] = dt;
                return FillItemGrid(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string UpdateSelectedItem(string RowID, string FPOID, string FPONo, string SupID, string SupNo, string CustID, string NoOfPkgs, string GodonRcptNo, string IsARE1, string Netweight, string GrWeight, string remarks)
        {
            try
            {
                int RowNo = Convert.ToInt32(RowID) - 1;
                DataTable dt = (DataTable)Session["SPDItems"];
                if (new Guid(FPOID) != Guid.Empty)
                {
                    dt.Rows[RowNo]["FPONOs"] = FPOID;
                    dt.Rows[RowNo]["FPOs"] = FPONo;
                }
                if (new Guid(SupID) != Guid.Empty)
                {
                    dt.Rows[RowNo]["SupplierID"] = SupID;
                    dt.Rows[RowNo]["SupplierNm"] = SupNo;
                }
                if (CustID != "")
                    dt.Rows[RowNo]["CustomerID"] = CustID;
                if (Convert.ToInt32(NoOfPkgs) != 0)
                    dt.Rows[RowNo]["NoOfPkgs"] = NoOfPkgs;
                if (Convert.ToDecimal(Netweight) > 0)
                    dt.Rows[RowNo]["NetWeight"] = Netweight;
                if (Convert.ToDecimal(GrWeight) > 0)
                    dt.Rows[RowNo]["GrWeight"] = GrWeight;
                if (remarks != "")
                    dt.Rows[RowNo]["Remarks"] = remarks;
                if (GodonRcptNo != "")
                    dt.Rows[RowNo]["LR_GodownNo"] = GodonRcptNo;
                dt.Rows[RowNo]["IsARE1"] = Convert.ToBoolean(IsARE1);
                dt.AcceptChanges();

                Session["SPDItems"] = dt;
                return FillItemGrid(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string EditRow(string RowID)
        {
            try
            {
                DataTable dt = (DataTable)Session["SPDItems"];
                string Result = "";
                if (dt != null && dt.Rows.Count > 0 && RowID != "")
                {
                    int RowNo = Convert.ToInt32(RowID);
                    string CustID = dt.Rows[RowNo]["CustomerID"].ToString();
                    string NoOfPkgs = dt.Rows[RowNo]["NoOfPkgs"].ToString();
                    string GodownRcptNo = dt.Rows[RowNo]["LR_GodownNo"].ToString();
                    string IsARE1 = dt.Rows[RowNo]["IsARE1"].ToString();
                    string Netweight = dt.Rows[RowNo]["NetWeight"].ToString();
                    string GrWeight = dt.Rows[RowNo]["GrWeight"].ToString();
                    string Remarks = dt.Rows[RowNo]["Remarks"].ToString();

                    Result = CustID + "~^~" + NoOfPkgs + "~^~" + GodownRcptNo + "~^~" + IsARE1 + "~^~" + Netweight + "~^~" + GrWeight + "~^~" + Remarks;
                }

                return Result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItem(string RowID)
        {
            try
            {
                DataTable dt = (DataTable)Session["SPDItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    int RowNo = Convert.ToInt32(RowID);
                    dt.Rows[RowNo].Delete();
                    dt.AcceptChanges();
                    Session["SPDItems"] = dt;
                }
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning for FPO", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        # endregion
    }
}