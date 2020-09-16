using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.IO;

namespace VOMS_ERP.Admin
{
    public partial class ModuleAccessMaster : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        CompanyBLL CBL = new CompanyBLL();
        ModuleAccessBLL MABL = new ModuleAccessBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        #endregion

        #region Default Page Load Event

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            GetData();
                            btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
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
                BindDropDownList(ddlCompany, CBL.SelectCompany(CommonBLL.FlagRegularDRP, new Guid(Session["CompanyID"].ToString())));
                BindTreeView(MABL.ModuleAccess(CommonBLL.FlagASelect, Guid.Empty), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
            }
        }

        protected void BindDropDownList(DropDownList ddl, DataSet commonDt)
        {
            try
            {
                if (commonDt != null)
                {
                    ddl.DataSource = commonDt.Tables[0];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
            }
        }

        public void BindTreeView(DataSet dset, bool IsEdit)
        {
            try
            {
                if (dset != null && dset.Tables.Count > 1 && dset.Tables[1].Rows.Count > 0)
                {
                    DataSet ds = new DataSet();
                    DataTable dtparent = new DataTable();
                    DataTable dtchild = new DataTable();

                    dtparent = (DataTable)dset.Tables[0];
                    dtparent.TableName = "A";
                    dtchild = (DataTable)dset.Tables[1];
                    dtchild.TableName = "B";
                    ds.Tables.Add(dtparent.Copy());
                    ds.Tables.Add(dtchild.Copy());
                    ds.Relations.Add("children", ds.Tables[0].Columns["ID"], ds.Tables[1].Columns["ParentID"]);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ModuleAcess.Nodes.Clear();
                        foreach (DataRow masterRow in ds.Tables[0].Rows)
                        {
                            TreeNode masterNode = new TreeNode((string)masterRow["ModuleName"], Convert.ToString(masterRow["ID"]));
                            ModuleAcess.Nodes.Add(masterNode);
                            //ModuleAcess.CollapseAll();
                            int ChildCount = masterRow.GetChildRows("Children").Count();
                            int ChildChecked = 0;
                            foreach (DataRow childRow in masterRow.GetChildRows("Children"))
                            {
                                TreeNode childNode = new TreeNode((string)childRow["MenuLocation"], Convert.ToString(childRow["ParentID"]));
                                masterNode.ChildNodes.Add(childNode);
                                childNode.Value = Convert.ToString(childRow["ID"]);
                                childNode.Checked = Convert.ToBoolean(childRow["IsAccess"]);
                                ChildChecked = Convert.ToBoolean(childRow["IsAccess"]) ? (ChildChecked + 1) : ChildChecked;
                            }
                            if (ChildChecked > 0)
                            {
                                masterNode.ExpandAll();
                                if (ChildCount == ChildChecked)
                                {
                                    masterNode.Checked = true;
                                }
                            }
                            else
                                masterNode.CollapseAll();
                        }
                    }

                    btnSave.Text = IsEdit ? "Change Access" : "Grant Access";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
                throw new Exception("Unable to populate treeview" + ex.Message);
            }
        }

        public DataTable GetSelectedPages()
        {
            try
            {
                DataTable PermissionList = new DataTable();
                PermissionList.Columns.Add("ScreenID");
                PermissionList.Columns.Add("ScreenName");
                PermissionList.Columns.Add("ModuleID");
                PermissionList.Columns.Add("ModuleName");
                PermissionList.Columns.Add("IsAccess");

                foreach (TreeNode node in ModuleAcess.Nodes)
                {
                    foreach (TreeNode child in node.ChildNodes)
                    {
                        if (child.Checked)
                            PermissionList.Rows.Add(child.Value, child.Text, child.Parent.Value, child.Parent.Text, true);
                        else
                        {
                            PermissionList.Rows.Add(child.Value, child.Text, child.Parent.Value, child.Parent.Text, false);
                        }
                    }
                }

                return PermissionList;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
                return null;
            }
        }

        private void ClearAll()
        {
            try
            {
                ddlCompany.SelectedIndex = -1;
                foreach (TreeNode tn in ModuleAcess.Nodes)
                {
                    tn.Checked = false;
                    foreach (TreeNode child in tn.ChildNodes)
                        child.Checked = false;
                }
                ModuleAcess.CollapseAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
            }
        }

        #endregion

        #region Selected Index Changed Event

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataSet dset = MABL.ModuleAccess(CommonBLL.FlagBSelect, new Guid(ddlCompany.SelectedValue));
                if (dset != null && dset.Tables.Count > 1 && dset.Tables[1].Rows.Count > 0)
                    BindTreeView(dset, true);
                else
                    BindTreeView(MABL.ModuleAccess(CommonBLL.FlagASelect, Guid.Empty), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
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
                DataTable SeletedData = GetSelectedPages();
                if (SeletedData.Rows.Count > 0)
                {
                    if (btnSave.Text == "Grant Access")
                    {
                        Res = MABL.InsertUpdatedDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCompany.SelectedValue), Guid.Empty, Guid.Empty, true,
                            new Guid(Session["UserID"].ToString()), SeletedData);
                        if (Res == 0)
                        {
                            ALS.AuditLog(res, "Save", "", "Module Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Module Access Master",
                                "Inserted Successfully in Application Access Master.");
                            ClearAll();
                        }
                    }
                    else if (btnSave.Text == "Change Access")
                    {
                        Res = MABL.InsertUpdatedDelete(CommonBLL.FlagUpdate, Guid.Empty, new Guid(ddlCompany.SelectedValue), Guid.Empty, Guid.Empty, true,
                            new Guid(Session["UserID"].ToString()), SeletedData);

                        if (Res == 0)
                        {
                            ALS.AuditLog(res, "Update", "", "Module Access Master", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/Log"), "Module Access Master",
                                "Updated Successfully in Application Access Master.");
                            ClearAll();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Module Access Master", ex.Message.ToString());
            }
        }

        #endregion

    }
}