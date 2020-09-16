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
using Microsoft.Reporting.WebForms;
using BAL;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class SevottamDetails : System.Web.UI.Page
    {
        # region Variables

        long ID;
        ErrorLog ELog = new ErrorLog();
        ReportDocument RptDoc = new ReportDocument();
        # endregion

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
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (Request.QueryString["ID"] != null && Request.QueryString.Count > 1)
                            GetData(new Guid(Request.QueryString["ID"]), Request.QueryString["Type"].ToString());
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "SevottamDetails", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Page On-Unload Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.SevottamDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                SevottamDtls.Dispose();
                SevottamDtls = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        #endregion

        # region Methods

        /// <summary>
        /// Bind Report Data
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Type"></param>
        protected void GetData(Guid ID, string Type)
        {
            try
            {
                # region RPT


                //RptDoc.FileName = Server.MapPath("\\RDLC\\SevottamDetails.rpt");
                RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\SevottamDetails.rpt");
                RptDoc.Load(RptDoc.FileName);
                SevottamBLL SevBLL = new SevottamBLL();

                DataSet dataset = new DataSet();
                dataset = SevBLL.SevottamRDLC(ID, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Type);

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
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
                
                //RptDoc.SetParameterValue("RPTa_CompanyName", dataset.Tables[0].Rows[0]["CompanyName"].ToString());
                //RptDoc.SetParameterValue("RPTa_CompanyAddr", dataset.Tables[0].Rows[0]["CompanyAddress"].ToString());
                //RptDoc.SetParameterValue("RPTa_CompanyFax", dataset.Tables[0].Rows[0]["CompanyFax"].ToString());
                //RptDoc.SetParameterValue("RPTa_CompanyEmail", dataset.Tables[0].Rows[0]["CompanyEmail"].ToString());
                
                if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    RptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                    RptDoc.SetParameterValue("CompanyLogo", imgpath);
                    RptDoc.SetParameterValue("CurrencySymbol", Session["CurrencySymbol"].ToString().Trim());
                    SevottamDtls.ReportSource = RptDoc;
                }

                # endregion
                lbtnBack.PostBackUrl = "~/Logistics/SevottamStatus.aspx";

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Details", ex.Message.ToString());
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

        # endregion
    }
}
