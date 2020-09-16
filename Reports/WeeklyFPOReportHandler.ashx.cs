using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BAL;
using System.Configuration;

namespace VOMS_ERP.Reports
{
    /// <summary>
    /// Summary description for WeeklyFPOReportHandler
    /// </summary>
    public class WeeklyFPOReportHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ErrorLog ELog = new ErrorLog();
            WeeklyReportsBLL we = new WeeklyReportsBLL();

            try
            {
                string Remarks = "", CurrStat = "", PrevStat = "";
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
                    case 10: PrevStat = value; break;
                    case 11: CurrStat = value; break;
                    case 12: Remarks = value; break;
                    default: throw new Exception("nothing saved");
                }
                if (value != "")
                    res = we.InsertUpdateDelete(CommonBLL.FlagBSelect, Guid.Empty, FPOID, FPOID, Remarks, CurrStat, PrevStat, DateTime.Now, DateTime.Now, Guid.Empty,
                                  DateTime.Now, Guid.Empty, DateTime.Now, true);
                if (res == 0)
                    context.Response.Write(value);
                else
                    context.Response.Write("");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(context.Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Reports", ex.Message.ToString());
                context.Response.Write("");
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