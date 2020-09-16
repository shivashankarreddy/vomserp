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

namespace VOMS_ERP.Purchases
{
    public partial class FPOVendorStatus : System.Web.UI.Page
    {
        # region variables
        int res;
        NewFPOBLL NFPOBLL = new NewFPOBLL();
        NewFPOStatusBLL NFPOsBLL = new NewFPOStatusBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewFQuotationBLL NFQBL = new NewFQuotationBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        Guid UserID;
        #endregion

        #region Default Page Load Event

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(FPOVendorStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            UserID = new Guid(Session["UserID"].ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Get Data and Bind to Controls


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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Binding GridView
        /// </summary>
        private void BindGridView(DataSet FrnPOrs)
        {
            try
            {
                DataTable dt = EmptyDt();
                if (FrnPOrs.Tables.Count > 0 && FrnPOrs.Tables[0].Rows.Count > 0)
                {
                    FrnPOrs.Tables[0].DefaultView.Sort = "FPODate";
                    FrnPOrs.AcceptChanges();
                }
                else
                    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
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
                dt.Columns.Add("CustomerNm");
                dt.Columns.Add("FrnEnqNmbr");
                dt.Columns.Add("Quotationnumber");
                dt.Columns.Add("Subject");
                dt.Columns.Add("DeptNm");
                dt.Columns.Add("QuotationDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("FPOID");
                dt.Columns.Add("FPODate");
                dt.Columns.Add("FPONo");
                dt.Columns.Add("FrnEnqNo");
                dt.Columns.Add("CustName");
                dt.Columns.Add("CreatedBy");

                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
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
                dt.Columns.Add(new DataColumn("CustomerNm", typeof(string)));
                dt.Columns.Add(new DataColumn("FrnEnqNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("Quotationnumber", typeof(string)));
                dt.Columns.Add(new DataColumn("Subject", typeof(string)));
                dt.Columns.Add(new DataColumn("DeptNm", typeof(string)));
                dt.Columns.Add(new DataColumn("QuotationDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Status", typeof(string)));


                DataRow dr = dt.NewRow();
                dr["ForeignQuotationId"] = 0;
                dr["CustomerNm"] = string.Empty;
                dr["FrnEnqNmbr"] = string.Empty;
                dr["Quotationnumber"] = string.Empty;
                dr["Subject"] = string.Empty;
                dr["DeptNm"] = string.Empty;
                dr["QuotationDate"] = DateTime.Now;
                dr["Status"] = string.Empty;

                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Redirect to Mail Send Page
        /// </summary>
        /// <param name="FEID"></param>
        private void GenPDF(int FPOID)
        {
            try
            {
                Response.Redirect("../Masters/EmailSend.aspx?FpoID=" + FPOID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Foreign Purchase Order Status Vendor ", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click events

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
                string FrmDt = "", ToDat = "", CreatedDT = "", STF = "";
                Guid LoginID = Guid.Empty, CusID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                    CusID = new Guid(((ArrayList)Session["UserDtls"])[11].ToString());

                if (Mode == "tldt")
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    if (HFToDate.Value != "")
                    {
                        ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                    }
                    else
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

                string FPONo = HFFPONo.Value;
                string FENo = HFFENo.Value;
                string Subject = HFSubject.Value;
                string Vend = HFVendor.Value;
                string Status = HFStatus.Value;
                string Cust = HFCust.Value;
                string OrderValue = HFFPValue.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NFPOsBLL.VendorFPO_Search(FrmDt.Replace("'", "''"), ToDat.Replace("'", "''"), FPONo.Replace("'", "''"), FENo.Replace("'", "''"), Subject.Replace("'", "''"),
                     Vend.Replace("'", "''"), OrderValue.Replace("'", "''"), Status.Replace("'", "''"), Cust.Replace("'", "''"), CreatedDT, LoginID, CusID, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
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

                    if (HFFENo.Value == "")
                    {
                        string MTitle = "DETAILS OF VENDOR FOREIGN PURCHASE ORDERS RECEIVED ", MTcustomer = "", MTDTS = "";
                        if (HFCust.Value != "")
                            MTcustomer = HFCust.Value;
                        if (FrmDt != "" && ToDat != "")
                            MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                        else if (FrmDt != "" && ToDat == "")
                            MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                        else
                            MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                        htextw.Write("<center><b>" + MTitle + " "
                                                  + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                                  + (MTDTS != "" ? MTDTS : "ON" + CreatedDT + "</center></b>"));
                    }
                    else
                    {
                        string Title = "STATUS OF VENDOR FOREIGN PURCHASE ORDER NO : " + ds.Tables[0].Rows[0]["FPO No"].ToString() + ", dt : "
                            + ds.Tables[0].Rows[0]["Date"].ToString();
                        if (HFCust.Value != "")
                            Title = Title + " RECEIVED FROM " + HFCust.Value.ToUpper();

                        htextw.Write("<center><b>" + Title + "</b></center>");
                    }
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
                        string headerTable = "<img src='" + CommonBLL.CommonLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    string style = @"<style> .text { mso-number-format:\@; } </style> ";
                    Response.Write(style);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Render Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

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
                DataTable EmptyFPO = CommonBLL.EmptyDtNewFPO();
                if (EmptyFPO.Columns.Contains("ItemDetailsId"))
                    EmptyFPO.Columns.Remove("ItemDetailsId");
                #region Delete
                if (result == "Success")
                {
                    DataTable dt = EmptyDt();
                    LPOrdersBLL NLPOBL = new LPOrdersBLL();
                    DataSet EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagYSelect, Guid.Empty, new Guid(ID), "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                     DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                     CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), Guid.Empty,
                         DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now,
                         0, 0, 0, Guid.Empty, true, true, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmptyFPO,
                         CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, LPO already created so delete LPO/ Error while Deleting " + ID;
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CancelItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);
                DataTable EmptyFPO = CommonBLL.EmptyDtNewFPO();
                if (EmptyFPO.Columns.Contains("ItemDetailsId"))
                    EmptyFPO.Columns.Remove("ItemDetailsId");

                #region Cancel
                if (result == "Success")
                {
                    DataTable dt = EmptyDt();
                    LPOrdersBLL NLPOBL = new LPOrdersBLL();
                    DataSet EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagYSelect, Guid.Empty, new Guid(ID), "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                     DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                     CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = NFPOBLL.InsertUpdateDelete(CommonBLL.FlagCSelect, new Guid(ID), "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), Guid.Empty,
                         DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now,
                         0, 0, 0, Guid.Empty, true, true, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                         new Guid(Session["UserID"].ToString()), DateTime.Now, true, EmptyFPO,
                         CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtNewFPOVerbal(), false);
                    }
                    if (res == 0)
                        result = "Success::Cancelled Successfully";
                    else
                        result = "Error::Cannot Cancel this Record, LPO already created so Cancel LPO/ Error while Cancelling " + ID;
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string IsForwarded(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string Value = "FALSE";
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);
                DataTable EmptyFPO = CommonBLL.EmptyDtNewFPO();
                if (EmptyFPO.Columns.Contains("ItemDetailsId"))
                    EmptyFPO.Columns.Remove("ItemDetailsId");

                #region Cancel
                if (result == "Success")
                {
                    DataTable dt = EmptyDt();
                    LPOrdersBLL NLPOBL = new LPOrdersBLL();
                    DataSet EditDS = NLPOBL.SelectLPOrders(CommonBLL.FlagYSelect, Guid.Empty, new Guid(ID), "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "",
                     DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(),
                     CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        DataSet ds = NFPOBLL.Select(CommonBLL.FlagBSelect, new Guid(ID), "", Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, false, false, Guid.Empty);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 0)
                                Value = "TRUE";
                        }
                    }
                    result = Value;
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status Vendor", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}
