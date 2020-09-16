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
using VOMS_ERP.Admin;
using System.Text;

namespace VOMS_ERP.Invoices
{
    public partial class PackingList : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        CustomerBLL CBLL = new CustomerBLL();
        CheckListBLL CLBLL = new CheckListBLL();
        PackingListBLL PLBLL = new PackingListBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        DataSet Ds = new DataSet();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(PackingList));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");

                        if (!IsPostBack)
                        {
                            //ClearInputs();
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                        }
                        //if (Convert.ToInt64(ddlChkLst.SelectedValue) > 0)
                        //{
                        //    //helper = new GridViewHelper(this.GridView1, false);
                        //    helper = new GridViewHelper(this.gvPackingList, false);
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                txtPkngListNoDT.Attributes.Add("readonly", "readonly");
                BindDropDownList(ddlNtfy, (CBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[0]);
                BindDropDownList(ddlChkLst, (CLBLL.SelectChekcListDtls(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()))).Tables[1]);
                BindDropDownList(ddlPlcOrgGds, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofOrigin).Tables[0]);
                BindDropDownList(ddlPlcFnlDstn, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty,
                    Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofFinalDestination).Tables[0]);
                BindDropDownList(ddlPrtLdng, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading).Tables[0]);
                BindDropDownList(ddlPrtDscrg, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge).Tables[0]);
                BindDropDownList(ddlPlcDlry, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PlaceofDelivery).Tables[0]);
                // BindDropDownList(ddlChkLst, (CLBLL.SelectChekcListDtls(CommonBLL.FlagRegularDRP, 0)).Tables[0]);
                //if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                //    EditRecord(Convert.ToInt64(Request.QueryString["ID"].ToString()));                
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditPackingList(new Guid(Request.QueryString["ID"].ToString()));
                }
                else if (((Request.QueryString["ChkLstID"] != null && Request.QueryString["ChkLstID"] != "") ?
                    new Guid(Request.QueryString["ChkLstID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlChkLst.SelectedValue = Request.QueryString["ChkLstID"];
                    FillInputFields(PLBLL.GetData(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, new Guid(ddlChkLst.SelectedValue), "", "",
                        DateTime.Now, "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", true,
                        Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Veiw 
        /// </summary>
        /// <param name="gvPackingList"></param>
        /// <param name="dataTable"></param>
        private void BindGridVeiw(GridView gv, DataTable CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Rows.Count > 0)
                {
                    DataTable dt = null;
                    int FPOs = 0;
                    if (CommonDt.Columns.Contains("ForeignPOId"))
                    {
                        dt = CommonDt.DefaultView.ToTable(true, "ForeignPOId");
                        FPOs = CommonDt.DefaultView.ToTable(true, "ForeignPOId").Rows.Count;
                        HFRowCount.Value = (FPOs + CommonDt.Rows.Count).ToString();
                    }
                    else
                    {
                        dt = CommonDt.DefaultView.ToTable(true, "FrnPoId");
                        FPOs = CommonDt.DefaultView.ToTable(true, "FrnPoId").Rows.Count;
                        HFRowCount.Value = (FPOs + CommonDt.Rows.Count).ToString();
                    }
                    Session["PlItems"] = CommonDt;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields from Check List Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void FillInputFields(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 2)
                {
                    if (CommonDt.Tables[0].Rows.Count > 0)
                        BindDropDownList(ddlCstmr, CommonDt.Tables[0]);
                    ddlCstmr.SelectedIndex = 1;
                    txtPkngListNo.Text = (CommonDt.Tables.Count > 2) ?
                        "PL/" + CommonDt.Tables[3].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                    txtPkngListNoDT.Text = DateTime.Now.ToString("dd-MM-yyyy");

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

                    if (CommonDt.Tables[1].Rows.Count > 0)
                    {
                        BindListBox(lbfpos, CommonDt.Tables[1]);
                        foreach (ListItem li in lbfpos.Items)
                            //if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }

                    Session["PackingListSubItems"] = null;
                    if (CommonDt.Tables.Count > 6 && CommonDt.Tables[6].Rows.Count > 0)
                        Session["PackingListSubItems"] = CommonDt.Tables[6];

                    if (CommonDt.Tables[2].Rows.Count > 0)// && Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == false)
                        BindGridVeiw(gvPackingList, CommonDt.Tables[2]);
                    //else if (CommonDt != null && CommonDt.Tables.Count > 5 && CommonDt.Tables[5].Rows.Count > 0
                    //    && Convert.ToBoolean(CommonDt.Tables[5].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    //{
                    //    Ds = PLBLL.GetData(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, new Guid(ddlChkLst.SelectedValue), "", "",
                    //    DateTime.Now, "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", true,
                    //    Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()));
                    //    if (Ds != null && Ds.Tables.Count > 0)
                    //    {
                    //        BindListBox(lbfpos, Ds.Tables[1]);
                    //        foreach (ListItem li in lbfpos.Items)
                    //            li.Selected = true;
                    //        lbfpos.Enabled = false;

                    //        BindGridVeiw(gvPackingList, Ds.Tables[0]);
                    //    }

                        //if (Ds != null && Ds.Tables.Count > 0)
                    //    BindGridVeiw(gvPackingList, Ds.Tables[0]);
                    // }
                    else
                        BindGridVeiw(gvPackingList, null);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Proforma Invoice Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditPackingList(Guid ID)
        {
            try
            {
                hfIsEdit.Value = "True";
                DataSet EditDS = new DataSet();
                EditDS = PLBLL.SelectDetails(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()), "");
                if (EditDS != null && EditDS.Tables.Count > 1) //&& EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0)
                {
                    if (EditDS != null && EditDS.Tables[0].Rows.Count > 0)
                    {
                        BindDropDownList(ddlChkLst, EditDS.Tables[3]);
                    }

                    ddlChkLst.SelectedValue = EditDS.Tables[0].Rows[0]["CheckLIstID"].ToString();

                    BindDropDownList(ddlCstmr, (CLBLL.SelectChekcListDtls1(CommonBLL.FlagASelect, new Guid(ddlChkLst.SelectedValue),
                    Guid.Empty)).Tables[0]);

                    ddlCstmr.SelectedValue = EditDS.Tables[0].Rows[0]["CustomerID"].ToString();
                    txtPkngListNo.Text = EditDS.Tables[0].Rows[0]["PkngListNo"].ToString();
                    txtPkngListNoDT.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["PkngListNoDt"].ToString()));
                    txtOtrRfs.Text = EditDS.Tables[0].Rows[0]["OtherRef"].ToString();
                    ddlNtfy.SelectedValue = EditDS.Tables[0].Rows[0]["Notify"].ToString();
                    ddlPlcOrgGds.SelectedValue = EditDS.Tables[0].Rows[0]["CntryOrgGds"].ToString();
                    ddlPlcFnlDstn.SelectedValue = EditDS.Tables[0].Rows[0]["CntryFnlDstn"].ToString();
                    txtPCrBy.Text = EditDS.Tables[0].Rows[0]["PreCrirBy"].ToString();
                    txtPlcRcptPCr.Text = EditDS.Tables[0].Rows[0]["PlcRcptPCrirBy"].ToString();
                    txtVslFlt.Text = EditDS.Tables[0].Rows[0]["VslFltNo"].ToString();
                    ddlPrtLdng.SelectedValue = EditDS.Tables[0].Rows[0]["PrtLdng"].ToString();
                    ddlPrtDscrg.SelectedValue = EditDS.Tables[0].Rows[0]["PrtDschrg"].ToString();
                    ddlPlcDlry.SelectedValue = EditDS.Tables[0].Rows[0]["PrtDlry"].ToString();
                    txtTrmDlryPmnt.Text = EditDS.Tables[0].Rows[0]["TrmsDlryPymnts"].ToString();
                    if (EditDS.Tables[4].Rows.Count > 0 && Convert.ToBoolean(EditDS.Tables[4].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        Ds = PLBLL.GetData(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, new Guid(ddlChkLst.SelectedValue), "", "",
                        DateTime.Now, "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", true,
                        Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()));
                        BindListBox(lbfpos, Ds.Tables[1]);
                        foreach (ListItem li in lbfpos.Items)
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }
                    else
                    {
                        BindListBox(lbfpos, EditDS.Tables[1]);
                        foreach (ListItem li in lbfpos.Items)
                            li.Selected = true;
                        lbfpos.Enabled = false;
                    }

                    //if (EditDS.Tables.Count > 2)
                    //    ViewState["CrntItms"] = EditDS.Tables[3];

                    Session["PackingListSubItems"] = null;
                    if (EditDS.Tables.Count > 5 && EditDS.Tables[5].Rows.Count > 0)
                        Session["PackingListSubItems"] = EditDS.Tables[5];

                    if (EditDS.Tables.Count > 1 && EditDS.Tables[2].Rows.Count > 0)
                        BindGridVeiw(gvPackingList, EditDS.Tables[2]);
                    else if (EditDS != null && EditDS.Tables.Count > 0 && EditDS.Tables[4].Rows.Count > 0
                        && Convert.ToBoolean(EditDS.Tables[4].Rows[0]["IsVerbalFPO"].ToString()) == true)
                    {
                        Ds = PLBLL.GetData(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(ddlChkLst.SelectedValue), "", "",
                        DateTime.Now, "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", true,
                        Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()));
                        if (Ds != null && Ds.Tables.Count > 0)
                            BindGridVeiw(gvPackingList, Ds.Tables[0]);
                    }

                    if (EditDS.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((EditDS.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["PkngList"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        imgAtchmt.Visible = true;
                    }
                    else
                        imgAtchmt.Visible = false;

                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
                else
                    btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="Gv"></param>
        /// <returns></returns>
        ///Changed by Dinesh on 14-05-2019 to Get FPO Ids in sequence as per GRID.
        private Tuple<DataTable, string> ConvertToDtbl(GridView Gv)
        {
            DataTable dt = CommonBLL.PackingListItems();
            try
            {
                //dt.Rows[0].Delete();
                DataTable dtt = (DataTable)Session["PlItems"];
                ///Changed by Dinesh on 14-05-2019 to Get FPO Ids in sequence as per GRID.
                string FpoIds = "";//String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                foreach (GridViewRow row in Gv.Rows)
                {
                    DataRow dr;
                    dr = dt.NewRow();
                    Guid FPOID = new Guid(((Label)row.FindControl("lblFrnPoID")).Text);
                    Guid ITMID = new Guid(((HiddenField)row.FindControl("hfItemID")).Value);
                    DataRow[] result = dtt.Select("ForeignPOId = '" + FPOID + "' and ItemID = '" + ITMID + "'");
                    if (result != null && result.Length > 0)
                    {
                        dr["PackingBoxFrom"] = result[0]["PackingBoxFrom"];
                        dr["PackingBoxTo"] = result[0]["PackingBoxTo"];
                        dr["FPOSNo"] = Convert.ToInt32(result[0]["FPOSNo"].ToString());
                    }
                    else
                    {
                        dr["PackingBoxFrom"] = 0;
                        dr["PackingBoxTo"] = 0;
                        dr["FPOSNo"] = 0;
                    }

                    dr["SNo"] = ((HiddenField)row.FindControl("HFSNo")).Value;
                    dr["ItemID"] = new Guid(((HiddenField)row.FindControl("hfItemID")).Value);
                    dr["StockItemID"] = new Guid(((Label)row.FindControl("lblsItemDtlsID")).Text);
                    dr["ItemDesc"] = "";//Convert.ToString(((Label)row.FindControl("lblItemDesc")).Text);
                    dr["PartNo"] = Convert.ToString(((Label)row.FindControl("lblPartno")).Text);
                    dr["make"] = Convert.ToString(((Label)row.FindControl("lblMk")).Text);
                    dr["Quantity"] = Convert.ToDecimal(((TextBox)row.FindControl("txtQuantity")).Text.Trim());
                    dr["units"] = new Guid(((HiddenField)row.FindControl("HFUnitsID")).Value);
                    if (row.RowIndex != 0)
                    {
                        GridViewRow prevRow = Gv.Rows[row.RowIndex - 1];

                        if (((HiddenField)row.FindControl("HFGDNID")).Value != "")
                        {
                            string GDNID = ((HiddenField)row.FindControl("HFGDNID")).Value.Trim();
                            string PreviousGDNID = ((HiddenField)prevRow.FindControl("HFGDNID")).Value.Trim();
                            string HFFPOID = ((HiddenField)row.FindControl("hfFrnPoID1")).Value.Trim();
                            string PrevFPOID = ((HiddenField)prevRow.FindControl("hfFrnPoID1")).Value.Trim();
                            if (GDNID == PreviousGDNID && HFFPOID == PrevFPOID)
                            {
                                dr["NetWeight"] = 0;//Convert.ToDecimal(((TextBox)row.FindControl("txtNetWeight")).Text);
                                dr["GrWeight"] = 0;//Convert.ToDecimal(((TextBox)row.FindControl("txtGrWeight")).Text);
                            }
                            else
                            {
                                dr["NetWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtNetWeight")).Text);
                                dr["GrWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtGrWeight")).Text);
                            }
                        }
                        else if (((HiddenField)row.FindControl("HFGRNID")).Value != "")
                        {
                            string GRNID = ((HiddenField)row.FindControl("HFGRNID")).Value.Trim();
                            string PreviousGRNID = ((HiddenField)prevRow.FindControl("HFGRNID")).Value.Trim();
                            string HFFPOID = ((HiddenField)row.FindControl("hfFrnPoID1")).Value.Trim();
                            string PrevFPOID = ((HiddenField)prevRow.FindControl("hfFrnPoID1")).Value.Trim();
                            if (GRNID == PreviousGRNID && HFFPOID == PrevFPOID)
                            {
                                dr["NetWeight"] = 0;//Convert.ToDecimal(((TextBox)row.FindControl("txtNetWeight")).Text);
                                dr["GrWeight"] = 0;//Convert.ToDecimal(((TextBox)row.FindControl("txtGrWeight")).Text);
                            }
                            else
                            {
                                dr["NetWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtNetWeight")).Text);
                                dr["GrWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtGrWeight")).Text);
                            }
                        }
                    }
                    else
                    {
                        dr["NetWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtNetWeight")).Text);
                        dr["GrWeight"] = Convert.ToDecimal(((TextBox)row.FindControl("txtGrWeight")).Text);
                    }
                    dr["FPONo"] = Convert.ToString(((HiddenField)row.FindControl("HFFPONo")).Value);
                    dr["FrnPoId"] = FPOID;
                    if (!FpoIds.Contains(dr["FrnPoId"].ToString()))
                        FpoIds += dr["FrnPoId"].ToString() + ",";

                    dr["PkgNos"] = ((HiddenField)row.FindControl("HFPkgNos")).Value.Trim();
                    dr["HSCode"] = ((Label)row.FindControl("lblHSCode")).Text;
                    if (((HiddenField)row.FindControl("HFGDNID")).Value != "")
                        dr["GDNID"] = new Guid(((HiddenField)row.FindControl("HFGDNID")).Value);
                    if (((HiddenField)row.FindControl("HFGRNID")).Value != "")
                        dr["GRNID"] = new Guid(((HiddenField)row.FindControl("HFGRNID")).Value);
                    dt.Rows.Add(dr);
                }
                Tuple<DataTable, string> ttp = new Tuple<DataTable, string>(dt, FpoIds.Trim(','));
                return ttp;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                ddlCstmr.SelectedIndex = ddlChkLst.SelectedIndex = -1;
                lbfpos.Items.Clear();
                txtPkngListNo.Text = txtPkngListNoDT.Text = txtOtrRfs.Text = "";
                ddlPlcOrgGds.SelectedIndex = ddlPlcFnlDstn.SelectedIndex =
                    ddlPrtLdng.SelectedIndex = ddlPrtDscrg.SelectedIndex = ddlPlcDlry.SelectedIndex = ddlNtfy.SelectedIndex = -1;
                txtPCrBy.Text = txtPlcRcptPCr.Text = txtVslFlt.Text = "";

                txtTrmDlryPmnt.Text = txtComments.Text = "";

                Session.Remove("PrfmaInvcDtls");
                Session.Remove("PkngList");
                BindGridVeiw(gvPackingList, null);
                divListBox.InnerHtml = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                        if (Session["PkngList"] != null)
                        {
                            alist = (ArrayList)Session["PkngList"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["PkngList"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["PkngList"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                if (Session["PkngList"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["PkngList"];
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
                string FpoIds = "";// String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string Atchmnts = Session["PkngList"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["PkngList"]).Cast<string>().ToArray());
                ///Changed by Dinesh on 14-05-2019 to Get FPO Ids in sequence as per GRID.
                Tuple<DataTable, string> tpl = ConvertToDtbl(gvPackingList);
                DataTable PLItms = tpl.Item1;
                FpoIds = tpl.Item2.Trim(',');

                if (PLItms != null)
                {
                    //var CheckData = PLItms.AsEnumerable().Where(P => (string.IsNullOrEmpty(P.Field<decimal?>("Quantity").Value.ToString())) || P.Field<decimal>("Quantity") == 0
                    //                                      || P.Field<decimal>("NetWeight") > P.Field<decimal>("GrWeight"))).ToList();
                    if (PLItms.AsEnumerable().Where(P => (string.IsNullOrEmpty(P.Field<decimal?>("Quantity").Value.ToString()))).Count() > 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Quantity cannot be Empty.');", true);
                        return;
                    }
                    if (PLItms.AsEnumerable().Where(P => P.Field<decimal>("Quantity") == 0).Count() > 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Quantity cannot be Zero.');", true);
                        return;
                    }
                    if (PLItms.AsEnumerable().Where(P => P.Field<decimal>("NetWeight") > P.Field<decimal>("GrWeight")).Count() > 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Net Weight can not be greator than Gross Weight');", true);
                        return;
                    }
                }
                DataRow[] result = PLItms.Select("PackingBoxFrom = 0 or PackingBoxTo = 0");

                DataTable dtSub = (DataTable)Session["PackingListSubItems"];
                DataRow[] drSub = null;
                if (dtSub != null && dtSub.Rows.Count > 0)
                    drSub = dtSub.Select("Netweight = '0' or GrossWeight = '0'");
                else if (dtSub == null)
                    dtSub = CommonBLL.PrfmaInvoice_SubItems();

                if (PLItms == null || PLItms.Rows.Count == 0 && result == null || result.Length > 0 || (drSub != null && drSub.Length > 0))
                {
                    if (result != null && result.Length > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill From and To Values.');", true);
                    else if (drSub.Length > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill Net and Gross Weights.');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to Insert.');", true);
                    //BindGridVeiw(gvPackingList, PLItms);
                }
                else
                {
                    if (dtSub.Columns.Contains("IsNew"))
                        dtSub.Columns.Remove("IsNew");
                    if (dtSub.Columns.Contains("UnitsName"))
                        dtSub.Columns.Remove("UnitsName");
                    dtSub.AcceptChanges();

                    if (btnSave.Text == "Save")
                    {
                        string path = Request.Path;
                        path = Path.GetFileName(path);

                        DataSet CurntID = PLBLL.SelectDetails(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()), path);

                        string PkingListRefNo = ""; string[] PkngLstNo = new string[] { }; int Count = 1;
                        if (CurntID.Tables[0].Rows.Count > 0)
                        { 
                            PkngLstNo = CurntID.Tables[0].Rows[0]["Description"].ToString().Split('/');
                            Count = int.Parse(PkngLstNo[2]) + 1;
                        }
                        if (CurntID.Tables.Count > 1)
                        {
                            PkingListRefNo = ((CurntID.Tables[1].Rows[0]["Name"].ToString()) + "/PL/" + Count + "/" + CommonBLL.FinacialYearShort);
                        }
                        else
                        {
                            PkingListRefNo = "PL/1" + CommonBLL.FinacialYearShort;
                        }
                        res = PLBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCstmr.SelectedValue),
                            new Guid(ddlChkLst.SelectedValue), FpoIds, PkingListRefNo, CommonBLL.DateInsert(txtPkngListNoDT.Text),
                            txtOtrRfs.Text.Trim(), new Guid(ddlNtfy.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue),
                            new Guid(ddlPlcFnlDstn.SelectedValue), txtPCrBy.Text.Trim(), txtPlcRcptPCr.Text.Trim(), txtVslFlt.Text.Trim(),
                            new Guid(ddlPrtLdng.SelectedValue), new Guid(ddlPrtDscrg.SelectedValue), new Guid(ddlPlcDlry.SelectedValue),
                            txtTrmDlryPmnt.Text.Trim(), Atchmnts, "", new Guid(Session["UserID"].ToString()), PLItms, dtSub, new Guid(Session["CompanyID"].ToString()));

                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Packing List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Packing List",
                                "Data inserted successfully.");
                            ClearInputs();
                            Response.Redirect("PackingListStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Packing List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List",
                                "Error while Saving.");
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        res = PLBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                            new Guid(ddlCstmr.SelectedValue), new Guid(ddlChkLst.SelectedValue), FpoIds,
                            txtPkngListNo.Text.Trim(), CommonBLL.DateInsert1(txtPkngListNoDT.Text), txtOtrRfs.Text.Trim(),
                            new Guid(ddlNtfy.SelectedValue), new Guid(ddlPlcOrgGds.SelectedValue), new Guid(ddlPlcFnlDstn.SelectedValue),
                            txtPCrBy.Text.Trim(), txtPlcRcptPCr.Text.Trim(), txtVslFlt.Text.Trim(), new Guid(ddlPrtLdng.SelectedValue),
                            new Guid(ddlPrtDscrg.SelectedValue), new Guid(ddlPlcDlry.SelectedValue),
                            txtTrmDlryPmnt.Text.Trim(), Atchmnts, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()), PLItms, dtSub, new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Packing List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Packing List",
                                "Updated successfully.");
                            ClearInputs();
                            Response.Redirect("PackingListStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Packing List", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List",
                                "Error while Updating.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                gvPackingList.DataSource = null;
                gvPackingList.DataBind();
                Response.Redirect("PackingList.aspx", false);
                //DataTable dt = new DataTable();
                //dt = ConvertToDtbl(gvPackingList);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// Check List Drop Down List Selected Index Chnged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlChkLst_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlChkLst.SelectedIndex != 0)
                {
                    DataSet ds = PLBLL.GetData(CommonBLL.FlagWCommonMstr, Guid.Empty, Guid.Empty, new Guid(ddlChkLst.SelectedValue), "", "", DateTime.Now,
                                 "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", Guid.Empty, Guid.Empty, Guid.Empty, "", "", true, Guid.Empty,
                                 DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()));
                    FillInputFields(ds);
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                //    txtPkngListNo.Text = (CommonDT.Tables.Count > 1) ?
                //        "VIPL/PINV/" + CommonDT.Tables[1].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                //    txtPkngListNoDT.Text = DateTime.Now.ToString("dd-MM-yyyy");
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                //string FPOIDs = string.Join(",", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                //DataSet CommonDT = CLBLL.SelectChekcListDtls(CommonBLL.FlagESelect, Convert.ToInt64(ddlChkLst.SelectedValue),
                //    ddlCstmr.SelectedValue, FPOIDs);
                //if (CommonDT != null && CommonDT.Tables.Count > 0)
                //{
                //    BindGridVeiw(gvPackingList, CommonDT.Tables[0]);
                //    txtPkngListNo.Text = (CommonDT.Tables.Count > 1) ?
                //        "VIPL/PINV/" + CommonDT.Tables[1].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort : "";
                //    txtPkngListNoDT.Text = DateTime.Now.ToString("dd-MM-yyyy");
                //}
                ////FillInputs(GDNBL.SelectGdnDtls(CommonBLL.FlagESelect, 0, Int64.Parse(ddlDspchInst.SelectedValue)));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPackingList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex != 0)
                    {
                        GridViewRow prevRow = this.gvPackingList.Rows[e.Row.RowIndex - 1];

                        if (((HiddenField)e.Row.FindControl("HFGDNID")).Value != "")
                        {
                            string GDNID = ((HiddenField)e.Row.FindControl("HFGDNID")).Value.Trim();
                            string PreviousGDNID = ((HiddenField)prevRow.FindControl("HFGDNID")).Value.Trim();
                            string FPOID = ((HiddenField)e.Row.FindControl("hfFrnPoID1")).Value.Trim();
                            string PrevFPOID = ((HiddenField)prevRow.FindControl("hfFrnPoID1")).Value.Trim();
                            if (GDNID == PreviousGDNID)
                            {
                                if (FPOID == PrevFPOID)
                                {
                                    ((TextBox)e.Row.FindControl("txtNetWeight")).Visible = false;
                                    ((TextBox)e.Row.FindControl("txtGrWeight")).Visible = false;
                                }
                            }
                        }
                        if (((HiddenField)e.Row.FindControl("HFGRNID")).Value != "")
                        {
                            string GRNID = ((HiddenField)e.Row.FindControl("HFGRNID")).Value.Trim();
                            string PreviousGRNID = ((HiddenField)prevRow.FindControl("HFGRNID")).Value.Trim();
                            string FPOID = ((HiddenField)e.Row.FindControl("hfFrnPoID1")).Value.Trim();
                            string PrevFPOID = ((HiddenField)prevRow.FindControl("hfFrnPoID1")).Value.Trim();
                            if (GRNID == PreviousGRNID)
                            {
                                if (FPOID == PrevFPOID)
                                {
                                    ((TextBox)e.Row.FindControl("txtNetWeight")).Visible = false;
                                    ((TextBox)e.Row.FindControl("txtGrWeight")).Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvPackingList_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvPackingList.HeaderRow == null) return;
                gvPackingList.UseAccessibleHeader = false;
                gvPackingList.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["PkngList"];
                if (all.Count >= ID)
                    all.RemoveAt(ID);
                Session["PkngList"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string FromFPOGroup(string ObjFrmVal, string ObjToVal, string ObjPrevToVal, string ObjFPOID, string PkgsVal)
        {
            try
            {
                bool IsFalse = true;
                DataTable dt = (DataTable)Session["PlItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    Guid FPOID = new Guid(ObjFPOID);
                    int ToVal = ObjToVal == "" ? 0 : Convert.ToInt32(ObjToVal),
                        FrmVal = ObjFrmVal == "" ? 0 : Convert.ToInt32(ObjFrmVal),
                        PrevToVal = ObjPrevToVal == "" ? 0 : Convert.ToInt32(ObjPrevToVal);
                    if (PrevToVal == 0 || (ToVal < FrmVal && ToVal > 0) || (PrevToVal > FrmVal && PrevToVal > 0))
                    {
                        IsFalse = false;
                        FrmVal = PrevToVal;
                        dt = ClearFrmTo(dt, FrmVal);
                    }
                    else
                    {
                        DataRow[] dr = dt.Select("ForeignPOId = '" + FPOID + "'");
                        if (dr != null && dr.Length > 0)
                        {
                            for (int i = 0; i < dr.Length; i++)
                                dr[i]["PackingBoxFrom"] = FrmVal;
                            dt.AcceptChanges();
                        }
                    }
                    Session["PlItems"] = dt;
                }
                return IsFalse.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string ToFPOGroup(string ObjFrmVal, string ObjToVal, string ObjNextFrmVal, string _FPOID, string PkgsVal)
        {
            try
            {
                bool IsFalse = true;
                DataTable dt = (DataTable)Session["PlItems"];
                if (dt != null && dt.Rows.Count > 0)
                {
                    Guid FPOID = new Guid(_FPOID);
                    int ToVal = ObjToVal == "" ? 0 : Convert.ToInt32(ObjToVal),
                        FrmVal = ObjFrmVal == "" ? 0 : Convert.ToInt32(ObjFrmVal),
                        NextFrmVal = ObjNextFrmVal == "" ? 0 : Convert.ToInt32(ObjNextFrmVal);
                    if (FrmVal == 0 || (FrmVal > ToVal && FrmVal > 0) || (ToVal > NextFrmVal && NextFrmVal > 0))
                    {
                        ToVal = FrmVal;
                        IsFalse = false;
                        dt = ClearFrmTo(dt, ToVal);
                    }
                    else
                    {
                        DataRow[] dr = dt.Select("ForeignPOId = '" + FPOID + "'");
                        if (dr != null && dr.Length > 0)
                        {
                            for (int i = 0; i < dr.Length; i++)
                                dr[i]["PackingBoxTo"] = ToVal;
                            dt.AcceptChanges();
                        }
                    }
                    Session["PlItems"] = dt;
                }
                return IsFalse.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private DataTable ClearFrmTo(DataTable dt, int Val)
        {
            try
            {
                DataRow[] dr = dt.Select("PackingBoxto > " + Val);
                if (dr != null && dr.Length > 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                        dr[i]["PackingBoxto"] = 0;
                    dt.AcceptChanges();
                }

                DataRow[] drr = dt.Select("PackingBoxFrom > " + Val);
                if (drr != null && drr.Length > 0)
                {
                    for (int i = 0; i < drr.Length; i++)
                        drr[i]["PackingBoxFrom"] = 0;
                    dt.AcceptChanges();
                }
                return dt;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                return null;
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
                DataTable dt = (DataTable)Session["PackingListSubItems"];
                DataRow[] dr = null;
                if (dt != null && dt.Rows.Count > 0)
                    dr = dt.Select("ItemId = '" + ItemID + "'");

                if (dr != null && dr.Length > 0)
                    dr[0].Delete();

                dt.AcceptChanges();
                Session["PackingListSubItems"] = dt;

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
        public string SaveChanges(string ItemID, string ParentID, string FPOID, string FESNO, string SNO, string ItmDesc, string HsCode, string PartNo, string Make, string Qty, string Netweight, string GrossWeight, string IsAdd)
        {
            try
            {
                FESNO = FESNO.Replace("\\", "");
                SNO = SNO.Replace("\\", "");
                decimal _QTY = Qty == "" ? 0 : Convert.ToDecimal(Qty);
                decimal _NW = Netweight == "" ? 0 : Convert.ToDecimal(Netweight);
                decimal _GW = GrossWeight == "" ? 0 : Convert.ToDecimal(GrossWeight);
                Guid ForeignPOID = FPOID == "" ? Guid.Empty : new Guid(FPOID);
                DataTable dt = (DataTable)Session["PackingListSubItems"];
                Guid ItmID = ItemID == "" ? Guid.Empty : new Guid(ItemID);
                DataRow[] dr = null;
                if (dt != null && dt.Rows.Count > 0)
                    dr = dt.Select("ItemId = '" + ItmID + "'");
                if (dr != null && dr.Length > 0)
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
                    if (Convert.ToDecimal(dr[0]["Quantity"].ToString()) != _QTY)
                    {
                        dr[0]["Quantity"] = _QTY;
                        dr[0]["Amount"] = (Convert.ToDecimal(dr[0]["Rate"].ToString()) * _QTY);
                    }
                    dr[0]["Netweight"] = _NW;
                    dr[0]["GrossWeight"] = _GW;

                    dt.AcceptChanges();
                    Session["PackingListSubItems"] = dt;
                }
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

        private string fillSubItems(Guid FPOID, string ParentID, string FESNO, string _SNO)
        {
            try
            {
                DataTable dt = (DataTable)Session["PackingListSubItems"];
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
                            dr[i]["ItemDes"].ToString() + "' style='width:300px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtHsCode" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["HSCode"].ToString() + "' style='width:80px;' class='subclass'/>" +
                            "<input type='hidden' id='hfItemID" + SNo + "' value='" + ItemID + "'/></td>");

                        sb.Append("<td><input type='text' id='txtPartNo" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["PartNumber"].ToString() + "' style='width:75px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtMake" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["Make"].ToString() + "' style='width:75px;' class='subclass'/></td>");

                        sb.Append("<td><input type='text' id='txtQuantity" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["Quantity"].ToString() + "' style='width:75px;' class='subclass'"
                        + "' onblur='extractNumber(this,0,false);' onkeyup='extractNumber(this,0,false);' "
                        + "onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");

                        sb.Append("<td>" + dr[i]["UnitsName"].ToString() + "</td>");

                        sb.Append("<td><input type='text' id='txtNetWeight" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["NetWeight"].ToString() + "' style='width:80px;' class='subclass'"
                        + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' "
                        + "onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");

                        sb.Append("<td><input type='text' id='txtGrossWeight" + SNo + "' onchange='SaveChanges(" + OnChange + ")' value='" +
                            dr[i]["GrossWeight"].ToString() + "' style='width:80px;' class='subclass'"
                        + "' onblur='extractNumber(this,2,false);' onkeyup='extractNumber(this,2,false);' "
                        + "onkeypress='return blockNonNumbers(this, event, true, false);'/></td>");

                        sb.Append("</tr>");
                    }
                    dt.AcceptChanges();
                    Session["PackingListSubItems"] = dt;
                }
                else
                    sb.Append("<tr><td align='center' colspan='999'> <font color='red'> No Rows to display</font></td></tr>");

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

        #endregion
    }
}
