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
using System.Text;

namespace VOMS_ERP.Logistics
{
    public partial class UC_GDN_Approve : System.Web.UI.UserControl
    {
        # region Variables
        static int GdnId;
        static string mailid;
        ErrorLog ELog = new ErrorLog();

        public delegate void ApprovedType();
        public event ApprovedType ButtonClicked;
        # endregion
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ModalPopupExtender1.Hide();
        }

        protected void btnTerminate_Click(object sender, EventArgs e)
        {
            int res = 1;
            try
            {
                IsApprovedBLL IABLL = new IsApprovedBLL();
                res = IABLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(GdnId.ToString()), CommonBLL.IsRejected, txtComment.Text.Trim(), 
                    new Guid(Session["UserID"].ToString()));
                if (res == 0)
                {
                    //SendMail();
                    ModalPopupExtender1.Hide();
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "GDN Status PopUp", "Rejected " + GdnId + " Successfully.");
                    ButtonClicked();
                }
                else
                {
                    ModalPopupExtender1.Show();
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Status PopUp", "Error While Rejecting " + GdnId + " .");
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage1('Error While Rejecting.');", true);
                }
            }
            catch (Exception ex)
            {
                ModalPopupExtender1.Show();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "GDN Status PopUp", ex.Message.ToString());
            }
        }

        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            ModalPopupExtender1.Hide();
        }

        # region Methods

        public void Show(int GDNID,string MailID)
        {
            GdnId = GDNID;
            mailid = MailID;
            ModalPopupExtender1.Show();
        }

        private void SendMail()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GDN-" + GdnId + " Created By You was Rejected. <br/>");
            sb.Append("<br/>");
            sb.Append("Thanks and Regards <br/>");
            sb.Append(Session["TLMailID"].ToString());
            string Sent = CommonBLL.SendApprovalMails(mailid, "", "Rejected", sb.ToString());
        }

        # endregion
    }
}