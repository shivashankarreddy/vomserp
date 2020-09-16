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
using System.Collections.Generic;
using BAL;
using Ajax;
using System.Text;
using System.IO;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Customer_Access
{
    public partial class Float_PI : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        CustomerBLL CustBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        ItemDetailsBLL IDBLL = new ItemDetailsBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        EMailsDetailsBLL EMDBL = new EMailsDetailsBLL();
        NewFQuotationBLL NFBLL = new NewFQuotationBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        static Guid GenSupID;
        static DataSet dss = new DataSet();
        static DataSet dsEnm = new DataSet();
        static DataSet EditDS;
        static Guid FEenquiryID;
        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    //if (CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString())
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations(); Deselect();");
                        btnsavenew.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        txtfedt.Attributes.Add("readonly", "readonly");
                        txtfeduedt.Text = DateTime.Now.ToString("dd-MM-yyyy");
                        txtfeduedt.Attributes.Add("readonly", "readonly");
                        txtfenqdt.Attributes.Add("readonly", "readonly");
                        Ajax.Utility.RegisterTypeForAjax(typeof(Float_PI));
                        if (!IsPostBack)
                        {
                            ClearAll();
                            GetData();

                            #region Add/Update Permission Code

                            if ((string[])Session["UsrPermissions"] != null &&
                               ((string[])Session["UsrPermissions"]).Contains("Edit") && Request.QueryString["ID"] != null)
                            {
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")// ::VARA No Need this line, can place above//&& Request.QueryString["ID"] != ""
                                {
                                    DivComments.Visible = true;
                                    EditRecord(new Guid(Request.QueryString["ID"].ToString()));
                                }
                            }
                            else if ((string[])Session["UsrPermissions"] != null && ((string[])Session["UsrPermissions"]).Contains("New"))
                            {
                                btnSave.Enabled = true;
                                btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no", false);

                            #endregion
                        }
                        Atta();
                    }
                    else
                        Response.Redirect("../Masters/CHome.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Float Enquiry Vendor", ex.Message.ToString());
            }
        }

        # region DDL SelectedIndexes

        /// <summary>
        /// Customer DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomerSelectionChanged();
        }

        private void CustomerSelectionChanged()
        {
            try
            {
                string custmrval = ddlcustmr.SelectedValue;
                ClearAll();
                ddlcustmr.SelectedValue = custmrval;
                GetFEnqNo_Cstmrs(new Guid(ddlcustmr.SelectedValue));
                divGridItems.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Frn Enquiry Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfenqy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlfenqy.SelectedIndex != 0)
                {
                    // GetReleasedLPOItems(new Guid(ddlfenqy.SelectedValue));
                    GetVendorNames(new Guid(ddlfenqy.SelectedValue), new Guid(ddlcustmr.SelectedValue));
                    GetForeignVendorNames(new Guid(ddlfenqy.SelectedValue), new Guid(ddlcustmr.SelectedValue));
                    GetLocalVendorNames(new Guid(ddlfenqy.SelectedValue), new Guid(ddlcustmr.SelectedValue));
                    divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                    FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                }
                else
                {
                    string cstmrval = ddlcustmr.SelectedValue;
                    ClearAll();
                    ddlcustmr.SelectedValue = cstmrval;
                    lbvendors.Items.Clear();
                    divGridItems.InnerHtml = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        // /// <summary>
        /// Supplier Category Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void ddlsuplrctgry_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        GetSupplierNames(new Guid(ddlsuplrctgry.SelectedValue));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Local Enquiry", ex.Message.ToString());
        //    }
        //}

        # endregion

        # region Methods
        /// <summary>
        /// This is used to download Attached Files
        /// </summary>
        /// <param name="fname"></param>
        private void DownloadFile(string fname)
        {
            try
            {
                string filepath = MapPath("~/") + "uploads/" + fname;//"@D://publishnew-12-12-12//uploads//";//
                FileInfo file = new FileInfo(filepath);
                if (file.Exists)
                {
                    Response.ClearContent();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.TransmitFile(file.FullName);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Calling Methods to Bind Default Data
        /// </summary>
        private void GetData()
        {
            GetCustomers();
            //GetVendorNames();
            GetDepartments();
            HselectedItems.Value = "";
            txtenqno.Enabled = false;
            if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
            {
                ddlcustmr.SelectedValue = Request.QueryString["CsID"].ToString();
                GetFEnqNo_Cstmrs(new Guid(ddlcustmr.SelectedValue));
                ddlfenqy.SelectedValue = Request.QueryString["FeqID"].ToString();
                divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                GetVendorNames(new Guid(Request.QueryString["FeqID"].ToString()), new Guid(Request.QueryString["CsID"].ToString()));
                GetForeignVendorNames(new Guid(ddlfenqy.SelectedValue), new Guid(ddlcustmr.SelectedValue));
                GetLocalVendorNames(new Guid(ddlfenqy.SelectedValue), new Guid(ddlcustmr.SelectedValue));
                //  GetReleasedLPOItems(new Guid(ddlfenqy.SelectedValue));
            }
            else if (ddlcustmr.Items.Count > 1)
            {
                ddlcustmr.SelectedIndex = 1;
                CustomerSelectionChanged();
            }
        }

        /// <summary>
        /// Fill Input Fields with Default Values or Previous Data
        /// </summary>
        /// <param name="FrnEnqNo"></param>
        private void FillInputFiels(Guid FrnEnqNo)
        {
            try
            {
                DataSet FrnEnqDetails = NEBLL.NewEnquiryEdit(CommonBLL.FlagModify, FrnEnqNo, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                    "", DateTime.Now, DateTime.Now, DateTime.Now, "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt());
                if (FrnEnqDetails.Tables.Count > 0 && FrnEnqDetails.Tables[0].Rows.Count > 0)
                {
                    ddlcustmr.SelectedValue = FrnEnqDetails.Tables[0].Rows[0]["CusmorId"].ToString();
                    ddlfenqy.SelectedValue = FrnEnqDetails.Tables[0].Rows[0]["ForeignEnquireId"].ToString();
                    Ddldeptnm.SelectedValue = FrnEnqDetails.Tables[0].Rows[0]["DepartmentId"].ToString();
                    txtsubject.Text = FrnEnqDetails.Tables[0].Rows[0]["Subject"].ToString();
                    txtfedt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(FrnEnqDetails.Tables[0].Rows[0]["EnquiryDate"].ToString()));
                    txtfenqdt.Text = CommonBLL.DateDisplay(DateTime.Now);
                    txtfeduedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(7));
                    string[] Attachments = (FrnEnqDetails.Tables[0].Rows[0]["Attachments"].ToString()).Split(',');
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < Attachments.Length; i++)
                        al.Add(Attachments[i]);
                    Session["Attachments"] = al;
                    Atta();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to create Dynamic buttons
        /// </summary>
        private void Atta()
        {
            try
            {
                ArrayList Attachments = (ArrayList)Session["Attachments"];
                if (Attachments != null && Attachments.Count > 0)
                {
                    if (Attachments[0].ToString() != "")
                    {
                        pnlattachemnts.Controls.Clear();
                        pnlDelAttachments.Controls.Clear();
                        for (int i = 0; i < Attachments.Count; i++)
                        {
                            if (Attachments[i].ToString() != "")
                                imgAtchmt.Visible = true;
                            else
                                imgAtchmt.Visible = false;

                            ImageButton btnDel = new ImageButton();
                            btnDel.ID = "btnDelAtchmnts" + i;
                            btnDel.Click += new ImageClickEventHandler(btnDel_Click);
                            btnDel.ImageUrl = "../images/d1.png";
                            btnDel.CssClass = "bcTdButtonDelAttachments";
                            btnDel.ToolTip = Attachments[i].ToString();
                            pnlDelAttachments.Controls.Add(btnDel);
                            Label lbl1 = new Label();
                            lbl1.ID = "lbldell" + i;
                            lbl1.Text = "<br/>";
                            pnlDelAttachments.Controls.Add(lbl1);
                            btnDel.Visible = true;

                            LinkButton lb = new LinkButton();
                            lb.ID = "lbtnAtchmnt" + i;
                            lb.Click += new EventHandler(lb_Click);
                            lb.Text = Attachments[i].ToString();
                            pnlattachemnts.Controls.Add(lb);
                            Label lbl = new Label();
                            lbl.ID = "lbl" + i;
                            lbl.Text = "<br/>";
                            pnlattachemnts.Controls.Add(lbl);
                            lb.Visible = true;
                        }
                    }
                    else
                    {
                        pnlattachemnts.Controls.Clear();
                        pnlDelAttachments.Controls.Clear();
                        imgAtchmt.Visible = false;
                        Label lbl = new Label();
                        lbl.ID = "lblNoAtchs";
                        lbl.Text = "No Attachments are Available";
                        pnlattachemnts.Controls.Add(lbl);
                    }
                }
                else
                {
                    pnlattachemnts.Controls.Clear();
                    pnlDelAttachments.Controls.Clear();
                    imgAtchmt.Visible = false;
                    Label lbl = new Label();
                    lbl.ID = "lblNoAtchs";
                    lbl.Text = "No Attachments are Available";
                    pnlattachemnts.Controls.Add(lbl);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        //private void GetReleasedLPOItems(Guid FEID)
        //{
        //    try
        //    {
        //        FEenquiryID = new Guid(ddlfenqy.SelectedValue);
        //        FEnquiryBLL FEBLL = new LEnquiryBLL();
        //        DataSet ds = new DataSet();
        //        ds = FEBLL.SelctLocalEnquiriesVendor(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty, FEID, "", "", Guid.Empty, DateTime.Now, DateTime.Now,
        //            DateTime.Now, Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.StatusTypeLPOrder, "", "", "", Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal());
        //        //ViewState["FEIDS"] = 
        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            Session["ReleasedLPOitems"] = ds.Tables[0].Rows[0][0].ToString();
        //        else
        //            Session["ReleasedLPOitems"] = "-9";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Vendor", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Get Customers
        /// </summary>
        private void GetCustomers()
        {
            try
            {
                ddlcustmr.Items.Clear();
                DataSet ds = new DataSet();
                ds = CustBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                //if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                //ds = CustBLL.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlcustmr.DataSource = ds;//ds.Tables[0] is required, at some times we got errors by assigning ds for binding ::VARA
                    ddlcustmr.DataTextField = "Description";
                    ddlcustmr.DataValueField = "ID";
                    ddlcustmr.DataBind();
                }
                ddlcustmr.Items.Insert(0, new ListItem("--Select Customer--", "0"));
                ddlcustmr.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Departments
        /// </summary>
        private void GetDepartments()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Departments);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Ddldeptnm.DataSource = ds;//ds.Tables[0] is required, at some times we got errors by assigning ds for binding ::VARA
                    Ddldeptnm.DataTextField = "Description";
                    Ddldeptnm.DataValueField = "ID";
                    Ddldeptnm.DataBind();
                }
                Ddldeptnm.Items.Insert(0, new ListItem("--Select Department--", "0"));
                Ddldeptnm.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Get Frn Enquiry using Customers
        /// </summary>
        /// <param name="CID"></param>
        private void GetFEnqNo_Cstmrs(Guid CID)
        {
            try
            {
                ddlfenqy.Items.Clear();
                DataSet ds = new DataSet();
                ds = NEBLL.NewEnquiryEdit(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, CID, Guid.Empty, "", DateTime.Now, "", "",
                    DateTime.Now, DateTime.Now, DateTime.Now, "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt());

                if (Request.QueryString["ID"] != null)
                {
                    if (Request.QueryString["ID"].ToString() != null && Request.QueryString["ID"].ToString() != "")// ::VARA No Need this line, can place above If Condition
                    {
                        ds = NEBLL.NewEnquiryEdit(CommonBLL.FlagJSelect, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, CID, Guid.Empty, "", DateTime.Now, "", "",
                        DateTime.Now, DateTime.Now, DateTime.Now, "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt());
                    }
                }
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlfenqy.DataSource = ds;//ds.Tables[0] is required, at some times we got errors by assigning ds for binding ::VARA
                    ddlfenqy.DataTextField = "Description";
                    ddlfenqy.DataValueField = "ID";
                    ddlfenqy.DataBind();
                }
                ddlfenqy.Items.Insert(0, new ListItem("--Select EnquiryNo--", Guid.Empty.ToString()));
                ddlfenqy.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        ///// <summary>
        ///// This is used to get Supplier Catagories
        ///// </summary>
        //private void GetSupplierCtgries()
        //{
        //    try
        //    {
        //        ddlsuplrctgry.Items.Clear();
        //        EnumMasterBLL EMBLL = new EnumMasterBLL();
        //        DataSet ds = new DataSet();
        //        ds = EMBLL.EnumMasterSelectforDescription(Convert.ToChar("X"), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "");
        //        Session["GenSupID"] = ds.Tables[0].Rows[0][0].ToString();
        //        GenSupID = new Guid(ds.Tables[0].Rows[0][0].ToString());
        //        ddlsuplrctgry.Items.Insert(0, new ListItem("General", Session["GenSupID"].ToString()));
        //        ddlsuplrctgry.SelectedIndex = 0;
        //        GetSupplierNames(new Guid(ddlfenqy.SelectedValue));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Local Enquiry", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// Bind Suppliers
        /// </summary>
        /// <param name="SCID"></param>
        private void GetVendorNames(Guid FEID, Guid CustId)
        {
            try
            {
                lbvendors.Items.Clear();
                SupplierBLL SBLL = new SupplierBLL();
                DataSet ds = new DataSet();
                ds = SBLL.GetVendorsByFe(CommonBLL.FlagSelectAll, CustId, FEID, "", Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].DefaultView.Sort = "BussName ASC";
                    lbvendors.DataSource = ds.Tables[0];
                    lbvendors.DataTextField = "BussName";
                    lbvendors.DataValueField = "ID";
                    lbvendors.DataBind();
                    //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                    //FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('No Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());

            }
        }


        /// <summary>
        /// Bind Foreign Suppliers
        /// </summary>
        /// <param name="SCID"></param>
        private void GetForeignVendorNames(Guid FEID, Guid CustId)
        {
            try
            {
                LblForeignVendor.Items.Clear();
                SupplierBLL SBLL = new SupplierBLL();
                DataSet ds = new DataSet();
                ds = SBLL.GetSuppliersByFe(CommonBLL.FlagFSelect, GenSupID, FEID, new Guid(Session["CompanyID"].ToString()));
                //ds = SBLL.GetVendorsByFe(CommonBLL.FlagSelectAll, CustId, FEID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {


                    ds.Tables[0].DefaultView.Sort = "BussName ASC";
                    LblForeignVendor.DataSource = ds.Tables[0];
                    LblForeignVendor.DataTextField = "BussName";
                    LblForeignVendor.DataValueField = "ID";
                    LblForeignVendor.DataBind();
                    //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                    //FillInputFiels(new Guid(ddlfenqy.SelectedValue));

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('No Foreign Suppliers Are Mapped For The Customer');", true);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());

            }
        }

        /// <summary>
        /// Bind Local Suppliers
        /// </summary>
        /// <param name="SCID"></param>
        private void GetLocalVendorNames(Guid FEID, Guid CustId)
        {
            try
            {
                LbllocalVendor.Items.Clear();
                SupplierBLL SBLL = new SupplierBLL();
                DataSet ds = new DataSet();
                ds = SBLL.GetSuppliersByFe(CommonBLL.FlagLSelect, GenSupID, FEID, new Guid(Session["CompanyID"].ToString()));
                //ds = SBLL.GetVendorsByFe(CommonBLL.FlagSelectAll, CustId, FEID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {


                    ds.Tables[0].DefaultView.Sort = "BussName ASC";
                    LbllocalVendor.DataSource = ds.Tables[0];
                    LbllocalVendor.DataTextField = "BussName";
                    LbllocalVendor.DataValueField = "ID";
                    LbllocalVendor.DataBind();
                    //divGridItems.InnerHtml = FillGridView(new Guid(ddlfenqy.SelectedValue));
                    //FillInputFiels(new Guid(ddlfenqy.SelectedValue));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('No Local Vendors Are Mapped For The Customer in Mapping Screen');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());

            }
        }

        /// <summary>
        /// Remove Columns
        /// </summary>
        /// <param name="dss"></param>
        /// <returns></returns>
        private DataSet RemoveColumns(DataSet dss)
        {
            try
            {
                if (dss.Tables[0].Columns.Contains("ItemDetailsID"))
                    dss.Tables[0].Columns.Remove("ItemDetailsID");
                if (dss.Tables[0].Columns.Contains("ForeignEnquireId"))
                    dss.Tables[0].Columns.Remove("ForeignEnquireId");
                if (dss.Tables[0].Columns.Contains("ForeignQuotationId"))
                    dss.Tables[0].Columns.Remove("ForeignQuotationId");
                if (dss.Tables[0].Columns.Contains("ForeignPOId"))
                    dss.Tables[0].Columns.Remove("ForeignPOId");
                if (dss.Tables[0].Columns.Contains("LocalQuotationId"))
                    dss.Tables[0].Columns.Remove("LocalQuotationId");
                if (dss.Tables[0].Columns.Contains("LocalPOId"))
                    dss.Tables[0].Columns.Remove("LocalPOId");
                if (dss.Tables[0].Columns.Contains("ExDutyPercentage"))
                    dss.Tables[0].Columns.Remove("ExDutyPercentage");
                if (dss.Tables[0].Columns.Contains("Rate"))
                    dss.Tables[0].Columns.Remove("Rate");
                if (dss.Tables[0].Columns.Contains("Amount"))
                    dss.Tables[0].Columns.Remove("Amount");
                if (dss.Tables[0].Columns.Contains("DiscountPercentage"))
                    dss.Tables[0].Columns.Remove("DiscountPercentage");
                if (dss.Tables[0].Columns.Contains("CreatedBy"))
                    dss.Tables[0].Columns.Remove("CreatedBy");
                if (dss.Tables[0].Columns.Contains("CreatedDate"))
                    dss.Tables[0].Columns.Remove("CreatedDate");
                if (dss.Tables[0].Columns.Contains("ModifiedBy"))
                    dss.Tables[0].Columns.Remove("ModifiedBy");
                if (dss.Tables[0].Columns.Contains("ModifiedDate"))
                    dss.Tables[0].Columns.Remove("ModifiedDate");
                if (dss.Tables[0].Columns.Contains("IsActive"))
                    dss.Tables[0].Columns.Remove("IsActive");

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Vendor", ex.Message.ToString());
            }
            return dss;
        }

        /// <summary>
        /// Save Record
        /// </summary>
        private void SaveRecord()
        {
            try
            {
                string Atchmnts = "";
                Filename = FileName();
                if (Session["Attachments"] == null)
                    Atchmnts = "";
                else
                {
                    ArrayList al = (ArrayList)Session["Attachments"];
                    for (int i = 0; i < al.Count; i++)
                        Atchmnts += al[i].ToString() + ",";
                }
                int res = 1;
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                DataTable dt = ds.Tables[0].Copy();
                if (dt.Columns.Contains("ItemDesc"))
                    dt.Columns.Remove("ItemDesc");
                if (dt.Columns.Contains("UnitName"))
                    dt.Columns.Remove("UnitName");
                if (dt.Columns.Contains("QPrice"))
                    dt.Columns.Remove("QPrice");
                if (dt.Columns.Contains("RoundOff"))
                    dt.Columns.Remove("RoundOff");
                if (dt.Columns.Contains("CompanyId"))
                    dt.Columns.Remove("CompanyId");
                if (dt.Columns.Contains("Currency"))
                    dt.Columns.Remove("Currency");
                string SelectedVendor = "";
                foreach (ListItem item in lbvendors.Items)
                {
                    if (item.Selected)
                        SelectedVendor += item.Value + ",";
                }
                if (SelectedVendor.EndsWith(","))
                    SelectedVendor = SelectedVendor.Remove(SelectedVendor.Length - 1, 1);
                if (lbvendors.GetSelectedIndices().Count() == 0)
                {
                    SelectedVendor = Guid.Empty.ToString();
                }

                string FSupp = String.Join(",", LblForeignVendor.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                FSupp = FSupp == "" || FSupp == null ? Guid.Empty.ToString() : FSupp;
                string LSupp = String.Join(",", LbllocalVendor.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                LSupp = LSupp == "" || LSupp == null ? Guid.Empty.ToString() : LSupp;
                Guid CustID = new Guid(ddlcustmr.SelectedValue);
                string FEnqryID = ddlfenqy.SelectedItem.Text;
                Guid DeptNo = new Guid(Ddldeptnm.SelectedValue);
                NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                if (dt.Rows.Count > 0)
                {
                    DataSet Rslt = null;
                    if (btnSave.Text == "Save")
                    {
                        Rslt = NEBLL.NewVendorEnquiryInsert(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlfenqy.SelectedValue), CustID, DeptNo, FEnqryID,
                           txtsubject.Text.Trim(), CommonBLL.DateInsert(txtfedt.Text), CommonBLL.DateInsert(txtfenqdt.Text),
                           CommonBLL.DateInsert(txtfeduedt.Text), txtimpinst.Text, SelectedVendor, FSupp, LSupp,
                           CommonBLL.StatusTypeFrnEnqID, Atchmnts.Trim(','), "", new Guid(Session["UserID"].ToString()),
                           Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), dt);
                    }
                    else if (btnSave.Text == "Update")
                    {

                        string LEN = EditDS.Tables[0].Rows[0]["EnquireNumber"].ToString();
                        res = NEBLL.NewVendorEnquiryUpdate(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), Guid.Empty, CustID,
                            DeptNo, LEN, txtsubject.Text.Trim(), CommonBLL.DateInsert(txtfedt.Text),
                            CommonBLL.DateInsert(txtfenqdt.Text), CommonBLL.DateInsert(txtfeduedt.Text), new Guid(SelectedVendor), FSupp, LSupp,
                            CommonBLL.StatusTypeFrnEnqID, Atchmnts.Trim(','), txtComments.Text.Trim(),
                            new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()),
                            true, new Guid(Session["CompanyID"].ToString()), dt);
                    }
                    if (btnSave.Text == "Save" && Rslt != null)
                    {
                        if (Rslt != null && Rslt.Tables.Count > 1 && Rslt.Tables[1].Rows.Count > 0)
                        {
                            //foreach (DataRow drow in Rslt.Tables[1].Rows)
                            //{
                            //    SendDefaultMails(EMDBL.SelectEMailDetails(CommonBLL.FlagWCommonMstr, new Guid(drow["ID"].ToString()), Guid.Empty, "", "", "",
                            //        DateTime.Now, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                            //}
                            ALS.AuditLog(0, btnSave.Text, ddlfenqy.SelectedItem.Text, "Customer Floated Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "Floated Foreign Enquiry Vendor by Customer", "Data inserted successfully.");
                            ClearAll();
                            Response.Redirect("Float_PIStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(-1, btnSave.Text, ddlfenqy.SelectedItem.Text, "Customer Floated Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Inserting.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", "Error while Inserting.");
                            Session["LEVendors"] = Session["TempDS"];
                            divGridItems.InnerHtml = FillGridView(Guid.Empty);
                        }
                    }
                    else if (Rslt == null && btnSave.Text == "Save")
                    {
                        ALS.AuditLog(-1, btnSave.Text, ddlfenqy.SelectedItem.Text, "Customer Floated Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", "Error while Inserting.");
                        Session["LEVendors"] = Session["TempDS"];
                        divGridItems.InnerHtml = FillGridView(Guid.Empty);
                    }
                    if (res == 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, ddlfenqy.SelectedItem.Text, "Customer Floated Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "SuccessMessage('Updated Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "Floated Foreign Enquiry Vendor by Customer", "Data Updated successfully.");
                        Response.Redirect("Float_PIStatus.aspx", false);
                    }
                    else if (res != 0 && btnSave.Text == "Update")
                    {
                        ALS.AuditLog(res, btnSave.Text, ddlfenqy.SelectedItem.Text, "Customer Floated Foreign Enquiry No:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", "Error while Updating.");
                        Session["LEVendors"] = Session["TempDS"];
                        divGridItems.InnerHtml = FillGridView(Guid.Empty);
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                    "ErrorMessage('There are No Items to Save/Update.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                Session["LEVendors"] = Session["TempDS"];
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", ex.Message.ToString());
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
                lbvendors.Enabled = false;
                DataSet dss = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagISelect, Guid.Empty, Guid.Empty, ID, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0,
                    0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    dss = RemoveColumns(dss);
                    Session["LEVendors1"] = dss;
                    DataTable dt = CommonBLL.EmptyDtLocal();
                    EditDS = new DataSet();
                    NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                    EditDS = NEBLL.NewFloatedEnquiryEdit(CommonBLL.FlagModify, ID, Guid.Empty, Guid.Empty, Guid.Empty, "", "", DateTime.Now, DateTime.Now,
                        DateTime.Now, "", Guid.Empty, "", "", 0, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), dt);
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                    {
                        DataSet FeItems = new DataSet();
                        FeItems.Tables.Add(EditDS.Tables[2].Copy());
                        FeItems = RemoveColumns(FeItems);
                        FeItems.Tables[0].Columns.Add(new DataColumn("Check", typeof(bool)));
                        Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                        for (int k = 0; k < FeItems.Tables[0].Rows.Count; k++)
                        {
                            Guid itemId = new Guid(FeItems.Tables[0].Rows[k]["ItemId"].ToString());
                            DataRow[] foundRows = EditDS.Tables[1].Select("ItemId = '" + itemId + "'");
                            Codes.Add(k, itemId);
                            if (foundRows.Length > 0)
                            {
                                FeItems.Tables[0].Rows[k]["Make"] = foundRows[0]["Make"];
                                FeItems.Tables[0].Rows[k]["Specifications"] = foundRows[0]["Specifications"];
                                FeItems.Tables[0].Rows[k]["Remarks"] = foundRows[0]["Remarks"];
                                FeItems.Tables[0].Rows[k]["Check"] = true;
                            }
                            else
                                FeItems.Tables[0].Rows[k]["Check"] = false;
                        }

                        Session["SelectedItems"] = Codes;
                        Session["LEVendors"] = FeItems;

                        txtenqno.Text = EditDS.Tables[0].Rows[0]["EnquireNumber"].ToString();

                        // string Instruc = EditDS.Tables[0].Rows[0]["Instruction"].ToString();

                        ddlcustmr.SelectedValue = EditDS.Tables[0].Rows[0]["CusmorId"].ToString();
                        GetFEnqNo_Cstmrs(new Guid(EditDS.Tables[0].Rows[0]["CusmorId"].ToString()));
                        ddlfenqy.SelectedValue = EditDS.Tables[0].Rows[0]["ParentFEnqID"].ToString();
                        Ddldeptnm.SelectedValue = EditDS.Tables[0].Rows[0]["DepartmentId"].ToString();
                        txtenqno.Text = EditDS.Tables[0].Rows[0]["EnquireNumber"].ToString();
                        txtsubject.Text = EditDS.Tables[0].Rows[0]["Subject"].ToString();
                        if (EditDS.Tables[0].Rows[0]["EnquiryDate"].ToString() != "")
                            txtfenqdt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["EnquiryDate"].ToString()));
                        if (EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString() != "")
                            txtfedt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["ReceivedDate"].ToString()));
                        if (EditDS.Tables[0].Rows[0]["DueDate"].ToString() != "")
                            txtfeduedt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(EditDS.Tables[0].Rows[0]["DueDate"].ToString()));
                        //GetReleasedLPOItems(new Guid(ddlfenqy.SelectedValue));
                        divGridItems.InnerHtml = FillGridView(Guid.Empty);

                        //GetSupplierCtgries();
                        if (EditDS.Tables[0].Rows[0]["VendorIds"].ToString() != Guid.Empty.ToString())
                        {
                            lbvendors.Items.Clear();//SupBusNm
                            ListItem li = new ListItem();
                            li.Text = EditDS.Tables[0].Rows[0]["VenBusNm"].ToString();
                            li.Value = EditDS.Tables[0].Rows[0]["VendorIds"].ToString();
                            lbvendors.Items.Add(li);
                            lbvendors.DataTextField = "VenBusNm";
                            lbvendors.DataTextField = "VendorIds";
                            lbvendors.SelectedValue = EditDS.Tables[0].Rows[0]["VendorIds"].ToString();
                            LblForeignVendor.Enabled = false;
                            LbllocalVendor.Enabled = false;
                        }
                        if (EditDS.Tables[0].Rows[0]["F_SupplierIds"].ToString() != Guid.Empty.ToString())
                        {
                            LblForeignVendor.Items.Clear();
                            ListItem Fli = new ListItem();
                            Fli.Text = EditDS.Tables[0].Rows[0]["FSupplier"].ToString();
                            Fli.Value = EditDS.Tables[0].Rows[0]["F_SupplierIds"].ToString();
                            LblForeignVendor.Items.Add(Fli);
                            LblForeignVendor.SelectedValue = EditDS.Tables[0].Rows[0]["F_SupplierIds"].ToString();
                            lbvendors.Enabled = false;
                            LbllocalVendor.Enabled = false;
                        }
                        if (EditDS.Tables[0].Rows[0]["L_SupplierIds"].ToString() != Guid.Empty.ToString())
                        {
                            LbllocalVendor.Items.Clear();
                            ListItem Lli = new ListItem();
                            Lli.Text = EditDS.Tables[0].Rows[0]["LSupplier"].ToString();
                            Lli.Value = EditDS.Tables[0].Rows[0]["L_SupplierIds"].ToString();
                            LbllocalVendor.Items.Add(Lli);
                            LbllocalVendor.SelectedValue = EditDS.Tables[0].Rows[0]["L_SupplierIds"].ToString();
                            lbvendors.Enabled = false;
                            LblForeignVendor.Enabled = false;
                        }



                        # region Attachments
                        if (EditDS.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        {
                            string[] Attachments = (EditDS.Tables[0].Rows[0]["Attachments"].ToString().Trim(',')).Split(',');
                            ArrayList al = new ArrayList();
                            for (int i = 0; i < Attachments.Length; i++)
                                al.Add(Attachments[i]);

                            Session["Attachments"] = al;
                            Atta();
                        }
                        # endregion



                        btnSave.Text = "Update";
                        ViewState["EditID"] = ID;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to delete Items of the FE
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteItemDetails(Guid ID)
        {
            //try
            //{
            //    DataTable dt = CommonBLL.EmptyDtLocal();
            //    LEnquiryBLL LEBLL = new LEnquiryBLL();
            //    int res = LEBLL.InsertUpdateDeleteLocalEnquiries(CommonBLL.FlagDelete, ID, Guid.Empty, Guid.Empty, "", "", Guid.Empty, DateTime.Now,
            //        DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, Guid.Empty, 0, "", "", "", Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()),
            //        DateTime.Now, false, new Guid(Session["CompanyID"].ToString()), dt);
            //    if (res == 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
            //            "SuccessMessage('Row Deleted Successfully.');", true);
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/Log"), "Floated Foreign Enquiry", "Row Deleted successfully.");
            //    }
            //    else if (res != 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Deleting.');", true);
            //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", "Error while Deleting.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    string linenum = ex.LineNumber().ToString();
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            //}
        }

        ///// <summary>
        ///// Bind Main Grid View
        ///// </summary>
        //private void BindMainGridView()
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        LEnquiryBLL LEBLL = new LEnquiryBLL();
        //        ds = LEBLL.SelctLocalEnquiries(CommonBLL.FlagASelect, 0, 0, 0, "", "", 0, DateTime.Now, DateTime.Now, DateTime.Now, 
        //            0, "", 0, "", "", "", Convert.ToInt64(Session["UserID"]), DateTime.Now, true, CommonBLL.EmptyDtLocal());
        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            gvLocalEnquire.DataSource = ds;
        //            gvLocalEnquire.DataBind();
        //        }
        //        else
        //            NoTable();
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());                
        //    }
        //}

        private void BeforeSave()
        {
            try
            {
                DataSet ds2 = (DataSet)Session["LEVendors"];
                DataTable TempDT = ds2.Tables[0].Copy();
                DataSet TempDs = new DataSet();
                TempDs.Tables.Add(TempDT);
                Session["TempDS"] = TempDs;
                for (int k = 0; k < ds2.Tables[0].Rows.Count; k++)
                {
                    if (!Convert.ToBoolean(ds2.Tables[0].Rows[k]["Check"]))
                        ds2.Tables[0].Rows[k].Delete();
                }
                ds2.Tables[0].AcceptChanges();
                if (ds2.Tables[0].Columns.Contains("Check"))
                    ds2.Tables[0].Columns.Remove("Check");

                ds2.Tables[0].AcceptChanges();
                if (txtfedt.Text.Trim() != "" && txtfeduedt.Text.Trim() != "" && txtfenqdt.Text.Trim() != "" && txtsubject.Text.Trim() != "")
                {
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0 && (lbvendors.GetSelectedIndices().Count() > 0 ||
                        LblForeignVendor.GetSelectedIndices().Count() > 0 || LbllocalVendor.GetSelectedIndices().Count() > 0))
                    {
                        SaveRecord();
                    }
                    else
                    {
                        if (lbvendors.GetSelectedIndices().Count() == 0)
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Select Atleast 1 Vendor.');", true);
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Fill All Details Properly.');", true);

                        Session["LEVendors"] = TempDs;
                        divGridItems.InnerHtml = FillGridView(Guid.Empty);
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Fill the Details Properly.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", ex.Message.ToString());
                Session["LEVendors"] = Session["TempDS"];
            }
        }

        ///// <summary>
        ///// This Method is used when There is no Table in DataSet
        ///// </summary>
        //private void NoTable()
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("S.No.");
        //        dt.Columns.Add("LocalEnquireId");
        //        dt.Columns.Add("CusmorId");
        //        dt.Columns.Add("DepartmentId");
        //        dt.Columns.Add("EnquireNumber");
        //        dt.Columns.Add("ForeignEnquiryDate");
        //        dt.Columns.Add("LEIssueDate");
        //        dt.Columns.Add("ResponseDueDate");
        //        ds.Tables.Add(dt);
        //        ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
        //        gvLocalEnquire.DataSource = ds;
        //        gvLocalEnquire.DataBind();
        //        int columncount = gvLocalEnquire.Rows[0].Cells.Count;
        //        gvLocalEnquire.Rows[0].Cells.Clear();
        //        gvLocalEnquire.Rows[0].Cells.Add(new TableCell());
        //        gvLocalEnquire.Rows[0].Cells[0].ColumnSpan = columncount;
        //        gvLocalEnquire.Rows[0].Cells[0].Text = "<center>No Records To Display...!</center>";
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        string linenum = ex.LineNumber().ToString();
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "New Local Enquiry", ex.Message.ToString());                
        //    }
        //}

        /// <summary>
        /// Clear All
        /// </summary>
        private void ClearAll()
        {
            try
            {
                Session["Attachments"] = null;
                txtenqno.Text = "";
                txtfedt.Text = "";
                txtsubject.Text = "";
                ddlcustmr.SelectedIndex = 0;
                Ddldeptnm.SelectedIndex = 0;
                ddlfenqy.SelectedIndex = 0;
                HselectedItems.Value = "";
                ViewState["EditID"] = null;
                lbvendors.Items.Clear();
                divGridItems.InnerHtml = "";
                pnlattachemnts.Controls.Clear();
                pnlDelAttachments.Controls.Clear();
                imgAtchmt.Visible = false;
                btnSave.Text = "Save";
                Session["CheckAllBoxes"] = true;
                Session["SelectedItems"] = null;
                Session["ReleasedLPOitems"] = null;
                txtfenqdt.Text = CommonBLL.DateDisplay(DateTime.Now);
                txtfeduedt.Text = CommonBLL.DateDisplay(DateTime.Now.AddDays(3));

                Session["CPage_PI"] = 1;
                Session["RowsDisplay_PI"] = 100;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor by Customer", ex.Message.ToString());
            }
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
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        /// <summary>
        /// Fill Grid View
        /// </summary>
        /// <param name="EnqID"></param>
        /// <returns></returns>         
        private string FillGridView(Guid EnqID)
        {
            try
            {
                DataSet dsi = new DataSet(); string ItemIDs = string.Empty;
                DataSet ds1 = new DataSet();
                string ErrMessage = "";
                if (Session["TblErrorMessage"] != null)
                    ErrMessage = Session["TblErrorMessage"].ToString();
                ds1 = (DataSet)Session["LEVendors1"];
                if (EnqID != Guid.Empty)
                {
                    dsi = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagSelectAll, Guid.Empty, Guid.Empty, EnqID, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        Guid.Empty, "", "", "", 0, 0, 0,
                        0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()));
                    dsi = RemoveColumns(dsi);
                    dsi.Tables[0].Columns.Add(new DataColumn("Check", typeof(bool)));
                    Dictionary<int, Guid> Codes = new Dictionary<int, Guid>();
                    for (int k = 0; k < dsi.Tables[0].Rows.Count; k++)
                    {
                        dsi.Tables[0].Rows[k]["Check"] = true;
                        Guid CodeID = new Guid(dsi.Tables[0].Rows[k]["ItemId"].ToString());
                        Codes.Add(k, CodeID);
                    }
                    Session["SelectedItems"] = Codes;
                    dsi.Tables[0].AcceptChanges();
                    Session["SelectedItems"] = Codes;
                    Session["LEVendors"] = dsi;
                    Session["TempFloatEnquiry"] = dsi;
                    Session["EnqID"] = EnqID;
                }
                else
                    dsi = (DataSet)Session["LEVendors"];

                dss = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(GenSupID.ToString()), new Guid(Session["CompanyID"].ToString()));
                dsEnm = EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), CommonBLL.Units);

                #region Paging

                string DisablePrevious = " disabled ", DisableNext = " disabled ";
                int Rows2Display = Convert.ToInt32(Session["RowsDisplay_PI"].ToString()), CurrentPage = Convert.ToInt32(Session["CPage_PI"].ToString()), Rows2Skip = 0;
                int RowsCount = dsi.Tables[0].Rows.Count, PageCount = 0;

                if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                {
                    if ((Convert.ToDecimal(RowsCount) / Convert.ToDecimal(Rows2Display)).ToString().Contains('.'))
                        PageCount = (RowsCount / Rows2Display) + 1;
                    else
                        PageCount = RowsCount / Rows2Display;

                    if (CurrentPage > PageCount && PageCount == 1)
                        CurrentPage = 1;
                    else if (CurrentPage > PageCount)
                        CurrentPage--;
                    else if (CurrentPage < 1)
                        CurrentPage++;
                    Rows2Skip = Rows2Display * (CurrentPage - 1);
                    if (CurrentPage == PageCount)
                        DisablePrevious = "";
                    else if (CurrentPage == 1)
                        DisableNext = "";
                    else
                    {
                        DisablePrevious = "";
                        DisableNext = "";
                    }
                    Session["CPage_PI"] = CurrentPage;
                }

                #endregion

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner' id='tblItems'" +
                    " style='font-size: small;'>" +
                    "<thead align='left'><tr>");
                if (Session["CheckAllBoxes"] == null)
                    sb.Append("<th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()'/></th>");
                else
                    sb.Append("<th class='rounded-First'><input id='ckhMain' type='checkbox' name='CheckAll' onclick='CheckAllBoxs()' " +
                        " checked='checked'/></th>");

                sb.Append("<th>SNo</th><th>Item Description</th><th>Part No</th><th>Specification</th><th>Make</th>" +
                    "<th>Quantity</th><th>Units</th><th class='rounded-Last'>Remarks</th>");
                sb.Append("</tr></thead><tbody class='bcGridViewMain'>");

                if (dsi.Tables.Count > 0 && dsi.Tables[0].Rows.Count > 0)
                {
                    #region Paging

                    DataSet ds = new DataSet();
                    DataTable dt = dsi.Tables[0].AsEnumerable().Skip(Rows2Skip).Take(Rows2Display).CopyToDataTable();
                    ds.Tables.Add(dt);

                    #endregion
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string sno = (Rows2Skip + i + 1).ToString();
                        sb.Append("<tr valign='Top'>");
                        if (Convert.ToBoolean(ds.Tables[0].Rows[i]["Check"]))
                            sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='AddItemColumn(" + sno +
                                ")' type='checkbox' checked='checked' name='CheckAll'/></td>");//checkBox
                        else
                            sb.Append("<td><input id='ckhChaild" + sno.ToString() + "' onclick='AddItemColumn(" + sno +
                                ")' type='checkbox' name='CheckAll'/></td>");//checkBox

                        sb.Append("<td>" + sno + "");//S.NO
                        sb.Append("<input type='hidden' name='HItmID' onchange='AddItemColumn(" + sno + ")' id='HItmID" + sno +
                            "' value='" + ds.Tables[0].Rows[i]["ItemId"].ToString() + "' width='5px' style='WIDTH: 5px;'/></td>");
                        if (ds.Tables[0].Rows[i]["ItemId"].ToString() == "")
                        //Item Desc 
                        {
                            sb.Append("<td><select id='ddl" + sno + "' class='PayElementCode' onchange='FillItemDRP(" + sno + ")'>");
                            if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow drr in dss.Tables[0].Rows)
                                    if (!ItemIDs.Contains(drr["ID"].ToString()))
                                    {
                                        sb.Append("<option value='" + drr["ID"].ToString() + "' >" + drr["ItemDescription"].ToString()
                                            + "</option>");
                                    }
                            }
                            sb.Append("</select>");
                            sb.Append("</td>");
                        }
                        else
                        {
                            if ((Convert.ToBoolean(Session["NewItemAdded"]) || ds.Tables[0].Rows[i]["ItemDesc"].ToString() == "")
                                && (ds.Tables[0].Rows.Count - 1) == i)
                            {

                                Guid NewitemID = new Guid(ds.Tables[0].Rows[i]["ItemId"].ToString());
                                DataRow[] selRow = dss.Tables[0].Select("ID = '" + NewitemID.ToString() + "'");
                                if (selRow.Length > 0)
                                {
                                    ds.Tables[0].Rows[i]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();
                                    sb.Append("<td><div class='expanderR'>" + ds.Tables[0].Rows[i]["ItemDesc"].ToString() +
                                        "</div></td>");//ItemDesc
                                    Session["NewItemAdded"] = false;
                                }
                            }
                            else
                            {
                                sb.Append("<td onMouseover='toolTip(" + sno + ");'><div id='mousefollow-examples1' class='expanderR'><div id='History" + sno + "' class='Htooltip'>"
                                    + ds.Tables[0].Rows[i]["ItemDesc"].ToString() + "</div></div></td>");
                                ItemIDs = ItemIDs + "," + ds.Tables[0].Rows[i]["ItemID"].ToString();
                            }

                        }
                        sb.Append("<td>" + ds.Tables[0].Rows[i]["PartNumber"].ToString() + "</td>");//PartNo

                        sb.Append("<td><textarea name='txtSpecifications' Class='bcAsptextboxmulti' onchange='AddItemColumn(" + sno +
                        ")' id='txtSpecifications" + sno + "' onfocus='ExpandTXT(" + sno + ")' onblur='ReSizeTXT(" + sno +
                        ")' style='height:20px; width:150px; resize:none;'>"
                        + ds.Tables[0].Rows[i]["Specifications"].ToString() + "</textarea></td>");
                        sb.Append("<td><input type='text' name='txtMake' onchange='AddItemColumn(" + sno + ")' id='txtMake" + sno +
                            "' value='" + ds.Tables[0].Rows[i]["Make"].ToString() + "' class='bcAsptextbox'/></td>");
                        sb.Append("<td><input  Style='text-align: right; width:50px;' type='text' readonly='true' " +
                        " name='txtQuantity' size='05px' " +
                            " onchange='AddItemColumn(" + sno + ")' id='txtQuantity" + sno + "' class='bcAsptextbox' value='" +
                            ds.Tables[0].Rows[i]["Quantity"].ToString() + "'onkeypress='return isNumberKey(event)' maxlength='6'/></td>");

                        if (ds.Tables[0].Rows[i]["UnitName"].ToString() == "")
                        {
                            sb.Append("<td><select id='ddlU" + sno + "' class='PayUnitCode' onchange='AddItemColumn(" + sno + ")'>");
                            if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                            {
                                sb.Append("<option value='0' selected='selected'>-SELECT-</option>");
                                foreach (DataRow dru in dsEnm.Tables[0].Rows)
                                    sb.Append("<option value='" + dru["ID"].ToString() + "' >" + dru["Description"].ToString() + "</option>");
                            }
                            sb.Append("</select></td>");
                        }
                        else
                            sb.Append("<td>" + ds.Tables[0].Rows[i]["UnitName"].ToString() + "</td>");
                        sb.Append("<td><input type='text' name='txtRemarks' onchange='AddItemColumn(" + sno + ")' id='txtRemarks"
                            + sno + "' value='" + ds.Tables[0].Rows[i]["Remarks"].ToString() + "' class='bcAsptextbox'/></td>");
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody>");
                sb.Append("<tfoot><th class='rounded-foot-left'></th><th colspan='6'>");

                #region Paging

                string disply = "<option value='" + Rows2Display + "'>" + Rows2Display + "</option>";
                string disply1 = "<option selected value='" + Rows2Display + "'>" + Rows2Display + "</option>";

                StringBuilder ss = new StringBuilder();
                ss.Append("<select id='ddlRowsChanged' onchange='RowsChanged()'>"
                        + "<option value='25'>25</option>"
                        + "<option value='50'>50</option>"
                        + "<option value='100'>100</option>"
                        + "<option value='200'>200</option>"
                    + "</select>");
                ss.Replace(disply, disply1);

                sb.Append("RowsCount:" + RowsCount + ",&nbsp; No.of Pages : " + PageCount + ",&nbsp; CurrentPage:" + CurrentPage + ""
                    + "<input type='hidden' id='hfCurrentPage' value='" + CurrentPage + "' /> ,&nbsp;Rows to Display :"

                    //+ ",&nbsp;Rows to Display : <input type='text' id='txtRowsChanged' Style='text-align: right; width:50px;' onkeypress='return isNumberKey(event)' "
                    //+ "maxlength='3' onchange='RowsChanged()' value='" + Rows2Display + "'/>"
                    + ss

                    + "<input " + DisablePrevious + " type='button' id='btnPrevious' value='Previous' onclick='PrevPage()' style='width:70px'/>"
                    + "<input " + DisableNext + " type='button' id='btnNext' value='Next' onclick='NextPage()'  style='width:70px' /></th>");

                #endregion

                sb.Append("<th style='height:17px;'><input type='hidden' name='TblErrorMessage' id='TblErrorMessage' value='"
                + ErrMessage + "' /></th>");
                sb.Append("<th class='rounded-foot-right'></th></tfoot></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
                return ErrMsg;
            }
        }


        /// <summary>
        /// Send E-Mails After Generation of Enquiry 
        /// </summary>
        protected void SendDefaultMails(DataSet ToAdrsTbl)
        {
            try
            {
                if (ToAdrsTbl != null && ToAdrsTbl.Tables.Count > 1)
                {
                    string ToAddrs = ToAdrsTbl.Tables[1].Rows[0]["LogInMailID"].ToString() + ", " + ToAdrsTbl.Tables[0].Rows[0]["Email"].ToString();
                    string CcAddrs = "";
                    string Atchmnts = "";
                    if (ToAdrsTbl.Tables[0].Columns.Contains("Attachments"))
                    {
                        if (ToAdrsTbl.Tables[0].Rows[0]["Attachments"].ToString() != "")
                        {
                            Atchmnts = ToAdrsTbl.Tables[0].Rows[0]["Attachments"].ToString();
                        }
                    }

                    string Rslt1 = CommonBLL.SendMails(ToAddrs, CcAddrs.Replace(",,", ","), txtsubject.Text, InformationEnqDtls(ToAdrsTbl), Atchmnts);
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
        protected string InformationEnqDtls(DataSet Details)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string SupID = "", CustID = "", FenqID = "";
                string EncryptUrl = "";


                if (Details.Tables.Count > 0 && Details.Tables[1].Rows.Count > 0 && Details.Tables[1].Rows[0]["LogInSupID"].ToString() != "")
                {
                    sb.Append(System.Environment.NewLine + "Sub : Kind Attn to " + Details.Tables[1].Rows[0]["SuplrCnctName"].ToString() + System.Environment.NewLine);

                    SupID = Details.Tables[1].Rows[0]["LogInSupID"].ToString();
                    CustID = Details.Tables[1].Rows[0]["CusmorId"].ToString();
                    FenqID = Details.Tables[1].Rows[0]["ForeignEnquireId"].ToString();
                    string LEID = Details.Tables[1].Rows[0]["LocalEnquireId"].ToString();
                    string LoginMailId = Details.Tables[1].Rows[0]["LogInMailID"].ToString();
                    string LoginPwd = Details.Tables[1].Rows[0]["LogInPwd"].ToString();

                    string EncryptString = BAL.StringEncrpt_Decrypt.Encrypt(SupID + "&CsID=" + CustID + "&FeqID=" + FenqID + "&LeqID= " + LEID + "&LMID=" + LoginMailId + "&LPWD=" + LoginPwd, true);

                    string url = HttpContext.Current.Request.Url.Authority;
                    if (HttpContext.Current.Request.ApplicationPath.Length > 1)
                        url += HttpContext.Current.Request.ApplicationPath;

                    EncryptUrl += System.Environment.NewLine + System.Environment.NewLine
                                + "   Click on the below link to fill Quotation Details... "
                                + System.Environment.NewLine + System.Environment.NewLine;//or Copy Paste on Browser address bar 

                    EncryptUrl += " <a href='http://" + url + "/Quotations/NewLQuotation.aspx?SupID=" + EncryptString + "'>Click here to fill</a> ";
                }

                string Body = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                "     We have an urgent export requirement for the enquiry Attached. Please send your most "
                + " competitive quotation for the same." + System.Environment.NewLine + System.Environment.NewLine +
                "     Please update the Quotation Details in the enclosed attachment and re-send the same or use below link to fill the Details."
                + "Awaiting for your quickest response." + System.Environment.NewLine + System.Environment.NewLine + EncryptUrl

                + System.Environment.NewLine + System.Environment.NewLine +
                "" + System.Environment.NewLine +
                "Thanking you." + System.Environment.NewLine + System.Environment.NewLine +
                "Yours Faithfully," + System.Environment.NewLine +
                Details.Tables[0].Rows[0]["UserName"].ToString() + System.Environment.NewLine
                + "VOLTA IMPEX PVT. LTD.," + System.Environment.NewLine + "123/3RT, 1ST FLOOR,"
                + System.Environment.NewLine + "S.R.NAGAR, HYDERABAD-500038" + System.Environment.NewLine
                + System.Environment.NewLine +
                "Tel: 91-40 23813280, 23810063" + System.Environment.NewLine + "Fax: 91-40 23715606"
                + System.Environment.NewLine + "Email: " + Details.Tables[0].Rows[0]["PriEmail"].ToString()
                + System.Environment.NewLine +
                "Contact No.: " + Details.Tables[0].Rows[0]["MobileNo"].ToString();

                sb.Append(Body);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
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

        # endregion

        # region ButtonClicks


        /// <summary>
        /// This is used to Delete Attachments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDel_Click(object sender, ImageClickEventArgs e)
        {
            var button = (ImageButton)sender;
            ArrayList Attachments = (ArrayList)Session["Attachments"];
            if (Attachments.Count > 0 && Attachments.Contains(button.ToolTip))
            {
                for (int i = 0; i < Attachments.Count; i++)
                {
                    if (Attachments[i].ToString() == button.ToolTip)
                    {
                        Attachments.RemoveAt(i);
                        break;
                    }
                }
            }
            Session["Attachments"] = Attachments;
            Atta();
        }

        /// <summary>
        /// This is used for Dynamically created Buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lb_Click(object sender, EventArgs e)
        {

            var button = (LinkButton)sender;
            DownloadFile(button.Text);
        }

        /// <summary>
        /// Save & New Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnsavenew_Click(object sender, EventArgs e)
        {
            BeforeSave();
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            BeforeSave();
        }

        /// <summary>
        /// Clear Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
                //Response.Redirect("LEVendors.Aspx", false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMethods

        #region Paging

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string ValidateRowsBeforeSave()
        {
            try
            {
                string Error = "";
                DataSet ds = (DataSet)Session["LEVendors"];
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow[] dr = ds.Tables[0].Select("Check = 'true'");
                    if (dr.Length == 0)
                        Error = "ERROR::Select atleast 1 Item to Save.";
                }
                return Error;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string NextPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_PI"] = (CPage + 1);
            Session["RowsDisplay_PI"] = txtRowsChanged;
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string PrevPage(string CurrentPage, int txtRowsChanged)
        {
            int CPage = Convert.ToInt32(CurrentPage);
            Session["CPage_PI"] = (CPage - 1);
            Session["RowsDisplay_PI"] = txtRowsChanged;
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string RowsChanged(string CurrentPage, string txtRowsChanged)
        {
            if (txtRowsChanged.Trim() != "")
            {
                if (Convert.ToInt32(Session["RowsDisplay_PI"].ToString()) != Convert.ToInt32(txtRowsChanged))
                {
                    int RowStart = ((Convert.ToInt32(Session["RowsDisplay_PI"].ToString()) * Convert.ToInt32(CurrentPage)) - Convert.ToInt32(Session["RowsDisplay_PI"].ToString())) + 1;

                    if (RowStart > Convert.ToInt32(txtRowsChanged))
                        Session["CPage_PI"] = RowStart / Convert.ToInt32(txtRowsChanged) + 1;
                    else if (RowStart < Convert.ToInt32(txtRowsChanged) && RowStart != 1)
                        Session["CPage_PI"] = (Convert.ToInt32(txtRowsChanged) + 1) / RowStart;
                }
                Session["RowsDisplay_PI"] = Convert.ToInt32(txtRowsChanged);
            }
            return FillGridView(Guid.Empty);
        }

        #endregion

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckAll(bool All)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                bool FlagAll = true;
                string SNos = "";
                //if (Session["ReleasedLPOitems"] == null && Session["ReleasedLPOitems"].ToString() != "-9")
                //  GetReleasedLPOItems(new Guid(ddlfenqy.SelectedValue));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string itemId = ds.Tables[0].Rows[i]["ItemId"].ToString();
                    //if (Session["ReleasedLPOitems"].ToString().Contains(itemId.ToUpper()))
                    ds.Tables[0].Rows[i]["Check"] = All;
                    //else
                    //{
                    //    ds.Tables[0].Rows[i]["Check"] = false;
                    //    SNos += (i + 1) + ",";
                    //    FlagAll = false;
                    //}
                }
                ds.AcceptChanges();
                if (SNos != "")
                    Session["TblErrorMessage"] = "LPO was released for " + SNos.Trim(',') + " Items";
                else
                    Session["TblErrorMessage"] = null;
                if (FlagAll == true && All == true)
                    Session["CheckAllBoxes"] = "1";
                else
                    Session["CheckAllBoxes"] = null;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
            return FillGridView(Guid.Empty);
        }


        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddNewRow(int RNo, string sv, string PrtNo, string spec, string Make, string Qty, string Units,
            string Rmrks, bool Check, bool ItmADD)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                Dictionary<int, Guid> Codes = (Dictionary<int, Guid>)HttpContext.Current.Session["SelectedItems"];
                if (sv != "" && !Codes.ContainsValue(new Guid(sv)) && new Guid(sv) != Guid.Empty)
                {
                    if (Codes.ContainsKey(RNo))
                        Codes[RNo] = new Guid(sv);
                    else
                        Codes.Add(RNo, new Guid(sv));
                }
                Session["SelectedItems"] = Codes;
                int RowCount = ds.Tables[0].Rows.Count;
                if (sv != "")
                    ds.Tables[0].Rows[RNo - 1]["ItemId"] = sv;
                if (PrtNo != "")
                    ds.Tables[0].Rows[RNo - 1]["PartNumber"] = PrtNo;

                ds.Tables[0].Rows[RNo - 1]["Specifications"] = spec;
                ds.Tables[0].Rows[RNo - 1]["Make"] = Make;
                if (Qty != "")
                    ds.Tables[0].Rows[RNo - 1]["Quantity"] = Qty;
                if (Units != "" && Units != "0")
                {
                    ds.Tables[0].Rows[RNo - 1]["UNumsId"] = Units;
                    DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + Units.ToString());
                    ds.Tables[0].Rows[RNo - 1]["UnitName"] = selRow[0]["Description"].ToString();
                }
                ds.Tables[0].Rows[RNo - 1]["Remarks"] = Rmrks;
                // if (Session["ReleasedLPOitems"] == null && Session["ReleasedLPOitems"].ToString() != "-9")
                //    GetReleasedLPOItems(FEenquiryID);
                string itemId = ds.Tables[0].Rows[RNo - 1]["ItemId"].ToString();
                //if (Session["ReleasedLPOitems"].ToString().Contains(itemId.ToUpper()))
                //{
                ds.Tables[0].Rows[RNo - 1]["Check"] = Check;
                Session["TblErrorMessage"] = "";
                // }
                //else
                //{
                //    ds.Tables[0].Rows[RNo - 1]["Check"] = false;
                //    Session["TblErrorMessage"] = "LPO was released for this Item";
                //}

                //string NewQty = ds.Tables[0].Rows[RowCount - 1]["Quantity"].ToString();
                //if (sv == "" && Units == "" && Qty != "" && NewQty != "")
                //{
                //    DataRow dr = ds.Tables[0].NewRow();
                //    ds.Tables[0].Rows.Add(dr);
                //}
                if (ItmADD)//adding new Row OR Item
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    ds.Tables[0].Rows.Add(dr);
                    ds.Tables[0].Rows[RNo]["Check"] = false;
                    ds.Tables[0].Rows[RNo]["Quantity"] = 0;
                }
                ds.AcceptChanges();
                Session["LEVendors"] = ds;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string BindGridView(string rowNo)
        {
            return FillGridView(new Guid(rowNo));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItem(int rowNo)
        {
            try
            {
                Dictionary<int, int> Codes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedItems"];
                string ItemsDetailsID = "";
                int res = 0;
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                ItemsDetailsID = ds.Tables[0].Rows[rowNo - 1]["LocalEnquireId"].ToString();
                if (ItemsDetailsID != "")
                    res = IDBLL.ItemDetailsDelete(CommonBLL.FlagDelete, new Guid(ItemsDetailsID), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                        Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, 0, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, false, new Guid(Session["CompanyID"].ToString()));
                if ((res == 0 && ItemsDetailsID != "") || ItemsDetailsID == "")
                {
                    ds.Tables[0].Rows[rowNo - 1].Delete();
                    ds.Tables[0].AcceptChanges();
                    if (Codes.ContainsKey(rowNo))
                        Codes.Remove(rowNo);
                }
                Session["SelectedItems"] = Codes;
            }
            catch (Exception ex)
            {
                string linenum = ex.LineNumber().ToString();
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry Vendor", ex.Message.ToString());
            }
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillItemDRP(int rowNo, int sv)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    DataRow[] selRow = dss.Tables[0].Select("ID = " + sv.ToString());
                    ds.Tables[0].Rows[rowNo - 1]["ItemId"] = selRow[0]["ID"].ToString();
                    ds.Tables[0].Rows[rowNo - 1]["ItemDesc"] = selRow[0]["ItemDescription"].ToString();

                    Dictionary<int, int> Codes = (Dictionary<int, int>)HttpContext.Current.Session["SelectedItems"];
                    if (sv != 0 && !Codes.ContainsValue(Convert.ToInt32(sv)) && Convert.ToInt32(sv) != 0)
                    {
                        if (Codes.ContainsKey(rowNo))
                            Codes[rowNo] = Convert.ToInt32(sv);
                        else
                            Codes.Add(rowNo, Convert.ToInt32(sv));
                    }
                    Session["SelectedItems"] = Codes;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string FillUnitDRP(int rowNo, int sv, string PrtNo, string spec, string Make, string Qty, string Units, string Rmrks)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = (DataSet)Session["LEVendors"];
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (dsEnm.Tables.Count > 0 && dsEnm.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] selRow = dsEnm.Tables[0].Select("ID = " + sv.ToString());
                        ds.Tables[0].Rows[rowNo - 1]["UNumsId"] = sv;
                        ds.Tables[0].Rows[rowNo - 1]["UnitName"] = selRow[0]["Description"].ToString();
                        ds.Tables[0].AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                string linenum = ex.LineNumber().ToString();
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Floated Foreign Enquiry", ex.Message.ToString());
            }
            return FillGridView(Guid.Empty);
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            ArrayList all = new ArrayList();
            all = CBLL.UploadedFileNames();
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
            for (int k = 0; k < all.Count; k++)
                sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
            sb.Append("</select>");
            return sb.ToString();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            ArrayList all = new ArrayList();
            all = CBLL.UploadedFileNames();
            StringBuilder sb = new StringBuilder();
            sb.Append("<select id='lbItems' style='background-color:#CCCCFF;width:221px;' name='lstItems' size='6'>");
            for (int k = 0; k < all.Count; k++)
                sb.Append("<option value=" + k.ToString() + ">" + all[k].ToString() + "</option>");
            sb.Append("</select>");
            return sb.ToString();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string GetHistoryItems(string ItemId)
        {

            DataSet dsd = new DataSet();
            dsd = IDBLL.ItemDetailsInsertUpdateEdit(CommonBLL.FlagSelectAll, Guid.Empty, new Guid(ItemId), new Guid(Session["EnqID"].ToString()), Guid.Empty,
                Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "", "", "", 0, 0, 0, 0, 0, Guid.Empty, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now,
                true, new Guid(Session["CompanyID"].ToString()));

            if (dsd.Tables[1].Columns.Contains("ItemId"))
                dsd.Tables[1].Columns.Remove("ItemId");
            if (dsd.Tables[1].Columns.Contains("ItemId1"))
                dsd.Tables[1].Columns.Remove("ItemId1");
            StringBuilder sb2 = new StringBuilder();
            var res = (from dc in dsd.Tables[1].Rows.Cast<DataRow>()
                       select dc.ItemArray).ToArray();
            if (res.Length > 0)
                sb2.Append("<b>Previous Rates :</b><br/>");
            else
                sb2.Append("<b>No Previous Rates available</b><br/>");
            foreach (var item in res)
            {
                sb2.Append(string.Join(" --> ", item.ToArray()));
                sb2.Append(" <br /> ");
            }
            return sb2.ToString();
        }

        # endregion
    }
}