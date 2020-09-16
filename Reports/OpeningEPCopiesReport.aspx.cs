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
    public partial class OpeningEPCopiesReport : System.Web.UI.Page
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

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string COMMERCIAL_INVOICE_No = HFCOMMERCIAL_INVOICE_No.Value;
                string COMMERCIAL_INVOICE_FromDate = HFCOMMERCIAL_INVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCOMMERCIAL_INVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string COMMERCIAL_INVOICE_ToDate = HFCOMMERCIAL_INVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCOMMERCIAL_INVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string PROFORMA_INVOICE_No = HFPROFORMA_INVOICE_No.Value;
                string PROFORMA_INVOICE_FromDate = HFPROFORMA_INVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string PROFORMA_INVOICE_ToDate = HFPROFORMA_INVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPROFORMA_INVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string AWB_BL_No = HFAWB_BL_No.Value;
                string AWB_BL_FromDate = HFAWB_BL_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAWB_BL_FromDate.Value).ToString("yyyy-MM-dd");
                string AWB_BL_ToDate = HFAWB_BL_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAWB_BL_ToDate.Value).ToString("yyyy-MM-dd");
                string Sb_no = HFSb_no.Value;
                string Sb_FromDate = HFSb_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_FromDate.Value).ToString("yyyy-MM-dd");
                string Sb_ToDate = HFSb_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSb_ToDate.Value).ToString("yyyy-MM-dd");
                string Sb_Staus = HFSb_Staus.Value;

                string No_Pages = HFNo_Pages.Value;
                string No_ARE_Forms = HFNo_ARE_Forms.Value;
                string ARE_Form_Status = HFARE_Form_Status.Value;
                string Load_Port = HFLoad_Port.Value;
                string Discharge_Port = HFDischarge_Port.Value;
                string Cha_Agent = HFCha_Agent.Value;

                if (COMMERCIAL_INVOICE_FromDate == "1-1-0001" || COMMERCIAL_INVOICE_FromDate == "1-1-1900")
                    COMMERCIAL_INVOICE_FromDate = "";
                if (COMMERCIAL_INVOICE_ToDate == "1-1-0001")
                    COMMERCIAL_INVOICE_ToDate = "";
                if (PROFORMA_INVOICE_FromDate == "1-1-0001" || PROFORMA_INVOICE_FromDate == "1-1-1900")
                    PROFORMA_INVOICE_FromDate = "";
                if (PROFORMA_INVOICE_ToDate == "1-1-0001")
                    PROFORMA_INVOICE_ToDate = "";
                if (AWB_BL_FromDate == "1-1-0001" || AWB_BL_FromDate == "1-1-1900")
                    AWB_BL_FromDate = "";
                if (AWB_BL_ToDate == "1-1-0001")
                    AWB_BL_ToDate = "";
                if (Sb_FromDate == "1-1-0001" || Sb_FromDate == "1-1-1900")
                    Sb_FromDate = "";
                if (Sb_ToDate == "1-1-0001")
                    Sb_ToDate = "";

                DataSet ds = RPBL.Export_OpeningEPCopies(COMMERCIAL_INVOICE_No, COMMERCIAL_INVOICE_FromDate, COMMERCIAL_INVOICE_ToDate, PROFORMA_INVOICE_No,
                    PROFORMA_INVOICE_FromDate, PROFORMA_INVOICE_ToDate, Sb_no, Sb_FromDate, Sb_ToDate, AWB_BL_No, AWB_BL_FromDate, AWB_BL_ToDate, Sb_Staus,
                    No_Pages, No_ARE_Forms, ARE_Form_Status, Load_Port, Discharge_Port, Cha_Agent, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "STATEMENT OF PENDING EP COPIES / SHIPPING BILLS / ARE-1 FORMS - AS ON Dt." + DateTime.Now.Date.ToString("dd.MM.yyyy");
                    string attachment = "attachment; filename=OpeningEPCopies.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    //htextw.Write("<center><b>" + Title + "</b></center>");

                    if (COMMERCIAL_INVOICE_FromDate != "" && Convert.ToDateTime(COMMERCIAL_INVOICE_FromDate).ToString("dd-MM-yyyy") == "01-01-1900")
                        COMMERCIAL_INVOICE_FromDate = "";
                    if (COMMERCIAL_INVOICE_ToDate != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(COMMERCIAL_INVOICE_ToDate)) == CommonBLL.EndDtMMddyyyy_FS)
                        COMMERCIAL_INVOICE_ToDate = "";

                    string MTitle = "STATEMENT OF PENDING EP COPIES / SHIPPING BILLS / ARE-1 FORMS - ", MTDTS = "";
                    //if (HFCust.Value != "")
                    //    MTcustomer = HFCust.Value;
                    if (COMMERCIAL_INVOICE_FromDate != "" && COMMERCIAL_INVOICE_FromDate != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(COMMERCIAL_INVOICE_FromDate)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(COMMERCIAL_INVOICE_ToDate));
                    else if (COMMERCIAL_INVOICE_FromDate != "" && COMMERCIAL_INVOICE_ToDate == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(COMMERCIAL_INVOICE_FromDate)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                              + MTDTS + "</center></b>");

                    DataGrid dgGrid = new DataGrid();
                    //dgGrid.HeaderStyle.Font.Bold = true;
                    //dgGrid.HeaderStyle.Font.Name = "Arial";
                    //dgGrid.HeaderStyle.Font.Size = 10;
                    //dgGrid.Font.Name = "Arial";
                    //dgGrid.Font.Size = 9;
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    dgGrid.RenderControl(htextw);
                    //Response.Write(CommonBLL.AddExcelStyling());
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