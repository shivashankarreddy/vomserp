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
using Ajax;
using System.Text;

namespace VOMS_ERP.Enquiries
{
    public partial class LEStatus : System.Web.UI.Page
    {
        # region variables
        int res;
        public static ArrayList Files = new ArrayList();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        BAL.FEnquiryBLL frnfenq = new FEnquiryBLL();
        BAL.LEnquiryBLL NLEBL = new LEnquiryBLL();
        BAL.EnumMasterBLL embal = new EnumMasterBLL();
        BAL.CustomerBLL cusmr = new CustomerBLL();
        BAL.SupplierBLL SUPLRBL = new SupplierBLL();
        CommonBLL CBLL = new CommonBLL();
        ItemMasterBLL ItemMstBLl = new ItemMasterBLL();
        ErrorLog ELog = new ErrorLog();
        static string GeneralCtgryID;
        static DataSet EditDS;
        static string btnSaveID = "";
        int UserID;
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
                Ajax.Utility.RegisterTypeForAjax(typeof(LEStatus));
                if (Session["UserID"] == null || (Session["UserID"].ToString()) == "")
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    if (!CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                        Response.Redirect("../Masters/Home.aspx?NP=no", false);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Get Data and Bind to Controls

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Binding GridView
        /// </summary>
        private void BindGridView(DataSet lcLEnqs)
        {
            try
            {
                DataTable dt = EmptyDt();
                if (lcLEnqs.Tables.Count > 0 && lcLEnqs.Tables[0].Rows.Count > 0)
                {
                    lcLEnqs.AcceptChanges();
                    ViewState["dset"] = lcLEnqs;
                }
                else
                    NoTable();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// This Method is used when There is no Table in DataSet
        /// </summary>
        private void NoTable()
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("S.No.");
                dt.Columns.Add("LocalEnquireId");
                dt.Columns.Add("Customer");
                dt.Columns.Add("FrnEnqNmbr");
                dt.Columns.Add("LocalEnqNmbr");
                dt.Columns.Add("Subject");
                dt.Columns.Add("Department");
                dt.Columns.Add("LEnquiryDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("SupplierNm");
                dt.Columns.Add("Instruction");
                dt.Columns.Add("CreatedBy");
                ds.Tables.Add(dt);
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
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
                dt.Columns.Add(new DataColumn("LocalEnquireId", typeof(long)));
                dt.Columns.Add(new DataColumn("Customer", typeof(string)));
                dt.Columns.Add(new DataColumn("FrnEnqNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("LocalEnqNmbr", typeof(string)));
                dt.Columns.Add(new DataColumn("Subject", typeof(string)));
                dt.Columns.Add(new DataColumn("Department", typeof(string)));
                dt.Columns.Add(new DataColumn("LEnquiryDate", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                dt.Columns.Add(new DataColumn("SupplierNm", typeof(string)));
                dt.Columns.Add(new DataColumn("Instruction", typeof(string)));
                dt.Columns.Add(new DataColumn("CreatedBy", typeof(long)));

                DataRow dr = dt.NewRow();
                dr["LocalEnquireId"] = 0;
                dr["Customer"] = string.Empty;
                dr["FrnEnqNmbr"] = string.Empty;
                dr["LocalEnqNmbr"] = string.Empty;
                dr["Subject"] = string.Empty;
                dr["Department"] = string.Empty;
                dr["LEnquiryDate"] = DateTime.Now;
                dr["Status"] = string.Empty;
                dr["SupplierNm"] = string.Empty;
                dr["Instruction"] = string.Empty;
                dr["CreatedBy"] = 0;
                dt.Rows.Add(dr);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Redirect to Mail-Send Page
        /// </summary>
        /// <param name="LEID"></param>
        private void GenPDF(int LEID)
        {
            try
            {
                Response.Redirect("../Masters/EmailSend.aspx?LeID=" + LEID, false);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Masters/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        protected void ClearInputs()
        {
            try
            {

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "IOM-2 Template Status", ex.Message.ToString());
            }
        }

        #endregion

        # region GridView Events

        /// <summary>
        /// This is used to get deleting / editing ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLenquiries_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //  int index = int.Parse(e.CommandArgument.ToString());
                //  //GridViewRow gvrow = gvLenquiries.Rows[index];
                ////  int ID = Convert.ToInt32(((Label)gvrow.FindControl("lblLocalEnquireId")).Text);
                //  if (e.CommandName == "Modify")
                //      Response.Redirect("../Enquiries/FloatEnquiry.Aspx?ID=" + ID, false);
                //  else if (e.CommandName == "Remove")
                //    //  DeleteItemDetails(ID);
                //  else if (e.CommandName == "Mail")
                //     // GenPDF(ID);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// GridView RowDataBound Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLenquiries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;

                int lastCellIndex = e.Row.Cells.Count - 1;
                ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                HiddenField CrtedBy = (HiddenField)e.Row.FindControl("HFCrtdBy");

                if (CommonBLL.AdminID != Convert.ToInt32(Session["UserID"]))
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
                    }
                    else
                    {
                        if ((!((string[])Session["UsrPermissions"]).Contains("Delete") ||
                            Convert.ToInt32(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
                            (!Session["TeamMembers"].ToString().Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                        {
                            //deleteButton.Enabled = false;
                            deleteButton.OnClientClick = "if (!alert('You do not have PERMISSIONS to Delete.')) return false;";
                        }
                        else
                            deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";
                        //ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                        if ((!((string[])Session["UsrPermissions"]).Contains("Edit") ||
                            Convert.ToInt32(Session["UserID"]) != Convert.ToInt32(CrtedBy.Value)) &&
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

                # region NotInUse
                //int lastCellIndex = e.Row.Cells.Count - 1;
                //ImageButton deleteButton = (ImageButton)e.Row.Cells[lastCellIndex].Controls[0];
                //deleteButton.OnClientClick = "if (!window.confirm('Are you sure you want to delete this?')) return false;";

                //Label CrtedBy = (Label)e.Row.FindControl("lblCrtdBy");
                //if (CommonBLL.AdminID != CommonBLL.UserID)
                //{
                //    if (CommonBLL.UserList[0].ToString() != CommonBLL.UserID.ToString())
                //    {
                //        if ((!CommonBLL.UsrPermissions.Contains("Delete") || CommonBLL.UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!CommonBLL.TeamMbrs.Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            deleteButton.Visible = false;
                //        }
                //        ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                //        if ((!CommonBLL.UsrPermissions.Contains("Edit") || CommonBLL.UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!CommonBLL.TeamMbrs.Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            EditButton.Visible = false;
                //        }
                //    }
                //    else
                //    {
                //        if ((!CommonBLL.UsrPermissions.Contains("Delete") || CommonBLL.UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!CommonBLL.UserList.Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            deleteButton.Visible = false;
                //        }
                //        ImageButton EditButton = (ImageButton)e.Row.Cells[lastCellIndex - 1].Controls[0];
                //        if ((!CommonBLL.UsrPermissions.Contains("Edit") || CommonBLL.UserID != Convert.ToInt32(CrtedBy.Value)) &&
                //            (!CommonBLL.UserList.Contains(CrtedBy.Value) || CommonBLL.AdminID == Convert.ToInt32(CrtedBy.Value)))
                //        {
                //            EditButton.Visible = false;
                //        }
                //    }
                //}
                # endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Grid View Pre-Render Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvLenquiries_PreRender(object sender, EventArgs e)
        {
            try
            {
                //  gvLenquiries.UseAccessibleHeader = false;
                // gvLenquiries.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        #endregion

        #region Search/Export to Excel/Export to Pdf Buttons Click Events

        /// <summary>
        /// Export to Pdf Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            //try
            //{
            //    if (gvLenquiries.Rows.Count > 0)
            //    {
            //        foreach (GridViewRow r in this.gvLenquiries.Controls[0].Controls)
            //        {
            //            r.Cells.RemoveAt(r.Cells.Count - 1);
            //            r.Cells.RemoveAt(r.Cells.Count - 1);
            //        }
            //    }
            //    Response.Clear(); //this clears the Response of any headers or previous output
            //    Response.Buffer = true; //ma
            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-disposition", "attachment;filename=LclEnquiries.pdf");
            //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    StringWriter sw = new StringWriter();
            //    HtmlTextWriter hw = new HtmlTextWriter(sw);
            //    gvLenquiries.RenderControl(hw);
            //    StringReader sr = new StringReader(sw.ToString());
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            //    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            //    pdfDoc.Open();
            //    htmlparser.Parse(sr);
            //    pdfDoc.Close();
            //    Response.Write(pdfDoc);
            //    HttpContext.Current.ApplicationInstance.CompleteRequest();
            //}
            //catch (Exception ex)
            //{
            //    string ErrMsg = ex.Message;
            //    int LineNo = ExceptionHelper.LineNumber(ex);
            //    ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            //}
        }

        /// <summary>
        /// Export to Excel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void btnExcelExpt_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {

                string Mode = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                string FrmDt = "", ToDat = "", CreatedDT = "";
                Guid LoginID = Guid.Empty;
                if ((CommonBLL.CustmrContactTypeText == Session["AccessRole"].ToString()) ||
                    CommonBLL.TraffickerContactTypeText == Session["AccessRole"].ToString() && Mode != null)
                    LoginID = new Guid(Session["UserID"].ToString());

                if (Mode == "odt")
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    if (HFToDate.Value != "")
                    {
                        ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                    }
                    else
                        ToDat = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else if (Mode == "tdt")
                {
                    CreatedDT = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    FrmDt = HFFromDate.Value == "" ? CommonBLL.StartDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFFromDate.Value).ToString("yyyy-MM-dd");
                    ToDat = HFToDate.Value == "" ? CommonBLL.EndDate.ToString("yyyy-MM-dd") : CommonBLL.DateCheck_Print(HFToDate.Value).ToString("yyyy-MM-dd");
                }
                string LENo = HFLENo.Value;
                string FENo = HFFENo.Value;
                string Subject = HFSubject.Value;
                string supp = HFSupplier.Value;
                string Customer = HFCustomer.Value;
                string Status = HFStatus.Value;

                if (FrmDt == "1-1-0001" || FrmDt == "1-1-1900")
                    FrmDt = "";
                if (ToDat == "1-1-0001")
                    ToDat = "";
                DataSet ds = NLEBL.LE_Search(FrmDt, ToDat, LENo, FENo, Subject, Status, supp, Customer, CreatedDT, LoginID, new Guid(Session["CompanyID"].ToString()));

                if (ds != null && ds.Tables.Count > 0)
                {
                    string attachment = "attachment; filename=LocalEnquirystatus.xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    if (FrmDt != "" && Convert.ToDateTime(FrmDt).ToString("dd-MM-yyyy") == "01-01-1900")
                        FrmDt = "";
                    if (ToDat != "" && (Convert.ToDateTime(ToDat).ToString("dd-MM-yyyy") == "01-01-0001" || CommonBLL.DateDisplay_2(Convert.ToDateTime(ToDat)) == CommonBLL.EndDtMMddyyyy_FS))
                        ToDat = "";

                    string MTitle = "STATUS OF LOCAL ENQUIRIES FLOATED", MTcustomer = "", MTDTS = "", MTENQNo = "";
                    if (HFCustomer.Value != "")
                        MTcustomer = HFCustomer.Value;
                    if (FrmDt != "" && ToDat != "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + CommonBLL.DateDisplay(Convert.ToDateTime(ToDat));
                    else if (FrmDt != "" && ToDat == "")
                        MTDTS = " DURING " + CommonBLL.DateDisplay(Convert.ToDateTime(FrmDt)) + " TO " + DateTime.Now.ToString("dd-MM-yyyy");
                    else
                        MTDTS = " TILL " + DateTime.Now.ToString("dd/MM/yyyy");
                    if (FENo != "")
                    {
                        MTENQNo = HFFENo.Value;
                        MTitle = "STATUS OF FOREIGN ENQUIRY NO. " + MTENQNo + " DT. " + ds.Tables[0].Rows[0]["FE Date"].ToString() + (MTcustomer == "" ? "" : " RECEIVED FROM " + MTcustomer.ToUpper());
                    }
                    else
                        MTitle = MTitle + " " + (MTcustomer != "" ? " TO " + MTcustomer.ToUpper() : "") + "" + MTDTS;
                    htextw.Write("<center><b>" + MTitle + "</center></b>");

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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export Verify Rendering
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearInputs();
                //Search();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Enquiry Status", ex.Message.ToString());
            }
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
                if (result == "Success")
                {
                    LEnquiryBLL LEBLL = new LEnquiryBLL();
                    DataSet EditDS = LEBLL.SelctLocalEnquiries(CommonBLL.FlagZSelect, new Guid(ID), Guid.Empty, Guid.Empty, "", "", Guid.Empty, DateTime.Now, DateTime.Now,
                    DateTime.Now, Guid.Empty, Guid.Empty, CommonBLL.StatusTypeLPOrder, "", "", "", Guid.Empty, DateTime.Now, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal());
                    if (EditDS.Tables.Count > 0 && EditDS.Tables[0].Rows.Count > 0)
                        res = -123;
                    else
                    {
                        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
                        res = NLEBL.InsertUpdateDeleteLocalEnquiries(CommonBLL.FlagDelete, new Guid(ID), Guid.Empty, Guid.Empty, "", "", Guid.Empty,
                            DateTime.Now, DateTime.Now, DateTime.Now, Guid.Empty, Guid.Empty, 0, "", "", "", new Guid(Session["UserID"].ToString()), DateTime.Now,
                            new Guid(Session["UserID"].ToString()), DateTime.Now, false, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDtLocal());
                    }
                }
                if (res == 0)
                    result = "Success::Deleted Successfully";
                else
                    result = "Error::Cannot Delete this Record, LQ already created so delete LQ " + ID;
                return result;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }


        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string EditDetails(string ID, string CreatedBy, string IsCust)
        {
            try
            {
                return CommonBLL.Can_EditDelete(true, CreatedBy);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Local Enquiry Status", ex.Message.ToString());
                return ErrMsg;
            }
        }
        #endregion
    }
}
