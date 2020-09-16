using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BAL;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VOMS_ERP.Admin
{
    /// <summary>
    /// Summary description for CVMapping1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class CVMapping1 : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
        public string GetMappings()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string VendorName = HttpContext.Current.Request.Params["sSearch_0"];
                string CustomerName = HttpContext.Current.Request.Params["sSearch_1"];

                StringBuilder s = new StringBuilder();

                if (VendorName != "")
                    s.Append(" and dbo.FN_GetVendorBussName(VendorId) LIKE '%" + VendorName + "%'");
                if (CustomerName != "")
                    s.Append(" and dbo.FN_MergeCustomer_Mapping(VendorId) LIKE '%" + CustomerName + "%'");


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE VendorName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                sb.Append(" ");
                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", CreatedDate ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "CreatedDate DESC";
                orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"declare @MAA TABLE(VendorId uniqueidentifier, VendorName nvarchar(max),CustomerName nvarchar(max), EDIT nvarchar(MAX), Delt nvarchar(MAX), CreatedDate nvarchar(max))
                             INSERT INTO @MAA (VendorId, VendorName,CustomerName,EDIT,Delt,CreatedDate)
                            SELECT distinct m.VendorId,dbo.FN_GetVendorBussName(VendorId) VendorName, dbo.FN_MergeCustomer_Mapping(VendorId) CustomerName,
                            '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this) />' EDIT, 
                            '<img src=../images/delete.png alt=Delete onclick=Delet(this) />' Delt, convert(varchar(12),m.CreatedDate)
                             FROM Mapping m 
                             {1}{4}
                             SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0} ) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].VendorId)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].VendorId) FROM @MAA) AS TotalDisplayRows
                                           ,[@MAA].VendorId     			   
			                               ,[@MAA].VendorName
                                           ,[@MAA].Customername
			                               ,[@MAA].EDIT
                                           ,[@MAA].Delt
                                           ,[@MAA].CreatedDate         
		                              FROM
			                              @MAA) RawResults) Results 
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";


                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    
                   ("Where m.IsActive <> 0 ") : " Where m.IsActive <> 0 " + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["VendorId"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string VenName = data["VendorName"].ToString().Replace("\"", "\\\"");
                    string MappID = data["VendorId"].ToString().Replace("\"", "\\\"");
                    VenName = VenName.Replace("\t", "-"); // 
                    sb.AppendFormat(@"""0"": ""{0}""", VenName.Replace(Environment.NewLine, "\\n")); ; // New Line
                    sb.Append(",");

                    string CustName = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    CustName = CustName.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", CustName.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");

                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping Status", ex.Message.ToString());
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
