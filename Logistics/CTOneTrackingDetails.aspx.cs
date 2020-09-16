using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Reporting.WebForms;

namespace VOMS_ERP.Logistics
{
    public partial class CTOneTrackingDetails : System.Web.UI.Page
    {

        ErrorLog ELog = new ErrorLog();
        ReportDocument rptDoc = new ReportDocument();

        #region Page Load Events

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    if (Request.QueryString["ID"] != null)
            //        GetData1(Convert.ToInt64(Request.QueryString["ID"]));
            //}

            if (Request.QueryString["ID"] != null)
                GetData(Convert.ToInt64(Request.QueryString["ID"]));
        }

        /// <summary>
        /// Page On-Load Event
        /// </summary>
        /// <param name="e"></param>
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


        # region Methods

        protected void GetData(long ID)
        {
            try
            {

                # region RPT

                ReportDataSource CT1TrackingDs = new ReportDataSource();

                CT1TrackingDs.Name = "vomserpdbDataSet_SP_CT1Tracking_RDLC";
                CT1TrackingBLL ct1trackBLL = new CT1TrackingBLL();
                DataSet dataset = ct1trackBLL.GetDataSet_RDLC(ID);

                //rptDoc.FileName = Server.MapPath("\\RDLC\\CTOneTracking.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CTOneTracking.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);

                CrystalReportViewer1.ReportSource = rptDoc;

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