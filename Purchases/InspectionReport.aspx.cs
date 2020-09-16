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
    public partial class InspectionReport : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        SupplierBLL SBLL = new SupplierBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        ThirdPartyBLL TPBL = new ThirdPartyBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        RqstInsptnPlnBLL IRBL = new RqstInsptnPlnBLL();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(InspectionReport));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        txtDispReadyDt.Attributes.Add("readonly", "readonly");
                        txtToDt.Attributes.Add("readonly", "readonly");
                        txtFrmDt.Attributes.Add("readonly", "readonly");
                        txtInspectionDate.Attributes.Add("readonly", "readonly");

                        if (!IsPostBack)
                        {
                            txtDispReadyDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            txtToDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            txtFrmDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                            txtInspectionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                BindDropDownList(ddlRefNo, IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagRegularDRP, Guid.Empty,"", new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlcustmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    BindDropDownList(ddlRefNo, IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagASelect, Guid.Empty,"", new Guid(Session["CompanyID"].ToString())));
                    EditRecord(IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()),"",new Guid(Session["CompanyID"].ToString())));
                }
                else if (((Request.QueryString["InsPlnID"] != null && Request.QueryString["InsPlnID"] != "") ?
                    new Guid(Request.QueryString["InsPlnID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlRefNo.SelectedValue = Request.QueryString["InsPlnID"].ToString().ToLower();
                    FillInputFields(IRBL.SelectInsptnReport(CommonBLL.FlagJSelect, Guid.Empty, new Guid(ddlRefNo.SelectedValue), Guid.Empty, Guid.Empty, DateTime.Now,
                    DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                    ddlcustmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();

                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagOSelect, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                    ddlsuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();
                    //txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    //txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();

                    txtInsplace.Text = CommonDt.Tables[0].Rows[0]["InsPlace"].ToString();
                    txtPerson.Text = CommonDt.Tables[0].Rows[0]["ContactPerson"].ToString();
                    txtContactNo.Text = CommonDt.Tables[0].Rows[0]["ContactNumber"].ToString();
                    txtInspectionDate.Text = CommonDt.Tables[0].Rows[0]["InsDate"].ToString();
                    //txtDispReadyDt.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    //txtRemarks.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    //txtStartDt.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    //txtEndDt.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    BindDropDownList(ddlSelf, IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagQSelect, Guid.Empty,CommonBLL.TraffickerContactTypeText, new Guid(Session["CompanyID"].ToString())));
                    BindDropDownList(ddlThirdParty, TPBL.SelectThirdParty(CommonBLL.FlagRegularDRP, Guid.Empty));

                    ddlStage.SelectedValue = ddlStage.Items.FindByText(CommonDt.Tables[0].Rows[0]["InsStage"].ToString()).Value;
                    ddlSelf.SelectedValue = CommonDt.Tables[0].Rows[0]["SelfInspector"].ToString();
                    ddlThirdParty.SelectedValue = CommonDt.Tables[0].Rows[0]["ThirdPartyInspector"].ToString();


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
                    ddlcustmr.Enabled = false;
                    ddlsuplr.Enabled = false;
                    ddlStage.Enabled = false;

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList(); //Attachments
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["IpRpt"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                    }

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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

                    ddlRefNo.SelectedValue = CommonDt.Tables[0].Rows[0]["InsPlanID"].ToString();
                    ddlcustmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();

                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagOSelect, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));

                    ddlsuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();
                    //txtCstmr.Text = CommonDt.Tables[0].Rows[0]["CustmNm"].ToString();
                    //txtSuplr.Text = CommonDt.Tables[0].Rows[0]["SuplrNm"].ToString();

                    txtInsplace.Text = CommonDt.Tables[0].Rows[0]["InsPlace"].ToString();
                    txtPerson.Text = CommonDt.Tables[0].Rows[0]["ContactPerson"].ToString();
                    txtContactNo.Text = CommonDt.Tables[0].Rows[0]["ContactNumber"].ToString();
                    txtInspectionDate.Text = CommonDt.Tables[0].Rows[0]["InsDate"].ToString();
                    txtDispReadyDt.Text = (CommonDt.Tables[0].Rows[0]["InsStatus"].ToString() == "Reject") ?
                            CommonDt.Tables[0].Rows[0]["ReplanDt"].ToString() : CommonDt.Tables[0].Rows[0]["DispReadinessDate"].ToString();
                    txtRemarks.Text = CommonDt.Tables[0].Rows[0]["InsDetails"].ToString();
                    txtFrmDt.Text = CommonDt.Tables[0].Rows[0]["StartDate"].ToString();
                    txtToDt.Text = CommonDt.Tables[0].Rows[0]["EndDate"].ToString();
                    ddlInsStatus.SelectedValue = (CommonDt.Tables[0].Rows[0]["InsStatus"].ToString() == "Reject" ? "2" : "1");
                    ddlStage.SelectedValue = ddlStage.Items.FindByText(CommonDt.Tables[0].Rows[0]["InsStage"].ToString()).Value;

                    BindDropDownList(ddlSelf, IRBL.SelectInsptnReportforDespcriptn(CommonBLL.FlagQSelect, Guid.Empty, CommonBLL.TraffickerContactTypeText, new Guid(Session["CompanyID"].ToString())));//CommonBLL.TraffickerContactType
                    BindDropDownList(ddlThirdParty, TPBL.SelectThirdParty(CommonBLL.FlagRegularDRP, Guid.Empty));

                    ddlSelf.SelectedValue = CommonDt.Tables[0].Rows[0]["SelfInspector"].ToString();
                    ddlThirdParty.SelectedValue = CommonDt.Tables[0].Rows[0]["ThirdPartyInspector"].ToString();


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
                    ddlcustmr.Enabled = false;
                    ddlsuplr.Enabled = false;
                    ddlStage.Enabled = false;
                    ddlRefNo.Enabled = false;
                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        Session.Remove("IpRpt");
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["IpRpt"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                    }
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CngDes", "javascrip:StatusChange();", true);
                    ddlcustmr.Enabled = false;
                    //ddlsuplrctgry.Enabled = false;
                    ddlsuplr.Enabled = false;
                    DivComments.Visible = true;
                    btnSend.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                ddlcustmr.SelectedIndex = ddlsuplr.SelectedIndex = ddlStage.SelectedIndex = ddlThirdParty.SelectedIndex = ddlRefNo.SelectedIndex= -1;
                ddlSelf.SelectedIndex = ddlInsStatus.SelectedIndex = -1;
                lbfpos.Items.Clear(); lblpos.Items.Clear(); Session.Remove("IpRpt");
                divListBox.InnerHtml = AttachedFiles();
                txtInsplace.Text = txtPerson.Text = txtContactNo.Text = txtInspectionDate.Text = txtFrmDt.Text = "";
                txtToDt.Text = txtRemarks.Text = "";
                txtDispReadyDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtToDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtFrmDt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtInspectionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                ddlRefNo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                        if (Session["IpRpt"] != null)
                        {
                            alist = (ArrayList)Session["IpRpt"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["IpRpt"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["IpRpt"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                if (Session["IpRpt"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["IpRpt"];
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
                DateTime DispRDt = (txtDispReadyDt.Text == "") ? DateTime.MaxValue : (ddlInsStatus.SelectedItem.Text != "Reject") ?
                    CommonBLL.DateInsert(txtDispReadyDt.Text) : DateTime.MaxValue;
                DateTime RePlanDt = (txtDispReadyDt.Text == "") ? DateTime.MaxValue : (ddlInsStatus.SelectedItem.Text == "Reject") ?
                    CommonBLL.DateInsert(txtDispReadyDt.Text) : DateTime.MaxValue;
                string Atchmnts = Session["IpRpt"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["IpRpt"]).Cast<string>().ToArray());
                if (btnSend.Text == "Save")
                {
                    res = IRBL.InsertUpdateDeleteInsptnReport(CommonBLL.FlagNewInsert,Guid.Empty, new Guid(ddlRefNo.SelectedValue),
                        new Guid(ddlSelf.SelectedValue), new Guid(ddlThirdParty.SelectedValue), CommonBLL.DateInsert(txtFrmDt.Text),
                        CommonBLL.DateInsert(txtToDt.Text), DispRDt, RePlanDt, txtRemarks.Text, ddlInsStatus.SelectedItem.Text,
                        Atchmnts, "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));

                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Report", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Inspection Report",
                            "Data inserted successfully.");
                        ClearInputs();
                        Response.Redirect("InsptnReportStatus.Aspx");
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Report", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report",
                            "Error while Saving.");
                    }
                }
                else if (btnSend.Text == "Update")
                {
                    res = IRBL.InsertUpdateDeleteInsptnReport(CommonBLL.FlagUpdate, new Guid(Request.QueryString["ID"].ToString()),
                        new Guid(ddlRefNo.SelectedValue), new Guid(ddlSelf.SelectedValue), new Guid(ddlThirdParty.SelectedValue),
                        CommonBLL.DateInsert(txtFrmDt.Text), CommonBLL.DateInsert(txtToDt.Text), DispRDt, RePlanDt, txtRemarks.Text,
                        ddlInsStatus.SelectedItem.Text, Atchmnts, txtComments.Text,
                        new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        res = IRBL.InsertUpdateDeleteInsptnPln(CommonBLL.FlagVUpdate, new Guid(ddlRefNo.SelectedValue), "", Guid.Empty,
                            DateTime.Now, txtInsplace.Text, txtPerson.Text, txtContactNo.Text, Guid.Empty, Guid.Empty, "", "", txtComments.Text,
                            new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Inspection Report", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Inspection Report",
                                "Data Updated successfully.");
                            ClearInputs();
                            Response.Redirect("InsptnReportStatus.Aspx");
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Inspection Report", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report",
                                "Error while Updating.");
                        }
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Inspection Report", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
            }
        }
        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// DDL Inspection Plan Number Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlRefNo.SelectedValue != Guid.Empty.ToString())
                {
                    string refno = ddlRefNo.SelectedValue;
                    ClearInputs();
                    ddlRefNo.SelectedValue = refno;
                    FillInputFields(IRBL.SelectInsptnReport(CommonBLL.FlagJSelect, Guid.Empty, new Guid(ddlRefNo.SelectedValue), Guid.Empty, Guid.Empty, DateTime.Now,
                        DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Report", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["IpRpt"];
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
