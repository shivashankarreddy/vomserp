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

namespace VOMS_ERP.Reports
{
    public partial class StatusofFpo : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Export to Excel Buttons Click Event

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string FpoNo = HFFPONO.Value;
                string FpoFrmDate = HFFpoFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFpoFrmDt.Value).ToString("yyyy-MM-dd");
                string FpoToDate = HFFpoToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFpoToDate.Value).ToString("yyyy-MM-dd");
                string FENO = HFFENO.Value;
                string FEFrmDt = HFFEFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFEFrmDt.Value).ToString("yyyy-MM-dd");
                string FEToDt = HFFEToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFEToDt.Value).ToString("yyyy-MM-dd");
                string FQNO = HFFQNO.Value;
                string FQFrmDt = HFFQFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFQFrmDt.Value).ToString("yyyy-MM-dd");
                string FQToDt = HFFQToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFQToDt.Value).ToString("yyyy-MM-dd");
                string TotAmt = HFFQAmount.Value;
                string Customer = HFCust.Value;
                string Stat = HFStat.Value;
                string Remarks = HFRemarks.Value;

                if (FpoFrmDate == "1-1-0001" || FpoFrmDate == "1-1-1900")
                    FpoFrmDate = "";
                if (FpoToDate == "1-1-0001")
                    FpoToDate = "";
                if (FEFrmDt == "1-1-0001" || FEFrmDt == "1-1-1900")
                    FEFrmDt = "";
                if (FEToDt == "1-1-0001")
                    FEToDt = "";
                if (FQFrmDt == "1-1-0001" || FQFrmDt == "1-1-1900")
                    FQFrmDt = "";
                if (FQToDt == "1-1-0001")
                    FQToDt = "";
                DataSet ds = RPBL.Export_FPOStatus(FpoNo, FpoFrmDate, FpoToDate, FENO, FEFrmDt, FEToDt, FQNO, FQFrmDt, FQToDt, TotAmt, Customer, Stat, Remarks, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {

                    string attachment = "attachment; filename=Fpo'sStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FpoFrmDate != "" && Convert.ToDateTime(FpoFrmDate).ToString("dd-MM-yyyy") == "01-01-1900")
                        FpoFrmDate = "";
                    if (FpoToDate != "" && (Convert.ToDateTime(FpoToDate).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(FpoToDate)) == CommonBLL.EndDtMMddyyyy_FS))
                        FpoToDate = "";

                    string MTitle = "DETAILS OF FPO RECEIVED", MTcustomer = "", MTDTS = "", MTAW = " AND AWAITED LPO's ";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FpoFrmDate != "" && FpoToDate != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FpoFrmDate)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(FpoToDate));
                    else if (FpoFrmDate != "" && FpoToDate == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FpoFrmDate)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " " + (MTcustomer != "" ? " TO " + MTcustomer.ToUpper() : "") + "" + MTAW + MTDTS + "</center></b>");

                    DataGrid dgGrid = new DataGrid();
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "FPO Awaited Status", ex.Message.ToString());
            }
        }

        #endregion

    }
}