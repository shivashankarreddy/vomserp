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

namespace VOMS_ERP.Purchases
{
    public partial class InspectionPlan : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        ContactBLL CNCTBL = new ContactBLL();
        ThirdPartyBLL TPBL = new ThirdPartyBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        RqstInsptnPlnBLL IPBL = new RqstInsptnPlnBLL();
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
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(InspectionPlan));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        txtInspectionDate.Attributes.Add("readonly", "readonly");
                        txtInspectionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                        if (!IsPostBack)
                        {
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }

        #endregion

        #region  Methods

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
                BindDropDownList(ddlReqNo, IPBL.SelectInsptnPln(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlReqNo, IPBL.SelectInsptnPln(CommonBLL.FlagASelect, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    EditRecord(IPBL.SelectInsptnPln(CommonBLL.FlagJSelect, new Guid(Request.QueryString["ID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                }
                else if (((Request.QueryString["InsReqID"] != null && Request.QueryString["InsReqID"] != "") ?
                    new Guid(Request.QueryString["InsReqID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlReqNo.SelectedValue = Request.QueryString["InsReqID"].ToString();
                    FillInputFields(IPBL.SelectInsptnPln(CommonBLL.FlagModify, Guid.Empty, new Guid(ddlReqNo.SelectedValue), "", "", "", Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                    txtReferenceNo.Text = "INSPLN/" + Session["AliasName"].ToString() + "/" + CommonDt.Tables[0].Rows[0]["RefNoID"].ToString()
                        + "/" + CommonBLL.FinacialYearShort;
                    txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();

                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                    foreach (ListItem li in lbfpos.Items)
                        li.Selected = true;

                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    lbfpos.Enabled = false;
                    lblpos.Enabled = false;

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["InspectionPlan"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                    }
                    BindDropDownList(ddlSelf, IPBL.SelectInsptnPlnForDropdownBind(CommonBLL.FlagQSelect, CommonBLL.TraffickerContactTypeText, new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddlThirdParty, TPBL.SelectThirdParty(CommonBLL.FlagRegularDRP, Guid.Empty));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit/Update Record
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    txtReferenceNo.Text = CommonDt.Tables[0].Rows[0]["RefNo"].ToString();
                    txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();

                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                    foreach (ListItem li in lbfpos.Items)
                        li.Selected = true;

                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty,
                        CommonDt.Tables[0].Rows[0]["FPOs"].ToString(),
                    CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    ddlReqNo.SelectedValue = CommonDt.Tables[0].Rows[0]["InsReqID"].ToString();
                    txtInspectionDate.Text = CommonDt.Tables[0].Rows[0]["InsDate"].ToString();
                    txtContactNo.Text = CommonDt.Tables[0].Rows[0]["ContactNumber"].ToString();
                    txtPerson.Text = CommonDt.Tables[0].Rows[0]["ContactPerson"].ToString();
                    txtInsplace.Text = CommonDt.Tables[0].Rows[0]["InsPlace"].ToString();

                    BindDropDownList(ddlSelf, IPBL.SelectInsptnPlnForDropdownBind(CommonBLL.FlagQSelect, CommonBLL.TraffickerContactTypeText, new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddlThirdParty, TPBL.SelectThirdParty(CommonBLL.FlagRegularDRP, Guid.Empty));
                    ddlSelf.SelectedValue = CommonDt.Tables[0].Rows[0]["SelfInspector"].ToString();
                    ddlThirdParty.SelectedValue = CommonDt.Tables[0].Rows[0]["ThirdPartyInspector"].ToString();

                    ddlStatus.Items.FindByText(CommonDt.Tables[0].Rows[0]["InsStage"].ToString()).Selected = true;

                    ddlReqNo.Enabled = false;
                    lbfpos.Enabled = false;
                    lblpos.Enabled = false;

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        Session.Remove("InspectionPlan");
                        string[] all = CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
                        CBLL.ClearUploadedFiles();
                        CBLL.UploadedFilesAL(all);
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["InspectionPlan"] = attms;
                        AttachedFiles();
                    }

                    DivComments.Visible = true;
                    btnSend.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                txtInspectionDate.Text = txtCstmr.Text = txtSuplr.Text = txtReferenceNo.Text = txtPerson.Text = "";
                txtInsplace.Text = txtContactNo.Text = txtComments.Text = "";
                ddlReqNo.SelectedIndex = ddlSelf.SelectedIndex = ddlThirdParty.SelectedIndex = ddlStatus.SelectedIndex = -1;
                lbfpos.Items.Clear(); lblpos.Items.Clear(); Session.Remove("InspectionPlan"); divListBox.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                        if (Session["InspectionPlan"] != null)
                        {
                            alist = (ArrayList)Session["InspectionPlan"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["InspectionPlan"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["InspectionPlan"] = alist;
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                if (Session["InspectionPlan"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["InspectionPlan"];
                    StringBuilder sb = new StringBuilder();
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
        #endregion

        #region Button Click Events

        /// <summary>
        /// Send/Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                string Atchmnts = Session["InspectionPlan"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["InspectionPlan"]).Cast<string>().ToArray());
                if (btnSend.Text == "Save")
                {
                    DataSet InsPlnID = IPBL.SelectInsptnPln(CommonBLL.FlagBSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    txtReferenceNo.Text = "INSPLN/" + Session["AliasName"].ToString() + "/" + InsPlnID.Tables[0].Rows[0]["RefNoID"] + "/"
                    + CommonBLL.FinacialYearShort;
                    res = IPBL.InsertUpdateDeleteInsptnPln(CommonBLL.FlagNewInsert, Guid.Empty, txtReferenceNo.Text, new Guid(ddlReqNo.SelectedValue),
                    CommonBLL.DateInsert(txtInspectionDate.Text), txtInsplace.Text, txtPerson.Text, txtContactNo.Text,
                    new Guid(ddlSelf.SelectedValue), new Guid(ddlThirdParty.SelectedValue), ddlStatus.SelectedItem.Text, Atchmnts,
                    "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan",
                            "Data inserted successfully.");
                        ClearInputs();
                        Response.Redirect("InsptnPlnStatus.Aspx");
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan",
                            "Error while Saving.");
                    }
                }
                else if (btnSend.Text == "Update")
                {
                    res = IPBL.InsertUpdateDeleteInsptnPln(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        txtReferenceNo.Text, new Guid(ddlReqNo.SelectedValue),
                    CommonBLL.DateInsert(txtInspectionDate.Text), txtInsplace.Text, txtPerson.Text, txtContactNo.Text,
                    new Guid(ddlSelf.SelectedValue), new Guid(ddlThirdParty.SelectedValue), ddlStatus.SelectedItem.Text, Atchmnts,
                    "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan",
                            "Data Updated successfully.");
                        ClearInputs();
                        Response.Redirect("InsptnPlnStatus.Aspx");
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
            }
        }
        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// DDL Request Numbe Selectece Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlReqNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlReqNo.SelectedValue != Guid.Empty.ToString())
                {
                    string reqstno = ddlReqNo.SelectedValue;
                    ClearInputs();
                    ddlReqNo.SelectedValue = reqstno;
                    FillInputFields(IPBL.SelectInsptnPln(CommonBLL.FlagModify, Guid.Empty, new Guid(ddlReqNo.SelectedValue), "", "", "", Guid.Empty, new Guid(Session["CompanyID"].ToString())));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["InspectionPlan"];
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

    }
}
