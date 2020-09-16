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
using System.Threading;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Ajax;
using System.Data.SqlClient;

namespace VOMS_ERP.Logistics
{
    public partial class SevottamStatus : System.Web.UI.Page
    {
        # region Variables

        ErrorLog ELog = new ErrorLog();
        SevottamBLL SEBLL = new SevottamBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        SupplierBLL SUPLRBL = new SupplierBLL();
        SevottamBLL SVBLL = new SevottamBLL();
        int UserID;

        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SevottamStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    //if (!IsPostBack)
                    //    GetData();
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        #endregion

        # region Methods

        /// <summary>
        /// Bind DropDownLists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        protected void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt.Tables[0];
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

        private void BindGridView(GridView gv, DataSet ds)
        {
            try
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    gv.DataSource = ds.Tables[0];
                else
                    gv.DataSource = null;
                gv.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        # endregion

        # region GridView Events

        /// <summary>
        /// Sevottam Grid Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottam_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                HiddenField CrtedBy = (HiddenField)e.Row.FindControl("hfCreatedBy");
                HiddenField hfSevottamRefNo = (HiddenField)e.Row.FindControl("hfSevottamRefNo");
                LinkButton HLFUPDT = (LinkButton)e.Row.FindControl("hlfUpdate");
                Label SVTMType = (Label)e.Row.FindControl("lblSvmType");
                Label SVTMID = (Label)e.Row.FindControl("lblLPOrderId");
                Label lblCT1RefDT = (Label)e.Row.FindControl("lblCT1RefDt");
                HLFUPDT.PostBackUrl = SVTMType.Text != "POE/UnUsed" ?
                    "SevottamCTOne.Aspx?ID=" + SVTMID.Text + "&Type=" + SVTMType.Text
                    : "SevottamPOEUpdate.Aspx?ID=" + SVTMID.Text;

                if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                {
                    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                    {
                        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value))))
                        {
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                }
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                if (CommonBLL.DateInsert(lblCT1RefDT.Text.Replace('/', '-')) == CommonBLL.EndDate)
                    lblCT1RefDT.Visible = false;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Sevottam Grid Row Command Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottam_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int res = 1;
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                //string SevID = GVSevottam.DataKeys[index].Values["SevID"].ToString();
                //string Type = ((Label)GVSevottam.Rows[index].FindControl("lblSvmType")).Text.ToString();
                //if (e.CommandName == "Modify")
                //    if (Type == "POE/UnUsed")
                //    {
                //        Response.Redirect("../Logistics/Sevottam.aspx?ID=" + SevID+"&Type="+Type, false);
                //    }
                //    else
                //    {
                //        Response.Redirect("../Logistics/Sevottam.aspx?ID=" + SevID, false);
                //    }
                //else if (e.CommandName == "Remove")
                //{
                //    SevottamBLL SVBLL = new SevottamBLL();
                //    if (Type == "POE/UnUsed")
                //    {
                //        res = SVBLL.InsertUpdateDeleteSvtmPoe(CommonBLL.FlagDelete, Convert.ToInt64(SevID),
                //                    "", "", "", "", "", CommonBLL.EmptyDtSevottamPOE(), CommonBLL.EmptyDtSevCT1Ledger(), Convert.ToInt64(Session["UserID"]));
                //    }
                //    else
                //    {
                //        res = SVBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(SevID),
                //                    "", "", "", "", 0, DateTime.Now, Convert.ToInt64(Session["UserID"]), DateTime.Now, true, CommonBLL.EmptyDtSevottamCT1(), "");
                //    }
                //    if (res == 0)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Successfully Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "Sevottam Status", "Deleted Record " + SevID + " Successfully.");
                //        Search();
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Cannot Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", "Cannot Delete Record " + SevID + ".");
                //    }
                //}                
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Sevottam Grid Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GVSevottam_PreRender(object sender, EventArgs e)
        {
            try
            {
                //GVSevottam.UseAccessibleHeader = false;
                //GVSevottam.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
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

                string CT1RfFrmDt = HFCT1RfFrmDt.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCT1RfFrmDt.Value).ToString("yyyy-MM-dd");
                string CT1RfToDt = HFCT1RfToDt.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFCT1RfToDt.Value).ToString("yyyy-MM-dd");
                string SevttamDftNo = HFSevttamDftNo.Value;
                string Type = HFType.Value;
                string SevttamRefNo = HFSevttamRefNo.Value;
                string CT1DftRfNo = HFCT1DftRfNo.Value;
                string CT1RfNo = HFCT1RfNo.Value;

                if (CT1RfFrmDt == "1-1-0001" || CT1RfFrmDt == "1-1-1900")
                    CT1RfFrmDt = "";
                if (CT1RfToDt == "1-1-0001")
                    CT1RfToDt = "";
                DataSet ds = SVBLL.GetDataForExport(SevttamDftNo, Type, SevttamRefNo, CT1DftRfNo, CT1RfNo, CT1RfFrmDt, CT1RfToDt,new Guid(Session["CompanyID"].ToString())); //, CreatedDT, LoginID
                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=SevottamStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (CT1RfFrmDt != "" && Convert.ToDateTime(CT1RfFrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        CT1RfFrmDt = "";
                    if (CT1RfToDt != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(CT1RfToDt)) == CommonBLL.EndDtMMddyyyy_FS)
                        CT1RfToDt = "";

                    string MTitle = " STATUS OF SEVOTTAM ", MTDTS = "";
                    if (CT1RfFrmDt != "" && CT1RfToDt != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(CT1RfFrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(CT1RfToDt));
                    else if (CT1RfFrmDt != "" && CT1RfToDt == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(CT1RfFrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");
                    htextw.Write("<center><b>" + MTitle + " " + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
                    dgGrid.DataSource = ds.Tables[0];
                    dgGrid.DataBind();
                    Tuple<string, DataGrid> t = CommonBLL.ExcelExportStyle(dgGrid);
                    dgGrid = t.Item2;
                    //Get the HTML for the control.
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
                Session["dsEx"] = null;
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

        #region Web Methods
        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string DeleteItemDetails(string ID, string type, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = "Success";//result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    LEnquiryBLL LEBLL = new LEnquiryBLL();
                    if (type == "POE/UnUsed")
                    {
                        res = SVBLL.InsertUpdateDeleteSvtmPoe(CommonBLL.FlagDelete, new Guid(ID), "", "", "", "", "", CommonBLL.EmptyDtSevottamPOE(),
                            CommonBLL.EmptyDtSevCT1Ledger(), new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()));
                    }
                    else
                    {
                        res = SVBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), "", "", "", "", new Guid(Session["UserID"].ToString()),
                            DateTime.Now, new Guid(Session["UserID"].ToString()), DateTime.Now, true, CommonBLL.EmptyDtSevottamCT1(), "",
                            new Guid(Session["CompanyID"].ToString()));
                    }
                }
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                    result = "Error::Cannot Delete this Record, this is used by another transection/ Error while Deleting ";

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                //return CommonBLL.Can_EditDelete(true, CreatedBy);
                return "Success";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion

    }
}
