using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;

namespace VOMS_ERP.Enquiries
{
    public partial class FEStatusTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetData();
            }
        }

        private void GetData()
        {
            try
            {
                //using (VOMSEntities)
                //{

                //}
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
            }
        }
    }
}