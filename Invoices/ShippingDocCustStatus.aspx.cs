using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;
using BAL;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Data;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Invoices
{
    public partial class ShippingDocCustStatus : System.Web.UI.Page
    {
        #region Variables
        int res = 999;//variable to store response
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        ShipDocCustBLL SDCBLL = new ShipDocCustBLL();
        #endregion

        #region Default Page Load Event
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ShippingDocCustStatus));
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {
                            // GetData();
                            //txtFrmDt.Attributes.Add("readonly", "readonly");
                            // txtToDt.Attributes.Add("readonly", "readonly");
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Document Customer Status", ex.Message.ToString());
            }
        }
        #endregion
        #region Web Methods
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string SDCID)//function to delete a given record
        {
            try
            {
                int res = 1;
                string result = CommonBLL.Can_EditDelete(false, CreatedBy);//function which checks if the given user has the necessary permissions for deleting 

                if (result == "Success")
                {
                    DataSet EditDS = SDCBLL.GetAirBOLNO(Convert.ToChar(CommonBLL.FlagCSelect), new Guid(Session["CompanyID"].ToString()), Guid.Empty);//function to get all the records in ExportShipmentDtls Table
                    string Stat = "";
                    //if (EditDS != null && EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                    //    Stat = EditDS.Tables[0].Rows[0]["StatusID"].ToString();
                    //if (Stat == "" || Convert.ToInt32(Stat) >= 87) //["StatusID"]
                    //res = -123;
                    //else
                    //{

                    res = SDCBLL.InsertUpdateDeleteShipDocCust(CommonBLL.FlagDelete, new Guid(SDCID), "", "", "", true, true, "", "", DateTime.Now, "", "", "", "", Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, new Guid(Session["CompanyID"].ToString()));//Function to delete a record based on its CommercialInvID
                    //}
                    if (res == 0)
                    {
                        result = "Success";
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Document Customer Status",
                       "Row Deleted successfully.");
                    }
                    else
                    {
                        result = "Error";
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Document Customer Status",
                       "Error while Deleting " + SDCID + ".");
                    }
                }

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Document Customer Status", sx.Message.ToString());
                return ErrMsg;
            }

        }
        protected void btnExcelExpt_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {

                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt1 = "", ToDat1 = "", CreatedDT = "";
                Guid LoginID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());

                if (Mode == "odt")
                {
                    FrmDt1 = HFCourSfromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCourSfromDate.Value).ToString("yyyy-MM-dd");

                    if (HFCourStoDate.Value != "")
                    {
                        ToDat1 = HFCourStoDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCourStoDate.Value).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        ToDat1 = DateTime.Now.ToString("yyyy-MM-dd");
                    }

                }

                else if (Mode == "tdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    FrmDt1 = HFCourSfromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCourSfromDate.Value).ToString("yyyy-MM-dd");

                    ToDat1 = HFCourStoDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCourStoDate.Value).ToString("yyyy-MM-dd");

                }
                string AWBNo = HFAWBNo.Value;
                string FPOs = HFFPOs.Value;
                string CourName = HFCourName.Value;
                string CourNo = HFCourNo.Value;
                string DocSBy = HFDocSBy.Value;
                string RecPer = HFRecPer.Value;
                string DocCar = HFDocCar.Value;
                string ContNo = HFContNo.Value;
                string Unit = HFUnit.Value;
                string Created = HFCreated.Value;

                if (FrmDt1 == "1-1-0001" || FrmDt1 == "1-1-1900")
                    FrmDt1 = "";
                if (ToDat1 == "1-1-0001")
                    ToDat1 = "";

                DataSet ds = SDCBLL.ShipDocSearch(FrmDt1, ToDat1, AWBNo, FPOs, CourName, CourNo, DocSBy, RecPer, DocCar, ContNo, Unit, Created, CreatedDT, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=ShippingDocumentsCustomerstatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt1 != "" && Convert.ToDateTime(FrmDt1).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt1 = "";
                    if (ToDat1 != "" && (Convert.ToDateTime(ToDat1).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat1)) == CommonBLL.EndDtMMddyyyy_FS))
                        ToDat1 = "";

                    string MTitle = "STATUS OF  SHIPPING DOCUMENTS SENT BY COUSTMER", MTcustomer = "", MTDTS = "", MTENQNo = "";
                    //if (HFCustomer.Value != "")
                    //    MTcustomer = HFCustomer.Value;
                    //if (FrmDt != "" && ToDat != "")
                    //    MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    //else if (FrmDt != "" && ToDat == "")
                    //    MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    //else
                    //    MTDTS = " TILL " + DateTime.Now.ToString("dd/MM/yyyy");
                    //if (FENo != "")
                    //{
                    //    MTENQNo = HFFENo.Value;
                    //    MTitle = "STATUS OF FOREIGN ENQUIRY NO. " + MTENQNo + " DT. " + ds.Tables[0].Rows[0]["FE Date"].ToString() + (MTcustomer == "" ? "" : " RECEIVED FROM " + MTcustomer.ToUpper());
                    //}
                    //else
                    MTitle = MTitle + " " + (MTcustomer != "" ? " TO " + MTcustomer.ToUpper() : "") + "" + MTDTS;
                    htextw.Write("<center><b>" + MTitle + "</center></b>");

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
                            string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                            //Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");
                            image.Save(FilePath);
                        }

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
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)//function to edit a given record 
        {
            try
            {

                return CommonBLL.Can_EditDelete(true, CreatedBy);//function which checks if the given user has the necessary permissions for  editing
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Shipping Document Customer Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        #endregion
    }
}