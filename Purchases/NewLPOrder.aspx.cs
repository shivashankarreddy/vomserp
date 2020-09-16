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
    public partial class NewLPOrder : System.Web.UI.Page
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
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string PriceSymbol = "";
        int FlagDiscount = 0;
        int FlagExDuty = 0;
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewLPOrder));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            HideUnwantedFields();
                            #region Paging

                            //if (Session["RowsDisplay_FPO"] == null)
                            Session["RowsDisplay_LPO"] = 100;
                            //if (Session["CPage_FPO"] == null)
                            Session["CPage_LPO"] = 1;

                            #endregion
                            GetData();
                            HFID.Value = Guid.Empty.ToString();
                            Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            if ((string[])Session["UsrPermissions"] != null &&
                                ((string[])Session["UsrPermissions"]).Contains("Edit") && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    Session["REQID"] = Request.QueryString["ID"].ToString();
                                    DivComments.Visible = true;
                                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Text = "Save";
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no", false);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods (Data Binding)
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
                //Guid FPOID = new Guid(ListBoxFPO.SelectedValue);
                //Guid LQID = new Guid(ListBoxLPO.SelectedValue);
                //Guid FEID = new Guid(ListBoxFEO.SelectedValue);
                string FPOIDs = Session["FPOIDS"].ToString();
                //Session["FEIDS"] = FEIDS;
                //Session["LPOIDS"] = LPOIDS;//String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LQIDs = Session["LPOIDS"].ToString();
                string FEIDs = Session["FEIDS"].ToString();
                Guid LPOID = Guid.Empty;
                if (Session["REQID"] != null)
                    LPOID = new Guid(Session["REQID"].ToString());
                ItemStatusBLL ISBLL = new ItemStatusBLL();
                ds = ISBLL.GetItemStatus(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["ddlCustomer"].ToString()), LPOID, CommonBLL.StatusTypeFPOrder, FEIDs.Trim(','), "", LQIDs.Trim(','), "", FPOIDs.Trim(','), "");
                if (ds.Tables.Count == 3) //&& ds.Tables[2].Rows.Count == 0) //ds.Tables[2].Rows[0][0].ToString() == "")
                {
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
                    ViewState["IsCustomer"] = "false";
                    Session["IsCustomer"] = "false";
                }
                else
                {
                    ViewState["IsCustomer"] = "true";
                    Session["IsCustomer"] = "true";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                al.Clear();
                al.Add(-1);
            }
            return al;
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
                BindDropDownList(ddlCustomer, cusmr.SelectCustomers(CommonBLL.FlagXSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (Request.QueryString["ID"] != null && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(Request.QueryString["CustID"].ToString()),
                        Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "",
                        Guid.Empty, "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));
                    ddlCustomer.Enabled = false;
                    Session["ddlCustomer"] = Request.QueryString["ID"].ToString();
                }

                BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (Request.QueryString["FpoID"] != null && Request.QueryString["FpoID"].ToString() != "" && Request.QueryString["CustID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    Session["ddlCustomer"] = Request.QueryString["CustID"].ToString();
                    string fpono = Request.QueryString["FpoID"];
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagRegularDRP, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
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
                    LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                        Guid.Empty.ToString(), DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                        100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                        DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                    ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    ListBoxFEO.Enabled = false;
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagHSelect, Guid.Empty, Guid.Empty, Guid.Empty, fpono, DateTime.Now, DateTime.Now,
                        100, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));
                    FillInputFields(fpono, Request.QueryString["CustID"]);
                    GetSessionData();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                    //ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                }
                // else
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                HFID.Value = ID.ToString();
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                bool IsExciseDuty = false;
                DataSet EditDS = new DataSet();
                EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagASelect, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(),
                                                CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
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
                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
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
                            if (item.Value == s.ToLower().Trim())
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "",
                        Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, FpoNo,
                        "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 80, Guid.Empty, false, false, "",
                        new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO,
                        CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                    ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    string fenumber = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    if (EditDS.Tables.Count > 3)
                    {
                        Session["Quantity"] = EditDS.Tables[3];
                    }
                    BindDropDownList(ddlSuplr, (NFPOBL.Select(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, FpoNo, "", "", DateTime.Now,
                        DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 80, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()))).Tables[2]);
                    FillInputFields(FpoNo, CustID);
                    ddlSuplr.SelectedValue = EditDS.Tables[0].Rows[0]["SupplierId"].ToString();
                    LocalPurchaseOrder(ListBoxLPO, (NLPOBL.SelectLPOrders(CommonBLL.FlagODRP, Guid.Empty, Guid.Empty, FpoNo, fenumber, Guid.Empty,
                        Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, new Guid(EditDS.Tables[0].Rows[0]["SupplierId"].ToString()), "",
                        DateTime.Now, 0, 80, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "")));
                    ListBoxLPO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    if (EditDS.Tables[1].Columns.Contains("Quantity"))
                        EditDS.Tables[1].Columns["Quantity"].ColumnName = "qty";
                    //gvLpoItems.DataSource = EditDS.Tables[1];
                    //gvLpoItems.DataBind();
                    Session["LPOCheckAllBoxes"] = true;
                    Session["LPOGrid"] = EditDS;
                    //ViewState["Quantity"] = EditDS.Tables[4];
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "safafdsaf",
                            "CalculateExDuty();", true);
                    divLPOItems.InnerHtml = FillGridView();

                    if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                        //gvLpoItems.Columns[11].Visible = false;

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

                        if ((EditDS.Tables[0].Rows[0]["SGST"].ToString() != "0.00" ||
                            EditDS.Tables[0].Rows[0]["SGSTPercentage"].ToString() != "0.00") &&
                            (EditDS.Tables[0].Rows[0]["SGST"].ToString() != "" ||
                            EditDS.Tables[0].Rows[0]["SGSTPercentage"].ToString() != ""))
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
                        else
                            txtSGST.Text = "0";

                        if ((EditDS.Tables[0].Rows[0]["IGST"].ToString() != "0.00" ||
                            EditDS.Tables[0].Rows[0]["IGSTPercentage"].ToString() != "0.00") &&
                            (EditDS.Tables[0].Rows[0]["IGST"].ToString() != "" ||
                            EditDS.Tables[0].Rows[0]["IGSTPercentage"].ToString() != ""))
                        {
                            if (EditDS.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00")
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
                        else
                            txtIGST.Text = "0";

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
                    if (IsExciseDuty == true || Convert.ToDecimal(EditDS.Tables[0].Rows[0]["ExDutyPercentage1"].ToString()) > 0)
                    {
                        ChkbCEEApl.Enabled = false;
                        txtCEEApl.Enabled = false;
                    }
                    else
                    {
                        ChkbCEEApl.Enabled = true;
                        txtCEEApl.Enabled = true;
                    }

                    ListBoxFPO.Enabled = ListBoxFEO.Enabled = false;
                    ddlSuplr.Enabled = false;
                    ListBoxLPO.Enabled = false;
                    ViewState["EditID"] = ID;
                    if (Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True")
                    {
                        btnSave.Text = "Save";
                        Session["REQAMID"] = Request.QueryString["IsAm"];
                    }
                    else
                        btnSave.Text = "Update";
                }
                else
                {
                    ListBoxFPO.Enabled = false;
                    ddlSuplr.Enabled = false;
                    ListBoxLPO.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
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
                    ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    ddlPrcBsis.SelectedValue = FPOrderDeatils.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = FPOrderDeatils.Tables[0].Rows[0]["PriceBasisText"].ToString();
                    hdfldCstmr.Value = FPOrderDeatils.Tables[0].Rows[0]["CusmorId"].ToString();
                    hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(FPOrderDeatils.Tables[0].Rows[0]["FPODate"].ToString()));
                    txtLpoDueDt.Text = DateTime.Now.ToString("dd-MM-yyyy");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This is used to fill Grid 
        /// </summary>
        /// <returns></returns>
        public string FillGridView()
        {
            try
            {
                DataSet dsi = (DataSet)Session["LPOGrid"];
                int TotalAmt = 0; string ItemIDs = string.Empty;
                string Message = "";
                bool IsExciseDuty = false;
                GTotal = 0;
                #region Paging

                string DisablePrevious = " disabled ", DisableNext = " disabled ";
                int Rows2Display = Convert.ToInt32(Session["RowsDisplay_LPO"].ToString()), CurrentPage = Convert.ToInt32(Session["CPage_LPO"].ToString()), Rows2Skip = 0;
                if (dsi != null)
                {
                    int RowsCount = dsi.Tables[1].Rows.Count, PageCount = 0;

                    if (dsi.Tables.Count > 0 && dsi.Tables[1].Rows.Count > 0)
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
                        Session["CPage_LPO"] = CurrentPage;
                    }

                #endregion

                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblItems'>" +
                   "<thead align='left'><tr >");
                    //sb.Append("<th class='rounded-First'>&nbsp;</th>");
                    if (Session["LPOCheckAllBoxes"] != null && Convert.ToBoolean(Session["LPOCheckAllBoxes"]) == false)
                        sb.Append("<tr><th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()'/></th>");
                    else
                        sb.Append("<tr><th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()' " +
                            " checked='checked'/></th>");
                    sb.Append("<th>SNo</th><th>Item Description</th><th align='center'>Part No</th><th align='center'>Specification</th>" +
                    "<th align='center'>Make</th><th>Quantity</th><th>Unit Name</th><th align='right'>Rate</th><th align='right'>Amount</th>" +
                    "<th align='center'>GST(%)</th><th align='center'>Disc(%)</th><th align='center'>Net-Rate</th><th class='rounded-Last'>Total</th></tr></thead>");
                    sb.Append("<tbody class='bcGridViewMain'>");


                    #region Paging

                    DataSet ds = new DataSet();
                    DataTable dt = dsi.Tables[1].AsEnumerable().Skip(Rows2Skip).Take(Rows2Display).CopyToDataTable();
                    ds.Tables.Add(dt);

                    string Hflbltmnt = string.Empty;
                    string hflblTotl = string.Empty;
                    #endregion
                    decimal TotalAmount = 0; decimal Amount = 0;
                    string MaxQty = ""; decimal CQuantity = 0; string RcvdQty = ""; string HF_ActualQty = ""; string ExcisDt = ""; decimal discountAmt = 0;
                    string HfDisable = string.Empty;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //if (Convert.ToBoolean(ds.Tables[1].Rows[i]["Check"].ToString()))
                            TotalAmount += Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                            string sno = (Rows2Skip + i + 1).ToString();
                            sb.Append("<tr valign='top'>");
                            sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' type='checkbox' onclick='CheckIndividualBoxs(" + sno + ")' ");
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["IsCheck"]))
                            {
                                sb.Append(" checked='checked' ");
                            }
                            sb.Append(" name='CheckAll'/></td>");
                            sb.Append("<td align='center'>" + sno + " <input type='hidden' name='hfFESNo' id='hfFESNo" + sno + "' value='" + sno + "' /></td>");//S.NO
                            sb.Append("<td valign='Top' width='200px'><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString()
                                    + "</div></td>");//ItemDesc
                            ItemIDs = ds.Tables[0].Rows[i]["ItemID"].ToString();

                            sb.Append("<td>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</td>");//PartNo

                            sb.Append("<td>" + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</td>");
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["Make"].ToString() + "</td>");

                            DataTable Quantity = (DataTable)Session["Quantity"];
                            string txtQty = string.Empty;
                            txtQty = ds.Tables[0].Rows[i]["qty"].ToString();
                            DataRow[] Qty = Quantity.Select("ItemId = '" + ItemIDs + "'");
                            Decimal EditQty = Convert.ToDecimal(txtQty);
                            ExcisDt = ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString();
                            string hfrate = string.Empty;
                            if (Qty != null && Qty.Length > 0)
                            {
                                HF_ActualQty = Qty[0]["ActualQty"].ToString();
                                MaxQty = Qty[0]["Quantity"].ToString();
                                CQuantity = Convert.ToDecimal(Qty[0]["CQuantity"].ToString());
                                if (Session["REQID"] != null && Session["REQAMID"] != null && (Convert.ToBoolean(Session["REQAMID"].ToString()) == true))
                                {
                                    //txtRate.Visible = true; GrnlRate.Visible = false;
                                    hfrate = "disabled";
                                    Amount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    //TotalAmount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    GTotal += Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["totalAmt"]), 2);
                                    RunningTotal += (Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString()));
                                }
                                else if (Session["REQID"] != null)
                                {
                                    Amount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    //TotalAmount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    //GTotal += Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    GTotal += Convert.ToDecimal(ds.Tables[0].Rows[i]["totalAmt"]);
                                    RunningTotal += (Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString()));
                                }
                                else
                                {
                                    if (Quantity.Rows[i]["LPONumberraised"] != null && Quantity.Rows[i]["LPOCancel"].ToString() != "True")
                                    {
                                        txtQty = ((Convert.ToDecimal(MaxQty) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                                    (Convert.ToDecimal(MaxQty) - Convert.ToDecimal(CQuantity)).ToString());
                                    }

                                    Amount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                    //TotalAmount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["QPrice"].ToString())), 2);
                                    GTotal += Math.Round((Convert.ToDecimal(ds.Tables[0].Rows[i]["totalAmt"])), 2);
                                    RunningTotal += (Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString()));
                                }

                                RcvdQty = ((Convert.ToDecimal(MaxQty) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                             (Convert.ToDecimal(MaxQty) - Convert.ToDecimal(CQuantity)).ToString());

                                HF_ActualQty = Qty[0]["ActualQty"].ToString();
                                MaxQty = Qty[0]["Quantity"].ToString();
                                CQuantity = Convert.ToDecimal(Qty[0]["CQuantity"].ToString());

                            }
                            else
                            {
                                Amount = Math.Round((Convert.ToDecimal(txtQty) * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString())), 2);
                                MaxQty = "0";
                                RcvdQty = "0";
                                CQuantity = 0;
                                // discountAmt = (Convert.ToDecimal(ds.Tables[0].Rows[i]["discount"]));
                                HF_ActualQty = (ds.Tables[0].Rows[i]["qty"].ToString());
                                RunningTotal += (Convert.ToDecimal(ds.Tables[0].Rows[i]["qty"].ToString())
                                * Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"].ToString()));
                                GTotal += Convert.ToDecimal(ds.Tables[0].Rows[i]["totalAmt"]);
                            }

                            if (Session["REQID"] != null && Session["REQID"].ToString() != "")
                            {
                                txtQty = CQuantity.ToString();
                            }

                            if (Convert.ToDecimal(MaxQty) == (Convert.ToDecimal(CQuantity) - EditQty))
                            {
                                //txtQty.Enabled = false;
                                HfDisable = "disabled";

                            }

                            if (Convert.ToDecimal(ExcisDt) == 0 && IsExciseDuty == false)
                            {
                                Session["ChkbCEEApl"] = true;
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                                Session["txtCEEApl"] = true;
                            }
                            else
                                IsExciseDuty = true;
                            Hflbltmnt = (decimal.Round(Convert.ToDecimal(RunningTotal), 0, MidpointRounding.AwayFromZero)).ToString("N");
                            hflblTotl = (decimal.Round(Convert.ToDecimal(GTotal), 0, MidpointRounding.AwayFromZero)).ToString("N");
                            sb.Append("<td><input " + HfDisable + " type='text' name='txtQuantity' size='05px' onchange='CheckQtys(" + sno
                              + ")' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["qty"].ToString()
                              + "'  "
                              + " maxlength='6' class='bcAsptextbox' style='width:50px;'/>"
                              + " <input type='hidden' name='Hfd_MQty' id='Hfd_MQty" + sno + "' value='"
                              + (ds.Tables[0].Columns.Contains("QTY") ? ds.Tables[0].Rows[i]["QTY"].ToString() : ds.Tables[0].Rows[i]["qty"].ToString()) + "'/>"
                                + " <input type='hidden' name='HFExdtPrcnt' id='HFExdtPrcnt" + sno + "' value='"
                              + (ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) + "'/>"
                              + "</td>");
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + " <input type='hidden' name='HF_RCVDQty' id='HF_RCVDQty" + sno + "' value='" + RcvdQty + "'/><input type='hidden' name='HF_ActualQty' id='HF_ActualQty" + sno + "' value='" + HF_ActualQty + "'/></td>");
                            sb.Append("<td align='right'>" + ds.Tables[0].Rows[i]["rate"].ToString() + " <input type='hidden' name='HF_CngPrice' id='HF_CngPrice" + sno + "' value='" + ds.Tables[0].Rows[i]["rate"].ToString() + "'/> </td> ");//PartNo

                            //sb.Append("<td><input " + hfrate + " type='text' name='debit_amt' disabled class='bcAsptextbox' maxlength='10' value='" +
                            //          ds.Tables[0].Rows[i]["rate"].ToString() + "'  id='Rate" + sno +
                            //           "' onblur='extractNumber(this,2,true);' onkeyup='extractNumber(this,2,true);' onkeypress='return blockNonNumbers(this, event, true, true);'  style='text-align: right; width: 120px;'/></td>");

                            sb.Append("<td align='right'>" + ds.Tables[0].Rows[i]["Amount"].ToString() + "</td>");//PartNo
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString() + "</td>");//PartNo
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "</td>");//PartNo
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["QPrice"].ToString() + "</td>");//PartNo
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["totalAmt"].ToString() + "</td>");//PartNo 
                            sb.Append("</tr>");
                        }

                        sb.Append("</tbody>");
                        sb.Append("<tfoot>");
                        sb.Append("<th colspan='6' class='rounded-foot-left'>");


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
                        if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                        {
                            PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                                .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();

                            //TotalAmountText.Text = "Amount(" + PriceSymbol + ") :";
                            // GrandTotalAmountText.Text = "Total(" + PriceSymbol + ") :";
                        }

                        sb.Append("<th colspan='4' align='right'><b><span>Total Amount(" + PriceSymbol + "):" + TotalAmount.ToString("N") + "</span></b></th>");
                        sb.Append("<th class='rounded-foot-right' colspan='4' align='center'><b><span>Grand Total Amount(" + PriceSymbol + "):" + GTotal.ToString("N") + "</span></b>  <input type='hidden' name='Hflbltmnt' id='Hflbltmnt' value='" + Hflbltmnt + "'/> <input type='hidden' name='hflblTotl' id='HflblTotl' value='" + hflblTotl + "'/> </th>");
                        //sb.Append("<th class='rounded-foot-right'><b><span></span></b></th>");
                        sb.Append("</tfoot>");
                    }
                    sb.Append("</tbody></table>");

                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return string.Empty;
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
                else if (dt != null && dt.Rows.Count > 0)
                    TotalAmt = Convert.ToInt32(dt.Compute("Sum(PaymentPercentage)", ""));
                if (Session["MessageLPO"] != null)
                    Message = Session["MessageLPO"].ToString();

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblPaymentTerms' " +
                    "align='center'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Payment(%)</th><th>Description</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt != null && dt.Rows.Count > 0)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="gvItems"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(DataSet dsi)
        {
            DataTable dt = CommonBLL.EmptyDtLPOrders();
            //DataSet dsi = (DataSet)Session["LPOGrid"];
            dt.Rows[0].Delete(); int exdt = 0, dscnt = 0;
            //foreach (GridViewRow row in gvItems.Rows)
            //{
            for (int i = 0; i < dsi.Tables[1].Rows.Count; i++)
            {
                DataRow dr;
                if (Convert.ToBoolean(dsi.Tables[1].Rows[i]["IsCheck"].ToString()))
                {
                    dr = dt.NewRow();
                    dr["ItemDetailsID"] = dsi.Tables[1].Rows[i]["ItemDetailsId"].ToString();//new Guid(((Label)row.FindControl("lblItemDetailsID")).Text);
                    dr["SNo"] = Convert.ToInt32(i + 1);
                    dr["ItemId"] = new Guid(dsi.Tables[1].Rows[i]["ItemID"].ToString());
                    dr["LPOrderId"] = Guid.Empty;
                    dr["PartNumber"] = dsi.Tables[1].Rows[i]["PartNumber"].ToString();
                    dr["Specifications"] = dsi.Tables[1].Rows[i]["Specifications"].ToString();
                    dr["Make"] = dsi.Tables[1].Rows[i]["Make"].ToString();
                    dr["Quantity"] = (String.IsNullOrWhiteSpace(dsi.Tables[1].Rows[i]["qty"].ToString()) ? 0 : Convert.ToDecimal(dsi.Tables[1].Rows[i]["qty"].ToString()));     //Convert.ToDecimal(((Label)row.FindControl("lblQty")).Text);
                    dr["Rate"] = Convert.ToDecimal(dsi.Tables[1].Rows[i]["rate"].ToString());
                    dr["Amount"] = Convert.ToDecimal(dsi.Tables[1].Rows[i]["Amount"].ToString());

                    if (dsi.Tables[1].Rows[i]["ExDutyPercentage"].ToString() == "")
                        dr["ExDutyPercentage"] = 0;
                    else
                        dr["ExDutyPercentage"] = Convert.ToDecimal(dsi.Tables[1].Rows[i]["ExDutyPercentage"].ToString());

                    if (dsi.Tables[1].Rows[i]["DiscountPercentage"].ToString() == "")
                        dr["DiscountPercentage"] = 0;
                    else
                        dr["DiscountPercentage"] = Convert.ToDecimal(dsi.Tables[1].Rows[i]["DiscountPercentage"].ToString());
                    dr["UNumsId"] = new Guid(dsi.Tables[1].Rows[i]["UNumsId"].ToString());
                    dr["Remarks"] = dsi.Tables[1].Rows[i]["Remarks"].ToString();
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
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.PriceTagText)))
                    {
                        Session["HideFields"] = HideFields.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearAll()
        {
            try
            {
                btnSave.Text = "Save";
                ddlCustomer.SelectedIndex = -1; //ddlsuplrctgry.SelectedIndex = -1;
                ListBoxFPO.SelectedIndex = ListBoxFEO.SelectedIndex = ListBoxFEO.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlSuplr.SelectedIndex = ListBoxLPO.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                txtDlvry.Text = txtimpinst.Text = txtLpoDt.Text = txtLpoDueDt.Text = txtLpono.Text = txtsubject.Text = "";
                ChkbInspcn.Checked = ChkbCEEApl.Checked = false; txtLpono.Enabled = false;
                txtDsnt.Text = txtExdt.Text = txtPkng.Text = txtSltx.Text = "0";
                txtLpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(3));
                ListBoxFPO.Enabled = true;
                ddlSuplr.Enabled = true;
                ListBoxLPO.Enabled = true;
                ViewState["EditID"] = null;
                Session["MessageLPO"] = null;
                Session["amountLPO"] = null;
                Session["PaymentTermsLPO"] = null;
                Session["LPOCheckAllBoxes"] = null;
                Session["PopUpErrMsg"] = null;
                Session["ddlCustomer"] = null;
                Session["FPOIDS"] = null;

                Session["LPOIDS"] = null;
                Session["FEIDS"] = null;

                Session["LPOGrid"] = null;
                Session["REQID"] = null;
                Session["REQAMID"] = null;
                Session["LPOGrid"] = null;
                divLPOItems.InnerHtml = FillGridView();
                Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                //gvLpoItems.DataSource = null;
                //gvLpoItems.DataBind();
                Session["LPOUploads"] = null;
                ListBoxFPO.Items.Clear();
                ListBoxFEO.Items.Clear();
                ListBoxLPO.Items.Clear();
                #region Paging

                //if (Session["RowsDisplay_FPO"] == null)
                Session["RowsDisplay_LPO"] = 100;
                //if (Session["CPage_FPO"] == null)
                Session["CPage_LPO"] = 1;

                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "LPO ", ex.Message.ToString());
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
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (ListBoxFPO.SelectedItem == null)
                {
                    ListBoxLPO.Items.Clear();
                }
                Session["LPOGrid"] = null;
                Session["PaymentTermsLPO"] = null;
                FillPaymentTerms();
                //gvLpoItems.DataSource = null;
                //gvLpoItems.DataBind();
                divLPOItems.InnerHtml = FillGridView();
                BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                    Guid.Empty.ToString(), DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                    100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                ListBoxFEO.Enabled = false;
                ListBoxLPO.Items.Clear();
                ddlSuplr.Items.Clear();
                BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagHSelect, Guid.Empty, Guid.Empty, Guid.Empty, fpono, DateTime.Now, DateTime.Now,
                    100, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));
                FillInputFields(fpono, ddlCustomer.SelectedValue);

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                    string FY = CommonBLL.GetFinYrShortName();
                    if (FY != "")
                    {
                        string path = Request.Path;
                        path = Path.GetFileName(path);
                        string feno1 = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        string feno2 = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        DataSet LQuotations = NLPOBL.GetDataSet(CommonBLL.FlagODRP, Guid.Empty, Guid.Empty, feno1, feno2, Guid.Empty, Guid.Empty,
                            Guid.Empty, DateTime.Now, DateTime.Now, new Guid(ddlSuplr.SelectedValue), "", DateTime.Now, 0, 100, "", new Guid(Session["UserID"].ToString()),
                            CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                        LocalPurchaseOrder(ListBoxLPO, LQuotations);
                        txtLpono.Text = LQuotations.Tables[3].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + (LQuotations.Tables[1].Rows[0]["LPOSequence"].ToString()).PadLeft(3, '0') + "/" +
                        ListBoxFPO.SelectedItem.Text.Trim() + "/" + CommonBLL.GetFinYrShortName();
                        ddlsuplrctgry.SelectedValue = LQuotations.Tables[2].Rows[0]["CategoryID"].ToString();
                        //gvLpoItems.DataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                     "ErrorMessage('No Financial Year Created,Please Create Financial Year in Financial Year Master.');", true);
                    }
                }
                else
                {
                    ddlPrcBsis.SelectedIndex = -1;
                    divPaymentTerms.InnerHtml = "";
                    txtDlvry.Text = "0";
                    ListBoxLPO.Items.Clear();
                    //gvLpoItems.DataSource = null;
                    //gvLpoItems.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                bool IsExciseDuty = false;
                if (ListBoxLPO.SelectedValue != "")
                {
                    string FEIDs = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string flqno = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string fpno = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet LclQuoteItems = NLQBL.LPOItemsByMulti(CommonBLL.FlagBSelect, new Guid(ListBoxLPO.SelectedValue), flqno, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, new Guid(ddlSuplr.SelectedValue), FEIDs, 0, "", Guid.Empty, CommonBLL.EmptyDtLocalQuotation(),
                        CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), Guid.Empty, fpno, new Guid(Session["CompanyID"].ToString()));

                    //Session["LPOItmQtyCheck"] = LclQuoteItems.Tables[4];

                    if (LclQuoteItems.Tables[1].Rows.Count > 0)
                    {
                        ComparisonDTS = (DataSet)ViewState["ComparisonDTS"];
                        if (ComparisonDTS != null)
                        {
                            ComparisonDTS.Tables[0].Merge(LclQuoteItems.Tables[1]);
                            ComparisonDTS.GetChanges();
                        }
                        if (LclQuoteItems.Tables.Count > 4)
                            Session["Quantity"] = LclQuoteItems.Tables[4];
                        Session["LPOCheckAllBoxes"] = true;
                        DataTable TOTAL_QTY_L = null;
                        if (LclQuoteItems.Tables.Count > 5)
                            TOTAL_QTY_L = LclQuoteItems.Tables[5];
                        Session["TOTAL_QTY_L"] = TOTAL_QTY_L;

                        LclQuoteItems.Tables[1].Columns.Add("QTYsum", typeof(decimal));//Need to Remove This Column Before Saving.
                        foreach (DataRow row in LclQuoteItems.Tables[1].Rows)
                        {
                            if (TOTAL_QTY_L != null && TOTAL_QTY_L.Rows.Count > 0)
                            {
                                DataRow rowsToUpdate = TOTAL_QTY_L.AsEnumerable().FirstOrDefault(r => r.Field<Guid>("itemid") == row.Field<Guid>("itemid"));
                                if (rowsToUpdate != null)
                                {
                                    row.SetField("quantity", Math.Abs(rowsToUpdate.Field<decimal>("QTY") - row.Field<decimal>("quantity")));
                                    row.SetField("amount", Math.Abs(row.Field<decimal>("quantity") * row.Field<decimal>("rate")));
                                    row.SetField("QTYsum", Math.Abs(row.Field<decimal>("quantity")));
                                }
                                else
                                    row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                            }
                            //else
                            //    row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                        }

                        //gvLpoItems.DataSource = LclQuoteItems.Tables[1];
                        //gvLpoItems.DataBind();

                        Session["LPOGrid"] = LclQuoteItems;
                        //FillGridView();
                        Session["LPOCheckAllBoxes"] = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "dfgdgfgf",
                                "CalculateExDuty();", true);
                        divLPOItems.InnerHtml = FillGridView();
                        if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                            //gvLpoItems.Columns[11].Visible = false;

                            ddlPrcBsis.SelectedValue = LclQuoteItems.Tables[0].Rows[0]["PriceBasis"].ToString();
                        txtPriceBasis.Text = LclQuoteItems.Tables[0].Rows[0]["PriceBasisText"].ToString();
                        txtDlvry.Text = LclQuoteItems.Tables[0].Rows[0]["DeliveryPeriods"].ToString();
                        if (txtDlvry.Text != "")
                            txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text))));

                        if ((Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString()) == 0) && IsExciseDuty == false)
                        {
                            ChkbCEEApl.Enabled = true; ChkbCEEApl.Checked = true;
                            Session["ChkbCEEApl"] = true;
                            txtCEEApl.Enabled = true;
                            IsExciseDuty = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDutyRmd",
                                "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                        }
                        else
                        {
                            ChkbCEEApl.Enabled = false; ChkbCEEApl.Checked = false;
                            Session["ChkbCEEApl"] = false;
                            txtCEEApl.Enabled = false;
                            IsExciseDuty = false;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDutyRmd",
                                "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                        }
                        txtExdt.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString()) == 0) ?
                            "0" : LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString();
                        if (txtExdt.Text != "0")
                        {
                            chkExdt.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty",
                                "CHeck('chkExdt', 'dvExdt');", true);
                        }
                        txtSGST.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["SGSTPercentage"].ToString()) == 0) ?
                            "0" : LclQuoteItems.Tables[0].Rows[0]["SGSTPercentage"].ToString();
                        if (txtSGST.Text != "0")
                        {
                            chkSGST.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowdvSGST",
                                "CHeck('chkSGST','dvSGST');", true);
                        }
                        txtIGST.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["IGSTPercentage"].ToString()) == 0) ?
                            "0" : LclQuoteItems.Tables[0].Rows[0]["IGSTPercentage"].ToString();
                        if (txtIGST.Text != "0")
                        {
                            chkIGST.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowdvIGST",
                                "CHeck('chkIGST', 'dvIGST');", true);
                        }
                        txtPkng.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["packingPercentage"].ToString()) == 0) ?
                            "0" : LclQuoteItems.Tables[0].Rows[0]["packingPercentage"].ToString();
                        if (txtPkng.Text != "0")
                        {
                            chkPkng.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng",
                                "CHeck('chkPkng', 'dvPkng');", true);
                        }

                        txtDsnt.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString()) > 0) ?
                            LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString() : LclQuoteItems.Tables[0].Rows[0]["DiscountPercentage"].ToString();
                        if (Convert.ToDecimal(txtDsnt.Text) > 0)
                        {
                            chkDsnt.Checked = true;
                            string SelVal = "";
                            rbtnDsnt.SelectedValue = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["Discount"].ToString()) > 0) ? "1" : "0";

                            SelVal = rbtnDsnt.SelectedValue;

                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt",
                                "CHeckForLpo('chkDsnt', 'dvDsnt','" + SelVal + "');", true);
                        }

                        txtSltx.Text = (Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["SaleTaxPercentage"].ToString()) == 0) ?
                            "0" : LclQuoteItems.Tables[0].Rows[0]["SaleTaxPercentage"].ToString();
                        if (txtSltx.Text != "0")
                        {
                            chkSltx.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx",
                                "CHeck('chkSltx', 'dvSltx');", true);
                        }
                    }
                    if (LclQuoteItems.Tables.Count > 1 && LclQuoteItems.Tables[2].Rows.Count > 0)
                    {
                        FillQuotationTerms(LclQuoteItems.Tables[2]);
                    }
                    var LineItemGST = LclQuoteItems.Tables[1].AsEnumerable().Where(R => R.Field<decimal>("ExDutyPercentage") > Decimal.Zero).ToList();
                    if (LineItemGST.Count > 0)
                    {
                        chkExdt.Checked = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDutyE", "CHeck('chkExdt','dvExdt');", true);
                        chkSGST.Checked = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSGSTE", "CHeck('chkSGST', 'dvSGST');", true);
                        chkIGST.Checked = false;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowIGSTE", "CHeck('chkIGST','dvIGST');", true);
                        chkExdt.Enabled = false;
                        chkSGST.Enabled = false;
                        chkIGST.Enabled = false;

                    }
                }
                else
                {
                    divPaymentTerms.InnerHtml = "";
                    txtDlvry.Text = "0";
                    //gvLpoItems.DataSource = null;
                    //gvLpoItems.DataBind();
                    Session["LPOGrid"] = null;

                    divLPOItems.InnerHtml = FillGridView();
                }

                GetSessionData();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }


        public void GetSessionData()
        {
            try
            {
                Session["FPOIDS"] = "";
                Session["FEIDS"] = "";
                Session["LPOIDS"] = "";

                string FPOIDS = string.Empty; string FEIDS = string.Empty; string LPOIDS = string.Empty;
                for (int i = 0; i < ListBoxFPO.Items.Count; i++)
                {
                    if (ListBoxFPO.Items[i].Selected)
                        FPOIDS = ListBoxFPO.Items[i].Value + "," + FPOIDS;
                }

                for (int i = 0; i < ListBoxFEO.Items.Count; i++)
                {
                    if (ListBoxFEO.Items[i].Selected)
                        FEIDS = ListBoxFEO.Items[i].Value + "," + FEIDS;
                }

                for (int i = 0; i < ListBoxLPO.Items.Count; i++)
                {
                    if (ListBoxLPO.Items[i].Selected)
                        LPOIDS = ListBoxLPO.Items[i].Value + "," + LPOIDS;
                }
                Session["FPOIDS"] = FPOIDS;
                Session["FEIDS"] = FEIDS;
                Session["LPOIDS"] = LPOIDS;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                    //ClearAll();
                    ddlSuplr.Items.Clear();
                    DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                    if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                        EmtpyFPO.Columns.Remove("ItemDetailsId");
                    LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagRegularDRP, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                        Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                        "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions()));

                    Session["ddlCustomer"] = ddlCustomer.SelectedValue;
                    Session["LPOGrid"] = null;
                    divLPOItems.InnerHtml = FillGridView();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Quantity Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void txtQty_TextChanged(object sender, EventArgs e)
        //{
        //    TextBox txet = (TextBox)sender;
        //    GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        //    Label GTotal = (Label)gvLpoItems.FooterRow.FindControl("lbltmnt");
        //    Label MTotal = (Label)gvLpoItems.FooterRow.FindControl("lblTotl");
        //    Label Rate = (Label)row.FindControl("lblRate");
        //    TextBox CngRate = (TextBox)row.FindControl("txtRate");
        //    TextBox CngQty = (TextBox)row.FindControl("txtQty");
        //    Label NetRate = (Label)row.FindControl("lblNRate");
        //    Label TotalR = (Label)row.FindControl("lblTotal");
        //    Label Amount = (Label)row.FindControl("lblAmount");
        //    HiddenField HdnRate = (HiddenField)row.FindControl("HF_CngPrice");

        //    Decimal TempVal = Convert.ToDecimal(GTotal.Text) - Convert.ToDecimal(Amount.Text);
        //    if (ChkbCEEApl.Checked == true)
        //    {
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls",
        //        "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
        //    }
        //    if (txet.Text != "")
        //    {
        //        if (Request.QueryString != null && Request.QueryString.Count >= 2 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] != null && (Convert.ToBoolean(Request.QueryString["IsAm"].ToString()) == true))
        //        {
        //            HdnRate.Value = CngRate.Text;
        //            decimal Rte = Convert.ToDecimal(CngRate.Text);
        //            decimal Qty = Convert.ToDecimal(CngQty.Text);
        //            Amount.Text = (Qty * Rte).ToString();
        //            TotalR.Text = (Qty * Rte).ToString();
        //            GTotal.Text = (TempVal + (Qty * Rte)).ToString();
        //            MTotal.Text = (TempVal + (Qty * Rte)).ToString();
        //        }
        //        else
        //        {
        //            decimal Rte = Convert.ToDecimal(Rate.Text);
        //            decimal Qty = Convert.ToDecimal(txet.Text);
        //            Amount.Text = (Qty * Rte).ToString();
        //            TotalR.Text = (Qty * Rte).ToString();
        //            GTotal.Text = (TempVal + (Qty * Rte)).ToString();
        //            MTotal.Text = (TempVal + (Qty * Rte)).ToString();
        //        }

        //    }
        //}

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
                if (res != 0)
                {
                    decimal exdt = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, SGST = 0, SGSTPrcnt = 0, IGST = 0, IGSTPrcnt = 0, sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0;
                    Filename = FileName();
                    DateTime lpoDate = CommonBLL.DateInsert(txtLpoDt.Text);
                    DateTime lpoDueDate = CommonBLL.DateInsert(txtLpoDueDt.Text);
                    string LPNS = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());
                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    if (TCs.Columns.Contains("Title"))
                        TCs.Columns.Remove("Title");
                    if (TCs.Columns.Contains("CompanyId"))
                        TCs.Columns.Remove("CompanyId");

                    DataTable dtbl = ConvertToDtbl((DataSet)Session["LPOGrid"]), sdt = (DataTable)Session["PaymentTermsLPO"];
                    if (dtbl.Columns.Contains("QTYsum"))
                        dtbl.Columns.Remove("QTYsum");
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

                        if (btnSave.Text == "Save" && dtbl.Rows.Count > 0 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True"
                             && ListBoxFEO.SelectedValue != "" && ListBoxLPO.SelectedValue != "")
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);

                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    SGSTPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    SGST = Convert.ToDecimal(txtSGST.Text);
                                else SGST = Convert.ToDecimal(txtSGST.Text);
                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    IGSTPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    IGST = Convert.ToDecimal(txtIGST.Text);
                                else IGST = Convert.ToDecimal(txtIGST.Text);

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
                            LPNS + "/" + CommonBLL.GetFinYrShortName() + "/Amend/" + CrntIdnt.Tables[1].Rows[0]["HistoryNum"].ToString();


                            string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, fponum, feonum, Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                ListBoxLPO.SelectedValue, txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, true, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, SGST, SGSTPrcnt, IGST, IGSTPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }
                        else if (btnSave.Text == "Save" && dtbl.Rows.Count > 0 && ListBoxFEO.SelectedValue != "" && ListBoxLPO.SelectedValue != "")
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    SGSTPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    SGST = Convert.ToDecimal(txtSGST.Text);
                                else SGST = Convert.ToDecimal(txtSGST.Text);
                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    IGSTPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    IGST = Convert.ToDecimal(txtIGST.Text);
                                else IGST = Convert.ToDecimal(txtIGST.Text);
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
                            LPNS + "/" + CommonBLL.GetFinYrShortName();

                            string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, fponum, feonum, Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                ListBoxLPO.SelectedValue, txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, true, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, SGST, SGSTPrcnt, IGST, IGSTPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", "Error while Saving.");
                                FillPaymentTerms();
                            }
                        }
                        else if (btnSave.Text == "Update" && dtbl.Rows.Count > 0 && ListBoxFEO.SelectedValue != "" && ListBoxLPO.SelectedValue != "")
                        {
                            if (chkExdt.Checked)
                                if (rbtnExdt.SelectedValue == "0")
                                    exdtPrcnt = Convert.ToDecimal(txtExdt.Text);
                                else if (rbtnExdt.SelectedValue == "1")
                                    exdt = Convert.ToDecimal(txtExdt.Text);
                                else exdt = Convert.ToDecimal(txtExdt.Text);
                            if (chkSGST.Checked)
                                if (rbtnSGST.SelectedValue == "0")
                                    SGSTPrcnt = Convert.ToDecimal(txtSGST.Text);
                                else if (rbtnSGST.SelectedValue == "1")
                                    SGST = Convert.ToDecimal(txtSGST.Text);
                                else SGST = Convert.ToDecimal(txtSGST.Text);
                            if (chkIGST.Checked)
                                if (rbtnIGST.SelectedValue == "0")
                                    IGSTPrcnt = Convert.ToDecimal(txtIGST.Text);
                                else if (rbtnIGST.SelectedValue == "1")
                                    IGST = Convert.ToDecimal(txtIGST.Text);
                                else IGST = Convert.ToDecimal(txtIGST.Text);
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
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                                Guid.Empty, fponum, feonum, Guid.Empty, new Guid(ListBoxFEO.SelectedValue), new Guid(ddlCustomer.SelectedValue),
                                txtLpono.Text, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue),
                                new Guid(ddlSuplr.SelectedValue), ListBoxLPO.SelectedValue, txtsubject.Text,
                                new Guid(ddlRsdby.SelectedValue), txtimpinst.Text, ChkbInspcn.Checked, true,
                                ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder,
                                txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs,
                                exdt, exdtPrcnt, SGST, SGSTPrcnt, IGST, IGSTPrcnt, dscnt, dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Update")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New Local Purchase Order",
                                    "Data Updated Successfully.");
                                ClearAll(); Session.Remove("TCs");
                                Response.Redirect("LPOStatus.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Updating.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", "Error while Updating.");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                //Response.Redirect("../Purchases/NewLPOrder.Aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                    HiddenField Rowno = (HiddenField)e.Row.FindControl("hfFESNo");
                    Label lblitem = (Label)e.Row.FindControl("lblItemID");
                    Label amount = (Label)e.Row.FindControl("lblAmount");
                    Label NetRate = (Label)e.Row.FindControl("lblNRate");
                    Label GrnlRate = (Label)e.Row.FindControl("lblRate");
                    Label Total = (Label)e.Row.FindControl("lblTotal");
                    Label TotalAmountText = ((Label)e.Row.FindControl("lbltmnt"));//(Label)e.Row.FindControl("LblAmo");
                    TextBox txtQty = (TextBox)e.Row.FindControl("txtQty");
                    HiddenField MaxQty = (HiddenField)e.Row.FindControl("Hfd_MQty");
                    HiddenField RcvdQty = (HiddenField)e.Row.FindControl("HF_RCVDQty");
                    TextBox txtRate = (TextBox)e.Row.FindControl("txtRate");
                    int RwNo = Convert.ToInt16(Rowno.Value) - 1;
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
                            if (Quantity.Rows[RwNo]["LPONumberraised"] != null && Quantity.Rows[RwNo]["LPOCancel"].ToString() != "True")
                            {
                                txtQty.Text = ((Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                            (Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)).ToString());
                            }

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
                    Label TotalAmountText = (Label)e.Row.FindControl("LblAmo");
                    Label GrandTotalAmountText = (Label)e.Row.FindControl("LblTotAmount");
                    if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                    {
                        string PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                            .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();

                        TotalAmountText.Text = "Amount(" + PriceSymbol + ") :";
                        GrandTotalAmountText.Text = "Total(" + PriceSymbol + ") :";
                    }
                    else
                    {
                        TotalAmountText.Text = "Amount :";
                        GrandTotalAmountText.Text = "Total :";
                    }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void gvLpoItems_PreRender(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (gvLpoItems.HeaderRow == null) return;
        //        gvLpoItems.UseAccessibleHeader = false;
        //        gvLpoItems.HeaderRow.TableSection = TableRowSection.TableHeader;
        //        gvLpoItems.FooterRow.TableSection = TableRowSection.TableFooter;
        //        gvLpoItems.Columns.Cast<DataControlField>().SingleOrDefault(x => x.HeaderText == "Ex-Dt(%)");
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
        //    }
        //}
        #endregion

        # region CheckBox Events
        /// <summary>
        /// This is Used to check all the CheckBoxes from Header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void HdrChkbx_OnCheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        CheckBox ChkParent = (CheckBox)sender;

        //        if (ChkParent.Checked)
        //        {
        //            ArrayList al = DT2Array();
        //            if (ViewState["IsCustomer"] == "false")
        //            {
        //                ArrayList al1 = (ArrayList)ViewState["LPORelease"];
        //                DataTable dt = (DataTable)ViewState["Quantity"];
        //                List<DataRow> _LRows = new List<DataRow>();
        //                if (dt.Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < gvLpoItems.Rows.Count; i++)
        //                    {
        //                        HiddenField HfItemID_DT = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
        //                        //var _Row = (from d in dt.AsEnumerable() where d.Field<Guid>("ItemId") == new Guid(HfItemID_DT.Value) select new  { d });
        //                        DataRow DR = dt.AsEnumerable().Where(P => P.Field<Guid>("ItemId") == new Guid(HfItemID_DT.Value)).FirstOrDefault();
        //                        _LRows.Add(DR);
        //                    }
        //                    DataTable _Table = _LRows.CopyToDataTable();
        //                }
        //                //dt = _Table;
        //                int j = 0;
        //                bool IsFPOChk = false;
        //                bool IsLPOChk = false;
        //                string LPOItems = "", LPOItms1 = "";
        //                string FPOItems = "";
        //                string Error = "";
        //                for (int i = 0; i < gvLpoItems.Rows.Count; i++)
        //                {

        //                    CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");

        //                    HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
        //                    if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
        //                        cb.Checked = true;
        //                    else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) &&
        //                        !al1.Contains(new Guid(HfItemID.Value)))
        //                        cb.Checked = true;

        //                    else
        //                    {
        //                        ChkParent.Checked = false;
        //                        cb.Checked = false;
        //                        if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
        //                        {
        //                            FPOItems += " " + (i + 1) + ", ";
        //                            IsFPOChk = true;
        //                        }
        //                        else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)))
        //                        {
        //                            LPOItems += " " + (i + 1) + ", ";
        //                            IsLPOChk = true;
        //                        }
        //                        else
        //                            Error = "Error while Checking.";
        //                    }

        //                    if (cb.Checked == true && dt.Rows.Count > 0 && dt.Rows[i]["LPONumberraised"] != null)
        //                    {
        //                        j = i - dt.Rows.Count;
        //                        string ItmQty = dt.Rows[i]["Quantity"].ToString();
        //                        string ItmCQty = dt.Rows[i]["CQuantity"].ToString();
        //                        if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0") && dt.Rows[i]["ItemId"].ToString() == HfItemID.Value
        //                            && dt.Rows[i]["LPOCancel"].ToString() != "True")
        //                            cb.Checked = false;
        //                        else if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0") && dt.Rows[i]["ItemId"].ToString() == HfItemID.Value
        //                            && dt.Rows[i]["LPOCancel"].ToString() == "True")
        //                            cb.Checked = true;
        //                        else
        //                            cb.Checked = true;
        //                        j++;
        //                    }

        //                }




        //                if (IsLPOChk && IsFPOChk)
        //                {
        //                    ScriptManager.RegisterStartupScript(this.Page,
        //                        this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item No(s)" + LPOItems.Trim(',') +
        //                         "And Item(s) " + FPOItems.Trim(',') + " are not selected in FPO.');", true);
        //                }
        //                else if (IsFPOChk == true && IsLPOChk == false)
        //                {
        //                    ScriptManager.RegisterStartupScript(this.Page,
        //                        this.GetType(), "ShowAll3", "ErrorMessage('Item(s) " + FPOItems.Trim(',')
        //                        + " are not selected in FPO.');", true);
        //                }
        //                else if (IsFPOChk == false && IsLPOChk == true)
        //                    ScriptManager.RegisterStartupScript(this.Page,
        //                        this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItems.Trim(',') + " ');", true);

        //                for (int i = 0; i < gvLpoItems.Rows.Count; i++)
        //                {
        //                    //CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");
        //                    //if (i >= dt.Rows.Count)
        //                    //    j = i - dt.Rows.Count;
        //                    //string ItmQty = dt.Rows[j]["Quantity"].ToString();
        //                    //string ItmCQty = dt.Rows[j]["CQuantity"].ToString();
        //                    //if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0"))
        //                    //{
        //                    //    cb.Checked = false;
        //                    //    LPOItms1 += " " + (j + 1) + ", ";
        //                    //    IsLPOChk = true;
        //                    //}
        //                    //j++;
        //                }
        //                if (IsLPOChk == true)
        //                    ScriptManager.RegisterStartupScript(this.Page,
        //                       this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItms1.Trim(',') + " ');", true);

        //            }
        //            else
        //            {
        //                for (int q = 0; q < gvLpoItems.Rows.Count; q++)
        //                {
        //                    CheckBox cb = (CheckBox)gvLpoItems.Rows[q].FindControl("ItmChkbx");
        //                    cb.Checked = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int j = 0; j < gvLpoItems.Rows.Count; j++)
        //            {
        //                CheckBox cb = (CheckBox)gvLpoItems.Rows[j].FindControl("ItmChkbx");
        //                cb.Checked = false;
        //            }
        //        }
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty1", "CHeck('chkExdt', 'dvExdt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng1", "CHeck('chkPkng', 'dvPkng');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt1", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx1", "CHeck('chkSltx', 'dvSltx');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
        //    }
        //    divListBox.InnerHtml = AttachedFiles();
        //}

        /// <summary>
        /// This is used to check Individual CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void ItmChkbx_OnCheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        CheckBox ItmChkbx = (CheckBox)sender;
        //        ArrayList al = DT2Array();
        //        ArrayList al1 = (ArrayList)ViewState["LPORelease"];
        //        int GvRowCount = gvLpoItems.Rows.Count;
        //        GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
        //        int rowIndex = Convert.ToInt32(row.RowIndex);
        //        DataTable dt = (DataTable)ViewState["Quantity"];
        //        HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[rowIndex].FindControl("HfItemID");
        //        if (ItmChkbx.Checked) //&& GvRowCount == Convert.ToInt16(dt.Rows.Count.ToString()) // removed by dinesh 08/06/2016 for Item check
        //        {

        //            // //int RIndex =dt.Rows.Count - rowIndex;
        //            //string ItmQty = dt.Rows[rowIndex]["Quantity"].ToString();// by dinesh on 08062016 for Item Checkbox check
        //            // string ItmCQty = dt.Rows[rowIndex]["CQuantity"].ToString(); //by dinesh on 08062016 for Item Checkbox check
        //            if (ViewState["IsCustomer"] == "false")
        //            {
        //                CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");

        //                if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
        //                    cb.Checked = true;
        //                else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) && !al1.Contains(new Guid(HfItemID.Value)))
        //                    // && (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0") // by dinesh on 08062016 for Item Checkbox check
        //                    cb.Checked = true;
        //                else
        //                {
        //                    cb.Checked = false;
        //                    if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
        //                        ScriptManager.RegisterStartupScript(this.Page,
        //                            this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected in FPO.');", true);
        //                    else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)))
        //                        //|| (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0") //by dinesh on 08062016 for Item Checkbox check
        //                        ScriptManager.RegisterStartupScript(this.Page,
        //                            this.GetType(), "ShowAll3", "ErrorMessage('This Item Was already Selected in Another LPO.');", true);
        //                    else
        //                        ScriptManager.RegisterStartupScript(this.Page,
        //                            this.GetType(), "ShowAll2", "ErrorMessage('Error while Checking.');", true);
        //                }
        //            }
        //            else
        //            {
        //                CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
        //                cb.Checked = true;
        //            }
        //        }
        //        else
        //        {
        //            if (ViewState["IsCustomer"] == "false")
        //            {
        //                CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
        //                cb.Checked = false;
        //                if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
        //                    ScriptManager.RegisterStartupScript(this.Page,
        //                        this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected.');", true);
        //            }
        //            else
        //            {
        //                CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
        //                cb.Checked = false;
        //            }
        //        }
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty2", "CHeck('chkExdt', 'dvExdt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "CHeck('chkPkng', 'dvPkng');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt2", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx2", "CHeck('chkSltx', 'dvSltx');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
        //    }
        //    divListBox.InnerHtml = AttachedFiles();
        //}
        # endregion

        #region Web Methods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string NextPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_LPO"] = (CPage + 1);
            Session["RowsDisplay_LPO"] = txtRowsChanged;
            DataSet dsi = (DataSet)Session["LPOGrid"];
            //ViewState["Quantity"] = dsi.Tables[4];
            Session["LPOGrid"] = dsi;
            return FillGridView();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PrevPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_LPO"] = (CPage - 1);
            Session["RowsDisplay_LPO"] = txtRowsChanged;
            DataSet dsi = (DataSet)Session["LPOGrid"];
            //ViewState["Quantity"] = dsi.Tables[4];
            return FillGridView();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string RowsChanged(string CurrentPage, string txtRowsChanged)
        {
            if (txtRowsChanged.Trim() != "")
            {
                if (Convert.ToInt32(Session["RowsDisplay_LPO"].ToString()) != Convert.ToInt32(txtRowsChanged))
                {
                    int RowStart = ((Convert.ToInt32(Session["RowsDisplay_LPO"].ToString()) * Convert.ToInt32(CurrentPage)) - Convert.ToInt32(Session["RowsDisplay_LPO"].ToString())) + 1;

                    if (RowStart > Convert.ToInt32(txtRowsChanged))
                        Session["CPage_LPO"] = RowStart / Convert.ToInt32(txtRowsChanged) + 1;
                    else if (RowStart < Convert.ToInt32(txtRowsChanged) && RowStart != 1)
                        Session["CPage_LPO"] = (Convert.ToInt32(txtRowsChanged) + 1) / RowStart;
                }
                Session["RowsDisplay_LPO"] = Convert.ToInt32(txtRowsChanged);
            }
            DataSet dsi = (DataSet)Session["LPOGrid"];
            //ViewState["Quantity"] = dsi.Tables[4];
            return FillGridView();
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckQtyAdd(string Sno, string lblamount, string lbltotamount, string CngRates, string Quantity, string CurrentPage, int txtRowsChanged)
        {
            try
            {
                int i = Convert.ToInt32(Sno) - 1;
                //bool ItmChkbx = bool.Parse(Chck);
                //CheckBox ItmChkbx = (CheckBox)sender;
                DataSet dsi = (DataSet)Session["LPOGrid"];
                decimal GTotal = Convert.ToDecimal(lblamount);
                decimal MTotal = Convert.ToDecimal(lbltotamount);

                string Rate = CngRates;
                string CngRate = dsi.Tables[1].Rows[i]["rate"].ToString();
                string CngQty = Quantity;
                string RetAmount = string.Empty;
                string NetRate = dsi.Tables[1].Rows[i]["QPrice"].ToString();
                string TotalR = dsi.Tables[1].Rows[i]["totalAmt"].ToString();
                string Amount = dsi.Tables[1].Rows[i]["totalAmt"].ToString();
                decimal DiscountPerc = Convert.ToDecimal((dsi.Tables[1].Rows[i]["DiscountPercentage"] != "" || dsi.Tables[1].Rows[i]["DiscountPercentage"] != DBNull.Value) ?
                    dsi.Tables[1].Rows[i]["DiscountPercentage"] : 0);

                Decimal TempVal = Convert.ToDecimal(GTotal) - Convert.ToDecimal(Amount);
                if (Convert.ToBoolean(Session["ChkbCEEApl"]) == true)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls",
                    "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                }
                if (Quantity != "")
                {
                    if (Session["REQID"] != null && Session["REQAMID"] != null && (Convert.ToBoolean(Session["REQAMID"].ToString()) == true))
                    {
                        //HdnRate.Value = CngRate;
                        decimal Rte = Convert.ToDecimal(CngRate);
                        decimal Qty = Convert.ToDecimal(CngQty);
                        dsi.Tables[1].Rows[i]["rate"] = Rte.ToString();
                        dsi.Tables[1].Rows[i]["totalAmt"] = (Qty * Rte).ToString();

                        RetAmount = (Qty * Rte).ToString();
                        //TotalR = (Qty * Rte).ToString();
                        GTotal = (TempVal + (Qty * Rte));
                        MTotal = (TempVal + (Qty * Rte));
                    }
                    else
                    {
                        decimal Rte = Convert.ToDecimal(CngRate);
                        decimal Qty = Convert.ToDecimal(Quantity);
                        dsi.Tables[1].Rows[i]["rate"] = Rte.ToString();
                        if (DiscountPerc == 0)
                        {
                            dsi.Tables[1].Rows[i]["totalAmt"] = (Qty * Rte).ToString();
                        }
                        else
                        {
                            dsi.Tables[1].Rows[i]["totalAmt"] = (Qty * Rte) - ((Qty * Rte) * (DiscountPerc / 100));
                        }
                        RetAmount = (Qty * Rte).ToString();
                        //TotalR.Text = (Qty * Rte).ToString();
                        GTotal = (TempVal + (Qty * Rte));
                        MTotal = (TempVal + (Qty * Rte));
                    }

                }
                //Session["Quantity"] = dsi.Tables[4];
                Session["LPOGrid"] = dsi;
                Session["CPage_LPO"] = (CurrentPage);
                Session["RowsDisplay_LPO"] = txtRowsChanged;
                //return divLPOItems.InnerHtml = FillGridView();
                return (RetAmount);
                //return FillGridView();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return FillGridView();
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ChecErrorMsg()
        {
            try
            {
                if (Session["PopUpErrMsg"] != null)
                    return Session["PopUpErrMsg"].ToString();
                else
                    return "";
            }

            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                //return FillGridView();
                return "";
            }
        }
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckAllBoxs(string Chck, string CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["PopUpErrMsg"] = null;
                bool ItmChkbx = bool.Parse(Chck);
                //CheckBox ItmChkbx = (CheckBox)sender;
                DataSet dsi = (DataSet)Session["LPOGrid"];
                ArrayList al = DT2Array();
                ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                //int GvRowCount = gvLpoItems.Rows.Count;
                //GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                //int rowIndex = Convert.ToInt32(row.RowIndex);
                DataTable dt = (DataTable)Session["Quantity"];
                string ErrMsg = string.Empty;
                //for (int i = 0; i < dsi.Tables[1].Rows.Count; i++)
                //{

                if (ItmChkbx) //&& GvRowCount == Convert.ToInt16(dt.Rows.Count.ToString()) // removed by dinesh 08/06/2016 for Item check
                {
                    // //int RIndex =dt.Rows.Count - rowIndex;
                    //string ItmQty = dt.Rows[rowIndex]["Quantity"].ToString();// by dinesh on 08062016 for Item Checkbox check
                    // string ItmCQty = dt.Rows[rowIndex]["CQuantity"].ToString(); //by dinesh on 08062016 for Item Checkbox check
                    if (Session["IsCustomer"] == "false")
                    {
                        List<DataRow> _LRows = new List<DataRow>();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dsi.Tables[1].Rows.Count; i++)
                            {
                                string HfItemID = dsi.Tables[1].Rows[i]["ItemID"].ToString();
                                bool IsCheck = Convert.ToBoolean(dsi.Tables[1].Rows[i]["IsCheck"].ToString());
                                //HiddenField HfItemID_DT = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
                                //var _Row = (from d in dt.AsEnumerable() where d.Field<Guid>("ItemId") == new Guid(HfItemID_DT.Value) select new  { d });
                                DataRow DR = dt.AsEnumerable().Where(P => P.Field<Guid>("ItemId") == new Guid(HfItemID)).FirstOrDefault();
                                _LRows.Add(DR);
                            }
                            DataTable _Table = _LRows.CopyToDataTable();
                        }
                        //dt = _Table;
                        int j = 0;
                        bool IsFPOChk = false;
                        bool IsLPOChk = false;
                        string LPOItems = "", LPOItms1 = "";
                        string FPOItems = "";
                        string Error = "";
                        for (int i = 0; i < dsi.Tables[1].Rows.Count; i++)
                        {

                            //CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");
                            bool cb = Convert.ToBoolean(dsi.Tables[1].Rows[i]["IsCheck"].ToString());
                            string HfItemID = dsi.Tables[1].Rows[i]["ItemID"].ToString();
                            if (!al.Contains(-1) && al.Contains(new Guid(HfItemID)) && Session["REQID"] != null)
                                cb = true;
                            else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID)) &&
                                !al1.Contains(new Guid(HfItemID)))
                                cb = true;

                            else
                            {
                                ItmChkbx = false;
                                cb = false;
                                if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID)))
                                {
                                    FPOItems += " " + (i + 1) + ", ";
                                    IsFPOChk = true;
                                }
                                else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID)))
                                {
                                    LPOItems += " " + (i + 1) + ", ";
                                    IsLPOChk = true;
                                }
                                else
                                    Error = "Error while Checking.";
                            }

                            if (cb == true && dt.Rows.Count > 0 && dt.Rows[i]["LPONumberraised"] != null)
                            {
                                j = i - dt.Rows.Count;
                                string ItmQty = dt.Rows[i]["Quantity"].ToString();
                                string ItmCQty = dt.Rows[i]["CQuantity"].ToString();
                                if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0") && dt.Rows[i]["ItemId"].ToString() == HfItemID
                                    && dt.Rows[i]["LPOCancel"].ToString() != "True")
                                    cb = false;
                                else if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0") && dt.Rows[i]["ItemId"].ToString() == HfItemID
                                    && dt.Rows[i]["LPOCancel"].ToString() == "True")
                                    cb = true;
                                else
                                    cb = true;
                                j++;
                            }
                            dsi.Tables[1].Rows[i]["IsCheck"] = cb;
                        }




                        if (IsLPOChk && IsFPOChk)
                        {
                            ErrMsg = "LPO is generated for Item No(s)" + LPOItems.Trim(',') +
                                 "And Item(s) " + FPOItems.Trim(',') + " are not selected in FPO.";
                            ScriptManager.RegisterStartupScript(this.Page,
                                this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item No(s)" + LPOItems.Trim(',') +
                                 "And Item(s) " + FPOItems.Trim(',') + " are not selected in FPO.');", true);
                            Session["LPOCheckAllBoxes"] = false;
                        }
                        else if (IsFPOChk == true && IsLPOChk == false)
                        {
                            ErrMsg = "Item(s) " + FPOItems.Trim(',')
                                + " are not selected in FPO.";
                            ScriptManager.RegisterStartupScript(this.Page,
                                this.GetType(), "ShowAll3", "ErrorMessage('Item(s) " + FPOItems.Trim(',')
                                + " are not selected in FPO.');", true);
                        }
                        else if (IsFPOChk == false && IsLPOChk == true)
                        {
                            ErrMsg = "LPO is generated for Item(s)" + LPOItems.Trim(',') + " ";
                            ScriptManager.RegisterStartupScript(this.Page,
                                this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItems.Trim(',') + " ');", true);
                        }
                        else if (IsLPOChk == true)
                        {
                            ErrMsg = "LPO is generated for Item(s)" + LPOItems.Trim(',') + " ";
                            ScriptManager.RegisterStartupScript(this.Page,
                               this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItems.Trim(',') + " ');", true);
                        }
                        //for (int i = 0; i < gvLpoItems.Rows.Count; i++)
                        //{
                        //    //CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");
                        //    //if (i >= dt.Rows.Count)
                        //    //    j = i - dt.Rows.Count;
                        //    //string ItmQty = dt.Rows[j]["Quantity"].ToString();
                        //    //string ItmCQty = dt.Rows[j]["CQuantity"].ToString();
                        //    //if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0"))
                        //    //{
                        //    //    cb.Checked = false;
                        //    //    LPOItms1 += " " + (j + 1) + ", ";
                        //    //    IsLPOChk = true;
                        //    //}
                        //    //j++;
                        //}



                    }
                    else
                    {
                        for (int q = 0; q < dsi.Tables[1].Rows.Count; q++)
                        {
                            bool IsCheck = bool.Parse(dsi.Tables[1].Rows[q]["IsCheck"].ToString());
                            IsCheck = true;
                            dsi.Tables[1].Rows[q]["IsCheck"] = IsCheck;
                        }
                    }


                }
                else
                {
                    for (int j = 0; j < dsi.Tables[1].Rows.Count; j++)
                    {
                        bool IsCheck = bool.Parse(dsi.Tables[1].Rows[j]["IsCheck"].ToString());
                        IsCheck = false;
                        dsi.Tables[1].Rows[j]["IsCheck"] = IsCheck;
                    }
                }

                //Session["Quantity"] = dsi.Tables[4];
                Session["LPOGrid"] = dsi;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty2", "CHeck('chkExdt', 'dvExdt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "CHeck('chkPkng', 'dvPkng');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt2", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx2", "CHeck('chkSltx', 'dvSltx');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
                //if (ItmChkbx)
                Session["LPOCheckAllBoxes"] = ItmChkbx;
                Session["PopUpErrMsg"] = ErrMsg;
                //else
                //    Session["LPOCheckAllBoxes"] = ItmChkbx;
                Session["CPage_LPO"] = (CurrentPage);
                Session["RowsDisplay_LPO"] = txtRowsChanged;
                return FillGridView();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return FillGridView();
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckIndividualBoxs(string Chck, int Rno, string CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["PopUpErrMsg"] = null;
                Rno = Rno - 1;
                bool ItmChkbx = bool.Parse(Chck);
                ArrayList al = DT2Array();
                ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                DataSet dsi = (DataSet)Session["LPOGrid"];
                string ErrMsg = string.Empty;
                //int GvRowCount = gvLpoItems.Rows.Count;
                //GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                //int rowIndex = Convert.ToInt32(row.RowIndex);
                DataTable dt = (DataTable)Session["Quantity"];

                string HfItemID = dsi.Tables[1].Rows[Rno]["ItemID"].ToString();
                bool IsCheck = Convert.ToBoolean(dsi.Tables[1].Rows[Rno]["IsCheck"].ToString());
                if (ItmChkbx) //&& GvRowCount == Convert.ToInt16(dt.Rows.Count.ToString()) // removed by dinesh 08/06/2016 for Item check
                {

                    // //int RIndex =dt.Rows.Count - rowIndex;
                    //string ItmQty = dt.Rows[rowIndex]["Quantity"].ToString();// by dinesh on 08062016 for Item Checkbox check
                    // string ItmCQty = dt.Rows[rowIndex]["CQuantity"].ToString(); //by dinesh on 08062016 for Item Checkbox check
                    if (Session["IsCustomer"] == "false")
                    {
                        //CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");

                        if (!al.Contains(-1) && al.Contains(new Guid(HfItemID)) && Session["REQID"] != null)
                            IsCheck = true;
                        else if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID)) && !al1.Contains(new Guid(HfItemID)))
                            // && (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0") // by dinesh on 08062016 for Item Checkbox check
                            IsCheck = true;
                        else
                        {
                            IsCheck = false;
                            if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID)))
                            {
                                ErrMsg = "This Item Was Not Selected in FPO.";
                                ScriptManager.RegisterStartupScript(this.Page,
                                    this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected in FPO.');", true);
                            }
                            else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID)))
                            {
                                //|| (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0") //by dinesh on 08062016 for Item Checkbox check
                                ErrMsg = "This Item Was already Selected in Another LPO.";
                                ScriptManager.RegisterStartupScript(this.Page,
                                    this.GetType(), "ShowAll3", "ErrorMessage('This Item Was already Selected in Another LPO.');", true);
                            }
                            else
                            {
                                ErrMsg = "Error while Checking.";
                                ScriptManager.RegisterStartupScript(this.Page,
                                    this.GetType(), "ShowAll2", "ErrorMessage('Error while Checking.');", true);
                            }
                        }
                    }
                    else
                    {
                        //CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                        IsCheck = true;
                    }
                }
                else
                {
                    if (Session["IsCustomer"] == "false")
                    {
                        //CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                        IsCheck = false;

                        if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID)))
                        {
                            ErrMsg = "This Item Was Not Selected.";
                            ScriptManager.RegisterStartupScript(this.Page,
                                this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected.');", true);
                        }
                    }
                    else
                    {
                        //CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                        IsCheck = false;
                    }
                }
                dsi.Tables[1].Rows[Rno]["IsCheck"] = IsCheck;
                Session["PopUpErrMsg"] = ErrMsg;
                Session["LPOGrid"] = dsi;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty2", "CHeck('chkExdt', 'dvExdt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "CHeck('chkPkng', 'dvPkng');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt2", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx2", "CHeck('chkSltx', 'dvSltx');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true);
                Session["CPage_LPO"] = (CurrentPage);
                Session["RowsDisplay_LPO"] = txtRowsChanged;
                return FillGridView();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return FillGridView();
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ValidateRowsBeforeSave()
        {
            try
            {
                string Error = "";
                DataSet ds = (DataSet)Session["LPOGrid"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    DataRow[] dr = ds.Tables[1].Select("ISCheck = 'true' and Quantity = '0'");
                    if (dr.Length > 0)
                    {
                        string SNO = dr[0]["fesno"].ToString();
                        string qty = dr[0]["Quantity"].ToString();
                        string rate = dr[0]["Rate"].ToString();
                        string ErrorMsg = "";
                        decimal number;
                        if (!Decimal.TryParse(qty, out number))
                            ErrorMsg = " Quantity ";
                        else if (number == 0)
                            ErrorMsg = " Quantity cannot be Zero";
                        else if (!Decimal.TryParse(rate, out number))
                            ErrorMsg = " Rate ";
                        else if (number == 0)
                            ErrorMsg = " Rate cannot be Zero";

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
        public string AddNewRow(int RNo, string Qty, string Rates)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LPOGrid"];
                decimal Rate = Convert.ToDecimal(Rates);
                decimal DiscountPerc = Convert.ToDecimal((ds.Tables[1].Rows[RNo - 1]["DiscountPercentage"] != "" || ds.Tables[1].Rows[RNo - 1]["DiscountPercentage"] != DBNull.Value) ?
                   ds.Tables[1].Rows[RNo - 1]["DiscountPercentage"] : 0);
                decimal ExDiscountPerc = Convert.ToDecimal((ds.Tables[1].Rows[RNo - 1]["ExDutyPercentage"] != "" || ds.Tables[1].Rows[RNo - 1]["ExDutyPercentage"] != DBNull.Value) ?
                   ds.Tables[1].Rows[RNo - 1]["ExDutyPercentage"] : 0);
                if (Qty != "")
                {
                    if (Convert.ToBoolean(HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["IsAm"]) == true)
                    {
                        if ((Convert.ToDecimal(ds.Tables[1].Rows[RNo - 1]["Qty"].ToString()) < Convert.ToDecimal(Qty)
                    || Convert.ToDecimal(ds.Tables[1].Rows[RNo - 1]["Qty"].ToString()) == Convert.ToDecimal(Qty)))
                        {
                            ds.Tables[1].Rows[RNo - 1]["Qty"] = Qty;
                            ds.Tables[1].Rows[RNo - 1]["Amount"] = (Convert.ToDecimal(Qty) * Rate);
                            ds.Tables[1].Rows[RNo - 1]["totalAmt"] = (Convert.ToDecimal(Qty) * Rate);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Item Quantity shouldn't be less with raised FPO.');", true);
                    }
                    else
                    {
                        ds.Tables[1].Rows[RNo - 1]["Qty"] = Qty;
                        ds.Tables[1].Rows[RNo - 1]["Amount"] = (Convert.ToDecimal(Qty) * Rate);
                        ds.Tables[1].Rows[RNo - 1]["totalAmt"] = (Convert.ToDecimal(Qty) * Rate);
                        if (DiscountPerc == 0)
                        {
                            ds.Tables[1].Rows[RNo - 1]["totalAmt"] = (Convert.ToDecimal(Qty) * Rate).ToString("N");
                        }
                        else
                        {
                            ds.Tables[1].Rows[RNo - 1]["totalAmt"] = ((Convert.ToDecimal(Qty) * Rate) - (Convert.ToDecimal(Qty) * Rate) * (DiscountPerc / 100)).ToString("N");
                        }
                        if (ExDiscountPerc != 0)
                        {
                            ds.Tables[1].Rows[RNo - 1]["totalAmt"] = ((Convert.ToDecimal(ds.Tables[1].Rows[RNo - 1]["totalAmt"]) + (Convert.ToDecimal(ds.Tables[1].Rows[RNo - 1]["totalAmt"]) * (ExDiscountPerc / 100)))).ToString("N");
                        }
                    }
                }
                ds.AcceptChanges();
                //Session["Quantity"] = ds.Tables[4];
                Session["LPOGrid"] = ds;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return FillGridView();
        }


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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CalculateExDuty(string ExDuty, string SGST, string IGST, string Discount, string Packing, string SalesTax, string AdsnlChrgs, bool chkDsnt,
            bool chkExdt, bool chkSGST, bool chkIGST, bool chkSltx, bool chkPkng, bool chkAdsnlChrgs, string rdbDscnt, string rdbExDty, string rdbSGST, string rdbIGST, string rdbPkg, string rdbAdd)
        {
            try
            {
                #region Items
                decimal Gtotal = 0;
                DataSet ds = (DataSet)Session["LPOGrid"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    decimal DiscountAmount = 0;
                    decimal Amount = 0, GTotalAmount = 0;
                    decimal PackageAmt = 0, PackageAmt1 = 0;
                    decimal SalesTaxAmt = 0;
                    decimal ExAmt = 0, ExAmt1 = 0;
                    decimal SGSTAmt = 0, SGSTAmt1 = 0;
                    decimal IGSTAmt = 0, IGSTAmt1 = 0;
                    decimal totalval = 0, totaldisc = 0;
                    decimal AdsnlChargesAmt = 0, AdsnlChargesAmt1 = 0;
                    int dbcount = ds.Tables[1].Rows.Count; decimal DbDisc = 0;
                    if (Convert.ToDecimal(Discount) >= 0 && Convert.ToInt32(rdbDscnt) == 1)
                        //DbDisc = Convert.ToDecimal(Discount) / dbcount;
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            totalval = totalval + ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())));
                        }
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        Amount = 0; GTotalAmount = 0;
                        if (Convert.ToBoolean(ds.Tables[1].Rows[i]["IsCheck"].ToString()))
                        {
                            if (FlagDiscount == 0)
                            {
                                /* if ((Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) > 0 || Convert.ToDecimal(Discount) > 0) && Convert.ToInt32(rdbDscnt) == 0)
                                 {
                                     if (Amount == 0)
                                         Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
                                     DiscountAmount =( ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) *
                                         (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100 + Convert.ToDecimal(Discount));
                                     Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) - DiscountAmount;
                                     GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                     Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) - ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                         (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) + Convert.ToDecimal(Discount))) / 100;
                                 }*/
                                if ((Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) > 0 || Convert.ToDecimal(Discount) > 0) && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    if (Amount == 0)
                                        Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
                                    DiscountAmount = (((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100 + ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * (Convert.ToDecimal(Discount))) / 100));
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) - ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) + Convert.ToDecimal(Discount))) / 100;
                                }
                                else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) -
                                       ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                       (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                }
                                else if ((Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) > 0) && Convert.ToInt32(rdbDscnt) == 0 && chkDsnt == true)
                                {
                                    // if (Amount == 0)
                                    //   Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
                                    //  DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) - (Convert.ToDecimal(Discount)));
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100 + (Convert.ToDecimal(DbDisc));
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) -
                                       ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                       (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                }
                                else if (Convert.ToDecimal(Discount) >= 0 && Convert.ToInt32(rdbDscnt) == 1)
                                {
                                    totaldisc = totalval - (Convert.ToDecimal(Discount));
                                    DbDisc = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * (Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()))) / totalval) * (Convert.ToDecimal(Discount));
                                    // if (Amount == 0)
                                    //   Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
                                    //  DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) - (Convert.ToDecimal(Discount)));
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) * (Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) * (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100 + (Convert.ToDecimal(DbDisc));
                                    Amount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) * (Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()))) - DiscountAmount) / Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString());
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) -
                                       ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                       (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]);
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                   Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()));
                                }
                            }
                            else
                            {
                                if (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                   Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) - ((Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString())) *
                                       (Convert.ToDecimal(ds.Tables[1].Rows[i]["DiscountPercentage"].ToString()) + Convert.ToDecimal(Discount))) / 100;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"]);
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                 Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()));
                                }
                            }

                            if (chkPkng != false && Convert.ToDecimal(Packing) > 0 && Convert.ToInt32(rdbPkg) == 0)
                            {
                                PackageAmt = (Amount * Convert.ToDecimal(Packing)) / 100;
                                PackageAmt1 = (GTotalAmount * Convert.ToDecimal(Packing)) / 100;
                            }
                            if (chkAdsnlChrgs != false && Convert.ToDecimal(AdsnlChrgs) > 0 && Convert.ToInt32(rdbAdd) == 0)
                            {
                                AdsnlChargesAmt = (Amount * Convert.ToDecimal(AdsnlChrgs)) / 100;
                                AdsnlChargesAmt1 = (GTotalAmount * Convert.ToDecimal(AdsnlChrgs)) / 100;
                            }
                            Amount = Amount + PackageAmt + AdsnlChargesAmt;
                            GTotalAmount = GTotalAmount + PackageAmt1 + AdsnlChargesAmt1;
                            if (FlagExDuty == 0)
                            {
                                if (Convert.ToDecimal(ds.Tables[1].Rows[i]["ExDutyPercentage"].ToString()) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    if (Amount == 0)
                                        Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
                                    ExAmt = (Amount * (Convert.ToDecimal(ds.Tables[1].Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                    ExAmt1 = (GTotalAmount * (Convert.ToDecimal(ds.Tables[1].Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                }
                                else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    ExAmt1 = (GTotalAmount * Convert.ToDecimal(ExDuty)) / 100;
                                }
                                else if (Convert.ToInt32(rdbExDty) == 0)
                                {
                                    ExAmt = 0; ExAmt1 = 0;
                                }

                                if (chkSGST != false && Convert.ToDecimal(SGST) > 0 && Convert.ToInt32(rdbSGST) == 0)
                                {
                                    SGSTAmt = (Amount * Convert.ToDecimal(SGST)) / 100;
                                    SGSTAmt1 = (GTotalAmount * Convert.ToDecimal(SGST)) / 100;
                                }
                                else if (Convert.ToInt32(rdbSGST) == 0)
                                {
                                    SGSTAmt = 0;
                                    SGSTAmt1 = 0;
                                }

                                if (chkIGST != false && Convert.ToDecimal(IGST) > 0 && Convert.ToInt32(rdbIGST) == 0)
                                {
                                    IGSTAmt = (Amount * Convert.ToDecimal(IGST)) / 100;
                                    IGSTAmt1 = (GTotalAmount * Convert.ToDecimal(IGST)) / 100;
                                }
                                else if (Convert.ToInt32(rdbIGST) == 0)
                                {
                                    IGSTAmt = 0; IGSTAmt1 = 0;
                                }
                            }
                            else
                            {
                                Amount = Convert.ToDecimal(Amount + ((Amount * Convert.ToDecimal(ds.Tables[0].Rows[i]
                                      ["ExDutyPercentage"].ToString()))
                                      / 100));
                                GTotalAmount = Convert.ToDecimal(GTotalAmount + ((GTotalAmount * Convert.ToDecimal(ds.Tables[0].Rows[i]
                                       ["ExDutyPercentage"].ToString()))
                                       / 100));
                            }
                            Amount = Amount + ExAmt + SGSTAmt + IGSTAmt;
                            GTotalAmount = GTotalAmount + ExAmt1 + SGSTAmt1 + IGSTAmt1;

                            if (Amount > 0)
                            {
                                ds.Tables[1].Rows[i]["QPrice"] = Math.Round(Amount, 2);
                                //if (Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 1)
                                //    GTotalAmount = GTotalAmount - Convert.ToDecimal(Discount);

                                if (Convert.ToDecimal(Discount) >= 0)
                                {
                                    ds.Tables[1].Rows[i]["totalAmt"] = Math.Round(Amount *
                                        Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()), 2);
                                    ds.Tables[1].Rows[i]["Amount"] = Math.Round(Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                        Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()), 2);
                                }
                                else
                                {
                                    ds.Tables[1].Rows[i]["totalAmt"] = Math.Round((GTotalAmount - DbDisc), 2);
                                    ds.Tables[1].Rows[i]["Amount"] = Math.Round(Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                        Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()), 2);
                                }
                            }
                            else
                            {
                                Amount = Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString());
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
                                ds.Tables[1].Rows[i]["QPrice"] = Math.Round(Amount, 2);
                                //if (Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 1)
                                //    GTotalAmount = GTotalAmount - Convert.ToDecimal(Discount);
                                ds.Tables[1].Rows[i]["totalAmt"] = Math.Round((GTotalAmount - DbDisc), 2);
                                ds.Tables[1].Rows[i]["Amount"] = Math.Round(Convert.ToDecimal(ds.Tables[1].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[1].Rows[i]["Qty"].ToString()), 2);
                                Amount = 0;
                            }
                        }
                    }

                    ds.AcceptChanges();
                    Session["LPOGrid"] = ds;
                }

                #endregion

                return FillGridView();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
                return FillGridView();
            }
        }




        #endregion
    }
}
