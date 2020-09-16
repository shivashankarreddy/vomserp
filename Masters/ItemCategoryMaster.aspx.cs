using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using VOMS_ERP.Admin;
using Ajax;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.OleDb;

namespace VOMS_ERP.Masters
{
    public partial class ItemCategoryMaster : System.Web.UI.Page
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
                Ajax.Utility.RegisterTypeForAjax(typeof(ItemCategoryMaster));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!IsPostBack)
                    {
                        // if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        // {
                        #region Add/Update Permission Code
                        //To Check User can have the Add/Update permissions
                        //  if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                        // {
                        btnSave.Enabled = true;
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        btnUpdate.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        // }
                        // else
                        //  {
                        //    btnSave.Enabled = false;
                        // btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        //  }
                        #endregion
                        //Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster_Revised));                        
                        //btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        //}
                        // else
                        //    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category", ex.Message.ToString());
            }
        }

        #endregion

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
                txtitemCatCode.Text = txtItemCatDesc.Text = ""; ChkIsSpareParts.Checked = false;
                hdfdItmCategoryID.Value = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category", ex.Message.ToString());
            }
        }

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
                if (txtitemCatCode.Text.Trim() != "" && txtItemCatDesc.Text.Trim() != "")
                {
                    Filename = FileName();

                    if (btnSave.Text == "Save")
                    {
                        int result = ItmCat.InsertUpdateDelte_Category(CommonBLL.FlagNewInsert, Guid.NewGuid(),
                            txtitemCatCode.Text, txtItemCatDesc.Text.Trim(), new Guid(Session["UserID"].ToString()),
                            DateTime.Now, Guid.Empty, DateTime.MaxValue, true, ChkIsSpareParts.Checked);
                        if (result == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Item Category", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Category", "Data Inserted successfully in Item Category.");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category", ex.Message.ToString());
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtitemCatCode.Text.Trim() != "" && txtItemCatDesc.Text.Trim() != "")
                {
                    Filename = FileName();


                    int result = ItmCat.InsertUpdateDelte_Category(CommonBLL.FlagUpdate, new Guid(hdfdItmCategoryID.Value),
                        txtitemCatCode.Text, txtItemCatDesc.Text, new Guid(Session["UserID"].ToString()),
                        DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, ChkIsSpareParts.Checked);
                    if (result == 0)
                    {
                        ALS.AuditLog(res, btnSave.Text, "", "Item Category", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Updated Successfully');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Category", "Data Updated successfully in Item Master.");
                        ClearAll();
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill all Mandatory details.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category", ex.Message.ToString());
            }
        }

        #endregion

        # region WEbMethods        

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = "";
                //string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                //if (result == "Success")
                //{

                res = ItmCat.InsertUpdateDelte_Category(CommonBLL.FlagDelete, new Guid(ID), "", "", Guid.Empty, DateTime.MaxValue, Guid.Empty, DateTime.MaxValue, true, false);
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                {
                    result = "Error::Cannot Delete this Record, Sub Category is linked/ Error while Deleting ";// + ID;
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Category",
                       "Data Deleted successfully from Item Master.");
                    ClearAll();
                }
                // }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Item Category", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Item category", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckCode(string Code)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('W', Code.Trim(), new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckUnCheck(string EditID, string IsChecked)
        {
            try
            {
                CheckBLL cbll = new CheckBLL();
                DataSet ds = ItmCat.GetCategory(CommonBLL.FlagCSelect, new Guid(EditID), IsChecked);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 &&
                                  ds.Tables[0].Rows[0][0].ToString() != "" && Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Item category", ex.Message.ToString());
                return false;
            }
        }

        # endregion

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = FileName1();
                DataSet ds = ReadExcelData();
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'01$'"))
                {
                    DataTable Spare = ds.Tables["'01$'"].AsEnumerable().Where(a => !a[0].ToString().Equals("")).CopyToDataTable();
                    DataSet dsRtnItms = ItmCat.CategoryBulkUpload(new Guid(Session["UserID"].ToString()), Spare);
                    //DataTable Itm = dsRtnItms.Tables[0];
                    //string NotInserted = Itm.Rows[0][0].ToString().Trim(',');
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
                    string sSheetName = dtSheetName.Rows[1]["TABLE_NAME"].ToString();
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

        private string FileName1()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }
    }
}