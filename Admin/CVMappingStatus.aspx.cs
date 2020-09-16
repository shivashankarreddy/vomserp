using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using Ajax;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.SqlClient;

namespace VOMS_ERP.Admin
{
    public partial class CVMappingStatus : System.Web.UI.Page
    {
        MappingBLL MBLL = new MappingBLL();
        ErrorLog ELog = new ErrorLog();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
            if (Session["UserName"] == null)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                //if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                if(Session["UserName"].ToString() == CommonBLL.SuperAdminRole)
                {
                    Ajax.Utility.RegisterTypeForAjax(typeof(CVMappingStatus));
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }
        #region Web Methods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteMappingDetails(string ID)
        {
            try
            {
                DataSet dset = new DataSet();
                int res = -999;
                string result = "";
                dset = MBLL.SelectCVMapping(CommonBLL.FlagFSelect,new Guid(ID));
                if (dset != null && dset.Tables[0].Rows.Count == 0)
                {
                    res = MBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Guid.Empty, new Guid(ID), Guid.Empty, Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now,"");
                }
                else
                    res = -123;

                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                    result = "Error::Cannot Delete this Record, this is used by another transaction/ Error while Deleting ";

        #endregion
                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        //[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        //public string EditMappingDetails(string ID)
        //{
        //    try
        //    {
        //        return CommonBLL.Can_EditDelete(true, CreatedBy);
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Admin/ErrorLog"), "Mapping Status", ex.Message.ToString());
        //        return ErrMsg;
        //    }
        //}


    }
}