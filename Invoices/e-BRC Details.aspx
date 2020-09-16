<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="e-BRC Details.aspx.cs" Inherits="VOMS_ERP.Invoices.e_BRC_Details" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="e-BRC Details" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
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
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Shipping Bill No<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlShipBillNo" AutoPostBack="true" TabIndex="2"
                                            CssClass="bcAspdropdown" required="required" 
                                            OnSelectedIndexChanged="ddlShipBillNo_SelectedIndexChanged" 
                                            ontextchanged="ddlShipBillNo_clear">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Shipping Bill Port<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:DropDownList runat="server" ID="ddlportloading" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Bank's File No<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtbankfileno" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Upload date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtUplDt" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Bill ID No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtbillidno" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">BRC No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtBRCNo" runat="server" onkeypress='return isSomeSplChars(event);' onchange="CheckEnquiryNo();"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Realised Value<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtRealVal" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"  onchange="checkAmount();"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            MaxLength="15"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Deducted Value<font color="red" size="2"> <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtDedVal" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            MaxLength="15"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel"><asp:Label ID="LblTotAmount" runat="server" Text="Total Amt"></asp:Label><font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtTotAmt" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"  onchange="checkAmount();"
                                            MaxLength="15"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Date of Realisation<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtRealDt" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Currency<font color="red" size="2"> <b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlcurrency" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">BRC Status<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlBRCStatus" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="ACTIVE" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="INACTIVE" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">BRC Utilisation Status<font color="red" size="2"> <b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlBRCUtlStatus" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="PARTIAL " Value="1"></asp:ListItem>
                                            <asp:ListItem Text="FULL " Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Remarks<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="bcAsptextbox" MaxLength="500"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="bcTdnormal">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" style="width: 39%">
                                                        <span id="Span2" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" align="right">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click1" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div2" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnclear" Text="Clear" OnClick="btnClear_Click" />
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
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">

        var dateToday = new Date();
        var dateBack = new Date(dateToday.setDate(dateToday.getDate() - 365));
        var MaxDate = new Date(dateToday.setDate(dateToday.getDate() + 365));

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtUplDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtRealDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
    </script>
    <script type="text/javascript">

        function isSomeSplChrs(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function Myvalidations() {


            if (($('[id$=ddlShipBillNo]').val()).trim() == '0') {
                ErrorMessage('Shipping Bill Number is required.');
                $('[id$=ddlShipBillNo]').focus();
                return false;
            }
            else if (($('[id$=ddlportloading]').val()).trim() == '') {
                ErrorMessage('Shipping Bill Port is required.');
                $('[id$=ddlportloading]').focus();
                return false;
            }

            else if (($('[id$=txtbankfileno]').val()).trim() == '') {
                ErrorMessage('Bank File No. is required.');
                $('[id$=txtbankfileno]').focus();
                return false;
            }

            else if (($('[id$=txtUplDt]').val()).trim() == '') {
                ErrorMessage('Upload Date is required.');
                $('[id$=txtUplDt]').focus();
                return false;
            }

            else if (($('[id$=txtbillidno]').val()).trim() == '') {
                ErrorMessage('Bill Id No. is required.');
                $('[id$=txtbillidno]').focus();
                return false;
            }
            else if (($('[id$=txtBRCNo]').val()).trim() == '') {
                ErrorMessage('BRC No. is required.');
                $('[id$=txtBRCNo]').focus();
                return false;
            }
            else if (($('[id$=txtRealVal]').val()).trim() == '') {
                ErrorMessage('Realised Value is required.');
                $('[id$=txtRealVal]').focus();
                return false;
            }
            else if (($('[id$=txtDedVal]').val()).trim() == '') {
                ErrorMessage('Deducted Value is required.');
                $('[id$=txtGrossweight]').focus();
                return false;
            }
            else if (($('[id$=txtTotAmt]').val()).trim() == '') {
                ErrorMessage('Total Amount is required.');
                $('[id$=txtTotAmt]').focus();
                return false;
            }
            else if (($('[id$=txtRealDt]').val()).trim() == '') {
                ErrorMessage('Date of Realisation is required.');
                $('[id$=txtRealDt]').focus();
                return false;
            }
            else if (($('[id$=ddlcurrency]').val()) == '0') {
                ErrorMessage('Currency is required.');
                $('[id$=ddlcurrency]').focus();
                return false;
            }
            else if (($('[id$=ddlBRCStatus]').val()) == '0') {
                ErrorMessage('BRC Status is required.');
                $('[id$=ddlBRCStatus]').focus();
                return false;
            }
            else if (($('[id$=ddlBRCUtlStatus]').val()) == '0') {
                ErrorMessage('BRC Utilisation Status is required.');
                $('[id$=ddlBRCUtlStatus]').focus();
                return false;
            }
            if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            else
                return true;
        }
    </script>

    <script type="text/javascript">
        function CheckEnquiryNo() {
            var enqNo = $('[id$=txtBRCNo]').val();
            var result = e_BRC_Details.CheckEBRCNo(enqNo);
            if (result.value == false) {
                ErrorMessage('E-BRC No. in use.');
                $('[id$=txtBRCNo]').val('');
                $('[id$=txtBRCNo]').focus();
                return false;
            }
        }
        function checkAmount() {
            var Frist = document.getElementById('<%= txtTotAmt.ClientID %>').value;
            var Second = document.getElementById('<%= txtRealVal.ClientID %>').value;
            var result1 = (Frist - Second);
            var result = result1.toFixed(2);
            if (result < 0) {
                ErrorMessage('Realised Values should be Lessthan or Equal to Total Value.')
                $('[id$=txtRealVal]').val('');
                $('[id$=txtRealVal]').focus();
                $('[id$=txtDedVal]').val('');
            } else if (result > 0 || result == '0.00') {
                document.getElementById('<%= txtDedVal.ClientID %>').value = result.toString();
            }
        }
    
        
    </script>
</asp:Content>
