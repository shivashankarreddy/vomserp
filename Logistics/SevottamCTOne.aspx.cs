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
using System.Drawing;
using System.Text;

namespace VOMS_ERP.Logistics
{
    public partial class SevottamCTOne : System.Web.UI.Page
    {
        # region Variables
        ErrorLog ELog = new ErrorLog();
        static string aTYPE = "";
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
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        Ajax.Utility.RegisterTypeForAjax(typeof(SevottamCTOne));
                        btnSend.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            if (Request.QueryString.Count > 1)
                                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                                {
                                    BindGridView(new Guid(Request.QueryString["ID"]), Request.QueryString["Type"].ToString());
                                }
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        #endregion

        # region Methods

        private void BindGridView(Guid SevID, string Type)
        {
            try
            {
                SevottamBLL SVBLL = new SevottamBLL();
                DataSet ds = new DataSet();
                ds = SVBLL.GetData(CommonBLL.FlagZSelect, SevID, "", "", Type, "", Guid.Empty, DateTime.Now, Guid.Empty, DateTime.Now, true,
                    CommonBLL.EmptyDtSevottamCT1(), "", new Guid(Session["CompanyID"].ToString()));
                if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    txtSevRefNo.Text = ds.Tables[1].Rows[0]["SevottamRefNo"].ToString();
                    aTYPE = ds.Tables[1].Rows[0]["Type"].ToString();
                    hfType.Value = aTYPE;
                    if (aTYPE == "New")
                    {
                        string[] LPOs = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LPONos")).ToArray(); //LPONos
                        string[] Ioms = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("IOMRefNo")).ToArray(); //IOMRefNo
                        string[] IsFrstTm = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("CT1ReferenceNo")).ToArray();

                        ViewState["LPONos"] = LPOs;
                        ViewState["IOMNos"] = Ioms;
                        ViewState["IsFrstTm"] = IsFrstTm;

                        GVSevottamCTOne.DataSource = ds.Tables[0];
                        GVSevottamCTOne.DataBind();
                    }
                    else if (aTYPE == "Cancel")
                    {
                        GVSevottamCTOne_Cancel.DataSource = ds.Tables[0];
                        GVSevottamCTOne_Cancel.DataBind();
                    }
                    else if (aTYPE == "UnUsed")
                    {
                        gvSevottamCTOne_POEUnUsed.DataSource = ds.Tables[0];
                        gvSevottamCTOne_POEUnUsed.DataBind();
                    }
                }
                else
                {
                    GVSevottamCTOne.DataSource = null;
                    GVSevottamCTOne.DataBind();
                    GVSevottamCTOne_Cancel.DataSource = null;
                    GVSevottamCTOne_Cancel.DataBind();
                    gvSevottamCTOne_POEUnUsed.DataSource = null;
                    gvSevottamCTOne_POEUnUsed.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This is Used to Convert GridView to DataTable
        /// </summary>
        /// <param name="gvCT1"></param>
        /// <returns></returns>
        private DataTable ConvertToDtbl()
        {
            DataTable dt = null;//New
            DataTable dt1 = null;//Cancel
            try
            {
                GridView gvCT1 = null;
                dt = CommonBLL.EmptyDtSevCT1Update();
                dt1 = CommonBLL.EmptyDtSevCT1Update_Cancel();
                DataTable dtt = CommonBLL.EmptyDtSevCT1Ledger();
                if (aTYPE != "" && aTYPE == "New")
                {
                    gvCT1 = GVSevottamCTOne;
                    dt.Rows[0].Delete();
                }
                else if (aTYPE != "" && aTYPE == "Cancel")
                {
                    gvCT1 = GVSevottamCTOne_Cancel;
                    dt1.Rows[0].Delete();
                }
                else if (aTYPE != "" && aTYPE == "UnUsed")
                {
                    gvCT1 = gvSevottamCTOne_POEUnUsed;
                    dt1.Rows[0].Delete();
                }
                dtt.Rows[0].Delete();

                foreach (GridViewRow row in gvCT1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        DataRow dr = null;
                        if (aTYPE == "New")
                            dr = dt.NewRow();
                        else if (aTYPE == "Cancel")
                            dr = dt1.NewRow();
                        else if (aTYPE == "UnUsed")
                            dr = dt1.NewRow();

                        DataRow drr = dtt.NewRow();

                        string CTOneDraftNo = null;
                        string txtCT1RefNo = null;
                        string txtInternalRefNo = null;
                        string txtInternalRefDT = null;
                        string txtNewInternalRefNo = null;
                        string txtNewInternalRefDT = null;
                        string txtBondBalval = null;
                        string txtCT1DT = null;
                        string txtAREForms = null;
                        string hfCT1ID = null;
                        string CT1BondVal = null;
                        string lblCTOneStatus = null;


                        if (aTYPE == "New")
                        {
                            CTOneDraftNo = ((Label)row.FindControl("lblCTOneDraftRefNo")).Text.Trim();
                            txtCT1RefNo = ((TextBox)row.FindControl("txtCTOneRefNo")).Text.Trim();
                            txtInternalRefNo = ((TextBox)row.FindControl("txtInternalRefNo")).Text.Trim();
                            txtInternalRefDT = ((TextBox)row.FindControl("txtInternalRefDT")).Text.Trim();
                            txtCT1DT = ((TextBox)row.FindControl("txtCTOneRefDT")).Text.Trim();
                            txtAREForms = ((TextBox)row.FindControl("txtNoOfAREForms")).Text.Trim();
                        }
                        else if (aTYPE == "Cancel")
                        {
                            CTOneDraftNo = ((HiddenField)row.FindControl("hfCTOneDraftRefNo")).Value.Trim();
                            txtCT1RefNo = ((TextBox)row.FindControl("txtCTOneRefNo")).Text.Trim();
                            txtNewInternalRefNo = ((TextBox)row.FindControl("txtInternalRefNo")).Text.Trim();
                            txtNewInternalRefDT = ((TextBox)row.FindControl("txtInternalRefDT")).Text.Trim();
                        }
                        else if (aTYPE == "UnUsed")
                        {
                            CTOneDraftNo = ((HiddenField)row.FindControl("hfCTOneDraftRefNo")).Value.Trim();
                            txtCT1RefNo = ((Label)row.FindControl("lblCTOneRefNo")).Text.Trim();
                            txtNewInternalRefNo = ((TextBox)row.FindControl("txtInternalRefNo")).Text.Trim();
                            txtNewInternalRefDT = ((TextBox)row.FindControl("txtInternalRefDT")).Text.Trim();
                        }

                        hfCT1ID = ((HiddenField)row.FindControl("hfCTOneID")).Value.Trim();
                        lblCTOneStatus = ((Label)row.FindControl("lblCTOneStatus")).Text.Trim();
                        txtBondBalval = ((TextBox)row.FindControl("txtBondBalanceVal")).Text.Trim();

                        if (aTYPE == "New")
                        {
                            dr["CT1RefNo"] = txtCT1RefNo;
                            dr["CT1InternalRefNo"] = txtInternalRefNo;
                            dr["CT1InternalRefDT"] = CommonBLL.DateInsert(txtInternalRefDT);
                            dr["CT1AREforms"] = (txtAREForms == "" ? 0 : int.Parse(txtAREForms));
                            dr["CT1RefDT"] = (txtCT1DT == "" ? CommonBLL.DateInsert(DateTime.MaxValue.ToShortDateString())
                            : CommonBLL.DateInsert(txtCT1DT));
                            if (txtCT1RefNo == "" && txtCT1DT == "" && txtAREForms == "" || txtAREForms == "0")
                                dr["CT1Status"] = "Draft";
                            else if (lblCTOneStatus == "Confirm" || lblCTOneStatus == "Draft")
                                dr["CT1Status"] = "Confirm";
                            else
                                dr["CT1Status"] = lblCTOneStatus;
                        }
                        else if (aTYPE == "Cancel")
                        {
                            dr["NewInternalRefNo"] = txtNewInternalRefNo;
                            dr["NewInternalRefDT"] = CommonBLL.DateInsert(txtNewInternalRefDT);
                            if (txtNewInternalRefDT == "" || txtNewInternalRefNo == "")
                                dr["CT1Status"] = lblCTOneStatus;
                            else
                                dr["CT1Status"] = "Cancelled";
                        }
                        else if (aTYPE == "UnUsed")
                        {
                            dr["NewInternalRefNo"] = txtNewInternalRefNo;
                            dr["NewInternalRefDT"] = CommonBLL.DateInsert(txtNewInternalRefDT);
                            if (txtNewInternalRefDT == "" || txtNewInternalRefNo == "")
                                dr["CT1Status"] = lblCTOneStatus;
                            else
                                dr["CT1Status"] = "Cancelled";
                        }
                        dr["CT1BondBalVal"] = Convert.ToDecimal(txtBondBalval);
                        dr["CT1ID"] = new Guid(hfCT1ID);

                        if (aTYPE == "New")
                            dt.Rows.Add(dr);
                        else if (aTYPE == "Cancel")
                            dt1.Rows.Add(dr);
                        else if (aTYPE == "UnUsed")
                            dt1.Rows.Add(dr);

                        drr["TransactionDate"] = Convert.ToDateTime(DateTime.MaxValue.ToShortDateString());

                        if (aTYPE == "New")
                        {
                            drr["Description"] = "Charged Against " + aTYPE + " CT-1 (" + txtCT1RefNo + ")";
                            drr["Debit"] = Convert.ToDecimal(((HiddenField)row.FindControl("hfCTOneBondVal")).Value.Trim());
                            drr["Credit"] = 0.00;
                        }
                        else if (aTYPE == "Cancel")
                        {
                            drr["Description"] = "Charged Against " + aTYPE + " CT-1 (" + txtCT1RefNo + ")";
                            drr["Debit"] = 0.00;
                            drr["Credit"] = Convert.ToDecimal(((Label)row.FindControl("lblCTOneBondVal")).Text.Trim());
                        }
                        else if (aTYPE == "UnUsed")
                        {
                            drr["Description"] = "Credited Against POE No. | CT-1 No. | ARE-1 No. :" + aTYPE + " CT-1 (" + txtCT1RefNo + ")";
                            drr["Debit"] = 0.00;
                            drr["Credit"] = Convert.ToDecimal(((Label)row.FindControl("lblCTOneBondVal")).Text.Trim());
                        }
                        drr["CTOneDraftNo"] = CTOneDraftNo;
                        dtt.Rows.Add(drr);
                    }
                }
                Session["CT1Update_New"] = dt;
                Session["CT1Update_Cancel"] = dt1;
                Session["CT1Ledger"] = dtt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
            return dt;
        }


        /// <summary>
        /// Send E-Mails After Generation of GRN 
        /// </summary>
        /// <param name="LpoNumber"></param>
        /// <param name="CT1Number"></param>
        /// <param name="GrnNumber"></param>
        /// <param name="SuplrName"></param>
        /// <param name="PrfmaInvcNmbr"></param>
        protected void SendDefaultMails(DataTable CTOneDtls)
        {
            try
            {
                string[] LPONos = (string[])ViewState["LPONos"];
                string[] IOMNos = (string[])ViewState["IOMNos"];
                string[] IsFrstTm = (string[])ViewState["IsFrstTm"];

                int Count = 0;
                foreach (DataRow Drow in CTOneDtls.Rows)
                {
                    string CcAddrs = "";
                    if (Session["TLMailID"].ToString() == Session["UserMail"].ToString())
                    {
                        //CcAddrs = "satya@bitchemy.com";
                    }
                    else
                    {
                        CcAddrs = Session["TLMailID"].ToString();//"satya@bitchemy.com, " + 
                    }

                    if (String.IsNullOrEmpty(IsFrstTm[Count].ToString().Trim()))
                    {
                        string Rslt1 = CommonBLL.SendMails(Session["UserMail"].ToString(), CcAddrs, "Information about CT-1 Preparation",
                            InformationCTONEDtls(LPONos[Count], IOMNos[Count], Drow["CT1RefNo"].ToString(), CommonBLL.DateDisplay(Convert.ToDateTime(Drow["CT1RefDT"].ToString()))));
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// E-Mail Body Format for Information about CT-1 Details
        /// </summary>
        /// <param name="LPOsDts"></param>
        /// <param name="AREdtls"></param>
        /// <returns></returns>
        protected string InformationCTONEDtls(string LPONos, string IomNmbr, string CT1Nmbr, string Date)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Dear Sir/Madam " + System.Environment.NewLine);
                sb.Append("SUB: CT-1 Prepared " + System.Environment.NewLine);
                sb.Append(" W.R.T your LPO No. " + LPONos + " and IOM No. " + IomNmbr + ", CT-1 No. " + CT1Nmbr + " Dt: " + Date + " is generated. ");
                sb.Append("Please collect the copies from Excise Department and prepare Dispatch Instructions to send it to the Supplier.");

                sb.Append(System.Environment.NewLine + "Regards, ");
                sb.Append(System.Environment.NewLine + "Admin ");
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
            return sb.ToString().Replace("\r\n", " <br /> ");
        }

        # endregion

        # region GridView Events

        protected void GVSevottamCTOne_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblCTOneval = (Label)e.Row.Cells[0].FindControl("lblCTOneval");
                    lblCTOneval.Text = (lblCTOneval.Text != "" || lblCTOneval.Text == "0.00") ?
                        Convert.ToInt64(Convert.ToDecimal(lblCTOneval.Text)).ToString("N") : "0.00";
                    Label lblCT1DraftRefNo = (Label)e.Row.Cells[0].FindControl("lblCTOneDraftRefNo");
                    Label lblCTOneStatus = (Label)e.Row.Cells[0].FindControl("lblCTOneStatus");
                    if (lblCT1DraftRefNo.Text != "" && (lblCTOneStatus.Text == "Confirm" || lblCTOneStatus.Text == "Draft"))
                    {
                        TextBox txtCTOneDt = (TextBox)e.Row.Cells[0].FindControl("txtCTOneRefDT");
                        TextBox txtInternalRefDt = (TextBox)e.Row.Cells[0].FindControl("txtInternalRefDT");

                        if (Convert.ToDateTime(txtCTOneDt.Text).ToString("dd-MM-yyyy") != DateTime.MaxValue.ToString("dd-MM-yyyy"))
                            txtCTOneDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtCTOneDt.Text)).ToString();
                        else
                            txtCTOneDt.Text = "";

                        if (txtInternalRefDt.Text.Trim() != "")
                            txtInternalRefDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtInternalRefDt.Text)).ToString();
                        else
                            txtInternalRefDt.Text = CommonBLL.DateDisplay(DateTime.Now).ToString();
                        txtCTOneDt.Attributes.Add("readonly", "readonly");
                        txtInternalRefDt.Attributes.Add("readonly", "readonly");
                    }
                    else
                    {
                        CheckBox chkClear = (CheckBox)e.Row.Cells[0].FindControl("chkClear");
                        chkClear.Enabled = false;
                        TextBox txtCTOneRefNo = (TextBox)e.Row.Cells[0].FindControl("txtCTOneRefNo");
                        txtCTOneRefNo.Enabled = false;
                        TextBox txtNoOfAREForms = (TextBox)e.Row.Cells[0].FindControl("txtNoOfAREForms");
                        txtNoOfAREForms.Enabled = false;
                        TextBox txtCTOneDt = (TextBox)e.Row.Cells[0].FindControl("txtCTOneRefDT");
                        txtCTOneDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtCTOneDt.Text)).ToString();
                        txtCTOneDt.Enabled = false;
                        txtCTOneDt.Attributes.Add("readonly", "readonly");
                        TextBox txtInternalRefDt = (TextBox)e.Row.Cells[0].FindControl("txtInternalRefDT");
                        txtInternalRefDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtInternalRefDt.Text)).ToString();
                        txtInternalRefDt.Attributes.Add("readonly", "readonly");
                        lblCTOneStatus.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        protected void GVSevottamCTOne_Cancel_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblCTOneval = (Label)e.Row.Cells[0].FindControl("lblCTOneval");
                    lblCTOneval.Text = (lblCTOneval.Text != "" || lblCTOneval.Text == "0.00") ?
                        Convert.ToInt64(Convert.ToDecimal(lblCTOneval.Text)).ToString("N") : "0.00";
                    HiddenField hfCT1DraftRefNo = (HiddenField)e.Row.Cells[0].FindControl("hfCTOneDraftRefNo");
                    Label lblCTOneStatus = (Label)e.Row.Cells[0].FindControl("lblCTOneStatus");
                    TextBox txtNewInternalRefDt = (TextBox)e.Row.Cells[0].FindControl("txtInternalRefDT");
                    Label lblInternalRefDT = (Label)e.Row.Cells[0].FindControl("lblInternalRefDT");

                    if (txtNewInternalRefDt.Text != "")
                        txtNewInternalRefDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtNewInternalRefDt.Text)).ToString();

                    if (lblInternalRefDT.Text.Trim() != "")
                        lblInternalRefDT.Text = CommonBLL.DateDisplay(Convert.ToDateTime(lblInternalRefDT.Text)).ToString();

                    txtNewInternalRefDt.Attributes.Add("readonly", "readonly");
                    lblCTOneStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        protected void gvSevottamCTOne_POEUnUsed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblCTOneval = (Label)e.Row.Cells[0].FindControl("lblCTOneval");
                    lblCTOneval.Text = (lblCTOneval.Text != "" || lblCTOneval.Text == "0.00") ?
                        Convert.ToInt64(Convert.ToDecimal(lblCTOneval.Text)).ToString("N") : "0.00";
                    HiddenField hfCT1DraftRefNo = (HiddenField)e.Row.Cells[0].FindControl("hfCTOneDraftRefNo");
                    Label lblCTOneStatus = (Label)e.Row.Cells[0].FindControl("lblCTOneStatus");
                    TextBox txtNewInternalRefDt = (TextBox)e.Row.Cells[0].FindControl("txtInternalRefDT");
                    Label lblInternalRefDT = (Label)e.Row.Cells[0].FindControl("lblInternalRefDT");

                    if (txtNewInternalRefDt.Text != "")
                        txtNewInternalRefDt.Text = CommonBLL.DateDisplay(Convert.ToDateTime(txtNewInternalRefDt.Text)).ToString();

                    if (lblInternalRefDT.Text.Trim() != "")
                        lblInternalRefDT.Text = CommonBLL.DateDisplay(Convert.ToDateTime(lblInternalRefDT.Text)).ToString();

                    txtNewInternalRefDt.Attributes.Add("readonly", "readonly");
                    lblCTOneStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        # endregion

        # region Button Click Events

        protected void btnSend_Click(object sender, EventArgs e)
        {
            int res = 1;
            try
            {
                SevottamBLL SVBLL = new SevottamBLL();
                DataTable dt = new DataTable();
                dt = ConvertToDtbl();
                DataTable dt1 = new DataTable();
                dt1 = (DataTable)Session["CT1Update_Cancel"];
                DataTable CT1Ledger = new DataTable();
                CT1Ledger = (DataTable)Session["CT1Ledger"];

                if (dt.Rows.Count > 0 && dt1.Rows.Count > 0 && Request.QueryString["ID"] != null && CT1Ledger != null
                && CT1Ledger.Rows.Count > 0)
                {
                    char flag = CommonBLL.FlagUpdate;
                    if (aTYPE == "Cancel")
                        flag = CommonBLL.FlagVUpdate;
                    if (aTYPE == "UnUsed")
                        flag = CommonBLL.FlagVUpdate;
                    res = SVBLL.UpdateSevCTOne(flag, new Guid(Request.QueryString["ID"]), txtSevRefNo.Text.Trim(), txtComments.Text.Trim(),
                        new Guid(Session["UserID"].ToString()), Convert.ToDateTime(DateTime.Now.ToShortDateString()), dt, dt1, CT1Ledger,
                        new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                    {
                        SendDefaultMails((DataTable)Session["CT1Update_New"]);
                        Session["CT1Ledger"] = null;
                        Response.Redirect("../Logistics/SevottamStatus.aspx", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                        "ErrorMessage('Error while updating.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Sevottam CTOne Update", "Cannot Delete Record.");
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Rows to Update.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CT1Ledger"] = null;
                Response.Redirect(HttpContext.Current.Request.Url.ToString(), false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam CTOne Update", ex.Message.ToString());
            }
        }

        # endregion

        # region WebMethods

        /// <summary>
        /// This is used to check Ref.No is in use or not
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagBSelect, RefNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetInternalRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagJSelect, RefNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool GetNewInternalRefNo(string RefNo)
        {
            CheckBLL cbll = new CheckBLL();
            return cbll.CheckSevRefNo(CommonBLL.FlagINewInsert, RefNo, new Guid(HttpContext.Current.Session["CompanyID"].ToString()));
        }
        # endregion
    }
}
