<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="BillLadingEntry.aspx.cs" Inherits="VOMS_ERP.Invoices.BillLadingEntry" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Bill of Lading" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span11" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr style="display: none;">
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Port Of Destination<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPlcDstnsn" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlPlcDstnsn_SelectedIndexChanged">
                                            <asp:ListItem Text="-- Select --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td align="left">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span13" class="bcLabel">Customer Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lbCustomers" SelectionMode="Multiple" OnSelectedIndexChanged="lbCustomers_SelectedIndexChanged"
                                            CssClass="bcAspMultiSelectListBox" AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">Proforma Invoice No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbPrfmaInvc" SelectionMode="Multiple" OnSelectedIndexChanged="lbPrfmaInvc_SelectedIndexChanged"
                                            CssClass="bcAspMultiSelectListBox" AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Shipping bill Numbers<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbShpngBil" SelectionMode="Multiple" AutoPostBack="true"
                                            CssClass="bcAspMultiSelectListBox" OnSelectedIndexChanged="ddlShipBillNo_SelectedIndexChanged">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Shipping Line <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtShippingLine" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Booking No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtBookingNo" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span34" class="bcLabel">Bill of Lading No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtbillLading" onkeypress="return isSomeSplChrs(event);"
                                            onchange="GetBOLN()" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">SOB date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtSOBDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Vessel <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtVessel" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Voyage <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtVoyage" onkeypress="return isSomeSplChrs(event);"
                                            TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Port of Loading <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPrtLdng" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Port of Discharge <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPrtDscrg" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Place of Receipt <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPlcRcpt" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Place of Delivery <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlPlcDlry" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span10" class="bcLabel">Freight<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <%--<asp:DropDownList ID="ddlFreight" runat="server" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlFreight_SelectedIndexChanged">
                                            <asp:ListItem Text="--Select Freight--" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="To-Pay" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Pre-Paid" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField runat="server" ID="hdfempty" />--%>
                                        <asp:TextBox ID="txtFreight" runat="server" onblur="extractNumber(this,3,true);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable" align="center">
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable" align="center">
                                        <div id="dvContainerDetails" runat="server" style="width: 80%;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">IDF No <font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtIDFNo" onkeypress="return isSomeSplChrs(event);"
                                            onchange="checkAmount();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span15" class="bcLabel">FERI No <font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtFERINo" onkeypress="return isSomeSplChrs(event);"
                                            onchange="checkAmount();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">ECTN No<font color="red" size="2"><b></b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtECTNNo" onkeypress="return isSomeSplChrs(event);"
                                            onchange="checkAmount();" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Tare weight (kgs)<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtTareWeight" onblur="extractNumber(this,3,true);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Gross weight (kgs)<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtGrWeight" onblur="extractNumber(this,3,true);"
                                            onkeyup="extractNumber(this,3,true);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Total No. of packages<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtTotalPkgs" onkeypress="return isNumberKey(event);"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span16" class="bcLabel">DOI of BL <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtDOIDt" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
  
                                    <td colspan="2" class="bcTdnormal">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" style="width: 39%">
                                                        <span id="Span12" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                </table>
            </td>
        </tr>
    </table>
    <%--OnCancelScript="CancelSelection()" CancelControlID="btnCancel" OnOkScript="Validations()"--%>
    <%--<ajax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="hdfempty"
        PopupControlID="Panel1" PopupDragHandleControlID="PopupHeader" Drag="true" BackgroundCssClass="modalBackground">
    </ajax:ModalPopupExtender>
    <asp:Panel ID="Panel1" runat="server" BorderStyle="Solid" BorderWidth="3px" CssClass="PopUpStyle">
        <div class="Popup">
            <div class="PopupHeader" id="PopupHeader">
                <center>
                    <h3>
                        Freight Charges
                    </h3>
                </center>
            </div>
            <div class="PopupBody">
                <table>
                    <tr>
                        <td colspan="2">
                            <div id="divMyMessage1" runat="server" align="center" class="formError1" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="2">
                            All <font color="red" size="2"><b>*</b></font>Fields are Mandatory
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="Span23" class="bcLabel">Payment Mode <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlPmtPop" CssClass="bcAspdropdown" onchange="DisplayChanges()">                                
    <asp:ListItem Text="-- Select --" Value="0">
    </asp:ListItem>
    <asp:ListItem Text="Cash" Value="1">
    </asp:ListItem>
    <asp:ListItem Text="Cheque" Value="2">
    </asp:ListItem>
    </asp:DropDownList> </td> </tr>
    <tr>
        <td colspan="2">
            <div id="dvcash" style="display: none;">
                <table>
                    <tr>
                        <td>
                            <span id="Span24" class="bcLabel">Amount <font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isNumberKey(event)"
                                CssClass="bcAsptextbox" ID="txtAmt"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvchque" style="display: none;">
                <table>
                    <tr>
                        <td>
                            <span id="Span25" class="bcLabel">Cheque No. <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isNumberKey(event)"
                                CssClass="bcAsptextbox" ID="txtChqno"></asp:TextBox>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="Span27" class="bcLabel">Cheque Date <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" Width="50%" CssClass="bcAsptextbox" ID="txtChqDt"></asp:TextBox>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="Span28" class="bcLabel">NEFT/RTGS Code :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="10" Width="50%" onkeypress="return isAlphaNumaric(event)"
                                CssClass="bcAsptextbox" ID="txtRtgsCd"></asp:TextBox>
                            <br />
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2">
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <asp:Button ID="btnDone" runat="server" Text="Done" OnClick="btnDone_Click" OnClientClick="return Validations()" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
        </td>
    </tr>
    </table> </div> </div> </asp:Panel>--%>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            var dateToday = new Date();

            $('[id$=txtSOBDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtSOBDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
//            $('[id$=txtDOIDt]').datepicker({
//                dateFormat: 'dd-mm-yy',
//                changeMonth: true,
//                changeYear: true,
//                maxDate: dateToday
//            });
            $('[id$=txtDOIDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

            $('[id$=txtChqDt]').datepicker({
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
        function isSomeSplChrs(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        function isDecimalKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function AddNewRow(RowNo) {
            var CType = $('[id$=ddlCType' + RowNo + ']').val();
            var CNO = $('[id$=txtCNO' + RowNo + ']').val();
            var CSealNo = $('[id$=txtCSealNo' + RowNo + ']').val();
            var LSealNo = $('[id$=txtLSealNo' + RowNo + ']').val();

            if (CType.trim() == '0') {
                $('[id$=ddlCType' + RowNo + ']').focus();
                ErrorMessage('Container Type is required.');
                return false;
            }
            else if (CNO == '') {
                $('[id$=txtCNO' + RowNo + ']').focus();
                ErrorMessage('Container Number cannot be empty.');
                return false;
            }
            else if (CSealNo == '') {
                $('[id$=txtCSealNo' + RowNo + ']').focus();
                ErrorMessage('Container Seal Number cannot be empty.');
                return false;
            }
            else if (LSealNo == '') {
                $('[id$=txtLSealNo' + RowNo + ']').focus();
                ErrorMessage('L Seal Number cannot be empty.');
                return false;
            }
            else {
                var result = BillLadingEntry.AddNewRow();
                var getDivContainer = GetClientID("dvContainerDetails").attr("id");
                $('#' + getDivContainer).html(result.value);
            }
        }

        function FillItemGrid(RowNo) {
            var CType = $('[id$=ddlCType' + RowNo + ']').val();
            var CNO = $('[id$=txtCNO' + RowNo + ']').val();
            var CSealNo = $('[id$=txtCSealNo' + RowNo + ']').val();
            var LSealNo = $('[id$=txtLSealNo' + RowNo + ']').val();

            var result = BillLadingEntry.SaveChanges(RowNo, CType, CNO, CSealNo, LSealNo);
            var getDivContainer = GetClientID("dvContainerDetails").attr("id");
            $('#' + getDivContainer).html(result.value);
        }

        function doConfirm(RowNo) {
            if (confirm("Are you sure you want to Delete this Record...?")) {
                var result = BillLadingEntry.DeleteRecord(RowNo);
                var getDivContainer = GetClientID("dvContainerDetails").attr("id");
                $('#' + getDivContainer).html(result.value);
            }
            else {
                return false;
            }
        }

        function Myvalidations() {
            try {
                var GdnItems = 0;
                if ($('#tblPaymentTerms').length)
                    GdnItems = $('#tblPaymentTerms tbody tr').length;
                if ($('#ctl00_ContentPlaceHolder1_lbCustomers :selected').length == 0) {
                    ErrorMessage('Select atleast One Customer');
                    $('[id$=ctl00_ContentPlaceHolder1_lbCustomers]').focus();
                    return false;
                }
                else if ($('#ctl00_ContentPlaceHolder1_lbPrfmaInvc :selected').length == 0) {
                    ErrorMessage('Select atleast One Proforma Invoice No.');
                    $('[id$=ctl00_ContentPlaceHolder1_lbPrfmaInvc]').focus();
                    return false;
                }
                else if ($('#ctl00_ContentPlaceHolder1_lbShpngBil :selected').length == 0) {
                    ErrorMessage('Select atleast One Shipping Bill No.');
                    $('[id$=ctl00_ContentPlaceHolder1_lbShpngBil]').focus();
                    return false;
                }
                else if (($('[id$=txtShippingLine]').val()).trim() == '') {
                    ErrorMessage('Shipping Line is required.');
                    $('[id$=txtShippingLine]').focus();
                    return false;
                }
                else if (($('[id$=txtBookingNo]').val()).trim() == '') {
                    ErrorMessage('Booking Number is required.');
                    $('[id$=txtBookingNo]').focus();
                    return false;
                }
                else if (($('[id$=txtbillLading]').val()).trim() == '') {
                    ErrorMessage('Bill of lading is required.');
                    $('[id$=txtbillLading]').focus();
                    return false;
                }
                else if (($('[id$=txtSOBDt]').val()).trim() == '') {
                    ErrorMessage('SOB date is required.');
                    $('[id$=txtSOBDt]').focus();
                    return false;
                }
                else if (($('[id$=txtVessel]').val()).trim() == '') {
                    ErrorMessage('Vessel is required.');
                    $('[id$=txtVessel]').focus();
                    return false;
                }
                else if (($('[id$=txtVoyage]').val()).trim() == '') {
                    ErrorMessage('Voyage is required.');
                    $('[id$=txtVoyage]').focus();
                    return false;
                }
                else if (($('[id$=ddlPrtLdng]').val()).trim() == '0') {
                    ErrorMessage('Port of loading is required.');
                    $('[id$=ddlPrtLdng]').focus();
                    return false;
                }
                else if (($('[id$=ddlPrtDscrg]').val()).trim() == '0') {
                    ErrorMessage('Port of discharge is required.');
                    $('[id$=ddlPrtDscrg]').focus();
                    return false;
                }
                else if (($('[id$=txtPlcRcpt]').val()).trim() == '') {
                    ErrorMessage('Place of receipt is required.');
                    $('[id$=txtPlcRcpt]').focus();
                    return false;
                }
                else if (($('[id$=ddlPlcDlry]').val()).trim() == '0') {
                    ErrorMessage('Place of delivery is required.');
                    $('[id$=ddlPlcDlry]').focus();
                    return false;
                }
                else if ((($('[id$=txtFreight]').val()).trim() == '0') || (($('[id$=txtFreight]').val()).trim() == '')) {
                    ErrorMessage('Freight is required.');
                    $('[id$=txtFreight]').focus();
                    return false;
                } //             Container Details is required HERE.......................................................................
                else if (GdnItems > 0) {
                    for (var i = 1; i <= GdnItems; i++) {
                        var CType = $('[id$=ddlCType' + i + ']').val();
                        var CNO = $('[id$=txtCNO' + i + ']').val();
                        var CSealNo = $('[id$=txtCSealNo' + i + ']').val();
                        var LSealNo = $('[id$=txtLSealNo' + i + ']').val();

                        if (CType.trim() == '0') {
                            $('[id$=ddlCType' + i + ']').focus();
                            ErrorMessage('Container Type is required in RowNo : ' + i + '.');
                            return false;
                        }
                        else if (CNO == '') {
                            $('[id$=txtCNO' + i + ']').focus();
                            ErrorMessage('Container Number cannot be empty in RowNo : ' + i + '.');
                            return false;
                        }
                        else if (CSealNo == '') {
                            $('[id$=txtCSealNo' + i + ']').focus();
                            ErrorMessage('Container Seal Number cannot be empty in RowNo : ' + i + '.');
                            return false;
                        }
                        else if (LSealNo == '') {
                            $('[id$=txtLSealNo' + i + ']').focus();
                            ErrorMessage('L Seal Number cannot be empty in RowNo : ' + i + '.');
                            return false;
                        }
                    }
                }
                else
                    return false;
                if ((($('[id$=txtIDFNo]').val()).trim() == '') && (($('[id$=txtECTNNo]').val()).trim() == '') && (($('[id$=txtFERINo]').val()).trim() == '')) {
                    ErrorMessage('IDF/FERI/ETC Number is required.');
                    $('[id$=txtIDFNo]').focus();
                    return false;
                }
                //                else if (($('[id$=txtECTNNo]').val()).trim() == '') {
                //                    ErrorMessage('ECTN Number is required.');
                //                    $('[id$=txtECTNNo]').focus();
                //                    return false;
                //                }
                //                
                else if (($('[id$=txtDate]').val()).trim() == '') {
                    ErrorMessage('Date is required.');
                    $('[id$=txtDate]').focus();
                    return false;
                }
                else if ((($('[id$=txtTareWeight]').val()).trim() == '0') || (($('[id$=txtTareWeight]').val()).trim() == '')) {
                    ErrorMessage('Tare Weight is required.');
                    $('[id$=txtTareWeight]').focus();
                    return false;
                }
                else if ((($('[id$=txtGrWeight]').val()).trim() == '0')||(($('[id$=txtGrWeight]').val()).trim() == '')) {
                    ErrorMessage('Gross Weight is required.');
                    $('[id$=txtGrWeight]').focus();
                    return false;
                }
                else if (($('[id$=txtTotalPkgs]').val()).trim() == '0' || ($('[id$=txtTotalPkgs]').val()).trim() == '') {
                    ErrorMessage('Total No. of packages are required.');
                    $('[id$=txtTotalPkgs]').focus();
                    return false;
                }
                else if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
            }
        }

        function Validations_Cancel() {
            $("#ddlFreight").val("0");
            $('[id$=ModalPopupExtender1]').hide();
            return false;
        }

        function CancelSelection() {
            document.getElementById("ctl00_ContentPlaceHolder1_ddlFrt").value = "0";
            document.getElementById("ctl00_ContentPlaceHolder1_ddlFrt").selectedIndex = -1;
            $('[id$=ddlPmtPop]').val("0");
            $('[id$=txtAmt]').val("");
            $('[id$=txtChqno]').val("");
            $('[id$=txtChqDt]').val("");
            $('[id$=txtRtgsCd]').val("");
            document.getElementById("ctl00_ContentPlaceHolder1_ModalPopupExtender1").hide();
            $find('ctl00_ContentPlaceHolder1_ModalPopupExtender1').hide();
        }

        function DisplayChanges() {
            if (($('[id$=ddlPmtPop]').val()).trim() == '1') {
                $('[id$=dvcash]').show();
                $('[id$=dvchque]').hide();
            }
            else if (($('[id$=ddlPmtPop]').val()).trim() == '2') {
                $('[id$=dvchque]').show();
                $('[id$=dvcash]').hide();
            }
            else {
                $('[id$=dvcash]').hide();
                $('[id$=dvchque]').hide();
            }
            $('[id$=ddlPmtMd]').val($('[id$=ddlPmtPop]').val());
        }

        function GetBOLN() {
            var enqNo = $('[id$=txtbillLading]').val();
            var result = BillLadingEntry.GetBOLN(enqNo);
            if (result.value == false) {
                $('[id$=txtbillLading]').val("");
                ErrorMessage('Bill of Lading Number Exists.');
                $('[id$=txtbillLading]').focus();
            }
        }


        function checkAmount() {
            //var Frist = document.getElementById('<%= txtIDFNo.ClientID %>').value;
            // var Second = document.getElementById('<%= txtFERINo.ClientID %>').value;

            if (document.getElementById('<%= txtIDFNo.ClientID %>').value != '') {
                document.getElementById('<%= txtFERINo.ClientID %>').disabled = true;
                document.getElementById('<%= txtECTNNo.ClientID %>').disabled = true;

            } else if (document.getElementById('<%= txtFERINo.ClientID %>').value != '') {
                document.getElementById('<%= txtIDFNo.ClientID %>').disabled = true;
                document.getElementById('<%= txtECTNNo.ClientID %>').disabled = true;

            } else if (document.getElementById('<%= txtECTNNo.ClientID %>').value != '') {

                document.getElementById('<%= txtIDFNo.ClientID %>').disabled = true;
                document.getElementById('<%= txtFERINo.ClientID %>').disabled = true;

            } else {
                document.getElementById('<%= txtIDFNo.ClientID %>').disabled = false;
                document.getElementById('<%= txtFERINo.ClientID %>').disabled = false;
                document.getElementById('<%= txtECTNNo.ClientID %>').disabled = false;


            }


        }

    </script>
</asp:Content>
