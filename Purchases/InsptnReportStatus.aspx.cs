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
    public partial class InsptnReportStatus : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        RqstInsptnPlnBLL IRBL = new RqstInsptnPlnBLL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(InsptnReportStatus));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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
                //DataTable dt = EmptyDt();
               // dt = CommonBLL.FirstRowPaymentTerms();
                if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "ICtldt")
                    //{
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagZSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                    //}
                    //if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "DCtldt")
                    //{
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagHSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                    //}
                    //else
                    //{
                    //    Search();
                    //}
                }
                if (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "ICtldt")
                    //{
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagZSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                    //}
                    //if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "DCtldt")
                    //{
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagHSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                    //} 
                    //else
                    //{
                    //    Search();
                    //}
                }
                else if (Request.QueryString.Count > 0 && !String.IsNullOrEmpty(Request.QueryString["Mode"].ToString()))
                {
                    //if (Request.QueryString["Mode"].ToString() == "Dtdt")
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagLSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")),
                    //    Convert.ToInt64(Session["UserID"])));

                    //else if (Request.QueryString["Mode"].ToString() == "DCtldt")
                     //  BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagLSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                    //else if (Request.QueryString["Mode"].ToString() == "ICtldt")
                    //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagISelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                    //    CommonBLL.StartDate, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), Convert.ToInt64(Session["UserID"])));
                }
                else
                    Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.StartDate, CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagESelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
                //else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagFSelect, int.Parse(hfSuplrId.Value), 0, 0, 0,
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
                //else
                //    BindGridView(gvInptnRpt, IRBL.SelectInsptnReport(CommonBLL.FlagCSelect, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet InptRpt)
        {
            try
            {
                if (InptRpt.Tables.Count > 0 && InptRpt.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = InptRpt;
                    gview.DataBind();
                }
                else
                {
                    //GetData();
                    //gview.DataSource = null;
                    gview.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Delete Record from IOM Template
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteRecord(Int64 ID)
        {
            //try
            //{
            //    res = IRBL.InsertUpdateDeleteInsptnReport(CommonBLL.FlagDelete, ID, 0, 0, 0, DateTime.Now, DateTime.Now, DateTime.Now,
            //        DateTime.Now, "", "", "", "", 0);
            //    if (res == 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Inspection Report Status",
            //            "Row Deleted successfully.");
            //        GetData();
            //    }
            //    else if (res != 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
            //            "ErrorMessage('Error while Deleting.');", true);
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status",
            //            "Error while Deleting " + ID + ".");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            //}
        }

        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("CInsID");
                dt.Columns.Add("RefNo");
                dt.Columns.Add("InsDate");
                dt.Columns.Add("SInsptctor");
                dt.Columns.Add("TdpInspctor");
                dt.Columns.Add("ContactPerson");
                dt.Columns.Add("ContactNumber");
                dt.Columns.Add("InsPlace");
                dt.Columns.Add("InsDetails");
                dt.Columns.Add("InsStage");
                dt.Columns.Add("DispRE");
                dt.Columns.Add("InsStatus");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                //gvInptnRpt.DataSource = ds;
                //gvInptnRpt.DataBind();
                //int columncount = gvInptnRpt.Rows[0].Cells.Count;
                ////gvLenquiries.Rows[0].Cells.Clear();
                //for (int i = 0; i < columncount; i++)
                //    gvInptnRpt.Rows[0].Cells[i].Style.Add("display", "none");
                //gvInptnRpt.Rows[0].Cells.Add(new TableCell());
                //gvInptnRpt.Rows[0].Cells[columncount].ColumnSpan = columncount;
                //gvInptnRpt.Rows[0].Cells[columncount].Text = "<center>No Records To Display...!</center>";
                //gvLenquiries.Rows[0].CssClass = "bcGridViewRowStyle odd";
                //gvLenquiries.Rows[0].Style.Add("class", "bcGridViewRowStyle odd");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnRpt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int lastCellIndex = e.Row.Cells.Count - 1;

                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnRpt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
               // int index = int.Parse(e.CommandArgument.ToString());
               //// GridViewRow gvrow = gvInptnRpt.Rows[index];
               //// int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblInsRptID")).Text);
               // if (e.CommandName == "Modify")
               //     Response.Redirect("../Purchases/InspectionReport.Aspx?ID=" + ID);
               // else if (e.CommandName == "Remove")
               //     //DeleteRecord(ID);
               // //else if (e.CommandName == "Mail")
               // //    Response.Redirect("../Masters/EmailSend.aspx?IPID=" + ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvInptnRpt_PreRender(object sender, EventArgs e)
        {
            try
            {
                //if (gvInptnRpt.HeaderRow == null) return;
                //gvInptnRpt.UseAccessibleHeader = false;
                //gvInptnRpt.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }
        #endregion

        //#region DataTable

        //private DataTable EmptyDt()
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        //dt.Columns.Add(new DataColumn("S.No.", typeof(int)));
        //        dt.Columns.Add(new DataColumn("CInsID", typeof(long)));
        //        dt.Columns.Add(new DataColumn("RefNo", typeof(long)));
        //        dt.Columns.Add(new DataColumn("InsDate", typeof(string)));
        //        dt.Columns.Add(new DataColumn("SInsptctor", typeof(string)));
        //        //dt.Columns.Add(new DataColumn("TdpInspctor", typeof(string)));
        //        //dt.Columns.Add(new DataColumn("ContactPerson", typeof(string)));
        //        dt.Columns.Add(new DataColumn("ContactNumber", typeof(string)));
        //        dt.Columns.Add(new DataColumn("InsDetails", typeof(string)));
        //        dt.Columns.Add(new DataColumn("DispRE", typeof(string)));
        //        dt.Columns.Add(new DataColumn("InsStatus", typeof(long)));

        //        DataRow dr = dt.NewRow();
        //        //dr["S.No."] = 0;
        //        dr["CInsID"] = 0;
        //        dr["RefNo"] = 0;
        //        dr["InsDate"] = string.Empty;
        //        dr["SInsptctor"] = string.Empty;
        //        dr["ContactNumber"] = string.Empty;
        //        dr["InsDetails"] = string.Empty;
        //        dr["DispRE"] = string.Empty;
        //        dr["InsStatus"] = 0;
        //        dt.Rows.Add(dr);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
        //        return null;
        //    }
        //}

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
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
                //if (gvInptnRpt.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvInptnRpt.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "", RFrmDt = "", RToDt = "";
                int LoginID = 0;
                if ((CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString() ||
                    CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) && Mode != null)
                    LoginID = Convert.ToInt32(((ArrayList)Session["UserDtls"])[1].ToString());

                if (Mode == "ICtldt")
                {
                    FrmDt = HFInsFDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsFDate.Value).ToString("yyyy-MM-dd");
                    if (HFInsTDate.Value != "")
                    {
                        ToDat = HFInsTDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsTDate.Value).ToString("yyyy-MM-dd");
                    }
                    else
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                    RFrmDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                    RToDt = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "DCtldt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    FrmDt = HFInsFDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsFDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFInsTDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInsTDate.Value).ToString("yyyy-MM-dd");
                    RFrmDt = HFReplanFDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFReplanFDt.Value).ToString("yyyy-MM-dd");
                    RToDt = HFReplanTDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFReplanTDt.Value).ToString("yyyy-MM-dd");
                }
                string RefNo = HFInsptnRefNo.Value;
                string Inso = HFInspector.Value;
                string TDPIns = HFTDInspector.Value;
                string CntPerSn = HFCntPersn.Value;
                string CntNum = HFConNum.Value;
                string CntAdd = HFConAddrs.Value;
                string InsDtls = HFInsDtls.Value;
                string Stage = HFStage.Value;
                string Status = HFStatus.Value;
                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                if (RFrmDt == "1-1-0001" || RFrmDt == "1-1-1900")
                    RFrmDt = "";
                if (RToDt == "1-1-0001")
                    RToDt = "";
                DataSet ds = IRBL.Insptn_Search(FrmDt, ToDat, RefNo, InsDtls, Inso, TDPIns, CntPerSn, CntNum, CntAdd, Stage, RFrmDt, RToDt, Status, CreatedDT, LoginID, Mode, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=InsptnReportDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = "STATUS OF INSPECTION REPORT ", MTcustomer = "", MTDTS = "";
                    
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    htextw.Write("<center><b>" + MTitle + " "
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
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        private DataTable EmptyDt()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("RefNo", typeof(long)));
                dt.Columns.Add(new DataColumn("InsDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("SInsptctor", typeof(string)));
                dt.Columns.Add(new DataColumn("TdpInspctor", typeof(string)));
                dt.Columns.Add(new DataColumn("ContactPerson", typeof(string)));
                dt.Columns.Add(new DataColumn("ContactNumber", typeof(string)));
                dt.Columns.Add(new DataColumn("InsPlace", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("InsDetails", typeof(string)));
                dt.Columns.Add(new DataColumn("InsStage", typeof(string)));
                dt.Columns.Add(new DataColumn("DispRE", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("InsStatus", typeof(string)));


                DataRow dr = dt.NewRow();
                dr["RefNo"] = 0;
                dr["InsDate"] = DateTime.Now;
                dr["SInsptctor"] = string.Empty;
                dr["TdpInspctor"] = string.Empty;
                dr["ContactPerson"] = string.Empty;
                dr["ContactNumber"] = string.Empty;
                dr["InsPlace"] = string.Empty;
                dr["InsDetails"] = string.Empty;
                dr["InsStage"] = string.Empty;
                dr["DispRE"] = DateTime.Now;
                dr["InsStatus"] = string.Empty;

                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region Web Methods

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

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
                    res = IRBL.InsertUpdateDeleteInsptnReport(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, DateTime.Now,
                    DateTime.Now, "", "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, this is used by another transection/ Error while Deleting " + ID;
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}
