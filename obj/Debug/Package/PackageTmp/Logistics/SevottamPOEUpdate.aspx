<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="SevottamPOEUpdate.aspx.cs" EnableEventValidation ="false" Inherits="VOMS_ERP.Logistics.SevottamPOEUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Sevottam POE Update"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" style="margin-right: 15%;"/>
                                    </td>
                                    <td align="right">
                                        <div runat="server" id="dvexport">
                                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                                class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td align="right">
                                        <div style="border: 0px solid #9CB5CB; float: right; background: #ECEFF5; padding: 5px;
                                            width: 98%; margin: 5px; height: 99%;">
                                            <span style="color: #175F99;">Sevottam Reference No. : <font color="red" size="2"><b>
                                                *</b></font>:</span>&nbsp;&nbsp;&nbsp;
                                            <asp:TextBox ID="txtSevRefNo" onkeypress="return isOrgName(event)" Text="" runat="server"></asp:TextBox>
                                        </div>
                                        <asp:HiddenField ID="hfType" runat="server" Value="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:GridView runat="server" ID="GVSevottamPOE" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" EmptyDataText="No Records To Display...!">
                                            <%--OnRowDataBound="GVSevottamCTOne_RowDataBound"--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." Visible="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Number">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblCTNo" Text='<%#Eval("CT1Number") %>'></asp:Label>
                                                        <asp:HiddenField ID="HFCT1ID" runat="server" Value='<%#Eval("CT1ID") %>' />
                                                        <asp:HiddenField ID="HFCTTrackingID" runat="server" Value='<%#Eval("CT1TrackingID") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <b>Total Amounts : </b>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblctdate" Text='<%#Eval("CT1Date") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblCTValue" Text='<%#Eval("CT1Value") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ARE-1 Number">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblareNo" Text='<%#Eval("ARE1Number") %>'></asp:Label>
                                                        <asp:HiddenField ID="HFARE1ID" runat="server" Value='<%#Eval("ARE1ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblaredate" Text='<%#Eval("ARE1Date") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ARE-1 Value">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblareval" Text='<%#Eval("ARE1Value") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Un-Utilized">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblUnUtlzd" Text='<%#Eval("UnUtilizedAmt") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Supplier">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSupplier" Text='<%#Eval("SupplierNm") %>'></asp:Label>
                                                        <asp:HiddenField ID="HFSUPLRID" runat="server" Value='<%#Eval("SuplrID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ECC No.">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblEccno" Text='<%#Eval("ECCNo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sevottam ID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblSvtmID" Text='<%#Eval("SevID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="POE No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtPOENo" onChange="ValidateRefNo(this.id)" runat="server" Text='<%# Eval("POENumber") %>'
                                                            MaxLength="180"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="DT">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDT" runat="server" Text='<%# Eval("POEDate") %>' Style="width: 70px;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amt Credited">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtAmtCredited" runat="server" onkeyup="extractNumber(this,2,false);"
                                                            onkeypress="return blockNonNumbers(this, event, true, false);" Text='<%# Eval("POEAmtCrtd") %>'>
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Remarks: <font color="red" size="2"><b>*</b></font>:</span>
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:TextBox runat="server" ID="txtComments" CssClass="bcAsptextboxmulti" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Update" OnClick="btnSave_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Refresh" OnClick="btnClear_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div3" class="bcButtonDiv">
                                                <a href="../Masters/Home.aspx" title="Exit" class="bcAlink" onclick="javascript:Exit()">
                                                    Exit </a>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>

    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>

    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>

    <script src="../JScript/validate2.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        $(function () {
            $('[id$=txtDT]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });

        function isOrgName(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 46 &&
            (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }


        function Myvalidations() {
            var CTOnes = CTOnes = $('[id$=GVSevottamPOE]')[0].rows.length;
            //var aType = $('[id$=hfType]').val();

            if (($('[id$=txtSevRefNo]').val()).trim() == '') {
                $('[id$=txtSevRefNo]').focus();
                ErrorMessage('Sevottam Ref.No. is Required.');
                return false;
            }
            else if (($('[id$=txtComments]').val()).trim() == '') {
                $('[id$=txtComments]').focus();
                ErrorMessage('Comments is Required.');
                return false;
            }
            else if (CTOnes > 0) {
                if (CTOnes == 1) {
                    ErrorMessage('No POEs to Save.');
                    return false;
                }
                else {
                    var select = 0;
                    for (var i = 2; i <= CTOnes; i++) {
                        var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                        var POENo = GetClientID("txtPOENo").attr("id");
                        var POEDt = GetClientID("txtDT").attr("id");
                        var POEValue = GetClientID("txtAmtCredited").attr("id");
                        if ((($('#' + POENo)[0].value).trim() != ''
                                    && ($('#' + POEDt)[0].value).trim() != ''
                                    && ($('#' + POEValue)[0].value).trim() != ''
                                    && ($('#' + POEValue)[0].value).trim() > 0)) {
                        }
                        else {
                            if (($('#' + POENo)[0].value).trim() == '') {
                                $('#' + POENo)[0].focus();
                                ErrorMessage('POE Ref.No. is required.');
                                return false;
                                break;
                            }
                            else if (($('#' + POEValue)[0].value).trim() == 0 || ($('#' + POEValue)[0].value).trim() == '') {
                                $('#' + POEValue)[0].focus();
                                ErrorMessage('POE value cannot be ZERO or Empty.');
                                return false;
                                break;
                            }
                            else if (($('#' + POEDt)[0].value).trim() == '') {
                                $('#' + POEDt)[0].focus();
                                ErrorMessage('POE Ref. Date is required.');
                                return false;
                                break;
                            }
                        }
                    }
                }
            }
            else {
                return true;
            }

        }        
    </script>

</asp:Content>
