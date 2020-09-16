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
using BAL;
using System.Xml.Linq;
using System.Linq;
using ClosedXML.Excel;
using Ionic.Zip;

namespace VOMS_ERP.Enquiries
{
    public partial class TrackerExportReport : System.Web.UI.Page
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
        DataSet _DS = new DataSet();
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "EnableButtons();", true);
            }
        }

        protected void btnFeTracker_Click(object sender, EventArgs e)
        {
            try
            {
                _DS = CommonBLL.GetFETrackerData(CommonBLL.FlagSelectAll, "", "", "", Guid.Empty);
                GetData(_DS, "FE Received Date");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

        protected void btnFPOTracker_Click(object sender, EventArgs e)
        {
            try
            {
                _DS = CommonBLL.GetFETrackerData(CommonBLL.FlagGSelect, "", "", "", Guid.Empty);
                GetData(_DS, "FPO Recivied Date");

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

        protected void btnFPOExportTracker_Click(object sender, EventArgs e)
        {
            try
            {
                _DS = CommonBLL.GetFPOExportDeliveryTrackerData(CommonBLL.FlagSelectAll, "", "", "", Guid.Empty);
                GetData(_DS, "FPO Received Date");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

        public void GetData(DataSet _DST, string sort)
        {
            try
            {
                DataTable _DT = new DataTable();

                //_DS = CommonBLL.GetFETrackerData(a, b, c, d, e);

                var Result = _DST.Tables[0].AsEnumerable()
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
                        var SDT = DT.AsEnumerable().OrderByDescending(P => P.Field<DateTime>(sort)).CopyToDataTable();
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
                    if (sort.Contains("FPO"))
                        Response.AddHeader("Content-Disposition", "attachment; filename=FPOTracker_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".zip");
                    else
                        Response.AddHeader("Content-Disposition", "attachment; filename=FETracker_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".zip");
                    Response.ContentType = "application/zip";
                    
                    _Zip.Save(Response.OutputStream);
                    try { Response.End(); }
                    catch { }
                    finally
                    {
                        Array.ForEach(Directory.GetFiles(Server.MapPath("/UPLOADS/TrackerExcels/"), "*.xlsx"), File.Delete);
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "EnableButtons()", true);
                        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "EnableButtons();", true);
                    }
                }
                //btnFPOExportTracker.Enabled = true;
                //btnFPOTracker.Enabled = true;
                //btnFeTracker.Enabled = true;
                //btnFeTracker.Attributes.Add("ReadOnly", "false");
                //btnFPOTracker.Attributes.Add("ReadOnly", "false");
                //btnFPOExportTracker.Attributes.Add("ReadOnly", "false");
                //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "EnableButtons();", true);
            }
            catch (Exception ex)
            {
                //btnFeTracker.Attributes.Add("ReadOnly", "false");
                //btnFPOTracker.Attributes.Add("ReadOnly", "false");
                //btnFPOExportTracker.Attributes.Add("ReadOnly", "false");
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "EnableButtons()", true);
                //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('sssss');", false);
                //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "EnableButtons();", false);
                //System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, typeof(System.Web.UI.Page), "Script", "EnableButtons();", true);
                //ClientScript.RegisterStartupScript(GetType(), "hwa", "EnableButtons();", true);
                //Response.Redirect(@"\Enquiries\TrackerExportReport.aspx", false);
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Tracker", ex.Message.ToString());
            }
        }

    }
}