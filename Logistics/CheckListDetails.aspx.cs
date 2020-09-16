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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;
using System.Text;
using Ajax;

namespace VOMS_ERP.Logistics
{
    public partial class CheckListDetails : System.Web.UI.Page
    {
        # region Variables

        long ID;
        ErrorLog ELog = new ErrorLog();
        ReportDocument RptDoc = new ReportDocument();
        # endregion

        #region Default Page Load

        /// <summary>
        /// Default Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        Session["CheckListDetails"] = "";
                        Session["CheckListDetailsHeader"] = "";
                        GetData(new Guid(Request.QueryString["ID"]));
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        /// <summary>
        /// Page On-Load Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.ShipmentPlanningDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                ShipmentPlanningDtls.Dispose();
                ShipmentPlanningDtls = null;

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
        /// Get Default Data
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData(Guid ID)
        {
            try
            {
                CheckListBLL CBLL = new CheckListBLL();
                string bivac = "false"; string ChkList = "";
                DataSet RptDS = CBLL.GetData(CommonBLL.FlagBSelect, ID, "", "", "", Guid.Empty, "", "", "", Guid.Empty,
                    DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                DataSet Itms = CBLL.GetData(ID);
                Session["CheckListDetailsHeader"] = RptDS;
                Session["CheckListDetails"] = Itms;
                if (RptDS != null && RptDS.Tables.Count > 0 && RptDS.Tables[0].Rows.Count > 0 &&
                    Itms != null && Itms.Tables.Count > 0 && Itms.Tables[0].Rows.Count > 0)
                {

                    //RptDoc.FileName = Server.MapPath("\\RDLC\\CheckListDetails.rpt");
                    RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CheckListDetails.rpt");
                    RptDoc.Load(RptDoc.FileName);

                    foreach (ReportObject item in RptDoc.ReportDefinition.ReportObjects)
                    {
                        if (item.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)item).SubreportName;
                            ReportDocument subRepDoc = RptDoc.Subreports[SubRepName];
                            if (SubRepName == "CheckListItems.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(Itms.Tables[0]);
                        }
                    }

                    string PrfInvNO_DT = RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() == "" ? "_______________" :
                        RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString();

                    string PrfInv_DT = RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvcDate"].ToString() == "" ? "__-__-____" :
                        CommonBLL.DateDisplay(Convert.ToDateTime(RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvcDate"].ToString()));

                    RptDoc.SetParameterValue("ShpmntPrfmaInvc", PrfInvNO_DT + " DT: " + PrfInv_DT);

                    //////////////
                    RptDoc.SetParameterValue("DT", RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvcDate"].ToString() == "" ? "__-__-____" :
                        CommonBLL.DateDisplay(Convert.ToDateTime(RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvcDate"].ToString())));
                    RptDoc.SetParameterValue("ShipmentType", RptDS.Tables[0].Rows[0]["shipmentModeType"].ToString());

                    RptDoc.SetParameterValue("Pkgs", Itms.Tables[0].Compute("sum(NoOfPkgs)", "").ToString());
                    RptDoc.SetParameterValue("NetWeight", Itms.Tables[0].Compute("sum(NetWeight)", "").ToString());
                    RptDoc.SetParameterValue("GrossWeight", Itms.Tables[0].Compute("sum(GrWeight)", "").ToString());
                    RptDoc.SetParameterValue("SupInvNos", Itms.Tables[0].Rows[0]["SupINvoiceNos"].ToString());


                    ShipmentPlanningDtls.ReportSource = RptDoc;
                    bivac = RptDS.Tables[0].Rows[0]["IsBivac"].ToString();
                    ChkList = RptDS.Tables[0].Rows[0]["CheckLIstID"].ToString();
                }

                if (bivac == "True")
                    lbtnBack.PostBackUrl = "~/Invoices/ShipmentPlanningDirectStatus.aspx";
                else
                    lbtnBack.PostBackUrl = "~/Logistics/CheckListStatus.aspx";

                if (ChkList == "")
                    lbtnCntnu.PostBackUrl = "~/Invoices/PrfmaInvoice.aspx?ChkLstID=" + ID + "&IsBivac=" + bivac;
                else
                {
                    lbtnCntnu.Text = "Proforma Invoice is prepared for this Shipment Planning Details.";
                    lbtnCntnu.ForeColor = Color.Red;
                    lbtnCntnu.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CheckList Details", ex.Message.ToString());
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

        //Html Table design for ExcelExport
        public string Export_Report_Excel(DataSet Itms)
        {
            CheckListBLL CBLL = new CheckListBLL();
            DataSet RptDS = (DataSet)Session["CheckListDetailsHeader"];
            string ShPfINVDt = RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvcDate"].ToString();
            if (ShPfINVDt == "31-12-9999 00:00:00")
                ShPfINVDt = "";
            if (ShPfINVDt != "31-12-9999 00:00:00" && ShPfINVDt != "")
            {
                DateTime Rdt = Convert.ToDateTime(ShPfINVDt);
                ShPfINVDt = Rdt.ToString("dd-MM-yyyy");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<table id=ExportReport width='100%'> <tr><td colspan=3>");
            sb.Append("<table id=ExportReport1 width='100%' >");
            sb.Append("<tr>");
            if (RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() != "")
            {
                sb.Append("<th style='font-family:Arial;font-size:13;' colspan=8  align='center'> SHIPMENT PLANNING FOR PROFORMA INVOICE NO " + ":" + " " + RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() + ";" + "" + " DT:" + ShPfINVDt + "</th>");
            }
            else
            {
                if (RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() != "" && ShPfINVDt == "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13;' colspan=8  align='center'> SHIPMENT PLANNING FOR PROFORMA INVOICE NO " + ":" + RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() + ";" + "" + " DT:" + "__-__-___" + "</th>");
                }
                else if (RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() == "" && ShPfINVDt != "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13;' colspan=8  align='center'> SHIPMENT PLANNING FOR PROFORMA INVOICE NO " + "_________" + ";" + "" + " DT:" + ShPfINVDt + "</th>");
                }
                else if (RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() == "" && ShPfINVDt == "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13;' colspan=8  align='center'> SHIPMENT PLANNING FOR PROFORMA INVOICE NO " + "_________" + ";" + "" + " DT:" + "__-__-___" + "</th>");
                }
            }
            sb.Append("</tr><tr>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td style='font-family:Arial;font-size:12;' colspan=1>Total no. of Pkgs" + " " + ":" + " " + Itms.Tables[0].Compute("sum(NoOfPkgs)", "").ToString() + "<td>" + "</td>" + "<td>" + "</td>" + "</td>" + "<td style='font-family:Arial;font-size:12;' colspan=1 align='left'>Net Weight" + " " + ":" + " " + Itms.Tables[0].Compute("sum(NetWeight)" + "", "").ToString() + " " + "Kgs (Approx)" + "</td>");
            //+ "<td>" + "</td>" +"<td>" + "</td>" + "</td>" + "<td colspan=2 align='left'>Invoice No:   " + RptDS.Tables[0].Rows[0]["ShpmntPrfmaInvc"].ToString() + " DT:" + ShPfINVDt + "</td>");
            sb.Append("</tr><tr>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td style='font-family:Arial;font-size:12;' colspan=1>Shipment" + " " + ":" + " " + RptDS.Tables[0].Rows[0]["shipmentModeType"].ToString() + "<td>" + "</td>" + "</td>" + "<td>" + "</td>" + "<td style='font-family:Arial;font-size:12;' colspan=1 align='left'>Gross Weight" + " " + ":" + " " + Itms.Tables[0].Compute("sum(GrWeight)", "").ToString() + " " + "Kgs (Approx)" + "</td>");
            sb.Append("</tr><tr>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td colspan=1>" + "</td>");
            sb.Append("<td style='font-family:Arial;font-size:12;' colspan=1>Supplier Invoice No" + " " + ":" + " " + Itms.Tables[0].Rows[0]["SupINvoiceNos"].ToString() + "<td>" + "</td>" + "</td>" + "<td>" + "</td>" + "<td colspan=1 >" + "</td>");
            sb.Append("</tr><tr>");
            sb.Append("</tr></table></td></tr> <tr><td></td></tr>");
            sb.Append(" <table width='10%' border = '1px'><tr>");
            sb.Append("<th style='font-family:Arial;font-size:12;' colspan=0>Sl.No</th><th style='font-family:Arial;font-size:12;'>Pkg. Nos.</th><th style='font-family:Arial;font-size:12;'>Supplier Name</th><th style='font-family:Arial;font-size:12;'>No. Of Pkgs</th><th style='font-family:Arial;font-size:12;'>FPO NO.</th><th style='font-family:Arial;font-size:12;text-align:center;'> Pkgs. to be collected from Transporter Godown LR No. / VIPL Godown Receipt No.</th><th style='font-family:Arial;font-size:12;'>Net. Weight  in Kgs</th><th style='font-family:Arial;font-size:12;'> Gross weight in  Kgs</th><th style='font-family:Arial;font-size:12;'>Remarks</th>                                                                                                          </tr>");
            for (int i = 0; i < Itms.Tables[0].Rows.Count; i++)
            {

                sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["SrNo"].ToString() + "</td>" +
                    //"<td width='11%'>" + Itms.Tables[0].Rows[i]["PkgNos"].ToString() + "</td>" +
                       "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + Itms.Tables[0].Rows[i]["PkgNos"].ToString() + "</td>" +
                      "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["supplierNm"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["NoOfPkgs"].ToString() + "</td>" +
                     "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["FPONOs"].ToString() + "</td>" +
                     "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["LR_GodownNo"].ToString() + "</td>" +
                    //"<td>" + Itms.Tables[0].Rows[i]["IsARE1"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["NetWeight"].ToString() + "</font></td>" +
                     "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["GrWeight"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + Itms.Tables[0].Rows[i]["Remarks"].ToString() + " </td></tr>");
            }
            sb.Append("</table></td></tr> <tr><td colspan=5> <table>");
            //sb.Append("</tr>");
            sb.Append("<td colspan=1>" + "</td>" + "<td colspan=1>" + "</td>" + "<td colspan=1 style='text-align:right;font-family:Arial;font-size:12;'><b> Total:  </b></td>" + "<td style='font-family:Arial;font-size:12;'><b>" + Itms.Tables[0].Compute("sum(NoOfPkgs)", "").ToString() + "</b></td>" + "<td>  </td>" + "<td>  </td>" + "<td style='font-family:Arial;font-size:12;'><b>" + Itms.Tables[0].Compute("sum(NetWeight)", "").ToString() + "</b></td>" + "<td style='font-family:Arial;font-size:12;'><b>" + Itms.Tables[0].Compute("sum(GrWeight)", "").ToString() + "</b></td>" + "<td>  </td>");
            sb.Append("</table> </td></tr>");

            sb.Append("<td colspan=5> <table><tr></tr><tr>");
            sb.Append("</tr>");
            //sb.Append(" <td></td>");
            sb.Append("<tr><td style='font-family:Arial;font-size:12;' colspan=1>" + "</td>" + "<td colspan=1>" + "</td>" + "<td style='font-family:Arial;font-size:12;'> 1)Stuffing Instructions :  </td></tr>");
            //sb.Append(" <td></td>");
            sb.Append("<tr><td colspan=1>" + "<td colspan=1>" + "</td>" + "</td>" + "<td style='font-family:Arial;font-size:12;'>2) B/L Instructions : </td></tr>");
            //sb.Append(" <td></td>");
            sb.Append("<tr><td colspan=1>" + "<td colspan=1>" + "</td>" + "</td>" + "<td style='font-family:Arial;font-size:12;'>3) Remarks/Special Instructions, if any :  </td></tr>");
            sb.Append("<tr></tr>");
            sb.Append("<tr><th></th></tr>" + "<th></th>" + "<th></th>" + "<th style='font-family:Arial;font-size:12;'colspan=1 align='left'> PREPARED BY </th>");
            sb.Append("<th></th>" + "<th style='font-family:Arial;font-size:12;' colspan=2 align='left'> CHECKED BY </th>");
            sb.Append("<th style='font-family:Arial;font-size:12;' colspan=2 align='left'> APPROVED BY </th>");
            sb.Append("</table> </td></tr></table>");

            string res = sb.ToString();

            return res;
            //sb.Append("</table> </td></tr></table>");


        }


        //Click event for Report Export
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            ReportExp();
        }

        # endregion
        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ReportExp()
        {

            Guid Id = (new Guid(Request.QueryString["ID"]));
            string filePath = System.IO.Path.GetFullPath(Server.MapPath("test123456.xls"));
            System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);

            try
            {

                DataSet RptDS = (DataSet)Session["CheckListDetailsHeader"];
                DataSet Itms = (DataSet)Session["CheckListDetails"];
                string filepath1 = @"SHIPMENTPLANNINGEXPORT" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.TimeOfDay.ToString() + ".xls";
                string ExcelData = Export_Report_Excel(Itms);
                string attachment = "attachment; filename= " + filepath1 + "";
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

                HttpContext.Current.Response.Write(ExcelData.ToString());
                HttpContext.Current.Response.End();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }

        #endregion
    }
}
