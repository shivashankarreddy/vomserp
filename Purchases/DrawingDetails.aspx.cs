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
    public partial class DrawingDetails : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        DrawingApprovalBLL DABLL = new DrawingApprovalBLL();
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
        /// Default Page Data
        /// </summary>
        private void GetData()
        {
            try
            {
                Guid DrawingID = new Guid(Request.QueryString["ID"]);

                #region RPT report

                ReportDataSource DrwingDtlsDSet = new ReportDataSource();
                ReportDataSource DrwingItmDSet = new ReportDataSource();

                DrwingDtlsDSet.Name = "vomserpdbDataSet_SP_DrawingDetails";
                DrwingItmDSet.Name = "vomserpdbDataSet_SP_Drawing_RDLC";

                DataSet dataset = DABLL.GetDrawingDetailsRDLC(DrawingID);


                //rptDoc.FileName = Server.MapPath("\\RDLC\\DrawingApprovals.rpt"); //SP_DrawingDetails
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\DrawingApprovals.rpt");
                rptDoc.Load(rptDoc.FileName);

                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];

                        if (SubRepName == "DrawingItems.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(DABLL.GetDataSet_RDLC(DrawingID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }

                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "Dtdt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/DrawingApprovalStatus.aspx?Mode=Dtdt";
                }
                else if (Mode == "DCtldt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/DrawingApprovalStatus.aspx?Mode=DCtldt";
                }
                else if (Request.QueryString["LPODA"] == "1")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx";
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Purchases/DrawingApprovalStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "DrawingApproval Details", ex.Message.ToString());
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
