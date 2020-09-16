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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Logistics
{
    public partial class DspchInstRpt : System.Web.UI.Page
    {
        # region variables
        Guid ID;
        ErrorLog ELog = new ErrorLog();
        DspchInstnsBLL DIBL = new DspchInstnsBLL();
        ReportDocument RptDoc = new ReportDocument();
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        //if (!IsPostBack)
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Dispatch Instructions Report", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Page On-Load Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.DispatchInstructionsDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                DispatchInstructionsDtls.Dispose();
                DispatchInstructionsDtls = null;

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

                LPOrdersBLL LPOBLL = new LPOrdersBLL();
                DataSet DspchInst = DIBL.SelectDspchInstnsRpt(ID, new Guid(Session["UserID"].ToString()));
                if (DspchInst.Tables.Count > 0)//&& DspchInst.Tables[0].Rows.Count > 0)
                {

                    //RptDoc.FileName = Server.MapPath("\\RDLC\\DspchInstRpt.rpt");
                    RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\DspchInstRpt.rpt");
                    RptDoc.Load(RptDoc.FileName);
                    RptDoc.Database.Tables[0].SetDataSource(DspchInst.Tables[1]);
                    if (DspchInst.Tables[0].Rows.Count > 0 && DspchInst.Tables[0].Rows[0]["CT1ID"] != null && DspchInst.Tables[0].Rows[0]["CT1ID"].ToString() != Guid.Empty.ToString())
                        RptDoc.SetParameterValue("RPT_CT1NO", DspchInst.Tables[1].Rows[0]["CT1FormNo"].ToString());
                    else
                        RptDoc.SetParameterValue("RPT_CT1NO", "");
                    DispatchInstructionsDtls.ReportSource = RptDoc;
                    string CustID = DspchInst.Tables[1].Rows[0]["CustomerID"].ToString();
                    lbtnBack.PostBackUrl = "~/Logistics/DspchInstnsStatus.aspx";

                    if (DspchInst.Tables[0].Rows.Count > 0 && DspchInst.Tables[0].Rows[0]["Status"].ToString() == "0")
                    {
                        lbtnCntnu.PostBackUrl = "~/Logistics/Gdn.aspx?DspInstID=" + Request.QueryString["ID"].ToString();
                        lbtnCntnu.Text = "Continue with GDN(Goods Dispatch Note) / ";
                        lbContinueGRN.PostBackUrl = "~/Logistics/Grn.aspx?DspInstID=" + Request.QueryString["ID"].ToString() + "&CustID=" + CustID;
                        lbContinueGRN.Text = "GRN(Goods Receipt Note)";
                    }
                    else if (DspchInst.Tables[1].Rows.Count > 0 && DspchInst.Tables[1].Rows[0]["Status"].ToString() == "0")
                    {
                        lbtnCntnu.PostBackUrl = "~/Logistics/Gdn.aspx?DspInstID=" + Request.QueryString["ID"].ToString();
                        lbtnCntnu.Text = "Continue with GDN(Goods Dispatch Note) / ";
                        lbContinueGRN.PostBackUrl = "~/Logistics/Grn.aspx?DspInstID=" + Request.QueryString["ID"].ToString() + "&CustID=" + CustID;
                        lbContinueGRN.Text = "GRN(Goods Receipt Note)";
                    }
                    else
                    {
                        lbtnCntnu.Visible = false;
                        lbContinueGRN.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Dispatch Instructions Report", ex.Message.ToString());
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
