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
    public partial class PIdetails : System.Web.UI.Page
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
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\PurchaseIndent.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);

                if (Session["LoginType"].ToString() == "Customer")
                {
                    //TextObject to = ((CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.ReportObjects["Text2"]);
                    ////to.Text = "";

                    //TextObject phone = ((CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.ReportObjects["Text16"]);
                    //phone.Text = "";
                }
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "PurchaseIndentItems.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFenqAllDetails_Items(ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CrystalReportViewer1.HasPrintButton = true;
                }

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[1].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != ""
                     && ((byte[])dataset.Tables[0].Rows[0]["CompanyLogo"]).Length > 0)
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                rptDoc.SetParameterValue("CompanyLogo", imgpath);
                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "tdt")
                {
                    lbtnBack.PostBackUrl = "~/Customer Access/PIStatus.aspx?Mode=tdt";
                }
                else if (Request.QueryString["FFEnqID"] != null && Request.QueryString["FFEnqID"] != "")
                {
                    lbtnBack.PostBackUrl = "~/Customer Access/Float_PI.aspx";
                }
                else
                    lbtnBack.PostBackUrl = "~/Customer Access/PIStatus.aspx";

                if (dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True")
                {
                    lbtnCntnu.PostBackUrl = "~/Customer Access/Float_PI.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                        + "&FeqID=" + ID;
                }
                else
                {
                    lbtnCntnu.Visible = false;
                }
                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()) && string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FEnqID"] != "")
                {
                    lbtnCntnu.Text = "Continue with Floated Foreign Enquiry";
                    lbtnCntnu.PostBackUrl = "~/Customer Access/Float_PI.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString() + "&FeqID=" + ID;
                }
                //if (string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != "" && dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True" && CommonBLL.CustmrContactTypeText != (((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    lbtnCntnu.Text = "Continue with FLOAT ENQUIRY";
                //    lbtnCntnu.PostBackUrl = "~/Customer Access/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
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