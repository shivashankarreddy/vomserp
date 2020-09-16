using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BAL;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading;
using VOMS_ERP.Admin;

namespace VOMS_ERP.Logistics
{
    public partial class Sevottam : System.Web.UI.Page
    {
        # region Variables
        static string TYPE;
        decimal TotalAmt = 0;
        decimal TotalAREAmt = 0;
        decimal TotalUnUtlsd = 0;
        ErrorLog ELog = new ErrorLog();
        CT1DetailsBLL CT1D = new CT1DetailsBLL();
        SevottamBLL SVBLL = new SevottamBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        static DataTable TbWorkSheetExport;

        static string SevDraftRefNo = "";
        static string PrevCTOneIDs = "";
        static string PresCTOneIDs = "";

        static Dictionary<string, string> CTOneID = new Dictionary<string, string>();
        static Dictionary<string, string> CTPID = new Dictionary<string, string>();
        static Dictionary<string, string> AREOneID = new Dictionary<string, string>();
        # endregion

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
                    Ajax.Utility.RegisterTypeForAjax(typeof(Sevottam));
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                            GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        #endregion

        # region Methods

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
        /// This is Used to Bind Data to Controls
        /// </summary>
        public void GetData()
        {
            try
            {
                ViewState["ChekedItms"] = "";
                if (Request.QueryString.Count > 1 && Request.QueryString["ID"] != null && Request.QueryString["Type"] != null)
                {
                    ViewState["EditID"] = Request.QueryString["ID"].ToString();
                    EditPOERecord(new Guid(Request.QueryString["ID"]), Request.QueryString["Type"].ToString());
                    BindGridView(gvExportWorkSheet, SVBLL.SelectSvtmPoe(CommonBLL.FlagCSelect, Guid.Empty, ViewState["ChekedItms"].ToString(), CommonBLL.EmptyDtSevottamPOE(),
                        Guid.Empty, new Guid(Session["CompanyID"].ToString())));
                    btnExportWorkSht.Enabled = true;
                }
                else if (Request.QueryString.Count > 0 && Request.QueryString["ID"] != null)
                {
                    ViewState["EditID"] = Request.QueryString["ID"].ToString();
                    EditRecord(new Guid(Request.QueryString["ID"]));
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear Controls
        /// </summary>
        private void ClearALL()
        {
            txtReferenceNo.Text = "";
            btnSend.Text = "Generate";
            TYPE = null;
            Session["Sevottamuploads"] = null;
            lblError.Enabled = false;
        }

        /// <summary>
        /// This is used to Bind Gridview
        /// </summary>
        /// <param name="Type"></param>
        private void BindGridView(string Type)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Type == "New")
                    ds = CT1D.GetDataSet(CommonBLL.FlagZSelect, new Guid(Session["CompanyID"].ToString()));
                else if (Type == "Cancel")
                    ds = CT1D.GetDataSet(CommonBLL.FlagXSelect, new Guid(Session["CompanyID"].ToString()));
                else if (Type == "UnUsed")
                    ds = CT1D.GetDataSet(CommonBLL.FlagYSelect, new Guid(Session["CompanyID"].ToString()));

                if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    ViewState["RowCount"] = ds.Tables[1].Rows[0][0].ToString();
                    DataColumn DCCheck = new DataColumn("Check", typeof(string));
                    DCCheck.DefaultValue = 0;
                    ds.Tables[0].Columns.Add(DCCheck);
                    if (Type == "UnUsed")
                    {
                        IEnumerable<DataRow> NonEmptyVals = from E in ds.Tables[0].AsEnumerable()
                                                            where E.Field<decimal>("UnUtilized") != 0
                                                                || E.Field<decimal>("ARE1Value") != 0
                                                            select E;
                        DataTable DataTblNEmpty = NonEmptyVals.CopyToDataTable<DataRow>();
                        gvSevottamPOE.DataSource = DataTblNEmpty;  //ds.Tables[0];
                        gvSevottamPOE.DataBind();
                        TbWorkSheetExport = ds.Tables[0].Copy();
                        dvSevottam.Visible = false;
                        dvSevottamPOE.Visible = true;
                        lblError.Visible = false;

                    }
                    else
                    {
                        gvSevottam.DataSource = ds.Tables[0];
                        gvSevottam.DataBind();
                        dvSevottam.Visible = true;
                        dvSevottamPOE.Visible = false;
                        lblError.Visible = false;
                    }
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "No Records To Display...!";
                    gvSevottam.DataSource = null;
                    gvSevottam.DataBind();
                    gvSevottamPOE.DataSource = null;
                    gvSevottamPOE.DataBind();
                    ViewState["RowCount"] = "1";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        //private void ChangeColNames()
        //{
        //    try
        //    {
        //        if (TbWorkSheetExport != null && TbWorkSheetExport.Rows.Count > 0)
        //        {
        //            if (TbWorkSheetExport.Columns.Contains("CT1ReferenceNo"))
        //                TbWorkSheetExport.Columns["CT1ReferenceNo"].ColumnName = "CT1Number";
        //            if (TbWorkSheetExport.Columns.Contains("RefDate"))
        //                TbWorkSheetExport.Columns["RefDate"].ColumnName = "CT1Date";
        //            if (TbWorkSheetExport.Columns.Contains("CT1BondValue"))
        //                TbWorkSheetExport.Columns["CT1BondValue"].ColumnName = "CT1Value";
        //            if (TbWorkSheetExport.Columns.Contains("ARE1No"))
        //                TbWorkSheetExport.Columns["ARE1No"].ColumnName = "ARE1Number";
        //            if (TbWorkSheetExport.Columns.Contains("ARE1Date"))
        //                TbWorkSheetExport.Columns["ARE1Date"].ColumnName = "ARE1Date";
        //            if (TbWorkSheetExport.Columns.Contains("UnUtilized"))
        //                TbWorkSheetExport.Columns["UnUtilized"].ColumnName = "UnUsedAmt";
        //            if (TbWorkSheetExport.Columns.Contains("UnUtilized"))
        //                TbWorkSheetExport.Columns["UnUtilized"].ColumnName = "UnUsedAmt";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
        //    }
        //}

        /// <summary>
        /// This is used to Bind Gridview
        /// </summary>
        /// <param name="Type"></param>
        private void BindGridView(GridView Gv, DataSet CommonDt)
        {
            try
            {
                if (CommonDt.Tables.Count > 0 && CommonDt.Tables[0].Rows.Count > 0)
                {
                    btnExportWorkSht.Visible = true;
                    if (TYPE == "UnUsed")
                        btnExportWorkSht.Enabled = true;
                    else
                        btnExportWorkSht.Enabled = false;
                    Gv.DataSource = CommonDt.Tables[0];
                }
                else
                    Gv.DataSource = null;
                Gv.DataBind();


            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to Convert GridView to DataTable
        /// </summary>
        /// <param name="gvCT1"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl(GridView gvCT1)
        {
            try
            {
                DataTable dt = CommonBLL.EmptyDtSevottamCT1();
                dt.Rows[0].Delete(); int exdt = 0, dscnt = 0;
                foreach (GridViewRow row in gvCT1.Rows)
                {
                    DataRow dr;
                    if (((CheckBox)row.FindControl("chkCTID")).Checked)
                    {
                        dr = dt.NewRow();
                        dr["SevID"] = Guid.Empty;
                        dr["SevDraftRefNo"] = "";
                        dr["CT1ID"] = new Guid(((HiddenField)row.FindControl("HFCT1ID")).Value.ToString());
                        PresCTOneIDs += ((HiddenField)row.FindControl("HFCT1ID")).Value.ToString() + ",";
                        dt.Rows.Add(dr);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// This is Used to Convert GridView to DataTable
        /// </summary>
        /// <param name="gvCT1"></param>
        /// <returns></returns>
        private DataTable ConvertToDtblSvtPoe(GridView gvCT1)
        {
            DataTable dt = CommonBLL.EmptyDtSevottamPOE();
            dt.Rows[0].Delete();
            foreach (GridViewRow row in gvCT1.Rows)
            {
                DataRow dr;
                if (((CheckBox)row.FindControl("chkCTID")).Checked)
                {
                    dr = dt.NewRow();

                    dr["CT1ID"] = new Guid(((HiddenField)row.FindControl("HFCT1ID")).Value);
                    dr["ARE1ID"] = new Guid(((HiddenField)row.FindControl("HFARE1ID")).Value);
                    dr["CT1TrackingID"] = new Guid(((HiddenField)row.FindControl("HFCTTrackingID")).Value);
                    dr["SevottamPOENmbr"] = "";
                    dr["SevottamRefNmbr"] = "";
                    dr["CT1Number"] = ((Label)row.FindControl("lblCTNo")).Text;
                    dr["CT1Date"] = CommonBLL.DateInsert1(((Label)row.FindControl("lblctdate")).Text);
                    dr["CT1Value"] = Convert.ToDecimal(((Label)row.FindControl("lblCTValue")).Text);
                    dr["ARE1Number"] = ((Label)row.FindControl("lblareNo")).Text;
                    dr["ARE1Date"] = CommonBLL.DateInsert1(((Label)row.FindControl("lblaredate")).Text);
                    dr["ARE1Value"] = Convert.ToDecimal(((Label)row.FindControl("lblareval")).Text);
                    dr["UnUtilizedAmt"] = Convert.ToDecimal(((Label)row.FindControl("lblUnUtlzd")).Text);
                    dr["Supplier"] = new Guid(((HiddenField)row.FindControl("HFSUPLRID")).Value);
                    dr["ECCNo"] = ((Label)row.FindControl("lblEccno")).Text;
                    dr["POENumber"] = "";
                    dr["POEDate"] = CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"));
                    dr["POEAmtCrtd"] = 0;
                    ViewState["SevotamIDnty"] = ((Label)row.FindControl("lblSvtmID")).Text;
                    PresCTOneIDs += ((HiddenField)row.FindControl("HFCT1ID")).Value + ",";
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>
        /// This is Used to EDIT Record
        /// </summary>
        /// <param name="ID"></param>
        private void EditRecord(Guid ID)
        {
            try
            {
                pnlActions.Visible = false;
                DataSet ds = new DataSet();
                ds = SVBLL.GetData(CommonBLL.FlagModify, ID, "", "", "", "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true,
                    CommonBLL.EmptyDtSevottamCT1(), "", new Guid(Session["CompanyID"].ToString()));

                if (ds.Tables.Count > 2 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0 && ds.Tables[2].Rows.Count > 0)
                {
                    txtReferenceNo.Text = ds.Tables[0].Rows[0]["SevottamRefNo"].ToString();
                    TYPE = ds.Tables[0].Rows[0]["Type"].ToString();
                    SevDraftRefNo = ds.Tables[0].Rows[0]["SevottamDraftRefNo"].ToString();
                    if (ds.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((ds.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["Sevottamuploads"] = attms;
                        divListBox1.InnerHtml = AttachedFiles();
                    }

                    DataColumn DCCheck = new DataColumn("Check", typeof(string));
                    DCCheck.DefaultValue = 0;
                    ds.Tables[1].Columns.Add(DCCheck);
                    for (int k = 0; k < ds.Tables[1].Rows.Count; k++)
                    {
                        Guid CT1Id = new Guid(ds.Tables[1].Rows[k]["CT1ID"].ToString());
                        DataRow[] foundRows = ds.Tables[2].Select("CT1ID = '" + CT1Id + "'");

                        if (foundRows.Length > 0)
                        {
                            PrevCTOneIDs += CT1Id + ",";
                            ds.Tables[1].Rows[k]["Check"] = 1;
                        }
                    }
                    if (TYPE == "New" || TYPE == "Cancel")
                    {
                        ds.Tables[1].AcceptChanges();
                        gvSevottam.DataSource = ds.Tables[1];
                        gvSevottam.DataBind();
                        dvSevottam.Visible = true;
                    }
                    btnSend.Text = "Update";
                    txtReferenceNo.Enabled = true;
                    DivAttachments.Visible = true;
                    DivComments.Visible = true;
                }
                else
                {
                    gvSevottam.DataSource = null;
                    gvSevottam.DataBind();
                    btnSend.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Edit POE/UnUtilized Records
        /// </summary>
        /// <param name="SvtmId"></param>
        /// <param name="SvtmType"></param>
        private void EditPOERecord(Guid SvtmId, string SvtmType)
        {
            try
            {
                rbgPOE.Checked = true;
                pnlActions.Visible = false;
                DataSet ds = SVBLL.SelectSvtmPoe(CommonBLL.FlagModify, SvtmId, SvtmType, CommonBLL.EmptyDtSevottamPOE(), Guid.Empty,
                    new Guid(Session["CompanyID"].ToString()));

                if (ds.Tables.Count > 2 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0 && ds.Tables[2].Rows.Count > 0)
                {
                    txtReferenceNo.Text = ds.Tables[0].Rows[0]["SevottamRefNo"].ToString();
                    TYPE = ds.Tables[0].Rows[0]["Type"].ToString();
                    SevDraftRefNo = ds.Tables[0].Rows[0]["SevottamDraftRefNo"].ToString();
                    if (ds.Tables[0].Rows[0]["Attachments"].ToString() != "")
                    {
                        ArrayList attms = new ArrayList();
                        attms.AddRange((ds.Tables[0].Rows[0]["Attachments"].ToString().Split(',')).ToArray());
                        Session["Sevottamuploads"] = attms;
                        divListBox1.InnerHtml = AttachedFiles();
                    }

                    DataColumn DCCheck = new DataColumn("Check", typeof(string));
                    DCCheck.DefaultValue = 0;
                    ds.Tables[2].Columns.Add(DCCheck);
                    for (int k = 0; k < ds.Tables[2].Rows.Count; k++)
                    {
                        Guid CT1Id = new Guid(ds.Tables[2].Rows[k]["CT1ID"].ToString());
                        DataRow[] foundRows = ds.Tables[1].Select("CT1ID = '" + CT1Id + "'");

                        if (foundRows.Length > 0)
                        {
                            PrevCTOneIDs += CT1Id + ",";
                            ds.Tables[2].Rows[k]["Check"] = 1;
                        }
                    }
                    if (TYPE == "UnUsed")
                    {
                        ds.Tables[2].AcceptChanges();
                        gvSevottamPOE.DataSource = ds.Tables[2];
                        gvSevottamPOE.DataBind();
                        dvSevottam.Visible = true;
                    }
                    btnSend.Text = "Update";
                    txtReferenceNo.Enabled = true;
                    DivAttachments.Visible = true;
                    dvSevottamPOE.Visible = true;
                    DivComments.Visible = true;
                }
                else
                {
                    gvSevottam.DataSource = null;
                    gvSevottam.DataBind();
                    btnSend.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
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
                if (AsyncFileUpload12.HasFile)
                {
                    ArrayList alist = new ArrayList();
                    string strPath = MapPath("~/uploads/");// +Path.GetFileName(AsyncFileUpload1.FileName);
                    string FileNames = CommonBLL.Replace(AsyncFileUpload12.FileName);
                    if (Session["Sevottamuploads"] != null)
                    {
                        alist = (ArrayList)Session["Sevottamuploads"];
                        if (!alist.Contains(FileNames))
                            alist.Add(FileNames);
                    }
                    else if (Session["Sevottamuploads"] == null)
                    {
                        alist.Add(FileNames);
                    }
                    Session["Sevottamuploads"] = alist;
                    AsyncFileUpload12.SaveAs(strPath + FileNames);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
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
                if (Session["Sevottamuploads"] != null)
                {
                    ArrayList all = new ArrayList();
                    all = (ArrayList)Session["Sevottamuploads"];
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
                return ex.Message;
            }
        }

        # endregion

        # region ClickEvents

        /// <summary>
        /// This is Used to fetch According to Selected RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbgNew_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                BindGridView("New");
                TYPE = "New";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to fetch According to Selected RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbgCancel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                BindGridView("Cancel");
                TYPE = "Cancel";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to Get unused Amount from GRN/GDN via CT-1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbgUnUtilize_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                BindGridView("UnUsed");
                TYPE = "UnUsed";
                btnExportWorkSht.Visible = true;
                btnExportWorkSht.Enabled = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to save record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSend_Click(object sender, EventArgs e)
        {
            int res = 1; Filename = FileName();
            try
            {
                string Atchmnts = Session["Sevottamuploads"] == null ? "" :
                        string.Join(",", ((ArrayList)Session["Sevottamuploads"]).Cast<string>().ToArray());
                DataTable SevDT = new DataTable();
                if (btnSend.Text == "Generate")
                {
                    if (rbgPOE.Checked == true)
                    {
                        SevDT = ConvertToDtblSvtPoe(gvSevottamPOE);
                        Session["SevCT1Updatepoe"] = SevDT;

                        SevDraftRefNo = "SEVPOE-UnUtl/" + Session["AliasName"].ToString() + "/" + ViewState["SevotamIDnty"].ToString()
                            + "/" + CommonBLL.FinacialYearShort + "";

                        res = SVBLL.InsertUpdateDeleteSvtmPoe(CommonBLL.FlagNewInsert, Guid.Empty, SevDraftRefNo, "", TYPE, Atchmnts, "",
                            SevDT, CommonBLL.EmptyDtSevCT1Ledger(), new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    }
                    else
                    {
                        SevDT = ConvertToDtbl(gvSevottam);
                        Session["SevCT1Update"] = SevDT;

                        SevDraftRefNo = "SEV/" + Session["AliasName"].ToString() + "/" + ViewState["RowCount"].ToString() + "/" + CommonBLL.FinacialYearShort + "";

                        res = SVBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, SevDraftRefNo, txtReferenceNo.Text.Trim(), TYPE, "",
                            new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, SevDT, "",
                            new Guid(Session["CompanyID"].ToString()));
                    }
                    if (res == 0)
                    {

                        ALS.AuditLog(res, "Save", "", "Sevottam", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Response.Redirect("../Logistics/SevottamStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, "Save", "", "Sevottam", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", "Error while Inserting.");
                    }
                }
                else if (btnSend.Text == "Update")
                {
                    if (rbgPOE.Checked == true)
                    {
                        SevDT = ConvertToDtblSvtPoe(gvSevottamPOE);
                        Session["SevCT1Updatepoe"] = SevDT;

                        SevDraftRefNo = "SEVPOE-UnUtl/" + Session["AliasName"].ToString() + "/" + ViewState["SevotamIDnty"].ToString()
                            + "/" + CommonBLL.FinacialYearShort + "";

                        res = SVBLL.InsertUpdateDeleteSvtmPoe(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), SevDraftRefNo,
                            txtReferenceNo.Text, TYPE, Atchmnts, txtComments.Text, SevDT, CommonBLL.EmptyDtSevCT1Ledger(),
                            new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                    }
                    else
                    {
                        SevDT = ConvertToDtbl(gvSevottam);

                        res = SVBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()), SevDraftRefNo, txtReferenceNo.Text.Trim(),
                            TYPE, Atchmnts, Guid.Empty, DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, SevDT,
                            txtComments.Text.Trim(), new Guid(Session["CompanyID"].ToString()));
                        if (TYPE == "Cancel" && res == 0)
                        {

                        }
                    }
                    if (res == 0)
                    {
                        ALS.AuditLog(res, "Update", "", "Sevottam", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        Session["SevCT1Update"] = null;
                        Response.Redirect("../Logistics/SevottamStatus.aspx", false);
                    }
                    else
                    {
                        ALS.AuditLog(res, "Update", "", "Sevottam", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", "Error while Updating.");
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to clear the Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearALL();
            Response.Redirect("../Logistics/Sevottam.aspx", false);
        }

        protected void OnButtonClicked(string Action)
        {
            Response.Redirect(Request.Url.ToString(), false);
        }

        # endregion

        # region GridViewEvents

        /// <summary>
        /// GridView Row DataBound for Sevottam New/Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSevottam_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (e.Row.RowType != DataControlRowType.DataRow) return;
                if (Request.QueryString["ID"] != null)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        HiddenField EditCheck = (HiddenField)e.Row.Cells[0].FindControl("HFEditCheck");
                        CheckBox chkBx = (CheckBox)e.Row.Cells[0].FindControl("chkCTID");
                        if (EditCheck.Value != "" && EditCheck.Value == "1")
                        {
                            //chkBx.Enabled = false;
                            chkBx.Checked = true;
                            Label lblCT1Val = (Label)e.Row.FindControl("lblCTValue");
                            TotalAmt += Convert.ToDecimal(lblCT1Val.Text);
                        }
                        //else                        
                        //    chkBx.Enabled = false;
                        if (txtReferenceNo.Text.Trim() != "")
                            chkBx.Enabled = false;
                    }
                    if (e.Row.RowType == DataControlRowType.Footer)
                    {
                        Label lblTotalVal = (Label)e.Row.FindControl("lblTotalVal");
                        lblTotalVal.Text = (decimal.Round(Convert.ToDecimal(TotalAmt), 0, MidpointRounding.AwayFromZero)).ToString("N");
                    }
                }

                Label lblCTOneval = (Label)e.Row.Cells[0].FindControl("lblCTValue");
                if (lblCTOneval != null)
                    lblCTOneval.Text = (lblCTOneval.Text != "" || lblCTOneval.Text == "0.00") ?
                        Convert.ToInt64(Convert.ToDecimal(lblCTOneval.Text)).ToString("N") : "0.00";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView Row DataBound for Sevottam POE/UnUtilized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvSevottamPOE_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Request.QueryString["ID"] != null)
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        string ChectedItms = ViewState["ChekedItms"].ToString();
                        HiddenField lblCTOneID = (HiddenField)e.Row.FindControl("HFCT1ID");
                        HiddenField EditCheck = (HiddenField)e.Row.Cells[0].FindControl("HFEditCheck");
                        CheckBox chkBx = (CheckBox)e.Row.Cells[0].FindControl("chkCTID");
                        if (EditCheck.Value != "" && EditCheck.Value == "1")
                        {
                            chkBx.Checked = true;
                            Label lblCT1Val = (Label)e.Row.FindControl("lblCTValue");
                            decimal UnUtlzdAmnt = Convert.ToDecimal(((Label)e.Row.FindControl("lblUnUtlzd")).Text);
                            decimal ARE1Amount = Convert.ToDecimal(((Label)e.Row.FindControl("lblareval")).Text);
                            TotalAmt += Convert.ToDecimal(lblCT1Val.Text);
                            TotalAREAmt += ARE1Amount;
                            TotalUnUtlsd += UnUtlzdAmnt;

                            ChectedItms = ChectedItms + "," + lblCTOneID.Value;
                            ViewState["ChekedItms"] = ChectedItms.TrimStart(',');
                            btnExportWorkSht.Enabled = true;
                        }
                        if (txtReferenceNo.Text.Trim() != "")
                            chkBx.Enabled = false;
                    }
                    if (e.Row.RowType == DataControlRowType.Footer)
                    {
                        Label lblCTTotal = (Label)e.Row.FindControl("lblCTTotalVal");
                        Label lblARETotal = (Label)e.Row.FindControl("lblARETotalVal");
                        Label lblUnUtlsdTotal = (Label)e.Row.FindControl("lblUnUtlsdTotalVal");
                        Label lblTotAmounText = (Label)e.Row.FindControl("lblTotAmount");
                        lblTotAmounText.Text = "Total Amount("+Session["CurrencySymbol"].ToString().Trim()+") :";
                        //lblCTTotal.Text = (decimal.Round(Convert.ToDecimal(TotalAmt), 0, MidpointRounding.AwayFromZero)).ToString("N");
                        lblARETotal.Text = (decimal.Round(Convert.ToDecimal(TotalAREAmt), 0, MidpointRounding.AwayFromZero)).ToString("N");
                        lblUnUtlsdTotal.Text = (decimal.Round(Convert.ToDecimal(TotalUnUtlsd), 0, MidpointRounding.AwayFromZero)).ToString("N");
                    }
                }

                Label lblCTOneval = (Label)e.Row.Cells[0].FindControl("lblCTValue");
                if (lblCTOneval != null)
                    lblCTOneval.Text = (lblCTOneval.Text != "" || lblCTOneval.Text == "0.00") ?
                        Convert.ToInt64(Convert.ToDecimal(lblCTOneval.Text)).ToString("N") : "0.00";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Export to WorkSheet Grid Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvExportWorkSheet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;

                Label Ct1Nmbr = (Label)e.Row.FindControl("lblCt1Nmbr");
                Label UnUtlized = (Label)e.Row.FindControl("lblUnUsedValue");
                Label TriplicateCopy = (Label)e.Row.FindControl("lblTCRValue");

                if (ViewState["LastCt1Nmbr"] == null)
                {
                    ViewState["LastCt1Nmbr"] = Ct1Nmbr.Text;
                }
                else if (ViewState["LastCt1Nmbr"].ToString() == Ct1Nmbr.Text)
                {
                    UnUtlized.Text = " - ";
                    ViewState["LastCt1Nmbr"] = Ct1Nmbr.Text;
                }
                else if (ViewState["LastCt1Nmbr"].ToString() != Ct1Nmbr.Text)
                {
                    ViewState["LastCt1Nmbr"] = Ct1Nmbr.Text;
                }
                TriplicateCopy.Text = (TriplicateCopy.Text.Split(',')[2].ToString().Trim());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to WorkSheet Grid Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvExportWorkSheet_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (gvExportWorkSheet.HeaderRow == null) return;
                gvExportWorkSheet.UseAccessibleHeader = false;
                gvExportWorkSheet.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is used to Select CT1 in the Gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkCTID_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbgPOE.Checked == true)
                {
                    string ChectedItms = ViewState["ChekedItms"].ToString();
                    CheckBox chkStatus = (CheckBox)sender;
                    GridViewRow row = (GridViewRow)chkStatus.NamingContainer;
                    HiddenField lblCTOneID = (HiddenField)row.FindControl("HFCT1ID");
                    HiddenField HFCTTrackingID = (HiddenField)row.FindControl("HFCTTrackingID");
                    HiddenField HFARE1ID = (HiddenField)row.FindControl("HFARE1ID");

                    Label lblARETotal = (Label)gvSevottamPOE.FooterRow.FindControl("lblARETotalVal");
                    decimal CT1Amount = Convert.ToDecimal(((Label)row.FindControl("lblCTValue")).Text);
                    decimal ARE1Amount = Convert.ToDecimal(((Label)row.FindControl("lblareval")).Text);
                    decimal UnUtlzdAmnt = Convert.ToDecimal(((Label)row.FindControl("lblUnUtlzd")).Text);
                    Label lblUnUtlsdTotl = (Label)gvSevottamPOE.FooterRow.FindControl("lblUnUtlsdTotalVal");
                    decimal TotalUnUtlsd = Convert.ToDecimal(lblUnUtlsdTotl.Text);
                    decimal TotalAREVal = Convert.ToDecimal(lblARETotal.Text);
                    decimal GrandCTTotal = 0, GrandARETotal = 0, GrandUnUtlsd = 0;

                    if (chkStatus.Checked)
                    {
                        GrandARETotal = (TotalAREVal + ARE1Amount);
                        lblARETotal.Text = GrandARETotal.ToString("N");
                        GrandUnUtlsd = (TotalUnUtlsd + UnUtlzdAmnt);
                        lblUnUtlsdTotl.Text = GrandUnUtlsd.ToString("N");
                        ChectedItms = ChectedItms + "," + lblCTOneID.Value;
                        ViewState["ChekedItms"] = ChectedItms.TrimStart(',');

                        if (!CTOneID.ContainsKey(lblCTOneID.Value))
                            CTOneID.Add(lblCTOneID.Value, lblCTOneID.Value);
                        if (!CTPID.ContainsKey(HFCTTrackingID.Value))
                            CTPID.Add(HFCTTrackingID.Value, HFCTTrackingID.Value);
                        if (!AREOneID.ContainsKey(HFARE1ID.Value) && HFARE1ID.Value != "0")
                            AREOneID.Add(HFARE1ID.Value, HFARE1ID.Value);
                    }
                    else
                    {
                        chkStatus.Checked = false;
                        GrandARETotal = (TotalAREVal - ARE1Amount);
                        lblARETotal.Text = GrandARETotal.ToString("N");
                        GrandUnUtlsd = (TotalUnUtlsd - UnUtlzdAmnt);
                        lblUnUtlsdTotl.Text = GrandUnUtlsd.ToString("N");
                        ChectedItms = ViewState["ChekedItms"].ToString();

                        if (ChectedItms.Contains(lblCTOneID.Value))
                        {
                            string[] Chkditms = ChectedItms.Split(',');
                            List<string> list = new List<string>(Chkditms);
                            list.Remove(lblCTOneID.Value.ToString());
                            Chkditms = list.ToArray();
                            ChectedItms = String.Join(",", Chkditms);
                            ChectedItms.TrimEnd(',').TrimStart(',').Replace(",,", ",");
                            ChectedItms.Trim();
                        }

                        ViewState["ChekedItms"] = ChectedItms;

                        if (CTOneID.ContainsKey(lblCTOneID.Value))
                            CTOneID.Remove(lblCTOneID.Value);
                        if (CTPID.ContainsKey(HFCTTrackingID.Value))
                            CTPID.Remove(HFCTTrackingID.Value);
                        if (AREOneID.ContainsKey(HFARE1ID.Value))
                            AREOneID.Remove(HFARE1ID.Value);
                    }

                    string CTOneIDs = string.Join(",", CTOneID.Select(x => x.Value).ToArray());
                    string CTPIDs = string.Join(",", CTPID.Select(x => x.Value).ToArray());
                    string AREOneIDs = string.Join(",", AREOneID.Select(x => x.Value).ToArray());

                    BindGridView(gvExportWorkSheet,
                        SVBLL.SelectSvtmPoe_Export(CommonBLL.FlagESelect, Guid.Empty, ChectedItms, CTPIDs, AREOneIDs, CommonBLL.EmptyDtSevottamPOE(), Guid.Empty,new Guid(Session["CompanyId"].ToString())));
                }
                else
                {
                    CheckBox chkStatus = (CheckBox)sender;
                    GridViewRow row = (GridViewRow)chkStatus.NamingContainer;
                    Label lblCT1Val = (Label)row.FindControl("lblCTValue");
                    HiddenField HFCT1Val = (HiddenField)row.FindControl("HFCT1ID");
                    DataSet ds = SVBLL.SevottamStatus(CommonBLL.FlagASelect, new Guid(HFCT1Val.Value.ToString()), TYPE, Guid.Empty, Guid.Empty, CommonBLL.StartDate, CommonBLL.EndDate);

                    Label lblTotal = (Label)gvSevottam.FooterRow.FindControl("lblTotalVal");
                    decimal Ct1Amount = Convert.ToDecimal(lblCT1Val.Text);
                    decimal TotalVal = Convert.ToDecimal(lblTotal.Text);
                    decimal GrandTotal = 0;

                    if (chkStatus.Checked)
                    {
                        GrandTotal = (TotalVal + Ct1Amount);
                        lblTotal.Text = GrandTotal.ToString("N");
                    }
                    else
                    {
                        chkStatus.Checked = false;
                        GrandTotal = (TotalVal - Ct1Amount);
                        lblTotal.Text = GrandTotal.ToString("N");
                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (Request.QueryString["ID"] != null && ds.Tables[0].Rows[0]["CT1ID"].ToString() == HFCT1Val.Value
                            && ds.Tables[0].Rows[0]["SevID"].ToString() != Request.QueryString["ID"])
                        {
                            chkStatus.Checked = false;
                            GrandTotal = (GrandTotal - Ct1Amount);
                            lblTotal.Text = GrandTotal.ToString("N");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                                "ShowAll", "ErrorMessage('This CT-1 was saved in another Sevottam.');", true);
                        }
                        else if (Request.QueryString["ID"] == null && ds.Tables[0].Rows[0]["CT1ID"].ToString() == HFCT1Val.Value)
                        {
                            chkStatus.Checked = false;
                            GrandTotal = (GrandTotal - Ct1Amount);
                            lblTotal.Text = GrandTotal.ToString("N");
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                                "ShowAll", "ErrorMessage('This CT-1 was saved in another Sevottam.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMethods

        /// <summary>
        /// Add Attachemts to List Box
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string AddItemListBox()
        {
            return AttachedFiles();
        }

        /// <summary>
        /// Delete Attachemts from List Box
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string DeleteItemListBox(int ID)
        {
            try
            {
                ArrayList all = (ArrayList)Session["Sevottamuploads"];
                all.RemoveAt(ID);
                return AttachedFiles();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                return ex.Message;
            }
        }

        /// <summary>
        /// This is used to get SEV Ref.No
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetSevRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagWCommonMstr, RefNo, new Guid(Session["CompanyID"].ToString()));
        }
        # endregion

        #region Export Buttons Click Events

        /// <summary>
        /// Generate WorkSheet Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportWorkSht_Click(object sender, EventArgs e)
        {
            try
            {
                gvExportWorkSheet.Visible = true;
                string attachment = "attachment; filename=WorkSheet.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/ms-excel";
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                gvExportWorkSheet.RenderControl(htextw);
                Response.Write(stw.ToString());
                Response.End();
                gvExportWorkSheet.Visible = false;
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
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
    }
}