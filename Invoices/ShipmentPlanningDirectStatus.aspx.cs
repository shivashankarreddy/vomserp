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

namespace VOMS_ERP.Invoices
{
    public partial class ShipmentPlanningDirectStatus : System.Web.UI.Page
    {
        # region Variables

        ErrorLog ELog = new ErrorLog();
        CustomerBLL CSTMRBL = new CustomerBLL();
        SupplierBLL SUPLRBL = new SupplierBLL();
        CheckListBLL CKBLL = new CheckListBLL();
        //ShipmentPlanningDirectBLL CKBLL = new ShipmentPlanningDirectBLL();
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
            Ajax.Utility.RegisterTypeForAjax(typeof(ShipmentPlanningDirectStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    //txtFrmDt.Attributes.Add("readonly", "readonly");
                    //txtToDt.Attributes.Add("readonly", "readonly");
                    //if (!IsPostBack)
                    //{
                    //    GetData();
                    //}
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        #endregion

        # region Methods

        /// <summary>
        /// Getdata for All DropDownLists and GridViews
        /// </summary>
        protected void GetData()
        {
            try
            {
                //Search();
                //BindDropDownList(ddlCustomer, CSTMRBL.SelectCustomers(CommonBLL.FlagRegularDRP, 0));
                //BindDropDownList(ddlSupplier, SUPLRBL.SelectSuppliers(CommonBLL.FlagRegularDRP, 0));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }

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

        /// <summary>
        /// Bind Grid View Using Data Set
        /// </summary>
        /// <param name="ds"></param>
        private void BindGridView(DataSet ds)
        {
            try
            {
                //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //    GVCheckList.DataSource = ds.Tables[0];
                //else
                //    GVCheckList.DataSource = null;
                //GVCheckList.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning Direct Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Search Method
        /// </summary>
        //private void Search()
        //{
        //    try
        //    {
        //        DataTable dt = CommonBLL.EmptyDTCheckedList();
        //        if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagCommonMstr, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0,
        //                "", false, 0, 0, "", "", 0, CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.DateInsert(txtToDt.Text), true, "BIVAC"));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagWCommonMstr, 0, "", 0, "", "", "", 0, 0, "",
        //                false, 0, 0, "", "", 0, CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.DateInsert(txtToDt.Text), true, "BIVAC"));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagCommonMstr, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC",
        //                0, CommonBLL.StartDate, 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagQSelect, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.StartDate, 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagXSelect, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0, CommonBLL.StartDate, 0,
        //                CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagQSelect, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC",
        //                0, CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagQSelect, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.StartDate, 0, CommonBLL.DateInsert(txtToDt.Text), true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagQSelect, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.DateInsert(txtToDt.Text), true, ""));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagXSelect, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagXSelect, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0, CommonBLL.StartDate, 0,
        //                CommonBLL.DateInsert(txtToDt.Text), true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagWCommonMstr, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagWCommonMstr, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0, CommonBLL.StartDate,
        //                0, CommonBLL.DateInsert(txtToDt.Text), true, ""));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != ""
        //            && txtToDt.Text.Trim() == "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagCommonMstr, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.DateInsert(txtFrmDt.Text), 0, CommonBLL.EndDate, true, ""));
        //        else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == ""
        //            && txtToDt.Text.Trim() != "")
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagCommonMstr, 0, ddlCustomer.SelectedValue, 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0,
        //                CommonBLL.StartDate, 0, CommonBLL.DateInsert(txtToDt.Text), true, ""));
        //        else
        //            BindGridView(CKBLL.GetData(CommonBLL.FlagSelectAll, 0, "", 0, "", "", "", 0, 0, "", false, 0, 0, "", "BIVAC", 0, DateTime.Now, 0,
        //                DateTime.Now, true, ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
        //    }
        //}

        # endregion

        # region GridView Events

        protected void GVCheckList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //if (e.Row.RowType != DataControlRowType.DataRow) return;
                //int lastCellIndex = e.Row.Cells.Count - 1;
                //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                //HiddenField CrtedBy = (HiddenField)e.Row.FindControl("hfCreatedBy");
                //string ChkLstID = GVCheckList.DataKeys[e.Row.RowIndex].Values["ChkListID"].ToString();

                //if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                //{
                //    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                //    {
                //        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                //            UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value))))
                //        {
                //            //deleteButton.Enabled = false;
                //            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                //        }
                //        else
                //            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                //        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                //        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            //EditButton.Enabled = false;
                //            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                //        }
                //        else
                //        {

                //        }
                //    }
                //    else
                //    {
                //        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            //deleteButton.Enabled = false;
                //            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                //        }
                //        else
                //            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                //        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                //        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            //EditButton.Enabled = false;
                //            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                //        }
                //    }
                //    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                //}
                //else
                //    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                ////if(hfSevottamRefNo.Value != "")
                ////    EditButton.OnClientClick = "if (!alert('Ones Sevottam Ref.No. is Entered it cannot EDIT.')) return false;";

                //HyperLink hlShipmntINVNo = (HyperLink)e.Row.FindControl("hlShipmntINVNo");
                //HyperLink hlChkLstRefNo = (HyperLink)e.Row.FindControl("hlChkLstRefNo");

                //if (hlShipmntINVNo.Text.Trim() != "")
                //{
                //    hlChkLstRefNo.Enabled = false;
                //    hlShipmntINVNo.NavigateUrl = "~/Logistics/CheckListDetails.Aspx?ID=" + ChkLstID;
                //}
                //else
                //    hlChkLstRefNo.NavigateUrl = "~/Logistics/CheckListDetails.aspx?ID=" + ChkLstID;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CheckList Status", ex.Message.ToString());
            }
        }

        protected void GVCheckList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int res = 1;
            try
            {
                //int index = Convert.ToInt32(e.CommandArgument);
                //GridViewRow gvrow = GVCheckList.Rows[index];
                //string ChkListID = GVCheckList.DataKeys[index].Values["ChkListID"].ToString();
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Invoices/ShipmentPlanningDirect.aspx?ID=" + ChkListID, false);

                //else if (e.CommandName == "Remove")
                //{
                //    CheckListBLL CLBLL = new CheckListBLL();
                //    res = CLBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(ChkListID), "", "", "", 0,
                //        "", "", "", DateTime.Now, "", 0, 0, 0, 0, 0, 0, "", "", "", "", 0, "", "", "", 0, DateTime.Now,
                //        0, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "");
                //    if (res == 0)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Successfully Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CheckList Status", "Deleted Record " + ChkListID + " Successfully.");
                //        Search();
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Cannot Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CheckList Status", "Cannot Delete Record " + ChkListID + ".");
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CheckList Status", ex.Message.ToString());
            }
        }

        protected void GVCheckList_PreRender(object sender, EventArgs e)
        {
            try
            {
                //GVCheckList.UseAccessibleHeader = false;
                //GVCheckList.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning Direct Status", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClick

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment Planning Direct Status", ex.Message.ToString());
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
                string FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("MM-dd-yyyy");
                string ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("MM-dd-yyyy") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("MM-dd-yyyy");
                string INVNo = HFShpINVNo.Value;
                string ShpmntRefNo = HFShpPlngNo.Value;
                string Cust = HFCustNm.Value;
                string FPOnos = HFFPO.Value;
                string ShpmntMd = HFShpmntMode.Value;
                string Status = HFStatus.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";

                DataSet ds = CKBLL.CheckList_BIVAC_Search(FrmDt, ToDat, INVNo, ShpmntRefNo, FPOnos, Cust, ShpmntMd, Status, new Guid(Session["CompanyID"].ToString()));
                string ToDateTime = CommonBLL.DateCheck_Print(HFToDate.Value).ToString("dd-MM-yyyy");//Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy");
                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=SHIPMENT_PLANNING_DIRECT_STATUS.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDateTime)) == CommonBLL.EndDtMMddyyyy_FS)
                        ToDat = "";

                    string MTitle = " STATUS OF SHIPMENT PLANNING DIRECT ", MTcustomer = "", MTDTS = "";
                    if (HFCustNm.Value != "")
                        MTcustomer = HFCustNm.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd-MM-yyyy");

                    htextw.Write("<center><b>" + MTitle + " "
                                               + (MTcustomer != "" ? " FOR " + MTcustomer.ToUpper() : "") + ""
                                               + MTDTS + "</center></b>");
                    DataGrid dgGrid = new DataGrid();
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
        public string DeleteItemDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                int res = 1;
                string result = result = CommonBLL.Can_EditDelete(false, CreatedBy);

                #region Delete
                if (result == "Success")
                {
                    res = CKBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), "", "", "", "", Guid.Empty, "", "", "", DateTime.Now, "", Guid.Empty,
                        Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, "","","", "", "", "", Guid.Empty, "", "", "", Guid.Empty, DateTime.Now,
                        Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCheckedList(), "", new Guid(Session["CompanyID"].ToString()));
                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record/ Error while Deleting " + ID;
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Sevottam Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Invoices/ErrorLog"), "Sevottam Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Sevottam Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion
    }
}
