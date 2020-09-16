<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="ReverseBillPaymentApproval.aspx.cs" Inherits="VOMS_ERP.Accounts.ReverseBillPaymentApproval" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Reverse Bill Payment Approval"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="lblCustName" class="bcLabel">Name of Supplier<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlSupplier" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlSupplier_SelectedIndexChanged" onchange="SetDate_Grid();">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblfedt" class="bcLabel">Approval Reference No:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlRefNumber" ValidationGroup="D" CssClass="bcAspdropdown"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlRefNumber_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        &nbsp;&nbsp;&nbsp;<b>Payment Against Bills/Proforma Invoice</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <asp:HiddenField ID="HFINV_LPOIDs" runat="server" Value="" />
                                                    <asp:HiddenField ID="HFPO_LPOIDs" runat="server" Value="" />
                                                    <asp:HiddenField ID="HFLPOsHistorySum" runat="server" Value="" />
                                                    <asp:HiddenField ID="HFSelectedLPOID" runat="server" Value="" />
                                                    <div style="overflow: auto; width: 100%;">
                                                        <div id="divGridOne" style="max-height: 250px; min-height: 100px;" runat="server"
                                                            width="100%" height="220px">
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        &nbsp;&nbsp;&nbsp;<b>Advance Against Purchase Order</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div style="overflow: auto; width: 100%;">
                                                        <div id="DivGridTwo" style="max-height: 250px; min-height: 100px;" runat="server"
                                                            width="100%" height="220px">
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        &nbsp;&nbsp;&nbsp;<b>History</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div style="overflow: auto; width: 100%;">
                                                        <div id="DivHistory" style="max-height: 250px; min-height: 50px;" runat="server"
                                                            width="100%">
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        &nbsp;&nbsp;&nbsp;<b>Cheques History</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <div style="overflow: auto; width: 100%;">
                                                        <div id="DivHistoryCheques" style="max-height: 250px; min-height: 50px;" runat="server"
                                                            width="100%">
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Balance Amount :</span>
                                    </td>
                                    <td colspan="5">
                                        <asp:Label ID="lblBalance" runat="server" Font-Bold="true" Text="."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr style="background-color: Gray; font-style: normal; color: White;">
                                                <td colspan="4">
                                                    &nbsp;&nbsp;&nbsp;Export Details :
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span id="Span1" class="bcLabel">Pro. Inv. No.:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtProInfNo" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span2" class="bcLabel">Pro. Inv. Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtProInvNoDate" runat="server" ReadOnly="true" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span id="Span3" class="bcLabel">Export Invoice No.:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtExportInvNo" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span5" class="bcLabel">Export Invoice Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtExportInvDate" runat="server" ReadOnly="true" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span id="Span6" class="bcLabel">Form &#39;C&#39; or &#39;H&#39; No.:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtFormCorHNo" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span7" class="bcLabel">Form &#39;C&#39; or &#39;H&#39; Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtFormCorHNoDate" runat="server" ReadOnly="true" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <asp:Panel ID="pnlCheque" runat="server">
                                            <table style="width: 100%; overflow: auto;">
                                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                                    <td>
                                                        &nbsp;&nbsp;&nbsp;Payment Details
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table style="width: 75%; font-size: small;" class="rounded-corner">
                                                            <thead>
                                                                <tr>
                                                                    <th class="rounded-First">
                                                                        S.No.
                                                                    </th>
                                                                    <th>
                                                                        Cheque No.
                                                                    </th>
                                                                    <th>
                                                                        Date
                                                                    </th>
                                                                    <th>
                                                                        Bank
                                                                    </th>
                                                                    <th>
                                                                        UTR No.
                                                                    </th>
                                                                    <th class="rounded-Last">
                                                                        Amount
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td>
                                                                        1
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtChequeNo" runat="server" ReadOnly="true" CssClass="bcAsptextbox"
                                                                            onchange="CheckChequeNo();"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtDate" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBankName" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtUTRNo" runat="server" ReadOnly="true" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <asp:Panel ID="RejectDtls" runat="server" Visible="false">
                                            <table style="width: 100%; overflow: auto;">
                                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                                    <td colspan="4">
                                                        &nbsp;&nbsp;&nbsp;Reverse Details
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="bcTdnormal" width="20%">
                                                        <span id="Span9" class="bcLabel">Reasons for Reverse<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td class="bcTdnormal" align="left" width="30%">
                                                        <asp:TextBox ID="txtRejectReasons" TextMode="MultiLine" runat="server" Width="80%"
                                                            CssClass="bcAsptextboxmulti"></asp:TextBox>
                                                    </td>
                                                    <td colspan="2" class="bcTdnormal" width="50%">
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" align="right">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div4" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnReject" Visible="false" Text="Reverse" OnClick="btnReject_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div2" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div3" class="bcButtonDiv">
                                                                <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <style>
        tr.SelectedRow td
        {
            background: #FFCF8B !important;
        }
    </style>
    <script type="text/javascript">
        try {
            $(window).load(function () {
                $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            });

            $(document).ready(function () {
                getdate();
            });
            function getdate() {
                $(".DatePicker").datepicker({
                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true,
                    showAnim: 'blind',
                    showButtonPanel: true
                });
            }

        }
        catch (Error) {
            ErrorMessage(Error.message);
        }
    </script>
    <script type="text/javascript">
        try {
            $(document).ready(function () {
                var dateToday = new Date();
                var dateBack = new Date(dateToday.setDate(dateToday.getDate() - 365));
                var MaxDate = new Date(dateToday.setDate(dateToday.getDate() + 365));
                $('[id$=txtDate]').datepicker({

                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
                });
                SetDate_Grid();
            });

            function SetDate_Grid() {
                $('[id$=txtCrntDate]').datepicker({

                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
                });
            }

        }
        catch (Error) {
            ErrorMessage(Error.message);
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $(".DatePicker").datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                showAnim: 'blind',
                showButtonPanel: true
            });
        });

        function MyRejectvalidations() {

            if (($('[id$=txtRejectReasons]').val()).trim() == '') {
                ErrorMessage('Reasons for reversal is Required.');
                $('[id$=txtRejectReasons]').focus();
                return false;
            }
            else
                return true;
        }

        //        function GetBal(IsKeyPress) {
        //            var chequePayAmt = 0; 
        //            var ChequeAmt = 0; 
        //            var INVPayAmt = $('#HFINVAmt0').val() == undefined ? "0" : $('#HFINVAmt0').val();
        //            var POPayAmt = $('#hfPOamt').val() == undefined ? "0" : $('#hfPOamt').val();
        //            var INVLPOAmt = $('#HFLPOamt0').val() == undefined ? "0" : $('#HFLPOamt0').val();
        //            var POLPOAmt = $('#HFPOLPOamt').val() == undefined ? "0" : $('#HFPOLPOamt').val();

        //            var TotalPayAmt = 0;
        //            if (parseFloat(chequePayAmt) > 0)
        //                TotalPayAmt = (parseFloat(chequePayAmt) + parseFloat(ChequeAmt));
        //            else
        //                TotalPayAmt = (parseFloat(ChequeAmt) + parseFloat(INVPayAmt) + parseFloat(POPayAmt));
        //            var TotalLPOsAmt = (parseFloat(INVLPOAmt) + parseFloat(POLPOAmt));
        //            TotalPayAmt = parseFloat(TotalPayAmt) + parseFloat($('#HFAllLPOHistory').val() == undefined ? "0" : $('#HFAllLPOHistory').val());
        //            var balAmt = parseFloat(TotalLPOsAmt) - parseFloat(TotalPayAmt);
        //            if (!IsKeyPress)
        //                $('[id$=txtAmount]').val((parseFloat(INVPayAmt) + parseFloat(POPayAmt)).toFixed(2));

        //            $('[id$=lblBalance]').text(balAmt.toFixed(2));
        //        }

    </script>
</asp:Content>
