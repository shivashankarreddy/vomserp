using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Ajax;
using BAL;
//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
//using X14 = DocumentFormat.OpenXml.Office2010.Excel;
//using DocumentFormat.OpenXml.Office2013.Excel;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Net;

//using OfficeOpenXml;
//using OfficeOpenXml.Style;


namespace VOMS_ERP.Enquiries
{
    public partial class FEStatus : System.Web.UI.Page
    {
        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        string qun = "0";
        DataSet dset;

        #endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(FEStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    string date = HttpContext.Current.Request.Params["sSearch_0"];
                    if (Session["AccessRole"].ToString() == "Manager")
                    {
                        Response.Redirect("../Enquiries/FEStatusNew.aspx", false);
                    }
                    else
                    {
                        if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        {

                            if (!IsPostBack)
                            {

                            }

                            //.ImageUrl = "Admin/ShowImg.ashx?id=" + new Guid(Session["CompanyID"].ToString());
                        }
                        else
                            Response.Redirect("../Masters/Home.aspx?NP=no", false);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Get Data and Bind to Controls

        /// <summary>
        /// Redirect to Mail Send Page
        /// </summary>
        /// <param name="FEID"></param>
        private void GenPDF(int FEID)
        {
            try
            {
                Response.Redirect("../Masters/EmailSend.aspx?FeID=" + FEID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Search/Export to Excel/Export to Pdf Buttons Click Events

        /// <summary>
        /// Export to Pdf Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to Excel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                FEnquiryBLL FEBL = new FEnquiryBLL();
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "";
                string FromRcvdDt = "", ToRcvdDt = "", CusID = "";
                Guid LoginID = Guid.Empty;

                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());
                else if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                {
                    CusID = ((ArrayList)Session["UserDtls"])[11].ToString();
                }

                if (Mode == "tdt")
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");

                FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                FromRcvdDt = HFRcvdFromDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRcvdFromDt.Value).ToString("yyyy-MM-dd");
                ToRcvdDt = HFRcvdToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFRcvdToDt.Value).ToString("yyyy-MM-dd");

                string FENo = HFFENo.Value;
                string Subject = HFSubject.Value;
                string Status = HFStatus.Value;
                string dept = HFDept.Value;
                string Cust = HFCust.Value;
                string CnctPrsn = HFCnctPrsn.Value;

                if (Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-0001" || Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "1-1-1900")
                    FrmDt = "";
                if (Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-0001")
                    ToDat = "";
                if (Convert.ToDateTime(FromRcvdDt).ToString("dd-MM-yyyy") == "01-01-0001")//|| Convert.ToDateTime(FromRcvdDt).ToString("dd-MM-yyyy") == "01-01-1900"
                    FromRcvdDt = "";
                if (Convert.ToDateTime(ToRcvdDt).ToString("dd-MM-yyyy") == "01-01-0001")
                    ToRcvdDt = "";

                DataSet ds = FEBL.Select_FESearch(FrmDt, ToDat, FENo, FromRcvdDt, ToRcvdDt, Subject, Status, dept, Cust, CnctPrsn, CreatedDT, LoginID, CusID, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=ForeignEnquirystatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (FrmDt != "" && Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";
                    string MTitle = "STATUS OF FOREIGN ENQUIRIES RECEIVED", MTcustomer = "", MTDTS = "";
                    if (HFCust.Value != "")
                        MTcustomer = HFCust.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");

                    DataGrid dgGrid = new DataGrid();
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    dgGrid.RenderControl(htextw);
                    Response.Write(t.Item1);
                    byte[] imge = null;
                    if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["CompanyLogo"].ToString() != "")
                    {
                        imge = (byte[])(ds.Tables[1].Rows[0]["CompanyLogo"]);
                        using (MemoryStream ms = new MemoryStream(imge))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                            //string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                            string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                            image.Save(FilePath);
                        }
                        //Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png")
                        string headerTable = "<img src='" + CommonBLL.CommonLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    else
                    {
                        string headerTable = "<img src='" + CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
                        Response.Write(headerTable);
                    }
                    Response.Write(stw.ToString());
                    Response.End();
                }
            }

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Exports
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
        {

        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    System.Data.DataTable dt = CommonBLL.EmptyDt();
                    System.Data.DataTable DtSubItems = CommonBLL.EmptyDt();
                    NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                    //if (HttpContext.Current.Session["AllFESubItems"].ToString() != "")
                    //    DtSubItems = (System.Data.DataTable)HttpContext.Current.Session["AllFESubItems"];
                    //else
                    DtSubItems = CommonBLL.FEEmpty_SubItems();
                    res = NEBLL.NewEnquiryInsert(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, "", "",
                        DateTime.Now, DateTime.Now, DateTime.Now, "", Guid.Empty, 0, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), false, new Guid(Session["CompanyID"].ToString()), dt, DtSubItems);
                }
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                    result = "Error::Cannot Delete this Record, LE already created so delete LE/ Error while Deleting " + ID;

                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string IsRegret(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Regret
                if (result == "Success")
                {
                    System.Data.DataTable dt = CommonBLL.EmptyDt();
                    NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                    res = NEBLL.IsReg(CommonBLL.FlagINewInsert, Convert.ToInt64(ID), 0, 0, "", "",
                        DateTime.Now, DateTime.Now, DateTime.Now, "", 0, "", "", 1, 1, false, 1, dt);
                }
                if (res == 0)
                    result = "Success:: Regreted Sucess";
                else
                    result = "Error::Cannot Regret" + ID;
                #endregion
                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

        #region ****Notin Use***** (Export Report to Excel)

        /// <summary>
        /// Method for Comprision Items Table
        /// </summary>
        /// <param name="ds"></param>
        public string Export_Report_Excel(DataSet ds)
        {
            try
            {
                string RecvdDt = ds.Tables[0].Rows[0]["ReceivedDate"].ToString();
                string DueDt = (ds.Tables[0].Rows[0]["DueDate"].ToString());
                if (RecvdDt == "31-12-9999 00:00:00")
                    RecvdDt = "";
                if (DueDt == "31-12-9999 00:00:00")
                    DueDt = "";
                if (RecvdDt != "31-12-9999 00:00:00" && RecvdDt != "")
                {
                    DateTime Rdt = Convert.ToDateTime(RecvdDt);
                    RecvdDt = Rdt.ToString("dd-MM-yyyy");
                }
                if (DueDt != "31-12-9999 00:00:00" && DueDt != "")
                {
                    DateTime Duedt = Convert.ToDateTime(DueDt);
                    DueDt = Duedt.ToString("dd-MM-yyyy");
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<table id=ExportReport width='100%'> <tr><td colspan=3>");
                sb.Append("<table id=ExportReport1 width='100%' border = '1px'>");
                sb.Append("<tr>");
                sb.Append("<th colspan=2 align='center'>" + ds.Tables[2].Rows[0]["OrgName"].ToString() + "</th>");
                sb.Append("<td valign='top' rowspan='4' colspan=3><b>Address:</b><span>" + ds.Tables[0].Rows[0]["CustAdd"].ToString() + "</span></td>");
                sb.Append("</tr><tr>");
                sb.Append("<td colspan=1>To </td> <td> <span>" + ds.Tables[0].Rows[0]["CompanyName"].ToString() + "</span></td>");
                sb.Append("</tr><tr>");
                sb.Append("<td colspan=1>Phone</td> <td align='left'><span>" + ds.Tables[0].Rows[0]["PhneNo"].ToString() + "</span></td>");
                sb.Append("</tr><tr>");
                sb.Append("<td colspan=1>Subject</td> <td><span>" + ds.Tables[0].Rows[0]["Subject"].ToString() + "</span></td>");
                sb.Append("</tr><tr>");
                sb.Append("<td colspan='1'>Ref. No.</td> <td><span>" + ds.Tables[0].Rows[0]["EnquireNumber"].ToString() + " - " + Convert.ToDateTime(ds.Tables[0].Rows[0]["EnquiryDate"].ToString()).ToString("dd-MM-yyyy") + "</span></td>");
                sb.Append("<td colspan='1'>Project/Department</td> <td colspan='2' align='left'><span>" + ds.Tables[0].Rows[0]["DeptName"].ToString() + "</span></td>");
                sb.Append("</tr><tr>");
                sb.Append("<td colspan='1'>Rec. Date</td> <td align='left'><span>" + RecvdDt + "</span></td>");
                sb.Append("<td colspan='1'>DueDate</td> <td colspan='2' align='left'><span>" + DueDt + "</span></td>");
                sb.Append("</tr></table></td></tr> <tr><td></td></tr> <tr><td colspan=3> <table width='100%'><tr>");
                sb.Append("<th align='center'  colspan=5>PURCHASE ENQUIRY</th></tr><tr>");
                sb.Append("<td colspan=5>Please quote for the following items at the earliest possible</td></tr></table> </td></tr> <tr><td colspan=3> <table width='10%' border = '1px'><tr>");
                sb.Append("<th>Sl.No</th><th colspan='3'>Item Description</th><th>Quantity</th></tr>");
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    string Spec = "", Make = "", PartNo = "";
                    if (ds.Tables[1].Rows[i]["Specifications"].ToString().Trim() != "")
                        Spec = " ; Spec:" + ds.Tables[1].Rows[i]["Specifications"].ToString();
                    if (ds.Tables[1].Rows[i]["Make"].ToString().Trim() != "")
                        Make = " ; Make:" + ds.Tables[1].Rows[i]["Make"].ToString();
                    if (ds.Tables[1].Rows[i]["PartNumber"].ToString().Trim() != "")
                        PartNo = " ; PartNo:" + ds.Tables[1].Rows[i]["PartNumber"].ToString();
                    sb.Append("<tr><td align='middle' style= 'width:85% !important'>" + ds.Tables[1].Rows[i]["FESNo"].ToString() + "</td>" +
                        "<td colspan='3' style= 'width:80% !important'>" + ds.Tables[1].Rows[i]["Description"].ToString() + Spec +
                        Make + PartNo +
                        "<td align='center' style= 'width:10% !important'>" + ds.Tables[1].Rows[i]["Quantity"].ToString() + " " +
                        ds.Tables[1].Rows[i]["Units"].ToString() + " </td></tr>");
                }
                sb.Append("</table></td></tr> <tr><td colspan=5> <table><tr><th align='left' colspan=3>Important Instructions :" + ds.Tables[0].Rows[0]["Instruction"].ToString() + "</th></tr><tr>");
                sb.Append("<th align='left'>Best Regards,</th></tr>");
                sb.Append("<tr><td colspan='5' align='left'>For " + ds.Tables[2].Rows[0]["OrgName"].ToString() + " </td></tr>");
                sb.Append("<tr><td colspan='5' align='left'>" + ds.Tables[0].Rows[0]["CustContactPersopn"].ToString() + "</td></tr>");
                sb.Append("<tr><td colspan='5' algin='left'>" + ds.Tables[0].Rows[0]["UsrDesignation"].ToString() + "</td></tr>");
                sb.Append("</table> </td></tr></table>");
                // Exceldiv.InnerHtml = sb.ToString();
                // divExcel.InnerHtml = sb.ToString();
                string res = sb.ToString();

                return res;
            }
            catch (Exception ex)
            {
                ErrorLog ELog = new ErrorLog();
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(HttpContext.Current.Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
                return ErrMsg;
            }
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ReportExp(string Id)
        {

            string filePath = System.IO.Path.GetFullPath(Server.MapPath("test12345.xlsx"));
            System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);

            try
            {
                DataSet ds = CRPTBLL.GetFenqDetails_Items(new Guid(Id), new Guid(Session["CompanyID"].ToString()));
                //InsertText(aFilePath, ds, Id);
                string ExcelData = Export_Report_Excel(ds);
                return ExcelData;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion

        #region Export Report ***Working*** (Testing)

        #region Invoking the Create and Insert Methods of Export.
        //[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        //public string ReportExp(string Id)
        //{

        //    string filePath = System.IO.Path.GetFullPath(Server.MapPath("test12345.xlsx"));
        //    System.IO.FileInfo targetFile = new System.IO.FileInfo(filePath);

        //    try
        //    {
        //        DataSet ds = CRPTBLL.GetFenqDetails_Items(new Guid(Id), new Guid(Session["CompanyID"].ToString()));
        //        //InsertText(aFilePath, ds, Id);
        //        string ExcelData = Export_Report_Excel(ds);
        //        return ExcelData;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

        private void DownloadFile(string fname, bool forceDownload)
        {
            string path = MapPath(fname);
            string name = Path.GetFileName(path);
            string ext = Path.GetExtension(path);
            string type = "";
            // set known types based on file extension  
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".htm":
                    case ".html":
                        type = "text/HTML";
                        break;

                    case ".txt":
                        type = "text/plain";
                        break;

                    case ".doc":
                    case ".rtf":
                        type = "Application/msword";
                        break;
                    case ".xls":
                    case ".xlsx":
                        type = "Application/msexcel";
                        break;
                }
            }
            if (forceDownload)
            {
                System.Web.HttpContext.Current.Response.AppendHeader("content-disposition",
                    "attachment; filename=" + name);
            }
            if (type != "")
                System.Web.HttpContext.Current.Response.ContentType = type;
            System.Web.HttpContext.Current.Response.WriteFile(path);
            System.Web.HttpContext.Current.Response.End();
        }
        #endregion

        #region Merging of Two Cells

        // Given a Worksheet and a cell name, verifies that the specified cell exists.
        // If it does not exist, creates a new cell. 
        //private static void CreateSpreadsheetCellIfNotExist(DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet, string cellName)
        //{
        //    string columnName = GetColumnName(cellName);
        //    uint rowIndex = GetRowIndex(cellName);

        //    IEnumerable<Row> rows = worksheet.Descendants<Row>().Where(r => r.RowIndex.Value == rowIndex);

        //    // If the Worksheet does not contain the specified row, create the specified row.
        //    // Create the specified cell in that row, and insert the row into the Worksheet.
        //    if (rows.Count() == 0)
        //    {
        //        Row row = new Row() { RowIndex = new UInt32Value(rowIndex) };
        //        Cell cell = new Cell() { CellReference = new StringValue(cellName) };
        //        row.Append(cell);
        //        worksheet.Descendants<SheetData>().First().Append(row);
        //        worksheet.Save();
        //    }
        //    else
        //    {
        //        Row row = rows.First();

        //        IEnumerable<Cell> cells = row.Elements<Cell>().Where(c => c.CellReference.Value == cellName);

        //        // If the row does not contain the specified cell, create the specified cell.
        //        if (cells.Count() == 0)
        //        {
        //            Cell cell = new Cell() { CellReference = new StringValue(cellName) };
        //            row.Append(cell);
        //            worksheet.Save();
        //        }
        //    }
        //}

        //// Given a cell name, parses the specified cell to get the column name.
        //private static string GetColumnName(string cellName)
        //{
        //    // Create a regular expression to match the column name portion of the cell name.
        //    Regex regex = new Regex("[A-Za-z]+");
        //    Match match = regex.Match(cellName);

        //    return match.Value;
        //}

        // Given a cell name, parses the specified cell to get the row index.
        private static uint GetRowIndex(string cellName)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            //Regex regex = new Regex("[A-Za-z]+");
            //Regex regex = new Regex("[A-Za-z0-9]+");
            Match match = regex.Match(cellName);

            return uint.Parse(match.Value);
        }

        // Given a SpreadsheetDocument and a worksheet name, get the specified worksheet.
        //private static DocumentFormat.OpenXml.Spreadsheet.Worksheet GetWorksheet(SpreadsheetDocument document, string worksheetName)
        //{

        //    IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == worksheetName);
        //    WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
        //    if (sheets.Count() == 0)
        //        return null;
        //    else
        //        return worksheetPart.Worksheet;
        //}

        // Given a document name, a worksheet name, and the names of two adjacent cells, merges the two cells.
        // When two cells are merged, only the content from one cell is preserved:
        // the upper-left cell for left-to-right languages or the upper-right cell for right-to-left languages.
        //private void MergeTwoCells(string docName, string sheetName, string cell1Name, string cell2Name)
        //{
        //    try
        //    {

        //        using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, true))
        //        {
        //            DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet = GetWorksheet(document, sheetName);
        //            if (worksheet == null || string.IsNullOrEmpty(cell1Name) || string.IsNullOrEmpty(cell2Name))
        //            {
        //                return;
        //            }

        //            // Verify if the specified cells exist, and if they do not exist, create them.
        //            CreateSpreadsheetCellIfNotExist(worksheet, cell1Name);
        //            CreateSpreadsheetCellIfNotExist(worksheet, cell2Name);

        //            MergeCells mergeCells;
        //            if (worksheet.Elements<MergeCells>().Count() > 0)
        //            {
        //                mergeCells = worksheet.Elements<MergeCells>().First();
        //            }
        //            else
        //            {
        //                mergeCells = new MergeCells();

        //                // Insert a MergeCells object into the specified position.
        //                if (worksheet.Elements<CustomSheetView>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
        //                }
        //                else if (worksheet.Elements<DataConsolidate>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<DataConsolidate>().First());
        //                }
        //                else if (worksheet.Elements<SortState>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SortState>().First());
        //                }
        //                else if (worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.AutoFilter>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.AutoFilter>().First());
        //                }
        //                else if (worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Scenarios>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Scenarios>().First());
        //                }
        //                else if (worksheet.Elements<ProtectedRanges>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<ProtectedRanges>().First());
        //                }
        //                else if (worksheet.Elements<SheetProtection>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetProtection>().First());
        //                }
        //                else if (worksheet.Elements<SheetCalculationProperties>().Count() > 0)
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetCalculationProperties>().First());
        //                }
        //                else
        //                {
        //                    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
        //                }
        //            }

        //            // Create the merged cell and append it to the MergeCells collection.
        //            MergeCell mergeCell = new MergeCell() { Reference = new StringValue(cell1Name + ":" + cell2Name) };
        //            mergeCells.Append(mergeCell);

        //            worksheet.Save();
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());

        //    }

        //}

        #endregion

        #region Create and Inserting data to the SpreadSheet.

        ////public void InsertText(string docName, DataSet ds, string Id)
        ////{
        ////    try
        ////    //{
        ////    //    DataSet Items;
        ////    //    uint j;
        ////    //    // Open the document for editing.
        ////    //    using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
        ////    //    {
        ////    //        // Get the SharedStringTablePart. If it does not exist, create a new one.
        ////    //        SharedStringTablePart shareStringPart;
        ////    //        if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        ////    //        {
        ////    //            shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        ////    //        }
        ////    //        else
        ////    //        {
        ////    //            shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
        ////    //        }

        ////    //        // Insert a new worksheet.
        ////    //        WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

        ////    //        // Create Styles and Insert into Workbook
        ////    //        WorkbookStylesPart stylesPart = spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        ////    //        CustomStyleSheet CSS = new CustomStyleSheet();
        ////    //        Stylesheet styles = CSS.CustomStyleshet();
        ////    //        styles.Save(stylesPart);

        ////    //        string OrgName = ds.Tables[0].Rows[0]["ORG"].ToString();
        ////    //        OrgName += ds.Tables[0].Rows[0]["StateCountry"].ToString();
        ////    //        int index = InsertSharedStringItem(OrgName, shareStringPart);
        ////    //        Cell cell = InsertCellInWorksheet("H", 1, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("I", 1, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("J", 1, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("K", 1, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;


        ////    //        index = InsertSharedStringItem(OrgName, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;
        ////    //        //cell.StyleIndex = 7;
        ////    //        //worksheetPart.Worksheet.SheetFormatProperties.DefaultRowHeight = 150D;

        ////    //        cell = InsertCellInWorksheet("C", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("D", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("E", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("F", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        string Address = "Address :";
        ////    //        index = InsertSharedStringItem(Address, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("G", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;
        ////    //        //cell.StyleIndex = 7;

        ////    //        string CustAdd = ds.Tables[0].Rows[0]["CustAdd"].ToString().Replace(",", "," + System.Environment.NewLine);
        ////    //        index = InsertSharedStringItem(CustAdd, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("H", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 5;

        ////    //        cell = InsertCellInWorksheet("I", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 5;

        ////    //        cell = InsertCellInWorksheet("J", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 5;

        ////    //        cell = InsertCellInWorksheet("K", 2, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 5;

        ////    //        string TO = "To :";
        ////    //        index = InsertSharedStringItem(TO, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 3, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string CompName = ds.Tables[0].Rows[0]["CompanyName"].ToString();
        ////    //        index = InsertSharedStringItem(CompName, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 3, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("D", 3, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("E", 3, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("F", 3, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string Phone = "Phone :";
        ////    //        index = InsertSharedStringItem(Phone, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 4, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string PhneNum = ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////    //        index = InsertSharedStringItem(PhneNum, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 4, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("D", 4, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("E", 4, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("F", 4, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string Subject = "Subject :";// +ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////    //        index = InsertSharedStringItem(Subject, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string Subject1 = ds.Tables[0].Rows[0]["Subject"].ToString();
        ////    //        index = InsertSharedStringItem(Subject1, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("D", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("E", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;


        ////    //        cell = InsertCellInWorksheet("F", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;


        ////    //        cell = InsertCellInWorksheet("G", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("H", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("I", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("J", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("K", 5, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;


        ////    //        string RefnoDt = "Ref. No. :";
        ////    //        index = InsertSharedStringItem(RefnoDt, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("B", 7, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string EnqNum = ds.Tables[0].Rows[0]["EnquireNumber"].ToString() + "-" + Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        ////    //        index = InsertSharedStringItem(EnqNum, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("D", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("E", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("F", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        string PrjDept = "Project/" + System.Environment.NewLine + "Department :";
        ////    //        index = InsertSharedStringItem(PrjDept, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("G", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 5;

        ////    //        cell = InsertCellInWorksheet("G", 7, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;


        ////    //        string DeptNam = ds.Tables[0].Rows[0]["DeptName"].ToString();
        ////    //        index = InsertSharedStringItem(DeptNam, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("I", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("I", 7, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 3;

        ////    //        cell = InsertCellInWorksheet("J", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("K", 6, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("K", 7, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;


        ////    //        string RecDate = "Rec. Date :";
        ////    //        index = InsertSharedStringItem(RecDate, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string RecDat = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        ////    //        index = InsertSharedStringItem(RecDat, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("D", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("E", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("F", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string DueDt = "Due Date :";
        ////    //        index = InsertSharedStringItem(DueDt, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("G", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string DateDue = Convert.ToDateTime(ds.Tables[0].Rows[0]["DueDate"]).ToString("dd/MM/yyyy");
        ////    //        index = InsertSharedStringItem(DateDue, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("H", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("I", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("J", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("K", 8, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string PurchseEnq = " PURCHASE ENQUIRY ";
        ////    //        index = InsertSharedStringItem(PurchseEnq, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("E", 10, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 4;

        ////    //        string Note = " Please quote for the following TOOLS FOR ELECTRICAL AND MECHANICAL, at the earliest possible ";
        ////    //        index = InsertSharedStringItem(Note, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 12, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);


        ////    //        string Slno = "Sl. No.";
        ////    //        index = InsertSharedStringItem(Slno, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string ItmDes = " Item Description ";
        ////    //        index = InsertSharedStringItem(ItmDes, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("C", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("D", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("E", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("F", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("G", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("H", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("I", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        string Qant = " Quantity";
        ////    //        index = InsertSharedStringItem(Qant, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("J", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        cell = InsertCellInWorksheet("K", 14, worksheetPart);
        ////    //        cell.CellValue = new CellValue("");
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        cell.StyleIndex = 9;

        ////    //        Items = CRPTBLL.GetFenqAllDetails_Items(new Guid(Id));

        ////    //        j = 15;
        ////    //        for (int i = 0; i < Items.Tables[0].Rows.Count; i++)
        ////    //        {
        ////    //            int SrNo = Convert.ToInt16(Items.Tables[0].Rows[i]["SrNo"].ToString());
        ////    //            index = InsertSharedStringItem(SrNo.ToString(), shareStringPart);
        ////    //            cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue(index.ToString());
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            string Descrip = Items.Tables[0].Rows[i]["ItemDescription"] + ";"
        ////    //                + (Items.Tables[0].Rows[i]["Specifications"].ToString() != "" ? Items.Tables[0].Rows[i]["Specifications"] + ";" : "")
        ////    //                + (Items.Tables[0].Rows[i]["Make"].ToString() != "" ? Items.Tables[0].Rows[i]["Make"] + ";" : "")
        ////    //                + (Items.Tables[0].Rows[i]["PartNumber"].ToString() != "" ? Items.Tables[0].Rows[i]["PartNumber"] : "");

        ////    //            index = InsertSharedStringItem(Descrip, shareStringPart);
        ////    //            cell = InsertCellInWorksheet("C", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue(index.ToString());
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("D", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("E", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("F", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("G", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("H", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("I", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            string Units = Items.Tables[0].Rows[i]["UOM"].ToString();
        ////    //            index = InsertSharedStringItem(Units, shareStringPart);
        ////    //            cell = InsertCellInWorksheet("J", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue(index.ToString());
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            cell = InsertCellInWorksheet("K", j, worksheetPart);
        ////    //            cell.CellValue = new CellValue("");
        ////    //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //            cell.StyleIndex = 9;

        ////    //            j++;
        ////    //        }
        ////    //        //j = 58; //Convert.ToUInt32(Items.Tables[0].Rows.Count.ToString());
        ////    //        j = j + Convert.ToUInt32(Items.Tables[0].Rows.Count) - 8;
        ////    //        string Imp = "Important Instructions:";
        ////    //        index = InsertSharedStringItem(Imp, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        j++;
        ////    //        string Best = "Best Regards,";
        ////    //        index = InsertSharedStringItem(Best, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////    //        //bool isBold = cellData.Style.Font.IsBold;
        ////    //        j++;
        ////    //        string For = " For " + ds.Tables[0].Rows[0]["ORG"].ToString();
        ////    //        index = InsertSharedStringItem(For, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////    //        j = j + 2;
        ////    //        string PersonName = ds.Tables[0].Rows[0]["CustContactPersopn"].ToString();
        ////    //        index = InsertSharedStringItem(PersonName, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////    //        j++;
        ////    //        string Com = "Commercial Manager";
        ////    //        index = InsertSharedStringItem(Com, shareStringPart);
        ////    //        cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////    //        cell.CellValue = new CellValue(index.ToString());
        ////    //        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////    //        //Save the new worksheet.
        ////    //        worksheetPart.Worksheet.Save();
        ////    //    }

        ////    //    string sheetName = "Sheet1";
        ////    //    string cell1Name = "B2";
        ////    //    string cell2Name = "F2";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "H1"; cell2Name = "K1";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "H2"; cell2Name = "K2";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "E10"; cell2Name = "K10";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "B12"; cell2Name = "K12";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C2"; cell2Name = "K2";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C3"; cell2Name = "F3";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C4"; cell2Name = "F4";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C5"; cell2Name = "K5";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "B6"; cell2Name = "B7";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "B6"; cell2Name = "H7";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C6"; cell2Name = "F7";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "G6"; cell2Name = "H7";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "I6"; cell2Name = "K7";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C8"; cell2Name = "F8";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "H8"; cell2Name = "K8";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "C14"; cell2Name = "I14";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //    cell1Name = "J14"; cell2Name = "K14";
        ////    //    MergeTwoCells(docName, sheetName, cell1Name, cell2Name);
        ////    //    j = 15;
        ////    //    for (int i = 0; i < Items.Tables[0].Rows.Count; i++)
        ////    //    {
        ////    //        cell1Name = "C" + j; cell2Name = "I" + j;
        ////    //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        ////    //        cell1Name = "J" + j; cell2Name = "K" + j;
        ////    //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);
        ////    //        j++;

        ////    //    }

        ////    //}
        ////    catch (Exception ex)
        ////    {
        ////        string ErrMsg = ex.Message;
        ////        int LineNo = ExceptionHelper.LineNumber(ex);
        ////        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
        ////    }
        ////}

        ////private void SetRowsReight(Row lstRows, UInt32 StartRowIndex, UInt32 EndRowIndex, double RowHeight)
        ////{
        ////    //foreach (Row row in lstRows)
        ////    //{
        ////        lstRows.Height = RowHeight;
        ////        lstRows.CustomHeight = true;
        ////    //}
        ////}

        ////// Given a document name and text, 
        ////// inserts a new work sheet and writes the text to cell "A1" of the new worksheet.

        ////public void InsertText(string docName, DataSet ds, string Id)
        ////{
        ////    try
        ////    {
        ////        // Open the document for editing.
        ////        using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
        ////        {
        ////            // Get the SharedStringTablePart. If it does not exist, create a new one.
        ////            SharedStringTablePart shareStringPart;
        ////            if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        ////            {
        ////                shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        ////            }
        ////            else
        ////            {
        ////                shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
        ////            }

        ////            // Insert a new worksheet.

        ////            WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

        ////            string OrgName = ds.Tables[0].Rows[0]["ORG"].ToString();
        ////            OrgName += ds.Tables[0].Rows[0]["StateCountry"].ToString();
        ////            int index = InsertSharedStringItem(OrgName, shareStringPart);
        ////            Cell cell = InsertCellInWorksheet("B", 2, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);


        ////            string Address = "Address :";
        ////            index = InsertSharedStringItem(Address, shareStringPart);
        ////            cell = InsertCellInWorksheet("G", 2, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string CustAdd = ds.Tables[0].Rows[0]["CustAdd"].ToString().Replace(",", "," + System.Environment.NewLine);
        ////            index = InsertSharedStringItem(CustAdd, shareStringPart);
        ////            cell = InsertCellInWorksheet("H", 2, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);


        ////            string TO = "To :";// +ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////            index = InsertSharedStringItem(TO, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 3, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string CompName = ds.Tables[0].Rows[0]["CompanyName"].ToString();
        ////            index = InsertSharedStringItem(CompName, shareStringPart);
        ////            cell = InsertCellInWorksheet("C", 3, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);


        ////            string Phone = "Phone :";// +ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////            index = InsertSharedStringItem(Phone, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 4, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string PhneNum = ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////            index = InsertSharedStringItem(PhneNum, shareStringPart);
        ////            cell = InsertCellInWorksheet("C", 4, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string Subject = "Subject :";// +ds.Tables[0].Rows[0]["PhneNo"].ToString();
        ////            index = InsertSharedStringItem(Subject, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 5, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string Subject1 = ds.Tables[0].Rows[0]["Subject"].ToString();
        ////            index = InsertSharedStringItem(Subject1, shareStringPart);
        ////            cell = InsertCellInWorksheet("C", 5, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string RefnoDt = "Ref. No. :";
        ////            index = InsertSharedStringItem(RefnoDt, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 6, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string EnqNum = ds.Tables[0].Rows[0]["EnquireNumber"].ToString() + "-" + Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        ////            index = InsertSharedStringItem(EnqNum, shareStringPart);
        ////            cell = InsertCellInWorksheet("C", 6, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string PrjDept = "Project/" + System.Environment.NewLine + "Department :";
        ////            index = InsertSharedStringItem(PrjDept, shareStringPart);
        ////            cell = InsertCellInWorksheet("G", 6, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string DeptNam = ds.Tables[0].Rows[0]["DeptName"].ToString();
        ////            index = InsertSharedStringItem(DeptNam, shareStringPart);
        ////            cell = InsertCellInWorksheet("H", 6, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string RecDate = "Rec. Date :";
        ////            index = InsertSharedStringItem(RecDate, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 8, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string RecDat = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        ////            index = InsertSharedStringItem(RecDat, shareStringPart);
        ////            cell = InsertCellInWorksheet("C", 8, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string DueDt = "Due Date :";
        ////            index = InsertSharedStringItem(DueDt, shareStringPart);
        ////            cell = InsertCellInWorksheet("G", 8, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string DateDue = Convert.ToDateTime(ds.Tables[0].Rows[0]["DueDate"]).ToString("dd/MM/yyyy");
        ////            index = InsertSharedStringItem(DateDue, shareStringPart);
        ////            cell = InsertCellInWorksheet("H", 8, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string PurchseEnq = " PURCHASE ENQUIRY ";
        ////            index = InsertSharedStringItem(PurchseEnq, shareStringPart);
        ////            cell = InsertCellInWorksheet("F", 10, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string Note = " Please quote for the following TOOLS FOR ELECTRICAL AND MECHANICAL, at the earliest possible ";
        ////            index = InsertSharedStringItem(Note, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 12, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string Slno = "Sl. No.";
        ////            index = InsertSharedStringItem(Slno, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", 14, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string ItmDes = " Item Description ";
        ////            index = InsertSharedStringItem(ItmDes, shareStringPart);
        ////            cell = InsertCellInWorksheet("D", 14, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            string Qant = " Quantity";
        ////            index = InsertSharedStringItem(Qant, shareStringPart);
        ////            cell = InsertCellInWorksheet("F", 14, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            DataSet Items = CRPTBLL.GetFenqAllDetails_Items(new Guid(Id));

        ////            uint j = 15;
        ////            for (int i = 0; i < Items.Tables[0].Rows.Count; i++)
        ////            {
        ////                int SrNo = Convert.ToInt16(Items.Tables[0].Rows[i]["SrNo"].ToString());
        ////                index = InsertSharedStringItem(SrNo.ToString(), shareStringPart);
        ////                cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////                cell.CellValue = new CellValue(index.ToString());
        ////                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////                string Descrip = Items.Tables[0].Rows[i]["ItemDescription"] + ";"
        ////                    + (Items.Tables[0].Rows[i]["Specifications"].ToString() != "" ? Items.Tables[0].Rows[i]["Specifications"] + ";" : "")
        ////                    + (Items.Tables[0].Rows[i]["Make"].ToString() != "" ? Items.Tables[0].Rows[i]["Make"] + ";" : "")
        ////                    + (Items.Tables[0].Rows[i]["PartNumber"].ToString() != "" ? Items.Tables[0].Rows[i]["PartNumber"] : "");

        ////                index = InsertSharedStringItem(Descrip, shareStringPart);
        ////                cell = InsertCellInWorksheet("D", j, worksheetPart);
        ////                cell.CellValue = new CellValue(index.ToString());
        ////                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////                string Units = Items.Tables[0].Rows[i]["UOM"].ToString();
        ////                index = InsertSharedStringItem(Units, shareStringPart);
        ////                cell = InsertCellInWorksheet("F", j, worksheetPart);
        ////                cell.CellValue = new CellValue(index.ToString());
        ////                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////                j++;
        ////            }
        ////            //j = 58; //Convert.ToUInt32(Items.Tables[0].Rows.Count.ToString());
        ////            j = j + Convert.ToUInt32(Items.Tables[0].Rows.Count) + 1;
        ////            string Best = "Best Regards,";
        ////            index = InsertSharedStringItem(Best, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        ////            j++;
        ////            string For = " For " + ds.Tables[0].Rows[0]["ORG"].ToString();
        ////            index = InsertSharedStringItem(For, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            j = j + 2;
        ////            string PersonName = ds.Tables[0].Rows[0]["CustContactPersopn"].ToString();
        ////            index = InsertSharedStringItem(PersonName, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            j++;
        ////            string Com = "Commercial Manager";
        ////            index = InsertSharedStringItem(PersonName, shareStringPart);
        ////            cell = InsertCellInWorksheet("B", j, worksheetPart);
        ////            cell.CellValue = new CellValue(index.ToString());
        ////            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        ////            // Save the new worksheet.
        ////            worksheetPart.Worksheet.Save();
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        string ErrMsg = ex.Message;
        ////        int LineNo = ExceptionHelper.LineNumber(ex);
        ////        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
        ////    }
        ////}




        //// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        //// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.

        //public void InsertText(string docName, DataSet ds, string Id)
        //{
        //    try
        //    {
        //        DataSet Items;
        //        uint j;
        //        // Open the document for editing.
        //        using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
        //        {
        //            // Get the SharedStringTablePart. If it does not exist, create a new one.
        //            SharedStringTablePart shareStringPart;
        //            if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
        //            {
        //                shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        //            }
        //            else
        //            {
        //                shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
        //            }

        //            // Insert a new worksheet.
        //            WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);


        //            // Disable the Gridlines in Worksheet
        //            worksheetPart.Worksheet = new Worksheet(new SheetViews(new SheetView() { WorkbookViewId = 0, ShowGridLines = new BooleanValue(false) }), new SheetData());



        //            // Create Styles and Insert into Workbook
        //            WorkbookStylesPart stylesPart = spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
        //            CustomStyleSheet CSS = new CustomStyleSheet();
        //            Stylesheet styles = CSS.CustomStyleshet();
        //            styles.Save(stylesPart);



        //            string OrgName = ds.Tables[0].Rows[0]["ORG"].ToString();
        //            OrgName += ds.Tables[0].Rows[0]["StateCountry"].ToString();
        //            int index = InsertSharedStringItem(OrgName, shareStringPart);
        //            Cell cell = InsertCellInWorksheet("H", 1, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 7;

        //            cell = InsertCellInWorksheet("I", 1, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 7;

        //            cell = InsertCellInWorksheet("J", 1, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 7;

        //            cell = InsertCellInWorksheet("K", 1, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 7;

        //            index = InsertSharedStringItem(OrgName, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 2, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("C", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("D", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("E", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("F", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("G", 1, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 11;

        //            string Address = "Address :";
        //            index = InsertSharedStringItem(Address, shareStringPart);
        //            cell = InsertCellInWorksheet("G", 2, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 11;


        //            string CustAdd = ds.Tables[0].Rows[0]["CustAdd"].ToString().Replace(",", "," + System.Environment.NewLine);
        //            index = InsertSharedStringItem(CustAdd, shareStringPart);
        //            cell = InsertCellInWorksheet("H", 2, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 5;

        //            cell = InsertCellInWorksheet("H", 3, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 2;

        //            cell = InsertCellInWorksheet("H", 4, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 2;

        //            cell = InsertCellInWorksheet("I", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 5;

        //            cell = InsertCellInWorksheet("J", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 5;

        //            cell = InsertCellInWorksheet("K", 2, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 5;

        //            cell = InsertCellInWorksheet("K", 3, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 1;

        //            cell = InsertCellInWorksheet("K", 4, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 1;

        //            string TO = "To :";
        //            index = InsertSharedStringItem(TO, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 3, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string CompName = ds.Tables[0].Rows[0]["CompanyName"].ToString();
        //            index = InsertSharedStringItem(CompName, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 3, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("D", 3, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("E", 3, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("F", 3, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string Phone = "Phone :";
        //            index = InsertSharedStringItem(Phone, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 4, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string PhneNum = ds.Tables[0].Rows[0]["PhneNo"].ToString();
        //            index = InsertSharedStringItem(PhneNum, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 4, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("D", 4, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("E", 4, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("F", 4, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string Subject = "Subject :";// +ds.Tables[0].Rows[0]["PhneNo"].ToString();
        //            index = InsertSharedStringItem(Subject, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 5, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string Subject1 = ds.Tables[0].Rows[0]["Subject"].ToString();
        //            index = InsertSharedStringItem(Subject1, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 5, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("D", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("E", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;


        //            cell = InsertCellInWorksheet("F", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;


        //            cell = InsertCellInWorksheet("G", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("H", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("I", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("J", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("K", 5, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;


        //            string RefnoDt = "Ref. No. :";
        //            index = InsertSharedStringItem(RefnoDt, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 6, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("B", 7, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string EnqNum = ds.Tables[0].Rows[0]["EnquireNumber"].ToString() + "-" + Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        //            index = InsertSharedStringItem(EnqNum, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 6, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("D", 6, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("E", 6, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("F", 6, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            string PrjDept = "Project/" + System.Environment.NewLine + "Department :";
        //            index = InsertSharedStringItem(PrjDept, shareStringPart);
        //            cell = InsertCellInWorksheet("G", 6, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 5;

        //            cell = InsertCellInWorksheet("G", 7, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;


        //            string DeptNam = ds.Tables[0].Rows[0]["DeptName"].ToString();
        //            index = InsertSharedStringItem(DeptNam, shareStringPart);
        //            cell = InsertCellInWorksheet("I", 6, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("I", 7, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 3;

        //            cell = InsertCellInWorksheet("J", 6, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("K", 6, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("K", 7, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;


        //            string RecDate = "Rec. Date :";
        //            index = InsertSharedStringItem(RecDate, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 8, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string RecDat = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
        //            index = InsertSharedStringItem(RecDat, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 8, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("D", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("E", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("F", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string DueDt = "Due Date :";
        //            index = InsertSharedStringItem(DueDt, shareStringPart);
        //            cell = InsertCellInWorksheet("G", 8, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string DateDue = Convert.ToDateTime(ds.Tables[0].Rows[0]["DueDate"]).ToString("dd/MM/yyyy");
        //            index = InsertSharedStringItem(DateDue, shareStringPart);
        //            cell = InsertCellInWorksheet("H", 8, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("I", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("J", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("K", 8, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string PurchseEnq = " PURCHASE ENQUIRY ";
        //            index = InsertSharedStringItem(PurchseEnq, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 9, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 4;

        //            string Note = " Please quote for the following TOOLS FOR ELECTRICAL AND MECHANICAL, at the earliest possible ";
        //            index = InsertSharedStringItem(Note, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 10, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);


        //            string Slno = "Sl. No.";
        //            index = InsertSharedStringItem(Slno, shareStringPart);
        //            cell = InsertCellInWorksheet("B", 11, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string ItmDes = " Item Description ";
        //            index = InsertSharedStringItem(ItmDes, shareStringPart);
        //            cell = InsertCellInWorksheet("C", 11, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("D", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("E", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("F", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("G", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("H", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("I", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            string Qant = " Quantity";
        //            index = InsertSharedStringItem(Qant, shareStringPart);
        //            cell = InsertCellInWorksheet("J", 11, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            cell = InsertCellInWorksheet("K", 11, worksheetPart);
        //            cell.CellValue = new CellValue("");
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            cell.StyleIndex = 9;

        //            Items = CRPTBLL.GetFenqAllDetails_Items(new Guid(Id));

        //            j = 12;
        //            for (int i = 0; i < Items.Tables[0].Rows.Count; i++)
        //            {
        //                int SrNo = Convert.ToInt16(Items.Tables[0].Rows[i]["SrNo"].ToString());
        //                index = InsertSharedStringItem(SrNo.ToString(), shareStringPart);
        //                cell = InsertCellInWorksheet("B", j, worksheetPart);
        //                cell.CellValue = new CellValue(index.ToString());
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                string Descrip = Items.Tables[0].Rows[i]["ItemDescription"] + ";"
        //                    + (Items.Tables[0].Rows[i]["Specifications"].ToString() != "" ? Items.Tables[0].Rows[i]["Specifications"] + ";" : "")
        //                    + (Items.Tables[0].Rows[i]["Make"].ToString() != "" ? Items.Tables[0].Rows[i]["Make"] + ";" : "")
        //                    + (Items.Tables[0].Rows[i]["PartNumber"].ToString() != "" ? Items.Tables[0].Rows[i]["PartNumber"] : "");

        //                index = InsertSharedStringItem(Descrip, shareStringPart);
        //                cell = InsertCellInWorksheet("C", j, worksheetPart);
        //                cell.CellValue = new CellValue(index.ToString());
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("D", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("E", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("F", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("G", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("H", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("I", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                string Units = Items.Tables[0].Rows[i]["UOM"].ToString();
        //                index = InsertSharedStringItem(Units, shareStringPart);
        //                cell = InsertCellInWorksheet("J", j, worksheetPart);
        //                cell.CellValue = new CellValue(index.ToString());
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                cell = InsertCellInWorksheet("K", j, worksheetPart);
        //                cell.CellValue = new CellValue("");
        //                cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //                cell.StyleIndex = 9;

        //                j++;
        //            }
        //            //j = 58; //Convert.ToUInt32(Items.Tables[0].Rows.Count.ToString());
        //            //j = j + Convert.ToUInt32(Items.Tables[0].Rows.Count) - 1;

        //            //string Imp = "Important Instructions:";
        //            //index = InsertSharedStringItem(Imp, shareStringPart);
        //            //cell = InsertCellInWorksheet("B", j, worksheetPart);
        //            //cell.CellValue = new CellValue(index.ToString());
        //            //cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            j++;
        //            string Best = "Best Regards,";
        //            index = InsertSharedStringItem(Best, shareStringPart);
        //            cell = InsertCellInWorksheet("B", j, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            //bool isBold = cellData.Style.Font.IsBold;
        //            j++;
        //            string For = " For " + ds.Tables[0].Rows[0]["ORG"].ToString();
        //            index = InsertSharedStringItem(For, shareStringPart);
        //            cell = InsertCellInWorksheet("B", j, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            j++;
        //            // j = j + 2;
        //            string PersonName = ds.Tables[0].Rows[0]["CustContactPersopn"].ToString();
        //            index = InsertSharedStringItem(PersonName, shareStringPart);
        //            cell = InsertCellInWorksheet("B", j, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        //            j++;
        //            //j++;
        //            string Com = "Commercial Manager";
        //            index = InsertSharedStringItem(Com, shareStringPart);
        //            cell = InsertCellInWorksheet("B", j, worksheetPart);
        //            cell.CellValue = new CellValue(index.ToString());
        //            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        //            //Save the new worksheet.
        //            worksheetPart.Worksheet.Save();
        //        }

        //        string sheetName = "Sheet1";
        //        string cell1Name = "B2";
        //        string cell2Name = "F2";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "H1"; cell2Name = "K1";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "B9"; cell2Name = "K9";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "B10"; cell2Name = "K10";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "G2"; cell2Name = "G5";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "H2"; cell2Name = "K5";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "B2"; cell2Name = "F2";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "C3"; cell2Name = "F3";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "C4"; cell2Name = "F4";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "C5"; cell2Name = "F5";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "B6"; cell2Name = "B7";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);


        //        cell1Name = "C6"; cell2Name = "F7";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "G6"; cell2Name = "H7";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "I6"; cell2Name = "K7";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "C8"; cell2Name = "F8";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "H8"; cell2Name = "K8";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "C11"; cell2Name = "I11";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        cell1Name = "J11"; cell2Name = "K11";
        //        MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //        j = 12;
        //        for (int i = 0; i < Items.Tables[0].Rows.Count; i++)
        //        {
        //            cell1Name = "C" + j; cell2Name = "I" + j;
        //            MergeTwoCells(docName, sheetName, cell1Name, cell2Name);

        //            cell1Name = "J" + j; cell2Name = "K" + j;
        //            MergeTwoCells(docName, sheetName, cell1Name, cell2Name);
        //            j++;

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
        //    }
        //}


        //private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        //{
        //    try
        //    {
        //        // If the part does not contain a SharedStringTable, create one.
        //        if (shareStringPart.SharedStringTable == null)
        //        {
        //            shareStringPart.SharedStringTable = new SharedStringTable();
        //        }

        //        int i = 0;

        //        // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
        //        foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
        //        {
        //            if (item.InnerText == text)
        //            {
        //                return i;
        //            }

        //            i++;
        //        }
        //        // The text does not exist in the part. Create the SharedStringItem and return its index.
        //        shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
        //        shareStringPart.SharedStringTable.Save();

        //        return i;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
        //        return '0';
        //    }
        //}

        //// Given a WorkbookPart, inserts a new worksheet.
        //private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        //{
        //    // Add a new worksheet part to the workbook.
        //    WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //    newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new SheetData());
        //    newWorksheetPart.Worksheet.Save();

        //    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
        //    string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

        //    // Get a unique ID for the new sheet.
        //    uint sheetId = 1;
        //    if (sheets.Elements<Sheet>().Count() > 0)
        //    {
        //        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        //    }

        //    string sheetName = "Sheet" + sheetId;

        //    // Append the new worksheet and associate it with the workbook.
        //    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
        //    sheets.Append(sheet);

        //    // sheets.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //    workbookPart.Workbook.Save();

        //    return newWorksheetPart;
        //}

        //// Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        //// If the cell already exists, returns it. 
        //private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        //{
        //    DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet = worksheetPart.Worksheet;
        //    SheetData sheetData = worksheet.GetFirstChild<SheetData>();
        //    string cellReference = columnName + rowIndex;


        //    // If the worksheet does not contain a row with the specified row index, insert one.
        //    Row row;
        //    if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
        //    {
        //        row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        //    }
        //    else
        //    {
        //        row = new Row() { RowIndex = rowIndex };
        //        row.Height = 250.25;
        //        sheetData.Append(row);
        //    }

        //    // If there is not a cell with the specified column name, insert one.  
        //    if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
        //    {
        //        return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
        //    }
        //    else
        //    {
        //        // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
        //        Cell refCell = null;
        //        foreach (Cell cell in row.Elements<Cell>())
        //        {
        //            if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
        //            {
        //                refCell = cell;
        //                break;
        //            }
        //        }

        //        Cell newCell = new Cell() { CellReference = cellReference };

        //        row.InsertBefore(newCell, refCell);


        //        worksheet.Save();
        //        return newCell;
        //    }
        //}

        //public void CreateSpreadsheetWorkbook(string filepath)
        //{
        //    try
        //    {
        //        // Create a spreadsheet document by supplying the filepath.
        //        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        //        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
        //            Create(filepath, SpreadsheetDocumentType.Workbook);


        //        // Add a WorkbookPart to the document.
        //        WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        //        workbookpart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

        //        // Add a WorksheetPart to the WorkbookPart.
        //        WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        //        worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new SheetData());


        //        // Add Sheets to the Workbook.
        //        DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
        //            AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

        //        workbookpart.Workbook.Save();
        //        // Close the document.
        //        spreadsheetDocument.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
        //    }
        //}

        #endregion

        #endregion

    }
}
