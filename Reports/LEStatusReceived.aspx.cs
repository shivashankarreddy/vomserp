using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading;
namespace VOMS_ERP.Reports
{
    public partial class LEStatusReceived : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        FEnquiryBLL frnfenq = new FEnquiryBLL();
        BAL.LEnquiryBLL NLEBL = new LEnquiryBLL();
        #endregion

        #region PageLoad

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["UserID"] == null || Session["UserID"].ToString() == "")
            //    Response.Redirect("../Login.aspx?logout=yes", false);
            //else
            //{
            //    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
            //    {
            //        if (!IsPostBack)
            //            Getdata();
            //    }
            //    else
            //        Response.Redirect("../Masters/Home.aspx?NP=no", false);
            //}
        }

        #endregion

        #region GetData

        protected void Getdata()
        {

        }

        #endregion

        #region Export To Excel

        /// <summary>
        /// Export to Excel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = ""; Guid LoginID = Guid.Empty;
                string FromRcvdDt = "", ToRcvdDt = "";
                FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                FromRcvdDt = HFRcvdFromDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRcvdFromDt.Value).ToString("yyyy-MM-dd");
                ToRcvdDt = HFRcvdToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRcvdToDt.Value).ToString("yyyy-MM-dd");

                string FENo = HFFENo.Value;
                string FloatedTo = HFFloatedTo.Value;
                string FloatedNo = HFFloatedNo.Value;
                string Comments = HFComments.Value;
                string Cust = HFCust.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                if (FromRcvdDt == "1-1-0001" || FromRcvdDt == "1-1-1900")
                    FromRcvdDt = "";
                if (ToRcvdDt == "1-1-0001")
                    ToRcvdDt = "";
                DataSet ds = NLEBL.LE_Search(FrmDt, ToDat, FloatedNo, FENo, "", "", FloatedTo, Cust, CreatedDT, LoginID, Comments, FromRcvdDt, ToRcvdDt, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=ForeignEnquirystatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF PENDING LOCAL ENQUIRIES  ", MTcustomer = "", MTDTS = "";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    htextw.Write("<center><b>" + MTitle + " " + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + "" + MTDTS + "</center></b>");
                    ds.Tables[0].Columns.Remove("Subject");
                    ds.Tables[0].Columns.Remove("Status");
                    ds.Tables[0].Columns.Remove("IsActive");
                    ds.Tables[0].AcceptChanges();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
        }

        #endregion
    }
}