using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class FieldAccessMaster : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        CompanyBLL CBL = new CompanyBLL();
        FieldAccessBLL FAB = new FieldAccessBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load Event

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
                        if (!IsPostBack)
                        {
                            GetData();
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
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

        private void GetData()
        {
            try
            {
                Guid CompanyID = Guid.Empty;
                if (Session["AccessRole"].ToString() != CommonBLL.SuperAdminRole)
                    CompanyID = new Guid(Session["CompanyId"].ToString());

                BindDropDownList(ddlCompany, CBL.SelectCompany(CommonBLL.FlagRegularDRP, CompanyID).Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataTable commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        public void BindGridView(DataTable dset, bool IsEdit)
        {
            try
            {
                if (dset != null && dset.Rows.Count > 0)
                {
                    gv_FieldAccess.DataSource = dset;
                    gv_FieldAccess.DataBind();
                    btnSave.Text = IsEdit ? "Update" : "Save";
                }
                else
                {
                    gv_FieldAccess.DataSource = null;
                    gv_FieldAccess.DataBind();
                    btnSave.Text = IsEdit ? "Update" : "Save";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
                throw new Exception("Unable to populate GridView" + ex.Message);
            }
        }

        private void EditDetails(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    //BindDropDownList(ddlScreen, FAB.GetFieldMaster(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlCompany.SelectedValue)).Tables[0]);
                    //ddlScreen.SelectedValue = CommonDt.Tables[0].Rows[0]["ScreenID"].ToString();
                    txtSequenceNumber.Text = CommonDt.Tables[0].Rows[0]["SequenceNmbr"].ToString();
                    DataSet Dset = FAB.GetFieldMaster(CommonBLL.FlagBSelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                    ViewState["ID"] = CommonDt.Tables[0].Rows[0]["ID"].ToString();
                    ViewState["EditItems"] = CommonDt.Tables[1];
                    ViewState["PriceTable"] = Dset.Tables[1];
                    ViewState["IsEdit"] = true;
                    BindGridView(Dset.Tables[0], true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
                throw new Exception("Unable to populate GridView" + ex.Message);
            }

        }

        public DataTable GetSelectedFields()
        {
            try
            {
                DataTable FieldList = new DataTable();
                FieldList.Columns.Add("ScreenID");
                FieldList.Columns.Add("FieldID");
                FieldList.Columns.Add("SubFieldID");

                foreach (GridViewRow row in gv_FieldAccess.Rows)
                {
                    if (((CheckBox)row.FindControl("FA_Chkb_Item")).Checked)
                    {
                        FieldList.Rows.Add((new Guid(ddlScreen.SelectedValue)), (new Guid(((Label)(row.FindControl("FA_FeildID"))).Text)),
                            (((Label)(row.FindControl("FA_lbl_FieldDescription"))).Text == CommonBLL.PriceTagText ?
                            (new Guid(((DropDownList)(row.FindControl("ddlPriceID"))).SelectedValue)) : Guid.Empty));
                    }
                }

                return FieldList;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
                return null;
            }
        }

        private void ClearAll()
        {
            try
            {
                ddlCompany.SelectedIndex = -1;
                ddlScreen.Items.Clear();
                txtSequenceNumber.Text = string.Empty;
                BindGridView(null, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Event

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompany.SelectedValue != Guid.Empty.ToString())
                {
                    BindDropDownList(ddlScreen, FAB.GetFieldMaster(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlCompany.SelectedValue)).Tables[0]);
                    BindGridView(null, false);
                    txtSequenceNumber.Text = string.Empty;
                }
                else
                    ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        protected void ddlScreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet FieldTable = FAB.GetFieldDetails(CommonBLL.FlagASelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                if (FieldTable != null && FieldTable.Tables.Count > 0 && FieldTable.Tables[0].Rows.Count > 0)
                {
                    EditDetails(FieldTable);
                }
                else
                {
                    DataSet dset = FAB.GetFieldMaster(CommonBLL.FlagBSelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                    if (dset != null && dset.Tables.Count > 0)
                    {
                        ViewState["PriceTable"] = dset.Tables[1];
                        BindGridView(dset.Tables[0], false);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int Res = -999;
                DataTable SeletedData = GetSelectedFields();
                if (SeletedData.Rows.Count > 0)
                {
                    if (btnSave.Text == "Save")
                    {
                        Res = FAB.InsertUpdatedDeltedFieldDetails(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCompany.SelectedValue), new Guid(ddlScreen.SelectedValue),
                            txtSequenceNumber.Text, Guid.Empty, new Guid(Session["UserID"].ToString()), SeletedData);
                        if (Res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Field Access Master", "Inserted Successfully in Application Field Access Master.");
                            ClearAll();
                        }
                    }
                    else if (btnSave.Text == "Update")
                    {
                        Res = FAB.InsertUpdatedDeltedFieldDetails(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()), new Guid(ddlCompany.SelectedValue), new Guid(ddlScreen.SelectedValue),
                            txtSequenceNumber.Text, Guid.Empty, new Guid(Session["UserID"].ToString()), SeletedData);
                        if (Res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Field Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Field Access Master",
                                "Updated Successfully in Application Access Master.");
                            ClearAll();
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select atleast one field name.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View RowDataBound Event

        protected void gv_FieldAccess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (((Label)e.Row.FindControl("FA_lbl_FieldDescription")).Text == CommonBLL.PriceTagText)
                    {
                        DropDownList ddl = (DropDownList)e.Row.FindControl("ddlPriceID");
                        ddl.Visible = true;
                        BindDropDownList(ddl, (ViewState["PriceTable"] != null ? (DataTable)ViewState["PriceTable"] : null));
                    }
                    if (ViewState["IsEdit"] != null && ((bool)ViewState["IsEdit"]))
                    {
                        if (ViewState["EditItems"] != null)
                        {
                            ((CheckBox)e.Row.FindControl("FA_Chkb_Item")).Checked =
                            (((DataTable)ViewState["EditItems"]).AsEnumerable().Any(r => r.Field<Guid>("FieldID") ==
                                (new Guid(((Label)e.Row.FindControl("FA_FeildID")).Text))));

                            if (((DataTable)ViewState["EditItems"]).AsEnumerable().Any(r => r.Field<Guid>("FieldID") ==
                                (new Guid(((Label)e.Row.FindControl("FA_FeildID")).Text))))
                            {
                                ((DropDownList)e.Row.FindControl("ddlPriceID")).SelectedValue =
                                ((((DataTable)ViewState["EditItems"]).AsEnumerable().Where(r => r.Field<Guid>("FieldID") ==
                                    (new Guid(((Label)e.Row.FindControl("FA_FeildID")).Text))).Select(a => a.Field<Guid>("SubFieldID")).ToArray())[0].ToString());
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Field Access Master", ex.Message.ToString());
            }
        }

        #endregion
    }
}