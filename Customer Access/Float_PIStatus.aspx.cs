﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ajax;
using BAL;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Collections;

namespace VOMS_ERP.Customer_Access
{
    public partial class Float_PIStatus : System.Web.UI.Page
    {

        # region variables
        int res;
        ErrorLog ELog = new ErrorLog();
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();

        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Float_PIStatus));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    string date = HttpContext.Current.Request.Params["sSearch_0"];
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {

                        }

                        //.ImageUrl = "Admin/ShowImg.ashx?id=" + new Guid(Session["CompanyID"].ToString());
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Enquiry Status", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click
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
                string vend = HFVen.Value;
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

                DataSet ds = FEBL.Select_FEFloatedSearch(FrmDt, ToDat, FENo, FromRcvdDt, ToRcvdDt, Subject, Status, dept, vend, Cust, CnctPrsn, CreatedDT, LoginID, CusID, new Guid(Session["CompanyID"].ToString()));

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
                            string FilePath = Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
                            //Server.MapPath("../images/Logos/" + Session["CompanyID"].ToString() + ".png");
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

            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Enquiry Status", ex.Message.ToString());
            }

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
                    //System.Data.DataTable DtSubItems = CommonBLL.EmptyDt();
                    NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                    //if (HttpContext.Current.Session["AllFESubItems"].ToString() != "")
                    //    DtSubItems = (System.Data.DataTable)HttpContext.Current.Session["AllFESubItems"];
                    //else
                    //DtSubItems = CommonBLL.FEEmpty_SubItems();
                    res = NEBLL.NewVendorEnquiryDelete(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, "", "",
                        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", "", 0, "", "", new Guid(Session["UserID"].ToString()),
                        new Guid(Session["UserID"].ToString()), false, new Guid(Session["CompanyID"].ToString()), dt);
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Enquiry Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), " Floated Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }

        }
        #endregion
    }
}