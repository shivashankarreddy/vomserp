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

namespace VOMS_ERP.Purchases
{
    /// <summary>
    /// Summary description for Purchases_WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Purchases_WebService : System.Web.Services.WebService
    {
        /// <summary>
        /// FOR FPO
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFPOItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FPOrderNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string FpValue = HttpContext.Current.Request.Params["sSearch_4"];
                string Cust = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];

                string query = "";
                string query1 = "";

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and FPODate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FPOrderNo != "")
                    s.Append(" and ForeignPurchaseOrderNo LIKE '%" + FPOrderNo.Replace("'", "''") + "%'");
                if (FeNo != "")
                    s.Append(" and ForeignEnquiryId LIKE '%" + FeNo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and StatusTypeId LIKE '%" + Status.Replace("'", "''") + "%'");
                if (FpValue != "")
                {
                    FpValue = FpValue.Replace(",", "");
                    s.Append(" and OrderValue LIKE '%" + FpValue.Replace("'", "''") + "%'");
                }
                if (Cust != "")
                    s.Append(" and CusmorId LIKE '%" + Cust.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignEnquiryId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CusmorId LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", FPODate ");
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

                if (FPOrderNo != "" || (date != null && date != "") || FeNo != "" || Subject != "" || FpValue != "" || Cust != "" || Status != "")
                {
                    query = @"  
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), ForeignPurchaseOrderId uniqueidentifier, 
							Subject nvarchar(MAX), OrderValue nvarchar(20), StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX),Cancel nvarchar(MAX),
                            Mail nvarchar(MAX), AMENDEDIT nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX),VendorId nvarchar(max),History nvarchar(max),Isverbal bit);

							INSERT INTO
								@MAA (CreatedDate,ForeignPurchaseOrderNo,  FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, StatusTypeId, 
								CusmorId,Cancel, Mail, AMENDEDIT,Edit, Delt, VendorId,History,Isverbal) 
								Select CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, 
								StatusTypeId, CusmorId,Cancel, MAIL, AMENDEDIT,EDIT, Delt, Vendor,History,IsVerbalFPO from View_GetFPOItems b
                                  {4};

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
										   ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
										   ,[@MAA].Cancel,[@MAA].Mail
										   ,[@MAA].AMENDEDIT
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].VendorId 
										   ,[@MAA].History,[@MAA].Isverbal
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                    query1 = @"  
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), ForeignPurchaseOrderId uniqueidentifier, 
							Subject nvarchar(MAX), OrderValue nvarchar(20), StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX),Cancel nvarchar(MAX),
                            Mail nvarchar(MAX), AMENDEDIT nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX),VendorId nvarchar(max),History nvarchar(max),Isverbal bit);

							INSERT INTO
								@MAA (CreatedDate,ForeignPurchaseOrderNo,  FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, StatusTypeId, 
								CusmorId,Cancel, Mail, AMENDEDIT,Edit, Delt, VendorId,History,Isverbal) 
								Select CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, 
								StatusTypeId, CusmorId,Cancel, '', AMENDEDIT,EDIT, Delt, Vendor,History,IsVerbalFPO from View_GetFPOItems b
                                  {4};

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
										   ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
										   ,[@MAA].Cancel,[@MAA].Mail
										   ,[@MAA].AMENDEDIT
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].VendorId 
										   ,[@MAA].History,[@MAA].Isverbal
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                }
                else
                {
                    query = @"  
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), ForeignPurchaseOrderId uniqueidentifier, 
							Subject nvarchar(MAX), OrderValue nvarchar(20), StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX),Cancel nvarchar(MAX),
                            Mail nvarchar(MAX), AMENDEDIT nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX),VendorId nvarchar(max),History nvarchar(max),Isverbal bit);

							INSERT INTO
								@MAA (CreatedDate,ForeignPurchaseOrderNo,  FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, StatusTypeId, 
								CusmorId,Cancel, Mail, AMENDEDIT,Edit, Delt, VendorId,History,Isverbal) 
								Select top 500 CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, 
								StatusTypeId, CusmorId,Cancel, MAIL, AMENDEDIT,EDIT, Delt, Vendor,History,IsVerbalFPO from View_GetFPOItems b
                                  {4} order by CreatedDate Desc;

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
										   ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
										   ,[@MAA].Cancel,[@MAA].Mail
										   ,[@MAA].AMENDEDIT
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].VendorId 
										   ,[@MAA].History,[@MAA].Isverbal
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                    query1 = @"  
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), ForeignPurchaseOrderId uniqueidentifier, 
							Subject nvarchar(MAX), OrderValue nvarchar(20), StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX),Cancel nvarchar(MAX),
                            Mail nvarchar(MAX), AMENDEDIT nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX),VendorId nvarchar(max),History nvarchar(max),Isverbal bit);

							INSERT INTO
								@MAA (CreatedDate,ForeignPurchaseOrderNo,  FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, StatusTypeId, 
								CusmorId,Cancel, Mail, AMENDEDIT,Edit, Delt, VendorId,History,Isverbal) 
								Select top 500 CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, OrderValue, 
								StatusTypeId, CusmorId,Cancel, '', AMENDEDIT,EDIT, Delt, Vendor,History,IsVerbalFPO from View_GetFPOItems b
                                  {4} order by CreatedDate Desc;

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
										   ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
										   ,[@MAA].Cancel,[@MAA].Mail
										   ,[@MAA].AMENDEDIT
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].VendorId 
										   ,[@MAA].History,[@MAA].Isverbal
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                }
                //(isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000') "
                //,'''+ REPLACE(REPLACE(dbo.FN_GetStatus(StatusTypeId),' ',''),'''','') +'''
                string where = " b.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'" + " or b.VendorId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
                if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and  IsActive <> 0 "
                                + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where :
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and  IsActive <> 0 " + s.ToString()
                                + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString()
                                + "' and IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where :
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' and IsActive <> 0 " + s.ToString()
                                + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                        }
                    }
                    else if (Mode == "tldt")
                    {
                        if (((ArrayList)(Session["UserDtls"]))[7].ToString() == CommonBLL.SuperAdminRole)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            " where IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and "
                            + where : " where IsActive <> 0 " + s.ToString() + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                        }
                        else if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            " where IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and "
                            + where : " where IsActive <> 0 " + s.ToString() + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString()
                                + "' and IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where :
                                " where CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' and IsActive <> 0 " + s.ToString()
                                + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                        }
                    }
                    //else
                    //{
                    //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    //       " where IsActive <> 0 and " + where : " where IsActive <> 0 " + s.ToString() + " and " + where);
                    //} 
                }
                else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                {
                    query = String.Format(query1, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        " WHERE  b.CustId in (Select Data from dbo.SplitString('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and" + where
                        : "WHERE b.CustId in (Select Data from dbo.SplitString('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) " + s.ToString()
                        + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                }
                else
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                        s.ToString() == "" ? " where IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000' and "
                        + where + ")" : " where IsActive <> 0 " + s.ToString()
                        + " and (isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000' and " + where + ")");
                }
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Fpno = data["ForeignPurchaseOrderNo"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string FPid = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    Fpno = Fpno.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=FPODetails.Aspx?ID={0}>{1}</a>""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["FPODate"]);
                    sb.Append(",");

                    string Len = data["ForeignEnquiryId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""4"": ""{0}""", Convert.ToDecimal(data["OrderValue"].ToString()).ToString("N"));
                    sb.Append(",");

                    string CusId = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", CusId.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Histroy = data["History"].ToString().Replace("\"", "\\\"");
                    Histroy = Histroy.Replace("\t", "-");
                    if (Histroy != "")
                    {
                        sb.AppendFormat(@"""7"": ""<a href=FPOAmendmentsStatus.aspx?ID={0}>{1}</a>""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""7"": """"");
                        sb.Append(",");
                    }
                    string Cancel = data["Cancel"].ToString().Replace("\"", "\\\"");
                    Cancel = Cancel.Replace("\t", "-");
                    //if (data["Isverbal"].ToString().Replace("\"", "\\\"") != "True")
                    //{
                    sb.AppendFormat(@"""8"": ""{0}""", Cancel.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""8"": ""{0}""", "");
                    //    sb.Append(",");
                    //}

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string AMENDEDIT = data["AMENDEDIT"].ToString().Replace("\"", "\\\"");
                    AMENDEDIT = AMENDEDIT.Replace("\t", "-");
                    if (data["Isverbal"].ToString().Replace("\"", "\\\"") != "True")
                    {
                        sb.AppendFormat(@"""10"": ""{0}""", AMENDEDIT.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""10"": ""{0}""", "");
                        sb.Append(",");
                    }



                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR FPO
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFPOAmendmentsItems()
        {
            try
            {
                string FPOID = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["ID"];
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FPOrderNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string FpValue = HttpContext.Current.Request.Params["sSearch_4"];
                string Cust = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and FPODate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FPOrderNo != "")
                    s.Append(" and ForeignPurchaseOrderNo LIKE '%" + FPOrderNo.Replace("'", "''") + "%'");
                if (FeNo != "")
                    s.Append(" and ForeignEnquiryId LIKE '%" + FeNo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and StatusTypeId LIKE '%" + Status.Replace("'", "''") + "%'");
                if (FpValue != "")
                {
                    FpValue = FpValue.Replace(",", "");
                    s.Append(" and dbo.FN_GetOrderValue(b.ForeignPurchaseOrderId) LIKE '%" + FpValue.Replace("'", "''") + "%'");
                }
                if (Cust != "")
                    s.Append(" and CusmorId LIKE '%" + Cust.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignEnquiryId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CusmorId LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", FPODate ");
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
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), ForeignPurchaseOrderId uniqueidentifier, 
							Subject nvarchar(MAX),OrderValue nvarchar(20),StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX),Isverbal bit,FPO_AmndmntId uniqueidentifier);

							INSERT INTO
								@MAA (CreatedDate ,FPODate , ForeignPurchaseOrderNo , ForeignEnquiryId , ForeignPurchaseOrderId , 
							Subject ,OrderValue,StatusTypeId , CusmorId ,Isverbal,FPO_AmndmntId )
                               Select distinct b.CreatedDate,b.FPODate,b.ForeignPurchaseOrderNo, dbo.FN_FEnquiryNumber(ForeignEnquiryId), b.ForeignPurchaseOrderId, b.Subject,
                                dbo.FN_GetOrderValue(b.FPO_AmndmntId) as OrderValue, 
                                dbo.FN_GetStatus(StatusTypeId), dbo.GetAllCustomerName(CusmorId),b.IsVerbalFPO,b.FPO_AmndmntId from FPO_Amndmnt b where b.IsActive<>0
                               {4}
                                group by b.CreatedDate,b.FPODate,b.ForeignPurchaseOrderNo, b.ForeignEnquiryId, b.ForeignPurchaseOrderId, b.Subject,--SUM(i.Quantity * i.Rate) , 
                                b.StatusTypeId, b.CusmorId,b.IsVerbalFPO,b.FPO_AmndmntId
                                 ;

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
                                           ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
										   ,[@MAA].Isverbal
                                           ,[@MAA].FPO_AmndmntId
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                string Where = "";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (FPOID == null || FPOID == "")
                {
                    FPOID = Guid.Empty.ToString();
                }
                //string where = "(b.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'" + " or b.VendorId = '" + new Guid(Session["CompanyID"].ToString()) + "')";
                Where = " and  b.ForeignPurchaseOrderId=" + "'" + new Guid(FPOID) + "'" + " and b.IsActive <> 0";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ? Where : s.ToString() + Where);
                //query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                //    s.ToString() == "" ? " where IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and "
                //    + where : " where IsActive <> 0 " + s.ToString()
                //    + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);

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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Fpno = data["ForeignPurchaseOrderNo"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string FPid = data["FPO_AmndmntId"].ToString().Replace("\"", "\\\"");
                    Fpno = Fpno.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=FPODetails_Amndmnt.aspx?ID={0}>{1}</a>""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["FPODate"]);
                    sb.Append(",");

                    string Len = data["ForeignEnquiryId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""4"": ""{0}""", Convert.ToDecimal(data["OrderValue"].ToString()).ToString("N"));
                    sb.Append(",");


                    string CusId = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", CusId.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR FPO Vendor
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetVenFPOItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FPOrderNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string Vendor = HttpContext.Current.Request.Params["sSearch_4"];
                string FpValue = HttpContext.Current.Request.Params["sSearch_5"];
                string Cust = HttpContext.Current.Request.Params["sSearch_6"];
                string Status = HttpContext.Current.Request.Params["sSearch_7"];

                StringBuilder s = new StringBuilder();
                if (date != "" && date.Length > 1)
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and FPODate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FPOrderNo != "")
                    s.Append(" and ForeignPurchaseOrderNo LIKE '%" + FPOrderNo.Replace("'", "''") + "%'");
                if (FeNo != "")
                    s.Append(" and ForeignEnquiryId LIKE '%" + FeNo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Vendor != "")
                    s.Append(" and dbo.FN_GetVendorBussName(Vendor) LIKE '%" + Vendor.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and StatusTypeId LIKE '%" + Status.Replace("'", "''") + "%'");
                if (FpValue != "")
                    s.Append(" and OrderValue LIKE '%" + FpValue.Replace("'", "''") + "%'");
                if (Cust != "")
                    s.Append(" and CusmorId LIKE '%" + Cust.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignEnquiryId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CusmorId LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", FPODate ");
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
							declare @MAA TABLE(CreatedDate datetime,FPODate datetime, ForeignPurchaseOrderNo nvarchar(400), ForeignEnquiryId nvarchar(MAX), 
                            ForeignPurchaseOrderId uniqueidentifier, Subject nvarchar(MAX), Vendor nvarchar(max),OrderValue nvarchar(20), 
                            StatusTypeId nvarchar(MAX), CusmorId nvarchar(MAX), IsVerbalFPO bit, AMENDEDIT nvarchar(1000), Mail nvarchar(MAX), 
                            Edit nvarchar(MAX), Delt nvarchar(MAX), IsFPOCancel bit, Cancel nvarchar(MAX), History nvarchar(1000));

							INSERT INTO
								@MAA (CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, Vendor, OrderValue, 
                                StatusTypeId, CusmorId, IsVerbalFPO, AMENDEDIT, Mail, Edit, Delt, IsFPOCancel, Cancel, History) 

								Select CreatedDate,ForeignPurchaseOrderNo, FPODate, ForeignEnquiryId, ForeignPurchaseOrderId, Subject, Vendor, OrderValue, 
								StatusTypeId, CusmorId, IsVerbalFPO,AMENDEDIT, MAIL, EDIT, Delt, IsFPOCancel, Cancel, History from View_GetVenFPOItems b {4};

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].FPODate
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].ForeignEnquiryId
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].Subject
										   ,[@MAA].Vendor 
										   ,[@MAA].OrderValue
										   ,[@MAA].StatusTypeId
										   ,[@MAA].CusmorId
                                           ,[@MAA].IsVerbalFPO
                                           ,[@MAA].AMENDEDIT
										   ,[@MAA].Mail
										   ,[@MAA].Edit
										   ,[@MAA].Delt 
                                           ,[@MAA].IsFPOCancel
                                           ,[@MAA].Cancel
                                           ,[@MAA].History
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                //,'''+ REPLACE(REPLACE(dbo.FN_GetStatus(StatusTypeId),' ',''),'''','') +'''
                string where = "  b.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        " WHERE  " + where //b.CustId in (Select Data from dbo.SplitString('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and (isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000') and
                        : " WHERE IsActive <> 0 " + s.ToString() + "  and " + where); // b.CustId in (Select Data from dbo.SplitString('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and (isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000')
                }
                else
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                        s.ToString() == "" ? " where IsActive <> 0 AND " + where : //(isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000') and 
                        " where IsActive <> 0 " + s.ToString() + " and " + where);//(isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000') 
                }
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Fpno = data["ForeignPurchaseOrderNo"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string FPid = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    Fpno = Fpno.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=FPODetails.Aspx?ID={0}&FPOVen=true>{1}</a>""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["FPODate"]);
                    sb.Append(",");

                    string Len = data["ForeignEnquiryId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Vend = data["Vendor"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", Vend.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""5"": ""{0}""", Convert.ToDecimal(data["OrderValue"].ToString()).ToString("N"));
                    sb.Append(",");

                    //string CusId = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""6"": ""{0}""", CusId.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    if (!Convert.ToBoolean(data["IsVerbalFPO"].ToString()) && !Convert.ToBoolean(data["IsFPOCancel"].ToString()))
                        sb.AppendFormat(@"""8"": ""{0}""", data["AMENDEDIT"].ToString());
                    else
                        sb.AppendFormat(@"""8"": ""{0}""", "");
                    sb.Append(",");

                    //string Histroy = data["History"].ToString().Replace("\"", "\\\"");
                    if (data["History"].ToString() != "")
                        sb.AppendFormat(@"""9"": ""<a href=FPOAmendmentsStatus.aspx?ID={0}>{1}</a>""", FPid, "Amndments");
                    else
                        sb.AppendFormat(@"""9"": """"");
                    sb.Append(",");

                    if (Convert.ToBoolean(data["IsFPOCancel"].ToString()))
                        sb.AppendFormat(@"""10"": ""{0}""", "");
                    else
                    {
                        string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                        Edit = Edit.Replace("\t", "-");
                        sb.AppendFormat(@"""10"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    }
                    sb.Append(",");

                    //// ************* Delete  *****************
                    //string Del = data["Cancel"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("\t", "-");
                    //sb.AppendFormat(@"""11"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    if (Convert.ToBoolean(data["IsFPOCancel"].ToString()))
                        sb.AppendFormat(@"""11"": ""{0}""", "<img src=../images/d1.png alt=Cancellation onclick=AlreadyCancelled();>");
                    else
                    {
                        string Del = data["Cancel"].ToString().Replace("\"", "\\\"");
                        Del = Del.Replace("\t", "-");
                        sb.AppendFormat(@"""11"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                    }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR LOCAL PURCHASE ORDER
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetLPOItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string LPNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string FPONo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string SuplierNme = HttpContext.Current.Request.Params["sSearch_4"];
                string Status = HttpContext.Current.Request.Params["sSearch_5"];

                string query = "";

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and LPOrderDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (LPNo != "")
                    s.Append(" and LocalPurchaseOrderNo LIKE '%" + LPNo.Replace("'", "''") + "%'");
                if (FPONo != "")
                    s.Append(" and dbo.FN_MergeTableColumnFPO(ForeignPurchaseOrderId) LIKE '%" + FPONo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and  Status LIKE '%" + Status.Replace("'", "''") + "%'");
                if (SuplierNme != "")
                    s.Append(" and SuplrNm LIKE '%" + SuplierNme.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE LPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LocalPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FPOrderNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SuplrNm LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", CreatedDate ");
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

                if (LPNo != "" || (date != null && date != "") || FPONo != "" || Subject != "" || SuplierNme != "" || Status != "")
                {
                    query = @"  
							declare @MAA TABLE(CreatedDate datetime,LPODate date,LocalPurchaseOrderId uniqueidentifier,FPOrderNmbr nvarchar(MAX),ForeignPurchaseOrderId nvarchar(MAX),
							LocalPurchaseOrderNo nvarchar(MAX), Subject nvarchar(MAX),SuplrNm nvarchar(MAX),IsDrawing varchar(5),DrawingID uniqueidentifier, 
							IsInspection varchar(5),CInspID uniqueidentifier,Status nvarchar(MAX),Histroy uniqueidentifier,Attachments nvarchar(MAX),Cancel nvarchar(MAX),Mail nvarchar(MAX),AmendEdit nvarchar(MAX),
							Edit nvarchar(MAX), Delt nvarchar(MAX),SupplierId uniqueidentifier,CusmorId uniqueidentifier,DrwngAprls bit,IsVerbalLPO bit)
							
                            INSERT INTO
								@MAA (CreatedDate,LPODate,LocalPurchaseOrderId,FPOrderNmbr,ForeignPurchaseOrderId,LocalPurchaseOrderNo,
									  Subject,SuplrNm,IsDrawing,DrawingID,IsInspection,CInspID,Status,Histroy,Attachments,Cancel,Mail,AmendEdit, Edit, Delt,SupplierId,CusmorId,
									  DrwngAprls,IsVerbalLPO)
									  Select distinct CreatedDate,LPOrderDate, LocalPurchaseOrderId, FPOrderNmbr, ForeignPurchaseOrderId, LocalPurchaseOrderNo, 
										Subject, SuplrNm, IsDrawing, DrawingID, IsInspection, CInsID, Status,  History,Attachments,Cancel, MAIL, AMENDEDIT, EDIT, 
									   Delt, SupplierId, CustomerId, DrwngAprls,IsVerbalLPO from View_GetLPOItems b  {4}                   
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM @MAA) AS TotalRows, 
											(SELECT count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].LPODate
										   ,[@MAA].LocalPurchaseOrderId      
										   ,[@MAA].FPOrderNmbr
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].LocalPurchaseOrderNo
										   ,[@MAA].Subject
										   ,[@MAA].SuplrNm
										   ,[@MAA].IsDrawing
										   ,[@MAA].DrawingID
										   ,[@MAA].IsInspection
										   ,[@MAA].CInspID
										   ,[@MAA].Status
										   ,[@MAA].Histroy
                                           ,[@MAA].Attachments
										   ,[@MAA].Cancel,[@MAA].MAIL
										   ,[@MAA].AmendEdit
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
										   ,[@MAA].SupplierId
										   ,[@MAA].CusmorId
										   ,[@MAA].DrwngAprls  ,[@MAA].IsVerbalLPO    
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                }
                else
                {
                    query = @"  
							declare @MAA TABLE(CreatedDate datetime,LPODate date,LocalPurchaseOrderId uniqueidentifier,FPOrderNmbr nvarchar(MAX),ForeignPurchaseOrderId nvarchar(MAX),
							LocalPurchaseOrderNo nvarchar(MAX), Subject nvarchar(MAX),SuplrNm nvarchar(MAX),IsDrawing varchar(5),DrawingID uniqueidentifier, 
							IsInspection varchar(5),CInspID uniqueidentifier,Status nvarchar(MAX),Histroy uniqueidentifier,Attachments nvarchar(MAX),Cancel nvarchar(MAX),Mail nvarchar(MAX),AmendEdit nvarchar(MAX),
							Edit nvarchar(MAX), Delt nvarchar(MAX),SupplierId uniqueidentifier,CusmorId uniqueidentifier,DrwngAprls bit,IsVerbalLPO bit)
						
								INSERT INTO
								@MAA (CreatedDate,LPODate,LocalPurchaseOrderId,FPOrderNmbr,ForeignPurchaseOrderId,LocalPurchaseOrderNo,
									  Subject,SuplrNm,IsDrawing,DrawingID,IsInspection,CInspID,Status,Histroy,Attachments,Cancel,Mail,AmendEdit, Edit, Delt,SupplierId,CusmorId,
									  DrwngAprls,IsVerbalLPO)
									  Select top 200 CreatedDate,LPOrderDate, LocalPurchaseOrderId, FPOrderNmbr, ForeignPurchaseOrderId, LocalPurchaseOrderNo, 
										Subject, SuplrNm, IsDrawing, DrawingID, IsInspection, CInsID, Status,  History,Attachments,Cancel, MAIL, AMENDEDIT, EDIT, 
									   Delt, SupplierId, CustomerId, DrwngAprls,IsVerbalLPO from View_GetLPOItems b  {4} order by CreatedDate Desc                  
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM @MAA) AS TotalRows, 
											(SELECT count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].LPODate
										   ,[@MAA].LocalPurchaseOrderId      
										   ,[@MAA].FPOrderNmbr
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].LocalPurchaseOrderNo
										   ,[@MAA].Subject
										   ,[@MAA].SuplrNm
										   ,[@MAA].IsDrawing
										   ,[@MAA].DrawingID
										   ,[@MAA].IsInspection
										   ,[@MAA].CInspID
										   ,[@MAA].Status
										   ,[@MAA].Histroy
                                           ,[@MAA].Attachments
										   ,[@MAA].Cancel,[@MAA].MAIL
										   ,[@MAA].AmendEdit
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
										   ,[@MAA].SupplierId
										   ,[@MAA].CusmorId
										   ,[@MAA].DrwngAprls  ,[@MAA].IsVerbalLPO    
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                }


                string where = " CompanyID = '" + ((HttpContext.Current.Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole ? Guid.Empty.ToString() : HttpContext.Current.Session["CompanyID"].ToString()) + "' and IsActive <> 0 ");

                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        //if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                                s.ToString() == "" ? " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and IsActive <> 0 and " + where :
                                                    " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and IsActive <> 0 and " + where :
                                " where  CONVERT(nvarchar(12), CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and IsActive <> 0" + s.ToString() + "and" + where);
                        }
                    }
                    else if (Mode == "tldt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where IsActive<> 0 and " + where : " where IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and IsActive <> 0 and " + where :
                                   "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and IsActive <> 0" + s.ToString() + " and " + where);
                        }
                    }

                    else if (Mode == "DPtdt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CONVERT(Date, DATEADD(week, DwngAprlRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 and " + where :
                                 "where CONVERT(Date, DATEADD(week, DwngAprlRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(Date, DATEADD(week, DwngAprlRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 and " + where :
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(Date, DATEADD(week, DwngAprlRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0" + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "Ict")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CONVERT(Date, DATEADD(week, InsptnRmdTm,LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 and " + where :
                                 "where CONVERT(Date, DATEADD(week, InsptnRmdTm,LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(Date, DATEADD(week, InsptnRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 and " + where :
                                  "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(Date, DATEADD(week, InsptnRmdTm, LPOrderDate)) = CONVERT(date, GETDATE()) and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "Etdt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CONVERT(date,DATEADD(WEEK,CEERmdTm,LPOrderDate)) = convert(date,getdate()) and IsActive<> 0 and " + where :
                                "where CONVERT(date,DATEADD(WEEK,CEERmdTm,LPOrderDate)) = convert(date,getdate()) and IsActive<> 0 and " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(date,DATEADD(WEEK,CEERmdTm,LPOrderDate)) = convert(date,getdate()) and IsActive<> 0 and " + where :
                                " where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and CONVERT(date,DATEADD(WEEK,CEERmdTm,LPOrderDate)) = convert(date,getdate()) and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "dtdtd")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where DrwngAprls = 1 AND ISNULL(Drawing, 0) != 3 and IsActive <> 0 and " + where :
                                " where DrwngAprls = 1 AND ISNULL(Drawing, 0) != 3 and IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and DrwngAprls = 1 AND ISNULL(Drawing, 0) != 3 and IsActive<> 0 and " + where :
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and DrwngAprls = 1 AND ISNULL(Drawing, 0) != 3 and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "mtd")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where LpoInspection = 1 AND ISNULL(ItemInspection, 0) != 3 and IsActive<> 0 and " + where :
                                "where LpoInspection = 1 AND ISNULL(ItemInspection, 0) != 3 and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and LpoInspection = 1 AND ISNULL(ItemInspection, 0) != 3 and IsActive<> 0 and " + where :
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and LpoInspection = 1 AND ISNULL(ItemInspection, 0) != 3 and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "cpd")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"] == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where IsCentralExcise = 1 AND ISNULL(CT1, 0) != 3 and IsActive<> 0 and " + where :
                            "where IsCentralExcise = 1 AND ISNULL(CT1, 0) != 3 and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and IsCentralExcise = 1 AND ISNULL(CT1, 0) != 3 and IsActive<> 0 and " + where :
                                "where CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and IsCentralExcise = 1 AND ISNULL(CT1, 0) != 3 and IsActive<> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else
                    {
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                            s.ToString() == "" ? " where IsActive <> 0 and " + where : " where IsActive <> 0 " + s.ToString() + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["LocalPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string LPONo = data["LocalPurchaseOrderNo"].ToString().Replace("\"", "\\\"");
                    string LPOId = data["LocalPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    LPONo = LPONo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=LPODetails.aspx?ID={0}>{1}</a>""", LPOId, LPONo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    sb.Append(",");

                    string RecDt = data["FPOrderNmbr"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["SuplrNm"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string IsDrawing = data["IsDrawing"].ToString().Replace("\"", "\\\"");
                    string FPOID = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    string CutID = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    string SuppID = data["SupplierId"].ToString().Replace("\"", "\\\"");
                    string DrwngId = data["DrawingID"].ToString().Replace("\"", "\\\"");
                    IsDrawing = IsDrawing.Replace("\t", "-");
                    if (Convert.ToBoolean(IsDrawing) == true && new Guid(DrwngId) != Guid.Empty)
                    {
                        sb.AppendFormat(@"""6"": ""<a href=../Purchases/DrawingDetails.Aspx?ID={0}>Drawings</a>""", DrwngId + "&LPODA=1", LPOId, CutID, SuppID, FPOID);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""6"": """"");
                        sb.Append(",");
                    }

                    string IsInspection = data["IsInspection"].ToString().Replace("\"", "\\\"");
                    IsInspection = IsInspection.Replace("\t", "-");
                    string InspId = data["CInspID"].ToString().Replace("\"", "\\\"");
                    if (Convert.ToBoolean(IsInspection) == true && new Guid(InspId) != Guid.Empty)
                    {
                        sb.AppendFormat(@"""7"": ""<a href=../Purchases/InsptnReportDetails.Aspx?ID={0}>Inspection</a>""", InspId + "&LPOINSP=1", LPOId, CutID, SuppID, FPOID);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""7"": """"");
                        sb.Append(",");
                    }

                    string Histroy = data["Histroy"].ToString().Replace("\"", "\\\"");
                    Histroy = Histroy.Replace("\t", "-");
                    if (Histroy != "")
                    {
                        sb.AppendFormat(@"""8"": ""<a href=../Purchases/LpoAmendments.aspx?LpoID={0}>Amendments</a>""", LPOId);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""8"": """"");
                        sb.Append(",");
                    }
                    string Attachment = data["Attachments"].ToString().Replace("\"", "\\\"");
                    Attachment = Attachment.Replace("\t", "-");
                    if (Attachment != "")
                    {
                        //sb.AppendFormat(@"""9"": ""<a id='btnSubmit' OnClick='DownloadBtnEvent()' href=>Attachments</a>""",  ATM.Replace(Environment.NewLine, "\\n"));
                        sb.AppendFormat(@"""9"": ""{0}""", Att_open(data["Attachments"].ToString()));
                        //Session["ATM"] = ATM;
                        sb.Append(",");
                        //Atta();
                    }
                    else
                    {
                        sb.AppendFormat(@"""9"": """"");
                        sb.Append(",");
                    }
                    string Cancel = data["Cancel"].ToString().Replace("\"", "\\\"");
                    Cancel = Cancel.Replace("\t", "-");
                    //if (data["IsVerbalLPO"].ToString().Replace("\"", "\\\"") != "True")
                    //{
                    sb.AppendFormat(@"""10"": ""{0}""", Cancel.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""9"": ""{0}""", "");
                    //    sb.Append(",");
                    //}

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string AmendEdit = data["AmendEdit"].ToString().Replace("\"", "\\\"");
                    AmendEdit = AmendEdit.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", AmendEdit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "LPO Status WebService", ex.Message.ToString());
                return "";
            }
        }


        /// <summary>
        /// FOR LOCAL PURCHASE ORDER
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetLPOAmendmensItems()
        {
            try
            {
                string LPOID = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["LpoID"];
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string date = HttpContext.Current.Request.Params["sSearch_0"];
                string LPNo = HttpContext.Current.Request.Params["sSearch_1"];
                string FPONo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string Status = HttpContext.Current.Request.Params["sSearch_4"];
                string SuplierNme = HttpContext.Current.Request.Params["sSearch_5"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and LPOrderDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (LPNo != "")
                    s.Append(" and LocalPurchaseOrderNo LIKE '%" + LPNo.Replace("'", "''") + "%'");
                if (FPONo != "")
                    s.Append(" and dbo.FN_MergeTableColumnFPO(ForeignPurchaseOrderId) LIKE '%" + FPONo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    Status = Status.Replace("'", "");
                s.Append(" and  REPLACE(Status,'''','') LIKE '%" + Status.Replace("'", "''") + "%'");
                if (SuplierNme != "")
                    s.Append(" and SuplrNm LIKE '%" + SuplierNme.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE LPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LocalPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FPOrderNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SuplrNm LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", CreatedDate ");
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
							declare @MAA TABLE(CreatedDate datetime,LPODate datetime,LocalPurchaseOrderId uniqueidentifier,LPOrderNmbr nvarchar(MAX),RefFPO nvarchar(MAX),
							Subject nvarchar(MAX),SuplrNm nvarchar(MAX),Status nvarchar(MAX))
							
								

								INSERT INTO
								@MAA (CreatedDate,LPODate,LocalPurchaseOrderId ,LPOrderNmbr ,RefFPO,Subject ,SuplrNm ,Status)
									  Select distinct  b.CreatedDate,LPOrderDate,ID,
										LocalPurchaseOrderNo,FPOrderNmbr,b.Subject,
										SuplrNm, Status from View_GetLPOAmendmensItems  b 
                                 
									 {4}                   
							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].LPODate
										   ,[@MAA].LocalPurchaseOrderId      
										   ,[@MAA].LPOrderNmbr
										   ,[@MAA].RefFPO,[@MAA].Subject,[@MAA].SuplrNm,[@MAA].Status
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";



                string where = "where LocalPurchaseOrderId =  '" + LPOID + "'";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                          where + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["LocalPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    sb.Append(",");
                    string LPONo = data["LPOrderNmbr"].ToString().Replace("\"", "\\\"");
                    string LPOId = data["LocalPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    LPONo = LPONo.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""<a href=LPOAmendmentsDetails.aspx?ID={0}&LPOID={1}>{2}</a>""", LPOId, LPOID, LPONo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    string RefFPO = data["RefFPO"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", RefFPO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");



                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["SuplrNm"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "LPO Status WebService", ex.Message.ToString());
                return "";
            }
        }



        /// <summary>
        /// FOR Drawing Approval
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetDrwngItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Drawing = HttpContext.Current.Request.Params["sSearch_0"];
                string Customer = HttpContext.Current.Request.Params["sSearch_1"];
                string Supplier = HttpContext.Current.Request.Params["sSearch_2"];
                string FPO = HttpContext.Current.Request.Params["sSearch_3"];
                string LPO = HttpContext.Current.Request.Params["sSearch_4"];
                string date = HttpContext.Current.Request.Params["sSearch_5"];

                StringBuilder s = new StringBuilder();

                if (Drawing != "")
                    s.Append(" and DrawingRefNo LIKE '%" + Drawing.Replace("'", "''") + "%'");
                if (Customer != "")
                    s.Append(" and CustNM LIKE '%" + Customer.Replace("'", "''") + "%'");
                if (Supplier != "")
                    s.Append(" and SuplrNm LIKE '%" + Supplier.Replace("'", "''") + "%'");
                if (FPO != "")
                    s.Append(" and FPO LIKE '%" + FPO.Replace("'", "''") + "%'");
                if (LPO != "")
                    s.Append(" and LPO LIKE '%" + LPO.Replace("'", "''") + "%'");
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and LPODate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE DrawingRefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CusmorId LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SupplierID LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FPO LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LPO LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LPODate LIKE ");
                    sb.Append(wrappedSearch);
                    //sb.Append(" OR CusmorId LIKE ");
                    //sb.Append(wrappedSearch);


                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;
                //sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                //sb.Append(" ");

                //sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                //orderByClause = sb.ToString();
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", DrawingID ");
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

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"  
							declare @MAA TABLE(createdDate datetime,DrawingID uniqueidentifier,LPODate date,DrawingRefNo nvarchar(MAX), FPO nvarchar(max),LPO nvarchar(max),
												SupplierId nvarchar(max),CusmorId nvarchar(max),Edit nvarchar(MAX), Delt nvarchar(MAX), CompanyId uniqueidentifier)
							INSERT
							INTO
								@MAA (createdDate,DrawingID,LPODate,DrawingRefNo,FPO,LPO,SupplierId,CusmorId,Edit,Delt,CompanyId)
									 Select s.CreatedDate,DrawingID, LPODate, DrawingRefNo,FPO,LPO,SuplrNm, CustNM, EDIT, Delt, CompanyId
									 from View_DrwngAprvlStatus s
								   
									 {4}                   
							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].DrawingID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].DrawingID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].createdDate,[@MAA].DrawingRefNo 
										   ,[@MAA].CusmorId
										   ,[@MAA].SupplierID      
										   ,[@MAA].FPO
										   ,[@MAA].LPO
										   ,[@MAA].LPODate
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
										   ,[@MAA].DrawingID
										   ,[@MAA].CompanyId     
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by createdDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                string where = " s.IsActive<> 0 and s.CompanyId= " + "'" + CompanyID + "'" + " ";
                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "DCtldt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where s.DrwngAprls = 1 AND ISNULL(s.Drawing, 0) = 3 AND s.IsActive <> 0 and " + where : " where s.DrwngAprls = 1 AND ISNULL(s.Drawing, 0) = 3 AND s.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where s.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and s.DrwngAprls = 1 AND ISNULL(s.Drawing, 0) = 3 and s.IsActive <> 0 and " + where :
                                "where s.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and s.DrwngAprls = 1 AND ISNULL(s.Drawing, 0) = 3 and s.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "Dtdt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "Where CONVERT(nvarchar(12), s.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and " + where :
                                "Where CONVERT(nvarchar(12), s.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where s.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(nvarchar(12), s.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and " + where :
                                 "where s.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(nvarchar(12), s.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString() + " and " + where);
                        }
                    }
                    else
                    {
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            "where s.IsActive<> 0 and " + where : "where s.IsActive <> 0" + s.ToString() + " and " + where);
                    }
                }
                s.Clear();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                //try
                //{
                //    conn.Open();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.ToString());
                //}

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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["DrawingID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    //sb.Append(",");

                    string DREF = data["DrawingRefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
                    string DID = data["DrawingID"].ToString().Replace("\"", "\\\"");
                    //ItemDescription = ItemDescription.Replace("'", "&#39;"); // Single Quote
                    //ItemDescription = ItemDescription.Replace(@"\","-"); //<a href="http://www.google.com/">second line</a>
                    DREF = DREF.Replace("\t", "-"); // 
                    sb.AppendFormat(@"""0"": ""<a href=DrawingDetails.Aspx?ID={0}>{1}</a>""", DID, DREF.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    string RecDt = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    //PartNo = PartNo.Replace("'", "&#39;");
                    //PartNo = PartNo.Replace(@"\", "-");
                    //RecDt = RecDt.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["SupplierID"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //Subjt = Subjt.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //string StatID = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
                    ////Spec = Spec.Replace("'", "&#39;");
                    ////Spec = Spec.Replace(@"\","-");
                    //StatID = StatID.Replace("\t", "-");
                    //sb.AppendFormat(@"""4"": ""{0}""", StatID.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string StatIDd = data["FPO"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //StatIDd = StatIDd.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //string DeptID = data["SupplierIds"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""5"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));

                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //DeptID = DeptID.Replace("\t", "-");
                    //sb.Append(",");

                    string CustID = data["LPO"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Del = Del.Replace("\t", "-");
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "Drawing Status WebService", ex.Message.ToString());
                return "";
            }
        }


        /// <summary>
        /// FOR Inspection 
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetInspItems()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string InsptnRefNo = HttpContext.Current.Request.Params["sSearch_0"];
                string InsDt = HttpContext.Current.Request.Params["sSearch_1"];
                string Inspector = HttpContext.Current.Request.Params["sSearch_2"];
                string TDInspector = HttpContext.Current.Request.Params["sSearch_3"];
                string CntPersn = HttpContext.Current.Request.Params["sSearch_4"];
                string CntNum = HttpContext.Current.Request.Params["sSearch_5"];
                string CntAddrs = HttpContext.Current.Request.Params["sSearch_6"];
                string InsDtls = HttpContext.Current.Request.Params["sSearch_7"];
                string Stage = HttpContext.Current.Request.Params["sSearch_8"];
                string ReplnDt = HttpContext.Current.Request.Params["sSearch_9"];
                string Status = HttpContext.Current.Request.Params["sSearch_10"];
                StringBuilder s = new StringBuilder();


                if (InsptnRefNo != "")
                    s.Append(" and RefNo LIKE '%" + InsptnRefNo.Replace("'", "''") + "%'");
                if (InsDt != "")
                {
                    InsDt = InsDt.Replace("'", "''");
                    DateTime FrmDt = InsDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(InsDt.Split('~')[0].ToString());
                    DateTime EndDat = InsDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(InsDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and InsDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Inspector != "")
                    s.Append(" and dbo.FN_GetUserName(SInsptctor) LIKE '%" + Inspector.Replace("'", "''") + "%'");
                if (TDInspector != "")
                    s.Append(" and dbo.FN_GetUserName(TdpInspctor) LIKE '%" + TDInspector.Replace("'", "''") + "%'");
                if (CntPersn != "")
                    s.Append(" and CP LIKE '%" + CntPersn.Replace("'", "''") + "%'");
                if (CntNum != "")
                    s.Append(" and CN LIKE '%" + CntNum.Replace("'", "''") + "%'");
                if (CntAddrs != "")
                    s.Append(" and CA LIKE '%" + CntAddrs.Replace("'", "''") + "%'");
                if (InsDtls != "")
                    s.Append(" and ir.InsDtls LIKE '%" + InsDtls.Replace("'", "''") + "%'");
                if (Stage != "")
                    s.Append(" and Ins LIKE '%" + Stage.Replace("'", "''") + "%'");
                if (ReplnDt != "")
                {
                    ReplnDt = ReplnDt.Replace("'", "''");
                    DateTime FrmDt = ReplnDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ReplnDt.Split('~')[0].ToString());
                    DateTime EndDat = ReplnDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ReplnDt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and (CASE When ir.DispRE != '' Then ir.DispRE Else ir.RePlanDate End) between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Status != "")
                    s.Append(" and ir.InsStatus LIKE '%" + Status.Replace("'", "''") + "%'");
                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE RefNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR InsDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SInsptctor LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CntDtls LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR InsDetails LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR InsStage LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR RePlanDt LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR InsStatus LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", InsPlanID ");
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
							declare @MAA TABLE(CInsID uniqueidentifier,InsPlanID uniqueidentifier,SInsptctor nvarchar(MAX), TdpInspctor nvarchar(MAX),
							InsDetails nvarchar(MAX),InsStatus nvarchar(MAX),RefNo nvarchar(MAX),
										InsStage nvarchar(MAX),ContactPerson nvarchar(MAX),ContactNumber nvarchar(MAX),InsDate date,InsPlace nvarchar(MAX), DispRE date, CreatedBy uniqueidentifier,CreatedDate datetime, EDIT nvarchar(Max), Delt nvarchar(Max))
							INSERT
							INTO
							   @MAA (CInsID,InsPlanID,SInsptctor,TdpInspctor,InsDetails,InsStatus,RefNo,InsStage,
								ContactPerson,ContactNumber,InsDate,InsPlace,DispRE, CreatedBy,CreatedDate,EDIT, Delt)
								Select CInsID,InsPlanID,SInsptctor,TdpInspctor,InsDetails,InsStatus,RefNo,InsStage,
								ContactPerson,ContactNumber,InsDate,InsPlace,DispRE, CreatedBy,CreatedDate,EDIT, Delt from View_GetInspItems ir
									 {4}                   
							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].InsPlanID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].InsPlanID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CInsID     
										   ,[@MAA].InsPlanID
										   ,[@MAA].SInsptctor      
										   ,[@MAA].TdpInspctor
										   ,[@MAA].InsDetails
										   ,[@MAA].InsStatus
										   ,[@MAA].RefNo
										   ,[@MAA].InsStage
										   ,[@MAA].ContactPerson
										   ,[@MAA].ContactNumber
										   ,[@MAA].InsDate
										   ,[@MAA].InsPlace 
										   ,[@MAA].DispRE 
										   ,[@MAA].CreatedBy
										   ,[@MAA].CreatedDate  
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
	 
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} Order by CreatedDate DESC";

                Guid CompanyId = new Guid(Session["CompanyId"].ToString());
                string where = " ir.IsActive  <> 0  and ir.CompanyId = " + "'" + CompanyId + "'" + " ";

                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "ICtldt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where ir.IsActive  <> 0 and " + where : "where ir.IsActive <> 0 " + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where ir.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and " + where :
                                "where ir.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + s.ToString() + " and " + where);
                        }
                    }
                    else if (Mode == "DCtldt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "Where CONVERT(nvarchar(12), ir.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and " + where :
                                 "Where CONVERT(nvarchar(12), ir.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString() + " and " + where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                "where ir.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(nvarchar(12), ir.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and " + where :
                                "where ir.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(nvarchar(12), ir.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString() + " and " + where);
                        }
                    }
                    else
                    {
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                             " where ir.IsActive <> 0 and " + where : " where ir.IsActive <> 0 " + s.ToString() + " and " + where);
                    }
                }
                s.Clear();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);

                //try
                //{
                //    conn.Open();
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.ToString());
                //}

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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CInsID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    //sb.Append(",");

                    string IREF = data["RefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
                    string CIID = data["CInsID"].ToString().Replace("\"", "\\\"");
                    //ItemDescription = ItemDescription.Replace("'", "&#39;"); // Single Quote
                    //ItemDescription = ItemDescription.Replace(@"\","-"); //<a href="http://www.google.com/">second line</a>
                    IREF = IREF.Replace("\t", "-"); // 
                    sb.AppendFormat(@"""0"": ""<a href=InsptnReportDetails.Aspx?ID={0}>{1}</a>""", CIID, IREF.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["InsDate"]);
                    sb.Append(",");

                    string Subjt = data["SInsptctor"].ToString().Replace("\"", "\\\"");
                    string TDIns = data["TdpInspctor"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //Subjt = Subjt.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"), TDIns.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""3"": ""{1}""", Subjt.Replace(Environment.NewLine, "\\n"), TDIns.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //string StatID = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
                    ////Spec = Spec.Replace("'", "&#39;");
                    ////Spec = Spec.Replace(@"\","-");
                    //StatID = StatID.Replace("\t", "-");
                    //sb.AppendFormat(@"""4"": ""{0}""", StatID.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string CntP = data["ContactPerson"].ToString().Replace("\"", "\\\"");
                    string CntN = data["ContactNumber"].ToString().Replace("\"", "\\\"");
                    string InsP = data["InsPlace"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //StatIDd = StatIDd.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", CntP.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""5"": ""{0}""", CntN.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    sb.AppendFormat(@"""6"": ""{0}""", InsP.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    //string DeptID = data["SupplierIds"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""5"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));

                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    //DeptID = DeptID.Replace("\t", "-");
                    //sb.Append(",");

                    string CustID = data["InsDetails"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Mail = data["InsStage"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""9"": ""{0:dd-MM-yyyy}""", data["DispRE"]);
                    sb.Append(",");

                    string Stat = data["InsStatus"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Stat = Stat.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", Stat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""11"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Spec = Spec.Replace("'", "&#39;");
                    //Spec = Spec.Replace(@"\","-");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "Drawing Status WebService", ex.Message.ToString());
                return "";
            }
        }


        /// <summary>
        /// FOR LOCAL PURCHASE ORDER Over Dues
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetLPOOverDue()
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


                string LPNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                //string Todate = HttpContext.Current.Request.Params["sSearch_2"];
                string FPONo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string SuplierNme = HttpContext.Current.Request.Params["sSearch_4"];
                string Status = HttpContext.Current.Request.Params["sSearch_5"];
                string CreatedBy = HttpContext.Current.Request.Params["sSearch_6"];
                string DelFrmDate = HttpContext.Current.Request.Params["sSearch_7"];
                //string DelToDate = HttpContext.Current.Request.Params["sSearch_8"];
                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and lp.LocalPurchaseOrderDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (LPNo != "")
                    s.Append(" and lp.LocalPurchaseOrderNo LIKE '%" + LPNo.Replace("'", "''") + "%'");
                if (FPONo != "")
                    s.Append(" and dbo.FN_MergeTableColumnFPO(lp.ForeignPurchaseOrderId) LIKE '%" + FPONo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and lp.Subject LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(lp.StatusTypeId) LIKE '%" + Status.Replace("'", "''") + "%'");
                if (SuplierNme != "")
                    s.Append(" and dbo.FN_GetSupplierNm(lp.SupplierId) LIKE '%" + SuplierNme.Replace("'", "''") + "%'");
                if (CreatedBy != "")
                    s.Append(" and lp.CreatedBy LIKE '%" + CreatedBy.Replace("'", "''") + "%'");

                if (DelFrmDate != "")
                {
                    DelFrmDate = DelFrmDate.Replace("'", "''");
                    DateTime DelFrmDt = DelFrmDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(DelFrmDate.Split('~')[0].ToString());
                    DateTime DelToDt = DelFrmDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(DelFrmDate.Split('~')[1].ToString());
                    if (DelFrmDt.ToShortDateString() != "1/1/0001" && DelToDt.ToShortDateString() != "1/1/0001")
                        s.Append(" and lp.DeliveryDate between '" + DelFrmDt.ToString("MM/dd/yyyy") + "' and '" + DelToDt.ToString("MM/dd/yyyy") + "'");
                }

                //var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE LocalPurchaseOrderDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LocalPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FrnPOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SupplierName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR DeliveryDate LIKE ");
                    sb.Append(wrappedSearch);
                    //sb.Append(" OR CusmorId LIKE ");
                    //sb.Append(wrappedSearch);


                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", LocalPurchaseOrderId ");
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

                string query = @"  
							declare @MAA TABLE(LocalPurchaseOrderId uniqueidentifier, LocalPurchaseOrderNo nvarchar(max), LocalPurchaseOrderDate datetime, 
                            FrnPOrderNo nvarchar(max), DeliveryDate datetime, Subject nvarchar(max), SupplierName nvarchar(max), 
                            Status  nvarchar(max),CreatedDate datetime, CreatedBy nvarchar(MAX))

							INSERT
							INTO
								@MAA (LocalPurchaseOrderId,LocalPurchaseOrderNo,LocalPurchaseOrderDate,FrnPOrderNo,DeliveryDate,Subject,SupplierName,Status,CreatedDate, CreatedBy)
								
                             Select LocalPurchaseOrderId,LocalPurchaseOrderNo,LocalPurchaseOrderDate,FrnPOrderNo,DeliveryDate,Subject,SupplierName,Status,CreatedDate, lp.CreatedBy from View_GetLPOOverDue lp {4}
														
							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].LocalPurchaseOrderId
										   ,[@MAA].LocalPurchaseOrderNo
										   ,[@MAA].LocalPurchaseOrderDate
										   ,[@MAA].FrnPOrderNo      
										   ,[@MAA].DeliveryDate
										   ,[@MAA].Subject
										   ,[@MAA].SupplierName
										   ,[@MAA].Status
                                           ,[@MAA].CreatedDate
                                           ,[@MAA].CreatedBy
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where  CONVERT(nvarchar(12), lp.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and lp.IsActive <> 0");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where  CONVERT(nvarchar(12), lp.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "and lp.IsActive <> 0");
                        }
                    }
                    else if (Mode == "tldt")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(), "and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "and lp.IsActive <> 0");
                        }
                    }
                    else if (Mode == "DPtdt")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where CONVERT(Date, DATEADD(week, lp.DwngAprlRmdTm, lp.LocalPurchaseOrderDate)) = CONVERT(date, GETDATE()) and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and CONVERT(Date, DATEADD(week, lp.DwngAprlRmdTm, lp.LocalPurchaseOrderDate)) = CONVERT(date, GETDATE()) and lp.IsActive<> 0");
                        }
                    }
                    else if (Mode == "Ict")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where CONVERT(Date, DATEADD(week, lp.InsptnRmdTm,lp.LocalPurchaseOrderDate)) = CONVERT(date, GETDATE()) and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and CONVERT(Date, DATEADD(week, lp.InsptnRmdTm, lp.LocalPurchaseOrderDate)) = CONVERT(date, GETDATE()) and lp.IsActive<> 0");
                        }
                    }
                    else if (Mode == "Etdt")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where CONVERT(date,DATEADD(WEEK,lp.CEERmdTm,lp.LocalPurchaseOrderDate)) = convert(date,getdate()) and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and CONVERT(date,DATEADD(WEEK,lp.CEERmdTm,lp.LocalPurchaseOrderDate)) = convert(date,getdate()) and lp.IsActive<> 0");
                        }
                    }
                    else if (Mode == "dtdtd")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.DrwngAprls = 1 AND ISNULL(ITS.Drawing, 0) != 3 and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and lp.DrwngAprls = 1 AND ISNULL(ITS.Drawing, 0) != 3 and lp.IsActive<> 0");
                        }
                    }
                    else if (Mode == "mtd")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.Inspection = 1 AND ISNULL(ITS.Inspection, 0) != 3 and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and lp.Inspection = 1 AND ISNULL(ITS.Inspection, 0) != 3 and lp.IsActive<> 0");
                        }
                    }
                    else if (Mode == "cpd")
                    {
                        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.IsCentralExcise = 1 AND ISNULL(ITS.CT1, 0) != 3 and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                        }
                        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where lp.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + " and lp.IsCentralExcise = 1 AND ISNULL(ITS.CT1, 0) != 3 and lp.IsActive<> 0");
                        }
                    }
                    else
                    {
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            "where lp.IsActive<> 0 and lp.CompanyId= " + "'" + CompanyID + "'" : " where lp.IsActive <> 0 and lp.CompanyId =" + "'" + CompanyID + "'" + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["LocalPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string LPONo = data["LocalPurchaseOrderNo"].ToString().Replace("\"", "\\\"");
                    string LPOId = data["LocalPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    LPONo = LPONo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{1}""", LPOId, LPONo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["LocalPurchaseOrderDate"]);
                    sb.Append(",");

                    string RecDt = data["FrnPOrderNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["SupplierName"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CreatedByy = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", CreatedByy.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""7"": ""{0:dd-MM-yyyy}""", data["DeliveryDate"]);
                    //sb.Append(",");
                    //string IsDrawing = data["IsDrawing"].ToString().Replace("\"", "\\\"");
                    //string FPOID = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    //string CutID = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    //string SuppID = data["SupplierId"].ToString().Replace("\"", "\\\"");
                    //IsDrawing = IsDrawing.Replace("\t", "-");
                    //if (Convert.ToBoolean(IsDrawing) == true)
                    //{
                    //    sb.AppendFormat(@"""6"": ""<a href=../Purchases/DrawingApproval.aspx?LpoID={0}&CustID={1}&SupID={2}&FPOID={3}>Drawings</a>""", LPOId, CutID, SuppID, FPOID);
                    //    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""6"": """"");
                    //    sb.Append(",");
                    //}

                    //string IsInspection = data["IsInspection"].ToString().Replace("\"", "\\\"");
                    //IsInspection = IsInspection.Replace("\t", "-");
                    //if (Convert.ToBoolean(IsInspection) == true)
                    //{
                    //    sb.AppendFormat(@"""7"": ""<a href=../Purchases/InspectionPlanRequest.aspx?LpoID={0}&CustID={1}&SupID={2}&FPOID={3}>Inspection</a>""", LPOId, CutID, SuppID, FPOID);
                    //    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""7"": """"");
                    //    sb.Append(",");
                    //}

                    //string Histroy = data["Histroy"].ToString().Replace("\"", "\\\"");
                    //Histroy = Histroy.Replace("\t", "-");
                    //if (Histroy != "")
                    //{
                    //    sb.AppendFormat(@"""8"": ""<a href=../Purchases/LpoAmendments.aspx?LpoID={0}>Amendments</a>""", LPOId);
                    //    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""8"": """"");
                    //    sb.Append(",");
                    //}
                    //string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    //Mail = Mail.Replace("\t", "-");
                    //sb.AppendFormat(@"""9"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string AmendEdit = data["AmendEdit"].ToString().Replace("\"", "\\\"");
                    //AmendEdit = AmendEdit.Replace("\t", "-");
                    //sb.AppendFormat(@"""10"": ""{0}""", AmendEdit.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    //Edit = Edit.Replace("\t", "-");
                    //sb.AppendFormat(@"""11"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("\t", "-");
                    //sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "LPO Status WebService", ex.Message.ToString());
                //return "";
            }
            // handles zero records
            //if (totalRecords == 0)
            //{
            sb.Append("{");
            sb.Append(@"""sEcho"": ");
            sb.AppendFormat(@"""{0}""", sEcho);
            sb.Append(",");
            sb.Append(@"""iTotalRecords"": 0");
            sb.Append(",");
            sb.Append(@"""iTotalDisplayRecords"": 0");
            sb.Append(", ");
            sb.Append(@"""aaData"": [ ]}");
            //sb.Append("]}");
            outputJson = sb.ToString();

            return outputJson;
            //}
        }


        /// <summary>
        /// FOR FPO
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFPOOverDues()
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

                string FPOrderNo = HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"];
                string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_3"];
                string Sup = HttpContext.Current.Request.Params["sSearch_4"];
                string Status = HttpContext.Current.Request.Params["sSearch_5"];
                string CreatedBy = HttpContext.Current.Request.Params["sSearch_6"];
                string DelDate = HttpContext.Current.Request.Params["sSearch_7"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [FPODate] between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FPOrderNo != "")
                    s.Append(" and [ForeignPurchaseOrderNo] LIKE '%" + FPOrderNo.Replace("'", "''") + "%'");
                if (FeNo != "")
                    s.Append(" and  [FrnEnqNo] LIKE '%" + FeNo.Replace("'", "''") + "%'");
                if (Subject != "")
                    s.Append(" and [Subject] LIKE '%" + Subject.Replace("'", "''") + "%'");
                if (Status != "")
                    s.Append(" and [Status] LIKE '%" + Status.Replace("'", "''") + "%'");
                if (CreatedBy != "")
                    s.Append(" and [CreatedBy] LIKE '%" + CreatedBy.Replace("'", "''") + "%'");
                if (DelDate != "")
                {
                    DelDate = DelDate.Replace("'", "''");
                    DateTime DelFrmDt = DelDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(DelDate.Split('~')[0].ToString());
                    DateTime DelToDat = DelDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(DelDate.Split('~')[1].ToString());
                    if (DelFrmDt.ToShortDateString() != "1/1/0001" && DelToDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and [DeliveryDate] between '" + DelFrmDt.ToString("MM/dd/yyyy") + "' and '" + DelToDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Sup != "")
                    s.Append(" and [CustomerName] LIKE '%" + Sup.Replace("'", "''") + "%'");

                //var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FrnEnqNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR DeliveryDate LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", FPODate ");
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
							declare @MAA TABLE(ForeignPurchaseOrderId nvarchar(max), ForeignPurchaseOrderNo nvarchar(max), FPODate datetime, 
												FrnEnqNo nvarchar(max), DeliveryDate datetime,
												Subject nvarchar(max), CustomerName nvarchar(max), Status nvarchar(max), CreatedDate datetime, CreatedBy nvarchar(MAX));
						 
							INSERT INTO
								@MAA (ForeignPurchaseOrderId, ForeignPurchaseOrderNo, FPODate,FrnEnqNo, DeliveryDate,Subject, CustomerName, Status, CreatedDate, CreatedBy) 
										select [ForeignPurchaseOrderId],[ForeignPurchaseOrderNo] ,[FPODate],
									   [FrnEnqNo],[DeliveryDate],[Subject] ,[CustomerName] ,[Status], CreatedDate, CreatedBy  from dbo.[View_FPOOverDue_Export] {4}

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ForeignPurchaseOrderId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].ForeignPurchaseOrderId
										   ,[@MAA].ForeignPurchaseOrderNo 
										   ,[@MAA].FPODate
										   ,[@MAA].FrnEnqNo
										   ,[@MAA].DeliveryDate
										   ,[@MAA].Subject
										   ,[@MAA].CustomerName
										   ,[@MAA].Status
                                           ,[@MAA].CreatedBy
										   ,[@MAA].CreatedDate 
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                //if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                //{
                //    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                //    if (Mode == "tdt")
                //    {
                //        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                //        {
                //            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                //                "where  CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)");
                //        }
                //        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                //        {
                //            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                //                "where  CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString());
                //        }
                //    }
                //    else if (Mode == "tldt")
                //    {
                //        if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                //        {
                //            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(), "where f.IsActive <> 0 order by FPODate DESC ");
                //        }
                //        else if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                //        {
                //            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                //                "where f.CreatedBy =" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString());
                //        }
                //    }
                //    else
                // {

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    "where IsActive <> 0 and CompanyId =" + "'" + CompanyID + "'" : " where IsActive <> 0 and CompanyId = " + "'" + CompanyID + "'" + s.ToString());
                //}
                s.Clear();
                //}
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignPurchaseOrderId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Fpno = data["ForeignPurchaseOrderNo"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string FPid = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    Fpno = Fpno.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{1}""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["FPODate"]);
                    sb.Append(",");

                    string Len = data["FrnEnqNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Len.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //sb.AppendFormat(@"""4"": ""{0}""", Convert.ToDecimal(data["OrderValue"].ToString()).ToString("N"));
                    //sb.Append(",");

                    string CusId = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", CusId.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CreatedByy = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", CreatedByy.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""7"": ""{0:dd-MM-yyyy}""", data["DeliveryDate"]);

                    //string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    //Mail = Mail.Replace("\t", "-");
                    //sb.AppendFormat(@"""7"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    //Edit = Edit.Replace("\t", "-");
                    //sb.AppendFormat(@"""8"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("\t", "-");
                    //sb.AppendFormat(@"""9"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                //return "";
            }

            // handles zero records
            sb.Append("{");
            sb.Append(@"""sEcho"": ");
            sb.AppendFormat(@"""{0}""", sEcho);
            sb.Append(",");
            sb.Append(@"""iTotalRecords"": 0");
            sb.Append(",");
            sb.Append(@"""iTotalDisplayRecords"": 0");
            sb.Append(", ");
            sb.Append(@"""aaData"": [ ]}");
            //sb.Append("]}");
            outputJson = sb.ToString();
            return outputJson;
        }

        /// <summary>
        /// FOR FPO
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetInsptnPlan()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string RefNo = HttpContext.Current.Request.Params["sSearch_0"];
                string InsDate = HttpContext.Current.Request.Params["sSearch_1"];
                string CustmNm = HttpContext.Current.Request.Params["sSearch_2"];
                string SuplrNm = HttpContext.Current.Request.Params["sSearch_3"];
                string FpoNos = HttpContext.Current.Request.Params["sSearch_4"];
                string LpoNos = HttpContext.Current.Request.Params["sSearch_5"];
                string InsStage = HttpContext.Current.Request.Params["sSearch_6"];

                StringBuilder s = new StringBuilder();
                if (InsDate != "")
                {
                    InsDate = InsDate.Replace("'", "''");
                    DateTime FrmDt = InsDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(InsDate.Split('~')[0].ToString());
                    DateTime EndDat = InsDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(InsDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ins.InsDate between '" + FrmDt.ToString("yyyy/MM/dd") + "' and '" + EndDat.ToString("yyyy/MM/dd") + "'");
                }
                if (RefNo != "")
                    s.Append(" and ins.RefNo LIKE '%" + RefNo.Replace("'", "''") + "%'");
                if (CustmNm != "")
                    s.Append(" and ins.CustmNm LIKE '%" + CustmNm.Replace("'", "''") + "%'");
                if (SuplrNm != "")
                    s.Append(" and ins.SuplrNm LIKE '%" + SuplrNm.Replace("'", "''") + "%'");
                if (FpoNos != "")
                    s.Append(" and ins.FpoNos LIKE '%" + FpoNos.Replace("'", "''") + "%'");
                if (LpoNos != "")
                    s.Append(" and ins.LpoNos LIKE '%" + LpoNos.Replace("'", "''") + "%'");
                if (InsStage != "")
                    s.Append(" and ins.InsStage LIKE '%" + InsStage.Replace("'", "''") + "%'");
                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE FPODate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR FrnEnqNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Subject LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Status LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR DeliveryDate LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", InsDate ");
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
							declare @MAA TABLE(CreatedDate datetime,InsPlanID nvarchar(max), RefNo nvarchar(max), InsDate datetime,CustmNm nvarchar(max),
								SuplrNm nvarchar(max), FpoNos nvarchar(max),LpoNos nvarchar(max), InsStage nvarchar(max),Status nvarchar(max),EDIT nvarchar(max),Delt nvarchar(max));

							INSERT INTO
								@MAA (CreatedDate,InsPlanID, RefNo, InsDate,CustmNm, SuplrNm,FpoNos, LpoNos, InsStage,Status,EDIT,Delt) 
								
								select CreatedDate,InsPlanID,RefNo,InsDate,CustmNm,SuplrNm,FpoNos,LpoNos,InsStage,Status,EDIT,Delt from View_GetInsptnPlan ins  where ins.IsActive <> 0 
                                {4}

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].InsPlanID)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].InsPlanID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].InsPlanID
										   ,[@MAA].RefNo 
										   ,[@MAA].InsDate
										   ,[@MAA].CustmNm
										   ,[@MAA].SuplrNm
										   ,[@MAA].FpoNos
										   ,[@MAA].LpoNos
										   ,[@MAA].InsStage
										   ,[@MAA].Status
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                string where = "and ins.CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + "'";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    " " + where : s.ToString() + where + "order by InsDate desc");
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["InsPlanID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string RefNum = data["RefNo"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    string InsPlanID = data["InsPlanID"].ToString().Replace("\"", "\\\"");
                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    RefNum = RefNum.Replace("\t", "-");
                    if (StatIDd != "Closed")
                    {
                        sb.AppendFormat(@"""0"": ""<a href=InspectionReport.aspx?InsPlnID={0}>{1}</a>""", InsPlanID, RefNum.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""0"": ""{1}""", InsPlanID, RefNum.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["InsDate"]);
                    sb.Append(",");

                    string CustmNam = data["CustmNm"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", CustmNam.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string SuplrNam = data["SuplrNm"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", SuplrNam.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FpoNums = data["FpoNos"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", FpoNums.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string LpoNums = data["LpoNos"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", LpoNums.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string InsStage_stat = data["InsStage"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", InsStage_stat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR RqstInspnPlan
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetRqstInspnPlan()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Cust = HttpContext.Current.Request.Params["sSearch_0"];
                string FPOrderNo = HttpContext.Current.Request.Params["sSearch_1"];
                string LPO = HttpContext.Current.Request.Params["sSearch_2"];
                string Sup = HttpContext.Current.Request.Params["sSearch_3"];
                string ReqNo = HttpContext.Current.Request.Params["sSearch_4"];
                string ReqDate = HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"];


                StringBuilder s = new StringBuilder();
                if (Cust != "")
                    s.Append(" and dbo.GetActiveCustomerName(Cust) LIKE '%" + Cust.Replace("'", "''") + "%'");
                if (FPOrderNo != "")
                    s.Append(" and dbo.FN_MergeTableColumnFPO(FPO) LIKE '%" + FPOrderNo.Replace("'", "''") + "%'");
                if (LPO != "")
                    s.Append(" and dbo.FN_MergeTableColumnLPO(LPO) LIKE '%" + LPO.Replace("'", "''") + "%'");
                if (Sup != "")
                    s.Append(" and dbo.FN_GetSupplierBusinessNm(Sup) LIKE '%" + Sup.Replace("'", "''") + "%'");
                if (ReqNo != "")
                    s.Append(" and RequestNumber LIKE '%" + ReqNo.Replace("'", "''") + "%'");
                if (ReqDate == "~")
                    ReqDate = ReqDate.Replace("~", "");
                if (ReqDate != "")
                {

                    ReqDate = ReqDate.Replace("'", "''");
                    DateTime FromDate = ReqDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ReqDate.Split('~')[0].ToString());
                    DateTime ToDate = ReqDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ReqDate.Split('~')[1].ToString());
                    if (FromDate.ToShortDateString() != "1/1/0001")
                        s.Append(" and RequestDate BETWEEN '" + FromDate.ToString("MM/dd/yyyy") + "' and '" + ToDate.ToString("MM/dd/yyyy") + "'");
                }
                if (Status != "")
                    s.Append(" and Status LIKE '%" + Status.Replace("'", "''") + "%'");



                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE CustomerName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ForeignPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR LocalPurchaseOrderNo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR SupplierName LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR RequestNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR RequestDate LIKE ");
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
                    orderByClause = orderByClause.Replace("0", ", RequestDate ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "InsReqID ASC";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"declare @MAA TABLE(CreatedDate datetime,CustomerName nvarchar(max), ForeignPurchaseOrderNo nvarchar(max), 
								LocalPurchaseOrderNo nvarchar(max), SupplierName nvarchar(max),RequestNumber nvarchar(max),
								RequestDate datetime,Status nvarchar(max), Mail nvarchar(MAX),Edit nvarchar(MAX), Delt nvarchar(MAX),InsReqID uniqueidentifier);                   
								
								INSERT INTO
								@MAA (CreatedDate,CustomerName, ForeignPurchaseOrderNo, LocalPurchaseOrderNo, SupplierName, RequestNumber, 
								RequestDate, Status, Mail, Edit, Delt, InsReqID) 
								Select Distinct CreatedDate,CustomerName, ForeignPurchaseOrderNo ,
                                                        LocalPurchaseOrderNo, SupplierName,
                                                        RequestNumber,  RequestDate, Status,
                                                       MAIL,
                                                       EDIT,
                                                       Delt,InsReqID from View_GetRqstInspnPlan  IR {4}


								SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								FROM (SELECT (SELECT count([@MAA].InsReqID)
								FROM @MAA) AS TotalRows
								, ( SELECT  count([@MAA].InsReqID) FROM @MAA) AS TotalDisplayRows			   
								,[@MAA].CreatedDate,[@MAA].InsReqID
								,[@MAA].CustomerName 
								,[@MAA].ForeignPurchaseOrderNo
								,[@MAA].LocalPurchaseOrderNo
								,[@MAA].SupplierName
								,[@MAA].RequestNumber
								,[@MAA].RequestDate
								,[@MAA].Status
								,[@MAA].MAIL
								,[@MAA].EDIT
								,[@MAA].Delt
								FROM
								@MAA) RawResults) Results

								WHERE
								RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                    "Where IR.IsActive <> 0 and IR.CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + "'" : " where IR.IsActive <> 0 and IR.CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + "'" + s.ToString());
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
                //var count = 0;

                while (data.Read())
                {

                    if (totalRecords.Length == 0)
                    {
                        totalRecords = data["TotalRows"].ToString();
                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    }
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["InsReqID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string Customer = data["CustomerName"].ToString().Replace("\"", "\\\"");

                    sb.AppendFormat(@"""0"": ""{0} """, Customer.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPO = data["ForeignPurchaseOrderNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", FPO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string LPOs = data["LocalPurchaseOrderNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", LPOs.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Supp = data["SupplierName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""3"": ""{0}""", Supp.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ReqNum = data["RequestNumber"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", ReqNum.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");


                    sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data["RequestDate"]);
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    if (StatIDd != "Closed")
                        sb.AppendFormat(@"""6"": ""<a href=InspectionPlan.aspx?InsReqID={0}>{1}</a>""", data["InsReqID"].ToString(), StatIDd.Replace(Environment.NewLine, "\\n"));
                    else
                        sb.AppendFormat(@"""6"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");

                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "Inspection Plan Request Status WebService", ex.Message.ToString());
                return "";
            }
        }

        /// <summary>
        /// FOR Produst Search
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetProduct()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                //string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Supp = HttpContext.Current.Request.Params["sSearch_0"];
                string PONo = HttpContext.Current.Request.Params["sSearch_1"];
                string PODt = HttpContext.Current.Request.Params["sSearch_2"];
                string Desc = HttpContext.Current.Request.Params["sSearch_3"];
                string PartNo = HttpContext.Current.Request.Params["sSearch_4"];
                string Make = HttpContext.Current.Request.Params["sSearch_5"];
                string Disc = HttpContext.Current.Request.Params["sSearch_6"];
                string Rate = HttpContext.Current.Request.Params["sSearch_7"];
                string CreatdBy = HttpContext.Current.Request.Params["sSearch_8"];

                StringBuilder s = new StringBuilder();
                if (PODt != "")
                {
                    PODt = PODt.Replace("'", "''");
                    DateTime FrmDt = PODt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PODt.Split('~')[0].ToString());
                    DateTime EndDat = PODt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PODt.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" where PODate between '" + FrmDt.ToString("yyyy/MM/dd") + "' and '" + EndDat.ToString("yyyy/MM/dd") + "'");
                }
                if (Supp != "")
                    s.Append(" and SupplierNm LIKE '%" + Supp.Replace("'", "''") + "%'");
                if (PONo != "")
                    s.Append(" and PONo LIKE '%" + PONo.Replace("'", "''") + "%'");
                if (Desc != "")
                    s.Append(" and Description LIKE '%" + Desc.Replace("'", "''") + "%'");
                if (PartNo != "")
                    s.Append(" and PartNumber LIKE '%" + PartNo.Replace("'", "''") + "%'");
                if (Make != "")
                    s.Append(" and Make LIKE '%" + Make.Replace("'", "''") + "%'");
                if (Disc != "")
                    s.Append(" and (CONVERT(varchar(18), ISNULL(DiscountID, 0.00)) + '/' + CONVERT(varchar(18), ISNULL(DiscountLP, 0.00))) LIKE '%" + Disc.Replace("'", "''") + "%'");
                if (Rate != "")
                    s.Append(" and Rate LIKE '%" + Rate.Replace("'", "''") + "%'");
                if (CreatdBy != "")
                    s.Append(" and CreatedBy LIKE '%" + CreatdBy.Replace("'", "''") + "%'");
                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE SupplierNm LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PONo LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Description LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PartNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Make LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Rate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CreatedBy LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                ////ORDERING
                sb.Clear();
                string orderByClause = string.Empty;
                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", ItemId ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "ItemId Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
							declare @MAA TABLE(ItemId uniqueidentifier,SupplierNm nvarchar(max), PONo nvarchar(max), PODate date,Description nvarchar(max),
								PartNumber nvarchar(max), Make nvarchar(max),Discount nvarchar(max), Rate nvarchar(max),CreatedBy nvarchar(max),CompanyId uniqueidentifier);

							INSERT INTO
								@MAA (ItemId,SupplierNm, PONo, PODate,Description, PartNumber,Make, Discount, Rate,CreatedBy,CompanyId) 
							   Select ItemId, SupplierNm, PONo,
                                                       PODate,  Description,
                                                       PartNumber, Make, Discount, Rate,
                                                       CreatedBy, CompanyId
                                                       from View_GetProduct ID                       
                                                                             
								 {4}
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ItemId)
											  FROM @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ItemId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].ItemId
										   ,[@MAA].SupplierNm
										   ,[@MAA].PONo 
										   ,[@MAA].PODate
										   ,[@MAA].Description
										   ,[@MAA].PartNumber
										   ,[@MAA].Make
										   ,[@MAA].Discount
										   ,[@MAA].Rate
										   ,[@MAA].CreatedBy
										   ,[@MAA].CompanyId 
									  FROM
										  @MAA {1}) RawResults) Results

				WHERE
				RowNumber BETWEEN {2} AND {3} order by PODate Desc";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (rawSearch == "")
                {
                    //query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    //    s.ToString() == "" ? " where isnull(ID.CompanyId,'') = " + "'" + Guid.Empty + "'" + "" : s.ToString() + " and isnull(ID.CompanyId,'') = " + "'" + Guid.Empty + "'" + " order by ItemId desc");
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                        s.ToString() == "" ? " where isnull(ID.CompanyId,'') = " + "'" + CompanyID.ToString() + "'" + "" : s.ToString() + " and isnull(ID.CompanyId,'') = " + "'" + CompanyID.ToString() + "'" + " order by ItemId desc");
                }
                else
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                            s.ToString() == "" ? " where isnull(ID.CompanyId,'') = " + "'" + CompanyID.ToString() + "'" + "" : s.ToString() + " and isnull(ID.CompanyId,'') = " + "'" + CompanyID.ToString() + "'" + " order by ItemId desc");
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
                //var count = 0;

                while (data.Read())
                {

                    if (totalRecords.Length == 0)
                    {
                        totalRecords = data["TotalRows"].ToString();
                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    }
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ItemId"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    //string RefNum = data["SupplierNm"].ToString().Replace("\"", "\\\"").Replace(@"\", "/");
                    //string InsPlanID = data["ItemId"].ToString().Replace("\"", "\\\"");
                    //string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    //RefNum = RefNum.Replace("\t", "-");

                    string Sup = data["SupplierNm"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""0"": ""{0}""", Sup.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string SuplrNam = data["PONo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", SuplrNam.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    //string FpoNums = data["PODate"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["PODate"]);
                    sb.Append(",");

                    string LpoNums = data["Description"].ToString().Replace("\"", "\\\"");
                    LpoNums = LpoNums.Replace("\n", "");
                    LpoNums = LpoNums.Replace("\t", "");
                    LpoNums = LpoNums.Replace("\r", "");
                    sb.AppendFormat(@"""3"": ""{0}""", LpoNums);//.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string InsStage_stat = data["PartNumber"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", InsStage_stat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Make"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Dis = data["Discount"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", Dis.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Rat = data["Rate"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""7"": ""{0}""", Rat.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Cre = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""8"": ""{0}""", Cre.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "FPO Status WebService", ex.Message.ToString());
                return "";
            }
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
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
                        + finUrl + "," + al[i].ToString() + ");' target='_blank'>" + al[i].ToString() + "</a><br/>");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            return sbb.ToString(); ;
        }
    }
}
