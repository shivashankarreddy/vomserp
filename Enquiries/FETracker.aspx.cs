using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using BAL;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;
using Ajax;
using System.Text;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using ClosedXML.Excel;
using Ionic.Zip;

namespace VOMS_ERP.Enquiries
{
    public partial class FETracker : System.Web.UI.Page
    {
        # region variables
        int res;
        public static ArrayList Files = new ArrayList();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        BAL.FEnquiryBLL frnfenq = new FEnquiryBLL();
        BAL.LEnquiryBLL NLEBL = new LEnquiryBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        BAL.CustomerBLL cusmr = new CustomerBLL();
        BAL.SupplierBLL SUPLRBL = new SupplierBLL();
        CommonBLL CBLL = new CommonBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ErrorLog ELog = new ErrorLog();
        static string GeneralCtgryID;
        static DataSet EditDS;
        static string btnSaveID = "";
        int UserID;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(FETracker));
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    //if (!CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    //    Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    if (!IsPostBack)
                    {
                        BindDropDownList(DDL_Customer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        #region GetDataMethod
        /// <summary>
        /// WebMethod to get the values for filling the grid
        /// </summary>
        /// <returns></returns>
        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(UseHttpGet = true)]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, Method = "GET")]
        //public static string GetData()
        //{
        //    string jsonText = string.Empty;
        //    try
        //    {
        //        string outputJson = string.Empty;
        //        var sb = new StringBuilder();
        //        string ServiceUris = string.Empty;
        //        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        //        int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
        //        int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
        //        string FE = HttpContext.Current.Request.Params["sSearch_1"];
        //        string MFFENo = HttpContext.Current.Request.Params["sSearch_2"];
        //        string MFFEDate = HttpContext.Current.Request.Params["sSearch_3"];
        //        string FPO = HttpContext.Current.Request.Params["sSearch_4"];
        //        string MFFPONo = HttpContext.Current.Request.Params["sSearch_5"];
        //        string MFFPODate = HttpContext.Current.Request.Params["sSearch_6"];
        //        DataSet ds = new DataSet();

        //        ds = CommonBLL.GetFETrackerData(CommonBLL.FlagSelectAll, FE ?? "", MFFEDate ?? "", "", Guid.Empty);

        //        int TotalRowCount = 0;
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            TotalRowCount = Convert.ToInt32(ds.Tables[0].Rows.Count);
        //            DataTable data = ds.Tables[0];

        //            for (int i = 0; i < data.Rows.Count; i++)
        //            {
        //                sb.Append("{");
        //                sb.AppendFormat(@"""DT_RowId"": ""{0}""", data.Rows[i]["CustomerID"].ToString());
        //                sb.Append(",");
        //                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", 2);
        //                sb.Append(",");

        //                sb.AppendFormat(@"""0"": ""{0}""", data.Rows[i]["ENQ received by email"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""1"": ""{0:dd-MM-yyyy}""", data.Rows[i]["FE Received Date"]);
        //                sb.Append(",");

        //                sb.AppendFormat(@"""2"": ""{0}""", data.Rows[i]["PresentStatus"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""3"": ""{0}""", data.Rows[i]["Days to create FE"]);
        //                sb.Append(",");

        //                sb.AppendFormat(@"""4"": ""{0}""", data.Rows[i]["Days to create LE"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""5"": ""{0}""", data.Rows[i]["Days to receive LQ"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""6"": ""{0}""", data.Rows[i]["No of LQ Received"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""7"": ""{0}""", data.Rows[i]["Days to create FQ"].ToString());
        //                sb.Append(",");

        //                sb.AppendFormat(@"""8"": ""{0}""", data.Rows[i]["Total days to reach FQ"].ToString());
        //                sb.Append("},");

        //            }
        //        }
        //        outputJson = sb.ToString();
        //        if (sb.Length > 0)
        //            outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        //        sb.Clear();
        //        sb.Append("{");
        //        sb.Append(@"""sEcho"": ");
        //        sb.AppendFormat(@"""{0}""", sEcho);
        //        sb.Append(",");
        //        sb.Append(@"""iTotalRecords"": ");
        //        sb.Append(TotalRowCount);
        //        sb.Append(",");
        //        sb.Append(@"""iTotalDisplayRecords"": ");
        //        sb.Append(TotalRowCount);
        //        sb.Append(", ");
        //        sb.Append(@"""aaData"": [ ");
        //        sb.Append(outputJson);
        //        sb.Append("]}");
        //        outputJson = sb.ToString();
        //        return outputJson;
        //    }

        //    catch (Exception ex)
        //    {
        //        return null;
        //        throw ex;
        //    }

        //}

        /// <summary>
        /// To Int Conversioin for converting string to integer
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        //public static int ToInt(string toParse)
        //{
        //    int result;
        //    if (int.TryParse(toParse, out result)) return result;

        //    return result;
        //}

        #endregion


        /// <summary>
        /// To Generate & Download a Zip Containing Excel Files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                DataSet _DS = new DataSet();
                DataTable _DT = new DataTable();

                _DS = CommonBLL.GetFETrackerData(CommonBLL.FlagSelectAll, "", "", "", Guid.Empty);

                var Result = _DS.Tables[0].AsEnumerable()
                                .GroupBy(P => P.Field<Guid>("CustomerID"))
                                .Select(C => C.CopyToDataTable()).ToList();

                foreach (DataTable DT in Result)
                {
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        string SheetName = Convert.ToString(DT.Rows[0]["CustomerName"]) == "" ? "Sheet1" : Convert.ToString(DT.Rows[0]["CustomerName"]);
                        string BookName = Convert.ToString(DT.Rows[0]["CustomerName"]) == "" ? "Volta" : Convert.ToString(DT.Rows[0]["CustomerName"]);
                        DT.Columns.Remove("CustomerName");
                        DT.Columns.Remove("CustomerID");
                        var SDT = DT.AsEnumerable().OrderByDescending(P=>P.Field<DateTime>("FE Received Date")).CopyToDataTable();
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            SDT.TableName = SheetName;
                            wb.Worksheets.Add(SDT);
                            string FilePath = Server.MapPath("/UPLOADS/TrackerExcels/" + BookName + ".xlsx");
                            try
                            {
                                if (File.Exists(FilePath))
                                    File.Delete(FilePath);
                            }
                            catch { }
                            wb.SaveAs(FilePath);
                            #region UNUSED

                            //Response.Clear();
                            //Response.Buffer = true;
                            //Response.Charset = "";
                            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            //Response.AddHeader("content-disposition", "attachment;filename=FETracker.xlsx");
                            //using (MemoryStream MyMemoryStream = new MemoryStream())
                            //{
                            //    wb.SaveAs(MyMemoryStream);                            
                            //    MyMemoryStream.WriteTo(Response.OutputStream);
                            //    Response.Flush();
                            //    try
                            //    { HttpContext.Current.Response.End(); }
                            //    catch { }

                            //}
                            #endregion
                        }
                    }
                }
                using (ZipFile _Zip = new ZipFile())
                {
                    _Zip.AddFiles(Directory.GetFiles(Server.MapPath("/UPLOADS/TrackerExcels/"), "*.xlsx"), "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=FETracker_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".zip");
                    Response.ContentType = "application/zip";
                    _Zip.Save(Response.OutputStream);
                    try { Response.End(); }
                    catch { }
                    finally
                    {
                        Array.ForEach(Directory.GetFiles(Server.MapPath("/UPLOADS/TrackerExcels/"), "*.xlsx"), File.Delete);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

        /// <summary>
        /// To Save & Generate render the data to grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string[] SFEs = Convert.ToString(Txt_MFFESearchNo.Value).Split(','); //Txt_MFFESearchNo.Text.Split(',');
                string[] SMFFENos = Txt_MFFENo.Text.Split(',');
                string[] SMFFEDates = Txt_MFFEDate.Text.Split(',');
                string CustName = DDL_Customer.SelectedIndex == 0 ? "" : DDL_Customer.SelectedItem.Text;
                Guid CustID = DDL_Customer.SelectedIndex == 0 ? Guid.Empty : new Guid(DDL_Customer.SelectedItem.Value);
                List<FEKeywords> _SLFE = new List<FEKeywords>();
                for (int i = 0; i < SMFFENos.Count(); i++)
                {
                    _SLFE.Add(new FEKeywords() { MFFENO = SMFFENos[i], MFFEDate = SMFFEDates[0], FESearchNO = SFEs[i], CustomerName = CustName, CustomerID = CustID });
                }

                XElement _XE = new XElement("KeyWords", from FEK in _SLFE select new XElement("KeyWord", new XElement("MFNo", FEK.MFFENO), new XElement("MFDate", FEK.MFFEDate), new XElement("SearchNo", FEK.FESearchNO), new XElement("CustomerName", FEK.CustomerName), new XElement("CustomerID", FEK.CustomerID)));

                CommonBLL.GetFETrackerData(CommonBLL.FlagISelect, _XE.ToString() ?? "", "", "", Guid.Empty);
                Txt_MFFENo.Text = "";
                Txt_MFFESearchNo.Value = "";
                Txt_MFFEDate.Text = "";
                DDL_Customer.SelectedIndex = 0;
               // GetData();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }


        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", Guid.Empty.ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }
    }
    class FEKeywords
    {
        public string MFFENO { get; set; }
        public string MFFEDate { get; set; }
        public string FESearchNO { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerID { get; set; }
    }
}