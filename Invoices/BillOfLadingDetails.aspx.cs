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
    public partial class BillOfLadingDetails : System.Web.UI.Page
    {
        # region Variables
        Guid ID;
        ErrorLog ELog = new ErrorLog();
        BillOfLadingBLL BLBLL = new BillOfLadingBLL();
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
                        GetData(new Guid(Request.QueryString["ID"]));
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

                ReportDataSource BLDtlsDSet = new ReportDataSource();
                ReportDataSource BLCntrItmDSet = new ReportDataSource();

                BLDtlsDSet.Name = "vomserpdbDataSet_SP_BillofLadingReportDtls";
                BLCntrItmDSet.Name = "vomserpdbDataSet_SP_BillOfLading_RDLC";

                DataSet dataset = BLBLL.BillofLadingReportDtls(ID);

                DataSet dsss = BLBLL.BillofLadingReportDtls(new Guid(Session["CompanyID"].ToString()));

                //rptDoc.FileName = Server.MapPath("\\RDLC\\BillofLadingDtlsCrp.rpt");
                if (dsss.Tables[1].Rows[0]["CompanyID"].ToString() == Session["CompanyID"].ToString() && dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Glocem\\BillofLadingDtlsCrp.rpt");
                }
                else
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\BillofLadingDtlsCrp.rpt");
                }
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "BLDContainerDtls.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(BLBLL.GetData_RDLC(ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }
                rptDoc.SetParameterValue("CurrencySymbol", Session["CurrencySymbol"].ToString().Trim());
                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                lbtnBack.PostBackUrl = "~/Invoices/BillOfLadingStatus.aspx";
                //lbtnCntnu.PostBackUrl = "~/Invoices/PrfmaInvoice.aspx?ChkLstID=" + ID;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading Details", ex.Message.ToString());
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
