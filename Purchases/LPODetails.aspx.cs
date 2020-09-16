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
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Drawing;

namespace VOMS_ERP.Purchases
{
    public partial class LPODetails : System.Web.UI.Page
    {
        # region variables
        Guid ID;
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        clsNum2WordBLL n2w = new clsNum2WordBLL();
        ErrorLog ELog = new ErrorLog();
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        ReportDocument rptDoc = new ReportDocument();
        FieldAccessBLL FAB = new FieldAccessBLL();
        string PriceSymbol = "";
        #endregion

        #region Default Page Load Evnet
        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                    Response.Redirect("../Login.aspx?logout=yes");
                else
                {
                    if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                    {
                        GetData();
                    }
                    else
                        Response.Redirect("../Masters/Home.aspx?NP=no");
                    //Response.Redirect("../Login.aspx?logout=no");
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.LclPurchaseOrderDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                LclPurchaseOrderDtls.Dispose();
                LclPurchaseOrderDtls = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        #endregion

        #region Get Data for Report

        /// <summary>
        /// Get Data for Report
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData()
        {
            try
            {
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != "")
                    ID = new Guid(Request.QueryString["ID"].ToString());
                else
                    ID = Guid.Empty;

                #region RPT report

                ReportDataSource LPODtlsDSet = new ReportDataSource();
                ReportDataSource LPOItmDSet = new ReportDataSource();
                ReportDataSource LPOPymntDSet = new ReportDataSource();
                ReportDataSource LPOTrmCdtnsDSet = new ReportDataSource();


                LPODtlsDSet.Name = "vomserpdbDataSet_SP_LPORPTItemDetails";
                LPOItmDSet.Name = "vomserpdbDataSet_SP_LPORPTAllItemDeatails";
                LPOPymntDSet.Name = "vomserpdbDataSet_SP_LPORPT_PaymentsTerms";
                LPOTrmCdtnsDSet.Name = "vomserpdbDataSet_SP_AllTermsConditions";


                DataSet dataset = CRPTBLL.GetLPODetails_Items(ID); ;
                DataSet ds = new DataSet();

                DataSet dsss = CRPTBLL.GetLPODetails_Items(new Guid(Session["CompanyID"].ToString())); ;

                //string val = "RDLC";
                //if (hfPath.Value != "")
                //    val = hfPath.Value;
                if (dsss.Tables[1].Rows[0]["CompanyID"].ToString() == Session["CompanyID"].ToString() && dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Glocem\\LpoCrp.rpt");
                }
                else
                {
                    rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\LpoCrp.rpt");
                }


                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];

                        if (SubRepName == "LQuotePriceBasicCrp.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetAllTermsConditons(CommonBLL.FlagZSelect, "LocalPurchaseOrderId", ID.ToString()).Tables[0]);
                        }
                        else if (SubRepName == "LpoBody.rpt")
                        {
                            ds = CRPTBLL.GetLPOAllDetails_Items(ID);
                            subRepDoc.Database.Tables[0].SetDataSource(ds.Tables[0]);
                        }
                        else if (SubRepName == "FQPayTerms.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(CRPTBLL.GetLPOPayments(CommonBLL.FlagZSelect, ID).Tables[0]);
                        }
                    }
                }

                if (Session["UserName"].ToString() == "System Admin")
                {
                    LclPurchaseOrderDtls.HasPrintButton = true;
                }

                DataSet HideFields = FAB.GetFieldDetails(CommonBLL.FlagESelect, Guid.Empty, new Guid(HttpContext.Current.Session["CompanyID"].ToString()), "NewLPOrder.Aspx");
                if (HideFields != null && HideFields.Tables.Count > 0)
                {
                    if (HideFields.Tables[0].AsEnumerable().Any(r => r.Field<string>("FieldDescription").Contains(CommonBLL.PriceTagText)))
                        HttpContext.Current.Session["HideFields"] = HideFields.Tables[0];
                }
                if (HttpContext.Current.Session["HideFields"] != null && ((DataTable)HttpContext.Current.Session["HideFields"]).Rows.Count > 0)
                {
                    PriceSymbol = (((DataTable)HttpContext.Current.Session["HideFields"]).AsEnumerable().Where(r => r.Field<string>("FieldDescription")
                        .Contains(CommonBLL.PriceTagText)).Select(s => s.Field<string>("PriceSymbol")).ToArray())[0].ToString();
                }

                byte[] imge = null;
                if (dataset != null && dataset.Tables[0] != null && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyLogo"].ToString() != "")
                {
                    imge = (byte[])(dataset.Tables[0].Rows[0]["CompanyLogo"]);
                    using (MemoryStream ms = new MemoryStream(imge))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                        string FilePath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png");//Server.MapPath("~/images/Logos/" + Session["CompanyID"].ToString() + ".png");
                        image.Save(FilePath);
                    }
                }
                string imgpath = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Logos\\" + Session["CompanyID"].ToString() + ".png"); //CommonBLL.CommonLogoUrl(HttpContext.Current);

                string AMT = "0.00";
                if (ds != null && ds.Tables.Count > 0)
                    AMT = Convert.ToDecimal(ds.Tables[0].Compute("sum(TotalAmt)", "")).ToString("N2");
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                Decimal DscntAmunt = Decimal.Zero; decimal NetTotal = 0, DiscRupeesCheck = 0;
                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                {
                    if (ds.Tables.Count > 1 && ds.Tables[3].Rows.Count > 0)
                        if (ds.Tables.Count > 1 && ds.Tables[3].Rows.Count > 0 && ConvertDecimal(ds.Tables[3].Rows[0]["DiscountRupees"]) != 0)
                        {
                            DiscRupeesCheck = ConvertDecimal(ds.Tables[3].Rows[0]["DiscountRupees"]);
                            //DiscRupeesCheck = DscntAmunt;
                        }
                        else if (ConvertDecimal(ds.Tables[2].Rows[0]["discountamt"]) != 0)
                            DscntAmunt = ConvertDecimal(ds.Tables[2].Rows[0]["discountamt"]);
                        else
                            DscntAmunt = ConvertDecimal(ds.Tables[2].Rows[0]["discountamt"]);
                }
                if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                {
                    //if (Convert.ToDecimal(ds.Tables[5].Rows[0]["DiscountPercentage"] ?? decimal.Zero) > 0)
                    //    DscntAmunt = Convert.ToDecimal(AMT) - (Convert.ToDecimal(AMT) - Convert.ToDecimal(AMT) * Convert.ToDecimal(ds.Tables[5].Rows[0]["DiscountPercentage"] ?? decimal.Zero) / 100); //= ConvertDecimal(ds.Tables[4].Rows[0]["DiscountAmount"]);
                    //else if (Convert.ToDecimal(ds.Tables[3].Rows[0]["DiscountRupees"] ?? decimal.Zero) > 0)
                    //    DscntAmunt = Convert.ToDecimal(AMT) - (Convert.ToDecimal(AMT) - Convert.ToDecimal(ds.Tables[3].Rows[0]["DiscountRupees"] ?? decimal.Zero));

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (DiscRupeesCheck != 0)
                            DscntAmunt += Convert.ToDecimal(ds.Tables[0].Compute("Sum(DscAmt)", "") ?? decimal.Zero);


                    }
                }
                Decimal SGST = Decimal.Zero; Decimal IGST = Decimal.Zero; Decimal CGST = Decimal.Zero;

                if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                {
                    SGST = ConvertDecimal(ds.Tables[5].Rows[0]["SGSTPercentage"]);
                    IGST = ConvertDecimal(ds.Tables[5].Rows[0]["IGSTPercentage"]);
                    CGST = ConvertDecimal(ds.Tables[5].Rows[0]["ExDutyPercentage"]);

                }
                if (DiscRupeesCheck != 0)
                    if (DscntAmunt != 0)
                        NetTotal = (Convert.ToDecimal(AMT)) - (Convert.ToDecimal(DiscRupeesCheck)) - (Convert.ToDecimal(DscntAmunt));
                    else
                        NetTotal = (Convert.ToDecimal(AMT)) - (Convert.ToDecimal(DiscRupeesCheck));
                else
                    NetTotal = Convert.ToDecimal(AMT) - Convert.ToDecimal(DscntAmunt);

                decimal SGSTAmount = 0; decimal IGSTAmount = 0; decimal CGSTAmount = 0; decimal GSTS = 0;
                SGSTAmount = NetTotal * SGST / 100;
                CGSTAmount = NetTotal * CGST / 100;
                IGSTAmount = NetTotal * IGST / 100;


                var RowCount = ds.Tables[0].AsEnumerable().Where(R => R.Field<decimal>("ExDUTYPercent") > 0).ToList();

                if (Convert.ToDecimal(SGSTAmount) != 0 || Convert.ToDecimal(CGSTAmount) != 0 || Convert.ToDecimal(IGSTAmount) != 0 || Convert.ToDecimal(DscntAmunt) != 0 || Convert.ToDecimal(DiscRupeesCheck) != 0)
                {
                    if (Convert.ToDecimal(SGSTAmount) != 0)
                    {
                        rptDoc.SetParameterValue("SGST", (SGSTAmount).ToString("N"));
                        NetTotal += SGSTAmount;
                        rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                    }
                    else
                    {
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                            rptDoc.SetParameterValue("SGST", "");

                    }
                    if (Convert.ToDecimal(CGSTAmount) != 0)
                    {
                        rptDoc.SetParameterValue("CGST", (CGSTAmount).ToString("N"));
                        NetTotal += CGSTAmount;
                        rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                    }
                    else
                    {
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                            rptDoc.SetParameterValue("CGST", "");

                    }
                    if (Convert.ToDecimal(IGSTAmount) != 0)
                    {
                        rptDoc.SetParameterValue("IGST", (IGSTAmount).ToString("N"));
                        NetTotal += IGSTAmount;
                        rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                    }
                    else
                    {
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                            rptDoc.SetParameterValue("IGST", "");
                    }
                    if (Convert.ToDecimal(DscntAmunt) != 0 && RowCount.Count == 0)
                    {
                        Decimal Tot_Dis_Amt = Decimal.Zero;
                        if (DiscRupeesCheck != 0)
                            //   NetTotal = NetTotal - DiscRupeesCheck;  //Commented by Dinesh on 10-06-2019 to calculate discount amount given in Rupees
                            Tot_Dis_Amt = Convert.ToDecimal(DscntAmunt) + Convert.ToDecimal(DiscRupeesCheck);
                        else
                            Tot_Dis_Amt = DscntAmunt;
                        rptDoc.SetParameterValue("discountamt", (Tot_Dis_Amt).ToString("N"));
                        rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                        rptDoc.SetParameterValue("discntamt", 0);
                        rptDoc.SetParameterValue("GST", 0);
                    }
                    else if (Convert.ToDecimal(DscntAmunt) != 0 && RowCount.Count > 0)
                    {
                        //Added by Dinesh on 10-06-2019 to calculate discount amount given in Rupees same as above condition
                        Decimal Tot_Dis_Amt = Decimal.Zero;
                        if (DiscRupeesCheck != 0)
                            Tot_Dis_Amt = Convert.ToDecimal(DscntAmunt) + Convert.ToDecimal(DiscRupeesCheck);
                        else
                            Tot_Dis_Amt = DscntAmunt;
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                        {
                            //rptDoc.SetParameterValue("discntamt", (DscntAmunt));
                            rptDoc.SetParameterValue("discntamt", (Tot_Dis_Amt));
                            rptDoc.SetParameterValue("GST", 0);
                            //rptDoc.SetParameterValue("NetTotalVal", "");
                        }
                        rptDoc.SetParameterValue("discountamt", "");
                        rptDoc.SetParameterValue("NetTotalVal", "");
                    }
                    else if (Convert.ToDecimal(DiscRupeesCheck) != 0 && RowCount.Count > 0)
                    {
                        Decimal Tot_Dis_Amt = Decimal.Zero;
                        if (DiscRupeesCheck != 0)
                            GSTS = ((NetTotal) * (Convert.ToDecimal(ds.Tables[0].Rows[0]["ExDUTYPercent"].ToString()) / 100));
                        if (DscntAmunt != 0)
                            Tot_Dis_Amt = Convert.ToDecimal(DscntAmunt) + Convert.ToDecimal(DiscRupeesCheck);
                        else
                            Tot_Dis_Amt = DiscRupeesCheck;
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                        {
                            rptDoc.SetParameterValue("discntamt", (Tot_Dis_Amt));
                            rptDoc.SetParameterValue("GST", (GSTS).ToString("N"));
                            //rptDoc.SetParameterValue("NetTotalVal", "");
                        }
                        rptDoc.SetParameterValue("discountamt", "");
                        rptDoc.SetParameterValue("NetTotalVal", "");
                    }
                    else if (Convert.ToDecimal(DiscRupeesCheck) != 0 && RowCount.Count == 0)
                    {
                        //if (DiscRupeesCheck != 0)
                        //    NetTotal = NetTotal - DiscRupeesCheck;
                        Decimal Tot_Dis_Amt = Decimal.Zero;
                        if (DscntAmunt != 0)
                            Tot_Dis_Amt = Convert.ToDecimal(DscntAmunt) + Convert.ToDecimal(DiscRupeesCheck);
                        else
                            Tot_Dis_Amt = DiscRupeesCheck;
                        rptDoc.SetParameterValue("discountamt", (Tot_Dis_Amt).ToString("N"));
                        rptDoc.SetParameterValue("NetTotalVal", NetTotal.ToString("N"));
                        rptDoc.SetParameterValue("discntamt", 0);
                        rptDoc.SetParameterValue("GST", 0);
                    }
                    else
                    {
                        rptDoc.SetParameterValue("discntamt", 0);
                        rptDoc.SetParameterValue("discountamt", "");
                        rptDoc.SetParameterValue("GST", 0);
                    }
                    //if (Convert.ToDecimal(DscntAmunt) != 0 && RowCount.Count > 0)
                    //{
                    //    rptDoc.SetParameterValue("discntamt", Math.Round(DscntAmunt).ToString("N"));
                    //}
                    //else
                    //{
                    //    rptDoc.SetParameterValue("discountamt", "");
                    //    //rptDoc.SetParameterValue("NetTotalVal", "");
                    //}

                }
                else
                {
                    if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                    {
                        rptDoc.SetParameterValue("IGST", "");
                        rptDoc.SetParameterValue("CGST", "");
                        rptDoc.SetParameterValue("SGST", "");
                        rptDoc.SetParameterValue("GST", 0);
                        rptDoc.SetParameterValue("discntamt", 0);
                    }
                    rptDoc.SetParameterValue("NetTotalVal", "");
                    rptDoc.SetParameterValue("discountamt", "");
                }
                // NetTotal = Math.Round(NetTotal); commented by dinesh on 12-6-2019 as amount is rounding and discount calc getting difference
                decimal DscAmt = 0; decimal ExDuty = 0; decimal NtAmount = 0; string AmtInWords = "";
                DscAmt = Convert.ToDecimal(ds.Tables[0].Compute("sum(DscAmt)", ""));// for line items
                ExDuty = Convert.ToDecimal(ds.Tables[0].Compute("sum(ExDUTY)", ""));// for line items

                var Calc_Data = (from mm in ds.Tables[0].AsEnumerable()
                                 select new
                                 {
                                     Tot = Convert.ToDecimal(mm.Field<string>("Quantity")) * Convert.ToDecimal(mm.Field<decimal>("ExDUTYPercent")),
                                     TotQty = Convert.ToDecimal(mm.Field<string>("Quantity"))

                                 }).ToList();

                decimal Qty_ExcDuty_Tot = Calc_Data.Sum(x => x.Tot);

                decimal Qty_Tot = Calc_Data.Sum(x => x.TotQty);

                decimal T_Calc = Qty_ExcDuty_Tot / Qty_Tot;
                decimal F_ExDuty = (NetTotal * T_Calc) / 100;

                if (DscAmt != 0 || ExDuty != 0)
                {
                    if (DiscRupeesCheck != 0)
                        if (DscAmt != 0)
                            NtAmount = (Convert.ToDecimal(AMT) - (DiscRupeesCheck + DscAmt)) + GSTS;
                        else
                            NtAmount = Convert.ToDecimal(AMT) - DiscRupeesCheck + GSTS;
                    else
                        NtAmount = Convert.ToDecimal(AMT) - DscAmt + ExDuty;
                    if (Convert.ToDecimal(SGSTAmount) != 0 || Convert.ToDecimal(CGSTAmount) != 0 || Convert.ToDecimal(IGSTAmount) != 0 || Convert.ToDecimal(DscntAmunt) != 0)
                    {
                        if (GSTS != 0)
                            NtAmount = NetTotal + GSTS;
                        else
                        {
                            NtAmount = NetTotal + F_ExDuty;
                            if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                                rptDoc.SetParameterValue("GST", (F_ExDuty).ToString("N"));
                        }
                    }
                    // string NetAmtInWords = ToWords.Num2WordConverter(NetTotal.ToString(), PriceSymbol).ToString();
                }
                if (NtAmount != 0)
                {
                    AmtInWords = ToWords.Num2WordConverter(NtAmount.ToString(), PriceSymbol).ToString();
                }
                else
                {
                    AmtInWords = ToWords.Num2WordConverter(NetTotal.ToString(), PriceSymbol).ToString();
                }
                rptDoc.SetParameterValue("TotalAmountInRs", AmtInWords);
                rptDoc.SetParameterValue("CompanyLogo", imgpath);
                string Terms1, Terms2, Terms3, Terms4, Terms5, Terms6, Terms7;
                try
                {
                    if (dataset.Tables[0].Rows[0]["CustomerId"].ToString().ToUpper() == "41BFE6DE-AE76-4F7A-8209-8725DC0154E6")
                    {
                        Terms1 = "Item/Product Name:";
                        Terms2 = "BatchNumber:";
                        Terms3 = "Model Number,Quantity,Capacity:";
                        Terms4 = "Manufacturer:";
                        Terms5 = "Year of Manufacture:";
                        Terms6 = "Country of Origin:";
                        Terms7 = "Certificate of Conformity for Machinery Parts:";
                        rptDoc.SetParameterValue("terms1", Terms1);
                        rptDoc.SetParameterValue("terms2", Terms2);
                        rptDoc.SetParameterValue("terms3", Terms3);
                        rptDoc.SetParameterValue("terms4", Terms4);
                        rptDoc.SetParameterValue("terms5", Terms5);
                        rptDoc.SetParameterValue("terms6", Terms6);
                        rptDoc.SetParameterValue("terms7", Terms7);
                    }
                    else
                    {
                        if (!dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                        {
                            rptDoc.SetParameterValue("terms1", "");
                            rptDoc.SetParameterValue("terms2", "");
                            rptDoc.SetParameterValue("terms3", "");
                            rptDoc.SetParameterValue("terms4", "");
                            rptDoc.SetParameterValue("terms5", "");
                            rptDoc.SetParameterValue("terms6", "");
                            rptDoc.SetParameterValue("terms7", "");
                        }
                    }
                }
                catch { }
                LclPurchaseOrderDtls.ReportSource = rptDoc;

                #endregion
                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    lbtnCntnu.Enabled = false;
                    lbtnCntnu.Visible = false;
                }
                else
                {
                    string Mode = (HttpContext.Current.Request.UrlReferrer) == null ? "" : HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query)["Mode"];
                    if (Mode == "tdt")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=tdt";
                    }
                    else if (Mode == "tldt")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=tldt";
                    }
                    else if (Mode == "DPtdt")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=DPtdt";
                    }
                    else if (Mode == "dtdtd")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=dtdtd";
                    }
                    else if (Mode == "Ict")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=Ict";
                    }
                    else if (Mode == "mtd")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=mtd";
                    }
                    else if (Mode == "Etdt")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=Etdt";
                    }
                    else if (Mode == "cpd")
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx?Mode=cpd";
                    }
                    else
                    {
                        lbtnBack.PostBackUrl = "~/Purchases/LPOStatus.aspx";
                    }
                    if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsVerbalFPO"].ToString()) == true && Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == false)
                    {
                        lbtnCntnu.Visible = false;
                        lbtnCntnu.Visible = false;
                    }
                    if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == true)
                    {
                        lbtnCntnu.Text = "No Further proceedings for Cancelled LPO";
                        lbtnCntnu.ForeColor = Color.Red;
                        lbtnCntnu.Enabled = false;
                        lbtnCntnu.Visible = true;
                    }

                    if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsLPOCancelled"].ToString()) == false)
                        {
                            if ((CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Logistics/RqstCEDtls.aspx")
                                || CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), "/Logistics/DespatchInstructions.aspx")))
                            {

                                lbtnCntnu.PostBackUrl = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                                   "~/Logistics/RqstCEDtls.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                                   "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString() :
                                   "~/Logistics/DespatchInstructions.aspx?CstmrID=" + dataset.Tables[0].Rows[0]["CustomerId"].ToString() +
                                   "&SuplrID=" + dataset.Tables[0].Rows[0]["SupplierId"].ToString() +
                                   "&FpoId=" + dataset.Tables[0].Rows[0]["ForeignPurchaseOrderId"].ToString() +
                                   "&LpoId=" + dataset.Tables[0].Rows[0]["LocalPurchaseOrderId"].ToString());
                                lbtnCntnu.Text = (Convert.ToBoolean(dataset.Tables[0].Rows[0]["IsCentralExcise"].ToString()) == true ?
                                    "Continue with REQUEST CE DETAILS" : "Continue with DISPATCH INSTRUCTIONS");
                            }
                            else
                                Response.Redirect("../Masters/Home.aspx?NP=no");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
            }
        }

        /// <summary>
        /// Bind Default/Static Data to Data Table
        /// </summary>
        /// <returns></returns>
        protected DataTable StaticData()
        {
            try
            {
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                DataRow dr1 = dt.NewRow();
                dt.Columns.Add("Title");
                dt.Columns.Add("Description");

                dr["Title"] = "Submission of Documents";
                dr["Description"] = "A set of following documents should be submitted to our Hyderabad office for arranging payment."
                                        + System.Environment.NewLine +
                                    "1) Commercial Invoice." + System.Environment.NewLine +
                                    "2) Copy of L.R." + System.Environment.NewLine +
                                    "3) Packing list including no. of packages, description " +
                                        "and quantity of goods in each pack, weight and size of boxes/packages";
                dt.Rows.Add(dr);

                dr1["Title"] = "Shipping";
                dr1["Description"] = "Following Shipping Marks should be marked on the packing box";

                dt.Rows.Add(dr1);
                return dt;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Purchases/ErrorLog"), "Local Purchase Order Details", ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region Methods

        private void CloseReports(ReportDocument reportDocument)
        {
            Sections sections = reportDocument.ReportDefinition.Sections;
            foreach (Section section in sections)
            {
                ReportObjects reportObjects = section.ReportObjects;
                foreach (ReportObject reportObject in reportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                        subReportDocument.Close();
                    }
                }
            }
            reportDocument.Close();
        }

        #endregion

        public Decimal ConvertDecimal(Object Value)
        {
            try
            {
                return Convert.ToDecimal(Value);
            }
            catch
            {
                return decimal.Zero;
            }

        }
    }
}
