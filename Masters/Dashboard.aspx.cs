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
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.Text;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;
using BAL;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Web.Hosting;

namespace VOMS_ERP.Masters
{
    public partial class Dashboard : System.Web.UI.Page
    {
        # region variables
        int res;
        DeshBoardBLL DBBL = new DeshBoardBLL();
        ErrorLog ELog = new ErrorLog();
        static ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Ajax.Utility.RegisterTypeForAjax(typeof(Dashboard));
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("~/Login.aspx?logout=yes", false);
                else
                {
                    //Page.Title = "";
                    if (Request.QueryString["NP"] != null)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('You are not having permission, please contact your Administrator...');", true);
                    else if (Session["NoPermission"] != null && Session["NoPermission"].ToString() == "YES")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "javascript:alert('You are not having permission, please contact your Administrator...');", true);
                        Session["NoPermission"] = null;
                    }

                    if (!IsPostBack)
                    {
                        if (Session["UserMail"].ToString() == "msrvprasad@gmail.com")
                            Response.Redirect("~/Enquiries/FeStatusOverView.aspx", false);
                        else if (Session["AccessRole"] != null && Session["AccessRole"].ToString() == "Customer")
                        {
                            Response.Redirect("~/Masters/Home.aspx", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("Logs/Masters/ErrorLog"), "Home Page", ex.Message.ToString());
            }
        }


        /// <summary>
        /// To Retrieve Data
        /// </summary>
        [WebMethod(EnableSession = true)]
        public static string GetChartData()
        {
            try
            {
                List<DashboardDetails> Main_Bar = new List<DashboardDetails>(); List<DashboardDetails> Main_2 = new List<DashboardDetails>(); List<DashboardDetails> Main_3 = new List<DashboardDetails>();
                List<DashboardDetails> Main_Bar_Child = new List<DashboardDetails>(); dynamic All_Copy = null; dynamic All_Copy_Trust = null; dynamic Yearly_Barchart = null;
                var Monthly_DD = (List<DashboardDetails>)null; dynamic Yearly_Barchart_Drilldown = null;
                dynamic Child_bar; string Res = string.Empty;
                DataSet ds = new DataSet();
                Guid CompanyID = HttpContext.Current.Session["CompanyID"].ToString() != null ? new Guid(HttpContext.Current.Session["CompanyID"].ToString()) : Guid.Empty;
                ds = IDBLL.GetDashboardDetails((DateTime.Now.Date), (DateTime.Now.AddDays(-30).Date), CompanyID);
                int z = 1;
                foreach (DataTable get in ds.Tables)
                {
                    z = 1;
                    if (get != null && get.Rows.Count > 0)
                    {

                        //int relationshipCount = get.AsEnumerable().Select(r => r.Field<string>("TransID")).Distinct().Count();
                        Main_Bar.Add(new DashboardDetails { name = (get.Rows[0]["Transactions"].ToString()), y = get.AsEnumerable().Select(r => r.Field<string>("TransID")).Distinct().Count(), drilldown = get.Rows[0]["Transactions"].ToString() + "_P" });
                        //GroupBy(gb => new {TransID = gb.mm.Field<string>("TransID").ToString() })
                        IEnumerable<DataRow> sequence = get.AsEnumerable();

                        Main_2 = (from mm in get.AsEnumerable()
                                  group new
                                  {
                                      mm
                                  }
                                  by new
                                  {
                                      name = mm.Field<string>("Name").ToString(),
                                      id = mm.Field<string>("Transactions").ToString()
                                  } into tempmm
                                  select new DashboardDetails
                                  {
                                      id = tempmm.Key.id + "_P",
                                      name = tempmm.Key.name + "_" + tempmm.Key.id,
                                      //data = tempmm.GroupBy(gb => new { name = gb.mm.Field<string>("Name").ToString(), no = gb.mm.Field<string>("NO").ToString(), id = gb.mm.Field<string>("Transactions").ToString() })
                                      //             .Where(mv => mv.Key.name == tempmm.Key.name)
                                      //             .Select(cc => new
                                      //             {
                                      //                 name = string.Format(cc.Key.no, cc.Key.id, cc.Key.name),
                                      //                 y = 1,
                                      //                 drilldown = ""
                                      //             }),
                                      y = tempmm.GroupBy(gb => new { name = gb.mm.Field<string>("Name").ToString(), no = gb.mm.Field<string>("NO").ToString(), id = gb.mm.Field<string>("Transactions").ToString() }).Count(),
                                      drilldown = tempmm.Key.id + "_P" + "_" + z++
                                  }).ToList();
                        if (get.Rows[0]["Transactions"].ToString() == "LPO" || get.Rows[0]["Transactions"].ToString() == "BPC" || get.Rows[0]["Transactions"].ToString() == "BPA")
                        {
                            All_Copy = new DashboardDetails
                            {
                                id = Main_2.Select(l => l.id).FirstOrDefault(),
                                data = Main_2,
                                name = "SUPPLIER WISE TRANSACTIONS"
                                //y = Main_2.Count()
                            };
                        }
                        else
                        {
                            All_Copy = new DashboardDetails
                            {
                                id = Main_2.Select(l => l.id).FirstOrDefault(),
                                data = Main_2,
                                name = "CUSTOMER WISE TRANSACTIONS"
                                //y = Main_2.Count()
                            };
                        }
                        Main_Bar_Child.Add(All_Copy);
                        z = 1;

                        Main_3 = (from mm in get.AsEnumerable()
                                  group new
                                  {
                                      mm
                                  }
                                  by new
                                  {
                                      name = mm.Field<string>("Name").ToString(),
                                      id = mm.Field<string>("Transactions").ToString()
                                  } into tempmm
                                  select new DashboardDetails
                                  {
                                      id = tempmm.Key.id + "_P" + "_" + z++,
                                      name = tempmm.Key.id + "_" + tempmm.Key.name,
                                      //Link = GetLink(tempmm.Key.id, tempmm.Key.name),
                                      data = tempmm.GroupBy(gb => new { name = gb.mm.Field<string>("Name").ToString(), no = gb.mm.Field<string>("NO").ToString(), id = gb.mm.Field<string>("Transactions").ToString(), TransID = gb.mm.Field<string>("TransID").ToString() })
                                                   .Where(mv => mv.Key.name == tempmm.Key.name)
                                                   .Select(cc => new
                                                   {
                                                       name = string.Format(cc.Key.no, cc.Key.id, cc.Key.name),
                                                       y = 1,
                                                       Link = GetLink(cc.Key.id, cc.Key.TransID),
                                                       drilldown = ""
                                                   }),
                                      y = tempmm.Count(),
                                      //drilldown = tempmm.Key.id + "_P" + "_" + tempmm.Key.i
                                  }).ToList();
                        All_Copy_Trust = new DashboardDetails
                        {
                            id = Main_3.Select(l => l.id).FirstOrDefault(),
                            data = Main_3,
                            name = "TRANSACTIONS",
                            y = Main_3.Count()
                        };

                        Main_Bar_Child.AddRange(Main_3);

                    }
                }

                Yearly_Barchart = new DashboardDetails[] { 
                        new DashboardDetails{
                        name = "TRANSACTIONS", 
                        data = Main_Bar
                        } };


                Yearly_Barchart_Drilldown = Main_Bar_Child;

                var DrillDown_Data = Yearly_Barchart_Drilldown;
                Res = JsonConvert.SerializeObject(new
                {
                    DrillDown_Data,
                    Yearly_Barchart
                });
                return Res.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        public static string GetLink(string Transaction, string ID)
        {
            try
            {
                string linkDesc = string.Empty;
                if (Transaction == "FE")
                {
                    linkDesc = "../Enquiries/FEDetails.aspx?FEnqID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "FQ")
                {
                    linkDesc = "../Quotations/FQDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "FPO")
                {
                    linkDesc = "../Purchases/FPODetails.Aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "LPO")
                {
                    linkDesc = "../Purchases/LPODetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "GDN")
                {
                    linkDesc = "../Logistics/GdnDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "GRN")
                {
                    linkDesc = "../Logistics/GrnDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "PI")
                {
                    linkDesc = "../Invoices/PrfmaInvoiceDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "PCK-List")
                {
                    linkDesc = "../Invoices/PackingListDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "CI")
                {
                    linkDesc = "../Invoices/CommercialInvoiceDetails.aspx?PINID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "BP-Raised")
                {
                    linkDesc = "../Accounts/BillpaymentApprovalDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                else if (Transaction == "BPA")
                {
                    linkDesc = "../Accounts/BillpaymentApprovalDetails.aspx?ID=" + ID + "&IsDash=1";
                }
                return linkDesc;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        #region Classes

        public class DashboardDetails
        {
            public string id { get; set; }
            public string name { get; set; }
            public decimal? y { get; set; }
            public object data { get; set; }
            public string drilldown { get; set; }
            public string code { get; set; }
            public string Link { get; set; }
            public Guid LID { get; set; }
        }

        #endregion
    }
}