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

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ShpngBillStatWebService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetShippingBillStat()
        {
            try
            {
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];

                string ShpngBilNmbr = HttpContext.Current.Request.Params["sSearch_0"];
                string ShpngBilDate = HttpContext.Current.Request.Params["sSearch_1"];
                string PrfmaInvoiceNmbr = HttpContext.Current.Request.Params["sSearch_2"];
                string PrfmaInvoiceDate = HttpContext.Current.Request.Params["sSearch_3"];
                string PrtLoading = HttpContext.Current.Request.Params["sSearch_4"];
                string PrtDischarge = HttpContext.Current.Request.Params["sSearch_5"];
                string CntryDestination = HttpContext.Current.Request.Params["sSearch_6"];
                string CntryOrigine = HttpContext.Current.Request.Params["sSearch_7"];

                StringBuilder s = new StringBuilder();
                if (ShpngBilDate != null && ShpngBilDate != "")
                {
                    DateTime FrmDt = ShpngBilDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ShpngBilDate.Split('~')[0].ToString());
                    DateTime EndDat = ShpngBilDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ShpngBilDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ShpngBilDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (PrfmaInvoiceDate != null && PrfmaInvoiceDate != "")
                {
                    DateTime FrmDt = PrfmaInvoiceDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[0].ToString());
                    DateTime EndDat = PrfmaInvoiceDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PrfmaInvoiceDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and PrfmaInvoiceDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (ShpngBilNmbr != "")
                    s.Append(" and ShpngBilNmbr LIKE '%" + ShpngBilNmbr + "%'");
                if (PrfmaInvoiceNmbr != "")
                    s.Append(" and PrfmaInvoiceNmbr LIKE '%" + PrfmaInvoiceNmbr + "%'");
                if (PrtLoading != "")
                    s.Append(" and PrtLoadingDes LIKE '%" + PrtLoading + "%'");
                if (PrtDischarge != "")
                    s.Append(" and PrtDischargeDes LIKE '%" + PrtDischarge + "%'");
                if (CntryDestination != "")
                    s.Append(" and CntryDestinationDes LIKE '%" + CntryDestination + "%'");
                if (CntryOrigine != "")
                    s.Append(" and CntryOrigineDes LIKE '%" + CntryOrigine + "%'");

                var sb = new StringBuilder();
                var filteredWhere = string.Empty;
                var wrappedSearch = "'%" + rawSearch + "%'";
                if (rawSearch.Length > 0)
                {
                    sb.Append(" WHERE ShpngBilNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR ShpngBilDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PrfmaInvoiceNmbr LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PrfmaInvoiceDate LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PrtLoadingDes LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR PrtDischargeDes LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CntryDestinationDes LIKE ");
                    sb.Append(wrappedSearch);
                    sb.Append(" OR CntryOrigineDes LIKE ");
                    sb.Append(wrappedSearch);

                    filteredWhere = sb.ToString();
                }


                ////ORDERING

                sb.Clear();

                string orderByClause = string.Empty;
                sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

                sb.Append(" ");

                sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

                orderByClause = "0 DESC"; // = sb.ToString();

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

                //Mail nvarchar(MAX), Edit nvarchar(MAX), Delt nvarchar(MAX)
                //, Mail, Edit, Delt

                string query = @"  
                            declare @MAA TABLE
                                            (
	                                            ID uniqueidentifier,
	                                            FS_AdrsDtlsID uniqueidentifier,
	                                            DBK_DtlsID uniqueidentifier,
	                                            INVC_DtlsID uniqueidentifier,
	                                            DEPB_DtlsID uniqueidentifier,
	                                            DA_DtlsID uniqueidentifier,
	                                            RefInvcID uniqueidentifier,
	                                            ShpngBilNmbr nvarchar(500),
	                                            ShpngBilDate datetime,
	                                            PrfmaInvoiceNmbr nvarchar(500),
	                                            PrfmaInvoiceDate datetime,
	                                            PrtLoadingDes nvarchar(100),
	                                            PrtDischargeDes nvarchar(100),
	                                            CntryDestinationDes nvarchar(100),
	                                            CntryOrigineDes nvarchar(100),
	                                            CreatedBy uniqueidentifier, 
                                                EDIT nvarchar(MAX), 
                                                Delt nvarchar(MAX),
                                                CreatedDate datetime
                                            )
                            INSERT
                            INTO
	                            @MAA ( ID ,FS_AdrsDtlsID ,DBK_DtlsID ,INVC_DtlsID ,DEPB_DtlsID ,DA_DtlsID ,RefInvcID ,
                                        ShpngBilNmbr ,ShpngBilDate ,PrfmaInvoiceNmbr ,PrfmaInvoiceDate ,PrtLoadingDes ,
	                                    PrtDischargeDes ,CntryDestinationDes ,CntryOrigineDes ,CreatedBy, EDIT, Delt, CreatedDate )
	                                Select ID ,FS_AdrsDtlsID ,DBK_DtlsID ,INVC_DtlsID ,DEPB_DtlsID ,DA_DtlsID ,RefInvcID ,
                                        ShpngBilNmbr ,ShpngBilDate ,PrfmaInvoiceNmbr ,PrfmaInvoiceDate ,PrtLoadingDes ,
	                                    PrtDischargeDes ,CntryDestinationDes ,CntryOrigineDes ,CreatedBy, EDIT, Delt, CreatedDate from View_ShippingBillStatus  SBD where SBD.IsActive<>0
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
                                        ,[@MAA].FS_AdrsDtlsID 
                                        ,[@MAA].DBK_DtlsID 
                                        ,[@MAA].INVC_DtlsID 
                                        ,[@MAA].DEPB_DtlsID 
                                        ,[@MAA].DA_DtlsID 
                                        ,[@MAA].RefInvcID 
                                        ,[@MAA].ShpngBilNmbr
                                        ,[@MAA].ShpngBilDate
                                        ,[@MAA].PrfmaInvoiceNmbr
                                        ,[@MAA].PrfmaInvoiceDate     
                                        ,[@MAA].PrtLoadingDes
                                        ,[@MAA].PrtDischargeDes
                                        ,[@MAA].CntryDestinationDes
                                        ,[@MAA].CntryOrigineDes
                                        ,[@MAA].CreatedBy
                                        ,[@MAA].EDIT
                                        ,[@MAA].Delt
                                        ,[@MAA].CreatedDate
                                        FROM
                                        @MAA {1}) RawResults) Results WHERE
                	                     RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";
                string Where = "";

                Guid CompanyID = new Guid(Session["CompanyID"].ToString());
                Where = " SBD.CompanyId = " + "'" + CompanyID + "'";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                         filteredWhere == "" ? " " + Where + s.ToString() == "" ? " SBD.IsActive <> 0 " + Where : s.ToString() + " and  SBD.IsActive <> 0 and " + Where
                         : s.ToString() == "" ? " and SBD.IsActive <> 0 and " + Where : s.ToString() + " and  SBD.IsActive <> 0 and " + Where);
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ID"].ToString()); // count++);
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");

                    string EnqNo = data["ShpngBilNmbr"].ToString().Replace("\"", "\\\"");
                    string EnqId = data["ID"].ToString();
                    EnqNo = EnqNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href=ShpngBilDtlsReport.aspx?ID={0}>{1}</a>""", EnqId, EnqNo.Replace(Environment.NewLine, "\\n")); // New Line
                    sb.Append(",");

                    sb.AppendFormat(@"""1"": ""{0:dd/MM/yyyy}""", data["ShpngBilDate"]);
                    sb.Append(",");

                    string PRFINVNO = data["PrfmaInvoiceNmbr"].ToString().Replace("\"", "\\\"");
                    PRFINVNO = PRFINVNO.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", PRFINVNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""3"": ""{0:dd/MM/yyyy}""", data["PrfmaInvoiceDate"]);
                    sb.Append(",");

                    string PrtLoadingDes = data["PrtLoadingDes"].ToString().Replace("\"", "\\\"");
                    PrtLoadingDes = PrtLoadingDes.Replace("\t", "-");
                    sb.AppendFormat(@"""4"": ""{0}""", PrtLoadingDes.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string PrtDischargeDes = data["PrtDischargeDes"].ToString().Replace("\"", "\\\"");
                    PrtDischargeDes = PrtDischargeDes.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", PrtDischargeDes.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CntryDestinationDes = data["CntryDestinationDes"].ToString().Replace("\"", "\\\"");
                    CntryDestinationDes = CntryDestinationDes.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", CntryDestinationDes.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CntryOrigineDes = data["CntryOrigineDes"].ToString().Replace("\"", "\\\"");
                    CntryOrigineDes = CntryOrigineDes.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", CntryOrigineDes.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");

                    //Int64 ID = Convert.ToInt64(((Label)gvrow.FindControl("lblRefShpngBil")).Text);
                    //Int64 FS_AdrsDtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblFSADtls")).Text);
                    //Int64 DBK_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDBKDtls")).Text);
                    //Int64 DEPB_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDEPBDtls")).Text);
                    //Int64 INVC_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblINVCDtls")).Text);
                    //Int64 DA_DtlsID = Convert.ToInt64(((Label)gvrow.FindControl("lblDADtls")).Text);

                    string IDs = "this,'" + data["FS_AdrsDtlsID"].ToString() + "','"
                                         + data["DBK_DtlsID"].ToString() + "','"
                                         + data["DEPB_DtlsID"].ToString() + "','"
                                         + data["INVC_DtlsID"].ToString() + "','"
                                         + data["DA_DtlsID"].ToString() + "'" ;// +"," + data["RefInvcID"].ToString();
                    Edit = Edit.Replace("this", IDs);
                    sb.AppendFormat(@"""8"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("this", IDs);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/INVOICES/ErrorLog"), "SHIPPING BILL DETAILS WebService", ex.Message.ToString());
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
