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

namespace VOMS_ERP.Quotations
{
    public partial class QotConfromationBasket : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CustomerBLL CSTRBL = new CustomerBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewFQuotationBLL FQBL = new NewFQuotationBLL();
        ComparisonStmntBLL CSBL = new ComparisonStmntBLL();

        #endregion

        #region Dafault Page Load Event

        /// <summary>
        /// Dafault Page Load Event
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
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()"); //
                        btnFqSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        btnFpoSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");

                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind Default page Data

        /// <summary>
        /// Bind Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlcustmr, CSTRBL.SelectCustomersDtlsGUIDBind(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyId"].ToString())));

                if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                {
                    ddlcustmr.SelectedValue = Request.QueryString["CsID"].ToString();
                    BindListBox(lbFenquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty,new Guid(ddlcustmr.SelectedValue),
                        Guid.Empty, "", DateTime.Now, "FeqID", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    string[] fen = Request.QueryString["FeqID"].ToString().Split(',');
                    foreach (ListItem item in lbFenquiry.Items)
                    {
                        foreach (string s in fen)
                        {
                            if (item.Value == s.ToLower().Trim())
                                item.Selected = true;
                        }
                    }

                    BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Request.QueryString["CsID"].ToString()), 35, new Guid(Session["UserID"].ToString()), Request.QueryString["FeqID"].ToString(), new Guid(Session["CompanyID"].ToString())));
                    if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                    {
                        BindListBox(lbFenquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty,new Guid(ddlcustmr.SelectedValue),
                         Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                         new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        ddlcustmr.Enabled = false;
                        btnFpoSave.Visible = btnFqSave.Visible = true;
                        btnSave.Visible = false;
                        //string[] fen1 = Request.QueryString["FeqID"].ToString().Split(',');
                        foreach (ListItem item in lbFenquiry.Items)
                        {
                            foreach (string s in fen)
                            {
                                if (item.Value == s.ToLower().Trim())
                                    item.Selected = true;
                            }
                        }
                    }
                    else
                    {
                        ddlcustmr.Enabled = true;
                        btnFpoSave.Visible = btnFqSave.Visible = false;
                        btnSave.Visible = true;
                    }
                }
                else if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                {
                    if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                    {
                        BindListBox(lbFenquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty,new Guid(ddlcustmr.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                            new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        ddlcustmr.Enabled = false;
                        btnFpoSave.Visible = btnFqSave.Visible = true;
                        btnSave.Visible = false;
                    }
                    else
                    {
                        ddlcustmr.Items.Clear();
                        BindDropDownList(ddlcustmr, CSTRBL.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), Guid.Empty));
                        lbFenquiry.SelectedIndex = -1;
                    }
                }
                else
                {
                    btnFpoSave.Visible = btnFqSave.Visible = false;
                    btnSave.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw
        /// </summary>
        /// <param name="SelectedItems"></param>
        protected void BindGidView(DataSet SelectedItems)
        {
            try
            {
                gvConformBskt.DataSource = SelectedItems.Tables[0];
                gvConformBskt.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDown Lists
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
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDown Lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        private void BindListBox(ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt;
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        private DataTable ConveToTable(GridView gv)
        {
            try
            {
                DataTable dt = CommonBLL.EmptyDtFQ();
                dt.Rows.RemoveAt(0);
                foreach (GridViewRow gvrow in gv.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["SNo"] = Convert.ToInt32(((Label)gvrow.FindControl("lblSno")).Text);
                    dr["ItemId"] = new Guid(((Label)gvrow.FindControl("lblItemID")).Text);
                    dr["ForeignQuotationId"] = new Guid(((Label)gvrow.FindControl("lblFenqID")).Text); //Item wise Foreign Enquir ID
                    dr["PartNumber"] = ((Label)gvrow.FindControl("lblPartNumber")).Text;
                    dr["Specifications"] = ((Label)gvrow.FindControl("lblSpecificaiton")).Text;
                    dr["Make"] = ((Label)gvrow.FindControl("lblMake")).Text;
                    dr["RoundOff"] = ddlRateChange.SelectedValue;
                    dr["Rate"] = Convert.ToDecimal(((Label)gvrow.FindControl("lblRate")).Text);
                    dr["QPrice"] = Convert.ToDecimal(((Label)gvrow.FindControl("lblQPrice")).Text);
                    dr["Quantity"] = Convert.ToDecimal(((Label)gvrow.FindControl("lblQty")).Text);
                    dr["Amount"] = Convert.ToDecimal(((Label)gvrow.FindControl("lblAmount")).Text);
                    dr["ExDutyPercentage"] = 0.00;
                    dr["DiscountPercentage"] = 0.00;
                    dr["UNumsId"] = new Guid(((Label)gvrow.FindControl("lblUnitID")).Text);
                    dr["Remarks"] = ((Label)gvrow.FindControl("lblItemDtlsID")).Text; //Item Details ID for Savisng into Basket
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Clear All Inputs
        /// </summary>
        private void ClearAll()
        {
            try
            {
                ddlcustmr.SelectedValue = "0";
                lbFenquiry.SelectedIndex = -1;
                BindGidView(null);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events and Selected Index Changed Evnets

        /// <summary>
        /// Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ( Session["AccessRole"].ToString() ==  CommonBLL.CustmrContactTypeText)  
                {
                    BindListBox(lbFenquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagKSelect, Guid.Empty,Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                }
                else
                {
                    BindListBox(lbFenquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                                "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                                new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Foreing Enquiry Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenqy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Session["AccessRole"].ToString() == CommonBLL.CustmrContactTypeText)  
                {
                    string ForEnqID = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), 35, new Guid(Session["UserID"].ToString()), ForEnqID, new Guid(Session["CompanyID"].ToString())));
                    btnFqSave.Visible = true;
                    btnFpoSave.Visible = true;
                    btnSave.Visible = false;
                }
                else
                {
                    string ForEnqID = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    BindGidView(CSBL.SelectBasketItems(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), 35, new Guid(Session["UserID"].ToString()), ForEnqID, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string ForEnqID = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Response.Redirect("NewFQuotation.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + ForEnqID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Confirm Items for FPO Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFpoSave_Click(object sender, EventArgs e)
        {
            try
            {
                ModalPopupExtender1.Show();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Pop Up Okey Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOkey_Click(object sender, EventArgs e)
        {
            try
            {
                string ForEnqIDs = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataTable TCs = CommonBLL.ATConditions();
                TCs.Rows.RemoveAt(0);
                TCs.AcceptChanges();
                DataTable PercentTable = CommonBLL.FirstRowPaymentTerms();
                PercentTable.Rows.RemoveAt(0);
                DataRow dr = PercentTable.NewRow();
                dr["SNo"] = 1;
                dr["PaymentPercentage"] = 100;
                dr["Description"] = "CAD Basis";
                PercentTable.Rows.Add(dr);
                PercentTable.AcceptChanges();

                // To Get Dynamic Business nmae for FQ Number
                string path = Request.Path;
                path = Path.GetFileName(path);
                DataSet Bussname = new DataSet();
                Bussname = CSBL.SelectBasketItemsCnt(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, 35, Guid.Empty,Guid.Empty.ToString(), new Guid(Session["CompanyID"].ToString()),path);
                string FQNumber = "";
                if (Bussname != null && Bussname.Tables.Count > 2)
                {
                    FQNumber = Bussname.Tables[4].Rows[0]["Name"] + "/" + ddlcustmr.SelectedItem.Text + "/DFPO/" + Bussname.Tables[3].Rows[0]["TCnt"].ToString() + "/" + CommonBLL.FinacialYearShort; //ForEnqIDs.Replace(',', '_')
                }
                DataTable FQItems = ConveToTable(gvConformBskt);
                Decimal Total = (decimal)FQItems.Compute("SUM(Amount)", "");
                Decimal MarginFOB = ((txtMargin.Text != null && txtMargin.Text != "0") ? ((Total * Convert.ToDecimal(txtMargin.Text)) / 100) : 0);
                Decimal USTatal = ((txtConversionRt.Text != null && txtConversionRt.Text != "0") ? ((Total + MarginFOB) / Convert.ToDecimal(txtConversionRt.Text)) : 0);

                DataSet res1 = FQBL.FQuotationInsert(CommonBLL.FlagINewInsert, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty,
                    new Guid(lbFenquiry.SelectedValue), ForEnqIDs, FQNumber, "", DateTime.Now.Date, "", Math.Round(USTatal, 4),
                    Convert.ToDecimal(txtMargin.Text.Trim()), Convert.ToDecimal(txtConversionRt.Text.Trim()), Convert.ToDecimal(0),
                    Guid.Empty, "Mumbai", DateTime.Now.AddDays(7).Date, 1, Guid.Empty, CommonBLL.StatusTypeFrnQuotID, "",
                    new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, false, FQItems, PercentTable, TCs, "", "", new Guid(Session["CompanyID"].ToString()));

                if (res1 != null && res1.Tables.Count > 1 && res1.Tables[1].Rows.Count > 0)
                    Response.Redirect("~/Purchases/NewFPOrder.Aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + ForEnqIDs + "&FqId=" + res1.Tables[1].Rows[0]["FQID"].ToString(), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Confirm Items for FQ Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFqSave_Click(object sender, EventArgs e)
        {
            try
            {
                string ForEnqID = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                Response.Redirect("NewFQuotation.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + ForEnqID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                string ForEnqID = String.Join(",", lbFenquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                res = CSBL.InsertDeleteBasketItems(CommonBLL.FlagDelete, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), 35, new Guid(Session["UserID"].ToString()), ForEnqID, new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {

                    Response.Redirect("LQComparisionByItems.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + ForEnqID, false);
                    ClearAll();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "LQ-Conformation Basket", "Error While Deleting");
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion

        #region GridView Events

        /// <summary>
        /// GridVeiw Pre-Rendering Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvConformBskt_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvConformBskt.HeaderRow == null) return;
                gvConformBskt.HeaderRow.TableSection = TableRowSection.TableHeader;
                gvConformBskt.UseAccessibleHeader = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Conformation Basket", ex.Message.ToString());
            }
        }

        #endregion
    }
}
