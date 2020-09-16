using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using Ajax;
using System.IO;
using System.Web;

namespace VOMS_ERP.Logistics
{
    public partial class RqstCEDtlsStatus : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
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
                Ajax.Utility.RegisterTypeForAjax(typeof(RqstCEDtlsStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Central Excise Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //string FromDt= "", ToDt = "";
                //string[] split = new string[]{"To"};
                //if (txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //{
                //FromDt = txtFrmDt.Text.Split(split, StringSplitOptions.RemoveEmptyEntries)[0].ToString().Trim();
                //ToDt = txtFrmDt.Text.Split(split, StringSplitOptions.RemoveEmptyEntries)[1].ToString().Trim();
                //FromDt = txtFrmDt.Text.Trim();
                //ToDt = txtToDt.Text.Trim();
                //}
                //if (txtSuplrNm.Text.Trim() != "" && FromDt.Trim() != "" && ToDt.Trim() != "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagESelect, 0, "", "", "", CommonBLL.DateInsert(FromDt), 
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(ToDt), "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && FromDt.Trim() != "" && ToDt.Trim() != "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagFSelect, 0, "", "", "", CommonBLL.DateInsert(FromDt), 
                //         0, "", CommonBLL.DateInsert(ToDt), "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && FromDt.Trim() == "" && ToDt.Trim() == "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagESelect, 0, "", "", "", CommonBLL.StartDate, 
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && FromDt.Trim() != "" && ToDt.Trim() == "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagESelect, 0, "", "", "", CommonBLL.DateInsert(FromDt), 
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() != "" && FromDt.Trim() == "" && ToDt.Trim() != "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagESelect, 0, "", "", "", CommonBLL.StartDate, 
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(ToDt), "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && FromDt.Trim() != "" && ToDt.Trim() == "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagFSelect, 0, "", "", "", CommonBLL.DateInsert(FromDt), 
                //         0, "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (txtSuplrNm.Text.Trim() == "" && FromDt.Trim() == "" && ToDt.Trim() != "")
                //    BindGridView(gvRced, IOMTBL.GetData(CommonBLL.FlagFSelect, 0, "", "", "", CommonBLL.StartDate, 
                //         0, "", CommonBLL.DateInsert(ToDt), "", "", "", "", "", CommonBLL.UserID, 
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else
                //    BindGridView(gvRced, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagJSelect, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from IOM Template
        /// </summary>
        /// <param name="ID"></param>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public int DeleteRecord(string ID)
        {
            try
            {
                res = RCEDBLL.InsertUpdateDeleteRqstCEDtls(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "", "", "", "", "", "", "",
                    Guid.Empty, Guid.Empty);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Request for Central Excise Status",
                        "Row Deleted successfully.");
                    //GetData();
                }

                else if (res != 0 && res == -6)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Cannot Delete this Record, IOM Created for this Reference');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status",
                        "Cannot Delete this Record, IOM Created for this Reference " + ID + ".");
                    res = -123;

                }
                //else
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                //        "ErrorMessage('Error while Deleting.');", true);
                //    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status",
                //        "Error while Deleting " + ID + ".");

                //}
                return res;
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
                return -1;
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRced_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                if (((HyperLink)e.Row.FindControl("hlbtnStatus")).Text != "Closed")
                {
                    Label Prcedid = (Label)e.Row.FindControl("lblRcedID");
                    HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnStatus");
                    //hlb.Text = "ss";
                    hlb.NavigateUrl = "IomForm.aspx?PIReqID=" + Prcedid.Text;
                }
                deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRced_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvRced.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblRcedID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/RqstCEDtls.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //    ;// GenPDF(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvRced_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvRced.HeaderRow == null) return;
                //gvRced.UseAccessibleHeader = false;
                //gvRced.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, EventArgs e)
        {
            try
            {
                string Cust = HFCust.Value;
                string FPOs = HFFPOs.Value;
                string LPOs = HFLPOs.Value;
                string status = HFStatus.Value;
                string Sup = HFSupplier.Value;
                string RqstNo = HFRqstNumber.Value;
                string FrmDt = HFRqstFromDT.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : Convert.ToDateTime(HFRqstFromDT.Value).ToString("yyyy-MM-dd");
                string ToDat = HFRqstToDT.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : Convert.ToDateTime(HFRqstToDT.Value).ToString("yyyy-MM-dd");

                if (FrmDt == "0001-1-1" || FrmDt == "1900-1-1")
                    FrmDt = "";
                if (ToDat == "0001-1-1")
                    ToDat = "";

                DataSet ds = RCEDBLL.PinvRqst_Search(Cust, FPOs, LPOs, Sup, RqstNo, status, FrmDt, ToDat, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=REQUEST_FOR_PROFORMA_INVOICE_STATUS.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";
                    string MTitle = "STATUS OF PROFORMA INVOICE REQUEST ", MTcustomer = "", MTDTS = "";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
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
                        string headerTable = "<img src='"+ CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    Response.Write(stw.ToString());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
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
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Status", ex.Message.ToString());
            }
        }
        #endregion

    }
}
