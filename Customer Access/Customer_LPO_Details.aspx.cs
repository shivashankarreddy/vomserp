using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using Microsoft.Reporting.WebForms;

namespace VOMS_ERP.Customer_Access
{
    public partial class Customer_LPO_Details : System.Web.UI.Page
    {

        # region variables
        Guid ID;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        ReportDocument rptDoc = new ReportDocument();
        FieldAccessBLL FAB = new FieldAccessBLL();
        string PriceSymbol = "";
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
                    //Response.Redirect("../Login.aspx?logout=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer LPO Details", ex.Message.ToString());
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

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                ReportDataSource LPOItmDSet = new ReportDataSource();
                ReportDataSource LPOPymntDSet = new ReportDataSource();
                ReportDataSource LPOTrmCdtnsDSet = new ReportDataSource();


                LPODtlsDSet.Name = "vomserpdbDataSet_SP_LPORPTItemDetails_Customer";
                LPOItmDSet.Name = "vomserpdbDataSet_SP_LPORPTAllItemDeatails";
                LPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetLPODetails_Items_Customer(ID); ;
                DataSet ds = new DataSet();

                DataSet dsss = CRPTBLL.GetLPODetails_Items_Customer(new Guid(Session["CompanyID"].ToString())); ;

                //string val = "RDLC";
                //if (hfPath.Value != "")
                //    val = hfPath.Value;
                if (dsss.Tables[1].Rows[0]["CompanyID"].ToString() == Session["CompanyID"].ToString() && dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Glocem\\LpoCrp.rpt");
                }
                else
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LpoCrp.rpt");
                }


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
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "LpoBody.rpt")
                        {
                            ds = CRPTBLL.GetLPOAllDetails_Items_Customer(ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagZSelect, ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    LclPurchaseOrderDtls.HasPrintButton = true;
                }

                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(HttpContext.Current.Session["CompanyID"].ToString()), "Customer_LPO.Aspx");
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.PriceTagText)))
                        HttpContext.Current.Session["HideFields"] = HideFields.Tables[0];
                }
                if (HttpContext.Current.Session["HideFields"] != null && ((DataTable)HttpContext.Current.Session["HideFields"]).Rows.Count > 0)
                {
                    PriceSymbol = (((DataTable)HttpContext.Current.Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                        .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
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
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png"); //CommonBLL.CommonLogoUrl(HttpContext.Current);

                string AMT = "0.00";
                if (ds != null && ds.Tables.Count > 0)
                    AMT = Convert.ToDecimal(ds.Tables[0].Compute("sum(TotalAmt)", "")).ToString("N2");
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                string DscntAmunt = ""; decimal NetTotal = 0;
                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                {
                    DscntAmunt = ds.Tables[2].Rows[0]["discountamt"].ToString() == "" ? "0" : ds.Tables[2].Rows[0]["discountamt"].ToString();
                    NetTotal = Convert.ToDecimal(AMT) - Convert.ToDecimal(DscntAmunt);
                    NetTotal = Math.Round(NetTotal);
                }
                string AmtInWords = ToWords.Num2WordConverter(NetTotal.ToString(), PriceSymbol).ToString();
                rptDoc.SetParameterValue("TotalAmountInRs", AmtInWords);
                rptDoc.SetParameterValue("CompanyLogo", imgpath);
                if (Convert.ToDecimal(DscntAmunt) != 0)
                {
                    rptDoc.SetParameterValue("discountamt", Convert.ToDecimal(DscntAmunt).ToString("N"));
                    rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                }
                else
                {
                    rptDoc.SetParameterValue("discountamt", "");
                    rptDoc.SetParameterValue("NetTotalVal", "");
                }
                LclPurchaseOrderDtls.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "tdt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=tdt";
                }
                else if (Mode == "tldt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=tldt";
                }
                else if (Mode == "DPtdt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=DPtdt";
                }
                else if (Mode == "dtdtd")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=dtdtd";
                }
                else if (Mode == "Ict")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=Ict";
                }
                else if (Mode == "mtd")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=mtd";
                }
                else if (Mode == "Etdt")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=Etdt";
                }
                else if (Mode == "cpd")
                {
                    lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=cpd";
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Customer Access/Customer_LPO_Status.aspx";
                }
                if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsVerbalFPO"].ToString()) == true && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == false)
                {
                    lbtnCntnu.Visible = false;
                    lbtnCntnu.Visible = false;
                }
                if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0 && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == true)
                {
                    lbtnCntnu.Text = "No Further proceedings for Cancelled LPO";
                    lbtnCntnu.ForeColor = Color.Red;
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = true;
                }

                if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == false)
                    {
                        if ((CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Logistics/RqstCEDtls.aspx")
                            || CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Logistics/DespatchInstructions.aspx")))
                        {

                            lbtnCntnu.PostBackUrl = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                               "~/Logistics/RqstCEDtls.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                               "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString() :
                               "~/Logistics/DespatchInstructions.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                               "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString() +
                               "&FpoId=" + Guid.Empty +
                               "&LpoId=" + dataset.Tables[0].Rows[0]["LocalPurchaseOrderId"].ToString());
                            lbtnCntnu.Text = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                                "Continue with REQUEST CE DETAILS" : "Continue with DISPATCH INSTRUCTIONS");
                        }
                        else
                            lbtnCntnu.Text = "No Further proceedings for Customer(Text NEED TO CHANGE AS PER FUNCTIONALITY)";
                        //else
                        //Response.Redirect("../Masters/Home.aspx?NP=no");
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Customer Local Purchase Order Details", ex.Message.ToString());
                return null;
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