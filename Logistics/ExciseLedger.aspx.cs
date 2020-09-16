using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;

namespace VOMS_ERP.Logistics
{
    public partial class ExciseLedger : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        ExciseLedgerBLL ELBLL = new ExciseLedgerBLL();
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
                        if (!IsPostBack)
                        {
                            GetData();
                            txtFrmDt.Attributes.Add("readonly", "readonly");
                            txtToDt.Attributes.Add("readonly", "readonly");
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
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
                LblTotAmount.Text = "Total Bond Amount("+Session["CurrencySymbol"].ToString().Trim()+") :";
                DataSet CommonDt = ELBLL.SelectExseBndLdgr(CommonBLL.FlagFSelect, Guid.Empty,Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if(CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                txtTBAmt.Text = CommonDt.Tables[0].Rows[0]["BondValue"].ToString();
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                txtToDt.Text = txtFrmDt.Text = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Central Excise Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                if (txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvSevottam, ELBLL.SelectExseBndLdgr(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, CommonBLL.DateInsert(txtFrmDt.Text),
                        CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyID"].ToString())));
                else if (txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                    BindGridView(gvSevottam, ELBLL.SelectExseBndLdgr(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, CommonBLL.StartDate, CommonBLL.EndDate, new Guid(Session["CompanyID"].ToString())));
                        //CommonBLL.DateInsert(CommonBLL.EndDate.ToString("dd-MM-yyyy")))); //CommonBLL.DateInsert(CommonBLL.StartDate.ToString("dd-MM-yyyy"))
                else if (txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(gvSevottam, ELBLL.SelectExseBndLdgr(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, CommonBLL.DateInsert(txtFrmDt.Text),
                        CommonBLL.EndDate, new Guid(Session["CompanyID"].ToString())));
                else if (txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvSevottam, ELBLL.SelectExseBndLdgr(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, CommonBLL.StartDate,
                        CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyID"].ToString())));
                else
                    BindGridView(gvSevottam, ELBLL.SelectExseBndLdgr(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
            }
        }

        protected string GetFormatedNumber(object number)
        {
            if (number != null)
            {
                //return String.Format("{0:f2}", (Math.Round((decimal) number, 0)));
                return (Math.Round((decimal)number, 0)).ToString("f2");
            }
            return "0.00";
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables[0].Rows.Count > 0)
                    txtTBAmt.Text = txtTBAmt.Text != "" ?
                        (Convert.ToDecimal(txtTBAmt.Text) + Convert.ToDecimal(CommonDt.Tables[0].Compute("Sum(Credit)", "").ToString())).ToString()
                        : CommonDt.Tables[0].Compute("Sum(Credit)", "").ToString();

                if (CommonDt.Tables.Count > 1 && CommonDt.Tables[0].Rows.Count > 0 && CommonDt.Tables[1].Rows.Count > 0)
                {
                    DataRow dr;
                    for (int i = 0; i < CommonDt.Tables[1].Rows.Count; i++)
                    {
                        dr = CommonDt.Tables[0].NewRow();
                        dr["TDate"] = CommonDt.Tables[1].Rows[i]["TDate"].ToString();
                        dr["Debit"] = "0.00";
                        dr["Credit"] = CommonDt.Tables[1].Rows[i]["BondValue"].ToString();
                        dr["Description"] = CommonDt.Tables[1].Rows[i]["TwsDscptn"].ToString();
                        CommonDt.Tables[0].Rows.Add(dr);
                    }
                    CommonDt.Tables[0].AcceptChanges();
                }                

                if (CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    if (txtTBAmt.Text != "")
                    {
                        //txtBndAmtDt.Text = (Convert.ToDecimal(txtTBAmt.Text) - 
                        //    Convert.ToDecimal(CommonDt.Tables[0].AsEnumerable().Sum(dr => dr.Field<decimal>("Debit"))) +
                        //    Convert.ToDecimal(CommonDt.Tables[0].AsEnumerable().Sum(dr => dr.Field<decimal>("Credit")))).ToString(); 
                        decimal Debit = Convert.ToDecimal(CommonDt.Tables[0].Compute("Sum(Debit)", "").ToString());
                        decimal Credit = Convert.ToDecimal(CommonDt.Tables[0].Compute("Sum(Credit)", "").ToString());
                        txtBndAmtDt.Text = (Credit - Debit).ToString();
                    }
                    //CommonDt.Tables[0].DefaultView.Sort = "TDate DESC";
                    gview.DataSource = CommonDt.Tables[0];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Generate Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
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
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Excise Ledger", ex.Message.ToString());
            }
        }
        #endregion
        
        #region Grid View Events
        
        /// <summary>
        /// Grid View Sevottam Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSevottam_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvSevottam.UseAccessibleHeader = false;
                gvSevottam.HeaderRow.TableSection = TableRowSection.TableHeader;
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
                if (gvSevottam.Rows.Count > 0)
                {
                    foreach (GridViewRow r in this.gvSevottam.Controls[0].Controls)
                    {
                        r.Cells.RemoveAt(r.Cells.Count - 1);
                        //r.Cells.RemoveAt(r.Cells.Count - 1);
                    }
                }
                string Title = "EXCISE LEDGER";
                string attachment = "attachment; filename=EXCISE_LEDGER.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/ms-excel";
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                htextw.Write("<center><b>");
                if (!String.IsNullOrEmpty(txtFrmDt.Text) && !String.IsNullOrEmpty(txtToDt.Text))
                    Title = Title + " from " + txtFrmDt.Text + " to " + txtToDt.Text + " ";
                else
                    Title = Title + "";
                htextw.Write(Title + "</b></center>");
                gvSevottam.RenderControl(htextw);
                Response.Write(stw.ToString());
                Response.End();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
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
                if (gvSevottam.Rows.Count > 0)
                {
                    foreach (GridViewRow r in this.gvSevottam.Controls[0].Controls)
                    {
                        r.Cells.RemoveAt(r.Cells.Count - 1);
                        //r.Cells.RemoveAt(r.Cells.Count - 1);
                    }
                }
                Response.Clear(); //this clears the Response of any headers or previous output
                Response.Buffer = true; //ma
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=EXCISE_LEDGER.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvSevottam.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                Response.Write(pdfDoc);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
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
                if (gvSevottam.Rows.Count > 0)
                {
                    foreach (GridViewRow r in this.gvSevottam.Controls[0].Controls)
                    {
                        r.Cells.RemoveAt(r.Cells.Count - 1);
                        //r.Cells.RemoveAt(r.Cells.Count - 1);
                    }
                }
                string attachment = "attachment; filename=EXCISE_LEDGER.doc";
                Response.ClearContent();
                //Response.Buffer = true;
                Response.AddHeader("content-disposition", attachment);//"attachment;filename=Shpmnt_Invoice.doc");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-word ";
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                gvSevottam.RenderControl(htextw);
                Response.Output.Write(stw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
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

    }
}