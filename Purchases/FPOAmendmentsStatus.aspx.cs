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
using System.Threading;

namespace VOMS_ERP.Purchases
{
    public partial class FPOAmendmentsStatus : System.Web.UI.Page
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

        /// <summary>
        /// Default Page Load Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(FPOAmendmentsStatus));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Amendment Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Amendment Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }
        # endregion

        #region Button Click events

        protected void btnClear_Click(object sender, EventArgs e)
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
        /// Export to Pdf Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
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
                string FrmDt = "", ToDat = "", CreatedDT = "", STF = "";
                Guid LoginID = Guid.Empty; string CusID = "";
                if (((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString()) && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                    CusID = ((ArrayList)Session["UserDtls"])[11].ToString();

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
                string Status = HFStatus.Value;
                string Cust = HFCust.Value;
                string OrderValue = HFFPValue.Value;
                string FPOID = Request.QueryString["ID"].ToString();
                OrderValue = OrderValue.Replace(",", "");

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NFPOsBLL.FPO_Amendment_Search(FrmDt.Replace("'", "''"),new Guid(FPOID), ToDat.Replace("'", "''"), FPONo.Replace("'", "''"), FENo.Replace("'", "''"), Subject.Replace("'", "''"),
                     OrderValue.Replace("'", "''"), Status.Replace("'", "''"), Cust.Replace("'", "''"), CreatedDT, LoginID.ToString(), CusID, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=FPOAmendmentStatus.xls";
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
                        string MTitle = "DETAILS OF FOREIGN PURCHASE ORDERS AMENDMENTS RECEIVED ", MTcustomer = "", MTDTS = "";
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
                        if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["FPO No"].ToString() != "")
                        {
                            string Title = "STATUS OF OF FOREIGN PURCHASE ORDER NO : " + ds.Tables[0].Rows[0]["FPO No"].ToString() + ", dt : "
                                + ds.Tables[0].Rows[0]["Date"].ToString();
                            if (HFCust.Value != "")
                                Title = Title + " RECEIVED FROM " + HFCust.Value.ToUpper();
                            htextw.Write("<center><b>" + Title + "</b></center>");
                        }
                        else
                        {
                            string Title = "STATUS OF OF FOREIGN PURCHASE ORDER ";
                            htextw.Write("<center><b>" + Title + "</b></center>");
                        }
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
                        string headerTable = "<img src='" + CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Purchase Order Status", ex.Message.ToString());
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
    }
}