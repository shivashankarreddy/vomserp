using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using BAL;
using System.IO;

namespace VOMS_ERP.Reports
{
    public partial class FPOAwaitedaspx : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        #endregion

        #region Default Page Load Evnet

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Export to Excel Buttons Click Event
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string FENO = HFFENO.Value;
                string FEFrmDt = HFFEFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFEFrmDt.Value).ToString("yyyy-MM-dd"); //HFFEFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateYearFrmt(HFFEFrmDt.Value).ToString("yyyy-MM-dd");
                string FEToDt = HFFEToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFEToDt.Value).ToString("yyyy-MM-dd");//HFFEToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateYearFrmt(HFFEToDt.Value).ToString("yyyy-MM-dd");
                string FQNO = HFFQNO.Value;
                string FQFrmDt = HFFQFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFQFrmDt.Value).ToString("yyyy-MM-dd"); //HFFQFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateYearFrmt(HFFQFrmDt.Value).ToString("yyyy-MM-dd");
                string FQToDt = HFFQToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFQToDt.Value).ToString("yyyy-MM-dd");//HFFQToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateYearFrmt(HFFQToDt.Value).ToString("yyyy-MM-dd");
                string TotAmt = HFFQAmount.Value;
                string Customer = HFCust.Value;
                string Stat = HFStat.Value;
                string Remarks = HFRemarks.Value;
                string CreatedBy = HFCreatBy.Value;
                string Comments = HFComments.Value;


                if (FEFrmDt == "1-1-0001" || FEFrmDt == "1-1-1900")
                    FEFrmDt = "";
                if (FEToDt == "1-1-0001")
                    FEToDt = "";
                if (FQFrmDt == "1-1-0001" || FQFrmDt == "1-1-1900")
                    FQFrmDt = "";
                if (FQToDt == "1-1-0001")
                    FQToDt = "";
                DataSet ds = RPBL.Export_FPOAwaited(FENO, FEFrmDt, FEToDt, FQNO, FQFrmDt, FQToDt, TotAmt, Customer, Stat, Remarks, CreatedBy, Comments,new Guid(Session["CompanyID"].ToString()));
                ds.Tables[0].Columns.Remove("StatusTypeId");
                if (ds != null && ds.Tables.Count > 0)
                {

                    string attachment = "attachment; filename=FposAwaited.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FQFrmDt != "" && Convert.ToDateTime(FQFrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FQFrmDt = "";
                    if (FQToDt != "" && (Convert.ToDateTime(FQToDt).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(FQToDt)) == CommonBLL.EndDtMMddyyyy_FS))
                        FQToDt = "";


                    string MTitle = "DETAILS OF FOREIGN QUOTATIONS SENT", MTcustomer = "", MTDTS = "", MTAW = " AND AWAITED FPO's ";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FQFrmDt != "" && FQToDt != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FQFrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(FQToDt));
                    else if (FQFrmDt != "" && FQToDt == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FQFrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                              + (MTcustomer != "" ? " TO " + MTcustomer.ToUpper() : "") + "" + MTAW
                                              + MTDTS + "</center></b>");

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

        /// <summary>
        /// Export Verify Rendering
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}
