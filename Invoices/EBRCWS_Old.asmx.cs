using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BAL;
using System.ServiceModel.Web;
using System.Web.Script.Services;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for EBRCWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class EBRCWS : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetEBRCStat()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string ShppngBllNo = HttpContext.Current.Request.Params["sSearch_0"];
                string Date = HttpContext.Current.Request.Params["sSearch_1"];
                string ShppngBillPort = HttpContext.Current.Request.Params["sSearch_2"];
                string BRCNo = HttpContext.Current.Request.Params["sSearch_3"];
                string BrcStat = HttpContext.Current.Request.Params["sSearch_4"];
                string BrcUti = HttpContext.Current.Request.Params["sSearch_5"];
                //string Freight = HttpContext.Current.Request.Params["sSearch_3"];
                //string PlaceOfDelivery = HttpContext.Current.Request.Params["sSearch_4"];


                StringBuilder s = new StringBuilder();
                if (Date != null && Date != "")
                {
                    DateTime FrmDt = Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(Date.Split('~')[0].ToString());
                    DateTime EndDat = Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and Convert(date,ebc.CnDate,103) between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                //if (PrfmaInvoiceDate != null && PrfmaInvoiceDate != "")
                //{
                //    DateTime FrmDt = PrfmaInvoiceDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[0].ToString());
                //    DateTime EndDat = PrfmaInvoiceDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[1].ToString());
                //    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                //        s.Append(" and PrfmaInvoiceDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                //}
                if (ShppngBllNo != "")
                    s.Append(" and ebc.CnShpngBilNm LIKE '%" + ShppngBllNo + "%'");
                if (ShppngBillPort != "")
                    s.Append(" and ebc.CnShpngBilPrt LIKE '%" + ShppngBillPort + "%'");
                if (BRCNo != "")
                    s.Append(" and ebc.BRCNo LIKE '%" + BRCNo + "%'");
                if (BrcStat != "")
                    s.Append(" and ebc.BRCStatus LIKE '%" + BrcStat + "%'");
                if (BrcUti != "")
                    s.Append(" and ebc.BRCUtiliseStat LIKE '%" + BrcUti + "%'");
                //if (Freight != "")
                //    s.Append(" and ab.Freight LIKE '%" + Freight + "%'");
                //if (PlaceOfDelivery != "")
                //    s.Append(" and ab.PlaceOfDelivery LIKE '%" + PlaceOfDelivery + "%'");

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

                    orderByClause = orderByClause.Replace("0", ", CnDate ");
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
                    orderByClause = "CreatedDate Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"declare @MAA TABLE (BRCID  uniqueidentifier,CnShpngBilNm nvarchar(max) ,CnDate datetime,ShpingBillPort nvarchar(max), 
                                CnShpngBilPrt nvarchar(max),BRCNo nvarchar(max), BRCStatus nvarchar(15), BRCUtiliseStat nvarchar(15), CreatedBy nvarchar(max), 
                                EDIT nvarchar(max),Delt nvarchar(max), CreatedDate datetime) 
                                INSERT INTO 
                                @MAA ( BRCID ,CnShpngBilNm ,CnDate ,ShpingBillPort,CnShpngBilPrt ,BRCNo,BRCStatus ,BRCUtiliseStat,CreatedBy,EDIT, Delt, CreatedDate)
	                            select ebc.BRCID,ebc.CnShpngBilNm,CONVERT(date, ebc.CnDate,103) CnDate,ebc.ShpingBillPort, ebc.CnShpngBilPrt, ebc.BRCNo, 
                                ebc.BRCStatus, ebc.BRCUtiliseStat, ebc.CreatedBy, '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+ 
                                CONVERT(nvarchar(40), ebc.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
                                '<img src=../images/delete.png alt=Delete onclick=Delet(this,''' + CONVERT(nvarchar(40), ebc.CreatedBy) +''',' 
                                +(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delt, ebc.CreatedDate from [view_E_BRC] ebc inner 
                                join Users u on u.ID = ebc.CreatedBy where ebc.IsActive <> 0 {4} 
                                SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
	                             FROM (SELECT (SELECT count([@MAA].BRCID) FROM @MAA) AS TotalRows, 
                                (SELECT  count( [@MAA].BRCID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].BRCID, [@MAA].CnShpngBilNm,
                                [@MAA].CnDate, [@MAA].CnShpngBilPrt, [@MAA].BRCNo, [@MAA].BRCStatus, [@MAA].BRCUtiliseStat, [@MAA].CreatedBy, 
                                [@MAA].EDIT, [@MAA].Delt, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results WHERE 
                                RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                         filteredWhere == "" ? " WHERE " + s.ToString() == "" ? " ebc.IsActive <> 0 " : s.ToString()
                         + " AND  ebc.IsActive <> 0 and ebc.CompanyId = '" + Session["CompanyID"] + "'"
                         : s.ToString() == "" ? "" : s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["BRCID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string CnShpngBilNm = data["CnShpngBilNm"].ToString().Replace("\"", "\\\"");
                    string BRCID = data["BRCID"].ToString();
                    CnShpngBilNm = CnShpngBilNm.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{1}""", BRCID, CnShpngBilNm.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    //sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    //sb.Append(",");

                    //string Custmr = data["CnDate"].ToString().Replace("\"", "\\\"");
                    //Custmr = Custmr.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["CnDate"]);
                    sb.Append(",");

                    //sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PrfmaInvoiceDate"]);
                    //sb.Append(",");

                    string CnShpngBilPrt = data["CnShpngBilPrt"].ToString().Replace("\"", "\\\"");
                    CnShpngBilPrt = CnShpngBilPrt.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", CnShpngBilPrt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string BRCNum = data["BRCNo"].ToString().Replace("\"", "\\\"");
                    BRCNum = BRCNum.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", BRCNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string BRCStatus = data["BRCStatus"].ToString().Replace("\"", "\\\"");
                    BRCStatus = BRCStatus.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", BRCStatus.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string BRCUtiliseStat = data["BRCUtiliseStat"].ToString().Replace("\"", "\\\"");
                    BRCUtiliseStat = BRCUtiliseStat.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", BRCUtiliseStat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("this", IDs);
                    sb.AppendFormat(@"""7"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));

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
