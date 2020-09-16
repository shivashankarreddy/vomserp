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
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Purchases
{
    public partial class InsptnReportDetails : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ReportDocument rptDoc = new ReportDocument();
        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
                GetData();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.CrystalReportViewer1 = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                CrystalReportViewer1.Dispose();
                CrystalReportViewer1 = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        private void GetData()
        {
            try
            {
                Guid ID = new Guid(Request.QueryString["ID"].ToString());

                #region RPT report

                ReportDataSource InsptnRptDtlsDSet = new ReportDataSource();
                InsptnRptDtlsDSet.Name = "vomserpdbDataSet_SP_InspectionReport";

                DataSet dataset = CRPTBLL.GetInspecitonReportDtls(ID);

                ReportDocument rptDoc = new ReportDocument();
                //rptDoc.FileName = Server.MapPath("\\RDLC\\InspectionDetailsCRP.rpt"); //SP_InspectionReport
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\InspectionDetailsCRP.rpt");
                rptDoc.Load(rptDoc.FileName);

                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }

                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "DCtldt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/InsptnReportStatus.aspx?Mode=DCtldt";
                }
                else if (Mode == "ICtldt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/InsptnReportStatus.aspx?Mode=ICtldt";
                }
                else if (Request.QueryString["LPOINSP"] == "1")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx";
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Purchases/InsptnReportStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Details", ex.Message.ToString());
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

        #endregion
    }
}
