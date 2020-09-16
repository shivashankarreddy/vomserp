using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace VOMS_ERP.Invoices
{
    public partial class ShpngBilDtlsReport : System.Web.UI.Page
    {

        # region Variables
        long ID;
        ErrorLog ELog = new ErrorLog();
        ShpngBilDtlsBLL SBDBL = new ShpngBilDtlsBLL();
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
            this.ShippingBillDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                ShippingBillDtls.Dispose();
                ShippingBillDtls = null;

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
        /// Bind Data Tables to Report Veiwer
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData(Guid ID)
        {
            try
            {

                #region RPT report

                ReportDataSource ShpngDtlsDSet = new ReportDataSource();
                ReportDataSource ShpngCntrItmDSet = new ReportDataSource();

                ShpngDtlsDSet.Name = "vomserpdbDataSet_SP_ShpngDtlsReport";
                ShpngCntrItmDSet.Name = "vomserpdbDataSet_SP_ShpngDtlsCNTRReport";

                DataSet dataset = SBDBL.SelectShpngBilDtlsReport(ID);


                //rptDoc.FileName = Server.MapPath("\\RDLC\\ShpngBillCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\ShpngBillCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "ShpngFSAddrs.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(SBDBL.SelectShpngBilCNTRDtlsReport(ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    ShippingBillDtls.HasPrintButton = true;
                }
                rptDoc.SetParameterValue("CurrencySymbol", Session["CurrencySymbol"].ToString().Trim());
                ShippingBillDtls.ReportSource = rptDoc;

                #endregion


                lbBtnContinue.PostBackUrl = "~/Invoices/ShpngBilDtls.aspx?ShpngBilID=" + dataset.Tables[0].Rows[0]["ID"].ToString();
                lbtnBack.PostBackUrl = "~/Invoices/ShpngBilStatus.aspx";
                lbBtnContinue.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
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