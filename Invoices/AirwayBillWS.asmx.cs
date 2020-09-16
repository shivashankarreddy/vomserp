using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Text;
using BAL;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for AirwayBillWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AirwayBillWS : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetAirWayBillStat()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string AWBNumber = HttpContext.Current.Request.Params["sSearch_0"];
                string Cust = HttpContext.Current.Request.Params["sSearch_1"];
                string ProformaINVIDs = HttpContext.Current.Request.Params["sSearch_2"];
                string Freight = HttpContext.Current.Request.Params["sSearch_3"];
                string PlaceOfDelivery = HttpContext.Current.Request.Params["sSearch_4"];
            

                StringBuilder s = new StringBuilder();
                //if (ShpngBilDate != null && ShpngBilDate != "")
                //{
                //    DateTime FrmDt = ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ShpngBilDate.Split('~')[0].ToString());
                //    DateTime EndDat = ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ShpngBilDate.Split('~')[1].ToString());
                //    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                //        s.Append(" and ShpngBilDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                //}
                //if (PrfmaInvoiceDate != null && PrfmaInvoiceDate != "")
                //{
                //    DateTime FrmDt = PrfmaInvoiceDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[0].ToString());
                //    DateTime EndDat = PrfmaInvoiceDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[1].ToString());
                //    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                //        s.Append(" and PrfmaInvoiceDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                //}
                if (AWBNumber != "")
                    s.Append(" and ab.AWBNumber LIKE '%" + AWBNumber + "%'");
                if (Cust != "")
                    s.Append(" and ab.Customers LIKE '%" + Cust + "%'");
                if (ProformaINVIDs != "")
                    s.Append(" and ab.ProformaINVIDs LIKE '%" + ProformaINVIDs + "%'");
                if (Freight != "")
                    s.Append(" and ab.Freight LIKE '%" + Freight + "%'");
                if (PlaceOfDelivery != "")
                    s.Append(" and ab.PlaceOfDelivery LIKE '%" + PlaceOfDelivery + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    //sb.Append(" WHERE ShpngBilNmbr LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR ShpngBilDate LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR PrfmaInvoiceNmbr LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR PrfmaInvoiceDate LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR PrtLoadingDes LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR PrtDischargeDes LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR CntryDestinationDes LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR CntryOrigineDes LIKE ");
                    //sb.Append(wrappedSearch);

                    //filteredWhere = sb.ToString();
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

                    orderByClause = orderByClause.Replace("0", ", AWBID ");
                    //orderByClause = orderByClause.Replace("1", ", EnquireNumber ");
                    //orderByClause = orderByClause.Replace("2", ", ReceivedDate ");
                    //orderByClause = orderByClause.Replace("3", ", Subject ");
                    //orderByClause = orderByClause.Replace("4", ", ForeignEnquireId ");
                    //orderByClause = orderByClause.Replace("5", ", StatusTypeId ");
                    //orderByClause = orderByClause.Replace("6", ", DepartmentId ");
                    //orderByClause = orderByClause.Replace("7", ", CusmorId ");
                    //orderByClause = orderByClause.Replace("8", ", MAIL ");
                    //orderByClause = orderByClause.Replace("9", ", EDIT ");
                    //orderByClause = orderByClause.Replace("10", ", Delt ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "AWBID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"  
                            declare @MAA TABLE
                                            (AWBID uniqueidentifier , AWBNumber nvarchar(max) ,Customers nvarchar(max),ProformaINVIDs nvarchar(max),
                                             Freight nvarchar(max),ShpngPrfmaInvcNmbr nvarchar(max),CreatedBy nvarchar(max),PlaceOfDelivery nvarchar(max)
                                             ,EDIT nvarchar(max),Delt nvarchar(max),CompanyId uniqueidentifier, CreatedDate datetime )
                            INSERT
                            INTO
	                            @MAA ( AWBID ,AWBNumber ,Customers ,ProformaINVIDs ,Freight ,ShpngPrfmaInvcNmbr ,CreatedBy,PlaceOfDelivery,EDIT, Delt, CompanyId, CreatedDate)
	                               Select AWBID ,AWBNumber ,Customers ,ProformaINVIDs ,Freight ,ShpngPrfmaInvcNmbr ,CreatedBy,PlaceOfDelivery,EDIT, Delt, CompanyId, CreatedDate from View_AirwayBillStat ab Where ab.IsActive<>0
	                                {4}                   

                            SELECT *
                            FROM 
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].AWBID)
                                        FROM
                                        @MAA) AS TotalRows
                                        , ( SELECT  count( [@MAA].AWBID) FROM @MAA {1}) AS TotalDisplayRows			   
                                        ,[@MAA].AWBID
                                        ,[@MAA].AWBNumber 
                                        ,[@MAA].Customers 
                                        ,[@MAA].ProformaINVIDs 
                                        ,[@MAA].Freight 
                                        ,[@MAA].ShpngPrfmaInvcNmbr 
                                        ,[@MAA].CreatedBy
                                        ,[@MAA].PlaceOfDelivery
                                        ,[@MAA].EDIT
                                        ,[@MAA].Delt
                                        ,[@MAA].CompanyId
                                        ,[@MAA].CreatedDate        
                                        FROM
                                        @MAA {1}) RawResults) Results WHERE
                	                     RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                         filteredWhere == "" ? " WHERE " + s.ToString() == "" ? " ab.IsActive <> 0 and ab.CompanyId = " + "'" + CompanyID + "'" : s.ToString() + " and  ab.IsActive <> 0 and ab.CompanyId = " + "'" + CompanyID + "'"
                         : s.ToString() == "" ? " and ab.CompanyId = " + "'" + CompanyID + "'" : s.ToString() + "and ab.CompanyId = " + "'" + CompanyID + "'");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["AWBID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string AWBNum = data["AWBNumber"].ToString().Replace("\"", "\\\"");
                    string AWBID = data["AWBID"].ToString();
                    AWBNum = AWBNum.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=AirWayBillDetails.Aspx?ID={0}>{1}</a>""", AWBID, AWBNum.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    //sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    //sb.Append(",");

                    string Custmr = data["Customers"].ToString().Replace("\"", "\\\"");
                    Custmr = Custmr.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", Custmr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PrfmaInvoiceDate"]);
                    //sb.Append(",");

                    string PrfrmaINVIDs = data["ProformaINVIDs"].ToString().Replace("\"", "\\\"");
                    PrfrmaINVIDs = PrfrmaINVIDs.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PrfrmaINVIDs.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Freght = data["Freight"].ToString().Replace("\"", "\\\"");
                    Freght= Freght.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Freght.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ShpngPrfmaInvcNmbr = data["PlaceOfDelivery"].ToString().Replace("\"", "\\\"");
                    ShpngPrfmaInvcNmbr = ShpngPrfmaInvcNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", ShpngPrfmaInvcNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("this", IDs);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/INVOICES/ErrorLog"), "SHIPPING BILL DETAILS WebService", ex.Message.ToString());
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
