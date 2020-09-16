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
    public partial class RqstCEDtls : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        SupplierBLL SBLL = new SupplierBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(RqstCEDtls));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            Session["RqstCEDtls"] = null;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                BindDropDownList(ddlcustmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlsuplrctgry, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty,
                    Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                if ((Request.QueryString["CstmrID"] != null && Request.QueryString["CstmrID"] != "") &&
                    (Request.QueryString["SuplrID"] != null && Request.QueryString["SuplrID"] != ""))
                {
                    ddlcustmr.SelectedValue = Request.QueryString["CstmrID"].ToString();
                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagESelect, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlsuplr.SelectedValue = Request.QueryString["SuplrID"].ToString();
                    FillInputs();
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                    txtRefno.Text = CommonDt.Tables[0].Rows[0]["RefNo"].ToString();
                    ddlcustmr.SelectedValue = CommonDt.Tables[0].Rows[0]["CustomerID"].ToString();

                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty,
                        "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value; //"274";
                    ddlsuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString(); //ForeignEnquiryIDs

                    ViewState["FrnEnqs"] = CommonDt.Tables[0].Rows[0]["ForeignEnquiryIDs"].ToString();//Status
                    ViewState["Status"] = CommonDt.Tables[0].Rows[0]["Status"].ToString();

                    BindListBox(lbfpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagKSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        new Guid(ddlsuplr.SelectedValue), CommonDt.Tables[0].Rows[0]["FPOs"].ToString(), "", "",
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lbfpos.Items)
                        li.Selected = true;

                    BindListBox(lblpos, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagLSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        new Guid(ddlsuplr.SelectedValue), "", CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "",
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["RqstCEDtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                    }

                    ddlcustmr.Enabled = false;
                    ddlsuplrctgry.Enabled = false;
                    ddlsuplr.Enabled = false;
                    DivComments.Visible = true;
                    btnSend.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Fill Input Fields
        /// </summary>
        protected void FillInputs()
        {
            try
            {
                DataSet Fpos = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagGSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplr.SelectedValue), "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                DataSet CEDtlsID = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagBSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()));

                if (Fpos.Tables.Count > 1 && Fpos.Tables[0].Rows.Count > 0)
                {
                    BindListBox(lbfpos, Fpos);
                    ddlsuplrctgry.SelectedValue = Fpos.Tables[1].Rows[0]["CategoryID"].ToString();
                    txtRefno.Text = "PInv/" + Session["AliasName"].ToString() + "/" + CEDtlsID.Tables[0].Rows[0]["ID"] + "/"
                        + CommonBLL.FinacialYearShort;
                }
                else
                {
                    lblpos.Items.Clear(); lbfpos.Items.Clear();
                    ddlsuplrctgry.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                ddlcustmr.SelectedValue = ddlsuplr.SelectedValue = ddlsuplrctgry.SelectedValue = Guid.Empty.ToString();
                lbfpos.Items.Clear(); lblpos.Items.Clear(); txtRefno.Text = ""; Session.Remove("RqstCEDtls");
                divListBox.InnerHtml = AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                        if (Session["RqstCEDtls"] != null)
                        {
                            alist = (ArrayList)Session["RqstCEDtls"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["RqstCEDtls"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["RqstCEDtls"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                if (Session["RqstCEDtls"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["RqstCEDtls"];
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
                int LineNo = ExceptionHelper.LineNumber(ex);
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
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
                string Atchmnts = Session["RqstCEDtls"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["RqstCEDtls"]).Cast<string>().ToArray());
                if (btnSend.Text == "Request")
                {
                    DataSet CEDtlsID = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagBSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    txtRefno.Text = "PInv/" + Session["AliasName"].ToString() + "/" + CEDtlsID.Tables[0].Rows[0]["ID"] + "/"
                    + CommonBLL.FinacialYearShort;
                    DataSet RqstCEDtlsID = RCEDBLL.InsertRqstCEDtlsRtnID(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplr.SelectedValue), FrnEnqs, FpoIds, LpoIds, txtRefno.Text, "", "In Progress...", Atchmnts,
                    new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));

                    if (RqstCEDtlsID.Tables.Count > 0)
                    {
                        Response.Redirect("../Masters/EmailSend.aspx?PrID=" + RqstCEDtlsID.Tables[0].Rows[0][0].ToString(), false);
                        ALS.AuditLog(0, "Save", "", "Request For Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Request for Central Excise Details",
                            "Data inserted successfully.");
                        ClearInputs(); string ddd = Server.MapPath("");
                    }
                    else
                    {
                        ALS.AuditLog(-1, "Save", "", "Request For Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details",
                            "Error while Saving.");
                    }
                }

                else if (btnSend.Text == "Update")
                {
                    res = RCEDBLL.InsertUpdateDeleteRqstCEDtls(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), FrnEnqs, FpoIds, LpoIds, txtRefno.Text,
                        txtComments.Text, ViewState["Status"].ToString(), Atchmnts, new Guid(Session["CompanyID"].ToString()),
                        new Guid(Session["UserID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, "Update", "", "Request For Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details",
                            "Data Updated successfully.");
                        ClearInputs();
                        Response.Redirect("RqstCEDtlsStatus.Aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, "Update", "", "Request For Proforma Invoice", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }
        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                {
                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        Guid.Empty, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplrctgry.Enabled = false;
                    lbfpos.Items.Clear(); lblpos.Items.Clear(); txtRefno.Text = "";
                }
                else
                {
                    ClearInputs();
                    //ddlsuplrctgry.Enabled = false;
                    //lbfpos.Items.Clear(); lblpos.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Category Selected index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplrctgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagFSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplrctgry.SelectedValue), "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to get LPO No's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet Lpos = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagINewInsert, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplr.SelectedValue), FpoIds, "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                BindListBox(lblpos, Lpos);
                ViewState["FrnEnqs"] = string.Join(", ", (from dc in Lpos.Tables[1].Rows.Cast<DataRow>()
                                                          select dc.Field<Guid>("ID").ToString()).ToArray());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Request for Central Excise Details", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["RqstCEDtls"];
                all.RemoveAt(ID);
                Session["RqstCEDtls"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }
        #endregion
    }
}
