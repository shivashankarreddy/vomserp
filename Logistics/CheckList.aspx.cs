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
using System.Text;
using System.Collections.Generic;
using System.IO;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Logistics
{
    public partial class CheckList : System.Web.UI.Page
    {
        # region Variables

        //static string GDNids = "";
        //static string GRNids = "";
        //static string FPOIDs = "";
        //static string CustID = "";

        decimal TotalNetWeight = 0;
        decimal TotalGrWeight = 0;
        int TotalPkgs = 0;

        ErrorLog ELog = new ErrorLog();
        CustomerBLL CBLL = new CustomerBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        NewFPOBLL FPOBLL = new NewFPOBLL();
        //static DataTable dt = new DataTable();
        GrnBLL GRNBL = new GrnBLL();
        AuditLogs ALS = new AuditLogs();
        //string Filename = "";

        Dictionary<string, string> CustID1 = new Dictionary<string, string>();
        Dictionary<string, string> GDNids1 = new Dictionary<string, string>();
        Dictionary<string, string> GRNids1 = new Dictionary<string, string>();
        Dictionary<string, string> FPOIDs1 = new Dictionary<string, string>();
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
                    Ajax.Utility.RegisterTypeForAjax(typeof(CheckList));
                    if (!IsPostBack)
                    {
                        Session["VerbalFpoDetails"] = null;
                        Session["GRNDetails"] = null;
                        Session["GDNDetails"] = null;
                        Session["Suppliers"] = null;
                        GetData();

                        //string TermsOfDeliveryPyment = "Routing of Payment:" + System.Environment.NewLine +
                        //    "Intermediary  Institution  : CITIBANK, NEW YORK," + System.Environment.NewLine +
                        //    "SWIFT Code                   : CITIUS33" + System.Environment.NewLine +
                        //    "Account with Institution   : ANDHRA BANK, MUMBAI" + System.Environment.NewLine +
                        //    "Account Number             : 36153818" + System.Environment.NewLine +
                        //    "SWIFT Code                   : ANDBINBB" + System.Environment.NewLine +
                        //    "Beneficiary Name & Address : VOLTA IMPEX PRIVATE  LIMITED" + System.Environment.NewLine +
                        //    "Bank Name & address    : ANDHRA BANK, S R NAGAR BRANCH, HYDERABAD– 500038 INDIA" + System.Environment.NewLine +
                        //    "Account Number             : 052211011007863" + System.Environment.NewLine +
                        //    "IFSC Code of Branch      : ANDB0000522" + System.Environment.NewLine +
                        //    "F.Ex: Dealer & Code       : ANDHRA BANK, OSB,PUNJAGUTTA, HYDERABAD 500038" + System.Environment.NewLine +
                        //    "IFSC Code of Branch      : ANDB0000667  " + System.Environment.NewLine +
                        //    "A   D Code No.                : 0340872 -8000009";
                        string TermsOfDeliveryPyment = "Routing of Payment:" + System.Environment.NewLine +
                            "Intermediary  Institution  : CITIBANK, NEW YORK," + System.Environment.NewLine +
                            "SWIFT CODE                 : CITIUS33" + System.Environment.NewLine +
                            "Account with Institution   : ANDHRA BANK, MUMBAI" + System.Environment.NewLine +
                            "Account Number             : 36153818" + System.Environment.NewLine +
                            "SWIFT CODE                 : ANDBINBB003" + System.Environment.NewLine +
                            "BENEFICIARY NAME & ADDRESS : VOLTA IMPEX PRIVATE  LIMITED" + System.Environment.NewLine +
                            "Bank Name & address      : ANDHRA BANK, S R NAGAR BRANCH, HYDERABAD– 500038 INDIA" + System.Environment.NewLine +
                            "Account Number             : 052211011007863" + System.Environment.NewLine +
                            "IFSC CODE of Branch        : ANDB0000522" + System.Environment.NewLine +
                            "F.Ex: Dealer & Code        : ANDHRA BANK, OSB, KOTI, HYDERABAD 500095" + System.Environment.NewLine +
                            "IFSC CODE of Branch        : ANDB0000667  " + System.Environment.NewLine +
                            //"A   D Code No.             : 0340872 -8000009" + System.Environment.NewLine + //modified by Satya :: Rahman mail dated on 09-July-2020 
                            "A   D Code No.             : 02900FE" + System.Environment.NewLine +
                            "Period of Payment          : 180 Days";

                        //string TermsOfDeliveryPyment = "Routing of Payment:" + System.Environment.NewLine +
                        //                               "Intermediary  Institution : CITIBANK, NEW YORK," + System.Environment.NewLine +
                        //                               "SWIFT CODE : CITIUS33" + System.Environment.NewLine +
                        //                               "Account with Institution : ANDHRA BANK, MUMBAI" + System.Environment.NewLine +
                        //                               System.Environment.NewLine +
                        //                               "Account Number : 36153818" + System.Environment.NewLine +
                        //                               "SWIFT CODE : ANDBINBB" + System.Environment.NewLine +
                        //                               "BENEFICIARY NAME & ADDRESS : VOLTA IMPEX PRIVATE  LIMITED" + System.Environment.NewLine +
                        //                               System.Environment.NewLine +
                        //                               "Bank Name & address : ANDHRA BANK, S R NAGAR BRANCH, HYDERABAD– 500038 INDIA" + System.Environment.NewLine +
                        //                               "Account Number : 052211011007863" + System.Environment.NewLine +
                        //                               "IFSC CODE of Branch : ANDB0000522" + System.Environment.NewLine +
                        //                               System.Environment.NewLine +
                        //                               "F.Ex: Dealer & Code : ANDHRA BANK, OSB,PUNJAGUTTA, HYDERABAD 500 038" + System.Environment.NewLine +
                        //                               System.Environment.NewLine +
                        //                               "IFSC CODE of Branch : ANDB0000667" + System.Environment.NewLine +
                        //                               System.Environment.NewLine +
                        //                               "A D Code No. : 0340872 -8000009";

                        txtTrmDlryPmnt.Text = TermsOfDeliveryPyment;
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        /// <summary>
        /// Page Load Complete Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            //Label1.Text = "Customer : (" + CustID + ") GDN : (" + GDNids + ") GRN : (" + GRNids + ")";
        }

        # endregion

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
                BindRadioList(rbtnshpmnt, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode));
                pnlVerbalFPO.Visible = false;
                ClearAll();
                Customers();
                GetCount();
                BindDropDownList(ddlNtfy, CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlPlcOrgGds, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofOrigin));
                BindDropDownList(ddlPlcFnlDstn, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofFinalDestination));
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading));
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge));
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery));
                BindDropDownList(ddlIncoTrm, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                //HttpContext.Current.Session["Locations"] = EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.Location); 

                if (Request.QueryString["ID"] != null)
                    EditRecord(new Guid(Request.QueryString["ID"]));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Count
        /// </summary>
        private string GetCount()
        {
            string Count = "1";
            int Counts = 1;
            try
            {
                DataSet ds = new DataSet();
                ds = CLBLL.SelectChekcListDtls(CommonBLL.FlagJSelect, new Guid(rbtnshpmnt.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    //Count = ds.Tables[0].Rows[0]["cCount"].ToString();
                    if (ds.Tables[0].Rows[0]["cCount"].ToString() != "")
                    {  
                    string[] ChkLst_No = ds.Tables[0].Rows[0]["cCount"].ToString().Split('/');
                    if (ChkLst_No.Length > 3)
                        Counts = int.Parse(ChkLst_No[2]) + 1;
                    }
                }
                ShipMntMode = rbtnshpmnt.SelectedItem.Text == "By Sea" ? "Sea" : "Air";
                txtRefNo.Text = Session["AliasName"].ToString() + "/" + ShipMntMode + "/" + Counts + "/" + CommonBLL.FinacialYearShort;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
            return txtRefNo.Text; //Session["AliasName"].ToString() + "/" + ShipMntMode + "/" + Count + "/" + CommonBLL.FinacialYearShort;
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        private void ClearAll()
        {
            // dt = null;
            Session["GRNDetails"] = null;
            Session["GDNDetails"] = null;
            lstbxVerbalFPOIDs.Items.Clear();
            ListBoxGDN.Items.Clear();
            ListBoxGRN.Items.Clear();
            ListBoxCustomer.SelectedIndex = -1;

            //GDNids = "";
            //GRNids = "";
            //CustID = "";
            txtRefNo.Text = "";
            GDNids1 = null;
            GRNids1 = null;
            CustID1 = null;
            TotalNetWeight = 0;
            TotalGrWeight = 0;
            TotalPkgs = 0;
            CustID1 = null;
            GDNids1 = null;
            GRNids1 = null;
            ViewState["LPOs"] = "";
            txtSupInv.Text = "";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        private void GetVerbalFPOs(string CustomerID)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Request.QueryString.Count == 0 && Request.QueryString["ID"] == null)
                    ds = CLBLL.SelectChekcListDtls(CommonBLL.FlagLSelect, Guid.Empty, CustomerID, new Guid(Session["CompanyID"].ToString()));
                else if (Request.QueryString["ID"] != null)
                    ds = CLBLL.SelectChekcListDtls(CommonBLL.FlagCSelect, new Guid(Request.QueryString["ID"]), CustomerID, new Guid(Session["CompanyID"].ToString()));
                else
                    ds = CLBLL.SelectChekcListDtls(CommonBLL.FlagCSelect, Guid.Empty, CustomerID, new Guid(Session["CompanyID"].ToString()));
                {
                    lstbxVerbalFPOIDs.DataSource = ds.Tables[0];
                    lstbxVerbalFPOIDs.DataTextField = "Description";
                    lstbxVerbalFPOIDs.DataValueField = "ID";
                    lstbxVerbalFPOIDs.DataBind();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
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
                ds = CLBLL.GetData(CommonBLL.FlagYSelect, Guid.Empty, "", GRNS, GDNS, Guid.Empty, "", "", "", Guid.Empty,
                    DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));

                if (ds.Tables.Count > 0)
                {
                    // Guid CustIDforVerb = new Guid(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                    DataTable dt = null;
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    DataRow dr;
                    int PkgsCount = 1, SNO = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        if (ds.Tables[0].Rows[i]["ID"].ToString() != "")
                        {
                            SNO++;
                            if (ViewState["Edit_Rows"] == null)
                                dr["SNo"] = SNO;
                            Guid CstmrID = new Guid(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                            dr["CustomerID"] = CstmrID;
                            dr["GDNID"] = new Guid(ds.Tables[0].Rows[i]["ID"].ToString());
                            int AddGDNPkgs = (ds.Tables[0].Rows[i]["PackagesNo"].ToString() == "" ? 0
                                : Convert.ToInt32(ds.Tables[0].Rows[i]["PackagesNo"].ToString()));
                            //if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                            //    dr["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                            //else
                            dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);
                            PkgsCount += AddGDNPkgs;
                            dr["SupplierID"] = new Guid(ds.Tables[0].Rows[i]["SupplierID"].ToString());
                            dr["SupplierNm"] = ds.Tables[0].Rows[i]["supplierNm"].ToString();
                            dr["NoOfPkgs"] = (ds.Tables[0].Rows[i]["PackagesNo"].ToString() == "" ? 0
                                : Convert.ToInt32(ds.Tables[0].Rows[i]["PackagesNo"].ToString()));
                            dr["FPONOs"] = ds.Tables[0].Rows[i]["FPONos"].ToString();
                            dr["FPOs"] = ds.Tables[0].Rows[i]["FPOs"].ToString();
                            dr["LR_GodownNo"] = ds.Tables[0].Rows[i]["GDNNo"].ToString();
                            dr["IsARE1"] = false;
                            dr["NetWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["NetWeight"].ToString());
                            dr["GrWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["GrossWeight"].ToString());
                            dr["aType"] = "GDN";
                        }
                        else
                            dr["GDNID"] = Guid.Empty;

                        DataTable dtt = null;
                        string edit_remarks = "";
                        if (ViewState["Edit_Rows"] != null)
                        {
                            dtt = (DataTable)ViewState["Edit_Rows"];
                            var data = dtt.Select("GDNID='" + ds.Tables[0].Rows[i]["ID"].ToString() + "'");
                            if (data.Count() > 0)
                                edit_remarks = data[0]["remarks"].ToString();
                        }
                        dr["Remarks"] = edit_remarks == "" ? ds.Tables[0].Rows[i]["Remarks"].ToString() : edit_remarks;
                        dt.Rows.Add(dr);
                    }
                    for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                    {
                        dr = dt.NewRow();
                        if (ds.Tables[1].Rows[j]["ID"].ToString() != "")
                        {
                            SNO++;
                            if (ViewState["Edit_Rows"] == null)
                                dr["SNo"] = SNO;
                            dr["CustomerID"] = new Guid(ds.Tables[1].Rows[j]["CustomerID"].ToString());
                            dr["GRNID"] = new Guid(ds.Tables[1].Rows[j]["ID"].ToString());
                            int AddPkgs = Convert.ToInt32(ds.Tables[1].Rows[j]["PackagesNo"].ToString());
                            //if (AddPkgs == 1 || AddPkgs == 0)
                            //    dr["PkgNos"] = (PkgsCount + AddPkgs - 1);
                            //else
                            dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddPkgs - 1);
                            PkgsCount += AddPkgs;
                            dr["SupplierID"] = new Guid(ds.Tables[1].Rows[j]["SupplierID"].ToString());
                            dr["SupplierNm"] = ds.Tables[1].Rows[j]["supplierNm"].ToString();
                            dr["NoOfPkgs"] = (ds.Tables[1].Rows[j]["PackagesNo"].ToString() == "" ? 0 :
                                Convert.ToInt32(ds.Tables[1].Rows[j]["PackagesNo"].ToString()));
                            dr["FPONOs"] = ds.Tables[1].Rows[j]["FPONos"].ToString();
                            dr["FPOs"] = ds.Tables[1].Rows[j]["FPOs"].ToString();
                            dr["LR_GodownNo"] = ds.Tables[1].Rows[j]["GRNNo"].ToString();
                            dr["IsARE1"] = true;
                            dr["NetWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[j]["NetWeight"].ToString());
                            dr["GrWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[j]["GrossWeight"].ToString());
                            dr["aType"] = "GRN";
                        }
                        else
                            dr["GRNID"] = Guid.Empty;


                        DataTable dttgrn = null;
                        string edit_remarks = "";
                        if (ViewState["Edit_Rows"] != null)
                        {
                            dttgrn = (DataTable)ViewState["Edit_Rows"];
                            var data = dttgrn.Select("GRNID='" + ds.Tables[1].Rows[j]["ID"].ToString() + "'");
                            if (data.Count() > 0)
                                edit_remarks = data[0]["remarks"].ToString();
                        }
                        dr["Remarks"] = edit_remarks == "" ? ds.Tables[1].Rows[j]["Remarks"].ToString() : edit_remarks;
                        //dr["Remarks"] = ds.Tables[1].Rows[j]["Remarks"].ToString();
                        dt.Rows.Add(dr);
                    }

                    // if (lstbxVerbalFPOIDs.SelectedValue != "")
                    // {
                    if (Session["VerbalFpoDetails"] != null && ((DataTable)Session["VerbalFpoDetails"]).Rows.Count > 0)
                    {
                        DataTable dtVFPO = (DataTable)Session["VerbalFpoDetails"];
                        //FPOIDs = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        for (int k = 0; k < dtVFPO.Rows.Count; k++)
                        {
                            //dr = dt.NewRow();
                            if (dtVFPO.Rows[k]["VerbalFPOs"].ToString() != "" && dtVFPO.Rows[k]["SupplierID"].ToString() != Guid.Empty.ToString()
                                && dtVFPO.Rows[k]["NoOfPkgs"].ToString() != "" && dtVFPO.Rows[k]["LR_GodownNo"].ToString() != ""
                                && dtVFPO.Rows[k]["NetWeight"].ToString() != "0" && dtVFPO.Rows[k]["GrWeight"].ToString() != "")
                            {
                                dr = dt.NewRow();
                                SNO++;
                                if (ViewState["Edit_Rows"] == null)
                                    dr["SNo"] = SNO;
                                dr["CustomerID"] = new Guid(dtVFPO.Rows[k]["CustomerID"].ToString());
                                // if (txtNoofPkgs.Text != "")
                                // {
                                int AddPkgs = Convert.ToInt32(dtVFPO.Rows[k]["NoOfPkgs"].ToString());
                                //if (AddPkgs == 1 || AddPkgs == 0)
                                //    dr["PkgNos"] = (PkgsCount + AddPkgs - 1);
                                //else
                                dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddPkgs - 1);
                                PkgsCount += AddPkgs;
                                //}
                                dr["SupplierID"] = new Guid(dtVFPO.Rows[k]["SupplierID"].ToString());//new Guid(ddlVerbalSupplier.SelectedValue);
                                dr["SupplierNm"] = dtVFPO.Rows[k]["SupplierNm"].ToString();//ddlVerbalSupplier.SelectedItem.Text;
                                //if (txtNoofPkgs.Text != "")
                                dr["NoOfPkgs"] = Convert.ToInt32(dtVFPO.Rows[k]["NoOfPkgs"].ToString());
                                dr["VerbalFPOs"] = new Guid(dtVFPO.Rows[k]["VerbalFPOs"].ToString());//FPOIDs;
                                dr["FPONOs"] = dtVFPO.Rows[k]["FPONOs"].ToString();
                                dr["LR_GodownNo"] = dtVFPO.Rows[k]["LR_GodownNo"].ToString();//txtVerbalGodownReceiptNo.Text;
                                dr["IsARE1"] = Convert.ToBoolean(dtVFPO.Rows[k]["IsARE1"]);
                                //if (txtVerbalNetWeight.Text != "")
                                dr["NetWeight"] = Convert.ToDecimal(dtVFPO.Rows[k]["NetWeight"].ToString());
                                //if (txtVerbalGrWeight.Text != "")
                                dr["GrWeight"] = Convert.ToDecimal(dtVFPO.Rows[k]["GrWeight"].ToString());
                                dr["aType"] = "VerbalFPO";
                                dr["Remarks"] = dtVFPO.Rows[k]["Remarks"].ToString(); //txtRemarks.Text;
                                dt.Rows.Add(dr);
                            }
                            //dt.Rows.Add(dr);
                        }
                    }
                    //}
                    if (ViewState["Edit_Rows"] != null)
                    {
                        DataTable Dt = (DataTable)ViewState["Edit_Rows"];
                        Dt = Dt.Select(null, "sno").CopyToDataTable();
                        int TT = 0;
                        for (int Y = 0; Y < Dt.Rows.Count; Y++)
                        {
                            var GDNRow = dt.Select("GDNID='" + Dt.Rows[Y]["GDNID"] + "'", "sno");
                            if (GDNRow.Count() > 0)
                                GDNRow[0]["Sno"] = ++TT;
                            var GRNRow = dt.Select("GRNID='" + Dt.Rows[Y]["GRNID"] + "'", "sno");
                            if (GRNRow.Count() > 0)
                                GRNRow[0]["Sno"] = ++TT;
                            var VFPORow = dt.Select("VerbalFPOs='" + Dt.Rows[Y]["VerbalFPOs"] + "'", "sno");
                            if (VFPORow.Count() > 0)
                                VFPORow[0]["Sno"] = ++TT;
                        }
                        dt = dt.Select(null, "sno desc").CopyToDataTable();
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            if (dt.Rows[k]["sno"] == DBNull.Value)
                                dt.Rows[k]["sno"] = k + 1;

                        }
                        dt = dt.Select(null, "sno").CopyToDataTable();
                        int Total_Pkgs = 0;
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            if (k == 0)
                                dt.Rows[k]["PkgNos"] = "1 - " + Convert.ToInt16(dt.Rows[k]["NoOfPkgs"] ?? "0");
                            else
                                dt.Rows[k]["PkgNos"] = (Total_Pkgs + 1) + " - "
                                                    + ((Total_Pkgs + 1) + (Convert.ToInt16(dt.Rows[k]["NoOfPkgs"] ?? "0") - 1));
                            Total_Pkgs += Convert.ToInt16(dt.Rows[k]["NoOfPkgs"] ?? "0");
                        }
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (gvCheckedList != null)

                            gvCheckedList.DataSource = dt;
                    }
                    else
                        gvCheckedList.DataSource = null;
                    if (gvCheckedList != null)
                        gvCheckedList.DataBind();

                    if (ds.Tables[0].Rows.Count > 0)
                        txtSupInv.Text = ds.Tables[0].Rows[0]["InvoiceNo"].ToString();
                    else if (ds.Tables[1].Rows.Count > 0)
                        txtSupInv.Text = ds.Tables[1].Rows[0]["InvoiceNo"].ToString();

                    if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                        Session["GRNDetails"] = ds.Tables[2];

                    Session["GDNDetails"] = dt;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
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
                    DataTable dt = null;
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    DataRow dr;
                    int PkgsCount = 1;
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        dr["SNo"] = Convert.ToInt32(ds.Tables[1].Rows[i]["SNo"].ToString());
                        Guid CstmrID = new Guid(ds.Tables[1].Rows[i]["CustomerID"].ToString());
                        dr["CustomerID"] = CstmrID;
                        int AddGDNPkgs = (ds.Tables[1].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[1].Rows[i]["NoOfPkgs"].ToString()));
                        //if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                        //    dr["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                        //else
                        dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);//PkgsCount + " - " + (PkgsCount + AddGDN Pkgs - 1);
                        PkgsCount += AddGDNPkgs;

                        dr["SupplierID"] = new Guid(ds.Tables[1].Rows[i]["SupplierID"].ToString());
                        dr["SupplierNm"] = ds.Tables[1].Rows[i]["SupplierNm"].ToString();
                        dr["NoOfPkgs"] = (ds.Tables[1].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[1].Rows[i]["NoOfPkgs"].ToString()));
                        //dr["FPONOs"] = ds.Tables[1].Rows[i]["FPONos"].ToString();
                        if (ds.Tables[1].Rows[i]["FPONos"].ToString() == "")
                            dr["FPONOs"] = ds.Tables[1].Rows[i]["VerbalFPOsName"].ToString();
                        else
                            dr["FPONOs"] = ds.Tables[1].Rows[i]["FPONos"].ToString();
                        dr["FPOs"] = ds.Tables[1].Rows[i]["FPOs"].ToString();
                        // dr["VerbalFPOsName"] = ds.Tables[1].Rows[i]["VerbalFPOsName"].ToString();
                        dr["VerbalFPOs"] = ds.Tables[1].Rows[i]["VerbalFPOs"].ToString();
                        dr["LR_GodownNo"] = ds.Tables[1].Rows[i]["LR_GodownNo"].ToString();
                        dr["IsARE1"] = Convert.ToBoolean(ds.Tables[1].Rows[i]["IsARE1"].ToString());
                        dr["NetWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[i]["NetWeight"].ToString());
                        dr["GrWeight"] = Convert.ToDecimal(ds.Tables[1].Rows[i]["GrWeight"].ToString());
                        dr["aType"] = ds.Tables[1].Rows[i]["aType"].ToString();
                        dr["Remarks"] = ds.Tables[1].Rows[i]["Remarks"].ToString();

                        if (ds.Tables[1].Rows[i]["GDNID"].ToString() != "")
                            dr["GDNID"] = new Guid(ds.Tables[1].Rows[i]["GDNID"].ToString());
                        else
                            dr["GDNID"] = Guid.Empty;
                        if (ds.Tables[1].Rows[i]["GRNID"].ToString() != "")
                            dr["GRNID"] = new Guid(ds.Tables[1].Rows[i]["GRNID"].ToString());
                        else
                            dr["GRNID"] = Guid.Empty;
                        dt.Rows.Add(dr);
                    }
                    Session["GDNDetails"] = dt;

                    if (dt != null && dt.Rows.Count > 0)
                        gvCheckedList.DataSource = dt;
                    else
                        gvCheckedList.DataSource = null;
                    gvCheckedList.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

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
        /// This is Used to Convert GridView to DataTable
        /// </summary>
        /// <param name="gvCT1"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView gvCheckList)
        {
            DataTable dt = new DataTable();
            dt = (DataTable)Session["GDNDetails"];
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
        /// List Box Selected Items
        /// </summary>
        private void LBSelectedItems()
        {
            try
            {
                string CustID = ""; string GDNids = ""; string GRNids = ""; string FPOIDs = "";

                if (Request.QueryString["ID"] != null)
                {
                    if (CustID1 != null && CustID1.Count > 0)
                    {
                        for (int i = 0; i < ListBoxCustomer.Items.Count; i++)
                            ListBoxCustomer.Items[i].Selected = CustID1.Where(Field => Field.Value.Trim().ToLower() == ListBoxCustomer.Items[i].Value.ToString().Trim().ToLower()).Any();
                    }
                }
                if (GDNids1 != null && GDNids1.Count > 0)
                {
                    for (int i = 0; i < ListBoxGDN.Items.Count; i++)
                        ListBoxGDN.Items[i].Selected = GDNids1.Where(Field => Field.Value.Trim().ToLower() == ListBoxGDN.Items[i].Value.ToString().Trim().ToLower()).Any();
                }
                if (GRNids1 != null && GRNids1.Count > 0)
                {
                    for (int i = 0; i < ListBoxGRN.Items.Count; i++)
                    {
                        string gr = ListBoxGRN.Items[i].Value.ToString().Trim().ToLower();
                        if (GRNids1.ContainsKey(gr.Trim()))
                            ListBoxGRN.Items[i].Selected = true;
                    }
                }
                if (FPOIDs1 != null && FPOIDs1.Count > 0)
                {
                    for (int i = 0; i < lstbxVerbalFPOIDs.Items.Count; i++)
                    {
                        string gr = lstbxVerbalFPOIDs.Items[i].Value.ToString();
                        if (FPOIDs1.ContainsKey(gr.Trim()))
                        {
                            lstbxVerbalFPOIDs.Items[i].Selected = true;
                            // lblFpoNosVerbal.Text = lstbxVerbalFPOIDs.Items[i].Text;
                            pnlVerbalFPO.Visible = true;
                        }
                    }
                }

                CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                FPOIDs = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Record
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                string CustID = ""; string GDNids = ""; string GRNids = ""; string FPOIDs = "";
                DataSet ds = new DataSet();
                ds = CLBLL.GetData(CommonBLL.FlagModify, ID, "", "", "", Guid.Empty, "", "", "",
                    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    Customers();
                    ViewState["Edit_Rows"] = ds.Tables[1];
                    CustID = ds.Tables[0].Rows[0]["CustomerID"].ToString();
                    CustID1 = CustID.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    // LBSelectedItems();
                    GDNids = ds.Tables[0].Rows[0]["GDNIDs"].ToString().Trim().ToLower();
                    GDNids1 = GDNids.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    GRNids = ds.Tables[0].Rows[0]["GRNIDs"].ToString().Trim().ToLower();
                    GRNids1 = GRNids.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    FPOIDs = ds.Tables[0].Rows[0]["VerbalFPOIDs"].ToString();
                    FPOIDs1 = FPOIDs.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    Session["GDN_IDs"] = GDNids;
                    Session["GRN_IDs"] = GRNids;
                    Session["Customer_IDs"] = CustID;
                    //Session["C_IDs"] = 
                    ListBoxCustmrSelected();
                    LBSelectedItems();
                    GetFPOs();
                    lbfpos.Items.Cast<ListItem>().ToList().ForEach(i => i.Selected = true);
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
                    txtEndUseCode.Text = ds.Tables[0].Rows[0]["EndUseCode"].ToString();
                    txtAddIntegratedTax.Text = ds.Tables[0].Rows[0]["IntegratedTax"].ToString();

                    # endregion

                    #region VerbalFPO
                    DataTable dt = new DataTable();
                    dt = (DataTable)Session["GDNDetails"];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataTable dtab = new DataTable();
                        DataRow[] dr = dt.Select("aType = 'VerbalFPO'");
                        if (dr.Length > 0)
                        {
                            DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                            if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                                EmtpyFPO.Columns.Remove("ItemDetailsId");
                            //               BindDropDownList(ddlVerbalSupplier, FPOBLL.GetDataSet(CommonBLL.FlagXSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                            //DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false,
                            //"", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions()));

                            if (Session["VerbalFpoDetails"] != null && ((DataTable)Session["VerbalFpoDetails"]).Rows.Count > 0)
                                dtab = (DataTable)Session["VerbalFpoDetails"];
                            else
                            {
                                int Count = dr.Length;
                                dtab = CommonBLL.EmptyDTCheckedListVerbal(Count, dtab);
                            }
                            for (int i = 0; i < dr.Length; i++)
                            {
                                dtab.Rows[i]["CustomerID"] = dr[i]["CustomerID"].ToString();
                                dtab.Rows[i]["SupplierID"] = dr[i]["SupplierID"].ToString();
                                dtab.Rows[i]["SupplierNm"] = dr[i]["SupplierNm"].ToString();
                                dtab.Rows[i]["NoOfPkgs"] = dr[i]["NoOfPkgs"].ToString();
                                dtab.Rows[i]["VerbalFPOs"] = dr[i]["VerbalFPOs"].ToString();
                                dtab.Rows[i]["FPONOs"] = dr[i]["FPONOs"].ToString();
                                dtab.Rows[i]["LR_GodownNo"] = dr[i]["LR_GodownNo"].ToString();
                                dtab.Rows[i]["IsARE1"] = Convert.ToBoolean(dr[i]["IsARE1"].ToString());
                                dtab.Rows[i]["NetWeight"] = dr[i]["NetWeight"].ToString();
                                dtab.Rows[i]["GrWeight"] = dr[i]["GrWeight"].ToString();
                                dtab.Rows[i]["Remarks"] = dr[i]["Remarks"].ToString();
                            }

                            //ddlVerbalSupplier.SelectedValue = dr[0]["SupplierID"].ToString();
                            //txtNoofPkgs.Text = dr[0]["PkgNos"].ToString();
                            //txtVerbalGodownReceiptNo.Text = dr[0]["LR_GodownNo"].ToString();
                            //chkVerbalAreOne.Checked = Convert.ToBoolean(dr[0]["IsARE1"].ToString());
                            //txtVerbalNetWeight.Text = dr[0]["NetWeight"].ToString();
                            //txtVerbalGrWeight.Text = dr[0]["GrWeight"].ToString();
                            //txtRemarks.Text = dr[0]["Remarks"].ToString();
                        }
                        Session["VerbalFpoDetails"] = dtab;
                        Session["GDNDetails"] = dt;
                        string FPOCount = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        divVerbalDetails.InnerHtml = FillVerbalFPOGrid(FPOCount);
                        DataSet ds_diplay = new DataSet();
                        ds_diplay = CLBLL.GetData(CommonBLL.FlagYSelect, Guid.Empty, "", GRNids, GDNids, Guid.Empty, "", "", "", Guid.Empty,
                            DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                        if (ds_diplay.Tables[2] != null && ds_diplay.Tables[2].Rows.Count > 0)
                        {
                            Session["GRNDetails"] = ds_diplay.Tables[2];
                            divGrnDetails.InnerHtml = FillGRNDetails();
                        }

                    }

                    #endregion

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box Selected Customers
        /// </summary>
        private void ListBoxCustmrSelected()
        {
            try
            {
                string CustID = ""; string GDNids = ""; string GRNids = "";
                // if (Request.QueryString["ID"] == null)
                // {
                CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                if (CustID == "" && (string)Session["Customer_IDs"] != "")
                    CustID = (string)Session["Customer_IDs"];
                if (GDNids == "" && (string)Session["GDN_IDs"] != "")
                    GDNids = (string)Session["GDN_IDs"];
                if (GRNids == "" && (string)Session["GRN_IDs"] != "")
                    GRNids = (string)Session["GRN_IDs"];
                if (Request.QueryString["ID"] != null)
                {
                    GDNids1 = GDNids.Split(',').ToDictionary(key => key, value => value);
                    GRNids1 = GRNids.Split(',').ToDictionary(key => key, value => value);
                }
                // }
                // else
                //  {
                //GDNids = (string)Session["GDN_IDs"];
                // GRNids = (string)Session["GRN_IDs"];
                //  CustID = (string)Session["Customer_IDs"];
                //}
                DataSet ds = new DataSet();
                if (Request.QueryString["ID"] == null)
                    ds = CLBLL.GetData(CommonBLL.FlagZSelect, Guid.Empty, CustID, "", "", Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                else
                    ds = CLBLL.GetData(CommonBLL.FlagPSelectAll, Guid.Empty, CustID, GRNids, GDNids, Guid.Empty, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                HttpContext.Current.Session["Suppliers"] = FPOBLL.GetDataSet(CommonBLL.FlagXSelect, Guid.Empty, CustID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                    DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false,
                    "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Contains("Description"))
                {
                    ListBoxGDN.DataSource = ds.Tables[0];
                    ListBoxGDN.DataTextField = "Description";
                    ListBoxGDN.DataValueField = "ID";
                    ListBoxGDN.DataBind();
                    //Commented on 17-05-2019 as we are not giving any inout for txtSupInv text box, using only for display purpose 
                    ViewState["LPOs"] = ViewState["LPOs"] + "," + string.Join(",", ds.Tables[0].AsEnumerable().Select(r => r.Field<string>("LPOs")).ToArray());
                    //ViewState["GDNInv"] = string.Join(",", ds.Tables[0].AsEnumerable().Select(r => r.Field<string>("GDNInv")).ToArray());
                }
                else
                {
                    ListBoxGDN.Items.Clear();
                    lbfpos.Items.Clear();
                    txtSupInv.Text = "";
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Columns.Contains("Description"))
                {
                    ListBoxGRN.DataSource = ds.Tables[1];
                    ListBoxGRN.DataTextField = "Description";
                    ListBoxGRN.DataValueField = "ID";
                    ListBoxGRN.DataBind();
                    //Commented on 17-05-2019 as we are not giving any inout for txtSupInv text box, using only for display purpose
                    ViewState["LPOs"] = ViewState["LPOs"] + "," + string.Join(",", ds.Tables[1].AsEnumerable().Select(r => r.Field<string>("LPOs")).ToArray());
                    // ViewState["GRNInv"] = string.Join(",", ds.Tables[1].AsEnumerable().Select(r => r.Field<string>("GRNInv")).ToArray());
                }
                else
                {
                    ListBoxGRN.Items.Clear();
                    lbfpos.Items.Clear();
                    txtSupInv.Text = "";
                }
                if (Request.QueryString["ID"] == null)
                {
                    CustID1 = CustID.Split(',').ToDictionary(key => key, value => value);
                    LBSelectedItems();
                    GetCheckListItems(GDNids, GRNids);
                }
                else
                    LBSelectedItems();
                // if (ViewState["GDNInv"] != null && ViewState["GRNInv"] != null)
                //   txtSupInv.Text = ViewState["GDNInv"].ToString() == "" ? ViewState["GRNInv"].ToString().Trim(',') : ViewState["GDNInv"].ToString().Trim(',') + "," + ViewState["GRNInv"].ToString().Trim(',');
                GetVerbalFPOs(CustID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        private void GetFPOs()
        {
            try
            {
                string GDNids = ""; string GRNids = "";

                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                DataSet dss = new DataSet();
                dss = CLBLL.GetData(CommonBLL.FlagKSelect, Guid.Empty, "", GRNids, GDNids, Guid.Empty, "", "", "", Guid.Empty,
                    DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    lbfpos.DataSource = dss.Tables[0];
                    lbfpos.DataTextField = "Description";
                    lbfpos.DataValueField = "ID";
                    lbfpos.DataBind();
                }
                else
                    lbfpos.Items.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
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
        /// List Box GDN Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxGDN_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string GDNids = ""; string GRNids = "";
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                GDNids1 = GDNids.Split(',').ToDictionary(key => key, value => value);
                GetCheckListItems(GDNids, GRNids);
                GetFPOs();
                lbfpos.Items.Cast<ListItem>().ToList().ForEach(i => i.Selected = true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box GRN Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListBoxGRN_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string GDNids = ""; string GRNids = "";

                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                GRNids1 = GRNids.Split(',').ToDictionary(key => key, value => value);
                GetCheckListItems(GDNids, GRNids);
                GetFPOs();
                if (GRNids != "")
                    divGrnDetails.InnerHtml = FillGRNDetails();
                else
                    divGrnDetails.InnerHtml = "";
                lbfpos.Items.Cast<ListItem>().ToList().ForEach(i => i.Selected = true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        protected void lstbxVerbalFPOIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string GDNids = ""; string GRNids = ""; string FPOIDs = "";

                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                if (lstbxVerbalFPOIDs.SelectedValue == "")
                {
                    pnlVerbalFPO.Visible = false;
                    DataTable dt = new DataTable();
                    if (Session["VerbalFpoDetails"] != null && ((DataTable)Session["VerbalFpoDetails"]).Rows.Count > 0)
                    {
                        //string FPOCount = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        //int CountSelected = FPOCount.Count();
                        //dt = (DataTable)Session["VerbalFpoDetails"];
                        //var DTRows = dt.AsEnumerable().Where(P => FPOCount.Contains(P.Field<string>("VerbalFPOs")));
                        //if (DTRows.Count() > 0)
                        //{
                        //    dt = DTRows.CopyToDataTable();
                        //    Session["VerbalFpoDetails"] = dt;
                        //}
                        //if (CountSelected > DTRows.Count())
                        //{
                        //    int Diff = CountSelected - DTRows.Count();
                        //    dt = CommonBLL.EmptyDTCheckedListVerbal(Diff, dt);
                        //    Session["VerbalFpoDetails"] = dt;
                        //}
                        //if (FPOCount == "")
                        //{
                        Session["VerbalFpoDetails"] = null;
                        //}
                        //dt = CommonBLL.EmptyDTCheckedListVerbal(dt.Rows);
                        FPOIDs = "";
                    }
                    GetCheckListItems(GDNids, GRNids);

                }
                else
                {
                    DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                    if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                        EmtpyFPO.Columns.Remove("ItemDetailsId");
                    pnlVerbalFPO.Visible = true;
                    // lblFpoNosVerbal.Text = lstbxVerbalFPOIDs.SelectedItem.Text;
                    //BindDropDownList(ddlVerbalSupplier, FPOBLL.GetDataSet(CommonBLL.FlagXSelect, Guid.Empty, "", new Guid(ListBoxCustomer.SelectedValue), Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                    //DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false,
                    //"", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions()));

                    string FPOsCount = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    // int Numb = FPOsCount.Split(',').Count()-1;
                    // FPOsCount = FPOsCount + ",";
                    divVerbalDetails.InnerHtml = FillVerbalFPOGrid(FPOsCount);
                    if (Session["VerbalFpoDetails"] != null && ((DataTable)Session["VerbalFpoDetails"]).Rows.Count > 0)
                        GetCheckListItems(GDNids, GRNids);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
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

        # region GridView Events

        /// <summary>
        /// Grid Veiw Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCheckedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblPkgs = (Label)e.Row.Cells[0].FindControl("lblPkgs");
                    TotalPkgs += Convert.ToInt32(lblPkgs.Text);
                    Label lblNetWeight = (Label)e.Row.Cells[0].FindControl("lblNetWeight");
                    TotalNetWeight += Convert.ToDecimal(lblNetWeight.Text);
                    Label lblGrWeight = (Label)e.Row.Cells[0].FindControl("lblGrWeight");
                    TotalGrWeight += Convert.ToDecimal(lblGrWeight.Text);
                }
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    Label lblTotalPkgs = (Label)e.Row.FindControl("lblTotalPkgs");
                    lblTotalPkgs.Text = TotalPkgs.ToString();
                    Label lblTotalNetWeight = (Label)e.Row.FindControl("lblTotalNetWeight");
                    lblTotalNetWeight.Text = TotalNetWeight > 0 ? TotalNetWeight.ToString("#.000") : "0.000";
                    Label lblTotalGrWeight = (Label)e.Row.FindControl("lblTotalGrWeight");
                    lblTotalGrWeight.Text = TotalGrWeight > 0 ? TotalGrWeight.ToString("#.000") : "0.000";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
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
            int res = 1; string Filename = FileName();
            DataTable dt = new DataTable();
            string CustID = ""; string GDNids = ""; string GRNids = ""; string FPOIDs = "";
            try
            {
                dt = (DataTable)Session["GDNDetails"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    txtSupInv.Text = "";
                    CustID = String.Join(",", ListBoxCustomer.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    FPOIDs = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                    GRNids = (GRNids == "" ? Guid.Empty.ToString() : GRNids);
                    GDNids = (GDNids == "" ? Guid.Empty.ToString() : GDNids);
                    FPOIDs = (FPOIDs == "" ? Guid.Empty.ToString() : FPOIDs);

                    string[] values = ViewState["LPOs"].ToString().Trim().Split(',').Distinct().ToArray();
                    values = values.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string LPOss = string.Join(",", Array.ConvertAll<object, string>(values.ToArray(), Convert.ToString));
                    dt = ConvertToDtbl(gvCheckedList);
                    if (btnSave.Text == "Save")
                    {
                        string ChkLstNo = GetCount();
                        res = CLBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, CustID, GRNids, GDNids, FPOIDs, new Guid(rbtnshpmnt.SelectedValue),
                            txtImpInstructions.Text.Trim(), ChkLstNo, "", DateTime.Now, txtOtrRfs.Text, new Guid(ddlNtfy.SelectedValue),
                            new Guid(ddlPlcOrgGds.SelectedValue), new Guid(ddlPlcFnlDstn.SelectedValue), new Guid(ddlPrtLdng.SelectedValue),
                            new Guid(ddlPrtDscrg.SelectedValue), new Guid(ddlPlcDlry.SelectedValue), txtPCrBy.Text, txtPlcRcptPCr.Text, txtVslFlt.Text,
                            txtEndUseCode.Text, txtAddIntegratedTax.Text,
                            txtTrmDlryPmnt.Text, new Guid(ddlIncoTrm.Text), txtPriceBasis.Text, txtSupInv.Text.Trim(), "",
                            new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, dt, LPOss,
                            new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Check List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("../Logistics/CheckListStatus.aspx", false);
                        }
                        else if (res != 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Check List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", "Error while Inserting.");
                            ViewState["LPOs"] = "";
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        res = CLBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), CustID, GRNids, GDNids, FPOIDs,
                            new Guid(rbtnshpmnt.SelectedValue), txtImpInstructions.Text.Trim(), txtRefNo.Text.Trim(), "", DateTime.Now, txtOtrRfs.Text,
                            new Guid(ddlNtfy.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue), new Guid(ddlPlcFnlDstn.SelectedValue),
                            new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue), new Guid(ddlPlcDlry.SelectedValue), txtPCrBy.Text,
                            txtPlcRcptPCr.Text, txtVslFlt.Text, txtEndUseCode.Text, txtAddIntegratedTax.Text, txtTrmDlryPmnt.Text, new Guid(ddlIncoTrm.Text), txtPriceBasis.Text, txtSupInv.Text.Trim(),
                            txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                            DateTime.Now, true, dt, LPOss, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Check List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            Response.Redirect("../Logistics/CheckListStatus.aspx", false);
                        }
                        else if (res != 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Check List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", "Error while Inserting.");
                            ViewState["LPOs"] = "";
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No GDNs or GRNs to Save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Logistics/CheckList.aspx", false);
        }

        protected void btnVerbalFPO_Click(object sender, EventArgs e)
        {
            try
            {
                string GDNids = "";
                string GRNids = "";

                GDNids = String.Join(",", ListBoxGDN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GRNids = String.Join(",", ListBoxGRN.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                btnVerbalFPO.Attributes.Add("OnClick", "javascript:return final()");
                GetCheckListItems(GDNids, GRNids);
                string FPOsCount = String.Join(",", lstbxVerbalFPOIDs.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                divVerbalDetails.InnerHtml = FillVerbalFPOGrid(FPOsCount);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Check List", ex.Message.ToString());
            }
        }

        # endregion

        #region Html Table to fill Verbal FPO and GRN Details
        private string FillVerbalFPOGrid(string Count)
        {
            try
            {
                int RowNo = 1;
                string[] FPOCount = Count.Split(',');
                int CountSelected = FPOCount.Count();
                DataTable dt = new DataTable();
                // if (ViewState["EditID"] != "")
                // {
                if (Session["VerbalFpoDetails"] != null && ((DataTable)Session["VerbalFpoDetails"]).Rows.Count > 0)
                {

                    dt = (DataTable)Session["VerbalFpoDetails"];
                    var DTRows = dt.AsEnumerable().Where(P => FPOCount.Contains(P.Field<string>("VerbalFPOs")));
                    if (DTRows.Count() > 0)
                    {
                        dt = DTRows.CopyToDataTable();
                    }
                    if (CountSelected != DTRows.Count())
                    {
                        int Diff = 0;
                        if (CountSelected > DTRows.Count())
                            Diff = CountSelected - DTRows.Count();
                        else if (CountSelected < DTRows.Count())
                            Diff = DTRows.Count() - CountSelected;
                        dt = CommonBLL.EmptyDTCheckedListVerbal(Diff, dt);
                    }
                    //dt = CommonBLL.EmptyDTCheckedListVerbal(dt.Rows);

                }
                else
                {

                    dt = CommonBLL.EmptyDTCheckedListVerbal(CountSelected, dt);
                }
                // }
                DataSet dsSuppliers = (DataSet)Session["Suppliers"];
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' " + " id='tblVerbalDetails'>");
                sb.Append("<thead align='centre'>");
                sb.Append("<tr style='font-size: small; font-weight: bold; color: Red;'><th>SNo</th><th>Supplier</th><th>No of Pkgs</th><th>Verbal FPO No</th><th>Godown Receipt No</th>" +
                    "<th width = '15px'>Covered Under ARE-1 </th><th>Net Weight Kgs</th><th>Gr Weight Kgs</th><th>Remarks</th></tr>");
                sb.Append("</thead><tbody>");

                if (Count != "")
                {
                    RowNo = dt.Rows.Count + 1;
                    for (int i = 0; i < CountSelected; i++)
                    {
                        var _DD = dt.AsEnumerable().Select(C => C.Field<string>("VerbalFPOs")).ToList();
                        var Item_ID = FPOCount.Where(P => !_DD.Contains(P)).ToList();
                        if ((dt.Rows[i]["VerbalFPOs"] == DBNull.Value || dt.Rows[i]["VerbalFPOs"] == "") && Item_ID.Count > 0)
                            dt.Rows[i]["VerbalFPOs"] = Item_ID[0];

                        DataSet d = FPOBLL.GetFPONameByID(CommonBLL.FlagASelect, new Guid(Convert.ToString(dt.Rows[i]["VerbalFPOs"])));

                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align: right; width: 30px;'>" + (i + 1) + "</td>");
                        dt.Rows[i]["SNo"] = SNo;
                        sb.Append("<td>");
                        # region Bind-DDL
                        sb.Append("<select id='ddlSuppliers" + SNo + "' onchange='SaveChanges(" + SNo + ")' Class='bcAspdropdown' width='120px'>");
                        sb.Append("<option value='" + Guid.Empty.ToString() + "'>-SELECT-</option>");

                        if (dsSuppliers != null && dsSuppliers.Tables.Count > 0)
                        {
                            foreach (DataRow row in dsSuppliers.Tables[0].Rows)
                            {
                                if (dt.Rows[i]["SupplierID"].ToString() == row["ID"].ToString())
                                {
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                                    dt.Rows[i]["SupplierNm"] = row["Description"].ToString();
                                }
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                        }
                        sb.Append("</select>");
                        #endregion
                        sb.Append("</td>");
                        sb.Append("<td><input type='text' name='txtNoPkgs' class='bcAsptextbox' value='" +
                                    dt.Rows[i]["NoOfPkgs"].ToString() + "'  id='txtNoPkgs" + SNo +
                                    "' onkeypress='return isNumberKey(event)' onchange='SaveChanges("
                                    + SNo + ")' maxlength='15' style='text-align: right; width: 100px;'/></td>");

                        sb.Append("<td>");
                        dt.Rows[i]["FPONOs"] = d.Tables[0].Rows[0]["FPONo"];
                        dt.Rows[i]["CustomerID"] = d.Tables[0].Rows[0]["CustomerID"];
                        sb.Append("<label id='lblFpoNo" + SNo + "' style='text-align: right; width: 120px;'>" + dt.Rows[i]["FPONOs"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td><input type='text' name='txtGRN' class='bcAsptextbox' value='" +
                                dt.Rows[i]["LR_GodownNo"].ToString() + "'  id='txtGRNNo" + SNo + "' onchange='SaveChanges(" + SNo +
                                ")' maxlength='50' style='text-align: right; width: 100px;'/></td>");

                        if (Convert.ToBoolean(dt.Rows[i]["IsARE1"]))
                            sb.Append("<td><input id='txtCoveredUnderARE" + SNo + "' onclick='SaveChanges(" + SNo + ")' type='checkbox' checked='checked' name='txtCoveredUnderARE'style='text-align: right; width: 50px;'/></td>");//checkBox
                        else
                            sb.Append("<td><input id='txtCoveredUnderARE" + SNo + "' onclick='SaveChanges(" + SNo +
                                ")' type='checkbox' name='txtCoveredUnderARE' style='text-align: right; width: 50px;'/></td>");//checkBox

                        //sb.Append("<td><input type='CheckBox' name='txtCoveredUnderARE' value='" +
                        //    dt.Rows[i]["IsARE1"].ToString() + "'  id='txtCoveredUnderARE" + SNo +
                        //    "' onchange='SaveChanges(" + SNo + ")' style='text-align: right; width: 50px;'/></td>");

                        sb.Append("<td><input type='text' name='NetWeight' class='bcAsptextbox' value='" +
                                dt.Rows[i]["NetWeight"].ToString() + "'  id='txtNetWt" + SNo +
                                "' onkeypress='return isNumberKey(event)' onchange='SaveChanges(" + SNo +
                                ")' maxlength='15' style='text-align: right; width: 100px;'/></td>");

                        sb.Append("<td><input type='text' name='Gross Weight' class='bcAsptextbox' value='" +
                                dt.Rows[i]["GrWeight"].ToString() + "'  id='txtGrsWt" + SNo +
                                "' onkeypress='return isNumberKey(event)' onchange='SaveChanges(" + SNo +
                                ")' maxlength='15' style='text-align: right; width: 100px;'/></td>");

                        sb.Append("<td><input type='text' name='Remarks' class='bcAsptextboxmulti' rows='3' cols='20' value='" +
                                dt.Rows[i]["Remarks"].ToString() + "' id='txtRemarks" + SNo +
                                "' onchange='SaveChanges(" + SNo +
                                ")' maxlength='300' style='text-align: left; width: 200px;'/></td>");
                        sb.Append("</tr>");
                    }

                    //sb.Append("<td><a href='javascript:void(0)' onclick='final()' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                    // sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='5' class='rounded-foot-right'>" +
                    //    "<input id='HfMessage' type='hidden' name='HfMessage' value=''/></th></tfoot>");
                }
                sb.Append("</tbody></table>");
                Session["VerbalFpoDetails"] = dt;
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

        private string FillGRNDetails()
        {
            try
            {
                int RowNo = 1;

                DataTable dt = new DataTable();
                // if (ViewState["EditID"] != "")
                // {
                if (Session["GRNDetails"] != null && ((DataTable)Session["GRNDetails"]).Rows.Count > 0)

                    dt = (DataTable)Session["GRNDetails"];
                else
                    dt = CommonBLL.EmptyDTGRNDetails();

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                if (dt.Rows[0]["LocationName"].ToString() != "")
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' " + " id='tblVerbalDetails'>");
                    sb.Append("<thead align='centre'>");
                    sb.Append("<tr style='font-size: small; font-weight: bold; color: Black;'><th style= 'text-align: left'>SNo</th><th style= 'text-align: left'>GRN Number</th><th style= 'text-align: left'>LocationName</th><th style= 'text-align: left'>BoxName</th><th style= 'text-align: left'>Dimensions</th></tr>");
                    sb.Append("</thead><tbody>");
                }

                RowNo = dt.Rows.Count + 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //dt.Rows[i]["VerbalFPOs"] = FPOCount[i];
                    if (dt.Rows[i]["LocationName"].ToString() != "")
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td style='align: left; width: 60px;'>" + (i + 1) + "</td>");

                        sb.Append("<td>");
                        sb.Append("<input type='text' readonly class='bcAsptextbox' value='" + dt.Rows[i]["GRNNo"].ToString() + "' id='lblGRNNo" + SNo + "' style='align: left; width: 180px;'></>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<input type='text' readonly class='bcAsptextbox' value='" + dt.Rows[i]["LocationName"].ToString() + "' id='Locname" + SNo + "' style='align: left; width: 180px;'></>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<input type='text' readonly class='bcAsptextbox' value='" + dt.Rows[i]["BoxName"].ToString() + "' id='BoxName" + SNo + "' style='align: left; width: 180px;'></>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<input type='text' readonly class='bcAsptextbox' value='" + dt.Rows[i]["Dimensions"].ToString() + "' id='Dimension" + SNo + "' style='align: left; width: 180px;'></>");
                        sb.Append("</td>");

                        sb.Append("</tr>");
                    }
                }

                //sb.Append("<td><a href='javascript:void(0)' onclick='final()' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                // sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='5' class='rounded-foot-right'>" +
                //    "<input id='HfMessage' type='hidden' name='HfMessage' value=''/></th></tfoot>");

                sb.Append("</tbody></table>");
                Session["GRNDetails"] = dt;
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

        #endregion

        #region WebMethods
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string VerbalFPOAdd(int rowNo, string Supp, string Pkgs, string GRN, bool ARE, string NetW, string GrossW, string Rema, string Items, string FPONumber)
        {
            DataTable dt = new DataTable();

            try
            {
                string[] FPOCount = Items.Split(',');
                int CountSelected = FPOCount.Count();

                dt = (DataTable)Session["VerbalFpoDetails"];
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = CommonBLL.EmptyDTCheckedListVerbal(CountSelected, dt);
                    Session["VerbalFpoDetails"] = dt;
                }
                int count = dt.Rows.Count;
                dt.Rows[rowNo - 1]["SupplierID"] = Supp;
                dt.Rows[rowNo - 1]["NoOfPkgs"] = Pkgs;
                dt.Rows[rowNo - 1]["LR_GodownNo"] = GRN;
                dt.Rows[rowNo - 1]["IsARE1"] = ARE;
                dt.Rows[rowNo - 1]["NetWeight"] = NetW;
                dt.Rows[rowNo - 1]["GrWeight"] = GrossW;
                dt.Rows[rowNo - 1]["Remarks"] = Rema;
                dt.Rows[rowNo - 1]["FPONOs"] = FPONumber;

                // if (IsNew)
                // {
                //     dt.Rows.Add(Guid.Empty, "", "",, 0, "", DateTime.Now);//.ToString("MM-dd-yyyy")
                // }

                dt.AcceptChanges();
                Session["VerbalFpoDetails"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
            if (dt != null && dt.Rows.Count > 0)
                Session["VerbalFpoDetails"] = dt;
            return FillVerbalFPOGrid(Items);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Count(string items)
        {
            try
            {
                divVerbalDetails.InnerHtml = FillVerbalFPOGrid(items);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Details", ex.Message.ToString());
            }
            return FillVerbalFPOGrid(items);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddtoGrid()
        {
            try
            {
                //divVerbalDetails.InnerHtml = FillVerbalFPOGrid(items);
                GetCheckListItems("", "");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Details", ex.Message.ToString());
            }
            return "";
        }

        #endregion

    }
}
