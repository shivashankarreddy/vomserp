using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;
using BAL;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Data;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class CommercialInvoiceStatus : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        IOMTemplate2BLL IMBL = new IOMTemplate2BLL();
        CommercialINVBLL CINBLL = new CommercialINVBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(CommercialInvoiceStatus));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            GetData();
                            //txtFrmDt.Attributes.Add("readonly", "readonly");
                            // txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0,
                //        int.Parse(hfCstmrID.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //        CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagBSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvCmrclInvoice, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagCSelect, 0, int.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))));
                //else
                //    BindGridView(gvCmrclInvoice, IMBL.Select(CommonBLL.FlagKSelect, 0, 0));
                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                res = INBLL.InsertUpdateDeletePrfmaInvcDtls(CommonBLL.FlagDelete, ID, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", Guid.Empty, "",
                    "", "", "", "", "", "", "", "", 0, "", "", false, new Guid(Session["UserID"].ToString()), CommonBLL.PrfmaInvcItems(), CommonBLL.PrfmaInvoice_SubItems(), new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Button ReadMore In Gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnreadMore_Click(object sender, EventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmrclInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvCmrclInvoice.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblPrfmaInvcID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Invoices/CommercialInvoice.Aspx?ID=" + ID, false);


                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                if (ErrMsg != "Thread was being aborted.")
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmrclInvoice_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvCmrclInvoice.HeaderRow == null) return;
                //gvCmrclInvoice.UseAccessibleHeader = false;
                //gvCmrclInvoice.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                string CinvNo = HFCINNo.Value;
                string PinNo = HFPinNo.Value;
                string VessNo = HFVessNo.Value;
                string Sbno = HFSbNo.Value;
                string BLNo = HFBLNo.Value;
                string CInvFdt = HFCFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCFrmDt.Value).ToString("yyyy-MM-dd");
                string CinvTdt = HFCToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCToDt.Value).ToString("yyyy-MM-dd");
                string PinFdt = HFPFrDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPFrDt.Value).ToString("yyyy-MM-dd");
                string PinvTDt = HFPToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPToDt.Value).ToString("yyyy-MM-dd");
                DataSet ds = INBLL.GetDataSetForExport(CinvNo, CInvFdt, CinvTdt, PinNo, PinFdt, PinvTDt, VessNo, Sbno, BLNo, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=CommercialInvoiceStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (CInvFdt != "" && Convert.ToDateTime(CInvFdt).ToString("dd-MM-yyyy") == "01-01-1900")
                        CInvFdt = "";
                    if (CinvTdt != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(CinvTdt)) == CommonBLL.EndDtMMddyyyy_FS)
                        CinvTdt = "";
                    if (PinFdt != "" && Convert.ToDateTime(PinFdt).ToString("dd-MM-yyyy") == "01-01-1900")
                        PinFdt = "";
                    if (PinvTDt != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(PinvTDt)) == CommonBLL.EndDtMMddyyyy_FS)
                        PinvTDt = "";
                    string MTitle = "STATUS OF COMMERCIAL INVOICE", MTDTS = "";

                    if (CInvFdt != "" && CinvTdt != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(CInvFdt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(CinvTdt));
                    else if (CInvFdt != "" && CinvTdt == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(CInvFdt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");


                    htextw.Write("<center><b>" + MTitle + " " + MTDTS + "</center></b>");
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to PDF Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //if (gvCmrclInvoice.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvCmrclInvoice.Controls[0].Controls)
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
                //gvCmrclInvoice.RenderControl(hw);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
                //if (gvCmrclInvoice.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvCmrclInvoice.Controls[0].Controls)
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
                //gvCmrclInvoice.RenderControl(htextw);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice Status Details", ex.Message.ToString());
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
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string PinvId)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                if (result == "Success")
                {
                    DataSet EditDS = CINBLL.SelectCmrclInvc(CommonBLL.FlagCSelect, Guid.Empty, new Guid(PinvId), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), "");
                    string Stat = "";
                    if (EditDS != null && EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        //    Stat = EditDS.Tables[0].Rows[0]["StatusID"].ToString();
                        //if (Stat == "" || Convert.ToInt32(Stat) >= 87) //["StatusID"]
                        res = -123;
                    else
                    {
                        res = CINBLL.InsetUpdateDeleteCmrclInvc(CommonBLL.FlagDelete, new Guid(ID), new Guid(PinvId), "", DateTime.Now, "", "", "", "",
                            new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, "");
                    }
                    if (res == 0)
                        result = "Success";
                    else
                        result = "Error";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}