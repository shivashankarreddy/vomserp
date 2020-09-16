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
using System.Data.SqlClient;
using Ajax;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Masters
{
    public partial class ItemMaster : System.Web.UI.Page
    {
        # region variables
        int res;
        ItemMasterBLL ItmMstr = new ItemMasterBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster));
                        if (!IsPostBack)
                            GetData();
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
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
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                BindDropDownList(ddlItmCtgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind GridView
        /// </summary>
        /// <param name="ItmMstDt"></param>
        protected void BindGridData(DataSet ItmMstDt)
        {
            try
            {
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownList
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
                ddl.SelectedValue = (ddl.Items.FindByText("General")).Value;
                ddl.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Set Values to inputs for Update
        /// </summary>
        /// <param name="ItmMstrDt"></param>
        protected void SetUpdateValues(DataSet ItmMstrDt)
        {
            try
            {
                ddlItmCtgry.SelectedValue = ItmMstrDt.Tables[0].Rows[0]["CategoryID"].ToString();
                txtItmDscrip.Text = ItmMstrDt.Tables[0].Rows[0]["ItemDescription"].ToString();
                txtspec.Text = ItmMstrDt.Tables[0].Rows[0]["Specification"].ToString();
                txtItmPrtNmbr.Text = ItmMstrDt.Tables[0].Rows[0]["PartNumber"].ToString();
                hdfdItmMstrID.Value = ItmMstrDt.Tables[0].Rows[0]["ID"].ToString();
                ChkHadSubItems.Checked = Convert.ToBoolean(ItmMstrDt.Tables[0].Rows[0]["IsSubItems"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                ddlItmCtgry.SelectedValue = (ddlItmCtgry.Items.FindByText("General")).Value;
                txtItmDscrip.Text = txtItmPrtNmbr.Text = txtspec.Text = hdfdItmMstrID.Value = "";
                btnSave.Text = "Save";
                ChkHadSubItems.Checked = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events (Insert Data into Item Master Table)

        /// <summary>
        /// Save and Updte Button Click Events
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
                    string itemCode = "";
                    if (txtItmDscrip.Text.Trim().Length > 8)
                        itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()).Substring(0, 8));
                    else
                        itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()));
                    int result = ItmMstr.InsertUpdateItemMaster(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlItmCtgry.SelectedValue),"", 
                        txtItmDscrip.Text, txtspec.Text, txtItmPrtNmbr.Text, itemCode, new Guid(Session["UserID"].ToString()), 
                        txtHSCode.Text, ChkHadSubItems.Checked, new Guid(Session["CompanyID"].ToString()));
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Item Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master", "Data Inserted successfully in Item Master.");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
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
                ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Filename = FileName();
            int result = ItmMstr.InsertUpdateItemMaster(CommonBLL.FlagUpdate, new Guid(hdfdItmMstrID.Value), new Guid(ddlItmCtgry.SelectedValue),"",
                txtItmDscrip.Text, txtspec.Text, txtItmPrtNmbr.Text, "", new Guid(Session["UserID"].ToString()), txtHSCode.Text,
                ChkHadSubItems.Checked, new Guid(Session["CompanyID"].ToString()));
            if (result == 0)
            {
                ALS.AuditLog(res, btnSave.Text, "", "Item Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "SuccessMessage('Updated Successfully');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master", "Data Updated successfully in Item Master.");
                ClearAll();
            }
        }

        #endregion

        # region WEbMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckItemsMaster(string ItemDesc, string PartNumber, string ItemSpec, string EID, string IsSubItem)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckItemsMaster('X', ItemDesc, PartNumber, ItemSpec, new Guid(EID), IsSubItem,new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {

                    res = ItmMstr.DeleteItemMaster(CommonBLL.FlagDelete, new Guid(ID));
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                    {
                        result = "Error::Cannot Delete this Record, LE already created so delete LE/ Error while Deleting " + ID;
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master",
                           "Data Deleted successfully from Item Master.");
                        ClearAll();
                    }
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        # endregion


    }
}
