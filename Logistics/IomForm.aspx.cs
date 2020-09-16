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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class IomForm : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        IOMTemplateBLL IOMTBLL = new IOMTemplateBLL();
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
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(IomForm));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        txtRcd.Attributes.Add("readonly", "readonly");
                        txtpIdt.Attributes.Add("readonly", "readonly");
                        if (!IsPostBack)
                        {
                            Session["IomTmp"] = null;
                            txtRcd.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            txtpIdt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                            divOpen_attachments.InnerHtml = Att_open();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        #endregion Methods

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
        /// Get Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                Session.Remove("IomTmp");
                BindDropDownList(ddlRefno, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlRefno, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(IOMTBLL.GetData(CommonBLL.FlagJSelect, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
                else if (((Request.QueryString["PIReqID"] != null && Request.QueryString["PIReqID"] != "") ?
                    new Guid(Request.QueryString["PIReqID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlRefno.SelectedValue = Request.QueryString["PIReqID"].ToString().ToLowerInvariant();
                    FillInputFields(IOMTBLL.GetData(CommonBLL.FlagModify, Guid.Empty, new Guid(Request.QueryString["PIReqID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                }
                else
                    ddl.DataSource = null;
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Display Data into Input Fields Using Data Set
        /// </summary>
        /// <param name="CommonDt"></param>
        private void FillInputFields(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    txtRefno.Text = "IOM-PUR/" + Session["AliasName"].ToString() + "/" + CommonDt.Tables[0].Rows[0]["RefNoID"].ToString()
                        + "/" + CommonBLL.FinacialYearShort;
                    txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();
                    txtFAdr.Text = Session["UserMail"].ToString();
                    ddlStatus.SelectedValue = "2";
                    ddlStatus.Enabled = false;

                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagGSelect, Guid.Empty,
                        new Guid(CommonDt.Tables[0].Rows[0]["CustomerID"].ToString()),
                        new Guid(CommonDt.Tables[0].Rows[0]["SupplierID"].ToString()),
                        "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().ToUpper().Contains(li.Value.ToUpper()))
                            li.Selected = true;
                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagINewInsert, Guid.Empty,
                    new Guid(CommonDt.Tables[0].Rows[0]["CustomerID"].ToString()),
                    new Guid(CommonDt.Tables[0].Rows[0]["SupplierID"].ToString()), CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                    {
                        if (CommonDt.Tables[0].Rows[0]["LPOs"].ToString().ToUpperInvariant().Contains(li.Value))
                            li.Selected = true;
                    }
                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["IomTmp"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                        divOpen_attachments.InnerHtml = Att_open();
                    }

                    txtBdy.Text = "Dear Sir/Madam," + System.Environment.NewLine + System.Environment.NewLine +
                        "       With reference to the listed IOM details, please arrange CT-1 Form at the" +
                        " earliest. Please conform the receipt by approving and update the status subsequently." + System.Environment.NewLine +
                        " The Consignment need _ _ set(s) of ARE-1 Forms for dispatch(es)." + System.Environment.NewLine +
                        " For any clarifications, kindly revert." + System.Environment.NewLine + System.Environment.NewLine +
                        " Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                        " Yours Faithfully," + System.Environment.NewLine +
                        " " + Session["UserName"].ToString() + ".";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit/Update Details
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                txtRefno.Text = CommonDt.Tables[0].Rows[0]["IOMRefNo"].ToString();
                txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();
                ddlRefno.SelectedValue = CommonDt.Tables[0].Rows[0]["ProformaReqID"].ToString();
                txtPsd.Attributes.Add("value", "TESTPSSWORD");
                txtTAdr.Text = CommonDt.Tables[0].Rows[0]["ToEmailID"].ToString();
                txtFAdr.Text = CommonDt.Tables[0].Rows[0]["FromEmailID"].ToString();
                txtSbjt.Text = CommonDt.Tables[0].Rows[0]["Subject"].ToString();
                txtpmIn.Text = CommonDt.Tables[0].Rows[0]["ProformaInvoice"].ToString();
                txtpIdt.Text = CommonDt.Tables[0].Rows[0]["ProformaDate"].ToString();
                txtRcd.Text = CommonDt.Tables[0].Rows[0]["IOMDate"].ToString();
                txtBdy.Text = CommonDt.Tables[0].Rows[0]["Body"].ToString();

                txtFAdr.Enabled = false;
                txtPsd.Enabled = false;
                txtRcd.Enabled = false;
                txtpmIn.Enabled = false;
                txtpIdt.Enabled = false;
                ddlRefno.Enabled = false;
                txtSbjt.Enabled = false;
                txtBdy.Enabled = false;
                txtTAdr.Enabled = false;
                ddlStatus.Items.RemoveAt(1);
                ddlStatus.Items.FindByText(CommonDt.Tables[0].Rows[0]["Status"].ToString()).Selected = true;
                BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty, CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "",
                    "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                foreach (ListItem li in lbfpos.Items)
                    if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Trim().ToLower().Contains(li.Value.Trim().ToLower()))
                        li.Selected = true;

                BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, "", CommonDt.Tables[0].Rows[0]["LPOs"].ToString(),
                    "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                foreach (ListItem li in lblpos.Items)
                    li.Selected = true;

                if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                {
                    Session.Remove("IomTmp");
                    ArrayList attms = new ArrayList();
                    attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                    Session["IomTmp"] = attms;
                    divListBox.InnerHtml = AttachedFiles();
                    divOpen_attachments.InnerHtml = Att_open();
                }
                DivComments.Visible = true;
                btnSend.Text = "Update";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtRcd.Text = txtRefno.Text = txtTAdr.Text = txtFAdr.Text = txtPsd.Text = txtpIdt.Text = txtpmIn.Text = "";
                txtSbjt.Text = txtCstmr.Text = txtSuplr.Text = txtComments.Text = txtBdy.Text = "";
                txtPsd.Attributes.Add("value", "");
                ddlRefno.SelectedIndex = ddlStatus.SelectedIndex = -1;
                lbfpos.Items.Clear();
                lblpos.Items.Clear();
                Session.Remove("IomTmp");
                divListBox.InnerHtml = AttachedFiles();
                divOpen_attachments.InnerHtml = "";
                btnSend.Text = "Send";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                        if (Session["IomTmp"] != null)
                        {
                            alist = (ArrayList)Session["IomTmp"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["IomTmp"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["IomTmp"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                if (Session["IomTmp"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["IomTmp"];
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                        {
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        }
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

        public string Att_open()
        {
            StringBuilder sbb = new StringBuilder();
            try
            {
                if (Session["IomTmp"] != null)
                {
                    string url = "../uploads/";
                    ArrayList al = new ArrayList();
                    al = (ArrayList)Session["IomTmp"];
                    for (int i = 0; i < al.Count; i++)
                    {
                        string finUrl = url + "" + al[i].ToString();
                        string fileName = al[i].ToString();
                        int fileExtPos = fileName.LastIndexOf(".");
                        if (fileExtPos >= 0)
                            fileName = fileName.Substring(0, fileExtPos);
                        sbb.Append(" " + (i + 1) + ") <a href='" + finUrl + "' id='openfile" + i + "' onclick='saveToDisk("
                            + finUrl + "," + al[i].ToString() + ");' target='_blank'>" + fileName + "</a><br/>");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            return sbb.ToString(); ;
        }

        #endregion

        #region Button Click Evetns

        /// <summary>
        /// Save/Send IOM Template Details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string btnText = "";
                Filename = FileName();
                string status = "";
                string Path = MapPath("~/uploads/");
                string Atchmnts = Session["IomTmp"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["IomTmp"]).Cast<string>().ToArray());
                if (btnSend.Text == "Send")
                {
                    String[] Cc = new String[0];
                    status = "Sent";
                    status = CommonBLL.SendMailsWithPath(txtFAdr.Text.Trim(), txtPsd.Text.Trim(), txtTAdr.Text.Trim().Split(','), Cc,
                        string.Empty, txtSbjt.Text, txtBdy.Text.Trim(), Atchmnts.Trim().Split(','));
                    if (status == "Sent")
                    {
                        DataSet IOMIDREF = IOMTBLL.GetData(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        txtRefno.Text = "IOM-PUR/" + Session["AliasName"].ToString() + "/" + IOMIDREF.Tables[0].Rows[0]["RefNoID"].ToString()
                       + "/" + CommonBLL.FinacialYearShort;
                        res = IOMTBLL.InsertUpdateDeleteIOMT(CommonBLL.FlagNewInsert, Guid.Empty, txtRefno.Text, txtTAdr.Text, txtFAdr.Text,
                            CommonBLL.DateInsert(txtRcd.Text), new Guid(ddlRefno.SelectedValue), txtpmIn.Text,
                            CommonBLL.DateInsert(txtpIdt.Text), txtSbjt.Text, txtBdy.Text, ddlStatus.SelectedItem.Text,
                            Atchmnts, "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    }
                }
                else if (btnSend.Text == "Update")
                {
                    res = IOMTBLL.InsertUpdateDeleteIOMT(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtRefno.Text, txtTAdr.Text, txtFAdr.Text, CommonBLL.DateInsert(txtRcd.Text), new Guid(ddlRefno.SelectedValue),
                        txtpmIn.Text, CommonBLL.DateInsert(txtpIdt.Text), txtSbjt.Text, txtBdy.Text, ddlStatus.SelectedItem.Text,
                        Atchmnts, txtComments.Text, new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                }
                if (res == 0)
                {
                    if (btnSend.Text == "Send")
                        btnText = "Save";
                    else
                        btnText = btnSend.Text;
                    ALS.AuditLog(res, btnText, "", "IOM Template", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "IOM Form Details", "Data inserted successfully.");
                    ClearInputs();
                    Response.Redirect("IomTemplateStatus.Aspx", false);
                }
                else
                {
                    if (btnSend.Text == "Send")
                    {
                        ALS.AuditLog(res, "Save", "", "IOM Template", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", "Error while Inserting. " + status);
                    }
                    else
                    {
                        ALS.AuditLog(res, "Update", "", "IOM Template", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", "Error while Updating. " + status);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Inputs
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Refference Number Selected index changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefno_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                divListBox.InnerHtml = "";
                divOpen_attachments.InnerHtml = "";
                FillInputFields(IOMTBLL.GetData(CommonBLL.FlagModify, Guid.Empty, new Guid(ddlRefno.SelectedValue), new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM Form Details", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["IomTmp"];
                all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        /// <summary>
        /// This is used to get Proforma Invoice No
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetProformaInvNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo('U', RefNo, Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string attachmnts_ReLoad()
        {
            return Att_open();
        }
        #endregion
    }
}
