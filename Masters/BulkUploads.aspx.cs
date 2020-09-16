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
    public partial class BulkUploads : System.Web.UI.Page
    {
        # region variables
        int res;
        ItemCategoryBAL ItmCat = new ItemCategoryBAL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        SparePartsBLL spbll = new SparePartsBLL();
        static string Filename = "";
        #endregion

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
                MasterPageFile = "~/CustomerMaster.master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(BulkUploads));
                btnUpload.Attributes.Add("OnClick", "javascript:return Myvalidations()");
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //string filename = FileName1();
                string items = "";
                DataSet ds = ReadExcelData();
                DataSet dsRtnCtgry, dsSubCtgry = null;
                if (FileUpload1.HasFile)
                {
                    if (ddlScreenname.SelectedValue != "ItemSubSubCategory" && ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'01$'"))
                    {
                        DataTable Spare = ds.Tables["'01$'"].AsEnumerable().Where(a => !a[0].ToString().Equals("")).CopyToDataTable();
                        dsSubCtgry = ItmCat.SCBulkUpload(Spare, new Guid(Session["UserId"].ToString()));// Sub Category
                        dsRtnCtgry = ItmCat.CategoryBulkUpload(new Guid(Session["UserID"].ToString()), Spare);//Spares + category 

                        if (dsSubCtgry != null && dsSubCtgry.Tables[0].Rows.Count > 0)
                        {
                            string _Script_Message = dsSubCtgry.Tables[0].Rows[0][0].ToString() +
                                                     dsSubCtgry.Tables[0].Rows[0][1].ToString() +
                                                     dsSubCtgry.Tables[0].Rows[0][2].ToString();
                            if (_Script_Message.Trim().Length > 0)
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "New", "alert('" + _Script_Message + "');", true);
                        }
                        if (dsRtnCtgry != null && dsRtnCtgry.Tables.Count > 0 && dsRtnCtgry.Tables[0].Rows.Count > 0 && dsRtnCtgry.Tables[0].Rows[0][0].ToString() != "")
                        {
                            if (dsRtnCtgry.Tables[0].Columns.Contains("ErrorMessage"))
                                items = dsRtnCtgry.Tables[0].Rows[0]["ErrorMessage"].ToString().Replace("'","").Trim(',');
                            else
                                items = dsRtnCtgry.Tables[0].Rows[0][0].ToString().Trim(',');
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('" + items + " are not Inserted.');", true);
                            
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Uploaded Successfully.');", true);
                    }
                    else if (ddlScreenname.SelectedValue == "ItemSubSubCategory" && ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'00$'"))
                    {
                        DataTable Spare = ds.Tables["'00$'"].AsEnumerable().Where(a => !a[0].ToString().Equals("")).CopyToDataTable();
                        DataSet dsRtnItms = ItmCat.SSCBulkUpload(Spare, new Guid(Session["UserID"].ToString()));

                        if (dsRtnItms != null && dsRtnItms.Tables[0].Rows.Count > 0)
                        {
                            string _Script_Message = dsRtnItms.Tables[0].Rows[0][0].ToString() +
                                                     dsRtnItms.Tables[0].Rows[0][1].ToString() +
                                                     dsRtnItms.Tables[0].Rows[0][2].ToString() +
                                                     dsRtnItms.Tables[0].Rows[0][3].ToString();
                            if (_Script_Message.Trim().Length > 0)
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "New", "alert('" + _Script_Message + "');", true);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Uploaded Successfully.');", true);

                    }
                    else if (ddlScreenname.SelectedValue == "SpareParts" && ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'SPARE PARTS CODES$'"))
                    {
                        DataTable Spare = ds.Tables["'SPARE PARTS CODES$'"];
                        DataSet dsRtnItms = spbll.GetData_SparesBulk(new Guid(Session["UserID"].ToString()), Spare);

                        if (dsRtnItms != null && dsRtnItms.Tables.Count > 0 && dsRtnItms.Tables[0].Rows.Count > 0 && dsRtnItms.Tables[0].Rows[0][0].ToString() != "")
                        {
                            if (dsRtnItms.Tables[0].Columns.Contains("ErrorMessage"))
                                items = dsRtnItms.Tables[0].Rows[0]["ErrorMessage"].ToString().Trim(',');
                            else
                                items = dsRtnItms.Tables[0].Rows[0][0].ToString().Trim(',');
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('" + items + " are not Inserted.');", true);
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Uploaded Successfully.');", true);
                    }
                    else if (ddlScreenname.SelectedValue == "ItemMaster" && ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("sheet1$"))
                    {
                        DataTable IM = ds.Tables["sheet1$"];
                        IM.Columns.Remove("ItemCode");
                        IM.Columns.Remove("REMARKS");
                        ItemMasterBLL _IMB = new ItemMasterBLL();
                        DataSet _DS_IM = _IMB.ItemMasterBulkUpload("C", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), IM);
                        if (_DS_IM != null && _DS_IM.Tables.Count > 0)
                        {
                            try
                            {
                                string _Msg = string.Format(@"Incorrect Format: " + string.Join(@",\r\n", _DS_IM.Tables[0].AsEnumerable().Select(P => P.Field<String>("Newitemcode")).Distinct().ToList()) +
                                            " \\n\\rCodes Already Exists:-" +
                                            "\\n\\rCategory Codes are :" + string.Join(@",", _DS_IM.Tables[1].AsEnumerable().Select(P => P.Field<String>("CCode")).Distinct().ToList()) +
                                            "\\n\\rSub-Category Codes are :" + string.Join(@",\r\n", _DS_IM.Tables[2].AsEnumerable().Select(P => P.Field<String>("SCCode")).Distinct().ToList()) +
                                            "\\n\\rSub-SubCategory Codes are :" + string.Join(@",\r\n", _DS_IM.Tables[3].AsEnumerable().Select(P => P.Field<String>("SSCCode")).Distinct().ToList()) +
                                            "\\n\\rItem Master Codes are :" + string.Join(@",\r\n", _DS_IM.Tables[4].AsEnumerable().Select(P => P.Field<String>("Newitemcode")).Distinct().ToList()) + " ");
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + _Msg + "');", true);
                            }
                            catch (Exception Ex)
                            {
                                if (Ex.Message.Contains("does not belong to table"))
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + _DS_IM.Tables[_DS_IM.Tables.Count - 1].Rows[0]["ErrorMessage"].ToString().Replace("'", "") + "');", true);
                                else                             
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + Ex.Message.Replace("'", "") + "');", true);
                            }
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to Insert.');", true);
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select Excel File to Upload.');", true);
                ddlScreenname.ClearSelection();
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
                DataSet dsOutput = new DataSet();
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                    FilePath = MapPath("~/uploads/" + FileName);
                    FileUpload1.SaveAs(FilePath);

                    string sConnection = "";
                    if (FileName.ToUpper().Contains(".XLSX"))
                        sConnection = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0 Xml;IMEX=3\";";
                    else
                        sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";

                    OleDbConnection dbCon = new OleDbConnection(sConnection);
                    dbCon.Open();
                    DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSheetName.Rows.Count > 0)
                    {
                        string sSheetName = "";
                        int RowIndex = 0;
                        if (dtSheetName.Rows[0]["TABLE_NAME"].ToString().Contains("_FilterDatabase"))
                            RowIndex++;

                        if (ddlScreenname.SelectedValue == "ItemCategory")//|| ddlScreenname.SelectedValue == "ItemSubCategory")
                            sSheetName = dtSheetName.Rows[RowIndex + 1]["TABLE_NAME"].ToString();
                        else
                            sSheetName = dtSheetName.Rows[RowIndex]["TABLE_NAME"].ToString();

                        string sQuery = "Select * From [" + sSheetName + "]";
                        OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                        OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                        DataTable dtData = new DataTable();
                        dtData.TableName = sSheetName;
                        dbDa.Fill(dtData);
                        dsOutput.Tables.Add(dtData);
                    }
                    dbCon.Close();
                }
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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlScreenname.SelectedIndex = -1;
            FileUpload1.PostedFile.InputStream.Dispose();
        }
    }
}