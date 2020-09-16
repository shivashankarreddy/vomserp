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

namespace VOMS_ERP.Purchases
{
    public partial class InsptnPlnStatus : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        RqstInsptnPlnBLL IPBL = new RqstInsptnPlnBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(InsptnPlnStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
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
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Inspection Plan Details from DB based on the parameter
        /// </summary>
        private void Search()
        {
            try
            {
                //if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagESelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagFSelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagESelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.StartDate, CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagESelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagESelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagFSelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagFSelect, 0, int.Parse(hfSuplrId.Value), 
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else
                //    BindGridView(gvInptnPln, IPBL.SelectInsptnPln(CommonBLL.FlagCSelect, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet InptPln)
        {
            try
            {
                if (InptPln.Tables.Count > 0 && InptPln.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = InptPln;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from IOM Template
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteRecord(string ID)
        {
            try
            {
                res = IPBL.InsertUpdateDeleteInsptnPln(CommonBLL.FlagDelete,new Guid(ID), "", Guid.Empty, DateTime.Now, "", "", "", Guid.Empty, Guid.Empty, "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Inspection Plan Status",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0 && res == -6)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Cannot Delete this Record, Inspeciton Report Created for this Reference');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status",
                        "Error while Deleting " + ID + ".");
                }
                else 
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status",
                        "Cannot Delete this Record, Inspeciton Report Created for this Reference" + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnPln_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int lastCellIndex = e.Row.Cells.Count - 1;
                
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    if (((Label)e.Row.FindControl("lblInsPlnSts")).Text != "Closed")
                    {
                        Label ipid = (Label)e.Row.FindControl("lblInsPlnID");
                        HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnRefno");
                        hlb.NavigateUrl = "InspectionReport.aspx?InsPlnID=" + ipid.Text;
                    }
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnPln_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvInptnPln.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblInsPlnID")).Text);
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Purchases/InspectionPlan.Aspx?ID=" + ID);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Mail")
                //    Response.Redirect("../Masters/EmailSend.aspx?IPID=" + ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnPln_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvInptnPln.HeaderRow == null) return;
                //gvInptnPln.UseAccessibleHeader = false;
                //gvInptnPln.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                string FrmDt = "", ToDat = "", CreatedDT = "", STF = "";
                int LoginID = 0, CusID = 0;
                
                    FrmDt = HFInsptnFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsptnFromDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFInsptnToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsptnToDate.Value).ToString("yyyy-MM-dd");

                string RefNo = HFReffNum.Value;
                string CustmNm = HFCustNm.Value;
                string SuplrNm = HFSuppNm.Value;
                string Status = HFStatus.Value;
                string FpoNos = HFFPONum.Value;
                string LpoNos = HFLPONm.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = IPBL.SelectInsptnPlnForExport(RefNo, FrmDt, ToDat, CustmNm, SuplrNm, FpoNos, LpoNos, Status, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    // string Title = "STATUS OF FOREIGN PURCHASE ORDERS RECEIVED";
                    string attachment = "attachment; filename=InsptnPlanDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF INSPECTION PLAN ", MTcustomer = "", MTDTS = "";
                    if (HFCustNm.Value != "")
                        MTcustomer = HFCustNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                              + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                              + (MTDTS != "" ? MTDTS : "ON" + CreatedDT + "</center></b>"));

                    //if (!String.IsNullOrEmpty(Cust) && Cust != "")
                    //    Title = "STATUS OF FOREIGN PURCHASE ORDERS RECEIVED FROM " + Cust.ToUpper() ;
                    //htextw.Write("<center><b>" + Title + "</b></center>");
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
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                    DataSet EditDS = IPBL.SelectInsptnPlnForDelete(CommonBLL.FlagCSelect, new Guid(ID), Guid.Empty, "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = IPBL.InsertUpdateDeleteInsptnPln(CommonBLL.FlagDelete, new Guid(ID), "", Guid.Empty, DateTime.Now, "",
                            "", "", Guid.Empty, Guid.Empty, "", "", "", new Guid(CreatedBy), new Guid(Session["CompanyID"].ToString()));
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, Another Transaction is using this record";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion
    }
}
