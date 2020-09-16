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
using System.Text;
using System.Configuration;
using System.Data.OleDb;

namespace VOMS_ERP.Masters
{
    public partial class SpareParts : System.Web.UI.Page
    {
        # region variables

        SparePartsBLL spbll = new SparePartsBLL();
        ItemCategoryBAL IcBal = new ItemCategoryBAL();
        ErrorLog ELog = new ErrorLog();

        #endregion

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
                MasterPageFile = "~/CustomerMaster.master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    #region Need 2 Change Later
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


                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    #endregion

                    if (!IsPostBack)
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(SpareParts));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        btnUpdate.Attributes.Add("OnClick", "javascript:return Myvalidations()");

                        btnUpdate.Visible = false;
                        ClearAll();
                        if (Request.QueryString["ID"] != null)
                        {
                            EditRecord();
                            divSpareParts.InnerHtml = GetData("checked");
                        }
                        else
                            divSpareParts.InnerHtml = GetData("");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item SubCategory", ex.Message.ToString());
            }
        }

        #region Methods

        private void EditRecord()
        {
            try
            {
                string ID = Request.QueryString["ID"];
                DataSet ds = spbll.GetData(CommonBLL.FlagESelect, new Guid(ID));
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    txtSparepartsCode.Text = ds.Tables[0].Rows[0]["Code"].ToString();
                    txtSparepartsCode.Enabled = Convert.ToInt32(ds.Tables[0].Rows[0]["InItemMaster"].ToString()) > 0 ? false : true;
                    txtSparePartsDesc.Text = ds.Tables[0].Rows[0]["Description"].ToString();
                    DataSet dsitms = new DataSet();
                    DataTable dt = ds.Tables[1].Copy();
                    dt.TableName = "ItemsTable";
                    dsitms.Tables.Add(dt);
                    Session["SpareParts"] = dsitms;
                    hfEditID.Value = ID;
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
            }
        }

        private string GetData(string CheckedHeader)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='tblItems' class='rounded-corner' style='font-size: small;'>");
                sb.Append("<thead align='left'>");
                string ChkBoxHeader = "<input " + CheckedHeader + " type='checkbox' id='ChkHead' onclick='CheckAll()' />";
                sb.Append("<th class='rounded-First'>" + ChkBoxHeader + "</th><th>SNo</th><th>Cat Code</th><th class='rounded-Last'>Cat Description</th>");
                //+ "<th>SubCat Code</th><th class='rounded-Last'>SubCat Description</th>");
                sb.Append("</thead><tbody>");

                DataSet ds = (DataSet)Session["SpareParts"];
                if (Session["SpareParts"] == null && ds == null)
                    ds = IcBal.GetSubCategory(CommonBLL.FlagZSelect, Guid.Empty, "00", "");
                Session["SpareParts"] = ds;

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("<tr>");
                        string SNO = (i + 1).ToString();
                        string Checked = Convert.ToBoolean(dt.Rows[i]["IsSparePart"].ToString()) == true ? "checked" : "";
                        sb.Append("<td><input type='checkbox'  " + Checked + "  id='ChkRow" + SNO + "' onclick=Check(this,'" + dt.Rows[i]["CatID"].ToString() + "') /></td>");
                        sb.Append("<td>" + SNO + "</td>");
                        sb.Append("<td><label id='lblcatCode" + SNO + "'  >" + string.Format("{0:00}", Convert.ToInt32(dt.Rows[i]["CatCode"].ToString())) + "</label></td>");
                        sb.Append("<td><label id='lblcatdesc" + SNO + "'  >" + dt.Rows[i]["CatDesc"].ToString() + "</label></td>");
                        //sb.Append("<td><label id='lblSubcatCode" + SNO + "'  >" + dt.Rows[i]["SubCatCode"].ToString() + "</label></td>");
                        //sb.Append("<td><label id='lblSubcatDesc" + SNO + "'  >" + dt.Rows[i]["SubCatDesc"].ToString() + "</label></td>");
                        sb.Append("</tr>");
                    }
                }
                else
                    sb.Append("<tr><td colspan='4' align='center'>No SubCategory is selected as SparePart(s)...!</td></tr>");
                sb.Append("</tbody>");
                sb.Append("<tfoot><th class='rounded-foot-left'></th><th colspan='2'></th><th class='rounded-foot-right'></th></tfoot>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
                return ex.Message;
            }
        }

        private void SaveRecord(bool IsUpdate)
        {
            try
            {
                int rslt = 1;
                DataTable dt = null;
                DataSet ds = (DataSet)Session["SpareParts"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].AsEnumerable().Where(x => Convert.ToBoolean(x.Field<string>("IsSparePart")) == true).Count() > 0)
                        dt = ds.Tables[0].AsEnumerable().Where(x => Convert.ToBoolean(x.Field<string>("IsSparePart")) == true).CopyToDataTable();
                }

                if (txtSparepartsCode.Text.Trim() != "" && txtSparePartsDesc.Text.Trim() != "" && dt != null && dt.Rows.Count > 0)
                {
                    Guid ID = Guid.Empty;

                    char Flag = CommonBLL.FlagINewInsert;
                    if (IsUpdate)
                    {
                        Flag = CommonBLL.FlagUpdate;
                        ID = new Guid(Request.QueryString["ID"]);
                    }
                    rslt = spbll.InsertUpdateDelete(Flag, ID, txtSparepartsCode.Text.Trim(),
                            txtSparePartsDesc.Text.Trim(), true, new Guid(Session["UserID"].ToString()), DateTime.Now, dt);
                    if (!IsUpdate)
                    {
                        if (rslt == 0)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Inserted Successfully.');", true);
                            Response.Redirect("spareparts.aspx");
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Inserting.');", true);
                    }
                    else
                    {
                        if (rslt == 0)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                            Response.Redirect("spareparts.aspx");
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Updating.');", true);
                    }
                }
                else
                {
                    if (txtSparepartsCode.Text.Trim() == "")
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Spare parts Code is required.');", true);
                    else if (txtSparepartsCode.Text.Trim() == "")
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Spare parts Description is required.');", true);
                    else if (dt != null && dt.Rows.Count > 0)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select atleast One Category.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
            }
        }

        private void ClearAll()
        {
            Session["SpareParts"] = null;
            txtSparepartsCode.Text = "";
            txtSparePartsDesc.Text = "";
        }

        #endregion

        #region Button Click

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveRecord(false);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SaveRecord(true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Session["SpareParts"] = null;
            Response.Redirect("SpareParts.aspx", false);
        }

        #endregion

        #region WebMethods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckAll(string IsChecked)
        {
            try
            {
                bool HadChecked = Convert.ToBoolean(IsChecked);
                DataSet ds = (DataSet)Session["SpareParts"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ds.Tables[0].Rows[i]["IsSparePart"] = HadChecked;
                    }
                    ds.Tables[0].AcceptChanges();
                }
                Session["SpareParts"] = ds;
                return GetData(HadChecked == true ? "checked" : "");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
                return GetData("");
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string Check(string CatID, string IsChecked)
        {
            try
            {
                bool HadChecked = Convert.ToBoolean(IsChecked);
                DataSet ds = (DataSet)Session["SpareParts"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dr = ds.Tables[0].Select("CatID = '" + CatID + "'");

                    if (dr != null && dr.Length > 0)
                    {
                        dr[0]["IsSparePart"] = HadChecked;
                    }
                    ds.Tables[0].AcceptChanges();
                }
                Session["SpareParts"] = ds;
                return GetData("");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
                return GetData("");
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string Delete(string ID, string CreatedBy)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    res = spbll.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), "", "", true, Guid.Empty, DateTime.Now, CommonBLL.EmptySparePartsDetails());
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                    {
                        result = "Error::Cannot Delete this Record, Sub Category is linked/ Error while Deleting " + ID;
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Category",
                           "Data Deleted successfully from Item Master.");
                        ClearAll();
                    }
                }
                else
                    result = "You do not have permissions to Delete.";

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
                return GetData("");
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SearchSparePartsCode(string Code, string EditID)
        {
            try
            {
                Guid ID = EditID == "" ? Guid.Empty : new Guid(EditID);
                CheckBLL cbll = new CheckBLL();
                return cbll.CheckNo(CommonBLL.FlagZSelect, Code, ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "SpareParts", ex.Message.ToString());
                return false;
            }
        }

        #endregion

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = FileName();
                DataSet ds = ReadExcelData();
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables.Contains("'SPARE PARTS CODES$'"))
                {
                    DataTable Spare = ds.Tables["'SPARE PARTS CODES$'"];
                    DataSet dsRtnItms = spbll.GetData_SparesBulk(new Guid(Session["UserID"].ToString()), Spare);
                    DataTable Itm = dsRtnItms.Tables[0];
                    divSpareParts.InnerHtml = GetData("");
                    string NotInserted = Itm.Rows[0][0].ToString().Trim(',');
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

        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }
    }
}