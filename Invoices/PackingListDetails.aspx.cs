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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using ta = DocumentFormat.OpenXml.InkML;
using tr = DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Wordprocessing;


namespace VOMS_ERP.Invoices
{
    public partial class PackingListDetails : System.Web.UI.Page
    {
        # region Variables

        long ID;
        ErrorLog ELog = new ErrorLog();
        ReportDocument RptDoc = new ReportDocument();
        # endregion

        #region Page Load Events

        /// <summary>
        /// Default Page Load Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["ExportPckList1"] = null;
            Session["ExportPckList2"] = null;
            if (Session["UserID"] == null || new Guid(Session["UserID"].ToString()) == Guid.Empty)
                Response.Redirect("../Login.aspx?logout=yes", false);
            else
            {
                if (CommonBLL.IsAuthorisedUser(new Guid(Session["UserID"].ToString()), Request.Path))
                {
                    GetData(new Guid(Request.QueryString["ID"]));
                }
                else
                    Response.Redirect("../Masters/Home.aspx?NP=no", false);
            }
        }

        /// <summary>
        /// Page Load On-Load Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.PackingListDtls = null;
            GC.Collect();
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                CloseReports(RptDoc);
                RptDoc.Dispose();
                PackingListDtls.Dispose();
                PackingListDtls = null;

            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                ELog.CreateErrorLog(Server.MapPath("../Logs/Enquiries/ErrorLog"), "Foreing Enquiry Details", ex.Message.ToString());
            }
        }

        #endregion

        # region Methods

        protected void GetData(Guid ID)
        {
            try
            {
                PackingListBLL PLBLL = new PackingListBLL();
                DataSet dataset = new DataSet();
                DataSet dsItems = new DataSet();
                dsItems = PLBLL.GetRDLC(ID);
                dataset = PLBLL.SelectDetails(CommonBLL.FlagZSelect, ID, Guid.Empty, Guid.Empty, CommonBLL.PackingListItems(), new Guid(Session["CompanyID"].ToString()), "");
                Session["ExportPckList1"] = dsItems;
                Session["ExportPckList2"] = dataset;
                if (dataset.Tables.Count > 1 && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[1].Rows.Count > 0)
                {
                    DataSet dsss = PLBLL.GetRDLC(new Guid(Session["CompanyID"].ToString()));
                    //RptDoc.FileName = Server.MapPath("\\RDLC\\PackingListCrp.rpt");
                    if (dsss.Tables[1].Rows[0]["CompanyID"].ToString() == Session["CompanyID"].ToString() && dsss.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("glocem"))
                    {
                        RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\Glocem\\PackingListCrp.rpt");
                    }
                    else
                    {
                        RptDoc.FileName = Server.MapPath("\\" + CommonBLL.GetReportsPath() + "\\PackingListCrp.rpt");
                    }
                    RptDoc.Load(RptDoc.FileName);

                    foreach (ReportObject item in RptDoc.ReportDefinition.ReportObjects)
                    {
                        if (item.Kind == ReportObjectKind.SubreportObject)
                        {
                            string SubRepName = ((SubreportObject)item).SubreportName;
                            ReportDocument subRepDoc = RptDoc.Subreports[SubRepName];
                            if (SubRepName == "PackingBody.rpt")
                                subRepDoc.Database.Tables[0].SetDataSource(dsItems.Tables[0]);
                        }
                    }
                    RptDoc.SetParameterValue("Date_RPT", dataset.Tables[0].Rows[0]["PrfrmInvDT"].ToString());
                    RptDoc.SetParameterValue("Invoiceno_RPT", dataset.Tables[0].Rows[0]["PrfmInvcNo"].ToString());
                    RptDoc.SetParameterValue("Grosswt_RPT", dsItems.Tables[0].Rows[0]["GrWeight"].ToString());
                    RptDoc.SetParameterValue("Netwt_RPT", dsItems.Tables[0].Rows[0]["NetWeight"].ToString());

                    RptDoc.SetParameterValue("RPT_B_ProformaInv_No_Dt", dataset.Tables[0].Rows[0]["PrfrmInvNoDT"].ToString());
                    RptDoc.SetParameterValue("RPT_C_FPONos", dataset.Tables[0].Rows[0]["FPONOs"].ToString());
                    RptDoc.SetParameterValue("RPT_D_Consignee", dataset.Tables[0].Rows[0]["OtherReferences"].ToString());
                    RptDoc.SetParameterValue("RPT_E_To", dataset.Tables[0].Rows[0]["OtherReferences"].ToString());
                    RptDoc.SetParameterValue("RPT_E_CustNm", dataset.Tables[0].Rows[0]["CustomerName"].ToString());
                    RptDoc.SetParameterValue("RPT_E_CustShpAdd", dataset.Tables[0].Rows[0]["CustShipAdd"].ToString());
                    string Notify = dataset.Tables[0].Rows[0]["Notify"].ToString();
                    string NotifyAdd = dataset.Tables[0].Rows[0]["NotifyAddress"].ToString();
                    string Phone = "PH : " + dataset.Tables[0].Rows[0]["Phone"].ToString();
                    string Fax = "FAX : " + dataset.Tables[0].Rows[0]["Fax1"].ToString();
                    if (NotifyAdd != "")
                        NotifyAdd = NotifyAdd + System.Environment.NewLine + Phone + " \t \t \t \t \t" + Fax;
                    RptDoc.SetParameterValue("RPT_F_Notify", Notify);
                    RptDoc.SetParameterValue("RPT_F_NotifyAdd", NotifyAdd);
                    //RptDoc.SetParameterValue("RPT_G_CountryOrigin", dataset.Tables[0].Rows[0]["PlcOrgnGds"].ToString());
                    RptDoc.SetParameterValue("RPT_H_CountryDestination", dataset.Tables[0].Rows[0]["PlcFnlDstn"].ToString());
                    RptDoc.SetParameterValue("RPT_I_Vessel", dataset.Tables[0].Rows[0]["VslFltNo"].ToString() == "303" ? "By Air" : "By Sea");
                    RptDoc.SetParameterValue("RPT_J_PortOfLoading", dataset.Tables[0].Rows[0]["PrtLdng"].ToString());
                    RptDoc.SetParameterValue("RPT_K_PortOfDischarge", dataset.Tables[0].Rows[0]["PrtDschrg"].ToString());
                    RptDoc.SetParameterValue("RPT_L_PortOfDelivery", dataset.Tables[0].Rows[0]["PlcDlvry"].ToString());
                    RptDoc.SetParameterValue("RPT_N_IncoTerms", dataset.Tables[0].Rows[0]["IncoTerms"].ToString());
                    RptDoc.SetParameterValue("RPT_M_TermsOfDelivery", dataset.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString());

                    if (dataset.Tables.Count > 1 && dataset.Tables[0].Rows.Count > 0 && dataset.Tables[0].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                    {
                        RptDoc.SetParameterValue("GST", dataset.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]);
                        RptDoc.SetParameterValue("CIN", dataset.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[1]);
                        RptDoc.SetParameterValue("STATE", dataset.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[2]);
                        RptDoc.SetParameterValue("GSTNo", dataset.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[0]);
                        RptDoc.SetParameterValue("GSTDate", dataset.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[1]);
                        RptDoc.SetParameterValue("EndCode", dataset.Tables[0].Rows[0]["EUCIT"].ToString());
                        RptDoc.SetParameterValue("PreCrgBy", dataset.Tables[0].Rows[0]["PreCrgBy"].ToString());
                        RptDoc.SetParameterValue("PlcRcpntPreCrgBy", dataset.Tables[0].Rows[0]["PlcRcpntPreCrgBy"].ToString());
                    }
                    
                    RptDoc.SetParameterValue("Phone", Phone);
                    RptDoc.SetParameterValue("Fax1", Fax);

                    RptDoc.SetParameterValue("TotalPkgs", dsItems.Tables[0].Rows[0]["NoOfPkgs"].ToString());
                    RptDoc.SetParameterValue("CustPkgs", dsItems.Tables[0].Rows[0]["CustPkgs"].ToString());
                    RptDoc.SetParameterValue("CustName", dsItems.Tables[0].Rows[0]["CustName"].ToString());
                    RptDoc.SetParameterValue("BillingCity", dsItems.Tables[0].Rows[0]["BillingCity"].ToString());

                    string TinNo = "IEC Code No. 0996008306";
                    if (dsItems.Tables.Count > 1 && dsItems.Tables[1].Rows.Count > 0 && dsItems.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                    {
                        RptDoc.SetParameterValue("CompanyDetails", dataset.Tables[0].Rows[0]["CompanyDetails"].ToString() + TinNo);
                    }
                    else
                    {
                        TinNo = "\n";
                        RptDoc.SetParameterValue("CompanyDetails", dataset.Tables[0].Rows[0]["CompanyDetails"].ToString() + TinNo);
                    }
                    RptDoc.SetParameterValue("CompanyName", dataset.Tables[0].Rows[0]["CompanyName"].ToString());

                    PackingListDtls.ReportSource = RptDoc;
                }
                if (Request.QueryString["IsDash"] != null && Request.QueryString["IsDash"].ToString() == "1")
                {
                    lbtnBack.PostBackUrl = "~/Masters/Dashboard.aspx";
                    //lbtnCntnu.Enabled = false;
                    //lbtnCntnu.Visible = false;
                }
                else
                {
                    lbtnBack.PostBackUrl = "~/Invoices/PackingListStatus.aspx";
                }
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.Message;
                int LineNo = ExceptionHelper.LineNumber(ex);
                ELog.CreateErrorLog(Server.MapPath("../Logs/Logistics/ErrorLog"), "PackingList Details", ex.Message.ToString());
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
                    string filepath = @"PackinglistExport" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.TimeOfDay.ToString() + ".doc";
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
                        HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>("r97");
                        FooterPart footerPart = mainPart.AddNewPart<FooterPart>("r98");

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

                        //OpenAndAddTextToWordDocument(mainPart, body);

                        AddHeaderAndFooter(mainPart, headerPart, footerPart);

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
                DataSet dtt1 = (DataSet)Session["ExportPckList1"];
                DataSet dtt = (DataSet)Session["ExportPckList2"];
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
                rp12s.FontSize = new tr.FontSize() { Val = "20" };
                RunFonts runFont_1 = new RunFonts();           // Create font
                runFont_1.Ascii = "Arial";
                rp12s.Append(runFont_1);
                ParagraphProperties pp11s = new ParagraphProperties();
                pp11s.Justification = new Justification() { Val = JustificationValues.Center };
                ParagraphProperties paragraphProperties11s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                p11s.Append(paragraphProperties11s);
                r12s.Append(rp12s);
                r12s.Append(new Text("PACKING LIST"));
                p11s.Append(r12s);
                p11s.Append(pp11s);
                tc11s.Append(p11s);
                tr1s.Append(tc11s);
                TableRowProperties tableRowProperties1s = new TableRowProperties();
                TableRowHeight tableRowHeight1s = new TableRowHeight() { Val = (UInt32Value)34U };

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
                RunProperties rp12sssa = new RunProperties();
                rp12sssa.Bold = new Bold();
                rp12sssa.FontSize = new tr.FontSize() { Val = "18" };
                run.Append(rp12sssa);
                run.AppendChild(new Text("Exporter: "));
                run.AppendChild(new Break());
                run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyName"].ToString()));
                run.AppendChild(new Break());
                run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyDetails"].ToString()));

                //run.AppendChild(new Break());
                //run.AppendChild(new Text("KONDAPUR, HYDERABAD, TELANGANA, INDIA-500081"));
                run.AppendChild(new Break());
                rn.RunProperties = new tr.RunProperties((new Bold()));
                rn.RunProperties.FontSize = new tr.FontSize() { Val = "18" };
                //if (dtt != null)
                //    if (dtt.Tables.Count > 1 && dtt.Tables[1].Rows.Count > 0 && dtt.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
                //    {
                RunFonts runFont_3 = new RunFonts();           // Create font
                runFont_3.Ascii = "Arial";
                rn.Append(runFont_3);
                rn.AppendChild(new Text("IEC Code No. 0996008306"));
                rn.AppendChild(new Break());
                //    }
                RunProperties rp12sssad = new RunProperties();
                rp12sssad.FontSize = new tr.FontSize() { Val = "18" };
                rn.Append(rp12sssad);
                rn.AppendChild(new Text("GSTIN No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]));
                rn.AppendChild(new Break());
                rn.AppendChild(new Text("CIN No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[1]));
                //rn.AppendChild(rp12sssa);
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
                RunProperties rp12sw3c = new RunProperties();
                Run rrrs = new Run();
                rp12sw3c.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8s12 = new RunFonts();           // Create font
                runFont8s12.Ascii = "Arial";
                rp12sw3c.Append(runFont8s12);
                rrrs.Append(rp12sw3c);
                rrrs.AppendChild(new Text(dtt.Tables[0].Rows[0]["PrfrmInvNoDT"].ToString()));
                pd.Append(rrrs);
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
                RunProperties rp12sw3ce = new RunProperties();
                Run rrrse = new Run();
                rp12sw3ce.FontSize = new tr.FontSize() { Val = "18" };
                rrrse.Append(rp12sw3ce);
                RunFonts runFont8s121 = new RunFonts();           // Create font
                runFont8s121.Ascii = "Arial";
                rp12sw3ce.Append(runFont8s121);
                rrrse.AppendChild(new Text("GSTIN No:" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]));
                pd2.Append(rrrse);
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
                rn3t.AppendChild(new Text("STATE :" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[2]));
                pd22.Append(rn3t);
                tr.TableCell col222 = new tr.TableCell();
                col222.Append(pd22);
                row22.Append(col222);
                tbl22.Append(row22);
                tr.TableCell col2s = new tr.TableCell();
                col2s.Append(new Paragraph(new Run(tbl22)));
                row2s.Append(col2s);
                tbl2.Append(row2s);

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
                RunProperties r4_ = new RunProperties();
                Run rn4t = new tr.Run();
                rn4t.Append(r4_);
                r4_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont8sf = new RunFonts();           // Create font
                runFont8sf.Ascii = "Arial";
                r4_.Append(runFont8sf);
                rn4t.AppendChild(new Text("Reverse Charge Applicable:No"));
                pd3.Append(rn4t);
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
                RunProperties r5_ = new RunProperties();
                Run rn5t = new tr.Run();
                rn5t.Append(r5_);
                r5_.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont5se = new RunFonts();           // Create font
                runFont5se.Ascii = "Arial";
                r5_.Append(runFont5se);
                //runrr3.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[1]));
                if (dtt.Tables[0].Rows[0]["EUCIT"].ToString() != "")
                {
                    if (dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',').Count() == 2)
                        rn5t.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[1]));
                    else if (dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',').Count() == 1)
                        rn5t.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[0]));
                    else
                        rn5t.AppendChild(new Text("END USE CODE "));
                }
                else
                    rn5t.AppendChild(new Text("END USE CODE "));
                pd33.Append(rn5t);
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
                RunProperties rp12swe4 = new RunProperties();
                Run runrr4 = new Run();
                //rp12swe4.FontSize = new tr.FontSize() { Val = "18" };
                runrr4.Append(rp12swe4);
                if (dtt.Tables[0].Rows[0]["FPONOs"].ToString().Length > 70)
                {
                    rp12swe4.FontSize = new tr.FontSize() { Val = "16" };
                    RunFonts runFont_4 = new RunFonts();           // Create font
                    runFont_4.Ascii = "Arial";
                    rp12swe4.Append(runFont_4);
                    string txtsplit = string.Empty;
                    string[] a = SplitByLenght("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString(), 70);
                    for (int i = 0; i < a.Length; i++)
                    {
                        runrr4.AppendChild(new Text(a[i]));
                        if (i != (a.Length - 1))
                            runrr4.AppendChild(new Break());
                    }
                    //runrr4.AppendChild(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString()));
                }
                else
                {
                    rp12swe4.FontSize = new tr.FontSize() { Val = "16" };
                    RunFonts runFont_4 = new RunFonts();           // Create font
                    runFont_4.Ascii = "Arial";
                    rp12swe4.Append(runFont_4);
                    runrr4.AppendChild(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString()));
                }

                pd4.Append(runrr4);
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
                //rn5.AppendChild(new Break());
                rn5.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                //rn5.AppendChild(new Break());
                rn5.AppendChild(new Text("PH:" + dtt.Tables[0].Rows[0]["Phone"].ToString()));

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
                RunFonts runFont_6 = new RunFonts();           // Create font
                runFont_6.Ascii = "Arial";
                rp234s.Append(runFont_6);
                run3.Append(rp234s);
                run3.AppendChild(new Text("Consignee : "));
                run3.AppendChild(new Break());
                run3.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustomerName"].ToString()));
                //run3.AppendChild(new Break());
                run3.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                //run3.AppendChild(new Break());
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
                //Run rrrr = new tr.Run();
                RunProperties rp22s = new RunProperties();
                rp22s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_7 = new RunFonts();           // Create font
                runFont_7.Ascii = "Arial";
                rp22s.Append(runFont_7);
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
                RunFonts runFont_8 = new RunFonts();           // Create font
                runFont_8.Ascii = "Arial";
                rp2s.Append(runFont_8);
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
                RunFonts runFont_9 = new RunFonts();           // Create font
                runFont_9.Ascii = "Arial";
                rp43s.Append(runFont_9);
                run4s.Append(rp43s);
                if (dtt.Tables[0].Rows[0]["Notify"].ToString() != "")
                {
                    run4s.AppendChild(new Text("Notify: " + dtt.Tables[0].Rows[0]["Notify"].ToString()));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["NotifyAddress"].ToString()));
                    //run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text("PH : " + dtt.Tables[0].Rows[0]["NPhone"].ToString() + " " + " Fax : " + dtt.Tables[0].Rows[0]["NFax1"].ToString()));
                    //run4s.Append(new Break());
                }
                else
                {
                    run4s.AppendChild(new Text("Notify : "));
                    run4s.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustomerName"].ToString()));
                    //run3.AppendChild(new Break());
                    run4s.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
                    //run3.AppendChild(new Break());
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
                SetTableStyle(tbl33s, "5000", false, false, true, false, true, true);
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
                RunFonts runFont_10 = new RunFonts();           // Create font
                runFont_10.Ascii = "Arial";
                rp333s.Append(runFont_10);
                rr3.Append(rp333s);
                if (dtt.Tables[0].Rows[0]["PreCrgBy"].ToString() != "")
                    rr3.Append(new Text("Pre-Carriage by:" + dtt.Tables[0].Rows[0]["PreCrgBy"].ToString()));
                else
                    rr3.Append(new Text("Pre-Carriage by:"));

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
                RunFonts runFont_11 = new RunFonts();           // Create font
                runFont_11.Ascii = "Arial";
                rp533s.Append(runFont_11);
                rr33.Append(rp533s);
                if (dtt.Tables[0].Rows[0]["PlcRcpntPreCrgBy"].ToString() != "")
                    rr33.Append(new Text("Place of receipt by Pre-Carriage: " + dtt.Tables[0].Rows[0]["PlcRcpntPreCrgBy"].ToString()));
                else
                    rr33.Append(new Text("Place of receipt by Pre-Carriage: "));

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
                RunFonts runFont_12 = new RunFonts();           // Create font
                runFont_12.Ascii = "Arial";
                rp633s.Append(runFont_12);
                rr63.Append(rp633s);
                rr63.Append(new Text("Vessel/Flight No: " + dtt.Tables[0].Rows[0]["VslFltNo"].ToString() == "303" ? "By Air" : "By Sea"));
                rr63.Append(new Break());
                //rr63.Append(new Break());
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
                RunFonts runFont_13 = new RunFonts();           // Create font
                runFont_13.Ascii = "Arial";
                rp63s.Append(runFont_13);
                rr633.Append(rp63s);
                rr633.Append(new Text("Port of Loading: " + dtt.Tables[0].Rows[0]["PrtLdng"].ToString()));
                rr633.Append(new Break());
                //rr633.Append(new Break());
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
                RunFonts runFont_14 = new RunFonts();           // Create font
                runFont_14.Ascii = "Arial";
                rp73s.Append(runFont_14);
                rr73.Append(rp73s);
                if (dtt.Tables[0].Rows[0]["PrtDschrg"].ToString().Length <= 15)
                    rr73.Append(new Break());
                rr73.Append(new Text("Port of Discharge:"));
                rr73.Append(new Break());
                rr73.Append(new Break());
                rr73.Append(new Text(dtt.Tables[0].Rows[0]["PrtDschrg"].ToString()));
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
                RunFonts runFont_15 = new RunFonts();           // Create font
                runFont_15.Ascii = "Arial";
                rp773s.Append(runFont_15);
                rr733.Append(rp773s);
                if (dtt.Tables[0].Rows[0]["PlcDlvry"].ToString().Length <= 15)
                    rr733.Append(new Break());
                rr733.Append(new Text("Place of Delivery:"));
                rr733.Append(new Break());
                rr733.Append(new Break());
                rr733.Append(new Text(dtt.Tables[0].Rows[0]["PlcDlvry"].ToString()));
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

                rp42s.Bold = new Bold();
                rp42s.FontSize = new tr.FontSize() { Val = "16" };
                RunFonts runFont_16 = new RunFonts();           // Create font
                runFont_16.Ascii = "Arial";
                rp42s.Append(runFont_16);
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
                run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["Incoterms"].ToString()));
                //run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["IncTrmLctn"].ToString()));
                run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split(',')[0]));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split(',')[1]));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text(dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split(',')[2]));
                //run42s.AppendChild(new Break());
                int uy = 0;
                foreach (var gg in dtt.Tables[0].Rows[0]["TrmsDlvryPmnt"].ToString().Split('\n'))
                {
                    if (gg != "")
                        run42s.AppendChild(new Text(gg + " , "));
                    //if (uy % 2 == 0)
                    //if (gg != "A   D Code No.                   : 0340872 -8000009") //modified by Satya :: Rahman mail dated on 09-July-2020 
                    if (gg != "A   D Code No.                   : 02900FE")
                        run42s.AppendChild(new Break());
                    uy += 1;
                }
                //run42s.AppendChild(new Text("SWIFT CODE                 : ANDBINBB"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("BENEFICIARY NAME & ADDRESS : VOLTA IMPEX PRIVATE  LIMITED"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("Bank Name & address      : ANDHRA BANK, S R NAGAR BRANCH, HYDERABAD– 500038 INDIA"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("Account Number             : 052211011007863"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("IFSC CODE of Branch        : ANDB0000522"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("F.Ex: Dealer & Code        : ANDHRA BANK, OSB,PUNJAGUTTA, HYDERABAD 500 038"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("IFSC CODE of Branch        : ANDB0000667"));
                //run42s.AppendChild(new Break());
                //run42s.AppendChild(new Text("A   D Code No.             : 0340872 -8000009"));
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
                RunFonts runFont_17 = new RunFonts();           // Create font
                runFont_17.Ascii = "Arial";
                rp81s.Append(runFont_17);
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
                SetTableStyle(tbl9s, "5000", false, false, false, false, true, true);
                tr.TableRow row91s = new tr.TableRow();
                tr.TableCell col91s = new tr.TableCell();
                
                
                Run rr91 = new tr.Run();
                RunProperties rp91s = new RunProperties();
                rp91s.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_18 = new RunFonts();           // Create font
                runFont_18.Ascii = "Arial";
                rp91s.Append(runFont_18);
                rp91s.Bold = new Bold();
                rr91.Append(rp91s);
                rr91.Append(new Text("Marks & Nos.No     No & Kind of Pkg.      Description of Goods"));
                tr.Paragraph pd91s = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd91s.Append(paragraphProperties91ss3s);
                pd91s.Append(rr91);
                col91s.Append(pd91s);
                col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3690" }));
                row91s.Append(col91s);

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
                RunFonts runFont_20 = new RunFonts();           // Create font
                runFont_20.Ascii = "Arial";
                rp92s.Append(runFont_20);
                rp92s.Bold = new Bold();
                rr92.Append(rp92s);
                rr92.Append(new Text("Quantity"));
                pd92ss.Append(rr92);

                col92ss.Append(pd92ss);
                col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1280" }));
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
                RunFonts runFont_21 = new RunFonts();           // Create font
                runFont_21.Ascii = "Arial";
                rp93s.Append(runFont_21);
                rp93s.Bold = new Bold();
                rr93.Append(rp93s);
                rr93.Append(new Text("Net Weight(KGS)"));
                pd93ss.Append(rr93);

                col93ss.Append(pd93ss);
                col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "270" }));
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
                RunFonts runFont_22 = new RunFonts();           // Create font
                runFont_22.Ascii = "Arial";
                rp94s.Append(runFont_22);
                rp94s.Bold = new Bold();
                rr94.Append(rp94s);
                rr94.Append(new Text("Gross Weight(KGS)"));
                pd94ss.Append(rr94);

                col94ss.Append(pd94ss);
                col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "330" }));
                row91s.Append(col94ss);
                TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new BottomBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new LeftBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new RightBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.Single),
                        },
                        new InsideHorizontalBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new InsideVerticalBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.Single),
                        }
                    )
                ); 
                // Append the TableProperties object to the empty table.
                tbl9s.AppendChild<TableProperties>(tblProp);


                tbl9s.Append(row91s);

                tr.TableRow row91sg = new tr.TableRow();
                tr.TableCell col91sg = new tr.TableCell();
                tr.Paragraph pd91sg = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3sg = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd91sg.Append(paragraphProperties91ss3sg);
                Run rr91g = new tr.Run();
                RunProperties rp91sg = new RunProperties();
                rp91sg.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_221 = new RunFonts();
                runFont_221.Ascii = "Arial";
                rr91g.Append(runFont_221);
                rp91sg.Bold = new Bold();
                rr91g.Append(rp91sg);
                ///Changed from 'TotalPkgs' to 'NoOfPkgs' because Total packages count is not coming as expected by Dinesh 14-05-2019
                rr91g.Append(new Text(dtt1.Tables[0].Rows[0]["CustName"].ToString() + "                     No. of Pkgs.  " + dtt1.Tables[0].Rows[0]["NoOfPkgs"].ToString() + "No(s)"));
                pd91sg.Append(rr91g);

                col91sg.Append(pd91sg);
                col91sg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3080" }));
                row91sg.Append(col91sg);

                tr.TableCell col92ssg = new tr.TableCell();
                tr.Paragraph pd92ssg = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ssg = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ssg.Append(paragraphProperties92ss33ssg);
                Run rr92g = new tr.Run();
                RunProperties rp92sg = new RunProperties();
                rp92sg.FontSize = new tr.FontSize() { Val = "18" };
                rp92sg.Bold = new Bold();
                rr92g.Append(rp92sg);
                rr92g.Append(new Text(""));
                pd92ssg.Append(rr92g);

                col92ssg.Append(pd92ssg);
                col92ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1200" }));
                row91sg.Append(col92ssg);

                tr.TableCell col93ssg = new tr.TableCell();
                tr.Paragraph pd93ssg = new tr.Paragraph();
                ParagraphProperties paragraphProperties93ss33ssg = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd93ssg.Append(paragraphProperties93ss33ssg);
                Run rr93g = new tr.Run();
                RunProperties rp93sg = new RunProperties();
                rp93sg.FontSize = new tr.FontSize() { Val = "18" };
                rp93sg.Bold = new Bold();
                rr93g.Append(rp93sg);
                rr93g.Append(new Text(""));
                pd93ssg.Append(rr93g);

                col93ssg.Append(pd93ssg);
                col93ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
                row91sg.Append(col93ssg);

                tr.TableCell col94ssg = new tr.TableCell();
                tr.Paragraph pd94ssg = new tr.Paragraph();
                ParagraphProperties paragraphProperties94ss33ssg = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd94ssg.Append(paragraphProperties94ss33ssg);
                Run rr94g = new tr.Run();
                RunProperties rp94sg = new RunProperties();
                rp94sg.FontSize = new tr.FontSize() { Val = "18" };
                rp94sg.Bold = new Bold();
                rr94g.Append(rp94sg);
                rr94g.Append(new Text(""));
                pd94ssg.Append(rr94g);

                col94ssg.Append(pd94ssg);
                col94ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
                row91sg.Append(col94ssg);

                tbl9s.Append(row91sg);


                tr.TableRow row91sgn = new tr.TableRow();
                tr.TableCell col91sgn = new tr.TableCell();
                tr.Paragraph pd91sgn = new tr.Paragraph();
                ParagraphProperties paragraphProperties91ss3sgn = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Left }
                                              );
                pd91sgn.Append(paragraphProperties91ss3sgn);
                //Run rr91gn = new tr.Run();
                //RunProperties rp91sgn = new RunProperties();
                //rp91sgn.FontSize = new tr.FontSize() { Val = "18" };
                //rp91sgn.Bold = new Bold();
                //rr91gn.Append(rp91sgn);
                Run rr91gn = new tr.Run();
                RunProperties rp91sgn = new RunProperties();
                rp91sgn.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_2241 = new RunFonts();           // Create font
                runFont_2241.Ascii = "Arial";
                rr91gn.Append(runFont_2241);
                rp91sgn.Bold = new Bold();
                rr91gn.Append(rp91sgn);
                rr91gn.Append(new Text(dtt1.Tables[0].Rows[0]["BillingCity"].ToString() + "             No." + dtt1.Tables[0].Rows[0]["CustPkgs"].ToString()) { Space = SpaceProcessingModeValues.Preserve });
                pd91sgn.Append(rr91gn);

                col91sgn.Append(pd91sgn);
                col91sgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3080" }));
                row91sgn.Append(col91sgn);

                tr.TableCell col92ssgn = new tr.TableCell();
                tr.Paragraph pd92ssgn = new tr.Paragraph();
                ParagraphProperties paragraphProperties92ss33ssgn = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd92ssgn.Append(paragraphProperties92ss33ssgn);
                Run rr92gn = new tr.Run();
                RunProperties rp92sgn = new RunProperties();
                rp92sgn.FontSize = new tr.FontSize() { Val = "18" };
                rp92sgn.Bold = new Bold();
                rr92gn.Append(rp92sgn);
                rr92gn.Append(new Text(""));
                pd92ssgn.Append(rr92gn);

                col92ssgn.Append(pd92ssgn);
                col92ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1200" }));
                row91sgn.Append(col92ssgn);

                tr.TableCell col93ssgn = new tr.TableCell();
                tr.Paragraph pd93ssgn = new tr.Paragraph();
                ParagraphProperties paragraphProperties93ss33ssgn = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd93ssgn.Append(paragraphProperties93ss33ssgn);
                Run rr93gn = new tr.Run();
                RunProperties rp93sgn = new RunProperties();
                rp93sgn.FontSize = new tr.FontSize() { Val = "18" };
                rp93sgn.Bold = new Bold();
                rr93gn.Append(rp93sgn);
                rr93gn.Append(new Text(""));
                pd93ssgn.Append(rr93gn);

                col93ssgn.Append(pd93ssgn);
                col93ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
                row91sgn.Append(col93ssgn);

                tr.TableCell col94ssgn = new tr.TableCell();
                tr.Paragraph pd94ssgn = new tr.Paragraph();
                ParagraphProperties paragraphProperties94ss33ssgn = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" },
                                              new Justification() { Val = JustificationValues.Center }
                                              );
                pd94ssgn.Append(paragraphProperties94ss33ssgn);
                Run rr94gn = new tr.Run();
                RunProperties rp94sgn = new RunProperties();
                rp94sgn.FontSize = new tr.FontSize() { Val = "18" };
                rp94sgn.Bold = new Bold();
                rr94gn.Append(rp94sgn);
                rr94gn.Append(new Text(""));
                pd94ssgn.Append(rr94gn);

                col94ssgn.Append(pd94ssgn);
                col94ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
                row91sgn.Append(col94ssgn);

                tbl9s.Append(row91sgn);

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



                //tr.TableRow row91sn = new tr.TableRow();
                //tr.TableCell col91sn = new tr.TableCell();
                //tr.Paragraph pd91sn = new tr.Paragraph();
                //ParagraphProperties paragraphProperties91ss3sn = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Left }
                //                              );
                //pd91sn.Append(paragraphProperties91ss3sn);
                //Run rr91n = new tr.Run();
                //RunProperties rp91sn = new RunProperties();
                //rp91sn.FontSize = new tr.FontSize() { Val = "20" };
                //rp91sn.Bold = new Bold();
                //rr91n.Append(rp91sn);
                //rr91n.Append(new Text(dtt1.Tables[0].Rows[0]["CustName"].ToString() + "             No. of Pkgs.  " + dtt1.Tables[0].Rows[0]["TotalPkgs"].ToString() + "No(s)") { Space = SpaceProcessingModeValues.Preserve });
                //pd91s.Append(rr91n);

                //col91sn.Append(pd91sn);
                ////col91sn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "1038" }));
                //row91sn.Append(col91sn);

                //tr.TableCell col92ssn = new tr.TableCell();
                //tr.Paragraph pd92ssn = new tr.Paragraph();
                //ParagraphProperties paragraphProperties92ss33ssn = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd92ssn.Append(paragraphProperties92ss33ssn);
                //Run rr92n = new tr.Run();
                //RunProperties rp92sn = new RunProperties();
                //rp92sn.FontSize = new tr.FontSize() { Val = "20" };
                //rp92sn.Bold = new Bold();
                //rr92n.Append(rp92sn);
                //rr92n.Append(new Text(" "));
                //pd92ssn.Append(rr92n);

                //col92ssn.Append(pd92ssn);
                ////col92ssn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "312" }));
                //row91sn.Append(col92ssn);

                //tr.TableCell col93ssn = new tr.TableCell();
                //tr.Paragraph pd93ssn = new tr.Paragraph();
                //ParagraphProperties paragraphProperties93ss33ssn = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd93ssn.Append(paragraphProperties93ss33ssn);
                //Run rr93n = new tr.Run();
                //RunProperties rp93sn = new RunProperties();
                //rp93sn.FontSize = new tr.FontSize() { Val = "20" };
                //rp93sn.Bold = new Bold();
                //rr93n.Append(rp93sn);
                //rr93n.Append(new Text(" "));
                //pd93ssn.Append(rr93n);

                //col93ssn.Append(pd93ssn);
                ////col93ssn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "264" }));
                //row91sn.Append(col93ssn);

                //tr.TableCell col94ssn = new tr.TableCell();
                //tr.Paragraph pd94ssn = new tr.Paragraph();
                //ParagraphProperties paragraphProperties94ss33ssn = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd94ssn.Append(paragraphProperties94ss33ssn);
                //Run rr94n = new tr.Run();
                //RunProperties rp94sn = new RunProperties();
                //rp94sn.FontSize = new tr.FontSize() { Val = "20" };
                //rp94sn.Bold = new Bold();
                //rr94n.Append(rp94sn);
                //rr94n.Append(new Text(" "));
                //pd94ssn.Append(rr94n);

                //col94ssn.Append(pd94ssn);
                ////col94ssn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "392" }));
                //row91sn.Append(col94ssn);

                //table1.Append(row91sn);

                //tr.TableRow row2sn = new tr.TableRow();
                //tr.TableCell col21sn = new tr.TableCell();
                //tr.Paragraph pd21sn = new tr.Paragraph();
                //ParagraphProperties paragraphProperties21ss3sn = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Left }
                //                              );
                //pd21sn.Append(paragraphProperties21ss3sn);
                //Run rr21n = new tr.Run();
                //RunProperties rp21sn = new RunProperties();
                //rp21sn.FontSize = new tr.FontSize() { Val = "20" };
                //rp21sn.Bold = new Bold();
                //rr21n.Append(rp21sn);
                //rr21n.Append(new Text(dtt1.Tables[0].Rows[0]["BillingCity"].ToString() + "             No." + dtt1.Tables[0].Rows[0]["CustPkgs"].ToString()) { Space = SpaceProcessingModeValues.Preserve });
                //pd21sn.Append(rr21n);

                //col21sn.Append(pd21sn);
                ////col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
                //row2sn.Append(col21sn);

                //tr.TableCell col22ss = new tr.TableCell();
                //tr.Paragraph pd22ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties22ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd22ss.Append(paragraphProperties22ss33ss);
                //Run rr22 = new tr.Run();
                //RunProperties rp22sfn = new RunProperties();
                //rp22sfn.FontSize = new tr.FontSize() { Val = "20" };
                //rp22sfn.Bold = new Bold();
                //rr22.Append(rp22sfn);
                //rr22.Append(new Text(" "));
                //pd22ss.Append(rr22);

                //col22ss.Append(pd22ss);
                ////col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                //row2sn.Append(col22ss);

                //tr.TableCell col23ss = new tr.TableCell();
                //tr.Paragraph pd23ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties23ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd23ss.Append(paragraphProperties23ss33ss);
                //Run rr23 = new tr.Run();
                //RunProperties rp23s = new RunProperties();
                //rp23s.FontSize = new tr.FontSize() { Val = "20" };
                //rp23s.Bold = new Bold();
                //rr23.Append(rp23s);
                //rr23.Append(new Text(" "));
                //pd23ss.Append(rr23);

                //col23ss.Append(pd23ss);
                ////col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                //row2sn.Append(col23ss);

                //tr.TableCell col24ss = new tr.TableCell();
                //tr.Paragraph pd24ss = new tr.Paragraph();
                //ParagraphProperties paragraphProperties24ss33ss = new ParagraphProperties(
                //                              new ParagraphStyleId() { Val = "No Spacing" },
                //                              new SpacingBetweenLines() { After = "0" },
                //                              new Justification() { Val = JustificationValues.Center }
                //                              );
                //pd24ss.Append(paragraphProperties24ss33ss);
                //Run rr24 = new tr.Run();
                //RunProperties rp24s = new RunProperties();
                //rp24s.FontSize = new tr.FontSize() { Val = "20" };
                //rp24s.Bold = new Bold();
                //rr24.Append(rp24s);
                //rr24.Append(new Text(" "));
                //pd24ss.Append(rr24);

                //col24ss.Append(pd24ss);
                ////col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                //row2sn.Append(col24ss);

                //table1.Append(row2sn);


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
                HeaderReference headerReference1 = new HeaderReference() { Type = HeaderFooterValues.Default, Id = "r97" };
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
                DataSet dtt = (DataSet)Session["ExportPckList2"];
                Footer f = new Footer();
                SetTableStyle(table, "6000", true, true, true, true, true, true);

                tr.Table tables = new tr.Table();
                SetTableStyle(tables, "5000", false, true, true, false, true, true);
                tr.TableRow row91ss = new tr.TableRow();
                tr.TableCell col91ss = new tr.TableCell();
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
                //RunProperties rp1212 = new RunProperties();
                RunFonts runFont_1 = new RunFonts();           // Create font
                runFont_1.Ascii = "Arial";
                rp91s.Append(runFont_1);
                rp91s.Bold = new Bold();
                rp91s.FontSize = new tr.FontSize() { Val = "18" };
                rp91s.Bold = new Bold();
                rr91.Append(rp91s);
                rr91.Append(new Text("THIRD PARTY SUPPLIER :CALDERYS REFRACTORIES LTD, GSTIN No.24AAFCA3610G1Z9, IEC CODE:1105003949:") { Space = SpaceProcessingModeValues.Preserve });
                pd91s.Append(rr91);

                col91s.Append(pd91s);
                col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "980" }));
                row91s.Append(col91s);

                //tables.Append(row91s);


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
                rp12s.FontSize = new tr.FontSize() { Val = "20" };
                ParagraphProperties pp11s = new ParagraphProperties();
                pp11s.Justification = new Justification() { Val = JustificationValues.Left };
                ParagraphProperties paragraphProperties11s = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                p11s.Append(paragraphProperties11s);
                p11s.Append(pp11s);
                r12s.Append(rp12s);
                r12s.Append((tables));
                p11s.Append(r12s);
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
                //table.Append(tr1s);


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
                RunFonts runFont_12 = new RunFonts();           // Create font
                runFont_12.Ascii = "Arial";
                rp1212.Append(runFont_12);
                rp1212.Bold = new Bold();
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
                r1212.Append(new Text("SUPPLY MEANT FOR EXPORT UNDER LETTER OF UNDERTAKING (LUT) WITHOUT PAYMENT OF INTEGRATED TAX//" + dtt.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[0] + "," + dtt.Tables[0].Rows[0]["GSTBOND"].ToString().Split(',')[1] + "  ISSUED BY GACHIBOWLI DIVISION, RANGAREDDY GST COMMISSIONARATE, VENGALARAO NAGAR STATE JURISDICTION, MIYAPUR CENTER JURISDICTION"));
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
                //runn.Append(rp22);
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
                f.Append(table);
                footerPart.Footer = f;
                SectionProperties sectionProperties1 = mainPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties1 == null)
                {
                    sectionProperties1 = new SectionProperties() { };
                    mainPart.Document.Body.Append(sectionProperties1);
                }
                FooterReference footerReference1 = new FooterReference() { Type = HeaderFooterValues.Default, Id = "r98" };

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

        private void GenerateBodyPart(MainDocumentPart mainPart, Body body)
        {
            try
            {
                Paragraph p = new Paragraph();
                Run r = new Run();
                Text t = new Text("Hello Body");
                tr.Table table = new tr.Table();
                DataSet dtt = (DataSet)Session["ExportPckList1"];
                DataSet dtt1 = (DataSet)Session["ExportPckList2"];

                SetTableStyle(table, "6000", true, true, true, true, false, true);

                int rowIndex = 1;
                string coltext = string.Empty;
                string coll = string.Empty; string getguid = string.Empty; string getSno = string.Empty;
                var gettt = string.Empty;
                //var kk = (dtt1.Tables[0].Rows[0]["FPOs"].ToString().Split(','));
                List<Tuple<string, string, string>> getFPONOS = new List<Tuple<string, string, string>>();
                foreach (DataRow rows in dtt.Tables[0].Rows)
                {
                    if (rows.ItemArray[0].ToString() != getguid || rows.ItemArray[1].ToString() != getSno)
                    {
                        getguid = rows.ItemArray[0].ToString();
                        getSno = rows.ItemArray[1].ToString();
                        gettt += rows.ItemArray[0].ToString() + ",";
                        getFPONOS.Add(Tuple.Create(getguid, rows.ItemArray[17].ToString(), rows.ItemArray[18].ToString()));

                    }
                }
                var kk = gettt.TrimEnd(',').Split(',');
                //var results = (from table1 in dtt1.Tables[0].AsEnumerable()
                //               join table2 in dtt.Tables[0].AsEnumerable() on (string)table1["FPOs"].ToString().Split(',').ToString() equals (string)table2["FPOId"]
                //               select new
                //               {
                //                   FPOs = (string)table1["FPOId"]
                //               }.FPOs);
                //Array.Sort(kk);

                tr.TableRow row1fn = new tr.TableRow();
                tr.TableCell col1fn = new tr.TableCell();
                Run run1sfn = new tr.Run();
                Paragraph p1sfn = new Paragraph();
                ParagraphProperties paragraphProperties1sfn = new ParagraphProperties(
                                             new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0", Before = "0" },
                                                                  new Justification() { Val = JustificationValues.Left }
                                              );
                //TableCellProperties cellPropertiess1fn = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                p1sfn.Append(paragraphProperties1sfn);
                RunProperties rp1sfn = new RunProperties();
                rp1sfn.Bold = new Bold();
                rp1sfn.Underline = new Underline();
                rp1sfn.FontSize = new tr.FontSize() { Val = "18" };
                RunFonts runFont_51 = new RunFonts();
                runFont_51.Ascii = "Arial";
                run1sfn.Append(runFont_51);
                run1sfn.Append(rp1sfn);
                run1sfn.AppendChild(new Text("CASE NO :"));
                p1sfn.Append(run1sfn);
                //col1fn.AppendChild(cellPropertiess1fn);

                col1fn.Append(p1sfn);
                col1fn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "970" }));
                row1fn.Append(col1fn);
                tr.TableCell col2fn = new tr.TableCell();
                tr.TableCell col3fn = new tr.TableCell();
                tr.TableCell col4fn = new tr.TableCell();
                col2fn.Append(new Paragraph(new Run(new Text(""))));
                col2fn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "348" }));
                row1fn.Append(col2fn);
                col3fn.Append(new Paragraph(new Run(new Text(""))));
                col3fn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "284" }));
                row1fn.Append(col3fn);
                col4fn.Append(new Paragraph(new Run(new Text(""))));
                col4fn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "292" }));
                row1fn.Append(col4fn);

                table.Append(row1fn);

                string SUPPGST = string.Empty;
                foreach (var arr in (getFPONOS))
                {

                    DataTable dt = new DataTable();
                    rowIndex = 1;
                    int ccc = dtt.Tables[0].Select("FPOId = '" + arr.Item1.Trim() + "' and PkgNos = '" + arr.Item2.Trim() + "' and NetWeight <> 0.00 ").Count();
                    if (ccc != 0)
                        dt = dtt.Tables[0].Select("FPOId = '" + arr.Item1.Trim() + "' and PkgNos = '" + arr.Item2.Trim() + "'  and NetWeight <> 0.00 ").CopyToDataTable();
                    else
                        dt = dtt.Tables[0].Select("FPOId = '" + arr.Item1.Trim() + "' and PkgNos = '" + arr.Item2.Trim() + "'").CopyToDataTable();
                    tr.TableRow row1f = new tr.TableRow();
                    tr.TableCell col1f = new tr.TableCell();
                    Run run1sf = new tr.Run();
                    Paragraph p1sf = new Paragraph();
                    ParagraphProperties paragraphProperties1sf = new ParagraphProperties(
                                                                    new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0", Before = "0" },
                                                                  new Justification() { Val = JustificationValues.Left },
                                                                  new ContextualSpacing() { Val = false }
                                                                    );
                    p1sf.Append(paragraphProperties1sf);
                    //RunProperties rp1sf = new RunProperties();
                    //rp1sf.Bold = new Bold();
                    //rp1sf.FontSize = new tr.FontSize() { Val = "20" };
                    //run1sf.Append(rp1sf);
                    //Run rr94 = new tr.Run();
                    RunProperties rp94s1 = new RunProperties();
                    rp94s1.FontSize = new tr.FontSize() { Val = "18" };
                    RunFonts runFont_55 = new RunFonts();           // Create font
                    runFont_55.Ascii = "Arial";
                    rp94s1.Append(runFont_55);
                    rp94s1.Bold = new Bold();
                    run1sf.Append(rp94s1);
                    run1sf.AppendChild(new Text(dt.Rows[0]["PkgNos"].ToString()));
                    p1sf.Append(run1sf);
                    //p1sf.Append(paragraphProperties1sf);

                    //col1f.AppendChild(cellPropertiess1f);


                    col1f.Append(p1sf);
                    col1f.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "948" }));
                    row1f.Append(col1f);
                    tr.TableCell col2f = new tr.TableCell();
                    tr.TableCell col3f = new tr.TableCell();
                    tr.TableCell col4f = new tr.TableCell();
                    col2f.Append(new Paragraph(new Run(new Text(""))));
                    col2f.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "322" }));
                    row1f.Append(col2f);
                    col3f.Append(new Paragraph(new Run(new Text(""))));
                    col3f.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "284" }));
                    row1f.Append(col3f);
                    col4f.Append(new Paragraph(new Run(new Text(""))));
                    col4f.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "292" }));
                    row1f.Append(col4f);

                    table.Append(row1f);

                    tr.TableRow row1 = new tr.TableRow();
                    tr.TableCell col1 = new tr.TableCell();
                    Run run1s = new tr.Run();
                    Paragraph p1s = new Paragraph();
                    ParagraphProperties paragraphProperties1s = new ParagraphProperties(
                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0", Before = "0" },
                                                                  new Justification() { Val = JustificationValues.Left },
                                                                  new ContextualSpacing() { Val = false }
                                                  );
                    //TableCellProperties cellPropertiess1 = new TableCellProperties(new NoWrap { Val = OnOffOnlyValues.On });
                    p1s.Append(paragraphProperties1s);
                    //RunProperties rp1s = new RunProperties();
                    //rp1s.Bold = new Bold();
                    //rp1s.FontSize = new tr.FontSize() { Val = "20" };
                    //run1s.Append(rp1s);
                    RunProperties rp42s1 = new RunProperties();
                    rp42s1.FontSize = new tr.FontSize() { Val = "18" };
                    RunFonts runFonts = new RunFonts();           // Create font
                    runFonts.Ascii = "Arial";
                    rp42s1.Append(runFonts);
                    run1s.Append(rp42s1);

                    coltext = "  " + dt.Rows[0]["FPONmbr"].ToString();
                    run1s.AppendChild(new Text(coltext) { Space = SpaceProcessingModeValues.Preserve });
                    p1s.Append(run1s);
                    //col1.AppendChild(cellPropertiess1);
                    SUPPGST += arr.Item3 + ",";
                    col1.Append(p1s);
                    row1.Append(col1);
                    tr.TableCell col2 = new tr.TableCell();
                    tr.TableCell col3 = new tr.TableCell();
                    tr.TableCell col4 = new tr.TableCell();
                    col2.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col2);
                    if (Convert.ToDecimal(dt.Rows[0]["NetWeight"].ToString()) != 0)
                        col3.Append(new Paragraph(new Run(new Text(dt.Rows[0]["NetWeight"].ToString()))));
                    else
                        col3.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col3);
                    if (Convert.ToDecimal(dt.Rows[0]["GrWeight"].ToString()) != 0)
                        col4.Append(new Paragraph(new Run(new Text(dt.Rows[0]["GrWeight"].ToString()))));
                    else
                        col4.Append(new Paragraph(new Run(new Text(""))));
                    row1.Append(col4);
                    int vccc = dtt.Tables[0].Select("FPOId = '" + arr.Item1.Trim() + "' and PkgNos = '" + arr.Item2.Trim() + "'").Count();
                    table.Append(row1);
                    foreach (DataRow row in dtt.Tables[0].Rows)
                    {
                        if (arr.Item1.Trim() == row["FPOId"].ToString() && arr.Item2.Trim() == row["PkgNos"].ToString())
                            //rowIndex += (int)dtt.Rows.IndexOf(row);


                            if (rowIndex == 1 && coltext.ToString().Trim() != row["FPONmbr"].ToString().Trim())
                            {
                                rowIndex = 1;

                                //coltext = ""; 
                            }
                            else
                            {
                                if (arr.Item1.Trim() == row["FPOId"].ToString() && arr.Item2.Trim() == row["PkgNos"].ToString())
                                {
                                    rowIndex++;
                                    int vcount = dt.Rows.Count;
                                    tr.TableRow row4s = new tr.TableRow();
                                    tr.TableCell col4s = new tr.TableCell();
                                    tr.Paragraph pd4s = new tr.Paragraph();
                                    ParagraphProperties paragraphProperties4ss3s = new ParagraphProperties(
                                                                  new ParagraphStyleId() { Val = "No Spacing" },
                                                                  new SpacingBetweenLines() { After = "0", Before = "0" },
                                                                  new Justification() { Val = JustificationValues.Left },
                                                                  new ContextualSpacing() { Val = false }
                                                                  );
                                    ParagraphProperties paragraphProperties4ss3sss = new ParagraphProperties(
                                                                  new Justification() { Val = JustificationValues.Left }
                                                                  );
                                    if (vccc == rowIndex - 1)
                                        pd4s.Append(paragraphProperties4ss3sss);
                                    else
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
                                    //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1038" }));
                                    row4s.Append(col4s);

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
                                    RunFonts runFonts5 = new RunFonts();           // Create font
                                    runFonts5.Ascii = "Arial";
                                    rp42s.Append(runFonts5);
                                    rr42.Append(rp42s);
                                    rr42.Append(new Text(row["UnitsNm"].ToString()));
                                    pd42ss.Append(rr42);

                                    col42ss.Append(pd42ss);
                                    //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
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
                                    rr43.Append(new Text(""));
                                    pd43ss.Append(rr43);

                                    col43ss.Append(pd43ss);
                                    //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
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
                                    rp44s.Bold = new Bold();
                                    RunFonts runFont1s = new RunFonts();           // Create font
                                    runFont1s.Ascii = "Arial";
                                    rp44s.Append(runFont1s);
                                    rr44.Append(rp44s);
                                    rr44.Append(new Text(""));
                                    pd44ss.Append(rr44);

                                    col44ss.Append(pd44ss);
                                    //col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "312" }));
                                    row4s.Append(col44ss);

                                    table.Append(row4s);
                                    coll = "";
                                }
                            }
                    }

                }

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
                run5ss.Append(new Text("TOTAL WEIGHT:") { Space = SpaceProcessingModeValues.Preserve });
                p5ss.Append(run5ss);

                col5s1.Append(p5ss);
                row5s.Append(col5s1);
                tr.TableCell col5s2 = new tr.TableCell();
                tr.TableCell col5s3 = new tr.TableCell();
                tr.TableCell col5s4 = new tr.TableCell();
                col5s2.Append(new Paragraph(new Run(new Text(""))));
                row5s.Append(col5s2);

                Paragraph p54ss5 = new Paragraph();
                ParagraphProperties paragraphProperties54ss5sss5 = new ParagraphProperties(
                                                   new ParagraphStyleId() { Val = "No Spacing" },
                                                   new SpacingBetweenLines() { After = "0", Before = "0" },
                                                   new Justification() { Val = JustificationValues.Center }
                                                   );
                p54ss5.Append(paragraphProperties54ss5sss5);
                Run run5ss25 = new tr.Run();
                RunProperties rp5ss25 = new RunProperties();
                rp5ss25.FontSize = new tr.FontSize() { Val = "18" };
                rp5ss25.Bold = new Bold();
                RunFonts runFont5ss25 = new RunFonts();           // Create font
                runFont5ss25.Ascii = "Arial";
                rp5ss25.Append(runFont5ss25);
                run5ss25.Append(rp5ss25);

                //var Calc_Data = (from mm in dtt.Tables[0].AsEnumerable()
                //                 select new
                //                 {
                //                     Tot = Convert.ToDecimal(mm.Field<decimal>("Quantity")) * Convert.ToDecimal(mm.Field<decimal>("ExDUTYPercent")),
                //                     TotQty = Convert.ToDecimal(mm.Field<string>("Quantity"))
                //                 }).ToList();



                run5ss25.Append(new Text(dtt.Tables[0].Compute("Sum(NetWeight)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
                p54ss5.Append(run5ss25);

                col5s3.Append(p54ss5);
                //col5s3.Append(new Paragraph(new Run(new Text(dtt.Tables[0].Rows[0]["NetWeight"].ToString()))));
                row5s.Append(col5s3);
                Paragraph p54ss = new Paragraph();
                ParagraphProperties paragraphProperties54ss5sss = new ParagraphProperties(
                                                   new ParagraphStyleId() { Val = "No Spacing" },
                                                   new SpacingBetweenLines() { After = "0", Before = "0" },
                                                   new Justification() { Val = JustificationValues.Center }
                                                   );
                p54ss.Append(paragraphProperties54ss5sss);
                Run run5ss2 = new tr.Run();
                RunProperties rp5ss2 = new RunProperties();
                rp5ss2.FontSize = new tr.FontSize() { Val = "18" };
                rp5ss2.Bold = new Bold();
                RunFonts runFont5ss2 = new RunFonts();           // Create font
                runFont5ss2.Ascii = "Arial";
                rp5ss2.Append(runFont5ss2);
                run5ss2.Append(rp5ss2);
                run5ss2.Append(new Text(dtt.Tables[0].Compute("Sum(GrWeight)", "").ToString()) { Space = SpaceProcessingModeValues.Preserve });
                p54ss.Append(run5ss2);

                //p54ss.Append(new Run(new Text(dtt.Tables[0].Rows[0]["GrWeight"].ToString()) { Space = SpaceProcessingModeValues.Preserve }));
                col5s4.Append(p54ss);
                row5s.Append(col5s4);

                table.Append(row5s);
                tr.Table tbl2 = new tr.Table();
                SetTableStyle(tbl2, "5000", false, false, true, true, true, false);
                tr.TableRow row6s = new tr.TableRow();
                tr.TableCell col6s1 = new tr.TableCell();
                tr.TableCell col6s2 = new tr.TableCell();
                tr.TableCell col6s3 = new tr.TableCell();
                tr.TableCell col6s4 = new tr.TableCell();
                Run run6ss = new tr.Run();

                tr.TableRow tr12 = new tr.TableRow();

                tr.TableCell tc1112 = new tr.TableCell();
                Paragraph p1112 = new Paragraph();
                Run r1212 = new Run();
                RunProperties rp1212 = new RunProperties();
                rp1212.Bold = new Bold();
                rp1212.FontSize = new tr.FontSize() { Val = "20" };
                ParagraphProperties pp1112 = new ParagraphProperties();
                pp1112.Justification = new Justification() { Val = JustificationValues.Left };
                ParagraphProperties paragraphProperties1112 = new ParagraphProperties(
                                              new ParagraphStyleId() { Val = "No Spacing" },
                                              new SpacingBetweenLines() { After = "0" }
                                              );
                TableCellProperties cellOneProperties = new TableCellProperties();
                cellOneProperties.Append(new HorizontalMerge()
                {
                    Val = MergedCellValues.Restart
                });

                TableCellProperties cellTwoProperties = new TableCellProperties();
                cellTwoProperties.Append(new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                });
                TableCellProperties cellThreeProperties = new TableCellProperties();
                cellThreeProperties.Append(new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                });
                TableCellProperties cellFourProperties = new TableCellProperties();
                cellFourProperties.Append(new HorizontalMerge()
                {
                    Val = MergedCellValues.Continue
                });

                p1112.Append(paragraphProperties1112);
                p1112.Append(pp1112);
                r1212.Append(rp1212);
                r1212.Append(new Text("THIRD PARTY SUPPLIER :" + SUPPGST.TrimEnd(',').ToString()));
                p1112.Append(r1212);
                tc1112.Append(p1112);
                tr12.Append(tc1112);

                tr.TableCell tc1212 = new tr.TableCell();
                Paragraph p1212 = new Paragraph();
                tc1212.Append(p1212);
                tr12.Append(tc1212);
                tbl2.Append(tr12);
                col6s1.Append(new Paragraph(new Run(tbl2)));
                row6s.Append(col6s1);
                col6s2.Append(new Paragraph(new Run(new Text(""))));
                row6s.Append(col6s2);
                col6s3.Append(new Paragraph(new Run(new Text(""))));
                row6s.Append(col6s3);
                col6s4.Append(new Paragraph(new Run(new Text(""))));
                row6s.Append(col6s4);
                col6s1.Append(cellOneProperties);
                col6s2.Append(cellTwoProperties);
                col6s3.Append(cellThreeProperties);
                col6s4.Append(cellFourProperties);

                table.Append(row6s);
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

        #region Customize Header and footer
        

        public void GenerateHeaderPartContent(HeaderPart hpart)
        {
            #region Header
            // add/modify header values
            Header h = new Header();
            tr.Table table1 = new tr.Table();
            // Add a Paragraph and a Run with the specified Text 
            DataSet dtt1 = (DataSet)Session["ExportPckList1"];
            DataSet dtt = (DataSet)Session["ExportPckList2"];
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
            rp12s.FontSize = new tr.FontSize() { Val = "20" };
            RunFonts runFont_1 = new RunFonts();           // Create font
            runFont_1.Ascii = "Arial";
            rp12s.Append(runFont_1);
            ParagraphProperties pp11s = new ParagraphProperties();
            pp11s.Justification = new Justification() { Val = JustificationValues.Center };
            ParagraphProperties paragraphProperties11s = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            p11s.Append(paragraphProperties11s);
            r12s.Append(rp12s);
            r12s.Append(new Text("PACKING LIST"));
            p11s.Append(r12s);
            p11s.Append(pp11s);
            tc11s.Append(p11s);
            tr1s.Append(tc11s);
            TableRowProperties tableRowProperties1s = new TableRowProperties();
            TableRowHeight tableRowHeight1s = new TableRowHeight() { Val = (UInt32Value)34U };

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
            RunProperties rp12sssa = new RunProperties();
            rp12sssa.Bold = new Bold();
            rp12sssa.FontSize = new tr.FontSize() { Val = "18" };
            run.Append(rp12sssa);
            run.AppendChild(new Text("Exporter: "));
            run.AppendChild(new Break());
            run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyName"].ToString()));
            run.AppendChild(new Break());
            run.AppendChild(new Text(dtt.Tables[0].Rows[0]["CompanyDetails"].ToString()));

            //run.AppendChild(new Break());
            //run.AppendChild(new Text("KONDAPUR, HYDERABAD, TELANGANA, INDIA-500081"));
            run.AppendChild(new Break());
            rn.RunProperties = new tr.RunProperties((new Bold()));
            rn.RunProperties.FontSize = new tr.FontSize() { Val = "18" };
            //if (dtt != null)
            //    if (dtt.Tables.Count > 1 && dtt.Tables[1].Rows.Count > 0 && dtt.Tables[1].Rows[0]["CompanyName"].ToString().ToLower().Contains("volta"))
            //    {
            RunFonts runFont_3 = new RunFonts();           // Create font
            runFont_3.Ascii = "Arial";
            rn.Append(runFont_3);
            rn.AppendChild(new Text("IEC Code No. 0996008306"));
            rn.AppendChild(new Break());
            //    }
            RunProperties rp12sssad = new RunProperties();
            rp12sssad.FontSize = new tr.FontSize() { Val = "18" };
            rn.Append(rp12sssad);
            rn.AppendChild(new Text("GSTIN No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]));
            rn.AppendChild(new Break());
            rn.AppendChild(new Text("CIN No: " + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[1]));
            //rn.AppendChild(rp12sssa);
            Paragraph p21s = new Paragraph(run);
            p21s.Append(rn);
            ParagraphProperties paragraphProperties1s = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" }
                                          );
            p21s.Append(paragraphProperties1s);
            tc21s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "5800" }));
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
            RunProperties rp12sw3c = new RunProperties();
            Run rrrs = new Run();
            rp12sw3c.FontSize = new tr.FontSize() { Val = "18" };
            RunFonts runFont8s12 = new RunFonts();           // Create font
            runFont8s12.Ascii = "Arial";
            rp12sw3c.Append(runFont8s12);
            rrrs.Append(rp12sw3c);
            rrrs.AppendChild(new Text(dtt.Tables[0].Rows[0]["PrfrmInvNoDT"].ToString()));
            pd.Append(rrrs);
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
            RunProperties rp12sw3ce = new RunProperties();
            Run rrrse = new Run();
            rp12sw3ce.FontSize = new tr.FontSize() { Val = "18" };
            rrrse.Append(rp12sw3ce);
            RunFonts runFont8s121 = new RunFonts();           // Create font
            runFont8s121.Ascii = "Arial";
            rp12sw3ce.Append(runFont8s121);
            rrrse.AppendChild(new Text("GSTIN No:" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[0]));
            pd2.Append(rrrse);
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
            rn3t.AppendChild(new Text("STATE :" + dtt.Tables[0].Rows[0]["GSTCIN"].ToString().Split(',')[2]));
            pd22.Append(rn3t);
            tr.TableCell col222 = new tr.TableCell();
            col222.Append(pd22);
            row22.Append(col222);
            tbl22.Append(row22);
            tr.TableCell col2s = new tr.TableCell();
            col2s.Append(new Paragraph(new Run(tbl22)));
            row2s.Append(col2s);
            tbl2.Append(row2s);

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
            RunProperties r4_ = new RunProperties();
            Run rn4t = new tr.Run();
            rn4t.Append(r4_);
            r4_.FontSize = new tr.FontSize() { Val = "18" };
            RunFonts runFont8sf = new RunFonts();           // Create font
            runFont8sf.Ascii = "Arial";
            r4_.Append(runFont8sf);
            rn4t.AppendChild(new Text("Reverse Charge Applicable:No"));
            pd3.Append(rn4t);
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
            RunProperties r5_ = new RunProperties();
            Run rn5t = new tr.Run();
            rn5t.Append(r5_);
            r5_.FontSize = new tr.FontSize() { Val = "18" };
            RunFonts runFont5se = new RunFonts();           // Create font
            runFont5se.Ascii = "Arial";
            r5_.Append(runFont5se);
            //runrr3.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[1]));
            if (dtt.Tables[0].Rows[0]["EUCIT"].ToString() != "")
            {
                if (dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',').Count() == 2)
                    rn5t.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[1]));
                else if (dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',').Count() == 1)
                    rn5t.AppendChild(new Text("END USE CODE" + dtt.Tables[0].Rows[0]["EUCIT"].ToString().Split(',')[0]));
                else
                    rn5t.AppendChild(new Text("END USE CODE "));
            }
            else
                rn5t.AppendChild(new Text("END USE CODE "));
            pd33.Append(rn5t);
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
            RunProperties rp12swe4 = new RunProperties();
            Run runrr4 = new Run();
            //rp12swe4.FontSize = new tr.FontSize() { Val = "18" };
            runrr4.Append(rp12swe4);
            if (dtt.Tables[0].Rows[0]["FPONOs"].ToString().Length > 70)
            {
                rp12swe4.FontSize = new tr.FontSize() { Val = "16" };
                RunFonts runFont_4 = new RunFonts();           // Create font
                runFont_4.Ascii = "Arial";
                rp12swe4.Append(runFont_4);
                string txtsplit = string.Empty;
                string[] a = SplitByLenght("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString(), 70);
                for (int i = 0; i < a.Length; i++)
                {
                    runrr4.AppendChild(new Text(a[i]));
                    if (i != (a.Length - 1))
                        runrr4.AppendChild(new Break());
                }
                //runrr4.AppendChild(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString()));
            }
            else
            {
                rp12swe4.FontSize = new tr.FontSize() { Val = "16" };
                RunFonts runFont_4 = new RunFonts();           // Create font
                runFont_4.Ascii = "Arial";
                rp12swe4.Append(runFont_4);
                runrr4.AppendChild(new Text("Buyer's Order No.:FPO No(s)." + dtt.Tables[0].Rows[0]["FPONOs"].ToString()));
            }

            pd4.Append(runrr4);
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
            //rn5.AppendChild(new Break());
            rn5.AppendChild(new Text(dtt.Tables[0].Rows[0]["CustShipAdd"].ToString()));
            //rn5.AppendChild(new Break());
            rn5.AppendChild(new Text("PH:" + dtt.Tables[0].Rows[0]["Phone"].ToString()));

            pd5.Append(rn5);
            tr.TableCell col5 = new tr.TableCell();
            col5.Append(pd5);
            row5.Append(col5);
            tbl2.Append(row5);


            tc22s.Append(new Paragraph(new Run(tbl2)));
            tr2s.Append(tc22s);
            table1.Append(tr2s);

             

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
            RunFonts runFont_17 = new RunFonts();           // Create font
            runFont_17.Ascii = "Arial";
            rp81s.Append(runFont_17);
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
            SetTableStyle(tbl9s, "5000", false, false, false, false, true, true);
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
            RunFonts runFont_18 = new RunFonts();           // Create font
            runFont_18.Ascii = "Arial";
            rp91s.Append(runFont_18);
            rp91s.Bold = new Bold();
            rr91.Append(rp91s);
            rr91.Append(new Text("Marks & Nos.No     No & Kind of Pkg.      Description of Goods"));
            pd91s.Append(rr91);

            col91s.Append(pd91s);
            //col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3270" }));
            col91s.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3690" }));
            row91s.Append(col91s);

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
            RunFonts runFont_20 = new RunFonts();           // Create font
            runFont_20.Ascii = "Arial";
            rp92s.Append(runFont_20);
            rp92s.Bold = new Bold();
            rr92.Append(rp92s);
            rr92.Append(new Text("Quantity"));
            pd92ss.Append(rr92);

            col92ss.Append(pd92ss);
            col92ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1280" }));
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
            RunFonts runFont_21 = new RunFonts();           // Create font
            runFont_21.Ascii = "Arial";
            rp93s.Append(runFont_21);
            rp93s.Bold = new Bold();
            rr93.Append(rp93s);
            rr93.Append(new Text("Net Weight(KGS)"));
            pd93ss.Append(rr93);

            col93ss.Append(pd93ss);
            col93ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "270" }));
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
            RunFonts runFont_22 = new RunFonts();           // Create font
            runFont_22.Ascii = "Arial";
            rp94s.Append(runFont_22);
            rp94s.Bold = new Bold();
            rr94.Append(rp94s);
            rr94.Append(new Text("Gross Weight(KGS)"));
            pd94ss.Append(rr94);

            col94ss.Append(pd94ss);
            col94ss.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "330" }));
            row91s.Append(col94ss);
            TableProperties tblProp = new TableProperties(
                    new TableBorders(
                        new TopBorder()
                        {
                            Val =
                            new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new BottomBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new LeftBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new RightBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.Single),
                        },
                        new InsideHorizontalBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.None),
                        },
                        new InsideVerticalBorder()
                        {
                            Val =
                                new EnumValue<BorderValues>(BorderValues.Single),
                        }
                    )
                );
            // Append the TableProperties object to the empty table.
            tbl9s.AppendChild<TableProperties>(tblProp);
            tbl9s.Append(row91s);

            tr.TableRow row91sg = new tr.TableRow();
            tr.TableCell col91sg = new tr.TableCell();
            tr.Paragraph pd91sg = new tr.Paragraph();
            ParagraphProperties paragraphProperties91ss3sg = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Left }
                                          );
            pd91sg.Append(paragraphProperties91ss3sg);
            Run rr91g = new tr.Run();
            RunProperties rp91sg = new RunProperties();
            rp91sg.FontSize = new tr.FontSize() { Val = "18" };
            RunFonts runFont_221 = new RunFonts();
            runFont_221.Ascii = "Arial";
            rr91g.Append(runFont_221);
            rp91sg.Bold = new Bold();
            rr91g.Append(rp91sg);
            ///Changed from 'TotalPkgs' to 'NoOfPkgs' because Total packages count is not coming as expected by Dinesh 14-05-2019
            rr91g.Append(new Text(dtt1.Tables[0].Rows[0]["CustName"].ToString() + "                     No. of Pkgs.  " + dtt1.Tables[0].Rows[0]["NoOfPkgs"].ToString() + "No(s)"));
            pd91sg.Append(rr91g);

            col91sg.Append(pd91sg);
            col91sg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3080" }));
            row91sg.Append(col91sg);

            tr.TableCell col92ssg = new tr.TableCell();
            tr.Paragraph pd92ssg = new tr.Paragraph();
            ParagraphProperties paragraphProperties92ss33ssg = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd92ssg.Append(paragraphProperties92ss33ssg);
            Run rr92g = new tr.Run();
            RunProperties rp92sg = new RunProperties();
            rp92sg.FontSize = new tr.FontSize() { Val = "18" };
            rp92sg.Bold = new Bold();
            rr92g.Append(rp92sg);
            rr92g.Append(new Text(""));
            pd92ssg.Append(rr92g);

            col92ssg.Append(pd92ssg);
            col92ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1200" }));
            row91sg.Append(col92ssg);

            tr.TableCell col93ssg = new tr.TableCell();
            tr.Paragraph pd93ssg = new tr.Paragraph();
            ParagraphProperties paragraphProperties93ss33ssg = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd93ssg.Append(paragraphProperties93ss33ssg);
            Run rr93g = new tr.Run();
            RunProperties rp93sg = new RunProperties();
            rp93sg.FontSize = new tr.FontSize() { Val = "18" };
            rp93sg.Bold = new Bold();
            rr93g.Append(rp93sg);
            rr93g.Append(new Text(""));
            pd93ssg.Append(rr93g);

            col93ssg.Append(pd93ssg);
            col93ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
            row91sg.Append(col93ssg);

            tr.TableCell col94ssg = new tr.TableCell();
            tr.Paragraph pd94ssg = new tr.Paragraph();
            ParagraphProperties paragraphProperties94ss33ssg = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd94ssg.Append(paragraphProperties94ss33ssg);
            Run rr94g = new tr.Run();
            RunProperties rp94sg = new RunProperties();
            rp94sg.FontSize = new tr.FontSize() { Val = "18" };
            rp94sg.Bold = new Bold();
            rr94g.Append(rp94sg);
            rr94g.Append(new Text(""));
            pd94ssg.Append(rr94g);

            col94ssg.Append(pd94ssg);
            col94ssg.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
            row91sg.Append(col94ssg);

            tbl9s.Append(row91sg);


            tr.TableRow row91sgn = new tr.TableRow();
            tr.TableCell col91sgn = new tr.TableCell();
            tr.Paragraph pd91sgn = new tr.Paragraph();
            ParagraphProperties paragraphProperties91ss3sgn = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Left }
                                          );
            pd91sgn.Append(paragraphProperties91ss3sgn);
            //Run rr91gn = new tr.Run();
            //RunProperties rp91sgn = new RunProperties();
            //rp91sgn.FontSize = new tr.FontSize() { Val = "18" };
            //rp91sgn.Bold = new Bold();
            //rr91gn.Append(rp91sgn);
            Run rr91gn = new tr.Run();
            RunProperties rp91sgn = new RunProperties();
            rp91sgn.FontSize = new tr.FontSize() { Val = "18" };
            RunFonts runFont_2241 = new RunFonts();           // Create font
            runFont_2241.Ascii = "Arial";
            rr91gn.Append(runFont_2241);
            rp91sgn.Bold = new Bold();
            rr91gn.Append(rp91sgn);
            rr91gn.Append(new Text(dtt1.Tables[0].Rows[0]["BillingCity"].ToString() + "             No." + dtt1.Tables[0].Rows[0]["CustPkgs"].ToString()) { Space = SpaceProcessingModeValues.Preserve });
            pd91sgn.Append(rr91gn);

            col91sgn.Append(pd91sgn);
            col91sgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "3080" }));
            row91sgn.Append(col91sgn);

            tr.TableCell col92ssgn = new tr.TableCell();
            tr.Paragraph pd92ssgn = new tr.Paragraph();
            ParagraphProperties paragraphProperties92ss33ssgn = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd92ssgn.Append(paragraphProperties92ss33ssgn);
            Run rr92gn = new tr.Run();
            RunProperties rp92sgn = new RunProperties();
            rp92sgn.FontSize = new tr.FontSize() { Val = "18" };
            rp92sgn.Bold = new Bold();
            rr92gn.Append(rp92sgn);
            rr92gn.Append(new Text(""));
            pd92ssgn.Append(rr92gn);

            col92ssgn.Append(pd92ssgn);
            col92ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "1200" }));
            row91sgn.Append(col92ssgn);

            tr.TableCell col93ssgn = new tr.TableCell();
            tr.Paragraph pd93ssgn = new tr.Paragraph();
            ParagraphProperties paragraphProperties93ss33ssgn = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd93ssgn.Append(paragraphProperties93ss33ssgn);
            Run rr93gn = new tr.Run();
            RunProperties rp93sgn = new RunProperties();
            rp93sgn.FontSize = new tr.FontSize() { Val = "18" };
            rp93sgn.Bold = new Bold();
            rr93gn.Append(rp93sgn);
            rr93gn.Append(new Text(""));
            pd93ssgn.Append(rr93gn);

            col93ssgn.Append(pd93ssgn);
            col93ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
            row91sgn.Append(col93ssgn);

            tr.TableCell col94ssgn = new tr.TableCell();
            tr.Paragraph pd94ssgn = new tr.Paragraph();
            ParagraphProperties paragraphProperties94ss33ssgn = new ParagraphProperties(
                                          new ParagraphStyleId() { Val = "No Spacing" },
                                          new SpacingBetweenLines() { After = "0" },
                                          new Justification() { Val = JustificationValues.Center }
                                          );
            pd94ssgn.Append(paragraphProperties94ss33ssgn);
            Run rr94gn = new tr.Run();
            RunProperties rp94sgn = new RunProperties();
            rp94sgn.FontSize = new tr.FontSize() { Val = "18" };
            rp94sgn.Bold = new Bold();
            rr94gn.Append(rp94sgn);
            rr94gn.Append(new Text(""));
            pd94ssgn.Append(rr94gn);

            col94ssgn.Append(pd94ssgn);
            col94ssgn.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "400" }));
            row91sgn.Append(col94ssgn);

            tbl9s.Append(row91sgn);

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

             

            h.Append(table1); 
            hpart.Header = h;

            #endregion
        }

        static void GenerateFooterPartContent(FooterPart fpart)
        {
            Footer footer1 = new Footer();
            Paragraph paragraph1 = new Paragraph();
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Footer" };
            paragraphProperties1.Append(paragraphStyleId1);
            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = "";
            run1.Append(text1);
            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            footer1.Append(paragraph1);
            fpart.Footer = footer1;
        }

        public void AddHeaderAndFooter(MainDocumentPart mainPart, HeaderPart ht, FooterPart ft)
        {
            // Delete the existing header and footer parts
            //mainPart.DeleteParts(mainPart.HeaderParts);
            //mainPart.DeleteParts(mainPart.FooterParts);

            // Create a new header and footer parts for first and default pages
            HeaderPart headerPart = ht;
            FooterPart footerPart = ft;
            HeaderPart defaultHeaderPart = mainPart.AddNewPart<HeaderPart>();
            //FooterPart defaultFooterPart = mainPart.AddNewPart<FooterPart>();

            //GenerateHeaderPartContentFirst(ht);
            GenerateHeaderPartContent(defaultHeaderPart);
            //GenerateFooterPartContentFirst(ft);
            //GenerateFooterPartContent(defaultFooterPart);

            // Get Id of the headerPart and footer parts
            string headerPartId = mainPart.GetIdOfPart(headerPart);
            string footerPartId = mainPart.GetIdOfPart(footerPart);
            string defaultHeaderPartId = mainPart.GetIdOfPart(defaultHeaderPart);
            string defaultFooterPartId = mainPart.GetIdOfPart(ft);

            // Get SectionProperties and Replace HeaderReference and FooterRefernce with new Id
            IEnumerable<SectionProperties> sections = mainPart.Document.Body.Elements<SectionProperties>();

            foreach (var section in sections)
            {
                // Delete existing references to headers and footers
                section.RemoveAllChildren<HeaderReference>();
                section.RemoveAllChildren<FooterReference>();
                section.RemoveAllChildren<TitlePage>();

                // Create the new header and footer reference node
                section.PrependChild<HeaderReference>(new HeaderReference() { Id = headerPartId, Type = HeaderFooterValues.First });
                section.PrependChild<HeaderReference>(new HeaderReference() { Id = defaultHeaderPartId, Type = HeaderFooterValues.Default });
                section.PrependChild<FooterReference>(new FooterReference() { Id = footerPartId, Type = HeaderFooterValues.First });
                section.PrependChild<FooterReference>(new FooterReference() { Id = defaultFooterPartId, Type = HeaderFooterValues.Default });
                section.PrependChild<TitlePage>(new TitlePage());
            }
            mainPart.Document.Save();
        }

        static void GenerateHeaderPartContentFirst(HeaderPart part)
        {
            Header header1 = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            header1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            header1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            header1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            header1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            header1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            header1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            header1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            header1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            header1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            header1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            header1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            header1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            header1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            header1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            header1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };
            Paragraph paragraph2 = new Paragraph();
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Header" };

            paragraphProperties1.Append(paragraphStyleId1);

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = "First Page Header";

            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            header1.Append(paragraph1);

            part.Header = header1;
        }

        static void GenerateFooterPartContentFirst(FooterPart part)
        {
            Footer footer1 = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            footer1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "00164C17", RsidRunAdditionDefault = "00164C17" };

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Footer" };

            paragraphProperties1.Append(paragraphStyleId1);

            Run run1 = new Run();
            Text text1 = new Text();
            text1.Text = "First Footer";

            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);

            footer1.Append(paragraph1);

            part.Footer = footer1;
        }


        #endregion

        # endregion
    }
}
