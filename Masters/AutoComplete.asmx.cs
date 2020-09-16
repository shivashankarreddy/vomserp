using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using BAL;


namespace VOMS_ERP.Masters
{
    /// <summary>
    /// Summary description for AutoComplete
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class AutoComplete : System.Web.Services.WebService
    {
        public AutoComplete()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetCustomers(string prefix)
        {
            List<string> customers = new List<string>();
            DataSet ds = new DataSet();
            SentMailsBLL SMBLL = new SentMailsBLL();
            ds = SMBLL.GetSubOrMailIDs(CommonBLL.FlagBSelect, prefix);
            if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    customers.Add(ds.Tables[0].Rows[i][0].ToString());
            }
            return customers.ToArray();
            # region NotInUse
            //using (SqlConnection conn = new SqlConnection())
            //{
            //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.CommandText = "select subject from SentMails where " +
            //        "subject like @SearchText + '%'";
            //        cmd.Parameters.AddWithValue("@SearchText", prefix);
            //        cmd.Connection = conn;
            //        conn.Open();
            //        using (SqlDataReader sdr = cmd.ExecuteReader())
            //        {
            //            while (sdr.Read())
            //            {
            //                customers.Add(string.Format("{0}", sdr["subject"]));
            //            }
            //        }
            //        conn.Close();
            //    }
            //    return customers.ToArray();
            //}
            # endregion
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetCustomersNm(string prefix)
        {
            List<string> customers = new List<string>();
            DataSet ds = new DataSet();
            //SelectCustomers
            CustomerBLL CSTBLL = new CustomerBLL();
            ds = CSTBLL.SearchCustomers(CommonBLL.FlagESelect, prefix,new Guid(Session["CompanyId"].ToString()));
            if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    customers.Add(string.Format("{0}-{1}", ds.Tables[0].Rows[i][0].ToString(), ds.Tables[0].Rows[i][1].ToString()));
            }
            return customers.ToArray();
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int CheckCustomerExistance(string prefix, string Type)
        {
            # region Checking Customer are Exist are not
            using (SqlConnection conn = new SqlConnection())
            {
                List<string> customers = new List<string>();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    if (Type == "OrgName")
                    {
                        cmd.CommandText = "SELECT Customer.OrgName as Rslt FROM Customer WHERE Customer.OrgName = @SearchText and Customer.CompanyId = '" +
                            HttpContext.Current.Session["CompanyID"].ToString() + "' and IsActive <> 0 ";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT Customer.BussName as Rslt FROM Customer WHERE Customer.BussName = @SearchText and Customer.CompanyId = '" +
                            HttpContext.Current.Session["CompanyID"].ToString() + "' and IsActive <> 0 ";
                    }

                    cmd.Parameters.AddWithValue("@SearchText", prefix);
                    cmd.Connection = conn;
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            customers.Add(string.Format("{0}", sdr["Rslt"]));
                        }
                    }
                    conn.Close();
                }
                return customers.ToArray().Count();
            }
            # endregion
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetSupplierNm(string prefix)
        {
            List<string> Supplier = new List<string>();
            DataSet ds = new DataSet();
            //SelectCustomers
            SupplierBLL SPLRBLL = new SupplierBLL();
            ds = SPLRBLL.SelectSuppliers(CommonBLL.FlagESelect, prefix);
            if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    Supplier.Add(string.Format("{0}-{1}", ds.Tables[0].Rows[i][0].ToString(), ds.Tables[0].Rows[i][1].ToString()));
            }
            return Supplier.ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetPriceBasis(string Parent)
        {
            try
            {
                Guid Prnt = new Guid(Parent);
                List<string> Supplier = new List<string>();
                EnumMasterBLL EMBL = new EnumMasterBLL();
                DataSet ds = new DataSet();
                if (Prnt != Guid.Empty)
                {
                    DataTable dtbl = new DataTable();
                    dtbl.Columns.Add("ID");
                    dtbl.Columns.Add("Description");
                    ds.Tables.Add(dtbl);
                }
                else
                    ds = EMBL.EnumMasterSelect(CommonBLL.FlagCommonMstr, Guid.Empty, Guid.Empty, Prnt, new Guid(Session["CompanyID"].ToString()));
                //
                DataRow _Dflt = ds.Tables[0].NewRow(); //.NewDataRow();
                _Dflt["ID"] = "0";
                _Dflt["Description"] = "Select";
                ds.Tables[0].Rows.InsertAt(_Dflt, 0);
                ds.Tables[0].AcceptChanges();
                if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        Supplier.Add(string.Format("{0}-{1}", ds.Tables[0].Rows[i][0].ToString(), ds.Tables[0].Rows[i][1].ToString()));
                }
                return Supplier.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public String CheckUniqueness(String Value, String FieldName, String TableName)
        //{

        //    return "";
        //}
    }
}
