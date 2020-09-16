using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Text;
using System.Collections;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Accounts
{
    public partial class BillPaymentApproval : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        ContactBLL CBL = new ContactBLL();
        InvoiceBLL INVBLL = new InvoiceBLL();
        SupplierBLL SPBLL = new SupplierBLL();
        CustomerBLL cusmr = new CustomerBLL();
        BillPaymentApprovalBLL BPABLL = new BillPaymentApprovalBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        //static long EditID = 0;
        static string ReadOnly = "";
        #endregion

        #region Default Page Load Event

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
                        Session["ReadOnly"] = "";
                        Ajax.Utility.RegisterTypeForAjax(typeof(BillPaymentApproval));
                        if (!IsPostBack)
                        {
                            LblPaymntAmt.Text = "Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                            LblBAmount.Text = "Balance Amount(" + Session["CurrencySymbol"].ToString().Trim() + "):";
                            ClearControls(false);
                            txtDate.Attributes.Add("readonly", "readonly");
                            txtRefNo.Attributes.Add("readonly", "readonly");
                            txtAmount.Attributes.Add("readonly", "readonly");
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            btnReject.Attributes.Add("OnClick", "Javascript:return MyRejectvalidations()");
                            BindDropDownList(ddlSupplier, SPBLL.SelectSuppliers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                            if (Session["TeamName"].ToString().ToLower() == "accounts" || Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                                pnlCheque.Visible = true;
                            else
                                pnlCheque.Visible = false;

                            #region Add/Update Permission Code
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                                Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                    EditRecord(new Guid(Request.QueryString["ID"]));
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Enabled = true;
                                btnSave.Text = "Save";
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no", false);
                            # endregion
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
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
                        if (Session["BPAUplds"] != null && Session["BPAUplds"].ToString() != "")
                        {
                            alist = (ArrayList)Session["BPAUplds"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["BPAUplds"] == null || Session["BPAUplds"].ToString() == "")
                            alist.Add(FileNames);
                        Session["BPAUplds"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('File Size is more than 25MB, Resize and Try Again');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
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
                if (Session["BPAUplds"] != null && Session["BPAUplds"].ToString() != "")
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["BPAUplds"];
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                    for (int k = 0; k < all.Count; k++)
                        sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                    sb.Append("</select>");
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ex.Message;
            }
        }

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
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        private void EditRecord(Guid ApprovalID)
        {
            try
            {
                DataSet EditDs = BPABLL.GetDataSet(CommonBLL.FlagModify, ApprovalID, Guid.Empty, "", "", new Guid(Session["CompanyID"].ToString()));
                if (EditDs != null && EditDs.Tables.Count > 2)
                {
                    string ChequeNo = EditDs.Tables[0].Rows[0]["ChequeNo"].ToString();
                    string ChequeDate = EditDs.Tables[0].Rows[0]["ChequeDate"].ToString();
                    string BankName = EditDs.Tables[0].Rows[0]["BankName"].ToString();
                    string UTRNo = EditDs.Tables[0].Rows[0]["UTRNo"].ToString();
                    string ChequeAmount = EditDs.Tables[0].Rows[0]["ChequeAmount"].ToString();
                    ViewState["CreatedBy"] = EditDs.Tables[0].Rows[0]["CreatedBy"].ToString();

                    if (Session["TeamName"].ToString().ToLower() == "accounts user team" || Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole
                        || Session["AccessRole"].ToString() == CommonBLL.AdminRole || Session["AccessRole"].ToString() == CommonBLL.AccountsContactTypeText) // Accounts and Admin and Superadmin
                    {
                        if (ChequeNo != "" && ChequeDate != "" && BankName != "" && ChequeAmount != "" && Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
                            Session["ReadOnly"] = " readonly='readonly' ";
                        else if (Session["AccessRole"].ToString() != CommonBLL.AdminRole) //&& Session["AccessRole"].ToString().ToLower() == "accounts user team")
                            Session["ReadOnly"] = " readonly='readonly' ";
                        else if (Session["AccessRole"].ToString() == CommonBLL.AccountsContactTypeText)
                            Session["ReadOnly"] = " readonly='readonly' ";
                        if (String.IsNullOrEmpty(ChequeNo))
                        {
                            btnReject.Visible = true;
                            RejectDtls.Visible = true;
                            pnlCheque.Visible = true;
                        }
                    }
                    else if (Session["TeamName"].ToString() != "Purchase I" || Session["TeamName"].ToString() != "Purchase II"
                        || Session["TeamName"].ToString() != "PURCHASE I" || Session["TeamName"].ToString() != "PURCHASE II"
                        || Session["TeamName"].ToString().ToLower() == "accounts user team") // Purchase
                    {
                        if (ChequeNo != "" && ChequeDate != "" && BankName != "" && ChequeAmount != "")
                        {
                            Session["ReadOnly"] = " readonly='readonly' ";
                            txtChequeNo.Enabled = false;
                            txtDate.Enabled = false;
                            txtBankName.Enabled = false;
                            txtUTRNo.Enabled = false;
                            txtAmount.Enabled = false;
                            pnlCheque.Visible = true;
                        }
                    }
                    else
                    {
                        Session["ReadOnly"] = " readonly='readonly' ";
                        txtChequeNo.Enabled = false;
                        txtDate.Enabled = false;
                        txtBankName.Enabled = false;
                        txtUTRNo.Enabled = false;
                        txtAmount.Enabled = false;
                        pnlCheque.Visible = false;
                    }

                    txtChequeNo.Text = ChequeNo.Trim() == "" ? "" : ChequeNo;
                    txtDate.Text = ChequeDate == "" ? "" : CommonBLL.DateDisplay(Convert.ToDateTime(ChequeDate));
                    txtBankName.Text = BankName == "" ? "" : BankName;
                    txtUTRNo.Text = UTRNo == "" ? "" : UTRNo;
                    txtAmount.Text = ChequeAmount == "" ? "" : ChequeAmount;

                    ddlSupplier.SelectedValue = EditDs.Tables[0].Rows[0]["SupplierID"].ToString();
                    Session["LPOrderIDs"] = BPABLL.GetDataSet(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlSupplier.SelectedValue), "", "", new Guid(Session["CompanyID"].ToString()));
                    Session["GridOne"] = EditDs.Tables[1];
                    Session["GridTwo"] = EditDs.Tables[2];
                    string INV = EditDs.Tables[0].Rows[0]["INV_LPOIDs"].ToString();
                    string PO = EditDs.Tables[0].Rows[0]["PO_LPOIDs"].ToString();
                    SelectedLPOs(INV, PO);

                    Session["EditID"] = new Guid(EditDs.Tables[0].Rows[0]["ApprovalID"].ToString());
                    txtRefNo.Text = EditDs.Tables[0].Rows[0]["RefNo"].ToString();

                    divGridOne.InnerHtml = GridOne(false);
                    DivGridTwo.InnerHtml = GridTwo(false);

                    txtProInfNo.Text = EditDs.Tables[0].Rows[0]["ProInvNo"].ToString();
                    txtProInvNoDate.Text = Convert.ToDateTime(EditDs.Tables[0].Rows[0]["ProInvNoDate"].ToString()) == CommonBLL.EndDate ? "" : CommonBLL.DateDisplay(Convert.ToDateTime(EditDs.Tables[0].Rows[0]["ProInvNoDate"].ToString()));
                    txtExportInvNo.Text = EditDs.Tables[0].Rows[0]["ExportInvNo"].ToString();
                    txtExportInvDate.Text = Convert.ToDateTime(EditDs.Tables[0].Rows[0]["ExportInvNoDate"].ToString()) == CommonBLL.EndDate ? "" : CommonBLL.DateDisplay(Convert.ToDateTime(EditDs.Tables[0].Rows[0]["ExportInvNoDate"].ToString()));
                    txtFormCorHNo.Text = EditDs.Tables[0].Rows[0]["FormCorHNo"].ToString();
                    txtFormCorHNoDate.Text = Convert.ToDateTime(EditDs.Tables[0].Rows[0]["FormCorHNoDate"].ToString()) == CommonBLL.EndDate ? "" : CommonBLL.DateDisplay(Convert.ToDateTime(EditDs.Tables[0].Rows[0]["FormCorHNoDate"].ToString()));

                    HFINV_LPOIDs.Value = INV;
                    HFPO_LPOIDs.Value = PO;
                    DivHistory.InnerHtml = GetLPOsHistory(INV, PO);
                    DivHistoryCheques.InnerHtml = GridhistoryCheques(INV, PO, true, false);


                    if (EditDs.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        string[] all = EditDs.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
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
                        Session["BPAUplds"] = attms;
                        divListBox.InnerHtml = sb.ToString();
                    }
                    else
                    {
                        divListBox.InnerHtml = "";
                        Session["BPAUplds"] = "";
                    }
                    ddlSupplier.Enabled = false;
                    btnSave.Text = "Update";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "GetBal(false);", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        protected void SendDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                string ToAddrs = "rajaprasadamuteam@yahoo.in";
                string CcAddrs = "";
                if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                    CcAddrs = "sprt.bvpl@yahoo.com, " + Session["UserMail"].ToString();
                else
                    CcAddrs = "sprt.bvpl@yahoo.com, " + Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), "Bill Payment Approval", InformationEnqDtls());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        protected string InformationEnqDtls()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                if (Session["EditID"] != null && Convert.ToInt32(Session["EditID"]) == 0)
                    sb.Append("SUB: Bill payment for approval has been applied " + System.Environment.NewLine + System.Environment.NewLine);
                else
                    sb.Append("SUB: Bill payment for approval has been Approved " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append(" for Reference No. " + txtRefNo.Text + ".");
                sb.Append("Please find the Bill payment in VOMS Application for the complete details.");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        protected void SendRejectDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                string ToAddrs = "rajaprasadamuteam@yahoo.in" + "," + ToAdrsTbl.Tables[0].Rows[0]["PriEmail"].ToString();
                string CcAddrs = "";
                if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                    CcAddrs = Session["UserMail"].ToString();
                else
                    CcAddrs = Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), "Bill Payment Approval has been Rejected", InformationRejectDtls());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        protected string InformationRejectDtls()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Bill Payment for Approval has been Rejected " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("Sorry for inconvenience, your Bill Payment for Approval Reference Number " + txtRefNo.Text + " has been Rejected due to following reasons.");
                sb.Append(txtRejectReasons.Text + System.Environment.NewLine);
                sb.Append("Please find the Bill Payment Reference Number " + txtRefNo.Text + " in VOMS Application for the complete details.");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        private string GetLPOsHistory(string INV_LPOIDs, string PO_LPOIDs)
        {
            try
            {
                string values = INV_LPOIDs + "," + PO_LPOIDs;
                DataSet ds = new DataSet();
                if (Session["EditID"] != null && new Guid(Session["EditID"].ToString()) == Guid.Empty)
                    ds = BPABLL.GetDataSet(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, values, values, new Guid(Session["CompanyID"].ToString()));
                else
                    ds = BPABLL.GetDataSet(CommonBLL.FlagCSelect, new Guid(Session["EditID"].ToString()), Guid.Empty, values, values, new Guid(Session["CompanyID"].ToString()));
                if (ds != null)
                {
                    DataTable dt = TableHistory();
                    DataRow dr;
                    int RowCount = 0;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RowCount = (RowCount + 1);
                            dr = dt.NewRow();
                            dr["SNo"] = RowCount;
                            dr["LPOID"] = ds.Tables[0].Rows[i]["LPOID"].ToString();
                            dr["LPONo"] = ds.Tables[0].Rows[i]["LPONo"].ToString();
                            dr["LPODT"] = ds.Tables[0].Rows[i]["LPODT"].ToString();
                            dr["LPOAmt"] = ds.Tables[0].Rows[i]["LPOAmt"].ToString();
                            dr["PrfInvNo"] = ds.Tables[0].Rows[i]["PrfInvNo"].ToString();
                            dr["PrfInvAmt"] = Convert.ToString(ds.Tables[0].Rows[i]["PrfInvAmt"]) == "" ? 0 : ds.Tables[0].Rows[i]["PrfInvAmt"];
                            dr["Date"] = ds.Tables[0].Rows[i]["PayDate"].ToString();
                            dr["Amount"] = ds.Tables[0].Rows[i]["PayAmt"].ToString();
                            dt.Rows.Add(dr);
                        }
                        dt.AcceptChanges();
                    }

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            RowCount = RowCount++;
                            dr = dt.NewRow();
                            dr["SNo"] = RowCount;
                            dr["LPOID"] = ds.Tables[1].Rows[i]["LPOID"].ToString();
                            dr["LPONo"] = ds.Tables[1].Rows[i]["LPONo"].ToString();
                            dr["LPODT"] = ds.Tables[1].Rows[i]["LPODT"].ToString();
                            dr["LPOAmt"] = ds.Tables[1].Rows[i]["LPOAmt"].ToString();
                            //dr["PrfInvNo"] = ds.Tables[1].Rows[i]["PrfInvNo"].ToString();
                            dr["Percent"] = ds.Tables[1].Rows[i]["AmtPercent"].ToString();
                            dr["Date"] = ds.Tables[1].Rows[i]["PayDate"].ToString();
                            dr["Amount"] = ds.Tables[1].Rows[i]["PayAmt"].ToString();
                            dt.Rows.Add(dr);
                        }
                        dt.AcceptChanges();
                    }

                    dt.AcceptChanges();
                    Session["History"] = dt;
                }
                return Gridhistory();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        private string GridOne(bool RowAdded)
        {
            try
            {
                decimal tamt = 0, LPOamt = 0;
                DataTable dt = (DataTable)Session["GridOne"];
                StringBuilder sb = new StringBuilder();
                if (!RowAdded)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOs' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt(" + Session["CurrencySymbol"].ToString().Trim() + ")</th><th>Proforma Inv No./Bill No.</th>" +
                        "<th>Proforma Inv Amount</th>" +
                        "<th>Date</th><th>Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")</th><th>Remarks</th><th></th><th class='rounded-Last'></th></tr>");
                    sb.Append("</thead><tbody>");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTable dtt = dt.Copy();
                    dtt = dtt.DefaultView.ToTable(true, "LPOID", "LPOAmt");
                    //HFLPOsHistorySum = dtt.
                    LPOamt = Convert.ToDecimal(dtt.Compute("sum(LPOAmt)", ""));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        tamt += Convert.ToDecimal(dt.Rows[i]["PayAmt"].ToString());
                        sb.Append("<tr>");
                        if (i == (dt.Rows.Count - 1))
                            sb.Append("<td>" + SNo + " <input type='hidden' id='HFINVAmt0' value='" + tamt + "' /> <input type='hidden' id='HFLPOamt0' value='" + LPOamt + "' /> </td>");
                        else
                            sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td id='tdd" + SNo + "'>" + dt.Rows[i]["LPONo"].ToString() + " <input type='hidden' id='HFLPOID" + SNo + "' value='" + dt.Rows[i]["LPOID"].ToString() + "' /></td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["LPODT"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PrfInvNo"].ToString() + " <input type='hidden' id='HFPRFMID" + SNo + "' value='' /></td>");
                        sb.Append("<td>" + Convert.ToString(dt.Rows[i]["PrfInvAmt"]) + "</td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["PayDate"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PayAmt"].ToString() + "<input type='hidden' id='HFLblAmt" + SNo + "' value='" + dt.Rows[i]["PayAmt"].ToString() + "' /> </td>");
                        sb.Append("<td>" + dt.Rows[i]["Remarks"].ToString() + "</td>");
                        sb.Append("<td>");
                        if (Session["ReadOnly"] == "")
                            sb.Append("<span class='gridactionicons'><a id='btnEdit" + SNo + "' href='javascript:void(0)' onclick='EditRow(this.id)' class='icons additionalrow' "
                                + " title='Edit' ><img src='../images/edit-icon.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        if (Session["ReadOnly"] == "")
                            sb.Append("<span class='gridactionicons'><a id='btnOneDelete" + SNo + "' href='javascript:void(0)' onclick='DeleteOneRow(" + SNo + ")' class='icons additionalrow' "
                                + " title='Delete' ><img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                }

                if (!RowAdded)
                {
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th class='rounded-foot-left'>");
                    sb.Append("<label id='lblEditID0' text=''></label>");
                    sb.Append("</th>");
                    sb.Append("<th align='left'>");
                    #region Fill LPO IDs

                    DataSet LPONos = (DataSet)Session["LPOrderIDs"];
                    sb.Append("<select id='ddlLPOs0' Class='bcAspdropdown' width='50px' onchange='GetProformaINV(true)'>");
                    sb.Append("<option value='00000000-0000-0000-0000-000000000000'>-SELECT-</option>");
                    if (LPONos != null && LPONos.Tables.Count > 0 && Session["ReadOnly"] == "")
                    {
                        Dictionary<int, string> dINV = (Dictionary<int, string>)Session["INVs"];
                        Dictionary<int, string> dPO = (Dictionary<int, string>)Session["POs"];
                        for (int i = 0; i < LPONos.Tables[0].Rows.Count; i++)
                        {
                            string val = LPONos.Tables[0].Rows[i]["ID"].ToString();
                            if ((dINV == null || dPO == null) || new Guid(Session["EditID"].ToString()) != Guid.Empty)
                                sb.Append("<option value='" + val + "'>"
                                    + LPONos.Tables[0].Rows[i]["Description"].ToString() + "</option>");
                            else if ((dINV == null || dPO == null) || (!dINV.ContainsValue(val) && !dPO.ContainsValue(val)))
                                sb.Append("<option value='" + val + "'>"
                                    + LPONos.Tables[0].Rows[i]["Description"].ToString() + "</option>");
                        }
                    }
                    sb.Append("</select>");

                    #endregion
                    sb.Append("</th>");

                    sb.Append("<th align='left'> <input type='text' id='txtLPODate0' class='bcAsptextbox' readonly='readonly' style='height:20px; width:100px; resize:none;'/> </th>");
                    sb.Append("<th align='left'> <input type='text' id='txtLPOAMT0' class='bcAsptextbox' readonly='readonly' style='text-align: right; width:80px;'/>");

                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtPrfInvNo0' class='bcAsptextbox' style='width:158px;' " + Session["ReadOnly"].ToString() + " />");
                    sb.Append("<input type='hidden' id='HFPrfInvID0' value='' />");
                    sb.Append("</th>");


                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtPrfInvAmt0' class='bcAsptextbox'  onblur='extractNumber(this,2,false);' "
                                + " onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' style='width:158px;' " + Session["ReadOnly"].ToString() + " />");
                    sb.Append("</th>");

                    sb.Append("<th align='left'> <input type='text' id='txtCrntDate0' class='bcAsptextbox DatePicker' readonly='readonly' style='height:20px; width:100px; resize:none;'/> </th>");
                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtAmt0' class='bcAsptextbox' style='text-align: right; width:80px;' onblur='extractNumber(this,2,false);' "
                                + " onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' onchange='CheckLPOAmount_INV();'  " + Session["ReadOnly"] + " />");
                    sb.Append("<input type='hidden' id='HFINVAmt0' value='" + tamt + "' />");
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtremarks0' text='' class='bcAsptextbox'  style='width:158px;' " + Session["ReadOnly"].ToString() + " /></th>");
                    sb.Append("<th>");
                    if (Session["ReadOnly"] == "")
                    {
                        sb.Append("<span class='gridactionicons'><a id='btnADD' href='javascript:void(0)' " +
                                                " onclick='AddNewRow()'class='icons additionalrow' title='ADD' >"
                                                + "<img src='../images/add-2-icon.png' style='border-style: none;'/></a></span>");

                        sb.Append("<span class='gridactionicons'><a id='btnUpdate' href='javascript:void(0)' " +
                            " onclick='UpdateRow()'class='icons additionalrow' title='Update' style='display:none;'>"
                            + "<img src='../images/Update.png' style='border-style: none;'/></a></span>");
                    }
                    sb.Append("</th>");
                    sb.Append("<th class='rounded-foot-right'>");
                    if (Session["ReadOnly"] == "")
                    {
                        sb.Append("<span class='gridactionicons'><a id='btnCancel' href='javascript:void(0)' " +
                                                " onclick='CancelEdit()'class='icons additionalrow' title='Cancel' >"
                                                + "<img src='../images/Stop-Normal-icon.png' style='border-style: none;'/></a></span>");
                    }
                    sb.Append("</th>");
                    sb.Append("</tr>");
                    sb.Append("</tfoot>");
                    sb.Append("</table>");
                }
                //}
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private string GridTwo(bool RowAdded)
        {
            try
            {
                decimal amt = 0, LPOamt = 0;
                DataTable dt = (DataTable)Session["GridTwo"];
                StringBuilder sb = new StringBuilder();
                if (!RowAdded)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOs1' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt(" + Session["CurrencySymbol"].ToString().Trim() + ")</th><th>LPO Amt(Excl GST)(" + Session["CurrencySymbol"].ToString().Trim() + ")</th><th>Proforma Inv No.</th><th>Amt %</th>" +
                        "<th>Date</th><th>Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")</th><th>Remarks</th><th></th><th class='rounded-Last'></th></tr>");
                    sb.Append("</thead><tbody>");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTable dtt = dt.Copy();
                    dtt = dtt.DefaultView.ToTable(true, "LPOID", "LPOAmt");
                    LPOamt = Convert.ToDecimal(dtt.Compute("sum(LPOAmt)", ""));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        amt += Convert.ToDecimal(dt.Rows[i]["PayAmt"].ToString());

                        sb.Append("<tr>");
                        if (i == (dt.Rows.Count - 1))
                            sb.Append("<td>" + SNo + " <input type='hidden' id='hfPOamt' value='" + amt + "' />  <input type='hidden' id='HFPOLPOamt' value='" + LPOamt + "'> </td>");
                        else
                            sb.Append("<td>" + SNo + " </td>");

                        sb.Append("<td id='tddPO" + SNo + "'>" + dt.Rows[i]["LPONo"].ToString() + " <input type='hidden' id='HFFLPOID" + SNo + "' value='" + dt.Rows[i]["LPOID"].ToString() + "' /></td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["LPODT"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt_GST"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PrfInvNo"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["AdvAmtPercnt"].ToString() + "</td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["PayDate"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PayAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Remarks"].ToString() + "</td>");
                        sb.Append("<td>");
                        if (Session["ReadOnly"] == "")
                            sb.Append("<span class='gridactionicons'><a id='btnEdit" + SNo + "' href='javascript:void(0)' onclick='EditRow1(this.id)' class='icons additionalrow' "
                                + " title='Edit' ><img src='../images/edit-icon.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("<td>");
                        if (Session["ReadOnly"] == "")
                            sb.Append("<span class='gridactionicons'><a id='btnTwoDelete" + SNo + "' href='javascript:void(0)' onclick='DeleteTwoRow(" + SNo + ")' class='icons additionalrow' "
                                + " title='Delete' ><img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                }

                if (!RowAdded)
                {
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th class='rounded-foot-left'>");
                    sb.Append("<label id='lblEditID' text=''></label>");
                    sb.Append("</th>");
                    sb.Append("<th align='left'>");
                    #region Fill LPO IDs

                    DataSet LPONos = (DataSet)Session["LPOrderIDs"];
                    sb.Append("<select id='ddlLPOs' Class='bcAspdropdown' width='50px' onchange='GetProformaINV1(true)'>");
                    sb.Append("<option value='" + Guid.Empty.ToString() + "'>-SELECT-</option>");
                    if (LPONos != null && LPONos.Tables.Count > 0 && Session["ReadOnly"] == "")
                    {
                        Dictionary<int, string> dINV = (Dictionary<int, string>)Session["INVs"];
                        Dictionary<int, string> dPO = (Dictionary<int, string>)Session["POs"];
                        for (int i = 0; i < LPONos.Tables[0].Rows.Count; i++)
                        {
                            string val = LPONos.Tables[0].Rows[i]["ID"].ToString();
                            if ((dINV == null || dPO == null) || (!dINV.ContainsValue(val) && !dPO.ContainsValue(val)))
                                sb.Append("<option value='" + val + "'>"
                                    + LPONos.Tables[0].Rows[i]["Description"].ToString() + "</option>");
                        }
                    }
                    sb.Append("</select>");

                    #endregion
                    sb.Append("</th>");

                    sb.Append("<th align='left'> <input type='text' id='txtLPODate' class='bcAsptextbox' readonly='readonly' style='height:20px; width:100px; resize:none;'/> </th>");
                    sb.Append("<th align='left'> <input type='text' id='txtLPOAMT' class='bcAsptextbox' readonly='readonly' style='text-align: right; width:80px;'/> ");
                    sb.Append("<th align='left'> <input type='text' id='txtLPOAMT_GST' class='bcAsptextbox' readonly='readonly' style='text-align: right; width:80px;'/> ");//Without GST Amount

                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtPrfInvNo1' class='bcAsptextbox' style='width:158px;' " + ReadOnly + " />");
                    //sb.Append("<input type='hidden' id='HFPrfInvID0' value='' />");
                    sb.Append("</th>");

                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtAmtPrcnt' class='bcAsptextbox' " + Session["ReadOnly"] + " style='text-align: right; width:60px;' onblur='extractNumber(this,0,false);' "
                                + " onkeyup='extractNumber(this,0,false); GetAmount();' onkeypress='return blockNonNumbers(this, event, true, false);' onchange='GetAmount();'/>");
                    sb.Append("</th>");

                    sb.Append("<th align='left'> <input type='text' id='txtCrntDate' class='bcAsptextbox DatePicker' readonly='readonly' style='height:20px; width:100px; resize:none;'/> </th>");
                    sb.Append("<th align='left'>");
                    sb.Append("<input type='text' id='txtAmt' class='bcAsptextbox' " + Session["ReadOnly"] + " style='text-align: right; width:80px;' onblur='extractNumber(this,2,false);' "
                                + " onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' onchange='CheckLPOAmount_PO();'/>");
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtremarks' text='' class='bcAsptextbox'  " + Session["ReadOnly"] + " /></th>");
                    sb.Append("<th>");
                    if (Session["ReadOnly"] == "")
                    {
                        sb.Append("<span class='gridactionicons'><a id='btnADD1' href='javascript:void(0)' " +
                                                " onclick='AddNewRow1()'class='icons additionalrow' title='ADD' >"
                                                + "<img src='../images/add-2-icon.png' style='border-style: none;'/></a></span>");

                        sb.Append("<span class='gridactionicons'><a id='btnUpdate1' href='javascript:void(0)' " +
                            " onclick='UpdateRow1()'class='icons additionalrow' title='Update' style='display:none;'>"
                            + "<img src='../images/Update.png' style='border-style: none;'/></a></span>");
                    }
                    sb.Append("</th>");
                    sb.Append("<th class='rounded-foot-right'>");
                    if (Session["ReadOnly"] == "")
                        sb.Append("<span class='gridactionicons'><a id='btnCancel1' href='javascript:void(0)' " +
                                                " onclick='CancelEdit1()'class='icons additionalrow' title='Cancel'>"
                                                + "<img src='../images/Stop-Normal-icon.png' style='border-style: none;'/></a></span>");
                    sb.Append("</th>");
                    sb.Append("</tr>");
                    sb.Append("</tfoot>");
                    sb.Append("</table>");
                }
                //}
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private DataTable TableHistory()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("SNo");
                dt.Columns.Add("LPOID");
                dt.Columns.Add("LPONo");
                dt.Columns.Add("LPODT");
                dt.Columns.Add("LPOAmt");
                dt.Columns.Add("PrfInvNo");
                dt.Columns.Add("PrfInvAmt");
                dt.Columns.Add("Percent");
                dt.Columns.Add("Date");
                dt.Columns.Add("Amount");

                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return null;
            }
        }

        private DataTable TableHistoryCheques()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("SNo");
                dt.Columns.Add("INV_LPOIDs");
                dt.Columns.Add("PO_LPOIDs");
                dt.Columns.Add("RefNo");
                dt.Columns.Add("ChequeNo");
                dt.Columns.Add("ChequeDate");
                dt.Columns.Add("BankName");
                dt.Columns.Add("ChequeAmount");
                dt.Columns.Add("UTRNo");
                dt.Columns.Add("RejactReasons");
                dt.Columns.Add("CancelCheque");

                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return null;
            }
        }

        private string Gridhistory()
        {
            try
            {
                DataTable dt = (DataTable)Session["History"];
                StringBuilder sb = new StringBuilder();
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOshistory' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt</th><th>Prf Inv No</th><th>Prf Inv Amt</th><th>Amt %</th>" +
                        "<th>Date</th><th>Amount</th><th class='rounded-Last'>INV/PO</th></tr>");
                    sb.Append("</thead><tbody>");
                    decimal total = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPONo"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPODT"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PrfInvNo"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PrfInvAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Percent"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Date"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Amount"].ToString() + "</td>");
                        if (dt.Rows[i]["Percent"].ToString() == "")
                            sb.Append("<td>INV</td>");
                        else
                            sb.Append("<td>PO</td>");
                        sb.Append("</tr>");
                        total += Convert.ToDecimal(dt.Rows[i]["Amount"].ToString());
                    }

                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th class='rounded-foot-left'>");
                    sb.Append("<label id='lblEditID' text=''></label>");
                    sb.Append("</th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'> </th>");
                    sb.Append("<th align='left'> </th>");
                    sb.Append("<th align='left'>Total : </th>");
                    sb.Append("<th align='left'><input type='hidden' id='HFAllLPOHistory' value='" + total + "' /> " + total + "  </th>");
                    sb.Append("<th class='rounded-foot-right'>  </th>");
                    sb.Append("</tr>");
                    sb.Append("</tfoot>");
                    sb.Append("</table>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private string GridhistoryCheques(string INV_LPOIDs, string PO_LPOIDs, bool IsDelete, bool IsAdded)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = (DataTable)Session["HistoryCheeques"];
                DataTable dthist = (DataTable)Session["History"];
                string Values = (INV_LPOIDs + "," + PO_LPOIDs).Trim(',');
                if (dt == null || dt.Rows.Count == 0)
                    dt = TableHistoryCheques();
                ds = new DataSet();
                ds.Tables.Add(TableHistoryCheques());
                DataSet TempDS = null;
                string[] dINV = Values.Trim(',').Split(',').Select(sValue => sValue.Trim()).Distinct().ToArray();
                string[] dPO = Values.Trim(',').Split(',').Select(sValue => sValue.Trim()).Distinct().ToArray();
                if (dINV[0] != Guid.Empty.ToString())
                {
                    for (int k = 0; k < dINV.Length; k++)
                    {
                        TempDS = BPABLL.GetDataSet(CommonBLL.FlagESelect, new Guid(Session["EditID"].ToString()), Guid.Empty, dINV[k].ToString(), dPO[k].ToString(), new Guid(Session["CompanyID"].ToString()));
                        if (TempDS != null && TempDS.Tables.Count > 0)
                        {
                            foreach (DataRow dr in TempDS.Tables[0].Rows)
                                ds.Tables[0].Rows.Add(dr.ItemArray);
                            ds.AcceptChanges();
                        }
                    }
                }
                if (dPO[0] != Guid.Empty.ToString())
                {
                    for (int l = 0; l < dPO.Length; l++)
                    {
                        TempDS = BPABLL.GetDataSet(CommonBLL.FlagESelect, new Guid(Session["EditID"].ToString()), Guid.Empty, dINV[l].ToString(), dPO[l].ToString(), new Guid(Session["CompanyID"].ToString()));
                        if (TempDS != null && TempDS.Tables.Count > 0)
                        {
                            foreach (DataRow dr in TempDS.Tables[0].Rows)
                                ds.Tables[0].Rows.Add(dr.ItemArray);
                            ds.AcceptChanges();
                        }
                    }
                }
                if ((ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0))
                {
                    DataRow dr = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string ChequeNo = ds.Tables[0].Rows[i]["ChequeNo"].ToString().ToLower();
                        DataRow[] rslt = null;
                        if (dt != null && dt.Rows.Count > 0)
                            rslt = dt.Select("ChequeNo = '" + ChequeNo + "'");
                        else if (dt == null)
                            dt = TableHistoryCheques();

                        if ((rslt == null || rslt.Length == 0 || rslt[0]["ChequeNo"].ToString().ToLower() != ChequeNo) && ChequeNo != "")
                        {

                            string[] CancleSt = ds.Tables[0].Rows[i]["RejactReasons"].ToString().Split(new[] { "~#" }, StringSplitOptions.None);
                            string cStatus = ""; decimal cAmount = 0;
                            if (CancleSt.Length > 1 && CancleSt[2].ToString() == "2")
                            {
                                cStatus = "/ <font color='Red'>Cancelled</font>";
                                cAmount = Convert.ToDecimal(CancleSt[1].ToString());
                            }
                            dr = dt.NewRow();
                            dr["SNo"] = (dt.Rows.Count + 1);
                            dr["INV_LPOIDs"] = INV_LPOIDs.Trim();
                            dr["PO_LPOIDs"] = PO_LPOIDs.Trim();
                            dr["RefNo"] = ds.Tables[0].Rows[i]["RefNo"].ToString();
                            dr["ChequeNo"] = ds.Tables[0].Rows[i]["ChequeNo"].ToString();
                            dr["ChequeDate"] = CommonBLL.DateDisplay(Convert.ToDateTime(ds.Tables[0].Rows[i]["ChequeDate"].ToString()));
                            dr["BankName"] = ds.Tables[0].Rows[i]["BankName"].ToString();
                            dr["UTRNo"] = ds.Tables[0].Rows[i]["UTRNo"].ToString();
                            dr["ChequeAmount"] = ds.Tables[0].Rows[i]["ChequeAmount"].ToString();
                            dr["CancelCheque"] = cStatus;
                            dt.Rows.Add(dr);
                        }
                    }
                    dt.AcceptChanges();
                    if (IsAdded)
                        Session["HistoryCheeques"] = dt;
                }

                StringBuilder sb = new StringBuilder();
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOshistoryCheques' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>Reference No.</th><th>Cheque No.</th><th>Cheque Date</th><th>Bank Name</th><th>UTR No.</th><th class='rounded-Last'>Amount</th></tr>");
                    sb.Append("</thead><tbody>");
                    decimal total = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["RefNo"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["ChequeNo"].ToString() + dt.Rows[i]["CancelCheque"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["ChequeDate"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["BankName"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["UTRNo"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["ChequeAmount"].ToString() + "</td>");
                        sb.Append("</tr>");
                        if (String.IsNullOrEmpty(dt.Rows[i]["CancelCheque"].ToString()))
                        {
                            total += Convert.ToDecimal(dt.Rows[i]["ChequeAmount"].ToString());
                        }

                    }

                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th class='rounded-foot-left'>");
                    sb.Append("<label id='lblEditID' text=''></label>");
                    sb.Append("</th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'> <input type='hidden' id='HFChequeAmount' value='" + total + "' /> Total :</th>");
                    sb.Append("<th class='rounded-foot-right'> " + total + " </th>");
                    sb.Append("</tr>");
                    sb.Append("</tfoot>");
                    sb.Append("</table>");
                }
                else
                {
                    DataTable dth = (DataTable)Session["History"];
                    if (dth != null && dth.Rows.Count > 0 && (dt == null || dt.Rows.Count == 0))
                    {
                        string LPOIDS = string.Join(",", dth.AsEnumerable().Select(x => x.Field<string>("LPOID")));
                        DataSet dss = BPABLL.GetDataSet(CommonBLL.FlagJSelect, new Guid(Session["EditID"].ToString()), Guid.Empty, LPOIDS, LPOIDS, new Guid(Session["CompanyID"].ToString()));
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                        {
                            string refNos = string.Join(",", dss.Tables[0].AsEnumerable().Select(x => x.Field<string>("RefNo")));
                            sb.Append("<center> Cheque has not been prepared for these : <label style='font-weight:bold'>" + refNos + "</label> reference Nos. </center>");
                        }
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private void ClearControls(bool IsddlChanged)
        {
            try
            {
                Session["EditID"] = Guid.Empty.ToString();
                Session["HistoryCheeques"] = null;
                Session["LPOrderIDs"] = null;
                Session["GridOne"] = null;
                Session["GridTwo"] = null;
                Session["INVs"] = null;
                Session["POs"] = null;
                divGridOne.InnerHtml = "";
                DivGridTwo.InnerHtml = "";
                DivHistory.InnerHtml = "";
                DivHistoryCheques.InnerHtml = "";
                HFINV_LPOIDs.Value = "";
                HFPO_LPOIDs.Value = "";
                HFLPOsHistorySum.Value = "";
                HFSelectedLPOID.Value = "";
                Session["ReadOnly"] = "";
                txtRefNo.Text = "";
                if (!IsddlChanged)
                    ddlSupplier.SelectedIndex = -1;
                ddlSupplier.Enabled = true;
                txtAmount.Text = "";
                txtBankName.Text = "";
                txtChequeNo.Text = "";
                txtDate.Text = "";
                txtUTRNo.Text = "";
                txtExportInvDate.Text = "";
                txtExportInvNo.Text = "";
                txtFormCorHNo.Text = "";
                txtFormCorHNoDate.Text = "";
                txtProInfNo.Text = "";
                txtProInvNoDate.Text = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {

            }
        }

        private void SelectedLPOs(string INV_LPOIDs, string PO_LPOIDs)
        {
            string[] dINV = INV_LPOIDs.Trim(',').Split(',').Select(sValue => sValue.Trim()).Distinct().ToArray();
            string[] dPO = PO_LPOIDs.Trim(',').Split(',').Select(sValue => sValue.Trim()).Distinct().ToArray();

            int ii = 0;
            Dictionary<int, string> ddINV;
            ddINV = dINV.ToDictionary(s => ++ii);
            Session["INVs"] = ddINV;

            int jj = 0;
            Dictionary<int, string> ddPO;
            ddPO = dPO.ToDictionary(s => ++jj);
            Session["POs"] = ddPO;
        }

        #endregion

        #region DDL Index Changed

        protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSupplier.SelectedValue != Guid.Empty.ToString())
                {
                    ClearControls(true);
                    Session["HistoryCheeques"] = null;
                    DataSet ds = BPABLL.GetDataSet(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlSupplier.SelectedValue), "", "", new Guid(Session["CompanyID"].ToString()));
                    Session["LPOrderIDs"] = ds;
                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["MaxID"].ToString().Trim() != "")
                        txtRefNo.Text = ds.Tables[1].Rows[0]["MaxID"].ToString().Trim() + "/" + CommonBLL.FinacialYearShort;
                    else
                        txtRefNo.Text = "PA0001/" + CommonBLL.FinacialYearShort;

                    Session["GridOne"] = CommonBLL.EmptyDTBillPaymentApproval_INV();
                    divGridOne.InnerHtml = GridOne(false);
                    Session["GridTwo"] = CommonBLL.EmptyDTBillPaymentApproval_PO();
                    DivGridTwo.InnerHtml = GridTwo(false);
                }
                else
                {
                    Session["LPOrderIDs"] = null;
                    divGridOne.InnerHtml = "";
                    DivGridTwo.InnerHtml = "";
                    txtRefNo.Text = "";
                    HFINV_LPOIDs.Value = "";
                    HFPO_LPOIDs.Value = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        #endregion

        #region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetProformaINV(string LPOID)
        {
            try
            {
                string rslt = ""; string PDate = "", PInvNo = "";
                DataSet ds = INVBLL.SelectPrfmaInvcDtls(CommonBLL.FlagYSelect, new Guid(HttpContext.Current.Session["EditID"].ToString()), new Guid(LPOID), "INV", new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    rslt = ds.Tables[0].Rows[0]["LPODate"].ToString() + "@,@" + ds.Tables[0].Rows[0]["TAMT"].ToString() + "@,@"
                        + ds.Tables[0].Rows[0]["PaidAmt"].ToString();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                    {
                        PDate = string.Join(",", ds.Tables[1].AsEnumerable().Select(x => x.Field<string>("PrfmaInvcDt")));
                        PInvNo = string.Join(",", ds.Tables[1].AsEnumerable().Select(x => x.Field<string>("PrfmInvcNo")));
                        
                        rslt += "@,@" + PDate + "@,@"
                        + PInvNo;
                    }
                }
                return rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddNewRow(int SNO, string LPOID, string LPONo, string LPODT, string LPOAmt, string PrfInvID, string PrfInvNo, string PrfInvAmt, string TaxInvoice, string PayDT, string PayAMT, string Remarks)
        {
            try
            {
                string rslt = "";
                DataTable dt = (DataTable)Session["GridOne"];
                int RowCount = dt.Rows.Count;
                int oldsno = RowCount == 0 ? 0 : Convert.ToInt32(dt.Rows[RowCount - 1]["SNo"]);
                //HFLPOsHistorySum.Value = LPOAmt;

                DataRow dr = dt.NewRow();
                dr["SNo"] = (oldsno + 1);
                dr["LPOID"] = LPOID;
                dr["LPONo"] = LPONo;
                dr["LPODT"] = CommonBLL.DateInsert(LPODT);
                dr["LPOAmt"] = LPOAmt;
                dr["PrfInvNo"] = PrfInvNo;
                dr["PrfInvAmt"] = PrfInvAmt == "" ? "0" : PrfInvAmt;
                dr["TaxInvoice"] = TaxInvoice;
                dr["PayDate"] = CommonBLL.DateInsert(PayDT);
                dr["PayAmt"] = PayAMT;
                dr["Remarks"] = Remarks;
                dt.Rows.Add(dr);

                dt.AcceptChanges();
                Session["GridOne"] = dt;
                return GridOne(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string UpdateRow(int SNO, string LPOID, string LPONo, string LPODT, string LPOAmt, string PrfInvID, string PrfInvNo, string PrfInvAmt, string TaxInvoice, string PayDT, string PayAMT, string Remarks)
        {
            try
            {
                //HFLPOsHistorySum.Value = LPOAmt;
                string rslt = "";
                DataTable dt = (DataTable)Session["GridOne"];
                SNO = (SNO - 1);
                dt.Rows[SNO]["LPOID"] = LPOID;
                dt.Rows[SNO]["LPONo"] = LPONo;
                dt.Rows[SNO]["LPODT"] = CommonBLL.DateInsert(LPODT);
                dt.Rows[SNO]["LPOAmt"] = LPOAmt;
                dt.Rows[SNO]["PrfInvNo"] = PrfInvNo;
                dt.Rows[SNO]["PrfInvAmt"] = PrfInvAmt;
                dt.Rows[SNO]["TaxInvoice"] = TaxInvoice;
                dt.Rows[SNO]["PayDate"] = CommonBLL.DateInsert(PayDT);
                dt.Rows[SNO]["PayAmt"] = PayAMT;
                dt.Rows[SNO]["Remarks"] = Remarks;
                dt.AcceptChanges();
                Session["GridOne"] = dt;
                return GridOne(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteOneRow(int RowID, string INV_LPOIDs, string PO_LPOIDs)
        {
            try
            {
                DataTable dt = (DataTable)Session["GridOne"];
                if (dt != null && dt.Rows.Count > 0)
                    dt.Rows[RowID - 1].Delete();
                dt.AcceptChanges();
                Session["GridOne"] = dt;
                SelectedLPOs(INV_LPOIDs, PO_LPOIDs);
                return GridOne(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetProformaINV1(string LPOID)
        {
            try
            {
                string rslt = "";
                DataSet ds = INVBLL.SelectPrfmaInvcDtls(CommonBLL.FlagYSelect, new Guid(Session["EditID"].ToString()), new Guid(LPOID), "", new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    rslt = ds.Tables[0].Rows[0]["LPODate"].ToString() + "@,@" + ds.Tables[0].Rows[0]["TAMT"].ToString() + "@,@"
                            + ds.Tables[0].Rows[0]["PaidAmt"].ToString() + "@,@" + ds.Tables[0].Rows[0]["TAMTGST"].ToString();
                return rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddNewRow1(int SNO, string LPOID, string LPONo, string LPODT, string LPOAmt, string LPOAmt_GST, string PrfInvNo, string AmtPercent, string PayDT, string PayAMT, string Remarks)
        {
            try
            {
                string rslt = "";
                DataTable dt = (DataTable)Session["GridTwo"];
                int RowCount = dt.Rows.Count;
                int oldsno = RowCount == 0 ? 0 : Convert.ToInt32(dt.Rows[RowCount - 1]["SNo"]);

                DataRow dr = dt.NewRow();
                dr["SNo"] = (oldsno + 1);
                dr["LPOID"] = LPOID;
                dr["LPONo"] = LPONo;
                dr["LPODT"] = CommonBLL.DateInsert(LPODT);
                dr["LPOAmt"] = LPOAmt;
                dr["LPOAmt_GST"] = LPOAmt_GST;
                dr["PrfInvNo"] = PrfInvNo;
                dr["AdvAmtPercnt"] = AmtPercent;
                dr["PayDate"] = CommonBLL.DateInsert(PayDT);
                dr["PayAmt"] = PayAMT;
                dr["Remarks"] = Remarks;
                dt.Rows.Add(dr);

                dt.AcceptChanges();
                Session["GridTwo"] = dt;
                return GridTwo(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string UpdateRow1(int SNO, string LPOID, string LPONo, string LPODT, string LPOAmt, string LPOAmt_GST, string PrfInvNo, string AmtPercent, string PayDT, string PayAMT, string Remarks)
        {
            try
            {
                string rslt = "";
                DataTable dt = (DataTable)Session["GridTwo"];
                SNO = (SNO - 1);
                dt.Rows[SNO]["LPOID"] = LPOID;
                dt.Rows[SNO]["LPONo"] = LPONo;
                dt.Rows[SNO]["LPODT"] = CommonBLL.DateInsert(LPODT);
                dt.Rows[SNO]["LPOAmt"] = LPOAmt;
                dt.Rows[SNO]["LPOAmt_GST"] = LPOAmt_GST;
                dt.Rows[SNO]["PrfInvNo"] = PrfInvNo;
                dt.Rows[SNO]["AdvAmtPercnt"] = AmtPercent;
                dt.Rows[SNO]["PayDate"] = CommonBLL.DateInsert(PayDT);
                dt.Rows[SNO]["PayAmt"] = PayAMT;
                dt.Rows[SNO]["Remarks"] = Remarks;
                dt.AcceptChanges();
                Session["GridTwo"] = dt;
                return GridTwo(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteTwoRow(int RowID, string INV_LPOIDs, string PO_LPOIDs)
        {
            try
            {
                DataTable dt = (DataTable)Session["GridTwo"];
                if (dt != null && dt.Rows.Count > 0)
                    dt.Rows[RowID - 1].Delete();
                dt.AcceptChanges();
                Session["GridTwo"] = dt;
                SelectedLPOs(INV_LPOIDs, PO_LPOIDs);
                return GridTwo(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetHistory(string INV_LPOIDs, string PO_LPOIDs)
        {
            try
            {
                return GetLPOsHistory(INV_LPOIDs, PO_LPOIDs);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetHistoryCheques(string INV_LPOIDs, string PO_LPOIDs, bool IsDelete, bool IsAdded)
        {
            try
            {
                DataTable dt = (DataTable)Session["HistoryCheeques"];
                if (IsDelete)
                    Session["HistoryCheeques"] = null;
                return GridhistoryCheques(INV_LPOIDs, PO_LPOIDs, IsDelete, IsAdded);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckChequeNo(string ChequeNo)
        {
            string Rslt = "False";
            try
            {
                DataSet ds = BPABLL.GetDataSet(CommonBLL.FlagKSelect, new Guid(Session["EditID"].ToString()), Guid.Empty, ChequeNo, "", new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ChequeNo.Trim().ToLower() == ds.Tables[0].Rows[0]["ChequeNo"].ToString().Trim().ToLower())
                        Rslt = "True";
                }
                else
                    Rslt = "False";
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return Rslt;
            }
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
                ArrayList all = (ArrayList)Session["BPAUplds"];
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
        public string GetLPOAmountAlert(string LPOID, string Amount, string ActualLPOAmnt, string previousAmttxtbox)
        {
            try
            {
                string FinalAmount = "";
                DataSet ds = BPABLL.GetDataSet(CommonBLL.FlagXSelect, new Guid(LPOID), new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string LPOPaidAmount = ds.Tables[0].Rows[0]["Amount"].ToString();
                    FinalAmount = (Convert.ToDouble(ActualLPOAmnt) - Convert.ToDouble(previousAmttxtbox) - Convert.ToDouble(LPOPaidAmount)).ToString();
                }
                return FinalAmount;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;

            }
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int res = 1;
                int Rcount = 0;
                DataTable DTINV = (DataTable)Session["GridOne"];
                DataTable DTPO = (DataTable)Session["GridTwo"];
                Filename = FileName();
                if ((DTINV != null && DTINV.Rows.Count > 0) || (DTPO != null && DTPO.Rows.Count > 0))
                {
                    string ProInvNo = txtProInfNo.Text.Trim();
                    DateTime ProInvNoDate = txtProInvNoDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtProInvNoDate.Text);
                    string ExportInvNo = txtExportInvNo.Text.Trim();
                    DateTime ExportInvNoDate = txtExportInvDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtExportInvDate.Text);
                    string FormCorHNo = txtFormCorHNo.Text.Trim();
                    DateTime FormCorHDate = txtFormCorHNoDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtFormCorHNoDate.Text);

                    string Attachments = "";
                    if (Session["BPAUplds"] != null && Session["BPAUplds"].ToString() != "")
                    {
                        ArrayList all = (ArrayList)Session["BPAUplds"];
                        Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                    }

                    if (btnSave.Text == "Save" && Session["UserID"] != null)
                    {
                        res = BPABLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlSupplier.SelectedValue), txtRefNo.Text.Trim(),
                                HFINV_LPOIDs.Value, HFPO_LPOIDs.Value, ProInvNo, ProInvNoDate, ExportInvNo, ExportInvNoDate, FormCorHNo, FormCorHDate,
                                "", DateTime.Now, "", "", 0, "", 0, Attachments, new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty,
                                DateTime.Now, DTINV, DTPO, new Guid(Session["CompanyID"].ToString()));

                        if (res == 0)
                        {

                            ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Bill Payment Approval", "Bill Payment Approval Saved Successfully.");
                            Response.Redirect("BillPaymentApprovalStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", "Error while Saving.");
                        }
                    }
                    else if (btnSave.Text == "Update" && Session["UserID"] != null)
                    {
                        string ChequeNo = txtChequeNo.Text;
                        DateTime ChequeDT = txtDate.Text == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtDate.Text);
                        string BankName = txtBankName.Text;
                        decimal Amount = (txtAmount.Text == "" || txtAmount.Text == "0") ? 0 : Convert.ToDecimal(txtAmount.Text);
                        res = BPABLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(Session["EditID"].ToString()), new Guid(ddlSupplier.SelectedValue),
                            txtRefNo.Text.Trim(), HFINV_LPOIDs.Value, HFPO_LPOIDs.Value, ProInvNo, ProInvNoDate, ExportInvNo, ExportInvNoDate,
                            FormCorHNo, FormCorHDate, ChequeNo, ChequeDT, BankName, txtUTRNo.Text, Amount, "", 0, Attachments, Guid.Empty, DateTime.Now,
                            new Guid(Session["UserID"].ToString()), DateTime.Now, DTINV, DTPO, new Guid(Session["CompanyID"].ToString()));

                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Bill Payment Approval", "Bill Payment Approval Updated Successfully.");
                            Session["EditID"] = 0;
                            Response.Redirect("BillPaymentApprovalStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", "Error while updating.");
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
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
                ClearControls(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Reject Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                int res = 1;
                DataTable DTINV = (DataTable)Session["GridOne"];
                DataTable DTPO = (DataTable)Session["GridTwo"];
                res = BPABLL.InsertUpdateDelete(CommonBLL.FlagRegularDRP, new Guid(Session["EditID"].ToString()), Guid.Empty, txtRefNo.Text.Trim(),
                           "", "", "", DateTime.Now, "", DateTime.Now, "", DateTime.Now, "", DateTime.Now, "", "", 0, txtRejectReasons.Text, 0, "",
                           Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, DTINV, DTPO,
                           new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    SendRejectDefaultMails(CBL.ContactsBindGridByCID(CommonBLL.FlagModify, ViewState["CreatedBy"].ToString(), new Guid(Session["CompanyID"].ToString())));
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Bill Payment Approval", "Bill Payment Approval Rejected Successfully.");
                    Session["EditID"] = 0;

                    Response.Redirect("BillPaymentApprovalStatus.aspx", false);
                }
                else
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Bill Payment Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Rejecting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", "Error while Rejecting.");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            }
        }

        #endregion
    }
}