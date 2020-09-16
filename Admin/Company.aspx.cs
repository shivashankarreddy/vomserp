using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using BAL;
using System.IO;
using System.Data;
using Ajax;
using System.Text;

namespace VOMS_ERP.Admin
{
    public partial class Company : System.Web.UI.Page
    {

        #region Varaibles

        ErrorLog ELog = new ErrorLog();
        CompanyBLL CmpBLL = new CompanyBLL();
        AuditLogs ALS = new AuditLogs();
        FieldAccessBLL FAB = new FieldAccessBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        static string Filename = "";

        #endregion

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null && Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(Company));
                        if (!IsPostBack)
                        {
                            GetData();
                            ClearAll();
                            #region Add/Update Permission Code
                            //To Check User can have the Add/Update permissions
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                //lbtnsavenew.Enabled = true;
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
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
        /// Bind DropDownLists and GridView Controls
        /// </summary>       
        protected void GetData()
        {
            try
            {
                BindGridData(CmpBLL.SelectCompany(CommonBLL.FlagSelectAll, Guid.Empty));
                BindDropDownList(ddlCurrency,FAB.GetFieldMaster(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty));
                BindDropDownList(ddlRegion, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                CmpnyLogo1.ImageUrl = "~/images/defaultlogo.jpg";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataSet commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt.Tables[0];
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

        protected void BindGridData(DataSet CusstDt)
        {
            try
            {
                gvCompany.DataSource = CusstDt;
                gvCompany.DataBind();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
            }
        }

        protected string ShowNmbrs(object item1)
        {
            try
            {
                string nubr = ((!String.IsNullOrEmpty(item1.ToString()))) ?
                (item1.ToString()) :
                (((!String.IsNullOrEmpty(item1.ToString()))) ?
                ((!String.IsNullOrEmpty(item1.ToString())) ? item1.ToString() : "") : "");
                return nubr;
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// This Method is used to Encrypr PWD
        /// </summary>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        public string MD5(string sPassword)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(sPassword);
                bs = x.ComputeHash(bs);

                foreach (byte b in bs)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
                return s.ToString();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "AssignUsers Master", ex.Message.ToString());
                return "";
            }
        }

        public void ClearAll()
        {
            try
            {
                txtCompany.Text = txtbsnm.Text = txtCmnpypbn.Text = txtCmpnyTtl.Text = ""; UserType.SelectedIndex = -1;
                txtmbl.Text = txtPswrd.Text = txtUsrId.Text = txtAdd.Text = txtCntctPersn.Text = ""; ddlRegion.SelectedIndex = ddlCurrency.SelectedIndex = -1;
                CascadingDropDown1.SelectedValue = "0"; hdfdCmpnyID.Value = ""; CmpnyLogo1.ImageUrl = "~/images/defaultlogo.jpg";
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
            }
        }

        #endregion

        #region Save/Save & New Events for Insert Customer
        /// <summary>
        /// Save/Save & new Button Click Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>       
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int result; string ContactType = "";
                Filename = FileName();
                byte[] img = new byte[0];
                if (UserType.SelectedItem.Text == "Customer")
                    ContactType = CommonBLL.CustmrContactTypeText;
                else
                    ContactType = CommonBLL.TraffickerContactTypeText;
                
                if (ImgCmpLogo.HasFile)
                {
                    string filename = ImgCmpLogo.PostedFile.FileName;
                    string filePath = Path.GetFileName(filename);
                    Stream fs = ImgCmpLogo.PostedFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    img = br.ReadBytes((Int32)fs.Length);
                }
                else if (ViewState["CmpnyLogo"] != null)
                {
                    img = (byte[])ViewState["CmpnyLogo"];
                }
                else
                    img = null;

                if (btnSave.Text == "Save")
                {
                        result = CmpBLL.InsertUpdateCompany(CommonBLL.FlagNewInsert, Guid.Empty, txtCompany.Text, txtbsnm.Text, txtUsrId.Text, MD5(txtPswrd.Text.Trim()),
                            txtAdd.Text, ddlCmpnyCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnyCntry.SelectedValue)
                            , ddlCmpnySt.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnySt.SelectedValue)
                            , ddlCmpnyCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnyCty.SelectedValue),
                            txtCmnpypbn.Text, txtUsrId.Text, txtmbl.Text, txtCntctPersn.Text, txtCmpnyTtl.Text, new Guid(ddlCurrency.SelectedValue), img, ContactType
                            ,UserType.SelectedItem.Text.ToString(),new Guid(ddlRegion.SelectedValue),new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                    
                    if (result == 0)
                    {
                        ALS.AuditLog(result, btnSave.Text, "", "Company Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(CmpBLL.SelectCompany(CommonBLL.FlagSelectAll, Guid.Empty));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Master",
                            "Data Inserted successfully in Company Master.");
                        ClearAll();
                    }
                    else
                    {
                        ALS.AuditLog(result, btnSave.Text, "", "Company Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                         "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "New Email BodyContent", "");
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    Guid CmpnyID = new Guid(hdfdCmpnyID.Value);
                    result = CmpBLL.InsertUpdateCompany(CommonBLL.FlagUpdate, CmpnyID, txtCompany.Text, txtbsnm.Text, txtUsrId.Text, MD5(txtPswrd.Text.Trim()),
                        txtAdd.Text, ddlCmpnyCntry.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnyCntry.SelectedValue)
                        , ddlCmpnySt.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnySt.SelectedValue)
                        , ddlCmpnyCty.SelectedValue == "" ? Guid.Empty : new Guid(ddlCmpnyCty.SelectedValue),
                        txtCmnpypbn.Text, txtUsrId.Text, txtmbl.Text, txtCntctPersn.Text, txtCmpnyTtl.Text, new Guid(ddlCurrency.SelectedValue), img, ContactType
                        ,UserType.SelectedItem.Text.ToString(), new Guid(ddlRegion.SelectedValue),new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                    if (result == 0)
                    {
                        ALS.AuditLog(result, btnSave.Text, "", "Company Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(CmpBLL.SelectCompany(CommonBLL.FlagSelectAll, Guid.Empty));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Company Master",
                            "Data Updated successfully in Company Master.");
                        btnSave.Text = "Save";
                    }
                    else
                    {
                        ALS.AuditLog(result, btnSave.Text, "", "Company Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                         "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "New Email BodyContent", "");
                    }
                }
                ClearAll();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
                GetData();
            }
            catch (Exception ex)
            {
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
            }
        }
        #endregion

        #region Grid RowBound

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCompany_RowDataBound(object sender, GridViewRowEventArgs e)
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCompany_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvCompany.UseAccessibleHeader = false;
                gvCompany.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCompany_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int res = 0;
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvCompany.Rows[index];
                if (e.CommandName == "Modify")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCmpnyId")).Text);
                    SetUpdateValues(CmpBLL.SelectCompany(CommonBLL.FlagModify, ID));
                    btnSave.Text = "Update"; //lbtnsavenew.Visible = false;
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblCmpnyId")).Text);
                    res = CmpBLL.DeleteCompany(CommonBLL.FlagDelete, ID);
                    BindGridData(CmpBLL.SelectCompany(CommonBLL.FlagSelectAll, Guid.Empty));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Customer Master",
                            "Data Deleted successfully in Customer Master.");
                        ClearAll();
                    }
                }
                gvrow.FindControl("lblCmpnyId").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        protected void SetUpdateValues(DataSet CompanyDT)
        {
            try
            {
                hdfdCmpnyID.Value = CompanyDT.Tables[0].Rows[0]["CompanyID"].ToString();
                txtCompany.Text = CompanyDT.Tables[0].Rows[0]["CompanyName"].ToString();
                txtbsnm.Text = CompanyDT.Tables[0].Rows[0]["BusinessName"].ToString();
                txtUsrId.Text = CompanyDT.Tables[0].Rows[0]["UserID"].ToString();
                txtAdd.Text = CompanyDT.Tables[0].Rows[0]["Address"].ToString();
                txtCmnpypbn.Text = CompanyDT.Tables[0].Rows[0]["PostBoxNumber"].ToString();
                txtmbl.Text = CompanyDT.Tables[0].Rows[0]["PhoneNumber"].ToString();
                txtCntctPersn.Text = CompanyDT.Tables[0].Rows[0]["ContactPerson"].ToString();
                txtCmpnyTtl.Text = CompanyDT.Tables[0].Rows[0]["CompanyTitle"].ToString();
                ddlCurrency.SelectedValue = CompanyDT.Tables[0].Rows[0]["Currency"].ToString();
                UserType.SelectedValue = UserType.Items.FindByText(CompanyDT.Tables[0].Rows[0]["UserType"].ToString()).Value;
                ddlRegion.SelectedValue = CompanyDT.Tables[0].Rows[0]["Region"].ToString();
                ViewState["CmpnyLogo"] = (byte[])CompanyDT.Tables[0].Rows[0]["CompanyLogo"];
                CmpnyLogo1.ImageUrl = "ShowImg.ashx?id=" + hdfdCmpnyID.Value;
                CascadingDropDown1.SelectedValue = CompanyDT.Tables[0].Rows[0]["Country"].ToString();
                CascadingDropDown2.SelectedValue = CompanyDT.Tables[0].Rows[0]["State"].ToString();
                CascadingDropDown3.SelectedValue = CompanyDT.Tables[0].Rows[0]["City"].ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Customer Master", ex.Message.ToString());
            }
        }

        #endregion

        #region WebMethod

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool CheckCustomerName(string Name, string Type)
        {
            try
            {
                DataSet dupCmpnyNm = null;
                if (Type == "CompanyName")
                {
                    dupCmpnyNm = new DataSet();
                    dupCmpnyNm = CmpBLL.SelectCompanyforDupChek(CommonBLL.FlagOSelect, Name, "", "");
                    if (dupCmpnyNm.Tables[0].Rows.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }
                else if (Type == "OrgName")
                {
                    dupCmpnyNm = new DataSet();
                    dupCmpnyNm = CmpBLL.SelectCompanyforDupChek(CommonBLL.FlagOSelect, "", Name, "");
                    if (dupCmpnyNm.Tables[0].Rows.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }
                else if (Type == "UsrId")
                {
                    dupCmpnyNm = new DataSet();
                    dupCmpnyNm = CmpBLL.SelectCompanyforDupChek(CommonBLL.FlagOSelect, "", "", Name);
                    if (dupCmpnyNm.Tables[0].Rows.Count > 0)
                    {
                        return false;
                    }
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Company", ex.Message.ToString());
                return false;
            }
        }

        #endregion
    }
}