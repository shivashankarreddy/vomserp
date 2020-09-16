using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using VOMS_ERP.Admin;
using System.Data;
using Ajax;
using System.Text;
using System.Globalization;

namespace VOMS_ERP.Invoices
{
    public partial class ShpngBilDtlsExcelReport : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CommercialINVBLL CIBL = new CommercialINVBLL();
        ExportShipmentDetailsBLL ExBLL = new ExportShipmentDetailsBLL();
        AuditLogs ALS = new AuditLogs();
        Guid custmrval;
        #endregion
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(ShpngBilDtlsExcelReport));
                        btnExcelExpt.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        ImageButton1.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlCommInvNo, ExBLL.GetExportDetailsNumber(Convert.ToChar("H")).Tables[0]);

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Export Excel Shipment Details", ex.Message.ToString());
            }

        }
        /// <summary>
        /// Commercial Invoice No dropdown binding 
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>

        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "CommercialInvNo";
                    ddl.DataValueField = "ExpShipDtlsID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Export Excel Shipment Details", ex.Message.ToString());
            }


        }

        /// <summary>
        /// GEETING THE EXCEL REPORT BASED ON THE INVOICE NUMBER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            Guid InvoiceNumb = new Guid(ddlCommInvNo.SelectedValue);
            if (InvoiceNumb != Guid.Empty || InvoiceNumb.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                ReportExp();
            }
            else
            {
                string strMsg = "Please select Commercial Invoice NO";
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + strMsg + "');", true);
                //Response.Redirect("~/Invoices/ShpngBilDtlsExcelReport.aspx");
            }
        }
        /// <summary>
        /// BINIDNG THE DATA BASED ON THE INVOICE NUMBER
        /// </summary>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ReportExp()
        {

            // Guid Id = (new Guid(Request.QueryString["ID"]));
            string filePath = System.IO.Path.GetFullPath(Server.MapPath("test123456.xls"));
            System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);
            Guid InvoiceNumb = new Guid(ddlCommInvNo.SelectedValue);

            try
            {
                DataSet ds = new DataSet();
                Guid uids = custmrval;
                ds = ExBLL.GetDetailsExportExcel(CommonBLL.FlagModify, InvoiceNumb);
                string filepath1 = @"EXPORT INVOICE NO." + ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() + "" + ".xls";
                string ExcelData = Export_Report_Excel(ds);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Export Excel Shipment Details ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// BINDING THE EXCEL REPORT AND DOWNLOADING BASED ON THE INVOICE NUMBER
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string Export_Report_Excel(DataSet ds)
        {

            string ShPfINVDt = ds.Tables[0].Rows[0]["PerfInvDate"].ToString();
            string CartingAtCfs = Convert.ToDateTime(ds.Tables[0].Rows[0]["DateofCargoCartatCFS"].ToString()).ToString("dd-MM-yyyy");
            if (CartingAtCfs == "31-12-9999")
                CartingAtCfs = "";
            string CustsExamStatus = Convert.ToDateTime(ds.Tables[0].Rows[0]["CustsExamStatus"].ToString()).ToString("dd-MM-yyyy");
            if (CustsExamStatus == "31-12-9999")
                CustsExamStatus = "";
            string ContainerStuffingDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ContainerStuffingDate"].ToString()).ToString("dd-MM-yyyy");
            if (ContainerStuffingDate == "31-12-9999")
                ContainerStuffingDate = "";
            //string VesselDetailsETAETD = Convert.ToDateTime(ds.Tables[0].Rows[0]["VesselDetailsETAETD"].ToString()).ToString("dd-MM-yyyy");
            //if (VesselDetailsETAETD == "31-12-9999")
            //    VesselDetailsETAETD = "";
            string BLAWBapprecon = Convert.ToDateTime(ds.Tables[0].Rows[0]["CustsExamStatus"].ToString()).ToString("dd-MM-yyyy");
            if (BLAWBapprecon == "31-12-9999")
                BLAWBapprecon = "";
            if (ShPfINVDt == "31 Dec 9999 12:00:00 AM")
                ShPfINVDt = "";
            if (ShPfINVDt != "31 Dec 9999 12:00:00 AM" && ShPfINVDt != "")
            {
                DateTime Rdt = Convert.ToDateTime(ShPfINVDt);
                ShPfINVDt = Rdt.ToString("dd-MM-yyyy");
            }
            string BLInvRecDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BLInvRecDate"].ToString()).ToString("dd-MM-yyyy");
            if (BLInvRecDate == "31-12-9999")
                BLInvRecDate = "";
            string BLPayDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BLPayDate"].ToString()).ToString("dd-MM-yyyy");
            if (BLPayDate == "31-12-9999")
                BLPayDate = "";
            string BLAppDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BLAppDate"].ToString()).ToString("dd-MM-yyyy");
            if (BLAppDate == "31-12-9999")
                BLAppDate = "";
            string BLRelDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BLRelDate"].ToString()).ToString("dd-MM-yyyy");
            if (BLRelDate == "31-12-9999")
                BLRelDate = "";
            string BLRecDateAtHYD = Convert.ToDateTime(ds.Tables[0].Rows[0]["BLRecDateAtHYD"].ToString()).ToString("dd-MM-yyyy");
            if (BLRecDateAtHYD == "31-12-9999")
                BLRecDateAtHYD = "";
            string ECTNReqDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ECTNReqDate"].ToString()).ToString("dd-MM-yyyy");
            if (ECTNReqDate == "31-12-9999")
                ECTNReqDate = "";
            string ECTNInvRecDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ECTNInvRecDate"].ToString()).ToString("dd-MM-yyyy");
            if (ECTNInvRecDate == "31-12-9999")
                ECTNInvRecDate = "";
            string ECTNPayDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ECTNPayDate"].ToString()).ToString("dd-MM-yyyy");
            if (ECTNPayDate == "31-12-9999")
                ECTNPayDate = "";
            string RFIFDIformreqon = Convert.ToDateTime(ds.Tables[0].Rows[0]["RFIFDIformreqon"].ToString()).ToString("dd-MM-yyyy");
            if (RFIFDIformreqon == "31-12-9999")
                RFIFDIformreqon = "";
            string RFIFDIformrecon = Convert.ToDateTime(ds.Tables[0].Rows[0]["RFIFDIformrecon"].ToString()).ToString("dd-MM-yyyy");
            if (RFIFDIformrecon == "31-12-9999")
                RFIFDIformrecon = "";
            string BIVACPreshipinspreqon = Convert.ToDateTime(ds.Tables[0].Rows[0]["BIVACPreshipinspreqon"].ToString()).ToString("dd-MM-yyyy");
            if (BIVACPreshipinspreqon == "31-12-9999")
                BIVACPreshipinspreqon = "";
            string ECTNNoRecDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ECTNNoRecDate"].ToString()).ToString("dd-MM-yyyy");
            if (ECTNNoRecDate == "31-12-9999")
                ECTNNoRecDate = "";
            string BIVACPreshipinspcomptdon = Convert.ToDateTime(ds.Tables[0].Rows[0]["BIVACPreshipinspcomptdon"].ToString()).ToString("dd-MM-yyyy");
            if (BIVACPreshipinspcomptdon == "31-12-9999")
                BIVACPreshipinspcomptdon = "";
            string CerfofOriginFAPCCIDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["CerfofOriginFAPCCIDate"].ToString()).ToString("dd-MM-yyyy");
            if (CerfofOriginFAPCCIDate == "31-12-9999")
                CerfofOriginFAPCCIDate = "";
            string PerfInvDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["PerfInvDate"].ToString()).ToString("dd-MM-yyyy");
            if (PerfInvDate == "31-12-9999")
                PerfInvDate = "";
            //string AVCoCConsigneeon = ds.Tables[0].Rows[0]["AVCoCConsigneeon"].ToString();
            string AVCoCConsigneeon = Convert.ToDateTime(ds.Tables[0].Rows[0]["AVCoCConsigneeon"].ToString()).ToString("dd-MM-yyyy");
            if (AVCoCConsigneeon == "31-12-9999")
                AVCoCConsigneeon = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<table id=ExportReport width='100%'> <tr><td colspan=3>");
            sb.Append("<table id=ExportReport1 width='100%'  >");
            sb.Append("<tr>");
            sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=6  align='center'> VOLTA IMPEX PVT LTD </th>");
            sb.Append("</tr><tr>");
            sb.Append("<table id=ExportReport1 width='100%'   >");
            if (ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() != "")
            {
                sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=6  align='center'> Shipping details of Export Invoice no " + ":" + " " + ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() + ";" + "" + " DT:" + ShPfINVDt + "</th>");
            }
            else
            {
                if (ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() != "" && ShPfINVDt == "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13; 'border = '1px' colspan=8  align='center'> Shipping details of Export Invoice no  " + ":" + ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() + ";" + "" + " DT:" + "__-__-___" + "</th>");
                }
                else if (ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() == "" && ShPfINVDt != "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=8  align='center'> Shipping details of Export Invoice no  " + "_________" + ";" + "" + " DT:" + ShPfINVDt + "</th>");
                }
                else if (ds.Tables[0].Rows[0]["CommercialInvNo"].ToString() == "" && ShPfINVDt == "")
                {
                    sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=8  align='center'> Shipping details of Export Invoice no  " + "_________" + ";" + "" + " DT:" + "__-__-___" + "</th>");
                }
            }
            sb.Append("</table>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td><td>");
            sb.Append(" <table width='20%' border = '1px'><tr>");
            sb.Append("<th style='font-family:Arial;font-size:12;' colspan=0>Sl.No</th><th style='font-family:Arial;font-size:12;'>Consignee: Diamond Cement Mali</th> <th style='font-family:Arial;font-size:12;'>Date</th>  <th style='font-family:Arial;font-size:12;'>No. of. Days from receipt of cargo</th><th style='font-family:Arial;font-size:12;' colspan=2 >Remarks</th></tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "1" + "</td>" +
                           "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Proforma Invoice & Packing List no. & date " + "</td>" +
                           "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["PerfInvNo"].ToString() + "," + "" + " DT:" + PerfInvDate + "</td>" +
                           "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                           "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "2" + "</td>" +
                          "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Mode of Shipment  " + "</td>" +
                          "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["ModeofShpt"].ToString() + "</td>" +
                          "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                          "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "3" + "</td>" +
                         "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "FOB Value  " + "</td>" +
                         "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["FobValueUSD"].ToString() + "</td>" +
                         "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                         "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "4" + "</td>" +
                      "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Freight & Insurance " + "</td>" +
                      "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["FreightIns"].ToString() + "</td>" +
                      "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                      "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "5" + "</td>" +
                     "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "CFR/CIF Value  " + "</td>" +
                     "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["CFRCIFValue"].ToString() + "</td>" +
                     "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                     "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "6" + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Port of loading  " + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["PortofLoading"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                    "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "7" + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Port of Discharge  " + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["PortofDisc"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                    "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "8" + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "No. Of. Packages  " + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["Noofpkgs"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                    "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "9" + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Net Weight in Kgs.  " + "</td>" +
                    "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["NetWeightinKgs"].ToString() + "</td>" +
                    "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                    "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "10" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Gross Weight in Kgs  " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["GrossWeightinKgs"].ToString() + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "11" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Final/Last cargo received at stores/CFS on  " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + CartingAtCfs + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "12" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Shipping Bill no. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["ShippBillNo"].ToString() + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "13" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Customs clearance completed on. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + CustsExamStatus + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "14" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Container Stuffing & Gate-in on. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ContainerStuffingDate + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "15" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "On-board vessel details with ETD & ETA. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["VesselDetailsETAETD"].ToString() + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "16" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "BL/AWB (1st print) applied & received on. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLAWBapprecon + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "17" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "BL / AWB No. & date " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["PartofBLAWB"].ToString() + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "18" + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Freight Invoice received on. " + "</td>" +
                   "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLInvRecDate + "</td>" +
                   "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                   "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "19" + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Freight invoice payment on. " + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLPayDate + "</td>" +
                  "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                  "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "20" + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Bill of Lading/AWB approved on. " + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLAppDate + "</td>" +
                  "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                  "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "21" + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Bill of Lading/AWB released on. " + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLRelDate + "</td>" +
                  "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                  "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "22" + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Bill of Lading/AWB (Soft Copy) received on. " + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLRecDateAtHYD + "</td>" +
                 "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                 "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "23" + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Bill of Lading/AWB (Hard Copy) received on. " + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BLRecDateAtHYD + "</td>" +
                 "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                 "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "24" + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "ECTN No. request on. " + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ECTNReqDate + "</td>" +
                "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "25" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "ECTN Invoice received on. " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ECTNInvRecDate + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "26" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "ECTN Invoice payment on. " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ECTNPayDate + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "27" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " Docs./Details pertaininng to ECTN received from consignee on. " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["DOCDetailsReqConsignee"].ToString() + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "28" + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " ECTN received on. " + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ECTNNoRecDate + "</td>" +
                "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "29" + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " RFI/FDI form requested on. " + "</td>" +
                "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + RFIFDIformreqon + "</td>" +
                "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "30" + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " RFI/FDI form received on. " + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + RFIFDIformrecon + "</td>" +
                 "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                 "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "31" + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " BIVAC/ Pre-shipment inspection request on. " + "</td>" +
                  "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BIVACPreshipinspreqon + "</td>" +
                  "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                  "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "32" + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + " BIVAC/ Pre-shipment inspection completed on . " + "</td>" +
                 "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + BIVACPreshipinspcomptdon + "</td>" +
                 "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
                 "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "33" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Certificate of Origin date . " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + CerfofOriginFAPCCIDate + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "34" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Export Invoice courier details  . " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["StatusofCommInv"].ToString() + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "35" + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "AV / CoC (certificate) forwarded to Consignee on  . " + "</td>" +
               "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + AVCoCConsigneeon + "</td>" +
               "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
               "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("<tr><td style='font-family:Arial;font-size:12;'>" + "36" + "</td>" +
              "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + "Other Information  . " + "</td>" +
              "<td STYLE='MSO-NUMBER-FORMAT:\\@;font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[0]["Remarks"].ToString() + "</td>" +
              "<td style='font-family:Arial;font-size:12;'>" + " " + "</td>" +
              "<td style='font-family:Arial;font-size:12;' colspan=2 >" + " " + "</td>" + " </tr>");

            sb.Append("</table></td></tr>");

            sb.Append("</td></tr></table>");

            string res = sb.ToString();

            return res;
        }
        /// <summary>
        ///  REPORT BASED ON DATE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Date_ExcelExpt_Click(object sender, ImageClickEventArgs e)
        {

            ReportExp_Excel_one();

        }
        private DateTime Dt()
        {
            if (toDate.Text != "")
            {
                DateTime todt = DateTime.ParseExact(toDate.Text, "dd-MM-yyyy", null);// CommonBLL.DateInsert(toDate.Text);
                return todt;
            }
            else
            {
                DateTime todt = DateTime.Now;
                return todt;
            }
        }
        /// <summary>
        /// BINIDNG THE DATA SET BASED ON THE FROM AND TODATE 
        /// </summary>
        public void ReportExp_Excel_one()
        {

            // Guid Id = (new Guid(Request.QueryString["ID"]));
            string filePath = System.IO.Path.GetFullPath(Server.MapPath("test123456.xls"));
            System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);
            // Guid InvoiceNumb = new Guid(ddlCommInvNo.SelectedValue);

            //String FromDate = fromDate.Text;
            //String ToDate = (toDate.Text);
            string DisplyDate = Convert.ToDateTime(DateTime.Now.ToString()).ToString("dd-MM-yyyy");
            DateTime frmdt = DateTime.ParseExact(fromDate.Text, "dd-MM-yyyy", null);
            // DateTime frmdt = Convert.ToDateTime(fromDate.Text);// CommonBLL.DateInsert(fromDate.Text);

            try
            {
                DataSet ds = new DataSet();
                Guid uids = custmrval;
                //ds = ExBLL.GetDataSetDate(CommonBLL.FlagOSelect, InvoiceNumb, frmdt);
                ds = ExBLL.GetDataSetDate(CommonBLL.FlagOSelect, frmdt, Dt());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string filepath1 = @"EXPORT INVOICE Details." + DisplyDate + "" + ".xls";
                    string ExcelData = Export_Report_Excel_FromDate_Todate(ds);
                    string attachment = "attachment; filename= " + filepath1 + "";
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

                    HttpContext.Current.Response.Write(ExcelData.ToString());
                    HttpContext.Current.Response.End();
                }
                else
                {
                    string strMsg = "No records Found";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "yourMessages", "ErrorMessage('" + strMsg + "');", true);

                    InvNumber.Checked = true;
                    InvcDate.Checked = false;
                    fromDate.Text = "";
                    toDate.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                string strMsg = "No records Found";
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + strMsg + "');", true);
                // ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Export Excel Shipment Details ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// DOWNLOADING AND DESIGNING THE EXCEL BASED ON THE AS ON DATE 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string Export_Report_Excel_FromDate_Todate(DataSet ds)
        {
            string DATEtIME = Convert.ToDateTime(DateTime.Now.ToString()).ToString("dd-MM-yyyy");
            StringBuilder sb = new StringBuilder();
            sb.Append("<table id=ExportReport width='100%'> <tr><td colspan=3>");
            sb.Append("<table id=ExportReport1 width='100%'  >");
            sb.Append("<tr>");
            sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=6  align='center'> VOLTA IMPEX PVT LTD </th>");
            sb.Append("</tr><tr>");
            sb.Append("<table id=ExportReport1 width='100%'   >");
            //if (ds.Tables[1].Rows[0]["CommercialInvoiceNo"].ToString() != "")
            //{
            sb.Append("<th style='font-family:Arial;font-size:13;' border = '1px' colspan=6  align='center'> PROFORMA INVOICES STATEMENT AS ON DATE" + ":" + " " + Convert.ToDateTime(DateTime.Now.ToString()).ToString("dd-MM-yyyy") + "</th>");
            sb.Append("</table>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append(" <table width='10%' border = '1px'><tr>");
            sb.Append("<th style='font-family:Arial;font-size:12;' colspan=0>Sl.No</th><th style='font-family:Arial;font-size:12;'> CONSIGNEE NAME</th><th style='font-family:Arial;font-size:12;'>BIVAC/PRESHIPT INSP DETAILS/STATUS</th><th style='font-family:Arial;font-size:12;'>SUPPLIER/CARGO DESCRIPTION</th> <th style='font-family:Arial;font-size:12;' colspan=1 >P.INV NO</th><th style='font-family:Arial;font-size:12;' colspan=1 >DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >MODE OF SHPT</th> <th style='font-family:Arial;font-size:12;' colspan=1 >FOB VALUE IN USD</th><th style='font-family:Arial;font-size:12;' colspan=1 >FREIGHT & INS</th><th style='font-family:Arial;font-size:12;' colspan=1 >CRF/CIF VALUE</th><th style='font-family:Arial;font-size:12;' colspan=1 >PCRT OF LOADING</th> <th style='font-family:Arial;font-size:12;' colspan=1 >PORT OF DISC</th> <th style='font-family:Arial;font-size:12;' colspan=1 >NO OF PKGS</th><th style='font-family:Arial;font-size:12;' colspan=1 >NET WEIGHT IN KGS</th><th style='font-family:Arial;font-size:12;' colspan=1 >GROSS WEIGHT IN KGS</th> <th style='font-family:Arial;font-size:12;' colspan=1 >SHIPPING BILL NO</th><th style='font-family:Arial;font-size:12;' colspan=1 >DATE OF CARGO CARTING AT CFT</th><th style='font-family:Arial;font-size:12;' colspan=1 >CUSTS.EXA M.STATUS/COMPLTN.DT</th><th style='font-family:Arial;font-size:12;' colspan=1 >CONTAINER NO </th><th style='font-family:Arial;font-size:12;' colspan=1 >CONTAINER STUFFING STATUS/DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >VESSEL DETAILS WITH</th><th style='font-family:Arial;font-size:12;' colspan=1 >PARTUCULARS OF BL/AWB</th><th style='font-family:Arial;font-size:12;' colspan=1 >PARTICULARS OF ECTN/URN/ID FNO</th><th style='font-family:Arial;font-size:12;' colspan=1 >ECTN REQUEST DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >ECTN INVOICE RECEIVED DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >ECTN PAYMENT STATUS DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >ECTN NO RECEIVED DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >BL INVOICE RECEIVED DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >BL PAYMENT STATUS DATE</th> <th style='font-family:Arial;font-size:12;' colspan=1 >BL APPROVED DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >BL RELESE STATUS DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >BL RECEIVED DATE AT HYD OFFICE</th> <th style='font-family:Arial;font-size:12;' colspan=1 >COMMERCIAL INVOICE DETAILS</th><th style='font-family:Arial;font-size:12;' colspan=1 >CERTF OF ORIGIN BY FAPCCI DATE</th><th style='font-family:Arial;font-size:12;' colspan=1 >STATUS OF COMMERCIAL INVOICE </th><th style='font-family:Arial;font-size:12;' colspan=1 >DOC/DETAILS REQUESTED FROM CONSIGNEE</th><th style='font-family:Arial;font-size:12;' colspan=1 >REMARKS</th></tr>");

            //sb.Append("<th style='font-family:Arial;font-size:12;' colspan=0>Sl.No</th><th style='font-family:Arial;font-size:12;'>Pkg. Nos.</th><th style='font-family:Arial;font-size:12;'>Supplier Name</th><th style='font-family:Arial;font-size:12;'>No. Of Pkgs</th><th style='font-family:Arial;font-size:12;'>FPO NO.</th><th style='font-family:Arial;font-size:12;text-align:center;'> Pkgs. to be collected from Transporter Godown LR No. / VIPL Godown Receipt No.</th><th style='font-family:Arial;font-size:12;'>Net. Weight  in Kgs</th><th style='font-family:Arial;font-size:12;'> Gross weight in  Kgs</th><th style='font-family:Arial;font-size:12;'>Remarks</th>             </tr>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int countnumber = i + 1;
                string PerfInvDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["PerfInvDate"].ToString()).ToString("dd-MM-yyyy");
                if (PerfInvDate == "31-12-9999")
                    PerfInvDate = "";
                string DateofCargoCartatCFS = Convert.ToDateTime(ds.Tables[0].Rows[i]["DateofCargoCartatCFS"].ToString()).ToString("dd-MM-yyyy");
                if (DateofCargoCartatCFS == "31-12-9999")
                    DateofCargoCartatCFS = "";
                string CustsExamStatus = Convert.ToDateTime(ds.Tables[0].Rows[i]["CustsExamStatus"].ToString()).ToString("dd-MM-yyyy");
                if (CustsExamStatus == "31-12-9999")
                    CustsExamStatus = "";
                string ContainerStuffingDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ContainerStuffingDate"].ToString()).ToString("dd-MM-yyyy");
                if (ContainerStuffingDate == "31-12-9999")
                    ContainerStuffingDate = "";
                //string VesselDetailsETAETD = Convert.ToDateTime(ds.Tables[0].Rows[i]["VesselDetailsETAETD"].ToString()).ToString("dd-MM-yyyy");
                //if (VesselDetailsETAETD == "31-12-9999")
                //    VesselDetailsETAETD = "";
                string ECTNReqDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ECTNReqDate"].ToString()).ToString("dd-MM-yyyy");
                if (ECTNReqDate == "31-12-9999")
                    ECTNReqDate = "";
                string ECTNInvRecDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ECTNInvRecDate"].ToString()).ToString("dd-MM-yyyy");
                if (ECTNInvRecDate == "31-12-9999")
                    ECTNInvRecDate = "";
                string ECTNPayDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ECTNPayDate"].ToString()).ToString("dd-MM-yyyy");
                if (ECTNPayDate == "31-12-9999")
                    ECTNPayDate = "";
                string ECTNNoRecDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ECTNNoRecDate"].ToString()).ToString("dd-MM-yyyy");
                if (ECTNNoRecDate == "31-12-9999")
                    ECTNNoRecDate = "";
                string BLInvRecDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BLInvRecDate"].ToString()).ToString("dd-MM-yyyy");
                if (BLInvRecDate == "31-12-9999")
                    BLInvRecDate = "";
                string BLPayDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BLPayDate"].ToString()).ToString("dd-MM-yyyy");
                if (BLPayDate == "31-12-9999")
                    BLPayDate = "";
                string BLAppDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BLAppDate"].ToString()).ToString("dd-MM-yyyy");
                if (BLAppDate == "31-12-9999")
                    BLAppDate = "";
                string BLRelDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["BLRelDate"].ToString()).ToString("dd-MM-yyyy");
                if (BLRelDate == "31-12-9999")
                    BLRelDate = "";
                String BLRecDateAtHYD = Convert.ToDateTime(ds.Tables[0].Rows[i]["BLRecDateAtHYD"].ToString()).ToString("dd-MM-yyyy");
                if (BLRecDateAtHYD == "31-12-9999")
                    BLRecDateAtHYD = "";
                string CerfofOriginFAPCCIDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["CerfofOriginFAPCCIDate"].ToString()).ToString("dd-MM-yyyy");
                if (CerfofOriginFAPCCIDate == "31-12-9999")
                    CerfofOriginFAPCCIDate = "";

                sb.Append("<tr><td  align = 'left' style='font-family:Arial;font-size:12;'>" + countnumber + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;' >" + ds.Tables[0].Rows[i]["ConsigneeName"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["BivacPreShiptInspDetails"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["SuppCargoDesc"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["PerfInvNo"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + PerfInvDate + "</td>" +
                       "<td  align = 'left'style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["ModeofShpt"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["FobValueUSD"].ToString() + "</td>" +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["FreightIns"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["CFRCIFValue"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["PortofLoading"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["PortofDisc"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["Noofpkgs"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["NetWeightinKgs"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["GrossWeightinKgs"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["ShippBillNo"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + DateofCargoCartatCFS + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + CustsExamStatus + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["ContainerNo"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ContainerStuffingDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["VesselDetailsETAETD"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["PartofBLAWB"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["PartofECTNURNID"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ECTNReqDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ECTNInvRecDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ECTNPayDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ECTNNoRecDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + BLInvRecDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + BLPayDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + BLAppDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + BLRelDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + BLRecDateAtHYD + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["CommInvDetails"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + CerfofOriginFAPCCIDate + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["StatusofCommInv"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["DOCDetailsReqConsignee"].ToString() + " </td> " +
                       "<td align = 'left' style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["Remarks"].ToString() + "</td>" +
                       "</tr>");
                //"<td style='font-family:Arial;font-size:12;'>" + ds.Tables[0].Rows[i]["Remarks"].ToString() + " </td> " + "</tr>");
            }

            sb.Append("</table></td></tr>");

            sb.Append("</td></tr></table>");

            string res = sb.ToString();

            return res;
        }


    }
}