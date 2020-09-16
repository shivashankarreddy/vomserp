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
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using VOMS_ERP.Admin;


namespace VOMS_ERP.Quotations
{
    public partial class LQComparisionByItems : System.Web.UI.Page
    {
        # region variables
        int res;
        BAL.LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        ComparisonStmntBLL CSBL = new ComparisonStmntBLL();
        BAL.CustomerBLL CSTRBL = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        FieldAccessBLL FAB = new FieldAccessBLL();
        string PriceSymbol = "";
        private string _seperator = "|";
        FileStream m_streams;
        private int m_currentPageIndex;
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        //static ArrayList ALSup;
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
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        Ajax.Utility.RegisterTypeForAjax(typeof(LQComparisionByItems));
                        if (!IsPostBack)
                        {
                            ClearAll();
                            Session["IsCheckedAll"] = false;
                            GetData();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        #endregion

        #region Methods for Bind Data

        /// <summary>
        /// Bind Defaul Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlCustomer, CSTRBL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyID"].ToString())));

                if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                {
                    ddlCustomer.SelectedValue = Request.QueryString["CsID"].ToString();
                    BindDropDownList(ddlEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagRegularDRP, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now,
                        "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()),
                        new Guid(Session["UserID"].ToString()), true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    BindListBox(lbEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty,
                        new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                        DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()), new Guid(Session["UserID"].ToString()),
                        true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));

                    ddlEnquiry.SelectedValue = Request.QueryString["FeqID"].ToString();

                    string feids = Request.QueryString["FeqID"].ToString();
                    string[] Fes = feids.Split(',');
                    foreach (System.Web.UI.WebControls.ListItem item in lbEnquiry.Items)
                    {
                        foreach (string s in Fes)
                        {
                            if (item.Value == s.ToLower().Trim())
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    string Enquiry = String.Join(",", lbEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                    BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagLSelect, new Guid(ddlEnquiry.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                        new Guid(ddlCustomer.SelectedValue), Enquiry));
                }
                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    if (ddlCustomer.SelectedValue != Guid.Empty.ToString())
                    {
                        BindDropDownList(ddlEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                            new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        BindListBox(lbEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                            "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                            new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        divCompare.InnerHtml = "";
                        ddlCustomer.Enabled = false;
                    }
                    else
                    {
                        BindDropDownList(ddlCustomer, CSTRBL.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()),
                            new Guid(Session["CompanyID"].ToString())));
                        ddlEnquiry.SelectedValue = "0";
                        divCompare.InnerHtml = "";
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Drop Down Lists
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        private void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                if (CommonDt != null && CommonDt.Tables.Count > 0)
                {
                    ddl.DataSource = CommonDt.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();
                }
                else
                {
                    ddl.DataSource = null;
                    ddl.DataBind();
                }
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Bind List Box
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="CommonDt"></param>
        private void BindListBox(System.Web.UI.WebControls.ListBox lb, DataSet CommonDt)
        {
            try
            {
                lb.DataSource = CommonDt;
                lb.DataTextField = "Description";
                lb.DataValueField = "ID";
                lb.DataBind();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Clear All Inputs
        /// </summary>
        protected void ClearAll()
        {
            try
            {
                txtComComments.Value = "";
                ddlCustomer.SelectedIndex = -1;
                ddlEnquiry.SelectedIndex = -1;
                divCompare.InnerHtml = "";
                lbEnquiry.Items.Clear();
                ViewState["CompareDT"] = null;
                Session["dtEdit"] = null;
                Session["IsCheckedAll"] = false;
                Session["dt"] = null;
                Session["ALSup"] = null;
                //if (ALSup != null)
                //    ALSup.Clear();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
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

        #endregion

        #region DropDownList Selected Index Changed Events

        /// <summary>
        /// Customer Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (ddlCustomer.SelectedValue != "0")
                {
                    if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                    {
                        BindListBox(lbEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                            new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        divCompare.InnerHtml = "";
                    }
                    else
                    {
                        BindListBox(lbEnquiry, NEBLL.NewEnquiryEdit(CommonBLL.FlagZSelect, Guid.Empty, Guid.Empty, new Guid(ddlCustomer.SelectedValue),
                            Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true,
                            new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        divCompare.InnerHtml = "";
                    }
                }
                else
                {
                    ddlEnquiry.SelectedValue = "0";
                    divCompare.InnerHtml = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Foreign Enquiry Drop Down List Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlEnquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string Enquiry = String.Join(",", lbEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());

                if (lbEnquiry.SelectedValue != "0")
                {
                    //if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                    //    BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagCSelect, new Guid(ddlEnquiry.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    //        new Guid(ddlCustomer.SelectedValue), Enquiry));
                    //else
                    //    BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagLSelect, new Guid(ddlEnquiry.SelectedValue), Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    //        new Guid(ddlCustomer.SelectedValue), Enquiry));

                    if (CommonBLL.CustmrContactTypeText == ((ArrayList)Session["UserDtls"])[7].ToString())
                        BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagCSelect, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()), Guid.Empty,
                            new Guid(ddlCustomer.SelectedValue), Enquiry));
                    else
                        BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                            new Guid(ddlCustomer.SelectedValue), Enquiry));
                }
                else
                {
                    divCompare.InnerHtml = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }
        #endregion

        #region Button Click Events

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        string[] suplst = new string[10];
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Filename = FileName();
                if (Session["dt"] != null)
                {
                    DataTable dt = (DataTable)Session["dt"];
                    ArrayList ALSup = (ArrayList)Session["ALSup"];
                    int CheckCount = 0;
                    for (int i = 0; i < ALSup.Count; i++)
                    {
                        string colname = ALSup[i].ToString();
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            string ItemDetailsID = dt.Rows[j][colname + "_IDID"].ToString();
                            string LQID = dt.Rows[j][colname + "_LQID"].ToString();
                            string FenqID = dt.Rows[j][colname + "_FEnqID"].ToString();
                            if (Convert.ToBoolean(dt.Rows[j][colname + "_CB"].ToString()) && ItemDetailsID != "" && LQID != "")
                            {
                                CheckCount++;
                                res = CSBL.InsertDeleteBasketItems(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ItemDetailsID),
                                                new Guid(FenqID), new Guid(LQID), Guid.Empty, Guid.Empty, Guid.Empty, 35, new Guid(Session["UserID"].ToString()), "", new Guid(Session["CompanyID"].ToString()));
                            }
                        }
                    }
                    if (res == 0 && btnSave.Text == "Select")
                    {
                        if (CheckCount > 0 && res == 0)
                        {
                            //ALS.AuditLog(res, "Save", "", "Local Quotation Comparsion:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            string Enquiry = String.Join(",", lbEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Selected Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "Local Quotation Comparison", "Data Inserted Successfully.");
                            CBLL.ClearUploadedFiles();
                            Response.Redirect("LConfromationBasket.aspx?CsID=" + ddlCustomer.SelectedValue + "&FeqID=" + Enquiry, false);
                            ClearAll();
                        }
                        else
                        {
                            //ALS.AuditLog(res, "Save", "", "Local Quotation Comparsion:", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Select atleast one Item.');", true);
                        }
                    }
                    else if (res != 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", "Error While Saving");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving.');", true);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Saving...!');", true);
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
                ClearAll();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }


        #endregion

        #region Bind Comparison Table

        /// <summary>
        /// Calling Methos for Comparision Items and Terms & Conditions
        /// </summary>
        /// <param name="ItemsTerms"></param>
        private void BindItemsTable(DataSet ItemsTerms)
        {
            try
            {
                if (ItemsTerms.Tables.Count > 1 && ItemsTerms.Tables[0].Rows.Count > 0)
                {
                    DataTable IT = ItemsTerms.Tables[0].Copy();

                    IT.Columns.Add("IsCheck", typeof(bool)).SetOrdinal(2);
                    foreach (DataRow row in IT.Rows)
                    {
                        row["IsCheck"] = false;
                    }
                    IT.AcceptChanges();
                    Session["dtEdit"] = IT;

                    ComparBySuppl_1(ItemsTerms);
                }
                else
                    divCompare.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        string qun = "0";

        StringBuilder sbb = new StringBuilder();

        private void ComparBySuppl_1(DataSet ds)
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                int NoofSups = 0;
                int SupCounting = 0;
                int emptyFlag = 0;
                int SNo = 1;
                ArrayList ALSup = new ArrayList();
                sbb.Append("<table id='Compare_Range'>");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];

                sbb.Append("<thead>");
                StringBuilder sbh = new StringBuilder();
                StringBuilder sbh1 = new StringBuilder();
                sbh.Append("<tr>");
                sbh1.Append("<tr>");
                dt.Columns.Add("SNO", typeof(long));
                dt.Columns.Add("ItemID", typeof(Guid));

                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(Session["CompanyID"].ToString()), "NewLQuotation.Aspx");
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.PriceTagText)))
                        Session["HideFields"] = HideFields.Tables[0];
                }

                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 5)
                        continue;

                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        if (i > CommonBLL.CompareByItmsSupplStColNo - 1)
                        {
                            string SupNm = ds.Tables[0].Columns[i].ToString().Trim();
                            dt.Columns.Add(SupNm + "_LQID", typeof(Guid));
                            dt.Columns.Add(SupNm + "_IDID", typeof(Guid));
                            dt.Columns.Add(SupNm + "_FEnqID", typeof(Guid));
                            dt.Columns.Add(SupNm + "_ItemStatus", typeof(string));

                            DataColumn dc = new DataColumn(SupNm + "_CB", typeof(bool));
                            dc.DefaultValue = false;
                            dt.Columns.Add(dc);
                            ALSup.Add(SupNm);

                            sbh.Append("<th colspan='6' style='border-right: 1px solid #A09797'>" + SupNm + "</th>");
                            sbh1.Append("<th>Spec.</th>");
                            sbh1.Append("<th>Make</th>");
                            sbh1.Append("<th>Rate</th>");
                            sbh1.Append("<th>Net-Rate</th>");
                            sbh1.Append("<th>Amount</th>");
                            sbh1.Append("<th><input type='checkbox' onclick='javascript:CheckAll1(" + i + "," + NoofSups + ");' name='ChkHead" + i + "' id='ChkHead" + i + "'/></th>");
                            NoofSups++;
                        }
                        else
                            sbh.Append("<th rowspan='2'>" + ds.Tables[0].Columns[i].ToString() + "</th>");
                    }
                    else
                        sbh.Append("<th rowspan='2'>S.No.</th>");
                }
                sbb.Append(sbh + "<th style='border-right: 1px solid #A09797'> </th></tr>");
                sbb.Append(sbh1 + "<th> </th></tr>");
                sbb.Append("</thead>");
                sbb.Append("<tbody>");

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int qty = 0, amt = 1, tcc = 2;
                    sbb.Append("<tr>");
                    dr["SNO"] = SNo;
                    for (int j = 1; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        if (j == 5)
                        {
                            continue;
                        }
                        if (j > CommonBLL.CompareByItmsSupplStColNo - 1)
                        {
                            if (j > CommonBLL.CompareByItmsSupplStColNo - 1)
                            {
                                string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                (CommonBLL.stringRowSeparators1, StringSplitOptions.None);
                                if (splitCols.Length > 1)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    sb.Append("<div id='mousefollow-examples'><div title='<b>Previous Rates :</b><br/>" +
                                        splitCols[7].Replace(",", "<br/>") + "'>" + splitCols[0] + "</div></div>");

                                    # region ToolTip In Table (Completed but) Not-Working
                                    //string Head = "<p style='font-weight:bold;'>Previous Rates</p>"
                                    //    + "<p style='font-weight:bold; text-align: center;'>Rate</p><p style='font-weight:bold; text-align: center; float:left'>LQ No</p>"
                                    //    + "<p style='font-weight:bold; text-align: center; float:left'>LQ DT</p><p>";

                                    //sb.Append("<div id='mousefollow-examples'><div title='" + Head + "" +
                                    //splitCols[7].Replace(",", "</p><p>").Replace("-->", "</p><p>") + "</p>'>" + splitCols[0] + "</div></div>");
                                    # endregion

                                    string dev = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/>") + (splitCols[6].Trim() == "" ? "" : "<span style='font-weight:bold;'><u>DEVIATION</u>:</span> " + splitCols[6]);

                                    sbb.Append("<td>" + dev + "</td>");
                                    sbb.Append("<td>" + splitCols[2] + "</td>");
                                    sbb.Append("<td align='right'>" + sb.ToString() + "</td>");
                                    sbb.Append("<td align='right'>" + splitCols[5] + "</td>");
                                    decimal Count = Convert.ToDecimal(Math.Round(Convert.ToDouble(splitCols[5]) * Convert.ToDouble(qun), 2));
                                    tot[j] += Math.Round(Convert.ToDouble(splitCols[5]) * Convert.ToDouble(qun), 2);
                                    sbb.Append("<td align='right'>" + Count.ToString("N") + "</td>");
                                    sbb.Append("<td style='border-right: 1px solid #A09797'>");
                                    sbb.Append("<input id='ChkHead" + j + "" + i + "' type='checkbox' onclick='CheckInd("
                                        + j + "" + i + "," + i + "," + SupCounting + ")'>");
                                    sbb.Append("<input id='fenqid" + j + "" + i + "' type='hidden' value='" + ds.Tables[0].Rows[i]["ForeignEnquiryId"].ToString() + "'>");
                                    sbb.Append("<input id='hfLQID" + j + "" + i + "' type='hidden' value='" + splitCols[3] + "'>");
                                    sbb.Append("<input id='hfItemDetailsID" + j + "" + i + "' type='hidden' value='" + splitCols[4] + "'>");
                                    sbb.Append("<input id='hfItemStatus" + i + "' type='hidden' value='" + splitCols[8] + "'>");
                                    sbb.Append("</td>");//6 


                                    dr["ItemID"] = new Guid(ds.Tables[0].Rows[i]["ItemId"].ToString());
                                    dr[ALSup[SupCounting] + "_LQID"] = new Guid(splitCols[3].ToString());
                                    dr[ALSup[SupCounting] + "_IDID"] = new Guid(splitCols[4].ToString());
                                    dr[ALSup[SupCounting] + "_FEnqID"] = new Guid(ds.Tables[0].Rows[i]["ForeignEnquiryId"].ToString());
                                    dr[ALSup[SupCounting] + "_CB"] = false;
                                    dr[ALSup[SupCounting] + "_ItemStatus"] = splitCols[8].ToString();
                                    SupCounting++;
                                }
                                else
                                {
                                    dr[ALSup[SupCounting] + "_CB"] = false;
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td style='border-right: 1px solid #A09797;'><input id='ChkHead"
                                        + j + "" + i + "' type='checkbox' onclick='CheckInd("
                                        + j + "" + i + "," + i + "," + SupCounting + ")' style='display:none;'></td>");
                                    SupCounting++;
                                }
                            }
                        }
                        else
                        {
                            if (j == 1)
                                sbb.Append("<td>" + (i + 1).ToString() + "</td>");
                            else
                                sbb.Append("<td><div class='expanderR'>"

                                    + ds.Tables[0].Rows[i].ItemArray[j].ToString() + "</div>"
                                    + "<input id='hfItemID" + j + "" + i + "' type='hidden' value='"
                                    + ds.Tables[0].Rows[i]["ItemId"].ToString() + "'></td>");
                            qun = ds.Tables[0].Rows[i].ItemArray[3].ToString();
                        }
                    }
                    sbb.Append("<td></td>");
                    sbb.Append("</tr>");
                    dt.Rows.Add(dr);
                    SupCounting = 0;
                    SNo++;
                    dr = dt.NewRow();
                }

                StringBuilder nsb = new StringBuilder();//This is for Total Amount in words
                StringBuilder nsb1 = new StringBuilder();//This is for Teerms & Conditions Empty Row
                if (Session["HideFields"] != null && ((DataTable)Session["HideFields"]).Rows.Count > 0)
                {
                    PriceSymbol = (((DataTable)Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                        .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                }
                sbb.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold;'>Total Amount : </span></td>"); //("+PriceSymbol+")
                sbb.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                nsb.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold;'>Total Amount in Words : </span></td>");//(" + PriceSymbol + ") 
                nsb.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                nsb1.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold; color: red;'>Terms & Conditions : </span></td>");
                nsb1.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                for (int s = 6; s < tot.Length; s++)
                {
                    string total = tot[s].ToString();
                    Decimal deci = Convert.ToDecimal(total);
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string words = N2W.Num2WordConverter(deci.ToString(), PriceSymbol).ToString();


                    sbb.Append("<td colspan='5' align='right'><span style='font-weight:bold;'>" + deci.ToString("N") + "</span></td>");
                    sbb.Append("<td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style='border-right: 1px solid #A09797'></td>");
                    nsb.Append("<td colspan='6' style='border-right: 1px solid #A09797'  align='center'><span style='font-weight:bold;'>" + words + "</span></td>");
                    nsb.Append("<td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td>");
                    nsb1.Append("<td></td><td></td><td></td><td></td><td></td><td style='border-right: 1px solid #A09797'></td>");
                }
                sbb.Append("<td></td>");
                sbb.Append("</tr>");
                sbb.Append(nsb + "<td></td></tr>");
                sbb.Append(nsb1 + "<td></td></tr>");

                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    sbb.Append("<tr>");
                    for (int j = 0; j < ds.Tables[1].Rows[i].ItemArray.Length; j++)
                    {
                        if (j == 0)
                            sbb.Append("<td></td><td colspan='3' align='right'><span style='font-weight:bold;'>" + ds.Tables[1].Rows[i].ItemArray[j].ToString() + " : </span></td><td style = 'display:none'></td><td style = 'display:none'></td>");
                        else
                            sbb.Append("<td colspan='6' style='border-right: 1px solid #A09797'>" + ds.Tables[1].Rows[i].ItemArray[j].ToString() + "</td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td>");
                    }
                    sbb.Append("<td></td></tr>");
                }

                sbb.Append("</tbody>");
                sbb.Append("</table>");
                divCompare.InnerHtml = sbb.ToString();
                Session["dt"] = dt;
                Session["ALSup"] = ALSup;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method for Comprision Items Table
        /// </summary>
        /// <param name="ds"></param>
        private void ComparBySuppl(DataSet ds)
        {
            try
            {
                int emptyFlag = 0;
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();

                ComparisionGrid.Rows.Add(row);
                ComparisionGrid.Rows.Add(row1);
                HtmlTableCell cel1 = new HtmlTableCell();
                cel1.ColSpan = 4;
                row1.Cells.Add(cel1);
                row1.Attributes.Add("class", "table_heading");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "Head" + i.ToString();
                        if (i < 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        //if (i == 1)
                        //{
                        //    cel.Attributes.Add("class", "headcol1");
                        //}
                        //else if (i == 2)
                        //{
                        //    cel.Attributes.Add("class", "headcol2");
                        //}
                        //else if (i == 3)
                        //{
                        //    cel.Attributes.Add("class", "headcol3");
                        //}
                        //else if (i == 4)
                        //{
                        //    cel.Attributes.Add("class", "headcol4");
                        //}
                        else
                        {
                            //cel.ColSpan = 6;
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            cel.ColSpan = 6;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celz = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            HtmlTableCell celB = new HtmlTableCell();
                            HtmlTableCell celEmpty = new HtmlTableCell();
                            HtmlTableCell celEmpty1 = new HtmlTableCell();
                            HtmlTableCell celEmpty2 = new HtmlTableCell();
                            System.Web.UI.WebControls.CheckBox chkb = new System.Web.UI.WebControls.CheckBox();
                            chkb.ID = "ch1" + i;
                            chkb.Attributes.Add("onclick", "javascript:CheckAll('" + chkb.ID + "');");

                            celx.InnerText = "Rate";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celz.InnerText = "Net Rate";
                            celA.InnerText = "Amount";
                            //celB.InnerText = "Select";
                            celB.Controls.Add(chkb);

                            //if (emptyFlag == 0)
                            //{
                            //    row1.Cells.Add(celEmpty);
                            //    row1.Cells.Add(celEmpty1);
                            //    row1.Cells.Add(celEmpty2);
                            //}
                            //emptyFlag += 1;
                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
                            row1.Cells.Add(celz);
                            row1.Cells.Add(celA);
                            row1.Cells.Add(celB);
                            cel.Attributes.Add("class", "top_heading left_border");
                        }
                        cel.InnerText = ds.Tables[0].Columns[i].ToString();
                        row.Cells.Add(cel);
                    }
                    else
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.Attributes.Add("class", "top_heading");
                        cel.InnerText = "S.No.";
                        row.Cells.Add(cel);
                    }
                }
                int k = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    row = new HtmlTableRow();
                    ComparisionGrid.Rows.Add(row);
                    if (k == 0)
                    {
                        row.Attributes.Add("class", "table_row2");
                        k += 1;
                    }
                    else
                    {
                        row.Attributes.Add("class", "table_row1");
                        k -= 1;
                    }
                    int qt = 0, am = 1, tc = 2;
                    for (int j = 1; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "Head" + i + j.ToString();
                        if (j > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                (CommonBLL.stringRowSeparators, StringSplitOptions.None);
                            HtmlTableCell celx = new HtmlTableCell();
                            System.Web.UI.WebControls.Label lblPrevRate = new System.Web.UI.WebControls.Label();
                            lblPrevRate.ID = "lblPrevRate" + j + i;
                            HtmlTableCell celx1 = new HtmlTableCell();
                            System.Web.UI.WebControls.Label lblSpec = new System.Web.UI.WebControls.Label();
                            lblSpec.ID = "lblSpec" + j + i;
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celz = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            celA.Align = "right";
                            HtmlTableCell celB = new HtmlTableCell();
                            System.Web.UI.WebControls.CheckBox chk = new System.Web.UI.WebControls.CheckBox();
                            chk.ID = "ch1" + j + i;
                            HiddenField lqtnid = new HiddenField();
                            lqtnid.ID = "lqtnid" + j + i;
                            HiddenField itdlsid = new HiddenField();
                            itdlsid.ID = "itdlsid" + j + i;
                            if (splitCols.Length > 1)
                            {
                                //celx.InnerText = splitCols[0];
                                //celx.ID = "lblRate" + j + i;
                                //celx.Attributes.Add("Title", "Previous Rates :\n" + splitCols[7].Replace(",", "\n"));
                                //celx.Attributes.Add("style", "tooltip");

                                StringBuilder sb = new StringBuilder();
                                sb.Append("<div id='mousefollow-examples'><div title='<b>Previous Rates :</b><br/>" +
                                    splitCols[7].Replace(",", "<br/>") + "'>" + splitCols[0] + "</div></div>");
                                //.Replace("Q.No.", "<font color='red'>Q.No.</font>")
                                lblPrevRate.Text = sb.ToString();
                                celx.Controls.Add(lblPrevRate);

                                //celx.Style.Add("<span>", "Previous Rates :\n" + splitCols[6].Replace(",", "\n"));
                                //lblSpec.Text = splitCols[1] + (splitCols[6].Trim() == "" ? "" : "<br/><br/><b><u>DEVIATION :</u></b><br/>" + splitCols[6]);
                                lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/><br/>") + (splitCols[6].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[6]);
                                celx1.Controls.Add(lblSpec);
                                cely.InnerText = splitCols[2];
                                # region NotInUSe
                                //if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                                //{
                                //    double dprc = Convert.ToDouble(splitCols[0].ToString()); int dcal = 1;
                                //    foreach (DataRow rcal in ds.Tables[1].Rows)
                                //    {
                                //        if (rcal["Description"].ToString() == "Discount(%)")
                                //        {
                                //            dprc = dprc - (Convert.ToDouble(rcal[dcal].ToString()) *
                                //                Convert.ToDouble(splitCols[0].ToString()) / 100);
                                //        }
                                //        if (rcal["Description"].ToString() == "Packing(%)")
                                //        {
                                //            dprc = dprc + (Convert.ToDouble(rcal[dcal].ToString()) *
                                //                Convert.ToDouble(splitCols[0].ToString()) / 100);
                                //        }
                                //        if (rcal["Description"].ToString() == "Excise Duty(%)")
                                //        {
                                //            dprc = dprc + (Convert.ToDouble(rcal[dcal].ToString()) *
                                //                Convert.ToDouble(splitCols[0].ToString()) / 100);
                                //        }
                                ////        if (rcal["Description"].ToString() == "Sales Tax(%)")
                                ////        {
                                ////            dprc = dprc + (Convert.ToDouble(rcal[dcal].ToString()) *
                                ////                Convert.ToDouble(splitCols[0].ToString()) / 100);
                                ////        }
                                ////    }
                                ////    celz.InnerText = dprc.ToString(); dcal++;
                                ////}
                                //else
                                # endregion
                                celz.InnerText = splitCols[5];
                                celA.InnerText = Convert.ToString(Math.Round(Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun), 2));
                                tot[j] += Math.Round(Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun), 2);
                                lqtnid.Value = splitCols[3];
                                itdlsid.Value = splitCols[4];
                                # region NotInUSe
                                //celz.InnerText = splitCols[3];
                                //if (splitCols[3] != null)
                                //{
                                //    string sltac, exid;
                                //    string disc = ds.Tables[1].Rows[1][tc].ToString();
                                //    if (ds.Tables[1].Rows[2][tc].ToString() == "Nill Against `CT1 & ARE1`inf")
                                //    {
                                //        exid = "0%";
                                //    }
                                //    else if (ds.Tables[1].Rows[2][tc].ToString() == "Not Applicableinf")
                                //    {
                                //        exid = "0%";
                                //    }
                                //    else
                                //    {
                                //        exid = ds.Tables[1].Rows[2][tc].ToString();
                                //    }
                                //    if (ds.Tables[1].Rows[3][tc].ToString() == "Nill Against Form `H`inf")
                                //    {
                                //        sltac = "0%";
                                //    }
                                //    else if (ds.Tables[1].Rows[3][tc].ToString() == "Not Applicableinf")
                                //    {
                                //        sltac = "0%";
                                //    }
                                //    else
                                //    {
                                //        sltac = ds.Tables[1].Rows[3][tc].ToString();
                                //    }
                                //    string pakg = ds.Tables[1].Rows[4][tc].ToString();
                                //    dis = Convert.ToDouble(disc.Replace("%", ""));
                                //    rate[qt] = Convert.ToDouble(celx.InnerText);
                                //    dis = Convert.ToDouble((dis * rate[qt]) / 100);
                                //    rate[qt] = rate[qt] - dis;
                                //    pak = Convert.ToDouble(pakg.Replace("%", ""));
                                //    pak = Convert.ToDouble(pak * (rate[qt] / 100));
                                //    rate[qt] = rate[qt] + pak;
                                //    exd = Convert.ToDouble(exid.Replace("%", ""));
                                //    exd = Convert.ToDouble((exd * (rate[qt] / 100)));
                                //    rate[qt] = rate[qt] + exd;
                                //    slt = Convert.ToDouble(sltac.Replace("%", ""));
                                //    slt = Convert.ToDouble(slt * (rate[qt] / 100));
                                //    rate[qt] = rate[qt] + slt;
                                //    celA.InnerText = String.Format("{0:0.0000}", Math.Round((rate[qt]), 4));
                                //    tot[j] += Math.Round((Convert.ToDouble(celA.InnerText) * Convert.ToDouble(qun)), 4);
                                //    qt++; am++;
                                //}
                                # endregion
                                celB.Controls.Add(chk);
                                celB.Controls.Add(lqtnid);
                                celB.Controls.Add(itdlsid);
                            }
                            tc++;
                            celx.Attributes.Add("class", "left_border");
                            celA.Align = "right";
                            celA.VAlign = "right";
                            row.Cells.Add(celx);
                            row.Cells.Add(celx1);
                            row.Cells.Add(cely);
                            row.Cells.Add(celz);
                            row.Cells.Add(celA);
                            row.Cells.Add(celB);
                        }
                        else
                        {
                            if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                            {
                                System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
                                lbl.ID = "ItmDesc" + i;
                                lbl.Text = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                lbl.ToolTip = ds.Tables[0].Rows[i].ItemArray[CommonBLL.CompareByItmsIgnoreColNo].ToString();
                                cel.Controls.Add(lbl);
                            }
                            else
                            {
                                cel.InnerText = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                if (ds.Tables[0].Rows[i].ItemArray[3] != "")
                                {
                                    qun = ds.Tables[0].Rows[i].ItemArray[3].ToString(); ;
                                }
                            }
                            if (j != CommonBLL.CompareByItmsIgnoreColNo)
                                row.Cells.Add(cel);
                            else
                            {
                                HiddenField hditmId = new HiddenField();
                                hditmId.ID = "hdn" + (i + 1).ToString();
                                hditmId.Value = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                cel.InnerText = (i + 1).ToString();
                                cel.Controls.Add(hditmId);
                                row.Cells.Add(cel);
                            }
                        }
                    }
                }
                row = new HtmlTableRow();
                HtmlTableCell celEmpty3 = new HtmlTableCell();
                HtmlTableCell celEmpty4 = new HtmlTableCell();
                HtmlTableCell celEmpty5 = new HtmlTableCell();
                ComparisionGrid.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;

                //row.Cells.Add(celEmpty3);
                //row.Cells.Add(celEmpty4);
                //row.Cells.Add(celEmpty5);
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 5; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    //cel1.InnerText = Convert.ToString(Math.Round(Convert.ToDecimal(tot[j]), 2));
                    cel1.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(tot[j]), 2));
                    cel1.ColSpan = 6;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    row.Cells.Add(cel1);
                }
                row = new HtmlTableRow();
                ComparisionGrid.Rows.Add(row);
                cel1 = new HtmlTableCell();
                celEmpty3 = new HtmlTableCell();
                celEmpty4 = new HtmlTableCell();
                celEmpty5 = new HtmlTableCell();
                cel1.InnerText = "Total Amount in Words";
                cel1.ColSpan = 4;
                //row.Cells.Add(celEmpty3);
                //row.Cells.Add(celEmpty4);
                //row.Cells.Add(celEmpty5);
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 5; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    string total = tot[j].ToString();
                    Decimal deci = Convert.ToDecimal(total);
                    //int total1 = Convert.ToInt32(deci);
                    //string CON = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                    //SqlConnection con = new SqlConnection(CON);
                    //SqlCommand cmd = new SqlCommand();
                    //cmd.CommandText = "select dbo.udf_Num_ToWords ('" + total1 + "')";
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Connection = con;
                    //SqlDataReader dr = default(SqlDataReader);
                    //con.Open();
                    //dr = cmd.ExecuteReader();
                    //dr.Read();
                    //string val = "";
                    //val = dr.GetString(0);
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string val = N2W.Num2WordConverter(deci.ToString(), "RS").ToString();
                    cel1.InnerText = val;
                    cel1.ColSpan = 6;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["Table"] = ComparisionGrid;
                //btnExcelExpt.Visible = true;
                //ImageButton1.Visible = true;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method for Comprision Terms & Conditions Table
        /// </summary>
        /// <param name="ds"></param>
        private void ComparBySupplTC(DataSet ds)
        {
            try
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid.Rows.Add(row);
                ComparisionGrid.Rows.Add(row1);
                HtmlTableCell cel = new HtmlTableCell();
                HtmlTableCell celEmpty6 = new HtmlTableCell();
                HtmlTableCell celEmpty7 = new HtmlTableCell();
                HtmlTableCell celEmpty8 = new HtmlTableCell();
                cel.ColSpan = 4;
                cel.InnerText = "Terms & Conditions";
                cel.ID = "TCHeading";

                row1.Cells.Add(cel);
                row1.Attributes.Add("class", "tearms");
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    row = new HtmlTableRow();
                    celEmpty6 = new HtmlTableCell();
                    celEmpty7 = new HtmlTableCell();
                    celEmpty8 = new HtmlTableCell();

                    ComparisionGrid.Rows.Add(row);
                    int k = 0;
                    for (int j = 0; j < ds.Tables[1].Rows[i].ItemArray.Length; j++)
                    {
                        cel = new HtmlTableCell();
                        cel.ID = i.ToString() + j.ToString();
                        cel.InnerText = ds.Tables[1].Rows[i].ItemArray[j].ToString();

                        if (j > 0)
                        {
                            if ((i == 0))
                            {
                                HtmlTableCell cel1 = new HtmlTableCell();
                                cel1.ColSpan = 6;

                                cel1.Attributes.Add("class", "left_border");
                                row1.Cells.Add(cel1);
                            }
                            cel.Attributes.Add("class", "left_border");
                        }

                        row.Cells.Add(cel);
                        if (k == 0)
                        {
                            row.Attributes.Add("class", "tearms1");
                            k += 1;
                        }
                        else
                        {
                            row.Attributes.Add("class", "tearms2");
                            k -= 1;
                        }
                        cel.ColSpan = 6;
                        if (j == 0)
                        {
                            cel.ColSpan = 4;
                        }
                    }
                }
                Session["ds"] = ComparisionGrid;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method for Comprision Items Table
        /// </summary>
        /// <param name="ds"></param>
        private void ComparBySuppl_Export(DataSet ds)
        {
            try
            {
                int emptyFlag = 0;
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid_Export.Border = 1;
                ComparisionGrid_Export.Rows.Add(row);
                ComparisionGrid_Export.Rows.Add(row1);
                HtmlTableCell cel1 = new HtmlTableCell();
                HtmlTableCell cel2 = new HtmlTableCell();
                HtmlTableCell cel3 = new HtmlTableCell();
                HtmlTableCell cel4 = new HtmlTableCell();
                HtmlTableCell cel5 = new HtmlTableCell();
                cel1.ColSpan = 4;
                row1.Cells.Add(cel1);
                row1.Attributes.Add("class", "table_heading");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit"];
                ArrayList ALSup = (ArrayList)Session["ALSup"];

                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    //string ColNm = ALSup[i - 1].ToString();

                    if (i == 5)
                    {
                        continue;
                    }
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i.ToString();
                        if (i < 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        else
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i >= CommonBLL.CompareByItmsSupplStColNo1)
                        {
                            //if (Convert.ToBoolean(dt.Rows[i - 1][ColNm + "_CB"].ToString()))
                            //{
                            cel.ColSpan = 6;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell(); celx.Align = "right";
                            HtmlTableCell celx1 = new HtmlTableCell(); celx1.Align = "right";
                            HtmlTableCell cely = new HtmlTableCell(); cely.Align = "right";
                            HtmlTableCell celz = new HtmlTableCell(); celz.Align = "right";
                            HtmlTableCell celA = new HtmlTableCell(); celA.Align = "right";
                            HtmlTableCell CellPr = new HtmlTableCell(); CellPr.Align = "left";
                            HtmlTableCell CellEmpty = new HtmlTableCell();
                            celx.InnerText = "Rate";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celz.InnerText = "QPrice";
                            celA.InnerText = "Amount";
                            CellPr.InnerText = "Previous Rates";
                            //CellPr.ColSpan = 2;
                            CellPr.Width = "359";
                            CellEmpty.InnerText = "";

                            if (emptyFlag == 0)
                            {

                            }
                            emptyFlag += 1;
                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
                            row1.Cells.Add(celz);
                            row1.Cells.Add(celA);
                            row1.Cells.Add(CellPr);
                            cel.Attributes.Add("class", "top_heading left_border");
                            //}
                        }
                        cel.InnerText = ds.Tables[0].Columns[i].ToString();
                        if (i > 6)
                        {
                            cel.ColSpan = 6;
                        }
                        row.Cells.Add(cel);
                    }
                    else
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.Attributes.Add("class", "top_heading");
                        cel.InnerText = "S.No.";
                        row.Cells.Add(cel);
                    }

                }
                int k = 0;
                //foreach (var item in ALSup)
                //{
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //string ColNm = item.ToString();
                    row = new HtmlTableRow();

                    if (k == 0)
                    {
                        row.Attributes.Add("class", "table_row2");
                        k += 1;
                    }
                    else
                    {
                        row.Attributes.Add("class", "table_row1");
                        k -= 1;
                    }
                    int qt = 0, am = 1, tc = 2;
                    for (int j = 1; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        if (j == 5)
                        {
                            continue;
                        }
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i + j.ToString();
                        if (j > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            if (Convert.ToBoolean(dtEdit.Rows[i]["IsCheck"].ToString()))
                            {
                                ComparisionGrid_Export.Rows.Add(row);
                                string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                    (CommonBLL.stringRowSeparators1, StringSplitOptions.None);
                                HtmlTableCell celx = new HtmlTableCell();
                                HtmlTableCell celx1 = new HtmlTableCell();
                                System.Web.UI.WebControls.Label lblSpec = new System.Web.UI.WebControls.Label();
                                lblSpec.ID = "lblSpec" + j + i;
                                HtmlTableCell cely = new HtmlTableCell();
                                HtmlTableCell celz = new HtmlTableCell();
                                HtmlTableCell celA = new HtmlTableCell();
                                celA.Align = "right";
                                HtmlTableCell CellPr = new HtmlTableCell();
                                CellPr.Align = "left";
                                HtmlTableCell CellEmpty = new HtmlTableCell();
                                HiddenField lqtnid = new HiddenField();
                                lqtnid.ID = "lqtnidEx" + j + i;
                                HiddenField itdlsid = new HiddenField();
                                itdlsid.ID = "itdlsidEx" + j + i;
                                if (splitCols.Length > 1)
                                {
                                    celx.InnerText = splitCols[0];
                                    lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                        "<br/><br/>") + (splitCols[6].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[6]);
                                    celx1.Controls.Add(lblSpec);
                                    cely.InnerText = splitCols[2];
                                    celz.InnerText = splitCols[5];
                                    celA.InnerText = (Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun)).ToString("N");
                                    string[] splt = splitCols[7].Split(','); string spllt = "";
                                    for (int ij = 0; ij < splt.Count(); ij++)
                                    {
                                        spllt = spllt + Environment.NewLine + splt[ij];
                                        //CellPr.Height = "60.00";
                                        //CellPr.Width = "43.00";
                                    }
                                    CellPr.InnerText = spllt;
                                    //CellPr.ColSpan = 2;
                                    CellEmpty.InnerText = "";
                                    tot[j] += Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun);
                                    lqtnid.Value = splitCols[3];
                                    itdlsid.Value = splitCols[4];
                                }
                                tc++;
                                celx.Attributes.Add("class", "left_border");
                                celA.Align = "right";
                                celA.VAlign = "right";

                                row.Cells.Add(celx);
                                row.Cells.Add(celx1);
                                row.Cells.Add(cely);
                                row.Cells.Add(celz);
                                row.Cells.Add(celA);
                                row.Cells.Add(CellPr);
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(dtEdit.Rows[i]["IsCheck"].ToString()))
                            {
                                if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                                {
                                    System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
                                    lbl.ID = "ItmDescEx" + i;
                                    lbl.Text = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                    lbl.ToolTip = ds.Tables[0].Rows[i].ItemArray[CommonBLL.CompareByItmsIgnoreColNo].ToString();
                                    cel.Controls.Add(lbl);
                                }
                                else
                                {
                                    cel.InnerText = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                    if (ds.Tables[0].Rows[i].ItemArray[3] != "")
                                    {
                                        qun = ds.Tables[0].Rows[i].ItemArray[3].ToString(); ;
                                    }
                                }
                                if (j != CommonBLL.CompareByItmsIgnoreColNo)
                                    row.Cells.Add(cel);
                                else
                                {
                                    HiddenField hditmId = new HiddenField();
                                    hditmId.ID = "hdnEx" + (i + 1).ToString();
                                    hditmId.Value = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                    cel.InnerText = (i + 1).ToString();
                                    cel.Controls.Add(hditmId);
                                    row.Cells.Add(cel);
                                }
                            }
                        }
                    }
                }
                //}
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel2 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    cel1.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(tot[j]), 2));
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    if (j >= 7)
                    {
                        cel1.ColSpan = 6;
                    }
                    row.Cells.Add(cel1);

                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel2 = new HtmlTableCell();
                cel3 = new HtmlTableCell();
                cel4 = new HtmlTableCell();
                cel1.InnerText = "Total Amount in Words";
                cel1.ColSpan = 4;

                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    System.Web.UI.WebControls.Label lblTotalAmt = new System.Web.UI.WebControls.Label();
                    lblTotalAmt.ID = "lblTotalAmt" + j;
                    string total = tot[j].ToString();
                    Decimal deci = Convert.ToDecimal(total);
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string val = "";
                    val = N2W.Num2WordConverter(deci.ToString(), "RS").ToString().Replace("(s)", "(s)<br/>");
                    lblTotalAmt.Text = val;
                    cel1.Align = "center";
                    cel1.Controls.Add(lblTotalAmt);
                    cel1.ColSpan = 6;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["TableEx"] = ComparisionGrid_Export;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method for Comprision Items Table
        /// </summary>
        /// <param name="ds"></param>
        private void ComparBySuppl_Export_All(DataSet ds)
        {
            try
            {
                int emptyFlag = 0;
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid_Export.Border = 1;
                ComparisionGrid_Export.Rows.Add(row);
                ComparisionGrid_Export.Rows.Add(row1);
                HtmlTableCell cel1 = new HtmlTableCell();
                HtmlTableCell cel2 = new HtmlTableCell();
                HtmlTableCell cel3 = new HtmlTableCell();
                HtmlTableCell cel4 = new HtmlTableCell();
                HtmlTableCell cel5 = new HtmlTableCell();
                cel1.ColSpan = 4;
                row1.Cells.Add(cel1);
                row1.Attributes.Add("class", "table_heading");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                ArrayList ALSup = (ArrayList)Session["ALSup"];

                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    //string ColNm = ALSup[i - 1].ToString();

                    if (i == 5)
                    {
                        continue;
                    }
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i.ToString();
                        if (i < 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        else
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i >= CommonBLL.CompareByItmsSupplStColNo1)
                        {
                            //if (Convert.ToBoolean(dt.Rows[i - 1][ColNm + "_CB"].ToString()))
                            //{
                            cel.ColSpan = 6;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell(); celx.Align = "right";
                            HtmlTableCell celx1 = new HtmlTableCell(); celx1.Align = "right";
                            HtmlTableCell cely = new HtmlTableCell(); cely.Align = "right";
                            HtmlTableCell celz = new HtmlTableCell(); celz.Align = "right";
                            HtmlTableCell celA = new HtmlTableCell(); celA.Align = "right";
                            HtmlTableCell CellPr = new HtmlTableCell(); CellPr.Align = "left";
                            HtmlTableCell CellEmpty = new HtmlTableCell();
                            celx.InnerText = "Rate";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celz.InnerText = "QPrice";
                            celA.InnerText = "Amount";
                            CellPr.InnerText = "Previous Rates";
                            //CellPr.ColSpan = 2;
                            CellPr.Width = "359";
                            CellEmpty.InnerText = "";

                            if (emptyFlag == 0)
                            {

                            }
                            emptyFlag += 1;
                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
                            row1.Cells.Add(celz);
                            row1.Cells.Add(celA);
                            row1.Cells.Add(CellPr);
                            cel.Attributes.Add("class", "top_heading left_border");
                            //}
                        }
                        cel.InnerText = ds.Tables[0].Columns[i].ToString();
                        if (i > 6)
                        {
                            cel.ColSpan = 6;
                        }
                        row.Cells.Add(cel);
                    }
                    else
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.Attributes.Add("class", "top_heading");
                        cel.InnerText = "S.No.";
                        row.Cells.Add(cel);
                    }

                }
                int k = 0;
                //foreach (var item in ALSup)
                //{
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //string ColNm = item.ToString();
                    row = new HtmlTableRow();

                    if (k == 0)
                    {
                        row.Attributes.Add("class", "table_row2");
                        k += 1;
                    }
                    else
                    {
                        row.Attributes.Add("class", "table_row1");
                        k -= 1;
                    }
                    int qt = 0, am = 1, tc = 2;
                    for (int j = 1; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        if (j == 5)
                        {
                            continue;
                        }
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i + j.ToString();
                        if (j > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            //if (Convert.ToBoolean(dt.Rows[i][ColNm + "_CB"].ToString()))
                            //{
                            ComparisionGrid_Export.Rows.Add(row);
                            string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                (CommonBLL.stringRowSeparators1, StringSplitOptions.None);
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            System.Web.UI.WebControls.Label lblSpec = new System.Web.UI.WebControls.Label();
                            lblSpec.ID = "lblSpec" + j + i;
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celz = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            celA.Align = "right";
                            HtmlTableCell CellPr = new HtmlTableCell();
                            CellPr.Align = "left";
                            HtmlTableCell CellEmpty = new HtmlTableCell();
                            HiddenField lqtnid = new HiddenField();
                            lqtnid.ID = "lqtnidEx" + j + i;
                            HiddenField itdlsid = new HiddenField();
                            itdlsid.ID = "itdlsidEx" + j + i;
                            if (splitCols.Length > 1)
                            {
                                celx.InnerText = splitCols[0];
                                lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/><br/>") + (splitCols[6].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[6]);
                                celx1.Controls.Add(lblSpec);
                                cely.InnerText = splitCols[2];
                                celz.InnerText = splitCols[5];
                                celA.InnerText = (Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun)).ToString("N");
                                string[] splt = splitCols[7].Split(','); string spllt = "";
                                for (int ij = 0; ij < splt.Count(); ij++)
                                {
                                    spllt = spllt + Environment.NewLine + splt[ij];
                                    //CellPr.Height = "60.00";
                                    //CellPr.Width = "43.00";
                                }
                                CellPr.InnerText = spllt;
                                //CellPr.ColSpan = 2;
                                CellEmpty.InnerText = "";
                                tot[j] += Convert.ToDouble(celz.InnerText) * Convert.ToDouble(qun);
                                lqtnid.Value = splitCols[3];
                                itdlsid.Value = splitCols[4];
                            }
                            tc++;
                            celx.Attributes.Add("class", "left_border");
                            celA.Align = "right";
                            celA.VAlign = "right";

                            row.Cells.Add(celx);
                            row.Cells.Add(celx1);
                            row.Cells.Add(cely);
                            row.Cells.Add(celz);
                            row.Cells.Add(celA);
                            row.Cells.Add(CellPr);
                            //}
                        }
                        else
                        {
                            //if (Convert.ToBoolean(dt.Rows[i][ColNm + "_CB"].ToString()))
                            //{
                            if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                            {
                                System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
                                lbl.ID = "ItmDescEx" + i;
                                lbl.Text = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                lbl.ToolTip = ds.Tables[0].Rows[i].ItemArray[CommonBLL.CompareByItmsIgnoreColNo].ToString();
                                cel.Controls.Add(lbl);
                            }
                            else
                            {
                                cel.InnerText = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                                if (ds.Tables[0].Rows[i].ItemArray[3] != "")
                                {
                                    qun = ds.Tables[0].Rows[i].ItemArray[3].ToString(); ;
                                }
                            }
                            if (j != CommonBLL.CompareByItmsIgnoreColNo)
                                row.Cells.Add(cel);
                            else
                            {
                                HiddenField hditmId = new HiddenField();
                                hditmId.ID = "hdnEx" + (i + 1).ToString();
                                hditmId.Value = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                cel.InnerText = (i + 1).ToString();
                                cel.Controls.Add(hditmId);
                                row.Cells.Add(cel);
                            }
                            //}
                        }
                    }
                    //}
                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel2 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    cel1.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(tot[j]), 2));
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    if (j >= 7)
                    {
                        cel1.ColSpan = 6;
                    }
                    row.Cells.Add(cel1);

                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel2 = new HtmlTableCell();
                cel3 = new HtmlTableCell();
                cel4 = new HtmlTableCell();
                cel1.InnerText = "Total Amount in Words";
                cel1.ColSpan = 4;

                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    System.Web.UI.WebControls.Label lblTotalAmt = new System.Web.UI.WebControls.Label();
                    lblTotalAmt.ID = "lblTotalAmt" + j;
                    string total = tot[j].ToString();
                    Decimal deci = Convert.ToDecimal(total);
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string val = "";
                    val = N2W.Num2WordConverter(deci.ToString(), "RS").ToString().Replace("(s)", "(s)<br/>");
                    lblTotalAmt.Text = val;
                    cel1.Align = "center";
                    cel1.Controls.Add(lblTotalAmt);
                    cel1.ColSpan = 6;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["TableEx"] = ComparisionGrid_Export;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Method for Comprision Terms & Conditions Table
        /// </summary>
        /// <param name="ds"></param>
        private void ComparBySupplTC_Export(DataSet ds)
        {
            try
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                ComparisionGrid_Export.Rows.Add(row1);
                HtmlTableCell cel = new HtmlTableCell();
                cel.ColSpan = 4;
                cel.InnerText = "Terms & Conditions";
                cel.ID = "TCHeadingEx";
                row1.Cells.Add(cel);
                row1.Attributes.Add("class", "tearms");
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                ArrayList ALSup = (ArrayList)Session["ALSup"];
                //foreach (var item in ALSup)
                //{
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    //string ColNm = item.ToString();
                    row = new HtmlTableRow();
                    ComparisionGrid_Export.Rows.Add(row);
                    int k = 0;
                    for (int j = 0; j < ds.Tables[1].Rows[i].ItemArray.Length; j++)
                    {
                        //if (Convert.ToBoolean(dt.Rows[i][ColNm + "_CB"].ToString()))
                        //{
                        cel = new HtmlTableCell();
                        cel.ID = "Ex" + i.ToString() + j.ToString();
                        cel.InnerText = ds.Tables[1].Rows[i].ItemArray[j].ToString();

                        if (j > 0)
                        {
                            if ((i == 0))
                            {
                                HtmlTableCell cel1 = new HtmlTableCell();
                                cel1.ColSpan = 6;
                                cel1.Attributes.Add("class", "left_border");
                                row1.Cells.Add(cel1);
                            }
                            cel.Attributes.Add("class", "left_border");
                        }
                        row.Cells.Add(cel);
                        if (k == 0)
                        {
                            row.Attributes.Add("class", "tearms1");
                            k += 1;
                        }
                        else
                        {
                            row.Attributes.Add("class", "tearms2");
                            k -= 1;
                        }
                        cel.ColSpan = 6;
                        if (j == 0)
                        {
                            cel.ColSpan = 4;
                        }
                        //}
                    }
                }
                Session["dsEx"] = ComparisionGrid_Export;
            }
            //}
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Check Box Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkb_CheckedChanged(object sender, EventArgs e)
        {

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
                if (divCompare.InnerHtml != "")
                {
                    if (Session["dsEx"] == null)
                        Export();
                    System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsEx"];
                    string Title = "Comparitive Statement of Local Quotations Received";
                    string attachment = "attachment; filename=Lcl_Quote_Comparison.xls";
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                    StringWriter stw = new StringWriter();
                    HtmlTextWriter htextw = new HtmlTextWriter(stw);
                    htextw.Write("<center><b>");
                    if (ddlCustomer.SelectedIndex != 0 && ddlEnquiry.SelectedIndex != 0)
                        Title = Title + " for " + ddlCustomer.SelectedItem.Text + ", Enq.No: " + ddlEnquiry.SelectedItem.Text + " ";
                    else if (ddlCustomer.SelectedIndex != 0)
                        Title = Title + " for " + ddlCustomer.SelectedItem.Text;
                    htextw.Write(Title + "</b></center>");
                    datatable.RenderControl(htextw);
                    HttpContext.Current.Response.Write(stw.ToString());
                    HttpContext.Current.Response.End();
                    Session["dsEx"] = null;
                }
            }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to PDF Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPdfExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (divCompare.InnerHtml != "")
                {
                    if (Session["dsEx"] == null)
                        Export();
                    System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsEx"];
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=LQCStatement.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter hw = new HtmlTextWriter(sw);
                    datatable.RenderControl(hw);
                    StringReader sr = new StringReader(sw.ToString());
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                    Response.End();
                    Session["dsEx"] = null;
                }
            }
            catch (ThreadAbortException ee)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Export to Word Buttton Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnWordExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (divCompare.InnerHtml != "")
                {
                    if (Session["dsEx"] == null)
                        Export();
                    System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsEx"];
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename=LQCStatement.doc");
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-word ";
                    System.IO.StringWriter sw = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);
                    datatable.RenderControl(hw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                    Response.End();
                    Session["dsEx"] = null;
                }
            }
            catch (ThreadAbortException ee)
            { }
            catch (Exception ex)
            {
                Session["dsEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
        {

        }

        /// <summary>
        /// This method is used to call for exporting.
        /// </summary>
        private void Export()
        {
            try
            {
                string Enquiry = String.Join(",", lbEnquiry.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ItemsTerms = CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagLSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                    new Guid(Session["CompanyId"].ToString()), new Guid(ddlCustomer.SelectedValue), Enquiry);
                if (ItemsTerms != null && ItemsTerms.Tables.Count > 1)
                {
                    DataTable dt = (DataTable)HttpContext.Current.Session["dt"];


                    if (!Convert.ToBoolean(Session["IsCheckedAll"].ToString()))
                        ComparBySuppl_Export_All(ItemsTerms);
                    else
                        ComparBySuppl_Export(ItemsTerms);
                    ComparBySupplTC_Export(ItemsTerms);
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison Exporting", ex.Message.ToString());
            }
        }

        #endregion

        # region Others

        /// <summary>
        /// Grid Veiw Row Data Bound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[0].CssClass = "locked";
                    e.Row.Cells[1].CssClass = "locked";
                    GridViewRow gvRow = e.Row;

                    for (int k = 9; k <= gvRow.Cells.Count; k++)
                    {
                        if (gvRow.Cells[0].Text != "" && gvRow.Cells[0].Text != "&nbsp;")
                        {
                            if (gvRow.Cells[k - 1].Text != "&nbsp;")
                            {
                                DataTable CompareDT = null; string ChkColName = "";
                                DataRowView dr = (DataRowView)e.Row.DataItem;
                                if (ViewState["CompareDT"] != null)
                                {
                                    CompareDT = (DataTable)ViewState["CompareDT"];
                                    ChkColName = CompareDT.Columns[k].ColumnName.ToString().Substring(0, CompareDT.Columns[k].ColumnName.ToString().LastIndexOf(_seperator)); ;
                                }
                                System.Web.UI.WebControls.CheckBox chk = new System.Web.UI.WebControls.CheckBox();
                                chk.EnableViewState = true;
                                chk.Enabled = true;
                                chk.ID = "chk" + ChkColName;
                                e.Row.Cells[k].Controls.Add(chk);

                            }
                            k = k + 5;
                        }
                        else
                        {

                        }
                    }

                }
                else if (e.Row.RowType == DataControlRowType.Header)
                {

                    GridViewRow gvRow = e.Row;
                    for (int k = 5; k <= gvRow.Cells.Count; k++)
                    {
                        DataTable CompareDT = null; string ChkColName = "";
                        DataRowView dr = (DataRowView)e.Row.DataItem;
                        if (ViewState["CompareDT"] != null)
                        {
                            CompareDT = (DataTable)ViewState["CompareDT"];
                            ChkColName = CompareDT.Columns[k].ColumnName.ToString().Substring(0, CompareDT.Columns[k].ColumnName.ToString().LastIndexOf(_seperator)); ;
                        }

                        System.Web.UI.WebControls.CheckBox chk = new System.Web.UI.WebControls.CheckBox();
                        chk.EnableViewState = true;
                        chk.Enabled = true;
                        chk.ID = "chkParent" + ChkColName;

                        e.Row.Cells[k].Controls.Add(chk);

                        k = k + 5;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// check Box Changed Evnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Pirnt Method
        /// </summary>
        /// <param name="filePath"></param>
        public void Printing(string filePath)
        {
            try
            {
                m_currentPageIndex = 0;
                m_streams = new FileStream(filePath, FileMode.Create);
                try
                {
                    const string printerName = "Microsoft XPS document Writer";
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    pd.PrinterSettings.PrinterName = printerName;
                    pd.DefaultPageSettings.Landscape = true;
                    pd.Print();
                }
                finally
                {
                    m_streams.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void Print()
        //{
        //    const string printerName =
        //       "Microsoft XPS document Writer";
        //    if (m_streams == null || m_streams.Count == 0)
        //        return;
        //    PrintDocument printDoc = new PrintDocument();
        //    printDoc.PrintController = new System.Drawing.Printing.StandardPrintController();

        //    //  ScriptManager.RegisterClientScriptBlock(this.Page, typeof(string), "print", "window.print();", true);

        //    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();

        //    DialogResult rs = pd.ShowDialog();

        //    if (rs == DialogResult.OK)
        //    {
        //        PrintDialog pt = new PrintDialog();
        //        printDoc.PrinterSettings.PrinterName = pd.PrinterSettings.PrinterName;

        //        // printDoc.DefaultPageSettings.Landscape = true;


        //        // printDoc.PrinterSettings.DefaultPageSettings.PaperSize =  "Legal";

        //        if (!printDoc.PrinterSettings.IsValid)
        //        {
        //            string msg = String.Format(
        //               "Can't find printer \"{0}\".", printerName);

        //            return;
        //        }
        //        printDoc.PrintPage += new PrintPageEventHandler(pd_PrintPage);
        //        printDoc.Print();
        //    }
        //}
        void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {

        }

        # endregion

        # region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckInd(int rowNo, int SupID, bool IsChecked)
        {
            string Res = "";
            try
            {
                ArrayList ALSup = (ArrayList)Session["ALSup"];
                string ColNm = ALSup[SupID].ToString();
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit"];
                for (int i = 0; i < ALSup.Count; i++)
                {
                    string colname = ALSup[i].ToString();
                    dt.Rows[rowNo][colname + "_CB"] = false;

                }
                if (Convert.ToInt32(dt.Rows[rowNo][ColNm + "_ItemStatus"].ToString()) <= 50)//60 Prev Comment **//PResent 40
                {
                    dt.Rows[rowNo][ColNm + "_CB"] = IsChecked;
                    dtEdit.Rows[rowNo]["IsCheck"] = IsChecked;
                    Res = "Checked";
                    //Session["IsCheckedAll"] = IsChecked;
                }
                else
                {
                    //Session["IsCheckedAll"] = IsChecked;
                }

                dt.AcceptChanges();

                for (int i = 0; i < ALSup.Count; i++)
                {
                    string colname = ALSup[i].ToString();
                    foreach (DataRow item in dt.Rows)
                    {
                        if (Convert.ToBoolean(item[colname + "_CB"].ToString()))
                        {
                            Session["IsCheckedAll"] = true;
                            //dtEdit.Rows[rowNo]["IsCheck"] = true;
                            goto FF;
                            //break;
                        }
                        else
                        {
                            Session["IsCheckedAll"] = false;
                            //dtEdit.Rows[rowNo]["IsCheck"] = false;
                        }
                    }
                    //Session["IsCheckedAll"] = false;
                }
                dtEdit.AcceptChanges();
            FF: ;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
                Res = "Error While Checking.";
            }
            return Res;
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.Read)]
        public string CheckHeader(int SupID, bool IsChecked)
        {
            string Res = "";
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                ArrayList ALSup = (ArrayList)Session["ALSup"];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit"];
                string ColNm = ALSup[SupID].ToString();
                for (int i = 0; i < ALSup.Count; i++)
                {
                    string colname = ALSup[i].ToString();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        dt.Rows[j][colname + "_CB"] = false;
                        if (dt.Rows[j][ColNm + "_IDID"].ToString() != "" && Convert.ToInt32(dt.Rows[j][ColNm + "_ItemStatus"].ToString()) <= 50)//60 Prev Comment ** Present //40
                        {
                            dt.Rows[j][ColNm + "_CB"] = IsChecked;
                            dtEdit.Rows[j]["IsCheck"] = IsChecked;
                            Session["IsCheckedAll"] = IsChecked;
                        }
                        else
                        {
                            Session["IsCheckedAll"] = IsChecked;
                            dtEdit.Rows[j]["IsCheck"] = IsChecked;
                        }
                    }
                }
                dt.AcceptChanges();
                dtEdit.AcceptChanges();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
                Res = "Error While Checking.";
            }
            return Res;
        }

        # endregion
    }
}
