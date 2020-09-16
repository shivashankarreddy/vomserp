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

namespace VOMS_ERP.Invoices
{
    public partial class AirWayBillDetails : System.Web.UI.Page
    {
        # region Variables

        Guid ID;
        ErrorLog ELog = new ErrorLog();
        AirWayBillBLL AwBLL = new AirWayBillBLL();
        ReportDocument rptDoc = new ReportDocument();
        # endregion

        #region Default Page Load

        /// <summary>
        /// Default Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (Request.QueryString["ID"] != null)
                        GetData(new Guid(Request.QueryString["ID"].ToString()));
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
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

        # region Methods

        /// <summary>
        /// Get Default Data
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData(Guid ID)
        {
            try
            {
                #region RPT report

                ReportDataSource AirWayBillDtlsDSet = new ReportDataSource();

                AirWayBillDtlsDSet.Name = "vomserpdbDataSet_SP_AirWayBillReportDtls";

                DataSet dataset = AwBLL.AirWayBillRptDtls(ID);


                //rptDoc.FileName = Server.MapPath("\\RDLC\\AriWayBillCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\AriWayBillCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);


                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }

                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                lbtnBack.PostBackUrl = "~/Invoices/AirWayBillStatus.aspx";
                //lbtnCntnu.PostBackUrl = "~/Invoices/PrfmaInvoice.aspx?ChkLstID=" + ID;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CheckList Details", ex.Message.ToString());
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
