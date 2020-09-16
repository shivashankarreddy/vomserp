using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class MateReceiptStatus : System.Web.UI.Page
    {

        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        MateReceiptBLL MRBL = new MateReceiptBLL();
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
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(MateReceiptStatus));
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                // Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        //protected void ClearInputs()
        //{
        //    try
        //    {
        //        txtToDt.Text = txtFrmDt.Text = txtCustomerNm.Text = "";
        //        hfCstmrID.Value = "0";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int lineNmbr = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This meathod is used to search Proforma Invoice Details from DB based on the parameter
        /// </summary>
        //private void Search()
        //{
        //    try
        //    {
        //        if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagBSelect, 0, 0,
        //                int.Parse(hfCstmrID.Value), CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), 0));
        //        else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagCSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, 0));
        //        else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagBSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.StartDate, CommonBLL.EndDate, 0));
        //        else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagBSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, 0));
        //        else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagBSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), 0));
        //        else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagCSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, 0));
        //        else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagCSelect, 0, 0, int.Parse(hfCstmrID.Value),
        //                CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), 0));
        //        else
        //            BindGridView(gvMateReceipt, MRBL.SelectMateReceiptDetails(CommonBLL.FlagSelectAll, 0, 0, 0, 0));

        //        ClearInputs();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int lineNmbr = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet Rceds)
        {
            try
            {
                if (Rceds.Tables.Count > 0 && Rceds.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = Rceds;
                    gview.DataBind();
                }
                else
                {
                    gview.DataSource = null;
                    gview.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from Proforma Invoice Details Status
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteRecord(Guid ID)
        {
            try
            {
                res = MRBL.InsertUpdateDeleteMateReceiptDtls(CommonBLL.FlagDelete, ID, Guid.Empty, Guid.Empty, "", DateTime.Now, Guid.Empty, Guid.Empty, 0,
                    "", 0, "", "", "", "", "", "", Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events
        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvMateReceipt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int lastCellIndex = e.Row.Cells.Count - 1;
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void gvMateReceipt_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    try
        //    {
        //        int index = int.Parse(e.CommandArgument.ToString());
        //        GridViewRow gvrow = gvMateReceipt.Rows[index];
        //        int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblgdnID")).Text);
        //        if (e.CommandName == "Modify")
        //            Response.Redirect("../Invoices/MateReceipt.Aspx?ID=" + ID, false);
        //        else if (e.CommandName == "Remove")
        //            DeleteRecord(ID);
        //        //else if (e.CommandName == "Mail")
        //        //    Response.Redirect("../Masters/EmailSend.aspx?DpItID=" + ID.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int lineNmbr = ExceptionHelper.LineNumber(ex);
        //        if (ErrMsg != "Thread was being aborted.")
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void gvMateReceipt_PreRender(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (gvMateReceipt.HeaderRow == null) return;
        //        gvMateReceipt.UseAccessibleHeader = false;
        //        gvMateReceipt.HeaderRow.TableSection = TableRowSection.TableHeader;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int lineNmbr = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
        //    }
        //}
        #endregion

        #region Button Click Events
        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
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

                string FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                string ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");

                string PInvNo = HFPInvNo.Value;
                string SBNo = HFSBNo.Value;
                string MRNo = HFMateReceiptNo.Value;
                string Forwarder = HFForwarderName.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = MRBL.MateReceiptSearch(FrmDt, ToDat, PInvNo, SBNo, MRNo, Forwarder, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {

                    string attachment = "attachment; filename=MateReceiptReportDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF Mate Receipt REPORT ", MTcustomer = "", MTDTS = "";

                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
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
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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

        #region Web Methods for Edit/Delete

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    res = MRBL.InsertUpdateDeleteMateReceiptDtls(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "",
                        DateTime.Now, Guid.Empty, Guid.Empty, 0, "", 0, "", "", "", "", "", "", Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record/ Referenced with E-BRC.";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}