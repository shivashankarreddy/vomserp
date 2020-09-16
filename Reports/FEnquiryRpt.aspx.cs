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

namespace VOMS_ERP.Reports
{
    public partial class FEnquiryRpt : System.Web.UI.Page
    {
        #region Variables
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        ErrorLog ELog = new ErrorLog();
        #endregion

        #region Default Page Load Event
        /// <summary>
        /// Deafult Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
            //        Response.Redirect("../Login.aspx?logout=yes");
            //    else
            //    {
            //        if (CommonBLL.IsAuthorisedUser((Session["UserID"]), Request.Path))
            //        {
            //            btnSearch.Attributes.Add("OnClick", "javascript:return Myvalidations()");
            //            if (!IsPostBack)
            //            {
            //                GetData();
            //                txtFrmDt.Attributes.Add("readonly", "readonly");
            //                txtToDt.Attributes.Add("readonly", "readonly");
            //            }
            //        }
            //        else
            //            Response.Redirect("../Masters/Home.aspx?NP=no");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            //}
        }
        #endregion

        #region Methods
        /// <summary>
        /// Default Data Loading...
        /// </summary>
        private void GetData()
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtFrmDt.Text = "";
                txtToDt.Text = "";
                txtCstNm.Text = "";
                //NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This meathod is used to search Mails from DB based on the parameter (Subject/Mail-ID)
        /// </summary>
        private void Search()
        {
            try
            {
                //DataTable dt = CommonBLL.EmptyDt();
                //if (hfCustomerId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //        Convert.ToDateTime(txtFrmDt.Text), Convert.ToDateTime(txtToDt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, Convert.ToInt64(Session["CompanyID"]), dt));
                //else if (hfCustomerId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    Convert.ToDateTime(txtFrmDt.Text), Convert.ToDateTime(txtToDt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, Convert.ToInt64(Session["CompanyID"]), dt));
                //else if (hfCustomerId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, Convert.ToInt64(Session["CompanyID"]), dt));
                //else if (hfCustomerId.Value != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, Convert.ToInt64(Session["CompanyID"]), dt));
                //else if (hfCustomerId.Value != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, Convert.ToInt64(Session["CompanyID"]), dt));
                //else if (hfCustomerId.Value == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (hfCustomerId.Value == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(hfCustomerId.Value), 0, "", "",
                //    CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else
                //    BindGridView(gvFERpt, NEBLL.NewEnquiryEdit(CommonBLL.FlagXSelect, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, "", 0, "", "", 0, 0, true, dt));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataSet EnqRpt)
        {
            try
            {
                if (EnqRpt.Tables.Count > 0 && EnqRpt.Tables[0].Rows.Count > 0)
                {
                    SetValues(EnqRpt);
                    gview.DataSource = EnqRpt;
                    gview.DataBind();
                }
                //else
                //NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Idividual Items
        /// </summary>
        /// <param name="EnqRpt"></param>
        private void SetValues(DataSet EnqRpt)
        {
            try
            {
                if (EnqRpt.Tables.Count > 0 && EnqRpt.Tables[0].Rows.Count > 0)
                {
                    lblLes.Text = EnqRpt.Tables[0].Select("StatusTypeId=20").Count().ToString();
                    lblfqs.Text = EnqRpt.Tables[0].Select("StatusTypeId=40").Count().ToString();
                    lblfps.Text = EnqRpt.Tables[0].Select("StatusTypeId=50").Count().ToString();
                    lbllps.Text = EnqRpt.Tables[0].Select("StatusTypeId=60").Count().ToString();
                    lblpis.Text = EnqRpt.Tables[0].Select("StatusTypeId=90").Count().ToString();
                    lblcls.Text = EnqRpt.Tables[0].Select("StatusTypeId=19").Count().ToString();
                    lblnps.Text = EnqRpt.Tables[0].Select("StatusTypeId=10").Count().ToString();
                    lblfpcs.Text = EnqRpt.Tables[0].Select("StatusTypeId=59").Count().ToString();
                    lbltfes.Text = EnqRpt.Tables[0].Rows.Count.ToString();
                    //lbltes.Text = EnqRpt.Tables[0].Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvFERpt_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvFERpt.UseAccessibleHeader = false;
                gvFERpt.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// This is a search button Event
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This is used to clear Controls
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Foreign Enquiry Reports", ex.Message.ToString());
            }
        }

        #endregion
    }
}
