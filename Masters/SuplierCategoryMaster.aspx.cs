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
    public partial class SuplierCategoryMaster : System.Web.UI.Page
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
                if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {

                    DataTable dtbl = new DataTable();
                    DataColumn dcl = new DataColumn();
                    dtbl.Columns.Add("ID");
                    dtbl.Columns.Add("Category Name");

                    DataRow frst = dtbl.NewRow();
                    frst["ID"] = "1";
                    frst["Category Name"] = "Spears";
                    dtbl.Rows.Add(frst);

                    DataRow second = dtbl.NewRow();
                    second["ID"] = "2";
                    second["Category Name"] = "Machinical";
                    dtbl.Rows.Add(second);
                    DataRow third = dtbl.NewRow();
                    third["ID"] = "3";
                    third["Category Name"] = "Electrical";
                    dtbl.Rows.Add(third);


                    gvCtgry.DataSource = dtbl;
                    gvCtgry.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
