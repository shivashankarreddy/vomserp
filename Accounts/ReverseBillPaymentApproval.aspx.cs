using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Text;

namespace VOMS_ERP.Accounts
{
    public partial class ReverseBillPaymentApproval : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        ContactBLL CBL = new ContactBLL();
        InvoiceBLL INVBLL = new InvoiceBLL();
        SupplierBLL SPBLL = new SupplierBLL();
        CustomerBLL cusmr = new CustomerBLL();
        BillPaymentApprovalBLL BPABLL = new BillPaymentApprovalBLL();
        static string ReadOnly = "";
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
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        ReadOnly = "";
                        Ajax.Utility.RegisterTypeForAjax(typeof(BillPaymentApproval));
                        if (!IsPostBack)
                        {
                            ClearControls(false);
                            txtDate.Attributes.Add("readonly", "readonly");
                            //txtAmount.Attributes.Add("readonly", "readonly");
                            btnReject.Attributes.Add("OnClick", "Javascript:return MyRejectvalidations()");
                            BindDropDownList(ddlSupplier, SPBLL.SelectSuppliers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                            if (Session["AccessRole"].ToString() == CommonBLL.AmdinContactTypeText || Session["AccessRole"].ToString() == CommonBLL.AccountsContactTypeText)
                                pnlCheque.Visible = true;
                            else
                                pnlCheque.Visible = false;

                            #region Add/Update Permission Code
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                                Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                    FillData(new Guid(Request.QueryString["ID"]));
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnReject.Enabled = true;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        private void FillData(Guid ApprovalID)
        {
            try
            {
                DataSet EditDs = BPABLL.GetDataSet(CommonBLL.FlagModify, ApprovalID, Guid.Empty, "", "", new Guid(Session["CompanyID"].ToString()));
                if (EditDs != null && EditDs.Tables.Count > 2)
                {
                    string Rmks = "";
                    if (EditDs.Tables[0].Rows[0]["Status"].ToString() == "2")
                    {
                        btnReject.Visible = false;
                        btnClear.Visible = false;
                        Rmks = "/Cancelled"; ;
                    }
                    else
                    {
                        btnReject.Visible = true;
                        btnClear.Visible = true;
                    }
                    string ChequeNo = EditDs.Tables[0].Rows[0]["ChequeNo"].ToString() + Rmks;
                    string ChequeDate = EditDs.Tables[0].Rows[0]["ChequeDate"].ToString();
                    string BankName = EditDs.Tables[0].Rows[0]["BankName"].ToString();
                    string UTRNo = EditDs.Tables[0].Rows[0]["UTRNo"].ToString();
                    string ChequeAmount = EditDs.Tables[0].Rows[0]["ChequeAmount"].ToString();
                    ViewState["CreatedBy"] = EditDs.Tables[0].Rows[0]["CreatedBy"].ToString();


                    if (Session["AccessRole"].ToString() == CommonBLL.AmdinContactTypeText || Session["AccessRole"].ToString() == CommonBLL.AccountsContactTypeText) // Accounts and Admin
                    {
                        if (ChequeNo != "" && ChequeDate != "" && BankName != "" && ChequeAmount != "" && Session["AccessRole"].ToString() != CommonBLL.AmdinContactTypeText)
                            ReadOnly = " readonly='readonly' ";
                        else if (Session["AccessRole"].ToString() != CommonBLL.AmdinContactTypeText)
                            ReadOnly = " readonly='readonly' ";
                    }
                    //else if (Session["TeamID"].ToString() == "3" || Session["TeamID"].ToString() == "4") // Purchase
                    //{
                    //    if (ChequeNo != "" && ChequeDate != "" && BankName != "" && ChequeAmount != "")
                    //        ReadOnly = " readonly='readonly' ";
                    //    txtChequeNo.Enabled = false;
                    //    txtDate.Enabled = false;
                    //    txtBankName.Enabled = false;
                    //    txtUTRNo.Enabled = false;
                    //    txtAmount.Enabled = false;
                    //    pnlCheque.Visible = true;
                    //}
                    else
                    {
                        ReadOnly = " readonly='readonly' ";
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

                    ViewState["EditID"] = new Guid(EditDs.Tables[0].Rows[0]["ApprovalID"].ToString());
                    ddlRefNumber.SelectedItem.Text = EditDs.Tables[0].Rows[0]["RefNo"].ToString();

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
                    ddlSupplier.Enabled = false;

                    RejectDtls.Visible = true;

                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "GetBal(false);", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        protected void SendRejectDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                string ToAddrs = "rajaprasadamuteam@yahoo.in" + "," + ToAdrsTbl.Tables[0].Rows[0]["PriEmail"].ToString();

                string CcAddrs = "";

                if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                {
                    CcAddrs = Session["UserMail"].ToString();
                }
                else
                {
                    CcAddrs = Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                }

                string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), "Bill Payment Approval has been Rejected", InformationRejectDtls());

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        protected string InformationRejectDtls()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Bill Payment for Approval has been Rejected " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("Sorry for inconvenience, your Bill Payment for Approval Reference Number " + ddlRefNumber.SelectedItem.Text + " has been Rejected due to following reasons.");
                sb.Append(txtRejectReasons.Text + System.Environment.NewLine);
                sb.Append("Please find the Bill Payment Reference Number " + ddlRefNumber.SelectedItem.Text + " in VOMS Application for the complete details.");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                //sb.Append(System.Environment.NewLine + "Admin ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        private string GetLPOsHistory(string INV_LPOIDs, string PO_LPOIDs)
        {
            try
            {
                string values = INV_LPOIDs + "," + PO_LPOIDs;
                DataSet ds = new DataSet();
                if (ViewState["EditID"] != null && ViewState["EditID"].ToString() == Guid.Empty.ToString())
                    ds = BPABLL.GetDataSet(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, values, values, new Guid(Session["CompanyID"].ToString()));
                else
                    ds = BPABLL.GetDataSet(CommonBLL.FlagCSelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, values, values, new Guid(Session["CompanyID"].ToString()));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        private string GridOne(bool RowAdded)
        {
            try
            {
                decimal tamt = 0;
                decimal LPOamt = 0;
                DataTable dt = (DataTable)Session["GridOne"];
                StringBuilder sb = new StringBuilder();

                if (!RowAdded)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOs' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt</th><th>Proforma Inv No.</th>" +
                        "<th>Date</th><th>Amount</th><th class='rounded-Last'>Remarks</th></tr>");
                    sb.Append("</thead><tbody>");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        tamt += Convert.ToDecimal(dt.Rows[i]["PayAmt"].ToString());
                        LPOamt += Convert.ToDecimal(dt.Rows[i]["LPOAmt"].ToString());

                        sb.Append("<tr>");
                        if (i == (dt.Rows.Count - 1))
                            sb.Append("<td>" + SNo + " <input type='hidden' id='HFINVAmt0' value='" + tamt + "' /> <input type='hidden' id='HFLPOamt0' value='" + LPOamt + "' /> </td>");
                        else
                            sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td id='tdd" + SNo + "'>" + dt.Rows[i]["LPONo"].ToString() + " <input type='hidden' id='HFLPOID" + SNo + "' value='" + dt.Rows[i]["LPOID"].ToString() + "' /></td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["LPODT"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PrfInvNo"].ToString() + " <input type='hidden' id='HFPRFMID" + SNo + "' value='' /></td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["PayDate"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PayAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Remarks"].ToString() + "</td>");
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
                    sb.Append("</th>");

                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'> </th>");

                    sb.Append("<th align='left'>");
                    sb.Append("");
                    sb.Append("<input type='hidden' id='HFPrfInvID0' value='' />");
                    sb.Append("</th>");

                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>");
                    sb.Append("<input type='hidden' id='HFINVAmt0' value='" + tamt + "' />");
                    sb.Append("</th>");
                    sb.Append("<th class='rounded-foot-right'></th>");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private string GridTwo(bool RowAdded)
        {
            try
            {
                decimal amt = 0;
                decimal LPOamt = 0;
                DataTable dt = (DataTable)Session["GridTwo"];
                StringBuilder sb = new StringBuilder();

                if (!RowAdded)
                {
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblLPOs1' class='rounded-corner' style='font-size: small;'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt</th><th>Amt %</th>" +
                        "<th>Date</th><th>Amount</th><th class='rounded-Last'>Remarks</th></tr>");
                    sb.Append("</thead><tbody>");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int SNo = (i + 1);
                        amt += Convert.ToDecimal(dt.Rows[i]["PayAmt"].ToString());
                        LPOamt += Convert.ToDecimal(dt.Rows[i]["LPOAmt"].ToString());

                        sb.Append("<tr>");
                        if (i == (dt.Rows.Count - 1))
                            sb.Append("<td>" + SNo + " <input type='hidden' id='hfPOamt' value='" + amt + "' />  <input type='hidden' id='HFPOLPOamt' value='" + LPOamt + "'> </td>");
                        else
                            sb.Append("<td>" + SNo + " </td>");

                        sb.Append("<td id='tddPO" + SNo + "'>" + dt.Rows[i]["LPONo"].ToString() + " <input type='hidden' id='HFFLPOID" + SNo + "' value='" + dt.Rows[i]["LPOID"].ToString() + "' /></td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["LPODT"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["LPOAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["AdvAmtPercnt"].ToString() + "</td>");
                        sb.Append("<td>" + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["PayDate"].ToString())) + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["PayAmt"].ToString() + "</td>");
                        sb.Append("<td>" + dt.Rows[i]["Remarks"].ToString() + "</td>");
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
                    sb.Append("</th>");

                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'> </th> ");

                    sb.Append("<th align='left'></th>");

                    sb.Append("<th align='left'>  </th>");
                    sb.Append("<th align='left'>");

                    sb.Append("</th>");
                    sb.Append("<th class='rounded-foot-right'></th>");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
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
                dt.Columns.Add("Percent");
                dt.Columns.Add("Date");
                dt.Columns.Add("Amount");

                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
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
                    sb.Append("<tr><th class='rounded-First'>SNo</th><th>LPO No.</th><th>LPO Date</th><th>LPO Amt</th><th>Prf Inv No</th><th>Amt %</th>" +
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
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

                for (int k = 0; k < dINV.Length; k++)
                {
                    TempDS = BPABLL.GetDataSet(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, dINV[k].ToString(), dPO[k].ToString(), new Guid(Session["CompanyID"].ToString()));//INV_LPOIDs, PO_LPOIDs
                    if (TempDS != null && TempDS.Tables.Count > 0)
                    {
                        foreach (DataRow dr in TempDS.Tables[0].Rows)
                        {
                            ds.Tables[0].Rows.Add(dr.ItemArray);
                        }
                        ds.AcceptChanges();
                    }
                }
                for (int l = 0; l < dPO.Length; l++)
                {
                    TempDS = BPABLL.GetDataSet(CommonBLL.FlagESelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, dINV[l].ToString(), dPO[l].ToString(), new Guid(Session["CompanyID"].ToString()));//INV_LPOIDs, PO_LPOIDs
                    if (TempDS != null && TempDS.Tables.Count > 0)
                    {
                        foreach (DataRow dr in TempDS.Tables[0].Rows)
                        {
                            ds.Tables[0].Rows.Add(dr.ItemArray);
                        }
                        ds.AcceptChanges();
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
                        DataSet dss = BPABLL.GetDataSet(CommonBLL.FlagJSelect, new Guid(ViewState["EditID"].ToString()), Guid.Empty, LPOIDS, LPOIDS, new Guid(Session["CompanyID"].ToString()));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
                return ErrMsg + ", " + lineNo;
            }
        }

        private void ClearControls(bool IsddlChanged)
        {
            try
            {
                ViewState["EditID"] = 0;
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
                ReadOnly = "";
                if (!IsddlChanged)
                    ddlSupplier.SelectedIndex = -1;
                ddlSupplier.Enabled = true;
                txtAmount.Text = "";
                txtBankName.Text = "";
                txtChequeNo.Text = "";
                txtDate.Text = "";
                txtExportInvDate.Text = "";
                txtExportInvNo.Text = "";
                txtFormCorHNo.Text = "";
                txtFormCorHNoDate.Text = "";
                txtProInfNo.Text = "";
                txtProInvNoDate.Text = "";
                ddlRefNumber.SelectedIndex = -1;

            }
            catch (Exception ex)
            {

            }
        }

        private void SelectedLPOs(string INV_LPOIDs, string PO_LPOIDs)
        {
            try
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
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        private DataTable ReduceCheckAmout(DataTable DTINV, string ColumnName)
        {
            try
            {

                foreach (DataRow Rrow in DTINV.Rows)
                {
                    Rrow[ColumnName] = "-" + Rrow[ColumnName].ToString(); //"\u00B1"
                    Rrow["Remarks"] = txtRejectReasons.Text;
                }
                DTINV.AcceptChanges();
                return DTINV;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region DDL Index Changed

        /// <summary>
        /// Supplier Name Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSupplier.SelectedValue != Guid.Empty.ToString())
                {
                    ClearControls(true);
                    Session["HistoryCheeques"] = null;
                    DataSet BPBDrp = BPABLL.GetDataSet(CommonBLL.FlagODRP, Guid.Empty, new Guid(ddlSupplier.SelectedValue), "", "", new Guid(Session["CompanyID"].ToString()));
                    BindDropDownList(ddlRefNumber, BPBDrp);
                    decimal ddd = Convert.ToDecimal("-" + 34567.78);
                }
                else
                {
                    Session["LPOrderIDs"] = null;
                    divGridOne.InnerHtml = "";
                    DivGridTwo.InnerHtml = "";
                    HFINV_LPOIDs.Value = "";
                    HFPO_LPOIDs.Value = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string lineNo = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Reference Number Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillData(new Guid(ddlRefNumber.SelectedValue));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Button Clicks

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

                DataTable RDTINVOICE = ReduceCheckAmout(DTINV, "PayAmt");
                DataTable RDTPORDER = ReduceCheckAmout(DTPO, "PayAmt");

                if ((DTINV != null && DTINV.Rows.Count > 0) || (DTPO != null && DTPO.Rows.Count > 0))
                {
                    string ProInvNo = txtProInfNo.Text.Trim();
                    DateTime ProInvNoDate = txtProInvNoDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtProInvNoDate.Text);
                    string ExportInvNo = txtExportInvNo.Text.Trim();
                    DateTime ExportInvNoDate = txtExportInvDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtExportInvDate.Text);
                    string FormCorHNo = txtFormCorHNo.Text.Trim();
                    DateTime FormCorHDate = txtFormCorHNoDate.Text.Trim() == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtFormCorHNoDate.Text);

                    string ChequeNo = txtChequeNo.Text;
                    DateTime ChequeDT = txtDate.Text == "" ? CommonBLL.EndDate : CommonBLL.DateInsert(txtDate.Text);
                    string BankName = txtBankName.Text;
                    decimal Amount = (txtAmount.Text == "" || txtAmount.Text == "0") ? 0 : Convert.ToDecimal(txtAmount.Text);
                    res = BPABLL.InsertUpdateDelete(CommonBLL.FlagZSelect, new Guid(ViewState["EditID"].ToString()), new Guid(ddlSupplier.SelectedValue), ddlRefNumber.SelectedItem.Text.Trim(),
                        HFINV_LPOIDs.Value, HFPO_LPOIDs.Value, ProInvNo, ProInvNoDate, ExportInvNo, ExportInvNoDate, FormCorHNo, FormCorHDate, ChequeNo,
                        ChequeDT, BankName, txtUTRNo.Text, Amount, txtRejectReasons.Text, Convert.ToDecimal("-" + Amount), "", new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, RDTINVOICE, RDTPORDER, new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Reverse Bill Payment Approval", "Bill Payment Approval Reversed Successfully.");
                        ViewState["EditID"] = 0;

                        Response.Redirect("BillPaymentApprovalStatus.aspx", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Reversing.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", "Error while Reversing.");
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }


            #region Not In User

            //try
            //{
            //    int res = 1;
            //    DataTable DTINV = (DataTable)Session["GridOne"];
            //    DataTable DTPO = (DataTable)Session["GridTwo"];
            //    res = BPABLL.InsertUpdateDelete(CommonBLL.FlagRegularDRP, EditID, 0, txtRefNo.Text.Trim(),
            //               "", "", "", DateTime.Now, "", DateTime.Now, "", DateTime.Now, "",
            //               DateTime.Now, "", "", 0, txtRejectReasons.Text, 0, DateTime.Now, Convert.ToInt64(Session["UserID"]), DateTime.Now, DTINV, DTPO);
            //    if (res == 0)
            //    {
            //        SendRejectDefaultMails(CBL.ContactsBindGridByCID(CommonBLL.FlagModify, Convert.ToInt64(ViewState["CreatedBy"].ToString())));
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Bill Payment Approval", "Bill Payment Approval Rejected Successfully.");
            //        EditID = 0;

            //        Response.Redirect("BillPaymentApprovalStatus.aspx", false);
            //    }
            //    else
            //    {
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Rejecting.');", true);
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", "Error while Rejecting.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval", ex.Message.ToString());
            //}

            #endregion

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", ex.Message.ToString());
            }
        }

        #endregion


    }
}