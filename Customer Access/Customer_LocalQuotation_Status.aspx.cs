using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.IO;
using System.Threading;
using System.Data;
using System.Text;
using Ajax;

namespace VOMS_ERP.Customer_Access
{
    public partial class Customer_LocalQuotation_Status : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        EMailsDetailsBLL EMDBL = new EMailsDetailsBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(Customer_LocalQuotation_Status));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {

                        }
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Web Method

        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public int DeleteItemDetails(string ID)
        {
            try
            {
                int res = 1;
                DataSet dss = new DataSet();
                LQuotaitonBLL NLQBL = new LQuotaitonBLL();
                dss = NLQBL.LclQuoteSelect(CommonBLL.FlagYSelect, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", 0, "", Guid.Empty,
                   CommonBLL.EmptyDtLocalQuotation(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (dss.Tables.Count >= 0 && dss.Tables[0].Rows.Count > 0)
                    res = -123;
                else
                {
                    NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                    res = NLQBL.LclQuoteInsertUpdate(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "",
                        DateTime.Now, DateTime.Now, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, Guid.Empty, "", DateTime.Now, 0, Guid.Empty, 0, "", "", new Guid(Session["UserID"].ToString()),
                        CommonBLL.EmptyDtLocalQuotation(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), CommonBLL.EmptyDtLQ_SubItems(),
                        new Guid(Session["CompanyID"].ToString()));
                }
                return res;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Status", ex.Message.ToString());
                return -1;
            }
        }

        #endregion

        #region Methods (Not In Use)

        /// <summary>
        /// Direct Offer to Cusotmer
        /// </summary>
        /// <param name="ID"></param>
        private void IsOfferToCustomer(Guid ID)
        {
            try
            {
                DataSet MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagBSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (MailDetails != null && MailDetails.Tables.Count > 0)
                {
                    string ToAddrs = MailDetails.Tables[0].Rows[0]["Email"].ToString();
                    string CcAddrs = Session["UserMail"].ToString();

                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), "Quotation Detials from VIPL", InformationEnqDtls(MailDetails.Tables[0].Rows[0]["EnquireNumber"].ToString(),
                    MailDetails.Tables[0].Rows[0]["EnquiryDate"].ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about Quotation Details
        /// </summary>
        /// <returns></returns>
        protected string InformationEnqDtls(string FEnquiryNumber, string FEDate)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Quotaton is for your Enquiry " + System.Environment.NewLine + System.Environment.NewLine);

                sb.Append(" We have an prepared quotation for your Enquiry No. " + FEnquiryNumber + ", Dt: " + Convert.ToDateTime(FEDate).ToString("dd-MM-yyyy") + ". ");
                sb.Append("Please find the Quotation in VOMS Application for the complete details and send your response. ");

                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Status", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        #endregion

        #region Export to Excel Buttons Click Event

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
                Guid LoginID = Guid.Empty;
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
                string LqNo = HFLqNmbr.Value;
                string RFeNo = HFRefFeNmbr.Value;
                string RLeNo = HFRefFeNmbr.Value;
                string Subject = HFSubject.Value;
                string Suplr = HFSuplr.Value;
                string CustomerName = HFCustomer.Value;
                string Status = HFStatus.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";

                DataSet ds = NLQBL.Select_LQSearch(FrmDt, ToDat, LqNo, RLeNo, RFeNo, Subject, Status, Suplr, CustomerName, CreatedDT, LoginID,
                    new Guid(Session["CompanyID"].ToString()));
                string FEnquir = "";
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    FEnquir = ds.Tables[0].Rows[0]["FE Date"].ToString();
                }
                if (ds != null && ds.Tables.Count > 0)
                {

                    string attachment = "attachment; filename=LclQuotations.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF LOCAL QUOTATION(S) RECEIVED", MTcustomer = "", MTDTS = "", MTENQNo = "";
                    if (HFCustomer.Value != "")
                        MTcustomer = HFCustomer.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    if (RFeNo != "")
                        MTENQNo = HFRefFeNmbr.Value;

                    if (MTcustomer != "" && RFeNo == "")
                        MTENQNo = " ENQUIRIES ";
                    else if (MTcustomer == "" && RFeNo == "")
                        MTENQNo = "";
                    else if ((MTcustomer != "" && RFeNo != "") || MTcustomer == "")
                        MTENQNo = ", ENQUIRY No : " + RFeNo + " " + ", Dt :" + FEnquir;

                    htextw.Write("<center><b>" + MTitle + " "
                        + (MTcustomer != "" ? " AGAINST " + MTcustomer.ToUpper() + " " : "")
                        + " " + (MTENQNo == "" ? MTDTS : MTENQNo + " " + (RFeNo != "" ? "" : MTDTS)) + "</center></b>");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Status", ex.Message.ToString());
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