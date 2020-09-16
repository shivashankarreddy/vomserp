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
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace VOMS_ERP.Quotations
{
    /// <summary>
    /// Summary description for Quotations_WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Quotations_WebService : System.Web.Services.WebService
    {
        /// <summary>
        /// FOR LOCAL Quotation
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFQItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string FqNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string REfFENO = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string Cust = HttpContext.Current.Request.Params["sSearch_4"];
                string Status = HttpContext.Current.Request.Params["sSearch_5"];
                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and QuotationDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FqNo != "")
                    s.Append(" and Quotationnumber LIKE '%" + FqNo + "%'");
                if (REfFENO != "")
                    s.Append(" and dbo.FN_MergeTableColumn_FE(FrnEnqIDs) LIKE '%" + REfFENO + "%'");
                if (Subject != "")
                    s.Append(" and f.Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusTypeId) LIKE '%" + Status + "%'");
                if (Cust != "")
                    s.Append(" and  dbo.GetActiveCustomerName(f.CusmorId) LIKE '%" + Cust + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE QuotationDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Quotationnumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FrnEnqNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustmrNm LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;
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

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"  
                            declare @MAA TABLE(CreatedDate datetime,QuotationDate datetime,ForeignQuotationId uniqueidentifier,Quotationnumber nvarchar(MAX),FrnEnqNmbr nvarchar(MAX),
                                                Subject nvarchar(MAX),Status nvarchar(MAX),CustmrNm nvarchar(MAX),StatusTypeId bigint,Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX))
                            INSERT
                            INTO
	                            @MAA (CreatedDate,QuotationDate,ForeignQuotationId,Quotationnumber,FrnEnqNmbr,Subject,Status,CustmrNm,StatusTypeId,Mail, Edit, Delt)
	                                Select distinct f.CreatedDate,QuotationDate,ForeignQuotationId,Quotationnumber,dbo.FN_MergeTableColumn_FE(FrnEnqIDs) FrnEnqNmbr, f.Subject, dbo.FN_GetStatus(f.StatusTypeId) Status, 
                                    dbo.GetActiveCustomerName(f.CusmorId) CustmrNm,f.StatusTypeId,  
                                    '<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL, 
                                    '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
                                        + CONVERT(nvarchar(max), f.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +','+ convert(varchar(10),fe.StatusTypeId)+') />' EDIT, 
                                    '<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
                                        + CONVERT(nvarchar(max), f.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delt 
                                     FROM [FQuotation] f inner join Users u on u.ID = f.CreatedBy 
                                     inner join FEnquiry fe on fe.ForeignEnquireId in (select * from dbo.SplitString(f.FrnEnqIDs,','))
	                                {4}

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].ForeignQuotationId)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].ForeignQuotationId) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].CreatedDate
                                           ,[@MAA].QuotationDate
			                               ,[@MAA].Quotationnumber      
                                           ,[@MAA].FrnEnqNmbr
                                           ,[@MAA].Subject
                                           ,[@MAA].Status
                                           ,[@MAA].CustmrNm
                                           ,[@MAA].ForeignQuotationId
                                           ,[@MAA].StatusTypeId
                                           ,[@MAA].MAIL
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                string where = "f.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "' ";

                if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        if (Session["IsUser"] == null && Session["IsUser"].ToString() == "0")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.IsActive <> 0 " :
                               " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.IsActive <> 0" + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where  CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' and " + where :
                                " where  CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' f.IsActive <> 0" + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "opd")
                    {
                        if (((ArrayList)Session["UserDtls"])[7].ToString() == CommonBLL.SuperAdminRole)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where f.IsActive <> 0 and " + where : " where f.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' and  f.IsActive <> 0 " :
                                " where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' and f.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                    }
                }
                else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        " WHERE  f.CusmorId in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "') and f.IsActive <> 0 "
                        : "WHERE" + (s.ToString() == "" ? " AND f.IsActive <> 0 AND " : " f.IsActive <> 0 " + s.ToString() + "AND") + " f.CusmorId in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "')");
                }
                else
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                        s.ToString() == "" ? " where f.IsActive <> 0 and " + where : " where f.IsActive <> 0 " + s.ToString() + " and " + where);
                }
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignQuotationId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string FQNo = data["Quotationnumber"].ToString().Replace("\"", "\\\"");
                    string FQId = data["ForeignQuotationId"].ToString().Replace("\"", "\\\"");
                    FQNo = FQNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=FQDetails.aspx?ID={0}>{1}</a>""", FQId, FQNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["QuotationDate"]);
                    sb.Append(",");

                    string RecDt = data["FrnEnqNmbr"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["CustmrNm"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "FQ Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR LOCAL Quotation
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetLQItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string LqNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string RLeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string RFeNo = HttpContext.Current.Request.Params["sSearch_3"];
                string Subject = HttpContext.Current.Request.Params["sSearch_4"];
                string Suplr = HttpContext.Current.Request.Params["sSearch_5"];
                string CustomerNm = HttpContext.Current.Request.Params["sSearch_6"];
                string Status = HttpContext.Current.Request.Params["sSearch_7"];

                var sb = new StringBuilder();
                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and QuotationDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (LqNo != "")
                    s.Append(" and Quotationnumber LIKE '%" + LqNo + "%'");
                if (RLeNo != "")
                    s.Append(" and dbo.FN_LEnquiryNumber(LocalEnquireId) LIKE '%" + RLeNo + "%'");
                if (RFeNo != "")
                    s.Append(" and dbo.FN_FEnquiryNumber(ForeignEnquiryId) LIKE '%" + RFeNo + "%'");
                if (Subject != "")
                    s.Append(" and LQuotation.Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusTypeId) LIKE '%" + Status + "%'");
                if (Suplr != "")
                    s.Append(" and dbo.FN_GetSupplierNm(SupplierId) LIKE '%" + Suplr + "%'");
                if (CustomerNm != "")
                    s.Append(" and dbo.GetActiveCustomerName(CustomerId) LIKE '%" + CustomerNm + "%'");


                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE QuotationDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Quotationnumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LocalEnquireId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignEnquiryId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SupplierId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", LocalQuotationId ");
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
                            declare @MAA TABLE(CreatedDate datetime,QuotationDate datetime, Quotationnumber nvarchar(400), LocalQuotationId Uniqueidentifier, LocalEnquireId nvarchar(MAX), ForeignEnquiryId nvarchar(MAX), 
                            Subject nvarchar(MAX), StatusTypeId nvarchar(MAX), SupplierId nvarchar(MAX), CustomerName nvarchar(MAX), Attachments nvarchar(MAX), Mail nvarchar(MAX), 
                            Edit nvarchar(MAX), Delt nvarchar(MAX))
                            INSERT  INTO
	                            @MAA ( CreatedDate,QuotationDate, Quotationnumber, LocalQuotationId, LocalEnquireId, ForeignEnquiryId, Subject, StatusTypeId, SupplierId, CustomerName, 
                                    Attachments, Mail, Edit, Delt)
	                                Select LQuotation.CreatedDate,QuotationDate, Quotationnumber, LocalQuotationId, dbo.FN_LEnquiryNumber(LocalEnquireId) LocalEnquireId, 
                                    dbo.FN_FEnquiryNumber(ForeignEnquiryId) ForeignEnquiryId, LQuotation.Subject, 
                                    (dbo.FN_GetStatus(f.StatusTypeId) + CASE WHEN (IsOffer = 1 and f.StatusTypeId = 30) THEN ' and Offered to Customer' ELSE '' END) StatusTypeId,
                                    dbo.FN_GetSupplierNm(SupplierId) SupplierId, dbo.GetActiveCustomerName(CustomerId) CustomerName,
                                    LQuotation.Attachments,
                                    '<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL, 
                                    '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this) />' EDIT, 
                                    '<img src=../images/delete.png alt=Delete onclick=Delet(this) />' Delt
                                     FROM LQuotation inner join FEnquiry f on f.ForeignEnquireId = LQuotation.ForeignEnquiryId
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].LocalQuotationId)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].LocalQuotationId) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].CreatedDate
                                           ,[@MAA].QuotationDate
			                               ,[@MAA].Quotationnumber 
                                           ,[@MAA].LocalQuotationId
                                           ,[@MAA].LocalEnquireId
                                           ,[@MAA].ForeignEnquiryId
                                           ,[@MAA].Subject
                                           ,[@MAA].StatusTypeId
                                           ,[@MAA].SupplierId
                                           ,[@MAA].CustomerName
                                           ,[@MAA].Attachments 
                                           ,[@MAA].Mail
                                           ,[@MAA].Edit
                                           ,[@MAA].Delt 
		                              FROM
			                              @MAA {1}) RawResults) Results 

                WHERE
                RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";

                Guid CompanyId = new Guid(Session["CompanyID"].ToString());
                string where = " LQuotation.IsActive <> 0 and LQuotation.CompanyId = " + "'" + CompanyId + "'" + " order by LQuotation.CreatedDate DESC ";
                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        //if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where  CONVERT(nvarchar(12), LQuotation.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and LQuotation.IsActive <> 0 and LQuotation.CompanyId = " + "'" + CompanyId + "'" :
                                 "where  CONVERT(nvarchar(12), LQuotation.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and LQuotation.IsActive <> 0" + s.ToString() + " and LQuotation.CompanyId = " + "'" + CompanyId + "'" + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where  CONVERT(nvarchar(12), LQuotation.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and LQuotation.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and LQuotation.CompanyId = " + "'" + CompanyId + "'" :
                                 "where  CONVERT(nvarchar(12), LQuotation.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and LQuotation.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" +
                                 " and LQuotation.CompanyId = " + "'" + CompanyId + "'" + s.ToString());
                        }
                    }
                    else if (Mode == "tldt")
                    {
                        //if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where LQuotation.IsActive <> 0 and " + where : " LQuotation.IsActive <> 0 " + s.ToString() + " and " + where );
                        }
                        else if (CommonBLL.CustmrContactTypeText ==((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where LQuotation.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and LQuotation.IsActive <> 0 and " + where :
                                "where LQuotation.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and LQuotation.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else
                    {
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            "where LQuotation.IsActive <> 0 and " + where : " where LQuotation.IsActive <> 0 " + s.ToString() + " and " + where);
                    }
                }
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["LocalQuotationId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Quono = data["Quotationnumber"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string LQid = data["LocalQuotationId"].ToString().Replace("\"", "\\\"");
                    Quono = Quono.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=LQDetails.Aspx?ID={0}>{1}</a>""", LQid, Quono.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["QuotationDate"]);
                    sb.Append(",");

                    string Len = data["LocalEnquireId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Fen = data["ForeignEnquiryId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Fen.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string DeptID = data["SupplierId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustomerName = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", CustomerName.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""7"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""8"": ""{0}""", Att_open(data["Attachments"].ToString()));
                    sb.Append(",");

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "LQ Status WebService", ex.Message.ToString());
                return "";
            }
        }


        public string Att_open(string Attachments)
        {
            StringBuilder sbb = new StringBuilder();
            try
            {
                string url = "../uploads/";
                ArrayList al = new ArrayList();
                al.AddRange(Attachments.Trim().Split(','));
                for (int i = 0; i < al.Count; i++)
                {
                    string finUrl = url + "" + al[i].ToString();
                    string fileName = al[i].ToString();
                    int fileExtPos = fileName.LastIndexOf(".");
                    if (fileExtPos >= 0)
                        fileName = fileName.Substring(0, fileExtPos);

                    sbb.Append("<a href='" + finUrl + "' id='openfile" + i + "' onclick='saveToDisk("
                        + finUrl + "," + al[i].ToString() + ");' target='_blank'>" + fileName + "</a><br/>");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            return sbb.ToString(); ;
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }
    }
}
