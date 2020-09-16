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
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
//using VOMS_ERP.RDLC;

namespace VOMS_ERP.Quotations
{
    public partial class FQDetails : System.Web.UI.Page
    {
        # region variables
        Guid ID;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ErrorLog ELog = new ErrorLog();
        //private int m_currentPageIndex;
        //private IList<Stream> m_streams;
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Details", ex.Message.ToString());
            }


        }

        /// <summary>
        /// Page OnUnload Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.FQuotationDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                FQuotationDtls.Dispose();
                FQuotationDtls = null;

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

                ReportDataSource dsItemDetails9 = new ReportDataSource();
                ReportDataSource dsItemDetails8 = new ReportDataSource();
                ReportDataSource dsItemDetails7 = new ReportDataSource();
                dsItemDetails9.Name = "vomserpdbDataSet_SP_FQRPTAllItemDeatails";
                //VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter
                DataSet dsItems9 = new DataSet();
                dsItems9 = CRPTBLL.GetFQAllDetails_Items(ID);
                dsItemDetails9.Value = dsItems9.Tables[0];

                dsItemDetails8.Name = "vomserpdbDataSet_SP_AllTermsConditions";//SP_LPORPT_PaymentsTerms
                DataSet dsItems8 = new DataSet();
                dsItems8 = CRPTBLL.GetAllTermsConditons(CommonBLL.FlagXSelect, "ForeignQuotationId", ID.ToString());

                dsItemDetails7.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";//SP_LPORPT_PaymentsTerms
                DataSet dsItems7 = new DataSet();
                dsItems7 = CRPTBLL.GetLPOPayments(CommonBLL.FlagXSelect, ID);

                //rptDoc.FileName = Server.MapPath("\\RDLC\\FQReport.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FQReport.rpt");
                rptDoc.Load(rptDoc.FileName);

                DataSet dataset1 = new DataSet();
                NewFQuotationBLL NFQBLL1 = new NewFQuotationBLL();
                dataset1 = NFQBLL1.Select(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "",
                    DateTime.Now, 0, Guid.Empty, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, CommonBLL.EmptyDtFQ(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));

                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (!dataset1.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                    {
                        //if (repOp.Name == "Picture2")
                        //    repOp.ObjectFormat.EnableSuppress = true;
                        if (repOp.Name == "VoltaFooter")
                            repOp.ObjectFormat.EnableSuppress = true;
                        if (repOp.Name == "FormetNo")
                            repOp.ObjectFormat.EnableSuppress = true;
                    }
                    else
                    {
                        if (repOp.Name == "FooterText")
                            repOp.ObjectFormat.EnableSuppress = true;
                    }
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "FQTC.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems8.Tables[0]);
                            subRepDoc.Database.Tables[1].SetDataSource(dsItems7.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems7.Tables[0]);
                        }
                        else
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems9.Tables[0]);
                            subRepDoc.Database.Tables[1].SetDataSource(dsItems8.Tables[0]);
                        }
                    }
                }

                rptDoc.SetParameterValue("RPTa_QuotationNo", dataset1.Tables[1].Rows[0]["Quotationnumber"].ToString());
                rptDoc.SetParameterValue("RPTc_KindAttn", dataset1.Tables[1].Rows[0]["Instruction"].ToString());
                rptDoc.SetParameterValue("RPTd_Subject", dataset1.Tables[1].Rows[0]["Subject"].ToString());
                rptDoc.SetParameterValue("RPTe_Reference", dataset1.Tables[1].Rows[0]["FEnqName"].ToString());
                rptDoc.SetParameterValue("RPTf_PriceBasis", dataset1.Tables[1].Rows[0]["PriceBasis"].ToString());
                rptDoc.SetParameterValue("RPTg_Delivery",
                    (Convert.ToDateTime(dataset1.Tables[1].Rows[0]["DeliveryDate"].ToString())).ToString("dd-MM-yyyy"));
                rptDoc.SetParameterValue("RPTj_QtnDate",
                    (Convert.ToDateTime(dataset1.Tables[1].Rows[0]["QuotationDate"].ToString())).ToString("dd-MM-yyyy"));
                rptDoc.SetParameterValue("RPTh_Payment", dataset1.Tables[1].Rows[0]["Quotationnumber"].ToString());
                rptDoc.SetParameterValue("RPTb_To", dataset1.Tables[1].Rows[0]["CustFlNm"].ToString());
                rptDoc.SetParameterValue("RPTm_CnctPrsn", dataset1.Tables[1].Rows[0]["CustCnctPrsn"].ToString());
                rptDoc.SetParameterValue("RPTm_CompanyCnctPrsn", dataset1.Tables[1].Rows[0]["CmpnyCntPersn"].ToString());
                rptDoc.SetParameterValue("RPTm_CompanyAddress", dataset1.Tables[1].Rows[0]["CompanyAddress"].ToString());
                rptDoc.SetParameterValue("RPTm_CompanyName", dataset1.Tables[1].Rows[0]["CompanyName"].ToString());
                rptDoc.SetParameterValue("RPTm_CompanyFax", dataset1.Tables[1].Rows[0]["CompanyFax"].ToString());
                rptDoc.SetParameterValue("RPTm_CompanyEmail", dataset1.Tables[1].Rows[0]["CompanyEmail"].ToString());

                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//CommonBLL.CommonLogoUrl(HttpContext.Current);
                if (!File.Exists(imgpath))
                {
                    byte[] imge = null;
                    if (dataset1 != null && dataset1.Tables[1] != null && dataset1.Tables[1].Rows.Count > 0 && dataset1.Tables[1].Rows[0]["CompanyLogo"].ToString() != ""
                         && ((byte[])dataset1.Tables[1].Rows[0]["CompanyLogo"]).Length > 0)
                    {
                        imge = (byte[])(dataset1.Tables[1].Rows[0]["CompanyLogo"]);
                        using (MemoryStream ms = new MemoryStream(imge))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                            string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                            image.Save(FilePath);
                        }
                    }
                }

                clsNum2WordBLL n2w1 = new clsNum2WordBLL();
                string Words1 = n2w1.Num2WordConverter(dataset1.Tables[1].Compute("Sum(TotalAmount)", "").ToString(), "").ToString();
                rptDoc.SetParameterValue("RPTi_TotalAmount", Words1);

                if (Session["UserName"].ToString() == "System Admin")
                {
                    FQuotationDtls.HasPrintButton = true;
                }
                rptDoc.SetParameterValue("CompanyLogo", imgpath);
                string LogoImage = "";
                if (((string)(dataset1.Tables[1].Rows[0]["CompanyName"].ToString().ToLower())).Contains("volta"))
                {
                    LogoImage = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\FottorLogo1.JPG");
                }
                else
                {
                    LogoImage = "";
                }
                rptDoc.SetParameterValue("ISOLOGO", LogoImage);
                FQuotationDtls.ReportSource = rptDoc;

                #endregion

                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
                else
                {
                    string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "opd")
                    {
                        lbtnBack.PostBackUrl = "~/Quotations/FQStatus.aspx?Mode=opd";
                    }
                    else if (Mode == "tdt")
                    {
                        lbtnBack.PostBackUrl = "~/Quotations/FQStatus.aspx?Mode=tdt";
                    }
                    else
                    {
                        lbtnBack.PostBackUrl = "~/Quotations/FQStatus.aspx";
                    }
                    if (dataset1.Tables[1].Rows[0]["StatusTypeId"].ToString() == "40" && dataset1.Tables[1].Rows[0]["PECompanyID"].ToString() == Session["CompanyID"].ToString())
                    {
                        // Array a = dataset1.Tables[1].Rows[0]["FrnEnqIDs"].ToString().Split(',');
                        // int b = a.Length;
                        //  if (b > 1)
                        lbtnCntnu.PostBackUrl = "~/Purchases/FQComparisionByItems.aspx?CsID=" + dataset1.Tables[1].Rows[0]["CusmorId"].ToString()
                        + "&FeqID=" + dataset1.Tables[1].Rows[0]["FrnEnqIDs"].ToString();
                        //  else
                        //  {
                        //      lbtnCntnu.Text = "Continue with Foreign Purchase Order";
                        //    lbtnCntnu.PostBackUrl = "~/Purchases/NewFPOrder.aspx?CsID=" + dataset1.Tables[1].Rows[0]["CusmorId"].ToString()
                        //  + "&FeqID=" + dataset1.Tables[1].Rows[0]["FrnEnqIDs"].ToString() + "&FeqIDs";
                        // }
                    }
                    else
                    {
                        lbtnCntnu.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Details", ex.Message.ToString());
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
