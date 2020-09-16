using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{


    public partial class ExpShipmentdtls : System.Web.UI.Page
    {

        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CommercialINVBLL CIBL = new CommercialINVBLL();
        ExportShipmentDetailsBLL ExBLL = new ExportShipmentDetailsBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string ddlcomminvdt = "";
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                if (!IsPostBack)
                {
                    GetData();

                }
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(CommercialInvoice));
                        //btnSave.Attributes.Add("OnClick");

                        //        if (!IsPostBack)

                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }
        private void GetData()
        {
            try
            {
                txtdate.Attributes.Add("readonly", "readonly");
                TxtDateCargo.Attributes.Add("readonly", "readonly");
                Txtcuststatus.Attributes.Add("readonly", "readonly");
                txtContStuffing.Attributes.Add("readonly", "readonly");
                txtEctnReqDate.Attributes.Add("readonly", "readonly");
                txtECTNInvReceviedDate.Attributes.Add("readonly", "readonly");
                txtECTNPayStatusdate.Attributes.Add("readonly", "readonly");
                txtECTNNoRecDate.Attributes.Add("readonly", "readonly");
                BLPayStatusDate.Attributes.Add("readonly", "readonly");
                txtblappDate.Attributes.Add("readonly", "readonly");
                txtblrelstatusDate.Attributes.Add("readonly", "readonly");
                txtblrecDateHyd.Attributes.Add("readonly", "readonly");
                txtcertOrigFAPCCI.Attributes.Add("readonly", "readonly");
                txtBlAWBapprec.Attributes.Add("readonly", "readonly");
                txtRFIFDIreqon.Attributes.Add("readonly", "readonly");
                txtRFIFDIrecon.Attributes.Add("readonly", "readonly");
                txtBivacinsreq.Attributes.Add("readonly", "readonly");
                txtBivacinscompldon.Attributes.Add("readonly", "readonly");
                BlInvRecDate.Attributes.Add("readonly", "readonly");
                txtAVCOCConigon.Attributes.Add("readonly", "readonly");
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                {
                    //BindDropDownList(ddlCommInvNo, ExBLL.GetDataSet1(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"])).Tables[0]);
                    // ddlCommInvNo.SelectedValue = Request.QueryString["ID"];
                    BindDropDownList1(ddlCommInvNo, ExBLL.GetCommInvData(Convert.ToChar("K")).Tables[0]);
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(ExBLL.GetDataSet1(Convert.ToChar("N"), new Guid(Request.QueryString["ID"].ToString())));
                }
                //ddlCommInvNo.SelectedValue = Request.QueryString["ID"].ToString();
                else
                {
                    BindDropDownList(ddlCommInvNo, ExBLL.GetCommInvData(Convert.ToChar("G")).Tables[0]);

                    //FillInputFields(IOMTBLL.Select(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, ddlRefno.SelectedValue, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }

        }
        protected void ddlCommInvNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCommInvNo.SelectedValue != "0")
                {
                    DataSet CommonDt;
                    CommonDt = ExBLL.GetDataSet1(Convert.ToChar("L"), new Guid(ddlCommInvNo.SelectedValue));
                    txtPerfInvNo.Text = CommonDt.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    txtdate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString()));
                    txtFOBValUSD.Text = CommonDt.Tables[0].Rows[0]["FOBValueINR"].ToString();
                    txtFrtIns.Text = CommonDt.Tables[0].Rows[0]["FreightAmount"].ToString();
                    txtPrtld.Text = CommonDt.Tables[0].Rows[0]["PrtLdng"].ToString();
                    txtPrtDis.Text = CommonDt.Tables[0].Rows[0]["PrtDschrg"].ToString();
                    txtnoofpcks.Text = CommonDt.Tables[0].Rows[0]["TotPkgs"].ToString();
                    txtnetwgtkgs.Text = CommonDt.Tables[0].Rows[0]["NetWeight"].ToString();
                    txtGrosswgtkgs.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    txtshpbllno.Text = CommonDt.Tables[0].Rows[0]["ShpngBilNmbr"].ToString();
                    txtvessdetETAETD.Text = CommonDt.Tables[0].Rows[0]["Vessel"].ToString();
                    TxtPartBLAWB.Text = CommonDt.Tables[0].Rows[0]["Blno"].ToString();
                    txtModeofShpt.Text = CommonDt.Tables[0].Rows[0]["ShipmentMode"].ToString();
                    if (CommonDt.Tables[0].Rows[0]["CNO"].ToString() != null)
                        txtContNo.Text = CommonDt.Tables[0].Rows[0]["CNO"].ToString();
                    TxtConsigneename.Text = CommonDt.Tables[0].Rows[0]["Notify"].ToString();
                }
                else
                {
                    clearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }


        private string GetCommInvDt(String ddlcomminv)
        {

            DataTable CommonDt = ExBLL.GetCommInvData(Convert.ToChar("G")).Tables[0];
            for (int i = 0; i < CommonDt.Rows.Count; i++)
            {
                if (ddlcomminv == CommonDt.Rows[i]["ID"].ToString())
                {
                    string dte = CommonDt.Rows[i]["CommercialInvoiceDate"].ToString();
                    return dte;
                }
            }
            return null;

        }
        private string GetCommInvDt1(String ddlcomminv)
        {

            DataTable CommonDt = ExBLL.GetCommInvData(Convert.ToChar("K")).Tables[0];
            for (int i = 0; i < CommonDt.Rows.Count; i++)
            {
                if (ddlcomminv == CommonDt.Rows[i]["CommercialInvID"].ToString())
                {
                    string dte = CommonDt.Rows[i]["CommercailInvDate"].ToString();
                    return dte;
                }
            }
            return null;

        }
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "CommercialInvoiceNo";
                    ddl.DataValueField = "ID";

                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }


        }
        protected void BindDropDownList1(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "CommercialInvNo";
                    ddl.DataValueField = "CommercialInvID";

                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }


        }



        protected void btnSave_Click(object sender, EventArgs e)
        {
            Filename = FileName();

            if (txtdate.Text == "")
            {
                txtdate.Text = "31-12-9999";
            }
            if (TxtDateCargo.Text == "")
            {
                TxtDateCargo.Text = "31-12-9999";
            }
            if (Txtcuststatus.Text == "")
            {
                Txtcuststatus.Text = "31-12-9999";
            }
            if (txtContStuffing.Text == "")
            {
                txtContStuffing.Text = "31-12-9999";
            }
            if (txtEctnReqDate.Text == "")
            {
                txtEctnReqDate.Text = "31-12-9999";
            }
            if (txtECTNInvReceviedDate.Text == "")
            {
                txtECTNInvReceviedDate.Text = "31-12-9999";
            }
            if (txtECTNPayStatusdate.Text == "")
            {
                txtECTNPayStatusdate.Text = "31-12-9999";
            }
            if (txtECTNPayStatusdate.Text == "")
            {
                txtECTNPayStatusdate.Text = "31-12-9999";
            }
            if (txtECTNNoRecDate.Text == "")
            {
                txtECTNNoRecDate.Text = "31-12-9999";
            }
            if (BLPayStatusDate.Text == "")
            {
                BLPayStatusDate.Text = "31-12-9999";
            }
            if (txtblappDate.Text == "")
            {
                txtblappDate.Text = "31-12-9999";
            }
            if (txtblrelstatusDate.Text == "")
            {
                txtblrelstatusDate.Text = "31-12-9999";
            }
            if (txtblrecDateHyd.Text == "")
            {
                txtblrecDateHyd.Text = "31-12-9999";
            }
            if (txtcertOrigFAPCCI.Text == "")
            {
                txtcertOrigFAPCCI.Text = "31-12-9999";
            }
            if (txtBlAWBapprec.Text == "")
            {
                txtBlAWBapprec.Text = "31-12-9999";
            }
            if (txtRFIFDIreqon.Text == "")
            {
                txtRFIFDIreqon.Text = "31-12-9999";
            }
            if (txtRFIFDIrecon.Text == "")
            {
                txtRFIFDIrecon.Text = "31-12-9999";
            }
            if (txtBivacinsreq.Text == "")
            {
                txtBivacinsreq.Text = "31-12-9999";
            }
            if (txtBivacinscompldon.Text == "")
            {
                txtBivacinscompldon.Text = "31-12-9999";
            }
            if (txtAVCOCConigon.Text == "")
            {
                txtAVCOCConigon.Text = "31-12-9999";
            }
            if (BlInvRecDate.Text == "")
            {
                BlInvRecDate.Text = "31-12-9999";
            }
            if (txtFOBValUSD.Text == "")
            {
                txtFOBValUSD.Text = "0.00";
            }
            if (txtFrtIns.Text == "")
            {
                txtFrtIns.Text = "0.00";
            }
            if (txtCFRCIFVal.Text == "")
            {
                txtCFRCIFVal.Text = "0.00";
            }
            if (txtnoofpcks.Text == "")
            {
                txtnoofpcks.Text = "0";
            }
            if (txtnetwgtkgs.Text == "")
            {
                txtnetwgtkgs.Text = "0.00";
            }
            if (txtGrosswgtkgs.Text == "")
            {
                txtGrosswgtkgs.Text = "0.00";
            }

            if (btnSave.Text == "Save")
            {
                res = ExBLL.InsertUpdateDeleteExpShipDlts(Convert.ToChar("I"), new Guid(ddlCommInvNo.SelectedValue), ddlCommInvNo.SelectedItem.ToString(), DateTime.Parse(GetCommInvDt(ddlCommInvNo.SelectedValue)),
  TxtConsigneename.Text, TxtBivac.Text, TxtSupplier.Text, txtPerfInvNo.Text, CommonBLL.DateInsert(txtdate.Text), txtModeofShpt.Text, decimal.Parse(txtFOBValUSD.Text), decimal.Parse(txtFrtIns.Text), decimal.Parse(txtCFRCIFVal.Text), txtPrtld.Text, txtPrtDis.Text, int.Parse(txtnoofpcks.Text), decimal.Parse(txtnetwgtkgs.Text), decimal.Parse(txtGrosswgtkgs.Text), txtshpbllno.Text, CommonBLL.DateInsert(TxtDateCargo.Text), CommonBLL.DateInsert(Txtcuststatus.Text), txtContNo.Text, CommonBLL.DateInsert(txtContStuffing.Text), txtvessdetETAETD.Text, TxtPartBLAWB.Text, ParticularsofEctnUrn.Text, CommonBLL.DateInsert(txtEctnReqDate.Text), CommonBLL.DateInsert(txtECTNInvReceviedDate.Text), CommonBLL.DateInsert(txtECTNPayStatusdate.Text), CommonBLL.DateInsert(txtECTNNoRecDate.Text), CommonBLL.DateInsert(BLPayStatusDate.Text), CommonBLL.DateInsert(txtblappDate.Text), CommonBLL.DateInsert(txtblrelstatusDate.Text), CommonBLL.DateInsert(txtblrecDateHyd.Text), txtCommInvDet.Text, CommonBLL.DateInsert(txtcertOrigFAPCCI.Text), txtstatusCommInv.Text, txtDetailsreqConsignee.Text, CommonBLL.DateInsert(txtBlAWBapprec.Text), CommonBLL.DateInsert(txtRFIFDIreqon.Text), CommonBLL.DateInsert(txtRFIFDIrecon.Text), CommonBLL.DateInsert(txtBivacinsreq.Text), CommonBLL.DateInsert(txtBivacinscompldon.Text), InvCourierDet.Text, CommonBLL.DateInsert(BlInvRecDate.Text), txtContStuffStat.Text, txtETCNPayStat.Text, txtBLpayStat.Text, txtBLRelStat.Text, CommonBLL.DateInsert(txtAVCOCConigon.Text), txtRemarks.Text, new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, CommonBLL.DateInsert(txtAVCOCConigon.Text), true);
                //Console.WriteLine("Hi");
                if (res == 0)
                {
                    ALS.AuditLog(res, btnSave.Text, "", "ExpShipmentdtls", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    Response.Redirect("../Invoices/ExpShipDtlsStatus.aspx", false);
                }
                else
                {
                    ALS.AuditLog(res, btnSave.Text, "", "ExpShipmentdtls", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                }
            }

            else if (btnSave.Text == "Update")
            {
                res = ExBLL.InsertUpdateDeleteExpShipDlts(Convert.ToChar("U"), new Guid(ddlCommInvNo.SelectedValue), ddlCommInvNo.SelectedItem.ToString(), DateTime.Parse(GetCommInvDt1(ddlCommInvNo.SelectedValue)),
     TxtConsigneename.Text, TxtBivac.Text, TxtSupplier.Text, txtPerfInvNo.Text, CommonBLL.DateInsert(txtdate.Text), txtModeofShpt.Text, decimal.Parse(txtFOBValUSD.Text), decimal.Parse(txtFrtIns.Text), decimal.Parse(txtCFRCIFVal.Text), txtPrtld.Text, txtPrtDis.Text, int.Parse(txtnoofpcks.Text), decimal.Parse(txtnetwgtkgs.Text), decimal.Parse(txtGrosswgtkgs.Text), txtshpbllno.Text, CommonBLL.DateInsert(TxtDateCargo.Text), CommonBLL.DateInsert(Txtcuststatus.Text), txtContNo.Text, CommonBLL.DateInsert(txtContStuffing.Text), txtvessdetETAETD.Text, TxtPartBLAWB.Text, ParticularsofEctnUrn.Text, CommonBLL.DateInsert(txtEctnReqDate.Text), CommonBLL.DateInsert(txtECTNInvReceviedDate.Text), CommonBLL.DateInsert(txtECTNPayStatusdate.Text), CommonBLL.DateInsert(txtECTNNoRecDate.Text), CommonBLL.DateInsert(BLPayStatusDate.Text), CommonBLL.DateInsert(txtblappDate.Text), CommonBLL.DateInsert(txtblrelstatusDate.Text), CommonBLL.DateInsert(txtblrecDateHyd.Text), txtCommInvDet.Text, CommonBLL.DateInsert(txtcertOrigFAPCCI.Text), txtstatusCommInv.Text, txtDetailsreqConsignee.Text, CommonBLL.DateInsert(txtBlAWBapprec.Text), CommonBLL.DateInsert(txtRFIFDIreqon.Text), CommonBLL.DateInsert(txtRFIFDIrecon.Text), CommonBLL.DateInsert(txtBivacinsreq.Text), CommonBLL.DateInsert(txtBivacinscompldon.Text), InvCourierDet.Text, CommonBLL.DateInsert(BlInvRecDate.Text), txtContStuffStat.Text, txtETCNPayStat.Text, txtBLpayStat.Text, txtBLRelStat.Text, CommonBLL.DateInsert(txtAVCOCConigon.Text), txtRemarks.Text, new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, CommonBLL.DateInsert(txtAVCOCConigon.Text), true);
                if (res == 0)
                {
                    ALS.AuditLog(res, btnSave.Text, "", "ExpShipmentdtls", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    Response.Redirect("../Invoices/ExpShipDtlsStatus.aspx", false);
                }
                else
                {
                    ALS.AuditLog(res, btnSave.Text, "", "ExpShipmentdtls", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                }
            }

        }

        private void clearInputs()
        {
            ddlCommInvNo.SelectedIndex = -1;
            TxtConsigneename.Text = TxtBivac.Text = TxtSupplier.Text = txtPerfInvNo.Text = txtdate.Text = txtModeofShpt.Text = txtFOBValUSD.Text = "";
            txtFrtIns.Text = txtCFRCIFVal.Text = txtPrtld.Text = txtPrtDis.Text = txtnoofpcks.Text = txtnetwgtkgs.Text = txtGrosswgtkgs.Text = "";
            txtshpbllno.Text = TxtDateCargo.Text = Txtcuststatus.Text = txtContNo.Text = txtContStuffing.Text = txtvessdetETAETD.Text = "";
            TxtPartBLAWB.Text = ParticularsofEctnUrn.Text = txtEctnReqDate.Text = txtECTNInvReceviedDate.Text = txtECTNPayStatusdate.Text = "";
            txtECTNNoRecDate.Text = BLPayStatusDate.Text = txtblappDate.Text = txtblrelstatusDate.Text = txtblrecDateHyd.Text = txtCommInvDet.Text = "";
            txtcertOrigFAPCCI.Text = txtstatusCommInv.Text = txtDetailsreqConsignee.Text = txtBlAWBapprec.Text = txtRFIFDIreqon.Text = "";
            txtRFIFDIrecon.Text = txtBivacinsreq.Text = txtBivacinscompldon.Text = InvCourierDet.Text = BlInvRecDate.Text = txtContStuffStat.Text = "";
            txtETCNPayStat.Text = txtETCNPayStat.Text = txtBLRelStat.Text = txtBLpayStat.Text = txtAVCOCConigon.Text = txtRemarks.Text = "";
        }
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }
        /// <summary>
        /// Edit Record
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                //ddlRefno.Enabled = false;
                ViewState["ID"] = CommonDt.Tables[0].Rows[0]["CommercialInvID"].ToString();
                ddlCommInvNo.SelectedIndex = ddlCommInvNo.Items.IndexOf(ddlCommInvNo.Items.FindByText(
CommonDt.Tables[0].Rows[0]["CommercialInvNo"].ToString()));
                TxtConsigneename.Text = CommonDt.Tables[0].Rows[0]["ConsigneeName"].ToString();
                TxtBivac.Text = CommonDt.Tables[0].Rows[0]["BivacPreShiptInspDetails"].ToString();
                TxtSupplier.Text = CommonDt.Tables[0].Rows[0]["SuppCargoDesc"].ToString();
                txtPerfInvNo.Text = CommonDt.Tables[0].Rows[0]["PerfInvNo"].ToString();
                txtdate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["PerfInvDate"].ToString()));
                if (txtdate.Text == "31-12-9999")
                {
                    txtdate.Text = "";
                }
                txtModeofShpt.Text = CommonDt.Tables[0].Rows[0]["ModeofShpt"].ToString();
                txtFOBValUSD.Text = CommonDt.Tables[0].Rows[0]["FobValueUSD"].ToString();
                txtFrtIns.Text = CommonDt.Tables[0].Rows[0]["FreightIns"].ToString();
                txtCFRCIFVal.Text = CommonDt.Tables[0].Rows[0]["CFRCIFValue"].ToString();
                txtPrtld.Text = CommonDt.Tables[0].Rows[0]["PortofLoading"].ToString();
                txtPrtDis.Text = CommonDt.Tables[0].Rows[0]["PortofDisc"].ToString();
                txtnoofpcks.Text = CommonDt.Tables[0].Rows[0]["Noofpkgs"].ToString();
                txtnetwgtkgs.Text = CommonDt.Tables[0].Rows[0]["NetWeightinKgs"].ToString();
                txtGrosswgtkgs.Text = CommonDt.Tables[0].Rows[0]["GrossWeightinKgs"].ToString();
                txtshpbllno.Text = CommonDt.Tables[0].Rows[0]["ShippBillNo"].ToString();
                TxtDateCargo.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateofCargoCartatCFS"].ToString()));
                if (TxtDateCargo.Text == "31-12-9999")
                {
                    TxtDateCargo.Text = "";
                }
                Txtcuststatus.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CustsExamStatus"].ToString()));
                if (Txtcuststatus.Text == "31-12-9999")
                {
                    Txtcuststatus.Text = "";
                }
                txtContNo.Text = CommonDt.Tables[0].Rows[0]["ContainerNo"].ToString();
                txtContStuffing.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ContainerStuffingDate"].ToString()));
                if (txtContStuffing.Text == "31-12-9999")
                {
                    txtContStuffing.Text = "";
                }
                txtvessdetETAETD.Text = CommonDt.Tables[0].Rows[0]["VesselDetailsETAETD"].ToString();
                TxtPartBLAWB.Text = CommonDt.Tables[0].Rows[0]["PartofBLAWB"].ToString();
                ParticularsofEctnUrn.Text = CommonDt.Tables[0].Rows[0]["PartofECTNURNID"].ToString();
                txtEctnReqDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ECTNReqDate"].ToString()));
                if (txtEctnReqDate.Text == "31-12-9999")
                {
                    txtEctnReqDate.Text = "";
                }
                txtECTNInvReceviedDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ECTNInvRecDate"].ToString()));
                if (txtECTNInvReceviedDate.Text == "31-12-9999")
                {
                    txtECTNInvReceviedDate.Text = "";
                }
                txtECTNPayStatusdate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ECTNPayDate"].ToString()));
                if (txtECTNPayStatusdate.Text == "31-12-9999")
                {
                    txtECTNPayStatusdate.Text = "";
                }
                txtECTNNoRecDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ECTNNoRecDate"].ToString()));
                if (txtECTNNoRecDate.Text == "31-12-9999")
                {
                    txtECTNNoRecDate.Text = "";
                }
                BLPayStatusDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLPayDate"].ToString()));
                if (BLPayStatusDate.Text == "31-12-9999")
                {
                    BLPayStatusDate.Text = "";
                }
                txtblappDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLAppDate"].ToString()));
                if (txtblappDate.Text == "31-12-9999")
                {
                    txtblappDate.Text = "";
                }
                txtblrelstatusDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLRelDate"].ToString()));
                if (txtblrelstatusDate.Text == "31-12-9999")
                {
                    txtblrelstatusDate.Text = "";
                }
                txtblrecDateHyd.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLRecDateAtHYD"].ToString()));
                if (txtblrecDateHyd.Text == "31-12-9999")
                {
                    txtblrecDateHyd.Text = "";
                }
                txtCommInvDet.Text = CommonDt.Tables[0].Rows[0]["CommInvDetails"].ToString();
                txtcertOrigFAPCCI.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CerfofOriginFAPCCIDate"].ToString()));
                if (txtcertOrigFAPCCI.Text == "31-12-9999")
                {
                    txtcertOrigFAPCCI.Text = "";
                }
                txtstatusCommInv.Text = CommonDt.Tables[0].Rows[0]["StatusofCommInv"].ToString();
                txtDetailsreqConsignee.Text = CommonDt.Tables[0].Rows[0]["DOCDetailsReqConsignee"].ToString();
                txtBlAWBapprec.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLAWBapprecon"].ToString()));
                if (txtBlAWBapprec.Text == "31-12-9999")
                {
                    txtBlAWBapprec.Text = "";
                }
                txtRFIFDIreqon.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["RFIFDIformreqon"].ToString()));
                if (txtRFIFDIreqon.Text == "31-12-9999")
                {
                    txtRFIFDIreqon.Text = "";
                }
                txtRFIFDIrecon.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["RFIFDIformrecon"].ToString()));
                if (txtRFIFDIrecon.Text == "31-12-9999")
                {
                    txtRFIFDIrecon.Text = "";
                }
                txtBivacinsreq.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BIVACPreshipinspreqon"].ToString()));
                if (txtBivacinsreq.Text == "31-12-9999")
                {
                    txtBivacinsreq.Text = "";
                }
                txtBivacinscompldon.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BIVACPreshipinspcomptdon"].ToString()));
                if (txtBivacinscompldon.Text == "31-12-9999")
                {
                    txtBivacinscompldon.Text = "";
                }
                InvCourierDet.Text = CommonDt.Tables[0].Rows[0]["ExpInvcourierdlts"].ToString();
                BlInvRecDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["BLInvRecDate"].ToString()));
                if (BlInvRecDate.Text == "31-12-9999")
                {
                    BlInvRecDate.Text = "";
                }
                txtContStuffStat.Text = CommonDt.Tables[0].Rows[0]["ContStuffStat"].ToString();
                txtETCNPayStat.Text = CommonDt.Tables[0].Rows[0]["ETCNPayStat"].ToString();
                txtBLpayStat.Text = CommonDt.Tables[0].Rows[0]["BLPayStat"].ToString();
                txtBLRelStat.Text = CommonDt.Tables[0].Rows[0]["BLRelStat"].ToString();
                txtAVCOCConigon.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["AVCoCConsigneeon"].ToString()));
                if (txtAVCOCConigon.Text == "31-12-9999")
                {
                    txtAVCOCConigon.Text = "";
                }
                txtRemarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();
                //txtSbno.Text = CommonDt.Tables[0].Rows[0]["Sbno"].ToString();
                //txtVessel.Text = CommonDt.Tables[0].Rows[0]["Vessel"].ToString();
                //txtSpmntInvcNo.Text = CommonDt.Tables[0].Rows[0]["CommercialInvoiceNo"].ToString();
                //txtSpmntInvcDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CommercialInvoiceDate"].ToString()));
                //txtNotify.Text = CommonDt.Tables[0].Rows[0]["Notify"].ToString();
                btnSave.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ExpShipmentdtls", ex.Message.ToString());
            }
        }

        protected void btnclear_Click(object sender, EventArgs e)
        {
            try
            {
                clearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "ExpShipmentdtls", ex.Message.ToString());
            }
        }


    }























}