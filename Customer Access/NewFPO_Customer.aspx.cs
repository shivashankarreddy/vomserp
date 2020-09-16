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
using System.Threading;
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Customer_Access
{
    public partial class NewFPO_Customer : System.Web.UI.Page
    {

        # region variables
        int res;
        NewFPOBLL NFPOBLL = new NewFPOBLL();
        NewFPOStatusBLL NFPOSBLL = new NewFPOStatusBLL();
        NewFQuotationBLL NFQBL = new NewFQuotationBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBL = new NewEnquiryBLL();
        CustomerBLL cusmr = new CustomerBLL();
        CheckBLL CBL = new CheckBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        ErrorLog ELog = new ErrorLog();
        static string GeneralCtgryID;
        static Guid GenSupID = Guid.Empty;
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        static DataSet dss = new DataSet();
        static DataSet dsEnm = new DataSet();
        static int FlagRepeat = 0;
        static bool RptFPOChecked = false;
        CommonBLL CBLL = new CommonBLL();
        string Empty = string.Empty;
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewFPO_Customer));
                        if (!IsPostBack)
                        {
                            ClearAll();
                            txtFpoDt.Attributes.Add("readonly", "readonly");
                            txtFpoDuedt.Attributes.Add("readonly", "readonly");
                            txtReceivedDate.Attributes.Add("readonly", "readonly");
                            txtfenqDt.Attributes.Add("readonly", "readonly");
                            txtFpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                            spnRfpolbl.Style.Add("display", "none");
                            spnRfpoddl.Style.Add("display", "none");
                            GetData();

                            EnumMasterBLL EMBLL = new EnumMasterBLL();
                            DataSet ds = new DataSet();
                            ds = EMBLL.EnumMasterSelect(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                            Session["GenSupID"] = ds.Tables[0].Rows[0][0].ToString();
                            GenSupID = new Guid(ds.Tables[0].Rows[0][0].ToString());

                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            txtfenqDt.Enabled = false;
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit")
                                && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    DivComments.Visible = true;
                                    EditRecord(Request.QueryString["ID"], 60);
                                    chkbIRO.Enabled = false;
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                DivComments.Visible = false;
                                chkbIRO.Enabled = true;
                                btnSave.Text = "Save";
                            }
                            else
                                Response.Redirect("../Masters/CHome.aspx?NP=no");
                        }
                        else
                        {
                            if (chkbIRO.Checked)
                            {
                                spnRfpolbl.Style.Add("Display", "block");
                                spnRfpoddl.Style.Add("Display", "block");
                            }
                            if (Session["PaymentTermsFPO"] != null)
                                divPaymentTerms.InnerHtml = FillPaymentTerms();
                        }
                        if (Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                        {
                            btnSave.Text = "Save";
                            txtFpoNo.Enabled = false;
                        }
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get LPO Items
        /// </summary>
        private void GetLPOitems50()
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                string FEID = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                long StatID = CommonBLL.StatusTypeFPOrder;
                if (Request.QueryString["ID"] != null)
                    StatID = CommonBLL.StatusTypeLPOrder;
                DataSet items = NFPOBLL.Select(CommonBLL.FlagKSelect, Guid.Empty, FEID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty,
                     DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, StatID, Guid.Empty, false, false, false, "",
                     new Guid(Session["UserID"].ToString()), DateTime.Now,
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO,
                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (items.Tables.Count > 0 && items.Tables[0].Rows.Count > 0)
                    Session["FPOSelectedItems"] = items.Tables[0].Rows[0][0].ToString();
                else
                    Session["FPOSelectedItems"] = "";
                if (items.Tables.Count > 1 && items.Tables[1].Rows.Count > 0)
                    Session["RegrettedItems"] = items.Tables[1].Rows[0][0].ToString();
                else
                    Session["RegrettedItems"] = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                divListBox.InnerHtml = AttachedFiles();
                GetGeneralID();
                BindDropDownList(ddlPrcBsis, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlRsdby, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindDropDownList(ddldept, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindRadioList(rbtnshpmnt, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode));

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    lblCustomerNm.Text = ddlcustomer.SelectedItem.Text;

                    if (!IsPostBack)
                        BindDropDownList(ddlRefFPO, NEBL.SelectFenquiries(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            DateTime.Now, DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));

                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    if (Request.QueryString.Count > 2 && Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "" && Request.QueryString["FqId"] != null
                                    && Request.QueryString["FqId"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');

                        //foreach (var item in states)
                        //{
                        //    if (Lstfenqy.Items.FindByValue("" + item + "") != null)
                        //        Lstfenqy.Items.FindByValue("" + item + "").Selected = true;
                        //}

                        foreach (string s in states)// I think the above commented lines are enough, I didnt checked Its my Guess ::VARA 
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        FillInputFields();
                    }
                    else if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');

                        //foreach (var item in states)
                        //{
                        //    if (Lstfenqy.Items.FindByValue("" + item + "") != null)
                        //        Lstfenqy.Items.FindByValue("" + item + "").Selected = true;
                        //}

                        foreach (string s in states)// I think the above commented lines are enough, I didnt checked Its my Guess ::VARA 
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        FillInputFields(Request.QueryString["FeqID"]);
                    }
                }
                else
                {

                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');

                        //foreach (var item in states)
                        //{
                        //    if (Lstfenqy.Items.FindByValue("" + item + "") != null)
                        //        Lstfenqy.Items.FindByValue("" + item + "").Selected = true;
                        //}

                        foreach (string s in states)// I think the above commented lines are enough, I didnt checked Its my Guess ::VARA 
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        FillInputFields(Request.QueryString["FeqID"]);
                    }
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
        /// This is used to bind Radio Button lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
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
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                //To Bind Data in to the ListBox
                if (ddlcustomer.SelectedValue != Guid.Empty.ToString())
                {
                    CommonDt.Tables[0].DefaultView.Sort = "Description ASC";
                    Lstfenqy.DataSource = CommonDt.Tables[0];
                    Lstfenqy.DataTextField = "Description";
                    Lstfenqy.DataValueField = "ID";
                    Lstfenqy.DataBind();
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
        /// This is to get genereal ID of items
        /// </summary>
        private void GetGeneralID()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.EnumMasterSelect(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["GeneralCtgryID"] = ds.Tables[0].Rows[0][0].ToString();
                    GeneralCtgryID = ds.Tables[0].Rows[0][0].ToString();
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
        /// This is used to fill Details when selected
        /// </summary>
        /// <param name="FQuoteID"></param>
        private void FillInputFields(string FEnqID)
        {
            try
            {
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");
                DataSet FQuoteDeatils = NFPOBLL.Select(CommonBLL.FlagHSelect, Guid.Empty, FEnqID, new Guid(ddlcustomer.SelectedValue), Guid.Empty,
                    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "",
                    DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO,
                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (FQuoteDeatils.Tables.Count >= 2 && FQuoteDeatils.Tables[0].Rows.Count > 0 &&
                    FQuoteDeatils.Tables[1].Rows.Count > 0)
                {
                    txtfenqDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(FQuoteDeatils.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                    txtsubject.Text = FQuoteDeatils.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = FQuoteDeatils.Tables[0].Rows[0]["Instruction"].ToString();
                    ddldept.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlRsdby.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    txtFpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                    txtFpoDuedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ?
                        DateTime.Now.AddDays(5).Date : DateTime.Now.AddDays(6).Date);
                    DataTable dtt = new DataTable();
                    DataView MyView = new DataView();
                    dtt = FQuoteDeatils.Tables[1].Copy();
                    MyView = dtt.DefaultView;
                    MyView.Sort = "FESNo ASC";
                    dtt = MyView.ToTable();

                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    if (!dtt.Columns.Contains("Check"))
                        dtt.Columns.Add(dc);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtt);
                    ds.Tables[0].DefaultView.Sort = "FESNo ASC";
                    Session["FloatEnquiryFPO"] = ds;
                    GetLPOitems50();
                    divFPOItems.InnerHtml = FillGridView(Empty);
                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                }
                else
                {
                    divFPOItems.InnerHtml = "";
                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                    Session["amountFPO"] = "";
                    Session["MessageFPO"] = "";
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
        /// This is used to fill Details when selected without Parameters
        /// </summary>
        /// <param name="FQuoteID"></param>
        private void FillInputFields()
        {
            try
            {
                DataSet FQuoteDeatils = NFPOSBLL.Select(CommonBLL.FlagCSelect, new Guid(Request.QueryString["FQID"].ToString()), Request.QueryString["FeqID"].ToString(), DateTime.Now, DateTime.Now, Guid.Empty);
                if (FQuoteDeatils.Tables.Count >= 2 && FQuoteDeatils.Tables[0].Rows.Count > 0 &&
                    FQuoteDeatils.Tables[1].Rows.Count > 0)
                {
                    txtfenqDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(FQuoteDeatils.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                    txtsubject.Text = FQuoteDeatils.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = FQuoteDeatils.Tables[0].Rows[0]["Instruction"].ToString();
                    ddldept.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlRsdby.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    txtFpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                    txtFpoDuedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ?
                        DateTime.Now.AddDays(5).Date : DateTime.Now.AddDays(6).Date);
                    DataTable dtt = new DataTable();
                    DataView MyView = new DataView();
                    dtt = FQuoteDeatils.Tables[1].Copy();
                    MyView = dtt.DefaultView;
                    MyView.Sort = "FESNo ASC";
                    dtt = MyView.ToTable();

                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    if (!dtt.Columns.Contains("Check"))
                        dtt.Columns.Add(dc);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtt);
                    ds.Tables[0].DefaultView.Sort = "FESNo ASC";
                    Session["FloatEnquiryFPO"] = ds;
                    GetLPOitems50();
                    divFPOItems.InnerHtml = FillGridView(Empty);
                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                }
                else
                {
                    divFPOItems.InnerHtml = "";
                    divPaymentTerms.InnerHtml = "";
                    Session["amountFPO"] = "";
                    Session["MessageFPO"] = "";
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
        /// This is used to Insert Update records
        /// </summary>
        private void SaveRecord()
        {
            try
            {
                int res = 1; Filename = FileName();
                DataSet ds = (DataSet)Session["FloatEnquiryFPO"];
                if (ds.Tables[0].Columns.Contains("ItemDetailsId"))
                    ds.Tables[0].Columns.Remove("ItemDetailsId");
                if (ds.Tables[0].Columns.Contains("ItemDesc"))
                    ds.Tables[0].Columns.Remove("ItemDesc");
                if (ds.Tables[0].Columns.Contains("UnitName"))
                    ds.Tables[0].Columns.Remove("UnitName");
                if (ds.Tables[0].Columns.Contains("QPrice"))
                    ds.Tables[0].Columns.Remove("QPrice");
                if (ds.Tables[0].Columns.Contains("RoundOff"))
                    ds.Tables[0].Columns.Remove("RoundOff");
                if (ds.Tables[0].Columns.Contains("PInvID"))
                    ds.Tables[0].Columns.Remove("PInvID");
                if (ds.Tables[0].Columns.Contains("HSCode"))
                    ds.Tables[0].Columns.Remove("HSCode");
                if (ds.Tables[0].Columns.Contains("PackingPercentage"))
                    ds.Tables[0].Columns.Remove("PackingPercentage");
                if (ds.Tables[0].Columns.Contains("Check"))
                    ds.Tables[0].Columns.Remove("Check");

                if (ds.Tables[0].Columns.Contains("CompanyId"))
                    ds.Tables[0].Columns.Remove("CompanyId");
                if (ds.Tables[0].Columns.Contains("IsSubItems"))
                    ds.Tables[0].Columns.Remove("IsSubItems");
                if (ds.Tables[0].Columns.Contains("Original_Quantity"))
                    ds.Tables[0].Columns.Remove("Original_Quantity");
                DataTable FPOdt = ds.Tables[0].Copy();

                DataTable Paymentdt = (DataTable)Session["PaymentTermsFPO"];
                if (Paymentdt.Columns.Contains("CompanyId"))
                    Paymentdt.Columns.Remove("CompanyId");

                DataTable TCs = CommonBLL.ATConditionsTitle();
                if (Session["TCs"] != null)
                {
                    TCs = (DataTable)Session["TCs"];
                }
                if (TCs.Columns.Contains("Title"))
                    TCs.Columns.Remove("Title");

                Guid CID = new Guid(ddlcustomer.SelectedValue);
                Guid DPT = new Guid(ddldept.SelectedValue);
                Guid FPORsdby = new Guid(ddlRsdby.SelectedValue);
                Guid PriceBasis = new Guid(ddlPrcBsis.SelectedValue);
                int DeliveryPeriod = Convert.ToInt32(txtDlvry.Text);
                DateTime DeliveryDt = DateTime.Now.AddDays(DeliveryPeriod * 7);
                string FENo = "";
                if (Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    FENo = Request.QueryString["FeqID"].ToString();
                else
                    FENo = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string Attachments = "";
                if (Session["FPOUploads"] != null)
                {
                    ArrayList all = (ArrayList)Session["FPOUploads"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                if (FPOdt.Rows.Count > 0 && FENo != "")
                {
                    if (btnSave.Text == "Save" && Request.QueryString["IsAm"] == null && Request.QueryString["IsAm"] != "True")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, FENo, CID, DPT, Guid.Empty, CommonBLL.DateInsert(txtfenqDt.Text), Guid.Empty, Guid.Empty,
                         DateTime.Now, txtFpoNo.Text.Trim(), txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                         CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                         PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2,
                         CommonBLL.StatusTypeFPOrder, new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked, false, "", new Guid(Session["UserID"].ToString()),
                         DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    else if (btnSave.Text == "Save" && Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagASelect, new Guid(Request.QueryString["ID"]), FENo, CID, DPT, Guid.Empty, CommonBLL.DateInsert(txtfenqDt.Text), Guid.Empty, Guid.Empty,
                        DateTime.Now, txtFpoNo.Text.Trim(), txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                        CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                        PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2,
                        CommonBLL.StatusTypeFPOrder, new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked, false, "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    else
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), FENo, CID, DPT, Guid.Empty,
                        CommonBLL.DateInsert(txtfenqDt.Text), Guid.Empty, Guid.Empty, DateTime.Now, txtFpoNo.Text.Trim(), txtsubject.Text.Trim(),
                        CommonBLL.DateInsert(txtFpoDt.Text), CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby,
                        txtimpinst.Text.Trim(), "", PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrder,
                        new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked, false, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            SendDefaultMails(cusmr.SelectCustomers(CommonBLL.FlagASelect, new Guid(ddlcustomer.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                        }
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO Customer", "Data inserted successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOStatus_Customer.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO", "Error while Inserting.");
                    }
                    if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO Customer", "Data Updated successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOStatus_Customer.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO Customer", "Error while Updating.");
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('There are No Items to Save/Update.');", true);
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string Errmsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Edit aselected Record
        /// </summary>
        /// <param name="ID">Record ID</param>
        private void EditRecord(string ID, long status)
        {
            try
            {

                DataSet EditDS = NFPOBLL.Select(CommonBLL.FlagModify, new Guid(ID), "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty,
                     DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "",
                     new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                     CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                    && EditDS.Tables[2].Rows.Count > 0)
                {
                    if (!chkbIRO.Checked)
                        Session["EditID"] = ID;
                    ddlcustomer.Enabled = false;
                    Lstfenqy.Enabled = false;
                    ViewState["EditID"] = ID;
                    GetData();
                    rbtnshpmnt.ClearSelection();
                    ddlcustomer.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEditForRepetedFPOCheck(CommonBLL.FlagQSelect, Guid.Empty, Guid.Empty,
                        new Guid(EditDS.Tables[0].Rows[0]["CusmorId"].ToString()), Guid.Empty, "",
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", status, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                        CommonBLL.EmptyDt(), Session["Custmr_SuplrID"].ToString()));
                    string FEnqids = EditDS.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();
                    string[] SptFEnqids = FEnqids.Split(',');

                    //foreach (var item in SptFEnqids)
                    //{
                    //    if (Lstfenqy.Items.FindByValue("" + item + "") != null)
                    //        Lstfenqy.Items.FindByValue("" + item + "").Selected = true;
                    //}

                    foreach (string s in SptFEnqids)// I think the above commented lines are enough, I didnt checked Its my Guess ::VARA 
                    {
                        foreach (ListItem item in Lstfenqy.Items)
                        {
                            if (item.Value == s) item.Selected = true;
                        }
                    }

                    ddldept.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlRsdby.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlPrcBsis.SelectedValue = EditDS.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = EditDS.Tables[0].Rows[0]["PriceBasisText"].ToString();

                    txtfenqDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["ForeignEnquiryDate"].ToString()));
                    if (EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString() != "")
                        txtReceivedDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString()));

                    txtFpoDt.Text = ((FlagRepeat == 0) ? CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["FPODate"].ToString())) :
                        CommonBLL.DateDisplay(DateTime.Now));
                    txtFpoDuedt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["FPODueDate"].ToString()));

                    if (FlagRepeat == 0)
                        txtFpoNo.Text = EditDS.Tables[0].Rows[0]["ForeignPurchaseOrderNo"].ToString();
                    txtsubject.Text = EditDS.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = EditDS.Tables[0].Rows[0]["Instruction"].ToString();
                    txtDlvry.Text = EditDS.Tables[0].Rows[0]["DeliveryPeriod"].ToString();
                    rbtnshpmnt.SelectedValue = EditDS.Tables[0].Rows[0]["ShipmentMode"].ToString();
                    DataTable dtt = new DataTable();
                    dtt = EditDS.Tables[1].Copy();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtt);

                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    if (!dtt.Columns.Contains("Check"))
                        dtt.Columns.Add(dc);

                    ds = RemoveColumns(ds);
                    Session["FloatEnquiryFPO"] = ds;
                    GetLPOitems50();
                    divFPOItems.InnerHtml = FillGridView(Empty);

                    Session["TCs"] = (CBL.SelectATConditions(CommonBLL.FlagYSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(ID), Guid.Empty, 0, Guid.Empty, "",
                        new Guid(Session["UserID"].ToString()))).Tables[0];

                    dtt = new DataTable();
                    dtt = DeletePaymntColumns(EditDS.Tables[2].Copy());
                    Session["PaymentTermsFPO"] = dtt;
                    if (dtt.Rows.Count > 0)
                    {
                        Session["amountFPO"] = dtt.Compute("Sum(PaymentPercentage)", "");
                        divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }

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
                        Session["FPOUploads"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                        divListBox.InnerHtml = "";

                    if (Convert.ToBoolean(EditDS.Tables[0].Rows[0]["Bivac"].ToString()))
                        Chkbivac.Checked = true;
                    else if (Convert.ToBoolean(EditDS.Tables[0].Rows[0]["Cotecna"].ToString()))
                        Chkcotecna.Checked = true;

                    btnSave.Text = chkbIRO.Checked ? "Save" : "Update";
                }
                else
                {
                    divFPOItems.InnerHtml = "";
                    divPaymentTerms.InnerHtml = "";
                    Session["amountFPO"] = "";
                    Session["MessageFPO"] = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
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
                ViewState["EditID"] = null;
                ddlcustomer.SelectedIndex = ddlfenq.SelectedIndex = ddlfenq.SelectedIndex = -1;
                ddldept.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlPrcBsis.SelectedIndex = -1;
                Lstfenqy.Items.Clear();
                ddlPrcBsis.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                Session["MessageFPO"] = null;
                Session["PaymentTermsFPO"] = null;
                Session["amountFPO"] = null;
                divPaymentTerms.InnerHtml = "";
                divFPOItems.InnerHtml = "";
                ddlcustomer.Enabled = true;
                Lstfenqy.Enabled = true;
                ddlfenq.Enabled = true;
                txtDlvry.Text = "";
                txtfenqDt.Text = "";
                txtReceivedDate.Text = "";
                txtFpoDuedt.Text = "";
                txtFpoNo.Text = "";
                txtimpinst.Text = "";
                txtsubject.Text = "";
                txtPriceBasis.Text = "";
                txtFpoDt.Text = "";
                Session["PaymentTermsFPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                Session["FPOSelectedItems"] = null;
                Session["RegrettedItems"] = null;
                Session["EditID"] = null;
                Session["FPOUploads"] = null;
                FlagRepeat = 0;
                //chkbIRO.Checked = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Thsi is used to Delete payment Items
        /// </summary>
        /// <param name="dtt"></param>
        /// <returns></returns>
        private DataTable DeletePaymntColumns(DataTable dtt)
        {
            try
            {
                dtt.Columns["PaymentSerialNo"].ColumnName = "SNo";
                dtt.Columns["Percentage"].ColumnName = "PaymentPercentage";
                dtt.Columns["Against"].ColumnName = "Description";
                dtt.Columns.Remove("PaymentTermsId");
                dtt.Columns.Remove("FQuotationId");
                dtt.Columns.Remove("FPurchaseOrderId");
                dtt.Columns.Remove("LQuotationId");
                dtt.Columns.Remove("LPurchaseOrderId");
                dtt.Columns.Remove("CreatedBy");
                dtt.Columns.Remove("CreatedDate");
                dtt.Columns.Remove("ModifiedBy");
                dtt.Columns.Remove("ModifiedDate");
                dtt.Columns.Remove("IsActive");
                Session["amountFPO"] = dtt.Compute("Sum(PaymentPercentage)", "").ToString();
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
        /// This is used to delete unnecesssary Columns from DataSet when Editing
        /// </summary>
        /// <param name="dss"></param>
        /// <returns></returns>
        private DataSet RemoveColumns(DataSet dss)
        {
            try
            {
                dss.Tables[0].Columns.Remove("LocalEnquireId");
                dss.Tables[0].Columns.Remove("ItemDetailsID");
                dss.Tables[0].Columns.Remove("ForeignEnquireId");
                dss.Tables[0].Columns.Remove("ForeignPOId");
                dss.Tables[0].Columns.Remove("LocalQuotationId");
                dss.Tables[0].Columns.Remove("LocalPOId");
                dss.Tables[0].Columns.Remove("CreatedBy");
                dss.Tables[0].Columns.Remove("CreatedDate");
                dss.Tables[0].Columns.Remove("ModifiedBy");
                dss.Tables[0].Columns.Remove("ModifiedDate");
                dss.Tables[0].Columns.Remove("IsActive");

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return dss;
        }

        /// <summary>
        /// This is used to Fill ItemDetails
        /// </summary>
        /// <param name="EnqID"></param>
        /// <returns></returns>
        private string FillGridView(string EnqID)
        {
            try
            {
                DataSet ds = new DataSet(); string ItemIDs = string.Empty;
                if (EnqID != "")
                {
                    ds = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(EnqID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0,
                        0, 0, 0, 0, Guid.Empty, "", new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                    ds = RemoveColumns(ds);
                    Session["FloatEnquiryFPO"] = ds;
                    Session["TempFloatEnquiry"] = ds;
                    Session["EnqID"] = EnqID;
                }
                else
                    ds = (DataSet)Session["FloatEnquiryFPO"];

                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, GenSupID, new Guid(Session["CompanyID"].ToString()));
                dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblItems'>" +
                "<thead align='left'><tr >");
                sb.Append("<th class='rounded-First'>&nbsp;</th>");
                sb.Append("<th>SNo</th><th>Item Description</th><th align='center'>Part No</th><th align='center'>Specification</th>" +
                "<th align='center'>Make</th><th>Quantity</th><th>Units</th><th align='right'>Rate($)</th><th align='right'>Amount($)</th>" +
                "<th align='center'>Remarks</th><th class='rounded-Last'>Regret</th></tr></thead>");
                sb.Append("<tbody class='bcGridViewMain'>");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (Session["FPOSelectedItems"] == null || Session["RegrettedItems"] == null || Session["RegrettedItems"] == "" || Session["FPOSelectedItems"] == "")
                        GetLPOitems50();

                    Dictionary<string, string> FPOSelectedItems = new Dictionary<string, string>();
                    Dictionary<string, string> RegrettedItems = new Dictionary<string, string>();
                    string FPOItems = Session["FPOSelectedItems"].ToString();
                    string RegRetedItms = Session["RegrettedItems"].ToString();

                    FPOSelectedItems = FPOItems.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    RegrettedItems = RegRetedItms.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());

                    decimal TotalAmount = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                            TotalAmount += Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                        string sno = (i + 1).ToString();
                        sb.Append("<tr valign='top'>");
                        sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' type='checkbox' onclick='CheckItem(" + sno + ")' ");
                        string itemId = ds.Tables[0].Rows[i]["ItemId"].ToString();
                        if (RptFPOChecked == false)
                        {
                            if (Session["EditID"] == null)
                            {
                                if (Session["FPOSelectedItems"] != null && Session["FPOSelectedItems"].ToString() != "")
                                {
                                    if (!FPOSelectedItems.ContainsKey(itemId))
                                    {
                                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                                            sb.Append(" checked='checked' ");
                                    }
                                    else
                                    {
                                        ds.Tables[0].Rows[i]["Check"] = false;
                                        sb.Append(" disabled='disabled' ");
                                    }
                                }
                                else
                                {
                                    if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                                        sb.Append(" checked='checked' ");
                                }
                            }
                            else
                            {
                                if (FPOSelectedItems.ContainsKey(itemId))
                                {
                                    if (RegrettedItems.ContainsKey(itemId))
                                        sb.Append(" disabled='disabled' ");

                                    if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                                        sb.Append(" checked='checked' ");
                                }
                                else
                                {
                                    if (RegrettedItems.ContainsKey(itemId))
                                        sb.Append(" disabled='disabled' ");

                                    if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))
                                        sb.Append(" checked='checked' ");
                                }
                            }
                        }
                        else
                        {
                            if (RegrettedItems.ContainsKey(itemId))
                            {
                                ds.Tables[0].Rows[i]["Check"] = false;// added newly
                                sb.Append(" disabled='disabled' ");
                            }
                            else
                            {
                                if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"].ToString()))// added newly
                                    sb.Append(" checked='checked' ");
                            }
                        }
                        sb.Append(" name='CheckAll'/></td>");

                        sb.Append("<td align='center'>" + (i + 1).ToString() + "</td>");//S.NO
                        sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + sno + ", 0)' id='HItmID" + sno
                            + "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString() + "' width='5px' style='WIDTH: 5px;'/></td>");
                        if (ds.Tables[0].Rows[i]["ItemId"].ToString() == "")
                        {
                            sb.Append("<td valign='Top' width='200px'><select id='ddl" + sno
                                + "' class='PayElementCode' onchange='FillItemDRP(" + sno + ")'>");
                            if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow drr in dss.Tables[0].Rows)
                                    if (!ItemIDs.Contains(drr["ID"].ToString()))
                                        sb.Append("<option value='" + drr["ID"].ToString() + "' >" + drr["ItemDescription"].ToString()
                                        + "</option>");
                            }
                            sb.Append("</select></td>");
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)'  class='icons additionalrow'  ID='btnShow"
                                    + (i + 1) + "'  title='Add Item to Item Master' onclick='fnOpen(" + sno + ","
                                    + i.ToString() + ")' ><img src='/images/AddNW.jpeg'/></a></span>");
                        }
                        else
                        {
                            sb.Append("<td valign='Top' width='200px'><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString()
                                + "</div></td>");//ItemDesc
                            ItemIDs = ItemIDs + "," + ds.Tables[0].Rows[i]["ItemID"].ToString();
                        }
                        sb.Append("<td>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</td>");//PartNo
                        sb.Append("<td><textarea name='txtSpecifications' id='txtSpecifications" + sno
                                        + "' onchange='AddItemColumn(" + sno + ", 0)' class='bcAsptextboxmulti' onfocus='ExpandTXT(" + sno
                                        + ")' onblur='ReSizeTXT(" + sno + ")' style='height:22px; width:150px; resize:none;'>"
                                        + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
                        sb.Append("<td><input type='text' name='txtMake' class='bcAsptextbox' onchange='AddItemColumn(" + sno
                            + ", 0)' id='txtMake" + sno + "' value='" + ds.Tables[0].Rows[i]["Make"].ToString() + "'/></td>");
                        sb.Append("<td><input type='text' name='txtQuantity' dir='rtl' size='05px' onchange='AddItemColumn(" + sno
                            + ", 0)' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString()
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' "
                            + " maxlength='6' class='bcAsptextbox' style='width:50px;'/></td>");
                        if (ds.Tables[0].Rows[i]["UnitName"].ToString() == "")//Units
                        {
                            sb.Append("<td><select id='ddlU" + sno + "' class='PayElementCode' onchange='AddItemColumn(" + sno + ", 0)'>");
                            if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow dru in dsEnm.Tables[0].Rows)
                                    sb.Append("<option value='" + dru["ID"].ToString() + "' >" + dru["Description"].ToString() + "</option>");
                            }
                            sb.Append("</select></td>");
                        }
                        else
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");
                        if (ds.Tables[0].Rows[i]["Rate"].ToString() == "0" || (Session["AccessRole"].ToString()) == CommonBLL.SuperAdminRole)
                            sb.Append("<td align='right'><input type='text' name='txtRate' dir='rtl' disabled = 'disabled' onchange='AddItemColumn(" + sno
                            + ", 0)' id='txtRate" + sno + "' value='" + ds.Tables[0].Rows[i]["Rate"].ToString()
                            + "' class='bcAsptextbox' maxlength='6' style='width:50px;' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");
                        else
                            sb.Append("<td align='right'><input type='text' name='txtRate' dir='rtl' disabled = 'disabled' onchange='AddItemColumn(" + sno
                            + ", 0)' id='txtRate" + sno + "' value='" + ds.Tables[0].Rows[i]["Rate"].ToString()
                            + "' class='bcAsptextbox' readonly='readonly'  style='width:50px;' maxlength='6' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");

                        sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString()).ToString("N") + "</td>");

                        sb.Append("<td><input type='text' name='txtRemarks' onchange='AddItemColumn(" + sno + ", 0)' id='txtRemarks" + sno
                            + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString() + "' class='bcAsptextbox' style='width:75px;'/></td>");

                        if (Session["EditID"] != null && !Session["RegrettedItems"].ToString().Contains(itemId))
                        {
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' class='icons deleteicon' title='Regret' ");
                            if (CommonBLL.SuperAdminRole == (Session["AccessRole"].ToString()))
                                sb.Append(" onclick='javascript:return doConfirmRegret(" + sno + ");' ");
                            else
                                sb.Append(" onclick='javascript:return AlertError();' ");
                            sb.Append(" ><img src='/images/Delete.png' style='border-style: none;'/></a></span></td>");
                        }
                        else if (Session["EditID"] == null)
                            sb.Append("<td>&nbsp;</td>");
                        else
                            sb.Append("<td>Regretted</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<th colspan='4' align='right' class='rounded-foot-left'><b><span></span></b></th>");
                    sb.Append("<th colspan='6' align='right'><b><span>Total Amount($) : " + TotalAmount.ToString("N") + "</span></b></th>");
                    sb.Append("<th align='center'><b><span>&nbsp;</span></b></th>");
                    sb.Append("<th class='rounded-foot-right'><b><span></span></b></th>");
                    sb.Append("</tfoot>");
                }
                sb.Append("</table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
                string linenum = ex.LineNumber().ToString();
                string ErrMsg = ex.Message;
                return "";
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
                int TotalAmt = 0;
                string Message = "";
                if (Session["amountFPO"] != null && Session["amountFPO"].ToString() != "")
                    TotalAmt = Convert.ToInt32(Session["amountFPO"]);
                if (Session["MessageFPO"] != null && Session["MessageFPO"].ToString() != "")
                    Message = Session["MessageFPO"].ToString();
                DataTable dt = (DataTable)Session["PaymentTermsFPO"];

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblPaymentTerms' " +
                    " align='center' style='font-size: medium;'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Payment(%)</th><th>Description</th>" +
                "<th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");
                        if (dt.Rows[i]["PaymentPercentage"].ToString() != "")
                            sb.Append("<td><input type='text' name='txtPercAmt' onfocus='this.select()' onMouseUp='return false' "
                                + " value='" + Convert.ToInt64(Convert.ToDouble(dt.Rows[i]["PaymentPercentage"].ToString())).ToString() + "' "
                                + " id='txtPercAmt" + SNo + "' onkeypress='return isNumberKey(event)' onchange='getPaymentValues(" + SNo + ")' "
                                + " maxlength='3' style='text-align: right; width:50px;' class='bcAsptextbox' "
                                + " onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");
                        else
                            sb.Append("<td><input type='text' name='txtPercAmt' value='" + dt.Rows[i]["PaymentPercentage"].ToString() + "' "
                                + " id='txtPercAmt" + SNo + "' onkeypress='return isNumberKey(event)' onchange='getPaymentValues(" + SNo + ")' maxlength='3' "
                                + " class='bcAsptextbox' onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");
                        sb.Append("<td><input type='text' name='txtDesc' value='"
                            + dt.Rows[i]["Description"].ToString() + "'  id='txtDesc" + SNo
                            + "' onchange='getPaymentValues(" + SNo + ")' class='bcAsptextbox'/></td>");
                        if (TotalAmt == 100)
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' "
                                + " onclick='javascript:return doConfirmPayment(" + SNo
                                + ")' title='Delete'><img src='/images/Delete.png'/></a></span></td>");
                        else if (dt.Rows.Count == 1)
                            sb.Append("<td><a href='javascript:void(0)' onclick='getPaymentValues("
                                + SNo + ")' class='icons additionalrow' title='Add Row'><img src='/images/add.jpeg'/></a></span></td>");
                        else if (dt.Rows.Count == (i + 1))
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmPayment(" + SNo
                                + ")' title='Delete'><img src='/images/Delete.png'/></a>&nbsp;&nbsp;"
                                + "<a href='javascript:void(0)' onclick='getPaymentValues(" + SNo
                                + ")' class='icons additionalrow' title='Add Row'><img src='/images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmPayment(" + SNo
                                + ")' title='Delete'><img src='/images/Delete.png'/></a></span></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='3' class='rounded-foot-right' " +
                    " align='left'> Total Percent : <b>" + TotalAmt +
                       "</b>%<input id='HfMessage' type='hidden' name='HfMessage' value='" + Message + "'/></th></tfoot>");
                }
                sb.Append("</tbody></table>");


                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// This is used to Bind DDL Reference FE
        /// </summary>
        private void BindReferenceDDL()
        {
            try
            {
                BindDropDownList(ddlRefFPO, NFPOBLL.SelectRepetedFpoBind(CommonBLL.FlagYSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 50, Guid.Empty, false, false, "",
                    new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                    CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()), Session["Custmr_SuplrID"].ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Send E-Mails After Generation of FPO
        /// </summary>
        protected void SendDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                string ToAddrs = "rajaprasadamuteam@yahoo.in";
                string CcAddrs = "";
                if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                {
                    CcAddrs = "sprt.bvpl@yahoo.com, " + Session["UserMail"].ToString();
                }
                else
                {
                    CcAddrs = "sprt.bvpl@yahoo.com, " + Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                }
                string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtls());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about FPO Details
        /// </summary>
        /// <returns></returns>
        protected string InformationEnqDtls()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Order for your Quotation " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append(" We are pleased to release our Purchase Order " + txtFpoNo.Text + " Dt: " + txtFpoDt.Text +
                    " for our requirement. Please find the order in VOMS Application for complete" +
                    " details and expedite to deliver the material as per delivery time mentioned.");
                sb.Append(System.Environment.NewLine + " Please confirm the receipt of the order and ensure packing is in good condition. ");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        private string AttachedFiles()
        {
            try
            {
                if (Session["FPOUploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["FPOUploads"];
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
                        if (Session["FPOUploads"] != null)
                        {
                            alist = (ArrayList)Session["FPOUploads"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["FPOUploads"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["FPOUploads"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO ", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save Button Click Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)Session["FloatEnquiryFPO"];
                string[] val = HselectedItems.Value.Split(',').ToArray();
                val = val.Where(s => !String.IsNullOrEmpty(s)).ToArray();

                ArrayList NotSel = new ArrayList();
                NotSel.AddRange(val);
                if (NotSel.Count > 0)
                {
                    NotSel.RemoveAt(0);
                    NotSel.Reverse();
                    if (NotSel.Count > 0)
                    {
                        //::VARA Need to Optimize Here
                        for (int i = 0; i < NotSel.Count; i++)
                        {
                            Guid del = new Guid(NotSel[i].ToString());
                            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                            {
                                if (del == new Guid(ds.Tables[0].Rows[k]["ItemID"].ToString()))
                                {
                                    ds.Tables[0].Rows[k].Delete();
                                    break;
                                }
                            }
                        }
                        ds.Tables[0].AcceptChanges();
                    }
                }
                if (ds.Tables[0].Rows.Count > 0 && txtDlvry.Text.Trim() != "" && txtfenqDt.Text != "" &&
                    ddlcustomer.SelectedValue != "0" && Lstfenqy.SelectedValue != "0")
                    SaveRecord();
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Fill All the Deatails Properly.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
                Response.Redirect("../Purchases/NewFPOrder.Aspx");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        #endregion

        #region DropDownLists Selected Index Change Events

        /// <summary>
        /// Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkbIRO.Checked && ddlRefFPO.Enabled)
                {

                }
                else
                {
                    if (ddlcustomer.SelectedValue != Guid.Empty.ToString())
                    {
                        string cstmr = ddlcustomer.SelectedValue;
                        ClearAll();
                        ddlcustomer.SelectedValue = cstmr;
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "",
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    }
                    else
                    {
                        ClearAll();
                    }
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
        /// Frn Enquiry Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenq_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string ForEnq = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (Lstfenqy.SelectedValue != "0")
                {
                    FillInputFields(ForEnq);

                }
                else if (Request.QueryString["FeqID"] != null)
                    FillInputFields(Request.QueryString["FeqID"]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }


        /// <summary>
        /// DropDownList Selected Index for Repeat FPO's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefFPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlRefFPO.SelectedValue != "00000000-0000-0000-0000-000000000000")
                {
                    FlagRepeat = 1;
                    EditRecord(ddlRefFPO.SelectedValue, CommonBLL.StatusTypeRepeatedFPO);
                    ddlcustomer.Enabled = true;
                }
                else
                {
                    ClearAll();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
        }

        #endregion

        #region Check Box Checked Changed Event

        /// <summary>
        /// Is Repeat Order Check Box Checked Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CHkShow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkbIRO.Checked)
                {
                    ddlRefFPO.Enabled = true;
                    BindReferenceDDL();
                }
                else
                {
                    ddlRefFPO.Enabled = false;
                    ddlRefFPO.SelectedValue = Guid.Empty.ToString();
                    ClearAll();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        #endregion

        # region WebMethods

        /// <summary>
        /// Add New Row to Item Table
        /// </summary>
        /// <param name="RNo"></param>
        /// <param name="sv"></param>
        /// <param name="PrtNo"></param>
        /// <param name="spec"></param>
        /// <param name="Make"></param>
        /// <param name="Qty"></param>
        /// <param name="Units"></param>
        /// <param name="Rmrks"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddNewRow(int RNo, string sv, string PrtNo, string spec, string Make, string Qty, string NRate,
            string Units, string Rmrks, int isnew)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                if (Convert.ToBoolean(HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["IsAm"]) == true)
                {
                    if (!ds.Tables[0].Columns.Contains("Original_Quantity"))
                        ds.Tables[0].Columns.Add("Original_Quantity", typeof(int));
                    if (ds.Tables[0].Rows[RNo - 1]["Original_Quantity"].ToString() == "")
                        ds.Tables[0].Rows[RNo - 1]["Original_Quantity"] = ds.Tables[0].Rows[RNo - 1]["Quantity"];
                }
                int RowCount = ds.Tables[0].Rows.Count; decimal Rate;
                if (ds.Tables[0].Rows[RNo - 1]["Rate"].ToString() != "0")
                {
                    Rate = Convert.ToDecimal(ds.Tables[0].Rows[RNo - 1]["Rate"].ToString());
                    Rate = Convert.ToDecimal(NRate);
                    ds.Tables[0].Rows[RNo - 1]["Rate"] = Rate;
                }
                else
                {
                    Rate = Convert.ToDecimal(NRate);
                    ds.Tables[0].Rows[RNo - 1]["Rate"] = Rate;
                }
                if (sv != "")
                    ds.Tables[0].Rows[RNo - 1]["ItemId"] = sv;
                if (PrtNo != "")
                    ds.Tables[0].Rows[RNo - 1]["PartNumber"] = PrtNo;
                if (spec != "")
                    ds.Tables[0].Rows[RNo - 1]["Specifications"] = spec;
                ds.Tables[0].Rows[RNo - 1]["Make"] = Make;
                if (Qty != "")
                {
                    if (Convert.ToBoolean(HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["IsAm"]) == true)
                    {
                        if ((Convert.ToDecimal(ds.Tables[0].Rows[RNo - 1]["Original_Quantity"].ToString()) < Convert.ToDecimal(Qty)
                    || Convert.ToDecimal(ds.Tables[0].Rows[RNo - 1]["Original_Quantity"].ToString()) == Convert.ToDecimal(Qty)))
                        {
                            ds.Tables[0].Rows[RNo - 1]["Quantity"] = Qty;
                            ds.Tables[0].Rows[RNo - 1]["Amount"] = (Convert.ToDecimal(Qty) * Rate);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Item Quantity shouldn't be less with raised FPO.');", true);
                    }
                    else
                    {
                        ds.Tables[0].Rows[RNo - 1]["Quantity"] = Qty;
                        ds.Tables[0].Rows[RNo - 1]["Amount"] = (Convert.ToDecimal(Qty) * Rate);
                    }
                }
                if (Units != "" && Units != "0")
                {
                    ds.Tables[0].Rows[RNo - 1]["UNumsId"] = Units;
                    //http://stackoverflow.com/questions/1485284/fastest-alternative-to-datatable-select-to-narrow-cached-data
                    DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + Units.ToString());//::VARA Select Query From DT Takes Long Time. DataTable.Rows.Find With Primary Key for single row return
                    ds.Tables[0].Rows[RNo - 1]["UnitName"] = selRow[0]["Description"].ToString();
                }
                ds.Tables[0].Rows[RNo - 1]["Remarks"] = Rmrks;
                string NewQty = ds.Tables[0].Rows[RowCount - 1]["Quantity"].ToString();
                if (sv == "" && Units == "" && spec != "" && Qty != "" && Make != "" && NewQty != "")
                {
                    if (isnew == 1)
                    {
                        DataRow dr = ds.Tables[0].NewRow();
                        dr["ExDutyPercentage"] = 0.00;
                        dr["DiscountPercentage"] = 0.00;
                        dr["QPrice"] = 0.00;
                        dr["Remarks"] = string.Empty;
                        dr["ForeignQuotationId"] = 0;
                        ds.Tables[0].Rows.Add(dr);
                    }
                }
                ds.AcceptChanges();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return FillGridView(Empty);
        }

        /// <summary>
        /// Bind GridView
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string BindGridView(string rowNo)
        {
            return FillGridView(rowNo);
        }

        /// <summary>
        /// Delete Items from Table
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItem(int rowNo)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];

                if (rowNo > 0)
                {
                    ds.Tables[0].Rows[rowNo - 1].Delete();
                    ds.Tables[0].AcceptChanges();
                    Session["FloatEnquiryFPO"] = ds;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return FillGridView(Empty);
        }

        /// <summary>
        /// Regret Items from Table
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool RegretItem(int rowNo, long FEID)
        {
            int res = 1;
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                if (rowNo > 0)
                {
                    long ItemID = Convert.ToInt64(ds.Tables[0].Rows[rowNo - 1]["ItemId"].ToString());
                    ItemStatusBLL ISBLL = new ItemStatusBLL();
                    res = ISBLL.UpdateItemStatus(Convert.ToChar('R'), ItemID, FEID, 0, 0, 0, 0, 0, CommonBLL.StatusTypeRegretFPO, "", "", "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            if (res != 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Fill Items to DropDown List
        /// </summary>
        /// <param name="rowNo"></param>
        /// <param name="sv"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillItemDRP(int rowNo, int sv)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    //http://stackoverflow.com/questions/1485284/fastest-alternative-to-datatable-select-to-narrow-cached-data
                    DataRow[] selRow = dss.Tables[0].Select("ID = " + sv.ToString());//::VARA Select Query From DT Takes Long Time. DataTable.Rows.Find with Primary Key for single row return
                    ds.Tables[0].Rows[rowNo - 1]["ItemId"] = selRow[0]["ID"].ToString();
                    ds.Tables[0].Rows[rowNo - 1]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();
                    ds.Tables[0].Rows[rowNo - 1]["Rate"] = 0.00;
                    ds.Tables[0].Rows[rowNo - 1]["Quantity"] = 0;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order", ex.Message.ToString());
            }
            return FillGridView(Empty);
        }

        /// <summary>
        /// Fill Units to DropDown List
        /// </summary>
        /// <param name="rowNo"></param>
        /// <param name="sv"></param>
        /// <param name="PrtNo"></param>
        /// <param name="spec"></param>
        /// <param name="Make"></param>
        /// <param name="Qty"></param>
        /// <param name="Units"></param>
        /// <param name="Rmrks"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillUnitDRP(int rowNo, int sv, string PrtNo, string spec, string Make, string Qty, string Units, string Rmrks)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + sv.ToString());
                        ds.Tables[0].Rows[rowNo - 1]["UNumsId"] = sv;
                        ds.Tables[0].Rows[rowNo - 1]["UnitName"] = selRow[0]["Description"].ToString();
                        ds.Tables[0].AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
            }
            return FillGridView(Empty);
        }

        /// <summary>
        /// Check Pruchase Order Number
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckFPOOrderNo(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckFPOOrderNo('O', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckItem(string IsChecked, string ID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                int rowNo = Convert.ToInt32(ID);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[rowNo - 1]["Check"] = Convert.ToBoolean(IsChecked);
                    ds.Tables[0].AcceptChanges();
                    Session["FloatEnquiryFPO"] = ds;
                }
                return FillGridView(Empty);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ErrMsg + ", Line : " + LineNo;
            }
        }

        /// <summary>
        /// This is used for Repeat FPO
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ChkBoxRptdFPOMode(bool RptFPOIsChecked)
        {
            RptFPOChecked = RptFPOIsChecked;
            return "";
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
                ArrayList all = (ArrayList)Session["FPOUploads"];
                if (all.Count > 0)
                    all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        # endregion

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
                dt = (DataTable)Session["PaymentTermsFPO"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[rowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["amountFPO"] = dt.Compute("Sum(PaymentPercentage)", "");
                Session["PaymentTermsFPO"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
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
                dt = (DataTable)Session["PaymentTermsFPO"];
                int count = dt.Rows.Count;
                int PmntPercent = 0;

                object OldAmt = dt.Rows[rowNo - 1]["PaymentPercentage"];
                PmntPercent = Convert.ToInt32(Convert.ToInt64(Convert.ToDouble(dt.Compute("Sum(PaymentPercentage)", ""))));
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
                    else if (PmntPercent >= 100 && (dt.Rows[count - 1]["PaymentPercentage"].ToString() == "")
                        || dt.Rows[count - 1]["PaymentPercentage"].ToString() == "0")
                        dt.Rows[count - 1].Delete();
                    Session["MessageFPO"] = "";
                    Session["AmountFPO"] = PmntPercent.ToString();
                }
                else
                    Session["MessageFPO"] = "Percentage Cannot Exceed 100";
                Session["PaymentTermsFPO"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
            return FillPaymentTerms();
        }

        # endregion
    }
}