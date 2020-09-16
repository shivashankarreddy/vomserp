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
    public partial class UC_Action : System.Web.UI.UserControl
    {
        # region Variables
        public delegate void SelectedHandler(string Action);
        public event SelectedHandler ButtonClicked;
        ErrorLog ELog = new ErrorLog();
        static string PageType = "";
        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        # region Button Clicks
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ModalPopupExtender1.Hide();
            //PerformActions("Cancel", Convert.ToInt64(HFPinvID.Value), Convert.ToInt64(HFCT1ID.Value));
        }

        protected void btnReApplicable_Click(object sender, EventArgs e)
        {
            try
            {
                if (PageType == "")
                    PerformActions("ReApplicable", HFPinvID.Value, HFCT1ID.Value);
                else if (PageType == "SevCU")                
                    PerformActionsSevUpdate("ReApplicable");                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "" + PageType == "" ? "CT-1 Status" : "Sevottam Update" + " (User Control)", ex.Message.ToString());
            }
        }

        protected void btnTerminate_Click(object sender, EventArgs e)
        {
            try
            {
                if (PageType == "")
                    PerformActions("Terminate", HFPinvID.Value, HFCT1ID.Value);
                else if (PageType == "SevCU")                
                    PerformActionsSevUpdate("Terminate");                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "" + PageType == "" ? "CT-1 Status" : "Sevottam Update" + " (User Control)", ex.Message.ToString());
            }
        }

        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            ModalPopupExtender1.Hide();
        }

        # endregion

        # region Methods

        public void Show(string PinvID, string CT1ID,string PgType)
        {
            PageType = PgType;
            HFPinvID.Value = PinvID;
            HFCT1ID.Value = CT1ID;
            ModalPopupExtender1.Show();
        }
       
        public void PerformActions(string Status, string PnvID, string CT1ID)
        {
            try
            {
                int resVal = 1;
                CT1DetailsBLL CT1BLL = new CT1DetailsBLL();
                resVal = CT1BLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(CT1ID), new Guid(PnvID), Guid.Empty, Guid.Empty, Guid.Empty, 0, 0, "", DateTime.Now, 0, Status, new Guid(Session["UserID"].ToString()), DateTime.Now,
                    new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                if (resVal == 0)
                    ButtonClicked(Status);
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "alert('Unable to Perform Action Please try again.')", true);
                    ButtonClicked(Status);
                }

                //if (PageType == "SevCU")//Sev_Cancel_Update
                //{ 

                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Status (User Control)", ex.Message.ToString());
            }
        }


        public void PerformActionsSevUpdate(string Status)
        {
            try
            {
                int resVal = 1;
                string[] Ct1IDs = HFCT1ID.Value.Split(',');
                Ct1IDs = Ct1IDs.Where(s => !String.IsNullOrEmpty(s)).ToArray();
                for (int i = 0; i < Ct1IDs.Length; i++)
                {                    
                    CT1DetailsBLL CT1BLL = new CT1DetailsBLL();
                    resVal = CT1BLL.InsertUpdateDelete(CommonBLL.FlagVUpdate, new Guid(Ct1IDs[i]), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 0, 0, "", DateTime.Now, 0, Status, new Guid(Session["UserID"].ToString()), DateTime.Now,
                        new Guid(Session["UserID"].ToString()), DateTime.Now, true);
                }
                if (resVal == 0)
                    Response.Redirect("../Logistics/SevottamStatus.aspx", false);
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "alert('Unable to Perform Action Please try again.')", true);
                    ButtonClicked(Status);
                }
                ModalPopupExtender1.Hide();                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status (User Control)", ex.Message.ToString());
            }
        }




        # endregion
    }
}