using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class e_BRC_Details : System.Web.UI.Page
    {

        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        ShpngBilDtlsBLL SBDBL = new ShpngBilDtlsBLL();
        BRCDetailsBLL BRCBLL = new BRCDetailsBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
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
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(e_BRC_Details));
                        txtUplDt.Attributes.Add("readonly", "readonly");
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
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
        /// Default Page Load Data
        /// </summary>
        private void GetData()
        {
            try
            {
                LblTotAmount.Text = "Total Amt(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                txtDate.Attributes.Add("readonly", "readonly");
                txtRealDt.Attributes.Add("readonly", "readonly");
                txtTotAmt.Attributes.Add("readonly", "readonly");//declaring amount value as readonly mode 
               // txtDedVal.Attributes.Add("readonly", "readonly");//declaring as readonly mode in decucted value 
                BindDropDownList(ddlShipBillNo, SBDBL.SelectShpngBilDtls(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, CommonBLL.EmptyFACDetails(), new Guid(Session["CompanyID"].ToString())).Tables[0]);
                BindDropDownList(ddlportloading, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading).Tables[0]);
                BindDropDownList(ddlcurrency, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Currency).Tables[0]);
                if (((Request.QueryString["PinvcID"] != null && Request.QueryString["PinvcID"] != "") ?
                    new Guid(Request.QueryString["PinvcID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlShipBillNo.SelectedValue = Request.QueryString["PinvcID"].ToString();
                    FillInputFields(BRCBLL.GetDataSet(CommonBLL.FlagCommonMstr, Guid.Empty, new Guid(ddlShipBillNo.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["EditID"] = Request.QueryString["ID"].ToString();
                    EditEBRCDtls(BRCBLL.GetDataSet(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    lb.DataSource = CommonDt;
                    lb.DataTextField = "Description";
                    lb.DataValueField = "ID";
                    lb.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw 
        /// </summary>
        /// <param name="gvPfrmaInvce"></param>
        /// <param name="dataTable"></param>
        private void BindGridVeiw(GridView gv, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    gv.DataSource = CommonDt;
                    gv.DataBind();
                }
                else
                {
                    gv.DataSource = null;
                    gv.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields from Check List Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputFields(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                   
                    ddlportloading.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLoading"].ToString();
                    txtDate.Text = CommonDt.Tables[0].Rows[0]["ShpingDT"].ToString();
                    txtTotAmt.Text = CommonDt.Tables[1].Rows[0]["InvoiceUsd"].ToString();// by getting amount in add by manju 
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Proforma Invoice Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditEBRCDtls(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    if (CommonDt != null && CommonDt.Tables.Count > 0)
                    {
                        ddlShipBillNo.SelectedValue = CommonDt.Tables[0].Rows[0]["ShpingBillID"].ToString();
                        ddlportloading.SelectedValue = CommonDt.Tables[0].Rows[0]["ShpingBillPort"].ToString();
                        txtDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["Date"].ToString()));
                        txtbankfileno.Text = CommonDt.Tables[0].Rows[0]["BanksFileNo"].ToString();

                        txtbillidno.Text = CommonDt.Tables[0].Rows[0]["BillIDNo"].ToString();
                        txtBRCNo.Text = CommonDt.Tables[0].Rows[0]["BRCNo"].ToString();
                        txtRealVal.Text = CommonDt.Tables[0].Rows[0]["RealisedValue"].ToString();
                        txtDedVal.Text = CommonDt.Tables[0].Rows[0]["DeductedValue"].ToString();
                        txtTotAmt.Text = CommonDt.Tables[0].Rows[0]["TotalAmount"].ToString();
                        txtRealDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DateRealised"].ToString()));
                        txtUplDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["uploadDate"].ToString()));
                        ddlcurrency.SelectedValue = CommonDt.Tables[0].Rows[0]["Currency"].ToString();
                        ddlBRCStatus.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["BRCStatus"].ToString()) == true ? "1" : "2";
                        ddlBRCUtlStatus.SelectedValue = Convert.ToBoolean(CommonDt.Tables[0].Rows[0]["BRCUtiliseStat"].ToString()) == true ? "1" : "2";
                        txtRemarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();

                        //if (CommonDt.Tables.Count > 1)
                        //{
                        //    gv_CntrDtls.DataSource = CommonDt.Tables[1];
                        //    gv_CntrDtls.DataBind();
                        //}

                        //if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        //{
                        //    ArrayList attms = new ArrayList();
                        //    attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        //    Session["MateRcpt"] = attms;
                        //    divListBox.InnerHtml = AttachedFiles();
                        //    imgAtchmt.Visible = true;
                        //}
                        //else
                        //    imgAtchmt.Visible = false;

                        //DivComments.Visible = true;
                        ViewState["EditID"] = Request.QueryString["ID"];
                        DivComments.Visible = true;
                        btnSave.Text = "Update";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Details
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                ddlShipBillNo.SelectedIndex = ddlBRCStatus.SelectedIndex = ddlBRCUtlStatus.SelectedIndex = -1;
                ddlcurrency.SelectedIndex = ddlportloading.SelectedIndex = -1;
                txtbankfileno.Text = txtbillidno.Text = txtBRCNo.Text = txtRealVal.Text = txtDate.Text = "";
                txtDedVal.Text = txtTotAmt.Text = txtRemarks.Text = txtRealDt.Text = txtUplDt.Text = "";

                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// Check List Drop Down List Selected Index Chnged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlShipBillNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlShipBillNo.SelectedIndex > 0)
                    FillInputFields(BRCBLL.GetDataSet(CommonBLL.FlagXSelect, Guid.Empty, new Guid(ddlShipBillNo.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                else
                {
                    ddlportloading.SelectedIndex = -1;
                    txtDate.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }

        protected void ddlShipBillNo_clear(object sender, EventArgs e)
        {
            try
            {
                if (ddlShipBillNo.SelectedIndex > 0)
                    ddlcurrency.SelectedIndex = ddlBRCStatus.SelectedIndex = ddlBRCUtlStatus.SelectedIndex = -1;
                txtbankfileno.Text = txtbillidno.Text = txtBRCNo.Text = txtRealVal.Text = txtDedVal.Text = txtRealDt.Text = txtUplDt.Text = txtUplDt.Text = txtRealDt.Text = txtRemarks.Text = txtDate.Text = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "e-BRC Details", ex.Message.ToString());
            }
        }
        
        #endregion

        #region Button Clicks

        protected void btnSave_Click1(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    res = BRCBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlShipBillNo.SelectedValue),
                            new Guid(ddlportloading.SelectedValue), CommonBLL.DateInsert(txtDate.Text), txtbankfileno.Text, CommonBLL.DateInsert(txtUplDt.Text),
                            txtbillidno.Text, txtBRCNo.Text, Convert.ToDecimal(txtRealVal.Text), Convert.ToDecimal(txtDedVal.Text), Convert.ToDecimal(txtTotAmt.Text),
                            CommonBLL.DateInsert(txtRealDt.Text), new Guid(ddlcurrency.SelectedValue),
                            ddlBRCStatus.SelectedValue == "1" ? true : false, ddlBRCUtlStatus.SelectedValue == "1" ? true : false, txtRemarks.Text,
                            new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), Guid.Empty,
                            CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, "", new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "E-BRC Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "E-BRC Details", "Data inserted successfully.");
                        ClearInputs();
                        Response.Redirect("eBRCStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "E-BRC Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", "Error while Saving.");
                    }
                }
                else
                {
                    res = BRCBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), new Guid(ddlShipBillNo.SelectedValue),
                        new Guid(ddlportloading.SelectedValue), CommonBLL.DateInsert(txtDate.Text), txtbankfileno.Text, CommonBLL.DateInsert(txtUplDt.Text),
                        txtbillidno.Text, txtBRCNo.Text, Convert.ToDecimal(txtRealVal.Text), Convert.ToDecimal(txtDedVal.Text), Convert.ToDecimal(txtTotAmt.Text),
                        CommonBLL.DateInsert(txtRealDt.Text), new Guid(ddlcurrency.SelectedValue),
                        ddlBRCStatus.SelectedValue == "1" ? true : false, ddlBRCUtlStatus.SelectedValue == "1" ? true : false, txtRemarks.Text,
                        Guid.Empty, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), new Guid(Session["UserID"].ToString()),
                        CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, txtComments.Text.Trim(), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "E-BRC Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "E-BRC Details", "Updated successfully.");
                        ClearInputs();
                        Response.Redirect("eBRCStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "E-BRC Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "E-BRC Details", ex.Message.ToString());
            }
        }

        #endregion

        # region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckEBRCNo(string EBRCNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckLQEnquiryNo('N', EBRCNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        # endregion
    }
}