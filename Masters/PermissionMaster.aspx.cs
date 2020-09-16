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

namespace VOMS_ERP.Masters
{
    public partial class PermissionMaster : System.Web.UI.Page
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
            //DataTable dtbl = new DataTable();
            //DataColumn dcl = new DataColumn();
            //dtbl.Columns.Add("ID");
            //dtbl.Columns.Add("Department Name");

            //DataRow frst = dtbl.NewRow();
            //frst["ID"] = "1";
            //frst["Department Name"] = "Manager";
            //dtbl.Rows.Add(frst);

            //DataRow second = dtbl.NewRow();
            //second["ID"] = "2";
            //second["Department Name"] = "Team Lead";
            //dtbl.Rows.Add(second);
            //DataRow third = dtbl.NewRow();
            //third["ID"] = "3";
            //third["Department Name"] = "Data Entry Operator";
            //dtbl.Rows.Add(third);


            //gvPmsnMstr.DataSource = dtbl;
            //gvPmsnMstr.DataBind();

            try
            {
                if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    ;
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
