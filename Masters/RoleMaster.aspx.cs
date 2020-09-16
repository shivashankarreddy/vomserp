using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using BAL;

namespace VOMS_ERP.Masters
{
    public partial class RoleMaster : System.Web.UI.Page
    {

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
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
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
                        DataTable dtbl = new DataTable();
                        DataColumn dcl = new DataColumn();
                        dtbl.Columns.Add("ID");
                        dtbl.Columns.Add("Department Name");

                        DataRow frst = dtbl.NewRow();
                        frst["ID"] = "1";
                        frst["Department Name"] = "Manager";
                        dtbl.Rows.Add(frst);

                        DataRow second = dtbl.NewRow();
                        second["ID"] = "2";
                        second["Department Name"] = "Team Lead";
                        dtbl.Rows.Add(second);
                        DataRow third = dtbl.NewRow();
                        third["ID"] = "3";
                        third["Department Name"] = "Team Member";
                        dtbl.Rows.Add(third);

                        gvRole.DataSource = dtbl;
                        gvRole.DataBind();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }
    }
}
