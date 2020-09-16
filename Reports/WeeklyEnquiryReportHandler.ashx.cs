using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using BAL;
using System.Data.SqlClient;
using System.Data;

namespace VOMS_ERP.Reports
{
    /// <summary>
    /// Summary description for WeeklyEnquiryReportHandler
    /// </summary>
    public class WeeklyEnquiryReportHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ErrorLog ELog = new ErrorLog();
            WeeklyReportsBLL we = new WeeklyReportsBLL();
            try
            {
                string Remarks = "", CurrStat = "";
                int res = 1;

                string ConString = ConfigurationManager.ConnectionStrings["Constring"].ToString();

                context.Response.ContentType = "text/plain";

                Guid FeID = new Guid(context.Request["id"]);
                int rowID = Convert.ToInt32(context.Request["rowId"]);
                int columnPosition = Convert.ToInt32(context.Request["columnPosition"]);
                int columnId = Convert.ToInt32(context.Request["columnId"]);
                string value = context.Request["value"].ToString().Trim();

                switch (columnId)
                {
                    case 7: CurrStat = value; break;
                    case 8: Remarks = value; break;
                    default: throw new Exception("nothing saved");
                }
                if (value != "")
                    res = we.InsertUpdateDelete(CommonBLL.FlagASelect, FeID, Guid.Empty, Guid.Empty, Remarks, CurrStat, "", DateTime.Now, DateTime.Now, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true);
                if (res == 0)
                    context.Response.Write(value);
                else
                    context.Response.Write("");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(context.Server.MapPath("../Logs/Reports/ErrorLog"), "Weekly Enquiry Report Handler", ex.Message.ToString());
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