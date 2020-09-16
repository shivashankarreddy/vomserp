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
using System.Net.Mail;
using BAL;

namespace VOMS_ERP.Masters
{
    public partial class Mails : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        EMailsDetailsBLL EMDBL = new EMailsDetailsBLL();
        # endregion


        #region Deafult Page Load Event
        /// <summary>
        /// Deafault Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        //btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Mail Master", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// This Method is used to Clear Controls
        /// </summary>
        private void ClearItems()
        {
            try
            {
                txtfrom.Text = "";
                txtpwd.Text = "";
                txtto.Text = "";
                txtsub.Text = "";
                txtcc.Text = "";
                txtbody.Text = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Mail Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used to SendMail with Attachment
        /// </summary>
        protected void SendMail()
        {
            try
            {
                string[] Rcpts = txtto.Text.Trim(',').Split(',');
                string[] Cc = txtcc.Text.Trim(',').Split(',');
                string[] Attachments = null;
                string atn = "";
                if (fileUpload1.HasFile)
                {
                    fileUpload1.SaveAs(MapPath("~/uploads/") + fileUpload1.FileName);
                    Attachments = new string[1];
                    Attachments[0] = MapPath("~/uploads/") + fileUpload1.FileName;
                }
                int res = 1;
                string Rslt = CommonBLL.SendMails(txtfrom.Text.Trim(), txtpwd.Text.Trim(), Rcpts, Cc, "", txtsub.Text.Trim(),
                    txtbody.Text.Trim(), Attachments);
                if (Rslt == "Sent")
                {
                    atn = fileUpload1.FileName;
                    res = EMDBL.InsertUpdateDeleteEMailDetails(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(Session["UserID"].ToString()), txtfrom.Text,
                        txtto.Text, txtcc.Text.Trim(), "", txtsub.Text.Trim(), txtbody.Text.Trim(), DateTime.Now, "GEN", atn, new Guid(Session["UserID"].ToString()),
                        new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Mail Sent Successfully.');", true);
                        ClearItems();
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/Log"), "Mail Master", "Mail Inserted Successfully in Mail Master.");
                        Response.Redirect("../Masters/SentMailDetails.aspx", false);
                    }
                    else
                        ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID,
                            "alert('Mail Not Send Please Check your input Detials/" + Rslt + "');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), UniqueID,
                        "alert('Mail Not Send Please Check your input Detials/" + Rslt + "');", true);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Mail Master", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error While Sending Mail...!');", true);
            }
        }

        # endregion

        # region ButtonClicks
        /// <summary>
        /// This Method is used to Send MAil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtfrom.Text.Trim() != "" && txtpwd.Text.Trim() != "" && txtto.Text.Trim() != "")
                    SendMail();
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Password, FromMial & ToMail are Manditory.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Mail Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This method is used to clear controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearItems();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Mail Master", ex.Message.ToString());
            }
        }

        # endregion
    }
}
