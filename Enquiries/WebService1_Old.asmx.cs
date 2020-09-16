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

namespace DataTables_Dot_Net2010
{
	/// <summary>
	/// Summary description for WebService1
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class WebService1 : System.Web.Services.WebService
	{

		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		public string GetItems()
		{
			try
			{
				//string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
			   // string value = ASCIIEncoding.ASCII.GetString(SelectedItems);
				//var astrem = HttpContext.Current.Request.InputStream;
				string SelectedItems = HttpContext.Current.Session["HFITemsValues"].ToString(); // HttpContext.Current.Request.Params["iAllSelectedItems"];

				//if (SelectedItems != "")
				//{
				//    var ary = new byte[1000];
				//    int i = 0;
				//    //byte[] arr = new byte[]{SelectedItems};
				//    foreach (byte item in SelectedItems)
				//    {
				//        ary[i] = item;
				//        i++;
				//    }
				//    //byte[] Arr = Encoding.ASCII.GetBytes(SelectedItems);
				//    SelectedItems = ASCIIEncoding.ASCII.GetString(ary);
				//}
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string ItemDesc = HttpContext.Current.Request.Params["iItemDescription"];
				string PartNumber = HttpContext.Current.Request.Params["iPartNumber"];
				string Specification = HttpContext.Current.Request.Params["iSpecification"];
                
				var sb = new StringBuilder();
				var whereClause = string.Empty;

				if (SelectedItems.Trim() != "")
				{
					sb.Append(" where dbo.FN_GetDescription(ItemMaster.CategoryID )= 'General' and ItemMaster.id not in (select * from dbo.SplitString('" + SelectedItems + "',','))");
					sb.Append(" and ItemDescription like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification,'') like '%" + Specification + "%'");
				}
				else
					sb.Append(" Where dbo.FN_GetDescription(ItemMaster.CategoryID )= 'General' and ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%'");

				whereClause = sb.ToString();
				sb.Clear();
				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";
				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE ID LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ItemDescription LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR PartNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Specification LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR HSCode LIKE ");
					sb.Append(wrappedSearch);
					filteredWhere = sb.ToString();
				}
				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = sb.ToString();
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", ID ");
					orderByClause = orderByClause.Replace("1", ", ItemDescription ");
					orderByClause = orderByClause.Replace("2", ", PartNumber ");
					orderByClause = orderByClause.Replace("3", ", Specification ");
					orderByClause = orderByClause.Replace("4", ", HSCode ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "ID ASC";

				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();
				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

				string query = @" declare @MA TABLE(ID uniqueidentifier, ItemDescription varchar(MAX), PartNumber varchar(MAX), Specification  varchar(MAX), 
								HSCode varchar(MAX)) INSERT INTO @MA ( ID, ItemDescription, PartNumber, Specification, HSCode ) 
									Select ID, ItemDescription, PartNumber, Specification, HSCode FROM [ItemMaster] {4}                   
									SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MA].ID) FROM @MA) AS TotalRows, 
									(SELECT  count( [@MA].ID) FROM @MA {1}) AS TotalDisplayRows, [@MA].ID, [@MA].ItemDescription, [@MA].PartNumber, 
									[@MA].Specification, [@MA].HSCode FROM @MA {1}) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3}";

				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause + " and ItemMaster.IsActive <> 0");
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
				var count = 1;

				while (data.Read())
				{
					if (totalRecords.Length == 0)
					{
						totalRecords = data["TotalRows"].ToString();
						totalDisplayRecords = data["TotalDisplayRows"].ToString();
					}
					sb.Append("{");
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"]);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					sb.AppendFormat(@"""0"": ""{0}""", count++);
					sb.Append(",");
					string ItemDescription = data["ItemDescription"].ToString().Replace("\"", "\\\"");// DoubleQuote Replacing 
					ItemDescription = ItemDescription.Replace("\t", "-"); // 
					sb.AppendFormat(@"""1"": ""{0}""", ItemDescription.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					string PartNo = data["PartNumber"].ToString().Replace("\"", "\\\"");
					PartNo = PartNo.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", PartNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Spec = data["Specification"].ToString().Replace("\"", "\\\"");
					Spec = Spec.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", Spec.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string HsCode = data["HSCode"].ToString().Replace("\"", "\\\"");
					HsCode = HsCode.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", HsCode.Replace(Environment.NewLine, "\\n"));
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
		/// FOR FOREIGN ENQUIRY
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetFItems()
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
				string CnctPrsn = HttpContext.Current.Request.Params["sSearch_6"];
				string Status = HttpContext.Current.Request.Params["sSearch_7"];
                string Filename = HttpContext.Current.Request.Form["HFFilename"];

                    // HttpContext.Current.Request.Params["sPaginationType"];//HttpContext.Current.Request.Url.AbsoluteUri;
				StringBuilder s = new StringBuilder();
				if (date != null && date != "")
				{
					DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
					DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and f.EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (FeNo != "")
					s.Append(" and f.EnquireNumber LIKE '%" + FeNo + "%'");
				if (RDate != null && RDate != "")
				{
					DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
					DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and f.ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (Subject != "")
					s.Append(" and f.Subject LIKE '%" + Subject + "%'");
				if (Status != "")
					s.Append(" and dbo.FN_GetStatus(f.StatusTypeId) LIKE '%" + Status + "%'");
				if (Dept != "")
					s.Append(" and dbo.FN_GetDescription(f.DepartmentId) LIKE '%" + Dept + "%'");
				if (Cust != "")
					s.Append(" and dbo.GetAllCustomerName(CusmorId) LIKE '%" + Cust + "%'");
				if (CnctPrsn != "")
					s.Append(" and dbo.FN_GetCustContactPerson(CusmorId) LIKE '%" + CnctPrsn + "%'");

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

				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", CreatedDate ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "CreatedDate DESC";
				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();
				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

				string query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), AMENDEDIT nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime,Export nvarchar(max), VendorIds nvarchar(max),
                            History nvarchar(MAX))
							
							;with a as(SELECT Distinct l.ForeignEnquiryId, l.IsOffer fROM LQuotation l WHERE l.IsOffer =1),
							b as ( Select distinct f.EnquiryDate, f.EnquireNumber, f.ReceivedDate, f.Subject, f.ForeignEnquireId, 
							(dbo.FN_GetStatus(f.StatusTypeId) + CASE WHEN (a.IsOffer = 1 and f.StatusTypeId = 30) THEN ' and Offered to Customer' ELSE '' END) StatusTypeId, 
							dbo.FN_GetDescription(f.DepartmentId) DepartmentId, dbo.GetAllCustomerName(f.CusmorId) CusmorId,f.CusmorId as custId, 
							dbo.FN_GetCustContactPerson(f.CusmorId) ContactPerson, f.IsActive, f.CreatedDate,f.CreatedBy,
							'<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL,
							'<img src=../images/Edit.jpeg alt=Edit onclick=Ammendement(this,''' + CONVERT(nvarchar(40), f.CreatedBy) 
								+ ''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +') />' AMENDEDIT, 
							'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
							+ CONVERT(nvarchar(max), f.CreatedBy) +''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +') />' EDIT, 
							'<img src=../images/delete.png alt=Delete onclick=Delet(this,''' + CONVERT(nvarchar(max), f.CreatedBy) +''',' 
							+(case when u.ContactType = 'Customer' then '1' else '0' end) +','''+Convert(nvarchar(max),f.CompanyId) +''') ) />' Delt,
							 '<img src=../images/EXCEL.png class= ''Exp'' value='''+Convert(varchar(40),f.ForeignEnquireId)+''' />' Export,f.CompanyId,f.VendorIds,FEAmndmnt.ForeignEnquireId as History   
							FROM [FEnquiry] f inner join Users u on u.ID = f.CreatedBy LEFT JOIN a ON a.ForeignEnquiryId = f.ForeignEnquireId left join FEAmndmnt  on FEAmndmnt.ForeignEnquireId = f.ForeignEnquireId
							{4} )

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History) 
							 SELECT EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId, StatusTypeId, DepartmentId, CusmorId, ContactPerson, MAIL, AMENDEDIT,
								EDIT, Delt, CompanyId, CreatedDate, Export, VendorIds,History FROM b order by b.CreatedDate Desc 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].AMENDEDIT,[@MAA].EDIT, [@MAA].Delt, [@MAA].Export, [@MAA].VendorIds, [@MAA].History FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
				string Where = "";
				Guid CompanyID = new Guid(Session["CompanyID"].ToString());
				if (new Guid(Session["CompanyID"].ToString()) != null && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
					Where = " (f.CompanyId = " + "'" + CompanyID + "'" + " or f.VendorIds = " + "'" + CompanyID + "'" + ") and ";
				string Role = HttpContext.Current.Session["AccessRole"].ToString();
				Where += " f.IsActive <> 0";
				if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
				{
					string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
					if (Mode == "tdt")
					{
						if (Session["IsUser"] == null && Session["IsUser"].ToString() == "0")
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								" where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where :
								" where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString()
								+ " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where);
						}
						else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString()))
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
									" where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() +
									"' AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where :
									" where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() + "' " + s.ToString()
									+ " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where);
						}
					}
				}
				else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
						" WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " : " f.IsActive <> 0 and (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + "AND")
						+ " f.CusmorId in (Select Data from dbo.SplitString('"
						+ ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and f.CompanyId = '" + Session["CompanyID"] + " '"
						: (s.ToString() == "" ? " AND f.IsActive <> 0 " : " AND f.IsActive <> 0  " + s.ToString() + "AND") +
						" f.custId in (Select Data from dbo.SplitString('"
						+ ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ','))  and f.CompanyId = '" + Session["CompanyID"] + "'");
				}
				else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
				   ("Where " + Where)//(isnull(convert(nvarchar(40),b.VendorIds),'') = '' or isnull(convert(nvarchar(40),b.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
				   : (" where f.IsActive <> 0  " //AND (isnull(convert(nvarchar(40),b.VendorIds),'') = '' or isnull(convert(nvarchar(40),b.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
				   + s.ToString() + " and " + Where));
				}
				else
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
						//Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
				   ("Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + Where) :
						//Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
				   (" where f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + " and " + Where));
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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");

					string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
					string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
					EnqNo = EnqNo.Replace("\t", "-"); // 
					sb.AppendFormat(@"""0"": ""<a href=FEDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
					sb.Append(",");

					if (data["ReceivedDate"].ToString().Replace("\"", "\\\"") != CommonBLL.EndDate.ToString())
					{
						sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
						sb.Append(",");
					}
					else
					{
						sb.AppendFormat(@"""2"": ""{0}""", "");
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

					string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
					ContactPerson = ContactPerson.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
					StatIDd = StatIDd.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

                    string Histroy = data["History"].ToString().Replace("\"", "\\\"");
                    Histroy = Histroy.Replace("\t", "-");
                    if (Histroy != "")
                    {
                        sb.AppendFormat(@"""8"": ""<a href=FeAmdmntDtls.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""8"": """"");
                        sb.Append(",");
                    }

					string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");

					Mail = Mail.Replace("\t", "-");
					sb.AppendFormat(@"""9"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string AMENDEDIT = data["AMENDEDIT"].ToString().Replace("\"", "\\\"");

					AMENDEDIT = AMENDEDIT.Replace("\t", "-");
					sb.AppendFormat(@"""10"": ""{0}""", AMENDEDIT.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");

					Edit = Edit.Replace("\t", "-");
					sb.AppendFormat(@"""11"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");


					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					Del = Del.Replace("\t", "-");
					sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Export = data["Export"].ToString().Replace("\"", "\\\"");
					Export = Export.Replace("\t", "-");
					sb.AppendFormat(@"""13"": ""{0}""", Export.Replace(Environment.NewLine, "\\n"));
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
		/// FOR FLOATED FOREIGN ENQUIRY
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetFFEItems()
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
				string Ven = HttpContext.Current.Request.Params["sSearch_5"];
				string Cust = HttpContext.Current.Request.Params["sSearch_6"];
				string CnctPrsn = HttpContext.Current.Request.Params["sSearch_7"];
				string Status = HttpContext.Current.Request.Params["sSearch_8"];

				StringBuilder s = new StringBuilder();
				if (date != null && date != "")
				{
					DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
					DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append("  EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (FeNo != "")
					s.Append(" and EnquireNumber LIKE '%" + FeNo + "%'");
				if (RDate != null && RDate != "")
				{
					DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
					DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append(" and ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (Subject != "")
					s.Append(" and f.Subject LIKE '%" + Subject + "%'");
				if (Status != "")
					s.Append(" and (dbo.FN_GetStatus(f.StatusTypeId) + CASE WHEN (a.IsOffer = 1 and f.StatusTypeId = 30) THEN ' and Offered to Customer' ELSE '' END) LIKE '%" + Status + "%'");
				if (Dept != "")
					s.Append(" and dbo.FN_GetDescription(f.DepartmentId) LIKE '%" + Dept + "%'");
				if (Ven != "")
					s.Append(" and dbo.FN_GetVendorBussName(ISNULL(f.VendorIds,'00000000-0000-0000-0000-000000000000')) LIKE '%" + Ven + "%'");
				if (Cust != "")
					s.Append(" and dbo.GetAllCustomerName(CusmorId) LIKE '%" + Cust + "%'");
				if (CnctPrsn != "")
					s.Append(" and dbo.FN_GetCustContactPerson(CusmorId) LIKE '%" + CnctPrsn + "%'");

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

				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", CreatedDate ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "CreatedDate DESC";
				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();
				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

				string query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), VendorIds nvarchar(max),CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime)
							
							;with a as(SELECT Distinct l.ForeignEnquiryId, l.IsOffer fROM LQuotation l WHERE l.IsOffer =1),
							b as ( Select distinct EnquiryDate, EnquireNumber, ReceivedDate, f.Subject, ForeignEnquireId, 
							(dbo.FN_GetStatus(f.StatusTypeId) + CASE WHEN (a.IsOffer = 1 and f.StatusTypeId = 30) THEN ' and Offered to Customer' ELSE '' END) StatusTypeId, 
							dbo.FN_GetDescription(f.DepartmentId) DepartmentId,dbo.FN_GetVendorBussName(ISNULL(f.VendorIds,'00000000-0000-0000-0000-000000000000')) VendorIds,dbo.GetAllCustomerName(CusmorId) CusmorId,CusmorId as custId, 
							dbo.FN_GetCustContactPerson(CusmorId) ContactPerson, f.IsActive, f.CreatedDate,f.CreatedBy,
							'<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL, 
							'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''
							+ CONVERT(nvarchar(max), f.CreatedBy) +''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +') />' EDIT, 
							'<img src=../images/delete.png alt=Delete onclick=Delet(this,''' + CONVERT(nvarchar(max), f.CreatedBy) +''',' 
							+(case when u.ContactType = 'Customer' then '1' else '0' end) +','''+Convert(nvarchar(max),f.CompanyId) +''') ) />' Delt,f.CompanyId                             
							FROM [FEnquiry] f inner join Users u on u.ID = f.CreatedBy LEFT JOIN a ON a.ForeignEnquiryId = f.ForeignEnquireId {4})

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, VendorIds,CusmorId, 
								ContactPerson, MAIL, EDIT, Delt, CompanyId, CreatedDate) 
							 SELECT EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId, StatusTypeId, DepartmentId, VendorIds,CusmorId, ContactPerson, MAIL, 
								EDIT, Delt, CompanyId, CreatedDate FROM b  
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].VendorIds, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].EDIT, [@MAA].Delt FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
				string Where = "";
				Guid CompanyID = new Guid(Session["CompanyID"].ToString());
				if (new Guid(Session["CompanyID"].ToString()) != null && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
					Where = "Where (f.CompanyId = " + "'" + CompanyID + "' or f.VendorIds = " + "'" + CompanyID + "'" + ") and f.ReceivedDate <> '" + CommonBLL.EndDate.ToString("yyyy-MM-dd") + 
						"' and ISNULL(f.VendorIds,'00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000'  and ";
				string Role = HttpContext.Current.Session["AccessRole"].ToString();
				Where += " f.IsActive <> 0 ";
			   
			   if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
						" WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND " : 
						" f.IsActive <> 0  and " + s.ToString() + " and ")
						+ " f.CusmorId in (Select * from dbo.SplitString('"
						+ ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and f.CompanyId = '" + Session["CompanyID"] + " ' and VendorIds <>'00000000-0000-0000-0000-000000000000' "
						: (s.ToString() == "" ? " AND b.IsActive <> 0  and " 
						: " AND f.IsActive <> 0  and " + s.ToString() + "AND ") +
						" f.CusmorId in (Select * from dbo.SplitString('"
						+ ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',')) and f.CompanyId = '" + Session["CompanyID"] + "' and VendorIds <> '00000000-0000-0000-0000-000000000000' ");
				}
				else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
				   (" " + Where) :
				   (" " + Where + " and " + s.ToString()));
				}
				else
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
						//Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
				   ("Where " + Where) :
						//Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
				   (" " 
				   + s.ToString() + " and " + Where));
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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");

					string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
					string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
					EnqNo = EnqNo.Replace("\t", "-"); // 
					sb.AppendFormat(@"""0"": ""<a href=FEDetails.aspx?FFEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
					sb.Append(",");

					sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
					sb.Append(",");

					string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
					Subjt = Subjt.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");

					DeptID = DeptID.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string VenID = data["VendorIds"].ToString().Replace("\"", "\\\"");
					VenID = VenID.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", VenID.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string CustID = data["CusmorId"].ToString().Replace("\"", "\\\"");

					CustID = CustID.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
					ContactPerson = ContactPerson.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
					StatIDd = StatIDd.Replace("\t", "-");
					sb.AppendFormat(@"""8"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
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
		/// FOR LOCAL ENQUIRY
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetLItems()
		{
			try
			{
				string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				var sb = new StringBuilder();

				string LeNo = HttpContext.Current.Request.Params["sSearch_0"];
				string date = HttpContext.Current.Request.Params["sSearch_1"];
				string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
				string Subject = HttpContext.Current.Request.Params["sSearch_3"];
				string Supplier = HttpContext.Current.Request.Params["sSearch_4"];
				string Customer = HttpContext.Current.Request.Params["sSearch_5"];
				string Status = HttpContext.Current.Request.Params["sSearch_6"];

				StringBuilder s = new StringBuilder();
				if (date != "")
				{
					DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
					DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
					if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
						s.Append("  l.LEIssueDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
				}
				if (LeNo != "")
					s.Append(" and l.EnquireNumber LIKE '%" + LeNo + "%'");
				if (FeNo != "")
					s.Append(" and dbo.FN_FEnquiryNumber(l.ForeignEnquireId) LIKE '%" + FeNo + "%'");
				if (Subject != "")
					s.Append(" and l.Subject LIKE '%" + Subject + "%'");
				if (Status != "")
				{
					string isofrstr = "and Offered to Customer".ToLowerInvariant();
					if (isofrstr.Contains(Status))
						s.Append("and (lq.IsOffer = 1 and fe.StatusTypeId = 30)");
					else
						s.Append(" and dbo.FN_GetStatus(l.StatusTypeId) LIKE '%" + Status + "%'");
				}
				if (Supplier != "")
					s.Append(" and dbo.FN_GetSupplierNm(l.SupplierIds) LIKE '%" + Supplier + "%'");
				if (Customer != "")
					s.Append(" and dbo.GetActiveCustomerName(l.CusmorId) LIKE '%" + Customer + "%'");

				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";
				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE LEIssueDate LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR EnquireNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ForeignEnquireId LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Subject LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR StatusTypeId LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR SupplierIds LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CustomerName LIKE ");
					sb.Append(wrappedSearch);

					filteredWhere = sb.ToString();
				}

				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", CreatedDate ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "ID ASC";
				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();
				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();
				string query = @"  
							declare @MAA TABLE(CreatedDate datetime,LEIssueDate datetime, EnquireNumber nvarchar(400), ForeignEnquireId nvarchar(MAX), Subject nvarchar(MAX), 
							LocalEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), SupplierIds nvarchar(MAX), CustomerName nvarchar(MAX), MAIL nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier)
							INSERT INTO
								@MAA (CreatedDate,LEIssueDate, EnquireNumber, ForeignEnquireId , Subject, LocalEnquireId, StatusTypeId, SupplierIds, CustomerName, MAIL, EDIT, Delt,CompanyId )
									Select l.CreatedDate,l.LEIssueDate, l.EnquireNumber, dbo.FN_FEnquiryNumber(l.ForeignEnquireId) ForeignEnquireId, l.Subject, l.LocalEnquireId, 
									(dbo.FN_GetStatus(l.StatusTypeId) + CASE WHEN (lq.IsOffer = 1 and fe.StatusTypeId = 30) THEN ' and Offered to Customer' ELSE '' END) StatusTypeId,
								   dbo.FN_GetSupplierNm(l.SupplierIds) SupplierIds, dbo.GetActiveCustomerName(l.CusmorId) as CustomerName,
									'<img src=../images/MailIcon.jpg alt=Mail onclick=mailsDetails(this) />' MAIL, 
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+ CONVERT(nvarchar(max), l.CreatedBy) +''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +') />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(100), l.CreatedBy) +''','+(case when u.ContactType = 'Customer' then '1' else '0' end) +','''+Convert(nvarchar(max),l.CompanyId) +''') />' Delt, l.CompanyId
									FROM [LEnquiry] l inner join Users u on u.ID = l.CreatedBy   
													  left join FEnquiry fe on fe.ForeignEnquireId = l.ForeignEnquireId
													  LEFT JOIN LQuotation lq ON l.localenquireID=lq.localenquireID  
									{1}{4} 

							SELECT *
							FROM
								(SELECT row_number() OVER ({0} ) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].LocalEnquireId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].LocalEnquireId) FROM @MAA) AS TotalDisplayRows			   
										   ,[@MAA].CreatedDate
										   ,[@MAA].LEIssueDate
										   ,[@MAA].EnquireNumber      
										   ,[@MAA].ForeignEnquireId
										   ,[@MAA].Subject
										   ,[@MAA].LocalEnquireId
										   ,[@MAA].StatusTypeId
										   ,[@MAA].SupplierIds
										   ,[@MAA].CustomerName
										   ,[@MAA].MAIL
										   ,[@MAA].EDIT
										   ,[@MAA].Delt 
									  FROM
										  @MAA) RawResults) Results 
											WHERE
												RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";

				string where = "";
				Guid CompanyID = new Guid(Session["CompanyID"].ToString());
				if (new Guid(Session["CompanyID"].ToString()) != null && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
					where = " l.CompanyId = " + "'" + CompanyID + "'" + " and ";
				string Role = HttpContext.Current.Session["AccessRole"].ToString();
				where += "l.IsActive <> 0 order by l.CreatedDate desc ";
				if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
				{
					string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
					if (Mode == "tdt")
					{
						if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								" where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and" + where :
								" where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString() + " and " + where);
						}
						else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
							(CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								" where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and " + where :
								" where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'"
								  + s.ToString() + " and " + where);
						}
					}
					else if (Mode == "odt")
					{
						if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0")
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
							" where " + where : " where l.IsActive <> 0 " + s.ToString() + " and " + where);
						}
						else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
								(CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
						{
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
								" where l.IsActive <> 0 and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and " + where :
								" where l.IsActive <> 0 and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + s.ToString() + " and " + where);
						}
					}
				}
				else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
				   ("Where" + where) : (" where " + s.ToString() + " and " + where));
				}
				else
				{
					query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
						(Role != CommonBLL.SuperAdminRole ? " where l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and" + where : " where " + where) :
						(Role != CommonBLL.SuperAdminRole ? " where l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + "and" + s.ToString() + " and " + where : " where " + where));
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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["LocalEnquireId"].ToString()); // count++);
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");

					string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
					string EnqId = data["LocalEnquireId"].ToString().Replace("\"", "\\\"");
					EnqNo = EnqNo.Replace("\t", "-");
					sb.AppendFormat(@"""0"": ""<a href=LEDetails.aspx?LEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["LEIssueDate"]);
					sb.Append(",");

					string RecDt = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
					RecDt = RecDt.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
					Subjt = Subjt.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string DeptID = data["SupplierIds"].ToString().Replace("\"", "\\\"");
					DeptID = DeptID.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string CustomerName = data["CustomerName"].ToString().Replace("\"", "\\\"");
					CustomerName = CustomerName.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", CustomerName.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
					StatIDd = StatIDd.Replace("\t", "-");
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
		/// FOR Item Master
		/// </summary>
		/// <param name="toParse"></param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetIMItems()
		{
			try
			{
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];
				string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
				string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
				string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification

				var sb = new StringBuilder();
				var whereClause = string.Empty;
				sb.Append(" Where dbo.FN_GetDescription(i.CategoryID) = 'General' and ItemDescription like '%" + ItemDesc + "%' and PartNumber like '%" + PartNumber + "%' and Specification like '%" + Specification + "%'");
				whereClause = sb.ToString();
				sb.Clear();
				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";
				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE CategoryID LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ItemDescription LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ItemDescription LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR PartNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Specification LIKE ");
					sb.Append(wrappedSearch);
					filteredWhere = sb.ToString();
				}
				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = sb.ToString();
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", CategoryID ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "CategoryID DESC";
				orderByClause = "ORDER BY " + orderByClause;
				sb.Clear();
				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();
				string Where = "";
				string CompanyID = HttpContext.Current.Session["CompanyID"].ToString();
				//if (Session["CompanyID"] != null && HttpContext.Current.Session["CompanyID"].ToString() != "" && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
				//    Where = " and i.CompanyId = '" + CompanyID + "'";

				string query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
							ID nvarchar(400),SubItems bit, EDIT nvarchar(MAX), Delt nvarchar(MAX), CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
								@MAA ( CategoryID, ItemDescription, PartNumber , Specification, ID,SubItems, EDIT, Delt, CompanyId, CreatedDate )
									Select dbo.FN_GetDescription(CategoryID), ItemDescription,PartNumber, Specification, i.ID,IsSubItems,
									'<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this) />' EDIT, 
									'<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
										+ CONVERT(nvarchar(100), i.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end)+') />' Delt, i.CompanyId, i.CreatedDate    
									
									FROM [ItemMaster] i inner join EnumMaster e on i.CategoryID = e.ID 
									inner join Users u on u.ID = i.CreatedBy {5} {4} 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
							[@MAA].Specification, [@MAA].ID, [@MAA].SubItems, [@MAA].EDIT, [@MAA].Delt, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
							WHERE RowNumber BETWEEN {2} AND {3} ";

				//Guid CompanyId = new Guid(Session["CompanyID"].ToString());
				query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause + " and i.IsActive <> 0 order by i.CreatedDate DESC ", Where);

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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["CategoryID"]);
					sb.Append(",");
					string itm = data["ItemDescription"].ToString().Replace("\"", "\\\"");
					itm = itm.Replace("\t", "-");
					sb.AppendFormat(@"""1"": ""{0}""", itm.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string RecDt = data["PartNumber"].ToString().Replace("\"", "\\\"");
					RecDt = RecDt.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string Subjt = data["Specification"].ToString().Replace("\"", "\\\"");
					Subjt = Subjt.Replace("\t", "-");
					sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");
					string SubItems = "";
					if (Convert.ToBoolean(data["SubItems"].ToString()))
						SubItems = "True";
					else
						SubItems = "False";
					sb.AppendFormat(@"""4"": ""{0}""", SubItems);
					sb.Append(",");
					sb.AppendFormat(@"""5"": ""{0}""", data["EDIT"].ToString());
					sb.Append(",");
					string Del = data["Delt"].ToString().Replace("\"", "\\\"");
					Del = Del.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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

		[WebMethod(EnableSession = true)]
		[ScriptMethod(UseHttpGet = true)]
		[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
		public string GetFeNew()
		{
			try
			{
				int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
				int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
				int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
				string rawSearch = HttpContext.Current.Request.Params["sSearch"];

				string date = HttpContext.Current.Request.Params["sSearch_0"];
				string FeNo = HttpContext.Current.Request.Params["sSearch_1"];
				string FPONoo = HttpContext.Current.Request.Params["sSearch_2"];
				string RDate = HttpContext.Current.Request.Params["sSearch_3"];
				string Subject = HttpContext.Current.Request.Params["sSearch_4"];
				string Status = HttpContext.Current.Request.Params["sSearch_5"];
				string Dept = HttpContext.Current.Request.Params["sSearch_6"];
				string Cust = HttpContext.Current.Request.Params["sSearch_7"];
				string ModeWhere = "";

				if (Session["UserID"].ToString() != "1" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
				{
					string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
					if (Mode == "tldt")
						ModeWhere = " and createdby =" + Session["UserID"];
				}

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
				if (FPONoo != "")
					s.Append(" and FPONumber LIKE '%" + FPONoo + "%'");
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
					s.Append(" and Status LIKE '%" + Status + "%'");
				if (Dept != "")
					s.Append(" and DepartmentId LIKE '%" + Dept + "%'");
				if (Cust != "")
					s.Append(" and CustmrNm LIKE '%" + Cust + "%'");

				var sb = new StringBuilder();
				var filteredWhere = string.Empty;
				var wrappedSearch = "'%" + rawSearch + "%'";
				if (rawSearch.Length > 0)
				{
					sb.Append(" WHERE EnquiryDate LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR EnquireNumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR FPONumber LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR ReceivedDate LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Subject LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR Status LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR DepartmentId LIKE ");
					sb.Append(wrappedSearch);
					sb.Append(" OR CustmrNm LIKE ");
					sb.Append(wrappedSearch);
					filteredWhere = sb.ToString();
				}
				sb.Clear();
				string orderByClause = string.Empty;
				sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
				sb.Append(" ");
				sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
				orderByClause = "0 DESC";
				if (!String.IsNullOrEmpty(orderByClause))
				{
					orderByClause = orderByClause.Replace("0", ", ForeignEnquireId ");
					orderByClause = orderByClause.Remove(0, 1);
				}
				else
					orderByClause = "ID ASC";
				orderByClause = "ORDER BY " + orderByClause;

				sb.Clear();

				var numberOfRowsToReturn = "";
				numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

				string query = @"  
							declare @MAA TABLE(ForeignEnquireId uniqueidentifier,EnquiryDate datetime,EnquireNumber varchar(500),FPONumber varchar(max),
							ReceivedDate datetime,Subject varchar(max),Status varchar(500),DepartmentId varchar(500),CustmrNm varchar(500),
							CreatedBy uniqueidentifier)
							INSERT
							INTO
								@MAA (ForeignEnquireId,EnquiryDate,EnquireNumber,FPONumber,ReceivedDate,Subject,Status,DepartmentId,CustmrNm,CreatedBy)
										select f.ForeignEnquireId,f.EnquiryDate,f.EnquireNumber,f.FPONumber,f.ReceivedDate,f.Subject,
										f.Status,f.DepartmentId,f.CustmrNm,f.CreatedBy from FE_SinglePage f
									{4}                   

							SELECT *
							FROM
								(SELECT row_number() OVER ({0}) AS RowNumber
									  , *
								 FROM
									 (SELECT (SELECT count([@MAA].ForeignEnquireId)
											  FROM
												  @MAA) AS TotalRows
										   , ( SELECT  count( [@MAA].ForeignEnquireId) FROM @MAA {1}) AS TotalDisplayRows			   
										   ,[@MAA].ForeignEnquireId
										   ,[@MAA].EnquiryDate
										   ,[@MAA].EnquireNumber
										   ,[@MAA].FPONumber
										   ,[@MAA].ReceivedDate
										   ,[@MAA].Subject
										   ,[@MAA].Status
										   ,[@MAA].DepartmentId
										   ,[@MAA].CustmrNm
										   ,[@MAA].CreatedBy 
									  FROM
										  @MAA {1}) RawResults) Results WHERE
												RowNumber BETWEEN {2} AND {3} order by ForeignEnquireId Desc";

				if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
				{
					string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
					if (Mode == "tldt")
					{
						if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
								" where f.IsActive <> 0 " + s.ToString());
						else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
							query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
								"where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' AND f.IsActive <> 0");
					}
					else
						query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, " where f.IsActive <> 0 " + s.ToString());
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
					sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString());
					sb.Append(",");
					sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
					sb.Append(",");
					sb.AppendFormat(@"""0"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
					sb.Append(",");

					string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
					string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
					EnqNo = EnqNo.Replace("\t", "-");
					sb.AppendFormat(@"""1"": ""<a href=FullDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string FPONo = data["FPONumber"].ToString().Replace("\"", "\\\"");
					FPONo = FPONo.Replace("\t", "-");
					sb.AppendFormat(@"""2"": ""{0}""", FPONo.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
					sb.Append(",");

					string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
					Subjt = Subjt.Replace("\t", "-");
					sb.AppendFormat(@"""4"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
					StatIDd = StatIDd.Replace("\t", "-");
					sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");
					DeptID = DeptID.Replace("\t", "-");
					sb.AppendFormat(@"""6"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
					sb.Append(",");

					string CustID = data["CustmrNm"].ToString().Replace("\"", "\\\"");
					CustID = CustID.Replace("\t", "-");
					sb.AppendFormat(@"""7"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
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

//        [WebMethod(EnableSession = true)]
//        [ScriptMethod(UseHttpGet = true)]
//        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
//        public string GetFeAmendments()
//        {
//            try
//            {
//                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
//                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
//                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
//                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

//                string date = HttpContext.Current.Request.Params["sSearch_0"];
//                string FeNo = HttpContext.Current.Request.Params["sSearch_1"];
//                string FPONoo = HttpContext.Current.Request.Params["sSearch_2"];
//                string RDate = HttpContext.Current.Request.Params["sSearch_3"];
//                string Subject = HttpContext.Current.Request.Params["sSearch_4"];
//                string Status = HttpContext.Current.Request.Params["sSearch_5"];
//                string Dept = HttpContext.Current.Request.Params["sSearch_6"];
//                string Cust = HttpContext.Current.Request.Params["sSearch_7"];
//                string ModeWhere = "";

//                if (Session["UserID"].ToString() != "1" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
//                {
//                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
//                    if (Mode == "tldt")
//                        ModeWhere = " and createdby =" + Session["UserID"];
//                }

//                StringBuilder s = new StringBuilder();
//                if (date != "")
//                {
//                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
//                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
//                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
//                        s.Append(" and EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
//                }
//                if (FeNo != "")
//                    s.Append(" and EnquireNumber LIKE '%" + FeNo + "%'");
//                if (FPONoo != "")
//                    s.Append(" and FPONumber LIKE '%" + FPONoo + "%'");
//                if (RDate != "")
//                {
//                    DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
//                    DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
//                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
//                        s.Append(" and ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
//                }
//                if (Subject != "")
//                    s.Append(" and Subject LIKE '%" + Subject + "%'");
//                if (Status != "")
//                    s.Append(" and Status LIKE '%" + Status + "%'");
//                if (Dept != "")
//                    s.Append(" and DepartmentId LIKE '%" + Dept + "%'");
//                if (Cust != "")
//                    s.Append(" and CustmrNm LIKE '%" + Cust + "%'");

//                var sb = new StringBuilder();
//                var filteredWhere = string.Empty;
//                var wrappedSearch = "'%" + rawSearch + "%'";
//                if (rawSearch.Length > 0)
//                {
//                    sb.Append(" WHERE EnquiryDate LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR EnquireNumber LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR FPONumber LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR ReceivedDate LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR Subject LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR Status LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR DepartmentId LIKE ");
//                    sb.Append(wrappedSearch);
//                    sb.Append(" OR CustmrNm LIKE ");
//                    sb.Append(wrappedSearch);
//                    filteredWhere = sb.ToString();
//                }
//                sb.Clear();
//                string orderByClause = string.Empty;
//                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
//                sb.Append(" ");
//                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
//                orderByClause = "0 DESC";
//                if (!String.IsNullOrEmpty(orderByClause))
//                {
//                    orderByClause = orderByClause.Replace("0", ", ForeignEnquireId ");
//                    orderByClause = orderByClause.Remove(0, 1);
//                }
//                else
//                    orderByClause = "ID ASC";
//                orderByClause = "ORDER BY " + orderByClause;

//                sb.Clear();

//                var numberOfRowsToReturn = "";
//                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

//                string query = @"  
//							declare @MAA TABLE(ForeignEnquireId uniqueidentifier,EnquireNumber varchar(500),EnquiryDate datetime,
//							ReceivedDate datetime,Subject varchar(max),Departmnt varchar(500),CustmrNm varchar(500),ContctPrsn varchar(500),Status varchar(500))
//							INSERT
//							INTO
//								@MAA (ForeignEnquireId,EnquireNumber,EnquiryDate,ReceivedDate,Subject,Departmnt,CustmrNm,ContctPrsn,Status)
//										select f.ForeignEnquireId,f.EnquiryDate,f.EnquireNumber,f.FPONumber,f.ReceivedDate,f.Subject,
//										f.Status,f.DepartmentId,f.CustmrNm,f.CreatedBy from FE_SinglePage f
//									{4}                   
//
//							SELECT *
//							FROM
//								(SELECT row_number() OVER ({0}) AS RowNumber
//									  , *
//								 FROM
//									 (SELECT (SELECT count([@MAA].ForeignEnquireId)
//											  FROM
//												  @MAA) AS TotalRows
//										   , ( SELECT  count( [@MAA].ForeignEnquireId) FROM @MAA {1}) AS TotalDisplayRows			   
//										   ,[@MAA].ForeignEnquireId
//										   ,[@MAA].EnquiryDate
//										   ,[@MAA].EnquireNumber
//										   ,[@MAA].FPONumber
//										   ,[@MAA].ReceivedDate
//										   ,[@MAA].Subject
//										   ,[@MAA].Status
//										   ,[@MAA].DepartmentId
//										   ,[@MAA].CustmrNm
//										   ,[@MAA].CreatedBy 
//									  FROM
//										  @MAA {1}) RawResults) Results WHERE
//												RowNumber BETWEEN {2} AND {3} order by ForeignEnquireId Desc";

//                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
//                {
//                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
//                    if (Mode == "tldt")
//                    {
//                        if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
//                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
//                                " where f.IsActive <> 0 " + s.ToString());
//                        else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
//                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
//                                "where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' AND f.IsActive <> 0");
//                    }
//                    else
//                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, " where f.IsActive <> 0 " + s.ToString());
//                }
//                s.Clear();
//                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
//                SqlConnection conn = new SqlConnection(connectionString);

//                if (conn.State == ConnectionState.Closed)
//                    conn.Open();

//                var DB = new SqlCommand();
//                DB.Connection = conn;
//                DB.CommandText = query;
//                var data = DB.ExecuteReader();

//                var totalDisplayRecords = "";
//                var totalRecords = "";
//                string outputJson = string.Empty;

//                var rowClass = "";
//                var count = 0;

//                while (data.Read())
//                {
//                    if (totalRecords.Length == 0)
//                    {
//                        totalRecords = data["TotalRows"].ToString();
//                        totalDisplayRecords = data["TotalDisplayRows"].ToString();
//                    }
//                    sb.Append("{");
//                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString());
//                    sb.Append(",");
//                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
//                    sb.Append(",");
//                    sb.AppendFormat(@"""0"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
//                    sb.Append(",");

//                    string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
//                    string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
//                    EnqNo = EnqNo.Replace("\t", "-");
//                    sb.AppendFormat(@"""1"": ""<a href=FullDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
//                    sb.Append(",");

//                    string FPONo = data["FPONumber"].ToString().Replace("\"", "\\\"");
//                    FPONo = FPONo.Replace("\t", "-");
//                    sb.AppendFormat(@"""2"": ""{0}""", FPONo.Replace(Environment.NewLine, "\\n"));
//                    sb.Append(",");

//                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
//                    sb.Append(",");

//                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
//                    Subjt = Subjt.Replace("\t", "-");
//                    sb.AppendFormat(@"""4"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
//                    sb.Append(",");

//                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
//                    StatIDd = StatIDd.Replace("\t", "-");
//                    sb.AppendFormat(@"""5"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
//                    sb.Append(",");

//                    string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");
//                    DeptID = DeptID.Replace("\t", "-");
//                    sb.AppendFormat(@"""6"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
//                    sb.Append(",");

//                    string CustID = data["CustmrNm"].ToString().Replace("\"", "\\\"");
//                    CustID = CustID.Replace("\t", "-");
//                    sb.AppendFormat(@"""7"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
//                    sb.Append("},");

//                }
//                conn.Close();
//                if (totalRecords.Length == 0)
//                {
//                    sb.Append("{");
//                    sb.Append(@"""sEcho"": ");
//                    sb.AppendFormat(@"""{0}""", sEcho);
//                    sb.Append(",");
//                    sb.Append(@"""iTotalRecords"": 0");
//                    sb.Append(",");
//                    sb.Append(@"""iTotalDisplayRecords"": 0");
//                    sb.Append(", ");
//                    sb.Append(@"""aaData"": [ ");
//                    sb.Append("]}");
//                    outputJson = sb.ToString();

//                    return outputJson;
//                }
//                outputJson = sb.Remove(sb.Length - 1, 1).ToString();
//                sb.Clear();

//                sb.Append("{");
//                sb.Append(@"""sEcho"": ");
//                sb.AppendFormat(@"""{0}""", sEcho);
//                sb.Append(",");
//                sb.Append(@"""iTotalRecords"": ");
//                sb.Append(totalRecords);
//                sb.Append(",");
//                sb.Append(@"""iTotalDisplayRecords"": ");
//                sb.Append(totalDisplayRecords);
//                sb.Append(", ");
//                sb.Append(@"""aaData"": [ ");
//                sb.Append(outputJson);
//                sb.Append("]}");
//                outputJson = sb.ToString();

//                return outputJson;
//            }
//            catch (Exception ex)
//            {
//                ErrorLog ELog = new ErrorLog();
//                string ErrMsg = ex.Message;
//                int LineNo = ExceptionHelper.LineNumber(ex);
//                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Added Items WebService", ex.Message.ToString());
//                return "";
//            }
//        }

		public static int ToInt(string toParse)
		{
			int result;
			if (int.TryParse(toParse, out result)) return result;

			return result;
		}
	}
}
