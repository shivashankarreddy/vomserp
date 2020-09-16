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
    /// Summary description for StatusFeRecevdHandler
    /// </summary>
    public class StatusFeRecevdHandler : IHttpHandler , IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            ErrorLog ELog = new ErrorLog();
            FEnquiryBLL FEBLL = new FEnquiryBLL();
            try
            {
                string Remarks = "" ;
                bool IsRegret = false ;
                int Status = 10;
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
                    case 7: 
                        { 
                            IsRegret = Convert.ToBoolean(value == "Select" ? false : (value == "UnRegret" ? false : true)); 
                            Status = (value == "Select" ? 10 : (value == "UnRegret" ? 10 : 9));
                            res = FEBLL.IsRegretStatus(CommonBLL.FlagCSelect, EnqID, Status, IsRegret, Remarks);
                        } break;
                    case 8:
                        {
                            Remarks = value;
                            res = FEBLL.IsRegretStatus(CommonBLL.FlagDelete, EnqID, Status, IsRegret, Remarks); //Flage D for update Remarks
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