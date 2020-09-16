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
using Ajax;
using VOMS_ERP.Admin;
using System.IO;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace VOMS_ERP.Purchases
{
    public partial class NewFPOrderVerbal : System.Web.UI.Page
    {
        # region variables
        int res;
        EnumMasterBLL embal = new EnumMasterBLL();
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
        static Guid EditID = Guid.Empty;
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

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
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewFPOrderVerbal));
                        if (!IsPostBack)
                        {
                            if (Request.QueryString["ID"] != null)
                                EditID = new Guid(Request.QueryString["ID"]);
                            ClearAll();
                            txtFpoDt.Attributes.Add("readonly", "readonly");
                            txtFpoDuedt.Attributes.Add("readonly", "readonly");
                            txtReceivedDate.Attributes.Add("readonly", "readonly");
                            txtFpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                            spnRfpolbl.Style.Add("display", "none");
                            spnRfpoddl.Style.Add("display", "none");
                            HF2.Value = Guid.Empty.ToString();
                            GetData();

                            EnumMasterBLL EMBLL = new EnumMasterBLL();
                            DataSet ds = new DataSet();
                            ds = EMBLL.EnumMasterSelect(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                            Session["GenSupID"] = ds.Tables[0].Rows[0][0].ToString();
                            GenSupID = new Guid(ds.Tables[0].Rows[0][0].ToString());

                            divPaymentTerms.InnerHtml = FillPaymentTerms();
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit")
                                && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    DivComments.Visible = true;
                                    HF2.Value = Request.QueryString["ID"].ToString();
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
                                Response.Redirect("../Masters/Home.aspx?NP=no");
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
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
        /// Get LPO Items
        /// </summary>
        private void GetLPOitems50()
        {
            try
            {
                //string FEID = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //long StatID = CommonBLL.StatusTypeFPOrder;
                //if (Request.QueryString["ID"] != null)
                //    StatID = CommonBLL.StatusTypeLPOrder;
                //DataSet items = NFPOBLL.Select(CommonBLL.FlagKSelect, Guid.Empty, FEID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty,
                //     DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, StatID, Guid.Empty, false, false, "",
                //     new Guid(Session["UserID"].ToString()), DateTime.Now,
                //     new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPO(),
                //     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                //if (items.Tables.Count > 0 && items.Tables[0].Rows.Count > 0)
                //    Session["FPOSelectedItems"] = items.Tables[0].Rows[0][0].ToString();
                //else
                //    Session["FPOSelectedItems"] = "";
                //if (items.Tables.Count > 1 && items.Tables[1].Rows.Count > 0)
                //    Session["RegrettedItems"] = items.Tables[1].Rows[0][0].ToString();
                //else
                //    Session["RegrettedItems"] = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                divFPOItems.InnerHtml = FillGridItems(Guid.Empty);
                BindDropDownList(ddlPrcBsis, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlRsdby, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindDropDownList(ddldept, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindRadioList(rbtnshpmnt, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode));

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    //lblCustomerNm.Text = ddlcustomer.SelectedItem.Text;

                    //if (!IsPostBack)
                    //    BindDropDownList(ddlRefFPO, NEBL.SelectFenquiries(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                    //        DateTime.Now, DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));

                    ////BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "",
                    ////"", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    //if (Request.QueryString.Count > 2 && Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                    //                Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "" && Request.QueryString["FqId"] != null
                    //                && Request.QueryString["FqId"].ToString() != "")
                    //{
                    //    ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                    //    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                    //        Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    //    //To Select the Passed values
                    //    string[] states = Request.QueryString["FeqID"].Split(',');
                    //    foreach (string s in states)
                    //    {
                    //        foreach (ListItem item in Lstfenqy.Items)
                    //        {
                    //            if (item.Value == s) item.Selected = true;
                    //        }
                    //    }
                    //    FillInputFields();
                    //}
                    //else if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                    //                Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    //{
                    //    ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                    //    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                    //        Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    //    //To Select the Passed values
                    //    string[] states = Request.QueryString["FeqID"].Split(',');
                    //    foreach (string s in states)
                    //    {
                    //        foreach (ListItem item in Lstfenqy.Items)
                    //        {
                    //            if (item.Value == s) item.Selected = true;
                    //        }
                    //    }
                    //    FillInputFields(Request.QueryString["FeqID"]);
                    //}
                }
                else
                {

                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        //BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                        //    Guid.Empty, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        //string[] states = Request.QueryString["FeqID"].Split(',');
                        //foreach (string s in states)
                        //{
                        //    foreach (ListItem item in Lstfenqy.Items)
                        //    {
                        //        if (item.Value == s) item.Selected = true;
                        //    }
                        //}
                        //FillInputFields(Request.QueryString["FeqID"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
            }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                    //Lstfenqy.DataSource = CommonDt.Tables[0];
                    //Lstfenqy.DataTextField = "Description";
                    //Lstfenqy.DataValueField = "ID";
                    //Lstfenqy.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                DataSet FQuoteDeatils = NFPOBLL.Select(CommonBLL.FlagHSelect, Guid.Empty, FEnqID, new Guid(ddlcustomer.SelectedValue), Guid.Empty,
                    Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "",
                    DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPO(),
                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (FQuoteDeatils.Tables.Count >= 2 && FQuoteDeatils.Tables[0].Rows.Count > 0 &&
                    FQuoteDeatils.Tables[1].Rows.Count > 0)
                {
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                DataTable EmtpyFPO = CommonBLL.EmptyDtNewFPOForVebal();
                if (EmtpyFPO.Columns.Contains("ItemDetailsId"))
                    EmtpyFPO.Columns.Remove("ItemDetailsId");


                DataTable FPOdt = (DataTable)Session["ItemCode"];

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
                string FENo = Guid.Empty.ToString();
                string Attachments = "";
                if (Session["FPOUploads"] != null)
                {
                    ArrayList all = (ArrayList)Session["FPOUploads"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                if (FPOdt.Rows.Count > 0)
                {
                    if (btnSave.Text == "Save" && Request.QueryString["IsAm"] == null && Request.QueryString["IsAm"] != "True")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, FENo, CID, DPT, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), Guid.Empty,
                         DateTime.Now, txtFpoNo.Text.Trim(), "", txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                         CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                         PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrder,
                         new Guid(rbtnshpmnt.SelectedValue), false, false, true, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, Paymentdt, TCs, Attachments,
                         new Guid(Session["CompanyID"].ToString()), FPOdt, false);
                    }
                    else if (btnSave.Text == "Save" && Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagASelect, new Guid(Request.QueryString["ID"]), FENo, CID, DPT, Guid.Empty,
                        DateTime.Now, Guid.Empty.ToString(), Guid.Empty, DateTime.Now, txtFpoNo.Text.Trim(), "", txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                        CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                        PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrder, new Guid(rbtnshpmnt.SelectedValue),
                        false, false, true, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, Paymentdt, TCs, Attachments,
                        new Guid(Session["CompanyID"].ToString()), FPOdt, false);
                    }
                    else
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), FENo, CID, DPT, Guid.Empty,
                        DateTime.Now, Guid.Empty.ToString(), Guid.Empty, DateTime.Now, txtFpoNo.Text.Trim(), "", txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                        CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                        PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrder, new Guid(rbtnshpmnt.SelectedValue),
                        false, false, true, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmtpyFPO, Paymentdt, TCs, Attachments,
                        new Guid(Session["CompanyID"].ToString()), FPOdt, false);
                    }
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                            SendDefaultMails(cusmr.SelectCustomers(CommonBLL.FlagASelect, new Guid(ddlcustomer.SelectedValue), new Guid(Session["CompanyID"].ToString())));

                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Verbal Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO", "Data inserted successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Verbal Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO", "Error while Inserting.");
                    }
                    if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Verbal Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO", "Data Updated successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtFpoNo.Text, "Verbal Foreign Purchase Order No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO", "Error while Updating.");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                HFID.Value = ID;
                DataSet EditDS = NFPOBLL.Select(CommonBLL.FlagModify, new Guid(ID), "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                     DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false, true, "",
                     new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                     CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                    && EditDS.Tables[2].Rows.Count > 0)
                {
                    if (!chkbIRO.Checked)
                        Session["EditID"] = ID;
                    ddlcustomer.Enabled = false;
                    ViewState["EditID"] = ID;
                    GetData();
                    ddlcustomer.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                    string FEnqids = EditDS.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();
                    string[] SptFEnqids = FEnqids.Split(',');
                    ddldept.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlRsdby.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlPrcBsis.SelectedValue = EditDS.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtPriceBasis.Text = EditDS.Tables[0].Rows[0]["PriceBasisText"].ToString();

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
                    fillEditGrid(ds);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ddlcustomer.SelectedIndex = -1;
                ddldept.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlPrcBsis.SelectedIndex = -1;
                ddlPrcBsis.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                Session["MessageFPO"] = null;
                Session["PaymentTermsFPO"] = null;
                Session["amountFPO"] = null;
                divPaymentTerms.InnerHtml = "";
                divFPOItems.InnerHtml = "";
                ddlcustomer.Enabled = true;
                txtDlvry.Text = "";
                txtFpoDt.Text = "";
                txtFpoDuedt.Text = "";
                txtFpoNo.Text = "";
                txtimpinst.Text = "";
                txtsubject.Text = "";
                Session["PaymentTermsFPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                Session["FPOSelectedItems"] = null;
                Session["RegrettedItems"] = null;
                Session["EditID"] = null;
                Session["FPOUploads"] = null;
                FlagRepeat = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearAllForRptOrdrUncheck()
        {
            try
            {
                btnSave.Text = "Save";
                ViewState["EditID"] = null;
                ddlcustomer.SelectedIndex = -1;
                ddldept.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlPrcBsis.SelectedIndex = -1;
                ddlPrcBsis.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                ddlRsdby.SelectedIndex = ddlPrcBsis.SelectedIndex = -1;
                Session["MessageFPO"] = null;
                Session["PaymentTermsFPO"] = null;
                Session["amountFPO"] = null;
                divPaymentTerms.InnerHtml = "";
                divFPOItems.InnerHtml = FillGridItems(Guid.Empty);
                ddlcustomer.Enabled = true;
                txtDlvry.Text = "";
                //txtFpoDt.Text = "";
                txtFpoDuedt.Text = "";
                txtFpoNo.Text = "";
                txtimpinst.Text = "";
                txtsubject.Text = "";
                txtReceivedDate.Text = "";
                Session["PaymentTermsFPO"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                Session["FPOSelectedItems"] = null;
                Session["RegrettedItems"] = null;
                Session["EditID"] = null;
                Session["FPOUploads"] = null;
                FlagRepeat = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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

                //"ItemId", "PartNumber","Specifications", "Make", "Quantity", "UNumsId", "ItemDetailsId"




                //dss.Tables[0].Columns.Remove("LocalEnquireId");
                ////dss.Tables[0].Columns.Remove("ItemDetailsID");
                //dss.Tables[0].Columns.Remove("ForeignEnquireId");
                //dss.Tables[0].Columns.Remove("ForeignPOId");
                //dss.Tables[0].Columns.Remove("LocalQuotationId");
                //dss.Tables[0].Columns.Remove("LocalPOId");
                //dss.Tables[0].Columns.Remove("CreatedBy");
                //dss.Tables[0].Columns.Remove("CreatedDate");
                //dss.Tables[0].Columns.Remove("ModifiedBy");
                //dss.Tables[0].Columns.Remove("ModifiedDate");
                //dss.Tables[0].Columns.Remove("IsActive");

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                                    + i.ToString() + ")' ><img src='../images/AddNW.jpeg'/></a></span>");
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
                            sb.Append("<td align='right'><input type='text' name='txtRate' dir='rtl' onchange='AddItemColumn(" + sno
                            + ", 0)' id='txtRate" + sno + "' value='" + ds.Tables[0].Rows[i]["Rate"].ToString()
                            + "' class='bcAsptextbox' maxlength='6' style='width:50px;' "
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");
                        else
                            sb.Append("<td align='right'><input type='text' name='txtRate' dir='rtl' onchange='AddItemColumn(" + sno
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
                            sb.Append(" ><img src='../images/Delete.png' style='border-style: none;'/></a></span></td>");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                                + ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");
                        else if (dt.Rows.Count == 1)
                            sb.Append("<td><a href='javascript:void(0)' onclick='getPaymentValues("
                                + SNo + ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else if (dt.Rows.Count == (i + 1))
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmPayment(" + SNo
                                + ")' title='Delete'><img src='../images/Delete.png'/></a>&nbsp;&nbsp;"
                                + "<a href='javascript:void(0)' onclick='getPaymentValues(" + SNo
                                + ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmPayment(" + SNo
                                + ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");

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
                BindDropDownList(ddlRefFPO, NFPOBLL.SelectRepetedFpoBind(CommonBLL.FlagYSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "",
                    "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 50, Guid.Empty, false, false, "",
                    new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                    CommonBLL.EmptyDtNewFPO(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()), Session["Custmr_SuplrID"].ToString()));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Verbal", ex.Message.ToString());
            }
        }


        public DataSet ReadExcelData()
        {
            try
            {
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

                    FilePath = MapPath("~/uploads/" + FileName); //Server.MapPath(FolderPath + FileName);
                    FileUpload1.SaveAs(FilePath);
                }
                string sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                OleDbConnection dbCon = new OleDbConnection(sConnection);
                dbCon.Open();
                DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataSet dsOutput = new DataSet();
                //for (int nCount = 0; nCount < dtSheetName.Rows.Count; nCount++)
                if (dtSheetName.Rows.Count > 0)
                {
                    int nCount = 0;
                    string sSheetName = dtSheetName.Rows[nCount]["TABLE_NAME"].ToString();
                    string sQuery = "Select * From [" + sSheetName + "]";
                    OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                    OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                    DataTable dtData = new DataTable(); //DataTable dtData_Clone = new DataTable();
                    dtData.TableName = sSheetName;
                    dbDa.Fill(dtData);
                    //dtData_Clone = dtData.Copy();
                    //foreach (DataRow value in dtData.Rows)
                    //{

                    //    if (string.IsNullOrEmpty(value.ItemArray[1].ToString()) && string.IsNullOrEmpty(value.ItemArray[5].ToString())
                    //        && string.IsNullOrEmpty(value.ItemArray[6].ToString()))
                    //    {
                    //        int Index = dtData.Rows.IndexOf(value);
                    //        dtData_Clone.Rows.RemoveAt(Index);
                    //        dtData_Clone.AcceptChanges();
                    //    }

                    //}
                    //dtData = dtData_Clone.Copy();                    
                    dsOutput.Tables.Add(dtData);
                }
                dbCon.Close();
                return dsOutput;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());

                return null;
            }
        }
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
                dt.Columns.Add(new DataColumn("Category", typeof(string)));
                dt.Columns.Add(new DataColumn("ItemDescription", typeof(Guid)));
                dt.Columns.Add(new DataColumn("PartNo", typeof(string)));
                dt.Columns.Add(new DataColumn("Specification", typeof(string)));
                dt.Columns.Add(new DataColumn("Make", typeof(string)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Units", typeof(Guid)));
                dt.Columns.Add(new DataColumn("HSCode", typeof(string)));
                dt.Columns.Add(new DataColumn("Rate", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                dt.Columns.Add(new DataColumn("Remarks", typeof(string)));
                dt.Columns.Add(new DataColumn("ID", typeof(Guid)));

                Session["ItemCode"] = dt;
                Session["SelectedItems"] = Codes;
                Session["SelectedCat"] = CatCodes;
                Session["SelectedUnits"] = UnitCodes;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                    "Specifications", "Make", "Quantity", "UNumsId", "HSCode","Rate", "Amount", "Remarks", "ItemDetailsId");
                dtt.Columns["ItemId"].ColumnName = "ItemDescription";
                dtt.Columns["PartNumber"].ColumnName = "PartNo";
                dtt.Columns["Specifications"].ColumnName = "Specification";
                dtt.Columns["ItemDetailsId"].ColumnName = "ID";
                dtt.Columns["UNumsId"].ColumnName = "Units";

                DataColumn dc = dtt.Columns.Add("SNo", typeof(int));
                DataColumn dc1 = dtt.Columns.Add("Category", typeof(Guid));
                dc.SetOrdinal(0);
                dc1.SetOrdinal(1);
                int sno = 1;
                foreach (DataRow dr in dtt.Rows)
                {
                    dr["SNo"] = sno;
                    dr["Category"] = GeneralCtgryID;
                    Codes1.Add(sno, new Guid(dr["ItemDescription"].ToString()));
                    UnitCodes1.Add(sno, new Guid(dr["Units"].ToString()));
                    sno++;
                }
                CatCodes1.Add(1, new Guid(GeneralCtgryID));
                HttpContext.Current.Session["SelectedItems"] = Codes1;
                HttpContext.Current.Session["SelectedCat"] = CatCodes1;
                HttpContext.Current.Session["SelectedUnits"] = UnitCodes1;

                HttpContext.Current.Session["ItemCode"] = dtt;
                divFPOItems.InnerHtml = FillItemGrid(false);//0, dtt, Codes1, dss, Convert.ToInt32(GeneralCtgryID), CatCodes1, 0, UnitCodes1, ""
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Verbal", ex.Message.ToString());
            }
        }

        private string FillItemGrid(bool ADD)
        {
            try
                {
                int RowNo = 1;
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
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
                        "<th>Make</th><th>Quantity</th><th>Units</th><th>HSCode</th><th>Rate($)</th><th>Amount($)</th><th>Remarks</th><th></th><th></th></tr>");
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
                            drarray = ds.Tables[0].Select("ID=" + "'" + dt.Rows[i]["ItemDescription"] + "'");
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
                        DataRow[] dr = dsUnits.Tables[0].Select("ID=" + "'" + dt.Rows[i]["Units"] + "'");
                        if (dr.Length > 0)
                        {
                            sb.Append("<label id='lblUnit" + (i + 1) + "'  >" + dr[0]["Description"].ToString() + "</label>");
                        }
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblHsCode" + (i + 1) + "'  >" + dt.Rows[i]["HSCode"].ToString() + "</label>");
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
                    //sb.Append("<select id='ddl0' Class='bcAspdropdown' width='50px' onchange='FillSpec_ItemDesc(0)'>");
                    //sb.Append("<option value='00000000-0000-0000-0000-000000000000'>-SELECT-</option>");
                    //int countRow = 0;

                    //foreach (DataRow row in dss.Tables[0].Rows)
                    //{
                    //    Guid ItemID = Guid.Empty;
                    //    if (!Codes.ContainsValue(new Guid(row["ID"].ToString())) && new Guid(row["ID"].ToString()) != ItemID)
                    //        sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["ItemDescription"].ToString() +
                    //            "</option>");
                    //    countRow++;
                    //    if (countRow == 50)
                    //        break;

                    //}
                    //sb.Append("</select>");
                    sb.Append("<input type='text' id='Txt_IM' style='height:20px; width:150px; resize:none;'>");
                    sb.Append("</th><th>");
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Enquiries/AddItems.aspx"))
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

                    sb.Append("<th><input type='text' id='txtHSCode0'"
                       + "onkeyup='alphaNumericAnd_AndSpace_Ifen_Slash(this);' class='bcAsptextboxmulti_Footer'></th>");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ex.Message;
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
                //DataSet ds = (DataSet)Session["FloatEnquiryFPO"];
                //string[] val = HselectedItems.Value.Split(',').ToArray();
                //val = val.Where(s => !String.IsNullOrEmpty(s)).ToArray();

                //ArrayList NotSel = new ArrayList();
                //NotSel.AddRange(val);
                //if (NotSel.Count > 0)
                //{
                //    NotSel.RemoveAt(0);
                //    NotSel.Reverse();
                //    if (NotSel.Count > 0)
                //    {
                //        for (int i = 0; i < NotSel.Count; i++)
                //        {
                //            Guid del = new Guid(NotSel[i].ToString());
                //            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                //            {
                //                if (del == new Guid(ds.Tables[0].Rows[k]["ItemID"].ToString()))
                //                {
                //                    ds.Tables[0].Rows[k].Delete();
                //                    break;
                //                }
                //            }
                //        }
                //        ds.Tables[0].AcceptChanges();
                //    }
                //}


                DataTable dt = (DataTable)Session["ItemCode"];
                if (dt != null && dt.Rows.Count > 0 && txtDlvry.Text.Trim() != "" && ddlcustomer.SelectedValue != Guid.Empty.ToString())
                    SaveRecord();
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Fill All the Deatails Properly.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                Response.Redirect("../Purchases/NewFPOrderVerbal.Aspx");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
            }
        }

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "fillinbox()", true);
                //System.Threading.Thread.Sleep(1000);
                string filename = FileName();
                DataTable dssss = new DataTable();
                if (Session["ItemCode"] != null)
                {
                    dssss = (DataTable)Session["ItemCode"];
                }
                DataTable Itm = dssss.Clone();
                DataSet ds = ReadExcelData();
                if (ds.Tables[0].Columns.Count == 7)
                {
                    for (int i = 1; i <= 7; i++)
                    {
                        ds.Tables[0].Columns.Add(new DataColumn());
                        ds.Tables[0].Columns[6 + i].SetOrdinal(i);
                    }
                }
                var _Res = ds.Tables["Items$"].Select("Quantity is null or Rate is null").ToArray();                
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Contains("Items$") && _Res.Count() <= 0  )
                    {
                        DataTable _DT = ds.Tables["Items$"].Clone();
                        _DT.Columns["Part No"].DataType = typeof(String);
                        _DT.Load(ds.Tables["Items$"].CreateDataReader());
                        _DT.Rows.Cast<DataRow>().ToList().ForEach(P =>
                        {
                            P.SetField<String>("Item Description", Regex.Replace(P.Field<String>("Item Description") ?? "", @"\t|\n|\r", ""));
                            P.SetField<String>("Part No", Regex.Replace(P.Field<String>("Part No") ?? "", @"\t|\n|\r", ""));
                            P.SetField<String>("Specification", Regex.Replace(P.Field<String>("Specification") ?? "", @"\t|\n|\r", ""));
                        });
                        if (_DT.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0)).Count() > 0)
                            _DT = _DT.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0)).CopyToDataTable();
                        _DT.TableName = "ExcelData";

                        DataSet dsRtnItms = ItemMstBLl.BulkUploadItems_FPO(_DT, new Guid(Session["CompanyId"].ToString()),
                            new Guid(Session["UserId"].ToString()), filename, txtFpoNo.Text);

                        if (dsRtnItms != null && dsRtnItms.Tables.Count > 0)
                        {
                            try
                            {
                                string _Msg = "";
                                if (dsRtnItms.Tables[0] != null && dsRtnItms.Tables[0].Rows.Count > 0)
                                {
                                    if (dsRtnItms.Tables[0].Columns.Count == 3 && dsRtnItms.Tables[0].Columns[0].ColumnName == "DuplicateCodes")
                                    {
                                        if (dsRtnItms.Tables[0].Rows[0]["InvalidFormat"].ToString() != "")
                                            _Msg += string.Format(@"Incorrect Format: " + dsRtnItms.Tables[0].Rows[0]["InvalidFormat"].ToString());
                                        if (dsRtnItms.Tables[0].Rows[0]["DuplicateCodes"].ToString() != "")
                                            _Msg += string.Format(@"File Contains Duplicate Codes:\r\n" + dsRtnItms.Tables[0].Rows[0]["DuplicateCodes"].ToString());
                                        if (dsRtnItms.Tables[0].Rows[0]["ItemCodesMisMatch"].ToString() != "")
                                            _Msg += string.Format(@"Either ItemCode or Description,Specification,Partno are Mismatched with Existing Data: \r\n" + dsRtnItms.Tables[0].Rows[0]["ItemCodesMisMatch"].ToString());
                                        //_Msg += string.Format(@"Incorrect Format: " + string.Join(@",\r\n", dsRtnItms.Tables[0].AsEnumerable().Select(P => P.Field<String>("InvalidFormat")).Distinct().ToList()) + " ");
                                        //_Msg += string.Format(@"\r\nFile Contains Duplicate Codes: " + string.Join(@",\r\n", dsRtnItms.Tables[0].AsEnumerable().Select(P => P.Field<String>("DuplicateCodes")).Distinct().ToList()) + " ");
                                    }
                                    else if (dsRtnItms.Tables[0].Columns.Count == 13 && dsRtnItms.Tables[0].Columns[0].ColumnName == "Sno")
                                    {
                                        //Itm = dsRtnItms.Tables[0];
                                        Itm = dsRtnItms.Tables[dsRtnItms.Tables.Count - 1];
                                        //Itm.Merge(dssss, true, MissingSchemaAction.Ignore);
                                        //Itm.Merge(dssss, false, MissingSchemaAction.Add);
                                        //dssss.Merge(Itm);
                                        DataTable dt = Itm.DefaultView.ToTable(true);
                                        Itm = dt;
                                    }
                                    if (_Msg != "")
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + _Msg + "');", true);
                                }
                            }
                            catch (Exception Ex)
                            {
                                if (Ex.Message.Contains("does not belong to table"))
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + dsRtnItms.Tables[dsRtnItms.Tables.Count - 1].Rows[0]["ErrorMessage"].ToString().Replace("'", "") + "');", true);
                                else
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + Ex.Message.Replace("'", "") + "');", true);
                            }
                        }
                        Session["ItemCode"] = Itm;
                        HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        divFPOItems.InnerHtml = FillItemGrid(false);

                        //var Dic = Itm.AsEnumerable().ToDictionary(row => row.Field<Int64>(0), row => row.Field<Guid>(2));
                        //Codes = Dic;
                        //if (dsRtnItms != null && dsRtnItms.Tables.Count > 0)
                        //{
                        //    ELog.CreateErrorLog(Server.MapPath("../Logs/BulkUpload/ErrorLog"), "New Foreign Enquiry Bulk Upload", dsRtnItms.Tables[1].Rows[0][0].ToString().Trim(','));
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('these New Item Codes " + dsRtnItms.Tables[1].Rows[0][0].ToString() + " are unable to Insert');", true);
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAlertMessage", "alert('these New Item Codes \r\n" + dsRtnItms.Tables[1].Rows[0][0].ToString().Trim(',') + " \r\nare unable to Insert');", true);
                        //}
                        //else
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill Quatity & Rate Fields In All Columns.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry Bulk Upload", ex.Message.ToString());
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
                if (ddlcustomer.SelectedValue == Guid.Empty.ToString())
                    ClearAll();
                else
                    divFPOItems.InnerHtml = FillItemGrid(false);
                //if (chkbIRO.Checked && ddlRefFPO.Enabled)
                //{

                //}
                //else
                //{
                //    if (ddlcustomer.SelectedValue != Guid.Empty.ToString())
                //    {
                //        string cstmr = ddlcustomer.SelectedValue;
                //        ClearAll();
                //        ddlcustomer.SelectedValue = cstmr;
                //        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "",
                //        "", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                //    }
                //    else
                //    {
                //        ClearAll();
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                //string ForEnq = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //if (Lstfenqy.SelectedValue != "0")
                //{
                //    FillInputFields(ForEnq);

                //}
                //else if (Request.QueryString["FeqID"] != null)
                //    FillInputFields(Request.QueryString["FeqID"]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                if (ddlRefFPO.SelectedValue != "0")
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                    //ddlRefFPO.Enabled = true;
                    ddlRefFPO.Enabled = true;
                    ddlRefFPO.Visible = true;
                    spnRfpolbl.Visible = true;
                    BindReferenceDDL();
                }
                else
                {
                    //ddlRefFPO.Enabled = false;
                    //ddlRefFPO.SelectedValue = Guid.Empty.ToString();
                    //ClearAll();
                    ddlRefFPO.Enabled = false;
                    ddlRefFPO.SelectedValue = Guid.Empty.ToString();
                    ddlRefFPO.Visible = false;
                    spnRfpolbl.Visible = false;
                    ClearAllForRptOrdrUncheck();
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
                    ds.Tables[0].Rows[RNo - 1]["Quantity"] = Qty;
                    ds.Tables[0].Rows[RNo - 1]["Amount"] = (Convert.ToDecimal(Qty) * Rate);
                }
                if (Units != "" && Units != "0")
                {
                    ds.Tables[0].Rows[RNo - 1]["UNumsId"] = Units;
                    DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + Units.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                    DataRow[] selRow = dss.Tables[0].Select("ID = " + sv.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "FPO Verbal", ex.Message.ToString());
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
                        int sno = 0;
                        try { sno = Convert.ToInt16(dt.Compute("Max(SNo)", string.Empty)); }catch { }
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

        #endregion

        # region WebMethods ADD items

        /// <summary>
        /// To Fill Item Grid view
        /// </summary>
        /// <param name="RowID">Selected Row No</param>
        /// <param name="SNo">row S.No.</param>
        /// <param name="CodeID">Item ID</param>
        /// <param name="ID"></param>
        /// <param name="CatID">Category ID</param>
        /// <param name="UnitID">Unit Id</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string FillItemGrid(int RowID, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID, string HSCode, string Rate, string Remarks)
        {
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataRow dr = dt.NewRow();
                dr["SNo"] = RowID;
                dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                dr["ItemDescription"] = ItemID;
                dr["PartNo"] = PartNo;
                dr["Specification"] = Spec;
                dr["Make"] = Make;
                dr["Quantity"] = Qty;
                dr["Units"] = UnitID;
                dr["HSCode"] = HSCode;
                dr["ID"] = Guid.Empty;

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
                HttpContext.Current.Session["ItemCode"] = dt;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                DataRow[] dr = dt.Select("ItemDescription = " + "'" + ItemID + "'");
                if (dr.Length > 0)
                    dr[0].Delete();
                dt.AcceptChanges();

                HttpContext.Current.Session["SelectedItems"] = Codes;
                HttpContext.Current.Session["ItemCode"] = dt;

                DataTable dtt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataSet dss = new DataSet();
                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                return FillItemGrid(false);//0, dtt, Codes, dss, Convert.ToInt32(GeneralCtgryID), CatCodes, 0, UnitCodes, ""
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                    DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                    DataRow dr = dt.NewRow();
                    dr["SNo"] = Convert.ToInt32(dt.Rows[rowNo - 1]["SNo"]) + 1;// rowNo + 1;
                    dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dr["ItemDescription"] = 0;
                    dr["PartNo"] = string.Empty;
                    dr["Specification"] = string.Empty;
                    dr["Make"] = string.Empty;
                    dr["Quantity"] = 0;
                    dr["Units"] = 0;
                    dr["ID"] = 0;
                    dt.Rows.Add(dr);

                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                    HttpContext.Current.Session["SelectedItems"] = Codes;
                    HttpContext.Current.Session["ItemCode"] = dt;
                    int count = dt.Rows.Count;
                    return FillItemGrid(false);//rowNo, SNo, CodeID, 1, 0, 0, "", "", "", "", ""
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                string Rslt = "";
                Guid ItemIDInt = new Guid(ItemID.Trim());
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] result = dss.Tables[0].Select("ID = " + "'" + ItemIDInt + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt = dr["PartNumber"].ToString() + " &@&" + dr["Specification"].ToString() + " &@& " + dr["Itemdescription"].ToString();
                    }
                }
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                DataTable dtt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                if (dtt != null && dtt.Rows.Count > 0 && dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dsRow = dss.Tables[0].Select("ID = " + "'" + ItemID + "'");
                    foreach (DataRow drr in dsRow)
                    {
                        Rslt = drr["ID"].ToString() + " &@&" + drr["ItemDescription"].ToString() + " &@&";
                    }

                    DataRow[] result = dtt.Select("ItemDescription = " + "'" + ItemID + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt += dr["PartNo"].ToString() + " &@&"
                              + dr["Specification"].ToString() + " &@&"
                              + dr["Make"].ToString() + " &@&"
                              + dr["Quantity"].ToString() + " &@&"
                              + dr["Units"].ToString() + " &@&"
                              + dr["HSCode"].ToString() + " &@&"
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to update the Items Selected
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ItemID"></param>
        /// <param name="PartNo"></param>
        /// <param name="Spec"></param>
        /// <param name="Make"></param>
        /// <param name="Qty"></param>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string UpdateSelectedItem(string value, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID, string HSCode, string Rate, string Remarks)
        {
            try
            {
                string Rslt = "";
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                string[] val = value.Split(',');
                if (dt != null && dt.Rows.Count > 0 && val.Length > 0)
                {
                    int rowNo = Convert.ToInt32(val[0]) - 1;
                    dt.Rows[rowNo]["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dt.Rows[rowNo]["ItemDescription"] = ItemID;
                    dt.Rows[rowNo]["PartNo"] = PartNo;
                    dt.Rows[rowNo]["Specification"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                        dt.Rows[rowNo]["Quantity"] = 0;
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["units"] = UnitID;
                    dt.Rows[rowNo]["HSCode"] = HSCode;

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
                    Session["ItemCode"] = dt;
                }
                var _Res = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                if (_Res != null)
                {
                    _Res[Convert.ToInt32(val[0])] = new Guid(ItemID);
                }
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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

        //[Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        //public bool CheckStat(string itmId, int rowNo)
        //{
        //    try
        //    {
        //        if (EditID != null && EditID != Guid.Empty)
        //        {
        //            if (Session["checkstat"] != null)
        //            {
        //                DataTable checkstat = (DataTable)Session["checkstat"];
        //                DataRow[] drow = checkstat.Select("ItemId = " + "'" + itmId + "'");
        //                int Cnt = drow.Count();
        //                if (Cnt > 0)
        //                    return (drow[0]["ChkState"].ToString() == "true" ? true : false);
        //                else
        //                    return true;
        //            }
        //            else
        //                return false;
        //        }
        //        else
        //            return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
        //        return false;
        //    }
        //}


        [WebMethod]
        /// <summary>
        /// To Retrive Auto Complete Data From DataBase 
        /// </summary>
        /// <param name="Txt_Text"></param>
        /// <returns></returns>
        public static List<string> GetAutoCompleteData(string Txt_Text)
        {
            try
            {
                var _SelectedItems = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                return ((DataSet)HttpContext.Current.Session["ItemsDescPrtNo"]).Tables[0].AsEnumerable()
                                      .Where(P => P.Field<string>("ItemDescription").ToLower().StartsWith(Txt_Text.ToLower()) &&
                                          !_SelectedItems.Select(TT => TT.Value.ToString()).Contains(P.Field<Guid>("ID").ToString()))
                                      .Select(C => C.Field<string>("ItemDescription") + " VAL--VAL--VAL " + C.Field<Guid>("ID").ToString()).Distinct().ToList();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ErrorLog ELog = new ErrorLog();
                ELog.CreateErrorLog(HttpContext.Current.Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return null;
            }
        }
        # endregion
    }
}