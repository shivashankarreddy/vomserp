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

namespace VOMS_ERP.Purchases
{
    public partial class NewLPOrderVerbal : System.Web.UI.Page
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewLPOrderVerbal));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            HideUnwantedFields();
                            GetData();
                            HFID.Value = Guid.Empty.ToString();
                            Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            //if ((string[])Session["UsrPermissions"] != null &&
                            //    ((string[])Session["UsrPermissions"]).Contains("Edit") && Request.QueryString["ID"] != null)
                            //{
                            if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                            {
                                DivComments.Visible = true;
                                HFID.Value = Request.QueryString["ID"].ToString();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                Guid FPOID = new Guid(ListBoxFPO.SelectedValue);
                string FPOIDs = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Guid LPOID = Guid.Empty;
                if (Request.QueryString["ID"] != null)
                    LPOID = new Guid(Request.QueryString["ID"]);
                ItemStatusBLL ISBLL = new ItemStatusBLL();
                ds = ISBLL.GetItemStatus(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, LPOID, CommonBLL.StatusTypeFPOrder, "", "", "", "", FPOIDs, "");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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

                if (Request.QueryString["ID"] != null && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(Request.QueryString["CustID"].ToString()),
                        Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "",
                        Guid.Empty, "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));
                    ddlCustomer.Enabled = false;
                }

                BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (Request.QueryString["FpoID"] != null && Request.QueryString["FpoID"].ToString() != "" && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    string fpono = Request.QueryString["FpoID"];
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagISelect, new Guid(fpono), "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                        Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                        "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));
                    string[] FPOLength = Request.QueryString["FpoID"].Split(',');
                    for (int i = 0; i < FPOLength.Length; i++)
                    {
                        ListItem item = ListBoxFPO.Items.FindByValue(FPOLength[i].ToString());
                        if (item != null)
                        {
                            ListBoxFPO.SelectedValue = FPOLength[i].ToString();
                        }
                    }
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
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                EditDS = NLPOBL.GetDataSetLPO_Verbal(CommonBLL.FlagModify, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, "",
                                                "Subject", Guid.Empty, "", false, false, false, 0, 0, 0, Guid.Empty, "",
                                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()),
                                                CommonBLL.EmptyDtLPOrdersVerbal(), CommonBLL.FirstRowPaymentTerms(),
                                                CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                    && EditDS.Tables[2].Rows.Count > 0)
                {
                    string FpoNo = EditDS.Tables[0].Rows[0]["ForeignPurchaseOrderId"].ToString();
                    string CustID = EditDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlCustomer.SelectedValue = CustID;

                    BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                    BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagZSelect, Guid.Empty, FpoNo, new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                        Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                        "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));

                    hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["FPODate"].ToString()));
                    string[] fponmbrs = FpoNo.Split(',');
                    foreach (ListItem item in ListBoxFPO.Items)
                    {
                        foreach (string s in fponmbrs)
                        {
                            if (item.Value == s)
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    BindDropDownList(ddlSuplr, sbll.SelectSuppliersForBind(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ddlSuplr.SelectedValue = EditDS.Tables[0].Rows[0]["SupplierID"].ToString();

                    if (EditDS.Tables.Count > 3)
                    {
                        ViewState["Quantity"] = EditDS.Tables[3];
                    }
                    string FPOSession = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    Session["FPOSelected"] = FPOSession;
                    FillInputFields(FpoNo, CustID);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(EditDS.Tables[1].Copy());
                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    ds.Tables[0].Columns.Add(dc);
                    Session["VLPOFloatEnquiry"] = ds;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CalcexDuty", "CalculateExDuty();", true);
                    divLPOItems.InnerHtml = FillGridView(ds, 0, true);

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
                    var LineItemGST = EditDS.Tables[1].AsEnumerable().Where(R => R.Field<decimal>("ExDutyPercentage") > Decimal.Zero).ToList();
                     if (LineItemGST.Count <= 0)
                     {
                         if (EditDS.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00" || EditDS.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != "0.00")
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
                         if (EditDS.Tables[0].Rows[0]["SGST"].ToString() != "0.00" || EditDS.Tables[0].Rows[0]["SGSTPercentage"].ToString() != "0.00")
                         {
                             if (EditDS.Tables[0].Rows[0]["SGST"].ToString() != "0.00")
                             {
                                 txtSGST.Text = EditDS.Tables[0].Rows[0]["SGST"].ToString();
                                 rbtnSGST.SelectedValue = "1";
                             }
                             else if (EditDS.Tables[0].Rows[0]["SGSTPercentage"].ToString() != "0.00")
                             {
                                 txtSGST.Text = EditDS.Tables[0].Rows[0]["SGSTPercentage"].ToString();
                                 rbtnSGST.SelectedValue = "0";
                             }
                             chkSGST.Checked = true;
                             ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSGSTE", "CHeck('chkSGST', 'dvSGST');", true);
                         }
                         if (EditDS.Tables[0].Rows[0]["IGST"].ToString() != "0.00" || EditDS.Tables[0].Rows[0]["IGSTPercentage"].ToString() != "0.00")
                         {
                             if (EditDS.Tables[0].Rows[0]["IGST"].ToString() != "0.00")
                             {
                                 txtIGST.Text = EditDS.Tables[0].Rows[0]["IGST"].ToString();
                                 rbtnIGST.SelectedValue = "1";
                             }
                             else if (EditDS.Tables[0].Rows[0]["IGSTPercentage"].ToString() != "0.00")
                             {
                                 txtIGST.Text = EditDS.Tables[0].Rows[0]["IGSTPercentage"].ToString();
                                 rbtnIGST.SelectedValue = "0";
                             }
                             chkIGST.Checked = true;
                             ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowIGSTE", "CHeck('chkIGST','dvIGST');", true);
                         }
                     }
                     else
                     {
                         chkExdt.Checked = false;
                         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", true);
                         chkSGST.Checked = false;
                         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSGSTE", "CHeck('chkSGST', 'dvSGST');", true);
                         chkIGST.Checked = false;
                         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowIGSTE", "CHeck('chkIGST','dvIGST');", true);
                         chkExdt.Enabled = false;
                         chkSGST.Enabled = false;
                         chkIGST.Enabled = false;
                     }
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
                    ListBoxFPO.Enabled = false;
                    ddlSuplr.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                    string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    DataSet CrntIdnt = NLPOBL.SelectLPOrders(CommonBLL.FlagISelect, Guid.Empty, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                    DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                    CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                    txtLpono.Text = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" +
                    ListBoxFPO.SelectedItem.Text.Trim() + "/" + CommonBLL.FinacialYearShort;

                    //Dinesh Check
                    //string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    Guid CompanyID = new Guid(HttpContext.Current.Session["CompanyID"].ToString());
                    Guid CustomerID = new Guid(FPOrderDeatils.Tables[0].Rows[0][0].ToString());
                    DataSet ds = IDBLL.SelectItemDtlsLPOVerbal(CommonBLL.FlagHSelect, fpono, CompanyID, CustomerID);
                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    ds.Tables[0].Columns.Add(dc);
                    if (ds.Tables[0].Columns.Contains("Rate"))
                        ds.Tables[0].Columns.Remove("Rate");
                    DataColumn dc1 = new DataColumn("Rate", typeof(decimal));
                    dc1.DefaultValue = 0;
                    ds.Tables[0].Columns.Add(dc1);
                    ds.Tables[0].AcceptChanges();
                    Session["VLPOFloatEnquiry"] = ds;
                    divLPOItems.InnerHtml = FillGridView(ds, 0, false);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ListBoxFPO.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlSuplr.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                txtDlvry.Text = txtimpinst.Text = txtLpoDt.Text = txtLpoDueDt.Text = txtLpono.Text = txtsubject.Text = "";
                ChkbInspcn.Checked = ChkbCEEApl.Checked = false;
                txtDsnt.Text = txtExdt.Text = txtPkng.Text = txtSltx.Text = "0";
                txtLpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(3));
                ListBoxFPO.Enabled = true;
                ddlSuplr.Enabled = true;
                ViewState["EditID"] = null;
                divLPOItems.InnerHtml = "";
                Session["MessageLPO"] = null;
                Session["amountLPO"] = null;
                Session["PaymentTermsLPO"] = null;
                Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                Session["LPOUploads"] = null;
                ListBoxFPO.Items.Clear();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods Items

        private string FillGridView(DataSet ds, decimal Gtotl, Boolean RateSve)
        {
            try
            {

                //string FOP = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //DataSet ds = new DataSet();
                //if (ds == null)
                ds = (DataSet)Session["VLPOFloatEnquiry"];
                string ItemIDs = string.Empty;
                FlagDiscount = 0;
                FlagExDuty = 0;
                if (RateSve == false)
                {
                    string fpno = "";
                    if (Session["FPOSelected"] != null)
                        fpno = Session["FPOSelected"].ToString();
                    else
                        fpno = ds.Tables[0].Rows[0]["ForeignPOID"].ToString();

                    //DataSet LclQuoteItems = NLQBL.LPOItemsByMulti(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty.ToString(), Guid.Empty, Guid.Empty,
                    //    Guid.Empty, Guid.Empty, new Guid(ddlSuplr.SelectedValue), "", 0, "", Guid.Empty, CommonBLL.EmptyDtLocalQuotation(),
                    //    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), Guid.Empty, fpno, new Guid(Session["CompanyID"].ToString()));
                    int DiscChkBox = 0;
                    int ExDutyChkBox = 0;
                    double ExPercent = 0;
                    if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                        ExPercent = Convert.ToDouble(ds.Tables[0].Rows[0]["ExPercent"].ToString());
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //if (LclQuoteItems.Tables[4].Rows[i]["CQuantity"].ToString() != ""
                            //    && ds.Tables[0].Rows[i]["ItemId"].ToString() == LclQuoteItems.Tables[4].Rows[i]["ItemId"].ToString()
                            //    && Convert.ToDecimal(LclQuoteItems.Tables[4].Rows[i]["Quantity"].ToString()) >= Convert.ToDecimal(LclQuoteItems.Tables[4].Rows[i]["CQuantity"].ToString()))
                            //{
                            //    ds.Tables[0].Rows[i]["Quantity"] = Convert.ToInt16(ds.Tables[0].Rows[i]["Quantity"]) -
                            //        Convert.ToInt16(LclQuoteItems.Tables[4].Rows[i]["CQuantity"]);
                            ds.Tables[0].Rows[i]["ExDutyPercentage"] = 0.1;
                            ds.Tables[0].Rows[i]["DiscountPercentage"] = "0.00";
                            //ds.Tables[0].Rows[i]["GrandTotal"] = "0.00";
                            //}
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0)
                                ExDutyChkBox = 1;
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0)
                                DiscChkBox = 1;
                            if (ds.Tables[0].Rows[i]["Quantity"].ToString() == "0")
                                ds.Tables[0].Rows[i]["Check"] = false;
                        }

                    }
                    if (DiscChkBox == 0)
                        chkDsnt.Enabled = true;
                    else
                    {
                        chkDsnt.Checked = false;
                        chkDsnt.Enabled = false;
                    }
                    if (ExDutyChkBox == 0)
                    {
                        chkExdt.Enabled = true;
                        chkSGST.Enabled = true;
                        chkIGST.Enabled = true;
                    }
                    else
                    {
                        chkExdt.Checked = false;
                        chkExdt.Enabled = false;
                        chkSGST.Checked = false;
                        chkSGST.Enabled = false;
                        chkIGST.Enabled = false;
                        chkIGST.Checked = false;
                    }
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataSet dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    DataSet dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                    " id='tblItems'><thead align='left'><tr>");
                    if (Session["VLPOCheckAllBoxes"] != null && Convert.ToBoolean(Session["VLPOCheckAllBoxes"]) == false)
                        sb.Append("<tr><th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()'/></th>");
                    else
                        sb.Append("<tr><th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()' " +
                            " checked='checked'/></th>");
                    //if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                    //    sb.Append("<th>SNo</th><th align='center'>Item Description</th>" +
                    //        "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
                    //        "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
                    //        "<th align='center'>Amount</th>" +
                    //        "<th align='center'>Discount</th><th align='center'>Net Rate</th>"
                    //        + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
                    //else
                    sb.Append("<th>SNo</th><th align='center'>Item Description</th>" +
                    "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
                    "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
                    "<th align='center'>Amount</th>" +
                    "<th align='center'>GST</th><th align='center'>Discount</th><th align='center'>Net Rate</th>"
                    + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
                    sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        decimal TotalAmount = 0;
                        decimal GrandTotal = 0;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string sno = (i + 1).ToString();
                            sb.Append("<tr valign='Top'>");
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
                            {
                                TotalAmount += Convert.ToDecimal(ds.Tables[0].Rows[i]["AmountC"].ToString());
                                Gtotl += Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString());
                                sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                        + sno + ")' type='checkbox' checked='checked' name='CheckAll'/></td>");
                            }
                            else
                                sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                    + sno + ")' type='checkbox' name='CheckAll' /></td>");

                            sb.Append("<td align='center'>" + sno + " <input type='hidden' name='hfFESNo' id='hfFESNo" + sno + "' value='" + sno + "' /></td>");//S.NO
                            sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + (i + 1).ToString() +
                                ")' id='HItmID" + (i + 1).ToString() + "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString()
                                + "' width='5px' style='WIDTH: 5px;' class='bcAsptextbox'/></td>");

                            sb.Append("<td valign='Top' width='200px'><div class='expanderR'><span id='lbldescip" + sno + "'>" +
                                ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</span></div></td>");

                            sb.Append("<td><span id='lblpartno" + sno + "'>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</span></td>");

                            sb.Append("<td><textarea name='txtSpecifications' id='txtSpecifications" + sno
                                        + "' onchange='FillItemsAll(" + sno + ")' Class='bcAsptextboxmulti' onfocus='ExpandTXT(" + sno
                                        + ")' onblur='ReSizeTXT(" + sno + ")' style='height:22px; width:150px; resize:none;'>"
                                        + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
                            sb.Append("<td><input type='text' name='txtMake' size='05px' onchange='FillItemsAll(" + sno
                                + ")' id='txtMake" + sno + "' value='"
                                + ds.Tables[0].Rows[i]["Make"].ToString() + "' style='width:50px;' class='bcAsptextbox'/></td>");
                            sb.Append("<td><input type='text' name='txtQuantity'  size='05px' onchange='FillItemsAll("
                            + sno + ")' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString()
                                + "' onkeypress='return blockNonNumbers(this, event, false, false);' " +
                                " maxlength='6' style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");

                            sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");

                            sb.Append("<td><input type='text' size='05px' name='txtPrice' id='txtPrice" + sno
                            + "' onfocus='this.select()' onMouseUp='return false' value='"
                                + ds.Tables[0].Rows[i]["Rate"].ToString() + "' onchange='CalculateTotalAmount(" + sno
                                + ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' " +
                                " onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                " style='text-align: right; width:50px;' class='bcAsptextbox'/> <input type='hidden'  name='HFStatus' id='HFStatus" + sno
                           + "' value='" + ds.Tables[0].Rows[i]["Check"].ToString() + "'style='text-align: right; width:50px;'/> </td>");

                            //sb.Append("<td></td>");
                            sb.Append("<td align='right'><span id='spnAmount" + sno + "'>"
                            + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["AmountC"].ToString()), 2) + "</span></td>");

                            sb.Append("<td><input type='text' size='05px' id='txtPercent" + sno +
                               "' onfocus='this.select()' onMouseUp='return false' value='"
                                   + ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString() + "' onchange='FillItemsAll(" + sno
                                   + ")' onblur='extractNumber(this,2,false);' onkeyup='CheckExDuty(" + sno
                                   + "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                   " maxlength='18' style='text-align: right; width: 50px;' class='bcAsptextbox'/>%</td>");


                            sb.Append("<td><input type='text' size='05px' name='txtDiscount' id='txtDiscount" + sno
                                + "' onfocus='this.select()' onMouseUp='return false' value='"
                                + ds.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "' onchange='FillItemsAll(" + sno +
                                ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='CheckDiscount(" + sno +
                                "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                "  style='text-align: right; width:50px;' class='bcAsptextbox'/>%</td>");
                            //if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                            //    sb.Append("<td></td>");
                            //else
                            //{

                            //}
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0)
                                FlagExDuty = 1;
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0)
                                FlagDiscount = 1;
                            sb.Append("<td align='right'><span id='spnNetRate" + sno + "'>"
                                + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["QPriceC"].ToString()), 2)
                            + "&nbsp;</span></td>");
                            if (ds.Tables[0].Rows[i]["GrandTotal"].ToString() != "")
                                sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>"
                                    + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString()), 2) + "</span></td>");
                            else
                                sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>0.00</span></td>");

                            sb.Append("<td><input type='text' name='txtRemarks' onchange='FillItemsAll(" + sno +
                                ")' id='txtRemarks" + sno + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString() +
                                "' style='width:60px' class='bcAsptextbox'/></td>");
                            sb.Append("<td></td>");
                            sb.Append("<td></td>");
                            sb.Append("</tr>");
                        }

                        if (Gtotl != 0)
                            GrandTotal = Convert.ToDecimal(Gtotl);

                        sb.Append("<tfoot>");
                        sb.Append("<tr class='bcGridViewHeaderStyle'>");
                        sb.Append("<th colspan='5' class='rounded-foot-left'><b><span></span></b></th>");
                        sb.Append("<th align='right'> <input type='hidden' name='HFExDuty' id='HFExDuty' value='" + FlagExDuty + "'/> </th>");
                        sb.Append("<th align='right'> <input type='hidden' name='HFDiscount' id='HFDiscount' value='"
                            + FlagDiscount + "'/> </th>");
                        sb.Append("<th colspan='4' align='right'><b><span>Total Amount : <label id='lblTotalAmt'>"
                            + Math.Round(Convert.ToDecimal(TotalAmount), 2) + "</label></span></b></th>");
                        sb.Append("<th colspan='4' align='right'><b><span>Grand Total : <label id='lblGTAmt'> "
                            + Math.Round(Convert.ToDecimal(GrandTotal), 2) + "</label></span></b></th>");
                        sb.Append("<th colspan='4' class='rounded-foot-right' ><b><span></span></b></th>");
                        sb.Append("</tr></tfoot>");
                    }
                    sb.Append("</tbody></table>");
                    Session["sb"] = sb.ToString();
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int Lno = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return ex.Message;
            }
        }

        #endregion

        #region DropDownLists Selected Index Changed Events

        /// <summary>
        /// FP Order Number Select Index changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFpoNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Session["FPOSelected"] = fpono;
                BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                ddlSuplr.Items.Clear();
                BindDropDownList(ddlSuplr, sbll.SelectSuppliersForBind(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                FillInputFields(fpono, ddlCustomer.SelectedValue);

                Guid CompanyID = new Guid(HttpContext.Current.Session["CompanyID"].ToString());

                DataSet ds = IDBLL.SelectItemDtlsLPOVerbal(CommonBLL.FlagHSelect, fpono, CompanyID, new Guid(ddlCustomer.SelectedValue));
                DataColumn dc = new DataColumn("Check", typeof(bool));
                dc.DefaultValue = true;
                ds.Tables[0].Columns.Add(dc);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    if (item["Status"].ToString() == Convert.ToString(50))
                        item["Check"] = true;
                    else
                        item["Check"] = false;
                }
                if (ds.Tables[0].Columns.Contains("Rate"))
                    ds.Tables[0].Columns.Remove("Rate");
                DataColumn dc1 = new DataColumn("Rate", typeof(decimal));
                dc1.DefaultValue = 0;
                ds.Tables[0].Columns.Add(dc1);
                ds.Tables[0].AcceptChanges();

                Session["VLPOFloatEnquiry"] = ds;
                divLPOItems.InnerHtml = FillGridView(ds, 0, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
            }
        }

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                    string feno1 = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
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
                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagISelect, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                        Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                        "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));
                }
                else
                {
                    ClearAll();
                    ddlSuplr.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                    decimal exdt = 0, sgst = 0, igst = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, sgstPrcnt = 0, igstPrcnt = 0, sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0;
                    DateTime lpoDate = CommonBLL.DateInsert(txtLpoDt.Text);
                    DateTime lpoDueDate = CommonBLL.DateInsert(txtLpoDueDt.Text);
                    string LPSN = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());
                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    if (TCs.Columns.Contains("Title"))
                        TCs.Columns.Remove("Title");
                    if (TCs.Columns.Contains("CompanyId"))
                        TCs.Columns.Remove("CompanyId");

                    DataSet DS = (DataSet)Session["VLPOFloatEnquiry"];
                    DataRow[] rows = DS.Tables[0].Select("Check = 'false'");
                    foreach (DataRow r in rows)
                        DS.Tables[0].Rows.Remove(r);
                    DS.Tables[0].AcceptChanges();

                    DataTable dtt = DS.Tables[0].DefaultView.ToTable(false,
                        "FESNo", "ItemId", "ForeignPOId", "PartNumber", "Specifications", "Make", "Rate", "QPriceC", "Quantity",
                        "Amount", "ExDutyPercentage", "DiscountPercentage", "UNumsId", "Remarks");
                    dtt.Columns["FESNo"].ColumnName = "SNo";
                    dtt.Columns["ForeignPOId"].ColumnName = "FPOID";
                    dtt.Columns["QPriceC"].ColumnName = "QPrice";


                    DataTable dtbl = dtt;
                    DataTable sdt = (DataTable)Session["PaymentTermsLPO"];

                    if (sdt.Columns.Contains("CompanyId"))
                        sdt.Columns.Remove("CompanyId");
                    if (dtbl.Rows.Count > 0)
                    {
                        string Attachments = "";
                        if (Session["LPOUploads"] != null)
                        {
                            ArrayList all = (ArrayList)Session["LPOUploads"];
                            Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                        }

                        if (btnSave.Text == "Save" && dtbl.Rows.Count > 0 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True")
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    sgstPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    sgst = Convert.ToDecimal(txtSGST.Text);
                                else sgst = Convert.ToDecimal(txtSGST.Text);

                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    igstPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    igst = Convert.ToDecimal(txtIGST.Text);
                                else igst = Convert.ToDecimal(txtIGST.Text);

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
                            string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + LpoNum + "/" +
                            LPSN + "/" + CommonBLL.FinacialYearShort + "/Amend/" + CrntIdnt.Tables[1].Rows[0]["HistoryNum"].ToString();


                            string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            //string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, fponum, "", Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, sgst, sgstPrcnt, igst, igstPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order Verbal",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }

                        else if (btnSave.Text == "Save" && dtbl.Rows.Count > 0)
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    sgstPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    sgst = Convert.ToDecimal(txtSGST.Text);
                                else sgst = Convert.ToDecimal(txtSGST.Text);

                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    igstPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    igst = Convert.ToDecimal(txtIGST.Text);
                                else igst = Convert.ToDecimal(txtIGST.Text);
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
                            string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/" +
                            LPSN + "/" + CommonBLL.FinacialYearShort;

                            string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            //string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, fponum, "", Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                "", txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, sgst, sgstPrcnt, igst, igstPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order Verbal",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }
                        else if (btnSave.Text == "Update" && dtbl.Rows.Count > 0)
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    sgstPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    sgst = Convert.ToDecimal(txtSGST.Text);
                                else sgst = Convert.ToDecimal(txtSGST.Text);

                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    igstPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    igst = Convert.ToDecimal(txtIGST.Text);
                                else igst = Convert.ToDecimal(txtIGST.Text);
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
                            string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                            res = NLPOBL.InsertUpdateDeleteLPO_Verbal(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                                Guid.Empty, fponum, "", Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                                txtLpono.Text, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue),
                                new Guid(ddlSuplr.SelectedValue), "", txtsubject.Text,
                                new Guid(ddlRsdby.SelectedValue), txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked,
                                ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder,
                                txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs,
                                exdt, exdtPrcnt, sgst, sgstPrcnt, igst, igstPrcnt, dscnt, dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()));
                            if (res == 0 && btnSave.Text == "Update")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order Verbal",
                                    "Data Updated Successfully.");
                                ClearAll(); Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Verbal Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Updating.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", "Error while Updating.");
                                FillPaymentTerms();
                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('There are No Items to Save/Update.');", true);
                    }
                    else if (dtbl.Rows.Count > 0)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                Response.Redirect("../Purchases/NewLPOrderVerbal.Aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                        int sno = 0;
                        try { sno = Convert.ToInt16(dt.Compute("Max(SNo)", string.Empty)); }
                        catch { }
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        if (dt.Rows[rowNo]["PaymentPercentage"].ToString() == "")
                        {
                            dt.Rows[rowNo]["PaymentPercentage"] = 0;
                            dt.Rows[rowNo]["SNo"] = (sno + 1);
                        }
                        else
                        {
                            int newCount = dt.Rows.Count;
                            dt.Rows[newCount - 1]["PaymentPercentage"] = 0;
                            dt.Rows[newCount - 1]["SNo"] = (sno + 1);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }
        }

        #endregion

        # region WebMethods Verbal

        /// <summary>
        /// To Fill Item Grid view
        /// </summary>
        /// <param name="rowNo">Selected Row No</param>
        /// <param name="SNo">row S.No.</param>
        /// <param name="CodeID">Item ID</param>
        /// <param name="ID"></param>
        /// <param name="CatID">Category ID</param>
        /// <param name="UnitID">Unit Id</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string FillItemGrid(int rowNo, int SNo, string CodeID, int ID, string CatID, string UnitID, string Spec, string Make,
            string Qty, string PartNo, double Rate, double Amount)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                DataSet dss = (DataSet)HttpContext.Current.Session["VLPOFloatEnquiry"];
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                if (CodeID_G != Guid.Empty)
                {
                    dss.Tables[0].Rows[rowNo]["Category"] = CatID_G;
                    dss.Tables[0].Rows[rowNo]["ItemDescription"] = CodeID_G;
                    dss.Tables[0].Rows[rowNo]["PartNo"] = PartNo;
                    dss.Tables[0].Rows[rowNo]["Specification"] = Spec;
                    dss.Tables[0].Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                        dss.Tables[0].Rows[rowNo]["Quantity"] = 0;
                    else
                        dss.Tables[0].Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dss.Tables[0].Rows[rowNo]["units"] = UnitID_G;
                    dss.Tables[0].Rows[rowNo]["Rate"] = Rate;
                    dss.Tables[0].Rows[rowNo]["Amount"] = Amount;
                }
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                Dictionary<int, Guid> CatCodes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedCat"];
                Dictionary<int, Guid> UnitCodes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedUnits"];

                if (!CatCodes.ContainsValue(CatID_G) && CatID_G != Guid.Empty)
                {
                    if (CatCodes.ContainsKey(SNo))
                        CatCodes[SNo] = CatID_G;
                    else
                        CatCodes.Add(SNo, CatID_G);
                }
                if (!Codes.ContainsValue(CodeID_G) && CodeID_G != Guid.Empty)
                {
                    if (Codes.ContainsKey(SNo))
                        Codes[SNo] = CodeID_G;
                    else
                        Codes.Add(SNo, CodeID_G);
                }

                if (!UnitCodes.ContainsValue(UnitID_G) && UnitID_G != Guid.Empty)
                {
                    if (UnitCodes.ContainsKey(SNo))
                        UnitCodes[SNo] = UnitID_G;
                    else
                        UnitCodes.Add(SNo, UnitID_G);
                }
                # region EmptyRow
                DataSet ds = new DataSet();
                ds = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ViewState["btnSaveID"] != null && ViewState["btnSaveID"].ToString() != "Update")
                {
                    if (dss.Tables[0].Rows.Count < ds.Tables[0].Rows.Count && (Codes.Count <= dss.Tables[0].Rows.Count))
                    {
                        if (CodeID_G != Guid.Empty && dss.Tables[0].Rows[dss.Tables[0].Rows.Count - 1]["ItemDescription"].ToString() != "0")
                        {
                            DataRow dr = dss.Tables[0].NewRow();
                            dr["SNo"] = dss.Tables[0].Rows.Count + 1;
                            dr["Category"] = 0;
                            dr["ItemDescription"] = 0;
                            dr["PartNo"] = string.Empty;
                            dr["Specification"] = string.Empty;
                            dr["Make"] = string.Empty;
                            dr["Quantity"] = 0;
                            dr["Units"] = 0;
                            dr["Rate"] = 0;
                            dr["Amount"] = 0;
                            dr["Remarks"] = string.Empty;
                            dr["ID"] = 0;
                            dss.Tables[0].Rows.Add(dr);
                        }
                        if (ID == 1)
                        {
                            if (dss.Tables[0].Rows[dss.Tables[0].Rows.Count - 1]["ItemDescription"].ToString() == string.Empty)
                            {
                                dss.Tables[0].Rows.RemoveAt(dss.Tables[0].Rows.Count - 1);
                            }
                        }
                    }
                }
                # endregion
                HttpContext.Current.Session["VLPOFloatEnquiry"] = dss;
                sb.Append(FillGridView(dss, 0, false));
                sb.Append("</tbody></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return FillGridView((DataSet)Session["VLPOFloatEnquiry"], 0, false);
            }
        }

        //[Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        //public string AddItemColumn(int rowNo, int SNo, int CodeID)
        //{
        //    try
        //    {
        //        DataTable dtbl = (DataTable)HttpContext.Current.Session["ItemCode"];
        //        int SLNo = dtbl.Rows.Count;
        //        DataTable dt = dtbl.Clone();
        //        DataRow dr = dt.NewRow();
        //        dr["SNo"] = SLNo;
        //        dr["Category"] = 0;
        //        dr["ItemDescription"] = 0;
        //        dr["PartNo"] = string.Empty;
        //        dr["Specification"] = string.Empty;
        //        dr["Make"] = string.Empty;
        //        dr["Quantity"] = 0;
        //        dr["Units"] = 0;
        //        dr["Rate"] = 0.00;
        //        dr["Amount"] = 0.00;
        //        dr["ID"] = 0;
        //        dt.Rows.Add(dr);


        //        Dictionary<int, int> Codes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedItems"];
        //        Dictionary<int, int> CatCodes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedCat"];
        //        Dictionary<int, int> UnitCodes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedUnits"];

        //        HttpContext.Current.Session["SelectedCat"] = CatCodes;
        //        HttpContext.Current.Session["SelectedItems"] = Codes;
        //        HttpContext.Current.Session["SelectedUnits"] = UnitCodes;
        //        HttpContext.Current.Session["ItemCode"] = dt;
        //        int count = dt.Rows.Count;
        //        return FillItemGrid(rowNo, SLNo, CodeID.ToString(), 1, "0", "0", "", "", "", "", 0, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
        //        return FillGridView((DataSet)Session["VLPOFloatEnquiry"], 0);
        //    }

        //}


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(int rowNo, int SNo, string CodeID, int ID, string CatID, string Spec, string Make, string Qty,
            double Rate, double Amount, string Rmrks, double Discount, string UnitID, double ExDuty, double CHKExDuty, double CHKDiscount,
            double CHKSalesTax, double CHKPacking, double CHKAdChrgs, bool ChkChaild, string Status)
        {
            try
            {

                DataSet dsEnm = new DataSet();
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                DataSet ds = (DataSet)Session["VLPOFloatEnquiry"];
                string Fpo = ds.Tables[0].Rows[0]["ForeignPOID"].ToString();
                DataSet dsForStatusCheck = NLPOBL.SelectLPOrders(CommonBLL.FlagVSelect, Guid.Empty, Guid.Empty, Fpo, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                                CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    double PackageAmt = 0;
                    double AmtAfterDiscount = 0;
                    double AdsnlChargesAmt = 0;
                    double AmtPcknAdsn = 0;
                    DataTable dt = ds.Tables[0];
                    CatID_G = Guid.Empty; // GeneralCtgryID;
                    if (!ChkChaild)
                        dt.Rows[rowNo]["Check"] = false;
                    else
                    {

                        if (Convert.ToInt16(ds.Tables[0].Rows[rowNo]["Status"].ToString()) < 60 && Qty != "0")
                        {
                            dt.Rows[rowNo]["Check"] = true; Status = "true";
                        }
                        else if (dt.Rows[rowNo]["Check"].ToString() != "True" && Qty != "0" && ChkChaild == true)
                        {
                            dt.Rows[rowNo]["Check"] = true; Status = "true";
                        }

                        else if (dt.Rows[rowNo]["Check"].ToString() == "True" && Qty != "0" && ChkChaild == true)
                        {
                            dt.Rows[rowNo]["Check"] = true; Status = "true";
                        }
                        else
                        {
                            Status = "false";
                        }
                        //else
                        //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShowwAll", "ErrorMessage('This Item was already raised in another VerbalLPO.');", true);

                    }
                    //if (dt.Rows[rowNo]["Check"].ToString() == "True")
                    //{
                    //    for (int i = 0; i < dsForStatusCheck.Tables[1].Rows.Count; i++)
                    //    {
                    //        if (dsForStatusCheck != null && dsForStatusCheck.Tables[0].Rows.Count > 0 &&
                    //            dsForStatusCheck.Tables[0].Rows[i]["ItemId"].ToString() == dt.Rows[rowNo]["ItemId"].ToString()
                    //            && dsForStatusCheck.Tables[0].Rows[i]["LPOQunatRaised"].ToString() == dsForStatusCheck.Tables[0].Rows[i]["FPOQuantEdited"].ToString())
                    //        {
                    //            //if ()
                    //            //&& Convert.ToInt32(dsForStatusCheck.Tables[0].Rows[rowNo]["Status"].ToString()) == 60 
                    //            dt.Rows[rowNo]["Check"] = false;
                    //        }
                    //    }
                    //}

                    if (Codes != null && !Codes.ContainsValue(CodeID_G) && CodeID_G != Guid.Empty)
                    {
                        if (Codes.ContainsKey(rowNo))
                            Codes[rowNo] = CodeID_G;
                        else
                            Codes.Add(rowNo, CodeID_G);
                        dt.Rows[rowNo]["ItemId"] = CodeID_G;
                    }
                    Session["SelectedItems"] = Codes;
                    dt.Rows[rowNo]["Specifications"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                    {
                        Qty = "0";
                        dt.Rows[rowNo]["Quantity"] = 0;
                    }
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["Rate"] = Convert.ToDouble(Rate);
                    if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                    {
                        if (UnitID_G != Guid.Empty)
                        {
                            DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + UnitID_G.ToString());
                            dt.Rows[rowNo]["UNumsId"] = UnitID_G;
                            dt.Rows[rowNo]["UnitName"] = selRow[0]["Description"].ToString();
                        }
                    }
                    dt.Rows[rowNo]["DiscountPercentage"] = Discount;
                    dt.Rows[rowNo]["ExDutyPercentage"] = ExDuty;
                    if (Discount > 0)
                        AmtAfterDiscount = Rate - ((Rate * Discount) / 100);
                    else
                        AmtAfterDiscount = Rate;

                    if (CHKPacking > 0)
                        PackageAmt = (AmtAfterDiscount * CHKPacking) / 100;

                    if (CHKAdChrgs > 0)
                        AdsnlChargesAmt = (AmtAfterDiscount * CHKAdChrgs) / 100;

                    AmtPcknAdsn = AmtAfterDiscount + PackageAmt + AdsnlChargesAmt;

                    if (ExDuty > 0)
                        AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExDuty) / 100);
                    else if (CHKExDuty > 0)
                        AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * CHKExDuty) / 100);

                    dt.Rows[rowNo]["Remarks"] = Rmrks;

                    ds.Tables[0].Rows[rowNo]["QPriceC"] = AmtPcknAdsn;
                    ds.Tables[0].Rows[rowNo]["GrandTotal"] = (AmtPcknAdsn * Convert.ToDouble(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()));
                    ds.Tables[0].Rows[rowNo]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Rate"].ToString()) *
                        Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()), 2));
                    ds.Tables[0].Rows[rowNo]["AmountC"] = Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Rate"].ToString()) *
                        Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()), 2);
                    Session["VLPOFloatEnquiry"] = ds;
                    return FillGridView(ds, 0, true);
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return FillGridView((DataSet)Session["VLPOFloatEnquiry"], 0, false);
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SaveChangesWhileRateChange(int rowNo, int SNo, string CodeID, int ID, string CatID, string Spec, string Make, string Qty,
            double Rate, double Amount, string Rmrks, double Discount, string UnitID, double ExDuty, double CHKExDuty, double CHKDiscount,
            double CHKSalesTax, double CHKPacking, double CHKAdChrgs, bool ChkChaild)
        {
            try
            {
                DataSet dsEnm = new DataSet();
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                DataSet ds = (DataSet)Session["VLPOFloatEnquiry"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    double PackageAmt = 0;
                    double AmtAfterDiscount = 0;
                    double AdsnlChargesAmt = 0;
                    double AmtPcknAdsn = 0;
                    DataTable dt = ds.Tables[0];

                    CatID_G = Guid.Empty;   //GeneralCtgryID;
                    //if (!ChkChaild)
                    //    dt.Rows[rowNo]["Check"] = false;
                    //else
                    //    dt.Rows[rowNo]["Check"] = true;

                    if (!Codes.ContainsValue(CodeID_G) && CodeID_G != Guid.Empty)
                    {
                        if (Codes.ContainsKey(rowNo))
                            Codes[rowNo] = new Guid(CodeID);
                        else
                            Codes.Add(rowNo, new Guid(CodeID));
                        dt.Rows[rowNo]["ItemId"] = CodeID;
                    }
                    Session["SelectedItems"] = Codes;
                    dt.Rows[rowNo]["Specifications"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                    {
                        Qty = "0";
                        dt.Rows[rowNo]["Quantity"] = 0;
                    }
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["Rate"] = Convert.ToDouble(Rate);
                    if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                    {
                        if (UnitID_G != Guid.Empty)
                        {
                            DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + UnitID_G.ToString());
                            dt.Rows[rowNo]["UNumsId"] = UnitID_G;
                            dt.Rows[rowNo]["UnitName"] = selRow[0]["Description"].ToString();
                        }
                    }
                    dt.Rows[rowNo]["DiscountPercentage"] = Discount;
                    dt.Rows[rowNo]["ExDutyPercentage"] = ExDuty;
                    if (Discount > 0)
                        AmtAfterDiscount = Rate - ((Rate * Discount) / 100);
                    else if (CHKDiscount > 0)
                        AmtAfterDiscount = Rate - ((Rate * CHKDiscount) / 100);
                    else
                        AmtAfterDiscount = Rate;

                    if (CHKPacking > 0)
                        PackageAmt = (AmtAfterDiscount * CHKPacking) / 100;

                    if (CHKAdChrgs > 0)
                        AdsnlChargesAmt = (AmtAfterDiscount * CHKAdChrgs) / 100;

                    AmtPcknAdsn = AmtAfterDiscount + PackageAmt + AdsnlChargesAmt;

                    if (ExDuty > 0)
                        AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExDuty) / 100);
                    else if (CHKExDuty > 0)
                        AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * CHKExDuty) / 100);


                    dt.Rows[rowNo]["Remarks"] = Rmrks;
                    dt.Rows[rowNo]["LocalEnquireId"] = Guid.Empty.ToString();
                    ds.Tables[0].Rows[rowNo]["QPriceC"] = String.Format("{0:0.00}", Rate);
                    ds.Tables[0].Rows[rowNo]["GrandTotal"] = String.Format("{0:0.00}", Math.Round((Convert.ToDouble(Rate) * Convert.ToDouble(ds.Tables[0].Rows[rowNo]["Quantity"].ToString())), 2));
                    ds.Tables[0].Rows[rowNo]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Rate"].ToString()) *
                        Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()), 2));
                    Session["VLPOFloatEnquiry"] = ds;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return false;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CalculateExDuty(string ExDuty, string SGST, string IGST, string Discount, string Packing, string SalesTax, string AdsnlChrgs, bool chkDsnt,
            bool chkExdt, bool chkSGST, bool chkIGST, bool chkSltx, bool chkPkng, bool chkAdsnlChrgs, string rdbDscnt, string rdbExDty,string rdbSGST, string rdbIGST, string rdbPkg, string rdbAdd)
        {
            try
            {
                #region Items
                decimal Gtotal = 0;
                DataSet ds = (DataSet)Session["VLPOFloatEnquiry"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal DiscountAmount = 0;
                    decimal Amount = 0;
                    decimal PackageAmt = 0;
                    decimal SalesTaxAmt = 0;
                    decimal ExAmt = 0;
                    decimal SGSTAmt = 0;
                    decimal IGSTAmt = 0;
                    decimal AdsnlChargesAmt = 0;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Amount = 0;
                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                        {
                            if (FlagDiscount == 0)
                            {
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    if (Amount == 0)
                                        Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]) - DiscountAmount;
                                }
                                else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]) - DiscountAmount;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                            }
                            else
                            {
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]) - DiscountAmount;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                            }

                            if (chkPkng != false && Convert.ToDecimal(Packing) > 0 && Convert.ToInt32(rdbPkg) == 0)
                            {
                                PackageAmt = (Amount * Convert.ToDecimal(Packing)) / 100;
                            }
                            if (chkAdsnlChrgs != false && Convert.ToDecimal(AdsnlChrgs) > 0 && Convert.ToInt32(rdbAdd) == 0)
                            {
                                AdsnlChargesAmt = (Amount * Convert.ToDecimal(AdsnlChrgs)) / 100;
                            }
                            Amount = Amount + PackageAmt + AdsnlChargesAmt;
                            if (FlagExDuty == 0)
                            {
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    if (Amount == 0)
                                        Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                                    ExAmt = (Amount * (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                }
                                else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                    ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                else if (Convert.ToInt32(rdbExDty) == 0)
                                    ExAmt = 0;

                                if (chkSGST != false && Convert.ToDecimal(SGST) > 0 && Convert.ToInt32(rdbSGST) == 0)
                                    SGSTAmt = (Amount * Convert.ToDecimal(SGST)) / 100;
                                else if (Convert.ToInt32(rdbSGST) == 0)
                                    SGSTAmt = 0;

                                if (chkIGST != false && Convert.ToDecimal(IGST) > 0 && Convert.ToInt32(rdbIGST) == 0)
                                    IGSTAmt = (Amount * Convert.ToDecimal(IGST)) / 100;
                                else if (Convert.ToInt32(rdbIGST) == 0)
                                    IGSTAmt = 0;
                            }
                            else
                                Amount = Convert.ToDecimal(Amount + ((Amount * Convert.ToDecimal(ds.Tables[0].Rows[i]
                                    ["ExDutyPercentage"].ToString()))
                                    / 100));

                            Amount = Amount + ExAmt + SGSTAmt + IGSTAmt;

                            if (Amount > 0)
                            {
                                ds.Tables[0].Rows[i]["QPriceC"] = Amount;
                                ds.Tables[0].Rows[i]["GrandTotal"] = (Amount * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString()));
                                ds.Tables[0].Rows[i]["Amount"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                            }
                            else
                            {
                                Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                                if (FlagExDuty == 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                        ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    else if (Convert.ToInt32(rdbExDty) == 0)
                                        ExAmt = 0;

                                    if (chkSGST != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbSGST) == 0)
                                        SGSTAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    else if (Convert.ToInt32(rdbSGST) == 0)
                                        SGSTAmt = 0;

                                    if (chkIGST != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbIGST) == 0)
                                        IGSTAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    else if (Convert.ToInt32(rdbIGST) == 0)
                                        IGSTAmt = 0;
                                }
                                Amount = Amount + ExAmt + SGSTAmt + IGSTAmt;
                                ds.Tables[0].Rows[i]["QPriceC"] = Amount;
                                ds.Tables[0].Rows[i]["GrandTotal"] = (Amount * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString()));
                                ds.Tables[0].Rows[i]["Amount"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                                Amount = 0;
                            }
                        }
                    }

                    ds.AcceptChanges();
                    Session["VLPOFloatEnquiry"] = ds;
                }

                #endregion

                return FillGridView(ds, Gtotal, true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order Verbal", ex.Message.ToString());
                return FillGridView((DataSet)Session["VLPOFloatEnquiry"], 0, true);
            }
        }

        #endregion
    }
}