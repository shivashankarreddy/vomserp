using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Text;
using BAL;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for PackingListService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class PackingListService : System.Web.Services.WebService
    {
        #region Packing List Web Services

        /// <summary>
        /// FOR FOREIGN ENQUIRY
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetPackingListDetails()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string ShpmntInvcNmbr = HttpContext.Current.Request.Params["sSearch_0"];
                string PkngLstNmbr = HttpContext.Current.Request.Params["sSearch_1"];
                string Date = HttpContext.Current.Request.Params["sSearch_2"];
                string CstmrName = HttpContext.Current.Request.Params["sSearch_3"];
                string RefFPOs = HttpContext.Current.Request.Params["sSearch_4"];
                string ShpmntPlngNmbr = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (Date != null && Date != "")
                {
                    DateTime FrmDt = Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(Date.Split('~')[0].ToString());
                    DateTime EndDat = Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and p.PkngListNoDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (ShpmntInvcNmbr != "")
                    s.Append(" and ShipmentInvcNmbr LIKE '%" + ShpmntInvcNmbr + "%'");
                if (PkngLstNmbr != "")
                    s.Append(" and p.PkngListNo LIKE '%" + PkngLstNmbr + "%'");
                if (CstmrName != "")
                    s.Append(" and CustomerName LIKE '%" + CstmrName + "%'");
                if (RefFPOs != "")
                    s.Append(" and RefFPOs LIKE '%" + RefFPOs + "%'");
                if (ShpmntPlngNmbr != "")
                    s.Append(" and ChklstRef LIKE '%" + ShpmntPlngNmbr + "%'");
                if (Status != "")
                    s.Append(" and Status LIKE '%" + Status + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE i.ShpmntPrfmaInvcNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR p.PkngListNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR p.PkngListNoDt LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR dbo.GetActiveCustomerName(p.CustomerID) LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR dbo.FN_MergeTableColumnFPO(p.FPOs) LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR c.ChkLstRefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR dbo.FN_GetStatus(i.StatusID) LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                //sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                //sb.Append(" ");
                //sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                orderByClause = "0 DESC"; 

                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", PkingLstID ");
                    //orderByClause = orderByClause.Replace("1", ", ShipmentInvoiceNumber ");
                    //orderByClause = orderByClause.Replace("2", ", PackingListNumber ");
                    //orderByClause = orderByClause.Replace("3", ", Date ");
                    //orderByClause = orderByClause.Replace("4", ", CustomerName ");
                    //orderByClause = orderByClause.Replace("5", ", RefFPOs ");
                    //orderByClause = orderByClause.Replace("6", ", StatusTypeId ");
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

                string query = @"declare @MAA TABLE(PkingLstID uniqueidentifier, ShipmentInvcNmbr nvarchar(400), PkngListNo nvarchar(MAX), PkngListNoDt date, 
                                 CustomerName nvarchar(MAX), RefFPOs nvarchar(MAX), ChkLstRefNo nvarchar(MAX),  
                                Status nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX), CreatedDate datetime)
                                INSERT INTO
                                @MAA (PkingLstID, ShipmentInvcNmbr, PkngListNo, PkngListNoDt, CustomerName, RefFPOs, ChkLstRefNo, Status, EDIT, Delt, CreatedDate)
                                Select PkingLstID, ShipmentInvcNmbr, PkngListNo, PkngListNoDt, CustomerName, RefFPOs, ChklstRef, Status, EDIT, Delt, CreatedDate from View_PackingList p {4}
                                SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber
                                , * FROM (SELECT (SELECT count([@MAA].PkingLstID) FROM @MAA) AS TotalRows
                                , ( SELECT  count( [@MAA].PkingLstID) FROM @MAA ) AS TotalDisplayRows
                                ,[@MAA].PkingLstID
                                ,[@MAA].ShipmentInvcNmbr
                                ,[@MAA].PkngListNo      
                                ,[@MAA].PkngListNoDt
                                ,[@MAA].CustomerName
                                ,[@MAA].RefFPOs     
                                ,[@MAA].ChkLstRefNo
                                ,[@MAA].Status
                                ,[@MAA].EDIT
                                ,[@MAA].Delt
                                ,[@MAA].CreatedDate
                                FROM @MAA) RawResults) Results WHERE
                                RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                Guid CompanyId = new Guid(Session["CompanyID"].ToString());

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        " WHERE p.CompanyId = " + "'" + CompanyId + "'" + (s.ToString() == "" ? " p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'" + " and " : " p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'" + " and " + s.ToString() + "AND") + " p.CustomerID in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "') "
                        : (s.ToString() == "" ? " AND p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'" + " AND " : " AND p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'" + s.ToString() + "AND") + " p.CustomerID in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "')");
                }
                else
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                        filteredWhere == "" ? (" WHERE p.CompanyId = " + "'" + CompanyId + "'" + (s.ToString() == "" ? (" and p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'") : (" and p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'"  +s.ToString())))
                        : (s.ToString() == "" ? (" AND p.IsActive <> 0 and p.CompanyId = " + "'" + CompanyId + "'") : (" and p.IsActive <> 0 and p.CompanyId =" + "'" + CompanyId + "'"  +s.ToString())));
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

                while (data.Read())
                {

                    if (totalRecords.Length == 0)
                    {
                        totalRecords = data["TotalRows"].ToString();
                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    }
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["PkingLstID"].ToString()); 
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string ShipmentInvcNmbr = data["ShipmentInvcNmbr"].ToString().Replace("\"", "\\\"");
                    string PkngListID = data["PkingLstID"].ToString();
                    ShipmentInvcNmbr = ShipmentInvcNmbr.Replace("\t", "-");
                    if (!String.IsNullOrEmpty(ShipmentInvcNmbr) || ShipmentInvcNmbr != "")
                    {
                        sb.AppendFormat(@"""0"": ""<a href=PackingListDetails.aspx?ID={0}>{1}</a>""", PkngListID, ShipmentInvcNmbr.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                        sb.AppendFormat(@"""1"": ""{0}""", data["PkngListNo"].ToString());
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""0"": ""{0}""", ShipmentInvcNmbr);
                        sb.Append(",");
                        sb.AppendFormat(@"""1"": ""<a href=PackingListDetails.aspx?ID={0}>{1}</a>""", PkngListID, data["PkngListNo"].ToString());
                        sb.Append(",");
                    }

                    sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["PkngListNoDt"]);
                    sb.Append(",");

                    string CustomerName = data["CustomerName"].ToString();
                    CustomerName = CustomerName.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CustomerName.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string DeptID = data["RefFPOs"].ToString().Replace("\"", "\\\"");

                    DeptID = DeptID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["ChkLstRefNo"].ToString().Replace("\"", "\\\"");

                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ContactPerson = data["Status"].ToString().Replace("\"", "\\\"");
                    ContactPerson = ContactPerson.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Packing List Service", ex.Message.ToString());
                return "";
            }
        }


        #endregion

        #region Service Methods

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }

        #endregion
    }
}
