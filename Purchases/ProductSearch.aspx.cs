using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using BAL;
using System.IO;
using System.Threading;

namespace VOMS_ERP.Purchases
{
    public partial class ProductSearch : System.Web.UI.Page
    {
        #region Variables
        ComparisonStmntBLL CSBL = new ComparisonStmntBLL();
        ErrorLog ELog = new ErrorLog();
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ProductSearch));
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Product Search", ex.Message.ToString());
            }

        }

        #endregion

        #region Bind Data

        /// <summary>
        /// Bind Defaul Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                // BindGridView(gvPSItms, CSBL.SelectBasketItems(CommonBLL.FlagXSelect, 0, 0, 0, 0, 0, 0, 0, 0, CommonBLL.UserID,""));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Product Search", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GridView Item Details
        /// </summary>
        /// <param name="gvitms"></param>
        /// <param name="CommonDs"></param>
        protected void BindGridView(GridView gvitms, DataSet CommonDs)
        {
            try
            {
                if (CommonDs.Tables.Count > 0 && CommonDs.Tables[0].Rows.Count > 0)
                {
                    gvitms.DataSource = CommonDs;
                    gvitms.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Product Search", ex.Message.ToString());
            }
        }

        #endregion

        #region

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPSItms_PreRender(object sender, EventArgs e)
        {
            try
            {
                //  gvPSItms.UseAccessibleHeader = false;
                // gvPSItms.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Export 2 Excel

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "", STF = "";
                int LoginID = 0, CusID = 0;

                string Supplier = HFSuplr.Value;
                string PONo = HFPONo.Value;
                string Desc = HFDesc.Value;
                string PartNo = HFPartNo.Value;
                string Make = HFmake.Value;
                string Discount = HFDisc.Value;
                string Rate = HFRate.Value;
                string CreatedBy = HFCreatedBy.Value;

                FrmDt = HFFrmDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFrmDate.Value).ToString("yyyy-MM-dd");
                ToDat = HFToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDt.Value).ToString("yyyy-MM-dd");

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                LPOrdersBLL lpoBLL = new LPOrdersBLL();
                DataSet ds = lpoBLL.Product_Search_Exp(Supplier, PONo, FrmDt, ToDat, Desc, PartNo, Make, Discount, Rate, CreatedBy, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    // string Title = "STATUS OF FOREIGN PURCHASE ORDERS RECEIVED";
                    string attachment = "attachment; filename=FPOStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF PRODUCT SEARCH ", MTcustomer = "", MTDTS = "";
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                              + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                              + (MTDTS != "" ? MTDTS : "ON" + CreatedDT + "</center></b>"));

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
                            string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                            image.Save(FilePath);
                        }

                        string headerTable = "<img src='" + Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png") + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    else
                    {
                        string headerTable = "<img src='" + Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\Admin.jpg" + "'margin-top =16px width=125 height=35 />");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Product Search Status", ex.Message.ToString());
            }
        }

        #endregion
    }
}
