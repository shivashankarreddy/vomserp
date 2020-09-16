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

    public partial class Home : System.Web.UI.Page
    {
        # region variables
        int res;
        DeshBoardBLL DBBL = new DeshBoardBLL();
        ErrorLog ELog = new ErrorLog();
        static ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        #endregion

        #region Default Page Load Event

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Home));
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
                        lblLoginName.Text = Session["UserName"].ToString();
                        //GetData();
                        if (Session["AccessRole"] != null && Session["AccessRole"].ToString() != "Customer")
                        {
                            Response.Redirect("~/Masters/Dashboard.aspx", false);
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

        #endregion

        # region Methods



        /// <summary>
        /// To Retrieve Data
        /// </summary>
        [WebMethod(EnableSession = true)]
        protected static string GetData()
        {
            try
            {
                List<DashboardDetails> Main_Bar = new List<DashboardDetails>(); List<DashboardDetails> Main = new List<DashboardDetails>();
                List<DashboardDetails> Main_Bar_Child = new List<DashboardDetails>(); dynamic All_Copy = null; dynamic All_Copy_Trust = null; dynamic Yearly_Barchart = null;
                var Monthly_DD = (List<DashboardDetails>)null; dynamic Yearly_Barchart_Drilldown = null;
                dynamic Child_bar;
                Guid CompanyID = HttpContext.Current.Session["CompanyID"].ToString() != null ? new Guid(HttpContext.Current.Session["CompanyID"].ToString()) : Guid.Empty;
                DataSet ds = new DataSet();
                ds = IDBLL.GetDashboardDetails((DateTime.Now.Date), (DateTime.Now.AddDays(-30).Date), CompanyID);

                foreach (DataTable get in ds.Tables)
                {
                    if (get != null && get.Rows.Count > 0)
                    {
                        Main_Bar.Add(new DashboardDetails { name = (get.Rows[0]["Transactions"].ToString() + "_" + DateTime.Now.Year), y = get.Rows.Count, drilldown = get.Rows[0]["Transactions"].ToString() });

                        IEnumerable<DataRow> sequence = get.AsEnumerable();

                        Main = (from mm in get.AsEnumerable()
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
                                    id = tempmm.Key.name,
                                    name = tempmm.Key.id,
                                    data = tempmm.GroupBy(gb => new { name = gb.mm.Field<string>("Name").ToString(), no = gb.mm.Field<string>("NO").ToString(), id = gb.mm.Field<string>("Transactions").ToString() })
                                                 .Where(mv => mv.Key.name == tempmm.Key.name)
                                                 .Select(cc => new
                                                 {
                                                     name = string.Format(cc.Key.no, cc.Key.id, cc.Key.name),
                                                     y = 1,
                                                     drilldown = tempmm.Key.name + " - " + cc.Key.id
                                                 })
                                }).ToList();

                        All_Copy = new DashboardDetails
                        {
                            id = Main.Select(l => l.id).FirstOrDefault(),
                            data = Main,
                            name = Main.Select(l => l.name).FirstOrDefault()
                        };

                        Main_Bar_Child.Add(All_Copy);

                    }
                }

                Yearly_Barchart = new DashboardDetails[] { 
                        new DashboardDetails{
                        name = "TRANSACTIONS", 
                        data = Main_Bar
                        } };


                Yearly_Barchart_Drilldown = Main_Bar_Child;

                var DrillDown_Data = Yearly_Barchart_Drilldown;
                return JsonConvert.SerializeObject(new
                {
                    DrillDown_Data,
                    Yearly_Barchart
                });
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearAll()
        {
            try
            {
                //btnSave.Text = "Save";
                //btnsavenew.Text = "Save & New";
                //btnSaveID = "Save";
                //ViewState["EditID"] = null;
                //ddlcustmr.SelectedIndex = ddldept.SelectedIndex = -1;
                //txtenqno.Text = txtsubject.Text = txtimpinst.Text = "";
                //txtenqdt.Text = txtrecvdt.Text = txtduedt.Text = "";
                //divListBox.InnerHtml = "";
                //txtenqdt.Text = txtrecvdt.Text = DateTime.Now.Date.ToShortDateString();
                //txtduedt.Text = DateTime.Now.AddDays(5).DayOfWeek != DayOfWeek.Sunday ? 
                //DateTime.Now.AddDays(5).Date.ToShortDateString() : DateTime.Now.AddDays(6).Date.ToShortDateString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Home Page", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Table Fiels (Dash Board Data)
        /// </summary>
        /// <param name="CommonTbl"></param>
        /// <param name="Type"></param>
        protected void BindTableFields(DataTable CommonTbl, string Type)
        {
            //if (CommonTbl.Rows.Count > 0)
            //{
            //    switch (Type)
            //    {
            //        case "FrnEnq": lblFrnEnqTd.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
            //            lblFQTd.Text = CommonTbl.Rows[0]["FrnQuote"].ToString();
            //            lblFPORcvdToday.Text = CommonTbl.Rows[0]["FrnPOrder"].ToString();

            //            lblLETd.Text = CommonTbl.Rows[0]["LclEnq"].ToString();
            //            lblLQTod.Text = CommonTbl.Rows[0]["LclQuote"].ToString();
            //            lblLpoTd.Text = CommonTbl.Rows[0]["LclPOrder"].ToString();

            //            lblDrwngAprlsTBCmpldToday.Text = CommonTbl.Rows[0]["DrwngAprlsTBCmpltd"].ToString();
            //            lblDrwngAprlsCmpldToday.Text = CommonTbl.Rows[0]["DrwngAprlsCmpltd"].ToString();

            //            lblInsptnTBCmpltdToday.Text = CommonTbl.Rows[0]["InsptnTBCmpltdToDay"].ToString();
            //            lblInsptnCmpltdToday.Text = CommonTbl.Rows[0]["InsptnCmpltdToDay"].ToString();

            //            lblCEDEATBCmpltdToday.Text = CommonTbl.Rows[0]["CT1DtlsTBCmpltdToday"].ToString();
            //            lblCEDEACmpltdToday.Text = CommonTbl.Rows[0]["CT1DtlsCmpltdToday"].ToString();

            //            break;
            //        case "LclEnq":
            //            lblFrnEnqTillDt.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
            //            lblFQTillDt.Text = CommonTbl.Rows[0]["FrnQuote"].ToString();
            //            lblFPORcvdTillDt.Text = CommonTbl.Rows[0]["FrnPOrder"].ToString();

            //            lblLETillDt.Text = CommonTbl.Rows[0]["LclEnq"].ToString();
            //            lblLQTillDt.Text = CommonTbl.Rows[0]["LclQuote"].ToString();
            //            lblLPOTillDt.Text = CommonTbl.Rows[0]["LclPOrder"].ToString();

            //            lblDrawingApprovalsCmpld.Text = CommonTbl.Rows[0]["DrwngAprlsCmpltd"].ToString();
            //            lblDrawingApprovalsPndng.Text = CommonTbl.Rows[0]["DrwngAprlsPndng"].ToString();

            //            lblInstptnCmpltdDt.Text = CommonTbl.Rows[0]["InsptnCmpltd"].ToString();
            //            lblInstptnPndngDt.Text = CommonTbl.Rows[0]["InsptnPndng"].ToString();

            //            lblCEDEACmpltDt.Text = CommonTbl.Rows[0]["CT1DtlsCmpltd"].ToString();
            //            lblCEDEAPndngDt.Text = CommonTbl.Rows[0]["CT1DtlsPndng"].ToString();


            //            break;
            //        case "Remainders1":
            //            //lblDrawingApprovalsToday.Text = CommonTbl.Rows[0][0].ToString();
            //            //lblInspectionApprovalsToday.Text = CommonTbl.Rows[0][1].ToString();
            //            //lblExciseDutyExcemptionToday.Text = CommonTbl.Rows[0][2].ToString();
            //            break;
            //        case "Remainders":
            //            //lblDrawingApprovals.Text = CommonTbl.Rows[0][0].ToString();
            //            //lblInspectionApprovals.Text = CommonTbl.Rows[0][1].ToString();
            //            //lblExciseDutyExcemption.Text = CommonTbl.Rows[0][2].ToString();
            //            break;
            //        case "LclQuote": ; break;
            //        case "FrnQuote": ; break;
            //        case "FrnPOrder": ; break;
            //        case "LclPOrder": ; break;
            //        case "Cancel":
            //            lblFECancel.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
            //            break;
            //        default: ; break;
            //    }
            //}
        }

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("S.No.");
                dt.Columns.Add("ForeignEnquireId");
                dt.Columns.Add("CusmorId");
                dt.Columns.Add("DepartmentId");
                dt.Columns.Add("EnquireNumber");
                dt.Columns.Add("EnquiryDate");
                dt.Columns.Add("ReceivedDate");
                dt.Columns.Add("DueDate");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());


                //int columncount = gvNewFE.Rows[0].Cells.Count;
                //gvNewFE.Rows[0].Cells.Clear();
                //gvNewFE.Rows[0].Cells.Add(new TableCell());
                //gvNewFE.Rows[0].Cells[0].ColumnSpan = columncount;
                //gvNewFE.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Home Page", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Empty Table for using select statement...
        /// </summary>
        /// <returns></returns>
        private DataTable EmptyDt()
        {
            try
            {
                return CommonBLL.EmptyDt();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Home Page", ex.Message.ToString());
                return null;
            }

            # region NotInUse
            //CommonBLL cbll=new CommonBLL ();
            //try
            //{
            //    DataTable dt = new DataTable();
            //    dt.Columns.Add(new DataColumn("Category", typeof(long)));
            //    dt.Columns.Add(new DataColumn("ItemDescription", typeof(long)));
            //    dt.Columns.Add(new DataColumn("PartNo", typeof(string)));
            //    dt.Columns.Add(new DataColumn("Specification", typeof(string)));
            //    dt.Columns.Add(new DataColumn("Make", typeof(string)));
            //    dt.Columns.Add(new DataColumn("Quantity", typeof(Int32)));
            //    dt.Columns.Add(new DataColumn("Units", typeof(long)));
            //    dt.Columns.Add(new DataColumn("ID", typeof(long)));

            //    DataRow dr = dt.NewRow();
            //    dr["Category"] = 0;
            //    dr["ItemDescription"] = 0;
            //    dr["PartNo"] = string.Empty;
            //    dr["Specification"] = string.Empty;
            //    dr["Make"] = string.Empty;
            //    dr["Quantity"] = 0;
            //    dr["Units"] = 0;
            //    dr["ID"] = 0;
            //    dt.Rows.Add(dr);
            //    return dt;
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            //    return null;
            //}
            #endregion
        }

        # endregion

        #region Classes

        public class DashboardDetails
        {
            public string id { get; set; }
            public string name { get; set; }
            public decimal? y { get; set; }
            public object data { get; set; }
            public string drilldown { get; set; }
            public string code { get; set; }
        }

        #endregion
    }
}
