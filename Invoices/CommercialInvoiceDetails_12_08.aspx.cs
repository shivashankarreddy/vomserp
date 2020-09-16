using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BAL;
using Microsoft.Reporting.WebForms;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using CrystalDecisions.Shared;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using tr = DocumentFormat.OpenXml.Wordprocessing;


namespace VOMS_ERP.Invoices
{
    public partial class CommercialInvoiceDetails : System.Web.UI.Page
    {
        # region Variables
        long ID;
        ErrorLog ELog = new ErrorLog();
        InvoiceBLL INBLL = new InvoiceBLL();
        CommonRPTBLL CRPTBLL = new CommonRPTBLL();
        ReportDocument rptDoc = new ReportDocument();
        # endregion

        #region Default Page Load Event

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_init(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    if (Request.QueryString["PINID"] != null)
                    {
                        Session["DtComsInv"] = "";
                        Session["DtComsInvset"] = "";
                        Session["TotalAmt"] = "";
                    }
                    GetData(new Guid(Request.QueryString["PINID"].ToString()));
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        /// <summary>
        /// Page On-Load Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            //this.CommercialInvoiceDetails = null;
            GC.Collect();
        }


        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(rptDoc);
                rptDoc.Dispose();
                CommercialInvoiceDtls.Dispose();
                CommercialInvoiceDtls = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }


        #endregion

        # region Methods

        /// <summary>
        /// Bind Data Tables to Report Veiwer
        /// </summary>
        /// <param name="ID"></param>
        protected void GetData(Guid ID)
        {
            try
            {


                #region RPT report

                ReportDataSource SPIDtlsDSet = new ReportDataSource();
                ReportDataSource SPIItmDSet = new ReportDataSource();

                SPIDtlsDSet.Name = "vomserpdbDataSet_SP_CommInvcDtls";
                SPIItmDSet.Name = "vomserpdbDataSet_SP_ShipmentPrfINV_RDLC";

                DataSet ds = INBLL.PrfmaInvcRDLC(ID);
                Session["DtComsInv"] = ds;
                DataSet dataset = CRPTBLL.GetCommInvcDtls(ID);
                Session["DtComsInvset"] = dataset;
                DataTable dt = new DataTable();
                //DataSet ds = CRPTBLL.GetShipmnetPrfmaInvcDtls(new Guid(Session["CompanyID"].ToString()));
                //Session["DtComsInvDataset"] = ds;

                //rptDoc.FileName = Server.MapPath("\\RDLC\\CommertialINVCrp.rpt");
                rptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\CommertialINVCrp.rpt");
                rptDoc.Load(rptDoc.FileName);
                rptDoc.Database.Tables[0].SetDataSource(dataset.Tables[0]);
                foreach (ReportObject repOp in rptDoc.ReportDefinition.ReportObjects)
                {
                    if (repOp.Kind == ReportObjectKind.SubreportObject)
                    {
                        string SubRepName = ((SubreportObject)repOp).SubreportName;
                        ReportDocument subRepDoc = rptDoc.Subreports[SubRepName];
                        if (SubRepName == "PInvBody.rpt")
                        {
                            subRepDoc.Database.Tables[0].SetDataSource(INBLL.PrfmaInvcRDLC(ID).Tables[0]);
                            dt = INBLL.PrfmaInvcRDLC(ID).Tables[0];
                        }
                    }
                }

                string AMT = "0.00";
                if (dt != null && dt.Rows.Count > 0)
                    AMT = Convert.ToDecimal(dt.Compute("sum(Amount)", "")).ToString("N2");
                clsNum2WordBLL ToWords = new clsNum2WordBLL();
                string AmtInWords = ToWords.Num2WordConverter(AMT.ToString(), "").ToString();
                AmtInWords = AmtInWords.Replace("(", "($");
                rptDoc.SetParameterValue("TotalAmountInRs", AmtInWords);
                Session["TotalAmt"] = AmtInWords;

                if (Session["UserName"].ToString() == "System Admin")
                {
                    CommercialInvoiceDtls.HasPrintButton = true;
                }

                CommercialInvoiceDtls.ReportSource = rptDoc;

                #endregion
                //if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CheckList_In_PkngLst"].ToString() == "")
                //    lbBtnContinue.PostBackUrl = "~/Invoices/PackingList.aspx?ChkLstID=" + dataset.Tables[0].Rows[0]["CheckLIstID"].ToString();
                //else
                //{
                //    lbBtnContinue.Text = "PackingList is Prepared for this ProformaInvoice.";
                //    lbBtnContinue.ForeColor = Color.Red;
                //    lbBtnContinue.Enabled = false;
                //}
                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    //lbtnCntnu.Enabled = false;
                    //lbtnCntnu.Visible = false;
                }
                else
                    lbtnBack.PostBackUrl = "~/Invoices/CommercialInvoiceStatus.aspx";
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }

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

        protected void btnWordExpt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

                ExportWord();

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());

            }



        }

        public void ExportWord()
        {
            try
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    string filepath = @"CommercialInvoiceExport" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.TimeOfDay.ToString() + ".doc";
                    // Create Document
                    using (WordprocessingDocument wordDocument =
                        WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
                    {
                        // Create the document structure and add some text.

                        Body docBody = new Body();
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = new Body();
                        //var doc = mainPart.Document;
                        //Adding Header and Footer Parts.
                        HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>("r95");
                        FooterPart footerPart = mainPart.AddNewPart<FooterPart>("r96");

                        // Get Id of the headerPart and footer parts
                        string headerPartId = mainPart.GetIdOfPart(headerPart);
                        string footerPartId = mainPart.GetIdOfPart(footerPart);
                        SectionProperties SecPro = new SectionProperties();
                        PageSize PSize = new PageSize();
                        //PSize.Width = 15000;
                        PSize.Height = 16500;
                        SecPro.Append(PSize);
                        body.Append(SecPro);

                        GenerateBodyPart(mainPart, body);

                        GenerateHeaderPart(mainPart, headerPart);

                        GenerateFooterPart(mainPart, footerPart);

                        mainPart.Document.Save();
                    }

                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + filepath + "");
                    mem.Position = 0;
                    mem.CopyTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();

                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Others/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }

        private void GenerateHeaderPart(MainDocumentPart mainPart, HeaderPart headerPart)
        {
            try
            {
                #region Header
                // add/modify header values
                Header h = new Header();
                tr.Table table1 = new tr.Table();
                // Add a Paragraph and a Run with the specified Text 
                DataSet dtt1 = (DataSet)Session["DtComsInv"];
                DataSet dtt = (DataSet)Session["DtComsInvset"];

                SetTableStyle(table1, "6000", true, true, true, true, true, true);
                tr.TableRow tr1s = new tr.TableRow();
                TableCellProperties tableCellPropertiess = new TableCellProperties();
                HorizontalMerge verticalMerges = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                tableCellPropertiess.Append(verticalMerges);
                TableCellProperties tableCellProperties1s = new TableCellProperties();
                HorizontalMerge verticalMerge1s = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                tableCellProperties1s.Append(verticalMerge1s);
                tr.TableCell tc11s = new tr.TableCell();
                Paragraph p11s = new Paragraph();
                Run r12s = new Run();
                RunProperties rp12s = new RunProperties();
                rp12s.Bold = new Bold();
                rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont = new RunFonts();   //m1 
                runFont.Ascii = "Arial";//m1
                r12s.Append(runFont);//m1
                ParagraphProperties pp11s = new ParagraphProperties();
                pp11s.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties11s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                p11s.Append(paragraphProperties11s);
                r12s.Append(rp12s);
                r12s.Append(new Text(dtt.Tables[0].Rows[0]["INVOICE"].ToString()));
                p11s.Append(r12s);
                p11s.Append(pp11s);
                tc11s.Append(p11s);
                tr1s.Append(tc11s);
                TableRowProperties tableRowProperties1s = new TableRowProperties();
                TableRowHeight tableRowHeight1s = new TableRowHeight() { Val = (UInt32Value)28U };

                tableRowProperties1s.Append(tableRowHeight1s);
                tr1s.InsertBefore(tableRowProperties1s, tc11s);
                tr.TableCell tc12s = new tr.TableCell();
                Paragraph p12s = new Paragraph();

                tc11s.Append(tableCellPropertiess);
                tc12s.Append(tableCellProperties1s);
                tc12s.Append(p12s);
                tr1s.Append(tc12s);
                table1.Append(tr1s);

                tr.TableRow tr2s = new tr.TableRow();
                tr.TableCell tc21s = new tr.TableCell();
                Run run = new tr.Run();
                Run rn = new tr.Run();
                RunFonts runFont_2 = new RunFonts();           // Create font
                runFont_2.Ascii = "Arial";
                run.Append(runFont_2);
                run.RunProperties = new tr.RunProperties((new Bold()));
                run.RunProperties.FontSize = new tr.FontSize() { Val = "18" };
                run.AppendChild(new Text("Exporter: "));
                run.AppendChild(new Break());
                run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyName"].ToString()));
                run.AppendChild(new Break());
                run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyAdd"].ToString()));
                //run.AppendChild(new Break());
                //run.AppendChild(new Text("KONDAPUR, HYDERABAD, TELANGANA, INDIA-500081"));
                run.AppendChild(new Break());
                rn.RunProperties = new tr.RunProperties((new Bold()));
                rn.RunProperties.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_3 = new RunFonts();           // Create font
                runFont_3.Ascii = "Arial";
                rn.Append(runFont_3);
                //if (dtt != null)
                //    if (dtt.Tables.Count > 1 && dtt.Tables[1].Rows.Count > 0 && dtt.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                //{
                rn.AppendChild(new Text("IEC Code No. 0996008306"));
                rn.AppendChild(new Break());
                //}
                rn.AppendChild(new Text("GSTIN No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]));
                rn.AppendChild(new Break());
                rn.AppendChild(new Text("TIN  No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[1]));
                Paragraph p21s = new Paragraph(run);
                p21s.Append(rn);
                ParagraphProperties paragraphProperties1s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p21s.Append(paragraphProperties1s);
                tc21s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3900" }));
                TableCellProperties cellPropertiess = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                tc21s.AppendChild(cellPropertiess);
                tc21s.Append(p21s);
                tr2s.Append(tc21s);

                tr.TableCell tc22s = new tr.TableCell();
                Paragraph p22s = new Paragraph();
                ParagraphProperties pp22s = new ParagraphProperties();
                pp22s.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties21s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                p22s.Append(paragraphProperties21s);
                p22s.Append(pp22s);

                tr.Table tbl2 = new tr.Table();
                SetTableStyle(tbl2, "5000", false, true, true, true, true, true);
                tr.TableRow row2 = new tr.TableRow();
                tr.Paragraph pd = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd.Append(paragraphProperties21ss);
                RunProperties r_ = new RunProperties();
                Run rn1t = new tr.Run();
                rn1t.Append(r_);
                r_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8s12 = new RunFonts();           // Create font
                runFont8s12.Ascii = "Arial";
                r_.Append(runFont8s12);
                // rn1t.Append
                // rn1t.Append(new Run(new Text(dtt.Tables[0].Rows[0]["PrfrmInvNoDT"].ToString())));
                pd.Append(new Run(new Text("Invoice No: " + dtt.Tables[0].Rows[0]["CommInvNoDT"].ToString())));
                pd.Append(rn1t);
                tr.TableCell col2 = new tr.TableCell();
                col2.Append(pd);
                row2.Append(col2);
                tbl2.Append(row2);

                tr.TableRow row2s = new tr.TableRow();

                tr.Table tbl22 = new tr.Table();
                SetTableStyle(tbl22, "5000", false, false, true, false, true, true);
                tr.TableRow row22 = new tr.TableRow();
                tr.Paragraph pd2 = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss2 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd2.Append(paragraphProperties21ss2);
                RunProperties r1_ = new RunProperties();
                Run rn2t = new tr.Run();
                rn2t.Append(r1_);
                r1_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8s121 = new RunFonts();           // Create font
                runFont8s121.Ascii = "Arial";
                r1_.Append(runFont8s121);
                pd2.Append(new Run(new Text("GSTIN No:" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0])));
                pd2.Append(rn2t);
                tr.TableCell col22 = new tr.TableCell();
                col22.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                col22.Append(pd2);
                row22.Append(col22);


                tr.Paragraph pd22 = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss22 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd22.Append(paragraphProperties21ss22);
                RunProperties r3_ = new RunProperties();
                Run rn3t = new tr.Run();
                rn3t.Append(r3_);
                r3_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8se = new RunFonts();           // Create font
                runFont8se.Ascii = "Arial";
                r3_.Append(runFont8se);
                pd22.Append(new Run(new Text("STATE :" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[2])));
                pd22.Append(rn3t);
                tr.TableCell col222 = new tr.TableCell();
                col222.Append(pd22);
                row22.Append(col222);
                tbl22.Append(row22);
                tr.TableCell col2s = new tr.TableCell();
                col2s.Append(new Paragraph(new Run(tbl22)));
                row2s.Append(col2s);
                tbl2.Append(row2s);
                //manju

                #region

                tr.TableRow row34s = new tr.TableRow();

                tr.Table tbl334 = new tr.Table();
                SetTableStyle(tbl334, "5000", false, false, true, false, true, true);
                tr.TableRow row334 = new tr.TableRow();
                tr.Paragraph pd34 = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss34 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd34.Append(paragraphProperties21ss34);
                RunProperties r4_ = new RunProperties();
                Run rn4t = new tr.Run();
                rn4t.Append(r4_);
                r4_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8sf = new RunFonts();           // Create font
                runFont8sf.Ascii = "Arial";
                r4_.Append(runFont8sf);
                pd34.Append(new Run(new Text("Other References: " + dtt.Tables[0].Rows[0]["PrfrmInvNoDT"].ToString())));
                pd34.Append(rn4t);
                tr.TableCell col334 = new tr.TableCell();
                col334.Append(pd34);
                col334.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2900" }));
                row334.Append(col334);
                tbl334.Append(row334);
                tr.TableCell col334s = new tr.TableCell();
                col334s.Append(new Paragraph(new Run(tbl334)));
                row34s.Append(col334s);
                tbl2.Append(row34s);

                tr.TableRow row41 = new tr.TableRow();
                tr.Paragraph pd41 = new tr.Paragraph();
                ParagraphProperties paragraphProperties41ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd41.Append(paragraphProperties41ss);
                //pd4.Append(new Run(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPOs"].ToString())));
                RunProperties rp234ssx1 = new RunProperties();
                Run rn5x1 = new tr.Run();
                rn5x1.Append(rp234ssx1);

                #endregion


                tr.TableRow row3s = new tr.TableRow();

                tr.Table tbl33 = new tr.Table();
                SetTableStyle(tbl33, "5000", false, false, true, false, true, true);
                tr.TableRow row33 = new tr.TableRow();
                tr.Paragraph pd3 = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss3 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd3.Append(paragraphProperties21ss3);
                RunProperties r5_ = new RunProperties();
                Run rn5t = new tr.Run();
                rn5t.Append(r5_);
                r5_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont5se = new RunFonts();           // Create font
                runFont5se.Ascii = "Arial";
                r5_.Append(runFont5se);
                pd3.Append(new Run(new Text("Reverse ChargeApplicable:No")));
                pd3.Append(rn5t);
                tr.TableCell col33 = new tr.TableCell();
                col33.Append(pd3);
                col33.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2900" }));
                row33.Append(col33);

                tr.Paragraph pd33 = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss33 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd33.Append(paragraphProperties21ss33);
                RunProperties r51_ = new RunProperties();
                Run rn5t1 = new tr.Run();
                rn5t1.Append(r51_);
                r51_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont5se1 = new RunFonts();           // Create font
                runFont5se1.Ascii = "Arial";
                r51_.Append(runFont5se1);
                if (dtt.Tables[0].Rows[0]["EUCIT"].ToString() != "")
                    pd33.Append(new Run(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[0])));
                else
                    pd33.Append(new Run(new Text("END USE CODE")));
                pd33.Append(rn5t1);
                tr.TableCell col333 = new tr.TableCell();
                col333.Append(pd33);
                row33.Append(col333);
                tbl33.Append(row33);
                tr.TableCell col3s = new tr.TableCell();
                col3s.Append(new Paragraph(new Run(tbl33)));
                row3s.Append(col3s);
                tbl2.Append(row3s);

                tr.TableRow row4 = new tr.TableRow();
                tr.Paragraph pd4 = new tr.Paragraph();
                ParagraphProperties paragraphProperties4ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd4.Append(paragraphProperties4ss);
                //pd4.Append(new Run(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPOs"].ToString())));
                RunProperties rp234ssx = new RunProperties();
                Run rn5x = new tr.Run();
                rn5x.Append(rp234ssx);
                if (dtt.Tables[0].Rows[0]["FPOs"].ToString().Length > 70)
                {
                    rp234ssx.FontSize = new tr.FontSize() { Val = "16" };
                    string txtsplit = string.Empty;
                    RunFonts runFont_4 = new RunFonts();           // Create font
                    runFont_4.Ascii = "Arial";
                    rp234ssx.Append(runFont_4);
                    string[] a = SplitByLenght("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPOs"].ToString(), 70);
                    for (int i = 0; i < a.Length; i++)
                    {
                        rn5x.AppendChild(new Text(a[i]));
                        if (i != (a.Length - 1))
                            rn5x.AppendChild(new Break());
                    }
                }
                else
                {
                    rp234ssx.FontSize = new tr.FontSize() { Val = "16" };
                    RunFonts runFont_4 = new RunFonts();           // Create font
                    runFont_4.Ascii = "Arial";
                    rp234ssx.Append(runFont_4);
                    rn5x.Append(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPOs"].ToString()));
                }

                pd4.Append(rn5x);
                tr.TableCell col4 = new tr.TableCell();
                col4.Append(pd4);
                row4.Append(col4);
                tbl2.Append(row4);

                tr.TableRow row5 = new tr.TableRow();
                tr.Paragraph pd5 = new tr.Paragraph();
                ParagraphProperties paragraphProperties5ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd5.Append(paragraphProperties5ss);

                Run run5 = new tr.Run();
                Run rn5 = new tr.Run();
                //run5.AppendChild(new Text("To: "));
                //run5.Append(new Break(), new Bold());

                run5.RunProperties = new tr.RunProperties((new Bold()));

                RunProperties rp234ss = new RunProperties();
                rp234ss.FontSize = new tr.FontSize() { Val = "16" };
                RunFonts runFont_5 = new RunFonts();           // Create font
                runFont_5.Ascii = "Arial";
                rp234ss.Append(runFont_5);
                rn5.Append(rp234ss);
                rn5.AppendChild(new Text("To: " + dtt.Tables[0].Rows[0]["CustomerName"].ToString()));
                rn5.AppendChild(new Break());
                rn5.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                rn5.AppendChild(new Break());
                rn5.AppendChild(new Text("PH:" + dtt.Tables[0].Rows[0]["Phone"].ToString()));
                pd5.Append(run5);
                pd5.Append(rn5);
                tr.TableCell col5 = new tr.TableCell();
                col5.Append(pd5);
                row5.Append(col5);
                tbl2.Append(row5);


                tc22s.Append(new Paragraph(new Run(tbl2)));
                tr2s.Append(tc22s);
                table1.Append(tr2s);

                tr.TableRow tr3s = new tr.TableRow();
                tr.TableCell tc31s = new tr.TableCell();
                Run run3 = new tr.Run();
                RunProperties rp234s = new RunProperties();
                rp234s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont1 = new RunFonts();   //m1 
                runFont1.Ascii = "Arial";//m1
                run3.Append(runFont1);//m1
                run3.Append(rp234s);
                run3.AppendChild(new Text("Consignee : "));
                run3.AppendChild(new Break());
                run3.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustomerName"].ToString()));
                run3.AppendChild(new Break());
                run3.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                run3.AppendChild(new Break());
                run3.AppendChild(new Text("PH:" + dtt.Tables[0].Rows[0]["Phone"].ToString() + " Fax: " + dtt.Tables[0].Rows[0]["Fax1"].ToString()));


                Paragraph p31s = new Paragraph(run3);
                ParagraphProperties paragraphProperties3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                p31s.Append(paragraphProperties3s);
                tc31s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5000" }));
                TableCellProperties cellPropertiess3 = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                tc31s.AppendChild(cellPropertiess3);
                tc31s.Append(p31s);
                tr3s.Append(tc31s);
                tr.TableCell tc32su = new tr.TableCell();

                tr.Table tbl32su = new tr.Table();
                SetTableStyle(tbl32su, "5000", false, true, false, true, true, true);
                tr.TableRow row32su = new tr.TableRow();
                tr.TableRow row323su = new tr.TableRow();
                tr.Paragraph pd232su = new tr.Paragraph();
                TableRowProperties trp1 = new TableRowProperties(new TableRowHeight() { Val = (UInt32Value)18U });
                TableRowProperties trp2 = new TableRowProperties(new TableRowHeight() { Val = (UInt32Value)18U });
                ParagraphProperties paragraphProperties32su = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd232su.Append(paragraphProperties32su);
                Run rrrr = new tr.Run();
                RunProperties rp22s = new RunProperties();
                rp22s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont2 = new RunFonts();   //m1 
                runFont2.Ascii = "Arial";//m1
                rrrr.Append(runFont2);//m1
                rrrr.Append(rp22s);
                rrrr.AppendChild(new Text("Country of Origin of Goods: " + dtt.Tables[0].Rows[0]["PlcOrgnGds"].ToString()));
                rrrr.AppendChild(new Break());
                pd232su.Append(rrrr);
                tr.TableCell col322su = new tr.TableCell();
                //col322su.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3100" }));
                col322su.Append(pd232su);
                row32su.Append(trp1);
                row32su.Append(col322su);
                tbl32su.Append(row32su);
                tr.Paragraph pd322su = new tr.Paragraph();
                ParagraphProperties paragraphProperties322su = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd322su.Append(paragraphProperties322su);
                Run rrrr2 = new tr.Run();
                RunProperties rp2s = new RunProperties();
                rp2s.FontSize = new tr.FontSize() { Val = "18" };

                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont4 = new RunFonts();   //m1 
                runFont4.Ascii = "Arial";//m1
                rrrr2.Append(runFont4);//m1
                rrrr2.Append(rp2s);
                rrrr2.AppendChild(new Text("Country of Final Destination: " + dtt.Tables[0].Rows[0]["PlcFnlDstn"].ToString()));
                rrrr2.AppendChild(new Break());
                pd322su.Append(rrrr2);
                tr.TableCell col322 = new tr.TableCell();
                col322.Append(pd322su);
                row323su.Append(trp2);
                row323su.Append(col322);
                tbl32su.Append(row323su);
                #region Merged unused

                tr.TableRow tr323s = new tr.TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32("5") }));
                //TableRow row = new TableRow(new TableRowProperties(new TableRowHeight() { Val = Convert.ToUInt32("20") }));
                TableCellProperties tblcelprop11 = new TableCellProperties();
                HorizontalMerge verticalMerge = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                tblcelprop11.Append(verticalMerge);


                TableCellProperties tblcelprop12 = new TableCellProperties();
                HorizontalMerge verticalMerge1 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                tblcelprop12.Append(verticalMerge1);

                tr.Paragraph pd332su = new tr.Paragraph();
                ParagraphProperties paragraphProperties332su = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "20" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd332su.Append(paragraphProperties332su);
                Run run323s = new tr.Run();
                run323s.AppendChild(new Text("Terms of Delivery and Payment:   "));
                //run323s.AppendChild(new Break());
                //run323s.AppendChild(new Text("Routing of Payment"));
                pd332su.Append(run323s);

                tr.TableCell col332su = new tr.TableCell();
                tr.TableCell col3321su = new tr.TableCell();
                col332su.Append(pd332su);
                //col332su.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "6100" }));
                Run run3232s = new tr.Run();
                run3232s.AppendChild(new Text("Routing of Payment"));
                col3321su.Append(new tr.Paragraph(run3232s));
                //col332su.Append(tblcelprop11);
                //col3321su.Append(tblcelprop12);

                //tr323s.Append(col332su);
                //tr323s.Append(col3321su);
                //tbl32su.Append(tr323s);
                #endregion
                tr.TableCell col333su = new tr.TableCell();
                col333su.Append(new Paragraph(new Run(tbl32su)));
                tr3s.Append(col333su);
                table1.Append(tr3s);

                tr.TableRow tr4s = new tr.TableRow();
                tr.TableCell tc41s = new tr.TableCell();
                tr.Table tbl52su = new tr.Table();
                SetTableStyle(tbl52su, "5000", false, true, false, true, true, true);
                tr.TableRow tr422s = new tr.TableRow();
                tr.TableCell tc4221s = new tr.TableCell();
                Run run4s = new tr.Run();
                RunProperties rp43s = new RunProperties();
                rp43s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont5 = new RunFonts();   //m1 
                runFont5.Ascii = "Arial";//m1
                run4s.Append(runFont5);//m1
                run4s.Append(rp43s);
                //run4s.AppendChild(new Text("Notify: " + dtt.Tables[0].Rows[0]["Notify"].ToString()));
                //run4s.AppendChild(new Break());
                //run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["NotifyShipAdd"].ToString()));
                //run4s.AppendChild(new Break());
                //run4s.AppendChild(new Text("PH : " + dtt.Tables[0].Rows[0]["NPhone"].ToString() + " " + " Fax : " + dtt.Tables[0].Rows[0]["NFax1"].ToString()));
                //run4s.Append(new Break());
                if (dtt.Tables[0].Rows[0]["Notify"].ToString() != "")
                {
                    run4s.AppendChild(new Text("Notify: " + dtt.Tables[0].Rows[0]["Notify"].ToString()));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["NotifyShipAdd"].ToString()));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text("PH : " + dtt.Tables[0].Rows[0]["NPhone"].ToString() + " " + " Fax : " + dtt.Tables[0].Rows[0]["NFax1"].ToString()));
                }
                else
                {
                    run4s.AppendChild(new Text("Notify : "));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustomerName"].ToString()));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text("PH:" + dtt.Tables[0].Rows[0]["Phone"].ToString() + " Fax: " + dtt.Tables[0].Rows[0]["Fax1"].ToString()));


                }
                Paragraph p41s = new Paragraph(run4s);
                ParagraphProperties paragraphProperties4s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p41s.Append(paragraphProperties4s);
                //tc41s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5100" }));
                TableCellProperties cellPropertiess4 = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On },
                    new TableCellWidth() { Type = TableWidthUnitValues.Pct });
                tc4221s.AppendChild(cellPropertiess4);

                tc4221s.Append(p41s);
                tr422s.Append(tc4221s);

                tbl52su.Append(tr422s);
                tr.TableRow row51 = new tr.TableRow();
                tr.TableCell col52 = new tr.TableCell();
                tr.Table tbl33s = new tr.Table();
                SetTableStyle(tbl33s, "5000", false, true, true, false, true, true);
                tr.TableRow row33s = new tr.TableRow();
                tr.Paragraph pd3s = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd3s.Append(paragraphProperties21ss3s);
                Run rr3 = new tr.Run();
                RunProperties rp333s = new RunProperties();
                rp333s.FontSize = new tr.FontSize() { Val = "16" };

                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont6 = new RunFonts();   //m1 
                runFont6.Ascii = "Arial";//m1
                rr3.Append(runFont6);//m1
                rr3.Append(rp333s);
                rr3.Append(new Text("Pre-Carriage by:" + dtt.Tables[0].Rows[0]["PreCrgBy"].ToString().Trim().ToString()));
                rr3.Append(new Break());
                rr3.Append(new Break());
                pd3s.Append(rr3);
                tr.TableCell col33s = new tr.TableCell();
                col33s.Append(pd3s);
                col33s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                row33s.Append(col33s);

                tr.Paragraph pd33ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd33ss.Append(paragraphProperties21ss33ss);
                Run rr33 = new tr.Run();
                RunProperties rp533s = new RunProperties();
                rp533s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont7 = new RunFonts();   //m1 
                runFont7.Ascii = "Arial";//m1
                rr33.Append(runFont7);//m1
                rr33.Append(rp533s);
                rr33.Append(new Text("Place of receipt by Pre-Carriage: " + dtt.Tables[0].Rows[0]["PlcRcpntPreCrgBy"].ToString()));
                rr33.Append(new Break());
                rr33.Append(new Break());
                pd33ss.Append(rr33);

                tr.TableCell col333ss = new tr.TableCell();
                col333ss.Append(pd33ss);
                col333ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                row33s.Append(col333ss);
                tbl33s.Append(row33s);

                tr.TableRow row63s = new tr.TableRow();
                tr.Paragraph pd63s = new tr.Paragraph();

                ParagraphProperties paragraphProperties61ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd63s.Append(paragraphProperties61ss3s);
                Run rr63 = new tr.Run();
                RunProperties rp633s = new RunProperties();
                rp633s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont8 = new RunFonts();   //m1 
                runFont8.Ascii = "Arial";//m1
                rr63.Append(runFont8);//m1
                rr63.Append(rp633s);
                rr63.Append(new Text("Vessel/Flight No: " + dtt.Tables[0].Rows[0]["ShipmentMode"].ToString()));
                rr63.Append(new Break());
                rr63.Append(new Break());
                pd63s.Append(rr63);
                tr.TableCell col633s = new tr.TableCell();
                col633s.Append(pd63s);
                col633s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                //col633s.Append(new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On }));
                row63s.Append(col633s);

                tr.Paragraph pd633ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties61ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd633ss.Append(paragraphProperties61ss33ss);
                Run rr633 = new tr.Run();
                RunProperties rp63s = new RunProperties();
                rp63s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont9 = new RunFonts();   //m1 
                runFont9.Ascii = "Arial";//m1
                rr633.Append(runFont9);//m1
                rr633.Append(rp63s);
                rr633.Append(new Text("Port of Loading: " + dtt.Tables[0].Rows[0]["PrtLdng"].ToString()));
                rr633.Append(new Break());
                rr633.Append(new Break());
                pd633ss.Append(rr633);
                tr.TableCell col6333ss = new tr.TableCell();
                col6333ss.Append(pd633ss);
                //col6333ss.Append(new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On }));
                col6333ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                row63s.Append(col6333ss);
                tbl33s.Append(row63s);

                tr.TableRow row73s = new tr.TableRow();
                tr.Paragraph pd73s = new tr.Paragraph();

                ParagraphProperties paragraphProperties71ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd73s.Append(paragraphProperties71ss3s);
                Run rr73 = new tr.Run();
                RunProperties rp73s = new RunProperties();
                rp73s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont10 = new RunFonts();   //m1 
                runFont10.Ascii = "Arial";//m1
                rr73.Append(runFont10);//m1
                rr73.Append(rp73s);
                rr73.Append(new Text("Port of Discharge:" + dtt.Tables[0].Rows[0]["PrtDschrg"].ToString()));
                rr73.Append(new Break());
                rr73.Append(new Break());
                pd73s.Append(rr73);
                tr.TableCell col733s = new tr.TableCell();
                col733s.Append(pd73s);
                col733s.Append(new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On }));
                col733s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                row73s.Append(col733s);

                tr.Paragraph pd733ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties71ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd733ss.Append(paragraphProperties71ss33ss);
                Run rr733 = new tr.Run();
                RunProperties rp773s = new RunProperties();
                rp773s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont11 = new RunFonts();   //m1 
                runFont11.Ascii = "Arial";//m1
                rr733.Append(runFont11);//m1
                rr733.Append(rp773s);
                rr733.Append(new Text("Place of Delivery:" + dtt.Tables[0].Rows[0]["PlcDlvry"].ToString()));
                rr733.Append(new Break());

                pd733ss.Append(rr733);
                tr.TableCell col7333ss = new tr.TableCell();
                col7333ss.Append(pd733ss);
                col7333ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2900" }));
                row73s.Append(col7333ss);
                tbl33s.Append(row73s);

                //tr.TableCell col3s = new tr.TableCell();
                col52.Append(new Paragraph(new Run(tbl33s)));
                row51.Append(col52);
                tbl52su.Append(row51);

                tc41s.Append(new Paragraph(new Run(tbl52su)));
                tr4s.Append(tc41s);

                tr.TableCell tc42s = new tr.TableCell();
                Paragraph p42s = new Paragraph();
                RunProperties rp42s = new RunProperties();
                Run run42s = new tr.Run();

                //rp42s.Bold = new Bold();
                rp42s.FontSize = new tr.FontSize() { Val = "16" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont12 = new RunFonts();   //m1 
                runFont12.Ascii = "Arial";//m1
                run42s.Append(runFont12);//m1
                ParagraphProperties pp42s = new ParagraphProperties();
                pp42s.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties42s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                p42s.Append(paragraphProperties42s);
                run42s.Append(rp42s);
                run42s.AppendChild(new Text("Terms of Delivery and Payment:"));
                run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["Incoterm"].ToString()));
                run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["IncTrmLctn"].ToString()));
                run42s.AppendChild(new Break());

                foreach (var gg in dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split('\n'))
                {

                    run42s.AppendChild(new Text(gg));
                    if (dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split('\n').Last() != gg)
                        run42s.AppendChild(new Break());
                }

                p42s.Append(run42s);
                p42s.Append(pp42s);
                tc42s.Append(p42s);
                //tc42s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct}));
                tr4s.Append(tc42s);
                table1.Append(tr4s);

                tr.TableRow tr8 = new tr.TableRow();
                tr.TableCell tc81 = new tr.TableCell();
                TableCellProperties trp81 = new TableCellProperties();
                HorizontalMerge vm81 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                trp81.Append(vm81);
                TableCellProperties trp82 = new TableCellProperties();
                HorizontalMerge vm82 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                trp82.Append(vm82);
                Paragraph p81s = new Paragraph();
                Run r81s = new Run();
                RunProperties rp81s = new RunProperties();
                rp81s.Bold = new Bold();
                rp81s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont13 = new RunFonts();   //m1 
                runFont13.Ascii = "Arial";//m1
                rp81s.Append(runFont13);//m1
                ParagraphProperties pp81s = new ParagraphProperties();
                pp81s.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties81s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                p81s.Append(paragraphProperties81s);
                r81s.Append(rp81s);

                tr.Table tbl9s = new tr.Table();
                SetTableStyle(tbl9s, "5000", false, false, true, false, true, true);
                tr.TableRow row91s = new tr.TableRow();
                tr.TableCell col91s = new tr.TableCell();
                tr.Paragraph pd91s = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd91s.Append(paragraphProperties91ss3s);
                Run rr91 = new tr.Run();
                RunProperties rp91s = new RunProperties();
                rp91s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont14 = new RunFonts();   //m1 
                runFont14.Ascii = "Arial";//m1
                rr91.Append(runFont14);//m1
                rp91s.Bold = new Bold();
                rr91.Append(rp91s);
                rr91.Append(new Text("Marks & Nos.No     No & Kind of Pkg.      Description of Goods"));

                rr91.AppendChild(new Break());
                tr.Paragraph pd91s1 = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3s1 = new ParagraphProperties(
                                            new ParagraphStyleId() { Val = "No Spacing" },
                                            new SpacingBetweenLines() { After = "0" },
                                            new Justification() { Val = JustificationValues.Left }
                                            );
                pd91s1.Append(paragraphProperties91ss3s1);
                rr91.AppendChild(new Text(dtt1.Tables[0].Rows[0]["CustNm"].ToString() + "                          No. of Pkgs.  " + dtt1.Tables[0].Rows[0]["TotalPkgs"].ToString()));
                rr91.AppendChild(new Break());
                rr91.AppendChild(new Text(dtt.Tables[0].Rows[0]["PlcFnlDstn"].ToString() + "                   No." + dtt1.Tables[0].Rows[0]["TotalPkgsToFrm"].ToString()));
                pd91s1.Append(rr91);

                col91s.Append(pd91s1);
                col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3350" }));
                row91s.Append(col91s);

                tr.TableCell col92ssb = new tr.TableCell();
                tr.Paragraph pd92ssb = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ssb = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ssb.Append(paragraphProperties92ss33ssb);
                Run rr92b = new tr.Run();
                RunProperties rp92sb = new RunProperties();
                rp92sb.FontSize = new tr.FontSize() { Val = "18" };
                //rp22s.FontSize = new tr.FontSize() { Val = "20" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont15 = new RunFonts();   //m1 
                runFont15.Ascii = "Arial";//m1
                rr92b.Append(runFont15);//m1
                rp92sb.Bold = new Bold();
                rr92b.Append(rp92sb);
                rr92b.Append(new Text("HSNCODE"));
                //rr92b.Append(new Text(" "));
                pd92ssb.Append(rr92b);

                col92ssb.Append(pd92ssb);
                col92ssb.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "230" }));
                row91s.Append(col92ssb);

                tr.TableCell col92ss = new tr.TableCell();
                tr.Paragraph pd92ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ss.Append(paragraphProperties92ss33ss);
                Run rr92 = new tr.Run();
                RunProperties rp92s = new RunProperties();
                rp92s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont16 = new RunFonts();   //m1 
                runFont16.Ascii = "Arial";//m1
                rp92s.Append(runFont16);//m1
                rp92s.Bold = new Bold();
                rr92.Append(rp92s);
                rr92.Append(new Text("Quantity"));
                pd92ss.Append(rr92);

                col92ss.Append(pd92ss);
                col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "200" }));
                row91s.Append(col92ss);

                tr.TableCell col93ss = new tr.TableCell();
                tr.Paragraph pd93ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties93ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd93ss.Append(paragraphProperties93ss33ss);
                Run rr93 = new tr.Run();
                RunProperties rp93s = new RunProperties();
                rp93s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont17 = new RunFonts();   //m1 
                runFont17.Ascii = "Arial";//m1
                rp93s.Append(runFont17);//m1
                rp93s.Bold = new Bold();
                rr93.Append(rp93s);
                rr93.Append(new Text("Unit Rate($)"));
                pd93ss.Append(rr93);

                col93ss.Append(pd93ss);
                col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "240" }));
                row91s.Append(col93ss);

                tr.TableCell col94ss = new tr.TableCell();
                tr.Paragraph pd94ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties94ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd94ss.Append(paragraphProperties94ss33ss);
                Run rr94 = new tr.Run();
                RunProperties rp94s = new RunProperties();
                rp94s.FontSize = new tr.FontSize() { Val = "18" };
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };//m1
                RunFonts runFont18 = new RunFonts();   //m1 
                runFont18.Ascii = "Arial";//m1
                rp94s.Append(runFont18);//m1
                rp94s.Bold = new Bold();
                rr94.Append(rp94s);
                rr94.Append(new Text("Amount($)"));
                pd94ss.Append(rr94);
                col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "210" }));
                col94ss.Append(pd94ss);

                row91s.Append(col94ss);

                tbl9s.Append(row91s);
                //

                //col52.Append(new Paragraph(new Run(tbl33s)));
                r81s.Append(tbl9s);
                p81s.Append(r81s);
                p81s.Append(pp81s);
                tc81.Append(p81s);
                tr8.Append(tc81);

                tr.TableCell tc82s = new tr.TableCell();
                Paragraph p82s = new Paragraph();

                tc81.Append(trp81);
                tc82s.Append(trp82);
                tc82s.Append(p82s);
                tr8.Append(tc82s);
                table1.Append(tr8);

                //table1.Append(row51);

                h.Append(table1);
                //derivedclass dd = new derivedclass();
                //var uu = dd.getarea(7);
                //var uo = dd.interestpermonth(5);
                //var kh = dd.totalamount(4, 45);
                headerPart.Header = h;
                SectionProperties sectionProperties1 = mainPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties1 == null)
                {
                    sectionProperties1 = new SectionProperties() { };
                    mainPart.Document.Body.Append(sectionProperties1);
                }
                HeaderReference headerReference1 = new HeaderReference() { Type = HeaderFooterValues.Default, Id = "r95" };
                sectionProperties1.InsertAt(headerReference1, 0);

                #endregion

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Others/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }

        private void GenerateFooterPart(MainDocumentPart mainPart, FooterPart footerPart)
        {
            try
            {
                #region Footer

                tr.Table table = new tr.Table();
                // Add a Paragraph and a Run with the specified Text 
                DataSet dtt = (DataSet)Session["DtComsInvset"];
                Footer f = new Footer();
                SetTableStyle(table, "6000", true, true, true, true, true, true);

                tr.Table tables = new tr.Table();
                SetTableStyle(tables, "5000", false, true, true, false, true, true);
                tr.TableRow row91ss = new tr.TableRow();
                tr.TableCell col91ss = new tr.TableCell();

                #region Commented on 18-07-2018


                //tr.TableRow row91s = new tr.TableRow();
                //tr.TableCell col91s = new tr.TableCell();
                //tr.Paragraph pd91s = new tr.Paragraph();
                //ParagraphProperties paragraphProperties91ss3s = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Right }
                //                              );
                //pd91s.Append(paragraphProperties91ss3s);
                //Run rr91 = new tr.Run();
                //RunProperties rp91s = new RunProperties();
                //rp91s.FontSize = new tr.FontSize() { Val = "20" };
                //rp91s.Bold = new Bold();
                //rr91.Append(rp91s);
                //rr91.Append(new Text("GST PAYABLE UNDER REVERSE CHARGE:") { Space = SpaceProcessingModeValues.Preserve });
                //pd91s.Append(rr91);

                //col91s.Append(pd91s);
                //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "980" }));
                //row91s.Append(col91s);

                //tr.TableCell col92ss = new tr.TableCell();
                //tr.Paragraph pd92ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties92ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd92ss.Append(paragraphProperties92ss33ss);
                //Run rr92 = new tr.Run();
                //RunProperties rp92s = new RunProperties();
                //rp92s.FontSize = new tr.FontSize() { Val = "20" };
                //rp92s.Bold = new Bold();
                //rr92.Append(rp92s);
                //rr92.Append(new Text(" "));
                //pd92ss.Append(rr92);

                //col92ss.Append(pd92ss);
                //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "300" }));
                //row91s.Append(col92ss);

                //tr.TableCell col93ss = new tr.TableCell();
                //tr.Paragraph pd93ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties93ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd93ss.Append(paragraphProperties93ss33ss);
                //Run rr93 = new tr.Run();
                //RunProperties rp93s = new RunProperties();
                //rp93s.FontSize = new tr.FontSize() { Val = "20" };
                //rp93s.Bold = new Bold();
                //rr93.Append(rp93s);
                //rr93.Append(new Text(" "));
                //pd93ss.Append(rr93);

                //col93ss.Append(pd93ss);
                //col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "250" }));
                //row91s.Append(col93ss);

                //tr.TableCell col94ss = new tr.TableCell();
                //tr.Paragraph pd94ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties94ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd94ss.Append(paragraphProperties94ss33ss);
                //Run rr94 = new tr.Run();
                //RunProperties rp94s = new RunProperties();
                //rp94s.FontSize = new tr.FontSize() { Val = "20" };
                //rp94s.Bold = new Bold();
                //rr94.Append(rp94s);
                //rr94.Append(new Text("0.00"));
                //pd94ss.Append(rr94);

                //col94ss.Append(pd94ss);
                //col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "375" }));
                //row91s.Append(col94ss);

                //tables.Append(row91s);


                //tr.TableRow tr1s = new tr.TableRow();
                //TableCellProperties tableCellPropertiess = new TableCellProperties();
                //HorizontalMerge verticalMerges = new HorizontalMerge()
                //{
                //    Val = MergedCellValues.Restart
                //};
                //tableCellPropertiess.Append(verticalMerges);
                //TableCellProperties tableCellProperties1s = new TableCellProperties();
                //HorizontalMerge verticalMerge1s = new HorizontalMerge()
                //{
                //    Val = MergedCellValues.Continue
                //};
                //tableCellProperties1s.Append(verticalMerge1s);
                //tr.TableCell tc11s = new tr.TableCell();
                //Paragraph p11s = new Paragraph();
                //Run r12s = new Run();
                //RunProperties rp12s = new RunProperties();
                //rp12s.Bold = new Bold();
                //rp12s.FontSize = new tr.FontSize() { Val = "20" };
                //ParagraphProperties pp11s = new ParagraphProperties();
                //pp11s.Justification = new Justification() { Val = JustificationValues.Left };
                //ParagraphProperties paragraphProperties11s = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" }
                //                              );
                //p11s.Append(paragraphProperties11s);
                //p11s.Append(pp11s);
                //r12s.Append(rp12s);
                //r12s.Append((tables));
                //p11s.Append(r12s);
                //tc11s.Append(p11s);
                //tr1s.Append(tc11s);
                //TableRowProperties tableRowProperties1s = new TableRowProperties();
                //TableRowHeight tableRowHeight1s = new TableRowHeight() { Val = (UInt32Value)28U };

                //tableRowProperties1s.Append(tableRowHeight1s);
                //tr1s.InsertBefore(tableRowProperties1s, tc11s);
                //tr.TableCell tc12s = new tr.TableCell();
                //Paragraph p12s = new Paragraph();

                //tc11s.Append(tableCellPropertiess);
                //tc12s.Append(tableCellProperties1s);
                //tc12s.Append(p12s);
                //tr1s.Append(tc12s);
                //table.Append(tr1s);

                //tr.TableRow tr1 = new tr.TableRow();
                //TableCellProperties tableCellProperties = new TableCellProperties();
                //HorizontalMerge verticalMerge = new HorizontalMerge()
                //{
                //    Val = MergedCellValues.Restart
                //};
                //tableCellProperties.Append(verticalMerge);
                //TableCellProperties tableCellProperties1 = new TableCellProperties();
                //HorizontalMerge verticalMerge1 = new HorizontalMerge()
                //{
                //    Val = MergedCellValues.Continue
                //};
                //tableCellProperties1.Append(verticalMerge1);
                //tr.TableCell tc11 = new tr.TableCell();
                //Paragraph p11 = new Paragraph();
                //Run r12 = new Run();
                //RunProperties rp12 = new RunProperties();
                //rp12.Bold = new Bold();
                //rp12.FontSize = new tr.FontSize() { Val = "20" };
                //ParagraphProperties pp11 = new ParagraphProperties();
                //pp11.Justification = new Justification() { Val = JustificationValues.Left };
                //ParagraphProperties paragraphProperties11 = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" }
                //                              );
                //p11.Append(paragraphProperties11);
                //p11.Append(pp11);
                //r12.Append(rp12);
                //r12.Append(new Text("In Words :" + Session["TotalAmt"].ToString() + ""));
                //p11.Append(r12);
                //tc11.Append(p11);
                //tr1.Append(tc11);
                //TableRowProperties tableRowProperties1 = new TableRowProperties();
                //TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)28U };

                //tableRowProperties1.Append(tableRowHeight1);
                //tr1.InsertBefore(tableRowProperties1, tc11);
                //tr.TableCell tc12 = new tr.TableCell();
                //Paragraph p12 = new Paragraph();

                //tc11.Append(tableCellProperties);
                //tc12.Append(tableCellProperties1);
                //tc12.Append(p12);
                //tr1.Append(tc12);
                //table.Append(tr1);

                #endregion

                tr.TableRow tr12 = new tr.TableRow();
                TableCellProperties tableCellProperties12 = new TableCellProperties();
                HorizontalMerge verticalMerge12 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                tableCellProperties12.Append(verticalMerge12);
                TableCellProperties tableCellProperties112 = new TableCellProperties();
                HorizontalMerge verticalMerge112 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                tableCellProperties112.Append(verticalMerge112);
                tr.TableCell tc1112 = new tr.TableCell();
                Paragraph p1112 = new Paragraph();
                Run r1212 = new Run();
                RunProperties rp1212 = new RunProperties();
                rp1212.Bold = new Bold();
                RunFonts runFont_1 = new RunFonts();
                runFont_1.Ascii = "Arial";
                rp1212.Append(runFont_1);
                rp1212.FontSize = new tr.FontSize() { Val = "18" };
                ParagraphProperties pp1112 = new ParagraphProperties();
                pp1112.Justification = new Justification() { Val = JustificationValues.Left };
                ParagraphProperties paragraphProperties1112 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p1112.Append(paragraphProperties1112);
                p1112.Append(pp1112);
                r1212.Append(rp1212);
                r1212.Append(new Text("SUPPLY MEANT FOR EXPORT UNDER LETTER OF UNDERTAKING (LUT) WITHOUT PAYMENT OF INTEGRATED TAX//" + dtt.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[0] + "," + dtt.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[1] + "  ISSUED BY GACHIBOWLI DIVISION,RANGAREDDY GST COMMISSIONARATE"));
                p1112.Append(r1212);
                tc1112.Append(p1112);
                tr12.Append(tc1112);
                TableRowProperties tableRowProperties112 = new TableRowProperties();
                TableRowHeight tableRowHeight112 = new TableRowHeight() { Val = (UInt32Value)28U };

                tableRowProperties112.Append(tableRowHeight112);
                tr12.InsertBefore(tableRowProperties112, tc1112);
                tr.TableCell tc1212 = new tr.TableCell();
                Paragraph p1212 = new Paragraph();

                tc1112.Append(tableCellProperties12);
                tc1212.Append(tableCellProperties112);
                tc1212.Append(p1212);
                tr12.Append(tc1212);
                table.Append(tr12);

                tr.TableRow tr2 = new tr.TableRow();
                string getts = string.Empty;
                //getts = "WE, HERE BY DECLARE THAT WE INTEND TO CLAIM REWARDS UNDER MERCHANDISE EXPORTS  "+ Environment.NewLine
                //                + "FROM INDIA SCHEME (MEIS) OF FTP 2015-2020 " + Environment.NewLine
                //                + "I/WE UNDERTAKE TO ABIDE BY THE PROVISIONS OF FOREIGN EXCHANGE MANAGEMENT ACT, 1999, " + Environment.NewLine
                //                + "AS AMENDED FROM TIME TO TIME, INCLUDING REALISATION OR REPATRIATION OF FOREIGN " + Environment.NewLine 
                //                + "EXCHANGE TO OR FROM INDIA ";
                tr.TableCell tc21 = new tr.TableCell();
                tr.Run runn = new tr.Run();
                RunProperties rp22 = new RunProperties();
                rp22.FontSize = new tr.FontSize() { Val = "16" };
                RunFonts runFont_2 = new RunFonts();           // Create font
                runFont_2.Ascii = "Arial";
                rp22.Append(runFont_2);
                runn.Append(rp22);
                runn.AppendChild(new Text("WE, HERE BY DECLARE THAT WE INTEND TO CLAIM REWARDS UNDER MERCHANDISE EXPORTS"));
                runn.AppendChild(new Break());
                runn.AppendChild(new Text("FROM INDIA SCHEME (MEIS) OF FTP 2015-2020"));
                runn.AppendChild(new Break());
                runn.AppendChild(new Text("I/WE UNDERTAKE TO ABIDE BY THE PROVISIONS OF FOREIGN EXCHANGE MANAGEMENT ACT, 1999, "));
                runn.AppendChild(new Break());
                runn.AppendChild(new Text("AS AMENDED FROM TIME TO TIME, INCLUDING REALISATION OR REPATRIATION OF FOREIGN"));
                runn.AppendChild(new Break());
                runn.AppendChild(new Text("EXCHANGE TO OR FROM INDIA"));
                //runn.AppendChild(new Break());

                Paragraph p21 = new Paragraph(runn);
                ParagraphProperties paragraphProperties1 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p21.Append(paragraphProperties1);
                tc21.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2900" }));
                TableCellProperties cellProperties = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                tc21.AppendChild(cellProperties);
                tc21.Append(p21);
                tr2.Append(tc21);

                tr.TableCell tc22 = new tr.TableCell();
                Paragraph p22 = new Paragraph();
                ParagraphProperties pp22 = new ParagraphProperties();
                //pp22.Justification = new Justification() { Val = JustificationValues.Right };
                ParagraphProperties paragraphProperties21 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Right }
                                              );
                p22.Append(paragraphProperties21);
                p22.Append(pp22);
                tr.Run runn3 = new tr.Run();
                RunProperties rp55 = new RunProperties();
                RunFonts runFont = new RunFonts();           // Create font
                rp55.FontSize = new tr.FontSize() { Val = "18" };
                rp55.Bold = new Bold();
                runFont.Ascii = "Arial";
                rp55.Append(runFont);
                runn3.Append(rp55);
                runn3.AppendChild(new Text("For " + dtt.Tables[0].Rows[0]["CompanyName"].ToString()));
                runn3.AppendChild(new Break());
                runn3.AppendChild(new Break());
                runn3.AppendChild(new Break());
                runn3.AppendChild(new Break());
                runn3.AppendChild(new Text("Authorized Signature"));
                p22.Append(runn3);
                tc22.Append(p22);

                tr2.Append(tc22);
                table.Append(tr2);
                //adding
                tr.TableRow tr121 = new tr.TableRow();
                TableCellProperties tableCellProperties121 = new TableCellProperties();
                HorizontalMerge verticalMerge121 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                tableCellProperties121.Append(verticalMerge121);
                TableCellProperties tableCellProperties1121 = new TableCellProperties();
                HorizontalMerge verticalMerge1121 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                tableCellProperties1121.Append(verticalMerge1121);
                tr.TableCell tc11121 = new tr.TableCell();
                Paragraph p11121 = new Paragraph();
                Run r12121 = new Run();
                RunProperties rp12121 = new RunProperties();
                rp12121.Bold = new Bold();
                RunFonts runFont_14 = new RunFonts();
                runFont_14.Ascii = "Arial";
                rp12121.Append(runFont_14);
                rp12121.FontSize = new tr.FontSize() { Val = "18" };
                ParagraphProperties pp11121 = new ParagraphProperties();
                pp11121.Justification = new Justification() { Val = JustificationValues.Left };
                ParagraphProperties paragraphProperties11121 = new ParagraphProperties(
                    //new ParagraphStyleId() { Val = "No Spacing" },
                    //new SpacingBetweenLines() { After = "0" }
                    //);
                                                new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                p11121.Append(paragraphProperties11121);
                p11121.Append(pp11121);
                r12121.Append(rp12121);
                r12121.Append(new Text(dtt.Tables[0].Rows[0]["BL_AWB_No"].ToString() + dtt.Tables[0].Rows[0]["SB_NO"].ToString()));
                p11121.Append(r12121);
                tc11121.Append(p11121);
                tr121.Append(tc11121);
                TableRowProperties tableRowProperties1121 = new TableRowProperties();
                TableRowHeight tableRowHeight1121 = new TableRowHeight() { Val = (UInt32Value)28U };

                tableRowProperties1121.Append(tableRowHeight1121);
                tr121.InsertBefore(tableRowProperties1121, tc11121);
                tr.TableCell tc12121 = new tr.TableCell();
                Paragraph p12121 = new Paragraph();

                tc11121.Append(tableCellProperties121);
                tc12121.Append(tableCellProperties1121);
                tc12121.Append(p12121);
                tr121.Append(tc12121);
                table.Append(tr121);

                f.Append(table);




                footerPart.Footer = f;
                SectionProperties sectionProperties1 = mainPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties1 == null)
                {
                    sectionProperties1 = new SectionProperties() { };
                    mainPart.Document.Body.Append(sectionProperties1);
                }
                FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.Default, Id = "r96" };

                sectionProperties1.InsertAt(footerReference1, 0);

                #endregion
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Others/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }

        #region NOTUsed
        //private void GenerateBodyPart(MainDocumentPart mainPart, Body body)
        //{
        //    try
        //    {
        //        Paragraph p = new Paragraph();
        //        Run r = new Run();
        //        Text t = new Text("Hello Body");
        //        tr.Table table = new tr.Table();

        //     //   DataSet dtt = (DataSet)Session["DtComsInv"];
        //       // DataSet dtt1 = (DataSet)Session["DtComsInvset"];
        //        DataSet dtt = (DataSet)Session["DtComsInv"];
        //        DataSet dtt1 = (DataSet)Session["DtComsInvset"];
        //        SetTableStyle(table, "6000", true, true, true, true, false, true);
        //        tr.TableRow row91s = new tr.TableRow();
        //        tr.TableCell col91s = new tr.TableCell();
        //        tr.Paragraph pd91s = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties91ss3s = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Left }
        //                                      );
        //        pd91s.Append(paragraphProperties91ss3s);
        //        Run rr91 = new tr.Run();
        //        RunProperties rp91s = new RunProperties();
        //        rp91s.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR1 = new RunFonts();   //m1 
        //        runFontR1.Ascii = "Arial";//m1
        //        rp91s.Append(runFontR1);//m1
        //        rp91s.Bold = new Bold();
        //        rr91.Append(rp91s);
        //        rr91.Append(new Text(""));
        //        //rr91.Append(new Text(dtt.Tables[0].Rows[0]["CustNm"].ToString() + "             No. of Pkgs.  " + dtt.Tables[0].Rows[0]["TotalPkgs"].ToString() + "No(s)") { Space = SpaceProcessingModeValues.Preserve });
        //        //rr91.Append(new Text(dtt.Tables[0].Rows[0]["CustNm"].ToString()) { Space = SpaceProcessingModeValues.Preserve });
        //        pd91s.Append(rr91);

        //        col91s.Append(pd91s);
        //        col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2345" }));
        //        row91s.Append(col91s);

        //        tr.TableCell col92ssz = new tr.TableCell();
        //        tr.Paragraph pd92ssz = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties92ss33ssz = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd92ssz.Append(paragraphProperties92ss33ssz);
        //        Run rr92z = new tr.Run();
        //        RunProperties rp92sz = new RunProperties();
        //        rp92sz.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR2 = new RunFonts();   //m1 
        //        runFontR2.Ascii = "Arial";//m1
        //        rp92sz.Append(runFontR2);//m1
        //        rp92sz.Bold = new Bold();
        //        rr92z.Append(rp92sz);
        //        rr92z.Append(new Text(" "));
        //        pd92ssz.Append(rr92z);

        //        col92ssz.Append(pd92ssz);
        //        col92ssz.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "656" }));
        //        row91s.Append(col92ssz);


        //        tr.TableCell col92ss = new tr.TableCell();
        //        tr.Paragraph pd92ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties92ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd92ss.Append(paragraphProperties92ss33ss);
        //        Run rr92 = new tr.Run();
        //        RunProperties rp92s = new RunProperties();
        //        rp92s.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR3 = new RunFonts();   //m1 
        //        runFontR3.Ascii = "Arial";//m1
        //        rp92s.Append(runFontR3);//m1
        //        rp92s.Bold = new Bold();
        //        rr92.Append(rp92s);
        //        rr92.Append(new Text(" "));
        //        pd92ss.Append(rr92);

        //        col92ss.Append(pd92ss);
        //        col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "526" }));//
        //        row91s.Append(col92ss);

        //        tr.TableCell col93ss = new tr.TableCell();
        //        tr.Paragraph pd93ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties93ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd93ss.Append(paragraphProperties93ss33ss);
        //        Run rr93 = new tr.Run();
        //        RunProperties rp93s = new RunProperties();
        //        rp93s.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR4 = new RunFonts();   //m1 
        //        runFontR4.Ascii = "Arial";//m1
        //        rp93s.Append(runFontR4);//m1
        //        rp93s.Bold = new Bold();
        //        rr93.Append(rp93s);
        //        rr93.Append(new Text(" "));
        //        pd93ss.Append(rr93);

        //        col93ss.Append(pd93ss);
        //        col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "449" }));//kk
        //        row91s.Append(col93ss);

        //        tr.TableCell col94ss = new tr.TableCell();
        //        tr.Paragraph pd94ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties94ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd94ss.Append(paragraphProperties94ss33ss);
        //        Run rr94 = new tr.Run();
        //        RunProperties rp94s = new RunProperties();
        //        rp94s.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR5 = new RunFonts();   //m1 
        //        runFontR5.Ascii = "Arial";//m1
        //        rp94s.Append(runFontR5);//m1
        //        rp94s.Bold = new Bold();
        //        rr94.Append(rp94s);
        //        rr94.Append(new Text(" "));
        //        pd94ss.Append(rr94);

        //        col94ss.Append(pd94ss);
        //        col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "666" }));
        //        row91s.Append(col94ss);

        //        table.Append(row91s);

        //        tr.TableRow row2s = new tr.TableRow();
        //        tr.TableCell col21s = new tr.TableCell();
        //        tr.Paragraph pd21s = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties21ss3s = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Left }
        //                                      );
        //        pd21s.Append(paragraphProperties21ss3s);
        //        Run rr21 = new tr.Run();
        //        RunProperties rp21s = new RunProperties();
        //        rp21s.FontSize = new tr.FontSize() { Val = "20" };
        //        RunFonts runFontR6 = new RunFonts();   //m1 
        //        runFontR6.Ascii = "Arial";//m1
        //        rp21s.Append(runFontR6);//m1
        //        rp21s.Bold = new Bold();
        //        rr21.Append(rp21s);
        //        rr21.Append(new Text(""));
        //        //rr21.Append(new Text(dtt.Tables[0].Rows[0]["PlcFnlDstn"].ToString() ) { Space = SpaceProcessingModeValues.Preserve });
        //        pd21s.Append(rr21);

        //        col21s.Append(pd21s);
        //        //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
        //        row2s.Append(col21s);

        //        tr.TableCell col22ssf = new tr.TableCell();
        //        tr.Paragraph pd22ssf = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties22ss33ssf = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd22ssf.Append(paragraphProperties22ss33ssf);
        //        Run rr22f = new tr.Run();
        //        RunProperties rp22sf = new RunProperties();
        //        rp22sf.FontSize = new tr.FontSize() { Val = "20" };
        //        rp22sf.Bold = new Bold();
        //        rr22f.Append(rp22sf);
        //        rr22f.Append(new Text(" "));
        //        pd22ssf.Append(rr22f);

        //        col22ssf.Append(pd22ssf);
        //        //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //        row2s.Append(col22ssf);

        //        tr.TableCell col22ss = new tr.TableCell();
        //        tr.Paragraph pd22ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties22ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd22ss.Append(paragraphProperties22ss33ss);
        //        Run rr22 = new tr.Run();
        //        RunProperties rp22s = new RunProperties();
        //        rp22s.FontSize = new tr.FontSize() { Val = "20" };
        //        rp22s.Bold = new Bold();
        //        rr22.Append(rp22s);
        //        rr22.Append(new Text(" "));
        //        pd22ss.Append(rr22);

        //        col22ss.Append(pd22ss);
        //        //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //        row2s.Append(col22ss);


        //        tr.TableCell col23ss = new tr.TableCell();
        //        tr.Paragraph pd23ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties23ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd23ss.Append(paragraphProperties23ss33ss);
        //        Run rr23 = new tr.Run();
        //        RunProperties rp23s = new RunProperties();
        //        rp23s.FontSize = new tr.FontSize() { Val = "20" };
        //        rp23s.Bold = new Bold();
        //        rr23.Append(rp23s);
        //        rr23.Append(new Text(" "));
        //        pd23ss.Append(rr23);

        //        col23ss.Append(pd23ss);
        //        //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //        row2s.Append(col23ss);

        //        tr.TableCell col24ss = new tr.TableCell();
        //        tr.Paragraph pd24ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties24ss33ss = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" },
        //                                      new Justification() { Val = JustificationValues.Center }
        //                                      );
        //        pd24ss.Append(paragraphProperties24ss33ss);
        //        Run rr24 = new tr.Run();
        //        RunProperties rp24s = new RunProperties();
        //        rp24s.FontSize = new tr.FontSize() { Val = "20" };
        //        rp24s.Bold = new Bold();
        //        rr24.Append(rp24s);
        //        rr24.Append(new Text(" "));
        //        pd24ss.Append(rr24);

        //        col24ss.Append(pd24ss);
        //        //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //        row2s.Append(col24ss);

        //        table.Append(row2s);

        //        int rowIndex = 1;
        //        string coltext = string.Empty;
        //        string coll = string.Empty;

        //        var kk = (dtt1.Tables[0].Rows[0]["FPOs"].ToString().Split(','));
        //        //var kkk = dtt.Tables[0].Select().OrderBy(k => k.ItemArray[15].ToString()).ToList();
        //        List<Tuple<string, string>> getFPONOS = new List<Tuple<string, string>>();
        //        string gettupl = string.Empty; string getguid = string.Empty; string getSno = string.Empty;
        //        foreach (DataRow getval in dtt.Tables[0].Rows)
        //        {
        //            if (getval.ItemArray[22].ToString() != getguid || getval.ItemArray[21].ToString() != getSno)
        //            {
        //                getguid = getval.ItemArray[22].ToString();
        //                getSno = getval.ItemArray[21].ToString();
        //                gettupl += getval.ItemArray[22].ToString() + ",";
        //                getFPONOS.Add(Tuple.Create(getval.ItemArray[21].ToString(), getval.ItemArray[22].ToString()));
        //            }
        //        }
        //        Array.Sort(kk);
        //        int rind = 1;
        //        foreach (var arr in (getFPONOS))
        //        {
        //            rowIndex = 1;

        //            tr.TableRow row1z = new tr.TableRow();
        //            tr.TableCell col1zd = new tr.TableCell();
        //           tr.TableCell col1z = new tr.TableCell();
        //            tr.TableCell col2z = new tr.TableCell();
        //            tr.TableCell col3z = new tr.TableCell();
        //            tr.TableCell col4z = new tr.TableCell();
        //            col1zd.Append(new Paragraph(new Run(new Text(""))));
        //            row1z.Append(col1zd);
        //            col1z.Append(new Paragraph(new Run(new Text(""))));
        //            row1z.Append(col1z);
        //            col2z.Append(new Paragraph(new Run(new Text(""))));
        //            row1z.Append(col2z);
        //            col3z.Append(new Paragraph(new Run(new Text(""))));
        //            row1z.Append(col3z);
        //            col4z.Append(new Paragraph(new Run(new Text(""))));
        //            row1z.Append(col4z);
        //            if (rind != 1)
        //                table.Append(row1z);
        //            rind++;
        //            tr.TableRow row1 = new tr.TableRow();
        //            tr.TableCell col1 = new tr.TableCell();
        //            Run run1s = new tr.Run();
        //            Paragraph p1s = new Paragraph();
        //            ParagraphProperties paragraphProperties1s = new ParagraphProperties(
        //                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                          new SpacingBetweenLines() { After = "0", Before = "0" },
        //                                          new ContextualSpacing() { Val = false }
        //                                          );
        //            TableCellProperties cellPropertiess1 = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
        //            RunProperties rp1s = new RunProperties();

        //            rp1s.Bold = new Bold();
        //            rp1s.FontSize = new tr.FontSize() { Val = "22" };
        //            RunFonts runFontR7 = new RunFonts();   //m1 
        //            runFontR7.Ascii = "Arial";//m1
        //            run1s.Append(runFontR7);//m1
        //            run1s.Append(rp1s);
        //            var dt = dtt.Tables[0].Select("ForeignPOId = '" + arr.Item2 + "' and FPOSNo = '" + arr.Item1 + "'").CopyToDataTable();

        //            // var dt = dtt.Tables[0].Select("ForeignPOId = '" + arr.Item2 + "'" ).CopyToDataTable();

        //            coltext = "  " + dt.Rows[0]["FPOSubHeading"].ToString();

        //            run1s.AppendChild(new Text(coltext) { Space = SpaceProcessingModeValues.Preserve });
        //            p1s.Append(paragraphProperties1s);



        //            col1.AppendChild(cellPropertiess1);
        //            p1s.Append(run1s);
        //            col1.Append(p1s);
        //            row1.Append(col1);
        //            tr.TableCell col2e = new tr.TableCell();
        //            tr.TableCell col2 = new tr.TableCell();
        //            tr.TableCell col3 = new tr.TableCell();
        //            tr.TableCell col4 = new tr.TableCell();
        //            col2e.Append(new Paragraph(new Run(new Text(""))));
        //            row1.Append(col2e);
        //            col2.Append(new Paragraph(new Run(new Text(""))));
        //            row1.Append(col2);
        //            col3.Append(new Paragraph(new Run(new Text(""))));
        //            row1.Append(col3);
        //            col4.Append(new Paragraph(new Run(new Text(""))));
        //            row1.Append(col4);

        //            table.Append(row1);
        //            foreach (DataRow row in dtt.Tables[0].Rows)
        //            {
        //                if (arr.Item2 == row["ForeignPOId"].ToString() && arr.Item1 == row["FPOSNo"].ToString())

        //                    if (rowIndex == 1 && coltext.ToString().Trim() != row["FPOSubHeading"].ToString().Trim())
        //                    {
        //                        rowIndex = 1;
        //                    }
        //                    else
        //                    {
        //                        if (arr.Item2 == row["ForeignPOId"].ToString() && arr.Item1 == row["FPOSNo"].ToString())
        //                        {
        //                            rowIndex = 2;
        //                            tr.TableRow row4s = new tr.TableRow();
        //                            tr.TableCell col4s = new tr.TableCell();
        //                            tr.Paragraph pd4s = new tr.Paragraph();
        //                            ParagraphProperties paragraphProperties4ss3s = new ParagraphProperties(
        //                                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                                          new SpacingBetweenLines() { After = "0", Before = "0", Line = "0", LineRule = LineSpacingRuleValues.Exact },
        //                                                          new Justification() { Val = JustificationValues.Left },
        //                                                          new ContextualSpacing() { Val = false }
        //                                                          );
        //                            pd4s.Append(paragraphProperties4ss3s);
        //                            Run rr4 = new tr.Run();
        //                            RunProperties rp4s = new RunProperties();
        //                            rp4s.FontSize = new tr.FontSize() { Val = "17" };
        //                            RunFonts runFont = new RunFonts();           // Create font
        //                            runFont.Ascii = "Arial";
        //                            rp4s.Append(runFont);
        //                            rr4.Append(rp4s);
        //                            coll += "  " + row["Description"].ToString();
        //                            if (row["Spec_Make"].ToString() != "")
        //                                coll += "," + row["Spec_Make"].ToString();
        //                            if (row["PartNumber"].ToString() != "")
        //                                coll += "," + row["PartNumber"].ToString();

        //                            rr4.Append(new Text(coll) { Space = SpaceProcessingModeValues.Preserve });
        //                            pd4s.Append(rr4);

        //                            col4s.Append(pd4s);
        //                            //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
        //                            row4s.Append(col4s);

        //                            #region imp hsn
        //                            tr.TableCell col42ssh = new tr.TableCell();
        //                            tr.Paragraph pd42ssh = new tr.Paragraph();
        //                            ParagraphProperties paragraphProperties42ss33ssh = new ParagraphProperties(
        //                                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                                          new SpacingBetweenLines() { After = "0" },
        //                                                          new Justification() { Val = JustificationValues.Center }
        //                                                          );
        //                            pd42ssh.Append(paragraphProperties42ss33ssh);
        //                            Run rr42h = new tr.Run();
        //                            RunProperties rp42sh = new RunProperties();
        //                            rp42sh.FontSize = new tr.FontSize() { Val = "18" };
        //                            RunFonts runFontsh = new RunFonts();           // Create font
        //                            runFontsh.Ascii = "Arial";
        //                            rp42sh.Append(runFontsh);
        //                            rr42h.Append(rp42sh);
        //                            rr42h.Append(new Text(row["HSCode"].ToString()));
        //                            //rr42h.Append(new Text(" "));
        //                            pd42ssh.Append(rr42h);

        //                            col42ssh.Append(pd42ssh);
        //                            //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //                            row4s.Append(col42ssh);
        //                            // rr42h.Remove();
        //                            // col42ssh.Remove();


        //                            #endregion





        //                            tr.TableCell col42ss = new tr.TableCell();
        //                            tr.Paragraph pd42ss = new tr.Paragraph();

        //                            ParagraphProperties paragraphProperties42ss33ss = new ParagraphProperties(
        //                                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                                          new SpacingBetweenLines() { After = "0" },
        //                                                          new Justification() { Val = JustificationValues.Center }
        //                                                          );
        //                            pd42ss.Append(paragraphProperties42ss33ss);
        //                            Run rr42 = new tr.Run();
        //                            RunProperties rp42s = new RunProperties();
        //                            rp42s.FontSize = new tr.FontSize() { Val = "18" };
        //                            RunFonts runFonts = new RunFonts();           // Create font
        //                            runFonts.Ascii = "Arial";
        //                            rp42s.Append(runFonts);
        //                            rr42.Append(rp42s);
        //                            rr42.Append(new Text(row["Quantity"].ToString()));
        //                            pd42ss.Append(rr42);

        //                            col42ss.Append(pd42ss);
        //                            //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //                            row4s.Append(col42ss);
        //                            tr.TableCell col43ss = new tr.TableCell();
        //                            tr.Paragraph pd43ss = new tr.Paragraph();
        //                            ParagraphProperties paragraphProperties43ss33ss = new ParagraphProperties(
        //                                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                                          new SpacingBetweenLines() { After = "0" },
        //                                                          new Justification() { Val = JustificationValues.Center }
        //                                                          );
        //                            pd43ss.Append(paragraphProperties43ss33ss);
        //                            Run rr43 = new tr.Run();
        //                            RunProperties rp43s = new RunProperties();
        //                            rp43s.FontSize = new tr.FontSize() { Val = "18" };
        //                            //rp43s.Bold = new Bold();
        //                            RunFonts runFontss = new RunFonts();           // Create font
        //                            runFontss.Ascii = "Arial";
        //                            rp43s.Append(runFontss);
        //                            rr43.Append(rp43s);
        //                            rr43.Append(new Text(row["Rate"].ToString()));
        //                            pd43ss.Append(rr43);

        //                            col43ss.Append(pd43ss);
        //                           // col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //                            row4s.Append(col43ss);

        //                            tr.TableCell col44ss = new tr.TableCell();
        //                            tr.Paragraph pd44ss = new tr.Paragraph();
        //                            ParagraphProperties paragraphProperties44ss33ss = new ParagraphProperties(
        //                                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                                          new SpacingBetweenLines() { After = "0" },
        //                                                          new Justification() { Val = JustificationValues.Center }
        //                                                          );
        //                            pd44ss.Append(paragraphProperties44ss33ss);
        //                            Run rr44 = new tr.Run();
        //                            RunProperties rp44s = new RunProperties();
        //                            rp44s.FontSize = new tr.FontSize() { Val = "18" };
        //                            //rp44s.Bold = new Bold();
        //                            RunFonts runFont1s = new RunFonts();           // Create font
        //                            runFont1s.Ascii = "Arial";
        //                            rp44s.Append(runFont1s);
        //                            rr44.Append(rp44s);
        //                            rr44.Append(new Text(row["Amount"].ToString()));
        //                            pd44ss.Append(rr44);

        //                            col44ss.Append(pd44ss);
        //                            //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
        //                            row4s.Append(col44ss);

        //                            table.Append(row4s);

        //                            coll = "";
        //                        }
        //                    }
        //            }
        //        }



        //        tr.TableRow row1sz = new tr.TableRow();
        //        tr.TableCell col1sz = new tr.TableCell();
        //        tr.TableCell col2szg = new tr.TableCell();
        //        tr.TableCell col2sz = new tr.TableCell();
        //        tr.TableCell col3sz = new tr.TableCell();
        //        tr.TableCell col4sz = new tr.TableCell();
        //        col1sz.Append(new Paragraph(new Run(new Text(""))));
        //        row1sz.Append(col1sz);
        //        col2szg.Append(new Paragraph(new Run(new Text(""))));
        //        row1sz.Append(col2szg);
        //        col2sz.Append(new Paragraph(new Run(new Text(""))));
        //        row1sz.Append(col2sz);
        //        col3sz.Append(new Paragraph(new Run(new Text(""))));
        //        row1sz.Append(col3sz);
        //        col4sz.Append(new Paragraph(new Run(new Text(""))));
        //        row1sz.Append(col4sz);
        //        table.Append(row1sz);

        //        tr.TableRow row5s = new tr.TableRow();
        //        tr.TableCell col5s1 = new tr.TableCell();
        //        Run run5ss = new tr.Run();
        //        Paragraph p5ss = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss5sss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "0", Before = "0", AfterLines = 0 },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p5ss.Append(paragraphProperties44ss5sss);
        //        RunProperties rp5ss = new RunProperties();
        //        rp5ss.FontSize = new tr.FontSize() { Val = "18" };
        //        rp5ss.Bold = new Bold();
        //        RunFonts runFont5ss = new RunFonts();           // Create font
        //        runFont5ss.Ascii = "Arial";
        //        rp5ss.Append(runFont5ss);
        //        run5ss.Append(rp5ss);
        //        run5ss.Append(new Text("") { Space = SpaceProcessingModeValues.Preserve });
        //        p5ss.Append(run5ss);

        //        col5s1.Append(p5ss);
        //        row5s.Append(col5s1);
        //        tr.TableCell col5s2 = new tr.TableCell();
        //        tr.TableCell col5s3 = new tr.TableCell();
        //        tr.TableCell col5s4 = new tr.TableCell();
        //        col5s2.Append(new Paragraph(new Run(new Text(""))));
        //        row5s.Append(col5s2);
        //        col5s3.Append(new Paragraph(new Run(new Text(""))));
        //        row5s.Append(col5s3);
        //        Paragraph p54ss = new Paragraph();
        //        ParagraphProperties paragraphProperties54ss5sss = new ParagraphProperties(
        //                                           new ParagraphStyleId() { Val = "No Spacing" },
        //                                           new SpacingBetweenLines() { After = "0", Before = "0" },
        //                                           new Justification() { Val = JustificationValues.Center }
        //                                           );
        //        p54ss.Append(paragraphProperties54ss5sss);
        //        p54ss.Append(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve }));
        //        col5s4.Append(p54ss);
        //        row5s.Append(col5s4);

        //        //table.Append(row5s);

        //        tr.TableRow row5 = new tr.TableRow();
        //        tr.TableCell col51 = new tr.TableCell();
        //        Run run5s = new tr.Run();
        //        Paragraph p5s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss5ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "4", Before = "0", AfterLines = 1 },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p5s.Append(paragraphProperties44ss5ss);
        //        RunProperties rp5s = new RunProperties();
        //        rp5s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp5s.Bold = new Bold();
        //        RunFonts runFont5s = new RunFonts();           // Create font
        //        runFont5s.Ascii = "Arial";
        //        rp5s.Append(runFont5s);
        //        run5s.Append(rp5s);
        //        run5s.Append(new Text("FOB " + dtt.Tables[0].Rows[0]["PrtLoding"].ToString() + " : ") { Space = SpaceProcessingModeValues.Preserve });
        //        p5s.Append(run5s);

        //        col51.Append(p5s);
        //        row5.Append(col51);
        //        tr.TableCell col52b = new tr.TableCell();
        //        tr.TableCell col52 = new tr.TableCell();
        //        tr.TableCell col53 = new tr.TableCell();
        //        tr.TableCell col54 = new tr.TableCell();
        //        col52b.Append(new Paragraph(new Run(new Text(""))));
        //        row5.Append(col52b);
        //        col52.Append(new Paragraph(new Run(new Text(""))));
        //        row5.Append(col52);
        //        col53.Append(new Paragraph(new Run(new Text(""))));
        //        row5.Append(col53);
        //        Paragraph p54s = new Paragraph();
        //        Run run6sz = new tr.Run();
        //        RunProperties rp6sz = new RunProperties();
        //        RunFonts runFont6sz = new RunFonts();
        //        ParagraphProperties paragraphProperties54ss5ss = new ParagraphProperties(
        //                                           new ParagraphStyleId() { Val = "No Spacing" },
        //                                           new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                           new Justification() { Val = JustificationValues.Center }
        //                                           );
        //        p54s.Append(paragraphProperties54ss5ss);
        //        rp6sz.FontSize = new tr.FontSize() { Val = "18" };
        //        rp6sz.Bold = new Bold();
        //        runFont6sz.Ascii = "Arial";
        //        rp6sz.Append(runFont6sz);
        //        run6sz.Append(rp6sz);
        //        run6sz.Append(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
        //        p54s.Append(run6sz);
        //        //p54s.Append(new Run(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve }));
        //        col54.Append(p54s);
        //        row5.Append(col54);

        //        table.Append(row5);

        //        tr.TableRow row6 = new tr.TableRow();
        //        tr.TableCell col61 = new tr.TableCell();
        //        Run run6s = new tr.Run();
        //        Paragraph p6s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss6ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p6s.Append(paragraphProperties44ss6ss);
        //        RunProperties rp6s = new RunProperties();
        //        rp6s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp6s.Bold = new Bold();
        //        RunFonts runFont6s = new RunFonts();           // Create font
        //        runFont6s.Ascii = "Arial";
        //        rp6s.Append(runFont6s);
        //        run6s.Append(rp6s);
        //        run6s.Append(new Text("HSN CODE /RITC HS CODE : ") { Space = SpaceProcessingModeValues.Preserve });
        //        p6s.Append(run6s);

        //        col61.Append(p6s);
        //        row6.Append(col61);
        //        tr.TableCell col62v = new tr.TableCell();
        //        tr.TableCell col62 = new tr.TableCell();
        //        tr.TableCell col63 = new tr.TableCell();
        //        tr.TableCell col64 = new tr.TableCell(); ;
        //        col62v.Append(new Paragraph(new Run(new Text(""))));
        //        row6.Append(col62v);
        //        col62.Append(new Paragraph(new Run(new Text(""))));
        //        row6.Append(col62);
        //        col63.Append(new Paragraph(new Run(new Text(""))));
        //        row6.Append(col63);
        //        col64.Append(new Paragraph(new Run(new Text(""))));
        //        row6.Append(col64);

        //        //table.Append(row6);

        //        tr.TableRow row7 = new tr.TableRow();
        //        tr.TableCell col71 = new tr.TableCell();
        //        Run run7s = new tr.Run();
        //        Paragraph p7s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss7ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p7s.Append(paragraphProperties44ss7ss);
        //        RunProperties rp7s = new RunProperties();
        //        rp7s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp7s.Bold = new Bold();
        //        RunFonts runFont7s = new RunFonts();           // Create font
        //        runFont7s.Ascii = "Arial";
        //        rp7s.Append(runFont7s);
        //        run7s.Append(rp7s);
        //        run7s.Append(new Text(dtt.Tables[0].Rows[0]["PlcDlvry"].ToString() + ":"));
        //        p7s.Append(run7s);

        //        col71.Append(p7s);
        //        row7.Append(col71);
        //        tr.TableCell col72c = new tr.TableCell();
        //        tr.TableCell col72 = new tr.TableCell();
        //        tr.TableCell col73 = new tr.TableCell();
        //        tr.TableCell col74 = new tr.TableCell();
        //        col72c.Append(new Paragraph(new Run(new Text(""))));
        //        row7.Append(col72c);
        //        col72.Append(new Paragraph(new Run(new Text(""))));
        //        row7.Append(col72);
        //        col73.Append(new Paragraph(new Run(new Text(""))));
        //        row7.Append(col73);
        //        Paragraph p2u4 = new tr.Paragraph();
        //        Run R2u4 = new tr.Run();
        //        RunProperties rp6szr = new RunProperties();
        //        RunFonts runFont6szr = new RunFonts();
        //        ParagraphProperties paragraphProperties54ss5ssr = new ParagraphProperties(
        //                                           new ParagraphStyleId() { Val = "No Spacing" },
        //                                           new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                           new Justification() { Val = JustificationValues.Center }
        //                                           );
        //        p2u4.Append(paragraphProperties54ss5ssr);
        //        rp6szr.FontSize = new tr.FontSize() { Val = "18" };
        //        rp6szr.Bold = new Bold();
        //        runFont6szr.Ascii = "Arial";
        //        rp6szr.Append(runFont6szr);
        //        R2u4.Append(rp6szr);

        //        R2u4.Append(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
        //        p2u4.Append(R2u4);
        //        col74.Append(p2u4);
        //        row7.Append(col74);
        //        //table.Append(row7);

        //        tr.TableRow row8 = new tr.TableRow();
        //        tr.TableCell col81 = new tr.TableCell();
        //        Run run8s = new tr.Run();
        //        Paragraph p8s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss8ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p8s.Append(paragraphProperties44ss8ss);
        //        RunProperties rp8s = new RunProperties();
        //        rp8s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp8s.Bold = new Bold();
        //        RunFonts runFont8s = new RunFonts();           // Create font
        //        runFont8s.Ascii = "Arial";
        //        rp8s.Append(runFont8s);
        //        run8s.Append(rp8s);
        //        run8s.Append(new Text(dtt.Tables[0].Rows[0]["ShipmentMode"].ToString() + " : "));
        //        p8s.Append(run8s);

        //        col81.Append(p8s);
        //        row8.Append(col81);
        //        tr.TableCell col82j = new tr.TableCell();
        //        tr.TableCell col82 = new tr.TableCell();
        //        tr.TableCell col83 = new tr.TableCell();
        //        tr.TableCell col84 = new tr.TableCell();
        //        Paragraph p10ssrr = new tr.Paragraph();
        //        col82j.Append(new Paragraph(new Run(new Text(""))));
        //        row8.Append(col82j);
        //        col82.Append(new Paragraph(new Run(new Text(""))));
        //        row8.Append(col82);
        //        col83.Append(new Paragraph(new Run(new Text(""))));
        //        row8.Append(col83);
        //        Run r789d = new tr.Run();
        //        r789d.Append(new Text(".00"));
        //        r789d.RunProperties = new tr.RunProperties((new Bold()));
        //        p10ssrr.AppendChild(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }));
        //        p10ssrr.Append(r789d);
        //        col84.Append(p10ssrr);
        //        row8.Append(col84);

        //        table.Append(row8);

        //        tr.TableRow row9 = new tr.TableRow();
        //        tr.TableCell col91 = new tr.TableCell();
        //        Run run9s = new tr.Run();
        //        Paragraph p9s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss9ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "2", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p9s.Append(paragraphProperties44ss9ss);
        //        RunProperties rp9s = new RunProperties();
        //        rp9s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp9s.Bold = new Bold();
        //        RunFonts runFont9s = new RunFonts();           // Create font
        //        runFont9s.Ascii = "Arial";
        //        rp9s.Append(runFont9s);
        //        run9s.Append(rp9s);
        //        run9s.Append(new Text(dtt.Tables[0].Rows[0]["InCoTerms"].ToString() == "" ? "" : dtt.Tables[0].Rows[0]["InCoTerms"].ToString() + " : "));
        //        p9s.Append(run9s);

        //        col91.Append(p9s);
        //        row9.Append(col91);
        //        tr.TableCell col92d = new tr.TableCell();
        //        tr.TableCell col92 = new tr.TableCell();
        //        tr.TableCell col93 = new tr.TableCell();
        //        tr.TableCell col94 = new tr.TableCell();
        //        col92d.Append(new Paragraph(new Run(new Text(""))));
        //        row9.Append(col92d);
        //        col92.Append(new Paragraph(new Run(new Text(""))));
        //        row9.Append(col92);
        //        col93.Append(new Paragraph(new Run(new Text(""))));
        //        row9.Append(col93);
        //        col94.Append(new Paragraph(new Run(new Text(""))));
        //        row9.Append(col94);
        //        if (dtt.Tables[0].Rows[0]["InCoTerms"].ToString() != "")
        //            table.Append(row9);

        //        tr.TableRow row10 = new tr.TableRow();
        //        tr.TableCell col101 = new tr.TableCell();
        //        Run run10s = new tr.Run();
        //        Paragraph p10s = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss10ss = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "2", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p10s.Append(paragraphProperties44ss10ss);
        //        RunProperties rp10s = new RunProperties();
        //        rp10s.FontSize = new tr.FontSize() { Val = "18" };
        //        rp10s.Bold = new Bold();
        //        RunFonts runFont10s = new RunFonts();           // Create font
        //        runFont10s.Ascii = "Arial";
        //        rp10s.Append(runFont10s);
        //        run10s.Append(rp10s);
        //        run10s.Append(new Text("ADD INTEGRATED TAX @ 0.1%: "));
        //        p10s.Append(run10s);

        //        col101.Append(p10s);
        //        row10.Append(col101);
        //        tr.TableCell col102f = new tr.TableCell();
        //        tr.TableCell col102 = new tr.TableCell();
        //        tr.TableCell col103 = new tr.TableCell();
        //        tr.TableCell col104 = new tr.TableCell();
        //        col102f.Append(new Paragraph(new Run(new Text(""))));
        //        row10.Append(col102f);
        //        col102.Append(new Paragraph(new Run(new Text(""))));
        //        row10.Append(col102);
        //        col103.Append(new Paragraph(new Run(new Text(""))));
        //        row10.Append(col103);
        //        tr.Paragraph p10ss = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties54ss10ss = new ParagraphProperties(
        //                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                          new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                          new Justification() { Val = JustificationValues.Center }
        //                                          );
        //        p10ss.Append(paragraphProperties54ss10ss);
        //        Run r789 = new tr.Run();
        //        r789.Append(new Text("0.00"));
        //        r789.RunProperties = new tr.RunProperties((new Bold()));
        //        p10ss.Append(r789);
        //        col104.Append(p10ss);
        //        row10.Append(col104);

        //        table.Append(row10);

        //        table.Append(row7);
        //        tr.TableRow row11g = new tr.TableRow();
        //        tr.TableCell col1011g = new tr.TableCell();
        //        Run run10s1g = new tr.Run();
        //        Paragraph p10s1g = new Paragraph();
        //        ParagraphProperties paragraphProperties44ss10ss1g = new ParagraphProperties(
        //                                               new ParagraphStyleId() { Val = "No Spacing" },
        //                                               new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                               new Justification() { Val = JustificationValues.Right }
        //                                               );
        //        p10s1g.Append(paragraphProperties44ss10ss1g);
        //        RunProperties rp10s1g = new RunProperties();
        //        rp10s1g.FontSize = new tr.FontSize() { Val = "18" };
        //        rp10s1g.Bold = new Bold();
        //        RunFonts runFont10s1g = new RunFonts();           // Create font
        //        runFont10s1g.Ascii = "Arial";
        //        rp10s1g.Append(runFont10s1g);
        //        run10s1g.Append(rp10s1g);
        //        run10s1g.Append(new Text("GST PAYABLE UNDER REVERSE CHARGE:"));
        //        p10s1g.Append(run10s1g);

        //        col1011g.Append(p10s1g);
        //        row11g.Append(col1011g);
        //        tr.TableCell col1021gd = new tr.TableCell();
        //        tr.TableCell col1021g = new tr.TableCell();
        //        tr.TableCell col1031g = new tr.TableCell();
        //        tr.TableCell col1041g = new tr.TableCell();
        //        col1021gd.Append(new Paragraph(new Run(new Text(""))));
        //        row11g.Append(col1021gd);
        //        col1021g.Append(new Paragraph(new Run(new Text(""))));
        //        row11g.Append(col1021g);
        //        col1031g.Append(new Paragraph(new Run(new Text(""))));
        //        row11g.Append(col1031g);
        //        tr.Paragraph p10ss1g = new tr.Paragraph();
        //        ParagraphProperties paragraphProperties54ss10ss1g = new ParagraphProperties(
        //                                          new ParagraphStyleId() { Val = "No Spacing" },
        //                                          new SpacingBetweenLines() { After = "4", Before = "0" },
        //                                          new Justification() { Val = JustificationValues.Center }
        //                                          );
        //        p10ss1g.Append(paragraphProperties54ss10ss1g);
        //        Run r789g = new tr.Run();
        //        r789g.Append(new Text("0.00"));
        //        r789g.RunProperties = new tr.RunProperties((new Bold()));
        //        p10ss1g.Append(r789g);
        //        //p10ss1g.Append(new Run(new Text("0.00")));
        //        col1041g.Append(p10ss1g);
        //        row11g.Append(col1041g);

        //        table.Append(row11g);


        //        tr.TableRow tr1 = new tr.TableRow();
        //        TableCellProperties tableCellProperties = new TableCellProperties();
        //        HorizontalMerge verticalMerge = new HorizontalMerge()
        //        {
        //            Val = MergedCellValues.Restart
        //        };
        //        tableCellProperties.Append(verticalMerge);
        //        TableCellProperties tableCellProperties1 = new TableCellProperties();
        //        HorizontalMerge verticalMerge1 = new HorizontalMerge()
        //        {
        //            Val = MergedCellValues.Continue
        //        };
        //        tableCellProperties1.Append(verticalMerge1);
        //        tr.TableCell tc11 = new tr.TableCell();
        //        Paragraph p11 = new Paragraph();
        //        Run r12 = new Run();
        //        RunProperties rp12 = new RunProperties();
        //        rp12.Bold = new Bold();
        //        rp12.FontSize = new tr.FontSize() { Val = "20" };
        //        ParagraphProperties pp11 = new ParagraphProperties();
        //        pp11.Justification = new Justification() { Val = JustificationValues.Center };
        //        ParagraphProperties paragraphProperties11 = new ParagraphProperties(
        //                                      new ParagraphStyleId() { Val = "No Spacing" },
        //                                      new SpacingBetweenLines() { After = "0" }
        //                                      );
        //        p11.Append(paragraphProperties11);
        //        p11.Append(pp11);
        //        r12.Append(rp12);
        //        r12.Append(new Text("In Words :" + Session["TotalAmt"].ToString() + ""));
        //        p11.Append(r12);
        //        tc11.Append(p11);
        //        tr1.Append(tc11);
        //        //TableRowProperties tableRowProperties1 = new TableRowProperties();
        //        //TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)28U };

        //        //tableRowProperties1.Append(tableRowHeight1);
        //        //tr1.InsertBefore(tableRowProperties1, tc11);
        //        tr.TableCell tc12d = new tr.TableCell();
        //        Paragraph p12d = new Paragraph();

        //        tc12d.Append(p12d);
        //        tr1.Append(tc12d);
        //        tr.TableCell tc12 = new tr.TableCell();
        //        Paragraph p12 = new Paragraph();

        //        tc12.Append(p12);
        //        tr1.Append(tc12);
        //        tr.TableCell tc13 = new tr.TableCell();
        //        Paragraph p13 = new Paragraph();
        //        tc13.Append(p13);
        //        tr1.Append(tc13);
        //        tr.TableCell tc14 = new tr.TableCell();
        //        Paragraph p14 = new Paragraph();

        //        tc14.Append(p14);
        //        tr1.Append(tc14);
        //        table.Append(tr1);

        //        body.Append(table);
        //        mainPart.Document.Body = body;
        //        //PageSize pageSize1 = new PageSize() { Width = (UInt32Value)11906U, Height = (UInt32Value)16838U };
        //        //IEnumerable<SectionProperties> sectionProperties1 = mainPart.Document.Body.Elements<SectionProperties>();
        //        //if (sectionProperties1 == null)
        //        //{
        //        //    sectionProperties1.Append(pageSize1);
        //        //    sectionProperties1 = new SectionProperties() { };
        //        //    mainPart.Document.Body.A ppend(sectionProperties1);
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrMsg = ex.Message;
        //        int LineNo = ExceptionHelper.LineNumber(ex);
        //        ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
        //        ELog.CreateErrorLog(Server.MapPath("../Logs/Others/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
        //    }
        //}
        #endregion
        private void GenerateBodyPart(MainDocumentPart mainPart, Body body)
        {
            try
            {
                Paragraph p = new Paragraph();
                Run r = new Run();
                Text t = new Text("Hello Body");
                tr.Table table = new tr.Table();
                DataSet dtt = (DataSet)Session["DtComsInv"];
                DataSet dtt1 = (DataSet)Session["DtComsInvset"];
                SetTableStyle(table, "6000", true, true, true, true, false, true);
                tr.TableRow row91s = new tr.TableRow();
                tr.TableCell col91s = new tr.TableCell();
                tr.Paragraph pd91s = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd91s.Append(paragraphProperties91ss3s);
                Run rr91 = new tr.Run();
                RunProperties rp91s = new RunProperties();
                rp91s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_1 = new RunFonts();           // Create font
                runFont_1.Ascii = "Arial";
                rp91s.Append(runFont_1);
                rp91s.Bold = new Bold();
                rr91.Append(rp91s);
                rr91.Append(new Text(""));
                // rr91.Append(new Text(dtt.Tables[0].Rows[0]["CustNm"].ToString() + "             No. of Pkgs.  " + dtt.Tables[0].Rows[0]["TotalPkgs"].ToString() + "No(s)") { Space = SpaceProcessingModeValues.Preserve });
                pd91s.Append(rr91);

                col91s.Append(pd91s);
                col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "1032" }));
                row91s.Append(col91s);

                tr.TableCell col92ssz = new tr.TableCell();
                tr.Paragraph pd92ssz = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ssz = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ssz.Append(paragraphProperties92ss33ssz);
                Run rr92z = new tr.Run();
                RunProperties rp92sz = new RunProperties();
                rp92sz.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_2 = new RunFonts();           // Create font
                runFont_2.Ascii = "Arial";
                rp92sz.Append(runFont_2);
                rp92sz.Bold = new Bold();
                rr92z.Append(rp92sz);
                rr92z.Append(new Text(" "));
                pd92ssz.Append(rr92z);

                col92ssz.Append(pd92ssz);
                col92ssz.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "282" }));
                row91s.Append(col92ssz);


                tr.TableCell col92ss = new tr.TableCell();
                tr.Paragraph pd92ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ss.Append(paragraphProperties92ss33ss);
                Run rr92 = new tr.Run();
                RunProperties rp92s = new RunProperties();
                rp92s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_3 = new RunFonts();           // Create font
                runFont_3.Ascii = "Arial";
                rp92s.Append(runFont_3);
                rp92s.Bold = new Bold();
                rr92.Append(rp92s);
                rr92.Append(new Text(" "));
                pd92ss.Append(rr92);

                col92ss.Append(pd92ss);
                col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "230" }));
                row91s.Append(col92ss);

                tr.TableCell col93ss = new tr.TableCell();
                tr.Paragraph pd93ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties93ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd93ss.Append(paragraphProperties93ss33ss);
                Run rr93 = new tr.Run();
                RunProperties rp93s = new RunProperties();
                rp93s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_4 = new RunFonts();           // Create font
                runFont_4.Ascii = "Arial";
                rp93s.Append(runFont_4);
                rp93s.Bold = new Bold();
                rr93.Append(rp93s);
                rr93.Append(new Text(" "));
                pd93ss.Append(rr93);

                col93ss.Append(pd93ss);
                col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "205" }));
                row91s.Append(col93ss);

                tr.TableCell col94ss = new tr.TableCell();
                tr.Paragraph pd94ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties94ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd94ss.Append(paragraphProperties94ss33ss);
                Run rr94 = new tr.Run();
                RunProperties rp94s = new RunProperties();
                rp94s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_5 = new RunFonts();           // Create font
                runFont_5.Ascii = "Arial";
                rp94s.Append(runFont_5);
                rp94s.Bold = new Bold();
                rr94.Append(rp94s);
                rr94.Append(new Text(" "));
                pd94ss.Append(rr94);

                col94ss.Append(pd94ss);
                col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "269" }));
                row91s.Append(col94ss);

                // table.Append(row91s);

                tr.TableRow row2s = new tr.TableRow();
                tr.TableCell col21s = new tr.TableCell();
                tr.Paragraph pd21s = new tr.Paragraph();
                ParagraphProperties paragraphProperties21ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd21s.Append(paragraphProperties21ss3s);
                Run rr21 = new tr.Run();
                RunProperties rp21s = new RunProperties();
                rp21s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_6 = new RunFonts();           // Create font
                runFont_6.Ascii = "Arial";
                rp21s.Append(runFont_6);
                rp21s.Bold = new Bold();
                rr21.Append(rp21s);
                rr21.Append(new Text(""));
                // rr21.Append(new Text(dtt.Tables[0].Rows[0]["PlcFnlDstn"].ToString() + "             No." + dtt.Tables[0].Rows[0]["TotalPkgsToFrm"].ToString()) { Space = SpaceProcessingModeValues.Preserve });
                pd21s.Append(rr21);

                col21s.Append(pd21s);
                //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
                row2s.Append(col21s);

                tr.TableCell col22ssf = new tr.TableCell();
                tr.Paragraph pd22ssf = new tr.Paragraph();
                ParagraphProperties paragraphProperties22ss33ssf = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd22ssf.Append(paragraphProperties22ss33ssf);
                Run rr22f = new tr.Run();
                RunProperties rp22sf = new RunProperties();
                rp22sf.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_7 = new RunFonts();           // Create font
                runFont_7.Ascii = "Arial";
                rp22sf.Append(runFont_7);
                rp22sf.Bold = new Bold();
                rr22f.Append(rp22sf);
                rr22f.Append(new Text(" "));
                pd22ssf.Append(rr22f);

                col22ssf.Append(pd22ssf);
                //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                row2s.Append(col22ssf);

                tr.TableCell col22ss = new tr.TableCell();
                tr.Paragraph pd22ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties22ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd22ss.Append(paragraphProperties22ss33ss);
                Run rr22 = new tr.Run();
                RunProperties rp22s = new RunProperties();
                rp22s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_8 = new RunFonts();           // Create font
                runFont_8.Ascii = "Arial";
                rp22s.Append(runFont_8);
                rp22s.Bold = new Bold();
                rr22.Append(rp22s);
                rr22.Append(new Text(" "));
                pd22ss.Append(rr22);

                col22ss.Append(pd22ss);
                //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                row2s.Append(col22ss);


                tr.TableCell col23ss = new tr.TableCell();
                tr.Paragraph pd23ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties23ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd23ss.Append(paragraphProperties23ss33ss);
                Run rr23 = new tr.Run();
                RunProperties rp23s = new RunProperties();
                rp23s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_9 = new RunFonts();           // Create font
                runFont_9.Ascii = "Arial";
                rp23s.Append(runFont_9);
                rp23s.Bold = new Bold();
                rr23.Append(rp23s);
                rr23.Append(new Text(" "));
                pd23ss.Append(rr23);

                col23ss.Append(pd23ss);
                //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                row2s.Append(col23ss);

                tr.TableCell col24ss = new tr.TableCell();
                tr.Paragraph pd24ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties24ss33ss = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd24ss.Append(paragraphProperties24ss33ss);
                Run rr24 = new tr.Run();
                RunProperties rp24s = new RunProperties();
                rp24s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_10 = new RunFonts();           // Create font
                runFont_10.Ascii = "Arial";
                rp24s.Append(runFont_10);
                rp24s.Bold = new Bold();
                rr24.Append(rp24s);
                rr24.Append(new Text(" "));
                pd24ss.Append(rr24);

                col24ss.Append(pd24ss);
                //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                row2s.Append(col24ss);

                //  table.Append(row2s);

                int rowIndex = 1;
                string coltext = string.Empty;
                string coll = string.Empty;

                var kk = (dtt1.Tables[0].Rows[0]["FPOs"].ToString().Split(','));
                //var kkk = dtt.Tables[0].Select().OrderBy(k => k.ItemArray[15].ToString()).ToList();
                List<Tuple<string, string>> getFPONOS = new List<Tuple<string, string>>();
                string gettupl = string.Empty; string getguid = string.Empty; string getSno = string.Empty;
                foreach (DataRow getval in dtt.Tables[0].Rows)
                {
                    if (getval.ItemArray[22].ToString() != getguid || getval.ItemArray[21].ToString() != getSno)
                    {
                        getguid = getval.ItemArray[22].ToString();
                        getSno = getval.ItemArray[21].ToString();
                        gettupl += getval.ItemArray[22].ToString() + ",";
                        getFPONOS.Add(Tuple.Create(getval.ItemArray[21].ToString(), getval.ItemArray[22].ToString()));
                    }
                }
                Array.Sort(kk);
                int rind = 1;
                foreach (var arr in (getFPONOS))
                {
                    rowIndex = 1;

                    tr.TableRow row1z = new tr.TableRow();
                    tr.TableCell col1zd = new tr.TableCell();
                    tr.TableCell col1z = new tr.TableCell();
                    tr.TableCell col2z = new tr.TableCell();
                    tr.TableCell col3z = new tr.TableCell();
                    tr.TableCell col4z = new tr.TableCell();
                    col1zd.Append(new Paragraph(new Run(new Text(""))));
                    row1z.Append(col1zd);
                    col1z.Append(new Paragraph(new Run(new Text(""))));
                    row1z.Append(col1z);
                    col2z.Append(new Paragraph(new Run(new Text(""))));
                    row1z.Append(col2z);
                    col3z.Append(new Paragraph(new Run(new Text(""))));
                    row1z.Append(col3z);
                    col4z.Append(new Paragraph(new Run(new Text(""))));
                    row1z.Append(col4z);
                    if (rind != 1)
                        table.Append(row1z);
                    rind++;
                    tr.TableRow row1 = new tr.TableRow();
                    tr.TableCell col1 = new tr.TableCell();
                    Run run1s = new tr.Run();
                    Paragraph p1s = new Paragraph();
                    ParagraphProperties paragraphProperties1s = new ParagraphProperties(
                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                  new SpacingBetweenLines() { After = "0", Before = "0" },
                                                  new ContextualSpacing() { Val = false }
                                                  );
                    TableCellProperties cellPropertiess1 = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                    RunProperties rp1s = new RunProperties();

                    rp1s.Bold = new Bold();
                    rp1s.FontSize = new tr.FontSize() { Val = "18" };
                    RunFonts runFont_12 = new RunFonts();           // Create font
                    runFont_12.Ascii = "Arial";
                    rp1s.Append(runFont_12);
                    run1s.Append(rp1s);
                    var dt = dtt.Tables[0].Select("ForeignPOId = '" + arr.Item2 + "' and FPOSNo = '" + arr.Item1 + "'").CopyToDataTable();

                    // var dt = dtt.Tables[0].Select("ForeignPOId = '" + arr.Item2 + "'" ).CopyToDataTable();

                    coltext = "  " + dt.Rows[0]["FPOSubHeading"].ToString();

                    run1s.AppendChild(new Text(coltext) { Space = SpaceProcessingModeValues.Preserve });
                    p1s.Append(paragraphProperties1s);



                    col1.AppendChild(cellPropertiess1);
                    p1s.Append(run1s);
                    col1.Append(p1s);
                    row1.Append(col1);
                    tr.TableCell col2e = new tr.TableCell();
                    tr.TableCell col2 = new tr.TableCell();
                    tr.TableCell col3 = new tr.TableCell();
                    tr.TableCell col4 = new tr.TableCell();
                    col2e.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col2e);
                    col2.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col2);
                    col3.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col3);
                    col4.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col4);

                    table.Append(row1);
                    foreach (DataRow row in dtt.Tables[0].Rows)
                    {
                        if (arr.Item2 == row["ForeignPOId"].ToString() && arr.Item1 == row["FPOSNo"].ToString())

                            if (rowIndex == 1 && coltext.ToString().Trim() != row["FPOSubHeading"].ToString().Trim())
                            {
                                rowIndex = 1;
                            }
                            else
                            {
                                if (arr.Item2 == row["ForeignPOId"].ToString() && arr.Item1 == row["FPOSNo"].ToString())
                                {
                                    rowIndex = 2;
                                    tr.TableRow row4s = new tr.TableRow();
                                    tr.TableCell col4s = new tr.TableCell();
                                    tr.Paragraph pd4s = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties4ss3s = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0", Before = "0", Line = "0", LineRule = LineSpacingRuleValues.Exact },
                                                                  new Justification() { Val = JustificationValues.Left },
                                                                  new ContextualSpacing() { Val = false }
                                                                  );
                                    pd4s.Append(paragraphProperties4ss3s);
                                    Run rr4 = new tr.Run();
                                    RunProperties rp4s = new RunProperties();
                                    rp4s.FontSize = new tr.FontSize() { Val = "18" };
                                    RunFonts runFont = new RunFonts();           // Create font
                                    runFont.Ascii = "Arial";
                                    rp4s.Append(runFont);
                                    rr4.Append(rp4s);
                                    coll += "  " + row["Description"].ToString();
                                    if (row["Spec_Make"].ToString() != "")
                                        coll += "," + row["Spec_Make"].ToString();
                                    if (row["PartNumber"].ToString() != "")
                                        coll += "," + row["PartNumber"].ToString();

                                    rr4.Append(new Text(coll) { Space = SpaceProcessingModeValues.Preserve });
                                    pd4s.Append(rr4);

                                    col4s.Append(pd4s);
                                    col4s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "1029" }));
                                    //col4s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2550" }));
                                    //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
                                    row4s.Append(col4s);

                                    tr.TableCell col42ssh = new tr.TableCell();
                                    tr.Paragraph pd42ssh = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties42ss33ssh = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0" },
                                                                  new Justification() { Val = JustificationValues.Center }
                                                                  );
                                    pd42ssh.Append(paragraphProperties42ss33ssh);
                                    Run rr42h = new tr.Run();
                                    RunProperties rp42sh = new RunProperties();
                                    rp42sh.FontSize = new tr.FontSize() { Val = "18" };
                                    RunFonts runFontsh = new RunFonts();           // Create font
                                    runFontsh.Ascii = "Arial";
                                    rp42sh.Append(runFontsh);
                                    rr42h.Append(rp42sh);
                                    rr42h.Append(new Text(row["HSCode"].ToString()));
                                    pd42ssh.Append(rr42h);

                                    col42ssh.Append(pd42ssh);
                                    col42ssh.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "277" }));
                                    //col42ssh.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "260" }));
                                    row4s.Append(col42ssh);

                                    tr.TableCell col42ss = new tr.TableCell();
                                    tr.Paragraph pd42ss = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties42ss33ss = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0" },
                                                                  new Justification() { Val = JustificationValues.Center }
                                                                  );
                                    pd42ss.Append(paragraphProperties42ss33ss);
                                    Run rr42 = new tr.Run();
                                    RunProperties rp42s = new RunProperties();
                                    rp42s.FontSize = new tr.FontSize() { Val = "18" };
                                    RunFonts runFonts = new RunFonts();           // Create font
                                    runFonts.Ascii = "Arial";
                                    rp42s.Append(runFonts);
                                    rr42.Append(rp42s);
                                    rr42.Append(new Text(row["Quantity"].ToString()));
                                    pd42ss.Append(rr42);

                                    col42ss.Append(pd42ss);
                                    col42ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "230" }));
                                    // col42ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "240" }));
                                    row4s.Append(col42ss);

                                    tr.TableCell col43ss = new tr.TableCell();
                                    tr.Paragraph pd43ss = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties43ss33ss = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0" },
                                                                  new Justification() { Val = JustificationValues.Center }
                                                                  );
                                    pd43ss.Append(paragraphProperties43ss33ss);
                                    Run rr43 = new tr.Run();
                                    RunProperties rp43s = new RunProperties();
                                    rp43s.FontSize = new tr.FontSize() { Val = "18" };
                                    //rp43s.Bold = new Bold();
                                    RunFonts runFontss = new RunFonts();           // Create font
                                    runFontss.Ascii = "Arial";
                                    rp43s.Append(runFontss);
                                    rr43.Append(rp43s);
                                    rr43.Append(new Text(row["Rate"].ToString()));
                                    pd43ss.Append(rr43);

                                    col43ss.Append(pd43ss);
                                    col43ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "205" }));
                                    //col43ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "480" }));
                                    row4s.Append(col43ss);

                                    tr.TableCell col44ss = new tr.TableCell();
                                    tr.Paragraph pd44ss = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties44ss33ss = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0" },
                                                                  new Justification() { Val = JustificationValues.Center }
                                                                  );
                                    pd44ss.Append(paragraphProperties44ss33ss);
                                    Run rr44 = new tr.Run();
                                    RunProperties rp44s = new RunProperties();
                                    rp44s.FontSize = new tr.FontSize() { Val = "18" };
                                    //rp44s.Bold = new Bold();
                                    RunFonts runFont1s = new RunFonts();           // Create font
                                    runFont1s.Ascii = "Arial";
                                    rp44s.Append(runFont1s);
                                    rr44.Append(rp44s);
                                    rr44.Append(new Text(row["Amount"].ToString()));
                                    pd44ss.Append(rr44);

                                    col44ss.Append(pd44ss);
                                    col44ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "269" }));
                                    //col44ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "145" }));
                                    row4s.Append(col44ss);

                                    table.Append(row4s);

                                    coll = "";
                                }
                            }
                    }
                }



                tr.TableRow row1sz = new tr.TableRow();
                tr.TableCell col1sz = new tr.TableCell();
                tr.TableCell col2szg = new tr.TableCell();
                tr.TableCell col2sz = new tr.TableCell();
                tr.TableCell col3sz = new tr.TableCell();
                tr.TableCell col4sz = new tr.TableCell();
                col1sz.Append(new Paragraph(new Run(new Text(""))));
                row1sz.Append(col1sz);
                col2szg.Append(new Paragraph(new Run(new Text(""))));
                row1sz.Append(col2szg);
                col2sz.Append(new Paragraph(new Run(new Text(""))));
                row1sz.Append(col2sz);
                col3sz.Append(new Paragraph(new Run(new Text(""))));
                row1sz.Append(col3sz);
                col4sz.Append(new Paragraph(new Run(new Text(""))));
                row1sz.Append(col4sz);
                table.Append(row1sz);

                tr.TableRow row5s = new tr.TableRow();
                tr.TableCell col5s1 = new tr.TableCell();
                Run run5ss = new tr.Run();
                Paragraph p5ss = new Paragraph();
                ParagraphProperties paragraphProperties44ss5sss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "0", Before = "0", AfterLines = 0 },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p5ss.Append(paragraphProperties44ss5sss);
                RunProperties rp5ss = new RunProperties();
                rp5ss.FontSize = new tr.FontSize() { Val = "18" };
                rp5ss.Bold = new Bold();
                RunFonts runFont5ss = new RunFonts();           // Create font
                runFont5ss.Ascii = "Arial";
                rp5ss.Append(runFont5ss);
                run5ss.Append(rp5ss);
                run5ss.Append(new Text("") { Space = SpaceProcessingModeValues.Preserve });
                p5ss.Append(run5ss);

                col5s1.Append(p5ss);
                row5s.Append(col5s1);
                tr.TableCell col5s2 = new tr.TableCell();
                tr.TableCell col5s3 = new tr.TableCell();
                tr.TableCell col5s4 = new tr.TableCell();
                col5s2.Append(new Paragraph(new Run(new Text(""))));
                row5s.Append(col5s2);
                col5s3.Append(new Paragraph(new Run(new Text(""))));
                row5s.Append(col5s3);
                Paragraph p54ss = new Paragraph();
                ParagraphProperties paragraphProperties54ss5sss = new ParagraphProperties(
                                                   new ParagraphStyleId() { Val = "No Spacing" },
                                                   new SpacingBetweenLines() { After = "0", Before = "0" },
                                                   new Justification() { Val = JustificationValues.Center }
                                                   );
                p54ss.Append(paragraphProperties54ss5sss);
                p54ss.Append(new Run(new Text("") { Space = SpaceProcessingModeValues.Preserve }));
                col5s4.Append(p54ss);
                row5s.Append(col5s4);

                //table.Append(row5s);

                tr.TableRow row5 = new tr.TableRow();
                tr.TableCell col51 = new tr.TableCell();
                Run run5s = new tr.Run();
                Paragraph p5s = new Paragraph();
                ParagraphProperties paragraphProperties44ss5ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "4", Before = "0", AfterLines = 1 },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p5s.Append(paragraphProperties44ss5ss);
                RunProperties rp5s = new RunProperties();
                rp5s.FontSize = new tr.FontSize() { Val = "18" };
                rp5s.Bold = new Bold();
                RunFonts runFont5s = new RunFonts();           // Create font
                runFont5s.Ascii = "Arial";
                rp5s.Append(runFont5s);
                run5s.Append(rp5s);
                run5s.Append(new Text("FOB " + dtt.Tables[0].Rows[0]["PrtLoding"].ToString() + " : ") { Space = SpaceProcessingModeValues.Preserve });
                p5s.Append(run5s);

                col51.Append(p5s);
                row5.Append(col51);
                tr.TableCell col52b = new tr.TableCell();
                tr.TableCell col52 = new tr.TableCell();
                tr.TableCell col53 = new tr.TableCell();
                tr.TableCell col54 = new tr.TableCell();
                col52b.Append(new Paragraph(new Run(new Text(""))));
                row5.Append(col52b);
                col52.Append(new Paragraph(new Run(new Text(""))));
                row5.Append(col52);
                col53.Append(new Paragraph(new Run(new Text(""))));
                row5.Append(col53);
                Paragraph p54s = new Paragraph();
                Run run6sz = new tr.Run();
                RunProperties rp6sz = new RunProperties();
                RunFonts runFont6sz = new RunFonts();
                ParagraphProperties paragraphProperties54ss5ss = new ParagraphProperties(
                                                   new ParagraphStyleId() { Val = "No Spacing" },
                                                   new SpacingBetweenLines() { After = "4", Before = "0" },
                                                   new Justification() { Val = JustificationValues.Center }
                                                   );
                p54s.Append(paragraphProperties54ss5ss);
                rp6sz.FontSize = new tr.FontSize() { Val = "18" };
                rp6sz.Bold = new Bold();
                runFont6sz.Ascii = "Arial";
                rp6sz.Append(runFont6sz);
                run6sz.Append(rp6sz);
                run6sz.Append(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
                p54s.Append(run6sz);
                //p54s.Append(new Run(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve }));
                col54.Append(p54s);
                row5.Append(col54);

                table.Append(row5);

                tr.TableRow row6 = new tr.TableRow();
                tr.TableCell col61 = new tr.TableCell();
                Run run6s = new tr.Run();
                Paragraph p6s = new Paragraph();
                ParagraphProperties paragraphProperties44ss6ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "4", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p6s.Append(paragraphProperties44ss6ss);
                RunProperties rp6s = new RunProperties();
                rp6s.FontSize = new tr.FontSize() { Val = "18" };
                rp6s.Bold = new Bold();
                RunFonts runFont6s = new RunFonts();           // Create font
                runFont6s.Ascii = "Arial";
                rp6s.Append(runFont6s);
                run6s.Append(rp6s);
                run6s.Append(new Text("HSN CODE /RITC HS CODE : ") { Space = SpaceProcessingModeValues.Preserve });
                p6s.Append(run6s);

                col61.Append(p6s);
                row6.Append(col61);
                tr.TableCell col62v = new tr.TableCell();
                tr.TableCell col62 = new tr.TableCell();
                tr.TableCell col63 = new tr.TableCell();
                tr.TableCell col64 = new tr.TableCell(); ;
                col62v.Append(new Paragraph(new Run(new Text(""))));
                row6.Append(col62v);
                col62.Append(new Paragraph(new Run(new Text(""))));
                row6.Append(col62);
                col63.Append(new Paragraph(new Run(new Text(""))));
                row6.Append(col63);
                col64.Append(new Paragraph(new Run(new Text(""))));
                row6.Append(col64);

                //table.Append(row6);

                tr.TableRow row7 = new tr.TableRow();
                tr.TableCell col71 = new tr.TableCell();
                Run run7s = new tr.Run();
                Paragraph p7s = new Paragraph();
                ParagraphProperties paragraphProperties44ss7ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "4", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p7s.Append(paragraphProperties44ss7ss);
                RunProperties rp7s = new RunProperties();
                rp7s.FontSize = new tr.FontSize() { Val = "18" };
                rp7s.Bold = new Bold();
                RunFonts runFont7s = new RunFonts();           // Create font
                runFont7s.Ascii = "Arial";
                rp7s.Append(runFont7s);
                run7s.Append(rp7s);
                run7s.Append(new Text(dtt.Tables[0].Rows[0]["PlcDlvry"].ToString() + ":"));
                p7s.Append(run7s);

                col71.Append(p7s);
                row7.Append(col71);
                tr.TableCell col72c = new tr.TableCell();
                tr.TableCell col72 = new tr.TableCell();
                tr.TableCell col73 = new tr.TableCell();
                tr.TableCell col74 = new tr.TableCell();
                col72c.Append(new Paragraph(new Run(new Text(""))));
                row7.Append(col72c);
                col72.Append(new Paragraph(new Run(new Text(""))));
                row7.Append(col72);
                col73.Append(new Paragraph(new Run(new Text(""))));
                row7.Append(col73);
                Paragraph p2u4 = new tr.Paragraph();
                Run R2u4 = new tr.Run();
                RunProperties rp6szr = new RunProperties();
                RunFonts runFont6szr = new RunFonts();
                ParagraphProperties paragraphProperties54ss5ssr = new ParagraphProperties(
                                                   new ParagraphStyleId() { Val = "No Spacing" },
                                                   new SpacingBetweenLines() { After = "4", Before = "0" },
                                                   new Justification() { Val = JustificationValues.Center }
                                                   );
                p2u4.Append(paragraphProperties54ss5ssr);
                rp6szr.FontSize = new tr.FontSize() { Val = "18" };
                rp6szr.Bold = new Bold();
                runFont6szr.Ascii = "Arial";
                rp6szr.Append(runFont6szr);
                R2u4.Append(rp6szr);

                R2u4.Append(new Text(dtt.Tables[0].Compute("Sum(Amount)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
                p2u4.Append(R2u4);
                col74.Append(p2u4);
                row7.Append(col74);
                //table.Append(row7);

                tr.TableRow row8 = new tr.TableRow();
                tr.TableCell col81 = new tr.TableCell();
                Run run8s = new tr.Run();
                Paragraph p8s = new Paragraph();
                ParagraphProperties paragraphProperties44ss8ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "4", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p8s.Append(paragraphProperties44ss8ss);
                RunProperties rp8s = new RunProperties();
                rp8s.FontSize = new tr.FontSize() { Val = "18" };
                rp8s.Bold = new Bold();
                RunFonts runFont8s = new RunFonts();           // Create font
                runFont8s.Ascii = "Arial";
                rp8s.Append(runFont8s);
                run8s.Append(rp8s);
                run8s.Append(new Text(dtt.Tables[0].Rows[0]["ShipmentMode"].ToString() + " : "));
                p8s.Append(run8s);

                col81.Append(p8s);
                row8.Append(col81);
                tr.TableCell col82j = new tr.TableCell();
                tr.TableCell col82 = new tr.TableCell();
                tr.TableCell col83 = new tr.TableCell();
                tr.TableCell col84 = new tr.TableCell();
                Paragraph p10ssrr = new tr.Paragraph();

                col82j.Append(new Paragraph(new Run(new Text(""))));
                row8.Append(col82j);
                col82.Append(new Paragraph(new Run(new Text(""))));
                row8.Append(col82);
                col83.Append(new Paragraph(new Run(new Text(""))));
                row8.Append(col83);
                Run r789d = new tr.Run();
                RunFonts runFont8s1 = new RunFonts();           // Create font
                runFont8s1.Ascii = "Arial";
                r789d.Append(runFont8s1);
                r789d.Append(new Text(".00"));
                r789d.RunProperties = new tr.RunProperties((new Bold()));
                r789d.RunProperties.FontSize = new tr.FontSize() { Val = "18" };

                p10ssrr.AppendChild(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }));
                p10ssrr.Append(r789d);
                col84.Append(p10ssrr);
                row8.Append(col84);

                table.Append(row8);

                tr.TableRow row9 = new tr.TableRow();
                tr.TableCell col91 = new tr.TableCell();
                Run run9s = new tr.Run();
                Paragraph p9s = new Paragraph();
                ParagraphProperties paragraphProperties44ss9ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "2", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p9s.Append(paragraphProperties44ss9ss);
                RunProperties rp9s = new RunProperties();
                rp9s.FontSize = new tr.FontSize() { Val = "18" };
                rp9s.Bold = new Bold();
                RunFonts runFont9s = new RunFonts();           // Create font
                runFont9s.Ascii = "Arial";
                rp9s.Append(runFont9s);
                run9s.Append(rp9s);
                run9s.Append(new Text(dtt.Tables[0].Rows[0]["InCoTerms"].ToString() == "" ? "" : dtt.Tables[0].Rows[0]["InCoTerms"].ToString() + " : "));
                p9s.Append(run9s);

                col91.Append(p9s);
                row9.Append(col91);
                tr.TableCell col92d = new tr.TableCell();
                tr.TableCell col92 = new tr.TableCell();
                tr.TableCell col93 = new tr.TableCell();
                tr.TableCell col94 = new tr.TableCell();
                col92d.Append(new Paragraph(new Run(new Text(""))));
                row9.Append(col92d);
                col92.Append(new Paragraph(new Run(new Text(""))));
                row9.Append(col92);
                col93.Append(new Paragraph(new Run(new Text(""))));
                row9.Append(col93);
                col94.Append(new Paragraph(new Run(new Text(""))));
                row9.Append(col94);
                if (dtt.Tables[0].Rows[0]["InCoTerms"].ToString() != "")
                    table.Append(row9);

                tr.TableRow row10 = new tr.TableRow();
                tr.TableCell col101 = new tr.TableCell();
                Run run10s = new tr.Run();
                Paragraph p10s = new Paragraph();
                ParagraphProperties paragraphProperties44ss10ss = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "2", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p10s.Append(paragraphProperties44ss10ss);
                RunProperties rp10s = new RunProperties();
                rp10s.FontSize = new tr.FontSize() { Val = "18" };
                rp10s.Bold = new Bold();
                RunFonts runFont10s = new RunFonts();           // Create font
                runFont10s.Ascii = "Arial";
                rp10s.Append(runFont10s);
                run10s.Append(rp10s);
                run10s.Append(new Text("ADD INTEGRATED TAX @ 0.1%: "));
                p10s.Append(run10s);

                col101.Append(p10s);
                row10.Append(col101);
                tr.TableCell col102f = new tr.TableCell();
                tr.TableCell col102 = new tr.TableCell();
                tr.TableCell col103 = new tr.TableCell();
                tr.TableCell col104 = new tr.TableCell();
                col102f.Append(new Paragraph(new Run(new Text(""))));
                row10.Append(col102f);
                col102.Append(new Paragraph(new Run(new Text(""))));
                row10.Append(col102);
                col103.Append(new Paragraph(new Run(new Text(""))));
                row10.Append(col103);
                tr.Paragraph p10ss = new tr.Paragraph();
                ParagraphProperties paragraphProperties54ss10ss = new ParagraphProperties(
                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                  new SpacingBetweenLines() { After = "4", Before = "0" },
                                                  new Justification() { Val = JustificationValues.Center }
                                                  );
                p10ss.Append(paragraphProperties54ss10ss);
                Run r789 = new tr.Run();
                RunFonts runFont8s2 = new RunFonts();           // Create font
                runFont8s2.Ascii = "Arial";
                r789.Append(runFont8s2);
                r789.Append(new Text("0.00"));
                r789.RunProperties = new tr.RunProperties((new Bold()));
                r789.RunProperties.FontSize = new tr.FontSize() { Val = "18" };

                p10ss.Append(r789);
                col104.Append(p10ss);
                row10.Append(col104);

                table.Append(row10);

                table.Append(row7);
                tr.TableRow row11g = new tr.TableRow();
                tr.TableCell col1011g = new tr.TableCell();
                Run run10s1g = new tr.Run();
                Paragraph p10s1g = new Paragraph();
                ParagraphProperties paragraphProperties44ss10ss1g = new ParagraphProperties(
                                                       new ParagraphStyleId() { Val = "No Spacing" },
                                                       new SpacingBetweenLines() { After = "4", Before = "0" },
                                                       new Justification() { Val = JustificationValues.Right }
                                                       );
                p10s1g.Append(paragraphProperties44ss10ss1g);
                RunProperties rp10s1g = new RunProperties();
                rp10s1g.FontSize = new tr.FontSize() { Val = "18" };
                rp10s1g.Bold = new Bold();
                RunFonts runFont10s1g = new RunFonts();           // Create font
                runFont10s1g.Ascii = "Arial";
                rp10s1g.Append(runFont10s1g);
                run10s1g.Append(rp10s1g);
                run10s1g.Append(new Text("GST PAYABLE UNDER REVERSE CHARGE:"));
                p10s1g.Append(run10s1g);

                col1011g.Append(p10s1g);
                row11g.Append(col1011g);
                tr.TableCell col1021gd = new tr.TableCell();
                tr.TableCell col1021g = new tr.TableCell();
                tr.TableCell col1031g = new tr.TableCell();
                tr.TableCell col1041g = new tr.TableCell();
                col1021gd.Append(new Paragraph(new Run(new Text(""))));
                row11g.Append(col1021gd);
                col1021g.Append(new Paragraph(new Run(new Text(""))));
                row11g.Append(col1021g);
                col1031g.Append(new Paragraph(new Run(new Text(""))));
                row11g.Append(col1031g);
                tr.Paragraph p10ss1g = new tr.Paragraph();
                ParagraphProperties paragraphProperties54ss10ss1g = new ParagraphProperties(
                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                  new SpacingBetweenLines() { After = "4", Before = "0" },
                                                  new Justification() { Val = JustificationValues.Center }
                                                  );
                p10ss1g.Append(paragraphProperties54ss10ss1g);
                Run r789g = new tr.Run();
                RunFonts runFont8s3 = new RunFonts();           // Create font
                runFont8s3.Ascii = "Arial";
                r789g.Append(runFont8s3);
                r789g.Append(new Text("0.00"));
                r789g.RunProperties = new tr.RunProperties((new Bold()));
                r789g.RunProperties.FontSize = new tr.FontSize() { Val = "18" };

                p10ss1g.Append(r789g);
                //p10ss1g.Append(new Run(new Text("0.00")));
                col1041g.Append(p10ss1g);
                row11g.Append(col1041g);

                table.Append(row11g);


                tr.TableRow tr1 = new tr.TableRow();
                TableCellProperties tableCellProperties = new TableCellProperties();
                HorizontalMerge verticalMerge = new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                };
                tableCellProperties.Append(verticalMerge);
                TableCellProperties tableCellProperties1 = new TableCellProperties();
                HorizontalMerge verticalMerge1 = new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                };
                tableCellProperties1.Append(verticalMerge1);
                tr.TableCell tc11 = new tr.TableCell();
                Paragraph p11 = new Paragraph();
                Run r12 = new Run();
                RunProperties rp12 = new RunProperties();
                RunFonts runFont_11 = new RunFonts();           // Create font
                runFont_11.Ascii = "Arial";
                rp12.Append(runFont_11);
                rp12.Bold = new Bold();
                rp12.FontSize = new tr.FontSize() { Val = "20" };
                ParagraphProperties pp11 = new ParagraphProperties();
                pp11.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties11 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p11.Append(paragraphProperties11);
                p11.Append(pp11);
                r12.Append(rp12);
                r12.Append(new Text("In Words :" + Session["TotalAmt"].ToString() + ""));
                p11.Append(r12);
                tc11.Append(p11);
                tr1.Append(tc11);
                //TableRowProperties tableRowProperties1 = new TableRowProperties();
                //TableRowHeight tableRowHeight1 = new TableRowHeight() { Val = (UInt32Value)28U };

                //tableRowProperties1.Append(tableRowHeight1);
                //tr1.InsertBefore(tableRowProperties1, tc11);
                tr.TableCell tc12d = new tr.TableCell();
                Paragraph p12d = new Paragraph();

                tc12d.Append(p12d);
                tr1.Append(tc12d);
                tr.TableCell tc12 = new tr.TableCell();
                Paragraph p12 = new Paragraph();

                tc12.Append(p12);
                tr1.Append(tc12);
                tr.TableCell tc13 = new tr.TableCell();
                Paragraph p13 = new Paragraph();
                tc13.Append(p13);
                tr1.Append(tc13);
                tr.TableCell tc14 = new tr.TableCell();
                Paragraph p14 = new Paragraph();

                tc14.Append(p14);
                tr1.Append(tc14);
                table.Append(tr1);
                body.Append(table);
                mainPart.Document.Body = body;
                //PageSize pageSize1 = new PageSize() { Width = (UInt32Value)11906U, Height = (UInt32Value)16838U };
                //IEnumerable<SectionProperties> sectionProperties1 = mainPart.Document.Body.Elements<SectionProperties>();
                //if (sectionProperties1 == null)
                //{
                //    sectionProperties1.Append(pageSize1);
                //    sectionProperties1 = new SectionProperties() { };
                //    mainPart.Document.Body.Append(sectionProperties1);
                //}
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "yourMessages", "alert('" + ex.Message.ToString() + "');", true);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Others/ErrorLog"), "Shipment ProformaInvoice Details", ex.Message.ToString());
            }
        }
        private static void SetTableStyle(tr.Table table, string wid, bool left, bool right, bool top, bool bottom, bool Horizontal, bool vertical)
        {
            TableProperties properties = new TableProperties();

            //table borders
            TableBorders borders = new TableBorders();
            if (top)
                borders.TopBorder = new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single) };
            if (bottom)
                borders.BottomBorder = new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single) };
            if (left)
                borders.LeftBorder = new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single) };
            if (right)
                borders.RightBorder = new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single) };
            if (Horizontal)
                borders.InsideHorizontalBorder = new InsideHorizontalBorder() { Val = BorderValues.Single };
            if (vertical)
                borders.InsideVerticalBorder = new InsideVerticalBorder() { Val = BorderValues.Single };

            properties.Append(borders);

            //set the table width to page width
            TableWidth tableWidth = new TableWidth() { Width = wid, Type = TableWidthUnitValues.Pct };
            properties.Append(new TableJustification { Val = TableRowAlignmentValues.Center });
            properties.Append(tableWidth);

            //add properties to table
            table.Append(properties);
        }

        private static void ChangeStyleDefinitionsPart1(StyleDefinitionsPart styleDefinitionsPart1)
        {
            Styles styles1 = styleDefinitionsPart1.Styles;
            tr.Style style1 = styles1.GetFirstChild<tr.Style>(); //get the specifc style
            Rsid rsid1 = new Rsid() { Val = "00B10D4B" };
            style1.Append(rsid1);
            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            tr.FontSize fontSize1 = new tr.FontSize() { Val = "144" };
            styleRunProperties1.Append(fontSize1);
            style1.Append(styleRunProperties1);
        }

        private string[] SplitByLenght(string s, int split)
        {
            //Like using List because I can just add to it 
            List<string> list = new List<string>();

            // Integer Division
            int TimesThroughTheLoop = s.Length / split;


            for (int i = 0; i < TimesThroughTheLoop; i++)
            {
                list.Add(s.Substring(i * split, split));

            }

            // Pickup the end of the string
            if (TimesThroughTheLoop * split != s.Length)
            {
                list.Add(s.Substring(TimesThroughTheLoop * split));
            }

            return list.ToArray();
        }

        # endregion
    }
}