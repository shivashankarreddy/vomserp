using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Text;

namespace VOMS_ERP.Reports
{
    public partial class WeeklyFPOReport : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        CustomerBLL CBL = new CustomerBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        WeeklyReportsBLL we = new WeeklyReportsBLL();
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
                    //if (CommonBLL.IsAuthorised(Convert.ToInt32(Session["UserID"]), Request.Path))
                    //{
                    if (!IsPostBack)
                    {
                        GetData();
                        txtFrmDt.Attributes.Add("readonly", "readonly");
                        txtToDt.Attributes.Add("readonly", "readonly");
                    }
                    //}
                    //else
                    //    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
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
                BindDropDownList(ddlcustomer, CBL.SearchCustomers(CommonBLL.FlagRegularDRP, "", new Guid(Session["CompanyId"].ToString())).Tables[0]);
                //ddlReportType.SelectedIndex = 1;                
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownLists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        private void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Events
        /// </summary>
        /// <param name="rv"></param>
        /// <param name="CommonDt"></param>
        private void BindGridView(GridView rv, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    //Enquiry Report,FPO Report,Enquiry Pending,Invoice Pending

                    rv.DataSource = CommonDt.Tables[0];
                    rv.DataBind();
                    //rv.Caption = "Weekly Reports for Enquiry";

                }
                else
                {
                    rv.DataSource = null;
                    rv.DataBind();
                    //rv.Caption = "";
                }
                //ListOfLegend();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                txtFrmDt.Text = txtToDt.Text = "";
                ddlcustomer.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Search Method
        /// </summary>
        private void Search()
        {
            try
            {
                if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("FPO Report", new Guid(ddlcustomer.SelectedValue),
                        "", "", new Guid(Session["CompanyId"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        private void ListOfLegend()
        {
            try
            {
                Guid CustID = new Guid(ddlcustomer.SelectedValue);
                DateTime CurrentDate = CommonBLL.StartDate;
                if (txtFrmDt.Text != "")
                    CurrentDate = CommonBLL.DateInsert(txtFrmDt.Text);
                DateTime PreviousDate = CommonBLL.EndDate;
                if (txtToDt.Text != "")
                    PreviousDate = CommonBLL.DateInsert(txtToDt.Text);
                DataSet ds = we.GetDataSet(CommonBLL.FlagSelectAll, Guid.Empty, CustID, CurrentDate, PreviousDate);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string Quotes1 = "";
                    string WaitingForQuot2 = "Forwaded to supplier, waiting for quote";
                    string DrawingPending3 = "";
                    if (ddlcustomer.SelectedValue.ToString() != "0")
                        Quotes1 = "Quotes given to " + ddlcustomer.SelectedItem.Text + ". Waiting for FPO";
                    else
                        Quotes1 = "Quotes given and Waiting for FPO ";
                    if (ddlcustomer.SelectedValue.ToString() != "0")
                        DrawingPending3 = "Drawing pending from " + ddlcustomer.SelectedItem.Text;
                    else
                        DrawingPending3 = "Drawing pending ";

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='400px'>");
                    sb.Append("<tr><td align='center' colspan='2'><b>LIST OF LEGEND</b></td></tr>");
                    sb.Append("<tr><td>" + Quotes1 + "</td><td>" + ds.Tables[0].Rows[0]["Quotes"].ToString() + "</td>");
                    sb.Append("<tr><td>" + WaitingForQuot2 + "</td><td>" + ds.Tables[0].Rows[0]["WaitingTQuote"].ToString() + "</td>");
                    sb.Append("<tr><td>" + DrawingPending3 + "</td><td>" + ds.Tables[0].Rows[0]["DrawingPndng"].ToString() + "</td>");
                    sb.Append("<tr><td>Clarification mail sent </td><td></td>");
                    sb.Append("<tr><td>Attending </td><td></td>");
                    sb.Append("<tr><td>Regret from DCT / VIPL </td><td></td>");
                    sb.Append("<tr><td>Quote / Enquiry older than 6 months, Fresh enquiry requested </td><td></td>");
                    sb.Append("<tr><td>Total : </td><td></td>");
                    sb.Append("</tr></table>");
                    lblLegend.Text = sb.ToString();
                }
                else
                    lblLegend.Text = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid Veiw Events

        //protected void gvNewFE_PreRender(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        GvWklyRpt.UseAccessibleHeader = false;
        //        GvWklyRpt.HeaderRow.TableSection = TableRowSection.TableHeader;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
        //    }
        //}

        //protected void GvWklyRpt_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            if (ddlReportType.SelectedItem.Text == "Enquiry Report")
        //            {
        //                TextBox txtRemarks = new TextBox();
        //                txtRemarks.ID = "txtRemarks";
        //                txtRemarks.TextMode = TextBoxMode.MultiLine;
        //                txtRemarks.TextChanged += new EventHandler(txtRemarks_TextChanged);
        //                txtRemarks.Attributes.Add("runat", "server");
        //                txtRemarks.AutoPostBack = true;
        //                txtRemarks.Text = (e.Row.DataItem as DataRowView).Row["Remarks"].ToString();
        //                e.Row.Cells[7].Controls.Add(txtRemarks);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
        //    }
        //}

        //void txtRemarks_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        TextBox txtRemainder = (sender as TextBox);
        //        GridViewRow row = (txtRemainder.NamingContainer as GridViewRow);
        //        string remarks = (row.FindControl("txtRemarks") as TextBox).Text;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
        //    }
        //}

        protected void GvWklyRpt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                GridViewRow gvRow = e.Row;
                if (gvRow.RowType == DataControlRowType.Header)
                {
                    TextBox cd = (TextBox)gvRow.FindControl("txtCurrentDt");
                    TextBox pd = (TextBox)gvRow.FindControl("txtPreviousDt");

                    cd.Attributes.Add("readonly", "readonly");
                    pd.Attributes.Add("readonly", "readonly");

                    if (ViewState["CurrentDT"] != null)
                        cd.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"]));

                    if (ViewState["PreviousDT"] != null)
                        pd.Text = CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["PreviousDT"]));
                }
                else if (gvRow.RowType == DataControlRowType.DataRow)
                {
                    string hfcd = ((HiddenField)gvRow.FindControl("hfcurrentDt")).Value;
                    string hfpd = ((HiddenField)gvRow.FindControl("hfPrevDt")).Value;
                    TextBox txtCurrentStat = (TextBox)gvRow.FindControl("txtCurentStat");
                    TextBox txtPrevStat = (TextBox)gvRow.FindControl("txtPreviousStat");

                    //if (ViewState["CurrentDT"] != null && hfpd != "" &&
                    //    CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"])) == CommonBLL.DateDisplay(Convert.ToDateTime(hfpd)))
                    //{
                    //    txtCurrentStat.Text = txtPrevStat.Text;
                    //}
                    //else
                    //{
                    //    txtCurrentStat.Text = "";
                    //    txtPrevStat.Text = "";
                    //}

                    if (ViewState["PreviousDT"] != null && hfcd != "" &&
                        CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["PreviousDT"])) == CommonBLL.DateDisplay(Convert.ToDateTime(hfcd)))
                    {
                        txtPrevStat.Text = txtCurrentStat.Text;
                        //if (CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["PreviousDT"])) != CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"])))
                        txtCurrentStat.Text = "";// txtCurrentStat.Text;
                    }
                    else if (ViewState["PreviousDT"] != null && hfcd != "" &&
                        CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["PreviousDT"])) == CommonBLL.DateDisplay(Convert.ToDateTime(hfpd)))
                    {
                        txtPrevStat.Text = txtPrevStat.Text;

                        if (ViewState["CurrentDT"] != null && hfcd != "" &&
                       CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"])) == CommonBLL.DateDisplay(Convert.ToDateTime(hfcd)))
                        {
                            if (txtPrevStat.Text != "")
                                txtPrevStat.Text = txtPrevStat.Text;
                            txtCurrentStat.Text = txtCurrentStat.Text;
                        }
                        else
                            txtCurrentStat.Text = "";
                    }
                    else if (ViewState["CurrentDT"] != null && hfcd != "" &&
                    CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"])) == CommonBLL.DateDisplay(Convert.ToDateTime(hfcd)))
                    {
                        //if (txtPrevStat.Text != "")
                        //    txtPrevStat.Text = txtPrevStat.Text;

                        txtCurrentStat.Text = txtCurrentStat.Text;
                        if (CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["PreviousDT"])) != CommonBLL.DateDisplay(Convert.ToDateTime(ViewState["CurrentDT"])))
                            txtPrevStat.Text = "";
                    }
                    else
                    {
                        txtCurrentStat.Text = "";
                        txtPrevStat.Text = "";
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        protected void txtCurrentDt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime CurrentDT = CommonBLL.DateInsert((sender as TextBox).Text);
                ViewState["CurrentDT"] = CurrentDT;
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        protected void txtPreviousDt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime PreviousDT = CommonBLL.DateInsert((sender as TextBox).Text);
                ViewState["PreviousDT"] = PreviousDT;
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        protected void GvWklyRpt_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = GvWklyRpt.Rows[index];
                Guid FPOID = new Guid(GvWklyRpt.DataKeys[index].Values["ForeignPurchaseOrderId"].ToString());
                //Guid LPOID = new Guid(GvWklyRpt.DataKeys[index].Values["LocalPurchaseOrderId"].ToString());
                if (e.CommandName == "Modify")
                {
                    DateTime pd = Convert.ToDateTime(ViewState["PreviousDT"]);
                    DateTime cd = Convert.ToDateTime(ViewState["CurrentDT"]);
                    string ps = ((TextBox)gvrow.FindControl("txtPreviousStat")).Text;
                    string cs = ((TextBox)gvrow.FindControl("txtCurentStat")).Text;
                    Guid LPOID = new Guid(((Label)gvrow.FindControl("lblLPOID")).Text);

                    if (CommonBLL.DateDisplay(pd) != "01-01-0001" && CommonBLL.DateDisplay(cd) != "01-01-0001")
                        res = we.InsertUpdateDelete(CommonBLL.FlagBSelect, Guid.Empty, FPOID, LPOID,
                            "", cs, ps, cd, pd, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowPkng2", "ErrorMessage('Previous Status and Current Status dates cannot be Empty.');", true);
                }
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        # endregion

        #region Button Click Events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbtnClear_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Reports/WeeklyFPOReport.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        protected void btnsave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime pd = CommonBLL.StartDate;
                DateTime cd = CommonBLL.StartDate;
                if (ViewState["PreviousDT"] != null)
                    pd = Convert.ToDateTime(ViewState["PreviousDT"]);
                if (ViewState["CurrentDT"] != null)
                    cd = Convert.ToDateTime(ViewState["CurrentDT"]);
                int count = 0;
                foreach (GridViewRow row in GvWklyRpt.Rows)
                {
                    int res = 1;
                    string PrevStat = ((TextBox)row.FindControl("txtPreviousStat")).Text;
                    string CurrentStat = ((TextBox)row.FindControl("txtCurentStat")).Text;
                    Guid FPOID = new Guid(((HiddenField)row.FindControl("hfFPOID")).Value);
                    Guid LPOID = new Guid(((Label)row.FindControl("lblLPOID")).Text);
                    if (pd != CommonBLL.StartDate && cd != CommonBLL.StartDate && CurrentStat.Trim() != "")
                    {
                        count++;
                        res = we.InsertUpdateDelete(CommonBLL.FlagBSelect, Guid.Empty, FPOID, LPOID, "", CurrentStat, PrevStat, cd, pd, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true);
                        if (res == 0)
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    }
                }
                if (count == 0)
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to save. Fill all the details properly.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
            }
        }

        #endregion

        #region Export to Excel

        /// <summary>
        /// Export to Excel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (GvWklyRpt.Rows.Count > 0)
                {
                    if (GvWklyRpt.Rows.Count > 0)
                    {
                        foreach (GridViewRow r in this.GvWklyRpt.Controls[0].Controls)
                        {
                            TableCell TPcel = new TableCell();
                            TableCell TCcel = new TableCell();
                            TextBox Pst = (TextBox)r.FindControl("txtPreviousStat");
                            TextBox Cst = (TextBox)r.FindControl("txtCurentStat");
                            TextBox PHst = (TextBox)r.FindControl("txtPreviousDt");
                            TextBox CHst = (TextBox)r.FindControl("txtCurrentDt");
                            r.Cells.RemoveAt(r.Cells.Count - 1);
                            r.Cells.RemoveAt(r.Cells.Count - 1);
                            r.Cells.RemoveAt(r.Cells.Count - 1);
                            if (r.RowType == DataControlRowType.Header)
                            {
                                TPcel.Text = "Previous Status <br /> (" + PHst.Text + ")";
                                r.Cells.Add(TPcel);
                                TCcel.Text = "Current Status <br /> (" + CHst.Text + ")";
                                r.Cells.Add(TCcel);
                            }
                            else if (r.RowType == DataControlRowType.DataRow)
                            {
                                TPcel.Text = Pst.Text;
                                r.Cells.Add(TPcel);
                                TCcel.Text = Cst.Text;
                                r.Cells.Add(TCcel);
                            }
                        }
                    }
                    string attachment = "attachment; filename=WeeklyReports_FPOs.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    string MTitle = " FPO WEEKLY REPORTS ", MTcustomer = "", MTDTS = "";
                    if (ddlcustomer.SelectedIndex != 0)
                        MTcustomer = ddlcustomer.SelectedItem.Text.ToUpper();
                    if (txtFrmDt.Text != "" && txtToDt.Text != "")
                        MTDTS = " DURING " + txtFrmDt.Text + " TO " + txtToDt.Text;
                    else if (txtFrmDt.Text != "" && txtToDt.Text == "")
                        MTDTS = " DURING " + txtFrmDt.Text + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htw.Write("<center><b>" + MTitle + " " + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + "" + MTDTS + "</center></b>");
                    GvWklyRpt = CommonBLL.AddGVStyle(GvWklyRpt);
                    GvWklyRpt.RenderControl(htw);
                    Response.Write(CommonBLL.AddExcelStyling());
                    string style = @"<style> TABLE { border: 1px solid black; } TD { border: 1px solid black; } </style> ";
                    Response.Write(style);
                    DataSet ds = RPBL.Export_WeekFPOStatus("", "", "", "", "", "", "", "", "", "", "", "", "", new Guid(Session["CompanyID"].ToString()));
                    byte[] imge = null;
                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                    {
                        imge = (byte[])(ds.Tables[0].Rows[0]["CompanyLogo"]);
                        using (MemoryStream ms = new MemoryStream(imge))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                            string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
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
                    //string headerTable = "<img src='" + Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png") + "'margin-top =16px width=125 height=35 />";
                    //Response.Write(headerTable);//Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png")
                    Response.Write(sw.ToString());
                    Response.End();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Please Search Records');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly FPO Reports", ex.Message.ToString());
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}