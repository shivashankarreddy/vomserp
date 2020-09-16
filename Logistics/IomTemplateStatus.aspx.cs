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
using System.Threading;
using System.IO;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Logistics
{
    public partial class IomTemplateStatus : System.Web.UI.Page
    {

        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
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
                Ajax.Utility.RegisterTypeForAjax(typeof(IomTemplateStatus));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        //btnSearch.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            GetData();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get Default Data
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                //txtFrmDt.Text = txtToDt.Text = txtSuplrNm.Text = "";
                //hfSuplrId.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search IOM Template from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagHSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagINewInsert, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         0, "", CommonBLL.DateInsert(txtToDt.Text).Date, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagHSelect, 0, "", "", "", CommonBLL.StartDate,
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagHSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagHSelect, 0, "", "", "", CommonBLL.StartDate,
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagINewInsert, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         0, "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagINewInsert, 0, "", "", "", CommonBLL.StartDate,
                //         0, "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else
                //    BindGridView(gvIomTmpt, IOMTBL.GetData(CommonBLL.FlagGSelect, 0, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet IomT)
        {
            try
            {
                if (IomT.Tables.Count > 0 && IomT.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = IomT;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        ///// <summary>
        ///// Delete Record from IOM Template
        ///// </summary>
        ///// <param name="ID"></param>
        //private void DeleteRecord(Int64 ID)
        //{
        //    try
        //    {
        //        res = IOMTBL.InsertUpdateDeleteIOMT(CommonBLL.FlagDelete, ID, "", "", "", DateTime.Now, 0, "", DateTime.Now, 
        //            "", "", "", "", "", 0);
        //        if (res == 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "IOM Template Status", "Row Deleted successfully.");
        //            GetData();
        //        }
        //        else if (res != 0 && res == -6)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Cannot Delete this Record, CT-1 Created for this Reference');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status",
        //                "Cannot Delete this Record, CT-1 Created for this Reference " + ID + ".");
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Error while Deleting.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status",
        //                "Error while Deleting " + ID + ".");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
        //    }
        //}
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvIomTmpt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {   
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    int lastCellIndex = e.Row.Cells.Count - 1;
                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                    HiddenField HFIsCT1 = (HiddenField)e.Row.FindControl("HFInCT1");
                    if (((Label)e.Row.FindControl("lblStatus")).Text == "Accepted" && !Convert.ToBoolean(HFIsCT1.Value))
                    {
                        Label iomid = (Label)e.Row.FindControl("lblIomID");
                        HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnRefno");
                        //hlb.Text = "ss";
                        hlb.NavigateUrl = "CTOneGeneration.aspx?IOMRefID=" + iomid.Text;
                    }
                    if (Convert.ToBoolean(HFIsCT1.Value))
                        EditButton.OnClientClick = "if (!alert('CT-1 was prepared for this IOM')) return false;";
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvIomTmpt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvIomTmpt.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblIomID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/IomForm.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //{ }// GenPDF(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvIomTmpt_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvIomTmpt.HeaderRow == null) return;
                //gvIomTmpt.UseAccessibleHeader = false;
                //gvIomTmpt.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
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
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
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
                string FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                string ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                string RefNmbr = HFRefNmbr.Value;
                string CstmrNm = HFCstnrNm.Value;
                string SuplrNm = HFSuplrNm.Value;
                string Subject = HFSubject.Value;
                string Status = HFStatus.Value;
                string FpoNmbrs = HFFpoNmbrs.Value;
                string LpoNmbrs = HFLpoNmbrs.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";

                DataSet ds = IOMTBL.Select_IOMSearch(RefNmbr, FrmDt, ToDat, CstmrNm, SuplrNm, Subject, FpoNmbrs, LpoNmbrs, Status, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    //string Title = "IOM TEMPLATE STATUS";
                    string attachment = "attachment; filename=IomTemplateDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    // htextw.Write("<center><b>" + Title + "</b></center>");
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF IOM TEMPLATE", MTcustomer = "", MTDTS = "";
                    if (HFCstnrNm.Value != "")
                        MTcustomer = HFCstnrNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    //else if (FrmDt == "" && ToDat != "")
                    //    MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
                    dgGrid.HeaderStyle.Font.Bold = true;
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
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Template Status", ex.Message.ToString());
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
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    //DataTable dt = EmptyDt();
                    LEnquiryBLL LEBLL = new LEnquiryBLL();
                    DataSet EditDS = IOMTBL.GetData(CommonBLL.FlagVSelect, new Guid(ID), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = IOMTBL.InsertUpdateDeleteIOMT(CommonBLL.FlagDelete, new Guid(ID), "", "", "", DateTime.Now, Guid.Empty, "", DateTime.Now, "", "", "", "", "", Guid.Empty, Guid.Empty);
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, this is used by another transection/ Error while Deleting ";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", sx.Message.ToString());
                return ErrMsg;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion

    }
}
