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

namespace VOMS_ERP.Customer_Access
{
    public partial class FQStatusCustomer : System.Web.UI.Page
    {
        # region variables
        int res;
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewFQuotationBLL NFQBL = new NewFQuotationBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        int UserID;
        #endregion

        #region PageLoad
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(FQStatusCustomer));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Get Data and Bind to Controls

        /// <summary>
        /// Getdata for All DropDownLists and GridViews
        /// </summary>
        protected void GetData()
        {
            try
            {
                BindGridView(NFQBL.Select(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "",
                Convert.ToDecimal(0), Convert.ToDecimal(0), Convert.ToDecimal(0), Convert.ToDecimal(0), Guid.Empty, "", DateTime.Now, 0,
                Guid.Empty, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(),
                CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownLists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Binding GridView
        /// </summary>
        private void BindGridView(DataSet FrnQues)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("S.No.");
                dt.Columns.Add("ForeignQuotationId");
                dt.Columns.Add("CustmrNm");
                dt.Columns.Add("FrnEnqNmbr");
                dt.Columns.Add("Quotationnumber");
                dt.Columns.Add("Subject");
                dt.Columns.Add("DeptNm");
                dt.Columns.Add("QuotationDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("CreatedBy");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteItemDetails(string ID)
        {
            try
            {
                int res = 1;
                NewFPOBLL NFPOBLL = new NewFPOBLL();
                DataSet EditDS = NFPOBLL.Select(CommonBLL.FlagLSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, ID,
                                 DateTime.Now, "", "", "",DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now,
                                 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                                 new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPO(), CommonBLL.FirstRowPaymentTerms(),
                                 CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (EditDS.Tables.Count >= 0 && EditDS.Tables[0].Rows.Count > 0)
                    res = -123;
                else
                {
                    DataTable dt = EmptyDt();
                    res = NFQBL.FQuotationInsert(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "",
                        0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, CommonBLL.StatusTypeFrnQuotID, "",
                        new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now,
                        true, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));
                }
                if (res == 0)
                {
                    BindGridView(NFQBL.Select(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", CommonBLL.StartDate,
                        "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, 0, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(),
                        CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString())));
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "Loca Quotation Status", "Row Deleted successfully.");
                }
                else if (res != 0)
                {
                    if (res == -123)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Cannot Delete this Record, FPO already created so delete FPO.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status",
                            "Cannot Delete Record " + ID + ".");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Deleting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status",
                            "Error while Deleting " + ID + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Empty Data Tables
        /// </summary>
        /// <returns></returns>
        private DataTable EmptyDt()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ForeignQuotationId", typeof(long)));
                dt.Columns.Add(new DataColumn("CustmrNm", typeof(string)));
                dt.Columns.Add(new DataColumn("FrnEnqNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("Quotationnumber", typeof(string)));
                dt.Columns.Add(new DataColumn("Subject", typeof(string)));
                dt.Columns.Add(new DataColumn("DeptNm", typeof(string)));
                dt.Columns.Add(new DataColumn("QuotationDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                dt.Columns.Add(new DataColumn("CreatedBy", typeof(long)));

                DataRow dr = dt.NewRow();
                dr["ForeignQuotationId"] = 0;
                dr["CustmrNm"] = string.Empty;
                dr["FrnEnqNmbr"] = string.Empty;
                dr["Quotationnumber"] = string.Empty;
                dr["Subject"] = string.Empty;
                dr["DeptNm"] = string.Empty;
                dr["QuotationDate"] = DateTime.Now;
                dr["Status"] = string.Empty;
                dr["CreatedBy"] = 0;

                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
                return null;
            }
        }

        protected void ClearInputs()
        {
            try
            {
                //txtFrmDt.Text = txtToDt.Text = "";
                //ddlCustomer.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM-2 Template Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click events

        /// <summary>
        /// Search/btnSave Button Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            DataTable dtt = new DataTable();
            dtt = CommonBLL.FirstRowPaymentTerms();
            try
            {
                DataTable dt = EmptyDt();
                BindGridView(NFQBL.Select(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "",
                    0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, 0, "", new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty,
                    DateTime.Now, true, CommonBLL.EmptyDtFQ(), dtt, CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to Pdf Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.Clear(); //this clears the Response of any headers or previous output
                Response.Buffer = true; //ma
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=FrnQuotations.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                Response.Write(pdfDoc);
                Response.End(); // HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

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
                Guid LoginID = Guid.Empty; string CusID = "";
                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    CusID = ((ArrayList)Session["UserDtls"])[11].ToString();
                }

                if (Mode == "opd")
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    if (HFToDate.Value != "")
                        ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                    else
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "tdt")
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                else
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                }

                string FENo = HFFQNo.Value;
                string ForeignEnquiryId = HFRefFENO.Value;
                string Subject = HFSubject.Value;
                string Status = HFStatus.Value;
                string Cust = HFCust.Value;

                if (FrmDt == "01-01-0001" || FrmDt == "01-01-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NFQBL.SelectSearch(FrmDt, ToDat, FENo, ForeignEnquiryId, Subject, Status, Cust, CreatedDT, LoginID, CusID, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=FrnQuotations.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF FOREIGN QUOTATIONS SENT", MTcustomer = "", MTDTS = "";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                               + (MTcustomer != "" ? " TO " + MTcustomer.ToUpper() : "") + ""
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }

        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);
                if (result == "Success")
                {
                    NewFPOBLL NFPOBLL = new NewFPOBLL();
                    DataSet EditDS = NFPOBLL.Select(CommonBLL.FlagLSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, ID,
                         DateTime.Now, "", "", "",DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0,
                         Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPOForCheckList(),
                         CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count >= 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        DataTable dt = EmptyDt();
                        res = NFQBL.FQuotationInsert(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, "", "",
                            DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, CommonBLL.StatusTypeFrnQuotID, "",
                            new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now,
                            true, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, FPO already created so delete FPO/ Error while Deleting ";
                }
                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Status", ex.Message.ToString());
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