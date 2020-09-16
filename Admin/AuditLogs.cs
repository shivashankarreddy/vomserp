using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Org.BouncyCastle.Asn1.Ocsp;
using BAL;

namespace VOMS_ERP.Admin
{
    public class AuditLogs
    {
        #region Varaibles
        AuditLogBLL AUBLL = new AuditLogBLL();
        EmailBodyBLL EBLL = new EmailBodyBLL();
        #endregion

        #region Audit Log for Insert
        
        public void AuditLog(int result,String BttnText,String EnqNo,String Actvty,Guid UserId,Guid CompanyId,String FileName)
        {
            int res = 0; DataSet dss = new DataSet(); Guid ScreenId = Guid.Empty; string Activity = "";
            //string Url = Request.Url.AbsolutePath;
            //Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            //string filename = "";
            //filename = Path.GetFileName(uri.AbsolutePath);
            dss = EBLL.GetEmBdyDetails(CommonBLL.FlagBSelect, Guid.Empty, CompanyId, Guid.Empty, FileName, ""
                            , UserId, DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
            if (dss != null && dss.Tables != null && dss.Tables[0].Rows.Count > 0 && dss.Tables[0].Rows[0]["ID"].ToString() != null)
            {
                ScreenId = new Guid(dss.Tables[0].Rows[0]["ID"].ToString());
            }

            if (result == 0 && BttnText == "Save")
                Activity = Actvty + EnqNo + " " + CommonBLL.SuccessMsg;
            else if (result == 0 && BttnText == "Update")
                Activity = Actvty + EnqNo + " " + CommonBLL.SuccessMsgUpdate;
            else if (result != 0 && BttnText == "Save")
                Activity = CommonBLL.ErrorMsg + " " + Actvty + EnqNo;
            else
                Activity = CommonBLL.ErrorMsgUpdate + " " + Actvty + EnqNo;
            res = AUBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, UserId, ScreenId,
                           DateTime.Now.Date.ToString(), DateTime.Now.ToString("H:mm:ss"), Activity, DateTime.Now, true, CompanyId);
        }

        #endregion

        #region Audit Log for Login

        public void AuditLogForLogin(int result, String BttnText, String EnqNo, String Actvty, Guid UserId, Guid CompanyId, String FileName)
        {
            int res = 0; DataSet dss = new DataSet(); Guid ScreenId = Guid.Empty; string Activity = "";
            //string Url = Request.Url.AbsolutePath;
            //Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            //string filename = "";
            //filename = Path.GetFileName(uri.AbsolutePath);
            //dss = EBLL.GetEmBdyDetails(CommonBLL.FlagBSelect, Guid.Empty, CompanyId, Guid.Empty, FileName, ""
            //                , UserId, DateTime.Now, Guid.Empty, DateTime.Now, true, "", Guid.Empty);
            //if (dss != null && dss.Tables != null && dss.Tables[0].Rows[0]["ID"].ToString() != null)
            //{
            //    ScreenId = new Guid(dss.Tables[0].Rows[0]["ID"].ToString());
            //}

            if (result == 0 && BttnText == "Submit")
                Activity = Actvty + " " + CommonBLL.LoginSucessMsg;
            else
                Activity =  Actvty + " " + CommonBLL.LogoutSucessMsg;
            res = AUBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, UserId, ScreenId,
                           DateTime.Now.Date.ToString(), DateTime.Now.ToString("H:mm:ss"), Activity, DateTime.Now, true, CompanyId);
        }

        #endregion
    }
}