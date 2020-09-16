using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using BAL;
using System.Text;
using System.Data;
using System.Collections;
using System.Globalization;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class ShpngBilDtls : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        ChaMasterBLL CMBL = new ChaMasterBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        ShpngBilDtlsBLL SBDBL = new ShpngBilDtlsBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load Events

        /// <summary>
        /// Default Page Load Events
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(ShpngBilDtls));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods

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
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                SpnFOB.InnerText = "FOB Value (" + Session["CurrencySymbol"].ToString().Trim() + ") :";
                SpnInr.InnerText = "(" + Session["CurrencySymbol"].ToString().Trim() + ") :";
                SpnSTF.InnerText = "Service Tax Refund (" + Session["CurrencySymbol"].ToString().Trim() + ") :";
                SpnTODB.InnerText = "Total Draw Back" ;  //(" + Session["CurrencySymbol"].ToString().Trim() + ") :";
                LblAmount.Text = "Invoice Value(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                LblFOBVal.Text = "FOB Value(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                txtLeoDt.Attributes.Add("readonly", "readonly");
                txtShpngBlDt.Attributes.Add("readonly", "readonly");
                txtLofspDt.Attributes.Add("readonly", "readonly");
                txtStfngDt.Attributes.Add("readonly", "readonly");
                txtFileDt.Attributes.Add("readonly", "readonly");
                txtRtnDt.Attributes.Add("readonly", "readonly");
                txtRbiWvDt.Attributes.Add("readonly", "readonly");
                txtDbkScrlDt.Attributes.Add("readonly", "readonly");
                txtLeoDate.Attributes.Add("readonly", "readonly");
                txtMrkDt.Attributes.Add("readonly", "readonly");
                txtAmntRmtdDt.Attributes.Add("readonly", "readonly");
                txtDbkScrolldDt.Attributes.Add("readonly", "readonly");
                txtPfrmInvcDt.Attributes.Add("readonly", "readonly");
                txtLetExptDt.Attributes.Add("readonly", "readonly");
                txtShpmntDt.Attributes.Add("readonly", "readonly");
                txtDepbLicDate.Attributes.Add("readonly", "readonly");
                txtLetExptDt.Attributes.Add("readonly", "readonly");//read only mode add by manju
                txtLeoDate.Attributes.Add("readonly", "readonly");//read only mode add by manju
                txtInsrncAmnt.Attributes.Add("readonly", "readonly");
                txtFrtAmnt.Attributes.Add("readonly", "readonly");
                txtDscntAmnt.Attributes.Add("readonly", "readonly");
                txtCmsnAmnt.Attributes.Add("readonly", "readonly");
                txtOtrDtcnsAmnt.Attributes.Add("readonly", "readonly");
                txtPkngChrgsAmnt.Attributes.Add("readonly", "readonly");
                txtPrdPmnt.Attributes.Add("readonly", "readonly");
                rbtnArests.SelectedValue = rbtnDBK.SelectedValue = rbtnepcpy.SelectedValue = "1";
                BindDropDownList(ddlPrfmaInvcNo, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagODRP, Guid.Empty, Guid.Empty, Guid.Empty, "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())).Tables[0]);
                BindDropDownList(ddlChaMstr, CMBL.SelectChaMaster(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[0]);
                BindDropDownList(ddlPlcOrgGds, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofOrigin).Tables[0]);
                BindDropDownList(ddlLUTARNNo, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.LUTARNNo).Tables[0]);
                BindDropDownList(ddlPlcFnlDstn, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty,
                    Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofFinalDestination).Tables[0]);
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading).Tables[0]);
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge).Tables[0]);
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge).Tables[0]);
                BindDropDownList(ddlCntrstp, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContainerTypes).Tables[0]);
                BindDropDownList(ddlInsrncCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                BindDropDownList(ddlFrtCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                BindDropDownList(ddlDscntCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                BindDropDownList(ddlCmsnCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                BindDropDownList(ddlOtrDtcnsCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                BindDropDownList(ddlPkngChrgsCrncy, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);

                if (((Request.QueryString["SbdID"] != null && Request.QueryString["SbdID"] != "") ?
                    new Guid(Request.QueryString["SbdID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["SbdID"].ToString();
                    EditShpngBilDtls(SBDBL.SelectShpngBilDtls(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()),
                        Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.EmptyFACDetails(), new Guid(Session["CompanyID"].ToString())));
                }
                if (Request.QueryString["RefIDs"] != null && Request.QueryString["RefIDs"] != "")
                {
                    string Parmas = StringEncrpt_Decrypt.Decrypt(Request["RefIDs"].ToString().Replace(' ', '+'), true);
                    string[] RefIDs = Parmas.Split(',');
                    if (RefIDs.Length >= 6)
                    {
                        Guid ID = new Guid(RefIDs[0].ToString()), FS_AdrsDtlsID = new Guid(RefIDs[1]),
                            DBK_DtlsID = new Guid(RefIDs[2]), DEPB_DtlsID = new Guid(RefIDs[3]),
                            INVC_DtlsID = new Guid(RefIDs[4]), DA_DtlsID = new Guid(RefIDs[5]);

                        ViewState["ID"] = Request.QueryString["RefIDs"].ToString();
                        EditShpngBilDtls(SBDBL.SelectShpngBilDtls(CommonBLL.FlagModify, ID, ID, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID,
                            Guid.Empty, CommonBLL.EmptyFACDetails(), new Guid(Session["CompanyID"].ToString())));
                    }
                }
                else
                {
                    Session["CntrDtls"] = CommonBLL.EmptyFACDetails();
                    HtmlGenericControl divCntrDtls = new HtmlGenericControl("divCntrDtls");
                    divCntrDtls.ID = "divCntrDtls";
                    divCntrDtls.InnerHtml = FillCntrDtls();
                    Panel2.Controls.Add(divCntrDtls);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Drop Down List
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Box
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox LB, DataTable CommonDt)
        {
            try
            {
                LB.DataSource = CommonDt;
                LB.DataTextField = "Description";
                LB.DataValueField = "ID";
                LB.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Default Inputs from Proforma Invoice Number
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputDtls(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    //txtStOrgn.Text = CommonDt.Tables[0].Rows[0]["StateOfOrigine"].ToString();
                    //txtStfngDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateofStuffing"].ToString()).ToString("dd-MM-yyyy");
                    //txtCntrsNo.Text = CommonDt.Tables[0].Rows[0]["NmbrofCntrs"].ToString();
                    //ddlCntrstp.SelectedValue = CommonDt.Tables[0].Rows[0]["TypeofCntrs"].ToString();
                    //txtRange.Text = CommonDt.Tables[0].Rows[0]["Range"].ToString();
                    //txtDvsn.Text = CommonDt.Tables[0].Rows[0]["Division"].ToString();
                    //txtCmsnrate.Text = CommonDt.Tables[0].Rows[0]["Commissionerate"].ToString();

                    ddlPrtLdng.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLdng"].ToString().Trim().ToLower();
                    ddlPrtDscrg.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDschrg"].ToString().Trim().ToLower();
                    ddlPlcFnlDstn.SelectedValue = CommonDt.Tables[0].Rows[0]["PlcFnlDstn"].ToString().Trim().ToLower();
                    ddlPlcOrgGds.SelectedValue = CommonDt.Tables[0].Rows[0]["PlcOrgnGds"].ToString().Trim().ToLower();
                    txtPfrmInvcNo.Attributes.Add("readonly", "readonly");
                    //txtTtlPkgs.Text = CommonDt.Tables[0].Rows[0]["TotPkgs"].ToString();
                    //txtLsePkgs.Text = CommonDt.Tables[0].Rows[0]["LoosePkts"].ToString();
                    //txtGrsWt.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    //txtNtWt.Text = CommonDt.Tables[0].Rows[0]["NetWeight"].ToString();
                    //txtFobVal.Text = CommonDt.Tables[0].Rows[0]["FOBValueINR"].ToString();
                    //txtNtrCrg.Text = CommonDt.Tables[0].Rows[0]["NatureOfCargo"].ToString();
                    //txtNCntrs.Text = CommonDt.Tables[0].Rows[0]["NmbrOfContainers"].ToString();
                    //txtTDBck.Text = CommonDt.Tables[0].Rows[0]["TotalDrawBackINR"].ToString();
                    //txtSrvcTxRfnd.Text = CommonDt.Tables[0].Rows[0]["ServiceTaxRefundINR"].ToString();
                    //txtDBKScrlNo.Text = CommonDt.Tables[0].Rows[0]["DBKScrollNmbr"].ToString();
                    //txtDbkScrlDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DBKDate"].ToString()).ToString("dd-MM-yyyy");
                    //txtDbkEPCRstat.Text = CommonDt.Tables[0].Rows[0]["EPCopyReceiptStauts"].ToString();
                    //txtLeoDate.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LEODate"].ToString()).ToString("dd-MM-yyyy");
                    //txtExmMrkID.Text = CommonDt.Tables[0].Rows[0]["ExamMarkID"].ToString();
                    //txtMrkDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ExamDate"].ToString()).ToString("dd-MM-yyyy");
                    //txtBnkAcNo.Text = CommonDt.Tables[0].Rows[0]["BankACNmbr"].ToString();
                    //txtAmntRmtdDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["AmntRemittedDate"].ToString()).ToString("dd-MM-yyyy");
                    //txtRemarks.Text = CommonDt.Tables[0].Rows[0]["AmntRemittedRemarks"].ToString();
                    //txtInvcInr.Text = CommonDt.Tables[0].Rows[0]["InvoiceValueINR"].ToString();
                    //txtInvcUsd.Text = CommonDt.Tables[0].Rows[0]["InvoiceValueUSD"].ToString();
                    //txtFobValInr.Text = CommonDt.Tables[0].Rows[0]["FOBValueINR"].ToString();

                    txtPfrmInvcNo.Text = CommonDt.Tables[0].Rows[0]["ShpmntPrfmaInvcNmbr"].ToString();
                    txtPfrmInvcDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString()).ToString("dd-MM-yyyy");

                    //txtNtCn.Text = CommonDt.Tables[0].Rows[0]["NatOfCon"].ToString();
                    //txtFCrInv.Text = CommonDt.Tables[0].Rows[0]["FCurrINV"].ToString();
                    //txtExcngRt.Text = CommonDt.Tables[0].Rows[0]["ExchangeRate"].ToString();
                    //txtInsrncRt.Text = CommonDt.Tables[0].Rows[0]["INSRNS_Rate"].ToString();
                    //ddlInsrncCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["INSRNS_Currency"].ToString();
                    //txtInsrncAmnt.Text = CommonDt.Tables[0].Rows[0]["INSRNS_Amount"].ToString();
                    //txtFrtRt.Text = CommonDt.Tables[0].Rows[0]["FRT_Rate"].ToString();
                    //ddlFrtCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["FRT_Currency"].ToString();
                    //txtFrtAmnt.Text = CommonDt.Tables[0].Rows[0]["FRT_Amount"].ToString();
                    //txtDscntRt.Text = CommonDt.Tables[0].Rows[0]["DSCNT_Rate"].ToString();
                    //ddlDscntCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["DSCNT_Currency"].ToString();
                    //txtDscntAmnt.Text = CommonDt.Tables[0].Rows[0]["DSCNT_Amount"].ToString();
                    //txtCmsnRt.Text = CommonDt.Tables[0].Rows[0]["CMSN_Rate"].ToString();
                    //ddlCmsnCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["CMSN_Currency"].ToString();
                    //txtCmsnAmnt.Text = CommonDt.Tables[0].Rows[0]["CMSN_Amount"].ToString();
                    //txtOtrDtcnsRt.Text = CommonDt.Tables[0].Rows[0]["OTRDTSN_Rate"].ToString();
                    //ddlOtrDtcnsCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["OTRDTSN_Currency"].ToString();
                    //txtOtrDtcnsAmnt.Text = CommonDt.Tables[0].Rows[0]["OTRDTSN_Amount"].ToString();
                    //txtPkngChrgsRt.Text = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Rate"].ToString();
                    //ddlPkngChrgsCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Currency"].ToString();
                    //txtPkngChrgsAmnt.Text = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Amount"].ToString();
                    //txtNtrPmnt.Text = CommonDt.Tables[0].Rows[0]["NatureofPmnt"].ToString();
                    //txtPrdPmnt.Text = CommonDt.Tables[0].Rows[0]["PeriodofPmnt"].ToString();
                    //rbtnFtpMntn.SelectedValue = CommonDt.Tables[0].Rows[0]["FTPMentioned"].ToString();
                    //rbtnInvcAtchmnt.SelectedValue = CommonDt.Tables[0].Rows[0]["Invoice"].ToString();
                    //rbtnPkngLstAtchmnt.SelectedValue = CommonDt.Tables[0].Rows[0]["PackingList"].ToString();
                    //rbtnSdfDclrtnAtchmnt.SelectedValue = CommonDt.Tables[0].Rows[0]["SDFDeclaration"].ToString();
                    //rbtnApdx4ADclrAtchmnt.SelectedValue = CommonDt.Tables[0].Rows[0]["Appendix4ADeclartion"].ToString();
                    //txtLetExptDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LETExportDate"].ToString()).ToString("dd-MM-yyyy");
                    //txtOfcrCstm.Text = CommonDt.Tables[0].Rows[0]["CustomsOfficer"].ToString();
                    //txtShpmntDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateofShipment"].ToString()).ToString("dd-MM-yyyy");
                    txtVslNm.Text = CommonDt.Tables[0].Rows[0]["VslFltNo"].ToString();
                    //txtVygNo.Text = CommonDt.Tables[0].Rows[0]["OVoyageNmbr"].ToString();
                    //txtExptrDEPBItems.Text = CommonDt.Tables[0].Rows[0]["ExpDEPBItems"].ToString();
                    //txtExptrNonDEPBItems.Text = CommonDt.Tables[0].Rows[0]["ExpNonDEPBItems"].ToString();
                    //txtCstmrAcptTFobValDEPBItms.Text = CommonDt.Tables[0].Rows[0]["CstmrAcptedDEPBItems"].ToString();
                    //txtDepbLicNmbr.Text = CommonDt.Tables[0].Rows[0]["LICNmbr"].ToString();
                    //txtDepbLicDate.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LICDate"].ToString()).ToString("dd-MM-yyyy");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Shipping Bill Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditShpngBilDtls(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count >= 2)
                {

                    Session["CntrDtls"] = CommonDt.Tables[1];
                    BindDropDownList(ddlPrfmaInvcNo, CommonDt.Tables[2]);
                    ddlPrfmaInvcNo.SelectedValue = CommonDt.Tables[0].Rows[0]["RefInvcID"].ToString();
                    //ddlPrfmaInvcNo.SelectedValue = CommonDt.Tables[0].Rows[0]["RefInvcID"].ToString();
                    txtLeoNo.Text = CommonDt.Tables[0].Rows[0]["LEONmbr"].ToString();
                    txtLeoDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LEODate"].ToString()).ToString("dd-MM-yyyy");
                    txtShpngBlNo.Text = CommonDt.Tables[0].Rows[0]["ShpngBilNmbr"].ToString();
                    rbtnepcpy.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["SB_Status"].ToString()) == true ? "1" : "0";
                    rbtnArests.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["ARE_Status"].ToString()) == true ? "1" : "0";
                    ddlLUTARNNo.Text = CommonDt.Tables[0].Rows[0]["LUTARNNo"].ToString();                   
                    txtShpngBlDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ShpngBilDate"].ToString()).ToString("dd-MM-yyyy");
                    ddlChaMstr.SelectedValue = CommonDt.Tables[0].Rows[0]["ChaID"].ToString();
                    txtStOrgn.Text = CommonDt.Tables[0].Rows[0]["StateOfOrigine"].ToString();
                    txtLofspNo.Text = CommonDt.Tables[0].Rows[0]["LoFSPNmbr"].ToString();
                    txtLofspDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LoFSPDate"].ToString()).ToString("dd-MM-yyyy");
                    txtStfngDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateofStuffing"].ToString()).ToString("dd-MM-yyyy");
                    txtFileNo.Text = CommonDt.Tables[0].Rows[0]["FileNmbr"].ToString();
                    txtFileDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["FileDate"].ToString()).ToString("dd-MM-yyyy");
                    txtCntrsNo.Text = CommonDt.Tables[0].Rows[0]["NmbrofCntrs"].ToString();
                    ddlCntrstp.SelectedValue = CommonDt.Tables[0].Rows[0]["TypeofCntrs"].ToString();
                    txtRange.Text = CommonDt.Tables[0].Rows[0]["Range"].ToString();
                    txtDvsn.Text = CommonDt.Tables[0].Rows[0]["Division"].ToString();
                    txtCmsnrate.Text = CommonDt.Tables[0].Rows[0]["Commissionerate"].ToString();
                    ddlPrtLdng.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLoading"].ToString();
                    ddlPrtDscrg.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDischarge"].ToString();
                    ddlPlcFnlDstn.SelectedValue = CommonDt.Tables[0].Rows[0]["CntryDestination"].ToString();
                    ddlPlcOrgGds.SelectedValue = CommonDt.Tables[0].Rows[0]["CntryOrigine"].ToString();
                    txtTtlPkgs.Text = CommonDt.Tables[0].Rows[0]["TotPkgs"].ToString();
                    txtLsePkgs.Text = CommonDt.Tables[0].Rows[0]["LoosePkts"].ToString();
                    txtGrsWt.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    txtNtWt.Text = CommonDt.Tables[0].Rows[0]["NetWeight"].ToString();
                    txtFobVal.Text = CommonDt.Tables[0].Rows[0]["FOBValueINR"].ToString();
                    //txtFobValRup.Text = CommonDt.Tables[0].Rows[0]["FobValuerRup"].ToString();
                    txtRtnNo.Text = CommonDt.Tables[0].Rows[0]["RotationNmbr"].ToString();
                    txtRtnDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["RotationDate"].ToString()).ToString("dd-MM-yyyy");
                    txtNtrCrg.Text = CommonDt.Tables[0].Rows[0]["NatureOfCargo"].ToString();
                    txtNCntrs.Text = CommonDt.Tables[0].Rows[0]["NmbrOfContainers"].ToString();
                    txtRbiWvNo.Text = CommonDt.Tables[0].Rows[0]["RBIWaiverNmbr"].ToString();
                    txtRbiWvDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["RBIWaiverDate"].ToString()).ToString("dd-MM-yyyy");
                    txtTDBck.Text = CommonDt.Tables[0].Rows[0]["TotalDrawBackINR"].ToString();
                    txtSrvcTxRfnd.Text = CommonDt.Tables[0].Rows[0]["ServiceTaxRefundINR"].ToString();
                    //txtDBKScrlNo.Text = CommonDt.Tables[0].Rows[0]["DBKScrollNmbr"].ToString();
                    txtDbkScrlDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DBKDate"].ToString()).ToString("dd-MM-yyyy");
                    txtDbkEPCRstat.Text = CommonDt.Tables[0].Rows[0]["EPCopyReceiptStauts"].ToString();
                    txtLeoDate.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LEODate"].ToString()).ToString("dd-MM-yyyy");
                    txtExmMrkID.Text = CommonDt.Tables[0].Rows[0]["ExamMarkID"].ToString();
                    txtMrkDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ExamDate"].ToString()).ToString("dd-MM-yyyy");
                    txtBnkAcNo.Text = CommonDt.Tables[0].Rows[0]["BankACNmbr"].ToString();
                    txtAmntRmtdDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["AmntRemittedDate"].ToString()).ToString("dd-MM-yyyy");
                    txtRemarks.Text = CommonDt.Tables[0].Rows[0]["AmntRemittedRemarks"].ToString();
                    txtDbkYRemarks.Text = CommonDt.Tables[0].Rows[0]["DBK_YRemarks"].ToString();
                    rbtnDBK.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["Is_DDB"].ToString()) == true ? "1" : "0";
                    IsDBK_CheckedChanged();
                    txtbnkscrlno.Text = CommonDt.Tables[0].Rows[0]["DBK_BankScrollNo"].ToString();
                    txtDbkScrolldDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DBK_ScrollDate"].ToString()).ToString("dd-MM-yyyy");
                    txtDbkAmountReceived.Text = CommonDt.Tables[0].Rows[0]["DBK_ReceivedAmount"].ToString();
                    txtremksdbk.Text = CommonDt.Tables[0].Rows[0]["DBK_ActionRemarks"].ToString();
                    txtAction.Text = CommonDt.Tables[0].Rows[0]["DBK_ActionTaken"].ToString();
                    txtInvcInr.Text = CommonDt.Tables[0].Rows[0]["InvoiceValueINR"].ToString();
                    txtInvcUsd.Text = CommonDt.Tables[0].Rows[0]["InvoiceValueUSD"].ToString();
                    txtFobValInr.Text = CommonDt.Tables[0].Rows[0]["InvcFOBValueINR"].ToString();//changed to InvcFOBValueINR to FOBValueINR
                    txtFobValRup.Text = CommonDt.Tables[0].Rows[0]["FobValuerRup"].ToString();
                    txtPfrmInvcNo.Text = CommonDt.Tables[0].Rows[0]["PrfmaInvoiceNmbr"].ToString();
                    txtPfrmInvcDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["PrfmaInvoiceDate"].ToString()).ToString("dd-MM-yyyy");
                    txtNtCn.Text = CommonDt.Tables[0].Rows[0]["NatOfCon"].ToString();
                    txtFCrInv.Text = CommonDt.Tables[0].Rows[0]["FCurrINV"].ToString();
                    txtExcngRt.Text = CommonDt.Tables[0].Rows[0]["ExchangeRate"].ToString();
                    InvcInrVal.InnerText = (Convert.ToDecimal(CommonDt.Tables[0].Rows[0]["ExchangeRate"].ToString()) * Convert.ToDecimal(CommonDt.Tables[0].Rows[0]["FCurrINV"].ToString())).ToString("N");
                    txtInsrncRt.Text = CommonDt.Tables[0].Rows[0]["INSRNS_Rate"].ToString();
                    ddlInsrncCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["INSRNS_Currency"].ToString();
                    txtInsrncAmnt.Text = CommonDt.Tables[0].Rows[0]["INSRNS_Amount"].ToString();
                    txtFrtRt.Text = CommonDt.Tables[0].Rows[0]["FRT_Rate"].ToString();
                    ddlFrtCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["FRT_Currency"].ToString();
                    txtFrtAmnt.Text = CommonDt.Tables[0].Rows[0]["FRT_Amount"].ToString();
                    txtDscntRt.Text = CommonDt.Tables[0].Rows[0]["DSCNT_Rate"].ToString();
                    ddlDscntCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["DSCNT_Currency"].ToString();
                    txtDscntAmnt.Text = CommonDt.Tables[0].Rows[0]["DSCNT_Amount"].ToString();
                    txtCmsnRt.Text = CommonDt.Tables[0].Rows[0]["CMSN_Rate"].ToString();
                    ddlCmsnCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["CMSN_Currency"].ToString();
                    txtCmsnAmnt.Text = CommonDt.Tables[0].Rows[0]["CMSN_Amount"].ToString();
                    txtOtrDtcnsRt.Text = CommonDt.Tables[0].Rows[0]["OTRDTSN_Rate"].ToString();
                    ddlOtrDtcnsCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["OTRDTSN_Currency"].ToString();
                    txtOtrDtcnsAmnt.Text = CommonDt.Tables[0].Rows[0]["OTRDTSN_Amount"].ToString();
                    txtPkngChrgsRt.Text = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Rate"].ToString();
                    ddlPkngChrgsCrncy.SelectedValue = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Currency"].ToString();
                    txtPkngChrgsAmnt.Text = CommonDt.Tables[0].Rows[0]["PKNGCHRGS_Amount"].ToString();
                    txtNtrPmnt.Text = CommonDt.Tables[0].Rows[0]["NatureofPmnt"].ToString();
                    txtPrdPmnt.Text = CommonDt.Tables[0].Rows[0]["PeriodofPmnt"].ToString();
                    rbtnFtpMntn.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["FTPMentioned"].ToString()) == true ? "1" : "0";//CommonDt.Tables[0].Rows[0]["FTPMentioned"].ToString();
                    rbtnInvcAtchmnt.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["Invoice"].ToString()) == true ? "1" : "0";//CommonDt.Tables[0].Rows[0]["Invoice"].ToString();
                    rbtnPkngLstAtchmnt.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["PackingList"].ToString()) == true ? "1" : "0";//CommonDt.Tables[0].Rows[0]["PackingList"].ToString();
                    rbtnSdfDclrtnAtchmnt.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["SDFDeclaration"].ToString()) == true ? "1" : "0";//CommonDt.Tables[0].Rows[0]["SDFDeclaration"].ToString();
                    rbtnApdx4ADclrAtchmnt.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["Appendix4ADeclartion"].ToString()) == true ? "1" : "0";//CommonDt.Tables[0].Rows[0]["Appendix4ADeclartion"].ToString();
                    txtLetExptDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LETExportDate"].ToString()).ToString("dd-MM-yyyy");
                    txtOfcrCstm.Text = CommonDt.Tables[0].Rows[0]["CustomsOfficer"].ToString();
                    txtShpmntDt.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateofShipment"].ToString()).ToString("dd-MM-yyyy");
                    txtVslNm.Text = CommonDt.Tables[0].Rows[0]["VessalName"].ToString();
                    txtVygNo.Text = CommonDt.Tables[0].Rows[0]["OVoyageNmbr"].ToString();
                    txtExptrDEPBItems.Text = CommonDt.Tables[0].Rows[0]["ExpDEPBItems"].ToString();
                    txtExptrNonDEPBItems.Text = CommonDt.Tables[0].Rows[0]["ExpNonDEPBItems"].ToString();
                    txtCstmrAcptTFobValDEPBItms.Text = CommonDt.Tables[0].Rows[0]["CstmrAcptedDEPBItems"].ToString();
                    txtDepbLicNmbr.Text = CommonDt.Tables[0].Rows[0]["LICNmbr"].ToString();
                    txtDepbLicDate.Text = Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["LICDate"].ToString()).ToString("dd-MM-yyyy");
                    ArrayList attms = new ArrayList();
                    attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                    Session["ShpngBilAtchms"] = attms;
                    divListBox.InnerHtml = AttachedFiles();
                    HtmlGenericControl divCntrDtls = new HtmlGenericControl("divCntrDtls");
                    divCntrDtls.ID = "divCntrDtls";
                    divCntrDtls.InnerHtml = FillCntrDtls();
                    //Panel2.Controls.Clear(divCntrDtls);
                    //Panel2.Controls.Add(divCntrDtls);


                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }

        }

        /// <summary>
        /// Clear All Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                ddlPrfmaInvcNo.SelectedIndex = ddlChaMstr.SelectedIndex = ddlPrtLdng.SelectedIndex = ddlPrtDscrg.SelectedIndex = -1;
                ddlPlcFnlDstn.SelectedIndex = ddlPlcOrgGds.SelectedIndex = ddlInsrncCrncy.SelectedIndex = ddlFrtCrncy.SelectedIndex = -1;
                ddlDscntCrncy.SelectedIndex = ddlCmsnCrncy.SelectedIndex = ddlOtrDtcnsCrncy.SelectedIndex = ddlPkngChrgsCrncy.SelectedIndex = -1;
                ddlCntrstp.SelectedIndex = -1;

                rbtnFtpMntn.SelectedIndex = rbtnInvcAtchmnt.SelectedIndex = rbtnPkngLstAtchmnt.SelectedIndex = rbtnSdfDclrtnAtchmnt.SelectedIndex = -1;
                rbtnApdx4ADclrAtchmnt.SelectedIndex = -1;

                txtLeoNo.Text = txtLeoDt.Text = txtShpngBlNo.Text = txtShpngBlDt.Text = txtStOrgn.Text = txtLofspNo.Text = "";
                txtLofspDt.Text = txtStfngDt.Text = txtFileNo.Text = txtFileDt.Text = txtCntrsNo.Text = "";
                txtRange.Text = txtDvsn.Text = txtCmsnrate.Text = txtTtlPkgs.Text = txtLsePkgs.Text = txtGrsWt.Text = txtNtWt.Text = "";
                txtFobVal.Text = txtRtnNo.Text = txtRtnDt.Text = txtNtrCrg.Text = txtNCntrs.Text = txtRbiWvNo.Text = txtRbiWvDt.Text = "";
                txtTDBck.Text = txtSrvcTxRfnd.Text = txtDbkScrlDt.Text = txtDbkEPCRstat.Text = txtLeoDate.Text = "";
                txtExmMrkID.Text = txtMrkDt.Text = txtBnkAcNo.Text = txtAmntRmtdDt.Text = txtInvcInr.Text = ""; //txtRemarks.Text = txtDBKScrlNo.Text =
                txtInvcUsd.Text = txtFobValInr.Text = txtPfrmInvcNo.Text = txtPfrmInvcDt.Text = txtNtCn.Text = txtFCrInv.Text = "";
                txtExcngRt.Text = txtInsrncRt.Text = txtInsrncAmnt.Text = txtFrtRt.Text = txtFrtAmnt.Text = txtDscntRt.Text = "";
                txtDscntAmnt.Text = txtCmsnRt.Text = txtCmsnAmnt.Text = txtOtrDtcnsRt.Text = txtOtrDtcnsAmnt.Text = txtPkngChrgsRt.Text = "";
                txtPkngChrgsAmnt.Text = txtNtrPmnt.Text = txtPrdPmnt.Text = txtLetExptDt.Text = txtOfcrCstm.Text = txtShpmntDt.Text = txtVslNm.Text = "";
                txtVygNo.Text = txtExptrDEPBItems.Text = txtExptrNonDEPBItems.Text = txtCstmrAcptTFobValDEPBItms.Text = txtDepbLicNmbr.Text = "";
                txtDepbLicDate.Text = txtDbkAmountReceived.Text = txtDbkYRemarks.Text = "";

                Session.Remove("CntrDtls");
                Session["CntrDtls"] = CommonBLL.EmptyFACDetails();
                HtmlGenericControl divCntrDtls = new HtmlGenericControl("divCntrDtls");
                divCntrDtls.ID = "divCntrDtls";
                divCntrDtls.InnerHtml = FillCntrDtls();
                Panel2.Controls.Add(divCntrDtls);
                btnSave.Text = "Save";
                Response.Redirect("ShpngBilDtls.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Default Values in Non String Values
        /// </summary>
        protected void SetDefaultValues()
        {
            try
            {
                txtLeoDt.Text = (txtLeoDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtLeoDt.Text);
                txtShpngBlDt.Text = (txtShpngBlDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtShpngBlDt.Text);
                txtShpngBlDt.Text = (txtShpngBlDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtShpngBlDt.Text);
                txtLofspDt.Text = (txtLofspDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtLofspDt.Text);
                txtStfngDt.Text = (txtStfngDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtStfngDt.Text);
                txtFileDt.Text = (txtFileDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtFileDt.Text);
                txtRtnDt.Text = (txtRtnDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtRtnDt.Text);
                txtRbiWvDt.Text = (txtRbiWvDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtRbiWvDt.Text);
                txtDbkScrlDt.Text = (txtDbkScrlDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtDbkScrlDt.Text);
                txtLeoDate.Text = (txtLeoDate.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtLeoDate.Text);
                txtMrkDt.Text = (txtMrkDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtMrkDt.Text);
                txtAmntRmtdDt.Text = (txtAmntRmtdDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtAmntRmtdDt.Text);
                txtDbkScrolldDt.Text = (txtDbkScrolldDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtDbkScrolldDt.Text);
                txtPfrmInvcDt.Text = (txtPfrmInvcDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtPfrmInvcDt.Text);
                txtLetExptDt.Text = (txtLetExptDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtLetExptDt.Text);
                txtShpmntDt.Text = (txtShpmntDt.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtShpmntDt.Text);
                txtDepbLicDate.Text = (txtDepbLicDate.Text == "" ? DateTime.Now.ToString("dd-MM-yyyy") : txtDepbLicDate.Text);

                txtCntrsNo.Text = (txtCntrsNo.Text == "" ? "0" : txtCntrsNo.Text);
                txtTtlPkgs.Text = (txtTtlPkgs.Text == "" ? "0" : txtTtlPkgs.Text);
                txtLsePkgs.Text = (txtLsePkgs.Text == "" ? "0" : txtLsePkgs.Text);
                txtGrsWt.Text = (txtGrsWt.Text == "" ? "0" : txtGrsWt.Text);
                txtNtWt.Text = (txtNtWt.Text == "" ? "0" : txtNtWt.Text);
                txtFobVal.Text = (txtFobVal.Text == "" ? "0" : txtFobVal.Text);
                txtNCntrs.Text = (txtNCntrs.Text == "" ? "0" : txtNCntrs.Text);
                txtTDBck.Text = (txtTDBck.Text == "" ? "0" : txtTDBck.Text);
                txtSrvcTxRfnd.Text = (txtSrvcTxRfnd.Text == "" ? "0" : txtSrvcTxRfnd.Text);
                txtInvcInr.Text = (txtInvcInr.Text == "" ? "0" : txtInvcInr.Text);
                txtInvcUsd.Text = (txtInvcUsd.Text == "" ? "0" : txtInvcUsd.Text);
                txtFobValInr.Text = (txtFobValInr.Text == "" ? "0" : txtFobValInr.Text);
                txtFobValRup.Text = (txtFobValRup.Text == "" ? "0" :txtFobValRup.Text);
                txtFCrInv.Text = (txtFCrInv.Text == "" ? "0" : txtFCrInv.Text);
                txtExcngRt.Text = (txtExcngRt.Text == "" ? "0" : txtExcngRt.Text);
                txtInsrncRt.Text = (txtInsrncRt.Text == "" ? "0" : txtInsrncRt.Text);
                txtInsrncAmnt.Text = (txtInsrncAmnt.Text == "" ? "0" : txtInsrncAmnt.Text);
                txtFrtRt.Text = (txtFrtRt.Text == "" ? "0" : txtFrtRt.Text);
                txtFrtAmnt.Text = (txtFrtAmnt.Text == "" ? "0" : txtFrtAmnt.Text);
                txtDscntRt.Text = (txtDscntRt.Text == "" ? "0" : txtDscntRt.Text);
                txtDscntAmnt.Text = (txtDscntAmnt.Text == "" ? "0" : txtDscntAmnt.Text);
                txtCmsnRt.Text = (txtCmsnRt.Text == "" ? "0" : txtCmsnRt.Text);
                txtCmsnAmnt.Text = (txtCmsnAmnt.Text == "" ? "0" : txtCmsnAmnt.Text);
                txtOtrDtcnsRt.Text = (txtOtrDtcnsRt.Text == "" ? "0" : txtOtrDtcnsRt.Text);
                txtOtrDtcnsAmnt.Text = (txtOtrDtcnsAmnt.Text == "" ? "0" : txtOtrDtcnsAmnt.Text);
                txtPkngChrgsRt.Text = (txtPkngChrgsRt.Text == "" ? "0" : txtPkngChrgsRt.Text);
                txtPkngChrgsAmnt.Text = (txtPkngChrgsAmnt.Text == "" ? "0" : txtPkngChrgsAmnt.Text);
                txtPrdPmnt.Text = (txtPrdPmnt.Text == "" ? "0" : txtPrdPmnt.Text);
                txtExptrDEPBItems.Text = (txtExptrDEPBItems.Text == "" ? "0" : txtExptrDEPBItems.Text);
                txtExptrNonDEPBItems.Text = (txtExptrNonDEPBItems.Text == "" ? "0" : txtExptrNonDEPBItems.Text);
                txtCstmrAcptTFobValDEPBItms.Text = (txtCstmrAcptTFobValDEPBItms.Text == "" ? "0" : txtCstmrAcptTFobValDEPBItms.Text);
                //txtDBKScrlNo.Text = (txtDBKScrlNo.Text == "" ? "0" : txtDBKScrlNo.Text);


                rbtnApdx4ADclrAtchmnt.SelectedValue = rbtnApdx4ADclrAtchmnt.SelectedValue == "" ? "0" : rbtnApdx4ADclrAtchmnt.SelectedValue;
                rbtnFtpMntn.SelectedValue = rbtnFtpMntn.SelectedValue == "" ? "0" : rbtnFtpMntn.SelectedValue;
                rbtnInvcAtchmnt.SelectedValue = rbtnInvcAtchmnt.SelectedValue == "" ? "0" : rbtnInvcAtchmnt.SelectedValue;
                rbtnPkngLstAtchmnt.SelectedValue = rbtnPkngLstAtchmnt.SelectedValue == "" ? "0" : rbtnPkngLstAtchmnt.SelectedValue;
                rbtnSdfDclrtnAtchmnt.SelectedValue = rbtnSdfDclrtnAtchmnt.SelectedValue == "" ? "0" : rbtnSdfDclrtnAtchmnt.SelectedValue;


                DataTable CntrDtls = (DataTable)Session["CntrDtls"];
                var NonEmptyRcrds = from E in CntrDtls.AsEnumerable() where E.Field<Guid>("CntrType") != Guid.Empty select E;
                if ((NonEmptyRcrds) != null)
                    CntrDtls = NonEmptyRcrds.CopyToDataTable<DataRow>();
                Session["CntrDtls"] = CntrDtls;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        protected void IsDBK_CheckedChanged()
        {
            try
            {
                if (rbtnDBK.SelectedIndex != 1)
                {
                    lblScrl.Visible = true;
                    txtbnkscrlno.Visible = true;
                    lblrmks.Visible = false;
                    txtremksdbk.Visible = false;
                    lblActn.Visible = false;
                    txtAction.Visible = false;
                    //lblDBKAmountReceived.Visible = true;
                    //txtDbkAmountReceived.Visible = true;
                    lblDbkYRemarks.Visible = true;
                    txtDbkYRemarks.Visible = true;
                }
                else
                {
                    lblScrl.Visible = false;
                    txtbnkscrlno.Visible = false;
                    lblrmks.Visible = true;
                    txtremksdbk.Visible = true;
                    lblActn.Visible = true;
                    txtAction.Visible = true;
                    //lblDBKAmountReceived.Visible = false;
                    //txtDbkAmountReceived.Visible = false;
                    lblDbkYRemarks.Visible = false;
                    txtDbkYRemarks.Visible = false;
                }
                HtmlGenericControl divCntrDtls = new HtmlGenericControl("divCntrDtls");
                divCntrDtls.ID = "divCntrDtls";
                divCntrDtls.InnerHtml = FillCntrDtls();
                Panel2.Controls.Add(divCntrDtls);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to fill Container Details
        /// </summary>
        /// <returns></returns>
        public string FillCntrDtls()
        {
            string Testing = "";
            StringBuilder sb = new StringBuilder();
            try
            {

                //string[] dateValues = { DateTime.Now.ToString("dd-MM-yyyy"), 
                //                        DateTime.Now.ToString("MM-dd-yyyy"), 
                //                        DateTime.Now.ToString("dd-MM-yy"), 
                //                        DateTime.Now.ToString("dd/MM/yyyy") };
                //string pattern = "MM-dd-yyyy";
                //DateTime parsedDate;

                //foreach (var dateValue in dateValues)
                //{
                //    if (DateTime.TryParseExact(dateValue, pattern, null,
                //                              DateTimeStyles.None, out parsedDate))
                //        Testing += ("Converted " + dateValue + " to " + parsedDate + "." + System.Environment.NewLine);
                //    else
                //        Testing += ("Unable to convert " + dateValue + " to a date and time." + System.Environment.NewLine);
                //}

                DataTable dt;
                if (Session["CntrDtls"] != null && ((DataTable)Session["CntrDtls"]).Rows.Count > 0)
                    dt = (DataTable)Session["CntrDtls"];
                else
                    dt = CommonBLL.EmptyFACDetails();

                string TotalAmt = "";
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' class='rounded-corner' border='0' id='tblCntrDtls' " +
                    "align='center'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>Container Type</th><th>Container No.</th><th>Size</th><th>Seal No.</th><th>Date</th>" +
                    "<th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Testing += dt.Rows[i]["CntrDate"].ToString() + " ,";
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");

                        sb.Append("<td>");
                        # region Bind-DDL
                        sb.Append("<select id='ddl" + (i + 1) + "' onchange='FillItemGrid(" + SNo + "," + i.ToString() + ")' Class='bcAspdropdown' width='50px'>");
                        sb.Append("<option value='" + Guid.Empty.ToString() + "'>-SELECT-</option>");

                        DataSet ds = new DataSet();
                        ds = EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.ContainerTypes);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (dt.Rows[i]["CntrType"].ToString() == row["ID"].ToString())
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                        }
                        sb.Append("</select>");
                        # endregion
                        sb.Append("</td>");

                        sb.Append("<td><input type='text' name='txtCntrNo' class='bcAsptextbox' value='" +
                                dt.Rows[i]["CntrNmbr"].ToString() + "'  id='txtCntrNo" + SNo +
                                "' onkeypress='return isNumberKey(event)' onchange='SaveChanges("
                                + SNo + ")' maxlength='15' style='text-align: right; width: 50px;'/></td>");

                        sb.Append("<td><input type='text' name='txtSize' class='bcAsptextbox' value='" +
                                dt.Rows[i]["CntrSize"].ToString() + "'  id='txtSize" + SNo +
                                "' onkeypress='return isNumberKey(event)' onchange='SaveChanges(" + SNo +
                                ")' maxlength='15' style='text-align: right; width: 50px;'/></td>");

                        sb.Append("<td><input type='text' name='txtSealNo' class='bcAsptextbox' value='" +
                            dt.Rows[i]["CntrSealNmbr"].ToString() + "'  id='txtSealNo" + SNo +
                            "' onchange='SaveChanges(" + SNo + ")'/></td>");

                        ////(DateTime.TryParseExact(dateValue, pattern, null,DateTimeStyles.None, out parsedDate)

                        //DateTime parsedDate;
                        //if (DateTime.TryParseExact(dt.Rows[i]["CntrDate"].ToString(), "dd-MM-yyyy", null, DateTimeStyles.None, out parsedDate))
                        //{ }
                        //else if (DateTime.TryParseExact(dt.Rows[i]["CntrDate"].ToString(), "MM-dd-yyyy", null, DateTimeStyles.None, out parsedDate))
                        //{ }

                        string[] CrtDtStrng = dt.Rows[i]["CntrDate"].ToString().Split(' ');

                        if (CrtDtStrng.Length > 1)
                            sb.Append("<td><input type='text' name='txtDate' readonly='readonly' class='bcAsptextbox DatePicker' value='" +
                                Convert.ToDateTime(dt.Rows[i]["CntrDate"].ToString()).ToString("dd-MM-yyyy") + "'  id='txtDate" + SNo +
                                "' onchange='SaveChanges(" + SNo + ")'/></td>");
                        else
                            sb.Append("<td><input type='text' name='txtDate' readonly='readonly' class='bcAsptextbox DatePicker' value='" +
                                 CommonBLL.DateFormat(dt.Rows[i]["CntrDate"].ToString()).ToString("dd-MM-yyyy") + "'  id='txtDate" + SNo +
                                "' onchange='SaveChanges(" + SNo + ")'/></td>");

                        //sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                        //        " onclick='javascript:return doConfirmCntrDtls(" + SNo +
                        //        ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");
                        if (dt.Rows.Count == 1)
                            sb.Append("<td><a href='javascript:void(0)' onclick='AddNewRow(" + SNo +
                                ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else if (dt.Rows.Count == (i + 1))
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirmCntrDtls(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a>&nbsp;&nbsp;<a href='javascript:void(0)' " +
                                " onclick='AddNewRow(" + SNo +
                                ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirmCntrDtls(" + SNo +
                                ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='5' class='rounded-foot-right'>" +
                        "<input id='HfMessage' type='hidden' name='HfMessage' value=''/></th></tfoot>");
                }
                sb.Append("</tbody></table>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
                return Testing; //string.Empty;
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
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");// +Path.GetFileName(AsyncFileUpload1.FileName);
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["ShpngBilAtchms"] != null)
                        {
                            alist = (ArrayList)Session["ShpngBilAtchms"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["ShpngBilAtchms"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["ShpngBilAtchms"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
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
                if (Session["ShpngBilAtchms"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["ShpngBilAtchms"];
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                    for (int k = 0; k < all.Count; k++)
                        sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                    sb.Append("</select>");
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        #endregion

        #region Drop Down List Selected Index Changed Events

        /// <summary>
        /// Proforma Invoice Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPrfmaInvcNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillInputDtls(SBDBL.SelectShpngBilDtls(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(ddlPrfmaInvcNo.SelectedValue),
                    CommonBLL.EmptyFACDetails(), new Guid(Session["CompanyID"].ToString())));
                HtmlGenericControl divCntrDtls = new HtmlGenericControl("divCntrDtls");
                divCntrDtls.ID = "divCntrDtls";
                divCntrDtls.InnerHtml = FillCntrDtls();
                Panel2.Controls.Add(divCntrDtls);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                SetDefaultValues();

                string Atchmnts = Session["ShpngBilAtchms"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["ShpngBilAtchms"]).Cast<string>().ToArray());

                //DataTable FS_CntrDtls = (DataTable)Session["CntrDtls"];
                //if (FS_CntrDtls != null && FS_CntrDtls.Rows.Count > 0)
                //{
                //    foreach (DataRow Drow in FS_CntrDtls.Rows)
                //    {
                //        string[] CrtDtStrng = Drow["CntrDate"].ToString().Split(' ');
                //        if (CrtDtStrng.Length > 1)
                //            Drow["CntrDate"] = Convert.ToDateTime(Drow["CntrDate"].ToString()).ToString("MM-dd-yyyy");
                //        else
                //            Drow["CntrDate"] = CommonBLL.DateFormat(Drow["CntrDate"].ToString()).ToString("MM-dd-yyyy");
                //    }
                //    FS_CntrDtls.AcceptChanges();
                //}
                //else
                //    FS_CntrDtls = CommonBLL.EmptyFACDetails();

                if (btnSave.Text == "Save")
                {
                    var chk =
                    res = SBDBL.InsertUpdateShpngBilDtls(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlPrfmaInvcNo.SelectedValue),
                        new Guid(ddlPrfmaInvcNo.SelectedValue), txtLeoNo.Text,
                        CommonBLL.DateInsert(txtLeoDt.Text), txtShpngBlNo.Text, CommonBLL.DateInsert(txtShpngBlDt.Text), new Guid(ddlChaMstr.SelectedValue),
                        Convert.ToInt16(rbtnepcpy.SelectedValue), Convert.ToInt16(rbtnArests.SelectedValue), new Guid(ddlLUTARNNo.SelectedValue), txtStOrgn.Text, txtLofspNo.Text, CommonBLL.DateInsert(txtLofspDt.Text), CommonBLL.DateInsert(txtStfngDt.Text), txtFileNo.Text,
                        CommonBLL.DateInsert(txtFileDt.Text), Convert.ToInt64(txtCntrsNo.Text), new Guid(ddlCntrstp.SelectedValue), txtRange.Text,
                        txtDvsn.Text, txtCmsnrate.Text, (DataTable)Session["CntrDtls"], new Guid(ddlPrtLdng.SelectedValue),
                        new Guid(ddlPrtDscrg.SelectedValue),
                        new Guid(ddlPlcFnlDstn.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue), Convert.ToInt64(txtTtlPkgs.Text),
                        Convert.ToInt64(txtLsePkgs.Text), Convert.ToDecimal(txtGrsWt.Text), Convert.ToDecimal(txtNtWt.Text),
                        Convert.ToDecimal(txtFobVal.Text), txtRtnNo.Text, CommonBLL.DateInsert(txtRtnDt.Text), txtNtrCrg.Text,
                        Convert.ToInt64(txtNCntrs.Text), txtRbiWvNo.Text, CommonBLL.DateInsert(txtRbiWvDt.Text), Convert.ToDecimal(txtTDBck.Text),
                        Convert.ToDecimal(txtSrvcTxRfnd.Text), "0", CommonBLL.DateInsert(txtDbkScrlDt.Text), txtDbkEPCRstat.Text, //txtDBKScrlNo.Text
                        CommonBLL.DateInsert(txtLeoDate.Text), txtExmMrkID.Text, CommonBLL.DateInsert(txtMrkDt.Text), txtBnkAcNo.Text,
                        CommonBLL.DateInsert(txtAmntRmtdDt.Text), txtRemarks.Text, Convert.ToInt16(rbtnDBK.SelectedValue), txtDbkAmountReceived.Text, txtbnkscrlno.Text, CommonBLL.DateInsert(txtDbkScrolldDt.Text),
                        txtDbkYRemarks.Text, txtAction.Text, txtremksdbk.Text, Convert.ToDecimal(txtInvcInr.Text), Convert.ToDecimal(txtInvcUsd.Text),
                        Convert.ToDecimal(txtFobValInr.Text), Convert.ToDecimal(txtFobValRup.Text), txtPfrmInvcNo.Text, CommonBLL.DateInsert(txtPfrmInvcDt.Text), txtNtCn.Text,
                        Convert.ToDecimal(txtFCrInv.Text), Convert.ToDecimal(txtExcngRt.Text), Convert.ToDecimal(0),
                        Convert.ToDecimal(txtInsrncRt.Text), new Guid(ddlInsrncCrncy.SelectedValue), Convert.ToDecimal(txtInsrncAmnt.Text),
                        Convert.ToDecimal(txtFrtRt.Text), new Guid(ddlFrtCrncy.SelectedValue), Convert.ToDecimal(txtFrtAmnt.Text),
                        Convert.ToDecimal(txtDscntRt.Text), new Guid(ddlDscntCrncy.SelectedValue), Convert.ToDecimal(txtDscntAmnt.Text),
                        Convert.ToDecimal(txtCmsnRt.Text), new Guid(ddlCmsnCrncy.SelectedValue), Convert.ToDecimal(txtCmsnAmnt.Text),
                        Convert.ToDecimal(txtOtrDtcnsRt.Text), new Guid(ddlOtrDtcnsCrncy.SelectedValue), Convert.ToDecimal(txtOtrDtcnsAmnt.Text),
                        Convert.ToDecimal(txtPkngChrgsRt.Text), new Guid(ddlPkngChrgsCrncy.SelectedValue), Convert.ToDecimal(txtPkngChrgsAmnt.Text),
                        txtNtrPmnt.Text, Convert.ToInt64(txtPrdPmnt.Text), Convert.ToInt16(rbtnFtpMntn.SelectedValue), Convert.ToInt16(rbtnInvcAtchmnt.SelectedValue),
                        Convert.ToInt16(rbtnPkngLstAtchmnt.SelectedValue), Convert.ToInt16(rbtnSdfDclrtnAtchmnt.SelectedValue),
                        Convert.ToInt16(rbtnApdx4ADclrAtchmnt.SelectedValue), CommonBLL.DateInsert(txtLetExptDt.Text), txtOfcrCstm.Text,
                        CommonBLL.DateInsert(txtShpmntDt.Text), txtVslNm.Text, txtVygNo.Text, Convert.ToDecimal(txtExptrDEPBItems.Text),
                        Convert.ToDecimal(txtExptrNonDEPBItems.Text), Convert.ToDecimal(txtCstmrAcptTFobValDEPBItms.Text),
                        txtDepbLicNmbr.Text, CommonBLL.DateInsert(txtDepbLicDate.Text), Atchmnts, "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Shipping Bill Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Shipping Bill Details",
                            "Saved successfully.");
                        ClearInputs();
                        Response.Redirect("ShpngBilStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Shipping Bill Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details",
                            "Error while Saving.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    string Parmas = StringEncrpt_Decrypt.Decrypt(ViewState["ID"].ToString().Replace(' ', '+'), true);
                    string[] RefIDs = Parmas.Split(',');
                    if (RefIDs.Length >= 6)
                    {
                        Guid ID = new Guid(RefIDs[0].ToString()), FS_AdrsDtlsID = new Guid(RefIDs[1]),
                            DBK_DtlsID = new Guid(RefIDs[2]), DEPB_DtlsID = new Guid(RefIDs[3]),
                            INVC_DtlsID = new Guid(RefIDs[4]), DA_DtlsID = new Guid(RefIDs[5]);

                        res = SBDBL.UpdateShpngBilDtls(CommonBLL.FlagUpdate, ID, ID, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID,
                            new Guid(ddlPrfmaInvcNo.SelectedValue), new Guid(ddlPrfmaInvcNo.SelectedValue), txtLeoNo.Text,
                            CommonBLL.DateInsert(txtLeoDt.Text), txtShpngBlNo.Text, CommonBLL.DateInsert(txtShpngBlDt.Text), new Guid(ddlChaMstr.SelectedValue),
                            Convert.ToInt16(rbtnepcpy.SelectedValue), Convert.ToInt16(rbtnArests.SelectedValue), new Guid(ddlLUTARNNo.SelectedValue), txtStOrgn.Text, txtLofspNo.Text, CommonBLL.DateInsert(txtLofspDt.Text), CommonBLL.DateInsert(txtStfngDt.Text), txtFileNo.Text,
                            CommonBLL.DateInsert(txtFileDt.Text), Convert.ToInt64(txtCntrsNo.Text), new Guid(ddlCntrstp.SelectedValue), txtRange.Text,
                            txtDvsn.Text, txtCmsnrate.Text, (DataTable)Session["CntrDtls"], new Guid(ddlPrtLdng.SelectedValue),
                            new Guid(ddlPrtDscrg.SelectedValue),
                            new Guid(ddlPlcFnlDstn.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue), Convert.ToInt64(txtTtlPkgs.Text),
                            Convert.ToInt64(txtLsePkgs.Text), Convert.ToDecimal(txtGrsWt.Text), Convert.ToDecimal(txtNtWt.Text),
                            Convert.ToDecimal(txtFobVal.Text), txtRtnNo.Text, CommonBLL.DateInsert(txtRtnDt.Text), txtNtrCrg.Text,
                            Convert.ToInt64(txtNCntrs.Text), txtRbiWvNo.Text, CommonBLL.DateInsert(txtRbiWvDt.Text), Convert.ToDecimal(txtTDBck.Text),
                            Convert.ToDecimal(txtSrvcTxRfnd.Text), "0", CommonBLL.DateInsert(txtDbkScrlDt.Text), txtDbkEPCRstat.Text, //txtDBKScrlNo.Text
                            CommonBLL.DateInsert(txtLeoDate.Text), txtExmMrkID.Text, CommonBLL.DateInsert(txtMrkDt.Text), txtBnkAcNo.Text,
                            CommonBLL.DateInsert(txtAmntRmtdDt.Text), txtRemarks.Text, Convert.ToInt16(rbtnDBK.SelectedValue), txtDbkAmountReceived.Text, txtbnkscrlno.Text, CommonBLL.DateInsert(txtDbkScrolldDt.Text),
                            txtDbkYRemarks.Text, txtAction.Text, txtremksdbk.Text, Convert.ToDecimal(txtInvcInr.Text),
                            Convert.ToDecimal(txtInvcUsd.Text), Convert.ToDecimal(txtFobValInr.Text), Convert.ToDecimal(txtFobValRup.Text),txtPfrmInvcNo.Text,
                            CommonBLL.DateInsert(txtPfrmInvcDt.Text), txtNtCn.Text,
                            Convert.ToDecimal(txtFCrInv.Text), Convert.ToDecimal(txtExcngRt.Text), Convert.ToDecimal(0),
                            Convert.ToDecimal(txtInsrncRt.Text), new Guid(ddlInsrncCrncy.SelectedValue), Convert.ToDecimal(txtInsrncAmnt.Text),
                            Convert.ToDecimal(txtFrtRt.Text), new Guid(ddlFrtCrncy.SelectedValue), Convert.ToDecimal(txtFrtAmnt.Text),
                            Convert.ToDecimal(txtDscntRt.Text), new Guid(ddlDscntCrncy.SelectedValue), Convert.ToDecimal(txtDscntAmnt.Text),
                            Convert.ToDecimal(txtCmsnRt.Text), new Guid(ddlCmsnCrncy.SelectedValue), Convert.ToDecimal(txtCmsnAmnt.Text),
                            Convert.ToDecimal(txtOtrDtcnsRt.Text), new Guid(ddlOtrDtcnsCrncy.SelectedValue), Convert.ToDecimal(txtOtrDtcnsAmnt.Text),
                            Convert.ToDecimal(txtPkngChrgsRt.Text), new Guid(ddlPkngChrgsCrncy.SelectedValue), Convert.ToDecimal(txtPkngChrgsAmnt.Text),
                            txtNtrPmnt.Text, Convert.ToInt64(txtPrdPmnt.Text), Convert.ToInt16(rbtnFtpMntn.SelectedValue), Convert.ToInt16(rbtnInvcAtchmnt.SelectedValue),
                            Convert.ToInt16(rbtnPkngLstAtchmnt.SelectedValue), Convert.ToInt16(rbtnSdfDclrtnAtchmnt.SelectedValue),
                            Convert.ToInt16(rbtnApdx4ADclrAtchmnt.SelectedValue), CommonBLL.DateInsert(txtLetExptDt.Text), txtOfcrCstm.Text,
                            CommonBLL.DateInsert(txtShpmntDt.Text), txtVslNm.Text, txtVygNo.Text, Convert.ToDecimal(txtExptrDEPBItems.Text),
                            Convert.ToDecimal(txtExptrNonDEPBItems.Text), Convert.ToDecimal(txtCstmrAcptTFobValDEPBItms.Text),
                            txtDepbLicNmbr.Text, CommonBLL.DateInsert(txtDepbLicDate.Text), Atchmnts, txtComments.Text, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    }
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Shipping Bill Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Shipping Bill Details",
                            "Updated successfully.");
                        ClearInputs();
                        Response.Redirect("ShpngBilStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Shipping Bill Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details",
                            "Error while Updating. Result=" + res);
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Radio Button BDK Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbtnDBK_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                IsDBK_CheckedChanged();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
        }



        #endregion

        # region Container Details WebMethods

        /// <summary>
        /// This is used to Delete payment Items
        /// </summary>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CntrDtlsDeleteItem(int rowNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["CntrDtls"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[rowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["CntrDtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
            return FillCntrDtls();
        }

        /// <summary>
        /// This is used to Additems and add new row
        /// </summary>
        /// <param name="rowNo"></param>
        /// <param name="Pay"></param>
        /// <param name="Desc"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CntrDtlsAddItem(int rowNo, string CntrNo, string CntrType, string Size, string SealNo, string Date, bool IsNew)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = (DataTable)Session["CntrDtls"];
                if (dt == null || dt.Rows.Count == 0)
                {
                    dt = CommonBLL.EmptyFACDetails();
                    Session["CntrDtls"] = dt;
                }
                string[] CrtDtStrng = Date.Split(' ');
                if (CrtDtStrng.Length > 1)
                {
                    Date = CrtDtStrng[0].ToString();
                }
                int count = dt.Rows.Count;
                dt.Rows[rowNo - 1]["CntrNmbr"] = CntrNo;
                dt.Rows[rowNo - 1]["CntrType"] = CntrType;
                dt.Rows[rowNo - 1]["CntrSize"] = Size;
                dt.Rows[rowNo - 1]["CntrSealNmbr"] = SealNo;
                dt.Rows[rowNo - 1]["CntrDate"] = CommonBLL.DateInsert(Date);

                if (IsNew)
                {
                    dt.Rows.Add(Guid.Empty, Guid.Empty, 0, Guid.Empty, 0, "", DateTime.Now);//.ToString("MM-dd-yyyy")
                }

                dt.AcceptChanges();
                Session["CntrDtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Details", ex.Message.ToString());
            }
            if (dt != null && dt.Rows.Count > 0)
                Session["CntrDtls"] = dt;
            return FillCntrDtls();
        }

        # endregion

        # region Attachment Details WebMethods

        /// <summary>
        /// This is used to Check the Enquiry Number
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckSHPBNo(string SHPBNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('C', SHPBNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["ShpngBilAtchms"];
                all.RemoveAt(ID);
                Session["ShpngBilAtchms"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }
        # endregion
    }
}