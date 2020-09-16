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
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Logistics
{
    public partial class GdnDetails : System.Web.UI.Page
    {
        # region variables
        Guid ID;
        GrnBLL GDNBL = new GrnBLL();
        ErrorLog ELog = new ErrorLog();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ItemDetailsBLL IDBL = new ItemDetailsBLL();
        ReportDocument RptDoc = new ReportDocument();
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
                        //if (!IsPostBack)
                        GetData();
                    }
                    else
                        Response.Redirect("../Login.aspx?logout=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Dispatch Note Report Details", ex.Message.ToString());
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.GoodDeliveryNoteDetails = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                GoodDeliveryNoteDetails.Dispose();
                GoodDeliveryNoteDetails = null;

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

                # region RPT

                DataSet RptDS = GDNBL.SelectGdnRptDtls(ID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (RptDS != null && RptDS.Tables.Count > 0 && RptDS.Tables[0].Rows.Count > 0)
                {
                    //LPOrdersBLL LPOBLL = new LPOrdersBLL();
                    DataSet GdnItms = IDBL.SelectItemDtlsForGdn(CommonBLL.FlagFSelect, ID);


                    //RptDoc.FileName = Server.MapPath("\\RDLC\\GdnDetails.rpt");
                    RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\GdnDetails.rpt");
                    RptDoc.Load(RptDoc.FileName);

                    RptDoc.Database.Tables[0].SetDataSource(RptDS.Tables[0]);
                    foreach (ReportObject item in RptDoc.ReportDefinition.ReportObjects)
                    {
                        if (item.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)item).SubreportName;
                            ReportDocument subRepDoc = RptDoc.Subreports[SubRepName];
                            if (SubRepName == "GDNItems.rpt")
                            {
                                subRepDoc.Database.Tables[0].SetDataSource(GdnItms.Tables[0]);
                            }
                        }
                    }

                    GoodDeliveryNoteDetails.ReportSource = RptDoc;

                    if (RptDS.Tables[0].Rows[0]["IsApproved"].ToString() == "Approved" && RptDS.Tables[0].Rows[0]["Status"].ToString() == "0")
                    {
                        lbtnCntnu.Visible = true;
                        lbtnCntnu.PostBackUrl = "Grn.Aspx?gdnid=" + ID;
                    }
                }
                #endregion
                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Logistics/GdnStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Dispatch Note Report Details", ex.Message.ToString());
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
