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

namespace VOMS_ERP.Purchases
{
    public partial class RqstInsptnPlnStatus : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        IOMTemplateBLL IOMTBL = new IOMTemplateBLL();
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
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(RqstInsptnPlnStatus));
                        if (!IsPostBack)
                        {
                            GetData();
                            
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
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
               //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
             //   txtToDt.Text = txtFrmDt.Text = txtSuplrNm.Text = "";
                //hfSuplrId.Value = "0";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Request For Inspection Plan Details from DB based on the parameter
        /// </summary>
        //private void Search()
        //{
        //    try
        //    {
        //        if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
        //         BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
        //        else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagWCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text).Date, CommonBLL.UserID));
        //        else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.StartDate, CommonBLL.EndDate, CommonBLL.UserID));
        //        else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
        //        else if (hfSuplrId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
        //        else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagWCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, CommonBLL.UserID));
        //        else if (hfSuplrId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagWCommonMstr, 0, 0, int.Parse(hfSuplrId.Value), "", "", "",
        //                CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), CommonBLL.UserID));
        //        else
        //            BindGridView(gvInptnPln, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagJSelect, 0));

        //        //ClearInputs();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet RinptPln)
        {
            try
            {
                if (RinptPln.Tables.Count > 0 && RinptPln.Tables[0].Rows.Count > 0)
                {
                    gview.DataSource = RinptPln;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
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
              //  res = RIPBL.InsertUpdateDeleteRqstInsptnPln(CommonBLL.FlagDelete, ID, 0, 0, "", "", "", "", "", "", "", 0);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Inspection Plan Request Status",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0 && res == -6)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Cannot Delete this Record, Inspeciton Plan Created for this Reference');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status",
                        "Cannot Delete this Record, Inspeciton Plan Created for this Reference" + ID + ".");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
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
                    if (((HyperLink)e.Row.FindControl("hlbtnStatus")).Text != "Closed")
                    {
                        Label Insrqid = (Label)e.Row.FindControl("lblInsReqID");
                        HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnStatus");
                        //hlb.Text = "ss";
                        hlb.NavigateUrl = "InspectionPlan.aspx?InsReqID=" + Insrqid.Text;
                    }
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void gvInptnPln_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    try
        //    {
        //        int index = int.Parse(e.CommandArgument.ToString());
        //        GridViewRow gvrow = gvInptnPln.Rows[index];
        //        int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblInsReqID")).Text);
        //        if (e.CommandName == "Modify")
        //            Response.Redirect("../Purchases/InspectionPlanRequest.Aspx?ID=" + ID);
        //        else if (e.CommandName == "Remove")
        //            DeleteRecord(ID);
        //        else if (e.CommandName == "Mail")
        //            Response.Redirect("../Masters/EmailSend.aspx?IPID=" + ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void gvInptnPln_PreRender(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (gvInptnPln.HeaderRow == null) return;
        //        gvInptnPln.UseAccessibleHeader = false;
        //        gvInptnPln.HeaderRow.TableSection = TableRowSection.TableHeader;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
        //    }
        //}
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
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
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
              

                string FrmDt = HFReqFDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFReqFDate.Value).ToString("yyyy-MM-dd");
                string ToDat = HFReqTDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFReqTDate.Value).ToString("yyyy-MM-dd");
                
                string Cust = HFCust.Value;
                string FPO = HFFPO.Value;
                string LPO = HFLPO.Value;
                string Supp = HFSupp.Value;
                string ReqNo = HFReqNo.Value;
                string Status = HFStatus.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = RIPBL.ReqInsSearch(FrmDt, ToDat, Cust, FPO, LPO, Supp, ReqNo, Status, 0, "", new Guid(Session["CompanyID"].ToString()));
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

                    string MTitle = "STATUS OF INSPECTION PLAN REQUEST STATUS ", MTcustomer = "", MTDTS = "";
                    //if (HFSupplier.Value != "")
                    //  MTcustomer = HFSupplier.Value;
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

            //    //if (gvInptnPln.Rows.Count > 0)
            //    //{
            //    //    foreach (GridViewRow r in this.gvInptnPln.Controls[0].Controls)
            //    //    {
            //    //        r.Cells.RemoveAt(r.Cells.Count - 1);
            //    //        r.Cells.RemoveAt(r.Cells.Count - 1);
            //    //    }
            //    //}

            //    //<asp:HiddenField ID="HFCust" runat="server" Value="" />
            //    //<asp:HiddenField ID="HFFPO" runat="server" Value="" />
            //    //<asp:HiddenField ID="HFLPO" runat="server" Value="" />
            //    //<asp:HiddenField ID="HFSupp" runat="server" Value="" />
            //    //<asp:HiddenField ID="HFReqNo" runat="server" Value="" />                
            //    //<asp:HiddenField ID="HFReqDate" runat="server" Value="" />
            //    //<asp:HiddenField ID="HFStatus" runat="server" Value="" />


            //    string Cust = HFCust.Value;
            //    string FPO = HFFPO.Value;
            //    string LPO = HFLPO.Value;
            //    string Supp = HFSupp.Value;
            //    string ReqNo = HFReqNo.Value;
            //    string ReqDate = HFReqDate.Value;
            //    string Status = HFStatus.Value;
               
            //    string Title = "REQUEST FOR INSPECTION PLAN STATUS";
            //    string attachment = "attachment; filename=InsptnRequestDtls.xls";
            //    Response.ClearContent();
            //    Response.AddHeader("content-disposition", attachment);
            //    Response.ContentType = "application/ms-excel";
            //    StringWriter stw = new StringWriter();
            //    HtmlTextWriter htextw = new HtmlTextWriter(stw);

            //    string MTitle = " STATUS OF INSPECTION PLAN REQUEST ", MTcustomer = "", MTDTS = "";

            //   // if (txtSuplrNm.Text != "")
            //     //   MTcustomer = txtSuplrNm.Text.ToUpper();
            //   // if (txtFrmDt.Text != "" && txtToDt.Text != "")
            //   //     MTDTS = " DURING " + txtFrmDt.Text + " TO " + txtToDt.Text;
            // //   else if (txtFrmDt.Text != "" && txtToDt.Text == "")
            //   //     MTDTS = " DURING " + txtFrmDt.Text + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
            //   // else
            //  //      MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

            //   // htextw.Write("<center><b>" + MTitle + " "
            //                           //+ (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
            //                           //+ MTDTS + "</center></b>");
            //    //htextw.Write("<center><b>");
            //    //if (txtSuplrNm.Text != "")
            //    //{
            //    //    string text = " STATUS OF INSPECTION PLAN REQUEST " + " OF " + txtSuplrNm.Text + "";
            //    //    //+ " DURING " + txtFrmDt.Text + " TO " + txtToDt.Text;
            //    //    Label1.Text = text;
            //    //}
            //    //else if (txtFrmDt.Text != null && txtToDt.Text != null)
            //    //{
            //    //    string text = " STATUS OF INSPECTION PLAN REQUEST " + " DURING" + txtFrmDt.Text + " TO " + txtToDt.Text;
            //    //    Label1.Text = text;
            //    //}
            //    //else if (txtSuplrNm.Text != "")
            //    //{
            //    //    string text = " STATUS OF INSPECTION PLAN REQUEST " + " OF " + txtS.SelectedItem.Text.ToUpper();
            //    //    Label1.Text = text;
            //    //}
            //    //else
            //    //{
            //    //    string text = " STATUS OF INSPECTION PLAN REQUEST " + " OF" + ddlcustomer.SelectedItem.Text + " ";
            //    //    Label1.Text = text;
            //    //}

            //    //Label1.RenderControl(htextw);
            //    ////if (!String.IsNullOrEmpty(txtSuplrNm.Text) && !String.IsNullOrEmpty(txtFrmDt.Text) && !String.IsNullOrEmpty(txtToDt.Text))
            //    ////    Title = Title + " of " + txtSuplrNm.Text + " from " + txtFrmDt.Text + " to " + txtToDt.Text + " ";
            //    ////else if (!String.IsNullOrEmpty(txtSuplrNm.Text))
            //    ////    Title = Title + " of " + txtSuplrNm.Text;
            //    //htextw.Write(Title + "</b></center>");
            //    //gvInptnPln = CommonBLL.AddGVStyle(gvInptnPln);
            //    //gvInptnPln.RenderControl(htextw);
            //    Response.Write(CommonBLL.AddExcelStyling());
            //    string Img = "http://" + Request.Url.Authority;
            //    Img = Img + @"/images/Volta_Logo.png";
            //    string headerTable = "<img src='" + Img + "'margin-top =16px width=125 height=35 />";
            //    Response.Write(headerTable);
            //    Response.Write(stw.ToString());
            //    Response.End();
            //}
            //catch (ThreadAbortException)
            //{ }
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request Status", ex.Message.ToString());
            //}
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
                    res = RIPBL.InsertUpdateDeleteRqstInsptnPln(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "", "", "", "", "", "", "", Guid.Empty,new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else if (res != 0 && res == -6)
                        result = "Cannot Delete this Record, Inspeciton Plan Created for this Reference" + ID + ".";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Request Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Request Inspection Plan Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion

    }
}
