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
using Newtonsoft.Json;
using System.Linq;

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
                string ItemCode = HttpContext.Current.Request.Params["ItemCode"]; // Item Code
                string ItemDesc = HttpContext.Current.Request.Params["iItemDescription"];
                string PartNumber = HttpContext.Current.Request.Params["iPartNumber"];
                string Specification = HttpContext.Current.Request.Params["iSpecification"];

                string ItemCat = HttpContext.Current.Request.Params["DDL_ItemCat"]; // Drop Down List Selected Item of Category
                string ItemSubCat = HttpContext.Current.Request.Params["DDL_ItemSubCat"]; /// Drop Down List Selected Item of SubCategory
                string ItemSubSubCat = HttpContext.Current.Request.Params["DDL_ItemSubSubCat"]; // Drop Down List Selected Item of Sub-SubCategory

                var sb = new StringBuilder();
                var whereClause = string.Empty;

                if (SelectedItems.Trim() != "")
                {
                    //sb.Append(" where dbo.FN_GetDescription(ItemMaster.CategoryID )= 'General' and ItemMaster.id not in (select * from dbo.SplitString('" + SelectedItems + "',','))");
                    sb.Append(" where (ItemMaster.id not in (select * from dbo.SplitString('" + SelectedItems + "',','))");
                    sb.Append(" and ISNULL(New_ItemCode, '') like '" + ItemCode + "%' and ItemDescription like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification,'') like '%" + Specification + "%')");
                }
                else
                    sb.Append(" Where (ISNULL(New_ItemCode, '') like '" + ItemCode + "%' and ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%')");

                if ((ItemCat != "" && ItemCat != Guid.Empty.ToString()) || (ItemSubCat != "" && ItemSubCat != Guid.Empty.ToString()) || (ItemSubSubCat != "" && ItemSubCat != Guid.Empty.ToString()))
                {
                    if (sb.ToString().ToLower().Contains("where"))
                        sb.Append(" and (CatRefID = '" + ItemCat + "' and SubCatRefID = '" + ItemSubCat + "' and SubSubCatRegID = '" + ItemSubSubCat + "')");
                    else
                        sb.Append(" Where (CatRefID = '" + ItemCat + "' and SubCatRefID = '" + ItemSubCat + "' and SubSubCatRegID = '" + ItemSubSubCat + "')");
                }
                sb.Replace(" and SubCatRefID = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and SubSubCatRegID = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and SubSubCatRegID = ''", "")
                    .Replace(" and SubCatRefID = ''", "");

                whereClause = sb.ToString();
                sb.Clear();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE ID LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR case when ISNULL(New_ItemCode,'') = '' then ItemDescription else New_ItemCode + '-' + ItemDescription end LIKE ");
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
                    orderByClause = "CreatedDate DESC";

                orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();
                //case when ISNULL(New_ItemCode,'') = '' then ItemDescription else CONVERT(nvarchar(100), format(New_ItemCode,'00000000000')) + '-' + ItemDescription end
                string query = @" declare @MA TABLE(ID uniqueidentifier, ItemDescription varchar(MAX), PartNumber varchar(MAX), Specification  varchar(MAX), 
								HSCode varchar(MAX)) INSERT INTO @MA ( ID, ItemDescription, PartNumber, Specification, HSCode ) 
									Select ID, Replace(Replace(ItemDescription,CHAR(13),''),CHAR(10),''), PartNumber, Specification, HSCode FROM [ItemMaster] {4}                   
									SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MA].ID) FROM @MA) AS TotalRows, 
									(SELECT  count( [@MA].ID) FROM @MA {1}) AS TotalDisplayRows, [@MA].ID, [@MA].ItemDescription, [@MA].PartNumber, 
									[@MA].Specification, [@MA].HSCode FROM @MA {1}) RawResults) Results WHERE RowNumber BETWEEN {2} AND {3}";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause + " and ItemMaster.IsActive <> 0 ");// and New_ItemCode like '80%' ");
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
                    sb.AppendFormat(@"""1"": ""{0}""", ItemDescription.Replace(Environment.NewLine, "\\n").Replace("\r", "\\r")); // New Line
                    sb.Append(",");

                    string PartNo = data["PartNumber"].ToString().Replace("\"", "\\\"");
                    PartNo = PartNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PartNo.Replace(Environment.NewLine, "\\n").Replace("\r", "\\r"));
                    sb.Append(",");

                    string Spec = data["Specification"].ToString().Replace("\"", "\\\"");
                    Spec = Spec.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", Spec.Replace(Environment.NewLine, "\\n").Replace("\r", "\\r"));

                    sb.Append(",");

                    string HsCode = data["HSCode"].ToString().Replace("\"", "\\\"");
                    HsCode = HsCode.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", HsCode.Replace(Environment.NewLine, "\\n").Replace("\r", "\\r"));
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

                string query = "";


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
                    s.Append(" and f.EnquireNumber LIKE '" + FeNo + "%'");
                if (RDate != null && RDate != "")
                {
                    DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                    DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and f.ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Subject != "")
                    s.Append(" and f.Subject LIKE '" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusID) LIKE '" + Status.Replace("'", "''") + "%'");
                if (Dept != "")
                    s.Append(" and dbo.FN_GetDescription(f.DepID) LIKE '" + Dept + "%'");
                if (Cust != "")
                    s.Append(" and dbo.GetAllCustomerName(f.CusID) LIKE '" + Cust + "%'");
                if (CnctPrsn != "")
                    s.Append(" and dbo.FN_GetCustContactPerson(f.CusID) LIKE '" + CnctPrsn + "%'");

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

                if (FeNo != "" || (date != null && date != "") || (RDate != null && RDate != "") || Subject != "" || Dept != "" || Cust != "" || CnctPrsn != "" || Status != "")
                {
                    query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), AMENDEDIT nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime,Export nvarchar(max), VendorIds nvarchar(max),
                            History nvarchar(MAX))				

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History) 
							 Select EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History  from View_ForeignEnquiry  f
                               {4} 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].AMENDEDIT,[@MAA].EDIT, [@MAA].Delt, [@MAA].Export, [@MAA].VendorIds, [@MAA].History FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                }
                else
                {
                    query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), AMENDEDIT nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime,Export nvarchar(max), VendorIds nvarchar(max),
                            History nvarchar(MAX))				

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History) 
							 Select top 500 EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History  from View_ForeignEnquiry  f
                               {4} order by CreatedDate desc
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].AMENDEDIT,[@MAA].EDIT, [@MAA].Delt, [@MAA].Export, [@MAA].VendorIds, [@MAA].History FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                }
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"] == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where :
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103)" + s.ToString()
                                + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                    " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() +
                                    "' AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where :
                                    " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() + "' " + s.ToString()
                                    + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where);
                        }
                    }
                }
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        " WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " : " f.IsActive <> 0 and (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + "AND")
                        + " f.custId in (Select Data from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and f.CompanyId = '" + Session["CompanyID"] + " '"
                        : (s.ToString() == "" ? " AND f.IsActive <> 0 " : " AND f.IsActive <> 0  " + s.ToString() + "AND") +
                        " f.custId in (Select Data from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ','))  and f.CompanyId = '" + Session["CompanyID"] + "'");
                }

                else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString() && (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   ("Where ((f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "')")//Where(isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
                   : (" where f.IsActive <> 0  " //AND (isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                   + s.ToString() + " and  ((f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "')")); //+ Where
                }
                else if (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   ("Where (f.CompanyId = " + "'" + CompanyID + "'" + "  and f.CompanyId = f.VendorIds " + "  or f.VendorIds = " + "'" + CompanyID + "')")//Where(isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
                   : (" where f.IsActive <> 0  " //AND (isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                   + s.ToString() + " and  (f.CompanyId = " + "'" + CompanyID + "'" + " and f.CompanyId = f.VendorIds " + "  or f.VendorIds = " + "'" + CompanyID + "')")); //+ Where
                }
                else
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
                        //Modified by Satya
                   (" Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + Where)//"Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') or "
                        // (" Where " + Where)//"Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') or "
                   :
                        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
                   (" where f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + " and " + Where));
                //AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
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
                        sb.AppendFormat(@"""8"": ""<a href=FeAmendments.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
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


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFItemsTest()
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

                string query = "";
                string query1 = "";

                // HttpContext.Current.Request.Params["sPaginationType"];//HttpContext.Current.Request.Url.AbsoluteUri;
                StringBuilder s = new StringBuilder();
                if (date != null && date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" f.EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FeNo != "")
                    s.Append(" and f.EnquireNumber LIKE '" + FeNo + "%'");
                if (RDate != null && RDate != "")
                {
                    DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                    DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and f.ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Subject != "")
                    s.Append(" and f.Subject LIKE '" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusID) LIKE '" + Status.Replace("'", "''") + "%'");
                if (Dept != "")
                    s.Append(" and dbo.FN_GetDescription(f.DepID) LIKE '" + Dept + "%'");
                if (Cust != "")
                    s.Append(" and dbo.GetAllCustomerName(f.CusID) LIKE '" + Cust + "%'");
                if (CnctPrsn != "")
                    s.Append(" and dbo.FN_GetCustContactPerson(f.CusID) LIKE '" + CnctPrsn + "%'");

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

                //                if (FeNo != "" || (date != null && date != "") || (RDate != null && RDate != "") || Subject != "" || Dept != "" || Cust != "" || CnctPrsn != "" || Status != "")
                //                {
                //                    query = @"Select EnquireNumber '0', EnquiryDateS '1', ReceivedDateS '2', Subject '3', DepartmentId '4',CusmorId '5',ContactPerson '6',StatusTypeId '7',
                //                            AMENDEDIT '8',MAIL '9',AMENDEDIT '10',EDIT '11', Delt '12',Export '13', ForeignEnquireId DT_RowId, '' DT_RowClass from View_ForeignEnquiry_Test  f
                //                               {4}";
                //                }
                //                else
                //                {

                query = @"Select EnquireNumberS '0', EnquiryDateS '1', ReceivedDateS '2', Subject '3', DepartmentId '4',CusmorId '5',ContactPerson '6',StatusTypeId '7',
                            History '8',MAIL '9',AMENDEDIT '10',EDIT '11', Delt '12',Export '13', ForeignEnquireId DT_RowId, '' DT_RowClass from View_ForeignEnquiry_Test  f
                               {4} order by CreatedDate desc";

                query1 = @"Select EnquireNumberS '0', EnquiryDateS '1', ReceivedDateS '2', Subject '3', DepartmentId '4',CusmorId '5',ContactPerson '6',StatusTypeId '7',
                            History '8','' '9',AMENDEDIT '10',EDIT '11', Delt '12',Export '13', ForeignEnquireId DT_RowId, '' DT_RowClass from View_ForeignEnquiry_Test  f
                               {4} order by CreatedDate desc";
                //}
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
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"] == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') <> '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000')  and " + Where :
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103)" + s.ToString()
                                + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') <> '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000')  and " + Where);
                        }
                        else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString() || (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                    " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() +
                                    "' AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where :
                                    " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.CreatedBy = '" + (Session["UserID"]).ToString() + "' " + s.ToString()
                                    + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " + Where);
                        }
                    }
                }
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query1, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        //" WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and " : " f.IsActive <> 0 and (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + "AND")
                     " WHERE " + (s.ToString() != "" ? s.ToString() : "f.IsActive <> 0 ") + " And" // +"?"+" s.ToString() AND ")
                    + " f.CreatedBy in (Select Data from dbo.SplitString('"
                      + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "', ',' )) and f.CompanyId = '" + Session["CompanyID"] + " '"
                      : (s.ToString() == "" ? " AND f.IsActive <> 0 " : " AND f.IsActive <> 0  " + s.ToString() + "AND") +
                      " f.CreatedBy in (Select Data from dbo.SplitString('"
                      + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "', ','))  and f.CompanyId = '" + Session["CompanyID"] + "'");
                }

                else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString() && (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   ("Where ((f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "')")//Where(isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
                   : (" where " //AND (isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                   + s.ToString() + " and  ((f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "')")); //+ Where
                }
                else if (CommonBLL.TraffickerContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   ("Where (f.CompanyId = " + "'" + CompanyID + "'" + "  and f.CompanyId = f.VendorIds " + "  or f.VendorIds = " + "'" + CompanyID + "')")//Where(isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
                   : (" where " //AND (isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                   + s.ToString() + " and  (f.CompanyId = " + "'" + CompanyID + "'" + " and f.CompanyId = f.VendorIds " + "  or f.VendorIds = " + "'" + CompanyID + "')")); //+ Where
                }
                else
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
                        //Modified by Satya
                   (" Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + Where)//"Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') or "
                        // (" Where " + Where)//"Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') or "
                   :
                        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
                   (" where f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + " and " + Where));
                //AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                s.Clear();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);


                int rows_returned;
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    rows_returned = sda.Fill(dt);
                    connection.Close();
                }

                DataTable dtt = dt;
                if (dt != null && dt.Rows.Count > 0)
                {
                    iDisplayLength = iDisplayLength == -1 ? dt.Rows.Count : iDisplayLength;
                    dtt = dtt.AsEnumerable().Skip(iDisplayStart).Take(iDisplayLength).CopyToDataTable();
                }
                string outputJson = string.Empty;
                if (dt != null && dtt != null)
                {
                    dynamic newtonresult = new
                    {
                        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                        iTotalRecords = dt.Rows.Count,
                        iTotalDisplayRecords = dt.Rows.Count,
                        aaData = dtt
                    };
                    outputJson = JsonConvert.SerializeObject(newtonresult);
                }
                else
                {
                    dynamic newtonresult = new
                    {
                        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = dtt
                    };
                    outputJson = JsonConvert.SerializeObject(newtonresult);
                }

                #region Not In Use
                //Response.Clear();
                //Response.ContentType = "application/json";
                //Response.Write(outputJson);


                //var totalDisplayRecords = "";
                //var totalRecords = "";

                //var rowClass = "";
                //var count = 0;


                //if (conn.State == ConnectionState.Closed)
                //    conn.Open();

                //var DB = new SqlCommand();
                //DB.Connection = conn;
                //DB.CommandText = query;
                //var data = DB.ExecuteReader();               

                //while (data.Read())
                //{
                //    if (totalRecords.Length == 0)
                //    {
                //        totalRecords = data["TotalRows"].ToString();
                //        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                //    }
                //    sb.Append("{");
                //    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ForeignEnquireId"].ToString()); // count++);
                //    sb.Append(",");
                //    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                //    sb.Append(",");

                //    string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                //    string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
                //    EnqNo = EnqNo.Replace("\t", "-"); // 
                //    sb.AppendFormat(@"""0"": ""<a href=FEDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
                //    sb.Append(",");

                //    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
                //    sb.Append(",");

                //    if (data["ReceivedDate"].ToString().Replace("\"", "\\\"") != CommonBLL.EndDate.ToString())
                //    {
                //        sb.AppendFormat(@"""2"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
                //        sb.Append(",");
                //    }
                //    else
                //    {
                //        sb.AppendFormat(@"""2"": ""{0}""", "");
                //        sb.Append(",");
                //    }

                //    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                //    Subjt = Subjt.Replace("\t", "-");
                //    sb.AppendFormat(@"""3"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");

                //    DeptID = DeptID.Replace("\t", "-");
                //    sb.AppendFormat(@"""4"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string CustID = data["CusmorId"].ToString().Replace("\"", "\\\"");

                //    CustID = CustID.Replace("\t", "-");
                //    sb.AppendFormat(@"""5"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string ContactPerson = data["ContactPerson"].ToString().Replace("\"", "\\\"");
                //    ContactPerson = ContactPerson.Replace("\t", "-");
                //    sb.AppendFormat(@"""6"": ""{0}""", ContactPerson.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string StatIDd = data["StatusTypeId"].ToString().Replace("\"", "\\\"");
                //    StatIDd = StatIDd.Replace("\t", "-");
                //    sb.AppendFormat(@"""7"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string Histroy = data["History"].ToString().Replace("\"", "\\\"");
                //    Histroy = Histroy.Replace("\t", "-");
                //    if (Histroy != "")
                //    {
                //        sb.AppendFormat(@"""8"": ""<a href=FeAmendments.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
                //        sb.Append(",");
                //    }
                //    else
                //    {
                //        sb.AppendFormat(@"""8"": """"");
                //        sb.Append(",");
                //    }

                //    string Mail = data["MAIL"].ToString().Replace("\"", "\\\"");

                //    Mail = Mail.Replace("\t", "-");
                //    sb.AppendFormat(@"""9"": ""{0}""", Mail.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string AMENDEDIT = data["AMENDEDIT"].ToString().Replace("\"", "\\\"");

                //    AMENDEDIT = AMENDEDIT.Replace("\t", "-");
                //    sb.AppendFormat(@"""10"": ""{0}""", AMENDEDIT.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");
                //    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");

                //    Edit = Edit.Replace("\t", "-");
                //    sb.AppendFormat(@"""11"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");


                //    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                //    Del = Del.Replace("\t", "-");
                //    sb.AppendFormat(@"""12"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");

                //    string Export = data["Export"].ToString().Replace("\"", "\\\"");
                //    Export = Export.Replace("\t", "-");
                //    sb.AppendFormat(@"""13"": ""{0}""", Export.Replace(Environment.NewLine, "\\n"));
                //    sb.Append("},");
                //}
                //conn.Close(); 


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
                //outputJson = sb.Remove(sb.Length - 1, 1).ToString();
                //sb.Clear();

                //sb.Append("{");
                //sb.Append(@"""sEcho"": ");
                //sb.AppendFormat(@"""{0}""", sEcho);
                //sb.Append(",");
                //sb.Append(@"""iTotalRecords"": ");
                //sb.Append(totalRecords);
                //sb.Append(",");
                //sb.Append(@"""iTotalDisplayRecords"": ");
                //sb.Append(totalDisplayRecords);
                //sb.Append(", ");
                //sb.Append(@"""aaData"": [ ");
                //sb.Append(outputJson);
                //sb.Append("]}");
                //outputJson = sb.ToString();
                #endregion

                return outputJson.Replace("PIdetails.aspx", "FEDetails.aspx");
            }
            catch (SqlException ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Added Items WebService", ex.Message.ToString());
                return "";
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
        /// FOR FOREIGN ENQUIRY AMENDMENT
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFAItems()
        {
            try
            {
                string FEID = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["FEnqID"];
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
                string query = "";

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
                    s.Append(" and f.Subject LIKE '" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusTypeId) LIKE '" + Status.Replace("'", "''") + "%'");
                if (Dept != "")
                    s.Append(" and dbo.FN_GetDescription(f.DepartmentId) LIKE '" + Dept + "%'");
                if (Cust != "")
                    s.Append(" and dbo.GetAllCustomerName(f.CusmorId) LIKE '" + Cust + "%'");
                if (CnctPrsn != "")
                    s.Append(" and dbo.FN_GetCustContactPerson(f.CusmorId) LIKE '" + CnctPrsn + "%'");

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
                if (FeNo != "" || (date != null && date != "") || (RDate != null && RDate != "") || Subject != "" || Dept != "" || Cust != "" || CnctPrsn != "" || Status != "")
                {
                    query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), AMENDEDIT nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime,Export nvarchar(max), VendorIds nvarchar(max),
                            History nvarchar(MAX))				

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History) 
							 Select EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History  from View_ForeignEnquiry  f
                               {4} 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].AMENDEDIT,[@MAA].EDIT, [@MAA].Delt, [@MAA].Export, [@MAA].VendorIds, [@MAA].History FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                }
                else
                {
                    query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
							ContactPerson nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime ,VendorIds nvarchar(max), AmendmentID uniqueidentifier)				

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
								ContactPerson, CompanyId, CreatedDate,VendorIds, AmendmentID) 
							
                           Select EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , dbo.FN_GetStatus(StatusTypeId) as Status,
                           dbo.FN_GetDescription(DepartmentId) as DepartmentId,dbo.GetAllCustomerName(CusmorId) as Customer, 
						   dbo.FN_GetCustContactPerson(CusmorId) as ContactPerson, CompanyId, CreatedDate,VendorIds,ID  from FEAmndmnt f where IsActive <>0 
                               {4}
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate,[@MAA].VendorIds,[@MAA].AmendmentID FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                }
                string Where = "";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (FEID == null || FEID == "")
                {
                    FEID = Guid.Empty.ToString();
                }
                string Role = HttpContext.Current.Session["AccessRole"].ToString();
                Where = " and  ForeignEnquireId=" + "'" + new Guid(FEID) + "'" + " and IsActive <> 0";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ? Where : s.ToString() + Where);
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
                    string EnqId = data["AmendmentID"].ToString().Replace("\"", "\\\"");
                    EnqNo = EnqNo.Replace("\t", "-"); // 
                    sb.AppendFormat(@"""0"": ""<a href=FeAmdmntDtls.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
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
                    //s.Append(" and (dbo.FN_GetStatus(f.StatusID) + CASE WHEN (a.IsOffer = 1 and f.StatusID = 30) THEN ' and Offered to Customer' ELSE '' END) LIKE '%" + Status + "%'");
                    s.Append(" and dbo.FN_GetStatus(StatusID) LIKE '%" + Status.Replace("'", "''") + "%'");
                if (Dept != "")
                    s.Append(" and dbo.FN_GetDescription(f.DeptID) LIKE '%" + Dept + "%'");
                if (Ven != "")
                    s.Append(" and dbo.FN_GetVendorBussName(ISNULL(f.VendorId,'00000000-0000-0000-0000-000000000000')) LIKE '%" + Ven + "%'");
                if (Cust != "")
                    s.Append(" and dbo.GetAllCustomerName(f.CusID) LIKE '%" + Cust + "%'");
                if (CnctPrsn != "")
                    s.Append(" and dbo.FN_GetCustContactPerson(f.CusID) LIKE '%" + CnctPrsn + "%'");

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
			     
        

							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, VendorIds,CusmorId, 
								ContactPerson, MAIL, EDIT, Delt, CompanyId, CreatedDate) 
							 SELECT EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId, StatusTypeId, DepartmentId, VendorIds,CusmorId, ContactPerson, MAIL, 
								EDIT, Delt, CompanyId, CreatedDate FROM View_FloatForeignEnquiry f 
                                 {4}
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].VendorIds, [@MAA].CusmorId, 
								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].EDIT, [@MAA].Delt FROM @MAA {1}) RawResults) Results WHERE 
								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                string Where = "";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (new Guid(Session["CompanyID"].ToString()) != null && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                    Where = "Where (f.CompanyId = " + "'" + CompanyID + "' or f.VendorId = " + "'" + CompanyID + "'" + ") and f.ReceivedDate <> '" + CommonBLL.EndDate.ToString("yyyy-MM-dd") +
                        "' and ISNULL(f.VendorId,'00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000'  and ";
                string Role = HttpContext.Current.Session["AccessRole"].ToString();
                Where += " f.IsActive <> 0 ";

                if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        " WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND " :
                        " f.IsActive <> 0  and " + s.ToString() + " and ")
                        + " f.custId in (Select * from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and f.CompanyId = '" + Session["CompanyID"] + " ' and VendorId <>'00000000-0000-0000-0000-000000000000' "
                        : (s.ToString() == "" ? " AND b.IsActive <> 0  and "
                        : " AND f.IsActive <> 0  and " + s.ToString() + "AND ") +
                        " f.custId in (Select * from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',')) and f.CompanyId = '" + Session["CompanyID"] + "' and VendorId <> '00000000-0000-0000-0000-000000000000' ");
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
                   ("" + Where) :
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

                string query = "";

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append("  l.LEIssueDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (LeNo != "")
                    s.Append("and l.EnquireNumber LIKE '%" + LeNo + "%'");
                if (FeNo != "")
                    s.Append(" and dbo.FN_FEnquiryNumber(l.FEId) LIKE '%" + FeNo + "%'");
                if (Subject != "")
                    s.Append(" and l.Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                {
                    string isofrstr = "and Offered to Customer".ToLowerInvariant();
                    if (isofrstr.Contains(Status))
                        s.Append("and (lq.IsOffer = 1 and fe.StatusId = 30)");
                    else
                        s.Append(" and dbo.FN_GetStatus(l.StatId) LIKE '%" + Status.Replace("'", "''") + "%'");
                }
                if (Supplier != "")
                    s.Append(" and dbo.FN_GetSupplierNm(l.SupplierId) LIKE '%" + Supplier + "%'");
                if (Customer != "")
                    s.Append(" and dbo.GetActiveCustomerName(l.CusId) LIKE '%" + Customer + "%'");

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
                string Top500 = "";
                if (LeNo == "" && (date == null || date == "") && FeNo == "" && Subject == "" && Supplier == "" && Customer == "" && Status == "")
                    Top500 = " top 500 ";

                query = @"  
							declare @MAA TABLE(CreatedDate datetime,LEIssueDate datetime, EnquireNumber nvarchar(400), ForeignEnquireId nvarchar(MAX), Subject nvarchar(MAX), 
							LocalEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), SupplierIds nvarchar(MAX), CustomerName nvarchar(MAX), MAIL nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier)
							INSERT INTO
								@MAA (CreatedDate,LEIssueDate, EnquireNumber, ForeignEnquireId , Subject, LocalEnquireId, StatusTypeId, SupplierIds, CustomerName, MAIL, EDIT, Delt,CompanyId )
									Select {5} CreatedDate,LEIssueDate, EnquireNumber, ForeignEnquireId , Subject, LocalEnquireId, StatusTypeId, SupplierIds, CustomerName, MAIL, EDIT, Delt,CompanyId from View_LocalEnquiry l
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

                //                else
                //                {
                //                    query = @"  
                //							declare @MAA TABLE(CreatedDate datetime,LEIssueDate datetime, EnquireNumber nvarchar(400), ForeignEnquireId nvarchar(MAX), Subject nvarchar(MAX), 
                //							LocalEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), SupplierIds nvarchar(MAX), CustomerName nvarchar(MAX), MAIL nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier)
                //							INSERT INTO
                //								@MAA (CreatedDate,LEIssueDate, EnquireNumber, ForeignEnquireId , Subject, LocalEnquireId, StatusTypeId, SupplierIds, CustomerName, MAIL, EDIT, Delt,CompanyId )
                //									Select top 500 CreatedDate,LEIssueDate, EnquireNumber, ForeignEnquireId , Subject, LocalEnquireId, StatusTypeId, SupplierIds, CustomerName, MAIL, EDIT, Delt,CompanyId from View_LocalEnquiry l
                //                                  {1}{4}
                //
                //							SELECT *
                //							FROM
                //								(SELECT row_number() OVER ({0} ) AS RowNumber
                //									  , *
                //								 FROM
                //									 (SELECT (SELECT count([@MAA].LocalEnquireId)
                //											  FROM
                //												  @MAA) AS TotalRows
                //										   , ( SELECT  count( [@MAA].LocalEnquireId) FROM @MAA) AS TotalDisplayRows			   
                //										   ,[@MAA].CreatedDate
                //										   ,[@MAA].LEIssueDate
                //										   ,[@MAA].EnquireNumber      
                //										   ,[@MAA].ForeignEnquireId
                //										   ,[@MAA].Subject
                //										   ,[@MAA].LocalEnquireId
                //										   ,[@MAA].StatusTypeId
                //										   ,[@MAA].SupplierIds
                //										   ,[@MAA].CustomerName
                //										   ,[@MAA].MAIL
                //										   ,[@MAA].EDIT
                //										   ,[@MAA].Delt 
                //									  FROM
                //										  @MAA) RawResults) Results 
                //											WHERE
                //												RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";
                //                }

                string where = "";
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                if (new Guid(Session["CompanyID"].ToString()) != null && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                    where = " l.CompanyId = " + "'" + CompanyID + "'" + " and ";
                string Role = HttpContext.Current.Session["AccessRole"].ToString();
                where += " l.IsActive <> 0 order by l.CreatedDate desc ";
                if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103) and" + where :
                                " where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)" + s.ToString() + " and " + where, Top500);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                            (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'" + " and " + where :
                                " where CONVERT(nvarchar(12), l.CreatedDate,103)= CONVERT(nvarchar(12),GETDATE(),103)  and l.CreatedBy =" + "'" + Session["UserID"].ToString() + "'"
                                  + s.ToString() + " and " + where, Top500);
                        }
                    }
                    else if (Mode == "odt")
                    {
                        if (Session["IsUser"] == null || Session["IsUser"].ToString() == "0" || Session["IsUser"].ToString() == "")
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                            " where " + where : " where l.IsActive <> 0 " + s.ToString() + " and " + where, Top500);
                        }
                        else if (CommonBLL.CustmrContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString() ||
                                (CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                        {
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                                " where l.IsActive <> 0  and " + where :
                                " where l.IsActive <> 0  and " + where, Top500);
                        }
                    }
                }
                else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                   ("Where" + where) : (" where " + s.ToString() + " and " + where), Top500);
                }
                else
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        (Role != CommonBLL.SuperAdminRole ? " where " + where : " where " + where) :
                        (Role != CommonBLL.SuperAdminRole ? " where " + s.ToString() + " and " + where : " where " + where), Top500);
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
                string rawSearch = "";
                string rawSearch_1 = HttpContext.Current.Request.Params["sSearch"];
                string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
                string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
                string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification

                var sb = new StringBuilder();
                var whereClause = string.Empty;
                sb.Append(" Where dbo.FN_GetDescription(i.CategoryID) = 'General' and ItemDescription like '%" + rawSearch_1
                    + "%' or PartNumber like '%" + rawSearch_1 + "%' or Specification like '%" + rawSearch_1 + "%'");
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
									Select dbo.FN_GetDescription(CategoryID) CategoryID, ItemDescription, PartNumber , Specification, ID,IsSubItems, EDIT, Delt, CompanyId, CreatedDate from View_ItemMaster i 
                             {5} {4} 
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
                        SubItems = "Yes";
                    else
                        SubItems = "No";
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


        /// <summary>
        /// FOR Item Master
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetIMItems_forRevised()
        {
            try
            {

                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                string ItemCode = HttpContext.Current.Request.Params["ItemCode"]; // Item Code
                string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
                string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
                string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification
                string ItemCat = HttpContext.Current.Request.Params["DDL_ItemCat"]; // Drop Down List Selected Item of Category
                string ItemSubCat = HttpContext.Current.Request.Params["DDL_ItemSubCat"]; /// Drop Down List Selected Item of SubCategory
                string ItemSubSubCat = HttpContext.Current.Request.Params["DDL_ItemSubSubCat"]; // Drop Down List Selected Item of Sub-SubCategory
                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var whereClause = string.Empty;
                if (R_S_Alter != "")
                {
                    sb.Append(" Where (isnull(ItemDescription,'') like '%" + R_S_Alter + "%' or isnull(New_ItemCode,'') like '%" + R_S_Alter + "%' or isnull(PartNumber,'') like '%" + R_S_Alter + "%' or isnull(Specification,'') like '%" + R_S_Alter + "%')");
                }
                //if (R_S_Alter == "" && (ItemCode != "" || ItemDesc != "" || PartNumber != "" || Specification != ""))
                //{
                //sb.Append(" Where (ISNULL(New_ItemCode, '') like '%" + ItemCode + "%' and (ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%')");
                //}
                //if ((ItemCat != "" && ItemCat != Guid.Empty.ToString()) || (ItemSubCat != "" && ItemSubCat != Guid.Empty.ToString()) || (ItemSubSubCat != "" && ItemSubCat != Guid.Empty.ToString()))
                //{
                if (sb.ToString().ToLower().Contains("where"))
                {
                    sb.Append(" and ISNULL(New_ItemCode, '') like '" + ItemCode + "%' and ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%'");
                    sb.Append(" and CatRefID = '" + ItemCat + "' and SubCatRefID = '" + ItemSubCat + "' and SubSubCatRegID = '" + ItemSubSubCat + "'");
                }
                else
                {
                    sb.Append(" Where ISNULL(New_ItemCode, '') like '" + ItemCode + "%' and ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%'");
                    sb.Append(" and CatRefID = '" + ItemCat + "' and SubCatRefID = '" + ItemSubCat + "' and SubSubCatRegID = '" + ItemSubSubCat + "'");
                }
                //}
                sb.Replace(" and CatRefID = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and CatRefID = ''", "")
                    .Replace(" and SubCatRefID = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and SubSubCatRegID = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and SubSubCatRegID = ''", "")
                    .Replace(" and SubCatRefID = ''", "");
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
                    //sb.Append(" OR ItemDescription LIKE ");
                    //sb.Append(wrappedSearch);
                    sb.Append(" OR PartNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Specification LIKE ");
                    sb.Append("or NewItemCode like");
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
                    //orderByClause = orderByClause.Replace("0", ", ItemCategory ");
                    //orderByClause = orderByClause.Replace("1", ", ItemSubCategory");
                    //orderByClause = orderByClause.Replace("2", ", ItemSubSubCategory");
                    orderByClause = orderByClause.Replace("0", ", New_ItemCode");
                    orderByClause = orderByClause.Replace("1", ", ItemDescription");
                    orderByClause = orderByClause.Replace("2", ", PartNumber");
                    orderByClause = orderByClause.Replace("3", ", Specification");
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
                //ItemCategory nvarchar(max),ItemSubCategory nvarchar(max),ItemSubSubCategory nvarchar(max),
                //ItemCategory,ItemSubCategory,ItemSubSubCategory,
                //category,sbucategory,SubSubCategory,

                /* OLD Query 
                string query = @"declare @MAA TABLE(CategoryID nvarchar(400), NewItemCode nvarchar(100),ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
                ID nvarchar(400),SubItems bit, EDIT nvarchar(MAX), Delt nvarchar(MAX), CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
                @MAA ( CategoryID, NewItemCode,ItemDescription, PartNumber , Specification, ID,SubItems, EDIT, Delt, CompanyId, CreatedDate )
                Select dbo.FN_GetDescription(CategoryID) CategoryID, New_ItemCode,ItemDescription, PartNumber , Specification, ID,
                IsSubItems, EDIT, Delt, CompanyId, CreatedDate from View_ItemMaster i 
                {5} {4} 
                SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
                (SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].NewItemCode, [@MAA].ItemDescription, [@MAA].PartNumber, 
                [@MAA].Specification, [@MAA].ID, [@MAA].SubItems, [@MAA].EDIT, [@MAA].Delt, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
                WHERE RowNumber BETWEEN {2} AND {3} "; **/

                //[@MAA].ItemCategory,  [@MAA].ItemSubCategory,  [@MAA].ItemSubSubCategory,
                //Guid CompanyId = new Guid(Session["CompanyID"].ToString());


                string query = "select * from(Select row_number() OVER ({0}) AS RowNumber,null AS TotalRows,null AS TotalDisplayRows,"
                    + "dbo.FN_GetDescription(CategoryID) CategoryID, New_ItemCode as NewItemCode,ItemDescription, PartNumber , Specification, i.ID,IsSubItems as SubItems,"
                    + " i.CompanyId, i.CreatedDate from ItemMaster i inner join Users u on u.ID = i.CreatedBy " +
                "{4}) as A where A.RowNumber BETWEEN {2} AND {3}";

                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0 " : whereClause + " and i.IsActive <> 0 ", Where);//and IsNull(i.New_ItemCode,'') not like '80%'

                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var DB = new SqlCommand();
                DB.Connection = conn;
                DB.CommandText = query;
                DB.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SqlCommandTimeOut"]);
                var data = DB.ExecuteReader();
                string queryc = "SELECT count(ID)  AS TotalRows FROM ItemMaster i {4}";
                queryc = String.Format(queryc, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0" : whereClause + " and i.IsActive <> 0", Where);//and IsNull(i.New_ItemCode,'') not like '80%'
                SqlConnection con = new SqlConnection(connectionString);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCommand _SCmd = new SqlCommand();
                _SCmd.CommandText = queryc;
                _SCmd.Connection = con;

                var totalDisplayRecords = Convert.ToString(_SCmd.ExecuteScalar());
                var totalRecords = totalDisplayRecords;
                if (con.State == ConnectionState.Open)
                    con.Close();
                string outputJson = string.Empty;
                var rowClass = "";
                while (data.Read())
                {
                    //if (totalRecords.Length == 0)
                    //{
                    //    totalRecords = data["TotalRows"].ToString();
                    //    totalDisplayRecords = data["TotalDisplayRows"].ToString();
                    //}
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    //sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["ItemCategory"]);
                    //sb.Append(",");
                    //sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["ItemSubCategory"]);
                    //sb.Append(",");
                    //sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["ItemSubSubCategory"]);
                    //sb.Append(",");
                    string itmCode = data["NewItemCode"].ToString().Replace("\"", "\\\"");
                    itmCode = itmCode.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", itmCode.Replace(Environment.NewLine, "\\n"));
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
                        SubItems = "Yes";
                    else
                        SubItems = "No";
                    sb.AppendFormat(@"""4"": ""{0}""", SubItems);
                    sb.Append(",");
                    if (Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                        sb.AppendFormat(@"""5"": ""{0}""", "<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this) />");// data["EDIT"].ToString());
                    else
                        sb.AppendFormat(@"""5"": ""{0}""", "");
                    sb.Append(",");
                    //string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    //Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", "<img src=../images/delete.png alt=Delete onclick=Delet(this,'" + data["ID"].ToString() + "',1) />");//Del.Replace(Environment.NewLine, "\\n"));
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
                if (sb.Length > 0)
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
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetIMItems_PreviousRates()
        {
            try
            {
                ItemDetailsBLL IDBLL = new ItemDetailsBLL();
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                // string ItemCode = HttpContext.Current.Request.Params["iItemCode"]; // Item Code
                string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
                string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
                string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification
                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var whereClause = string.Empty;
                string query = "";
                //if (R_S_Alter != "")
                //{
                //    sb.Append(" Where (isnull(ItemDescription,'') like '%" + R_S_Alter + "%' or isnull(New_ItemCode,'') like '%" + R_S_Alter + "%' or isnull(PartNumber,'') like '%" + R_S_Alter + "%' or isnull(Specification,'') like '%" + R_S_Alter + "%')");
                //}

                sb.Append(" Where (ISNULL(ItemDescription, '') like '%" + ItemDesc + "%' and ISNULL(PartNumber,'') like '%" + PartNumber + "%' and ISNULL(Specification, '') like '%" + Specification + "%')");

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
                    //sb.Append(" OR ItemDescription LIKE ");
                    //sb.Append(wrappedSearch);
                    sb.Append(" OR PartNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Specification LIKE ");
                    sb.Append("or NewItemCode like");
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
                    //orderByClause = orderByClause.Replace("0", ", ItemCategory ");
                    //orderByClause = orderByClause.Replace("1", ", ItemSubCategory");
                    //orderByClause = orderByClause.Replace("2", ", ItemSubSubCategory");
                    orderByClause = orderByClause.Replace("0", ", ItemDescription");
                    orderByClause = orderByClause.Replace("1", ", PartNumber");
                    orderByClause = orderByClause.Replace("2", ", Specification");
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
                if (ItemDesc != "" || PartNumber != "" || Specification != "")
                {
                    query = "select * from(Select row_number() OVER ({0}) AS RowNumber,null AS TotalRows,null AS TotalDisplayRows,"
                       + "dbo.FN_GetDescription(CategoryID) CategoryID, New_ItemCode as NewItemCode,ItemDescription, PartNumber , Specification, i.ID,IsSubItems as SubItems,"
                       + " i.CompanyId, i.CreatedDate from ItemMaster i inner join Users u on u.ID = i.CreatedBy " +
                   "{4}) as A where A.RowNumber BETWEEN {2} AND {3}";
                }
                else
                {
                    query = "select top 100 * from(Select row_number() OVER ({0}) AS RowNumber,null AS TotalRows,null AS TotalDisplayRows,"
                           + "dbo.FN_GetDescription(CategoryID) CategoryID, New_ItemCode as NewItemCode,ItemDescription, PartNumber , Specification, i.ID,IsSubItems as SubItems,"
                           + " i.CompanyId, i.CreatedDate from ItemMaster i inner join Users u on u.ID = i.CreatedBy " +
                       "{4}) as A where A.RowNumber BETWEEN {2} AND {3}";
                }
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0 " : whereClause + " and i.IsActive <> 0 ", Where);//and IsNull(i.New_ItemCode,'') not like '80%'

                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlConnection conn = new SqlConnection(connectionString);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var DB = new SqlCommand();
                DB.Connection = conn;
                DB.CommandText = query;
                DB.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SqlCommandTimeOut"]);
                var data = DB.ExecuteReader();
                string queryc = "SELECT count(ID)  AS TotalRows FROM ItemMaster i {4}";
                queryc = String.Format(queryc, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0" : whereClause + " and i.IsActive <> 0", Where);//and IsNull(i.New_ItemCode,'') not like '80%'
                SqlConnection con = new SqlConnection(connectionString);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlCommand _SCmd = new SqlCommand();
                _SCmd.CommandText = queryc;
                _SCmd.Connection = con;

                var totalDisplayRecords = Convert.ToString(_SCmd.ExecuteScalar());
                var totalRecords = totalDisplayRecords;
                if (con.State == ConnectionState.Open)
                    con.Close();
                string outputJson = string.Empty;
                var rowClass = "";

                while (data.Read())
                {
                    sb.Append("{");
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    string itmCode = data["NewItemCode"].ToString().Replace("\"", "\\\"");
                    itmCode = itmCode.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", itmCode.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string itm = data["ItemDescription"].ToString().Replace("\"", "\\\"");
                    itm = itm.Replace("\t", "-").Replace("\\P","");
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
                        SubItems = "Yes";
                    else
                        SubItems = "No";
                    sb.AppendFormat(@"""4"": ""{0}""", SubItems);
                    sb.Append(",");
                    // DataSet ds = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagQSelect, Guid.Empty, new Guid(data["ID"].ToString()), Guid.Empty, Guid.Empty,
                    //  Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now,
                    // true, Guid.Empty);
                    //  if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    sb.AppendFormat(@"""5"": ""{0}""", "<img src=../images/EXCEL.png style='cursor: pointer;' class= 'Exp' value='" + data["ID"].ToString() + "'/>");
                    //  else
                    //    sb.AppendFormat(@"""5"": ""{0}""", "NA");
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
                if (sb.Length > 0)
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
        public string GetIMItems_forRevisedFor_Customer()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Session["HFITemsValues_PI"].ToString();
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                // string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
                // string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
                // string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification
                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var whereClause = string.Empty;

                string Category = HttpContext.Current.Request.Params["sSearch_0"];
                string SCategory = HttpContext.Current.Request.Params["sSearch_1"];
                string SSCategory = HttpContext.Current.Request.Params["sSearch_2"];
                string ItemDescription = HttpContext.Current.Request.Params["sSearch_3"];
                string PartNo = HttpContext.Current.Request.Params["sSearch_4"];
                string Spec = HttpContext.Current.Request.Params["sSearch_5"];

                if (SelectedItems.Trim() != "")
                {
                    sb.Append(" and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',','))");

                    #region NotInUse
                    //if (R_S_Alter != "")//|| (ItemDesc != "" & PartNumber != "" & Specification != "")
                    //{
                    //    sb.Append(" and (dbo.FN_GetItemCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),2)) like  '%" + R_S_Alter.Trim()
                    //        + "%' or  dbo.FN_GetItemSubCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),4)) like '%" + R_S_Alter.Trim()
                    //        + "%' or dbo.FN_GetItemSubSubCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),6)) like '%" + R_S_Alter.Trim()
                    //        + "%' or ItemDescription like '%" + R_S_Alter.Trim() + "%' or PartNumber like '%" + R_S_Alter.Trim() + "%' or Specification like '%" + R_S_Alter.Trim() + "%') and i.ID not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',','))");
                    //    //sb.Append(" and dbo.FN_GetItemCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),2)) like  '%" + R_S_Alter.Trim()
                    //    //   + "%' and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',',')) or dbo.FN_GetItemSubCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),4)) like '%" + R_S_Alter.Trim()
                    //    //   + "%' and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',',')) or dbo.FN_GetItemSubSubCategory(dbo.Get_Split_New_Item_Code(ISNULL(New_ItemCode,''),6)) like '%" + R_S_Alter.Trim()
                    //    //   + "%'and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',',')) or ItemDescription like '%" + R_S_Alter.Trim()
                    //    //   + "%' and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',',')) or PartNumber like '%" + R_S_Alter.Trim()
                    //    //   + "%' and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',',')) or Specification like '%" + R_S_Alter.Trim()
                    //    //   + "%' and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',','))");
                    //} 
                    #endregion

                    if (Category != "")
                        sb.Append(" and i.category like '%" + Category + "%'");
                    if (SCategory != "")
                        sb.Append(" and i.sbucategory like '%" + SCategory + "%'");
                    if (SSCategory != "")
                        sb.Append(" and i.SubSubCategory like '%" + SSCategory + "%'");
                    if (ItemDescription != "")
                        sb.Append(" and ItemDescription like '%" + ItemDescription + "%'");
                    if (PartNo != "")
                        sb.Append(" and PartNumber like '%" + PartNo + "%'");
                    if (Spec != "")
                        sb.Append(" and Specification like '%" + Spec + "%'");
                    whereClause = sb.ToString();
                }
                else
                {
                    if (Category != "")
                        sb.Append(" and  i.category like '%" + Category + "%'");
                    if (SCategory != "")
                        sb.Append(" and i.sbucategory like '%" + SCategory + "%'");
                    if (SSCategory != "")
                        sb.Append(" and i.SubSubCategory like '%" + SSCategory + "%'");
                    if (ItemDescription != "")
                        sb.Append(" and ItemDescription like '%" + ItemDescription + "%'");
                    if (PartNo != "")
                        sb.Append(" and PartNumber like '%" + PartNo + "%'");
                    if (Spec != "")
                        sb.Append(" and Specification like '%" + Spec + "%'");
                    whereClause = sb.ToString();
                    //whereClause = sb.ToString();
                }
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
                    orderByClause = orderByClause.Replace("0", ", ItemCategory ");
                    orderByClause = orderByClause.Replace("1", ", ItemSubCategory");
                    orderByClause = orderByClause.Replace("2", ", ItemSubSubCategory");
                    orderByClause = orderByClause.Replace("3", ", ItemDescription");
                    orderByClause = orderByClause.Replace("4", ", PartNumber");
                    orderByClause = orderByClause.Replace("5", ", Specification");
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
                string query = "";
                //if (Session["CompanyID"] != null && HttpContext.Current.Session["CompanyID"].ToString() != "" && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                //    Where = " and i.CompanyId = '" + CompanyID + "'";
                if (ItemDescription != "" || PartNo != "" || Spec != "")
                {
                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
							ID nvarchar(400), itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max), SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID, itemcategory,itemsubcategory,itemsubsubcategory, SubItems, CompanyId, CreatedDate )
									Select '' as CategoryID, ItemDescription, PartNumber , Specification, ID, i.category,i.sbucategory,i.SubSubCategory,
                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                             {5} {4} 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
							[@MAA].Specification, [@MAA].ID, [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory,  [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
							WHERE RowNumber BETWEEN {2} AND {3} ";
                }
                //i.category,i.sbucategory,i.SubSubCategory,
                else
                {
                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
							ID nvarchar(400), itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max), SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID, itemcategory,itemsubcategory,itemsubsubcategory,SubItems, CompanyId, CreatedDate )
									Select '' as CategoryID, ItemDescription, PartNumber , Specification, ID, i.category,i.sbucategory,i.SubSubCategory,
                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                             {5} {4} 
							SELECT  * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
							[@MAA].Specification, [@MAA].ID, [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory,  [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
							WHERE RowNumber BETWEEN {2} AND {3} ";

                    //                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
                    //							ID nvarchar(400),SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
                    //								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID,SubItems, CompanyId, CreatedDate )
                    //									Select top 100 '' as CategoryID, ItemDescription, PartNumber , Specification, ID,
                    //                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                    //                             {5} {4} 
                    //							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
                    //							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
                    //							[@MAA].Specification, [@MAA].ID,   [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
                    //							WHERE RowNumber BETWEEN {2} AND {3} ";

                    //                    query = @"declare @maa table(categoryid nvarchar(400), itemdescription nvarchar(max), partnumber nvarchar(max), specification nvarchar(max), 
                    //                							id nvarchar(400),itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max),subitems bit, edit nvarchar(max), delt nvarchar(max), companyid uniqueidentifier, createddate datetime) insert into
                    //                								@maa (categoryid, itemdescription, partnumber , specification, id,itemcategory,itemsubcategory,itemsubsubcategory,subitems, edit, delt, companyid, createddate )
                    //                									select dbo.fn_getdescription(categoryid) categoryid, itemdescription, partnumber , specification, id,
                    //                                                    dbo.fn_getitemcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),2)),
                    //                                                    dbo.fn_getitemsubcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),4))
                    //                                                    ,dbo.fn_getitemsubsubcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),6)),
                    //                                                    issubitems, edit, delt, companyid, createddate from view_itemmaster i 
                    //                                             {5} {4} 
                    //                							select * from (select row_number() over ({0}) as rownumber, * from (select (select count([@maa].id) from @maa) as totalrows, 
                    //                							(select count( [@maa].id) from @maa {1}) as totaldisplayrows, [@maa].categoryid, [@maa].itemdescription, [@maa].partnumber, 
                    //                							[@maa].specification, [@maa].id,  [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory, [@maa].subitems, [@maa].edit, [@maa].delt, [@maa].companyid, [@maa].createddate from @maa {1}) rawresults) results 
                    //                							where rownumber between {2} and {3} ";
                }
                //Guid CompanyId = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0 order by i.CreatedDate DESC " : "Where i.IsActive <> 0" + whereClause + " and i.IsActive <> 0 order by i.CreatedDate DESC ", Where);



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
                int Rows = 0;
                //if (totalRecords.Length == 0)
                //{
                //    totalRecords = data["TotalRows"].ToString();
                //    totalDisplayRecords = data["TotalDisplayRows"].ToString();
                //}

                while (data.Read())
                {
                    Rows++;
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
                    sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["ItemCategory"]);
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["ItemSubCategory"]);
                    sb.Append(",");
                    sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["ItemSubSubCategory"]);
                    sb.Append(",");
                    string itm = data["ItemDescription"].ToString().Replace("\"", "\\\"");
                    itm = itm.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", itm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string RecDt = data["PartNumber"].ToString().Replace("\"", "\\\"");
                    RecDt = RecDt.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string Subjt = data["Specification"].ToString().Replace("\"", "\\\"");
                    Subjt = Subjt.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    string SubItems = "";
                    if (Convert.ToBoolean(data["SubItems"].ToString()))
                        SubItems = "Yes";
                    else
                        SubItems = "No";
                    sb.AppendFormat(@"""6"": ""{0}""", SubItems);
                    //                    sb.Append(",");
                    sb.Append("},");
                    //if (Rows == 100)
                    //    break;
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
        public string GetIMItems_forRevisedFor_CustomerTest()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Session["HFITemsValues_PI"].ToString();
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                // string ItemDesc = HttpContext.Current.Request.Params["iItemDesc"]; // Item Description
                // string PartNumber = HttpContext.Current.Request.Params["iPartNo"]; // PartNumber
                // string Specification = HttpContext.Current.Request.Params["iSpec"]; // Specification

                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var whereClause = "";
                string outputJson = string.Empty;

                string Category = HttpContext.Current.Request.Params["sSearch_0"];
                string SCategory = HttpContext.Current.Request.Params["sSearch_1"];
                string SSCategory = HttpContext.Current.Request.Params["sSearch_2"];
                string ItemDescription = HttpContext.Current.Request.Params["sSearch_3"];
                string PartNo = HttpContext.Current.Request.Params["sSearch_4"];
                string Spec = HttpContext.Current.Request.Params["sSearch_5"];

                string CompanyID = HttpContext.Current.Session["CompanyID"].ToString();



                #region Not In Use
                ////List<ItemMasterDAL_Cust>
                //var itms = VomsBAL.Get_ItemMaster(sEcho, iDisplayLength, iDisplayStart, Category, SCategory, SSCategory, ItemDescription, PartNo, Spec, SelectedItems, new Guid(CompanyID));


                //if (itms != null && itms.Count > 0)
                //{
                //    dynamic newtonresult = new
                //    {
                //        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                //        iTotalRecords = itms.Count,
                //        iTotalDisplayRecords = itms.Count,
                //        aaData = itms
                //    };
                //    outputJson = JsonConvert.SerializeObject(newtonresult);
                //}
                //else
                //{
                //    dynamic newtonresult = new
                //    {
                //        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                //        iTotalRecords = 0,
                //        iTotalDisplayRecords = 0,
                //        aaData = itms
                //    };
                //    outputJson = JsonConvert.SerializeObject(newtonresult);
                //} 
                #endregion

                if (SelectedItems.Trim() != "")
                    sb.Append(" and i.id not in (select * from dbo.SplitString('" + SelectedItems.ToUpper() + "',','))");

                if (Category != "")
                    sb.Append(" and i.category like '%" + Category + "%'");
                if (SCategory != "")
                    sb.Append(" and i.sbucategory like '%" + SCategory + "%'");
                if (SSCategory != "")
                    sb.Append(" and i.SubSubCategory like '%" + SSCategory + "%'");
                if (ItemDescription != "")
                    sb.Append(" and ItemDescription like '%" + ItemDescription + "%'");
                if (PartNo != "")
                    sb.Append(" and PartNumber like '%" + PartNo + "%'");
                if (Spec != "")
                    sb.Append(" and Specification like '%" + Spec + "%'");
                whereClause = sb.ToString();
                sb.Clear();

                #region Not In Use
                //var filteredWhere = string.Empty;
                //var wrappedSearch = "'%" + rawSearch + "%'";
                //if (rawSearch.Length > 0)
                //{
                //    sb.Append(" WHERE CategoryID LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR ItemDescription LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR ItemDescription LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR PartNumber LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR Specification LIKE ");
                //    sb.Append(wrappedSearch);
                //    filteredWhere = sb.ToString();
                //}
                //sb.Clear();
                //string orderByClause = string.Empty;
                //sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                //sb.Append(" ");
                //sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                //orderByClause = sb.ToString();
                //if (!String.IsNullOrEmpty(orderByClause))
                //{
                //    orderByClause = orderByClause.Replace("0", ", ItemCategory ");
                //    orderByClause = orderByClause.Replace("1", ", ItemSubCategory");
                //    orderByClause = orderByClause.Replace("2", ", ItemSubSubCategory");
                //    orderByClause = orderByClause.Replace("3", ", ItemDescription");
                //    orderByClause = orderByClause.Replace("4", ", PartNumber");
                //    orderByClause = orderByClause.Replace("5", ", Specification");
                //    orderByClause = orderByClause.Remove(0, 1);
                //}
                //else
                //    orderByClause = "CategoryID DESC";
                //orderByClause = "ORDER BY " + orderByClause;
                //sb.Clear();
                //var numberOfRowsToReturn = "";
                //numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();
                //string Where = ""; 
                #endregion

                string query = "";

                query = @"Select  i.category '0', i.sbucategory '1', i.SubSubCategory '2', ItemDescription '3', PartNumber '4', Specification '5',
                                ID DT_RowId, '' DT_RowClass from View_ItemMaster_Customer i Where i.IsActive <> 0 {0}";

                //'' as '0',  ID '4',  IsSubItems '8', CompanyId '9', CreatedDate '10', i.category 

                #region NotInUse

                //                //if (Session["CompanyID"] != null && HttpContext.Current.Session["CompanyID"].ToString() != "" && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                //                //    Where = " and i.CompanyId = '" + CompanyID + "'";
                //                if (ItemDescription != "" || PartNo != "" || Spec != "")
                //                {
                //                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
                //							ID nvarchar(400), itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max), SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
                //								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID, itemcategory,itemsubcategory,itemsubsubcategory, SubItems, CompanyId, CreatedDate )
                //									Select '' as CategoryID, ItemDescription, PartNumber , Specification, ID, i.category,i.sbucategory,i.SubSubCategory,
                //                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                //                             {5} {4} 
                //							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
                //							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
                //							[@MAA].Specification, [@MAA].ID, [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory,  [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
                //							WHERE RowNumber BETWEEN {2} AND {3} ";
                //                }
                //                //i.category,i.sbucategory,i.SubSubCategory,
                //                else
                //                {
                //                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
                //							ID nvarchar(400), itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max), SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
                //								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID, itemcategory,itemsubcategory,itemsubsubcategory,SubItems, CompanyId, CreatedDate )
                //									Select '' as CategoryID, ItemDescription, PartNumber , Specification, ID, i.category,i.sbucategory,i.SubSubCategory,
                //                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                //                             {5} {4} 
                //							SELECT  * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
                //							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
                //							[@MAA].Specification, [@MAA].ID, [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory,  [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
                //							WHERE RowNumber BETWEEN {2} AND {3} ";

                //                    //                    query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemDescription nvarchar(MAX), PartNumber nvarchar(MAX), Specification nvarchar(MAX), 
                //                    //							ID nvarchar(400),SubItems bit, CompanyId uniqueidentifier, CreatedDate datetime) INSERT INTO
                //                    //								@MAA (CategoryID, ItemDescription, PartNumber , Specification, ID,SubItems, CompanyId, CreatedDate )
                //                    //									Select top 100 '' as CategoryID, ItemDescription, PartNumber , Specification, ID,
                //                    //                                  IsSubItems, CompanyId, CreatedDate from View_ItemMaster_Customer i 
                //                    //                             {5} {4} 
                //                    //							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].ID) FROM @MAA) AS TotalRows, 
                //                    //							(SELECT count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemDescription, [@MAA].PartNumber, 
                //                    //							[@MAA].Specification, [@MAA].ID,   [@MAA].SubItems, [@MAA].CompanyId, [@MAA].CreatedDate FROM @MAA {1}) RawResults) Results 
                //                    //							WHERE RowNumber BETWEEN {2} AND {3} ";

                //                    //                    query = @"declare @maa table(categoryid nvarchar(400), itemdescription nvarchar(max), partnumber nvarchar(max), specification nvarchar(max), 
                //                    //                							id nvarchar(400),itemcategory nvarchar(max),itemsubcategory nvarchar(max),itemsubsubcategory nvarchar(max),subitems bit, edit nvarchar(max), delt nvarchar(max), companyid uniqueidentifier, createddate datetime) insert into
                //                    //                								@maa (categoryid, itemdescription, partnumber , specification, id,itemcategory,itemsubcategory,itemsubsubcategory,subitems, edit, delt, companyid, createddate )
                //                    //                									select dbo.fn_getdescription(categoryid) categoryid, itemdescription, partnumber , specification, id,
                //                    //                                                    dbo.fn_getitemcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),2)),
                //                    //                                                    dbo.fn_getitemsubcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),4))
                //                    //                                                    ,dbo.fn_getitemsubsubcategory(dbo.get_split_new_item_code(isnull(new_itemcode,''),6)),
                //                    //                                                    issubitems, edit, delt, companyid, createddate from view_itemmaster i 
                //                    //                                             {5} {4} 
                //                    //                							select * from (select row_number() over ({0}) as rownumber, * from (select (select count([@maa].id) from @maa) as totalrows, 
                //                    //                							(select count( [@maa].id) from @maa {1}) as totaldisplayrows, [@maa].categoryid, [@maa].itemdescription, [@maa].partnumber, 
                //                    //                							[@maa].specification, [@maa].id,  [@maa].itemcategory,  [@maa].itemsubcategory,  [@maa].itemsubsubcategory, [@maa].subitems, [@maa].edit, [@maa].delt, [@maa].companyid, [@maa].createddate from @maa {1}) rawresults) results 
                //                    //                							where rownumber between {2} and {3} ";
                //                }
                //                //Guid CompanyId = new Guid(Session["CompanyID"].ToString()); 

                #endregion

                //orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, 
                //whereClause == "" ? " where i.IsActive <> 0 order by i.CreatedDate DESC " : "Where i.IsActive <> 0" + whereClause + " and i.IsActive <> 0 order by i.CreatedDate DESC ", Where

                query = String.Format(query, whereClause);


                #region Not In Use

                //SqlConnection conn = new SqlConnection(connectionString);
                //if (conn.State == ConnectionState.Closed)
                //    conn.Open();
                //var DB = new SqlCommand();
                //DB.Connection = conn;
                //DB.CommandText = query;
                //var data = DB.ExecuteReader();
                //var totalDisplayRecords = "";
                //var totalRecords = "";
                //string outputJson = string.Empty;
                //var rowClass = "";
                //int Rows = 0;
                ////if (totalRecords.Length == 0)
                ////{
                ////    totalRecords = data["TotalRows"].ToString();
                ////    totalDisplayRecords = data["TotalDisplayRows"].ToString();
                ////}

                //while (data.Read())
                //{
                //    Rows++;
                //    if (totalRecords.Length == 0)
                //    {
                //        totalRecords = data["TotalRows"].ToString();
                //        totalDisplayRecords = data["TotalDisplayRows"].ToString();
                //    }
                //    sb.Append("{");
                //    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
                //    sb.Append(",");
                //    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                //    sb.Append(",");
                //    sb.AppendFormat(@"""0"": ""{0:dd-MM-yyyy}""", data["ItemCategory"]);
                //    sb.Append(",");
                //    sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["ItemSubCategory"]);
                //    sb.Append(",");
                //    sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["ItemSubSubCategory"]);
                //    sb.Append(",");
                //    string itm = data["ItemDescription"].ToString().Replace("\"", "\\\"");
                //    itm = itm.Replace("\t", "-");
                //    sb.AppendFormat(@"""3"": ""{0}""", itm.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");
                //    string RecDt = data["PartNumber"].ToString().Replace("\"", "\\\"");
                //    RecDt = RecDt.Replace("\t", "-");
                //    sb.AppendFormat(@"""4"": ""{0}""", RecDt.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");
                //    string Subjt = data["Specification"].ToString().Replace("\"", "\\\"");
                //    Subjt = Subjt.Replace("\t", "-");
                //    sb.AppendFormat(@"""5"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                //    sb.Append(",");
                //    string SubItems = "";
                //    if (Convert.ToBoolean(data["SubItems"].ToString()))
                //        SubItems = "Yes";
                //    else
                //        SubItems = "No";
                //    sb.AppendFormat(@"""6"": ""{0}""", SubItems);
                //    //                    sb.Append(",");
                //    sb.Append("},");
                //    //if (Rows == 100)
                //    //    break;
                //}
                //conn.Close();
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
                //outputJson = sb.Remove(sb.Length - 1, 1).ToString();
                //sb.Clear();

                //sb.Append("{");
                //sb.Append(@"""sEcho"": ");
                //sb.AppendFormat(@"""{0}""", sEcho);
                //sb.Append(",");
                //sb.Append(@"""iTotalRecords"": ");
                //sb.Append(totalRecords);
                //sb.Append(",");
                //sb.Append(@"""iTotalDisplayRecords"": ");
                //sb.Append(totalDisplayRecords);
                //sb.Append(", ");
                //sb.Append(@"""aaData"": [ ");
                //sb.Append(outputJson);
                //sb.Append("]}");
                //outputJson = sb.ToString(); 

                #endregion


                int rows_returned;
                DataTable dt = new DataTable();
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    rows_returned = sda.Fill(dt);
                    connection.Close();
                }

                DataTable dtt = dt;
                if (dt != null && dt.Rows.Count > 0)
                {
                    iDisplayLength = iDisplayLength == -1 ? dt.Rows.Count : iDisplayLength;
                    dtt = dtt.AsEnumerable().Skip(iDisplayStart).Take(iDisplayLength).CopyToDataTable();
                }
                if (dt != null && dtt != null)
                {
                    dynamic newtonresult = new
                    {
                        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                        iTotalRecords = dt.Rows.Count,
                        iTotalDisplayRecords = dt.Rows.Count,
                        aaData = dtt
                    };
                    outputJson = JsonConvert.SerializeObject(newtonresult);
                }
                else
                {
                    dynamic newtonresult = new
                    {
                        sEcho = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = dtt
                    };
                    outputJson = JsonConvert.SerializeObject(newtonresult);
                }

                return outputJson;
            }
            catch (SqlException ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Added Items WebService", ex.Message.ToString());
                return "";
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
        public string GetIMItems_Customer()
        {

            try
            {
                string SelectedItems = HttpContext.Current.Session["HFITemsValues_PI"].ToString();
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Category = HttpContext.Current.Request.Params["sSearch_0"];
                string SCategory = HttpContext.Current.Request.Params["sSearch_1"];
                string SSCategory = HttpContext.Current.Request.Params["sSearch_2"];
                string ItemDescription = HttpContext.Current.Request.Params["sSearch_3"];
                string PartNo = HttpContext.Current.Request.Params["sSearch_4"];
                string Spec = HttpContext.Current.Request.Params["sSearch_5"];

                #region Connection
                var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                SqlDataReader dr = null;
                string json = "";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Json", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@sEcho", SqlDbType.NVarChar).Value = sEcho;
                        cmd.Parameters.Add("@TotalRows", SqlDbType.NVarChar).Value = iDisplayStart;
                        cmd.Parameters.Add("@TotalDisplayRows", SqlDbType.NVarChar).Value = iDisplayLength;
                        cmd.Parameters.Add("@SelectedItems", SqlDbType.NVarChar).Value = SelectedItems;
                        cmd.Parameters.Add("@ItemCategory", SqlDbType.NVarChar).Value = Category;
                        cmd.Parameters.Add("@itemsubcategory", SqlDbType.NVarChar).Value = SCategory;
                        cmd.Parameters.Add("@itemsubsubcategory", SqlDbType.NVarChar).Value = SSCategory;
                        cmd.Parameters.Add("@ItemDescription", SqlDbType.NVarChar).Value = ItemDescription;
                        cmd.Parameters.Add("@PartNumber", SqlDbType.NVarChar).Value = PartNo;
                        cmd.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = Spec;


                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            json = dr[0].ToString()
                                .Replace(Environment.NewLine, "\\n")
                                .Replace("'", "&#39;")
                                //.Replace("\"", '\\"')
                                //.Replace("\&", "\\&")
                                .Replace("\r", "\\r")
                                .Replace("\t", "\\t")
                                .Replace("\b", "\\b")
                                .Replace("\f", "\\f");

                            if (json.Trim() == "")
                            {
                                StringBuilder sb = new StringBuilder();
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
                                json = sb.ToString();
                            }
                        }
                    }
                }
                #endregion

                return json;
            }
            catch (Exception ex)
            {
                string Msg = ex.Message;
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
                    if (RDate != "~")
                    {
                        DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                        DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                        if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                            s.Append(" and ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                    }
                }
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and Status LIKE '%" + Status.Replace("'", "''") + "%'");
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
                    orderByClause = orderByClause.Replace("0", ", EnquiryDate");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "EnquiryDate Desc";
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
												RowNumber BETWEEN {2} AND {3} order by ReceivedDate Desc";

                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tldt")
                    {
                        if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                                " where f.IsActive <> 0 " + s.ToString());
                        if (Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                                " where f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'" + s.ToString());
                        else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' AND f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'");
                    }
                    else
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, " where f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'" + s.ToString());
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
                    sb.AppendFormat(@"""1"": ""<a href=FEStages.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
                    //sb.AppendFormat(@"""1"": ""<a href=FullDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));                    
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


        /// <summary>
        /// Status Page for Overview Page
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetFeOverview()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string Cust = HttpContext.Current.Request.Params["sSearch_0"];
                string Edate = HttpContext.Current.Request.Params["sSearch_1"];
                string FeNo = HttpContext.Current.Request.Params["sSearch_2"];
                string FPONoo = HttpContext.Current.Request.Params["sSearch_4"];
                string RDate = HttpContext.Current.Request.Params["sSearch_3"];
                string date = HttpContext.Current.Request.Params["sSearch_5"];
                string Dept = HttpContext.Current.Request.Params["sSearch_8"];
                string Status = HttpContext.Current.Request.Params["sSearch_7"];
                string Subject = HttpContext.Current.Request.Params["sSearch_6"];
                //string date = HttpContext.Current.Request.Params["sSearch_7"];
                string ModeWhere = "";

                if (Session["UserID"].ToString() != "1" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tldt")
                        ModeWhere = " and createdby =" + Session["UserID"];
                }

                StringBuilder s = new StringBuilder();
                if (Edate != "" && Edate != "~")
                {
                    DateTime FrmDt = Edate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(Edate.Split('~')[0].ToString());
                    DateTime EndDat = Edate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(Edate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (date != "" && date != "~")
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and FPODueDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FeNo != "")
                    s.Append(" and EnquireNumber LIKE '%" + FeNo + "%'");
                if (FPONoo != "")
                    s.Append(" and FPONumber LIKE '%" + FPONoo + "%'");
                if (RDate != "")
                {
                    if (RDate != "~")
                    {
                        DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                        DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                        if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                            s.Append(" and ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                    }
                }
                if (Subject != "")
                    s.Append(" and Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and Status LIKE '%" + Status.Replace("'", "''") + "%'");
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
                    sb.Append(" OR FPODueDate LIKE ");
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
                    orderByClause = orderByClause.Replace("0", ", EnquiryDate");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "EnquiryDate Desc";
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
							declare @MAA TABLE(ForeignEnquireId uniqueidentifier,EnquiryDate datetime,EnquireNumber varchar(500),FPONumber varchar(max),
							ReceivedDate datetime,FPODueDate datetime,Subject varchar(max),Status varchar(500),DepartmentId varchar(500),CustmrNm varchar(500),
							CreatedBy uniqueidentifier)
							INSERT
							INTO
								@MAA (ForeignEnquireId,EnquiryDate,EnquireNumber,FPONumber,ReceivedDate,f.FPODueDate,Subject,Status,DepartmentId,CustmrNm,CreatedBy)
										select f.ForeignEnquireId,f.EnquiryDate,f.EnquireNumber,f.FPONumber,f.ReceivedDate,f.FPODueDate,f.Subject,
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
										   ,[@MAA].FPODueDate
										   ,[@MAA].CustmrNm
										   ,[@MAA].CreatedBy 
                                           ,[@MAA].DepartmentId 
									  FROM
										  @MAA {1}) RawResults) Results WHERE
												RowNumber BETWEEN {2} AND {3} order by ReceivedDate Desc";

                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tldt")
                    {
                        if (Session["AccessRole"].ToString() == CommonBLL.SuperAdminRole)
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                                " where f.IsActive <> 0 " + s.ToString());
                        if (Session["AccessRole"].ToString() == CommonBLL.AdminRole)
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                                " where f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'" + s.ToString());
                        else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) || (CommonBLL.TraffickerContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn + s.ToString(),
                                "where f.CreatedBy = '" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[1].ToString() + "' AND f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'");
                    }
                    else
                        query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, " where f.IsActive <> 0 and f.CompanyId = '" + Session["CompanyID"] + "'" + s.ToString());
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
                    //sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
                    //sb.Append(",");

                    string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                    string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");
                    EnqNo = EnqNo.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""<a href=FEStages.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));
                    //sb.AppendFormat(@"""1"": ""<a href=FullDetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n"));                    
                    sb.Append(",");

                    string FPONo = data["FPONumber"].ToString().Replace("\"", "\\\"");
                    FPONo = FPONo.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", FPONo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["ReceivedDate"]);
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["EnquiryDate"]);
                    sb.Append(",");

                    string Subjt = data["Subject"].ToString().Replace("\"", "\\\"");
                    Subjt = Subjt.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", Subjt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string DeptID = data["DepartmentId"].ToString().Replace("\"", "\\\"");
                    DeptID = DeptID.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", DeptID.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string StatIDd = data["Status"].ToString().Replace("\"", "\\\"");
                    StatIDd = StatIDd.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", StatIDd.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""5"": ""{0:dd/MM/yyyy}""", data["FPODueDate"]);
                    sb.Append(",");

                    string CustID = data["CustmrNm"].ToString().Replace("\"", "\\\"");
                    CustID = CustID.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", CustID.Replace(Environment.NewLine, "\\n"));
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
        public string GetItemsCategory()
        {
            try
            {

                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                // string ItemCode = HttpContext.Current.Request.Params["iItemCode"]; // Item Code
                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var whereClause = string.Empty;
                if (R_S_Alter != "")
                {
                    sb.Append(" Where isnull(Code,'') like '%" + R_S_Alter + "%' or isnull(Description,'') like '%" + R_S_Alter + "%'");
                }
                whereClause = sb.ToString();
                sb.Clear();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR Description LIKE ");
                    sb.Append(wrappedSearch);
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
                    orderByClause = orderByClause.Replace("0", ", ItemCode ");
                    orderByClause = orderByClause.Replace("1", ", ItemDescription");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "Code DESC";
                orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();
                string Where = "";
                string CompanyID = HttpContext.Current.Session["CompanyID"].ToString();
                //if (Session["CompanyID"] != null && HttpContext.Current.Session["CompanyID"].ToString() != "" && new Guid(Session["CompanyID"].ToString()) != Guid.Empty)
                //    Where = " and i.CompanyId = '" + CompanyID + "'";

                string query = @"declare @MAA TABLE(CategoryID nvarchar(400), ItemCode nvarchar(100),ItemDescription nvarchar(MAX), EDIT nvarchar(MAX), Delt nvarchar(MAX), CreatedDate datetime, IsSpareParts bit, InItemMaster int) 
                                INSERT INTO @MAA ( CategoryID, ItemCode,ItemDescription, EDIT, Delt,CreatedDate, IsSpareParts, InItemMaster)
									Select Id, Code,Description,EDIT, Delt, CreatedDate, IsSpareParts, InItemMaster from View_ItemCategory i 
                             {5} {4} 
							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].CategoryID) FROM @MAA) AS TotalRows, 
							(SELECT count( [@MAA].CategoryID) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].CategoryID, [@MAA].ItemCode, [@MAA].ItemDescription, [@MAA].EDIT, [@MAA].Delt, 
                            [@MAA].CreatedDate, [@MAA].IsSpareParts, [@MAA].InItemMaster FROM @MAA {1}) RawResults) Results 
							WHERE RowNumber BETWEEN {2} AND {3} ";

                //Guid CompanyId = new Guid(Session["CompanyID"].ToString());
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause == "" ? " where i.IsActive <> 0 order by i.CreatedDate DESC " : whereClause + " and i.IsActive <> 0 order by i.CreatedDate DESC ", Where);

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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["CategoryID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");
                    sb.AppendFormat(@"""0"": ""{0}""", data["ItemCode"]);
                    sb.Append(",");
                    sb.AppendFormat(@"""1"": ""{0}""", data["ItemDescription"]);
                    sb.Append(",");
                    sb.AppendFormat(@"""2"": ""{0}""", data["IsSpareParts"].ToString());
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("EditDetails(this)", data["InItemMaster"].ToString() == "0" ? "EditDetails(this,0)" : "EditDetails(this,1)");
                    sb.AppendFormat(@"""3"": ""{0}""", Edit);
                    sb.Append(",");
                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Item Category WebService", ex.Message.ToString());
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
