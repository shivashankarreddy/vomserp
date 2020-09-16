using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class ModuleScreenMapping : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        CompanyBLL CBL = new CompanyBLL();
        ModuleAccessBLL MAB = new ModuleAccessBLL();
        FieldAccessBLL FAB = new FieldAccessBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load Event

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    GetData();
                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
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

        private void GetData()
        {
            try
            {
                BindDropDownList(ddlCompany, CBL.SelectCompany(CommonBLL.FlagRegularDRP, Guid.Empty).Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataTable commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt;
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        private void EditDetails(DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    //BindDropDownList(ddlScreen, FAB.GetFieldMaster(CommonBLL.FlagASelect, Guid.Empty, new Guid(ddlCompany.SelectedValue)).Tables[0]);
                    //ddlScreen.SelectedValue = CommonDt.Tables[0].Rows[0]["ScreenID"].ToString();
                    
                    DataSet Dset = FAB.GetFieldMaster(CommonBLL.FlagBSelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                    ViewState["ID"] = CommonDt.Tables[0].Rows[0]["ID"].ToString();
                    ViewState["EditItems"] = CommonDt.Tables[1];
                    ViewState["PriceTable"] = Dset.Tables[1];
                    ViewState["IsEdit"] = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
                throw new Exception("Unable to populate GridView" + ex.Message);
            }

        }

        private void ClearAll()
        {
            try
            {
                ddlCompany.SelectedIndex = ddlModuleName.SelectedIndex =  ddlScreen.SelectedIndex = -1;
                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Event

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlModuleName, MAB.ModuleAccess(CommonBLL.FlagESelect, new Guid(ddlCompany.SelectedValue)).Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        protected void ddlModuleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlScreen, MAB.ModuleAccess(CommonBLL.FlagFSelect, new Guid(ddlCompany.SelectedValue)).Tables[0]);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        protected void ddlScreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet FieldTable = FAB.GetFieldDetails(CommonBLL.FlagASelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                if (FieldTable != null && FieldTable.Tables.Count > 0 && FieldTable.Tables[0].Rows.Count > 0)
                {
                    EditDetails(FieldTable);
                }
                else
                {
                    DataSet dset = FAB.GetFieldMaster(CommonBLL.FlagBSelect, new Guid(ddlScreen.SelectedValue), new Guid(ddlCompany.SelectedValue));
                    if (dset != null && dset.Tables.Count > 0)
                    {
                        ViewState["PriceTable"] = dset.Tables[1];
                        
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        
        #endregion

        #region Button Click Events

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                int Res = -999;
                
                //if (SeletedData.Rows.Count > 0)
                //{
                //    if (btnSave.Text == "Save")
                //    {
                //        Res = FAB.InsertUpdatedDeltedFieldDetails(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCompany.SelectedValue), new Guid(ddlScreen.SelectedValue),
                //            txtSequenceNumber.Text, Guid.Empty, new Guid(Session["UserID"].ToString()), SeletedData);
                //        if (Res == 0)
                //        {
                //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                //            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Module Screen Mapping Master", "Inserted Successfully in Application Field Access Master.");
                //            ClearAll();
                //        }
                //    }
                //    else if (btnSave.Text == "Update")
                //    {
                //        Res = FAB.InsertUpdatedDeltedFieldDetails(CommonBLL.FlagUpdate, new Guid(ViewState["ID"].ToString()), new Guid(ddlCompany.SelectedValue), new Guid(ddlScreen.SelectedValue),
                //            txtSequenceNumber.Text, Guid.Empty, new Guid(Session["UserID"].ToString()), SeletedData);
                //        if (Res == 0)
                //        {
                //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully');", true);
                //            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Module Screen Mapping Master",
                //                "Updated Successfully in Application Access Master.");
                //            ClearAll();
                //        }
                //    }
                //}
                //else
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select atleast one field name.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Screen Mapping Master", ex.Message.ToString());
            }
        }

        #endregion
    }
}