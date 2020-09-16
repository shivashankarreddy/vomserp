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
using System.Collections.Generic;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Purchases
{
    public partial class DrawingApproval : System.Web.UI.Page
    {
        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        CommonBLL CBLL = new CommonBLL();
        SupplierBLL SBLL = new SupplierBLL();
        CustomerBLL CSTBLL = new CustomerBLL();
        EnumMasterBLL EMBLL = new EnumMasterBLL();
        RqstCEDtlsBLL RCEDBLL = new RqstCEDtlsBLL();
        DrawingApprovalBLL DABLL = new DrawingApprovalBLL();
        static Guid EditID = Guid.Empty;
        static Guid CustomerID = Guid.Empty;
        static Guid SupplierID = Guid.Empty;
        static string ErrorMessage = "";
        static Dictionary<string, string> DicLPOs = new Dictionary<string, string>();
        AuditLogs ALS = new AuditLogs();
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
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        txtRecvDt.Attributes.Add("readonly", "readonly");
                        txtApprovalReqDt.Attributes.Add("readonly", "readonly");
                        txtCustReponseDt.Attributes.Add("readonly", "readonly");
                        txtSuppIntimationDt.Attributes.Add("readonly", "readonly");
                        txtSelfApprovalDT.Attributes.Add("readonly", "readonly");
                        Ajax.Utility.RegisterTypeForAjax(typeof(DrawingApproval));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            DicLPOs = new Dictionary<string, string>();
                            GetData();
                            divListBox.InnerHtml = AttachedFiles();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        #endregion

        #region  Methods

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

        /// <summary>
        /// Bind Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlcustmr, CSTBLL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyId"].ToString())));
                BindDropDownList(ddlsuplrctgry, EMBLL.EnumMasterSelectforDescription(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()), CommonBLL.SupplierCategory));

                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                   new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    EditRecord(DABLL.GetDataSet(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()),
                        Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "", "", "",
                        Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "", new Guid(Session["CompanyId"].ToString())));
                }
                else if (Request.QueryString["LPOID"] != null)
                {
                    ddlcustmr.SelectedValue = Request.QueryString["CustID"];
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplrctgry.Enabled = false;
                    BindDropDownList(ddlsuplr, DABLL.GetDataSet(CommonBLL.FlagHSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                       new Guid(ddlsuplrctgry.SelectedValue), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlsuplr.SelectedValue = Request.QueryString["SupID"];

                    //BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagFSelect, 0,
                    //int.Parse(ddlcustmr.SelectedValue), int.Parse(ddlsuplrctgry.SelectedValue), "", "", "", CommonBLL.UserID));

                    BindDropDownList(ddlFPO, DABLL.GetDataSet(CommonBLL.FlagGSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplr.SelectedValue), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlFPO.SelectedValue = Request.QueryString["FPOID"];
                    BindLPO();
                    ddlLPO.SelectedValue = Request.QueryString["LPOID"];
                    BindGridView();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        private void BindLPO()
        {
            try
            {
                if (new Guid(ddlFPO.SelectedValue) != Guid.Empty)
                {
                    string FpoIds = "";// String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    DataSet Lpos = RCEDBLL.SelectRqstCEDtlsDrawngAppr(CommonBLL.FlagZSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                        new Guid(ddlsuplr.SelectedValue), new Guid(ddlFPO.SelectedValue), Guid.Empty, "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (Lpos.Tables.Count > 0 && Lpos.Tables[0].Rows.Count > 0)
                    {
                        ddlLPO.DataSource = Lpos.Tables[0];
                        ddlLPO.DataTextField = "Description";
                        ddlLPO.DataValueField = "ID";
                        ddlLPO.DataBind();
                        ddlLPO.Items.Insert(0, new ListItem("-- Select LPO--", "0"));

                        txtLPODate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(Lpos.Tables[1].Rows[0][0].ToString()));
                    }
                }
                else
                {
                    divDrawings.InnerHtml = "";
                    txtRemarks.Text = "";
                    txtSubject.Text = "";
                    txtSelfApprovalDT.Text = "";
                    txtSuppIntimationDt.Text = "";
                    txtCustReponseDt.Text = "";
                    txtApprovalReqDt.Text = "";
                    txtRecvDt.Text = "";
                    txtLPODate.Text = "";
                    ddlLPO.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit/Update Record
        /// </summary>
        /// <param name="CommonDt"></param>
        private void EditRecord(DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 1)
                {
                    Guid CustID = new Guid(CommonDt.Tables[0].Rows[0]["CustomerID"].ToString());
                    Guid SuplierID = new Guid(CommonDt.Tables[0].Rows[0]["SupplierID"].ToString());
                    ddlcustmr.SelectedValue = CustID.ToString();
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplrctgry.Enabled = false;
                    BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagFSelect, Guid.Empty,
                        CustID, new Guid(ddlsuplrctgry.SelectedValue), "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
                    ddlsuplr.SelectedValue = SuplierID.ToString();

                    DataSet Fpos = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagGSelect, Guid.Empty,
                   CustID, SuplierID, "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (Fpos.Tables.Count > 0)
                    {
                        ddlFPO.DataSource = Fpos.Tables[0];
                        ddlFPO.DataTextField = "Description";
                        ddlFPO.DataValueField = "ID";
                        ddlFPO.DataBind();
                        ddlFPO.Items.Insert(0, new ListItem("-- Select FPO--", "0"));
                    }
                    ddlFPO.SelectedValue = CommonDt.Tables[0].Rows[0]["FPO"].ToString();
                    txtSubject.Text = CommonDt.Tables[0].Rows[0]["Subject"].ToString();
                    hfDrawingRefNo.Value = CommonDt.Tables[0].Rows[0]["DrawingRefNo"].ToString();
                    txtRemarks.Text = CommonDt.Tables[0].Rows[0]["Remarks"].ToString();
                    DataSet Lpos = RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagXSelect, Guid.Empty,
                    new Guid(CommonDt.Tables[0].Rows[0]["CustomerID"].ToString()),
                    new Guid(CommonDt.Tables[0].Rows[0]["SupplierID"].ToString()),
                    CommonDt.Tables[0].Rows[0]["FPO"].ToString(), "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    if (Lpos.Tables.Count > 0 && Lpos.Tables[0].Rows.Count > 0)
                    {
                        ddlLPO.DataSource = Lpos.Tables[0];
                        ddlLPO.DataTextField = "Description";
                        ddlLPO.DataValueField = "ID";
                        ddlLPO.DataBind();
                        ddlLPO.Items.Insert(0, new ListItem("-- Select LPO--", "0"));

                        txtLPODate.Text = CommonBLL.DateDisplay(Convert.ToDateTime(Lpos.Tables[1].Rows[0][0].ToString()));
                    }
                    ddlLPO.SelectedValue = CommonDt.Tables[0].Rows[0]["LPO"].ToString();
                    //txtRefNo.Text = CommonDt.Tables[0].Rows[0]["DrawingRefNo"].ToString();
                    if (CommonDt.Tables[0].Rows[0]["DrawingReceivedDate"].ToString() != "")
                        txtRecvDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["DrawingReceivedDate"].ToString()));
                    if (CommonDt.Tables[0].Rows[0]["ApprovalReqDate"].ToString() != "")
                        txtApprovalReqDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["ApprovalReqDate"].ToString()));
                    if (CommonDt.Tables[0].Rows[0]["CustResponseDate"].ToString() != "")
                        txtCustReponseDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["CustResponseDate"].ToString()));
                    if (CommonDt.Tables[0].Rows[0]["SuppIntimationDate"].ToString() != "")
                        txtSuppIntimationDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["SuppIntimationDate"].ToString()));
                    if (CommonDt.Tables[0].Rows[0]["SelfApprovalDate"].ToString() != "")
                        txtSelfApprovalDT.Text = CommonBLL.DateDisplay(Convert.ToDateTime(CommonDt.Tables[0].Rows[0]["SelfApprovalDate"].ToString()));

                    if (CommonDt.Tables[0].Rows[0]["Attachment"].ToString() != "")
                    {
                        string[] all = CommonDt.Tables[0].Rows[0]["Attachment"].ToString().Split(',');
                        CBLL.ClearUploadedFiles();
                        CBLL.UploadedFilesAL(all);
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        ArrayList attms = new ArrayList();
                        attms.AddRange((CommonDt.Tables[0].Rows[0]["Attachment"].ToString().Split(',')).ToArray());
                        Session["Drawing Approval"] = attms;
                        AttachedFiles();
                    }
                    divDrawings.InnerHtml = FillItemGrid(CommonDt.Tables[1]);
                    Session["DrawingApp"] = CommonDt.Tables[1];
                    //ddlStatus.Items.FindByText(CommonDt.Tables[0].Rows[0]["ResponseStatus"].ToString()).Selected = true;
                    ddlcustmr.Enabled = false;
                    ddlsuplrctgry.Enabled = false;
                    DivComments.Visible = true;
                    btnSend.Text = "Update";
                    EditID = new Guid(Request.QueryString["ID"].ToString());
                    ViewState["EditID"] = Request.QueryString["ID"];
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
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
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind List Boxes
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        protected void BindListBox(ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt.Tables[0];
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        private void ClearInputs()
        {
            try
            {
                Session.Remove("Drawing Approval");
                EditID = Guid.Empty;
                DicLPOs = new Dictionary<string, string>();
                ddlcustmr.SelectedIndex = ddlFPO.SelectedIndex = ddlLPO.SelectedIndex = ddlsuplr.SelectedIndex = ddlsuplrctgry.SelectedIndex = 0;
                //ddlStatus.SelectedIndex = ddlsuplr.SelectedIndex = ddlsuplrctgry.SelectedIndex = -1;
                txtApprovalReqDt.Text = "";
                txtComments.Text = "";
                txtCustReponseDt.Text = "";
                txtLPODate.Text = "";
                txtRecvDt.Text = "";
                //txtRefNo.Text = "";
                txtRemarks.Text = "";
                txtSelfApprovalDT.Text = "";
                txtSuppIntimationDt.Text = "";
                txtSubject.Text = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
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
                if (AsyncFileUpload1.HasFile)
                {
                    if (AsyncFileUpload1.PostedFile.ContentLength < 25165824)
                    {
                        ArrayList alist = new ArrayList();
                        string strPath = MapPath("~/uploads/");// +Path.GetFileName(AsyncFileUpload1.FileName);
                        string FileNames = CommonBLL.Replace(AsyncFileUpload1.FileName);
                        if (Session["Drawing Approval"] != null)
                        {
                            alist = (ArrayList)Session["Drawing Approval"];
                            if (!alist.Contains(FileNames))
                                alist.Add(FileNames);
                        }
                        else if (Session["Drawing Approval"] == null)
                        {
                            alist.Add(FileNames);
                        }
                        Session["Drawing Approval"] = alist;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
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
                if (Session["Drawing Approval"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["Drawing Approval"];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return ex.Message;
            }
        }

        /// <summary>
        /// This is used to Bind Drawing Grid
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string FillItemGrid(DataTable dt)
        {
            try
            {
                DataSet dsFPOs = new DataSet();
                dsFPOs = DABLL.GetDataSet(CommonBLL.FlagASelect, Guid.Empty, CustomerID, SupplierID, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                Session["FPOs"] = dsFPOs;

                //DataTable dt = new DataTable();
                //dt = (DataTable)Session["DrawingApp"];

                StringBuilder sb = new StringBuilder();
                sb.Append("");
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' class='rounded-corner'" +
                " id='DrawingAppTbl'><thead align='left'><tr>");
                sb.Append("<th class='rounded-First'>SNo</th><th>Drawing Ref.No.</th><th>Description</th><th>Comments</th><th>Res. Status</th><th class='rounded-Last' width='6%'></th>");
                sb.Append("</tr></thead><tbody>");

                if (dt != null && dt.Rows.Count > 0)
                {
                    //ds = ItemMstBLl.SelectItemMaster(CommonBLL.FlagRegularDRP, 0, Convert.ToInt32(GeneralCtgryID));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string SNo = (i + 1).ToString();
                        sb.Append("<tr>");
                        sb.Append("<td valign='top'>" + SNo + "</td>");

                        # region Not In USe
                        //#region Fill FPO Dropdown

                        //sb.Append("<td>");
                        //sb.Append("<select id='ddlFPO" + SNo + "' onchange='FillItemGrid(" + SNo + ")' Class='bcAspdropdown' style='width:120px;'>");
                        //sb.Append("<option value='0'>-SELECT-</option>");

                        //foreach (DataRow row in dsFPOs.Tables[0].Rows)
                        //{
                        //    //if (Codes.ContainsKey(Convert.ToInt32(dt.Rows[i]["SNo"])) &&
                        //    //    Codes[Convert.ToInt32(dt.Rows[i]["SNo"])].ToString() == row["ID"].ToString())
                        //    //{
                        //    //    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                        //    //        row["ItemDescription"].ToString() + "</option>");
                        //    //    ds2 = ItemMstBLl.SelectItemMaster(CommonBLL.FlagModify, (Convert.ToInt32(row["ID"].ToString())), 0);
                        //    //}
                        //    //else if (!Codes.ContainsValue(Convert.ToInt32(row["ID"])) && Convert.ToInt32(row["ID"]) != CodeID)
                        //    //if (dt.Rows.Count - 1 == i)
                        //    //{
                        //    if (dt.Rows[i]["FPOID"].ToString() == row["ID"].ToString())
                        //        sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                        //    else
                        //        sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                        //    //}
                        //}
                        //sb.Append("</select>");
                        //sb.Append("</td>");

                        //#endregion

                        //#region Fill LPO Dropdown
                        //sb.Append("<td>");
                        //sb.Append("<select id='ddlLPO" + SNo + "' onchange='FillItemGrid(" + SNo + ")' Class='bcAspdropdown' style='width:180px;'>");
                        //sb.Append("<option value='0'>-SELECT-</option>");
                        //string LPODate = "";
                        //if (dt.Rows[i]["FPOID"].ToString() != "")
                        //{
                        //    DataSet dsLPOs = DABLL.GetDataSet(CommonBLL.FlagBSelect, Convert.ToInt64(dt.Rows[i]["FPOID"].ToString()), 0, 0, 0);
                        //    foreach (DataRow row in dsLPOs.Tables[0].Rows)
                        //    {
                        //        //if (Codes.ContainsKey(Convert.ToInt32(dt.Rows[i]["SNo"])) &&
                        //        //    Codes[Convert.ToInt32(dt.Rows[i]["SNo"])].ToString() == row["ID"].ToString())
                        //        //{
                        //        //    sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" +
                        //        //        row["ItemDescription"].ToString() + "</option>");
                        //        //    ds2 = ItemMstBLl.SelectItemMaster(CommonBLL.FlagModify, (Convert.ToInt32(row["ID"].ToString())), 0);
                        //        //}
                        //        //else if (!Codes.ContainsValue(Convert.ToInt32(row["ID"])) && Convert.ToInt32(row["ID"]) != CodeID)


                        //        if (dt.Rows[i]["LPOID"].ToString() == row["ID"].ToString())
                        //        {
                        //            sb.Append("<option value='" + row["ID"].ToString() + "' selected='selected'>" + row["Description"].ToString() + "</option>");
                        //            LPODate = row["LPODate"].ToString();
                        //        }
                        //        else if (!DicLPOs.ContainsValue(row["ID"].ToString()))
                        //            sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                        //        //else
                        //        //    sb.Append("<option value='" + row["ID"].ToString() + "'>" + row["Description"].ToString() + "</option>");
                        //    }
                        //}
                        //sb.Append("</select>");
                        //sb.Append("<input type='hidden' name='HFLPODate' id='HFLPODate" + SNo + "' value='" + LPODate + "'/>");
                        //sb.Append("</td>");

                        //#endregion

                        //sb.Append("<td><input type='text' name='txtDrawingRefNo' class='bcAsptextbox' id='txtDrawingRefNo" + SNo + "'" +
                        //    " value='" + dt.Rows[i]["DrawingRefNo"].ToString() + "' onchange='GetRefNo(" + SNo + ")' style='width:90px;'/></td>");

                        //sb.Append("<td><input type='text' name='txtRcvdDT' id='txtRcvdDT" + SNo + "' onchange='FillItemGrid(" + SNo + ")'" +
                        //    "  value='" + dt.Rows[i]["RcvdDT"].ToString() + "' readonly='readonly' class='bcAsptextbox' style='width:90px;'/></td>");

                        //sb.Append("<td><input type='text' name='txtApprovalReqDT' id='txtApprovalReqDT" + SNo + "' onchange='FillItemGrid(" + SNo + ")'" +
                        //    "  value='" + dt.Rows[i]["AppRqstDT"].ToString() + "' readonly='readonly' class='bcAsptextbox' style='width:90px;'/></td>");

                        //sb.Append("<td><input type='text' name='txtCustResponseDT' id='txtCustResponseDT" + SNo + "'  onchange='FillItemGrid(" + SNo + ")'" +
                        //    " value='" + dt.Rows[i]["CustRqstDT"].ToString() + "' readonly='readonly' class='bcAsptextbox' style='width:90px;'/></td>");

                        //sb.Append("<td><input type='text' name='txtSuppIntDT' id='txtSuppIntDT" + SNo + "' onchange='FillItemGrid(" + SNo + ")'" +
                        //    " value='" + dt.Rows[i]["SuppIntDT"].ToString() + "' readonly='readonly' class='bcAsptextbox' style='width:90px;'/></td>");
                        # endregion

                        sb.Append("<td valign='top'><input type='text' name='txtDrawingRefNo' class='bcAsptextbox' id='txtDrawingRefNo" + SNo + "'" +
                            " value='" + dt.Rows[i]["DrawingRefNo"].ToString() + "' onchange='GetRefNo(" + SNo + ")' style='width:150px;'/></td>");

                        sb.Append("<td valign='top'><textarea name='txtDescription' id='txtDescription" + SNo + "' onchange='ReSizeTXT(this.id);FillItemGrid("
                            + SNo + ");' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' onblur='ReSizeTXT(this.id)' style='height:20px; width:200px; resize:none;'>"
                            + dt.Rows[i]["Description"].ToString() + "</textarea></td>");

                        sb.Append("<td valign='top'><textarea name='txtComments' class='bcAsptextbox' id='txtComments" + SNo + "' onchange='FillItemGrid("
                            + SNo + ")' class='bcAsptextboxmulti' onfocus='ExpandTXT(this.id)' onblur='ReSizeTXT(this.id)' style='height:20px; width:200px; resize:none;'>"
                            + dt.Rows[i]["Comments"].ToString() + "</textarea></td>");

                        #region Fill Status Dropdown
                        string stat = (dt.Rows[i]["ResStat"].ToString() != "" ? dt.Rows[i]["ResStat"].ToString() : "0");
                        sb.Append("<td valign='top'>");
                        sb.Append("<select id='ddlStatus" + (i + 1) + "' onchange='FillItemGrid(" + SNo + ")' class='bcAspdropdown' style='width:80px;'>");
                        sb.Append("<option value='0'>-SELECT-</option>");
                        sb.Append("<option value='1' " + (stat != "1" ? "" : "selected='selected'") + ">Accepted</option>");
                        sb.Append("<option value='2' " + (stat != "2" ? "" : "selected='selected'") + ">Rejected</option>");
                        sb.Append("</select>");
                        sb.Append("</td>");
                        #endregion

                        sb.Append("<td valign='top'>");
                        if (dt.Rows.Count == 1)
                            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;<span class='gridactionicons'><a href='javascript:void(0)'" +
                        " onclick='AddNewRow(" + SNo + ")'" +
                        " class='icons additionalrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a></span>");
                        else if (dt.Rows.Count - 1 == i)
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' onclick='javascript:return doConfirm(" + SNo + ")' " +
                                " title='Delete'><img src='../images/btnDelete.png' style='border-style: none;'/></a></span>&nbsp;&nbsp;" +
                                "<a href='javascript:void(0)' onclick='AddNewRow(" + SNo + ")' " +
                                " class='icons additionalrow' title='Add New Row'><img src='../images/btnAdd.png' style='border-style: none;'/></a>");
                        else
                            sb.Append("<span class='gridactionicons'><a href='javascript:void(0)' " +
                                " onclick='javascript:return doConfirm(" + SNo + ")' class='icons deleteicon' " +
                                " title='Delete' OnClientClick='javascript:return doConfirm();'>" +
                                " <img src='../images/btnDelete.png' style='border-style: none;'/></a></span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                }
                else
                {
                    sb.Append("<td colspan='6'><center><b>No rows found...!</center></b></td>");
                }
                sb.Append("</tbody>");
                sb.Append("<tfoot><th class='rounded-foot-left'></th><th colspan='4' style='height:17px;'></th>");
                //sb.Append("<th><input type='hidden' name='HFRowCount' id='HFRowCount' value='" + dt.Rows.Count + "'/></th>");
                sb.Append("<th class='rounded-foot-right'></th></tfoot></table>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return ErrMsg;
            }
        }

        /// <summary>
        /// This is used to create Drawing table
        /// </summary>
        private void BindGridView()
        {
            try
            {
                DataTable dt = new DataTable();
                if (Request.QueryString["ID"] == null)
                    dt = CommonBLL.EmptyDTDrawingDetails();
                else
                    dt = (DataTable)Session["DrawingApp"];
                Session["DrawingApp"] = dt;
                divDrawings.InnerHtml = FillItemGrid(dt);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events

        /// <summary>
        /// Send/Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            int res = 1; Filename = FileName();
            try
            {
                DateTime SelfApprovalDT = DateTime.MaxValue;// String.Join(", ", lbfpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray()); 
                if (txtSelfApprovalDT.Text != "")
                    SelfApprovalDT = CommonBLL.DateInsert(txtSelfApprovalDT.Text);
                DateTime ApprovalRqstDT = DateTime.Now;// String.Join(", ", lblpos.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                if (txtApprovalReqDt.Text != "")
                    ApprovalRqstDT = CommonBLL.DateInsert(txtApprovalReqDt.Text);
                string responseStat = "";
                //if (ddlStatus.SelectedValue != "0")
                //    responseStat = ddlStatus.SelectedItem.Text;
                string Atchmnts = Session["Drawing Approval"] == null ? "" :
                    string.Join(",", ((ArrayList)Session["Drawing Approval"]).Cast<string>().ToArray());
                DataTable DrawingDtails = (DataTable)Session["DrawingApp"];
                if (DrawingDtails.Rows.Count > 0)
                {
                    if (btnSend.Text == "Save")
                    {
                        int count = 1;
                        string DrawingRefNo = "";
                        DataSet dss = new DataSet();
                        dss = DABLL.GetDataSet(CommonBLL.FlagINewInsert, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0 && dss.Tables[0].Rows[0][0].ToString() != "")
                            count = Convert.ToInt32(dss.Tables[0].Rows[0][0].ToString()) + 1;
                        DrawingRefNo = "DA/" + Session["AliasName"] + "/" + count + "/" + CommonBLL.FinacialYearShort;
                        res = DABLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                            new Guid(ddlsuplr.SelectedValue), new Guid(ddlFPO.SelectedValue), new Guid(ddlLPO.SelectedValue),
                            CommonBLL.DateInsert(txtLPODate.Text), Atchmnts, DrawingRefNo, CommonBLL.DateInsert(txtRecvDt.Text),
                            ApprovalRqstDT, SelfApprovalDT, CommonBLL.DateInsert(txtCustReponseDt.Text), CommonBLL.DateInsert(txtSuppIntimationDt.Text),
                            responseStat, txtRemarks.Text.Trim(), "", new Guid(Session["UserID"].ToString()),
                            CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")),
                            true, DrawingDtails, txtSubject.Text.Trim(), new Guid(Session["CompanyId"].ToString()));

                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Drawing Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Drawing Approval", "Data inserted successfully.");
                            Response.Redirect("../Purchases/DrawingApprovalStatus.Aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Drawing Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", "Error while Saving.");
                        }
                    }
                    else if (btnSend.Text == "Update")
                    {
                        res = DABLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), new Guid(ddlcustmr.SelectedValue),
                             new Guid(ddlsuplr.SelectedValue), new Guid(ddlFPO.SelectedValue), new Guid(ddlLPO.SelectedValue),
                             CommonBLL.DateInsert(txtLPODate.Text), Atchmnts, hfDrawingRefNo.Value, CommonBLL.DateInsert(txtRecvDt.Text), ApprovalRqstDT, SelfApprovalDT,
                             CommonBLL.DateInsert(txtCustReponseDt.Text), CommonBLL.DateInsert(txtSuppIntimationDt.Text), responseStat, txtRemarks.Text.Trim(),
                             txtComments.Text.Trim(), Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, DrawingDtails, txtSubject.Text.Trim(), new Guid(Session["CompanyId"].ToString()));
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Drawing Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/Log"), "Drawing Approval", "Data Updated successfully.");
                            Response.Redirect("../Purchases/DrawingApprovalStatus.Aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSend.Text, "", "Drawing Approval", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", "Error while Updating.");
                        }
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Drawings to save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
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
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }
        #endregion

        #region Selected Index Changed Events

        /// <summary>
        /// Customer Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (new Guid(ddlcustmr.SelectedValue) != Guid.Empty)
                {
                    ddlsuplrctgry.SelectedValue = ddlsuplrctgry.Items.FindByText("General").Value;
                    ddlsuplrctgry.Enabled = false;
                    BindDropDownList(ddlsuplr, DABLL.GetDataSet(CommonBLL.FlagHSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                       new Guid(ddlsuplrctgry.SelectedValue), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyId"].ToString())));
                }
                else
                    ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Category Selected index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplrctgry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlsuplr, RCEDBLL.SelectRqstCEDtls(CommonBLL.FlagFSelect, Guid.Empty,
                    new Guid(ddlcustmr.SelectedValue), new Guid(ddlsuplrctgry.SelectedValue), "", "", "", new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Supplier Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlsuplr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDropDownList(ddlFPO, DABLL.GetDataSet(CommonBLL.FlagGSelect, Guid.Empty, new Guid(ddlcustmr.SelectedValue),
                    new Guid(ddlsuplr.SelectedValue), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        protected void ddlFPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindLPO();
        }

        protected void ddlLPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CustomerID = new Guid(ddlcustmr.SelectedValue);
                SupplierID = new Guid(ddlsuplr.SelectedValue);
                if (ddlLPO.SelectedValue != "0")
                    BindGridView();
                else
                    divDrawings.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        #endregion

        #region Web Methods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                if (Session["Drawing Approval"] != null)
                {
                    ArrayList all = (ArrayList)Session["Drawing Approval"];
                    if (all.Count > 0)
                        all.RemoveAt(ID);
                }
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return ex.Message;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetRefNo(string DrawingRefNo)
        {
            bool res = false;
            try
            {
                DataSet ds = new DataSet();
                ds = DABLL.GetDataSet(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, DateTime.Now, "", DrawingRefNo.ToLower(), DateTime.Now, DateTime.Now,
                    DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTDrawingDetails(), "", new Guid(Session["CompanyId"].ToString()));
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (EditID != Guid.Empty)
                        res = false;
                    else
                        res = true;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
            return res;
        }

        #endregion

        # region Gridview Events Not INUSE

        protected void GvDrawingApprovals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header) return;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var ddlFPO = (DropDownList)e.Row.FindControl("ddlFPO");
                    DataSet dss = (DataSet)Session["FPOs"];
                    if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                    {
                        ddlFPO.DataSource = dss.Tables[0];
                        ddlFPO.DataTextField = "Description";
                        ddlFPO.DataValueField = "ID";
                        ddlFPO.DataBind();
                        ddlFPO.SelectedValue = ((HiddenField)e.Row.FindControl("hfFPO")).Value;
                    }
                    if (ddlFPO.SelectedValue != "0")
                    {
                        var ddlLPO = (DropDownList)e.Row.FindControl("ddlLPO");
                        DataSet dsLPO = DABLL.GetDataSet(CommonBLL.FlagBSelect, new Guid(ddlFPO.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                        if (dsLPO.Tables.Count > 0)
                        {
                            ddlLPO.DataSource = dsLPO.Tables[0];
                            ddlLPO.DataTextField = "Description";
                            ddlLPO.DataValueField = "ID";
                            ddlLPO.DataBind();
                            if (((HiddenField)e.Row.FindControl("hfLPO")).Value == "")
                            {
                                ddlLPO.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select LPO--", "0"));
                                ddlLPO.SelectedValue = "0";
                            }
                            else
                                ddlLPO.SelectedValue = ((HiddenField)e.Row.FindControl("hfLPO")).Value;
                        }
                    }
                }

                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    var ddlAddFPO = (DropDownList)e.Row.FindControl("ddlAddFPO");

                    DataSet ds = new DataSet();
                    ds = DABLL.GetDataSet(CommonBLL.FlagASelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()));
                    if (ds.Tables.Count > 0)
                    {
                        ddlAddFPO.DataSource = ds.Tables[0];
                        ddlAddFPO.DataTextField = "Description";
                        ddlAddFPO.DataValueField = "ID";
                        ddlAddFPO.DataBind();
                        ddlAddFPO.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select FPO--", "0"));
                        Session["FPOs"] = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        protected void GvDrawingApprovals_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)Session["DrawingApp"];
                if (e.CommandName == "Add")
                {
                    if ((new Guid(dt.Rows[0]["FPOID"].ToString()) == Guid.Empty) && (new Guid(dt.Rows[0]["LPOID"].ToString()) == Guid.Empty))
                        dt.Rows.RemoveAt(0);

                    DataRow DR = dt.NewRow();
                    //DR["SNO"] = dt.Rows.Count + 1;
                    //DR["FPOID"] = ((DropDownList)GvDrawingApprovals.FooterRow.FindControl("ddlAddFPO")).SelectedValue;
                    //DR["LPOID"] = ((DropDownList)GvDrawingApprovals.FooterRow.FindControl("ddlAddFPO")).SelectedValue;
                    //DR["DrawingRefNo"] = ((TextBox)GvDrawingApprovals.FooterRow.FindControl("txtAddDrawingRefNo")).Text;
                    //DR["RcvdDT"] = ((TextBox)GvDrawingApprovals.FooterRow.FindControl("txtAddReceivedDT")).Text;
                    //DR["AppRqstDT"] = ((TextBox)GvDrawingApprovals.FooterRow.FindControl("txtAddApprovalReqDT")).Text;
                    //DR["CustRqstDT"] = ((TextBox)GvDrawingApprovals.FooterRow.FindControl("txtAddCustResDT")).Text;
                    //DR["SuppIntDT"] = ((TextBox)GvDrawingApprovals.FooterRow.FindControl("txtAddSuppIntDT")).Text;
                    //DR["ResStat"] = ((DropDownList)GvDrawingApprovals.FooterRow.FindControl("ddlAddResStatus")).SelectedValue;
                    dt.Rows.Add(DR);
                }
                else if (e.CommandName == "Delete")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    if (dt.Rows.Count >= index)
                    {
                        dt.Rows.RemoveAt(index);
                        dt.AcceptChanges();
                    }
                }
                Session["DrawingApp"] = dt;
                //GvDrawingApprovals.DataSource = dt;
                //GvDrawingApprovals.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
            }
        }

        protected void GvDrawingApprovals_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void ddlFPO_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlAddFPO_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //long FPOval =  Convert.ToInt64(((DropDownList)GvDrawingApprovals.FooterRow.FindControl("ddlAddFPO")).SelectedValue);
                //var ddlLPO = (DropDownList)GvDrawingApprovals.FooterRow.FindControl("ddlAddLPO");
                //DataSet ds = new DataSet();
                //ds = DABLL.GetDataSet(CommonBLL.FlagBSelect, FPOval, 0, 0, 0);
                //if (ds.Tables.Count > 0)
                //{
                //    ddlLPO.DataSource = ds.Tables[0];
                //    ddlLPO.DataTextField = "Description";
                //    ddlLPO.DataValueField = "ID";
                //    ddlLPO.DataBind();
                //    ddlLPO.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select LPO--", "0"));
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
            }
        }

        protected void ddlLPO_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlAddLPO_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        # endregion

        # region WebMethods Grid

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string AddNewRow()
        {
            DataTable dt = (DataTable)Session["DrawingApp"];
            try
            {
                dt.Rows.Add(dt.NewRow());
                dt.AcceptChanges();
                Session["DrawingApp"] = dt;
                return FillItemGrid(dt);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return FillItemGrid(dt);
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string SaveChanges(string RowNo, string DrRfNo, string Desc, string Comments, string status)
        {
            DataTable dt = (DataTable)Session["DrawingApp"];
            try
            {
                int RNo = Convert.ToInt32(RowNo) - 1;

                dt.Rows[RNo]["DrawingRefNo"] = DrRfNo;
                dt.Rows[RNo]["Description"] = Desc;
                dt.Rows[RNo]["Comments"] = Comments;
                dt.Rows[RNo]["ResStat"] = status;
                dt.AcceptChanges();
                Session["DrawingApp"] = dt;
                return FillItemGrid(dt);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return FillItemGrid(dt);
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string DeleteRecord(string RNo)
        {
            DataTable dt = (DataTable)Session["DrawingApp"];
            try
            {
                int RowNo = Convert.ToInt32(RNo) - 1;
                //string lpoid = dt.Rows[RowNo]["LPOID"].ToString();
                dt.Rows.RemoveAt(RowNo);
                dt.AcceptChanges();
                //DicLPOs.Remove(lpoid);
                Session["DrawingApp"] = dt;
                return FillItemGrid(dt);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Drawing Approval", ex.Message.ToString());
                return FillItemGrid(dt);
            }
        }
        # endregion
    }
}
