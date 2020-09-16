using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Threading;
using Ajax;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
//using ExcelUtility;

namespace VOMS_ERP.Logistics
{
    public partial class CTOneTrackingStatus : System.Web.UI.Page
    {
        # region Variables

        ErrorLog ELog = new ErrorLog();
        CT1TrackingBLL CT1TBLL = new CT1TrackingBLL();
        CustomerBLL CSTMRBL = new CustomerBLL();
        SupplierBLL SUPLRBL = new SupplierBLL();
        int UserID;

        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CTOneTrackingStatus));
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    //txtFrmDt.Attributes.Add("readonly", "readonly");
                    //txtToDt.Attributes.Add("readonly", "readonly");
                    if (!IsPostBack)
                    {
                        GetData();
                    }
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        # region Methods

        /// <summary>
        /// Getdata for All DropDownLists and GridViews
        /// </summary>
        protected void GetData()
        {
            try
            {
                Search();
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

        private void BindGridView(DataSet ds)
        {
            try
            {
                //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //    GVCTOnetracking.DataSource = ds.Tables[0];
                //else
                //    GVCTOnetracking.DataSource = null;
                //GVCTOnetracking.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        private void Search()
        {
            try
            {
                //if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "") // T
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue),
                //        Convert.ToInt64(ddlCustomer.SelectedValue), Convert.ToDateTime(txtFrmDt.Text), Convert.ToDateTime(txtToDt.Text)));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() != "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, 0, 0, 0, Convert.ToDateTime(txtFrmDt.Text), Convert.ToDateTime(txtToDt.Text)));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "") // T
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), Convert.ToInt64(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagQSelect, 0, 0, 0, Convert.ToInt64(ddlCustomer.SelectedValue),
                //        CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() == "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagXSelect, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), 0, CommonBLL.StartDate, CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagQSelect, 0, 0, 0, Convert.ToInt64(ddlCustomer.SelectedValue), Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagQSelect, 0, 0, 0, Convert.ToInt64(ddlCustomer.SelectedValue), CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text)));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagXSelect, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), 0, Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagXSelect, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), 0, CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text)));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, 0, 0, 0, Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue == "0" && ddlCustomer.SelectedValue == "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "")
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagWCommonMstr, 0, 0, 0, 0, CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text)));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() != "" && txtToDt.Text.Trim() == "") // T
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), Convert.ToInt64(ddlCustomer.SelectedValue),
                //        Convert.ToDateTime(txtFrmDt.Text), CommonBLL.EndDate));
                //else if (ddlSupplier.SelectedValue != "0" && ddlCustomer.SelectedValue != "0" && txtFrmDt.Text.Trim() == "" && txtToDt.Text.Trim() != "") // T
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagCommonMstr, 0, 0, Convert.ToInt64(ddlSupplier.SelectedValue), Convert.ToInt64(ddlCustomer.SelectedValue), 
                //        CommonBLL.StartDate, Convert.ToDateTime(txtToDt.Text)));
                //else
                //    BindGridView(CT1TBLL.GetDataSet(CommonBLL.FlagSelectAll, 0, 0, 0, 0, CommonBLL.StartDate, CommonBLL.EndDate));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
            }
        }


        //public string GetValue(string templatePath, string xmlString)
        //{
        //    XDocument xmlObj = XDocument.Parse(xmlString);
        //    XDocument result = GetResultXml(templatePath, xmlObj);

        //    return (result == null) ? string.Empty : result.Document.ToString();
        //}
        //private XDocument GetResultXml(string templatePath, XDocument xmlObj)
        //{
        //    XDocument result = new XDocument();
        //    using (XmlWriter writer = result.CreateWriter())
        //    {
        //        XslCompiledTransform xslt = new XslCompiledTransform();
        //        xslt.Load(templatePath);
        //        xslt.Transform(xmlObj.CreateReader(), writer);
        //    }
        //    return result;
        //}​


        //private void Export2Excel(long CTPID)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds = CT1TBLL.GetDataSet(CommonBLL.FlagZSelect, CTPID, 0, 0, 0, CommonBLL.StartDate, CommonBLL.EndDate);

        //        string tab = "\t";

        //        StringBuilder sb = new StringBuilder();

        //        //sb.AppendLine("<html>");
        //        //sb.AppendLine(tab + "<body>");
        //        sb.AppendLine(tab + tab + "<table>");

        //        // headers.
        //        sb.Append(tab + tab + tab + "<tr>");

        //        foreach (DataColumn dc in ds.Tables[0].Columns)
        //        {
        //            sb.AppendFormat("<td>{0}</td>", dc.ColumnName);
        //        }

        //        sb.AppendLine("</tr>");

        //        // data rows
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            sb.Append(tab + tab + tab + "<tr>");

        //            foreach (DataColumn dc in ds.Tables[0].Columns)
        //            {
        //                string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
        //                sb.AppendFormat("<td>{0}</td>", cellValue);
        //            }

        //            sb.AppendLine("</tr>");
        //        }

        //        sb.AppendLine(tab + tab + "</table>");
        //        //sb.AppendLine(tab + "</body>");
        //        //sb.AppendLine("</html>");

        //        //HttpResponse response = HttpContext.Current.Response;
        //        //response.Clear();
        //        //response.Charset = "";
        //        //response.ContentType = "application/vnd.ms-excel";
        //        //response.AddHeader("Content-Disposition", "attachment;filename=\"CTOneTaskReport.xls\"");

        //        //using (StringWriter sw = new StringWriter())
        //        //{
        //        //    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
        //        //    {
        //        //        // instantiate a datagrid
        //        //        DataGrid dg = new DataGrid();
        //        //        dg.DataSource = ds.Tables[0];
        //        //        dg.DataBind();
        //        //        dg.HeaderStyle.Font.Bold = true;
        //        //        dg.RenderControl(htw);
        //        //        response.Write(sw.ToString());
        //        //        response.End();
        //        //    }
        //        //}
        //    }
        //    catch (ThreadAbortException tae)
        //    { }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
        //    }
        //}

        # endregion

        # region GridView Events

        protected void GVCTOnetracking_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                HiddenField CrtedBy = (HiddenField)e.Row.FindControl("hfCreatedBy");
                //HiddenField hfSevottamRefNo = (HiddenField)e.Row.FindControl("hfSevottamRefNo");
                if (CommonBLL.AdminID != Convert.ToInt64(Session["UserID"]))
                {
                    if (Session["TeamMembers"] != null && !Session["TeamMembers"].ToString().Contains(Session["UserID"].ToString()))
                    {
                        if ((string[])Session["UsrPermissions"] != null && ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value))))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || UserID != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            //EditButton.Enabled = false;
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
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") || Convert.ToInt64(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            //EditButton.Enabled = false;
                            EditButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Edit.')) return false;";
                        }
                    }
                    deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                }
                else
                    deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                //if(hfSevottamRefNo.Value != "")
                //    EditButton.OnClientClick = "if (!alert('Ones Sevottam Ref.No. is Entered it cannot EDIT.')) return false;";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        protected void GVCTOnetracking_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int res = 1;
            try
            {
                //int index = Convert.ToInt32(e.CommandArgument);
                //string CTpID = GVCTOnetracking.DataKeys[index].Values["CTpID"].ToString();
                //if (e.CommandName == "Modify")
                //    Response.Redirect("../Logistics/CTOneTracking.aspx?ID=" + CTpID, false);
                //else if (e.CommandName == "Remove")
                //{
                //    res = CT1TBLL.InsertUpdateDelete(CommonBLL.FlagDelete, Convert.ToInt64(CTpID), 0, 0, "", "", 0, 0, 0, DateTime.Now,
                //        Convert.ToInt64(Session["UserID"]), DateTime.Now, 0, DateTime.Now, true, CommonBLL.EmptyDTCT1trackingDetails(), "");
                //    if (res == 0)
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Successfully Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CTOne Tracking Status", "Deleted Record " + CTpID + " Successfully.");
                //        Search();
                //    }
                //    else
                //    {
                //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Cannot Delete this Record.');", true);
                //        ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", "Cannot Delete Record " + CTpID + ".");
                //    }
                //}
                //else if (e.CommandName == "Export2Excel")
                //{ 
                //    Export2Excel(Convert.ToInt64(CTpID));
                //}
            }
            catch (ThreadAbortException tae)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        protected void GVCTOnetracking_PreRender(object sender, EventArgs e)
        {
            try
            {
                //GVCTOnetracking.UseAccessibleHeader = false;
                //GVCTOnetracking.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                //int LineNo = ExceptionHelper.LineNumber(ex);
                //ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        # endregion

        # region ButtonClick

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //if (GVCTOnetracking.Rows.Count > 0)
                //{
                //    foreach (GridViewRow r in this.GVCTOnetracking.Controls[0].Controls)
                //    {
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //        r.Cells.RemoveAt(r.Cells.Count - 1);
                //    }
                //}
                //Response.Clear(); //this clears the Response of any headers or previous output
                //Response.Buffer = true; //ma
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=CTOneTaskstatus.pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //StringWriter sw = new StringWriter();
                //HtmlTextWriter hw = new HtmlTextWriter(sw);
                //GVCTOnetracking.RenderControl(hw);
                //StringReader sr = new StringReader(sw.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                //PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                //pdfDoc.Open();
                //htmlparser.Parse(sr);
                //pdfDoc.Close();
                //Response.Write(pdfDoc);
                //Response.End();
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        protected void btnExcelExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string CT_1RefNo = HFCT_1RefNo.Value;
                string FPONos = HFFPONos.Value;
                string LPONos = HFLPONos.Value;
                string CustNm = HFCustNm.Value;
                string SuppNm = HFSuppNm.Value;

                DataSet ds = CT1TBLL.GetDataSet_Export(CT_1RefNo, FPONos, LPONos, CustNm, SuppNm, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=CT1TaskStatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    string MTitle = "STATUS OF CT-1 TASK", MTcustomer = "", MTDTS = "";

                    if (HFCustNm.Value != "")
                        MTcustomer = HFCustNm.Value;
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
                        string headerTable = "<img src='"+ CommonBLL.CommonAdminLogoUrl(HttpContext.Current) + "' margin-top =16px width=125 height=35 />";
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CTOne Tracking Status", ex.Message.ToString());
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        # endregion

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
                    res = CT1TBLL.InsertUpdateDelete(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, 0, "", "", Guid.Empty, Guid.Empty, 0, DateTime.Now,
                        new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), DateTime.Now, Guid.Empty, DateTime.Now, true, CommonBLL.EmptyDTCT1trackingDetails(), "");

                    if (res == 0)
                        result = "Success::Deleted Successfully";
                    else
                        result = "Error::Cannot Delete this Record, Another Transaction is using this record";
                }
                #endregion

                return result;
            }
            catch (SqlException sx)
            {
                string ErrMsg = sx.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", sx.Message.ToString());
                return ErrMsg;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Inspection Plan Status", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }

        //[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        //public void Export2Excel(long CTPID)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds = CT1TBLL.GetDataSet(CommonBLL.FlagZSelect,Convert.ToInt32(CTPID), 0, 0, 0, CommonBLL.StartDate, CommonBLL.EndDate);

        //        HttpResponse response = HttpContext.Current.Response;
        //        response.Clear();
        //        response.Charset = "";
        //        response.ContentType = "application/vnd.ms-excel";
        //        response.AddHeader("Content-Disposition", "attachment;filename=\"CTOneTaskReport.xls\"");

        //        using (StringWriter sw = new StringWriter())
        //        {
        //            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
        //            {
        //                // instantiate a datagrid
        //                DataGrid dg = new DataGrid();
        //                dg.DataSource = ds.Tables[0];
        //                dg.DataBind();
        //                dg.HeaderStyle.Font.Bold = true;
        //                dg.RenderControl(htw);
        //                response.Write(sw.ToString());
        //                //HttpContext.Current.Response.Redirect("~/Logistics/CTOnetrackingStatus.aspx", false); 
        //                response.End();
        //            }
        //        }
        //    }
        //    catch (ThreadAbortException tae)
        //    { }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
        //    }
        //}

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string Export2Excel(string CTPID)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = CT1TBLL.GetDataSet(CommonBLL.FlagZSelect, new Guid(CTPID), Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.StartDate, CommonBLL.EndDate, new Guid(Session["CompanyID"].ToString()));

                string tab = "\t";

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(tab + tab + "<table cellspacing='1px' border='1px'>");

                // headers.
                sb.Append(tab + tab + tab + "<tr>");

                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    sb.AppendFormat("<th>{0}</th>", dc.ColumnName);
                }

                sb.AppendLine("</tr>");

                // data rows
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sb.Append(tab + tab + tab + "<tr>");

                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
                        sb.AppendFormat("<td>{0}</td>", cellValue);
                    }

                    sb.AppendLine("</tr>");
                }

                string tabl = Convert.ToString(sb.AppendLine(tab + tab + "</table>"));

                return tabl;
            }
            catch (ThreadAbortException tae)
            { return null; }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Status", ex.Message.ToString());
                return null;
            }
        }

        #endregion
    }
}