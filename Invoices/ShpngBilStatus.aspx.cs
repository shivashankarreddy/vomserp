using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Threading;
using System.IO;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class ShpngBilStatus : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        ShpngBilDtlsBLL SBDBL = new ShpngBilDtlsBLL();
        BillOfLadingBLL BOLBL = new BillOfLadingBLL();
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
                    Ajax.Utility.RegisterTypeForAjax(typeof(ShpngBilStatus));
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            //GetData();
                            //txtFrmDt.Attributes.Add("readonly", "readonly");
                            //txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                //txtToDt.Text = txtFrmDt.Text = txtCustomerNm.Text = "";
                //hfCstmrID.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Proforma Invoice Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //DataTable dt = CommonBLL.EmptyFACDetails();
                //if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagBSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), dt));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagCSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, dt));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagBSelect, 0, Int64.Parse(hfCstmrID.Value),
                //         CommonBLL.StartDate, CommonBLL.EndDate, dt));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagBSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, dt));
                //else if (hfCstmrID.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagBSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), dt));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagCSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, dt));
                //else if (hfCstmrID.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagCSelect, 0, Int64.Parse(hfCstmrID.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), dt));
                //else
                //    BindGridView(GvShpngBil, SBDBL.SelectShpngBilDtls(CommonBLL.FlagSelectAll, 0, 0, CommonBLL.StartDate, CommonBLL.EndDate, dt));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = CommonDt;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from Proforma Invoice Details Status
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteRecord(string ID, string RefShpngBilID, string FS_AdrsDtlsID, string DBK_DtlsID, string DEPB_DtlsID, string INVC_DtlsID, string DA_DtlsID)
        {
            try
            {
                res = SBDBL.DeleteShpngBilDtls(CommonBLL.FlagDelete, new Guid(ID), new Guid(RefShpngBilID), new Guid(FS_AdrsDtlsID), new Guid(DBK_DtlsID),new Guid(DEPB_DtlsID), new Guid(INVC_DtlsID),
                    new Guid(DA_DtlsID), Guid.Empty, CommonBLL.EmptyFACDetails());
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Shipping Bill Status",
                        "Deleted successfully.");
                    GetData();
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GvShpngBil_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GvShpngBil_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = GvShpngBil.Rows[index];
                //Int64 ID = Convert.ToInt64(((Label)gvrow.FindControl("lblRefShpngBil")).Text);
                //Int64 FS_AdrsDtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblFSADtls")).Text);
                //Int64 DBK_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDBKDtls")).Text);
                //Int64 DEPB_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDEPBDtls")).Text);
                //Int64 INVC_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblINVCDtls")).Text);
                //Int64 DA_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDADtls")).Text);

                ////int ID = Convert.ToInt32(GvShpngBil.DataKeys[index].Values["PkingLstID"]);
                //string[] RefIDs = new string[] { ID.ToString(), FS_AdrsDtlsID.ToString(), DBK_DtlsID.ToString(), DEPB_DtlsID.ToString(), 
                //    INVC_DtlsID.ToString(), DA_DtlsID.ToString() };

                //if (e.CommandName == "Modify")
                //    //Response.Redirect("../Invoices/ShpngBilDtls.Aspx?SbdID=" + ID, false);
                //    Response.Redirect("ShpngBilDtls.Aspx?RefIDs=" + StringEncrpt_Decrypt.Encrypt(String.Join(",", RefIDs), true), false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID, ID, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GvShpngBil_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (GvShpngBil.HeaderRow == null) return;
                //GvShpngBil.UseAccessibleHeader = false;
                //GvShpngBil.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Export Buttons Click Events

        /// <summary>
        /// Export to Excel Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string ShpngBilFD = "", ShpngBilTD = "", CreatedDT = "";
                string PrfInvFD = "", PrfInvTD = "";
                Guid LoginID = Guid.Empty;

                string ShpngBilNmbr = HFShippingBillNo.Value;
                string ShpngBilFromDate = HFShippingBillFromDate.Value;
                string ShpngBilToDate = HFShippingBillToDate.Value;
                string PrfmaInvoiceNmbr = HFPrfmaInvoiceNmbr.Value;
                string PrfmaInvoiceFromDate = HFPrfmaInvoiceFromDate.Value;
                string PrfmaInvoiceToDate = HFPrfmaInvoiceToDate.Value;
                string PrtLoadingDes = HFPrtLoading.Value;
                string PrtDischargeDes = HFPrtDischarge.Value;
                string CntryDestinationDes = HFCntryDestination.Value;
                string CntryOrigineDes = HFCntryOrigine.Value;

                ShpngBilFD = HFShippingBillFromDate.Value == "" ? CommonBLL.StartDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFShippingBillFromDate.Value).ToString("MM-dd-yyyy");
                ShpngBilTD = HFShippingBillToDate.Value == "" ? CommonBLL.EndDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFShippingBillToDate.Value).ToString("MM-dd-yyyy");
                PrfInvFD = HFPrfmaInvoiceFromDate.Value == "" ? CommonBLL.StartDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFPrfmaInvoiceFromDate.Value).ToString("MM-dd-yyyy");
                PrfInvTD = HFPrfmaInvoiceToDate.Value == "" ? CommonBLL.EndDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFPrfmaInvoiceToDate.Value).ToString("MM-dd-yyyy");

                if (ShpngBilFD == "01-01-0001" || ShpngBilFD == "1-1-1900")
                    ShpngBilFD = "";
                if (ShpngBilTD == "01-01-0001")
                    ShpngBilTD = "";
                if (PrfInvFD == "01-01-0001")
                    PrfInvFD = "";
                if (PrfInvTD == "01-01-0001")
                    PrfInvTD = "";

                DataSet ds = new DataSet();
                ShpngBilDtlsBLL sbll = new ShpngBilDtlsBLL();
                ds = sbll.Shipping_Bill_Export(ShpngBilNmbr, ShpngBilFD, ShpngBilTD, PrfmaInvoiceNmbr, PrfInvFD, PrfInvTD,
                    PrtLoadingDes, PrtDischargeDes, CntryDestinationDes, CntryOrigineDes, new Guid(Session["CompanyId"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=Shipping_Bill_Status.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);


                    if (ShpngBilFD != "" && ShpngBilFD  == "01-01-1900")
                        ShpngBilFD = "";
                    if (ShpngBilFD != "" && ShpngBilTD == "01-01-0001" || ShpngBilTD == CommonBLL.EndDate.ToString("MM-dd-yyyy"))
                        ShpngBilTD = "";
                    string MTitle = "STATUS OF SHIPPING BILL RECEIVED", MTcustomer = "", MTDTS = "";

                    if (ShpngBilFD != "" && ShpngBilTD != "")
                        MTDTS = " DURING " + HFShippingBillFromDate.Value.Replace('/', '-') + " TO " + HFShippingBillToDate.Value.Replace('/', '-');
                    else if (ShpngBilFD != "" && ShpngBilTD == "")
                        MTDTS = " DURING " + HFShippingBillFromDate.Value.Replace('/', '-') + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
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
                                //Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
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
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string FS_AdrsDtlsID, string DBK_DtlsID, string DEPB_DtlsID, string INVC_DtlsID, string DA_DtlsID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false,CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    DataSet BillOfLading = new DataSet();
                    BillOfLading = BOLBL.GetDataSet(CommonBLL.FlagASelect, Guid.Empty, new Guid(ID));
                    if (BillOfLading != null && BillOfLading.Tables.Count > 0 && BillOfLading.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = SBDBL.DeleteShpngBilDtls(CommonBLL.FlagDelete, new Guid(ID), new Guid(ID), new Guid(FS_AdrsDtlsID),
                        new Guid(DBK_DtlsID), new Guid(DEPB_DtlsID), new Guid(INVC_DtlsID), new Guid(DA_DtlsID),
                        Guid.Empty, CommonBLL.EmptyFACDetails());
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, AirwayBill/BillOfLading already created so delete AirwayBill/BillOfLading. Error while Deleting " + ID;
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ErrMsg);
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string Encrypt(string ID, string FS_AdrsDtlsID, string DBK_DtlsID, string DEPB_DtlsID, string INVC_DtlsID, string DA_DtlsID)
        {
            try
            {
                string[] RefIDs = new string[] { ID, FS_AdrsDtlsID, DBK_DtlsID, DEPB_DtlsID, INVC_DtlsID, DA_DtlsID };
                return StringEncrpt_Decrypt.Encrypt(String.Join(",", RefIDs), true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Bill Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}