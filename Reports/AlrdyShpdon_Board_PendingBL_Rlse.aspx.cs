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
    public partial class AlrdyShpdon_Board_PendingBL_Rlse : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                lblTitle.Text = lblTitle.Text + DateTime.Now.ToString("dd.MM.yyyy") + " [[[ WEEK No. " +
                   CultureInfo.InstalledUICulture.DateTimeFormat.Calendar.GetWeekOfYear(DateTime.Now,
                   CalendarWeekRule.FirstDay, CultureInfo.InstalledUICulture.DateTimeFormat.FirstDayOfWeek).ToString() + "]]]";
            }

        }

        #endregion

        #region Export to Excel Buttons Click Event

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string INVOICE_No = HFINVOICE_No.Value;
                string INVOICE_FromDate = HFINVOICE_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFINVOICE_FromDate.Value).ToString("yyyy-MM-dd");
                string INVOICE_ToDate = HFINVOICE_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFINVOICE_ToDate.Value).ToString("yyyy-MM-dd");
                string Type_of_Consignment = HFType_of_Consignment.Value;
                string Amount_in_USD_FOB = HFAmount_in_USD_FOB.Value;
                string Freight = HFFreight.Value;
                string Cost_and_Freight = HFCost_and_Freight.Value;
                string POL = HFPOL.Value;
                string POD = HFPOD.Value;
                string No_of_Pkgs = HFNo_of_Pkgs.Value;
                string Gross_Weight = HFGross_Weight.Value;
                string Net_Weight = HFNet_Weight.Value;
                string ShpngBl_No = HFShpngBl_No.Value;
                string Sb_FromDate = HFShpngBl_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFShpngBl_FromDate.Value).ToString("yyyy-MM-dd");
                string Sb_ToDate = HFShpngBl_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFShpngBl_ToDate.Value).ToString("yyyy-MM-dd");
                string Container_No = HFContainer_No.Value;
                string BL_No_AWB_No = HFBL_No_AWB_No.Value;
                string BL_No_AWB_FromDate = HFBL_No_AWB_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFBL_No_AWB_FromDate.Value).ToString("yyyy-MM-dd");
                string BL_No_AWB_ToDate = HFBL_No_AWB_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFBL_No_AWB_ToDate.Value).ToString("yyyy-MM-dd");
                string Contact_Person = HFContact_Person.Value;
                string Remarks = HFRemarks.Value;
                string VESSEL_Details = HFVESSEL_Details.Value;

                if (INVOICE_FromDate == "1-1-0001" || INVOICE_FromDate == "1-1-1900")
                    INVOICE_FromDate = "";
                if (INVOICE_ToDate == "1-1-0001")
                    INVOICE_ToDate = "";

                if (Sb_FromDate == "1-1-0001" || Sb_FromDate == "1-1-1900")
                    Sb_FromDate = "";
                if (Sb_ToDate == "1-1-0001")
                    Sb_ToDate = "";

                if (BL_No_AWB_FromDate == "1-1-0001" || BL_No_AWB_FromDate == "1-1-1900")
                    BL_No_AWB_FromDate = "";
                if (BL_No_AWB_ToDate == "1-1-0001")
                    BL_No_AWB_ToDate = "";


                DataSet ds = RPBL.Export_AlrdyShpdon_Board_PendingBL_Rlse(INVOICE_No, INVOICE_FromDate, INVOICE_ToDate, Type_of_Consignment, Amount_in_USD_FOB, Freight,
                    Cost_and_Freight, POL, POD, No_of_Pkgs, Gross_Weight, Net_Weight, ShpngBl_No, Sb_FromDate, Sb_ToDate, Container_No, BL_No_AWB_No, BL_No_AWB_FromDate,
                    BL_No_AWB_ToDate, Contact_Person, Remarks, VESSEL_Details, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "STATEMENT OF CARGO - ALREADY SHIPPED ON BOARD / PENDING BILL OF LADING RELEASES - AS ON Dt." + DateTime.Now.Date.ToString("dd.MM.yyyy")
                        + " [[[ WEEK No. " +
                   CultureInfo.InstalledUICulture.DateTimeFormat.Calendar.GetWeekOfYear(DateTime.Now,
                   CalendarWeekRule.FirstDay, CultureInfo.InstalledUICulture.DateTimeFormat.FirstDayOfWeek).ToString() + "]]]";
                    string attachment = "attachment; filename=AlrdyShpdon_Board_PendingBL_Rlse.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    htextw.Write("<center><b>" + Title + "</b></center>");
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