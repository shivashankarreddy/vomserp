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
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class CompanyDetails : System.Web.UI.Page
    {
        # region variables
        int res;
        BAL.CustomerBLL CSTMRBL = new BAL.CustomerBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        CompanyBLL CMPBLL = new CompanyBLL();
        CompanyDetailsBLL CMPDBLL = new CompanyDetailsBLL();
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
                    Response.Redirect("../login.aspx", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                            GetData();
                        #region Add/Update Permission Code
                        //To Check User can have the Add/Update permissions
                        if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                        {
                            btnSave.Enabled = true;
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                        {
                            btnSave.Enabled = false;
                            btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        }
                        #endregion
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
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
        /// Default Page Data Load Method
        /// </summary>
        private void GetData()
        {
            try
            {
                //ClearInputs();
                if (CommonBLL.SuperAdminRole == Session["AccessRole"].ToString())
                {
                    BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagCommonMstr, new Guid(Session["CompanyID"].ToString())));
                }
                else
                    BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlCntry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty,
                    Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                BindDropDownList(ddlorgnm, CMPBLL.SelectCompany(CommonBLL.FlagASelect, new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        protected void FillInputFields(DataSet CommonDt)
        {
            try
            {
                ClearInputs();
                if (CommonDt != null && CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    BindDropDownList(ddlorgnm, CMPBLL.SelectCompany(CommonBLL.FlagASelect, new Guid(Session["CompanyID"].ToString())));
                    hdfdCmpDtlsID.Value = CommonDt.Tables[0].Rows[0]["ID"].ToString().Trim();
                    //ddlorgnm.SelectedValue = ddlorgnm.Items.FindByText(CommonDt.Tables[0].Rows[0]["CMNAME"].ToString().ToUpper().Trim()).Value;
                    ddlorgnm.SelectedValue = CommonDt.Tables[0].Rows[0]["CMID"].ToString();
                    txtbsnm.Text = CommonDt.Tables[0].Rows[0]["BusinessName"].ToString();
                    txttpn.Text = CommonDt.Tables[0].Rows[0]["Telephone"].ToString().Trim();
                    txtmbl.Text = CommonDt.Tables[0].Rows[0]["AltContact"].ToString().Trim();
                    txtpem.Text = CommonDt.Tables[0].Rows[0]["PMail"].ToString().Trim();
                    txtsem.Text = CommonDt.Tables[0].Rows[0]["SMail"].ToString().Trim();
                    txtfx.Text = CommonDt.Tables[0].Rows[0]["Fax"].ToString().Trim();
                    txtCntctPrsn.Text = CommonDt.Tables[0].Rows[0]["ContactPrsn"].ToString().Trim();
                    txtAltCntctPrsn.Text = CommonDt.Tables[0].Rows[0]["AltContactPrsn"].ToString().Trim();

                    ddlSt.ClearSelection(); ddlCty.ClearSelection(); ddlCntry.ClearSelection();
                    ddlCntry.Items.FindByText(CommonDt.Tables[0].Rows[0]["Country"].ToString()).Selected = true;
                    BindDropDownList(ddlSt, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.State));
                    ddlSt.Items.FindByText(CommonDt.Tables[0].Rows[0]["State"].ToString()).Selected = true;
                    //BindDropDownList(ddlCty, embal.EnumMasterSelectforDescription(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),CommonBLL.City));
                    BindDropDownList(ddlCty, embal.EnumMasterSelectforDescription(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, new Guid(ddlSt.SelectedValue), new Guid(Session["CompanyID"].ToString()), CommonBLL.City));
                    ddlCty.Items.FindByText(CommonDt.Tables[0].Rows[0]["City"].ToString()).Selected = true;
                    txtstrt.Text = CommonDt.Tables[0].Rows[0]["Address1"].ToString().Trim();
                    txtArea.Text = CommonDt.Tables[0].Rows[0]["Address2"].ToString().Trim();
                    txtpb.Text = CommonDt.Tables[0].Rows[0]["Address3"].ToString().Trim();
                    txtEmailCC.Text = CommonDt.Tables[0].Rows[0]["EmailCC"].ToString().Trim();
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearInputs()
        {
            try
            {

                txttpn.Text = txtmbl.Text = txtpem.Text = txtsem.Text = txtfx.Text = txtbsnm.Text = "";
                txtCntctPrsn.Text = txtAltCntctPrsn.Text = txtstrt.Text = txtArea.Text = txtpb.Text = ""; txtEmailCC.Text = "";
                ddlorgnm.SelectedIndex = -1;
                ddlCntry.SelectedIndex = ddlSt.SelectedIndex = ddlCty.SelectedIndex = -1;
                hdfdCmpDtlsID.Value = Guid.Empty.ToString();
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid Data
        /// </summary>
        /// <param name="CusstDt"></param>
        protected void BindGridData(DataSet CusstDt)
        {
            try
            {
                if (CusstDt != null && CusstDt.Tables.Count > 0)
                {
                    DataView DView = new DataView(CusstDt.Tables[0]);
                    DView.Sort = "ID DESC";
                    gvCmpnyDtls.DataSource = DView;
                }
                else
                    gvCmpnyDtls.DataSource = null;
                gvCmpnyDtls.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Drop Down Lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.Items.Clear();
                if (CommonDt != null && CommonDt.Tables.Count > 0)
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
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
                if (btnSave.Text == "Save")
                {
                    res = CSTMRBL.InsertUpdateDeleteCmpnyDtlToInsert(CommonBLL.FlagNewInsert, Guid.Empty, ddlorgnm.SelectedItem.Text, txtCntctPrsn.Text,
                        txtAltCntctPrsn.Text, txtstrt.Text, txtArea.Text, txtpb.Text, ddlCty.SelectedItem.Text, ddlSt.SelectedItem.Text,
                        ddlCntry.SelectedItem.Text, txttpn.Text, txtmbl.Text, txtfx.Text, txtpem.Text, txtsem.Text,
                        new Guid(Session["UserID"].ToString()), new Guid(ddlorgnm.SelectedValue), txtEmailCC.Text);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Company Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Details",
                            "Data Inserted successfully.");
                        ClearInputs();
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Company Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Details", "Error while Saving.");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    res = CSTMRBL.InsertUpdateDeleteCmpnyDtlToInsert(CommonBLL.FlagUpdate, new Guid(hdfdCmpDtlsID.Value),
                        ddlorgnm.SelectedItem.Text, txtCntctPrsn.Text,
                        txtAltCntctPrsn.Text, txtstrt.Text, txtArea.Text, txtpb.Text, ddlCty.SelectedItem.Text,
                        ddlSt.SelectedItem.Text, ddlCntry.SelectedItem.Text, txttpn.Text, txtmbl.Text, txtfx.Text,
                        txtpem.Text, txtsem.Text, new Guid(Session["UserID"].ToString()), new Guid(ddlorgnm.SelectedValue), txtEmailCC.Text);
                    if (res == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Company Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        if (CommonBLL.SuperAdminRole == Session["AccessRole"].ToString())
                        {
                            BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagCommonMstr, new Guid(Session["CompanyID"].ToString())));
                        }
                        else
                            BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagSelectAll, new Guid(Session["CompanyID"].ToString())));

                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Details",
                            "Data Updated successfully.");
                        ClearInputs();
                    }
                    else
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Company Details", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Details", "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
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
                Response.Redirect("../Masters/CompanyDetails.aspx");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmpnyDtls_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvCmpnyDtls.UseAccessibleHeader = false;
                gvCmpnyDtls.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmpnyDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvCmpnyDtls.Rows[index];
                if (e.CommandName == "Modify")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCmpnyDtls")).Text);
                    FillInputFields(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagModify, ID));
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCmpnyDtls")).Text);
                    res = CSTMRBL.InsertUpdateDeleteCmpnyDtlToInsert(CommonBLL.FlagDelete, ID, "", "", "", "", "", "", "", "", "", "", "",
                        "", "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyId"].ToString()), "");
                    BindGridData(CSTMRBL.SelectCmpnyDtls(CommonBLL.FlagSelectAll, Guid.Empty));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Details",
                            "Data Deleted successfully.");
                        ClearInputs();
                    }
                }
                gvrow.FindControl("lblCmpnyDtls").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCmpnyDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];

                if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
                    EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";

                if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                else
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }
        #endregion

        #region Selected Index Changed Events
        protected void ddlCntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCntry.SelectedValue != "0")
                    BindDropDownList(ddlSt, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.State));
                else
                    BindDropDownList(ddlSt, null);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }

        protected void ddlSt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlSt.SelectedValue != "0")
                    //BindDropDownList(ddlCty, embal.EnumMasterSelect(CommonBLL.FlagCommonMstr, 0, 0, int.Parse(ddlSt.SelectedValue), Convert.ToInt32(Session["CompanyID"])));
                    BindDropDownList(ddlCty, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.City));
                else
                    BindDropDownList(ddlCty, null);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }


        protected void ddlorgnm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlorgnm.SelectedValue != Guid.Empty.ToString())
                {
                    DataSet Ds = new DataSet();
                    Ds = CMPBLL.SelectCompany(CommonBLL.FlagXSelect, new Guid(ddlorgnm.SelectedValue));
                    if (Ds != null && Ds.Tables != null && Ds.Tables[0].Rows.Count == 0)
                    {
                        txtbsnm.Text = Ds.Tables[1].Rows[0]["BusinessName"].ToString();
                        txtmbl.Text = Ds.Tables[1].Rows[0]["PhoneNumber"].ToString();
                        txtpem.Text = Ds.Tables[1].Rows[0]["PrimaryEMail"].ToString();
                        txtCntctPrsn.Text = Ds.Tables[1].Rows[0]["ContactPerson"].ToString();
                        ddlCntry.SelectedValue = Ds.Tables[1].Rows[0]["Country"].ToString();
                        BindDropDownList(ddlSt, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.State));
                        ddlSt.SelectedValue = Ds.Tables[1].Rows[0]["State"].ToString();
                        BindDropDownList(ddlCty, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.City));
                        ddlCty.SelectedValue = Ds.Tables[1].Rows[0]["City"].ToString();
                        txtstrt.Text = Ds.Tables[1].Rows[0]["Address"].ToString();
                        btnSave.Text = "Save";
                    }
                    else
                    {
                        hdfdCmpDtlsID.Value = Ds.Tables[2].Rows[0]["ID"].ToString();
                        txtbsnm.Text = Ds.Tables[2].Rows[0]["BusinessName"].ToString();
                        txttpn.Text = Ds.Tables[2].Rows[0]["Telephone"].ToString();
                        txtmbl.Text = Ds.Tables[2].Rows[0]["AltContact"].ToString();
                        txtpem.Text = Ds.Tables[2].Rows[0]["PMail"].ToString();
                        txtsem.Text = Ds.Tables[2].Rows[0]["SMail"].ToString();
                        txtfx.Text = Ds.Tables[2].Rows[0]["Fax"].ToString();
                        txtCntctPrsn.Text = Ds.Tables[2].Rows[0]["ContactPrsn"].ToString();
                        txtAltCntctPrsn.Text = Ds.Tables[2].Rows[0]["AltContactPrsn"].ToString();

                        //ddlCntry.SelectedValue = ddlCntry.Items.FindByText(Ds.Tables[2].Rows[0]["Country"].ToString().ToLower()).Value;
                        var Country = from i in ddlCntry.Items.Cast<ListItem>()
                                      where ((ListItem)i).Text.ToLower().Contains(Ds.Tables[2].Rows[0]["Country"].ToString().ToLower())
                                      select i;
                        ddlCntry.SelectedValue = Country.ToList()[0].Value;

                        BindDropDownList(ddlSt, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.State));
                        //ddlSt.SelectedValue = ddlSt.Items.FindByText(Ds.Tables[2].Rows[0]["State"].ToString()).Value;
                        var State = from i in ddlSt.Items.Cast<ListItem>()
                                    where ((ListItem)i).Text.ToLower().Contains(Ds.Tables[2].Rows[0]["State"].ToString().ToLower())
                                    select i;
                        ddlSt.SelectedValue = State.ToList()[0].Value;

                        BindDropDownList(ddlCty, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.City));
                        //ddlCty.SelectedValue = ddlCty.Items.FindByText(Ds.Tables[2].Rows[0]["City"].ToString()).Value;
                        var City = from i in ddlCty.Items.Cast<ListItem>()
                                   where ((ListItem)i).Text.ToLower().Contains(Ds.Tables[2].Rows[0]["City"].ToString().ToLower())
                                   select i;
                        ddlCty.SelectedValue = City.ToList()[0].Value;

                        txtstrt.Text = Ds.Tables[2].Rows[0]["Address1"].ToString();
                        txtArea.Text = Ds.Tables[2].Rows[0]["Address2"].ToString();
                        txtpb.Text = Ds.Tables[2].Rows[0]["Address3"].ToString();
                        txtEmailCC.Text = Ds.Tables[2].Rows[0]["EmailCC"].ToString();
                        btnSave.Text = "Update";
                    }
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company Details", ex.Message.ToString());
            }
        }
        #endregion
    }
}
