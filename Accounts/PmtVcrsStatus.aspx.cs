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

namespace VOMS_ERP.Accounts
{
    public partial class PmtVcrsStatus : System.Web.UI.Page
    {
        
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        PmtVcrsBLL PVBL = new PmtVcrsBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        IOMTemplateBLL IOMTBL = new IOMTemplateBLL();
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
                        if (!IsPostBack)
                        {
                            GetData();
                            txtFrmDt.Attributes.Add("readonly", "readonly");
                            txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                txtToDt.Text = txtFrmDt.Text = txtSuplrNm.Text = "";
                hfSuplrId.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Central Excise Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 
                        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.DateFormat(txtToDt.Text)));
                else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value),
                        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.DateFormat(txtToDt.Text)));
                else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), CommonBLL.StartDate,
                          CommonBLL.EndDate));
                else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 
                        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.EndDate));
                else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), CommonBLL.StartDate,
                         CommonBLL.DateFormat(txtToDt.Text)));
                else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value), 
                        CommonBLL.DateFormat(txtFrmDt.Text), CommonBLL.EndDate));
                else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value), 
                        CommonBLL.StartDate, CommonBLL.DateFormat(txtToDt.Text)));
                else
                    BindGridView(gvPmtVcrs, PVBL.SelectPmtVcrs(CommonBLL.FlagSelectAll, 0));

                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet Rceds)
        {
            try
            {
                if (Rceds.Tables.Count > 0 && Rceds.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = Rceds;
                    gview.DataBind();
                }
                else
                {
                    gview.DataSource = null;
                    gview.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from IOM Template
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteRecord(Int64 ID)
        {
            try
            {
                res = PVBL.InsertUpdateDeletePmtVcrs(CommonBLL.FlagDelete, ID, 0, 0, "", "", 0, "", DateTime.Now, 0, 0, DateTime.Now, "", 
                    "", "", 0);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/Log"), "Payment Voucher Status",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0 && res == -6)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Cannot Delete this Record, Next Payment is Released with this Reference');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status",
                        "Cannot Delete this Record, IOM Created for this Reference " + ID + ".");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtVcrs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtVcrs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvPmtVcrs.Rows[index];
                int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblPmtVchrID")).Text);
                if (e.CommandName == "Modify")
                    Response.Redirect("../Accounts/PaymentVoucher.Aspx?ID=" + ID, false);
                else if (e.CommandName == "Remove")
                    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //    ;// GenPDF(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPmtVcrs_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvPmtVcrs.HeaderRow == null) return;
                gvPmtVcrs.UseAccessibleHeader = false;
                gvPmtVcrs.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Payment Voucher Status", ex.Message.ToString());
            }
        }
        #endregion
    }
}
