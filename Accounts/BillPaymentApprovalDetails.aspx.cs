using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Reporting.WebForms;

namespace VOMS_ERP.Accounts
{
    public partial class BillPaymentApprovalDetails : System.Web.UI.Page
    {
        # region Variables
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Constring"].ToString());
        ErrorLog ELog = new ErrorLog();
        BillPaymentApprovalBLL BPABLL = new BillPaymentApprovalBLL();
        ReportDocument RptDoc = new ReportDocument();
        # endregion

        protected void Page_init(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (Request.QueryString["ID"] != null)
                        GetData(new Guid(Request.QueryString["ID"]));
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.CT1Details = null;
            GC.Collect();
            //CT1Details.Dispose();       
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                CT1Details.Dispose();
                CT1Details = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        # region Methods

        protected void GetData(Guid ApprovalID)
        {
            try
            {
                DataSet RptDS = new DataSet();
                RptDS = BPABLL.GetReport(ApprovalID, "SP_BillPayDetails");
                if (RptDS != null && RptDS.Tables.Count > 0 && RptDS.Tables[0].Rows.Count > 0)
                {
                    DataSet dsItems_INV = BPABLL.GetReport(ApprovalID, "SP_BillPayDetails_INV");
                    DataSet dsItems_PO = BPABLL.GetReport(ApprovalID, "SP_BillPayDetails_PO");
                    DataSet dsItems_History = BPABLL.GetReport(ApprovalID, "SP_BillPayDetails_History");
                    DataSet dsItems_Cheques = BPABLL.GetReport(ApprovalID, "SP_BillPayDetails_Cheques");

                    //RptDoc.FileName = Server.MapPath("\\RDLC\\BillPayApproval.rpt");
                    RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\BillPayApproval.rpt");
                    RptDoc.Load(RptDoc.FileName);
                    foreach (ReportObject item in RptDoc.ReportDefinition.ReportObjects)
                    {
                        if (item.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)item).SubreportName;
                            ReportDocument subRepDoc = RptDoc.Subreports[SubRepName];
                            if (SubRepName == "BillPayApproval_INV.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems_INV.Tables[0]);
                            else if (SubRepName == "BillPayApproval_PO.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems_PO.Tables[0]);
                            else if (SubRepName == "BillPayApproval_History.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems_History.Tables[0]);
                            else if (SubRepName == "BillPayApproval_Cheques.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems_Cheques.Tables[0]);
                        }
                    }

                    RptDoc.SetParameterValue("A-RefNo", RptDS.Tables[0].Rows[0]["RefNo"].ToString());
                    RptDoc.SetParameterValue("B_SupNm", RptDS.Tables[0].Rows[0]["SupNm"].ToString());
                    RptDoc.SetParameterValue("C_ProInvNo", RptDS.Tables[0].Rows[0]["ProInvNo"].ToString());
                    RptDoc.SetParameterValue("D_ProInvDate", RptDS.Tables[0].Rows[0]["ProInvNoDate"].ToString());
                    RptDoc.SetParameterValue("E_ExpInvNo", RptDS.Tables[0].Rows[0]["ExportInvNo"].ToString());
                    RptDoc.SetParameterValue("F_ExpInvNoDate", RptDS.Tables[0].Rows[0]["ExportInvNoDate"].ToString());
                    RptDoc.SetParameterValue("G_FormCorHNo", RptDS.Tables[0].Rows[0]["FormCorHNo"].ToString());
                    RptDoc.SetParameterValue("H_FormCorHNoDate", RptDS.Tables[0].Rows[0]["FormCorHNoDate"].ToString());

                    RptDoc.SetParameterValue("I_BankName", RptDS.Tables[0].Rows[0]["BankName"].ToString());
                    RptDoc.SetParameterValue("J_ChequeNo", RptDS.Tables[0].Rows[0]["ChequeNo"].ToString());
                    RptDoc.SetParameterValue("K_ChequeDate", RptDS.Tables[0].Rows[0]["ChequeDate"].ToString());
                    RptDoc.SetParameterValue("L_ChequeAmount", RptDS.Tables[0].Rows[0]["ChequeAmount"].ToString());

                    decimal TotalAmtPaid = 0;
                    if (dsItems_INV != null && dsItems_INV.Tables.Count > 0 & dsItems_INV.Tables[0].Rows.Count > 0)
                        TotalAmtPaid = Convert.ToDecimal(dsItems_INV.Tables[0].Compute("Sum(PayAmt)", ""));
                    if (dsItems_PO != null && dsItems_PO.Tables.Count > 0 & dsItems_PO.Tables[0].Rows.Count > 0)
                        TotalAmtPaid += Convert.ToDecimal(dsItems_PO.Tables[0].Compute("Sum(PayAmt)", ""));
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string Words = N2W.Num2WordConverter(TotalAmtPaid.ToString(), "RS").ToString();
                    RptDoc.SetParameterValue("M_TotalAmt", Words);

                    DataTable dtt = dsItems_INV.Tables[0].Copy();
                    dtt = dtt.DefaultView.ToTable(true, "LPOID", "LPOAmt");
                    decimal Total_LPO_INV = 0;
                    if (dtt.Rows.Count > 0)
                        Total_LPO_INV = Convert.ToDecimal(dtt.Compute("sum(LPOAmt)", ""));
                    RptDoc.SetParameterValue("N_Total_LPO_INV", Total_LPO_INV.ToString("N"));

                    DataTable dt = dsItems_PO.Tables[0].Copy();
                    dt = dt.DefaultView.ToTable(true, "LPOID", "LPOAmt");
                    decimal Total_LPO_PO = 0;
                    if (dt.Rows.Count > 0)
                        Total_LPO_PO = Convert.ToDecimal(dt.Compute("sum(LPOAmt)", ""));
                    RptDoc.SetParameterValue("O_Total_LPO_PO", Total_LPO_PO.ToString("N"));

                    DataTable dt_H = dsItems_History.Tables[0].Copy();
                    dt_H = dt_H.DefaultView.ToTable(true, "LPOID", "LPOAmt");
                    decimal Total_LPO_H = 0;
                    if (dt_H.Rows.Count > 0)
                        Total_LPO_H = Convert.ToDecimal(dt_H.Compute("sum(LPOAmt)", ""));
                    RptDoc.SetParameterValue("P_Total_LPO_H", Total_LPO_H.ToString("N"));
                    //RptDoc.SetParameterValue("","");
                    CT1Details.ReportSource = RptDoc;
                }
                lbtnBack.PostBackUrl = "../Accounts/BillpaymentApprovalStatus.aspx";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Billpayment Approval Details", ex.Message.ToString());
            }
        }


        private void CloseReports(ReportDocument reportDocument)
        {
            Sections sections = reportDocument.ReportDefinition.Sections;
            foreach (Section section in sections)
            {
                ReportObjects reportObjects = section.ReportObjects;
                foreach (ReportObject reportObject in reportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                        subReportDocument.Close();
                    }
                }
            }
            reportDocument.Close();
        }

        # endregion
    }
}