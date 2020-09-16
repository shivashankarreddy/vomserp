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
using System.Data.SqlClient;
using System.Web.Services;
using System.Collections.Generic;

namespace VOMS_ERP.Masters
{
    public partial class SentMailDetails : System.Web.UI.Page
    {
        # region Variables
        SentMailsBLL SMBLL = new SentMailsBLL();
        ErrorLog ELog = new ErrorLog();
        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Deafult Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser( new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        #region Add/Update Permission Code
                        //To Check User can have the Add/Update permissions
                        if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                        {
                            btnSearch.Enabled = true;
                            btnSearch.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                        {
                            btnSearch.Enabled = false;
                            btnSearch.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        }
                        #endregion
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SentMails Master", ex.Message.ToString());
            }
        }
        #endregion

        # region Methods

        protected void GetData()
        {
            BindGridView(gvSntMls, SMBLL.GetSentMailsSearch(CommonBLL.FlagESelect, null, CommonBLL.StartDate.ToString("yyyy-MM-dd"),
                CommonBLL.EndDate.ToString("yyyy-MM-dd"),new Guid(Session["CompanyID"].ToString())));
        }

        protected void BindGridView(GridView gview, DataSet mails)
        {
            if (mails.Tables.Count > 0 && mails.Tables[0].Rows.Count > 0)
            {
                gview.DataSource = mails;
                gview.DataBind();
            }
            else
                NoTable();
        }
        /// <summary>
        /// This meathod is used to search Mails from DB based on the parameter (Subject/Mail-ID)
        /// </summary>
        private void Search()
        {
            try
            {
                string FromDate = null;
                string ToDate = null;
                if (txtFrmDt.Text.Trim() != "")
                    FromDate = CommonBLL.DateInsert(txtFrmDt.Text.Trim()).ToString();
                if (txtToDt.Text.Trim() != "")
                    ToDate = CommonBLL.DateInsert(txtToDt.Text.Trim()).ToString();
                DataSet ds = new DataSet();

                if (txtSubEml.Text.Trim() == "")
                    ds = SMBLL.GetSentMailsSearch(Convert.ToChar("E"), null, Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd"), Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd"), new Guid(Session["CompanyID"].ToString()));
                else
                    ds = SMBLL.GetSentMailsSearch(Convert.ToChar("E"), txtSubEml.Text.Trim(), Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd"), Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd"), new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BindGridView(gvSntMls, ds);
                }
                else
                    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SentMails Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to Visible ReadMore Button
        /// </summary>
        /// <param name="Desc"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected bool SetVisibility(object Desc, int length)
        {
            return Desc.ToString().Length > length;
        }

        /// <summary>
        /// This method is used when there is no table
        /// </summary>
        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("S.No.");
                dt.Columns.Add("ID");
                dt.Columns.Add("ToAddr");
                dt.Columns.Add("CcAddr");
                dt.Columns.Add("Subject");
                dt.Columns.Add("Body");
                dt.Columns.Add("SentDate");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvSntMls.DataSource = ds;
                gvSntMls.DataBind();
                int columncount = gvSntMls.Rows[0].Cells.Count;
                //gvLpoItms.Rows[0].Cells.Clear();
                for (int i = 0; i < columncount; i++)
                    gvSntMls.Rows[0].Cells[i].Style.Add("display", "none");

                gvSntMls.Rows[0].Cells.Add(new TableCell());
                gvSntMls.Rows[0].Cells[columncount].ColumnSpan = columncount;
                gvSntMls.Rows[0].Cells[columncount].Text = "<center>No Records To Display...!</center>";

                //gvSntMls.Rows[0].Cells.Clear();
                //gvSntMls.Rows[0].Cells.Add(new TableCell());
                //gvSntMls.Rows[0].Cells[0].ColumnSpan = columncount;
                //gvSntMls.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
        # endregion

        # region ButtonClicks

        /// <summary>
        /// This is a search button Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSubEml.Text.Trim() != "" || (txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != ""))
                {
                    if (txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                    {
                        if (CommonBLL.DateInsert(txtFrmDt.Text.Trim()) <= CommonBLL.DateInsert(txtToDt.Text.Trim()))
                            Search();
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('FromDate must be less than ToDate.');", true);
                    }
                    else
                        Search();
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('To-Date is Required');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SentMails Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Button ReadMore In Gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnreadMore_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton button = (LinkButton)sender;
                GridViewRow row = button.NamingContainer as GridViewRow;
                Label descLabel = row.FindControl("lblBody") as Label;
                button.Text = (button.Text == "Read More") ? "Hide" : "Read More";
                //string temp = descLabel.Text;
                if (button.Text == "Hide")
                {
                    Label ExtraLabel1 = row.FindControl("lblExtra") as Label;
                    descLabel.Text = ExtraLabel1.Text.Replace("\r\n", "<br/>");//Text Full Data
                    descLabel.ToolTip = ExtraLabel1.ToolTip; // toolTip Limited Text                   
                }
                else
                {
                    Label ExtraLabel = row.FindControl("lblExtra") as Label;
                    descLabel.Text = ExtraLabel.ToolTip;//.Replace("\r\n", "<br/>");
                    descLabel.ToolTip = ExtraLabel.Text;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SentMails Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFrmDt.Text = "";
            txtToDt.Text = "";
            txtSubEml.Text = "";
            //NoTable();

        }
        # endregion

        # region GridView Events

        /// <summary>
        /// GridView RowDataBound Event for adding ...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSntMls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string ID = ((Label)e.Row.FindControl("lblID")).Text;
                    int length = ((Label)e.Row.FindControl("lblBody")).Text.Length;
                    if (length == 100)
                        ((Label)e.Row.FindControl("lblBody")).Text += "...";
                    ((Label)e.Row.FindControl("lblExtra")).ToolTip += "...";

                    ((Label)e.Row.FindControl("lblBody")).Text = ((Label)e.Row.FindControl("lblBody")).Text.Replace("%$%", "'");
                    ((Label)e.Row.FindControl("lblExtra")).Text = ((Label)e.Row.FindControl("lblExtra")).Text.Replace("%$%", "'");
                    ((Label)e.Row.FindControl("lblBody")).ToolTip = ((Label)e.Row.FindControl("lblBody")).ToolTip.Replace("%$%", "'");
                    ((Label)e.Row.FindControl("lblExtra")).ToolTip = ((Label)e.Row.FindControl("lblExtra")).ToolTip.Replace("%$%", "'");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSntMls_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvSntMls.UseAccessibleHeader = false;
                gvSntMls.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SentMails Master", ex.Message.ToString());
            }
        }

        # endregion

        # region Services

        /// <summary>
        /// This is web service for autocomplete
        /// </summary>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<string> GetSubOrMailIDs(string SearchString)
        {
            List<string> result = new List<string>();
            try
            {
                DataSet ds = new DataSet();
                SentMailsBLL SMBLL = new SentMailsBLL();
                ds = SMBLL.GetSubOrMailIDs(Convert.ToChar("B"), SearchString);
                if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        result.Add(ds.Tables[0].Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                string ErrMSg = ex.Message;
            }
            return result;
        }

        # endregion

    }
}
