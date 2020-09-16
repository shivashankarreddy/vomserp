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
    public partial class PendingPinkCopies : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Export to Excel Buttons Click Event
        protected void btnExcelExpt_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {
                string FPONO = HFFPONo.Value;
                string Ct1Val = HFCT1No.Value;
                string Ct1_FromDate = HFct1_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFct1_FromDate.Value).ToString("yyyy-MM-dd");
                string Ct1_ToDate = HFct1_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFct1_ToDate.Value).ToString("yyyy-MM-dd");
                string CT1_Form_Mumbai_Hyd = HFCT1FORMISSDMUMHYD.Value;
                string Are1No = HFARE1No.Value;
                string Are_FromDate = HFAre_FromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAre_FromDate.Value).ToString("yyyy-MM-dd");
                string Are_ToDate = HFAre_ToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFAre_ToDate.Value).ToString("yyyy-MM-dd");
                string SBNo = HFSBNo.Value;
                string SB_FromDate = HFSB_FromDATE.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSB_FromDATE.Value).ToString("yyyy-MM-dd");
                string SB_ToDate = HFSB_ToDATE.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFSB_ToDATE.Value).ToString("yyyy-MM-dd");
                string PInvNo = HFINVNo.Value;
                string CHAAgent = HFCHAAgent.Value;
                string SUPPLIERNAME = HFSUPPLIERNAME.Value;
                string AMOUNT = HFAMOUNT.Value;
                string STATUS = HFSTATUS.Value;
                //string ARE1 = HFARE1.Value;
                string ExInvNo = HFExInvNo.Value;
                string Ex_FromDate = HFEx_FromDATE.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFEx_FromDATE.Value).ToString("yyyy-MM-dd");
                string EX_ToDate = HFEx_ToDATE.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFEx_ToDATE.Value).ToString("yyyy-MM-dd");
                if (Ct1_FromDate == "1-1-0001" || Ct1_FromDate == "1-1-1900")
                    Ct1_FromDate = "";
                if (Ct1_ToDate == "1-1-0001")
                    Ct1_ToDate = "";
                if (SB_FromDate == "1-1-0001" || SB_FromDate == "1-1-1900")
                    SB_FromDate = "";
                if (SB_ToDate == "1-1-0001")
                    SB_ToDate = "";
                if (Ex_FromDate == "1-1-0001" || Ex_FromDate == "1-1-1900")
                    Ex_FromDate = "";
                if (EX_ToDate == "1-1-0001")
                    EX_ToDate = "";

                DataSet ds = RPBL.Export_PendngPnkCpies(FPONO, Ct1Val, Ct1_FromDate, Ct1_ToDate, CT1_Form_Mumbai_Hyd, Are1No, Are_FromDate,
                    Are_ToDate, SBNo, SB_FromDate, SB_ToDate, PInvNo, CHAAgent, SUPPLIERNAME, AMOUNT, STATUS, ExInvNo, Ex_FromDate, EX_ToDate,
                    new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "PENDING PINK COPIES - AS ON Dt." + DateTime.Now.Date.ToString("dd.MM.yyyy");
                    string attachment = "attachment; filename=PendingPinkCopies.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (Ct1_FromDate != "" && Convert.ToDateTime(Ct1_FromDate).ToString("dd-MM-yyyy") == "01-01-1900")
                        Ct1_FromDate = "";
                    if (Ct1_ToDate != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(Ct1_ToDate)) == CommonBLL.EndDtMMddyyyy_FS)
                        Ct1_ToDate = "";

                    string MTitle = "STATUS OF PENDING PINK COPIES", MTcustomer = "", MTDTS = "";
                    if (HFSUPPLIERNAME.Value != "")
                        MTcustomer = HFSUPPLIERNAME.Value;
                    if (Ct1_FromDate != "" && Ct1_ToDate != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(Ct1_FromDate)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(Ct1_ToDate));
                    else if (Ct1_FromDate != "" && Ct1_ToDate == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(Ct1_FromDate)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    //else if (FrmDt == "" && ToDat != "")
                    //    MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");

                    DataGrid dgGrid = new DataGrid();
                    //dgGrid.HeaderStyle.Font.Bold = true;
                    //dgGrid.HeaderStyle.Font.Name = "Arial";
                    //dgGrid.HeaderStyle.Font.Size = 9;
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