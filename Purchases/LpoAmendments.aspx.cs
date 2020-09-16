using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Collections;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;

namespace VOMS_ERP.Purchases
{
    public partial class LpoAmendments : System.Web.UI.Page
    {

        # region variables
        int res;
        LPOrdersBLL NLPOBL = new LPOrdersBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewFQuotationBLL NFQBL = new NewFQuotationBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        SupplierBLL SUPLRBL = new SupplierBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        int UserID;
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Defaul Page Load Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                DataTable dt = new DataTable();
                dt = CommonBLL.FirstRowPaymentTerms();
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

        private void Search()
        {
            try
            {
                DataTable dt = EmptyDt();
                #region Hold
                //if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagWCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagQSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagXSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagQSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagQSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagXSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagXSelect, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagWCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagWCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0"
                //    && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NLPOBL.SelectLPOrders(CommonBLL.FlagCommonMstr, 0, 0, "", "", 0, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), Int64.Parse(ddlSupplier.SelectedValue), "",
                //        DateTime.Now, 0, 80, "", Convert.ToInt64(Session["UserID"]), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                //        CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0));
                //else
                #endregion
                BindGridView(NLPOBL.GetAmendmentDataSet(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Binding GridView
        /// </summary>
        private void BindGridView(DataSet lcPOrs)
        {
            try
            {
            //    DataTable dt = EmptyDt();
            //    if (lcPOrs.Tables.Count > 0 && lcPOrs.Tables[0].Rows.Count > 0)
            //    {
            //        lcPOrs.Tables[0].DefaultView.Sort = "LPOrderDate";
            //        lcPOrs.AcceptChanges();

            //        gvLpoItms.DataSource = lcPOrs;
            //        gvLpoItms.DataBind();
            //        ViewState["dset"] = lcPOrs;
            //    }
            //    else
            //        NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                dt.Columns.Add("LocalPurchaseOrderId");
                dt.Columns.Add("SuplrNm");
                dt.Columns.Add("FPOrderNmbr");
                dt.Columns.Add("LocalPurchaseOrderNo");
                dt.Columns.Add("Subject");
                dt.Columns.Add("DeptNm");
                dt.Columns.Add("LPOrderDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("CreatedBy");

                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteItemDetails(Guid ID)
        {
            try
            {
                int res = 1;
                DataSet EditDS = new DataSet();
                LPOrdersBLL NLPOBL = new LPOrdersBLL();
                EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagYSelect, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                    DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                    CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()),"");
                if (EditDS.Tables.Count >= 0 && EditDS.Tables[0].Rows.Count > 0)
                {
                    res = -123;
                }
                else
                {
                    DataTable dt = EmptyDt();
                    res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagDelete, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty,
                    "", "", Guid.Empty, "", true, true, true, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, 0, "", Guid.Empty, CommonBLL.EmptyDtLPOrders(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()),false);
                }
                if (res == 0)
                {
                    GetData();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Loca Purchase Order Status", "Deleted successfully.");
                }
                else if (res != 0)
                {
                    if (res == -123)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Cannot Delete this Record, LPO already created so delete LPO.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"),
                            "Local Purchase Order Status", "Cannot Delete Record " + ID + ".");
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Deleting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"),
                            "Local Purchase Order Status", "Error while Deleting " + ID + ".");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                dt.Columns.Add(new DataColumn("LocalPurchaseOrderId", typeof(long)));
                dt.Columns.Add(new DataColumn("SuplrNm", typeof(string)));
                dt.Columns.Add(new DataColumn("FPOrderNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("LocalPurchaseOrderNo", typeof(string)));
                dt.Columns.Add(new DataColumn("Subject", typeof(string)));
                dt.Columns.Add(new DataColumn("DeptNm", typeof(string)));
                dt.Columns.Add(new DataColumn("LPOrderDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                dt.Columns.Add(new DataColumn("CreatedBy", typeof(string)));


                DataRow dr = dt.NewRow();
                dr["LocalPurchaseOrderId"] = 0;
                dr["SuplrNm"] = string.Empty;
                dr["FPOrderNmbr"] = string.Empty;
                dr["LocalPurchaseOrderNo"] = string.Empty;
                dr["Subject"] = string.Empty;
                dr["DeptNm"] = string.Empty;
                dr["LPOrderDate"] = DateTime.Now;
                dr["Status"] = string.Empty;
                dr["CreatedBy"] = 0;

                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region Button Click events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            Search();
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
                //if (gvLpoItms.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvLpoItms.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                Response.Clear(); //this clears the Response of any headers or previous output
                Response.Buffer = true; //ma
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=LPOrders.pdf");
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
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                //if (gvLpoItms.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvLpoItms.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}


                string FrmDt = "", ToDat = "", CreatedDT = "";
                Guid LoginID = Guid.Empty; Guid CID = Guid.Empty;
                //if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                //    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());

                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");

                string LPO = HFLPONo.Value;
                string FPONo = HFFPONo.Value;
                string Subject = HFSubject.Value;
                string Status = HFStatus.Value;
                string supplier = HFSupplier.Value;
                string LPOId = Request.QueryString["LpoID"].ToString();
                Status = Status.Replace("'", "");
                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NLPOBL.LPO_AmendmentSearch(FrmDt.Replace("'", "''"), ToDat.Replace("'", "''"), LPO.Replace("'", "''"), FPONo.Replace("'", "''"),
                    Subject.Replace("'", "''"), Status.Replace("'", "''"), supplier.Replace("'", "''"), CreatedDT, LoginID, "", new Guid(Session["CompanyID"].ToString()),LPOId);

                if (ds != null && ds.Tables.Count > 0)
                {
                    // string Title = "STATUS OF LOCAL PURCHASE ORDERS RELEASED";
                    string attachment = "attachment; filename=LPOStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    //  htextw.Write("<center><b>" + Title + "</b></center>");
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF LOCAL PURCHASE ORDERS AMENDED", MTcustomer = "", MTDTS = "";
                    if (HFSupplier.Value != "")
                        MTcustomer = HFSupplier.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    //else if (FrmDt == "" && ToDat != "")
                    //    MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
                    if (ds.Tables[0].Columns.Contains("LocalPurchaseOrderId"))
                        ds.Tables[0].Columns.Remove("LocalPurchaseOrderId");
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

                //string Title = "LOCAL PURCHASE ORDER STATUS";
                //string attachment = "attachment; filename=LPorders.xls";
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", attachment);
                //Response.ContentType = "application/ms-excel";
                //StringWriter stw = new StringWriter();
                //HtmlTextWriter htextw = new HtmlTextWriter(stw);
                //htextw.Write("<center><b>");
                //htextw.Write(Title + "</b></center>");
                //Response.Write(stw.ToString());
                //Response.End();
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

        /// <summary>
        /// Rendering for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}