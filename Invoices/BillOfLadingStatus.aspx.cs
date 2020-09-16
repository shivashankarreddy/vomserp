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
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class BillOfLadingStatus : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        BillOfLadingBLL BlBLL = new BillOfLadingBLL();
        # endregion

        #region Default Page Load Events

        /// <summary>
        /// Default Page Load Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(BillOfLadingStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    //  txtFrmDt.Attributes.Add("readonly", "readonly");
                    //  txtToDt.Attributes.Add("readonly", "readonly");
                    if (!IsPostBack)
                        Search();
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        #endregion

        # region MEthods

        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("BLID");
                dt.Columns.Add("BillofLadingNo");
                dt.Columns.Add("Customers");
                dt.Columns.Add("ShpngPrfmaInvcNmbr");
                dt.Columns.Add("Frieght");
                dt.Columns.Add("PlaceOfDelivery");
                dt.Columns.Add("CreatedBy");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                // GVBillOfLading.DataSource = ds;
                //  GVBillOfLading.DataBind();
                // int columncount = GVBillOfLading.Rows[0].Cells.Count;
                // for (int i = 0; i < columncount; i++)
                //     GVBillOfLading.Rows[0].Cells[i].Style.Add("display", "none");

                // GVBillOfLading.Rows[0].Cells.Add(new TableCell());
                // GVBillOfLading.Rows[0].Cells[columncount].ColumnSpan = columncount;
                //  GVBillOfLading.Rows[0].Cells[columncount].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Bill of lading Status", ex.Message.ToString());
            }
        }

        private void BindGridView(DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    //GVBillOfLading.DataSource = ds.Tables[0];
                    // GVBillOfLading.DataBind();
                }
                else
                    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        private void Search()
        {
            try
            {
                //if (txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //  BindGridView(BlBLL.GetDataSet(CommonBLL.FlagSelectAll, 0, CommonBLL.StartDate, CommonBLL.EndDate));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", ex.Message.ToString());
            }
        }

        # endregion

        #region Grid View Events

        /// <summary>
        /// Grid Veiw Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVBillOfLading_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // int res = 1;
                // int index = Convert.ToInt32(e.CommandArgument);
                //// string BLID = GVBillOfLading.DataKeys[index].Values["BLID"].ToString();
                // if (e.CommandName == "Modify")
                // //    Response.Redirect("../Invoices/BillLadingEntry.aspx?ID=" + BLID, false);
                //// else if (e.CommandName == "Remove")
                //// {
                //     AirWayBillBLL AWBLL = new AirWayBillBLL();
                //    // res = AWBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(BLID));
                //     if (res == 0)
                //     {
                //         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Successfully Delete this Record.');", true);
                //         ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Bill Of Lading Status", "Deleted Record " + BLID + " Successfully.");
                //         Search();
                //     }
                //     else
                //     {
                //         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Cannot Delete this Record.');", true);
                //         ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill Of Lading Status", "Cannot Delete Record " + BLID + ".");
                //     }
                // }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill Of Lading Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View PreRender Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVBillOfLading_PreRender(object sender, EventArgs e)
        {
            try
            {
                // GVBillOfLading.UseAccessibleHeader = false;
                //   GVBillOfLading.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        #endregion

        #region Export Buttons Click Events

        /// <summary>
        /// Export to Excel Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string BOLNo = HFBOLNo.Value;
                string Cust = HFCus.Value;
                string PrfrmInvNo = HFPInvNo.Value;
                string Freight = HFFreig.Value;
                string PlcOfDelvry = HFPODelvry.Value;

                DataSet ds = BlBLL.GetDataSetForExport(BOLNo, Cust, PrfrmInvNo, Freight, PlcOfDelvry, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=BillOfLadingStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    string MTitle = "STATUS OF BILL OF LADING", MTDTS = "", MTCust = "";
                    if (HFCus.Value != "")
                        MTCust = HFCus.Value;
                    MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " " + (MTCust != "" ? " FOR " + MTCust.ToUpper() : "") + "" + MTDTS + "</center></b>");
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
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", ex.Message.ToString());
            }

        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string CompanyId)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                if (result == "Success")
                {
                    DataSet EditDS = BlBLL.GetDataSet(CommonBLL.FlagSelectAll, new Guid(ID), CommonBLL.StartDate, CommonBLL.EndDate);
                    string Stat = "";
                    if (EditDS != null && EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        Stat = EditDS.Tables[0].Rows[0]["StatusID"].ToString();
                    if (Stat == "" || Convert.ToInt32(Stat) >= 87) //["StatusID"]
                        res = -123;
                    else
                    {
                        res = BlBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), CommonBLL.EmptyDTBillOfladingContainer());
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, Another Transaction is using this record";
                }

                //if (result == "Success")
                //{
                //    DataSet Bol = new DataSet();
                //    Bol = BlBLL.GetDataSet(CommonBLL.FlagSelectAll, Convert.ToInt64(ID), CommonBLL.StartDate, CommonBLL.EndDate);
                //    if (Bol != null && Bol.Tables.Count >= 0 && Bol.Tables[0].Rows.Count > 0)
                //        res = -123;
                //    else
                //    {
                //        res = BlBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(ID));
                //    }
                //    if (res == 0)
                //        result = "Success::Deleted Successfully";
                //    else
                //        result = "Error::Cannot Delete this Record/ Error while Deleting " + ID;
                //}
                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Proforma Invoice Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

    }
}