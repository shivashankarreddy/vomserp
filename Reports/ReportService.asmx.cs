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

namespace VOMS_ERP.Reports
{
    /// <summary>
    /// Summary description for ReportService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ReportService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetOpeningEPCopies()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string P_CommercialInvoiceNo = HttpContext.Current.Request.Params["sSearch_0"];
                string P_CommercialInvoiceDate = HttpContext.Current.Request.Params["sSearch_1"];
                string P_PrfmInvcNo = HttpContext.Current.Request.Params["sSearch_2"];
                string P_PrfmaInvcDt = HttpContext.Current.Request.Params["sSearch_3"];
                string P_AWL_BL = HttpContext.Current.Request.Params["sSearch_4"];
                string P_AWL_BL_Date = HttpContext.Current.Request.Params["sSearch_5"];
                string P_ShpngBilNmbr = HttpContext.Current.Request.Params["sSearch_6"];
                string P_ShpngBilDate = HttpContext.Current.Request.Params["sSearch_7"];
                string P_SHIPPING_BILL_STATUS = HttpContext.Current.Request.Params["sSearch_8"];
                string P_No_OF_PAGES = HttpContext.Current.Request.Params["sSearch_9"];
                string P_No_OF_ARE_FORMS = HttpContext.Current.Request.Params["sSearch_10"];
                string P_ARE_FORM_STATUS = HttpContext.Current.Request.Params["sSearch_11"];
                string P_LOAD_PORT = HttpContext.Current.Request.Params["sSearch_12"];
                string P_DISCHARGE_PORT = HttpContext.Current.Request.Params["sSearch_13"];
                string P_CHA_AGENT = HttpContext.Current.Request.Params["sSearch_14"];

                StringBuilder s = new StringBuilder();
                if (P_CommercialInvoiceNo != "")
                    s.Append(" and [CommercialInvoiceNo] LIKE '%" + P_CommercialInvoiceNo + "%'");
                if (P_CommercialInvoiceDate != "")
                {
                    DateTime FrmDt = P_CommercialInvoiceDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_CommercialInvoiceDate.Split('~')[0].ToString());
                    DateTime EndDat = P_CommercialInvoiceDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_CommercialInvoiceDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [CommercialInvoiceDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (P_PrfmInvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + P_PrfmInvcNo + "%'");
                if (P_PrfmaInvcDt != "")
                {
                    DateTime FrmDt = P_PrfmaInvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_PrfmaInvcDt.Split('~')[0].ToString());
                    DateTime EndDat = P_PrfmaInvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_PrfmaInvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_ShpngBilNmbr != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + P_ShpngBilNmbr + "%'");
                if (P_ShpngBilDate != "")
                {
                    DateTime FrmDt = P_ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = P_ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_AWL_BL != "")
                    s.Append(" and [AWL/BL] LIKE '%" + P_AWL_BL + "%'");
                if (P_AWL_BL_Date != "")
                {
                    DateTime FrmDt = P_AWL_BL_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_AWL_BL_Date.Split('~')[0].ToString());
                    DateTime EndDat = P_AWL_BL_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_AWL_BL_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [AWL/BL Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_SHIPPING_BILL_STATUS != "")
                    s.Append(" and [SHIPPING BILL STATUS] LIKE '%" + P_SHIPPING_BILL_STATUS + "%'");
                if (P_No_OF_PAGES != "")
                    s.Append(" and [No. OF PAGES] LIKE '%" + P_No_OF_PAGES + "%'");
                if (P_No_OF_ARE_FORMS != "")
                    s.Append(" and [No. OF ARE-1 FORMS] LIKE '%" + P_No_OF_ARE_FORMS + "%'");
                if (P_ARE_FORM_STATUS != "")
                    s.Append(" and [ARE-1 FORM STATUS] LIKE '%" + P_ARE_FORM_STATUS + "%'");
                if (P_LOAD_PORT != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + P_LOAD_PORT + "%'");
                if (P_DISCHARGE_PORT != "")
                    s.Append(" and [DISCHARGE PORT] LIKE '%" + P_DISCHARGE_PORT + "%'");
                if (P_CHA_AGENT != "")
                    s.Append(" and [CHA AGENT] LIKE '%" + P_CHA_AGENT + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [CommercialInvoiceNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CommercialInvoiceDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [SHIPPING BILL STATUS] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [No. OF PAGES] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [No. OF ARE-1 FORMS] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ARE-1 FORM STATUS] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [DISCHARGE PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CHA AGENT] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", CommercialInvoiceNo");
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
							declare @MAA TABLE([CommercialInvoiceNo] nvarchar(500),[CommercialInvoiceDate] datetime,[PrfmInvcNo] nvarchar(500),
												[PrfmaInvcDt] datetime,[ShpngBilNmbr] nvarchar(500),[ShpngBilDate] datetime,
												[AWL/BL] nvarchar(500),[AWL/BL Date] datetime,[SHIPPING BILL STATUS] varchar(Max), 
												[No. OF PAGES] varchar(1),[No. OF ARE-1 FORMS] varchar(1),[ARE-1 FORM STATUS] varchar(Max),
												[LOAD PORT] nvarchar(max),[DISCHARGE PORT] nvarchar(max),[CHA AGENT] nvarchar(max), [SBID] uniqueidentifier, [CreatedDate] datetime)
							INSERT
							INTO
								@MAA ([CommercialInvoiceNo],[CommercialInvoiceDate],[PrfmInvcNo],[PrfmaInvcDt],[ShpngBilNmbr],[ShpngBilDate],
									[AWL/BL],[AWL/BL Date],[SHIPPING BILL STATUS],[No. OF PAGES],[No. OF ARE-1 FORMS],[ARE-1 FORM STATUS],[LOAD PORT],
									[DISCHARGE PORT],[CHA AGENT], [SBID],[CreatedDate])
									SELECT [CommercialInvoiceNo],[CommercialInvoiceDate],[PrfmInvcNo],[PrfmaInvcDt],[ShpngBilNmbr],[ShpngBilDate],
									[AWL/BL],[AWL/BL Date],[SHIPPING BILL STATUS],[No. OF PAGES],[No. OF ARE-1 FORMS],[ARE-1 FORM STATUS],[LOAD PORT],
									[DISCHARGE PORT],[CHA AGENT], SBID, CreatedDate FROM [dbo].[View_OpeningEPCopies]
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
										 FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows			   
										 ,[@MAA].[CommercialInvoiceNo], [@MAA].[CommercialInvoiceDate], [@MAA].[PrfmInvcNo]
										 ,[@MAA].[PrfmaInvcDt], [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], [@MAA].[AWL/BL]
										 ,[@MAA].[AWL/BL Date], [@MAA].[SHIPPING BILL STATUS], [@MAA].[No. OF PAGES]
										 ,[@MAA].[No. OF ARE-1 FORMS], [@MAA].[ARE-1 FORM STATUS], [@MAA].[LOAD PORT]
										 ,[@MAA].[DISCHARGE PORT], [@MAA].[CHA AGENT], [@MAA].[SBID], [@MAA].CreatedDate
									  FROM @MAA {1}) RawResults) Results WHERE
									  RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc "; //order by ForeignEnquireId Desc";//"where f.IsActive <> 0 "

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " where CompanyId =" + "'" + CompanyID + "'" : " where CompanyId = " + "'" + CompanyID + "'" + s.ToString());
                //s.Clear();
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CommercialInvoiceNo"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", "");
                    //sb.Append(",");
                    string CommercialInvoiceNo = data["CommercialInvoiceNo"].ToString().Replace("\"", "\\\"");
                    CommercialInvoiceNo = CommercialInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", CommercialInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["CommercialInvoiceDate"]);
                    sb.Append(",");
                    string PrfmInvcNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PrfmInvcNo = PrfmInvcNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PrfmInvcNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string AWLBL = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    AWLBL = AWLBL.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", AWLBL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string ShpngBilNmbr = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""7"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string SHIPPING_BILL_STATUS = data["SHIPPING BILL STATUS"].ToString().Replace("\"", "\\\"");
                    SHIPPING_BILL_STATUS = SHIPPING_BILL_STATUS.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", SHIPPING_BILL_STATUS.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string No_OF_PAGES = data["No. OF PAGES"].ToString().Replace("\"", "\\\"");
                    No_OF_PAGES = No_OF_PAGES.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", No_OF_PAGES.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string No_OF_ARE_FORMS = data["No. OF ARE-1 FORMS"].ToString().Replace("\"", "\\\"");
                    No_OF_ARE_FORMS = No_OF_ARE_FORMS.Replace("\t", "-");
                    if (No_OF_ARE_FORMS == "")
                    {
                        No_OF_ARE_FORMS = "0";
                    }
                    sb.AppendFormat(@"""10"": ""{0}""", No_OF_ARE_FORMS.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ARE_FORM_STATUS = data["ARE-1 FORM STATUS"].ToString().Replace("\"", "\\\"");
                    ARE_FORM_STATUS = ARE_FORM_STATUS.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", ARE_FORM_STATUS.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string DISCHARGE_PORT = data["DISCHARGE PORT"].ToString().Replace("\"", "\\\"");
                    DISCHARGE_PORT = DISCHARGE_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", DISCHARGE_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CHA_AGENT = data["CHA AGENT"].ToString().Replace("\"", "\\\"");
                    CHA_AGENT = CHA_AGENT.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", CHA_AGENT.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR DUTY DRAWBACK AMOUNT
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetDutyDBAMT()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string P_CmrclInvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string P_CmrclInvcDt = HttpContext.Current.Request.Params["sSearch_1"];
                string P_PrfmInvcNo = HttpContext.Current.Request.Params["sSearch_2"];
                string P_PrfmaInvcDt = HttpContext.Current.Request.Params["sSearch_3"];
                string P_ShpngBilNmbr = HttpContext.Current.Request.Params["sSearch_4"];
                string P_ShpngBilDate = HttpContext.Current.Request.Params["sSearch_5"];
                string P_PrtLoading = HttpContext.Current.Request.Params["sSearch_6"];
                string P_PrtDschrg = HttpContext.Current.Request.Params["sSearch_7"];
                string P_CHA_AGENT = HttpContext.Current.Request.Params["sSearch_8"];
                string P_DutyDrawBackAmount_asperSB = HttpContext.Current.Request.Params["sSearch_9"];
                string P_Duty_DrawBackAmount_received = HttpContext.Current.Request.Params["sSearch_10"];
                string P_REMARKS_ASPER_ICEGATECUSTOMS_QUERY = HttpContext.Current.Request.Params["sSearch_11"];
                string P_ACTIONTAKEN_TOBETAKEN = HttpContext.Current.Request.Params["sSearch_12"];

                StringBuilder s = new StringBuilder();
                if (P_PrfmInvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + P_PrfmInvcNo + "%'");
                if (P_PrfmaInvcDt != "")
                {
                    DateTime FrmDt = P_PrfmaInvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_PrfmaInvcDt.Split('~')[0].ToString());
                    DateTime EndDat = P_PrfmaInvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_PrfmaInvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_ShpngBilNmbr != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + P_ShpngBilNmbr + "%'");
                if (P_ShpngBilDate != "")
                {
                    DateTime FrmDt = P_ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = P_ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_PrtLoading != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + P_PrtLoading + "%'");
                if (P_PrtDschrg != "")
                    s.Append(" and [DISCHARGE PORT] LIKE '%" + P_PrtDschrg + "%'");
                if (P_CHA_AGENT != "")
                    s.Append(" and [CHA AGENT] LIKE '%" + P_CHA_AGENT + "%'");
                if (P_DutyDrawBackAmount_asperSB != "")
                    s.Append(" and [DUTY DRAWBACK AMOUNT AS PER SB] LIKE '%" + P_DutyDrawBackAmount_asperSB + "%'");
                if (P_Duty_DrawBackAmount_received != "")
                    s.Append(" and [DUTY DRAWBACK AMOUNT RECEIVED] LIKE '%" + P_Duty_DrawBackAmount_received + "%'");
                if (P_REMARKS_ASPER_ICEGATECUSTOMS_QUERY != "")
                    s.Append(" and [REMARKS AS PER ICE GATE CUSTOMS QUERY] LIKE '%" + P_REMARKS_ASPER_ICEGATECUSTOMS_QUERY + "%'");
                if (P_ACTIONTAKEN_TOBETAKEN != "")
                    s.Append(" and [ACTION TAKEN / TO BE TAKEN] LIKE '%" + P_ACTIONTAKEN_TOBETAKEN + "%'");
                if (P_CmrclInvcNo != "")
                    s.Append(" and [CommercialInvoiceNo] LIKE '%" + P_CmrclInvcNo + "%'");
                if (P_CmrclInvcDt != "")
                {
                    DateTime FrmDt = P_CmrclInvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_CmrclInvcDt.Split('~')[0].ToString());
                    DateTime EndDat = P_CmrclInvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_CmrclInvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [CommercialInvoiceDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [DISCHARGE PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CHA AGENT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [DUTY DRAWBACK AMOUNT AS PER SB] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [DUTY DRAWBACK AMOUNT RECEIVED] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [REMARKS AS PER ICE GATE CUSTOMS QUERY] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ACTION TAKEN / TO BE TAKEN] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", PrfmInvcNo");
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

                string query = @"  
							declare @MAA TABLE([PrfmInvcNo] nvarchar(500),[PrfmaInvcDt] datetime,[ShpngBilNmbr] nvarchar(500),
												[ShpngBilDate] datetime, [LOAD PORT] nvarchar(max), [DISCHARGE PORT] nvarchar(max),[CHA AGENT] nvarchar(max), 
												[SBID] nvarchar(max), [DUTY DRAWBACK AMOUNT AS PER SB] nvarchar(400), [DUTY DRAWBACK AMOUNT RECEIVED] nvarchar(400),    
												[REMARKS AS PER ICE GATE CUSTOMS QUERY] nvarchar(max), [ACTION TAKEN / TO BE TAKEN] nvarchar(300), 
												CommercialInvoiceNo nvarchar(500), CommercialInvoiceDate datetime, CreatedDate datetime)
							INSERT
							INTO
								@MAA ([PrfmInvcNo],[PrfmaInvcDt],[ShpngBilNmbr],[ShpngBilDate],[LOAD PORT],[DISCHARGE PORT],[CHA AGENT], [SBID],
									  [DUTY DRAWBACK AMOUNT AS PER SB],[DUTY DRAWBACK AMOUNT RECEIVED],[REMARKS AS PER ICE GATE CUSTOMS QUERY],
									   [ACTION TAKEN / TO BE TAKEN], CommercialInvoiceNo, CommercialInvoiceDate, [CreatedDate])
									SELECT [PrfmInvcNo],[PrfmaInvcDt],[ShpngBilNmbr],[ShpngBilDate],[LOAD PORT], [DISCHARGE PORT],[CHA AGENT], [SBID],
										   [DUTY DRAWBACK AMOUNT AS PER SB],[DUTY DRAWBACK AMOUNT RECEIVED],[REMARKS AS PER ICE GATE CUSTOMS QUERY],
										   [ACTION TAKEN / TO BE TAKEN], CommercialInvoiceNo, CommercialInvoiceDate, [CreatedDate] FROM [dbo].[View_DutyDrawBack] 
									{4}                   

							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
										 FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows			   
										 ,[@MAA].[PrfmInvcNo],[@MAA].[PrfmaInvcDt], [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], [@MAA].[LOAD PORT]
										 ,[@MAA].[DISCHARGE PORT], [@MAA].[CHA AGENT], [@MAA].[SBID], [@MAA].[DUTY DRAWBACK AMOUNT AS PER SB], [@MAA].[DUTY DRAWBACK AMOUNT RECEIVED]
										 ,[@MAA].[REMARKS AS PER ICE GATE CUSTOMS QUERY], [@MAA].[ACTION TAKEN / TO BE TAKEN] 
										 ,[@MAA].CommercialInvoiceNo, [@MAA].CommercialInvoiceDate, [@MAA].CreatedDate
									  FROM @MAA {1}) RawResults) Results WHERE
									  RowNumber BETWEEN {2} AND {3}  order by [CreatedDate] desc"; //order by ForeignEnquireId Desc";//"where f.IsActive <> 0 "

                string Where = "and CompanyId = '" + Context.Session["CompanyID"].ToString() + "'";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, " where IsActive <> 0 " + Where + s.ToString() + "");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");

                    string CommercialInvoiceNo = data["CommercialInvoiceNo"].ToString().Replace("\"", "\\\"");
                    CommercialInvoiceNo = CommercialInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", CommercialInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["CommercialInvoiceDate"]);
                    sb.Append(",");
                    string PInvoiceNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string ShpngBilNmbr = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string DISCHARGE_PORT = data["DISCHARGE PORT"].ToString().Replace("\"", "\\\"");
                    DISCHARGE_PORT = DISCHARGE_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", DISCHARGE_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CHA_AGENT = data["CHA AGENT"].ToString().Replace("\"", "\\\"");
                    CHA_AGENT = CHA_AGENT.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", CHA_AGENT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string DutyDrawBackAmount_asperSB = data["DUTY DRAWBACK AMOUNT AS PER SB"].ToString().Replace("\"", "\\\"");
                    DutyDrawBackAmount_asperSB = DutyDrawBackAmount_asperSB.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", DutyDrawBackAmount_asperSB.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Duty_DrawBackAmount_received = data["DUTY DRAWBACK AMOUNT RECEIVED"].ToString().Replace("\"", "\\\"");
                    Duty_DrawBackAmount_received = Duty_DrawBackAmount_received.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", Duty_DrawBackAmount_received.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string REMARKS_ASPER_ICEGATECUSTOMS_QUERY = data["REMARKS AS PER ICE GATE CUSTOMS QUERY"].ToString().Replace("\"", "\\\"");
                    REMARKS_ASPER_ICEGATECUSTOMS_QUERY = REMARKS_ASPER_ICEGATECUSTOMS_QUERY.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", REMARKS_ASPER_ICEGATECUSTOMS_QUERY.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string ACTIONTAKEN_TOBETAKEN = data["ACTION TAKEN / TO BE TAKEN"].ToString().Replace("\"", "\\\"");
                    ACTIONTAKEN_TOBETAKEN = ACTIONTAKEN_TOBETAKEN.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", ACTIONTAKEN_TOBETAKEN.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string INVsforBRCsPending()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string B_InvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string B_InvcDt = HttpContext.Current.Request.Params["sSearch_1"];
                string B_PkngNo = HttpContext.Current.Request.Params["sSearch_2"];
                string B_PkngDt = HttpContext.Current.Request.Params["sSearch_3"];
                string B_AWB_BLNo = HttpContext.Current.Request.Params["sSearch_4"];
                string B_AWB_BLDt = HttpContext.Current.Request.Params["sSearch_5"];
                string B_ShpngBilNmbr = HttpContext.Current.Request.Params["sSearch_6"];
                string B_ShpngBilDate = HttpContext.Current.Request.Params["sSearch_7"];
                string B_Port = HttpContext.Current.Request.Params["sSearch_8"];
                string B_CIF_AMOUNT = HttpContext.Current.Request.Params["sSearch_9"];
                string B_FRIEGHT = HttpContext.Current.Request.Params["sSearch_10"];
                string B_FOB_VALUE = HttpContext.Current.Request.Params["sSearch_11"];
                string B_NAME_OF_THE_PARTY = HttpContext.Current.Request.Params["sSearch_12"];
                string B_TO_PORT = HttpContext.Current.Request.Params["sSearch_13"];


                StringBuilder s = new StringBuilder();
                if (B_InvcNo != "")
                    s.Append(" and [CommercialInvoiceNo] LIKE '%" + B_InvcNo + "%'");
                if (B_InvcDt != "")
                {
                    DateTime FrmDt = B_InvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_InvcDt.Split('~')[0].ToString());
                    DateTime EndDat = B_InvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_InvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [CommercialInvoiceDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_PkngNo != "")
                    s.Append(" and [PkngListNo] LIKE '%" + B_PkngNo + "%'");
                if (B_PkngDt != "")
                {
                    DateTime FrmDt = B_PkngDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_PkngDt.Split('~')[0].ToString());
                    DateTime EndDat = B_PkngDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_PkngDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PkngListNoDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                } if (B_AWB_BLNo != "")
                    s.Append(" and [AWL/BL] LIKE '%" + B_AWB_BLNo + "%'");
                if (B_AWB_BLDt != "" && B_AWB_BLDt != "~")
                {
                    DateTime FrmDt = B_AWB_BLDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_AWB_BLDt.Split('~')[0].ToString());
                    DateTime EndDat = B_AWB_BLDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_AWB_BLDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [AWL/BL Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_ShpngBilNmbr != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + B_ShpngBilNmbr + "%'");
                if (B_ShpngBilDate != "")
                {
                    DateTime FrmDt = B_ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = B_ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_Port != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + B_Port + "%'");
                if (B_CIF_AMOUNT != "")
                    s.Append(" and [CIF Amount] LIKE '%" + B_CIF_AMOUNT + "%'");
                if (B_FRIEGHT != "")
                    s.Append(" and [FREIGHT] LIKE '%" + B_FRIEGHT + "%'");
                if (B_FOB_VALUE != "")
                    s.Append(" and [FOB Value] LIKE '%" + B_FOB_VALUE + "%'");
                if (B_NAME_OF_THE_PARTY != "")
                    s.Append(" and [Name of the Party] LIKE '%" + B_NAME_OF_THE_PARTY + "%'");
                if (B_TO_PORT != "")
                    s.Append(" and [TO PORT] LIKE '%" + B_TO_PORT + "%'");


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
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

                    orderByClause = orderByClause.Replace("0", ", CommercialInvoiceDate");
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

                string query = @"  
							declare @MAA TABLE([CommercialInvoiceNo] nvarchar(max),[CommercialInvoiceDate] datetime, [PkngListNo] nvarchar(max),[PkngListNoDt] datetime, 
							[ShpngBilNmbr] nvarchar(max), [ShpngBilDate] datetime, [LOAD PORT] nvarchar(max), [TO PORT] nvarchar(max), 
							[AWL/BL] nvarchar(max), [AWL/BL Date] datetime, [SBID] nvarchar(max), [CIF Amount] nvarchar(max), FREIGHT nvarchar(max), 
							[FOB Value] nvarchar(max), [Name of the Party] nvarchar(max), CreatedDate datetime)
							INSERT INTO
								@MAA ([CommercialInvoiceNo],[CommercialInvoiceDate],[PkngListNo], [PkngListNoDt], [ShpngBilNmbr],[ShpngBilDate], [AWL/BL], [AWL/BL Date], 
								[LOAD PORT],[CIF Amount],[FREIGHT], [FOB Value], [Name of the Party],[TO PORT],[SBID], CreatedDate)
								Select CommercialInvoiceNo, CommercialInvoiceDate, PkngListNo, PkngListNoDt, ShpngBilNmbr, ShpngBilDate, [AWL/BL], [AWL/BL Date], 
								[LOAD PORT], [CIF Amount], FREIGHT, [FOB Value], [Name of the Party], [TO PORT], SBID, CreatedDate from dbo.View_INVsforBRCsPending where IsActive <> 0
								{4} {5}                  
							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
								FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].[CommercialInvoiceNo], 
								[@MAA].[CommercialInvoiceDate], [@MAA].[PkngListNo],[@MAA].[PkngListNoDt], [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], 
								[@MAA].[AWL/BL], [@MAA].[AWL/BL Date], [@MAA].[LOAD PORT], [@MAA].[CIF Amount], [@MAA].[FREIGHT], [@MAA].[FOB Value], 
								[@MAA].[Name of the Party], [@MAA].[TO PORT],[@MAA].[SBID], [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results WHERE
								RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";

                string Where = " and CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + "'";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString(), Where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string PInvoiceNo = data["CommercialInvoiceNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    if (data["CommercialInvoiceDate"].ToString() != "" && Convert.ToDateTime(data["CommercialInvoiceDate"]) != CommonBLL.StartDate)
                        sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["CommercialInvoiceDate"]);
                    else
                        sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", "");
                    sb.Append(",");
                    string PkngNo = data["PkngListNo"].ToString().Replace("\"", "\\\"");
                    PkngNo = PkngNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PkngNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PkngListNoDt"]);
                    sb.Append(",");
                    string ShpngBilNmbr = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string AWL_BL = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    AWL_BL = AWL_BL.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", AWL_BL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""7"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CIF_Amount = data["CIF Amount"].ToString().Replace("\"", "\\\"");
                    CIF_Amount = CIF_Amount.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", CIF_Amount.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FREIGHT = data["FREIGHT"].ToString().Replace("\"", "\\\"");
                    FREIGHT = FREIGHT.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", FREIGHT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FOB_Value = data["FOB Value"].ToString().Replace("\"", "\\\"");
                    FOB_Value = FOB_Value.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", FOB_Value.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Name_of_the_Party = data["Name of the Party"].ToString().Replace("\"", "\\\"");
                    Name_of_the_Party = Name_of_the_Party.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", Name_of_the_Party.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string TO_PORT = data["TO PORT"].ToString().Replace("\"", "\\\"");
                    TO_PORT = TO_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", TO_PORT.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string INVsforEPSBsPending()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string B_InvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string B_InvcDt = HttpContext.Current.Request.Params["sSearch_1"];
                string B_PkngNo = HttpContext.Current.Request.Params["sSearch_2"];
                string B_PkngDt = HttpContext.Current.Request.Params["sSearch_3"];
                string B_AWB_BLNo = HttpContext.Current.Request.Params["sSearch_4"];
                string B_AWB_BLDt = HttpContext.Current.Request.Params["sSearch_5"];
                string B_ShpngBilNmbr = HttpContext.Current.Request.Params["sSearch_6"];
                string B_ShpngBilDate = HttpContext.Current.Request.Params["sSearch_7"];
                string B_Port = HttpContext.Current.Request.Params["sSearch_8"];
                string B_CIF_AMOUNT = HttpContext.Current.Request.Params["sSearch_9"];
                string B_FRIEGHT = HttpContext.Current.Request.Params["sSearch_10"];
                string B_FOB_VALUE = HttpContext.Current.Request.Params["sSearch_11"];
                string B_NAME_OF_THE_PARTY = HttpContext.Current.Request.Params["sSearch_12"];
                string B_TO_PORT = HttpContext.Current.Request.Params["sSearch_13"];


                StringBuilder s = new StringBuilder();
                if (B_InvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + B_InvcNo + "%'");
                if (B_InvcDt != "")
                {
                    DateTime FrmDt = B_InvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_InvcDt.Split('~')[0].ToString());
                    DateTime EndDat = B_InvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_InvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_PkngNo != "")
                    s.Append(" and [PkngListNo] LIKE '%" + B_PkngNo + "%'");
                if (B_PkngDt != "")
                {
                    DateTime FrmDt = B_PkngDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_PkngDt.Split('~')[0].ToString());
                    DateTime EndDat = B_PkngDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_PkngDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PkngListNoDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                } if (B_AWB_BLNo != "")
                    s.Append(" and [AWL/BL] LIKE '%" + B_AWB_BLNo + "%'");
                if (B_AWB_BLDt != "")
                {
                    DateTime FrmDt = B_AWB_BLDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_AWB_BLDt.Split('~')[0].ToString());
                    DateTime EndDat = B_AWB_BLDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_AWB_BLDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [AWL/BL Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_ShpngBilNmbr != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + B_ShpngBilNmbr + "%'");
                if (B_ShpngBilDate != "")
                {
                    DateTime FrmDt = B_ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = B_ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (B_Port != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + B_Port + "%'");
                if (B_CIF_AMOUNT != "")
                    s.Append(" and [CIF Amount] LIKE '%" + B_CIF_AMOUNT + "%'");
                if (B_FRIEGHT != "")
                    s.Append(" and [FREIGHT] LIKE '%" + B_FRIEGHT + "%'");
                if (B_FOB_VALUE != "")
                    s.Append(" and [FOB Value] LIKE '%" + B_FOB_VALUE + "%'");
                if (B_NAME_OF_THE_PARTY != "" && B_NAME_OF_THE_PARTY != null)
                    s.Append(" and [Name of the Party] LIKE '%" + B_NAME_OF_THE_PARTY + "%'");
                if (B_TO_PORT != "" && B_TO_PORT != null)
                    s.Append(" and [TO PORT] LIKE '%" + B_TO_PORT + "%'");


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PkngListNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PkngListNoDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [TO PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CIF Amount] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FREIGHT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FOB Value] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Name of the Party] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", PrfmInvcNo");
                    //orderByClause = orderByClause.Replace("1", ", [CommercialInvoiceDate] ");
                    //orderByClause = orderByClause.Replace("2", ", [PrfmInvcNo] ");
                    //orderByClause = orderByClause.Replace("3", ", [PrfmaInvcDt] ");
                    //orderByClause = orderByClause.Replace("4", ", [ShpngBilNmbr] ");
                    //orderByClause = orderByClause.Replace("5", ", [ShpngBilDate] ");
                    //orderByClause = orderByClause.Replace("6", ", [AWL/BL] ");
                    //orderByClause = orderByClause.Replace("7", ", [AWL/BL Date] ");
                    //orderByClause = orderByClause.Replace("8", ", [SHIPPING BILL STATUS] ");
                    //orderByClause = orderByClause.Replace("9", ", [No. OF PAGES] ");
                    //orderByClause = orderByClause.Replace("11", ", [No. OF ARE-1 FORMS] ");
                    //orderByClause = orderByClause.Replace("12", ", [ARE-1 FORM STATUS] ");
                    //orderByClause = orderByClause.Replace("13", ", [LOAD PORT] ");
                    //orderByClause = orderByClause.Replace("14", ", [DISCHARGE PORT] ");
                    //orderByClause = orderByClause.Replace("15", ", [CHA AGENT] ");
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

                string query = @"  
							declare @MAA TABLE([PrfmInvcNo] nvarchar(500),[PrfmaInvcDt] datetime, [PkngListNo] nvarchar(500),[PkngListNoDt] datetime, 
							[ShpngBilNmbr] nvarchar(500), [ShpngBilDate] datetime, [LOAD PORT] nvarchar(max), [TO PORT] nvarchar(max), 
							[AWL/BL] nvarchar(max), [AWL/BL Date] datetime, [SBID] nvarchar(max), [CIF Amount] nvarchar(20), FREIGHT nvarchar(20), 
							[FOB Value] nvarchar(20), [Name of the Party] nvarchar(max))
							INSERT INTO
								@MAA ([PrfmInvcNo],[PrfmaInvcDt],[PkngListNo], [PkngListNoDt], [ShpngBilNmbr],[ShpngBilDate], [AWL/BL], [AWL/BL Date], 
								[LOAD PORT],[CIF Amount],[FREIGHT], [FOB Value], [Name of the Party],[TO PORT],[SBID])
								Select PrfmInvcNo, PrfmaInvcDt, PkngListNo, PkngListNoDt, ShpngBilNmbr, ShpngBilDate, [AWL/BL], [AWL/BL Date], 
								[LOAD PORT], [CIF Amount], FREIGHT, [FOB Value], [Name of the Party], [TO PORT], SBID from dbo.View_INVsforBRCsPending
								where IsActive <> 0
								{4}                   
							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
								FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].[PrfmInvcNo], 
								[@MAA].[PrfmaInvcDt], [@MAA].[PkngListNo],[@MAA].[PkngListNoDt], [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], 
								[@MAA].[AWL/BL], [@MAA].[AWL/BL Date], [@MAA].[LOAD PORT], [@MAA].[CIF Amount], [@MAA].[FREIGHT], [@MAA].[FOB Value], 
								[@MAA].[Name of the Party], [@MAA].[TO PORT],[@MAA].[SBID] FROM @MAA {1}) RawResults) Results WHERE
								RowNumber BETWEEN {2} AND {3} ";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + "order by PrfmaInvcDt DESC");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string PInvoiceNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string PkngNo = data["PkngListNo"].ToString().Replace("\"", "\\\"");
                    PkngNo = PkngNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PkngNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PkngListNoDt"]);
                    sb.Append(",");
                    string ShpngBilNmbr = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string AWL_BL = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    AWL_BL = AWL_BL.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", AWL_BL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""7"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CIF_Amount = data["CIF Amount"].ToString().Replace("\"", "\\\"");
                    CIF_Amount = CIF_Amount.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", CIF_Amount.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FREIGHT = data["FREIGHT"].ToString().Replace("\"", "\\\"");
                    FREIGHT = FREIGHT.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", FREIGHT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FOB_Value = data["FOB Value"].ToString().Replace("\"", "\\\"");
                    FOB_Value = FOB_Value.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", FOB_Value.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Name_of_the_Party = data["Name of the Party"].ToString().Replace("\"", "\\\"");
                    Name_of_the_Party = Name_of_the_Party.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", Name_of_the_Party.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string TO_PORT = data["TO PORT"].ToString().Replace("\"", "\\\"");
                    TO_PORT = TO_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", TO_PORT.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string AlrdyShpdOnBoard_Pending_BL_Rlse()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string S_InvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string S_Invc_Date = HttpContext.Current.Request.Params["sSearch_1"];
                string S_Type_of_Consignment = HttpContext.Current.Request.Params["sSearch_2"];
                string S_Amount_in_USD_FOB = HttpContext.Current.Request.Params["sSearch_3"];
                string S_Freight = HttpContext.Current.Request.Params["sSearch_4"];
                string S_Cost_and_Freight = HttpContext.Current.Request.Params["sSearch_5"];
                string S_POL = HttpContext.Current.Request.Params["sSearch_6"];
                string S_POD = HttpContext.Current.Request.Params["sSearch_7"];
                string S_No_of_Pkgs = HttpContext.Current.Request.Params["sSearch_8"];
                string S_Gross_Weight = HttpContext.Current.Request.Params["sSearch_9"];
                string S_Net_Weight = HttpContext.Current.Request.Params["sSearch_10"];
                string S_ShpngBl_No = HttpContext.Current.Request.Params["sSearch_11"];
                string S_ShpngBl_Date = HttpContext.Current.Request.Params["sSearch_12"];
                string S_Container_No = HttpContext.Current.Request.Params["sSearch_13"];
                string S_BL_No_AWB_No = HttpContext.Current.Request.Params["sSearch_14"];
                string S_BL_No_AWB_Date = HttpContext.Current.Request.Params["sSearch_15"];
                string S_Contact_Person = HttpContext.Current.Request.Params["sSearch_16"];
                string S_Remarks = HttpContext.Current.Request.Params["sSearch_17"];
                string S_VESSEL_Details = HttpContext.Current.Request.Params["sSearch_18"];

                StringBuilder s = new StringBuilder();
                if (S_InvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + S_InvcNo + "%'");
                if (S_Invc_Date != "")
                {
                    DateTime FrmDt = S_Invc_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_Invc_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Type_of_Consignment != "")
                    s.Append(" and [ShipmentMode] LIKE '%" + S_Type_of_Consignment + "%'");
                if (S_Amount_in_USD_FOB != "")
                    s.Append(" and [FOB Value] LIKE '%" + S_Amount_in_USD_FOB + "%'");
                if (S_Freight != "")
                    s.Append(" and [FREIGHT] LIKE '%" + S_Freight + "%'");
                if (S_Cost_and_Freight != "")
                    s.Append(" and [CIF Amount] LIKE '%" + S_Cost_and_Freight + "%'");
                if (S_POL != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + S_POL + "%'");
                if (S_POD != "")
                    s.Append(" and [TO PORT] LIKE '%" + S_POD + "%'");
                if (S_No_of_Pkgs != "")
                    s.Append(" and [No of Pkgs] LIKE '%" + S_No_of_Pkgs + "%'");
                if (S_Gross_Weight != "")
                    s.Append(" and [GrossWeight] LIKE '%" + S_Gross_Weight + "%'");
                if (S_Net_Weight != "")
                    s.Append(" and [NetWeight] LIKE '%" + S_Net_Weight + "%'");
                if (S_ShpngBl_No != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + S_ShpngBl_No + "%'");
                if (S_ShpngBl_Date != "")
                {
                    DateTime FrmDt = S_ShpngBl_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_ShpngBl_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Container_No != "")
                    s.Append(" and [ContainerNumber] LIKE '%" + S_Container_No + "%'");
                if (S_BL_No_AWB_No != "")
                    s.Append(" and [AWL/BL] LIKE '%" + S_BL_No_AWB_No + "%'");

                if (S_BL_No_AWB_Date != "")
                {
                    DateTime FrmDt = S_BL_No_AWB_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_BL_No_AWB_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [AWL/BL Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Contact_Person != "")
                    s.Append(" and [ContactPerson] LIKE '%" + S_Contact_Person + "%'");

                if (S_Remarks != "")
                    s.Append(" and [Remarks] LIKE '%" + S_Remarks + "%'");

                if (S_VESSEL_Details != "")
                {
                    s.Append(" and [Vessel] LIKE '%" + S_VESSEL_Details + "%'");
                }


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShipmentMode] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FOB Value] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FREIGHT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CIF Amount] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [TO PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [No of Pkgs] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [GrossWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [NetWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContainerNumber] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContactPerson] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Remarks] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Vessel] LIKE ");
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
                    orderByClause = orderByClause.Replace("0", ", PrfmInvcNo");
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

                string query = @"  
							declare @MAA TABLE([PrfmInvcNo] nvarchar(500),[PrfmaInvcDt] datetime, ShipmentMode nvarchar(500), 
								[FOB Value] nvarchar(20), FREIGHT nvarchar(20), [CIF Amount] nvarchar(20), [LOAD PORT] nvarchar(max), 
								[TO PORT] nvarchar(max), [No of Pkgs] nvarchar(20), GrossWeight nvarchar(20), NetWeight nvarchar(20),
								[ShpngBilNmbr] nvarchar(500), [ShpngBilDate] datetime,   ContainerNumber nvarchar(500),
								[AWL/BL] nvarchar(max), [AWL/BL Date] datetime, ContactPerson nvarchar(500), Remarks nvarchar(Max), 
								Vessel nvarchar(500), [SBID] nvarchar(max), CreatedDate datetime)
								INSERT INTO
								@MAA ([PrfmInvcNo],[PrfmaInvcDt], ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], 
								[TO PORT], [No of Pkgs], GrossWeight, NetWeight, [ShpngBilNmbr], [ShpngBilDate],  ContainerNumber,
								[AWL/BL], [AWL/BL Date], ContactPerson, Remarks, Vessel, [SBID], CreatedDate)
								Select PrfmInvcNo, PrfmaInvcDt, ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], [TO PORT], 
								[No of Pkgs], GrossWeight, NetWeight, ShpngBilNmbr, ShpngBilDate, ContainerNumber, [AWL/BL], [AWL/BL Date], 
								ContactPerson, Remarks, Vessel, SBID, CreatedDate from dbo.View_AlrdyShpdOnBoard_Pending_BL_Rlse 
								{4}                   
							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
								FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].[PrfmInvcNo], 
								[@MAA].[PrfmaInvcDt], [@MAA].ShipmentMode, [@MAA].[FOB Value], [@MAA].FREIGHT, [@MAA].[CIF Amount], [@MAA].[LOAD PORT], 
								[@MAA].[TO PORT], [@MAA].[No of Pkgs], [@MAA].GrossWeight, [@MAA].NetWeight, [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], 
								[@MAA].ContainerNumber, [@MAA].[AWL/BL], [@MAA].[AWL/BL Date], [@MAA].ContactPerson, [@MAA].Remarks, [@MAA].Vessel, 
								[@MAA].[SBID], [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3} order by CreatedDate desc ";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(), " where CompanyId = '" + Session["CompanyID"] + "' and Vessel <> ''");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string PInvoiceNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string ShipmentMode = data["ShipmentMode"].ToString().Replace("\"", "\\\"");
                    ShipmentMode = ShipmentMode.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", ShipmentMode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FOB_Value = data["FOB Value"].ToString().Replace("\"", "\\\"");
                    FOB_Value = FOB_Value.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", FOB_Value.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FREIGHT = data["FREIGHT"].ToString().Replace("\"", "\\\"");
                    FREIGHT = FREIGHT.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", FREIGHT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CIF_Amount = data["CIF Amount"].ToString().Replace("\"", "\\\"");
                    CIF_Amount = CIF_Amount.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CIF_Amount.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string TO_PORT = data["TO PORT"].ToString().Replace("\"", "\\\"");
                    TO_PORT = TO_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", TO_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string No_of_Pkgs = data["No of Pkgs"].ToString().Replace("\"", "\\\"");
                    No_of_Pkgs = No_of_Pkgs.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", No_of_Pkgs.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string GrossWeight = data["GrossWeight"].ToString().Replace("\"", "\\\"");
                    GrossWeight = GrossWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", GrossWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string NetWeight = data["NetWeight"].ToString().Replace("\"", "\\\"");
                    NetWeight = NetWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", NetWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string ShpngBilNmbr = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""12"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string ContainerNumber = data["ContainerNumber"].ToString().Replace("\"", "\\\"");
                    ContainerNumber = ContainerNumber.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", ContainerNumber.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string AWL_BL = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    AWL_BL = AWL_BL.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", AWL_BL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""15"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
                    ContactPerson = ContactPerson.Replace("\t", "-");
                    sb.AppendFormat(@"""16"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Remarks = data["Remarks"].ToString().Replace("\"", "\\\"");
                    Remarks = Remarks.Replace("\t", "-");
                    sb.AppendFormat(@"""17"": ""{0}""", Remarks.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Vessel = data["Vessel"].ToString().Replace("\"", "\\\"");
                    Vessel = Vessel.Replace("\t", "-");
                    sb.AppendFormat(@"""18"": ""{0}""", Vessel.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string Gated_into_Sea_Port_Awating_SOB_Confirmation_Vessel_Sailing()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string S_InvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string S_Invc_Date = HttpContext.Current.Request.Params["sSearch_1"];
                string S_Type_of_Consignment = HttpContext.Current.Request.Params["sSearch_2"];
                string S_Amount_in_USD_FOB = HttpContext.Current.Request.Params["sSearch_3"];
                string S_Freight = HttpContext.Current.Request.Params["sSearch_4"];
                string S_Cost_and_Freight = HttpContext.Current.Request.Params["sSearch_5"];
                string S_POL = HttpContext.Current.Request.Params["sSearch_6"];
                string S_POD = HttpContext.Current.Request.Params["sSearch_7"];
                string S_No_of_Pkgs = HttpContext.Current.Request.Params["sSearch_8"];
                string S_Gross_Weight = HttpContext.Current.Request.Params["sSearch_9"];
                string S_Net_Weight = HttpContext.Current.Request.Params["sSearch_10"];
                string S_ShpngBl_No = HttpContext.Current.Request.Params["sSearch_11"];
                string S_ShpngBl_Date = HttpContext.Current.Request.Params["sSearch_12"];
                string S_Container_No = HttpContext.Current.Request.Params["sSearch_13"];
                string S_BL_No_AWB_No = HttpContext.Current.Request.Params["sSearch_14"];
                string S_BL_No_AWB_Date = HttpContext.Current.Request.Params["sSearch_15"];
                string S_Contact_Person = HttpContext.Current.Request.Params["sSearch_16"];
                string S_Remarks = HttpContext.Current.Request.Params["sSearch_17"];
                string S_VESSEL_Details = HttpContext.Current.Request.Params["sSearch_18"];

                StringBuilder s = new StringBuilder();
                if (S_InvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + S_InvcNo + "%'");
                if (S_Invc_Date != "")
                {
                    DateTime FrmDt = S_Invc_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_Invc_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Type_of_Consignment != "")
                    s.Append(" and [ShipmentMode] LIKE '%" + S_Type_of_Consignment + "%'");
                if (S_Amount_in_USD_FOB != "")
                    s.Append(" and [FOB Value] LIKE '%" + S_Amount_in_USD_FOB + "%'");
                if (S_Freight != "")
                    s.Append(" and [FREIGHT] LIKE '%" + S_Freight + "%'");
                if (S_Cost_and_Freight != "")
                    s.Append(" and [CIF Amount] LIKE '%" + S_Cost_and_Freight + "%'");
                if (S_POL != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + S_POL + "%'");
                if (S_POD != "")
                    s.Append(" and [TO PORT] LIKE '%" + S_POD + "%'");
                if (S_No_of_Pkgs != "")
                    s.Append(" and [No of Pkgs] LIKE '%" + S_No_of_Pkgs + "%'");
                if (S_Gross_Weight != "")
                    s.Append(" and [GrossWeight] LIKE '%" + S_Gross_Weight + "%'");
                if (S_Net_Weight != "")
                    s.Append(" and [NetWeight] LIKE '%" + S_Net_Weight + "%'");
                if (S_ShpngBl_No != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + S_ShpngBl_No + "%'");
                if (S_ShpngBl_Date != "")
                {
                    DateTime FrmDt = S_ShpngBl_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_ShpngBl_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBilDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Container_No != "")
                    s.Append(" and [ContainerNumber] LIKE '%" + S_Container_No + "%'");
                if (S_BL_No_AWB_No != "")
                    s.Append(" and [AWL/BL] LIKE '%" + S_BL_No_AWB_No + "%'");

                if (S_BL_No_AWB_Date != "")
                {
                    DateTime FrmDt = S_BL_No_AWB_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_BL_No_AWB_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[1].ToString());
                    if (//(FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001") || 
                        (FrmDt.ToShortDateString() != "01-01-1900" && EndDat.ToShortDateString() != "31-12-9999"))
                        s.Append(" and [AWL/BL Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Contact_Person != "")
                    s.Append(" and [ContactPerson] LIKE '%" + S_Contact_Person + "%'");

                if (S_Remarks != "")
                    s.Append(" and [Remarks] LIKE '%" + S_Remarks + "%'");

                if (S_VESSEL_Details != "")
                    s.Append(" and [Vessel] LIKE '%" + S_VESSEL_Details + "%'");


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShipmentMode] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FOB Value] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FREIGHT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CIF Amount] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [TO PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [No of Pkgs] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [GrossWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [NetWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContainerNumber] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContactPerson] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Remarks] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Vessel] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", SBID");
                    //orderByClause = orderByClause.Replace("1", ", [CommercialInvoiceDate] ");
                    //orderByClause = orderByClause.Replace("2", ", [PrfmInvcNo] ");
                    //orderByClause = orderByClause.Replace("3", ", [PrfmaInvcDt] ");
                    //orderByClause = orderByClause.Replace("4", ", [ShpngBilNmbr] ");
                    //orderByClause = orderByClause.Replace("5", ", [ShpngBilDate] ");
                    //orderByClause = orderByClause.Replace("6", ", [AWL/BL] ");
                    //orderByClause = orderByClause.Replace("7", ", [AWL/BL Date] ");
                    //orderByClause = orderByClause.Replace("8", ", [SHIPPING BILL STATUS] ");
                    //orderByClause = orderByClause.Replace("9", ", [No. OF PAGES] ");
                    //orderByClause = orderByClause.Replace("11", ", [No. OF ARE-1 FORMS] ");
                    //orderByClause = orderByClause.Replace("12", ", [ARE-1 FORM STATUS] ");
                    //orderByClause = orderByClause.Replace("13", ", [LOAD PORT] ");
                    //orderByClause = orderByClause.Replace("14", ", [DISCHARGE PORT] ");
                    //orderByClause = orderByClause.Replace("15", ", [CHA AGENT] ");
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

                string query = @"  
							declare @MAA TABLE([PrfmInvcNo] nvarchar(500),[PrfmaInvcDt] datetime, ShipmentMode nvarchar(500), 
								[FOB Value] nvarchar(20), FREIGHT nvarchar(20), [CIF Amount] nvarchar(20), [LOAD PORT] nvarchar(max), 
								[TO PORT] nvarchar(max), [No of Pkgs] nvarchar(20), GrossWeight nvarchar(20), NetWeight nvarchar(20),
								[ShpngBilNmbr] nvarchar(500), [ShpngBilDate] datetime,   ContainerNumber nvarchar(500),
								[AWL/BL] nvarchar(max), [AWL/BL Date] datetime, ContactPerson nvarchar(500), Remarks nvarchar(Max), 
								Vessel nvarchar(500), [SBID] nvarchar(max), CreatedDate datetime)
								INSERT INTO
								@MAA ([PrfmInvcNo],[PrfmaInvcDt], ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], 
								[TO PORT], [No of Pkgs], GrossWeight, NetWeight, [ShpngBilNmbr], [ShpngBilDate],  ContainerNumber,
								[AWL/BL], [AWL/BL Date], ContactPerson, Remarks, Vessel, [SBID], CreatedDate)
								Select PrfmInvcNo, PrfmaInvcDt, ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], [TO PORT], 
								[No of Pkgs], GrossWeight, NetWeight, ShpngBilNmbr, ShpngBilDate, ContainerNumber, [AWL/BL], Null, 
								ContactPerson, Remarks, Vessel, SBID, CreatedDate from dbo.View_Gated_into_Sea_Port_Awating_SOB_Confirmation_Vessel_Sailing 
								{5} {4}                   
							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].SBID)
								FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].SBID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].[PrfmInvcNo], 
								[@MAA].[PrfmaInvcDt], [@MAA].ShipmentMode, [@MAA].[FOB Value], [@MAA].FREIGHT, [@MAA].[CIF Amount], [@MAA].[LOAD PORT], 
								[@MAA].[TO PORT], [@MAA].[No of Pkgs], [@MAA].GrossWeight, [@MAA].NetWeight, [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], 
								[@MAA].ContainerNumber, [@MAA].[AWL/BL], [@MAA].[AWL/BL Date], [@MAA].ContactPerson, [@MAA].Remarks, [@MAA].Vessel, 
								[@MAA].[SBID], [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3} order by CreatedDate desc ";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString(), " where CompanyId = '" + Session["CompanyID"] + "'");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string PInvoiceNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string ShipmentMode = data["ShipmentMode"].ToString().Replace("\"", "\\\"");
                    ShipmentMode = ShipmentMode.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", ShipmentMode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FOB_Value = data["FOB Value"].ToString().Replace("\"", "\\\"");
                    FOB_Value = FOB_Value.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", FOB_Value.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FREIGHT = data["FREIGHT"].ToString().Replace("\"", "\\\"");
                    FREIGHT = FREIGHT.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", FREIGHT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CIF_Amount = data["CIF Amount"].ToString().Replace("\"", "\\\"");
                    CIF_Amount = CIF_Amount.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CIF_Amount.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string TO_PORT = data["TO PORT"].ToString().Replace("\"", "\\\"");
                    TO_PORT = TO_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", TO_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string No_of_Pkgs = data["No of Pkgs"].ToString().Replace("\"", "\\\"");
                    No_of_Pkgs = No_of_Pkgs.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", No_of_Pkgs.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string GrossWeight = data["GrossWeight"].ToString().Replace("\"", "\\\"");
                    GrossWeight = GrossWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", GrossWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string NetWeight = data["NetWeight"].ToString().Replace("\"", "\\\"");
                    NetWeight = NetWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", NetWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string ShpngBilNmbr = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""12"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string ContainerNumber = data["ContainerNumber"].ToString().Replace("\"", "\\\"");
                    ContainerNumber = ContainerNumber.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", ContainerNumber.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string AWL_BL = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    AWL_BL = AWL_BL.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", AWL_BL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""15"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
                    ContactPerson = ContactPerson.Replace("\t", "-");
                    sb.AppendFormat(@"""16"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Remarks = data["Remarks"].ToString().Replace("\"", "\\\"");
                    Remarks = Remarks.Replace("\t", "-");
                    sb.AppendFormat(@"""17"": ""{0}""", Remarks.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Vessel = data["Vessel"].ToString().Replace("\"", "\\\"");
                    Vessel = Vessel.Replace("\t", "-");
                    sb.AppendFormat(@"""18"": ""{0}""", Vessel.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string Ready_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string S_InvcNo = HttpContext.Current.Request.Params["sSearch_0"];
                string S_Invc_Date = HttpContext.Current.Request.Params["sSearch_1"];
                string S_Type_of_Consignment = HttpContext.Current.Request.Params["sSearch_2"];
                string S_Amount_in_USD_FOB = HttpContext.Current.Request.Params["sSearch_3"];
                string S_Freight = HttpContext.Current.Request.Params["sSearch_4"];
                string S_Cost_and_Freight = HttpContext.Current.Request.Params["sSearch_5"];
                string S_POL = HttpContext.Current.Request.Params["sSearch_6"];
                string S_POD = HttpContext.Current.Request.Params["sSearch_7"];
                string S_No_of_Pkgs = HttpContext.Current.Request.Params["sSearch_8"];
                string S_Gross_Weight = HttpContext.Current.Request.Params["sSearch_9"];
                string S_Net_Weight = HttpContext.Current.Request.Params["sSearch_10"];
                string S_ShpngBl_No = HttpContext.Current.Request.Params["sSearch_11"];
                string S_ShpngBl_Date = HttpContext.Current.Request.Params["sSearch_12"];
                string S_Container_No = HttpContext.Current.Request.Params["sSearch_13"];
                string S_BL_No_AWB_No = HttpContext.Current.Request.Params["sSearch_14"];
                string S_BL_No_AWB_Date = HttpContext.Current.Request.Params["sSearch_15"];
                string S_Contact_Person = HttpContext.Current.Request.Params["sSearch_16"];
                string S_Remarks = HttpContext.Current.Request.Params["sSearch_17"];
                string S_VESSEL_Details = HttpContext.Current.Request.Params["sSearch_18"];

                StringBuilder s = new StringBuilder();
                if (S_InvcNo != "")
                    s.Append(" and [PrfmInvcNo] LIKE '%" + S_InvcNo + "%'");
                if (S_Invc_Date != "")
                {
                    DateTime FrmDt = S_Invc_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_Invc_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_Invc_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [PrfmaInvcDt] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Type_of_Consignment != "")
                    s.Append(" and [ShipmentMode] LIKE '%" + S_Type_of_Consignment + "%'");
                if (S_Amount_in_USD_FOB != "")
                    s.Append(" and [FOB Value] LIKE '%" + S_Amount_in_USD_FOB + "%'");
                if (S_Freight != "")
                    s.Append(" and [FREIGHT] LIKE '%" + S_Freight + "%'");
                if (S_Cost_and_Freight != "")
                    s.Append(" and [CIF Amount] LIKE '%" + S_Cost_and_Freight + "%'");
                if (S_POL != "")
                    s.Append(" and [LOAD PORT] LIKE '%" + S_POL + "%'");
                if (S_POD != "")
                    s.Append(" and [TO PORT] LIKE '%" + S_POD + "%'");
                if (S_No_of_Pkgs != "")
                    s.Append(" and [No of Pkgs] LIKE '%" + S_No_of_Pkgs + "%'");
                if (S_Gross_Weight != "")
                    s.Append(" and [GrossWeight] LIKE '%" + S_Gross_Weight + "%'");
                if (S_Net_Weight != "")
                    s.Append(" and [NetWeight] LIKE '%" + S_Net_Weight + "%'");
                if (S_ShpngBl_No != "")
                    s.Append(" and [ShpngBilNmbr] LIKE '%" + S_ShpngBl_No + "%'");
                if (S_ShpngBl_Date != "" && S_ShpngBl_Date != "~")
                {
                    DateTime FrmDt = S_ShpngBl_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_ShpngBl_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_ShpngBl_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ISNULL([ShpngBilDate],'01/01/1900') between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Container_No != "")
                    s.Append(" and [ContainerNumber] LIKE '%" + S_Container_No + "%'");
                if (S_BL_No_AWB_No != "")
                    s.Append(" and [AWL/BL] LIKE '%" + S_BL_No_AWB_No + "%'");

                if (S_BL_No_AWB_Date != "" && S_BL_No_AWB_Date != "~")
                {
                    DateTime FrmDt = S_BL_No_AWB_Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[0].ToString());
                    DateTime EndDat = S_BL_No_AWB_Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(S_BL_No_AWB_Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ISNULL([AWL/BL Date],'01/01/1900') between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (S_Contact_Person != "")
                    s.Append(" and [ContactPerson] LIKE '%" + S_Contact_Person + "%'");

                if (S_Remarks != "")
                    s.Append(" and [Remarks] LIKE '%" + S_Remarks + "%'");

                if (S_VESSEL_Details != "")
                    s.Append(" and [Vessel] LIKE '%" + S_VESSEL_Details + "%'");


                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [PrfmInvcNo] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvcDt] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShipmentMode] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FOB Value] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [FREIGHT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CIF Amount] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [LOAD PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [TO PORT] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [No of Pkgs] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [GrossWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [NetWeight] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilNmbr] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBilDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContainerNumber] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AWL/BL Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ContactPerson] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Remarks] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Vessel] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", PrfmaInvcDt");
                    //orderByClause = orderByClause.Replace("1", ", [CommercialInvoiceDate] ");
                    //orderByClause = orderByClause.Replace("2", ", [PrfmInvcNo] ");
                    //orderByClause = orderByClause.Replace("3", ", [PrfmaInvcDt] ");
                    //orderByClause = orderByClause.Replace("4", ", [ShpngBilNmbr] ");
                    //orderByClause = orderByClause.Replace("5", ", [ShpngBilDate] ");
                    //orderByClause = orderByClause.Replace("6", ", [AWL/BL] ");
                    //orderByClause = orderByClause.Replace("7", ", [AWL/BL Date] ");
                    //orderByClause = orderByClause.Replace("8", ", [SHIPPING BILL STATUS] ");
                    //orderByClause = orderByClause.Replace("9", ", [No. OF PAGES] ");
                    //orderByClause = orderByClause.Replace("11", ", [No. OF ARE-1 FORMS] ");
                    //orderByClause = orderByClause.Replace("12", ", [ARE-1 FORM STATUS] ");
                    //orderByClause = orderByClause.Replace("13", ", [LOAD PORT] ");
                    //orderByClause = orderByClause.Replace("14", ", [DISCHARGE PORT] ");
                    //orderByClause = orderByClause.Replace("15", ", [CHA AGENT] ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "PrfmaInvcDt Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
							declare @MAA TABLE([PrfmInvcNo] nvarchar(500),[PrfmaInvcDt] datetime, ShipmentMode nvarchar(500), 
								[FOB Value] nvarchar(20), FREIGHT nvarchar(20), [CIF Amount] nvarchar(20), [LOAD PORT] nvarchar(max), 
								[TO PORT] nvarchar(max), [No of Pkgs] nvarchar(20), GrossWeight nvarchar(20), NetWeight nvarchar(20),
								[ShpngBilNmbr] nvarchar(500), [ShpngBilDate] datetime,   ContainerNumber nvarchar(500),
								[AWL/BL] nvarchar(max), [AWL/BL Date] datetime, ContactPerson nvarchar(500), Remarks nvarchar(Max), 
								Vessel nvarchar(500), [SBID] nvarchar(max),CreatedDate datetime)
								INSERT INTO
								@MAA ([PrfmInvcNo],[PrfmaInvcDt], ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], 
								[TO PORT], [No of Pkgs], GrossWeight, NetWeight, [ShpngBilNmbr], [ShpngBilDate],  ContainerNumber,
								[AWL/BL], [AWL/BL Date], ContactPerson, Remarks, Vessel, [SBID],CreatedDate)
								Select PrfmInvcNo, PrfmaInvcDt, ShipmentMode, [FOB Value], FREIGHT, [CIF Amount], [LOAD PORT], [TO PORT], 
								[No of Pkgs], GrWeight, NetWeight, ShpngBilNmbr, NULL, ContainerNumber, [AWL/BL], NULL, 
								ContactPerson, Remarks, Vessel, SBID,CreatedDate from dbo.View_Ready_Pending_CustomsClerance_Cntrs_Stuffing_YetToBe_Dispatched 
								{5} {4}         
							SELECT * FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].PrfmaInvcDt)
								FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].PrfmaInvcDt) FROM @MAA) AS TotalDisplayRows, [@MAA].[PrfmInvcNo], 
								[@MAA].[PrfmaInvcDt], [@MAA].ShipmentMode, CAST( [@MAA].[FOB Value] as decimal(18, 2)) as [FOB Value], [@MAA].FREIGHT, 
								CAST( [@MAA].[CIF Amount] as decimal(18, 2)) as [CIF Amount], [@MAA].[LOAD PORT], 
								[@MAA].[TO PORT], [@MAA].[No of Pkgs], [@MAA].GrossWeight, [@MAA].NetWeight, [@MAA].[ShpngBilNmbr], [@MAA].[ShpngBilDate], 
								[@MAA].ContainerNumber, [@MAA].[AWL/BL], [@MAA].[AWL/BL Date], [@MAA].ContactPerson, [@MAA].Remarks, [@MAA].Vessel, 
								[@MAA].[SBID],[@MAA].[CreatedDate] FROM @MAA) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3} order by [CreatedDate] desc";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString(), " where CompanyId = '" + Session["CompanyID"] + "'");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SBID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string PInvoiceNo = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");
                    string ShipmentMode = data["ShipmentMode"].ToString().Replace("\"", "\\\"");
                    ShipmentMode = ShipmentMode.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", ShipmentMode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FOB_Value = data["FOB Value"].ToString().Replace("\"", "\\\"");
                    FOB_Value = FOB_Value.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", FOB_Value.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FREIGHT = data["FREIGHT"].ToString().Replace("\"", "\\\"");
                    FREIGHT = FREIGHT.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", FREIGHT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CIF_Amount = data["CIF Amount"].ToString().Replace("\"", "\\\"");
                    CIF_Amount = CIF_Amount.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CIF_Amount.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string LOAD_PORT = data["LOAD PORT"].ToString().Replace("\"", "\\\"");
                    LOAD_PORT = LOAD_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", LOAD_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string TO_PORT = data["TO PORT"].ToString().Replace("\"", "\\\"");
                    TO_PORT = TO_PORT.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", TO_PORT.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string No_of_Pkgs = data["No of Pkgs"].ToString().Replace("\"", "\\\"");
                    No_of_Pkgs = No_of_Pkgs.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", No_of_Pkgs.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string GrossWeight = data["GrossWeight"].ToString().Replace("\"", "\\\"");
                    GrossWeight = GrossWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", GrossWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string NetWeight = data["NetWeight"].ToString().Replace("\"", "\\\"");
                    NetWeight = NetWeight.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", NetWeight.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string ShpngBilNmbr = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    ShpngBilNmbr = ShpngBilNmbr.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", ShpngBilNmbr.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""12"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");
                    string ContainerNumber = data["ContainerNumber"].ToString().Replace("\"", "\\\"");
                    ContainerNumber = ContainerNumber.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", ContainerNumber.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string AWL_BL = data["AWL/BL"].ToString().Replace("\"", "\\\"");
                    AWL_BL = AWL_BL.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", AWL_BL.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""15"": ""{0:dd/MM/yyyy}""", data["AWL/BL Date"]);
                    sb.Append(",");
                    string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
                    ContactPerson = ContactPerson.Replace("\t", "-");
                    sb.AppendFormat(@"""16"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Remarks = data["Remarks"].ToString().Replace("\"", "\\\"");
                    Remarks = Remarks.Replace("\t", "-");
                    sb.AppendFormat(@"""17"": ""{0}""", Remarks.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Vessel = data["Vessel"].ToString().Replace("\"", "\\\"");
                    Vessel = Vessel.Replace("\t", "-");
                    sb.AppendFormat(@"""18"": ""{0}""", Vessel.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        #region Working in Progress

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string CARGO_SALES_COMMERCIAL_INVOICE_PREPARED()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                #region Not In Use
                string ExpInv = HttpContext.Current.Request.Params["sSearch_0"];
                string ExpInvcDt = HttpContext.Current.Request.Params["sSearch_1"];
                string ModOfShpmnt = HttpContext.Current.Request.Params["sSearch_2"];
                string FobVal = HttpContext.Current.Request.Params["sSearch_3"];
                string B_FRT_Amount = HttpContext.Current.Request.Params["sSearch_4"];
                string CifVal = HttpContext.Current.Request.Params["sSearch_5"];
                string B_PortOfLoading = HttpContext.Current.Request.Params["sSearch_6"];
                string B_PortOfDischarge = HttpContext.Current.Request.Params["sSearch_7"];
                string B_TotPkgs = HttpContext.Current.Request.Params["sSearch_8"];
                string B_GrossWeight = HttpContext.Current.Request.Params["sSearch_9"];
                string B_NetWeight = HttpContext.Current.Request.Params["sSearch_10"];
                string B_ShpngBillNo = HttpContext.Current.Request.Params["sSearch_11"];
                string B_ShpngBillDate = HttpContext.Current.Request.Params["sSearch_12"];
                string B_ContainerNOs = HttpContext.Current.Request.Params["sSearch_13"];
                string B_CommercialINVNo = HttpContext.Current.Request.Params["sSearch_15"];
                string B_CommercialINVDate = HttpContext.Current.Request.Params["sSearch_16"];


                StringBuilder s = new StringBuilder();
                if (ExpInv != "")
                    s.Append(" and v.ShipmentINVNo LIKE '%" + ExpInv + "%'");

                if (ExpInvcDt != "")
                {
                    DateTime FrmDt = ExpInvcDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ExpInvcDt.Split('~')[0].ToString());
                    DateTime EndDat = ExpInvcDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ExpInvcDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and v.[INV Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (ModOfShpmnt != "")
                    s.Append(" and v.ShipmentMode like  '%" + ModOfShpmnt + "%'");

                if (CifVal != "")
                    s.Append(" and v.[CIF Val] like  '%" + CifVal + "%'");

                if (FobVal != "")
                    s.Append(" and v.FOBValueINR like '%" + FobVal + "%'");

                if (B_FRT_Amount != "")
                    s.Append(" and v.FRT_Amount like '%" + B_FRT_Amount + "%'");

                if (B_PortOfLoading != "")
                    s.Append(" and [Port Of Loading] like  '%" + B_PortOfLoading + "%'");

                if (B_PortOfDischarge != "")
                    s.Append(" and [Port Of Discharge] like  '%" + B_PortOfDischarge + "%'");

                if (B_TotPkgs != "")
                    s.Append(" and v.TotPkgs like '%" + B_TotPkgs + "%'");

                if (B_NetWeight != "")
                    s.Append(" and v.NetWeight like '%" + B_NetWeight + "%'");

                if (B_GrossWeight != "")
                    s.Append(" and v.GrossWeight like '%" + B_GrossWeight + "%'");

                if (B_ShpngBillNo != "")
                    s.Append(" and v.ShpngBillNo like  '%" + B_ShpngBillNo + "%'");

                if (B_ShpngBillDate != "")
                {
                    DateTime FrmDt = B_ShpngBillDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_ShpngBillDate.Split('~')[0].ToString());
                    DateTime EndDat = B_ShpngBillDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_ShpngBillDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and v.[ShpngBill Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (B_ContainerNOs != "")
                    s.Append(" and v.[Container NOs] like  '%" + B_ContainerNOs + "%'");

                if (B_CommercialINVNo != "")
                    s.Append(" and v.CommercialINVNo like  '%" + B_CommercialINVNo + "%'");

                if (B_CommercialINVDate != "")
                {
                    DateTime FrmDt = B_CommercialINVDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(B_CommercialINVDate.Split('~')[0].ToString());
                    DateTime EndDat = B_CommercialINVDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(B_CommercialINVDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and v.[CommercialINV Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                #endregion

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE ShipmentINVNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [INV Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FOBValueINR LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FRT_Amount LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Port Of Loading] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Port Of Discharge] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR TotPkgs LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR NetWeight LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR GrossWeight LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ShpngBillNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShpngBill Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Container NOs] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CommercialINVNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [CommercialINV Date] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", ID");
                    //orderByClause = orderByClause.Replace("1", ", [CommercialInvoiceDate] ");
                    //orderByClause = orderByClause.Replace("2", ", [PrfmInvcNo] ");
                    //orderByClause = orderByClause.Replace("3", ", [PrfmaInvcDt] ");
                    //orderByClause = orderByClause.Replace("4", ", [ShpngBilNmbr] ");
                    //orderByClause = orderByClause.Replace("5", ", [ShpngBilDate] ");
                    //orderByClause = orderByClause.Replace("6", ", [AWL/BL] ");
                    //orderByClause = orderByClause.Replace("7", ", [AWL/BL Date] ");
                    //orderByClause = orderByClause.Replace("8", ", [SHIPPING BILL STATUS] ");
                    //orderByClause = orderByClause.Replace("9", ", [No. OF PAGES] ");
                    //orderByClause = orderByClause.Replace("11", ", [No. OF ARE-1 FORMS] ");
                    //orderByClause = orderByClause.Replace("12", ", [ARE-1 FORM STATUS] ");
                    //orderByClause = orderByClause.Replace("13", ", [LOAD PORT] ");
                    //orderByClause = orderByClause.Replace("14", ", [DISCHARGE PORT] ");
                    //orderByClause = orderByClause.Replace("15", ", [CHA AGENT] ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "pf.ID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
							declare @MAA TABLE(ID uniqueidentifier, [ShipmentINVNo] nvarchar(500) ,[INV Date] datetime,ShipmentMode nvarchar(500),FOBValueINR nvarchar(500),
							FRT_Amount nvarchar(500),[CIF Val] nvarchar(500),[Port Of Loading] nvarchar(500),
							[Port Of Discharge] nvarchar(500),TotPkgs nvarchar(500),NetWeight nvarchar(500),GrossWeight nvarchar(500),
							ShpngBillNo nvarchar(500), [ShpngBill Date] datetime,[Container NOs] nvarchar(500),CommercialINVNo nvarchar(500), 
							[CommercialINV Date] datetime, [AirwayBill/BillOfLading] nvarchar(500), CreatedDate datetime)
							INSERT INTO
								@MAA (ID, [ShipmentINVNo] ,[INV Date],ShipmentMode,FOBValueINR,FRT_Amount,[CIF Val],
							[Port Of Loading],[Port Of Discharge],TotPkgs,NetWeight,GrossWeight,ShpngBillNo, 
							[ShpngBill Date],[Container NOs],CommercialINVNo, [CommercialINV Date], [AirwayBill/BillOfLading], CreatedDate)
							select v.ID,v.ShipmentINVNo,v.[INV Date],v.ShipmentMode,v.FOBValueINR,v.FRT_Amount,
							v.[CIF Val],v.[Port Of Loading],v.[Port Of Discharge],v.TotPkgs,v.NetWeight,v.GrossWeight,v.ShpngBillNo,v.[ShpngBill Date],
							v.[Container NOs],v.CommercialINVNo,v.[CommercialINV Date],v.[AirwayBill/BillOfLading], v.CreatedDate from View_CommercialINV_Prepared v 
							{4}                   
							SELECT * FROM
							(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID)
							FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, 
							[@MAA].[ID], 
							[@MAA].[ShipmentINVNo], 
							[@MAA].[INV Date], 
							[@MAA].ShipmentMode,
							[@MAA].FOBValueINR,
							[@MAA].FRT_Amount, 
							[@MAA].[CIF Val], 
							[@MAA].[Port Of Loading], 
							[@MAA].[Port Of Discharge], 
							[@MAA].TotPkgs, 
							[@MAA].NetWeight, 
							[@MAA].GrossWeight, 
							[@MAA].ShpngBillNo, 
							[@MAA].[ShpngBill Date], 
							[@MAA].[Container NOs], 
							[@MAA].CommercialINVNo,
							[@MAA].[CommercialINV Date],
							[@MAA].[AirwayBill/BillOfLading],
							[@MAA].CreatedDate 
							FROM @MAA {1}) RawResults) Results WHERE
							RowNumber BETWEEN {2} AND {3} order by CreatedDate desc ";

                string where = " where v.IsActive<> 0 and v.CompanyId = '" + Session["CompanyID"] + "'";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ? where : where + s.ToString());

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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    string PInvoiceNo = data["ShipmentINVNo"].ToString().Replace("\"", "\\\"");
                    PInvoiceNo = PInvoiceNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", PInvoiceNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["INV Date"]);
                    sb.Append(",");

                    string PkngNo = data["ShipmentMode"].ToString().Replace("\"", "\\\"");
                    PkngNo = PkngNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PkngNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""3"": ""{0}""", data["FOBValueINR"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""4"": ""{0}""", data["FRT_Amount"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0}""", data["CIF Val"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""6"": ""{0}""", data["Port Of Loading"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""7"": ""{0}""", data["Port Of Discharge"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""8"": ""{0}""", data["TotPkgs"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""9"": ""{0}""", data["NetWeight"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""10"": ""{0}""", data["GrossWeight"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""11"": ""{0}""", data["ShpngBillNo"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""12"": ""{0:dd/MM/yyyy}""", data["ShpngBill Date"]);
                    sb.Append(",");
                    sb.AppendFormat(@"""13"": ""{0}""", data["Container NOs"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""14"": ""{0}""", data["AirwayBill/BillOfLading"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""15"": ""{0}""", data["CommercialINVNo"].ToString().Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""16"": ""{0:dd/MM/yyyy}""", data["CommercialINV Date"]);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }

        #endregion

        /// <summary>
        /// FOR Pending Pink Copies
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetPendngPnkCopes()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string P_FpoNum = HttpContext.Current.Request.Params["sSearch_0"];
                string P_Ct1Num = HttpContext.Current.Request.Params["sSearch_1"];
                string P_Ct1Date = HttpContext.Current.Request.Params["sSearch_2"];
                string P_Ct1basedHydMum = HttpContext.Current.Request.Params["sSearch_3"];
                string P_ARE1ID = HttpContext.Current.Request.Params["sSearch_4"];
                string P_CreatedDate = HttpContext.Current.Request.Params["sSearch_5"];
                string P_ShpngBillNo = HttpContext.Current.Request.Params["sSearch_6"];
                string P_ShpngBilDate = HttpContext.Current.Request.Params["sSearch_7"];
                string P_PrfmaInvNum = HttpContext.Current.Request.Params["sSearch_8"];
                string P_Chaagent = HttpContext.Current.Request.Params["sSearch_9"];
                string P_SuppNm = HttpContext.Current.Request.Params["sSearch_10"];
                string P_CTOneVal = HttpContext.Current.Request.Params["sSearch_11"];
                string P_Status = HttpContext.Current.Request.Params["sSearch_12"];
                string P_ExiseINVNo = HttpContext.Current.Request.Params["sSearch_16"];
                string P_ExINVDT = HttpContext.Current.Request.Params["sSearch_17"];


                StringBuilder s = new StringBuilder();
                if (P_FpoNum != "")
                    s.Append(" and [FpoNum] LIKE '%" + P_FpoNum + "%'");
                if (P_Ct1Num != "")
                    s.Append(" and [Ct1Num] LIKE '%" + P_Ct1Num + "%'");
                if (P_Ct1Date != "")
                {
                    DateTime FrmDt = P_Ct1Date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_Ct1Date.Split('~')[0].ToString());
                    DateTime EndDat = P_Ct1Date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_Ct1Date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [Ct1Date] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_Ct1basedHydMum != "")
                    s.Append(" and [Ct1basedHydMum] LIKE '%" + P_Ct1basedHydMum + "%'");
                if (P_ARE1ID != "")
                    s.Append(" and [ARE1ID] LIKE '%" + P_ARE1ID + "%'");
                if (P_CreatedDate != "")
                {
                    DateTime FrmDt = P_CreatedDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_CreatedDate.Split('~')[0].ToString());
                    DateTime EndDat = P_CreatedDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_CreatedDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [CreatedDate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (P_ShpngBillNo != "")
                    s.Append(" and [ShpngBillNo] LIKE '%" + P_ShpngBillNo + "%'");
                if (P_ShpngBilDate != "")
                {
                    DateTime FrmDt = P_ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = P_ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ShpngBillDT] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (P_PrfmaInvNum != "")
                    s.Append(" and [PrfmaInvNum] LIKE '%" + P_PrfmaInvNum + "%'");
                if (P_Chaagent != "")
                    s.Append(" and [Chaagent] LIKE '%" + P_Chaagent + "%'");
                if (P_SuppNm != "")
                    s.Append(" and [SuppNm] LIKE '%" + P_SuppNm + "%'");
                if (P_CTOneVal != "")
                    s.Append(" and  [CTOneVal] LIKE '%" + P_CTOneVal + "%'");
                if (P_Status != "")
                    s.Append(" and [Status] LIKE '%" + P_Status + "%'");
                if (P_ExiseINVNo != "")
                    s.Append(" and [ExiseINVNo] LIKE '%" + P_ExiseINVNo + "%'");
                if (P_ExINVDT != "")
                {
                    DateTime FrmDt = P_ExINVDT.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(P_ExINVDT.Split('~')[0].ToString());
                    DateTime EndDat = P_ExINVDT.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(P_ExINVDT.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [ExINVDT] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE [FpoNum] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Ct-1Num] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Date] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Ct1FrmAtHydMum] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Are-1Num] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [AreDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShppngBllNum] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ShppngBllDate] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [PrfmaInvNum] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Chaagent] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [SupplNm] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Ct-1Val] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [Status] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ExInvNum] LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR [ExINVDT] LIKE ");
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

                    orderByClause = orderByClause.Replace("0", ", Date");
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

                string query = @"  
							declare @MAA TABLE([CtpId] uniqueidentifier,[FpoNum] nvarchar(500),[Ct-1Num] nvarchar(500),[Date] datetime,[Ct1FrmAtHydMum] nvarchar(100),
												[Are-1Num] nvarchar(500), [AreDate] datetime,[ShppngBllNum] nvarchar(max),[ShppngBllDate] datetime, 
												[PrfmaInvNum] nvarchar(max),[Chaagent] nvarchar(400),
												[SupplNm] nvarchar(400), [Ct-1Val] nvarchar(400),[Status] nvarchar(20),[Are1forms] nvarchar(100),    
												[ExInvNum] nvarchar(max),[ExInvDt] datetime)
							INSERT
							INTO
								@MAA ([CtpId],[FpoNum],[Ct-1Num],[Date],[Ct1FrmAtHydMum],[Are-1Num],[AreDate],[ShppngBllNum],[ShppngBllDate],[PrfmaInvNum],[Chaagent],[SupplNm],[Ct-1Val],[Status],[Are1forms],[ExInvNum],[ExInvDt])
										SELECT [CTpID],[FpoNum],[Ct1Num],[Ct1Date],[Ct1basedHydMum],[ARE1ID], [CreatedDate],[ShpngBillNo], [ShpngBillDT],
										   [PrfmaInvNum],[Chaagent],[SuppNm],[CTOneVal],[Status],[ARE1Forms],[ExiseINVNo],[ExINVDT] FROM [dbo].[View_PendingPinkCopies]
									 {5} {4}               

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].CtpId)
										 FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].[FpoNum]) FROM @MAA {1}) AS TotalDisplayRows			   
										 ,[@MAA].[CtpId],[@MAA].[FpoNum],[@MAA].[Ct-1Num], [@MAA].[Date],[@MAA].[Ct1FrmAtHydMum], [@MAA].[Are-1Num], [@MAA].[AreDate]
										 ,[@MAA].[ShppngBllNum], [@MAA].[ShppngBllDate], [@MAA].[PrfmaInvNum] ,[@MAA].[Chaagent] ,[@MAA].[SupplNm], [@MAA].[Ct-1Val], [@MAA].[Status]
										 , [@MAA].[Are1forms],[@MAA].[ExInvNum], [@MAA].[ExInvDt]
									  FROM @MAA {1}) RawResults) Results WHERE
									  RowNumber BETWEEN {2} AND {3} order by [Date] desc"; //order by ForeignEnquireId Desc";//"where f.IsActive <> 0 "
                string Where = "where ComapnyId = '" + Context.Session["CompanyID"].ToString() + "'";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString(), Where, "");
                //s.Clear();
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CtpId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
                    //sb.Append(",");
                    string FpoNum = data["FpoNum"].ToString().Replace("\"", "\\\"");
                    FpoNum = FpoNum.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", FpoNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    //sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["PrfmaInvcDt"]);
                    //sb.Append(",");
                    string Ct1Num = data["Ct-1Num"].ToString().Replace("\"", "\\\"");
                    Ct1Num = Ct1Num.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", Ct1Num.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["Date"]);
                    sb.Append(",");
                    string Ct1FrmAtHydMum = data["Ct1FrmAtHydMum"].ToString().Replace("\"", "\\\"");
                    Ct1FrmAtHydMum = Ct1FrmAtHydMum.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Ct1FrmAtHydMum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Are1Num = data["Are-1Num"].ToString().Replace("\"", "\\\"");
                    Are1Num = Are1Num.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", Are1Num.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["AreDate"]);
                    sb.Append(",");
                    string ShppngBllNum = data["ShppngBllNum"].ToString().Replace("\"", "\\\"");
                    ShppngBllNum = ShppngBllNum.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ShppngBllNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""7"": ""{0:dd/MM/yyyy}""", data["ShppngBllDate"]);
                    sb.Append(",");
                    string PrfmaInvNum = data["PrfmaInvNum"].ToString().Replace("\"", "\\\"");
                    PrfmaInvNum = PrfmaInvNum.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", PrfmaInvNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Chaagent = data["Chaagent"].ToString().Replace("\"", "\\\"");
                    Chaagent = Chaagent.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", Chaagent.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string SupplNm = data["SupplNm"].ToString().Replace("\"", "\\\"");
                    SupplNm = SupplNm.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", SupplNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Ct1Val = data["Ct-1Val"].ToString().Replace("\"", "\\\"");
                    Ct1Val = Ct1Val.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", Ct1Val.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Status = data["Status"].ToString().Replace("\"", "\\\"");
                    Status = Status.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", Status.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Are1forms = data["Are1forms"].ToString().Replace("\"", "\\\"");
                    string[] Are1formsplit = Are1forms.Split(',');
                    Are1forms = Are1forms.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", Are1formsplit[0].Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""14"": ""{0}""", Are1formsplit[1].Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""15"": ""{0}""", Are1formsplit[2].Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string ExInvNum = data["ExInvNum"].ToString().Replace("\"", "\\\"");
                    ExInvNum = ExInvNum.Replace("\t", "-");
                    sb.AppendFormat(@"""16"": ""{0}""", ExInvNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""17"": ""{0:dd/MM/yyyy}""", data["ExInvDt"]);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Reports/ErrorLog"), "Report Service", ex.Message.ToString());
                return "";
            }
        }


        /// <summary>
        /// FOR FOREIGN ENQUIRY
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        //[WebMethod(EnableSession = true)]
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetStatusFERecv()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FeNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string RDate = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string Dept = HttpContext.Current.Request.Params["sSearch_4"];
                string Cust = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];
                string createdBy = HttpContext.Current.Request.Params["sSearch_9"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FeNo != "")
                    s.Append(" and EnquireNumber LIKE '%" + FeNo + "%'");
                if (RDate != "")
                {
                    DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                    DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and StatusTypeId LIKE '%" + Status + "%'");
                if (Dept != "")
                    s.Append(" and DepartmentId LIKE '%" + Dept + "%'");
                if (Cust != "")
                    s.Append(" and CusmorId LIKE '%" + Cust + "%'");
                if (createdBy != "")
                    s.Append(" and CreatedBy LIKE '%" + createdBy + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE EnquiryDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR EnquireNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ReceivedDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR DepartmentId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CusmorId LIKE ");
                    sb.Append(wrappedSearch);


                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                sb.Append(" ");

                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                orderByClause = "0 DESC";

                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ForeignEnquireId ");
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
                    orderByClause = "ID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
							declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId Uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX), 
							IsRegret nvarchar(MAX), Remarks nvarchar(MAX), CreatedDate datetime, CreatedBy nvarchar(MAX))
							INSERT INTO @MAA ( EnquireNumber,EnquiryDate,  ReceivedDate, Subject, ForeignEnquireId ,  DepartmentId, CusmorId, 
								StatusTypeId, IsRegret, Remarks, CreatedDate, CreatedBy)
								Select EnquireNumber,EnquiryDate,  ReceivedDate, Subject, ForeignEnquireId ,  DepartmentId, CusmorId, 
								StatusTypeId, IsRegret, Remarks, CreatedDate, f.Created_By from View_GetStatusFERec f
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].EnquireNumber)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].EnquiryDate
										   ,[@MAA].EnquireNumber      
										   ,[@MAA].ReceivedDate
										   ,[@MAA].Subject
										   ,[@MAA].ForeignEnquireId     
										   ,[@MAA].StatusTypeId 
										   ,[@MAA].DepartmentId
										   ,[@MAA].CusmorId 
										   ,[@MAA].IsRegret
										   ,[@MAA].Remarks
										   ,[@MAA].CreatedDate 
                                           ,[@MAA].CreatedBy
									  FROM
										  @MAA {1}) RawResults) Results WHERE
										 RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    "where f.IsActive <> 0 and StatusId <= 10  and f.CompanyId =" + "'" + CompanyID + "'" : "where f.IsActive <> 0 and StatusId <= 10 and f.CompanyId = " + "'" + CompanyID + "'" + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                    string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");

                    EnqNo = EnqNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", EnqNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    if (Convert.ToDateTime(data["EnquiryDate"]).ToString("yyyy-MM-dd") == "9999-12-31")
                    {
                        sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", "");
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
                        sb.Append(",");
                    }

                    if (Convert.ToDateTime(data["ReceivedDate"]).ToString("yyyy-MM-dd") == "9999-12-31")
                    {
                        sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", "");
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
                        sb.Append(",");
                    }

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    Subjt = Subjt.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");
                    DeptID = DeptID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                    StatIDd = StatIDd.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""7"": ""{0}""", data["IsRegret"].ToString());
                    sb.Append(",");

                    string Remarkss = data["Remarks"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""8"": ""{0}""", Remarkss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string createdByy = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""9"": ""{0}""", createdByy.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Added Items WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FPO's Awaiting
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFPOAwaited()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string FENo = HttpContext.Current.Request.Params["sSearch_0"];
                string FEDt = HttpContext.Current.Request.Params["sSearch_1"];
                string FQNo = HttpContext.Current.Request.Params["sSearch_2"];
                string FQDt = HttpContext.Current.Request.Params["sSearch_3"];
                string FQAmt = HttpContext.Current.Request.Params["sSearch_4"];
                string Cust = HttpContext.Current.Request.Params["sSearch_5"];
                string Stat = HttpContext.Current.Request.Params["sSearch_6"];
                string Remarks = HttpContext.Current.Request.Params["sSearch_7"];
                string CreatedBy = HttpContext.Current.Request.Params["sSearch_8"];
                StringBuilder s = new StringBuilder();
                if (FENo != "")
                    s.Append(" and EnquireNumber LIKE '%" + FENo + "%'");

                if (FEDt != "")
                {
                    DateTime FrmDt = FEDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(FEDt.Split('~')[0].ToString());
                    DateTime EndDat = FEDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(FEDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (FQNo != "")
                    s.Append(" and Quotationnumber LIKE '%" + FQNo + "%'");

                if (FQDt != "")
                {
                    DateTime FrmDt = FQDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(FQDt.Split('~')[0].ToString());
                    DateTime EndDat = FQDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(FQDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and QuotationDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (FQAmt != "")
                    s.Append(" and TotalAmount LIKE '%" + FQAmt + "%'");

                if (Cust != "")
                    s.Append(" and CustomerName LIKE '%" + Cust + "%'");
                if (Stat != "")
                {
                    if (Stat.ToUpper() == "CANCEL" || Stat.ToUpper() == "C" || Stat.ToUpper() == "CA" || Stat.ToUpper() == "CAN" || Stat.ToUpper() == "CANC" || Stat.ToUpper() == "CANCE")
                        Stat = "44";
                    else if (Stat.ToUpper() == "PLACEDTOOTHERS" || Stat.ToUpper() == "P" || Stat.ToUpper() == "PL"
                        || Stat.ToUpper() == "PLA" || Stat.ToUpper() == "PLAC" || Stat.ToUpper() == "PLACE" || Stat.ToUpper() == "PLACED"
                        || Stat.ToUpper() == "PLACEDT" || Stat.ToUpper() == "PLACEDTO" || Stat.ToUpper() == "PLACEDTOO" || Stat.ToUpper() == "PLACEDTOOT"
                        || Stat.ToUpper() == "PLACEDTOOTH" || Stat.ToUpper() == "PLACEDTOOTHE" || Stat.ToUpper() == "PLACEDTOOTHER")
                        Stat = "43";
                    s.Append(" and b.StatusTypeId LIKE '%" + Stat + "%'");
                }
                if (Remarks != "")
                    s.Append(" and Remarks LIKE '%" + Remarks + "%'");
                if (CreatedBy != "")
                    s.Append(" and CreatedBy LIKE '%" + CreatedBy + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE EnquireNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR EnquiryDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Quotationnumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR QuotationDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR TotalAmount LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Stat LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Remarks LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CreatedBy LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }

                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;

                sb.ToString();

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ForeignQuotationId ");

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

                string query = @"  
						declare @MAA TABLE(FQID uniqueidentifier,FEID uniqueidentifier,ForeignQuotationId uniqueidentifier,EnquireNumber nvarchar(MAX),EnquiryDate date,Quotationnumber nvarchar(MAX),QuotationDate date,
						TotalAmount nvarchar(MAX),CustomerName nvarchar(MAX), Stat bigint, Remarks nvarchar(MAX),CreatedBy nvarchar(MAX), CompanyId uniqueidentifier);
						
						INSERT INTO @MAA (FQID,FEID,ForeignQuotationId, EnquireNumber,EnquiryDate,Quotationnumber,QuotationDate,TotalAmount,CustomerName,Stat,Remarks,CreatedBy,CompanyId)
						SELECT FQID,FEID,FrnQuotationID,EnquireNumber,EnquiryDate,Quotationnumber,QuotationDate,TotalAmount,CustomerName,StatusTypeId,Remarks,CreatedBy,CompanyId from View_FPOAwaited b
							{4}
							select * From 
								
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].ForeignQuotationId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].FQID) FROM @MAA {1}) AS TotalDisplayRows
										   ,[@MAA].FQID
										   ,[@MAA].FEID
										   ,[@MAA].ForeignQuotationId
										   ,[@MAA].EnquireNumber
										   ,[@MAA].EnquiryDate
										   ,[@MAA].Quotationnumber
										   ,[@MAA].QuotationDate
										   ,[@MAA].TotalAmount
										   ,[@MAA].CustomerName
										   ,[@MAA].Stat
										   ,[@MAA].Remarks
                                           ,[@MAA].CreatedBy
										   ,[@MAA].CompanyId         
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by QuotationDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   " where IsActive <> 0 and CompanyId = " + "'" + CompanyID + "'" : " where IsActive <> 0 and CompanyId =" + "'" + CompanyID + "'" + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["FQID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    string FENO = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                    //PartNo = PartNo.Replace("'", "&#39;");
                    //PartNo = PartNo.Replace(@"\", "-");
                    //RecDt = RecDt.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", FENO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["EnquiryDate"]);
                    sb.Append(",");

                    string FQNO = data["Quotationnumber"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
                    sb.AppendFormat(@"""2"": ""{0}""", FQNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""3"": ""{0:dd-MM-yyyy}""", data["QuotationDate"]);
                    sb.Append(",");


                    string TotAmt = data["TotalAmount"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", TotAmt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Status = "";
                    string statID = data["Stat"].ToString();
                    //if (statID == "40")
                    //    Status = "";
                    if (statID == "43")
                        Status = "PLACEDTOOTHERS";
                    if (statID == "44")
                        Status = "CANCEL";
                    sb.AppendFormat(@"""6"": ""{0}""", Status);
                    sb.Append(",");

                    string Remarkss = data["Remarks"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""7"": ""{0}""", Remarkss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string CreatedB = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""8"": ""{0}""", CreatedB.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "FQ Status WebService", ex.Message.ToString());
                return "";
            }
        }


        /// <summary>
        /// FPO's Awaiting
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFPOStatus()
        {
            int sEcho = 0, iDisplayLength = 0, iDisplayStart = 0;
            var totalDisplayRecords = "";
            int totalRecords = 0;
            string outputJson = string.Empty;
            var sb = new StringBuilder();
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FpoNo = HttpContext.Current.Request.Params["sSearch_0"];
                string FpoDt = HttpContext.Current.Request.Params["sSearch_1"];
                string FENo = HttpContext.Current.Request.Params["sSearch_2"];
                string FEDt = HttpContext.Current.Request.Params["sSearch_3"];
                //string FQNo = HttpContext.Current.Request.Params["sSearch_4"];
                //string FQDt = HttpContext.Current.Request.Params["sSearch_5"];
                string FQAmt = HttpContext.Current.Request.Params["sSearch_4"];
                string Cust = HttpContext.Current.Request.Params["sSearch_5"];
                string Stat = HttpContext.Current.Request.Params["sSearch_6"];
                string Remarks = HttpContext.Current.Request.Params["sSearch_7"];
                string CreatedBy = HttpContext.Current.Request.Params["sSearch_8"];

                StringBuilder s = new StringBuilder();
                if (FpoNo != "")
                    s.Append(" and FPONum LIKE '%" + FpoNo + "%'");

                if (FpoDt != "")
                {
                    DateTime FpoFrmDt = FpoDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(FpoDt.Split('~')[0].ToString());
                    DateTime FpoEndDat = FpoDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(FpoDt.Split('~')[1].ToString());
                    if (FpoFrmDt.ToShortDateString() != "1/1/0001" && FpoEndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and fp.FpoDate between '" + FpoFrmDt.ToString("MM/dd/yyyy") + "' and '" + FpoEndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FENo != "")
                    s.Append(" and EnquireNumber LIKE '%" + FENo + "%'");

                if (FEDt != "")
                {
                    DateTime FrmDt = FEDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(FEDt.Split('~')[0].ToString());
                    DateTime EndDat = FEDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(FEDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FQAmt != "")
                    s.Append(" and TotalAmount LIKE '%" + FQAmt + "%'");

                if (Cust != "")
                    s.Append(" and CustomerName LIKE '%" + Cust + "%'");
                if (Stat != "")
                    s.Append(" and  Stat LIKE '%" + Stat + "%'");
                if (Remarks != "")
                    s.Append(" and Remarks LIKE '%" + Remarks + "%'");
                if (CreatedBy != "")
                    s.Append(" and CreatedBy LIKE '%" + Remarks + "%'");


                //var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FpoNum LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FpoDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR EnquireNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR EnquiryDate LIKE ");
                    sb.Append(wrappedSearch);
                    //sb.Append(" OR Quotationnumber LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR QuotationDate LIKE ");
                    //sb.Append(wrappedSearch);
                    sb.Append(" OR TotalAmount LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Stat LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Remarks LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CreatedBy LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }

                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;

                sb.ToString();

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", FpoId ");

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

                string query = @"  
						declare @MAA TABLE(FpoId uniqueidentifier,FpoNum nvarchar(max),FpoDate datetime, FEID uniqueidentifier,ForeignQuotationId uniqueidentifier,EnquireNumber nvarchar(MAX),EnquiryDate date,Quotationnumber nvarchar(MAX),QuotationDate date,
						TotalAmount nvarchar(MAX),CustomerName nvarchar(MAX), Stat nvarchar(50), Remarks nvarchar(MAX), CreatedBy nvarchar(MAX));

						INSERT INTO @MAA (FpoId,FpoNum,FpoDate, EnquireNumber,EnquiryDate,TotalAmount,CustomerName,Stat,Remarks, CreatedBy)
                        
                        Select FpoId,FpoNum,FpoDate, EnquireNumber,EnquiryDate,TotalAmount,CustomerName,Stat,Remarks, fp.CreatedBy from View_GetFPOStatus fp where fp.StatusTypeId >= 50 and  fp.StatusTypeId < 60
						
						{4}
						select * From
								
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].FpoId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].FpoId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].FpoId,[@MAA].FpoNum,[@MAA].FpoDate
										   ,[@MAA].EnquireNumber
										   ,[@MAA].EnquiryDate      
										   ,[@MAA].TotalAmount
										   ,[@MAA].CustomerName
										   ,[@MAA].Stat
										   ,[@MAA].Remarks
                                           ,[@MAA].CreatedBy
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by FpoDate Desc";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " and fp.CompanyId = " + "'" + CompanyID + "'" : s.ToString() + " and fp.CompanyId = " + "'" + CompanyID + "'");
                s.Clear();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var DB = new SqlCommand();
                DB.Connection = conn;
                DB.CommandText = query;
                var data = DB.ExecuteReader();

                //var totalDisplayRecords = "";
                //var totalRecords = "";
                //string outputJson = string.Empty;

                var rowClass = "";
                var count = 0;

                while (data.Read())
                {

                    if (totalRecords == 0)
                    {
                        totalRecords = Convert.ToInt32(data["TotalRows"].ToString());
                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    }
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["FpoId"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    string FpoNO = data["FpoNum"].ToString().Replace("\"", "\\\"");
                    //PartNo = PartNo.Replace("'", "&#39;");
                    //PartNo = PartNo.Replace(@"\", "-");
                    //RecDt = RecDt.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", FpoNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["FpoDate"]);
                    sb.Append(",");

                    string FENO = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                    //PartNo = PartNo.Replace("'", "&#39;");
                    //PartNo = PartNo.Replace(@"\", "-");
                    //RecDt = RecDt.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", FENO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string EDate = data["EnquiryDate"].ToString().Replace("\"", "\\\"");
                    if (Convert.ToDateTime(EDate).ToString("dd-MM-yyyy") == "31-12-9999")
                    {
                        sb.AppendFormat(@"""3"": """"", "");
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""3"": ""{0:dd-MM-yyyy}""", data["EnquiryDate"]);
                        sb.Append(",");
                    }

                    //string FQNO = data["Quotationnumber"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
                    //sb.AppendFormat(@"""4"": ""{0}""", FQNO.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data["QuotationDate"]);
                    //sb.Append(",");


                    string TotAmt = data["TotalAmount"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", TotAmt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""6"": ""{0}""", data["Stat"].ToString());
                    sb.Append(",");

                    string Remarkss = data["Remarks"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""7"": ""{0}""", Remarkss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CreatedByy = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""8"": ""{0}""", CreatedByy.Replace(Environment.NewLine, "\\n"));
                    sb.Append("},");
                }
                conn.Close();
                //// handles zero records
                //if (totalRecords.Length == 0)
                //{
                //    sb.Append("{");
                //    sb.Append(@"""sEcho"": ");
                //    sb.AppendFormat(@"""{0}""", sEcho);
                //    sb.Append(",");
                //    sb.Append(@"""iTotalRecords"": 0");
                //    sb.Append(",");
                //    sb.Append(@"""iTotalDisplayRecords"": 0");
                //    sb.Append(", ");
                //    sb.Append(@"""aaData"": [ ");
                //    sb.Append("]}");
                //    outputJson = sb.ToString();

                //    return outputJson;
                //}
                if (totalRecords > 0)
                {
                    outputJson = sb.Remove(sb.Length - 1, 1).ToString();
                    sb.Clear();

                    sb.Append("{");
                    sb.Append(@"""sEcho"": ");
                    sb.AppendFormat(@"""{0}""", sEcho);
                    sb.Append(",");
                    sb.Append(@"""iTotalRecords"": ");
                    sb.Append(totalRecords.ToString());
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
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "FQ Status WebService", ex.Message.ToString());
                //return "";
            }
            sb.Append("{");
            sb.Append(@"""sEcho"": ");
            sb.AppendFormat(@"""{0}""", sEcho);
            sb.Append(",");
            sb.Append(@"""iTotalRecords"": 0");
            sb.Append(",");
            sb.Append(@"""iTotalDisplayRecords"": 0");
            sb.Append(", ");
            sb.Append(@"""aaData"": [ ]}");
            outputJson = sb.ToString();
            return outputJson;
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;
            return result;
        }
    }
}
