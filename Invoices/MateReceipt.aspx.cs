using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Collections;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Invoices
{
    public partial class MateReceipt : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        EnumMasterBLL EMBAL = new EnumMasterBLL();
        MateReceiptBLL MRBL = new MateReceiptBLL();
        ShpngBilDtlsBLL SBDBL = new ShpngBilDtlsBLL();
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
                    Response.Redirect("~/Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations();");
                        Ajax.Utility.RegisterTypeForAjax(typeof(MateReceipt));
                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                txtDate.Attributes.Add("readonly", "readonly");
                BindDropDownList(ddlPrfmaInvcNo, INBLL.SelectPrfmaInvcDtls(CommonBLL.FlagJSelect, Guid.Empty, Guid.Empty, Guid.Empty, "", Guid.Empty, new Guid(Session["CompanyID"].ToString())).Tables[0]);
                BindDropDownList(ddlportloading, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofLoading).Tables[0]);
                BindDropDownList(ddlportdischarge, EMBAL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.PortofDischarge).Tables[0]);
                BindDropDownList(ddlSBNo, SBDBL.SelectShpngBilDtls(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, DateTime.Now, DateTime.Now, CommonBLL.EmptyFACDetails(), new Guid(Session["CompanyID"].ToString())).Tables[0]);
                if (((Request.QueryString["PinvcID"] != null && Request.QueryString["PinvcID"] != "") ?
                    new Guid(Request.QueryString["PinvcID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ddlPrfmaInvcNo.SelectedValue = Request.QueryString["PinvcID"].ToString();
                    FillInputFields(MRBL.SelectMateReceiptDetails(CommonBLL.FlagCommonMstr, Guid.Empty, new Guid(ddlPrfmaInvcNo.SelectedValue),
                        Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                }
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["ID"] = Request.QueryString["ID"].ToString();
                    DataSet ds = MRBL.SelectMateReceiptDetails(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()),
                        Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    BindDropDownList(ddlPrfmaInvcNo, ds.Tables[2]);
                    ddlPrfmaInvcNo.Enabled = false;
                    EditMeteReceipt(ds);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    ddlSBNo.SelectedValue = CommonDt.Tables[0].Rows[0]["ShpngID"].ToString().ToLower();
                    ddlportloading.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLdng"].ToString().Trim().ToLower();
                    ddlportdischarge.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDschrg"].ToString().Trim().ToLower();
                    txtTotalPackages.Text = CommonDt.Tables[0].Rows[0]["TotPkgs"].ToString();
                    txtGrossWt.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                    if (CommonDt.Tables.Count > 1)
                    {
                        gv_CntrDtls.DataSource = CommonDt.Tables[1];
                        gv_CntrDtls.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit Proforma Invoice Details
        /// </summary>
        /// <param name="CommonDt"></param>
        protected void EditMeteReceipt(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0)
                {
                    if (CommonDt != null && CommonDt.Tables.Count > 0)
                    {
                        ddlPrfmaInvcNo.SelectedValue = CommonDt.Tables[0].Rows[0]["RefPInvcID"].ToString();
                        ddlSBNo.SelectedValue = CommonDt.Tables[0].Rows[0]["ShpngBillNmbr"].ToString();
                        ddlportloading.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtLoding"].ToString();
                        ddlportdischarge.SelectedValue = CommonDt.Tables[0].Rows[0]["PrtDschrg"].ToString();
                        txtTotalPackages.Text = CommonDt.Tables[0].Rows[0]["TotlPkgs"].ToString();
                        txtEMNo.Text = CommonDt.Tables[0].Rows[0]["EMNmbr"].ToString();
                        txtGrossWt.Text = CommonDt.Tables[0].Rows[0]["GrossWeight"].ToString();
                        txtDate.Text = CommonDt.Tables[0].Rows[0]["CnMRDate"].ToString();
                        txtMateReceiptNo.Text = CommonDt.Tables[0].Rows[0]["MReceiptNmbr"].ToString();
                        txtLinerName.Text = CommonDt.Tables[0].Rows[0]["LinerName"].ToString();
                        txtVesselName.Text = CommonDt.Tables[0].Rows[0]["VesselName"].ToString();
                        txtForwarderName.Text = CommonDt.Tables[0].Rows[0]["ForwarderName"].ToString();
                        txtRemarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();

                        if (CommonDt.Tables.Count > 1)
                        {
                            gv_CntrDtls.DataSource = CommonDt.Tables[1];
                            gv_CntrDtls.DataBind();
                        }

                        if (CommonDt.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        {
                            ArrayList attms = new ArrayList();
                            attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                            Session["MateRcpt"] = attms;
                            divListBox.InnerHtml = AttachedFiles();
                            imgAtchmt.Visible = true;
                        }
                        else
                            imgAtchmt.Visible = false;

                        DivComments.Visible = true;
                        btnSave.Text = "Update";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Convert Gridveiw to Data Table
        /// </summary>
        /// <param name="Gv"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView Gv)
        {
            try
            {
                DataTable dt = CommonBLL.PrfmaInvcItems();
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                ddlPrfmaInvcNo.SelectedIndex = ddlSBNo.SelectedIndex = ddlportloading.SelectedIndex = ddlportdischarge.SelectedIndex = -1;
                txtMateReceiptNo.Text = txtTotalPackages.Text = txtEMNo.Text = txtGrossWt.Text = txtLinerName.Text = "";
                txtVesselName.Text = txtForwarderName.Text = txtRemarks.Text = " ";

                BindGridVeiw(gv_CntrDtls, null);

                divListBox.InnerHtml = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                        if (Session["MateRcpt"] != null)
                        {
                            alist = (ArrayList)Session["MateRcpt"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["MateRcpt"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["MateRcpt"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                if (Session["MateRcpt"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["MateRcpt"];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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

        #endregion

        #region Selected Index/Text Changed Events

        /// <summary>
        /// Check List Drop Down List Selected Index Chnged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPrfmaInvcNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillInputFields(MRBL.SelectMateReceiptDetails(CommonBLL.FlagCommonMstr, Guid.Empty, new Guid(ddlPrfmaInvcNo.SelectedValue),
                    Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events


        /// <summary>
        /// Grid View Row-Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_CntrDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    if (ViewState["CrntItms"] != null)
                    {
                        DataTable CrntItms = (DataTable)ViewState["CrntItms"];
                        Guid RefID = new Guid(((Label)e.Row.FindControl("lblsItemDtlsID")).Text);
                        ((CheckBox)e.Row.FindControl("chkbitm")).Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_CntrDtls_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gv_CntrDtls.HeaderRow == null) return;
                gv_CntrDtls.UseAccessibleHeader = false;
                gv_CntrDtls.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                string Atchmnts = Session["MateRcpt"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["MateRcpt"]).Cast<string>().ToArray());
                if (btnSave.Text == "Save")
                {
                    res = MRBL.InsertUpdateDeleteMateReceiptDtls(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlPrfmaInvcNo.SelectedValue),
                        new Guid(ddlSBNo.SelectedValue), txtMateReceiptNo.Text, CommonBLL.DateInsert(txtDate.Text),
                        new Guid(ddlportloading.SelectedValue), new Guid(ddlportdischarge.SelectedValue), Convert.ToDecimal(txtTotalPackages.Text),
                        txtEMNo.Text, Convert.ToDecimal(txtGrossWt.Text), txtLinerName.Text, txtVesselName.Text, txtForwarderName.Text,
                        txtRemarks.Text, Atchmnts, "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Mate Receipt", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Mate Receipt",
                            "Data inserted successfully.");
                        ClearInputs();
                        Response.Redirect("MateReceiptStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Mate Receipt", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt",
                            "Error while Saving.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = MRBL.InsertUpdateDeleteMateReceiptDtls(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()),
                        new Guid(ddlPrfmaInvcNo.SelectedValue), new Guid(ddlSBNo.SelectedValue), txtMateReceiptNo.Text, CommonBLL.DateInsert(txtDate.Text),
                        new Guid(ddlportloading.SelectedValue), new Guid(ddlportdischarge.SelectedValue), Convert.ToDecimal(txtTotalPackages.Text),
                        txtEMNo.Text, Convert.ToDecimal(txtGrossWt.Text), txtLinerName.Text, txtVesselName.Text, txtForwarderName.Text,
                        txtRemarks.Text, Atchmnts, txtComments.Text, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Mate Receipt", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/Log"), "Mate Receipt",
                            "Updated successfully.");
                        ClearInputs();
                        Response.Redirect("MateReceiptStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Mate Receipt", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt",
                            "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
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
                ArrayList all = (ArrayList)Session["MateRcpt"];
                all.RemoveAt(ID);
                Session["MateRcpt"] = all;
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Mate Receipt", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool GetMR(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('A', EnqNo,new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        #endregion
    }
}