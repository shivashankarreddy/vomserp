using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using VOMS_ERP.Admin;
using Ajax;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.OleDb;

namespace VOMS_ERP.Masters
{
    public partial class ItemSubSubCategory : System.Web.UI.Page
    {
        # region variables

        int res;
        ItemCategoryBAL ItmCat = new ItemCategoryBAL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";

        #endregion

        #region Default Page Load

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ItemSubSubCategory));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!IsPostBack)
                    {
                        //if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        // {
                        #region Add/Update Permission Code
                        //To Check User can have the Add/Update permissions
                        //if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                        // {
                        btnSave.Enabled = true;
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        // }
                        //else
                        //{
                        // btnSave.Enabled = false;
                        //  btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        //  }
                        #endregion

                        GetData();
                        //Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster_Revised));                        
                        //btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        //}
                        // else
                        //     Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }

        }

        #endregion

        /// <summary>
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                ///To Bind Category in DDL
                DataSet ds = ItmCat.GetSubCategory(CommonBLL.FlagVSelect, Guid.Empty, "0", "");
                DataTable DT = ds.Tables[0].Select(null, "Description").CopyToDataTable();                
                BindDropDownList(ddlItmCtgry, DT);
                if (Request.QueryString["ID"] != null)
                {
                    Guid ID = new Guid(Request.QueryString["ID"]);
                    DataSet Edit = ItmCat.GetSubSubCategory(CommonBLL.FlagModify, ID, "00", Guid.Empty, Guid.Empty);
                    if (Edit != null && Edit.Tables[0].Rows.Count > 0)
                    {
                        ddlItmCtgry.SelectedValue = Edit.Tables[0].Rows[0]["RefCatId"].ToString();
                        ItemCategoryChange();
                        ddlItmSubCatCode.SelectedValue = Edit.Tables[0].Rows[0]["RefSubCatId"].ToString();
                        txtItmSubSubCatCode.Text = Edit.Tables[0].Rows[0]["Code"].ToString();
                        //txtItmSubSubCatCode.Enabled = Edit.Tables[0].Rows[0]["InItemMaster"].ToString() == "0" ? true : false;
                        txtItmSubSubCatDesc.Text = Edit.Tables[0].Rows[0]["Description"].ToString();
                        hfEditID.Value = Edit.Tables[0].Rows[0]["Id"].ToString();
                        btnSave.Text = "Update";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
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
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }
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
        /// Clear all input fields
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                txtItmSubSubCatCode.Text = txtItmSubSubCatDesc.Text = ""; // txtItemCatCode.Text = txtItmSubCatCode.Text =
                ddlItmCtgry.Items.Clear();
                ddlItmSubCatCode.Items.Clear();
                btnSave.Text = "Save";
                txtItmSubSubCatCode.Enabled = false;
                Response.Redirect("ItemSubSubCategory.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }
        }

        protected void ItemCategoryChange()
        {
            try
            {
                if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
                {
                    DataSet ds = ItmCat.GetSubCategory(CommonBLL.FlagPSelectAll, new Guid(ddlItmCtgry.SelectedValue), "0", "");
                    DataTable DT = ds.Tables[0].Select(null, "Description").CopyToDataTable();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        BindDropDownList(ddlItmSubCatCode, DT);
                        //txtItmSubSubCatCode.Enabled = true;
                    }
                }
                else
                {
                    ddlItmSubCatCode.Items.Clear();
                    txtItmSubSubCatCode.Text = txtItmSubSubCatDesc.Text = "";
                    txtItmSubSubCatCode.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }

        }

        protected void ItemSubCategoryChange()
        {
            try
            {
                if (ddlItmSubCatCode.SelectedValue != Guid.Empty.ToString())
                {
                    txtItmSubSubCatCode.Enabled = true;
                    //txtItmSubSubCatCode.Enabled = true;
                }
                else
                {
                    txtItmSubSubCatCode.Enabled = false;
                    //txtItmSubSubCatCode.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }

        }

        #region Selected Index Change Event

        protected void ddlItmCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ItemCategoryChange();
                //if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
                //{
                //    DataSet ds = ItmCat.GetSubCategory(CommonBLL.FlagPSelectAll, new Guid(ddlItmCtgry.SelectedValue), 0, "");
                //    if (ds != null && ds.Tables[0].Rows.Count > 0)
                //    {
                //        BindDropDownList(ddlItmSubCatCode, ds);
                //    }
                //}
                //else
                //{
                //    ddlItmSubCatCode.Items.Clear();
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }
        }

        protected void ddlItmSubCatCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemSubCategoryChange();
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
                if (txtItmSubSubCatCode.Text.Trim() != "" && txtItmSubSubCatDesc.Text.Trim() != "" && ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
                {
                    Filename = FileName();

                    if (btnSave.Text == "Save")
                    {
                        int result = ItmCat.InsertUpdateDelte_SubSubCategory(CommonBLL.FlagNewInsert, Guid.NewGuid(), txtItmSubSubCatCode.Text, txtItmSubSubCatDesc.Text, new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatCode.SelectedValue), new Guid(Session["UserID"].ToString()),
                            DateTime.Now, Guid.Empty, DateTime.MaxValue, true);
                        if (result == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Item Sub Sub Category", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Sub Sub Category", "Data Inserted successfully in Item Category.");
                            ClearAll();
                        }
                    }
                    if (btnSave.Text == "Update")
                    {
                        int result = ItmCat.InsertUpdateDelte_SubSubCategory(CommonBLL.FlagUpdate, new Guid(hfEditID.Value),txtItmSubSubCatCode.Text,
                            txtItmSubSubCatDesc.Text, new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatCode.SelectedValue), new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                        if (result == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Item Sub Sub Category", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Sub Sub Category", "Data Updated successfully in Item Master.");
                            ClearAll();
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill all Mandatory details.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub Category", ex.Message.ToString());
            }
        }

        //protected void btnUpdate_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtItmSubSubCatCode.Text.Trim() != "" && txtItmSubSubCatDesc.Text.Trim() != "" && ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
        //        {
        //            Filename = FileName();


        //            int result = ItmCat.InsertUpdateDelte_SubSubCategory(CommonBLL.FlagUpdate, new Guid(hfEditID.Value), Convert.ToInt64(txtItmSubSubCatCode.Text),
        //                txtItmSubSubCatDesc.Text, new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatCode.SelectedValue), new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true);
        //            if (result == 0)
        //            {
        //                ALS.AuditLog(res, btnSave.Text, "", "Item Sub Sub Category", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
        //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                    "SuccessMessage('Updated Successfully');", true);
        //                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Sub Sub Category", "Data Updated successfully in Item Master.");
        //                ClearAll();
        //            }
        //        }
        //        else
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill all Mandatory details.');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Sub Sub category", ex.Message.ToString());
        //    }
        //}

        #endregion

        # region WEbMethods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID)
        {
            try
            {
                int res = 1;
                string result = "Error::Cannot Delete this Record";

                #region Delete

                res = ItmCat.InsertUpdateDelte_SubSubCategory(CommonBLL.FlagDelete, new Guid(ID), "", "", Guid.Empty, Guid.Empty, Guid.Empty, DateTime.MaxValue, Guid.Empty, DateTime.MaxValue, true);
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                {
                    result = "Error::Cannot Delete this Record ";// + ID;
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master",
                       "Data Deleted successfully from Item Sub Sub Category.");
                    ClearAll();
                }

                #endregion
                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Master/ErrorLog"), "Item Master", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Master/ErrorLog"), "Item Master", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckCode(string Code, string CatID, string SubCatID)
        {
            try
            {
                DataSet ds = new DataSet();
                string res = "";
                if (Code != "")
                {
                    ds = ItmCat.GetSubSubCategory(CommonBLL.FlagZSelect, Guid.Empty, Code, new Guid(CatID), new Guid(SubCatID));
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        res = "false";
                    else
                        res = "true";
                }
                return res;
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

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = FileNamee();
                DataSet ds = ReadExcelData();
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'00$'"))
                {
                    DataTable Spare = ds.Tables["'00$'"];
                    DataSet dsRtnItms = ItmCat.SSCBulkUpload(Spare, new Guid(Session["UserID"].ToString()));
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry Bulk Upload", ex.Message.ToString());
            }
        }

        public DataSet ReadExcelData()
        {
            try
            {
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                    FilePath = MapPath("~/uploads/" + FileName);
                    FileUpload1.SaveAs(FilePath);
                }
                string sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                OleDbConnection dbCon = new OleDbConnection(sConnection);
                dbCon.Open();
                DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataSet dsOutput = new DataSet();
                if (dtSheetName.Rows.Count > 0)
                {
                    int nCount = 0;
                    string sSheetName = dtSheetName.Rows[nCount]["TABLE_NAME"].ToString();
                    string sQuery = "Select * From [" + sSheetName + "]";
                    OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                    OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                    DataTable dtData = new DataTable();
                    dtData.TableName = sSheetName;
                    dbDa.Fill(dtData);
                    dsOutput.Tables.Add(dtData);
                }
                dbCon.Close();
                return dsOutput;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return null;
            }
        }

        private string FileNamee()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }
    }
}