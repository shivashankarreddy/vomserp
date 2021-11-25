using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using BAL;

namespace VOMS_ERP.Enquiries
{
    public class CustomStyleSheet : Stylesheet
    {
        public Stylesheet CustomStyleshet()
        {
            Stylesheet ss = new Stylesheet();
            try
            {
                //Stylesheet ss = new Stylesheet();
                Fonts fts = new Fonts();
                DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                FontName ftn = new FontName();
                ftn.Val = StringValue.FromString("Calibri");
                FontSize ftsz = new FontSize();
                Bold bd = new Bold();
                ftsz.Val = DoubleValue.FromDouble(11);
                ft.FontName = ftn;
                ft.FontSize = ftsz;
                fts.Append(ft);

                //Bold
                ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                ftn = new FontName();
                Bold bdd = new Bold();
                ftn.Val = StringValue.FromString("Calibri");
                ftsz = new FontSize();
                ftsz.Val = DoubleValue.FromDouble(11);
                ft.FontName = ftn;
                ft.FontSize = ftsz;
                ft.Bold = bdd;
                fts.Append(ft);

                //UnderLine
                ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                ftn = new FontName();
                Underline ul = new Underline();
                ftn.Val = StringValue.FromString("Calibri");
                ftsz = new FontSize();
                ftsz.Val = DoubleValue.FromDouble(11);
                ft.FontName = ftn;
                ft.FontSize = ftsz;
                ft.Underline = ul;
                fts.Append(ft);

                fts.Count = UInt32Value.FromUInt32((uint)fts.ChildElements.Count);

                //Alignment of the Text in the Cell.
                Alignment align = new Alignment();
                align.Horizontal = HorizontalAlignmentValues.Left;
                align.WrapText = true;
                //CellFormat cf = new CellFormat() { Alignment = align };


                //Alignment of the Text in the Cell.
                Alignment align1 = new Alignment();
                align1.Horizontal = HorizontalAlignmentValues.Left;
                align1.Vertical = VerticalAlignmentValues.Top;
                //align1.WrapText = true;
                //CellFormat cf = new CellFormat() { Alignment = align };

                //Alignment of the Text in the Cell.
                Alignment align2 = new Alignment();
                align2.Horizontal = HorizontalAlignmentValues.Right;
                align2.Vertical = VerticalAlignmentValues.Top;
                //align1.WrapText = true;
                //CellFormat cf = new CellFormat() { Alignment = align };

                Alignment align3 = new Alignment();
                align3.Horizontal = HorizontalAlignmentValues.Center;
                align3.Vertical = VerticalAlignmentValues.Center;


                Fills fills = new Fills();
                Fill fill;
                PatternFill patternFill;
                fill = new Fill();
                patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.None;
                fill.PatternFill = patternFill;
                fills.Append(fill);

                fill = new Fill();
                patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.Gray125;
                fill.PatternFill = patternFill;
                fills.Append(fill);

                fill = new Fill();
                patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.Solid;
                patternFill.ForegroundColor = new ForegroundColor();
                patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString("00ff9728");
                patternFill.BackgroundColor = new BackgroundColor();
                patternFill.BackgroundColor.Rgb = HexBinaryValue.FromString("00ff9728");//patternFill.ForegroundColor.Rgb;
                fill.PatternFill = patternFill;
                fills.Append(fill);

                fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);

                Borders borders = new Borders();
                Border border = new Border();
                border.LeftBorder = new LeftBorder();
                //border.LeftBorder.Style = BorderStyleValues.Thin;
                border.RightBorder = new RightBorder();
                //border.RightBorder.Style = BorderStyleValues.Thin;
                border.TopBorder = new TopBorder();
                // border.TopBorder.Style = BorderStyleValues.Thin;
                border.BottomBorder = new BottomBorder();
                //border.BottomBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);

                //Boarder Index 1
                border = new Border();
                border.LeftBorder = new LeftBorder();
                border.LeftBorder.Style = BorderStyleValues.Thin;
                border.RightBorder = new RightBorder();
                border.RightBorder.Style = BorderStyleValues.Thin;
                border.TopBorder = new TopBorder();
                border.TopBorder.Style = BorderStyleValues.Thin;
                border.BottomBorder = new BottomBorder();
                border.BottomBorder.Style = BorderStyleValues.Thin;
                border.VerticalBorder = new VerticalBorder();
                border.VerticalBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);


                //Boarder Index 2
                border = new Border();
                border.LeftBorder = new LeftBorder();
                border.RightBorder = new RightBorder();
                border.TopBorder = new TopBorder();
                border.TopBorder.Style = BorderStyleValues.Thin;
                border.BottomBorder = new BottomBorder();
                border.BottomBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);


                //Boarder Index 3
                border = new Border();
                border.LeftBorder = new LeftBorder();
                border.RightBorder = new RightBorder();
                //border.TopBorder = new TopBorder();
                //border.TopBorder.Style = BorderStyleValues.Medium;
                border.BottomBorder = new BottomBorder();
                border.BottomBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);

                //Boder index =4
                border = new Border();
                //border.LeftBorder = new LeftBorder();
                //border.LeftBorder.Style = BorderStyleValues.Medium;
                border.RightBorder = new RightBorder();
                border.RightBorder.Style = BorderStyleValues.Thin;
                border.TopBorder = new TopBorder();
                //border.TopBorder.Style = BorderStyleValues.Medium;
                border.BottomBorder = new BottomBorder();
                //border.BottomBorder.Style = BorderStyleValues.Medium;
                border.VerticalBorder = new VerticalBorder();
                border.VerticalBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);

                //Boder index =5
                border = new Border();
                border.LeftBorder = new LeftBorder();
                border.LeftBorder.Style = BorderStyleValues.Thin;
                //border.RightBorder = new RightBorder();
                //border.RightBorder.Style = BorderStyleValues.Medium;
                border.TopBorder = new TopBorder();
                border.TopBorder.Style = BorderStyleValues.Thin;
                border.BottomBorder = new BottomBorder();
                border.BottomBorder.Style = BorderStyleValues.Thin;
                border.VerticalBorder = new VerticalBorder();
                border.VerticalBorder.Style = BorderStyleValues.Thin;
                border.DiagonalBorder = new DiagonalBorder();
                borders.Append(border);


                borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);

                CellStyleFormats csfs = new CellStyleFormats();
                CellFormat cf = new CellFormat();
                cf.NumberFormatId = 0;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 0;
                csfs.Append(cf);
                csfs.Count = UInt32Value.FromUInt32((uint)csfs.ChildElements.Count);

                uint iExcelIndex = 164;
                NumberingFormats nfs = new NumberingFormats();
                CellFormats cfs = new CellFormats();

                cf = new CellFormat();
                cf.NumberFormatId = 0;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 0;
                cf.FormatId = 0;
                cfs.Append(cf);

                NumberingFormat nfDateTime = new NumberingFormat();
                nfDateTime.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                nfDateTime.FormatCode = StringValue.FromString("dd/mm/yyyy hh:mm:ss");
                nfs.Append(nfDateTime);

                NumberingFormat nf4decimal = new NumberingFormat();
                nf4decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                nf4decimal.FormatCode = StringValue.FromString("#,##0.0000");
                nfs.Append(nf4decimal);

                // #,##0.00 is also Excel style index 4
                NumberingFormat nf2decimal = new NumberingFormat();
                nf2decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                nf2decimal.FormatCode = StringValue.FromString("#,##0.00");
                nfs.Append(nf2decimal);

                // @ is also Excel style index 49
                NumberingFormat nfForcedText = new NumberingFormat();
                nfForcedText.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                nfForcedText.FormatCode = StringValue.FromString("@");
                nfs.Append(nfForcedText);

                // index 1
                // Format dd/mm/yyyy
                cf = new CellFormat();
                cf.NumberFormatId = nfForcedText.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 4;
                cf.FormatId = 0;
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 2
                // Format #,##0.00
                cf = new CellFormat();
                cf.NumberFormatId = nfForcedText.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 5;
                cf.FormatId = 0;
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 3
                cf = new CellFormat();
                cf.NumberFormatId = nfDateTime.NumberFormatId;
                cf.FontId = 1;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyFont = true;
                cf.ApplyBorder = true;
                //cf = new CellFormat() { Alignment = align };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 4
                cf = new CellFormat();
                cf.NumberFormatId = nf4decimal.NumberFormatId;
                cf.FontId = 2;
                cf.FillId = 0;
                cf.BorderId = 0;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyFont = true;
                cf = new CellFormat() { Alignment = align3 };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 5
                cf = new CellFormat();
                cf.NumberFormatId = nf2decimal.NumberFormatId;
                cf.FontId = 1;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyFont = true;
                cf.ApplyBorder = true;
                cf = new CellFormat() { Alignment = align };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 6
                cf = new CellFormat();
                cf.NumberFormatId = nfForcedText.NumberFormatId;
                cf.FontId = 1;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyFont = true;
                //cf = new CellFormat() { Alignment = align };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 7
                // Header text
                cf = new CellFormat();
                cf.NumberFormatId = nfDateTime.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyBorder = true;
                cf = new CellFormat() { Alignment = align2 };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 8
                // column text
                cf = new CellFormat();
                cf.NumberFormatId = nfForcedText.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 9
                // coloured 2 decimal text
                cf = new CellFormat();
                cf.NumberFormatId = nf2decimal.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 10
                // coloured column text
                cf = new CellFormat();
                cf.NumberFormatId = nfForcedText.NumberFormatId;
                cf.FontId = 0;
                cf.FillId = 2;
                cf.BorderId = 2;
                cf.FormatId = 0;
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);

                // index 11
                cf = new CellFormat();
                cf.NumberFormatId = nfDateTime.NumberFormatId;
                cf.FontId = 1;
                cf.FillId = 0;
                cf.BorderId = 1;
                cf.FormatId = 0;
                cf.ApplyAlignment = true;
                cf.ApplyFont = true;
                cf.ApplyBorder = true;
                cf = new CellFormat() { Alignment = align1 };
                cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                cfs.Append(cf);


                nfs.Count = UInt32Value.FromUInt32((uint)nfs.ChildElements.Count);
                cfs.Count = UInt32Value.FromUInt32((uint)cfs.ChildElements.Count);

                ss.Append(nfs);
                ss.Append(fts);
                ss.Append(fills);
                ss.Append(borders);
                ss.Append(csfs);
                ss.Append(cfs);

                CellStyles css = new CellStyles();
                CellStyle cs = new CellStyle();
                cs.Name = StringValue.FromString("Normal");
                cs.FormatId = 0;
                cs.BuiltinId = 0;
                css.Append(cs);
                css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
                this.Append(css);

                DifferentialFormats dfs = new DifferentialFormats();
                dfs.Count = 0;
                ss.Append(dfs);

                TableStyles tss = new TableStyles();
                tss.Count = 0;
                tss.DefaultTableStyle = StringValue.FromString("TableStyleMedium9");
                tss.DefaultPivotStyle = StringValue.FromString("PivotStyleLight16");
                ss.Append(tss);
                return ss;
                //Fonts fts = new Fonts();
                //DocumentFormat.OpenXml.Spreadsheet.Font ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                //FontName ftn = new FontName();
                //ftn.Val = StringValue.FromString("Calibri");
                //FontSize ftsz = new FontSize();
                //Bold bd = new Bold();
                //ftsz.Val = DoubleValue.FromDouble(11);
                //ft.FontName = ftn;
                //ft.FontSize = ftsz;
                //fts.Append(ft);

                ////Bold
                //ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                //ftn = new FontName();
                //Bold bdd = new Bold();
                //ftn.Val = StringValue.FromString("Calibri");
                //ftsz = new FontSize();
                //ftsz.Val = DoubleValue.FromDouble(11);
                //ft.FontName = ftn;
                //ft.FontSize = ftsz;
                //ft.Bold = bdd;
                //fts.Append(ft);

                ////UnderLine
                //ft = new DocumentFormat.OpenXml.Spreadsheet.Font();
                //ftn = new FontName();
                //Underline ul = new Underline();
                //ftn.Val = StringValue.FromString("Calibri");
                //ftsz = new FontSize();
                //ftsz.Val = DoubleValue.FromDouble(11);
                //ft.FontName = ftn;
                //ft.FontSize = ftsz;
                //ft.Underline = ul;
                //fts.Append(ft);

                //fts.Count = UInt32Value.FromUInt32((uint)fts.ChildElements.Count);

                ////Alignment of the Text in the Cell.
                //Alignment align = new Alignment();
                //align.Horizontal = HorizontalAlignmentValues.Left;
                //align.WrapText = true;
                ////CellFormat cf = new CellFormat() { Alignment = align };


                //Alignment Vertalign = new Alignment();
                //Vertalign.Vertical = VerticalAlignmentValues.Top;
                ////Vertalign.WrapText = true;

                //Fills fills = new Fills();
                //Fill fill;
                //PatternFill patternFill;
                //fill = new Fill();
                //patternFill = new PatternFill();
                //patternFill.PatternType = PatternValues.None;
                //fill.PatternFill = patternFill;
                //fills.Append(fill);

                //fill = new Fill();
                //patternFill = new PatternFill();
                //patternFill.PatternType = PatternValues.Gray125;
                //fill.PatternFill = patternFill;
                //fills.Append(fill);

                //fill = new Fill();
                //patternFill = new PatternFill();
                //patternFill.PatternType = PatternValues.Solid;
                //patternFill.ForegroundColor = new ForegroundColor();
                //patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString("00ff9728");
                //patternFill.BackgroundColor = new BackgroundColor();
                //patternFill.BackgroundColor.Rgb = HexBinaryValue.FromString("00ff9728");//patternFill.ForegroundColor.Rgb;
                //fill.PatternFill = patternFill;
                //fills.Append(fill);

                //fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);

                //Borders borders = new Borders();
                //Border border = new Border();
                //border.LeftBorder = new LeftBorder();
                //border.RightBorder = new RightBorder();
                //border.TopBorder = new TopBorder();
                //border.BottomBorder = new BottomBorder();
                //border.DiagonalBorder = new DiagonalBorder();
                //borders.Append(border);

                ////Boarder Index 1
                //border = new Border();
                //border.LeftBorder = new LeftBorder();
                //border.LeftBorder.Style = BorderStyleValues.Medium;
                //border.RightBorder = new RightBorder();
                //border.RightBorder.Style = BorderStyleValues.Medium;
                //border.TopBorder = new TopBorder();
                //border.TopBorder.Style = BorderStyleValues.Medium;
                //border.BottomBorder = new BottomBorder();
                //border.BottomBorder.Style = BorderStyleValues.Medium;
                ////border.VerticalBorder = new VerticalBorder();
                ////border.VerticalBorder.Style = BorderStyleValues.Medium;
                //border.DiagonalBorder = new DiagonalBorder();
                //borders.Append(border);


                ////Boarder Index 2
                //border = new Border();
                //border.LeftBorder = new LeftBorder();
                //border.RightBorder = new RightBorder();
                //border.TopBorder = new TopBorder();
                //border.TopBorder.Style = BorderStyleValues.Medium;
                //border.BottomBorder = new BottomBorder();
                //border.BottomBorder.Style = BorderStyleValues.Medium;
                //border.DiagonalBorder = new DiagonalBorder();
                //borders.Append(border);



                //borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);

                //CellStyleFormats csfs = new CellStyleFormats();
                //CellFormat cf = new CellFormat();
                //cf.NumberFormatId = 0;
                //cf.FontId = 0;
                //cf.FillId = 0;
                //cf.BorderId = 0;
                //csfs.Append(cf);
                //csfs.Count = UInt32Value.FromUInt32((uint)csfs.ChildElements.Count);

                //uint iExcelIndex = 164;
                //NumberingFormats nfs = new NumberingFormats();
                //CellFormats cfs = new CellFormats();

                //cf = new CellFormat();
                //cf.NumberFormatId = 0;
                //cf.FontId = 0;
                //cf.FillId = 0;
                //cf.BorderId = 0;
                //cf.FormatId = 0;
                //cfs.Append(cf);

                //NumberingFormat nfDateTime = new NumberingFormat();
                //nfDateTime.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                //nfDateTime.FormatCode = StringValue.FromString("dd/mm/yyyy hh:mm:ss");
                //nfs.Append(nfDateTime);

                //NumberingFormat nf4decimal = new NumberingFormat();
                //nf4decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                //nf4decimal.FormatCode = StringValue.FromString("#,##0.0000");
                //nfs.Append(nf4decimal);

                //// #,##0.00 is also Excel style index 4
                //NumberingFormat nf2decimal = new NumberingFormat();
                //nf2decimal.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                //nf2decimal.FormatCode = StringValue.FromString("#,##0.00");
                //nfs.Append(nf2decimal);

                //// @ is also Excel style index 49
                //NumberingFormat nfForcedText = new NumberingFormat();
                //nfForcedText.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++);
                //nfForcedText.FormatCode = StringValue.FromString("@");
                //nfs.Append(nfForcedText);

                //// index 1
                //// Format dd/mm/yyyy
                //cf = new CellFormat();
                //cf.NumberFormatId = 15;
                //cf.FontId = 0;
                //cf.FillId = 0;
                //cf.BorderId = 0;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 2
                //// Format #,##0.00
                //cf = new CellFormat();
                //cf.NumberFormatId = 4;
                //cf.FontId = 2;
                //cf.FillId = 0;
                //cf.BorderId = 0;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 3
                //cf = new CellFormat();
                //cf.NumberFormatId = nfDateTime.NumberFormatId;
                //cf.FontId = 1;
                //cf.FillId = 0;
                //cf.BorderId = 1;
                //cf.FormatId = 0;
                //cf.ApplyAlignment = true;
                //cf.ApplyFont = true;
                ////cf = new CellFormat() { Alignment = align };
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 4
                //cf = new CellFormat();
                //cf.NumberFormatId = nf4decimal.NumberFormatId;
                //cf.FontId = 2;
                //cf.FillId = 0;
                //cf.BorderId = 0;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 5
                //cf = new CellFormat();
                //cf.NumberFormatId = nf2decimal.NumberFormatId;
                //cf.FontId = 1;
                //cf.FillId = 0;
                //cf.BorderId = 1;
                //cf.FormatId = 0;
                //cf.ApplyAlignment = true;
                //cf.ApplyFont = true;
                //cf = new CellFormat() { Alignment = align };
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 6
                //cf = new CellFormat();
                //cf.NumberFormatId = nfForcedText.NumberFormatId;
                //cf.FontId = 1;
                //cf.FillId = 0;
                //cf.BorderId = 1;
                //cf.FormatId = 0;
                //cf.ApplyAlignment = true;
                //cf.ApplyFont = true;
                ////cf = new CellFormat() { Alignment = align };
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 7
                //// Header text
                //cf = new CellFormat();
                //cf.NumberFormatId = nfForcedText.NumberFormatId;
                ////cf.FontId = 1;
                ////cf.FillId = 0;
                ////cf.BorderId = 1;
                ////cf.FormatId = 0;
                //cf.ApplyAlignment = true;
                //cf.ApplyBorder = true;
                //cf = new CellFormat() { Alignment = Vertalign };
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 8
                //// column text
                //cf = new CellFormat();
                //cf.NumberFormatId = nfForcedText.NumberFormatId;
                //cf.FontId = 0;
                //cf.FillId = 0;
                //cf.BorderId = 1;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 9
                //// coloured 2 decimal text
                //cf = new CellFormat();
                //cf.NumberFormatId = nf2decimal.NumberFormatId;
                //cf.FontId = 0;
                //cf.FillId = 0;
                //cf.BorderId = 1;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);

                //// index 10
                //// coloured column text
                //cf = new CellFormat();
                //cf.NumberFormatId = nfForcedText.NumberFormatId;
                //cf.FontId = 0;
                //cf.FillId = 2;
                //cf.BorderId = 2;
                //cf.FormatId = 0;
                //cf.ApplyNumberFormat = BooleanValue.FromBoolean(true);
                //cfs.Append(cf);


                //nfs.Count = UInt32Value.FromUInt32((uint)nfs.ChildElements.Count);
                //cfs.Count = UInt32Value.FromUInt32((uint)cfs.ChildElements.Count);

                //ss.Append(nfs);
                //ss.Append(fts);
                //ss.Append(fills);
                //ss.Append(borders);
                //ss.Append(csfs);
                //ss.Append(cfs);

                //CellStyles css = new CellStyles();
                //CellStyle cs = new CellStyle();
                //cs.Name = StringValue.FromString("Normal");
                //cs.FormatId = 0;
                //cs.BuiltinId = 0;
                //css.Append(cs);
                //css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
                //this.Append(css);

                //DifferentialFormats dfs = new DifferentialFormats();
                //dfs.Count = 0;
                //ss.Append(dfs);

                //TableStyles tss = new TableStyles();
                //tss.Count = 0;
                //tss.DefaultTableStyle = StringValue.FromString("TableStyleMedium9");
                //tss.DefaultPivotStyle = StringValue.FromString("PivotStyleLight16");
                //ss.Append(tss);
                //return ss;
            }
            catch (Exception ex)
            {
                string ErrMsg = ex.ToString();
                int LineNo = ExceptionHelper.LineNumber(ex);
                return ss;
            }
        }
    }
}