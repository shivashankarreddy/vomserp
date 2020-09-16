﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using BAL;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;
namespace VOMS_ERP.Invoices
{
    /// <summary>
    /// Summary description for DispatchShipDocStatus
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DispatchShipDocStatus : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        public string GetDispShipDocStatus()
        {
            try
            {
                string SelectedItems = HttpContext.Current.Request.Params["iAllSelectedItems"];
                int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
                int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
                int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
                string rawSearch = HttpContext.Current.Request.Params["sSearch"];


                string AWBNo = HttpContext.Current.Request.Params["sSearch_0"];

                string FPOs = HttpContext.Current.Request.Params["sSearch_1"];
                string CourName = HttpContext.Current.Request.Params["sSearch_2"];
                string CourNo = HttpContext.Current.Request.Params["sSearch_3"];
                string CourSDate = HttpContext.Current.Request.Params["sSearch_4"];
                string DocSBy = HttpContext.Current.Request.Params["sSearch_5"];
                string RecPer = HttpContext.Current.Request.Params["sSearch_6"];
                string RecDate = HttpContext.Current.Request.Params["sSearch_7"];
                string DocCar = HttpContext.Current.Request.Params["sSearch_8"];
                string ContNo = HttpContext.Current.Request.Params["sSearch_9"];
                string RecPer2 = HttpContext.Current.Request.Params["sSearch_10"];
                string RecDate2 = HttpContext.Current.Request.Params["sSearch_11"];
                string CreatedBy = HttpContext.Current.Request.Params["sSearch_12"];


                StringBuilder s = new StringBuilder();

                if (CourSDate != "")
                {
                    CourSDate = CourSDate.Replace("'", "''");
                    DateTime FrmDt = CourSDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(CourSDate.Split('~')[0].ToString());
                    DateTime EndDat = CourSDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(CourSDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and CourierSentDate between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }
                if (AWBNo != "")
                    s.Append(" and AirBillId LIKE '%" + AWBNo.Replace("'", "''") + "%'");


                if (FPOs != "")
                    s.Append(" and FPOIds LIKE '%" + FPOs.Replace("'", "''") + "%'");

                if (CourName != "")
                    s.Append(" and CourierName LIKE '%" + CourName.Replace("'", "''") + "%'");

                if (CourNo != "")
                    s.Append(" and CourierNo LIKE '%" + CourNo.Replace("'", "''") + "%'");

                if (DocSBy != "")
                    s.Append(" and DocumentSentBy LIKE '%" + DocSBy.Replace("'", "''") + "%'");

                if (RecPer != "")
                    s.Append(" and ReceivedPersonCourier LIKE '%" + RecPer.Replace("'", "''") + "%'");

                if (RecDate != "")
                {
                    RecDate = RecDate.Replace("'", "''");
                    DateTime FrmDt = RecDate.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RecDate.Split('~')[0].ToString());
                    DateTime EndDat = RecDate.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RecDate.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ReceivedOnCourier between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                if (DocCar != "")
                    s.Append(" and DocumentCarrier LIKE '%" + DocCar.Replace("'", "''") + "%'");

                if (ContNo != "")
                    s.Append(" and CarrierContactNo LIKE '%" + ContNo.Replace("'", "''") + "%'");

                if (RecPer2 != "")
                    s.Append(" and ReceivedPerson LIKE '%" + RecPer2.Replace("'", "''") + "%'");

                if (CreatedBy != "")
                    s.Append(" and CreatedBy LIKE '%" + RecPer2.Replace("'", "''") + "%'");

                if (RecDate2 != "")
                {
                    RecDate2 = RecDate2.Replace("'", "''");
                    DateTime FrmDt = RecDate2.Split('~')[0].ToString() == "" ? CommonBLL.StartDate : CommonBLL.DateCheck(RecDate2.Split('~')[0].ToString());
                    DateTime EndDat = RecDate2.Split('~')[1].ToString() == "" ? CommonBLL.EndDate : CommonBLL.DateCheck(RecDate2.Split('~')[1].ToString());
                    if (FrmDt.ToShortDateString() != "1/1/0001" && EndDat.ToShortDateString() != "1/1/0001")
                        s.Append(" and ReceivedOn between '" + FrmDt.ToString("MM/dd/yyyy") + "' and '" + EndDat.ToString("MM/dd/yyyy") + "'");
                }

                var sb = new StringBuilder();

                var filteredWhere = string.Empty;

                var wrappedSearch = "'%" + rawSearch + "%'";

                sb.Clear();

                string orderByClause = string.Empty;

                orderByClause = "0 DESC";
                if (!String.IsNullOrEmpty(orderByClause))
                {

                    orderByClause = orderByClause.Replace("0", ", DSDId");
                    orderByClause = orderByClause.Remove(0, 1);
                }
                else
                {
                    orderByClause = "CreatedOn Desc";
                }
                orderByClause = "ORDER BY " + orderByClause;

                sb.Clear();

                var numberOfRowsToReturn = "";
                numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

                string query = @"  
                							declare @MAA TABLE(DSDId Uniqueidentifier,AirBillId nvarchar(MAX), FPOIds nvarchar(MAX),CourierName nvarchar(MAX),  CourierNo nvarchar(MAX),CourierSentDate date,DocumentSentBy nvarchar(MAX),ReceivedPersonCourier nvarchar(MAX),ReceivedOnCourier Date,DocumentCarrier nvarchar(MAX),CarrierContactNo nvarchar(MAX),ReceivedPerson nvarchar(MAX),ReceivedOn Date,CreatedBy  nvarchar(MAX),
                											EDIT nvarchar(MAX),Delt nvarchar(MAX), CreatedOn datetime)
                							INSERT
                							INTO
                								@MAA (DSDId,AirBillId,FPOIds,CourierName,CourierNo,CourierSentDate,DocumentSentBy,ReceivedPersonCourier,ReceivedOnCourier,DocumentCarrier,CarrierContactNo,ReceivedPerson,ReceivedOn,CreatedBy, Edit, Delt, CreatedOn)
                								Select DSDId,AirBillId,FPOIds,CourierName,CourierNo,CourierSentDate,DocumentSentBy,ReceivedPersonCourier,ReceivedOnCourier,DocumentCarrier,CarrierContactNo,ReceivedPerson,ReceivedOn,CreatedBy, Edit, Delt, CreatedOn from view_DispShipDoc im
                								{4}                   
                							SELECT * FROM (SELECT row_number() OVER ({0}) AS RowNumber, * FROM (SELECT (SELECT count([@MAA].DSDId) FROM @MAA) AS TotalRows
                										   , ( SELECT  count( [@MAA].DSDId) FROM @MAA {1}) AS TotalDisplayRows	
                                                           ,[@MAA].DSDId
                                                           ,[@MAA].AirBillId  		   
                										   ,[@MAA].FPOIds   
                										   ,[@MAA].CourierName
                										   ,[@MAA].CourierNo
                                                           ,[@MAA].CourierSentDate
                										   ,[@MAA].DocumentSentBy
                                                           ,[@MAA].ReceivedPersonCourier
                                                           ,[@MAA].ReceivedOnCourier
                                                           ,[@MAA].DocumentCarrier
                                                           ,[@MAA].CarrierContactNo  
                                                           ,[@MAA].ReceivedPerson
                                                           ,[@MAA].ReceivedOn
                                                           ,[@MAA].CreatedBy
                										   ,[@MAA].EDIT,[@MAA].Delt
                										   ,[@MAA].CreatedOn 
                									  FROM
                										  @MAA {1}) RawResults) Results 
                											
                											WHERE
                												RowNumber BETWEEN {2} AND {3} order by CreatedOn Desc";
                Guid CompanyId = new Guid(Session["CompanyID"].ToString());
                string where = " order by im.CreatedOn desc ";
                query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn,
                    s.ToString() == "" ? " where im.CompanyId = " + "'" + CompanyId + "'" + where : " where im.CompanyId = " + "'" + CompanyId + "'" + s.ToString() + "  " + where);
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
                    //sb.Append("{");
                    //sb.AppendFormat(@"""DT_RowId"": ""{0}""", data["SDCId"].ToString());
                    //sb.Append(",");
                    //sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                    //sb.Append(",");

                    sb.Append("{");
                    string AirWBNo = data["AirBillId"].ToString().Replace("\"", "\\\"");
                    //string IOMId = data["IOM2ID"].ToString().Replace("\"", "\\\"");
                    //string PinvId = data["PrfmaInvcID"].ToString().Replace("\"", "\\\"");
                    AirWBNo = AirWBNo.Replace("\t", "-");
                    sb.AppendFormat(@"""0"": ""{0}""", AirWBNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string FPOIds = data["FPOIds"].ToString().Replace("\"", "\\\"");
                    //string IOMId = data["IOM2ID"].ToString().Replace("\"", "\\\"");
                    //string PinvId = data["PrfmaInvcID"].ToString().Replace("\"", "\\\"");
                    FPOIds = FPOIds.Replace("\t", "-");
                    sb.AppendFormat(@"""1"": ""{0}""", FPOIds.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CourierName = data["CourierName"].ToString().Replace("\"", "\\\"");
                    //string IOMId = data["IOM2ID"].ToString().Replace("\"", "\\\"");
                    //string PinvId = data["PrfmaInvcID"].ToString().Replace("\"", "\\\"");
                    CourierName = CourierName.Replace("\t", "-");
                    sb.AppendFormat(@"""2"": ""{0}""", CourierName.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CourierNo = data["CourierNo"].ToString().Replace("\"", "\\\"");
                    CourierNo = CourierNo.Replace("\t", "-");
                    sb.AppendFormat(@"""3"": ""{0}""", CourierNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");





                    //sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data["CommercialInvoiceDate"]);
                    //sb.Append(",");

                    string CourierSendDate = data["CourierSentDate"].ToString().Replace("\"", "\\\"");
                    DateTime oDate = Convert.ToDateTime(CourierSendDate);
                    string odt = oDate.ToString("dd-MM-yyyy");
                    if (CourierSendDate == CommonBLL.EndDate.ToString())
                    {
                        sb.AppendFormat(@"""4"": ""{0:dd-MM-yyyy}""", "");
                    }
                    else
                    {
                        sb.AppendFormat(@"""4"": ""{0:dd-MM-yyyy}""", data["CourierSentDate"]);
                    }
                    sb.Append(",");



                    string DocumentSent = data["DocumentSentBy"].ToString().Replace("\"", "\\\"");
                    DocumentSent = DocumentSent.Replace("\t", "-");
                    sb.AppendFormat(@"""5"": ""{0}""", DocumentSent.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ReceivedPerson = data["ReceivedPersonCourier"].ToString().Replace("\"", "\\\"");
                    ReceivedPerson = ReceivedPerson.Replace("\t", "-");
                    sb.AppendFormat(@"""6"": ""{0}""", ReceivedPerson.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ReceviedOn = data["ReceivedOnCourier"].ToString().Replace("\"", "\\\"");
                    DateTime oDate1 = Convert.ToDateTime(ReceviedOn);
                    if (ReceviedOn == CommonBLL.EndDate.ToString())
                    {
                        sb.AppendFormat(@"""7"": ""{0:dd-MM-yyyy}""", "");
                    }
                    else
                    {
                        sb.AppendFormat(@"""7"": ""{0:dd-MM-yyyy}""", data["ReceivedOnCourier"]);
                    }
                    sb.Append(",");

                    string DocumentCarrier = data["DocumentCarrier"].ToString().Replace("\"", "\\\"");
                    DocumentCarrier = DocumentCarrier.Replace("\t", "-");
                    sb.AppendFormat(@"""8"": ""{0}""", DocumentCarrier.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string CarrierContactNo = data["CarrierContactNo"].ToString().Replace("\"", "\\\"");
                    CarrierContactNo = CarrierContactNo.Replace("\t", "-");
                    sb.AppendFormat(@"""9"": ""{0}""", CarrierContactNo.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Units = data["ReceivedPerson"].ToString().Replace("\"", "\\\"");
                    Units = Units.Replace("\t", "-");
                    sb.AppendFormat(@"""10"": ""{0}""", Units.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string ReceviedOn2 = data["ReceivedOn"].ToString().Replace("\"", "\\\"");
                    DateTime oDate2 = Convert.ToDateTime(ReceviedOn2);
                    if (ReceviedOn2 == CommonBLL.EndDate.ToString())
                    {
                        sb.AppendFormat(@"""11"": ""{0:dd-MM-yyyy}""", "");
                    }
                    else
                    {
                        sb.AppendFormat(@"""11"": ""{0:dd-MM-yyyy}""", data["ReceivedOn"]);
                    }
                    sb.Append(",");


                    string CreatedBy2 = data["CreatedBy"].ToString().Replace("\"", "\\\"");
                    CreatedBy2 = CreatedBy2.Replace("\t", "-");
                    sb.AppendFormat(@"""12"": ""{0}""", CreatedBy2.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");
                    //string CNm = data["Vessel"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""4"": ""{0}""", CNm.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string FPo = data["Sbno"].ToString().Replace("\"", "\\\"");
                    //FPo = FPo.Replace("\t", "-");
                    //sb.AppendFormat(@"""5"": ""{0}""", FPo.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    //string ChkLst = data["Blno"].ToString().Replace("\"", "\\\"");
                    //sb.AppendFormat(@"""6"": ""{0}""", ChkLst.Replace(Environment.NewLine, "\\n"));
                    //sb.Append(",");

                    string Edit = data["EDIT"].ToString().Replace("\"", "\\\"");
                    Edit = Edit.Replace("\t", "-");
                    sb.AppendFormat(@"""13"": ""{0}""", Edit.Replace(Environment.NewLine, "\\n"));
                    sb.Append(",");

                    string Del = data["Delt"].ToString().Replace("\"", "\\\"");
                    Del = Del.Replace("\t", "-");
                    sb.AppendFormat(@"""14"": ""{0}""", Del.Replace(Environment.NewLine, "\\n"));
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
