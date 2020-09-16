using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BAL;
using System.Configuration;
using System.Web.SessionState;

namespace VOMS_ERP.Reports
{
    /// <summary>
    /// Summary description for FPOAwaitingHandler
    /// </summary>
    public class FPOAwaitingHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            ErrorLog ELog = new ErrorLog();
            FEnquiryBLL FEBLL = new FEnquiryBLL();
            try
            {
                string Remarks = "A", IsCancel_Hold = "";
                int res = 1;

                string ConString = ConfigurationManager.ConnectionStrings["Constring"].ToString();

                context.Response.ContentType = "text/plain";

                Guid FPOID = new Guid(context.Request["id"]);
                int rowID = Convert.ToInt32(context.Request["rowId"]);
                int columnPosition = Convert.ToInt32(context.Request["columnPosition"]);
                int columnId = Convert.ToInt32(context.Request["columnId"]);
                string value = context.Request["value"].ToString().Trim();

                switch (columnId)
                {
                    case 6: IsCancel_Hold = value; break;
                    case 7: Remarks = value; break;
                    default: throw new Exception("nothing saved");
                }

                res = FEBLL.VUpdateEnqiryHead(CommonBLL.FlagVUpdate, FPOID, new Guid(context.Session["UserID"].ToString()), IsCancel_Hold, Remarks);
                if (res == 0)
                    context.Response.Write(value);
                else
                    context.Response.Write("");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(context.Server.MapPath("../Logs/Reports/ErrorLog"), "FPO Awaiting Reports", ex.Message.ToString());
                context.Response.Write(ErrMsg);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}