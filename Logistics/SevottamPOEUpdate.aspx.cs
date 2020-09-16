using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using BAL;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Threading;

namespace VOMS_ERP.Logistics
{
    public partial class SevottamPOEUpdate : System.Web.UI.Page
    {

        #region Variables
        int res = 999;
        ErrorLog ELog = new ErrorLog();
        SevottamBLL SVPEBL = new SevottamBLL();
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
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            GetData();
                        }
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Bind Default Data
        /// </summary>
        protected void GetData()
        {
            try
            {
                if (((Request.QueryString["ID"] != null && Request.QueryString["ID"] != "") ?
                    new Guid(Request.QueryString["ID"].ToString()) : Guid.Empty) != Guid.Empty)
                {
                    ViewState["SvtmID"] = Request.QueryString["ID"].ToString();
                    FillInputFields(SVPEBL.SelectSvtmPoe(CommonBLL.FlagASelect, new Guid(Request.QueryString["ID"].ToString()),
                        "UnUsed", CommonBLL.EmptyDtSevottamPOE(), Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        private void FillInputFields(DataSet Commondt)
        {
            try
            {
                if (Commondt.Tables.Count > 1)
                {
                    if (Commondt.Tables[1].Rows.Count > 0)
                    {
                        txtSevRefNo.Text = Commondt.Tables[1].Rows[0]["SevottamRefNo"].ToString();
                        txtComments.Text = Commondt.Tables[1].Rows[0]["Comments"].ToString();
                        ViewState["SevottamDraftRefNo"] = Commondt.Tables[1].Rows[0]["SevottamDraftRefNo"].ToString();
                    }
                    BindGridView(GVSevottamPOE, Commondt.Tables[0]);
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        protected void ClearInputs()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Grid View Using DataSet
        /// </summary>
        /// <param name="gview"></param>
        /// <param name="EnqRpt"></param>
        private void BindGridView(GridView gview, DataTable Rceds)
        {
            try
            {
                if (Rceds.Rows.Count > 0)
                {
                    gview.DataSource = Rceds;
                    gview.DataBind();
                }
                else
                {
                    gview.DataSource = null;
                    gview.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
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
            try
            {

                DataTable dtt = CommonBLL.EmptyDtSevCT1Ledger();
                dt.Rows[0].Delete();
                dtt.Rows[0].Delete();
                foreach (GridViewRow row in gvCT1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    DataRow drr = dtt.NewRow();

                    dr["CT1ID"] = Convert.ToInt32(((HiddenField)row.FindControl("HFCT1ID")).Value);
                    dr["ARE1ID"] = Convert.ToInt32(((HiddenField)row.FindControl("HFARE1ID")).Value);
                    dr["CT1TrackingID"] = Convert.ToInt32(((HiddenField)row.FindControl("HFCTTrackingID")).Value);
                    dr["SevottamPOENmbr"] = ""; //Draft Number
                    dr["SevottamRefNmbr"] = txtSevRefNo.Text;
                    dr["CT1Number"] = ((Label)row.FindControl("lblCTNo")).Text;
                    dr["CT1Date"] = CommonBLL.DateInsert1(((Label)row.FindControl("lblctdate")).Text);
                    dr["CT1Value"] = Convert.ToDecimal(((Label)row.FindControl("lblCTValue")).Text);
                    dr["ARE1Number"] = ((Label)row.FindControl("lblareNo")).Text;
                    dr["ARE1Date"] = CommonBLL.DateInsert1(((Label)row.FindControl("lblaredate")).Text);
                    dr["ARE1Value"] = Convert.ToDecimal(((Label)row.FindControl("lblareval")).Text);
                    dr["UnUtilizedAmt"] = Convert.ToDecimal(((Label)row.FindControl("lblUnUtlzd")).Text);
                    dr["Supplier"] = Convert.ToInt32(((HiddenField)row.FindControl("HFSUPLRID")).Value);
                    dr["ECCNo"] = ((Label)row.FindControl("lblEccno")).Text;
                    dr["POENumber"] = ((TextBox)row.FindControl("txtPOENo")).Text;
                    dr["POEDate"] = CommonBLL.DateInsert(((TextBox)row.FindControl("txtDT")).Text.Replace("/", "-"));
                    dr["POEAmtCrtd"] = Convert.ToDecimal(((TextBox)row.FindControl("txtAmtCredited")).Text);
                    ViewState["SevotamIDnty"] = ((Label)row.FindControl("lblSvtmID")).Text;
                    dt.Rows.Add(dr);

                    string txtCT1RefNo = ((Label)row.FindControl("lblCTNo")).Text.Trim();
                    string txtARERefNo = ((Label)row.FindControl("lblareNo")).Text.Trim();
                    string txtPOENo = ((TextBox)row.FindControl("txtPOENo")).Text.Trim();

                    drr["TransactionDate"] = CommonBLL.DateInsert(((TextBox)row.FindControl("txtDT")).Text.Replace("/", "-"));
                    drr["Description"] = "Credited Against POE No. | CT-1 No. | ARE-1 No. : " + txtPOENo + " | " + txtCT1RefNo + " | " + txtARERefNo;
                    drr["Debit"] = 0.00;
                    drr["Credit"] = Convert.ToDecimal(((TextBox)row.FindControl("txtAmtCredited")).Text.Trim());
                    drr["CTOneDraftNo"] = txtCT1RefNo;

                    dtt.Rows.Add(drr);

                }
                Session["CT1Ledger"] = dtt;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
            return dt;
        }



        #endregion

        #region Grid View Events
        /// <summary>
        /// Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottamPOE_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int lastCellIndex = e.Row.Cells.Count - 1;
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                else
                {
                    TextBox TxtDate = (TextBox)e.Row.FindControl("txtDT");
                    TxtDate.Attributes.Add("readonly", "readonly");
                    ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottamPOE_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = int.Parse(e.CommandArgument.ToString());
                GridViewRow gvrow = GVSevottamPOE.Rows[index];
                int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblgdnID")).Text);
                if (e.CommandName == "Modify")
                    Response.Redirect("../Invoices/MateReceipt.Aspx?ID=" + ID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                if (ErrMsg != "Thread was being aborted.")
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottamPOE_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (GVSevottamPOE.HeaderRow == null) return;
                GVSevottamPOE.UseAccessibleHeader = false;
                GVSevottamPOE.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events
        /// <summary>
        /// Search Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable SevDT = ConvertToDtblSvtPoe(GVSevottamPOE);
                Session["SevCT1Updatepoe"] = SevDT;
                DataTable CT1Ledger = (DataTable)Session["CT1Ledger"];

                res = SVPEBL.InsertUpdateDeleteSvtmPoe(CommonBLL.FlagUpdate, new Guid(ViewState["SvtmID"].ToString()),
                    ViewState["SevottamDraftRefNo"].ToString(), txtSevRefNo.Text, "UnUsed", "", txtComments.Text, SevDT, CT1Ledger,
                    new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString()));
                if (res == 0)
                {
                    Response.Redirect("../Logistics/SevottamStatus.aspx", false);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                            "ErrorMessage('Error while Inserting.');", true);
                    ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", "Error while Inserting.");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear Input Fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //DataTable SevDT = ConvertToDtblSvtPoe(GVSevottamPOE);
                ClearInputs();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNmbr = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
            }
        }
        #endregion

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
                string attachment = "attachment; filename=Enquiry.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/ms-excel";
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                GVSevottamPOE.RenderControl(htextw);
                Response.Write(stw.ToString());
                Response.End();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam POE-UnUtilized Update", ex.Message.ToString());
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