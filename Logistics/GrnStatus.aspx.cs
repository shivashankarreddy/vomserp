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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Logistics
{
    public partial class GrnStatus : System.Web.UI.Page
    {
        #region Variables
        static Guid UserID;int res = 999;
        ErrorLog ELog = new ErrorLog(); 
        GrnBLL GRNBL = new GrnBLL();
        static Dictionary<string, string> TeamMembers = new Dictionary<string, string>();
        
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
                Ajax.Utility.RegisterTypeForAjax(typeof(GrnStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        //btnSearch.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            if (Session["TeamMembers"].ToString() != "All")
                                TeamMembers = Session["TeamMembers"].ToString().Split(',').ToDictionary(key => key.Trim(), 
                                value => value.Trim());
                            UserID = new Guid(Session["UserID"].ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Central Excise Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagFSelect, 0, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagGSelect, 0, 0, 0, 0, CommonBLL.DateInsert(txtFrmDt.Text),
                //         CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagFSelect, 0, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagFSelect, 0, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagFSelect, 0, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagGSelect, 0, 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagGSelect, 0, 0, 0, 0, CommonBLL.StartDate,
                //         CommonBLL.DateInsert(txtToDt.Text)));
                //else
                //    BindGridView(gvGRN, GRNBL.SelectGrnDtls(CommonBLL.FlagSelectAll, 0, 0, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Change ARE-1 Details from Grid View to Data Table
        /// </summary>
        /// <param name="CT1Dtls"></param>
        /// <returns></returns>
        protected DataTable ChangeTbleFlds(DataTable CT1Dtls)
        {
            try
            {
                if (CT1Dtls.Columns.Contains("SNo"))
                    CT1Dtls.Columns.Remove("SNo");
                //for (int i = 0; i < CT1Dtls.Rows.Count; i++)                
                //    CT1Dtls.Rows[i]["Date"] = CommonBLL.DateInsert(CT1Dtls.Rows[i]["Date"].ToString());


                CT1Dtls.AcceptChanges();

                return CT1Dtls;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
                return null;
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
        //        DataTable EmptyTbl = ChangeTbleFlds(CommonBLL.FirstRowCT1Dtls());
        //        res = GRNBL.InsertUpdateDeleteGrnDtls(CommonBLL.FlagDelete, ID, "", 0, 0, "", "", 0, 0, 0, "", DateTime.Now, "", "",
        //            DateTime.Now, 0, 0, 0, 0, "", DateTime.Now, "", DateTime.Now, "", "", 0, 0, 0, 0, 0, DateTime.Now, "", 0,
        //            "", "", 0, CommonBLL.GdnItems(), "", EmptyTbl, "");//, null
        //        if (res == 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Godown Receipt Note(GRN) Status",
        //                "Row Deleted successfully.");
        //            GetData();
        //        }
        //        else if (res != 0 && res == -6)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Cannot Delete this Record, IOM Created for this Reference');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status",
        //                "Cannot Delete this Record, IOM Created for this Reference " + ID + ".");
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Error while Deleting.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status",
        //                "Error while Deleting " + ID + ".");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
        //    }
        //}
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];

                if (((Label)e.Row.FindControl("lblStatus")).Text == "0")
                {
                    Label Grnid = (Label)e.Row.FindControl("lblgrnID");
                    //Label Status = (Label)e.Row.FindControl("lblStatus");
                    //HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnStatus");
                    //hlb.Text = "ss";
                    //hlb.Text = "GRN-" + Grnid.Text;
                    //hlb.NavigateUrl = "CheckList.aspx?grnid=" + Grnid.Text;
                }
                deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                ////GridViewRow gvrow = gvGRN.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblgrnID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/Grn.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvGRN.HeaderRow == null) return;
                //gvGRN.UseAccessibleHeader = false;
                //gvGRN.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Receipt Note(GRN) Status", ex.Message.ToString());
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
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "";
                Guid LoginID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == ((Session["UserID"]).ToString()) ||
                    CommonBLL.TraffickerContactTypeText == ((Session["UserDtls"]).ToString()) && Mode != null))
                    LoginID = new Guid((Session["UserID"]).ToString());
                if (Mode == "tldt")
                {
                    FrmDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                    ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "Etdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    FrmDt = HFRecvdFrmDte.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRecvdFrmDte.Value).ToString("yyyy-MM-dd");
                    ToDat = HFRecvdToDte.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRecvdToDte.Value).ToString("yyyy-MM-dd");
                }
                string GRNNo = HFGRNNo.Value;
                string RefNmbr = HFRefNmbr.Value;
                
                string FPONum = HFFPONum.Value;
                string LPONum = HFLPONum.Value;
                string SuppNm = HFSuppNm.Value;
                string InvNm = HFInvNm.Value;
                
                string InvFrmDte = HFInvFrmDte.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInvFrmDte.Value).ToString("yyyy-MM-dd");
                string InvToDte = HFInvToDte.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInvToDte.Value).ToString("yyyy-MM-dd");
                

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = GRNBL.SelForExport(FrmDt, ToDat, GRNNo, RefNmbr, FPONum, LPONum, SuppNm, InvNm, InvFrmDte, InvToDte, CreatedDT, LoginID,new Guid(Session["CompanyId"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "GRN STATUS";
                    string attachment = "attachment; filename=GrnStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF GRN", MTsupp = "", MTDTS = "";
                    if (HFSuppNm.Value != "")
                        MTsupp = HFSuppNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                   

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTsupp != "" ? " FROM " + MTsupp.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    //Get the HTML for the control.
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Status", ex.Message.ToString());
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
                    DataTable EmptyTbl = ChangeTbleFlds(CommonBLL.FirstRowCT1Dtls());
                    LEnquiryBLL LEBLL = new LEnquiryBLL();
                    DataSet EditDS = GRNBL.SelectGrnDtls(CommonBLL.FlagWCommonMstr, new Guid(ID), Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = GRNBL.InsertUpdateDeleteGrnDtls(CommonBLL.FlagDelete, new Guid(ID), "", Guid.Empty, Guid.Empty, "", "", Guid.Empty, Guid.Empty,
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, Guid.Empty, 0, 0, 0, "", DateTime.Now, "", DateTime.Now, "", "", 0, 0, 0, 0, 0,
                            DateTime.Now, "", 0, "", "", Guid.Empty, CommonBLL.GdnItems(), "", EmptyTbl, "", Guid.Empty);
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, this is used by another transection/ Error while Deleting " + ID;
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
