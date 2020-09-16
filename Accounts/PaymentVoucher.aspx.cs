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

namespace VOMS_ERP.Logistics
{
    public partial class PaymentVoucher : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        PmtVcrsBLL PVBL = new PmtVcrsBLL();
        SupplierBLL SBLL = new SupplierBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstInsptnPlnBLL RIPBL = new RqstInsptnPlnBLL();
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
                if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorised(Convert.ToInt32(Session["UserID"]), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(PaymentVoucher));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            txtPaymentDt.Attributes.Add("readonly", "readonly");
                            txtChequeDt.Attributes.Add("readonly", "readonly");
                            txtPaymentDt.Text = DateTime.Now.ToShortDateString();
                            txtChequeDt.Text = DateTime.Now.ToShortDateString();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        #endregion

        #region  Methods

        /// <summary>
        /// Bind Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlcustmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, 0, 0));
                BindDropDownList(ddlBankName, EMBLL.EnumMasterSelect(CommonBLL.FlagRegularDRP, 0, BAL.CommonBLL.aBank, 0, Convert.ToInt32(Session["CompanyID"])));
                //BindDropDownList(ddlsuplrctgry, EMBLL.EnumMasterSelect(CommonBLL.FlagRegularDRP, 0, CommonBLL.SupplierCategory, 0, Convert.ToInt32(Session["CompanyID"])));
                BindDropDownList(ddlPaidAgainest, EMBLL.EnumMasterSelect(CommonBLL.FlagRegularDRP, 0, CommonBLL.PaymentAgainest, 0, Convert.ToInt32(Session["CompanyID"])));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    Convert.ToInt64(Request.QueryString["ID"].ToString()) : 0) != 0)
                {
                    EditRecord(PVBL.SelectPmtVcrs(CommonBLL.FlagModify, Convert.ToInt32(Request.QueryString["ID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit/Update Record
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    ddlcustmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CstmrID"].ToString();

                    BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagESelect, 0, int.Parse(ddlcustmr.SelectedValue), 0,
                        "", "", "", CommonBLL.UserID));
                    ddlsuplrctgry.SelectedValue = "274";
                    ddlsuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SuplrID"].ToString();

                    BindListBox(lbfpos, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagKSelect, 0, int.Parse(ddlcustmr.SelectedValue),
                        int.Parse(ddlsuplr.SelectedValue), CommonDt.Tables[0].Rows[0]["FpoIDs"].ToString(), "", "", CommonBLL.UserID));
                    foreach (ListItem li in lbfpos.Items)
                        li.Selected = true;

                    BindListBox(lblpos, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagLSelect, 0, int.Parse(ddlcustmr.SelectedValue),
                        int.Parse(ddlsuplr.SelectedValue), "", CommonDt.Tables[0].Rows[0]["LpoIDs"].ToString(), "", CommonBLL.UserID));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    txtChequeDt.Text = CommonDt.Tables[0].Rows[0]["CheckDt"].ToString();
                    txtChequeNo.Text = CommonDt.Tables[0].Rows[0]["CheckNumber"].ToString();
                    txtPaidAmt.Text = CommonDt.Tables[0].Rows[0]["PaidAmt"].ToString();
                    txtPaymentDt.Text = CommonDt.Tables[0].Rows[0]["PaymentDt"].ToString();
                    txtRTGS.Text = CommonDt.Tables[0].Rows[0]["RNCode"].ToString();
                    ddlBankName.SelectedValue = CommonDt.Tables[0].Rows[0]["BankName"].ToString();
                    txtRemarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();
                    ddlPaidAgainest.SelectedValue = CommonDt.Tables[0].Rows[0]["PaymentAgainst"].ToString();

                    if (CommonDt.Tables[0].Rows[0]["RefIDs"].ToString().Trim() != "")
                    {
                        BindListBox(lbPdAgnst, ddlPaidAgainest.SelectedItem.Text == "Drawings" ?
                            PVBL.SelectPmtVcrs(CommonBLL.FlagWCommonMstr, 0) : PVBL.SelectPmtVcrs(CommonBLL.FlagXSelect, 0));
                        lbPdAgnst.Visible = true;
                        Span5.Visible = true;
                    }
                    else
                    {
                        lbPdAgnst.Visible = false;
                        Span5.Visible = false;
                    }

                    ddlcustmr.Enabled = false;
                    ddlsuplrctgry.Enabled = false;
                    ddlsuplr.Enabled = false;
                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
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
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt.Tables[0];
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtPaymentDt.Text = txtPaidAmt.Text = txtChequeNo.Text =
                    txtChequeDt.Text = txtRTGS.Text = txtRemarks.Text = "";
                ddlcustmr.SelectedValue = ddlsuplr.SelectedValue = ddlsuplrctgry.SelectedValue = ddlBankName.SelectedValue = "0";
                lbfpos.Items.Clear(); lblpos.Items.Clear(); lbPdAgnst.Items.Clear();
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Send/Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string RefIDs = String.Join(", ", lbPdAgnst.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (btnSave.Text == "Save")
                {
                    res = PVBL.InsertUpdateDeletePmtVcrs(CommonBLL.FlagNewInsert, 0, int.Parse(ddlcustmr.SelectedValue),
                     int.Parse(ddlsuplr.SelectedValue), FpoIds, LpoIds, Int64.Parse(ddlPaidAgainest.SelectedValue), RefIDs,
                     Convert.ToDateTime(txtPaymentDt.Text), Convert.ToDecimal(txtPaidAmt.Text), Convert.ToDecimal(txtChequeNo.Text),
                     Convert.ToDateTime(txtChequeDt.Text), txtRTGS.Text, ddlBankName.SelectedValue, txtRemarks.Text,
                     Convert.ToInt64(Session["UserID"].ToString()));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher",
                            "Data inserted successfully.");
                        ClearInputs();
                        Response.Redirect("PmtVcrsStatus.aspx", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher",
                            "Error while Saving.");
                    }
                }

                else if (btnSave.Text == "Update")
                {
                    res = PVBL.InsertUpdateDeletePmtVcrs(CommonBLL.FlagUpdate, Convert.ToInt64(Request.QueryString["ID"].ToString()),
                        int.Parse(ddlcustmr.SelectedValue), int.Parse(ddlsuplr.SelectedValue), FpoIds, LpoIds,
                        Int64.Parse(ddlPaidAgainest.SelectedValue), RefIDs,
                     Convert.ToDateTime(txtPaymentDt.Text), Convert.ToDecimal(txtPaidAmt.Text), Convert.ToDecimal(txtChequeNo.Text),
                     Convert.ToDateTime(txtChequeDt.Text), txtRTGS.Text, ddlBankName.SelectedValue, txtRemarks.Text,
                     Convert.ToInt64(Session["UserID"].ToString()));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher",
                            "Data Updated successfully.");
                        ClearInputs();
                        Response.Redirect("PmtVcrsStatus.aspx", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagESelect, 0,
                    int.Parse(ddlcustmr.SelectedValue), 0, "", "", "", CommonBLL.UserID));
                ddlsuplrctgry.SelectedValue = "274";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Category Selected index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplrctgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagFSelect, 0,
                    int.Parse(ddlcustmr.SelectedValue), int.Parse(ddlsuplrctgry.SelectedValue), "", "", "", CommonBLL.UserID));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet Fpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagGSelect, 0,
                    int.Parse(ddlcustmr.SelectedValue), int.Parse(ddlsuplr.SelectedValue), "", "", "", CommonBLL.UserID);
                if (Fpos.Tables.Count > 0)
                    BindListBox(lbfpos, Fpos);
                ddlsuplrctgry.SelectedValue = Fpos.Tables[1].Rows[0]["CategoryID"].ToString();
                //txtRefno.Text = "INSP/" + Session["AliasName"].ToString() + "/" + Fpos.Tables[1].Rows[0]["ID"].ToString() + "/" 
                //   + CommonBLL.FinacialYearShort;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList FPO's Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //var selectedNames = lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet Lpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagINewInsert, 0,
                    int.Parse(ddlcustmr.SelectedValue), int.Parse(ddlsuplr.SelectedValue), FpoIds, "", "", CommonBLL.UserID);
                ViewState["FrnEnqs"] = string.Join(", ", (from dc in Lpos.Tables[1].Rows.Cast<DataRow>()
                                                          select ID.ToString()).ToArray());
                BindListBox(lblpos, Lpos);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box LPOs Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lblpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList Payment Againest Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPaidAgainest_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlPaidAgainest.SelectedItem.Text == "LPO")
                {
                    lbPdAgnst.Visible = false;
                    Span5.Visible = false;
                }
                else
                {
                    BindListBox(lbPdAgnst, ddlPaidAgainest.SelectedItem.Text == "Drawings" ? PVBL.SelectPmtVcrs(CommonBLL.FlagWCommonMstr, 0) :
                        PVBL.SelectPmtVcrs(CommonBLL.FlagXSelect, 0));
                    lbPdAgnst.Visible = true;
                    Span5.Visible = true;
                    // While Selecting Inspections for list box, List Box Value is Inspection Report ID and List Box Text 
                    //is Inspeciton Plan Reference Number
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Rendering Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtDtls_PreRender(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher", ex.Message.ToString());
            }
        }
        #endregion

    }
}
