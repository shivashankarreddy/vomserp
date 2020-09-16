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
using System.Drawing.Printing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;

namespace VOMS_ERP.Enquiries
{
    public partial class LEDetails : System.Web.UI.Page
    {
        # region variables
        long ID;
        ErrorLog ELog = new ErrorLog();
        LEnquiryBLL LEBLL = new LEnquiryBLL();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ReportDocument rptDoc = new ReportDocument();
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

                //PrinterSettings p = new PrinterSettings();
                //string aPrinter = "";
                //foreach (string printer in PrinterSettings.InstalledPrinters)
                //{
                //    p.PrinterName = printer;
                //    if (p.IsDefaultPrinter)
                //        aPrinter= printer;
                //}

                //p.DefaultPageSettings.Landscape = true;


                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Details", ex.Message.ToString());
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
                Guid ID = Guid.Empty;
                if (Request.QueryString["LEnqID"] != null && Request.QueryString["LEnqID"] != "")
                    ID = new Guid(Request.QueryString["LEnqID"].ToString());
                else
                    ID = Guid.Empty;

                #region RPT report

                ReportDataSource LeDtlsDSet = new ReportDataSource();
                ReportDataSource LeItmDSet = new ReportDataSource();


                LeDtlsDSet.Name = "vomserpdbDataSet_SP_LERPTItemDeatails";
                LeItmDSet.Name = "vomserpdbDataSet_SP_LERPTAllItemDeatails";

                DataSet dataset = CRPTBLL.GetLenqDetails_Items(ID);

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != ""
                    && ((byte[])dataset.Tables[0].Rows[0]["CompanyLogo"]).Length > 0)
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//CommonBLL.CommonLogoUrl(HttpContext.Current);

                //rptDoc.FileName = Server.MapPath("\\RDLC\\LEnqCrp.rpt");                
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LEnqCrp.rpt");
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "PATH", rptDoc.FileName.ToString());
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                //if (!dataset.Tables[0].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                //{
                //    rptDoc.ReportDefinition.ReportObjects.Item("SpecialNote").ObjectFormat.EnableSuppress = True;
                //}
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (!dataset.Tables[0].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                    {
                        if (repOp.Name == "Picture2")
                            repOp.ObjectFormat.EnableSuppress = true;
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
                        if (SubRepName == "LenqItemsCrp.rpt")
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLEnqAllDetails_Items(ID).Tables[0]);
                    }
                }

                if (Session["AccessRole"].ToString() == "Super Admin" || Session["AccessRole"].ToString() == "Admin")
                    CrystalReportViewer1.HasPrintButton = true;

                rptDoc.SetParameterValue(0, imgpath);//"C:\\Users\\admin\\Desktop\\BitChemy\\VOMS5.5\\VOMS_ERP\\images\\Logos\\admin.jpg");

                CrystalReportViewer1.ReportSource = rptDoc;

                #endregion

                string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                if (Mode == "tdt")
                    lbtnBack.PostBackUrl = "~/Enquiries/LEStatus.aspx?Mode=tdt";
                else if (Mode == "odt")
                    lbtnBack.PostBackUrl = "~/Enquiries/LEStatus.aspx?Mode=odt";
                else
                    lbtnBack.PostBackUrl = "~/Enquiries/LEStatus.aspx";

                if ((Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) <= 50 || Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) == 55) && Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) != 45 && Convert.ToInt32(dataset.Tables[1].Rows[0]["StatusTypeId"]) != 46)
                {
                    lbtnCntnu.Enabled = true;
                    lbtnCntnu.PostBackUrl = "~/Quotations/NewLQuotation.aspx?CsID=" + dataset.Tables[0].Rows[0]["CusmorId"].ToString()
                        + "&FeqID=" + dataset.Tables[0].Rows[0]["ForeignEnquireId"].ToString() + "&LeqID=" + ID;
                }
                else
                {
                    //lbtnCntnu.Text = "Local Enquiry Already Floated";
                    //lbtnCntnu.ForeColor = Color.Red;
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Details", ex.Message.ToString());
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
