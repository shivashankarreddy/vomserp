using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using BAL;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace VOMS_ERP.Customer_Access
{
    /// <summary>
    /// Summary description for Customer_LPO_WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Customer_LPO_WebService : System.Web.Services.WebService
    {

        /// <summary>
        /// FOR LOCAL PURCHASE ORDER
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetCustomerLPOItems()
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
                // string FPONo = HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_2"];
                string SuplierNme = HttpContext.Current.Request.Params["sSearch_3"];
                string Status = HttpContext.Current.Request.Params["sSearch_4"];

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
                //if (FPONo != "")
                //    s.Append(" and dbo.FN_MergeTableColumnFPO(ForeignPurchaseOrderId) LIKE '%" + FPONo.Replace("'", "''") + "%'");
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
                    //sb.Append(" OR FPOrderNmbr LIKE ");
                    //sb.Append(wrappedSearch);
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

                if (LPNo != "" || (date != null && date != "") || Subject != "" || SuplierNme != "" || Status != "")
                {
                    query = @"  
							declare @MAA TABLE(CreatedDate datetime,LPODate date,LocalPurchaseOrderId uniqueidentifier,FPOrderNmbr nvarchar(MAX),ForeignPurchaseOrderId nvarchar(MAX),
							LocalPurchaseOrderNo nvarchar(MAX), Subject nvarchar(MAX),SuplrNm nvarchar(MAX),IsDrawing varchar(5),DrawingID uniqueidentifier, 
							IsInspection varchar(5),CInspID uniqueidentifier,Status nvarchar(MAX),Histroy uniqueidentifier,Cancel nvarchar(MAX),Mail nvarchar(MAX),AmendEdit nvarchar(MAX),
							Edit nvarchar(MAX), Delt nvarchar(MAX),SupplierId uniqueidentifier,CusmorId uniqueidentifier,DrwngAprls bit,IsVerbalLPO bit)
							
                            INSERT INTO
								@MAA (CreatedDate,LPODate,LocalPurchaseOrderId,FPOrderNmbr,ForeignPurchaseOrderId,LocalPurchaseOrderNo,
									  Subject,SuplrNm,IsDrawing,DrawingID,IsInspection,CInspID,Status,Histroy,Cancel,Mail,AmendEdit, Edit, Delt,SupplierId,CusmorId,
									  DrwngAprls,IsVerbalLPO)
									  Select distinct CreatedDate,LPOrderDate, LocalPurchaseOrderId, FPOrderNmbr, ForeignPurchaseOrderId, LocalPurchaseOrderNo, 
										Subject, SuplrNm, IsDrawing, DrawingID, IsInspection, CInsID, Status,  History,Cancel, MAIL, AMENDEDIT, EDIT, 
									   Delt, SupplierId, CustomerId, DrwngAprls,IsVerbalLPO from View_GetLPOItems b  {4}                   
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM @MAA) AS TotalRows, 
											(SELECT count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].LPODate
										   ,[@MAA].LocalPurchaseOrderId      
										   ,[@MAA].LocalPurchaseOrderNo
										   ,[@MAA].Subject
										   ,[@MAA].SuplrNm
										   ,[@MAA].IsDrawing
										   ,[@MAA].DrawingID
										   ,[@MAA].IsInspection
										   ,[@MAA].CInspID
										   ,[@MAA].Status
										   ,[@MAA].Histroy
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
							IsInspection varchar(5),CInspID uniqueidentifier,Status nvarchar(MAX),Histroy uniqueidentifier,Cancel nvarchar(MAX),Mail nvarchar(MAX),AmendEdit nvarchar(MAX),
							Edit nvarchar(MAX), Delt nvarchar(MAX),SupplierId uniqueidentifier,CusmorId uniqueidentifier,DrwngAprls bit,IsVerbalLPO bit,IsLPOCancelled bit)
						
								INSERT INTO
								@MAA (CreatedDate,LPODate,LocalPurchaseOrderId,FPOrderNmbr,ForeignPurchaseOrderId,LocalPurchaseOrderNo,
									  Subject,SuplrNm,IsDrawing,DrawingID,IsInspection,CInspID,Status,Histroy,Cancel,Mail,AmendEdit, Edit, Delt,SupplierId,CusmorId,
									  DrwngAprls,IsVerbalLPO,IsLPOCancelled)
									  Select top 200 CreatedDate,LPOrderDate, LocalPurchaseOrderId, FPOrderNmbr, ForeignPurchaseOrderId, LocalPurchaseOrderNo, 
										Subject, SuplrNm, IsDrawing, DrawingID, IsInspection, CInsID, Status,  History,Cancel, MAIL, AMENDEDIT, EDIT, 
									   Delt, SupplierId, CustomerId, DrwngAprls,IsVerbalLPO, IsLPOCancelled from View_GetLPOItems b  {4} order by CreatedDate Desc                  
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].LocalPurchaseOrderId)
											  FROM @MAA) AS TotalRows, 
											(SELECT count( [@MAA].LocalPurchaseOrderId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].LPODate
										   ,[@MAA].LocalPurchaseOrderId      
										   ,[@MAA].LocalPurchaseOrderNo
										   ,[@MAA].Subject
										   ,[@MAA].SuplrNm
										   ,[@MAA].IsDrawing
										   ,[@MAA].DrawingID
										   ,[@MAA].IsInspection
										   ,[@MAA].CInspID
										   ,[@MAA].Status
										   ,[@MAA].Histroy
										   ,[@MAA].Cancel,[@MAA].MAIL
										   ,[@MAA].AmendEdit
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
										   ,[@MAA].SupplierId
										   ,[@MAA].CusmorId
										   ,[@MAA].DrwngAprls  ,[@MAA].IsVerbalLPO
                                           ,[@MAA].IsLPOCancelled 
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
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
                    sb.AppendFormat(@"""0"": ""<a href=Customer_LPO_Details.aspx?ID={0}>{1}</a>""", LPOId, LPONo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["LPODate"]);
                    sb.Append(",");

                    //string RecDt = data["FPOrderNmbr"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CustID = data["SuplrNm"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string IsDrawing = data["IsDrawing"].ToString().Replace("\"", "\\\"");
                    //string FPOID = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
                    string CutID = data["CusmorId"].ToString().Replace("\"", "\\\"");
                    string SuppID = data["SupplierId"].ToString().Replace("\"", "\\\"");
                    //string DrwngId = data["DrawingID"].ToString().Replace("\"", "\\\"");
                    //IsDrawing = IsDrawing.Replace("\t", "-");
                    //if (Convert.ToBoolean(IsDrawing) == true && new Guid(DrwngId) != Guid.Empty)
                    //{
                    //    sb.AppendFormat(@"""5"": ""<a href=../Purchases/DrawingDetails.Aspx?ID={0}>Drawings</a>""", DrwngId + "&LPODA=1", LPOId, CutID, SuppID, "");
                    //    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""5"": """"");
                    //    sb.Append(",");
                    //}

                    //string IsInspection = data["IsInspection"].ToString().Replace("\"", "\\\"");
                    //IsInspection = IsInspection.Replace("\t", "-");
                    //string InspId = data["CInspID"].ToString().Replace("\"", "\\\"");
                    //if (Convert.ToBoolean(IsInspection) == true && new Guid(InspId) != Guid.Empty)
                    //{
                    //    sb.AppendFormat(@"""6"": ""<a href=../Purchases/InsptnReportDetails.Aspx?ID={0}>Inspection</a>""", InspId + "&LPOINSP=1", LPOId, CutID, SuppID, "");
                    //    sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""6"": """"");
                    //    sb.Append(",");
                    //}

                    string Histroy = data["Histroy"].ToString().Replace("\"", "\\\"");
                    Histroy = Histroy.Replace("\t", "-");
                    if (Histroy != "")
                    {
                        sb.AppendFormat(@"""5"": ""<a href=../Purchases/LpoAmendments.aspx?LpoID={0}>Amendments</a>""", LPOId);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""5"": """"");
                        sb.Append(",");
                    }
                    //string Cancel = data["Cancel"].ToString().Replace("\"", "\\\"");
                    //Cancel = Cancel.Replace("\t", "-");
                    ////if (data["IsVerbalLPO"].ToString().Replace("\"", "\\\"") != "True")
                    ////{
                    //sb.AppendFormat(@"""8"": ""{0}""", Cancel.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(@"""9"": ""{0}""", "");
                    //    sb.Append(",");
                    //}

                    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");
                    Mail = Mail.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    if (Convert.ToBoolean(data["IsLPOCancelled"].ToString()))
                        sb.AppendFormat(@"""7"": ""{0}""", "");
                    else
                    {
                        string AmendEdit = data["AmendEdit"].ToString().Replace("\"", "\\\"");
                        AmendEdit = AmendEdit.Replace("\t", "-");
                        sb.AppendFormat(@"""7"": ""{0}""", AmendEdit.Replace(Environment.NewLine, "\\n"));
                    }
                    sb.Append(",");
                    if (Convert.ToBoolean(data["IsLPOCancelled"].ToString()))
                        sb.AppendFormat(@"""8"": ""{0}""", "");
                    else
                    {
                        string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                        Edit = Edit.Replace("\t", "-");
                        sb.AppendFormat(@"""8"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    }
                    sb.Append(",");
                    //string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("\t", "-");
                    //sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                    if (Convert.ToBoolean(data["IsLPOCancelled"].ToString()))
                        sb.AppendFormat(@"""9"": ""{0}""", "<img src=../images/d1.png alt=Cancellation onclick=AlreadyCancelled();>");
                    else
                    {
                        string Cancel = data["Cancel"].ToString().Replace("\"", "\\\"");
                        Cancel = Cancel.Replace("\t", "-");
                        sb.AppendFormat(@"""9"": ""{0}""", Cancel.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchase/ErrorLog"), "Customer LPO Status WebService", ex.Message.ToString());
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
