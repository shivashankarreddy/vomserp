<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="PaymentVoucher.aspx.cs" Inherits="VOMS_ERP.Logistics.PaymentVoucher"
    Title="Volta Impex Pvt. Ltd." %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Payment Voucher" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span8" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlcustmr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="Select Customer"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblRctdSctg" class="bcLabel">Supplier Category :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsuplrctgry" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlsuplrctgry_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select Category"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsuplr" AutoPostBack="true" OnSelectedIndexChanged="ddlsuplr_SelectedIndexChanged"
                                            CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Supplier"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            OnSelectedIndexChanged="lbfpos_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            OnSelectedIndexChanged="lblpos_SelectedIndexChanged"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Local PO(s) Amount <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:Label runat="server" ID="lblTotalAmt" CssClass="bcLabel"></asp:Label>
                                    </td>
                                </tr>
                                <%-- New row begin --%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Paid Amount <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPaidAmt" CssClass="bcAsptextbox" MaxLength="10"
                                            onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Paid Against <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPaidAgainest" AutoPostBack="true" CssClass="bcAspdropdown"
                                            OnSelectedIndexChanged="ddlPaidAgainest_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="-- Select --"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" runat="server" class="bcLabel">List <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lbPdAgnst" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Payment Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPaymentDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">Cheque Number <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtChequeNo" CssClass="bcAsptextbox" MaxLength="10"
                                            onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Cheque Date <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtChequeDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">RTGS/NEFT Code <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRTGS" onkeypress="return isSomeSplChar(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblDuedt" class="bcLabel">Bank Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--<asp:TextBox runat="server" ID="txtBankName" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" ID="ddlBankName" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblIns" class="bcLabel">Remarks:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRemarks" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <%--new row end--%>
                                <tr>
                                    <td colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="40%">
                                                        <span id="Span10" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="Left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
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
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
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
                    <tr>
                        <td align="center" colspan="6">
                            <table border="0" cellpadding="0" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" style="display: none;">
                                            <asp:GridView runat="server" ID="gvPmtDtls" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                EmptyDataText="No Records are Exists" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" OnPreRender="gvPmtDtls_PreRender"
                                                OnRowCommand="gvPmtDtls_RowCommand" OnRowDataBound="gvPmtDtls_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblPmtVchrID" Text='<% #Eval("ID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Customer Name" DataField="CustmNm" />
                                                    <asp:BoundField HeaderText="FPO Number(s)" DataField="FPONos" />
                                                    <asp:BoundField HeaderText="LPO Number(s)" DataField="LPONos" />
                                                    <asp:BoundField HeaderText="Supplier Name" DataField="SuplrNm" />
                                                    <asp:BoundField HeaderText="Payment Date" DataField="PaymentDt" />
                                                    <asp:BoundField HeaderText="Payment Amount" DataField="PaidAmt" />
                                                    <asp:BoundField HeaderText="Payment Againest" DataField="PaymentAgnst" />
                                                    <asp:BoundField HeaderText="Payment Againest List" DataField="RefNos" />
                                                    <asp:BoundField HeaderText="Check Number" DataField="CheckNumber" />
                                                    <asp:BoundField HeaderText="Check Date" DataField="CheckDt" />
                                                    <asp:BoundField HeaderText="Bank Name" DataField="BankName" />
                                                </Columns>
                                            </asp:GridView>
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

    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtPaymentDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtChequeDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isSomeSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function Myvalidations() {
            try {
                if (($('[id$=ddlcustmr]').val()).trim() == '0') {

                    ErrorMessage('Customer Name is Required.');
                    $('[id$=ddlcustmr]').focus();

                    return false;
                }
                else if (($('[id$=ddlsuplr]').val()).trim() == '0') {

                    ErrorMessage('Supplier Name is Required.');
                    $('[id$=ddlsuplr]').focus();

                    return false;
                }
                else if ($('[id$=lbfpos]').val() == null) {

                    ErrorMessage('Foreign PO(s) is Required.');
                    $('[id$=lbfpos]').focus();

                    return false;
                }
                else if ($('[id$=lblpos]').val() == null) {

                    ErrorMessage('Local PO(s) is Required.');
                    $('[id$=lblpos]').focus();

                    return false;
                }
                else if (($('[id$=txtPaymentDt]').val()).trim() == '') {

                    ErrorMessage('Payment Date is Required.');
                    $('[id$=ddlsuplr]').focus();

                    return false;
                }
                else if (($('[id$=txtPaidAmt]').val()).trim() == '') {

                    ErrorMessage('Paid Amount is Required.');
                    $('[id$=ddlsuplr]').focus();

                    return false;
                }
                else if (($('[id$=ddlPaidAgainest]').val()).trim() == '0') {

                    ErrorMessage('Paid Againest is Required.');
                    $('[id$=ddlsuplr]').focus();

                    return false;
                }
                else if (($('[id$=txtChequeNo]').val()).trim() == '') {

                    ErrorMessage('Cheque Number is Required.');
                    $('[id$=txtChequeNo]').focus();

                    return false;
                }
                else if (($('[id$=txtChequeDt]').val()).trim() == '') { //

                    ErrorMessage('Cheque Date is Required.');
                    $('[id$=txtChequeDt]').focus();

                    return false;
                }
                else if (($('[id$=txtBankName]').val()).trim() == '') {

                    ErrorMessage('Bank Name is Required.');
                    $('[id$=txtBankName]').focus();

                    return false;
                }
                else if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {

                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();

                        return false;
                    }
                }
                else {
                    return true;
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

    </script>

</asp:Content>
