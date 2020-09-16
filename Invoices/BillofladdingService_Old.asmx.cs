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
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for BillofladdingService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BillofladdingService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }


        /// <summary>
        /// FOR Bill Of Ladding Status  
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetBOLStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string BOLNum = HttpContext.Current.Request.Params["sSearch_0"];
                string Cust = HttpContext.Current.Request.Params["sSearch_1"];
                string PInvNo = HttpContext.Current.Request.Params["sSearch_2"];
                string Freig = HttpContext.Current.Request.Params["sSearch_3"];
                string PlceofDel = HttpContext.Current.Request.Params["sSearch_4"];

                StringBuilder s = new StringBuilder();

                if (BOLNum != "")
                    s.Append(" and b.BillofLadingNo LIKE '%" + BOLNum.Replace("'", "''") + "%'");
                if (Cust != "")
                    s.Append(" and dbo.FN_MergeTableColumn_Cust(b.CustomerIDs) LIKE '%" + Cust.Replace("'", "''") + "%'");
                if (PInvNo != "")
                    s.Append(" and dbo.FN_MergeTableColumn_ShpmntPfrmaInv(b.ProformaINVIDs) LIKE '%" + PInvNo.Replace("'", "''") + "%'");
                if (Freig != "")
                    s.Append(" and  b.Frieght LIKE '%" + Freig.Replace("'", "''") + "%'");
                if (PlceofDel != "")
                    s.Append(" and  dbo.FN_GetDescription(b.PlaceOfDelivery) LIKE '%" + PlceofDel.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                //if (rawSearch.Length > 0)
                //{
                //    sb.Append(" WHERE LPODate LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR LocalPurchaseOrderNo LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR FPOrderNmbr LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR Subject LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR Status LIKE ");
                //    sb.Append(wrappedSearch);
                //    sb.Append(" OR SuplrNm LIKE ");
                //    sb.Append(wrappedSearch);
                //    //sb.Append(" OR CusmorId LIKE ");
                //    //sb.Append(wrappedSearch);


                //    filteredWhere = sb.ToString();
                //}

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", BLID ");
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
                    orderByClause = "BLID Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
                            declare @MAA TABLE(BLID uniqueidentifier, BillofLadingNo nvarchar(MAX), Customers nvarchar(MAX), ShpngPrfmaInvcNmbr nvarchar(MAX), 
                                            Frieght nvarchar(MAX),PlaceOfDelivery nvarchar(MAX),EDIT nvarchar(MAX), Delt nvarchar(MAX), CompanyId uniqueidentifier, CreatedDate datetime)
                            INSERT
                            INTO
	                            @MAA (BLID,BillofLadingNo,Customers,ShpngPrfmaInvcNmbr,Frieght, PlaceOfDelivery, Edit, Delt, CompanyId, CreatedDate)
	                             select b.BLID, b.BillofLadingNo,dbo.FN_MergeTableColumn_Cust(b.CustomerIDs) as Customers, 
			                        dbo.FN_MergeTableColumn_ShpmntPfrmaInv(b.ProformaINVIDs) as ShpngPrfmaInvcNmbr,b.Frieght,
			                        dbo.FN_GetDescription(b.PlaceOfDelivery) as PlaceOfDelivery,
                                    '<img src=../images/Edit.jpeg alt=Edit onclick=EditDetails(this,''' + CONVERT(varchar(40), b.CreatedBy) 
                                     +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +') />' EDIT, 
                                     '<img src=../images/delete.png alt=Delete onclick=Delet(this,'''
                                     + CONVERT(varchar(40), b.CreatedBy) +''','+(case when u.ContactType = 'Trafficker' then '1' else '0' end) +','''+ Convert(nvarchar(max),b.CompanyId)+''') />' Delt,b.CompanyId , b.CreatedDate 
			                        from BillOfLading b 		
                                    inner join Users u on u.ID = b.CreatedBy
                                       
	                                 {4}                   
                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].BLID)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].BLID) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].BLID
			                               ,[@MAA].BillofLadingNo      
                                           ,[@MAA].Customers
                                           ,[@MAA].ShpngPrfmaInvcNmbr
                                           ,[@MAA].Frieght
                                           ,[@MAA].PlaceOfDelivery
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt
                                           ,[@MAA].CompanyId
                                           ,[@MAA].CreatedDate      
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                string where = " b.IsActive <> 0 and b.CompanyId = " + "'" + CompanyID + "'" + " order by b.CreatedDate desc ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    s.ToString() == "" ? " where b.IsActive <> 0 and " + where : " where b.IsActive <> 0 " + s.ToString() + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["BLID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string BOLNo = data["BillofLadingNo"].ToString().Replace("\"", "\\\"");
                    string BId = data["BLID"].ToString().Replace("\"", "\\\"");
                    BOLNo = BOLNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href= BillOfLadingDetails.Aspx?ID={0}>{1}</a>""", BId, BOLNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Pinv = data["Customers"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", Pinv.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CNm = data["ShpngPrfmaInvcNmbr"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", CNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPo = data["Frieght"].ToString().Replace("\"", "\\\"");
                    FPo = FPo.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ChkLst = data["PlaceOfDelivery"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", ChkLst.Replace(Environment.NewLine, "\\n"));
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

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }
    }
}
