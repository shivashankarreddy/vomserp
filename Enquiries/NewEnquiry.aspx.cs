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
using System.Text;
using System.IO;
using System.Collections.Generic;
using Ajax;
using System.Data.OleDb;
using VOMS_ERP.Admin;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;


namespace VOMS_ERP.Enquiries
{
    public partial class NewEnquiry : System.Web.UI.Page
    {
        # region variables
        int res = 1;
        public static ArrayList Files = new ArrayList();
        EnumMasterBLL embal = new EnumMasterBLL();
        ItemMasterBLL ItmMstr = new ItemMasterBLL();
        CustomerBLL cusmr = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        NewFPOBLL NFPOBLL = new NewFPOBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ErrorLog ELog = new ErrorLog();
        AuditLogBLL AUBLL = new AuditLogBLL();
        EmailBodyBLL EBLL = new EmailBodyBLL();
        AuditLogs ALS = new AuditLogs();
        static string GeneralCtgryID;
        static DataSet EditDS;
        static string btnSaveID = "";
        int RefFlag = 0;
        static long EditID = 0;
        static string Filename = "";
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
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(NewEnquiry));
                        if (!IsPostBack)
                        {
                            ClearAll();
                            btnSaveID = btnSave.Text;
                            txtduedt.Attributes.Add("readonly", "readonly");
                            txtenqdt.Attributes.Add("readonly", "readonly");
                            txtrecvdt.Attributes.Add("readonly", "readonly");
                            if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                                hfLoginUser.Value = "false";

                            GetData();

                            #region Add/Update Permission Code
                            if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("Edit") &&
                                Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                                    btnSave.Text = "Update";
                                    btnSave.Enabled = true;
                                    btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                                    CHkShow.Enabled = false;
                                    if (Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                                    {
                                        txtenqno.Enabled = false;
                                        btnSave.Text = "Save";
                                    }
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Enabled = true;
                                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                                btnSave.Text = "Save";
                                CHkShow.Enabled = true;
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no", false);
                            # endregion
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
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
                //System.Threading.Thread.Sleep(3000);
                if ((string[])Session["UsrPermissions"] != null)
                { }
                GetGeneralID();
                BindDropDownList(ddlcustmr, cusmr.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                BindDropDownList(ddldept, embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments));

                if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                {
                    BindDropDownList(ddlcustmr, cusmr.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    //dlcustmr.SelectedValue = ((ArrayList)Session["Custmr_SuplrID"])[8].ToString();
                    lblCustomerNm.Text = ddlcustmr.SelectedItem.Text;
                    lblRcvdDt.Text = "Sent Date";
                    BindDropDownList(ddlRefForeignEnqNo, NEBLL.SelectFenquiries(CommonBLL.FlagESelect, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        DateTime.Now, DateTime.Now, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    txtrecvdt.Enabled = false;
                    txtduedt.Enabled = false;
                }

                divListBox.InnerHtml = AttachedFiles();
                divFEItems.InnerHtml = FillGridItems(Guid.Empty);
                if ((string[])Session["UsrPermissions"] != null)
                { }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind DDL Reference FE
        /// </summary>
        private void BindReferenceDDL()
        {
            try
            {
                DataTable dt = EmptyDt();
                BindDropDownList(ddlRefForeignEnqNo, NFPOBLL.Select(CommonBLL.FlagYSelect, Guid.Empty, "", Guid.Empty, Guid.Empty, Guid.Empty,
                    DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "",
                    DateTime.Now, 0, 0, 50, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                    new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(),
                    CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to bind dropdown lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>        
        protected void ClearAll()
        {
            try
            {
                //Session["uploads"] = null;
                btnSave.Text = "Save";
                btnSaveID = "Save";
                ViewState["EditID"] = null;
                Session["uploads"] = null;
                ddlcustmr.SelectedIndex = ddldept.SelectedIndex = -1;
                txtenqno.Text = txtsubject.Text = txtimpinst.Text = "";
                txtenqdt.Text = txtrecvdt.Text = txtduedt.Text = "";
                divListBox.InnerHtml = "";
                EditID = 0;
                divFEItems.InnerHtml = FillGridItems(Guid.Empty);
                Session["HFITemsValues"] = null;
                HFIsSubItem.Value = "";
                Session["AllFESubItems"] = null;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// To Fill Empty Item table grid on Page Load
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns HTML Items Table</returns>
        public string FillGridItems(Guid id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            DataTable dt = new DataTable();
            HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
            Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
            Dictionary<int, Guid> CatCodes = new Dictionary<int, Guid>();
            Dictionary<int, Guid> UnitCodes = new Dictionary<int, Guid>();

            dt.Columns.Add(new DataColumn("SNo", typeof(int)));
            dt.Columns.Add(new DataColumn("Category", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemDescription", typeof(Guid)));
            dt.Columns.Add(new DataColumn("PartNo", typeof(string)));
            dt.Columns.Add(new DataColumn("Specification", typeof(string)));
            dt.Columns.Add(new DataColumn("Make", typeof(string)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Units", typeof(Guid)));
            dt.Columns.Add(new DataColumn("ID", typeof(Guid)));
            dt.Columns.Add(new DataColumn("IsSubItems", typeof(bool)));

            Session["ItemCode"] = dt;
            Session["SelectedItems"] = Codes;
            Session["SelectedCat"] = CatCodes;
            Session["SelectedUnits"] = UnitCodes;

            return FillItemGrid(false);//0, dt, Codes, ds, 0, CatCodes, 0, UnitCodes, ""
        }

        /// <summary>
        /// This is used to Save & Update
        /// </summary>
        private void SaveRecord()
        {
            try
            {

                Guid CustID = new Guid(ddlcustmr.SelectedValue);
                Guid DeptID = new Guid(ddldept.SelectedValue);
                Guid CmpnyId = new Guid(Session["CompanyID"].ToString());
                DataTable dtt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataTable DtSubItems = EmptyDt();
                Filename = FileName();
                if (HttpContext.Current.Session["AllFESubItems"] != null)
                    DtSubItems = (DataTable)HttpContext.Current.Session["AllFESubItems"];
                else
                    DtSubItems = CommonBLL.FEEmpty_SubItems();
                if (DtSubItems.Columns.Contains("ItemDetailsId"))
                    DtSubItems.Columns.Remove("ItemDetailsId");
                if (!DtSubItems.Columns.Contains("Specifications"))
                    DtSubItems.Columns.Add("Specifications", typeof(string));
                if (dtt.Columns.Contains("IsSubItems"))
                    dtt.Columns.Remove("IsSubItems");
                int RowCount = dtt.Rows.Count - 1;
                for (int v = 0; v < dtt.Rows.Count - 1; v++)
                {
                    RowCount = dtt.Rows.Count - 1;
                    if (dtt.Rows[RowCount]["ItemDescription"].ToString() == "0" ||
                        dtt.Rows[RowCount]["Quantity"].ToString() == "0" || dtt.Rows[RowCount]["Units"].ToString() == "0")
                        dtt.Rows.RemoveAt(RowCount);
                }
                string Attachments = "";
                if (Session["uploads"] != null)
                {
                    ArrayList all = (ArrayList)Session["uploads"];
                    Attachments = string.Join(",", all.ToArray().Select(o => o.ToString()).ToArray()).ToString();
                }

                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                if (dtt.Rows.Count > 0)
                {
                    //if (hfLoginUser.Value == "true")
                    //{
                    //    if (txtrecvdt.Text == "")
                    //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Enquiry Received Date is Required');", true);
                    //    if (txtduedt.Text == "")
                    //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Enquiry Due Date is Required');", true);
                    //}
                    if (btnSave.Text == "Save" && Request.QueryString["IsAm"] == null && Request.QueryString["IsAm"] != "True")
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate, txtenqno.Text.Trim(),
                               txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text), CommonBLL.EndDate,
                               CommonBLL.EndDate, txtimpinst.Text.Trim(), Guid.Empty, CommonBLL.StatusTypeFrnEnqID, Attachments, "",
                               new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                        else
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagNewInsert, Guid.Empty, Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate, txtenqno.Text.Trim(),
                            txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text), CommonBLL.DateInsert(txtrecvdt.Text),
                            CommonBLL.DateInsert(txtduedt.Text), txtimpinst.Text.Trim(), CmpnyId, CommonBLL.StatusTypeFrnEnqID, Attachments, "",
                            new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                    else if (btnSave.Text == "Save" && Request.QueryString["IsAm"] != null && Request.QueryString["IsAm"] == "True")
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagFSelect, new Guid(Request.QueryString["ID"]), Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate, txtenqno.Text.Trim(),
                               txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text), CommonBLL.EndDate,
                               CommonBLL.EndDate, txtimpinst.Text.Trim(), Guid.Empty, CommonBLL.StatusTypeFrnEnqID, Attachments, "",
                               new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                        else
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagFSelect, new Guid(Request.QueryString["ID"]), Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate, txtenqno.Text.Trim(),
                            txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text), CommonBLL.DateInsert(txtrecvdt.Text),
                            CommonBLL.DateInsert(txtduedt.Text), txtimpinst.Text.Trim(), CmpnyId, CommonBLL.StatusTypeFrnEnqID, Attachments, "",
                            new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                    else if (btnSave.Text == "Update")
                        if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate,
                                txtenqno.Text.Trim(), txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text),
                                CommonBLL.EndDate, CommonBLL.EndDate, txtimpinst.Text.Trim(), Guid.Empty,
                                CommonBLL.StatusTypeFrnEnqID, Attachments, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()),
                                new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                        else
                        {
                            res = NEBLL.NewEnquiryInsert(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), Guid.Empty, CustID, DeptID, "", CommonBLL.EndDate,
                              txtenqno.Text.Trim(), txtsubject.Text.Trim(), CommonBLL.DateInsert(txtenqdt.Text),
                              CommonBLL.DateInsert(txtrecvdt.Text), CommonBLL.DateInsert(txtduedt.Text), txtimpinst.Text.Trim(), CmpnyId,
                              CommonBLL.StatusTypeFrnEnqID, Attachments, txtComments.Text.Trim(), new Guid(Session["UserID"].ToString()),
                              new Guid(Session["UserID"].ToString()), true, CmpnyId, dtt, DtSubItems);
                        }
                    if (res == 0 && btnSave.Text == "Save")
                    {

                        ALS.AuditLog(res, btnSave.Text, txtenqno.Text, "Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('" + CommonBLL.SuccessMsg + "');", true);

                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "NewEnquiry", "Saved successfully.");
                        CBLL.ClearUploadedFiles();
                        ClearAll();
                        divFEItems.InnerHtml = FillGridItems(Guid.Empty);
                    }
                    else if (res != 0 && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtenqno.Text, "Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('" + CommonBLL.ErrorMsg + "');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", "Error while Saving.");
                    }
                    else if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtenqno.Text, "Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "NewEnquiry", "Data Updated successfully.");
                        CBLL.ClearUploadedFiles();
                        ClearAll();
                        divFEItems.InnerHtml = FillGridItems(Guid.Empty);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, txtenqno.Text, "Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", "Error while Updating.");
                    }
                }
                else
                {
                    divFEItems.InnerHtml = FillGridItems(Guid.Empty);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('There are No Items to Save/Update.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Edit Records
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                PnlImp.Visible = false;
                DataTable dt = EmptyDt();
                EditDS = new DataSet();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                EditDS = NEBLL.NewEnquiryEdit(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                    DateTime.Now, "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), dt);
                if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                {
                    if (RefFlag == 1)
                        ddlRefForeignEnqNo.Enabled = true;
                    else
                        ddlRefForeignEnqNo.Enabled = false;
                    ddlcustmr.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                    ddldept.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                    txtsubject.Text = EditDS.Tables[0].Rows[0]["Subject"].ToString();
                    if (RefFlag == 0)
                    {
                        EditID = 1;
                        Session["checkstat"] = EditDS.Tables[2];
                        DivComments.Visible = true;
                        txtenqno.Text = EditDS.Tables[0].Rows[0]["EnquireNumber"].ToString();
                        if (EditDS.Tables[0].Rows[0]["EnquiryDate"].ToString() != "")
                            txtenqdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["EnquiryDate"].ToString()));
                        if (EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString() != "")
                            txtrecvdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                        if (EditDS.Tables[0].Rows[0]["DueDate"].ToString() != "")
                            txtduedt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["DueDate"].ToString()));
                        txtimpinst.Text = EditDS.Tables[0].Rows[0]["Instruction"].ToString();

                        if (EditDS.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        {
                            string[] all = EditDS.Tables[0].Rows[0]["Attachments"].ToString().Split(',');
                            CBLL.ClearUploadedFiles();
                            CBLL.UploadedFilesAL(all);
                            StringBuilder sb = new StringBuilder();
                            ArrayList attms = new ArrayList();
                            sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                            for (int k = 0; k < all.Length; k++)
                            {
                                if (all[k].ToString() != "")
                                {
                                    sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                                    attms.Add(all[k].ToString());
                                }
                            }
                            sb.Append("</select>");
                            Session["uploads"] = attms;
                            divListBox.InnerHtml = sb.ToString();
                        }
                        else
                            divListBox.InnerHtml = "";
                    }

                    fillEditGrid();
                    ddlcustmr.Enabled = false;
                    if (RefFlag == 0)
                    {
                        btnSave.Text = "Update";
                        ViewState["EditID"] = ID;
                        btnSaveID = "Update";
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }
        /// <summary>
        /// This is used to Fill Grid Items
        /// </summary>
        private void fillEditGrid()
        {
            DataSet dss = new DataSet();
            DataSet OldItms = new DataSet();
            //dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
            //OldItms = ItemMstBLl.SelectItemMaster(CommonBLL.FlagOSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
            HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
            HttpContext.Current.Session["OldItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagOSelect, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
            Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
            Dictionary<int, Guid> Codes1 = new Dictionary<int, Guid>();
            Dictionary<int, Guid> CatCodes1 = new Dictionary<int, Guid>();
            Dictionary<int, Guid> UnitCodes1 = new Dictionary<int, Guid>();

            DataTable dtt = EditDS.Tables[1].DefaultView.ToTable(false, "ItemId", "PartNumber",
                "Specifications", "Make", "Quantity", "UNumsId", "ItemDetailsId", "IsSubItems");
            dtt.Columns["ItemId"].ColumnName = "ItemDescription";
            dtt.Columns["PartNumber"].ColumnName = "PartNo";
            dtt.Columns["Specifications"].ColumnName = "Specification";
            dtt.Columns["ItemDetailsId"].ColumnName = "ID";
            dtt.Columns["UNumsId"].ColumnName = "Units";

            DataColumn dc = dtt.Columns.Add("SNo", typeof(int));
            DataColumn dc1 = dtt.Columns.Add("Category", typeof(Guid));
            // DataColumn dc2 = dtt.Columns.Add("IsSubItems", typeof(bool));
            dc.SetOrdinal(0);
            dc1.SetOrdinal(1);
            int sno = 1;
            foreach (DataRow dr in dtt.Rows)
            {
                dr["SNo"] = sno;
                dr["Category"] = GeneralCtgryID;
                Codes1.Add(sno, new Guid(dr["ItemDescription"].ToString()));
                UnitCodes1.Add(sno, new Guid(dr["Units"].ToString()));
                sno++;
            }
            if (EditDS.Tables.Count >= 3 && EditDS.Tables[3].Rows.Count > 0)
            {
                string ItemId = "";
                foreach (DataRow item in EditDS.Tables[1].Rows)
                {
                    ItemId = item["ItemId"].ToString();
                    Codes1.Add(Convert.ToInt32((sno)), new Guid(ItemId));
                    Session["SelectedItems"] = Codes1;
                    sno++;
                }
            }
            CatCodes1.Add(1, new Guid(GeneralCtgryID));
            HttpContext.Current.Session["SelectedItems"] = Codes1;
            HttpContext.Current.Session["SelectedCat"] = CatCodes1;
            HttpContext.Current.Session["SelectedUnits"] = UnitCodes1;
            Session["AllFESubItems"] = EditDS.Tables[3].DefaultView.ToTable(false, "SNo", "ParentItemID", "ItemId", "SpecDes", "PartNumber", "Make", "Quantity", "UNumsId", "Specifications");
            HttpContext.Current.Session["ItemCode"] = dtt;
            divFEItems.InnerHtml = FillItemGrid(false);//0, dtt, Codes1, dss, Convert.ToInt32(GeneralCtgryID), CatCodes1, 0, UnitCodes1, ""
        }

        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteItemDetails(string ID)
        {
            try
            {
                DataTable dt = EmptyDt();
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                System.Data.DataTable DtSubItems = CommonBLL.EmptyDt();
                if (HttpContext.Current.Session["AllFESubItems"].ToString() != "")
                    DtSubItems = (System.Data.DataTable)HttpContext.Current.Session["AllFESubItems"];
                else
                    DtSubItems = CommonBLL.FEEmpty_SubItems();
                int res = NEBLL.NewEnquiryInsert(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now,
                    "", Guid.Empty, 0, "", "", Guid.Empty, new Guid(Session["UserID"].ToString()), false, new Guid(Session["CompanyID"].ToString()), dt, DtSubItems);
                if (res == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('Row Deleted Successfully.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "NewEnquiry", "Row Deleted successfully.");
                }
                else if (res != 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Deleting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", "Error while Deleting.");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is to get genereal ID
        /// </summary>
        private void GetGeneralID()
        {
            try
            {
                DataSet ds = new DataSet();
                EnumMasterBLL EMBLL = new EnumMasterBLL();
                ds = EMBLL.EnumMasterSelect(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Session["GeneralCtgryID"] = ds.Tables[0].Rows[0][0].ToString();
                    GeneralCtgryID = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "NewEnquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Empty Table for using select statement...
        /// </summary>
        /// <returns></returns>
        private DataTable EmptyDt()
        {
            return CommonBLL.EmptyDt();
        }

        /// <summary>
        /// This is used to Upload Files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileUploadComplete(object sender, EventArgs e)
        {
            try
            {
                if (AsyncFileUpload1.HasFile)
                {
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["uploads"] != null)
                        {
                            alist = (ArrayList)Session["uploads"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["uploads"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["uploads"] = alist;
                        AsyncFileUpload1.SaveAs(strPath + FileNames);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('File Size is more than 25MB, Resize and Try Again');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Contact Master", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Bind Attachments
        /// </summary>
        /// <returns></returns>
        private string AttachedFiles()
        {
            try
            {
                if (Session["uploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["uploads"];
                    StringBuilder sb = new StringBuilder();
                    if (all.Count > 0)
                    {
                        sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
                        for (int k = 0; k < all.Count; k++)
                            sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
                        sb.Append("</select>");
                    }
                    return sb.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// This is used to Fill Item Details in Grid To Add
        /// </summary>
        /// <param name="ADD"></param>
        /// <returns></returns>
        private string FillItemGrid(bool ADD)
        {
            try
            {
                int RowNo = 1;
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataSet ds2 = null; DataRow[] drarray = null; DataRow[] drarrayolditems = null;
                if (Session["ItemUnits"] == null)
                {
                    Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
                }
                DataSet dsUnits = (DataSet)Session["ItemUnits"];
                StringBuilder sb = new StringBuilder();
                if (!ADD)
                {
                    sb.Append("");
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' " + " id='tblItems'>");
                    sb.Append("<thead align='left'>");
                    sb.Append("<tr><th></th><th>SNo</th><th>Item Description</th><th></th><th>Part No</th><th>Specification</th>" +
                        "<th>Make</th><th>Quantity</th><th>Units</th><th></th><th></th></tr>");
                    sb.Append("</thead><tbody>");
                }

                #region Body

                if (dt.Rows.Count > 0)
                {
                    RowNo = dt.Rows.Count + 1;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ds2 = null;
                        sb.Append("<tr>");
                        string sno = (i + 1).ToString();
                        string ExpandImg = "";
                        if (Convert.ToBoolean(dt.Rows[i]["IsSubItems"].ToString()) == true)
                        {
                            ExpandImg = "<span class='gridactionicons'><a id='btnExpand" + sno + "' href='javascript:void(0)' " +
                                                " onclick='ExpandSubItems(" + sno + ")' title='Expand'><img id='" + sno +
                                                "' src='../images/expand.png' style='border-style: none; height: 18px; width: 18px;'/></a></span>";

                        }
                        sb.Append("<td align='center'> " + ExpandImg + " </td>");
                        sb.Append("<td>" + (i + 1) + "</td>");
                        dt.Rows[i]["SNo"] = (i + 1);
                        sb.Append("<td>");

                        DataSet ds = (DataSet)Session["ItemsDescPrtNo"];
                        DataSet Olditemsds = (DataSet)Session["OldItemsDescPrtNo"];
                        if (dt.Rows.Count > 0)
                        {
                            drarray = ds.Tables[0].Select("ID=" + "'" + dt.Rows[i]["ItemDescription"] + "'");
                            if (drarray.Count<DataRow>() > 0)
                            {
                                sb.Append("<textarea id='txtItemDesc" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' "
                                    + "onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                                    + drarray[0][1].ToString() + " </textarea>");
                                sb.Append("<input type='hidden' id='hfItemID" + (i + 1) + "' value='" + drarray[0][0].ToString() + "'/>");
                            }
                            //if(Olditemsds!= null)
                            else
                            {
                                drarrayolditems = Olditemsds.Tables[0].Select("ID=" + "'" + dt.Rows[i]["ItemDescription"] + "'");
                                if (drarrayolditems.Count<DataRow>() > 0)
                                {
                                    sb.Append("<textarea id='txtItemDesc" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' "
                                        + "onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                                        + drarrayolditems[0][1].ToString() + " </textarea>");
                                    sb.Append("<input type='hidden' id='hfItemID" + (i + 1) + "' value='" + drarrayolditems[0][0].ToString() + "'/>");
                                }
                            }
                        }
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<textarea id='txtPartNo" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)'"
                            + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                            + (drarray.Length > 0 ? drarray[0][2].ToString() : "") + " </textarea>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<textarea id='txtSpec" + (i + 1) + "' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)'"
                            + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;' readonly='readonly'>"
                            + (drarray.Length > 0 ? drarray[0][3].ToString() : "") + " </textarea>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblMake" + (i + 1) + "'  >" + dt.Rows[i]["Make"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        sb.Append("<label id='lblQuantity" + (i + 1) + "'  >" + dt.Rows[i]["Quantity"].ToString() + "</label>");
                        sb.Append("</td>");

                        sb.Append("<td>");
                        DataRow[] dr = dsUnits.Tables[0].Select("ID=" + "'" + dt.Rows[i]["Units"].ToString().ToLower() + "'");
                        if (dr.Length > 0)
                        {
                            sb.Append("<label id='lblUnit" + (i + 1) + "'  >" + dr[0]["Description"].ToString() + "</label>");
                        }
                        sb.Append("</td>");

                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnDel" + (i + 1) + "' href='javascript:void(0)' " +
                            " onclick='javascript:return doConfirm(" + i.ToString() + ")' class='icons deleteicon' title='Delete'>" +
                            " <img src='../images/Delete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");

                        sb.Append("<td align='right'>");
                        sb.Append("<span class='gridactionicons'><a id='btnEdit" + (i + 1) + "' href='javascript:void(0)' " +
                            " onclick='javascript:return EditSelectedItem(" + i.ToString() + ")' class='icons deleteicon' title='Edit'>" +
                            " <img src='../images/Edit.jpeg' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                        if (Convert.ToBoolean(dt.Rows[i]["IsSubItems"].ToString()) == true && EditID == 1)
                            FillGrid_SupItems(drarray[0][0].ToString(), dt.Rows[i]["SNo"].ToString(), false, Convert.ToBoolean(EditID));
                    }

                    dt.AcceptChanges();
                }
                #endregion

                if (!ADD)
                {
                    sb.Append("</tbody>");
                    sb.Append("<tfoot>");
                    sb.Append("<tr>");
                    sb.Append("<th></th>");
                    sb.Append("<th><label id='lblEditID0'></label></th>");
                    sb.Append("<th>");
                    #region Fill Items Dropdown
                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                    DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];

                    //dss.Tables[0].AsEnumerable().Where(C => Codes.Select(DD=>DD.Value).ToList()
                    //    .Contains(C.Field<Guid>("ID"))).ToList().ForEach(PT => dss.Tables[0].Rows.Remove(PT));

                    //sb.Append("<select id='ddl0' Class='bcAspdropdown' width='50px' onchange='FillSpec_ItemDesc(0)'>");
                    //sb.Append("<option value='00000000-0000-0000-0000-000000000000'>-SELECT-</option>");
                    //int countRow = 0;

                    //foreach (DataRow row in dss.Tables[0].Rows)
                    //{
                    //    Guid ItemID = Guid.Empty;
                    //    if (!Codes.ContainsValue(new Guid(row["ID"].ToString())) && new Guid(row["ID"].ToString()) != ItemID)
                    //        sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["ItemDescription"].ToString() +
                    //            "</option>");
                    //    countRow++;
                    //    if (countRow == 50)
                    //        break;
                    //}

                    //sb.Append("</select>");
                    sb.Append("<input type='text' id='Txt_IM' style='height:20px; width:150px; resize:none;'>");
                    sb.Append("</th><th>");
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Enquiries/AddItems.aspx"))
                    {
                        sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' class='icons additionalrow' "
                            + " ID='btnShow'  title='Add Item to Item Master' onclick='fnOpen(0)'>"
                            + " <img src='../images/AddNW.jpeg' style='border-style: none;'/></a></span>");
                    }

                    #endregion
                    sb.Append("</th>");
                    sb.Append("<th><input type='text' id='txtPartNo0' class='bcAsptextboxmulti_Footer' readonly='readonly' style='height:20px; width:150px; resize:none;'></th>");
                    sb.Append("<th><textarea id='txtSpec0' class='bcAsptextboxmulti_Footer' onfocus='ExpandTXT(this.id)' readonly='readonly'"
                                + " onblur='ReSizeTXT(this.id)' style='height:20px; width:150px; resize:none;'></textarea></th>");
                    sb.Append("<th><input type='text' id='txtMake0' class='bcAsptextboxmulti_Footer' style='height:20px; width:150px; resize:none;'></th>");
                    sb.Append("<th><input type='text' id='txtQuantity0' onblur='extractNumber(this,2,false);' "
                        + "onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' class='bcAsptextboxmulti_Footer'></th>");
                    sb.Append("<th>");
                    #region Fill Units Dropdown
                    sb.Append("<select id='ddlUnits0' class='bcAspdropdown' style='width:85px;'>");

                    sb.Append("<option value='0'>-SELECT-</option>");

                    foreach (DataRow row in dsUnits.Tables[0].Rows)
                    {
                        if (row["Description"].ToString().ToUpper() == "NO(S)")
                            sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                        else
                            sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                    }
                    sb.Append("</select>");
                    #endregion
                    sb.Append("</th>");
                    sb.Append("<th>");

                    sb.Append("<span class='gridactionicons'><a id='btnaddItem' href='javascript:void(0)'" +
                                        " onclick='AddItemRow(0)'class='icons additionalrow' title='Add Row' style='display:display;'>"
                                        + "<img src='../images/add.jpeg' style='border-style: none;'/></a></span>");
                    sb.Append("<span class='gridactionicons'><a id='btnEditItem' href='javascript:void(0)' " +
                                                " onclick='UpdateSelectedItem()' class='icons deleteicon' style='display:none;'" +
                                                " title='Edit selected item' >" +
                                                " <img src='../images/Edit.jpeg' style='border-style: none;'/></a></span>");//OnClientClick='javascript:return doConfirm();'
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("<span class='gridactionicons'><a id='btnCancel' href='javascript:void(0)' " +
                                        " onclick='CancelEdit()'class='icons additionalrow' title='Cancel' style='display:none;'>"
                                        + "<img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                    sb.Append("</th></tr>");
                    sb.Append("</tfoot></table>");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ex.Message;
            }
        }

        /// <summary>
        /// Send E-Mails After Generation of Enquiry 
        /// </summary>
        protected void SendDefaultMails(DataSet ToAdrsTbl, string Type)
        {
            try
            {
                string ToAddrs, CcAddrs = "";
                if (Type == "Customer")
                {
                    ToAddrs = (!String.IsNullOrEmpty(ToAdrsTbl.Tables[0].Rows[0]["ASSGINEDUSER"].ToString()) ?
                ToAdrsTbl.Tables[0].Rows[0]["ASSGINEDUSER"].ToString() : ToAdrsTbl.Tables[0].Rows[0]["MNGRMAIL"].ToString());
                    if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                    {
                        CcAddrs = Session["UserMail"].ToString();
                    }
                    else
                    {
                        CcAddrs = Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                    }
                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtls());
                }
                else if (Type == "Volta")
                {
                    ToAddrs = ToAdrsTbl.Tables[0].Rows[0]["CMPNYMAIL"].ToString();
                    if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                    {
                        CcAddrs = Session["UserMail"].ToString();
                    }
                    else
                    {
                        CcAddrs = Session["TLMailID"].ToString() + ", " + Session["UserMail"].ToString();
                    }
                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtls("Volta"));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about Enquiry Details
        /// </summary>
        /// <returns></returns>
        protected string InformationEnqDtls()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Quotaton Required for Enquiry " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append(" We have an urgent requirement for Enquiry No. " + txtenqno.Text + " Dt: " + txtenqdt.Text + " . ");
                sb.Append("Please find the Enquiry in VOMS Application for the complete details and send your most competitive quotation for the same. ");
                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Note: This is system generated e-mail, please do not reply this.");
                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        /// <summary>
        /// Mail Body Content 
        /// </summary>
        /// <param name="volta"></param>
        /// <returns></returns>
        protected string InformationEnqDtls(string volta)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append("SUB: Process Started for your Enquiry " + txtenqno.Text + System.Environment.NewLine + System.Environment.NewLine);
                sb.Append(" We have started processing your urgent requirement, Ref: Enquiry No. " + txtenqno.Text + " Dt: " + txtenqdt.Text + " . ");
                sb.Append("Please find the Enquiry in VOMS Application for complete details and send your comments for the same. " + System.Environment.NewLine);
                sb.Append(System.Environment.NewLine + "Note: This is system generated e-mail, please do not reply this.");
                sb.Append(System.Environment.NewLine + System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + Session["UserName"].ToString());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        public DataSet ReadExcelData()
        {
            try
            {
                string FilePath = "";
                if (FileUpload1.HasFile)
                {
                    string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

                    FilePath = MapPath("~/uploads/" + FileName); //Server.MapPath(FolderPath + FileName);
                    FileUpload1.SaveAs(FilePath);
                }
                string sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                OleDbConnection dbCon = new OleDbConnection(sConnection);
                dbCon.Open();
                DataTable dtSheetName = dbCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataSet dsOutput = new DataSet();
                //for (int nCount = 0; nCount < dtSheetName.Rows.Count; nCount++)
                if (dtSheetName.Rows.Count > 0)
                {
                    int nCount = 0;
                    string sSheetName = dtSheetName.Rows[nCount]["TABLE_NAME"].ToString();
                    string sQuery = "Select * From [" + sSheetName + "]";
                    OleDbCommand dbCmd = new OleDbCommand(sQuery, dbCon);
                    OleDbDataAdapter dbDa = new OleDbDataAdapter(dbCmd);
                    DataTable dtData = new DataTable(); //DataTable dtData_Clone = new DataTable();
                    dtData.TableName = sSheetName;
                    dbDa.Fill(dtData);
                    //dtData_Clone = dtData.Copy();
                    //foreach (DataRow value in dtData.Rows)
                    //{

                    //    if (string.IsNullOrEmpty(value.ItemArray[1].ToString()) && string.IsNullOrEmpty(value.ItemArray[5].ToString())
                    //        && string.IsNullOrEmpty(value.ItemArray[6].ToString()))
                    //    {
                    //        int Index = dtData.Rows.IndexOf(value);
                    //        dtData_Clone.Rows.RemoveAt(Index);
                    //        dtData_Clone.AcceptChanges();
                    //    }

                    //}
                    //dtData = dtData_Clone.Copy();                    
                    dsOutput.Tables.Add(dtData);
                }
                dbCon.Close();
                return dsOutput;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());

                return null;
            }
        }

        /// <summary>
        /// Get File Name
        /// </summary>
        /// <returns></returns>
        private string FileName()
        {
            string Url = Request.Url.AbsolutePath;
            Uri uri = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            string filename = "";
            filename = Path.GetFileName(uri.AbsolutePath);
            return filename;
        }

        #region SubItems

        private void AddColumns_SubItems(Boolean IsEdit)
        {
            try
            {
                DataTable dt = (DataTable)Session["AllFESubItems"];
                if (dt == null)
                    dt = CommonBLL.FEEmpty_SubItems();

                if (!dt.Columns.Contains("Specifications"))
                    dt.Columns.Add("Specifications", typeof(string));

                dt.AcceptChanges();
                Session["AllFESubItems"] = dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New SubItem", ex.Message.ToString());
            }
        }


        private string FillGrid_SupItems(string ParentItemID, string TRid, bool IsAdd, bool IsEdit)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string RowID = TRid;
                double LableID = Convert.ToDouble(TRid);
                DataTable dt = (DataTable)Session["AllFESubItems"];
                if (dt == null)
                    dt = CommonBLL.FEEmpty_SubItems();
                DataRow[] rslt = dt.Select("ParentItemId = '" + ParentItemID + "'");
                if (rslt.Length == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["ItemId"] = Guid.Empty;
                    dr["ParentItemId"] = ParentItemID;
                    dr["Quantity"] = 0;
                    dr["UNumsId"] = Guid.Empty;
                    dt.Rows.Add(dr);
                    rslt = dt.Select("ParentItemId = '" + ParentItemID + "'");
                }
                if (rslt != null && rslt.Length > 0)
                {

                    for (int i = 0; i < rslt.Length; i++)
                    {
                        if (IsEdit == true)
                            rslt = EditDS.Tables[3].Select("ParentItemId = '" + ParentItemID + "'");
                        string SNo = (i + 1).ToString();
                        RowID = (TRid + "a" + (i + 1));
                        LableID = (LableID + 0.1);
                        sb.Append("<tr id='" + RowID + "' class='DEL" + TRid + " RA'>");
                        sb.Append("<td ></td>");
                        sb.Append("<td ><input type ='hidden' id='hfSubItemID" + RowID + "' value='" + rslt[i]["ItemId"].ToString() + "'>" +
                            "<lable id='lblSubSNo" + RowID + "'>" + LableID + "</td>");
                        rslt[i]["SNo"] = LableID;
                        sb.Append("<td ><lable id='lblItemDesc" + RowID + "'>" + rslt[i]["SpecDes"].ToString() + "");
                        if (i == rslt.Length - 1)
                        {
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' "
                                    + " class='icons fnOpen'  ID='btnShow" + RowID + "'  title='Add Item to Item Master'><img src='../images/add.jpeg'/></a></span>");
                        }
                        sb.Append("</td>");
                        sb.Append("<td ></td>");
                        sb.Append("<td ><lable id='lblPartNo" + RowID + "'>" + rslt[i]["PartNumber"].ToString() + "</td>");
                        sb.Append("<td ><textarea id='txtDesc-Spec" + RowID + "' class='bcAsptextboxmulti' onchange='savechanges1(" + TRid + "," + SNo
                            + ",this); ' onMouseUp='return false' " + " onfocus='ExpandTXTt(" + SNo + "); this.select()' onblur='ReSizeTXTt(" + SNo
                            + ")' style='height:20px; width:150px; resize:none;' >" + rslt[i]["Specifications"].ToString() + "" + "</textarea></td>");

                        sb.Append("<td ><input type='text' id='txtMake" + RowID + "' onchange='savechanges1(" + TRid + "," + SNo + ",this);' class='bcAsptextbox' style='width:50px;' value='" + rslt[i]["Make"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");
                        sb.Append("<td ><input type='text' id='txtQuantity" + RowID + "' onkeypress='return blockNonNumbers(this, event, true, false);' onchange='savechanges1(" + TRid + "," + SNo + ",this); ' class='bcAsptextbox' style='width:50px;text-align: right;' value='" + rslt[i]["Quantity"].ToString() + "' onfocus='this.select()' onMouseUp='return false'/></td>");

                        #region Fill Units Dropdown
                        sb.Append("<td align='right'>");
                        sb.Append("<select id='ddlUnits" + RowID + "' class='bcAspdropdown' style='width:85px;' onchange='savechanges1(" + TRid + "," + SNo + ",this);'>");
                        sb.Append("<option value='00000000-0000-0000-0000-000000000000'>-SELECT-</option>");
                        if (Session["ItemUnits"] == null)
                            Session["ItemUnits"] = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);
                        DataSet dsUnits = (DataSet)Session["ItemUnits"];
                        foreach (DataRow row in dsUnits.Tables[0].Rows)
                        {
                            if (rslt[i]["UnumsID"].ToString() == row["ID"].ToString())
                            {
                                sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                                    row["Description"].ToString() + "</option>");
                            }
                            else
                                sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                        }
                        sb.Append("</select>");
                        sb.Append("</td>");

                        #endregion

                        sb.Append("</td>");

                        #region Buttons

                        sb.Append("<td valign='top'>");
                        if (rslt.Length - 1 == i)
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' " +
                                " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td valign='top'>" +
                                "<a href='javascript:void(0)' onclick='Add_Sub_Itms1(" + SNo + ")' " +
                                " class='icons additionalrow addrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></td>");
                        else
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return Delete_SubItem(" + TRid + "," + SNo + ", this)' class='icons deleteicon' " +
                                " title='Delete' OnClientClick='javascript:return doConfirm();'>" +
                                " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span></td><td>");
                        sb.Append("</td>");
                        #endregion
                        sb.Append("</tr>");

                    }
                }
                dt.AcceptChanges();
                Session["AllSubItems"] = dt;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New LQ", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        #endregion

        # endregion

        # region DDl Events

        /// <summary>
        /// This is Used to Fill controlls using previous FE ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlRefForeignEnqNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlRefForeignEnqNo.SelectedValue != Guid.Empty.ToString())
                {
                    RefFlag = 1;
                    EditDS = new DataSet();
                    NewFPOBLL NFPOBLL = new NewFPOBLL();
                    EditDS = NFPOBLL.Select(CommonBLL.FlagModify, new Guid(ddlRefForeignEnqNo.SelectedValue), "", Guid.Empty, Guid.Empty, Guid.Empty,
                        DateTime.Now, Guid.Empty.ToString(), DateTime.Now, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, "", "", Guid.Empty, "",
                        DateTime.Now, 0, 0, 0, Guid.Empty, false, false, false, "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                            new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtNewFPOForCheckList(), CommonBLL.FirstRowPaymentTerms(),
                            CommonBLL.ATConditions(), new Guid(Session["CompanyID"].ToString()));
                    if (EditDS.Tables.Count >= 3 && EditDS.Tables[0].Rows.Count > 0 && EditDS.Tables[1].Rows.Count > 0
                        && EditDS.Tables[2].Rows.Count > 0)
                    {
                        ddlcustmr.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                        fillEditGrid();
                    }
                    ddlRefForeignEnqNo.Enabled = true;
                    //CHkShow.Enabled = false;
                }
                else
                {
                    ddlRefForeignEnqNo.Enabled = true;
                    ddlcustmr.Enabled = true;
                    CHkShow.Enabled = true;
                    //CHkShow.Checked = false;
                    ClearAll();
                    GetData();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        # endregion

        #region Check Box Checked Changed Event

        /// <summary>
        /// Is Repeat Order Check Box Checked Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CHkShow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CHkShow.Checked)
                {
                    ddlRefForeignEnqNo.Enabled = true;
                    BindReferenceDDL();
                }
                else
                {
                    ddlRefForeignEnqNo.SelectedIndex = -1;
                    ddlRefForeignEnqNo.Enabled = false;
                    ClearAll();
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        #endregion

        # region ButtonClicks

        /// <summary>
        /// This is used to add another attachment..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbtnAtchmts_Click(object sender, EventArgs e)
        {
            try
            {
                FileUpload fup = new FileUpload();
                fup.ID = "flupld";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This button is used to call save record method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtenqdt.Text.Trim() != "" || txtrecvdt.Text.Trim() != "" || txtduedt.Text.Trim() != ""
                || ddlcustmr.SelectedIndex != 0 || ddldept.SelectedIndex != 0)
            {
                SaveRecord();
                if (res == 0)
                {
                    Response.Redirect("FEStatus.Aspx", false);
                }
            }

            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('Please Fill All Details Properly.');", true);
        }

        /// <summary>
        /// This is used to clear all the Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnclear_Click(object sender, EventArgs e)
        {
            ClearAll();
            divFEItems.InnerHtml = FillGridItems(Guid.Empty);
            Response.Redirect("NewEnquiry.aspx", false);
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ItemCode"] = null;
                DataSet ds = ReadExcelData();

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Contains("Items$"))
                    {
                        DataSet dsUnits = embal.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                        DataTable dtCloned = ds.Tables["Items$"].Clone();
                        dtCloned.Columns["Sno"].DataType = typeof(Int32);
                        dtCloned.Columns["Item Description"].DataType = typeof(string);
                        dtCloned.Columns["Part No"].DataType = typeof(string);
                        dtCloned.Columns["Specification"].DataType = typeof(string);
                        dtCloned.Columns["Make"].DataType = typeof(string);
                        dtCloned.Columns["Quantity"].DataType = typeof(decimal);
                        dtCloned.Columns["Units"].DataType = typeof(string);
                        //Column Added
                        dtCloned.Columns.Add("UnumsID", typeof(Guid));
                        dtCloned.Columns.Add("ItemId", typeof(Guid));
                        DataColumn dc = new DataColumn("IsSubItems", typeof(bool));
                        dc.DefaultValue = false;
                        dtCloned.Columns.Add(dc);

                        DataColumn dc1 = new DataColumn("Category", typeof(Guid));
                        dc1.DefaultValue = new Guid(Session["GeneralCtgryID"].ToString());
                        dtCloned.Columns.Add(dc1);

                        DataColumn dc2 = new DataColumn("ID", typeof(Guid));
                        dc2.DefaultValue = Guid.Empty;
                        dtCloned.Columns.Add(dc2);

                        Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                        int RowId = 0;
                        Guid ItemCatagory = Guid.Empty;
                        Guid ItemID = Guid.Empty;
                        foreach (DataRow row in ds.Tables["Items$"].Rows)
                        {
                            dtCloned.ImportRow(row);
                            string Desc = row["Item Description"].ToString();
                            string PartNo = row["Part No"].ToString();
                            string Spec = row["Specification"].ToString();
                            string Make = row["Make"].ToString();

                            DataSet dss = embal.EnumMasterSelectforDescription(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory);
                            if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            {
                                ItemCatagory = new Guid(dss.Tables[0].Rows[0][0].ToString());

                                #region Inserting Item
                                string itemCode = "";
                                if (Desc.Trim().Length > 8)
                                    itemCode = (ItemCatagory + (Desc.Trim()).Substring(0, 8));
                                else
                                    itemCode = (ItemCatagory + (Desc.Trim()));

                                // check whether Item Exists or Not
                                DataSet ItmExsts = ItmMstr.InsertUpdateItemMasterWithID(CommonBLL.FlagYSelect, Guid.Empty, ItemCatagory, "",
                                    Desc, Spec, PartNo, itemCode, new Guid(Session["UserID"].ToString()), "", false, new Guid(Session["CompanyID"].ToString()));
                                if (ItmExsts != null && ItmExsts.Tables.Count > 0 && ItmExsts.Tables[0].Rows.Count > 0)
                                {
                                    Codes.Add(RowId, new Guid(ItmExsts.Tables[0].Rows[0][0].ToString()));
                                    ItemID = new Guid(ItmExsts.Tables[0].Rows[0][0].ToString());
                                }
                                else
                                {
                                    DataSet result = ItmMstr.InsertUpdateItemMasterWithID(CommonBLL.FlagNewInsert, Guid.Empty, ItemCatagory, "",
                                        Desc, Spec, PartNo, itemCode, new Guid(Session["UserID"].ToString()), "", false, new Guid(Session["CompanyID"].ToString()));
                                    if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                                    {
                                        Codes.Add(RowId, new Guid(result.Tables[0].Rows[0][0].ToString()));
                                        ItemID = new Guid(result.Tables[0].Rows[0][0].ToString());
                                    }
                                }
                                string U = row["Units"].ToString();
                                if (dsUnits != null && dsUnits.Tables.Count > 0 && dsUnits.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] units = dsUnits.Tables[0].Select("Description = '" + row["Units"].ToString() + "'");
                                    if (units.Length == 0)
                                        units = dsUnits.Tables[0].Select("Description = 'No(s)'");
                                    if (units.Length > 0)
                                        dtCloned.Rows[RowId]["UnumsID"] = new Guid(units[0]["ID"].ToString());
                                }
                                dtCloned.Rows[RowId]["ItemId"] = ItemID;
                                #endregion

                                RowId++;
                            }
                        }
                        if (Codes != null && Codes.Count > 0)
                        {
                            HttpContext.Current.Session["SelectedItems"] = Codes;
                            Session["HFITemsValues"] = string.Join(",", Codes.ToArray().Select(o => o.Value.ToString().Trim()).ToArray()).ToString();
                        }
                        if (dtCloned.Columns.Contains("ItemId"))
                            dtCloned.Columns["ItemId"].ColumnName = "ItemDescription";
                        if (dtCloned.Columns.Contains("Part No"))
                            dtCloned.Columns["Part No"].ColumnName = "PartNo";
                        if (dtCloned.Columns.Contains("Units"))
                            dtCloned.Columns["Units"].ColumnName = "UnitsDesc";
                        if (dtCloned.Columns.Contains("UnumsID"))
                            dtCloned.Columns["UnumsID"].ColumnName = "Units";

                        DataTable dt =
                            dtCloned.DefaultView.ToTable(false, "SNo", "Category", "ItemDescription", "PartNo", "Specification", "Make", "Quantity", "Units", "ID", "IsSubItems");
                        Session["ItemCode"] = dt;
                        HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        divFEItems.InnerHtml = FillItemGrid(false);

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
            }
        }

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", "fillinbox()", true);
                //System.Threading.Thread.Sleep(1000);
                string filename = FileName();
                DataTable dssss = new DataTable();
                if (Session["ItemCode"] != null)
                {
                    dssss = (DataTable)Session["ItemCode"];
                }
                DataTable Itm = dssss.Clone();
                DataSet ds = ReadExcelData();
                if (ds.Tables[0].Columns.Count == 7)
                {
                    for (int i = 1; i <= 7; i++)
                    {
                        ds.Tables[0].Columns.Add(new DataColumn());
                        ds.Tables[0].Columns[6 + i].SetOrdinal(i);
                    }
                }
                Dictionary<Int64, Guid> Codes = new Dictionary<Int64, Guid>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Contains("Items$"))
                    {
                        DataTable _DT = ds.Tables["Items$"].Clone();
                        _DT.Columns["Part No"].DataType = typeof(String);
                        _DT.Load(ds.Tables["Items$"].CreateDataReader());
                        if (_DT.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0)).Count() > 0)
                            _DT = _DT.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull || string.Compare((field as string), string.Empty) == 0)).CopyToDataTable();
                        DataSet dsRtnItms = ItmMstr.InsertUpdateItemMasterDataTable(_DT, new Guid(Session["CompanyId"].ToString()),
                            new Guid(Session["UserId"].ToString()), filename, txtenqno.Text);
                        if (dsRtnItms != null && dsRtnItms.Tables.Count > 0)
                        {
                            try
                            {
                                string _Msg = "";
                                if (dsRtnItms.Tables[0] != null && dsRtnItms.Tables[0].Rows.Count > 0)
                                {
                                    if (dsRtnItms.Tables[0].Columns.Count == 3 && dsRtnItms.Tables[0].Columns[0].ColumnName == "DuplicateCodes")
                                    {
                                        //if (dsRtnItms.Tables[0].Rows[0]["Sno"].ToString() != "")
                                        //    _Msg += string.Format(@"Incorrect Format: " + dsRtnItms.Tables[0].Rows[0]["InvalidFormat"].ToString());
                                        if (dsRtnItms.Tables[0].Rows[0]["InvalidFormat"].ToString() != "")
                                            _Msg += string.Format(@"Incorrect Format:\r\n" + dsRtnItms.Tables[0].Rows[0]["InvalidFormat"].ToString());
                                        if (dsRtnItms.Tables[0].Rows[0]["DuplicateCodes"].ToString() != "")
                                            _Msg += string.Format(@"File Contains Duplicate Codes:\r\n" + dsRtnItms.Tables[0].Rows[0]["DuplicateCodes"].ToString());
                                        if (dsRtnItms.Tables[0].Rows[0]["ItemCodesMisMatch"].ToString() != "")
                                            _Msg += string.Format(@"Either ItemCode or Description,Specification,Partno are Mismatched with Existing Data: \r\n" + dsRtnItms.Tables[0].Rows[0]["ItemCodesMisMatch"].ToString());
                                        //_Msg += string.Format(@"Duplicate/Incorrect Format : " + string.Join(@",\r\n", dsRtnItms.Tables[0].AsEnumerable().Select(P => P.Field<Int32>("Sno")+" " + P.Field<string>("ItemDescription")).Distinct().ToList()) + " ");
                                        //_Msg += string.Format(@"\r\nFile Contains Duplicate Codes: " + string.Join(@",\r\n", dsRtnItms.Tables[0].AsEnumerable().Select(P => P.Field<String>("DuplicateCodes")).Distinct().ToList()) + " ");
                                    }
                                    else if (dsRtnItms.Tables[0].Columns.Count == 10 && dsRtnItms.Tables[0].Columns[0].ColumnName == "Sno")
                                    {
                                        //Itm = dsRtnItms.Tables[0];
                                        Itm = dsRtnItms.Tables[dsRtnItms.Tables.Count - 1];
                                        //Itm.Merge(dssss, true, MissingSchemaAction.Ignore);
                                        //Itm.Merge(dssss, false, MissingSchemaAction.Add);
                                        //dssss.Merge(Itm);
                                        DataTable dt = Itm.DefaultView.ToTable(true);
                                        Itm = dt;
                                    }
                                    if (_Msg != "")
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + _Msg + "');", true);
                                }
                            }
                            catch (Exception Ex)
                            {
                                if (Ex.Message.Contains("does not belong to table"))
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + dsRtnItms.Tables[dsRtnItms.Tables.Count - 1].Rows[0]["ErrorMessage"].ToString().Replace("'", "") + "');", true);
                                else
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "alert('" + Ex.Message.Replace("'", "") + "');", true);
                            }
                        }

                        Session["ItemCode"] = Itm;
                        HttpContext.Current.Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        divFEItems.InnerHtml = FillItemGrid(false);

                        //var Dic = Itm.AsEnumerable().ToDictionary(row => row.Field<Int64>(0), row => row.Field<Guid>(2));
                        //Codes = Dic;
                        //if (dsRtnItms != null && dsRtnItms.Tables.Count > 0)
                        //{
                        //    ELog.CreateErrorLog(Server.MapPath("../Logs/BulkUpload/ErrorLog"), "New Foreign Enquiry Bulk Upload", dsRtnItms.Tables[1].Rows[0][0].ToString().Trim(','));
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('these New Item Codes " + dsRtnItms.Tables[1].Rows[0][0].ToString() + " are unable to Insert');", true);
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAlertMessage", "alert('these New Item Codes \r\n" + dsRtnItms.Tables[1].Rows[0][0].ToString().Trim(',') + " \r\nare unable to Insert');", true);
                        //}
                        //else
                        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry Bulk Upload", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMethods

        /// <summary>
        /// To Fill Item Grid view
        /// </summary>
        /// <param name="RowID">Selected Row No</param>
        /// <param name="SNo">row S.No.</param>
        /// <param name="CodeID">Item ID</param>
        /// <param name="ID"></param>
        /// <param name="CatID">Category ID</param>
        /// <param name="UnitID">Unit Id</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string FillItemGrid(int RowID, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID, Boolean IsSubItem)
        {
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataRow dr = dt.NewRow();
                dr["SNo"] = RowID;
                dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                dr["ItemDescription"] = ItemID;
                dr["PartNo"] = PartNo;
                dr["Specification"] = Spec;
                dr["Make"] = Make;
                dr["Quantity"] = Qty;
                dr["Units"] = UnitID;
                dr["ID"] = Guid.Empty;
                dr["IsSubItems"] = IsSubItem;
                dt.Rows.Add(dr);
                dt.AcceptChanges();
                HttpContext.Current.Session["ItemCode"] = dt;
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                int CodeCount = 0;
                if (Codes.Count > 0)
                    CodeCount = Codes.Keys.Max();
                CodeCount += 1;
                if (!Codes.ContainsValue(new Guid(ItemID)) && ItemID != Guid.Empty.ToString())
                    Codes.Add(CodeCount, new Guid(ItemID));

                Session["SelectedItems"] = Codes;
                return FillItemGrid(true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// Delete Item Table Item
        /// </summary>
        /// <param name="ItemID">Selected Row</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItem(string ItemID)
        {
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                DataRow[] dr = dt.Select("ItemDescription = " + "'" + ItemID + "'");
                if (dr.Length > 0)
                    dr[0].Delete();
                dt.AcceptChanges();
                Codes.Where(P => P.Value.ToString() == ItemID).ToList().ForEach(CC => Codes.Remove(CC.Key));
                HttpContext.Current.Session["SelectedItems"] = Codes;
                HttpContext.Current.Session["ItemCode"] = dt;

                DataTable dtt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataSet dss = new DataSet();
                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                return FillItemGrid(false);//0, dtt, Codes, dss, Convert.ToInt32(GeneralCtgryID), CatCodes, 0, UnitCodes, ""
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + ", Line No : " + LineNo;
            }
        }

        /// <summary>
        /// Add new row in a Item grid
        /// </summary>
        /// <param name="rowNo">Selected Row No</param>
        /// <param name="SNo"></param>
        /// <param name="CodeID">Item ID</param>
        /// <returns>Returns HTML Items Table</returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemColumn(int rowNo, int SNo, int CodeID)
        {
            try
            {
                if (rowNo == 0 && SNo == 0 && CodeID == 0)
                {
                    return FillGridItems(Guid.Empty);
                }
                else
                {
                    DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                    DataRow dr = dt.NewRow();
                    dr["SNo"] = Convert.ToInt32(dt.Rows[rowNo - 1]["SNo"]) + 1;// rowNo + 1;
                    dr["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dr["ItemDescription"] = 0;
                    dr["PartNo"] = string.Empty;
                    dr["Specification"] = string.Empty;
                    dr["Make"] = string.Empty;
                    dr["Quantity"] = 0;
                    dr["Units"] = 0;
                    dr["ID"] = 0;
                    dt.Rows.Add(dr);

                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];

                    HttpContext.Current.Session["SelectedItems"] = Codes;
                    HttpContext.Current.Session["ItemCode"] = dt;
                    int count = dt.Rows.Count;
                    return FillItemGrid(false);//rowNo, SNo, CodeID, 1, 0, 0, "", "", "", "", ""
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }
        /// <summary>
        /// This is used to Fill Item Description
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string FillSpec_ItemDesc(string ItemID)
        {
            try
            {
                string Rslt = "";
                Guid ItemIDInt = new Guid(ItemID.Trim());
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] result = dss.Tables[0].Select("ID = " + "'" + ItemIDInt + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt = dr["PartNumber"].ToString() + " &@&" + dr["Specification"].ToString() + " &@& " + Convert.ToBoolean(dr["IsSubItems"]) + " &@& " + dr["Itemdescription"].ToString();
                    }
                }
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Edit the selected Item Row 
        /// </summary>
        /// <param name="SelID"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string EditItemRow(string SelID, string ItemID)
        {
            try
            {
                string Rslt = "";
                DataTable dtt = (DataTable)HttpContext.Current.Session["ItemCode"];
                DataSet dss = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
                DataSet oldItemsds = (DataSet)HttpContext.Current.Session["OldItemsDescPrtNo"];
                if (dtt != null && dtt.Rows.Count > 0 && dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dsRow = dss.Tables[0].Select("ID = " + "'" + ItemID + "'");
                    if (dsRow.Count() > 0)
                    {
                        foreach (DataRow drr in dsRow)
                        {
                            Rslt = drr["ID"].ToString() + " &@&" + drr["ItemDescription"].ToString() + " &@&";
                        }
                    }
                    else
                    {
                        DataRow[] dsRows = oldItemsds.Tables[0].Select("ID = " + "'" + ItemID + "'");
                        foreach (DataRow drr in dsRows)
                        {
                            Rslt = drr["ID"].ToString() + " &@&" + drr["ItemDescription"].ToString() + " &@&";
                        }
                    }

                    DataRow[] result = dtt.Select("ItemDescription = " + "'" + ItemID + "'");
                    foreach (DataRow dr in result)
                    {
                        Rslt += dr["PartNo"].ToString() + " &@&"
                              + dr["Specification"].ToString() + " &@&"
                              + dr["Make"].ToString() + " &@&"
                              + dr["Quantity"].ToString() + " &@&"
                              + dr["Units"].ToString();
                    }
                }
                return Rslt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to update the Items Selected
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ItemID"></param>
        /// <param name="PartNo"></param>
        /// <param name="Spec"></param>
        /// <param name="Make"></param>
        /// <param name="Qty"></param>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string UpdateSelectedItem(string value, string ItemID, string PartNo, string Spec, string Make, string Qty, string UnitID)
        {
            try
            {
                string Rslt = "";
                DataTable dt = (DataTable)HttpContext.Current.Session["ItemCode"];
                string[] val = value.Split(',');
                if (dt != null && dt.Rows.Count > 0 && val.Length > 0)
                {
                    int rowNo = Convert.ToInt32(val[0]) - 1;
                    dt.Rows[rowNo]["Category"] = new Guid(Session["GeneralCtgryID"].ToString());
                    dt.Rows[rowNo]["ItemDescription"] = ItemID;
                    dt.Rows[rowNo]["PartNo"] = PartNo;
                    dt.Rows[rowNo]["Specification"] = Spec;
                    dt.Rows[rowNo]["Make"] = Make;
                    if (Qty == "")
                        dt.Rows[rowNo]["Quantity"] = 0;
                    else
                        dt.Rows[rowNo]["Quantity"] = Convert.ToDecimal(Qty);
                    dt.Rows[rowNo]["units"] = UnitID;
                    dt.AcceptChanges();
                    Session["ItemCode"] = dt;
                }
                var _Res = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                if (_Res != null)
                {
                    _Res[Convert.ToInt32(val[0])] = new Guid(ItemID);
                }
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Add New Item 
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string NewItemAdded()
        {
            try
            {
                string Rslt = "";
                Session["ItemsDescPrtNo"] = ItemMstBLl.SelectItemMaster(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                return FillItemGrid(false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        /// <summary>
        /// This is used to Delete Uploaded Files/ Attachments
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["uploads"];
                if (all.Count > 0)
                    all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return ErrMsg + " Line No : " + LineNo;
            }
        }

        /// <summary>
        /// This is used to Check the Enquiry Number
        /// </summary>
        /// <param name="EnqNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckEnquiryNo(string EnqNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckEnquiryNo('E', EnqNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }


        /// <summary>
        /// This is used to Check the Item Status 
        /// </summary>
        /// <param name="itmId"></param>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public bool CheckStat(string itmId, int rowNo)
        {
            try
            {
                if (EditID != 0)
                {
                    if (Session["checkstat"] != null)
                    {
                        DataTable checkstat = (DataTable)Session["checkstat"];
                        DataRow[] drow = checkstat.Select("ItemId = " + "'" + itmId + "'");
                        int Cnt = drow.Count();
                        if (Cnt > 0)
                            return (drow[0]["ChkState"].ToString() == "true" ? true : false);
                        else
                            return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return false;
            }
        }

        #region SubItems

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Expand_FEs(string ParentItemId, string TRid)
        {
            try
            {
                bool EDit = false;
                AddColumns_SubItems(false);
                if (EditID != 0)
                    EDit = true;
                return FillGrid_SupItems(ParentItemId, TRid, false, EDit);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetItemDesc_Spec(string ParentItemId, string TRid)
        {
            try
            {
                string desc = "", Spec = "", PNo = "";
                DataSet ds = ItemMstBLl.SelectItemMaster(CommonBLL.FlagASelect, new Guid(ParentItemId.Trim()), Guid.Empty, new Guid(Session["CompanyID"].ToString()));//, CommonBLL.SupplierCategory
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    desc = ds.Tables[0].Rows[0]["ItemDescription"].ToString();
                    Spec = ds.Tables[0].Rows[0]["Specification"].ToString();
                    PNo = ds.Tables[0].Rows[0]["PartNumber"].ToString();
                }
                return desc + "^~^," + PNo + "^~^," + Spec;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message;
            }
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Add_Sub_Itms(string RowID, string TRid, string SubRowID, string ParentItemId, string ItemId,
            string ItmDesc, string Spec, string PNo, string Make, string Qty, string UnitID, string IsAdd)
        {
            try
            {
                DataTable dt = (DataTable)Session["AllFESubItems"];
                if (dt != null && ItemId.Trim() != "")
                {
                    Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)Session["SelectedItems"];
                    if (Codes == null)
                        Codes = new Dictionary<int, Guid>();
                    Guid SelItemID = new Guid(ItemId);
                    if (Codes == null || !Codes.ContainsValue(SelItemID))
                    {
                        Codes.Add(Convert.ToInt32((Codes.Last().Key) + 1), SelItemID);
                        Session["SelectedItems"] = Codes;
                        Session["HFITemsValues"] = SelItemID;
                    }

                    DataRow[] result = dt.Select("SNo = '" + SubRowID + "'");
                    if (result.Length > 0)
                    {
                        if (ParentItemId.Trim() != "" && ParentItemId.Trim() != "0")
                            result[0]["ParentItemId"] = new Guid(ParentItemId);
                        if (ItemId.Trim() != "" && ItemId.Trim() != Guid.Empty.ToString())
                            result[0]["ItemId"] = new Guid(ItemId);
                        result[0]["SpecDes"] = ItmDesc.Trim();
                        result[0]["Make"] = Make.Trim();
                        result[0]["PartNumber"] = PNo.Trim();
                        result[0]["Specifications"] = Spec.Trim();
                        if (Qty.Trim() != "")
                            result[0]["Quantity"] = Convert.ToDecimal(Qty);
                        if (UnitID.Trim() != "" || UnitID.Trim() != "00000000-0000-0000-0000-000000000000" && UnitID.Trim() != Guid.Empty.ToString())
                            result[0]["UnumsID"] = new Guid(UnitID);

                        if (Convert.ToBoolean(IsAdd))
                        {
                            DataRow dr = dt.NewRow();
                            dr["SNo"] = (Convert.ToDouble(SubRowID) + 0.1);
                            dr["ItemId"] = Guid.Empty;
                            dr["ParentItemId"] = ParentItemId;
                            dr["Quantity"] = 0;
                            dr["UNumsId"] = Guid.Empty;
                            dt.Rows.Add(dr);
                        }
                    }
                    dt.AcceptChanges();
                    Session["AllFESubItems"] = dt;
                }
                else
                    dt = CommonBLL.FEEmpty_SubItems();

                return FillGrid_SupItems(ParentItemId, TRid, Convert.ToBoolean(IsAdd), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "LQ", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string Delete_SubItem(string ParentItemId, string TRid, string SNo, string ItemID)
        {
            try
            {
                string subItmID = TRid + "." + SNo;
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)Session["SelectedItems"];
                if (Codes == null)
                    Codes = new Dictionary<int, Guid>();

                if (ItemID != "" && Codes != null && Codes.ContainsValue(new Guid(ItemID)))
                {
                    int KeyVal = Codes.FirstOrDefault(x => x.Value == new Guid(ItemID)).Key;
                    Codes.Remove(KeyVal);
                    Session["SelectedItems"] = Codes;
                    Session["HFITemsValues"] = ItemID;
                }

                DataTable dt = (DataTable)Session["AllFESubItems"];
                DataRow[] result = dt.Select("SNo = '" + subItmID + "'");
                if (result.Length > 0)
                    result[0].Delete();

                dt.AcceptChanges();
                Session["AllFESubItems"] = dt;
                return FillGrid_SupItems(ParentItemId, TRid, false, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
                return ex.Message + ", line No : " + LineNo;
            }
        }

        #endregion

        [WebMethod]
        /// <summary>
        /// To Retrive Auto Complete Data From DataBase 
        /// </summary>
        /// <param name="Txt_Text"></param>
        /// <returns></returns>
        public static List<string> GetAutoCompleteData(string Txt_Text)
        {
            try
            {
                var _SelectedItems = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                return ((DataSet)HttpContext.Current.Session["ItemsDescPrtNo"]).Tables[0].AsEnumerable()
                                      .Where(P => P.Field<string>("ItemDescription").ToLower().StartsWith(Txt_Text.ToLower()) &&
                                          !_SelectedItems.Select(TT => TT.Value.ToString()).Contains(P.Field<Guid>("ID").ToString()))
                                      .Select(C => C.Field<string>("ItemDescription") + " VAL--VAL--VAL " + C.Field<Guid>("ID").ToString())
                                      .OrderBy(O => O).Distinct().ToList();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ErrorLog ELog = new ErrorLog();
                ELog.CreateErrorLog(HttpContext.Current.Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// This is used to Check the Item Status 
        /// </summary>
        /// <param name="itmId"></param>
        /// <param name="rowNo"></param>
        /// <returns></returns>
        //[Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        //public void UpdateSeletedItem(string itmId)
        //{
        //    try
        //    {
        //        DataSet DS = (DataSet)HttpContext.Current.Session["ItemsDescPrtNo"];
        //        //DS.Tables[0].AsEnumerable().Where(C => C.Field<Guid>("ID") == new Guid(itmId)).ToList().ForEach(CC => DS.Tables[0].Rows.Remove(CC));
        //        Session["ItemsDescPrtNo"] = DS;
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Foreign Enquiry", ex.Message.ToString());
        //    }
        //}


        # endregion
    }
}
