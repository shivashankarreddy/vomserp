﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Web;
using BAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Reporting.WebForms;
using System.Drawing;


namespace VOMS_ERP.Quotations
{
    public partial class LQDetails : System.Web.UI.Page
    {
        # region variables
        Guid ID;
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        FieldAccessBLL FAB = new FieldAccessBLL();
        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        string PriceSymbol = "";
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Default Page Load Event

        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    ConfigureCrystalReports();
        //}

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Details", ex.Message.ToString());
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.LclQuotationDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                LclQuotationDtls.Dispose();
                LclQuotationDtls = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Get Data for Report

        /// <summary>
        /// Get Data for Report
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData()
        {
            try
            {
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                    ID = new Guid(Request.QueryString["ID"].ToString());
                else
                    ID = Guid.Empty;

                #region RPT report

                ReportDataSource LQDtlsDSet = new ReportDataSource();
                ReportDataSource LQItmDSet = new ReportDataSource();
                ReportDataSource LQPymntDSet = new ReportDataSource();
                ReportDataSource LQTrmCdtnsDSet = new ReportDataSource();


                LQDtlsDSet.Name = "vomserpdbDataSet_SP_LQRPTItemDeatails";
                LQItmDSet.Name = "vomserpdbDataSet_SP_LQRPTAllItemDeatails";
                LQPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LQTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetLquteDetails_Items(ID); ;

                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "NewLQuotation.Aspx");
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.PriceTagText)))
                        Session["HideFields"] = HideFields.Tables[0];
                }
                //string val = "RDLC";
                //if (hfPath.Value != "")
                //    val = hfPath.Value;
                //rptDoc.FileName = Server.MapPath("\\" + val + "\\LQuotCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LQuotCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                DataSet ds = new DataSet();

                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagWCommonMstr, "LocalQuotationId", ID.ToString()).Tables[0]);
                        else if (SubRepName == "LQuoteBodyCrp.rpt")
                        {
                            //string CurrentcySymbol = "Rs.";
                            //FieldAccessBLL fabll = new FieldAccessBLL();
                            //DataSet dss = fabll.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "NewLQuotation.aspx");
                            //if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            //    CurrentcySymbol = ((dss.Tables[0]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                            //                    .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();

                            //subRepDoc = rptDoc.OpenSubreport("LQuoteBodyCrp.rpt");
                            //subRepDoc.SetParameterValue("CurrentcySymbol", CurrentcySymbol);

                            ds = CRPTBLL.GetLLQAllDetails_Items(ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagWCommonMstr, ID).Tables[0]);
                    }
                }

                if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole || Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                    LclQuotationDtls.HasPrintButton = true;



                if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                {
                    PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                        .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                }

                string AmtFoot = ds.Tables[0].Compute("sum(Amount)", "").ToString();
                string NetAmtFoot = ds.Tables[0].Compute("sum(netamount)", "").ToString();
                AmtFoot = Math.Round(Convert.ToDecimal(AmtFoot), 0).ToString();
                NetAmtFoot = Math.Round(Convert.ToDecimal(NetAmtFoot), 0).ToString();

                if (AmtFoot == "")
                    AmtFoot = "0.00";
                if (NetAmtFoot == "")
                    NetAmtFoot = "0.00";

                clsNum2WordBLL n2w = new clsNum2WordBLL();
                string Words = n2w.Num2WordConverter(NetAmtFoot, PriceSymbol).ToString();
                rptDoc.SetParameterValue("ToWords", Words);


                //string CurrentcySymbol = "Rs.";
                //FieldAccessBLL fabll = new FieldAccessBLL();
                //DataSet dss = fabll.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "NewLQuotation.aspx");
                //if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                //    CurrentcySymbol = ((dss.Tables[0]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                //                    .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();

                //rptDoc.SetParameterValue("CurrentcySymbol", CurrentcySymbol, "LQuoteBodyCrp.rpt");

                LclQuotationDtls.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "tdt")
                {
                    lbtnBack.PostBackUrl = "~/Quotations/LQStatus.aspx?Mode=tdt";
                }
                else if (Mode == "tldt")
                {
                    lbtnBack.PostBackUrl = "~/Quotations/LQStatus.aspx?Mode=tldt";
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Quotations/LQStatus.aspx";
                }
                if ((Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) <= 50 || Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) == 55) && Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) != 45 && Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) != 46)
                {
                    //if (dataset.Tables[2] != null && dataset.Tables[2].Rows.Count > 0 && Convert.ToInt32(dataset.Tables[2].Rows[0]["FCount"]) > 1)
                    //  {
                    lbtnCntnu.PostBackUrl = "~/Quotations/LQComparisionByItems.aspx?CsID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString()
                    + "&FeqID=" + dataset.Tables[0].Rows[0]["ForeignEnquiryId"].ToString();
                    //  }
                    // else
                    // {
                    //     lbtnCntnu.Text = "Continue with Foreign Quotation";
                    //     lbtnCntnu.PostBackUrl = "~/Quotations/NewFQuotation.aspx?CsID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString()
                    //         + "&FeqID=" + dataset.Tables[0].Rows[0]["ForeignEnquiryId"].ToString() + "&FeqIDs";
                    // }

                }
                else
                {
                    // lbtnCntnu.Text = "Local Quotation is Raised";
                    // lbtnCntnu.ForeColor = Color.Red;
                    //lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string[] lineNumber = ex.StackTrace.Split('.');
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Details " + lineNumber
                    + "", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods

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

        #endregion
    }
}
