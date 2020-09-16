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
    /// <summary>
    ///This Class is written for ExpShipDtlsStatus page to get requried data and logic to attain its functionality as well to edit or delete a              particular record
    /// </summary>
    public partial class ExpShipDtlsStatus : System.Web.UI.Page
    {
        #region Variables
        int res = 999;//variable to store response
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        ExportShipmentDetailsBLL ExBLL = new ExportShipmentDetailsBLL();
        //AuditLogs ALS = new AuditLogs();
        #endregion
        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)//methods and logic to execute on page load
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ExpShipDtlsStatus));
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status", ex.Message.ToString());
            }
        }
        #endregion

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string CommercialInvID)//function to delete a given record
        {
            try
            {
                int res = 1;
                string result = CommonBLL.Can_EditDelete(false, CreatedBy);//function which checks if the given user has the necessary permissions for deleting 

                if (result == "Success")
                {
                    DataSet EditDS = ExBLL.GetCommInvData(Convert.ToChar(CommonBLL.FlagASelect));//function to get all the records in ExportShipmentDtls Table
                    string Stat = "";
                    if (EditDS != null && EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        //    Stat = EditDS.Tables[0].Rows[0]["StatusID"].ToString();
                        //if (Stat == "" || Convert.ToInt32(Stat) >= 87) //["StatusID"]
                        res = -123;
                    else
                    {
                        res = ExBLL.InsertUpdateDeleteExpShipDlts(CommonBLL.FlagDelete, new Guid(CommercialInvID), "", DateTime.Now, "", "", "", "", DateTime.Now, "", 0, 0, 0, "", "", 1, 0, 0, "", DateTime.Now, DateTime.Now, "", DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", DateTime.Now, "", "", DateTime.Now
, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", DateTime.Now, "", "", "", "", DateTime.Now, "",
                            new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, new Guid(), DateTime.Now, true);//Function to delete a record based on its CommercialInvID
                    }
                    if (res == 0)
                    {
                        result = "Success";
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status",
                       "Row Deleted successfully.");
                    }
                    else
                    {
                        result = "Error";
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status",
                       "Error while Deleting " + CommercialInvID + ".");
                    }
                }

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status", ex.Message.ToString());
                return ErrMsg;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Export Shipment Details Status", ex.Message.ToString());
                return ErrMsg;
            }
        }




    }
}