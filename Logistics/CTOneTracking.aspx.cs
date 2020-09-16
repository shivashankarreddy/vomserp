using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using System.Data;
using System.Text;
using VOMS_ERP.Admin;
using System.IO;

namespace VOMS_ERP.Logistics
{
    public partial class CTOneTracking : System.Web.UI.Page
    {
        # region variables
        ErrorLog ELog = new ErrorLog();
        CT1DetailsBLL CT1BLL = new CT1DetailsBLL();
        CT1TrackingBLL CT1TBLL = new CT1TrackingBLL();
        AuditLogs ALS = new AuditLogs();
        static string Filename = "";
        static DataTable tblARE;        
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                    Response.Redirect("../Login.aspx?logout=yes", false);
                else
                {
                    txtunUtilisedDt.Attributes.Add("readonly", "readonly");
                    Ajax.Utility.RegisterTypeForAjax(typeof(CTOneTracking));
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
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Generation", ex.Message.ToString());
            }
        }

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

        private void GetData()
        {
            try
            {
                Guid Qury = Guid.Empty;
                char Flag = CommonBLL.FlagCSelect;
                LblAmount.Text= "Amount("+Session["CurrencySymbol"].ToString().Trim()+")";
                if (Request.QueryString["ID"] != null)
                {
                    Flag = CommonBLL.FlagFSelect;
                    Qury = new Guid(Request.QueryString["ID"]);
                }

                DataSet ds = CT1BLL.GetDataSet(Flag, Qury ,new Guid(Session["CompanyID"].ToString()));
                BindDropDownList(ds, ddlCTOneNo);
                if (Request.QueryString["ID"] != null)
                    EditRercord(new Guid(Request.QueryString["ID"].ToString()));
                ViewState["EditID"] = Request.QueryString["ID"];
                
                

               
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }

        private void BindDropDownList(DataSet ds, DropDownList ddl)
        {
            try
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddl.DataSource = ds.Tables[0];
                    ddl.DataTextField = "Description";
                    ddl.DataValueField = "ID";
                    ddl.DataBind();                    
                }
                ddl.Items.Insert(0, new ListItem("-- Select --", Guid.Empty.ToString()));
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }

        private void AddColumns()
        {            
            try
            {
                DataColumn dc = new DataColumn("Check", typeof(bool));
                dc.DefaultValue = false;
                tblARE.Columns.Add(dc);
                tblARE.Columns.Add("ExiseINVNo", typeof(string));
                tblARE.Columns.Add("ExINVval", typeof(decimal));
                tblARE.Columns.Add("ExINVDT", typeof(DateTime));
                tblARE.Columns.Add("SalesINVNo", typeof(string));
                tblARE.Columns.Add("SalesINVDT", typeof(DateTime));
                tblARE.Columns.Add("SalesINVAmt", typeof(decimal));
                dc = new DataColumn("ARE1Forms", typeof(string));
                dc.DefaultValue = "false,false,false,false,false";
                tblARE.Columns.Add(dc);                
                dc = new DataColumn("SBCopy", typeof(bool));
                dc.DefaultValue = false;
                tblARE.Columns.Add(dc);
                dc = new DataColumn("ExchCopy", typeof(bool));
                dc.DefaultValue = false;
                tblARE.Columns.Add(dc);
                dc = new DataColumn("ExpCopy", typeof(bool));
                dc.DefaultValue = false;
                tblARE.Columns.Add(dc);                
                dc = new DataColumn("IsSevottam", typeof(bool));
                dc.DefaultValue = false;
                tblARE.Columns.Add(dc);                
                tblARE.AcceptChanges();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }

        private void EditRercord(Guid ID)
        {
            try
            {
                DataSet dss = CT1TBLL.GetDataSet(CommonBLL.FlagModify, new Guid(Request.QueryString["ID"].ToString()), Guid.Empty, Guid.Empty, Guid.Empty, CommonBLL.StartDate, CommonBLL.StartDate, new Guid(Session["CompanyID"].ToString()));
                if (dss != null && dss.Tables.Count >= 2 && dss.Tables[0].Rows.Count > 0 && dss.Tables[1].Rows.Count > 0 && dss.Tables[2].Rows.Count > 0)
                {
                    ddlCTOneNo.SelectedValue = dss.Tables[0].Rows[0]["CTOneID"].ToString();
                    //txtCToneno.Text = dss.Tables[0].Rows[0]["CT1ReferenceNo"].ToString();
                    txtCTOneVal.Text = dss.Tables[0].Rows[0]["CTOneVal"].ToString();
                    txtFpoNos.Text = dss.Tables[0].Rows[0]["FPONos"].ToString();
                    txtLpoNos.Text = dss.Tables[0].Rows[0]["LPONos"].ToString();
                    txtSupplierName.Text = dss.Tables[0].Rows[0]["SupplierNm"].ToString();
                    txtCustomerName.Text = dss.Tables[0].Rows[0]["CustmrNm"].ToString();
                    HFFpos.Value = dss.Tables[0].Rows[0]["FPOs"].ToString();
                    HFLpos.Value = dss.Tables[0].Rows[0]["LPOs"].ToString();
                    HFSupplierID.Value = dss.Tables[0].Rows[0]["SupID"].ToString();
                    HFCustID.Value = dss.Tables[0].Rows[0]["CustID"].ToString();

                    if (Convert.ToDecimal(dss.Tables[0].Rows[0]["UnUsedAmt"].ToString()) > 0)
                    {
                        chkIsUnutilised.Checked = true;
                        txtunutilisedAmt.Text = dss.Tables[0].Rows[0]["UnUsedAmt"].ToString();
                        txtunUtilisedDt.Text = dss.Tables[0].Rows[0]["UnUsedDate"].ToString();
                    }

                    tblARE = dss.Tables[2].Copy();
                    AddColumns();
                    DataTable dt = new DataTable();
                    dt = dss.Tables[1].Copy();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string ARE1ID = "";
                        string GRNID = "";
                        string PrfINVID = "";
                        string ShpngBillID = "";
                        string BLID = "";
                        string AWBID = "";
                        string MRcptID = "";
                        string BRCID = "";

                        Guid AreID = new Guid(dt.Rows[i]["ARE1ID"].ToString());
                        DataRow[] foundRows = tblARE.Select("ARE1ID = '" + AreID + "'");
                        if (foundRows.Length > 0)
                        {
                            ARE1ID = foundRows[0]["ARE1ID"].ToString();
                            GRNID = foundRows[0]["GRNID"].ToString();
                            PrfINVID = foundRows[0]["PrfINVID"].ToString();
                            ShpngBillID = foundRows[0]["ShpngBillID"].ToString();
                            if (foundRows[0]["BLID"].ToString() != Guid.Empty.ToString() && foundRows[0]["BLID"].ToString() != "")
                            {
                                BLID = foundRows[0]["BLID"].ToString();
                            }
                            else
                            {
                                BLID = foundRows[0]["AWBID"].ToString();
                            }
                            MRcptID = foundRows[0]["MRcptID"].ToString();
                            BRCID = foundRows[0]["BRCID"].ToString();

                            if (ARE1ID != "" && GRNID != "" && PrfINVID != "" && ShpngBillID != "" && BLID != "" && MRcptID != "" && BRCID != "")
                            {
                                int RowIndex = tblARE.Rows.IndexOf(foundRows[0]);
                                tblARE.Rows[RowIndex]["Check"] = true;
                                tblARE.Rows[RowIndex]["ExiseINVNo"] = dt.Rows[i]["ExiseINVNo"].ToString();
                                tblARE.Rows[RowIndex]["ExINVval"] = dt.Rows[i]["ExINVval"].ToString();
                                tblARE.Rows[RowIndex]["ExINVDT"] = dt.Rows[i]["ExINVDT"].ToString();
                                tblARE.Rows[RowIndex]["SalesINVNo"] = dt.Rows[i]["SalesINVNo"].ToString();
                                tblARE.Rows[RowIndex]["SalesINVDT"] = dt.Rows[i]["SalesINVDT"].ToString();
                                tblARE.Rows[RowIndex]["SalesINVAmt"] = dt.Rows[i]["SalesINVAmt"].ToString();
                                tblARE.Rows[RowIndex]["ARE1Forms"] = dt.Rows[i]["ARE1Forms"].ToString();
                                tblARE.Rows[RowIndex]["SBCopy"] = dt.Rows[i]["SBCopy"].ToString();
                                tblARE.Rows[RowIndex]["ExchCopy"] = dt.Rows[i]["ExchCopy"].ToString();
                                tblARE.Rows[RowIndex]["ExpCopy"] = dt.Rows[i]["ExpCopy"].ToString();
                                tblARE.Rows[RowIndex]["IsSevottam"] = dt.Rows[i]["IsSevottam"].ToString();
                            }
                        }                        
                    }
                    tblARE.AcceptChanges();

                    //DataColumn dc = new DataColumn("Check", typeof(bool));
                    //dc.DefaultValue = true;
                    //tblARE.Columns.Add(dc);

                    DivAREOneDetails.InnerHtml = FillAREDetails(tblARE);
                    ViewState["EditID"] = Request.QueryString["ID"];
                    ddlCTOneNo.Enabled = false;
                    DivComments.Visible = true;
                    btnSave.Text = "Update";
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }

        private string FillAREDetails(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                decimal GTotal = 0;
                sb.Append("<table width='100%' cellspacing='0' cellpadding='0' border='0' id='rounded-corner' class='rounded-corner'>" +
                "<thead align='left'><tr><th class='rounded-First' width='3%'>Select</th><th width='3%'>SNo</th><th class='rounded-Last' width='94%'>ARE-1 Details</th></tr></thead>");
                sb.Append("<tbody class='bcGridViewMain'>");
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GTotal += Convert.ToDecimal(dt.Rows[i]["ARE1Value"].ToString());
                        HFAccordions.Value = dt.Rows.Count.ToString();
                        string sno = (i + 1).ToString();
                        string isSevottam = "";
                        if (Convert.ToBoolean(dt.Rows[i]["IsSevottam"]))
                            isSevottam = " disabled='disabled' ";
                        sb.Append("<tr valign='top'>");
                        sb.Append("<td valign='top'><input id='ChkFullRow" + sno + "' type='checkbox' onchange='CheckFullRow(" + sno + ")'");
                        if (Convert.ToBoolean(dt.Rows[i]["Check"]))
                            sb.Append(" checked='checked' ");
                        sb.Append(" name='ChkFullRow" + sno + "' " + isSevottam + " /></td>");
                        sb.Append("<td align='center'>" + sno + "</td>");
                        sb.Append("<td valign='top'>");

                        # region ARE-1 Details

                        sb.Append("<div id='accordion" + sno + "' style='font-size:small; text-shadow=0px 0px #000000;'><h5>ARE-1 NO : " + dt.Rows[i]["ARE1No"].ToString() + "</h5><div>");
                        sb.Append("<table width='100%'>");

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblARENo' class='bcLabel'>ARE-1 Number :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtARENo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["ARE1No"].ToString() + "' /></td>");

                        string ARE1Date = "";
                        if (dt.Rows[i]["ARE1Date"].ToString() != "")
                            ARE1Date = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["ARE1Date"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblAREDT' class='bcLabel'>ARE-1 Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtAREDT" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ARE1Date + "' /></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblAREVal' class='bcLabel'>ARE-1 Value :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtAREVal" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["ARE1Value"].ToString() + "' /></td>");
                        sb.Append("</tr>");

                        # region ARE-Forms
                        string[] arefrms = dt.Rows[i]["ARE1Forms"].ToString().Split(',');
                        sb.Append("<tr>");
                        //sb.Append("<td colspan='6' class='bcTdnormal'>");
                        //sb.Append("<table width='100%'>");
                        //sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'>Original(O) <input id='ChkOrig" + sno
                            + "' type='checkbox' onchange='CheckAres(" + sno + ")' " + isSevottam + " "
                            + ((arefrms != null && arefrms.Length > 0 && arefrms[0].ToString() != "" && Convert.ToBoolean(arefrms[0])) ? (" checked='checked' ") : "") + " /></td>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'>Duplicate(D) <input id='ChkDup" + sno
                            + "' type='checkbox' onchange='CheckAres(" + sno + ")' " + isSevottam + "  "
                            + ((arefrms != null && arefrms.Length > 1 && arefrms[1].ToString() != "" && Convert.ToBoolean(arefrms[1])) ? (" checked='checked' ") : "") + "/></td>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'>Triplicate(P) <input id='ChkTrip" + sno
                            + "' type='checkbox' onchange='CheckAres(" + sno + ")' " + isSevottam + "  "
                            + ((arefrms != null && arefrms.Length > 2 && arefrms[2].ToString() != "" && Convert.ToBoolean(arefrms[2])) ? (" checked='checked' ") : "") + "/></td>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'>Green(G) <input id='ChkGreen" + sno
                            + "' type='checkbox' onchange='CheckAres(" + sno + ")' " + isSevottam + "  "
                            + ((arefrms != null && arefrms.Length > 3 && arefrms[3].ToString() != "" && Convert.ToBoolean(arefrms[3])) ? (" checked='checked' ") : "") + "/></td>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'>Blue(B) <input id='ChkBlue" + sno
                            + "' type='checkbox' onchange='CheckAres(" + sno + ")' " + isSevottam + "  "
                            + ((arefrms != null && arefrms.Length > 4 && arefrms[4].ToString() != "" && Convert.ToBoolean(arefrms[4])) ? (" checked='checked' ") : "") + "/></td>");
                        sb.Append("<td class='rounded-corner1' valign='baseline'></td>");
                        //sb.Append("</tr>");
                        //sb.Append("</table>");
                        //sb.Append("</td>");
                        sb.Append("</tr>");
                        # endregion

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblGRNNo' class='bcLabel'>GRN Number :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtGRNNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["GRNNo"].ToString() + "' /></td>");

                        string GRNDate = "";
                        if (dt.Rows[i]["GRNDate"].ToString() != "")
                            GRNDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["GRNDate"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblGRNDT' class='bcLabel'>GRN Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtGRNDT" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + GRNDate + "' /></td>");

                        if (dt.Rows[i]["BillofLadingNo"].ToString() != "")
                        {
                            sb.Append("<td class='rounded-corner1'><span id='lblAREVal' class='bcLabel'>Bill of Lading No :</span></td>");
                            sb.Append("<td class='rounded-corner1'><input id='txtAREVal" + sno + "' class='bcAsptextbox' " +
                                " type='text' readonly='readonly' value='" + dt.Rows[i]["BillofLadingNo"].ToString() + "' /></td>");
                            //sb.Append("<td class='rounded-corner1'></td>");
                            //sb.Append("<td class='rounded-corner1'></td>");
                            sb.Append("</tr>");
                        }
                        else 
                        {
                            sb.Append("<td class='rounded-corner1'><span id='lblAREValk' class='bcLabel'>AirWay Bill No :</span></td>");
                            sb.Append("<td class='rounded-corner1'><input id='txtAREVal" + sno + "' class='bcAsptextbox' " +
                                " type='text' readonly='readonly' value='" + dt.Rows[i]["AWBNumber"].ToString() + "' /></td>");
                            //sb.Append("<td class='rounded-corner1'></td>");
                            //sb.Append("<td class='rounded-corner1'></td>");
                            sb.Append("</tr>");

                        }

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblPrfInvNo' class='bcLabel'>Proforma INV No :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txPrfInvNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["PrfmInvcNo"].ToString() + "' /></td>");

                        string PrfINVDate = "";
                        if (dt.Rows[i]["PrfmaInvcDt"].ToString() != "")
                            PrfINVDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["PrfmaInvcDt"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblPrfInvDT' class='bcLabel'>Proforma INV DT :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtPrfINVDt" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + PrfINVDate + "' /></td>");

                        sb.Append("<td class='rounded-corner1'></td>");
                        sb.Append("<td class='rounded-corner1'></td>");                        
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblShpngBlNo' class='bcLabel'>Shipping Bill No :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtShpngBlNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["ShpngBilNmbr"].ToString() + "' /></td>");

                        string ShpngDate = "";
                        if (dt.Rows[i]["ShpngBilDate"].ToString() != "")
                            ShpngDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["ShpngBilDate"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblShpngBilDT' class='bcLabel'>Shipping Bill DT :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtShpngBilDT" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + ShpngDate + "' /></td>");

                        sb.Append("<td class='rounded-corner1'></td>");
                        sb.Append("<td class='rounded-corner1'></td>");
                        sb.Append("</tr>");

                        string MRDate = "";
                        if (dt.Rows[i]["MRDate"].ToString() != "")
                            MRDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["MRDate"].ToString()));
                        
                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblMTRNo' class='bcLabel'>Mate Receipt No :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtMTRNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["MReceiptNmbr"].ToString() + "' /></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblMateRcptDT' class='bcLabel'>Mate Receipt Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtMateRcptDT" + sno + "' class='bcAsptextbox' " +
                            " type='text' value='" + MRDate + "' readonly='readonly' /></td>");

                        sb.Append("<td class='rounded-corner1'></td>");
                        sb.Append("<td class='rounded-corner1'></td>");                         
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblEPCopy' class='bcLabel'>Ep S/B Copy :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='ChkSBCopy" + sno + "'  " + isSevottam + " "
                            + " type='checkbox' onchange='CheckSBCopy(" + sno + ")' "
                            + (Convert.ToBoolean(dt.Rows[i]["SBCopy"].ToString()) ? (" checked='checked' ") : "") + "/></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblExchCopy' class='bcLabel'>Exch S/B Copy :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='ChkExchCopy" + sno + "'  " + isSevottam + " "
                            + " type='checkbox' onchange='ChkExchCopy(" + sno + ")' "
                            + (Convert.ToBoolean(dt.Rows[i]["ExchCopy"].ToString()) ? (" checked='checked' ") : "") + "/></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblExpCopy' class='bcLabel'>Exp S/B Copy :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='ChkExpCopy" + sno + "'  " + isSevottam + " "
                            + " type='checkbox' onchange='ChkExpCopy(" + sno + ")' "
                            + (Convert.ToBoolean(dt.Rows[i]["ExpCopy"].ToString()) ? (" checked='checked' ") : "") + "/></td>");
                        sb.Append("</tr>");
                                                
                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblBRCNo' class='bcLabel'>BRC Number :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txBRCNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["BRCNo"].ToString() + "' /></td>");

                        string BRCDate = "";
                        if (dt.Rows[i]["BRCDate"].ToString() != "")
                            BRCDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["BRCDate"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblBRCDt' class='bcLabel'>BRC Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtBRCDt" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + BRCDate + "' /></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblBRCAmt' class='bcLabel'>BRC-Amount :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtBRCAmt" + sno + "' class='bcAsptextbox' " +
                            " type='text' readonly='readonly' value='" + dt.Rows[i]["BRCAmt"].ToString() + "' /></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblSalesInvNo' class='bcLabel'>Sales Invoice No. :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtSalesInvNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' value='" + dt.Rows[i]["SalesINVNo"].ToString() + "'  " + isSevottam + " onchange='SaveChanges(" + sno + ")' /></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblSalesInvAmt' class='bcLabel'>Sales Inv Amount :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtSalesInvAmt" + sno + "' class='bcAsptextbox' " +
                            " type='text' value='" + dt.Rows[i]["SalesINVAmt"].ToString() + "'  " + isSevottam + " onchange='SaveChanges(" + sno
                            + ")' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");

                        string Date = "";
                        if (dt.Rows[i]["SalesINVDT"].ToString() != "")
                            Date = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["SalesINVDT"].ToString()));


                        sb.Append("<td class='rounded-corner1'><span id='lblSalesInvDT' class='bcLabel'>Sales Invoice Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtSalesInvDT" + sno + "' class='bcAsptextbox DatePicker' " +
                            " type='text' value='" + Date + "'  " + isSevottam + "   onchange='SaveChanges(" + sno + ")' readonly='readonly' /></td>");
                        //+ dt.Rows[i]["SalesINVDT"].ToString() == "" ? "" : CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["SalesINVDT"].ToString())) 
                        //+ "'  onchange='SaveChanges(" + sno + ")' /></td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td class='rounded-corner1'><span id='lblExInvNo' class='bcLabel'>Excise Invoice :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtExInvNo" + sno + "' class='bcAsptextbox' " +
                            " type='text' value='" + dt.Rows[i]["ExiseINVNo"].ToString() + "'  " + isSevottam + "  onchange='SaveChanges(" + sno + ")' /></td>");

                        sb.Append("<td class='rounded-corner1'><span id='lblExInvVal' class='bcLabel'>Ex. Inv Value :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtExInvVal" + sno + "' class='bcAsptextbox' "
                            + " type='text' value='" + dt.Rows[i]["ExINVval"].ToString() + "'  " + isSevottam + "  onchange='SaveChanges(" + sno
                            + ")' onkeyup='extractNumber(this,2,false);' onkeypress='return blockNonNumbers(this, event, true, false);' /></td>");

                        string ExcInvDate = "";
                        if (dt.Rows[i]["ExINVDT"].ToString() != "")
                            ExcInvDate = CommonBLL.DateDisplay(Convert.ToDateTime(dt.Rows[i]["ExINVDT"].ToString()));

                        sb.Append("<td class='rounded-corner1'><span id='lblExInvVal' class='bcLabel'>Excise Invoice Date :</span></td>");
                        sb.Append("<td class='rounded-corner1'><input id='txtExciseInvDT" + sno + "' class='bcAsptextbox DatePicker' " +
                            " type='text' value='" + ExcInvDate + "'  onchange='SaveChanges(" + sno + ")' readonly='readonly' /></td>");
                        sb.Append("</tr>");

                        sb.Append("</table>");
                        sb.Append("</div></div>");

                        # endregion

                        sb.Append("</td></tr>");
                    }
                }
                else
                    sb.Append("<td colspan='3' align='center'>No Records fetched...!</td>");

                sb.Append("<tfoot>");
                sb.Append("<tr>");
                sb.Append("<th class='rounded-foot-left'></th>");
                sb.Append("<th align='right'><span></span></th>");
                sb.Append("<th class='rounded-foot-right' align='right' width='80%'><span>G-Total : " + GTotal + "</span></th>");
                ViewState["GTotal"] = GTotal;
                sb.Append("</tr>");
                sb.Append("</tfoot>");
                sb.Append("</tbody></table>");
                if (GTotal >= Convert.ToDecimal(txtCTOneVal.Text))
                    chkIsUnutilised.Enabled = false;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return ex.Message + " Line No : " + LineNo;
            }            
        }

        # endregion

        # region Button Clicks

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int res = 1;
                Filename = FileName();
                int flag = 0;
                string ErrMessage = "";
                int RCount = 0;
                if (tblARE.Rows.Count > 0)
                {
                    DataTable dt = CommonBLL.EmptyDTCT1trackingDetails();
                    for (int i = 0; i < tblARE.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(tblARE.Rows[i]["Check"]))
                        {
                            if (tblARE.Rows[i]["ARE1ID"].ToString() == "" || tblARE.Rows[i]["GRNID"].ToString() == "" ||
                                tblARE.Rows[i]["PrfINVID"].ToString() == "" || tblARE.Rows[i]["PrfmInvcNo"].ToString() == "" || tblARE.Rows[i]["PrfmaInvcDt"].ToString() == "" ||
                                tblARE.Rows[i]["ShpngBillID"].ToString() == "" || tblARE.Rows[i]["ShpngBilNmbr"].ToString() == "" || tblARE.Rows[i]["ShpngBilDate"].ToString() == "" ||
                                tblARE.Rows[i]["MRcptID"].ToString() == "" || tblARE.Rows[i]["MReceiptNmbr"].ToString() == "" || tblARE.Rows[i]["MRDate"].ToString() == "" ||
                                tblARE.Rows[i]["BRCID"].ToString() == "" || tblARE.Rows[i]["ARE1Forms"].ToString() == "" || 
                                (tblARE.Rows[i]["BLID"].ToString() == "" && tblARE.Rows[i]["AWBID"].ToString() == "") ||
                                Convert.ToBoolean(tblARE.Rows[i]["SBCopy"].ToString()) == false || Convert.ToBoolean(tblARE.Rows[i]["ExchCopy"].ToString()) == false || Convert.ToBoolean(tblARE.Rows[i]["ExpCopy"].ToString()) == false ||
                                tblARE.Rows[i]["ExiseINVNo"].ToString() == "" || tblARE.Rows[i]["ExINVval"].ToString() == "" || tblARE.Rows[i]["ExINVDT"].ToString() == "" || 
                                tblARE.Rows[i]["SalesINVNo"].ToString() == "" || tblARE.Rows[i]["SalesINVDT"].ToString() == "" || tblARE.Rows[i]["SalesINVAmt"].ToString() == "")
                            {
                                flag = 1;
                                ErrMessage = "Please Fill all details in Are-1 forms at Row No. : " + (i + 1);
                            }

                            string[] Are1Forms = tblARE.Rows[i]["ARE1Forms"].ToString().Split(',');
                            //if (Are1Forms == null || Are1Forms.Length < 5)
                            //{
                            //    flag = 1;
                            //    ErrMessage = "Please Check all Are-1 forms in Row Number : " + (i + 1);
                            //}

                            //if ((Are1Forms != null && Are1Forms.Length > 0 && Are1Forms[0].ToString() != "" && !Convert.ToBoolean(Are1Forms[0].ToString()))
                            //    || (Are1Forms != null && Are1Forms.Length > 1 && Are1Forms[1].ToString() != "" && !Convert.ToBoolean(Are1Forms[1].ToString()))
                            //    || (Are1Forms != null && Are1Forms.Length > 2 && Are1Forms[2].ToString() != "" && !Convert.ToBoolean(Are1Forms[2].ToString()))
                            //    || (Are1Forms != null && Are1Forms.Length > 3 && Are1Forms[3].ToString() != "" && !Convert.ToBoolean(Are1Forms[3].ToString()))
                            //    || (Are1Forms != null && Are1Forms.Length > 4 && Are1Forms[4].ToString() != "" && !Convert.ToBoolean(Are1Forms[4].ToString())))

                            if ((Are1Forms != null && Are1Forms.Length > 0 && Are1Forms[0].ToString() != "" && !Convert.ToBoolean(Are1Forms[0].ToString()))
                                   || (Are1Forms != null && Are1Forms.Length > 1 && Are1Forms[1].ToString() != "" && !Convert.ToBoolean(Are1Forms[1].ToString()))
                                   && ((Are1Forms != null && Are1Forms.Length > 2 && Are1Forms[2].ToString() != "" && !Convert.ToBoolean(Are1Forms[2].ToString()))
                                 //|| (Are1Forms != null && Are1Forms.Length > 3 && Are1Forms[3].ToString() != "" && !Convert.ToBoolean(Are1Forms[3].ToString()))
                                   || (Are1Forms != null && Are1Forms.Length > 4 && Are1Forms[4].ToString() != "" && !Convert.ToBoolean(Are1Forms[4].ToString()))))
                            {
                                flag = 1;
                                ErrMessage = "Please Check all Are-1 forms in Row No. : " + (i + 1);
                            }

                            if (flag == 0)
                            {
                                DataRow dr = dt.NewRow();
                                //dr["ARE1ID"] = tblARE.Rows[i]["ARE1ID"];
                                //dr["GRNID"] = tblARE.Rows[i]["GRNID"];
                                //dr["PrfINVID"] = tblARE.Rows[i]["PrfINVID"];
                                //dr["ShpngBillID"] = tblARE.Rows[i]["ShpngBillID"];
                                //dr["BLID"] = tblARE.Rows[i]["BLID"];
                                //dr["MRcptID"] = tblARE.Rows[i]["MRcptID"];
                                //dr["BRCID"] = tblARE.Rows[i]["BRCID"];
                                //dr["ARE1Forms"] = tblARE.Rows[i]["ARE1Forms"];
                                //dr["SBCopy"] = tblARE.Rows[i]["SBCopy"];
                                //dr["ExiseINVNo"] = tblARE.Rows[i]["ExiseINVNo"];
                                //dr["ExINVval"] = tblARE.Rows[i]["ExINVval"];
                                //dr["SalesINVNo"] = tblARE.Rows[i]["SalesINVNo"];
                                //dr["SalesINVDT"] = Convert.ToDateTime(tblARE.Rows[i]["SalesINVDT"]);
                                //dr["SalesINVAmt"] = tblARE.Rows[i]["SalesINVAmt"];

                                dr["ARE1ID"] = tblARE.Rows[i]["ARE1ID"];
                                dr["GRNID"] = tblARE.Rows[i]["GRNID"];
                                dr["PrfINVID"] = tblARE.Rows[i]["PrfINVID"];
                                dr["PrfINVNo"] = tblARE.Rows[i]["PrfmInvcNo"];
                                dr["PrfINVDT"] = tblARE.Rows[i]["PrfmaInvcDt"];
                                dr["ShpngBillID"] = tblARE.Rows[i]["ShpngBillID"];
                                dr["ShpngBillNo"] = tblARE.Rows[i]["ShpngBilNmbr"];
                                dr["ShpngBillDT"] = tblARE.Rows[i]["ShpngBilDate"];
                                if (tblARE.Rows[i]["BLID"].ToString() != "")
                                {
                                    dr["BLID"] = tblARE.Rows[i]["BLID"];
                                }
                                else
                                {
                                    dr["AWBID"] = tblARE.Rows[i]["AWBID"];
                                }
                               // dr["BLID"] = tblARE.Rows[i]["BLID"];
                                dr["MRcptID"] = tblARE.Rows[i]["MRcptID"];
                                dr["MRcptNo"] = tblARE.Rows[i]["MReceiptNmbr"];
                                dr["MRcptDT"] = tblARE.Rows[i]["MRDate"];
                                dr["BRCID"] = tblARE.Rows[i]["BRCID"];
                                dr["ARE1Forms"] = tblARE.Rows[i]["ARE1Forms"];
                                dr["SBCopy"] = tblARE.Rows[i]["SBCopy"];
                                dr["ExchCopy"] = tblARE.Rows[i]["SBCopy"];
                                dr["ExpCopy"] = tblARE.Rows[i]["SBCopy"];
                                dr["ExiseINVNo"] = tblARE.Rows[i]["ExiseINVNo"];
                                dr["ExINVval"] = tblARE.Rows[i]["ExINVval"];
                                dr["ExINVDT"] = tblARE.Rows[i]["ExINVDT"];
                                dr["SalesINVNo"] = tblARE.Rows[i]["SalesINVNo"];
                                dr["SalesINVDT"] = Convert.ToDateTime(tblARE.Rows[i]["SalesINVDT"]);
                                dr["SalesINVAmt"] = tblARE.Rows[i]["SalesINVAmt"];
                                dt.Rows.Add(dr);
                                RCount += 1;
                            }
                        }
                    }

                    DateTime UnUsedDT = CommonBLL.EndDate;
                    decimal UnUsedAmt = 0;
                    if (Convert.ToBoolean(chkIsUnutilised.Checked))
                    {
                        UnUsedDT = CommonBLL.DateInsert(txtunUtilisedDt.Text);
                        UnUsedAmt = Convert.ToDecimal(txtunutilisedAmt.Text);
                    }
                    if (btnSave.Text == "Save" && dt.Rows.Count > 0 && flag == 0)
                    {
                        res = CT1TBLL.InsertUpdateDelete(CommonBLL.FlagNewInsert, Guid.Empty, new Guid(ddlCTOneNo.SelectedValue), Convert.ToDecimal(txtCTOneVal.Text),
                            HFFpos.Value, HFLpos.Value, new Guid(HFSupplierID.Value), new Guid(HFCustID.Value), UnUsedAmt, UnUsedDT,
                            new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), Guid.Empty,
                            CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, dt, "");
                        if (res == 0)
                        {

                            ALS.AuditLog(res, btnSave.Text, "", "CT-1 Tracking", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Saved Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Tracking",
                                "Inserted successfully.");
                            Response.Redirect("CTOneTrackingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "CT-1 Tracking", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Saving.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking",
                                "Error while Saving.");
                            DivAREOneDetails.InnerHtml = FillAREDetails(tblARE);
                        }
                    }
                    else if (btnSave.Text == "Update" && dt.Rows.Count > 0 && flag == 0)
                    {
                        res = CT1TBLL.InsertUpdateDelete(CommonBLL.FlagUpdate, new Guid(ViewState["EditID"].ToString()),
                            new Guid(ddlCTOneNo.SelectedValue),
                            Convert.ToDecimal(txtCTOneVal.Text), HFFpos.Value, HFLpos.Value, new Guid(HFSupplierID.Value),
                            new Guid(HFCustID.Value), UnUsedAmt, UnUsedDT,
                            new Guid(Session["CompanyID"].ToString()), new Guid(Session["UserID"].ToString()), CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy"))
                            , new Guid(Session["UserID"].ToString()),CommonBLL.DateInsert(DateTime.Now.ToString("dd-MM-yyyy")), true, dt, txtComments.Text.Trim());
                        if (res == 0)
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "CT-1 Tracking", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "SuccessMessage('Updated Successfully.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/Log"), "CT-1 Tracking",
                                "Updated successfully.");
                            Response.Redirect("CTOneTrackingStatus.aspx", false);
                        }
                        else
                        {
                            ALS.AuditLog(res, btnSave.Text, "", "CT-1 Tracking", new Guid(Session["UserId"].ToString()), new Guid(Session["CompanyId"].ToString()), Filename);
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll",
                                "ErrorMessage('Error while Updating.');", true);
                            ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking",
                                "Error while Updating.");
                            DivAREOneDetails.InnerHtml = FillAREDetails(tblARE);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ShowAll", "ErrorMessage('" + ErrMessage + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Response.Redirect("CTOneTracking.aspx", false);
        }
        # endregion

        # region Selected Index Changed

        protected void ddlCTOneNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCTOneNo.SelectedValue != "0")
                {
                    DataSet ds = CT1BLL.GetDataSet(CommonBLL.FlagESelect, new Guid(ddlCTOneNo.SelectedValue), new Guid(Session["CompanyID"].ToString()));
                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                    {
                        //txtCToneno.Text = ds.Tables[0].Rows[0]["CT1ReferenceNo"].ToString();
                        txtCTOneVal.Text = ds.Tables[0].Rows[0]["CT1BondValue"].ToString();
                        txtFpoNos.Text = ds.Tables[0].Rows[0]["FPONos"].ToString();
                        txtLpoNos.Text = ds.Tables[0].Rows[0]["LPONos"].ToString();
                        txtSupplierName.Text = ds.Tables[0].Rows[0]["SupplierNm"].ToString();
                        txtCustomerName.Text = ds.Tables[0].Rows[0]["CustmrNm"].ToString();
                        HFFpos.Value = ds.Tables[0].Rows[0]["FPOs"].ToString();
                        HFLpos.Value = ds.Tables[0].Rows[0]["LPOs"].ToString();
                        HFSupplierID.Value = ds.Tables[0].Rows[0]["SupplierID"].ToString();
                        HFCustID.Value = ds.Tables[0].Rows[0]["CustomerID"].ToString();
                        tblARE = new DataTable();
                        if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        {
                            tblARE = ds.Tables[1].Copy();
                            AddColumns();
                        }
                        DivAREOneDetails.InnerHtml = FillAREDetails(tblARE);
                        hdfunutil.Value = (Convert.ToDecimal(txtCTOneVal.Text) - Convert.ToDecimal(ViewState["GTotal"].ToString())).ToString();
                        hdfunutildate.Value = DateTime.Now.Date.ToString("dd-MM-yyyy");
                        chkIsUnutilised.Checked = false;
                        txtunUtilisedDt.Text = txtunutilisedAmt.Text = "";
                    }
                }
                else
                {
                    DivAREOneDetails.InnerHtml = "";
                    txtCustomerName.Text = "";
                    txtFpoNos.Text = "";
                    txtLpoNos.Text = "";
                    txtSupplierName.Text = "";
                    //txtCToneno.Text = "";
                    txtCTOneVal.Text = "";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
            }
        }
        
        # endregion

        # region WebMethods

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool SBCopy(string RNo, string SBCopyCheck)
        {
            try
            {
                bool ret = false;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    tblARE.Rows[RowNo]["SBCopy"] = Convert.ToBoolean(SBCopyCheck);
                    if (Convert.ToBoolean(SBCopyCheck))
                        ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return false;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool ChkExchCopy(string RNo, string SBCopyCheck)
        {
            try
            {
                bool ret = false;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    tblARE.Rows[RowNo]["ExchCopy"] = Convert.ToBoolean(SBCopyCheck);
                    if (Convert.ToBoolean(SBCopyCheck))
                        ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return false;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool ChkExpCopy(string RNo, string SBCopyCheck)
        {
            try
            {
                bool ret = false;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    tblARE.Rows[RowNo]["ExpCopy"] = Convert.ToBoolean(SBCopyCheck);
                    if (Convert.ToBoolean(SBCopyCheck))
                        ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return false;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool AreForms(string RNo, string org, string dup, string Trip, string green, string blue)
        {
            try
            {
                bool ret = false;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    tblARE.Rows[RowNo]["ARE1Forms"] = (org + "," + dup + "," + Trip + "," + green + "," + blue).ToString();
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return false;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public bool SaveChanges(string RNo, string ExiseINVNo, string ExINVval, string SalesINVNo, string SalesINVDT, string SalesINVAmt, string ExINVdt)
        {
            try
            {
                bool ret = false;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    tblARE.Rows[RowNo]["ExiseINVNo"] = ExiseINVNo;
                    if (ExINVval != "")
                        tblARE.Rows[RowNo]["ExINVval"] = Convert.ToDecimal(ExINVval);
                    tblARE.Rows[RowNo]["SalesINVNo"] = SalesINVNo;
                    if (SalesINVDT != "")
                        tblARE.Rows[RowNo]["SalesINVDT"] = CommonBLL.DateInsert(SalesINVDT);
                    if (SalesINVAmt != "")
                        tblARE.Rows[RowNo]["SalesINVAmt"] = Convert.ToDecimal(SalesINVAmt);
                    if (ExINVdt != "")
                        tblARE.Rows[RowNo]["ExINVDT"] = CommonBLL.DateInsert(ExINVdt);
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return false;
            }
        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string CheckFullRow(string RNo, string IsChecked)
        {
            try
            {
                string ret = "";
                bool Save = true;
                int RowNo = (Convert.ToInt32(RNo) - 1);
                if (tblARE != null && tblARE.Rows.Count >= RowNo)
                {
                    if (tblARE.Rows[RowNo]["ARE1No"].ToString() == "")
                    {
                        Save = false;
                        ret += "Are-1 No., ";
                    }
                    if (tblARE.Rows[RowNo]["ARE1Date"].ToString() == "")
                    {
                        Save = false;
                        ret += "Are-1 Date, ";
                    }
                    if (tblARE.Rows[RowNo]["ARE1Value"].ToString() == "")
                    {
                        Save = false;
                        ret += "Are-1 Value, ";
                    }

                    string[] Are1Forms = tblARE.Rows[RowNo]["ARE1Forms"].ToString().Split(',');
                    if ((Are1Forms != null && Are1Forms.Length > 0 && Are1Forms[0].ToString() != "" && !Convert.ToBoolean(Are1Forms[0].ToString()))
                           || (Are1Forms != null && Are1Forms.Length > 1 && Are1Forms[1].ToString() != "" && !Convert.ToBoolean(Are1Forms[1].ToString()))
                           && ((Are1Forms != null && Are1Forms.Length > 2 && Are1Forms[2].ToString() != "" && !Convert.ToBoolean(Are1Forms[2].ToString()))
                           //|| (Are1Forms != null && Are1Forms.Length > 3 && Are1Forms[3].ToString() != "" && !Convert.ToBoolean(Are1Forms[3].ToString()))
                           || (Are1Forms != null && Are1Forms.Length > 4 && Are1Forms[4].ToString() != "" && !Convert.ToBoolean(Are1Forms[4].ToString()))))
                    {
                        Save = false;
                        ret += "ARE-1 Forms";
                    }
                    if (tblARE.Rows[RowNo]["GRNNo"].ToString() == "")
                    {
                        Save = false;
                        ret += "GRN No., ";
                    }
                    if (tblARE.Rows[RowNo]["GRNDate"].ToString() == "")
                    {
                        Save = false;
                        ret += "GRN Date, ";
                    }
                    if (tblARE.Rows[RowNo]["PrfmInvcNo"].ToString() == "")
                    {
                        Save = false;
                        ret += "Proforma Invoice No., ";
                    }
                    if (tblARE.Rows[RowNo]["ShpngBilNmbr"].ToString() == "")
                    {
                        Save = false;
                        ret += "Shipping Bill No., ";
                    }

                    if (tblARE.Rows[RowNo]["BillofLadingNo"].ToString() == "" && tblARE.Rows[RowNo]["AWBNumber"].ToString() == "")
                    {
                        Save = false;
                        ret += "Bill of Lading No., ";
                    }
                    if (tblARE.Rows[RowNo]["MReceiptNmbr"].ToString() == "")
                    {
                        Save = false;
                        ret += "Mate Receipt No., ";
                    }
                    if (!Convert.ToBoolean(tblARE.Rows[RowNo]["SBCopy"].ToString()))
                    {
                        Save = false;
                        ret += "SB Copy, ";
                    }
                    if (tblARE.Rows[RowNo]["BRCNo"].ToString() == "")
                    {
                        Save = false;
                        ret += "BRC No., ";
                    }
                    if (tblARE.Rows[RowNo]["BRCDate"].ToString() == "")
                    {
                        Save = false;
                        ret += "BRC Date, ";
                    }
                    if (tblARE.Rows[RowNo]["BRCAmt"].ToString() == "")
                    {
                        Save = false;
                        ret += "BRC-Amount , ";
                    }
                    if (tblARE.Rows[RowNo]["SalesINVNo"].ToString() == "")
                    {
                        Save = false;
                        ret += "Sales Invoice No., ";
                    }
                    if (tblARE.Rows[RowNo]["SalesINVAmt"].ToString() == "")
                    {
                        Save = false;
                        ret += "Sales Invoice Amount, ";
                    }
                    if (tblARE.Rows[RowNo]["SalesINVDT"].ToString() == "")
                    {
                        Save = false;
                        ret += "Sales Invoice Date, ";
                    }
                    if (tblARE.Rows[RowNo]["ExiseINVNo"].ToString() == "")
                    {
                        Save = false;
                        ret += "Excise Invoice No., ";
                    }
                    if (tblARE.Rows[RowNo]["ExINVval"].ToString() == "")
                    {
                        Save = false;
                        ret += "Excise Invoice value";
                    }

                    if (Save == false)
                    {
                        ret += " are required in Row No : " + RNo + ".";
                        tblARE.Rows[RowNo]["Check"] = Convert.ToBoolean(IsChecked);
                    }
                    else
                    {
                        ret = "";
                        tblARE.Rows[RowNo]["Check"] = Convert.ToBoolean(IsChecked);
                    }
                }
                else
                    ret = "Error in Selection Please refresh the page.";
                return ret;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "CT-1 Tracking", ex.Message.ToString());
                return ErrMsg;
            }
        }

        # endregion

        protected void chkIsUnutilised_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}