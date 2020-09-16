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

namespace VOMS_ERP.Logistics
{
    public partial class HSCode : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        ItemMasterBLL ItmMstrBLL = new ItemMasterBLL();
        static DataSet DSS;
        # endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["HSCodeID"] != null)
                {
                    ArrayList al = new ArrayList();
                    al = (ArrayList)Session["HSCodeID"];
                    txtSpec.Text = al[2].ToString().Replace("~~", "''");
                    Guid ItemID = new Guid(al[1].ToString());
                    BidItemDesc(ItemID);
                }
            }
        }

        private void BidItemDesc(Guid ItemID)
        {
            try
            {
                DSS = new DataSet();
                DataSet ds = ItmMstrBLL.SelectItemMaster(CommonBLL.FlagXSelect, ItemID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DSS.Tables.Add(ds.Tables[0].Copy());
                    ddlItmDescription.DataSource = ds.Tables[0];
                    ddlItmDescription.DataTextField = "Description";
                    ddlItmDescription.DataValueField = "ID";
                    ddlItmDescription.DataBind();
                    txtHSCode.Text = ds.Tables[0].Rows[0]["HSCode"].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "HSCode", ex.Message.ToString());
            }
        }

        protected void ddlItmDescription_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Guid ItemID = new Guid(DSS.Tables[0].Rows[0]["ID"].ToString());
                Guid Catagory = new Guid(DSS.Tables[0].Rows[0]["CategoryID"].ToString());
                string Desc = DSS.Tables[0].Rows[0]["ItemDescription"].ToString();
                string spec = DSS.Tables[0].Rows[0]["Specification"].ToString();
                string partNo = DSS.Tables[0].Rows[0]["PartNumber"].ToString();
                int result = ItmMstrBLL.InsertUpdateItemMaster(CommonBLL.FlagUpdate, ItemID,
                        Catagory,"", Desc, spec, partNo, "", new Guid(Session["UserID"].ToString()), txtHSCode.Text, false, new Guid(Session["CompanyID"].ToString()));
                if (result == 0)
                {
                    ArrayList al = new ArrayList();
                    al = (ArrayList)Session["HSCodeID"];
                    al.Add(txtHSCode.Text.Trim());
                    Session["HSCodeID"] = al;
                    HFSelectedVal.Value = DSS.Tables[0].Rows[0]["ID"].ToString();
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "HSCode", "HS code " + HFSelectedVal.Value + " Updated Successfully.");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll1", "SendSelectedVal();", true);
                }
                else
                {
                    HFSelectedVal.Value = DSS.Tables[0].Rows[0]["ID"].ToString();
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "HSCode", "HS code " + HFSelectedVal.Value + " Error while Updating.");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Updating.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "HSCode", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Updating.');", true);
            }
        }
    }
}
