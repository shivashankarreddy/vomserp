<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CTOneTracking.aspx.cs" Inherits="VOMS_ERP.Logistics.CTOneTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable" colspan="6">
                <table style="width: 100%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="CT-1 Tracking" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 100%">
                    <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                        align="center">
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Select CT-1 No.<font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:DropDownList runat="server" ID="ddlCTOneNo" CssClass="bcAspdropdown" Height="22px"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlCTOneNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">CT-1 value <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtCTOneVal" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                            </td>
                            <td class="bcTdnormal">
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">FPO Numbers <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtFpoNos" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                                <asp:HiddenField ID="HFFpos" runat="server" />
                                <asp:HiddenField ID="HFLpos" runat="server" />
                                <asp:HiddenField ID="HFSupplierID" runat="server" />
                                <asp:HiddenField ID="HFCustID" runat="server" />
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">LPO Numbers <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtLpoNos" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                            </td>
                            <td class="bcTdnormal">
                            </td>
                        </tr>
                        <tr>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSupplierName" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                                <span class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>:</span>
                            </td>
                            <td class="bcTdnormal">
                                <asp:TextBox runat="server" ID="txtCustomerName" CssClass="bcAsptextbox" ReadOnly="true"></asp:TextBox>
                            </td>
                            <td class="bcTdnormal">
                            </td>
                            <td class="bcTdnormal">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="bcTdNewTable">
                                <div id="DivAREOneDetails" runat="server" style="overflow: auto; width: 98%; min-height: 100px;">
                                </div>
                                <asp:HiddenField ID="HFAccordions" runat="server" />
                            </td>
                        </tr>
                        <tr style="background-color: Gray; font-style: normal; color: White;">
                            <td colspan="6">
                                UnUtilised Details
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" align="center">
                                <table width="60%">
                                    <tr>
                                        <td>
                                            <span class="bcLabel">IsUnutilised <font color="red" size="2"><b></b></font>:</span>
                                        </td>
                                        <td class="bcTdnormal" align="left">
                                            <asp:CheckBox runat="server" ID="chkIsUnutilised" 
                                                 onchange="Unutilised();"  />
                                                <asp:HiddenField ID="hdfunutil"  runat="server" />
                                                <asp:HiddenField ID="hdfunutildate"  runat="server" />
                                        </td>
                                        <td>
                                            1.
                                        </td>
                                        <td>
                                            <span class="bcLabel">Date <font color="red" size="2"><b></b></font>:</span>
                                        </td>
                                        <td class="bcTdnormal">
                                            <asp:TextBox runat="server" ID="txtunUtilisedDt" Text="" CssClass="bcAsptextbox DatePicker"
                                                Enabled="false"></asp:TextBox>
                                        </td>
                                        <td>
                                            <span class="bcLabel"><asp:Label ID="LblAmount" runat="server" Text="Amount"></asp:Label><font color="red" size="2"><b></b></font>:</span>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtunutilisedAmt" Text="" CssClass="bcAsptextbox"
                                                Enabled="false" onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" class="bcTdNewTable">
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
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                    align="center">
                    <tr>
                        <td class="bcTdNewTable" colspan="6">
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="bcTdNewTable" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClientClick="javascript:validations()"
                                                    OnClick="btnSave_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                    Text="Clear" OnClick="btnClear_Click" />
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
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        function Unutilised() {
              var ischecked = $('[id$=chkIsUnutilised]').is(':checked');
              if (ischecked == true) {
                  $('[id$=txtunutilisedAmt]').val($('[id$=hdfunutil]').val());
                  $('[id$=txtunUtilisedDt]').val($('[id$=hdfunutildate]').val());
              }
              else {
               $('[id$=txtunutilisedAmt]').val("");
               $('[id$=txtunUtilisedDt]').val("");
              }
        }

        $(document).ready(function () {
            $('[id$=chkIsUnutilised]').click(function () {
                var ischecked = $('[id$=chkIsUnutilised]').is(':checked');
                if (ischecked == true) {
                    $('[id$=txtunUtilisedDt]').removeAttr("disabled");
                    $('[id$=txtunutilisedAmt]').removeAttr("disabled");
                }
                else {
                    $('[id$=txtunUtilisedDt]').attr("disabled", true);
                    $('[id$=txtunutilisedAmt]').attr("disabled", true);
                }
            });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('.DatePicker').datepicker({
                //maxDate: dateToday,
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });

//        $('[id$=txtunUtilisedDt]').datepicker({
//            //minDate: dateBack,
//            maxDate: dateToday,
//            dateFormat: 'dd-mm-yy',
//            changeMonth: true,
//            changeYear: true
////            , showAnim: 'blind'
////            , showButtonPanel: true
//        });


        $('[id$=txtunUtilisedDt]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
        });

        $(function () {
            var count = $('[id$=HFAccordions]').val();
            for (var i = 0; i <= count; i++) {
                $("#accordion" + i + "").accordion({
                    active: false,
                    autoHeight: false,
                    navigation: true,
                    collapsible: true
                });
            }
        });

        function CheckSBCopy(SNo) {
            var SBCopy = $("#ChkSBCopy" + SNo).is(':checked');
            var result = CTOneTracking.SBCopy(SNo, SBCopy);
            if (result.value == false) {
                $("#ChkSBCopy" + SNo).prop('checked', false);
                //ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        function ChkExchCopy(SNo) {
            var SBCopy = $("#ChkExchCopy" + SNo).is(':checked');
            var result = CTOneTracking.ChkExchCopy(SNo, SBCopy);
            if (result.value == false) {
                $("#ChkExchCopy" + SNo).prop('checked', false);
                //ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        function ChkExpCopy(SNo) {
            var SBCopy = $("#ChkExpCopy" + SNo).is(':checked');
            var result = CTOneTracking.ChkExpCopy(SNo, SBCopy);
            if (result.value == false) {
                $("#ChkExpCopy" + SNo).prop('checked', false);
                //ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        function SaveChanges(SNo) {
            var ExiseINVNo = $('[id$=txtExInvNo' + SNo + ']').val();
            var ExINVval = $('[id$=txtExInvVal' + SNo + ']').val();
            var ExINVdt = $('[id$=txtExciseInvDT' + SNo + ']').val();
            var SalesINVNo = $('[id$=txtSalesInvNo' + SNo + ']').val();
            var SalesINVDT = $('[id$=txtSalesInvDT' + SNo + ']').val();
            var SalesINVAmt = $('[id$=txtSalesInvAmt' + SNo + ']').val();

            var result = CTOneTracking.SaveChanges(SNo, ExiseINVNo, ExINVval, SalesINVNo, SalesINVDT, SalesINVAmt, ExINVdt);
            if (result.value == false) {
                $("#ChkSBCopy" + SNo).prop('checked', false);
                //ErrorMessage('This Internal Reference No. is already in Use');
            }
        }

        function CheckAres(RNo) {
            var org = $("#ChkOrig" + RNo).is(':checked');
            var dup = $("#ChkDup" + RNo).is(':checked');
            var Trip = $("#ChkTrip" + RNo).is(':checked');
            var green = $("#ChkGreen" + RNo).is(':checked');
            var blue = $("#ChkBlue" + RNo).is(':checked');

            var result = CTOneTracking.AreForms(RNo, org, dup, Trip, green, blue);
            if (result.value == false) {
                $("#ChkOrig" + RNo).prop('checked', false);
                $("#ChkDup" + RNo).prop('checked', false);
                $("#ChkTrip" + RNo).prop('checked', false);
                $("#ChkGreen" + RNo).prop('checked', false);
                $("#ChkBlue" + RNo).prop('checked', false);
            }
        }

        function CheckFullRow(SNo) {
            var isChecked = $("#ChkFullRow" + SNo).is(':checked');
            var result = CTOneTracking.CheckFullRow(SNo, isChecked);
            if (result.value != "") {
                $("#ChkFullRow" + SNo).prop('checked', false);
                ErrorMessage(result.value);
            }
        }
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            try {
                if (($('[id$=ddlCTOneNo]').val()).trim() == "00000000-0000-0000-0000-000000000000") {
                    ErrorMessage('CT-1 Number is required.');
                    $('[id$=ddlCTOneNo]').focus();
                    return false;
                }
                else if (($('[id$=txtCTOneVal]').val()).trim() == "") {
                    $('[id$=txtCTOneVal]').focus();
                    ErrorMessage('CT-1 value is required.');
                }
                else if (($('[id$=txtCTOneVal]').val()).trim() == "0") {
                    $('[id$=txtCTOneVal]').focus();
                    ErrorMessage('CT-1 value cannot be Zero.');
                }
                else if (($('[id$=txtFpoNos]').val()).trim() == "") {
                    $('[id$=txtFpoNos]').focus();
                    ErrorMessage('FPO numbers are required.');
                }
                else if (($('[id$=txtLpoNos]').val()).trim() == "") {
                    $('[id$=txtLpoNos]').focus();
                    ErrorMessage('LPO numbers are required.');
                }
                else if (($('[id$=txtSupplierName]').val()).trim() == "") {
                    $('[id$=txtSupplierName]').focus();
                    ErrorMessage('Supplier Name is required.');
                }
                else if (($('[id$=txtCustomerName]').val()).trim() == "") {
                    $('[id$=txtCustomerName]').focus();
                    ErrorMessage('Customer Name is required.');
                }
                else if (($('[id$=txtCustomerName]').val()).trim() == "") {
                    $('[id$=txtCustomerName]').focus();
                    ErrorMessage('Customer Name is required.');
                }

                var RowCount = $('#rounded-corner tbody tr').length;
                var ChkCount = 0;
                if ((parseInt(RowCount) / 9) > 0) {
                    var RCount = (parseInt(RowCount) / 9);
                    for (var i = 1; i <= RCount; i++) {
                        var Checked = $("#ChkFullRow" + i).is(':checked');
                        if (Checked == true) {
                            ChkCount = parseInt(ChkCount) + 1;

                            if (($('[id$=txtARENo' + i + ']').val()).trim() == "") {
                                $('[id$=txtARENo' + i + ']').focus();
                                ErrorMessage('ARE-1 No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtAREDT' + i + ']').val()).trim() == "") {
                                $('[id$=txtAREDT' + i + ']').focus();
                                ErrorMessage('ARE-1 Date is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtAREVal' + i + ']').val()).trim() == "") {
                                $('[id$=txtAREDT' + i + ']').focus();
                                ErrorMessage('ARE-1 Date is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkOrig" + i).is(':checked') == false) {
                                $('[id$=ChkOrig' + i + ']').focus();
                                ErrorMessage('ARE-1 Form original is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkDup" + i).is(':checked') == false) {
                                $('[id$=ChkDup' + i + ']').focus();
                                ErrorMessage('ARE-1 Form Duplicate is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkTrip" + i).is(':checked') == false && $("#ChkBlue" + i).is(':checked') == false) {
                                //$('[id$=ChkTrip' + i + ']').focus();
                                ErrorMessage('ARE-1 Form Triplicate/Blue is required in row no. : ' + i + '');
                                return false;
                            }
                            //                            else if ($("#ChkGreen" + i).is(':checked') == false) {
                            //                                $('[id$=ChkGreen' + i + ']').focus();
                            //                                ErrorMessage('ARE-1 Form Green is required in row no. : ' + i + '');
                            //                                return false;
                            //                            }
                            //                            else if ($("#ChkBlue" + i).is(':checked') == false) {
                            //                                $('[id$=ChkBlue' + i + ']').focus();
                            //                                ErrorMessage('ARE-1 Form Blue is required in row no. : ' + i + '');
                            //                                return false;
                            //                            }
                            else if (($('[id$=txtGRNNo' + i + ']').val()).trim() == "") {
                                $('[id$=txtGRNNo' + i + ']').focus();
                                ErrorMessage('GRN No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtGRNDT' + i + ']').val()).trim() == "") {
                                $('[id$=txtGRNDT' + i + ']').focus();
                                ErrorMessage('GRN Date is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txPrfInvNo' + i + ']').val()).trim() == "") {
                                $('[id$=txPrfInvNo' + i + ']').focus();
                                ErrorMessage('proforma Invoice No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtShpngBlNo' + i + ']').val()).trim() == "") {
                                $('[id$=txtShpngBlNo' + i + ']').focus();
                                ErrorMessage('Shipping Bill No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtAREVal' + i + ']').val()).trim() == "") {
                                $('[id$=txtAREVal' + i + ']').focus();
                                ErrorMessage('Bill of Lading No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtMTRNo' + i + ']').val()).trim() == "") {
                                $('[id$=txtMTRNo' + i + ']').focus();
                                ErrorMessage('Mate Receipt No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkSBCopy" + i).is(':checked') == false) {
                                $('[id$=ChkSBCopy' + i + ']').focus();
                                ErrorMessage('Shipping Bill Copy is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkExchCopy" + i).is(':checked') == false) {
                                $('[id$=ChkExchCopy' + i + ']').focus();
                                ErrorMessage('Excise Shipping Bill Copy is required in row no. : ' + i + '');
                                return false;
                            }
                            else if ($("#ChkExpCopy" + i).is(':checked') == false) {
                                $('[id$=ChkExpCopy' + i + ']').focus();
                                ErrorMessage('Export Shipping Bill Copy is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txBRCNo' + i + ']').val()).trim() == "") {
                                $('[id$=txBRCNo' + i + ']').focus();
                                ErrorMessage('BRC No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtBRCDt' + i + ']').val()).trim() == "") {
                                $('[id$=txtBRCDt' + i + ']').focus();
                                ErrorMessage('BRC Date is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtBRCAmt' + i + ']').val()).trim() == "") {
                                $('[id$=txtBRCAmt' + i + ']').focus();
                                ErrorMessage('BRC Amount is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtBRCAmt' + i + ']').val()).trim() != "" && ($('[id$=txtBRCAmt' + i + ']').val()).trim() == "0") {
                                $('[id$=txtBRCAmt' + i + ']').focus();
                                ErrorMessage('BRC Amount cannot be Zero in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtSalesInvNo' + i + ']').val()).trim() == "") {
                                $('[id$=txtSalesInvNo' + i + ']').focus();
                                ErrorMessage('Sales Invoice No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtSalesInvAmt' + i + ']').val()).trim() == "") {
                                $('[id$=txtSalesInvAmt' + i + ']').focus();
                                ErrorMessage('Sales Invoice Amount is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtSalesInvAmt' + i + ']').val()).trim() != "" && ($('[id$=txtSalesInvAmt' + i + ']').val()).trim() == "0") {
                                $('[id$=txtSalesInvAmt' + i + ']').focus();
                                ErrorMessage('Sales Invoice Amount cannot be Zero in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtSalesInvDT' + i + ']').val()).trim() == "") {
                                $('[id$=txtSalesInvDT' + i + ']').focus();
                                ErrorMessage('Sales Invoice Date is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtExInvNo' + i + ']').val()).trim() == "") {
                                $('[id$=txtExInvNo' + i + ']').focus();
                                ErrorMessage('Ex Invoice No. is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtExInvVal' + i + ']').val()).trim() == "") {
                                $('[id$=txtExInvVal' + i + ']').focus();
                                ErrorMessage('Ex Invoice Amount is required in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtExInvVal' + i + ']').val()).trim() != "" && ($('[id$=txtExInvVal' + i + ']').val()).trim() == "0") {
                                $('[id$=txtExInvVal' + i + ']').focus();
                                ErrorMessage('Ex Invoice Amount cannot be Zero in row no. : ' + i + '');
                                return false;
                            }
                            else if (($('[id$=txtExciseInvDT' + i + ']').val()).trim() == "") {
                                $('[id$=txtExciseInvDT' + i + ']').focus();
                                ErrorMessage('Ex Invoice Date is required in row no. : ' + i + '');
                                return false;
                            }
                        }
                    }

                    if ($('[id$=chkIsUnutilised]').is(':checked') == true) {
                        if (($('[id$=txtunUtilisedDt]').val()).trim() == "") {
                            $('[id$=txtunUtilisedDt]').focus();
                            ErrorMessage('UnUtilised Date is required.');
                            return false;
                        }

                        if (($('[id$=txtunutilisedAmt]').val()).trim() == "") {
                            $('[id$=txtunutilisedAmt]').focus();
                            ErrorMessage('Amount is required.');
                            return false;
                        }
                        else if (parseInt(($('[id$=txtunutilisedAmt]').val()).trim()) == 0) {
                            $('[id$=txtunutilisedAmt]').focus();
                            ErrorMessage('Amount cannot be zero.');
                            return false;
                        }
                    }
                    if ($('[id$=DivComments]').css("visibility") == "visible") {
                        if (($('[id$=txtComments]').val()).trim() == '') {
                            ErrorMessage('Comment is Required.');
                            $('[id$=txtComments]').focus();
                            return false;
                        }
                    }
                    if (ChkCount == 0) {
                        ErrorMessage('Select atleast Single ARE-1 Form.');
                        return false;
                    }
                }
                else {
                    ErrorMessage('No Are-1 forms to save');
                    return false;
                }
                //return false;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }
    </script>
    <script type="text/javascript">
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
    </script>
</asp:Content>
