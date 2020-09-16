using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BAL;
using System.Data;
using VOMS_ERP.Admin;
using Ajax;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;

namespace VOMS_ERP.Masters
{
    public partial class ItemMaster_Revised : System.Web.UI.Page
    {
        # region variables
        int res;
        ItemMasterBLL ItmMstr = new ItemMasterBLL();
        EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        ItemCategoryBAL IcBal = new ItemCategoryBAL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string new_ItmCode = "";
        static string ItmCodetxt = "";
        static string OldItemDesc = "";
        static string OldPartNum = "";
        static string OldSpecification = "";
        bool oldCheckbox = false;
        #endregion

        #region Default Page Load

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster_Revised));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!IsPostBack)
                    {
                        if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        {
                            if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
                            {
                                //PnlImp.Visible = true;
                            }

                            #region Add/Update Permission Code
                            //To Check User can have the Add/Update permissions
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Enabled = true;
                                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                                btnUpdate.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            }
                            else
                            {
                                btnSave.Enabled = false;
                                btnSave.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                            }
                            #endregion
                            //Ajax.Utility.RegisterTypeForAjax(typeof(ItemMaster_Revised));                        
                            //btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                            Response.Redirect("../Masters/Home.aspx?NP=no", false);
                        GetData();
                        //ddlItmCtgry.Enabled = ddlItmSubCatgry.Enabled = ddlItmSubSubCatgry.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "BindDataTable();", true);
            base.OnPreRender(e);
        }

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
                BindDropDownListForCatgry(ddlItmCtgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                {
                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                }
                else
                {
                    btnSave.Visible = true;
                    btnUpdate.Visible = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        private void EditRecord(Guid ID)
        {
            try
            {
                DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ddlItmCtgry.SelectedValue = ds.Tables[0].Rows[0]["CatRefID"].ToString();
                    DDLItmCatChange();
                    ddlItmSubCatgry.SelectedValue = ds.Tables[0].Rows[0]["SubCatRefID"].ToString();
                    DDLItmSubCatChange();
                    if (ds.Tables[0].Rows[0]["SubSubCatRegID"].ToString() != "")
                    {
                        ddlItmSubSubCatgry.SelectedValue = ds.Tables[0].Rows[0]["SubSubCatRegID"].ToString();
                        DDLItmSubSubCatChange();
                    }
                    else if (ds.Tables[0].Rows[0]["SparesID"].ToString() != "")
                    {
                        ddlItmSubSubCatgry.SelectedValue = ds.Tables[0].Rows[0]["SparesID"].ToString();
                        DDLItmSubSubCatChange();
                    }
                    string aa = lblCode.Text + "" + lblSCode.Text + "" + lblSSCode.Text;
                    txtItmCode.Text = ds.Tables[0].Rows[0]["New_ItemCode"].ToString().Replace(aa, "");
                    ItmCodetxt = txtItmCode.Text;
                    txtItmDscrip.Text = ds.Tables[0].Rows[0]["ItemDescription"].ToString();
                    OldItemDesc = txtItmDscrip.Text;
                    txtItmPrtNmbr.Text = ds.Tables[0].Rows[0]["PartNumber"].ToString();
                    OldPartNum = txtItmPrtNmbr.Text;
                    txtspec.Text = ds.Tables[0].Rows[0]["Specification"].ToString();
                    OldSpecification = txtspec.Text;
                    ChkHadSubItems.Checked = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsSubItems"].ToString());
                    oldCheckbox = ChkHadSubItems.Checked;
                    hfEditItemCode.Value = GetNewItm_Code();
                    hdfdItmMstrID.Value = ID.ToString();
                    HF_EditID.Value = ID.ToString();
                }

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
                ddl.Items.Insert(0, new ListItem("--Select--", Guid.Empty.ToString()));
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
        protected void BindDropDownListForCatgry(DropDownList ddl, DataSet CommonDt)
        {
            try
            {

                txtItmCode.Enabled = false;
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "Id";
                ddl.DataBind();
                //ddl.Items.Insert(0, new ListItem(CommonDt.Tables[0].Rows[0]["Description"].ToString(), CommonDt.Tables[0].Rows[0]["ID"].ToString()));
                //DDLItmCatChange();
                //DDLItmSubCatChange();
                //DDLItmSubSubCatChange();
                //ddlItmCtgry.Enabled = ddlItmSubCatgry.Enabled = ddlItmSubSubCatgry.Enabled = false;
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
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
                //ddlItmCtgry.SelectedValue = Guid.Empty.ToString();
                //txtItmCode.Text = "";
                //txtItmDscrip.Text = txtItmPrtNmbr.Text = txtspec.Text = hdfdItmMstrID.Value = "";
                //btnSave.Text = "Save";
                //ChkHadSubItems.Checked = false;
                Response.Redirect("ItemMaster_Revised.aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        private DataSet ReadExcelData()
        {
            try
            {
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

                    FilePath = MapPath("~/uploads/" + FileName); //Server.MapPath(FolderPath + FileName);
                    FileUpload1.SaveAs(FilePath);
                }
                string sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                OleDbConnection dbCon = new OleDbConnection(sConnection);
                dbCon.Open();
                DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataTable dataTable = new DataTable();
                DataSet dsOutput = new DataSet();
                //for (int nCount = 0; nCount < dtSheetName.Rows.Count; nCount++)
                if (dtSheetName.Rows.Count > 0)
                {
                    int nCount = 0;
                    string sSheetName = dtSheetName.Rows[nCount]["TABLE_NAME"].ToString();
                    string sQuery = "Select * From [" + sSheetName + "] ";
                    OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                    OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                    DataTable dtData = new DataTable();

                    //dtData.TableName = sSheetName;WHERE NOT ([ItemCode] IS NULL OR [ItemDescription] IS NULL)
                    //DataTable newDt = dtData.Copy();
                    //newDt = (DataTable)dtData.Rows.Cast<DataRow>().Where(row => row.ItemArray.Any(field => !(field is System.DBNull)));
                    dbDa.Fill(dtData);
                    //dataTable = dtData.Copy();
                    //dataTable = dtData.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string).Trim(), string.Empty) == 0)).CopyToDataTable();
                    //dataTable = (DataTable)dataTable.Rows.Cast<DataRow>().Where(row => row.ItemArray.Any(field => !(field is System.DBNull)));
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

        private string GetNewItm_Code()
        {
            string ItmCode = "";
            try
            {
                if (ddlItmCtgry.SelectedValue != "" && ddlItmSubCatgry.SelectedValue != "" && ddlItmSubSubCatgry.SelectedValue != "" && txtItmCode.Text != "")
                {
                    ItmCode = lblCode.Text + lblSCode.Text + lblSSCode.Text + txtItmCode.Text;
                }
                else if (ddlItmCtgry.SelectedValue != "" && ddlItmSubCatgry.SelectedValue != "" && ddlItmSubSubCatgry.SelectedValue == "" && txtItmCode.Text.Length == 7)
                {
                    ItmCode = lblCode.Text + lblSCode.Text + txtItmCode.Text;
                }
                //else if (btnSave.Visible == false && btnUpdate.Text == "Update")
                //    ItmCode = txtItmCode.Text;
                return ItmCode;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
                return "";
            }

        }

        private void DDLItmCatChange()
        {
            try
            {

                lblCode.Text = "";
                lblSCode.Text = "";
                lblSSCode.Text = "";
                if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
                {
                    lblCode.Text = ddlItmCtgry.SelectedItem.Text.Split('-')[0];
                    ddlItmSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Items.Clear();
                    ddlItmSubCatgry.Enabled = true;
                    DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagPSelectAll, Guid.Empty, new Guid(ddlItmCtgry.SelectedValue), Guid.Empty);
                    DataTable DT = ds.Tables[0].Select(null, "SubCatDescription").CopyToDataTable();
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        ddlItmSubCatgry.DataSource = DT;
                        ddlItmSubCatgry.DataTextField = "SubCatDescription";
                        ddlItmSubCatgry.DataValueField = "SubCatID";
                        ddlItmSubCatgry.DataBind();
                        //ddlItmSubCatgry.Items.Insert(0, new ListItem(ds.Tables[0].Rows[0]["SubCatDescription"].ToString(), ds.Tables[0].Rows[0]["SubCatID"].ToString()));
                        ddlItmSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                    }
                    else
                    {
                        lblSCode.Text = "";
                        ddlItmSubCatgry.Enabled = false;
                        ddlItmSubCatgry.Items.Clear();
                        ddlItmSubSubCatgry.Items.Clear();
                        txtItmCode.Text = "";
                    }
                }
                else
                {
                    ddlItmSubCatgry.Enabled = false;
                    ddlItmSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Items.Clear();
                    txtItmCode.Text = "";
                }
                //ddlItmSubCatgry.Items.Insert(0, new ListItem("--Select Item Sub Category--", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        private void DDLItmSubCatChange()
        {
            try
            {
                DataTable DT = new DataTable(); DataTable dtaa = new DataTable();
                HFItemCatCodeIsSpare.Value = "False";
                HFItemSubCatCodeIsSpare.Value = "False";
                ddlItmSubSubCatgry.Items.Clear();
                ddlItmSubSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString() && ddlItmSubCatgry.SelectedValue != Guid.Empty.ToString())
                {
                    lblSCode.Text = ddlItmSubCatgry.SelectedItem.Text.Split('-')[0];
                    txtItmCode.MaxLength = 7;

                   
                    //ddlItmSubSubCatgry.Enabled = true;
                    DataSet dssCatg = IcBal.GetSubCategory(CommonBLL.FlagRegularDRP, Guid.Empty, "0", "");
                    DataSet dssSubCatg = IcBal.GetSubCategory(CommonBLL.FlagESelect, new Guid(ddlItmSubCatgry.SelectedValue), "0", "");
                    int cnt = dssCatg.Tables[0].Select("CatID <> '" + ddlItmCtgry.SelectedValue + "'").Count();
                    if (cnt > 0)
                        dtaa = dssCatg.Tables[0].Select("CatID <> '" + ddlItmCtgry.SelectedValue + "'").CopyToDataTable();
                    if (Convert.ToBoolean(dssSubCatg.Tables[0].Rows[0]["IsSpareParts"].ToString()) || Convert.ToBoolean(dtaa.Rows[0]["IsSpareParts"].ToString()))
                    {
                        lblSSCode.Text = "";
                        ddlItmSubSubCatgry.Enabled = false;
                        //ddlItmSubSubCatgry.Items.Clear();
                        txtItmCode.Text = "";
                        txtItmCode.Enabled = true;
                        HFItemCatCodeIsSpare.Value = dtaa.Rows[0]["IsSpareParts"].ToString();
                        HFItemSubCatCodeIsSpare.Value = dssSubCatg.Tables[0].Rows[0]["IsSpareParts"].ToString();
                    }
                    else
                    {
                        DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagVSelect, new Guid(ddlItmSubCatgry.SelectedValue), new Guid(ddlItmCtgry.SelectedValue), Guid.Empty);
                        int count = ds.Tables[0].Select(null, "SubSubCatDescription").Count();
                        if (count > 0)
                            DT = ds.Tables[0].Select(null, "SubSubCatDescription").CopyToDataTable();
                        else
                            DT = null;
                        if (DT != null && DT.Rows.Count > 0)
                        {
                            ddlItmSubSubCatgry.DataSource = DT;
                            ddlItmSubSubCatgry.DataTextField = "SubSubCatDescription";
                            ddlItmSubSubCatgry.DataValueField = "SubSubCatID";
                            ddlItmSubSubCatgry.DataBind();
                            txtItmCode.Enabled = false;
                            ddlItmSubSubCatgry.Enabled = true;
                            // ddlItmSubSubCatgry.Items.Insert(0, new ListItem(ds.Tables[0].Rows[0]["SubSubCatDescription"].ToString(), ds.Tables[0].Rows[0]["SubSubCatID"].ToString()));
                            ddlItmSubSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                        }
                    }

                    //else
                    //{
                    //    lblSSCode.Text = "";
                    //    ddlItmSubSubCatgry.Enabled = false;
                    //    ddlItmSubSubCatgry.Items.Clear();
                    //    txtItmCode.Text = "";
                    //    txtItmCode.Enabled = true;
                    //}
                }
                else
                {
                    ddlItmSubSubCatgry.Enabled = false;
                    ddlItmSubSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                    txtItmCode.Text = "";
                    txtItmCode.Enabled = false;
                }
                //ddlItmSubSubCatgry.Items.Insert(0, new ListItem(ds.Tables[0].Rows[0]["SubCatDescription"].ToString(), ds.Tables[0].Rows[0]["SubCatID"].ToString()));

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        private void DDLItmSubSubCatChange()
        {
            try
            {
                if (ddlItmSubSubCatgry.Enabled == true && ddlItmSubSubCatgry.SelectedValue != Guid.Empty.ToString())
                {
                    lblSSCode.Text = ddlItmSubSubCatgry.SelectedItem.Text.Split('-')[0];
                    txtItmCode.MaxLength = 4;
                    txtItmCode.Enabled = true;
                }
                else
                {
                    txtItmCode.MaxLength = 7;
                    lblSSCode.Text = "";
                    txtItmCode.Enabled = false;
                }
                //txtItmCode.Text = lblCode.Text + "" + lblSCode.Text + "" + lblSSCode.Text + "" + txtItmCode.Text;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        #endregion

        #region DDL Selected Index Change Events

        protected void ddlItmCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DDLItmCatChange();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        protected void ddlItmSubCatgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DDLItmSubCatChange();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }

        protected void ddlItmSubSubCatgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDLItmSubSubCatChange();
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
                if (txtItmDscrip.Text.Trim() != "" && ddlItmCtgry.SelectedValue != Guid.Empty.ToString() && ddlItmSubCatgry.SelectedValue != Guid.Empty.ToString())//txtItmCode.Text.Trim() != ""
                {
                    Filename = FileName();
                    new_ItmCode = GetNewItm_Code();
                    //btnSave.Attributes.Add("OnClick", "javascript:SearchItmCode()");
                    if (btnSave.Text == "Save")
                    {
                        string itemCode = "";
                        bool CanSave = true;
                        if (ddlItmSubSubCatgry.Items.Count > 1 && ddlItmSubSubCatgry.SelectedValue == Guid.Empty.ToString())
                            CanSave = false;

                        if (CanSave)
                        {
                            //if (txtItmDscrip.Text.Trim().Length > 8)
                            //  itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()).Substring(0, 8));
                            //  else
                            //    itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()));
                            //Guid? SSID =  ddlItmSubSubCatgry.SelectedIndex == -1 ? null : new Guid(ddlItmSubSubCatgry.SelectedValue;
                            Guid? SSid = null, SpareID = null;
                            //if (ddlItmSubSubCatgry.SelectedIndex != 0 && ddlItmSubSubCatgry.SelectedItem.Text.Contains(":Spares"))
                            //    SpareID = new Guid(ddlItmSubSubCatgry.SelectedValue);
                            //else if (ddlItmSubSubCatgry.SelectedIndex != 0 && !ddlItmSubSubCatgry.SelectedItem.Text.Contains(":Spares"))
                            //    SSid = new Guid(ddlItmSubSubCatgry.SelectedValue);
                            if (ddlItmSubSubCatgry.SelectedIndex > 0)
                                SSid = new Guid(ddlItmSubSubCatgry.SelectedValue);

                            int result = ItmMstr.InsertUpdateItemMasterNew(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty,
                                new_ItmCode, txtItmDscrip.Text, txtspec.Text, txtItmPrtNmbr.Text, new_ItmCode, new Guid(Session["UserID"].ToString()),
                                txtHSCode.Text, ChkHadSubItems.Checked, new Guid(Session["CompanyID"].ToString()), new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatgry.SelectedValue)
                                , SSid, SpareID, 0, 0);
                            if (result == 0)
                            {
                                ALS.AuditLog(res, btnSave.Text, "", "Item Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');window.location ='ItemMaster_Revised.aspx';", true);
                                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master", "Data Inserted successfully in Item Master.");
                                ClearAll();
                            }
                        }
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Sub Sub Category is required when Mapped.');", true);
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill all Mandatory details.');", true);
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
            try
            {
                if (txtItmDscrip.Text.Trim() != "" && ddlItmCtgry.SelectedValue != Guid.Empty.ToString() && ddlItmSubCatgry.SelectedValue != Guid.Empty.ToString())
                {
                    Filename = FileName();
                    new_ItmCode = GetNewItm_Code();
                    string itemCode = "";

                    bool CanSave = true;
                    if (ddlItmSubSubCatgry.Items.Count > 1 && ddlItmSubSubCatgry.SelectedValue == Guid.Empty.ToString())
                        CanSave = false;

                    if (CanSave)
                    {
                        Guid? SSid = null, SpareID = null;
                        if (ddlItmSubSubCatgry.SelectedIndex != 0 && ddlItmSubSubCatgry.SelectedItem.Text.Contains(":Spares"))
                            SpareID = new Guid(ddlItmSubSubCatgry.SelectedValue);
                        else if (ddlItmSubSubCatgry.SelectedIndex != 0 && !ddlItmSubSubCatgry.SelectedItem.Text.Contains(":Spares"))
                            SSid = new Guid(ddlItmSubSubCatgry.SelectedValue);

                        if (txtItmDscrip.Text.Trim().Length > 8)
                            itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()).Substring(0, 8));
                        else
                            itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()));
                        int result = ItmMstr.InsertUpdateItemMasterNew(CommonBLL.FlagUpdate, new Guid(hdfdItmMstrID.Value), Guid.Empty, new_ItmCode,
                            txtItmDscrip.Text, txtspec.Text, txtItmPrtNmbr.Text, new_ItmCode, new Guid(Session["UserID"].ToString()), txtHSCode.Text,
                            ChkHadSubItems.Checked, new Guid(Session["CompanyID"].ToString()), new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatgry.SelectedValue)
                                , SSid, SpareID, 0, 0);//, SSid, SpareID, 0, 0);
                        if (result == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "Item Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ALS.AuditLog(res, btnSave.Text, "", "Item Master Item Update:" + "ItemCode:" + ItmCodetxt + " ,OldItemDesc: " + OldItemDesc + ",OldPartNumber:" + OldPartNum + ",OldSpecification: " + OldSpecification + ",OldCheckBox:" + oldCheckbox + "", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "window.location ='ItemMaster_Revised.aspx';SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master", "Data Updated successfully in Item Master.");
                            ClearAll();
                        }
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Sub Sub Category is required when Mapped.');", true);
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Fill all Mandatory details.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master Revised", ex.Message.ToString());
            }
        }

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = FileName();
                DataSet Itm = new DataSet();
                DataSet ds = ReadExcelData();
                //if (ds.Tables[0].Columns.Count > 13)
                //{
                //    for (int i = 0; i <= ds.Tables[0].Columns.Count; )
                //    {
                //        if (i > 13)
                //        {
                //            ds.Tables[0].Columns.RemoveAt(14);
                //            i = 14;
                //        }
                //        i++;
                //    }

                //}
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    //Int64 m = 0;
                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //{
                    //    m = Convert.ToInt64(ds.Tables[0].Rows[i]["Code"].ToString());
                    //}
                    //if (ds.Tables.Contains("Sheet1$"))
                    //{
                    //Itm = ItmMstr.InsertUpdateItemMasterDataTable_New(ds.Tables[0], new Guid(Session["CompanyId"].ToString()),
                    //    new Guid(Session["UserId"].ToString()), filename, "");
                    //ds.Tables[0].Columns.Remove("Sno");
                    ds.Tables[0].Columns.Remove("ItemCode");
                    ds.Tables[0].Columns.Remove("REMARKS");

                    Itm = ItmMstr.ItemMasterBulkUpload("C", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), ds.Tables[0]);
                    //}
                    if (Itm != null && Itm.Tables.Count > 0)//&& Itm.Tables[0].Rows[0][0].ToString() != "")
                    {
                        if (Itm.Tables[0].Rows.Count > 0)
                            ELog.CreateErrorLog(Server.MapPath("../Logs/BulkUpload/ErrorLog"), "Item Master Revised Bulk Upload", Itm.Tables[0].Rows[0][0].ToString().Trim(','));
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('these New Item Codes " + Itm.Tables[0].Rows[0][0].ToString() + " are unable to Insert');", true);
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAlertMessage", "alert('these New Item Codes \r\n" + Itm.Tables[0].Rows[0][0].ToString().Trim(',') + " \r\nare unable to Insert');", true);
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);                        
                        try
                        {
                            string _Msg = string.Format(@"Incorrect Format: " + string.Join(@",\r\n", Itm.Tables[0].AsEnumerable().Select(P => P.Field<String>("Newitemcode")).Distinct().ToList()) +
                                        " \\n\\rCodes Already Exists:-" +
                                        "\\n\\rCategory Codes are :" + string.Join(@",", Itm.Tables[1].AsEnumerable().Select(P => P.Field<String>("CCode")).Distinct().ToList()) +
                                        "\\n\\rSub-Category Codes are :" + string.Join(@",\r\n", Itm.Tables[2].AsEnumerable().Select(P => P.Field<String>("SCCode")).Distinct().ToList()) +
                                        "\\n\\rSub-SubCategory Codes are :" + string.Join(@",\r\n", Itm.Tables[3].AsEnumerable().Select(P => P.Field<String>("SSCCode")).Distinct().ToList()) +
                                        "\\n\\rItem Master Codes are :" + string.Join(@",\r\n", Itm.Tables[4].AsEnumerable().Select(P => P.Field<String>("Newitemcode")).Distinct().ToList()) + " ");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + _Msg + "');", true);
                        }
                        catch (Exception Ex)
                        {
                            if (Ex.Message.Contains("does not belong to table"))
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + Itm.Tables[Itm.Tables.Count - 1].Rows[0]["ErrorMessage"].ToString().Replace("'", "") + "');", true);
                            else
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + Ex.Message.Replace("'", "") + "');", true);
                        }
                    }
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('these rownumbers" + Itm.Tables[1].Rows[0]["NotInserted"].ToString() + " are unable to Insert');", true);

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master Revised Bulk Upload", ex.Message.ToString());
            }
        }

        #endregion

        # region WEbMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckItemsMaster(string ItemDesc, string PartNumber, string ItemSpec, string EID, string IsSubItem)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckItemsMaster('X', ItemDesc, PartNumber, ItemSpec, new Guid(EID), IsSubItem, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
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

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckCode(string Code)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('X', Code, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        # endregion
    }
}