using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BAL;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class TermsNConditions : System.Web.UI.Page
    {
        # region variables
        int res;
        TermsMasterBLL TMBL = new TermsMasterBLL();
        ItemMasterBLL ItmMstr = new ItemMasterBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        //public AddItemDelegate AddItemCallback;
        #endregion

        #region Default Page Load Event

        //void Page_PreInit(object sender, EventArgs e)
        //{
        //    if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
        //    {
        //        MasterPageFile = "~/CustomerMaster.master";
        //    }
        //}

        /// <summary>
        /// Default Page Load Event
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
                    if (!IsPostBack)
                    {
                        Session["TCs"] = null;
                        GetData();
                    }
                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind Default Data, DropDownList and GridVeiw

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <returns></returns>
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        /// <summary>
        /// Bind Data to DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                string TArea = hdfldTArea.Value;
                string TAr = "";
                if (Request.QueryString.Count > 0)
                    TAr = Request.QueryString["TAr"].ToString();
                if (Request.QueryString["LPO"] != null)
                {
                    if (Request.QueryString["LPO"].ToString() == "true")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                            TMBL.SelectTermsMaster(CommonBLL.FlagXSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                    else if (Request.QueryString["LPO"].ToString() == "false")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                           TMBL.SelectTermsMaster(CommonBLL.FlagYSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                }
                else if (Request.QueryString["VLPO"] != null)
                {
                    if (Request.QueryString["VLPO"].ToString() == "false")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                           TMBL.SelectTermsMaster(CommonBLL.FlagYSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                }
                else if (Request.QueryString["FQ"] != null)
                {
                    if (Request.QueryString["FQ"].ToString() == "true")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                              TMBL.SelectTermsMaster(CommonBLL.FlagLSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                    else
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                              TMBL.SelectTermsMaster(CommonBLL.FlagXSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                }
                else if (Request.QueryString["LQ"] != null)
                {
                    DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                          TMBL.SelectTermsMaster(CommonBLL.FlagXSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                        ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                    {
                        BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                        Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                    }
                }
                else if (Request.QueryString["FPO"] != null)
                {
                    if (Request.QueryString["FPO"].ToString() == "true")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["FQQuteID"])) ? null :
                          TMBL.SelectTermsMaster(CommonBLL.FlagLSelect, new Guid(Request.QueryString["FQQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["FQQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                    else if (Request.QueryString["FPO"].ToString() == "false")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["FQQuteID"])) ? null :
                          TMBL.SelectTermsMaster(CommonBLL.FlagZSelect, new Guid(Request.QueryString["FQQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["FQQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                }
                else if (Request.QueryString["VFPO"] != null)
                {
                    if (Request.QueryString["VFPO"].ToString() == "false")
                    {
                        DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["FQQuteID"])) ? null :
                          TMBL.SelectTermsMaster(CommonBLL.FlagZSelect, new Guid(Request.QueryString["FQQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (LQTrmCndtns != null && Request.QueryString["FQQuteID"].ToString() != Guid.Empty.ToString())
                            ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                        {
                            BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                            Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                        }
                    }
                }
                else
                {
                    DataSet LQTrmCndtns = (Request.QueryString.Count < 2 && string.IsNullOrEmpty(Request.QueryString["LclQuteID"])) ? null :
                         TMBL.SelectTermsMaster(CommonBLL.FlagLSelect, new Guid(Request.QueryString["LclQuteID"]), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (LQTrmCndtns != null && Request.QueryString["LclQuteID"].ToString() != Guid.Empty.ToString())
                        ViewState["LclQuoteDt"] = LQTrmCndtns.Tables[0];
                    {
                        BindGridview(gvTmsCndtns, (TMBL.SelectTermsMaster(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, TAr, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                        Session["TCs"] = ConvertToDtbl(gvTmsCndtns);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownList
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GirdView using Gridview and Dataset
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="CommonDt"></param>
        protected void BindGridview(GridView gv, DataTable CommonDt)
        {
            try
            {
                gv.DataSource = CommonDt;
                gv.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="gvItems"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView gvTerms)
        {
            DataTable dt = CommonBLL.ATConditionsTitle();
            dt.Rows[0].Delete(); int tc = 0;
            foreach (GridViewRow row in gvTerms.Rows)
            {
                DataRow dr;
                if (((CheckBox)row.FindControl("ChkbItm")).Checked)
                {
                    dr = dt.NewRow();
                    dr["RefID"] = Guid.Empty;
                    dr["SNo"] = tc = tc + 1;
                    dr["Title"] = Convert.ToString(((Label)row.FindControl("lblttl")).Text);
                    dr["TermsID"] = new Guid(((Label)row.FindControl("lblTrmID")).Text);
                    dr["Against"] = Convert.ToString(((TextBox)row.FindControl("txtdecrp")).Text);

                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                GetData();
                Session.Remove("TCs");
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        #endregion

        #region GridView Row Data Bound Event

        protected void gvTmsCndtns_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            if (Session["TCs"] != null)
            {
                DataTable dbl = (DataTable)Session["TCs"];
                bool check = (from DataRow dr in dbl.Rows
                              where dr["TermsID"].ToString() ==
                                  ((Label)e.Row.FindControl("lblTrmID")).Text
                              select true).FirstOrDefault();
                if (check)
                {
                    ((CheckBox)e.Row.FindControl("ChkbItm")).Checked = check;
                    ((TextBox)e.Row.FindControl("txtdecrp")).Text = (from DataRow dr in dbl.Rows
                                                                     where dr["TermsID"].ToString() ==
                                                                         ((Label)e.Row.FindControl("lblTrmID")).Text
                                                                     select (string)dr["Against"]).FirstOrDefault();
                }
            }
            if (ViewState["LclQuoteDt"] != null)
            {
                DataTable dbl = (DataTable)ViewState["LclQuoteDt"];
                bool check = (from DataRow dr in dbl.Rows
                              where dr["TermsID"].ToString() ==
                                  ((Label)e.Row.FindControl("lblTrmID")).Text
                              select true).FirstOrDefault();
                if (check)
                {
                    ((CheckBox)e.Row.FindControl("ChkbItm")).Checked = check;
                    ((TextBox)e.Row.FindControl("txtdecrp")).Text = (from DataRow dr in dbl.Rows
                                                                     where dr["TermsID"].ToString() ==
                                                                         ((Label)e.Row.FindControl("lblTrmID")).Text
                                                                     select (string)dr["Against"]).FirstOrDefault();
                }
            }
        }

        #endregion

        #region  Button Click Events

        /// <summary>
        /// Save and Updte Button Click Events (Return Addtional Terms and Conditions to Page)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                DataTable dtbl = null;
                if (btnSave.Text == "Save")
                {
                    dtbl = ConvertToDtbl(gvTmsCndtns);
                    Session["TCs"] = dtbl;
                    ClientScript.RegisterStartupScript(GetType(), "CloseScript", "window.close();", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Terms & Conditions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Exit/Close button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExit_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
