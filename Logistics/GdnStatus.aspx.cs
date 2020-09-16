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
using System.Text;
using System.IO;
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Logistics
{
    public partial class GdnStatus : System.Web.UI.Page
    {
        #region Variables
        static Dictionary<string, string> TeamMembers = new Dictionary<string, string>();
        ErrorLog ELog = new ErrorLog(); int res = 999;
        GrnBLL GDNBL = new GrnBLL();
        static int UserID;
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
                Ajax.Utility.RegisterTypeForAjax(typeof(GdnStatus));
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
                                TeamMembers = Session["TeamMembers"].ToString().Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                            UserID = Convert.ToInt32(Session["UserID"]);
                            GetData();
                            // txtFrmDt.Attributes.Add("readonly", "readonly");
                            // txtToDt.Attributes.Add("readonly", "readonly");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0, CommonBLL.DateInsert(txtFrmDt.Text),
                //         CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() != "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagFSelect, 0, 0, int.Parse(hfSuplrId.Value),
                //        CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text)));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0,
                //        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (txtSuplrNm.Text.Trim() == "" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagGSelect, 0, 0, 0, CommonBLL.StartDate,
                //         CommonBLL.DateInsert(txtToDt.Text)));
                //else
                //    BindGridView(gvGDN, GDNBL.SelectGdnDtls(CommonBLL.FlagSelectAll, 0, 0));

                //ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                res = GDNBL.InsertUpdateDeleteGdnDtls(CommonBLL.FlagDelete, new Guid(ID), "", Guid.Empty, "", "", "", Guid.Empty, "", DateTime.Now, "", "",
                    DateTime.Now, Guid.Empty, 0, 0, 0, "", DateTime.Now, "", DateTime.Now, "", "", 0, 0, 0, 0, 0, DateTime.Now, "", 0,
                    "", "", Guid.Empty, CommonBLL.GdnItems(), "", "", new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Godown Delivery Note(GDN) Status",
                        "Row Deleted successfully.");
                    GetData();
                }
                else if (res != 0 && res == -6)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Cannot Delete this Record, IOM Created for this Reference');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status",
                        "Cannot Delete this Record, IOM Created for this Reference " + ID + ".");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status",
                        "Error while Deleting " + ID + ".");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
            }
        }

        protected void OnButtonClicked()
        {
            Response.Redirect(Request.Url.ToString(), false);
        }

        private void SendMail(string ID, string Mailid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GDN-" + ID + " Created By You was Approved. <br/>");
            sb.Append("<br/>");
            sb.Append("Thanks and Regards <br/>");
            sb.Append(Session["TLMailID"].ToString());
            string Sent = CommonBLL.SendApprovalMails(Mailid, "", "Approved", sb.ToString());
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGDN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                LinkButton lbRejected = (LinkButton)e.Row.Cells[lastCellIndex - 2].Controls[0];
                LinkButton lbApproved = (LinkButton)e.Row.Cells[lastCellIndex - 3].Controls[0];
                HiddenField CrtedBy = (HiddenField)e.Row.FindControl("hfCreatedBy");
                string IsApproved = ((HiddenField)e.Row.FindControl("hfIsApproved")).Value;
                if (((Label)e.Row.FindControl("lblStatus")).Text == "0")
                {
                    Label Gdnid = (Label)e.Row.FindControl("lblgdnID");
                    //Label Status = (Label)e.Row.FindControl("lblStatus");
                    HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnStatus");
                    //hlb.Text = "GDN-" + Gdnid.Text;
                    if (IsApproved == "Approved")
                    {
                        hlb.NavigateUrl = "GdnDetails.aspx?ID=" + Gdnid.Text;
                        hlb.ToolTip = "Continue with GRN...";
                    }
                    else
                    {
                        hlb.NavigateUrl = "GdnDetails.aspx?ID=" + Gdnid.Text;
                        hlb.ToolTip = "Not Approved, Please Approve it and Continue...";
                    }
                }
                else
                {
                    Label Gdnid = (Label)e.Row.FindControl("lblgdnID");
                    HyperLink hlb = (HyperLink)e.Row.FindControl("hlbtnStatus");
                    //hlb.Text = "GDN-" + Gdnid.Text;
                }
                if (IsApproved == "Approved")
                {
                    lbApproved.Enabled = false;
                    lbRejected.Enabled = false;
                    lbApproved.Text = "Approved";
                    lbRejected.Text = "";
                    EditButton.OnClientClick = "if (!alert('Cannot Edit when Approved.')) return false;";
                }
                else if (IsApproved == "Rejected")
                {
                    lbApproved.Enabled = false;
                    lbRejected.Enabled = false;
                    lbApproved.Text = "";
                    lbRejected.Text = "Rejected";
                    EditButton.OnClientClick = "if (!alert('Cannot Edit when Rejected.')) return false;";
                }

                if (Session["TeamMembers"].ToString() == "All" ||
                    Convert.ToInt64(Session["TLID"].ToString()) == Convert.ToInt64(Session["UserID"].ToString()) ||
                    !TeamMembers.ContainsKey(Session["UserID"].ToString()))
                {
                    //gvGDN.Columns[11].Visible = true;
                    //gvGDN.Columns[10].Visible = true;
                }
                else
                {
                    //gvGDN.Columns[11].Visible = false;
                    //gvGDN.Columns[10].Visible = false;
                }

                if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                {
                    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                    {
                        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value))))
                        {
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    else
                    {
                        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") ||
                            Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                }
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGDN_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = int.Parse(e.CommandArgument.ToString());
                //GridViewRow gvrow = gvGDN.Rows[index];
                //int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblgdnID")).Text);
                //string MailID = ((HiddenField)gvrow.FindControl("hfCreatedByMailId")).Value;
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/Gdn.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                //    DeleteRecord(ID);
                //else if (e.CommandName == "Reject")
                //    IsApprove.Show(ID, MailID);
                //else if (e.CommandName == "Approve")
                //{
                //    IsApprovedBLL IABLL = new IsApprovedBLL();
                //    res = IABLL.InsertUpdateDelete(CommonBLL.FlagUpdate, ID, CommonBLL.IsApproved, "", Convert.ToInt64(Session["UserID"]));
                //    if (res == 0)
                //    {
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "GDN Status", "Approved " + ID + " Successfully.");
                //        if (Session["TeamMembers"].ToString() != "All" && TeamMembers.ContainsKey(Session["UserID"].ToString()))
                //            SendMail(ID.ToString(), MailID);
                //        Search();
                //    }
                //    else
                //    {
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Status", "Error While Rejecting " + ID + " .");
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                //            "ErrorMessage('Error While Accepting.');", true);
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Accepting...');", true);
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGDN_PreRender(object sender, EventArgs e)
        {
            try
            {
                //gvGDN.UseAccessibleHeader = false;
                //gvGDN.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                //if (gvGDN.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.gvGDN.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                string DisInsNo = HFDisInsNo.Value;
                string FrmDt = HFDspFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFDspFrmDt.Value).ToString("yyyy-MM-dd");
                string ToDat = HFDspToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFDspToDt.Value).ToString("yyyy-MM-dd");
                string FPO = HFFPONo.Value;
                string LPO = HFLPONo.Value;
                string Suplr = HFSuplrNm.Value;
                string InvcNo = HFInvcNo.Value;
                string InvFDt = HFInvFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInvFrmDt.Value).ToString("yyyy-MM-dd");
                string InvTDt = HFInvToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFInvToDt.Value).ToString("yyyy-MM-dd");
                string Refgdn = HFRefGDN.Value;
                string Status = HFStatus.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                if (InvFDt == "1-1-0001" || FrmDt == "1-1-1900")
                    InvFDt = "";
                if (InvTDt == "1-1-0001")
                    InvTDt = "";
                DataSet ds = GDNBL.GDN_Search(DisInsNo, FrmDt, ToDat, FPO, LPO, Suplr, InvcNo, InvFDt, InvTDt, Refgdn, Status, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {

                    //string Title = "GOODS DISPATCH NOTE STATUS";
                    string attachment = "attachment; filename=GdnDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = " STATUS OF GOODS DISPATCH NOTE ", MTcustomer = "", MTDTS = "";
                    if (HFSuplrNm.Value != "")
                        MTcustomer = HFSuplrNm.Value;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Godown Delivery Note(GDN) Status", ex.Message.ToString());
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
                    DataSet EditDS = GDNBL.SelectGdnDtls(CommonBLL.FlagASelect, new Guid(ID), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        res = GDNBL.InsertUpdateDeleteGdnDtls(CommonBLL.FlagDelete, new Guid(ID), "", Guid.Empty, "", "", "", Guid.Empty, "", DateTime.Now, "", "",
                    DateTime.Now, Guid.Empty, 0, 0, 0, "", DateTime.Now, "", DateTime.Now, "", "", 0, 0, 0, 0, 0, DateTime.Now, "", 0,
                    "", "", Guid.Empty, CommonBLL.GdnItems(), "", "", new Guid(Session["CompanyID"].ToString()));
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


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string UpdateStatus(string ID, string CreatedBy, string MailID)
        {
            try
            {
                IsApprovedBLL IABLL = new IsApprovedBLL();
                res = IABLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ID), CommonBLL.IsApproved, "", new Guid(Session["UserID"].ToString()));
                if (res == 0)
                {
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "GDN Status", "Approved Successfully.");
                    if (Session["TeamMembers"].ToString() != "All" && TeamMembers.ContainsKey(Session["UserID"].ToString()))
                        SendMail(ID.ToString(), MailID);
                    return "Success";
                }
                else
                {
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Status", "Error While Rejecting " + ID + " .");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error While Accepting.');", true);
                    return "Error";

                }

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
