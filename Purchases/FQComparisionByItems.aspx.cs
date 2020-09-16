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
using Ajax;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace VOMS_ERP.Purchases
{
    public partial class FQComparisionByItems : System.Web.UI.Page
    {
        # region variables
        int res = -999;
        LQuotaitonBLL NLQBL = new LQuotaitonBLL();
        NewEnquiryBLL NEBLL = new NewEnquiryBLL();
        ComparisonStmntBLL CSBL = new ComparisonStmntBLL();
        CustomerBLL CSTRBL = new CustomerBLL();
        CommonBLL CBLL = new CommonBLL();
        ErrorLog ELog = new ErrorLog();
        StringBuilder sbb = new StringBuilder();
        int ColNo = 0;
        string Custid = Guid.Empty.ToString();
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
                        Ajax.Utility.RegisterTypeForAjax(typeof(FQComparisionByItems));
                        btnSave.Attributes.Add("OnClick", "javascript:return Myvalidations()");
                        if (!IsPostBack)
                        {
                            ClearAll();
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind Form Data

        /// <summary>
        /// Calling all Methods for Default Data
        /// </summary>
        private void GetData()
        {
            try
            {
                BindDropDownList(ddlcustmr, CSTRBL.SelectCustomers(CommonBLL.FlagRegularDRP, Guid.Empty, new Guid(Session["CompanyId"].ToString())));

                if (CommonBLL.CustmrContactTypeText == (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    if (Request.QueryString["CsID"] != null)
                        Custid = Request.QueryString["CsID"].ToString();
                    else
                        Custid = Session["Custmr_SuplrID"].ToString();
                    BindDropDownList(ddlcustmr, CSTRBL.SelectCustomers(CommonBLL.FlagCSelect, new Guid(Session["UserID"].ToString()), new Guid(Session["CompanyID"].ToString())));
                    ddlcustmr.SelectedValue = Custid;
                    BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagPSelectAll, Guid.Empty, Guid.Empty, new Guid(Custid), Guid.Empty, "", DateTime.Now, "",
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    if (Request.QueryString["FeqID"] != null)
                    {
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (System.Web.UI.WebControls.ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s.ToLower().Trim()) item.Selected = true;
                            }
                        }
                    }
                    BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                           new Guid(ddlcustmr.SelectedValue), Request.QueryString["FeqID"]));
                    HFFQSelectedItems.Value = GetSelectedItems(Request.QueryString["FeqID"]);
                    ddlcustmr.Visible = false;
                    lblCustName.Visible = false;
                }
                else
                {
                    if (Request.QueryString["CsID"] != null && Request.QueryString["CsID"].ToString() != "" &&
                                    Request.QueryString["FeqID"] != null && Request.QueryString["FeqID"].ToString() != "")
                    {
                        Lstfenqy.Enabled = false;
                        ddlcustmr.Enabled = false;
                        ddlcustmr.SelectedValue = Request.QueryString["CsID"].ToString();
                        BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagPSelectAll, Guid.Empty, Guid.Empty,
                            new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "", "", DateTime.Now, DateTime.Now,
                            DateTime.Now, "", 60, "", "", new Guid(Session["UserID"].ToString()),
                            new Guid(Session["UserID"].ToString()), true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                        string[] states = Request.QueryString["FeqID"].Split(',');
                        foreach (string s in states)
                        {
                            foreach (System.Web.UI.WebControls.ListItem item in Lstfenqy.Items)
                            {
                                if (item.Value == s.ToLower().Trim()) item.Selected = true;
                            }
                        }
                        BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty,
                            new Guid(ddlcustmr.SelectedValue), Request.QueryString["FeqID"]));
                        HFFQSelectedItems.Value = GetSelectedItems(Request.QueryString["FeqID"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind DropDownLists using DropDown and Dataset
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="CommonDt"></param>
        private void BindDropDownList(DropDownList ddl, DataSet CommonDt)
        {
            try
            {
                ddl.DataSource = CommonDt;
                ddl.DataTextField = "Description";
                ddl.DataValueField = "ID";
                ddl.DataBind();
                ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", Guid.Empty.ToString()));

                if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                {
                    CommonDt.Tables[0].DefaultView.Sort = "Description ASC";
                    Lstfenqy.DataSource = CommonDt.Tables[0];
                    Lstfenqy.DataTextField = "Description";
                    Lstfenqy.DataValueField = "ID";
                    Lstfenqy.DataBind();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Clear All Inputs
        /// </summary>
        protected void ClearAll()
        {
            try
            {

                if (CommonBLL.CustmrContactTypeText != (((ArrayList)Session["UserDtls"])[7].ToString()))
                {
                    Lstfenqy.Items.Clear();
                    ddlcustmr.SelectedIndex = -1;
                }
                else
                    Lstfenqy.SelectedIndex = -1;
                ComparisionGrid.DataBind();
                divCompare.InnerHtml = "";
                Session["dt"] = null;
                Session["ALSup"] = null;
                Session["ItemIDs"] = null;
                Session["dtEdit_FQ"] = null;
                Session["IsCheckedAll_FQ"] = false;
                Lstfenqy.Enabled = true;
                ddlcustmr.Enabled = true;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }
        #endregion

        #region Bind Comparison Table

        /// <summary>
        /// Binding Items Table for Comprision
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
                    Session["dtEdit_FQ"] = IT;

                    ComparBySuppl_1(ItemsTerms);
                }
                else
                {
                    divCompare.InnerHtml = "";
                    Session["Table"] = null;
                    Session["ds"] = null;
                    Session["TableFQEx"] = null;
                    Session["dsFQEx"] = null;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Designing/Formating Comparison Table
        /// </summary>
        /// <param name="ds"></param>
        string qun = "0";
        private void ComparBySuppl(DataSet ds)
        {
            try
            {
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
                        if (i <= 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        else
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            cel.ColSpan = 5;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            HtmlTableCell celB = new HtmlTableCell();
                            CheckBox chkb = new CheckBox();
                            chkb.ID = "ch1" + i;
                            chkb.Attributes.Add("onclick", "javascript:CheckAll('" + chkb.ID + "');");

                            celx.InnerText = "Rate($)";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celA.InnerText = "Amount($)";
                            celB.InnerText = "Select";
                            celB.Controls.Add(chkb);

                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
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
                            HtmlTableCell celx1 = new HtmlTableCell();
                            System.Web.UI.WebControls.Label lblSpec = new System.Web.UI.WebControls.Label();
                            lblSpec.ID = "lblSpec" + j + i;
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            celA.Align = "right";
                            HtmlTableCell celB = new HtmlTableCell();
                            CheckBox chk = new CheckBox();
                            chk.ID = "ch1" + j + i;
                            chk.Attributes.Add("onclick", "javascript:CheckIndividual('" + chk.ID.ToString() + "','" + (i + 1).ToString() + "');");
                            HiddenField lqtnid = new HiddenField();
                            lqtnid.ID = "fqtnid" + j + i;
                            HiddenField itdlsid = new HiddenField();
                            itdlsid.ID = "itdlsid" + j + i;
                            if (splitCols.Length > 1)
                            {
                                celx.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(splitCols[0]), 2)); ;
                                lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/><br/>") + (splitCols[5].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[5]);
                                celx1.Controls.Add(lblSpec);
                                cely.InnerText = splitCols[2];
                                celA.InnerText = String.Format("{0:0.00}", Convert.ToString(Convert.ToDouble(splitCols[0]) *
                                    Convert.ToDouble(qun)));
                                tot[j] += Convert.ToDouble(splitCols[0]) * Convert.ToDouble(qun);
                                lqtnid.Value = splitCols[3];
                                itdlsid.Value = splitCols[4];
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
                            row.Cells.Add(celA);
                            row.Cells.Add(celB);
                        }
                        else
                        {
                            if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                            {
                                Label lbl = new Label();
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
                ComparisionGrid.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 5; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    cel1.InnerText = String.Format("{0:0.0000}", Math.Round(Convert.ToDecimal(tot[j]), 4));
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    row.Cells.Add(cel1);
                }
                row = new HtmlTableRow();
                ComparisionGrid.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "USD($): Total Amount in Words";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 5; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    string total = tot[j].ToString();
                    Decimal deci = Convert.ToDecimal(total);
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string val = N2W.Num2WordConverter(deci.ToString(), "").ToString();
                    cel1.InnerText = val;
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["Table"] = ComparisionGrid;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }


        private void ComparBySuppl_1(DataSet ds)
        {
            try
            {
                # region Test

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
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 5)
                    {
                        continue;
                    }
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        if (i > CommonBLL.FQCompareByItmsSupplStColNo1)
                        {
                            string FQName = ds.Tables[0].Columns[i].ToString().Trim();
                            string FQNm = ColNo.ToString();
                            dt.Columns.Add(FQNm + "_LQID", typeof(Guid));
                            dt.Columns.Add(FQNm + "_IDID", typeof(Guid));
                            if (!dt.Columns.Contains("FEnqID"))
                                dt.Columns.Add("FEnqID", typeof(string));
                            DataColumn dc = new DataColumn(FQNm + "_CB", typeof(bool));
                            dc.DefaultValue = false;
                            dt.Columns.Add(dc);
                            ALSup.Add(FQNm);

                            sbh.Append("<th colspan='5' style='border-right: 1px solid #A09797'>" + FQName + "</th>");
                            sbh1.Append("<th>Spec.</th>");
                            sbh1.Append("<th>Make</th>");
                            sbh1.Append("<th>Rate($)</th>");
                            sbh1.Append("<th>Amount($)</th>");
                            sbh1.Append("<th><input type='checkbox' onclick='javascript:CheckAll(" + i + "," + ColNo + ");' name='ch1" + i + "' id='ch1" + i + "'/></th>");
                            NoofSups++;
                            ColNo++;
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
                        if (j > CommonBLL.FQCompareByItmsSupplStColNo1)
                        {
                            if (j > CommonBLL.FQCompareByItmsSupplStColNo1)
                            {

                                string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                (CommonBLL.stringRowSeparators1, StringSplitOptions.None);
                                if (splitCols.Length > 1)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    sb.Append("<div id='mousefollow-examples'><div title='<b>Previous Rates :</b><br/>" +
                                        splitCols[6].Replace(",", "<br/>") + "'>" + splitCols[0] + "</div></div>");

                                    string dev = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/>") + (splitCols[5].Trim() == "" ? "" : "<span style='font-weight:bold;'><u>DEVIATION</u>:</span> " + splitCols[5]);

                                    sbb.Append("<td>" + dev + "</td>");
                                    sbb.Append("<td>" + splitCols[2] + "</td>");
                                    //sbb.Append("<td align='right'>" +  + "</td>");
                                    sbb.Append("<td align='right'><div id='mousefollow-examples'><div title='<b>Previous Rates :</b><br/>" +
                                        splitCols[6].Replace(",", "<br/>") + "'>" + splitCols[0] + "</div></div></td>");
                                    decimal Count = Convert.ToDecimal(Math.Round(Convert.ToDouble(splitCols[0]) * Convert.ToDouble(qun), 2));
                                    tot[j] += Math.Round(Convert.ToDouble(splitCols[0]) * Convert.ToDouble(qun), 2);
                                    sbb.Append("<td align='right'>" + Count.ToString("N") + "</td>");
                                    sbb.Append("<td style='border-right: 1px solid #A09797'>");
                                    sbb.Append("<input class='checkRow' id='ch1" + j + "" + i
                                        + "' type='checkbox' onclick='javascript:CheckIndividual(" + j + "" + i + "," + (i + 1) + "," + SupCounting + ")'>");
                                    sbb.Append("<input id='fenqid" + j + "" + i + "' type='hidden' value='" + ds.Tables[0].Rows[i]["ForeignEnquireId"].ToString() + "'>");
                                    sbb.Append("<input id='fqtnid" + j + "" + i + "' type='hidden' value='" + splitCols[3] + "'>");
                                    sbb.Append("<input id='itdlsid" + j + "" + i + "' type='hidden' value='" + splitCols[4] + "'>");
                                    sbb.Append("</td>");
                                    dr["ItemID"] = new Guid(ds.Tables[0].Rows[i]["ItemId"].ToString());
                                    dr[ALSup[SupCounting] + "_LQID"] = new Guid(splitCols[3]);
                                    dr[ALSup[SupCounting] + "_IDID"] = new Guid(splitCols[4]);
                                    dr[ALSup[SupCounting] + "_CB"] = false;
                                    dr["FEnqID"] = ds.Tables[0].Rows[i]["ForeignEnquireId"].ToString();
                                    SupCounting++;
                                }
                                else
                                {
                                    dr[ALSup[SupCounting] + "_CB"] = false;
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td></td>");
                                    sbb.Append("<td style='border-right: 1px solid #A09797;'><input id='ch1" + j + ""
                                        + i + "' type='checkbox' onclick='CheckIndividual("
                                        + j + "" + i + "," + (i + 1) + ")' style='display:none;'></td>");
                                    SupCounting++;
                                }
                            }
                        }
                        else
                        {
                            if (j == 1)
                                sbb.Append("<td>" + (i + 1).ToString() + "</td>");
                            else
                                sbb.Append("<td>" + ds.Tables[0].Rows[i].ItemArray[j].ToString()
                                    + "<input id='hdn" + (i + 1) + "' type='hidden' value='"
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

                sbb.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold;'>Total Amount : </span></td>");
                sbb.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                nsb.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold;'>Total Amount in Words : </span></td>");
                nsb.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                nsb1.Append("<tr><td></td><td colspan='3' align='right'><span style='font-weight:bold; color: red;'>Terms & Conditions : </span></td>");
                nsb1.Append("<td style = 'display:none'></td><td style = 'display:none'></td>");
                for (int s = 6; s < tot.Length; s++)
                {
                    string total = tot[s].ToString();
                    Decimal deci = Math.Round(Convert.ToDecimal(total));
                    clsNum2WordBLL N2W = new clsNum2WordBLL();
                    string words = N2W.Num2WordConverter(deci.ToString(), "$").ToString();


                    sbb.Append("<td colspan='4' align='right'><span style='font-weight:bold;'>" + deci.ToString("N") + "</span></td>");
                    sbb.Append("<td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style='border-right: 1px solid #A09797'></td>");
                    nsb.Append("<td colspan='5' style='border-right: 1px solid #A09797'  align='center'><span style='font-weight:bold;'>" + words + "</span></td>");
                    nsb.Append("<td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td>");
                    nsb1.Append("<td></td><td></td><td></td><td></td><td style='border-right: 1px solid #A09797'></td>");
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
                            sbb.Append("<td colspan='5' style='border-right: 1px solid #A09797'>" + ds.Tables[1].Rows[i].ItemArray[j].ToString() + "</td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td><td style = 'display:none'></td>");
                    }
                    sbb.Append("<td></td></tr>");
                }

                sbb.Append("</tbody>");
                sbb.Append("</table>");
                divCompare.InnerHtml = sbb.ToString();
                Session["dt"] = dt;
                Session["ALSup"] = ALSup;
                # endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }


        /// <summary>
        /// Designing/Formating Terms & Conditions in the Comparison Table
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
                cel.ColSpan = 4;
                cel.InnerText = "Terms & Conditions";
                cel.ID = "TCHeading";
                row1.Cells.Add(cel);
                row1.Attributes.Add("class", "tearms");
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    row = new HtmlTableRow();
                    ComparisionGrid.Rows.Add(row);
                    int k = 0;
                    for (int j = 0; j < ds.Tables[1].Rows[i].ItemArray.Length; j++)
                    {
                        cel = new HtmlTableCell();
                        cel.ID = i.ToString() + j.ToString();
                        cel.InnerText = ds.Tables[1].Rows[i].ItemArray[j].ToString().Replace("inf", "");
                        if (cel.InnerText == "Not Applicableinf")
                            cel.InnerText = "Not Applicable";
                        else if (cel.InnerText == "Nill Against Form `H`inf")
                            cel.InnerText = "Nill Against Form `H`";
                        else if (cel.InnerText == "Nill Against `CT1 & ARE1`inf")
                            cel.InnerText = "Nill Against `CT1 & ARE1`";
                        if (j > 0)
                        {
                            if ((i == 0))
                            {
                                HtmlTableCell cel1 = new HtmlTableCell();
                                cel1.ColSpan = 5;
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
                        cel.ColSpan = 5;
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
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
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid_Export.Border = 1;
                ComparisionGrid_Export.Rows.Add(row);
                ComparisionGrid_Export.Rows.Add(row1);
                HtmlTableCell cel1 = new HtmlTableCell();
                cel1.ColSpan = 4;
                row1.Cells.Add(cel1);
                row1.Attributes.Add("class", "table_heading");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit_FQ"];
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 5)
                    {
                        continue;
                    }
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i.ToString();
                        if (i <= 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        else
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            cel.ColSpan = 4;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            HtmlTableCell celPR = new HtmlTableCell(); celPR.Align = "left";

                            celx.InnerText = "Rate($)";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celA.InnerText = "Amount($)";
                            celPR.InnerText = "Previous Rates";
                            celPR.Width = "431";
                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
                            row1.Cells.Add(celA);
                            row1.Cells.Add(celPR);
                            cel.Attributes.Add("class", "top_heading left_border");
                        }
                        cel.InnerText = ds.Tables[0].Columns[i].ToString();
                        if (i >= 6)
                        {
                            cel.ColSpan = 5;
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
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
                                lblSpec.ID = "lblSpecEx" + j + i;
                                HtmlTableCell cely = new HtmlTableCell();
                                HtmlTableCell celA = new HtmlTableCell();
                                celA.Align = "right";
                                HtmlTableCell celPrr = new HtmlTableCell();
                                celPrr.Align = "left";
                                if (splitCols.Length > 1)
                                {
                                    celx.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(splitCols[0]), 2)); ;
                                    lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                        "<br/><br/>") + (splitCols[5].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[5]);
                                    celx1.Controls.Add(lblSpec);
                                    cely.InnerText = splitCols[2];
                                    celA.InnerText = String.Format("{0:0.00}", Convert.ToString(Convert.ToDouble(splitCols[0]) *
                                        Convert.ToDouble(qun)));
                                    tot[j] += Convert.ToDouble(splitCols[0]) * Convert.ToDouble(qun);
                                    string[] splt = splitCols[6].Split(','); string spllt = "";
                                    for (int ij = 0; ij < splt.Count(); ij++)
                                    {
                                        spllt = spllt + Environment.NewLine + splt[ij];
                                    }
                                    celPrr.InnerText = spllt;
                                }
                                tc++;
                                celx.Attributes.Add("class", "left_border");
                                celA.Align = "right";
                                celA.VAlign = "right";
                                row.Cells.Add(celx);
                                row.Cells.Add(celx1);
                                row.Cells.Add(cely);
                                row.Cells.Add(celA);
                                row.Cells.Add(celPrr);
                            }

                        }
                        else
                        {
                            if (Convert.ToBoolean(dtEdit.Rows[i]["IsCheck"].ToString()))
                            {
                                if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                                {
                                    Label lbl = new Label();
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
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    cel1.InnerText = String.Format("{0:0.0000}", Math.Round(Convert.ToDecimal(tot[j]), 4));
                    cel1.ColSpan = 4;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    if (j >= 7)
                    {
                        cel1.ColSpan = 5;
                    }
                    row.Cells.Add(cel1);
                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "USD($): Total Amount in Words";
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
                    val = N2W.Num2WordConverter(deci.ToString(), "").ToString();
                    lblTotalAmt.Text = val;
                    cel1.InnerText = val;
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["TableFQEx"] = ComparisionGrid_Export;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
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
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableRow row1 = new HtmlTableRow();
                ComparisionGrid_Export.Border = 1;
                ComparisionGrid_Export.Rows.Add(row);
                ComparisionGrid_Export.Rows.Add(row1);
                HtmlTableCell cel1 = new HtmlTableCell();
                cel1.ColSpan = 4;
                row1.Cells.Add(cel1);
                row1.Attributes.Add("class", "table_heading");
                double[] tot = new double[ds.Tables[0].Columns.Count];
                double[] dec = new double[ds.Tables[0].Columns.Count];
                double dis, exd, pak, slt;
                double[] rate = new double[ds.Tables[0].Columns.Count];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit_FQ"];
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    if (i == 5)
                    {
                        continue;
                    }
                    if (i > CommonBLL.CompareByItmsIgnoreColNo)
                    {
                        HtmlTableCell cel = new HtmlTableCell();
                        cel.ID = "HeadEx" + i.ToString();
                        if (i <= 4)
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        else
                        {
                            cel.Attributes.Add("class", "top_heading");
                        }
                        if (i > CommonBLL.CompareByItmsSupplStColNo)
                        {
                            cel.ColSpan = 4;
                            cel.Align = "center";
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            HtmlTableCell celPR = new HtmlTableCell(); celPR.Align = "left";

                            celx.InnerText = "Rate($)";
                            celx.Attributes.Add("class", "left_border");
                            celx1.InnerText = "Spec.";
                            cely.InnerText = "Make";
                            celA.InnerText = "Amount($)";
                            celPR.InnerText = "Previous Rates";
                            celPR.Width = "431";
                            row1.Cells.Add(celx);
                            row1.Cells.Add(celx1);
                            row1.Cells.Add(cely);
                            row1.Cells.Add(celA);
                            row1.Cells.Add(celPR);
                            cel.Attributes.Add("class", "top_heading left_border");
                        }
                        cel.InnerText = ds.Tables[0].Columns[i].ToString();
                        if (i >= 6)
                        {
                            cel.ColSpan = 5;
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
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
                            //if (Convert.ToBoolean(dtEdit.Rows[i]["IsCheck"].ToString()))
                            //{
                            ComparisionGrid_Export.Rows.Add(row);
                            string[] splitCols = ds.Tables[0].Rows[i].ItemArray[j].ToString().Split
                                (CommonBLL.stringRowSeparators1, StringSplitOptions.None);
                            HtmlTableCell celx = new HtmlTableCell();
                            HtmlTableCell celx1 = new HtmlTableCell();
                            System.Web.UI.WebControls.Label lblSpec = new System.Web.UI.WebControls.Label();
                            lblSpec.ID = "lblSpecEx" + j + i;
                            HtmlTableCell cely = new HtmlTableCell();
                            HtmlTableCell celA = new HtmlTableCell();
                            celA.Align = "right";
                            HtmlTableCell celPrr = new HtmlTableCell();
                            celPrr.Align = "left";
                            if (splitCols.Length > 1)
                            {
                                celx.InnerText = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(splitCols[0]), 2)); ;
                                lblSpec.Text = (splitCols[1].Trim() == "" ? "" : splitCols[1].Trim() +
                                    "<br/><br/>") + (splitCols[5].Trim() == "" ? "" : "<u><b>DEVIATION: </b></u><br/>" + splitCols[5]);
                                celx1.Controls.Add(lblSpec);
                                cely.InnerText = splitCols[2];
                                celA.InnerText = String.Format("{0:0.00}", Convert.ToString(Convert.ToDouble(splitCols[0]) *
                                    Convert.ToDouble(qun)));
                                tot[j] += Convert.ToDouble(splitCols[0]) * Convert.ToDouble(qun);
                                string[] splt = splitCols[6].Split(','); string spllt = "";
                                for (int ij = 0; ij < splt.Count(); ij++)
                                {
                                    spllt = spllt + Environment.NewLine + splt[ij];
                                }
                                celPrr.InnerText = spllt;
                            }
                            tc++;
                            celx.Attributes.Add("class", "left_border");
                            celA.Align = "right";
                            celA.VAlign = "right";
                            row.Cells.Add(celx);
                            row.Cells.Add(celx1);
                            row.Cells.Add(cely);
                            row.Cells.Add(celA);
                            row.Cells.Add(celPrr);
                            //}

                        }
                        else
                        {
                            //if (Convert.ToBoolean(dtEdit.Rows[i]["IsCheck"].ToString()))
                            //{
                            if (j == (CommonBLL.CompareByItmsIgnoreColNo - 1))
                            {
                                Label lbl = new Label();
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
                                hditmId.ID = "hdnEx" + (i + 1).ToString();
                                hditmId.Value = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                cel.InnerText = (i + 1).ToString();
                                cel.Controls.Add(hditmId);
                                row.Cells.Add(cel);
                            }
                            //}
                        }
                    }
                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "Total Amount";
                cel1.ColSpan = 4;
                row.Cells.Add(cel1);
                row.Attributes.Add("class", "total_amount");
                for (int j = 6; j < tot.Length; j++)
                {
                    cel1 = new HtmlTableCell();
                    cel1.InnerText = String.Format("{0:0.0000}", Math.Round(Convert.ToDecimal(tot[j]), 4));
                    cel1.ColSpan = 4;
                    cel1.Attributes.Add("class", "left_border");
                    cel1.Align = "right";
                    if (j >= 7)
                    {
                        cel1.ColSpan = 5;
                    }
                    row.Cells.Add(cel1);
                }
                row = new HtmlTableRow();
                ComparisionGrid_Export.Rows.Add(row);
                cel1 = new HtmlTableCell();
                cel1.InnerText = "USD($): Total Amount in Words";
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
                    val = N2W.Num2WordConverter(deci.ToString(), "").ToString();
                    lblTotalAmt.Text = val;
                    cel1.InnerText = val;
                    cel1.ColSpan = 5;
                    cel1.Attributes.Add("class", "left_border");
                    row.Cells.Add(cel1);
                }
                Session["TableFQEx"] = ComparisionGrid_Export;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
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
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    row = new HtmlTableRow();
                    ComparisionGrid_Export.Rows.Add(row);
                    int k = 0;
                    for (int j = 0; j < ds.Tables[1].Rows[i].ItemArray.Length; j++)
                    {
                        cel = new HtmlTableCell();
                        cel.ID = "Ex" + i.ToString() + j.ToString();
                        cel.InnerText = ds.Tables[1].Rows[i].ItemArray[j].ToString().Replace("inf", "");
                        if (cel.InnerText == "Not Applicableinf")
                            cel.InnerText = "Not Applicable";
                        else if (cel.InnerText == "Nill Against Form `H`inf")
                            cel.InnerText = "Nill Against Form `H`";
                        else if (cel.InnerText == "Nill Against `CT1 & ARE1`inf")
                            cel.InnerText = "Nill Against `CT1 & ARE1`";
                        if (j > 0)
                        {
                            if ((i == 0))
                            {
                                HtmlTableCell cel1 = new HtmlTableCell();
                                cel1.ColSpan = 4;
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
                        cel.ColSpan = 4;
                        if (j == 0)
                        {
                            cel.ColSpan = 4;
                        }
                    }
                }
                Session["dsFQEx"] = ComparisionGrid_Export;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }


        #endregion

        #region Button Click Events and DropDown Selected Index Changed Events

        /// <summary>
        /// Customer DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlcustmr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;
                if (Session["dt"] != null && ((DataTable)Session["dt"]).Rows.Count > 0)
                    dt = (DataTable)Session["dt"];
                if (ddlcustmr.SelectedValue != Guid.Empty.ToString())
                {
                    BindDropDownList(ddlfenqy, NEBLL.NewEnquiryEdit(CommonBLL.FlagPSelectAll, Guid.Empty, Guid.Empty, new Guid(ddlcustmr.SelectedValue), Guid.Empty, "", DateTime.Now, "",
                        "", DateTime.Now, DateTime.Now, DateTime.Now, "", 60, "", "", Guid.Empty, Guid.Empty, true, new Guid(Session["CompanyID"].ToString()), CommonBLL.EmptyDt()));
                    ddlfenqy.SelectedValue = Guid.Empty.ToString();
                    divCompare.InnerHtml = "";
                }
                else
                {
                    ddlfenqy.SelectedValue = Guid.Empty.ToString();
                    divCompare.InnerHtml = "";
                    if (dt != null && dt.Rows.Count > 0)
                        dt.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Frn Enquiry DropDownList Selected Index Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Lstfenqy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Lstfenqy.SelectedValue != "")
                {
                    string ForEnqID = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                    BindItemsTable(CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyID"].ToString()),
                        new Guid(ddlcustmr.SelectedValue), ForEnqID));
                    HFFQSelectedItems.Value = GetSelectedItems(ForEnqID);
                }
                else
                    divCompare.InnerHtml = "";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList ALSup = (ArrayList)Session["ALSup"];
                string FoREnquID = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataTable dt = null;
                if (Session["dt"] != null)
                {
                    dt = (DataTable)Session["dt"];
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < ALSup.Count; i++)
                    {
                        string colname = ALSup[i].ToString();
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            string ItemDetailsID = dt.Rows[j][colname + "_IDID"].ToString().Trim();
                            string FQID = dt.Rows[j][colname + "_LQID"].ToString().Trim();
                            string FEnqID = dt.Rows[j]["FEnqID"].ToString().Trim();
                            if (Convert.ToBoolean(dt.Rows[j][colname + "_CB"].ToString()) && ItemDetailsID != "" && FQID != "")
                            {
                                res = CSBL.InsertDeleteBasketItems(CommonBLL.FlagINewInsert, Guid.Empty, new Guid(ItemDetailsID),
                                            new Guid(FEnqID), Guid.Empty, new Guid(FQID), Guid.Empty, Guid.Empty, 45, new Guid(Session["UserID"].ToString()), "", new Guid(Session["CompanyID"].ToString()));
                            }
                        }
                    }
                    if (res == 0 && btnSave.Text == "Select")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "SuccessMessage('Selected Successfully.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/Log"), "Foreign Quotation Comparison", "Data Inserted Successfully.");
                        CBLL.ClearUploadedFiles();
                        Response.Redirect("ConfromationBasket.aspx?CsID=" + ddlcustmr.SelectedValue + "&FeqID=" + FoREnquID, false);
                        ClearAll();
                    }
                    else if (res != 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('Error while Inserting.');", true);
                        ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Foreign Quotation Comparison", "Error While Saving");
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('No Items to Save.');", true);
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
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
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Foreign Quotation Comparison", ex.Message.ToString());
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
                if (Session["dsFQEx"] == null)
                    Export();
                System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsFQEx"];

                string Title = "Foreign Quotation Comparision Statement";
                string attachment = "attachment; filename=Frn_Quote_Comparison.xls";
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.AddHeader("content-disposition", attachment);
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                StringWriter stw = new StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                htextw.Write("<center><b>");
                if (ddlcustmr.SelectedIndex != 0 && Lstfenqy.SelectedIndex != 0)
                    Title = Title + " for " + ddlcustmr.SelectedItem.Text + ", Enq.No: " + Lstfenqy.SelectedItem.Text + " ";
                else if (ddlcustmr.SelectedIndex != 0)
                    Title = Title + " for " + ddlcustmr.SelectedItem.Text;
                htextw.Write(Title + "</b></center>");
                datatable.RenderControl(htextw);
                HttpContext.Current.Response.Write(stw.ToString());
                HttpContext.Current.Response.End();
                Session["dsFQEx"] = null;
            }
            catch (Exception ex)
            {
                Session["dsFQEx"] = null;
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
                if (Session["dsFQEx"] == null)
                    Export();
                System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsFQEx"];
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=FQCStatement.pdf");
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
                Response.End();
                Session["dsFQEx"] = null;
            }
            catch (ThreadAbortException ee)
            { }
            catch (Exception ex)
            {
                Session["dsFQEx"] = null;
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
                if (Session["dsFQEx"] == null)
                    Export();
                System.Web.UI.HtmlControls.HtmlTable datatable = (System.Web.UI.HtmlControls.HtmlTable)Session["dsFQEx"];
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=FQCStatement.doc");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-word ";
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw);
                datatable.RenderControl(hw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                Session["dsFQEx"] = null;
            }
            catch (ThreadAbortException ee)
            { }
            catch (Exception ex)
            {
                Session["dsFQEx"] = null;
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Rendering Method for Export
        /// </summary>
        /// <param name="control"></param>
        public override void VerifyRenderingInServerForm(Control control)
        {

        }

        /// <summary>
        /// This method is used to call for exporting.
        /// </summary>
        private void Export()
        {
            try
            {
                string ForEnqID = String.Join(",", Lstfenqy.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray());
                DataSet ItemsTerms = CSBL.SelectFrCmprsnStmnt(CommonBLL.FlagFSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, new Guid(Session["CompanyId"].ToString())
                    , new Guid(ddlcustmr.SelectedValue), ForEnqID);
                if (ItemsTerms != null && ItemsTerms.Tables.Count > 1)
                {
                    if (!Convert.ToBoolean(Session["IsCheckedAll_FQ"].ToString()))
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

        # region WebMethods

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string GetSelectedItems(string FEID)
        {
            ItemStatusBLL ISBLL = new ItemStatusBLL();
            DataSet items = new DataSet();
            Dictionary<string, string> ItemIDs = new Dictionary<string, string>();
            items = ISBLL.GetItemStatus(CommonBLL.FlagKSelect, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.StatusTypeFPOrder, FEID, "", "", "", "", "");
            if (items.Tables.Count > 0 && items.Tables[0].Rows.Count > 0)
            {
                string ids = items.Tables[0].Rows[0][0].ToString().Trim();
                if (ids != "")
                    ItemIDs = ids.Split(',').ToDictionary(key => key.Trim(), value => value.Trim());
                else
                {
                    if (ItemIDs != null)
                        ItemIDs.Clear();
                }
                Session["ItemIDs"] = ItemIDs;
                return ids;
            }
            return "";
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string CheckAll(int QNo, bool IsChecked)
        {
            string Res = "";
            try
            {
                ArrayList ALSup = (ArrayList)Session["ALSup"];
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit_FQ"];
                Dictionary<string, string> ItemIDs = (Dictionary<string, string>)HttpContext.Current.Session["ItemIDs"];
                if (dt.Rows.Count > 0)
                {
                    string ColNm = ALSup[QNo].ToString();
                    for (int i = 0; i < ALSup.Count; i++)
                    {
                        string colname = ALSup[i].ToString();
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            dt.Rows[j][colname + "_CB"] = false;
                            if (dt.Rows[j][colname + "_IDID"].ToString().Trim() != "" && ColNm == colname)
                            {
                                string FenqID = dt.Rows[j]["FEnqID"].ToString();
                                string ItmID = dt.Rows[j]["ItemId"].ToString();
                                string gr = dt.Rows[j]["FEnqID"].ToString().ToUpperInvariant() + "_" + dt.Rows[j]["ItemId"].ToString().ToUpperInvariant();
                                if (ItemIDs != null && ItemIDs.ContainsKey(gr.Trim()))
                                {
                                    dt.Rows[j][colname + "_CB"] = false;
                                    dtEdit.Rows[j]["IsCheck"] = false;
                                    Session["IsCheckedAll_FQ"] = false;
                                }
                                if (ItemIDs != null && !ItemIDs.ContainsKey(gr.Trim()))
                                {
                                    dt.Rows[j][ColNm + "_CB"] = IsChecked;
                                    dtEdit.Rows[j]["IsCheck"] = IsChecked;
                                    Session["IsCheckedAll_FQ"] = IsChecked;
                                }
                                else if (ItemIDs == null)
                                {
                                    dt.Rows[j][ColNm + "_CB"] = IsChecked;
                                    dtEdit.Rows[j]["IsCheck"] = IsChecked;
                                    Session["IsCheckedAll_FQ"] = IsChecked;
                                }
                            }
                        }
                    }
                    dt.AcceptChanges();
                    dtEdit.AcceptChanges();
                }
                else
                    Res = "No Rows";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
                Res = "Exception";
            }
            return Res;
        }

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ChecIndividual(int RowNo, int QNo, bool IsChecked)
        {
            string Res = "";
            try
            {
                ArrayList ALSup = (ArrayList)HttpContext.Current.Session["ALSup"];
                DataTable dt = (DataTable)HttpContext.Current.Session["dt"];
                DataTable dtEdit = (DataTable)HttpContext.Current.Session["dtEdit_FQ"];
                Dictionary<string, string> ItemIDs = (Dictionary<string, string>)HttpContext.Current.Session["ItemIDs"];
                if (dt.Rows.Count > 0)
                {
                    string ColNm = ALSup[QNo].ToString();

                    for (int i = 0; i < ALSup.Count; i++)
                    {
                        string colname = ALSup[i].ToString();
                        dt.Rows[RowNo - 1][colname + "_CB"] = false;
                    }
                    string gr = dt.Rows[RowNo - 1]["FEnqID"].ToString() + "_" + dt.Rows[RowNo - 1]["ItemId"].ToString();

                    if (ItemIDs != null && ItemIDs.ContainsKey(gr.Trim()))
                    {
                        dt.Rows[RowNo - 1][QNo + "_CB"] = false;
                        dtEdit.Rows[RowNo - 1]["IsCheck"] = false;
                    }
                    if (ItemIDs != null && !ItemIDs.ContainsKey(gr.Trim()))
                    {
                        dt.Rows[RowNo - 1][QNo + "_CB"] = IsChecked;
                        dtEdit.Rows[RowNo - 1]["IsCheck"] = IsChecked;
                    }
                    else
                    {
                        dt.Rows[RowNo - 1][QNo + "_CB"] = IsChecked;
                        dtEdit.Rows[RowNo - 1]["IsCheck"] = IsChecked;
                    }
                    dt.AcceptChanges();


                    for (int i = 0; i < ALSup.Count; i++)
                    {
                        string colname = ALSup[i].ToString();
                        foreach (DataRow item in dt.Rows)
                        {
                            if (Convert.ToBoolean(item[colname + "_CB"].ToString()))
                            {
                                Session["IsCheckedAll_FQ"] = true;
                                //dtEdit.Rows[rowNo]["IsCheck"] = true;
                                goto FF;
                                //break;
                            }
                            else
                            {
                                Session["IsCheckedAll_FQ"] = false;
                                //dtEdit.Rows[rowNo]["IsCheck"] = false;
                            }
                        }
                        //Session["IsCheckedAll"] = false;
                    }
                FF: ;
                }
                else
                    Res = "No Rows";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int lineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Quotations/ErrorLog"), "Local Quotation Comparison", ex.Message.ToString());
                Res = "Exception";
            }
            return Res;
        }

        # endregion
    }
}
