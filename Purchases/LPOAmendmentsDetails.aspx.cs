using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.IO;
using System.Data;
using Microsoft.Reporting.WebForms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Purchases
{
    public partial class LPOAmendmentsDetails : System.Web.UI.Page
    {

        # region variables
        Guid ID;
        Guid LPOID;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Default Page Load Evnet
        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.LclPurchaseOrderDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                LclPurchaseOrderDtls.Dispose();
                LclPurchaseOrderDtls = null;

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
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "" && Request.QueryString["LPOID"] != null && Request.QueryString["LPOID"] != "")
                {
                    LPOID = new Guid(Request.QueryString["LPOID"].ToString());
                    ID = new Guid(Request.QueryString["ID"].ToString());
                }
                else
                {
                    ID = Guid.Empty;
                    LPOID = Guid.Empty;
                }

                #region RPT report

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                ReportDataSource LPOItmDSet = new ReportDataSource();
                ReportDataSource LPOPymntDSet = new ReportDataSource();
                ReportDataSource LPOTrmCdtnsDSet = new ReportDataSource();


                LPODtlsDSet.Name = "vomserpdbDataSet_SP_LPO_HSTRY_AllTermsConditions";
                LPOItmDSet.Name = "vomserpdbDataSet_SP_LPO_HSTRY_RPTAllItemDeatails";
                LPOPymntDSet.Name = "vomserpdbDataSet_SP_LPO_HSTRY_RPT_PaymentsTerms";
                LPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetLPOHistoryDetails_Items(LPOID, ID);
                DataSet ds = new DataSet();

                //rptDoc.FileName = Server.MapPath("\\RDLC\\LpoCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LpoCrp.rpt");
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
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllHistoryTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", LPOID.ToString(), ID).Tables[0]);
                        }
                        else if (SubRepName == "LpoBody.rpt")
                        {
                            ds = CRPTBLL.GetLPOHistoryAllDetails_Items(LPOID, ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOHistoryPayments(CommonBLL.FlagZSelect, LPOID, ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    LclPurchaseOrderDtls.HasPrintButton = true;
                }

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("~/images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");

                string AMT = "0.00";
                if (ds != null && ds.Tables.Count > 0)
                    AMT = Convert.ToDecimal(ds.Tables[0].Compute("sum(TotalAmt)", "")).ToString("N2");
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                string AmtInWords = ToWords.Num2WordConverter(AMT, "RS").ToString();
                rptDoc.SetParameterValue("TotalAmountInRs", AmtInWords);
                rptDoc.SetParameterValue("CompanyLogo", imgpath);
                rptDoc.SetParameterValue("discountamt", "");
                rptDoc.SetParameterValue("NetTotalVal", "");
                LclPurchaseOrderDtls.ReportSource = rptDoc;

                #endregion

                lbtnBack.PostBackUrl = "~/Purchases/LpoAmendments.aspx?LpoID=" + Request.QueryString["LPOID"];

                if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    lbtnCntnu.PostBackUrl = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                        "~/Logistics/RqstCEDtls.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                        "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString() :
                        "~/Logistics/DespatchInstructions.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                        "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString());
                    lbtnCntnu.Text = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                        "Continue with REQUEST CE DETAILS" : "Continue with DISPATCH INSTRUCTIONS");
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Default/Static Data to Data Table
        /// </summary>
        /// <returns></returns>
        protected DataTable StaticData()
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                DataRow dr1 = dt.NewRow();
                dt.Columns.Add("Title");
                dt.Columns.Add("Description");

                dr["Title"] = "Submission of Documents";
                dr["Description"] = "A set of following documents should be submitted to our Hyderabad office for arranging payment."
                                        + System.Environment.NewLine +
                                    "1) Commercial Invoice." + System.Environment.NewLine +
                                    "2) Copy of L.R." + System.Environment.NewLine +
                                    "3) Packing list including no. of packages, description " +
                                        "and quantity of goods in each pack, weight and size of boxes/packages";
                dt.Rows.Add(dr);

                dr1["Title"] = "Shipping";
                dr1["Description"] = "Following Shipping Marks should be marked on the packing box";

                dt.Rows.Add(dr1);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
                return null;
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