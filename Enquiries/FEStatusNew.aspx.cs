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
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;

namespace VOMS_ERP.Enquiries
{
    public partial class FEStatusNew : System.Web.UI.Page
    {
        # region variables
        int res;
        public static ArrayList Files = new ArrayList();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        BAL.FEnquiryBLL frnfenq = new FEnquiryBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        BAL.CustomerBLL cusmr = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ErrorLog ELog = new ErrorLog();
        static string GeneralCtgryID;
        static DataSet EditDS;
        static string btnSaveID = "";
        #endregion

        #region Default Page Load Event
        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        if (!IsPostBack)
                        {

                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }
        #endregion

        #region Get Data and Bind to Controls
        /// <summary>
        /// Getdata for All DropDownLists and GridViews
        /// </summary>
        //protected void GetData()
        //{
        //    try
        //    {
        //        DataTable dt = EmptyDt();
        //        BindDropDownList(ddlCustomer, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, 0));

        //        if (CommonBLL.CustmrContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
        //        {
        //            if (Request.QueryString["Mode"] == "tldt")
        //            {
        //                BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagJSelect, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
        //                DateTime.Now, "", 0, "", "", Convert.ToInt64(Session["UserID"]), 0, true, dt));
        //            }
        //            else
        //                BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagSelectAll, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
        //            DateTime.Now, "", 0, "", "", 0, 0, true, dt));
        //        }
        //        else if (CommonBLL.TraffickerContactType == Convert.ToInt64(((ArrayList)Session["UserDtls"])[7].ToString()))
        //        {
        //            if (Request.QueryString["Mode"] == "tldt")
        //            {
        //                BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagJSelect, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
        //                DateTime.Now, "", 0, "", "", Convert.ToInt64(Session["UserID"]), 0, true, dt));
        //            }
        //            else
        //                BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagSelectAll, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
        //            DateTime.Now, "", 0, "", "", 0, 0, true, dt));
        //        }
        //        else
        //            BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagSelectAll, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
        //            DateTime.Now, "", 0, "", "", 0, 0, true, dt));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
        //    }
        //}
        /// <summary>
        /// Bind DropDownLists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }
        /// <summary>
        /// Binding GridView
        /// </summary>
        //private void BindGridView(DataSet FrnEnqs)
        //{
        //    try
        //    {
        //        DataTable dt = EmptyDt();
        //        if (FrnEnqs.Tables.Count > 0 && FrnEnqs.Tables[0].Rows.Count > 0)
        //        {
        //            gvNewFE.DataSource = FrnEnqs;
        //            gvNewFE.DataBind();
        //            ViewState["dset"] = FrnEnqs;
        //            //if (!CommonBLL.UsrPermissions.Contains("Edit"))
        //            //{
        //            //    NoEdit(gvNewFE);
        //            //}
        //            //if (!CommonBLL.UsrPermissions.Contains("Delete"))
        //            //{
        //            //    NoDelete(gvNewFE);
        //            //}
        //        }
        //        else
        //            NoTable();

        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        //private void NoTable()
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("S.No.");
        //        dt.Columns.Add("ForeignEnquireId");
        //        dt.Columns.Add("CusmorId");
        //        dt.Columns.Add("ItemDetailsID");
        //        dt.Columns.Add("DepartmentId");
        //        dt.Columns.Add("EnquireNumber");
        //        dt.Columns.Add("EnquiryNumber");
        //        dt.Columns.Add("EnquiryDate");
        //        dt.Columns.Add("ReceivedDate");
        //        dt.Columns.Add("DueDate");
        //        dt.Columns.Add("FPONumber");
        //        dt.Columns.Add("Subject");
        //        dt.Columns.Add("Status");
        //        dt.Columns.Add("StatusTypeId");
        //        dt.Columns.Add("CreatedBy");
        //        ds.Tables.Add(dt);
        //        ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
        //        gvNewFE.DataSource = ds;
        //        gvNewFE.DataBind();
        //        int columncount = gvNewFE.Rows[0].Cells.Count;
        //        gvNewFE.Rows[0].Cells.Clear();
        //        gvNewFE.Rows[0].Cells.Add(new TableCell());
        //        gvNewFE.Rows[0].Cells[0].ColumnSpan = columncount;
        //        gvNewFE.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
        //    }
        //}


        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteItemDetails(Guid ID)
        {
            try
            {
                DataTable dt = EmptyDt();DataTable DtSubItems = EmptyDt();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                if (HttpContext.Current.Session["AllFESubItems"].ToString() != "")
                    DtSubItems = (DataTable)HttpContext.Current.Session["AllFESubItems"];
                else
                    DtSubItems = CommonBLL.FEEmpty_SubItems();
                int res = NEBLL.NewEnquiryInsert(CommonBLL.FlagDelete, ID, Guid.Empty,Guid.Empty,Guid.Empty, "", DateTime.Now,"","",
                    DateTime.Now, DateTime.Now, DateTime.Now, "", Guid.Empty,0, "", "", Guid.Empty, Guid.Empty, false, new Guid(Session["CompanyID"].ToString()), dt,DtSubItems);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "Enquiry Status New", "Row Deleted successfully.");
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", "Error while Deleting.");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Empty Data Tables
        /// </summary>
        /// <returns></returns>
        private DataTable EmptyDt()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("SNo", typeof(Int32)));
                dt.Columns.Add(new DataColumn("Category", typeof(long)));
                dt.Columns.Add(new DataColumn("ItemDescription", typeof(long)));
                dt.Columns.Add(new DataColumn("PartNo", typeof(string)));
                dt.Columns.Add(new DataColumn("Specification", typeof(string)));
                dt.Columns.Add(new DataColumn("Make", typeof(string)));
                dt.Columns.Add(new DataColumn("Quantity", typeof(Int32)));
                dt.Columns.Add(new DataColumn("Units", typeof(long)));
                dt.Columns.Add(new DataColumn("ID", typeof(long)));

                DataRow dr = dt.NewRow();
                dr["SNo"] = 0;
                dr["Category"] = 0;
                dr["ItemDescription"] = 0;
                dr["PartNo"] = string.Empty;
                dr["Specification"] = string.Empty;
                dr["Make"] = string.Empty;
                dr["Quantity"] = 0;
                dr["Units"] = 0;
                dr["ID"] = 0;
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
                return null;
            }
        }

        private void NoEdit(GridView gv)
        {
            try
            {
                ;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }
        private void NoDelete(GridView gv)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }
        #endregion

        #region Search/Export to Excel/Export to Pdf Buttons Click Events

        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //DataTable dt = EmptyDt();
                //if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagSelectAll, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, "", 0, "", "", 0, 0, true, dt));

                //DataTable dt = EmptyDt();
                //if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //        CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagFSelect, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue != "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() != "" && txttodt.Text.Trim() == "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.DateInsert(txtfromdt.Text), CommonBLL.EndDate, DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else if (ddlCustomer.SelectedValue == "0" && txtfromdt.Text.Trim() == "" && txttodt.Text.Trim() != "")
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagWCommonMstr, 0, int.Parse(ddlCustomer.SelectedValue), 0, "", "",
                //    CommonBLL.StartDate, CommonBLL.DateInsert(txttodt.Text), DateTime.Now, "", 0, "", "", 0, 0, true, dt));
                //else
                //    BindGridView(NEBLL.NewEnquiryEdit(CommonBLL.FlagXSelect, 0, 0, 0, "", "", DateTime.Now, DateTime.Now,
                //        DateTime.Now, "", 0, "", "", 0, 0, true, dt));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }

        #endregion

        #region Grid Veiw Events

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvNewFE_PreRender(object sender, EventArgs e)
        {
            try
            {
                //gvNewFE.UseAccessibleHeader = false;
                //gvNewFE.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvNewFE_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string status = ((Label)e.Row.FindControl("lblStatusID")).Text;
                    //string[] statval = status.Split('.');
                    switch (status.Trim())
                    {
                        case "10": e.Row.BackColor = System.Drawing.Color.LightPink; break; // This will make row back color Light Pik
                        case "20": e.Row.BackColor = System.Drawing.Color.Orange; break; // This will make row back color Orange
                        case "30": e.Row.BackColor = System.Drawing.Color.Khaki; break; // This will make row back color Aque
                        case "40": e.Row.BackColor = System.Drawing.Color.Gray; break; // This will make row back color Gray
                        case "50": e.Row.BackColor = System.Drawing.Color.LightBlue; break; // This will make row back color Light Blue
                        case "60": e.Row.BackColor = System.Drawing.Color.LawnGreen; break; // This will make row back color Laqn Green
                        case "61": e.Row.BackColor = System.Drawing.Color.DimGray; break; // This will make row back color Bisque
                        case "62": e.Row.BackColor = System.Drawing.Color.Chocolate; break; // This will make row back color Bisque
                        case "63": e.Row.BackColor = System.Drawing.Color.Coral; break; // This will make row back color Bisque
                        case "64": e.Row.BackColor = System.Drawing.Color.Crimson; break; // This will make row back color Bisque
                        case "65": e.Row.BackColor = System.Drawing.Color.Bisque; break; // This will make row back color Bisque
                        case "66": e.Row.BackColor = System.Drawing.Color.CadetBlue; break; // This will make row back color Bisque
                        case "67": e.Row.BackColor = System.Drawing.Color.DarkKhaki; break; // This will make row back color Bisque
                        case "68": e.Row.BackColor = System.Drawing.Color.Fuchsia; break; // This will make row back color Bisque
                        case "69": e.Row.BackColor = System.Drawing.Color.DarkCyan; break; // This will make row back color Bisque
                        case "70": e.Row.BackColor = System.Drawing.Color.Gold; break; // This will make row back color Bisque
                        case "75": e.Row.BackColor = System.Drawing.Color.Ivory; break; // This will make row back color Bisque
                        case "80": e.Row.BackColor = System.Drawing.Color.LightSteelBlue; break; // This will make row back color Bisque
                        case "81": e.Row.BackColor = System.Drawing.Color.MintCream; break; // This will make row back color Bisque
                        case "82": e.Row.BackColor = System.Drawing.Color.MistyRose; break; // This will make row back color Bisque
                        case "83": e.Row.BackColor = System.Drawing.Color.Olive; break; // This will make row back color Bisque
                        case "85": e.Row.BackColor = System.Drawing.Color.Orchid; break; // This will make row back color Bisque
                        case "86": e.Row.BackColor = System.Drawing.Color.PaleGreen; break; // This will make row back color Bisque
                        case "87": e.Row.BackColor = System.Drawing.Color.Peru; break; // This will make row back color Bisque
                        case "88": e.Row.BackColor = System.Drawing.Color.RosyBrown; break; // This will make row back color Bisque
                        case "89": e.Row.BackColor = System.Drawing.Color.Salmon; break; // This will make row back color Bisque
                        case "90": e.Row.BackColor = System.Drawing.Color.Sienna; break; // This will make row back color Bisque
                        case "91": e.Row.BackColor = System.Drawing.Color.SkyBlue; break; // This will make row back color Bisque
                        case "92": e.Row.BackColor = System.Drawing.Color.Tomato; break; // This will make row back color Bisque
                        case "95": e.Row.BackColor = System.Drawing.Color.Turquoise; break; // This will make row back color Bisque
                        default: e.Row.BackColor = System.Drawing.Color.Indigo; break; // This will make row back color Indio
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }

        #endregion

        #region Export To Excel

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"] != "")
                {
                    string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    string FrmDt = "", ToDat = "";
                    string FromRcvdDt = "", ToRcvdDt = "";
                    Guid LoginID = Guid.Empty;
                    if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()) || (CommonBLL.TraffickerContactTypeText == (((ArrayList)(HttpContext.Current.Session["UserDtls"]))[7].ToString())))
                        LoginID = new Guid(((ArrayList)Session["UserDtls"])[1].ToString());

                    if (Mode == "tldt")
                    {
                        FrmDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                        FromRcvdDt = CommonBLL.StartDate.ToString("yyyy-MM-dd");
                        ToRcvdDt = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToShortDateString() : CommonBLL.DateCheck_Print(HFFromDate.Value).ToShortDateString();
                        ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToShortDateString() : CommonBLL.DateCheck_Print(HFToDate.Value).ToShortDateString();
                        FromRcvdDt = HFRcvdFromDt.Value == "" ? CommonBLL.StartDate.ToShortDateString() : CommonBLL.DateCheck_Print(HFRcvdFromDt.Value).ToShortDateString();
                        ToRcvdDt = HFRcvdToDt.Value == "" ? CommonBLL.EndDate.ToShortDateString() : CommonBLL.DateCheck_Print(HFRcvdToDt.Value).ToShortDateString();
                    }

                    string FENo = HFFENo.Value;
                    string FPONo = HFFPONo.Value;
                    string Subject = HFSubject.Value;
                    string Status = HFStatus.Value;
                    string dept = HFDept.Value;
                    string Cust = HFCust.Value;

                    if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                        FrmDt = "";
                    if (ToDat == "1-1-0001")
                        ToDat = "";
                    if (FromRcvdDt == "1-1-0001" || FromRcvdDt == "1-1-1900")
                        FromRcvdDt = "";
                    if (ToRcvdDt == "1-1-0001")
                        ToRcvdDt = "";
                    DataSet ds = frnfenq.FE_Search_SinglePage(FrmDt, ToDat, FENo, FPONo, FromRcvdDt, ToRcvdDt, Subject, Status, dept, Cust, LoginID, new Guid(Session["CompanyID"].ToString()));

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
                        htextw.Write("<center><b>" + MTitle + " " + (MTcustomer != "" ? " FROM " + MTcustomer.ToUpper() : "") + "" + MTDTS + "</center></b>");

                        DataGrid dgGrid = new DataGrid();
                        dgGrid.HeaderStyle.Font.Bold = true;

                        if (ds.Tables[0].Columns.Contains("ForeignEnquireId"))
                            ds.Tables[0].Columns.Remove("ForeignEnquireId");
                        if (ds.Tables[0].Columns.Contains("CreatedBy"))
                            ds.Tables[0].Columns.Remove("CreatedBy");

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
                            string headerTable = "<img src='" + CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "'margin-top =16px width=125 height=35 />";
                            Response.Write(headerTable);
                        }
                        Response.Write(stw.ToString());
                        Response.End();
                    }
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status New", ex.Message.ToString());
            }
        }

        #endregion
    }
}
