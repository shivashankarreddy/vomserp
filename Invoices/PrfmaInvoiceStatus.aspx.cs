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
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class PrfmaInvoiceStatus : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(PrfmaInvoiceStatus));
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
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
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                //txtToDt.Text = txtFrmDt.Text = txtCustomerNm.Text = "";
                //hfCstmrID.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Proforma Invoice Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0,
                //        int.Parse(hfCstmrID.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //        CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else
                //    BindGridView(gvPrfmaInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagASelect, 0, 0, ""));
                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from Proforma Invoice Details Status
        /// </summary>
        /// <param name="ID"></param>
        //public string DeleteRecord(string ID, string CreatedBy, String IsCust)
        //{
        //    try
        //    {
        //        res = INBLL.InsertUpdateDeletePrfmaInvcDtls(CommonBLL.FlagDelete, Convert.ToInt64(ID), 0, 0, "", "", DateTime.Now, "", 0, "",
        //            "", "", "", "", "", "", "", "", "", "", 0, CommonBLL.PrfmaInvcItems());
        //        if (res == 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details",
        //                "Row Deleted successfully.");
        //            GetData();
        //        }
        //        else if (res != 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Error while Deleting.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details",
        //                "Error while Deleting " + ID + ".");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int lineNmbr = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
        //        return res;
        //    }
        //}

        /// <summary>
        /// Button ReadMore In Gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnreadMore_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton button = (LinkButton)sender;
                GridViewRow row = button.NamingContainer as GridViewRow;
                Label descLabel = row.FindControl("lblTDP") as Label;
                button.Text = (button.Text == "Read More") ? "Hide" : "Read More";
                //string temp = descLabel.Text;
                if (button.Text == "Hide")
                {
                    Label ExtraLabel1 = row.FindControl("lblExtra") as Label;
                    descLabel.Text = ExtraLabel1.Text.Replace("\n", "<br/>");//Text Full Data
                    descLabel.ToolTip = ExtraLabel1.ToolTip; // toolTip Limited Text                   
                }
                else
                {
                    Label ExtraLabel = row.FindControl("lblExtra") as Label;
                    descLabel.Text = ExtraLabel.ToolTip;//.Replace("\r\n", "<br/>");
                    descLabel.ToolTip = ExtraLabel.Text;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Visible ReadMore Button
        /// </summary>
        /// <param name="Desc"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected bool SetVisibility(object Desc, int length)
        {
            return Desc.ToString().Length > length;
        }

        #endregion

        #region Grid View Events
        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPrfmaInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int lastCellIndex = e.Row.Cells.Count - 1;
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    // string PInvID = gvPrfmaInvoice.DataKeys[e.Row.RowIndex].Values["ID"].ToString();

                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                    int length = ((Label)e.Row.FindControl("lblTDP")).Text.Length;
                    if (length == 100)
                        ((Label)e.Row.FindControl("lblTDP")).Text += "...";
                    ((Label)e.Row.FindControl("lblExtra")).ToolTip += "...";

                    //((Label)e.Row.FindControl("lblTDP")).Text = ((Label)e.Row.FindControl("lblBody")).Text.Replace("%$%", "'");
                    ((Label)e.Row.FindControl("lblExtra")).Text = ((Label)e.Row.FindControl("lblExtra")).Text.Replace("%$%", "'");
                    //((Label)e.Row.FindControl("lblTDP")).ToolTip = ((Label)e.Row.FindControl("lblBody")).ToolTip.Replace("%$%", "'");
                    ((Label)e.Row.FindControl("lblExtra")).ToolTip = ((Label)e.Row.FindControl("lblExtra")).ToolTip.Replace("%$%", "'");


                    HyperLink hypINVNo = (HyperLink)e.Row.FindControl("hlInvoiceNo");
                    HyperLink hypPRfINVNo = (HyperLink)e.Row.FindControl("hlPrfmInvcNo");

                    //if (hypINVNo.Text.Trim() != "")
                    //{
                    //    hypPRfINVNo.Enabled = false;
                    //    hypINVNo.NavigateUrl = "~/Invoices/PrfmaInvoiceDetails.Aspx?ID=" + PInvID;
                    //}
                    //else
                    //    hypPRfINVNo.NavigateUrl = "~/Invoices/PrfmaInvoiceDetails.aspx?ID=" + PInvID;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPrfmaInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvPrfmaInvoice.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblPrfmaInvcID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Invoices/PrfmaInvoice.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //    Response.Redirect("../Masters/EmailSend.aspx?DpItID=" + ID.ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                if (ErrMsg != "Thread was being aborted.")
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPrfmaInvoice_PreRender(object sender, EventArgs e)
        {
            try
            {
                //    if (gvPrfmaInvoice.HeaderRow == null) return;
                //    gvPrfmaInvoice.UseAccessibleHeader = false;
                //    gvPrfmaInvoice.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }
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
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
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
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
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
                string ShpInvNo = HFShpINVNo.Value;
                string PInvNo = HFPInvNo.Value;
                string Cust = HFCustNm.Value;
                string FPOnos = HFFPO.Value;
                string Trms = HFTrmsDelPaymnt.Value;
                string ShpPlNo = HFShpPlngNo.Value;
                string Status = HFStat.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";

                DataSet ds = INBLL.PInv_Search(FrmDt, ToDat, ShpInvNo, PInvNo, Trms, Cust, FPOnos, ShpPlNo, Status, new Guid(Session["CompanyId"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=SHIPMENT_PLANING_DIRECT_STATUS.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = " STATUS OF PROFORMA INVOICE ", MTcustomer = "", MTDTS = "";
                    if (HFCustNm.Value != "")
                        MTcustomer = HFCustNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + ""
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        //try
        //{
        //    if (gvPrfmaInvoice.Rows.Count > 0)
        //    {
        //        foreach (GridViewRow r in this.gvPrfmaInvoice.Controls[0].Controls)
        //        {
        //            r.Cells.RemoveAt(r.Cells.Count - 1);
        //            r.Cells.RemoveAt(r.Cells.Count - 1);
        //        }
        //    }
        //    string Title = "PROFORMA INVOICE STATUS";
        //    string attachment = "attachment; filename=Shpmnt_Invoice.xls";
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", attachment);
        //    Response.ContentType = "application/ms-excel";
        //    StringWriter stw = new StringWriter();
        //    HtmlTextWriter htextw = new HtmlTextWriter(stw);
        //    string MTitle = " STATUS OF PROFORMA INVOICE ", MTcustomer = "", MTDTS = "";

        //    if (txtCustomerNm.Text != "")
        //        MTcustomer = txtCustomerNm.Text.ToUpper();
        //    if (txtFrmDt.Text != "" && txtToDt.Text != "")
        //        MTDTS = " DURING " + txtFrmDt.Text + " TO " + txtToDt.Text;
        //    else if (txtFrmDt.Text != "" && txtToDt.Text == "")
        //        MTDTS = " DURING " + txtFrmDt.Text + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
        //    else
        //        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

        //    htextw.Write("<center><b>" + MTitle + " "
        //                           + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
        //                           + MTDTS + "</center></b>");
        //    //htextw.Write("<center><b>");
        //    //if (!String.IsNullOrEmpty(txtCustomerNm.Text) && !String.IsNullOrEmpty(txtFrmDt.Text) && !String.IsNullOrEmpty(txtToDt.Text))
        //    //    Title = Title + " of " + txtCustomerNm.Text + " from " + txtFrmDt.Text + " to " + txtToDt.Text + " ";
        //    //else if (!String.IsNullOrEmpty(txtCustomerNm.Text))
        //    //    Title = Title + " of " + txtCustomerNm.Text;
        //    //htextw.Write(Title + "</b></center>");
        //    gvPrfmaInvoice = CommonBLL.AddGVStyle(gvPrfmaInvoice);
        //    gvPrfmaInvoice.RenderControl(htextw);
        //    Response.Write(CommonBLL.AddExcelStyling());
        //    string headerTable = "<img src='" + CommonBLL.CommonLogoUrl(HttpContext.Current) + "'margin-top =16px width=125 height=35 />";
        //    Response.Write(headerTable);
        //    Response.Write(stw.ToString());
        //    Response.End();
        //}
        //catch (ThreadAbortException)
        //{ }
        //catch (Exception ex)
        //{
        //    Session["dsEx"] = null;
        //    string ErrMsg = ex.Message;
        //    int lineNmbr = ExceptionHelper.LineNumber(ex);
        //    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
        //}


        /// <summary>
        /// Export to PDF Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //if (gvPrfmaInvoice.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvPrfmaInvoice.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                //Response.Clear(); //this clears the Response of any headers or previous output
                //Response.Buffer = true; //ma
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=Shpmnt_Invoice.pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //StringWriter sw = new StringWriter();
                //HtmlTextWriter hw = new HtmlTextWriter(sw);
                //gvPrfmaInvoice.RenderControl(hw);
                //StringReader sr = new StringReader(sw.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                //pdfDoc.Open();
                //htmlparser.Parse(sr);
                //pdfDoc.Close();
                //Response.Write(pdfDoc);
                //Response.End();

            }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to Word Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnWordExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //if (gvPrfmaInvoice.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvPrfmaInvoice.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                //string attachment = "attachment; filename=Prfma_Invoice.doc";
                //Response.ClearContent();
                ////Response.Buffer = true;
                //Response.AddHeader("content-disposition", attachment);//"attachment;filename=Shpmnt_Invoice.doc");
                //Response.Charset = "";
                //Response.ContentType = "application/vnd.ms-word ";
                //StringWriter stw = new StringWriter();
                //HtmlTextWriter htextw = new HtmlTextWriter(stw);
                //gvPrfmaInvoice.RenderControl(htextw);
                //Response.Output.Write(stw.ToString());
                //Response.Flush();
                //Response.End();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice Status Details", ex.Message.ToString());
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
                    DataSet Pinv = new DataSet();
                    Pinv = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagASelect, new Guid(ID), Guid.Empty, "", new Guid(Session["CompanyID"].ToString()));
                    if (Pinv != null && Pinv.Tables.Count >= 0 && Pinv.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = INBLL.InsertUpdateDeletePrfmaInvcDtls(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", Guid.Empty, "",
                        "", "", "", "", "", "", "", "", 0, "", "", false, Guid.Empty, CommonBLL.PrfmaInvcItems(), CommonBLL.PrfmaInvoice_SubItems(), new Guid(Session["CompanyId"].ToString()));
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record/ Error while Deleting ";
                }
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
