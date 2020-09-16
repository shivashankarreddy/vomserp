using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using Ajax;

namespace VOMS_ERP.Masters
{
    public partial class ItemsMapping : System.Web.UI.Page
    {
        #region variables

        ItemsMappingBLL IMBLL = new ItemsMappingBLL();
        ItemCategoryBAL CBL = new ItemCategoryBAL();
        ErrorLog ELog = new ErrorLog();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ItemsMapping));
            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
            btnUpdate.Attributes.Add("OnClick", "javascript:return Myvalidations()");

            if (!IsPostBack)
            {
                btnUpdate.Visible = false;
                GetData();
            }
        }

        #region Methods

        private void GetData()
        {
            try
            {
                BindDropDownList(ddlItmCtgry, CBL.GetSubCategory(CommonBLL.FlagRegularDRP, Guid.Empty, 0, ""));

                if (Request.QueryString["ID"] != null)
                {
                    EditRecord();
                }
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        private void EditRecord()
        {
            try
            {
                DataSet ds = IMBLL.GetData(CommonBLL.FlagESelect, new Guid(Request.QueryString["ID"]));
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    string CtgryID = ds.Tables[1].Rows[0]["CategoryID"].ToString();
                    string CtgryCode = ds.Tables[1].Rows[0]["CatCode"].ToString();

                    string SubCtgryID = ds.Tables[1].Rows[0]["SubCategoryID"].ToString();
                    string SubCtgryCode = ds.Tables[1].Rows[0]["SubCatCode"].ToString();

                    string SubSubCtgryID = ds.Tables[1].Rows[0]["SubSubCategoryID"].ToString();
                    string SubSubCtgryCode = ds.Tables[1].Rows[0]["SubSubCatCode"].ToString();


                    ddlItmCtgry.SelectedValue = ds.Tables[1].Rows[0]["CatCode"].ToString();
                    hfItmCtgry.Value = CtgryID;
                    lblItemCtgryCode.Text = CtgryCode;

                    BindDropDownList(ddlItmSubCtgry, CBL.GetSubCategory(CommonBLL.FlagBSelect, new Guid(hfItmCtgry.Value), 0, ""));
                    ddlItmSubCtgry.SelectedValue = SubCtgryCode;
                    lblItemSubCtgryCode.Text = ddlItmSubCtgry.SelectedItem.Value != "0" ? ddlItmSubCtgry.SelectedItem.Value : ".";
                    hfItmSubCtgry.Value = SubCtgryID;


                    BindDropDownList(ddlItmSubSubCtgry, CBL.GetSubSubCategory(CommonBLL.FlagRegularDRP, Guid.Empty, 0, new Guid(hfItmCtgry.Value),
                        new Guid(hfItmSubCtgry.Value)));
                    ddlItmSubSubCtgry.SelectedValue = SubSubCtgryCode;
                    lblItemSubSubCtgryCode.Text = ddlItmSubSubCtgry.SelectedItem.Value != "0" ? ddlItmSubSubCtgry.SelectedItem.Value : ".";
                    hfItmSubCtgry.Value = SubSubCtgryID;

                    btnSave.Visible = false;
                    btnUpdate.Visible = true;
                    btnUpdate.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataSet ds)
        {
            try
            {
                ddl.Items.Clear();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ddl.ID == "ddlItmCtgry")
                        ViewState["ItmCtgry"] = ds.Tables[0];
                    else if (ddl.ID == "ddlItmSubCtgry")
                        ViewState["ItmSubCtgry"] = ds.Tables[0];
                    else if (ddl.ID == "ddlItmSubSubCtgry")
                        ViewState["ItmSubSubCtgry"] = ds.Tables[0];

                    ddl.DataSource = ds.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("--" + ddl.ID.Replace("ddl", "select ") + "--", "0"));
                Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        protected string GetSelectedID(DataTable dt, string Code)
        {
            string value = Guid.Empty.ToString();
            try
            {
                if (Code != "" && Code != ".")
                {
                    var results = from DataRow myRow in dt.Rows
                                  where myRow["ID"].ToString() == Code
                                  select myRow;
                    if (results != null && results.ToList().Count > 0)
                        value = results.ToList()[0]["CatID"].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
            return value;
        }

        protected void Clear()
        {
            try
            {
                if (ddlItmCtgry.SelectedIndex == 0)
                {
                    ddlItmSubCtgry.SelectedIndex = -1;
                    ddlItmSubSubCtgry.SelectedIndex = -1;
                    //txtItemCode.Text = "";
                    //lblFullItemCode.Text = ".";
                    lblItemCtgryCode.Text = ".";
                    lblItemSubCtgryCode.Text = ".";
                    lblItemSubSubCtgryCode.Text = ".";
                }
                else if (ddlItmSubCtgry.SelectedIndex == 0)
                {
                    //ddlItmSubCtgry.SelectedIndex = -1;
                    ddlItmSubSubCtgry.SelectedIndex = -1;
                    //txtItemCode.Text = "";
                    //lblFullItemCode.Text = ".";
                    //lblItemCtgryCode.Text = ".";
                    lblItemSubCtgryCode.Text = ".";
                    lblItemSubSubCtgryCode.Text = ".";
                }
                else if (ddlItmSubSubCtgry.SelectedIndex == 0)
                {
                    //ddlItmSubCtgry.SelectedIndex = -1;
                    //ddlItmSubSubCtgry.SelectedIndex = -1;
                    //txtItemCode.Text = "";
                    //lblFullItemCode.Text = ".";
                    //lblItemCtgryCode.Text = ".";
                    //lblItemSubCtgryCode.Text = ".";
                    lblItemSubSubCtgryCode.Text = ".";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        private void ClearAll()
        {
            btnSave.Visible = true;
            btnUpdate.Visible = false;
            ddlItmCtgry.SelectedIndex = -1;
            ddlItmSubCtgry.SelectedIndex = -1;
            ddlItmSubSubCtgry.SelectedIndex = -1;
            //txtItemCode.Text = "";
            //lblFullItemCode.Text = ".";
            lblItemCtgryCode.Text = ".";
            lblItemSubCtgryCode.Text = ".";
            lblItemSubSubCtgryCode.Text = ".";
        }

        #endregion

        #region DDl Selected Index Change

        protected void ddlItmCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblItemCtgryCode.Text = ddlItmCtgry.SelectedItem.Value != "0" ? ddlItmCtgry.SelectedItem.Value : ".";
            hfItmCtgry.Value = GetSelectedID((DataTable)ViewState["ItmCtgry"], lblItemCtgryCode.Text);
            BindDropDownList(ddlItmSubCtgry, CBL.GetSubCategory(CommonBLL.FlagBSelect, new Guid(hfItmCtgry.Value), 0, ""));
            Clear();
        }

        protected void ddlItmSubCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblItemSubCtgryCode.Text = ddlItmSubCtgry.SelectedItem.Value != "0" ? ddlItmSubCtgry.SelectedItem.Value : ".";
            hfItmSubCtgry.Value = GetSelectedID((DataTable)ViewState["ItmSubCtgry"], lblItemSubCtgryCode.Text);
            BindDropDownList(ddlItmSubSubCtgry, CBL.GetSubSubCategory(CommonBLL.FlagRegularDRP, Guid.Empty, 0, new Guid(hfItmCtgry.Value), new Guid(hfItmSubCtgry.Value)));
            //BindDropDownList(ddlItems, IMBLL.GetData(CommonBLL.FlagBSelect));
            Clear();
        }

        protected void ddlItmSubSubCtgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (ddlItmSubSubCtgry.SelectedIndex > 0)
                //    txtItemCode.MaxLength = 5;
                //else
                //    txtItemCode.MaxLength = 7;
                lblItemSubSubCtgryCode.Text = ddlItmSubSubCtgry.SelectedItem.Value != "0" ? ddlItmSubSubCtgry.SelectedItem.Value : ".";
                hfItmSubSubCtgry.Value = GetSelectedID((DataTable)ViewState["ItmSubSubCtgry"], lblItemSubSubCtgryCode.Text);
                Clear();

                //BindDropDownList(ddlItems, IMBLL.GetData(CommonBLL.FlagBSelect));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        #endregion

        #region Button Click Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //new Guid(ddlItems.SelectedValue),lblFullItemCode.Text
                Guid SubSubCat = Guid.Empty;
                if (ddlItmSubSubCtgry.SelectedIndex != 0)
                    SubSubCat = new Guid(hfItmSubSubCtgry.Value);
                int res = IMBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(hfItmCtgry.Value),
                    new Guid(hfItmSubCtgry.Value), SubSubCat, Guid.Empty, "",
                    Guid.Empty, new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                    ClearAll();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //new Guid(ddlItems.SelectedValue),lblFullItemCode.Text
                Guid SubSubCat = Guid.Empty;
                if (ddlItmSubSubCtgry.SelectedIndex != 0)
                    SubSubCat = new Guid(hfItmSubSubCtgry.Value);
                int res = IMBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, Guid.Empty, new Guid(hfItmCtgry.Value),
                            new Guid(hfItmSubCtgry.Value), SubSubCat, Guid.Empty, "",
                            Guid.Empty, Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully');", true);
                    ClearAll();
                    Response.Redirect("itemsMaster.aspx", false);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region WebMethods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID)
        {
            try
            {
                int res = 1;
                string result = "Error::Cannot Delete this Record";

                #region Delete

                res = IMBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID));
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                {
                    result = "Error::Cannot Delete this Record, Error while Deleting.";
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Item Master",
                       "Data Deleted successfully from Item Master.");
                    ClearAll();
                }

                #endregion
                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Items Mapping", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}