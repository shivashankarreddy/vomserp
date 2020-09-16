using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Web;
using System.Web.Script.Services;
using System.Text;
using BAL;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace VOMS_ERP.Accounts
{
    /// <summary>
    /// Summary description for BillpmntService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BillpmntService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// FOR Bill Payment Approval Status  
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetBPAStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string RefNo = HttpContext.Current.Request.Params["sSearch_0"];
                string SupNm = HttpContext.Current.Request.Params["sSearch_1"];
                string Lpo = HttpContext.Current.Request.Params["sSearch_2"];
                string InvNo = HttpContext.Current.Request.Params["sSearch_3"];
                string PymntTp = HttpContext.Current.Request.Params["sSearch_4"];
                string PayDt = HttpContext.Current.Request.Params["sSearch_5"];
                string Amt = HttpContext.Current.Request.Params["sSearch_6"];
                string ChqNo = HttpContext.Current.Request.Params["sSearch_7"];
                string ChqDt = HttpContext.Current.Request.Params["sSearch_8"];
                string Bnk = HttpContext.Current.Request.Params["sSearch_9"];
                string Stat = HttpContext.Current.Request.Params["sSearch_10"].ToUpper();

                StringBuilder s = new StringBuilder();

                if (PayDt != "")
                {
                    PayDt = PayDt.Replace("'", "''");
                    DateTime FrmDt = PayDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(PayDt.Split('~')[0].ToString());
                    DateTime EndDat = PayDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(PayDt.Split('~')[1].ToString());
                    if ((FrmDt.ToShortDateString() != "01-01-1900" || FrmDt.ToShortDateString() != "01-01-0001") && EndDat.ToShortDateString() != "31-12-9999")
                        s.Append(" and isnull(PayDate,'') between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (RefNo != "")
                    s.Append(" and v.RefNo LIKE '%" + RefNo.Replace("'", "''") + "%'");
                if (SupNm != "")
                    s.Append(" and v.SupplierNm LIKE '%" + SupNm.Replace("'", "''") + "%'");
                if (Lpo != "")
                    s.Append(" and v.LPONmbr LIKE '%" + Lpo.Replace("'", "''") + "%'");
                if (InvNo != "")
                    s.Append(" and v.PrfInvNo LIKE '%" + InvNo.Replace("'", "''") + "%'");
                if (PymntTp != "")
                    s.Append(" and v.PaymentTp LIKE '%" + PymntTp.Replace("'", "''") + "%'");
                if (ChqDt != "")
                {
                    ChqDt = ChqDt.Replace("'", "''");
                    DateTime CFrmDt = ChqDt.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(ChqDt.Split('~')[0].ToString());
                    DateTime CEndDat = ChqDt.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(ChqDt.Split('~')[1].ToString());
                    if ((CFrmDt.ToShortDateString() != "01-01-1900" || CFrmDt.ToShortDateString() != "01-01-0001") && CEndDat.ToShortDateString() != "31-12-9999")
                        s.Append(" and isnull(v.ChqDt,'') between '" + CFrmDt.ToString("MM/dd/yyyy") + "' and '" + CEndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (Amt != "")
                    s.Append(" and v.ChequeAmount LIKE '%" + Amt.Replace("'", "''") + "%'");
                if (ChqNo != "")
                    s.Append(" and v.ChequeNo LIKE '%" + ChqNo.Replace("'", "''") + "%'");
                if (Bnk != "")
                    s.Append(" and v.BankName LIKE '%" + Bnk.Replace("'", "''") + "%'");
                if (Stat != "")
                    s.Append(" and v.Status LIKE '%" + Stat.Replace("'", "''") + "%'");

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

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
                    orderByClause = "CreatedDate Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
                            declare @MAA TABLE(ApprovalID uniqueidentifier, SupplierNm nvarchar(MAX), RefNo nvarchar(MAX), LPONmbr nvarchar(MAX), 
                                            PrfInvNo nvarchar(MAX),PaymentTp nvarchar(MAX), PayDate datetime, ChequeNo nvarchar(MAX), 
                                            ChqDt datetime, ChequeAmount nvarchar(MAX), BankName nvarchar(MAX), Status nvarchar(MAX),EDIT nvarchar(MAX),Delt nvarchar(MAX), CreatedDate datetime)
                            INSERT
                            INTO
	                            @MAA (ApprovalID,SupplierNm,RefNo,LPONmbr,PrfInvNo, PaymentTp,PayDate, ChequeNo, ChqDt, ChequeAmount,BankName,Status,EDIT,Delt, CreatedDate)
	                            
                                 Select ApprovalID,SupplierNm,RefNo,LPONmbr,PrfInvNo, PaymentTp,PayDate, ChequeNo, ChqDt, ChequeAmount,BankName,Status,EDIT,Delt, CreatedDate from View_BillPaymentApprovalStatus v {4}
                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MAA].ApprovalID)
				                              FROM
					                              @MAA) AS TotalRows
			                               , ( SELECT  count( [@MAA].ApprovalID) FROM @MAA {1}) AS TotalDisplayRows			   
			                               ,[@MAA].ApprovalID
			                               ,[@MAA].SupplierNm      
                                           ,[@MAA].RefNo
                                           ,[@MAA].LPONmbr
                                           ,[@MAA].PrfInvNo
                                           ,[@MAA].PaymentTp
                                           ,[@MAA].PayDate
                                           ,[@MAA].ChequeNo
                                           ,[@MAA].ChqDt 
                                           ,[@MAA].ChequeAmount
                                           ,[@MAA].BankName
                                           ,[@MAA].Status
                                           ,[@MAA].EDIT 
                                           ,[@MAA].Delt
                                           ,[@MAA].CreatedDate 
		                              FROM
			                              @MAA {1}) RawResults) Results 
                                            
                                            WHERE
                	                            RowNumber BETWEEN {2} AND {3} order by CreatedDate Desc";

                string where = " order by CreatedDate desc ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    s.ToString() == "" ? " where v.CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + " '" : " where v.IsActive<>0 and v.CompanyId = '" + HttpContext.Current.Session["CompanyID"].ToString() + " '" + s.ToString());
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
                    sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["ApprovalID"].ToString());
                    sb.Append(",");
                    sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    sb.Append(",");


                    string BRefNO = data["RefNo"].ToString().Replace("\"", "\\\"");
                    string AppId = data["ApprovalID"].ToString().Replace("\"", "\\\"");
                    //string PinvId = data["PrfmaInvcID"].ToString().Replace("\"", "\\\"");
                    BRefNO = BRefNO.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""<a href= BillpaymentApprovalDetails.aspx?ID={0}>{1}</a>""", AppId, BRefNO.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Pinv = data["SupplierNm"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""1"": ""{0}""", Pinv.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CNm = data["LPONmbr"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""2"": ""{0}""", CNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPo = data["PrfInvNo"].ToString().Replace("\"", "\\\"");
                    FPo = FPo.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ChkLst = data["PaymentTp"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""4"": ""{0}""", ChkLst.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""5"": ""{0:dd-MM-yyyy}""", data["PayDate"]);
                    sb.Append(",");

                    string ChqAmt = data["ChequeAmount"].ToString().Replace("\"", "\\\"");
                    ChqAmt = ChqAmt.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ChqAmt.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Chqno = data["ChequeNo"].ToString().Replace("\"", "\\\"");
                    Chqno = Chqno.Replace("\t", "-");
                    sb.AppendFormat(@"""7"": ""{0}""", Chqno.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    sb.AppendFormat(@"""8"": ""{0:dd-MM-yyyy}""", data["ChqDt"]);
                    sb.Append(",");

                    string BnkNm = data["BankName"].ToString().Replace("\"", "\\\"");
                    BnkNm = BnkNm.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", BnkNm.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Statu = data["Status"].ToString().Replace("\"", "\\\"");
                    sb.AppendFormat(@"""10"": ""{0}""", Statu);
                    sb.Append(",");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Accounts/ErrorLog"), "Bill Payment Status WebService", ex.Message.ToString());
                return "";
            }
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;

            return result;
        }

        private string GetStatus(string Val)
        {
            string RetVal = "Created";
            switch (Val)
            {
                case "0": RetVal = "Rejected"; break;
                case "1": RetVal = "Approved"; break;
                case "2": RetVal = "Check Lost/Reverse Bill Payment"; break;
                default: RetVal = "Created"; break;
            }
            return RetVal;
        }

    }
}
