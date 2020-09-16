using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Threading;
using System.IO;


namespace VOMS_ERP.Reports
{
    public partial class DutyDrawBackAmountReport : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #endregion

        #region Export to Excel Buttons Click Event

        protected void btnExcelExpt_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {
                string COMMERCIAL_INVOICENO = HFCOMMERCIAL_INVOICENO.Value;
                string COMMERCIAL_InvFromDate = HFCOMMERCIAL_InvFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCOMMERCIAL_InvFromDate.Value).ToString("yyyy-MM-dd");
                string COMMERCIAL_InvToDate = HFCOMMERCIAL_InvToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCOMMERCIAL_InvToDate.Value).ToString("yyyy-MM-dd");
                string PROFORMA_INVOICE_No = HFINVOICE_No.Value;
                string PROFORMA_INVOICE_FromDate = HFPROFORMA_INVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string PROFORMA_INVOICE_ToDate = HFPROFORMA_INVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string Sb_no = HFSBILL_No.Value;
                string Sb_FromDate = HFSb_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_FromDate.Value).ToString("yyyy-MM-dd");
                string Sb_ToDate = HFSb_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_ToDate.Value).ToString("yyyy-MM-dd");
                string Load_Port = HFPORTOF_SHIPMENT.Value;
                string Discharge_Port = HFPORTOF_DISCH.Value;
                string Cha_Agent = HFCHA_AGENT.Value;
                string Duty_DBAMOUNT = HFDUTY_DB_AMT_SB.Value;
                string Duty_DBAmtRec = HFDUTY_DB_AMTRECVD.Value;
                string Remarks = HFREMARKS.Value;
                string Action_Taken = HFACTION_DETAILS.Value;
                if (PROFORMA_INVOICE_FromDate == "1-1-0001" || PROFORMA_INVOICE_FromDate == "1-1-1900")
                    PROFORMA_INVOICE_FromDate = "";
                if (PROFORMA_INVOICE_ToDate == "1-1-0001")
                    PROFORMA_INVOICE_ToDate = "";
                if (Sb_FromDate == "1-1-0001" || Sb_FromDate == "1-1-1900")
                    Sb_FromDate = "";
                if (Sb_ToDate == "1-1-0001")
                    Sb_ToDate = "";

                DataSet ds = RPBL.Export_DutyDrawBack(COMMERCIAL_INVOICENO, COMMERCIAL_InvFromDate, COMMERCIAL_InvToDate, PROFORMA_INVOICE_No,
                    PROFORMA_INVOICE_FromDate, PROFORMA_INVOICE_ToDate, Sb_no, Sb_FromDate, Sb_ToDate, Load_Port, Discharge_Port, Cha_Agent,
                    Duty_DBAMOUNT, Duty_DBAmtRec, Remarks, Action_Taken, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "DUTY DRAWBACK AMOUNT - AS ON Dt." + DateTime.Now.Date.ToString("dd.MM.yyyy");
                    string attachment = "attachment; filename=DutyDrawBackAmount.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (PROFORMA_INVOICE_FromDate != "" && Convert.ToDateTime(PROFORMA_INVOICE_FromDate).ToString("dd-MM-yyyy") == "01-01-1900")
                        PROFORMA_INVOICE_FromDate = "";
                    if (PROFORMA_INVOICE_ToDate != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(PROFORMA_INVOICE_ToDate)) == CommonBLL.EndDtMMddyyyy_FS)
                        PROFORMA_INVOICE_ToDate = "";

                    string MTitle = "STATUS OF DutyDrawBack Amount  ", MTDTS = "";
                    if (PROFORMA_INVOICE_FromDate != "" && PROFORMA_INVOICE_ToDate != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(PROFORMA_INVOICE_FromDate)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(PROFORMA_INVOICE_ToDate));
                    else if (PROFORMA_INVOICE_FromDate != "" && PROFORMA_INVOICE_ToDate == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(PROFORMA_INVOICE_FromDate)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}