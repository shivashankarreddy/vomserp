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
    public partial class InspectionPlanRequest : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        SupplierBLL SBLL = new SupplierBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstInsptnPlnBLL RIPBL = new RqstInsptnPlnBLL();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(InspectionPlanRequest));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                BindDropDownList(ddlsuplrctgry, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
                //BindDropDownList(ddlsuplr, SBLL.GetSuppliersByFeDescrptn(CommonBLL.FlagSelectAll, CommonBLL.CategoryForSupp, FEID));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    EditRecord(RIPBL.SelectRqstInsptnPln(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString())));
                }
                else if (Request.QueryString["LPOID"] != null)
                {
                    ddlcustmr.SelectedValue = Request.QueryString["CustID"];
                    string cstmr = ddlcustmr.SelectedValue;
                    ClearInputs();
                    ddlcustmr.SelectedValue = cstmr;
                    BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagESelect, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText(CommonBLL.CategoryForSupp).Value;
                    ddlsuplrctgry.Enabled = false;

                    ddlsuplr.SelectedValue = Request.QueryString["SupID"];
                    DataSet Fpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagGSelect, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), "", "", "", new Guid(Session["UserID"].ToString()),
                        new Guid(Session["CompanyID"].ToString()));
                    if (Fpos.Tables.Count > 0)
                        BindListBox(lbfpos, Fpos);
                    ddlsuplrctgry.SelectedValue = Fpos.Tables[1].Rows[0]["CategoryID"].ToString();
                    txtRefno.Text = "INSP/" + Session["AliasName"].ToString() + "/" + Fpos.Tables[1].Rows[0]["ID"].ToString() + "/"
                        + CommonBLL.FinacialYearShort;

                    lbfpos.SelectedValue = Request.QueryString["FPOID"];
                    string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet Lpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagINewInsert, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), FpoIds, "", "", new Guid(Session["UserID"].ToString())
                        , new Guid(Session["CompanyID"].ToString()));
                    ViewState["FrnEnqs"] = string.Join(", ", (from dc in Lpos.Tables[1].Rows.Cast<DataRow>()
                                                              select dc.Field<Int64>("ID").ToString()).ToArray());
                    BindListBox(lblpos, Lpos);
                    lblpos.SelectedValue = Request.QueryString["LPOID"];
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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

                    BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagESelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty,
                        "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplr.SelectedValue = CommonDt.Tables[0].Rows[0]["SupplierID"].ToString();

                    BindListBox(lbfpos, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagGSelect, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), "", "", "", new Guid(Session["UserID"].ToString()),
                    new Guid(Session["CompanyID"].ToString())));

                    foreach (ListItem li in lbfpos.Items)
                        if (CommonDt.Tables[0].Rows[0]["FPOs"].ToString().Contains(li.Value))
                            li.Selected = true;

                    BindListBox(lblpos, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagLSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        new Guid(ddlsuplr.SelectedValue), "", CommonDt.Tables[0].Rows[0]["LPOs"].ToString(), "", new Guid(Session["UserID"].ToString()),
                        new Guid(Session["CompanyID"].ToString())));
                    foreach (ListItem li in lblpos.Items)
                        li.Selected = true;

                    if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        Session.Remove("RqstIPDtls");
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["RqstIPDtls"] = attms;
                        divListBox.InnerHtml = AttachedFiles();
                    }
                    if (CommonDt != null && CommonDt.Tables.Count > 1)
                        ViewState["FrnEnqs"] = string.Join(", ", (from dc in CommonDt.Tables[1].Rows.Cast<DataRow>()
                                                                  select dc.Field<string>("ID").ToString()).ToArray());

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                ddlcustmr.SelectedIndex = ddlsuplr.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                lbfpos.Items.Clear(); lblpos.Items.Clear(); txtRefno.Text = ""; Session.Remove("RqstIPDtls");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                        if (Session["RqstIPDtls"] != null)
                        {
                            alist = (ArrayList)Session["RqstIPDtls"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["RqstIPDtls"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["RqstIPDtls"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                if (Session["RqstIPDtls"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["RqstIPDtls"];
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
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string LpoIds = String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                string FrnEnqs = ViewState["FrnEnqs"] != null ? ViewState["FrnEnqs"].ToString() : "";
                string Atchmnts = Session["RqstIPDtls"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["RqstIPDtls"]).Cast<string>().ToArray());
                if (btnSend.Text == "Request")
                {
                    DataSet IPDtlsID = RIPBL.SelectRqstInsptnPlnDescp(CommonBLL.FlagBSelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    txtRefno.Text = "INSP/" + Session["AliasName"].ToString() + "/" + IPDtlsID.Tables[0].Rows[0]["ID"] + "/"
                    + CommonBLL.FinacialYearShort;
                    DataSet RqstInsptPlnID = RIPBL.InsertRqstInsptnPlnRtnIDDesc(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                     new Guid(ddlsuplr.SelectedValue), FrnEnqs, FpoIds, LpoIds, txtRefno.Text, "", "In Progress...", Atchmnts,
                     new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));

                    if (RqstInsptPlnID.Tables.Count > 0)
                    {
                        Response.Redirect("../Masters/EmailSend.aspx?IPID=" + RqstInsptPlnID.Tables[0].Rows[0]["InspID"].ToString());
                        ALS.AuditLog(0, "Save", "", "Request for Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request",
                            "Data inserted successfully.");
                        ClearInputs();
                    }
                    else
                    {
                        ALS.AuditLog(-1, "Save", "", "Request for Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request",
                            "Error while Saving.");
                    }
                }

                else if (btnSend.Text == "Update")
                {
                    res = RIPBL.InsertUpdateDeleteRqstInsptnPln(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), FrnEnqs, FpoIds, LpoIds, txtRefno.Text,
                        txtComments.Text, "In Progress...", Atchmnts, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Request for Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request",
                            "Data Updated successfully.");
                        ClearInputs();
                        Response.Redirect("RqstInsptnPlnStatus.Aspx");
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSend.Text, "", "Request for Inspection Plan", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                if (ddlcustmr.SelectedValue != "0")
                {
                    string cstmr = ddlcustmr.SelectedValue;
                    ClearInputs();
                    ddlcustmr.SelectedValue = cstmr;
                    BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagESelect, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplrctgry.Enabled = false;
                }
                else
                {
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                BindDropDownList(ddlsuplr, RIPBL.SelectRqstInsptnPln(CommonBLL.FlagFSelect, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplrctgry.SelectedValue), "", "", "", new Guid(Session["UserID"].ToString()),
                    new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                if (ddlsuplr.SelectedValue != "0")
                {
                    DataSet Fpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagGSelect, Guid.Empty,
                        new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (Fpos.Tables.Count > 0)
                        BindListBox(lbfpos, Fpos);
                    ddlsuplrctgry.SelectedValue = Fpos.Tables[1].Rows[0]["CategoryID"].ToString();
                    txtRefno.Text = "INSP/" + Session["AliasName"].ToString() + "/" + Fpos.Tables[1].Rows[0]["ID"].ToString() + "/"
                        + CommonBLL.FinacialYearShort;
                }
                else
                {
                    lbfpos.Items.Clear();
                    lblpos.Items.Clear();
                    txtRefno.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
            }
        }

        /// <summary>
        /// DropDownList FPO's Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbfpos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //var selectedNames = lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();
                string FpoIds = String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet Lpos = RIPBL.SelectRqstInsptnPln(CommonBLL.FlagINewInsert, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplr.SelectedValue), FpoIds, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (Lpos.Tables[0] != null && Lpos.Tables[0].Rows.Count > 0 && Convert.ToBoolean(Lpos.Tables[0].Rows[0]["IsVerbalLPO"].ToString()) == false )
                    ViewState["FrnEnqs"] = string.Join(", ", (from dc in Lpos.Tables[1].Rows.Cast<DataRow>() select dc.Field<string>("ID").ToString()).ToArray());

                BindListBox(lblpos, Lpos);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Request", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["RqstIPDtls"];
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
