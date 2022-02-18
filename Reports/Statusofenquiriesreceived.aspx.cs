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

namespace VOMS_ERP.Reports
{
    public partial class Statusofenquiriesreceived : System.Web.UI.Page
    {
        # region Variables
        int accordions = 0;
        static DataSet EditDS;
        static string GeneralCtgryID;
        static int Itemtables = 0;
        DspchInstnsBLL DIBL = new DspchInstnsBLL();
        EnumMasterBLL EMBL = new EnumMasterBLL();
        GrnBLL GDNBL = new GrnBLL();
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
        StringBuilder sbm = new StringBuilder();
        string br = ",<br/>\n";
        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Statusofenquiriesreceived));
                if (!IsPostBack)
                {   
                    BindData();
                    txtFromDate.Attributes.Add("readonly", "readonly");
                    txtToDate.Attributes.Add("readonly", "readonly");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Statusofenquiriesreceived", ex.Message.ToString());
            }
        }


        #region Bind Data

        private void BindData()
        {
            try
            {
                DateTime fromdate = txtFromDate.Text.Trim() == "" ? DateTime.Now.AddDays(-15) : CommonBLL.DateInsert(txtFromDate.Text);
                DateTime todate = txtToDate.Text.Trim() == "" ? DateTime.Now : CommonBLL.DateInsert(txtToDate.Text);
                
                if (txtFromDate.Text.Trim() == "")
                    txtFromDate.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(-15));
                if (txtToDate.Text.Trim() == "")
                    txtToDate.Text = CommonBLL.DateDisplay(DateTime.Now);

                EditDS = new DataSet();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                EditDS = NEBLL.StatusOfEnquiriesReceived(fromdate, todate);
                StringBuilder sb = new StringBuilder();
                sb.Append("<table id='tblFeStaging'><thead>");
                sb.Append("<tr>"
                    + "<td>ENQUIRY NO</td>"
                    + "<td>ENQUIRY DATE</td>"
                    + "<td>CUSTOMER NAME</td>"
                    + "<td>DATE OF F -QTN GIVEN TO CUSTOMER</td>"
                    + "<td>FPO  NO</td>"
                    + "<td>DATE OF FPO</td>"
                    + "<td>FPO VALUE (USD)</td>"
                    + "<td>DATE OF LPO</td>"
                    + "<td>DATE OF MATERIAL RECEIVED AT GODOWN</td>"
                    + "<td>DATE OF LEO</td>"
                    + "<td>FOB-USD</td>"
                    + "<td>FOB-INR</td>"
                    + "<td>LPO INCHARGE</td>"
                    + "<td>REMARKS</td>"
                    + "</tr>");
                sb.Append("</thead><tbody>");
                if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < EditDS.Tables[0].Rows.Count; i++)
                    {
                        sb.Append("<tr><td>" + EditDS.Tables[0].Rows[i]["enquirenumber"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["enquirydate"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["Customer"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["QuotationDate"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["FPO NO"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["DATE OF FPO"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["FPO VALUE (USD)"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["DATE OF LPO"].ToString() + "</td>");                        
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["ReceivedAtGodown"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["LEODate"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["FOB INR"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["FOB USD"].ToString() + "</td>");
                        sb.Append("<td>" + EditDS.Tables[0].Rows[i]["LPOIncharge"].ToString() + "</td>");
                        sb.Append("<td></td></tr>");
                    }
                }
                else
                {
                    sb.Append("<tr><td colspan='15'> No Records to Display. </td></tr>");
                }
                sb.Append("<tbody>");
                divtable.InnerHtml = sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Statusofenquiriesreceived", ex.Message.ToString());
            }
        }

        #endregion

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BindData();
        }

    }
}