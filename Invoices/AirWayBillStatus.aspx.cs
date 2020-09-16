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
using System.Collections;

namespace VOMS_ERP.Invoices
{
    public partial class AirWayBillStatus : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        AirWayBillBLL AWBLL = new AirWayBillBLL();
        # endregion

        #region Default Page Load Events

        /// <summary>
        /// Default Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(AirWayBillStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (!IsPostBack)
                    {
                        //txtFrmDt.Attributes.Add("readonly", "readonly");
                        //txtToDt.Attributes.Add("readonly", "readonly");
                        Search();
                    }
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
                dt.Columns.Add("AWBID");
                dt.Columns.Add("AWBNumber");
                dt.Columns.Add("Customers");
                dt.Columns.Add("ProformaINVIDs");
                dt.Columns.Add("ShpngPrfmaInvcNmbr");
                dt.Columns.Add("Freight");
                dt.Columns.Add("PlaceOfDelivery");
                dt.Columns.Add("CreatedBy");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                //GVAirWayBill.DataSource = ds;
                //GVAirWayBill.DataBind();
                //int columncount = GVAirWayBill.Rows[0].Cells.Count;
                //for (int i = 0; i < columncount; i++)
                //    GVAirWayBill.Rows[0].Cells[i].Style.Add("display", "none");

                //GVAirWayBill.Rows[0].Cells.Add(new TableCell());
                //GVAirWayBill.Rows[0].Cells[columncount].ColumnSpan = columncount;
                //GVAirWayBill.Rows[0].Cells[columncount].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
        }

        private void BindGridView(DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    //GVAirWayBill.DataSource = ds.Tables[0];
                    //GVAirWayBill.DataBind();
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
                BindGridView(AWBLL.GetDataSet(CommonBLL.FlagSelectAll, Guid.Empty, CommonBLL.StartDate, CommonBLL.EndDate));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", ex.Message.ToString());
            }
        }

        # endregion

        # region GridView Events

        protected void GVAirWayBill_PreRender(object sender, EventArgs e)
        {
            try
            {
                //GVAirWayBill.UseAccessibleHeader = false;
                //GVAirWayBill.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        protected void GVAirWayBill_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", ex.Message.ToString());
            }
        }

        protected void GVAirWayBill_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int res = 1;
                //int index = Convert.ToInt32(e.CommandArgument);
                ////string AWBID = GVAirWayBill.DataKeys[index].Values["AWBID"].ToString();
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Invoices/AirWayBill.aspx?ID=" + AWBID, false);
                //else if (e.CommandName == "Remove")
                //{
                //    AirWayBillBLL AWBLL = new AirWayBillBLL();
                //    res = AWBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(AWBID));
                //    if (res == 0)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Successfully Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "AirWayBill Status", "Deleted Record " + AWBID + " Successfully.");
                //        Search();
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Cannot Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", "Cannot Delete Record " + AWBID + ".");
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "AirWayBill Status", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClick

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        # endregion

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

                string AWBNo = HFAWBNo.Value;
                string Cust = HFCust.Value;
                string PrfrmInvNo = HFPrfrmInvNo.Value;
                string Freight = HFFreight.Value;
                string PlcOfDelvry = HFPlcOfDelvry.Value;

                DataSet ds = AWBLL.GetDataSetForExport(AWBNo, Cust, PrfrmInvNo, Freight, PlcOfDelvry, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=AWBStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    

                    string MTitle = "STATUS OF AIRWAY BILL", MTDTS = "";

                    MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
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

                #region Delete
                if (result == "Success")
                {
                    DataSet EditDS = AWBLL.GetDataSet(CommonBLL.FlagSelectAll, new Guid(ID));
                    string Stat = EditDS.Tables[0].Rows[0]["StatusID"].ToString();
                    if (Convert.ToInt32(Stat) >= 87) //["StatusID"]
                        res = -123;
                    else
                    {
                        res = AWBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID));
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, Another Transaction is using this record";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

    }
}