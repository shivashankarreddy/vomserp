using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BAL;
using System.Collections.Generic;
using System.Text;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Enquiries
{
    public partial class AddItems : System.Web.UI.Page
    {
        # region variables
        int res;
        BAL.ItemMasterBLL ItmMstr = new ItemMasterBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        string new_ItmCode = "";
        #endregion

        #region Default Page Load Event
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Show All", "NoPermissions();", true);
                    //Response.Redirect("../Login.aspx?logout=yes", false);
                }
                else
                {
                    Ajax.Utility.RegisterTypeForAjax(typeof(AddItems));
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Enabled = false;
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Page.LoadComplete += new EventHandler(Page_LoadComplete);
                        txtItmCode.Enabled = false;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Show All", "NoPermissions();", true);
                        //Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
            }
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
        }
        #endregion

        #region Methods
        private void DDLItmCatChange()
        {
            try
            {
                if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString())
                {
                    ddlItmSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Items.Clear();
                    ddlItmSubCatgry.Enabled = true;
                    ddlItmSubSubCatgry.Enabled = false;
                    DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagPSelectAll, Guid.Empty, new Guid(ddlItmCtgry.SelectedValue), Guid.Empty);
                    DataTable DT = ds.Tables[0].Select(null, "SubCatDescription").CopyToDataTable();
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        ddlItmSubCatgry.DataSource = DT;
                        ddlItmSubCatgry.DataTextField = "SubCatDescription";
                        ddlItmSubCatgry.DataValueField = "SubCatID";
                        ddlItmSubCatgry.DataBind();
                        ddlItmSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                    }
                    else
                    {
                        ddlItmSubSubCatgry.Enabled = false;
                        ddlItmSubCatgry.Enabled = false;
                        ddlItmSubCatgry.Items.Clear();
                        ddlItmSubSubCatgry.Items.Clear();
                        txtItmCode.Text = "";
                    }
                }
                else
                {
                    ddlItmSubSubCatgry.Enabled = false;
                    ddlItmSubCatgry.Enabled = false;
                    ddlItmSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Items.Clear();
                    txtItmCode.Text = "";
                }
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
                if (ddlItmCtgry.SelectedValue != Guid.Empty.ToString() && ddlItmSubCatgry.SelectedValue != Guid.Empty.ToString())
                {
                    txtItmCode.MaxLength = 7;
                    ddlItmSubSubCatgry.Items.Clear();
                    ddlItmSubSubCatgry.Enabled = true;
                    DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagVSelect, new Guid(ddlItmSubCatgry.SelectedValue), new Guid(ddlItmCtgry.SelectedValue), Guid.Empty);
                    DataTable DT = ds.Tables[0].Select(null, "SubSubCatDescription").CopyToDataTable();
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        ddlItmSubSubCatgry.DataSource = DT;
                        ddlItmSubSubCatgry.DataTextField = "SubSubCatDescription";
                        ddlItmSubSubCatgry.DataValueField = "SubSubCatID";
                        ddlItmSubSubCatgry.DataBind();
                        txtItmCode.Enabled = false;
                        ddlItmSubSubCatgry.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                    }
                    else
                    {
                        ddlItmSubSubCatgry.Enabled = false;
                        ddlItmSubSubCatgry.Items.Clear();
                        txtItmCode.Text = "";
                        txtItmCode.Enabled = true;
                    }
                }
                else
                {
                    ddlItmSubSubCatgry.Enabled = false;
                    ddlItmSubSubCatgry.Items.Clear();
                    txtItmCode.Text = "";
                    txtItmCode.Enabled = false;
                }
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
                    txtItmCode.MaxLength = 4;
                    txtItmCode.Enabled = true;
                }
                else
                {
                    txtItmCode.MaxLength = 7;
                    txtItmCode.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind DropDownList

        private string GetNewItm_Code()
        {
            string ItmCode = "";
            try
            {
                if (ddlItmCtgry.SelectedValue != "" && ddlItmSubCatgry.SelectedValue != "" && ddlItmSubSubCatgry.SelectedValue != "" && txtItmCode.Text != "")
                {
                    ItmCode = ddlItmCtgry.SelectedItem.Text.Split('-')[0] + "" + ddlItmSubCatgry.SelectedItem.Text.Split('-')[0] + "" + ddlItmSubSubCatgry.SelectedItem.Text.Split('-')[0] + "" + txtItmCode.Text;
                }
                else if (ddlItmCtgry.SelectedValue != "" && ddlItmSubCatgry.SelectedValue != "" && ddlItmSubSubCatgry.SelectedValue == "" && txtItmCode.Text.Length == 7)
                {
                    ItmCode = ddlItmCtgry.SelectedItem.Text.Split('-')[0] + ddlItmSubCatgry.SelectedItem.Text.Split('-')[0] + txtItmCode.Text;
                }
                //if (ddlItmCtgry.SelectedValue != "" && ddlItmSubCatgry.SelectedValue != "" && ddlItmSubSubCatgry.SelectedValue != "" && txtItmCode.Text != "")
                //{
                //    ItmCode = txtItmCode.Text;
                //}
                return ItmCode;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Master", ex.Message.ToString());
                return "";
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
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "id";
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
        /// Bind Data to DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                Session["HFITemsValues"] = "";
                if (Codes != null && Codes.Count > 0)
                {
                    HFAllSelectedItems.Value = string.Join(",", Codes.ToArray().Select(o => o.Value.ToString().Trim()).ToArray()).ToString();
                    Session["HFITemsValues"] = HFAllSelectedItems.Value;
                }

                //BindGridData(ItmMstr.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty,new Guid(Session["CompanyID"].ToString())));
                //BindDropDownList(ddlItmCtgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));                
                BindDropDownListForCatgry(ddlItmCtgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
                ddlItmSubCatgry.Items.Clear();
                ddlItmSubSubCatgry.Items.Clear();
                ddlItmSubCatgry.Enabled = false;
                ddlItmSubSubCatgry.Enabled = false;
                //BindDropDownListForCatgry(ddlItmSubCatgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagBSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
                //BindDropDownListForCatgry(ddlItmSubSubCatgry, embal.EnumMasterSelectforDescription(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), ""));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
            }
        }
        protected void BindGridData(DataSet ItmMstDt)
        {
            try
            {
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                foreach (var item in Codes)
                {
                    DataRow[] dr = ItmMstDt.Tables[0].Select("ID = " + "'" + item.Value + "'");
                    dr[0].Delete();
                }
                ItmMstDt.AcceptChanges();
                //gvItmMstr.DataSource = ItmMstDt;
                //gvItmMstr.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
            }
        }
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                ddl.SelectedValue = (ddl.Items.FindByText("General")).Value;
                ddl.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
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

        #region Insert Data into Item Master Table

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
                new_ItmCode = GetNewItm_Code();
                if (btnSave.Text == "Save" && txtItmDscrip.Text.Trim() != "")
                {
                    Session["NewItemAdded"] = false;
                    //string itemCode = "";
                    //if (txtItmDscrip.Text.Trim().Length > 8)
                    //    itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()).Substring(0, 8));
                    //else
                    //    itemCode = (ddlItmCtgry.SelectedValue + (txtItmDscrip.Text.Trim()));
                    Guid? SSCatID = null;
                    if (ddlItmSubSubCatgry.SelectedIndex > 0)
                        SSCatID = new Guid(ddlItmSubSubCatgry.SelectedValue);
                    if (new_ItmCode.Length == 11)
                    {
                        DataSet result = ItmMstr.InsertUpdateItemMasterPopUp(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, new_ItmCode,
                            txtItmDscrip.Text, txtspec.Text, txtItmPrtNmbr.Text, new_ItmCode, new Guid(Session["UserID"].ToString()), txtHSCode.Text,
                            ChkHadSubItems.Checked, new Guid(Session["CompanyID"].ToString()), new Guid(ddlItmCtgry.SelectedValue), new Guid(ddlItmSubCatgry.SelectedValue)
                            , SSCatID, null, 0, 0);
                        if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                        {
                            ALS.AuditLog(0, btnSave.Text, txtItmDscrip.Text, "Item :", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "Add Items", "Inserted successfully in Item Master.");
                            Session["NewItemAdded"] = true;
                            DataSet _DS = (DataSet)Session["ItemsDescPrtNo"];
                            DataRow _DR = _DS.Tables[0].NewRow();
                            _DR["ID"] = result.Tables[0].Rows[0][0];
                            _DR["ItemDescription"] = txtItmDscrip.Text == "" ? "" : txtItmDscrip.Text;
                            _DR["PartNumber"] = txtItmPrtNmbr.Text == "" ? "" : txtItmPrtNmbr.Text;
                            _DR["Specification"] = txtspec.Text == "" ? "" : txtspec.Text;
                            _DS.Tables[0].Rows.Add(_DR);
                            Session["ItemsDescPrtNo"] = _DS;
                            //DataSet ds = ItmMstr.SelectItemMaster(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));//CategoryId --247
                            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            //{
                            HFSelectedVal.Value = result.Tables[0].Rows[0][0].ToString() + "," + "Added";
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Show All", "SendSelectedVal();", true);
                            //}
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Inserting.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ALS.AuditLog(-1, btnSave.Text, txtItmDscrip.Text, "Item :", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Inserting.');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
            }
        }
        #endregion

        #region Clear Inputs

        /// <summary>
        /// Clear all input fields
        /// </summary>

        protected void ClearAll()
        {
            try
            {
                ddlItmCtgry.SelectedIndex = 0;
                txtItmDscrip.Text = txtItmPrtNmbr.Text = txtspec.Text = "";
                btnSave.Text = "Save";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Add Items", ex.Message.ToString());
            }
        }
        #endregion

        # region GridView Events

        /// <summary>
        /// GridView RowDataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvItmMstr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType != DataControlRowType.DataRow) return;

            //int lastCellIndex = e.Row.Cells.Count - 1;
            //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];

            //if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
            //    EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";

            //if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
            //    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            //else
            //    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";                        
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvItmMstr_PreRender(object sender, EventArgs e)
        {
            try
            {
                //gvItmMstr.UseAccessibleHeader = false;
                //gvItmMstr.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        # endregion

        # region WEbMethods
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool CheckItemsMaster(string ItemDesc, string PrtNmbr, string ItemSpec, string Sub)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckItemsMaster('X', ItemDesc, PrtNmbr, ItemSpec, Guid.Empty, Sub, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public byte[] HfValues(string Val)
        {
            byte[] array = Encoding.ASCII.GetBytes(Val);
            string xyz = ASCIIEncoding.ASCII.GetString(array);
            //string Encryp = LoginBLL.MD5(Val);
            return array;
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckCode(string Code)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckNo('X', Code, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        # endregion

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Enquiries/AddItems.aspx");
        }
    }
}
