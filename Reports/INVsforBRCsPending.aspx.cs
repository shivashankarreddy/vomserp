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
    public partial class INVsforBRCsPending : System.Web.UI.Page
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
            lblTitle.Text = lblTitle.Text + DateTime.Now.ToString("dd.MM.yyyy");
        }

        #endregion

        #region Export to Excel Buttons Click Event

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string INVOICE_No = HFINVOICE_No.Value;
                string INVOICE_FromDate = HFPROFORMA_INVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string INVOICE_ToDate = HFPROFORMA_INVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string PACKING_No = HFPACKING_No.Value;
                string PKNG_LIST_FromDate = HFPKNG_LIST_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPKNG_LIST_FromDate.Value).ToString("yyyy-MM-dd");
                string PKNG_LIST_ToDate = HFPKNG_LIST_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPKNG_LIST_ToDate.Value).ToString("yyyy-MM-dd");
                string AWB_BL_No = HFAWB_BL_No.Value;
                string AWB_BL_FromDate = HFAWB_BL_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAWB_BL_FromDate.Value).ToString("yyyy-MM-dd");
                string AWB_BL_ToDate = HFAWB_BL_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAWB_BL_ToDate.Value).ToString("yyyy-MM-dd");
                string SBILL_No = HFSBILL_No.Value;
                string Sb_FromDate = HFSb_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_FromDate.Value).ToString("yyyy-MM-dd");
                string Sb_ToDate = HFSb_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_ToDate.Value).ToString("yyyy-MM-dd");
                string PORT = HFPORT.Value;
                string CIF_AMOUNT = HFCIF_AMOUNT.Value;
                string FRIEGHT = HFFRIEGHT.Value;
                string FOB_VALUE = HFFOB_VALUE.Value;
                string NAME_OF_THE_PARTY = HFNAME_OF_THE_PARTY.Value;
                string TO_PORT = HFTO_PORT.Value;


                if (INVOICE_FromDate == "1-1-0001" || INVOICE_FromDate == "1-1-1900")
                    INVOICE_FromDate = "";
                if (INVOICE_ToDate == "1-1-0001")
                    INVOICE_ToDate = "";
                if (PKNG_LIST_FromDate == "1-1-0001" || PKNG_LIST_FromDate == "1-1-1900")
                    PKNG_LIST_FromDate = "";
                if (PKNG_LIST_ToDate == "1-1-0001")
                    PKNG_LIST_ToDate = "";
                if (AWB_BL_FromDate == "1-1-0001" || AWB_BL_FromDate == "1-1-1900")
                    AWB_BL_FromDate = "";
                if (AWB_BL_ToDate == "1-1-0001")
                    AWB_BL_ToDate = "";
                if (Sb_FromDate == "1-1-0001" || Sb_FromDate == "1-1-1900")
                    Sb_FromDate = "";
                if (Sb_ToDate == "1-1-0001")
                    Sb_ToDate = "";

                DataSet ds = RPBL.Export_INVsforBRCsPending(INVOICE_No, INVOICE_FromDate, INVOICE_ToDate, PACKING_No, PKNG_LIST_FromDate, PKNG_LIST_ToDate,
                    AWB_BL_No, AWB_BL_FromDate, AWB_BL_ToDate, SBILL_No, Sb_FromDate, Sb_ToDate, PORT, CIF_AMOUNT, FRIEGHT, FOB_VALUE, NAME_OF_THE_PARTY,
                    TO_PORT, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "LIST OF INVOICES FOR WHICH BRC's ARE PENDING - AS ON Dt." + DateTime.Now.Date.ToString("dd.MM.yyyy");
                    string attachment = "attachment; filename=INVsForBRCsPending.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (INVOICE_FromDate != "" && Convert.ToDateTime(INVOICE_FromDate).ToString("dd-MM-yyyy") == "01-01-1900")
                        INVOICE_FromDate = "";
                    if (INVOICE_ToDate != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(INVOICE_ToDate)) == CommonBLL.EndDtMMddyyyy_FS)
                        INVOICE_ToDate = "";

                    string MTitle = " LIST OF INVOICES FOR WHICH BRC's ARE PENDING ", MTDTS = "";
                    if (HFPROFORMA_INVOICE_FromDate.Value != "" && HFPROFORMA_INVOICE_ToDate.Value != "")
                        MTDTS = " DURING " + HFPROFORMA_INVOICE_FromDate.Value + " TO " + HFPROFORMA_INVOICE_ToDate.Value;
                    else if (HFPROFORMA_INVOICE_FromDate.Value != "" && HFPROFORMA_INVOICE_ToDate.Value == "")
                        MTDTS = " DURING " + HFPROFORMA_INVOICE_FromDate.Value + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
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