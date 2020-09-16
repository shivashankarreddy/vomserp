using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using VOMS_ERP.Logistics;
using System.Collections;
using System.Text;
using VOMS_ERP.Admin;
using System.Web;
using System.IO;

namespace VOMS_ERP
{
    public partial class CTOneGeneration : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        IOMTemplateBLL IOMTBLL = new IOMTemplateBLL();
        ProformaExciseDetailsBLL PFEDBLL = new ProformaExciseDetailsBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        CT1GenerationFormBLL CT1Form = new CT1GenerationFormBLL();
        ItemMasterBLL imbll = new ItemMasterBLL();
        EnumMasterBLL EnumBLL = new EnumMasterBLL();
        static DateTime CTOneRefDT = Convert.ToDateTime(DateTime.MaxValue.ToShortDateString());
        static bool ChkAllStatus = false;
        static decimal ExdutyAmt = 0;
        static bool ChkExduty = false;
        static double ChkExdutyAmt = 0;
        static double DiscountPercentAmt = 0;
        static double PackingPercentAmt = 0;
        static string SupID = "";
        static string PinvReqID = "";
        EnumMasterBLL embal = new EnumMasterBLL();
        Dictionary<int, int> Codes = new Dictionary<int, int>();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        # endregion

        #region Default Page Load Event

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if ((Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty) && Request.QueryString["SupID"] == null)
                    Response.Redirect("../Login.aspx?logout=yes", false);

                else
                {
                    if ((Session["UserID"] != null && CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path)) || Request.QueryString["SupID"] != null)
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        txtFactryAdd.Attributes.Add("readonly", "readonly");
                        txtDate.Attributes.Add("readonly", "readonly");
                        txtRefDate.Attributes.Add("readonly", "readonly");
                        txtCEDAmt.Attributes.Add("readonly", "readonly");
                        txtCtOneVal.Attributes.Add("readonly", "readonly");
                        txtHeadingNumbers.Attributes.Add("readonly", "readonly");
                        //txtDispatchMdt.Attributes.Add("readonly", "readonly");
                        Ajax.Utility.RegisterTypeForAjax(typeof(CTOneGeneration));
                        if (!IsPostBack)
                        {
                            ClearAll();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
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

        /// <summary>
        /// This is Used to Get Selected DDLHypothe
        /// </summary>
        private void Hypoth_SelectedVal()
        {
            try
            {
                if (ddlHypothecation.SelectedValue != "0")
                {
                    SupplierUnitsBLL SUBLL = new SupplierUnitsBLL();
                    DataSet SUpds = SUBLL.Select(CommonBLL.FlagZSelect, new Guid(ddlHypothecation.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    ddlUnits.DataSource = SUpds.Tables[0];
                    ddlUnits.DataTextField = "Description";
                    ddlUnits.DataValueField = "ID";
                    ddlUnits.DataBind();
                    ddlUnits.Items.Insert(0, new ListItem("-- Select Units --", Guid.Empty.ToString()));

                    if (SUpds.Tables.Count > 1 && SUpds.Tables[1].Rows.Count > 0)
                    {
                        txtFactryAdd.Text = SUpds.Tables[1].Rows[0]["FctryAdrs"].ToString();
                        txtRange.Text = SUpds.Tables[1].Rows[0]["Range"].ToString();
                        txtDivision.Text = SUpds.Tables[1].Rows[0]["Division"].ToString();
                        txtCommissioneRate.Text = SUpds.Tables[1].Rows[0]["Commissionerate"].ToString();
                        txtVRange.Text = SUpds.Tables[1].Rows[0]["RangeAddress"].ToString();
                        txtVDivision.Text = SUpds.Tables[1].Rows[0]["DivisionAddress"].ToString();
                        txtVCommissionerate.Text = SUpds.Tables[1].Rows[0]["CommissionerateAddress"].ToString();
                        HFhypothecation.Value = SUpds.Tables[1].Rows[0][0].ToString();
                    }
                    else
                        txtFactryAdd.Text = HFSupplierAdd.Value;
                }
                else
                {
                    ddlUnits.SelectedIndex = -1;
                    ddlUnits.Items.Clear();
                    txtFactryAdd.Text = HFSupplierAdd.Value;
                    txtRange.Text = txtDivision.Text = txtCommissioneRate.Text = "";
                    txtVDivision.Text = txtVCommissionerate.Text = txtVRange.Text = "";
                    DataSet CommonDt = CT1Form.SelectCT1GF(CommonBLL.FlagISelect, Guid.Empty, new Guid(ddlIOMRefNo.SelectedValue), Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()));
                    if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                    {
                        txtRange.Text = CommonDt.Tables[0].Rows[0]["Range"].ToString();
                        txtDivision.Text = CommonDt.Tables[0].Rows[0]["Division"].ToString();
                        txtCommissioneRate.Text = CommonDt.Tables[0].Rows[0]["Commissionerate"].ToString();
                        txtVRange.Text = CommonDt.Tables[0].Rows[0]["RangeAddress"].ToString();
                        txtVDivision.Text = CommonDt.Tables[0].Rows[0]["DivisionAddress"].ToString();
                        txtVCommissionerate.Text = CommonDt.Tables[0].Rows[0]["CommissionerateAddress"].ToString();

                        BindDropDownList(ddlUnits, CommonDt.Tables[1]);
                    }
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
                           "CHeck('chkDsnt', 'dvDsnt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
                           "CHeck('chkExdt', 'dvExdt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
                           "CHeck('chkPkng', 'dvPkng');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Default Data
        /// </summary>
        private void GetData()
        {
            try
            {

                if (Request.QueryString["SupID"] != null)
                {
                    string DecryptString = StringEncrpt_Decrypt.Decrypt(Request.QueryString["SupID"].Replace(' ', '+'), true);
                    string[] qs = DecryptString.Split('&');

                    SupID = qs[0];
                    PinvReqID = qs[1].Split('=')[1].Trim();
                    string LMID = qs[2].Split('=')[1].Trim();
                    string LPWD = qs[3].Split('=')[1].Trim();
                    string CMPID = qs[4].Split('=')[1].Trim();
                    if (LMID != "" && LPWD != "")
                    {
                        LoginBLL bll = new LoginBLL();
                        if (bll.LogIn(CommonBLL.FlagSelectAll, LMID, LPWD, false, new Guid(CMPID)))
                        {
                            ArrayList al = bll.UserDetails();
                            Session["UserMail"] = LMID;
                            Session["UserID"] = al[1].ToString(); Session["UserID"] = new Guid(al[1].ToString());
                            Session["UserName"] = al[0].ToString(); Session["UserName"] = al[0].ToString();
                            Session["AliasName"] = al[4].ToString();
                            Session["TLMailID"] = al[5].ToString(); Session["TLID"] = al[6].ToString();
                            Session["LoginType"] = al[7].ToString();
                            Session["CompanyID"] = al[12].ToString();

                            CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path);
                            if (Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"] != null))
                            {
                                GetGenCategory();
                                Label1.Text = "Proforma Invoice";
                                HFPrfINVID.Value = PinvReqID;
                                ddlIOMRefNo.Enabled = false;
                                DataSet ds = new DataSet();
                                ds = IOMTBLL.GetData(CommonBLL.FlagYSelect, Guid.Empty, "", "", "", DateTime.Now, new Guid(PinvReqID),
                                    "", DateTime.Now, "", "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    string PInvID = ds.Tables[0].Rows[0]["PInvID"].ToString();
                                    string CT1ID = ds.Tables[0].Rows[0]["CT1ID"].ToString();
                                    string PrfINVReqID = ds.Tables[0].Rows[0]["PrfINVReqID"].ToString();
                                    EditRecord(new Guid(PInvID), "", new Guid(CT1ID), PrfINVReqID);
                                }
                                else
                                {
                                    FillInputs(IOMTBLL.GetData(CommonBLL.FlagXSelect, Guid.Empty, "", "", "", DateTime.Now, new Guid(PinvReqID),
                                        "", DateTime.Now, "", "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString())));
                                }
                            }
                            else
                            {
                                Response.Redirect("../Login.aspx?logout=no", false);
                            }
                        }
                    }
                }
                else if (Request.QueryString["ReApp"] == null && Request.QueryString["PinvID"] != null &&
                    Request.QueryString["IomID"] != null && Request.QueryString["CT1"] != null && Request.QueryString["PrfINVID"] != null)
                    EditRecord(new Guid(Request.QueryString["PinvID"]), Request.QueryString["IomID"],
                                new Guid(Request.QueryString["CT1"]), Request.QueryString["PrfINVID"]);
                else if (Request.QueryString["ReApp"] != null && Request.QueryString["PinvID"] != null &&
                    Request.QueryString["IomID"] != null && Request.QueryString["CT1"] != null)
                    EditRecord(new Guid(Request.QueryString["PinvID"].ToString()), Request.QueryString["IomID"],
                                new Guid(Request.QueryString["CT1"].ToString()), Request.QueryString["PrfINVID"]);

                GetGenCategory();
                BindIOMRefNo();
                BindDropDownList(ddlLocation, EnumBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.CTOneLocations).Tables[0]);
                //if (Session["AllSubItems"] == null)
                //  Session["AllSubItems"] = CommonBLL.CT1ItemsTable_SubItems();

                if (Request.QueryString["IOMRefID"] != null && Request.QueryString["IOMRefID"] != "")
                {
                    ddlIOMRefNo.SelectedValue = Request.QueryString["IOMRefID"].ToString();
                    FillInputs(IOMTBLL.GetData(CommonBLL.FlagZSelect, new Guid(ddlIOMRefNo.SelectedValue),
                                "", "", "", DateTime.Now, Guid.Empty, "", DateTime.Now, "", "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to get General Catagory
        /// </summary>
        private void GetGenCategory()
        {
            EnumMasterBLL embal = new EnumMasterBLL();
            DataSet ds = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                CommonBLL.SupplierCategory);
            ddlCategory.DataSource = ds.Tables[0];
            ddlCategory.DataTextField = "Description";
            ddlCategory.DataValueField = "ID";
            ddlCategory.DataBind();
            ddlCategory.SelectedValue = ddlCategory.Items.FindByText("General").Value;
            ddlCategory.Enabled = false;
            Session["GenSupID"] = new Guid(ddlCategory.SelectedValue);
        }

        /// <summary>
        /// This is used to bind IOMRefNo
        /// </summary>
        private void BindIOMRefNo()
        {
            try
            {
                DataSet ds = new DataSet();
                char Flag = CommonBLL.FlagZSelect;
                if (Request.QueryString["PinvID"] != null)
                    Flag = CommonBLL.FlagXSelect;
                ds = PFEDBLL.Select(Flag, Guid.Empty, Guid.Empty, 0, "", "", "", "", "", "", "", "", "", "",
                    DateTime.Now, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlIOMRefNo.DataSource = ds.Tables[0];
                    ddlIOMRefNo.DataTextField = "Description";
                    ddlIOMRefNo.DataValueField = "ID";
                    ddlIOMRefNo.DataBind();
                }
                ddlIOMRefNo.Items.Insert(0, new ListItem("-- Select IOMRefNo--", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Inputs from DataSet Using IOM Ref No.
        /// </summary>
        /// <param name="CommonDt"></param>
        private void FillInputs(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count >= 2 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    txtCustomer.Text = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    txtSupplierNm.Text = CommonDt.Tables[0].Rows[0]["SupplierNm"].ToString();
                    txtImpInstructions.Text = CommonDt.Tables[0].Rows[0]["Body"].ToString();
                    txtProfInv.Text = CommonDt.Tables[0].Rows[0]["ProformaInvoice"].ToString();
                    Guid SupId = new Guid(CommonDt.Tables[0].Rows[0]["SupplierID"].ToString());
                    HFSuplierID.Value = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();
                    txtDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ProformaDate"].ToString()));
                    txtFactryAdd.Text = CommonDt.Tables[0].Rows[0]["SuppAdd"].ToString();
                    HFSupplierAdd.Value = CommonDt.Tables[0].Rows[0]["SuppAdd"].ToString();

                    string[] FPOs = CommonDt.Tables[1].Rows[0]["fpo"].ToString().Split(',');
                    string[] LPOs = CommonDt.Tables[1].Rows[0]["lpo"].ToString().Split(',');
                    string[] FPONos = CommonDt.Tables[1].Rows[0]["fpono"].ToString().Split(',');
                    string[] LPONos = CommonDt.Tables[1].Rows[0]["lpono"].ToString().Split(',');

                    txtRange.Text = CommonDt.Tables[0].Rows[0]["Range"].ToString();
                    txtDivision.Text = CommonDt.Tables[0].Rows[0]["Division"].ToString();
                    txtCommissioneRate.Text = CommonDt.Tables[0].Rows[0]["Commissionerate"].ToString();
                    txtVRange.Text = CommonDt.Tables[0].Rows[0]["RangeAddress"].ToString();
                    txtVDivision.Text = CommonDt.Tables[0].Rows[0]["DivisionAddress"].ToString();
                    txtVCommissionerate.Text = CommonDt.Tables[0].Rows[0]["CommissionerateAddress"].ToString();
                    HFCustID.Value = CommonDt.Tables[0].Rows[0]["CstmrID"].ToString();

                    Dictionary<string, string> NewFPOs = new Dictionary<string, string>();
                    Dictionary<string, string> NewLPOs = new Dictionary<string, string>();

                    for (int i = 0; i < FPOs.Length; i++)
                    {
                        ListBoxFPO.Items.Add(new ListItem(FPONos[i], FPOs[i]));
                        ListBoxFPO.SelectionMode = ListSelectionMode.Multiple;
                    }
                    for (int j = 0; j < LPOs.Length; j++)
                    {
                        ListBoxLPO.Items.Add(new ListItem(LPONos[j], LPOs[j]));
                        ListBoxLPO.SelectionMode = ListSelectionMode.Multiple;
                    }
                    if (CommonDt != null && CommonDt.Tables.Count > 3 && CommonDt.Tables[3].Rows.Count > 0)
                    {
                        BindDropDownList(ddlUnits, CommonDt.Tables[3]);
                    }

                    DataTable ItemsTable = new DataTable();
                    ItemsTable = CommonDt.Tables[2].Copy();
                    DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                    DCCheck.DefaultValue = true;
                    ItemsTable.Columns.Add(DCCheck);
                    DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                    DCTotalAmt.DefaultValue = 0;
                    ItemsTable.Columns.Add(DCTotalAmt);
                    ItemsTable = Calculations(ItemsTable, -1);
                    DivItemDetails.InnerHtml = FillItemDetails(ItemsTable, "");
                    DvTerms.Visible = true;
                    SupplierUnitsBLL SUBLL = new SupplierUnitsBLL();
                    DataSet SUpds = SUBLL.Select(CommonBLL.FlagXSelect, Guid.Empty, SupId, new Guid(Session["CompanyID"].ToString()));
                    ddlHypothecation.DataSource = SUpds.Tables[0];
                    ddlHypothecation.DataTextField = "Description";
                    ddlHypothecation.DataValueField = "ID";
                    ddlHypothecation.DataBind();
                    ddlHypothecation.Items.Insert(0, new ListItem("-- Select Hypothecation--", Guid.Empty.ToString()));
                    string CT1Count = CommonDt.Tables[1].Rows[0]["CT1Count"].ToString();
                    txtCT1DrftRefNo.Text = "CT1Draft/" + Session["AliasName"] + "/" + CT1Count + "/" + CommonBLL.FinacialYearShort + "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to EDIT Records
        /// </summary>
        /// <param name="PinvID"></param>
        /// <param name="IomID"></param>
        /// <param name="CT1"></param>
        private void EditRecord(Guid PinvID, string IomID, Guid CT1, string PrfINVID)
        {
            try
            {
                ViewState["PINVID"] = PinvID;
                ViewState["CT1ID"] = CT1;
                HFPrfINVID.Value = PrfINVID;
                GetGenCategory();
                DataSet EditDS = new DataSet();
                if (PrfINVID == Guid.Empty.ToString() && IomID != "")
                    EditDS = CT1Form.SelectCT1GF(CommonBLL.FlagModify, CT1, new Guid(IomID), PinvID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                else if (PrfINVID != Guid.Empty.ToString() && IomID == "")
                    EditDS = CT1Form.SelectCT1GF(CommonBLL.FlagVSelect, CT1, Guid.Empty, PinvID, new Guid(PrfINVID.Trim()), new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count > 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                                            && EditDS.Tables[2].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0)
                {
                    ddlIOMRefNo.SelectedValue = EditDS.Tables[2].Rows[0]["IOMRefID"].ToString();
                    txtCustomer.Text = EditDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    txtSupplierNm.Text = EditDS.Tables[0].Rows[0]["SupplierNm"].ToString();
                    txtImpInstructions.Text = EditDS.Tables[0].Rows[0]["Body"].ToString();
                    txtProfInv.Text = EditDS.Tables[0].Rows[0]["ProformaInvoice"].ToString();
                    txtDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["ProformaDate"].ToString()));
                    Guid SupId = new Guid(EditDS.Tables[0].Rows[0]["SupplierID"].ToString());
                    HFSuplierID.Value = EditDS.Tables[0].Rows[0]["SupplierID"].ToString();
                    HFCustID.Value = EditDS.Tables[1].Rows[0]["CustomerID"].ToString();
                    string[] FPOs = EditDS.Tables[1].Rows[0]["FPOIDs"].ToString().Split(',');
                    string[] LPOs = EditDS.Tables[1].Rows[0]["LPOIDs"].ToString().Split(',');
                    string[] FPONos = EditDS.Tables[1].Rows[0]["FPONos"].ToString().Split(',');
                    string[] LPONos = EditDS.Tables[1].Rows[0]["LPONos"].ToString().Split(',');
                    ddlLocation.SelectedValue = EditDS.Tables[2].Rows[0]["Location"].ToString();
                    Dictionary<string, string> NewFPOs = new Dictionary<string, string>();
                    Dictionary<string, string> NewLPOs = new Dictionary<string, string>();

                    for (int i = 0; i < FPOs.Length; i++)
                    {
                        ListBoxFPO.Items.Add(new ListItem(FPONos[i], FPOs[i]));
                        ListBoxFPO.SelectionMode = ListSelectionMode.Multiple;
                    }

                    for (int j = 0; j < LPOs.Length; j++)
                    {
                        ListBoxLPO.Items.Add(new ListItem(LPONos[j], LPOs[j]));
                        ListBoxLPO.SelectionMode = ListSelectionMode.Multiple;
                    }

                    SupplierUnitsBLL SUBLL = new SupplierUnitsBLL();
                    DataSet SUpEditDS = SUBLL.Select(CommonBLL.FlagXSelect, Guid.Empty, SupId, new Guid(Session["CompanyID"].ToString()));
                    ddlHypothecation.DataSource = SUpEditDS.Tables[0];
                    ddlHypothecation.DataTextField = "Description";
                    ddlHypothecation.DataValueField = "ID";
                    ddlHypothecation.DataBind();
                    ddlHypothecation.Items.Insert(0, new ListItem("-- Select Hypothecation--", Guid.Empty.ToString()));
                    if (EditDS.Tables[1].Rows[0]["HypothecationID"].ToString() != "")
                    {
                        ddlHypothecation.SelectedValue = EditDS.Tables[1].Rows[0]["HypothecationID"].ToString();

                        DataSet SUpds = SUBLL.Select(CommonBLL.FlagZSelect,
                            new Guid(EditDS.Tables[1].Rows[0]["HypothecationID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        ddlUnits.DataSource = SUpds.Tables[0];
                        ddlUnits.DataTextField = "Description";
                        ddlUnits.DataValueField = "ID";
                        ddlUnits.DataBind();
                        ddlUnits.Items.Insert(0, new ListItem("-- Select Units --", Guid.Empty.ToString()));

                        if (SUpds.Tables.Count > 1 && SUpds.Tables[1].Rows.Count > 0)
                            HFhypothecation.Value = SUpds.Tables[1].Rows[0][0].ToString();

                        if (EditDS.Tables[1].Rows[0]["BranchID"].ToString() != "")
                            ddlUnits.SelectedValue = EditDS.Tables[1].Rows[0]["BranchID"].ToString();
                    }

                    txtCtOneVal.Text = EditDS.Tables[1].Rows[0]["CT1BondValue"].ToString();
                    if (EditDS.Tables[1].Rows[0]["BondBalanceValue"].ToString() != "")
                        txtBondBalance.Text = EditDS.Tables[1].Rows[0]["BondBalanceValue"].ToString();

                    if (EditDS.Tables[1].Rows[0]["DscntPrcnt"].ToString() != ""
                        && Convert.ToDecimal(EditDS.Tables[1].Rows[0]["DscntPrcnt"].ToString()) > 0)
                    {
                        chkDsnt.Checked = true;
                        txtDsnt.Text = EditDS.Tables[1].Rows[0]["DscntPrcnt"].ToString();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
                            "CHeck('chkDsnt', 'dvDsnt');", true);
                    }
                    if (EditDS.Tables[1].Rows[0]["PkngPrcnt"].ToString() != ""
                        && Convert.ToDecimal(EditDS.Tables[1].Rows[0]["PkngPrcnt"].ToString()) > 0)
                    {
                        chkPkng.Checked = true;
                        txtPkng.Text = EditDS.Tables[1].Rows[0]["PkngPrcnt"].ToString();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
                                   "CHeck('chkPkng', 'dvPkng');", true);
                    }
                    if (EditDS.Tables[1].Rows[0]["ExcsePrcnt"].ToString() != "" &&
                        Convert.ToDecimal(EditDS.Tables[1].Rows[0]["ExcsePrcnt"].ToString()) > 0)
                    {
                        chkExdt.Checked = true;
                        txtExdt.Text = EditDS.Tables[1].Rows[0]["ExcsePrcnt"].ToString();
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
                           "CHeck('chkExdt', 'dvExdt');", true);
                    }

                    if (Request.QueryString["ReApp"] == null)
                    {
                        txtInternalRefNo.Text = EditDS.Tables[1].Rows[0]["InternalRefNo"].ToString();
                        txtCT1DrftRefNo.Text = EditDS.Tables[1].Rows[0]["CT1DraftRefNo"].ToString();
                        txtRefNo.Text = EditDS.Tables[1].Rows[0]["CT1ReferenceNo"].ToString();
                        if (Convert.ToDateTime(EditDS.Tables[1].Rows[0]["RefDate"].ToString()).ToString("dd-MM-yyyy")
                            == DateTime.MaxValue.ToString("dd-MM-yyyy"))
                            txtRefDate.Text = "";
                        else
                            txtRefDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[1].Rows[0]["RefDate"].ToString()));
                        txtNoOfAREForms.Text = EditDS.Tables[1].Rows[0]["NoofARE1Forms"].ToString();
                    }
                    else
                    {
                        txtInternalRefNo.Text = "";
                        string CT1Count = EditDS.Tables[5].Rows[0]["CT1Count"].ToString();
                        txtCT1DrftRefNo.Text = "CT1Draft/" + Session["UserID"] + "/" + CT1Count + "/" + CommonBLL.FinacialYearShort + "";
                    }

                    txtCEDAmt.Text = (decimal.Round(Convert.ToDecimal(EditDS.Tables[2].Rows[0]["CExDAmount"].ToString()), 0,
                        MidpointRounding.AwayFromZero)).ToString("N");
                    txtExRegNo.Text = EditDS.Tables[2].Rows[0]["ExRegNo"].ToString();
                    txtECCNo.Text = EditDS.Tables[2].Rows[0]["ECCNo"].ToString();
                    txtRange.Text = EditDS.Tables[2].Rows[0]["ExRange"].ToString();
                    txtDivision.Text = EditDS.Tables[2].Rows[0]["Division"].ToString();
                    txtCommissioneRate.Text = EditDS.Tables[2].Rows[0]["Commissionerate"].ToString();
                    txtVRange.Text = EditDS.Tables[2].Rows[0]["VoltaExRange"].ToString();
                    txtVDivision.Text = EditDS.Tables[2].Rows[0]["VoltaDivision"].ToString();
                    txtVCommissionerate.Text = EditDS.Tables[2].Rows[0]["VoltaCommissionerate"].ToString();
                    txtDutyDrawBackNo.Text = EditDS.Tables[2].Rows[0]["DutyDrawbackSNo"].ToString();
                    if (Convert.ToDateTime(EditDS.Tables[2].Rows[0]["MaterialDispatchDate"].ToString()).ToString("dd-MM-yyyy")
                        == DateTime.MaxValue.ToString("dd-MM-yyyy"))
                        txtDispatchMdt.Text = "";
                    else
                        txtDispatchMdt.Text =
                            CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[2].Rows[0]["MaterialDispatchDate"].ToString()));
                    txtFactryAdd.Text = EditDS.Tables[2].Rows[0]["FactoryAddress"].ToString();
                    HFSupplierAdd.Value = EditDS.Tables[2].Rows[0]["FactoryAddress"].ToString();
                    txtMaterlGatePass.Text = EditDS.Tables[2].Rows[0]["MaterialDescription"].ToString();
                    txtHeadingNumbers.Text = EditDS.Tables[2].Rows[0]["TariffHedingNo"].ToString();


                    DataTable ItemsTable = new DataTable();
                    ItemsTable = EditDS.Tables[3].Copy();
                    DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                    DCCheck.DefaultValue = true;
                    ItemsTable.Columns.Add(DCCheck);
                    DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                    DCTotalAmt.DefaultValue = 0;
                    ItemsTable.Columns.Add(DCTotalAmt);
                    for (int k = 0; k < ItemsTable.Rows.Count; k++)
                    {
                        Guid itemId = new Guid(EditDS.Tables[3].Rows[k]["ItemId"].ToString());
                        DataRow[] foundRows = EditDS.Tables[4].Select("ItemId = '" + itemId + "'");

                        if (foundRows.Length > 0)
                        {
                            double Quantity = Convert.ToDouble(foundRows[0]["Quantity"].ToString());
                            double Rate = Convert.ToDouble(foundRows[0]["Rate"].ToString());
                            double ExDutyPercentage = 0;
                            if (foundRows[0]["ExDutyPercentage"].ToString() != "")
                                ExDutyPercentage = Convert.ToDouble(foundRows[0]["ExDutyPercentage"].ToString());
                            double PackingPercentage = 0;
                            if (foundRows[0]["PackingPercentage"].ToString() != "")
                                PackingPercentage = Convert.ToDouble(foundRows[0]["PackingPercentage"].ToString());
                            double DiscountPercentage = 0;
                            if (foundRows[0]["DiscountPercentage"].ToString() != "")
                                DiscountPercentage = Convert.ToDouble(foundRows[0]["DiscountPercentage"].ToString());
                            Guid ItemStatus = Guid.Empty;
                            if (foundRows[0]["ItemStatus"].ToString() != "")
                                ItemStatus = new Guid(foundRows[0]["ItemStatus"].ToString());
                            ItemsTable.Rows[k]["HSCode"] = foundRows[0]["HSCode"].ToString();
                            ItemsTable.Rows[k]["Check"] = true;
                            ItemsTable.Rows[k]["Quantity"] = Quantity;
                            ItemsTable.Rows[k]["Rate"] = Rate;
                            ItemsTable.Rows[k]["ExDutyPercentage"] = ExDutyPercentage;
                            ItemsTable.Rows[k]["PackingPercentage"] = PackingPercentage;
                            ItemsTable.Rows[k]["DiscountPercentage"] = DiscountPercentage;
                            ItemsTable.Rows[k]["ItemStatus"] = ItemStatus;
                        }
                        else
                            ItemsTable.Rows[k]["Check"] = false;
                    }
                    chkPkng.Enabled = (from row in ItemsTable.Rows.Cast<DataRow>()
                                       where row.Field<decimal>("PackingPercentage") != 0
                                       select row).Count() > 0 ?
                                        false : true;
                    chkExdt.Enabled = (from row in ItemsTable.Rows.Cast<DataRow>()
                                       where row.Field<decimal>("ExDutyPercentage") != 0
                                       select row).Count() > 0 ?
                                       false : true;
                    chkDsnt.Enabled = (from row in ItemsTable.Rows.Cast<DataRow>()
                                       where row.Field<decimal>("DiscountPercentage") != 0
                                       select row).Count() > 0 ?
                                       false : true;

                    ItemsTable.AcceptChanges();
                    ItemsTable = Calculations(ItemsTable);
                    if (EditDS.Tables[1].Rows[0]["Attachments"].ToString() != "")
                    {
                        string[] all = EditDS.Tables[1].Rows[0]["Attachments"].ToString().Split(',');
                        StringBuilder sb = new StringBuilder();
                        ArrayList attms = new ArrayList();
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Length; k++)
                        {
                            if (all[k].ToString() != "")
                            {
                                sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                                attms.Add(all[k].ToString());
                            }
                        }
                        sb.Append("</select>");
                        Session["CtoneUplds"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                        divListBox.InnerHtml = "";

                    if (EditDS.Tables.Count > 5)
                    {
                        Session["AllSubItems"] = CommonBLL.CT1ItemsTable_SubItems();
                        DataTable SubItms = EditDS.Tables[5].Copy();
                        DataColumn dc = new DataColumn("Check", typeof(Boolean));
                        dc.DefaultValue = true;
                        if (!SubItms.Columns.Contains("check"))
                            SubItms.Columns.Add(dc);
                        if (!SubItms.Columns.Contains("TotalAmt"))
                        {
                            DataColumn TotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                            TotalAmt.DefaultValue = 0;
                            SubItms.Columns.Add(TotalAmt);
                        }
                        SubItms.AcceptChanges();
                        Session["AllSubItems"] = SubItms;
                    }

                    DivItemDetails.InnerHtml = FillItemDetails(ItemsTable, "");
                    if (Request.QueryString["ReApp"] == null)
                    {
                        btnSave.Text = "Update";
                        DvTerms.Visible = true;
                        ddlIOMRefNo.Enabled = false;
                        DivComments.Visible = true;
                        txtRefNo.Enabled = true;
                        txtRefDate.Enabled = true;
                        txtNoOfAREForms.Enabled = true;
                    }
                    else
                    {
                        btnSave.Text = "Save";
                        txtRefNo.Enabled = false;
                        txtRefDate.Enabled = false;
                        txtNoOfAREForms.Enabled = false;
                        DvTerms.Visible = true;
                    }


                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is ussed to Clear ALL Controls and Sessions
        /// </summary>
        private void ClearAll()
        {
            Session["SelectedItems"] = null;
            Session["AllSubItems"] = null;
            Session["CTOneItems"] = null;
            Session["HSCodeID"] = null;
            Session["HSCodeID"] = null;
            ddlIOMRefNo.Enabled = true;
            txtBondBalance.Text = "";
            txtCEDAmt.Text = "";
            txtCommissioneRate.Text = "";
            ddlHypothecation.SelectedIndex = ddlUnits.SelectedIndex = -1;
            txtCustomer.Text = "";
            txtDate.Text = txtDispatchMdt.Text = txtDivision.Text = txtDutyDrawBackNo.Text = txtECCNo.Text = "";
            txtExRegNo.Text = txtFactryAdd.Text = txtHeadingNumbers.Text = txtImpInstructions.Text = txtInternalRefNo.Text = "";
            txtMaterlGatePass.Text = txtNoOfAREForms.Text = txtProfInv.Text = txtRange.Text = txtRefDate.Text = "";
            txtRefNo.Text = txtSupplierNm.Text = txtVCommissionerate.Text = txtVDivision.Text = "";
            txtVRange.Text = txtComments.Text = DivItemDetails.InnerHtml = "";
            ListBoxFPO.Items.Clear();
            ListBoxLPO.Items.Clear();
            DvTerms.Visible = false;
            txtRefNo.Enabled = false;
            txtRefDate.Enabled = false;
            txtNoOfAREForms.Enabled = false;
            txtCEDAmt.Text = "0";
            txtCtOneVal.Text = "0";

            #region Paging

            Session["RowsDisplay_CT1"] = 100;
            Session["CPage_CT1"] = 1;

            #endregion
        }

        /// <summary>
        /// This is used to calculate quantity, rate,EXPercent,PackingPercent
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="RNo"></param>
        /// <returns></returns>
        private DataTable Calculations(DataTable dt, int RNo)
        {
            try
            {
                decimal TotalAmt = 0;
                bool DscntPrcnt = true, PkngPrcnt = true, ExsePrcnt = true;
                DataTable Subdt = (DataTable)Session["AllSubItems"];
                if (RNo == -1)
                {
                    #region CT-1 Items

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dt.Rows[i]["Check"]))
                        {
                            ExdutyAmt = 0;
                            Decimal Rate = Convert.ToDecimal(dt.Rows[i]["Rate"].ToString());

                            //Discount Percentage Calculation
                            if (dt.Rows[i]["DiscountPercentage"].ToString() == "" || dt.Rows[i]["DiscountPercentage"].ToString() == "0.00")
                                dt.Rows[i]["DiscountPercentage"] = 0.00;
                            else
                                DscntPrcnt = false;
                            TotalAmt = Rate - (Convert.ToDecimal(dt.Rows[i]["DiscountPercentage"].ToString()) * Rate) / 100;

                            //Packing Percentage Calculation
                            if (dt.Rows[i]["PackingPercentage"].ToString() == "" || dt.Rows[i]["PackingPercentage"].ToString() == "0.00")
                                dt.Rows[i]["PackingPercentage"] = 0.00;
                            else
                                PkngPrcnt = false;
                            TotalAmt = TotalAmt + (Convert.ToDecimal(dt.Rows[i]["PackingPercentage"].ToString()) * TotalAmt) / 100;

                            //Ex-Duty Percentage Calculation
                            if (dt.Rows[i]["ExDutyPercentage"].ToString() == "" || dt.Rows[i]["ExDutyPercentage"].ToString() == "0.00")
                                dt.Rows[i]["ExDutyPercentage"] = 0.00;
                            else
                                ExsePrcnt = false;
                            TotalAmt = TotalAmt + (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                            TotalAmt = Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) * TotalAmt;

                            //ExdutyAmt += (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                            dt.Rows[i]["TotalAmt"] = TotalAmt;
                        }
                    }

                    //chkDsnt.Enabled = DscntPrcnt; chkPkng.Enabled = PkngPrcnt; chkExdt.Enabled = ExsePrcnt;
                    #endregion

                    #region SubItems

                    if (Subdt != null && Subdt.Rows.Count > 0)
                    {
                        for (int i = 0; i < Subdt.Rows.Count; i++)
                        {
                            if (Convert.ToBoolean(Subdt.Rows[i]["Check"]))
                            {
                                ExdutyAmt = 0;
                                Decimal Rate = Convert.ToDecimal(Subdt.Rows[i]["Rate"].ToString());

                                //Discount Percentage Calculation
                                if (Subdt.Rows[i]["DiscountPercentage"].ToString() == "" || Subdt.Rows[i]["DiscountPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["DiscountPercentage"] = 0.00;
                                else
                                    DscntPrcnt = false;
                                TotalAmt = Rate - (Convert.ToDecimal(Subdt.Rows[i]["DiscountPercentage"].ToString()) * Rate) / 100;

                                //Packing Percentage Calculation
                                if (Subdt.Rows[i]["PackingPercentage"].ToString() == "" || Subdt.Rows[i]["PackingPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["PackingPercentage"] = 0.00;
                                else
                                    PkngPrcnt = false;
                                TotalAmt = TotalAmt + (Convert.ToDecimal(Subdt.Rows[i]["PackingPercentage"].ToString()) * TotalAmt) / 100;

                                //Ex-Duty Percentage Calculation
                                if (Subdt.Rows[i]["ExDutyPercentage"].ToString() == "" || Subdt.Rows[i]["ExDutyPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["ExDutyPercentage"] = 0.00;
                                else
                                    ExsePrcnt = false;
                                TotalAmt = TotalAmt + (Convert.ToDecimal(Subdt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                                TotalAmt = Convert.ToDecimal(Subdt.Rows[i]["Quantity"].ToString()) * TotalAmt;

                                //ExdutyAmt += (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                                Subdt.Rows[i]["TotalAmt"] = TotalAmt;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region CT-1 Items

                    if (Convert.ToBoolean(dt.Rows[RNo]["Check"]))
                    {
                        Decimal Rate = Convert.ToDecimal(dt.Rows[RNo]["Rate"].ToString());

                        //Discount Percentage Calculation
                        if (dt.Rows[RNo]["DiscountPercentage"].ToString() == "" || dt.Rows[RNo]["DiscountPercentage"].ToString() == "0")
                            dt.Rows[RNo]["DiscountPercentage"] = 0;
                        else
                            DscntPrcnt = false;
                        TotalAmt = Rate - (Convert.ToDecimal(dt.Rows[RNo]["DiscountPercentage"].ToString()) * Rate) / 100;

                        //Packing Percentage Calculation
                        if (dt.Rows[RNo]["PackingPercentage"].ToString() == "" || dt.Rows[RNo]["PackingPercentage"].ToString() == "0")
                            dt.Rows[RNo]["PackingPercentage"] = 0;
                        else
                            PkngPrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(dt.Rows[RNo]["PackingPercentage"].ToString()) * TotalAmt) / 100;

                        //Ex-Duty Percentage Calculation
                        if (dt.Rows[RNo]["ExDutyPercentage"].ToString() == "" || dt.Rows[RNo]["ExDutyPercentage"].ToString() == "0")
                            dt.Rows[RNo]["ExDutyPercentage"] = 0;
                        else
                            ExsePrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(dt.Rows[RNo]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                        TotalAmt = Convert.ToDecimal(dt.Rows[RNo]["Quantity"].ToString()) * TotalAmt;
                        //ExdutyAmt += (Convert.ToDecimal(dt.Rows[RNo]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                        dt.Rows[RNo]["TotalAmt"] = TotalAmt;

                        ChkExduty = false;
                        ChkExdutyAmt = 0;
                    }
                    #endregion

                    #region SubItems
                    string ParentItmID = dt.Rows[RNo]["ItemID"].ToString();
                    DataRow[] result = Subdt.Select("ParentItemId = '" + ParentItmID + "'");
                    foreach (var row in result)
                    {
                        Decimal Rate = Convert.ToDecimal(row["Rate"].ToString());

                        //Discount Percentage Calculation
                        if (row["DiscountPercentage"].ToString() == "" || row["DiscountPercentage"].ToString() == "0")
                            row["DiscountPercentage"] = 0;
                        else
                            DscntPrcnt = false;
                        TotalAmt = Rate - (Convert.ToDecimal(row["DiscountPercentage"].ToString()) * Rate) / 100;

                        //Packing Percentage Calculation
                        if (row["PackingPercentage"].ToString() == "" || row["PackingPercentage"].ToString() == "0")
                            row["PackingPercentage"] = 0;
                        else
                            PkngPrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(row["PackingPercentage"].ToString()) * TotalAmt) / 100;

                        //Ex-Duty Percentage Calculation
                        if (row["ExDutyPercentage"].ToString() == "" || row["ExDutyPercentage"].ToString() == "0")
                            row["ExDutyPercentage"] = 0;
                        else
                            ExsePrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(row["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                        TotalAmt = Convert.ToDecimal(row["Quantity"].ToString()) * TotalAmt;
                        //ExdutyAmt += (Convert.ToDecimal(dt.Rows[RNo]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                        row["TotalAmt"] = TotalAmt;

                        ChkExduty = false;
                        ChkExdutyAmt = 0;
                    }

                    #endregion
                }
                dt.AcceptChanges();
                Subdt.AcceptChanges();
                Session["AllSubItems"] = Subdt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
            return dt;
        }

        /// <summary>
        /// Calculation for Discount, Packng and Excise Duty
        /// </summary>
        /// <param name="CommonDt"></param>
        /// <param name="DscntAll"></param>
        /// <param name="PkngAll"></param>
        /// <param name="ExcseAll"></param>
        /// <returns></returns>
        private DataTable Calculations(DataTable CommonDt, decimal DscntAll, decimal PkngAll, decimal ExcseAll)
        {
            try
            {
                decimal TotalAmt = 0;
                for (int i = 0; i < CommonDt.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(CommonDt.Rows[i]["Check"]))
                    {
                        Decimal Rate = Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString());
                        TotalAmt = Rate - (DscntAll * Rate) / 100;
                        TotalAmt = TotalAmt + (PkngAll * TotalAmt) / 100;
                        TotalAmt = TotalAmt + (ExcseAll * TotalAmt) / 100;
                        TotalAmt = Convert.ToDecimal(CommonDt.Rows[i]["Quantity"].ToString()) * TotalAmt;
                        CommonDt.Rows[i]["TotalAmt"] = TotalAmt;
                    }
                }
                CommonDt.AcceptChanges();
                return CommonDt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Calculation of Excise Duty and Packing
        /// </summary>
        /// <param name="CommonDt"></param>
        /// <returns></returns>
        private DataTable Calculations(DataTable CommonDt)
        {
            try
            {
                decimal CalDsnt = 0, CalPkng = 0, CalExcse = 0, TotalAmt = 0;
                if (chkDsnt.Checked)
                {
                    if (txtDsnt.Text.Trim() != "0" && txtDsnt.Text.Trim() != "")
                    {
                        CalDsnt = Convert.ToDecimal(txtDsnt.Text);
                        DiscountPercentAmt = Convert.ToDouble(txtDsnt.Text);
                    }
                }
                if (chkPkng.Checked)
                {
                    if (txtPkng.Text.Trim() != "0" && txtPkng.Text != "")
                    {
                        CalPkng = Convert.ToDecimal(txtPkng.Text);
                        PackingPercentAmt = Convert.ToDouble(txtPkng.Text);
                    }
                }
                if (chkExdt.Checked)
                {
                    if (txtExdt.Text.Trim() != "0" && txtExdt.Text.Trim() != "")
                    {
                        CalExcse = Convert.ToDecimal(txtExdt.Text);
                        ChkExduty = true;
                        ChkExdutyAmt = Convert.ToDouble(txtExdt.Text);
                    }
                }
                for (int i = 0; i < CommonDt.Rows.Count; i++)
                {
                    Decimal Rate = Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString());
                    TotalAmt = Rate - (CalDsnt * Rate) / 100;
                    TotalAmt = TotalAmt + (CalPkng * TotalAmt) / 100;
                    TotalAmt = TotalAmt + (CalExcse * TotalAmt) / 100;
                    TotalAmt = Convert.ToDecimal(CommonDt.Rows[i]["Quantity"].ToString()) * TotalAmt;
                    CommonDt.Rows[i]["TotalAmt"] = TotalAmt;
                }
                CommonDt.AcceptChanges();
                return CommonDt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return null;
            }
        }

        private void Calculations_Sub(DataTable dt, string RNo)
        {
            try
            {
                decimal TotalAmt = 0;
                bool DscntPrcnt = true, PkngPrcnt = true, ExsePrcnt = true;
                DataTable Subdt = (DataTable)Session["AllSubItems"];
                if (RNo == "-1" && RNo != "")
                {
                    #region SubItems

                    if (Subdt != null && Subdt.Rows.Count > 0)
                    {
                        for (int i = 0; i < Subdt.Rows.Count; i++)
                        {
                            if (Convert.ToBoolean(Subdt.Rows[i]["Check"]))
                            {
                                ExdutyAmt = 0;
                                Decimal Rate = Convert.ToDecimal(Subdt.Rows[i]["Rate"].ToString());

                                //Discount Percentage Calculation
                                if (Subdt.Rows[i]["DiscountPercentage"].ToString() == "" || Subdt.Rows[i]["DiscountPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["DiscountPercentage"] = 0.00;
                                else
                                    DscntPrcnt = false;
                                TotalAmt = Rate - (Convert.ToDecimal(Subdt.Rows[i]["DiscountPercentage"].ToString()) * Rate) / 100;

                                //Packing Percentage Calculation
                                if (Subdt.Rows[i]["PackingPercentage"].ToString() == "" || Subdt.Rows[i]["PackingPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["PackingPercentage"] = 0.00;
                                else
                                    PkngPrcnt = false;
                                TotalAmt = TotalAmt + (Convert.ToDecimal(Subdt.Rows[i]["PackingPercentage"].ToString()) * TotalAmt) / 100;

                                //Ex-Duty Percentage Calculation
                                if (Subdt.Rows[i]["ExDutyPercentage"].ToString() == "" || Subdt.Rows[i]["ExDutyPercentage"].ToString() == "0.00")
                                    Subdt.Rows[i]["ExDutyPercentage"] = 0.00;
                                else
                                    ExsePrcnt = false;
                                TotalAmt = TotalAmt + (Convert.ToDecimal(Subdt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                                TotalAmt = Convert.ToDecimal(Subdt.Rows[i]["Quantity"].ToString()) * TotalAmt;

                                //ExdutyAmt += (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                                Subdt.Rows[i]["TotalAmt"] = TotalAmt;
                            }
                        }
                    }
                    #endregion
                }
                else if (RNo != "-1" && RNo != "")
                {
                    #region SubItems
                    string ParentItmID = dt.Rows[Convert.ToInt32(RNo) - 1]["ItemID"].ToString();
                    DataRow[] result = Subdt.Select("ParentItemId = '" + ParentItmID + "'");
                    foreach (var row in result)
                    {
                        Decimal Rate = Convert.ToDecimal(row["Rate"].ToString());

                        //Discount Percentage Calculation
                        if (row["DiscountPercentage"].ToString() == "" || row["DiscountPercentage"].ToString() == "0")
                            row["DiscountPercentage"] = 0;
                        else
                            DscntPrcnt = false;
                        TotalAmt = Rate - (Convert.ToDecimal(row["DiscountPercentage"].ToString()) * Rate) / 100;

                        //Packing Percentage Calculation
                        if (row["PackingPercentage"].ToString() == "" || row["PackingPercentage"].ToString() == "0")
                            row["PackingPercentage"] = 0;
                        else
                            PkngPrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(row["PackingPercentage"].ToString()) * TotalAmt) / 100;

                        //Ex-Duty Percentage Calculation
                        if (row["ExDutyPercentage"].ToString() == "" || row["ExDutyPercentage"].ToString() == "0")
                            row["ExDutyPercentage"] = 0;
                        else
                            ExsePrcnt = false;
                        TotalAmt = TotalAmt + (Convert.ToDecimal(row["ExDutyPercentage"].ToString()) * TotalAmt) / 100;

                        TotalAmt = Convert.ToDecimal(row["Quantity"].ToString()) * TotalAmt;
                        //ExdutyAmt += (Convert.ToDecimal(dt.Rows[RNo]["ExDutyPercentage"].ToString()) * TotalAmt) / 100;
                        row["TotalAmt"] = TotalAmt;

                        ChkExduty = false;
                        ChkExdutyAmt = 0;
                    }

                    #endregion
                }
                Subdt.AcceptChanges();
                Session["AllSubItems"] = Subdt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to fill GridView (Items)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string FillItemDetails(DataTable dt, string ErrMsgQty)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region Paging

                string DisablePrevious = " disabled ", DisableNext = " disabled ";
                int Rows2Display = Convert.ToInt32(Session["RowsDisplay_CT1"].ToString()), CurrentPage = Convert.ToInt32(Session["CPage_CT1"].ToString()), Rows2Skip = 0;
                int RowsCount = dt.Rows.Count, PageCount = 0;

                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToDecimal(RowsCount) / Convert.ToDecimal(Rows2Display)).ToString().Contains('.'))
                        PageCount = (RowsCount / Rows2Display) + 1;
                    else
                        PageCount = RowsCount / Rows2Display;

                    if (CurrentPage > PageCount && PageCount == 1)
                        CurrentPage = 1;
                    else if (CurrentPage > PageCount)
                        CurrentPage--;
                    else if (CurrentPage < 1)
                        CurrentPage++;
                    Rows2Skip = Rows2Display * (CurrentPage - 1);
                    if (CurrentPage == PageCount)
                        DisablePrevious = "";
                    else if (CurrentPage == 1)
                        DisableNext = "";
                    else
                    {
                        DisablePrevious = "";
                        DisableNext = "";
                    }
                    Session["CPage_CT1"] = CurrentPage;
                }

                #endregion

                Session["CTOneItems"] = null;
                Session["CTOneItems"] = dt;
                string ItemIDs = string.Empty;
                bool DscntPrcnt = true, PkngPrcnt = true, ExsePrcnt = true;
                DataSet dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["GenSupID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (Session["ItemUnits"] == null)
                    Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                        CommonBLL.Units);
                DataSet dsUnits = (DataSet)Session["ItemUnits"];
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='rounded-corner' class='rounded-corner'>" +
                "<thead align='left'><tr>");
                sb.Append("<th class='rounded-First'></th><th><input id='ckhHeader' type='checkbox' onchange='CheckHeader()'");
                if (ChkAllStatus)
                    sb.Append(" checked='checked' ");
                sb.Append(" name='ckhheader'/></th>");
                sb.Append("<th>SNo</th>" + "<th style='width:80px;' align='right'>Status</th>" + "<th align='center' width='180px;'>LPO No.</th><th>Item Desc. & Spec</th><th align='center'>HSCode</th>" +
                "<th>Quantity</th><th align='right'>Rate</th><th align='center'>Discount</th><th align='center'>Packing</th> " +
                "<th align='center'>Ex-Duty</th><th style='width:30px;' align='right'>Units</th>" +
                "<th align='right'>TotalAmount</th><th class='rounded-Last' style='width:50px'></th></tr></thead>");
                sb.Append("<tbody class='bcGridViewMain'>");
                if (dt.Rows.Count > 0)
                {
                    double TotalRate = 0;
                    double GrandTotal = 0;
                    double examt = 0;
                    string HSCode = "";

                    #region Paging

                    dt = dt.AsEnumerable().Skip(Rows2Skip).Take(Rows2Display).CopyToDataTable();

                    #endregion

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        double total = 0;
                        string sno = (Rows2Skip + i + 1).ToString();
                        sb.Append("<tr id='" + sno + "' valign='top'>");

                        string ExpandImg = "";
                        //if()
                        if (Convert.ToBoolean(dt.Rows[i]["IsSubItems"]))
                        {
                            ExpandImg = "<span class='gridactionicons'><a id='btnExpand" + sno + "' href='javascript:void(0)' " +
                                             " onclick='ExpandSubItems(" + sno + ")' title='Expand'><img id='" + sno + "' src='../images/expand.png' style='border-style: none; height: 18px; width: 18px;'/></a></span>";
                        }
                        sb.Append("<td align='center'> " + ExpandImg + " </td>");

                        sb.Append("<td><input id='ckhChaild" + sno + "' type='checkbox' onchange='CheckChaild(" + sno + ")'");
                        if (Convert.ToBoolean(dt.Rows[i]["Check"]))
                            sb.Append(" checked='checked' ");
                        sb.Append(" name='ckhChaild'/></td>");

                        sb.Append("<td align='center'>" + sno);
                        sb.Append("<input type='hidden' name='HFESNo'  id='HFESNo" + sno + "' value='" + dt.Rows[i]["FESNo"].ToString() +
                            "' width='2px' style='WIDTH: 2px;'/>");
                        sb.Append("<input type='hidden' name='HFLPOID'  id='HFLPOID" + sno + "' value='" + dt.Rows[i]["LPOID"].ToString() +
                            "' width='2px' style='WIDTH: 2px;'/>");
                        sb.Append("<input type='hidden' name='HItmID'  id='HItmID" + sno + "' value='" + dt.Rows[i]["ItemDetailsId"].ToString() +
                            "' width='5px' style='WIDTH: 5px;'/></td>");
                        if (Convert.ToBoolean(dt.Rows[i]["IsSubItems"]))
                        {
                            #region Item Staus

                            sb.Append("<td align='right'>");
                            sb.Append("<select id='ddlStatus" + sno + "' class='bcAspdropdown' style='width:85px;' onchange='UpdateItems(" + sno + ")'>");

                            //if ((!UnitCodes.ContainsKey(Convert.ToInt32(dt.Rows[i]["SNo"]))))
                            sb.Append("<option value='" + Guid.Empty + "'>-SELECT-</option>");
                            if (Session["ItemStaus"] == null)
                                Session["ItemStatus"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                                    new Guid(Session["CompanyID"].ToString()), CommonBLL.Item_Status_CT1);

                            DataSet dsStatus = (DataSet)Session["ItemStatus"];
                            foreach (DataRow row in dsStatus.Tables[0].Rows)
                            {
                                if (dt.Rows[i]["ItemStatus"].ToString() == row["ID"].ToString())
                                {
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                                        row["Description"].ToString() + "</option>");
                                }
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                            sb.Append("</select>");
                            sb.Append("</td>");

                            #endregion
                        }
                        else
                        {
                            sb.Append("<td align='right'>");
                            sb.Append("</td>");
                        }

                        sb.Append("<td>" + dt.Rows[i]["LPONo"].ToString()
                            + " <input type='hidden' name='HFItemID'  id='HFItemID" + sno + "' value='" + dt.Rows[i]["ItemId"].ToString()
                            + "' width='5px' style='WIDTH: 5px;'/></td>");
                        sb.Append("<td><textarea class='bcAsptextboxmulti' name='txtDesc-Spec' id='txtDesc-Spec" + sno + "' onchange='UpdateItems(" + sno
                            + ")' onMouseUp='return false' onfocus='ExpandTXT(" + sno + "); this.select()' onblur='ReSizeTXT(" + sno
                            + ")' style='height:20px; width:150px; resize:none;'>" + dt.Rows[i]["SpecDes"].ToString().Replace("~~", "&#34;")
                            + "</textarea></td>");
                        sb.Append("<td><input type='text' onfocus='this.select()' onMouseUp='return false' class='bcAsptextbox' size='10px' name='txtHSCode' id='txtHSCode" + sno + "' value = '"
                            + dt.Rows[i]["HSCode"].ToString() + "' onchange='UpdateItems(" + sno + ")' style='width:75px;'/>");
                        sb.Append("<td><input type='text' onfocus='this.select()' onMouseUp='return false' class='bcAsptextbox' name='txtQuantity' dir='rtl' size='05px' style='width:50px;' onchange='UpdateItems(" + sno
                            + ")' id='txtQuantity" + sno + "' value='" + dt.Rows[i]["Quantity"].ToString()
                            + "'onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' " +
                            " onkeypress='return blockNonNumbers(this, event, true, false);' "
                            + " maxlength='6'/></td>");
                        sb.Append("<td align='right' size='05px' style='width:50px;'>" + dt.Rows[i]["Rate"].ToString() + "</td>");
                        sb.Append("<td align='center'><input type='text' onfocus='this.select()' onMouseUp='return false' class='bcAsptextbox' dir='rtl' name='txtDscnt' size='05px' style='width:50px; float:none;' onchange='UpdateItems("
                            + sno + ")' id='txtDscnt" + sno
                            + "' value='" + dt.Rows[i]["DiscountPercentage"].ToString() + "' maxlength='18' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false); CalculateMax(this.id);' " +
                            " onkeypress='return blockNonNumbers(this, event, true, false);' />%</td>");
                        sb.Append("<td align='center'><input type='text' onfocus='this.select()' onMouseUp='return false' class='bcAsptextbox' dir='rtl' name='txtPkingPercentage' size='05px' style='width:50px; float:none;' onchange='UpdateItems("
                            + sno + ")' id='txtPkingPercentage" + sno
                            + "' value='" + dt.Rows[i]["PackingPercentage"].ToString() + "' maxlength='18' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false); CalculateMax(this.id);' " +
                            " onkeypress='return blockNonNumbers(this, event, true, false);' />%</td>");
                        sb.Append("<td align='center'><input type='text' onfocus='this.select()' onMouseUp='return false' class='bcAsptextbox' dir='rtl' name='txtExDuty' size='05px' style='width:50px; float:none;' onchange='UpdateItems("
                            + sno + ")' id='txtExDuty" + sno
                            + "' value='" + dt.Rows[i]["ExDutyPercentage"].ToString() + "' maxlength='18' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false); CalculateMax(this.id);' " +
                            " onkeypress='return blockNonNumbers(this, event, true, false);' />%</td>");
                        sb.Append("<td align='right'>");
                        DataRow[] dr = dsUnits.Tables[0].Select("ID= '" + dt.Rows[i]["UnumsID"] + "'");
                        if (dr.Length > 0)
                        {
                            sb.Append("<label id='lblUnits" + sno + "'  >" + dr[0]["Description"].ToString() + "</label>");
                        }
                        sb.Append("</td>");

                        if (Convert.ToBoolean(dt.Rows[i]["Check"]))
                        {
                            if (dt.Rows[i]["HSCode"].ToString() != "")
                                HSCode += ", " + dt.Rows[i]["HSCode"].ToString();
                            total = Convert.ToDouble(dt.Rows[i]["TotalAmt"].ToString());
                            GrandTotal += total;
                            sb.Append("<td align='right'>" + total.ToString("N") + "</td>");
                            double examt1 = 0;
                            double Rate = Convert.ToDouble(dt.Rows[i]["Rate"].ToString());
                            TotalRate += Rate;
                            double DiscountPercentage = Convert.ToDouble(dt.Rows[i]["DiscountPercentage"].ToString()) > 0
                                ? Convert.ToDouble(dt.Rows[i]["DiscountPercentage"].ToString()) : DiscountPercentAmt;
                            examt1 = Rate - (DiscountPercentage * Rate) / 100;
                            double PackingPercentage = Convert.ToDouble(dt.Rows[i]["PackingPercentage"].ToString()) > 0
                                ? Convert.ToDouble(dt.Rows[i]["PackingPercentage"].ToString()) : PackingPercentAmt;
                            examt1 = examt1 + (PackingPercentage * examt1) / 100;
                            examt1 = Convert.ToDouble(dt.Rows[i]["Quantity"].ToString()) * examt1;
                            if (Convert.ToDouble(dt.Rows[i]["ExDutyPercentage"].ToString()) > 0 || (ChkExduty == true && ChkExdutyAmt > 0))
                            {
                                double exdutyamount = (Convert.ToDouble(dt.Rows[i]["ExDutyPercentage"].ToString())) > 0
                                    ? (Convert.ToDouble(dt.Rows[i]["ExDutyPercentage"].ToString())) : ChkExdutyAmt;
                                examt += (examt1 * exdutyamount / 100);
                            }
                        }
                        else
                            sb.Append("<td align='right'>" + total.ToString("N") + "</td>");
                        sb.Append("<td></td>");
                        sb.Append("</tr>");
                        if (dt.Rows[i]["DiscountPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[i]["DiscountPercentage"].ToString()) != 0)
                        {
                            DscntPrcnt = false;
                        }
                        if (dt.Rows[i]["PackingPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[i]["PackingPercentage"].ToString()) != 0)
                        {
                            PkngPrcnt = false;
                        }
                        if (dt.Rows[i]["ExDutyPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) != 0)
                        {
                            ExsePrcnt = false;
                        }
                    }

                    DataTable dt_Sub = (DataTable)Session["AllSubItems"];
                    if (dt_Sub != null && dt_Sub.Rows.Count > 0)
                    {
                        for (int J = 0; J < dt_Sub.Rows.Count; J++)
                        {
                            if (Convert.ToBoolean(dt_Sub.Rows[J]["Check"]))
                            {
                                if (dt_Sub.Rows[J]["DiscountPercentage"].ToString() != "" && Convert.ToDecimal(dt_Sub.Rows[J]["DiscountPercentage"].ToString()) != 0)
                                    DscntPrcnt = false;
                                if (dt_Sub.Rows[J]["PackingPercentage"].ToString() != "" && Convert.ToDecimal(dt_Sub.Rows[J]["PackingPercentage"].ToString()) != 0)
                                    PkngPrcnt = false;
                                if (dt_Sub.Rows[J]["ExDutyPercentage"].ToString() != "" && Convert.ToDecimal(dt_Sub.Rows[J]["ExDutyPercentage"].ToString()) != 0)
                                    ExsePrcnt = false;
                            }
                        }
                    }

                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th colspan='8' class='rounded-foot-left'><input type='hidden' ID='hdfDscntAll' value='" + DscntPrcnt + "' />");

                    #region Paging

                    string disply = "<option value='" + Rows2Display + "'>" + Rows2Display + "</option>";
                    string disply1 = "<option selected value='" + Rows2Display + "'>" + Rows2Display + "</option>";

                    StringBuilder ss = new StringBuilder();
                    ss.Append("<select id='ddlRowsChanged' onchange='RowsChanged()'>"
                            + "<option value='25'>25</option>"
                            + "<option value='50'>50</option>"
                            + "<option value='100'>100</option>"
                            + "<option value='200'>200</option>"
                        + "</select>");
                    ss.Replace(disply, disply1);

                    sb.Append("RowsCount:" + RowsCount + ",&nbsp; No.of Pages : " + PageCount + ",&nbsp; CurrentPage:" + CurrentPage + ""
                        + "<input type='hidden' id='hfCurrentPage' value='" + CurrentPage + "' /> ,&nbsp;Rows to Display :"
                        + ss
                        + "<input " + DisablePrevious + " type='button' id='btnPrevious' value='Previous' onclick='PrevPage()' style='width:70px'/>"
                        + "<input " + DisableNext + " type='button' id='btnNext' value='Next' onclick='NextPage()'  style='width:70px' /></th>");

                    #endregion


                    sb.Append("<input type='hidden' ID='hdfErrMsg' value='" + ErrMsgQty + "' /><input type='hidden' ID='hdfPkngAll' value='" + PkngPrcnt + "' /><input type='hidden' ID='hdfHSCode' value='" + HSCode.Trim(',') + "' />");
                    sb.Append("<th colspan='2'><input type='hidden' ID='hdfExDutyAmt' value='" +
                        string.Format("{0:0.00}", (decimal.Round(Convert.ToDecimal(examt), 0, MidpointRounding.AwayFromZero)))
                        + "' /><input type='hidden' ID='hdfExseAll' value='" + ExsePrcnt + "' />"
                        + "<input type='hidden' ID='hfTotalRate_Foot' value='" + TotalRate + "' />"
                        + "<input type='hidden' ID='hfGt_Foot' value='" + GrandTotal + "' />");
                    sb.Append("<span>Total Rate : <label id='lblRateTotal'>" + TotalRate + "</label></span></th>");
                    sb.Append("<th colspan='2' align='right'><span>ExDutyTotal : <label id='lblExDuty'>" +
                        (decimal.Round(Convert.ToDecimal(examt), 0, MidpointRounding.AwayFromZero)).ToString("N") + "</label></span></th>");
                    sb.Append("<th colspan='2' align='right'><span>G-Total : <label id='lblGTotal'>" +
                        (decimal.Round(Convert.ToDecimal(GrandTotal), 0, MidpointRounding.AwayFromZero)).ToString("N") + "</label></span></th>");
                    sb.Append("<th class='rounded-foot-right'></th></tr>");
                    sb.Append("</tfoot>");
                }
                sb.Append("</tbody></table>");
                Session["HSCodeID"] = null;
                ChkAllStatus = false;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to ADD SubItems based on LPO ID
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="LPOID"></param>
        /// <param name="LPONo"></param>
        /// <param name="ParentItemID"></param>
        /// <param name="TRid"></param>
        /// <returns></returns>
        private string Fill_SubItemDetails(DataTable dtt, string LPOID, string LPONo, string ParentItemID, string TRid, bool IsAdd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                bool DscntPrcnt = true, PkngPrcnt = true, ExsePrcnt = true;
                string RowID = TRid;
                double LableID = Convert.ToDouble(TRid);
                DataTable dt = (DataTable)Session["AllSubItems"];
                if (dt == null)
                    dt = CommonBLL.CT1ItemsTable_SubItems();

                DataRow[] rslt = dt.Select("LPOID = '" + LPOID + "' AND ParentItemId = '" + ParentItemID + "'");
                if (rslt.Length == 0 && dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["LPOID"] = LPOID;
                    dr["Quantity"] = 0;
                    dr["Rate"] = 0;
                    dr["ExDutyPercentage"] = 0;
                    dr["PackingPercentage"] = 0;
                    dr["DiscountPercentage"] = 0;
                    dr["ParentItemId"] = ParentItemID;
                    dr["Check"] = true;
                    dt.Rows.Add(dr);
                    rslt = dt.Select("LPOID = '" + LPOID + "' AND ParentItemId = '" + ParentItemID + "'");
                }
                if (rslt != null && rslt.Length > 0)
                {
                    double TotalRate = 0;
                    double GrandTotal = 0;
                    double examt = 0;
                    string HSCode = "";
                    GrandTotal = Convert.ToDouble(dt.Compute("Sum(TotalAmt)", "Check = 1"));
                    for (int i = 0; i < rslt.Length; i++)
                    {
                        double total = 0;
                        if (Convert.ToBoolean(rslt[i]["Check"]))
                        {
                            string SNo = (i + 1).ToString();
                            RowID = (TRid + "a" + (i + 1));
                            LableID = (LableID + 0.1);
                            if (rslt[i]["SNo"] != null && rslt[i]["SNo"].ToString() != "")
                                LableID = Convert.ToDouble(rslt[i]["SNo"]);
                            else
                                rslt[i]["SNo"] = LableID;
                            sb.Append("<tr id='" + RowID + "' class='DEL" + TRid + "'>");
                            sb.Append("<td ><input type ='hidden' id='hfSubItemID" + RowID + "' value='" + rslt[i]["ItemId"].ToString() + "'>");
                            sb.Append("</td>");
                            sb.Append("<td ><input type ='hidden' id='hfLPOID" + RowID + "' value='" + rslt[i]["LPOID"].ToString() + "'></td>");
                            sb.Append("<td ><lable id='lblSubSNo" + RowID + "'>" + LableID + "</td>");
                            //rslt[i]["SNo"] = LableID;
                            sb.Append("<td></td>");
                            sb.Append("<td>" + rslt[i]["LPONo"].ToString() + "</td>");
                            sb.Append("<td><textarea id='txtDesc-Spec" + RowID + "' class='bcAsptextboxmulti' onchange='savechanges1(" + TRid + "," + SNo + ",this);' onMouseUp='return false' onfocus='ExpandTXTt(" + SNo + "); this.select()' onblur='ReSizeTXTt(" + SNo
                                + ")' style='height:20px; width:150px; resize:none;'>" + rslt[i]["SpecDes"].ToString() + "" + "</textarea>");
                            if (i == rslt.Length - 1)
                            {
                                //sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' "
                                //        + " class='icons fnOpen'  ID='btnShow" + RowID + "'  title='Add Item to Item Master' onclick='fnOpen(" + SNo
                                //        + ")' ><img src='../images/add.jpeg'/></a></span>");
                            }
                            sb.Append("</td>");
                            sb.Append("<td><input type='text' id='txtHSCode" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' class='bcAsptextbox' style='width:65px;' value='" + rslt[i]["HSCode"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                            sb.Append("<td><input type='text' id='txtQuantity" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' class='bcAsptextbox' style='width:45px;' value='" + rslt[i]["Quantity"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                            sb.Append("<td align='right'><input type='text' id='txtRate" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' class='bcAsptextbox' style='width:65px;' value='" + rslt[i]["Rate"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                            sb.Append("<td align='center'><input type='text' id='txtDscnt" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' dir='rtl' class='bcAsptextbox' style='width:30px; float:none;' value='" + rslt[i]["DiscountPercentage"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/>%</td>");
                            sb.Append("<td align='center'><input type='text' id='txtPking" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' dir='rtl' class='bcAsptextbox' style='width:30px; float:none;' value='" + rslt[i]["PackingPercentage"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/>%</td>");
                            sb.Append("<td align='center'><input type='text' id='txtExDuty" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' dir='rtl' class='bcAsptextbox' style='width:30px; float:none;' value='" + rslt[i]["ExDutyPercentage"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/>%</td>");

                            #region Fill Units Dropdown
                            sb.Append("<td align='right'>");
                            sb.Append("<select id='ddlUnits" + RowID + "' class='bcAspdropdown' style='width:70px;' onchange='savechanges1(" + TRid + "," + SNo + ",this);'>");

                            //if ((!UnitCodes.ContainsKey(Convert.ToInt32(dt.Rows[i]["SNo"]))))
                            sb.Append("<option value='0'>-SELECT-</option>");
                            if (Session["ItemUnits"] == null)
                                Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                                    new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                            DataSet dsUnits = (DataSet)Session["ItemUnits"];
                            foreach (DataRow row in dsUnits.Tables[0].Rows)
                            {
                                if (rslt[i]["UnumsID"].ToString() == row["ID"].ToString())
                                {
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                                        row["Description"].ToString() + "</option>");
                                }
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                            sb.Append("</select>");
                            sb.Append("</td>");
                            #endregion

                            #region Calculations

                            if (Convert.ToBoolean(rslt[i]["Check"]))
                            {
                                //txtHeadingNumbers
                                if (rslt[i]["HSCode"].ToString() != "")
                                    HSCode += ", " + rslt[i]["HSCode"].ToString();
                                //else
                                //    HSCode += dt.Rows[i]["HSCode"].ToString();

                                total = Convert.ToDouble(rslt[i]["TotalAmt"].ToString());// *Convert.ToDouble(dt.Rows[i]["Rate"].ToString());
                                //GrandTotal += total;
                                //sb.Append("<td align='right'>" + total.ToString("N") + "</td>");


                                double examt1 = 0;
                                double Rate = Convert.ToDouble(rslt[i]["Rate"].ToString());
                                TotalRate += Rate;

                                //Discount Percentage Calculation
                                //double DiscountPercentage = Convert.ToDouble(rslt[i]["DiscountPercentage"].ToString()) > 0
                                //    ? Convert.ToDouble(rslt[i]["DiscountPercentage"].ToString()) : DiscountPercentAmt;
                                //examt1 = Rate - (DiscountPercentage * Rate) / 100;

                                //Packing Percentage Calculation
                                //double PackingPercentage = Convert.ToDouble(rslt[i]["PackingPercentage"].ToString()) > 0
                                //    ? Convert.ToDouble(rslt[i]["PackingPercentage"].ToString()) : PackingPercentAmt;
                                //examt1 = examt1 + (PackingPercentage * examt1) / 100; ///recent comment for removing Sub items Calculation 

                                examt1 = Convert.ToDouble(rslt[i]["Quantity"].ToString()) * Rate;
                                //if (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString()) > 0 || (ChkExduty == true && ChkExdutyAmt > 0))
                                //{
                                //    double exdutyamount = (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString())) > 0
                                //        ? (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString())) : ChkExdutyAmt;
                                //    examt += (examt1 * exdutyamount / 100);
                                //}//recent comment for removing Sub items Calculation
                            }

                            sb.Append("<td align='right'>" + total.ToString("N"));
                            if (i == (rslt.Length - 1))
                            {
                                sb.Append("<input type ='hidden' id='hfGT_sub' value='" + GrandTotal + "'>");
                                sb.Append("<input type ='hidden' id='hfRateT_sub' value='" + TotalRate + "'>");
                                sb.Append("<input type ='hidden' id='hfExDtT_sub' value='" + examt + "'>");

                                for (int J = 0; J < dt.Rows.Count; J++)
                                {
                                    if (Convert.ToBoolean(dt.Rows[J]["Check"]))
                                    {
                                        if (dt.Rows[J]["DiscountPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["DiscountPercentage"].ToString()) != 0)
                                            DscntPrcnt = false;
                                        if (dt.Rows[J]["PackingPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["PackingPercentage"].ToString()) != 0)
                                            PkngPrcnt = false;
                                        if (dt.Rows[J]["ExDutyPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["ExDutyPercentage"].ToString()) != 0)
                                            ExsePrcnt = false;
                                    }
                                }
                                sb.Append("<input type='hidden' ID='hdfDscntAll_Sub' value='" + DscntPrcnt + "' />"
                                    + " <input type='hidden' ID='hdfPkngAll_Sub' value='" + PkngPrcnt + "' />"
                                    + " <input type='hidden' ID='hdfExseAll_Sub' value='" + ExsePrcnt + "' />");
                            }
                            sb.Append("</td>");
                            #endregion

                            #region Buttons

                            sb.Append("<td valign='center'>");
                            //if (rslt.Length == 1)
                            //    sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;<span class='gridactionicons'><a href='javascript:void(0)'" +
                            //" onclick='Add_Sub_Itms(" + SNo + ")'" +
                            //" class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></span>");
                            //else 
                            if (rslt.Length - 1 == i)
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' " +
                                    " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span>&nbsp;&nbsp;" +
                                    "<a href='javascript:void(0)' onclick='Add_Sub_Itms(" + SNo + ")' " +
                                    " class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a>");
                            else
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' " +
                                    " onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' class='icons deleteicon' " +
                                    " title='Delete' OnClientClick='javascript:return doConfirm();'>" +
                                    " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                            sb.Append("</td>");
                            #endregion
                            sb.Append("</tr>");
                        }
                    }
                }
                dt.AcceptChanges();
                Session["AllSubItems"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
            return sb.ToString();
        }


        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Send E-Mails After Generation of GRN 
        /// </summary>
        /// <param name="LpoNumber"></param>
        /// <param name="CT1Number"></param>
        /// <param name="GrnNumber"></param>
        /// <param name="SuplrName"></param>
        /// <param name="PrfmaInvcNmbr"></param>
        protected void SendDefaultMails(string LPOs, string CT1DraftNo, string date)
        {
            try
            {
                string[] LPONos = LPOs.Split(',');
                string lps = "";
                for (int i = 0; i < LPONos.Length; i++)
                {
                    if (i == 0)
                        lps = "     1) " + LPONos[i] + System.Environment.NewLine;
                    else
                        lps = lps + "    " + (i + 1) + ") " + LPONos[i] + System.Environment.NewLine;
                }
                string CcAddrs = "";
                SupplierBLL spbll = new SupplierBLL();
                DataSet ds = new DataSet();
                if (PinvReqID != "" && PinvReqID != "0")
                    ds = spbll.SelectSuppliers(CommonBLL.FlagZSelect, new Guid(PinvReqID), Guid.Empty);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    CcAddrs += ", " + ds.Tables[0].Rows[0]["PriEmail"].ToString();

                string Rslt1 = CommonBLL.SendMails(Session["UserMail"].ToString(), CcAddrs, "Information about CT-1 Preparation",
                                            InformationCTONEDtls(lps, CT1DraftNo, date));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about CT-1 Details
        /// </summary>
        /// <param name="LPOsDts"></param>
        /// <param name="AREdtls"></param>
        /// <returns></returns>
        protected string InformationCTONEDtls(string LPONos, string CT1DraftNmbr, string Date)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine);
                sb.Append("SUB: CT-1 Prepared " + System.Environment.NewLine);
                sb.Append(" W.R.T your LPO No. " + System.Environment.NewLine + LPONos + System.Environment.NewLine
                    + " CT-1 Draft No. " + CT1DraftNmbr + " Dt: " + Date + " is generated. ");
                sb.Append("Please collect the copies from Excise Department and prepare Dispatch Instructions to send it to the Supplier.");

                sb.Append(System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + "Admin ");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// This is used to Upload Files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileUploadComplete(object sender, EventArgs e)
        {
            try
            {
                if (AsyncFileUpload1.HasFile)
                {
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["CtoneUplds"] != null)
                        {
                            alist = (ArrayList)Session["CtoneUplds"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["CtoneUplds"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["CtoneUplds"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('File Size is more than 25MB, Resize and Try Again');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["CtoneUplds"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["CtoneUplds"];
                    StringBuilder sb = new StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        sb.Append("</select>");
                    }
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        # endregion

        # region DDl Events

        /// <summary>
        /// This is used to bind Details when IOM no is Selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlIOMRefNo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListBoxFPO.Items.Clear();
                ListBoxLPO.Items.Clear();
                if (ddlIOMRefNo.SelectedValue != "0")
                    FillInputs(IOMTBLL.GetData(CommonBLL.FlagZSelect, new Guid(ddlIOMRefNo.SelectedValue),
                        "", "", "", DateTime.Now, Guid.Empty, "", DateTime.Now, "", "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString())));
                else
                {
                    ClearAll();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to select Hypothencation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlHypothecation_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Hypoth_SelectedVal();
                DivItemDetails.InnerHtml = FillItemDetails((DataTable)Session["CTOneItems"], "");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Unit Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlUnits.SelectedValue != "0")
                {
                    SupplierUnitsBLL SUBLL = new SupplierUnitsBLL();
                    DataSet SUpds = SUBLL.Select(CommonBLL.FlagSelectAll, new Guid(ddlUnits.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (SUpds.Tables.Count > 0 && SUpds.Tables[0].Rows.Count > 0)
                        txtFactryAdd.Text = SUpds.Tables[0].Rows[0]["FctryAdrs"].ToString();
                    txtRange.Text = SUpds.Tables[0].Rows[0]["Range"].ToString();
                    txtDivision.Text = SUpds.Tables[0].Rows[0]["Division"].ToString();
                    txtCommissioneRate.Text = SUpds.Tables[0].Rows[0]["Commissionerate"].ToString();
                    txtVRange.Text = SUpds.Tables[0].Rows[0]["RangeAddress"].ToString();
                    txtVDivision.Text = SUpds.Tables[0].Rows[0]["DivisionAddress"].ToString();
                    txtVCommissionerate.Text = SUpds.Tables[0].Rows[0]["CommissionerateAddress"].ToString();
                }
                else
                {
                    txtFactryAdd.Text = HFhypothecation.Value;
                    txtRange.Text = txtDivision.Text = txtCommissioneRate.Text = "";
                    txtVDivision.Text = txtVCommissionerate.Text = txtVRange.Text = "";
                    Hypoth_SelectedVal();
                }
                DivItemDetails.InnerHtml = FillItemDetails((DataTable)Session["CTOneItems"], "");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
                           "CHeck('chkDsnt', 'dvDsnt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
                           "CHeck('chkExdt', 'dvExdt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
                           "CHeck('chkPkng', 'dvPkng');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        ///// <summary>
        ///// List of FPO No's for Tool Tip
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ListBoxFPO_PreRender(object sender, EventArgs e)
        //{
        //    foreach (ListItem FPOItems in ListBoxFPO.Items)
        //        FPOItems.Attributes.Add("title", FPOItems.Text);
        //}

        ///// <summary>
        ///// List of LPO No's for Tool Tip
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ListBoxLPO_PreRender(object sender, EventArgs e)
        //{
        //    foreach (ListItem LPOItems in ListBoxLPO.Items)
        //        LPOItems.Attributes.Add("title", LPOItems.Text);
        //}

        ///// <summary>
        ///// Excise Duty Text Changed Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void txtExdt_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DivItemDetails.InnerHtml = FillItemDetails(Calculations((DataTable)Session["CTOneItems"]));
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
        //                   "CHeck('chkExdt', 'dvExdt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
        //                   "CHeck('chkPkng', 'dvPkng');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
        //                   "CHeck('chkDsnt', 'dvDsnt');", true);
        //        //'CHeck("chkExdt", "dvExdt")'
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
        //    }
        //}

        ///// <summary>
        ///// Packing Persentage Text Changed Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void txtPkng_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DivItemDetails.InnerHtml = FillItemDetails(Calculations((DataTable)Session["CTOneItems"]));
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
        //                   "CHeck('chkPkng', 'dvPkng');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
        //                   "CHeck('chkExdt', 'dvExdt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
        //                   "CHeck('chkDsnt', 'dvDsnt');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
        //    }
        //}

        ///// <summary>
        ///// Discount Persentage Text Changed Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void txtDsnt_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DivItemDetails.InnerHtml = FillItemDetails(Calculations((DataTable)Session["CTOneItems"]));
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
        //                   "CHeck('chkDsnt', 'dvDsnt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
        //                   "CHeck('chkExdt', 'dvExdt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
        //                   "CHeck('chkPkng', 'dvPkng');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
        //    }
        //}

        private DataTable AddNewRow(DataTable dt, string SubRowID, string LPOID, string ParentItemId)
        {
            try
            {
                DataRow dr = dt.NewRow();
                dr["SNo"] = (Convert.ToDouble(SubRowID) + 0.1);
                dr["LPOID"] = LPOID;
                dr["Quantity"] = 0;
                dr["Rate"] = 0;
                dr["ExDutyPercentage"] = 0;
                dr["PackingPercentage"] = 0;
                dr["DiscountPercentage"] = 0;
                dr["ParentItemId"] = ParentItemId;
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return null;
            }
        }

        # endregion

        # region ButtonClicks

        /// <summary>
        /// This is Used to Save Records And Edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int res = 1;
                int flag = 0;
                string ErrorMsg = "";
                string RefNo = "";
                int AreForms = 0;
                string FPos = String.Join(", ", ListBoxFPO.Items.Cast<ListItem>().Select(i => i.Value).ToArray());
                string LPos = String.Join(", ", ListBoxLPO.Items.Cast<ListItem>().Select(i => i.Value).ToArray());
                string LPONumbers = String.Join(", ", ListBoxLPO.Items.Cast<ListItem>().Select(i => i.Text).ToArray());
                if (txtRefDate.Text != "")
                    CTOneRefDT = CommonBLL.DateInsert(txtRefDate.Text);
                string Status = "Draft";
                if (txtRefNo.Text.Trim() != "" && txtRefDate.Text != "" && txtNoOfAREForms.Text != "" &&
                    Convert.ToInt32(txtNoOfAREForms.Text.Trim()) > 0)
                {
                    Status = "Confirm";
                    RefNo = txtRefNo.Text.Trim();
                    AreForms = Convert.ToInt32(txtNoOfAREForms.Text.Trim());
                }
                Guid IOMRefNo = new Guid(ddlIOMRefNo.SelectedValue);
                decimal CExDAmt = Convert.ToDecimal(txtCEDAmt.Text.Trim());
                DataTable CT1Items = (DataTable)Session["CTOneItems"];
                DataTable CheckedRows = CommonBLL.CT1ItemsTable();
                CheckedRows.Rows[0].Delete();
                CheckedRows.Columns.Add("Check");
                CheckedRows.Columns.Add("TotalAmt");
                string[] CheckedItems = HselectedItems.Value.Split(',');
                CheckedItems = CheckedItems.Where(s => !String.IsNullOrEmpty(s)).ToArray();
                foreach (DataRow dr in CT1Items.Rows)
                {
                    if (Convert.ToBoolean(dr["Check"].ToString()))
                    {
                        CheckedRows.Rows.Add(dr.ItemArray);

                        Guid LPOID = new Guid(dr["LPOID"].ToString());
                        Guid ItemID = new Guid(dr["ItemId"].ToString());
                        GrnBLL gbll = new GrnBLL();
                        DataSet ds = new DataSet();
                        ds = gbll.SelectGrnDtls(CommonBLL.FlagKSelect, LPOID, ItemID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (ds.Tables.Count > 0)
                        {
                            decimal newQty = 0;
                            decimal CT1SumQty = Convert.ToDecimal(dr["Quantity"].ToString());

                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
                                newQty = Convert.ToDecimal(ds.Tables[0].Rows[0]["ReceivedQty"].ToString());

                            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0][0].ToString() != "")
                                CT1SumQty += Convert.ToDecimal(ds.Tables[1].Rows[0]["Quantity"].ToString());
                            if (CT1SumQty < newQty)
                            {
                                flag = 1;
                                int index = CT1Items.Rows.IndexOf(dr);
                                ErrorMsg = "Quantity value cannot be less than the sum of GDN/GRN ( " + newQty + " ) in SNO. " + (index + 1) + ".";
                                break;
                            }
                        }
                    }
                }

                DataTable dt_sub = (DataTable)Session["AllSubItems"];
                DataTable SubItms = new DataTable();
                if (dt_sub != null && dt_sub.Rows.Count > 0 && (dt_sub.Rows[0]["ItemID"].ToString() != "" && dt_sub.Rows[0]["ItemID"].ToString() != Guid.Empty.ToString()))
                {
                    SubItms = dt_sub.Copy();
                    for (int i = 0; i < SubItms.Rows.Count; i++)
                    {
                        if (!Convert.ToBoolean(SubItms.Rows[i]["Check"]))
                            SubItms.Rows[i].Delete();
                        if (SubItms.Rows[i]["ItemId"].ToString() == "" ||
                           Convert.ToDecimal(SubItms.Rows[i]["Quantity"].ToString()) <= 0 ||
                            Convert.ToDecimal(SubItms.Rows[i]["Rate"].ToString()) <= 0)
                            SubItms.Rows[i].Delete();
                    }
                    if (SubItms.Columns.Contains("Check"))
                        SubItms.Columns.Remove("Check");
                    if (SubItms.Columns.Contains("TotalAmt"))
                        SubItms.Columns.Remove("TotalAmt");
                    SubItms.AcceptChanges();
                }
                else
                {
                    SubItms = CommonBLL.CT1ItemsTable_SubItems();
                }

                if (flag == 0)
                {
                    CheckedRows.AcceptChanges();
                    if (CheckedRows.Columns.Contains("Check"))
                        CheckedRows.Columns.Remove("Check");
                    if (CheckedRows.Columns.Contains("TotalAmt"))
                        CheckedRows.Columns.Remove("TotalAmt");
                    CheckedRows.AcceptChanges();
                }
                if (Request.QueryString["ID"] != null)
                    AreForms = Convert.ToInt32(txtNoOfAREForms.Text.Trim());
                Guid PrfINVID = Guid.Empty;
                if (HFPrfINVID.Value.Trim() != "" && HFPrfINVID.Value.Trim() != "0")
                    PrfINVID = new Guid(HFPrfINVID.Value.Trim());

                string Attachments = "";
                if (Session["CtoneUplds"] != null)
                {
                    ArrayList all = (ArrayList)Session["CtoneUplds"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                if (btnSave.Text == "Save" && flag == 0)
                {
                    DataSet ds = IOMTBLL.GetData(CommonBLL.FlagOSelect, Guid.Empty, "", "", "", DateTime.Now,
                        Guid.Empty, "", DateTime.Now, "", "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));

                    string CT1Count = ds.Tables[0].Rows[0]["CT1ID"].ToString();
                    string CT1Draft = "CT1Draft/" + Session["AliasName"] + "/" + CT1Count + "/" + CommonBLL.FinacialYearShort + "";

                    Guid CT1ID = Guid.Empty;
                    if (Request.QueryString["ReApp"] != null)
                        CT1ID = new Guid(Request.QueryString["CT1"]);
                    res = CT1Form.InsertUpdateDelete(CommonBLL.FlagNewInsert, CT1ID, new Guid(HFSuplierID.Value), new Guid(ddlHypothecation.SelectedValue),
                        new Guid(ddlUnits.SelectedValue), Convert.ToDecimal(txtCtOneVal.Text.Trim()), 0, RefNo, CTOneRefDT, "", AreForms, Status, CT1Draft,
                        (chkDsnt.Checked ? (txtDsnt.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtDsnt.Text)) : 0),
                        (chkPkng.Checked ? (txtPkng.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtPkng.Text)) : 0),
                        (chkExdt.Checked ? (txtExdt.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtExdt.Text)) : 0), FPos, LPos, new Guid(HFCustID.Value),
                        IOMRefNo, CExDAmt, txtExRegNo.Text.Trim(), txtRange.Text.Trim(), txtDivision.Text.Trim(), txtCommissioneRate.Text.Trim(),
                        txtVRange.Text.Trim(), txtVDivision.Text.Trim(), txtVCommissionerate.Text.Trim(), txtMaterlGatePass.Text.Trim(),
                        txtHeadingNumbers.Text.Trim(), txtDutyDrawBackNo.Text.Trim(), txtDispatchMdt.Text != "" ? CommonBLL.DateInsert(txtDispatchMdt.Text) : DateTime.MaxValue, txtFactryAdd.Text.Trim(),
                        txtECCNo.Text.Trim(), new Guid(ddlLocation.SelectedValue), "", Attachments, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                        new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, CheckedRows, SubItms,
                        PrfINVID);
                    if (res == 0)
                    {
                        if (Request.QueryString["SupID"] != null)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Generation", "Inserted Successfully by supplier.");
                            SendDefaultMails(LPONumbers, CT1Draft, txtDate.Text);
                            ClearAll();
                            Session.Abandon();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "Close_Supplier('Proforma Invoice Added Successfully. If you need to edit click on link in your mail');", true);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Generation", "Inserted Successfully.");
                            Response.Redirect("../Logistics/CTOneStatus.aspx", false);
                        }
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                           "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", "Error while Inserting.");
                    }
                }
                else if (btnSave.Text == "Update" && flag == 0)
                {
                    res = CT1Form.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["CT1ID"].ToString()),
                        new Guid(HFSuplierID.Value), new Guid(ddlHypothecation.SelectedValue),
                        new Guid(ddlUnits.SelectedValue), Convert.ToDecimal(txtCtOneVal.Text.Trim()), 0, RefNo,
                        CTOneRefDT, "", AreForms, Status, txtCT1DrftRefNo.Text.Trim(),
                        (chkDsnt.Checked ? (txtDsnt.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtDsnt.Text)) : 0),
                        (chkPkng.Checked ? (txtPkng.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtPkng.Text)) : 0),
                        (chkExdt.Checked ? (txtExdt.Text.Trim() == "" ? 0 : Convert.ToDecimal(txtExdt.Text)) : 0), FPos, LPos, new Guid(HFCustID.Value),
                        IOMRefNo, CExDAmt, txtExRegNo.Text.Trim(), txtRange.Text.Trim(), txtDivision.Text.Trim(), txtCommissioneRate.Text.Trim(),
                        txtVRange.Text.Trim(), txtVDivision.Text.Trim(), txtVCommissionerate.Text.Trim(), txtMaterlGatePass.Text.Trim(),
                        txtHeadingNumbers.Text.Trim(), txtDutyDrawBackNo.Text.Trim(), txtDispatchMdt.Text != "" ? CommonBLL.DateInsert(txtDispatchMdt.Text) : DateTime.MaxValue,
                        txtFactryAdd.Text.Trim(), txtECCNo.Text.Trim(), new Guid(ddlLocation.SelectedValue), txtComments.Text.Trim(), Attachments,
                        new Guid(ViewState["PINVID"].ToString()), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()),
                        Convert.ToDateTime(DateTime.Now.ToShortDateString()), new Guid(Session["UserID"].ToString()),
                        Convert.ToDateTime(DateTime.Now.ToShortDateString()), true, CheckedRows, SubItms, PrfINVID);
                    if (res == 0)
                    {
                        if (Request.QueryString["SupID"] != null)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Generation", "Updated Successfully by Supplier.");
                            ClearAll();
                            Session.Abandon();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "Close_Supplier('Proforma Invoice Updated Successfully. If you need to edit click on link in your mail');", true);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Generation", "Updated Successfully.");
                            Response.Redirect("../Logistics/CTOneStatus.aspx", false);
                        }
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", "Error while Updating.");
                    }
                }
                else if (flag == 1)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Show", "ErrorMessage('" + ErrorMsg + "');", true);
                    DivItemDetails.InnerHtml = FillItemDetails(CT1Items, ErrorMsg);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxDsnt",
                           "CHeck('chkDsnt', 'dvDsnt');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxExcse",
                               "CHeck('chkExdt', 'dvExdt');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkBxPkng",
                               "CHeck('chkPkng', 'dvPkng');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear ALL Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
            Response.Redirect("../Logistics/CTOneGeneration.aspx", false);
        }

        # endregion

        # region WebMetods

        #region Paging

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ValidateRowsBeforeSave()
        {
            try
            {
                string Error = "";
                DataTable ds = (DataTable)Session["CTOneItems"];
                if (ds != null && ds.Rows.Count > 0)
                {
                    DataRow[] dr = ds.Select("Check = 'true' and (HSCode = '' or Quantity = 0)");
                    if (dr.Length > 0)
                    {
                        string SNO = dr[0]["fesno"].ToString();
                        string qty = dr[0]["Quantity"].ToString();
                        string rate = dr[0]["HSCode"].ToString(), ErrorMsg = "";
                        decimal number;
                        if (!Decimal.TryParse(qty, out number))
                            ErrorMsg = " Quantity ";
                        else if (number == 0)
                            ErrorMsg = " Quantity cannot be Zero";
                        else if (rate == "")
                            ErrorMsg = " Rate cannot be empty";
                        Error = "ERROR::" + ErrorMsg + " in SNo. " + SNO + ".";
                    }
                }
                return Error;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string NextPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_CT1"] = (CPage + 1);
            Session["RowsDisplay_CT1"] = txtRowsChanged;
            DataTable dt = (DataTable)Session["CTOneItems"];
            return FillItemDetails(dt, "");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PrevPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_CT1"] = (CPage - 1);
            Session["RowsDisplay_CT1"] = txtRowsChanged;
            DataTable dt = (DataTable)Session["CTOneItems"];
            return FillItemDetails(dt, "");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string RowsChanged(string CurrentPage, string txtRowsChanged)
        {
            if (txtRowsChanged.Trim() != "")
            {
                if (Convert.ToInt32(Session["RowsDisplay_CT1"].ToString()) != Convert.ToInt32(txtRowsChanged))
                {
                    int RowStart = ((Convert.ToInt32(Session["RowsDisplay_CT1"].ToString()) * Convert.ToInt32(CurrentPage)) - Convert.ToInt32(Session["RowsDisplay_CT1"].ToString())) + 1;

                    if (RowStart > Convert.ToInt32(txtRowsChanged))
                        Session["CPage_CT1"] = RowStart / Convert.ToInt32(txtRowsChanged) + 1;
                    else if (RowStart < Convert.ToInt32(txtRowsChanged) && RowStart != 1)
                        Session["CPage_CT1"] = (Convert.ToInt32(txtRowsChanged) + 1) / RowStart;
                }
                Session["RowsDisplay_CT1"] = Convert.ToInt32(txtRowsChanged);
            }
            DataTable dt = (DataTable)Session["CTOneItems"];
            return FillItemDetails(dt, "");
        }

        #endregion

        /// <summary>
        /// This is used to Save changes in DataTable (ITEMS)
        /// </summary>
        /// <param name="RNo"></param>
        /// <param name="Spec"></param>
        /// <param name="Qty"></param>
        /// <param name="ExDuty"></param>
        /// <param name="txtPckngPrcnt"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(string RNo, string SNo, string Spec, string Qty, string ExDuty, string txtPckngPrcnt, string Dscnt, string HSCode, string ItemStatus)
        {
            string ErrMsgQty = "";
            DataTable dt = (DataTable)Session["CTOneItems"];
            try
            {
                int RowNo = (Convert.ToInt32(RNo) - 1);
                decimal Quantity = Convert.ToDecimal(Qty);
                decimal ExciseDuty = Convert.ToDecimal(ExDuty);
                decimal PckingPercent = Convert.ToDecimal(txtPckngPrcnt);
                dt.Rows[RowNo]["SpecDes"] = Spec.Replace("#@%", "~~");

                Guid LPOID = new Guid(dt.Rows[RowNo]["LPOID"].ToString());
                Guid ItemID = new Guid(dt.Rows[RowNo]["ItemId"].ToString());
                GrnBLL gbll = new GrnBLL();
                DataSet ds = new DataSet();
                ds = gbll.SelectGrnDtls(CommonBLL.FlagKSelect, LPOID, ItemID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0)
                {
                    decimal newQty = 0;
                    decimal CT1SumQty = Quantity;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
                        newQty = Convert.ToDecimal(ds.Tables[0].Rows[0]["ReceivedQty"].ToString());

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0][0].ToString() != "")
                        CT1SumQty += Convert.ToDecimal(ds.Tables[1].Rows[0]["Quantity"].ToString());
                    if (CT1SumQty >= newQty)
                        dt.Rows[RowNo]["Quantity"] = Quantity;
                    else
                        ErrMsgQty = "Quantity value cannot be less than the sum of GDN/GRN ( " + newQty + " )."; ;
                }
                else
                    dt.Rows[RowNo]["Quantity"] = Quantity;
                dt.Rows[RowNo]["FESNo"] = SNo;
                dt.Rows[RowNo]["ExDutyPercentage"] = ExciseDuty;
                dt.Rows[RowNo]["PackingPercentage"] = PckingPercent;
                dt.Rows[RowNo]["DiscountPercentage"] = Dscnt;
                dt.Rows[RowNo]["HSCode"] = HSCode;
                if (ItemStatus != "")
                    dt.Rows[RowNo]["ItemStatus"] = ItemStatus;
                else
                    dt.Rows[RowNo]["ItemStatus"] = Guid.Empty;

                dt = Calculations(dt, RowNo);
                Session["CTOneItems"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
            return FillItemDetails(dt, ErrMsgQty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string Calculations(string ExDuty, string Discount, string Packing, bool chkDsnt, bool chkExdt, bool chkPkng)
        {
            try
            {
                DataTable CommonDt = (DataTable)Session["CTOneItems"];

                bool IsDscnt = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("DiscountPercentage") > 0);
                bool IsExse = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("ExDutyPercentage") > 0);
                bool IsPkng = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("PackingPercentage") > 0);

                if (IsDscnt)
                { Discount = "0"; }
                if (IsExse)
                { ExDuty = "0"; }
                if (IsPkng)
                { Packing = "0"; }

                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    decimal DiscountAmount = 0;
                    decimal Amount = 0;
                    decimal PackageAmt = 0;
                    decimal ExAmt = 0;

                    #region CT-1 Items
                    for (int i = 0; i < CommonDt.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(CommonDt.Rows[i]["Check"]))
                        {
                            if (Convert.ToDecimal(CommonDt.Rows[i]["DiscountPercentage"].ToString()) > 0)
                            {
                                DiscountAmount = ((Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString())) *
                                    (Convert.ToDecimal(CommonDt.Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]) - DiscountAmount;
                            }
                            else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                            {
                                DiscountAmount = ((Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString())) *
                                    (Convert.ToDecimal(Discount))) / 100;
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]) - DiscountAmount;
                            }
                            else
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]);

                            if (Convert.ToDecimal(CommonDt.Rows[i]["PackingPercentage"].ToString()) > 0)
                            {
                                PackageAmt = (Amount * (Convert.ToDecimal(CommonDt.Rows[i]["PackingPercentage"].ToString()))) / 100;
                                Amount = Amount + PackageAmt;
                            }
                            else if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                            {
                                PackageAmt = (Amount * Convert.ToDecimal(Packing)) / 100;
                                Amount = Amount + PackageAmt;
                            }
                            if (Convert.ToDecimal(CommonDt.Rows[i]["ExDutyPercentage"].ToString()) > 0)
                            {
                                ExAmt = (Amount * (Convert.ToDecimal(CommonDt.Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                Amount = Amount + ExAmt;
                            }
                            else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                            {
                                ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                Amount = Amount + ExAmt;
                            }

                            // This is to calculate Packing Percentage Amount
                            if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                                PackingPercentAmt = Convert.ToDouble(Packing);
                            else
                                PackingPercentAmt = 0;

                            // This is to calculate Discount Percentage Amount
                            if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                                DiscountPercentAmt = Convert.ToDouble(Discount);
                            else
                                DiscountPercentAmt = 0;

                            // This is to calculate Ex-Duty Percentage Amount
                            if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                            {
                                ChkExduty = true;
                                ChkExdutyAmt = Convert.ToDouble(ExDuty);
                            }
                            else
                            {
                                ChkExduty = false;
                                ChkExdutyAmt = 0;
                            }

                            CommonDt.Rows[i]["TotalAmt"] = (Amount *
                                Convert.ToDecimal(CommonDt.Rows[i]["Quantity"].ToString()));
                        }
                    }
                    CommonDt.AcceptChanges();
                    Session["CTOneItems"] = CommonDt;
                    #endregion

                    #region CT-1 Sub Items
                    DataTable dt = (DataTable)Session["AllSubItems"];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        decimal DiscountAmount_Sub = 0;
                        decimal Amount_Sub = 0;
                        decimal PackageAmt_Sub = 0;
                        decimal ExAmt_Sub = 0;
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (Convert.ToBoolean(dt.Rows[j]["Check"]))
                            {
                                if (Convert.ToDecimal(dt.Rows[j]["DiscountPercentage"].ToString()) > 0)
                                {
                                    DiscountAmount_Sub = ((Convert.ToDecimal(dt.Rows[j]["Rate"].ToString())) *
                                        (Convert.ToDecimal(dt.Rows[j]["DiscountPercentage"].ToString()))) / 100;
                                    Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]) - DiscountAmount_Sub;
                                }
                                else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                                {
                                    DiscountAmount_Sub = ((Convert.ToDecimal(dt.Rows[j]["Rate"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                    Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]) - DiscountAmount_Sub;
                                }
                                else
                                    Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]);

                                if (Convert.ToDecimal(dt.Rows[j]["PackingPercentage"].ToString()) > 0)
                                {
                                    PackageAmt_Sub = (Amount_Sub * (Convert.ToDecimal(dt.Rows[j]["PackingPercentage"].ToString()))) / 100;
                                    Amount_Sub = Amount_Sub + PackageAmt_Sub;
                                }
                                else if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                                {
                                    PackageAmt_Sub = (Amount_Sub * Convert.ToDecimal(Packing)) / 100;
                                    Amount_Sub = Amount_Sub + PackageAmt_Sub;
                                }
                                if (Convert.ToDecimal(dt.Rows[j]["ExDutyPercentage"].ToString()) > 0)
                                {
                                    ExAmt_Sub = (Amount_Sub * (Convert.ToDecimal(dt.Rows[j]["ExDutyPercentage"].ToString()))) / 100;
                                    Amount_Sub = Amount_Sub + ExAmt_Sub;
                                }
                                else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                                {
                                    ExAmt_Sub = (Amount_Sub * Convert.ToDecimal(ExDuty)) / 100;
                                    Amount_Sub = Amount_Sub + ExAmt_Sub;
                                }

                                // This is to calculate Packing Percentage Amount_Sub
                                if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                                    PackingPercentAmt = Convert.ToDouble(Packing);
                                else
                                    PackingPercentAmt = 0;

                                // This is to calculate Discount Percentage Amount_Sub
                                if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                                    DiscountPercentAmt = Convert.ToDouble(Discount);
                                else
                                    DiscountPercentAmt = 0;

                                // This is to calculate Ex-Duty Percentage Amount_Sub
                                if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                                {
                                    ChkExduty = true;
                                    ChkExdutyAmt = Convert.ToDouble(ExDuty);
                                }
                                else
                                {
                                    ChkExduty = false;
                                    ChkExdutyAmt = 0;
                                }

                                dt.Rows[j]["TotalAmt"] = (Amount_Sub *
                                    Convert.ToDecimal(dt.Rows[j]["Quantity"].ToString()));
                            }
                        }
                        dt.AcceptChanges();
                    }
                    Session["AllSubItems"] = dt;
                    #endregion

                    return FillItemDetails(CommonDt, "");
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillItemDetails(null, "");
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string Calculations_Sub(string ExDuty, string Discount, string Packing, bool chkDsnt, bool chkExdt, bool chkPkng,
            string LPOID, string LPONo, string ParentItemId, string TRid, string IsAdd)
        {
            try
            {
                DataTable CommonDt = (DataTable)Session["CTOneItems"];

                bool IsDscnt = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("DiscountPercentage") > 0);
                bool IsExse = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("ExDutyPercentage") > 0);
                bool IsPkng = CommonDt.AsEnumerable()
                                   .Any(e => e.Field<Decimal>("PackingPercentage") > 0);

                if (IsDscnt)
                { Discount = "0"; }
                if (IsExse)
                { ExDuty = "0"; }
                if (IsPkng)
                { Packing = "0"; }

                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    decimal DiscountAmount = 0;
                    decimal Amount = 0;
                    decimal PackageAmt = 0;
                    decimal ExAmt = 0;

                    #region CT-1 Items
                    for (int i = 0; i < CommonDt.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(CommonDt.Rows[i]["Check"]))
                        {
                            if (Convert.ToDecimal(CommonDt.Rows[i]["DiscountPercentage"].ToString()) > 0)
                            {
                                DiscountAmount = ((Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString())) *
                                    (Convert.ToDecimal(CommonDt.Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]) - DiscountAmount;
                            }
                            else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                            {
                                DiscountAmount = ((Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString())) *
                                    (Convert.ToDecimal(Discount))) / 100;
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]) - DiscountAmount;
                            }
                            else
                                Amount = Convert.ToDecimal(CommonDt.Rows[i]["Rate"]);

                            if (Convert.ToDecimal(CommonDt.Rows[i]["PackingPercentage"].ToString()) > 0)
                            {
                                PackageAmt = (Amount * (Convert.ToDecimal(CommonDt.Rows[i]["PackingPercentage"].ToString()))) / 100;
                                Amount = Amount + PackageAmt;
                            }
                            else if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                            {
                                PackageAmt = (Amount * Convert.ToDecimal(Packing)) / 100;
                                Amount = Amount + PackageAmt;
                            }
                            if (Convert.ToDecimal(CommonDt.Rows[i]["ExDutyPercentage"].ToString()) > 0)
                            {
                                ExAmt = (Amount * (Convert.ToDecimal(CommonDt.Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                Amount = Amount + ExAmt;
                            }
                            else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                            {
                                ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                Amount = Amount + ExAmt;
                            }

                            // This is to calculate Packing Percentage Amount
                            if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                                PackingPercentAmt = Convert.ToDouble(Packing);
                            else
                                PackingPercentAmt = 0;

                            // This is to calculate Discount Percentage Amount
                            if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                                DiscountPercentAmt = Convert.ToDouble(Discount);
                            else
                                DiscountPercentAmt = 0;

                            // This is to calculate Ex-Duty Percentage Amount
                            if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                            {
                                ChkExduty = true;
                                ChkExdutyAmt = Convert.ToDouble(ExDuty);
                            }
                            else
                            {
                                ChkExduty = false;
                                ChkExdutyAmt = 0;
                            }

                            CommonDt.Rows[i]["TotalAmt"] = (Amount *
                                Convert.ToDecimal(CommonDt.Rows[i]["Quantity"].ToString()));
                        }
                    }
                    CommonDt.AcceptChanges();
                    Session["CTOneItems"] = CommonDt;
                    #endregion

                    //Recent Comment For removing Sub Items Calculation
                    #region CT-1 Sub Items
                    DataTable dt = (DataTable)Session["AllSubItems"];
                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    decimal DiscountAmount_Sub = 0;
                    //    decimal Amount_Sub = 0;
                    //    decimal PackageAmt_Sub = 0;
                    //    decimal ExAmt_Sub = 0;
                    //    for (int j = 0; j < dt.Rows.Count; j++)
                    //    {
                    //        if (Convert.ToBoolean(dt.Rows[j]["Check"]))
                    //        {
                    //            if (Convert.ToDecimal(dt.Rows[j]["DiscountPercentage"].ToString()) > 0)
                    //            {
                    //                DiscountAmount_Sub = ((Convert.ToDecimal(dt.Rows[j]["Rate"].ToString())) *
                    //                    (Convert.ToDecimal(dt.Rows[j]["DiscountPercentage"].ToString()))) / 100;
                    //                Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]) - DiscountAmount_Sub;
                    //            }
                    //            else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                    //            {
                    //                DiscountAmount_Sub = ((Convert.ToDecimal(dt.Rows[j]["Rate"].ToString())) *
                    //                    (Convert.ToDecimal(Discount))) / 100;
                    //                Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]) - DiscountAmount_Sub;
                    //            }
                    //            else
                    //                Amount_Sub = Convert.ToDecimal(dt.Rows[j]["Rate"]);

                    //            if (Convert.ToDecimal(dt.Rows[j]["PackingPercentage"].ToString()) > 0)
                    //            {
                    //                PackageAmt_Sub = (Amount_Sub * (Convert.ToDecimal(dt.Rows[j]["PackingPercentage"].ToString()))) / 100;
                    //                Amount_Sub = Amount_Sub + PackageAmt_Sub;
                    //            }
                    //            else if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                    //            {
                    //                PackageAmt_Sub = (Amount_Sub * Convert.ToDecimal(Packing)) / 100;
                    //                Amount_Sub = Amount_Sub + PackageAmt_Sub;
                    //            }
                    //            if (Convert.ToDecimal(dt.Rows[j]["ExDutyPercentage"].ToString()) > 0)
                    //            {
                    //                ExAmt_Sub = (Amount_Sub * (Convert.ToDecimal(dt.Rows[j]["ExDutyPercentage"].ToString()))) / 100;
                    //                Amount_Sub = Amount_Sub + ExAmt_Sub;
                    //            }
                    //            else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                    //            {
                    //                ExAmt_Sub = (Amount_Sub * Convert.ToDecimal(ExDuty)) / 100;
                    //                Amount_Sub = Amount_Sub + ExAmt_Sub;
                    //            }

                    //            // This is to calculate Packing Percentage Amount_Sub
                    //            if (chkPkng != false && Convert.ToDecimal(Packing) > 0)
                    //                PackingPercentAmt = Convert.ToDouble(Packing);
                    //            else
                    //                PackingPercentAmt = 0;

                    //            // This is to calculate Discount Percentage Amount_Sub
                    //            if (chkDsnt != false && Convert.ToDecimal(Discount) > 0)
                    //                DiscountPercentAmt = Convert.ToDouble(Discount);
                    //            else
                    //                DiscountPercentAmt = 0;

                    //            // This is to calculate Ex-Duty Percentage Amount_Sub
                    //            if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0)
                    //            {
                    //                ChkExduty = true;
                    //                ChkExdutyAmt = Convert.ToDouble(ExDuty);
                    //            }
                    //            else
                    //            {
                    //                ChkExduty = false;
                    //                ChkExdutyAmt = 0;
                    //            }

                    //            dt.Rows[j]["TotalAmt"] = (Amount_Sub *
                    //                Convert.ToDecimal(dt.Rows[j]["Quantity"].ToString()));
                    //        }
                    //    }
                    //}
                    //dt.AcceptChanges();
                    //Session["AllSubItems"] = dt;
                    #endregion

                    return Fill_SubItemDetails(dt, LPOID, LPONo, ParentItemId, TRid, Convert.ToBoolean(IsAdd));
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillItemDetails(null, "");
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string ChkBoxChecked(string RNo, bool Action)
        {
            DataTable dt = (DataTable)Session["CTOneItems"];
            try
            {
                int RowNo = (Convert.ToInt32(RNo) - 1);
                dt.Rows[RowNo]["Check"] = Action;
                dt.AcceptChanges();
                Session["CTOneItems"] = dt;
                string ParentItmID = dt.Rows[RowNo]["ItemId"].ToString();

                DataTable Subdt = (DataTable)Session["AllSubItems"];
                if (Subdt != null && Subdt.Rows.Count > 0)
                {
                    if (!Subdt.Columns.Contains("check"))
                    {
                        DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                        DCCheck.DefaultValue = false;
                        Subdt.Columns.Add(DCCheck);
                    }
                    DataRow[] result = Subdt.Select("ParentItemId = " + ParentItmID + "");
                    foreach (var row in result)
                        row["check"] = Action;

                    Subdt.AcceptChanges();
                    Session["AllSubItems"] = Subdt;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
            return FillItemDetails(dt, "");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string CheckHeader(bool ChkStat)
        {
            DataTable dt = (DataTable)Session["CTOneItems"];
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["Check"] = ChkStat;
                ChkAllStatus = ChkStat;
                dt.AcceptChanges();
                Session["CTOneItems"] = dt;

                DataTable Subdt = (DataTable)Session["AllSubItems"];
                if (Subdt != null && Subdt.Rows.Count > 0)
                {
                    if (!Subdt.Columns.Contains("check"))
                    {
                        DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                        DCCheck.DefaultValue = false;
                        Subdt.Columns.Add(DCCheck);
                    }
                    if (!dt.Columns.Contains("TotalAmt"))
                    {
                        DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                        DCTotalAmt.DefaultValue = 0;
                        dt.Columns.Add(DCTotalAmt);
                    }
                    for (int i = 0; i < Subdt.Rows.Count; i++)
                        Subdt.Rows[i]["Check"] = ChkStat;

                    Subdt.AcceptChanges();
                    Session["AllSubItems"] = Subdt;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
            return FillItemDetails(dt, "");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public void ItemIDSelect(string RNo, string ItemID, string Spec)
        {
            try
            {
                ArrayList al = new ArrayList();
                al.Add(Convert.ToInt32(RNo) - 1);
                al.Add(ItemID);
                al.Add(Spec.Replace("#@%", "~~"));
                Session["HSCodeID"] = al;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to get CT-1 Internal Ref.No
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetCTOneInternalRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo('V', RefNo, Guid.Empty);
        }

        /// <summary>
        /// This is used to get CT-1 Ref.No
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetCTOneRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo('A', RefNo, Guid.Empty);
        }

        /// <summary>
        /// This is used to check ECC-NO
        /// </summary>
        /// <param name="RefNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool CheckECC(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagKSelect, RefNo, new Guid(Session["CompanyID"].ToString()));// Check ECC-No
        }

        /// <summary>
        /// File Upload medhtd
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        /// <summary>
        /// Attachment Delete Method
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["CtoneUplds"];
                all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetItemDesc_Spec(string ItmID)
        {
            string RtnVal = "";
            try
            {
                DataSet ds = imbll.SelectItemMaster(CommonBLL.FlagASelect, new Guid(ItmID), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string itmDesc = ds.Tables[0].Rows[0]["ItemDescription"].ToString();
                    string spec = (ds.Tables[0].Rows[0]["Specification"].ToString().Trim()) != "" ? (" Spec : " + ds.Tables[0].Rows[0]["Specification"].ToString()) : "";
                    RtnVal = itmDesc + spec;
                }
                return RtnVal;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Expand_LPOs(string LPOID, string ParentItemId, string TRid)
        {
            try
            {
                string LPONo = "";
                DataTable dtt = CommonBLL.CT1ItemsTable_SubItems();
                DataTable dt = (DataTable)Session["AllSubItems"];
                if (dt == null)
                    dt = CommonBLL.CT1ItemsTable_SubItems();
                if (!dt.Columns.Contains("Check"))
                {
                    DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                    DCCheck.DefaultValue = false;
                    dt.Columns.Add(DCCheck);
                }
                if (!dt.Columns.Contains("TotalAmt"))
                {
                    DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                    DCTotalAmt.DefaultValue = 0;
                    dt.Columns.Add(DCTotalAmt);
                }
                dt.AcceptChanges();
                Session["AllSubItems"] = dt;
                Calculations_Sub((DataTable)Session["CTOneItems"], TRid);
                return Fill_SubItemDetails(dtt, LPOID, LPONo, ParentItemId, TRid, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Add_Sub_Itms(string RowID, string TRid, string SubRowID, string LPOID, string ParentItemId, string ItemId, string FESNo, string LPONo, string ItmDetailsID,
            string ItmDesc, string HSCode, string Qty, string Rate, string Disc, string Pkng, string exduty, string UnitID,
            string DiscPercent, string PkngPercent, string ExDutyPercent, string IsAdd)
        {
            try
            {
                DataTable dt = (DataTable)Session["AllSubItems"];
                //Commented on 12/02/2016 sub items //if (dt != null && ItemId.Trim() != "")
                if (dt != null && ItmDesc.Trim() != "")
                {
                    //ItemId = Guid.NewGuid();
                    //decimal DscntAll_C = Convert.ToDecimal(DscntAll), PkngAll_C = Convert.ToDecimal(PkngAll),
                    //    ExseAll_C = Convert.ToDecimal(ExseAll), AddChrgsAll_C = Convert.ToDecimal(AddChrgsAll);

                    //Dictionary<Guid, Guid> Codes = (Dictionary<Guid, Guid>)Session["SelectedItems"];
                    //if (Codes == null)
                    //    Codes = new Dictionary<Guid, Guid>();
                    //Guid SelItemID = new Guid(ItemId);
                    //if (Codes == null || !Codes.Keys.Contains(SelItemID))
                    //{
                    //    Codes.Add(SelItemID, SelItemID);
                    //    Session["SelectedItems"] = Codes;
                    //}

                    DataRow[] result = dt.Select("SNo = " + SubRowID + "");
                    if (result.Length > 0)
                    {
                        if (FESNo.Trim() != "")
                            result[0]["FESNO"] = Convert.ToInt64(FESNo);
                        if (ItmDetailsID.Trim() != "")
                            result[0]["ItemDetailsId"] = ItmDetailsID;
                        if (ParentItemId.Trim() != "" && ParentItemId.Trim() != "0")
                            result[0]["ParentItemId"] = ParentItemId;
                        //if (ItemId.Trim() != "" && ItemId.Trim() != "0")
                        result[0]["ItemId"] = Guid.NewGuid();
                        //result[0]["LPONo"] = result[0]["LPONo"];
                        result[0]["SpecDes"] = ItmDesc.Trim();
                        result[0]["HSCode"] = HSCode.Trim();
                        if (Qty.Trim() != "")
                            result[0]["Quantity"] = Convert.ToDecimal(Qty);
                        if (UnitID.Trim() != "" && UnitID.Trim() != "0")
                            result[0]["UnumsID"] = UnitID;
                        if (Rate.Trim() != "" && Rate.Trim() != "0")
                            result[0]["Rate"] = Convert.ToDecimal(Rate);
                        if (result[0]["Quantity"].ToString() != "" && result[0]["Rate"].ToString() != "")
                            result[0]["TotalAmt"] = Convert.ToDecimal(Rate) * Convert.ToDecimal(Qty);
                        if (exduty.Trim() != "")
                            result[0]["ExDutyPercentage"] = Convert.ToDecimal(exduty);
                        if (Pkng.Trim() != "")
                            result[0]["PackingPercentage"] = Convert.ToDecimal(Pkng);
                        if (Disc.Trim() != "")
                            result[0]["DiscountPercentage"] = Convert.ToDecimal(Disc);

                        //if (Discount > 0)
                        //    AmtAfterDiscount = Rate_C - ((Rate_C * Discount) / 100);
                        //else if (DscntAll_C > 0)
                        //    AmtAfterDiscount = Rate_C - ((Rate_C * DscntAll_C) / 100);
                        //else
                        //    AmtAfterDiscount = Rate_C;

                        //if (PkngAll_C > 0)
                        //    PackageAmt = (AmtAfterDiscount * PkngAll_C) / 100;

                        //if (AddChrgsAll_C > 0)
                        //    AdsnlChargesAmt = (AmtAfterDiscount * AddChrgsAll_C) / 100;

                        //AmtPcknAdsn = AmtAfterDiscount + PackageAmt + AdsnlChargesAmt;

                        //if (ExseAll_C > 0)
                        //    AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExseAll_C) / 100);
                        //else if (ExseAll_C > 0)
                        //    AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExseAll_C) / 100);


                        //result[0]["QPrice"] = AmtPcknAdsn;
                        //result[0]["TotalAmt"] = (AmtPcknAdsn * Convert.ToDecimal(result[0]["Quantity"].ToString()));
                        //result[0]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(result[0]["Rate"].ToString()) *
                        //    Convert.ToDecimal(result[0]["Quantity"].ToString()), 2));

                        result[0]["LPOID"] = LPOID;
                        if (Convert.ToBoolean(IsAdd))
                        {
                            DataRow dr = dt.NewRow();
                            dr["SNo"] = (Convert.ToDouble(SubRowID) + 0.1);
                            dr["LPOID"] = LPOID;
                            dr["Quantity"] = 0;
                            dr["Rate"] = 0;
                            dr["ExDutyPercentage"] = 0;
                            dr["PackingPercentage"] = 0;
                            dr["DiscountPercentage"] = 0;
                            dr["ParentItemId"] = ParentItemId;
                            dr["Check"] = true;
                            dt.Rows.Add(dr);
                        }
                    }
                    dt.AcceptChanges();
                    Session["AllSubItems"] = dt;
                }
                else
                    dt = CommonBLL.CT1ItemsTable_SubItems();
                Calculations_Sub((DataTable)Session["CTOneItems"], TRid);
                return Fill_SubItemDetails(dt, LPOID, LPONo, ParentItemId, TRid, Convert.ToBoolean(IsAdd));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Delete_SubItem(string LPOID, string ParentItemId, string TRid, string SNo, string ItemID)
        {
            try
            {
                string subItmID = TRid + "." + SNo;
                //Dictionary<Guid, Guid> Codes = (Dictionary<Guid, Guid>)Session["SelectedItems"];
                //if (Codes == null)
                //    Codes = new Dictionary<Guid, Guid>();

                //if (ItemID != "" && Codes != null && Codes.Keys.Contains(new Guid(ItemID)))
                //{
                //    Codes.Remove(new Guid(ItemID));
                //    Session["SelectedItems"] = Codes;
                //}

                DataTable dt = (DataTable)Session["AllSubItems"];
                DataRow[] result = dt.Select("SNo = " + subItmID + "");
                if (result.Length > 0)
                    result[0].Delete();

                dt.AcceptChanges();
                Session["AllSubItems"] = dt;
                return Fill_SubItemDetails(dt, LPOID, "", ParentItemId, TRid, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Validate_SubItem_BeforeSaving()
        {
            string ErrorMsg = ""; string FinalMsg = ""; string RowID = "";
            try
            {
                DataTable dt = (DataTable)Session["AllSubItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RowID = dt.Rows[i]["SNo"].ToString();
                        if (dt.Rows[i]["ItemId"].ToString() != "")
                        {
                            if (dt.Rows[i]["ItemId"].ToString() == "")
                                ErrorMsg += "ItemID";
                            if (dt.Rows[i]["HSCode"].ToString() == "")
                                ErrorMsg += "," + "HSCode";
                            if (Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) <= 0)
                                ErrorMsg += "," + "Quantity";
                            if (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) < 0)
                                ErrorMsg += "," + "ExDutyPercentage";
                            if (Convert.ToDecimal(dt.Rows[i]["PackingPercentage"].ToString()) < 0)
                                ErrorMsg += "," + "PackingPercentage";
                            if (Convert.ToDecimal(dt.Rows[i]["DiscountPercentage"].ToString()) < 0)
                                ErrorMsg += "," + "DiscountPercentage";
                            if (dt.Rows[i]["UnumsID"].ToString() == "")
                                ErrorMsg += "," + "Units";
                            if (ErrorMsg != "")
                                break;
                        }
                    }
                }
                if (RowID != "" && ErrorMsg != "")
                {
                    FinalMsg = "Please correct the following " + ErrorMsg.Trim(',') + " Details in Row No : " + RowID.Split('.')[0] + " Sub Item : " + RowID.Split('.')[1];
                }
                return FinalMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        # endregion
    }
}