using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.IO;
using System.Threading;
using System.Globalization;

namespace VOMS_ERP.Reports
{
    public partial class StmtOfCargoSales_CommercialINVPrepared : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Export to Excel Buttons Click Event

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {

            try
            {
                string ShipmentINVNo = HFExpInvNo.Value;
                string INVFDt = HFExp_INVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFExp_INVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string INVTDt = HFExp_INVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFExp_INVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string ShipmentMode = HFModeofshp.Value;
                string FOBValueINR = HFFOBVal.Value;
                string FRT_Amount = HFFreigh.Value;
                string CIFVal = HFCIFVal.Value;
                string PortOfLoading = HFPOL.Value;
                string PortOfDischarge = HFPOD.Value;
                string TotPkgs = HFNoPkgs.Value;
                string NetWeight = HFGrsWght.Value;
                string GrossWeight = HFNetWght.Value;
                string ShpngBillNo = HFSbno.Value;
                string Sb_FromDate = HFSB_FrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSB_FrmDt.Value).ToString("yyyy-MM-dd");
                string Sb_ToDate = HFSB_ToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSB_ToDt.Value).ToString("yyyy-MM-dd");
                string ContainerNOs = HFContNo.Value;
                string CommercialINVNo = HFCommInvNo.Value;
                string CommercialINVFDt = HFComInv_FrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFComInv_FrmDt.Value).ToString("yyyy-MM-dd");
                string CommercialINVTDt = HFComInv_ToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFComInv_ToDt.Value).ToString("yyyy-MM-dd");

                if (INVFDt == "1-1-0001" || INVFDt == "1-1-1900")
                    INVFDt = "";
                if (INVTDt == "1-1-0001")
                    INVTDt = "";

                if (Sb_FromDate == "1-1-0001" || Sb_FromDate == "1-1-1900")
                    Sb_FromDate = "";
                if (Sb_ToDate == "1-1-0001")
                    Sb_ToDate = "";

                if (CommercialINVFDt == "1-1-0001" || CommercialINVFDt == "1-1-1900")
                    CommercialINVFDt = "";
                if (CommercialINVTDt == "1-1-0001")
                    CommercialINVTDt = "";

                CommercialINVBLL cbll = new CommercialINVBLL();
                DataSet ds = cbll.CommercialINVRpt_Export(ShipmentINVNo, INVFDt, INVTDt, ShipmentMode, FOBValueINR, FRT_Amount, CIFVal, PortOfLoading,
                    PortOfDischarge, TotPkgs, NetWeight, GrossWeight, ShpngBillNo, Sb_FromDate, Sb_ToDate, ContainerNOs, CommercialINVNo,
                    CommercialINVFDt, CommercialINVTDt, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=COMMERCIAL_INVOICE_PREPARED.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (INVFDt != "" && Convert.ToDateTime(INVFDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        INVFDt = "";
                    if (INVTDt != "" && (Convert.ToDateTime(INVTDt).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(INVTDt)) == CommonBLL.EndDtMMddyyyy_FS))
                        INVTDt = "";

                    string MTitle = "STATEMENT OF CARGO - SALES / COMMERCIAL INVOICE PREPARED", MTDTS = "";

                    if (INVFDt != "" && INVTDt != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(INVFDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(INVTDt));
                    else if (INVFDt != "" && INVTDt == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(INVFDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " " + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    dgGrid.RenderControl(htextw);
                    Response.Write(t.Item1);
                    byte[] imge = null;
                    if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["CompanyLogo"].ToString() != "")
                    {
                        imge = (byte[])(ds.Tables[1].Rows[0]["CompanyLogo"]);
                        using (MemoryStream ms = new MemoryStream(imge))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                            string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                                //Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                            image.Save(FilePath);
                        }

                        string headerTable = "<img src='" + CommonBLL.CommonLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    else
                    {
                        string headerTable = "<img src='" + CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    Response.Write(stw.ToString());
                    Response.End();
                }
            }

            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ErrorLog ELog = new ErrorLog();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}