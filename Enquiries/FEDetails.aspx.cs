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
using System.Drawing;

namespace VOMS_ERP.Enquiries
{
    public partial class FEDetails : System.Web.UI.Page
    {
        #region Variables
        Guid ID, CompanyID;

        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ReportDocument rptDoc = new ReportDocument();
        #endregion

        #region Default Page Load Event

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
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
        //            Response.Redirect("../Login.aspx?logout=yes", false);
        //        else
        //        {
        //            if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
        //            {
        //                GetData();
        //            }
        //            else
        //                Response.Redirect("../Masters/Home.aspx?NP=no", false);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
        //    }
        //}

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
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FEnquiryCrp.rpt");
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "PATH FE", rptDoc.FileName.ToString());
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
                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
                else
                {
                    string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        lbtnBack.PostBackUrl = "~/Enquiries/FEStatus.aspx?Mode=tdt";
                    }
                    else if (Request.QueryString["FFEnqID"] != null && Request.QueryString["FFEnqID"] != "")
                    {
                        lbtnBack.PostBackUrl = "~/Enquiries/FloatedEnquiryStatus.aspx";
                    }
                    else
                        lbtnBack.PostBackUrl = "~/Enquiries/FEStatus.aspx";

                    if (dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True")
                    {
                        if ((Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) <= 50 || Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) == 55) && Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) != 45 && Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) != 46)
                        {
                            lbtnCntnu.Enabled = true;
                            lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                                + "&FeqID=" + ID;
                        }
                        else
                        {
                            //lbtnCntnu.Text = "Foreign Enquiry Already Floated";
                            //lbtnCntnu.ForeColor = Color.Red;
                            lbtnCntnu.Enabled = false;
                            lbtnCntnu.Visible = false;
                        }
                    }
                    else
                    {
                        lbtnCntnu.Visible = false;
                    }
                    if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()) && string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FEnqID"] != "")
                    {
                        lbtnCntnu.Text = "Continue with Floated Foreign Enquiry";
                        lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiryVendor.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString() + "&FeqID=" + ID;
                    }
                    if (string.IsNullOrEmpty(Request.QueryString["FFEnqID"]) && Request.QueryString["FFEnqID"] != "" && dataset.Tables[0].Rows[0]["IsRegret"].ToString() != "True" && CommonBLL.CustmrContactTypeText != (((ArrayList)Session["UserDtls"])[7].ToString()))
                    {
                        if ((Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) <= 50 || Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) == 55) && Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) != 45 && Convert.ToInt32(dataset.Tables[0].Rows[0]["StatusTypeId"]) != 46)
                        {
                            lbtnCntnu.Enabled = true;
                            lbtnCntnu.Text = "Continue with FLOAT ENQUIRY";
                            lbtnCntnu.PostBackUrl = "~/Enquiries/FloatEnquiry.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                                + "&FeqID=" + ID;
                        }
                        else
                        {
                            //lbtnCntnu.Text = "Foreign Enquiry Already Floated";
                            //lbtnCntnu.ForeColor = Color.Red;
                            lbtnCntnu.Enabled = false;
                            lbtnCntnu.Visible = false;
                        }
                    }
                }

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
