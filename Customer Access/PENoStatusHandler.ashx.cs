using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BAL;
using System.Configuration;

namespace VOMS_ERP.Customer_Access
{
    /// <summary>
    /// Summary description for PENoStatusHandler
    /// </summary>
    public class PENoStatusHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            ErrorLog ELog = new ErrorLog();
            FEnquiryBLL FEBLL = new FEnquiryBLL();
            try
            {
                string Status = "";
                int res = 1;
                string ConString = ConfigurationManager.ConnectionStrings["Constring"].ToString();
                context.Response.ContentType = "text/plain";
                Guid EnqID = new Guid(context.Request["id"]);
                int rowID = Convert.ToInt32(context.Request["rowId"]);
                int columnPosition = Convert.ToInt32(context.Request["columnPosition"]);
                int columnId = Convert.ToInt32(context.Request["columnId"]);
                string value = context.Request["value"].ToString().Trim();
                switch (columnId)
                {
                    case 1:
                        {
                            Status = (value == "Select" ? "" : value) ;
                            res = FEBLL.IsRegretStatus(CommonBLL.FlagUpdate, EnqID, 0, false, Status);
                        } break;
                    
                    default: throw new Exception("nothing saved");
                }

                if (res == 0)
                    context.Response.Write(value);
                else
                    context.Response.Write("");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(context.Server.MapPath("../Logs/Enquiries/ErrorLog"), "PE Number Status", ex.Message.ToString());
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