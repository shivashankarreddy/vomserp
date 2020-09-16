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
using Microsoft.Reporting.WebForms;
using BAL;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;

namespace VOMS_ERP.Purchases
{
    public partial class CTOneDetails : System.Web.UI.Page
    {
        # region Variables
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Constring"].ToString());
        long ID;
        ErrorLog ELog = new ErrorLog();
        CTOneItemDetailsBLL CTOneBLL = new CTOneItemDetailsBLL();
        ReportDocument RptDoc = new ReportDocument();
        # endregion

        #region Page Load Events

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {

            string QuryStrng = Request.QueryString["ID"].Replace("\'", string.Empty);
            //QuryStrng = QuryStrng;
            if (Request.QueryString["ID"] != null)
                GetData(new Guid(QuryStrng));
        }

        /// <summary>
        /// Page On-Load Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.CT1Details = null;
            GC.Collect();
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

        #endregion

        # region Methods

        protected void GetData(Guid ID)
        {
            try
            {

                # region RPT


                DataSet RptDS = new DataSet();
                RptDS = CTOneBLL.Select(Guid.Empty, ID, Guid.Empty, Guid.Empty);
                if (RptDS.Tables.Count > 0 && RptDS.Tables[0].Rows.Count > 0)
                {
                    DataSet dsItems = new DataSet();
                    dsItems = CTOneBLL.SelectItems(ID, Guid.Empty, Guid.Empty, Guid.Empty);

                    //if (RptDS.Tables[0].Rows[0]["Location"].ToString() == "0" || RptDS.Tables[0].Rows[0]["Location"].ToString() == "712")
                    if (RptDS.Tables[0].Rows[0]["Location"].ToString() == Guid.Empty.ToString() ||
                        RptDS.Tables[0].Rows[0]["LocationText"].ToString().ToLower() == "hyderabad")
                    {
                        //RptDoc.FileName = Server.MapPath("\\RDLC\\CTOneDetails.rpt");
                        RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CTOneDetails.rpt");
                    }
                    else if (RptDS.Tables[0].Rows[0]["Location"].ToString() == Guid.Empty.ToString() ||
                        RptDS.Tables[0].Rows[0]["LocationText"].ToString().ToLower() == "mumbai-1")
                    {
                        //RptDoc.FileName = Server.MapPath("\\RDLC\\CTOneDetailsMumbai.rpt");
                        RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CTOneDetailsMum.rpt");
                    }
                    else// (RptDS.Tables[0].Rows[0]["Location"].ToString() == Guid.Empty.ToString() ||
                       //RptDS.Tables[0].Rows[0]["LocationText"].ToString().ToLower() == "mumbai-1")
                    {
                        //RptDoc.FileName = Server.MapPath("\\RDLC\\CTOneDetailsMumbai.rpt");
                        RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CTOneDetailsMumbai.rpt");
                    }
                    RptDoc.Load(RptDoc.FileName);

                    foreach (ReportObject item in RptDoc.ReportDefinition.ReportObjects)
                    {
                        if (item.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)item).SubreportName;
                            ReportDocument subRepDoc = RptDoc.Subreports[SubRepName];
                            if (SubRepName == "CTOneItems.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems.Tables[0]);
                        }
                    }

                    RptDoc.SetParameterValue("RPTg_CTOneNo", (RptDS.Tables[0].Rows[0]["CT1ReferenceNo"].ToString() ==
                            "" ? "Serial Number  __________" : "Serial Number " + RptDS.Tables[0].Rows[0]["CT1ReferenceNo"].ToString()) + " / (" + CommonBLL.FinacialYearShort + ")");

                    RptDoc.SetParameterValue("RPTa_Range", RptDS.Tables[0].Rows[0]["ExRange"].ToString() + ", " + RptDS.Tables[0].Rows[0]["VoltaExRange"].ToString());
                    RptDoc.SetParameterValue("RPTc_Division", RptDS.Tables[0].Rows[0]["Division"].ToString() + ", " + RptDS.Tables[0].Rows[0]["VoltaDivision"].ToString());
                    RptDoc.SetParameterValue("RPTe_CommissioRt", RptDS.Tables[0].Rows[0]["Commissionerate"].ToString() + ", " + RptDS.Tables[0].Rows[0]["VoltaCommissionerate"].ToString());
                    RptDoc.SetParameterValue("RPTh_ManifacturingAddress", RptDS.Tables[0].Rows[0]["FactoryAddress"].ToString().Replace(",", "," + Environment.NewLine));
                    RptDoc.SetParameterValue("RPTi_ECCNO", RptDS.Tables[0].Rows[0]["ECCNo"].ToString());
                    RptDoc.SetParameterValue("RPTm_FPOs", RptDS.Tables[0].Rows[0]["FPOs"].ToString());

                    string InterNalRefNo = RptDS.Tables[0].Rows[0]["InternalRefNo"].ToString() == "" ? "____________" : RptDS.Tables[0].Rows[0]["InternalRefNo"].ToString();
                    string BondVal = (RptDS.Tables[0].Rows[0]["BondBalanceValue"].ToString() == "" || RptDS.Tables[0].Rows[0]["BondBalanceValue"].ToString() == "0.00")
                                            ? "___________________" : RptDS.Tables[0].Rows[0]["BondBalanceValue"].ToString();
                    string RefDate =
                        CommonBLL.DateDisplay(Convert.ToDateTime(RptDS.Tables[0].Rows[0]["RefDate"].ToString())) == CommonBLL.DateDisplay(DateTime.MaxValue)
                        ? "__-__-____" : CommonBLL.DateDisplay(Convert.ToDateTime(RptDS.Tables[0].Rows[0]["RefDate"].ToString()));
                    string AmtFoot = dsItems.Tables[0].Rows[dsItems.Tables[0].Rows.Count - 1]["EXpercent"].ToString();
                    if (AmtFoot == "")
                        AmtFoot = "0.00";
                    clsNum2WordBLL n2w = new clsNum2WordBLL();
                    string Words = n2w.Num2WordConverter(AmtFoot, "RS").ToString();
                    string Conc = "I here by declare that I have made a provisional debit of Rs. " + Convert.ToInt64(Convert.ToDouble(AmtFoot)).ToString("N")
                            + "/- (" + Words + ") in the Bond Account at serial No." + InterNalRefNo + " dt." + RefDate
                            + " and on this day and after the above mentioned debit, the balance in the Bond Account is Rs." + BondVal + " /-";
                    RptDoc.SetParameterValue("RPTZ_All", Conc);
                    RptDoc.SetParameterValue("RPTq_TariffHedingNo", RptDS.Tables[0].Rows[0]["TariffHedingNo"].ToString());
                    //RptDoc.SetParameterValue("CurrencySymbol",Session["CurrencySymbol"].ToString().Trim());
                    CT1Details.ReportSource = RptDoc;
                }


                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "Etdt")
                    lbtnBack.PostBackUrl = "~/Logistics/CTOneStatus.aspx?Mode=Etdt";
                else if (Mode == "ECtldt")
                    lbtnBack.PostBackUrl = "~/Logistics/CTOneStatus.aspx?Mode=ECtldt";
                else
                    lbtnBack.PostBackUrl = "~/Logistics/CTOneStatus.aspx";

                if (RptDS.Tables.Count > 0 && RptDS.Tables[0].Rows.Count > 0 && RptDS.Tables[0].Rows[0]["CT1ReferenceNo"].ToString() == "")
                {
                    if (RptDS.Tables.Count > 2 && RptDS.Tables[2].Rows.Count > 0
                        && RptDS.Tables[2].Rows[0]["SevottamRefNo"].ToString().Trim() == "")
                    {
                        string SEVID = RptDS.Tables[2].Rows[0]["SevID"].ToString();
                        lbtnCntnu.PostBackUrl = "~/Logistics/Sevottam.aspx?ID=" + SEVID;
                        lbtnCntnu.Text = "Continue with Sevottam to Add Sevottam Ref. No.";
                    }
                    else if (RptDS.Tables.Count > 2 && RptDS.Tables[2].Rows.Count > 0
                        && RptDS.Tables[2].Rows[0]["SevottamRefNo"].ToString().Trim() != "")
                    {
                        string SEVID = RptDS.Tables[2].Rows[0]["SevID"].ToString();
                        lbtnCntnu.PostBackUrl = "~/Logistics/SevottamCTOne.Aspx?ID=" + SEVID;
                        lbtnCntnu.Text = "Continue with Sevottam to update CT-1 Ref. No(s).";
                    }
                    else
                    {
                        lbtnCntnu.Text = "Continue with Sevottam";
                        lbtnCntnu.PostBackUrl = "~/Logistics/Sevottam.aspx";
                    }
                }
                else
                {
                    string CustID = RptDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    string SuppID = RptDS.Tables[0].Rows[0]["SupplierID"].ToString();
                    string Fpos = RptDS.Tables[0].Rows[0]["RefFPOs"].ToString();
                    string Lpos = RptDS.Tables[0].Rows[0]["RefLPOs"].ToString();
                    if (RptDS.Tables[0].Rows[0]["Status"].ToString().Trim() == "Confirm")
                    {
                        lbtnCntnu.PostBackUrl = "~/Logistics/DespatchInstructions.aspx?CstmrID=" + CustID + "&SuplrID=" + SuppID + "&FpoId=" + Fpos + "&LpoId=" + Lpos;
                        lbtnCntnu.Text = "Continue with Dispatch Instructions";
                    }

                }
                # endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Details", ex.Message.ToString());
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
