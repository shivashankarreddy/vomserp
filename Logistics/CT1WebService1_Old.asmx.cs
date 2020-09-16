using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using BAL;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace VOMS_ERP.Logistics
{
	/// <summary>
	/// Summary description for CT1WebService1
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class CT1WebService1 : System.Web.Services.WebService
	{

		/// <summary>
		/// FOR LOCAL Quotation
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetIOMItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];


				string RefNmbr = HttpContext.Current.Request.Params["sSearch_0"];
				string date = HttpContext.Current.Request.Params["sSearch_1"];
				string CstmrNme = HttpContext.Current.Request.Params["sSearch_2"];
				string SuplrNme = HttpContext.Current.Request.Params["sSearch_3"];
				string Subjet = HttpContext.Current.Request.Params["sSearch_4"];
				string FpoNmbrs = HttpContext.Current.Request.Params["sSearch_5"];
				string LpoNmbrs = HttpContext.Current.Request.Params["sSearch_6"];
				string Stat = HttpContext.Current.Request.Params["sSearch_7"];

				var sb = new StringBuilder();
				StringBuilder s = new StringBuilder();
				if (date != "")
				{
					DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
					DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and IOMDt between '" + FrmDt.ToString("yyyy-MM-dd") + "' and '" + EndDat.ToString("yyyy-MM-dd") + "'");
				}
				if (RefNmbr != "")
					s.Append(" and IOMRefNo LIKE '%" + RefNmbr + "%'");
				if (CstmrNme != "")
					s.Append(" and dbo.GetActiveCustomerName(pr.CustomerID) LIKE '%" + CstmrNme + "%'");
				if (SuplrNme != "")
					s.Append(" and dbo.FN_GetSupplierNm(pr.SupplierID) LIKE '%" + SuplrNme + "%'");
				if (Subjet != "")
					s.Append(" and Subject LIKE '%" + Subjet + "%'");
				if (FpoNmbrs != "")
					s.Append(" and dbo.FN_MergeTableColumnFPO(pr.FPOs) LIKE '%" + FpoNmbrs + "%'");
				if (LpoNmbrs != "")
					s.Append(" and dbo.FN_MergeTableColumnLPO(pr.LPOs) LIKE '%" + LpoNmbrs + "%'");
				if (Stat != "")
					s.Append(" and (case when tp.IOMID in (select pf.IOMRefID from ProformaExciseDetails pf) then dbo.FN_GetStatus(tp.StatusTypeID) else tp.Status end) LIKE '%" + Stat + "%'");


				var filteredWhere = string.Empty;

				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE IOMRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR IOMDt LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CustmNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SuplrNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Subject LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FpoNos LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR LpoNos LIKE ");
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

					orderByClause = orderByClause.Replace("0", ", IOMID ");
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
							declare @MAA TABLE(CreatedDate datetime,IOMID Uniqueidentifier,IOMRefNo varchar(Max),IOMDt datetime,CustmNm nvarchar(MAX),SuplrNm nvarchar(MAX),Subject nvarchar(MAX),
												FpoNos nvarchar(MAX),LpoNos nvarchar(MAX),Status varchar(Max),StatusTypeID bigint, Edit nvarchar(MAX), Delt nvarchar(MAX))
							INSERT
							INTO
								@MAA (CreatedDate,IOMID,IOMRefNo,IOMDt,CustmNm,SuplrNm,Subject,FpoNos,LpoNos,Status,StatusTypeID,Edit,Delt)
									Select tp.CreatedDate,IOMID,IOMRefNo,IOMDt,dbo.GetActiveCustomerName(pr.CustomerID) CustmNm,dbo.FN_GetSupplierNm(pr.SupplierID) SuplrNm,
									 Subject,dbo.FN_MergeTableColumnFPO(pr.FPOs) FpoNos,dbo.FN_MergeTableColumnLPO(pr.LPOs) LpoNos,
									(case when tp.IOMID in (select pf.IOMRefID from ProformaExciseDetails pf)   
									then dbo.FN_GetStatus(tp.StatusTypeID) else tp.Status end) as Status,tp.StatusTypeID, 
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
										+ CONVERT(nvarchar(40), tp.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(40), tp.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delet 
									 FROM [IOMTemplate] tp 
									inner join ProformaRequest pr on tp.ProformaReqID = pr.PInvReqID
									inner join Users u on u.ID = tp.CreatedBy
									Where tp.IsActive <> 0  
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].IOMID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].IOMID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].IOMRefNo
										   ,[@MAA].IOMDt      
										   ,[@MAA].CustmNm
										   ,[@MAA].SuplrNm
										   ,[@MAA].Subject
										   ,[@MAA].FpoNos
										   ,[@MAA].LpoNos
										   ,[@MAA].Status
										   ,[@MAA].StatusTypeID
										   ,[@MAA].EDIT
										   ,[@MAA].Delt
										   ,[@MAA].IOMID
										   
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				string where = "";
				if (new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
				{
					where = "and tp.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
				}
				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + where + " ");

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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["IOMID"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					//sb.AppendFormat(@"""0"": ""{0}""", data["RowNumber"]);
					//sb.Append(",");
					string Status = data["Status"].ToString().Replace("\"", "\\\"");
					string IOMRefNo = data["IOMRefNo"].ToString().Replace("\"", "\\\"");
					string IOMID = data["IOMID"].ToString().Replace("\"", "\\\"");
					string StatusId = data["StatusTypeID"].ToString().Replace("\"", "\\\"");
					IOMRefNo = IOMRefNo.Replace("\t", "-"); // 
					if (Status == "Created" || Status == "Rejected" || Convert.ToInt32(StatusId) == 67)
					{
						sb.AppendFormat(@"""0"": ""{0}""", IOMRefNo.Replace(Environment.NewLine, "\\n")); // New Line
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""0"": ""<a href=CTOneGeneration.aspx?IOMRefID={0}>{1}</a>""", IOMID, IOMRefNo.Replace(Environment.NewLine, "\\n")); // New Line
						sb.Append(",");
					}

					string IOMDt = data["IOMDt"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["IOMDt"]);
					sb.Append(",");

					string CustmNm = data["CustmNm"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""2"": ""{0}""", CustmNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string SuplrNm = data["SuplrNm"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""3"": ""{0}""", SuplrNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Subject = data["Subject"].ToString().Replace("\"", "\\\"");
					Subject = Subject.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", Subject.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					string FpoNos = data["FpoNos"].ToString().Replace("\"", "\\\"");
					FpoNos = FpoNos.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", FpoNos.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string LpoNos = data["LpoNos"].ToString().Replace("\"", "\\\"");
					LpoNos = LpoNos.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", LpoNos.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");




					Status = Status.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", Status.Replace(Environment.NewLine, "\\n"));
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
		public string GetCT1Items()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string CT1DN = HttpContext.Current.Request.Params["sSearch_0"];
				string CT1RN = HttpContext.Current.Request.Params["sSearch_1"];
				string date = HttpContext.Current.Request.Params["sSearch_2"];
				string IOMNo = HttpContext.Current.Request.Params["sSearch_3"];
				string FPONo = HttpContext.Current.Request.Params["sSearch_4"];
				string LPONo = HttpContext.Current.Request.Params["sSearch_5"];
				string supplier = HttpContext.Current.Request.Params["sSearch_6"];
				string status = HttpContext.Current.Request.Params["sSearch_7"];

				StringBuilder s = new StringBuilder();

				if (CT1DN != "")
					s.Append(" and CT1DraftRefNo LIKE '%" + CT1DN.Replace("'", "''") + "%'");
				if (CT1RN != "")
					s.Append(" and CT1ReferenceNo LIKE '%" + CT1RN.Replace("'", "''") + "%'");
				if (date != "")
				{
					date = date.Replace("'", "''");
					DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
					DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and RefDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (IOMNo != "")
					s.Append(" and dbo.FN_GetIOmNm(pf.IOMRefID) + ' :: DT : ' + CONVERT(varchar(12),im.IOMDt,103) LIKE '%" + IOMNo.Replace("'", "''") + "%'");
				if (FPONo != "")
					s.Append(" and dbo.FN_MergeTableColumnFPO_DT(pf.RefFPOs) LIKE '%" + FPONo.Replace("'", "''") + "%'");
				if (LPONo != "")
					s.Append(" and dbo.FN_MergeTableColumnLPO_DT(pf.RefLPOs) LIKE '%" + LPONo.Replace("'", "''") + "%'");
				if (supplier != "")
					s.Append(" and dbo.FN_GetSupplierNm(ct.SupplierID) LIKE '%" + supplier.Replace("'", "''") + "%'");
				if (status != "")
					s.Append(" and ct.Status LIKE '%" + status.Replace("'", "''") + "%'");


				var sb = new StringBuilder();

				var filteredWhere = string.Empty;

				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE CT1DraftRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CT1ReferenceNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR RefDate LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR IOmNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR LPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SupplierNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Status LIKE ");
					sb.Append(wrappedSearch);


					filteredWhere = sb.ToString();
				}


				sb.Clear();

				string orderByClause = string.Empty;

				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{

					orderByClause = orderByClause.Replace("0", ", CT1ID ");

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
							declare @MAA TABLE(CreatedDate datetime,CT1DraftRefNo varchar(Max),CT1ReferenceNo varchar(Max),RefDate datetime,IOmNo nvarchar(MAX),FPOs nvarchar(MAX),
							LPOs nvarchar(MAX), SupplierNm nvarchar(MAX),Status nvarchar(MAX),Reapplicable nvarchar(MAX),Rapply nvarchar(MAX),Edit nvarchar(MAX), Delt nvarchar(MAX),
							CT1ID nvarchar(MAX), IOMID nvarchar(MAX), PInvID nvarchar(MAX))
							INSERT INTO
								@MAA (CreatedDate,CT1DraftRefNo,CT1ReferenceNo,RefDate,IOmNo,FPOs,LPOs,SupplierNm,Status,Reapplicable,Rapply,Edit,Delt,CT1ID, IOMID, PInvID)
									Select ct.CreatedDate,CT1DraftRefNo,CT1ReferenceNo,RefDate,dbo.FN_GetIOmNm(pf.IOMRefID) + ' :: DT : ' + CONVERT(varchar(12),im.IOMDt,103) IOmNo,
									 dbo.FN_MergeTableColumnFPO_DT(pf.RefFPOs) as FPOs,dbo.FN_MergeTableColumnLPO_DT(pf.RefLPOs) as LPOs,
									 dbo.FN_GetSupplierNm(ct.SupplierID) as SupplierNm,ct.Status,
									(case when ct.Status = 'ReApplicable' then 
									'<a href=''javascript:void(0);'' onclick=ReApplicable('''+ CONVERT(nvarchar(max),pf.PInvID) +''','''+ CONVERT(nvarchar(max),pf.IOMRefID) +''','''+ CONVERT(nvarchar(max),ct.CT1ID) +''')>ReApplicable</a>' else '' end ) as Reapplicable, 
									 (case when ct.Status = 'Confirm' then'<img src=../images/d1.png alt=Reapply onclick=Reapply(this,'''+CONVERT(nvarchar(40), 
									pf.IOMRefID)+''','''+CONVERT(nvarchar(40),pf.PInvID)+''') />' else '' end ) Rapply,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(40),pf.IOMRefID)+''','''+CONVERT(nvarchar(40), 
									pf.PInvID)+''','''+CONVERT(nvarchar(40),pf.PrfINVReqID)+''','''
										+ CONVERT(nvarchar(40), ct.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +',''' 
										+CONVERT(nvarchar(40),ct.Status)+''',{5}) />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(40), ct.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delet, 
									''''+CONVERT(nvarchar(40),ct.CT1ID)+'''', ''''+CONVERT(nvarchar(40),  im.IOMID)+'''', ''''+CONVERT(nvarchar(40),   pf.PInvID )+''''
									 FROM [CT1Details] ct inner join ProformaExciseDetails pf on ct.PInvID = pf.PInvID   
									left join IOMTemplate im on im.IOMID = pf.IOMRefID inner join Users u on u.ID = ct.CreatedBy 
									{4}                   

							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM
									 (SELECT (SELECT count([@MAA].CT1ID) FROM @MAA) AS TotalRows, 
									( SELECT  count( [@MAA].CT1ID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].CT1DraftRefNo
										   ,[@MAA].CT1ReferenceNo      
										   ,[@MAA].RefDate
										   ,[@MAA].IOmNo
										   ,[@MAA].FPOs
										   ,[@MAA].LPOs
										   ,[@MAA].SupplierNm
										   ,[@MAA].Status
										   ,[@MAA].Reapplicable,[@MAA].Rapply
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].CT1ID
										   ,[@MAA].IOMID
										   ,[@MAA].PInvID
										   
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				string where = "";
				string where1 = "";
				string AccessRole = HttpContext.Current.Session["AccessRole"].ToString();
				if (new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
				{
					where1 = " and ct.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
				}
				if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
				{
					string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
					if (Mode == "Etdt")
					{
						if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								"where CONVERT(date,ct.CreatedDate,103)= CONVERT(date,GETDATE(),103) " + where1 + where :
								"where CONVERT(date,ct.CreatedDate,103)= CONVERT(date,GETDATE(),103) " + s.ToString() + where1 + where);
						}
						else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
							(CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								"where  pf.CreatedBy = " + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(date,ct.CreatedDate,103)= CONVERT(date,GETDATE(),103) " + where1 :
								"where  pf.CreatedBy = " + "'" + Session["UserID"].ToString() + "'" + "and CONVERT(date,ct.CreatedDate,103)= CONVERT(date,GETDATE(),103)" + s.ToString() + where1 + where);
						}
					}
					else if (Mode == "ECtldt")
					{
						if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + where1 + " order by CT1ID DESC ");
						}
						else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
							(CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								" where  pf.CreatedBy = " + "'" + Session["UserID"].ToString() + "'" + where1 + " order by CT1ID DESC " :
								" where  pf.CreatedBy = " + "'" + Session["UserID"].ToString() + "'" + s.ToString() + where1 + where);
						}
					}
					else
					{
						query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + where1 + " order by CT1ID DESC ", "''" + AccessRole + "''");
					}
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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CT1ID"].ToString());
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					string CT1DraftRefNo = data["CT1DraftRefNo"].ToString().Replace("\"", "\\\"");
					string CT1ID = data["CT1ID"].ToString().Replace("\"", "\\\"");
					CT1DraftRefNo = CT1DraftRefNo.Replace("\t", "-");
					string CT1ReferenceNo = data["CT1ReferenceNo"].ToString().Replace("\"", "\\\"");
					CT1ReferenceNo = CT1ReferenceNo.Replace("\t", "-");
					if (CT1ReferenceNo == "")
					{
						sb.AppendFormat(@"""0"": ""<a href=CTOneDetails.aspx?ID={0}>{1}</a>""", CT1ID, CT1DraftRefNo.Replace(Environment.NewLine, "\\n")); // New Line
						sb.Append(",");

					}
					else
					{
						sb.AppendFormat(@"""0"": ""{0}""", CT1DraftRefNo.Replace(Environment.NewLine, "\\n")); // New Line
						sb.Append(",");
					}
					sb.AppendFormat(@"""1"": ""<a href=CTOneDetails.aspx?ID={0}>{1}</a>""", CT1ID, CT1ReferenceNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");
					string RefDate = data["RefDate"].ToString().Replace("\"", "\\\"");
					string em = "";
					if (Convert.ToDateTime(RefDate) == CommonBLL.EndDate || Convert.ToDateTime(RefDate) == CommonBLL.EndDate1)
					{
						sb.AppendFormat(@"""2"": ""{0}""", em);
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["RefDate"]);
						sb.Append(",");
					}
					string IOmNo = data["IOmNo"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""3"": ""{0}""", IOmNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string FPOs = data["FPOs"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""4"": ""{0}""", FPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string LPOs = data["LPOs"].ToString().Replace("\"", "\\\"");
					LPOs = LPOs.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", LPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string SupplierNm = data["SupplierNm"].ToString().Replace("\"", "\\\"");
					SupplierNm = SupplierNm.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", SupplierNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string Status = data["Status"].ToString().Replace("\"", "\\\"");
					Status = Status.Replace("\t", "-");
					//if (data["IOMID"].ToString() == "")
					//    data["IOMID"].ToString() = "";
					if (Status == "ReApplicable")
					{
						sb.AppendFormat(@"""7"": ""{0}""", data["Reapplicable"].ToString().Replace("\"", "\\\""));
					}
					else
						sb.AppendFormat(@"""7"": ""{0}""", Status.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string Rapply = data["Rapply"].ToString().Replace("\"", "\\\"");
					Rapply = Rapply.Replace("\t", "-");
					sb.AppendFormat(@"""8"": ""{0}""", Rapply.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
					Edit = Edit.Replace("\t", "-");
					Edit = Edit.Replace(Environment.NewLine, "\\n");
					sb.AppendFormat(@"""9"": ""{0}""", Edit);
					sb.Append(",");
					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					Del = Del.Replace("\t", "-");
					sb.AppendFormat(@"""10"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
		public string GetSevItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string SevttamDftNo = HttpContext.Current.Request.Params["sSearch_0"];
				string Type1 = HttpContext.Current.Request.Params["sSearch_1"];
				string SevttamRefNo = HttpContext.Current.Request.Params["sSearch_2"];
				string CT1DftRfNo = HttpContext.Current.Request.Params["sSearch_3"];
				string CT1RfNo = HttpContext.Current.Request.Params["sSearch_4"];
				string CT1RfDt = HttpContext.Current.Request.Params["sSearch_5"];

				StringBuilder s = new StringBuilder();

				if (SevttamDftNo != "")
					s.Append(" and SVM.SevottamDraftRefNo LIKE '%" + SevttamDftNo.Replace("'", "''") + "%'");
				if (Type1 != "")
					s.Append(" and SVM.Type LIKE '%" + Type1.Replace("'", "''") + "%'");
				if (CT1RfDt != "")
				{
					CT1RfDt = CT1RfDt.Replace("'", "''");
					DateTime FrmDt = CT1RfDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(CT1RfDt.Split('~')[0].ToString());
					DateTime EndDat = CT1RfDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(CT1RfDt.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and dbo.FN_CT1RefDtt(CT1ID) between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (SevttamRefNo != "")
					s.Append(" and SVM.SevottamRefNo LIKE '%" + SevttamRefNo.Replace("'", "''") + "%'");
				if (CT1DftRfNo != "")
					s.Append(" and dbo.FN_GetCTOneDraftNos_Sevottam(SVM.SevID) LIKE '%" + CT1DftRfNo.Replace("'", "''") + "%'");
				if (CT1RfNo != "")
					s.Append(" and dbo.FN_GetCTOneRefNos_Sevottam(SVM.SevID) LIKE '%" + CT1RfNo.Replace("'", "''") + "%'");


				var sb = new StringBuilder();
				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE Type LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SevottamRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SevottamDraftRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CT1DraftRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CT1RefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR RefDt LIKE ");
					sb.Append(wrappedSearch);
					filteredWhere = sb.ToString();
				}

				sb.Clear();
				string orderByClause = string.Empty;
				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", SevID ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "ID ASC";
				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();

				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

				string query = @"  
							declare @MAA TABLE(CreatedDate datetime,SevID uniqueidentifier,Type varchar(Max),RefDt datetime,SevottamRefNo nvarchar(MAX),SevottamDraftRefNo nvarchar(MAX),CT1DraftRefNo nvarchar(MAX),
												CT1RefNo nvarchar(MAX),Edit nvarchar(MAX), Delt nvarchar(MAX))
							INSERT
							INTO
								@MAA (CreatedDate,SevID,Type,SevottamRefNo,SevottamDraftRefNo,CT1DraftRefNo,CT1RefNo,RefDt,Edit,Delt)
									Select SVM.CreatedDate,SVM.SevID, SVM.Type, SVM.SevottamRefNo, SVM.SevottamDraftRefNo, 
									dbo.FN_GetCTOneDraftNos_Sevottam(SVM.SevID) AS CT1DraftRefNo, 
									dbo.FN_GetCTOneRefNos_Sevottam(SVM.SevID) as CT1RefNo, dbo.FN_CT1RefDtt(SCD.CT1ID) AS RefDt ,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+SVM.Type+''','''
										+ CONVERT(nvarchar(max), SVM.CreatedBy) +''','+(case when u.ContactType = 'trafficer' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''+SVM.Type+''','''
										+ CONVERT(nvarchar(max), SVM.CreatedBy) +''','+(case when u.ContactType = 'trafficer' then '1' else '0' end) +') />' Delet 
									 FROM Sevottam_Search SVM
									INNER JOIN SevottamCT1Details SCD ON SVM.SevID = SCD.SevID
									inner join Users u on u.ID = SVM.CreatedBy
									{4}
									UNION
									SELECT SVM.CreatedDate,SVM.SevID, CASE WHEN SVM.Type = 'UnUsed' THEN 'POE/UnUsed' ELSE SVM.Type END AS Type, 
									SVM.SevottamRefNo, SVM.SevottamDraftRefNo, 
									dbo.FN_CT1DrftNmbr(SPE.CT1ID) AS CT1DraftRefNo, dbo.FN_CT1Nmbr(SPE.CT1ID) as CT1RefNo, SPE.CT1Date,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+SVM.Type+''','''
										+ CONVERT(nvarchar(max), SVM.CreatedBy) +''','+(case when u.ContactType = 'trafficer' then '1' else '0' end) +','''+CONVERT(varchar,Status)+''') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''+SVM.Type+''','''
										+ CONVERT(nvarchar(max), SVM.CreatedBy) +''','+(case when u.ContactType = 'trafficer' then '1' else '0' end) +') />' Delet 
									FROM Sevottam_Search SVM INNER JOIN 
									SevottamPOE SPE ON SVM.SevID = SPE.SevottamID 
									inner join Users u on u.ID = SVM.CreatedBy 
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].SevID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].SevID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].SevID
										   ,[@MAA].Type      
										   ,[@MAA].SevottamRefNo
										   ,[@MAA].SevottamDraftRefNo
										   ,[@MAA].CT1DraftRefNo
										   ,[@MAA].CT1RefNo
										   ,[@MAA].RefDt
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				string where = "";
				if (new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
					where = "and svm.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, "where SVM.IsActive <> 0" + where + s.ToString());

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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SevID"].ToString());
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");

					string Type = data["Type"].ToString().Replace("\"", "\\\"");
					string SevottamDraftRefNo = data["SevottamDraftRefNo"].ToString().Replace("\"", "\\\"");

					string SevID = data["SevID"].ToString().Replace("\"", "\\\"");
					SevottamDraftRefNo = SevottamDraftRefNo.Replace("\t", "-");
					sb.AppendFormat(@"""0"": ""<a href=../Logistics/SevottamDetails.Aspx?ID={0}&Type={1}>{2}</a>""", SevID, Type, SevottamDraftRefNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0}""", Type.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string SevottamRefNo = data["SevottamRefNo"].ToString().Replace("\"", "\\\"");
					SevottamRefNo = SevottamRefNo.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""<a href=SevottamCTOne.Aspx?ID={0}&Type={1}>{2}</a>""", SevID, Type, SevottamRefNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string CT1DraftRefNo = data["CT1DraftRefNo"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""3"": ""{0}""", CT1DraftRefNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string CT1RefNo = data["CT1RefNo"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""4"": ""{0}""", CT1RefNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string RefDt = data["RefDt"].ToString().Replace("\"", "\\\"");
					RefDt = RefDt.Replace("\t", "-");
					string em = "";
					if (Convert.ToDateTime(RefDt) == CommonBLL.EndDate || Convert.ToDateTime(RefDt) == CommonBLL.EndDate1)
					{
						sb.AppendFormat(@"""5"": ""{0}""", em);
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["RefDt"]);
						sb.Append(",");
					}

					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
					Edit = Edit.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					Del = Del.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
				return "";
			}
		}


		/// <summary>
		/// FOR GDN
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetGDNItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string DisInsNo = HttpContext.Current.Request.Params["sSearch_0"];
				string DisDt = HttpContext.Current.Request.Params["sSearch_1"];
				string FPO = HttpContext.Current.Request.Params["sSearch_2"];
				string LPO = HttpContext.Current.Request.Params["sSearch_3"];
				string Suplr = HttpContext.Current.Request.Params["sSearch_4"];
				string InvcNo = HttpContext.Current.Request.Params["sSearch_5"];
				string InvDt = HttpContext.Current.Request.Params["sSearch_6"];
				string Refgdn = HttpContext.Current.Request.Params["sSearch_7"];
				string status = HttpContext.Current.Request.Params["sSearch_8"];

				StringBuilder s = new StringBuilder();

				if (DisInsNo != "")
					s.Append(" and dbo.FN_DsptchInstNmbr(DspchInstn) LIKE '%" + DisInsNo.Replace("'", "''") + "%'");

				if (DisDt != "")
				{
					DisDt = DisDt.Replace("'", "''");
					DateTime FrmDt = DisDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(DisDt.Split('~')[0].ToString());
					DateTime EndDat = DisDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(DisDt.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and DspchDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (FPO != "")
					s.Append(" and dbo.FN_MergeTableColumnFPO(FPOs) LIKE '%" + FPO.Replace("'", "''") + "%'");

				if (LPO != "")
					s.Append(" and dbo.FN_MergeTableColumnLPO(LPOs) LIKE '%" + LPO.Replace("'", "''") + "%'");
				if (Suplr != "")
					s.Append(" and dbo.FN_GetSupplierNm(SuplrID) LIKE '%" + Suplr.Replace("'", "''") + "%'");
				if (InvcNo != "")
					s.Append(" and InvoiceNo LIKE '%" + InvcNo.Replace("'", "''") + "%'");
				if (InvDt != "")
				{
					InvDt = InvDt.Replace("'", "''");
					DateTime FrmDt = InvDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(InvDt.Split('~')[0].ToString());
					DateTime EndDat = InvDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(InvDt.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and InvoiceDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (Refgdn != "")
					s.Append(" and RefGDN LIKE '%" + Refgdn.Replace("'", "''") + "%'");
				if (status != "")
					s.Append(" and IsApproved LIKE '%" + status.Replace("'", "''") + "%'");

				var sb = new StringBuilder();
				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE DspchNmbr LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR DspchDt LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR LPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SuplrNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR InvoiceNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR InvoiceDt LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR RefGDN LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR IsApproved LIKE ");
					sb.Append(wrappedSearch);
					filteredWhere = sb.ToString();
				}

				sb.Clear();

				string orderByClause = string.Empty;
				orderByClause = "0 DESC";
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
				string query = @"declare @MAA TABLE(CreatedDate Datetime,ID Uniqueidentifier,DspchNmbr varchar(Max),DspchDt date,FPOs nvarchar(max),LPOs nvarchar(MAX),SuplrNm nvarchar(MAX), 
									InvoiceNo nvarchar(MAX), InvoiceDt date,RefGDN nvarchar(MAX),IsApproved nvarchar(MAX), CreatedMail nvarchar(MAX), 
									Edit nvarchar(MAX), Delt nvarchar(MAX)) 
									INSERT INTO @MAA (CreatedDate,ID,DspchNmbr,DspchDt,FPOs,LPOs,SuplrNm,InvoiceNo,InvoiceDt,RefGDN,IsApproved,  CreatedMail,Edit, Delt)
									Select  gd.CreatedDate,gd.ID,dbo.FN_DsptchInstNmbr(DspchInstn) DspchNmbr,DspchDt,dbo.FN_MergeTableColumnFPO(FPOs) fponmbrs, 
									dbo.FN_MergeTableColumnLPO(LPOs) lponmbrs,dbo.FN_GetSupplierNm(SuplrID) SuplrNm,InvoiceNo, InvoiceDt, 
									RefGDN,IsApproved, u.PriEmail, '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+ IsApproved +''','''
										+ CONVERT(nvarchar(40), gd.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(40), gd.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delet 
									 FROM [Gdn] gd inner join Users u on u.ID = gd.CreatedBy and gd.IsActive <> 0 {4}                   
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, ( SELECT  count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, 
								[@MAA].CreatedDate,[@MAA].ID, [@MAA].DspchNmbr, [@MAA].DspchDt, [@MAA].FPOs, [@MAA].LPOs, [@MAA].SuplrNm, [@MAA].InvoiceNo, [@MAA].InvoiceDt, 
								[@MAA].RefGDN, [@MAA].IsApproved, [@MAA].CreatedMail, [@MAA].Edit, [@MAA].Delt FROM @MAA {1}) RawResults) Results  
								WHERE RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				string where = "  gd.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + " and " + where + " order by ID DESC ");

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
					string DspchNmbr = data["DspchNmbr"].ToString().Replace("\"", "\\\"");
					string ID = data["ID"].ToString().Replace("\"", "\\\"");
					DspchNmbr = DspchNmbr.Replace("\t", "-");
					sb.AppendFormat(@"""0"": ""{0}""", DspchNmbr.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string DspchDt = data["DspchDt"].ToString().Replace("\"", "\\\"");
					DspchDt = DspchDt.Replace("\t", "-");
					string em = "";
					if (Convert.ToDateTime(DspchDt) == CommonBLL.EndDate || Convert.ToDateTime(DspchDt) == CommonBLL.EndDate1)
					{
						sb.AppendFormat(@"""1"": ""{0}""", em);
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["DspchDt"]);
						sb.Append(",");
					}
					string FPOs = data["FPOs"].ToString().Replace("\"", "\\\"");
					FPOs = FPOs.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", FPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string LPOs = data["LPOs"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""3"": ""{0}""", LPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string SuplrNm = data["SuplrNm"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""4"": ""{0}""", SuplrNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string InvoiceNo = data["InvoiceNo"].ToString().Replace("\"", "\\\"");
					InvoiceNo = InvoiceNo.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", InvoiceNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string InvoiceDt = data["InvoiceDt"].ToString().Replace("\"", "\\\"");
					InvoiceDt = InvoiceDt.Replace("\t", "-");
					string emm = "";
					if (Convert.ToDateTime(InvoiceDt) == CommonBLL.EndDate || Convert.ToDateTime(InvoiceDt) == CommonBLL.EndDate1)
					{
						sb.AppendFormat(@"""6"": ""{0}""", emm);
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""6"": ""{0:dd/MM/yyyy}""", data["InvoiceDt"]);
						sb.Append(",");
					}

					string RefGDN = data["RefGDN"].ToString().Replace("\"", "\\\"");
					string IsApproved = data["IsApproved"].ToString().Replace("\"", "\\\"");
					string CreatedMail_ID = "&#39;" + data["CreatedMail"].ToString().Replace("\"", "\\\"") + "&#39;";
					IsApproved = IsApproved.Replace("\t", "-");
					RefGDN = RefGDN.Replace("\t", "-");
					if (IsApproved == "Approved")
						sb.AppendFormat(@"""7"": ""<a href=../Logistics/GdnDetails.aspx?ID={1} title='Continue with GRN...'>{0}</a>""", RefGDN.Replace(Environment.NewLine, "\\n"), ID);
					else
						sb.AppendFormat(@"""7"": ""{0}""", RefGDN.Replace(Environment.NewLine, "\\n"), ID);
					sb.Append(",");

					if (IsApproved == "Approved")
						sb.AppendFormat(@"""8"": ""{0}""", IsApproved.Replace(Environment.NewLine, "\\n"));
					else
					{
						if (CommonBLL.AdminRole == (Session["AccessRole"].ToString()))
							sb.AppendFormat(@"""8"": ""<a href='javascript:;' title='Not Approved, Please Approve it and Continue...' onclick='javascript:UpdateStatus(this, {1})'>{0}</a>""", IsApproved.Replace(Environment.NewLine, "\\n"), CreatedMail_ID);
						else
							sb.AppendFormat(@"""8"": ""{0}""", IsApproved.Replace(Environment.NewLine, "\\n"));
					}
					sb.Append(",");

					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
					Edit = Edit.Replace("\t", "-");
					sb.AppendFormat(@"""9"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					Del = Del.Replace("\t", "-");
					sb.AppendFormat(@"""10"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
		public string GetGRNItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				var sb = new StringBuilder();

				string GRNNo = HttpContext.Current.Request.Params["sSearch_0"];
				string RefNmbr = HttpContext.Current.Request.Params["sSearch_1"];
				string RecvfrmDte = HttpContext.Current.Request.Params["sSearch_2"];
				string FPONm = HttpContext.Current.Request.Params["sSearch_3"];
				string LPONm = HttpContext.Current.Request.Params["sSearch_4"];
				string SuppNm = HttpContext.Current.Request.Params["sSearch_5"];
				string InvNm = HttpContext.Current.Request.Params["sSearch_6"];
				string InvfmDt = HttpContext.Current.Request.Params["sSearch_7"];
				StringBuilder s = new StringBuilder();

				if (GRNNo != "")
					s.Append(" and RefGRN LIKE '%" + GRNNo.Replace("'", "''") + "%'");
				if (RefNmbr != "")
					s.Append(" and (Case When ((ISNULL(DspchInstID, '" + Guid.Empty.ToString() + "') = '" + Guid.Empty.ToString() + "') and (ISNULL(GdnID, '" + Guid.Empty.ToString() + "') = '" + Guid.Empty.ToString() + "')) Then" +
									"(RefGRN) else (Case When (ISNULL(DspchInstID, '" + Guid.Empty.ToString() + "') != '" + Guid.Empty.ToString() + "') then " +
									"dbo.FN_DsptchInstNmbr(DspchInstID) Else (Select RefGDN From Gdn gd Where gd.ID= grn.GdnID) End) end) LIKE '%" + RefNmbr.Replace("'", "''") + "%'");
				if (RecvfrmDte != "")
				{
					RecvfrmDte = RecvfrmDte.Replace("'", "''");
					DateTime FrmDt = RecvfrmDte.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RecvfrmDte.Split('~')[0].ToString());
					DateTime EndDat = RecvfrmDte.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RecvfrmDte.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and ReceivedDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (FPONm != "")
					s.Append(" and dbo.FN_MergeTableColumnFPO(FPOs) LIKE '%" + FPONm.Replace("'", "''") + "%'");
				if (LPONm != "")
					s.Append(" and dbo.FN_MergeTableColumnLPO(LPOs) LIKE '%" + LPONm.Replace("'", "''") + "%'");
				if (SuppNm != "")
					s.Append(" and dbo.FN_GetSupplierNm(SuplrID) LIKE '%" + SuppNm.Replace("'", "''") + "%'");
				if (InvNm != "")
					s.Append(" and InvoiceNo LIKE '%" + InvNm.Replace("'", "''") + "%'");
				if (InvfmDt != "")
				{
					InvfmDt = InvfmDt.Replace("'", "''");
					DateTime InvFrmDt = InvfmDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(InvfmDt.Split('~')[0].ToString());
					DateTime InvToDt = InvfmDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(InvfmDt.Split('~')[1].ToString());
					if (InvFrmDt.ToShortDateString() != "1/1/0001" && InvToDt.ToString() != "1/1/0001")
						s.Append(" and InvoiceDt between '" + InvFrmDt.ToString("MM/dd/yyyy") + "' and '" + InvToDt.ToString("MM/dd/yyyy") + "'");
				}

				var filteredWhere = string.Empty;

				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE RefGRN LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR RefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ReceivedDt LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR LPOs LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SuplrNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR InvoiceNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR InvoiceDt LIKE ");
					sb.Append(wrappedSearch);

					filteredWhere = sb.ToString();
				}
				sb.Clear();
				string orderByClause = string.Empty;
				orderByClause = "0 DESC";
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

				string query = @"  Declare @EmptyGuid Uniqueidentifier = CAST(CAST(0 as Binary) as Uniqueidentifier) 
								declare @MAA TABLE(CreatedDate datetime,ID Uniqueidentifier,DspchInstID Uniqueidentifier,GdnID Uniqueidentifier,RefGRN varchar(Max), 
								RefNo varchar(Max),ReceivedDt datetime, FPOs nvarchar(max),LPOs nvarchar(MAX),SuplrNm nvarchar(MAX),InvoiceNo nvarchar(MAX), 
								InvoiceDt datetime,Edit nvarchar(MAX), Delt nvarchar(MAX)) 
								INSERT INTO @MAA (CreatedDate,ID,DspchInstID,GdnID,RefGRN,RefNo,ReceivedDt,FPOs,LPOs,SuplrNm,InvoiceNo,InvoiceDt,Edit, Delt)
									Select  grn.CreatedDate,grn.ID,DspchInstID,GdnID,RefGRN,(Case When ((ISNULL(DspchInstID, @EmptyGuid) = @EmptyGuid) and 
									(ISNULL(GdnID, @EmptyGuid) = @EmptyGuid)) Then      
									(RefGRN) else (Case When (ISNULL(DspchInstID, @EmptyGuid) != @EmptyGuid) then     
									dbo.FN_DsptchInstNmbr(DspchInstID) Else (Select RefGDN From Gdn gd Where gd.ID= grn.GdnID) End) end)     
									RefNo,ReceivedDt,dbo.FN_MergeTableColumnFPO(FPOs) fponmbrs
									,dbo.FN_MergeTableColumnLPO(LPOs) lponmbrs,dbo.FN_GetSupplierNm(SuplrID) SuplrNm,InvoiceNo,InvoiceDt,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
										+ CONVERT(nvarchar(40), grn.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(40), grn.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delet 
									 FROM [Grn] grn inner join Users u on u.ID = grn.CreatedBy  and grn.IsActive <> 0
									{4}                   
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, *
								 FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
								( SELECT  count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CreatedDate,  [@MAA].ID,[@MAA].DspchInstID, [@MAA].GdnID, 
								[@MAA].RefGRN, [@MAA].RefNo,[@MAA].ReceivedDt, [@MAA].FPOs, [@MAA].LPOs, [@MAA].SuplrNm, [@MAA].InvoiceNo, 
								[@MAA].InvoiceDt, [@MAA].Edit, [@MAA].Delt FROM @MAA {1}) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				string where = "  grn.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'";
				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + " and " + where);

				//query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + " order by ID DESC ");

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
					string RefNo = data["RefNo"].ToString().Replace("\"", "\\\"");
					string RefGRN = data["RefGRN"].ToString().Replace("\"", "\\\"");
					string ID = data["ID"].ToString().Replace("\"", "\\\"");
					RefGRN = RefGRN.Replace("\t", "-"); // 
					sb.AppendFormat(@"""0"": ""<a href=../Logistics/GrnDetails.aspx?ID={0}>{1}</a>""", ID, RefGRN.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0}""", RefNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string ReceivedDt = data["ReceivedDt"].ToString().Replace("\"", "\\\"");
					ReceivedDt = ReceivedDt.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["ReceivedDt"]);
					sb.Append(",");

					string FPOs = data["FPOs"].ToString().Replace("\"", "\\\"");
					FPOs = FPOs.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", FPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string LPOs = data["LPOs"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""4"": ""{0}""", LPOs.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string SuplrNm = data["SuplrNm"].ToString().Replace("\"", "\\\"");
					sb.AppendFormat(@"""5"": ""{0}""", SuplrNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string InvoiceNo = data["InvoiceNo"].ToString().Replace("\"", "\\\"");
					InvoiceNo = InvoiceNo.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", InvoiceNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string InvoiceDt = data["InvoiceDt"].ToString().Replace("\"", "\\\"");
					InvoiceDt = InvoiceDt.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0:dd-MM-yyyy}""", data["InvoiceDt"]);
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
		public string GetDISPItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string DisptInstNo = HttpContext.Current.Request.Params["sSearch_0"];
				string CT1RfNo = HttpContext.Current.Request.Params["sSearch_1"];
				string ContctPersns = HttpContext.Current.Request.Params["sSearch_2"];
				string ContctNum = HttpContext.Current.Request.Params["sSearch_3"];
				string ShippingAdd = HttpContext.Current.Request.Params["sSearch_4"];
				string SupplierNm = HttpContext.Current.Request.Params["sSearch_5"];

				StringBuilder s = new StringBuilder();

				if (DisptInstNo != "")
					s.Append(" and RefNo LIKE '%" + DisptInstNo.Replace("'", "''") + "%'");
				if (CT1RfNo != "")
					s.Append(" and dbo.FN_MergeTableColumnCT1(di.CT1ID) LIKE '%" + CT1RfNo.Replace("'", "''") + "%'");
				if (SupplierNm != "")
					s.Append(" and dbo.FN_GetSupplierNm(di.SupplierID) LIKE '%" + SupplierNm.Replace("'", "''") + "%'");
				if (ContctPersns != "")
				{
					s.Append(" and (ContactPersonName LIKE '%" + ContctPersns.Replace("'", "''") + "%'");
					s.Append(" or AlternativePersonName LIKE '%" + ContctPersns.Replace("'", "''") + "%')");
				}
				if (ContctNum != "")
				{
					s.Append(" and (ContactNumber LIKE '%" + ContctNum.Replace("'", "''") + "%'");
					s.Append(" or AlternativeContactNumber LIKE '%" + ContctNum.Replace("'", "''") + "%')");
				}
				if (ShippingAdd != "")
					s.Append(" and ShippingAddress LIKE '%" + ShippingAdd.Replace("'", "''") + "%'");


				var sb = new StringBuilder();

				var filteredWhere = string.Empty;

				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE RefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CT1ReferenceNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ContactPersonName LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR AlternativePersonName LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ContactNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR AlternativeContactNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ShippingAddress LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SuplrNm LIKE ");
					sb.Append(wrappedSearch);

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

				//Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
				//, Mail, Edit, Delt

				string query = @"  
							declare @MAA TABLE(CreatedDate datetime,DInsID uniqueidentifier,RefNo varchar(Max),CT1ReferenceNo varchar(Max),ContactPersonName varchar(Max),AlternativePersonName varchar(Max),AlternativeContactNumber varchar(Max),
												ContactNumber nvarchar(max),ShippingAddress nvarchar(MAX),SuplrNm nvarchar(MAX),Mail nvarchar(MAX),Edit nvarchar(MAX), Delt nvarchar(MAX), CompanyId uniqueidentifier)
							INSERT
							INTO
								@MAA (CreatedDate,DInsID,RefNo,CT1ReferenceNo,ContactPersonName,AlternativePersonName,ContactNumber,AlternativeContactNumber,ShippingAddress,SuplrNm,Mail,Edit, Delt,CompanyId)
									Select  di.CreatedDate,DInsID,RefNo,dbo.FN_MergeTableColumnCT1(di.CT1ID) CT1ReferenceNo,ContactPersonName,AlternativePersonName,ContactNumber,AlternativeContactNumber,ShippingAddress
									,dbo.FN_GetSupplierNm(di.SupplierID) SuplrNm,'<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
										+ CONVERT(varchar(40), di.CreatedBy) +''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(varchar(40), di.CreatedBy) +''','+ (case when u.ContactType = 'Customer' then '1' else '0' end) +','''+ Convert(nvarchar(max),di.CompanyId)+''') />' Delet, di.CompanyId 
									 FROM [DespatchInstructions] di 
									inner join Users u on u.ID = di.CreatedBy
									and di.IsActive <> 0
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].DInsID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].DInsID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].DInsID
										   ,[@MAA].RefNo      
										   ,[@MAA].CT1ReferenceNo
										   ,[@MAA].ContactPersonName
										   ,[@MAA].AlternativePersonName
										   ,[@MAA].ContactNumber
										   ,[@MAA].AlternativeContactNumber
										   ,[@MAA].ShippingAddress
										   ,[@MAA].SuplrNm
										   ,[@MAA].Mail
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   ,[@MAA].CompanyId
										   
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

				Guid CompanyID = new Guid(Session["CompanyID"].ToString());

				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + " and di.CompanyId =" + "'" + CompanyID + "'" + " ");

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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["DInsID"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");

					string CT1ReferenceNo = data["CT1ReferenceNo"].ToString().Replace("\"", "\\\"");
					string RefNo = data["RefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
					string DInsID = data["DInsID"].ToString().Replace("\"", "\\\"");
					//ItemDescription = ItemDescription.Replace("'", "&#39;"); // Single Quote
					//ItemDescription = ItemDescription.Replace(@"\","-"); //<a href="http://www.google.com/">second line</a>
					RefNo = RefNo.Replace("\t", "-"); // 
					sb.AppendFormat(@"""0"": ""<a href=../Logistics/DspchInstRpt.aspx?ID={0}>{1}</a>""", DInsID, RefNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0}""", CT1ReferenceNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");
					string ContactPersonName = data["ContactPersonName"].ToString().Replace("\"", "\\\"");
					string AlternativePersonName = data["AlternativePersonName"].ToString().Replace("\"", "\\\"");
					//PartNo = PartNo.Replace("'", "&#39;");
					//PartNo = PartNo.Replace(@"\", "-");
					//RecDt = RecDt.Replace("\t", "-");<a href=../Logistics/SevottamCTOne.Aspx?ID={0}&Type={1}>{2}</a>
					ContactPersonName = ContactPersonName.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}/{1}""", ContactPersonName.Replace(Environment.NewLine, "\\n"), AlternativePersonName.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					//PartNo = PartNo.Replace("'", "&#39;");
					//PartNo = PartNo.Replace(@"\", "-");
					//RecDt = RecDt.Replace("\t", "-");<a href=../Logistics/SevottamCTOne.Aspx?ID={0}&Type={1}>{2}</a>
					//AlternativePersonName = AlternativePersonName.Replace("\t", "-");
					//sb.AppendFormat(@"""4"": ""{0}""", AlternativePersonName.Replace(Environment.NewLine, "\\n"));
					//sb.Append(",");

					string ContactNumber = data["ContactNumber"].ToString().Replace("\"", "\\\"");
					string AlternativeContactNumber = data["AlternativeContactNumber"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					//Subjt = Subjt.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}/{1}""", ContactNumber.Replace(Environment.NewLine, "\\n"), AlternativeContactNumber.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					//AlternativeContactNumber = AlternativeContactNumber.Replace("\t", "-");
					//sb.AppendFormat(@"""5"": ""{0}""", AlternativeContactNumber.Replace(Environment.NewLine, "\\n"));
					//sb.Append(",");

					string ShippingAddress = data["ShippingAddress"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					ShippingAddress = ShippingAddress.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", ShippingAddress.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string SuplrNm = data["SuplrNm"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					//StatIDd = StatIDd.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", SuplrNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					//string SupplierNm = data["SupplierNm"].ToString().Replace("\"", "\\\"");
					////string FPOID = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
					////string CutID = data["CusmorId"].ToString().Replace("\"", "\\\"");
					////string SuppID = data["SupplierId"].ToString().Replace("\"", "\\\"");
					////Spec = Spec.Replace("'", "&#39;");
					////Spec = Spec.Replace(@"\","-");
					//SupplierNm = SupplierNm.Replace("\t", "-");
					//sb.AppendFormat(@"""7"": ""{0}""", SupplierNm.Replace(Environment.NewLine, "\\n"));
					//sb.Append(",");

					string Mail = data["Mail"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					Mail = Mail.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0:mm-dd-yyyy}""", Mail.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					Edit = Edit.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
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
		public string GetIOMLOGItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string RefNo = HttpContext.Current.Request.Params["sSearch_0"];
				string IOMDt = HttpContext.Current.Request.Params["sSearch_1"];
				string Cust = HttpContext.Current.Request.Params["sSearch_2"];
				string Sub = HttpContext.Current.Request.Params["sSearch_3"];
				string FPO = HttpContext.Current.Request.Params["sSearch_4"];
				string LPO = HttpContext.Current.Request.Params["sSearch_5"];
				string Status = HttpContext.Current.Request.Params["sSearch_6"];

				StringBuilder s = new StringBuilder();

				if (RefNo != "")
					s.Append(" and IOMRefNo LIKE '%" + RefNo + "%'");
				if (IOMDt != "")
				{
					DateTime FrmDt = IOMDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(IOMDt.Split('~')[0].ToString());
					DateTime EndDat = IOMDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(IOMDt.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and IOMDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (Cust != "")
					s.Append(" and dbo.FN_MergeCustShortName(cl.CustomerID) LIKE '%" + Cust + "%'");
				if (Sub != "")
					s.Append(" and Subject LIKE '%" + Sub + "%'");
				if (FPO != "")
					s.Append(" and dbo.FN_MergeTableColumnFPO(tp.Fpos) LIKE '%" + FPO + "%'");
				if (LPO != "")
					s.Append(" and dbo.FN_MergeTableColumnLPO(tp.Lpos) LIKE '%" + LPO + "%'");
				if (Status != "")
					s.Append(" and tp.Status LIKE '%" + Status + "%'");

				s.Append(" and tp.CompanyId = '" + Session["CompanyID"].ToString() + "'");

				var sb = new StringBuilder();

				var filteredWhere = string.Empty;

				var wrappedSearch = "'%" + rawSearch + "%'";

				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE IOMRefNo LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CustmNm LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Subject LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FpoNos LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR LpoNos LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Status LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR IOMDt LIKE ");
					sb.Append(wrappedSearch);


					filteredWhere = sb.ToString();
				}


				////ORDERING

				sb.Clear();

				string orderByClause = string.Empty;

				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{

					orderByClause = orderByClause.Replace("0", ", IOM2ID ");
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
							declare @MAA TABLE(CreatedDate Datetime,IOM2ID uniqueidentifier,IOMRefNo varchar(Max),CustmNm varchar(Max),Subject varchar(Max),FpoNos varchar(Max),
												LpoNos nvarchar(max),Status nvarchar(MAX),IOMDt date,Edit nvarchar(MAX), Delt nvarchar(MAX))
							INSERT
							INTO
								@MAA (CreatedDate,IOM2ID,IOMRefNo,CustmNm,Subject,FpoNos,LpoNos,Status,IOMDt,Edit, Delt)
									Select  tp.CreatedDate,IOM2ID,IOMRefNo,dbo.FN_MergeCustShortName(cl.CustomerID) CustmNm,Subject,dbo.FN_MergeTableColumnFPO(tp.Fpos) FpoNos
									 ,dbo.FN_MergeTableColumnLPO(tp.Lpos) LpoNos,tp.Status,IOMDt,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
										+ CONVERT(nvarchar(40), tp.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(40), tp.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' Delet 
									 FROM [IOMTemplate2] tp 
									inner join CheckList cl on tp.ChkListID = cl.ChkListID
									inner join Users u on u.ID = tp.CreatedBy
									and tp.IsActive <> 0
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].IOM2ID)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].IOM2ID) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate,[@MAA].IOM2ID
										   ,[@MAA].IOMRefNo      
										   ,[@MAA].CustmNm
										   ,[@MAA].Subject
										   ,[@MAA].FpoNos
										   ,[@MAA].LpoNos
										   ,[@MAA].Status
										   ,[@MAA].IOMDt
										   ,[@MAA].Edit
										   ,[@MAA].Delt
										   
									  FROM
										  @MAA {1}) RawResults) Results 
											
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";


				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() + " ");

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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["IOM2ID"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					string IOMTT = data["IOMDt"].ToString().Replace("\"", "\\\"");
					string IOMRefNo = data["IOMRefNo"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
					string DInsID = data["IOM2ID"].ToString().Replace("\"", "\\\"");
					//ItemDescription = ItemDescription.Replace("'", "&#39;"); // Single Quote
					//ItemDescription = ItemDescription.Replace(@"\","-"); //<a href="http://www.google.com/">second line</a>
					IOMRefNo = IOMRefNo.Replace("\t", "-"); // 
					sb.AppendFormat(@"""0"": ""{0}""", IOMRefNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					IOMTT = IOMTT.Replace("\t", "-");
					sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["IOMDt"]); // New Line
					sb.Append(",");
					string CustmNm = data["CustmNm"].ToString().Replace("\"", "\\\"");

					//PartNo = PartNo.Replace("'", "&#39;");
					//PartNo = PartNo.Replace(@"\", "-");
					//RecDt = RecDt.Replace("\t", "-");<a href=../Logistics/SevottamCTOne.Aspx?ID={0}&Type={1}>{2}</a>
					CustmNm = CustmNm.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", CustmNm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Subject = data["Subject"].ToString().Replace("\"", "\\\"");
					//PartNo = PartNo.Replace("'", "&#39;");
					//PartNo = PartNo.Replace(@"\", "-");
					//RecDt = RecDt.Replace("\t", "-");<a href=../Logistics/SevottamCTOne.Aspx?ID={0}&Type={1}>{2}</a>
					//AlternativePersonName = AlternativePersonName.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", Subject.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					//string ContactNumber = data["ContactNumber"].ToString().Replace("\"", "\\\"");
					//string AlternativeContactNumber = data["AlternativeContactNumber"].ToString().Replace("\"", "\\\"");
					////Spec = Spec.Replace("'", "&#39;");
					////Spec = Spec.Replace(@"\","-");
					////Subjt = Subjt.Replace("\t", "-");
					//sb.AppendFormat(@"""5"": ""{0}""", ContactNumber.Replace(Environment.NewLine, "\\n"));
					//sb.Append(",");


					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					//AlternativeContactNumber = AlternativeContactNumber.Replace("\t", "-");
					//sb.AppendFormat(@"""5"": ""{0}""", AlternativeContactNumber.Replace(Environment.NewLine, "\\n"));
					//sb.Append(",");

					string FpoNos = data["FpoNos"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					FpoNos = FpoNos.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", FpoNos.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string LpoNos = data["LpoNos"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					//StatIDd = StatIDd.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", LpoNos.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					string Statuss = data["Status"].ToString().Replace("\"", "\\\"");
					//string FPOID = data["ForeignPurchaseOrderId"].ToString().Replace("\"", "\\\"");
					//string CutID = data["CusmorId"].ToString().Replace("\"", "\\\"");
					//string SuppID = data["SupplierId"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					Statuss = Statuss.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", Statuss.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					//string Mail = data["Mail"].ToString().Replace("\"", "\\\"");
					////Spec = Spec.Replace("'", "&#39;");
					////Spec = Spec.Replace(@"\","-");
					//Mail = Mail.Replace("\t", "-");
					//sb.AppendFormat(@"""7"": ""{0:mm-dd-yyyy}""",Mail.Replace(Environment.NewLine, "\\n") );
					//sb.Append(",");

					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
					Edit = Edit.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					//Spec = Spec.Replace("'", "&#39;");
					//Spec = Spec.Replace(@"\","-");
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
				ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Added Items WebService", ex.Message.ToString());
				return "";
			}
		}


		private int ToInt(string toParse)
		{
			int result;
			if (int.TryParse(toParse, out result)) return result;

			return result;
		}
	}
}
