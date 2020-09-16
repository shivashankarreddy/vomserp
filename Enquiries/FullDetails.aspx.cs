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
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace VOMS_ERP.Enquiries
{
    public partial class FullDetails : System.Web.UI.Page
    {
        # region Variables
        int accordions = 0;
        static DataSet EditDS;
        static string GeneralCtgryID;
        static int Itemtables = 0;
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        NewFPOBLL NFPOBL = new NewFPOBLL();
        CustomerBLL cusmr = new CustomerBLL();
        LPOrdersBLL NLPOBL = new LPOrdersBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstCEDtlsBLL RCEDBL = new RqstCEDtlsBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        # endregion

        #region Default Page Load Event

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if ((Session["UserID"] == null || Session["UserID"].ToString() == "") &&
                    CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!IsPostBack)
                    {
                        if (Request.QueryString["FEnqID"] != null && Request.QueryString["FEnqID"] != "")
                        {
                            //GetGeneralID();
                            GetData(new Guid(Request.QueryString["FEnqID"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Bind Default Date

        private void GetGeneralID()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.EnumMasterSelect(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["GeneralCtgryID"] = ds.Tables[0].Rows[0][0].ToString();
                    GeneralCtgryID = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
            }
        }

        private void GetData(Guid ID)
        {
            divFEAll.InnerHtml = BindForeignEnq(ID);
            divFenqAll.InnerHtml = BindLocalEnq(ID);
            LQData.InnerHtml = BindLocalQuotations(ID);
            divFQuotation.InnerHtml = BindForeignQuotation(ID);
            FPData.InnerHtml = BindForeignPurchaseOrders(ID);
            LPOData.InnerHtml = BindLocalPurchaseOrders(ID);
            //CExciseData.InnerHtml = BindCentralExciseDetails(ID);
            //InsptnData.InnerHtml = BindInspectionDetails(ID);
           // Drawings.InnerHtml = DrawingDetails(ID);
            //DivPINVRequest.InnerHtml = ProformaInvoiceRequest(ID);
            //DivIOMtemplate.InnerHtml = IOMTemplateRes(ID);
            //CT1Data.InnerHtml = BindCT1Details(ID);
            DpchInstnData.InnerHtml = BindDespatchInstructionDetails(ID);
            DivGRN.InnerHtml = BindGRNDetails(ID);
            DivCheckListDetails.InnerHtml = BindCheckListDetails(ID);
            DivShipmentProforma.InnerHtml = BindShipmentProformaINvoiceDetails(ID);
            DivPackingList.InnerHtml = BindPackingListDetails(ID);
            DivShpngBillDetails.InnerHtml = BindShippingBillDetails(ID);
            DivAirWayBill.InnerHtml = BindAirWayBillDetails(ID);
            DivBillOfLading.InnerHtml = BindBillOfladingDetails(ID);
            DivMateReceipt.InnerHtml = BindMateReceiptDetails(ID);
            DivEBRCDetails.InnerHtml = BindBRCDetails(ID);

            HFItemsTable.Value = Itemtables.ToString();
        }

        #endregion

        #region Bind Methods

        private string BindForeignEnq(Guid ID)
        {
            try
            {
                string Attachments = "";
                DataTable dt = CommonBLL.EmptyDt();
                EditDS = new DataSet();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                EditDS = NEBLL.NewEnquiryEdit(CommonBLL.FlagCSelect, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, "", "",
                    DateTime.Now, DateTime.Now, DateTime.Now, "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), dt);
                if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < EditDS.Tables[0].Rows.Count; i++)
                    {
                        DataSet dss = new DataSet();
                        dss.Tables.Add(EditDS.Tables[1].Copy());
                        Session["Items"] = dss;//
                        lblFENo.Text = " (Enquiry Date :  " + EditDS.Tables[0].Rows[i]["EnquiryDate"].ToString() + " :: No : " + EditDS.Tables[0].Rows[i]["EnquireNumber"].ToString() + ")";
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustName' class='bcLabel'>Name of Customer ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlcustmr" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["CustName"].ToString() + "' name='ddlcustmr" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDept' class='bcLabel'>Project/Department Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='Ddldeptnm" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["DeptName"].ToString() + "' name='Ddldeptnm" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEnqNo' class='bcLabel'>Enquiry Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfenqy" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["EnquireNumber"].ToString() +
                            "' name='ddlfenqy" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>Subject ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtsubject" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["Subject"].ToString() +
                            "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>Enquiry Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txteqdt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["EnquiryDate"].ToString() + "' name='txteqdt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRcsdt" + i + "' class='bcLabel'>Received Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRcqdt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["ReceivedDate"].ToString() + "' name='txtRcdt" + i +
                            "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDueDt' class='bcLabel'>Due Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDueDt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["DueDate"].ToString() + "' name='txtDueDt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInst' class='bcLabel'>Imp Instructions ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtImpInst" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + EditDS.Tables[0].Rows[i]["Instruction"].ToString() + "' name='txtImpInst" + i +
                            "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Attachments
                        Attachments = EditDS.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (Attachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Foreign Enquiry Attachments", Attachments) + "</td></tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                            "&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td>");//1                        
                        sb.Append(FillGridView("FE"));//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (EditDS.Tables.Count >= 2 && EditDS.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < EditDS.Tables[2].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + EditDS.Tables[2].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + EditDS.Tables[2].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + EditDS.Tables[2].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindLocalEnq(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                LEnquiryBLL LEBLL = new LEnquiryBLL();
                DataTable dt = CommonBLL.EmptyDtLocal();
                DataSet LEDS = LEBLL.SelctLocalEnquiries(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, ID, "", "", Guid.Empty, DateTime.Now,
                    DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 0, "", "", "", new Guid(Session["UserID"].ToString()),
                    DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), dt);
                if (LEDS.Tables.Count > 0 && LEDS.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < LEDS.Tables[0].Rows.Count; i++)
                    {
                        Guid LocalItemID = new Guid(LEDS.Tables[0].Rows[i]["LocalEnquireId"].ToString());
                        DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, LocalItemID,
                            Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                            DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                        Session["Items"] = dss;

                        if (lblLENo.Text == "")//
                        {
                            lblLENo.Text = " (Date : " + LEDS.Tables[0].Rows[i]["LEIssueDate"].ToString() + " :: No : " + LEDS.Tables[0].Rows[i]["EnquireNumber"].ToString() + ")";
                        }
                        else
                        {
                            lblLENo.Text = lblLENo.Text + " & ( Date : " + LEDS.Tables[0].Rows[i]["LEIssueDate"].ToString() + " :: No : " + LEDS.Tables[0].Rows[i]["EnquireNumber"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustName' class='bcLabel'>Name of Customer ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlcustmr" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["CustName"].ToString() + "' name='ddlcustmr" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEnqNo' class='bcLabel'>Enquiry Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfenqy" + i + "' class='bcAsptextbox'" +
                            " type='text' readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["FEnquireNo"].ToString() +
                            "' name='ddlfenqy" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLocalenqno' class='bcLabel'>Local Enquiry Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlenqno" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["EnquireNumber"].ToString() + "' name='txtlenqno" + i +
                            "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblfedt" + i + "' class='bcLabel'>Foreign Enquiry Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtfedt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["EnquiryDate"].ToString() + "' name='txtfedt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblIssdt" + i + "' class='bcLabel'>LE Issue Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlenqdt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["LEIssueDate"].ToString() + "' name='txtlenqdt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRecvDt' class='bcLabel'>Responce Due Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtleduedt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["ResponseDueDate"].ToString() + "' name='txtleduedt" + i +
                            "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>Subject ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtsubject" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtsubject" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDept' class='bcLabel'>Project/Department Name " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='Ddldeptnm" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["DepartmentName"].ToString() + "' name='Ddldeptnm" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEnquiry' class='bcLabel'>Supplier Category<font " +
                            " color='red' size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlsuplrctgry" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["SupCatName"].ToString() + "' name='ddlsuplrctgry" + i +
                            "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='Span2" + i + "' class='bcLabel'>Supplier Name(s) " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='lbsuplrs" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + LEDS.Tables[0].Rows[i]["SupName"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                            "&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td>");//1                        
                        sb.Append(FillGridView("LE"));//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (dss.Tables.Count >= 2 && dss.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int le = 0; le < dss.Tables[2].Rows.Count; le++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>" +
                                    dss.Tables[2].Rows[le]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>" +
                                    dss.Tables[2].Rows[le]["CreatedDate"].ToString() + "'>" + (le + 1) + ") " +
                                    dss.Tables[2].Rows[le]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                else
                    return sb.Append("<b><center> No Local Enquiry was available. </center></b>").ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindLocalQuotations(Guid EnqID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataSet LQDS = NLQBL.LclQuoteSelect(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, EnqID, Guid.Empty, Guid.Empty, "", 0, "", Guid.Empty,
                    CommonBLL.EmptyDtLocalQuotation(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (LQDS.Tables.Count > 0 && LQDS.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < LQDS.Tables[0].Rows.Count; i++)
                    {
                        Guid LocalItemID = new Guid(LQDS.Tables[0].Rows[i]["LclQuoteID"].ToString());
                        DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagYSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                            LocalItemID, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                            DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                        DataSet newDS = new DataSet();
                        newDS.Tables.Add(dss.Tables[1].Copy());
                        Session["Items"] = dss;
                        if (lblLQNo.Text == "")
                        {
                            lblLQNo.Text = " (Date :  " + LQDS.Tables[0].Rows[i]["QuotationDate"].ToString() + " :: No : " + LQDS.Tables[0].Rows[i]["Quotationnumber"].ToString() + ")";
                        }
                        else
                        {
                            lblLQNo.Text = lblLQNo.Text + " & (Date : " + LQDS.Tables[0].Rows[i]["QuotationDate"].ToString() + " :: No : " + LQDS.Tables[0].Rows[i]["Quotationnumber"].ToString() + ")";
                        }

                        Session["PaymentsAll"] = newDS.Tables[0].Copy();

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustName' class='bcLabel'>" +
                            "Name of Customer:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlcustmr" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["CustomerName"].ToString() + "' name='ddlcustmr" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEnqNo' class='bcLabel'>" +
                            "Enquiry Number :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfenqy" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["FEnquireNo"].ToString() + "' name='ddlfenqy" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLocalenqno' class='bcLabel'>" +
                            "Local Enquiry Number :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlenqno" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["LclEnquNmbr"].ToString() + "' name='txtlenqno" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuplr" + i +
                            "' class='bcLabel'>Supplier Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuplr" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["SuplrName"].ToString() + "' name='txtfedt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllqnmbr" + i +
                            "' class='bcLabel'>Quotation Number :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlqnmbr" + i
                            + "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["Quotationnumber"].ToString() + "' name='txtlenqdt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllqDt' class='bcLabel'>" +
                            "Quotation Date:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlqdt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["QuotationDate"].ToString() + "' name='txtleduedt" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQSubject' class='bcLabel'>" +
                            "Subject:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQsubject" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQDept' class='bcLabel'>" +
                            "Project/Department Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='DdlQdeptnm" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["DepartmentName"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQimpinstrctions' class='bcLabel'>" +
                            "Important Instructions:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQImpInst" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["Instruction"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1                        
                        sb.Append(FillGridView("LQ"));//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Terms & Conditions</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQExDt' class='bcLabel'>" +
                            "Excise Duty:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQExDt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["ExDutyPercentage"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQSlTx' class='bcLabel'>" +
                            "Packing:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQSlTx" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["PackingPercentage"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQDscnt' class='bcLabel'>" +
                            "Discount:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQDscnt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQPbBs" + i +
                            "' class='bcLabel'>Price Basis:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["PriceBasis"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQDlPd" + i +
                            "' class='bcLabel'>Delivery Period:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["DeliveryPeriods"].ToString() + "' name='lbsuplrs" + i + "'/></td>");

                        sb.Append("<td class='bcTdnormal'><span id='lblQAdsCh' class='bcLabel'>" +
                            "Additional Charges:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQAdsCh" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LQDS.Tables[0].Rows[i]["AdsnlCharges"].ToString() + "' name='ddladsch" + i + "'/></td>");

                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp; Payment Terms:</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>");
                        sb.Append(FillPaymentTerms());
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        List<string> LEComments = CommonBLL.SplitTextByWord(LQDS.Tables[0].Rows[i]["Comments"].ToString(), "$~#");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (dss.Tables.Count >= 2 && dss.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int lq = 0; lq < dss.Tables[2].Rows.Count; lq++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>" +
                                    dss.Tables[2].Rows[lq]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>" +
                                    dss.Tables[2].Rows[lq]["CreatedDate"].ToString() + "'>" + (lq + 1) + ") " +
                                    dss.Tables[2].Rows[lq]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                return sb.Append("<b><center> No Local Quotation was available. </center></b>").ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindForeignQuotation(Guid ID)
        {
            try
            {
                string FQAttachments = "";
                NewFQuotationBLL NFBLL = new NewFQuotationBLL();
                DataTable dt = CommonBLL.EmptyDtLocal();
                DataSet FQDS = NFBLL.Select(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty, Guid.Empty, ID, "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "",
                    DateTime.Now, 0, Guid.Empty, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, CommonBLL.EmptyDtFQ(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));
                if (FQDS.Tables.Count > 0 && FQDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < FQDS.Tables[0].Rows.Count; i++)
                    {
                        Guid FQItemID = new Guid(FQDS.Tables[0].Rows[0]["ForeignQuotationId"].ToString());
                        DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, FQItemID,
                            Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                            DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                        DataSet newDS = new DataSet();
                        newDS.Tables.Add(dss.Tables[1].Copy());
                        Session["Items"] = dss;

                        if (lblFQNo.Text == "")
                        {
                            lblFQNo.Text = " (Date : " + FQDS.Tables[0].Rows[i]["QuotationDate"].ToString() + " :: No : " + FQDS.Tables[0].Rows[i]["FQuotationNumber"].ToString() + ")";
                        }
                        else
                        {
                            lblFQNo.Text = lblFQNo.Text + " & (Date : " + FQDS.Tables[0].Rows[i]["QuotationDate"].ToString() + " :: No : " + FQDS.Tables[0].Rows[i]["FQuotationNumber"].ToString() + ")";
                        }

                        Session["PaymentsAll"] = newDS.Tables[0].Copy();
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>");
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustName' class='bcLabel'>Name of Customer: " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlcustmr" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["CustName"].ToString() + "' name='ddlcustmr" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEnqNo' class='bcLabel'>Foreign Enquiry Number: " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfenqy" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["FEnquireNumber"].ToString() + "' name='ddlfenqy" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFQuotationNumber' class='bcLabel'>Foreign Quotation Number: " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFQuotationNumber" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["FQuotationNumber"].ToString() +
                            "' name='txtFQuotationNumber" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>Subject:<font " +
                            " color='red' size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtsubject" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtsubject" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDept' class='bcLabel'>Project/Department Name: " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='Ddldeptnm" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["DeptName"].ToString() +
                            "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblQuotationdt" + i + "' class='bcLabel'>Quotation Date: " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtQuotationdt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + FQDS.Tables[0].Rows[i]["QuotationDate"].ToString() +
                            "' name='txtQuotationdt" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInstructions" + i +
                            "' class='bcLabel'>Important Instructions:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInstructions" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["Instruction"].ToString() + "' name='txtInstructions" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPriceBasis' class='bcLabel'>Price Basis:" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPriceBasis" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["PriceBasis"].ToString() + "' name='txtPriceBasis" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDeliveryPeriod' class='bcLabel'> " +
                            "Delivery Period::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDeliveryPeriod" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["DeliveryPeriod"].ToString() + "' name='txtDeliveryPeriod" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'> " +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Terms & Conditions :</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='TotalAmount" + i + "' class='bcLabel'>TotalAmount(RS):</span></td>");
                        sb.Append("<td class='bcTdnormal'><b><input id='txtTotalAmount" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["TotalAmount"].ToString() + "' name='txtTotalAmount" + i + "'/></b></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFobMargin' class='bcLabel'>" +
                            "Fob + Margin(%)::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFobMargin" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["FobMargin"].ToString() + "' name='txtFobMargin" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblConversionRate' class='bcLabel'>" +
                            "Conversion Rate::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtConversionRate" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FQDS.Tables[0].Rows[i]["ConversionRate"].ToString() + "' name='txtConversionRate" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Attachments
                        FQAttachments = FQDS.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (FQAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Foreign Quotation Attachments", FQAttachments) + "</td></tr>");


                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append(FillGridView("FQ"));//Items
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp; Payment Terms:</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>");
                        sb.Append(FillPaymentTerms());
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (dss.Tables.Count >= 2 && dss.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int fq = 0; fq < dss.Tables[2].Rows.Count; fq++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>" +
                                    dss.Tables[2].Rows[fq]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>" +
                                    dss.Tables[2].Rows[fq]["CreatedDate"].ToString() + "'>" + (fq + 1) + ") " +
                                    dss.Tables[2].Rows[fq]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                else
                    return "<b><center> No Foreign Quotation was available. </center></b>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindForeignPurchaseOrders(Guid EnqID)
        {
            try
            {
                DataSet FPDS = NFPOBL.Select(CommonBLL.FlagFSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, EnqID, DateTime.Now, Guid.Empty.ToString(), DateTime.Now,
                    "", "", "",DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "",
                    new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true,
                    CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                if (FPDS.Tables.Count > 0 && FPDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string FPONos = "";
                    for (int i = 0; i < FPDS.Tables[0].Rows.Count; i++)
                    {
                        Guid LocalItemID = new Guid(FPDS.Tables[0].Rows[i]["ForeignPurchaseOrderId"].ToString());
                        DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                            Guid.Empty, LocalItemID, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty,
                            DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                        DataSet newDS = new DataSet();
                        newDS.Tables.Add(dss.Tables[1].Copy());
                        Session["Items"] = dss;
                        if (lblFPONos.Text == "")
                        {
                            lblFPONos.Text = " (Date : " + Convert.ToDateTime(FPDS.Tables[0].Rows[i]["FPODate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + FPDS.Tables[0].Rows[i]["ForeignPurchaseOrderNo"].ToString() + ")";
                        }
                        else
                        {
                            lblFPONos.Text = lblFPONos.Text + " & (Date : " + Convert.ToDateTime(FPDS.Tables[0].Rows[i]["FPODate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + FPDS.Tables[0].Rows[i]["ForeignPurchaseOrderNo"].ToString() + ")";
                        }

                        Session["PaymentsAll"] = newDS.Tables[0].Copy();
                        FPONos += FPDS.Tables[0].Rows[i]["ForeignPurchaseOrderNo"].ToString() + ", ";
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblfpono' class='bcLabel'>Customer Name " +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfpono" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["CustomerName"].ToString() + "' name='ddlcustmr" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblfqteno' class='bcLabel'>" +
                            "Frn Enquiry No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfqteno" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["FrnEnqID"].ToString() + "' name='ddlfenqy" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblflenqno' class='bcLabel'>" +
                            "Frn Enquiry Date :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtfenqno" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='" + FPDS.Tables[0].Rows[i]["FrnEnqDate"].ToString()
                            + "' name='txtlenqno" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllpdueDt' class='bcLabel'>" +
                            "Department Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlpduedt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["DeptNm"].ToString() + "' name='txtleduedt" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuplrCtgry" + i +
                            "' class='bcLabel'>Frn Purchase Order No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuplrCtgry" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["ForeignPurchaseOrderNo"].ToString() + "' name='txtfedt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuplrNm" + i +
                            "' class='bcLabel'>FPO Date:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuplrNm" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + CommonBLL.DateDisplay_1(Convert.ToDateTime(FPDS.Tables[0].Rows[i]["FPODate"].ToString()))
                            + "' name='txtlenqdt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllqNo' class='bcLabel'>" +
                            "FPO Due Date :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlqNo" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + CommonBLL.DateDisplay_1(Convert.ToDateTime(FPDS.Tables[0].Rows[i]["FPODueDate"].ToString()))
                            + "' name='txtleduedt" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPSubject' class='bcLabel'>" +
                            "Subject:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPsubject" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDept' class='bcLabel'>" +
                            "Department Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='DdlLPdeptnm" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["FPRaiseBy"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPimpinstrctions' class='bcLabel'>" +
                            "Important Instructions:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPImpInst" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["Instruction"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1                        
                        sb.Append(FillGridView("FPO"));//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Terms & Conditions</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPPbBs" + i +
                            "' class='bcLabel'>Price Basis:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["PriceBasis"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDlPd" + i +
                            "' class='bcLabel'>Delivery Period:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + FPDS.Tables[0].Rows[i]["DeliveryPeriod"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp; Payment Terms:</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>");
                        sb.Append(FillPaymentTerms());
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        List<string> LEComments = CommonBLL.SplitTextByWord(FPDS.Tables[0].Rows[i]["Comments"].ToString(), "$~#");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (dss.Tables.Count >= 2 && dss.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int fp = 0; fp < dss.Tables[2].Rows.Count; fp++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>" +
                                        dss.Tables[2].Rows[fp]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>" +
                                        dss.Tables[2].Rows[fp]["CreatedDate"].ToString() + "'>" + (fp + 1) + ") " +
                                        dss.Tables[2].Rows[fp]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    //lblFPONos.Text = " ( " + FPONos.Trim(' ').Trim(',') + " )";
                    return sb.ToString();
                }
                return ("<b><center> No Foreign Purchase Order was available. </center></b>");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindLocalPurchaseOrders(Guid EnqID)
        {
            try
            {
                DataSet LPDS = NLPOBL.SelectLPOrders(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, "", "", Guid.Empty, EnqID, Guid.Empty,
                    DateTime.Now, DateTime.Now, Guid.Empty, "", DateTime.Now, 0, 0, "", new Guid(Session["UserID"].ToString()),
                    CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), 0, 0, 0, 0, 0, 0,
                    0, 0, new Guid(Session["CompanyID"].ToString()), "");
                if (LPDS.Tables.Count > 0 && LPDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    string LPONos = "";
                    for (int i = 0; i < LPDS.Tables[0].Rows.Count; i++)
                    {
                        Guid LocalItemID = new Guid(LPDS.Tables[0].Rows[i]["LocalPurchaseOrderId"].ToString());
                        DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                            Guid.Empty, Guid.Empty, Guid.Empty, LocalItemID, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty,
                            DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                        DataSet newDS = new DataSet();
                        newDS.Tables.Add(dss.Tables[1].Copy());
                        Session["Items"] = dss;
                        Session["PaymentsAll"] = newDS.Tables[0].Copy();
                        LPONos += LPDS.Tables[0].Rows[i]["LocalPurchaseOrderNo"].ToString() + ", ";

                        if (lblLPONos.Text == "")
                        {
                            lblLPONos.Text = " (Date : " + LPDS.Tables[0].Rows[i]["LPOrderDate"].ToString() + " :: No : " + LPDS.Tables[0].Rows[i]["LocalPurchaseOrderNo"].ToString() + ")";
                        }
                        else
                        {
                            lblLPONos.Text = lblLPONos.Text + " & (Date : " + LPDS.Tables[0].Rows[i]["LPOrderDate"].ToString() + " :: No : " + LPDS.Tables[0].Rows[i]["LocalPurchaseOrderNo"].ToString() + ")";
                        }


                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblfpono' class='bcLabel'>" +
                            "Frn Purchase Order No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='ddlfpono" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["FPOrderNmbr"].ToString() + "' name='ddlcustmr" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblflenqno' class='bcLabel'>" +
                            "Frn Enquiry No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtfenqno" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["FrnEnqNo"].ToString() + "' name='txtlenqno" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllpono" + i +
                            "' class='bcLabel'>Lcl Purchase Order No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlpono" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["LocalPurchaseOrderNo"].ToString() + "' name='txtfedt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllpodt" + i +
                            "' class='bcLabel'>LPO Date:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlpodt" + i
                            + "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["LPOrderDate"].ToString() + "' name='txtlenqdt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllpdueDt' class='bcLabel'>" +
                            "LPO Due Date:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlpduedt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + CommonBLL.DateDisplay_1(Convert.ToDateTime(LPDS.Tables[0].Rows[i]["LocalPurchaseOrderDueDate"].ToString()))
                            + "' name='txtleduedt" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuplrCtgry" + i +
                            "' class='bcLabel'>Supplier Category :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuplrCtgry" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["SuplCatgry"].ToString() + "' name='txtfedt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuplrNm" + i +
                            "' class='bcLabel'>Supplier Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuplrNm" + i
                            + "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["SuplrNm"].ToString() + "' name='txtlenqdt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbllqNo' class='bcLabel'>" +
                            "Lcl Quotation No. :</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtlqNo" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["QuotationNo"].ToString() + "' name='txtleduedt" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPSubject' class='bcLabel'>" +
                            "Subject:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPsubject" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDept' class='bcLabel'>" +
                            "Department Name:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='DdlLPdeptnm" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["DeptNm"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPimpinstrctions' class='bcLabel'>" +
                            "Important Instructions:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPImpInst" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["Instruction"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1                        
                        sb.Append(FillGridView("LPO"));//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Tracking</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPinspt' class='bcLabel'>" +
                            "Is Inspection:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPinspt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["Inspection"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPiceea' class='bcLabel'>" +
                            " Is Central Excise Excemption Applicable:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPiceea" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["IsCentralExcise"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDscnt' class='bcLabel'>" +
                            "Drawing Approvals:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPDrawingApproval" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["DrwngAprls"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");


                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Terms & Conditions</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPExDt' class='bcLabel'>" +
                            "Excise Duty:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPExDt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["ExDutyPercentage"].ToString() + "' name='txtsubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPSlTx' class='bcLabel'>" +
                            "Sales Tax:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPSlTx" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["SaleTaxPercentage"].ToString() + "' name='Ddldeptnm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPPking' class='bcLabel'>" +
                            "Packing:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPPking" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["PackingPercentage"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDscnt' class='bcLabel'>" +
                            "Discount:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPDscnt" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["DiscountPercentage"].ToString() + "' name='ddlsuplrctgry" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPPbBs" + i +
                            "' class='bcLabel'>Price Basis:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["PriceBasis"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPDlPd" + i +
                            "' class='bcLabel'>Delivery Period:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPPbBs" + i +
                            "' class='bcAsptextbox' type='text' readonly='readonly' value='"
                            + LPDS.Tables[0].Rows[i]["DeliveryPeriod"].ToString() + "' name='lbsuplrs" + i + "'/></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp; Payment Terms:</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>");
                        sb.Append(FillPaymentTerms());
                        sb.Append("</td>");
                        sb.Append("</tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'>" +
                            "<td colspan='6'>&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        List<string> LEComments = CommonBLL.SplitTextByWord(LPDS.Tables[0].Rows[i]["Comments"].ToString(), "$~#");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (dss.Tables.Count >= 2 && dss.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int lp = 0; lp < dss.Tables[2].Rows.Count; lp++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>" +
                                    dss.Tables[2].Rows[lp]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>" +
                                    dss.Tables[2].Rows[lp]["CreatedDate"].ToString() + "'>" + (lp + 1) + ") " +
                                    dss.Tables[2].Rows[lp]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    //lblLPONos.Text = " ( " + LPONos.Trim(' ').Trim(',') + " )";
                    return sb.ToString();
                }
                return ("<b><center> No Local Purchase Order was available. </center></b>");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindInspectionDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string Attachments = "";
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet InsptDt = FeBLL.SelectEnqiryHead(CommonBLL.FlagSelectAll, ID, Guid.Empty, Guid.Empty);
                if (InsptDt.Tables.Count > 2 && InsptDt.Tables[0].Rows.Count > 0 && InsptDt.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < InsptDt.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='PlanRefNo' class='bcLabel'>Plan Ref. No. :<font color='red' " +
                            " size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='PlanRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["InspPlanNo"].ToString() + "' name='PlanRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='CustomerName' class='bcLabel'>Customer Name :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='CustomerName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["CustomerNm"].ToString() + "' name='CustomerName" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierName" + i + "' class='bcLabel'>Supplier Name:" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierName" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["SupplierNm"].ToString() +
                            "' name='txtSupplierName" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPOs" + i + "' class='bcLabel'>FPO(s)::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtFPOs" + i + "' >" + InsptDt.Tables[0].Rows[i]["FPOs"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>LPO(s):</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtLPOs" + i + "' >" + InsptDt.Tables[0].Rows[i]["LPOs"].ToString() + " </textarea></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='PlaceofInspection' class='bcLabel'>Place of Inspection :<font color='red' " +
                            " size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofInspection" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["InsPlace"].ToString() + "' name='txtPlaceofInspection" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='ContactPerson' class='bcLabel'>Contact Person :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtContactPerson" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["ContactPerson"].ToString() + "' name='txtContactPerson" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblContactNumber" + i + "' class='bcLabel'>Contact Number :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtContactNumber" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["ContactNumber"].ToString() +
                            "' name='txtContactNumber" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblStageofInspection' class='bcLabel'>Stage of Inspection :<font color='red' " +
                            " size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtStageofInspection" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["InsStage"].ToString() + "' name='txtStageofInspection" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblThirdPartInspector' class='bcLabel'>Third Part Inspector :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtThirdPartInspector" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["ThirdPartyID"].ToString() + "' name='txtThirdPartInspector" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSelfInspector" + i + "' class='bcLabel'>Self Inspector :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSelfInspector" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["SelfInspector"].ToString() +
                            "' name='txtSelfInspector" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInspectionDate' class='bcLabel'>Inspection Date :<font color='red' " +
                            " size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInspectionDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(InsptDt.Tables[0].Rows[i]["InsDate"].ToString()))
                            + "' name='txtInspectionDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblStartDate' class='bcLabel'>Start Date :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtStartDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(InsptDt.Tables[0].Rows[i]["StartDate"].ToString()))
                            + "' name='txtStartDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEndDate" + i + "' class='bcLabel'>End Date :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtEndDate" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(InsptDt.Tables[0].Rows[i]["EndDate"].ToString()))
                            + "' name='txtEndDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInspectionStatus' class='bcLabel'>Inspection Status :<font color='red' " +
                            " size='2'></font>:</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInspectionStatus" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["InsStatus"].ToString() + "' name='txtInspectionStatus" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInspectionDetails' class='bcLabel'>Inspection Details :" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInspectionDetails" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["InsDetails"].ToString() + "' name='txtInspectionDetails" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDispatchReadinessDt" + i + "' class='bcLabel'>Dispatch Readiness Dt:" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDispatchReadinessDt" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(InsptDt.Tables[0].Rows[i]["DispReadinessDate"].ToString()))
                            + "' name='txtDispatchReadinessDt" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");



                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (InsptDt.Tables.Count > 2 && InsptDt.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < InsptDt.Tables[2].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + InsptDt.Tables[2].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + InsptDt.Tables[2].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + InsptDt.Tables[2].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else if (InsptDt.Tables[1].Rows.Count > 0 && InsptDt.Tables[1].Rows[0][0].ToString() != "")
                {
                    if (Convert.ToBoolean(InsptDt.Tables[1].Rows[0][0].ToString()))
                        sb.Append("<b><center> Inspection Report has been pending. </center></b>");
                    else if (!Convert.ToBoolean(InsptDt.Tables[1].Rows[0][0].ToString()))
                        sb.Append("<b><center> No Inspection was available. </center></b>");
                }
                else
                    sb.Append("<b><center> No Inspection was available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string DrawingDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string Attachments = "";
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet InsptDt = FeBLL.SelectEnqiryHead(CommonBLL.FlagKSelect, ID, Guid.Empty, Guid.Empty); // in progress this query
                if (InsptDt.Tables.Count > 0 && InsptDt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < InsptDt.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomerName' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomerName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["CustomerId"].ToString() + "' name='txtCustomerName" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierCategory' class='bcLabel'>Supplier Category ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierCategory" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["SupCat"].ToString() + "' name='txtSupplierCategory" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierName" + i + "' class='bcLabel'>Supplier Name:" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierName" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["SupNm"].ToString() +
                            "' name='txtSupplierName" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPOs" + i + "' class='bcLabel'>FPO(s)::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea  id='txtFPOs" + i + "' class='bcAsptextboxmulti' " +
                            " readonly='readonly' name='txtFPOs" + i + "' >" + InsptDt.Tables[0].Rows[i]["FPONumber"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>LPO(s):</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPOs" + i + "' class='bcAsptextboxmulti' " +
                            " readonly='readonly' name='txtLPOs" + i + "' >" + InsptDt.Tables[0].Rows[i]["LPONumber"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPODate" + i + "' class='bcLabel'>LPO Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLPODate" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["LPODT"].ToString() +
                            "' name='txtLPODate" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDrawingRefNo' class='bcLabel'>Drawing Ref. No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDrawingRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='----' name='txtDrawingRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblReceivedDate' class='bcLabel'>Received Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtReceivedDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["RcvdDT"].ToString()
                            + "' name='txtReceivedDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblApprovalReqDt" + i + "' class='bcLabel'>Approval Req. Dt. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtApprovalReqDt" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["AppRqstDT"].ToString()
                            + "' name='txtApprovalReqDt" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustResponseDate' class='bcLabel'>Cust. Response Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustResponseDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["CustrspDT"].ToString()
                            + "' name='txtCustResponseDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSuppIntimationDate' class='bcLabel'>Supp. Intimation Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSuppIntimationDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["SupIntDT"].ToString()
                            + "' name='txtSuppIntimationDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblResponseStatus" + i + "' class='bcLabel'>Response Status ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtResponseStatus" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='----' name='txtResponseStatus" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRemarks' class='bcLabel'>Remarks ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRemarks" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + InsptDt.Tables[0].Rows[i]["Remarks"].ToString() + "' name='txtRemarks" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSelfApprovalDate' class='bcLabel'>Self Approval Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSelfApprovalDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" +
                            (InsptDt.Tables[0].Rows[i]["SlfAppDT"].ToString() == "" ? "" : InsptDt.Tables[0].Rows[i]["SlfAppDT"].ToString())
                            + "' name='txtSelfApprovalDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        Guid DrawingID = new Guid(InsptDt.Tables[0].Rows[i]["DrawingID"].ToString());
                        DataSet ds = new DataSet();
                        ds = FeBLL.SelectEnqiryHead(CommonBLL.FlagLSelect, DrawingID, Guid.Empty, Guid.Empty);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<tr><td>");

                            # region Drawing Details
                            DataTable dt = ds.Tables[0].Copy();

                            sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner'" +
                            " id='DrawingAppTbl'><thead align='left'><tr>");
                            sb.Append("<th class='rounded-First'>SNo</th><th>Drawing Ref.No.</th><th>Description</th><th>Comments</th><th class='rounded-Last'>Res. Status</th>");
                            sb.Append("</tr></thead><tbody>");

                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                string SNo = (j + 1).ToString();
                                sb.Append("<tr>");
                                sb.Append("<td valign='top'>" + SNo + "</td>");

                                sb.Append("<td valign='top'><input type='text' readonly='readonly' name='txtDrawingRefNo' class='bcAsptextbox' id='txtDrawingRefNo" + SNo + "'" +
                                    " value='" + dt.Rows[j]["DrawingRefNo"].ToString() + "' style='width:150px;'/></td>");

                                sb.Append("<td valign='top'><textarea readonly='readonly' name='txtDescription' id='txtDescription" + SNo + "' class='bcAsptextboxmulti' "
                                    + "onfocus='ExpandTXT(this.id)' onblur='ReSizeTXT(this.id)' style='height:20px; width:200px; resize:none;'>"
                                    + dt.Rows[j]["Description"].ToString() + "</textarea></td>");

                                sb.Append("<td valign='top'><textarea readonly='readonly' name='txtComments' class='bcAsptextbox' id='txtComments" + SNo + "' "
                                    + "class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' onblur='ReSizeTXT(this.id)' style='height:20px; width:200px; resize:none;'>"
                                    + dt.Rows[j]["Comments"].ToString() + "</textarea></td>");

                                #region Fill Status Dropdown
                                string stat = dt.Rows[j]["status"].ToString();
                                sb.Append("<td valign='top'>");
                                sb.Append("<input type='text' readonly='readonly' name='txtStatus' class='bcAsptextbox' id='txtStatus" + SNo + "' style='width:80px;'");
                                if (stat != "1")
                                    sb.Append(" value='Rejected' />");
                                else
                                    sb.Append(" value='Accepted' />");
                                sb.Append("</td>");
                                #endregion

                                sb.Append("</tr>");
                            }
                            sb.Append("</tbody>");
                            sb.Append("<tfoot><th class='rounded-foot-left'></th><th colspan='3' style='height:17px;'></th>");
                            sb.Append("<th class='rounded-foot-right'></th></tfoot></table>");

                            # endregion

                            sb.Append("</tr></td>");
                        }


                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (InsptDt.Tables.Count >= 2 && InsptDt.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < InsptDt.Tables[2].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + InsptDt.Tables[2].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + InsptDt.Tables[2].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + InsptDt.Tables[2].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else if (InsptDt.Tables[1].Rows.Count > 0 && InsptDt.Tables[1].Rows[0][0].ToString() != "")
                {
                    if (Convert.ToBoolean(InsptDt.Tables[1].Rows[0][0].ToString()))
                        sb.Append("<b><center> Drawing Report has been pending. </center></b>");
                    else if (!Convert.ToBoolean(InsptDt.Tables[1].Rows[0][0].ToString()))
                        sb.Append("<b><center> No Drawing was available. </center></b>");
                }
                else
                    sb.Append("<b><center> No Drawing was available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string ProformaInvoiceRequest(Guid ID)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                DataSet ds = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagCSelect, ID, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomerName' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomerName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["CustomerNm"].ToString() + "' name='txtCustomerName" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierCategory' class='bcLabel'>Supplier Category ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierCategory" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["SupCtgry"].ToString() + "' name='txtSupplierCategory" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierName" + i + "' class='bcLabel'>Supplier Name:" +
                            ":</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierName" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ds.Tables[0].Rows[i]["SuplrNm"].ToString() +
                            "' name='txtSupplierName" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPOs" + i + "' class='bcLabel'>FPO(s)::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtFPOs" + i + "' >" + ds.Tables[0].Rows[i]["FPONos"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>LPO(s):</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtLPOs" + i + "' >" + ds.Tables[0].Rows[i]["LPONos"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefNo" + i + "' class='bcLabel'>Ref. No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ds.Tables[0].Rows[i]["RefNo"].ToString() +
                            "' name='txtRefNo" + i + "'/></td>");
                        sb.Append("</tr>");

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Attachments
                        string PInvAttachments = "";
                        PInvAttachments = ds.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (PInvAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Proforma Invoice Request Attachments", PInvAttachments) + "</td></tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[1].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[1].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[1].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[1].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                else
                    return ("<b><center> No Proforma Invoice Request was available. </center></b>");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ex.Message;
            }
        }

        private string IOMTemplateRes(Guid ID)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                IOMTemplateBLL IOMTBLL = new IOMTemplateBLL();
                DataSet ds = IOMTBLL.GetData(CommonBLL.FlagCSelect, ID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //if (lblIOMTNo.Text == "")
                        //{
                        //    lblIOMTNo.Text = " ( " + ds.Tables[0].Rows[i]["IOMRefNo"].ToString() + " :: Date : " + ds.Tables[0].Rows[i]["IOMDate"].ToString() + ")";
                        //}
                        //else
                        //{
                        //    lblIOMTNo.Text = lblIOMTNo.Text + " & ( " + ds.Tables[0].Rows[i]["IOMRefNo"].ToString() + " :: Date : " + ds.Tables[0].Rows[i]["IOMDate"].ToString() + ")";
                        //}

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefNo' class='bcLabel'>Ref. No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["IOMRefNo"].ToString() + "' name='txtRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["IOMDate"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPinvReqNo" + i + "' class='bcLabel'>PInv. Req No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPinvReqNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ds.Tables[0].Rows[i]["RefNo"].ToString() +
                            "' name='txtPinvReqNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblToAddress' class='bcLabel'>To Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtToAddress" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["ToEmailID"].ToString() + "' name='txtToAddress" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFromAddress' class='bcLabel'>From Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFromAddress" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["FromEmailID"].ToString() + "' name='txtFromAddress" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPassword" + i + "' class='bcLabel'>Password ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPassword" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='*******' name='txtSupplierName" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNo' class='bcLabel'>Proforma Invoice No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["ProformaInvoice"].ToString() + "' name='txtProformaInvoiceNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPIVDate' class='bcLabel'> P.Invoice Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPIVDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["ProformaDate"].ToString() + "' name='txtPIVDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject" + i + "' class='bcLabel'>Subject ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSubject" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ds.Tables[0].Rows[i]["Subject"].ToString() +
                            "' name='txtSubject" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["CustmNm"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplier' class='bcLabel'>Supplier ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplier" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ds.Tables[0].Rows[i]["SuplrNm"].ToString() + "' name='txtSupplier" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblStatus" + i + "' class='bcLabel'>Status ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtStatus" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ds.Tables[0].Rows[i]["Status"].ToString() +
                            "' name='txtStatus" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPOs" + i + "' class='bcLabel'>FPO(s)::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtFPOs" + i + "' >" + ds.Tables[0].Rows[i]["FPONos"].ToString() + " </textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>LPO(s):</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPOs" + i + "' class='bcAsptextboxmulti' " +
                            " type='text' readonly='readonly' name='txtLPOs" + i + "' >" + ds.Tables[0].Rows[i]["LPONos"].ToString() + " </textarea></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMessageBody" + i + "' class='bcLabel'>Message Body ::</span></td>");
                        sb.Append("<td class='bcTdnormal' colspan='5'><textarea id='txtMessageBody" + i + "' class='bcAsptextboxmulti' " +
                            " style='height:100px;width:96%;' type='text' readonly='readonly' name='txtMessageBody" + i + "' >" + ds.Tables[0].Rows[i]["Body"].ToString() +
                            " </textarea></td>");
                        sb.Append("</tr>");

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        // Attachments
                        string PInvAttachments = "";
                        PInvAttachments = ds.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (PInvAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "IOM (Inter Office Memo) Attachments", PInvAttachments) + "</td></tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(ds.Tables[0].Rows[i]["IOMID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet dss = new DataSet();
                        dss = cbll.GetComments(CommonBLL.FlagESelect, CommentsID, "");
                        if (dss.Tables.Count > 1 && dss.Tables[1].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[1].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + dss.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + dss.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + dss.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                    return sb.ToString();
                }
                else
                    return ("<b><center> No IOM (Inter Office Memo) Template was available. </center></b>");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ex.Message;
            }
        }

        private string BindCT1Details(Guid ID)
        {
            try
            {
                decimal DscntPrcnt = 0;
                decimal ExcsePrcnt = 0;
                decimal PkngPrcnt = 0;
                StringBuilder sb = new StringBuilder();
                string Attachments = "";
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet CTOneDt = FeBLL.SelectEnqiryHead(CommonBLL.FlagXSelect, ID, Guid.Empty, Guid.Empty);
                if (CTOneDt.Tables.Count > 0 && CTOneDt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < CTOneDt.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>");
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Proforma Invoice
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Proforma Invoice</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblIOMRefNo.' class='bcLabel'>IOM Ref No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtIOMRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["IOMRefNo"].ToString() + "' name='txtIOMRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["CustomerNm"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate" + i + "' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(CTOneDt.Tables[0].Rows[i]["ProformaDate"].ToString()))
                            + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierCategory' class='bcLabel'>Supplier Category ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierCategory" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["SupplierCtgry"].ToString() + "' name='txtSupplierCategory" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplier' class='bcLabel'>Supplier ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplier" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["SupplierNm"].ToString() + "' name='txtSupplier" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoice" + i + "' class='bcLabel'>Proforma Invoice ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoice" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["ProformaInvoice"].ToString() +
                            "' name='txtProformaInvoice" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>FPO ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + CTOneDt.Tables[0].Rows[i]["FPONOs"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPONOs' class='bcLabel'>LPO ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtLPONOs" + i + "'/>" + CTOneDt.Tables[0].Rows[i]["LPONOs"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>Subject ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSubject" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='' name='txtSubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblImportantInstructions' class='bcLabel'>Important Instructions ::</span></td>");
                        sb.Append("<td class='bcTdnormal' colspan='3'><textarea id='txtImportantInstructions" + i + "' style='height:38px;width: 555px;' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtImportantInstructions" + i +
                            "'/>" + CTOneDt.Tables[0].Rows[i]["Body"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        DataSet dss = new DataSet();
                        DataSet ds = new DataSet();
                        DataSet ds1 = new DataSet();
                        Guid PinvID = new Guid(CTOneDt.Tables[0].Rows[i]["PInvID"].ToString());
                        Guid PinvReqID = new Guid(CTOneDt.Tables[0].Rows[i]["PInvReqID"].ToString());//CustomerID
                        Guid CT1ID = new Guid(CTOneDt.Tables[0].Rows[i]["CT1ID"].ToString());//DeptID
                        ds = FeBLL.SelectEnqiryHead(CommonBLL.FlagJSelect, PinvID, PinvReqID, CT1ID);
                        if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                        {
                            DscntPrcnt = Convert.ToDecimal(CTOneDt.Tables[0].Rows[i]["DscntPrcnt"].ToString());
                            ExcsePrcnt = Convert.ToDecimal(CTOneDt.Tables[0].Rows[i]["ExcsePrcnt"].ToString());
                            PkngPrcnt = Convert.ToDecimal(CTOneDt.Tables[0].Rows[i]["PkngPrcnt"].ToString());
                            DataColumn DCTotalAmt = new DataColumn("TotalAmt", typeof(decimal));
                            DCTotalAmt.DefaultValue = 0;
                            ds.Tables[0].Columns.Add(DCTotalAmt);

                            DataColumn DCTotalAmt1 = new DataColumn("TotalAmt", typeof(decimal));
                            DCTotalAmt1.DefaultValue = 0;
                            ds.Tables[1].Columns.Add(DCTotalAmt1);

                            DataColumn dcCol = new DataColumn("IsChecked", typeof(bool));
                            dcCol.DefaultValue = false;
                            ds.Tables[1].Columns.Add(dcCol);

                            for (int k = 0; k < ds.Tables[1].Rows.Count; k++)
                            {
                                Guid itemId = new Guid(ds.Tables[1].Rows[k]["ItemId"].ToString());
                                DataRow[] foundRows = ds.Tables[0].Select("ItemId = '" + itemId + "'");

                                if (foundRows.Length > 0)
                                {
                                    double Quantity = Convert.ToDouble(foundRows[0]["Quantity"].ToString());
                                    double Rate = Convert.ToDouble(foundRows[0]["Rate"].ToString());
                                    double ExDutyPercentage = 0;
                                    if (foundRows[0]["ExDutyPercentage"].ToString() != "")
                                        ExDutyPercentage = Convert.ToDouble(foundRows[0]["ExDutyPercentage"].ToString());
                                    double PackingPercentage = 0;
                                    if (foundRows[0]["PackingPercentage"].ToString() != "")
                                        PackingPercentage = Convert.ToDouble(foundRows[0]["PackingPercentage"].ToString());

                                    ds.Tables[1].Rows[k]["HSCode"] = foundRows[0]["HSCode"].ToString();
                                    ds.Tables[1].Rows[k]["Quantity"] = Quantity;
                                    ds.Tables[1].Rows[k]["Rate"] = Rate;
                                    ds.Tables[1].Rows[k]["ExDutyPercentage"] = ExDutyPercentage;
                                    ds.Tables[1].Rows[k]["PackingPercentage"] = PackingPercentage;
                                    ds.Tables[1].Rows[k]["IsChecked"] = true;
                                }
                            }
                            ds.Tables[1].AcceptChanges();
                            DataTable dt = new DataTable();
                            dt = ds.Tables[1].Copy();

                            dt = CTOne_Calculations(dt, DscntPrcnt, PkngPrcnt, ExcsePrcnt);
                            ds1.Tables.Add(dt);
                            Session["Items"] = ds1;
                        }
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                            "&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1                        
                        if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                            sb.Append(FillGridView("CT1"));//Items
                        else
                            sb.Append("<b><center> No Items to show </center></b>");//Items
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                                "&nbsp;&nbsp;&nbsp;Terms & Conditions</td></tr>");//Added Items
                            sb.Append("<tr>");
                            sb.Append("<td class='bcLabel'>Discount :: </td><td>" + DscntPrcnt.ToString("N") + "</td>");
                            sb.Append("<td class='bcLabel'>Packing :: </td><td>" + PkngPrcnt.ToString("N") + "</td>");
                            sb.Append("<td class='bcLabel'>Excise Duty :: </td><td>" + ExcsePrcnt.ToString("N") + "</td>");
                            sb.Append("</tr>");
                        }
                        # endregion

                        # region Ex-Details
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Ex-Details</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCentralExDutyAmt' class='bcLabel'>Central Ex-Duty Amt ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCentralExDutyAmt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["CExDAmount"].ToString() + "' name='txtCentralExDutyAmt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblExRegNo' class='bcLabel'>Ex-Reg No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtExRegNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["ExRegNo"].ToString() + "' name='txtExRegNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblECCNo" + i + "' class='bcLabel'>ECC No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtECCNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["ECCNo"].ToString() +
                            "' name='txtECCNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRange' class='bcLabel'>Range ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRange" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["ExRange"].ToString() + "' name='txtRange" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDivision' class='bcLabel'>Division ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDivision" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["Division"].ToString() + "' name='txtDivision" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCommissioneRate" + i + "' class='bcLabel'>CommissioneRate ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCommissioneRate" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["Commissionerate"].ToString() +
                            "' name='txtCommissioneRate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRangeAddress' class='bcLabel'>Range Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRangeAddress" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["VoltaExRange"].ToString() + "' name='txtRangeAddress" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDivisionAddress' class='bcLabel'>Division Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDivisionAddress" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["VoltaDivision"].ToString() + "' name='txtDivisionAddress" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCommissioneRateAddress" + i + "' class='bcLabel'>CommissioneRate Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCommissioneRateAddress" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["VoltaCommissionerate"].ToString() +
                            "' name='txtCommissioneRateAddress" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDutyDrawbackSNo' class='bcLabel'>Duty Drawback S.No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDutyDrawbackSNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["DutyDrawbackSNo"].ToString() + "' name='txtDutyDrawbackSNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDispatchmaterialDate' class='bcLabel'>Dispatch material Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDispatchmaterialDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CommonBLL.DateDisplay_1(Convert.ToDateTime(CTOneDt.Tables[0].Rows[i]["MaterialDispatchDate"].ToString()))
                            + "' name='txtDispatchmaterialDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMaterialGatepassInvoice' class='bcLabel'>Material Gatepass & Invoice ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtMaterialGatepassInvoice" + i + "' class='bcAsptextboxmulti' style='height:60px;width: 180px;' type='text' " +
                            " readonly='readonly' name='txtMaterialGatepassInvoice" + i + "'/>" + CTOneDt.Tables[0].Rows[i]["MaterialDescription"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblExTariffHeadingNos' class='bcLabel'>Ex-Tariff Heading No's ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtExTariffHeadingNos" + i + "' class='bcAsptextboxmulti' type='text' style='height:60px;width: 180px;' " +
                            " readonly='readonly' name='txtExTariffHeadingNos" + i + "'/>" + CTOneDt.Tables[0].Rows[i]["TariffHedingNo"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMrfFactoryAddress' class='bcLabel'>Mrf. Factory Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtMrfFactoryAddress" + i + "' class='bcAsptextboxmulti' type='text' style='height:60px;width: 180px;' " +
                            " readonly='readonly' name='txtMrfFactoryAddress" + i + "'/>" + CTOneDt.Tables[0].Rows[i]["FactoryAddress"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblHypothecation' class='bcLabel'>Hypothecation ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtHypothecation" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["HypothecationID"].ToString() + "' name='txtHypothecation" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMrfUnits' class='bcLabel'>Mrf. Units ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtMrfUnits" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["BranchID"].ToString() + "' name='txtMrfUnits" + i +
                            "'/></td>");
                        sb.Append(" </tr>");
                        # endregion

                        # region CT-1
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;CT-1</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCTOneDraftRefNo' class='bcLabel'>CT-1 Draft Ref. No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCTOneDraftRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["CT1DraftRefNo"].ToString() + "' name='txtCTOneDraftRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBondBalanceValue' class='bcLabel'>Bond Balance Value ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBondBalanceValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["BondBalanceValue"].ToString() + "' name='txtBondBalanceValue" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInternalRefNo" + i + "' class='bcLabel'>Internal Ref. No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInternalRefNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["InternalRefNo"].ToString() +
                            "' name='txtInternalRefNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCTOnevalue' class='bcLabel'>CT-1 value ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCTOnevalue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["CT1BondValue"].ToString() + "' name='txtCTOnevalue" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefNo' class='bcLabel'>Ref No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["CT1ReferenceNo"].ToString() + "' name='txtRefNo" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefDate" + i + "' class='bcLabel'>Ref Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefDate" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='"
                            + CommonBLL.DateDisplay_1(Convert.ToDateTime(CTOneDt.Tables[0].Rows[i]["RefDate"].ToString())) +
                            "' name='txtRefDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNoOfAREForms' class='bcLabel'>No.Of ARE Forms ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNoOfAREForms" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + CTOneDt.Tables[0].Rows[i]["NoofARE1Forms"].ToString() + "' name='txtNoOfAREForms" + i +
                            "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[2].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[2].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[2].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[2].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> CT-1 is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindDespatchInstructionDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string Attachments = "";
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet DscpIns = FeBLL.SelectEnqiryHead(CommonBLL.FlagASelect, ID, Guid.Empty, Guid.Empty);
                if (DscpIns.Tables.Count > 0 && DscpIns.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < DscpIns.Tables[0].Rows.Count; i++)
                    {
                        if (lblDINo.Text == "") 
                        {
                            lblDINo.Text = " (Date : " + Convert.ToDateTime(DscpIns.Tables[0].Rows[i]["CreatedDate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + DscpIns.Tables[0].Rows[i]["RefNo"].ToString() + ")";
                        }
                        else
                        {
                            lblDINo.Text = lblDINo.Text + " & (Date : " + Convert.ToDateTime(DscpIns.Tables[0].Rows[i]["CreatedDate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + DscpIns.Tables[0].Rows[i]["RefNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Dispatch Instructions
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Dispatch Instructions</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["CustNm"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierNm' class='bcLabel'>Supplier Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierNm" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["SupplierNm"].ToString() + "' name='txtSupplierNm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefNo" + i + "' class='bcLabel'>Reference No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["RefNo"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>Foreign PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + DscpIns.Tables[0].Rows[i]["FPONos"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPONOs' class='bcLabel'>Local PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtLPONOs" + i + "'/>" + DscpIns.Tables[0].Rows[i]["LPONos"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPONOs' class='bcLabel'>CT-1 Ref No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtCTones" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtCTones" + i + "'/>" + DscpIns.Tables[0].Rows[i]["CT1No"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAREForms' class='bcLabel'>No. of ARE-1 Form Sets ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAREForms" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["ARE1FrmSets"].ToString() + "' name='txtAREForms" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>Subject ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSubject" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["Subject"].ToString() + "' name='txtSubject" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingAddress" + i + "' class='bcLabel'>Shipping Address ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtShippingAddress" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtShippingAddress" + i + "'/>" + DscpIns.Tables[0].Rows[i]["ShippingAddress"].ToString()
                            + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblContactPerson' class='bcLabel'>Contact Person ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtContactPerson" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["ContactPersonName"].ToString() + "' name='txtContactPerson" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblContactNo' class='bcLabel'>Contact No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtContactNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["ContactNumber"].ToString() + "' name='txtContactNo" + i +
                            "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAlternateContactPerson' class='bcLabel'>Alternate Contact Person ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAlternateContactPerson" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["AlternativePersonName"].ToString() + "' name='txtAlternateContactPerson" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAlternateContactNo' class='bcLabel'>Alternate Contact No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAlternateContactNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DscpIns.Tables[0].Rows[i]["AlternativeContactNumber"].ToString() + "' name='txtAlternateContactNo" + i +
                            "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        // Attachments
                        string DispinstAttachments = "";
                        DispinstAttachments = DscpIns.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (DispinstAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Dispatch Instructions Attachments", DispinstAttachments) + "</td></tr>");

                        // Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(DscpIns.Tables[0].Rows[i]["DInsID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet ds = new DataSet();
                        ds = cbll.GetComments(CommonBLL.FlagFSelect, CommentsID, "DInsID");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        // End of Comments


                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Dispatch Instructions is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindGRNDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet DsGRN = FeBLL.SelectEnqiryHead(CommonBLL.FlagBSelect, ID, Guid.Empty, Guid.Empty);
                if (DsGRN.Tables.Count > 0 && DsGRN.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < DsGRN.Tables[0].Rows.Count; i++)
                    {
                        if (lblGRNo.Text == "")
                        {
                            lblGRNo.Text = " (Date : " + DsGRN.Tables[0].Rows[i]["ReceivedDt"].ToString() + " :: No : " + DsGRN.Tables[0].Rows[i]["RefGRN"].ToString() + ")";
                        }
                        else
                        {
                            lblGRNo.Text = lblGRNo.Text + " & (Date : " + DsGRN.Tables[0].Rows[i]["ReceivedDt"].ToString() + " :: No : " + DsGRN.Tables[0].Rows[i]["RefGRN"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region GRN Details
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;GRN Details</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["CustNm"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");

                        string isDispatch = (DsGRN.Tables[0].Rows[i]["DspchInstID"].ToString() == "0" ? "" : " checked = 'checked' ");
                        sb.Append("<td class='bcTdnormal'><span id='lblDispatchInstrctions" + i + "' class='bcLabel'>Is Dispatch Instrctions :: </span> ");
                        sb.Append("&nbsp;&nbsp; <input type='checkbox' id='chkIsDispIns' " + isDispatch + " disabled='disabled' /> </td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRefNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["DspInsNo"].ToString() + "' name='txtRefNo" + i + "'/></td>");

                        string isGDN = (DsGRN.Tables[0].Rows[i]["GdnID"].ToString() == "0" ? "" : " checked = 'checked' ");
                        sb.Append("<td class='bcTdnormal'><span id='lblRefNo" + i + "' class='bcLabel'>Is GDN ::</span>");
                        sb.Append("&nbsp;&nbsp; <input type='checkbox' id='chkIsGDN' " + isGDN + " disabled='disabled' /> </td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGDNRefNo" + i + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["GdnNo"].ToString() + "' name='txtGDNRefNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>Foreign PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + DsGRN.Tables[0].Rows[i]["FPONos"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSupplierNm' class='bcLabel'>Supplier Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSupplierNm" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["SuplrNm"].ToString() + "' name='txtSupplierNm" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLPONOs' class='bcLabel'>Local PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtLPONOs" + i + "'/>" + DsGRN.Tables[0].Rows[i]["LPONos"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofReceipt' class='bcLabel'>Place of Receipt ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofReceipt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["GodownNm"].ToString() + "' name='txtPlaceofReceipt" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTransporterName' class='bcLabel'>Transporter Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTransporterName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["TrnsptrNm"].ToString() + "' name='txtTransporterName" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblReceivedDate" + i + "' class='bcLabel'>Received Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtReceivedDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["ReceivedDt"].ToString() + "' name='txtReceivedDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblWayBillNo' class='bcLabel'>Way Bill No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtWayBillNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["WayBillNo"].ToString() + "' name='txtWayBillNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblWayBillDate' class='bcLabel'>Way Bill Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtWayBillDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["WayBillDt"].ToString() + "' name='txtWayBillDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTruckNo' class='bcLabel'>Truck No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTruckNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["TruckNo"].ToString() + "' name='txtTruckNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPackingType' class='bcLabel'>Packing Type ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPackingType" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["packingType"].ToString() + "' name='txtPackingType" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNoofPackages' class='bcLabel'>No. of Packages ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNoofPackages" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["PackagesNo"].ToString() + "' name='txtNoofPackages" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGrossWeight' class='bcLabel'>Gross Weight : Kgs ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGrossWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["GrossWeight"].ToString() + "' name='txtGrossWeight" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNetWeight' class='bcLabel'>Net Weight : Kgs ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNetWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["NetWeight"].ToString() + "' name='txtNetWeight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDCNo' class='bcLabel'>DC No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDCNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["DCNo"].ToString() + "' name='txtDCNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDCNoDate' class='bcLabel'>DC No. Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDCNoDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["DCDt"].ToString() + "' name='txtDCNoDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        string Freignt = (DsGRN.Tables[0].Rows[i]["packingType"].ToString() == "1" ? "To-Pay" : "Pre-Pay");
                        sb.Append("<td class='bcTdnormal'><span id='lblFreight' class='bcLabel'>Freight ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFreight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Freignt + "' name='txtFreight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInvoiceNo' class='bcLabel'>Invoice No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInvoiceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["InvoiceNo"].ToString() + "' name='txtInvoiceNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInvoiceDate' class='bcLabel'>Invoice Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInvoiceDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["InvoiceDt"].ToString() + "' name='txtInvoiceDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        string GoodsCondtion = "";
                        if (DsGRN.Tables[0].Rows[i]["GoodsCndtn"].ToString() == "1")
                            GoodsCondtion = "Good";
                        else if (DsGRN.Tables[0].Rows[i]["GoodsCndtn"].ToString() == "2")
                            GoodsCondtion = "Bad";
                        else if (DsGRN.Tables[0].Rows[i]["GoodsCndtn"].ToString() == "3")
                            GoodsCondtion = "Breakage";
                        sb.Append("<td class='bcTdnormal'><span id='lblConditionofGoods' class='bcLabel'>Condition of Goods ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtConditionofGoods" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + GoodsCondtion + "' name='txtConditionofGoods" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPaymentMode' class='bcLabel'>Payment Mode ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPaymentMode" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + (DsGRN.Tables[0].Rows[i]["Payment"].ToString() == "1" ? "Cash" : "Cheque")
                            + "' name='txtPaymentMode" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRemarks' class='bcLabel'>Remarks ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRemarks" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["Remarks"].ToString() + "' name='txtRemarks" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region ARE-1 Forms

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                          "&nbsp;&nbsp;&nbsp;CT-1 & ARE-1 DETAILS   </td></tr>");//Added Items                        
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1  
                        DataSet AREForms = FeBLL.SelectEnqiryHead(CommonBLL.FlagESelect, new Guid(DsGRN.Tables[0].Rows[i]["GRNID"].ToString()), Guid.Empty, Guid.Empty);
                        if (AREForms.Tables.Count > 0 && AREForms.Tables[0].Rows.Count > 0)
                            sb.Append(FillCT1Dtls(AREForms.Tables[0]));
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        # endregion

                        # region GRN Items

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                           "&nbsp;&nbsp;&nbsp;Goods Receipt Note Items</td></tr>");//Added Items                        
                        sb.Append("<tr>");
                        sb.Append("<td colspan='6'>");//1  
                        DataSet GRNItems = FeBLL.SelectEnqiryHead(CommonBLL.FlagCSelect, new Guid(DsGRN.Tables[0].Rows[i]["GRNID"].ToString()), Guid.Empty, Guid.Empty);
                        if (DsGRN.Tables.Count > 0 && DsGRN.Tables[0].Rows.Count > 0)
                        {
                            Session["Items"] = GRNItems;
                            sb.Append(FillGridView("GRN"));//Items
                        }
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        # endregion

                        // Attachments
                        string GRNtAttachments = "";
                        GRNtAttachments = DsGRN.Tables[0].Rows[i]["Attachements"].ToString().Trim();
                        if (GRNtAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Goods Receipt Note Attachments", GRNtAttachments) + "</td></tr>");

                        # region Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(DsGRN.Tables[0].Rows[i]["GRNID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet ds = new DataSet();
                        ds = cbll.GetComments(CommonBLL.FlagASelect, CommentsID, "GRNID");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Goods Receipt Note is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindCheckListDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet DsGRN = FeBLL.SelectEnqiryHead(CommonBLL.FlagFSelect, ID, Guid.Empty, Guid.Empty);
                if (DsGRN.Tables.Count > 0 && DsGRN.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < DsGRN.Tables[0].Rows.Count; i++)
                    {
                        if (lblSPDNo.Text == "")
                        {
                            lblSPDNo.Text = " (Date :" + Convert.ToDateTime(DsGRN.Tables[0].Rows[i]["CreatedDate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + DsGRN.Tables[0].Rows[i]["ChkLstRefNo"].ToString() + ")";
                        }
                        else
                        {
                            lblSPDNo.Text = lblSPDNo.Text + " & (Date :" + Convert.ToDateTime(DsGRN.Tables[0].Rows[i]["CreatedDate"]).ToString("dd/MM/yyyy").Replace('-', '/') + " :: No : " + DsGRN.Tables[0].Rows[i]["ChkLstRefNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region CheckList Header Details

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                          "&nbsp;&nbsp;&nbsp;Header Details   </td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNotify' class='bcLabel'>Notify ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNotify" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["NotifyNm"].ToString() + "' name='txtNotify" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCountryofOrginofGoods' class='bcLabel'>Country of Orgin of Goods ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCountryofOrginofGoods" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["CountryOrgGoods"].ToString() + "' name='txtCountryofOrginofGoods" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCountryofFinalDestination' class='bcLabel'>Country of Final Destination ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCountryofFinalDestination" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["CountryFinalDest"].ToString() + "' name='TXTCountryofFinalDestination" + i +
                            "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfLoading' class='bcLabel'>Port Of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["PortLoading"].ToString() + "' name='txtPortOfLoading" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfDischarge' class='bcLabel'>Port Of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["PortDischarge"].ToString() + "' name='txtPortOfDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofDelivery' class='bcLabel'>Place of Delivery ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofDelivery" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["portDelivery"].ToString() + "' name='txtPlaceofDelivery" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPreCarriageby' class='bcLabel'>Pre-Carriage by ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPreCarriageby" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["PreCrirBy"].ToString() + "' name='txtPreCarriageby" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofreceiptbypreCarrier' class='bcLabel'>Place of receipt by pre-Carrier ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofreceiptbypreCarrier" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["PlcRcptPCrirBy"].ToString() + "' name='txtPlaceofreceiptbypreCarrier" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVesselFlightNo' class='bcLabel'>Vessel / Flight No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVesselFlightNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["VslFltNo"].ToString() + "' name='txtVesselFlightNo" + i +
                            "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTermsOfDeliveryandPayment' class='bcLabel'>Terms Of Delivery and Payment ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtTermsOfDeliveryandPayment" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtTermsOfDeliveryandPayment" + i + "'/>" + DsGRN.Tables[0].Rows[i]["TrmsDlryPymnts"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblIncoterm' class='bcLabel'>Incoterm ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtIncoterm" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["IncTrm"].ToString() + "' name='txtIncoterm" + i + "'/> <br/><br/>"
                            + "<input id='txtIncoterm1" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["IncTrmLctn"].ToString() + "' name='txtIncoterm1" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblOtherReferences' class='bcLabel'>Other References ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtOtherReferences" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtOtherReferences" + i + "'/>" + DsGRN.Tables[0].Rows[i]["OtherRef"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region CheckList/Shipment Planning Details
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Shipment Planning Details</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomerName' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomerName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["CustNm"].ToString() + "' name='txtCustomerName" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGDN' class='bcLabel'>Goods Dispatch Note(GDN) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGDN" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["GDNNos"].ToString() + "' name='txtGDN" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGRN' class='bcLabel'>Goods Receipt Note(GDN) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGRN" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["GRNNos"].ToString() + "' name='txtGRN" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>Foreign PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + "FPO Nos" + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShipmentMode' class='bcLabel'>Shipment Mode ::</span> <br/><br/>"
                            + " <span id='lblCheckListReferenceNo' class='bcLabel'>Shpmnt Plng Reference No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtShipmentMode" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["ShipmentMode"].ToString() + "' name='txtShipmentMode" + i + "'/> <br/><br/>"
                            + "<input id='txtCheckListReferenceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + DsGRN.Tables[0].Rows[i]["ChkLstRefNo"].ToString() + "' name='txtCheckListReferenceNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblImportantInstructions' class='bcLabel'>Important Instructions ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtImportantInstructions" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtLPONOs" + i + "'/>" + DsGRN.Tables[0].Rows[i]["ImpInstructions"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region ListDetails

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;GRN/GDN Details</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdNewTable' colspan='6'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='font-size: small; background-color: #F5F4F4; border: solid 1px #ccc' width='100%' cellspacing='0' cellpadding='0' border='0' id='tblGRN_GDNItems'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                        sb.Append("<td>SNo</td><td>Pkg.Nos</td>" +
                        "<td>name of the Supplier & Place</td><td>No of Pkgs</td><td>FPO No.</td><td style='width:20%'>Pkgs. to be collected from Transport Godown LR No. / VIPL Godown Receipt No.</td>"
                        + "<td style='width:20%'>Covered under ARE-1 if TRUE, details & availability ARE-1</td><td>Net Weight Kgs</td><td>Gr Weight Kgs</td><td>Remarks</td>");
                        sb.Append("</tr></thead><tbody class='bcGridViewMain'>");
                        decimal PackagesTotal = 0;
                        decimal NetWeight = 0;
                        decimal GrWeight = 0;
                        Guid CheckListID = new Guid(DsGRN.Tables[0].Rows[i]["ChkListID"].ToString());
                        DataSet dss = FeBLL.SelectEnqiryHead(CommonBLL.FlagGSelect, CheckListID, Guid.Empty, Guid.Empty);
                        DataTable dtt = GetCheckListItemsEdit(dss);
                        if (dtt.Rows.Count > 0)
                        {
                            for (int c = 0; c < dtt.Rows.Count; c++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + (c + 1) + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["PkgNos"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["SupplierNm"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["NoOfPkgs"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["FPONOs"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["LR_GodownNo"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["IsARE1"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["NetWeight"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["GrWeight"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[c]["Remarks"].ToString() + "</td>");
                                PackagesTotal += Convert.ToDecimal(dtt.Rows[c]["NoOfPkgs"].ToString());
                                NetWeight += Convert.ToDecimal(dtt.Rows[c]["NetWeight"].ToString());
                                GrWeight += Convert.ToDecimal(dtt.Rows[c]["GrWeight"].ToString());
                                sb.Append("</tr>");
                            }
                        }

                        sb.Append("<tr class='bcGridViewHeaderStyle'>");
                        sb.Append("<td colspan='4' align='right'><b><span>Total Packages : " + PackagesTotal + "</span></b></td>");
                        sb.Append("<td colspan='3' align='right'><b><span>Total Weight : </span></b></td>");
                        sb.Append("<td><b><span>" + NetWeight + "</span></b></td>");
                        sb.Append("<td><b><span>" + GrWeight + "</span></b></td>");
                        sb.Append("<td><b><span></span></b></td>");
                        sb.Append("</tr>");
                        sb.Append("</tbody></table>");
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        # endregion

                        # region Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(DsGRN.Tables[0].Rows[i]["ChkListID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet ds = new DataSet();
                        ds = cbll.GetComments(CommonBLL.FlagASelect, CommentsID, "ChkListID");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Shipment Planning is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindShipmentProformaINvoiceDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet ShpPrfInv = FeBLL.SelectEnqiryHead(CommonBLL.FlagHSelect, ID, Guid.Empty, Guid.Empty);
                if (ShpPrfInv.Tables.Count > 0 && ShpPrfInv.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ShpPrfInv.Tables[0].Rows.Count; i++)
                    {
                        if (lblSPINo.Text == "")
                        {
                            lblSPINo.Text = " (Date :" + ShpPrfInv.Tables[0].Rows[i]["PrfmaInvcDt"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["PrfmInvcNo"].ToString() + ")";
                        }
                        else
                        {
                            lblSPINo.Text = lblSPINo.Text + " & (Date :" + ShpPrfInv.Tables[0].Rows[i]["PrfmaInvcDt"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["PrfmInvcNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Proforma Invoice Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCheckList' class='bcLabel'>Shipment Planning No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCheckList" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["ChkListNo"].ToString() + "' name='txtCheckList" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["CustomerName"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>Foreign PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + ShpPrfInv.Tables[0].Rows[i]["FPONmbrs"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNo' class='bcLabel'>Proforma Invoice No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrfmInvcNo"].ToString() + "' name='txtProformaInvoiceNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceDate' class='bcLabel'>Proforma Invoice Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrfmaInvcDt"].ToString() + "' name='txtProformaInvoiceDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblOtherReferences" + i + "' class='bcLabel'>Other References ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtOtherReferences" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["OtherReferences"].ToString() + "' name='txtOtherReferences" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNotify' class='bcLabel'>Notify ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNotify" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["Notify"].ToString() + "' name='txtNotify" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofOrginofGoods' class='bcLabel'>Place of Orgin of Goods ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofOrginofGoods" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcOrgnGds"].ToString() + "' name='txtPlaceofOrginofGoods" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofFinalDestination ' class='bcLabel'>Place of Final Destination  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofFinalDestination " + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcFnlDstn"].ToString() + "' name='txtPlaceofFinalDestination " + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfLoading' class='bcLabel'>Port Of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrtLdng"].ToString() + "' name='txtPortOfLoading" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfDischarge' class='bcLabel'>Port Of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrtDschrg"].ToString() + "' name='txtPortOfDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofDelivery' class='bcLabel'>Place of Delivery ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofDelivery" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcDlvry"].ToString() + "' name='txtPlaceofDelivery" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPreCarriageby' class='bcLabel'>Pre-Carriage by ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPreCarriageby" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PreCrgBy"].ToString() + "' name='txtPreCarriageby" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofreceiptbypreCarrier' class='bcLabel'>Place of receipt by pre-Carrier ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofreceiptbypreCarrier" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcRcpntPreCrgBy"].ToString() + "' name='txtPlaceofreceiptbypreCarrier" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVesselFlightNo' class='bcLabel'>Vessel / Flight No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVesselFlightNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["VslFltNo"].ToString() + "' name='txtVesselFlightNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTermsOfDeliveryandPayment' class='bcLabel'>Terms Of Delivery and Payment ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTermsOfDeliveryandPayment" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["TrmsDlvryPmnt"].ToString() + "' name='txtTermsOfDeliveryandPayment" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region Item Details

                        Itemtables += 1;

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Item Details</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td width='100%' colspan='6'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table id='tblPrfINVItems" + Itemtables + "' width='100%'><thead><tr>");
                        sb.Append("<th>FPO Nos</th><th>Item Description</th><th>HS-Code</th><th>Part No</th><th>Make</th><th>Quantity</th><th>Units</th><td>Rate($)</th><th>Amount($)</th>");
                        sb.Append("</tr></thead><tbody>");

                        Guid PrfINVID = new Guid(ShpPrfInv.Tables[0].Rows[i]["PrfINVID"].ToString());
                        DataSet dss = FeBLL.SelectEnqiryHead(CommonBLL.FlagISelect, PrfINVID, Guid.Empty, Guid.Empty);
                        DataTable dtt = dss.Tables[0].Copy();
                        if (dtt.Rows.Count > 0)
                        {
                            for (int D = 0; D < dtt.Rows.Count; D++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + dtt.Rows[D]["FPONmbr"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Description"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["HSCode"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["PartNumber"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Make"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["DspchQty"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["UnitNm"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Rate"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Amount"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        else
                        {
                            sb.Append("<tr>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td colspan='9'><center><b>No Rows to dispaly.</b></center></td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("</tbody>");
                        sb.Append("</table>");
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        # endregion

                        // Attachments
                        string ShpmentAttachments = "";
                        ShpmentAttachments = ShpPrfInv.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (ShpmentAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Goods Receipt Note Attachments", ShpmentAttachments) + "</td></tr>");

                        # region Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(ShpPrfInv.Tables[0].Rows[i]["PrfINVID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet ds = new DataSet();
                        ds = cbll.GetComments(CommonBLL.FlagASelect, CommentsID, "PrfmaInvcID");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Shipping Proforma Invoice is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindPackingListDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet ShpPrfInv = FeBLL.SelectEnqiryHead(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty);
                if (ShpPrfInv.Tables.Count > 0 && ShpPrfInv.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ShpPrfInv.Tables[0].Rows.Count; i++)
                    {
                        if (lblPLNo.Text == "")
                        {
                            lblPLNo.Text = " (Date :" + ShpPrfInv.Tables[0].Rows[i]["PkingLstDT"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["PkngListNo"].ToString() + ")";
                        }
                        else
                        {
                            lblPLNo.Text = lblPLNo.Text + " & (Date :" + ShpPrfInv.Tables[0].Rows[i]["PkingLstDT"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["PkngListNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Proforma Invoice Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCheckList' class='bcLabel'>Shipment Planning No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCheckList" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["ChkListNo"].ToString() + "' name='txtCheckList" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["CustomerName"].ToString() + "' name='txtCustomer" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFPONOs' class='bcLabel'>Foreign PO(s) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtFPONOs" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtFPONOs" + i + "'/>" + ShpPrfInv.Tables[0].Rows[i]["FPONmbrs"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPackingListNo' class='bcLabel'>Packing List No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPackingListNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PkngListNo"].ToString() + "' name='txtPackingListNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPkingListDT' class='bcLabel'>Packing List Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPkingListDT" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PkingLstDT"].ToString() + "' name='txtPkingListDT" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblOtherReferences" + i + "' class='bcLabel'>Other References ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtOtherReferences" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["OtherReferences"].ToString() + "' name='txtOtherReferences" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNotify' class='bcLabel'>Notify ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNotify" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["Notify"].ToString() + "' name='txtNotify" + i +
                            "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofOrginofGoods' class='bcLabel'>Place of Orgin of Goods ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofOrginofGoods" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcOrgnGds"].ToString() + "' name='txtPlaceofOrginofGoods" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofFinalDestination ' class='bcLabel'>Place of Final Destination  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofFinalDestination " + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcFnlDstn"].ToString() + "' name='txtPlaceofFinalDestination " + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfLoading' class='bcLabel'>Port Of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrtLdng"].ToString() + "' name='txtPortOfLoading" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortOfDischarge' class='bcLabel'>Port Of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortOfDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrtDschrg"].ToString() + "' name='txtPortOfDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofDelivery' class='bcLabel'>Place of Delivery ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofDelivery" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcDlvry"].ToString() + "' name='txtPlaceofDelivery" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPreCarriageby' class='bcLabel'>Pre-Carriage by ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPreCarriageby" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PreCrgBy"].ToString() + "' name='txtPreCarriageby" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofreceiptbypreCarrier' class='bcLabel'>Place of receipt by pre-Carrier ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofreceiptbypreCarrier" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcRcpntPreCrgBy"].ToString() + "' name='txtPlaceofreceiptbypreCarrier" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVesselFlightNo' class='bcLabel'>Vessel / Flight No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVesselFlightNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["VslFltNo"].ToString() + "' name='txtVesselFlightNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTermsOfDeliveryandPayment' class='bcLabel'>Terms Of Delivery and Payment ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTermsOfDeliveryandPayment" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["TrmsDlvryPmnt"].ToString() + "' name='txtTermsOfDeliveryandPayment" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region Item Details

                        Itemtables += 1;

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Item Details</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td width='100%' colspan='6'>");
                        sb.Append("<table id='tblPrfINVItems" + Itemtables + "' width='100%'><thead><tr>");
                        sb.Append("<th>FPO Nos</th><th>Item Description</th><th>HS-Code</th><th>Part No</th><th>Make</th><th>Quantity</th><th>Units</th><td>Net Weight (kgs)</th><th>Gross Weight (kgs)</th>");
                        sb.Append("</tr></thead><tbody>");

                        Guid PknfLstID = new Guid(ShpPrfInv.Tables[0].Rows[i]["PkingLstID"].ToString());
                        DataSet dss = FeBLL.SelectEnqiryHead(CommonBLL.FlagNewInsert, PknfLstID, Guid.Empty, Guid.Empty);
                        DataTable dtt = dss.Tables[0].Copy();
                        if (dtt.Rows.Count > 0)
                        {
                            for (int D = 0; D < dtt.Rows.Count; D++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td>" + dtt.Rows[D]["FPONmbr"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Description"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["HSCode"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["PartNumber"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["Make"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["DspchQty"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["UnitNm"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["NetWeight"].ToString() + "</td>");
                                sb.Append("<td>" + dtt.Rows[D]["GrWeight"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        else
                        {
                            sb.Append("<tr>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td style='display:none'></td>");
                            sb.Append("<td colspan='9'><center><b>No Rows to dispaly.</b></center></td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("</tbody>");
                        sb.Append("</table>");
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        # endregion

                        // Attachments
                        string ShpmentAttachments = "";
                        ShpmentAttachments = ShpPrfInv.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (ShpmentAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Goods Receipt Note Attachments", ShpmentAttachments) + "</td></tr>");

                        # region Comments
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                        sb.Append("<tr>");
                        sb.Append("<td colspan='999' style='font-size: small;'>");
                        Guid CommentsID = new Guid(ShpPrfInv.Tables[0].Rows[i]["PkingLstID"].ToString());
                        CommentsBLL cbll = new CommentsBLL();
                        DataSet ds = new DataSet();
                        ds = cbll.GetComments(CommonBLL.FlagASelect, CommentsID, "PkngList");
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<table width='100%'>");
                            for (int a = 0; a < ds.Tables[0].Rows.Count; a++)
                            {
                                sb.Append("<tr>");
                                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                                    + ds.Tables[0].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                                    + ds.Tables[0].Rows[a]["comments"].ToString() + "</div></div></td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</table>");
                        }
                        else
                            sb.Append("No Comments");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        # endregion


                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Packing List is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindShippingBillDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet ShpBll = FeBLL.SelectEnqiryHead(CommonBLL.FlagWCommonMstr, ID, Guid.Empty, Guid.Empty);
                if (ShpBll.Tables.Count > 0 && ShpBll.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ShpBll.Tables[0].Rows.Count; i++)
                    {
                        if (lblSBDNo.Text == "")
                        {
                            lblSBDNo.Text = " (Date :" + ShpBll.Tables[0].Rows[i]["ShpngBilDate"].ToString() + " :: No : " + ShpBll.Tables[0].Rows[i]["ShpngBilNmbr"].ToString() + ")";
                        }
                        else
                        {
                            lblSBDNo.Text = lblSBDNo.Text + " & (Date :" + ShpBll.Tables[0].Rows[i]["ShpngBilDate"].ToString() + " :: No : " + ShpBll.Tables[0].Rows[i]["ShpngBilNmbr"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Shipping Bill Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNo' class='bcLabel'>Proforma Invoice No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ShpngPrfInvcNo"].ToString() + "' name='txtProformaInvoiceNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLEONo' class='bcLabel'>LEO No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLEONo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LEONmbr"].ToString() + "' name='txtLEONo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLEODate' class='bcLabel'>LEO Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtLEODate" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtLEODate" + i + "'/>" + ShpBll.Tables[0].Rows[i]["LEODate"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillNo' class='bcLabel'>Shipping Bill No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtShippingBillNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ShpngBilNmbr"].ToString() + "' name='txtShippingBillNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillDate' class='bcLabel'>Shipping Bill Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtShippingBillDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ShpngBilDate"].ToString() + "' name='txtShippingBillDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCHA" + i + "' class='bcLabel'>CHA ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCHA" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ChaNm"].ToString() + "' name='txtCHA" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblStateofOrigin' class='bcLabel'>State of Origin ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtStateofOrigin" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["StateOfOrigine"].ToString() + "' name='txtStateofOrigin" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # region  Facotry Sealed
                        sb.Append("<tr style='background-color: #BEBEBE; color: Blue;'><td colspan='6'><center><b> Facotry Sealed </b></center></td></tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                          "&nbsp;&nbsp;&nbsp;Address of Stuffing :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLoFSPNo' class='bcLabel'>LoFSP No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLoFSPNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LoFSPNmbr"].ToString() + "' name='txtLoFSPNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLoFSPDate' class='bcLabel'>LoFSP Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLoFSPDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LoFSPDate"].ToString() + "' name='txtLoFSPDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDateofStuffing' class='bcLabel'>Date of Stuffing ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDateofStuffing" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["DateofStuffing"].ToString() + "' name='txtDateofStuffing" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFileNo' class='bcLabel'>File No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFileNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["FileNmbr"].ToString() + "' name='txtFileNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFileDate' class='bcLabel'>File Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFileDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["FileDate"].ToString() + "' name='txtFileDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNoofContainers' class='bcLabel'>No. of Containers ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNoofContainers" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["NmbrofCntrs"].ToString() + "' name='txtNoofContainers" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTypeofContainers' class='bcLabel'>Type of Containers ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTypeofContainers" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["TypeofContainer"].ToString() + "' name='txtTypeofContainers" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRange' class='bcLabel'>Range ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRange" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["Range"].ToString() + "' name='txtRange" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDivision' class='bcLabel'>Division ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDivision" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["Division"].ToString() + "' name='txtDivision" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                          "&nbsp;&nbsp;&nbsp;Container Details :</td></tr>");

                        DataSet Blds1 = FeBLL.SelectEnqiryHead(CommonBLL.FlagYSelect, new Guid(ShpBll.Tables[0].Rows[i]["ID"].ToString()), Guid.Empty, Guid.Empty);
                        if (Blds1.Tables.Count > 0 && Blds1.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td colspan='6'>" + FillContainerDetails(Blds1.Tables[0], "SBL") + "</td>");
                            sb.Append("</tr>");
                        }


                        # endregion

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortingofLoading' class='bcLabel'>Porting of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortingofLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["PrtLoading"].ToString() + "' name='txtPortingofLoading" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofDischarge' class='bcLabel'>Port of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["PrtDischarge"].ToString() + "' name='txtPortofDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCntryofDestination' class='bcLabel'>Cntry of Destination ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCntryofDestination" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["CntryDestination"].ToString() + "' name='txtCntryofDestination" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCountryofOrigin' class='bcLabel'>Country of Origin ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCountryofOrigin" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["CntryOrigine"].ToString() + "' name='txtCountryofOrigin" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalPackages' class='bcLabel'>Total Packages ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalPackages" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["TotPkgs"].ToString() + "' name='txtTotalPackages" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLoosePackets' class='bcLabel'>Loose Packets ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLoosePackets" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LoosePkts"].ToString() + "' name='txtLoosePackets" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGrWeight' class='bcLabel'>Gross Weight(Kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGrWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["GrossWeight"].ToString() + "' name='txtGrWeight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNetWeight' class='bcLabel'>Net Weight(Kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNetWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["NetWeight"].ToString() + "' name='txtNetWeight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFOBValue' class='bcLabel'>FOB Value (INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFOBValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["FOBValueINR"].ToString() + "' name='txtFOBValue" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRotationNo' class='bcLabel'>Rotation No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRotationNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["RotationNmbr"].ToString() + "' name='txtRotationNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRotationDate' class='bcLabel'>Rotation Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRotationDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["RotationDate"].ToString() + "' name='txtRotationDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNatureofCargo' class='bcLabel'>Nature of Cargo ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNatureofCargo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["NatureOfCargo"].ToString() + "' name='txtNatureofCargo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNoofContainers' class='bcLabel'>No. of Containers ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNoofContainers" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["NmbrOfContainers"].ToString() + "' name='txtNoofContainers" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRBIWaiverNo' class='bcLabel'>RBI Waiver No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRBIWaiverNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["RBIWaiverNmbr"].ToString() + "' name='txtRBIWaiverNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["RBIWaiverDate"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # region DBK
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                                                  "&nbsp;&nbsp;&nbsp;DBK :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalDrawBack' class='bcLabel'>Total Draw Back (INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalDrawBack" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["TotalDrawBackINR"].ToString() + "' name='txtTotalDrawBack" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblServiceTaxRefund' class='bcLabel'>Service Tax Refund (INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtServiceTaxRefund" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ServiceTaxRefundINR"].ToString() + "' name='txtServiceTaxRefund" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDBKScrollNo' class='bcLabel'>DBK Scroll No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDBKScrollNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["DBKScrollNmbr"].ToString() + "' name='txtDBKScrollNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["DBKDate"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEPCopyReceiptStauts' class='bcLabel'>EP Copy Receipt Stauts ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtEPCopyReceiptStauts" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["EPCopyReceiptStauts"].ToString() + "' name='txtEPCopyReceiptStauts" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLEODate' class='bcLabel'>LEO Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLEODate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["DBKLEODate"].ToString() + "' name='txtLEODate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblExamMarkID' class='bcLabel'>Exam Mark ID ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtExamMarkID" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ExamMarkID"].ToString() + "' name='txtExamMarkID" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMarkDate' class='bcLabel'>Mark Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtMarkDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ExamDate"].ToString() + "' name='txtMarkDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBankNo' class='bcLabel'>Bank A/c No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBankNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["BankACNmbr"].ToString() + "' name='txtBankNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAmountRemittedDate' class='bcLabel'>Amount Remitted Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAmountRemittedDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["AmntRemittedDate"].ToString() + "' name='txtAmountRemittedDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAmountRemittedRemarks' class='bcLabel'>Amount Remitted Remarks ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAmountRemittedRemarks" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["AmntRemittedRemarks"].ToString() + "' name='txtAmountRemittedRemarks" + i + "'/></td>");
                        sb.Append(" </tr>");
                        # endregion

                        # region Invoice Details
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                                                  "&nbsp;&nbsp;&nbsp;Invoice Details :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInvoiceValue' class='bcLabel'>Invoice Value (INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInvoiceValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["InvoiceValueINR"].ToString() + "' name='txtInvoiceValue" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInvoiceValueDollar' class='bcLabel'>Invoice Value ($) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInvoiceValueDollar" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["InvoiceValueUSD"].ToString() + "' name='txtInvoiceValueDollar" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFOBValue' class='bcLabel'>FOB Value (INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFOBValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["InvcFOBValueINR"].ToString() + "' name='txtFOBValue" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNo' class='bcLabel'>Proforma Invoice No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["PrfmaInvoiceNmbr"].ToString() + "' name='txtProformaInvoiceNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPInvoiceDate' class='bcLabel'>P. Invoice Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPInvoiceDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["PrfmaInvoiceDate"].ToString() + "' name='txtPInvoiceDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblNoofCon' class='bcLabel'>Nat. of Con ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtNoofCon" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["NoOfContainers"].ToString() + "' name='txtNoofCon" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFCurr' class='bcLabel'>F.Curr (Inv) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFCurr" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["FCurrINV"].ToString() + "' name='txtFCurr" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblExchangeRate' class='bcLabel'>Exchange Rate ::</span></td>");
                        sb.Append("<td class='bcTdnormal'>1.00 USD = <input id='txtExchangeRate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ExchangeRate"].ToString() + "' name='txtExchangeRate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblINR' class='bcLabel'>(INR) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtINR" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ICurrINR"].ToString() + "' name='txtINR" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td colspan='6' width='100%'>");
                        sb.Append("<table class='rounded-corner' style='width: 100%;'><thead>");
                        sb.Append("<td style='height: 25px' class='rounded-First'>&nbsp;</td><td style='height: 25px'><span class='bcLabel'>Rate</span></td>");
                        sb.Append("<td style='height: 25px'><span class='bcLabel'>Currency</span></td><td style='height: 25px' class='rounded-Last'>"
                            + "<span class='bcLabel'>Amount</span></td></thead>");
                        sb.Append("<tbody><tr><td><span class='bcLabelright'>Insurance</span></td>");
                        sb.Append("<td><input type='text' ID='txtInsrncRt' value='" + ShpBll.Tables[0].Rows[i]["INSRNS_Rate"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtInsrncCurrency' value='" + ShpBll.Tables[0].Rows[i]["INSRNS_Currency"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtInsrncAmnt' value='" + ShpBll.Tables[0].Rows[i]["INSRNS_Amount"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Freight</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt'value='" + ShpBll.Tables[0].Rows[i]["FRT_Rate"].ToString() + "'  CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt'value='" + ShpBll.Tables[0].Rows[i]["FRT_Currency"].ToString() + "'  CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt'value='" + ShpBll.Tables[0].Rows[i]["FRT_Amount"].ToString() + "'  CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Discount</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["DSCNT_Rate"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["DSCNT_Currency"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["DSCNT_Amount"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Commission</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["CMSN_Rate"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["CMSN_Currency"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["CMSN_Amount"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Other Deductions</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["OTRDTSN_Rate"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["OTRDTSN_Currency"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["OTRDTSN_Amount"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Packing Charges</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["PKNGCHRGS_Rate"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["PKNGCHRGS_Currency"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["PKNGCHRGS_Amount"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>Nature of Payment</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["NatureofPmnt"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td><span class='bcLabel'>Period of Payment</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["PeriodofPmnt"].ToString() + "' CssClass='bcAsptextbox' /></td></tr>");

                        sb.Append("<tr><td><span class='bcLabelright'>FTP Mentioned or Not?</span></td>");
                        sb.Append("<td><input type='text' ID='txtFrtRt' value='" + ShpBll.Tables[0].Rows[i]["FTPMentioned"].ToString() + "' CssClass='bcAsptextbox' /></td>");
                        sb.Append("<td>&nbsp;</td><td>&nbsp;</td>");

                        sb.Append("</tr></tbody>");
                        sb.Append("<tfoot><td class='rounded-foot-left'></td><td colspan='2'></td><td class='rounded-foot-right'></td></tfoot></table>");

                        sb.Append(" </td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region Documents Attached
                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                                                  "&nbsp;&nbsp;&nbsp;Documents Attached :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblInvoices' class='bcLabel'>Invoices ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtInvoices" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["Invoice"].ToString() + "' name='txtInvoices" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPackingList' class='bcLabel'>Packing List ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPackingList" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["PackingList"].ToString() + "' name='txtPackingList" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSDFDeclaration' class='bcLabel'>SDF Declaration ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSDFDeclaration" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["SDFDeclaration"].ToString() + "' name='txtSDFDeclaration" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAppendix' class='bcLabel'>Appendix III with 4A Declaration ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAppendix" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["Appendix4ADeclartion"].ToString() + "' name='txtAppendix" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLETExportDate' class='bcLabel'>LET Export Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLETExportDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LETExportDate"].ToString() + "' name='txtLETExportDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblOfficerofCustom' class='bcLabel'>Officer of Custom ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtOfficerofCustom" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["CustomsOfficer"].ToString() + "' name='txtOfficerofCustom" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDateofShipment' class='bcLabel'>Date of Shipment ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDateofShipment" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["DateofShipment"].ToString() + "' name='txtDateofShipment" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVesselName' class='bcLabel'>Vessel Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVesselName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["VessalName"].ToString() + "' name='txtVesselName" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblOVoyageNo' class='bcLabel'>OVoyage No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtOVoyageNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["OVoyageNmbr"].ToString() + "' name='txtOVoyageNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblComments' class='bcLabel'>Comments ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtComments" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["Comments"].ToString() + "' name='txtComments" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        # region DEPB Details

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                                                  "&nbsp;&nbsp;&nbsp;DEPB Details :</td></tr>");
                        sb.Append("<tr><td colspan='6' width='100%'>");

                        sb.Append("<table width='100%'>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcLabelright' colspan='2'><span id='lblExporterDEPBItems' class='bcLabel'>Total FOB Value Declared by Exporter for DEPB Items ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtExporterDEPBItems" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ExpDEPBItems"].ToString() + "' name='txtExporterDEPBItems" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span class='bcLabel'>    USD</span></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcLabelright' colspan='2'><span id='lblExporterNonDEPBItems' class='bcLabel'>Total FOB Value Declared by Exporter for Non-DEPB Items ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtExporterNonDEPBItems" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["ExpNonDEPBItems"].ToString() + "' name='txtExporterNonDEPBItems" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span class='bcLabel'>    USD</span></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcLabelright' colspan='2'><span id='lblCstmrAcptTFobValDEPBItms' class='bcLabel'>Customs Accepted Total FOB Value for DEPB Items ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCstmrAcptTFobValDEPBItms" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["CstmrAcptedDEPBItems"].ToString() + "' name='txtCstmrAcptTFobValDEPBItms" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span class='bcLabel'>    USD</span></td>");
                        sb.Append(" </tr>");
                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDEPBLic' class='bcLabel'>DEPB Lic ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDEPBLic" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LICNmbr"].ToString() + "' name='txtDEPBLic" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDEPBLicDate' class='bcLabel'>DEPB Lic Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDEPBLicDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpBll.Tables[0].Rows[i]["LICDate"].ToString() + "' name='txtDEPBLicDate" + i + "'/></td>");
                        sb.Append(" </tr>");
                        sb.Append("</table>");

                        # endregion

                        // Attachments
                        string ShpmentAttachments = "";
                        ShpmentAttachments = ShpBll.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (ShpmentAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Shipping Bill Attachments", ShpmentAttachments) + "</td></tr>");

                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Shipping Bill Details are not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindAirWayBillDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet ShpPrfInv = FeBLL.SelectEnqiryHead(CommonBLL.FlagPSelectAll, ID, Guid.Empty, Guid.Empty);
                if (ShpPrfInv.Tables.Count > 0 && ShpPrfInv.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ShpPrfInv.Tables[0].Rows.Count; i++)
                    {
                        if (lblABNo.Text == "")
                        {
                            lblABNo.Text = " (Date :" + ShpPrfInv.Tables[0].Rows[i]["ExecutableDT"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["AWBNumber"].ToString() + ")";
                        }
                        else
                        {
                            lblABNo.Text = lblABNo.Text + " & (Date :" + ShpPrfInv.Tables[0].Rows[i]["ExecutableDT"].ToString() + " :: No : " + ShpPrfInv.Tables[0].Rows[i]["AWBNumber"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>");
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Air Way Bill Details

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                          "&nbsp;&nbsp;&nbsp;Flow to make a Bill of Lading/AWB :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["CustomerIDs"].ToString() + "' name='txtCustomer" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNumber' class='bcLabel'>Proforma Invoice Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNumber" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PrfINVIDs"].ToString() + "' name='txtProformaInvoiceNumber" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillNumber' class='bcLabel'>Shipping Bill Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtShippingBillNumber" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtShippingBillNumber" + i + "'/>" + ShpPrfInv.Tables[0].Rows[i]["ShippingBillIds"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                          "&nbsp;&nbsp;&nbsp;Shipping Line :</td></tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblAWBNumber' class='bcLabel'>AWB Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtAWBNumber" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["AWBNumber"].ToString() + "' name='txtAWBNumber" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblExecutableDate' class='bcLabel'>Executable Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtExecutableDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["ExecutableDT"].ToString() + "' name='txtExecutableDate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofDischarge" + i + "' class='bcLabel'>Port of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PODischarge"].ToString() + "' name='txtPortofDischarge" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofReceipt' class='bcLabel'>Place of Receipt ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofReceipt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcOfRcpt"].ToString() + "' name='txtPlaceofReceipt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofDelivery' class='bcLabel'>Place of Delivery ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofDelivery" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["PlcOfDlvry"].ToString() + "' name='txtPlaceofDelivery" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFreight' class='bcLabel'>Freight  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFreight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["Freight"].ToString() + "' name='txtFreight" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTareWeight' class='bcLabel'>Tare Weight(Kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTareWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["tareWeight"].ToString() + "' name='txtTareWeight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGrossWeight' class='bcLabel'>Gross Weight(Kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGrossWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["GWeight"].ToString() + "' name='txtGrossWeight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalNoofPkgs' class='bcLabel'>Total No. of Pkgs ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalNoofPkgs" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["TotalPkgs"].ToString() + "' name='txtTotalNoofPkgs" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDimensionsinCms' class='bcLabel'>Dimensions in Cms ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDimensionsinCms" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["Dimenctions"].ToString() + "' name='txtDimensionsinCms" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalPrepaid' class='bcLabel'>Total Pre-paid(if Pre-paid) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalPrepaid" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + ShpPrfInv.Tables[0].Rows[i]["TotalPrePaid"].ToString() + "' name='txtTotalPrepaid" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> AirWayBill is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindBillOfladingDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet Blds = FeBLL.SelectEnqiryHead(CommonBLL.FlagQSelect, ID, Guid.Empty, Guid.Empty);
                if (Blds.Tables.Count > 0 && Blds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Blds.Tables[0].Rows.Count; i++)
                    {
                        if (lblBLNo.Text == "")
                        {
                            lblBLNo.Text = " (Date :" + Blds.Tables[0].Rows[i]["SOBDT"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["BillofLadingNo"].ToString() + ")";
                        }
                        else
                        {
                            lblBLNo.Text = lblBLNo.Text + " & (Date :" + Blds.Tables[0].Rows[i]["SOBDT"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["BillofLadingNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>");
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Air Way Bill Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCustomer' class='bcLabel'>Customer Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCustomer" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["CustomerIDs"].ToString() + "' name='txtCustomer" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblProformaInvoiceNumber' class='bcLabel'>Proforma Invoice Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtProformaInvoiceNumber" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PrfINVIDs"].ToString() + "' name='txtProformaInvoiceNumber" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillNumber' class='bcLabel'>Shipping Bill Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtShippingBillNumber" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtShippingBillNumber" + i + "'/>" + Blds.Tables[0].Rows[i]["ShippingBillIds"].ToString() + "</textarea></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingLine' class='bcLabel'>Shipping Line ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtShippingLine" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["ShippingLine"].ToString() + "' name='txtShippingLine" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBookingNo' class='bcLabel'>Booking No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBookingNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["BookingNo"].ToString() + "' name='txtBookingNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBillofLadingNo" + i + "' class='bcLabel'>Bill of Lading No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBillofLadingNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["BillofLadingNo"].ToString() + "' name='txtBillofLadingNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblSOBdate' class='bcLabel'>SOB date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtSOBdate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["SOBDT"].ToString() + "' name='txtSOBdate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVessel' class='bcLabel'>Vessel ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVessel" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Vessel"].ToString() + "' name='txtVessel" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVoyage' class='bcLabel'>Voyage  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVoyage" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Voyage"].ToString() + "' name='txtVoyage" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofLoading' class='bcLabel'>Port of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PortOfLoading"].ToString() + "' name='txtPortofLoading" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofDischarge' class='bcLabel'>Port of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PortOfDischarge"].ToString() + "' name='txtPortofDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofReceipt' class='bcLabel'>Place of Receipt ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofReceipt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PlaceOfRcpt"].ToString() + "' name='txtPlaceofReceipt" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPlaceofDelivery' class='bcLabel'>Place of Delivery ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPlaceofDelivery" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PlaceOfDelivery"].ToString() + "' name='txtPlaceofDelivery" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblFreight' class='bcLabel'>Freight ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtFreight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Frieght"].ToString() + "' name='txtFreight" + i + "'/></td>");
                        sb.Append(" </tr>");

                        DataSet Blds1 = FeBLL.SelectEnqiryHead(CommonBLL.FlagRegularDRP, new Guid(Blds.Tables[0].Rows[i]["BLID"].ToString()), Guid.Empty, Guid.Empty);
                        if (Blds1.Tables.Count > 0 && Blds1.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td colspan='6'>" + FillContainerDetails(Blds1.Tables[0], "BL") + "</td>");
                            sb.Append("</tr>");
                        }

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblIDFNo' class='bcLabel'>IDF No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtIDFNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["IDFNo"].ToString() + "' name='txtIDFNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblECTNNo' class='bcLabel'>ECTN No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtECTNNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["ECTNNo"].ToString() + "' name='txtECTNNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Date"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");


                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTareweight' class='bcLabel'>Tare weight (kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTareweight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Tweight"].ToString() + "' name='txtTareweight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGrossweight' class='bcLabel'>Gross weight (kgs) ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGrossweight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Gweight"].ToString() + "' name='txtGrossweight" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalNoofpkgs' class='bcLabel'>Total No. of pkgs ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalNoofpkgs" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Totalpkgs"].ToString() + "' name='txtTotalNoofpkgs" + i + "'/></td>");
                        sb.Append(" </tr>");

                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Bill of Lading is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindMateReceiptDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet Blds = FeBLL.SelectEnqiryHead(CommonBLL.FlagCommonMstr, ID, Guid.Empty, Guid.Empty); // T
                if (Blds.Tables.Count > 0 && Blds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Blds.Tables[0].Rows.Count; i++)
                    {
                        if (lblMRNo.Text == "")
                        {
                            lblMRNo.Text = " (Date :" + Blds.Tables[0].Rows[i]["CnMRDate"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["MReceiptNmbr"].ToString() + ")";
                        }
                        else
                        {
                            lblMRNo.Text = lblMRNo.Text + " & (Date :" + Blds.Tables[0].Rows[i]["CnMRDate"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["MReceiptNmbr"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>");
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Air Way Bill Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPionvNo' class='bcLabel'>Proforma Invoice No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPionvNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["RefPInvcNo"].ToString() + "' name='txtPionvNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillNumber' class='bcLabel'>Shipping Bill Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtShippingBillNumber" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtShippingBillNumber" + i + "'/>" + Blds.Tables[0].Rows[i]["ShippingBillNo"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["CnMRDate"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofLoading' class='bcLabel'>Port of Loading ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofLoading" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PrtLoding"].ToString() + "' name='txtPortofLoading" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblPortofDischarge' class='bcLabel'>Port of Discharge ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtPortofDischarge" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["PrtDschrg"].ToString() + "' name='txtPortofDischarge" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblMateReceiptNo" + i + "' class='bcLabel'>Mate Receipt No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtMateReceiptNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["MReceiptNmbr"].ToString() + "' name='txtMateReceiptNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalPackages' class='bcLabel'>Total Packages ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalPackages" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["TotlPkgs"].ToString() + "' name='txtTotalPackages" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblEMNo' class='bcLabel'>EM No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtEMNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["EMNmbr"].ToString() + "' name='txtEMNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblGrossWeight' class='bcLabel'>Gross Weight  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtGrossWeight" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["GrossWeight"].ToString() + "' name='txtGrossWeight" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblLinerName' class='bcLabel'>Liner Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtLinerName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["LinerName"].ToString() + "' name='txtLinerName" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblVesselName' class='bcLabel'>Vessel Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtVesselName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["VesselName"].ToString() + "' name='txtVesselName" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblForwarderName' class='bcLabel'>Forwarder Name ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtForwarderName" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["ForwarderName"].ToString() + "' name='txtForwarderName" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblREMARKS' class='bcLabel'>REMARKS ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtREMARKS" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Remarks"].ToString() + "' name='txtREMARKS" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                           "&nbsp;&nbsp;&nbsp;Container Details</td></tr>");

                        DataSet Blds1 = FeBLL.SelectEnqiryHead(CommonBLL.FlagUpdate, new Guid(Blds.Tables[0].Rows[i]["ID"].ToString()), Guid.Empty, Guid.Empty);
                        if (Blds1.Tables.Count > 0 && Blds1.Tables[0].Rows.Count > 0)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td colspan='6'>");
                            sb.Append("<table style='font-size: small; background-color: #F5F4F4; border: solid 1px #ccc' width='100%' cellspacing='0' cellpadding='0' border='0' id='tblItems'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                            sb.Append("<td>S.No.</td><td>Container Number</td><td>Container Type</td><td>Container Size</td><td>Container Seal No.</td><td>Container Date</td>");
                            sb.Append("</tr></thead><tbody class='bcGridViewMain'>");
                            for (int k = 0; k < Blds1.Tables[0].Rows.Count; k++)
                            {
                                sb.Append("<tr>");

                                sb.Append("<td>" + (k + 1) + "</td>");
                                sb.Append("<td>" + Blds1.Tables[0].Rows[k]["CntrNmbr"].ToString() + "</td>");
                                sb.Append("<td>" + Blds1.Tables[0].Rows[k]["CntrType"].ToString() + "</td>");
                                sb.Append("<td>" + Blds1.Tables[0].Rows[k]["CntrSize"].ToString() + "</td>");
                                sb.Append("<td>" + Blds1.Tables[0].Rows[k]["CntrSealNmbr"].ToString() + "</td>");
                                sb.Append("<td>" + Blds1.Tables[0].Rows[k]["CntrDate"].ToString() + "</td>");

                                sb.Append("</tr>");
                            }
                            sb.Append("<tr class='bcGridViewHeaderStyle'><td colspan='6' align='right'><b><span></span></b></td></tr>");
                            sb.Append("</tbody></table>");
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }


                        # endregion

                        // Attachments
                        string ShpmentAttachments = "";
                        ShpmentAttachments = Blds.Tables[0].Rows[i]["Attachments"].ToString().Trim();
                        if (ShpmentAttachments != "")
                            sb.Append("<tr><td colspan='6' >" + Att_open("", "Mate Receipt Attachments", ShpmentAttachments) + "</td></tr>");

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");
                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> Mate Receipt is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        private string BindBRCDetails(Guid ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                FEnquiryBLL FeBLL = new FEnquiryBLL();
                DataSet Blds = FeBLL.SelectEnqiryHead(CommonBLL.FlagVSelect, ID, Guid.Empty, Guid.Empty); // T
                if (Blds.Tables.Count > 0 && Blds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < Blds.Tables[0].Rows.Count; i++)
                    {
                        if (lblEBRNo.Text == "")
                        {
                            lblEBRNo.Text = " (Date :" + Blds.Tables[0].Rows[i]["Date"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["BRCNo"].ToString() + ")";
                        }
                        else
                        {
                            lblEBRNo.Text = lblEBRNo.Text + " & (Date :" + Blds.Tables[0].Rows[i]["Date"].ToString() + " :: No : " + Blds.Tables[0].Rows[i]["BRCNo"].ToString() + ")";
                        }

                        sb.Append("<table align='center' class='MainTable3'>");
                        sb.Append("<tbody><tr>");
                        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                        # region Air Way Bill Details

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillNumber' class='bcLabel'>Shipping Bill Number ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><textarea id='txtShippingBillNumber" + i + "' class='bcAsptextboxmulti' type='text' " +
                            " readonly='readonly' name='txtShippingBillNumber" + i + "'/>" + Blds.Tables[0].Rows[i]["ShpingBillNo"].ToString() + "</textarea></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblShippingBillPort' class='bcLabel'>Shipping Bill Port ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtShippingBillPort" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["ShpingBillPort"].ToString() + "' name='txtShippingBillPort" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDate' class='bcLabel'>Date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Date"].ToString() + "' name='txtDate" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBanksFileNo' class='bcLabel'>Banks File No ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBanksFileNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["BanksFileNo"].ToString() + "' name='txtBanksFileNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblUploaddate' class='bcLabel'>Upload date ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtUploaddate" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["uploadDate"].ToString() + "' name='txtUploaddate" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBillIDNo" + i + "' class='bcLabel'>Bill ID No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBillIDNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["BillIDNo"].ToString() + "' name='txtBillIDNo" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBRCNo' class='bcLabel'>BRC No. ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBRCNo" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["BRCNo"].ToString() + "' name='txtBRCNo" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblRealisedValue' class='bcLabel'>Realised Value ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtRealisedValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["RealisedValue"].ToString() + "' name='txtRealisedValue" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDeductedValue' class='bcLabel'>Deducted Value  ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDeductedValue" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["DeductedValue"].ToString() + "' name='txtDeductedValue" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='bcTdnormal'><span id='lblTotalAmt' class='bcLabel'>Total Amt ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtTotalAmt" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["TotalAmount"].ToString() + "' name='txtTotalAmt" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblDateofRealisation' class='bcLabel'>Date of Realisation ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtDateofRealisation" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["DateRealised"].ToString() + "' name='txtDateofRealisation" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblCurrency' class='bcLabel'>Currency ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtCurrency" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Currency"].ToString() + "' name='txtCurrency" + i + "'/></td>");
                        sb.Append(" </tr>");

                        sb.Append("<tr>");
                        string BRCStatus = Blds.Tables[0].Rows[i]["BRCStatus"].ToString() == "1" ? "ACTIVE" : "INACTIVE";
                        string UtiisedStat = Blds.Tables[0].Rows[i]["BRCUtiliseStat"].ToString() == "1" ? "Available" : "UnAvailable";
                        sb.Append("<td class='bcTdnormal'><span id='lblBRCStatus' class='bcLabel'>BRC Status ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBRCStatus" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + BRCStatus + "' name='txtBRCStatus" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblBRCUtilisationStatus' class='bcLabel'>BRC Utilisation Status ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtBRCUtilisationStatus" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + UtiisedStat + "' name='txtBRCUtilisationStatus" + i + "'/></td>");
                        sb.Append("<td class='bcTdnormal'><span id='lblREMARKS' class='bcLabel'>REMARKS ::</span></td>");
                        sb.Append("<td class='bcTdnormal'><input id='txtREMARKS" + i + "' class='bcAsptextbox' type='text' " +
                            " readonly='readonly' value='" + Blds.Tables[0].Rows[i]["Remarks"].ToString() + "' name='txtREMARKS" + i + "'/></td>");
                        sb.Append(" </tr>");


                        # endregion

                        sb.Append("</table>");// 2
                        sb.Append("</td>");//1
                        sb.Append("</tr>");

                        sb.Append("</tbody></tr>");
                        sb.Append("</table>");
                    }
                }
                else
                    sb.Append("<b><center> E-BRC is not available. </center></b>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// Bind Central Excise Details
        /// </summary>
        /// <returns></returns>
        private string BindCentralExciseDetails(Guid ID)
        {
            try
            {
                //string Attachments = "";
                //DataSet CommonDt = RCEDBL.SelectRqstCEDtls(CommonBLL.FlagSelectAll, int.Parse(ID.ToString()));
                //if (CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                //{
                //    StringBuilder sb = new StringBuilder();
                //    for (int i = 0; i < CommonDt.Tables[0].Rows.Count; i++)
                //    {
                //        //DataSet dss = new DataSet();
                //        //dss.Tables.Add(CommonDt.Tables[1].Copy());
                //        //Session["Items"] = dss;
                //        sb.Append("<table align='center' class='MainTable3'>");
                //        sb.Append("<tbody><tr>");
                //        sb.Append("<td class='bcTdNewTable'>"); //class='bcTdNewTable' // 1
                //        sb.Append("<table style='background-color: #F5F4F4; border: solid 1px #ccc' width='100%'>");// 2

                //        sb.Append("<tr>");
                //        sb.Append("<td class='bcTdnormal'><span id='lblCustName' class='bcLabel'>Name of Customer:<font color='red' " +
                //            " size='2'></font>:</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='ddlcustmr" + i + "' class='bcAsptextbox' type='text' " +
                //            " readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["CustName"].ToString() + "' name='ddlcustmr" + i +
                //            "'/></td>");
                //        sb.Append("<td class='bcTdnormal'><span id='lblDept' class='bcLabel'>Supplier Category:" +
                //            ":</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='Ddldeptnm" + i + "' class='bcAsptextbox' type='text' " +
                //            " readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["SuplrNm"].ToString() + "' name='Ddldeptnm" + i +
                //            "'/></td>");
                //        sb.Append("<td class='bcTdnormal'><span id='lblEnqNo' class='bcLabel'>Supplier Name: " +
                //            ":</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='ddlfenqy" + i + "' class='bcAsptextbox' " +
                //            " type='text' readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["SuplrNm"].ToString() +
                //            "' name='ddlfenqy" + i + "'/></td>");
                //        sb.Append(" </tr>");

                //        sb.Append("<tr>");
                //        sb.Append("<td class='bcTdnormal'><span id='lblSubject' class='bcLabel'>FPO(s): " +
                //            ":</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='txtsubject" + i + "' class='bcAsptextbox' " +
                //            " type='text' readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["FpoIds"].ToString() +
                //            "' name='txtsubject" + i + "'/></td>");
                //        sb.Append("<td class='bcTdnormal'><span id='lbleqdt" + i + "' class='bcLabel'>LPO(s):</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='txteqdt" + i + "' class='bcAsptextbox' type='text' " +
                //            " readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["LpoIds"].ToString() + "' name='txteqdt" + i +
                //            "'/></td>");
                //        sb.Append("<td class='bcTdnormal'><span id='lblRcsdt" + i + "' class='bcLabel'>Reference No.: " +
                //            " :</span></td>");
                //        sb.Append("<td class='bcTdnormal'><input id='txtRcqdt" + i + "' class='bcAsptextbox' type='text' " +
                //            " readonly='readonly' value='" + CommonDt.Tables[0].Rows[i]["RefNo"].ToString() + "' name='txtRcdt" + i +
                //            "'/></td>");
                //        sb.Append("</tr>");
                //        sb.Append("</table>");// 2
                //        sb.Append("</td>");//1
                //        sb.Append("</tr>");

                //        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'> " +
                //            "&nbsp;&nbsp;&nbsp;Added Items</td></tr>");//Added Items
                //        sb.Append("<tr>");
                //        sb.Append("<td>");//1                        
                //        sb.Append(FillGridView("FE"));//Items
                //        sb.Append("</td>");//1
                //        sb.Append("</tr>");

                //        // Comments
                //        sb.Append("<tr style='background-color: Gray; font-size: small; color: White;'><td colspan='6'>" +
                //            "&nbsp;&nbsp;&nbsp;Comments</td></tr>");
                //        //List<string> FQComments = CommonBLL.SplitTextByWord(CommonDt.Tables[0].Rows[i]["Comments"].ToString(), "$~#");
                //        sb.Append("<tr>");
                //        sb.Append("<td colspan='999' style='font-size: small;'>");
                //        //if (FQComments.Count > 0)
                //        //if(CommonDt.Tables.Count > 2)
                //        if (CommonDt.Tables.Count >= 2 && CommonDt.Tables[2].Rows.Count > 0)
                //        {
                //            sb.Append("<table width='100%'>");
                //            for (int a = 0; a < CommonDt.Tables[2].Rows.Count; a++)
                //            {
                //                sb.Append("<tr>");
                //                //sb.Append("<td class='bcTdnormal'><span id='lblDueDt' class='bcLabel'>Comment <font color='red' size='2'>" 
                //                //+ a + "</font> : </span></td>");
                //                sb.Append("<td ><div id='mousefollow-examples'><div title='<b>Commented By : </b>"
                //                    + CommonDt.Tables[2].Rows[a]["CreatedBy"].ToString() + "<br/><b>Commented Date : </b>"
                //                    + CommonDt.Tables[2].Rows[a]["CreatedDate"].ToString() + "'>" + (a + 1) + ") "
                //                    + CommonDt.Tables[2].Rows[a]["comments"].ToString() + "</div></div></td>");
                //                sb.Append("</tr>");
                //            }
                //            sb.Append("</table>");
                //        }
                //        else
                //            sb.Append("No Comments");
                //        sb.Append("</td>");
                //        sb.Append("</tr>");
                //        // End of Comments

                //        sb.Append("</tbody></tr>");
                //        sb.Append("</table>");
                //    }
                //    return sb.ToString();
                //}
                return string.Empty;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

        # region Items & Payments

        private string FillGridView(string Info)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["Items"];
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table style='font-size: small; background-color: #F5F4F4; border: solid 1px #ccc' width='100%' cellspacing='0' cellpadding='0' border='0' id='tblItems'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                if (Info == "LQ")
                {
                    sb.Append("<td>SNo</td><td>Item Description</td>" +
                        "<td>Part No</td><td>Specification</td><td>Make</td><td align='center'>Quantity</td><td>Units</td><td align='right'>Price</td><td align='right'>Amount</td><td align='right'>Ex Duty</td><td align='right'>Discount</td><td align='right'>QPrice</td>");
                }
                else if (Info == "LPO")
                {
                    sb.Append("<td>SNo</td><td>Item Description</td>" +
                        "<td>Part No</td><td>Specification</td><td>Make</td><td align='center'>Quantity</td><td>Units</td><td align='right'>Price</td><td align='right'>Amount</td><td align='right'>Ex Duty</td><td align='right'>Discount</td>");
                }
                else if (Info == "FE" || Info == "LE")
                {
                    sb.Append("<td>SNo</td><td>Item Description</td>" +
                        "<td>Part No</td><td>Specification</td><td>Make</td><td align='center'>Quantity</td><td>Units</td>");
                }
                else if (Info != "FE" && Info != "LE" && Info != "CT1" && Info != "GRN")
                {
                    sb.Append("<td>SNo</td><td>Item Description</td>" +
                        "<td>Part No</td><td>Specification</td><td>Make</td><td align='center'>Quantity</td><td>Units</td><td>Price</td><td>Amount</td>");
                }
                else if (Info == "CT1")
                {
                    sb.Append("<td>&nbsp;</td><td>SNo</td><td align='center'>LPO No.</td><td>ItemDesc & Spec.</td>" +
                        "<td>HS Code</td><td>Quantity</td><td>Rate</td><td>Discount</td><td>Packing</td><td>Ex-Duty</td><td>Total Amount</td>");
                }
                else if (Info == "GRN")
                {
                    sb.Append("<td>SNo</td><td>Item Desc</td><td>Part No</td><td>Make</td><td>Actual Qty</td><td>Arrived Qty</td><td>GDN/GRN Rcvd Qty</td><td>Rmng Qty</td><td>Units</td>");
                }
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    decimal TotalAmount = 0;
                    decimal TotalRate = 0;
                    if (Info != "CT1")
                    {
                        if (ds.Tables[0].Columns.Contains("Amount"))
                            TotalAmount = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Amount)", "").ToString());
                    }
                    else
                    {
                        if (ds.Tables[0].Columns.Contains("TotalAmt"))
                            TotalAmount = Convert.ToDecimal(ds.Tables[0].Compute("Sum(TotalAmt)", "").ToString());

                        if (ds.Tables[0].Columns.Contains("Rate"))
                            TotalRate = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Rate)", "").ToString());
                    }

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string sno = (i + 1).ToString();
                        sb.Append("<tr valign='Top'>");
                        if (Info == "CT1")
                        {
                            sb.Append("<td><input id='ckhChaild" + sno + "' type='checkbox' disabled='disabled'");
                            if (Convert.ToBoolean(ds.Tables[0].Rows[i]["IsChecked"]))
                                sb.Append(" checked='checked' ");
                            sb.Append(" name='ckhChaild'/></td>");
                        }
                        sb.Append("<td align='center'>" + sno + "</td>");
                        if (Info != "CT1")
                            sb.Append("<td valign='Top' width='300px'><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</div></td>");
                        else
                        {
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["LPONo"].ToString() + "</td>");
                            sb.Append("<td valign='Top' width='300px'><div class='expanderR'>" + ds.Tables[0].Rows[i]["SpecDes"].ToString() + "</div></td>");
                        }
                        if (Info != "CT1")
                        {
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</td>");//PartNo
                            if (Info != "GRN")
                                sb.Append("<td width='200px'><div class='expanderR'>" + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</div></td>");
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["Make"].ToString() + "</td>");
                        }
                        else
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["HSCode"].ToString() + "</td>");//HSCode

                        sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["Quantity"].ToString() + "</td>");
                        if (Info == "GRN")
                        {
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["ReceivedQty"].ToString() + "</td>");
                            sb.Append("<td align='center'>" + ds.Tables[0].Rows[i]["RemainingQty"].ToString() + "</td>");
                            sb.Append("<td align='center'>" + (Convert.ToDecimal(ds.Tables[0].Rows[i]["Quantity"].ToString()) - Convert.ToDecimal(ds.Tables[0].Rows[i]["ReceivedQty"].ToString())) + "</td>");
                        }

                        if (Info != "CT1")
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");

                        if (Info != "FE" && Info != "LE" && Info != "CT1" && Info != "GRN")
                        {
                            sb.Append("<td align='right'>" + ds.Tables[0].Rows[i]["Rate"].ToString() + "</td>");
                            sb.Append("<td align='right'>" + ds.Tables[0].Rows[i]["Amount"].ToString() + "</td>");
                        }

                        if (Info == "CT1")
                            sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"].ToString()).ToString("N") + "</td>");

                        if (Info == "LQ" || Info == "LPO" || Info == "CT1")
                        {
                            sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["ExDutyPercentage"].ToString()).ToString("N") + " %</td>");

                            if (Info == "CT1")
                                sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["PackingPercentage"].ToString()).ToString("N") + "%</td>");

                            sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["DiscountPercentage"].ToString()).ToString("N") + " %</td>");

                            if (Info == "LQ")
                                sb.Append("<td align='right'>" + ds.Tables[0].Rows[i]["QPrice"].ToString() + "</td>");
                        }

                        if (Info == "CT1")
                            sb.Append("<td align='right'>" + Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalAmt"].ToString()).ToString("N") + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr class='bcGridViewHeaderStyle'>");
                    if (Info != "FE" && Info != "LE" && Info != "CT1" && Info != "GRN")
                    {
                        sb.Append("<td colspan='3' align='right'><b><span></span></b></td>");
                        sb.Append("<td colspan='6' align='right'><b><span>Total Amount : " + TotalAmount.ToString("N") + "</span></b></td>");
                        sb.Append("<td colspan='3'><b><span></span></b></td>");
                    }
                    else if (Info == "CT1")
                    {
                        sb.Append("<td colspan='7' align='right'><b><span>Total Rate : " + TotalRate.ToString("N") + "</span></b></td>");
                        sb.Append("<td colspan='4' align='right'><b><span>Total Amount : " + TotalAmount.ToString("N") + "</span></b></td>");
                    }
                    else if (Info == "GRN")
                        sb.Append("<td colspan='9'></td>");
                    else
                        sb.Append("<td colspan='7'></td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return "";
            }
        }

        public string FillPaymentTerms()
        {
            try
            {
                DataTable dt = (DataTable)Session["PaymentsAll"];
                dt.Columns["PaymentSerialNo"].ColumnName = "SNo";
                dt.Columns["Percentage"].ColumnName = "PaymentPercentage";
                dt.Columns["Against"].ColumnName = "Description";
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' border='0' id='tblPaymentTerms' align='center'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                sb.Append("<td>SNo</td><td>Payment Percentage</td><td>Description</td><td width='45px'>&nbsp;</td>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td><input type='text' readonly='readonly' name='txtPercAmt' class='Amount' value='" + dt.Rows[i]["PaymentPercentage"].ToString() + "'  id='txtPercAmt" + SNo + "' onkeypress='return isNumberKey(event)' onchange='getPaymentValues(" + SNo + ")' maxlength='3'/></td>");
                        sb.Append("<td><input type='text' readonly='readonly' name='txtDesc' class='Amount' value='" + dt.Rows[i]["Description"].ToString() + "'  id='txtDesc" + SNo + "' onchange='getPaymentValues(" + SNo + ")'/></td>");
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
                return string.Empty;
            }
        }

        public string Att_open(string val, string heading, string Attachments)
        {
            StringBuilder sbb = new StringBuilder();
            try
            {
                string url = "../uploads/";
                string url1 = "~/no-script.html";
                ArrayList al = new ArrayList();
                al.AddRange(Attachments.Trim().Split(','));
                sbb.Append("<div id='accordion" + accordions + "' style='font-size:small; text-shadow=0px 0px #000000;'><h5>" + heading + "</h5><div>");
                for (int i = 0; i < al.Count; i++)
                {
                    string finUrl = url + "" + al[i].ToString();
                    string fileName = al[i].ToString();
                    int fileExtPos = fileName.LastIndexOf(".");
                    if (fileExtPos >= 0)
                        fileName = fileName.Substring(0, fileExtPos);

                    sbb.Append(" " + (i + 1) + ") <a href='" + finUrl + "' id='openfile" + i + "' onclick='saveToDisk("
                        + finUrl + "," + al[i].ToString() + ");' target='_blank'>" + fileName + "</a><br/>");
                }
                sbb.Append("</div></div>");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            HFAccordions.Value = accordions.ToString();
            accordions++;
            return sbb.ToString(); ;
        }

        #endregion

        # region CT-1 Calc

        private DataTable CTOne_Calculations(DataTable CommonDt, decimal CalDsnt, decimal CalPkng, decimal CalExcse)
        {
            try
            {
                decimal TotalAmt = 0;
                for (int i = 0; i < CommonDt.Rows.Count; i++)
                {
                    Decimal Rate = Convert.ToDecimal(CommonDt.Rows[i]["Rate"].ToString());
                    TotalAmt = Rate - (CalDsnt * Rate) / 100;
                    TotalAmt = TotalAmt + (CalPkng * TotalAmt) / 100;
                    TotalAmt = TotalAmt + (CalExcse * TotalAmt) / 100;
                    TotalAmt = Convert.ToDecimal(CommonDt.Rows[i]["Quantity"].ToString()) * TotalAmt;
                    CommonDt.Rows[i]["TotalAmt"] = TotalAmt;
                }
                CommonDt.AcceptChanges();
                return CommonDt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return null;
            }
        }

        # endregion

        # region CT-1 ARE-1 Forms

        public string FillCT1Dtls(DataTable dt)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblCT1Dtls' " +
                    " align='center' style='font-size: medium;'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                sb.Append("<th class='rounded-First'>SNo</th><th>CT1 Number</th><th>Date</th><th>CT1 Value</th><th>ARE-1 Number</th><th>Forms</th><th class='rounded-Last'>ARE1 Value</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");

                        sb.Append("<td style='font-size: small;'>" + dt.Rows[i]["CT1No"].ToString().ToString() + "</td>");
                        sb.Append("<td><input type='text' name='txtDate' value='" + dt.Rows[i]["Date"].ToString() + "' id='txtDate" + SNo
                            + "'  readonly='readonly' style='text-align: left; " + " width:80px;' class='bcAsptextbox DatePicker'/></td>");

                        sb.Append("<td><input type='text' readonly='readonly' name='txtValue' value='"
                            + dt.Rows[i]["CT1Value"].ToString() + "' id='txtValue" + SNo
                            + "' style='text-align: right; width:80px;' class='bcAsptextbox'/></td>");

                        sb.Append("<td><input type='text' name='txtARE1No' onfocus='this.select()' "
                                + " onMouseUp='return false' value='"
                                + dt.Rows[i]["ARE1No"].ToString().ToString()
                                + "'  id='txtARE1No" + SNo + "' style='text-align: LEFT;' class='bcAsptextbox'/></td>");

                        string[] ARE1Forms = (dt.Rows[i]["Forms"].ToString()).Split(',');
                        string white = "check = 'false'", buff = "check = 'false'", blue = "check = 'false'",
                            green = "check = 'false'", pink = "check = 'false'";
                        if (ARE1Forms.Length > 1)
                        {
                            white = (Convert.ToBoolean(ARE1Forms[0].ToString()) ? "checked = 'true'" : "check = 'false'");
                            buff = (Convert.ToBoolean(ARE1Forms[1].ToString()) ? "checked = 'true'" : "check = 'false'");
                            blue = (Convert.ToBoolean(ARE1Forms[2].ToString()) ? "checked = 'true'" : "check = 'false'");
                            green = (Convert.ToBoolean(ARE1Forms[3].ToString()) ? "checked = 'true'" : "check = 'false'");
                            pink = (Convert.ToBoolean(ARE1Forms[4].ToString()) ? "checked = 'true'" : "check = 'false'");
                        }

                        sb.Append("<td><table><tr><td><input type='checkbox' id='chkbARE1FormWht" + SNo
                            + "' name='chkbARE1FormWht' " + white + " title ='White' value='1' /><span style='font-size:small'>White</span> " +
                            "</td><td><input type='checkbox' " + buff + " id='chkbARE1FormBff" + SNo
                            + "' name='chkbARE1FormBff' title ='Buff' value='2' /><span style='font-size:small'>Buff</span></td>" +
                            "<td><input type='checkbox' " + blue + "  id='chkbARE1FormBle" + SNo
                            + "' name='chkbARE1FormBle' title ='Blue' value='3' /><span style='font-size:small'>Blue</span></td>" +
                            "</tr><tr><td><input type='checkbox' " + green + "  id='chkbARE1FormGrn" + SNo
                            + "' name='chkbARE1FormGrn' title ='Green' value='4' /><span style='font-size:small'>Green</span></td><td>" +
                            "<input type='checkbox' " + pink + "  id='chkbARE1FormPnk" + SNo
                            + "' name='chkbARE1FormPnk' title ='Pink' value='5' /><span style='font-size:small'>Pink</span>" +
                            "</td><td>&nbsp;</td></tr></table></td>");

                        sb.Append("<td><input type='text' name='txtARE1Value' value='"
                            + Convert.ToDouble(dt.Rows[i]["AREValue"].ToString()) + "' id='txtARE1Value" + SNo
                            + "' style='text-align: right; width:60px;' class='bcAsptextbox'/></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("</tbody>");
                }
                sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='6' class='rounded-foot-right' " +
                    " align='left'><input id='HfMessage' type='hidden' name='HfMessage' value=''/></th></tfoot>");
                sb.Append("</table>");


                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
                return string.Empty;
            }
        }

        # endregion

        # region Methods

        private DataTable GetCheckListItemsEdit(DataSet ds)
        {
            DataTable dt = null;
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt = CommonBLL.EmptyDTCheckedList();
                    dt.Rows[0].Delete();
                    DataRow dr;
                    int PkgsCount = 1;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        Guid CstmrID = new Guid(ds.Tables[0].Rows[i]["CustomerID"].ToString());
                        dr["CustomerID"] = CstmrID;
                        int AddGDNPkgs = (ds.Tables[0].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[0].Rows[i]["NoOfPkgs"].ToString()));
                        if (AddGDNPkgs == 1 || AddGDNPkgs == 0)
                            dr["PkgNos"] = (PkgsCount + AddGDNPkgs - 1);
                        else
                            dr["PkgNos"] = PkgsCount + " - " + (PkgsCount + AddGDNPkgs - 1);
                        PkgsCount += AddGDNPkgs;
                        dr["SupplierID"] = new Guid(ds.Tables[0].Rows[i]["SupplierID"].ToString());
                        dr["SupplierNm"] = ds.Tables[0].Rows[i]["SupplierNm"].ToString();
                        dr["NoOfPkgs"] = (ds.Tables[0].Rows[i]["NoOfPkgs"].ToString() == "" ? 0 :
                            Convert.ToInt32(ds.Tables[0].Rows[i]["NoOfPkgs"].ToString()));
                        dr["FPONOs"] = ds.Tables[0].Rows[i]["FPONos"].ToString();
                        dr["FPOs"] = ds.Tables[0].Rows[i]["FPOs"].ToString();
                        dr["LR_GodownNo"] = ds.Tables[0].Rows[i]["LR_GodownNo"].ToString();
                        dr["IsARE1"] = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsARE1"].ToString());
                        dr["NetWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["NetWeight"].ToString());
                        dr["GrWeight"] = Convert.ToDecimal(ds.Tables[0].Rows[i]["GrWeight"].ToString());
                        dr["aType"] = ds.Tables[0].Rows[i]["aType"].ToString();
                        dr["Remarks"] = ds.Tables[0].Rows[i]["Remarks"].ToString();

                        if (ds.Tables[0].Rows[i]["GDNID"].ToString() != "")
                            dr["GDNID"] = new Guid(ds.Tables[0].Rows[i]["GDNID"].ToString());
                        else
                            dr["GDNID"] = Guid.Empty;
                        if (ds.Tables[0].Rows[i]["GRNID"].ToString() != "")
                            dr["GRNID"] = new Guid(ds.Tables[0].Rows[i]["GRNID"].ToString());
                        else
                            dr["GRNID"] = Guid.Empty;
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Full Details", ex.Message.ToString());
            }
            return dt;
        }

        private string FillContainerDetails(DataTable dt, string Info)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' " +
                " id='tblPaymentTerms' align='center'><thead align='center'><tr>");
                if (Info == "BL")
                    sb.Append("<th class='rounded-First'>SNo</th><th>Container Type</th><th>Container No</th><th>C Seal No.</th>" +
                        "<th class='rounded-Last'>L Seal No.</th>");
                else
                    sb.Append("<th class='rounded-First'>SNo</th><th>Container Type</th><th>Container No</th><th>Container Size</th><th>Seal No.</th>" +
                        "<th>Date</th><th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td>" + SNo + "</td>");
                        sb.Append("<td>&nbsp;<input type='text' name='txtCNO' value='" + dt.Rows[i]["CTYPE"].ToString().Replace("'", "") + "' "
                            + " id='txtCNO" + SNo + "' readonly='readonly' class='bcAsptextbox'/></td>");

                        sb.Append("<td>&nbsp;<input type='text' name='txtCNO' value='" + dt.Rows[i]["CNO"].ToString() + "' "
                            + " id='txtCNO" + SNo + "' readonly='readonly' class='bcAsptextbox'/></td>");

                        if (Info == "SBL")
                            sb.Append("<td>&nbsp;<input type='text' name='txtCSize' value='" + dt.Rows[i]["CntrSize"].ToString() + "' "
                            + " id='txtCSize" + SNo + "' readonly='readonly' class='bcAsptextbox'/></td>");

                        sb.Append("<td align='right'><input type='text' name='txtCSealNo' readonly='readonly' value='"
                            + dt.Rows[i]["CSealNo"].ToString() + "'  id='txtCSealNo" + SNo + "' maxlength='500' class='bcAsptextbox'/></td>");

                        if (Info == "SBL")
                            sb.Append("<td>&nbsp;<input type='text' name='txtCDT' value='" + dt.Rows[i]["CntrDate"].ToString() + "' "
                            + " id='txtCDT" + SNo + "' readonly='readonly' class='bcAsptextbox'/></td><td>&nbsp;</td>");
                        else
                            sb.Append("<td><input type='text' name='txtLSealNo' value='" + dt.Rows[i]["LSealNo"].ToString() + "' "
                                + " id='txtLSealNo" + SNo + "' readonly='readonly' class='bcAsptextbox'/></td>");
                        sb.Append("</tr>");
                    }
                    if (Info == "BL")
                        sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='4' class='rounded-foot-right' " +
                        " align='left'><input id='HfMessage' type='hidden' name='HfMessage' value='"
                        + "" + "'/></th></tfoot>");
                    else
                        sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='6' class='rounded-foot-right' " +
                        " align='left'><input id='HfMessage' type='hidden' name='HfMessage' value='"
                        + "" + "'/></th></tfoot>");
                }
                sb.Append("</tbody></table>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Bill of Lading", ex.Message.ToString());
                return string.Empty;
            }
        }

        # endregion
    }
}