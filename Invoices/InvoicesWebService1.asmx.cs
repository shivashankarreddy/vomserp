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

namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for InvoicesWebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class InvoicesWebService1 : System.Web.Services.WebService
    {

        /// <summary>
        /// FOR Shipment Invoice Status  
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetPInvStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string SInvNo = HttpContext.Current.Request.Params["sSearch_0"];
                string PInvNo = HttpContext.Current.Request.Params["sSearch_1"];
                string date = HttpContext.Current.Request.Params["sSearch_2"];
                string Trms = HttpContext.Current.Request.Params["sSearch_3"];
                string CustNm = HttpContext.Current.Request.Params["sSearch_4"];
                string RefFPO = HttpContext.Current.Request.Params["sSearch_5"];
                string ShpPlngNo = HttpContext.Current.Request.Params["sSearch_6"];
                string Stat = HttpContext.Current.Request.Params["sSearch_7"];

                StringBuilder s = new StringBuilder();
                if (date != "")
                {
                    date = date.Replace("'", "''");
                    DateTime FrmDt = date.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(date.Split('~')[0].ToString());
                    DateTime EndDat = date.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(date.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and PrfmaInvcDt between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (SInvNo != "")
                    s.Append(" and InvoiceNo LIKE '%" + SInvNo.Replace("'", "''") + "%'");
                if (PInvNo != "")
                    s.Append(" and p.PrfmInvcNo LIKE '%" + PInvNo.Replace("'", "''") + "%'");
                if (Trms != "")
                    s.Append(" and TrmsDlvryPmnt LIKE '%" + Trms.Replace("'", "''") + "%'");
                if (CustNm != "")
                    s.Append(" and  CustomerName LIKE '%" + CustNm.Replace("'", "''") + "%'");
                if (RefFPO != "")
                    s.Append(" and FPONmbrs LIKE '%" + RefFPO.Replace("'", "''") + "%'");
                if (ShpPlngNo != "")
                    s.Append(" and CheckListNo LIKE '%" + ShpPlngNo.Replace("'", "''") + "%'");
                if (Stat != "")
                    s.Append(" and Status LIKE '%" + Stat.Replace("'", "''") + "%'");

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
                    //sb.Append(" OR CusmorId LIKE ");
                    //sb.Append(wrappedSearch);


                    filteredWhere = sb.ToString();
                }

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", ID ");
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
                            declare @MAA TABLE(ID uniqueidentifier, PrfmInvcNo nvarchar(MAX), PrfmaInvcDt date, TrmsDlvryPmnt nvarchar(MAX), CustomerName nvarchar(MAX),FPONmbrs nvarchar(MAX), InvoiceNo nvarchar(MAX), Status nvarchar(MAX), CheckListNo nvarchar(MAX), 
                                               EDIT nvarchar(MAX), Delt nvarchar(MAX),CompanyId uniqueidentifier, CreatedDate datetime)
                            INSERT
                            INTO
	                            @MAA (ID,PrfmInvcNo,PrfmaInvcDt,TrmsDlvryPmnt,CustomerName,
                                      FPONmbrs,InvoiceNo, Status, CheckListNo, Edit, Delt,CompanyId,CreatedDate)
	                              Select ID,PrfmInvcNo,PrfmaInvcDt,TrmsDlvryPmnt,CustomerName,
                                      FPONmbrs,InvoiceNo, Status, CheckListNo, Edit, Delt,CompanyId,CreatedDate from View_ShipmentInvoice p
                                    
	                                 {4}                   
                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].ID)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].ID) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].ID
			                               ,[@MAA].PrfmInvcNo      
                                           ,[@MAA].PrfmaInvcDt
                                           ,[@MAA].TrmsDlvryPmnt
                                           ,[@MAA].CustomerName
                                           ,[@MAA].FPONmbrs
                                           ,[@MAA].InvoiceNo
                                           ,[@MAA].Status 
                                           ,[@MAA].CheckListNo
                                           ,[@MAA].EDIT
                                           ,[@MAA].Delt
                                           ,[@MAA].CompanyId    
                                           ,[@MAA].CreatedDate  
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                
                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                string where = " p.IsActive <> 0 and p.CompanyId =" + "'" + CompanyID + "'" + " order by CreatedDate desc ";
                //query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn , s.ToString()+ "and lp.IsActive<> 0 order by LocalPurchaseOrderId DESC ");
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    s.ToString() == "" ? " where p.IsActive <> 0 and " + where : " where p.IsActive <> 0 " + s.ToString() + " and " + where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string SInv = data["InvoiceNo"].ToString().Replace("\"", "\\\"");
                    string PId = data["ID"].ToString().Replace("\"", "\\\"");
                    string Pinv = data["PrfmInvcNo"].ToString().Replace("\"", "\\\"");
                    SInv = SInv.Replace("\t", "-");
                    if (SInv != "")
                    {
                        sb.AppendFormat(@"""0"": ""<a href= PrfmaInvoiceDetails.aspx?ID={0}>{1}</a>""", PId, SInv.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""0"": ""{1}""", PId, SInv.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    
                    //Pinv = Pinv.Replace("\t","-");
                    if (SInv == "")
                    {
                        sb.AppendFormat(@"""1"": ""<a href= PrfmaInvoiceDetails.aspx?ID={0}>{1}</a>""", PId, Pinv.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.AppendFormat(@"""1"": ""{0}""", Pinv.Replace(Environment.NewLine, "\\n"));
                        sb.Append(",");
                    }

                    sb.AppendFormat(@"""2"": ""{0:dd-MM-yyyy}""", data["PrfmaInvcDt"]);
                    sb.Append(",");

                    string TDR = data["TrmsDlvryPmnt"].ToString();//.Replace("\"", "\\\"");
                    TDR = TDR.Replace("\n", "");
                    TDR = TDR.Replace("\r", "");
                    sb.AppendFormat(@"""3"": ""{0}""", TDR);//TDR.Replace(Environment.NewLine, "\\n")
                    sb.Append(",");

                    string CNm = data["CustomerName"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", CNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPo = data["FPONmbrs"].ToString().Replace("\"", "\\\"");
                    FPo = FPo.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ChkLst = data["CheckListNo"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""6"": ""{0}""", ChkLst.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Statu = data["Status"].ToString().Replace("\"", "\\\"");
                    Stat = Statu.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Statu.Replace(Environment.NewLine, "\\n"));
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

