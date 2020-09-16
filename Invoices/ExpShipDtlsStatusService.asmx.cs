using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using BAL;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for ExpShipDtlsStatusService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ExpShipDtlsStatusService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetCINVStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string CommInNo = HttpContext.Current.Request.Params["sSearch_0"];

                string PrInvNo = HttpContext.Current.Request.Params["sSearch_1"];
                string PrInvDt = HttpContext.Current.Request.Params["sSearch_2"];


                StringBuilder s = new StringBuilder();

                if (PrInvDt != "")
                {
                    PrInvDt = PrInvDt.Replace("'", "''");
                    DateTime FrmDt = PrInvDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PrInvDt.Split('~')[0].ToString());
                    DateTime EndDat = PrInvDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PrInvDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and PerfInvDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (PrInvNo != "")
                    s.Append(" and PerfInvNo LIKE '%" + PrInvNo.Replace("'", "''") + "%'");


                if (CommInNo != "")
                    s.Append(" and im.CommercialInvNo LIKE '%" + CommInNo.Replace("'", "''") + "%'");


                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", CommercialInvID ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "CreatedDate Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
                							declare @MAA TABLE(CommercialInvID Uniqueidentifier,CommercialInvNo nvarchar(MAX), PrfmInvcNo nvarchar(MAX), PrfmaInvcDt date, 
                											EDIT nvarchar(MAX),Delt nvarchar(MAX), CreatedDate datetime)
                							INSERT
                							INTO
                								@MAA (CommercialInvID,CommercialInvNo,PrfmInvcNo,PrfmaInvcDt, Edit, Delt, CreatedDate)
                								Select CommercialInvID,CommercialInvNo,PerfInvNo,PerfInvDate,Edit, Delt, CreatedDate from View_ExportShipDtls im
                								{4}                   
                							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].CommercialInvID) FROM @MAA) AS TotalRows
                										   , ( SELECT  count( [@MAA].CommercialInvID) FROM @MAA {1}) AS TotalDisplayRows			   
                										   ,[@MAA].CommercialInvID
                										   ,[@MAA].CommercialInvNo   
                										   ,[@MAA].PrfmInvcNo
                										   ,[@MAA].PrfmaInvcDt
                										   
                										   ,[@MAA].EDIT,[@MAA].Delt
                										   ,[@MAA].CreatedDate 
                									  FROM
                										  @MAA {1}) RawResults) Results 
                											
                											WHERE
                												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                Guid CompanyId = new Guid(Session["CompanyID"].ToString());
                string where = " order by im.CreatedDate desc ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    s.ToString() == "" ? " where im.IsActive <> 0 and im.CompanyId = " + "'" + CompanyId + "'" + where : " where im.IsActive <> 0 and im.CompanyId = " + "'" + CompanyId + "'" + s.ToString() + "  " + where);
                s.Clear();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var DB = new SqlCommand();
                DB.Connection = conn;
                DB.CommandText = query;
                var data = DB.ExecuteReader();

                var totalDisplayRecords = "";
                var totalRecords = "";
                string outputJson = string.Empty;

                var rowClass = "";
                var count = 0;

                while (data.Read())
                {
                    if (totalRecords.Length == 0)
                    {
                        totalRecords = data["TotalRows"].ToString();
                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    }
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CommercialInvID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string CinvNO = data["CommercialInvNo"].ToString().Replace("\"", "\\\"");
                    //string IOMId = data["IOM2ID"].ToString().Replace("\"", "\\\"");
                    //string PinvId = data["PrfmaInvcID"].ToString().Replace("\"", "\\\"");
                    CinvNO = CinvNO.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", CinvNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["CommercialInvoiceDate"]);
                    //sb.Append(",");

                    string Pinv = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", Pinv.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");

                    //string CNm = data["Vessel"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""4"": ""{0}""", CNm.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string FPo = data["Sbno"].ToString().Replace("\"", "\\\"");
                    //FPo = FPo.Replace("\t", "-");
                    //sb.AppendFormat(@"""5"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string ChkLst = data["Blno"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""6"": ""{0}""", ChkLst.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                    sb.Append("},");

                }
                conn.Close();
                if (totalRecords.Length == 0)
                {
                    sb.Append("{");
                    sb.Append(@"""sEcho"": ");
                    sb.AppendFormat(@"""{0}""", sEcho);
                    sb.Append(",");
                    sb.Append(@"""iTotalRecords"": 0");
                    sb.Append(",");
                    sb.Append(@"""iTotalDisplayRecords"": 0");
                    sb.Append(", ");
                    sb.Append(@"""aaData"": [ ");
                    sb.Append("]}");
                    outputJson = sb.ToString();

                    return outputJson;
                }
                outputJson = sb.Remove(sb.Length - 1, 1).ToString();
                sb.Clear();

                sb.Append("{");
                sb.Append(@"""sEcho"": ");
                sb.AppendFormat(@"""{0}""", sEcho);
                sb.Append(",");
                sb.Append(@"""iTotalRecords"": ");
                sb.Append(totalRecords);
                sb.Append(",");
                sb.Append(@"""iTotalDisplayRecords"": ");
                sb.Append(totalDisplayRecords);
                sb.Append(", ");
                sb.Append(@"""aaData"": [ ");
                sb.Append(outputJson);
                sb.Append("]}");
                outputJson = sb.ToString();

                return outputJson;
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "LPO Status WebService", ex.Message.ToString());
                return "";
            }
        }


        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }
    }


}
