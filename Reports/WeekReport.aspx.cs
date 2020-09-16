using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using BAL;
using System.IO;
using System.Web.UI.HtmlControls;

namespace VOMS_ERP.Reports
{
    public partial class WeekReport : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        ReportBLL RPBL = new ReportBLL();
        CustomerBLL CBL = new CustomerBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
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
                if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorised(Convert.ToInt32(Session["UserID"]), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            GetData();
                            txtFrmDt.Attributes.Add("readonly", "readonly");
                            txtToDt.Attributes.Add("readonly", "readonly");
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
                BindDropDownList(ddlcustomer, CBL.SearchCustomers(CommonBLL.FlagRegularDRP, "").Tables[0]);
                ddlReportType.SelectedIndex = 1;
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
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
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
        /// <param name="gv"></param>
        /// <param name="CommonDt"></param>
        private void BindGridView(GridView gv, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    gv.DataSource = CommonDt.Tables[0];
                    gv.DataBind();
                    gv.Caption = "Weekly Reports for " + ddlReportType.SelectedItem.Text;
                }
                else
                {
                    gv.DataSource = null;
                    gv.DataBind();
                    gv.Caption = "";
                }
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
                if (ddlcustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text)));
                else if (ddlcustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text)));
                else if (ddlcustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.EndDate));
                else if (ddlcustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                else if (ddlcustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text)));
                else if (ddlcustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.EndDate));
                else if (ddlcustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                    CommonBLL.StartDate, CommonBLL.DateInsert(txtToDt.Text)));
                else
                    BindGridView(GvWklyRpt, RPBL.SelectWeeklyRptDtls(ddlReportType.SelectedItem.Text, int.Parse(ddlcustomer.SelectedValue),
                        "", ""));
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
        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvNewFE_PreRender(object sender, EventArgs e)
        {
            try
            {
                GvWklyRpt.UseAccessibleHeader = false;
                GvWklyRpt.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
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
                Response.Redirect("../Reports/WeekReport.aspx", false);
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
                if (GvWklyRpt.Rows.Count > 0)
                {
                    string Title = "WEEKLY REPORT TYPE";
                    string attachment = "attachment; filename=WeeklyReports_" + ddlReportType.SelectedItem.Text.Replace(" ", "") + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    htw.Write("<center><b>");
                    if (ddlReportType.SelectedIndex != 0 && ddlcustomer.SelectedIndex != 0 && !String.IsNullOrEmpty(txtFrmDt.Text) && !String.IsNullOrEmpty(txtToDt.Text))
                        Title = Title + " : " + ddlReportType.SelectedItem.Text + ", of " + ddlcustomer.SelectedItem.Text + " from " + txtFrmDt.Text + " to " + txtToDt.Text + " ";
                    else if (ddlReportType.SelectedIndex != 0 && ddlcustomer.SelectedIndex != 0)
                        Title = Title + " : " + ddlReportType.SelectedItem.Text + ", of " + ddlcustomer.SelectedItem.Text;
                    else if (ddlReportType.SelectedIndex != 0)
                        Title = Title + " : " + ddlReportType.SelectedItem.Text;
                    else if (ddlcustomer.SelectedIndex != 0)
                        Title = Title + ", of " + ddlcustomer.SelectedItem.Text;
                    else if (!String.IsNullOrEmpty(txtFrmDt.Text))
                        Title = Title + " from " + txtFrmDt.Text;
                    else if (!String.IsNullOrEmpty(txtToDt.Text))
                        Title = Title + " from " + txtToDt.Text;
                    htw.Write(Title + "</b></center>");
                    HtmlForm frm = new HtmlForm();
                    GvWklyRpt.Parent.Controls.Add(frm);
                    GvWklyRpt.HeaderStyle.Font.Bold = true;
                    GvWklyRpt.HeaderStyle.Font.Name = "Arial";
                    GvWklyRpt.HeaderStyle.Font.Size = 9;
                    GvWklyRpt.Font.Name = "Arial";
                    GvWklyRpt.Font.Size = 9;
                    frm.Attributes["runat"] = "server";
                    frm.Controls.Add(GvWklyRpt);
                    frm.RenderControl(htw);
                    Response.Write(CommonBLL.AddExcelStyling());
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