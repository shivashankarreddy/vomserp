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
using System.Collections.Generic;
using Ajax;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Quotations
{
    public partial class NewLQuotation : System.Web.UI.Page
    {
        //Comments added by Satya :: GST Changes
        //Note: for GST changes used existing "Exduty" & "ExdutyPercentage" database fields to "CGST" & "CGSTPercentage"

        # region variables
        int res;
        //public static ArrayList Files = new ArrayList();
        CheckBLL CBL = new CheckBLL();
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CustomerBLL cusmr = new CustomerBLL();
        LEnquiryBLL NLEBL = new LEnquiryBLL();
        NewEnquiryBLL NEBL = new NewEnquiryBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        EMailsDetailsBLL EMDBL = new EMailsDetailsBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string PriceSymbol = "";
        //static string GeneralCtgryID;
        //static DataSet EditDS;
        //static string btnSaveID = "";
        //static int GenSupID;
        //static DataSet dss = new DataSet();
        //static DataSet dsEnm = new DataSet();
        int FlagDiscount = 0;
        int FlagExDuty = 0;
        string read;
        #endregion

        #region Default Page Load Evnet

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if ((Session["UserID"] == null || Session["UserID"].ToString() == "") && Request.QueryString["SupID"] == null)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if ((Session["UserID"] != null && CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path)) || Request.QueryString["SupID"] != null)
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewLQuotation));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            ClearAll();
                            txtdt.Attributes.Add("readonly", "readonly");
                            txtqdt.Attributes.Add("readonly", "readonly");
                            HttpContext.Current.Session["IsItems"] = false;
                            Session["PaymentTerms"] = CommonBLL.FirstRowPaymentTerms();
                            GetData();
                            HFID.Value = Guid.Empty.ToString();
                            HideUnwantedFields();
                            Session.Remove("TCs");
                            if (Request.QueryString["SupID"] != null && Session["UsrPermissions"] == null && ((string[])Session["UsrPermissions"] == null))
                            {
                                Response.Redirect("../Login.aspx?logout=no", false);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                            }
                            else
                            {
                                if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                                    (Request.QueryString["ID"] != null || Request.QueryString["SupID"] != null))
                                {
                                    if (Request.QueryString["ID"] != null && Request.QueryString["ID"].ToString() != "")
                                    {
                                        DivComments.Visible = true;
                                        EditLQuotation(new Guid(Request.QueryString["ID"].ToString()));
                                    }
                                    else
                                        Session.Remove("TCs");
                                }
                                else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New") &&
                                    Request.QueryString["ID"] == null)
                                {
                                    divPaymentTerms.InnerHtml = FillPaymentTerms();
                                    btnSave.Text = "Save";
                                    btnSave.Enabled = true;
                                }
                                else
                                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                            }
                        }
                        else
                            if (Session["PaymentTerms"] != null)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                if (Request.QueryString["SupID"] != null && Request.QueryString["SupID"].ToString() != "")
                {
                    GetValuesFromDecryption(Request.QueryString["SupID"].ToString());
                    BindDropDownList(ddlPriceBasis, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                    BindDropDownList(ddldept, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                }
                else
                {
                    GetGeneralID();
                    BindDropDownList(ddlPriceBasis, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Incotrms));
                    BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddldept, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));
                }

                if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "" &&
                    Request.QueryString["LeqID"] != null && Request.QueryString["LeqID"].ToString() != "")
                {
                    ddlcustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "",
                        DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()),
                        CommonBLL.EmptyDt()));
                    ddlfenq.SelectedValue = Request.QueryString["FeqID"].ToString();
                    BindDropDownList(ddllenq, NLEBL.SelctLocalEnquiries(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                        new Guid(ddlfenq.SelectedValue), "", "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 60, "", "",
                        "", Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal()));
                    ddllenq.SelectedValue = Request.QueryString["LeqID"].ToString();
                    divLQItems.InnerHtml = EditRecord(new Guid(ddllenq.SelectedValue));
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        protected void GetValuesFromDecryption(string EncryptionString)
        {
            try
            {
                string DecryptString = StringEncrpt_Decrypt.Decrypt(Request.QueryString["SupID"].Replace(' ', '+'), true);
                string[] qs = DecryptString.Split('&');

                //SupID = qs[0];
                string CstmrID = qs[1].Split('=')[1].Trim();
                string FenqID = qs[2].Split('=')[1].Trim();
                string LenqID = qs[3].Split('=')[1].Trim();
                string LMID = qs[4].Split('=')[1].Trim();
                string LPWD = qs[5].Split('=')[1].Trim();
                string CmpnyID = qs[6].Split('=')[1].Trim();
                if (LMID != "" && LPWD != "")
                {
                    LoginBLL bll = new LoginBLL();
                    if (bll.LogIn(CommonBLL.FlagSelectAll, LMID, LPWD, false, new Guid(CmpnyID)))
                    {
                        ArrayList al = bll.UserDetails();
                        Session["UserMail"] = LMID;
                        Session["UserID"] = al[1].ToString();
                        Session["UserName"] = al[0].ToString();
                        //CommonBLL.AliasName = al[4].ToString();
                        Session["CompanyID"] = al[12].ToString();
                        Session["UserDtls"] = al;
                        CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path);
                        if (Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"] != null))
                        {
                            DataSet IsLQuote = NLQBL.LclQuoteSelect(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(LenqID), Guid.Empty,
                                DateTime.Now, DateTime.Now, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                            BindDropDownList(ddlcustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                            if (IsLQuote != null && IsLQuote.Tables.Count > 0 && IsLQuote.Tables[0].Rows.Count > 0)
                            {
                                DivComments.Visible = true;
                                EditLQuotation(new Guid(IsLQuote.Tables[0].Rows[0]["LocalQuotationId"].ToString()));
                            }
                            else
                            {
                                ddlcustomer.SelectedValue = CstmrID;
                                BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty,
                                    new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                                    DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                                ddlfenq.SelectedValue = FenqID;
                                BindDropDownList(ddllenq, NLEBL.SelctLocalEnquiries(CommonBLL.FlagRegularDRP, Guid.Empty,
                                    new Guid(ddlcustomer.SelectedValue), new Guid(ddlfenq.SelectedValue), "",
                                    "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 20, "", "", "", Guid.Empty,
                                    DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal()));
                                ddllenq.SelectedValue = LenqID;
                                if (ddllenq.SelectedValue != Guid.Empty.ToString())
                                {
                                    divLQItems.InnerHtml = EditRecord(new Guid(ddllenq.SelectedValue));
                                }
                                else
                                {
                                    ddlcustomer.SelectedIndex = ddlfenq.SelectedIndex = 0;
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "ErrorMessage('Local Quotation Already Prepared for this Enquiry.');", true);
                                }
                            }

                            ddlcustomer.Enabled = ddlfenq.Enabled = ddllenq.Enabled = false;
                        }
                        else
                        {
                            //Session.Abandon();
                            Response.Redirect("../Login.aspx?logout=no", false);
                        }
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
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is to get genereal ID
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
                    string GeneralCtgryID = ds.Tables[0].Rows[0][0].ToString();
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
        /// Clear all input fields
        /// </summary>        
        protected void ClearAll()
        {
            try
            {
                ddlcustomer.Enabled = true;
                ddlfenq.Enabled = true;
                ddllenq.Enabled = true;
                btnSave.Text = "Save";
                //btnSaveID = "Save";
                ViewState["EditID"] = null;
                ddlcustomer.SelectedIndex = ddlfenq.SelectedIndex = ddllenq.SelectedIndex = -1;
                ddldept.SelectedIndex = ddlPriceBasis.SelectedIndex = -1;
                Session.Remove("MessageLQ");
                Session.Remove("PaymentTerms");
                Session.Remove("AmountLQ");
                Session.Remove("FloatEnquiry");
                Session.Remove("NLQUplds");
                Session["PaymentTerms"] = CommonBLL.FirstRowPaymentTerms();
                divPaymentTerms.InnerHtml = FillPaymentTerms();
                lblsupplier.Text = "SUPPLIER ORGANIZATION NAME";
                txtdt.Text = txtlquotno.Text = txtsubject.Text = txtimpinst.Text = txtDlvry.Text = txtPriceBasis.Text = "";
                txtExdt.Text = txtPkng.Text = txtDsnt.Text = txtAdtnChrgs.Text = "0";  //txtSltx.Text =
                divLQItems.InnerHtml = "";
                Session.Remove("ItemCode");
                Session.Remove("AllLQSubItems");
                txtdt.Text = txtqdt.Text = "";          //DateTime.Now.Date.ToShortDateString();

                //txtduedt.Text = DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ? 
                //DateTime.Now.AddDays(5).Date.ToShortDateString() : DateTime.Now.AddDays(6).Date.ToShortDateString();

                #region Paging

                //if (Session["RowsDisplay"] == null)
                Session["RowsDisplay"] = 100;
                //if (Session["CPage"] == null)
                Session["CPage"] = 1;

                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Save Method 
        /// </summary>
        protected void SaveQuote()
        {
            try
            {
                DataTable PercentTable = (DataTable)Session["PaymentTerms"];
                DataSet TempDS = ((DataSet)Session["FloatEnquiry"]);
                DataTable Dtbl = ((DataSet)Session["FloatEnquiry"]).Tables[0];
                Filename = FileName();
                DataRow[] rows = Dtbl.Select("Check = 'false'");
                foreach (DataRow r in rows)
                    Dtbl.Rows.Remove(r);
                Dtbl.AcceptChanges();

                if (Dtbl.Columns.Contains("GrandTotal"))
                    Dtbl.Columns.Remove("GrandTotal");
                if (Dtbl.Columns.Contains("Check"))
                    Dtbl.Columns.Remove("Check");
                if (Dtbl.Columns.Contains("RoundOff"))
                    Dtbl.Columns.Remove("RoundOff");
                if (Dtbl.Columns.Contains("PInvID"))
                    Dtbl.Columns.Remove("PInvID");
                if (Dtbl.Columns.Contains("HSCode"))
                    Dtbl.Columns.Remove("HSCode");
                if (Dtbl.Columns.Contains("PackingPercentage"))
                    Dtbl.Columns.Remove("PackingPercentage");
                if (Dtbl.Columns.Contains("IsSubItems"))
                    Dtbl.Columns.Remove("IsSubItems");
                if (Dtbl.Columns.Contains("ItemStatus"))
                    Dtbl.Columns.Remove("ItemStatus");
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
                            int del = Convert.ToInt32(NotSel[i].ToString());
                            for (int k = 0; k < Dtbl.Rows.Count; k++)
                            {
                                if (del == Convert.ToInt32(Dtbl.Rows[k]["ItemID"].ToString()))
                                {
                                    Dtbl.Rows[k].Delete();
                                    break;
                                }
                            }
                        }
                        Dtbl.AcceptChanges();
                    }
                }
                DataTable dt = CommonBLL.EmptyDtLQ_SubItems(); //(DataTable)Session["AllLQSubItems"];

                if (dt != null)
                {
                    if (dt.Columns.Contains("Check"))
                        dt.Columns.Remove("Check");
                    if (dt.Columns.Contains("TotalAmt"))
                        dt.Columns.Remove("TotalAmt");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (new Guid(dt.Rows[i]["ItemId"].ToString()) == Guid.Empty &&
                            Convert.ToDecimal(dt.Rows[i]["Rate"].ToString()) <= 0 &&
                            Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) <= 0)
                        {
                            dt.Rows[i].Delete();
                        }
                    }
                    dt.AcceptChanges();
                }
                string Attachments = "";
                if (Session["NLQUplds"] != null)
                {
                    ArrayList all = (ArrayList)Session["NLQUplds"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                if (btnSave.Text == "Save" && Dtbl.Rows.Count > 0)
                {
                    decimal exdt = 0, sgst = 0, igst = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, sgstPrcnt = 0, igstPrcnt = 0,
                        sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0, AdsnlCharges = 0, AdsnlChargesRS = 0;

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

                    if (chkPkng.Checked)
                        if (rbtnPkng.SelectedValue == "0")
                            pkngPrcnt = Convert.ToDecimal(txtPkng.Text);
                        else if (rbtnPkng.SelectedValue == "1")
                            pkng = Convert.ToDecimal(txtPkng.Text);
                        else pkng = Convert.ToDecimal(txtPkng.Text);
                    if (chkACgs.Checked)
                        if (rbtnAdtnChrgs.SelectedValue == "0")
                            AdsnlCharges = Convert.ToDecimal(txtAdtnChrgs.Text);
                        else if (rbtnAdtnChrgs.SelectedValue == "1")
                            AdsnlChargesRS = Convert.ToDecimal(txtAdtnChrgs.Text);

                    string DlvryDt = (DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text)))).ToString("dd-MM-yyyy");

                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    TCs.Columns.Remove("Title");

                    if (Dtbl.Columns.Contains("ItemDetailsIDU"))
                        Dtbl.Columns.Remove("ItemDetailsIDU");
                    if (Dtbl.Columns.Contains("ItemDesc"))
                        Dtbl.Columns.Remove("ItemDesc");
                    if (Dtbl.Columns.Contains("UnitName"))
                        Dtbl.Columns.Remove("UnitName");
                    if (Dtbl.Columns.Contains("LocalEnquireId"))
                        Dtbl.Columns["LocalEnquireId"].ColumnName = "LocalQuotationId";
                    if (Dtbl.Columns.Contains("CompanyID"))
                        Dtbl.Columns.Remove("CompanyID");

                    if (Dtbl.Rows.Count > 0)
                    {
                        res = NLQBL.LclQuoteInsertUpdate(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlcustomer.SelectedValue), new Guid(ddldept.SelectedValue),
                            new Guid(ddlfenq.SelectedValue), new Guid(ddllenq.SelectedValue), new Guid(lblsupplierID.Text), txtlquotno.Text, txtsubject.Text,
                            CommonBLL.DateInsert(txtdt.Text), CommonBLL.DateInsert(txtqdt.Text), txtimpinst.Text, exdt, exdtPrcnt, sgst, sgstPrcnt, igst, igstPrcnt,
                            dscnt, dscntPrcnt, sltx, sltxPrcnt,
                            pkng, pkngPrcnt, AdsnlChargesRS, AdsnlCharges, new Guid(ddlPriceBasis.SelectedValue), txtPriceBasis.Text, CommonBLL.DateInsert(DlvryDt),
                            int.Parse(txtDlvry.Text), Guid.Empty, CommonBLL.StatusTypeLclQuotID, Attachments, "", new Guid(Session["UserID"].ToString()),
                            Dtbl, PercentTable, TCs, dt, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0 && btnSave.Text == "Save")
                        {
                            if ((((ArrayList)Session["UserDtls"])[7].ToString()) == CommonBLL.SuplrContactTypeText)
                            {
                                SendDefaultMails(EMDBL.SelectEMailDetails(CommonBLL.FlagWCommonMstr, new Guid(ddllenq.SelectedValue), Guid.Empty, "", "", "",
                                        DateTime.Now, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                                ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "New Local Quotation", "Data Inserted Successfully.");
                                CBLL.ClearUploadedFiles();
                                ClearAll();
                                Session.Remove("TCs");
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('Saved Successfully, if you need to edit click on the link in the mail.'); window.location='../Login.aspx';", true);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Saved Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "New Local Quotation", "Data Inserted Successfully.");
                                CBLL.ClearUploadedFiles();
                                ClearAll();
                                Session.Remove("TCs");
                                Response.Redirect("LQStatus.Aspx", false);
                            }
                        }
                        else if (res != 0 && btnSave.Text == "Save")
                        {
                            ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", "Error while Inserting.");
                            Session["FloatEnquiry"] = TempDS;
                            divLQItems.InnerHtml = FillGridView(Guid.Empty, 0);
                        }
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('There are No Items to Save.');", true);
                    }
                }
                else if (btnSave.Text == "Update" && Dtbl.Rows.Count > 0)
                {
                    if (PercentTable.Columns.Contains("LQuotationId"))
                        PercentTable.Columns.Remove("LQuotationId");
                    if (PercentTable.Columns.Contains("CompanyID"))
                        PercentTable.Columns.Remove("CompanyID");
                    decimal exdt = 0, sgst = 0, igst = 0, sltx = 0, dscnt = 0, pkng = 0, exdtPrcnt = 0, sgstPrcnt = 0, igstPrcnt = 0,
                        sltxPrcnt = 0, dscntPrcnt = 0, pkngPrcnt = 0, AdsnlCharges = 0, AdsnlChargesRS = 0;

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

                    if (chkPkng.Checked)
                        if (rbtnPkng.SelectedValue == "0")
                            pkngPrcnt = Convert.ToDecimal(txtPkng.Text);
                        else if (rbtnPkng.SelectedValue == "1")
                            pkng = Convert.ToDecimal(txtPkng.Text);
                        else pkng = Convert.ToDecimal(txtPkng.Text);
                    if (chkACgs.Checked)
                        if (rbtnAdtnChrgs.SelectedValue == "0")
                            AdsnlCharges = Convert.ToDecimal(txtAdtnChrgs.Text);
                        else if (rbtnAdtnChrgs.SelectedValue == "1")
                            AdsnlChargesRS = Convert.ToDecimal(txtAdtnChrgs.Text);

                    string DlvryDt = (DateTime.Now.AddDays(7 * (Convert.ToDouble(txtDlvry.Text)))).ToString("dd-MM-yyyy");

                    DataTable TCs = CommonBLL.ATConditionsTitle();
                    if (Session["TCs"] != null)
                    {
                        TCs = (DataTable)Session["TCs"];
                    }
                    if (TCs.Columns.Contains("Title"))
                        TCs.Columns.Remove("Title");

                    if (Dtbl.Columns.Contains("ItemDetailsIDU"))
                        Dtbl.Columns.Remove("ItemDetailsIDU");
                    if (Dtbl.Columns.Contains("ItemDesc"))
                        Dtbl.Columns.Remove("ItemDesc");
                    if (Dtbl.Columns.Contains("UnitName"))
                        Dtbl.Columns.Remove("UnitName");
                    if (Dtbl.Columns.Contains("LocalEnquireId"))
                        Dtbl.Columns["LocalEnquireId"].ColumnName = "LocalQuotationId";
                    if (Dtbl.Columns.Contains("CompanyID"))
                        Dtbl.Columns.Remove("CompanyID");

                    if (Dtbl.Rows.Count > 0)
                    {
                        res = NLQBL.LclQuoteInsertUpdate(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), new Guid(ddlcustomer.SelectedValue), new Guid(ddldept.SelectedValue),
                            new Guid(ddlfenq.SelectedValue), new Guid(ddllenq.SelectedValue), new Guid(lblsupplierID.Text), txtlquotno.Text, txtsubject.Text,
                            CommonBLL.DateInsert(txtdt.Text), CommonBLL.DateInsert(txtqdt.Text), txtimpinst.Text, exdt, exdtPrcnt, sgst, sgstPrcnt, igst, igstPrcnt, dscnt, dscntPrcnt, sltx,
                            sltxPrcnt, pkng, pkngPrcnt, AdsnlChargesRS, AdsnlCharges, new Guid(ddlPriceBasis.SelectedValue), txtPriceBasis.Text,
                            CommonBLL.DateInsert(DlvryDt), int.Parse(txtDlvry.Text), Guid.Empty, CommonBLL.StatusTypeLclQuotID, Attachments, txtComments.Text.Trim(),
                            new Guid(Session["UserID"].ToString()), Dtbl, PercentTable, TCs, dt, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0 && btnSave.Text == "Update")
                        {
                            if (((ArrayList)Session["UserDtls"])[7].ToString() == CommonBLL.SuplrContactTypeText)
                            {
                                ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                SendDefaultMails(EMDBL.SelectEMailDetails(CommonBLL.FlagWCommonMstr, new Guid(ddllenq.SelectedValue), Guid.Empty, "", "", "",
                                        DateTime.Now, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())), "UpdateMails");
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "New Local Quotation", "Data Updated Successfully.");
                                CBLL.ClearUploadedFiles();
                                ClearAll(); Session.Remove("TCs");
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "Redit", "alert('Updated Successfully.'); window.location='../Login.aspx';", true);
                            }
                            else
                            {
                                ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                    "SuccessMessage('Updated Successfully.');", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "New Local Quotation", "Data Updated Successfully.");
                                CBLL.ClearUploadedFiles();
                                ClearAll(); Session.Remove("TCs");
                                Response.Redirect("LQStatus.Aspx", false);
                            }
                        }
                        else if (res != 0 && btnSave.Text == "Update")
                        {
                            ALS.AuditLog(res, btnSave.Text, txtlquotno.Text, "Local Quotation No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", "Error while Updating.");
                            Session["FloatEnquiry"] = TempDS;
                            divLQItems.InnerHtml = FillGridView(Guid.Empty, 0);
                        }
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Select atleast 1 Item(s).');", true);
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('There are No Items to Update.');", true);
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Recored
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string EditRecord(Guid ID)
        {
            try
            {

                DataTable dt = CommonBLL.EmptyDtLocal();
                DataSet EditDS = new DataSet();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                EditDS = NLEBL.SelctLocalEnquiries(CommonBLL.FlagModify, ID, new Guid(ddlcustomer.SelectedValue),
                    new Guid(ddlfenq.SelectedValue), "", "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 0, "", "", "", Guid.Empty,
                    DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), dt);
                if (EditDS.Tables.Count > 1 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0)
                {
                    ddlcustomer.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                    ddldept.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    ddlfenq.SelectedValue = EditDS.Tables[0].Rows[0]["ForeignEnquireId"].ToString();
                    txtsubject.Text = EditDS.Tables[0].Rows[0]["Subject"].ToString();
                    txtdt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    txtqdt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    txtimpinst.Text = EditDS.Tables[0].Rows[0]["Instruction"].ToString();
                    lblsupplier.Text = EditDS.Tables[0].Rows[0]["Supplier"].ToString();
                    lblsupplierID.Text = EditDS.Tables[0].Rows[0]["SupplierIds"].ToString();
                    hfLeIssueDt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["LEIssueDate"].ToString()));
                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
                    DataSet dss = new DataSet();
                    dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    Dictionary<int, Guid> Codes1 = new Dictionary<int, Guid>();
                    Dictionary<int, Guid> CatCodes1 = new Dictionary<int, Guid>();
                    Dictionary<int, Guid> UnitCodes1 = new Dictionary<int, Guid>();

                    DataTable dtt = EditDS.Tables[1].DefaultView.ToTable(false, "ItemId", "PartNumber", "Specifications",
                        "Make", "Quantity", "UNumsId", "ItemDetailsId", "Rate", "Amount");
                    dtt.Columns["ItemId"].ColumnName = "ItemDescription";
                    dtt.Columns["PartNumber"].ColumnName = "PartNo";
                    dtt.Columns["Specifications"].ColumnName = "Specification";
                    dtt.Columns["ItemDetailsId"].ColumnName = "ID";
                    dtt.Columns["UNumsId"].ColumnName = "Units";
                    dtt.Columns["Rate"].ColumnName = "Rate";
                    dtt.Columns["Amount"].ColumnName = "Amount";
                    dtt.Columns.Add(new DataColumn("ExDutyPercentage", typeof(decimal)));
                    dtt.Columns.Add(new DataColumn("DiscountPercentage", typeof(decimal)));

                    DataColumn dc = dtt.Columns.Add("SNo", typeof(int));
                    DataColumn dc1 = dtt.Columns.Add("Category", typeof(Guid));
                    dc.SetOrdinal(0);
                    dc1.SetOrdinal(1);
                    int sno = 1;
                    foreach (DataRow dr in dtt.Rows)
                    {
                        dr["SNo"] = sno;
                        dr["Category"] = Guid.Empty;
                        dr["Rate"] = "0.00";
                        dr["Amount"] = "0.00";
                        dr["ExDutyPercentage"] = "0.1";
                        dr["DiscountPercentage"] = "0.00";
                        Codes1.Add(sno, new Guid(dr["ItemDescription"].ToString()));
                        UnitCodes1.Add(sno, new Guid(dr["Units"].ToString()));
                        sno++;
                    }
                    CatCodes1.Add(1, Guid.Empty);
                    HttpContext.Current.Session["SelectedItems"] = Codes1;
                    HttpContext.Current.Session["SelectedCat"] = CatCodes1;
                    HttpContext.Current.Session["SelectedUnits"] = UnitCodes1;
                    HttpContext.Current.Session["IsItems"] = true;

                    DataRow drn = dtt.NewRow();
                    drn["SNo"] = 1;
                    drn["Category"] = Guid.Empty;
                    drn["ItemDescription"] = Guid.Empty;
                    drn["PartNo"] = string.Empty;
                    drn["Specification"] = string.Empty;
                    drn["Make"] = string.Empty;
                    drn["Quantity"] = 0;
                    drn["Units"] = Guid.Empty;
                    drn["Rate"] = "0.00";
                    drn["Amount"] = "0.00";
                    drn["ExDutyPercentage"] = "0.1";
                    drn["DiscountPercentage"] = "0.00";
                    drn["ID"] = Guid.Empty;
                    dtt.Rows.Add(drn);

                    HttpContext.Current.Session["EnqId"] = ID;
                    HttpContext.Current.Session["ItemCode"] = dtt;
                    DataSet dstbl = new DataSet();

                    if (EditDS.Tables.Count >= 4)
                    {
                        if (EditDS.Tables[3].ToString() != null && EditDS.Tables[3].ToString() != "")
                        {
                            Session["AllLQSubItems"] = EditDS.Tables[3];
                        }
                    }
                    sb.Append(FillGridView(new Guid(ddlfenq.SelectedValue), 0));

                    sb.Append("</tbody></table>");

                    ViewState["btnSaveID"] = "Update";
                    return sb.ToString();
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// Edit L-Quotation
        /// </summary>
        /// <param name="ID"></param>
        private void EditLQuotation(Guid ID)
        {
            try
            {
                ViewState["EditID"] = ID;
                HFID.Value = ID.ToString();
                DataSet dss = new DataSet();
                dss = NLQBL.LclQuoteSelect(CommonBLL.FlagASelect, ID, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", 0, "", Guid.Empty,
                    CommonBLL.EmptyDtLocalQuotation(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (dss.Tables.Count >= 4 && dss.Tables[0].Rows.Count > 0 && dss.Tables[1].Rows.Count > 0 && dss.Tables[2].Rows.Count > 0)
                {
                    ddlcustomer.SelectedValue = dss.Tables[0].Rows[0]["CustomerId"].ToString();

                    BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagODRP, Guid.Empty, Guid.Empty, new Guid(dss.Tables[0].Rows[0]["CustomerId"].ToString()), Guid.Empty, "", DateTime.Now, "",
                   "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    ddlfenq.SelectedValue = dss.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();

                    BindDropDownList(ddllenq, NLEBL.SelctLocalEnquiries(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(dss.Tables[0].Rows[0]["CustomerId"].ToString()),
                        new Guid(dss.Tables[0].Rows[0]["ForeignEnquiryId"].ToString()), "",
                    "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 60, "", "", "", Guid.Empty,
                    DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal()));
                    ddllenq.SelectedValue = dss.Tables[0].Rows[0]["LocalEnquireId"].ToString();

                    if (dss.Tables.Count >= 5)
                    {
                        Session["AllLQSubItems"] = dss.Tables[4];
                        AddColumns_SubItems(true);
                    }

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CalcexDuty", "CalculateExDuty();", true);
                    divLQItems.InnerHtml = FillGridView(dss);

                    txtlquotno.Text = dss.Tables[0].Rows[0]["Quotationnumber"].ToString();
                    if (dss.Tables[0].Rows[0]["EntryDate"].ToString() != "")
                        txtdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(dss.Tables[0].Rows[0]["EntryDate"].ToString()));
                    txtsubject.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                    ddldept.SelectedValue = dss.Tables[0].Rows[0]["DepartmentId"].ToString();
                    lblsupplierID.Text = dss.Tables[0].Rows[0]["SupplierId"].ToString();
                    lblsupplier.Text = dss.Tables[0].Rows[0]["SuplrNm"].ToString();
                    txtimpinst.Text = dss.Tables[0].Rows[0]["instruction"].ToString();
                    txtPriceBasis.Text = dss.Tables[0].Rows[0]["PriceBasisText"].ToString();
                    txtqdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(dss.Tables[0].Rows[0]["QuotationDate"].ToString()));
                    hfLeIssueDt.Value = CommonBLL.DateDisplay(Convert.ToDateTime(dss.Tables[0].Rows[0]["LEIssueDate"].ToString()));

                    DataSet edFe = (DataSet)Session["FloatEnquiry"];
                    Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                    for (int k = 0; k < edFe.Tables[0].Rows.Count; k++)
                    {

                        if (Convert.ToDecimal(edFe.Tables[0].Rows[k]["ExDutyPercentage"].ToString()) > 0)
                            FlagExDuty = 1;
                        if (Convert.ToDecimal(edFe.Tables[0].Rows[k]["DiscountPercentage"].ToString()) > 0)
                            FlagDiscount = 1;

                        Guid itemId = new Guid(edFe.Tables[0].Rows[k]["ItemId"].ToString());
                        Codes.Add(k, itemId);
                    }
                    Session["SelectedItems"] = Codes;
                    if (FlagDiscount == 0)
                        chkDsnt.Enabled = true;
                    else
                        chkDsnt.Enabled = false;

                    if (FlagExDuty == 0)
                    {
                        chkExdt.Enabled = true;
                        chkSGST.Enabled = true;
                        chkIGST.Enabled = true;
                    }
                    else
                    {
                        chkExdt.Enabled = false;
                        chkSGST.Enabled = false;
                        chkIGST.Enabled = false;
                    }
                    Session["TCs"] = (CBL.SelectATConditions(CommonBLL.FlagWCommonMstr, Guid.Empty, ID, Guid.Empty, Guid.Empty, Guid.Empty, 0, Guid.Empty, "",
                        new Guid(Session["UserID"].ToString()))).Tables[0];
                    var LineItemGST = dss.Tables[1].AsEnumerable().Where(R => R.Field<decimal>("ExDutyPercentage") > Decimal.Zero).ToList();
                    if (LineItemGST.Count <= 0)
                    {
                        if (dss.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00" || dss.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != "0.00")
                        {
                            if (dss.Tables[0].Rows[0]["ExDuty"].ToString() != "0.00")
                            {
                                txtExdt.Text = dss.Tables[0].Rows[0]["ExDuty"].ToString();
                                rbtnExdt.SelectedValue = "1";
                            }
                            else if (dss.Tables[0].Rows[0]["ExDutyPercentage"].ToString() != "0.00")
                            {
                                txtExdt.Text = dss.Tables[0].Rows[0]["ExDutyPercentage"].ToString();
                                rbtnExdt.SelectedValue = "0";
                            }
                            chkExdt.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", true);
                        }
                        if (dss.Tables[0].Rows[0]["SGST"].ToString() != "0.00" || dss.Tables[0].Rows[0]["SGSTPercentage"].ToString() != "0.00")
                        {
                            if (dss.Tables[0].Rows[0]["SGST"].ToString() != "0.00")
                            {
                                txtSGST.Text = dss.Tables[0].Rows[0]["SGST"].ToString();
                                rbtnSGST.SelectedValue = "1";
                            }
                            else if (dss.Tables[0].Rows[0]["SGSTPercentage"].ToString() != "0.00")
                            {
                                txtSGST.Text = dss.Tables[0].Rows[0]["SGSTPercentage"].ToString();
                                rbtnSGST.SelectedValue = "0";
                            }
                            chkSGST.Checked = true;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowSGSTE", "CHeck('chkSGST', 'dvSGST');", true);
                        }
                        if (dss.Tables[0].Rows[0]["IGST"].ToString() != "0.00" || dss.Tables[0].Rows[0]["IGSTPercentage"].ToString() != "0.00")
                        {
                            if (dss.Tables[0].Rows[0]["IGST"].ToString() != "0.00")
                            {
                                txtIGST.Text = dss.Tables[0].Rows[0]["IGST"].ToString();
                                rbtnIGST.SelectedValue = "1";
                            }
                            else if (dss.Tables[0].Rows[0]["IGSTPercentage"].ToString() != "0.00")
                            {
                                txtIGST.Text = dss.Tables[0].Rows[0]["IGSTPercentage"].ToString();
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
                    if (dss.Tables[0].Rows[0]["packing"].ToString() != "0.00" || dss.Tables[0].Rows[0]["packingPercentage"].ToString() != "0.00")
                    {
                        if (dss.Tables[0].Rows[0]["packing"].ToString() != "0.00")
                        {
                            txtPkng.Text = dss.Tables[0].Rows[0]["packing"].ToString();
                            rbtnPkng.SelectedValue = "1";
                        }
                        else if (dss.Tables[0].Rows[0]["packingPercentage"].ToString() != "0.00")
                        {
                            txtPkng.Text = dss.Tables[0].Rows[0]["packingPercentage"].ToString();
                            rbtnPkng.SelectedValue = "0";
                        }
                        chkPkng.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkPkng", "CHeck('chkPkng', 'dvPkng');", true);
                    }

                    rbtnDsnt.SelectedValue = "0";
                    if (dss.Tables[0].Rows[0]["Discount"].ToString() != "0.00" ||
                        dss.Tables[0].Rows[0]["DiscountPercentage"].ToString() != "0.00")
                    {
                        if (dss.Tables[0].Rows[0]["Discount"].ToString() != "0.00")
                        {
                            txtDsnt.Text = dss.Tables[0].Rows[0]["Discount"].ToString();
                            rbtnDsnt.SelectedValue = "1";
                        }
                        else if (dss.Tables[0].Rows[0]["DiscountPercentage"].ToString() != "0.00")
                        {
                            txtDsnt.Text = dss.Tables[0].Rows[0]["DiscountPercentage"].ToString();
                            rbtnDsnt.SelectedValue = "0";
                        }
                        chkDsnt.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkDsnt", "CHeck('chkDsnt', 'dvDsnt');", true);
                    }

                    if (dss.Tables[0].Rows[0]["AdsnlCharges"].ToString() != "0.00")
                    {
                        txtAdtnChrgs.Text = dss.Tables[0].Rows[0]["AdsnlCharges"].ToString();
                        rbtnAdtnChrgs.SelectedValue = "1";

                        chkACgs.Checked = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowchkAdsnlChrgs",
                            "CHeck('chkACgs', 'dvAdtnChgs');", true);
                    }

                    if (dss.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        string[] all = dss.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
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
                        Session["NLQUplds"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                        divListBox.InnerHtml = "";

                    ddlPriceBasis.SelectedValue = dss.Tables[0].Rows[0]["PriceBasis"].ToString();
                    txtDlvry.Text = dss.Tables[0].Rows[0]["DeliveryPeriods"].ToString();
                    DataTable PaymentDt = dss.Tables[2].Copy();
                    PaymentDt.Columns["Against"].ColumnName = "Description";
                    PaymentDt.Columns["PaymentSerialNo"].ColumnName = "SNo";
                    PaymentDt.Columns["Percentage"].ColumnName = "PaymentPercentage";
                    RemoveColumns(PaymentDt);

                    Session["PaymentTerms"] = PaymentDt;
                    if (PaymentDt.Rows.Count > 0)
                    {
                        Session["AmountLQ"] = Convert.ToInt32(PaymentDt.Compute("Sum(PaymentPercentage)", ""));
                        divPaymentTerms.InnerHtml = FillPaymentTerms();
                    }
                    btnSave.Text = "Update";
                    ViewState["EditID"] = ID;
                    ddlcustomer.Enabled = ddlfenq.Enabled = ddllenq.Enabled = txtdt.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Remove Columns
        /// </summary>
        /// <param name="dtt"></param>
        /// <returns></returns>
        private DataTable RemoveColumns(DataTable dtt)
        {
            try
            {
                dtt.Columns.Remove("PaymentTermsId");
                dtt.Columns.Remove("FQuotationId");
                dtt.Columns.Remove("FPurchaseOrderId");
                dtt.Columns.Remove("LPurchaseOrderId");
                dtt.Columns.Remove("CreatedBy");
                dtt.Columns.Remove("CreatedDate");
                dtt.Columns.Remove("ModifiedBy");
                dtt.Columns.Remove("ModifiedDate");
                dtt.Columns.Remove("IsActive");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
            }
            return dtt;
        }

        /// <summary>
        /// Payment Terms Filling
        /// </summary>
        /// <returns></returns>
        public string FillPaymentTerms()
        {
            try
            {
                int TotalAmt = 0;
                string Message = "";
                if (Session["AmountLQ"] != null)
                    TotalAmt = Convert.ToInt32(Session["AmountLQ"]);
                if (Session["MessageLQ"] != null)
                    Message = Session["MessageLQ"].ToString();
                DataTable dt = (DataTable)Session["PaymentTerms"];

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='32%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                " id='tblPaymentTerms' align='center'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Payment(%)</th>" +
                    "<th>Description</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)//onBlur 
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");

                        sb.Append
                            ("<td align='right'><input type='text' name='txtPercAmt' onfocus='this.select()' onMouseUp='return false' value='"
                            + Convert.ToDouble(dt.Rows[i]["PaymentPercentage"].ToString()).ToString("F0") + "'  id='txtPercAmt" + SNo
                            + "' onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' onkeypress='return blockNonNumbers(this, event, true, false);' "
                            + "onchange='getPaymentValues(" + SNo
                            + ")' maxlength='3' style='text-align: right; width: 50px;' class='bcAsptextbox'/></td>");
                        sb.Append("<td><input type='text' name='txtDesc' value='" + dt.Rows[i]["Description"].ToString()
                            + "'  id='txtDesc" + SNo + "' onchange='getPaymentValues(" + SNo + ")' class='bcAsptextbox'/></td>");
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
                                ")' title='Delete'><img src='../images/Delete.png'/></a>&nbsp;&nbsp; " +
                                "<a href='javascript:void(0)' onclick='getPaymentValues(" + SNo +
                                ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                "onclick='javascript:return doConfirmPayment(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='3' class='rounded-foot-right' " +
                    " align='left'> Total Percent : <b>" + TotalAmt + "</b>%<input id='HfMessage' type='hidden' name='HfMessage' value='"
                    + Message + "'/></th></tfoot>");
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
        /// To Fill Items Grid
        /// </summary>
        /// <param name="CodeID">Item code</param>
        /// <param name="dt">Data Table</param>
        /// <param name="Codes">Selected Items</param>
        /// <param name="ds">Items Dataset</param>
        /// <param name="CatID">Category ID</param>
        /// <param name="CatCodes">Selected Category ID</param>
        /// <param name="UnitID">Unit ID</param>
        /// <param name="UnitCodes">Selected Units</param>
        /// <returns>Returns HTML Items Table</returns>
        private string FillGridView(Guid EnqID, decimal Gtotl)
        {
            try
            {
                DataSet dsi = new DataSet(); string ItemIDs = string.Empty;
                DataSet ds1 = new DataSet();
                FlagDiscount = 0;
                FlagExDuty = 0;
                if (EnqID != Guid.Empty)
                {

                    dsi = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, EnqID, new Guid(ddllenq.SelectedValue),
                        Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                        DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));

                    dsi.Tables[0].Columns["ItemDetailsID"].ColumnName = "ItemDetailsIDU";
                    dsi.Tables[0].Columns.Remove("ForeignEnquireId");
                    dsi.Tables[0].Columns.Remove("ForeignQuotationId");
                    dsi.Tables[0].Columns.Remove("ForeignPOId");
                    dsi.Tables[0].Columns.Remove("LocalQuotationId");
                    dsi.Tables[0].Columns.Remove("LocalPOId");
                    dsi.Tables[0].Columns.Remove("CreatedBy");
                    dsi.Tables[0].Columns.Remove("CreatedDate");
                    dsi.Tables[0].Columns.Remove("ModifiedBy");
                    dsi.Tables[0].Columns.Remove("ModifiedDate");
                    dsi.Tables[0].Columns.Remove("IsActive");
                    dsi.Tables[0].Columns.Add("GrandTotal", typeof(double));
                    dsi.Tables[0].Columns.Add("Check", typeof(bool));
                    Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                    int DiscChkBox = 0;
                    int ExDutyChkBox = 0;
                    double ExPercent = 0;
                    if (dsi.Tables.Count > 1 && dsi.Tables[1].Rows.Count > 0)
                        ExPercent = Convert.ToDouble(dsi.Tables[1].Rows[0]["ExPercent"].ToString());
                    for (int k = 0; k < dsi.Tables[0].Rows.Count; k++)
                    {
                        dsi.Tables[0].Rows[k]["ExDutyPercentage"] = 0.1;
                        dsi.Tables[0].Rows[k]["DiscountPercentage"] = "0.00";
                        dsi.Tables[0].Rows[k]["GrandTotal"] = "0.00";
                        dsi.Tables[0].Rows[k]["Check"] = true;
                        if (Convert.ToDecimal(dsi.Tables[0].Rows[k]["ExDutyPercentage"].ToString()) > 0)
                            ExDutyChkBox = 1;
                        if (Convert.ToDecimal(dsi.Tables[0].Rows[k]["DiscountPercentage"].ToString()) > 0)
                            DiscChkBox = 1;
                        Guid CodeID = new Guid(dsi.Tables[0].Rows[k]["ItemId"].ToString());
                        Codes.Add(k, CodeID);
                    }
                    Session["SelectedItems"] = Codes;
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
                    dsi.Tables[0].AcceptChanges();
                    Session["FloatEnquiry"] = dsi;
                    Session["TempFloatEnquiry"] = dsi;
                    Session["EnqID"] = EnqID;
                }
                else
                    dsi = (DataSet)Session["FloatEnquiry"];
                if (dsi != null && dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                {
                    DataSet dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    DataSet dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                    if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                    {
                        PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                            .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                    }

                    #region Paging

                    string DisablePrevious = " disabled ", DisableNext = " disabled ";
                    int Rows2Display = Convert.ToInt32(Session["RowsDisplay"].ToString()), CurrentPage = Convert.ToInt32(Session["CPage"].ToString()), Rows2Skip = 0;
                    int RowsCount = dsi.Tables[0].Rows.Count, PageCount = 0;

                    if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                    {
                        if ((Convert.ToDecimal(RowsCount) / Convert.ToDecimal(Rows2Display)).ToString().Contains('.'))
                            PageCount = (RowsCount / Rows2Display) + 1;
                        else
                            PageCount = RowsCount / Rows2Display;

                        if (Convert.ToInt32(Session["CPage"].ToString()) > PageCount)
                            CurrentPage--;
                        else if (Convert.ToInt32(Session["CPage"].ToString()) < 1)
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
                    }

                    #endregion

                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                    " id='tblItems' style='font-size: small;'><thead align='left'><tr>");
                    //if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                    //    sb.Append("<th class='rounded-First'>&nbsp;</th><th></th><th align='center'>SNo</th><th align='center'>Item Description</th>" +
                    //        "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
                    //        "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
                    //        "<th align='center'>Amount</th>" +
                    //        "<th align='center'>Discount</th><th align='center'>Net Rate</th>"
                    //        + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
                    //else
                    //sb.Append("<tr><th class='rounded-First' colspan='14'>"
                    //+ "</th><th class='rounded-Last'></th></tr>");

                    sb.Append("<tr><th class='rounded-First'>&nbsp;</th><th></th><th align='center'>SNo</th><th align='center'>Item Description</th>" +
                    "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
                    "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
                    "<th align='center'>Amount</th>" +
                    "<th align='center'>Discount</th><th align='center'>GST</th><th align='center'>Net Rate</th>"
                    + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
                    sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                    if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                    {
                        #region Paging

                        DataSet ds = new DataSet();
                        DataTable dt = dsi.Tables[0].AsEnumerable().Skip(Rows2Skip).Take(Rows2Display).CopyToDataTable();
                        ds.Tables.Add(dt);

                        #endregion
                        decimal TotalAmount = 0;
                        decimal GrandTotal = 0;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string sno = (Rows2Skip + i + 1).ToString();
                            sb.Append("<tr valign='Top'>");

                            string ExpandImg = "";
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["IsSubItems"]))
                            {
                                ExpandImg = "<span class='gridactionicons'><a id='btnExpand" + sno + "' href='javascript:void(0)' " +
                                                 " onclick='ExpandSubItems(" + sno + ")' title='Expand'><img id='" + sno +
                                                 "' src='../images/expand.png' style='border-style: none; height: 18px; width: 18px;'/></a></span>";
                            }
                            sb.Append("<td align='center'> " + ExpandImg + " </td>");

                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
                            {
                                TotalAmount += Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString());
                                GrandTotal += Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString());
                                sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                    + sno + ")' type='checkbox' checked='checked' name='CheckAll'/></td>");
                            }
                            else
                                sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                    + sno + ")' type='checkbox' name='CheckAll' /></td>");
                            sb.Append("<td align='center'>" + (sno).ToString());
                            sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + (i + 1).ToString() +
                                ")' id='HItmID" + (i + 1).ToString() + "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString()
                                + "' width='5px' style='WIDTH: 5px;' class='bcAsptextbox'/></td>");

                            if (ds.Tables[0].Rows[i]["ItemDetailsIdU"].ToString() == "" &&
                                ds.Tables[0].Rows[i]["ItemId"].ToString() == "")
                            {
                                sb.Append("<td><select id='ddl" + sno + "' class='PayElementCode' onchange='FillItemDRP(" + sno + ")'>");
                                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                                {
                                    sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                    foreach (DataRow drr in dss.Tables[0].Rows)
                                        if (!ItemIDs.Contains(drr["ID"].ToString()))
                                        {
                                            sb.Append("<option value='" + drr["ID"].ToString() + "' >" + drr["ItemDescription"].ToString()
                                                + "</option>");
                                        }
                                }
                                sb.Append("</select>");
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)'  class='icons additionalrow'  ID='btnShow"
                                            + (i + 1) + "'  title='Add Item to Item Master' onclick='fnOpenItems(" + sno + ","
                                            + i.ToString() + ")' ><img src='../images/AddNW.jpeg'/></a></span></td>");
                            }
                            else
                            {
                                if ((Convert.ToBoolean(Session["NewItemAdded"]) || ds.Tables[0].Rows[i]["ItemDesc"].ToString() == "")
                                    && (ds.Tables[0].Rows.Count - 1) == i)
                                {
                                    Guid NewitemID = new Guid(ds.Tables[0].Rows[i]["ItemId"].ToString());
                                    DataRow[] selRow = dss.Tables[0].Select("ID = '" + NewitemID.ToString() + "'");
                                    if (selRow.Length > 0)
                                    {
                                        ds.Tables[0].Rows[i]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();
                                        sb.Append("<td><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString()
                                            + "</div></td>");
                                        Session["NewItemAdded"] = false;
                                    }
                                }
                                else
                                    sb.Append("<td valign='Top' width='200px'><div class='expanderR'><span id='lbldescip"
                                        + sno + "'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</span></div></td>");
                                ItemIDs = ItemIDs + "," + ds.Tables[0].Rows[i]["ItemID"].ToString();
                            }
                            sb.Append("<td><span id='lblpartno" + sno + "'>" + ds.Tables[0].Rows[i]["PartNumber"].ToString()
                                + "</span></td>");

                            sb.Append("<td><textarea name='txtSpecifications' id='txtSpecifications" + sno
                                        + "' onchange='FillItemsAll(" + sno + ")' Class='bcAsptextboxmulti' onfocus='ExpandTXT(" + sno
                                        + ")' onblur='ReSizeTXT(" + sno + ")' style='height:22px; width:150px; resize:none;'>"
                                        + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
                            sb.Append("<td><input type='text' name='txtMake' size='05px' onchange='FillItemsAll(" + sno
                                + ")' id='txtMake" + sno + "' value='"
                                + ds.Tables[0].Rows[i]["Make"].ToString() + "' style='width:50px;' class='bcAsptextbox'/></td>");
                            sb.Append("<td><input type='text' name='txtQuantity' readonly='true' size='05px' onchange='FillItemsAll("
                            + sno + ")' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString()
                                + "' onkeypress='return blockNonNumbers(this, event, false, false);' " +
                                " maxlength='6' style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");

                            if (ds.Tables[0].Rows[i]["UnitName"].ToString() == "")
                            {
                                sb.Append("<td width='20px'><select id='ddlUnits" + sno + "' class='PayUnitCode' onchange='FillItemsAll("
                                    + sno + ")'>");
                                if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                                {
                                    sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                    foreach (DataRow dru in dsEnm.Tables[0].Rows)
                                        sb.Append("<option value='" + dru["ID"].ToString() + "' >"
                                            + dru["Description"].ToString() + "</option>");
                                }
                                sb.Append("</select></td>");
                            }
                            else
                                sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");

                            sb.Append("<td><input type='text' size='05px' name='txtPrice' id='txtPrice" + sno
                            + "' onfocus='this.select()' onMouseUp='return false' value='"
                                + ds.Tables[0].Rows[i]["Rate"].ToString() + "' onchange='CalculateTotalAmount(" + sno
                                + ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' " +
                                " onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                " style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");

                            //if (chkDsnt.Checked == true || chkDsnt.Enabled == true)
                            //    read = "READONLY";
                            //else
                            //    read = "";'"+ read +"
                            sb.Append("<td align='right'><span id='spnAmount" + sno + "'>"
                            + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"].ToString()), 2) + "</span></td>");
                            sb.Append("<td><input type='text' size='05px' name='txtDiscount' id='txtDiscount" + sno
                                + "' onfocus='this.select()' onMouseUp='return false'  value='"
                                + ds.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "' onchange='FillItemsAll(" + sno +
                                ")' maxlength='18' onblur='extractNumber(this,2,false);' onkeyup='CheckDiscount(" + sno +
                                "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                "  style='text-align: right; width:50px;' class='bcAsptextbox'/>%</td>");
                            //if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                            //    sb.Append("");
                            //else
                            //{
                            sb.Append("<td><input type='text' size='05px' id='txtPercent" + sno +
                            "' onfocus='this.select()' onMouseUp='return false' value='"
                                + Convert.ToString(ds.Tables[0].Rows[i]["ExDutyPercentage"]) + "' onchange='FillItemsAll(" + sno
                                + ")' onblur='extractNumber(this,2,false);' onkeyup='CheckExDuty(" + sno
                                + "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' " +
                                " maxlength='18' style='text-align: right; width: 50px;' class='bcAsptextbox'/>%</td>");
                            // }
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0)
                                FlagExDuty = 1;
                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0)
                                FlagDiscount = 1;
                            sb.Append("<td align='right'><span id='spnNetRate" + sno + "'>"
                                + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["QPrice"].ToString()), 2)
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

                        #region Sub Items

                        DataTable dt_S = (DataTable)Session["AllLQSubItems"];
                        if (dt_S != null && dt_S.Rows.Count > 0)
                        {
                            if (!dt_S.Columns.Contains("GrandTotal"))
                                dt_S.Columns.Add("GrandTotal", typeof(double));
                            if (!dt_S.Columns.Contains("Check"))
                                dt_S.Columns.Add("Check", typeof(bool));
                            for (int m = 0; m < dt_S.Rows.Count; m++)
                            {
                                dt_S.Rows[m]["GrandTotal"] = "0.00";
                                dt_S.Rows[m]["Check"] = true;
                            }
                            dt_S.AcceptChanges();
                            for (int j = 0; j < dt_S.Rows.Count; j++)
                            {
                                if (Convert.ToDecimal(dt_S.Rows[j]["ExDutyPercentage"].ToString()) > 0)
                                    FlagExDuty = 1;
                                if (Convert.ToDecimal(dt_S.Rows[j]["DiscountPercentage"].ToString()) > 0)
                                    FlagDiscount = 1;
                                if (Convert.ToBoolean(dt_S.Rows[j]["Check"]))
                                {
                                    TotalAmount += Convert.ToDecimal(dt_S.Rows[j]["Amount"]);
                                    GrandTotal += Convert.ToDecimal(dt_S.Rows[j]["GrandTotal"]);
                                }
                            }
                        }

                        #endregion

                        if (Gtotl != 0)
                            GrandTotal = Convert.ToDecimal(Gtotl);
                        sb.Append("</tbody>");
                        sb.Append("<tfoot>");
                        sb.Append("<tr class='bcGridViewHeaderStyle'>");
                        sb.Append("<th colspan='7' class='rounded-foot-left'><b><span></span></b></th>");
                        sb.Append("<th align='right'> <input type='hidden' name='HFExDuty' id='HFExDuty' value='" + FlagExDuty + "'/> </th>");
                        sb.Append("<th align='right'> <input type='hidden' name='HFDiscount' id='HFDiscount' value='"
                            + FlagDiscount + "'/> </th>");
                        sb.Append("<th colspan='4' align='right'><b><span>Total Amount(" + PriceSymbol + "): <label id='lblTotalAmt'>"
                            + Math.Round(Convert.ToDecimal(TotalAmount), 2) + "</label></span></b></th>");
                        sb.Append("<th colspan='4' align='right'><b><span>Grand Total(" + PriceSymbol + "): <label id='lblGTAmt'> "
                            + Math.Round(Convert.ToDecimal(GrandTotal), 2) + "</label></span></b></th>");
                        sb.Append("<th colspan='4' class='rounded-foot-right' ><b><span></span></b></th>");
                        sb.Append("</tr>");
                    }

                    sb.Append("<tfoot><tr><th></th><th colspan='16'>");

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

                        //+ ",&nbsp;Rows to Display : <input type='text' id='txtRowsChanged' Style='text-align: right; width:50px;' onkeypress='return isNumberKey(event)' "
                        //+ "maxlength='3' onchange='RowsChanged()' value='" + Rows2Display + "'/>"
                        + ss

                        + "<input " + DisablePrevious + " type='button' id='btnPrevious' value='Previous' onclick='PrevPage()' style='width:70px'/>"
                        + "<input " + DisableNext + " type='button' id='btnNext' value='Next' onclick='NextPage()'  style='width:70px' /></th>");

                    #endregion

                    sb.Append("<th></th></tr></tfoot>");
                    sb.Append("</table>");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return ex.Message;
            }
        }

        /// <summary>
        /// Fill Items to Item Table
        /// </summary>
        /// <param name="EnqID"></param>
        /// <param name="LQuoteID"></param>
        /// <returns></returns>
        private string FillGridView(DataSet FnlItems)
        {
            try
            {
                DataSet ds = new DataSet(); string ItemIDs = string.Empty;
                ds.Tables.Add(FnlItems.Tables[3].Copy());
                if (ds != null)
                {
                    ds.Tables[0].Columns["ItemDetailsID"].ColumnName = "ItemDetailsIDU";
                    ds.Tables[0].Columns.Remove("ForeignEnquireId");
                    ds.Tables[0].Columns.Remove("ForeignQuotationId");
                    ds.Tables[0].Columns.Remove("ForeignPOId");
                    ds.Tables[0].Columns.Remove("LocalQuotationId");
                    ds.Tables[0].Columns.Remove("LocalPOId");
                    ds.Tables[0].Columns.Remove("CreatedBy");
                    ds.Tables[0].Columns.Remove("CreatedDate");
                    ds.Tables[0].Columns.Remove("ModifiedBy");
                    ds.Tables[0].Columns.Remove("ModifiedDate");
                    ds.Tables[0].Columns.Remove("IsActive");

                    ds.Tables[0].Columns.Add("GrandTotal", typeof(double));
                    ds.Tables[0].Columns.Add("Check", typeof(bool));

                    for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                    {
                        Guid itemId = new Guid(ds.Tables[0].Rows[k]["ItemId"].ToString());
                        DataRow[] foundRows = FnlItems.Tables[1].Select("ItemId = '" + itemId + "'");

                        if (foundRows.Length > 0)
                        {
                            double Amount = Convert.ToDouble(foundRows[0]["Amount"].ToString());
                            double Rate = Convert.ToDouble(foundRows[0]["QPrice"].ToString());
                            double Quantity = Convert.ToDouble(foundRows[0]["Quantity"].ToString());

                            ds.Tables[0].Rows[k]["Make"] = foundRows[0]["Make"].ToString();
                            ds.Tables[0].Rows[k]["Specifications"] = foundRows[0]["Specifications"].ToString();
                            ds.Tables[0].Rows[k]["Remarks"] = foundRows[0]["Remarks"].ToString();

                            ds.Tables[0].Rows[k]["Rate"] = Convert.ToDouble(foundRows[0]["Rate"].ToString());
                            ds.Tables[0].Rows[k]["QPrice"] = Rate;
                            ds.Tables[0].Rows[k]["Amount"] = String.Format("{0:0.00}", Math.Round(Amount, 2));
                            ds.Tables[0].Rows[k]["DiscountPercentage"] = Convert.ToDouble(foundRows[0]["DiscountPercentage"].ToString());
                            ds.Tables[0].Rows[k]["ExDutyPercentage"] = Convert.ToDouble(foundRows[0]["ExDutyPercentage"].ToString());
                            ds.Tables[0].Rows[k]["GrandTotal"] = String.Format("{0:0.00}", Math.Round(Rate * Quantity, 2));
                            ds.Tables[0].Rows[k]["Check"] = true;
                        }
                        else
                        {
                            ds.Tables[0].Rows[k]["Rate"] = 0.00;
                            ds.Tables[0].Rows[k]["QPrice"] = 0.00;
                            ds.Tables[0].Rows[k]["Amount"] = 0.00;
                            ds.Tables[0].Rows[k]["DiscountPercentage"] = 0.00;
                            ds.Tables[0].Rows[k]["ExDutyPercentage"] = 0.1;
                            ds.Tables[0].Rows[k]["GrandTotal"] = 0;
                            ds.Tables[0].Rows[k]["Check"] = false;
                        }

                    }
                    ds.Tables[0].AcceptChanges();

                    Session["FloatEnquiry"] = ds;
                    Session["TempFloatEnquiry"] = ds;
                }
                else
                    ds = (DataSet)Session["FloatEnquiry"];

                DataSet dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                DataSet dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);


                if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                {
                    PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                        .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='98%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                    " id='tblItems'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>&nbsp;</th><th></th><th align='center'>SNo</th><th align='center'>Item Description</th>" +
                    "<th align='center'>Part No</th><th align='center'>Spec</th><th align='center'>Make" +
                    "</th><th align='center'>Qty</th><th align='center'>Units</th><th align='center'>Price</th>" +
                    "<th align='center'>Amount</th>" +
                    "<th align='center'>Discount</th><th align='center'>Ex Duty</th><th align='center'>Net Rate</th>"
                    + "<th align='center'>Total</th><th>Remarks</th><th></th><th class='rounded-Last'></th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal TotalAmount = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Amount)", "").ToString());
                    decimal GrandTotal = Convert.ToDecimal(ds.Tables[0].Compute("Sum(GrandTotal)", "").ToString());
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string sno = (i + 1).ToString();
                        sb.Append("<tr valign='Top'>");

                        string ExpandImg = "";
                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["IsSubItems"]))
                        {
                            ExpandImg = "<span class='gridactionicons'><a id='btnExpand" + sno + "' href='javascript:void(0)' " +
                                             " onclick='ExpandSubItems(" + sno + ")' title='Expand'><img id='" + sno +
                                             "' src='../images/expand.png' style='border-style: none; height: 18px; width: 18px;'/></a></span>";
                        }
                        sb.Append("<td align='center'> " + ExpandImg + " </td>");

                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
                        {
                            sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                + sno + ")' type='checkbox' checked='checked' name='CheckAll'/></td>");
                        }
                        else
                            sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='FillItemsAll("
                                + sno + ")' type='checkbox' name='CheckAll'/></td>");
                        sb.Append("<td align='center'>" + (i + 1).ToString());
                        sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + (i + 1).ToString() + ")' id='HItmID"
                            + (i + 1).ToString() + "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString() +
                            "' width='5px' style='WIDTH: 5px;'/>");
                        sb.Append("</td>");
                        if (ds.Tables[0].Rows[i]["ItemDetailsIdU"].ToString() == "" && ds.Tables[0].Rows[i]["ItemId"].ToString() == "")
                        {
                            sb.Append("<td><select id='ddl" + sno + "' class='PayElementCode' onchange='FillItemDRP(" + sno + ")'>");
                            if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow drr in dss.Tables[0].Rows)
                                    if (!ItemIDs.Contains(drr["ID"].ToString()))
                                    {
                                        sb.Append("<option value='" + drr["ID"].ToString() + "' >" + drr["ItemDescription"].ToString() +
                                            "</option>");
                                    }
                            }
                            sb.Append("</select>");
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)'  class='icons additionalrow'  ID='btnShow"
                                    + (i + 1) + "'  title='Add Item to Item Master' onclick='fnOpen(" + sno + ","
                                    + i.ToString() + ")' ><img src='../images/AddNW.jpeg'/></a></span></td>");
                        }
                        else
                        {
                            sb.Append("<td valign='Top' width='200px'><div class='expanderR'><span id='lbldescip" + sno +
                                "'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</span></div></td>");
                            ItemIDs = ItemIDs + "," + ds.Tables[0].Rows[i]["ItemID"].ToString();
                        }
                        sb.Append("<td><span id='lblpartno" + sno + "'>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</span></td>");
                        sb.Append("<td><textarea name='txtSpecifications' id='txtSpecifications" + sno
                                        + "' onchange='FillItemsAll(" + sno + ")' Class='bcAsptextboxmulti' onfocus='ExpandTXT(" + sno
                                        + ")' onblur='ReSizeTXT(" + sno + ")' style='height:22px; width:150px; resize:none;'>"
                                        + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
                        sb.Append("<td><input type='text' name='txtMake' size='5px' class='bcAsptextbox' onchange='FillItemsAll(" + sno +
                            ")' id='txtMake" + sno + "' value='" + ds.Tables[0].Rows[i]["Make"].ToString() + "' style='width:50px;'/></td>");
                        sb.Append("<td><input type='text' name='txtQuantity' readonly='true' size='05px' onchange='FillItemsAll(" + sno +
                            ")' id='txtQuantity" + sno + "' value='" + ds.Tables[0].Rows[i]["Quantity"].ToString() +
                            "' onkeypress='return isNumberKey(event)' onblur='AmountCalulate(" + sno + "); FillItemsAll(" + sno +
                            ");' maxlength='6' style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");

                        if (ds.Tables[0].Rows[i]["UnitName"].ToString() == "")
                        {
                            sb.Append("<td width='20px'><select id='ddlUnits" + sno + "' class='PayUnitCode' onchange='FillItemsAll(" + sno +
                                ")'>");
                            if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow dru in dsEnm.Tables[0].Rows)
                                    sb.Append("<option value='" + dru["ID"].ToString() + "' >" + dru["Description"].ToString() + "</option>");
                            }
                            sb.Append("</select></td>");
                        }
                        else
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");
                        sb.Append("<td><input type='text' size='05px' name='txtPrice' id='txtPrice" + sno
                            + "' onfocus='this.select()' onMouseUp='return false' value='" +
                            ds.Tables[0].Rows[i]["Rate"].ToString() + "'  onchange='FillItemsAll(" + sno +
                            ")' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' " +
                            "onkeypress='return blockNonNumbers(this, event, true, false);' FillItemsAll(" + sno +
                            ");' maxlength='18' style='text-align: right; width:50px;' class='bcAsptextbox'/></td>");
                        sb.Append("<td align='right'><span id='spnAmount" + sno + "'>"
                            + Math.Round((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                            Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())), 2).ToString()
                        + "</span></td>");
                        sb.Append("<td><input type='text' size='5px' name='txtDiscount' id='txtDiscount" + sno
                            + "' onfocus='this.select()' onMouseUp='return false' value='"
                            + ds.Tables[0].Rows[i]["DiscountPercentage"].ToString() +
                            "' onblur='extractNumber(this,2,false);' class='bcAsptextbox' onkeyup='CheckDiscount(" + sno +
                            "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, " +
                            " false);' onchange='FillItemsAll(" + sno + ")' maxlength='18' style='text-align: right; width:50px;'/>%</td>");
                        if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                            sb.Append("<th align='right'> &nbsp;</th>");
                        else
                        {
                            sb.Append("<td><input type='text' size='05px' id='txtPercent" + sno
                                + "' onfocus='this.select()' onMouseUp='return false' value='" + Convert.ToString(ds.Tables[0].Rows[i]["ExDutyPercentage"]) + "' onchange='FillItemsAll(" + sno +
                                ")' onblur='extractNumber(this,2,false);' class='bcAsptextbox' onkeyup='CheckExDuty(" + sno +
                                "); extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event," +
                                " true, false);' maxlength='18' style='text-align: right; width: 50px;'/>%</td>");
                        }
                        if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0)
                            FlagExDuty = 1;
                        if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0)
                            FlagDiscount = 1;

                        sb.Append("<td align='right'>" + Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["QPrice"].ToString()), 2)
                        + "&nbsp;</td>");
                        if (ds.Tables[0].Rows[i]["GrandTotal"].ToString() != "")
                            sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>" +
                                Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[i]["GrandTotal"].ToString()), 2) + "</span></td>");
                        else
                            sb.Append("<td align='right'><span id='spnGrandTotal" + sno + "'>0.00</span></td>");
                        sb.Append("<td><input type='text' name='txtRemarks' size='10px' onchange='FillItemsAll(" + sno + ")' id='txtRemarks"
                            + sno + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString()
                            + "' style='width:60px;' class='bcAsptextbox'/></td>");
                        sb.Append("<td></td>");
                        sb.Append("<td></td>");
                        sb.Append("</tr>");
                    }

                    #region Sub Items

                    DataTable dt_S = (DataTable)Session["AllLQSubItems"];
                    if (dt_S != null && dt_S.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt_S.Rows.Count; j++)
                        {
                            if (Convert.ToDecimal(dt_S.Rows[j]["ExDutyPercentage"].ToString()) > 0)
                                FlagExDuty = 1;
                            if (Convert.ToDecimal(dt_S.Rows[j]["DiscountPercentage"].ToString()) > 0)
                                FlagDiscount = 1;
                            if (Convert.ToBoolean(dt_S.Rows[j]["Check"]))
                            {
                                TotalAmount += Convert.ToDecimal(dt_S.Rows[j]["Amount"]);
                                GrandTotal += Convert.ToDecimal(dt_S.Rows[j]["TotalAmt"]);
                            }
                        }
                    }

                    #endregion

                    sb.Append("<tfoot>");
                    sb.Append("<tr class='bcGridViewHeaderStyle'>");
                    sb.Append("<th colspan='5' class='rounded-foot-left'><b><span></span></b></th>");
                    sb.Append("<th align='right'> <input type='hidden' name='HFExDuty' id='HFExDuty' value='" + FlagExDuty + "'/> </th>");
                    sb.Append("<th align='right'> <input type='hidden' name='HFDiscount' id='HFDiscount' value='" + FlagDiscount + "'/> </th>");
                    sb.Append("<th colspan='4' align='right'><b><span>Total Amount(" + PriceSymbol + "): <label id='lblTotalAmt'>"
                            + Math.Round(Convert.ToDecimal(TotalAmount), 2) + "</label></span></b></th>");
                    sb.Append("<th colspan='4' align='right'><b><span>Grand Total(" + PriceSymbol + "): <label id='lblGTAmt'> "
                        + Math.Round(Convert.ToDecimal(GrandTotal), 2) + "</label></span></b></th>");
                    sb.Append("<th colspan='4' class='rounded-foot-right'><b><span></span></b></th>");
                    sb.Append("</tr></tfoot>");
                }
                sb.Append("</tbody></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string FillGrid_SupItems(string ParentItemID, string TRid, bool IsAdd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                FlagDiscount = 0; FlagExDuty = 0;
                bool DscntPrcnt = true, PkngPrcnt = true, ExsePrcnt = true, ChkExduty = true;
                string RowID = TRid;
                double DiscountPercentAmt = 0, PackingPercentAmt = 0, ChkExdutyAmt = 0;
                double LableID = Convert.ToDouble(TRid);
                DataTable dt = (DataTable)Session["AllLQSubItems"];
                if (dt == null)
                    dt = CommonBLL.EmptyDtLQ_SubItems();

                DataRow[] rslt = dt.Select("ParentItemId = '" + ParentItemID + "'");
                if (rslt.Length == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["ItemId"] = Guid.Empty;
                    dr["ParentItemId"] = ParentItemID;
                    dr["Quantity"] = 0;
                    dr["Rate"] = 0.00;
                    dr["QPrice"] = 0.00;
                    dr["Amount"] = 0.00;
                    dr["ExDutyPercentage"] = 0.1;
                    dr["DiscountPercentage"] = 0.00;
                    dr["UNumsId"] = Guid.Empty;
                    dr["Check"] = true;
                    dt.Rows.Add(dr);
                    rslt = dt.Select("ParentItemId = '" + ParentItemID + "'");
                }
                if (rslt != null && rslt.Length > 0)
                {
                    double TotalAmount = 0;
                    double GrandTotal = 0;
                    double examt = 0;
                    for (int i = 0; i < rslt.Length; i++)
                    {
                        double total = 0;
                        if (Convert.ToBoolean(rslt[i]["Check"]))
                        {
                            string SNo = (i + 1).ToString();
                            RowID = (TRid + "a" + (i + 1));
                            LableID = (LableID + 0.1);

                            sb.Append("<tr id='" + RowID + "' class='DEL" + TRid + " RA'>");
                            sb.Append("<td ></td>");
                            sb.Append("<td ></td>");
                            sb.Append("<td ><input type ='hidden' id='hfSubItemID" + RowID + "' value='" + rslt[i]["ItemId"].ToString() + "'>" +
                                "<lable id='lblSubSNo" + RowID + "'>" + LableID + "</td>");
                            rslt[i]["SNo"] = LableID;

                            sb.Append("<td ><lable id='lblItemDesc" + RowID + "'>" + rslt[i]["ItemDesc"].ToString() + "");
                            if (i == rslt.Length - 1)
                            {
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' "
                                        + " class='icons fnOpen'  ID='btnShow" + RowID + "'  title='Add Item to Item Master' onclick='fnOpen_AddSubItems(" + SNo
                                        + ")' ><img src='../images/add.jpeg'/></a></span>");
                            }
                            sb.Append("</td>");

                            sb.Append("<td ><lable id='lblPartNo" + RowID + "'>" + rslt[i]["PartNumber"].ToString() + "</td>");
                            sb.Append("<td ><textarea id='txtDesc-Spec" + RowID + "' class='bcAsptextboxmulti' onchange='savechanges1(" + TRid + "," + SNo
                                + ",this); ' onMouseUp='return false' " + " onfocus='ExpandTXTt(" + SNo + "); this.select()' onblur='ReSizeTXTt(" + SNo
                                + ")' style='height:20px; width:150px; resize:none;' >" + rslt[i]["Specifications"].ToString() + "" + "</textarea></td>");

                            sb.Append("<td ><input type='text' id='txtMake" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' class='bcAsptextbox' style='width:50px;' value='" + rslt[i]["Make"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                            sb.Append("<td ><input type='text' id='txtQuantity" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this); ' class='bcAsptextbox' style='width:50px;text-align: right;' value='" + rslt[i]["Quantity"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");

                            #region Fill Units Dropdown
                            sb.Append("<td align='right'>");
                            sb.Append("<select id='ddlUnits" + RowID + "' class='bcAspdropdown' style='width:85px;' onchange='savechanges1(" + TRid + "," + SNo + ",this);'>");

                            //if ((!UnitCodes.ContainsKey(Convert.ToInt32(dt.Rows[i]["SNo"]))))
                            sb.Append("<option value='0'>-SELECT-</option>");
                            if (Session["ItemUnits"] == null)
                                Session["ItemUnits"] = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

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

                            sb.Append("<td align='right'><input type='text' id='txtRate" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this); QtyCheck(" + TRid + "," + SNo + ");' class='bcAsptextbox' style='width:50px;text-align: right;' value='" + rslt[i]["Rate"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                            sb.Append("<td align='right'><lable id='lblAmount" + RowID + "'>" + rslt[i]["Amount"].ToString() + "</td>");
                            sb.Append("<td align='center'><input type='text' id='txtDscnt" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' dir='rtl' class='bcAsptextbox' style='width:50px; float:none;' value='" + rslt[i]["DiscountPercentage"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/>%</td>");
                            sb.Append("<td align='center'><input type='text' id='txtExDuty" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' dir='rtl' class='bcAsptextbox' style='width:50px; float:none;' value='" + rslt[i]["ExDutyPercentage"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/>%</td>");
                            sb.Append("<td align='right'><lable id='lblNetRate" + RowID + "'>" + rslt[i]["QPrice"].ToString() + "</td>");

                            if (rslt[i]["TotalAmt"].ToString() != "")
                                sb.Append("<td align='right'><span id='spnGrandTotal" + RowID + "'>"
                                    + Math.Round(Convert.ToDecimal(rslt[i]["TotalAmt"].ToString()), 2) + "</span></td>");
                            else
                                sb.Append("<td align='right'><span id='spnGrandTotal" + RowID + "'>0.00</span></td>");


                            if (Convert.ToDecimal(rslt[i]["ExDutyPercentage"].ToString()) > 0)
                                FlagExDuty = 1;
                            if (Convert.ToDecimal(rslt[i]["DiscountPercentage"].ToString()) > 0)
                                FlagDiscount = 1;

                            sb.Append("<td>");
                            sb.Append("<input type='text' name='txtRemarks' onchange='savechanges1(" + TRid + "," + SNo + ",this);' id='txtRemarks" + RowID
                                + "' value='" + rslt[i]["Remarks"].ToString() + "' style='width:60px' class='bcAsptextbox'/>");
                            if ((rslt.Length - 1) == i)
                            {
                                DataSet ds = (DataSet)Session["FloatEnquiry"];
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    TotalAmount += Convert.ToDouble(ds.Tables[0].Compute("Sum(Amount)", "Check = 1"));
                                    GrandTotal += Convert.ToDouble(ds.Tables[0].Compute("Sum(GrandTotal)", "Check = 1"));
                                }

                                TotalAmount += Convert.ToDouble(dt.Compute("Sum(Amount)", "Check = 1"));
                                GrandTotal += Convert.ToDouble(dt.Compute("Sum(TotalAmt)", "Check = 1"));

                                sb.Append("<input type='hidden' name='HFExDuty_S' id='HFExDuty_S' value='" + FlagExDuty + "'/>");
                                sb.Append("<input type='hidden' name='HFDiscount_S' id='HFDiscount_S' value='" + FlagDiscount + "'/>");

                                sb.Append("<input type='hidden' name='HFTotalAmt_S' id='HFTotalAmt_S' value='" + TotalAmount + "'/>");
                                sb.Append("<input type='hidden' name='HFGTAmt_S' id='HFGTAmt_S' value='" + GrandTotal + "'/>");
                            }
                            sb.Append("</td>");

                            #region Calculations

                            if (Convert.ToBoolean(rslt[i]["Check"]))
                            {
                                //txtHeadingNumbers
                                //if (rslt[i]["HSCode"].ToString() != "")
                                //    HSCode += ", " + rslt[i]["HSCode"].ToString();
                                //else
                                //    HSCode += dt.Rows[i]["HSCode"].ToString();

                                total = Convert.ToDouble(rslt[i]["TotalAmt"].ToString());// *Convert.ToDouble(dt.Rows[i]["Rate"].ToString());
                                //GrandTotal += total;
                                //sb.Append("<td align='right'>" + total.ToString("N") + "</td>");


                                double examt1 = 0;
                                double Rate = Convert.ToDouble(rslt[i]["Rate"].ToString());
                                TotalAmount += Rate;

                                //Discount Percentage Calculation
                                double DiscountPercentage = Convert.ToDouble(rslt[i]["DiscountPercentage"].ToString()) > 0
                                    ? Convert.ToDouble(rslt[i]["DiscountPercentage"].ToString()) : DiscountPercentAmt;
                                examt1 = Rate - (DiscountPercentage * Rate) / 100;

                                //Packing Percentage Calculation
                                //double PackingPercentage = Convert.ToDouble(rslt[i]["PackingPercentage"].ToString()) > 0
                                //    ? Convert.ToDouble(rslt[i]["PackingPercentage"].ToString()) : PackingPercentAmt;
                                //examt1 = examt1 + (PackingPercentage * examt1) / 100;

                                examt1 = Convert.ToDouble(rslt[i]["Quantity"].ToString()) * examt1;
                                if (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString()) > 0 || (ChkExduty == true && ChkExdutyAmt > 0))
                                {
                                    double exdutyamount = (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString())) > 0
                                        ? (Convert.ToDouble(rslt[i]["ExDutyPercentage"].ToString())) : ChkExdutyAmt;
                                    examt += (examt1 * exdutyamount / 100);
                                }
                            }

                            //sb.Append("<td align='right'>" + total.ToString("N"));
                            //if (i == (rslt.Length - 1))
                            //{
                            //    sb.Append("<input type ='hidden' id='hfGT_sub' value='" + GrandTotal + "'>");
                            //    sb.Append("<input type ='hidden' id='hfRateT_sub' value='" + TotalRate + "'>");
                            //    sb.Append("<input type ='hidden' id='hfExDtT_sub' value='" + examt + "'>");

                            //    for (int J = 0; J < dt.Rows.Count; J++)
                            //    {
                            //        if (Convert.ToBoolean(dt.Rows[J]["Check"]))
                            //        {
                            //            if (dt.Rows[J]["DiscountPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["DiscountPercentage"].ToString()) != 0)
                            //                DscntPrcnt = false;
                            //            //if (dt.Rows[J]["PackingPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["PackingPercentage"].ToString()) != 0)
                            //            //    PkngPrcnt = false;
                            //            if (dt.Rows[J]["ExDutyPercentage"].ToString() != "" && Convert.ToDecimal(dt.Rows[J]["ExDutyPercentage"].ToString()) != 0)
                            //                ExsePrcnt = false;
                            //        }
                            //    }
                            //    sb.Append("<input type='hidden' ID='hdfDscntAll_Sub' value='" + DscntPrcnt + "' />"
                            //        + " <input type='hidden' ID='hdfPkngAll_Sub' value='" + PkngPrcnt + "' />"
                            //        + " <input type='hidden' ID='hdfExseAll_Sub' value='" + ExsePrcnt + "' />");
                            //}
                            //sb.Append("</td>");
                            #endregion

                            #region Buttons

                            sb.Append("<td valign='top'>");
                            if (rslt.Length - 1 == i)
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' " +
                                    " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td valign='top'>" +
                                    "<a href='javascript:void(0)' onclick='Add_Sub_Itms(" + SNo + ")' " +
                                    " class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></td>");
                            else
                                sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' " +
                                    " onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' class='icons deleteicon' " +
                                    " title='Delete' OnClientClick='javascript:return doConfirm();'>" +
                                    " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td>");
                            sb.Append("</td>");
                            #endregion
                            sb.Append("</tr>");
                        }
                    }
                }
                dt.AcceptChanges();
                Session["AllSubItems"] = dt;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New LQ", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        private void AddColumns_SubItems(Boolean IsEdit)
        {
            try
            {
                DataTable dt = (DataTable)Session["AllLQSubItems"];
                if (dt == null)
                    dt = CommonBLL.EmptyDtLQ_SubItems();
                if (!dt.Columns.Contains("Check"))
                {
                    DataColumn DCCheck = new DataColumn("Check", typeof(bool));
                    DCCheck.DefaultValue = IsEdit;
                    dt.Columns.Add(DCCheck);
                }
                if (!dt.Columns.Contains("TotalAmt"))
                {
                    DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                    DCTotalAmt.DefaultValue = 0;
                    dt.Columns.Add(DCTotalAmt);
                }
                //if (IsEdit)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //        dt.Rows[i]["TotalAmt"] = Convert.ToDecimal(dt.Rows[i]["Rate"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                //}
                dt.AcceptChanges();
                Session["AllLQSubItems"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New LQ", ex.Message.ToString());
            }
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
                        lblED.Visible = chkExdt.Visible = chkSGST.Visible = chkIGST.Visible = false;
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowChkExDuty", "CHeck('chkExdt','dvExdt');", false);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
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
                        string strPath = MapPath("~/uploads/");
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["NLQUplds"] != null)
                        {
                            alist = (ArrayList)Session["NLQUplds"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["NLQUplds"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["NLQUplds"] = alist;
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
                if (Session["NLQUplds"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["NLQUplds"];
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

        /// <summary>
        /// Send E-Mails After Generation of Quotation 
        /// </summary>
        protected void SendDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                if (ToAdrsTbl != null && ToAdrsTbl.Tables.Count > 1)
                {
                    string ToAddrs = ToAdrsTbl.Tables[0].Rows[0]["PriEmail"].ToString();
                    string CcAddrs = Session["UserMail"].ToString();

                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtls(ToAdrsTbl.Tables[1].Rows[0]["LEIssueDate"].ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New LQ", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Send E-Mails After Generation of Quotation 
        /// </summary>
        protected void SendDefaultMails(DataSet ToAdrsTbl, string UpdateMails)
        {
            try
            {
                if (ToAdrsTbl != null && ToAdrsTbl.Tables.Count > 1)
                {
                    string ToAddrs = ToAdrsTbl.Tables[0].Rows[0]["PriEmail"].ToString();
                    string CcAddrs = Session["UserMail"].ToString();

                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtlsUpdatted(ToAdrsTbl.Tables[1].Rows[0]["LEIssueDate"].ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about Quotation Details
        /// </summary>
        /// <returns></returns>
        protected string InformationEnqDtls(string LEDAte)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Quotaton is Prepared for your Enquiry " + System.Environment.NewLine + System.Environment.NewLine);

                sb.Append(" We have an prepared quotation for your Enquiry No. " + ddllenq.SelectedItem.Text + " Dt: " + Convert.ToDateTime(LEDAte).ToString("dd-MM-yyyy") + ". ");
                sb.Append("Please find the Quotation in VOMS Application for the complete details and send your response. ");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                //sb.Append(System.Environment.NewLine + "Admin ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// E-Mail Body Format for Information about Quotation Details
        /// </summary>
        /// <returns></returns>
        protected string InformationEnqDtlsUpdatted(string LEDAte)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Quotaton is Updated for your Enquiry " + System.Environment.NewLine + System.Environment.NewLine);

                sb.Append(" We have updated quotation for your Enquiry No. " + ddllenq.SelectedItem.Text + " Dt: " + Convert.ToDateTime(LEDAte).ToString("dd-MM-yyyy") + ". ");
                sb.Append("Please find the Quotation in VOMS Application for the complete details and send your response. ");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                //sb.Append(System.Environment.NewLine + "Admin ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
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

        #endregion

        # region WebMethods

        #region MyRegion

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ValidateRowsBeforeSave()
        {
            try
            {
                string Error = "";
                DataSet ds = (DataSet)Session["FloatEnquiry"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dr = ds.Tables[0].Select("Check = 'true' and Rate = '0'");
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
        public string NextPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage"] = (CPage + 1);
            Session["RowsDisplay"] = txtRowsChanged;
            return FillGridView(Guid.Empty, 0);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PrevPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage"] = (CPage - 1);
            Session["RowsDisplay"] = txtRowsChanged;
            return FillGridView(Guid.Empty, 0);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string RowsChanged(string CurrentPage, string txtRowsChanged)
        {
            if (txtRowsChanged.Trim() != "")
            {
                if (Convert.ToInt32(Session["RowsDisplay"].ToString()) != Convert.ToInt32(txtRowsChanged))
                {
                    int RowStart = ((Convert.ToInt32(Session["RowsDisplay"].ToString()) * Convert.ToInt32(CurrentPage)) - Convert.ToInt32(Session["RowsDisplay"].ToString())) + 1;

                    if (RowStart > Convert.ToInt32(txtRowsChanged))
                        Session["CPage"] = RowStart / Convert.ToInt32(txtRowsChanged) + 1;
                    else if (RowStart < Convert.ToInt32(txtRowsChanged) && RowStart != 1)
                        Session["CPage"] = (Convert.ToInt32(txtRowsChanged) + 1) / RowStart;
                }
                Session["RowsDisplay"] = Convert.ToInt32(txtRowsChanged);
            }
            return FillGridView(Guid.Empty, 0);
        }

        #endregion


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
            string Qty, string PartNo, double Rate, double Amount, int CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Session["RowsDisplay"] = txtRowsChanged;
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                if (CodeID_G != Guid.Empty)
                {
                    dt.Rows[rowNo]["Category"] = CatID_G;
                    dt.Rows[rowNo]["ItemDescription"] = CodeID_G;
                    dt.Rows[rowNo]["PartNo"] = PartNo;
                    dt.Rows[rowNo]["Specification"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                        dt.Rows[rowNo]["Quantity"] = 0;
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["units"] = UnitID_G;
                    dt.Rows[rowNo]["Rate"] = Rate;
                    dt.Rows[rowNo]["Amount"] = Amount;
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
                    if (dt.Rows.Count < ds.Tables[0].Rows.Count && (Codes.Count <= dt.Rows.Count))
                    {
                        if (CodeID_G != Guid.Empty && dt.Rows[dt.Rows.Count - 1]["ItemDescription"].ToString() != "0")
                        {
                            DataRow dr = dt.NewRow();
                            dr["SNo"] = dt.Rows.Count + 1;
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
                            dt.Rows.Add(dr);
                        }
                        if (ID == 1)
                        {
                            if (dt.Rows[dt.Rows.Count - 1]["ItemDescription"].ToString() == string.Empty)
                            {
                                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                            }
                        }
                    }
                }
                # endregion
                HttpContext.Current.Session["ItemCode"] = dt;
                sb.Append(FillGridView(new Guid(HttpContext.Current.Session["EnqId"].ToString()), 0));
                sb.Append("</tbody></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddNewRow(int RNo, string sv, string PrtNo, string spec, string Make, string Qty, string Units,
            double Rate, double Amount, string Rmrks, int CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Session["RowsDisplay"] = txtRowsChanged;
                DataSet ds = new DataSet();
                DataSet dsEnm =
                ds = (DataSet)Session["FloatEnquiry"];
                if (sv != "")
                    ds.Tables[0].Rows[RNo - 1]["ItemId"] = sv;
                if (PrtNo != "")
                    ds.Tables[0].Rows[RNo - 1]["PartNumber"] = PrtNo;
                ds.Tables[0].Rows[RNo - 1]["Specifications"] = spec;
                ds.Tables[0].Rows[RNo - 1]["Make"] = Make;
                ds.Tables[0].Rows[RNo - 1]["Quantity"] = Qty;
                if (Units != "")
                {
                    ds.Tables[0].Rows[RNo - 1]["UNumsId"] = Units;
                    DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + Units.ToString());
                    ds.Tables[0].Rows[RNo - 1]["UnitName"] = selRow[0]["Description"].ToString();
                }
                ds.Tables[0].Rows[RNo - 1]["Rate"] = Convert.ToDouble(Rate);
                ds.Tables[0].Rows[RNo - 1]["Amount"] = Convert.ToDouble(Amount);
                ds.Tables[0].Rows[RNo - 1]["Remarks"] = Rmrks;
                if (sv == "" && Units == "")
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    ds.Tables[0].Rows.Add(dr);
                    ds.Tables[0].Rows[RNo]["Amount"] = "0.00";
                    ds.Tables[0].Rows[RNo]["Rate"] = "0.00";
                    ds.Tables[0].Rows[RNo]["QPrice"] = "0.00";
                    ds.Tables[0].Rows[RNo]["GrandTotal"] = "0.00";
                    ds.Tables[0].Rows[RNo]["Quantity"] = "0";
                    ds.Tables[0].Rows[RNo]["ExDutyPercentage"] = "0.1";
                    ds.Tables[0].Rows[RNo]["DiscountPercentage"] = "0.00";
                    ds.Tables[0].Rows[RNo]["Check"] = true;
                }
                ds.AcceptChanges();
                Session["FloatEnquiry"] = ds;
                return FillGridView(Guid.Empty, 0);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }

        /// <summary>
        /// Delete Item Table Item
        /// </summary>
        /// <param name="rowNo">Selected Row</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItem(int rowNo, int CurrentPage)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Dictionary<int, int> Codes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedItems"];
                string ItemsDetailsID = "";
                int res = 0;
                DataSet ds = new DataSet();
                ds = (DataSet)Session["FloatEnquiry"];
                ItemsDetailsID = ds.Tables[0].Rows[rowNo - 1]["ItemDetailsIdU"].ToString();
                if ((res == 0 && ItemsDetailsID != "") || ItemsDetailsID == "")
                {
                    ds.Tables[0].Rows[rowNo - 1].Delete();
                    ds.Tables[0].AcceptChanges();
                    if (Codes.ContainsKey(rowNo - 1))
                        Codes.Remove(rowNo - 1);
                }
                Session["SelectedItems"] = Codes;
                return FillGridView(Guid.Empty, 0);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
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
        public string AddItemColumn(int rowNo, int SNo, int CodeID, int CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Session["RowsDisplay"] = txtRowsChanged;
                DataTable dtbl = (DataTable)HttpContext.Current.Session["ItemCode"];
                int SLNo = dtbl.Rows.Count;
                DataTable dt = dtbl.Clone();
                DataRow dr = dt.NewRow();
                dr["SNo"] = SLNo;
                dr["Category"] = 0;
                dr["ItemDescription"] = 0;
                dr["PartNo"] = string.Empty;
                dr["Specification"] = string.Empty;
                dr["Make"] = string.Empty;
                dr["Quantity"] = 0;
                dr["Units"] = 0;
                dr["Rate"] = 0.00;
                dr["Amount"] = 0.00;
                dr["ID"] = 0;
                dt.Rows.Add(dr);


                Dictionary<int, int> Codes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedItems"];
                Dictionary<int, int> CatCodes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedCat"];
                Dictionary<int, int> UnitCodes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedUnits"];

                HttpContext.Current.Session["SelectedCat"] = CatCodes;
                HttpContext.Current.Session["SelectedItems"] = Codes;
                HttpContext.Current.Session["SelectedUnits"] = UnitCodes;
                HttpContext.Current.Session["ItemCode"] = dt;
                int count = dt.Rows.Count;
                return FillItemGrid(rowNo, SLNo, CodeID.ToString(), 1, "0", "0", "", "", "", "", 0, 0, CurrentPage, txtRowsChanged);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }

        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillItemDRP(int rowNo, int sv, int CurrentPage)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                DataSet ds = new DataSet();
                DataSet dss = new DataSet();
                ds = (DataSet)Session["FloatEnquiry"];
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] selRow = dss.Tables[0].Select("ID = " + sv.ToString());
                    ds.Tables[0].Rows[rowNo - 1]["ItemId"] = selRow[0]["ID"].ToString();
                    ds.Tables[0].Rows[rowNo - 1]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();
                }
                return FillGridView(Guid.Empty, 0);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillUnitDRP(int rowNo, int sv, string PrtNo, string spec, string Make, string Qty, string Units,
            double Rate, double Amount, string Rmrks)
        {
            try
            {
                DataSet ds = new DataSet();
                DataSet dsEnm = new DataSet();
                ds = (DataSet)Session["FloatEnquiry"];
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
                return FillGridView(Guid.Empty, 0);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(int rowNo, int SNo, string CodeID, int ID, string CatID, string Spec, string Make, string Qty,
            double Rate, double Amount, string Rmrks, double Discount, string UnitID, double ExDuty, double CHKExDuty, double CHKDiscount,
            double CHKSalesTax, double CHKPacking, double CHKAdChrgs, bool ChkChaild, int CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Session["RowsDisplay"] = txtRowsChanged;
                DataSet dsEnm = new DataSet();
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                DataSet ds = (DataSet)Session["FloatEnquiry"];
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
                        dt.Rows[rowNo]["Check"] = true;

                    if (!Codes.ContainsValue(CodeID_G) && CodeID_G != Guid.Empty)
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

                    ds.Tables[0].Rows[rowNo]["QPrice"] = AmtPcknAdsn;
                    ds.Tables[0].Rows[rowNo]["GrandTotal"] = (AmtPcknAdsn * Convert.ToDouble(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()));
                    ds.Tables[0].Rows[rowNo]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Rate"].ToString()) *
                        Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()), 2));
                    Session["FloatEnquiry"] = ds;
                    return FillGridView(Guid.Empty, 0);
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SaveChangesWhileRateChange(int rowNo, int SNo, string CodeID, int ID, string CatID, string Spec, string Make, string Qty,
            double Rate, double Amount, string Rmrks, double Discount, string UnitID, double ExDuty, double CHKExDuty, double CHKDiscount,
            double CHKSalesTax, double CHKPacking, double CHKAdChrgs, bool ChkChaild, int CurrentPage, int txtRowsChanged)
        {
            try
            {
                Session["CPage"] = CurrentPage;
                Session["RowsDisplay"] = txtRowsChanged;
                DataSet dsEnm = new DataSet();
                Guid CatID_G = Guid.Empty, CodeID_G = Guid.Empty, UnitID_G = Guid.Empty;
                CatID_G = ((CatID != "" && CatID != "0") ? new Guid(CatID) : Guid.Empty);
                CodeID_G = ((CodeID != "" && CodeID != "0") ? new Guid(CodeID) : Guid.Empty);
                UnitID_G = ((UnitID != "" && UnitID != "0") ? new Guid(UnitID) : Guid.Empty);
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                DataSet ds = (DataSet)Session["FloatEnquiry"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    double PackageAmt = 0;
                    double AmtAfterDiscount = 0;
                    double AdsnlChargesAmt = 0;
                    double AmtPcknAdsn = 0;
                    DataTable dt = ds.Tables[0];

                    CatID_G = Guid.Empty;   //GeneralCtgryID;
                    if (!ChkChaild)
                        dt.Rows[rowNo]["Check"] = false;
                    else
                        dt.Rows[rowNo]["Check"] = true;

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
                    ds.Tables[0].Rows[rowNo]["QPrice"] = String.Format("{0:0.00}", Rate);
                    ds.Tables[0].Rows[rowNo]["GrandTotal"] = String.Format("{0:0.00}", Math.Round((Convert.ToDouble(Rate) * Convert.ToDouble(ds.Tables[0].Rows[rowNo]["Quantity"].ToString())), 2));
                    ds.Tables[0].Rows[rowNo]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Rate"].ToString()) *
                        Convert.ToDecimal(ds.Tables[0].Rows[rowNo]["Quantity"].ToString()), 2));
                    Session["FloatEnquiry"] = ds;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return false;
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
                DataSet ds = (DataSet)Session["FloatEnquiry"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal DiscountAmount = 0;
                    decimal Amount = 0, GTotalAmount = 0;
                    decimal PackageAmt = 0, PackageAmt1 = 0;
                    decimal SalesTaxAmt = 0;
                    decimal ExAmt = 0, ExAmt1 = 0;
                    decimal SGSTAmt = 0, SGSTAmt1 = 0;
                    decimal IGSTAmt = 0, IGSTAmt1 = 0;
                    decimal AdsnlChargesAmt = 0, AdsnlChargesAmt1 = 0;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Amount = 0; GTotalAmount = 0;
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
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) - ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                }
                                else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) -
                                        ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                   Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString()));
                                }
                            }
                            else
                            {
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount = ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]) - DiscountAmount;
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) - ((Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) * Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString())) *
                                        (Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                                    GTotalAmount = (Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                   Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString()));
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
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    if (Amount == 0)
                                        Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                                    ExAmt = (Amount * (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                    ExAmt1 = (GTotalAmount * (Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()))) / 100;
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
                                    SGSTAmt = 0; SGSTAmt1 = 0;
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
                                ds.Tables[0].Rows[i]["QPrice"] = Amount;
                                ds.Tables[0].Rows[i]["GrandTotal"] = (GTotalAmount);
                                ds.Tables[0].Rows[i]["Amount"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                            }
                            else
                            {
                                Amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString());
                                if (FlagExDuty == 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                    {
                                        ExAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                        ExAmt1 = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    }
                                    else if (Convert.ToInt32(rdbExDty) == 0)
                                    { ExAmt = 0; ExAmt1 = 0; }

                                    if (chkSGST != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbSGST) == 0)
                                    {
                                        SGSTAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                        SGSTAmt1 = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    }
                                    else if (Convert.ToInt32(rdbSGST) == 0)
                                    { SGSTAmt = 0; SGSTAmt1 = 0; }

                                    if (chkIGST != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbIGST) == 0)
                                    {
                                        IGSTAmt = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                        IGSTAmt1 = (Amount * Convert.ToDecimal(ExDuty)) / 100;
                                    }
                                    else if (Convert.ToInt32(rdbIGST) == 0)
                                    { IGSTAmt = 0; IGSTAmt1 = 0; }
                                }
                                Amount = Amount + ExAmt + SGSTAmt + IGSTAmt;
                                GTotalAmount = GTotalAmount + ExAmt1 + SGSTAmt1 + IGSTAmt1;
                                ds.Tables[0].Rows[i]["QPrice"] = Amount;
                                ds.Tables[0].Rows[i]["GrandTotal"] = (GTotalAmount);
                                ds.Tables[0].Rows[i]["Amount"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString());
                                Amount = 0;
                            }
                        }
                    }

                    ds.AcceptChanges();
                    Session["FloatEnquiry"] = ds;
                }

                #endregion

                #region Sub Items

                DataTable dss = (DataTable)Session["AllLQSubItems"];
                if (dss != null && dss.Rows.Count > 0)
                {
                    decimal DiscountAmount_S = 0;
                    decimal Amount_S = 0;
                    decimal PackageAmt_S = 0;
                    decimal SalesTaxAmt_S = 0;
                    decimal ExAmt_S = 0;
                    decimal AdsnlChargesAmt_S = 0;
                    for (int i = 0; i < dss.Rows.Count; i++)
                    {
                        Amount_S = 0;
                        if (Convert.ToBoolean(dss.Rows[i]["Check"].ToString()))
                        {
                            //Discount Calculation (Price - Discount)
                            if (FlagDiscount == 0)
                            {
                                if (Convert.ToDecimal(dss.Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount_S = ((Convert.ToDecimal(dss.Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(dss.Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"]) - DiscountAmount_S;
                                }
                                else if (chkDsnt != false && Convert.ToDecimal(Discount) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount_S = ((Convert.ToDecimal(dss.Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(Discount))) / 100;
                                    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"]) - DiscountAmount_S;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"]);
                            }
                            else
                            {
                                if (Convert.ToDecimal(dss.Rows[i]["DiscountPercentage"].ToString()) > 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    DiscountAmount_S = ((Convert.ToDecimal(dss.Rows[i]["Rate"].ToString())) *
                                        (Convert.ToDecimal(dss.Rows[i]["DiscountPercentage"].ToString()))) / 100;
                                    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"]) - DiscountAmount_S;
                                }
                                else if (Convert.ToInt32(rdbDscnt) == 0)
                                    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"]);
                            }


                            //Packing Calculation ((Price - Discount)+Packing amount)
                            if (chkPkng != false && Convert.ToDecimal(Packing) > 0 && Convert.ToInt32(rdbPkg) == 0)
                            {
                                PackageAmt_S = (Amount_S * Convert.ToDecimal(Packing)) / 100;
                                //Amount = Amount + PackageAmt;
                            }
                            if (chkAdsnlChrgs != false && Convert.ToDecimal(AdsnlChrgs) > 0 && Convert.ToInt32(rdbAdd) == 0)
                            {
                                AdsnlChargesAmt_S = (Amount_S * Convert.ToDecimal(AdsnlChrgs)) / 100;
                                //Amount = Amount + AdsnlChargesAmt;
                            }
                            Amount_S = Amount_S + PackageAmt_S + AdsnlChargesAmt_S;
                            //Excise Duty Calculation (((Price - Discount)+Packing amount) + Excise Duty )
                            if (FlagExDuty == 0)
                            {
                                if (Convert.ToDecimal(dss.Rows[i]["ExDutyPercentage"].ToString()) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    if (Amount_S == 0)
                                        Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString());
                                    ExAmt_S = (Amount_S * (Convert.ToDecimal(dss.Rows[i]["ExDutyPercentage"].ToString()))) / 100;

                                    //ExAmt_S = (Amount_S * (Convert.ToDecimal(dss.Rows[i]["ExDutyPercentage"].ToString()))) / 100;
                                }
                                else if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                {
                                    //if (Amount_S == 0)
                                    //    Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString());
                                    ExAmt_S = (Amount_S * Convert.ToDecimal(ExDuty)) / 100;
                                }
                                else if (Convert.ToInt32(rdbAdd) == 0)
                                    ExAmt_S = 0;
                            }
                            else
                                Amount_S = Convert.ToDecimal(Amount_S + ((Amount_S * Convert.ToDecimal(dss.Rows[i]["ExDutyPercentage"].ToString())) / 100));

                            //Sales Tax Calculation(((Price - Discount)+Packing amount) + Sales Tax )
                            //if (chkSltx != false && Convert.ToDecimal(SalesTax) > 0)
                            //    SalesTaxAmt = (Amount * Convert.ToDecimal(SalesTax)) / 100;
                            Amount_S = Amount_S + ExAmt_S; // +SalesTaxAmt;

                            //dss.Rows[i]["QPrice"] = Amount_S;
                            //dss.Rows[i]["TotalAmt"] = (Amount_S * Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString()));
                            //dss.Rows[i]["Amount"] = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString()) *
                            //    Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString());

                            if (Amount_S > 0)
                            {
                                dss.Rows[i]["QPrice"] = Amount_S;
                                dss.Rows[i]["TotalAmt"] = (Amount_S * Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString()));
                                dss.Rows[i]["Amount"] = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString());
                            }
                            else
                            {
                                Amount_S = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString());
                                if (FlagExDuty == 0 && Convert.ToInt32(rdbDscnt) == 0)
                                {
                                    if (chkExdt != false && Convert.ToDecimal(ExDuty) > 0 && Convert.ToInt32(rdbExDty) == 0)
                                        ExAmt_S = (Amount_S * Convert.ToDecimal(ExDuty)) / 100;
                                    else if (Convert.ToInt32(rdbExDty) == 0)
                                        ExAmt_S = 0;
                                }
                                Amount_S = Amount_S + ExAmt_S;
                                dss.Rows[i]["QPrice"] = Amount_S;
                                dss.Rows[i]["TotalAmt"] = (Amount_S * Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString()));
                                dss.Rows[i]["Amount"] = Convert.ToDecimal(dss.Rows[i]["Rate"].ToString()) *
                                    Convert.ToDecimal(dss.Rows[i]["Quantity"].ToString());
                                Amount_S = 0;
                            }
                        }
                    }
                    dss.AcceptChanges();
                    Session["AllLQSubItems"] = dss;
                }

                #endregion

                if (Convert.ToInt32(rdbDscnt) == 1)
                {
                    decimal GT = 0;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        GT = Convert.ToDecimal(ds.Tables[0].Compute("sum(GrandTotal)", "Check=1"));

                    if (dss != null && dss.Rows.Count > 0)
                        GT += Convert.ToDecimal(dss.Compute("sum(TotalAmt)", "Check=1"));

                    decimal Dscunt = 0, PackageAmnt = 0, ExAmnt = 0, AdsnlChargesAmnt = 0, DiscuntGT = 0;

                    if (Convert.ToInt32(rdbDscnt) == 1 && Convert.ToDecimal(Discount) > 0)
                        Dscunt = Convert.ToDecimal(Discount);

                    DiscuntGT = GT - Dscunt;
                    if (Convert.ToInt32(rdbExDty) == 0 && Convert.ToDecimal(ExDuty) > 0)
                    {
                        ExAmnt = ((DiscuntGT) * (Convert.ToDecimal(ExDuty))) / 100;
                    }
                    if (Convert.ToInt32(rdbPkg) == 0 && Convert.ToDecimal(Packing) > 0)
                    {
                        if (ExAmnt > 0)
                            PackageAmnt = ((DiscuntGT + ExAmnt) * (Convert.ToDecimal(Packing))) / 100;
                        else
                            PackageAmnt = ((DiscuntGT) * (Convert.ToDecimal(Packing))) / 100;
                    }
                    if (Convert.ToInt32(rdbAdd) == 0 && Convert.ToDecimal(AdsnlChrgs) > 0)
                    {
                        if (ExAmnt > 0)
                            AdsnlChargesAmnt = ((DiscuntGT + ExAmnt) * (Convert.ToDecimal(AdsnlChrgs))) / 100;
                        else
                            AdsnlChargesAmnt = ((DiscuntGT) * (Convert.ToDecimal(AdsnlChrgs))) / 100;
                    }
                    Gtotal = (DiscuntGT + ExAmnt + PackageAmnt + AdsnlChargesAmnt);
                }

                return FillGridView(Guid.Empty, Gtotal);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
                return FillGridView(Guid.Empty, 0);
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckEnquiryNo(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckLQEnquiryNo('Q', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
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
                ArrayList all = (ArrayList)Session["NLQUplds"];
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

        #region Sub Items

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Add_Sub_Itms(string RowID, string TRid, string SubRowID, string ParentItemId, string ItemId,
            string ItmDesc, string Spec, string PNo, string Make, string Qty, string Rate, string Disc, string exduty, string UnitID, string Remarks,
            string DiscPercent, string PkngPercent, string ExDutyPercent,
            string DscntAll, string PkngAll, string ExseAll, string AddChrgsAll, string ChkDsnt, string ChkPkng, string ChkExse, string ChkAddChrgs,
            string IsAdd)
        {
            try
            {
                decimal PackageAmt = 0;
                decimal AmtAfterDiscount = 0;
                decimal AdsnlChargesAmt = 0;
                decimal AmtPcknAdsn = 0;

                decimal Discount = 0, Rate_C = 0;
                if (Disc != "")
                    Discount = Convert.ToDecimal(Disc);
                if (Rate != "")
                    Rate_C = Convert.ToDecimal(Rate);

                decimal DscntAll_C = Convert.ToDecimal(DscntAll), PkngAll_C = Convert.ToDecimal(PkngAll),
                        ExseAll_C = Convert.ToDecimal(ExseAll), AddChrgsAll_C = Convert.ToDecimal(AddChrgsAll);

                DataTable dt = (DataTable)Session["AllLQSubItems"];
                if (dt != null && ItemId.Trim() != "")
                {
                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)Session["SelectedItems"];
                    if (Codes == null)
                        Codes = new Dictionary<int, Guid>();
                    Guid SelItemID = new Guid(ItemId);
                    if (Codes == null || !Codes.ContainsValue(SelItemID))
                    {
                        Codes.Add(Convert.ToInt32((Codes.Last().Key) + 1), SelItemID);
                        Session["SelectedItems"] = Codes;
                    }

                    DataRow[] result = dt.Select("SNo = '" + SubRowID + "'");
                    if (result.Length > 0)
                    {
                        if (ParentItemId.Trim() != "" && ParentItemId.Trim() != "0")
                            result[0]["ParentItemId"] = new Guid(ParentItemId);
                        if (ItemId.Trim() != "" && ItemId.Trim() != Guid.Empty.ToString())
                            result[0]["ItemId"] = new Guid(ItemId);
                        result[0]["ItemDesc"] = ItmDesc.Trim();
                        result[0]["Make"] = Make.Trim();
                        if (Qty.Trim() != "")
                            result[0]["Quantity"] = Convert.ToDecimal(Qty);
                        if (Rate.Trim() != "" && Rate.Trim() != "0")
                            result[0]["Rate"] = Convert.ToDecimal(Rate);
                        if (result[0]["Quantity"].ToString() != "" && result[0]["Rate"].ToString() != "")
                            result[0]["Amount"] = Convert.ToDecimal(Rate) * Convert.ToDecimal(Qty);
                        if (UnitID.Trim() != "" && UnitID.Trim() != "0" && UnitID.Trim() != Guid.Empty.ToString())
                            result[0]["UnumsID"] = new Guid(UnitID);
                        result[0]["Remarks"] = Remarks;
                        if (Disc.Trim() != "")
                            result[0]["DiscountPercentage"] = Discount;


                        result[0]["ExDutyPercentage"] = ExDutyPercent;
                        if (Discount > 0)
                            AmtAfterDiscount = Rate_C - ((Rate_C * Discount) / 100);
                        else if (DscntAll_C > 0)
                            AmtAfterDiscount = Rate_C - ((Rate_C * DscntAll_C) / 100);
                        else
                            AmtAfterDiscount = Rate_C;

                        if (PkngAll_C > 0)
                            PackageAmt = (AmtAfterDiscount * PkngAll_C) / 100;

                        if (AddChrgsAll_C > 0)
                            AdsnlChargesAmt = (AmtAfterDiscount * AddChrgsAll_C) / 100;

                        AmtPcknAdsn = AmtAfterDiscount + PackageAmt + AdsnlChargesAmt;

                        if (ExseAll_C > 0)
                            AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExseAll_C) / 100);
                        else if (ExseAll_C > 0)
                            AmtPcknAdsn = AmtPcknAdsn + ((AmtPcknAdsn * ExseAll_C) / 100);


                        result[0]["QPrice"] = AmtPcknAdsn;
                        result[0]["TotalAmt"] = (AmtPcknAdsn * Convert.ToDecimal(result[0]["Quantity"].ToString()));
                        result[0]["Amount"] = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(result[0]["Rate"].ToString()) *
                            Convert.ToDecimal(result[0]["Quantity"].ToString()), 2));


                        //if (exduty.Trim() != "")
                        //    result[0]["ExDutyPercentage"] = Convert.ToInt64(exduty);
                        if (Convert.ToBoolean(IsAdd))
                        {
                            DataRow dr = dt.NewRow();
                            dr["SNo"] = (Convert.ToDouble(SubRowID) + 0.1);
                            dr["ItemId"] = Guid.Empty;
                            dr["ParentItemId"] = ParentItemId;
                            dr["Quantity"] = 0;
                            dr["Rate"] = 0.00;
                            dr["QPrice"] = 0.00;
                            dr["Amount"] = 0.00;
                            dr["ExDutyPercentage"] = 0.1;
                            dr["DiscountPercentage"] = 0.00;
                            dr["UNumsId"] = Guid.Empty;
                            dr["Check"] = true;
                            dt.Rows.Add(dr);
                        }
                    }
                    dt.AcceptChanges();
                    Session["AllLQSubItems"] = dt;
                }
                else
                    dt = CommonBLL.CT1ItemsTable_SubItems();

                return FillGrid_SupItems(ParentItemId, TRid, Convert.ToBoolean(IsAdd));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Expand_LPOs(string ParentItemId, string TRid)
        {
            try
            {
                AddColumns_SubItems(false);
                return FillGrid_SupItems(ParentItemId, TRid, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetItemDesc_Spec(string ParentItemId, string TRid)
        {
            try
            {
                string desc = "", Spec = "", PNo = "";
                DataSet ds = ItemMstBLl.SelectItemMaster(CommonBLL.FlagASelect, new Guid(ParentItemId.Trim()), Guid.Empty, new Guid(Session["CompanyID"].ToString()));//, CommonBLL.SupplierCategory
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    desc = ds.Tables[0].Rows[0]["ItemDescription"].ToString();
                    Spec = ds.Tables[0].Rows[0]["Specification"].ToString();
                    PNo = ds.Tables[0].Rows[0]["PartNumber"].ToString();
                }
                return desc + "^~^," + Spec + "^~^," + PNo;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Delete_SubItem(string ParentItemId, string TRid, string SNo, string ItemID)
        {
            try
            {
                string subItmID = TRid + "." + SNo;
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)Session["SelectedItems"];
                if (Codes == null)
                    Codes = new Dictionary<int, Guid>();

                if (ItemID != "" && Codes != null && Codes.ContainsValue(new Guid(ItemID)))
                {
                    int KeyVal = Codes.FirstOrDefault(x => x.Value == new Guid(ItemID)).Key;
                    Codes.Remove(KeyVal);
                    Session["SelectedItems"] = Codes;
                }

                DataTable dt = (DataTable)Session["AllLQSubItems"];
                DataRow[] result = dt.Select("SNo = '" + subItmID + "'");
                if (result.Length > 0)
                    result[0].Delete();

                dt.AcceptChanges();
                Session["AllLQSubItems"] = dt;
                return FillGrid_SupItems(ParentItemId, TRid, false);
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
                DataTable dt = (DataTable)Session["AllLQSubItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dt.Rows[i]["Check"].ToString()))
                        {
                            RowID = dt.Rows[i]["SNo"].ToString();
                            if (dt.Rows[i]["ItemId"].ToString() != Guid.Empty.ToString() && dt.Rows[i]["Rate"].ToString() != "0" && Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) > 0)
                            {
                                if (dt.Rows[i]["ItemId"].ToString() == "" || dt.Rows[i]["ItemId"].ToString() == Guid.Empty.ToString())
                                    ErrorMsg += "ItemID";
                                if (dt.Rows[i]["Rate"].ToString() == "" || dt.Rows[i]["Rate"].ToString() == "0")
                                    ErrorMsg += "," + "Rate";
                                if (Convert.ToDecimal(dt.Rows[i]["Quantity"].ToString()) <= 0)
                                    ErrorMsg += "," + "Quantity";
                                if (Convert.ToDecimal(dt.Rows[i]["ExDutyPercentage"].ToString()) < 0)
                                    ErrorMsg += "," + "ExDutyPercentage";
                                if (Convert.ToDecimal(dt.Rows[i]["DiscountPercentage"].ToString()) < 0)
                                    ErrorMsg += "," + "DiscountPercentage";
                                if (dt.Rows[i]["UNumsId"].ToString() == "" || dt.Rows[i]["UNumsId"].ToString() == "0"
                                                                           || dt.Rows[i]["UNumsId"].ToString() == Guid.Empty.ToString())
                                    ErrorMsg += "," + "Units";
                                if (ErrorMsg != "")
                                    break;
                            }
                        }
                    }
                }
                if (ErrorMsg != "")
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

        #endregion

        #endregion

        # region Payment WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PaymentDeleteItem(int rowNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["PaymentTerms"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[rowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["AmountLQ"] = dt.Compute("Sum(PaymentPercentage)", "");
                Session["PaymentTerms"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
            return FillPaymentTerms();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PaymentAddItem(int rowNo, string Pay, string Desc)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["PaymentTerms"];
                int count = dt.Rows.Count;
                int PmntPercent = 0;
                double Payy = Convert.ToDouble(Pay);
                object OldAmt = dt.Rows[rowNo - 1]["PaymentPercentage"];
                PmntPercent = Convert.ToInt32(dt.Compute("Sum(PaymentPercentage)", ""));
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
                    Session["MessageLQ"] = "";
                    Session["AmountLQ"] = PmntPercent.ToString();
                }
                else
                    Session["MessageLQ"] = "Percentage Cannot Exceed 100";
                Session["PaymentTerms"] = dt;
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

        #region DropDownList Selected Index Changed Events
        /// <summary>
        /// Customer DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string CstmrVal = ddlcustomer.SelectedValue;
                ClearAll();
                ddlcustomer.SelectedValue = CstmrVal;
                BindDropDownList(ddlfenq, NEBL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "",
                    DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Frn Enquiry DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenq_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string CstmrVal = ddlcustomer.SelectedValue;
                string FenqVal = ddlfenq.SelectedValue;
                ClearAll();
                ddlcustomer.SelectedValue = CstmrVal;
                ddlfenq.SelectedValue = FenqVal;
                DataTable dt = CommonBLL.EmptyDtLocal();
                BindDropDownList(ddllenq, NLEBL.SelctLocalEnquiries(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(ddlcustomer.SelectedValue),
                    new Guid(ddlfenq.SelectedValue), "", "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 60, "", "",
                    "", Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), dt));
                divLQItems.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Lcl Enquiry DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddllenq_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string CstmrVal = ddlcustomer.SelectedValue;
                string FenqVal = ddlfenq.SelectedValue;
                string lclval = ddllenq.SelectedValue;
                ClearAll();
                ddlcustomer.SelectedValue = CstmrVal;
                ddlfenq.SelectedValue = FenqVal;
                ddllenq.SelectedValue = lclval;
                divLQItems.InnerHtml = EditRecord(new Guid(ddllenq.SelectedValue));
                //HideUnwantedFields();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Clear Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
                Response.Redirect("../Quotations/NewLQuotation.Aspx", false);
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Save/Update Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveQuote();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Local Quotation", ex.Message.ToString());
            }
        }

        #endregion
    }
}
