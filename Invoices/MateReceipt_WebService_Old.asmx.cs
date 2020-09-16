using System;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ServiceModel.Web;
using BAL;
using System.Collections;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for MateReceipt_WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class MateReceipt_WebService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetMateReceiptStatus()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string PInvNo = HttpContext.Current.Request.Params["sSearch_0"];
                string SBNo = HttpContext.Current.Request.Params["sSearch_1"];
                string date = HttpContext.Current.Request.Params["sSearch_2"];
                string MateReceiptNo = HttpContext.Current.Request.Params["sSearch_3"];
                string ForwarderName = HttpContext.Current.Request.Params["sSearch_4"];


                StringBuilder s = new StringBuilder();

                if (PInvNo != "")
                    s.Append(" and dbo.FN_GetPrfmaInvcNumber(MR.RefPInvcID) LIKE '%" + PInvNo + "%'");
                if (SBNo != "")
                    s.Append(" and dbo.FN_GetShpngBilNumber(MR.ShpngBillNmbr) LIKE '%" + SBNo + "%'");
                if (date != null && date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and MR.MRDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (MateReceiptNo != "")
                    s.Append(" and MR.MReceiptNmbr LIKE '%" + MateReceiptNo + "%'");
                if (ForwarderName != "")
                    s.Append(" and MR.ForwarderName LIKE '%" + ForwarderName + "%'");

                s.Append(" and MR.CompanyId = '" + Session["CompanyID"] + "'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE PInvNO LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SBNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Date LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ReceiptNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForwarderName LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                sb.Append(" ");

                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                orderByClause = "0 DESC"; // = sb.ToString();

                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ID ");

                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "ID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"  
                            declare @MAA TABLE(ID uniqueidentifier, PInvNO nvarchar(400), SBNo nvarchar(MAX),Date date, 
                            ReceiptNo nvarchar(MAX), ForwarderName nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX),CreatedDate datetime)
                            INSERT
                            INTO
	                            @MAA ( ID, PInvNO, SBNo, Date, ReceiptNo, ForwarderName, EDIT, Delt, CreatedDate )
	                                SELECT MR.ID,dbo.FN_GetPrfmaInvcNumber(MR.RefPInvcID) as PInvcNmbr,
                                    dbo.FN_GetShpngBilNumber(MR.ShpngBillNmbr), MR.MRDate, MR.MReceiptNmbr,MR.ForwarderName,
                                    '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,''' 
                                    + CONVERT(nvarchar(40), MR.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT,
                                    '<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
                                    + CONVERT(nvarchar(40), MR.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delt, MR.CreatedDate                  
				                    FROM MateReceipt MR INNER JOIN Users u on u.ID = MR.CreatedBy {4}                   

                            SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].PInvNO)
				                              FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].ID
                                           ,[@MAA].PInvNO
			                               ,[@MAA].SBNo      
                                           ,[@MAA].Date
                                           ,[@MAA].ReceiptNo
                                           ,[@MAA].ForwarderName
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt
                                           ,[@MAA].CreatedDate                                                         
		                              FROM
			                              @MAA {1}) RawResults) Results WHERE
                	                     RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                /// For Admin Login DashBoard
                //string ModeWhere = "";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    filteredWhere == "" ? " WHERE " + s.ToString() == "" ? " MR.IsActive <> 0 " : s.ToString() + " AND  MR.IsActive <> 0 "
                    : s.ToString() == "" ? " AND MR.IsActive <> 0 " : s.ToString() + " AND  MR.IsActive <> 0 ");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string PInvNO = data["PInvNO"].ToString().Replace("\"", "\\\"");
                    PInvNO = PInvNO.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string SBNo1 = data["SBNo"].ToString().Replace("\"", "\\\"");
                    SBNo1 = SBNo1.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", SBNo1.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["Date"]);
                    sb.Append(",");


                    string ReceiptNo = data["ReceiptNo"].ToString().Replace("\"", "\\\"");
                    ReceiptNo = ReceiptNo.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", ReceiptNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ForwarderName1 = data["ForwarderName"].ToString().Replace("\"", "\\\"");

                    ForwarderName1 = ForwarderName1.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", ForwarderName1.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");

                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                    sb.Append("},");



                }
                conn.Close();
                // handles zero records
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
