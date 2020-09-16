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
using System.Globalization;
using System.Collections.Generic;
using Ajax;
using System.IO;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Customer_Access
{
    public partial class VerbalLPO_Customer : System.Web.UI.Page
    {
        # region variables

        /// <summary>
        /// Global Variables in the Page
        /// </summary>

        int res = -999;
        LPOrdersBLL NLPOBL = new LPOrdersBLL();
        NewFPOBLL NFPOBL = new NewFPOBLL();
        NewFQuotationBLL NFQBL = new NewFQuotationBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBL = new NewEnquiryBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        CustomerBLL cusmr = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        CheckBLL CBL = new CheckBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        ErrorLog ELog = new ErrorLog();
        decimal RunningTotal = 0, GTotal = 0;
        DataSet ComparisonDTS = new DataSet();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        IOMTemplateBLL IOMTBLL = new IOMTemplateBLL();
        LEnquiryBLL LEQBL = new LEnquiryBLL();
        SupplierBLL sbll = new SupplierBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        int FlagDiscount = 0;
        int FlagExDuty = 0;
        string fpono = "";
        static string GeneralCtgryID;
        #endregion

        #region Dafault Page Load Event

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
                        txtLpono.Attributes.Add("readonly", "readonly");
                        Ajax.Utility.RegisterTypeForAjax(typeof(VerbalLPO_Customer));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            HideUnwantedFields();
                            GetData();

                            Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            //if ((string[])Session["UsrPermissions"] != null &&
                            //    ((string[])Session["UsrPermissions"]).Contains("Edit") && Request.QueryString["ID"] != null)
                            //{
                            if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                            {
                                DivComments.Visible = true;
                                EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                            }
                            //}
                            //else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            //{
                            //    btnSave.Text = "Save";
                            //}
                            //else
                            //    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                        }
                        else
                            if (Session["PaymentTermsLPO"] != null)
                                divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods (Data Binding)
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
        /// This is used to convert DS to ArrayList
        /// </summary>
        /// <returns></returns>
        private ArrayList DT2Array()
        {
            ArrayList al = new ArrayList();
            try
            {
                DataSet ds = new DataSet();
                ////Guid FPOID = new Guid(ListBoxFPO.SelectedValue);
                ////string FPOIDs = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Guid LPOID = Guid.Empty;
                if (Request.QueryString["ID"] != null)
                    LPOID = new Guid(Request.QueryString["ID"]);
                ItemStatusBLL ISBLL = new ItemStatusBLL();
                ////ds = ISBLL.GetItemStatus(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, LPOID, CommonBLL.StatusTypeFPOrder, "", "", "", "", FPOIDs, "");
                ds = ISBLL.GetItemStatus(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, LPOID, CommonBLL.StatusTypeFPOrder, "", "", "", "", "", "");
                foreach (DataRow dtrow in ds.Tables[0].Rows)
                    al.Add(dtrow[0]);
                ArrayList al1 = new ArrayList();
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[1].Rows)
                        al1.Add(dtrow[0]);
                    ViewState["LPORelease"] = al1;
                }
                else
                {
                    al1.Add(0);
                    ViewState["LPORelease"] = al1;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                al.Clear();
                al.Add(-1);
            }
            return al;
        }

        /// <summary>
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                ClearAll();
                divListBox.InnerHtml = AttachedFiles();
                txtLpoDt.Attributes.Add("readonly", "readonly");
                txtLpoDueDt.Attributes.Add("readonly", "readonly");
                GetGeneralID();

                BindDropDownList(ddlCustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (Request.QueryString["ID"] != null && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    ////LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(Request.QueryString["CustID"].ToString()),
                    ////    Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "",
                    ////    Guid.Empty, "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                    ////    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    ////    CommonBLL.ATConditions()));
                    ddlCustomer.Enabled = false;
                }
                else if (Request.QueryString["FpoID"] != null && Request.QueryString["FpoID"].ToString() != "" && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    string fpono = Request.QueryString["FpoID"];
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                    ////LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagISelect, new Guid(fpono), "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    ////    Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    ////    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                    ////    new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    ////    CommonBLL.ATConditions()));
                    ////string[] FPOLength = Request.QueryString["FpoID"].Split(',');
                    ////for (int i = 0; i < FPOLength.Length; i++)
                    ////{
                    ////    ListItem item = ListBoxFPO.Items.FindByValue(FPOLength[i].ToString());
                    ////    if (item != null)
                    ////    {
                    ////        ListBoxFPO.SelectedValue = FPOLength[i].ToString();
                    ////    }
                    ////}
                    //LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                    //    Guid.Empty, DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                    //    100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                    //    DateTime.Now, true, CommonBLL.EmptyDtNewFPO(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                    //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    //ListBoxFEO.Enabled = false;
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    BindDropDownList(ddlSuplr, sbll.SelectSuppliersForBind(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    FillInputFields(fpono, Request.QueryString["CustID"]);
                }
                else if (ddlCustomer.Items.Count > 1)
                {
                    ddlCustomer.SelectedIndex = 1;
                    CustomerSelectionChanged(); ;
                }
                divLPOItems.InnerHtml = FillGridItems(Guid.Empty);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill the List Box
        /// </summary>
        /// <param name="cntrl"></param>
        /// <param name="ds"></param>
        private void LocalPurchaseOrder(ListBox cntrl, DataSet ds)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Supplier bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownSuppList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "OrgName";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Enabled = true;
                if (ddl.ID == "ddlsuplrctgry")
                {
                    ddl.SelectedValue = "274";
                    ddl.Enabled = false;
                }
                else
                    ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
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
                ddl.Enabled = true;
                if (ddl.ID == "ddlsuplrctgry")
                {
                    ddl.SelectedValue = ddl.Items.FindByText("General").Value;
                    ddl.Enabled = false;
                }
                else
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Drop Down Lists
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
                ddl.Enabled = true;
                if (ddl.ID == "ddlsuplrctgry")
                {
                    ddl.SelectedValue = "274";
                    ddl.Enabled = false;
                }
                else
                    ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is to get genereal ID of items
        /// </summary>
        private void GetGeneralID()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.EnumMasterSelect(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["GeneralCtgryID"] = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Edit Selected Record
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");

                bool IsExciseDuty = false;
                DataSet EditDS = new DataSet();
                EditDS = NLPOBL.GetDataSetLPO_Verbal(CommonBLL.FlagOSelect, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, "",
                                                "Subject", Guid.Empty, "", false, false, false, 0, 0, 0, Guid.Empty, "",
                                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()),
                                                CommonBLL.EmptyDtLPOrdersVerbal(), CommonBLL.FirstRowPaymentTerms(),
                                                CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                    && EditDS.Tables[2].Rows.Count > 0)
                {
                    ////string FpoNo = EditDS.Tables[0].Rows[0]["ForeignPurchaseOrderId"].ToString();
                    string CustID = EditDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlCustomer.SelectedValue = CustID;

                    BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                    BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                    ////LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagZSelect, Guid.Empty, FpoNo, new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    ////    Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    ////    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                    ////    new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    ////    CommonBLL.ATConditions()));

                    ////hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["FPODate"].ToString()));
                    ////string[] fponmbrs = FpoNo.Split(',');
                    ////foreach (ListItem item in ListBoxFPO.Items)
                    ////{
                    ////    foreach (string s in fponmbrs)
                    ////    {
                    ////        if (item.Value == s)
                    ////        {
                    ////            item.Selected = true;
                    ////        }
                    ////    }
                    ////}
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    BindDropDownList(ddlSuplr, sbll.SelectSuppliersForBind(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ddlSuplr.SelectedValue = EditDS.Tables[0].Rows[0]["SupplierID"].ToString();

                    if (EditDS.Tables.Count > 3)
                    {
                        ViewState["Quantity"] = EditDS.Tables[3];
                    }
                    ////string FPOSession = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    ////Session["FPOSelected"] = FPOSession;
                    ////FillInputFields(FpoNo, CustID);
                    FillInputFields("", CustID);

                    //DataSet ds = new DataSet();
                    //ds.Tables.Add(EditDS.Tables[1].Copy());
                    //DataColumn dc = new DataColumn("Check", typeof(bool));
                    //dc.DefaultValue = true;
                    //ds.Tables[0].Columns.Add(dc);
                    Session["VerbalLPOCust"] = EditDS.Tables[1];
                    Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                    for (int i = 0; i < EditDS.Tables[1].Rows.Count; i++)
                    {
                        Codes.Add((i + 1), new Guid(EditDS.Tables[1].Rows[i]["ItemId"].ToString()));
                    }
                    HttpContext.Current.Session["SelectedItems"] = Codes;

                    //divLPOItems.InnerHtml = FillGridView(ds, 0, true);
                    divLPOItems.InnerHtml = FillItemGrid(false);

                    Session["TCs"] = (CBL.SelectATConditions(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, ID, 0, Guid.Empty, "",
                                      new Guid(Session["UserID"].ToString()))).Tables[0];
                    ddlsuplrctgry.SelectedValue = EditDS.Tables[0].Rows[0]["SuplierCategoryId"].ToString();
                    ddlRsdby.SelectedValue = EditDS.Tables[0].Rows[0]["FPORaisedByDept"].ToString();
                    ddlPrcBsis.SelectedValue = EditDS.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = EditDS.Tables[0].Rows[0]["PriceBasisText"].ToString();
                    txtsubject.Text = EditDS.Tables[0].Rows[0]["Subject"].ToString();
                    ChkbCEEApl.Checked = Convert.ToBoolean(EditDS.Tables[0].Rows[0]["IsCentralExcise"].ToString()) ? true : false;
                    ChkbInspcn.Checked = Convert.ToBoolean(EditDS.Tables[0].Rows[0]["Inspection"].ToString()) ? true : false;
                    ChkbDrwngAprls.Checked = Convert.ToBoolean(EditDS.Tables[0].Rows[0]["DrwngAprls"].ToString()) ? true : false;

                    txtCEEApl.Text = (ChkbCEEApl.Checked) ? EditDS.Tables[0].Rows[0]["CEERmdTm"].ToString() : "0";
                    txtInsptn.Text = (ChkbInspcn.Checked) ? EditDS.Tables[0].Rows[0]["InsptnRmdTm"].ToString() : "0";
                    txtDrwngAprls.Text = (ChkbDrwngAprls.Checked) ? EditDS.Tables[0].Rows[0]["DwngAprlRmdTm"].ToString() : "0";

                    if (Convert.ToBoolean(EditDS.Tables[0].Rows[0]["DrwngAprls"].ToString()))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDwngApls",
                            "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
                    }
                    if (Convert.ToBoolean(EditDS.Tables[0].Rows[0]["Inspection"].ToString()))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowInsption",
                            "CHeck('ChkbInspcn', 'dvInsptn');", true);
                    }
                    if (Convert.ToBoolean(EditDS.Tables[0].Rows[0]["IsCentralExcise"].ToString()))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls",
                            "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                    }
                    txtLpoDt.Text = Convert.ToDateTime(EditDS.Tables[0].Rows[0]["LocalPurchaseOrderDate"].ToString()).ToString("dd-MM-yyyy");
                    txtLpoDueDt.Text = Convert.ToDateTime(EditDS.Tables[0].Rows[0]["DeliveryDate"].ToString()).ToString("dd-MM-yyyy");
                    hdfldCstmr.Value = EditDS.Tables[0].Rows[0]["CustomerId"].ToString();
                    txtDlvry.Text = EditDS.Tables[0].Rows[0]["DeliveryPeriod"].ToString();
                    txtimpinst.Text = EditDS.Tables[0].Rows[0]["Instruction"].ToString();
                    txtLpono.Text = EditDS.Tables[0].Rows[0]["LocalPurchaseOrderNo"].ToString();
                    DataTable Ptb = EditDS.Tables[2].Copy();
                    Ptb = DeletePaymntColumns(Ptb);
                    Session["PaymentTermsLPO"] = Ptb;
                    Session["amountLPO"] = Ptb.Compute("Sum(PaymentPercentage)", "");
                    divPaymentTerms.InnerHtml = FillPaymentTerms();

                    if ((EditDS.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00" ||
                        EditDS.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != "0.00") &&
                        (EditDS.Tables[0].Rows[0]["ExDuty"].ToString() != "" ||
                        EditDS.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != ""))
                    {
                        if (EditDS.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00")
                        {
                            txtExdt.Text = EditDS.Tables[0].Rows[0]["ExDuty"].ToString();
                            rbtnExdt.SelectedValue = "1";
                        }
                        else if (EditDS.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != "0.00")
                        {
                            txtExdt.Text = EditDS.Tables[0].Rows[0]["ExDutyPercentage"].ToString();
                            rbtnExdt.SelectedValue = "0";
                        }
                        chkExdt.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", true);
                    }
                    else
                        txtExdt.Text = "0";

                    if ((EditDS.Tables[0].Rows[0]["SaleTax"].ToString() != "0.00" ||
                        EditDS.Tables[0].Rows[0]["SaleTaxPercentage"].ToString() != "0.00") &&
                        (EditDS.Tables[0].Rows[0]["SaleTax"].ToString() != "" ||
                        EditDS.Tables[0].Rows[0]["SaleTaxPercentage"].ToString() != ""))
                    {
                        if (EditDS.Tables[0].Rows[0]["SaleTax"].ToString() != "0.00")
                        {
                            txtSltx.Text = EditDS.Tables[0].Rows[0]["SaleTax"].ToString();
                            rbtnSltx.SelectedValue = "1";
                        }
                        else if (EditDS.Tables[0].Rows[0]["SaleTaxPercentage"].ToString() != "0.00")
                        {
                            txtSltx.Text = EditDS.Tables[0].Rows[0]["SaleTaxPercentage"].ToString();
                            rbtnSltx.SelectedValue = "0";
                        }
                        chkSltx.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkSltx", "CHeck('chkSltx', 'dvSltx');", true);
                    }
                    else
                        txtSltx.Text = "0";

                    if ((EditDS.Tables[0].Rows[0]["packing"].ToString() != "0.00" ||
                        EditDS.Tables[0].Rows[0]["packingPercentage"].ToString() != "0.00") &&
                        (EditDS.Tables[0].Rows[0]["packing"].ToString() != "" ||
                        EditDS.Tables[0].Rows[0]["packingPercentage"].ToString() != ""))
                    {
                        if (EditDS.Tables[0].Rows[0]["packing"].ToString() != "0.00")
                        {
                            txtPkng.Text = EditDS.Tables[0].Rows[0]["packing"].ToString();
                            rbtnPkng.SelectedValue = "1";
                        }
                        else if (EditDS.Tables[0].Rows[0]["packingPercentage"].ToString() != "0.00")
                        {
                            txtPkng.Text = EditDS.Tables[0].Rows[0]["packingPercentage"].ToString();
                            rbtnPkng.SelectedValue = "0";
                        }
                        chkPkng.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkPkng", "CHeck('chkPkng', 'dvPkng');", true);
                    }
                    else
                        txtPkng.Text = "0";

                    if ((EditDS.Tables[0].Rows[0]["Discount"].ToString() != "0.00" ||
                        EditDS.Tables[0].Rows[0]["DiscountPercentage"].ToString() != "0.00") &&
                        (EditDS.Tables[0].Rows[0]["Discount"].ToString() != "" ||
                        EditDS.Tables[0].Rows[0]["DiscountPercentage"].ToString() != ""))
                    {
                        if (EditDS.Tables[0].Rows[0]["Discount"].ToString() != "0.00")
                        {
                            txtDsnt.Text = EditDS.Tables[0].Rows[0]["Discount"].ToString();
                            rbtnDsnt.SelectedValue = "1";
                        }
                        else if (EditDS.Tables[0].Rows[0]["DiscountPercentage"].ToString() != "0.00")
                        {
                            txtDsnt.Text = EditDS.Tables[0].Rows[0]["DiscountPercentage"].ToString();
                            rbtnDsnt.SelectedValue = "0";
                        }
                        chkDsnt.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkDsnt", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                    }
                    else
                        txtDsnt.Text = "0";

                    if (EditDS.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        string[] all = EditDS.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
                        CBLL.ClearUploadedFiles();
                        CBLL.UploadedFilesAL(all);
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
                        Session["LPOUploads"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                        divListBox.InnerHtml = "";

                    string Test = "";
                    if (IsExciseDuty == true)
                    {
                        ChkbCEEApl.Enabled = false;
                        txtCEEApl.Enabled = false;
                    }
                    else
                    {
                        ChkbCEEApl.Enabled = true;
                        txtCEEApl.Enabled = true;
                    }

                    ddlSuplr.Enabled = false;
                    ViewState["EditID"] = ID;
                    if (Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True")
                        btnSave.Text = "Save";
                    else
                        btnSave.Text = "Update";
                }
                else
                {
                    ////ListBoxFPO.Enabled = false;
                    ddlSuplr.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to delete payment Columns
        /// </summary>
        /// <param name="dtt"></param>
        /// <returns></returns>
        private DataTable DeletePaymntColumns(DataTable dtt)
        {
            try
            {
                if (dtt.Columns.Contains("PaymentSerialNo"))
                    dtt.Columns["PaymentSerialNo"].ColumnName = "SNo";
                if (dtt.Columns.Contains("Percentage"))
                    dtt.Columns["Percentage"].ColumnName = "PaymentPercentage";
                if (dtt.Columns.Contains("Against"))
                    dtt.Columns["Against"].ColumnName = "Description";
                if (dtt.Columns.Contains("PaymentTermsId"))
                    dtt.Columns.Remove("PaymentTermsId");
                if (dtt.Columns.Contains("FQuotationId"))
                    dtt.Columns.Remove("FQuotationId");
                if (dtt.Columns.Contains("FPurchaseOrderId"))
                    dtt.Columns.Remove("FPurchaseOrderId");
                if (dtt.Columns.Contains("LQuotationId"))
                    dtt.Columns.Remove("LQuotationId");
                if (dtt.Columns.Contains("LPurchaseOrderId"))
                    dtt.Columns.Remove("LPurchaseOrderId");
                if (dtt.Columns.Contains("CreatedBy"))
                    dtt.Columns.Remove("CreatedBy");
                if (dtt.Columns.Contains("CreatedDate"))
                    dtt.Columns.Remove("CreatedDate");
                if (dtt.Columns.Contains("ModifiedBy"))
                    dtt.Columns.Remove("ModifiedBy");
                if (dtt.Columns.Contains("ModifiedDate"))
                    dtt.Columns.Remove("ModifiedDate");
                if (dtt.Columns.Contains("IsActive"))
                    dtt.Columns.Remove("IsActive");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
            return dtt;
        }

        /// <summary>
        /// To fill input fields in the form
        /// </summary>
        /// <param name="FpoID"></param>
        private void FillInputFields(string FpoID, string CustID)
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                DataSet FPOrderDeatils = NFPOBL.SelectItemSet(CommonBLL.FlagOSelect, Guid.Empty, FpoID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                    Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0,
                    0, 80, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions());

                if (FPOrderDeatils.Tables[0].Rows.Count > 0)
                {
                    txtsubject.Text = FPOrderDeatils.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = FPOrderDeatils.Tables[0].Rows[0]["Instruction"].ToString();
                    txtDlvry.Text = FPOrderDeatils.Tables[0].Rows[0]["DeliveryPeriod"].ToString();
                    ddlRsdby.SelectedValue = FPOrderDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    ddlPrcBsis.SelectedValue = FPOrderDeatils.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = FPOrderDeatils.Tables[0].Rows[0]["PriceBasisText"].ToString();
                    hdfldCstmr.Value = FPOrderDeatils.Tables[0].Rows[0]["CusmorId"].ToString();
                    hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(FPOrderDeatils.Tables[0].Rows[0]["FPODate"].ToString()));
                    txtLpoDueDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    ////string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    DataSet CrntIdnt = NLPOBL.SelectLPOrders(CommonBLL.FlagISelect, Guid.Empty, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                    DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                    CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                    ////txtLpono.Text = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" +
                    ////ListBoxFPO.SelectedItem.Text.Trim() + "/" + CommonBLL.FinacialYearShort;

                    txtLpono.Text = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/"
                        + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" + CommonBLL.FinacialYearShort;

                    //Dinesh Check
                    //string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    Guid CompanyID = new Guid(HttpContext.Current.Session["CompanyID"].ToString());
                    DataSet ds = IDBLL.SelectItemDtlsLPOVerbal(CommonBLL.FlagHSelect, fpono, CompanyID, new Guid(ddlCustomer.SelectedValue));
                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    ds.Tables[0].Columns.Add(dc);
                    if (ds.Tables[0].Columns.Contains("Rate"))
                        ds.Tables[0].Columns.Remove("Rate");
                    DataColumn dc1 = new DataColumn("Rate", typeof(decimal));
                    dc1.DefaultValue = 0;
                    ds.Tables[0].Columns.Add(dc1);
                    ds.Tables[0].AcceptChanges();
                    Session["VerbalLPOCust"] = ds;
                    divLPOItems.InnerHtml = FillItemGrid(false);
                }
                if (FPOrderDeatils.Tables[1].Rows.Count > 0)
                {
                    if (Request.QueryString["IsAm"] == "")
                    {
                        ComparisonDTS.Tables.Add(FPOrderDeatils.Tables[1].Copy());
                        ViewState["ComparisonDTS"] = ComparisonDTS;
                    }

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to fill payment details
        /// </summary>
        /// <returns></returns>
        public string FillPaymentTerms()
        {
            try
            {
                DataTable dt = (DataTable)Session["PaymentTermsLPO"];
                int TotalAmt = 0;
                string Message = "";
                if (Session["amountLPO"] != null)
                    TotalAmt = Convert.ToInt32(Session["amountLPO"]);
                else
                    TotalAmt = Convert.ToInt32(dt.Compute("Sum(PaymentPercentage)", ""));
                if (Session["MessageLPO"] != null)
                    Message = Session["MessageLPO"].ToString();

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblPaymentTerms' " +
                    "align='center'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Payment(%)</th><th>Description</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");
                        if (dt.Rows[i]["PaymentPercentage"].ToString() != "")
                            sb.Append("<td><input type='text' name='txtPercAmt' class='bcAsptextbox' value='" +
                                Convert.ToInt64(Convert.ToDouble(dt.Rows[i]["PaymentPercentage"].ToString())).ToString() + "' id='txtPercAmt" + SNo + "' "
                                + " onchange='getPaymentValues(" + SNo + ")' maxlength='3' style='text-align: right; width: 50px;' "
                                + " onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");
                        else
                            sb.Append("<td><input type='text' name='txtPercAmt' class='bcAsptextbox' value='" + dt.Rows[i]["PaymentPercentage"].ToString() + "' "
                                + " id='txtPercAmt" + SNo + "' onkeypress='return isNumberKey(event)' onchange='getPaymentValues(" + SNo + ")' maxlength='3' "
                                + "style='text-align: right; width: 50px;' onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");
                        sb.Append("<td><input type='text' name='txtDesc' class='bcAsptextbox' value='" +
                            dt.Rows[i]["Description"].ToString() + "'  id='txtDesc" + SNo +
                            "' onchange='getPaymentValues(" + SNo + ")'/></td>");
                        if (TotalAmt == 100)
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirmPayment(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");
                        else if (dt.Rows.Count == 1)
                            sb.Append("<td><a href='javascript:void(0)' onclick='getPaymentValues(" + SNo +
                                ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else if (dt.Rows.Count == (i + 1))
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirmPayment(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a>&nbsp;&nbsp;<a href='javascript:void(0)' " +
                                " onclick='getPaymentValues(" + SNo +
                                ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirmPayment(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='3' class='rounded-foot-right'> Total Percent : <b>"
                    + TotalAmt + "</b>%<input id='HfMessage' type='hidden' name='HfMessage' value='" + Message + "'/></th></tfoot>");
                }
                sb.Append("</tbody></table>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Fill Local Quotation Payment Terms
        /// </summary>
        /// <param name="QuotationTerms"></param>
        protected void FillQuotationTerms(DataTable QuotationTerms)
        {
            try
            {
                DataTable Dtble = CommonBLL.FirstRowPaymentTerms();
                Dtble.Rows.RemoveAt(0);
                DataRow dtrw = Dtble.NewRow();
                foreach (DataRow drw in QuotationTerms.Rows)
                {
                    dtrw = Dtble.NewRow();
                    dtrw["Sno"] = drw["PaymentSerialNo"];
                    dtrw["PaymentPercentage"] = drw["Percentage"];
                    dtrw["Description"] = drw["Against"];
                    Dtble.Rows.Add(dtrw);
                }
                Session["PaymentTermsLPO"] = Dtble;
                divPaymentTerms.InnerHtml = FillPaymentTerms();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="gvItems"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView gvItems)
        {
            DataTable dt = CommonBLL.EmptyDtLPOrders();
            dt.Rows[0].Delete(); int exdt = 0, dscnt = 0;
            foreach (GridViewRow row in gvItems.Rows)
            {
                DataRow dr;
                if (((CheckBox)row.FindControl("ItmChkbx")).Checked)
                {
                    dr = dt.NewRow();
                    dr["ItemDetailsID"] = new Guid(((Label)row.FindControl("lblItemDetailsID")).Text);
                    dr["SNo"] = Convert.ToInt32(((HiddenField)row.FindControl("hfFESNo")).Value);
                    dr["ItemId"] = new Guid(((Label)row.FindControl("lblItemID")).Text);
                    dr["LPOrderId"] = (((Label)row.FindControl("lblLPOrderId")).Text == "0" ? Guid.Empty : new Guid(((Label)row.FindControl("lblLPOrderId")).Text));
                    dr["PartNumber"] = ((Label)row.FindControl("lblPrtNo")).Text;
                    dr["Specifications"] = ((Label)row.FindControl("lblSpec")).Text;
                    dr["Make"] = ((Label)row.FindControl("lblMake")).Text;
                    dr["Quantity"] = (String.IsNullOrWhiteSpace(((TextBox)row.FindControl("txtQty")).Text) ? 0 : Convert.ToDecimal(((TextBox)row.FindControl("txtQty")).Text));     //Convert.ToDecimal(((Label)row.FindControl("lblQty")).Text);
                    dr["Rate"] = Convert.ToDecimal(((HiddenField)row.FindControl("HF_CngPrice")).Value);
                    dr["Amount"] = Convert.ToDecimal(((Label)row.FindControl("lblAmount")).Text);
                    if (row.FindControl("lblExdtPrcnt").Controls.Count > 0)
                    {
                        if (((Label)row.FindControl("lblExdtPrcnt")).Text == "")
                            dr["ExDutyPercentage"] = 0;
                        else
                            dr["ExDutyPercentage"] = Convert.ToDecimal(((Label)row.FindControl("lblExdtPrcnt")).Text);
                    }
                    else
                        dr["ExDutyPercentage"] = 0;
                    if (((Label)row.FindControl("lblDscntPrcnt")).Text == "")
                        dr["DiscountPercentage"] = 0;
                    else
                        dr["DiscountPercentage"] = Convert.ToDecimal(((Label)row.FindControl("lblDscntPrcnt")).Text);
                    dr["UNumsId"] = new Guid(((Label)row.FindControl("lblUnitID")).Text);
                    dr["Remarks"] = ((Label)row.FindControl("lblRmrks")).Text;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private void HideUnwantedFields()
        {
            try
            {
                FieldAccessBLL FAB = new FieldAccessBLL();
                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), System.IO.Path.GetFileName(Request.Path));
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                    {
                        Session["HideFields"] = HideFields.Tables[0];
                    }
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.TotalExciseDutyText)))
                    {
                        lblED.Visible = chkExdt.Visible = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", false);
                    }
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.ExciseDutyExcemptionText)))
                    {
                        ECEA_spn_txt.Visible = ChkbCEEApl.Visible = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearAll()
        {
            try
            {
                Session["FPOSelected"] = null;
                btnSave.Text = "Save";
                ////ListBoxFPO.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlSuplr.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                txtDlvry.Text = txtimpinst.Text = txtLpoDt.Text = txtLpoDueDt.Text = txtLpono.Text = txtsubject.Text = "";
                ChkbInspcn.Checked = ChkbCEEApl.Checked = false;
                txtDsnt.Text = txtExdt.Text = txtPkng.Text = txtSltx.Text = "0";
                txtLpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(3));
                ////ListBoxFPO.Enabled = true;
                ddlSuplr.Enabled = true;
                ViewState["EditID"] = null;
                divLPOItems.InnerHtml = "";
                Session["MessageLPO"] = null;
                Session["amountLPO"] = null;
                Session["PaymentTermsLPO"] = null;
                Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                Session["LPOUploads"] = null;
                ////ListBoxFPO.Items.Clear();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        private string AttachedFiles()
        {
            try
            {
                if (Session["LPOUploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["LPOUploads"];
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
                        if (Session["LPOUploads"] != null)
                        {
                            alist = (ArrayList)Session["LPOUploads"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["LPOUploads"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["LPOUploads"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods Items Not In Use

        #region Not In Use
        //private string FillGridView(DataSet ds, decimal Gtotl, Boolean RateSve)
        //{
        //    try
        //    {

        //        //string FOP = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
        //        //DataSet ds = new DataSet();
        //        //if (ds == null)
        //        ds = (DataSet)Session["VerbalLPOCust"];
        //        string ItemIDs = string.Empty;
        //        FlagDiscount = 0;
        //        FlagExDuty = 0;
        //        if (RateSve == false)
        //        {
        //            string fpno = "";
        //            if (Session["FPOSelected"] != null)
        //                fpno = Session["FPOSelected"].ToString();
        //            else
        //                fpno = ds.Tables[0].Rows[0]["ForeignPOID"].ToString();

        //            DataSet LclQuoteItems = NLQBL.LPOItemsByMulti(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty.ToString(), Guid.Empty, Guid.Empty,
        //                Guid.Empty, Guid.Empty, new Guid(ddlSuplr.SelectedValue), "", 0, "", Guid.Empty, CommonBLL.EmptyDtLocalQuotation(),
        //                CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), Guid.Empty, fpno, new Guid(Session["CompanyID"].ToString()));

        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (LclQuoteItems.Tables[4].Rows[i]["CQuantity"].ToString() != ""
        //                        && ds.Tables[0].Rows[i]["ItemId"].ToString() == LclQuoteItems.Tables[4].Rows[i]["ItemId"].ToString()
        //                        && Convert.ToDecimal(LclQuoteItems.Tables[4].Rows[i]["Quantity"].ToString()) >= Convert.ToDecimal(LclQuoteItems.Tables[4].Rows[i]["CQuantity"].ToString()))
        //                    {
        //                        ds.Tables[0].Rows[i]["Quantity"] = Convert.ToInt16(ds.Tables[0].Rows[i]["Quantity"]) -
        //                            Convert.ToInt16(LclQuoteItems.Tables[4].Rows[i]["CQuantity"]);

        //                    }
        //                    if (ds.Tables[0].Rows[i]["Quantity"].ToString() == "0")
        //                        ds.Tables[0].Rows[i]["Check"] = false;
        //                }

        //            }
        //        }
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            DataSet dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
        //            DataSet dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

        //            StringBuilder sb = new StringBuilder();
        //            sb.Append("");
        //            sb.Append("<table width='98%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
        //            " id='tblItems'><thead align='left'><tr>");
        //            if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
        //                sb.Append("<th class='rounded-First'>&nbsp;</th><th></th><th align='center'>SNo</th><th align='center'>Item Description</th>" +
        //                    "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
        //                    "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
        //                    "<th align='center'>Amount</th>" +
        //                    "<th align='center'>Discount</th><th align='center'>Net Rate</th>"
        //                    + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
        //            else
        //                sb.Append("<th class='rounded-First'>&nbsp;</th><th></th><th align='center'>SNo</th><th align='center'>Item Description</th>" +
        //                "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
        //                "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
        //                "<th align='center'>Amount</th>" +
        //                "<th align='center'>Discount</th><th align='center'>Ex Duty</th><th align='center'>Net Rate</th>"
        //                + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
        //            sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

        //            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                decimal TotalAmount = 0;
        //                decimal GrandTotal = 0;
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    string sno = (i + 1).ToString();
        //                    sb.Append("<tr valign='Top'>");
        //                    if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
        //                    {
        //                        TotalAmount += Convert.ToDecimal(ds.Tables[0].Rows[i]["AmountC"].ToString());
        //                        Gtotl += Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString());
        //                        sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
        //                                + sno + ")' type='checkbox' checked='checked' name='CheckAll'/></td>");
        //                    }
        //                    else
        //                        sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
        //                            + sno + ")' type='checkbox' name='CheckAll' /></td>");
        //                    sb.Append("<td></td>");

        //                    sb.Append("<td align='center'>" + (i + 1).ToString());
        //                    sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + (i + 1).ToString() +
        //                        ")' id='HItmID" + (i + 1).ToString() + "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString()
        //                        + "' width='5px' style='WIDTH: 5px;' class='bcAsptextbox'/></td>");

        //                    sb.Append("<td valign='Top' width='200px'><div class='expanderR'><span id='lbldescip" + sno + "'>" +
        //                        ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</span></div></td>");

        //                    sb.Append("<td><span id='lblpartno" + sno + "'>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</span></td>");

        //                    sb.Append("<td><textarea name='txtSpecifications' id='txtSpecifications" + sno
        //                                + "' onchange='FillItemsAll(" + sno + ")' Class='bcAsptextboxmulti' onfocus='ExpandTXT(" + sno
        //                                + ")' onblur='ReSizeTXT(" + sno + ")' style='height:22px; width:150px; resize:none;'>"
        //                                + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
        //                    sb.Append("<td><input type='text' name='txtMake' size='05px' onchange='FillItemsAll(" + sno
        //                        + ")' id='txtMake" + sno + "' value='"
        //                        + ds.Tables[0].Rows[i]["Make"].ToString() + "' style='width:50px;' class='bcAsptextbox'/></td>");
        //                    sb.Append("<td><input type='text' name='txtQuantity'  size='05px' onchange='FillItemsAll("
        //                    + sno + ")' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString()
        //                        + "' onkeypress='return blockNonNumbers(this, event, false, false);' " +
        //                        " maxlength='6' style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");

        //                    sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");

        //                    sb.Append("<td><input type='text' size='05px' name='txtPrice' id='txtPrice" + sno
        //                    + "' onfocus='this.select()' onMouseUp='return false' value='"
        //                        + ds.Tables[0].Rows[i]["Rate"].ToString() + "' onchange='CalculateTotalAmount(" + sno
        //                        + ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' " +
        //                        " onkeypress='return blockNonNumbers(this, event, true, false);' " +
        //                        " style='text-align: right; width:50px;' class='bcAsptextbox'/> <input type='hidden'  name='HFStatus' id='HFStatus" + sno
        //                   + "' value='" + ds.Tables[0].Rows[i]["Check"].ToString() + "'style='text-align: right; width:50px;'/> </td>");

        //                    //sb.Append("<td></td>");

        //                    sb.Append("<td align='right'><span id='spnAmount" + sno + "'>"
        //                    + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["AmountC"].ToString()), 2) + "</span></td>");
        //                    sb.Append("<td><input type='text' size='05px' name='txtDiscount' id='txtDiscount" + sno
        //                        + "' onfocus='this.select()' onMouseUp='return false' value='"
        //                        + ds.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "' onchange='FillItemsAll(" + sno +
        //                        ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='CheckDiscount(" + sno +
        //                        "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
        //                        "  style='text-align: right; width:50px;' class='bcAsptextbox'/>%</td>");
        //                    if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
        //                        sb.Append("<td></td>");
        //                    else
        //                    {
        //                        sb.Append("<td><input type='text' size='05px' id='txtPercent" + sno +
        //                        "' onfocus='this.select()' onMouseUp='return false' value='"
        //                            + ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString() + "' onchange='FillItemsAll(" + sno
        //                            + ")' onblur='extractNumber(this,2,false);' onkeyup='CheckExDuty(" + sno
        //                            + "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
        //                            " maxlength='18' style='text-align: right; width: 50px;' class='bcAsptextbox'/>%</td>");
        //                    }
        //                    if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0)
        //                        FlagExDuty = 1;
        //                    if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0)
        //                        FlagDiscount = 1;
        //                    sb.Append("<td align='right'><span id='spnNetRate" + sno + "'>"
        //                        + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["QPriceC"].ToString()), 2)
        //                    + "&nbsp;</span></td>");
        //                    if (ds.Tables[0].Rows[i]["GrandTotal"].ToString() != "")
        //                        sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>"
        //                            + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString()), 2) + "</span></td>");
        //                    else
        //                        sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>0.00</span></td>");

        //                    sb.Append("<td><input type='text' name='txtRemarks' onchange='FillItemsAll(" + sno +
        //                        ")' id='txtRemarks" + sno + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString() +
        //                        "' style='width:60px' class='bcAsptextbox'/></td>");
        //                    sb.Append("<td></td>");
        //                    sb.Append("<td></td>");
        //                    sb.Append("</tr>");
        //                }

        //                if (Gtotl != 0)
        //                    GrandTotal = Convert.ToDecimal(Gtotl);

        //                sb.Append("<tfoot>");
        //                sb.Append("<tr class='bcGridViewHeaderStyle'>");
        //                sb.Append("<th colspan='5' class='rounded-foot-left'><b><span></span></b></th>");
        //                sb.Append("<th align='right'> <input type='hidden' name='HFExDuty' id='HFExDuty' value='" + FlagExDuty + "'/> </th>");
        //                sb.Append("<th align='right'> <input type='hidden' name='HFDiscount' id='HFDiscount' value='"
        //                    + FlagDiscount + "'/> </th>");
        //                sb.Append("<th colspan='4' align='right'><b><span>Total Amount : <label id='lblTotalAmt'>"
        //                    + Math.Round(Convert.ToDecimal(TotalAmount), 2) + "</label></span></b></th>");
        //                sb.Append("<th colspan='4' align='right'><b><span>Grand Total : <label id='lblGTAmt'> "
        //                    + Math.Round(Convert.ToDecimal(GrandTotal), 2) + "</label></span></b></th>");
        //                sb.Append("<th colspan='4' class='rounded-foot-right' ><b><span></span></b></th>");
        //                sb.Append("</tr></tfoot>");
        //            }
        //            sb.Append("</tbody></table>");
        //            Session["sb"] = sb.ToString();
        //            return sb.ToString();
        //        }
        //        else
        //            return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int Lno = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
        //        return ex.Message;
        //    }
        //} 
        #endregion

        #endregion

        #region Methods ADD Items

        public string FillGridItems(Guid id)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                DataTable dt = new DataTable();
                HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                Dictionary<int, Guid> CatCodes = new Dictionary<int, Guid>();
                Dictionary<int, Guid> UnitCodes = new Dictionary<int, Guid>();

                dt.Columns.Add(new DataColumn("SNo", typeof(int)));
                dt.Columns.Add(new DataColumn("ItemId", typeof(Guid)));
                dt.Columns.Add(new DataColumn("FPOID", typeof(Guid)));//Need to Remove While Saving
                dt.Columns.Add(new DataColumn("PartNumber", typeof(string)));
                dt.Columns.Add(new DataColumn("Specifications", typeof(string)));
                dt.Columns.Add(new DataColumn("Make", typeof(string)));
                dt.Columns.Add(new DataColumn("Rate", typeof(decimal)));
                dt.Columns.Add(new DataColumn("QPrice", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                dt.Columns.Add(new DataColumn("ExDutyPercentage", typeof(decimal)));
                dt.Columns.Add(new DataColumn("DiscountPercentage", typeof(decimal)));
                dt.Columns.Add(new DataColumn("UNumsId", typeof(Guid)));
                dt.Columns.Add(new DataColumn("Remarks", typeof(string)));

                dt.Columns.Add(new DataColumn("ItemDetailsId", typeof(Guid)));//Need to Remove While Saving
                dt.Columns.Add(new DataColumn("Category", typeof(string)));//Need to Remove While Saving

                Session["VerbalLPOCust"] = dt;
                Session["SelectedItems"] = Codes;
                Session["SelectedCat"] = CatCodes;
                Session["SelectedUnits"] = UnitCodes;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "Customer FPO Verbal", ex.Message.ToString());
            }
            return FillItemGrid(false);
        }

        private void fillEditGrid(DataSet EditDS)
        {
            try
            {
                DataSet dss = new DataSet();
                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
                Dictionary<int, Guid> Codes1 = new Dictionary<int, Guid>();
                Dictionary<int, Guid> CatCodes1 = new Dictionary<int, Guid>();
                Dictionary<int, Guid> UnitCodes1 = new Dictionary<int, Guid>();


                DataTable dtt = EditDS.Tables[0].DefaultView.ToTable(false, "ItemId", "PartNumber",
                    "Specifications", "Make", "Quantity", "UNumsId", "Rate", "Amount", "Remarks", "ItemDetailsId");
                //dtt.Columns["ItemId"].ColumnName = "ItemId";
                //dtt.Columns["PartNumber"].ColumnName = "PartNumber";
                //dtt.Columns["Specifications"].ColumnName = "Specifications";
                //dtt.Columns["ItemDetailsId"].ColumnName = "ItemDetailsId";
                //dtt.Columns["UNumsId"].ColumnName = "UNumsId";

                DataColumn dc = dtt.Columns.Add("SNo", typeof(int));
                DataColumn dc1 = dtt.Columns.Add("Category", typeof(Guid));
                dc.SetOrdinal(0);
                dc1.SetOrdinal(1);
                int sno = 1;
                foreach (DataRow dr in dtt.Rows)
                {
                    dr["SNo"] = sno;
                    dr["Category"] = GeneralCtgryID;
                    Codes1.Add(sno, new Guid(dr["ItemId"].ToString()));
                    UnitCodes1.Add(sno, new Guid(dr["UNumsId"].ToString()));
                    sno++;
                }
                CatCodes1.Add(1, new Guid(GeneralCtgryID));
                HttpContext.Current.Session["SelectedItems"] = Codes1;
                HttpContext.Current.Session["SelectedCat"] = CatCodes1;
                HttpContext.Current.Session["SelectedUnits"] = UnitCodes1;

                HttpContext.Current.Session["VerbalLPOCust"] = dtt;
                divLPOItems.InnerHtml = FillItemGrid(false);//0, dtt, Codes1, dss, Convert.ToInt32(GeneralCtgryID), CatCodes1, 0, UnitCodes1, ""
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "Customer FPO Verbal", ex.Message.ToString());
            }
        }

        private string FillItemGrid(bool ADD)
        {
            try
            {
                int RowNo = 1;
                DataTable dt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                DataSet ds2 = null;
                if (Session["ItemUnits"] == null)
                {
                    Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
                }
                DataSet dsUnits = (DataSet)Session["ItemUnits"];
                StringBuilder sb = new StringBuilder();
                if (!ADD)
                {
                    sb.Append("");
                    // sb.Append("<div class='aligntable' id='aligntbl' style='margin-left: 10px !important;'>");
                    sb.Append("<table width='98%' height='15%' cellspacing='0' cellpadding='0' border='0' " + " id='tblItems'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th>SNo</th><th>Item Description</th><th></th><th>Part No</th><th>Specification</th>" +
                        "<th>Make</th><th>Quantity</th><th>Units</th><th>Rate($)</th><th>Amount($)</th><th>Remarks</th><th></th><th></th></tr>");
                    sb.Append("</thead><tbody>");
                }

                #region Body
                decimal TotalAmount = 0;
                if (dt.Rows.Count > 0)
                {
                    RowNo = dt.Rows.Count + 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        TotalAmount += Convert.ToDecimal(dt.Rows[i]["Amount"].ToString());
                        ds2 = null;
                        sb.Append("<tr>");

                        sb.Append("<td>" + (i + 1) + "</td>");
                        dt.Rows[i]["SNo"] = (i + 1);
                        sb.Append("<td>");
                        DataRow[] drarray = null;
                        DataSet ds = (DataSet)Session["ItemsDescPrtNo"];
                        if (dt.Rows.Count > 0)
                        {
                            drarray = ds.Tables[0].Select("ID=" + "'" + dt.Rows[i]["ItemId"] + "'");
                            if (drarray.Count<DataRow>() > 0)
                            {
                                sb.Append("<textarea id='txtItemDesc" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' "
                                    + "onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                                    + drarray[0][1].ToString() + " </textarea>");
                                sb.Append("<input type='hidden' id='hfItemID" + (i + 1) + "' value='" + drarray[0][0].ToString() + "'/>");
                            }
                        }
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<textarea id='txtPartNo" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)'"
                            + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                            + drarray[0][2].ToString() + " </textarea>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<textarea id='txtSpec" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)'"
                            + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                            + drarray[0][3].ToString() + " </textarea>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblMake" + (i + 1) + "'  >" + dt.Rows[i]["Make"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblQuantity" + (i + 1) + "'  >" + dt.Rows[i]["Quantity"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        DataRow[] dr = dsUnits.Tables[0].Select("ID=" + "'" + dt.Rows[i]["UNumsId"] + "'");
                        if (dr.Length > 0)
                        {
                            sb.Append("<label id='lblUnit" + (i + 1) + "'  >" + dr[0]["Description"].ToString() + "</label>");
                        }
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblRate" + (i + 1) + "'  >" + dt.Rows[i]["Rate"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblAmount" + (i + 1) + "'  >" + dt.Rows[i]["Amount"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblRemarks" + (i + 1) + "'  >" + dt.Rows[i]["Remarks"].ToString() + "</label>");
                        sb.Append("</td>");
                        //sb.Append("<label id='lblTotalAmount0'>Total Amount($) :" + TotalAmount + "</label>");
                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnDel" + (i + 1) + "' href='javascript:void(0)' " +
                            " onclick='javascript:return doConfirmm(" + i.ToString() + ")' class='icons deleteicon' title='Delete'>" +
                            " <img src='../images/Delete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");

                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnEdit" + (i + 1) + "' href='javascript:void(0)' " +
                            " onclick='javascript:return EditSelectedItem(" + i.ToString() + ")' class='icons deleteicon' title='Edit'>" +
                            " <img src='../images/Edit.jpeg' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    dt.AcceptChanges();
                }
                #endregion

                if (!ADD)
                {
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th><label id='lblEditID0'></label></th>");
                    sb.Append("<th>");
                    #region Fill Items Dropdown
                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                    DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                    sb.Append("<select id='ddl0' Class='bcAspdropdown' width='50px' onchange='FillSpec_ItemDesc(0)'>");
                    sb.Append("<option value='00000000-0000-0000-0000-000000000000'>-SELECT-</option>");
                    int countRow = 0;

                    foreach (DataRow row in dss.Tables[0].Rows)
                    {
                        Guid ItemID = Guid.Empty;
                        if (!Codes.ContainsValue(new Guid(row["ID"].ToString())) && new Guid(row["ID"].ToString()) != ItemID)
                            sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["ItemDescription"].ToString() +
                                "</option>");
                        countRow++;
                    }

                    sb.Append("</select>");
                    sb.Append("</th><th>");
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Customer Access/AddItem_PI.aspx"))
                    {
                        sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' class='icons additionalrow' "
                            + " ID='btnShow'  title='Add Item to Item Master' onclick='fnOpen(0)'>"
                            + " <img src='../images/AddNW.jpeg' style='border-style: none;'/></a></span>");
                    }

                    #endregion
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtPartNo0' class='bcAsptextboxmulti_Footer' readonly='readonly' style='height:20px; width:150px; resize:none;'></th>");
                    sb.Append("<th><textarea id='txtSpec0' class='bcAsptextboxmulti_Footer' onfocus='ExpandTXT(this.id)' readonly='readonly'"
                                + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;'></textarea></th>");
                    sb.Append("<th><input type='text' id='txtMake0' class='bcAsptextboxmulti_Footer' style='height:20px; width:150px; resize:none;'></th>");
                    sb.Append("<th><input type='text' id='txtQuantity0' onblur='extractNumber(this,2,false);' "
                        + "onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' class='bcAsptextboxmulti_Footer'></th>");
                    sb.Append("<th>");
                    #region Fill Units Dropdown
                    sb.Append("<select id='ddlUnits0' class='bcAspdropdown' style='width:85px;'>");

                    sb.Append("<option value='0'>-SELECT-</option>");

                    foreach (DataRow row in dsUnits.Tables[0].Rows)
                    {
                        if (row["Description"].ToString().ToUpper() == "NO(S)")
                            sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                        else
                            sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                    }
                    sb.Append("</select>");
                    #endregion
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtRate0' onblur='extractNumber(this,2,false);' "
                        + "onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' class='bcAsptextboxmulti_Footer'></th>");
                    sb.Append("<th align='right'>");
                    sb.Append("<label id='lblTotalAmount0'>Total Amount($) :" + TotalAmount + "</label>");
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtRemarks0' class='bcAsptextboxmulti_Footer' style='height:20px; width:150px; resize:none;'></th>");
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
                                        " onclick='CancelEdit()'class='icons additionalrow' title='Cancel' style='display:none;'>"
                                        + "<img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                    sb.Append("</th></tr>");
                    sb.Append("</tfoot></table>");
                    // sb.Append("</div>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ex.Message;
            }
        }

        #endregion

        #region DropDownLists Selected Index Changed Events

        #region Not In USe
        /// <summary>
        /// FP Order Number Select Index changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ////protected void ddlFpoNo_SelectedIndexChanged(object sender, EventArgs e)
        ////{
        ////    try
        ////    {
        ////        fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
        ////        Session["FPOSelected"] = fpono;
        ////        BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
        ////            new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

        ////        ddlSuplr.Items.Clear();
        ////        BindDropDownList(ddlSuplr, sbll.SelectSuppliersForBind(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
        ////        FillInputFields(fpono, ddlCustomer.SelectedValue);



        ////        DataSet ds = IDBLL.SelectItemDtlsLPOVerbal(CommonBLL.FlagHSelect, fpono);
        ////        DataColumn dc = new DataColumn("Check", typeof(bool));
        ////        dc.DefaultValue = true;
        ////        ds.Tables[0].Columns.Add(dc);
        ////        foreach (DataRow item in ds.Tables[0].Rows)
        ////        {
        ////            if (item["Status"].ToString() == Convert.ToString(50))
        ////                item["Check"] = true;
        ////            else
        ////                item["Check"] = false;
        ////        }
        ////        if (ds.Tables[0].Columns.Contains("Rate"))
        ////            ds.Tables[0].Columns.Remove("Rate");
        ////        DataColumn dc1 = new DataColumn("Rate", typeof(decimal));
        ////        dc1.DefaultValue = 0;
        ////        ds.Tables[0].Columns.Add(dc1);
        ////        ds.Tables[0].AcceptChanges();


        ////        Session["VerbalLPOCust"] = ds;
        ////        divLPOItems.InnerHtml = FillGridView(ds, 0, false);
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        string ErrMsg = ex.Message;
        ////        int LineNo = ExceptionHelper.LineNumber(ex);
        ////        ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
        ////    }
        ////} 
        #endregion

        /// <summary>
        /// F-Quotation Number Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFquoteNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenq_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Name Selected Index Changed Event for reference L-Quotation Number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSuplr.SelectedValue != Guid.Empty.ToString())
                {
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    ////string feno1 = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    //string feno2 = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    //DataSet LQuotations = NLPOBL.GetDataSet(CommonBLL.FlagODRP, Guid.Empty, Guid.Empty, feno1, feno2, Guid.Empty, Guid.Empty,
                    //    Guid.Empty, DateTime.Now, DateTime.Now, new Guid(ddlSuplr.SelectedValue), "", DateTime.Now, 0, 100, "", new Guid(Session["UserID"].ToString()),
                    //    CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                    //LocalPurchaseOrder(ListBoxLPO, LQuotations);
                    //txtLpono.Text = LQuotations.Tables[3].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + (LQuotations.Tables[1].Rows[0]["LPOSequence"].ToString()).PadLeft(3, '0') + "/" +
                    //ListBoxFPO.SelectedItem.Text.Trim() + "/" + CommonBLL.FinacialYearShort;
                    //ddlsuplrctgry.SelectedValue = LQuotations.Tables[2].Rows[0]["CategoryID"].ToString();
                    //gvLpoItems.DataBind();
                }
                else
                {
                    ddlPrcBsis.SelectedIndex = -1;
                    divPaymentTerms.InnerHtml = "";
                    txtDlvry.Text = "0";
                    //ListBoxLPO.Items.Clear();
                    ////gvLpoItems.DataSource = null;
                    ////gvLpoItems.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// L-Quotation Number Selected Index Changed Event for Items Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlLQuoteNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //bool IsExciseDuty = false;
                ////if (ListBoxLPO.SelectedValue != "")
                ////{
                ////string flqno = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //string fpno = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //DataSet LclQuoteItems = NLQBL.LPOItemsByMulti(CommonBLL.FlagBSelect, Guid.Empty, "", Guid.Empty, Guid.Empty,
                //    Guid.Empty, Guid.Empty, new Guid(ddlSuplr.SelectedValue), "", 0, "", Guid.Empty, CommonBLL.EmptyDtLocalQuotation(),
                //    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), Guid.Empty, fpno, new Guid(Session["CompanyID"].ToString()));
                //if (LclQuoteItems.Tables[1].Rows.Count > 0)
                //{
                //    ComparisonDTS = (DataSet)ViewState["ComparisonDTS"];
                //    if (ComparisonDTS != null)
                //    {
                //        ComparisonDTS.Tables[0].Merge(LclQuoteItems.Tables[1]);
                //        ComparisonDTS.GetChanges();
                //    }
                //    if (LclQuoteItems.Tables.Count > 4)
                //        ViewState["Quantity"] = LclQuoteItems.Tables[4];

                //    ////gvLpoItems.DataSource = LclQuoteItems.Tables[1];
                //    ////gvLpoItems.DataBind();

                //    ////if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                //    ////    gvLpoItems.Columns[11].Visible = false;

                //    ddlPrcBsis.SelectedValue = LclQuoteItems.Tables[0].Rows[0]["PriceBasis"].ToString();
                //    txtPriceBasis.Text = LclQuoteItems.Tables[0].Rows[0]["PriceBasisText"].ToString();
                //    txtDlvry.Text = LclQuoteItems.Tables[0].Rows[0]["DeliveryPeriods"].ToString();
                //    if (txtDlvry.Text != "")
                //        txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text))));

                //    if ((Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString()) == 0) && IsExciseDuty == false)
                //    {
                //        ChkbCEEApl.Enabled = true; ChkbCEEApl.Checked = true;
                //        txtCEEApl.Enabled = true;
                //        IsExciseDuty = true;
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDutyRmd",
                //            "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                //    }
                //    else
                //    {
                //        ChkbCEEApl.Enabled = false; ChkbCEEApl.Checked = false;
                //        txtCEEApl.Enabled = false;
                //        IsExciseDuty = false;
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDutyRmd",
                //            "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                //    }
                //    txtExdt.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString()) == 0) ?
                //        "0" : LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString();
                //    if (txtExdt.Text != "0")
                //    {
                //        chkExdt.Checked = true;
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty",
                //            "CHeck('chkExdt', 'dvExdt');", true);
                //    }

                //    txtPkng.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["packingPercentage"].ToString()) == 0) ?
                //        "0" : LclQuoteItems.Tables[0].Rows[0]["packingPercentage"].ToString();
                //    if (txtPkng.Text != "0")
                //    {
                //        chkPkng.Checked = true;
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng",
                //            "CHeck('chkPkng', 'dvPkng');", true);
                //    }

                //    txtDsnt.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString()) > 0) ?
                //        LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString() : LclQuoteItems.Tables[0].Rows[0]["DiscountPercentage"].ToString();
                //    if (Convert.ToDecimal(txtDsnt.Text) > 0)
                //    {
                //        chkDsnt.Checked = true;
                //        string SelVal = "";
                //        rbtnDsnt.SelectedValue = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString()) > 0) ? "1" : "0";

                //        SelVal = rbtnDsnt.SelectedValue;

                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt",
                //            "CHeckForLpo('chkDsnt', 'dvDsnt','" + SelVal + "');", true);
                //    }

                //    txtSltx.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["SaleTaxPercentage"].ToString()) == 0) ?
                //        "0" : LclQuoteItems.Tables[0].Rows[0]["SaleTaxPercentage"].ToString();
                //    if (txtSltx.Text != "0")
                //    {
                //        chkSltx.Checked = true;
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx",
                //            "CHeck('chkSltx', 'dvSltx');", true);
                //    }
                //}
                //if (LclQuoteItems.Tables.Count > 1 && LclQuoteItems.Tables[2].Rows.Count > 0)
                //{
                //    FillQuotationTerms(LclQuoteItems.Tables[2]);
                //}
                //}
                //else
                //{
                //    divPaymentTerms.InnerHtml = "";
                //    txtDlvry.Text = "0";
                //    gvLpoItems.DataSource = null;
                //    gvLpoItems.DataBind();
                //}

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomerSelectionChanged();
        }

        private void CustomerSelectionChanged()
        {
            try
            {
                if (ddlCustomer.SelectedValue != Guid.Empty.ToString())
                {
                    ClearAll();
                    ddlSuplr.Items.Clear();
                    DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                    if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                        EmtpyFPO.Columns.Remove("ItemDetailsId");
                    ////LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagISelect, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    ////    Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    ////    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                    ////    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    ////    CommonBLL.ATConditions()));
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    GetLocalVendorNames();
                }
                else
                {
                    ClearAll();
                    ddlSuplr.Items.Clear();
                }
                divLPOItems.InnerHtml = FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        private void GetLocalVendorNames()
        {
            try
            {
                ddlSuplr.Items.Clear();
                SupplierBLL SBLL = new SupplierBLL();
                DataSet ds = new DataSet();
                ds = SBLL.GetSuppliersByFe(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "BussName ASC";
                    ddlSuplr.DataSource = ds.Tables[0];
                    ddlSuplr.DataTextField = "BussName";
                    ddlSuplr.DataValueField = "ID";
                    ddlSuplr.DataBind();
                    ddlSuplr.Items.Insert(0, new ListItem("-- Select Supplier --", Guid.Empty.ToString()));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('No Local Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());

            }
        }

        /// <summary>
        /// Quantity Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtQty_TextChanged(object sender, EventArgs e)
        {
            TextBox txet = (TextBox)sender;
            GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
            //Label GTotal = (Label)gvLpoItems.FooterRow.FindControl("lbltmnt");
            //Label MTotal = (Label)gvLpoItems.FooterRow.FindControl("lblTotl");
            Label Rate = (Label)row.FindControl("lblRate");
            TextBox CngRate = (TextBox)row.FindControl("txtRate");
            TextBox CngQty = (TextBox)row.FindControl("txtQty");
            Label NetRate = (Label)row.FindControl("lblNRate");
            Label TotalR = (Label)row.FindControl("lblTotal");
            Label Amount = (Label)row.FindControl("lblAmount");
            HiddenField HdnRate = (HiddenField)row.FindControl("HF_CngPrice");

            //Decimal TempVal = Convert.ToDecimal(GTotal.Text) - Convert.ToDecimal(Amount.Text);
            if (ChkbCEEApl.Checked == true)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls",
                "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
            }
            if (txet.Text != "")
            {
                if (Request.QueryString != null && Request.QueryString.Count >= 2 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] != null && (Convert.ToBoolean(Request.QueryString["IsAm"].ToString()) == true))
                {
                    HdnRate.Value = CngRate.Text;
                    decimal Rte = Convert.ToDecimal(CngRate.Text);
                    decimal Qty = Convert.ToDecimal(CngQty.Text);
                    Amount.Text = (Qty * Rte).ToString();
                    TotalR.Text = (Qty * Rte).ToString();
                    //GTotal.Text = (TempVal + (Qty * Rte)).ToString();
                    //MTotal.Text = (TempVal + (Qty * Rte)).ToString();
                }
                else
                {
                    decimal Rte = Convert.ToDecimal(Rate.Text);
                    decimal Qty = Convert.ToDecimal(txet.Text);
                    Amount.Text = (Qty * Rte).ToString();
                    TotalR.Text = (Qty * Rte).ToString();
                    //GTotal.Text = (TempVal + (Qty * Rte)).ToString();
                    //MTotal.Text = (TempVal + (Qty * Rte)).ToString();
                }

            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save/Update LP Orders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (res != 0)
                {
                    decimal exdt = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0;
                    DateTime lpoDate = CommonBLL.DateInsert(txtLpoDt.Text);
                    DateTime lpoDueDate = CommonBLL.DateInsert(txtLpoDueDt.Text);
                    ////string LPSN = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());
                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    if (TCs.Columns.Contains("Title"))
                        TCs.Columns.Remove("Title");
                    if (TCs.Columns.Contains("CompanyId"))
                        TCs.Columns.Remove("CompanyId");

                    DataTable VerbalDT = (DataTable)Session["VerbalLPOCust"];
                    if (VerbalDT.Columns.Contains("ItemDetailsId"))
                        VerbalDT.Columns.Remove("ItemDetailsId");
                    if (VerbalDT.Columns.Contains("Category"))
                        VerbalDT.Columns.Remove("Category");


                    //DataRow[] rows = VerbalDT.Tables[0].Select("Check = 'false'");
                    //foreach (DataRow r in rows)
                    //    VerbalDT.Tables[0].Rows.Remove(r);
                    //VerbalDT.Tables[0].AcceptChanges();

                    //DataTable dtt = VerbalDT.Tables[0].DefaultView.ToTable(false,
                    //    "FESNo", "ItemId", "ForeignPOId", "PartNumber", "Specifications", "Make", "Rate", "QPriceC", "Quantity",
                    //    "Amount", "ExDutyPercentage", "DiscountPercentage", "UNumsId", "Remarks");
                    //dtt.Columns["FESNo"].ColumnName = "SNo";
                    //dtt.Columns["ForeignPOId"].ColumnName = "FPOID";
                    //dtt.Columns["QPriceC"].ColumnName = "QPrice";

                    //DataTable dtbl = dtt;
                    DataTable sdt = (DataTable)Session["PaymentTermsLPO"];

                    if (sdt.Columns.Contains("CompanyId"))
                        sdt.Columns.Remove("CompanyId");
                    if (VerbalDT.Rows.Count > 0)
                    {
                        string Attachments = "";
                        if (Session["LPOUploads"] != null)
                        {
                            ArrayList all = (ArrayList)Session["LPOUploads"];
                            Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                        }

                        if (btnSave.Text == "Save" && VerbalDT.Rows.Count > 0 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True")
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkDsnt.Checked)
                                if (rbtnDsnt.SelectedValue == "0")
                                    dscntPrcnt = Convert.ToDecimal(txtDsnt.Text);
                                else if (rbtnDsnt.SelectedValue == "1")
                                    dscnt = Convert.ToDecimal(txtDsnt.Text);
                                else dscnt = Convert.ToDecimal(txtDsnt.Text);
                            if (chkSltx.Checked)
                                if (rbtnSltx.SelectedValue == "0")
                                    sltxPrcnt = Convert.ToDecimal(txtSltx.Text);
                                else if (rbtnSltx.SelectedValue == "1")
                                    sltx = Convert.ToDecimal(txtSltx.Text);
                                else sltx = Convert.ToDecimal(txtSltx.Text);
                            if (chkPkng.Checked)
                                if (rbtnPkng.SelectedValue == "0")
                                    pkngPrcnt = Convert.ToDecimal(txtPkng.Text);
                                else if (rbtnPkng.SelectedValue == "1")
                                    pkng = Convert.ToDecimal(txtPkng.Text);
                                else pkng = Convert.ToDecimal(txtPkng.Text);

                            DateTime DlvryDt = DateTime.Now;
                            DlvryDt = DlvryDt.Date.AddDays(7 * (Convert.ToDouble(txtDlvry.Text)));

                            string LpoNum = txtLpono.Text.Split('/')[2];
                            string path = Request.Path;
                            path = Path.GetFileName(path);
                            DataSet CrntIdnt = NLPOBL.SelectLPOrders(CommonBLL.FlagISelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                                CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                            ////string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + LpoNum + "/" +
                            ////LPSN + "/" + CommonBLL.FinacialYearShort + "/Amend/" + CrntIdnt.Tables[1].Rows[0]["HistoryNum"].ToString();

                            ////string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + LpoNum + "/" +
                            ////CommonBLL.FinacialYearShort + "/Amend/" + CrntIdnt.Tables[1].Rows[0]["HistoryNum"].ToString();


                            ////string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            //string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            ////res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, fponum, "", Guid.Empty, Guid.Empty,
                            ////    new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                            ////    "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                            ////    txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                            ////    (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                            ////    (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                            ////    (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                            ////    new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                            ////    CommonBLL.StatusTypeLPOrder, "",
                            ////    new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, dscnt,
                            ////    dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));

                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, "", "", Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), txtLpono.Text.Trim(), lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), VerbalDT, sdt, TCs, exdt, exdtPrcnt,0,0,0,0, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/Log"), "VerbalLPO_Customer",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }

                        else if (btnSave.Text == "Save" && VerbalDT.Rows.Count > 0)
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkDsnt.Checked)
                                if (rbtnDsnt.SelectedValue == "0")
                                    dscntPrcnt = Convert.ToDecimal(txtDsnt.Text);
                                else if (rbtnDsnt.SelectedValue == "1")
                                    dscnt = Convert.ToDecimal(txtDsnt.Text);
                                else dscnt = Convert.ToDecimal(txtDsnt.Text);
                            if (chkSltx.Checked)
                                if (rbtnSltx.SelectedValue == "0")
                                    sltxPrcnt = Convert.ToDecimal(txtSltx.Text);
                                else if (rbtnSltx.SelectedValue == "1")
                                    sltx = Convert.ToDecimal(txtSltx.Text);
                                else sltx = Convert.ToDecimal(txtSltx.Text);
                            if (chkPkng.Checked)
                                if (rbtnPkng.SelectedValue == "0")
                                    pkngPrcnt = Convert.ToDecimal(txtPkng.Text);
                                else if (rbtnPkng.SelectedValue == "1")
                                    pkng = Convert.ToDecimal(txtPkng.Text);
                                else pkng = Convert.ToDecimal(txtPkng.Text);

                            DateTime DlvryDt = DateTime.Now;
                            DlvryDt = DlvryDt.Date.AddDays(7 * (Convert.ToDouble(txtDlvry.Text)));
                            string path = Request.Path;
                            path = Path.GetFileName(path);
                            DataSet CrntIdnt = NLPOBL.SelectLPOrders(CommonBLL.FlagISelect, Guid.Empty, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                                CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                            ////string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" +
                            ////LPSN + "/" + CommonBLL.FinacialYearShort;

                            string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" +
                            CommonBLL.FinacialYearShort;

                            ////string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            //string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            ////res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, fponum, "", Guid.Empty, Guid.Empty,
                            ////    new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                            ////    "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                            ////    txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                            ////    (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                            ////    (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                            ////    (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                            ////    new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                            ////    CommonBLL.StatusTypeLPOrder, "",
                            ////    new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, dscnt,
                            ////    dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));

                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, "", "", Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), VerbalDT, sdt, TCs, exdt, exdtPrcnt,0,0,0,0, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));

                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/Log"), "VerbalLPO_Customer",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }
                        else if (btnSave.Text == "Update" && VerbalDT.Rows.Count > 0)
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkDsnt.Checked)
                                if (rbtnDsnt.SelectedValue == "0")
                                    dscntPrcnt = Convert.ToDecimal(txtDsnt.Text);
                                else if (rbtnDsnt.SelectedValue == "1")
                                    dscnt = Convert.ToDecimal(txtDsnt.Text);
                                else dscnt = Convert.ToDecimal(txtDsnt.Text);
                            if (chkSltx.Checked)
                                if (rbtnSltx.SelectedValue == "0")
                                    sltxPrcnt = Convert.ToDecimal(txtSltx.Text);
                                else if (rbtnSltx.SelectedValue == "1")
                                    sltx = Convert.ToDecimal(txtSltx.Text);
                                else sltx = Convert.ToDecimal(txtSltx.Text);
                            if (chkPkng.Checked)
                                if (rbtnPkng.SelectedValue == "0")
                                    pkngPrcnt = Convert.ToDecimal(txtPkng.Text);
                                else if (rbtnPkng.SelectedValue == "1")
                                    pkng = Convert.ToDecimal(txtPkng.Text);
                                else pkng = Convert.ToDecimal(txtPkng.Text);

                            DateTime DlvryDt = DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text)));
                            ////string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                            ////res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                            ////    Guid.Empty, fponum, "", Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                            ////    txtLpono.Text, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue),
                            ////    new Guid(ddlSuplr.SelectedValue), "", txtsubject.Text,
                            ////    new Guid(ddlRsdby.SelectedValue), txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked,
                            ////    ChkbDrwngAprls.Checked,
                            ////    (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                            ////    (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                            ////    (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                            ////    new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                            ////    CommonBLL.StatusTypeLPOrder,
                            ////    txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs,
                            ////    exdt, exdtPrcnt, dscnt, dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));

                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                                Guid.Empty, "", "", Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                                txtLpono.Text, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue),
                                new Guid(ddlSuplr.SelectedValue), "", txtsubject.Text,
                                new Guid(ddlRsdby.SelectedValue), txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked,
                                ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder,
                                txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), VerbalDT, sdt, TCs,
                                exdt, exdtPrcnt,0,0,0,0, dscnt, dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));

                            if (res == 0 && btnSave.Text == "Update")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/Log"), "VerbalLPO_Customer",
                                    "Data Updated Successfully.");
                                ClearAll(); Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Updating.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", "Error while Updating.");
                                FillPaymentTerms();
                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('There are No Items to Save/Update.');", true);
                    }
                    else if (VerbalDT.Rows.Count > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('There are no ITEMS to Save, select minimum 1 Item.');", true);

                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('You should not save twice.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear All Form Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
                Response.Redirect("../Customer Access/NewLPOrderVerbal.Aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        #endregion

        # region Payment WebMethods

        /// <summary>
        /// This is used to Delete payment Items
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PaymentDeleteItem(int rowNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["PaymentTermsLPO"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[rowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["amountLPO"] = dt.Compute("Sum(PaymentPercentage)", "");
                Session["PaymentTermsLPO"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
            return FillPaymentTerms();
        }

        /// <summary>
        /// This is used to Additems and add new row
        /// </summary>
        /// <param name="rowNo"></param>
        /// <param name="Pay"></param>
        /// <param name="Desc"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PaymentAddItem(int rowNo, string Pay, string Desc)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["PaymentTermsLPO"];
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = CommonBLL.FirstRowPaymentTerms();
                    Session["PaymentTermsLPO"] = dt;
                }
                int count = dt.Rows.Count;
                int PmntPercent = 0;

                object OldAmt = dt.Rows[rowNo - 1]["PaymentPercentage"];
                PmntPercent = Convert.ToInt32(Convert.ToInt64(Convert.ToDouble(dt.Compute("Sum(PaymentPercentage)", ""))));
                //PmntPercent += Convert.ToInt32(Pay) - Convert.ToInt32(OldAmt);
                PmntPercent += Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Pay))) - Convert.ToInt32(OldAmt);

                if (PmntPercent <= 100)
                {
                    dt.Rows[rowNo - 1]["PaymentPercentage"] = Pay;
                    dt.Rows[rowNo - 1]["Description"] = Desc;

                    string amt = dt.Rows[count - 1]["PaymentPercentage"].ToString();
                    if (amt != "" && amt != "0" && dt.Rows[count - 1]["Description"].ToString() != "" && PmntPercent < 100)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        if (dt.Rows[rowNo]["PaymentPercentage"].ToString() == "")
                        {
                            dt.Rows[rowNo]["PaymentPercentage"] = 0;
                            dt.Rows[rowNo]["SNo"] = (rowNo + 1);
                        }
                        else
                        {
                            int newCount = dt.Rows.Count;
                            dt.Rows[newCount - 1]["PaymentPercentage"] = 0;
                            dt.Rows[newCount - 1]["SNo"] = (rowNo + 1);
                        }
                    }
                    else if (PmntPercent >= 100 && (dt.Rows[count - 1]["PaymentPercentage"].ToString() == "") || dt.Rows[count - 1]["PaymentPercentage"].ToString() == "0" && count > 1)
                        dt.Rows[count - 1].Delete();
                    Session["MessageLPO"] = null;
                    Session["amountLPO"] = PmntPercent.ToString();
                }
                else
                    Session["MessageLPO"] = "Percentage Cannot Exceed 100";
                Session["PaymentTermsLPO"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
            return FillPaymentTerms();
        }

        # endregion

        #region GridView Events

        /// <summary>
        /// Items GridView Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLpoItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                bool IsExciseDuty = false;
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].CssClass = "rounded-First";
                    e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-Last";
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    decimal CQuantity = 0;

                    //UsersGrid.Columns.Cast<DataControlField>().SingleOrDefault(x => x.HeaderText == "Email");

                    HiddenField HFExdtPrcnt = (HiddenField)e.Row.FindControl("HFExdtPrcnt");
                    Label lblitem = (Label)e.Row.FindControl("lblItemID");
                    Label amount = (Label)e.Row.FindControl("lblAmount");
                    Label NetRate = (Label)e.Row.FindControl("lblNRate");
                    Label GrnlRate = (Label)e.Row.FindControl("lblRate");
                    Label Total = (Label)e.Row.FindControl("lblTotal");
                    TextBox txtQty = (TextBox)e.Row.FindControl("txtQty");
                    HiddenField MaxQty = (HiddenField)e.Row.FindControl("Hfd_MQty");
                    HiddenField RcvdQty = (HiddenField)e.Row.FindControl("HF_RCVDQty");
                    TextBox txtRate = (TextBox)e.Row.FindControl("txtRate");

                    DataTable Quantity = (DataTable)ViewState["Quantity"];

                    DataRow[] Qty = Quantity.Select("ItemId = '" + lblitem.Text + "'");
                    Decimal EditQty = Convert.ToDecimal(txtQty.Text);
                    if (Qty != null && Qty.Length > 0)
                    {
                        ((HiddenField)e.Row.FindControl("HF_ActualQty")).Value = Qty[0]["ActualQty"].ToString();
                        MaxQty.Value = Qty[0]["Quantity"].ToString();
                        CQuantity = Convert.ToDecimal(Qty[0]["CQuantity"].ToString());

                        if (Request.QueryString != null && Request.QueryString.Count >= 2 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] != null && (Convert.ToBoolean(Request.QueryString["IsAm"].ToString()) == true))
                        {
                            txtRate.Visible = true; GrnlRate.Visible = false;
                            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2);
                            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                        }
                        else if (Request.QueryString != null && Request.QueryString.Count == 1 && Request.QueryString["ID"] != null)
                        {
                            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2);
                            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                        }
                        else
                        {
                            txtQty.Text = ((Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                        (Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)).ToString());
                            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "QPrice"))), 2).ToString();
                            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "QPrice"))), 2);
                            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                        }

                        RcvdQty.Value = ((Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                        (Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)).ToString());
                    }
                    else
                    {
                        amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                        MaxQty.Value = "0";
                        RcvdQty.Value = "0";
                        CQuantity = 0;

                        RunningTotal += (Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "qty"))
                        * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                        GTotal += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "totalAmt"));
                    }

                    if (Request.QueryString["ID"] != null && Request.QueryString["ID"].ToString() != "")
                    {
                        txtQty.Text = CQuantity.ToString();
                    }

                    if (Convert.ToDecimal(MaxQty.Value) == (Convert.ToDecimal(CQuantity) - EditQty))
                    {
                        txtQty.Enabled = false;
                    }

                    if (Convert.ToDecimal(HFExdtPrcnt.Value) == 0 && IsExciseDuty == false)
                    {
                        ChkbCEEApl.Enabled = true;
                        txtCEEApl.Enabled = true;
                    }
                    else
                        IsExciseDuty = true;
                }
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    ((Label)e.Row.FindControl("lbltmnt")).Text = (decimal.Round(Convert.ToDecimal(RunningTotal), 0, MidpointRounding.AwayFromZero)).ToString("N");
                    ((Label)e.Row.FindControl("lblTotl")).Text = (decimal.Round(Convert.ToDecimal(GTotal), 0, MidpointRounding.AwayFromZero)).ToString("N");
                    e.Row.Cells[0].CssClass = "rounded-foot-left";
                    e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-foot-right";
                }
                if (Request.QueryString["ID"] != null)
                {
                    if (e.Row.RowType != DataControlRowType.DataRow) return;
                    else
                    {
                        CheckBox cb = (CheckBox)e.Row.FindControl("ItmChkbx");
                        cb.Checked = true;
                    }
                }
                if (e.Row.RowType == DataControlRowType.Footer)
                {

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLpoItems_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvLpoItems.HeaderRow == null) return;
                //gvLpoItems.UseAccessibleHeader = false;
                //gvLpoItems.HeaderRow.TableSection = TableRowSection.TableHeader;
                //gvLpoItems.FooterRow.TableSection = TableRowSection.TableFooter;
                //gvLpoItems.Columns.Cast<DataControlField>().SingleOrDefault(x => x.HeaderText == "Ex-Dt(%)");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
        }
        #endregion

        # region CheckBox Events
        /// <summary>
        /// This is Used to check all the CheckBoxes from Header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HdrChkbx_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //CheckBox ChkParent = (CheckBox)sender;

                //if (ChkParent.Checked)
                //{
                //    ArrayList al = DT2Array();
                //    ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                //    bool IsFPOChk = false;
                //    bool IsLPOChk = false;
                //    string LPOItems = "";
                //    string FPOItems = "";
                //    string Error = "";
                //    for (int i = 0; i < gvLpoItems.Rows.Count; i++)
                //    {
                //        CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");

                //        HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
                //        if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
                //            cb.Checked = true;
                //        else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) &&
                //            !al1.Contains(new Guid(HfItemID.Value)))
                //            cb.Checked = true;
                //        else
                //        {
                //            ChkParent.Checked = false;
                //            cb.Checked = false;
                //            if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //            {
                //                FPOItems += " " + (i + 1) + ", ";
                //                IsFPOChk = true;
                //            }
                //            else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)))
                //            {
                //                LPOItems += " " + (i + 1) + ", ";
                //                IsLPOChk = true;
                //            }
                //            else
                //                Error = "Error while Checking.";
                //        }
                //    }

                //    if (IsLPOChk && IsFPOChk)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page,
                //            this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item No(s)" + LPOItems.Trim(',') +
                //             "And Item(s) " + FPOItems.Trim(',') + " are not selected in FPO.');", true);
                //    }
                //    else if (IsFPOChk == true && IsLPOChk == false)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page,
                //            this.GetType(), "ShowAll3", "ErrorMessage('Item(s) " + FPOItems.Trim(',')
                //            + " are not selected in FPO.');", true);
                //    }
                //    else if (IsFPOChk == false && IsLPOChk == true)
                //        ScriptManager.RegisterStartupScript(this.Page,
                //            this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItems.Trim(',') + " ');", true);
                //}
                //else
                //{
                //    for (int j = 0; j < gvLpoItems.Rows.Count; j++)
                //    {
                //        CheckBox cb = (CheckBox)gvLpoItems.Rows[j].FindControl("ItmChkbx");
                //        cb.Checked = false;
                //    }
                //}
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty1", "CHeck('chkExdt', 'dvExdt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng1", "CHeck('chkPkng', 'dvPkng');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt1", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx1", "CHeck('chkSltx', 'dvSltx');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
            divListBox.InnerHtml = AttachedFiles();
        }

        /// <summary>
        /// This is used to check Individual CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ItmChkbx_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //CheckBox ItmChkbx = (CheckBox)sender;
                //GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                //int rowIndex = Convert.ToInt32(row.RowIndex);
                //if (ItmChkbx.Checked)
                //{
                //    ArrayList al = DT2Array();
                //    ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                //    CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                //    HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[rowIndex].FindControl("HfItemID");
                //    if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
                //        cb.Checked = true;
                //    else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) && !al1.Contains(new Guid(HfItemID.Value)))
                //        cb.Checked = true;
                //    else
                //    {
                //        cb.Checked = false;
                //        if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //            ScriptManager.RegisterStartupScript(this.Page,
                //                this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected in FPO.');", true);
                //        else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)))
                //            ScriptManager.RegisterStartupScript(this.Page,
                //                this.GetType(), "ShowAll3", "ErrorMessage('This Item Was already Selected in Another LPO.');", true);
                //        else
                //            ScriptManager.RegisterStartupScript(this.Page,
                //                this.GetType(), "ShowAll2", "ErrorMessage('Error while Checking.');", true);
                //    }
                //}
                //else
                //{
                //    CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                //    cb.Checked = false;
                //}
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty2", "CHeck('chkExdt', 'dvExdt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "CHeck('chkPkng', 'dvPkng');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt2", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx2", "CHeck('chkSltx', 'dvSltx');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
            }
            divListBox.InnerHtml = AttachedFiles();
        }
        # endregion

        #region Web Methods
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool CheckDuplicateLPOs(string LpoNmbr)
        {
            try
            {
                DataSet Duplicate = NLPOBL.GetDuplicateLPODataSet(CommonBLL.FlagVSelect, LpoNmbr);
                if (Duplicate != null && Duplicate.Tables.Count > 0 && Duplicate.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return true;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["LPOUploads"];
                if (all.Count > 0)
                    all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }
        }

        #endregion

        # region WebMethods ADD items

        /// <summary>
        /// To Fill Item Grid view
        /// </summary>
        /// <param name="RowID">Selected Row No</param>
        /// <param name="SNo">row S.No.</param>
        /// <param name="CodeID">Item ID</param>
        /// <param name="ItemDetailsId"></param>
        /// <param name="CatID">Category ID</param>
        /// <param name="UnitID">Unit Id</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string FillItemGrid(int RowID, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID, string Rate, string Remarks)
        {
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                DataRow dr = dt.NewRow();
                dr["SNo"] = RowID;
                dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                dr["ItemId"] = ItemID;
                dr["PartNumber"] = PartNo;
                dr["Specifications"] = Spec;
                dr["Make"] = Make;
                dr["Quantity"] = Qty;
                dr["UNumsId"] = UnitID;
                dr["ItemDetailsId"] = Guid.Empty;

                if (Rate != "" && Rate != "0" && Convert.ToDecimal(Rate) > 0)
                {
                    dr["Rate"] = Convert.ToDecimal(Rate);
                    dr["Amount"] = Convert.ToDecimal(Rate) * Convert.ToDecimal(Qty);
                }
                else
                {
                    dr["Rate"] = 0;
                    dr["Amount"] = 0;
                }
                dr["Remarks"] = Remarks;


                dt.Rows.Add(dr);
                dt.AcceptChanges();
                HttpContext.Current.Session["VerbalLPOCust"] = dt;
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                int CodeCount = 0;
                if (Codes.Count > 0)
                    CodeCount = Codes.Keys.Max();
                CodeCount += 1;
                if (!Codes.ContainsValue(new Guid(ItemID)) && ItemID != Guid.Empty.ToString())
                    Codes.Add(CodeCount, new Guid(ItemID));

                Session["SelectedItems"] = Codes;
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// Delete Item Table Item
        /// </summary>
        /// <param name="ItemID">Selected Row</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItems(string ItemID)
        {
            try
            {
                Guid ID = new Guid(ItemID);
                DataTable dt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                DataRow[] dr = dt.Select("ItemId = " + "'" + ID + "'");
                if (dr.Length > 0)
                {
                    dr[0].Delete();
                    dt.AcceptChanges();

                    if (Codes != null && Codes.Count > 0 && Codes.ContainsValue(ID))
                    {
                        var item = Codes.First(kvp => kvp.Value == ID);
                        Codes.Remove(item.Key);
                    }
                }
                HttpContext.Current.Session["SelectedItems"] = Codes;
                HttpContext.Current.Session["VerbalLPOCust"] = dt;

                //DataTable dtt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                //DataSet dss = new DataSet();
                //dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                return FillItemGrid(false);//0, dtt, Codes, dss, Convert.ToInt32(GeneralCtgryID), CatCodes, 0, UnitCodes, ""
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        /// <summary>
        /// Add new row in a Item grid
        /// </summary>
        /// <param name="rowNo">Selected Row No</param>
        /// <param name="SNo"></param>
        /// <param name="CodeID">Item ID</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemColumn(int rowNo, int SNo, int CodeID)
        {
            try
            {
                if (rowNo == 0 && SNo == 0 && CodeID == 0)
                {
                    return FillGridItems(Guid.Empty);
                }
                else
                {
                    DataTable dt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                    DataRow dr = dt.NewRow();
                    dr["SNo"] = Convert.ToInt32(dt.Rows[rowNo - 1]["SNo"]) + 1;// rowNo + 1;
                    dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dr["ItemId"] = Guid.Empty;
                    dr["PartNumber"] = string.Empty;
                    dr["Specifications"] = string.Empty;
                    dr["Make"] = string.Empty;
                    dr["Quantity"] = 0;
                    dr["UNumsId"] = 0;
                    dr["ItemDetailsId"] = 0;
                    dt.Rows.Add(dr);

                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                    HttpContext.Current.Session["SelectedItems"] = Codes;
                    HttpContext.Current.Session["VerbalLPOCust"] = dt;
                    int count = dt.Rows.Count;
                    return FillItemGrid(false);//rowNo, SNo, CodeID, 1, 0, 0, "", "", "", "", ""
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Fill Item Description
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string FillSpec_ItemDesc(string ItemID)
        {
            try
            {
                //string Rslt = "";
                //Guid ItemIDInt = new Guid(ItemID.Trim());
                //DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                //if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                //{
                //    DataRow[] result = dss.Tables[0].Select("ID = " + "'" + ItemIDInt + "'");
                //    foreach (DataRow dr in result)
                //    {
                //        Rslt = dr["PartNumber"].ToString() + " &@&" + dr["Specification"].ToString();
                //    }
                //}
                //return Rslt;

                string Rslt = "";
                Guid ItemIDInt = new Guid(ItemID.Trim());
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] result = dss.Tables[0].Select("ID = " + "'" + ItemIDInt + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt = dr["ID"].ToString() + "&@&" + dr["ItemDescription"].ToString() + "&@&" + dr["PartNumber"].ToString() + " &@&" + dr["Specification"].ToString() + " &@& " + Convert.ToBoolean(dr["IsSubItems"]);
                    }
                }
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Edit the selected Item Row 
        /// </summary>
        /// <param name="SelID"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string EditItemRow(string SelID, string ItemID)
        {
            try
            {
                string Rslt = "";
                DataTable dtt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                if (dtt != null && dtt.Rows.Count > 0 && dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dsRow = dss.Tables[0].Select("ID = " + "'" + ItemID + "'");
                    foreach (DataRow drr in dsRow)
                    {
                        //Rslt = drr["ItemDetailsId"].ToString() + " &@&" + drr["ItemId"].ToString() + " &@&";
                        Rslt = drr["ID"].ToString() + " &@&" + drr["ItemDescription"].ToString() + " &@&";
                    }

                    DataRow[] result = dtt.Select("ItemId = " + "'" + ItemID + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt += dr["PartNumber"].ToString() + " &@&"
                              + dr["Specifications"].ToString() + " &@&"
                              + dr["Make"].ToString() + " &@&"
                              + dr["Quantity"].ToString() + " &@&"
                              + dr["UNumsId"].ToString() + " &@&"
                              + dr["Rate"].ToString() + " &@&"
                              + dr["Remarks"].ToString();
                    }
                }
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to update the Items Selected
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ItemID"></param>
        /// <param name="PartNumber"></param>
        /// <param name="Spec"></param>
        /// <param name="Make"></param>
        /// <param name="Qty"></param>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string UpdateSelectedItem(string value, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID, string Rate, string Remarks)
        {
            try
            {
                string Rslt = "";
                DataTable dt = (DataTable)HttpContext.Current.Session["VerbalLPOCust"];
                string[] val = value.Split(',');
                if (dt != null && dt.Rows.Count > 0 && val.Length > 0)
                {
                    int rowNo = Convert.ToInt32(val[0]) - 1;
                    dt.Rows[rowNo]["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dt.Rows[rowNo]["ItemId"] = ItemID;
                    dt.Rows[rowNo]["PartNumber"] = PartNo;
                    dt.Rows[rowNo]["Specifications"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                        dt.Rows[rowNo]["Quantity"] = 0;
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["UNumsId"] = UnitID;

                    if (Rate != "" && Rate != "0" && Convert.ToDecimal(Rate) > 0)
                    {
                        dt.Rows[rowNo]["Rate"] = Convert.ToDecimal(Rate);
                        dt.Rows[rowNo]["Amount"] = Convert.ToDecimal(Rate) * Convert.ToDecimal(Qty);
                    }
                    else
                    {
                        dt.Rows[rowNo]["Rate"] = 0;
                        dt.Rows[rowNo]["Amount"] = 0;
                    }
                    dt.Rows[rowNo]["Remarks"] = Remarks;


                    dt.AcceptChanges();
                    Session["VerbalLPOCust"] = dt;
                }
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Add New Item 
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string NewItemAdded()
        {
            try
            {
                string Rslt = "";
                Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Check the Enquiry Number
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckEnquiryNo(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckEnquiryNo('E', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string ValidateItems()
        {
            try
            {
                bool HadError = false;
                string Msg = "";
                DataTable dt = (DataTable)Session["VerbalLPOCust"];
                if (dt == null || dt.Rows.Count == 0)
                {
                    HadError = true;
                    Msg = "No Rows to Save.";
                }
                else if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["ItemId"].ToString() == "" || new Guid(dt.Rows[i]["ItemId"].ToString()) == Guid.Empty)
                        {
                            Msg = "Item Description is required in row no " + (i + 1) + ".";
                            HadError = true;
                        }
                        if (dt.Rows[i]["Quantity"].ToString() == "" || Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) == 0)
                        {
                            Msg = "Quantity is required in row no " + (i + 1) + ".";
                            HadError = true;
                        }
                        if (dt.Rows[i]["UNumsId"].ToString() == "" || new Guid(dt.Rows[i]["UNumsId"].ToString()) == Guid.Empty)
                        {
                            Msg = "Units is required in row no " + (i + 1) + ".";
                            HadError = true;
                        }
                        if (dt.Rows[i]["Rate"].ToString() == "" || Convert.ToDecimal(dt.Rows[i]["Rate"].ToString()) == 0)
                        {
                            Msg = "rate is required in row no " + (i + 1) + ".";
                            HadError = true;
                        }
                    }
                }
                if (HadError)
                    Msg = "ERROR::" + Msg;
                return Msg;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Customer Access/ErrorLog"), "VerbalLPO_Customer", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }
        }

        # endregion
    }
}