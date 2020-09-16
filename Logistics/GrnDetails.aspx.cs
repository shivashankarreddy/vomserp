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
using System.IO;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Logistics
{
    public partial class GrnDetails : System.Web.UI.Page
    {

        # region variables
        Guid ID;
        GrnBLL GRNBL = new GrnBLL();
        ErrorLog ELog = new ErrorLog();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ItemDetailsBLL IDBL = new ItemDetailsBLL();
        ReportDocument rptDoc = new ReportDocument();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        #endregion

        #region Default Page Load Evnet
        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Login.aspx?logout=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note Report Details", ex.Message.ToString());
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.GoodReceiptNoteDetails = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                GoodReceiptNoteDetails.Dispose();
                GoodReceiptNoteDetails = null;

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

                ReportDataSource GrnDtlsDSet = new ReportDataSource();
                ReportDataSource GrnItmDSet = new ReportDataSource();
                ReportDataSource GrnARE1DtlsDSet = new ReportDataSource();

                GrnDtlsDSet.Name = "vomserpdbDataSet_SP_Grn";
                GrnItmDSet.Name = "vomserpdbDataSet_SP_ItemDetails";
                GrnARE1DtlsDSet.Name = "vomserpdbDataSet_SP_ARE1Details";

                DataSet GRNDtls = GRNBL.SelectGrnRptDtls(ID, Guid.Empty);
                DataSet GRNAREItms = GRNBL.SelectARE1Dtls(ID);

                GRNDtls.Tables[0].Rows[0]["ARE1No"] = GRNAREItms.Tables[0].Rows.Count.ToString();


                //rptDoc.FileName = Server.MapPath("\\RDLC\\GrnDtlsCrp.rpt"); 
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\GrnDtlsCrp.rpt");
                rptDoc.Load(rptDoc.FileName);

                rptDoc.Database.Tables[0].SetDataSource(GRNDtls.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];

                        if (SubRepName == "GrnItems.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(IDBL.SelectItemDtlsForGrnandGdn(CommonBLL.FlagGSelect, ID).Tables[0]);
                        }
                        else if (SubRepName == "GrnAre1Crp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(GRNAREItms.Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    GoodReceiptNoteDetails.HasPrintButton = true;
                }

                GoodReceiptNoteDetails.ReportSource = rptDoc;

                #endregion

                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
                else
                {

                    lbtnCntnu.Visible = true;
                    if (Convert.ToBoolean(GRNDtls.Tables[0].Rows[0]["IsBivac"].ToString()) != true)
                    {
                        lbtnCntnu.PostBackUrl = "CheckList.Aspx?gdnid=" + ID;
                    }
                    else
                        lbtnCntnu.Visible = false;

                    lbtnBack.PostBackUrl = "~/Logistics/GrnStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note Report Details", ex.Message.ToString());
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
