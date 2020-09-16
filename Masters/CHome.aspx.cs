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

namespace VOMS_ERP.Masters
{
    public partial class CHome : System.Web.UI.Page
    {
        # region variables
        int res;
        DeshBoardBLL DBBL = new DeshBoardBLL();
        ErrorLog ELog = new ErrorLog();
        #endregion

        #region Page Load

        void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["UserType"] != null && Session["UserType"].ToString() == "Customer")
            {
                MasterPageFile = "~/CustomerMaster.master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                        GetData();
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
        /// Bind Data to GirdView and DropDownList
        /// </summary>
        protected void GetData()
        {
            try
            {
                //if (CommonBLL.AdminRole == (Session["AccessRole"].ToString())) // Admin
                if (Session["IsUser"] == "" || Session["IsUser"] == null) // Admin
                {
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagCommonMstr, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "FrnEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagSelectAll, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "LclEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagYSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                                            Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagCSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                                            Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders1");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagPSelectAll, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Cancel");
                }
                else if (Session["UserID"].ToString().Trim() == Session["TLID"].ToString().Trim()) // TeamLead
                {
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagGSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "FrnEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagRegularDRP, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "LclEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagWCommonMstr, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["TeamMembers"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagASelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["TeamMembers"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders1");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagZSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Cancel");
                }
                else // User
                {
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagGSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "FrnEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagRegularDRP, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "LclEnq");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagXSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagBSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Remainders1");
                    BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagZSelect, CommonBLL.StartDate, CommonBLL.EndDate,
                        Session["UserID"].ToString(), new Guid(Session["CompanyID"].ToString()))).Tables[0], "Cancel");
                }

                //BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagCommonMstr, CommonBLL.StartDate, CommonBLL.EndDate, 0)).Tables[0], 
                //"LclQuote");
                //BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagCommonMstr, CommonBLL.StartDate, CommonBLL.EndDate, 0)).Tables[0],
                //"FrnQuote");
                //BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagSelectAll, CommonBLL.StartDate, CommonBLL.EndDate, 0)).Tables[0], 
                //"FrnPOrder");
                //BindTableFields((DBBL.SelectDBCounts(CommonBLL.FlagCommonMstr, CommonBLL.StartDate, CommonBLL.EndDate, 0)).Tables[0], 
                //"LclPOrder");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Home Page", ex.Message.ToString());
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
            if (CommonTbl.Rows.Count > 0)
            {
                switch (Type)
                {
                    case "FrnEnq": lblFrnEnqTd.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
                        lblFQTd.Text = CommonTbl.Rows[0]["FrnQuote"].ToString();
                        lblFPORcvdToday.Text = CommonTbl.Rows[0]["FrnPOrder"].ToString();

                        lblLETd.Text = CommonTbl.Rows[0]["LclEnq"].ToString();
                        lblLQTod.Text = CommonTbl.Rows[0]["LclQuote"].ToString();
                        lblLpoTd.Text = CommonTbl.Rows[0]["LclPOrder"].ToString();

                        lblDrwngAprlsTBCmpldToday.Text = CommonTbl.Rows[0]["DrwngAprlsTBCmpltd"].ToString();
                        lblDrwngAprlsCmpldToday.Text = CommonTbl.Rows[0]["DrwngAprlsCmpltd"].ToString();

                        lblInsptnTBCmpltdToday.Text = CommonTbl.Rows[0]["InsptnTBCmpltdToDay"].ToString();
                        lblInsptnCmpltdToday.Text = CommonTbl.Rows[0]["InsptnCmpltdToDay"].ToString();

                        lblCEDEATBCmpltdToday.Text = CommonTbl.Rows[0]["CT1DtlsTBCmpltdToday"].ToString();
                        lblCEDEACmpltdToday.Text = CommonTbl.Rows[0]["CT1DtlsCmpltdToday"].ToString();

                        break;
                    case "LclEnq":
                        lblFrnEnqTillDt.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
                        lblFQTillDt.Text = CommonTbl.Rows[0]["FrnQuote"].ToString();
                        lblFPORcvdTillDt.Text = CommonTbl.Rows[0]["FrnPOrder"].ToString();

                        lblLETillDt.Text = CommonTbl.Rows[0]["LclEnq"].ToString();
                        lblLQTillDt.Text = CommonTbl.Rows[0]["LclQuote"].ToString();
                        lblLPOTillDt.Text = CommonTbl.Rows[0]["LclPOrder"].ToString();

                        lblDrawingApprovalsCmpld.Text = CommonTbl.Rows[0]["DrwngAprlsCmpltd"].ToString();
                        lblDrawingApprovalsPndng.Text = CommonTbl.Rows[0]["DrwngAprlsPndng"].ToString();

                        lblInstptnCmpltdDt.Text = CommonTbl.Rows[0]["InsptnCmpltd"].ToString();
                        lblInstptnPndngDt.Text = CommonTbl.Rows[0]["InsptnPndng"].ToString();

                        lblCEDEACmpltDt.Text = CommonTbl.Rows[0]["CT1DtlsCmpltd"].ToString();
                        lblCEDEAPndngDt.Text = CommonTbl.Rows[0]["CT1DtlsPndng"].ToString();


                        break;
                    case "Remainders1":
                        //lblDrawingApprovalsToday.Text = CommonTbl.Rows[0][0].ToString();
                        //lblInspectionApprovalsToday.Text = CommonTbl.Rows[0][1].ToString();
                        //lblExciseDutyExcemptionToday.Text = CommonTbl.Rows[0][2].ToString();
                        break;
                    case "Remainders":
                        //lblDrawingApprovals.Text = CommonTbl.Rows[0][0].ToString();
                        //lblInspectionApprovals.Text = CommonTbl.Rows[0][1].ToString();
                        //lblExciseDutyExcemption.Text = CommonTbl.Rows[0][2].ToString();
                        break;
                    case "LclQuote": ; break;
                    case "FrnQuote": ; break;
                    case "FrnPOrder": ; break;
                    case "LclPOrder": ; break;
                    case "Cancel":
                        lblFECancel.Text = CommonTbl.Rows[0]["FrnEnq"].ToString();
                        break;
                    default: ; break;
                }
            }
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
    }
}