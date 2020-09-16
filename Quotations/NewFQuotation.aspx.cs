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
using System.IO;
using System.Text;
using System.Collections.Generic;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Quotations
{
    public partial class NewFQuotation : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        public static ArrayList Files = new ArrayList();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBL = new NewEnquiryBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        LEnquiryBLL NLEBL = new LEnquiryBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        CustomerBLL cusmr = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        CheckBLL CBL = new CheckBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        static string GeneralCtgryID;
        static DataSet EditDS, EditDSet;
        static string btnSaveID = "";
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        static int GenSupID; decimal RunningTotal = 0;
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        //static DataTable dtItems = new DataTable();
        //static DataTable dtt = new DataTable();
        //static DataSet dsItems = new DataSet();
        //static DataSet dsEnm = new DataSet();

        # endregion

        #region Default Page Load

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
                        txtfquoteno.Attributes.Add("readonly", "readonly");
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewFQuotation));
                        if (!IsPostBack)
                        {
                            ClearAll();
                            txtdt.Attributes.Add("readonly", "readonly");
                            txtTotalAmount.Attributes.Add("readonly", "readonly");
                            HideUnwantedFields();
                            HFID.Value = Guid.Empty.ToString();
                            HFEditID.Value = Guid.Empty.ToString();
                            GetData();
                            Session["PaymentTermsFQ"] = CommonBLL.FirstRowPaymentTerms();
                            divPaymentTerms.InnerHtml = FillPaymentTerms();

                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit")
                                && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    DivComments.Visible = true;
                                    EditRecord(new Guid(Request.QueryString["ID"]));
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                                btnSave.Text = "Save";
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no");

                        }
                        else
                            if (Session["PaymentTermsFQ"] != null)
                                divPaymentTerms.InnerHtml = FillPaymentTerms();
                        divListBox.InnerHtml = AttachedFiles();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        #endregion

        # region Methods

        /// <summary>
        /// This is used to bind DropDownLists
        /// </summary>
        protected void GetData()
        {
            try
            {
                GetGeneralID();
                BindDropDownList(ddlPrcBsis, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddldept, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != Guid.Empty.ToString() &&
                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != Guid.Empty.ToString())
                {
                    ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();

                    BindListBox(lblEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty,
                      new Guid(ddlcustomer.SelectedValue.ToString()), Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                      DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()),
                      true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    string[] fens = Request.QueryString["FeqID"].ToString().Split(',');
                    foreach (ListItem item in lblEnquiry.Items)
                    {
                        foreach (string s in fens)
                        {
                            if (item.Value == s.ToLower().Trim())
                                item.Selected = true;
                        }
                    }
                    SelectedRecord(Request.QueryString["FeqID"].ToString());

                    string ss = fens.First();

                    TermsMasterBLL TMBL = new TermsMasterBLL();
                    DataSet dsj = TMBL.SelectTermsMaster(CommonBLL.FlagJSelect, new Guid(ss), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    HFID.Value = dsj.Tables[0].Rows[0]["LocalQuotationId"].ToString();
                    HFEditID.Value = Guid.Empty.ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind DRP's
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
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Box
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        private void BindListBox(System.Web.UI.WebControls.ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt;
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to General ID
        /// </summary>
        private void GetGeneralID()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to get details of a foreignQuotation Details
        /// </summary>
        /// <param name="ID"></param>
        private void SelectedRecord(string Enquiry)
        {
            try
            {
                DataTable dt = CommonBLL.EmptyDtLocal();
                if (Enquiry != "")
                {
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    EditDSet = new DataSet();
                    ComparisonStmntBLL CSBLL = new ComparisonStmntBLL();
                    //if (Request.QueryString != null && Request.QueryString.Count > 0 && !Request.QueryString.ToString().Contains("FeqIDs"))
                    EditDSet = CSBLL.SelectBasketItemsCnt(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, 35, Guid.Empty, Enquiry, new Guid(Session["CompanyID"].ToString()), path);
                    //  else
                    //      EditDSet = CSBLL.SelectBasketItemsCnt(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    //    Guid.Empty, Guid.Empty, 35, Guid.Empty, Enquiry, new Guid(Session["CompanyID"].ToString()), path);
                    if (EditDSet.Tables.Count >= 3 && EditDSet.Tables[0].Rows.Count > 0 && EditDSet.Tables[1].Rows.Count > 0)
                    {
                        string FinYear = CommonBLL.GetFinYrShortName();
                        if (FinYear != "")
                        {
                            string CustName = EditDSet.Tables[0].Rows[0]["Custname"].ToString();
                            txtfquoteno.Text = EditDSet.Tables[4].Rows[0]["Name"] + "/" + EditDSet.Tables[0].Rows[0]["EnquireNumber"].ToString() +
                                "/" + CustName +
                                //"/" + EditDSet.Tables[0].Rows[0]["ForeignEnquireId"].ToString() + 
                                "/" + CommonBLL.NumToChar(Convert.ToInt32(EditDSet.Tables[0].Rows[0]["FQCount"].ToString())) +
                                "/" + EditDSet.Tables[3].Rows[0]["TCnt"].ToString() +
                                "/" + CommonBLL.GetFinYrShortName();
                            ddldept.SelectedValue = EditDSet.Tables[0].Rows[0]["DepartmentId"].ToString();
                            txtsubject.Text = EditDSet.Tables[0].Rows[0]["Subject"].ToString();
                            //txtimpinst.Text = EditDS.Tables[0].Rows[0]["Instruction"].ToString();
                            if (EditDSet.Tables[0].Rows[0]["ReceivedDate"].ToString() != "")
                                hfFeReceivedDt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDSet.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                            if (EditDSet.Tables.Count >= 3 && EditDSet.Tables[2].Rows.Count > 0)
                            {
                                txtCvsRt.Text = EditDSet.Tables[2].Rows[0]["DCRate"].ToString();
                            }
                            DataTable dtt = new DataTable();
                            dtt = EditDSet.Tables[1].Copy();
                            dtt = RemoveColumns(dtt);
                            DataSet dss = new DataSet();
                            dss.Tables.Add(dtt);
                            //dss = AddCol(dss);
                            Session["FQItems"] = dss;
                            Session["FQSelectedItems"] = dss;
                            FillGridView(Guid.Empty);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                 "ErrorMessage('No Financial Year Created,Please Create Financial Year in Financial Year Master.');", true);

                    }
                    else
                        NoTable();
                }
                else
                    FillGridView(Guid.Empty);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Thsi is used to Fill GridView
        /// </summary>
        /// <param name="EnqID"></param>
        private void FillGridView(Guid EnqID)
        {
            try
            {
                DataSet ds = new DataSet();
                if (EnqID != Guid.Empty)
                {
                    ds = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, EnqID, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                        DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                    Session["FQItems"] = ds;
                    Session["FQSelectedItems"] = ds;
                    Session["EnqID"] = EnqID;
                }
                else
                {
                    if (lblEnquiry.SelectedIndex == -1)
                        Session["FQItems"] = null;
                    ds = (DataSet)Session["FQItems"];
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        gvFquoteItems.DataSource = ds;
                        gvFquoteItems.DataBind();
                        txtTotalAmount.Text = ds.Tables[0].Compute("Sum(Amount)", "").ToString();
                        txtTotalAmount.Text = Convert.ToDecimal(txtTotalAmount.Text).ToString("0.00");
                        if (txtMargin.Text == "" || txtCvsRt.Text == "")
                            gvFquoteItems.Columns[8].Visible = false;
                        else
                            gvFquoteItems.Columns[8].Visible = true;
                    }
                    else
                        NoTable();
                }
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        private void NoTable()
        {
            try
            {
                txtTotalAmount.Text = "";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("ItemDesc");
                dt.Columns.Add("PartNumber");
                dt.Columns.Add("Specifications");
                dt.Columns.Add("Make");
                dt.Columns.Add("rate");
                dt.Columns.Add("QPrice");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Amount");
                dt.Columns.Add("UnitName");
                dt.Columns.Add("RoundOff");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                ds.Tables[0].Rows[0]["RoundOff"] = "0";
                gvFquoteItems.DataSource = ds;
                gvFquoteItems.DataBind();
                int columncount = gvFquoteItems.Rows[0].Cells.Count;
                gvFquoteItems.Rows[0].Cells.Clear();
                gvFquoteItems.Rows[0].Cells.Add(new TableCell());
                gvFquoteItems.Rows[0].Cells[0].ColumnSpan = columncount;
                gvFquoteItems.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotations", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear control values
        /// </summary>
        private void ClearAll()
        {
            Session["FQuploads"] = null;
            Session["AmountFQ"] = null;
            Session["MessageFQ"] = null;
            Session["PaymentTermsFQ"] = null;
            Session["FQItems"] = null;
            Session["FQSelectedItems"] = null;
            ViewState["EditID"] = null;
            Session["StaticDTItems"] = null;
            ViewState["StaticDTT"] = null;
            //ddlfenq.Enabled = true;
            ddlcustomer.Enabled = true;
            txtdt.Text = CommonBLL.DateDisplay(DateTime.Now);
            //txtdt.Text = "";
            txtCvsRt.Text = "";
            txtDlvry.Text = "";
            txtfquoteno.Text = "";
            txtimpinst.Text = "";
            txtMargin.Text = "";
            txtsubject.Text = "";
            txtTotalAmount.Text = "";
            divPaymentTerms.InnerHtml = "";
            ddlcustomer.SelectedIndex = -1;
            ddldept.SelectedIndex = -1;
            //ddlfenq.SelectedIndex = -1;
            ddlPrcBsis.SelectedIndex = -1;
            gvFquoteItems.DataSource = null;
            gvFquoteItems.DataBind();
        }

        /// <summary>
        /// This is used to save and update records
        /// </summary>
        private void SaveRecord()
        {
            int flag = 0;
            string message = ""; Filename = FileName();

            DataTable FQItems = ((DataSet)Session["FQItems"]).Tables[0].Copy();
            for (int i = 0; i < FQItems.Rows.Count; i++)
            {
                if ((Convert.ToDecimal(FQItems.Rows[i]["Rate"].ToString()) * Convert.ToDecimal(FQItems.Rows[i]["Amount"].ToString())) <= 0)
                {
                    flag = 1;
                    message += (i + 1) + ",";
                }
            }

            DataTable PercentTable = (DataTable)Session["PaymentTermsFQ"];
            if (PercentTable.Columns.Contains("CompanyId"))
                PercentTable.Columns.Remove("CompanyId");
            int res = 1;
            try
            {
                DataTable TCs = CommonBLL.ATConditionsTitle();
                if (Session["TCs"] != null)
                {
                    TCs = (DataTable)Session["TCs"];
                }
                if (TCs.Columns.Contains("Title"))
                    TCs.Columns.Remove("Title");
                if (TCs.Columns.Contains("CompanyID"))
                    TCs.Columns.Remove("CompanyID");

                string Attachments = "";
                ArrayList all = (ArrayList)Session["FQuploads"];
                if (all != null)
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray());

                Guid CustID = new Guid(ddlcustomer.SelectedValue);
                Guid DeptID = new Guid(ddldept.SelectedValue);
                string FeIDs = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DateTime DelvDate = DateTime.Now.AddDays(Convert.ToDouble(txtDlvry.Text) * 7);
                NewFQuotationBLL NFBLL = new NewFQuotationBLL();
                if (FQItems.Rows.Count > 0 && flag == 0)
                {
                    if (FQItems.Columns.Contains("ItemDesc"))
                        FQItems.Columns.Remove("ItemDesc");
                    if (FQItems.Columns.Contains("UnitName"))
                        FQItems.Columns.Remove("UnitName");
                    if (FQItems.Columns.Contains("ItemStatus_FQRe"))
                        FQItems.Columns.Remove("ItemStatus_FQRe");
                    if (FQItems.Columns.Contains("IsChecked"))
                        FQItems.Columns.Remove("IsChecked");
                    FQItems.AcceptChanges();

                    if (btnSave.Text == "Save")
                    {

                        #region GetNewFQ No.

                        string path = Request.Path;
                        path = Path.GetFileName(path);
                        string fqNoNew = "";
                        string Enquiry = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        ComparisonStmntBLL CSBLL = new ComparisonStmntBLL();
                        DataSet ds = CSBLL.SelectBasketItemsCnt(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                            Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 35, Guid.Empty, Enquiry, new Guid(Session["CompanyID"].ToString()), path);
                        if (ds != null && ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0
                            && ds.Tables[0].Columns.Contains("Custname") && ds.Tables[0].Rows[0]["Custname"].ToString() != "")
                        {
                            string CustName = ds.Tables[0].Rows[0]["Custname"].ToString();
                            fqNoNew = EditDSet.Tables[4].Rows[0]["Name"] + "/" + ds.Tables[0].Rows[0]["EnquireNumber"].ToString() + "/" + CustName +
                                "/" + CommonBLL.NumToChar(Convert.ToInt32(ds.Tables[0].Rows[0]["FQCount"].ToString())) +
                                "/" + ds.Tables[3].Rows[0]["TCnt"].ToString() +
                                "/" + CommonBLL.FinacialYearShort;
                        }

                        #endregion

                        res = NFBLL.FQuotationInsert(CommonBLL.FlagNewInsert, Guid.Empty, CustID, DeptID, Guid.Empty, FeIDs,
                            (fqNoNew.Trim() == "" ? txtfquoteno.Text : fqNoNew),
                            txtsubject.Text.Trim(), CommonBLL.DateInsert(txtdt.Text), txtimpinst.Text.Trim(),
                            Convert.ToDecimal(txtTotalAmount.Text), Convert.ToDecimal(txtMargin.Text.Trim()),
                            Convert.ToDecimal(txtCvsRt.Text.Trim()), Convert.ToDecimal(0),
                            new Guid(ddlPrcBsis.SelectedValue), txtPriceBasis.Text, DelvDate,
                            Convert.ToInt32(txtDlvry.Text), Guid.Empty, CommonBLL.StatusTypeFrnQuotID, "",
                            new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, FQItems,
                            PercentTable, TCs, Attachments, new Guid(Session["CompanyID"].ToString()));
                    }
                    else
                    {
                        DataSet ds = NFBLL.Select(CommonBLL.FlagGSelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "",
                            0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, FQItems, PercentTable,
                            TCs, "", new Guid(Session["CompanyID"].ToString()));
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(),
                                "showalert", "alert('Please clear Items from Foreign Quotation Conformation Basket.');", true);
                        }
                        else
                        {
                            res = NFBLL.FQuotationInsert(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), CustID, DeptID, Guid.Empty, FeIDs,
                                txtfquoteno.Text.Trim(), txtsubject.Text.Trim(), CommonBLL.DateInsert(txtdt.Text), txtimpinst.Text.Trim(),
                                Convert.ToDecimal(txtTotalAmount.Text), Convert.ToDecimal(txtMargin.Text.Trim()),
                                Convert.ToDecimal(txtCvsRt.Text.Trim()), Convert.ToDecimal(0), new Guid(ddlPrcBsis.SelectedValue),
                                txtPriceBasis.Text, DelvDate, Convert.ToInt32(txtDlvry.Text), Guid.Empty,
                                CommonBLL.StatusTypeFrnQuotID, txtComments.Text.Trim(),
                                new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now,
                                true, FQItems, PercentTable, TCs, Attachments, new Guid(Session["CompanyID"].ToString()));
                        }
                    }
                    if (res == 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtfquoteno.Text, "Foreign Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "NewFquotation",
                            "Data inserted successfully in NewFquotation.");
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FQStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtfquoteno.Text, "Foreign Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "NewFquotation", "Getting Error.");
                        divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }
                    else if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtfquoteno.Text, "Foreign Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "NewFquotation",
                            "Data Updated successfully in NewFquotation.");
                        ViewState["EditID"] = null;
                        btnSave.Text = "Save";
                        ClearAll(); Session.Remove("TCs");
                        Response.Redirect("FQStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtfquoteno.Text, "Foreign Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "NewFquotation", "Getting Error while Updating.");
                    }
                }
                else
                {
                    if (flag == 1)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Rate or amount cannot be zero of S.No. " + message.Trim(',') + ".');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('There are No Items to Save/Update.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                //ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "New Foreign Quotation", "Data Inserted Successfully.");
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Remove Columns form Data Table
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable RemoveColumns(DataTable dt)
        {
            if (dt.Columns.Contains("PaymentSerialNo"))
                dt.Columns["PaymentSerialNo"].ColumnName = "SNo";
            if (dt.Columns.Contains("Percentage"))
                dt.Columns["Percentage"].ColumnName = "PaymentPercentage";
            if (dt.Columns.Contains("Against"))
                dt.Columns["Against"].ColumnName = "Description";
            if (dt.Columns.Contains("PaymentTermsId"))
                dt.Columns.Remove("PaymentTermsId");
            if (dt.Columns.Contains("FQuotationId"))
                dt.Columns.Remove("FQuotationId");
            if (dt.Columns.Contains("FPurchaseOrderId"))
                dt.Columns.Remove("FPurchaseOrderId");
            if (dt.Columns.Contains("LQuotationId"))
                dt.Columns.Remove("LQuotationId");
            if (dt.Columns.Contains("LPurchaseOrderId"))
                dt.Columns.Remove("LPurchaseOrderId");
            if (dt.Columns.Contains("CreatedBy"))
                dt.Columns.Remove("CreatedBy");
            if (dt.Columns.Contains("CreatedDate"))
                dt.Columns.Remove("CreatedDate");
            if (dt.Columns.Contains("ModifiedBy"))
                dt.Columns.Remove("ModifiedBy");
            if (dt.Columns.Contains("ModifiedDate"))
                dt.Columns.Remove("ModifiedDate");
            if (dt.Columns.Contains("IsActive"))
                dt.Columns.Remove("IsActive");

            if (dt.Columns.Contains("ItemDetailsId"))
                dt.Columns.Remove("ItemDetailsId");
            //if (dt.Columns.Contains("ForeignQuotationId"))
            //    dt.Columns.Remove("ForeignQuotationId");
            if (dt.Columns.Contains("LocalQuotationId"))
                dt.Columns.Remove("LocalQuotationId");
            if (dt.Columns.Contains("ForeignEnquireId"))
                dt.Columns.Remove("ForeignEnquireId");
            if (dt.Columns.Contains("LocalEnquireId"))
                dt.Columns.Remove("LocalEnquireId");
            if (dt.Columns.Contains("ForeignPOId"))
                dt.Columns.Remove("ForeignPOId");
            if (dt.Columns.Contains("LocalPOId"))
                dt.Columns.Remove("LocalPOId");
            //if (dt.Columns.Contains("ItemDesc"))
            //    dt.Columns.Remove("ItemDesc");
            //if (dt.Columns.Contains("UnitName"))
            //    dt.Columns.Remove("UnitName");
            dt.AcceptChanges();
            return dt;
        }

        /// <summary>
        /// This is used to edit selected Record
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                HFID.Value = Guid.Empty.ToString();
                HFEditID.Value = ID.ToString();
                EditDS = new DataSet();
                NewFQuotationBLL NFBLL = new NewFQuotationBLL();
                EditDS = NFBLL.Select(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now,
                    0, Guid.Empty, 0, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now,
                    false, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0 &&
                    EditDS.Tables[2].Rows.Count > 0)
                {

                    txtDlvry.Text = EditDS.Tables[1].Rows[0]["DeliveryPeriod"].ToString();
                    txtMargin.Text = EditDS.Tables[1].Rows[0]["FobMargin"].ToString();
                    txtCvsRt.Text = EditDS.Tables[1].Rows[0]["ConversionRate"].ToString();
                    txtPriceBasis.Text = EditDS.Tables[1].Rows[0]["PriceBasisText"].ToString();
                    hfFeReceivedDt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[1].Rows[0]["FeReceivedDate"].ToString()));
                    GetData();
                    //SelectedRecord(EditDS.Tables[1].Rows[0]["FrnEnqIDs"].ToString());
                    //FrnEnqIDs
                    DataTable dtt = new DataTable();
                    dtt = EditDS.Tables[0].Copy();
                    DataSet dsss = new DataSet();
                    dsss.Tables.Add(dtt);
                    //dsss = AddCol(dsss);
                    Session["FQItems"] = dsss;
                    Session["FQSelectedItems"] = dsss;
                    //dtItems = dsss.Tables[0].Copy();
                    ViewState["StaticDTT"] = dsss.Tables[0].Copy();
                    dtt = new DataTable();
                    dtt = EditDS.Tables[2].Copy();
                    dtt = RemoveColumns(dtt);

                    Session["AmountFQ"] = dtt.Compute("Sum(PaymentPercentage)", "").ToString();
                    txtTotalAmount.Text = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Compute("Sum(Amount)", "").ToString()).ToString("0.00");
                    Session["PaymentTermsFQ"] = dtt;
                    ddlcustomer.SelectedValue = EditDS.Tables[1].Rows[0]["CusmorId"].ToString();

                    BindListBox(lblEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty,
                        new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                        DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()),
                        true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    string[] FenqIDs = EditDS.Tables[1].Rows[0]["FrnEnqIDs"].ToString().Split(',');
                    foreach (ListItem item in lblEnquiry.Items)
                    {
                        foreach (string s in FenqIDs)
                        {
                            if (item.Value == s.ToLower().Trim())
                                item.Selected = true;
                        }
                    }
                    FillGridView(Guid.Empty);
                    if (dtt.Rows.Count > 0)
                    {
                        Session["AmountFQ"] = dtt.Compute("Sum(PaymentPercentage)", "");
                        divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }
                    Session["TCs"] = (CBL.SelectATConditions(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, ID, Guid.Empty, Guid.Empty, 0, Guid.Empty, "",
                        new Guid(Session["UserID"].ToString()))).Tables[0];

                    txtfquoteno.Text = EditDS.Tables[1].Rows[0]["Quotationnumber"].ToString();
                    txtsubject.Text = EditDS.Tables[1].Rows[0]["Subject"].ToString();
                    ddldept.SelectedValue = EditDS.Tables[1].Rows[0]["DepartmentId"].ToString();
                    txtdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[1].Rows[0]["QuotationDate"].ToString()));
                    ddlPrcBsis.SelectedValue = EditDS.Tables[1].Rows[0]["PriceBasis"].ToString();
                    txtimpinst.Text = EditDS.Tables[1].Rows[0]["Instruction"].ToString();

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
                        Session["FQuploads"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                        divListBox.InnerHtml = "";


                    btnSave.Text = "Update";
                    ViewState["EditID"] = ID;
                    lblEnquiry.Enabled = false;
                    ddlcustomer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
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
                        string strPath = MapPath("~/uploads/");// +Path.GetFileName(AsyncFileUpload1.FileName);
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["FQuploads"] != null)
                        {
                            alist = (ArrayList)Session["FQuploads"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["FQuploads"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["FQuploads"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
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
                if (Session["FQuploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["FQuploads"];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return ex.Message;
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
                if (Session["AmountFQ"] != null)
                    TotalAmt = Convert.ToInt32(Session["AmountFQ"]);
                if (Session["MessageFQ"] != null)
                    Message = Session["MessageFQ"].ToString();
                DataTable dt = (DataTable)Session["PaymentTermsFQ"];
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblPaymentTerms' " +
                    " align='center'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Payment(%)</th><th>Description</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)//onBlur 
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");
                        if (dt.Rows[i]["PaymentPercentage"].ToString() != "")
                            sb.Append("<td><input type='text' name='txtPercAmt' class='bcAsptextbox' onfocus='this.select()' " +
                                " onMouseUp='return false' value='" + Convert.ToInt64(Convert.ToDouble(dt.Rows[i]["PaymentPercentage"].ToString())).ToString("F0") + "' "
                                + " onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);' "
                                + " id='txtPercAmt" + SNo + "' onchange='getPaymentValues(" + SNo + ")' maxlength='3' style='text-align: right; width: 50px;'/></td>");
                        else
                            sb.Append("<td><input type='text' name='txtPercAmt' class='bcAsptextbox' value='" + dt.Rows[i]["PaymentPercentage"].ToString() + "' "
                                + " id='txtPercAmt" + SNo + "' onchange='getPaymentValues(" + SNo + ")' maxlength='3' style='text-align: right; width: 50px;' "
                                + " onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");
                        sb.Append("<td><input type='text' name='txtDesc' class='bcAsptextbox' value='" + dt.Rows[i]["Description"].ToString()
                            + "'  id='txtDesc" + SNo + "' onchange='getPaymentValues(" + SNo + ")'/></td>");
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

                return sb.ToString();// FillItemGrid(0, dt, Codes, ds, 0, CatCodes, 0, UnitCodes);
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
        /// This is used to add column Change
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private DataSet AddCol(DataSet ds)
        {
            if (!ds.Tables[0].Columns.Contains("Change"))
            {
                ds.Tables[0].Columns.Add("Change");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    ds.Tables[0].Rows[i]["Change"] = "0";
                ds.Tables[0].AcceptChanges();
            }
            return ds;
        }

        private void HideUnwantedFields()
        {
            try
            {
                FieldAccessBLL FAB = new FieldAccessBLL();
                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), System.IO.Path.GetFileName(Request.Path));
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
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

        # endregion

        # region Selected IndexChange

        /// <summary>
        /// This is used to Change Customer Index Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlcustomer.SelectedValue != "0")
                {
                    //BindDropDownList(NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, 0, int.Parse(ddlcustomer.SelectedValue), 0, "",
                    //    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", 0, 0, true, CommonBLL.EmptyDt()));
                    BindListBox(lblEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                        Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    NoTable();
                    txtMargin.Text = "";
                    txtCvsRt.Text = "";
                }
                else
                {
                    ClearAll();
                    NoTable();
                    txtMargin.Text = "";
                    txtCvsRt.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Change FrnEnquiry Index Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void lblEnquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string Enquiry = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                SelectedRecord(Enquiry);
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DDL Rate Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRateChange_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ////dtt = new DataTable();
                //if (dtt == null || dtt.Rows.Count == 0)
                //    dtt = ((DataSet)Session["FQSelectedItems"]).Tables[0].Copy();

                if (ViewState["StaticDTT"] == null || ((DataTable)ViewState["StaticDTT"]).Rows.Count == 0)
                    ViewState["StaticDTT"] = ((DataSet)Session["FQSelectedItems"]).Tables[0].Copy();

                DataSet ds = (DataSet)Session["FQItems"];
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.Parent.Parent;
                int idx = row.RowIndex;
                //string selVal = ds.Tables[0].Rows[idx]["RoundOff"].ToString();
                if (ddl.SelectedValue != "0")
                {
                    ds.Tables[0].Rows[idx]["RoundOff"] = ddl.SelectedValue;
                    //((HiddenField)row.FindControl("hfSV")).Value = ddl.SelectedValue;
                    decimal Rate = Convert.ToDecimal(string.Format("{0:0.00}", ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"].ToString()));
                    decimal Amount = 0;
                    decimal RateTemp = 0;
                    if (ddl.SelectedValue == "1") // 1 Decimals
                    {
                        //Math.Round(3.44, 1); //Returns 3.4.
                        //Math.Round(3.45, 1); //Returns 3.4.
                        //Math.Round(3.46, 1); //Returns 3.5.
                        //decimal.Round(Rate, 1, MidpointRounding.AwayFromZero);
                        decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());
                        ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", decimal.Round(Rate, 1, MidpointRounding.AwayFromZero));
                        ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round(Rate, 1, MidpointRounding.AwayFromZero) * qty));
                        //ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round((Rate * qty), 1, MidpointRounding.AwayFromZero)));
                        Amount = decimal.Round(Rate, 1, MidpointRounding.AwayFromZero) * qty;
                    }
                    else if (ddl.SelectedValue == "2") // 2 Decimals
                    {
                        decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());
                        ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", decimal.Round(Rate, 2, MidpointRounding.AwayFromZero));
                        ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round(Rate, 2, MidpointRounding.AwayFromZero) * qty));
                        //ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round((Rate*qty), 2, MidpointRounding.AwayFromZero)));
                        Amount = decimal.Round(Rate, 2, MidpointRounding.AwayFromZero) * qty;
                    }
                    else if (ddl.SelectedValue == "3") // 0 Decimal
                    {
                        decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());

                        ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", Convert.ToInt32(Rate));
                        ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", Convert.ToInt32(Rate) * qty);
                        Amount = Convert.ToInt32(Rate) * qty;

                        //ds.Tables[0].Rows[idx]["Amount"] = Convert.ToInt32(Rate * qty).ToString("N");
                    }

                    if (Rate <= 0 || Amount <= 0)
                    {
                        ddl.SelectedIndex = -1;
                        ds.Tables[0].Rows[idx]["Rate"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"];
                        ds.Tables[0].Rows[idx]["Amount"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Amount"];
                        ds.Tables[0].Rows[idx]["RoundOff"] = "0";
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                                            "ErrorMessage('Rate and Amount cannot be Zero.');", true);
                    }
                }
                else
                {
                    ds.Tables[0].Rows[idx]["Rate"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"];
                    ds.Tables[0].Rows[idx]["Amount"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Amount"];
                    ds.Tables[0].Rows[idx]["RoundOff"] = ddl.SelectedValue;
                }
                //ds.Tables[0].AcceptChanges();
                txtTotalAmount.Text = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Amount)", "").ToString()).ToString("0.00");
                //Session["FQItems"] = ds;
                gvFquoteItems.DataSource = ds.Tables[0];
                gvFquoteItems.DataBind();
                //ViewState["StaticDTT"] = null;
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
                //ViewState["StaticDTT"] = null;
            }
        }

        protected void ddlRateChangeHeader_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ////dtt = new DataTable();
                //if (dtt == null || dtt.Rows.Count == 0)
                //    dtt = ((DataSet)Session["FQSelectedItems"]).Tables[0].Copy();


                if (ViewState["StaticDTT"] == null || ((DataTable)ViewState["StaticDTT"]).Rows.Count == 0)
                    ViewState["StaticDTT"] = ((DataSet)Session["FQSelectedItems"]).Tables[0].Copy();

                DataSet ds = (DataSet)Session["FQItems"];
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.Parent.Parent;
                //int idx = row.RowIndex;
                //string selVal = ds.Tables[0].Rows[idx]["RoundOff"].ToString();
                for (int idx = 0; idx < ds.Tables[0].Rows.Count; idx++)
                {
                    if (ddl.SelectedValue != "0")
                    {
                        ds.Tables[0].Rows[idx]["RoundOff"] = ddl.SelectedValue;
                        //((HiddenField)row.FindControl("hfSV")).Value = ddl.SelectedValue;
                        decimal Rate = Convert.ToDecimal(string.Format("{0:0.00}", ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"].ToString()));
                        decimal Amount = 0;
                        decimal RateTemp = 0;
                        if (ddl.SelectedValue == "1") // 1 Decimals
                        {
                            //Math.Round(3.44, 1); //Returns 3.4.
                            //Math.Round(3.45, 1); //Returns 3.4.
                            //Math.Round(3.46, 1); //Returns 3.5.
                            //decimal.Round(Rate, 1, MidpointRounding.AwayFromZero);
                            decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());
                            ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", decimal.Round(Rate, 1, MidpointRounding.AwayFromZero));
                            ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round(Rate, 1, MidpointRounding.AwayFromZero) * qty));
                            //ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round((Rate * qty), 1, MidpointRounding.AwayFromZero)));
                            Amount = decimal.Round(Rate, 1, MidpointRounding.AwayFromZero) * qty;
                        }
                        else if (ddl.SelectedValue == "2") // 2 Decimals
                        {
                            decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());
                            ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", decimal.Round(Rate, 2, MidpointRounding.AwayFromZero));
                            ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round(Rate, 2, MidpointRounding.AwayFromZero) * qty));
                            //ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", (decimal.Round((Rate*qty), 2, MidpointRounding.AwayFromZero)));
                            Amount = decimal.Round(Rate, 2, MidpointRounding.AwayFromZero) * qty;
                        }
                        else if (ddl.SelectedValue == "3") // 0 Decimal
                        {
                            decimal qty = Convert.ToDecimal(((DataTable)ViewState["StaticDTT"]).Rows[idx]["Quantity"].ToString());

                            ds.Tables[0].Rows[idx]["Rate"] = string.Format("{0:0.00}", Convert.ToInt32(Rate));
                            ds.Tables[0].Rows[idx]["Amount"] = string.Format("{0:0.00}", Convert.ToInt32(Rate) * qty);
                            Amount = Convert.ToInt32(Rate) * qty;

                            //ds.Tables[0].Rows[idx]["Amount"] = Convert.ToInt32(Rate * qty).ToString("N");
                        }

                        if (Rate <= 0 || Amount <= 0)
                        {
                            //ddl.SelectedIndex = -1;
                            ds.Tables[0].Rows[idx]["Rate"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"];
                            ds.Tables[0].Rows[idx]["Amount"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Amount"];
                            ds.Tables[0].Rows[idx]["RoundOff"] = "0";
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                                                "ErrorMessage('Rate and Amount cannot be Zero.');", true);
                        }
                    }
                    else
                    {
                        ds.Tables[0].Rows[idx]["Rate"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Rate"];
                        ds.Tables[0].Rows[idx]["Amount"] = ((DataTable)ViewState["StaticDTT"]).Rows[idx]["Amount"];
                        ds.Tables[0].Rows[idx]["RoundOff"] = ddl.SelectedValue;
                    }
                }
                ds.Tables[0].AcceptChanges();
                txtTotalAmount.Text = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Amount)", "").ToString()).ToString("0.00");
                //Session["FQItems"] = ds;
                gvFquoteItems.DataSource = ds;
                gvFquoteItems.DataBind();
                //ViewState["StaticDTT"] = null;
                ((DropDownList)(gvFquoteItems.HeaderRow.FindControl("ddlRateChangeHeader"))).SelectedValue = ddl.SelectedValue;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
                //ViewState["StaticDTT"] = null;
            }
        }

        # endregion

        # region ButtonClicks

        /// <summary>
        /// This is used to Save And Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblEnquiry.SelectedValue != "0" && txtfquoteno.Text != "" && ddlPrcBsis.SelectedValue != "0" &&
                    txtMargin.Text != "" && txtMargin.Text != "0" && txtCvsRt.Text != "" && txtCvsRt.Text != "0")
                    SaveRecord();
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Fill All the Details Properly.');", true);
                    gvFquoteItems.DataBind();
                    //ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear all controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Quotations/NewFQuotation.aspx", false);
                // ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnNext = (Button)sender;
                int i = gvFquoteItems.PageIndex + 1;
                if (i < gvFquoteItems.PageCount)
                    gvFquoteItems.PageIndex = i;
                FillGridView(Guid.Empty);
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
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
                int i = gvFquoteItems.PageCount;
                if (gvFquoteItems.PageIndex > 0)
                    gvFquoteItems.PageIndex = gvFquoteItems.PageIndex - 1;
                FillGridView(Guid.Empty);
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
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
                gvFquoteItems.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                FillGridView(Guid.Empty);
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = ddlPageSize.SelectedValue;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMethods

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
                dt = (DataTable)Session["PaymentTermsFQ"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[rowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["AmountFQ"] = dt.Compute("Sum(PaymentPercentage)", "");
                Session["PaymentTermsFQ"] = dt;
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
                dt = (DataTable)Session["PaymentTermsFQ"];
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
                    Session["MessageFQ"] = "";
                    Session["AmountFQ"] = PmntPercent.ToString();
                }
                else
                    Session["MessageFQ"] = "Percentage Cannot Exceed 100";
                Session["PaymentTermsFQ"] = dt;
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
        /// Add Attachemts to List Box
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        /// <summary>
        /// Delete Attachemts from List Box
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["FQuploads"];
                if (all.Count > 0)
                    all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckEnquiryNo(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckLQEnquiryNo('Y', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        # endregion

        #region Text Chaned Events
        /// <summary>
        /// Text Box Text Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtMargin_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMargin.Text != "" && Convert.ToDecimal(txtMargin.Text) <= 100)
                {
                    ConvertRsToDollar();
                    txtCvsRt.Focus();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                 "ErrorMessage('FOB + Margin value cannot exceed 100%');", true);
                    txtMargin.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Text Box Text Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCvsRt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtCvsRt.Text != null && txtCvsRt.Text != "0")
                {
                    if (Convert.ToDecimal(txtCvsRt.Text) <= 100)
                        ConvertRsToDollar();
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                 "ErrorMessage('Conversion rate cannot exceed 100.');", true);
                        txtCvsRt.Text = "";
                    }
                }
                else
                {
                    string Enquiry = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    SelectedRecord(Enquiry);
                }
                if (txtCvsRt.Text == "")
                {
                    string Enquiry = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    SelectedRecord(Enquiry);
                }
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This method for Conversition Rs to Dollar
        /// </summary>
        private void ConvertRsToDollar()
        {
            try
            {
                if ((txtMargin.Text != "") && (txtCvsRt.Text != ""))
                {
                    DataSet ds = new DataSet();
                    DataSet ds1 = new DataSet();
                    //ds1.Tables.Add(dtItems);
                    if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                    {
                        NewFQuotationBLL NFBLL = new NewFQuotationBLL();
                        EditDS = NFBLL.Select(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"]), Guid.Empty, Guid.Empty, Guid.Empty,
                            "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now,
                            0, Guid.Empty, 0, "", new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now,
                            false, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));
                        if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0 &&
                            EditDS.Tables[2].Rows.Count > 0)
                        {

                            DataTable dtt = new DataTable();
                            dtt = EditDS.Tables[0].Copy();
                            DataSet dsss = new DataSet();
                            dsss.Tables.Add(dtt);
                            Session["FQItems"] = dsss;
                            Session["FQSelectedItems"] = dsss;

                        }
                    }
                    else
                    {
                        EditDS = new DataSet();
                        ComparisonStmntBLL CSBLL = new ComparisonStmntBLL();
                        string Enquiry = String.Join(",", lblEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                        //  if (Request.QueryString != null && Request.QueryString.Count > 0 && !Request.QueryString.ToString().Contains("FeqIDs"))
                        EditDS = CSBLL.SelectBasketItems(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(lblEnquiry.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 35, Guid.Empty, Enquiry, new Guid(Session["CompanyID"].ToString()));
                        // else
                        //     EditDS = CSBLL.SelectBasketItems(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, new Guid(lblEnquiry.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 35, Guid.Empty, Enquiry, new Guid(Session["CompanyID"].ToString()));
                        if (EditDS.Tables.Count >= 2 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0)
                        {
                            DataTable dtt = new DataTable();
                            dtt = EditDS.Tables[1].Copy();
                            dtt = RemoveColumns(dtt);
                            DataSet dss = new DataSet();
                            dss.Tables.Add(dtt);
                            Session["FQItems"] = dss;
                            Session["FQSelectedItems"] = dss;
                        }
                    }

                    DataSet dt = (DataSet)HttpContext.Current.Session["FQItems"];

                    if (dt != null)
                    {
                        for (int rowNo = 0; rowNo < dt.Tables[0].Rows.Count; rowNo++)
                        {
                            double arate = Convert.ToDouble(dt.Tables[0].Rows[rowNo]["QPrice"]);
                            double aAmount = Convert.ToDouble(dt.Tables[0].Rows[rowNo]["Amount"]);
                            double aQty = Convert.ToDouble(dt.Tables[0].Rows[rowNo]["Quantity"]);
                            double Margin = 0.0;
                            if (txtMargin.Text != "" && txtMargin.Text != null) Margin = Convert.ToDouble(txtMargin.Text);
                            double Convamt = 0.0;
                            if (txtCvsRt.Text != "" && txtCvsRt.Text != null) Convamt = Convert.ToDouble(txtCvsRt.Text);
                            double aconRate = 0.0;
                            double aconTotAmt = 0.0;

                            aconRate = Math.Round(((arate + ((arate * Margin) / 100)) / Convamt), 2);
                            aconTotAmt = Math.Round((aconRate * aQty), 2);

                            dt.Tables[0].Rows[rowNo]["Rate"] = string.Format("{0:0.00}", aconRate);
                            dt.Tables[0].Rows[rowNo]["Amount"] = string.Format("{0:0.00}", aconTotAmt);
                        }
                    }

                    HttpContext.Current.Session["FQItems"] = dt;

                    ds = (DataSet)Session["FQItems"];
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        //ds = AddCol(ds);
                        ViewState["StaticDTT"] = ds.Tables[0];
                        gvFquoteItems.DataSource = ds;
                        gvFquoteItems.DataBind();
                        txtTotalAmount.Text = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Amount)", "").ToString()).ToString("0.00");
                        gvFquoteItems.Columns[8].Visible = true;
                    }
                    //else
                    //    NoTable();
                    txtCvsRt.Focus();
                }
                if (((DataTable)Session["PaymentTermsFQ"]).Rows.Count > 0)
                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                else
                {
                    Session["PaymentTermsFQ"] = CommonBLL.FirstRowPaymentTerms();
                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                }
                ((DropDownList)gvFquoteItems.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid Veiw Events
        /// <summary>
        /// Row Data Bound Event for GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvFquoteItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].CssClass = "rounded-First";
                    e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-Last";
                    if (txtMargin.Text == "" || txtCvsRt.Text == "")
                    {
                        if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                        {
                            string PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                                .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                            ((Label)(e.Row.FindControl("lblhdrRt"))).Text = "Rate(" + PriceSymbol + ")";
                            ((Label)(e.Row.FindControl("lblhdrAmt"))).Text = "Amount(" + PriceSymbol + ")";
                            lblTtlAmt.Text = "Total Amount(" + PriceSymbol + ") :";
                        }
                        else
                        {
                            ((Label)(e.Row.FindControl("lblhdrRt"))).Text = "Rate(" + Session["CurrencySymbol"] + ")";
                            ((Label)(e.Row.FindControl("lblhdrAmt"))).Text = "Amount(" + Session["CurrencySymbol"] + ")";
                            lblTtlAmt.Text = "Total Amount(" + Session["CurrencySymbol"] + ") :";
                        }
                    }
                    else
                    {
                        ((Label)(e.Row.FindControl("lblhdrRt"))).Text = "Rate($)";
                        ((Label)(e.Row.FindControl("lblhdrAmt"))).Text = "Amount($)";
                        lblTtlAmt.Text = "Total Amount($) :";
                    }
                }
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    RunningTotal += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Amount"));
                    decimal amt = Convert.ToDecimal(((Label)(e.Row.FindControl("lblmailID"))).Text);
                    ((Label)(e.Row.FindControl("lblmailID"))).Text = amt.ToString("N");
                }
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    ((Label)e.Row.FindControl("lbltmnt")).Text = RunningTotal.ToString("N");
                    e.Row.Cells[0].CssClass = "rounded-foot-left";
                    e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-foot-right";

                    if (gvFquoteItems.PageIndex == 0)
                        ((Button)e.Row.FindControl("btnPrevious")).Enabled = false;
                    else
                        ((Button)e.Row.FindControl("btnPrevious")).Enabled = true;

                    if (gvFquoteItems.PageCount == (gvFquoteItems.PageIndex) + 1)
                        ((Button)e.Row.FindControl("btnNext")).Enabled = false;
                    else
                        ((Button)e.Row.FindControl("btnNext")).Enabled = true;
                    ((DropDownList)e.Row.FindControl("ddlPageSize")).SelectedValue = gvFquoteItems.PageSize.ToString();
                    ((Label)e.Row.FindControl("lblFooterPaging")).Text = "Total Pages: " + gvFquoteItems.PageCount + ", Current Page:" + (gvFquoteItems.PageIndex + 1) + ", Rows to Display:";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void gvFquoteItems_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvFquoteItems.UseAccessibleHeader = false;
                gvFquoteItems.HeaderRow.TableSection = TableRowSection.TableHeader;
                gvFquoteItems.FooterRow.TableSection = TableRowSection.TableFooter;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
        #endregion

    }
}
