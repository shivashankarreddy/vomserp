<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="SevottamCTOne.aspx.cs" Inherits="VOMS_ERP.Logistics.SevottamCTOne"
     %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Sevottam CT-1 Update"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
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
                                            <asp:TextBox ID="txtSevRefNo" Text="" runat="server"></asp:TextBox>
                                        </div>
                                        <asp:HiddenField ID="hfType" runat="server" Value="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:GridView runat="server" ID="GVSevottamCTOne" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" EmptyDataText="No Records To Display...!"
                                            OnRowDataBound="GVSevottamCTOne_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." Visible="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="FPO Nos.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFPONos" runat="server" Text='<%# Eval("FPONos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Supplier Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSupplierNm" runat="server" Text='<%# Eval("SupplierNm") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Draft No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneDraftRefNo" runat="server" Text='<%# Eval("CT1DraftRefNo") %>'></asp:Label>
                                                        <asp:HiddenField ID="hfCTOneBondVal" runat="server" Value='<%# Eval("CT1BondValue") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Ref. No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCTOneRefNo" onChange="ValidateRefNo(this.id)" runat="server"
                                                            Text='<%# Eval("CT1ReferenceNo") %>' MaxLength="180"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Ref. DT">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCTOneRefDT" runat="server" Text='<%# Eval("RefDate") %>' Style="width: 80px;"></asp:TextBox>
                                                        <asp:HiddenField ID="hfCTOneID" runat="server" Value='<%# Eval("CT1ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal Ref. No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefNo" runat="server" Text='<%# Eval("InternalRefNo") %>'
                                                            onChange="ValidateInternalRefNo(this.id)"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal Ref.DT.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefDT" runat="server" Text='<%# Eval("InternalRefDt") %>'
                                                            Style="width: 80px;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField HeaderText="CT-1 Value" DataField="CT1BondValue" />--%>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblCTOneval" runat="server" Text='<%# Eval("CT1BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bond Balance Value">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtBondBalanceVal" runat="server" Text='<%# Eval("BondBalanceValue") %>'
                                                            Style="width: 100px;" onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                            MaxLength="18"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="No.of ARE Forms">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtNoOfAREForms" runat="server" Text='<%# Eval("NoofARE1Forms") %>'
                                                            onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                                            onkeypress="return blockNonNumbers(this, event, false, false);" MaxLength="1"
                                                            onchange="return CheckAREforms(this.id)" Style="width: 50px;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Status.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Clear">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkClear" runat="server" onchange="ClearRow()" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:GridView runat="server" ID="GVSevottamCTOne_Cancel" AutoGenerateColumns="false"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            Width="100%" EmptyDataText="No Records To Display...!" OnRowDataBound="GVSevottamCTOne_Cancel_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." Visible="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="FPO Nos.">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hfCTOneID" runat="server" Value='<%# Eval("CT1ID") %>' />
                                                        <asp:Label ID="lblFPONos" runat="server" Text='<%# Eval("FPONos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Supplier Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSupplierNm" runat="server" Text='<%# Eval("SupplierNm") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Ref. No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneRefNo" onChange="ValidateRefNo(this.id)" runat="server" Text='<%# Eval("CT1ReferenceNo") %>'
                                                            MaxLength="180"></asp:Label>
                                                        <asp:HiddenField ID="hfCTOneDraftRefNo" runat="server" Value='<%# Eval("CT1DraftRefNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneBondVal" runat="server" Text='<%# Eval("CT1BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal ref. No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInternalRefNo" runat="server" Text='<%# Eval("InternalRefNo") %>'
                                                            onChange="ValidateInternalRefNo(this.id)"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal Ref.DT.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInternalRefDT" runat="server" Text='<%# Eval("InternalRefDt") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="New Internal ref. No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefNo" runat="server" Text='<%# Eval("NewInternalRefNo") %>'
                                                            onchange="ValidateInternalRefNo(this.id);"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="New Internal ref. DT.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefDT" runat="server" Text='<%# Eval("NewInternalRefDT") %>'
                                                            Style="width: 80px;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField HeaderText="CT-1 Value" DataField="CT1BondValue" />--%>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblCTOneval" runat="server" Text='<%# Eval("CT1BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bond Balance Value">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtBondBalanceVal" runat="server" Text='<%# Eval("BondBalanceValue") %>'
                                                            Style="width: 100px;" onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                            MaxLength="18"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Status.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Clear">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkClear" runat="server" onchange="ClearRow1()" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>

                                        <asp:GridView runat="server" ID="gvSevottamCTOne_POEUnUsed" AutoGenerateColumns="false"
                                            RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            Width="100%" EmptyDataText="No Records To Display...!" OnRowDataBound="gvSevottamCTOne_POEUnUsed_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." Visible="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="FPO Nos.">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hfCTOneID" runat="server" Value='<%# Eval("CT1ID") %>' />
                                                        <asp:Label ID="lblFPONos" runat="server" Text='<%# Eval("FPONos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Supplier Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSupplierNm" runat="server" Text='<%# Eval("SupplierNm") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Ref. No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneRefNo" onChange="ValidateRefNo(this.id)" runat="server" Text='<%# Eval("CT1ReferenceNo") %>'
                                                            MaxLength="180"></asp:Label>
                                                        <asp:HiddenField ID="hfCTOneDraftRefNo" runat="server" Value='<%# Eval("CT1DraftRefNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneBondVal" runat="server" Text='<%# Eval("CT1BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal ref. No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInternalRefNo" runat="server" Text='<%# Eval("InternalRefNo") %>'
                                                            onChange="ValidateInternalRefNo(this.id)"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Internal Ref.DT.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInternalRefDT" runat="server" Text='<%# Eval("InternalRefDt") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="New Internal ref. No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefNo" runat="server" Text='<%# Eval("NewInternalRefNo") %>'
                                                            onchange="ValidateInternalRefNo(this.id);"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="New Internal ref. DT.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtInternalRefDT" runat="server" Text='<%# Eval("NewInternalRefDT") %>'
                                                            Style="width: 80px;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Value">
                                                    <ItemTemplate>
                                                    <asp:Label ID="lblCTOneval" runat="server" Text='<%# Eval("CT1BondValue") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bond Balance Value">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtBondBalanceVal" runat="server" Text='<%# Eval("BondBalanceValue") %>'
                                                            Style="width: 100px;" onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                                            MaxLength="18"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CT-1 Status.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCTOneStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Clear">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkClear" runat="server" onchange="ClearRow1()" />
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
                                        <span class="bcLabel">Comments <font color="red" size="2"><b>*</b></font>:</span>
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
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Update" OnClick="btnSend_Click" />
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
        $(function() {
            var dateToday = new Date();
            $('[id$=txtCTOneRefDT]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

        $(function() {
        var dateToday = new Date();
            $('[id$=txtInternalRefDT]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

//        $(function() {
//            $('[id$=txtNoOfAREForms]').keyup(function(e) {
//                var id = $(this).attr('id');
//                var Forms = $('[id$=' + id + ']').val();
//                if (Forms > 5) {
//                    ErrorMessage('ARE forms cannot be greater than 5..');
//                    $('[id$=' + id + ']').val('');
//                    $('[id$=' + id + ']').focus();
//                    return true;
//                }
//                else if (Forms == "") {
//                    ErrorMessage('ARE forms cannot be Empty..');
//                    $('[id$=' + id + ']').val('');
//                    $('[id$=' + id + ']').focus();
//                    return true;
//                }
//                else if (Forms == 0) {
//                    ErrorMessage('ARE forms cannot be Zero..');
//                    $('[id$=' + id + ']').val('');
//                    $('[id$=' + id + ']').focus();
//                    return true;
//                }
//            });
//        });

        function CheckAREforms(ctrlID) {
            var Forms = document.getElementById(ctrlID).value;
            if (Forms > 5) {
                ErrorMessage('ARE forms cannot be greater than 5.');
                $('[id$=' + ctrlID + ']').val('');
                $('[id$=' + ctrlID + ']').focus();
                return false;
            }
            else if (Forms == "") {
                ErrorMessage('ARE forms cannot be Empty.');
                $('[id$=' + ctrlID + ']').val('');
                $('[id$=' + ctrlID + ']').focus();
                return false;
            }
            else if (Forms == 0) {
                ErrorMessage('ARE forms cannot be Zero.');
                $('[id$=' + ctrlID + ']').val('');
                $('[id$=' + ctrlID + ']').focus();
                return false;
            }
        }

        function SuccessMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }

        function ErrorMessage(msg) {
            $("#<%=divMyMessage.ClientID %> span").remove();
            $('[id$=divMyMessage]').append('<span class="Error">' + msg + '</span>');
            $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        }
        function ClearRow(ctrlID) {
            //var refVal = document.getElementById(ctrlID).value;            
            var CTOnesLength = $('[id$=GVSevottamCTOne]')[0].rows.length
            for (var i = 2; i <= CTOnesLength; i++) {
                var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                var chkbval = GetClientID(chkbx + "_chkClear").attr("id");
                if ($('#' + chkbval)[0].checked) {
                    var RefNo = GetClientID(chkbx + "_txtCTOneRefNo").attr("id");
                    var RefDT = GetClientID(chkbx + "_txtCTOneRefDT").attr("id");
                    var AREForms = GetClientID(chkbx + "_txtNoOfAREForms").attr("id");
                    var CT1InternalRefNo = GetClientID(chkbx + "_txtInternalRefNo").attr("id");
                    var CT1InternalRefDt = GetClientID(chkbx + "_txtInternalRefDT").attr("id");
                    var CT1BondBalVal = GetClientID(chkbx + "_txtBondBalanceVal").attr("id");

                    $('#' + RefNo).val('');
                    $('#' + RefDT).val('');
                    $('#' + AREForms).val('');
                    $('#' + CT1InternalRefNo).val('');
                    $('#' + CT1InternalRefDt).val('');
                    $('#' + CT1BondBalVal).val('');
                    $('#' + chkbval)[0].checked = false;
                    break;
                }
            }
        }

        function ClearRow1(ctrlID) {
            //var refVal = document.getElementById(ctrlID).value;
            var CTOnesLength = $('[id$=GVSevottamCTOne_Cancel]')[0].rows.length
            for (var i = 2; i <= CTOnesLength; i++) {
                var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                var chkbval = GetClientID(chkbx + "_chkClear").attr("id");
                if ($('#' + chkbval)[0].checked) {
                    var NewInternalRefNo = GetClientID(chkbx + "_txtInternalRefNo").attr("id");
                    var NewInternalRefDt = GetClientID(chkbx + "_txtInternalRefDT").attr("id");
                    var CT1BondBalVal = GetClientID(chkbx + "_txtBondBalanceVal").attr("id");

                    $('#' + NewInternalRefNo).val('');
                    $('#' + NewInternalRefDt).val('');
                    //$('#' + CT1BondBalVal).val('');
                    $('#' + chkbval)[0].checked = false;
                    break;
                }
            }
        }

        function ValidateRefNo(ctrlID) {
            var aType = $('[id$=hfType]').val();
            if (aType != '' && aType == 'New') {
                var refVal = document.getElementById(ctrlID).value;
                var stat = 0;
                var CTOnesLength = $('[id$=GVSevottamCTOne]')[0].rows.length
                for (var i = 2; i <= CTOnesLength; i++) {
                    var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                    var CTOneRefNo = GetClientID(chkbx + "_txtCTOneRefNo").attr("id");
                    if (ctrlID != CTOneRefNo) {
                        if (($('#' + CTOneRefNo)[0].value).trim() == refVal) {
                            ErrorMessage('This Ref.No. is already in another field.');
                            document.getElementById(ctrlID).value = "";
                            $('#' + ctrlID).focus();
                            stat = 1;
                            break;
                        }
                    }
                }
                if (stat == 0) {
                    var result = SevottamCTOne.GetRefNo(refVal);
                    if (result.value == false) {
                        document.getElementById(ctrlID).value = "";
                        $('#' + ctrlID).focus();
                        ErrorMessage('This Ref.No. is already in another field..');
                    }
                }
            }
        }

        function ValidateInternalRefNo(ctrlID) {
            var aType = $('[id$=hfType]').val();
            var refVal = document.getElementById(ctrlID).value
            var stat = 0;
            var CTOnesLength = "";
            if (aType != '' && aType == "Cancel")
                CTOnesLength = $('[id$=GVSevottamCTOne_Cancel]')[0].rows.length;
            else if (aType != '' && aType == "New")
                CTOnesLength = $('[id$=GVSevottamCTOne]')[0].rows.length;

            for (var i = 2; i <= CTOnesLength; i++) {
                var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                var CTOneInternalRefNo = GetClientID(chkbx + "_txtInternalRefNo").attr("id");
                if (ctrlID != CTOneInternalRefNo) {
                    if (($('#' + CTOneInternalRefNo)[0].value).trim() == refVal) {
                        ErrorMessage('This ' + (aType == 'New' ? 'InternalRef.No.' : 'NewInternalRef.No.') + ' is already in another field.');
                        document.getElementById(ctrlID).value = "";
                        $('#' + ctrlID).focus();
                        stat = 1;
                        break;
                    }
                }
            }
            if (stat == 0) {
                var result = "";
                if (aType != '' && aType == "Cancel")
                    result = SevottamCTOne.GetNewInternalRefNo(refVal);
                else if (aType != '' && aType == "New")
                    result = SevottamCTOne.GetInternalRefNo(refVal);
                if (result.value == false) {
                    document.getElementById(ctrlID).value = "";
                    $('#' + ctrlID).focus();
                    ErrorMessage('This ' + (aType == 'New' ? 'InternalRef.No.' : 'NewInternalRef.No.') + ' is already in another field.');
                }
            }
        }
       
    </script>

    <script type="text/javascript">

        function Myvalidations() {
            var CTOnes = 0;
            var aType = $('[id$=hfType]').val();
            if (aType != '' && aType == 'New')
                CTOnes = $('[id$=GVSevottamCTOne]')[0].rows.length;
            else if (aType != '' && aType == 'Cancel')
                CTOnes = $('[id$=GVSevottamCTOne_Cancel]')[0].rows.length;
            else if (aType != '' && aType == 'UnUsed')
                CTOnes = $('[id$=gvSevottamCTOne_POEUnUsed]')[0].rows.length;
            if (CTOnes == 0) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">No CT-Ones to Save.</span>');
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (CTOnes > 0) {
                if (CTOnes == 1) {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">No CT-Ones to Save.</span>');
                    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                    return false;
                }
                else {
                    var select = 0;
                    for (var i = 2; i <= CTOnes; i++) {
                        var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                        if (aType == "New") {
                            var CTOneRefNo = GetClientID(chkbx + "_txtCTOneRefNo").attr("id");
                            var CTOneRefDT = GetClientID(chkbx + "_txtCTOneRefDT").attr("id");
                            var NoOfAREForms = GetClientID(chkbx + "_txtNoOfAREForms").attr("id");
                            var BondBalval = GetClientID(chkbx + "_txtBondBalanceVal").attr("id");
                            var CT1InternalRefNo = GetClientID(chkbx + "_txtInternalRefNo").attr("id");
                            var CT1InternalRefDt = GetClientID(chkbx + "_txtInternalRefDT").attr("id");

                            if ((($('#' + CTOneRefNo)[0].value).trim() != ''
                                    && ($('#' + CTOneRefDT)[0].value).trim() != ''
                                    && ($('#' + NoOfAREForms)[0].value).trim() != ''
                                    && ($('#' + NoOfAREForms)[0].value).trim() > 0
                                    && ($('#' + BondBalval)[0].value).trim() > 0
                                    && ($('#' + BondBalval)[0].value).trim() != ''
                                    && ($('#' + CT1InternalRefNo)[0].value).trim() != ''
                                    && ($('#' + CT1InternalRefDt)[0].value).trim() != '')) {
                            }
                            else {
                                if (($('#' + CTOneRefNo)[0].value).trim() == '') {
                                    $('#' + CTOneRefNo)[0].focus();
                                    ErrorMessage('CT-1 Ref.No. is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + BondBalval)[0].value).trim() == 0 || ($('#' + BondBalval)[0].value).trim() == '') {
                                    $('#' + BondBalval)[0].focus();
                                    ErrorMessage('Bond Balance value cannot be ZERO or Empty. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + CTOneRefDT)[0].value).trim() == '') {
                                    $('#' + CTOneRefDT)[0].focus();
                                    ErrorMessage('CT-1 Ref. Date is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + NoOfAREForms)[0].value).trim() == '') {
                                    $('#' + NoOfAREForms)[0].focus();
                                    ErrorMessage('CT-1 ARE forms is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + NoOfAREForms)[0].value).trim() == 0) {
                                    $('#' + NoOfAREForms)[0].focus();
                                    ErrorMessage('CT-1 ARE forms cannot be ZERO. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + CT1InternalRefNo)[0].value).trim() == '') {
                                $('#' + CT1InternalRefNo)[0].focus();
                                    ErrorMessage('CT-1 Internal Ref.No. is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + CT1InternalRefDt)[0].value).trim() == '') {
                                $('#' + CT1InternalRefNo)[0].focus();
                                    ErrorMessage('CT-1 Internal Ref. Date is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                            }
                        }
                        else if (aType == "Cancel" || aType == "UnUsed") {
                            var BondBalval = GetClientID(chkbx + "_txtBondBalanceVal").attr("id");
                            var CT1InternalRefNo = GetClientID(chkbx + "_txtInternalRefNo").attr("id");
                            var CT1InternalRefDt = GetClientID(chkbx + "_txtInternalRefDT").attr("id");

                            if ((($('#' + CT1InternalRefNo)[0].value).trim() != ''
                                    && ($('#' + CT1InternalRefDt)[0].value).trim() != ''
                                    && ($('#' + BondBalval)[0].value).trim() > 0
                                    && ($('#' + BondBalval)[0].value).trim() != '')) {
                            }
                            else {
                                if (($('#' + CT1InternalRefNo)[0].value).trim() == '') {
                                    $('#' + CT1InternalRefNo)[0].focus();
                                    ErrorMessage('New Internal Ref.No. is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + BondBalval)[0].value).trim() == 0 || ($('#' + BondBalval)[0].value).trim() == '') {
                                    $('#' + BondBalval)[0].focus();
                                    ErrorMessage('Bond Balance value cannot be ZERO or Empty. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                                else if (($('#' + CT1InternalRefDt)[0].value).trim() == '') {
                                    $('#' + CT1InternalRefDt)[0].focus();
                                    ErrorMessage('New Internal Ref. Date is required. (if u completed the remaining fields.)');
                                    return false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

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
            else {
                return true;
            }

        }        
    </script>

</asp:Content>
