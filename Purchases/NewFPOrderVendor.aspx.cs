﻿using System;
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
using System.IO;

namespace VOMS_ERP.Purchases
{
    public partial class NewFPOrderVendor : System.Web.UI.Page
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
        SupplierBLL SBLL = new SupplierBLL();
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
        #endregion

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

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
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewFPOrderVendor));
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
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal Verbal", ex.Message.ToString());
            }

        }

        #region Methods

        /// <summary>
        /// Get LPO Items
        /// </summary>
        private void GetLPOitems50()
        {
            try
            {
                string FEID = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                long StatID = CommonBLL.StatusTypeFPOrder;
                if (Request.QueryString["ID"] != null)
                    StatID = CommonBLL.StatusTypeLPOrder;
                DataSet items = NFPOBLL.Select(CommonBLL.FlagKSelect, Guid.Empty, FEID, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                     DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, StatID, Guid.Empty, false, false, false, "",
                     new Guid(Session["UserID"].ToString()), DateTime.Now,
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPOForCheckList(),
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal Verbal", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                string feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                divListBox.InnerHtml = AttachedFiles();
                GetGeneralID();
                BindDropDownList(ddlPrcBsis, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlRsdby, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindDropDownList(ddldept, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                BindDropDownList_Vendor(ddlVendor, SBLL.GetVendorsByFe(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, feno_Multi, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindRadioList(rbtnshpmnt, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ShipmentMode));
                if (ddlcustomer.Items.Count > 1)
                {
                    ddlcustomer.SelectedIndex = 1;
                    CustomerSelectionChanged();
                }
                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    lblCustomerNm.Text = ddlcustomer.SelectedItem.Text;
                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"] != "")
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                    if (!IsPostBack)
                        BindDropDownList(ddlRefFPO, NEBL.SelectFenquiries(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            DateTime.Now, DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));

                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    if (Request.QueryString.Count > 2 && Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "" && Request.QueryString["FqId"] != null
                                    && Request.QueryString["FqId"].ToString() != "")
                    {

                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }

                        FillInputFields();
                        feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        BindDropDownList_Vendor(ddlVendor, SBLL.GetVendorsByFe(CommonBLL.FlagVSelect, new Guid(ddlcustomer.SelectedValue), Guid.Empty, feno_Multi, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    }
                    else if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        FillInputFields(Request.QueryString["FeqID"]);
                        feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        BindDropDownList_Vendor(ddlVendor, SBLL.GetVendorsByFe(CommonBLL.FlagVSelect, new Guid(ddlcustomer.SelectedValue), Guid.Empty, feno_Multi, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    }
                }
                else
                {

                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //To Select the Passed values
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        FillInputFields(Request.QueryString["FeqID"]);
                        feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        BindDropDownList_Vendor(ddlVendor, SBLL.GetVendorsByFe(CommonBLL.FlagVSelect, new Guid(ddlcustomer.SelectedValue), Guid.Empty, feno_Multi, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal", ex.Message.ToString());
            }
        }

        private void ForeignQuotationNo(ListBox cntrl, DataSet ds)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ddl.Items.Clear();
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataValueField = "ID";
                    ddl.DataTextField = "Description";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                //To Bind Data in to the ListBox
                if (ddlcustomer.SelectedValue != Guid.Empty.ToString() && CommonDt != null && CommonDt.Tables.Count > 0)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }
        }

        protected void BindDropDownList1(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = CommonDt.Tables[0].Columns[1].ColumnName;
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                //To Bind Data in to the ListBox
                if (ddlVendor.SelectedValue != Guid.Empty.ToString())
                {
                    CommonDt.Tables[0].DefaultView.Sort = "Description ASC";
                    Lstfqn.DataSource = CommonDt.Tables[0];
                    Lstfqn.DataTextField = "Description";
                    Lstfqn.DataValueField = "ID";
                    Lstfqn.DataBind();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }

        }


        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList_Vendor(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataValueField = "ID";
                    ddl.DataTextField = "Description";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Binding Vendors in DropDownList
        /// </summary>
        /// <param name="FEID"></param>
        /// <param name="CustId"></param>
        private void GetVendorNames(Guid FPOID, Guid CustId)
        {
            try
            {
                ddlVendor.Items.Clear();
                string feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ds = new DataSet();
                ds = SBLL.GetVendorsByFe(CommonBLL.FlagISelect, CustId, Guid.Empty, feno_Multi, FPOID, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "BussName ASC";
                    ddlVendor.DataSource = ds.Tables[0];
                    ddlVendor.DataTextField = "BussName";
                    ddlVendor.DataValueField = "ID";
                    ddlVendor.DataBind();
                    ddlVendor.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                    //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                    //FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPOForCheckList(),
                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (FQuoteDeatils.Tables.Count >= 2 && FQuoteDeatils.Tables[0].Rows.Count > 0 &&
                    (FQuoteDeatils.Tables[1].Rows.Count >= 0))
                {
                    txtfenqDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(FQuoteDeatils.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                    txtsubject.Text = FQuoteDeatils.Tables[0].Rows[0]["Subject"].ToString();
                    txtimpinst.Text = FQuoteDeatils.Tables[0].Rows[0]["Instruction"].ToString();
                    ddldept.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlRsdby.SelectedValue = FQuoteDeatils.Tables[0].Rows[0]["DepartmentId"].ToString();
                    txtFpoDt.Text = CommonBLL.DateDisplay(DateTime.Now);
                    //txtFpoDuedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ?
                    //    DateTime.Now.AddDays(5).Date : DateTime.Now.AddDays(6).Date);
                    //DataTable dtt = new DataTable();
                    //DataView MyView = new DataView();
                    //dtt = FQuoteDeatils.Tables[1].Copy();
                    //MyView = dtt.DefaultView;
                    //MyView.Sort = "FESNo ASC";
                    //dtt = MyView.ToTable();

                    //DataColumn dc = new DataColumn("Check", typeof(bool));
                    //dc.DefaultValue = true;
                    //if (!dtt.Columns.Contains("Check"))
                    //    dtt.Columns.Add(dc);

                    //DataSet ds = new DataSet();
                    //ds.Tables.Add(dtt);
                    //ds.Tables[0].DefaultView.Sort = "FESNo ASC";
                    //Session["FloatEnquiryFPO"] = ds;
                    //GetLPOitems50();
                    //divFPOItems.InnerHtml = FillGridView(Empty);
                    //divPaymentTerms.InnerHtml = FillPaymentTerms();

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }
        }

        private void FillInputFields1(string FQno)
        {
            try
            {
                DataTable EmptyFPO = CommonBLL.EmptyDtNewFPOForCheckList();
                if (EmptyFPO.Columns.Contains("ItemDetailsId"))
                    EmptyFPO.Columns.Remove("ItemDetailsId");
                string FEnq = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet FQuoteDeatils = NFPOBLL.SelectFPOVendor(CommonBLL.FlagHSelect, Guid.Empty, FEnq, new Guid(ddlcustomer.SelectedValue), Guid.Empty,
                    Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, FQno, "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "",
                    DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                     new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmptyFPO,
                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (FQuoteDeatils.Tables.Count >= 3 && FQuoteDeatils.Tables[0].Rows.Count > 0 &&
                    (FQuoteDeatils.Tables[1].Rows.Count > 0 || FQuoteDeatils.Tables[2].Rows.Count > 0))
                {
                    DataTable TOTAL_QTY = null;
                    if (FQuoteDeatils.Tables.Count > 3)
                        TOTAL_QTY = FQuoteDeatils.Tables[3];
                    Session["TOTAL_QTY"] = TOTAL_QTY;

                    txtFpoDuedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ?
                    DateTime.Now.AddDays(5).Date : DateTime.Now.AddDays(6).Date);

                    DataTable dtt = new DataTable();
                    DataView MyView = new DataView();
                    dtt = FQuoteDeatils.Tables[2].Copy();
                    MyView = dtt.DefaultView;
                    MyView.Sort = "FESNo ASC";
                    dtt = MyView.ToTable();
                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    if (!dtt.Columns.Contains("Check"))
                        dtt.Columns.Add(dc);
                    for (int h = 0; h < dtt.Rows.Count; h++)
                    {
                        if (dtt.Rows[h]["FPORelease"] != "" || dtt.Rows[h]["FPORelease"] == "0.00")
                            dtt.Rows[h]["Check"] = false;
                        else
                            dtt.Rows[h]["Check"] = true;
                        //if (dtt.Rows[h]["FPORelease"] != "" && dtt.Rows[h]["FPORelease"] != "0.00")
                        //    dtt.Rows[h]["Quantity"] = dtt.Rows[h]["FPORelease"];

                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtt);
                    ds.Tables[0].DefaultView.Sort = "FESNo ASC";

                    ds.Tables[0].Columns.Add("QTY", typeof(decimal));//Need to Remove This Column Before Saving.


                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (TOTAL_QTY != null && TOTAL_QTY.Rows.Count > 0)
                        {
                            DataRow rowsToUpdate = TOTAL_QTY.AsEnumerable().FirstOrDefault(r => r.Field<Guid>("itemid") == row.Field<Guid>("itemid"));
                            if (rowsToUpdate != null)
                            {
                                decimal quantity = 0;
                                if (rowsToUpdate.Field<decimal>("QTY") > row.Field<decimal>("quantity"))//Received QTY > ActualQTY
                                    quantity = 0;// (rowsToUpdate.Field<decimal>("QTY") - row.Field<decimal>("quantity"));
                                else if (rowsToUpdate.Field<decimal>("QTY") < row.Field<decimal>("quantity"))
                                    quantity = (row.Field<decimal>("quantity") - rowsToUpdate.Field<decimal>("QTY"));

                                row.SetField("quantity", quantity > 0 ? quantity : 0);
                                row.SetField("amount", Math.Abs(row.Field<decimal>("quantity") * row.Field<decimal>("rate")));
                                row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                            }
                            else
                                row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                        }
                        else
                            row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                    }

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

                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    //    "ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Insert Update records
        /// </summary>
        private void SaveRecord()
        {
            try
            {
                int res = 1;
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
                if (ds.Tables[0].Columns.Contains("IsSubItems"))
                    ds.Tables[0].Columns.Remove("IsSubItems");
                if (ds.Tables[0].Columns.Contains("CompanyId"))
                    ds.Tables[0].Columns.Remove("CompanyId");
                if (ds.Tables[0].Columns.Contains("ItemStatus"))
                    ds.Tables[0].Columns.Remove("ItemStatus");
                if (ds.Tables[0].Columns.Contains("FPORelease"))
                    ds.Tables[0].Columns.Remove("FPORelease");
                if (ds.Tables[0].Columns.Contains("NN"))
                    ds.Tables[0].Columns.Remove("NN");
                if (ds.Tables[0].Columns.Contains("itd"))
                    ds.Tables[0].Columns.Remove("itd");
                if (ds.Tables[0].Columns.Contains("QTY"))
                    ds.Tables[0].Columns.Remove("QTY");

                IEnumerable<DataRow> query = from order in ds.Tables[0].AsEnumerable()
                                             where order.Field<bool>("Check") == true && order.Field<decimal>("Quantity") > 0
                                             select order;

                DataTable FPOdt = query.CopyToDataTable<DataRow>();

                if (FPOdt.Columns.Contains("Check"))
                    FPOdt.Columns.Remove("Check");
                FPOdt.AcceptChanges();

                //FPOdt = ds.Tables[0].Copy();

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

                string FQNums = String.Join(",", Lstfqn.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string Attachments = "";
                if (Session["FPOUploads"] != null)
                {
                    ArrayList all = (ArrayList)Session["FPOUploads"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                if (FPOdt.Rows.Count > 0 && FENo != "")
                {
                    if (btnSave.Text == "Save" && Request.QueryString["ID"] != null && Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"].ToString() == "True")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagASelect, new Guid(Request.QueryString["ID"]), FENo, CID, DPT, Guid.Empty,
                         CommonBLL.DateInsert(txtfenqDt.Text), FQNums, new Guid(ddlVendor.SelectedValue), DateTime.Now, txtFpoNo.Text.Trim(), "",
                         txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text), CommonBLL.DateInsert(txtReceivedDate.Text),
                         CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "", PriceBasis, txtPriceBasis.Text, DeliveryDt,
                         DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrderVendor, new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked,
                         false, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                         FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    else if (btnSave.Text == "Save")
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagCommonMstr, Guid.Empty, FENo, CID, DPT, Guid.Empty, CommonBLL.DateInsert(txtfenqDt.Text), FQNums,
                         new Guid(ddlVendor.SelectedValue), DateTime.Now, txtFpoNo.Text.Trim(), "", txtsubject.Text.Trim(), CommonBLL.DateInsert(txtFpoDt.Text),
                         CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby, txtimpinst.Text.Trim(), "",
                         PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2,
                         CommonBLL.StatusTypeFPOrderVendor, new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked, false, "", new Guid(Session["UserID"].ToString()),
                         DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    else
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), FENo, CID, DPT, Guid.Empty,
                        CommonBLL.DateInsert(txtfenqDt.Text), FQNums, new Guid(ddlVendor.SelectedValue), DateTime.Now, txtFpoNo.Text.Trim(), "", txtsubject.Text.Trim(),
                        CommonBLL.DateInsert(txtFpoDt.Text), CommonBLL.DateInsert(txtReceivedDate.Text), CommonBLL.DateInsert(txtFpoDuedt.Text), FPORsdby,
                        txtimpinst.Text.Trim(), "", PriceBasis, txtPriceBasis.Text, DeliveryDt, DeliveryPeriod, 2, CommonBLL.StatusTypeFPOrderVendor,
                        new Guid(rbtnshpmnt.SelectedValue), Chkbivac.Checked, Chkcotecna.Checked, false, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, FPOdt, Paymentdt, TCs, Attachments, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            //SendDefaultMails(cusmr.SelectCustomers(CommonBLL.FlagISelect, new Guid(ddlVendor.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                        }
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO Vendor", "Data inserted successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOVendorStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO Vendor", "Error while Inserting.");
                    }
                    if (res == 0 && btnSave.Text == "Update")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "New FPO Vendor", "Data Updated successfully.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FPOVendorStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New FPO Vendor", "Error while Updating.");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ddlVendor.Enabled = false;
                Lstfqn.Enabled = false;
                DataSet EditDS = NFPOBLL.Select(CommonBLL.FlagModify, new Guid(ID), "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(),
                     DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "",
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
                    ddlcustomer.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                    string FEnqids = EditDS.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();

                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEditForRepetedFPOCheck(CommonBLL.FlagQSelect, Guid.Empty, Guid.Empty,
                        new Guid(EditDS.Tables[0].Rows[0]["CusmorId"].ToString()), Guid.Empty, FEnqids,
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", status, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                        CommonBLL.EmptyDt(), Session["Custmr_SuplrID"].ToString()));

                    string[] SptFEnqids = FEnqids.Split(',');
                    foreach (string s in SptFEnqids)
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
                    //ddlVendor.SelectedValue = EditDS.Tables[0].Rows[0]["VendorId"].ToString();
                    if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                    {
                        BindDropDownList_Vendor(ddlVendor, SBLL.GetVendorsByFe(CommonBLL.FlagModify, new Guid(ddlcustomer.SelectedValue), Guid.Empty, FEnqids, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    }
                    ddlVendor.SelectedValue = EditDS.Tables[0].Rows[0]["VendorId"].ToString();

                    ForeignQuotationNo(Lstfqn, NFQBL.SelectForFPOVendor(CommonBLL.FlagWCommonMstr, Guid.Empty, new Guid(ddlcustomer.SelectedValue), new Guid(ddlVendor.SelectedValue), Guid.Empty, FEnqids, "", "",
                                     DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, status, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDtFQ(),
                                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString())));
                    Lstfqn.Items.Cast<ListItem>().Select(n => n).ToList().ForEach(n => n.Selected = true);

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
                    DataTable dtt = new DataTable(); DataTable dtt_Clone = new DataTable();
                    dtt = EditDS.Tables[1].Copy();
                    if (EditDS.Tables.Count >= 4 && EditDS.Tables[4].Rows.Count > 0)
                    {
                        dtt_Clone = EditDS.Tables[4].Copy();
                        DataColumn dc_Clone = new DataColumn("Check", typeof(bool));
                        dc_Clone.DefaultValue = false;
                        if (!dtt_Clone.Columns.Contains("Check"))
                            dtt_Clone.Columns.Add(dc_Clone);

                    }
                    DataColumn dc = new DataColumn("Check", typeof(bool));
                    dc.DefaultValue = true;
                    if (!dtt.Columns.Contains("Check"))
                        dtt.Columns.Add(dc);
                    dtt.Merge(dtt_Clone);
                    //DataView dv = dtt.DefaultView;
                    //dv.Sort = "FESNo ASC";
                    //dtt = dv.ToTable();
                    dtt.DefaultView.Sort = "FESNo ASC";
                    dtt = dtt.DefaultView.ToTable();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dtt);
                    ds = RemoveColumns(ds);

                    DataTable TOTAL_QTY = null;
                    if (EditDS.Tables.Count > 5)
                        TOTAL_QTY = EditDS.Tables[5];
                    Session["TOTAL_QTY"] = TOTAL_QTY;

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        //(Convert.ToDecimal(row["QTY"].ToString()) != Convert.ToDecimal(row["QTY"].ToString())) &&
                        if (TOTAL_QTY != null && TOTAL_QTY.Rows.Count > 0)
                        {
                            DataRow rowsToUpdate = TOTAL_QTY.AsEnumerable().FirstOrDefault(r => r.Field<Guid>("itemid") == row.Field<Guid>("itemid"));
                            if (rowsToUpdate != null)
                            {
                                if (row["QTY"].ToString() == "")
                                    row["QTY"] = 0;
                                decimal QTY = ((rowsToUpdate.Field<decimal>("AQTY") - rowsToUpdate.Field<decimal>("QTY")) + row.Field<decimal>("QTY"));
                                row.SetField("QTY", QTY > 0 ? QTY : 0);
                                row.SetField("amount", Math.Abs(row.Field<decimal>("quantity") * row.Field<decimal>("rate")));
                                //row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                            }
                            else
                                row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                        }
                        else
                            row.SetField("QTY", Math.Abs(row.Field<decimal>("quantity")));
                    }

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
                    if (Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                        btnSave.Text = "Save";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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

                Session["CPage_FPO"] = 1;
                Session["RowsDisplay_FPO"] = 100;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                DataSet dsi = new DataSet(); string ItemIDs = string.Empty;
                if (EnqID != "")
                {
                    dsi = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(EnqID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0,
                        0, 0, 0, 0, Guid.Empty, "", new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                    dsi = RemoveColumns(dsi);
                    Session["FloatEnquiryFPO"] = dsi;
                    Session["TempFloatEnquiry"] = dsi;
                    Session["EnqID"] = EnqID;
                }
                else
                    dsi = (DataSet)Session["FloatEnquiryFPO"];

                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, GenSupID, new Guid(Session["CompanyID"].ToString()));
                dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                #region Paging

                string DisablePrevious = " disabled ", DisableNext = " disabled ";
                int Rows2Display = Convert.ToInt32(Session["RowsDisplay_FPO"].ToString()), CurrentPage = Convert.ToInt32(Session["CPage_FPO"].ToString()), Rows2Skip = 0;
                int RowsCount = dsi.Tables[0].Rows.Count, PageCount = 0;

                if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
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
                    Session["CPage_FPO"] = CurrentPage;
                }

                #endregion

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblItems'>" +
                "<thead align='left'><tr >");
                sb.Append("<th class='rounded-First'>&nbsp;</th>");//<th><input id='ckhHeader type='checkbox' checked='checked' ></th>
                sb.Append("<th>SNo</th><th>Item Description</th><th align='center'>Part No</th><th align='center'>Specification</th>" +
                "<th align='center'>Make</th><th>Quantity</th><th>Units</th><th align='right'>Rate($)</th><th align='right'>Amount($)</th>" +
                "<th align='center'>Remarks</th><th class='rounded-Last'>Regret</th></tr></thead>");
                sb.Append("<tbody class='bcGridViewMain'>");
                if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                {
                    if (Session["FPOSelectedItems"] == null || Session["RegrettedItems"] == null || Session["RegrettedItems"] == "" || Session["FPOSelectedItems"] == "")
                        GetLPOitems50();

                    Dictionary<string, string> FPOSelectedItems = new Dictionary<string, string>();
                    Dictionary<string, string> RegrettedItems = new Dictionary<string, string>();
                    string FPOItems = Session["FPOSelectedItems"].ToString();
                    string RegRetedItms = Session["RegrettedItems"].ToString();

                    FPOSelectedItems = FPOItems.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                    RegrettedItems = RegRetedItms.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());

                    #region Paging

                    DataSet ds = new DataSet();
                    DataTable dt = dsi.Tables[0].AsEnumerable().Skip(Rows2Skip).Take(Rows2Display).CopyToDataTable();
                    ds.Tables.Add(dt);

                    #endregion

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

                        sb.Append("<td><input type='text' name='txtQuantity' dir='rtl' size='05px' onchange='CheckQTY(" + sno
                            + ", 0)' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString()
                            + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' "
                            + " maxlength='6' class='bcAsptextbox' style='width:50px;'/>"
                            + " <input type='hidden' name='hfQTY' id='hfQTY" + sno + "' value='"
                            + (ds.Tables[0].Columns.Contains("QTY") ? ds.Tables[0].Rows[i]["QTY"].ToString() : ds.Tables[0].Rows[i]["Quantity"].ToString()) + "'/>"
                            + "</td>");

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
                    sb.Append("<th colspan='6' align='right' class='rounded-foot-left'>");

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

                    sb.Append("<th colspan='4' align='right'><b><span>Total Amount($) : " + TotalAmount.ToString("N") + "</span></b></th>");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                BindDropDownList(ddlRefFPO, NFPOBLL.SelectRepetedFpoBind(CommonBLL.FlagGSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "",
                    "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 50, Guid.Empty, false, false, "",
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                        "ErrorMessage('Fill All the Details Properly.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal Vendor", ex.Message.ToString());
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
                Response.Redirect("../Purchases/NewFPOrderVendor.Aspx");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
            CustomerSelectionChanged();
        }

        private void CustomerSelectionChanged()
        {
            try
            {
                if (!chkbIRO.Checked && !ddlRefFPO.Enabled)
                {
                    if (ddlcustomer.SelectedValue != Guid.Empty.ToString())
                    {
                        string cstmr = ddlcustomer.SelectedValue;
                        ClearAll();
                        ddlcustomer.SelectedValue = cstmr;
                        BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", 50, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    }
                    else
                        ClearAll();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal Verbal", ex.Message.ToString());
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
                if (Lstfenqy.SelectedValue != "0" || Lstfenqy.SelectedValue != "")
                {
                    Guid FEID = Guid.Empty;
                    if (!chkbIRO.Checked)
                        FEID = new Guid(ForEnq.Split(',')[0]);
                    FillInputFields(ForEnq);
                    DataSet ds = new DataSet();
                    ds = SBLL.GetVendorsByFe(CommonBLL.FlagVSelect, new Guid(ddlcustomer.SelectedValue), FEID, ForEnq, new Guid(ddlRefFPO.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].DefaultView.Sort = "Description ASC";
                        ddlVendor.DataSource = ds.Tables[0];
                        ddlVendor.DataTextField = "Description";
                        ddlVendor.DataValueField = "ID";
                        ddlVendor.DataBind();
                        ddlVendor.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                        //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                        //FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                        if (!chkbIRO.Checked)
                            txtFpoNo.Text = Lstfenqy.Items.FindByValue(FEID.ToString()).Text + "/" + ds.Tables[1].Rows[0]["RCount"].ToString();
                        else
                        {
                            txtFpoNo.Text = "Rep/" + ddlRefFPO.SelectedItem.Text + "/" + ds.Tables[1].Rows[0]["RCount"].ToString();
                        }
                    }
                    else
                    {
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        //"ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('FPO is released to Floated Vendor/Supplier(s)');", true);
                    }

                }
                else if (Request.QueryString["FeqID"] != null)
                    FillInputFields(Request.QueryString["FeqID"]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                    string feno_Multi = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    FlagRepeat = 1;
                    EditRecord(ddlRefFPO.SelectedValue, CommonBLL.StatusTypeRepeatedFPO);
                    ddlcustomer.Enabled = true;
                    DataSet ds = new DataSet();
                    ds = SBLL.GetVendorsByFe(CommonBLL.FlagVSelect, new Guid(ddlcustomer.SelectedValue), Guid.Empty, feno_Multi, new Guid(ddlRefFPO.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].DefaultView.Sort = "BussName ASC";
                        ddlVendor.DataSource = ds.Tables[0];
                        ddlVendor.DataTextField = "BussName";
                        ddlVendor.DataValueField = "ID";
                        ddlVendor.DataBind();
                        //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                        //FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                    }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                    Lstfenqy.Items.Clear();
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

        #region Paging

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ValidateRowsBeforeSave()
        {
            try
            {
                string Error = "";
                DataSet ds = (DataSet)Session["FloatEnquiryFPO"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dr = ds.Tables[0].Select("Check = 'true' and (Quantity = 0 or Rate = 0)");
                    if (dr.Length > 0)
                    {
                        string SNO = dr[0]["fesno"].ToString();
                        string qty = dr[0]["Quantity"].ToString();
                        string rate = dr[0]["Rate"].ToString(), ErrorMsg = "";
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
        public string NextPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_FPO"] = (CPage + 1);
            Session["RowsDisplay_FPO"] = txtRowsChanged;
            return FillGridView("");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PrevPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_FPO"] = (CPage - 1);
            Session["RowsDisplay_FPO"] = txtRowsChanged;
            return FillGridView("");
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string RowsChanged(string CurrentPage, string txtRowsChanged)
        {
            if (txtRowsChanged.Trim() != "")
            {
                if (Convert.ToInt32(Session["RowsDisplay_FPO"].ToString()) != Convert.ToInt32(txtRowsChanged))
                {
                    int RowStart = ((Convert.ToInt32(Session["RowsDisplay_FPO"].ToString()) * Convert.ToInt32(CurrentPage)) - Convert.ToInt32(Session["RowsDisplay_FPO"].ToString())) + 1;

                    if (RowStart > Convert.ToInt32(txtRowsChanged))
                        Session["CPage_FPO"] = RowStart / Convert.ToInt32(txtRowsChanged) + 1;
                    else if (RowStart < Convert.ToInt32(txtRowsChanged) && RowStart != 1)
                        Session["CPage_FPO"] = (Convert.ToInt32(txtRowsChanged) + 1) / RowStart;
                }
                Session["RowsDisplay_FPO"] = Convert.ToInt32(txtRowsChanged);
            }
            return FillGridView("");
        }

        #endregion

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
                decimal Quantity;
                if (decimal.TryParse(Qty, out Quantity) && Quantity > -1)
                {
                    ds.Tables[0].Rows[RNo - 1]["Quantity"] = Quantity;
                    ds.Tables[0].Rows[RNo - 1]["Amount"] = (Quantity * Rate);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
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
        public string CheckItem(string IsChecked, string ID, string ItemId)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiryFPO"];
                int rowNo = Convert.ToInt32(ID);
                //for (int g = 0; g < ds.Tables[0].Rows.Count; g++)
                //{
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && (ds.Tables[0].Rows[rowNo - 1]["FPORelease"] != "0.00" && ds.Tables[0].Rows[rowNo - 1]["FPORelease"] == "")
                    //&& ds.Tables[0].Rows[rowNo-1]["check"] != "false")
                    && ds.Tables[0].Rows[rowNo - 1]["ItemId"].ToString() == ItemId)
                {
                    ds.Tables[0].Rows[rowNo - 1]["Check"] = Convert.ToBoolean(IsChecked);
                    ds.Tables[0].AcceptChanges();
                    Session["FloatEnquiryFPO"] = ds;
                }
                //else
                //{

                //}
                //}
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

        protected void ddlVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlVendor.SelectedValue != Guid.Empty.ToString())
                {
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    string feno = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    string vendor = ddlVendor.SelectedValue;
                    DataSet FQuotations = NFQBL.SelectForFPOVendor(CommonBLL.FlagXSelect, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, Guid.Empty, feno, "", "",
                                     DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, 50, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDtFQ(),
                                     CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(ddlVendor.SelectedValue));
                    ForeignQuotationNo(Lstfqn, FQuotations);
                }
                else
                {
                    Lstfqn.Items.Clear();
                    //ForeignQuotationNo(Lstfqn, null);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    //     "ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
                if (Lstfqn.SelectedValue == "")
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal Verbal", ex.Message.ToString());
            }
        }

        protected void Lstfqn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string FQno = String.Join(",", Lstfqn.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FEnq = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (Lstfqn.SelectedValue != Guid.Empty.ToString())
                {
                    FillInputFields1(FQno);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Foreign Purchase Order Verbal Verbal", ex.Message.ToString());
            }
        }
    }
}
