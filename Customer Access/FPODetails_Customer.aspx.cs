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
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;
namespace VOMS_ERP.Customer_Access
{
    public partial class FPODetails_Customer : System.Web.UI.Page
    {

        # region variables
        Guid ID;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no");
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
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

                ReportDataSource FPODtlsDSet = new ReportDataSource();
                ReportDataSource FPOItmDSet = new ReportDataSource();
                ReportDataSource FPOPymntDSet = new ReportDataSource();
                ReportDataSource FPOTrmCdtnsDSet = new ReportDataSource();


                FPODtlsDSet.Name = "vomserpdbDataSet_SP_FPORPTItemDetails";
                FPOItmDSet.Name = "vomserpdbDataSet_SP_FPORPTAllItemDeatails";
                FPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                FPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetFPODetails_Items_Customer(ID); ;


                //rptDoc.FileName = Server.MapPath("\\RDLC\\FpoDetailsCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FpoDetailsCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagYSelect, "ForeignPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "FpoItemsCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFPOAllDetails_Items(ID).Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagYSelect, ID).Tables[0]);

                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    FrnPurchaseOrderDtls.HasPrintButton = true;
                }

                FrnPurchaseOrderDtls.ReportSource = rptDoc;

                #endregion
                string CustID = "0";
                if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    CustID = dataset.Tables[0].Rows[0]["CusmorId"].ToString();

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "tdt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/FPOStatus.aspx?Mode=tdt";
                }
                else if (Mode == "tldt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/FPOStatus.aspx?Mode=tldt";
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Purchases/FPOStatus.aspx";
                }
                if (dataset.Tables[0].Rows[0]["StatusTypeId"].ToString() == "58" || dataset.Tables[0].Rows[0]["StatusTypeId"].ToString() == "59")
                {
                    lbtnCntnu.Visible = false;
                }
                else if ((((ArrayList)Session["UserDtls"])[7].ToString()) != CommonBLL.CustmrContactTypeText && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsFPOCancel"].ToString()) == false)
                {
                    lbtnCntnu.PostBackUrl = "~/Purchases/NewLPOrder.aspx?FpoID=" + ID + "&CustID=" + CustID;
                    lbtnCntnu.Text = "Continue with LOCAL PURCHASE ORDER";
                    lbtnCntnu.Visible = true;
                }
                if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsVerbalFPO"].ToString()) == true && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsFPOCancel"].ToString()) == false)
                {
                    lbtnCntnu.PostBackUrl = "~/Purchases/NewLPOrderVerbal.aspx?FpoID=" + ID + "&CustID=" + CustID;
                    lbtnCntnu.Text = "Continue With VerbalLPO";
                }
                if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsFPOCancel"].ToString()) == true)
                {
                    lbtnCntnu.Text = "No Further proceedings for Cancelled FPO";
                    lbtnCntnu.ForeColor = Color.Red;
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = true;
                }
                if (Request.QueryString["FPOVen"] == "true")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/FPOVendorStatus.aspx";
                }
                //if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    lbtnCntnu.Text = "Continue with VERBAL LOCAL PURCHASE ORDER";
                //    lbtnCntnu.Text = ""
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Purchase Order Details", ex.Message.ToString());
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