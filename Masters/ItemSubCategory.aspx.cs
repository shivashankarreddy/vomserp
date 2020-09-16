using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using VOMS_ERP.Admin;
using System.Data;
using System.IO;
using Ajax;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace VOMS_ERP.Masters
{
    public partial class ItemSubCategory : System.Web.UI.Page
    {
        # region variables
        int res;
        ItemMasterBLL ItmMstr = new ItemMasterBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ItemCategoryBAL IcBal = new ItemCategoryBAL();
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
                Ajax.Utility.RegisterTypeForAjax(typeof(ItemSubCategory));
                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    #region Need 2 Change Later
                    //if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    //{
                    //    #region Add/Update Permission Code
                    //    //To Check User can have the Add/Update permissions
                    //    if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                    //    {
                    //        btnSave.Enabled = true;
                    //        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    //    }
                    //    else
                    //    {
                    //        btnSave.Enabled = false;
                    //        btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                    //    }
                    //    #endregion


                    //    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    //}
                    //else
                    //    Response.Redirect("../Masters/Home.aspx?NP=no", false); 
                    #endregion

                    if (!IsPostBack)
                        GetData();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "BindDataTable();", true);
            base.OnPreRender(e);
        }

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
                DataSet ds = IcBal.GetSubCategory(CommonBLL.FlagRegularDRP, Guid.Empty, "0", "");
                ds.Tables[0].Columns.Add("ID_Description", typeof(string), "id +'-'+ Description");
                DataTable DT = ds.Tables[0].Select(null, "ID_Description").CopyToDataTable();
                BindDropDownList(ddlItmCtgry, DT);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownList
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataTable CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "ID_Description";
                ddl.DataValueField = "CatID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
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
                //txtspec.Text = ItmMstrDt.Tables[0].Rows[0]["Specification"].ToString();
                //txtItmPrtNmbr.Text = ItmMstrDt.Tables[0].Rows[0]["PartNumber"].ToString();
                hdfdItmMstrID.Value = ItmMstrDt.Tables[0].Rows[0]["ID"].ToString();
                //ChkHadSubItems.Checked = Convert.ToBoolean(ItmMstrDt.Tables[0].Rows[0]["IsSubItems"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                txtItmDscrip.Text = hdfdItmMstrID.Value = "";//txtItmPrtNmbr.Text = txtspec.Text = 
                ddlItmCtgry.SelectedIndex = -1;
                txtSubCategoryCode.Text = "";
                txtItmDscrip.Text = "";
                HF_EditID.Value = "";
                hfSubCatEdit.Value = "";
                btnSave.Text = "Save";
                ChkIsSpareParts.Checked = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
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
                    int result = 0;
                    result = IcBal.InsertUpdateDelte_SubCategory(CommonBLL.FlagNewInsert, Guid.Empty, txtSubCategoryCode.Text.Trim(),
                        txtItmDscrip.Text, new Guid(ddlItmCtgry.SelectedValue), ddlItmCtgry.SelectedItem.Text.Split('-')[0], new Guid(Session["UserId"].ToString()),
                        DateTime.Now, new Guid(Session["UserId"].ToString()), DateTime.Now, true, ChkIsSpareParts.Checked);
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Item SubCategory", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item SubCategory", "Data Inserted successfully in Item Master.");
                        ClearAll();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Filename = FileName();
            int result = 0;
            result = IcBal.InsertUpdateDelte_SubCategory(CommonBLL.FlagUpdate, new Guid(HF_EditID.Value), txtSubCategoryCode.Text.Trim(),
                        txtItmDscrip.Text, new Guid(ddlItmCtgry.SelectedValue), ddlItmCtgry.SelectedValue, new Guid(Session["UserId"].ToString()),
                        DateTime.Now, new Guid(Session["UserId"].ToString()), DateTime.Now, true, ChkIsSpareParts.Checked);
            if (result == 0)
            {
                ALS.AuditLog(res, btnSave.Text, "", "Item SubCategory", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "SuccessMessage('Updated Successfully');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item SubCategory", "Data Updated successfully in Item Master.");
                ClearAll();
            }
        }

        #endregion

        # region WEbMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckSubCategory(string value, string catgry)
        {
            bool RtnVsal = false;
            try
            {
                //IcBal = new ItemCategoryBAL();
                DataSet ds = IcBal.GetSubCategory(CommonBLL.FlagCSelect, Guid.Empty, value, catgry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)// && ds.Tables[0].Rows[0]["Code"].ToString().ToLower() == value.ToLower())
                {
                    RtnVsal = false;
                }
                else
                    RtnVsal = true;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
            return RtnVsal;
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID)
        {
            try
            {
                int res = 1;
                string result = "Error::Cannot Delete this Record";
                //string result = CommonBLL.Can_EditDelete(false, "");

                //#region Delete
                //if (result == "Success")
                //{

                //    res = ItmMstr.DeleteItemMaster(CommonBLL.FlagDelete, new Guid(ID));
                //    if (res == 0)
                //        result = "Success::Deleted Successfully";
                //    else
                //    {
                //        result = "Error::Cannot Delete this Record, LE already created so delete LE/ Error while Deleting " + ID;
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item SubCategory",
                //           "Data Deleted successfully from Item SubCategory.");
                //        ClearAll();
                //    }
                //}
                //#endregion

                //return result;
                res = IcBal.InsertUpdateDelte_SubCategory(CommonBLL.FlagDelete, new Guid(ID), "", "", Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, false);
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                {
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item SubCategory", "Data Deleted successfully from Item SubCategory.");
                    ClearAll();
                }
                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
                return ErrMsg;
            }
        }

        # endregion

        protected void BTN_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = FileUpload_SubCat.FileName;
                string _OCON_Str;
                string FilePath = Server.MapPath(@"~/Masters/") + FileName;
                FileUpload_SubCat.PostedFile.SaveAs(FilePath);

                if (FileName.ToUpper().Contains(".XLSX"))
                    _OCON_Str = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0 Xml;IMEX=3\";";
                else
                    _OCON_Str = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + FilePath + ";Extended Properties=Excel 8.0;";

                OleDbConnection _OLEDB = new OleDbConnection();
                _OLEDB.ConnectionString = _OCON_Str;
                _OLEDB.Open();
                OleDbCommand _OCMD = new OleDbCommand();
                _OCMD.Connection = _OLEDB;
                _OCMD.CommandText = "SELECT * FROM [01$]";
                OleDbDataAdapter _OLEDA = new OleDbDataAdapter();
                _OLEDA.SelectCommand = _OCMD;
                DataTable ExcelDataHeader = new DataTable();
                _OLEDA.Fill(ExcelDataHeader);
                _OLEDB.Close();
                File.Delete(FilePath);
                if (ExcelDataHeader.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).Count() > 0)
                    ExcelDataHeader = ExcelDataHeader.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                DataSet DS = IcBal.SCBulkUpload(ExcelDataHeader, new Guid(Session["UserId"].ToString()));

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }
    }
}