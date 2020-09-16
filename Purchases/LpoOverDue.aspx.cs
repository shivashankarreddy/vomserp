using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using BAL;
using System.Collections;
using System.Data;
using System.IO;

namespace VOMS_ERP.Purchases
{
    public partial class LpoOverDue : System.Web.UI.Page
    {

        #region Variables
        ErrorLog ELog = new ErrorLog();
        LPOrdersBLL NLPOBL = new LPOrdersBLL();
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Export Excel
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
                string FrmDt = "", ToDat = "", CreatedDT = "";
                Guid LoginID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString() ||
                    CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) && Mode != null)
                    LoginID = new Guid(((ArrayList)Session["UserDtls"])[1].ToString());

                if (Mode == "tldt")
                {
                    FrmDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                    ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "tdt")
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                else
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                }

                string LPO = HFLPONo.Value;
                string FPONo = HFFPONo.Value;
                string Subject = HFSubject.Value;
                string Status = HFStatus.Value;
                string supplier = HFSupplier.Value;
                string createdByName = HFCreatedBy.Value;
                string DelFrmDt = "";
                string DelToDt = "";

                DelFrmDt = HFDelFrmDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFDelFrmDate.Value).ToString("yyyy-MM-dd");
                DelToDt = HFDelToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFDelToDate.Value).ToString("yyyy-MM-dd");

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NLPOBL.LPOOverDue_Search(FrmDt, ToDat, LPO.Replace("'", "''"), FPONo.Replace("'", "''"), Subject.Replace("'", "''"),
                    Status.Replace("'", "''"), supplier.Replace("'", "''"), DelFrmDt, DelToDt, CreatedDT, LoginID, Mode,
                    new Guid(Session["CompanyID"].ToString()), createdByName);
                ds.Tables[0].Columns.Remove(ds.Tables[0].Columns["CreatedDate"]);

                if (ds != null && ds.Tables.Count > 0)
                {
                    //string Title = "STATUS OF PURCHASE ORDERS OVER DUE";
                    string attachment = "attachment; filename=LPOOverDue.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF LOCAL PURCHASE ORDERS OVER DUE ", MTDTS = "";
                    //if (HFCust.Value != "")
                    //    MTcustomer = HFCust.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                              + (MTDTS != "" ? MTDTS : "ON" + CreatedDT + "</center></b>"));

                    // htextw.Write("<center><b>" + Title + "</b></center>");
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
                            string FilePath = Server.MapPath("../images/Logos/" + Session["AliasName"].ToString() + ".png");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }
        #endregion
    }
}