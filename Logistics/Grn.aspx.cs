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
using System.Text;
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class Grn : System.Web.UI.Page
    {
        #region Variables
        GrnBLL GRNBL = new GrnBLL(); int res = 999;
        ErrorLog ELog = new ErrorLog();
        ContactBLL CNTBl = new ContactBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        EnumMasterBLL EMBL = new EnumMasterBLL();
        DspchInstnsBLL DIBL = new DspchInstnsBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        CT1GenerationFormBLL CTGFBL = new CT1GenerationFormBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
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
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(Grn));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        txtRcvdDt.Attributes.Add("readonly", "readonly");
                        txtWBDt.Attributes.Add("readonly", "readonly");
                        txtDcDt.Attributes.Add("readonly", "readonly");
                        txtInvDt.Attributes.Add("readonly", "readonly");

                        if (!IsPostBack)
                        {
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                        }
                        CheckBoxesClick();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods

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
        /// Default Page Load Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                LblAmount.Text = "Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                Session.Remove("grndtls");
                DataTable CTDTLs = CommonBLL.FirstRowCT1Dtls();
                if (!CTDTLs.Columns.Contains("TempValue"))
                    CTDTLs.Columns.Add("TempValue", typeof(decimal));
                foreach (DataRow dr in CTDTLs.Rows)
                {
                    dr["TempValue"] = 0;
                }
                Session["CT1Dtls"] = CTDTLs;
                BindDropDownList(ddlCstmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlPlcRcpt, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Godowns));
                BindDropDownList(ddlPkngTp, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PackageTypes));
                BindDropDownList(ddlLocation, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Location));

                if (((Request.QueryString["gdnid"] != null && Request.QueryString["gdnid"] != "") ?
                    new Guid(Request.QueryString["gdnid"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlGDspchNote, GRNBL.SelectGdnDtls(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ddlGDspchNote.SelectedValue = Request.QueryString["gdnid"].ToString();
                    FillInputs(GRNBL.SelectGrnDtls(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(ddlGDspchNote.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlGDspchNote, GRNBL.SelectGdnDtls(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddlDpchInst, DIBL.SelectDspchInstns(CommonBLL.FlagLSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    Session["GrnID"] = Request.QueryString["ID"].ToString();
                    EditGRN(GRNBL.SelectGrnDtls(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
                if (Request.QueryString["DspInstID"] != null && Request.QueryString["DspInstID"] != ""
                    && Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    ddlCstmr.SelectedValue = Request.QueryString["CustID"].ToString();
                    Custmr_Selected();
                    chkbDpchInst.Checked = true;
                    ddlDpchInst.SelectedValue = Request.QueryString["DspInstID"].ToString();
                    ViewState["DspInstID"] = Request.QueryString["DspInstID"].ToString();
                    DispatchInst_Selected();
                }
                if (Session["CT1Dtls"] != null)
                    divCT1Dtls.InnerHtml = FillCT1Dtls();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    lb.DataSource = CommonDt.Tables[0];
                    lb.DataTextField = "Description";
                    lb.DataValueField = "ID";
                    lb.DataBind();
                }
                else
                {
                    lb.DataSource = null;
                    lb.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="CommonDt"></param>
        protected void BindGridVeiw(GridView gv, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    gv.DataSource = CommonDt;
                    gv.DataBind();
                }
                else
                {
                    gv.DataSource = null;
                    gv.DataBind();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="Gv"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView Gv)
        {
            DataTable dt = CommonBLL.GdnItems();
            dt.Rows[0].Delete(); int tc = 0;
            string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
            DataTable dtt = (DataTable)Session["GRN_Items"];

            foreach (DataRow row in dtt.Rows)
            {
                DataRow dr;
                if (Convert.ToBoolean(row["check"]) && Convert.ToDecimal(row["DspchQty"].ToString()) > 0)
                {
                    dr = dt.NewRow();
                    dr["SNo"] = row["Sno"];//((HiddenField)row.FindControl("hfFSNo")).Value;
                    dr["ItemDtlsID"] = row["StockItemsId"];//Convert.ToString(((Label)row.FindControl("lblItemDtlsID")).Text);
                    dr["ItemId"] = new Guid(row["ItemId"].ToString());//new Guid(((Label)row.FindControl("lblItemID")).Text);
                    dr["FrnEnqId"] = FrnEnqs;
                    dr["FrnPoId"] = FpoIds;
                    dr["LclPoId"] = row["LocalPOId"];//Convert.ToString(((Label)row.FindControl("lblLclPoID")).Text);
                    dr["GdnId"] = Guid.Empty;
                    dr["Description"] = row["Description"];// Convert.ToString(((TextBox)row.FindControl("txtItmDesc")).Text);
                    dr["Quantity"] = row["Quantity"];//Convert.ToString(((Label)row.FindControl("lblQuantity")).Text);
                    dr["DspchQty"] = Convert.ToDecimal(row["DspchQty"].ToString());//Convert.ToDecimal(((TextBox)row.FindControl("txtCrntQty")).Text);
                    dr["RcvdQty"] = Convert.ToDecimal(row["ReceivedQty"].ToString());//Convert.ToDecimal(((Label)row.FindControl("lblRcvdQty")).Text);
                    dr["RmngQty"] = row["RemainingQty"];//Convert.ToString(((Label)row.FindControl("lblRmngQty")).Text);
                    dr["HSCode"] = row["HSCode"];//((HiddenField)row.FindControl("hfHSCode")).Value;
                    dt.Rows.Add(dr);
                }
            }

            //foreach (GridViewRow row in Gv.Rows)
            //{
            //    DataRow dr;
            //    if (((CheckBox)row.FindControl("chkbitm")).Checked)
            //    {
            //        dr = dt.NewRow();
            //        dr["SNo"] = ((HiddenField)row.FindControl("hfFSNo")).Value;
            //        dr["ItemDtlsID"] = Convert.ToString(((Label)row.FindControl("lblItemDtlsID")).Text);
            //        dr["ItemId"] = new Guid(((Label)row.FindControl("lblItemID")).Text);
            //        dr["FrnEnqId"] = FrnEnqs;
            //        dr["FrnPoId"] = FpoIds;
            //        dr["LclPoId"] = Convert.ToString(((Label)row.FindControl("lblLclPoID")).Text);
            //        dr["GdnId"] = Guid.Empty;
            //        dr["Description"] = Convert.ToString(((TextBox)row.FindControl("txtItmDesc")).Text);
            //        dr["Quantity"] = Convert.ToString(((Label)row.FindControl("lblQuantity")).Text);
            //        dr["DspchQty"] = Convert.ToDecimal(((TextBox)row.FindControl("txtCrntQty")).Text);
            //        dr["RcvdQty"] = Convert.ToDecimal(((Label)row.FindControl("lblRcvdQty")).Text);
            //        dr["RmngQty"] = Convert.ToString(((Label)row.FindControl("lblRmngQty")).Text);
            //        dr["HSCode"] = ((HiddenField)row.FindControl("hfHSCode")).Value;
            //        dt.Rows.Add(dr);
            //    }
            //}

            return dt;
        }

        /// <summary>
        /// Fill Inputs
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputs(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    ddlPlcRcpt.SelectedValue = Guid.Empty.ToString();
                    ddlCstmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    Session["CstmrID"] = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlCstmr.Enabled = false; chkbGdnNmbr.Checked = true;
                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                    DataSet LposwithDate = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                        CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    ViewState["LposwithDate"] = LposwithDate;
                    BindListBox(lblpos, LposwithDate);
                    foreach (ListItem li in lblpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["LPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                    BindDropDownList(ddlSuplr, GRNBL.SelectGrnDtls(CommonBLL.FlagISelect, Guid.Empty, "", CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    "", new Guid(ddlCstmr.SelectedValue), Guid.Empty));
                    ddlSuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SuplrID"].ToString().ToUpperInvariant();
                    txtTnptrNm.Text = CommonDt.Tables[0].Rows[0]["TrnsptrNm"].ToString();
                    txtRcvdDt.Text = CommonDt.Tables[0].Rows[0]["DspchDt"].ToString();
                    txtWBNo.Text = CommonDt.Tables[0].Rows[0]["WayBillNo"].ToString();
                    txtWBDt.Text = CommonDt.Tables[0].Rows[0]["WayBillDt"].ToString();
                    txtTkNo.Text = CommonDt.Tables[0].Rows[0]["TruckNo"].ToString();
                    ddlPkngTp.SelectedValue = CommonDt.Tables[0].Rows[0]["PkngTp"].ToString();
                    txtPkngNo.Text = CommonDt.Tables[0].Rows[0]["PackagesNo"].ToString();
                    txtGW.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    txtNW.Text = CommonDt.Tables[0].Rows[0]["NetWeight"].ToString();
                    ddlFrt.Text = CommonDt.Tables[0].Rows[0]["Frieght"].ToString();
                    txtDcNo.Text = CommonDt.Tables[0].Rows[0]["DCNo"].ToString();
                    txtDcDt.Text = CommonDt.Tables[0].Rows[0]["DCDt"].ToString();
                    ddlGdsCndtn.SelectedValue = CommonDt.Tables[0].Rows[0]["GoodsCndtn"].ToString();
                    txtInvNo.Text = CommonDt.Tables[0].Rows[0]["InvoiceNo"].ToString();
                    txtInvDt.Text = CommonDt.Tables[0].Rows[0]["InvoiceDt"].ToString();
                    ddlPmtMd.SelectedValue = CommonDt.Tables[0].Rows[0]["Payment"].ToString();
                    if (CommonDt.Tables[0].Rows[0]["Attachements"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachements"].ToString().Split(',')).ToArray());
                        Session["grndtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;
                    if (CommonDt.Tables.Count > 1)
                    {
                        DataTable dt = CommonDt.Tables[1];

                        DataColumn dc = new DataColumn("check", typeof(Boolean));
                        dc.DefaultValue = true;
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc);
                        }
                        ViewState["IsChecked"] = true;
                        //dt.Columns.Add(dc);
                        Session["GRN_Items"] = dt;
                        CheckGridItemQuantity(dt);
                        BindGridVeiw(gvGRN, dt);

                        //Session["GRN_Items"] = CommonDt.Tables[1];
                        //BindGridVeiw(gvGRN, CommonDt.Tables[1]);
                    }
                    if (CommonDt.Tables.Count > 2)
                    {
                        if (ViewState["FrnEnqs"] != null)
                            ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[2].Rows.Cast<DataRow>()
                                                                      select dc.Field<string>("ID").ToString()).ToArray());
                    }
                    lbfpos.Enabled = lblpos.Enabled = ddlSuplr.Enabled = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowGdnDrp",
                            "ChangeDisplay('ctl00_ContentPlaceHolder1_chkbGdnNmbr', 'dvGdnNmbr');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Inputs
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputsByDspch(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    ddlPlcRcpt.SelectedValue = Guid.Empty.ToString();
                    ddlCstmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlCstmr.Enabled = false;
                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                    lbfpos.Enabled = false;
                    DataSet LposwithDate = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                        CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    ViewState["LposwithDate"] = LposwithDate;
                    BindListBox(lblpos, LposwithDate);
                    foreach (ListItem li in lblpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["LPOs"].ToString().ToUpperInvariant().Contains(li.Value.ToUpperInvariant()))
                            li.Selected = true;
                    BindDropDownList(ddlSuplr, GRNBL.SelectGrnDtls(CommonBLL.FlagISelect, Guid.Empty, "", CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    "", new Guid(ddlCstmr.SelectedValue), Guid.Empty));
                    ddlSuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString().ToUpperInvariant();
                    ddlSuplr.Enabled = false;
                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["grndtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;
                    SetDefaultDates();
                    if (CommonDt.Tables.Count > 1)
                    {
                        DataTable dt = CommonDt.Tables[1];
                        DataColumn dc = new DataColumn("check", typeof(Boolean));
                        dc.DefaultValue = false;
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc);
                        }
                        ViewState["IsChecked"] = true;
                        Session["GRN_Items"] = dt;
                        CheckGridItemQuantity(dt);
                        BindGridVeiw(gvGRN, dt);

                        //Session["GRN_Items"] = CommonDt.Tables[1];
                        //BindGridVeiw(gvGRN, CommonDt.Tables[1]);
                    }
                    if (CommonDt.Tables.Count > 2)
                    {
                        ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[2].Rows.Cast<DataRow>()
                                                                  select dc.Field<String>("ID").ToString()).ToArray());
                    }
                    ddlCstmr.Enabled = ddlSuplr.Enabled = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDspchDrp",
                            "ChangeDisplay('ctl00_ContentPlaceHolder1_chkbDpchInst', 'dvDpchInst');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Inputs
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditGRN(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    ddlCstmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    ddlGDspchNote.SelectedValue = CommonDt.Tables[0].Rows[0]["GdnID"].ToString();
                    Session["CstmrID"] = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    ViewState["GrnNmbr"] = CommonDt.Tables[0].Rows[0]["RefGRN"].ToString();

                    if (!String.IsNullOrEmpty(CommonDt.Tables[0].Rows[0]["GdnID"].ToString()) &&
                        new Guid(CommonDt.Tables[0].Rows[0]["GdnID"].ToString()) != Guid.Empty)
                    {
                        ddlGDspchNote.SelectedValue = CommonDt.Tables[0].Rows[0]["GdnID"].ToString();
                        chkbGdnNmbr.Checked = true;
                        chkbGdnNmbr.Enabled = true;
                    }
                    else
                    {
                        chkbGdnNmbr.Enabled = false;
                        chkbGdnNmbr.Checked = false;
                    }

                    if (!String.IsNullOrEmpty(CommonDt.Tables[0].Rows[0]["DspchInstID"].ToString()) &&
                    new Guid(CommonDt.Tables[0].Rows[0]["DspchInstID"].ToString()) != Guid.Empty)
                    {
                        ddlDpchInst.SelectedValue = CommonDt.Tables[0].Rows[0]["DspchInstID"].ToString();
                        chkbDpchInst.Checked = true;
                        chkbDpchInst.Enabled = true;
                    }
                    else
                    {
                        chkbDpchInst.Enabled = false;
                        chkbDpchInst.Checked = false;
                    }
                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                    DataSet LposwithDate = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                        CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    ViewState["LposwithDate"] = LposwithDate;
                    BindListBox(lblpos, LposwithDate);
                    foreach (ListItem li in lblpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["LPOs"].ToString().ToLowerInvariant().Contains(li.Value))
                            li.Selected = true;
                    BindDropDownList(ddlSuplr, GRNBL.SelectGrnDtls(CommonBLL.FlagISelect, Guid.Empty, "", CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    "", new Guid(ddlCstmr.SelectedValue), Guid.Empty));

                    ddlPlcRcpt.SelectedValue = CommonDt.Tables[0].Rows[0]["Godown"].ToString();
                    ddlSuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SuplrID"].ToString().ToUpperInvariant();
                    txtTnptrNm.Text = CommonDt.Tables[0].Rows[0]["TrnsptrNm"].ToString();
                    txtRcvdDt.Text = CommonDt.Tables[0].Rows[0]["ReceivedDt"].ToString();
                    txtWBNo.Text = CommonDt.Tables[0].Rows[0]["WayBillNo"].ToString();
                    txtWBDt.Text = CommonDt.Tables[0].Rows[0]["WayBillDt"].ToString();
                    txtTkNo.Text = CommonDt.Tables[0].Rows[0]["TruckNo"].ToString();
                    ddlPkngTp.SelectedValue = CommonDt.Tables[0].Rows[0]["PkngTp"].ToString();
                    txtPkngNo.Text = CommonDt.Tables[0].Rows[0]["PackagesNo"].ToString();
                    txtGW.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    txtNW.Text = CommonDt.Tables[0].Rows[0]["NetWeight"].ToString();
                    ddlFrt.Text = CommonDt.Tables[0].Rows[0]["Frieght"].ToString();
                    txtDcNo.Text = CommonDt.Tables[0].Rows[0]["DCNo"].ToString();
                    txtDcDt.Text = CommonDt.Tables[0].Rows[0]["DCDt"].ToString();
                    ddlGdsCndtn.SelectedValue = CommonDt.Tables[0].Rows[0]["GoodsCndtn"].ToString();
                    txtInvNo.Text = CommonDt.Tables[0].Rows[0]["InvoiceNo"].ToString();
                    txtInvDt.Text = CommonDt.Tables[0].Rows[0]["InvoiceDt"].ToString();
                    txtremarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();
                    ddlLocation.SelectedValue = CommonDt.Tables[0].Rows[0]["LocationID"].ToString();
                    txtBoxname.Text = CommonDt.Tables[0].Rows[0]["BoxName"].ToString();
                    txtDimen.Text = CommonDt.Tables[0].Rows[0]["Dimensions"].ToString();
                    if (CommonDt.Tables.Count > 3 && CommonDt.Tables[3].Rows.Count > 0)
                    {
                        ViewState["GrnItms"] = CommonDt.Tables[3];
                    }

                    if (CommonDt.Tables.Count > 2)
                    {
                        DataSet CTDTLs = new DataSet();
                        if (CommonDt.Tables[3].Rows.Count <= 0)
                            CTDTLs.Tables.Add(CommonBLL.FirstRowCT1Dtls());
                        else
                            CTDTLs.Tables.Add(CommonDt.Tables[3].Copy());

                        if (CTDTLs != null && CTDTLs.Tables.Count > 0)
                            if (!CTDTLs.Tables[0].Columns.Contains("TempValue"))
                                CTDTLs.Tables[0].Columns.Add("TempValue", typeof(decimal));
                        foreach (DataRow dr in CTDTLs.Tables[0].Rows)
                        {
                            dr["TempValue"] = dr["AREValue"];
                        }

                        Session["CT1Dtls"] = CTDTLs.Tables[0];
                    }
                    if (Session["CT1Dtls"] != null)
                        divCT1Dtls.InnerHtml = FillCT1Dtls();

                    if (CommonDt.Tables[0].Rows[0]["Attachements"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachements"].ToString().Split(',')).ToArray());
                        Session["grndtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;
                    if (CommonDt.Tables.Count > 1 && CommonDt.Tables[1].Rows.Count > 0)
                    {
                        DataTable dt = CommonDt.Tables[1];
                        DataColumn dc = new DataColumn("check", typeof(Boolean));
                        dc.DefaultValue = false;
                        if (!dt.Columns.Contains(dc.ColumnName))
                            dt.Columns.Add(dc);
                        dt.Select(string.Format("[DspchQty] > '{0}'", 0.ToString())).ToList<DataRow>().ForEach(r => r["check"] = true);
                        Session["GRN_Items"] = dt;

                        int chkCount = dt.AsEnumerable().Where(r => ((Convert.ToBoolean(r["check"]))).Equals(true)).ToList<DataRow>().Count();
                        int RCount = dt.AsEnumerable().Count();
                        if (chkCount == RCount)
                            ViewState["IsChecked"] = true;
                        CheckGridItemQuantity(dt);
                        BindGridVeiw(gvGRN, dt);

                        //Session["GRN_Items"] = CommonDt.Tables[1];
                        //BindGridVeiw(gvGRN, CommonDt.Tables[1]);
                    }
                    if (CommonDt.Tables.Count > 2 && CommonDt.Tables[2].Rows.Count > 0 && CommonDt.Tables[2].Rows[0]["ID"] != System.DBNull.Value)
                    {
                        ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[2].Rows.Cast<DataRow>()
                                                                  select dc.Field<string>("ID").ToString()).ToArray());
                    }
                    btnSave.Text = "Update";
                    DivComments.Visible = true;
                    ddlCstmr.Enabled = ddlSuplr.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Check Input Fields for Empty
        /// </summary>
        protected void CheckInputFieldFormats()
        {
            try
            {
                txtRcvdDt.Text = (String.IsNullOrEmpty(txtRcvdDt.Text.Trim()) ? DateTime.Now.ToString("dd-MM-yyyy") : txtRcvdDt.Text);
                txtWBDt.Text = (String.IsNullOrEmpty(txtWBDt.Text.Trim()) ? DateTime.Now.ToString("dd-MM-yyyy") : txtWBDt.Text);
                txtPkngNo.Text = (String.IsNullOrEmpty(txtPkngNo.Text.Trim()) ? "0" : txtPkngNo.Text);
                txtGW.Text = (String.IsNullOrEmpty(txtGW.Text.Trim()) ? "0" : txtGW.Text);
                txtNW.Text = (String.IsNullOrEmpty(txtNW.Text.Trim()) ? "0" : txtNW.Text);
                txtDcDt.Text = (String.IsNullOrEmpty(txtDcDt.Text.Trim()) ? DateTime.Now.ToString("dd-MM-yyyy") : txtDcDt.Text);
                txtInvDt.Text = (String.IsNullOrEmpty(txtInvDt.Text.Trim()) ? DateTime.Now.ToString("dd-MM-yyyy") : txtInvDt.Text);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Default Dates
        /// </summary>
        protected void SetDefaultDates()
        {
            try
            {
                txtRcvdDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to fill CT-1 Details
        /// </summary>
        /// <returns></returns>
        public string FillCT1Dtls()
        {
            try
            {
                DataTable dt = (DataTable)Session["CT1Dtls"];
                if (dt.Rows.Count <= 0)
                {
                    dt = CommonBLL.FirstRowCT1Dtls();
                    if (!dt.Columns.Contains("TempValue"))
                        dt.Columns.Add("TempValue", typeof(decimal));
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["TempValue"] = dr["AREValue"];
                    }
                }


                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='50%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblCT1Dtls' " +
                    " align='center' style='font-size: medium;'><thead align='left'><tr class='bcGridViewHeaderStyle'>");
                sb.Append("<th class='rounded-First'>SNo</th><th>CT1 Number</th><th>Date</th><th>CT1 Value</th><th>ARE-1 Number</th><th>ARE-1 Date</th><th>Forms</th><th>ARE1 Value</th>" +
                "<th class='rounded-Last'>&nbsp;</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td align='center'>" + SNo + "</td>");

                        sb.Append("<td>");
                        # region Bind-DDL
                        sb.Append("<select id='ddl" + (i + 1) + "' onchange='CT1No_Change(" + SNo + ")' Class='bcAspdropdown' width='50px'>");
                        sb.Append("<option value='0'>-SELECT-</option>");

                        DataSet ds = new DataSet();

                        if (Session["CstmrID"] != null && Session["CstmrID"].ToString() != Guid.Empty.ToString() && Session["GrnID"] == null && Session["CstmrID"] != null)
                            ds = CTGFBL.SelectCT1GF(CommonBLL.FlagODRP, new Guid(Session["CstmrID"].ToString()), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        else if (Session["CstmrID"] != null && Session["CstmrID"].ToString() != Guid.Empty.ToString() && Session["GrnID"] != null && Session["CstmrID"] != null)
                            ds = CTGFBL.SelectCT1GF(CommonBLL.FlagKSelect, new Guid(Session["CstmrID"].ToString()), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));

                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (dt.Rows[i]["RefCT1ID"].ToString() == row["ID"].ToString())
                                    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                                else
                                    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                            }
                        }

                        sb.Append("</select>");
                        # endregion
                        sb.Append("</td>");

                        sb.Append("<td><input type='text' name='txtDate' value='"
                            + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["Date"].ToString())) + "'  id='txtDate" + SNo
                            + "' onchange='AddCT1Dtls(" + SNo + ")' readonly='readonly' style='text-align: left; " +
                            " width:80px;' class='bcAsptextbox DatePicker'/></td>");

                        sb.Append("<td><input type='text' readonly='readonly' name='txtValue' value='"
                            + Convert.ToDouble(dt.Rows[i]["CT1Value"].ToString()) + "' onkeypress='return isNumberKey(event)' id='txtValue" + SNo
                            + "' onchange='AddCT1Dtls(" + SNo + ")' style='text-align: right; width:80px;' class='bcAsptextbox'/></td>");


                        sb.Append("<td><input type='text' name='txtARE1No' onfocus='this.select()' "
                                + " onMouseUp='return false' value='"
                                + dt.Rows[i]["ARE1No"].ToString().ToString()
                                + "'  id='txtARE1No" + SNo + "' onchange='AddCT1Dtls("
                                + SNo + ")' style='text-align: LEFT;' class='bcAsptextbox'/></td>");

                        sb.Append("<td><input type='text' name='txtAREDate' value='"
                            + CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["AREDate"].ToString())) + "'  id='txtAREDate" + SNo
                            + "' onchange='AddCT1Dtls(" + SNo + ")' readonly='readonly' style='text-align: left; " +
                            " width:80px;' class='bcAsptextbox DatePicker'/></td>");

                        string[] ARE1Forms = (dt.Rows[i]["Forms"].ToString()).Split(',');
                        string white = "check = 'false'", buff = "check = 'false'", blue = "check = 'false'",
                            green = "check = 'false'", pink = "check = 'false'";
                        if (ARE1Forms.Length > 1)
                        {
                            white = (Convert.ToBoolean(ARE1Forms[0].ToString()) ? "checked = 'true'" : "check = 'false'");
                            buff = (Convert.ToBoolean(ARE1Forms[1].ToString()) ? "checked = 'true'" : "check = 'false'");
                            blue = (Convert.ToBoolean(ARE1Forms[2].ToString()) ? "checked = 'true'" : "check = 'false'");
                            green = (Convert.ToBoolean(ARE1Forms[3].ToString()) ? "checked = 'true'" : "check = 'false'");
                            pink = (Convert.ToBoolean(ARE1Forms[4].ToString()) ? "checked = 'true'" : "check = 'false'");
                        }

                        sb.Append("<td><table><tr><td><input type='checkbox' onchange='AddCT1Dtls(" + SNo + ")' id='chkbARE1FormWht" + SNo
                            + "' name='chkbARE1FormWht' " + white + " title ='White' value='1' /><span style='font-size:small'>White</span> " +
                            "</td><td><input type='checkbox' " + buff + " onchange='AddCT1Dtls(" + SNo + ")' id='chkbARE1FormBff" + SNo
                            + "' name='chkbARE1FormBff' title ='Buff' value='2' /><span style='font-size:small'>Buff</span></td>" +
                            "<td><input type='checkbox' " + blue + " onchange='AddCT1Dtls(" + SNo + ")' id='chkbARE1FormBle" + SNo
                            + "' name='chkbARE1FormBle' title ='Blue' value='3' /><span style='font-size:small'>Blue</span></td>" +
                            "</tr><tr><td><input type='checkbox' " + green + " onchange='AddCT1Dtls(" + SNo + ")' id='chkbARE1FormGrn" + SNo
                            + "' name='chkbARE1FormGrn' title ='Green' value='4' /><span style='font-size:small'>Green</span></td><td>" +
                            "<input type='checkbox' " + pink + " onchange='AddCT1Dtls(" + SNo + ")' id='chkbARE1FormPnk" + SNo
                            + "' name='chkbARE1FormPnk' title ='Pink' value='5' /><span style='font-size:small'>Pink</span>" +
                            "</td><td>&nbsp;</td></tr></table></td>");

                        sb.Append("<td><input type='text' name='txtARE1Value' value='"
                            + Convert.ToDouble(dt.Rows[i]["AREValue"].ToString()) + "' onkeypress='return isNumberKey(event)' id='txtARE1Value" + SNo
                            + "' onchange='AreOne_Value(" + SNo + ")' onfocus='AreOne_ValueTemp(" + SNo + ")' style='text-align: right; width:60px;' class='bcAsptextbox'/>");
                        sb.Append("<input type='hidden' name ='hfnAREval' id='hfnAREval" + SNo + "' value='" + Convert.ToDouble(dt.Rows[i]["TempValue"].ToString())
                            + "' /></td>");


                        if (dt.Rows.Count == 1)
                            sb.Append("<td><a href='javascript:void(0)' onclick='NewCT1Dtls("
                                + SNo + ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");

                        else if (dt.Rows.Count == (i + 1))
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmCT1Dtls(" + SNo
                                + ")' title='Delete'><img src='../images/Delete.png'/></a>&nbsp;&nbsp;"
                                + "<a href='javascript:void(0)' onclick='NewCT1Dtls(" + SNo
                                + ")' class='icons additionalrow' title='Add Row'><img src='../images/add.jpeg'/></a></span></td>");
                        else
                            sb.Append("<td><span class='gridactionicons'><a href='javascript:void(0)'"
                                + " onclick='javascript:return doConfirmCT1Dtls(" + SNo
                                + ")' title='Delete'><img src='../images/Delete.png'/></a></span></td>");

                        sb.Append("</tr>");
                    }
                    sb.Append("</tbody>");
                }
                sb.Append("<tfoot><th class='rounded-foot-left'>&nbsp;</th><th colspan='8' class='rounded-foot-right' " +
                    " align='left'><input id='HfMessage' type='hidden' name='HfMessage' value='" + ((ViewState["ErrorMessage"] != null
                    && ViewState["ErrorMessage"].ToString() == "") ? ViewState["ErrorMessage"].ToString() : "") + "'/></th></tfoot>");
                sb.Append("</table>");


                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Change ARE-1 Details from Grid View to Data Table
        /// </summary>
        /// <param name="CT1Dtls"></param>
        /// <returns></returns>
        protected DataTable ChangeTbleFlds(DataTable CT1Dtls)
        {
            try
            {
                if (CT1Dtls.Columns.Contains("SNo"))
                    CT1Dtls.Columns.Remove("SNo");
                if (CT1Dtls.Columns.Contains("TempValue"))
                    CT1Dtls.Columns.Remove("TempValue");

                CT1Dtls.AcceptChanges();

                return CT1Dtls;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Send E-Mails After Generation of GRN 
        /// </summary>
        /// <param name="LpoNumber"></param>
        /// <param name="CT1Number"></param>
        /// <param name="GrnNumber"></param>
        /// <param name="SuplrName"></param>
        /// <param name="PrfmaInvcNmbr"></param>
        protected void SendDefaultMails()
        {
            try
            {
                DataTable LPOsWithDate = ((DataSet)ViewState["LposwithDate"]).Tables[0];
                string[] selval = lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
                string LPOsDts = "";

                foreach (DataRow dr in LPOsWithDate.Rows)
                {
                    foreach (string st in selval)
                    {
                        if (Convert.ToInt64(dr["ID"].ToString()) == Convert.ToInt64(st))
                        {
                            LPOsDts = LPOsDts + dr["Description"].ToString() + " Dt : " + Convert.ToDateTime(dr["Date"].ToString()).ToString("dd/MM/yyyy") + ", ";
                        }
                    }
                }

                DataTable AREDtls = (DataTable)Session["CT1Dtls"];
                string AREDts = "";
                foreach (DataRow drow in AREDtls.Rows)
                {
                    AREDts = AREDts + drow["CT1No"] + " Dt : " + drow["Date"] + ", ";
                }
                string CcAddrs = "";
                if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                {

                }
                else
                {
                    CcAddrs = Session["TLMailID"].ToString();
                }

                string Rslt1 = CommonBLL.SendMails(Session["UserMail"].ToString(), CcAddrs, "Information on GRNs", InformationGrnDtls(LPOsDts, AREDts));
                string Rslt2 = CommonBLL.SendMails(Session["UserMail"].ToString(), CcAddrs, "Request Triplicate Copy of ARE-1 form", TriplicateCopy(LPOsDts, AREDts));
                string Rslt3 = CommonBLL.SendMails(Session["UserMail"].ToString(), CcAddrs, "Reminder on ARE-1s & EP S/B Copies", AREOneAndEPSBCopy(LPOsDts, AREDts));

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about GRN Details
        /// </summary>
        /// <param name="LPOsDts"></param>
        /// <param name="AREdtls"></param>
        /// <returns></returns>
        protected string InformationGrnDtls(string LPOsDts, string AREdtls)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine);
                sb.Append("SUB: GRN Receipt " + System.Environment.NewLine);
                sb.Append("Ref: LPO No. " + LPOsDts.TrimEnd(',') + System.Environment.NewLine);
                sb.Append("Supplier: " + ddlSuplr.SelectedItem.Text + System.Environment.NewLine);
                sb.Append("W.R.T the above material has arrived under GRN: " + ViewState["GrnNmbr"].ToString() + " dt: " + DateTime.Now.ToString("dd/MM/yyyy") +
                    " at " + ddlPlcRcpt.SelectedItem.Text + System.Environment.NewLine);
                sb.Append(System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + "VIPL Godown login name ");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// E-Mail Body Format for Triplicate Copy Request
        /// </summary>
        /// <param name="LPOsDts"></param>
        /// <param name="AREdtls"></param>
        /// <returns></returns>
        protected string TriplicateCopy(string LPOsDts, string AREdtls)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine);
                sb.Append("SUB: TRIPLICATE ARE-1 Form " + System.Environment.NewLine);
                sb.Append("Ref: LPO No. " + LPOsDts.TrimEnd(',') + System.Environment.NewLine);
                sb.Append("     CT-1 No. " + AREdtls.TrimEnd(',') + System.Environment.NewLine);
                sb.Append("     GRN No. " + ViewState["GrnNmbr"].ToString() + " dt: " + DateTime.Now.ToString("dd/MM/yyyy") + System.Environment.NewLine);
                sb.Append("Supplier: " + ddlSuplr.SelectedItem.Text + System.Environment.NewLine);
                sb.Append("W.R.T the above reference, this is to remind on pending TRIPLICATE ARE-1 Form to process the POE. " + System.Environment.NewLine);
                sb.Append("PS: Update by the Excise department team in their CT-1 task page after receiving the said physical copy shall suspend this Reminder. "
                    + System.Environment.NewLine);
                sb.Append(System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + "VIPL Godown login name ");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// E-Mail Body Format for ARE-1 And EP S/B Copies Request
        /// </summary>
        /// <param name="LPOsDts"></param>
        /// <param name="AREdtls"></param>
        /// <returns></returns>
        protected string AREOneAndEPSBCopy(string LPOsDts, string AREdtls)
        {
            StringBuilder sb = new StringBuilder();
            try
            {

                sb.Append("Dear Sir/Madam " + System.Environment.NewLine);
                sb.Append("SUB: ARE-1s & EP S/B Copies " + System.Environment.NewLine);
                sb.Append("Ref: LPO No. " + LPOsDts.TrimEnd(',') + System.Environment.NewLine);
                sb.Append("     CT-1 No. " + AREdtls.TrimEnd(',') + System.Environment.NewLine);
                sb.Append("     GRN No. " + ViewState["GrnNmbr"].ToString() + " dt: " + DateTime.Now.ToString("dd/MM/yyyy") + System.Environment.NewLine);
                sb.Append("     Proforma Invoice No. " + txtInvNo.Text.Trim() + " dt: " + txtInvDt.Text.Trim() + System.Environment.NewLine);
                sb.Append("Supplier: " + ddlSuplr.SelectedItem.Text + System.Environment.NewLine);
                sb.Append("W.R.T the above details, this is to remind on the pending Original & Duplicate ARE-1, EP Shipping bill copies to process the POE. "
                    + System.Environment.NewLine);
                sb.Append("PS: Update by the Excise Department team in their CT-1 task page after receiving the said physical copies shall suspend this Reminder. "
                    + System.Environment.NewLine);
                sb.Append(System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + "VIPL Godown login name ");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                ddlGDspchNote.SelectedIndex = ddlPkngTp.SelectedIndex = ddlFrt.SelectedIndex = ddlSuplr.SelectedIndex = ddlLocation.SelectedIndex = -1;
                ddlDpchInst.SelectedIndex = ddlCstmr.SelectedIndex = ddlPmtMd.SelectedIndex =
                ddlGdsCndtn.SelectedIndex = ddlPlcRcpt.SelectedIndex = -1;
                txtTnptrNm.Text = txtRcvdDt.Text = txtTkNo.Text = txtWBNo.Text = txtWBDt.Text = "";
                txtPkngNo.Text = txtGW.Text = txtNW.Text = txtDcNo.Text = txtDcDt.Text = txtInvNo.Text = txtInvDt.Text = "";
                txtremarks.Text = txtBoxname.Text = txtDimen.Text = "";
                DataTable CTDTLs = CommonBLL.FirstRowCT1Dtls();
                if (!CTDTLs.Columns.Contains("TempValue"))
                    CTDTLs.Columns.Add("TempValue", typeof(decimal));
                foreach (DataRow dr in CTDTLs.Rows)
                {
                    dr["TempValue"] = 0;
                }
                Session["CT1Dtls"] = CTDTLs;
                divCT1Dtls.InnerHtml = FillCT1Dtls();

                lbfpos.Items.Clear();
                lblpos.Items.Clear();

                chkbDpchInst.Enabled = true;
                chkbGdnNmbr.Enabled = true;
                ddlCstmr.Enabled = true;

                Session.Remove("grndtls");
                BindGridVeiw(gvGRN, null);
                divListBox.InnerHtml = "";
                btnSave.Text = "Save";
                Session["GRN_Items"] = null;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Upload Files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileUploadComplete(object sender, EventArgs e)
        {
            try
            {
                if (AsyncFileUpload1.HasFile)
                {
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["grndtls"] != null)
                        {
                            alist = (ArrayList)Session["grndtls"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["grndtls"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["grndtls"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["grndtls"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["grndtls"];
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        sb.Append("</select>");
                    }
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Check ChechBoexes for Checked or not
        /// </summary>
        protected void CheckBoxesClick()
        {
            try
            {
                if (chkbDpchInst.Checked)
                {
                    //if (ddlCstmr.SelectedValue != "0")
                    //{
                    //    BindDropDownList(ddlDpchInst, DIBL.SelectDspchInstns(CommonBLL.FlagODRP, Guid.Empty, "",
                    //    new Guid(ddlCstmr.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    //}
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowDspchDrp",
                                "ChangeDisplay('ctl00_ContentPlaceHolder1_chkbDpchInst', 'dvDpchInst');", true);
                }
                if (chkbGdnNmbr.Checked)
                {
                    //if (ddlCstmr.SelectedValue != "0")
                    //    BindDropDownList(ddlGDspchNote, GRNBL.SelectGdnDtls(CommonBLL.FlagHSelect, new Guid(ddlCstmr.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowGdnDrp",
                                "ChangeDisplay('ctl00_ContentPlaceHolder1_chkbGdnNmbr', 'dvGdnNmbr');", true);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        private void Custmr_Selected()
        {
            try
            {
                if (ddlCstmr.SelectedValue != "0")
                {
                    string custID = ddlCstmr.SelectedValue;
                    ClearInputs();
                    ddlCstmr.SelectedValue = custID;
                    BindDropDownList(ddlDpchInst, DIBL.SelectDspchInstns(CommonBLL.FlagODRP, Guid.Empty, "",
                    new Guid(ddlCstmr.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    Session["CstmrID"] = custID;
                    if (Session["CT1Dtls"] != null)
                        divCT1Dtls.InnerHtml = FillCT1Dtls();

                    BindDropDownList(ddlGDspchNote, GRNBL.SelectGdnDtls(CommonBLL.FlagHSelect, new Guid(ddlCstmr.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    BindListBox(lbfpos, GRNBL.SelectGrnDtls(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(custID), new Guid(Session["CompanyID"].ToString())));
                    lbfpos.Enabled = true;
                }
                else
                {
                    ClearInputs();
                    chkbDpchInst.Enabled = chkbGdnNmbr.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        private void DispatchInst_Selected()
        {
            try
            {
                if (ddlDpchInst.SelectedValue != "0")
                {
                    string DspchInst = ddlDpchInst.SelectedValue;
                    ClearInputs();
                    ddlDpchInst.SelectedValue = DspchInst;
                    FillInputsByDspch(GRNBL.SelectGrnDtls(CommonBLL.FlagHSelect, Guid.Empty, new Guid(ddlDpchInst.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName(); int NoofAre1Frms = 0;
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
                string Atchmnts = Session["grndtls"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["grndtls"]).Cast<string>().ToArray());
                DataSet DispatchSelval = GRNBL.SelectGrnDtlsLpoCnt(CommonBLL.FlagZSelect, Guid.Empty, new Guid(ddlDpchInst.SelectedValue), Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                DataSet GdnSelVal = GRNBL.SelectGrnDtlsLpoCnt(CommonBLL.FlagXSelect, Guid.Empty, Guid.Empty, new Guid(ddlGDspchNote.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                string RefNo = ((ddlGDspchNote.SelectedValue != Guid.Empty.ToString()) ? GdnSelVal.Tables[0].Rows[0]["ID"].ToString() + "/" :
                    ((ddlDpchInst.SelectedValue != Guid.Empty.ToString()) ? DispatchSelval.Tables[0].Rows[0]["ID"].ToString() + "/" : ""));

                string RefNoViewState = ((ddlGDspchNote.SelectedValue != Guid.Empty.ToString()) ? ddlGDspchNote.SelectedValue + "/" :
                   ((ddlDpchInst.SelectedValue != Guid.Empty.ToString()) ? ddlDpchInst.SelectedValue + "/" : ""));
                ViewState["GrnNmbr"] = RefNoViewState;

                DataSet CrntId = GRNBL.SelectGrnDtls(CommonBLL.FlagJSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));

                CheckInputFieldFormats();
                DataTable StockItms = ConvertToDtbl(gvGRN);
                DataTable CT1UsedDetails = ChangeTbleFlds((DataTable)Session["CT1Dtls"]);
                NoofAre1Frms = CT1UsedDetails.Rows.Count;
                //for (int i = 0; i < CT1UsedDetails.Rows.Count; i++)
                //{
                //    string NofAre1Forms = CT1UsedDetails.Rows[i]["Forms"].ToString();//["Forms"].ToString();
                //    NoofAre1Frms = NofAre1Forms.Split(',').Count();//CT1UsedDetails.AsEnumerable().Count(row => row.Field<string>("Forms") != "");
                //} 
                if (btnSave.Text == "Save")
                {
                    string GRNNo = Session["AliasName"] + "/" + RefNo + "GR-" +
                        CrntId.Tables[0].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort;

                    res = GRNBL.InsertUpdateDeleteGrnDtls(CommonBLL.FlagNewInsert, Guid.Empty, GRNNo, new Guid(ddlDpchInst.SelectedValue),
                        new Guid(ddlGDspchNote.SelectedValue), FpoIds, LpoIds, new Guid(ddlPlcRcpt.SelectedValue),
                        new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), txtTnptrNm.Text,
                        CommonBLL.DateInsert(txtRcvdDt.Text), txtTkNo.Text, txtWBNo.Text, CommonBLL.DateInsert(txtWBDt.Text),
                        new Guid(ddlPkngTp.SelectedValue), Convert.ToDecimal(txtPkngNo.Text), Convert.ToDecimal(txtGW.Text),
                        Convert.ToDecimal(txtNW.Text), txtDcNo.Text, CommonBLL.DateInsert(txtDcDt.Text), txtInvNo.Text,
                        CommonBLL.DateInsert(txtInvDt.Text), "", Convert.ToString(NoofAre1Frms), 0, //txtArefmsNo.Text, ARE1Forms, Convert.ToDecimal(txtAreVal.Text), 
                        Convert.ToInt16(ddlFrt.SelectedValue), Convert.ToInt16(ddlPmtPop.SelectedValue),
                        Convert.ToInt16(ddlGdsCndtn.SelectedValue), Atchmnts, txtremarks.Text, new Guid(Session["UserID"].ToString()),
                        StockItms, "", CT1UsedDetails, "", new Guid(Session["CompanyID"].ToString()), new Guid(ddlLocation.SelectedValue), txtBoxname.Text, txtDimen.Text);

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Goods Receipt Note(GRN) Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        SendDefaultMails();

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Goods Receipt Note(GRN) Details",
                            "Inserted successfully.");
                        ClearInputs();
                        Response.Redirect("GrnStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Goods Receipt Note(GRN) Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details",
                            "Error while Saving.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = GRNBL.InsertUpdateDeleteGrnDtls(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()), "",
                        new Guid(ddlDpchInst.SelectedValue), new Guid(ddlGDspchNote.SelectedValue), FpoIds, LpoIds,
                        new Guid(ddlPlcRcpt.SelectedValue), new Guid(ddlCstmr.SelectedValue),
                        new Guid(ddlSuplr.SelectedValue), txtTnptrNm.Text,
                        CommonBLL.DateInsert(txtRcvdDt.Text), txtTkNo.Text, txtWBNo.Text, CommonBLL.DateInsert(txtWBDt.Text),
                       new Guid(ddlPkngTp.SelectedValue), Convert.ToDecimal(txtPkngNo.Text), Convert.ToDecimal(txtGW.Text),
                        Convert.ToDecimal(txtNW.Text), txtDcNo.Text, CommonBLL.DateInsert(txtDcDt.Text), txtInvNo.Text,
                        CommonBLL.DateInsert(txtInvDt.Text), "", Convert.ToString(NoofAre1Frms), 0,  //txtArefmsNo.Text, ARE1Forms, Convert.ToDecimal(txtAreVal.Text),
                        Convert.ToInt16(ddlFrt.SelectedValue),
                        Convert.ToInt16(ddlPmtPop.SelectedValue), Convert.ToInt16(ddlGdsCndtn.SelectedValue),
                        Atchmnts, txtremarks.Text, new Guid(Session["UserID"].ToString()), StockItms, "", CT1UsedDetails, txtComments.Text.Trim(),
                        new Guid(Session["CompanyID"].ToString()), new Guid(ddlLocation.SelectedValue), txtBoxname.Text, txtDimen.Text);

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Goods Receipt Note(GRN) Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Goods Receipt Note(GRN) Details",
                            "Updated successfully.");
                        ClearInputs();
                        Response.Redirect("GrnStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Goods Receipt Note(GRN) Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
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
                chkbDpchInst.Checked = chkbGdnNmbr.Checked = false;
                ClearInputs();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// Dispatch Instructions Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlDpchInst_SelectedIndexChanged(object sender, EventArgs e)
        {
            DispatchInst_Selected();
        }

        /// <summary>
        /// Dispatch Instructions Drop Down List Selected Index Chnged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlGDspchNote_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlGDspchNote.SelectedValue != Guid.Empty.ToString())
                {
                    string GDspchNote = ddlGDspchNote.SelectedValue;
                    string CustomerID = ddlCstmr.SelectedValue;
                    ClearInputs();
                    ddlCstmr.SelectedValue = CustomerID;
                    ddlGDspchNote.SelectedValue = GDspchNote;
                    FillInputs(GRNBL.SelectGrnDtls(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(ddlGDspchNote.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Customer Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCstmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            Custmr_Selected();
        }

        /// <summary>
        /// Foreign PO's DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbfpos.SelectedValue != "")
                {
                    chkbDpchInst.Enabled = chkbGdnNmbr.Enabled = false;
                    string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    BindDropDownList(ddlSuplr, GRNBL.SelectGrnDtls(CommonBLL.FlagISelect, Guid.Empty, "", FpoIds, "",
                    new Guid(ddlCstmr.SelectedValue), Guid.Empty));
                    lblpos.Items.Clear();
                    BindGridVeiw(gvGRN, null);
                    divCT1Dtls.InnerHtml = FillCT1Dtls();
                }
                else
                {
                    ddlSuplr.SelectedIndex = -1;
                    lblpos.Items.Clear();
                    BindGridVeiw(gvGRN, null);
                    chkbDpchInst.Enabled = true;
                    chkbGdnNmbr.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList Suppliers Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSuplr.SelectedValue != "0")
                {
                    string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet LposwithDate = GRNBL.SelectGrnDtls(CommonBLL.FlagBSelect, Guid.Empty, "", FpoIds, "",
                    new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue));
                    ViewState["LposwithDate"] = LposwithDate;
                    BindListBox(lblpos, LposwithDate);
                    lblpos.Enabled = true;
                    divCT1Dtls.InnerHtml = FillCT1Dtls();
                }
                else
                {
                    lblpos.Items.Clear();
                    BindGridVeiw(gvGRN, null);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList Local POs Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lblpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet CommonDt = GRNBL.SelectGrnDtls(CommonBLL.FlagCSelect, Guid.Empty, "", "", LpoIds,
                new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue));
                if (CommonDt != null && CommonDt.Tables.Count > 1 && CommonDt.Tables[1].Rows.Count > 0)
                    ViewState["GrnRefItms"] = CommonDt.Tables[1];
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = CommonDt.Tables[0];
                    if (!dt.Columns.Contains("Check"))
                    {
                        DataColumn dc = new DataColumn("check", typeof(Boolean));
                        dc.DefaultValue = false;
                        dt.Columns.Add(dc);
                    }
                    ViewState["IsChecked"] = true;
                    Session["GRN_Items"] = dt;
                    CheckGridItemQuantity(dt);
                    BindGridVeiw(gvGRN, dt);
                    //Session["GRN_Items"] = CommonDt.Tables[0];
                    //BindGridVeiw(gvGRN, CommonDt.Tables[0]);
                }
                divCT1Dtls.InnerHtml = FillCT1Dtls();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Drop Down List Freight Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlFrt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlFrt.SelectedValue == "1")
                {
                    ddlPmtPop.SelectedValue = "0";
                    ModalPopupExtender1.Show();
                }
                else
                {
                    ModalPopupExtender1.Hide();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Received Quantity Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtRcvdQty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtRcvdQty = (TextBox)sender;
                int rowIndex = Convert.ToInt32(((GridViewRow)txtRcvdQty.Parent.Parent).RowIndex);
                int dtRowINdex = (gvGRN.PageIndex * gvGRN.PageSize) + rowIndex;

                GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;

                Label ActlQty = (Label)currentRow.FindControl("lblQuantity");
                TextBox CrntQty = (TextBox)currentRow.FindControl("txtCrntQty");
                Label RmngQty = (Label)currentRow.FindControl("lblRmngQty");
                Label CTOQty = (Label)currentRow.FindControl("lblCTOQty");
                Label RcvdQty = (Label)currentRow.FindControl("lblRcvdQty");
                HiddenField DsptchQty = (HiddenField)currentRow.FindControl("HFDsptchQty");

                if (String.IsNullOrEmpty(CrntQty.Text) || Convert.ToDecimal(CrntQty.Text) == 0)
                {
                    CrntQty.Text = "0";
                    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(RcvdQty.Text)).ToString();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "EmptyQty",
                                "ErrorMessage('Arrived Quantity should not be Empty or 0.');", true);
                }
                else
                {
                    if (Convert.ToDecimal(RcvdQty.Text) != 0)
                    {
                        if (new Guid(ddlGDspchNote.SelectedValue) == Guid.Empty)
                        {
                            if (Convert.ToDecimal(ActlQty.Text) < (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text)))
                            {
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                               (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + DsptchQty.ClientID + "');", true);
                            }
                            else
                                //  RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text)) + Convert.ToDecimal(RcvdQty.Text)).ToString();
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                        }
                        else
                        {
                            if (Convert.ToDecimal(ActlQty.Text) < (Convert.ToDecimal(CrntQty.Text)))
                            {
                                RmngQty.Text = RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + DsptchQty.ClientID + "');", true);
                            }
                            else
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                            //  RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text))).ToString();
                        }
                        //else
                        //{
                        //    if (Convert.ToDecimal(ActlQty.Text) < ((Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))))
                        //    {
                        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                        //            "CheckActualQty('" + CrntQty.ClientID + "');", true);
                        //    }
                        //    else
                        //    {
                        //        if (new Guid(ddlGDspchNote.SelectedValue) == Guid.Empty)
                        //        {
                        //            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                        //                (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                        //        }
                        //        else
                        //        {
                        //            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                        //                   (Convert.ToDecimal(CrntQty.Text))).ToString();
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        if (new Guid(ddlGDspchNote.SelectedValue) == Guid.Empty)
                        {
                            if (Convert.ToDecimal(ActlQty.Text) < ((Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))))
                            {
                                string prevRemQty = RmngQty.Text;

                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                                    (Convert.ToDecimal(CrntQty.Text)) + Convert.ToDecimal(RcvdQty.Text)).ToString();
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + CrntQty.Text + "', '" + RmngQty.ClientID + "', '" + ActlQty.Text + "');", true);

                            }
                            else
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                            //RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text)) + Convert.ToDecimal(RcvdQty.Text)).ToString();
                        }
                        else
                        {
                            //if (new Guid(ddlGDspchNote.SelectedValue) == Guid.Empty)
                            //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - ((Convert.ToDecimal(CrntQty.Text)) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                            //   else
                            //RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - ((Convert.ToDecimal(CrntQty.Text)))).ToString();
                            if (Convert.ToDecimal(ActlQty.Text) < (Convert.ToDecimal(CrntQty.Text)))
                            {
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + DsptchQty.ClientID + "');", true);
                            }
                            else
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                        }
                    }
                }

                DataTable dt = (DataTable)Session["GRN_Items"];
                dt.Rows[dtRowINdex]["ReceivedQty"] = RcvdQty.Text;
                dt.Rows[dtRowINdex]["RemainingQty"] = Convert.ToDecimal(RmngQty.Text);
                dt.Rows[dtRowINdex]["DspchQty"] = Convert.ToDecimal(CrntQty.Text);
                dt.AcceptChanges();
                Session["GRN_Items"] = dt;
                //BindGridVeiw(gvGRN, dt);
                divCT1Dtls.InnerHtml = FillCT1Dtls();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }



        /// <summary>
        /// Checkiing Grid Functionality for default values binding.
        /// </summary>
        /// <param name="DT"></param>
        public void CheckGridItemQuantity(DataTable DT)
        {
            try
            {
                string CrntQty, RmngQty, ActlQty, RcvdQty = string.Empty;
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    CrntQty = DT.Rows[i]["DspchQty"].ToString();
                    ActlQty = DT.Rows[i]["quantity"].ToString();
                    RmngQty = DT.Rows[i]["RemainingQty"].ToString();
                    RcvdQty = DT.Rows[i]["ReceivedQty"].ToString();

                    if (String.IsNullOrEmpty(CrntQty) || Convert.ToDecimal(CrntQty) == 0)
                    {
                        RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RcvdQty)).ToString();
                        CrntQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RmngQty)).ToString();
                        if (Convert.ToDecimal(RcvdQty) > Convert.ToDecimal(CrntQty))
                            RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RcvdQty) - Convert.ToDecimal(CrntQty)).ToString();
                        else if (Convert.ToDecimal(CrntQty) > Convert.ToDecimal(RcvdQty))
                            RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(CrntQty) - Convert.ToDecimal(RcvdQty)).ToString();
                        else if (Convert.ToDecimal(CrntQty) == Convert.ToDecimal(RcvdQty))
                            RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RcvdQty)).ToString();
                        else
                            RmngQty = (Convert.ToDecimal(ActlQty) - (Convert.ToDecimal(CrntQty) - Convert.ToDecimal(RcvdQty))).ToString();
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "EmptyQty",
                        //            "ErrorMessage('Dispatch Quantity should not be Empty or 0.');", true);
                    }
                    else
                    {
                        if (Convert.ToDecimal(RcvdQty) != 0)
                        {
                            if (Convert.ToDecimal(ActlQty) < (Convert.ToDecimal(CrntQty) + Convert.ToDecimal(RcvdQty)))
                            {
                                //CrntQty = "0";
                                RmngQty = (Convert.ToDecimal(ActlQty) -
                                    (Convert.ToDecimal(CrntQty) + Convert.ToDecimal(RcvdQty))).ToString();
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                //        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                //        "ErrorMessage('Sum of Dispatch Quantity should be less than CT-1 Quantity.');", true);
                            }
                            else if (Convert.ToDecimal(ActlQty) < Convert.ToDecimal(CrntQty))
                            {
                                //CrntQty = "0";
                                RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(CrntQty)).ToString();
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                //        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                //        "ErrorMessage('Dispatch Quantity should be less than CT-1 Quantity.');", true);
                            }

                            else
                            {
                                if (Convert.ToDecimal(ActlQty) < Convert.ToDecimal(CrntQty))
                                {
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    //    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                                }
                                else
                                    RmngQty = (Convert.ToDecimal(ActlQty) -
                                        (Convert.ToDecimal(CrntQty) + Convert.ToDecimal(RcvdQty))).ToString();
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(ActlQty) < Convert.ToDecimal(CrntQty))
                            {
                                RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(CrntQty)).ToString();
                                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                //        "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                            }
                            else
                            {
                                if (Convert.ToDecimal(ActlQty) < Convert.ToDecimal(CrntQty))
                                {
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    //    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                                }
                                else
                                    RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(CrntQty)).ToString();
                            }
                        }
                    }

                    DT.Rows[i]["ReceivedQty"] = Convert.ToDecimal(RcvdQty);
                    DT.Rows[i]["RemainingQty"] = Convert.ToDecimal(RmngQty);
                    DT.Rows[i]["DspchQty"] = Convert.ToDecimal(CrntQty);
                }
                DT.AcceptChanges();
                Session["GRN_Items"] = DT;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Received Quantity Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtHscCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtRcvdQty = (TextBox)sender;
                int rowIndex = Convert.ToInt32(((GridViewRow)txtRcvdQty.Parent.Parent).RowIndex);
                int dtRowINdex = (gvGRN.PageIndex * gvGRN.PageSize) + rowIndex;

                GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;

                TextBox HscCode = (TextBox)currentRow.FindControl("txtHscCode");

                DataTable dt = (DataTable)Session["GRN_Items"];
                dt.Rows[dtRowINdex]["HSCode"] = HscCode.Text;
                dt.AcceptChanges();
                Session["GRN_Items"] = dt;
                BindGridVeiw(gvGRN, dt);
                divCT1Dtls.InnerHtml = FillCT1Dtls();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvGRN.HeaderRow == null) return;
                gvGRN.HeaderRow.TableSection = TableRowSection.TableHeader;
                gvGRN.FooterRow.TableSection = TableRowSection.TableFooter;
                gvGRN.UseAccessibleHeader = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row-Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGRN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DataTable DT = (DataTable)Session["GRN_Items"];
                int rows = e.Row.DataItemIndex;
                if (e.Row.RowType != DataControlRowType.DataRow)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        ((CheckBox)e.Row.FindControl("chkbhdr")).Checked = Convert.ToBoolean(ViewState["IsChecked"]);
                        e.Row.Cells[0].CssClass = "rounded-First";
                        e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-Last";
                    }

                    if (e.Row.RowType == DataControlRowType.Footer)
                    {
                        e.Row.Cells[0].CssClass = "rounded-foot-left";
                        e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-foot-right";

                        if (gvGRN.PageIndex == 0)
                            ((Button)e.Row.FindControl("btnPrev")).Enabled = false;
                        else
                            ((Button)e.Row.FindControl("btnPrev")).Enabled = true;

                        if (gvGRN.PageCount == (gvGRN.PageIndex) + 1)
                            ((Button)e.Row.FindControl("btnNext")).Enabled = false;
                        else
                            ((Button)e.Row.FindControl("btnNext")).Enabled = true;

                        ((Label)e.Row.FindControl("lblFooterPaging")).Text = "Total Pages: " + gvGRN.PageCount + ", Current Page:" + (gvGRN.PageIndex + 1) + ", Rows to Display:";
                    }
                }
                else
                {
                    DataTable dt = (DataTable)Session["GRN_Items"];
                    Label ActlQty = (Label)e.Row.FindControl("lblQuantity");
                    TextBox CrntQty = (TextBox)e.Row.FindControl("txtCrntQty");
                    Label RmngQty = (Label)e.Row.FindControl("lblRmngQty");
                    Guid RefID = new Guid(((Label)e.Row.FindControl("lblItemDtlsID")).Text);
                    Label RcvdQty = (Label)e.Row.FindControl("lblRcvdQty");
                    TextBox HscCode = (TextBox)e.Row.FindControl("txtHscCode");


                    if (Convert.ToDecimal(ActlQty.Text) == Convert.ToDecimal(RcvdQty.Text) && Convert.ToDecimal(ActlQty.Text) == Convert.ToDecimal(CrntQty.Text))
                    {
                        RmngQty.Text = (Convert.ToDecimal(CrntQty.Text) -
                            (Convert.ToDecimal(RcvdQty.Text))).ToString();
                    }
                    else
                    {
                        if (ViewState["DspInstID"] != null)
                        {
                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                                 (Convert.ToDecimal(RcvdQty.Text))).ToString();
                            CrntQty.Text = (RmngQty.Text).ToString();
                        }
                        if (ViewState["ID"] != null)
                        {
                            RcvdQty.Text = (Convert.ToDecimal(CrntQty.Text)).ToString();
                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                               (Convert.ToDecimal(RcvdQty.Text))).ToString();
                        }
                        //else
                        //RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString(); //Comented this as already calculation are done in another method
                    }
                    //else if (ViewState["ID"] != null)
                    //{
                    //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                    //}
                    //else
                    //{
                    //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                    //    //RmngQty.Text = ActlQty.Text;
                    //    //CrntQty.Text = "0.00";
                    //}

                    if (ViewState["ID"] != null)
                    {
                        if (Convert.ToDecimal(CrntQty.Text) > 0)
                        {
                            ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                            DT.Rows[rows]["check"] = true;
                        }
                        else
                        {
                            ((CheckBox)e.Row.FindControl("chkbitm")).Checked = false;
                            DT.Rows[rows]["check"] = false;
                        }
                        if ((Convert.ToDecimal(RmngQty.Text) <= 0))
                        {
                            if (((CheckBox)e.Row.FindControl("chkbitm")).Checked)
                            {
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = true;
                                CrntQty.Enabled = true;
                                HscCode.Enabled = true;
                                DT.Rows[rows]["check"] = true;
                            }
                            else
                            {
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = false;
                                CrntQty.Enabled = false;
                                HscCode.Enabled = false;
                                DT.Rows[rows]["check"] = false;
                            }
                        }
                    }
                    else
                    {
                        if ((Convert.ToDecimal(RmngQty.Text) <= 0) || (Convert.ToDecimal(RcvdQty.Text) == Convert.ToDecimal(ActlQty.Text)))
                        {
                            // if (new Guid(ddlDpchInst.SelectedValue) == Guid.Empty && new Guid(ddlGDspchNote.SelectedValue) == Guid.Empty)
                            // {
                            if (Convert.ToDecimal(CrntQty.Text) > 0)
                            {
                                if (Convert.ToBoolean(dt.Rows[rows]["check"]) == true)
                                {
                                    ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                                    dt.Rows[rows]["check"] = true;
                                }
                                else
                                {
                                    ((CheckBox)e.Row.FindControl("chkbitm")).Checked = false;
                                    dt.Rows[rows]["check"] = false;
                                }
                            }

                            if (Convert.ToDecimal(RcvdQty.Text) >= Convert.ToDecimal(ActlQty.Text))
                            {
                                //    ((CheckBox)e.Row.FindControl("chkbitm")).Checked = false;
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = false;
                                CrntQty.Enabled = false;
                                HscCode.Enabled = false;
                            }
                            // }
                        }
                        else
                        {
                            if (Convert.ToBoolean(DT.Rows[rows]["check"]) == false)
                            {
                                DT.Rows[rows]["check"] = false;
                            }
                            else
                            {
                                DT.Rows[rows]["check"] = true;
                            }
                        }
                    }
                    dt.Rows[e.Row.RowIndex]["ReceivedQty"] = RcvdQty.Text;
                    dt.Rows[e.Row.RowIndex]["RemainingQty"] = Convert.ToDecimal(RmngQty.Text);
                    dt.Rows[e.Row.RowIndex]["DspchQty"] = Math.Round(Convert.ToDecimal(CrntQty.Text));
                    dt.AcceptChanges();
                    dt.AcceptChanges();
                    Session["GRN_Items"] = dt;
                }

                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Checked Changed

        protected void chkbitm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chkChail = (CheckBox)sender;
                int rowIndex = Convert.ToInt32(((GridViewRow)chkChail.Parent.Parent).RowIndex);
                int dtRowINdex = (gvGRN.PageIndex * gvGRN.PageSize) + rowIndex;

                DataTable dt = (DataTable)Session["GRN_Items"];
                dt.Rows[dtRowINdex]["check"] = chkChail.Checked;
                dt.AcceptChanges();
                Session["GRN_Items"] = dt;

                if (chkChail.Checked == false)
                    ViewState["IsChecked"] = false;
                else
                {
                    int chkCount = dt.AsEnumerable().Where(r => (Convert.ToBoolean(r["check"])).Equals(true)).ToList<DataRow>().Count();
                    int RCount = dt.AsEnumerable().Count();
                    if (chkCount == RCount)
                        ViewState["IsChecked"] = true;
                }
                BindGridVeiw(gvGRN, dt);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        protected void chkbhdr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox ckmain = (CheckBox)gvGRN.HeaderRow.FindControl("chkbhdr");
                DataTable dt = (DataTable)Session["GRN_Items"];
                dt.AsEnumerable().Where(r => (Convert.ToBoolean(r["check"])).Equals(!ckmain.Checked)).ToList<DataRow>().ForEach(r => r["check"] = Convert.ToString(ckmain.Checked));
                dt.AcceptChanges();
                Session["GRN_Items"] = dt;
                ViewState["IsChecked"] = ckmain.Checked;
                CheckGridItemQuantity(dt);
                BindGridVeiw(gvGRN, dt);
                ckmain.Checked = ((CheckBox)sender).Checked;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnNext = (Button)sender;
                int i = gvGRN.PageIndex + 1;
                if (i < gvGRN.PageCount)
                    gvGRN.PageIndex = i;
                DataTable dt = (DataTable)Session["GRN_Items"];
                Session["GRN_Items"] = dt;
                BindGridVeiw(gvGRN, dt);
                ((DropDownList)gvGRN.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvGRN.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnPrevious = (Button)sender;
                int i = gvGRN.PageCount;
                if (gvGRN.PageIndex > 0)
                    gvGRN.PageIndex = gvGRN.PageIndex - 1;
                DataTable dt = (DataTable)Session["GRN_Items"];
                Session["GRN_Items"] = dt;
                BindGridVeiw(gvGRN, dt);
                ((DropDownList)gvGRN.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvGRN.PageSize.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        protected void ddlPageSize_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlPageSize = (DropDownList)sender;
                gvGRN.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                DataTable dt = (DataTable)Session["GRN_Items"];
                Session["GRN_Items"] = dt;
                BindGridVeiw(gvGRN, dt);
                ((DropDownList)gvGRN.FooterRow.FindControl("ddlPageSize")).SelectedValue = ddlPageSize.SelectedValue;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckWayBillNo(string waybillno)
        {
            bool Res = false;
            DataSet ds = GRNBL.SelectGrnDtls('L', Guid.Empty, waybillno, "", "", Guid.Empty, Guid.Empty);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (waybillno.ToLower() == ds.Tables[0].Rows[0][0].ToString().ToLower())
                    Res = false;
                else
                    Res = true;
            }
            else
                Res = true;
            return Res;
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckDcNo(string dcno)
        {
            bool Res = false;
            DataSet ds = GRNBL.SelectGrnDtls('O', Guid.Empty, dcno, "", "", Guid.Empty, Guid.Empty);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (dcno.ToLower() == ds.Tables[0].Rows[0][0].ToString().ToLower())
                    Res = false;
                else
                    Res = true;
            }
            else
                Res = true;
            return Res;
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckInvNo(string invno)
        {
            bool Res = false;
            DataSet ds = GRNBL.SelectGrnDtls('Q', Guid.Empty, invno, "", "", Guid.Empty, Guid.Empty);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (invno.ToLower() == ds.Tables[0].Rows[0][0].ToString().ToLower())
                    Res = false;
                else
                    Res = true;
            }
            else
                Res = true;
            return Res;
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["grndtls"];
                all.RemoveAt(ID);
                Session["grndtls"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        // ARE-1 Details.......
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddCT1Dtls(int RowNo, string CT1ID, string CT1Numbr, string CT1Date, string CT1Value, string ARE1Numbr, string ARE1Date,
            string ARE1Value, string ARE1Forms, string NewRw)
        {
            try
            {
                int MRowNo = (RowNo - 1);
                DataTable dt = new DataTable();
                dt = (DataTable)Session["CT1Dtls"];
                dt.Rows[MRowNo]["RefCT1ID"] = CT1ID;
                dt.Rows[MRowNo]["CT1No"] = CT1Numbr;
                dt.Rows[MRowNo]["CT1Value"] = CT1Value;
                dt.Rows[MRowNo]["ARE1No"] = ARE1Numbr;
                dt.Rows[MRowNo]["AREDate"] = CommonBLL.DateInsert(ARE1Date);
                dt.Rows[MRowNo]["AREValue"] = ARE1Value;
                dt.Rows[MRowNo]["Forms"] = ARE1Forms;

                if (NewRw == "NewRow")
                    dt.Rows.Add(RowNo, "", Convert.ToString(CommonBLL.DateDisplay(DateTime.Now.Date)), 0, "", Convert.ToString(CommonBLL.DateDisplay(DateTime.Now.Date)), 0, "", 0, 0);

                dt.AcceptChanges();
                Session["CT1Dtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return FillCT1Dtls();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string NewCT1Dtls(int RowNo, string CT1ID, string CT1Numbr, string CT1Date, string CT1Value, string ARE1Numbr, string ARE1Date,
            string ARE1Value, string ARE1Forms)
        {
            try
            {
                int MRowNo = (RowNo - 1);
                DataTable dt = new DataTable();
                dt = (DataTable)Session["CT1Dtls"];
                dt.Rows[MRowNo]["RefCT1ID"] = CT1ID;
                dt.Rows[MRowNo]["CT1No"] = CT1Numbr;
                dt.Rows[MRowNo]["CT1Value"] = CT1Value;
                dt.Rows[MRowNo]["ARE1No"] = ARE1Numbr;
                dt.Rows[MRowNo]["AREDate"] = CommonBLL.DateInsert(ARE1Date);
                dt.Rows[MRowNo]["AREValue"] = ARE1Value;
                dt.Rows[MRowNo]["Forms"] = ARE1Forms;
                if (CT1ID != "0" && CT1Numbr != "" && CT1Date != "" && CT1Value != "0" && ARE1Numbr != "" && ARE1Value != "0" && ARE1Forms != "")
                {
                    if (Session["GrnID"] != null && Session["GrnID"].ToString() != "")
                        dt.Rows.Add("", DateTime.Now.Date.ToString(), 0, "", DateTime.Now.Date.ToString(), 0, "", Guid.Empty, Guid.Empty, 0);
                    else
                        dt.Rows.Add(RowNo, "", DateTime.Now.Date.ToString(), 0, "", DateTime.Now.Date.ToString(), 0, "", Guid.Empty, Guid.Empty, 0);
                }

                dt.AcceptChanges();
                Session["CT1Dtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return FillCT1Dtls();
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string CT1No_Change(string RowNo, string CT1ID)
        {
            try
            {
                int MRowNo = Convert.ToInt32(RowNo) - 1;
                DataTable dt = new DataTable();
                dt = (DataTable)Session["CT1Dtls"];

                CT1DetailsBLL CDBLL = new CT1DetailsBLL();
                DataSet ds = CDBLL.GetDataSet(CommonBLL.FlagASelect, new Guid(CT1ID), new Guid(Session["CompanyID"].ToString()));
                dt.Rows[MRowNo]["RefCT1ID"] = CT1ID;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt.Rows[MRowNo]["CT1Value"] = ds.Tables[0].Rows[0]["CT1BondValue"].ToString();
                    dt.Rows[MRowNo]["Date"] = Convert.ToDateTime(ds.Tables[0].Rows[0]["CreatedDate"].ToString()).Date;
                }
                else
                {
                    dt.Rows[MRowNo]["CT1Value"] = "0";
                    dt.Rows[MRowNo]["Date"] = Convert.ToString(CommonBLL.DateDisplay(DateTime.Now.Date));
                }
                dt.AcceptChanges();
                Session["CT1Dtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return FillCT1Dtls();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string AreOne_Value(string RowNo, string CT1ID, string ARE1Val, string HdfnAREVal)
        {
            try
            {
                if (ARE1Val != "" && CT1ID != "0" && CT1ID != "")
                {
                    decimal CT1Value = 0;
                    decimal AreOneSum = 0;
                    decimal ArePrevVal = 0;
                    decimal CrntVal = 0;
                    int MRowNo = Convert.ToInt32(RowNo) - 1;
                    DataTable dt = new DataTable();
                    dt = (DataTable)Session["CT1Dtls"];

                    CT1DetailsBLL CDBLL = new CT1DetailsBLL();
                    DataSet ds = CDBLL.GetDataSet(CommonBLL.FlagBSelect, new Guid(CT1ID), new Guid(Session["CompanyID"].ToString()));
                    dt.Rows[MRowNo]["RefCT1ID"] = CT1ID;
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["ARE1Value"].ToString() != "")
                    {
                        AreOneSum = Convert.ToDecimal(ds.Tables[0].Rows[0]["ARE1Value"].ToString());
                        ArePrevVal = Convert.ToDecimal(ds.Tables[0].Rows[0]["ARE1Value"].ToString());

                        if (Convert.ToDecimal(HdfnAREVal) != 0)
                        {
                            ArePrevVal = ArePrevVal - Convert.ToDecimal(HdfnAREVal);
                            AreOneSum = AreOneSum - Convert.ToDecimal(HdfnAREVal);
                        }
                    }
                    CT1Value = Convert.ToDecimal(dt.Rows[MRowNo]["CT1Value"].ToString());

                    if ((AreOneSum + Convert.ToDecimal(ARE1Val)) <= CT1Value)
                    {
                        dt.Rows[MRowNo]["AREValue"] = ARE1Val;
                        ViewState["ErrorMessage"] = "";
                    }
                    else
                    {
                        dt.Rows[MRowNo]["AREValue"] = "0";
                        ViewState["ErrorMessage"] = "You cannot add more than " + (CT1Value - AreOneSum) + " , Previous ARE-1 value for this CT-1 is " + AreOneSum;
                    }
                    dt.AcceptChanges();
                    Session["CT1Dtls"] = dt;
                }
                else
                {
                    if (ARE1Val == "0" || ARE1Val == "")
                        ViewState["ErrorMessage"] = "Are-1 value cannot be empty or zero.";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return FillCT1Dtls();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteCT1Dtls(int RowNo)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = (DataTable)Session["CT1Dtls"];
                if (dt.Rows.Count != 1)
                {
                    dt.Rows[RowNo - 1].Delete();
                    dt.AcceptChanges();
                }
                Session["CT1Dtls"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Receipt Note(GRN) Details", ex.Message.ToString());
            }
            return FillCT1Dtls();
        }


        #endregion


    }
}
