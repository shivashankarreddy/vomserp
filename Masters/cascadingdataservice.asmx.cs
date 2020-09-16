using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using BAL;
using AjaxControlToolkit;
using System.Collections.Specialized;

namespace VOMS_ERP.Masters
{
    /// <summary>
    /// Summary description for cascadingdataservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class cascadingdataservice : System.Web.Services.WebService
    {
        ErrorLog ELog = new ErrorLog();
        //[WebMethod]
        //public string HelloWorld()
        //{
        //    return "Hello World";
        //}
        EnumMasterBLL embal = new EnumMasterBLL();
        [WebMethod(EnableSession = true)]
        public AjaxControlToolkit.CascadingDropDownNameValue[] BindCountrydropdown(string knownCategoryValues, string category)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Countries);
                List<AjaxControlToolkit.CascadingDropDownNameValue> CountryDetails = new List<AjaxControlToolkit.CascadingDropDownNameValue>();

                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    string categoryID = dRow["ID"].ToString();
                    string categoryName = dRow["Description"].ToString();
                    CountryDetails.Add(new AjaxControlToolkit.CascadingDropDownNameValue(categoryName, categoryID));
                }
                return CountryDetails.ToArray();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
                return null;
                
            }
        }

        [WebMethod(EnableSession = true)]
        public CascadingDropDownNameValue[] BindStatedropdown(string knownCategoryValues, string category)
        {
            
            try
            {
                Guid CountryID;
                StringDictionary countrydetails = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
                if (countrydetails["Country"].ToString() != Guid.Empty.ToString())
                {
                    CountryID = new Guid(countrydetails["Country"].ToString());
                    DataSet ds = new DataSet();
                    ds = embal.EnumMasterSelectforDescription(BAL.CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, CountryID, new Guid(Session["CompanyID"].ToString()), CommonBLL.State);
                    List<CascadingDropDownNameValue> statedetails = new List<CascadingDropDownNameValue>();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dtstaterow in ds.Tables[0].Rows)
                        {
                            string stateID = dtstaterow["ID"].ToString();
                            string statename = dtstaterow["Description"].ToString();
                            statedetails.Add(new CascadingDropDownNameValue(statename, stateID));
                        }
                        return statedetails.ToArray();
                    }
                    return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Supplier Master", ex.Message.ToString());
                return null;
               
            }
        }
        [WebMethod(EnableSession = true)]
        public CascadingDropDownNameValue[] BindCityropdown(string knownCategoryValues, string category)
        {
            List<CascadingDropDownNameValue> regiondetails = new List<CascadingDropDownNameValue>();
            try
            {
                Guid stateID;
                StringDictionary statedetails = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
                if (statedetails["State"].ToString() != Guid.Empty.ToString())
                {
                    stateID = new Guid(statedetails["State"].ToString());
                    DataSet ds = new DataSet();
                    ds = embal.EnumMasterSelectforDescription(BAL.CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, stateID, new Guid(Session["CompanyID"].ToString()), CommonBLL.City);
                    foreach (DataRow dtregionrow in ds.Tables[0].Rows)
                    {
                        string regionID = dtregionrow["ID"].ToString();
                        string regionname = dtregionrow["Description"].ToString();
                        regiondetails.Add(new CascadingDropDownNameValue(regionname, regionID));

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
            return regiondetails.ToArray();
        }
    }
}
