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
using System.Threading;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class Supplier : System.Web.UI.Page
    {
        # region variables
        int res;
        BAL.SupplierBLL suplrbl = new BAL.SupplierBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        FieldAccessBLL FAB = new FieldAccessBLL();
        static string Filename = "";
        #endregion

        #region Default Page Load Event

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                gvsupplier.Attributes.Add("onkeyup", "alert();");
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(Supplier));
                        #region Add/Update Permission Code
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier", ex.Message.ToString());
            }
        }
        #endregion

        #region Save Events for Insert Into Supplier

        /// <summary>
        /// Save Button Click Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                //int CmpnyID = 0;
                //if (Session["CompanyID"] != "")
                //{
                //    CmpnyID = Convert.ToInt16(Session["CompanyID"]);
                //}
                string ContactName = "";
                if (txtCtPrsn.Text.Trim() != "")
                    ContactName = ddlHonorific.SelectedValue + " " + txtCtPrsn.Text.Trim();
                if (btnSave.Text == "Save")
                {
                    int result = suplrbl.InsertUpdateSupplierForInsert(CommonBLL.FlagNewInsert, Guid.Empty, txtorgnm.Text.Trim(), txtbsnm.Text.Trim(), txttpn.Text,
                                txtmbl.Text, txtpem.Text, txtsem.Text, txtfx.Text, ddlregion.SelectedItem.Text.ToString(),
                                ddlctgry.SelectedValue == Guid.Empty.ToString() ? Guid.Empty : new Guid(ddlctgry.SelectedValue), txtblngstrt.Text,
                                ((ddlBilngCty.SelectedValue != "" && ddlBilngCty.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlBilngCty.SelectedValue) : Guid.Empty),
                                ((ddlblngSt.SelectedValue != "" && ddlblngSt.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlblngSt.SelectedValue) : Guid.Empty), txtblngpb.Text,
                                ((ddlblngCntry.SelectedValue != "" && ddlblngCntry.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlblngCntry.SelectedValue) : Guid.Empty), txtshpngstrt.Text,
                                ((ddlshpngCty.SelectedValue != "" && ddlshpngCty.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngCty.SelectedValue) : Guid.Empty),
                                ((ddlshpngSt.SelectedValue != "" && ddlshpngSt.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngSt.SelectedValue) : Guid.Empty), txtshpngpb.Text,
                                ((ddlshpngCntry.SelectedValue != "" && ddlshpngCntry.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngCntry.SelectedValue) : Guid.Empty), ContactName,
                                ddlBanks.SelectedValue == Guid.Empty.ToString() ? Guid.Empty : new Guid(ddlBanks.SelectedValue), new Guid(ddlCurrency.SelectedValue), txtbrncNm.Text, txtacntnbr.Text, txtrtgscd.Text, txtRange.Text,
                                txtDivision.Text, txtCommissionerate.Text, txtRngAdrs.Text, txtDvsnAdrs.Text, txtCmsnAdrs.Text, txtGST.Text, txtIecNo.Text,
                                new Guid(Session["CompanyID"].ToString()), Guid.Empty, new Guid(Session["UserID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Supplier Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(suplrbl.SelectSuppliersForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Supplier Master",
                            "Data Inserted successfully in Supplier Master.");
                        ClearInputs();
                    }
                }
                else if (btnSave.Text == "Update")
                {
                    Guid SuplrID = new Guid(hdfdsuplrID.Value);
                    int result = suplrbl.InsertUpdateSupplierForInsert(CommonBLL.FlagUpdate, SuplrID, txtorgnm.Text, txtbsnm.Text, txttpn.Text,
                                txtmbl.Text, txtpem.Text, txtsem.Text, txtfx.Text, ddlregion.SelectedItem.Text.ToString(),
                                ddlctgry.SelectedValue == Guid.Empty.ToString() ? Guid.Empty : new Guid(ddlctgry.SelectedValue), txtblngstrt.Text,
                                ((ddlBilngCty.SelectedValue != "" && ddlBilngCty.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlBilngCty.SelectedValue) : Guid.Empty),
                                ((ddlblngSt.SelectedValue != "" && ddlblngSt.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlblngSt.SelectedValue) : Guid.Empty), txtblngpb.Text,
                                ((ddlblngCntry.SelectedValue != "" && ddlblngCntry.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlblngCntry.SelectedValue) : Guid.Empty), txtshpngstrt.Text,
                                ((ddlshpngCty.SelectedValue != "" && ddlshpngCty.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngCty.SelectedValue) : Guid.Empty),
                                ((ddlshpngSt.SelectedValue != "" && ddlshpngSt.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngSt.SelectedValue) : Guid.Empty), txtshpngpb.Text,
                                ((ddlshpngCntry.SelectedValue != "" && ddlshpngCntry.SelectedValue != Guid.Empty.ToString()) ? new Guid(ddlshpngCntry.SelectedValue) : Guid.Empty), ContactName,
                                ddlBanks.SelectedValue == Guid.Empty.ToString() ? Guid.Empty : new Guid(ddlBanks.SelectedValue), new Guid(ddlCurrency.SelectedValue), txtbrncNm.Text, txtacntnbr.Text, txtrtgscd.Text, txtRange.Text,
                                txtDivision.Text, txtCommissionerate.Text, txtRngAdrs.Text, txtDvsnAdrs.Text, txtCmsnAdrs.Text, txtGST.Text, txtIecNo.Text,
                                new Guid(Session["CompanyID"].ToString()), Guid.Empty, new Guid(Session["UserID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Supplier Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        BindGridData(suplrbl.SelectSuppliersForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Supplier Master",
                            "Data Updated successfully in Supplier Master.");
                        btnSave.Text = "Save";
                        ClearInputs();
                    }
                }
                ddlctgry.SelectedItem.Text = CommonBLL.CategoryForSupp;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear All Control values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        #endregion

        #region Bind DropDownList and GirdView Controls

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
                BindGridData(suplrbl.SelectSuppliersForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddlctgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), BAL.CommonBLL.SupplierCategory));
                BindDropDownList(ddlBanks, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), BAL.CommonBLL.aBank));
                BindDropDownList(ddlblngCntry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                BindDropDownList(ddlshpngCntry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries));
                BindDropDownList(ddlCurrency, FAB.GetFieldMaster(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty));
                if (Session["UserType"].ToString() != "" && Session["UserType"].ToString() == "Customer")
                {
                    lblRegion.Visible = true;
                    ddlregion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier", ex.Message.ToString());
            }
        }
        protected void BindGridData(DataSet SuplrDt)
        {
            try
            {
                if (SuplrDt != null)
                {
                    gvsupplier.DataSource = SuplrDt.Tables[0];
                }
                else
                    gvsupplier.DataSource = null;
                gvsupplier.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier", ex.Message.ToString());
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
                ddl.Enabled = true;
                if (ddl.ID == "ddlctgry")
                {
                    ddl.SelectedValue = ddl.Items.FindByText("General").Value;
                    ddl.Enabled = false;
                }
                else
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier", ex.Message.ToString());
            }
        }

        protected string ShowNmbrs(object item1, object item2)
        {
            string nubr = ((!String.IsNullOrEmpty(item1.ToString())) && (!String.IsNullOrEmpty(item2.ToString()))) ?
                (item1.ToString() + "/" + item2.ToString()) :
                (((!String.IsNullOrEmpty(item1.ToString())) || (!String.IsNullOrEmpty(item2.ToString()))) ?
                ((!String.IsNullOrEmpty(item1.ToString())) ? item1.ToString() : item2.ToString()) : "");

            return nubr;
        }

        #endregion

        #region GridView Row Data Bound and RowCommnd for Modify/Delete Supplier
        /// <summary>
        /// GridView Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsupplier_RowDataBound(object sender, GridViewRowEventArgs e)
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

        /// <summary>
        /// Grid Veiw Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsupplier_PreRender(object sender, EventArgs e)
        {
            try
            {
                string x = gvsupplier.EditIndex.ToString();
                gvsupplier.UseAccessibleHeader = false;
                gvsupplier.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsupplier_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = gvsupplier.Rows[index];
                if (e.CommandName == "Modify")
                {
                    ClearInputs();
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblsuplrcd")).Text);
                    SetUpdateValues(suplrbl.SelectSuppliersForBind(CommonBLL.FlagModify, ID, Guid.Empty));
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    Guid ID = new Guid(((Label)gvrow.FindControl("lblsuplrcd")).Text);
                    res = suplrbl.DeleteSuppliersToDelete(CommonBLL.FlagDelete, ID);
                    BindGridData(suplrbl.SelectSuppliersForBind(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Deleted Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Supplier Master",
                            "Data Deleted successfully in Supplier Master.");
                        ClearInputs();
                    }
                }
                gvrow.FindControl("lblsuplrcd").Focus();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
            }
        }

        protected void SetUpdateValues(DataSet ModifyDt)
        {
            try
            {
                if (ModifyDt != null && ModifyDt.Tables.Count > 0 && ModifyDt.Tables[0].Rows.Count > 0)
                {
                    hdfdsuplrID.Value = ModifyDt.Tables[0].Rows[0]["ID"].ToString();
                    txtorgnm.Text = ModifyDt.Tables[0].Rows[0]["OrgName"].ToString();
                    txtbsnm.Text = ModifyDt.Tables[0].Rows[0]["BussName"].ToString();
                    txttpn.Text = ModifyDt.Tables[0].Rows[0]["Phone"].ToString();
                    txtmbl.Text = ModifyDt.Tables[0].Rows[0]["Mobile"].ToString();
                    txtpem.Text = ModifyDt.Tables[0].Rows[0]["PriEmail"].ToString();
                    txtsem.Text = ModifyDt.Tables[0].Rows[0]["SecEmail"].ToString();
                    txtfx.Text = ModifyDt.Tables[0].Rows[0]["Fax1"].ToString();

                    if (Session["UserType"].ToString() != "" && Session["UserType"].ToString() == "Customer")
                    {
                        ddlregion.Visible = true;
                        lblRegion.Visible = true;
                        ddlregion.SelectedValue = ddlregion.Items.FindByText(ModifyDt.Tables[0].Rows[0]["Region"].ToString()).Value;
                        //ddlctgry.SelectedValue = Guid.Empty.ToString();
                    }
                    ddlctgry.SelectedValue = ModifyDt.Tables[0].Rows[0]["CategoryID"].ToString();

                    CascadingDropDown1.SelectedValue = ModifyDt.Tables[0].Rows[0]["BillCountry"].ToString();
                    CascadingDropDown2.SelectedValue = ModifyDt.Tables[0].Rows[0]["BillState"].ToString();
                    CascadingDropDown3.SelectedValue = ModifyDt.Tables[0].Rows[0]["BillCity"].ToString();
                    txtblngstrt.Text = ModifyDt.Tables[0].Rows[0]["BillStreet"].ToString();
                    txtblngpb.Text = ModifyDt.Tables[0].Rows[0]["BillPin"].ToString();

                    CascadingDropDown4.SelectedValue = ModifyDt.Tables[0].Rows[0]["ShipCountry"].ToString() == Guid.Empty.ToString() ?
                        CascadingDropDown4.SelectedValue = "0" : ModifyDt.Tables[0].Rows[0]["ShipCountry"].ToString();
                    CascadingDropDown5.SelectedValue = ModifyDt.Tables[0].Rows[0]["ShipState"].ToString() == Guid.Empty.ToString() ?
                        CascadingDropDown5.SelectedValue = "0" : ModifyDt.Tables[0].Rows[0]["ShipState"].ToString();
                    CascadingDropDown6.SelectedValue = ModifyDt.Tables[0].Rows[0]["ShipCity"].ToString() == Guid.Empty.ToString() ?
                        CascadingDropDown6.SelectedValue = "0" : ModifyDt.Tables[0].Rows[0]["ShipCity"].ToString();
                    txtshpngstrt.Text = ModifyDt.Tables[0].Rows[0]["ShipStreet"].ToString();
                    txtshpngpb.Text = ModifyDt.Tables[0].Rows[0]["ShipPin"].ToString();
                    txtacntnbr.Text = ModifyDt.Tables[0].Rows[0]["AccountNo"].ToString();
                    txtbrncNm.Text = ModifyDt.Tables[0].Rows[0]["Branch"].ToString();
                    txtrtgscd.Text = ModifyDt.Tables[0].Rows[0]["RTGSCode"].ToString();

                    txtRange.Text = ModifyDt.Tables[0].Rows[0]["Range"].ToString();
                    txtRngAdrs.Text = ModifyDt.Tables[0].Rows[0]["RangeAddress"].ToString();
                    txtDivision.Text = ModifyDt.Tables[0].Rows[0]["Division"].ToString();
                    txtDvsnAdrs.Text = ModifyDt.Tables[0].Rows[0]["DivisionAddress"].ToString();
                    txtCommissionerate.Text = ModifyDt.Tables[0].Rows[0]["Commissionerate"].ToString();
                    txtCmsnAdrs.Text = ModifyDt.Tables[0].Rows[0]["CommissionerateAddress"].ToString();
                    txtGST.Text = ModifyDt.Tables[0].Rows[0]["GST"].ToString();
                    txtIecNo.Text = ModifyDt.Tables[0].Rows[0]["IECNo"].ToString();
                    if (ModifyDt.Tables[0].Rows[0]["ContactPerson"].ToString() != "")
                    {
                        string[] Contact = ModifyDt.Tables[0].Rows[0]["ContactPerson"].ToString().Split('.');
                        if (Contact.Length > 1)
                        {
                            ddlHonorific.SelectedValue = Contact[0] + ".";
                            txtCtPrsn.Text = Contact[1];
                        }
                        else
                            txtCtPrsn.Text = Contact[0];
                    }
                    if (ModifyDt.Tables[0].Rows[0]["BankID"].ToString() != "")
                        ddlBanks.SelectedValue = ModifyDt.Tables[0].Rows[0]["BankID"].ToString();

                    ddlCurrency.SelectedValue = ModifyDt.Tables[0].Rows[0]["Currency"].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Clear Input Fields

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {
                hdfdsuplrID.Value = txtorgnm.Text = txtbsnm.Text = txttpn.Text = txtmbl.Text = ""; ddlregion.SelectedIndex = -1;
                txtpem.Text = txtsem.Text = txtfx.Text = txtblngstrt.Text = txtblngpb.Text = "";
                hdfdsuplrID.Value = Guid.Empty.ToString();
                txtRange.Text = txtRngAdrs.Text = txtDivision.Text = txtDvsnAdrs.Text = txtCommissionerate.Text = txtCmsnAdrs.Text = "";
                ddlHonorific.SelectedIndex = ddlBanks.SelectedIndex = ddlCurrency.SelectedIndex = -1;
                CascadingDropDown1.SelectedValue = Guid.Empty.ToString();
                CascadingDropDown4.SelectedValue = Guid.Empty.ToString();
                txtshpngstrt.Text = txtshpngpb.Text = txtacntnbr.Text = txtbrncNm.Text = txtrtgscd.Text = txtIecNo.Text = txtGST.Text = txtCtPrsn.Text = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
            }
        }
        #endregion

        # region WebMethods

        /// <summary>
        /// This is used to check Supplier org Nm in use or not
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetSupplierOrgName(string OrgNm)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagCSelect, OrgNm, new Guid(Session["CompanyID"].ToString()));
        }

        /// <summary>
        /// This is used to check Supplier Business Nm is in use or not
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetBusinessName(string businessNm)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagGSelect, businessNm, new Guid(Session["CompanyID"].ToString()));
        }

        /// <summary>
        /// This is used to check Supplier Primary MailID is in use or not
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetPMailID(string businessNm)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagHSelect, businessNm, new Guid(Session["CompanyID"].ToString()));
        }
        # endregion
    }
}
