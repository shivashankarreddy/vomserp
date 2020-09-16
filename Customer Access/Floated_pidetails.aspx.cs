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
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace VOMS_ERP.Customer_Access
{
    public partial class Floated_pidetails : System.Web.UI.Page
    {
        #region Variables
        Guid ID, CompanyID;

        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
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
                        Response.Redirect("../Masters/CHome.aspx?NP=no", false);
                }

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
                if (Request.QueryString["FEnqID"] != null && Request.QueryString["FEnqID"] != "")
                {
                    ID = new Guid(Request.QueryString["FEnqID"]);
                    CompanyID = new Guid(Session["CompanyID"].ToString());
                }
                else if (Request.QueryString["FFEnqID"] != null && Request.QueryString["FFEnqID"] != "")
                {
                    ID = new Guid(Request.QueryString["FFEnqID"]);
                    CompanyID = new Guid(Session["CompanyID"].ToString());
                }
                else
                {
                    ID = Guid.Empty;
                    CompanyID = Guid.Empty;
                }
                #region RPT report

                ReportDataSource FeDtlsDSet = new ReportDataSource();
                ReportDataSource FeItmDSet = new ReportDataSource();

                //if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    FeDtlsDSet.Name = "vomserpdbDataSet_SP_FFERPTItemDeatails";
                //    FeItmDSet.Name = "vomserpdbDataSet_SP_FERPTAllItemDeatails";
                //}
                //else
                //{
                FeDtlsDSet.Name = "vomserpdbDataSet_SP_FFERPTItemDeatails";
                FeItmDSet.Name = "vomserpdbDataSet_SP_FERPTAllItemDeatails";
                //}
                DataSet dataset = CRPTBLL.GetFenqDetails_Items(ID, CompanyID);


                //rptDoc.FileName = Server.MapPath("\\RDLC\\FEnquiryCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FEnquiryCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);

                if (Session["LoginType"].ToString() == "Customer")
                {
                    TextObject to = ((CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.ReportObjects["Text2"]);
                    //to.Text = "";

                    TextObject phone = ((CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.ReportObjects["Text16"]);
                    //phone.Text = "";
                }
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "FenqItemsCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFenqAllDetails_Items(ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }

                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                //if (Mode == "tdt")
                //{
                //    lbtnBack.PostBackUrl = "~/Enquiries/FEStatus.aspx?Mode=tdt";
                //}
                //else
                if (Request.QueryString["FFEnqID"] != null && Request.QueryString["FFEnqID"] != "")
                {
                    lbtnBack.PostBackUrl = "~/Customer Access/Float_PIStatus.aspx";
                }

                if (dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True")
                {
                    lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                        + "&FeqID=" + ID;
                }
                else
                {
                    lbtnCntnu.Visible = false;
                }
                if (!string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != ""
                    && dataset.Tables[0].Rows[0]["VendorIds"].ToString() != Guid.Empty.ToString() && dataset.Tables[0].Rows[0]["VendorIds"].ToString() != "")
                {
                    //lbtnCntnu.Text = "Continue with FLOAT ENQUIRY";
                    //lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                    //    + "&FeqID=" + ID;
                    //lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiryVendor.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString() + "&FeqID=" + ID;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != ""
                    && dataset.Tables[0].Rows[0]["F_SupplierIds"].ToString() != Guid.Empty.ToString() && dataset.Tables[0].Rows[0]["F_SupplierIds"].ToString() != "")
                {
                    lbtnCntnu.Text = "Continue with Foreign Quotation";
                    //lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                    lbtnCntnu.PostBackUrl = "~/Customer Access/NewFQ_floatedPI.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString() + "&FeqID=" + ID;
                }

                if (!string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != ""
                    && dataset.Tables[0].Rows[0]["L_SupplierIds"].ToString() != Guid.Empty.ToString() && dataset.Tables[0].Rows[0]["L_SupplierIds"].ToString() != "")
                {
                    lbtnCntnu.Text = "Continue with Local Quotation";
                    lbtnCntnu.PostBackUrl = "~/Customer Access/Customer_LocalQuotation.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                    + "&FeqID=" + dataset.Tables[0].Rows[0]["ForeignEnquireId"].ToString(); //+ "&LeqID=" + ID;
                    //lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                    //lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiryVendor.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString() + "&FeqID=" + ID;
                }

                //if (string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != "" && dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True" && CommonBLL.CustmrContactTypeText != (((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    lbtnCntnu.Text = "Continue with FLOAT ENQUIRY";
                //    lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                //        + "&FeqID=" + ID;
                //}

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreign Enquiy Details", ex.Message.ToString());
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