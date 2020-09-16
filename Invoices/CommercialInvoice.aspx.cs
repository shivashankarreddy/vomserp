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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class CommercialInvoice : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        InvoiceBLL INBLL = new InvoiceBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        IOMTemplate2BLL IOMTBLL = new IOMTemplate2BLL();
        CommercialINVBLL CIBL = new CommercialINVBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string ChkListId = "";
        #endregion

        #region Default Page Load Event

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(CommercialInvoice));
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        #endregion Default Page Load Event

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
        /// Get Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                gvCmrclInvce.Columns[11].HeaderText = "Rate(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                gvCmrclInvce.Columns[12].HeaderText = "Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")"; 
                BindDropDownList(ddlRefno, IOMTBLL.Select(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[0]);

                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                {
                    BindDropDownList(ddlRefno, CIBL.SelectCmrclInvc(CommonBLL.FlagBSelect, new Guid(Request.QueryString["ID"]), Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), "").Tables[0]);
                    ddlRefno.SelectedValue = Request.QueryString["ID"].ToString().ToLowerInvariant();
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(CIBL.SelectCmrclInvc(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlRefno.SelectedValue), new Guid(Session["CompanyID"].ToString()),
                        new Guid(Session["UserID"].ToString()),""));
                    DataSet CmrclInvcItms = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagModify, new Guid(ddlRefno.SelectedValue), Guid.Empty, "", new Guid(Session["CompanyID"].ToString()));
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables.Count > 0)
                        ChkListId = CmrclInvcItms.Tables[0].Rows[0]["ChkListID"].ToString();
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables[5].Rows.Count > 0 && Convert.ToBoolean(CmrclInvcItms.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        CmrclInvcItms = CLBLL.SelectChekcListDtls(CommonBLL.FlagSelectAll, new Guid(ChkListId), "", new Guid(Session["CompanyID"].ToString()));
                        gvCmrclInvce.DataSource = CmrclInvcItms.Tables[1];
                        gvCmrclInvce.DataBind();
                    }
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables.Count > 3)
                    {
                        gvCmrclInvce.DataSource = CmrclInvcItms.Tables[3];
                        gvCmrclInvce.DataBind();
                    }
                }

                else if (Request.QueryString["ddlRefno"] != null && Request.QueryString[""] != "")
                {
                    FillInputFields(IOMTBLL.Select(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, ddlRefno.SelectedValue, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                    ddl.DataTextField = "PrfmInvcNo";
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

        /// <summary>
        /// Fill Inputs Fields
        /// </summary>
        /// <param name="CommonDt"></param>
        private void FillInputFields(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    txtBlno.Text = CommonDt.Tables[0].Rows[0]["Blno"].ToString();
                    txtSbno.Text = CommonDt.Tables[0].Rows[0]["Sbno"].ToString();
                    txtVessel.Text = CommonDt.Tables[0].Rows[0]["Vessel"].ToString();
                    txtSpmntInvcNo.Text = CommonDt.Tables[0].Rows[0]["CommercialInvoiceNo"].ToString();
                    if (CommonDt.Tables[0].Rows[0]["CommercialInvoiceDate"].ToString() != "")
                        txtSpmntInvcDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CommercialInvoiceDate"].ToString()));
                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Check List does't have P.Invoice or Packing List.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Record
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                ddlRefno.Enabled = false;
                ViewState["ID"] = CommonDt.Tables[0].Rows[0]["ID"].ToString();
                txtBlno.Text = CommonDt.Tables[0].Rows[0]["Blno"].ToString();
                txtSbno.Text = CommonDt.Tables[0].Rows[0]["Sbno"].ToString();
                txtVessel.Text = CommonDt.Tables[0].Rows[0]["Vessel"].ToString();
                txtSpmntInvcNo.Text = CommonDt.Tables[0].Rows[0]["CommercialInvoiceNo"].ToString();
                txtSpmntInvcDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CommercialInvoiceDate"].ToString()));
                txtNotify.Text = CommonDt.Tables[0].Rows[0]["Notify"].ToString();
                btnSave.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtBlno.Text = txtComments.Text = txtSbno.Text = txtSpmntInvcNo.Text = txtSpmntInvcDt.Text = txtNotify.Text =  "";
                txtVessel.Text = "";
                ddlRefno.SelectedValue = Guid.Empty.ToString();
                btnSave.Text = "Save";
                ddlRefno.Enabled = true;
                gvCmrclInvce.DataSource = null;
                gvCmrclInvce.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoices", ex.Message.ToString());
            }
        }

        #endregion Methods

        #region DropDownList Selected Index Changed Event

        /// <summary>
        /// DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefno_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlRefno.SelectedValue != "0")
                {
                    FillInputFields(IOMTBLL.Select(CommonBLL.FlagZSelect, new Guid(ddlRefno.SelectedValue), Guid.Empty, "", new Guid(Session["CompanyID"].ToString())));
                    DataSet CmrclInvcItms = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagModify, new Guid(ddlRefno.SelectedValue), Guid.Empty, "", new Guid(Session["CompanyID"].ToString()));
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables.Count > 0)
                        ChkListId = CmrclInvcItms.Tables[0].Rows[0]["ChkListID"].ToString();
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables[5].Rows.Count > 0 && Convert.ToBoolean(CmrclInvcItms.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        CmrclInvcItms = CLBLL.SelectChekcListDtls(CommonBLL.FlagSelectAll, new Guid(ChkListId), "", new Guid(Session["CompanyID"].ToString()));
                        gvCmrclInvce.DataSource = CmrclInvcItms.Tables[1];
                        gvCmrclInvce.DataBind();
                    }
                    if (CmrclInvcItms != null && CmrclInvcItms.Tables.Count > 3)
                    {
                        gvCmrclInvce.DataSource = CmrclInvcItms.Tables[3];
                        gvCmrclInvce.DataBind();
                    }
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmrclInvce_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    if (ViewState["CrntItms"] != null)
                    {
                        DataTable CrntItms = (DataTable)ViewState["CrntItms"];
                        Guid RefID = new Guid(((Label)e.Row.FindControl("lblsItemDtlsID")).Text);
                        ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                    }
                    decimal amount = Convert.ToDecimal(((Label)e.Row.FindControl("lblAmount")).Text);
                    Label lblAmount = (Label)e.Row.FindControl("lblAmount");
                    lblAmount.Text = amount.ToString("N");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmrclInvce_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvCmrclInvce.HeaderRow != null)
                {
                    gvCmrclInvce.UseAccessibleHeader = false;
                    gvCmrclInvce.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commertial Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Event

        /// <summary>
        /// Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (btnSave.Text == "Save")
                {
                    res = CIBL.InsetUpdateDeleteCmrclInvc(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlRefno.SelectedValue), txtSpmntInvcNo.Text,
                    CommonBLL.DateInsert(txtSpmntInvcDt.Text), txtVessel.Text, txtBlno.Text, txtSbno.Text, txtComments.Text,
                    new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, txtNotify.Text);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Commercial Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/CommercialInvoiceStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Commercial Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = CIBL.InsetUpdateDeleteCmrclInvc(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()), new Guid(ddlRefno.SelectedValue),
                        txtSpmntInvcNo.Text, CommonBLL.DateInsert(txtSpmntInvcDt.Text), txtVessel.Text, txtBlno.Text, txtSbno.Text, txtComments.Text,
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, txtNotify.Text);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Commercial Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Invoices/CommercialInvoiceStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Commercial Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Commercial Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Web Method

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckCommercialInvNo(string CommercialInvoiceNo)
        {
            CommercialINVBLL CIBLL = new CommercialINVBLL();

            return CIBLL.CheckCInvNo(CommonBLL.FlagESelect, CommercialInvoiceNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
            //return CIBL.CheckCInvNo(CommonBLL.FlagBSelect, CommercialInvoiceNo);
        }

        #endregion

    }
}