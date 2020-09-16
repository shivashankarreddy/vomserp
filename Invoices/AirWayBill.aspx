<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="AirWayBill.aspx.cs" Inherits="VOMS_ERP.Invoices.AirWayBill" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Air Waybill Entry"
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
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="9">
                                        Flow to make a Bill of Lading/AWB:
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Port of Destination<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:DropDownList ID="ddlPod" runat="server" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlPod_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Customer Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:ListBox ID="lbCustomer" runat="server" CssClass="bcAspMultiSelectListBox" AutoPostBack="True"
                                            OnSelectedIndexChanged="lbCustomer_SelectedIndexChanged" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Proforma Invoice Number <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:ListBox ID="lbPInv" runat="server" CssClass="bcAspMultiSelectListBox" AutoPostBack="True"
                                            OnSelectedIndexChanged="lbPInv_SelectedIndexChanged" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px; height: 5px">
                                        <span class="bcLabel">Shipping Bill Number <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px; height: 5px">

                                        <asp:ListBox ID="lbShippingBill" runat="server" CssClass="bcAspMultiSelectListBox" AutoPostBack="True" OnSelectedIndexChanged="ddlShipBillNo_SelectedIndexChanged"
                                            SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="9">
                                        Shipping Line:
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">AWB Number<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtAwb" onkeypress="return isSomeSplChrs(event);" onchange="CheckAB()"
                                            runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Executable Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtExecDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Port of Discharge<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Place of Receipt<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtPlaceofReceipt" onkeypress="return isSomeSplChrs(event);" runat="server"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Place of Delivery <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <%--<td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Freight<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:DropDownList ID="ddlFreight" runat="server" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="--Select Freight--" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="To-Pay" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Pre-Paid" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField runat="server" ID="hdfempty" />                                        
                                    </td>--%>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Freight<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtFreight" runat="server" onblur="extractNumber(this,3,true);"
                                            MaxLength="18" onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Tare Weight(Kgs)<font color="red" size="2"> <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtTareweight" runat="server" onblur="extractNumber(this,3,true);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Gross Weight(Kgs)<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtGrossweight" runat="server" onblur="extractNumber(this,3,true);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Total No. of Pkgs<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtNoPkgs" runat="server" onkeypress="return isNumberKey(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Dimensions in Cms<font color="red" size="2"> </font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtDimensions" TextMode="MultiLine" runat="server"  Style="height: 43px;"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Total Pre-paid<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtTotalprepaid" onblur="extractNumber(this,2,false);" onkeyup="extractNumber(this,2,false);"
                                            onkeypress="return blockNonNumbers(this, event, true, false);" runat="server"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
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
                                    <td colspan="6" align="right">
                                        <center>
                                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                                <tbody>
                                                    <tr valign="middle">
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div2" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()"
                                                                    Text="Clear" OnClick="btnclear_Click" />
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
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtExecDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            if ($('#ctl00_ContentPlaceHolder1_lbCustomer :selected').length == 0) {
                ErrorMessage('Select atleast One Customer');
                $('[id$=ctl00_ContentPlaceHolder1_lbCustomer]').focus();
                return false;
            }
            else if ($('#ctl00_ContentPlaceHolder1_lbPInv :selected').length == 0) {
                ErrorMessage('Select atleast One Proforma Invoice No.');
                $('[id$=ctl00_ContentPlaceHolder1_lbPInv]').focus();
                return false;
            }
            else if ($('#ctl00_ContentPlaceHolder1_lbShippingBill :selected').length == 0) {
                ErrorMessage('Select atleast One Shipping Bill No.');
                $('[id$=ctl00_ContentPlaceHolder1_lbShippingBill]').focus();
                return false;
            }
            else if (($('[id$=txtAwb]').val()).trim() == '') {
                ErrorMessage('AWB Number is required.');
                $('[id$=txtAwb]').focus();
                return false;
            }
            else if (($('[id$=txtExecDate]').val()).trim() == '') {
                ErrorMessage('Executable Date is required.');
                $('[id$=txtExecDate]').focus();
                return false;
            }
            else if ($('[id$=ddlPrtDscrg]').val() == '0') {
                ErrorMessage('Place of Discharge is required.');
                $('[id$=ddlPrtDscrg]').focus();
                return false;
            }
//            else if (($('[id$=txtPlaceofReceipt]').val()).trim() == '') {
//                ErrorMessage('Place of receipt is required.');
//                $('[id$=txtPlaceofReceipt]').focus();
//                return false;
//            }
            else if ($('[id$=ddlPlcDlry]').val() == '0') {
                ErrorMessage('Place of Delivery is required.');
                $('[id$=ddlPlcDlry]').focus();
                return false;
            }
            //            else if (($('[id$=ddlFreight]').val()).trim() == '0') {
            //                ErrorMessage('Freight is required.');
            //                $('[id$=txtFreight]').focus();
            //                return false;
            //            }
            else if ((($('[id$=txtFreight]').val()).trim() == '') || (($('[id$=txtFreight]').val()).trim() == '0')) {
                ErrorMessage('Freight is required.');
                $('[id$=txtFreight]').focus();
                return false;
            }
            else if ((($('[id$=txtTareweight]').val()).trim() == '') || (($('[id$=txtTareweight]').val()).trim() == '0')) {
                ErrorMessage('Tare weight is required.');
                $('[id$=txtTareweight]').focus();
                return false;
            }
            //else if ((($('[id$=txtGrossweight]').val()).trim() == '') || (($('[id$=txtGrossweight]').val()).trim() == '0')) {
            else if ((($('[id$=txtGrossweight]').val()).trim() == '0') || (($('[id$=txtGrossweight]').val()).trim() == '')) {
                ErrorMessage('Gross weight is required.');
                $('[id$=txtGrossweight]').focus();
                return false;
            }
            else if ((($('[id$=txtNoPkgs]').val()).trim() == '0') || (($('[id$=txtNoPkgs]').val()).trim() == '')) {
                ErrorMessage('No. of packages is required.');
                $('[id$=txtNoPkgs]').focus();
                return false;
            }
//            else if (($('[id$=txtDimensions]').val()).trim() == '') {
//                ErrorMessage('Dimensions is required.');
//                $('[id$=txtDimensions]').focus();
//                return false;
//            }
            if (($('[id$=txtTotalprepaid]').val()).trim() == '') {
                ErrorMessage('Total Pre-Paid is required.');
                $('[id$=txtTotalprepaid]').focus();
                return false;
            }
             if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            return true;
        }

        function CheckAB() {
            var enqNo = $('[id$=txtAwb]').val();
            var result = AirWayBill.CheckAB(enqNo);
            if (result.value == false) {
                $('[id$=txtAwb]').val("");
                ErrorMessage('Airwaybill Number Exists.');
                $('[id$=txtAwb]').focus();
            }
        }


        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isDimension(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && charCode != 47 && charCode != 45 && charCode != 88 && charCode != 120 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function isSomeSplChrs(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>
