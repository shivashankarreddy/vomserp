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

namespace VOMS_ERP.Customer_Access
{
    public partial class FQConfirmation_Customer : System.Web.UI.Page
    {

        # region variables
        int res;
        BAL.LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        ComparisonStmntBLL CSBL = new ComparisonStmntBLL();
        BAL.CustomerBLL CSTRBL = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        #endregion

        #region Page Load

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
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion

        #region Bind Default Page Data

        /// <summary>
        /// Calling Methods for Default Page Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlcustmr, CSTRBL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    //BindDropDownList(ddlcustmr, CSTRBL.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        ddlcustmr.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()),
                            new Guid(Session["UserID"].ToString()), true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                        //{
                        BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        //}
                        //To Select the Values of a ListBox
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s) item.Selected = true;
                            }
                        }
                        BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, new Guid(ddlfenqy.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty,
                            new Guid(ddlcustmr.SelectedValue), 45, new Guid(Session["UserID"].ToString()), Request.QueryString["FeqID"], new Guid(Session["CompanyID"].ToString())));
                    }
                    else
                    {
                        ddlcustmr.SelectedIndex = 1;
                        CustomerSelectionChanged();
                    }
                }
                else if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                {
                    ddlcustmr.SelectedValue = Request.QueryString["CsID"].ToString();
                    BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()),
                        new Guid(Session["UserID"].ToString()), true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    string[] states = Request.QueryString["FeqID"].Split(',');
                    foreach (string s in states)
                    {
                        foreach (ListItem item in Lstfenqy.Items)
                        {
                            if (item.Value == s) item.Selected = true;
                        }
                    }
                    string ForEnqIdLst = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), 45, new Guid(Session["UserID"].ToString()), ForEnqIdLst, new Guid(Session["CompanyID"].ToString())));
                }
                else
                {
                    ddlcustmr.SelectedIndex = 1;
                    CustomerSelectionChanged();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using Dataset
        /// </summary>
        /// <param name="SelectedItems"></param>
        protected void BindGidView(DataSet SelectedItems)
        {
            try
            {
                if (SelectedItems != null && SelectedItems.Tables.Count > 0)
                {
                    gvConformBskt.DataSource = SelectedItems.Tables[0];
                    gvConformBskt.DataBind();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownLists using DropDowns and Dataset
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        private void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", Guid.Empty.ToString()));
                //To Bind the Data in the List Box
                if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                {
                    CommonDt.Tables[0].DefaultView.Sort = "Description ASC";
                    Lstfenqy.DataSource = CommonDt.Tables[0];
                    Lstfenqy.DataTextField = "Description";
                    Lstfenqy.DataValueField = "ID";
                    Lstfenqy.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear All Inputs in the Page
        /// </summary>
        private void ClearAll()
        {
            try
            {
                ddlcustmr.SelectedIndex = -1;
                Lstfenqy.Items.Clear();
                BindGidView(null);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }
        #endregion

        #region DropDownList Selected Index Changed Events

        /// <summary>
        /// Customer DropDownList Selected Index Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomerSelectionChanged();
        }

        private void CustomerSelectionChanged()
        {
            try
            {
                string cstmr = ddlcustmr.SelectedValue;
                ClearAll();
                ddlcustmr.SelectedValue = cstmr;
                BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Frn Enquiry DropDownList Selected Index Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenqy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string ForEnqID = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (Lstfenqy.SelectedValue != "")
                    BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), 45, new Guid(Session["UserID"].ToString()), ForEnqID, new Guid(Session["CompanyID"].ToString())));
                else
                {
                    string cstmr = ddlcustmr.SelectedValue;
                    ClearAll();
                    ddlcustmr.SelectedValue = cstmr;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string fponum = String.Join(",", Lstfenqy.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (gvConformBskt.Rows.Count > 0 && CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                    Response.Redirect("~/Purchases/NewFPOrderVendor.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + fponum, false);
                //else if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    Response.Redirect("NewFPOrderVendor.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + fponum, false);
                //}
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Items to Confirm.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Confirmation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                string ForEnqId = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                res = CSBL.InsertDeleteBasketItems(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), 45, new Guid(Session["UserID"].ToString()), ForEnqId, new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    Response.Redirect("FQComparision_Customer.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + ForEnqId, true);
                    ClearAll();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "FQ-Conformation Basket", "Error While Deleting");
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foregn Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion
    }
}