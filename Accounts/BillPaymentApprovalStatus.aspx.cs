using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ajax;
using BAL;

namespace VOMS_ERP.Accounts
{
    public partial class BillPaymentApprovalStatus : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog();
        BillPaymentApprovalBLL BPABLL = new BillPaymentApprovalBLL();
        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(BillPaymentApprovalStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (!IsPostBack)
                        GetData();
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        private void Search()
        {
            try
            {
                //    if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagGSelect, int.Parse(hfSuplrId.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //            CommonBLL.DateInsert(txtToDt.Text)));
                //    else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagHSelect, int.Parse(hfSuplrId.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //            CommonBLL.DateInsert(txtToDt.Text)));
                //    else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagGSelect, int.Parse(hfSuplrId.Value), CommonBLL.StartDate, CommonBLL.EndDate));

                //    else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagGSelect, int.Parse(hfSuplrId.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //            CommonBLL.EndDate));

                //    else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagGSelect, int.Parse(hfSuplrId.Value), CommonBLL.StartDate,
                //            CommonBLL.DateInsert(txtToDt.Text)));
                //    else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagHSelect, int.Parse(hfSuplrId.Value), CommonBLL.DateInsert(txtFrmDt.Text),
                //            CommonBLL.EndDate));
                //    else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagHSelect, int.Parse(hfSuplrId.Value), CommonBLL.StartDate,
                //            CommonBLL.DateInsert(txtToDt.Text)));
                //    else
                //        BindGridView(gvBillPayment, BPABLL.GetDataSet(CommonBLL.FlagISelect, 0, 0, "", ""));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        protected void ClearInputs()
        {
            try
            {
                //txtToDt.Text = txtFrmDt.Text = txtSuplrNm.Text = "";
                //hfSuplrId.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        private void DeleteRecord(Guid ID)
        {
            try
            {
                int res = 1;
                res = BPABLL.InsertUpdateDelete(CommonBLL.FlagDelete, ID, Guid.Empty, "", "", "", "", DateTime.Now, "", DateTime.Now, "", DateTime.Now, "", DateTime.Now,
                    "", "", 0, "", 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, CommonBLL.EmptyDTBillPaymentApproval_INV(),
                    CommonBLL.EmptyDTBillPaymentApproval_PO(), new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Delete Successfully.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while deleting.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        #endregion

        #region GridView Events

        protected void gvBillPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header) return;

                Label BStatus = (Label)e.Row.FindControl("lblStatus");
                switch (BStatus.Text)
                {
                    case "0": BStatus.Text = "Rejected"; break;
                    case "1": BStatus.Text = "Approved"; break;
                    case "2": BStatus.Text = "Check Lost/Reverse Bill Payment"; break;
                    default: BStatus.Text = "Created";
                        break;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        protected void gvBillPayment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = Convert.ToInt32(e.CommandArgument);
                //string ApprovalID = gvBillPayment.DataKeys[index].Values["ApprovalID"].ToString();
                //string Status = ((Label)gvBillPayment.Rows[index].FindControl("lblStatus")).Text;
                //if (e.CommandName == "Modify")
                //{
                //    if (Status == "Check Lost/Reverse Bill Payment")
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Edit Not Permitted for Reverse Bill Payment/Check Lost.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", "Edit Not Permitted for Reverse Bill Payment/Check Lost.");
                //    }
                //    else if (Status == "Rejected")
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Edit Not Permitted for Rejected Bill Payment.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", "Edit Not Permitted for Rejected Bill Payment.");
                //    }
                //    else
                //    {
                //        Response.Redirect("../Accounts/BillPaymentApproval.Aspx?ID=" + ApprovalID, false);
                //    }
                //}
                //else if (e.CommandName == "Remove")
                //{
                //    if (Status == "Check Lost/Reverse Bill Payment")
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Delete Not Permitted for Reverse Bill Payment/Check Lost.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", "Delete Not Permitted for Reverse Bill Payment/Check Lost.");
                //    }
                //    else if (Status == "Rejected")
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Delete Not Permitted for Rejected Bill Payment.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Reverse Bill Payment Approval", "Delete Not Permitted for Rejected Bill Payment.");
                //    }
                //    else
                //    {
                //        DeleteRecord(Convert.ToInt64(ApprovalID));
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        protected void gvBillPayment_PreRender(object sender, EventArgs e)
        {
            try
            {
                //gvBillPayment.UseAccessibleHeader = false;
                //gvBillPayment.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Approval Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to Excel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string Ref = HFRefNo.Value;
                string Sup = HFSuplrNm.Value;
                string Lpo = HFLPO.Value;
                string Amt = HFAmt.Value;
                string Chqno = HFChqNo.Value;
                string Bank = HFBank.Value;
                string PaymentType = HFPymntTp.Value;
                string stat = HFStat.Value;
                string InvNo = HFInvNmbr.Value;

                string FrmDt = HFPymntFDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPymntFDt.Value).ToString("yyyy-MM-dd");
                string ToDat = HFPymntToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFPymntToDt.Value).ToString("yyyy-MM-dd");
                string ChkFrDt = HFChqFDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFChqFDt.Value).ToString("yyyy-MM-dd");
                string ChkToDt = HFChqTDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFChqTDt.Value).ToString("yyyy-MM-dd");

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                if (ChkFrDt == "1-1-0001" || ChkFrDt == "1-1-1900")
                    ChkFrDt = "";
                if (ChkToDt == "1-1-0001" || ChkToDt == "1-1-1900")
                    ChkToDt = "";

                DataSet ds = BPABLL.BPA_Search(FrmDt, ToDat, Ref, InvNo, Sup, Lpo, Amt, Chqno, ChkFrDt, ChkToDt, Bank, PaymentType, stat,new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=BillPaymentApprovalstatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-1900")
                        ToDat = "";

                    string MTitle = "STATUS OF BILL PAYMENT APPROVAL", MTcustomer = "", MTDTS = "";
                    if (HFSuplrNm.Value != "")
                        MTcustomer = HFSuplrNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");
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
                                //Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }

        }

        /// <summary>
        /// Rendering Method for Exports
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, int Stat)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    //DataTable dt =  
                    DataSet EditDS = BPABLL.GetDataSet(CommonBLL.FlagISelect, new Guid(ID), Guid.Empty, "", "", new Guid(Session["CompanyID"].ToString()));
                    if (Stat == 4)
                    {
                        res = BPABLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, "", "", "", "", DateTime.Now, "", DateTime.Now, "", DateTime.Now, "", DateTime.Now,
                            "", "", 0, "", 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, CommonBLL.EmptyDTBillPaymentApproval_INV(),
                            CommonBLL.EmptyDTBillPaymentApproval_PO(), new Guid(Session["CompanyID"].ToString()));
                    }
                    else
                        res = -123;
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, this is used by another transection/ Error while Deleting " + ID;

                #endregion
                }
                return result;
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust, string Stat)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}