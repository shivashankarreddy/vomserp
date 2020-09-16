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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class Gdn : System.Web.UI.Page
    {
        #region Variables
        ErrorLog ELog = new ErrorLog(); int res = 999;
        GrnBLL GDNBL = new GrnBLL();
        EnumMasterBLL EMBL = new EnumMasterBLL();
        DspchInstnsBLL DIBL = new DspchInstnsBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        static Dictionary<string, string> TeamMembers = new Dictionary<string, string>();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(Gdn));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        txtRcvdDt.Attributes.Add("readonly", "readonly");
                        txtWBDt.Attributes.Add("readonly", "readonly");
                        txtDcDt.Attributes.Add("readonly", "readonly");
                        txtInvDt.Attributes.Add("readonly", "readonly");//CancelSelection();
                        txtChqDt.Attributes.Add("readonly", "readonly");

                        if (!IsPostBack)
                        {
                            if (Session["TeamMembers"].ToString() != "All")
                                TeamMembers = Session["TeamMembers"].ToString().Split(',').ToDictionary(key => key.Trim(),
                                value => value.Trim());
                            ClearInputs();
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                BindDropDownList(ddlDspchInst, DIBL.SelectDspchInstns(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlPkngTp, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PackageTypes));
                BindDropDownList(ddlDspchDstn, EMBL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Godowns));

                if (((Request.QueryString["DspInstID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["DspInstID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["DspInstID"] = Request.QueryString["DspInstID"].ToString();
                    ddlDspchInst.SelectedValue = Request.QueryString["DspInstID"].ToString();
                    FillInputs(GDNBL.SelectGdnDtls(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlDspchInst.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlDspchInst, DIBL.SelectDspchInstns(CommonBLL.FlagLSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditGDN(GDNBL.SelectGdnDtls(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
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
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.Items.Clear();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                lb.DataSource = CommonDt.Tables[0];
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw 
        /// </summary>
        /// <param name="gvGDN"></param>
        /// <param name="dataTable"></param>
        private void BindGridVeiw(GridView gv, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    Session["GDNBindTable"] = CommonDt;
                    gv.DataSource = CommonDt;
                    gv.DataBind();
                    ((DropDownList)gvGDN.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvGDN.PageSize.ToString();
                }
                else
                {
                    Session["GDNBindTable"] = null;
                    gv.DataSource = null;
                    gv.DataBind();
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
        /// Fill Inputs
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputs(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    //ddlPlcRcpt.SelectedValue = "0";
                    ddlDspchDstn.SelectedValue = Guid.Empty.ToString();
                    txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();
                    hdfSuplrID.Value = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();

                    //SetDefaultDates();
                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Trim().ToLower().Contains(li.Value.Trim().ToLower()))
                            li.Selected = true;

                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                        CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    if (CommonDt != null && CommonDt.Tables.Count > 3 && CommonDt.Tables[3].Rows.Count > 0)
                    {
                        ViewState["DIItms"] = CommonDt.Tables[3];
                    }

                    if (CommonDt.Tables.Count > 1)
                    {
                        CheckGridItemQuantity(CommonDt.Tables[1]);
                        BindGridVeiw(gvGDN, CommonDt.Tables[1]);
                    }
                    if (CommonDt.Tables.Count > 2 && CommonDt.Tables[2].Rows.Count > 0)
                    {
                        ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[2].Rows.Cast<DataRow>()
                                                                  select new Guid(dc.Field<string>("ID").ToString())));
                    }
                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["gdndtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;
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
                        CrntQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RcvdQty)).ToString();
                        if (Convert.ToDecimal(RcvdQty) > Convert.ToDecimal(CrntQty))
                            RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(RcvdQty) - Convert.ToDecimal(CrntQty)).ToString();
                        else
                            RmngQty = (Convert.ToDecimal(ActlQty) - Convert.ToDecimal(CrntQty) - Convert.ToDecimal(RcvdQty)).ToString();

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
                Session["GDNBindTable"] = DT;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Edit Goods Dispatch Note Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditGDN(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    ddlDspchInst.SelectedValue = CommonDt.Tables[0].Rows[0]["DspchInstn"].ToString();
                    //txtDspchDstn.Text = CommonDt.Tables[0].Rows[0]["Destination"].ToString();
                    string Destination = CommonDt.Tables[0].Rows[0]["Destination"].ToString();
                    if (CommonBLL.IsValidGuid(Destination))
                    {
                        ddlDspchDstn.SelectedValue = Destination;
                        txtDspchDstn.Visible = false;
                        ddlDspchDstn.Visible = true;
                    }
                    else
                    {
                        ddlDspchDstn.Visible = false;
                        txtDspchDstn.Visible = true;
                        txtDspchDstn.Text = Destination;
                    }
                    txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();
                    hdfSuplrID.Value = CommonDt.Tables[0].Rows[0]["SuplrID"].ToString();
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
                    txtAdtnlDocs.Text = CommonDt.Tables[0].Rows[0]["ARE1No"].ToString();

                    if (CommonDt.Tables[0].Rows[0]["Attachements"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachements"].ToString().Split(',')).ToArray());
                        Session["gdndtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;
                    //SetDefaultDates();
                    //if (CommonDt.Tables.Count > 3 && CommonDt.Tables[3].Rows.Count > 0)
                    //{
                    //    ViewState["GdnItms"] = CommonDt.Tables[3];
                    //}

                    DataTable DT = CommonDt.Tables[1];
                    //int chkCount = DT.AsEnumerable().Where(r => ((r["IsItemCheck"].ToString())) > 0).ToList<DataRow>().Count();
                    DT.Select(string.Format("[DspchQty] > '{0}'", 0.ToString())).ToList<DataRow>().ForEach(k => k["IsItemCheck"] = true);
                    int chkCount = DT.AsEnumerable().Where(r => ((bool)bool.Parse(r["IsItemCheck"].ToString())).Equals(true)).ToList<DataRow>().Count();
                    int RCount = DT.AsEnumerable().Count();
                    if (chkCount == RCount)
                        ViewState["IsChecked_GDN"] = true;
                    Session["GDNBindTable"] = DT;
                    if (DT.Rows.Count > 0)
                        BindGridVeiw(gvGDN, DT);
                    SetDefaultDates();
                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().ToUpper().Contains(li.Value.ToUpper()))
                            li.Selected = true;

                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                        CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    if (CommonDt.Tables.Count > 1)
                    {
                        BindGridVeiw(gvGDN, CommonDt.Tables[1]);
                    }
                    if (CommonDt.Tables.Count > 2)
                    {
                        ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[2].Rows.Cast<DataRow>()
                                                                  select dc.Field<string>("ID").ToString()).ToArray());
                    }
                    ddlDspchInst.Enabled = false;
                    btnSave.Text = "Update";
                    DivComments.Visible = true;
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
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="Gv"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView Gv)
        {

            DataTable dt = CommonBLL.GdnItems();
            try
            {
                dt.Rows[0].Delete(); int tc = 0;
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
                DataTable DT = (DataTable)Session["GDNBindTable"];
                for (int q = 0; q < DT.Rows.Count; q++)
                {
                    DataRow dr;
                    if (Convert.ToBoolean(DT.Rows[q]["IsItemCheck"]))
                    {
                        dr = dt.NewRow();
                        dr["SNo"] = DT.Rows[q]["SNo"].ToString();//((HiddenField)row.FindControl("hfFSNo")).Value;
                        dr["ItemDtlsID"] = DT.Rows[q]["StockItemsID"];//Convert.ToString(((Label)row.FindControl("lblItemDtlsID")).Text);
                        dr["ItemId"] = new Guid(Convert.ToString(DT.Rows[q]["ItemId"]));//new Guid(((Label)row.FindControl("lblItemID")).Text.ToString());
                        dr["FrnEnqId"] = FrnEnqs;
                        dr["FrnPoId"] = FpoIds;
                        dr["LclPoId"] = DT.Rows[q]["LocalPOId"];//Convert.ToString(((Label)row.FindControl("lblLclPoID")).Text);// LpoIds;
                        //dr["GdnId"] = Convert.ToString(((TextBox)row.FindControl("txtdecrp")).Text);lblLclPoID
                        dr["Description"] = DT.Rows[q]["Description"].ToString();//Convert.ToString(((TextBox)row.FindControl("txtItmDesc")).Text);
                        dr["Quantity"] = DT.Rows[q]["Quantity"];//Convert.ToDecimal(((Label)row.FindControl("lblQuantity")).Text);
                        dr["DspchQty"] = DT.Rows[q]["DspchQty"]; //Convert.ToDecimal(((TextBox)row.FindControl("txtCrntQty")).Text);
                        dr["RcvdQty"] = DT.Rows[q]["ReceivedQty"];//Convert.ToDecimal(((Label)row.FindControl("lblRcvdQty")).Text);
                        dr["RmngQty"] = DT.Rows[q]["RemainingQty"];//Convert.ToString(((Label)row.FindControl("lblRmngQty")).Text);
                        dr["HSCode"] = DT.Rows[q]["HSCode"]; //((HiddenField)row.FindControl("hfHSCode")).Value;
                        dt.Rows.Add(dr);
                    }
                }
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }

            return dt;
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

                txtAmt.Text = (String.IsNullOrEmpty(txtAmt.Text.Trim()) ? "0" : txtAmt.Text);
                txtChqno.Text = (String.IsNullOrEmpty(txtChqno.Text.Trim()) ? "0" : txtChqno.Text);
                txtChqDt.Text = (String.IsNullOrEmpty(txtChqDt.Text.Trim()) ? DateTime.Now.ToString("dd-MM-yyyy") : txtChqDt.Text);

                //txtAreVal.Text = (String.IsNullOrEmpty(txtAreVal.Text.Trim()) ? "0" : txtAreVal.Text);

                ddlPmtPop.SelectedValue = (Convert.ToInt16(ddlFrt.SelectedValue) == 2 ? "1" : ddlPmtPop.SelectedValue);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                txtWBDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtDcDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtInvDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtChqDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                ddlPkngTp.ClearSelection(); ddlDspchDstn.ClearSelection(); ddlDspchInst.ClearSelection();
                ddlFrt.SelectedValue = hdfSuplrID.Value = Guid.Empty.ToString(); //ddlDspchInst.SelectedValue =
                ddlPmtMd.SelectedValue = ddlGdsCndtn.SelectedValue = "0";

                txtTnptrNm.Text = txtRcvdDt.Text = txtTkNo.Text = txtWBNo.Text = txtWBDt.Text = "";
                txtPkngNo.Text = txtGW.Text = txtNW.Text = txtDcNo.Text = txtDcDt.Text = txtInvNo.Text = txtInvDt.Text = "";
                txtAmt.Text = txtChqno.Text = txtChqDt.Text = txtRtgsCd.Text = txtSuplr.Text = txtremarks.Text = "";
                txtAdtnlDocs.Text = "";

                lbfpos.Items.Clear();
                lblpos.Items.Clear();
                ViewState["IsChecked_GDN"] = true;
                Session["GDNBindTable"] = null;
                Session.Remove("gdndtls");
                BindGridVeiw(gvGDN, null);
                divListBox.InnerHtml = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                        string strPath = MapPath("~/uploads/");// +Path.GetFileName(AsyncFileUpload1.FileName);
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["gdndtls"] != null)
                        {
                            alist = (ArrayList)Session["gdndtls"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["gdndtls"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["gdndtls"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
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
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["gdndtls"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["gdndtls"];
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
                return ex.Message;
            }
        }

        /// <summary>
        /// This is ussed to send mail to the team lead when user creates a new GDN
        /// </summary>
        /// <param name="ID"></param>
        private void SendMail(string GDNNumber, string ID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Dear Sir/Madam, <br/>");
                sb.Append("A GDN: " + GDNNumber + " has been created. Please check the details and approve. <br/>");
                sb.Append("<br/>");
                sb.Append("Thanks and Regards <br/>");
                sb.Append(Session["UserName"].ToString());
                string Sent = CommonBLL.SendApprovalMails(Session["TLMailID"].ToString(), "", "Waiting for Approval", sb.ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                Filename = FileName();
                string IsApproved = "";
                if (Session["AccessRole"].ToString() == CommonBLL.AdminRole && (Session["TeamMembers"].ToString() == "All" || !TeamMembers.ContainsKey(Session["UserID"].ToString())))
                    IsApproved = "Approved";
                else
                    IsApproved = "Waiting";
                //string ARE1Forms = string.Join(", ", chkblAREfms.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
                DataSet CrntId = GDNBL.SelectGdnDtls(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                DataSet DispatchSelVal = GDNBL.SelectGdnDtls(CommonBLL.FlagZSelect, Guid.Empty, new Guid(ddlDspchInst.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                string Atchmnts = Session["gdndtls"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["gdndtls"]).Cast<string>().ToArray());
                CheckInputFieldFormats();

                DataTable StockItms = ConvertToDtbl(gvGDN);
                if (btnSave.Text == "Save")
                {
                    //+ lblpos.SelectedValue + "/"
                    string GDNNo = Session["AliasName"] + "/" + DispatchSelVal.Tables[0].Rows[0]["ID"].ToString() + "/GD-" +
                    CrntId.Tables[0].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort;
                    DataSet ds = new DataSet();
                    ds = GDNBL.InsertReturnGdnID(CommonBLL.FlagNewInsert, Guid.Empty, GDNNo, new Guid(ddlDspchInst.SelectedValue),
                        FpoIds, LpoIds, ddlDspchDstn.SelectedValue, new Guid(hdfSuplrID.Value), txtTnptrNm.Text,
                        CommonBLL.DateInsert(txtRcvdDt.Text), txtTkNo.Text, txtWBNo.Text, CommonBLL.DateInsert(txtWBDt.Text),
                        new Guid(ddlPkngTp.SelectedValue), Convert.ToDecimal(txtPkngNo.Text), Convert.ToDecimal(txtGW.Text),
                        Convert.ToDecimal(txtNW.Text), txtDcNo.Text, CommonBLL.DateInsert(txtDcDt.Text), txtInvNo.Text,
                        CommonBLL.DateInsert(txtInvDt.Text), txtAdtnlDocs.Text.Trim(), "", 0,
                        Convert.ToInt32(ddlFrt.SelectedValue),
                        Convert.ToInt32(ddlPmtPop.SelectedValue), Convert.ToDecimal(txtAmt.Text), Convert.ToDecimal(txtChqno.Text),
                        CommonBLL.DateInsert(txtChqDt.Text), txtRtgsCd.Text, Convert.ToInt32(ddlGdsCndtn.SelectedValue),
                        Atchmnts, txtremarks.Text, new Guid(Session["UserID"].ToString()), StockItms, IsApproved, "", new Guid(Session["CompanyID"].ToString()));

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Count == 1)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "GDN Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        //  SendMail(GDNNo, ds.Tables[0].Rows[0][0].ToString());
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "GDN Details",
                            "Inserted successfully.");
                        ClearInputs();
                        Response.Redirect("GdnStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "GDN Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details",
                            "Error while Saving.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    string Destination = ddlDspchDstn.Visible == true ? ddlDspchDstn.SelectedValue : txtDspchDstn.Text;

                    res = GDNBL.InsertUpdateDeleteGdnDtls(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()), "",
                        new Guid(ddlDspchInst.SelectedValue), FpoIds, LpoIds, Destination,
                        new Guid(hdfSuplrID.Value), txtTnptrNm.Text, CommonBLL.DateInsert(txtRcvdDt.Text),
                        txtTkNo.Text, txtWBNo.Text, CommonBLL.DateInsert(txtWBDt.Text),
                        new Guid(ddlPkngTp.SelectedValue), Convert.ToDecimal(txtPkngNo.Text), Convert.ToDecimal(txtGW.Text),
                        Convert.ToDecimal(txtNW.Text), txtDcNo.Text, CommonBLL.DateInsert(txtDcDt.Text), txtInvNo.Text,
                        CommonBLL.DateInsert(txtInvDt.Text), txtAdtnlDocs.Text.Trim(), "", 0,
                        Convert.ToInt32(ddlFrt.SelectedValue),
                        Convert.ToInt32(ddlFrt.SelectedValue), Convert.ToDecimal(txtAmt.Text), Convert.ToDecimal(txtChqno.Text),
                        CommonBLL.DateInsert(txtChqDt.Text), txtRtgsCd.Text, Convert.ToInt32(ddlGdsCndtn.SelectedValue),
                        Atchmnts, txtremarks.Text, new Guid(Session["UserID"].ToString()), StockItms, "", txtComments.Text.Trim(), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "GDN Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "GDN Details",
                            "Updated successfully.");
                        ClearInputs();
                        Response.Redirect("GdnStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "GDN Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details",
                            "Error while Updating.");
                    }
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
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
                //DataSet CrntId = GDNBL.SelectGdnDtls(CommonBLL.FlagBSelect, 0, 0);
                //DataTable StockItms = ConvertToDtbl(gvGDN);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnNext = (Button)sender;
                int i = gvGDN.PageIndex + 1;
                if (i < gvGDN.PageCount)
                    gvGDN.PageIndex = i;
                //FillGridView(Guid.Empty);
                DataTable DT = (DataTable)Session["GDNBindTable"];
                BindGridVeiw(gvGDN, DT);

                //for (int q = 0; q < gvGDN.Rows.Count; q++)
                //{
                //    if (Convert.ToBoolean(DT.Rows[q]["IsItemCheck"]) == true)
                //    {
                //        CheckBox cb = (CheckBox)gvGDN.Rows[q].FindControl("chkbitm");
                //        cb.Checked = true;
                //        DT.Rows[q]["IsItemCheck"] = true;
                //    }
                //    else
                //    {
                //        CheckBox cb = (CheckBox)gvGDN.Rows[q].FindControl("chkbitm");
                //        cb.Checked = false;
                //        DT.Rows[q]["IsItemCheck"] = true;
                //    }
                //}
                ((DropDownList)gvGDN.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvGDN.PageSize.ToString();
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
                int i = gvGDN.PageCount;
                if (gvGDN.PageIndex > 0)
                    gvGDN.PageIndex = gvGDN.PageIndex - 1;
                DataTable DT = (DataTable)Session["GDNBindTable"];
                BindGridVeiw(gvGDN, DT);
                ((DropDownList)gvGDN.FooterRow.FindControl("ddlPageSize")).SelectedValue = gvGDN.PageSize.ToString();
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
                gvGDN.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                DataTable DT = (DataTable)Session["GDNBindTable"];
                BindGridVeiw(gvGDN, DT);
                ((DropDownList)gvGDN.FooterRow.FindControl("ddlPageSize")).SelectedValue = ddlPageSize.SelectedValue;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "New Foreign Quotation", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// Dispatch Instructions Drop Down List Selected Index Chnged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlDspchInst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlDspchInst.SelectedValue != Guid.Empty.ToString())
                {
                    string DspchInst = ddlDspchInst.SelectedValue;
                    ClearInputs();
                    ddlDspchInst.SelectedValue = DspchInst;
                    FillInputs(GDNBL.SelectGdnDtls(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlDspchInst.SelectedValue), new Guid(Session["CompanyID"].ToString())));
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                TextBox txtCrntQty = (TextBox)sender;
                GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;
                int rowIndex = Convert.ToInt32(((GridViewRow)txtCrntQty.Parent.Parent).RowIndex);
                int dtRowINdex = (gvGDN.PageIndex * gvGDN.PageSize) + rowIndex;

                Label ActlQty = (Label)currentRow.FindControl("lblQuantity");
                TextBox CrntQty = (TextBox)currentRow.FindControl("txtCrntQty");
                Label RmngQty = (Label)currentRow.FindControl("lblRmngQty");
                Label RcvdQty = (Label)currentRow.FindControl("lblRcvdQty");

                if (String.IsNullOrEmpty(CrntQty.Text) || Convert.ToDecimal(CrntQty.Text) == 0)
                {
                    CrntQty.Text = "0.00";
                    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(RcvdQty.Text)).ToString();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "EmptyQty",
                                "ErrorMessage('Dispatch Quantity should not be Empty or 0.');", true);
                }
                else
                {
                    if (Convert.ToDecimal(RcvdQty.Text) != 0)
                    {
                        if (Convert.ToDecimal(ActlQty.Text) < (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text)))
                        {
                            //CrntQty.Text = "0";
                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                                (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                            //        "ErrorMessage('Sum of Dispatch Quantity should be less than CT-1 Quantity.');", true);
                        }
                        else if (Convert.ToDecimal(ActlQty.Text) < Convert.ToDecimal(CrntQty.Text))
                        {
                            //CrntQty.Text = "0";
                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                            //        "ErrorMessage('Dispatch Quantity should be less than CT-1 Quantity.');", true);
                        }

                        else
                        {
                            if (Convert.ToDecimal(ActlQty.Text) < Convert.ToDecimal(CrntQty.Text))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                            }
                            else
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                                    (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(ActlQty.Text) < Convert.ToDecimal(CrntQty.Text))
                        {
                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                        }
                        else
                        {
                            if (Convert.ToDecimal(ActlQty.Text) < Convert.ToDecimal(CrntQty.Text))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RcvdQty",
                                    "CheckActualQty('" + CrntQty.ClientID + "', '" + RmngQty.ClientID + "', '" + ActlQty.ClientID + "');", true);
                            }
                            else
                                RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                        }
                    }
                }

                DataTable dt = (DataTable)Session["GDNBindTable"];
                dt.Rows[dtRowINdex]["ReceivedQty"] = Convert.ToDecimal(RcvdQty.Text);
                dt.Rows[dtRowINdex]["RemainingQty"] = Convert.ToDecimal(RmngQty.Text);
                dt.Rows[dtRowINdex]["DspchQty"] = Convert.ToDecimal(CrntQty.Text);
                dt.AcceptChanges();
                Session["GDNBindTable"] = dt;
                //BindGridVeiw(gvGDN, dt);
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
                int dtRowINdex = (gvGDN.PageIndex * gvGDN.PageSize) + rowIndex;

                GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;

                TextBox HscCode = (TextBox)currentRow.FindControl("txtHscCode");

                DataTable dt = (DataTable)Session["GDNBindTable"];
                dt.Rows[dtRowINdex]["HSCode"] = HscCode.Text;
                dt.AcceptChanges();
                Session["GDNBindTable"] = dt;
                //BindGridVeiw(gvGDN, dt); 
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to check all the CheckBoxes from Header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkbhdr_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox ChkParent = (CheckBox)sender;
                DataTable DT = (DataTable)Session["GDNBindTable"];
                if (ChkParent.Checked)
                {
                    ViewState["IsChecked_GDN"] = true;
                    for (int q = 0; q < DT.Rows.Count; q++)
                    {
                        int rowss = gvGDN.Rows.Count;
                        //if (Convert.ToBoolean(DT.Rows[q]["IsItemCheck"]) == false)
                        //{
                        if (q < rowss)
                        {
                            CheckBox cb = (CheckBox)gvGDN.Rows[q].FindControl("chkbitm");
                            cb.Checked = true;
                        }
                        DT.Rows[q]["IsItemCheck"] = true;
                        //}
                        //else
                        //{
                        //    if (q < rowss)
                        //    {
                        //        CheckBox cb = (CheckBox)gvGDN.Rows[q].FindControl("chkbitm");
                        //        cb.Checked = false;
                        //    }
                        //    DT.Rows[q]["IsItemCheck"] = false;
                        //}
                    }

                }
                else
                {
                    ViewState["IsChecked_GDN"] = false;
                    for (int j = 0; j < DT.Rows.Count; j++)
                    {
                        int rowss = gvGDN.Rows.Count;
                        //if (Convert.ToBoolean(DT.Rows[q]["IsItemCheck"]) == false)
                        //{
                        if (j < rowss)
                        {
                            CheckBox cb = (CheckBox)gvGDN.Rows[j].FindControl("chkbitm");
                            cb.Checked = false;
                        }
                        DT.Rows[j]["IsItemCheck"] = false;
                    }
                }
                Session["GDNBindTable"] = DT;
                BindGridVeiw(gvGDN, DT);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
            divListBox.InnerHtml = AttachedFiles();
        }

        /// <summary>
        /// This is used to check Individual CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkbitm_OnCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox ItmChkbx = (CheckBox)sender;
                GridViewRow row = (GridViewRow)ItmChkbx.Parent.Parent;
                int rowIndex = Convert.ToInt32(row.RowIndex);
                int DataIndex = Convert.ToInt32(row.DataItemIndex);

                DataTable DT = (DataTable)Session["GDNBindTable"];

                if (ItmChkbx.Checked)
                {
                    CheckBox cb = (CheckBox)gvGDN.Rows[rowIndex].FindControl("chkbitm");
                    cb.Checked = true;

                    DT.Rows[DataIndex]["IsItemCheck"] = true;
                }
                else
                    DT.Rows[DataIndex]["IsItemCheck"] = false;
                Session["GDNBindTable"] = DT;
                if (ItmChkbx.Checked == false)
                    ViewState["IsChecked_GDN"] = false;
                else
                {
                    int chkCount = DT.AsEnumerable().Where(r => ((bool)bool.Parse(r["IsItemCheck"].ToString())).Equals(true)).ToList<DataRow>().Count();
                    int RCount = DT.AsEnumerable().Count();
                    if (chkCount == RCount)
                        ViewState["IsChecked_GDN"] = true;
                }
                //BindGridVeiw(gvGDN, DT);
                Session["GDNBindTable"] = DT;
                BindGridVeiw(gvGDN, DT);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "New Local Purchase Order", ex.Message.ToString());
            }
            divListBox.InnerHtml = AttachedFiles();
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGDN_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DataTable DT = (DataTable)Session["GDNBindTable"];
                int rows = e.Row.DataItemIndex;

                if (e.Row.RowType == DataControlRowType.Header)
                    ((CheckBox)e.Row.FindControl("chkbhdr")).Checked = Convert.ToBoolean(ViewState["IsChecked_GDN"]);
                if (e.Row.RowType != DataControlRowType.DataRow)
                {
                    if (e.Row.RowType == DataControlRowType.Header)
                    {
                        e.Row.Cells[0].CssClass = "rounded-First";
                        e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-Last";
                    }

                    if (e.Row.RowType == DataControlRowType.Footer)
                    {
                        e.Row.Cells[0].CssClass = "rounded-foot-left";
                        e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "rounded-foot-right";

                        if (gvGDN.PageIndex == 0)
                            ((Button)e.Row.FindControl("btnPrevious")).Enabled = false;
                        else
                            ((Button)e.Row.FindControl("btnPrevious")).Enabled = true;

                        if (gvGDN.PageCount == (gvGDN.PageIndex) + 1)
                            ((Button)e.Row.FindControl("btnNext")).Enabled = false;
                        else
                            ((Button)e.Row.FindControl("btnNext")).Enabled = true;
                        ((DropDownList)e.Row.FindControl("ddlPageSize")).SelectedValue = gvGDN.PageSize.ToString();
                        ((Label)e.Row.FindControl("lblFooterPaging")).Text = "Total Pages: " + gvGDN.PageCount + ", Current Page:" + (gvGDN.PageIndex + 1) + ", Rows to Display:";
                    }
                }
                else
                {
                    Label ActlQty = (Label)e.Row.FindControl("lblQuantity");
                    TextBox CrntQty = (TextBox)e.Row.FindControl("txtCrntQty");
                    Label RmngQty = (Label)e.Row.FindControl("lblRmngQty");
                    Guid RefID = new Guid(((Label)e.Row.FindControl("lblItemDtlsID")).Text);
                    Label RcvdQty = (Label)e.Row.FindControl("lblRcvdQty");
                    TextBox HscCode = (TextBox)e.Row.FindControl("txtHscCode");
                    //if (Convert.ToDecimal(RcvdQty.Text) > 0)
                    //{
                    //    RcvdQty.Text = (Convert.ToDecimal(RcvdQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                    //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                    //        (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                    //}
                    //else if (ViewState["ID"] != null)
                    //{
                    //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                    //        (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                    //}
                    ////if (ViewState["ID"] == null) // && ViewState["GdnItms"] != null)
                    ////{
                    ////    RmngQty.Text = ActlQty.Text;
                    ////    CrntQty.Text = "0.00";
                    ////}
                    //else
                    //{
                    //    RmngQty.Text = ActlQty.Text;
                    //    //CrntQty.Text = "0.00";
                    //}


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
                            CrntQty.Text = RmngQty.Text;
                        }
                        if (ViewState["ID"] != null)
                        {
                            if (Convert.ToDecimal(RcvdQty.Text) != 0)
                                RcvdQty.Text = (Convert.ToDecimal(RcvdQty.Text) - Convert.ToDecimal(CrntQty.Text)).ToString();
                            else
                                RcvdQty.Text = (Convert.ToDecimal(CrntQty.Text) - Convert.ToDecimal(RcvdQty.Text)).ToString();

                            RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                                   (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString();
                        }
                        //else
                        //    RmngQty.Text = (Convert.ToDecimal(ActlQty.Text) -
                        //        (Convert.ToDecimal(CrntQty.Text) + Convert.ToDecimal(RcvdQty.Text))).ToString(); Comented this as already calculation are done in another method
                    }

                    if (ViewState["ID"] != null) //|| ViewState["DspInstID"] != null) //ViewState["DspInstID"] != null --Kept this to raise multiple GDN
                    {
                        if (Convert.ToDecimal(CrntQty.Text) > 0)
                        {
                            ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                            DT.Rows[rows]["IsItemCheck"] = true;
                        }
                        else
                        {
                            ((CheckBox)e.Row.FindControl("chkbitm")).Checked = false;
                            DT.Rows[rows]["IsItemCheck"] = false;
                        }
                        if ((Convert.ToDecimal(RmngQty.Text) <= 0))
                        {
                            if (((CheckBox)e.Row.FindControl("chkbitm")).Checked)
                            {
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = true;
                                CrntQty.Enabled = true;
                                HscCode.Enabled = true;
                                DT.Rows[rows]["IsItemCheck"] = true;
                            }
                            else
                            {
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = false;
                                CrntQty.Enabled = false;
                                HscCode.Enabled = false;
                                DT.Rows[rows]["IsItemCheck"] = false;
                            }
                        }
                    }
                    else
                    {
                        if ((Convert.ToDecimal(RmngQty.Text) <= 0) || (Convert.ToDecimal(RcvdQty.Text) == Convert.ToDecimal(ActlQty.Text)))
                        {
                            if (Convert.ToDecimal(CrntQty.Text) > 0)
                            {
                                if (Convert.ToBoolean(DT.Rows[rows]["IsItemCheck"]) == true)
                                {
                                    ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                                    DT.Rows[rows]["IsItemCheck"] = true;
                                }
                                else
                                {
                                    ((CheckBox)e.Row.FindControl("chkbitm")).Checked = false;
                                    DT.Rows[rows]["IsItemCheck"] = false;
                                }
                            }
                            if (Convert.ToDecimal(RcvdQty.Text) >= Convert.ToDecimal(ActlQty.Text))
                            {
                                ((CheckBox)e.Row.FindControl("chkbitm")).Visible = false;
                                CrntQty.Enabled = false;
                                HscCode.Enabled = false;
                                DT.Rows[rows]["IsItemCheck"] = false;
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(DT.Rows[rows]["IsItemCheck"]) == false)
                            {
                                DT.Rows[rows]["IsItemCheck"] = false;
                            }
                            else
                            {
                                DT.Rows[rows]["IsItemCheck"] = true;
                            }
                        }
                    }
                }
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                Session["GDNBindTable"] = DT;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
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
                //if (gvGDN.HeaderRow == null) return;
                //gvGDN.HeaderRow.TableSection = TableRowSection.TableHeader;
                //gvGDN.FooterRow.TableSection = TableRowSection.TableFooter;
                //gvGDN.UseAccessibleHeader = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Goods Delivery Note(GDN) Details", ex.Message.ToString());
            }
        }
        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckWayBillNo1(string waybillno)
        {
            bool Res = false;
            DataSet ds = GDNBL.SelectGdnDtls('C', Guid.Empty, Guid.Empty, "", "", waybillno, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
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
        public bool CheckDcNo1(string dcno)
        {
            bool Res = false;
            DataSet ds = GDNBL.SelectGdnDtls('I', Guid.Empty, Guid.Empty, "", "", dcno, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
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
        public bool CheckInvNo1(string invno)
        {
            bool Res = false;
            DataSet ds = GDNBL.SelectGdnDtls('J', Guid.Empty, Guid.Empty, "", "", invno, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
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
            try
            {
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["gdndtls"];
                all.RemoveAt(ID);
                Session["gdndtls"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Details", ex.Message.ToString());
                return ex.Message;
            }
        }

        #endregion

    }
}
