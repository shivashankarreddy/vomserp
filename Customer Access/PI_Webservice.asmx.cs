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
using System.Web.Script.Serialization;

namespace VOMS_ERP.Customer_Access
{
    /// <summary>
    /// Summary description for PI_Webservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class PI_Webservice : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
        public string GetFItems()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FeNo = HttpContext.Current.Request.Params["sSearch_0"] == null ? "" : HttpContext.Current.Request.Params["sSearch_0"];
                string date = HttpContext.Current.Request.Params["sSearch_1"] == null ? "" : HttpContext.Current.Request.Params["sSearch_1"];
                //string RDate = HttpContext.Current.Request.Params["sSearch_2"] == null ? "" : HttpContext.Current.Request.Params["sSearch_2"];
                string Subject = HttpContext.Current.Request.Params["sSearch_2"] == null ? "" : HttpContext.Current.Request.Params["sSearch_2"];
                string Dept = HttpContext.Current.Request.Params["sSearch_3"] == null ? "" : HttpContext.Current.Request.Params["sSearch_3"];
                string Cust = HttpContext.Current.Request.Params["sSearch_4"] == null ? "" : HttpContext.Current.Request.Params["sSearch_4"];
                string CnctPrsn = HttpContext.Current.Request.Params["sSearch_5"] == null ? "" : HttpContext.Current.Request.Params["sSearch_5"];
                string Status = HttpContext.Current.Request.Params["sSearch_6"] == null ? "" : HttpContext.Current.Request.Params["sSearch_6"];
                string Filename = HttpContext.Current.Request.Form["HFFilename"] == null ? "" : HttpContext.Current.Request.Params["HFFilename"];

                //string mode, option, user, searchKey, orderByColumn, orderByDir, estMstSrno, pnlsrno, jsonString;
                //int limit, start, draw;

                //mode = HttpContext.Current.Request.Params["mode"] == null ? "" : HttpContext.Current.Request.Params["mode"].ToString();
                //option = HttpContext.Current.Request.Params["option"] == null ? "" : HttpContext.Current.Request.Params["option"].ToString();
                //user = HttpContext.Current.Request.Params["user"] == null ? "" : HttpContext.Current.Request.Params["user"].ToString();
                //searchKey = HttpContext.Current.Request.Params["search[value]"] == null ? "" : HttpContext.Current.Request.Params["search[value]"].ToString();
                //orderByColumn = HttpContext.Current.Request.Params["order[0][column]"] == null ? "" : HttpContext.Current.Request.Params["order[0][column]"].ToString();
                //orderByDir = HttpContext.Current.Request.Params["order[0][dir]"] == null ? "" : HttpContext.Current.Request.Params["order[0][dir]"].ToString();
                //estMstSrno = HttpContext.Current.Request.Params["estMstSrno"] == null ? "" : HttpContext.Current.Request.Params["estMstSrno"].ToString();
                //pnlsrno = HttpContext.Current.Request.Params["pnlsrno"] == null ? "" : HttpContext.Current.Request.Params["pnlsrno"].ToString();

                //draw = HttpContext.Current.Request.Params["draw"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Request.Params["draw"].ToString());
                //limit = HttpContext.Current.Request.Params["length"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Request.Params["length"].ToString());
                //start = HttpContext.Current.Request.Params["start"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Request.Params["start"].ToString());

                string query = "";

                // HttpContext.Current.Request.Params["sPaginationType"];//HttpContext.Current.Request.Url.AbsoluteUri;
                StringBuilder s = new StringBuilder();
                if (date != null && date != "" && date.Length > 1)
                {
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and f.EnquiryDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (FeNo != "")
                    s.Append(" and f.EnquireNumber LIKE '%" + FeNo + "%'");
                //if (RDate != null && RDate != "")
                //{
                //    DateTime FrmDt = RDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RDate.Split('~')[0].ToString());
                //    DateTime EndDat = RDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RDate.Split('~')[1].ToString());
                //    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                //        s.Append(" and f.ReceivedDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                //}
                if (Subject != "")
                    s.Append(" and f.Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusID) LIKE '%" + Status.Replace("'", "''") + "%'");
                if (Dept != "")
                    s.Append(" and dbo.FN_GetDescription(f.DepID) LIKE '%" + Dept + "%'");
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
                    //sb.Append(wrappedSearch);
                    //sb.Append(" OR ReceivedDate LIKE ");
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

                //                string query = @"declare @MAA TABLE(EnquiryDate date, EnquireNumber nvarchar(400), ReceivedDate date, Subject nvarchar(MAX),
                //							ForeignEnquireId uniqueidentifier, StatusTypeId nvarchar(MAX), DepartmentId nvarchar(MAX), CusmorId nvarchar(MAX),  
                //							ContactPerson nvarchar(MAX), MAIL nvarchar(MAX), AMENDEDIT nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate DateTime,Export nvarchar(max), VendorIds nvarchar(max),
                //                            History nvarchar(MAX))				
                //
                //							INSERT INTO @MAA (EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
                //								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History) 
                //							 Select EnquiryDate, EnquireNumber, ReceivedDate, Subject, ForeignEnquireId , StatusTypeId, DepartmentId, CusmorId, 
                //								ContactPerson, MAIL, AMENDEDIT,EDIT, Delt, CompanyId, CreatedDate,Export,VendorIds,History  from View_ForeignEnquiry  f
                //                               {4}
                //							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].EnquireNumber) FROM @MAA) AS TotalRows, 
                //								 (SELECT  count( [@MAA].EnquireNumber) FROM @MAA {1}) AS TotalDisplayRows, [@MAA].EnquiryDate, [@MAA].EnquireNumber,  
                //								 [@MAA].ReceivedDate, [@MAA].Subject, [@MAA].ForeignEnquireId, [@MAA].StatusTypeId, [@MAA].DepartmentId, [@MAA].CusmorId, 
                //								 [@MAA].ContactPerson, [@MAA].CreatedDate, [@MAA].MAIL, [@MAA].AMENDEDIT,[@MAA].EDIT, [@MAA].Delt, [@MAA].Export, [@MAA].VendorIds, [@MAA].History FROM @MAA {1}) RawResults) Results WHERE 
                //								 RowNumber BETWEEN {2} AND {3} order by CreatedDate desc";

                //                query = @"Select EnquireNumberS , EnquiryDateS , ReceivedDateS , Subject , DepartmentId ,CusmorId ,ContactPerson ,StatusTypeId ,
                //                                            AMENDEDIT ,MAIL ,AMENDEDIT ,EDIT , Delt ,Export  from View_ForeignEnquiry_Test  f
                //                                               {4} order by CreatedDate desc";
                //                //, ForeignEnquireId DT_RowId, '' DT_RowClass

                query = @"Select EnquireNumberS as '0', EnquiryDateS '1', Subject '2', DepartmentId '3',CusmorId '4',ContactPerson '5',StatusTypeId '6',
                                            History '7',MAIL '8',AMENDEDIT '9',EDIT '10', Delt '11',Export '12', ForeignEnquireId DT_RowId, '' DT_RowClass from View_ForeignEnquiry_Test  f
                                               {4} order by CreatedDate desc";

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
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.IsActive <> 0 " +
                             "AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')" +
                             "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                               "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " +
                               " and f.ParentFEnqID = f.ForeignEnquireId and " + Where :
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103)" + s.ToString()
                                + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or "
                            + "isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                            "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                            "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " +
                            "and f.ParentFEnqID = f.ForeignEnquireId  and " + Where);
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
                if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        " WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or" +
                        " isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or "
                        + "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                        "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and f.ParentFEnqID = f.ForeignEnquireId  and " : " f.IsActive <> 0 and (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                        "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or "
                        + "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and f.ParentFEnqID = f.ForeignEnquireId " + s.ToString() + "AND")
                        + "  f.CompanyId = '" + Session["CompanyID"] + " '"
                        : (s.ToString() == "" ? " AND f.IsActive <> 0 " : " AND f.IsActive <> 0  " + s.ToString() + "AND") +
                        "  f.CompanyId = '" + Session["CompanyID"] + "'");
                    //f.custId in (Select Data from dbo.SplitString('"
                    //+ ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',')) 
                    /* f.custId in (Select Data from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' ))*/
                }
                //else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
                //{
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                //   ("Where (f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "'")//Where(isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and 
                //   : (" where f.IsActive <> 0  " //AND (isnull(convert(nvarchar(40),b.VendorId),'') = '' or isnull(convert(nvarchar(40),b.VendorId),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                //   + s.ToString() + " and  (f.CompanyId = " + "'" + CompanyID + "'" + " and f.VendorIds = '00000000-0000-0000-0000-000000000000' and f.CompanyId = f.VendorIds " + ")  or f.VendorIds = " + "'" + CompanyID + "'")); //+ Where
                //}
                //else
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                //        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
                //   (" Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + Where)//"Where (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') or "
                //   :
                //        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
                //   (" where f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " + s.ToString() + " and " + Where));
                ////AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')
                s.Clear();

                #region NotInUse

                //var connectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
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
                //var count = 0;

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
                //    sb.AppendFormat(@"""0"": ""<a href=PIdetails.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
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
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    limit = limit == -1 ? dt.Rows.Count : limit;
                //    dtt = dtt.AsEnumerable().Skip(start).Take(limit).CopyToDataTable();
                //}
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
                        draw = Convert.ToInt32(sEcho == 0 ? 0 : sEcho),
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

                return outputJson
                    .Replace(Environment.NewLine, "\\n")
                    .Replace("\t", "-").Replace("../Enquiries/FeAmendments.aspx", "../Customer%20Access/PE_Amendment.aspx");

                //return outputJson;
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
        public string GetPENoItems()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string FeNo = HttpContext.Current.Request.Params["sSearch_0"] == null ? "" : HttpContext.Current.Request.Params["sSearch_0"];
                string Status = HttpContext.Current.Request.Params["sSearch_1"] == null ? "" : HttpContext.Current.Request.Params["sSearch_1"];
                string Filename = HttpContext.Current.Request.Form["HFFilename"] == null ? "" : HttpContext.Current.Request.Params["HFFilename"];

                string query = "";
                StringBuilder s = new StringBuilder();
                if (FeNo != "")
                    s.Append(" and f.EnquireNumber LIKE '%" + FeNo + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusID) LIKE '%" + Status.Replace("'", "''") + "%'");
                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" Where EnquireNumber LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR StatusTypeId LIKE ");
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

                query = @"  
							declare @MAA TABLE(EnquireNumber nvarchar(400), ForeignEnquireId uniqueidentifier,PEStatus nvarchar(50), CreatedDate datetime)
							INSERT INTO @MAA (EnquireNumber, ForeignEnquireId, PEStatus, CreatedDate )
								Select EnquireNumber,ForeignEnquireId,PEStatus, CreatedDate  from View_GetStatusPERec f
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
										   ,[@MAA].EnquireNumber
										   ,[@MAA].ForeignEnquireId     
										   ,[@MAA].PEStatus
                                           ,[@MAA].CreatedDate 
									  FROM
										  @MAA {1}) RawResults) Results WHERE
										 RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

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
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103) and f.IsActive <> 0 " +
                             "AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')" +
                             "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                               "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " +
                               " and f.ParentFEnqID = f.ForeignEnquireId and " + Where :
                                " where CONVERT(nvarchar(12), f.CreatedDate,103)= CONVERT(nvarchar(50),GETDATE(),103)" + s.ToString()
                                + " AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or "
                            + "isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                            "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                            "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') " +
                            "and f.ParentFEnqID = f.ForeignEnquireId  and " + Where);
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
                if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, filteredWhere == "" ?
                        " WHERE " + (s.ToString() == "" ? " f.IsActive <> 0 AND (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or" +
                        " isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or "
                        + "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                        "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and f.ParentFEnqID = f.ForeignEnquireId  and " : " f.IsActive <> 0 and (isnull(convert(nvarchar(40),f.VendorIds),'') = '' or isnull(convert(nvarchar(40),f.VendorIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or " +
                        "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and (isnull(convert(nvarchar(40),f.F_SupplierIds),'') = '' or "
                        + "isnull(convert(nvarchar(40),f.F_SupplierIds),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000')  " +
                        "and f.ParentFEnqID = f.ForeignEnquireId " + s.ToString() + "AND")
                        + "  f.CompanyId = '" + Session["CompanyID"] + " '"
                        : (s.ToString() == "" ? " AND f.IsActive <> 0 " : " AND f.IsActive <> 0  " + s.ToString() + "AND") +
                        "  f.CompanyId = '" + Session["CompanyID"] + "'");
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


                    string EnqNo = data["EnquireNumber"].ToString().Replace("\"", "\\\"");
                    string EnqId = data["ForeignEnquireId"].ToString().Replace("\"", "\\\"");

                    EnqNo = EnqNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", EnqNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Remarkss = data["PEStatus"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", Remarkss.Replace(Environment.NewLine, "\\n"));
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
            catch (SqlException ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "PE Number Status", ex.Message.ToString());
                return "";
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "PE Number Status", ex.Message.ToString());
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
                        + "  f.CompanyId = '" + Session["CompanyID"] + " ' and (VendorId <> '00000000-0000-0000-0000-000000000000' or F_SupplierIds <> '00000000-0000-0000-0000-000000000000'  or L_SupplierIds <> '00000000-0000-0000-0000-000000000000' ) "
                        : (s.ToString() == "" ? " AND b.IsActive <> 0  and "
                        : " AND f.IsActive <> 0  and " + s.ToString() + "AND ") +
                        "  f.CompanyId = '" + Session["CompanyID"] + "' and (VendorId <> '00000000-0000-0000-0000-000000000000' or F_SupplierIds <> '00000000-0000-0000-0000-000000000000' or L_SupplierIds <> '00000000-0000-0000-0000-000000000000' ) ");
                }

                /* f.custId in (Select * from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',')) and
                 */

                /*f.custId in (Select * from dbo.SplitString('"
                        + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' )) and
                 */
                //else if (CommonBLL.AmdinContactTypeText == Session["AccessRole"].ToString())
                //{
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                //   (" " + Where) :
                //   (" " + Where + " and " + s.ToString()));
                //}
                //else
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                //        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and " + Where :
                //   ("" + Where) :
                //        //Role != CommonBLL.SuperAdminRole ? " Where b.CreatedBy =" + "'" + (Session["UserID"]).ToString() + "'" + " and b.IsActive <> 0 " + s.ToString() + "and " + Where :
                //   (" "
                //   + s.ToString() + " and " + Where));
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
                    sb.AppendFormat(@"""0"": ""<a href=Floated_pidetails.aspx?FFEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
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
                    s.Append(" and dbo.FN_MergeTableColumn_FE(FEID) LIKE '%" + REfFENO + "%'");
                if (Subject != "")
                    s.Append(" and f.Subject LIKE '%" + Subject + "%'");
                if (Status != "")
                    s.Append(" and dbo.FN_GetStatus(f.StatusTypeId) LIKE '%" + Status.Replace("'", "''") + "%'");
                if (Cust != "")
                    s.Append(" and  dbo.GetActiveCustomerName(CustID) LIKE '%" + Cust + "%'");

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
	                                Select CreatedDate,QuotationDate,ForeignQuotationId,Quotationnumber,FrnEnqNmbr,Subject,Status,VendorIds,StatusTypeId,Mail, Edit, Delt from View_GetFQItems  f WITH(NOLOCK)
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

                string where = "f.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "' or f.CustID = '" + new Guid(Session["CompanyID"].ToString()) + "' ";

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
                //else if (CommonBLL.CustmrContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()))
                //{
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                //        " WHERE  f.CustID in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "') and f.IsActive <> 0 "
                //        : "WHERE" + (s.ToString() == "" ? " AND f.IsActive <> 0 AND " : " f.IsActive <> 0 " + s.ToString() + "AND") + " f.CustID in ('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "')");
                //}
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
                DB.CommandTimeout = 0;
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
                    sb.AppendFormat(@"""0"": ""<a href=FQDetails_Customer.aspx?ID={0}>{1}</a>""", FQId, FQNo.Replace(Environment.NewLine, "\\n"));
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

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" FPODate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
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

                string query = @"  
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
                //(isnull(convert(nvarchar(40),Vendor),'') != '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') != '00000000-0000-0000-0000-000000000000') "
                //,'''+ REPLACE(REPLACE(dbo.FN_GetStatus(StatusTypeId),' ',''),'''','') +'''
                string where = "(b.CompanyId = '" + new Guid(Session["CompanyID"].ToString()) + "'" + " or b.CustId = '" + new Guid(Session["CompanyID"].ToString()) + "')";
                if (HttpContext.Current.Request.UrlReferrer.Query != "" && HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        if (Session["IsUser"] == null && Session["IsUser"].ToString() == "0")
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
                    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, s.ToString() == "" ?
                        " WHERE (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and" + where
                        : "WHERE " + s.ToString()
                        + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                }
                //else b.CustId in (Select Data from dbo.SplitString('" + ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[11].ToString() + "', ',' ))and
                //{
                //    query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                //        s.ToString() == "" ? " where IsActive <> 0 and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and "
                //        + where : " where IsActive <> 0 " + s.ToString()
                //        + " and (isnull(convert(nvarchar(40),Vendor),'') = '' or isnull(convert(nvarchar(40),Vendor),'00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000') and " + where);
                //}
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
                    sb.AppendFormat(@"""0"": ""<a href=FPODetails_Customer.aspx?ID={0}>{1}</a>""", FPid, Fpno.Replace(Environment.NewLine, "\\n"));
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

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }

        /// <summary>
        /// FOR FOREIGN ENQUIRY AMENDMENT
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetPEAmdItems()
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
                    sb.AppendFormat(@"""0"": ""<a href=PE_Details_Amendment.aspx?FEnqID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
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

    }
}
