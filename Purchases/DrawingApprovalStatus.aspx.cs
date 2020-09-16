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
using System.Threading;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Purchases
{
    public partial class DrawingApprovalStatus : System.Web.UI.Page
    {
        # region Variables
        int UserID;
        ErrorLog ELog = new ErrorLog();
        DrawingApprovalBLL DABLL = new DrawingApprovalBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        SupplierBLL SUPLRBL = new SupplierBLL();
        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(DrawingApprovalStatus));
            if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (!IsPostBack)
                    {
                        //txtfromdt.Attributes.Add("readonly", "readonly");
                        //txttodt.Attributes.Add("readonly", "readonly");
                        //GetData();
                        //txtfromdt.Attributes.Add("readonly", "readonly");
                        //txttodt.Attributes.Add("readonly", "readonly");
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no");
            }
        }

        #endregion

        # region Methods

        /// <summary>
        /// This method is used to bind search functional controls
        /// </summary>
        protected void GetData()
        {
            try
            {
                //BindDropDownList(ddlCustomer, CSTMRBL.SelectCustomers(CommonBLL.FlagRegularDRP, 0));
                //BindDropDownList(ddlSupplier, SUPLRBL.SelectSuppliers(CommonBLL.FlagRegularDRP, 0));
                DataTable dt = EmptyDt();
                dt = CommonBLL.FirstRowPaymentTerms();
                //if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                    //if (Request.QueryString["Mode"].ToString() == "Dtdt")
                    //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagJSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                    //    Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                    //    DateTime.Now, DateTime.Now, "", "", "", Convert.ToInt64(Session["UserID"]), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                    //    true, CommonBLL.EmptyDTDrawingDetails(), ""));
                    //if (Request.QueryString["Mode"].ToString() == "DCtldt")
                    //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagZSelect, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                    //    DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Convert.ToInt64(Session["UserID"]),
                    //    CommonBLL.StartDate, 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), true,
                    //    CommonBLL.EmptyDTDrawingDetails(), ""));
                    //else
                    //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                    //           DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0,
                    //           DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //}
                //if (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
                //{
                //    if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "Dtdt")
                //        BindGridView(DABLL.GetDataSet(CommonBLL.FlagJSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, "", "", "", Convert.ToInt64(Session["UserID"]), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                //        true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //    else if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "DCtldt")
                //        BindGridView(DABLL.GetDataSet(CommonBLL.FlagZSelect, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //        DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Convert.ToInt64(Session["UserID"]),
                //        CommonBLL.StartDate, 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), true,
                //        CommonBLL.EmptyDTDrawingDetails(), ""));
                //    else
                //        BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //           DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0,
                //           DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //}
                //else if (Request.QueryString.Count > 0 && !String.IsNullOrEmpty(Request.QueryString["Mode"].ToString()))
                //{
                //    if (Convert.ToInt64(Session["UserID"]) == CommonBLL.AdminID)
                //    {
                //        if (Request.QueryString["Mode"].ToString() == "Dtdt")
                //            BindGridView(DABLL.GetDataSet(CommonBLL.FlagLSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //            Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                //            DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                //            true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //        if (Request.QueryString["Mode"].ToString() == "DPtdt")
                //            BindGridView(DABLL.GetDataSet(CommonBLL.FlagOSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //            Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                //            DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                //            true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //        if (Request.QueryString["Mode"].ToString() == "DCtldt")
                //            BindGridView(DABLL.GetDataSet(CommonBLL.FlagYSelect, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //            DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Convert.ToInt64(Session["UserID"]),
                //            CommonBLL.StartDate, 0, CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyy")), true,
                //            CommonBLL.EmptyDTDrawingDetails(), ""));
                //    }
                //    else
                //        BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //            DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0,
                //            DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //    BindDropDownList(ddlCustomer, CSTMRBL.SelectCustomers(CommonBLL.FlagRegularDRP, 0));
                //    BindDropDownList(ddlSupplier, SUPLRBL.SelectSuppliers(CommonBLL.FlagRegularDRP, 0));
                //}
                //else
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //           DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0,
                //           DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), ""));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        //private void BindGridView()
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds = DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
        //            DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0,
        //            DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "");
        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            GVDrawingApprovalStat.DataSource = ds.Tables[0];
        //            GVDrawingApprovalStat.DataBind();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This is used to bind DDL's
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        //protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        //{
        //    try
        //    {
        //        ddl.DataSource = CommonDt.Tables[0];
        //        ddl.DataTextField = "Description";
        //        ddl.DataValueField = "ID";
        //        ddl.DataBind();
        //        ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This is used to delete
        /// </summary>
        /// <param name="ID"></param>
        //public void DeleteItemDetails(Guid ID)
        //{
        //    try
        //    {
        //        int res = 1;
        //        DataSet EditDS = new DataSet();
        //        LPOrdersBLL NLPOBL = new LPOrdersBLL();
        //        res = DABLL.InsertUpdateDelete(CommonBLL.FlagDelete, ID, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", "",
        //            DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Guid.Empty,
        //            DateTime.Now, new Guid(Session[UserID].ToString()), DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "",new Guid(Session["CompanyId"].ToString()));
        //        if (res == 0)
        //        {
        //            BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", "",
        //                DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty,
        //                DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "", new Guid(Session["CompanyId"].ToString())));
        //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
        //            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Drawing Approval Status", "Deleted successfully.");
        //        }
        //        else if (res != 0)
        //        {
        //            if (res == -123)
        //            {
        //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                    "ErrorMessage('Cannot Delete this Record, LPO already created so delete LPO.');", true);
        //                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status",
        //                    "Cannot Delete Record " + ID + ".");
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
        //                    "ErrorMessage('Error while Deleting.');", true);
        //                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status",
        //                    "Error while Deleting " + ID + ".");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This is used to Bind GridView
        /// </summary>
        /// <param name="ds"></param>
        private void BindGridView(DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                  //  GVDrawingApprovalStat.DataSource = ds;
                   // GVDrawingApprovalStat.DataBind();
                }
                else
                {
                    NoTable();
                    //GVDrawingApprovalStat.DataSource = null;
                    //GVDrawingApprovalStat.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        #endregion

        #region DataTable

        private DataTable EmptyDt()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("DrawingID", typeof(long)));
                dt.Columns.Add(new DataColumn("DrawingRefNo", typeof(long)));
                dt.Columns.Add(new DataColumn("CustNM", typeof(string)));
                dt.Columns.Add(new DataColumn("SupNM", typeof(string)));
                dt.Columns.Add(new DataColumn("FPO", typeof(string)));
                dt.Columns.Add(new DataColumn("LPO", typeof(string)));
                dt.Columns.Add(new DataColumn("LPODate", typeof(string)));
                dt.Columns.Add(new DataColumn("CreatedBy", typeof(long)));

                DataRow dr = dt.NewRow();
                dr["DrawingID"] = 0;
                dr["CustNM"] = string.Empty;
                dr["SupNM"] = string.Empty;
                dr["FPO"] = string.Empty;
                dr["LPO"] = string.Empty;
                dr["LPODate"] = string.Empty;
                dr["CreatedBy"] = 0;
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
                return null;
            }
        }

        private void NoTable()
        {
            //try
            //{
            //    DataSet ds = new DataSet();
            //    DataTable dt = new DataTable();
            //    dt.Columns.Add("DrawingID");
            //    dt.Columns.Add("DrawingRefNo");
            //    dt.Columns.Add("CustNM");
            //    dt.Columns.Add("SupNM");
            //    dt.Columns.Add("FPO");
            //    dt.Columns.Add("LPO");
            //    dt.Columns.Add("LPODate");
            //    dt.Columns.Add("CreatedBy");
            //    ds.Tables.Add(dt);
            //    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            //    //GVDrawingApprovalStat.DataSource = ds;
            //   // GVDrawingApprovalStat.DataBind();
            //   // int columncount = GVDrawingApprovalStat.Rows[0].Cells.Count;
            //    //gvLenquiries.Rows[0].Cells.Clear();
            //    for (int i = 0; i < columncount; i++)
            //    //    GVDrawingApprovalStat.Rows[0].Cells[i].Style.Add("display", "none");
            // //   GVDrawingApprovalStat.Rows[0].Cells.Add(new TableCell());
            //  //  GVDrawingApprovalStat.Rows[0].Cells[columncount].ColumnSpan = columncount;
            //  //  GVDrawingApprovalStat.Rows[0].Cells[columncount].Text = "<center>No Records To Display...!</center>";
            //    //gvLenquiries.Rows[0].CssClass = "bcGridViewRowStyle odd";
            //    //gvLenquiries.Rows[0].Style.Add("class", "bcGridViewRowStyle odd");
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            //}
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// This is used to search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text),
                //        0, CommonBLL.DateInsert(txttodt.Text), true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text), 0, CommonBLL.DateInsert(txttodt.Text),
                //        true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0, CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagQSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0, CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagXSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0, CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagQSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text), 0,
                //        CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagQSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0,
                //        CommonBLL.DateInsert(txttodt.Text), true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagXSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text), 0,
                //        CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagXSelect, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0,
                //        CommonBLL.DateInsert(txttodt.Text), true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text), 0,
                //        CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue == "0" && ddlSupplier.SelectedValue == "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0,
                //        CommonBLL.DateInsert(txttodt.Text), true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() != ""
                //    && txttodt.Text.Trim() == "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.DateInsert(txtfromdt.Text), 0,
                //        CommonBLL.EndDate, true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else if (ddlCustomer.SelectedValue != "0" && ddlSupplier.SelectedValue != "0" && txtfromdt.Text.Trim() == ""
                //    && txttodt.Text.Trim() != "")
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, Int64.Parse(ddlCustomer.SelectedValue),
                //        Int64.Parse(ddlSupplier.SelectedValue), 0, 0, DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, CommonBLL.StartDate, 0,
                //        CommonBLL.DateInsert(txttodt.Text), true, CommonBLL.EmptyDTDrawingDetails(), ""));
                //else
                //    BindGridView(DABLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, 0, DateTime.Now, "", "",
                //        DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", 0, DateTime.Now,
                //        0, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), ""));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        #endregion

        # region GridView Events

        /// <summary>
        /// This is used for prerender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVDrawingApprovalStat_PreRender(object sender, EventArgs e)
        {
            try
            {
                //GVDrawingApprovalStat.UseAccessibleHeader = false;
                //GVDrawingApprovalStat.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVDrawingApprovalStat_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
               // GridViewRow gvrow = GVDrawingApprovalStat.Rows[index];
               // int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblLPOrderId")).Text);
                if (e.CommandName == "Modify")
                    Response.Redirect("../Purchases/DrawingApproval.Aspx?ID=" + ID, false);
                //else if (e.CommandName == "Remove")
                 //   DeleteItemDetails(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Row Deleting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVDrawingApprovalStat_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid Veiw Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVDrawingApprovalStat_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                Label CrtedBy = (Label)e.Row.FindControl("lblCrtdBy");

                if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                {
                    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                    {
                        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            UserID != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text))))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //EditButton.Enabled = false;
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    else
                    {
                        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                        Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") ||
                        Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Text)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Text) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Text)))
                        {
                            //EditButton.Enabled = false;
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                }
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        # endregion

        #region Export Buttons Click Events

        /// <summary>
        /// Export to Excel Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //if (GVDrawingApprovalStat.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.GVDrawingApprovalStat.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "";
                int LoginID = 0;
                if ((CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString() ||
                    CommonBLL.TraffickerContactTypeText == ((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString()) && Mode != null)
                    LoginID = Convert.ToInt32(((ArrayList)Session["UserDtls"])[1].ToString());

                if (Mode == "DCtldt")
                {
                    FrmDt = HFLPOFDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFLPOFDt.Value).ToString("yyyy-MM-dd");
                    if (HFLPOTDt.Value != "")
                    {
                        ToDat = HFLPOTDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFLPOTDt.Value).ToString("yyyy-MM-dd");
                    }
                    else
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "Dtdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    FrmDt = HFLPOFDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFLPOFDt.Value).ToString("yyyy-MM-dd");
                    ToDat = HFLPOTDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFLPOTDt.Value).ToString("yyyy-MM-dd");
                }
                string DRefNo = HFDrwngRefNo.Value;
                string Cstnm = HFCustNm.Value;
                string Supnm = HFSuplNm.Value;
                string FPO = HFFPONo.Value;
                string LPO = HFLPONo.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = DABLL.Drwng_Search(FrmDt, ToDat, DRefNo, Cstnm, Supnm, FPO, LPO, CreatedDT, LoginID, Mode, new Guid(Session["CompanyID"].ToString()));
                if (ds != null && ds.Tables.Count > 0)
                {
                    //string Title = "DRAWING APPROVAL STATUS";
                    string attachment = "attachment; filename=DrawingAprlsDtls.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";
                    string MTitle = "STATUS OF DRAWING APPROVAL ", MTcustomer = "", MTDTS = "";
                    if (HFCustNm.Value != "")
                        MTcustomer = HFCustNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                             + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + ""
                                             + MTDTS + "</center></b>");

                    // htextw.Write("<center><b>" + Title + "</b></center>");
                    DataGrid dgGrid = new DataGrid();
                    //dgGrid.HeaderStyle.Font.Bold = true;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        #endregion

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust, string CompanyId)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    //DataTable dt = EmptyDt();
                    //DataSet EditDS = DABLL.GetDataSet(CommonBLL.FlagSelectAll,Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", "",
                    //        DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty,
                    //        DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "", new Guid(Session["CompanyId"].ToString()));
                    //if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                    //    res = -123;

                    //else
                    //{
                        res = DABLL.InsertUpdateDelete(CommonBLL.FlagDelete,new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", "",
                    DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Guid.Empty,
                    DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "",new Guid(Session["CompanyId"].ToString()));
                    }
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, LPO already created so delete LPO/ Error while Deleting " + ID;
                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
         #endregion //delete
        
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
    }
}
