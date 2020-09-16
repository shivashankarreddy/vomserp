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
using VOMS_ERP.Admin;
using System.IO;
using System.Text;

namespace VOMS_ERP.Invoices
{
    public partial class PrfmaInvoice : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        CustomerBLL CBLL = new CustomerBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        DataSet Dst = new DataSet();
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
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(PrfmaInvoice));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");

                        if (!IsPostBack)
                        {
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                        }
                        //if (Convert.ToInt64(ddlChkLst.SelectedValue) > 0)
                        //{
                        //    //helper = new GridViewHelper(this.GridView1, false);
                        //    helper = new GridViewHelper(this.gvPfrmaInvce, false);
                        //    ConfigSample();
                        //}
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
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
                gvPfrmaInvce.Columns[11].HeaderText = "Rate(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                gvPfrmaInvce.Columns[12].HeaderText = "Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                txtPIDate.Attributes.Add("readonly", "readonly");
                BindDropDownList(ddlNtfy, (CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                BindDropDownList(ddlChkLst, (CLBLL.SelectChekcListDtls(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                BindDropDownList(ddlBivacShpmntPlnngNo, (CLBLL.SelectChekcListDtls(CommonBLL.FlagISelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                BindDropDownList(ddlPlcOrgGds, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofOrigin).Tables[0]);
                BindDropDownList(ddlPlcFnlDstn, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty,
                    Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofFinalDestination).Tables[0]);
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading).Tables[0]);
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge).Tables[0]);
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery).Tables[0]);
                if (((Request.QueryString["ChkLstID"] != null && Request.QueryString["ChkLstID"] != "") ?
                    new Guid(Request.QueryString["ChkLstID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    string SelValue = "";

                    if (Convert.ToBoolean(Request.QueryString["IsBivac"].ToString()) == true)
                    {
                        CHkBivac.Checked = true;
                        ddlBivacShpmntPlnngNo.SelectedValue = Request.QueryString["ChkLstID"].ToString();
                        SelValue = Request.QueryString["ChkLstID"].ToString();
                        FillInputFields(CLBLL.SelectChekcListDtls(CommonBLL.FlagFSelect, new Guid(SelValue), "", new Guid(Session["CompanyID"].ToString())), true);
                        ddlChkLst.Enabled = false;
                    }
                    else
                    {
                        hfDirect.Value = "9";
                        ddlChkLst.SelectedValue = Request.QueryString["ChkLstID"].ToString();
                        SelValue = Request.QueryString["ChkLstID"].ToString();
                        FillInputFields(CLBLL.SelectChekcListDtls(CommonBLL.FlagWCommonMstr, new Guid(SelValue), "", new Guid(Session["CompanyID"].ToString())), false);
                        ddlChkLst.Enabled = true;
                    }
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                   new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditPrfmaInvc(INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, "", new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    lb.DataSource = CommonDt;
                    lb.DataTextField = "Description";
                    lb.DataValueField = "ID";
                    lb.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw 
        /// </summary>
        /// <param name="gvPfrmaInvce"></param>
        /// <param name="dataTable"></param>
        private void BindGridVeiw(GridView gv, DataTable CommonDt)
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields from Check List Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputFields(DataSet CommonDt, bool IsBivac)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 2)
                {
                    if (CommonDt.Tables[0].Rows.Count > 0)
                        BindDropDownList(ddlCstmr, CommonDt.Tables[0]);
                    ddlCstmr.SelectedIndex = 1;
                    txtPrfmInvce.Text = (CommonDt.Tables.Count > 2) ?
                        "PINV/" + Session["AliasName"] + "/" + CommonDt.Tables[3].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                    txtPIDate.Text = DateTime.Now.ToString("dd-MM-yyyy");

                    Dictionary<Guid, Guid> PF_FPOs = new Dictionary<Guid, Guid>();
                    if (!IsBivac && CommonDt != null && CommonDt.Tables.Count > 6 && CommonDt.Tables[6].Rows[0]["PF_FPOs"].ToString() != "")
                    {
                        //string FPOS = CommonDt.Tables[6].Rows[0]["PF_FPOs"].ToString().Trim(',');
                        //hfPrevFPOIDs.Value = FPOS;
                        string[] Dicval = CommonDt.Tables[6].Rows[0]["PF_FPOs"].ToString().Trim(',').Split(',');
                        PF_FPOs = Dicval.ToDictionary(s => new Guid(s.Split(',')[0]), s => new Guid(s.Split(',')[0]));
                    }
                    //else
                    //{
                    //    //Need to Check Properly Tables Count                         
                    //    //string FPOS = CommonDt.Tables[4].Rows[0]["PF_FPOs"].ToString().Trim(',');
                    //    //hfPrevFPOIDs.Value = FPOS;
                    //    string[] Dicval = CommonDt.Tables[4].Rows[0]["PF_FPOs"].ToString().Trim(',').Split(',');
                    //    PF_FPOs = Dicval.ToDictionary(s => new Guid(s.Split(',')[0]), s => new Guid(s.Split(',')[0]));
                    //}
                    Session["PF_FPOs"] = PF_FPOs;

                    if (CommonDt.Tables.Count > 3)
                    {
                        //txtPrfmInvce.Text = CommonDt.Tables[4].Rows[0]["PrfmInvcNo"].ToString();
                        //txtPIDate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[4].Rows[0]["PrfmInvcDt"].ToString()));
                        txtOtrRfs.Text = CommonDt.Tables[4].Rows[0]["OtherRef"].ToString();
                        ddlNtfy.SelectedValue = CommonDt.Tables[4].Rows[0]["Notify"].ToString();
                        ddlPlcOrgGds.SelectedValue = CommonDt.Tables[4].Rows[0]["CntryOrgGds"].ToString();
                        ddlPlcFnlDstn.SelectedValue = CommonDt.Tables[4].Rows[0]["CntryFnlDstn"].ToString();
                        ddlPrtLdng.SelectedValue = CommonDt.Tables[4].Rows[0]["PrtLdng"].ToString();
                        ddlPrtDscrg.SelectedValue = CommonDt.Tables[4].Rows[0]["PrtDschrg"].ToString();
                        ddlPlcDlry.SelectedValue = CommonDt.Tables[4].Rows[0]["PrtDlry"].ToString();
                        txtPCrBy.Text = CommonDt.Tables[4].Rows[0]["PreCrirBy"].ToString();
                        txtPlcRcptPCr.Text = CommonDt.Tables[4].Rows[0]["PlcRcptPCrirBy"].ToString();
                        txtVslFlt.Text = CommonDt.Tables[4].Rows[0]["VslFltNo"].ToString();
                        txtTrmDlryPmnt.Text = CommonDt.Tables[4].Rows[0]["TrmsDlryPymnts"].ToString();
                        //txtTrmDlryPmnt.Text = CommonDt.Tables[4].Rows[0]["TrmsDlryPymnts"].ToString();
                        //txtTrmDlryPmnt.Text = CommonDt.Tables[4].Rows[0]["TrmsDlryPymnts"].ToString();
                    }

                    if (CommonDt.Tables[1].Rows.Count > 0)
                    {
                        BindListBox(lbfpos, CommonDt.Tables[1]);
                        foreach (ListItem li in lbfpos.Items)
                            //if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }

                    Session["PrfmaInvcSubItems"] = null;
                    if (CommonDt.Tables.Count > 7 && CommonDt.Tables[7] != null)
                        Session["PrfmaInvcSubItems"] = CommonDt.Tables[7];

                    Session["PrfmaInvcChkd"] = CommonDt.Tables[2];
                    if (CommonDt.Tables[2].Rows.Count > 0 && (!CommonDt.Tables[5].Columns.Contains("IsVerbalFPO")) || Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == false)//Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == false)
                    {
                        BindGridVeiw(gvPfrmaInvce, CommonDt.Tables[2]);
                    }
                    else if (CommonDt.Tables[5] != null && CommonDt.Tables[5].Rows.Count > 0
                 && Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        Dst = CLBLL.SelectChekcListDtls(CommonBLL.FlagWCommonMstr, new Guid(ddlChkLst.SelectedValue), "", new Guid(Session["CompanyID"].ToString()));
                        if (Dst != null && Dst.Tables.Count > 0)
                        {
                            BindListBox(lbfpos, Dst.Tables[1]);
                            foreach (ListItem li in lbfpos.Items)
                            {
                                li.Selected = true;
                                lbfpos.Enabled = false;
                            }
                            BindGridVeiw(gvPfrmaInvce, Dst.Tables[2]);
                        }

                    }
                    else
                    {
                        BindGridVeiw(gvPfrmaInvce, null);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        private void ConfigSample()
        {
            try
            {
                //helper.RegisterGroup("FPONmbr", true, true);
                //helper.ApplyGroupSort();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Proforma Invoice Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditPrfmaInvc(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    hfDirect.Value = "9";
                    hfIsEdit.Value = true.ToString();
                    Dictionary<Guid, Guid> PF_FPOs = new Dictionary<Guid, Guid>();
                    if (CommonDt != null && CommonDt.Tables.Count > 6)
                    {
                        string FPOS = CommonDt.Tables[6].Rows[0]["PFE_FPOs"].ToString().Trim(',');
                        hfPrevFPOIDs.Value = FPOS;
                        if (CommonDt.Tables[6].Rows[0]["PF_FPOs"].ToString() != "")
                        {
                            string[] Dicval = CommonDt.Tables[6].Rows[0]["PF_FPOs"].ToString().Trim(',').Split(',');
                            PF_FPOs = Dicval.ToDictionary(s => new Guid(s.Split(',')[0]), s => new Guid(s.Split(',')[0]));
                        }
                    }
                    Session["PF_FPOs"] = PF_FPOs;

                    if (CommonDt.Tables.Count > 7 && CommonDt.Tables[7].Rows.Count > 0)
                        Session["PrfmaInvcSubItems"] = CommonDt.Tables[7];

                    string[] val = CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Split(',');
                    if (CommonDt.Tables.Count > 3 && CommonDt.Tables[4].Rows.Count > 0)
                    {
                        //DataTable dt = CommonDt.Tables[2];
                        //if (dt != null && dt.Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < val.Length; i++)
                        //    {
                        //        if (val[i].Trim() != "")
                        //        {
                        //            var query = dt.AsEnumerable().Where(r => r.Field<Guid>("ID") != new Guid(val[i]));
                        //            if (query != null)
                        //            {
                        //                foreach (var row in query.ToList())
                        //                {
                        //                    row.Delete();
                        //                    row.AcceptChanges();
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        if (CommonDt.Tables[0].Rows.Count > 0)
                        {
                            if (CommonDt.Tables[0].Rows[0]["IsBivac"].ToString() == "1")
                            {
                                CHkBivac.Checked = true;
                                ddlBivacShpmntPlnngNo.Enabled = true;
                                ddlChkLst.Enabled = false;
                                BindDropDownList(ddlBivacShpmntPlnngNo, CommonDt.Tables[4]);
                                ddlBivacShpmntPlnngNo.SelectedValue = CommonDt.Tables[0].Rows[0]["ChkListID"].ToString();
                            }
                            else
                            {
                                BindDropDownList(ddlChkLst, CommonDt.Tables[4]);
                                ddlChkLst.SelectedValue = CommonDt.Tables[0].Rows[0]["ChkListID"].ToString();
                                CHkBivac.Checked = false;
                                ddlBivacShpmntPlnngNo.Enabled = false;
                                ddlChkLst.Enabled = true;
                            }
                        }
                    }
                    BindDropDownList(ddlCstmr, (CLBLL.SelectChekcListDtls(CommonBLL.FlagASelect, new Guid(CommonDt.Tables[0].Rows[0]["ChkListID"].ToString()),
                    ddlCstmr.SelectedValue, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                    ddlCstmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();
                    txtPrfmInvce.Text = CommonDt.Tables[0].Rows[0]["PrfmInvcNo"].ToString();
                    txtPIDate.Text = CommonDt.Tables[0].Rows[0]["PrfmaInvcDt"].ToString();
                    txtOtrRfs.Text = CommonDt.Tables[0].Rows[0]["OtherRef"].ToString();
                    txtFrieghtAmt.Text = CommonDt.Tables[0].Rows[0]["FreightAmount"].ToString();
                    ddlNtfy.SelectedValue = CommonDt.Tables[0].Rows[0]["Notify"].ToString();
                    ddlPlcOrgGds.SelectedValue = CommonDt.Tables[0].Rows[0]["CntryOrgGds"].ToString();
                    ddlPlcFnlDstn.SelectedValue = CommonDt.Tables[0].Rows[0]["CntryFnlDstn"].ToString();
                    txtPCrBy.Text = CommonDt.Tables[0].Rows[0]["PreCrirBy"].ToString();
                    txtPlcRcptPCr.Text = CommonDt.Tables[0].Rows[0]["PlcRcptPCrirBy"].ToString();
                    txtVslFlt.Text = CommonDt.Tables[0].Rows[0]["VslFltNo"].ToString();
                    ddlPrtLdng.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLdng"].ToString();
                    ddlPrtDscrg.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDschrg"].ToString();
                    ddlPlcDlry.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDlry"].ToString();
                    txtTrmDlryPmnt.Text = CommonDt.Tables[0].Rows[0]["TrmsDlryPymnts"].ToString();

                    if (CommonDt.Tables[5] != null && CommonDt.Tables[5].Rows.Count > 0
                  && Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        BindListBox(lbfpos, CommonDt.Tables[2]);
                        foreach (ListItem li in lbfpos.Items)
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }
                    else
                    {
                        BindListBox(lbfpos, CommonDt.Tables[2]);
                        foreach (ListItem li in lbfpos.Items)
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }

                    if (CommonDt.Tables.Count > 2)
                    {
                        //DataTable tbl3 = CommonDt.Tables[3];
                        //for (int i = 0; i < val.Length; i++)
                        //{
                        //    if (val[i].ToString().Trim() != "")
                        //    {
                        //        var query = tbl3.AsEnumerable().Where(r => r.Field<Guid>("ItmForeignPOId") != new Guid(val[i]));
                        //        if (query != null)
                        //        {
                        //            foreach (var row in query.ToList())
                        //            {
                        //                row.Delete();
                        //                row.AcceptChanges();
                        //            }
                        //        }
                        //    }
                        //}
                        //ViewState["CrntItms"] = tbl3;
                        ViewState["CrntItms"] = CommonDt.Tables[3];
                        Session["PrfmaInvcChkd"] = CommonDt.Tables[3];
                    }
                    if (CommonDt.Tables.Count > 1 && CommonDt.Tables[3].Rows.Count > 0)
                        BindGridVeiw(gvPfrmaInvce, CommonDt.Tables[3]);
                    else
                    {
                        Dst = CLBLL.SelectChekcListDtls(CommonBLL.FlagSelectAll, new Guid(ddlChkLst.SelectedValue), "", new Guid(Session["CompanyID"].ToString()));
                        BindGridVeiw(gvPfrmaInvce, Dst.Tables[1]);
                    }

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["PrfmaInvcDtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;

                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="Gv"></param>
        /// <returns></returns>
        private Tuple<DataTable, string> ConvertToDtbl(GridView Gv)
        {
            try
            {
                DataTable dt = CommonBLL.PrfmaInvcItems_New();
                dt.Rows[0].Delete();
                string FpoIds = "";//String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                #region Not In Use
                //foreach (GridViewRow row in Gv.Rows)
                //{
                //    DataRow dr;
                //    if (((CheckBox)row.FindControl("chkbitm")).Checked)
                //    {
                //        dr = dt.NewRow();
                //        dr["SNo"] = Convert.ToInt32(((HiddenField)row.FindControl("hfFESNo")).Value);
                //        dr["StckItmDtlsID"] = new Guid(((Label)row.FindControl("lblsItemDtlsID")).Text);
                //        dr["ItemId"] = new Guid(((Label)row.FindControl("lblItemID")).Text);
                //        dr["FrnEnqId"] = Guid.Empty;
                //        dr["FrnPoId"] = new Guid(((Label)row.FindControl("lblFrnPoID")).Text);
                //        dr["LclPoId"] = Guid.Empty;
                //        dr["CustomerID"] = new Guid(ddlCstmr.SelectedValue);
                //        dr["Quantity"] = Convert.ToString(((Label)row.FindControl("lblQuantity")).Text);
                //        dr["Rate"] = Convert.ToString(((Label)row.FindControl("lblRate")).Text);
                //        dr["HSCode"] = ((Label)row.FindControl("lblHSCode")).Text;

                //        dt.Rows.Add(dr);
                //    }
                //} 
                #endregion

                DataTable dtChecked = (DataTable)Session["PrfmaInvcChkd"];
                if (dtChecked != null && dtChecked.Rows.Count > 0)
                {
                    DataRow dr;
                    for (int i = 0; i < dtChecked.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dtChecked.Rows[i]["IsChecked"]))
                        {
                            //dtChecked.Rows[i][""].ToString()
                            dr = dt.NewRow();
                            dr["FPOSNo"] = Convert.ToInt32(dtChecked.Rows[i]["FPOSNo"]);
                            dr["SNo"] = Convert.ToInt32(dtChecked.Rows[i]["Sno"]); //Convert.ToInt32(((HiddenField)row.FindControl("hfFESNo")).Value);
                            dr["StckItmDtlsID"] = new Guid(dtChecked.Rows[i]["StockItemsId"].ToString());//new Guid(((Label)row.FindControl("lblsItemDtlsID")).Text);
                            dr["StockItemsID"] = new Guid(dtChecked.Rows[i]["SItemsID"].ToString());
                            dr["ItemId"] = new Guid(dtChecked.Rows[i]["ItemId"].ToString());//new Guid(((Label)row.FindControl("lblItemID")).Text);
                            dr["FrnEnqId"] = Guid.Empty;
                            dr["FrnPoId"] = new Guid(dtChecked.Rows[i]["ItmForeignPOId"].ToString());//new Guid(((Label)row.FindControl("lblFrnPoID")).Text);
                            if (!FpoIds.Contains(dr["FrnPoId"].ToString()))
                                FpoIds += dr["FrnPoId"].ToString() + ",";
                            dr["LclPoId"] = Guid.Empty;
                            dr["CustomerID"] = new Guid(ddlCstmr.SelectedValue);
                            dr["Quantity"] = dtChecked.Rows[i]["DspchQty"].ToString();//Convert.ToString(((Label)row.FindControl("lblQuantity")).Text);
                            dr["Rate"] = dtChecked.Rows[i]["Rate"].ToString();//Convert.ToString(((Label)row.FindControl("lblRate")).Text);
                            dr["HSCode"] = dtChecked.Rows[i]["HSCode"].ToString();//((Label)row.FindControl("lblHSCode")).Text;

                            dt.Rows.Add(dr);
                        }
                    }
                }
                Tuple<DataTable, string> ttp = new Tuple<DataTable, string>(dt, FpoIds.Trim(','));
                return ttp;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                ddlCstmr.SelectedIndex = ddlChkLst.SelectedIndex = ddlPlcOrgGds.SelectedIndex = ddlPlcFnlDstn.SelectedIndex = ddlBivacShpmntPlnngNo.SelectedIndex = -1;
                ddlPrtLdng.SelectedIndex = ddlPrtDscrg.SelectedIndex = ddlPlcDlry.SelectedIndex = ddlNtfy.SelectedIndex = -1;
                lbfpos.Items.Clear();
                txtPrfmInvce.Text = txtPIDate.Text = txtOtrRfs.Text = "";
                txtPCrBy.Text = txtPlcRcptPCr.Text = txtVslFlt.Text = "";
                txtTrmDlryPmnt.Text = txtComments.Text = "";

                Session.Remove("PrfmaInvcDtls");
                BindGridVeiw(gvPfrmaInvce, null);
                divListBox.InnerHtml = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
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
                        if (Session["PrfmaInvcDtls"] != null)
                        {
                            alist = (ArrayList)Session["PrfmaInvcDtls"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["PrfmaInvcDtls"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["PrfmaInvcDtls"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
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
                if (Session["PrfmaInvcDtls"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["PrfmaInvcDtls"];
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                    for (int k = 0; k < all.Count; k++)
                        sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                    sb.Append("</select>");
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        /// <summary>
        /// Bind Amount By Runtime Calculations
        /// </summary>
        /// <param name="Qty"></param>
        /// <param name="Rate"></param>
        /// <returns></returns>
        protected decimal CalculateTotal(decimal Qty, decimal Rate)
        {
            return (Qty * Rate);
        }

        /// <summary>
        /// Fill Grid View
        /// </summary>
        /// <param name="EnqID"></param>
        /// <returns></returns>         
        //private string FillGridView(DataSet ds)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("");
        //        sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblItems'" +
        //            " style='font-size: small;'>" +
        //            "<thead align='left'><tr>");
        //        if (Session["CheckAllBoxes"] == null)
        //            sb.Append("<th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()'/></th>");
        //        else
        //            sb.Append("<th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()' " +
        //                " checked='checked'/></th>");

        //        sb.Append("<th>Item Desc</th><th>HS-Code</th><th>Part No</th><th>Make</th>" +
        //            "<th>Quantity</th><th>Units</th><th>Rate</th><th class='rounded-Last'>Amount</th>");
        //        sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {


        //                string sno = (i + 1).ToString();
        //                sb.Append("<tr valign='Top'>");


        //                //if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
        //                //    sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='AddItemColumn(" + sno +
        //                //        ")' type='checkbox' checked='checked' name='CheckAll'/></td>");//checkBox
        //                //else
        //                //    sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='AddItemColumn(" + sno +
        //                //        ")' type='checkbox' name='CheckAll'/></td>");//checkBox

        //                sb.Append("<td>" + (i + 1).ToString() + "");//S.NO
        //                sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + sno + ")' id='HItmID" + sno +
        //                    "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString() + "' width='5px' style='WIDTH: 5px;'/></td>");
        //                if (ds.Tables[0].Rows[i]["ItemId"].ToString() == "")
        //                //Item Desc 
        //                {
        //                    sb.Append("<td><select id='ddl" + sno + "' class='PayElementCode' onchange='FillItemDRP(" + sno + ")'>");
        //                    if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
        //                    {
        //                        sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
        //                        foreach (DataRow drr in dss.Tables[0].Rows)
        //                            if (!ItemIDs.Contains(drr["ID"].ToString()))
        //                            {
        //                                sb.Append("<option value='" + drr["ID"].ToString() + "' >" + drr["ItemDescription"].ToString()
        //                                    + "</option>");
        //                            }
        //                    }
        //                    sb.Append("</select>");
        //                    sb.Append("</td>");
        //                }
        //                else
        //                {
        //                    if ((Convert.ToBoolean(Session["NewItemAdded"]) || ds.Tables[0].Rows[i]["ItemDesc"].ToString() == "")
        //                        && (ds.Tables[0].Rows.Count - 1) == i)
        //                    {

        //                        Guid NewitemID = new Guid(ds.Tables[0].Rows[i]["ItemId"].ToString());
        //                        DataRow[] selRow = dss.Tables[0].Select("ID = '" + NewitemID.ToString() + "'");
        //                        if (selRow.Length > 0)
        //                        {
        //                            ds.Tables[0].Rows[i]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();
        //                            sb.Append("<td><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString() +
        //                                "</div></td>");//ItemDesc
        //                            Session["NewItemAdded"] = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sb.Append("<td onMouseover='toolTip(" + (i + 1) + ");'><div id='mousefollow-examples1' class='expanderR'><div id='History" + (i + 1) + "' class='Htooltip'>"
        //                            + ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</div></div></td>");
        //                        ItemIDs = ItemIDs + "," + ds.Tables[0].Rows[i]["ItemID"].ToString();
        //                    }

        //                }
        //                sb.Append("<td>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</td>");//PartNo

        //                sb.Append("<td><textarea name='txtSpecifications' Class='bcAsptextboxmulti' onchange='AddItemColumn(" + sno +
        //                ")' id='txtSpecifications" + sno + "' onfocus='ExpandTXT(" + (i + 1) + ")' onblur='ReSizeTXT(" + (i + 1) +
        //                ")' style='height:20px; width:150px; resize:none;'>"
        //                + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
        //                sb.Append("<td><input type='text' name='txtMake' onchange='AddItemColumn(" + sno + ")' id='txtMake" + sno +
        //                    "' value='" + ds.Tables[0].Rows[i]["Make"].ToString() + "' class='bcAsptextbox'/></td>");
        //                sb.Append("<td><input  Style='text-align: right; width:50px;' type='text' readonly='true' " +
        //                " name='txtQuantity' size='05px' " +
        //                    " onchange='AddItemColumn(" + sno + ")' id='txtQuantity" + sno + "' class='bcAsptextbox' value='" +
        //                    ds.Tables[0].Rows[i]["Quantity"].ToString() + "'onkeypress='return isNumberKey(event)' maxlength='6'/></td>");

        //                if (ds.Tables[0].Rows[i]["UnitName"].ToString() == "")//Units
        //                {
        //                    sb.Append("<td><select id='ddlU" + sno + "' class='PayUnitCode' onchange='AddItemColumn(" + sno + ")'>");
        //                    if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
        //                    {
        //                        sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
        //                        foreach (DataRow dru in dsEnm.Tables[0].Rows)
        //                            sb.Append("<option value='" + dru["ID"].ToString() + "' >" + dru["Description"].ToString() + "</option>");
        //                    }
        //                    sb.Append("</select></td>");
        //                }
        //                else
        //                    sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");
        //                sb.Append("<td><input type='text' name='txtRemarks' onchange='AddItemColumn(" + sno + ")' id='txtRemarks"
        //                    + sno + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString() + "' class='bcAsptextbox'/></td>");
        //                sb.Append("</tr>");
        //            }
        //        }
        //        sb.Append("</tbody>");
        //        sb.Append("<tfoot><th class='rounded-foot-left'></th>");
        //        sb.Append("<th colspan='7' style='height:17px;'><input type='hidden' name='TblErrorMessage' id='TblErrorMessage' value='"
        //        + ErrMessage + "' /></th>");
        //        sb.Append("<th class='rounded-foot-right'></th></tfoot></table>");
        //        return sb.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry", ex.Message.ToString());
        //        return ErrMsg;
        //    }
        //}

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
                string FpoIds = ""; //String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string Atchmnts = Session["PrfmaInvcDtls"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["PrfmaInvcDtls"]).Cast<string>().ToArray());
                Tuple<DataTable, string> tpl = ConvertToDtbl(gvPfrmaInvce);
                DataTable InvcItms = tpl.Item1;
                FpoIds = tpl.Item2.Trim(',');
                Boolean IsFinalInvc = false;

                DataTable dtSub = (DataTable)Session["PrfmaInvcSubItems"];
                DataRow[] drSub = null;
                if (dtSub != null && dtSub.Rows.Count > 0)
                    drSub = dtSub.Select("Amount = '0'");
                else if (dtSub == null)
                    dtSub = CommonBLL.PrfmaInvoice_SubItems();

                if (FpoIds.Trim() != "" && (drSub == null || drSub.Length == 0))
                {
                    if (dtSub.Columns.Contains("IsNew"))
                    {
                        dtSub.Columns.Remove("IsNew");
                        dtSub.AcceptChanges();
                    }
                    string[] PINV_No = new string[] { };
                    int Count =1;
                    if (InvcItms.Rows.Count > 0 && btnSave.Text == "Save")
                    {
                        Guid Id = Guid.Empty;
                        DataSet CurntID = INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, "", new Guid(Session["CompanyID"].ToString()));
                        if (CurntID.Tables[0].Rows.Count > 0)
                        {
                            PINV_No = CurntID.Tables[0].Rows[0]["Description"].ToString().Split('/');
                            Count = int.Parse(PINV_No[2]) + 1;
                        }
                        txtPrfmInvce.Text = "VIPL/PINV/" + Count + "/" + CommonBLL.FinacialYearShort;

                        if (ddlChkLst.SelectedIndex != 0)
                            Id = new Guid(ddlChkLst.SelectedValue);
                        else
                            Id = new Guid(ddlBivacShpmntPlnngNo.SelectedValue);
                        decimal FrieghtAmount = 0;
                        if (txtFrieghtAmt.Text != "")
                            FrieghtAmount = Convert.ToDecimal(txtFrieghtAmt.Text);
                        res = INBLL.InsertUpdateDeletePrfmaInvcDtls_New(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCstmr.SelectedValue), Id, FpoIds, txtPrfmInvce.Text.Trim(), CommonBLL.DateInsert(txtPIDate.Text),
                            txtOtrRfs.Text.Trim(), new Guid(ddlNtfy.SelectedValue), ddlPlcOrgGds.SelectedValue, ddlPlcFnlDstn.SelectedValue,
                            txtPCrBy.Text.Trim(), txtPlcRcptPCr.Text.Trim(), txtVslFlt.Text.Trim(), ddlPrtLdng.SelectedValue,
                            ddlPrtDscrg.SelectedValue, ddlPlcDlry.SelectedValue, txtTrmDlryPmnt.Text.Trim(), FrieghtAmount, Atchmnts, "", IsFinalInvc,
                            new Guid(Session["UserID"].ToString()), InvcItms, dtSub, new Guid(Session["CompanyId"].ToString()));

                        if (res == 0)
                        {

                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Proforma Invoice",
                                "Data inserted successfully.");
                            ClearInputs();
                            Response.Redirect("PrfmaInvoiceStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice",
                                "Error while Saving.");
                        }
                    }
                    else if (InvcItms.Rows.Count > 0 && btnSave.Text == "Update")
                    {
                        Guid Id = Guid.Empty;
                        if (ddlChkLst.SelectedIndex != 0)
                            Id = new Guid(ddlChkLst.SelectedValue);
                        else
                            Id = new Guid(ddlBivacShpmntPlnngNo.SelectedValue);
                        decimal FrieghtAmount = 0;
                        if (txtFrieghtAmt.Text != "")
                            FrieghtAmount = Convert.ToDecimal(txtFrieghtAmt.Text);
                        res = INBLL.InsertUpdateDeletePrfmaInvcDtls_New(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                            new Guid(ddlCstmr.SelectedValue), Id, FpoIds,
                            txtPrfmInvce.Text.Trim(), CommonBLL.DateInsert1(txtPIDate.Text), txtOtrRfs.Text.Trim(),
                            new Guid(ddlNtfy.SelectedValue), ddlPlcOrgGds.SelectedValue, ddlPlcFnlDstn.SelectedValue, txtPCrBy.Text.Trim(),
                            txtPlcRcptPCr.Text.Trim(), txtVslFlt.Text.Trim(), ddlPrtLdng.SelectedValue, ddlPrtDscrg.SelectedValue,
                            ddlPlcDlry.SelectedValue, txtTrmDlryPmnt.Text.Trim(), FrieghtAmount, Atchmnts, txtComments.Text, IsFinalInvc,
                            new Guid(Session["UserID"].ToString()), InvcItms, dtSub, new Guid(Session["CompanyId"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Proforma Invoice",
                                "Updated successfully.");
                            ClearInputs();
                            Response.Redirect("PrfmaInvoiceStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice",
                                "Error while Updating.");
                        }
                    }
                }
                else
                {
                    if (drSub.Length > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill All the Sub Items Properly.');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Atleast 1 FPO should be selected.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
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
                CHkBivac.Checked = false;
                //DataTable dt = new DataTable();
                //dt = ConvertToDtbl(gvPfrmaInvce);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// DropDownList Shipment Planning BIVAC/COTACNA Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlChkLst_SelectedIndexChanged1(object sender, EventArgs e)
        {
            try
            {
                if (ddlChkLst.SelectedIndex != 0 && ddlBivacShpmntPlnngNo.SelectedIndex == 0)
                    FillInputFields(CLBLL.SelectChekcListDtls(CommonBLL.FlagWCommonMstr, new Guid(ddlChkLst.SelectedValue), "", new Guid(Session["CompanyID"].ToString())), false);
                else if (ddlChkLst.SelectedIndex == 0 && ddlBivacShpmntPlnngNo.SelectedIndex != 0)
                {
                    FillInputFields(CLBLL.SelectChekcListDtls(CommonBLL.FlagFSelect, new Guid(ddlBivacShpmntPlnngNo.SelectedValue), "", new Guid(Session["CompanyID"].ToString())), true);
                    ddlChkLst.Enabled = false;
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Customer Drop Down List  Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCstmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //DataSet CommonDT = CLBLL.SelectChekcListDtls(CommonBLL.FlagCSelect, 0, ddlCstmr.SelectedValue);
                //if (CommonDT != null && CommonDT.Tables.Count > 0)
                //{
                //    BindListBox(lbfpos, CommonDT.Tables[0]);
                //    txtPrfmInvce.Text = (CommonDT.Tables.Count > 1) ?
                //        "VIPL/PINV/" + CommonDT.Tables[1].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                //    txtPIDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box FPOs Selected Index Chaned Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string FPOIDs = string.Join(",", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet CommonDT = CLBLL.SelectChekcListDtls(CommonBLL.FlagESelect, new Guid(ddlChkLst.SelectedValue),
                    ddlCstmr.SelectedValue, FPOIDs);
                if (CommonDT != null && CommonDT.Tables.Count > 0)
                {
                    BindGridVeiw(gvPfrmaInvce, CommonDT.Tables[0]);
                    txtPrfmInvce.Text = (CommonDT.Tables.Count > 1) ?
                        "VIPL/PINV/" + CommonDT.Tables[1].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                    txtPIDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                }
                //FillInputs(GDNBL.SelectGdnDtls(CommonBLL.FlagESelect, 0, Int64.Parse(ddlDspchInst.SelectedValue)));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Check Box BIVAC Checked Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CHkBivac_CheckedChanged(object sender, EventArgs e)
        {
            ClearInputs();
            if (CHkBivac.Checked)
            {
                ddlBivacShpmntPlnngNo.Enabled = true;
                ddlChkLst.Enabled = false;
            }
            else
            {
                ddlBivacShpmntPlnngNo.Enabled = false;
                ddlChkLst.Enabled = true;
            }
        }

        /// <summary>
        /// HSCode Text Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtHscCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox test = (TextBox)sender;
                int rowIndex = Convert.ToInt32(((GridViewRow)test.Parent.Parent).RowIndex);
                int dtRowINdex = (gvPfrmaInvce.PageIndex * gvPfrmaInvce.PageSize) + rowIndex;

                GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;

                TextBox HscCode = (TextBox)currentRow.FindControl("txtHSCode");

                DataTable dt = (DataTable)Session["PrfmaInvcChkd"];
                dt.Rows[dtRowINdex]["HSCode"] = HscCode.Text;
                dt.AcceptChanges();
                hdnScroll2Row.Value = dtRowINdex.ToString();
                Session["PrfmaInvcChkd"] = dt;
                BindGridVeiw(gvPfrmaInvce, dt);
                HscCode.Focus();
                //divP.InnerHtml = FillCT1Dtls();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPfrmaInvce_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    //e.Row.Cells[4].Text = "Rate(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                    //e.Row.Cells[5].Text = "Amount(" + Session["CurrencySymbol"].ToString().Trim() + ")";
                }
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    Guid ItemID = new Guid(((HiddenField)e.Row.FindControl("hfItemID")).Value.Trim());
                    bool hfIsSubItem = Convert.ToBoolean(((HiddenField)e.Row.FindControl("hfIsSubItem")).Value);
                    HtmlImage img = (HtmlImage)e.Row.FindControl("imgExpand");
                    //if (hfIsSubItem)
                    //{
                    //if (Session["PrfmaInvcSubItems"] != null && ((DataTable)Session["PrfmaInvcSubItems"]).Rows.Count > 0)
                    //{

                    //    //lblSubItems.Text = fillSubItems(ItemID);
                    //    //dvSubItems.InnerHtml = fillSubItems(ItemID);
                    //}

                    //}
                    //else
                    //    img.Visible = false;


                    if (ViewState["CrntItms"] != null)
                    {
                        DataTable CrntItms = (DataTable)ViewState["CrntItms"];
                        Guid RefID = new Guid(((Label)e.Row.FindControl("lblsItemDtlsID")).Text);
                        ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                        //(from row in CrntItms.Rows.Cast<DataRow>() where row.Field<Int64>("StockItemsId") == RefID select row).Count() > 0 ?
                        //true : false;
                    }
                    decimal amount = Convert.ToDecimal(((Label)e.Row.FindControl("lblAmount")).Text);
                    Label lblAmount = (Label)e.Row.FindControl("lblAmount");
                    lblAmount.Text = amount.ToString("N");

                    TextBox HscCode = (TextBox)e.Row.FindControl("txtHSCode");
                    HscCode.Focus();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        private string fillSubItems(Guid FPOID, string ParentID, string FESNO, string _SNO)
        {
            try
            {
                DataTable dt = (DataTable)Session["PrfmaInvcSubItems"];
                StringBuilder sb = new StringBuilder();

                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0'>");
                DataRow[] dr = null;

                if (dt != null && dt.Rows.Count > 0)
                    dr = dt.Select("ParentItemID = '" + ParentID + "'");
                //sb.Append("<thead>");
                //sb.Append("<th>Item Desc</th><th>HS-Code</th><th>Part No</th><th>Make</th><th>Quantity</th><th>Units</th><th>Rate</th><th>Amount</th><th></th><th></th>");
                //sb.Append("</thead>");
                sb.Append("<tbody>");
                if (dr != null && dr.Length > 0 && dr[0][0].ToString() != "")
                {
                    string SNo = "0";
                    for (int i = 0; i < dr.Length; i++)
                    {
                        string ItemID = dr[i]["ItemId"].ToString();
                        SNo = FESNO + "." + (i + 1).ToString();
                        dr[i]["SNo"] = SNo;

                        string OnChange = "&#39;" + ParentID + "&#39;,&#39;" + SNo + "&#39;,this,false";
                        string OnChange_ADD = "&#39;" + ParentID + "&#39;,&#39;" + SNo + "&#39;,this,true";
                        sb.Append("<tr>");
                        sb.Append("<td><input type='text' id='txtDescription" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["ItemDes"].ToString() + "' style='width:350px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtHsCode" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["HSCode"].ToString() + "' style='width:80px;' class='subclass'/>" +
                            "<input type='hidden' id='hfItemID" + SNo + "' value='" + ItemID + "'/></td>");

                        sb.Append("<td><input type='text' id='txtPartNo" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["PartNumber"].ToString() + "' style='width:80px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtMake" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["Make"].ToString() + "' style='width:80px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtQuantity" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["Quantity"].ToString() + "' style='width:80px;' class='subclass'"
                        + "' onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' "
                        + "onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");

                        string Units = BindUnits(SNo, dr[i]["UnumsId"].ToString(), "" + ParentID + "");
                        sb.Append("<td>" + Units + "</td>"); //Units

                        sb.Append("<td><input type='text' id='txtRate" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["Rate"].ToString() + "' style='width:80px;' class='subclass'"
                        + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' "
                        + "onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");

                        decimal Amount = Convert.ToDecimal(dr[i]["Rate"].ToString()) * Convert.ToDecimal(dr[i]["Quantity"].ToString());

                        sb.Append("<td><label id='txtAmount" + SNo + "' style='width:80px;' class='subclass'>" + Amount.ToString() + "</label></td>");
                        dr[i]["Amount"] = Amount;

                        if (dr.Length - 1 == i)
                            sb.Append("<td valign='center'><span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return Delete_SubItem(&#39;" + SNo + "&#39;, this,&#39;" + ItemID + "&#39;)' " +
                                " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td valign='center'>" +
                                "<a href='javascript:void(0)' onclick='SaveChanges(" + OnChange_ADD + ")' " +
                                " class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></td>");
                        else
                            sb.Append("<td valign='center'><span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return Delete_SubItem(&#39;" + SNo + "&#39;, this,&#39;" + ItemID + "&#39;)' class='icons deleteicon' title='Delete'>" +
                                " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td></td>");

                        sb.Append("</tr>");
                    }
                    dt.AcceptChanges();
                    Session["PrfmaInvcSubItems"] = dt;
                }
                else
                {
                    sb.Append("<tr>");
                    string OnChange = "&#39;" + ParentID + "&#39;,&#39;" + FESNO + "&#39;,this,false";
                    string OnChange_ADD = "&#39;" + ParentID + "&#39;,&#39;" + FESNO + "&#39;,this,true";
                    sb.Append("<td><input type='text' id='txtDescription" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='' style='width:350px;' class='subclass'/></td>");
                    sb.Append("<td><input type='text' id='txtHsCode" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='' style='width:80px;' class='subclass'/>" +
                        "<input type='hidden' id='hfItemID" + FESNO + "' value=''/></td>");
                    sb.Append("<td><input type='text' id='txtPartNo" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='' style='width:80px;' class='subclass'/></td>");
                    sb.Append("<td><input type='text' id='txtMake" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='' style='width:60px;' class='subclass'/></td>");

                    sb.Append("<td><input type='text' id='txtQuantity" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='0' style='width:80px;' class='subclass'/></td>");
                    string Units = BindUnits("" + FESNO + "", "0", "" + ParentID + "");
                    sb.Append("<td>" + Units + "</td>"); //Units
                    sb.Append("<td><input type='text' id='txtRate" + FESNO + "' onchange='SaveChanges(" + OnChange + ")' value='0' style='width:80px;' class='subclass'/></td>");
                    sb.Append("<td><label id='lblAmount" + FESNO + "' style='width:80px;' class='subclass'>0</label></td>");
                    sb.Append("<td></td><td><span class='gridactionicons'><a href='javascript:void(0)' onclick='SaveChanges(" + OnChange_ADD + ")' " +
                              " class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return "";
            }
        }

        private string BindUnits(string SNo, string SelectedUnit, string ParentID)
        {
            try
            {
                if (Session["ItemUnits"] == null)
                    Session["ItemUnits"] = EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                        new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
                DataSet dsUnits = (DataSet)Session["ItemUnits"];

                StringBuilder sb = new StringBuilder();
                sb.Append("<select id='ddlUnits" + SNo + "' class='bcAspdropdown' style='width:70px;' onchange='SaveChanges(&#39;" + ParentID + "&#39;,&#39;" + SNo + "&#39;,this,false);'>");
                sb.Append("<option value='0'>-SELECT-</option>");
                foreach (DataRow row in dsUnits.Tables[0].Rows)
                {
                    if (SelectedUnit == row["ID"].ToString())
                    {
                        sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                            row["Description"].ToString() + "</option>");
                    }
                    else
                        sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                }
                sb.Append("</select>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPfrmaInvce_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvPfrmaInvce.HeaderRow != null)
                {
                    gvPfrmaInvce.UseAccessibleHeader = false;
                    gvPfrmaInvce.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
            }
        }

        #endregion

        #region Web Methods

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
                Page pp = (Page)HttpContext.Current.Handler;
                GridView gv = (GridView)pp.FindControl("gvPfrmaInvce");

                ArrayList all = (ArrayList)Session["PrfmaInvcDtls"];
                all.RemoveAt(ID);
                Session["PrfmaInvcDtls"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckBoxChecked(string IsChecked, string fpoNo, string Chked_FPOID)
        {
            try
            {
                string Msg = "";
                bool ObjChecked = Convert.ToBoolean(IsChecked);
                Guid Checked_FPOID = new Guid(Chked_FPOID);
                Guid FPOID = Guid.Empty;
                Dictionary<Guid, string> dict = (Dictionary<Guid, string>)Session["FposDict"] == null
                                                                                            ? dict = new Dictionary<Guid, string>()
                                                                                            : (Dictionary<Guid, string>)Session["FposDict"];

                Dictionary<Guid, Guid> PF_FPOs = (Dictionary<Guid, Guid>)Session["PF_FPOs"] == null
                                                                                            ? PF_FPOs = new Dictionary<Guid, Guid>()
                                                                                            : (Dictionary<Guid, Guid>)Session["PF_FPOs"];

                if (!PF_FPOs.ContainsKey(Checked_FPOID))
                {
                    DataTable dt = (DataTable)Session["PrfmaInvcChkd"];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (new Guid(dt.Rows[i]["ItmForeignPOId"].ToString()) == Checked_FPOID
                                && dt.Rows[i]["FPONmbr"].ToString().ToLower() == fpoNo.ToLower())
                            {
                                FPOID = new Guid(dt.Rows[i]["ItmForeignPOId"].ToString());
                                dt.Rows[i]["IsChecked"] = ObjChecked;
                            }
                        }
                        dt.AcceptChanges();
                        Session["PrfmaInvcChkd"] = dt;
                    }
                }
                if (PF_FPOs.ContainsKey(Checked_FPOID))
                    Msg = "already checked previously";
                //Session["PF_FPOs"] = PF_FPOs;

                if (ObjChecked && !PF_FPOs.ContainsKey(Checked_FPOID) && !dict.ContainsKey(Checked_FPOID))
                    dict.Add(Checked_FPOID, fpoNo);
                else if (!ObjChecked && dict.ContainsKey(Checked_FPOID))
                    dict.Remove(Checked_FPOID);
                Session["FposDict"] = dict;

                return Msg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string BindSubItems(string ParentID, string FPOID, string FESNO)
        {
            try
            {
                Guid ForeignPOID = FPOID == "" ? Guid.Empty : new Guid(FPOID);
                string Rslt = fillSubItems(ForeignPOID, ParentID, FESNO, "0");
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string Delete_SubItem(string RID, string ItemID, string ParentID, string FPOID)
        {
            try
            {
                RID = RID.Replace("\\", "");
                if (RID.Contains('.'))
                    RID = RID.Split('.')[0];
                DataTable dt = (DataTable)Session["PrfmaInvcSubItems"];
                DataRow[] dr = null;
                if (dt != null && dt.Rows.Count > 0)
                    dr = dt.Select("ItemId = '" + ItemID + "'");

                if (dr != null && dr.Length > 0)
                    dr[0].Delete();

                dt.AcceptChanges();
                Session["PrfmaInvcSubItems"] = dt;

                string Rslt = fillSubItems(new Guid(FPOID), ParentID, RID, RID);
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(string ItemID, string ParentID, string FPOID, string FESNO, string SNO, string ItmDesc, string HsCode, string PartNo, string Make, string Qty, string Units, string Rate, string IsAdd)
        {
            try
            {
                FESNO = FESNO.Replace("\\", "");
                SNO = SNO.Replace("\\", "");
                decimal _QTY = Qty == "" ? 0 : Convert.ToDecimal(Qty);
                decimal _Rate = Rate == "" ? 0 : Convert.ToDecimal(Rate);
                Guid ForeignPOID = FPOID == "" ? Guid.Empty : new Guid(FPOID);
                DataTable dt = (DataTable)Session["PrfmaInvcSubItems"];
                Guid ItmID = ItemID == "" ? Guid.Empty : new Guid(ItemID);
                DataRow[] dr = null;
                if (dt != null && dt.Rows.Count > 0)
                    dr = dt.Select("ItemId = '" + ItmID + "'");
                if (dr == null || dr.Length == 0)
                {
                    DataRow NewDr = dt.NewRow();
                    NewDr["SNo"] = FESNO + ".1";
                    NewDr["FESNO"] = FESNO;
                    NewDr["ParentItemID"] = ParentID;
                    NewDr["ItemId"] = Guid.NewGuid();
                    NewDr["FPOID"] = ForeignPOID;
                    NewDr["ItemDes"] = ItmDesc;
                    NewDr["HSCode"] = HsCode;
                    NewDr["PartNumber"] = PartNo;
                    NewDr["Make"] = Make;
                    NewDr["Quantity"] = _QTY;
                    NewDr["UnumsId"] = Units == "0" ? Guid.Empty : new Guid(Units);
                    NewDr["Rate"] = _Rate;
                    NewDr["NetWeight"] = 0;
                    NewDr["GrossWeight"] = 0;
                    NewDr["Amount"] = (_QTY * _Rate);
                    if (dt.Columns.Contains("IsNew"))
                        NewDr["IsNew"] = "True";
                    dt.Rows.Add(NewDr);
                }
                else
                {
                    if (SNO != "")
                        dr[0]["SNo"] = SNO;
                    dr[0]["FESNO"] = FESNO;
                    dr[0]["ParentItemID"] = ParentID;
                    dr[0]["ItemId"] = ItmID;
                    dr[0]["FPOID"] = ForeignPOID;
                    dr[0]["ItemDes"] = ItmDesc;
                    dr[0]["HSCode"] = HsCode;
                    dr[0]["PartNumber"] = PartNo;
                    dr[0]["Make"] = Make;
                    dr[0]["Quantity"] = _QTY;
                    dr[0]["UnumsId"] = Units == "0" ? Guid.Empty : new Guid(Units);
                    dr[0]["Rate"] = _Rate;
                    dr[0]["NetWeight"] = 0;
                    dr[0]["GrossWeight"] = 0;
                    dr[0]["Amount"] = (_QTY * _Rate);
                }
                if (Convert.ToBoolean(IsAdd))
                {
                    DataRow NewDr = dt.NewRow();
                    NewDr["SNo"] = (Convert.ToDecimal(SNO) + 0.1m);
                    NewDr["FESNO"] = FESNO;
                    NewDr["ParentItemID"] = ParentID;
                    NewDr["ItemId"] = Guid.NewGuid();
                    NewDr["FPOID"] = ForeignPOID;
                    NewDr["ItemDes"] = "";
                    NewDr["HSCode"] = "";
                    NewDr["PartNumber"] = "";
                    NewDr["Make"] = "";
                    NewDr["Quantity"] = 0;
                    NewDr["UnumsId"] = Guid.Empty;
                    NewDr["Rate"] = 0;
                    NewDr["NetWeight"] = 0;
                    NewDr["GrossWeight"] = 0;
                    NewDr["Amount"] = 0;
                    if (dt.Columns.Contains("IsNew"))
                        NewDr["IsNew"] = "True";
                    dt.Rows.Add(NewDr);
                }

                dt.AcceptChanges();
                Session["PrfmaInvcSubItems"] = dt;

                string Rslt = fillSubItems(ForeignPOID, ParentID, FESNO, SNO);
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Proforma Invoice", ex.Message.ToString());
                return ex.Message;
            }
        }

        #endregion
    }
}
