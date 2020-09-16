using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ServiceModel.Web;
using BAL;

namespace VOMS_ERP.Logistics
{
    /// <summary>
    /// Summary description for LogisticsWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class LogisticsWS : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetPISItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Cust = HttpContext.Current.Request.Params["sSearch_0"];
                string FPOs = HttpContext.Current.Request.Params["sSearch_1"];
                string LPOs = HttpContext.Current.Request.Params["sSearch_2"];
                string Supplier = HttpContext.Current.Request.Params["sSearch_3"];
                string RqstNo = HttpContext.Current.Request.Params["sSearch_4"];
                string RqstDT = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (RqstDT != "")
                {
                    DateTime FrmDt = RqstDT.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RqstDT.Split('~')[0].ToString());
                    DateTime EndDat = RqstDT.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RqstDT.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and cast(CreatedDate as date) between  '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Cust != "")
                    s.Append(" and dbo.GetActiveCustomerName(Cust) LIKE '%" + Cust + "%'");
                if (FPOs != "")
                    s.Append(" and dbo.FN_MergeTableColumnFPO(FPO) LIKE '%" + FPOs + "%'");
                if (LPOs != "")
                    s.Append(" and dbo.FN_MergeTableColumnLPO(LPO) LIKE '%" + LPOs + "%'");
                if (Supplier != "")
                    s.Append(" and dbo.FN_GetSupplierNm(Sup) LIKE '%" + Supplier + "%'");
                if (RqstNo != "")
                    s.Append(" and ReqNo LIKE '%" + RqstNo + "%'");
                if (Status != "")
                    s.Append(" and Stat LIKE '%" + Status + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE CustomerID LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FPOs LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LPOs LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SupplierID LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR RefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CreatedDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", PInvReqID ");
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
                            declare @MAA TABLE(CustomerID nvarchar(400), FPOs nvarchar(400), LPOs nvarchar(MAX), SupplierID nvarchar(MAX), 
                            RefNo nvarchar(MAX), CreatedDate datetime, Status nvarchar(MAX), PInvReqID nvarchar(MAX),Edit nvarchar(MAX), Delt nvarchar(MAX),Ct1 nvarchar(MAX))
                            INSERT
                            INTO
	                            @MAA ( CustomerID, FPOs, LPOs, SupplierID, RefNo, CreatedDate, Status, PInvReqID,Edit, Delt,Ct1)
	                               Select CustomerID, FPOs, LPOs, SupplierID, RefNo, CreatedDate, Status, PInvReqID,Edit, Delt,Ct1 from View_PISItems PR
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].PInvReqID)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].PInvReqID) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].CustomerID
			                               ,[@MAA].FPOs 
                                           ,[@MAA].LPOs
                                           ,[@MAA].SupplierID
                                           ,[@MAA].RefNo
                                           ,[@MAA].CreatedDate
                                           ,[@MAA].Status
                                           ,[@MAA].PInvReqID
                                           ,[@MAA].Edit
                                           ,[@MAA].Delt ,[@MAA].Ct1 
		                              FROM
			                              @MAA {1}) RawResults) Results

                WHERE
                RowNumber BETWEEN {2} AND {3} order by CreatedDate DESC";


                string where = "PR.IsActive <> 0 order by CreatedDate DESC ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                     " where CompanyId = '" + Session["CompanyID"].ToString() + "' and PR.IsActive <> 0 and " + where : " where PR.IsActive <> 0 " + s.ToString() + " and CompanyId = '" + Session["CompanyID"].ToString() + "' and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["PInvReqID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    string Len = data["CustomerID"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""0"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string FPo = data["FPOs"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string LPo = data["LPOs"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", LPo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string SupId = data["SupplierID"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", SupId.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Ref = data["RefNo"].ToString().Replace("\"", "\\\"");
                    Ref = Ref.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", Ref.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data["CreatedDate"]);
                    sb.Append(",");
                    string Stat = data["Status"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
                    string Pinv = data["PInvReqID"].ToString().Replace("\"", "\\\"");
                    Pinv = Pinv.Replace("\t", "-");
                    Stat = Stat.Replace("\t", "-");
                    if (Stat == "In Progress...")
                    {
                        if (data["Ct1"].ToString() == "Ct-1 Completed")
                        {
                            sb.AppendFormat(@"""6"": ""{0}""", "Closed");
                            sb.Append(",");
                        }
                        else
                        {
                            sb.AppendFormat(@"""6"": ""<a href=../Logistics/IomForm.aspx?PIReqID={0}>{1}</a>""", Pinv, Stat);
                            sb.Append(",");
                        }
                        
                    }
                    else
                    {
                        sb.AppendFormat(@"""6"": ""{0}""", Stat);
                        sb.Append(",");
                    }
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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetCheckList()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                var sb = new StringBuilder();

                string date = HttpContext.Current.Request.Params["sSearch_0"];
                string InvoiceNo = HttpContext.Current.Request.Params["sSearch_1"];
                string ChkLstRefNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Cust = HttpContext.Current.Request.Params["sSearch_3"];
                string GDNNOs = HttpContext.Current.Request.Params["sSearch_4"];
                string GRNNOs = HttpContext.Current.Request.Params["sSearch_5"];
                string SHpMntMode = HttpContext.Current.Request.Params["sSearch_6"];
                string Status = HttpContext.Current.Request.Params["sSearch_7"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and cast(c.CreatedDate as date) between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (InvoiceNo != "")
                    s.Append(" and c.InvoiceNo LIKE '%" + InvoiceNo + "%'");
                if (ChkLstRefNo != "")
                    s.Append(" and c.ChkLstRefNo LIKE '%" + ChkLstRefNo + "%'");
                if (Cust != "")
                    s.Append(" and c.CustomerNm LIKE '%" + Cust + "%'");
                if (GDNNOs != "")
                    s.Append(" and c.GDNnos LIKE '%" + GDNNOs + "%'");
                if (GRNNOs != "")
                    s.Append(" and c.GRNnos LIKE '%" + GRNNOs + "%'");
                if (SHpMntMode != "")
                    s.Append(" and c.ShipMntMd LIKE '%" + SHpMntMode + "%'");
                if (Status != "")
                    s.Append(" and c.Status LIKE '%" + Status + "%'");


                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE InvoiceNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ChkLstRefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerNm LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR GDNnos LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR GRNnos LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ShipMntMd LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                sb.Append(" ");

                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                orderByClause = "0 DESC";  //sb.ToString();

                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ChkListID ");
                    //orderByClause = orderByClause.Replace("1", ", EnquireNumber ");
                    //orderByClause = orderByClause.Replace("2", ", ForeignEnquireId ");
                    //orderByClause = orderByClause.Replace("3", ", Subject ");
                    //orderByClause = orderByClause.Replace("4", ", LEIssueDate ");
                    //orderByClause = orderByClause.Replace("5", ", StatusTypeId ");
                    //orderByClause = orderByClause.Replace("6", ", SupplierIds ");
                    ////orderByClause = orderByClause.Replace("7", ", CusmorId ");
                    //orderByClause = orderByClause.Replace("7", ", Mail ");
                    //orderByClause = orderByClause.Replace("8", ", Edit ");
                    //orderByClause = orderByClause.Replace("9", ", Delete ");
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

                string query = @"declare @MAA TABLE(ChkListID uniqueidentifier, CreatedDate datetime,InvoiceNo nvarchar(MAX),ChkLstRefNo nvarchar(MAX),CustomerNm nvarchar(MAX),
                                 GDNnos nvarchar(max),GRNnos nvarchar(max), ShipMntMd  nvarchar(MAX),Status  nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX))
                            INSERT INTO
	                            @MAA (ChkListID,CreatedDate ,InvoiceNo,ChkLstRefNo,CustomerNm,GDNnos,GRNnos, ShipMntMd,Status, EDIT, Delt)
	                               Select ChkListID,CreatedDate ,InvoiceNo,ChkLstRefNo,CustomerNm,GDNnos,GRNnos, ShipMntMd,Status, EDIT, Delt from View_ChkList c
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0} ) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].CreatedDate)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].ChkListID) FROM @MAA {1}) AS TotalDisplayRows
                                           ,[@MAA].ChkListID                                           
                                           ,[@MAA].CreatedDate			   
			                               ,[@MAA].InvoiceNo
			                               ,[@MAA].ChkLstRefNo      
                                           ,[@MAA].CustomerNm
                                           ,[@MAA].GDNnos
                                           ,[@MAA].GRNnos
                                           ,[@MAA].ShipMntMd
                                           ,[@MAA].Status
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                string cmpny = "";
                if (Session["CompanyID"].ToString() != "0")
                    cmpny = " c.CompanyId = '" + Session["CompanyID"].ToString() + "' ";
                string where = " " + cmpny + " and c.IsActive <> 0 order by CreatedDate DESC ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " where c.IsActive <> 0 and " + where : " where c.IsActive <> 0 " + s.ToString() + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ChkListID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["CreatedDate"]);
                    sb.Append(",");

                    string InvoiceNoo = data["InvoiceNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing
                    string ChkLstNo = data["ChkLstRefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing
                    string ChkLstId = data["ChkListID"].ToString().Replace("\"", "\\\"");

                    InvoiceNo = InvoiceNo.Replace("\t", "-");
                    if (InvoiceNoo != "")
                        sb.AppendFormat(@"""1"": ""<a href=CheckListDetails.aspx?ID={0}>{1}</a>""", ChkLstId, InvoiceNoo.Replace(Environment.NewLine, "\\n")); // New Line
                    else
                        sb.AppendFormat(@"""1"": ""{0}""", InvoiceNoo.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    ChkLstNo = ChkLstNo.Replace("\t", "-");
                    if (InvoiceNo == "")
                        sb.AppendFormat(@"""2"": ""<a href=CheckListDetails.aspx?ID={0}>{1}</a>""", ChkLstId, ChkLstNo.Replace(Environment.NewLine, "\\n")); // New Line
                    else
                        sb.AppendFormat(@"""2"": ""{0}""", ChkLstNo.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    string CustNm = data["CustomerNm"].ToString().Replace("\"", "\\\"");
                    CustNm = CustNm.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CustNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string GDNNOss = data["GDNnos"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    GDNNOss = GDNNOss.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", GDNNOss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string GRNNOss = data["GRNnos"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    GRNNOss = GRNNOss.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", GRNNOss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ShpmntMode = data["ShipMntMd"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    ShpmntMode = ShpmntMode.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ShpmntMode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Stat = data["Status"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Stat = Stat.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Stat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "CheckList Status WebService", ex.Message.ToString());
                return "";
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetCheckList_BIVAC()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                var sb = new StringBuilder();

                string date = HttpContext.Current.Request.Params["sSearch_0"];
                string InvoiceNo = HttpContext.Current.Request.Params["sSearch_1"];
                string ChkLstRefNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Cust = HttpContext.Current.Request.Params["sSearch_3"];
                string FPOnos = HttpContext.Current.Request.Params["sSearch_4"];
                string SHpMntMode = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and cast(c.CreatedDate as date) between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (InvoiceNo != "")
                    s.Append(" and c.InvoiceNo LIKE '%" + InvoiceNo + "%'");
                if (ChkLstRefNo != "")
                    s.Append(" and c.ChkLstRefNo LIKE '%" + ChkLstRefNo + "%'");
                if (Cust != "")
                    s.Append(" and c.CustomerNm LIKE '%" + Cust + "%'");
                if (FPOnos != "")
                    s.Append(" and c.FPOnos LIKE '%" + FPOnos + "%'");
                if (SHpMntMode != "")
                    s.Append(" and c.ShipMntMd LIKE '%" + SHpMntMode + "%'");
                if (Status != "")
                    s.Append(" and c.Status LIKE '%" + Status + "%'");


                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE InvoiceNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ChkLstRefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerNm LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FPOnos LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ShipMntMd LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                sb.Append(" ");

                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                orderByClause = "0 DESC";  //sb.ToString();

                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ChkListID ");
                    //orderByClause = orderByClause.Replace("1", ", EnquireNumber ");
                    //orderByClause = orderByClause.Replace("2", ", ForeignEnquireId ");
                    //orderByClause = orderByClause.Replace("3", ", Subject ");
                    //orderByClause = orderByClause.Replace("4", ", LEIssueDate ");
                    //orderByClause = orderByClause.Replace("5", ", StatusTypeId ");
                    //orderByClause = orderByClause.Replace("6", ", SupplierIds ");
                    ////orderByClause = orderByClause.Replace("7", ", CusmorId ");
                    //orderByClause = orderByClause.Replace("7", ", Mail ");
                    //orderByClause = orderByClause.Replace("8", ", Edit ");
                    //orderByClause = orderByClause.Replace("9", ", Delete ");
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

                string query = @"declare @MAA TABLE(ChkListID uniqueidentifier, CreatedDate datetime,InvoiceNo nvarchar(500),ChkLstRefNo nvarchar(100),CustomerNm nvarchar(max),
                                 FPOnos nvarchar(max), ShipMntMd  nvarchar(100),Status  nvarchar(500), EDIT nvarchar(MAX), Delt nvarchar(MAX))
                            INSERT INTO
	                            @MAA (ChkListID,CreatedDate ,InvoiceNo,ChkLstRefNo,CustomerNm,FPOnos, ShipMntMd,Status, EDIT, Delt)
	                                Select ChkListID,CreatedDate ,InvoiceNo,ChkLstRefNo,CustomerNm,FPOnos, ShipMntMd,Status, EDIT, Delt from View_ChkListBivac c
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0} ) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].CreatedDate)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].ChkListID) FROM @MAA {1}) AS TotalDisplayRows
                                           ,[@MAA].ChkListID                                           
                                           ,[@MAA].CreatedDate			   
			                               ,[@MAA].InvoiceNo
			                               ,[@MAA].ChkLstRefNo      
                                           ,[@MAA].CustomerNm
                                           ,[@MAA].FPOnos
                                           ,[@MAA].ShipMntMd
                                           ,[@MAA].Status
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt 
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                string cmpny = "";
                if (Session["CompanyID"].ToString() != "0")
                    cmpny = " c.CompanyId =  '" + Session["CompanyID"].ToString() + "'";
                string where = " " + cmpny + " and c.IsActive <> 0 order by CreatedDate DESC ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " where c.IsActive <> 0 and " + where : " where c.IsActive <> 0 " + s.ToString() + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ChkListID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["CreatedDate"]);
                    sb.Append(",");

                    string InvoiceNoo = data["InvoiceNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing
                    string ChkLstNo = data["ChkLstRefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing
                    string ChkLstId = data["ChkListID"].ToString().Replace("\"", "\\\"");

                    InvoiceNo = InvoiceNo.Replace("\t", "-");
                    if (InvoiceNoo != "")
                        sb.AppendFormat(@"""1"": ""<a href=../Logistics/CheckListDetails.aspx?ID={0}>{1}</a>""", ChkLstId, InvoiceNoo.Replace(Environment.NewLine, "\\n")); // New Line
                    else
                        sb.AppendFormat(@"""1"": ""{0}""", InvoiceNoo.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    ChkLstNo = ChkLstNo.Replace("\t", "-");
                    if (InvoiceNo == "")
                        sb.AppendFormat(@"""2"": ""<a href=../Logistics/CheckListDetails.aspx?ID={0}>{1}</a>""", ChkLstId, ChkLstNo.Replace(Environment.NewLine, "\\n")); // New Line
                    else
                        sb.AppendFormat(@"""2"": ""{0}""", ChkLstNo.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    string CustNm = data["CustomerNm"].ToString().Replace("\"", "\\\"");
                    CustNm = CustNm.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CustNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPOnoss = data["FPOnos"].ToString().Replace("\"", "\\\"");
                    FPOnoss = FPOnoss.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", FPOnoss.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ShpmntMode = data["ShipMntMd"].ToString().Replace("\"", "\\\"");
                    ShpmntMode = ShpmntMode.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", ShpmntMode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Stat = data["Status"].ToString().Replace("\"", "\\\"");
                    Stat = Stat.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Stat.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "CheckList Status WebService", ex.Message.ToString());
                return "";
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetCt_1TaskStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                var sb = new StringBuilder();

                //string date = HttpContext.Current.Request.Params["sSearch_0"];
                string CT_1RefNo = HttpContext.Current.Request.Params["sSearch_0"];
                string FPONos = HttpContext.Current.Request.Params["sSearch_1"];
                string LPONos = HttpContext.Current.Request.Params["sSearch_2"];
                string CustNm = HttpContext.Current.Request.Params["sSearch_3"];
                //string GRNNOs = HttpContext.Current.Request.Params["sSearch_5"];
                string SuppNm = HttpContext.Current.Request.Params["sSearch_4"];
                //string Status = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (CT_1RefNo != "")
                    s.Append(" and ct.CTOneRefNo LIKE '%" + CT_1RefNo + "%'");
                if (FPONos != "")
                    s.Append(" and ct.FPONos LIKE '%" + FPONos + "%'");
                if (LPONos != "")
                    s.Append(" and ct.LPONos LIKE '%" + LPONos + "%'");
                if (CustNm != "")
                    s.Append(" and ct.CustNm LIKE '%" + CustNm + "%'");
                if (SuppNm != "")
                    s.Append(" and ct.SupNm LIKE '%" + SuppNm + "%'");


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

                orderByClause = "0 DESC";  //sb.ToString();

                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", CTpID ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "CTpID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"declare @MAA TABLE(CreatedDate datetime,CTpID uniqueidentifier,CTOneRefNo nvarchar(max),FPONos nvarchar(max),LPONos nvarchar(max),
                                 CustNm nvarchar(max),SupNm nvarchar(max),EDIT nvarchar(MAX), Delt nvarchar(MAX))
                            INSERT INTO
	                            @MAA (CreatedDate,CTpID,CTOneRefNo ,FPONos,LPONos,CustNm,SupNm,EDIT, Delt)
	                                Select CreatedDate,CTpID,CTOneRefNo ,FPONos,LPONos,CustNm,SupNm,EDIT, Delt from View_CT1TaskStatus ct
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0} ) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].CTpID)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].CTpID) FROM @MAA {1}) AS TotalDisplayRows
                                           ,[@MAA].CreatedDate ,[@MAA].CTpID                                          
                                           ,[@MAA].CTOneRefNo			   
			                               ,[@MAA].FPONos
			                               ,[@MAA].LPONos      
                                           ,[@MAA].CustNm
                                           ,[@MAA].SupNm
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt 
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";


                string where = " ct.IsActive <> 0 ";
                string where1 = "";
                if (new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                {
                    where1 = "and CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
                }

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " where ct.IsActive <> 0" + where1 + " " : " where ct.IsActive <> 0 " + s.ToString() + where1 + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CTpID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string CTOneRefNo = data["CTOneRefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing
                    string CTpID = data["CTpID"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""0"": ""<a href='javascript:return void(0)' id='myLink' class='abcd' >{1}</a>""", CTpID, CTOneRefNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPONums = data["FPONos"].ToString().Replace("\"", "\\\"");
                    FPONums = FPONums.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", FPONums.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string LPONm = data["LPONos"].ToString().Replace("\"", "\\\"");
                    LPONm = LPONm.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", LPONm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustNme = data["CustNm"].ToString().Replace("\"", "\\\"");
                    CustNme = CustNme.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CustNme.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string SupNm = data["SupNm"].ToString().Replace("\"", "\\\"");
                    SupNm = SupNm.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", SupNm.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "CheckList Status WebService", ex.Message.ToString());
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
