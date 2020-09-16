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

namespace VOMS_ERP.Enquiries
{
    public partial class ImportEnquiry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             try
            {
                if (Session["UserID"] == null || Convert.ToInt64(Session["UserID"]) == 0)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
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
    }
}
