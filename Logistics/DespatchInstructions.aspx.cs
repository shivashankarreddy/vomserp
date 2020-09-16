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

namespace VOMS_ERP.Logistics
{
    public partial class DespatchInstructions : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        DspchInstnsBLL DPIBL = new DspchInstnsBLL();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(DespatchInstructions));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
        /// Bind Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlCstmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if ((Request.QueryString["CstmrID"] != null && Request.QueryString["CstmrID"] != "") &&
                   (Request.QueryString["SuplrID"] != null && Request.QueryString["SuplrID"] != "") &&
                   (Request.QueryString["FpoId"] != null && Request.QueryString["FpoId"] != "") &&
                   (Request.QueryString["LpoId"] != null && Request.QueryString["LpoId"] != ""))
                {
                    ddlCstmr.SelectedValue = Request.QueryString["CstmrID"].ToString();
                    BindDropDownList(ddlSuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagQSelect, Guid.Empty,
                    new Guid(ddlCstmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlSuplr.SelectedValue = Request.QueryString["SuplrID"].ToString();
                    string[] states = Request.QueryString["FpoId"].Split(',');
                    string[] Lpostates = Request.QueryString["LpoId"].Split(',');
                    ViewState["FpoId"] = states;
                    ViewState["LpoId"] = Lpostates;
                    FillInputs();
                }

                else if ((Request.QueryString["CstmrID"] != null && Request.QueryString["CstmrID"] != "") &&
                  (Request.QueryString["SuplrID"] != null && Request.QueryString["SuplrID"] != ""))
                {
                    ddlCstmr.SelectedValue = Request.QueryString["CstmrID"].ToString();
                    BindDropDownList(ddlSuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagQSelect, Guid.Empty,
                    new Guid(ddlCstmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlSuplr.SelectedValue = Request.QueryString["SuplrID"].ToString();
                    FillInputs();
                }
                else if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(DPIBL.SelectDspchInstns(CommonBLL.FlagFSelect, new Guid(Request.QueryString["ID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields Using Data Set for Edit
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                ddlCstmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();

                BindDropDownList(ddlSuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagESelect, Guid.Empty,
                   new Guid(ddlCstmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                ddlSuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();
                txtSbjt.Text = CommonDt.Tables[0].Rows[0]["Subject"].ToString(); //"Despatch Instructions"; 
                txtCnctPrsn.Text = CommonDt.Tables[0].Rows[0]["ContactPersonName"].ToString();
                txtCnctNo.Text = CommonDt.Tables[0].Rows[0]["ContactNumber"].ToString();
                txtAltCnctPrsn.Text = CommonDt.Tables[0].Rows[0]["AlternativePersonName"].ToString();
                txtAltCnctNo.Text = CommonDt.Tables[0].Rows[0]["AlternativeContactNumber"].ToString();
                txtSpngAdr.Text = CommonDt.Tables[0].Rows[0]["ShippingAddress"].ToString();
                txtRefno.Text = CommonDt.Tables[0].Rows[0]["RefNo"].ToString();//RefNo
                txtAREOFS.Text = CommonDt.Tables[0].Rows[0]["ARE1FrmSets"].ToString();
                BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                    CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                foreach (ListItem li in lbfpos.Items)
                    li.Selected = true;

                BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "",
                    CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                foreach (ListItem li in lblpos.Items)
                    li.Selected = true;
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                GetLPOItems(LpoIds);
                BindListBox(lbCtoRn, DPIBL.SelectDspchInstns(CommonBLL.FlagGSelect, Guid.Empty, CommonDt.Tables[0].Rows[0]["LPOs"].ToString(),
                    new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), new Guid(Session["CompanyId"].ToString())));

                if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                {
                    ArrayList attms = new ArrayList();
                    attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                    Session["DsphInst"] = attms;
                    divListBox.InnerHtml = AttachedFiles();
                }
                //txtSbjt.Enabled = false;
                ddlSuplr.Enabled = ddlCstmr.Enabled = lbfpos.Enabled = false;
                lblpos.Enabled = txtAREOFS.Enabled = lbCtoRn.Enabled = false;
                DivComments.Visible = true;
                btnSave.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields
        /// </summary>
        protected void FillInputs()
        {
            try
            {
                lbfpos.Items.Clear(); lblpos.Items.Clear(); lbCtoRn.Items.Clear();

                DataSet RefNo = DPIBL.SelectDspchInstns(CommonBLL.FlagBSelect, new Guid(ddlSuplr.SelectedValue), new Guid(Session["CompanyID"].ToString()));

                if (RefNo.Tables.Count > 0 && RefNo.Tables[0].Rows.Count > 0)
                    txtRefno.Text = RefNo.Tables[2].Rows[0]["Name"].ToString() + "/" + RefNo.Tables[0].Rows[0]["BussName"].ToString() + "/" + RefNo.Tables[0].Rows[0]["ID"].ToString()
                        + "/" + CommonBLL.FinacialYearShort;
                if (RefNo.Tables.Count > 1 && RefNo.Tables[1].Rows.Count > 0)
                {
                    //Commented By Dinesh For Manual Entry on 27/01/2016
                    //txtCnctPrsn.Text = RefNo.Tables[1].Rows[0]["ContactPrsn"].ToString();
                    //txtAltCnctPrsn.Text = RefNo.Tables[1].Rows[0]["AltContactPrsn"].ToString();
                    //txtCnctNo.Text = RefNo.Tables[1].Rows[0]["Telephone"].ToString();
                    //txtAltCnctNo.Text = RefNo.Tables[1].Rows[0]["AltContact"].ToString();
                    //txtSpngAdr.Text = RefNo.Tables[1].Rows[0]["Address1"].ToString() + ", " + System.Environment.NewLine +
                    //    RefNo.Tables[1].Rows[0]["Address2"].ToString() + ", " + RefNo.Tables[1].Rows[0]["City"].ToString();
                }

                BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagCommonMstr, Guid.Empty,
                    new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                if (ViewState["FpoId"] != null)
                {
                    foreach (string s in (string[])ViewState["FpoId"])
                    {
                        foreach (ListItem item in lbfpos.Items)
                        {
                            if (item.Value == s) item.Selected = true;
                        }
                    }
                }
                FpoSelectedIndex();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                ddlCstmr.SelectedIndex = ddlSuplr.SelectedIndex = -1;
                txtAREOFS.Text = txtSbjt.Text = txtSpngAdr.Text = txtCnctPrsn.Text = "";
                txtCnctNo.Text = txtAltCnctNo.Text = txtAltCnctPrsn.Text = txtRefno.Text = txtComments.Text = "";
                ddlCstmr.Enabled = ddlSuplr.Enabled = true;
                lbfpos.Items.Clear();
                lblpos.Items.Clear();
                lbCtoRn.Items.Clear();
                DivComments.Visible = false;
                btnSave.Text = "Save";
                divListBox.InnerHtml = "";
                Session.Remove("DsphInst");
                gvLPOItems.Visible = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
                        if (Session["DsphInst"] != null)
                        {
                            alist = (ArrayList)Session["DsphInst"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["DsphInst"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["DsphInst"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('File Size is more than 25MB, Resize and Try Again');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
                if (Session["DsphInst"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["DsphInst"];
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

        private void GetLPOItems(string LpoIds)
        {
            try
            {
                DataSet ds = DPIBL.DeleteDspchInstns(CommonBLL.FlagZSelect, Guid.Empty, "", "", Guid.Empty, Guid.Empty, "", LpoIds, 0, "", "", "", "", "", "", "", "", Guid.Empty, new Guid(Session["CompanyId"].ToString()));
                gvLPOItems.DataSource = ds.Tables[0];
                gvLPOItems.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        private void FpoSelectedIndex()
        {
            lblpos.Items.Clear(); lbCtoRn.Items.Clear();

            string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagWCommonMstr, Guid.Empty,
                new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), FpoIds, "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
            if (ViewState["LpoId"] != null)
            {
                foreach (string s in (string[])ViewState["LpoId"])
                {
                    foreach (ListItem item in lblpos.Items)
                    {
                        if (item.Value == s) item.Selected = true;
                    }
                }

            }
            LpoSelectedIndex();
        }

        private void LpoSelectedIndex()
        {
            lbCtoRn.Items.Clear();
            gvLPOItems.Visible = true;
            string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
            BindListBox(lbCtoRn, DPIBL.SelectDspchInstns(CommonBLL.FlagGSelect, Guid.Empty, LpoIds, new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), new Guid(Session["CompanyId"].ToString())));
            //txtSbjt.Text = "Despatch Instructions";
            //txtSbjt.Enabled = false;
            GetLPOItems(LpoIds);
            lbCtoRn.Enabled = false;
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
                ArrayList all = (ArrayList)Session["DsphInst"];
                all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Customer Drop Down List Selected Index Changed Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCstmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCstmr.SelectedValue != "0")
                {
                    string cstmr = ddlCstmr.SelectedValue;
                    ClearInputs();
                    ddlCstmr.SelectedValue = cstmr;
                    BindDropDownList(ddlSuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagQSelect, Guid.Empty,
                        new Guid(ddlCstmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlSuplr.SelectedValue = "0";
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSuplr.SelectedValue != "0")
                {
                    FillInputs();
                }
                else
                {
                    string cstmr = ddlCstmr.SelectedValue;
                    ClearInputs();
                    ddlCstmr.SelectedValue = cstmr;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box FPO's Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FpoSelectedIndex();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        /// <summary>
        /// List Box LPO's Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lblpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LpoSelectedIndex();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
                Filename = FileName();
                string Atchmnts = Session["DsphInst"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["DsphInst"]).Cast<string>().ToArray());
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string CT1Rfn = String.Join(", ", lbCtoRn.Items.Cast<ListItem>().Select(i => i.Value).ToArray());
                txtAREOFS.Text = (txtAREOFS.Text.Trim() == "" ? "0" : txtAREOFS.Text);

                if (btnSave.Text == "Save")
                {
                    string path = Request.Path;
                    path = Path.GetFileName(path);
                    DataSet RefNo = DPIBL.SelectDspchInstns(CommonBLL.FlagBSelect, new Guid(ddlSuplr.SelectedValue), new Guid(Session["CompanyID"].ToString()));

                    DataSet LPOsNoCnt = DPIBL.SelectDspchInstnsLpo(CommonBLL.FlagISelect, lblpos.SelectedValue, new Guid(Session["CompanyID"].ToString()),path);

                    if (RefNo.Tables.Count > 0 && RefNo.Tables[0].Rows.Count > 0)
                        txtRefno.Text = RefNo.Tables[2].Rows[0]["Name"].ToString() + "/" + Session["AliasName"] + "/" //+ LPOsNoCnt.Tables[0].Rows[0]["LocalPurchaseOrderId"].ToString() + "/"
                            + RefNo.Tables[0].Rows[0]["ID"].ToString() + "/" + CommonBLL.FinacialYearShort;

                    res = DPIBL.InsertUpdateDeleteDspchInstns(CommonBLL.FlagNewInsert, Guid.Empty, txtRefno.Text, CT1Rfn,
                        new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue), FpoIds, LpoIds,
                        Convert.ToInt32(txtAREOFS.Text), txtSbjt.Text, txtSpngAdr.Text, txtCnctPrsn.Text, txtCnctNo.Text,
                        txtAltCnctPrsn.Text, txtAltCnctNo.Text, txtComments.Text, Atchmnts, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyId"].ToString()));
                }
                else
                    res = DPIBL.InsertUpdateDeleteDspchInstns(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtRefno.Text, CT1Rfn, new Guid(ddlCstmr.SelectedValue), new Guid(ddlSuplr.SelectedValue),
                        FpoIds, LpoIds, Convert.ToInt32(txtAREOFS.Text), txtSbjt.Text, txtSpngAdr.Text, txtCnctPrsn.Text,
                        txtCnctNo.Text, txtAltCnctPrsn.Text, txtAltCnctNo.Text, txtComments.Text, Atchmnts,
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyId"].ToString()));
                if (res == 0)
                {
                    ALS.AuditLog(res, btnSave.Text, "", "Despatch Instructions", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Despatch Instructions",
                        "Data inserted successfully.");
                    ClearInputs();
                    Response.Redirect("DspchInstnsStatus.Aspx", false);
                }
                else
                {
                    if (btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Despatch Instructions", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions",
                            "Error while Inserting.");
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Despatch Instructions", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
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
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Despatch Instructions", ex.Message.ToString());
            }
        }

        #endregion
    }
}
