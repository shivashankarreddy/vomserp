using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using VOMS_ERP.Admin;
using System.Collections;
using System.IO;
using System.Text;
using Ajax;

namespace VOMS_ERP.Customer_Access
{
    public partial class Customer_LPO : System.Web.UI.Page
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(Customer_LPO));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            HideUnwantedFields();
                            GetData();

                            Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            if ((string[])Session["UsrPermissions"] != null &&
                                ((string[])Session["UsrPermissions"]).Contains("Edit") && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    DivComments.Visible = true;
                                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Text = "Save";
                            }
                            else
                                Response.Redirect("../Masters/CHome.aspx?NP=no", false);
                        }
                        else
                            if (Session["PaymentTermsLPO"] != null)
                                divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                Guid LQID = new Guid(ListBoxLPO.SelectedValue);
                Guid FEID = new Guid(ListBoxFEO.SelectedValue);
                // string FPOIDs = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LQIDs = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FEIDs = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Guid LPOID = Guid.Empty;
                if (Request.QueryString["ID"] != null)
                    LPOID = new Guid(Request.QueryString["ID"]);
                ItemStatusBLL ISBLL = new ItemStatusBLL();
                ds = ISBLL.GetItemStatus(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, LPOID, CommonBLL.StatusTypeLclQuotID, FEIDs, "", LQIDs, "", "", "");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                BindDropDownList(ddlCustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                //if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                //    BindDropDownList(ddlCustomer, cusmr.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));

                if (Request.QueryString["ID"] != null && Request.QueryString["CsID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CustID"];
                    //LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(Request.QueryString["CustID"].ToString()),
                    //    Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "",
                    //    Guid.Empty, "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                    //    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    //    CommonBLL.ATConditions()));
                    ddlCustomer.Enabled = false;
                }
                else
                {
                    ddlCustomer.SelectedIndex = 1;
                    CustomerSelectionChanged();
                }

                BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "" && Request.QueryString["CsID"] != null)
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CsID"];
                    string FeID = Request.QueryString["FeqID"];
                    LocalPurchaseOrder(ListBoxFEO, NEBL.NewEnquiryEdit(CommonBLL.FlagGSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "",
                          DateTime.Now, DateTime.Now, DateTime.Now, "", 100, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                        CommonBLL.EmptyDt()));

                    string[] FeLength = Request.QueryString["FeqID"].Split(',');
                    //for (int i = 0; i < FeLength.Length; i++)
                    //{
                    //    ListItem item = ListBoxFEO.Items.FindByValue(FeLength[i].ToString());
                    //    if (item != null)
                    //    {
                    //        ListBoxFEO.SelectedValue = FeLength[i].ToString();
                    //    }
                    //}
                    //       
                    foreach (string s in FeLength)
                    {
                        foreach (ListItem item in ListBoxFEO.Items)
                        {
                            if (item.Value == s) item.Selected = true;
                        }
                    }

                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                    #region Unused Check IF Problem

                    //LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagRegularDRP, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    //    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    //    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                    //    new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    //    CommonBLL.ATConditions()));
                    //string[] FPOLength = Request.QueryString["FpoID"].Split(',');
                    //for (int i = 0; i < FPOLength.Length; i++)
                    //{
                    //    ListItem item = ListBoxFPO.Items.FindByValue(FPOLength[i].ToString());
                    //    if (item != null)
                    //    {
                    //        ListBoxFPO.SelectedValue = FPOLength[i].ToString();
                    //    }
                    //}

                    //LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty, Guid.Empty, DateTime.Now,
                    //    Guid.Empty, DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                    //    100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                    //    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));


                    //*********** 04/02/2016 Commented Dinesh
                    //LocalPurchaseOrder(ListBoxFEO, NEBL.NewEnquiryEdit(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", "",
                    //      DateTime.Now, DateTime.Now, DateTime.Now, "", 100, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                    //    CommonBLL.EmptyDt()));
                    //********End
                    #endregion

                    //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    //ListBoxFEO.Enabled = false;
                    //string feno = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                    BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, FeID, DateTime.Now, DateTime.Now,
                        100, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));
                    FillInputFields(FeID, Request.QueryString["CsID"]);
                }
                else
                {
                    ddlCustomer.SelectedIndex = 1;
                    CustomerSelectionChanged();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagXSelect, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                                                DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(),
                                                CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                    && EditDS.Tables[2].Rows.Count > 0)
                {
                    string feno = EditDS.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();
                    string CustID = EditDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlCustomer.SelectedValue = CustID;

                    BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                    BindDropDownList(ddlRsdby, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                    //LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagJSelect, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    //    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    //    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                    //    new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    //    CommonBLL.ATConditions()));

                    hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["EnquiryDate"].ToString()));

                    LocalPurchaseOrder(ListBoxFEO, NEBL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "",
                            DateTime.Now, DateTime.Now, DateTime.Now, "", 80, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                            CommonBLL.EmptyDt()));
                    string[] fenmbrs = feno.Split(',');
                    foreach (ListItem item in ListBoxFEO.Items)
                    {
                        foreach (string s in fenmbrs)
                        {
                            if (item.Value == s.ToLower())
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                    //LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "",
                    //    Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, FpoNo,
                    //    "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 80, Guid.Empty, false, false, "",
                    //    new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO,
                    //    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));

                    //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    string fenumber = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    if (EditDS.Tables.Count > 3)
                    {
                        ViewState["Quantity"] = EditDS.Tables[3];
                    }
                    //BindDropDownList(ddlSuplr, (NFPOBL.Select(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, FpoNo, "", DateTime.Now,
                    //    DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 80, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()),
                    //    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    //    CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()))).Tables[2]);

                    BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, feno, DateTime.Now, DateTime.Now,
                    80, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));

                    FillInputFields(feno, CustID);
                    ddlSuplr.SelectedValue = EditDS.Tables[0].Rows[0]["SupplierId"].ToString();
                    LocalPurchaseOrder(ListBoxLPO, (NLPOBL.SelectLPOrders(CommonBLL.FlagPSelectAll, Guid.Empty, Guid.Empty, "", fenumber, Guid.Empty,
                        Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, new Guid(EditDS.Tables[0].Rows[0]["SupplierId"].ToString()), "",
                        DateTime.Now, 0, 80, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "")));
                    ListBoxLPO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    if (EditDS.Tables[1].Columns.Contains("Quantity"))
                    {
                        DataColumn Chk = new DataColumn("Check", typeof(bool));
                        Chk.DefaultValue = false;
                        EditDS.Tables[1].Columns["Quantity"].ColumnName = "qty";
                        EditDS.Tables[3].Columns.Add(Chk);

                        if (EditDS.Tables[4].Rows.Count > 0)
                        {
                            for (int i = 0; i < EditDS.Tables[4].Rows.Count; i++)
                            {
                                if (EditDS.Tables[3].Select("ItemId ='" + EditDS.Tables[4].Rows[i]["ItemId"] + "' and FESNo=" + EditDS.Tables[4].Rows[i]["FESNo"]).Count() > 0)
                                {
                                    for (int j = 0; j < EditDS.Tables[3].Rows.Count; j++)
                                    {
                                        if (EditDS.Tables[3].Rows[j]["ItemId"].ToString() == EditDS.Tables[4].Rows[i]["ItemID"].ToString() && EditDS.Tables[3].Rows[j]["FESNO"].ToString() == EditDS.Tables[4].Rows[i]["FESNO"].ToString())
                                        {
                                            EditDS.Tables[3].Rows[j]["Check"] = true;
                                            //Decimal ActQty = Convert.ToDecimal(EditDS.Tables[3].Rows[j]["CQuantity"]);
                                            //Decimal PrevQty = Convert.ToDecimal(EditDS.Tables[4].Rows[i]["CQuantity"]);
                                            //Decimal TQty = ActQty - PrevQty;
                                            EditDS.Tables[3].Rows[j]["Quantity"] = EditDS.Tables[4].Rows[i]["Quantity"];
                                            EditDS.Tables[3].Rows[j]["CQuantity"] = EditDS.Tables[4].Rows[i]["CQuantity"];
                                        }
                                    }
                                }
                            }
                            EditDS.Tables[3].AcceptChanges();
                            ViewState["Quantity"] = EditDS.Tables[3];
                        }
                    }

                    DataTable TOTAL_QTY_L = null;
                    if (EditDS.Tables.Count > 5)
                        TOTAL_QTY_L = EditDS.Tables[5];
                    Session["TOTAL_QTY_L"] = TOTAL_QTY_L;

                    DataView dv = EditDS.Tables[1].DefaultView;
                    dv.Sort = "FESNO";
                    DataTable sortedDT = dv.ToTable();


                    //EditDS.Tables[1].Columns.Add("QTYsum", typeof(decimal));//Need to Remove This Column Before Saving.
                    foreach (DataRow row in sortedDT.Rows)
                    {
                        if ((Convert.ToDecimal(row["qty"].ToString()) != Convert.ToDecimal(row["QTYsum"].ToString())) && TOTAL_QTY_L != null && TOTAL_QTY_L.Rows.Count > 0)
                        {
                            DataRow rowsToUpdate = TOTAL_QTY_L.AsEnumerable().FirstOrDefault(r => r.Field<Guid>("itemid") == row.Field<Guid>("itemid"));
                            if (rowsToUpdate != null)
                            {
                                decimal qty = (rowsToUpdate.Field<decimal>("QTY") - row.Field<decimal>("qty"));
                                row.SetField("qty", qty > 0 ? qty : 0);
                                row.SetField("amount", Math.Abs(row.Field<decimal>("qty") * row.Field<decimal>("rate")));
                                row.SetField("QTYsum", Math.Abs(Convert.ToDecimal(row.Field<decimal>("qty"))));
                            }
                            else
                                row.SetField("qty", Math.Abs(row.Field<decimal>("qty")));
                        }
                        else
                            row.SetField("qty", Math.Abs(row.Field<decimal>("qty")));
                    }
                    Session["CustomerLPOGrid"] = sortedDT;
                    //gvLpoItems.DataSource = sortedDT;
                    //gvLpoItems.DataBind();
                    ViewState["IsChecked_CLPO"] = true;
                    BindGridVeiw(gvLpoItems, sortedDT);

                    if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                        gvLpoItems.Columns[11].Visible = false;

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
                        //chkExdt.Checked = true;
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", true);
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

                    ListBoxFEO.Enabled = false;
                    ddlSuplr.Enabled = false;
                    ListBoxLPO.Enabled = false;
                    ViewState["EditID"] = ID;
                    if (Request.QueryString["ID"] != null && Request.QueryString["IsAm"] == "True")
                        btnSave.Text = "Save";
                    else
                        btnSave.Text = "Update";
                }
                else
                {
                    ddlSuplr.Enabled = false;
                    ListBoxLPO.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Foreign Purchase Order", ex.Message.ToString());
            }
            return dtt;
        }

        /// <summary>
        /// To fill input fields in the form
        /// </summary>
        /// <param name="FpoID"></param>
        private void FillInputFields(string FEID, string CustID)
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                DataSet LQDetails = NFPOBL.SelectItemSet(CommonBLL.FlagPSelectAll, Guid.Empty, FEID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                    Guid.Empty.ToString(), DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0,
                    0, 80, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions());

                if (LQDetails.Tables[0].Rows.Count > 0)
                {
                    txtsubject.Text = LQDetails.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = LQDetails.Tables[0].Rows[0]["Instruction"].ToString();
                    txtDlvry.Text = LQDetails.Tables[0].Rows[0]["DeliveryPeriods"].ToString();
                    ddlRsdby.SelectedValue = LQDetails.Tables[0].Rows[0]["DepartmentId"].ToString();
                    //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                    ddlPrcBsis.SelectedValue = LQDetails.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = LQDetails.Tables[0].Rows[0]["PriceBasisText"].ToString();
                    hdfldCstmr.Value = LQDetails.Tables[0].Rows[0]["CustomerId"].ToString();
                    hfFPODt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(LQDetails.Tables[0].Rows[0]["QuotationDate"].ToString()));
                    txtLpoDueDt.Text = DateTime.Now.ToString("dd-MM-yyyy");

                }
                if (LQDetails.Tables[1].Rows.Count > 0)
                {
                    if (Request.QueryString["IsAm"] == "")
                    {
                        ComparisonDTS.Tables.Add(LQDetails.Tables[1].Copy());
                        ViewState["ComparisonDTS"] = ComparisonDTS;
                    }

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="gvItems"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(DataTable DT)
        {
            DataTable dts = CommonBLL.EmptyDtLPOrders();
            dts.Rows[0].Delete(); int exdt = 0, dscnt = 0;
            //foreach (GridViewRow row in gvItems.Rows)
            //{
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                DataRow dr;
                if (Convert.ToBoolean(DT.Rows[i]["IsItemCheck"].ToString()))
                {
                    dr = dts.NewRow();
                    dr["ItemDetailsID"] = DT.Rows[i]["ItemDetailsId"];//new Guid(((Label)row.FindControl("lblItemDetailsID")).Text);
                    dr["SNo"] = DT.Rows[i]["FESNo"];//Convert.ToInt32(((HiddenField)row.FindControl("hfFESNo")).Value);
                    dr["ItemId"] = DT.Rows[i]["ItemId"];//new Guid(((Label)row.FindControl("lblItemID")).Text);
                    dr["LPOrderId"] = DT.Rows[i]["ItemDetailsId"] == "0" ? Guid.Empty : DT.Rows[i]["ItemDetailsId"];//(((Label)row.FindControl("lblLPOrderId")).Text == "0" ? Guid.Empty : new Guid(((Label)row.FindControl("lblLPOrderId")).Text));
                    dr["PartNumber"] = DT.Rows[i]["PartNumber"];//((Label)row.FindControl("lblPrtNo")).Text;
                    dr["Specifications"] = DT.Rows[i]["Specifications"];//((Label)row.FindControl("lblSpec")).Text;
                    dr["Make"] = DT.Rows[i]["Make"];//((Label)row.FindControl("lblMake")).Text;
                    dr["Quantity"] = string.IsNullOrWhiteSpace((DT.Rows[i]["qty"].ToString())) ? "0" : DT.Rows[i]["qty"];//(String.IsNullOrWhiteSpace(((TextBox)row.FindControl("txtQty")).Text) ? 0 : Convert.ToDecimal(((TextBox)row.FindControl("txtQty")).Text));     //Convert.ToDecimal(((Label)row.FindControl("lblQty")).Text);
                    dr["Rate"] = DT.Rows[i]["rate"];//Convert.ToDecimal(((HiddenField)row.FindControl("HF_CngPrice")).Value);
                    dr["Amount"] = DT.Rows[i]["Amount"];//Convert.ToDecimal(((Label)row.FindControl("lblAmount")).Text);
                    if (Convert.ToInt32(DT.Rows[i]["ExDutyPercentage"]) > 0)
                    {
                        if (DT.Rows[i]["ExDutyPercentage"] == "")
                            dr["ExDutyPercentage"] = 0;
                        else
                            dr["ExDutyPercentage"] = DT.Rows[i]["ExDutyPercentage"];//Convert.ToDecimal(((Label)row.FindControl("lblExdtPrcnt")).Text);
                    }
                    else
                        dr["ExDutyPercentage"] = 0;
                    if (DT.Rows[i]["DiscountPercentage"] == "")
                        dr["DiscountPercentage"] = 0;
                    else
                        dr["DiscountPercentage"] = Convert.ToDecimal(DT.Rows[i]["DiscountPercentage"]);//Convert.ToDecimal(((Label)row.FindControl("lblDscntPrcnt")).Text);
                    dr["UNumsId"] = DT.Rows[i]["UNumsId"];//new Guid(((Label)row.FindControl("lblUnitID")).Text);
                    dr["Remarks"] = DT.Rows[i]["Remarks"]; //((Label)row.FindControl("lblRmrks")).Text;
                    dts.Rows.Add(dr);
                }
            }
            return dts;
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
                        //ECEA_spn_txt.Visible = ChkbCEEApl.Visible = false;
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowCEEAprls", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ListBoxFEO.SelectedIndex = ListBoxFEO.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlSuplr.SelectedIndex = ListBoxLPO.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                txtDlvry.Text = txtimpinst.Text = txtLpoDt.Text = txtLpoDueDt.Text = txtLpono.Text = txtsubject.Text = "";
                ChkbInspcn.Checked = ChkbCEEApl.Checked = false; txtLpono.Enabled = false;
                txtDsnt.Text = txtExdt.Text = txtPkng.Text = txtSltx.Text = "0";
                txtLpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(3));
                ddlSuplr.Enabled = true;
                ListBoxLPO.Enabled = true;
                ViewState["EditID"] = null;
                Session["MessageLPO"] = null;
                Session["amountLPO"] = null;
                Session["PaymentTermsLPO"] = null;
                Session["CustomerLPOGrid"] = null;
                ViewState["IsChecked_CLPO"] = false;
                Session["PaymentTermsLPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                gvLpoItems.DataSource = null;
                gvLpoItems.DataBind();
                Session["LPOUploads"] = null;
                ListBoxFEO.Items.Clear();
                ListBoxLPO.Items.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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


        /// <summary>
        /// Bind Grid Veiw 
        /// </summary>
        /// <param name="gvGDN"></param>
        /// <param name="dataTable"></param>
        private void BindGridVeiw(GridView gv, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    Session["CustomerLPOGrid"] = CommonDt;
                    gv.DataSource = CommonDt;
                    gv.DataBind();
                    ((DropDownList)gvLpoItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvLpoItems.PageSize.ToString();
                }
                else
                {
                    Session["CustomerLPOGrid"] = null;
                    gv.DataSource = null;
                    gv.DataBind();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                //DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                //if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                //    EmtpyFPO.Columns.Remove("ItemDetailsId");
                //string fpono = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //if (ListBoxFPO.SelectedItem == null)
                //{
                //    ListBoxLPO.Items.Clear();
                //}
                //gvLpoItems.DataSource = null;
                //gvLpoItems.DataBind();
                //BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                //    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                //LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                //    Guid.Empty, DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                //    100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                //    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                //ListBoxFEO.Enabled = false;
                //ListBoxLPO.Items.Clear();
                //ddlSuplr.Items.Clear();
                //BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagHSelect, Guid.Empty, Guid.Empty, Guid.Empty, fpono, DateTime.Now, DateTime.Now,
                //    100, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));
                //FillInputFields(fpono, ddlCustomer.SelectedValue);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                string feno = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //if (ListBoxFPO.SelectedItem == null)
                //{
                //    ListBoxLPO.Items.Clear();
                //}
                ViewState["IsChecked_CLPO"] = false;
                gvLpoItems.DataSource = null;
                gvLpoItems.DataBind();
                BindDropDownList(ddlsuplrctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                //LocalPurchaseOrder(ListBoxFEO, (NFPOBL.GetDataSet(CommonBLL.FlagQSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now,
                //    Guid.Empty, DateTime.Now, fpono, "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0,
                //    100, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()),
                //    DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions())));
                //ListBoxFEO.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);
                //ListBoxFEO.Enabled = false;
                ListBoxLPO.Items.Clear();
                ddlSuplr.Items.Clear();
                BindDropDownSuppList(ddlSuplr, (LEQBL.GetDataSet(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, feno, DateTime.Now, DateTime.Now,
                    100, Guid.Empty, new Guid(Session["CompanyID"].ToString()))));
                FillInputFields(feno, ddlCustomer.SelectedValue);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ViewState["IsChecked_CLPO"] = false;
                if (ddlSuplr.SelectedValue != Guid.Empty.ToString())
                {
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    // string feno1 = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string feno2 = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet LQuotations = NLPOBL.GetDataSet(CommonBLL.FlagPSelectAll, Guid.Empty, Guid.Empty, "", feno2, Guid.Empty, Guid.Empty,
                        Guid.Empty, DateTime.Now, DateTime.Now, new Guid(ddlSuplr.SelectedValue), "", DateTime.Now, 0, 100, "", new Guid(Session["UserID"].ToString()),
                        CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), path);
                    LocalPurchaseOrder(ListBoxLPO, LQuotations);
                    txtLpono.Text = LQuotations.Tables[3].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + (LQuotations.Tables[1].Rows[0]["LPOSequence"].ToString()).PadLeft(3, '0')
                        + "/" + CommonBLL.GetFinYrShortName();
                    //ListBoxFPO.SelectedItem.Text.Trim() + "/" + CommonBLL.GetFinYrShortName();
                    ddlsuplrctgry.SelectedValue = LQuotations.Tables[2].Rows[0]["CategoryID"].ToString();
                    gvLpoItems.DataBind();
                }
                else
                {
                    ddlPrcBsis.SelectedIndex = -1;
                    divPaymentTerms.InnerHtml = "";
                    txtDlvry.Text = "0";
                    ListBoxLPO.Items.Clear();
                    gvLpoItems.DataSource = null;
                    gvLpoItems.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ViewState["IsChecked_CLPO"] = false;
                if (ListBoxLPO.SelectedValue != "")
                {
                    string flqno = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string feno = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet LclQuoteItems = NLQBL.LPOItemsByMulti(CommonBLL.FlagPSelectAll, new Guid(ListBoxLPO.SelectedValue), flqno, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, new Guid(ddlSuplr.SelectedValue), "", 0, "", Guid.Empty, CommonBLL.EmptyDtLocalQuotation(),
                        CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), Guid.Empty, feno, new Guid(Session["CompanyID"].ToString()));

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
                            ViewState["Quantity"] = LclQuoteItems.Tables[4];

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
                                    decimal qty = 0;

                                    if (rowsToUpdate.Field<decimal>("QTY") > row.Field<decimal>("quantity"))//Received QTY > ActualQTY
                                        qty = rowsToUpdate.Field<decimal>("QTY") - row.Field<decimal>("quantity");
                                    else
                                        qty = row.Field<decimal>("quantity") - rowsToUpdate.Field<decimal>("QTY");

                                    row.SetField("qty", qty > 0 ? qty : 0);
                                    row.SetField("amount", Math.Abs(Convert.ToDecimal(row.Field<string>("qty")) * row.Field<decimal>("rate")));
                                    row.SetField("totalAmt", row.Field<decimal>("amount"));
                                    row.SetField("QTYsum", Math.Abs(Convert.ToDecimal(row.Field<string>("qty"))));
                                }
                                else
                                    row.SetField("qty", Math.Abs(Convert.ToDecimal(row.Field<string>("qty"))));
                            }
                            else
                                row.SetField("qty", Math.Abs(Convert.ToDecimal(row.Field<string>("qty"))));
                        }
                        Session["CustomerLPOGrid"] = LclQuoteItems.Tables[1];
                        //gvLpoItems.DataSource = LclQuoteItems.Tables[1];
                        //gvLpoItems.DataBind();
                        BindGridVeiw(gvLpoItems, LclQuoteItems.Tables[1]);
                        if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.InlineExciseDutyText)))
                            gvLpoItems.Columns[11].Visible = false;

                        ddlPrcBsis.SelectedValue = LclQuoteItems.Tables[0].Rows[0]["PriceBasis"].ToString();
                        txtPriceBasis.Text = LclQuoteItems.Tables[0].Rows[0]["PriceBasisText"].ToString();
                        txtDlvry.Text = LclQuoteItems.Tables[0].Rows[0]["DeliveryPeriods"].ToString();
                        if (txtDlvry.Text != "")
                            txtLpoDueDt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text))));

                        if ((Convert.ToDecimal(LclQuoteItems.Tables[0].Rows[0]["ExDutyPercentage"].ToString()) == 0) && IsExciseDuty == false)
                        {
                            ChkbCEEApl.Enabled = true; ChkbCEEApl.Checked = true;
                            txtCEEApl.Enabled = true;
                            IsExciseDuty = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDutyRmd",
                                "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                        }
                        else
                        {
                            ChkbCEEApl.Enabled = false; ChkbCEEApl.Checked = false;
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
                }
                else
                {
                    divPaymentTerms.InnerHtml = "";
                    txtDlvry.Text = "0";
                    gvLpoItems.DataSource = null;
                    gvLpoItems.DataBind();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                    //ClearAll();
                    ddlSuplr.Items.Clear();
                    DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                    if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                        EmtpyFPO.Columns.Remove("ItemDetailsId");
                    //LocalPurchaseOrder(ListBoxFPO, NFPOBL.GetDataSet(CommonBLL.FlagRegularDRP, Guid.Empty, "", new Guid(ddlCustomer.SelectedValue), Guid.Empty,
                    //    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty,
                    //    "", DateTime.Now, 0, 0, CommonBLL.StatusTypeRepeatedFPO, Guid.Empty, false, false, "", new Guid(Session["UserID"].ToString()),
                    //    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, CommonBLL.FirstRowPaymentTerms(),
                    //    CommonBLL.ATConditions()));
                    LocalPurchaseOrder(ListBoxFEO, NEBL.NewEnquiryEdit(CommonBLL.FlagGSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "",
                            DateTime.Now, DateTime.Now, DateTime.Now, "", 100, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                            CommonBLL.EmptyDt()));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
            GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;
            int rowIndex = Convert.ToInt32(((GridViewRow)txet.Parent.Parent).RowIndex);
            int dtRowINdex = (gvLpoItems.PageIndex * gvLpoItems.PageSize) + rowIndex;

            GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
            Label GTotal = (Label)gvLpoItems.FooterRow.FindControl("lbltmnt");
            Label MTotal = (Label)gvLpoItems.FooterRow.FindControl("lblTotl");
            Label Rate = (Label)row.FindControl("lblRate");
            TextBox CngRate = (TextBox)row.FindControl("txtRate");
            TextBox CngQty = (TextBox)row.FindControl("txtQty");
            Label NetRate = (Label)row.FindControl("lblNRate");
            Label TotalR = (Label)row.FindControl("lblTotal");
            Label Amount = (Label)row.FindControl("lblAmount");
            HiddenField HdnRate = (HiddenField)row.FindControl("HF_CngPrice");

            Decimal TempVal = Convert.ToDecimal(GTotal.Text) - Convert.ToDecimal(Amount.Text);
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
                    GTotal.Text = (TempVal + (Qty * Rte)).ToString();
                    MTotal.Text = (TempVal + (Qty * Rte)).ToString();
                }
                else
                {
                    decimal Rte = Convert.ToDecimal(Rate.Text);
                    decimal Qty = Convert.ToDecimal(txet.Text);
                    Amount.Text = (Qty * Rte).ToString();
                    TotalR.Text = (Qty * Rte).ToString();
                    GTotal.Text = (TempVal + (Qty * Rte)).ToString();
                    MTotal.Text = (TempVal + (Qty * Rte)).ToString();
                }

            }

            DataTable dt = (DataTable)Session["CustomerLPOGrid"];
            dt.Rows[dtRowINdex]["totalAmt"] = TotalR.Text;
            dt.Rows[dtRowINdex]["Amount"] = Convert.ToDecimal(Amount.Text);
            //dt.Rows[dtRowINdex]["DspchQty"] = CrntQty.Text;
            dt.AcceptChanges();
            Session["CustomerLPOGrid"] = dt;

        }


        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnNext = (Button)sender;
                int i = gvLpoItems.PageIndex + 1;
                if (i < gvLpoItems.PageCount)
                    gvLpoItems.PageIndex = i;
                //FillGridView(Guid.Empty);
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                BindGridVeiw(gvLpoItems, DT);

                ((DropDownList)gvLpoItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvLpoItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnPrevious = (Button)sender;
                int i = gvLpoItems.PageCount;
                if (gvLpoItems.PageIndex > 0)
                    gvLpoItems.PageIndex = gvLpoItems.PageIndex - 1;
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                BindGridVeiw(gvLpoItems, DT);
                ((DropDownList)gvLpoItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvLpoItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void ddlPageSize_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlPageSize = (DropDownList)sender;
                gvLpoItems.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                BindGridVeiw(gvLpoItems, DT);
                ((DropDownList)gvLpoItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = ddlPageSize.SelectedValue;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
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
                if (res != 0)
                {
                    decimal exdt = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0;
                    Filename = FileName();
                    DateTime lpoDate = CommonBLL.DateInsert(txtLpoDt.Text);
                    DateTime lpoDueDate = CommonBLL.DateInsert(txtLpoDueDt.Text);
                    // string LPNS = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());
                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    if (TCs.Columns.Contains("Title"))
                        TCs.Columns.Remove("Title");
                    if (TCs.Columns.Contains("CompanyId"))
                        TCs.Columns.Remove("CompanyId");

                    DataTable dtbl = ConvertToDtbl((DataTable)Session["CustomerLPOGrid"]), sdt = (DataTable)Session["PaymentTermsLPO"];
                    IEnumerable<DataRow> query = from order in dtbl.AsEnumerable()
                                                 where order.Field<decimal>("Quantity") == 0
                                                 select order;

                    if (dtbl.Columns.Contains("QTYsum"))
                        dtbl.Columns.Remove("QTYsum");
                    if (sdt.Columns.Contains("CompanyId"))
                        sdt.Columns.Remove("CompanyId");
                    if (dtbl.Rows.Count > 0 && query != null && query.ToList().Count == 0)
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
                            string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + LpoNum +
                                "/" + CommonBLL.GetFinYrShortName() + "/Amend/" + CrntIdnt.Tables[1].Rows[0]["HistoryNum"].ToString();


                            // string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string LQnums = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagModify, new Guid(ViewState["EditID"].ToString()), Guid.Empty, Guid.Empty.ToString(), feonum, Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                LQnums, txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrder, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Customer Local Purchase Order",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Customer Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", "Error while Saving.");
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
                            string LPONONew = CrntIdnt.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" + CrntIdnt.Tables[0].Rows[0]["LPOSequence"].ToString() + "/"
                                + CommonBLL.GetFinYrShortName();

                            //string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string LQnums = String.Join(",", ListBoxLPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty.ToString(), feonum, Guid.Empty, Guid.Empty,
                                new Guid(ddlCustomer.SelectedValue), LPONONew, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                                LQnums, txtsubject.Text, new Guid(ddlRsdby.SelectedValue),
                                txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked, ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrderCustomer, "",
                                new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs, exdt, exdtPrcnt, dscnt,
                                dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Save")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Customer Local Purchase Order",
                                    "Data Inserted Successfully.");
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Customer Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", "Error while Saving.");
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
                            //string fponum = String.Join(",", ListBoxFPO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            string feonum = String.Join(",", ListBoxFEO.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                            res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagZSelect, new Guid(ViewState["EditID"].ToString()),
                                Guid.Empty, "", feonum, Guid.Empty, new Guid(ListBoxFEO.SelectedValue), new Guid(ddlCustomer.SelectedValue),
                                txtLpono.Text, lpoDate, lpoDueDate, new Guid(ddlsuplrctgry.SelectedValue),
                                new Guid(ddlSuplr.SelectedValue), ListBoxLPO.SelectedValue, txtsubject.Text,
                                new Guid(ddlRsdby.SelectedValue), txtimpinst.Text, ChkbInspcn.Checked, ChkbCEEApl.Checked,
                                ChkbDrwngAprls.Checked,
                                (txtDrwngAprls.Text.Trim() == "" ? 0 : int.Parse(txtDrwngAprls.Text)),
                                (txtInsptn.Text.Trim() == "" ? 0 : int.Parse(txtInsptn.Text)),
                                (txtCEEApl.Text.Trim() == "" ? 0 : int.Parse(txtCEEApl.Text)),
                                new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, lpoDueDate, int.Parse(txtDlvry.Text),
                                CommonBLL.StatusTypeLPOrderCustomer,
                                txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), dtbl, sdt, TCs,
                                exdt, exdtPrcnt, dscnt, dscntPrcnt, sltx, sltxPrcnt, pkng, pkngPrcnt, Attachments, new Guid(Session["CompanyID"].ToString()), false);
                            if (res == 0 && btnSave.Text == "Update")
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Customer Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Customer Local Purchase Order",
                                    "Data Updated Successfully.");
                                ClearAll(); Session.Remove("TCs");
                                Response.Redirect("Customer_LPO_Status.aspx", false);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtLpono.Text, "Customer Local Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Error while Updating.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", "Error while Updating.");
                                FillPaymentTerms();
                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('There are No Items to Save/Update.');", true);
                    }
                    else if (dtbl.Rows.Count > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('There are no ITEMS to Save, select minimum 1 Item.');", true);
                    else if (query != null && query.ToList().Count > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                                            "ErrorMessage('There are ITEMS with ZERO Quantity, Quantity should be greater than ZERO.');", true);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                int rows = e.Row.DataItemIndex;
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].CssClass = "rounded-First";
                    e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-Last";
                }
                if (e.Row.RowType == DataControlRowType.Header)
                    ((CheckBox)e.Row.FindControl("HdrChkbx")).Checked = Convert.ToBoolean(ViewState["IsChecked_CLPO"]);
                if (!IsPostBack)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        ((CheckBox)e.Row.FindControl("HdrChkbx")).Checked = true;
                    }
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    HiddenField HFLocalPOID = ((HiddenField)e.Row.FindControl("HFLocalPOID"));
                    if (Request.QueryString["ID"] != null && HFLocalPOID.Value != "")
                    {
                        if (Convert.ToBoolean(DT.Rows[rows]["IsItemCheck"]))
                            ((CheckBox)e.Row.FindControl("ItmChkbx")).Checked = true;
                        else
                            ((CheckBox)e.Row.FindControl("ItmChkbx")).Checked = false;
                    }
                    else
                        ((CheckBox)e.Row.FindControl("ItmChkbx")).Checked = Convert.ToBoolean(DT.Rows[rows]["IsItemCheck"]);
                    RunningTotal += Convert.ToDecimal(((Label)e.Row.FindControl("lblAmount")).Text);
                    GTotal += Convert.ToDecimal(((Label)e.Row.FindControl("lblTotal")).Text);
                }
                //}
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


                    if (gvLpoItems.PageIndex == 0)
                        ((Button)e.Row.FindControl("btnPrevious")).Enabled = false;
                    else
                        ((Button)e.Row.FindControl("btnPrevious")).Enabled = true;

                    if (gvLpoItems.PageCount == (gvLpoItems.PageIndex) + 1)
                        ((Button)e.Row.FindControl("btnNext")).Enabled = false;
                    else
                        ((Button)e.Row.FindControl("btnNext")).Enabled = true;
                    ((DropDownList)e.Row.FindControl("ddlPageSize")).SelectedValue = gvLpoItems.PageSize.ToString();
                    ((Label)e.Row.FindControl("lblFooterPaging")).Text = "Total Pages: " + gvLpoItems.PageCount + ", Current Page:" + (gvLpoItems.PageIndex + 1) + ", Rows to Display:";
                }
                Session["CustomerLPOGrid"] = DT;
                #region Not In Use
                //bool IsExciseDuty = false;

                //if (e.Row.RowType == DataControlRowType.DataRow)
                //{
                //    decimal CQuantity = 0;

                //    //UsersGrid.Columns.Cast<DataControlField>().SingleOrDefault(x => x.HeaderText == "Email");

                //    HiddenField HFExdtPrcnt = (HiddenField)e.Row.FindControl("HFExdtPrcnt");
                //    //HiddenField Rowno = (HiddenField)e.Row.RowIndex; //(HiddenField)e.Row.FindControl("hfFESNo");
                //    Label lblitem = (Label)e.Row.FindControl("lblItemID");
                //    Label amount = (Label)e.Row.FindControl("lblAmount");
                //    Label NetRate = (Label)e.Row.FindControl("lblNRate");
                //    Label GrnlRate = (Label)e.Row.FindControl("lblRate");
                //    Label Total = (Label)e.Row.FindControl("lblTotal");
                //    Label TotalAmountText = ((Label)e.Row.FindControl("lbltmnt"));//(Label)e.Row.FindControl("LblAmo");
                //    TextBox txtQty = (TextBox)e.Row.FindControl("txtQty");
                //    HiddenField MaxQty = (HiddenField)e.Row.FindControl("Hfd_MQty");
                //    HiddenField RcvdQty = (HiddenField)e.Row.FindControl("HF_RCVDQty");
                //    TextBox txtRate = (TextBox)e.Row.FindControl("txtRate");
                //    int RwNo = e.Row.RowIndex; //Convert.ToInt16(Rowno.Value) - 1;

                //    DataTable Quantity = (DataTable)ViewState["Quantity"];

                //    DataRow[] Qty = Quantity.Select("ItemId = '" + lblitem.Text + "'");
                //    Decimal EditQty = Convert.ToDecimal(txtQty.Text);
                //    if (Qty != null && Qty.Length > 0)
                //    {
                //        ((HiddenField)e.Row.FindControl("HF_ActualQty")).Value = Qty[0]["ActualQty"].ToString();
                //        MaxQty.Value = Qty[0]["Quantity"].ToString();
                //        //if (Qty[0]["Check"].ToString().ToLower() == "false")
                //        if (Request.QueryString != null && Request.QueryString["ID"] != null)
                //        {
                //            if ((Convert.ToDecimal(Qty[0]["Quantity"].ToString())) == (Convert.ToDecimal(Qty[0]["ActualQty"].ToString())))
                //                CQuantity = Convert.ToDecimal(Qty[0]["ActualQty"].ToString());
                //            else
                //                CQuantity = Convert.ToDecimal(Qty[0]["CQuantity"].ToString());
                //        }
                //        else
                //            CQuantity = Convert.ToDecimal(Qty[0]["CQuantity"].ToString());
                //        //else
                //        //    CQuantity = 0;

                //        if (Request.QueryString != null && Request.QueryString.Count >= 2 && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] != null && (Convert.ToBoolean(Request.QueryString["IsAm"].ToString()) == true))
                //        {
                //            txtRate.Visible = true; GrnlRate.Visible = false;
                //            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2);
                //            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                //        }
                //        else if (Request.QueryString != null && Request.QueryString.Count == 1 && Request.QueryString["ID"] != null)
                //        {
                //            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2);
                //            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                //        }
                //        else
                //        {
                //            if (Quantity.Rows[RwNo]["LPONumberraised"] != null && Quantity.Rows[RwNo]["LPOCancel"].ToString() != "True")
                //            {
                //                txtQty.Text = ((Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)) < 0 ? "0" :
                //            (Convert.ToDecimal(MaxQty.Value) - Convert.ToDecimal(CQuantity)).ToString());
                //            }

                //            amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //            Total.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "QPrice"))), 2).ToString();
                //            GTotal += Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "QPrice"))), 2);
                //            RunningTotal += (Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                //        }

                //        RcvdQty.Value = ((Convert.ToDecimal(CQuantity) - Convert.ToDecimal(MaxQty.Value)) < 0 ? "0" :
                //        (Convert.ToDecimal(CQuantity) - Convert.ToDecimal(MaxQty.Value)).ToString());
                //    }
                //    else
                //    {
                //        amount.Text = Math.Round((Convert.ToDecimal(txtQty.Text) * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate"))), 2).ToString();
                //        MaxQty.Value = "0";
                //        RcvdQty.Value = "0";
                //        CQuantity = 0;

                //        RunningTotal += (Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "qty"))
                //        * Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "rate")));
                //        GTotal += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "totalAmt"));
                //    }

                //    if (Request.QueryString["ID"] != null && Request.QueryString["ID"].ToString() != "")
                //    {
                //        txtQty.Text = CQuantity.ToString();
                //    }

                //    if (Convert.ToDecimal(MaxQty.Value) == Convert.ToDecimal(Qty[0]["ActualQty"])) //&& Qty[0]["Check"].ToString().ToLower() == "true"
                //    {
                //        if ((Quantity.Columns.Contains("Check")) && Qty[0]["Check"].ToString().ToLower() == "true")
                //            txtQty.Enabled = false;
                //    }

                //    if (Convert.ToDecimal(HFExdtPrcnt.Value) == 0 && IsExciseDuty == false)
                //    {
                //        ChkbCEEApl.Enabled = true;
                //        txtCEEApl.Enabled = true;
                //    }
                //    else
                //        IsExciseDuty = true;

                //    if (Request.QueryString["ID"] != null)
                //    {
                //        if (e.Row.RowType != DataControlRowType.DataRow) return;
                //        else
                //        {
                //            CheckBox cb = (CheckBox)e.Row.FindControl("ItmChkbx");
                //            //if (Quantity.Rows[RwNo])
                //            if (Quantity.Rows.Count > RwNo)
                //            {
                //                if (Quantity.Columns.Contains("Check"))
                //                {
                //                    if (Quantity.Rows[RwNo]["Check"] != null && Quantity.Rows[RwNo]["Check"].ToString() == "True")
                //                    {
                //                        cb.Checked = true;
                //                    }
                //                    else
                //                        cb.Checked = false;
                //                }
                //            }
                //            else
                //                cb.Checked = false;
                //        }
                //    }
                //}
                //if (e.Row.RowType == DataControlRowType.Footer)
                //{

                //} 
                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                if (gvLpoItems.HeaderRow == null) return;
                gvLpoItems.UseAccessibleHeader = false;
                gvLpoItems.HeaderRow.TableSection = TableRowSection.TableHeader;
                gvLpoItems.FooterRow.TableSection = TableRowSection.TableFooter;
                gvLpoItems.Columns.Cast<DataControlField>().SingleOrDefault(x => x.HeaderText == "Ex-Dt(%)");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                CheckBox ChkParent = (CheckBox)sender;
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                if (ChkParent.Checked)
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int rowss = gvLpoItems.Rows.Count;
                        if (i < rowss)
                        {
                            CheckBox cbk = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");
                            if (ChkParent.Checked)
                            {
                                cbk.Checked = true;
                                ViewState["IsChecked_CLPO"] = true;
                            }
                        }
                        DT.Rows[i]["IsItemCheck"] = true;
                    }
                }
                else
                {
                    ViewState["IsChecked_CLPO"] = false;
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int rowss = gvLpoItems.Rows.Count;
                        if (i < rowss)
                        {
                            CheckBox cbk = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");
                            cbk.Checked = false;
                            //DT.Rows[i]["IsItemCheck"] = false;
                        }
                        DT.Rows[i]["IsItemCheck"] = false;
                    }
                }
                Session["CustomerLPOGrid"] = DT;
                BindGridVeiw(gvLpoItems, DT);
                #region MyRegion
                //if (ChkParent.Checked)
                //{
                //    ArrayList al = DT2Array();
                //    ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                //    DataTable dt = (DataTable)ViewState["Quantity"];
                //    List<DataRow> _LRows = new List<DataRow>();
                //    for (int i = 0; i < gvLpoItems.Rows.Count; i++)
                //    {
                //        HiddenField HfItemID_DT = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
                //        //var _Row = (from d in dt.AsEnumerable() where d.Field<Guid>("ItemId") == new Guid(HfItemID_DT.Value) select new  { d });
                //        DataRow DR = dt.AsEnumerable().Where(P => P.Field<Guid>("ItemId") == new Guid(HfItemID_DT.Value)).FirstOrDefault();
                //        _LRows.Add(DR);
                //    }
                //    DataTable _Table = _LRows.CopyToDataTable();
                //    //dt = _Table;
                //    int j = 0;
                //    //bool IsFPOChk = false;
                //    bool IsLPOChk = false;
                //    //   string LPOItems = "", LPOItms1 = "";
                //    string LQItems = "";
                //    string Error = "";
                //    for (int i = 0; i < gvLpoItems.Rows.Count; i++)
                //    {
                //        string Selected = "false";
                //        CheckBox cb = (CheckBox)gvLpoItems.Rows[i].FindControl("ItmChkbx");

                //        HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[i].FindControl("HfItemID");
                //        if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
                //        {
                //            cb.Checked = true;
                //            Selected = "true";
                //        }
                //        if (al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //        {
                //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Released.');", true);
                //            cb.Checked = false;
                //        }
                //        else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) &&
                //            !al1.Contains(new Guid(HfItemID.Value)))
                //        {
                //            cb.Checked = true;
                //            Selected = "true";
                //        }
                //        if (!al.Contains(-1) && !al1.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && al1.Contains(new Guid(HfItemID.Value)))
                //        {
                //            if ((dt.Columns.Contains("Check")) && dt.Rows[i]["Check"].ToString().ToLower() == "true")
                //                cb.Checked = true;
                //            Selected = "true";
                //        }
                //        // else
                //        //  {
                //        if (Selected == "false")
                //        {
                //            ChkParent.Checked = false;
                //            cb.Checked = false;
                //        }
                //        if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //        {
                //            LQItems += " " + (i + 1) + ", ";
                //            //IsFPOChk = true;
                //            IsLPOChk = true;
                //            ChkParent.Checked = false;
                //        }
                //        //else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)))
                //        //{
                //        //    LPOItems += " " + (i + 1) + ", ";
                //        //    IsLPOChk = true;
                //        //}
                //        else
                //            Error = "Error while Checking.";
                //        // }

                //        if (cb.Checked == true && dt.Rows[i]["LPONumberraised"] != null)
                //        {
                //            j = i - dt.Rows.Count;
                //            string ItmQty = dt.Rows[i]["Quantity"].ToString();
                //            string ItmCQty = dt.Rows[i]["CQuantity"].ToString();
                //            //if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0") && al1.Contains(new Guid(HfItemID.Value))//dt.Rows[i]["ItemId"].ToString() == HfItemID.Value
                //            //    && dt.Rows[i]["LPOCancel"].ToString() != "True" )
                //            //    cb.Checked = false;// recent comment on 20/02/2016 by dinesh need to cross check 
                //            if ((Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0") && al1.Contains(new Guid(HfItemID.Value))//dt.Rows[i]["ItemId"].ToString() == HfItemID.Value
                //                && dt.Rows[i]["LPOCancel"].ToString() == "True")
                //                cb.Checked = true;
                //            else
                //                cb.Checked = true;
                //            j++;
                //        }

                //    }

                //    //if (IsLPOChk && IsFPOChk)
                //    //{
                //    //    ScriptManager.RegisterStartupScript(this.Page,
                //    //        this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item No(s)" + LPOItems.Trim(',') +
                //    //         "And Item(s) " + LQItems.Trim(',') + " are not selected in FPO.');", true);
                //    //}
                //    if (IsLPOChk == true)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page,
                //            this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LQItems.Trim(',') + " ');", true);
                //    }
                //    //else if (IsFPOChk == false && IsLPOChk == true)
                //    //    ScriptManager.RegisterStartupScript(this.Page,
                //    //        this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItems.Trim(',') + " ');", true); // recent comment 20/02/2016

                //    //  for (int i = 0; i < gvLpoItems.Rows.Count; i++)
                //    //   {
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
                //    //  }
                //    //if (IsLPOChk == true)
                //    //    ScriptManager.RegisterStartupScript(this.Page,
                //    //       this.GetType(), "ShowAll3", "ErrorMessage('LPO is generated for Item(s)" + LPOItms1.Trim(',') + " ');", true); 


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
                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
            }
            divPaymentTerms.InnerHtml = FillPaymentTerms();
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
                CheckBox ItmChkbx = (CheckBox)sender;
                //ItmChkbx.Checked = true;
                GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                int rowIndex = Convert.ToInt32(row.RowIndex);
                int DataIndex = Convert.ToInt32(row.DataItemIndex);
                DataTable DT = (DataTable)Session["CustomerLPOGrid"];
                CheckBox cbk = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                if (ItmChkbx.Checked)
                {
                    cbk.Checked = true;
                    DT.Rows[DataIndex]["IsItemCheck"] = true;
                }
                else
                {
                    cbk.Checked = false;
                    DT.Rows[DataIndex]["IsItemCheck"] = false;
                }
                Session["CustomerLPOGrid"] = DT;
                #region Not In Use
                //ArrayList al = DT2Array();
                //ArrayList al1 = (ArrayList)ViewState["LPORelease"];
                //int GvRowCount = gvLpoItems.Rows.Count;
                //GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                //int rowIndex = Convert.ToInt32(row.RowIndex);
                //DataTable dt = (DataTable)ViewState["Quantity"];
                //HiddenField HfItemID = (HiddenField)gvLpoItems.Rows[rowIndex].FindControl("HfItemID");
                //if (ItmChkbx.Checked) //&& GvRowCount == Convert.ToInt16(dt.Rows.Count.ToString()) Commented to show all items need to check
                //{
                //    string selected = "false";
                //    //int RIndex =dt.Rows.Count - rowIndex;
                //    string ItmQty = dt.Rows[rowIndex]["Quantity"].ToString();
                //    string ItmCQty = dt.Rows[rowIndex]["CQuantity"].ToString();

                //    CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");

                //    if (!al.Contains(-1) && !al.Contains(new Guid(HfItemID.Value)) && Request.QueryString["ID"] != null)
                //    {
                //        cb.Checked = true;
                //        selected = "true";
                //    }
                //    if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Released in Other LPO.');", true);
                //        cb.Checked = false;
                //    }
                //    //if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)) && (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0"))
                //    //{
                //    //   // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Released.');", true);
                //    //    cb.Checked = true;
                //    //}
                //    else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && !al1.Contains(new Guid(HfItemID.Value))) //&&
                //        //(Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0"))
                //        cb.Checked = true;
                //    else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) && !al1.Contains(new Guid(HfItemID.Value)) &&
                //        (Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) != Convert.ToDouble("0"))
                //        cb.Checked = true;
                //    else
                //    {
                //        if (selected == "false")
                //        {
                //            cb.Checked = false;
                //            if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //                ScriptManager.RegisterStartupScript(this.Page,
                //                    this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Released in Other LPO.');", true);
                //            //else if (!al.Contains(-1) && al.Contains(new Guid(HfItemID.Value)) ||
                //            //(Convert.ToDouble(ItmQty) - Convert.ToDouble(ItmCQty)) == Convert.ToDouble("0"))
                //            //    ScriptManager.RegisterStartupScript(this.Page,
                //            //        this.GetType(), "ShowAll3", "ErrorMessage('This Item Was already Selected in Another LPO.');", true);
                //            else
                //                ScriptManager.RegisterStartupScript(this.Page,
                //                    this.GetType(), "ShowAll2", "ErrorMessage('Error while Checking.');", true);
                //        }
                //    }
                //}
                //else
                //{
                //    CheckBox cb = (CheckBox)gvLpoItems.Rows[rowIndex].FindControl("ItmChkbx");
                //    cb.Checked = false;
                //    if (!al.Contains(-1) && al1.Contains(new Guid(HfItemID.Value)))
                //        ScriptManager.RegisterStartupScript(this.Page,
                //            this.GetType(), "ShowAll3", "ErrorMessage('This Item Was Not Selected.');", true);
                //}
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty2", "CHeck('chkExdt', 'dvExdt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "CHeck('chkPkng', 'dvPkng');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDscnt2", "CHeckForLpo('chkDsnt', 'dvDsnt');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSltx2", "CHeck('chkSltx', 'dvSltx');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty3", "CHeck('ChkbCEEApl', 'dvCEEApl');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty4", "CHeck('ChkbInspcn', 'dvInsptn');", true);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowExDuty5", "CHeck('ChkbDrwngAprls', 'dvDrwngAprls');", true); 
                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }
        }

        #endregion
    }
}
