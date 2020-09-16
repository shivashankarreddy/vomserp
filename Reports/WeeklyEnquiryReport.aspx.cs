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
    public partial class WeeklyEnquiryReport : System.Web.UI.Page
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
                if (Session["UserID"].ToString() == "" || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    //if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    //{
                    if (!IsPostBack)
                    {
                        GetData();
                        txtFrmDt.Attributes.Add("readonly", "readonly");
                        txtToDt.Attributes.Add("readonly", "readonly");
                    }
                    // }
                    //else
                    //    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Events
        /// </summary>
        /// <param name="rv"></param>
        /// <param name="CommonDt"></param>
        private void BindGridView(Repeater rv, DataSet commonDt)
        {
            try
            {
                if (commonDt != null && commonDt.Tables.Count > 0 && commonDt.Tables[0].Rows.Count > 0)
                {
                    rv.DataSource = commonDt.Tables[0];
                    rv.DataBind();
                    commonDt.Tables[0].Columns.Remove("CreatedDate");
                    commonDt.AcceptChanges();
                }
                else
                {
                    rv.DataSource = null;
                    rv.DataBind();
                }
                //ListOfLegend();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) != Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate, new Guid(Session["CompanyId"].ToString())));
                else if (new Guid(ddlcustomer.SelectedValue) == Guid.Empty && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text), new Guid(Session["CompanyId"].ToString())));
                else
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue),
                        "", "", new Guid(Session["CompanyId"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                    if (new Guid(ddlcustomer.SelectedValue.ToString()) != Guid.Empty)
                        Quotes1 = "Quotes given to " + ddlcustomer.SelectedItem.Text + ". Waiting for FPO";
                    else
                        Quotes1 = "Quotes given and Waiting for FPO ";
                    if (new Guid(ddlcustomer.SelectedValue.ToString()) != Guid.Empty)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
        //    }
        //}

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                Response.Redirect("../Reports/WeeklyEnquiryReport.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                DateTime FromDT = CommonBLL.StartDate, ToDT = CommonBLL.EndDate;

                if (txtFrmDt.Text.Trim() != "")
                    FromDT = CommonBLL.DateInsert(txtFrmDt.Text);
                if (txtToDt.Text.Trim() != "")
                    ToDT = CommonBLL.DateInsert(txtToDt.Text);

                BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls("Enquiry Report", new Guid(ddlcustomer.SelectedValue), FromDT, ToDT, new Guid(Session["CompanyId"].ToString())));
                if (GvWklyRpt.Items.Count > 0)
                {
                    string attachment = "attachment; filename=WeeklyEnquiryReport_" + ddlcustomer.SelectedItem.Text.Replace(" ", "") + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    string MTitle = " WEEKLY ENQUIRY REPORT ", MTcustomer = "", MTDTS = "";

                    if (ddlcustomer.SelectedIndex != 0)
                        MTcustomer = ddlcustomer.SelectedItem.Text.ToUpper();
                    if (txtFrmDt.Text != "" && txtToDt.Text != "")
                        MTDTS = " DURING " + txtFrmDt.Text + " TO " + txtToDt.Text;
                    else if (txtFrmDt.Text != "" && txtToDt.Text == "")
                        MTDTS = " DURING " + txtFrmDt.Text + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htw.Write("<center><b>" + MTitle + " " + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + "" + MTDTS + "</center></b>");
                    GvWklyRpt.RenderControl(htw);
                    Response.Write(CommonBLL.AddExcelStyling());
                    string style = @"<style> TABLE { border: 1px solid black; } TD { border: 1px solid black; } </style> ";
                    Response.Write(style);
                    string headerTable = "<img src='" + CommonBLL.CommonLogoUrl(HttpContext.Current) + "'margin-top =16px width=125 height=35 />";
                    Response.Write(headerTable);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion
    }
}