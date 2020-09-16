using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Text;
using BAL;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace VOMS_ERP.Masters
{
    /// <summary>
    /// Summary description for ItemCategory
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ItemCategory : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetSubCategoryItems()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];
                string ItemCode = HttpContext.Current.Request.Params["ItemCode"]; // Item Code
                string iItemSubCatg = HttpContext.Current.Request.Params["iItemSubCatg"]; // Item Description
                string ItemSubDesc = HttpContext.Current.Request.Params["ItemSubDesc"]; // PartNumber

                string ItemCat = HttpContext.Current.Request.Params["DDL_ItemCat"]; // Drop Down List Selected Item of Category
                string ItemSubCat = HttpContext.Current.Request.Params["ItemSubCatgCode"]; /// Drop Down List Selected Item of SubCategory
                string ItemSubSubCat = HttpContext.Current.Request.Params["ItemDesc"]; // 

                string R_S_Alter = rawSearch;
                rawSearch = "";
                string query = "";
                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var whereClause = string.Empty;
                if (R_S_Alter != "")
                {
                    sb.Append(" Where (s.SCode like '%" + R_S_Alter + "%' or s.SDesc like '%" + R_S_Alter + "%' or s.CCode like '%" + R_S_Alter + "%' or s.CDesc like '%" + R_S_Alter + "%')");
                }

                if (sb.ToString().ToLower().Contains("where"))
                {
                    sb.Append(" and s.CCode like '" + ItemCode + "%' and s.SCode like '%" + iItemSubCatg + "%' and s.SDesc like '%" + ItemSubDesc + "%'");

                }
                else
                {
                    sb.Append("Where s.CCode like '" + ItemCode + "%' and s.SCode like '%" + iItemSubCatg + "%' and s.SDesc like '%" + ItemSubDesc + "%'");
                }

                if ((ItemCat != Guid.Empty.ToString()) || (ItemSubCat != "" && ItemSubSubCat != "") || (ItemSubSubCat != "" && ItemSubCat != ""))
                {
                    if (sb.ToString().ToLower().Contains("where"))
                    {
                        sb.Append(" and (s.RefCatId = '" + ItemCat + "')");
                    }
                    else
                    {
                        sb.Append(" Where (s.RefCatId = '" + ItemCat + "')");
                    }
                    if (ItemSubCat != "" && ItemSubSubCat != "")
                        sb.Append(" and s.SCode like '%" + ItemSubCat + "%' and s.SDesc like '%" + ItemSubSubCat + "%'");
                    else if (ItemSubCat != "")
                        sb.Append(" and s.SCode like '%" + ItemSubCat + "%'");
                    else if (ItemSubSubCat != "")
                        sb.Append(" and s.SDesc like '%" + ItemSubSubCat + "%'");
                }
                sb.Replace(" and s.RefCatId = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and s.RefCatId = ''", "");

                whereClause = sb.ToString();
                sb.Clear();
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE s.SCode LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.SDesc LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.CCode LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.CDesc LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                sb.Append(" ");
                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                //orderByClause = "0 DESC";
                orderByClause = sb.ToString();
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", SCode ");
                    orderByClause = orderByClause.Replace("1", ", SDesc ");
                    orderByClause = orderByClause.Replace("2", ", CCode ");
                    orderByClause = orderByClause.Replace("3", ", CDesc ");
                    orderByClause = orderByClause.Replace("4", ", IsSpareParts ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "SCode ASC";
                orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                //                query = @"select FORMAT(s.Code,'00') as '0', s.Description as '1', FORMAT(c.Code,'00') as '2', c.Description as '3', s.IsSpareParts '4',
                //                            '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '5', 
                //                            '<img src=../images/Delete.jpeg alt=Delete onclick=DeleteDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '6' 
                //                             from Sub_Category s join Category c on c.Id = s.RefCatId
                //                               {0} order by s.Code";


                query = @"select s.SCode as '0', s.SDesc as '1', s.CCode as '2', s.CDesc as '3', s.IsSpareParts as '4',  
                        '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''','+CONVERT(nvarchar(50),s.InItemMaster)+') />' as '5',
                        '<img src=../images/Delete.jpeg alt=Delete onclick=DeleteDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '6'
                        from ( select s.id, s.Code as SCode, s.Description as SDesc, c.Code as CCode, 
                        c.Description as CDesc, s.IsSpareParts, (select top 1 COUNT(i.SparesID) from ItemMaster i where i.SubCatRefID = s.ID) as InItemMaster,s.RefCatId  
                        from Sub_Category s join Category c on c.Id = s.RefCatId) as s {0} {1} ";

                query = String.Format(query, whereClause, orderByClause, filteredWhere, "", "", "");

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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetSubSubCategoryItems()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string ItemCode = HttpContext.Current.Request.Params["ItemCode"]; // Item Code
                string iItemSubCatg = HttpContext.Current.Request.Params["iItemSubCatg"]; // Item Description
                string ItemSubDesc = HttpContext.Current.Request.Params["ItemSubDesc"]; // PartNumber
                string ItemSubSubCatg = HttpContext.Current.Request.Params["ItemSubSubCatg"]; // PartNumber

                string ItemCat = HttpContext.Current.Request.Params["DDL_ItemCat"]; // Drop Down List Selected Item of Category
                string ItemSubSubCat = HttpContext.Current.Request.Params["ItemSubCatgCode"]; /// Drop Down List Selected Item of SubCategory
                string ItemSubSubCatDesc = HttpContext.Current.Request.Params["ItemDesc"]; // 
                string ItemSubCat = HttpContext.Current.Request.Params["DDL_ItemSubCat"]; // 

                string R_S_Alter = rawSearch;
                rawSearch = "";
                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var whereClause = string.Empty;
                if (R_S_Alter != "")
                {
                    sb.Append(" Where (c.Code like '%" + R_S_Alter + "%' or s.Code like '%" + R_S_Alter + "%' or sc.Code like '%" + R_S_Alter + "%' or s.Description like '%" + R_S_Alter + "%' or c.Description like '%" + R_S_Alter + "%')");
                }

                if (sb.ToString().ToLower().Contains("where"))
                {
                    sb.Append(" and c.Code like '" + ItemCode + "%' and s.Code like '%" + iItemSubCatg + "%' and sc.Code like '%" + ItemSubSubCatg + "%' and s.Description like '%" + ItemSubDesc + "%'");

                }
                else
                {
                    sb.Append("Where s.Code like '" + ItemCode + "%' and s.Code like '%" + iItemSubCatg + "%' and sc.Code like '%" + ItemSubSubCatg + "%' and s.Description like '%" + ItemSubDesc + "%'");
                }

                if ((ItemCat != Guid.Empty.ToString()) || (ItemSubCat != Guid.Empty.ToString() && ItemSubSubCat != "") || (ItemSubSubCat != "" && ItemSubCat != Guid.Empty.ToString()))
                {
                    if (sb.ToString().ToLower().Contains("where"))
                    {
                        if (ItemSubCat != Guid.Empty.ToString())
                            sb.Append(" and (s.RefCatId = '" + ItemCat + "') and (sc.Id = '" + ItemSubCat + "')");
                        else
                            sb.Append(" and (s.RefCatId = '" + ItemCat + "')");
                    }
                    else
                    {
                        if (ItemSubCat != Guid.Empty.ToString())
                            sb.Append(" where (s.RefCatId = '" + ItemCat + "') and (sc.Id = '" + ItemSubCat + "')");
                        else
                            sb.Append(" where (s.RefCatId = '" + ItemCat + "')");
                         
                    }
                    if (ItemSubDesc != "" && ItemSubSubCatg != "")
                        sb.Append(" and s.Code like '%" + ItemSubSubCat + "%' and s.Description like '%" + ItemSubSubCatDesc + "%'");
                    else if (ItemSubSubCatg != "")
                        sb.Append(" and s.Code like '%" + ItemSubSubCat + "%'");
                    else if (ItemSubDesc != "")
                        sb.Append(" and s.Description like '%" + ItemSubSubCatDesc + "%'");
                }
                sb.Replace(" and s.RefCatId = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and s.RefCatId = ''", "")
                    .Replace(" and sc.Id = '00000000-0000-0000-0000-000000000000'", "")
                    .Replace(" and sc.Id = ''", "");

                whereClause = sb.ToString();
                sb.Clear();

                string query = "";
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE s.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.Description LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR c.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR c.Description LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR sc.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR sc.Description LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                sb.Append(" ");
                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                //orderByClause = "0 DESC";
                orderByClause = sb.ToString(); 
                if (!String.IsNullOrEmpty(orderByClause))
                {
                    orderByClause = orderByClause.Replace("0", ", s.Code ");
                    orderByClause = orderByClause.Replace("1", ", s.Description ");
                    orderByClause = orderByClause.Replace("2", ", sc.Code ");
                    orderByClause = orderByClause.Replace("3", ", sc.Description ");
                    orderByClause = orderByClause.Replace("4", ", c.Code ");
                    orderByClause = orderByClause.Replace("5", ", c.Description ");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                    orderByClause = "s.Code ASC";
                orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                query = @"select s.Code as '0', s.Description as '1', sc.Code as '2', sc.Description as '3', c.Code as '4', c.Description as '5',
                            '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '6', 
                            '<img src=../images/Delete.jpeg alt=Delete onclick=DeleteDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '7',s.RefCatId,sc.Id  
                             from SubSubCategory s join Category c on c.Id = s.RefCatId join Sub_Category sc on sc.ID = s.RefSubCatId
                               {0} {1}";

                query = String.Format(query, whereClause, orderByClause, filteredWhere, "", "", "");

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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetMappedItems()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string query = "";
                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE c.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR c.Description LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.Description LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR sS.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR sS.Description LIKE ");
                    sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();



                query = @"select c.Code '0', c.Description as '1', s.Code '2', S.Description as '3', ss.Code '4', sS.Description as '5', 
                                            '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(50),i.MappingID)+''') />' as '6', 
                                            '<img src=../images/Delete.jpeg alt=Delete onclick=DeleteDetails(this,'''+CONVERT(nvarchar(50),i.MappingID)+''') />' as '7' 
                                            from ItemsMapping i  join Category c on c.Id = i.CategoryID 
                                            join Sub_Category s on s.Id = i.SubCategoryID left join SubSubCategory ss on ss.Id = i.SubSubCategoryID
                                               {0} order by s.Code";


                //                string aa = @"select i.MappingID, i.ItemCode, 
                //                                              c.Code CatCode, c.Description as CatDesc, 
                //                                              s.Code SubCatCode, S.Description as SubCatDesc, 
                //                                              ss.Code SubSubCatCode, sS.Description as SubSubCatDesc  
                //                                              from ItemsMapping i  join Category c on c.Id = i.CategoryID 
                //                                              join Sub_Category s on s.Id = i.SubCategoryID  join SubSubCategory ss on ss.Id = i.SubSubCategoryID";

                query = String.Format(query, filteredWhere);

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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetSpareParts()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string query = "";
                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE s.Code LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR s.Description LIKE ");
                    sb.Append(wrappedSearch);
                    //sb.Append(" OR c.Code LIKE ");
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR c.Description LIKE ");
                    //sb.Append(wrappedSearch);
                    filteredWhere = sb.ToString();
                }

                sb.Clear();
                string orderByClause = string.Empty;
                //sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));
                //sb.Append(" ");
                //sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);
                //orderByClause = "0 DESC";
                //if (!String.IsNullOrEmpty(orderByClause))
                //{
                //    orderByClause = orderByClause.Replace("0", ", CreatedDate ");
                //    orderByClause = orderByClause.Remove(0, 1);
                //}
                //else
                //    orderByClause = "CreatedDate DESC";
                //orderByClause = "ORDER BY " + orderByClause;
                sb.Clear();
                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                query = @"select s.Code '0', s.Description '1', 
                            '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,'''+CONVERT(nvarchar(50),s.Id)+''') />' as '2', 
                            '<img src=../images/Delete.jpeg alt=Delete onclick=DeleteDetails('''+CONVERT(nvarchar(50),s.Id)+''','''+CONVERT(nvarchar(50),s.CreatedBy)+''') />' as '3' 
                             from SpareParts s                              
                             {0} order by s.CreatedDate desc";

                query = String.Format(query, filteredWhere);

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




        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }
    }
}
