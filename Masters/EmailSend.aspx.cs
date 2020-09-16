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
using System.Net.Mail;
using System.Text;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Threading;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Masters
{
    public partial class EmailSend : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        DspchInstnsBLL DIBL = new DspchInstnsBLL();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        EMailsDetailsBLL EMDBL = new EMailsDetailsBLL();
        ReportDocument crystalReport = new ReportDocument();
        EmailBodyBLL EBLL = new EmailBodyBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        static string MailType = string.Empty;
        static string RedirectAddr = string.Empty;
        #endregion

        #region Default Page Load

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        /// <summary>
        /// Default Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(EmailSend));
                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                if (!IsPostBack)
                {
                    Session.Remove("uploads");
                    GetData();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(crystalReport);
                crystalReport.Dispose();
                //CrystalReportViewer1.Dispose();
                //CrystalReportViewer1 = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Bind Default Data

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <returns></returns>
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        /// <summary>
        /// Get Mail Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                if (Request.QueryString["FeID"] != null && Request.QueryString["FeID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["FeID"].ToString()), "FE");
                    //lbtnAtchmnt.Text = GenerateFePDF(int.Parse(Request.QueryString["FeID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFePDF_New(new Guid(Request.QueryString["FeID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    MailType = "FE"; RedirectAddr = "../Enquiries/FEStatus.aspx";
                }
                else if (Request.QueryString["PeID"] != null && Request.QueryString["PeID"].ToString() != "") //PE Customer Flow
                {
                    GetEmailDetails(new Guid(Request.QueryString["PeID"].ToString()), "PE");
                    //lbtnAtchmnt.Text = GenerateFePDF(int.Parse(Request.QueryString["FeID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFePDF_New(new Guid(Request.QueryString["PeID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    MailType = "PE"; RedirectAddr = "../Customer Access/PIStatus.aspx";
                }
                else if (Request.QueryString["FFeID"] != null && Request.QueryString["FFeID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["FFeID"].ToString()), "FFE");
                    //lbtnAtchmnt.Text = GenerateFePDF(int.Parse(Request.QueryString["FeID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFePDF_New(new Guid(Request.QueryString["FFeID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    MailType = "FFE"; RedirectAddr = "../Enquiries/FloatedEnquiryStatus.aspx";
                }
                else if (Request.QueryString["PIFFeID"] != null && Request.QueryString["PIFFeID"].ToString() != "")//PI FFE
                {
                    GetEmailDetails(new Guid(Request.QueryString["PIFFeID"].ToString()), "PI_FFE");
                    lbtnAtchmnt.Text = GenerateFePDF_New(new Guid(Request.QueryString["PIFFeID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    MailType = "PI_FFE"; RedirectAddr = "../Customer Access/Float_PIStatus.aspx";
                }
                else if (Request.QueryString["LeID"] != null && Request.QueryString["LeID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["LeID"].ToString()), "LE");
                    lbtnAtchmnt.Text = GenerateLePDF_New(new Guid(Request.QueryString["LeID"].ToString()));
                    MailType = "LE"; RedirectAddr = "../Enquiries/LEStatus.aspx";
                }
                else if (Request.QueryString["LQID"] != null && Request.QueryString["LQID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["LQID"].ToString()), "LQ");
                    lbtnAtchmnt.Text = GenerateLQPDF_New(new Guid(Request.QueryString["LQID"].ToString()));
                    MailType = "LQ"; RedirectAddr = "../Quotations/LQStatus.aspx";
                }
                else if (Request.QueryString["CLQID"] != null && Request.QueryString["CLQID"].ToString() != "") //Customer Flow
                {
                    GetEmailDetails(new Guid(Request.QueryString["CLQID"].ToString()), "CLQ");
                    lbtnAtchmnt.Text = GenerateLQPDF_New(new Guid(Request.QueryString["CLQID"].ToString()));
                    MailType = "CLQ"; RedirectAddr = "../Customer Access/Customer_LocalQuotation_Status.aspx";
                }
                else if (Request.QueryString["FqID"] != null && Request.QueryString["FqID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["FqID"].ToString()), "FQ");
                    //lbtnAtchmnt.Text = GenerateFQPDF(int.Parse(Request.QueryString["FqID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFQPDF_New(new Guid(Request.QueryString["FqID"].ToString()));
                    MailType = "FQ"; RedirectAddr = "../Quotations/FQStatus.aspx";
                }
                else if (Request.QueryString["CFqID"] != null && Request.QueryString["CFqID"].ToString() != "") //Customer Flow
                {
                    GetEmailDetails(new Guid(Request.QueryString["CFqID"].ToString()), "CFQ");
                    //lbtnAtchmnt.Text = GenerateFQPDF(int.Parse(Request.QueryString["FqID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFQPDF_New(new Guid(Request.QueryString["CFqID"].ToString()));
                    MailType = "CFQ"; RedirectAddr = "../Customer Access/FQStatusCustomer.aspx";
                }
                else if (Request.QueryString["FpoID"] != null && Request.QueryString["FpoID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["FpoID"].ToString()), "FPO");
                    //lbtnAtchmnt.Text = GenerateFPOPDF(int.Parse(Request.QueryString["FpoID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFPOPDF_New(new Guid(Request.QueryString["FpoID"].ToString()));
                    MailType = "FPO"; RedirectAddr = "../Purchases/FPOStatus.aspx";
                }
                else if (Request.QueryString["CFpoID"] != null && Request.QueryString["CFpoID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["CFpoID"].ToString()), "CFPO");
                    //lbtnAtchmnt.Text = GenerateFPOPDF(int.Parse(Request.QueryString["FpoID"].ToString()));
                    lbtnAtchmnt.Text = GenerateFPOPDF_New(new Guid(Request.QueryString["CFpoID"].ToString()));
                    MailType = "CFPO"; RedirectAddr = "../Purchases/FPOVendorStatus.aspx";
                }
                else if (Request.QueryString["LpoID"] != null && Request.QueryString["LpoID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["LpoID"].ToString()), "LPO");
                    //lbtnAtchmnt.Text = GenerateLPOPDF(int.Parse(Request.QueryString["LpoID"].ToString()));
                    lbtnAtchmnt.Text = GenerateLPOPDF_New(new Guid(Request.QueryString["LpoID"].ToString()));
                    MailType = "LPO"; RedirectAddr = "../Purchases/LPOStatus.aspx";
                }
                else if (Request.QueryString["LpoID_Customer"] != null && Request.QueryString["LpoID_Customer"].ToString() != "") // Customer Flow
                {
                    GetEmailDetails(new Guid(Request.QueryString["LpoID_Customer"].ToString()), "CLPO");
                    //lbtnAtchmnt.Text = GenerateLPOPDF(int.Parse(Request.QueryString["LpoID"].ToString()));
                    lbtnAtchmnt.Text = GenerateLPOPDF_Customer(new Guid(Request.QueryString["LpoID_Customer"].ToString()));
                    MailType = "CLPO"; RedirectAddr = "../Customer Access/Customer_LPO_Status.aspx";
                }
                else if (Request.QueryString["PrID"] != null && Request.QueryString["PrID"].ToString() != "")
                {

                    GetEmailDetails(new Guid(Request.QueryString["PrID"].ToString()), "RCED");
                    lbtnAtchmnt.Text = GenerateCT1PDF_New(new Guid(Request.QueryString["PrID"].ToString()));
                    MailType = "RCED"; RedirectAddr = "../Logistics/RqstCEDtlsStatus.aspx";
                }
                else if (Request.QueryString["DpItID"] != null && Request.QueryString["DpItID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["DpItID"].ToString()), "DPIT");
                    lbtnAtchmnt.Text = GenerateDspchInstPDF(new Guid(Request.QueryString["DpItID"].ToString()));
                    MailType = "DPIT"; RedirectAddr = "../Logistics/DspchInstnsStatus.aspx";
                }
                else if (Request.QueryString["IPID"] != null && Request.QueryString["IPID"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["IPID"].ToString()), "INPT");
                    //lbtnAtchmnt.Text = GenerateDspchInstPDF(int.Parse(Request.QueryString["PIID"].ToString()));
                    MailType = "INPT"; RedirectAddr = "../Purchases/RqstInsptnPlnStatus.aspx";
                }
                else if (Request.QueryString["SEVTM"] != null && Request.QueryString["SEVTM"].ToString() != "")
                {
                    GetEmailDetails(new Guid(Request.QueryString["SEVTM"].ToString()), "SEVTM");
                    //lbtnAtchmnt.Text = GenerateDspchInstPDF(int.Parse(Request.QueryString["PIID"].ToString()));
                    MailType = "SEVTM"; RedirectAddr = "../Purchases/RqstInsptnPlnStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get E-Mails Basisc Data
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="EMType"></param>
        protected void GetEmailDetails(Guid ID, string EMType)
        {
            try
            {
                DataSet MailDetails = null;
                switch (EMType)
                {
                    case "FE": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagVSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "PE": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagVSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                   "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "FFE": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagVSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "PI_FFE": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagISelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                "", CommonBLL.SuplrContactTypeText, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "LE": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagWCommonMstr, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", CommonBLL.SuplrContactTypeText, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "LQ": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagBSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "CLQ": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagBSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "FQ": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagXSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "CFQ": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagXSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "FPO": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagASelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "CFPO": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagASelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    CommonBLL.TraffickerContactTypeText, "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "LPO": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagYSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "CLPO": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagYSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "RCED": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagZSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "DPIT": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagESelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    case "INPT": MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagFSelect, ID, Guid.Empty, "", "", "", DateTime.Now,
                    "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                    default: MailDetails = EMDBL.SelectEMailDetails(CommonBLL.FlagWCommonMstr, ID, Guid.Empty, "", "", "", DateTime.Now,
                        "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())); break;
                }
                BindEmailDetails(MailDetails, EMType);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Filling All Input Detials Using DataSet
        /// </summary>
        /// <param name="Details"></param>
        protected void BindEmailDetails(DataSet Details, string EMType)
        {
            try
            {
                DataSet dss = new DataSet();

                string Message = ""; string EncryptUrl = "";
                ArrayList alist = new ArrayList();
                if (Details.Tables.Count > 0 && Details.Tables[0].Rows.Count > 0)
                {
                    //txtSender.Text = Details.Tables[0].Rows[0]["PriEmail"].ToString();
                    txtSender.Text = Session["UserMail"].ToString();
                    txtRcpts.Text = Details.Tables[0].Rows[0]["Email"].ToString() + ", " + Details.Tables[0].Rows[0]["SecondEmail"].ToString();
                    if (Details.Tables[0].Columns.Contains("EmailCC"))
                        txtCc.Text = "dnrao@voltaimpex.com,rahman@voltaimpex.com"; //Details.Tables[0].Rows[0]["EmailCC"].ToString();
                    // "avnrao@voltaimpex.com";
                    txtSbjct.Text = Details.Tables[0].Rows[0]["Subject"].ToString();
                    txtMsgBdy.Text = "";

                    //    "Dear Sir, " + System.Environment.NewLine + System.Environment.NewLine +
                    //"We have an urgent export requirement for the enquiry Attached. Please send your most "
                    //+ "competitive quotation for the same." + System.Environment.NewLine + System.Environment.NewLine
                    //+ System.Environment.NewLine +
                    //"Thanking you" + System.Environment.NewLine + System.Environment.NewLine +
                    //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                    //+ "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "RAJAPRAASADAMU BUILDING, Masjidbanda,"
                    //+ System.Environment.NewLine + "Kondapur, HYDERABAD-500 081" + System.Environment.NewLine + System.Environment.NewLine +
                    //"Tel: 91-40 23813280, 23810063" + System.Environment.NewLine + "Fax: 91-40 23715606"
                    //+ System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                    //+ System.Environment.NewLine +
                    //"Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();


                    if (EMType == "FE" || EMType == "FFE")//|| EMType == "FPO"
                    {
                        CustomerBLL cusmr = new CustomerBLL();
                        DataSet ds = cusmr.SelectCustomers(CommonBLL.FlagASelect, new Guid(Details.Tables[0].Rows[0]["CusmorId"].ToString()), new Guid(Session["CompanyID"].ToString()));
                        txtRcpts.Text = ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim() == "" ?
                            ds.Tables[0].Rows[0]["MNGRMAIL"].ToString().Trim() :
                            ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim();
                        txtCc.Text = "";
                        //txtCc.Text = ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim() != "" ?
                        //    ds.Tables[0].Rows[0]["MNGRMAIL"].ToString().Trim() : "";
                    }
                    if (EMType == "FFE")//
                    {
                        CustomerBLL cusmr = new CustomerBLL();
                        DataSet ds = cusmr.SelectCustomers(CommonBLL.FlagISelect, new Guid(Details.Tables[0].Rows[0]["VendorIds"].ToString()), new Guid(Session["CompanyID"].ToString()));
                        txtRcpts.Text = ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim() == "" ?
                            ds.Tables[0].Rows[0]["MNGRMAIL"].ToString().Trim() :
                            ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim();
                        txtCc.Text = "";
                        //txtCc.Text = ds.Tables[0].Rows[0]["ASSGINEDUSER"].ToString().Trim() != "" ?
                        //    ds.Tables[0].Rows[0]["MNGRMAIL"].ToString().Trim() : "";
                    }

                    if (EMType == "PI_FFE")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "Float_PIStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        string SupID = "", CustID = "", FenqID = "";
                        EncryptUrl = "";//CsID=1&FeqID=207&LeqID=293

                        if (Details.Tables.Count > 0 && Details.Tables[1].Rows.Count > 0 && Details.Tables[1].Rows[0]["LogInSupID"].ToString() != "")
                        {
                            SupID = Details.Tables[1].Rows[0]["LogInSupID"].ToString();
                            CustID = Details.Tables[1].Rows[0]["CusmorId"].ToString();
                            FenqID = Details.Tables[1].Rows[0]["ForeignEnquireId"].ToString();
                            string LEID = Details.Tables[1].Rows[0]["ForeignEnquireId"].ToString();
                            string LoginMailId = Details.Tables[1].Rows[0]["LogInMailID"].ToString();
                            string LoginPwd = Details.Tables[1].Rows[0]["LogInPwd"].ToString();
                            string CmpnyID = Session["CompanyID"].ToString();
                            string EncryptString = BAL.StringEncrpt_Decrypt.Encrypt(SupID + "&CsID=" + CustID + "&FeqID=" + FenqID + "&LeqID= " + LEID + "&LMID="
                                + LoginMailId + "&LPWD=" + LoginPwd + "&CmpnyID=" + CmpnyID, true);

                            string url = HttpContext.Current.Request.Url.Authority;
                            if (HttpContext.Current.Request.ApplicationPath.Length > 1)
                                url += HttpContext.Current.Request.ApplicationPath;

                            EncryptUrl += System.Environment.NewLine + System.Environment.NewLine
                                        + "Click on the below link to fill Quotation Details... "
                                        + System.Environment.NewLine + System.Environment.NewLine;//or Copy Paste on Browser address bar 

                            if (Details.Tables[1].Rows[0]["SupplierType"].ToString() == "FSupplier")
                                EncryptUrl += "http://" + url + "/Customer%20Access/NewFQ_floatedPI.aspx?SupID=" + EncryptString + "";
                            else if (Details.Tables[1].Rows[0]["SupplierType"].ToString() == "LSupplier")
                                EncryptUrl += "http://" + url + "/Customer%20Access/Customer_LocalQuotation.aspx?SupID=" + EncryptString + "";
                            else
                                //For Vendor If exixts
                                EncryptUrl += "";

                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            //if (Message.Contains("Enquiry No."))
                            // {
                            string FindString = "for the same.";
                            int index4 = Message.IndexOf(FindString);
                            Message = Message.Insert(index4 + FindString.Length, EncryptUrl);
                            //  Message = Message.Insert(EncryptUrl);
                            //int index3 = Message.IndexOf("Dt:");
                            //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            //  }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["ContactPerson"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString() +
                                    "," + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "," + dss.Tables[1].Rows[0]["State"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }
                        else if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("quotation for the same."))
                            {
                                int index2 = Message.IndexOf("quotation for the same.");
                                Message = Message.Insert(index2 + "quotation for the same.".Length, EncryptUrl);
                                //int index3 = Message.IndexOf("Dt:");
                                //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                System.Environment.NewLine + dss.Tables[1].Rows[0]["ContactPerson"].ToString() +
                                System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString() +
                                "," + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                "," + dss.Tables[1].Rows[0]["State"].ToString() +
                                System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                               System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message; //dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                    }

                    if (EMType == "LE")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "LEStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        string SupID = "", CustID = "", FenqID = "";
                        EncryptUrl = "";//CsID=1&FeqID=207&LeqID=293

                        if (Details.Tables.Count > 0 && Details.Tables[1].Rows.Count > 0 && Details.Tables[1].Rows[0]["LogInSupID"].ToString() != "")
                        {
                            //txtMsgBdy.Text = System.Environment.NewLine + "Sub : Kind Attn to " + Details.Tables[1].Rows[0]["SuplrCnctName"].ToString() + System.Environment.NewLine;

                            //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine
                            //    + "We have an urgent export requirement for the enquiry Attached. Please send your most "
                            //    + "competitive quotation for the same." + System.Environment.NewLine + System.Environment.NewLine
                            //    + "Please update the Quotation Details in the enclosed attachment and re-send the same or use below link to fill the Details."
                            //    + "Awaiting for your quickest response." + System.Environment.NewLine + System.Environment.NewLine;

                            SupID = Details.Tables[1].Rows[0]["LogInSupID"].ToString();
                            CustID = Details.Tables[1].Rows[0]["CusmorId"].ToString();
                            FenqID = Details.Tables[1].Rows[0]["ForeignEnquireId"].ToString();
                            string LEID = Details.Tables[1].Rows[0]["LocalEnquireId"].ToString();
                            string LoginMailId = Details.Tables[1].Rows[0]["LogInMailID"].ToString();
                            string LoginPwd = Details.Tables[1].Rows[0]["LogInPwd"].ToString();
                            string CmpnyID = Session["CompanyID"].ToString();
                            string EncryptString = BAL.StringEncrpt_Decrypt.Encrypt(SupID + "&CsID=" + CustID + "&FeqID=" + FenqID + "&LeqID= " + LEID + "&LMID="
                                + LoginMailId + "&LPWD=" + LoginPwd + "&CmpnyID=" + CmpnyID, true);

                            string url = HttpContext.Current.Request.Url.Authority;
                            if (HttpContext.Current.Request.ApplicationPath.Length > 1)
                                url += HttpContext.Current.Request.ApplicationPath;

                            EncryptUrl += System.Environment.NewLine + System.Environment.NewLine
                                        + "Click on the below link to fill Quotation Details... "
                                        + System.Environment.NewLine + System.Environment.NewLine;//or Copy Paste on Browser address bar 

                            EncryptUrl += "http://" + url + "/Quotations/NewLQuotation.aspx?SupID=" + EncryptString + "";


                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("for the same."))
                            {
                                int index2 = Message.IndexOf("for the same.");
                                Message = Message.Insert(index2 + "for the same.".Length, EncryptUrl);
                                //int index3 = Message.IndexOf("Dt:");
                                //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                string s = dss.Tables[1].Rows[0]["Address"].ToString();
                                string[] sa = s.Split(',');

                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString() +
                                    System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;

                            //txtMsgBdy.Text += System.Environment.NewLine + System.Environment.NewLine +
                            //"" + System.Environment.NewLine +
                            //"Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                            //"Yours faithfully," + System.Environment.NewLine +
                            //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                            //+ Details.Tables[0].Rows[0]["CompanyName"].ToString() + System.Environment.NewLine + Details.Tables[0].Rows[0]["Address1"].ToString()
                            //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address2"].ToString() + System.Environment.NewLine
                            //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address3"].ToString() + System.Environment.NewLine
                            //+ System.Environment.NewLine + "Tel: " + Details.Tables[0].Rows[0]["Phone"].ToString().TrimStart('/') + System.Environment.NewLine + "Fax: " +
                            //Details.Tables[0].Rows[0]["Fax"].ToString() + System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                            //+ System.Environment.NewLine +
                            //"Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                        }
                        else
                        {

                            if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                            {
                                Message = dss.Tables[0].Rows[0]["Message"].ToString();
                                if (Message.Contains("quotation for the same."))
                                {
                                    int index2 = Message.IndexOf("quotation for the same.");
                                    Message = Message.Insert(index2 + "quotation for the same.".Length, EncryptUrl);
                                    //int index3 = Message.IndexOf("Dt:");
                                    //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                                }
                                if (Message.Contains("Yours faithfully,"))
                                {
                                    //string s = dss.Tables[1].Rows[0]["Address"].ToString();
                                    //string[] sa = s.Split(',');

                                    int index2 = Message.IndexOf("Yours faithfully,");
                                    Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" //+ dss.Tables[1].Rows[0]["State"].ToString() 
                                     + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                        //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString() +
                                    System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString());
                                }
                                //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                                txtMsgBdy.Text = Message; //dss.Tables[0].Rows[0]["Message"].ToString();
                            }

                            //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                            //"We have an urgent export requirement for the enquiry Attached. Please send your most "
                            //+ "competitive quotation for the same." + System.Environment.NewLine + System.Environment.NewLine + EncryptUrl


                            //+ System.Environment.NewLine + System.Environment.NewLine +
                            //"" + System.Environment.NewLine +
                            //"Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                            //"Yours faithfully," + System.Environment.NewLine +
                            //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                            //+ Details.Tables[0].Rows[0]["CompanyName"].ToString() + System.Environment.NewLine + Details.Tables[0].Rows[0]["Address1"].ToString()
                            //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address2"].ToString() + System.Environment.NewLine
                            //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address3"].ToString() + System.Environment.NewLine
                            //+ System.Environment.NewLine +
                            //"Tel: " + Details.Tables[0].Rows[0]["Phone"].ToString().TrimStart('/') + System.Environment.NewLine + "Fax: " +
                            //Details.Tables[0].Rows[0]["Fax"].ToString() + System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                            //+ System.Environment.NewLine +
                            //"Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                        }

                    }
                    else if (EMType == "LQ")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "LQStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        DateTime EnquiryDT = Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["EnquireNumber"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, EnquiryDT.ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString()
                                     + System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString()); //dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            else if (Message.Contains("Regards,"))
                            {
                                int index2 = Message.IndexOf("Regards,");
                                Message = Message.Insert(index2 + "Regards,".Length,
                                    System.Environment.NewLine + "System Admin");
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine
                        //   + "SUB: Quotation is for your Enquiry " + System.Environment.NewLine + System.Environment.NewLine
                        //   + " We have prepared a quotation for your Enquiry No. " + Details.Tables[0].Rows[0]["EnquireNumber"].ToString() + ", Dt: "
                        //   + CommonBLL.DateDisplay(Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString())) +
                        //   ". Please find the Quotation in VOMS Application for the complete details and send your response. "
                        //   + System.Environment.NewLine + System.Environment.NewLine + "Regards, "
                        //   + System.Environment.NewLine + Session["UserName"].ToString();
                    }
                    else if (EMType == "CLQ")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "Customer_LocalQuotation_Status.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        DateTime EnquiryDT = Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["EnquireNumber"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, EnquiryDT.ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +//+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine
                        //   + "SUB: Quotation is for your Enquiry " + System.Environment.NewLine + System.Environment.NewLine
                        //   + " We have prepared a quotation for your Enquiry No. " + Details.Tables[0].Rows[0]["EnquireNumber"].ToString() + ", Dt: "
                        //   + CommonBLL.DateDisplay(Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString())) +
                        //   ". Please find the Quotation in VOMS Application for the complete details and send your response. "
                        //   + System.Environment.NewLine + System.Environment.NewLine + "Regards, "
                        //   + System.Environment.NewLine + Session["UserName"].ToString();
                    }
                    else if (EMType == "FE")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FEStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                            && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["EnquireNumber"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +  //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString());//+ dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();    
                        }
                        //    "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine
                        //    + "SUB: Quotation Required for Enquiry " + System.Environment.NewLine + System.Environment.NewLine
                        //    + "We have an urgent requirement for Enquiry No. " + Details.Tables[0].Rows[0]["EnquireNumber"].ToString()
                        //    + ", Dt: " + Details.Tables[0].Rows[0]["EnquiryDate"].ToString() + " . " + System.Environment.NewLine
                        //    + "Please find the Enquiry in VOMS Application for the complete details and send your most competitive quotation for the same."
                        //    + System.Environment.NewLine + System.Environment.NewLine

                        //    + System.Environment.NewLine + System.Environment.NewLine + "Regards, " +
                        //System.Environment.NewLine + Session["UserName"].ToString();
                    }
                    else if (EMType == "PE")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "PIStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                            && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["EnquireNumber"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    // System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();    
                        }
                    }
                    else if (EMType == "FFE")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FloatedEnquiryStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["EnquireNumber"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    // System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine
                        //    + "SUB: Quotation Required for Enquiry " + System.Environment.NewLine + System.Environment.NewLine
                        //    + "We have an urgent requirement for Enquiry No. " + Details.Tables[0].Rows[0]["EnquireNumber"].ToString()
                        //    + "Dt: " + Details.Tables[0].Rows[0]["EnquiryDate"].ToString() + " . " + System.Environment.NewLine
                        //    + "Please find the Enquiry in VOMS Application for the complete details and send your most competitive quotation for the same."
                        //    + System.Environment.NewLine + System.Environment.NewLine

                        //    + System.Environment.NewLine + System.Environment.NewLine + "Regards, " +
                        //System.Environment.NewLine + Session["UserName"].ToString();
                    }
                    else if (EMType == "FQ")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FQStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["FEnqNo"].ToString());
                                int index3 = Message.IndexOf("Dated:");
                                Message = Message.Insert(index3 + "Dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("quotation No"))
                            {
                                int index2 = Message.IndexOf("quotation No: ");
                                Message = Message.Insert(index2 + "quotation No: ".Length, Details.Tables[0].Rows[0]["Quotationnumber"].ToString());
                                int index3 = Message.IndexOf("Quotation Dated:");
                                Message = Message.Insert(index3 + "Quotation Dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["QuotationDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString()
                                    + System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString());

                                //+ dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //"With reference to your valuable Enquiry No. " + Details.Tables[0].Rows[0]["FEnqNo"].ToString()
                        //+ " Date " + CommonBLL.DateDisplay(Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString()))
                        //+ System.Environment.NewLine +
                        //"We are pleased to submit our quotation. Kindly release the FPO to execute the order immediately."
                        //+ System.Environment.NewLine + System.Environment.NewLine +
                        //"Thanking you" + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "RAJAPRAASADAMU BUILDING, Masjidbanda,"
                        //+ System.Environment.NewLine + "Kondapur, HYDERABAD-500 081" + System.Environment.NewLine
                        //+ System.Environment.NewLine + "Tel: 91-40 23813280, 23810063" + System.Environment.NewLine
                        //+ "Fax: 91-40 23715606" + System.Environment.NewLine + "Email: "
                        //+ Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //+ System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    else if (EMType == "CFQ")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FQStatusCustomer.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Enquiry No."))
                            {
                                int index2 = Message.IndexOf("Enquiry No.");
                                Message = Message.Insert(index2 + "Enquiry No.".Length, Details.Tables[0].Rows[0]["FEnqNo"].ToString());
                                int index3 = Message.IndexOf("Dated:");
                                Message = Message.Insert(index3 + "Dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("quotation No"))
                            {
                                int index2 = Message.IndexOf("quotation No: ");
                                Message = Message.Insert(index2 + "quotation No: ".Length, Details.Tables[0].Rows[0]["Quotationnumber"].ToString());
                                int index3 = Message.IndexOf("Quotation Dated:");
                                Message = Message.Insert(index3 + "Quotation Dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["QuotationDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["ContactPerson"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //"With reference to your valuable Enquiry No. " + Details.Tables[0].Rows[0]["FEnqNo"].ToString()
                        //+ " Date " + CommonBLL.DateDisplay(Convert.ToDateTime(Details.Tables[0].Rows[0]["EnquiryDate"].ToString()))
                        //+ System.Environment.NewLine +
                        //"We are pleased to submit our quotation. Kindly release the FPO to execute the order immediately."
                        //+ System.Environment.NewLine + System.Environment.NewLine +
                        //"Thanking you" + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "RAJAPRAASADAMU BUILDING, Masjidbanda,"
                        //+ System.Environment.NewLine + "Kondapur, HYDERABAD-500 081" + System.Environment.NewLine
                        //+ System.Environment.NewLine + "Tel: 91-40 23813280, 23810063" + System.Environment.NewLine
                        //+ "Fax: 91-40 23715606" + System.Environment.NewLine + "Email: "
                        //+ Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //+ System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    else if (EMType == "FPO")
                    {
                        txtRcpts.Text = "dnrao@voltaimpex.com";
                        txtCc.Text = "";
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FPOStatus.aspx", ""
                           , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Purchase Order:"))
                            {
                                int index2 = Message.IndexOf("Purchase Order:");
                                Message = Message.Insert(index2 + "Purchase Order:".Length, Details.Tables[0].Rows[0]["ForeignPurchaseOrderNo"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["FPODate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //+ dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +//+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    // System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString() +
                                    System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString());  //+ dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                        //    txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //    "We are pleased to release our Purchase Order " + Details.Tables[0].Rows[0]["ForeignPurchaseOrderNo"].ToString()
                        //    + " Dt: " + Details.Tables[0].Rows[0]["FPODate"].ToString() +
                        //" for our requirement. Please find the order in VOMS Application for complete" +
                        //" details and expedite to deliver the material as per delivery time mentioned." +
                        //System.Environment.NewLine + " Please confirm the receipt of the order and ensure packing is in good condition. "
                        //    + System.Environment.NewLine + System.Environment.NewLine +
                        //    "Thanking you" + System.Environment.NewLine + System.Environment.NewLine +
                        //    "Yours faithfully," + System.Environment.NewLine +
                        //    Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //    + "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "RAJAPRAASADAMU BUILDING, Masjidbanda,"
                        //    + System.Environment.NewLine + "Kondapur, HYDERABAD-500 081" + System.Environment.NewLine
                        //    + System.Environment.NewLine + "Tel: 91-40 23813280, 23810063" + System.Environment.NewLine
                        //    + "Fax: 91-40 23715606" + System.Environment.NewLine + "Email: "
                        //    + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //    + System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    else if (EMType == "CFPO")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "FPOVendorStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("Purchase Order:"))
                            {
                                int index2 = Message.IndexOf("Purchase Order:");
                                Message = Message.Insert(index2 + "Purchase Order:".Length, Details.Tables[0].Rows[0]["ForeignPurchaseOrderNo"].ToString());
                                int index3 = Message.IndexOf("Dt:");
                                Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["FPODate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["ContactPerson"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    // System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;//dss.Tables[0].Rows[0]["Message"].ToString();
                        }
                    }
                    else if (EMType == "LPO")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "LPOStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("P.O. No. :"))
                            {
                                int index2 = Message.IndexOf("P.O. No. :");
                                Message = Message.Insert(index2 + "P.O. No. :".Length, Details.Tables[0].Rows[0]["LocalPurchaseOrderNo"].ToString());
                                int index3 = Message.IndexOf("Dt. :");
                                Message = Message.Insert(index3 + "Dt. :".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["LocalPurchaseOrderDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("quotation No: "))
                            {
                                int index2 = Message.IndexOf("quotation No: ");
                                Message = Message.Insert(index2 + "quotation No: ".Length, Details.Tables[0].Rows[0]["LqNum"].ToString());
                                int index3 = Message.IndexOf("dated:");
                                Message = Message.Insert(index3 + "dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["LqDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //+ dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString() +
                                    System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString());//+ dss.Tables[1].Rows[0]["PrimaryEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //    "Sub.: P.O. No. : " + Details.Tables[0].Rows[0]["LocalPurchaseOrderNo"].ToString() + ", Dt. :" +
                        //    CommonBLL.DateDisplay(Convert.ToDateTime(Details.Tables[0].Rows[0]["LocalPurchaseOrderDate"].ToString()))
                        //    + System.Environment.NewLine +
                        //"We are pleased to release our Purchase Order for our Export requirement." +
                        //"Please find the enclosed order documents for complete details and expedite to " +
                        //"deliver the material as per delivery time mentioned."
                        //+ System.Environment.NewLine +
                        //"Please confirm the receipt of the order and ensure packing is in good condition" +
                        //System.Environment.NewLine + System.Environment.NewLine +
                        //"Thanking you" + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "RAJAPRAASADAMU BUILDING, Masjidbanda,"
                        //+ System.Environment.NewLine + "Kondapur, HYDERABAD-500 081" + System.Environment.NewLine
                        //+ System.Environment.NewLine + "Tel: 91-40 23813280, 23810063" + System.Environment.NewLine
                        //+ "Fax: 91-40 23715606" + System.Environment.NewLine + "Email: "
                        //+ Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //+ System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    else if (EMType == "CLPO")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "Customer_LPO_Status.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("P.O. No. :"))
                            {
                                int index2 = Message.IndexOf("P.O. No. :");
                                Message = Message.Insert(index2 + "P.O. No. :".Length, Details.Tables[0].Rows[0]["LocalPurchaseOrderNo"].ToString());
                                int index3 = Message.IndexOf("Dt. :");
                                Message = Message.Insert(index3 + "Dt. :".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["LocalPurchaseOrderDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("quotation No: "))
                            {
                                int index2 = Message.IndexOf("quotation No: ");
                                Message = Message.Insert(index2 + "quotation No: ".Length, Details.Tables[0].Rows[0]["LqNum"].ToString());
                                int index3 = Message.IndexOf("dated:");
                                Message = Message.Insert(index3 + "dated:".Length, Convert.ToDateTime(Details.Tables[0].Rows[0]["LqDate"].ToString()).ToString("dd-MM-yyyy"));
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["ContactPerson"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +//+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }
                    }
                    else if (EMType == "RCED")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "RqstCEDtls.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        string lponumbers = string.Empty;
                        if (Details.Tables[0].Rows[0]["LPOs"].ToString().Split(',').Count() > 1)
                        {
                            int i = 1;
                            foreach (string s in Details.Tables[0].Rows[0]["LPOs"].ToString().Split(','))
                            {
                                lponumbers += i + ") " + s.Trim() + System.Environment.NewLine + "     ";
                                i++;
                            }
                        }
                        else
                        {
                            lponumbers = "1) " + Details.Tables[0].Rows[0]["LPOs"].ToString() + System.Environment.NewLine + "     ";
                        }

                        string SupID = "";
                        string FPOIDs = Details.Tables[0].Rows[0]["FPOIDs"].ToString();
                        string LPOIDs = Details.Tables[0].Rows[0]["LPOIDs"].ToString();

                        EncryptUrl = "";

                        if (Details.Tables.Count > 0 && Details.Tables[1].Rows.Count > 0 && Details.Tables[1].Rows[0]["LogInSupID"].ToString() != "")
                        {
                            SupID = Details.Tables[1].Rows[0]["LogInSupID"].ToString();
                            string PinvReqID = Details.Tables[1].Rows[0]["PInvReqID"].ToString();
                            string LoginMailId = Details.Tables[1].Rows[0]["LogInMailID"].ToString();
                            string LoginPwd = Details.Tables[1].Rows[0]["LogInPwd"].ToString();

                            string EncryptString = BAL.StringEncrpt_Decrypt.Encrypt(SupID + "&PinvReqID=" + PinvReqID + "&LMID=" + LoginMailId + "&LPWD=" +
                                LoginPwd + "&CmpnyID=" + Session["CompanyID"].ToString(), true);
                            string url = HttpContext.Current.Request.Url.Authority;
                            if (HttpContext.Current.Request.ApplicationPath.Length > 1)
                                url += HttpContext.Current.Request.ApplicationPath;

                            EncryptUrl += System.Environment.NewLine + System.Environment.NewLine + "Click on the below link to fill CT-1 Details... "
                                        + System.Environment.NewLine + System.Environment.NewLine;

                            EncryptUrl += "http://" + url + "/Logistics/CTOneGeneration.aspx?SupID=" + EncryptString + "";
                        }

                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("order number(s)"))
                            {
                                int index2 = Message.IndexOf("order number(s)");
                                Message = Message.Insert(index2 + "order number(s)".Length, lponumbers);
                                int index3 = Message.IndexOf("quickest response.");
                                Message = Message.Insert(index3 + "quickest response.".Length, EncryptUrl);
                            }
                            if (Message.Contains("Reminder"))
                            {
                                int index2 = Message.IndexOf("Reminder");
                                Message = Message.Insert(index2 + "Reminder".Length, System.Environment.NewLine + EncryptUrl);
                            }

                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                     System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                     System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                     "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                     System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                     "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +//+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                    System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                     "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }

                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //"With reference to our order number(s) " + System.Environment.NewLine +
                        //"" + lponumbers + System.Environment.NewLine +
                        //"We request you to issue a Proforma Invoice which is duly Signed & Stamped."
                        //+ System.Environment.NewLine +
                        //"Please update the Excise Details in the enclosed attachment without fail, and re-send the same."
                        //+ System.Environment.NewLine +
                        //"The above said documents are urgently required to process for CT-1 Bond and ARE-1 Forms."
                        //+ System.Environment.NewLine +
                        //"Awaiting for your quickest response."

                        //+ EncryptUrl

                        //+ System.Environment.NewLine + System.Environment.NewLine +
                        //"" + System.Environment.NewLine +
                        //"Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ Details.Tables[0].Rows[0]["CompanyName"].ToString() + System.Environment.NewLine + Details.Tables[0].Rows[0]["Address1"].ToString()
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address2"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address3"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine +
                        //"Tel: " + Details.Tables[0].Rows[0]["Phone"].ToString().TrimStart('/') + System.Environment.NewLine + "Fax: " +
                        //Details.Tables[0].Rows[0]["Fax"].ToString() + System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //+ System.Environment.NewLine +
                        //"Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    else if (EMType == "DPIT")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "DspchInstnsStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        string lponumbers = string.Empty;
                        if (Details.Tables[0].Rows[0]["LPOs"].ToString().Split(',').Count() > 1)
                        {
                            int i = 1;
                            foreach (string s in Details.Tables[0].Rows[0]["LPOs"].ToString().Split(','))
                            {
                                lponumbers += i + ") " + s.Trim() + System.Environment.NewLine + "     ";
                                i++;
                            }
                        }
                        else
                        {
                            lponumbers = System.Environment.NewLine + "1) " + Details.Tables[0].Rows[0]["LPOs"].ToString() + System.Environment.NewLine + "     ";
                        }

                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("LPO Number(s)"))
                            {
                                int index2 = Message.IndexOf("LPO Number(s)");
                                Message = Message.Insert(index2 + "LPO Number(s)".Length, lponumbers);
                                //int index3 = Message.IndexOf("Dt:");
                                //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                string Add = string.Empty;
                                if (dss.Tables[1].Rows[0]["Address"].ToString() != "" && dss.Tables[1].Rows[0]["Address"].ToString().Split(',').Count() > 1)
                                {
                                    int g = 1;
                                    foreach (string s in dss.Tables[1].Rows[0]["Address"].ToString().Split(','))
                                    {
                                        Add += s.Trim() + System.Environment.NewLine;
                                        g++;
                                    }
                                }
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + Add + //dss.Tables[1].Rows[0]["Address"].ToString() +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +//+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString() + System.Environment.NewLine +
                                     "Contact No.:" + Details.Tables[0].Rows[0]["MobileNo"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }

                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //"With reference to the LPO Number(s) " + System.Environment.NewLine +
                        //"" + lponumbers + System.Environment.NewLine +
                        //"We are here by sending you Dispatch Instructions. Please find the enclosed attachment " +
                        //"and revert with confirmation of receipt immediately." + System.Environment.NewLine +
                        //"We need the following set of documents soon after the dispatch." + System.Environment.NewLine +
                        //"Submission of Documents : A set of following documents shall be submitted to our Hyderabad office for " +
                        //"arranging payment." + System.Environment.NewLine +
                        //"1) Commercial Invoice." + System.Environment.NewLine +
                        //"2) Copy of L.R." + System.Environment.NewLine +
                        //"3) Packing list including no. of packages, description and quantity of goods in " +
                        //"each pack, weight and size of boxes/packages."
                        //+ System.Environment.NewLine
                        //+ System.Environment.NewLine +
                        //"Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ Details.Tables[0].Rows[0]["CompanyName"].ToString() + System.Environment.NewLine + Details.Tables[0].Rows[0]["Address1"].ToString()
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address2"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address3"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine +
                        //"Tel: " + Details.Tables[0].Rows[0]["Phone"].ToString().TrimStart('/') + System.Environment.NewLine + "Fax: " +
                        //Details.Tables[0].Rows[0]["Fax"].ToString() + System.Environment.NewLine + "Email: "
                        //+ Details.Tables[0].Rows[0]["PriEmail"].ToString() + System.Environment.NewLine +
                        //"Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();

                    }
                    else if (EMType == "INPT")
                    {
                        dss = EBLL.GetEmBdyDetails(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty, "RqstInsptnPlnStatus.aspx", ""
                            , new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
                        string lponumbers = string.Empty;
                        if (Details.Tables[0].Rows[0]["LPOs"].ToString().Split(',').Count() > 1)
                        {
                            int i = 1;
                            foreach (string s in Details.Tables[0].Rows[0]["LPOs"].ToString().Split(','))
                            {
                                lponumbers += i + ") " + s.Trim() + System.Environment.NewLine + "     ";
                                i++;
                            }
                        }
                        else
                        {
                            lponumbers = "1) " + Details.Tables[0].Rows[0]["LPOs"].ToString() + System.Environment.NewLine + "     ";
                        }
                        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0
                           && dss.Tables[0].Rows[0]["Message"].ToString() != "")
                        {
                            Message = dss.Tables[0].Rows[0]["Message"].ToString();
                            if (Message.Contains("order number(s)"))
                            {
                                int index2 = Message.IndexOf("order number(s)");
                                Message = Message.Insert(index2 + "order number(s) ".Length, System.Environment.NewLine + System.Environment.NewLine + lponumbers);
                                //int index3 = Message.IndexOf("Dt:");
                                //Message = Message.Insert(index3 + "Dt:".Length, Details.Tables[0].Rows[0]["EnquiryDate"].ToString());
                            }
                            if (Message.Contains("Yours faithfully,"))
                            {
                                int index2 = Message.IndexOf("Yours faithfully,");
                                Message = Message.Insert(index2 + "Yours faithfully,".Length,
                                    System.Environment.NewLine + Details.Tables[0].Rows[0]["UserName"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["Address"].ToString().Replace(", ", ",\r") +
                                    "," + //dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() +
                                    System.Environment.NewLine + dss.Tables[1].Rows[0]["City"].ToString() +
                                    "-" + dss.Tables[1].Rows[0]["PostBoxNumber"].ToString() + //+ dss.Tables[1].Rows[0]["State"].ToString() +
                                    //System.Environment.NewLine + dss.Tables[1].Rows[0]["Country"].ToString() +
                                   System.Environment.NewLine + "Tele/Fax :" + dss.Tables[1].Rows[0]["PhoneNumber"].ToString() + System.Environment.NewLine +
                                    "Email :" + Details.Tables[0].Rows[0]["PriEMail"].ToString());
                            }
                            //txtSbjct.Text = dss.Tables[0].Rows[0]["Subject"].ToString();
                            txtMsgBdy.Text = Message;
                        }
                        //txtMsgBdy.Text = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                        //"With reference to our order number(s) " + System.Environment.NewLine +
                        //"" + lponumbers + System.Environment.NewLine +
                        //"Let us have the status on readiness of the ordered material/equipment to make us to plan for Inspection Schedule."
                        //+ System.Environment.NewLine +
                        //"Awaiting for your quickest response."
                        //+ System.Environment.NewLine + System.Environment.NewLine +
                        //"" + System.Environment.NewLine +
                        //"Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                        //"Yours faithfully," + System.Environment.NewLine +
                        //Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                        //+ Details.Tables[0].Rows[0]["CompanyName"].ToString() + System.Environment.NewLine + Details.Tables[0].Rows[0]["Address1"].ToString()
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address2"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine + Details.Tables[0].Rows[0]["Address3"].ToString() + System.Environment.NewLine
                        //+ System.Environment.NewLine + "Tel: " + Details.Tables[0].Rows[0]["Phone"].ToString().TrimStart('/') + System.Environment.NewLine + "Fax: " +
                        //Details.Tables[0].Rows[0]["Fax"].ToString() + System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                        //+ System.Environment.NewLine + "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();
                    }
                    if (Details.Tables[0].Columns.Contains("Attachments"))
                    {
                        if (Details.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        {
                            string[] attachs = Details.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
                            foreach (string s in attachs)
                                alist.Add(s);
                            Session["uploads"] = alist;
                            divListBox.InnerHtml = AttachedFiles();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Upload Files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileUploadComplete(object sender, EventArgs e)
        {
            try
            {
                if (AsyncFileUpload1.HasFile)
                {
                    ArrayList alist = new ArrayList();
                    string strPath = MapPath("~/uploads/") + Path.GetFileName(AsyncFileUpload1.FileName);
                    string FileNames = AsyncFileUpload1.FileName;
                    if (Session["uploads"] != null)
                    {
                        alist = (ArrayList)Session["uploads"];
                        if (!alist.Contains(FileNames))
                            alist.Add(FileNames);
                    }
                    else if (Session["uploads"] == null)
                    {
                        alist.Add(FileNames);
                    }
                    Session["uploads"] = alist;
                    AsyncFileUpload1.SaveAs(strPath);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["uploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["uploads"];
                    StringBuilder sb = new StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        sb.Append("</select>");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
                return ex.Message;
            }
        }

        private void DownloadFile(string fname, bool forceDownload)
        {
            try
            {
                FileInfo filenme = new FileInfo(fname);
                string name = Path.GetFileName(fname);
                string ext = Path.GetExtension(fname);
                string type = "";
                if (ext != null)
                {
                    if (ext.ToLower() == ".pdf")
                    {
                        type = "Application/PDF";
                    }
                }
                if (forceDownload)
                {
                    Response.AddHeader("content-disposition", "attachment; filename=" + name);
                }
                if (type != "")
                    Response.ContentType = type;
                Response.WriteFile(filenme.FullName);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (ThreadAbortException er)
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        private void CloseReports(ReportDocument reportDocument)
        {
            Sections sections = reportDocument.ReportDefinition.Sections;
            foreach (Section section in sections)
            {
                ReportObjects reportObjects = section.ReportObjects;
                foreach (ReportObject reportObject in reportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                        subReportDocument.Close();
                    }
                }
            }
            reportDocument.Close();
        }

        /// <summary>
        /// Clear All Inputs
        /// </summary>
        private void ClearAll()
        {
            try
            {
                txtSender.Text = txtPwd.Text = txtRcpts.Text = txtSbjct.Text = txtCc.Text = txtMsgBdy.Text =
                    txtSuplrCncts.Text = lbtnAtchmnt.Text = MailType = "";
                Session.Remove("uploads"); Session.Remove("filename");
                divListBox.InnerHtml = "";
                //AsyncFileUpload1.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        ///// <summary>
        ///// Sending E-Mail With Attachments
        ///// </summary>
        ///// <param name="FrmAddr"></param>
        ///// <param name="Pwd"></param>
        ///// <param name="Toaddr"></param>
        ///// <param name="Ccaddr"></param>
        ///// <param name="BccAddr"></param>
        ///// <param name="Subject"></param>
        ///// <param name="Body"></param>
        ///// <param name="atchmnt"></param>
        ///// <returns></returns>
        //protected string SendMails(string FrmAddr, string Pwd, string[] Toaddr, string[] Ccaddr, string BccAddr, string Subject, 
        //    string Body, string[] atchmnt)
        //{
        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        foreach (string s in Toaddr)
        //            if (s.Trim() != "")
        //                mail.To.Add(s.Trim());
        //        foreach (string s in Ccaddr)
        //            if (s.Trim() != "")
        //                mail.CC.Add(s.Trim());
        //        foreach (string s in Toaddr)
        //            if (s.Trim() != "")
        //                mail.Bcc.Add(s.Trim());
        //        mail.From = new MailAddress(FrmAddr.Trim());
        //        mail.Subject = Subject;
        //        mail.Body = Body;
        //        foreach (string s in atchmnt)
        //            if (s != "")
        //                mail.Attachments.Add(new Attachment(s.Trim()));
        //        mail.IsBodyHtml = false;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "mail.voltaimpex.com"; //Or Your SMTP Server Address
        //        smtp.Port = 25; // SMTP Port Number
        //        mail.Priority = MailPriority.Normal;
        //        smtp.EnableSsl = false; //ValidateServerCertificate();
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.Credentials = new System.Net.NetworkCredential(FrmAddr.Trim(), Pwd.Trim());

        //        smtp.Send(mail);
        //        return "Sent";
        //    }
        //    catch (Exception ex)
        //    {
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
        //        return ex.Message;
        //    }
        //}

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save/Send Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                string[] Rcpts = txtRcpts.Text.Trim(',').Split(',');
                string[] Cc = txtCc.Text.Trim(',').Split(',');
                ArrayList al = new ArrayList();
                string[] Attachments = new string[0];
                if (Session["uploads"] != null)
                {
                    al = (ArrayList)Session["uploads"];
                    Attachments = new string[al.Count + 1];
                }
                else
                    Attachments = new string[1];
                Attachments[0] = (lbtnAtchmnt.Text != "" ? lbtnAtchmnt.Text.Trim() : "");
                for (int i = 0; i < al.Count; i++)
                    Attachments[i + 1] = MapPath("~/uploads/") + Path.GetFileName(al[i].ToString());
                string Rslt = CommonBLL.SendMails(txtSender.Text.Trim(), txtPwd.Text.Trim(), Rcpts, Cc, "",
                    txtSbjct.Text.Trim(), txtMsgBdy.Text.Trim(), Attachments);
                if (Rslt == "Sent")
                {
                    res = EMDBL.InsertUpdateDeleteEMailDetails(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(Session["UserID"].ToString()), txtSender.Text.Trim(),
                        txtRcpts.Text.Trim(), txtCc.Text.Trim(), "", txtSbjct.Text.Trim(), txtMsgBdy.Text.Trim(), DateTime.Now,
                        MailType, Attachments[0].ToString(), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (MailType == "LQ")
                    {
                        res = NLQBL.LclQuoteInsertUpdate(CommonBLL.FlagCSelect, new Guid(Request.QueryString["LQID"].ToString()), Guid.Empty, Guid.Empty,
                            Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, DateTime.Now, "", 0, 0,0,0,0,0, 0, 0, 0, 0, 0, 0, 0, 0, Guid.Empty, "",
                            DateTime.Now, 0, Guid.Empty, 0, "", "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLocalQuotation(),
                            CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), CommonBLL.EmptyDtLQ_SubItems(), new Guid(Session["CompanyID"].ToString()));
                    }
                    else if (MailType == "CLQ")
                    {
                        res = NLQBL.LclQuoteInsertUpdate(CommonBLL.FlagCSelect, new Guid(Request.QueryString["CLQID"].ToString()), Guid.Empty, Guid.Empty,
                            Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, DateTime.Now, "", 0, 0,0,0,0,0, 0, 0, 0, 0, 0, 0, 0, 0, Guid.Empty, "",
                            DateTime.Now, 0, Guid.Empty, 0, "", "", new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLocalQuotation(),
                            CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), CommonBLL.EmptyDtLQ_SubItems(), new Guid(Session["CompanyID"].ToString()));
                    }
                    if (res == 0)
                    {
                        ALS.AuditLog(res, "Save", "", "Email Send", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Session.Remove("uploads");
                        ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID,
                            "alert('Mail Sent Successfully');window.location='" + RedirectAddr + "';", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Email Send", "Mail Sent And Details Inserted Successfully");
                        ClearAll();
                    }
                    else
                    {
                        ALS.AuditLog(res, "Save", "", "Email Send", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Mail Sent But Details not Inserted');", true);
                        Session.Remove("uploads");
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Email Send", "Mail Sent But Details not Inserted");
                        ClearAll();
                    }
                    if (Session["uploads"] != null)
                    {
                        Session.Remove("uploads");
                    }

                }
                else
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Email Send", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send Error ", Rslt);
                    if (Rslt.Contains("Could not find file"))
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID, "alert('Mail Not Sent Cause Attachement Could not found');", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID, "alert('Mail Not Sent Please Check your input Detials');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Download File for Display/View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbtnAtchmnt_Click(object sender, EventArgs e)
        {
            DownloadFile(lbtnAtchmnt.Text, true);
        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            try
            {
                ArrayList all = new ArrayList();
                all = (ArrayList)Session["uploads"];
                StringBuilder sb = new StringBuilder();
                sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                for (int k = 0; k < all.Count; k++)
                    sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                sb.Append("</select>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["uploads"];
                if (all.Count > 0)
                    all.RemoveAt(ID);
                StringBuilder sb = new StringBuilder();
                if (all.Count > 0)
                {
                    sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                    for (int k = 0; k < all.Count; k++)
                        sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                    sb.Append("</select>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }



        #endregion

        #region Generate RDLC Report

        protected string GenerateFePDF(Guid ID, Guid CompanyId)
        {
            try
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FEDetails.rdlc");

                DataSet dataset = new DataSet();
                dataset = CRPTBLL.GetFenqDetails_Items(ID, CompanyId);
                List<ReportParameter> PhoneParameter = new List<ReportParameter>();
                //+ VbCrLf +

                PhoneParameter.Add(new ReportParameter("Parm_Deptname", dataset.Tables[0].Rows[0]["DeptName"].ToString()));//DeptName
                PhoneParameter.Add(new ReportParameter("Parm_EnqNumber", dataset.Tables[0].Rows[0]["EnquireNumber"].ToString()
                    + '_' + (Convert.ToDateTime(dataset.Tables[0].Rows[0]["EnquiryDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("Parm_Subject", dataset.Tables[0].Rows[0]["Subject"].ToString()));
                PhoneParameter.Add(new ReportParameter("Parm_ReceivedDate",
                    (Convert.ToDateTime(dataset.Tables[0].Rows[0]["ReceivedDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("Parm_DueDate",
                    (Convert.ToDateTime(dataset.Tables[0].Rows[0]["DueDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("Parm_Instructions", dataset.Tables[0].Rows[0]["Instruction"].ToString()));
                PhoneParameter.Add(new ReportParameter("Parm_Address", dataset.Tables[0].Rows[0]["CustAdd"].ToString()));
                PhoneParameter.Add(new ReportParameter("Parm_OrgAndAdd", dataset.Tables[0].Rows[0]["ORG"].ToString()));
                PhoneParameter.Add(new ReportParameter("Parm_CityAndCountry", dataset.Tables[0].Rows[0]["StateCountry"].ToString()));
                PhoneParameter.Add(new ReportParameter("Param_Instructions", dataset.Tables[0].Rows[0]["instruction"].ToString()));
                PhoneParameter.Add(new ReportParameter("Param_UsrDesignation", dataset.Tables[0].Rows[0]["UsrDesignation"].ToString()));
                PhoneParameter.Add(new ReportParameter("Param_UsrName", dataset.Tables[0].Rows[0]["UsrName"].ToString()));
                PhoneParameter.Add(new ReportParameter("Param_NoofPages", (ReportViewer1.ServerReport.GetTotalPages() + 1).ToString()));

                ReportViewer1.LocalReport.SetParameters(PhoneParameter);
                //vomserpdbDataSet ds=new vomserpdbDataSet ();
                //this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SP_FERPTAllItemDeatails", ds.Tables[0]));


                ReportDataSource dsItemDetails = new ReportDataSource();
                // dsEnumMasterDetails.Name = "VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter";
                dsItemDetails.Name = "vomserpdbDataSet_SP_FERPTAllItemDeatails";
                //VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter
                DataSet dsItems = new DataSet();
                dsItems = CRPTBLL.GetFenqAllDetails_Items(ID);
                dsItemDetails.Value = dsItems.Tables[0];

                localReport.DataSources.Add(dsItemDetails);

                this.ReportViewer1.LocalReport.Refresh();

                //this.ReportViewer1.LocalReport.Refresh(); //PostBackUrl="~/Quotations/NewLQuotation.aspx"

                string filename = MapPath("~/uploads/") + ID + "FEnq.pdf";
                CreatePDF(filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Local Enquiry Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// Local Enquiry RDLC Report Generation
        /// </summary>
        /// <param name="ID"></param>
        protected string GenerateLEPDF(Guid ID)
        {
            try
            {

                ReportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LEDetails.rdlc");
                LEnquiryBLL LEBLL = new LEnquiryBLL();
                DataSet dataset = new DataSet();
                ReportViewer1.LocalReport.EnableExternalImages = true;
                dataset = LEBLL.SelctLocalEnquiries(CommonBLL.FlagPSelectAll, ID, Guid.Empty, Guid.Empty, "", "", Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now,
                                Guid.Empty, Guid.Empty, 0, "", "", "", Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal());
                List<ReportParameter> PhoneParameter = new List<ReportParameter>();

                PhoneParameter.Add(new ReportParameter("RPTa_From", dataset.Tables[0].Rows[0]["Froms"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTg_RefNoAndDate",
                    dataset.Tables[0].Rows[0]["EnquireNumber"].ToString() + " _ " + dataset.Tables[0].Rows[0]["CreatedDate"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTc_Telephone", dataset.Tables[0].Rows[0]["Phone"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTd_Fax", dataset.Tables[0].Rows[0]["Fax1"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTe_Email", dataset.Tables[0].Rows[0]["PriEmail"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTf_Subject", dataset.Tables[0].Rows[0]["Subject"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTb_To", dataset.Tables[0].Rows[0]["SupName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTj_ProjectOrDeptName", dataset.Tables[0].Rows[0]["DeptName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTh_Date", dataset.Tables[0].Rows[0]["CreatedDate"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTi_DueDate", dataset.Tables[0].Rows[0]["DueDate"].ToString()));

                PhoneParameter.Add(new ReportParameter("RPTk_CompanyName", dataset.Tables[1].Rows[0]["CompanyName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTl_Add1", dataset.Tables[1].Rows[0]["Address1"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTj_Add2", dataset.Tables[1].Rows[0]["Address2"].ToString()));
                //PhoneParameter.Add(new ReportParameter("RPTk_Add3", dataset.Tables[1].Rows[0]["Address3"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTl_City", dataset.Tables[1].Rows[0]["City"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTm_State", dataset.Tables[1].Rows[0]["State"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTn_Country", dataset.Tables[1].Rows[0]["Country"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTo_Telephone", dataset.Tables[1].Rows[0]["Telephone"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTp_Fax", "Fax: " + dataset.Tables[1].Rows[0]["Fax"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTq_Pmail", "EMail: " + dataset.Tables[0].Rows[0]["UEmal"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTr_Smail", dataset.Tables[1].Rows[0]["SMail"].ToString()));

                PhoneParameter.Add(new ReportParameter("RPTt_CreatedBy", dataset.Tables[0].Rows[0]["UserName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTu_Designation", dataset.Tables[0].Rows[0]["Designation"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTv_NoofPages", (ReportViewer1.ServerReport.GetTotalPages() + 1).ToString()));


                string imagePath = "file:\\" + MapPath("\\images\\Volta_Logo.png");
                string Footer1imagePath = "file:\\" + MapPath("\\images\\footer1.png");
                string Footer2imagePath = "file:\\" + MapPath("\\images\\FottorImage2.png");
                //string Footer2imagePath = "file:\\" + MapPath("\\images\\123.png");
                PhoneParameter.Add(new ReportParameter("RPTs_ImagePath", imagePath));
                PhoneParameter.Add(new ReportParameter("RPT_FooterImage1", Footer1imagePath));
                PhoneParameter.Add(new ReportParameter("RPT_FooterImage2", Footer2imagePath));


                //D:\voms\VOMS_ERP\images\logo.png
                //= file:\D:\voms\VOMS_ERP\images\logo.png
                ReportViewer1.LocalReport.SetParameters(PhoneParameter);
                //vomserpdbDataSet ds=new vomserpdbDataSet ();
                //this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SP_FERPTAllItemDeatails", ds.Tables[0]));

                ReportDataSource dsItemDetails = new ReportDataSource();
                // dsEnumMasterDetails.Name = "VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter";
                dsItemDetails.Name = "vomserpdbDataSet_SP_LERPTAllItemDeatails";
                //VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter
                DataSet dsItems = new DataSet();
                dsItems = CRPTBLL.GetLEnqAllDetails_Items(ID);
                dsItemDetails.Value = dsItems.Tables[0];

                localReport.DataSources.Add(dsItemDetails);

                this.ReportViewer1.LocalReport.Refresh();

                //this.ReportViewer1.LocalReport.Refresh(); //PostBackUrl="~/Quotations/NewLQuotation.aspx"

                string filename = MapPath("~/uploads/") + ID + "LEnq.pdf";
                CreatePDF(filename);

                return filename;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Local Enquiry Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// Foreign Quotation RDLC Report Generation
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected string GenerateFQPDF(Guid ID)
        {
            try
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FQDetails.rdlc");
                ReportViewer1.LocalReport.EnableExternalImages = true;
                NewFQuotationBLL NFQBLL = new NewFQuotationBLL();
                DataSet dataset = new DataSet();
                dataset = NFQBLL.Select(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "",
                    DateTime.Now, 0, Guid.Empty, 0, "", new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty,
                    DateTime.Now, false, CommonBLL.EmptyDtFQ(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));

                List<ReportParameter> PhoneParameter = new List<ReportParameter>();

                PhoneParameter.Add(new ReportParameter("RPTa_QuotationNo", dataset.Tables[1].Rows[0]["Quotationnumber"].ToString()));//DeptName
                PhoneParameter.Add(new ReportParameter("RPTc_KindAttn", dataset.Tables[1].Rows[0]["Instruction"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTd_Subject", dataset.Tables[1].Rows[0]["Subject"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTe_Reference", dataset.Tables[1].Rows[0]["FEnqName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTf_PriceBasis", dataset.Tables[1].Rows[0]["PriceBasis"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTg_Delivery",
                    (Convert.ToDateTime(dataset.Tables[1].Rows[0]["DeliveryDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("RPTj_QtnDate",
                    (Convert.ToDateTime(dataset.Tables[1].Rows[0]["QuotationDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("RPTh_Payment", dataset.Tables[1].Rows[0]["Quotationnumber"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTb_To", dataset.Tables[1].Rows[0]["CustName"].ToString()));
                //PhoneParameter.Add(new ReportParameter("Parm_CityAndCountry", dataset.Tables[0].Rows[0]["StateCountry"].ToString()));

                clsNum2WordBLL n2w = new clsNum2WordBLL();
                //string Number = dsItems.Tables[0].Compute("Sum(Amount)", "").ToString();
                string Words = n2w.Num2WordConverter(dataset.Tables[1].Compute("Sum(TotalAmount)", "").ToString(), "").ToString();
                PhoneParameter.Add(new ReportParameter("RPTi_TotalAmount", Words));

                string imagePath = "file:\\" + MapPath("\\images\\Volta_Logo.png");
                string Footer1imagePath = "file:\\" + MapPath("\\images\\footer1.png");
                string Footer2imagePath = "file:\\" + MapPath("\\images\\FottorImage2.png");
                //string Footer2imagePath = "file:\\" + MapPath("\\images\\123.png");
                PhoneParameter.Add(new ReportParameter("RPT_ImageHeader", imagePath));
                PhoneParameter.Add(new ReportParameter("RPT_ImageFooter1", Footer1imagePath));
                PhoneParameter.Add(new ReportParameter("RPT_ImageFooter2", Footer2imagePath));

                ReportViewer1.LocalReport.SetParameters(PhoneParameter);

                ReportDataSource dsItemDetails = new ReportDataSource();
                ReportDataSource dsItemDetails1 = new ReportDataSource();
                ReportDataSource dsItemDetails2 = new ReportDataSource();

                dsItemDetails.Name = "vomserpdbDataSet_SP_FQRPTAllItemDeatails";
                DataSet dsItems = new DataSet();
                dsItems = CRPTBLL.GetFQAllDetails_Items(ID);
                dsItemDetails.Value = dsItems.Tables[0];

                dsItemDetails1.Name = "vomserpdbDataSet_SP_AllTermsConditions";
                DataSet dsItems1 = new DataSet();
                dsItems1 = CRPTBLL.GetAllTermsConditons(CommonBLL.FlagXSelect, "ForeignQuotationId", ID.ToString());
                dsItemDetails1.Value = dsItems1.Tables[0];

                dsItemDetails2.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                DataSet dsItems2 = new DataSet();
                dsItems2 = CRPTBLL.GetLPOPayments(CommonBLL.FlagXSelect, ID);
                dsItemDetails2.Value = dsItems2.Tables[0];

                localReport.DataSources.Clear();
                localReport.DataSources.Add(dsItemDetails);
                localReport.DataSources.Add(dsItemDetails1);
                localReport.DataSources.Add(dsItemDetails2);

                this.ReportViewer1.LocalReport.Refresh();

                string filename = MapPath("~/uploads/") + ID + "FQuote.pdf";
                CreatePDF(filename);

                return filename;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Enquiry Details", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// Foreign Purchase Order RDLC Report Generation
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected string GenerateFPOPDF(Guid ID)
        {
            try
            {
                this.ReportViewer1.LocalReport.Refresh();
                ReportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FPODetails.rdlc");
                NewFPOBLL NFPOBLL = new NewFPOBLL();
                DataSet dataset = new DataSet();
                dataset = NFPOBLL.Select(CommonBLL.FlagBSelect, ID, "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", "", DateTime.Now,
                    DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "", DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()),
                    DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPO(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));

                string[] addr = dataset.Tables[0].Rows[0]["CustBillAdd"].ToString().Split(',');

                List<ReportParameter> PhoneParameter = new List<ReportParameter>();

                PhoneParameter.Add(new ReportParameter("RPTa_From", dataset.Tables[0].Rows[0]["OrgName"].ToString()));//DeptName
                //PhoneParameter.Add(new ReportParameter("RPTb_To", dataset.Tables[0].Rows[0]["Instruction"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTg_Subject", dataset.Tables[0].Rows[0]["Subject"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTd_TelephoneNo", dataset.Tables[0].Rows[0]["Phone"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTe_FaxNo", dataset.Tables[0].Rows[0]["Fax1"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTf_EmailID", dataset.Tables[0].Rows[0]["PriEmail"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTh_Ref", dataset.Tables[0].Rows[0]["FQuotationName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTi_RefAndDate",
                    (Convert.ToDateTime(dataset.Tables[0].Rows[0]["FPODate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("RPTk_ShipmentMode", dataset.Tables[0].Rows[0]["ShipmentModeText"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTj_ReleaseDate",
                    (Convert.ToDateTime(dataset.Tables[0].Rows[0]["FPODueDate"].ToString())).ToString("dd-MM-yyyy")));
                PhoneParameter.Add(new ReportParameter("RPTl_Department", dataset.Tables[0].Rows[0]["Department"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTm_Payments", dataset.Tables[0].Rows[0]["Instruction"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTn_Address", addr[0].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTo_Address1", addr[1].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTp_Address2", addr[2].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTq_Address3", addr[3].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTr_Address4", addr[2].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTs_FPONo", dataset.Tables[0].Rows[0]["ForeignPurchaseOrderNo"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTt_FEnqNo", dataset.Tables[0].Rows[0]["FEnqNo"].ToString()));


                ReportViewer1.LocalReport.SetParameters(PhoneParameter);
                //vomserpdbDataSet ds=new vomserpdbDataSet ();
                //this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SP_FERPTAllItemDeatails", ds.Tables[0]));

                ReportDataSource dsItemDetails = new ReportDataSource();
                ReportDataSource dsItemDetails1 = new ReportDataSource();
                ReportDataSource dsItemDetails2 = new ReportDataSource();
                // dsEnumMasterDetails.Name = "VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter";
                dsItemDetails.Name = "vomserpdbDataSet_SP_FPRPTAllItemDeatails";
                //VOMS_ERP.vomserpdbDataSetTableAdapters.SP_FERPTAllItemDeatailsTableAdapter
                DataSet dsItems = new DataSet();
                dsItems = CRPTBLL.GetFPOAllDetails_Items(ID);
                dsItemDetails.Value = dsItems.Tables[0];
                string totalAmt = dsItems.Tables[0].Compute("Sum(Amount)", "").ToString();
                if (totalAmt == "")
                    totalAmt = "0.00";
                string Words = n2w.Num2WordConverter(totalAmt, "").ToString();
                PhoneParameter.Add(new ReportParameter("RPTu_TotalAmount", Words));
                ReportViewer1.LocalReport.SetParameters(PhoneParameter);

                dsItemDetails1.Name = "vomserpdbDataSet_SP_AllTermsConditions";//SP_LPORPT_PaymentsTerms
                DataSet dsItems1 = new DataSet();
                dsItems1 = CRPTBLL.GetAllTermsConditons(CommonBLL.FlagYSelect, "ForeignPurchaseOrderId", ID.ToString());
                dsItemDetails1.Value = dsItems1.Tables[0];


                dsItemDetails2.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";//SP_LPORPT_PaymentsTerms
                DataSet dsItems2 = new DataSet();
                dsItems2 = CRPTBLL.GetLPOPayments(CommonBLL.FlagYSelect, ID);
                dsItemDetails2.Value = dsItems2.Tables[0];

                localReport.DataSources.Clear();
                localReport.DataSources.Add(dsItemDetails);
                localReport.DataSources.Add(dsItemDetails1);
                localReport.DataSources.Add(dsItemDetails2);

                this.ReportViewer1.LocalReport.Refresh();

                string filename = MapPath("~/uploads/") + ID + "FPOrder.pdf";
                CreatePDF(filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Local Purchase Order RDLC Report Generation
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected string GenerateLPOPDF(Guid ID)
        {
            try
            {
                ReportViewer1 = new ReportViewer();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LPODetails.rdlc");
                LPOrdersBLL LPOBLL = new LPOrdersBLL();
                DataSet dataset = new DataSet();
                dataset = LPOBLL.SelectLPOrders(CommonBLL.FlagASelect, ID, Guid.Empty, "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, Guid.Empty, "", DateTime.Now, 0, 0, "",
                    new Guid(Session["UserID"].ToString()), CommonBLL.EmptyDtLPOrders(), CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(),
                    0, 0, 0, 0, 0, 0, 0, 0, new Guid(Session["CompanyID"].ToString()), "");

                List<ReportParameter> PhoneParameter = new List<ReportParameter>();

                PhoneParameter.Add(new ReportParameter("RPTa_LPONumber", dataset.Tables[0].Rows[0]["LocalpurchaseOrderNo"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTb_LPODate",
                    CommonBLL.DateDisplay(Convert.ToDateTime(dataset.Tables[0].Rows[0]["LocalpurchaseOrderDate"].ToString()))));
                PhoneParameter.Add(new ReportParameter("RPTf_OrgName", dataset.Tables[0].Rows[0]["OrgName"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTc_LPOAddress", dataset.Tables[0].Rows[0]["SupAdd"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTd_Subject", dataset.Tables[0].Rows[0]["Subject"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTe_QuoteNoAndDate",
                    dataset.Tables[0].Rows[0]["Quotationnumber"].ToString() + ". " +
                    CommonBLL.DateDisplay(Convert.ToDateTime(dataset.Tables[0].Rows[0]["QuotationDate"].ToString()))));
                PhoneParameter.Add(new ReportParameter("RPTg_VCode", dataset.Tables[0].Rows[0]["VCode"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTh_DeliveryDate",
                    dataset.Tables[0].Rows[0]["DeliveryPeriod"].ToString() + " WEEKS. "));
                PhoneParameter.Add(new ReportParameter("RPTi_ExDutyPercent", dataset.Tables[0].Rows[0]["ExDutyPercentage"].ToString() + " %"));
                PhoneParameter.Add(new ReportParameter("RPTj_SalesTaxPercent",
                    dataset.Tables[0].Rows[0]["SaleTaxPercentage"].ToString() + " %"));
                PhoneParameter.Add(new ReportParameter("RPTk_PackingPercentage",
                    dataset.Tables[0].Rows[0]["PackingPercentage"].ToString() + " %"));
                PhoneParameter.Add(new ReportParameter("RPTl_LPOAddress1", dataset.Tables[0].Rows[0]["BillStreet"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTm_LPOAddress2", dataset.Tables[0].Rows[0]["City"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTn_LPOAddress3", dataset.Tables[0].Rows[0]["StateNm"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTo_LPOAddress4", dataset.Tables[0].Rows[0]["Cntry"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTp_LPOAddress5", dataset.Tables[0].Rows[0]["BillPin"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTt_FrnPOrder", dataset.Tables[0].Rows[0]["FrnPOrder"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTu_CustmName", dataset.Tables[0].Rows[0]["CustmNm"].ToString()));
                PhoneParameter.Add(new ReportParameter("RPTv_CustmAddr", dataset.Tables[0].Rows[0]["CustmAddr"].ToString()));

                ReportDataSource dsItemDetails = new ReportDataSource();
                dsItemDetails.Name = "vomserpdbDataSet_SP_LPORPTAllItemDeatails";
                DataSet dsItems = new DataSet();
                dsItems = CRPTBLL.GetLPOAllDetails_Items(ID);
                dsItemDetails.Value = dsItems.Tables[0];

                string Words = n2w.Num2WordConverter(dsItems.Tables[0].Compute("Sum(Amount)", "").ToString(), "RS").ToString();
                PhoneParameter.Add(new ReportParameter("RPTs_TotalAmount", Words));

                ReportDataSource dsItemDetails1 = new ReportDataSource();
                dsItemDetails1.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                DataSet dsItems1 = new DataSet();
                dsItems1 = CRPTBLL.GetLPOPayments(CommonBLL.FlagZSelect, ID);
                dsItemDetails1.Value = dsItems1.Tables[0];

                ReportDataSource dsItemDetails2 = new ReportDataSource();
                dsItemDetails2.Name = "vomserpdbDataSet_SP_AllTermsConditions";
                DataSet dsItems2 = new DataSet();
                dsItems2 = CRPTBLL.GetAllTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", ID.ToString());
                dsItemDetails2.Value = dsItems2.Tables[0];

                ReportDataSource StaticTable = new ReportDataSource();
                StaticTable.Name = "vomserpdbDataSet_DataTable1";
                DataTable StaticData1 = StaticData();
                StaticTable.Value = StaticData1;

                ReportViewer1.LocalReport.SetParameters(PhoneParameter);

                localReport.DataSources.Clear();
                localReport.DataSources.Add(dsItemDetails);
                localReport.DataSources.Add(dsItemDetails1);
                localReport.DataSources.Add(dsItemDetails2);
                localReport.DataSources.Add(StaticTable);
                this.ReportViewer1.LocalReport.Refresh();

                string filename = MapPath("~/uploads/") + ID + "LPOrder.pdf";
                CreatePDF(filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Bind Default/Static Data to Data Table
        /// </summary>
        /// <returns></returns>
        protected DataTable StaticData()
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                DataRow dr1 = dt.NewRow();
                dt.Columns.Add("Title");
                dt.Columns.Add("Description");

                dr["Title"] = "Submission of Documents";
                dr["Description"] = "A set of following documents should be submitted to our Hyderabad office for arranging payment."
                                        + System.Environment.NewLine +
                                    "1) Commercial Invoice." + System.Environment.NewLine +
                                    "2) Copy of L.R." + System.Environment.NewLine +
                                    "3) Packing list including no. of packages, description" +
                                        "and quantity of goods in each pack, weight and size of boxes/packages";
                dt.Rows.Add(dr);

                dr1["Title"] = "Shipping";
                dr1["Description"] = "Following Shipping Marks should be marked on the packing box";

                dt.Rows.Add(dr1);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Dispatch Instructions RDLC Report Generation
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected string GenerateDspchInstPDF(Guid ID)
        {
            try
            {
                ReportViewer1 = new ReportViewer();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                LocalReport localReport = ReportViewer1.LocalReport;

                localReport.ReportPath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\DspchInstRpt.rdlc");
                LPOrdersBLL LPOBLL = new LPOrdersBLL();

                DataSet DspchInst = DIBL.SelectDspchInstnsRpt(ID, new Guid(Session["UserID"].ToString()));

                ReportDataSource datasource = new ReportDataSource("vomserpdbDataSet_SP_DispatchInstRpt", DspchInst.Tables[0]);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasource);
                ReportViewer1.LocalReport.Refresh();

                string filename = MapPath("~/uploads/") + ID + "DspchInst.pdf";
                CreatePDF(filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Preparation of Dspch. Inst. in Email Send", ex.Message.ToString());
                return string.Empty;
            }
        }
        #endregion

        #region Create Report PDF

        /// <summary>
        /// Generate PDF
        /// </summary>
        /// <param name="fileName"></param>
        private void CreatePDF(string fileName)
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                ReportViewer1.ProcessingMode = ProcessingMode.Local;

                byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType,
                    out encoding, out extension, out streamIds, out warnings);

                using (FileStream stream = File.OpenWrite(fileName))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Email Send", ex.Message.ToString());
            }
        }
        protected string GenerateFePDF_New(Guid ID, Guid CompanyId)
        {
            try
            {
                ReportDataSource FeDtlsDSet = new ReportDataSource();
                ReportDataSource FeItmDSet = new ReportDataSource();
                FeDtlsDSet.Name = "vomserpdbDataSet_SP_FERPTItemDeatails";
                FeItmDSet.Name = "vomserpdbDataSet_SP_FERPTAllItemDeatails";
                DataSet dataset = CRPTBLL.GetFenqDetails_Items(ID, CompanyId);
                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FEnquiryCrp.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];
                        if (SubRepName == "FenqItemsCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFenqAllDetails_Items(ID).Tables[0]);
                        }
                    }
                }

                string filename = MapPath("~/uploads/") + ID + "FEnq.pdf";
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Local Enquiry Details", ex.Message.ToString());

                return null;
            }
        }

        //protected string GenerateFFePDF_New(Guid ID, Guid CompanyId)
        //{
        //    try
        //    {
        //        ReportDataSource FeDtlsDSet = new ReportDataSource();
        //        ReportDataSource FeItmDSet = new ReportDataSource();
        //        FeDtlsDSet.Name = "vomserpdbDataSet_SP_FERPTItemDeatails";
        //        FeItmDSet.Name = "vomserpdbDataSet_SP_FERPTAllItemDeatails";
        //        DataSet dataset = CRPTBLL.GetFFenqDetails_Items(ID, CompanyId);
        //        crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FEnquiryCrp.rpt"));
        //        crystalReport.Load(crystalReport.FileName);
        //        crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
        //        foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
        //        {
        //            if (repOp.Kind == ReportObjectKind.SubreportObject)
        //            {
        //                string SubRepName = ((SubreportObject)repOp).SubreportName;
        //                ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];
        //                if (SubRepName == "FenqItemsCrp.rpt")
        //                {
        //                    subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFenqAllDetails_Items(ID).Tables[0]);
        //                }
        //            }
        //        }

        //        string filename = MapPath("~/uploads/") + ID + "FEnq.pdf";
        //        crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

        //        return filename;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Local Enquiry Details", ex.Message.ToString());

        //        return null;
        //    }
        //}

        protected string GenerateLePDF_New(Guid ID)
        {
            try
            {
                ReportDataSource LeDtlsDSet = new ReportDataSource();
                ReportDataSource LeItmDSet = new ReportDataSource();

                LeDtlsDSet.Name = "vomserpdbDataSet_SP_LERPTItemDeatails";
                LeItmDSet.Name = "vomserpdbDataSet_SP_LERPTAllItemDeatails";

                DataSet dataset = CRPTBLL.GetLenqDetails_Items(ID);

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = CommonBLL.CommonLogoUrl(HttpContext.Current);

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LEnqCrp.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];
                        if (SubRepName == "LenqItemsCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLEnqAllDetails_Items(ID).Tables[0]);
                        }
                    }
                }
                crystalReport.SetParameterValue(0, imgpath);
                string filename = MapPath("~/uploads/") + dataset.Tables[0].Rows[0]["EnquireNumber"].ToString().Replace('/', '_') + "LEnq.pdf";
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Foreign Enquiry Details", ex.Message.ToString());

                return null;
            }
        }
        protected string GenerateFQPDF_New(Guid ID)
        {
            try
            {
                ReportDataSource dsItemDetails9 = new ReportDataSource();
                ReportDataSource dsItemDetails8 = new ReportDataSource();
                ReportDataSource dsItemDetails7 = new ReportDataSource();
                dsItemDetails9.Name = "vomserpdbDataSet_SP_FQRPTAllItemDeatails";
                dsItemDetails8.Name = "vomserpdbDataSet_SP_AllTermsConditions";
                dsItemDetails7.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                DataSet dsItems9 = new DataSet();
                dsItems9 = CRPTBLL.GetFQAllDetails_Items(ID);
                dsItemDetails9.Value = dsItems9.Tables[0];
                DataSet dsItems8 = new DataSet();
                dsItems8 = CRPTBLL.GetAllTermsConditons(CommonBLL.FlagXSelect, "ForeignQuotationId", ID.ToString());
                DataSet dsItems7 = new DataSet();
                dsItems7 = CRPTBLL.GetLPOPayments(CommonBLL.FlagXSelect, ID);

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FQReport.rpt"));
                crystalReport.Load(crystalReport.FileName);

                DataSet dataset1 = new DataSet();
                NewFQuotationBLL NFQBLL1 = new NewFQuotationBLL();
                dataset1 = NFQBLL1.Select(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, "", 0, 0, 0, 0, Guid.Empty, "",
                    DateTime.Now, 0, Guid.Empty, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, CommonBLL.EmptyDtFQ(),
                    CommonBLL.FirstRowPaymentTerms(), CommonBLL.ATConditions(), "", new Guid(Session["CompanyID"].ToString()));

                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {
                    if (!dataset1.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                    {
                        if (repOp.Name == "VoltaFooter")
                            repOp.ObjectFormat.EnableSuppress = true;
                        if (repOp.Name == "FormetNo")
                            repOp.ObjectFormat.EnableSuppress = true;
                    }
                    else
                    {
                        if (repOp.Name == "FooterText")
                            repOp.ObjectFormat.EnableSuppress = true;
                    }

                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];
                        if (SubRepName == "FQTC.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems8.Tables[0]);
                            subRepDoc.Database.Tables[1].SetDataSource(dsItems7.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems7.Tables[0]);
                        }
                        else
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(dsItems9.Tables[0]);
                            subRepDoc.Database.Tables[1].SetDataSource(dsItems8.Tables[0]);
                        }
                    }
                }


                crystalReport.SetParameterValue("RPTa_QuotationNo", dataset1.Tables[1].Rows[0]["Quotationnumber"].ToString());
                crystalReport.SetParameterValue("RPTc_KindAttn", dataset1.Tables[1].Rows[0]["Instruction"].ToString());
                crystalReport.SetParameterValue("RPTd_Subject", dataset1.Tables[1].Rows[0]["Subject"].ToString());
                crystalReport.SetParameterValue("RPTe_Reference", dataset1.Tables[1].Rows[0]["FEnqName"].ToString());
                crystalReport.SetParameterValue("RPTf_PriceBasis", dataset1.Tables[1].Rows[0]["PriceBasis"].ToString());
                crystalReport.SetParameterValue("RPTg_Delivery",
                    (Convert.ToDateTime(dataset1.Tables[1].Rows[0]["DeliveryDate"].ToString())).ToString("dd-MM-yyyy"));
                crystalReport.SetParameterValue("RPTj_QtnDate",
                    (Convert.ToDateTime(dataset1.Tables[1].Rows[0]["QuotationDate"].ToString())).ToString("dd-MM-yyyy"));
                crystalReport.SetParameterValue("RPTh_Payment", dataset1.Tables[1].Rows[0]["Quotationnumber"].ToString());
                crystalReport.SetParameterValue("RPTb_To", dataset1.Tables[1].Rows[0]["CustFlNm"].ToString());
                crystalReport.SetParameterValue("RPTm_CnctPrsn", dataset1.Tables[1].Rows[0]["CustCnctPrsn"].ToString());
                crystalReport.SetParameterValue("RPTm_CompanyCnctPrsn", dataset1.Tables[1].Rows[0]["CmpnyCntPersn"].ToString());
                crystalReport.SetParameterValue("RPTm_CompanyAddress", dataset1.Tables[1].Rows[0]["CompanyAddress"].ToString());
                crystalReport.SetParameterValue("RPTm_CompanyName", dataset1.Tables[1].Rows[0]["CompanyName"].ToString());
                crystalReport.SetParameterValue("RPTm_CompanyFax", dataset1.Tables[1].Rows[0]["CompanyFax"].ToString());
                crystalReport.SetParameterValue("RPTm_CompanyEmail", dataset1.Tables[1].Rows[0]["CompanyEmail"].ToString());
                byte[] imge = null;
                if (dataset1 != null && dataset1.Tables[1] != null && dataset1.Tables[1].Rows.Count > 0 && dataset1.Tables[1].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset1.Tables[1].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                crystalReport.SetParameterValue("CompanyLogo", imgpath);
                //PhoneParameter.Add(new ReportParameter("Parm_CityAndCountry", dataset.Tables[0].Rows[0]["StateCountry"].ToString()));

                clsNum2WordBLL n2w1 = new clsNum2WordBLL();
                //string Number = dsItems.Tables[0].Compute("Sum(Amount)", "").ToString();
                string Words1 = n2w1.Num2WordConverter(dataset1.Tables[1].Compute("Sum(TotalAmount)", "").ToString(), "").ToString();
                crystalReport.SetParameterValue("RPTi_TotalAmount", Words1);
                string LogoImage = "";
                if (((string)(dataset1.Tables[1].Rows[0]["CompanyName"].ToString().ToLower())).Contains("volta"))
                {
                    LogoImage = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\FottorLogo1.JPG");
                }
                else
                {
                    LogoImage = "";
                }
                crystalReport.SetParameterValue("ISOLOGO", LogoImage);

                string filename = MapPath("~/uploads/") + ID + "FQuot.pdf";
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/Foreign Quotation Details", ex.Message.ToString());

                return null;
            }
        }
        protected string GenerateFPOPDF_New(Guid ID)
        {
            try
            {
                ReportDataSource FPODtlsDSet = new ReportDataSource();
                ReportDataSource FPOItmDSet = new ReportDataSource();
                ReportDataSource FPOPymntDSet = new ReportDataSource();
                ReportDataSource FPOTrmCdtnsDSet = new ReportDataSource();


                FPODtlsDSet.Name = "vomserpdbDataSet_SP_FPORPTItemDetails";
                FPOItmDSet.Name = "vomserpdbDataSet_SP_FPORPTAllItemDeatails";
                FPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                FPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";

                DataSet dataset = CRPTBLL.GetFPODetails_Items(ID);

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\FpoDetailsCrp.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagYSelect, "ForeignPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "FpoItemsCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetFPOAllDetails_Items(ID).Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagYSelect, ID).Tables[0]);

                        }
                    }
                }

                string filename = MapPath("~/uploads/") + ID + "FPO.pdf";
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/FPO Details", ex.Message.ToString());

                return null;
            }
        }
        protected string GenerateLPOPDF_New(Guid ID)
        {
            try
            {

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                ReportDataSource LPOItmDSet = new ReportDataSource();
                ReportDataSource LPOPymntDSet = new ReportDataSource();
                ReportDataSource LPOTrmCdtnsDSet = new ReportDataSource();


                LPODtlsDSet.Name = "vomserpdbDataSet_SP_LPORPTItemDetails";
                LPOItmDSet.Name = "vomserpdbDataSet_SP_LPORPTAllItemDeatails";
                LPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";

                DataSet dataset = CRPTBLL.GetLPODetails_Items(ID); ;
                DataSet ds = new DataSet();

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LpoCrp.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {

                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "LpoBody.rpt")
                        {
                            ds = CRPTBLL.GetLPOAllDetails_Items(ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagZSelect, ID).Tables[0]);
                        }
                    }
                }

                string filename = MapPath("~/uploads/") + ID + "LPO.pdf";

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("~/images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png"); //CommonBLL.CommonLogoUrl(HttpContext.Current);

                string AMT = "0.00";
                if (ds != null && ds.Tables.Count > 0)
                    AMT = Convert.ToDecimal(ds.Tables[0].Compute("sum(TotalAmt)", "")).ToString("N2");

                string DscntAmunt = ""; decimal NetTotal = 0;
                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                {
                    DscntAmunt = ds.Tables[2].Rows[0]["discountamt"].ToString();
                    NetTotal = Convert.ToDecimal(AMT) - Convert.ToDecimal(DscntAmunt);
                }
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                string AmtInWords = ToWords.Num2WordConverter(AMT, "RS").ToString();
                crystalReport.SetParameterValue("TotalAmountInRs", AmtInWords);
                crystalReport.SetParameterValue("CompanyLogo", imgpath);
                if (Convert.ToDecimal(DscntAmunt) != 0)
                {
                    crystalReport.SetParameterValue("discountamt", Convert.ToDecimal(DscntAmunt).ToString("N"));
                    crystalReport.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                }
                else
                {
                    crystalReport.SetParameterValue("discountamt", "");
                    crystalReport.SetParameterValue("NetTotalVal", "");
                }
                //crystalReport.SetParameterValue("discountamt", ""); 
                //crystalReport.SetParameterValue("NetTotalVal", "");

                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/LPO Details", ex.Message.ToString());

                return null;
            }
        }

        /// <summary>
        /// LPO Customer Mail Send PDF
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected string GenerateLPOPDF_Customer(Guid ID)
        {
            try
            {

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                ReportDataSource LPOItmDSet = new ReportDataSource();
                ReportDataSource LPOPymntDSet = new ReportDataSource();
                ReportDataSource LPOTrmCdtnsDSet = new ReportDataSource();


                LPODtlsDSet.Name = "vomserpdbDataSet_SP_LPORPTItemDetails_Customer";
                LPOItmDSet.Name = "vomserpdbDataSet_SP_LPORPTAllItemDeatails";
                LPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";

                DataSet dataset = CRPTBLL.GetLPODetails_Items_Customer(ID); ;
                DataSet ds = new DataSet();

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LpoCrp.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {

                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "LpoBody.rpt")
                        {
                            ds = CRPTBLL.GetLPOAllDetails_Items(ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagZSelect, ID).Tables[0]);
                        }
                    }
                }

                string filename = MapPath("~/uploads/") + ID + "LPO.pdf";

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("~/images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png"); //CommonBLL.CommonLogoUrl(HttpContext.Current);

                string AMT = "0.00";
                if (ds != null && ds.Tables.Count > 0)
                    AMT = Convert.ToDecimal(ds.Tables[0].Compute("sum(TotalAmt)", "")).ToString("N2");

                string DscntAmunt = ""; decimal NetTotal = 0;
                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                {
                    DscntAmunt = ds.Tables[2].Rows[0]["discountamt"].ToString();
                    NetTotal = Convert.ToDecimal(AMT) - Convert.ToDecimal(DscntAmunt);
                }
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                string AmtInWords = ToWords.Num2WordConverter(AMT, "RS").ToString();
                crystalReport.SetParameterValue("TotalAmountInRs", AmtInWords);
                crystalReport.SetParameterValue("CompanyLogo", imgpath);
                if (Convert.ToDecimal(DscntAmunt) != 0)
                {
                    crystalReport.SetParameterValue("discountamt", Convert.ToDecimal(DscntAmunt).ToString("N"));
                    crystalReport.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                }
                else
                {
                    crystalReport.SetParameterValue("discountamt", "");
                    crystalReport.SetParameterValue("NetTotalVal", "");
                }
                //crystalReport.SetParameterValue("discountamt", ""); 
                //crystalReport.SetParameterValue("NetTotalVal", "");

                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/LPO Details", ex.Message.ToString());

                return null;
            }
        }

        protected string GenerateCT1PDF_New(Guid ID)
        {
            try
            {

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                LPODtlsDSet.Name = "vomserpdbDataSet_SP_ProformaINVReq_RDLC";

                DataSet dataset = CRPTBLL.GetPInvReqDtls(ID);

                crystalReport.Load(Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\ProformaINnvReq.rpt"));
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);

                string filename = MapPath("~/uploads/") + ID + "_ExciseDetailsRequisition.pdf";
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/LPO Details", ex.Message.ToString());

                return null;
            }
        }
        protected string GenerateLQPDF_New(Guid ID)
        {
            try
            {
                ReportDataSource LQDtlsDSet = new ReportDataSource();
                ReportDataSource LQItmDSet = new ReportDataSource();
                ReportDataSource LQPymntDSet = new ReportDataSource();
                ReportDataSource LQTrmCdtnsDSet = new ReportDataSource();
                ReportDocument rptDoc = new ReportDocument();

                LQDtlsDSet.Name = "vomserpdbDataSet_SP_LQRPTItemDeatails";
                LQItmDSet.Name = "vomserpdbDataSet_SP_LQRPTAllItemDeatails";
                LQPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LQTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetLquteDetails_Items(ID); ;

                //ReportDocument rptDoc = new ReportDocument();
                crystalReport.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LQuotCrp.rpt");
                crystalReport.Load(crystalReport.FileName);
                crystalReport.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in crystalReport.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = crystalReport.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagWCommonMstr, "LocalQuotationId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "LQuoteBodyCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLLQAllDetails_Items(ID).Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagWCommonMstr, ID).Tables[0]);

                        }
                    }
                }

                //LclQuotationDtls.ReportSource = rptDoc;
                DataSet ds = new DataSet();
                ds = CRPTBLL.GetLLQAllDetails_Items(ID);
                string AmtFoot = ds.Tables[0].Compute("sum(Amount)", "").ToString();
                if (AmtFoot == "")
                    AmtFoot = "0.00";
                clsNum2WordBLL n2w = new clsNum2WordBLL();
                string Words = n2w.Num2WordConverter(AmtFoot, "RS").ToString();
                crystalReport.SetParameterValue("ToWords", Words);
                //LclQuotationDtls.ReportSource = rptDoc;

                string filename = MapPath("~/uploads/") + ID + "LQuotCrp.pdf";
                //CreatePDF(filename);
                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);
                return filename;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/LPO Details", ex.Message.ToString());

                return null;

            }
        }

        #endregion

        //protected string GenerateDspchInstPDF_New(int ID)
        //{
        //    try
        //    {
        //        LPOrdersBLL LPOBLL = new LPOrdersBLL();
        //        DataSet DspchInst = DIBL.SelectDspchInstnsRpt(ID, Convert.ToInt64(Session["UserID"].ToString()));
        //        ReportDocument RptDoc = new ReportDocument();
        //        RptDoc.FileName = Server.MapPath("\\RDLC\\DspchInstRpt.rpt");
        //        RptDoc.Load(RptDoc.FileName);
        //        RptDoc.Database.Tables[0].SetDataSource(DspchInst.Tables[1]);

        //        string filename = MapPath("~/uploads/") + ID + "DspchInst.pdf";
        //        crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filename);

        //        return filename;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "EmailSend/DispatchInst Details", ex.Message.ToString());

        //        return null;
        //    }
        //}

    }
}
