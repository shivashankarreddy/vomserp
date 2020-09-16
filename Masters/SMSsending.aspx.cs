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
    public partial class SMSsending : System.Web.UI.Page
    {
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
                            btnSend.Enabled = true;
                            btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        }
                        else
                        {
                            btnSend.Enabled = false;
                            btnSend.Attributes.Add("OnClick", "javascript:return NoPermissionMessage()");
                        }
                        #endregion
                        DataTable dtbl = new DataTable();
                        DataColumn dcl = new DataColumn();
                        dtbl.Columns.Add("ID");
                        dtbl.Columns.Add("Department Name");

                        DataRow frst = dtbl.NewRow();
                        frst["ID"] = "+919598564782";
                        frst["Department Name"] = "L V L Rao (Manager)";
                        dtbl.Rows.Add(frst);

                        DataRow second = dtbl.NewRow();
                        second["ID"] = "9854726322";
                        second["Department Name"] = "V H S Sing (Sale's Manager)";
                        dtbl.Rows.Add(second);

                        DataRow third = dtbl.NewRow();
                        third["ID"] = "9265486975";
                        third["Department Name"] = "Y K S Roy (Asst. Manager)";
                        dtbl.Rows.Add(third);

                        gvSuplrs.DataSource = dtbl;
                        gvSuplrs.DataBind();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvSuplrs_PreRender(object sender, EventArgs e)
        {
            try
            {
                gvSuplrs.UseAccessibleHeader = false;
                gvSuplrs.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        protected void gvSuplrs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            int lastCellIndex = e.Row.Cells.Count - 1;
            ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
            ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];

            if ((string[])Session["UsrPermissions"] != null && !((string[])Session["UsrPermissions"]).Contains("Edit"))
                EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";

            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Delete"))
                deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            else
                deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
        }
        
    }
}
