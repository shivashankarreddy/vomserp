using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using Ajax;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace VOMS_ERP.Customer_Access
{
    public partial class Customer_LPO_Status : System.Web.UI.Page
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
                Ajax.Utility.RegisterTypeForAjax(typeof(Customer_LPO_Status));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
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
  
        private void Search()
        {
            try
            {
              
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
                DataTable dt = EmptyDt();
                if (lcPOrs.Tables.Count > 0 && lcPOrs.Tables[0].Rows.Count > 0)
                {
                    lcPOrs.Tables[0].DefaultView.Sort = "LPOrderDate";
                    lcPOrs.AcceptChanges();
                    ViewState["dset"] = lcPOrs;
                }
                else
                    NoTable();
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

        protected void ClearInputs()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM-2 Template Status", ex.Message.ToString());
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
                //dt.Columns.Add(new DataColumn("FPOrderNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("LocalPurchaseOrderNo", typeof(string)));
                dt.Columns.Add(new DataColumn("Subject", typeof(string)));
                dt.Columns.Add(new DataColumn("DeptNm", typeof(string)));
                dt.Columns.Add(new DataColumn("LPOrderDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                dt.Columns.Add(new DataColumn("CreatedBy", typeof(string)));


                DataRow dr = dt.NewRow();
                dr["LocalPurchaseOrderId"] = 0;
                dr["SuplrNm"] = string.Empty;
               // dr["FPOrderNmbr"] = string.Empty;
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
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
                Response.AddHeader("content-disposition", "attachment;filename=LPOrders.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                //gvLpoItms.RenderControl(hw);
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
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "";
                Guid LoginID = Guid.Empty; Guid CID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());

                if (Mode == "tldt")
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    if (HFToDate.Value != "")
                    {
                        ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                    }
                    else
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                    ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "tdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
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

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NLPOBL.LPO_Search(FrmDt.Replace("'", "''"), ToDat.Replace("'", "''"), LPO.Replace("'", "''"), FPONo.Replace("'", "''"),
                    Subject.Replace("'", "''"), Status.Replace("'", "''"), supplier.Replace("'", "''"), CreatedDT, LoginID, Mode, new Guid(Session["CompanyID"].ToString()));

                ds.Tables[0].Columns.Remove("CreatedDate");
                ds.AcceptChanges();

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

                    string MTitle = "STATUS OF LOCAL PURCHASE ORDERS RELEASED", MTcustomer = "", MTDTS = "";
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

        # region GridView Events

        /// <summary>
        /// This is used to get deleting / editing ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLpoItms_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
           
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLpoItms_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int index = e.Row.RowIndex;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                Label CrtedBy = (Label)e.Row.FindControl("lblCrtdBy");

                LinkButton BtnDrawing = (LinkButton)e.Row.Cells[lastCellIndex - 4].Controls[0];
            

                if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                {
                    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                    {
                        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            UserID != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text))))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //EditButton.Enabled = false;
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    else
                    {
                        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //EditButton.Enabled = false;
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                }
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                # region NotInUse
              
                # endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLpoItms_PreRender(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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

                #region Delete
                if (result == "Success")
                {
                    DataSet LPOVerbal = new DataSet();
                    LPOVerbal = NLPOBL.GetDataSetLPO_Verbal(CommonBLL.FlagQSelect, new Guid(ID), Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty,
                                            "", "", Guid.Empty, "", true, true, true, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, 0, "", Guid.Empty, CommonBLL.EmptyDtLPOrdersVerbal(),
                                            CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()));
                    DataTable dt = EmptyDt();
                    DataSet EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagYSelect, new Guid(ID), Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                    DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                    CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0 &&
                        Convert.ToBoolean(EditDS.Tables[1].Rows[0]["IsVerbalFPO"].ToString()) == false || (LPOVerbal.Tables.Count > 0 && LPOVerbal.Tables[1].Rows.Count > 0))
                        res = -123;
                    else
                    {
                        res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty,
                        "", "", Guid.Empty, "", true, true, true, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, 0, "", Guid.Empty, CommonBLL.EmptyDtLPOrders(),
                        CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()), false);
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, this is used by another transaction/ Error while Deleting ";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string Cancel(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    res = NLPOBL.InsertUpdateDeleteLPOrders(CommonBLL.FlagRegularDRP, new Guid(ID), Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty,
                    "", "", Guid.Empty, "", true, true, true, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, 0, "", Guid.Empty, CommonBLL.EmptyDtLPOrders(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, "", new Guid(Session["CompanyID"].ToString()), true);
                    if (res == 0)
                        result = "Success";
                    else
                        result = "Error";
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}