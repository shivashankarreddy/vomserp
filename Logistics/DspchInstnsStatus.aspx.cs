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
using System.IO;
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Logistics
{
    public partial class DspchInstnsStatus : System.Web.UI.Page
    {

        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        IOMTemplateBLL IOMTBL = new IOMTemplateBLL();
        DspchInstnsBLL DPINBL = new DspchInstnsBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(DspchInstnsStatus));
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Despatch Instructions from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvDspchInstns,  IOMTBL.GetData(CommonBLL.FlagKSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagLSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text).Date, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagKSelect, 0, "", "", "", CommonBLL.StartDate,
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagKSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagKSelect, 0, "", "", "", CommonBLL.StartDate,
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagLSelect, 0, "", "", "", CommonBLL.DateInsert(txtFrmDt.Text),
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.EndDate, "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvDspchInstns, IOMTBL.GetData(CommonBLL.FlagLSelect, 0, "", "", "", CommonBLL.StartDate,
                //         int.Parse(hfSuplrId.Value), "", CommonBLL.DateInsert(txtToDt.Text), "", "", "", "", "", CommonBLL.UserID,
                //        DateTime.Now, CommonBLL.UserID, DateTime.Now, true));
                //else
                //    BindGridView(gvDspchInstns, DPINBL.SelectDspchInstns(CommonBLL.FlagESelect, 0));

                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from Despatch Instructions Status
        /// </summary>
        /// <param name="ID"></param>
        //private void DeleteRecord(Int64 ID)
        //{
        //    try
        //    {

        //        DataSet rslt = DPINBL.DeleteDspchInstns(CommonBLL.FlagDelete, ID, "", "", 0, 0, "", "", 0, "", "", "", "", "", "", "", "", 0);
        //        if (rslt != null && rslt.Tables.Count > 0)
        //        {
        //            if (rslt.Tables[0].Rows.Count > 0)
        //            {
        //                if (Convert.ToInt64(rslt.Tables[0].Rows[0]["Rslt"].ToString()) > 0)
        //                {
        //                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
        //                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Despatch Instructions Status",
        //                        "Row Deleted successfully.");
        //                    GetData();
        //                }
        //                else
        //                {
        //                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Already GDN or GRN Created/Error while Deleting.');", true);
        //                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status",
        //                        "Already GDN or GRN Created/Error while Deleting " + ID + ".");
        //                }
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Already GDN or GRN Created/Error while Deleting.');", true);
        //                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status",
        //                    "Already GDN or GRN Created/Error while Deleting " + ID + ".");
        //            }
        //        }
        //        else if (rslt == null || rslt.Tables.Count <= 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                "ErrorMessage('Already GDN or GRN Created/Error while Deleting.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status",
        //                "Already GDN or GRN Created/Error while Deleting " + ID + ".");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
        //    }
        //}
        #endregion

        #region Grid View Events
        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvDspchInstns_RowDataBound(object sender, GridViewRowEventArgs e)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvDspchInstns_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvDspchInstns.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblDspchInstns")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/DespatchInstructions.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //    Response.Redirect("../Masters/EmailSend.aspx?DpItID=" + ID.ToString(), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvDspchInstns_PreRender(object sender, EventArgs e)
        {
            try
            {
                //gvDspchInstns.UseAccessibleHeader = false;
                //gvDspchInstns.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
                string CT1RfFrmDt = "", CT1RfToDt = "", CreatedDT = "", FrmDt = "", ToDat = "";
                Guid LoginID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == ((Session["UserDtls"]).ToString()) ||
                    CommonBLL.TraffickerContactTypeText == ((Session["UserDtls"]).ToString()) && Mode != null))
                    LoginID =new Guid(((ArrayList)Session["UserDtls"])[1].ToString());
                if (Mode == "tldt")
                {
                    CT1RfFrmDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                    CT1RfToDt = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "Etdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                }
                string DisptInstNo = HFDisptInstNo.Value;
                string CT1RfNo = HFCT1RfNo.Value;
                
                string ContctPersns = HFContctPersns.Value;
                string ContctNum = HFContctNum.Value;
                string ShippingAdd = HFShippingAdd.Value; string SupplierNm = HFSupplierNm.Value;

                if (CT1RfFrmDt == "1-1-0001" || CT1RfFrmDt == "1-1-1900")
                    CT1RfFrmDt = "";
                if (CT1RfToDt == "1-1-0001")
                    CT1RfToDt = "";
                DataSet ds = DPINBL.SelectDspchInstnsForExport(DisptInstNo, CT1RfNo, ContctPersns, ContctNum, ShippingAdd, SupplierNm, CreatedDT, LoginID, new Guid(Session["CompanyId"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string Title = "DISPATCH INSTRUCTIONS STATUS";
                    string attachment = "attachment; filename=DispatchInStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF DISPATCH INSTRUCTIONS", MTsupp = "", MTDTS = "";
                    if (HFSupplierNm.Value != "")
                        MTsupp = HFSupplierNm.Value;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions Status", ex.Message.ToString());
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
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string CompanyId)
        {
            try
            {
                int res = 0;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    DataSet EditDS = DPINBL.SelectDspchInstns(CommonBLL.FlagASelect, new Guid(ID), new Guid(Session["CompanyId"].ToString()));
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = DPINBL.InsertUpdateDeleteDspchInstns(CommonBLL.FlagDelete, new Guid(ID), "", "", Guid.Empty, Guid.Empty, "", "", 0, "", "", "", "", "", "", "", "", Guid.Empty,new Guid(Session["CompanyId"].ToString()));
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
