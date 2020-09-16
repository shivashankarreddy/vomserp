<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="BillPaymentApproval.aspx.cs" Inherits="VOMS_ERP.Accounts.BillPaymentApproval"
    Title="" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Bill Payment Approval"
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
                                        <asp:TextBox runat="server" ID="txtRefNo" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
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
                                        <span id="Span8" class="bcLabel">
                                            <asp:Label ID="LblBAmount" runat="server" Text="Balance Amount :"></asp:Label></span>
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
                                                    <asp:TextBox ID="txtProInfNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span2" class="bcLabel">Pro. Inv. Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtProInvNoDate" runat="server" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span id="Span3" class="bcLabel">Export Invoice No.:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtExportInvNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span5" class="bcLabel">Export Invoice Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtExportInvDate" runat="server" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span id="Span6" class="bcLabel">Form &#39;C&#39; or &#39;H&#39; No.:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtFormCorHNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span id="Span7" class="bcLabel">Form &#39;C&#39; or &#39;H&#39; Date:</span>
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox ID="txtFormCorHNoDate" runat="server" CssClass="bcAsptextbox DatePicker"></asp:TextBox>
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
                                                                        <asp:Label ID="LblPaymntAmt" runat="server" Text="Amount"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td>
                                                                        1
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtChequeNo" runat="server" CssClass="bcAsptextbox" onchange="CheckChequeNo();"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBankName" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtUTRNo" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
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
                                                        &nbsp;&nbsp;&nbsp;Reject Details
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="bcTdnormal" width="20%">
                                                        <span id="Span9" class="bcLabel">Reasons for Reject:</span>
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
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane3" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">Attachments</a></Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel2" runat="server" Width="98%">
                                                                <table>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:Label ID="lblstatus" runat="server" Style="font-family: Arial; font-size: small;"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td valign="top">
                                                                            <ajax:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnClientUploadError="uploadError"
                                                                                OnClientUploadComplete="uploadComplete" OnClientUploadStarted="uploadStarted"
                                                                                UploaderStyle="Modern" CompleteBackColor="LightGreen" UploadingBackColor="Yellow"
                                                                                ThrobberID="ThrobberImg" OnUploadedComplete="FileUploadComplete" CssClass="FileUploadClass" />
                                                                            <asp:Image runat="server" ID="ThrobberImg" ImageUrl="~/images/uploadingImage.gif"
                                                                                AlternateText="loading" />
                                                                        </td>
                                                                        <td valign="top">
                                                                            <div id="divListBox" runat="server" width="221px">
                                                                            </div>
                                                                        </td>
                                                                        <td valign="top">
                                                                            <a id="lnkdelete" href="javascript:void(0)">Delete Item</a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </Content>
                                                    </ajax:AccordionPane>
                                                </Panes>
                                            </ajax:Accordion>
                                        </div>
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
                                                        <%--<td align="center" valign="middle" class="bcTdButton" style="visibility: hidden">
                                                            <div id="Div4" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnsavenew" Text="Save & New" />
                                                            </div>
                                                        </td>--%>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div1" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                                            </div>
                                                        </td>
                                                        <td align="center" valign="middle" class="bcTdButton">
                                                            <div id="Div4" class="bcButtonDiv">
                                                                <asp:LinkButton runat="server" ID="btnReject" Visible="false" Text="Reject" OnClick="btnReject_Click" />
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
            background: #FFCF8B !important; /*border-left: 0.1ex solid #FFCF8B !important;
    border-right: 0.1ex solid #FFCF8B !important;
    border-top: 0.3ex solid #FFCF8B !important;*/
        }
    </style>
    <script type="text/javascript">
        function uploadComplete() {
            var result = BillPaymentApproval.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                //$get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
                SuccessMessage('File Uploaded Successfully.');
            }
            /* Clear Content */
            var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
            var txts = AsyncFileUpload.getElementsByTagName("input");
            for (var i = 0; i < txts.length; i++) {
                txts[i].value = "";
                txts[i].style.backgroundColor = "transparent";
            }
        }
        function uploadError() {
            //$get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            //$get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != "") {
                if (confirm("Are you sure you want to delete selected attachment ?")) {
                    var result = BillPaymentApproval.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                    SuccessMessage('Selected Attachment Deleted Successfully.');
                }
            }
            Expnder();
        });
    </script>
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

            $("#txtAmt0").on('keyup', function () {
                //CheckLPOAmount_INV();
            });

            function CheckLPOAmount_INV() {
                //var INVs = $('[id$=HFINV_LPOIDs]').val().split(',');
                var LPOID = $("#ddlLPOs0").val();
                var Amt = $("#txtAmt0").val();
                var HField = ""; var TotVal = 0;
                var ActualLPOAmnt = $("#txtLPOAMT0").val();
                var RwCunt = $("#tblLPOs > tbody > tr").length;

                for (var i = 1; i <= RwCunt; i++) {
                    if (LPOID == $('[id$=HFLPOID' + i + ']').val()) {
                        HField = $('[id$=HFLblAmt' + i + ']').val();
                        TotVal = parseFloat(TotVal) + parseFloat(HField);
                    }
                }
                if (RwCunt == 0) {
                    TotVal = Amt;
                }
                else
                    TotVal += parseFloat(Amt);
                var result = BillPaymentApproval.GetLPOAmountAlert(LPOID, Amt, ActualLPOAmnt, TotVal);
                var LPOAmt = $("#txtLPOAMT0").val();
                //var LPOHist = $('[id$=HFLPOsHistorySum]').val() == "" ? 0 : $('[id$=HFLPOsHistorySum]').val();
                var PaidAmt = 0;
                if ($('[id$=HFAllLPOHistory]').length > 0)
                    PaidAmt = $('[id$=HFAllLPOHistory]').val() == "" ? 0 : $('[id$=HFAllLPOHistory]').val();
                var FinaAmt = parseFloat(ActualLPOAmnt) - parseFloat(PaidAmt);
                //alert("PAMt "+PaidAmt);
                if (result.value != "") {
                    var Condition = ((parseFloat(LPOAmt) * 28) / 100);
                    var MinPay = parseFloat(FinaAmt) - parseFloat(Condition);
                    var MaxPay = parseFloat(FinaAmt) + parseFloat(Condition);
                    //alert("Name : " +Condition+" Location : " +Condition);
                    //if (parseFloat(result.value) < 0) {
                    //                    if (parseFloat(Amt) < parseFloat(MinPay)) {
                    //                        ErrorMessage('Amount entered is 5% Less than LPO Amount.');
                    //                        //if (!confirm('Amount entered is Upto 5% Less than LPO Amount. Do you want to continue ?'))
                    //                        $("#txtAmt0").val('0');
                    //                    }
                    //                    else 

                    if (parseFloat(Amt) > parseFloat(MaxPay)) { //&& 1 == 2) {//Here Placed Condition 1==2 because as per GST Requirement need to increase more than 5% 
                        ErrorMessage('Amount entered is 28% Greater than LPO Amount.');
                        $("#txtAmt0").val('0');
                    }

                }
            }

            function AddNewRow() {
                var LPOID = $("#ddlLPOs0").val();
                var LPONo = $("#ddlLPOs0 :selected").text();
                var LPODT = $('#txtLPODate0').val();
                var LPOAMT = ($('#txtLPOAMT0').val() == '' || $('#txtLPOAMT0').val() == '.') ? 0 : $('#txtLPOAMT0').val();
                var PrfINVID = '0'; // $('#HFPrfInvID0').val();
                var PrfInvNo = $('#txtPrfInvNo0').val();
                var PrfInvAmt = $('#txtPrfInvAmt0').val();
                var TaxInvoice = ""; // $('#txtTaxInvoice0').val();
                var CrntDT = $('#txtCrntDate0').val();
                var Amt = ($('#txtAmt0').val() == '' || $('#txtAmt0').val() == '.') ? 0 : $('#txtAmt0').val();
                var Remarks = $('#txtremarks0').val();
                var RowCount = (Number($('#divGridOne tbody tr').length));

                if (LPOID != '00000000-0000-0000-0000-000000000000' && LPODT != '' && parseFloat(LPOAMT) > 0 && PrfInvNo != '' && PrfINVID != '' && CrntDT != '' && parseFloat(Amt) > 0) {
                    var result = BillPaymentApproval.AddNewRow(RowCount, LPOID, LPONo, LPODT, LPOAMT, PrfINVID, PrfInvNo, PrfInvAmt, TaxInvoice, CrntDT, Amt, Remarks);
                    $('#tblLPOs tbody').html('');
                    $('#tblLPOs tbody').append(result.value);

                    $("#ddlLPOs0").val('00000000-0000-0000-0000-000000000000');
                    $('#txtLPODate0').val('');
                    $('#txtLPOAMT0').val('');
                    $('#HFPrfInvID0').val('');
                    $('#txtPrfInvNo0').val('');
                    $('#txtPrfInvAmt0').val('');
                    //$('#txtTaxInvoice0').val('');
                    $('#txtCrntDate0').val('');
                    $('#txtAmt0').val('');
                    $('#txtremarks0').val('');
                    $("#ddlLPOs0").focus();

                    var AllLPOIDs = $('[id$=HFINV_LPOIDs]').val();
                    if (AllLPOIDs == "")
                        AllLPOIDs = LPOID;
                    else
                        AllLPOIDs = AllLPOIDs + ',' + LPOID;
                    $('[id$=HFINV_LPOIDs]').val(AllLPOIDs);
                    $('[id$=HFLPOsHistorySum]').val('');
                    var LPOs = $('[id$=HFPO_LPOIDs]').val();
                    GetHistory(LPOID, LPOs, false, true);
                    RemoveLPOid(LPOID, true);
                    GetBal(false);
                    $('#ddlLPOs').removeAttr("disabled");
                }
                else {
                    if (LPOID == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Local PO Id is required.');
                        $("#ddlLPOs0").focus();
                    }
                    else if (LPODT == '') {
                        ErrorMessage('Local PO Date is required.');
                        $("#txtLPODate0").focus();
                    }
                    else if (parseFloat(LPOAMT) <= 0) {
                        ErrorMessage('Local PO Amount is required.');
                        $("#txtLPOAMT0").focus();
                    }
                    else if (PrfInvNo == '') {
                        ErrorMessage('ProformaInvoice is required.');
                        $("#txtPrfInvNo0").focus();
                    }
                    //                    else if (TaxInvoice == '') {
                    //                        ErrorMessage('TaxInvoice is required.');
                    //                        $("#txtTaxInvoice0").focus();
                    //                    }
                    else if (CrntDT == '') {
                        ErrorMessage('date is required.');
                        $("#txtCrntDate0").focus();
                    }
                    else if (parseFloat(Amt) == 'NaN' || parseFloat(Amt) <= 0) {
                        ErrorMessage('Amount is required.');
                        $("#txtAmt0").focus();
                    }
                }
            }

            function UpdateRow() {
                var LPOID = $("#ddlLPOs0").val();
                var LPONo = $("#ddlLPOs0 :selected").text();
                var LPODT = $('#txtLPODate0').val();
                var LPOAMT = ($('#txtLPOAMT0').val() == '' || $('#txtLPOAMT0').val() == '.') ? 0 : $('#txtLPOAMT0').val();
                var PrfINVID = 0; // $('#HFPrfInvID0').val();
                var PrfInvNo = $('#txtPrfInvNo0').val();
                var PrfInvAmt = $('#txtPrfInvAmt0').val();
                var TaxInvoice = ""; // $('#txtTaxInvoice0').val();
                var CrntDT = $('#txtCrntDate0').val();
                var Amt = ($('#txtAmt0').val() == '' || $('#txtAmt0').val() == '.') ? 0 : $('#txtAmt0').val();
                var Remarks = $('#txtremarks0').val();
                var RowCount = $('#lblEditID0').text();

                if (LPOID != 0 && LPODT != '' && parseFloat(LPOAMT) > 0 && PrfInvNo != '' && CrntDT != '' && parseFloat(Amt) > 0) {
                    var result = BillPaymentApproval.UpdateRow(RowCount, LPOID, LPONo, LPODT, LPOAMT, PrfINVID, PrfInvNo, PrfInvAmt, TaxInvoice, CrntDT, Amt, Remarks);
                    $('#tblLPOs tbody').html('');
                    //$('#tblLPOs').html(result.value);
                    $('#tblLPOs tbody').append(result.value);
                    $('#lblEditID0').text('');
                    $("#ddlLPOs0").val('0');
                    $('#txtLPODate0').val('');
                    $('#txtLPOAMT0').val('');
                    $('#HFPrfInvID0').val('');
                    $('#txtPrfInvNo0').val('');
                    $('#txtPrfInvAmt0').val('');
                    //$('#txtTaxInvoice0').val('');
                    $('#txtCrntDate0').val('');
                    $('#txtAmt0').val('');
                    $('#txtremarks0').val('');
                    $("#ddlLPOs0").focus();

                    $('#btnADD').show();
                    //$('#btnCancel').hide();
                    $('#btnUpdate').hide();
                    $('[id$=HFLPOsHistorySum]').val('');
                    $("#ddlLPOs").removeAttr("disabled");
                    RemoveLPOid(LPOID, true);
                    var selectedLPOID = $('[id$=HFSelectedLPOID]').val();
                    if (LPOID != selectedLPOID) {

                        var INVs = $('[id$=HFINV_LPOIDs]').val().split(',');
                        var LPOs = $('[id$=HFPO_LPOIDs]').val();
                        if ($.inArray(selectedLPOID, INVs) != -1) {
                            INVs.splice($.inArray(selectedLPOID, INVs), 1);
                            INVs.push(LPOID);
                            $('[id$=HFINV_LPOIDs]').val(INVs.join(","));
                            GetHistory(LPOID, LPOs, false, true);
                        }
                    }
                    GetBal(false);
                }
                else {
                    if (LPOID == 0) {
                        ErrorMessage('Local PO Id is required.');
                        $("#ddlLPOs0").focus();
                    }
                    else if (LPODT == '') {
                        ErrorMessage('Local PO Date is required.');
                        $("#txtLPODate0").focus();
                    }
                    else if (parseFloat(LPOAMT) <= 0) {
                        ErrorMessage('Local PO Amount is required.');
                        $("#txtLPOAMT0").focus();
                    }
                    else if (PrfInvNo = '') {
                        ErrorMessage('ProformaInvoice is required.');
                        $("#txtPrfInvNo0").focus();
                    }
                    //                    else if (TaxInvoice = '') {
                    //                        ErrorMessage('TaxInvoice is required.');
                    //                        $("#txtTaxInvoice0").focus();
                    //                    }
                    else if (CrntDT == '') {
                        ErrorMessage('ProformaInvoice date is required.');
                        $("#txtLPODate0").focus();
                    }
                    else if (parseFloat(Amt) <= 0 || parseFloat(Amt) == NaN) {
                        ErrorMessage('Amount is required.');
                        $("#txtAmt0").focus();
                    }
                }
            }

            function GetProformaINV(NoYes) {
                var INV_LPOID = $("#ddlLPOs0").val();
                var result = BillPaymentApproval.GetProformaINV(INV_LPOID);
                var str = result.value.split('@,@');
                //$('#HFPrfInvID0').val(str[0]);
                //$('#txtPrfInvNo0').val(str[1]);
                //$('#txtCrntDate0').val(str[2]);   
                if ($("#ddlLPOs0").val() != '00000000-0000-0000-0000-000000000000') {
                    $('#ddlLPOs').attr("disabled", "disabled");
                }
                else {
                    $('#ddlLPOs').removeAttr("disabled");
                }
                if (NoYes) {
                    $('#txtAmt0').val('');
                    $('#txtLPODate0').val(str[0]);
                    $('#txtLPOAMT0').val(str[1]);
                    $('[id$=txtProInvNoDate]').val(str[3]);
                    $('[id$=txtProInfNo]').val(str[4]);
                }
                $('[id$=HFLPOsHistorySum]').val(str[2]);
                if (NoYes)
                    GetHistory(INV_LPOID, '', false, false);
                //SumOfLPOid(INV_LPOID);
            }

            $('#tblLPOs tbody tr').live('dblclick', function (e) {
                e.preventDefault();
                var SelectedRowIndex = this.sectionRowIndex;
                var LPOID = $('#HFLPOID' + Number(SelectedRowIndex + 1)).val();
                var PrfInvID = $('#HFPRFMID' + Number(SelectedRowIndex + 1)).val();
                if ($(this).hasClass('SelectedRow')) {
                    $(this).removeClass('SelectedRow');
                    RemoveLPOid(LPOID, true);
                    $('#lblEditID0').text('');
                    $("#ddlLPOs0").val('0');
                    $('#txtLPODate0').val('');
                    $('#txtLPOAMT0').val('');
                    $('#HFPrfInvID0').val('');
                    $('#txtPrfInvNo0').val('');
                    $('#txtPrfInvAmt0').val('');
                    //$('#txtTaxInvoice0').val('');
                    $('#txtCrntDate0').val('');
                    $('#txtAmt0').val('');
                    $('#lblEditID0').val('');
                    $('#txtremarks0').val('');
                    $('[id$=HFSelectedLPOID]').val('');
                    $('#btnADD').show();
                    //$('#btnCancel').hide();
                    $('#btnUpdate').hide();
                    $("#ddlLPOs").removeAttr("disabled");
                }
                else {
                    $('#tblLPOs tbody tr').removeClass("SelectedRow");
                    $(this).addClass('SelectedRow');
                    var tableData = $(this).children("td").map(function () {
                        return $(this).text();
                    }).get();

                    //alert("LPOID : " + LPOID + ", Selected Row Index" + SelectedRowIndex);
                    //alert("LPOID : " + LPOID + ", RowIndex : " + SelectedRowIndex + " , " + $.trim(tableData[0]) + " , " + $.trim(tableData[1]) + " , " + $.trim(tableData[2]));
                    $('[id$=HFSelectedLPOID]').val(LPOID);
                    var select = document.getElementById("ddlLPOs0");
                    var option = document.createElement('option');
                    option.text = $.trim(tableData[1]);
                    option.value = LPOID;
                    if ($("#ddlLPOs0 option[value=" + LPOID.trim() + "]").length == 0) {
                        select.add(option, 1);
                    }

                    var select1 = document.getElementById("ddlLPOs");
                    var option1 = document.createElement('option');
                    option1.text = $.trim(tableData[1]);
                    option1.value = LPOID;
                    if ($("#ddlLPOs option[value=" + LPOID.trim() + "]").length == 0) {
                        select1.add(option1, 1);
                        $("#ddlLPOs").attr("disabled", "disabled");
                    }

                    $('#lblEditID0').text($.trim(tableData[0]));
                    $("#ddlLPOs0").val(LPOID.trim());
                    $('#txtLPODate0').val($.trim(tableData[2]));
                    $('#txtLPOAMT0').val($.trim(tableData[3]));
                    $('#HFPrfInvID0').val(PrfInvID);
                    $('#txtPrfInvNo0').val($.trim(tableData[4]));
                    $('#txtPrfInvAmt0').val($.trim(tableData[5]));
                    //$('#txtTaxInvoice0').val($.trim(tableData[5]));
                    $('#txtCrntDate0').val($.trim(tableData[6]));
                    $('#txtAmt0').val($.trim(tableData[7])); // $('#txtAmt0').val(''); 
                    $('#txtremarks0').val($.trim(tableData[8]));
                    GetProformaINV(false);

                    $('#btnADD').hide();
                    $('#btnCancel').show();
                    $('#btnUpdate').show();
                }
            });

            function CancelEdit() {
                var SelectedLPOID = $("#ddlLPOs0").val();
                var editID = $('[id$=HFSelectedLPOID]').val();
                if ($('#lblEditID0').text() != '') {
                    if (editID == SelectedLPOID) {
                        RemoveLPOid(SelectedLPOID, true);
                        $('#lblEditID0').text('');
                    }
                    else {
                        RemoveLPOid(editID, true);
                        $('#lblEditID0').text('');
                    }
                }
                else
                    $('#lblEditID0').text('');
                $("#ddlLPOs0").val('0');
                $('#txtLPODate0').val('');
                $('#txtLPOAMT0').val('');
                $('#HFPrfInvID0').val('');
                $('#txtPrfInvNo0').val('');
                $('#txtPrfInvAmt0').val('');
                //$('#txtTaxInvoice0').val('');
                $('#txtCrntDate0').val('');
                $('#txtAmt0').val('');
                $('#txtremarks0').val('');
                $('#lblEditID0').val('');
                $('#tblLPOs tbody tr').removeClass("SelectedRow");
                $('#btnADD').show();
                //$('#btnCancel').hide();
                $('#btnUpdate').hide();
                $("#ddlLPOs").removeAttr("disabled");
                GetHistory('', '', false, false);
            }

            function DeleteOneRow(RowID) {
                if (confirm("Are you sure you want to Delete?")) {
                    $("#ddlLPOs").removeAttr("disabled");
                    var LPOId = $('#HFLPOID' + RowID).val();
                    var LPONo = $('#tdd' + RowID).text().trim();
                    var HFINV_LPOIDs = $('[id$=HFINV_LPOIDs]').val().split(',');
                    if ($.inArray(LPOId, HFINV_LPOIDs) != -1) {
                        HFINV_LPOIDs.splice($.inArray(LPOId, HFINV_LPOIDs), 1);
                        $('[id$=HFINV_LPOIDs]').val(HFINV_LPOIDs);
                        var POIDs = $('[id$=HFPO_LPOIDs]').val();

                        var result = BillPaymentApproval.DeleteOneRow(RowID, HFINV_LPOIDs, POIDs);
                        $('#tblLPOs').html(result.value);
                        GetHistory(HFINV_LPOIDs, POIDs, true, false);
                        GetBal(false);
                        AddItemDDL(LPOId, LPONo);
                        getdate();
                    }
                }
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
                    //                minDate: dateBack,
                    //                maxDate: dateToday,
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
                    //                minDate: dateBack,
                    //                maxDate: dateToday,
                    dateFormat: 'dd-mm-yy',
                    changeMonth: true,
                    changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
                });
            }

            $("#txtAmt").on('keyup', function () {
                CheckLPOAmount_PO();
            });

            function CheckLPOAmount_PO() {
                var Amt = $("#txtAmt").val();
                var LPOAmt = $("#txtLPOAMT_GST").val();
                var LPOHist = $('[id$=HFAllLPOHistory]').val() == "" ? 0 : $('[id$=HFAllLPOHistory]').val();

                var PaidAmt = 0;
                var FinaAmt = parseFloat(LPOAmt) - parseFloat(LPOHist);
                //alert("PAMt "+PaidAmt);
                //if (result.value != "") {
                var Condition = ((parseFloat(LPOAmt) * 5) / 100);
                var MinPay = parseFloat(FinaAmt) - parseFloat(Condition);
                var MaxPay = parseFloat(FinaAmt) + parseFloat(Condition);
                //alert("Name : " +Condition+" Location : " +Condition);
                //if (parseFloat(result.value) < 0) {
                //                if ((parseFloat(LPOHist) + parseFloat(Amt)) < parseFloat(MinPay)) {
                //                    ErrorMessage('Amount entered is 5% Less than Sum of History LPO Amount.');
                //                    //if (!confirm('Amount entered is Upto 5% Less than LPO Amount. Do you want to continue ?'))
                //                    $("#txtAmt").val('0');
                //                }
                //                else 
                if ((parseFloat(FinaAmt)) > parseFloat(MaxPay)) {
                    ErrorMessage('Amount entered is 5% Greater than Sum of history LPO Amount.');
                    $("#txtAmt").val('0');
                }
                //}

                //                if ((parseFloat(LPOHist) + parseFloat(Amt)) > parseFloat(LPOAmt)) {
                //                    //$("#txtAmt").val('');
                //                    ErrorMessage('Amount Entered cannot be greater than sum of history paid + Amt (' + Amt + ' + ' + LPOHist + ') > ' + LPOAmt + '.');
                //                }
            }

            function AddNewRow1() {
                var LPOID = $("#ddlLPOs").val();
                var LPONo = $("#ddlLPOs :selected").text();
                var LPODT = $('#txtLPODate').val();
                var LPOAMT = ($('#txtLPOAMT').val() == '' || $('#txtLPOAMT').val() == '.') ? 0 : $('#txtLPOAMT').val();
                var LPOAMT_GST = ($('#txtLPOAMT_GST').val() == '' || $('#txtLPOAMT_GST').val() == '.') ? 0 : $('#txtLPOAMT_GST').val();
                var PrfmaInvNo = ($('#txtPrfInvNo1').val());
                var AmtPrcnt = $('#txtAmtPrcnt').val();
                var CrntDT = $('#txtCrntDate').val();
                var Amt = ($('#txtAmt').val() == '' || $('#txtAmt').val() == '.') ? 0 : $('#txtAmt').val();
                var Remarks = $('#txtremarks').val();
                var RowCount = (Number($('#divGridTwo tbody tr').length));

                if (LPOID != 0 && LPODT != '' && parseFloat(LPOAMT) > 0 && CrntDT != '' && parseFloat(Amt) > 0 && Number(AmtPrcnt) > 0) {
                    var result = BillPaymentApproval.AddNewRow1(RowCount, LPOID, LPONo, LPODT, LPOAMT, LPOAMT_GST, PrfmaInvNo, AmtPrcnt, CrntDT, Amt, Remarks);
                    $('#tblLPOs1 tbody').html('');
                    $('#tblLPOs1 tbody').append(result.value);

                    $("#ddlLPOs").val('0');
                    $('#txtLPODate').val('');
                    $('#txtLPOAMT').val('');
                    $('#txtLPOAMT_GST').val('');
                    $('#txtPrfInvNo1').val('');
                    $('#txtAmtPrcnt').val('');
                    $('#txtCrntDate').val('');
                    $('#txtAmt').val('');
                    $('#txtremarks').val('');
                    $("#ddlLPOs").focus();

                    var AllLPOIDs1 = $('[id$=HFPO_LPOIDs]').val();
                    if (AllLPOIDs1 == "")
                        AllLPOIDs1 = LPOID;
                    else
                        AllLPOIDs1 = AllLPOIDs1 + ',' + LPOID;
                    $('[id$=HFPO_LPOIDs]').val(AllLPOIDs1);
                    $('[id$=HFLPOsHistorySum]').val('');
                    GetHistory('', '', false, false);
                    RemoveLPOid(LPOID, false);
                    GetBal(false);
                    $('#ddlLPOs0').removeAttr("disabled");
                }
                else {
                    if (LPOID == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Local PO Id is required.');
                        $("#ddlLPOs").focus();
                    }
                    else if (LPODT == '') {
                        ErrorMessage('Local PO Date is required.');
                        $("#txtLPODate").focus();
                    }
                    else if (parseFloat(LPOAMT) <= 0) {
                        ErrorMessage('Local PO Amount is required.');
                        $("#txtLPOAMT").focus();
                    }
                    else if (AmtPrcnt == '') {
                        ErrorMessage('Amount percent is required.');
                        $("#txtAmtPrcnt").focus();
                    }
                    else if (CrntDT == '') {
                        ErrorMessage('date is required.');
                        $("#txtCrntDate").focus();
                    }
                    else if (parseFloat(Amt) <= 0 || parseFloat(Amt) == NaN) {
                        ErrorMessage('Amount is required.');
                        $("#txtAmt").focus();
                    }
                }
            }

            function UpdateRow1() {
                var LPOID = $("#ddlLPOs").val();
                var LPONo = $("#ddlLPOs :selected").text();
                var LPODT = $('#txtLPODate').val();
                var LPOAMT = ($('#txtLPOAMT').val() == '' || $('#txtLPOAMT').val() == '.') ? 0 : $('#txtLPOAMT').val();
                var LPOAMT_GST = ($('#txtLPOAMT_GST').val() == '' || $('#txtLPOAMT_GST').val() == '.') ? 0 : $('#txtLPOAMT_GST').val();
                //var PrfINVID = $('#HFPrfInvID').val();
                var PrfINVID = $('#txtPrfInvNo1').val();
                var AmtPrcnt = $('#txtAmtPrcnt').val();
                var CrntDT = $('#txtCrntDate').val();
                var Amt = ($('#txtAmt').val() == '' || $('#txtAmt').val() == '.') ? 0 : $('#txtAmt').val();
                var Remarks = $('#txtremarks').val();
                var RowCount = $('#lblEditID').text();

                if (LPOID != 0 && LPODT != '' && parseFloat(LPOAMT) > 0 && Number(AmtPrcnt) > 0 && CrntDT != '' && parseFloat(Amt) > 0) {
                    var result = BillPaymentApproval.UpdateRow1(RowCount, LPOID, LPONo, LPODT, LPOAMT, LPOAMT_GST, PrfINVID, AmtPrcnt, CrntDT, Amt, Remarks);
                    $('#tblLPOs1 tbody').html('');
                    //$('#tblLPOs').html(result.value);
                    $('#tblLPOs1 tbody').append(result.value);
                    $('#lblEditID').text('');
                    $("#ddlLPOs").val('0');
                    $('#txtLPODate').val('');
                    $('#txtLPOAMT').val('');
                    $('#txtLPOAMT_GST').val('');
                    //$('#HFPrfInvID').val('');
                    $('#txtPrfInvNo1').val('');
                    $('#txtAmtPrcnt').val('');
                    $('#txtCrntDate').val('');
                    $('#txtAmt').val('');
                    $('#txtremarks').val('');
                    $('#btnADD1').show();
                    //$('#btnCancel1').hide();
                    $('#btnUpdate1').hide();

                    $("#ddlLPOs").focus();
                    $('[id$=HFLPOsHistorySum]').val('');
                    $("#ddlLPOs0").removeAttr("disabled");
                    RemoveLPOid(LPOID, false);
                    var selectedLPOID = $('[id$=HFSelectedLPOID]').val();
                    if (LPOID != selectedLPOID) {

                        var INVs = $('[id$=HFINV_LPOIDs]').val();
                        var LPOs = $('[id$=HFPO_LPOIDs]').val().split(',');
                        if ($.inArray(selectedLPOID, LPOs) != -1) {
                            LPOs.splice($.inArray(selectedLPOID, LPOs), 1);
                            LPOs.push(LPOID);
                            $('[id$=HFPO_LPOIDs]').val(LPOs.join(","));
                            GetHistory(INVs, LPOID, false, true);
                        }
                    }
                    $('[id$=HFSelectedLPOID]').val('');
                    GetBal(false);
                }
                else {
                    if (LPOID == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Local PO Id is required.');
                        $("#ddlLPOs").focus();
                    }
                    else if (LPODT == '') {
                        ErrorMessage('Local PO Date is required.');
                        $("#txtLPODate").focus();
                    }
                    else if (parseFloat(LPOAMT) <= 0) {
                        ErrorMessage('Local PO Amount is required.');
                        $("#txtLPOAMT").focus();
                    }
                    else if (AmtPrcnt = '') {
                        ErrorMessage('Amount percent is required.');
                        $("#txtAmtPrcnt").focus();
                    }
                    else if (CrntDT == '') {
                        ErrorMessage('date is required.');
                        $("#txtCrntDate").focus();
                    }
                    else if (parseFloat(Amt) <= 0 || parseFloat(Amt) == NaN) {
                        ErrorMessage('Amount is required.');
                        $("#txtAmt").focus();
                    }
                }
            }

            function GetProformaINV1(NoYes) {
                var PO_LPOID = $("#ddlLPOs").val();
                var result = BillPaymentApproval.GetProformaINV1(PO_LPOID);
                var str = result.value.split('@,@');
                //$('#HFPrfInvID').val(str[0]);
                //$('#txtAmtPrcnt').val(str[1]);
                //$('#txtCrntDate').val(str[2]);ddlLPOs
                if ($("#ddlLPOs").val() != '00000000-0000-0000-0000-000000000000') {
                    $('#ddlLPOs0').attr("disabled", "disabled");
                }
                else {
                    $('#ddlLPOs0').removeAttr("disabled");
                }

                if (NoYes) {
                    if ($('[id$=HFSelectedLPOID]').val() == "")
                        $('#lblEditID').text('');
                    $('#txtLPODate').val('');
                    $('#txtLPOAMT').val('');
                    $('#txtLPOAMT_GST').val('');
                    $('#HFPrfInvID').val('');
                    $('#txtAmtPrcnt').val('');
                    $('#txtCrntDate').val('');
                    $('#txtAmt').val('');
                    $('#lblEditID').val('');
                    $('#txtremarks').val('');

                    $('#txtLPODate').val(str[0]);
                    $('#txtLPOAMT').val(str[1]);
                    $('#txtLPOAMT_GST').val(str[3]);
                }
                $('[id$=HFLPOsHistorySum]').val(str[2]);
                if (NoYes)
                    GetHistory('', PO_LPOID, false, false);
            }

            $('#tblLPOs1 tbody tr').live('dblclick', function (e) {
                e.preventDefault();
                var SelectedRowIndex = this.sectionRowIndex;
                var LPOID = $('#HFFLPOID' + Number(SelectedRowIndex + 1)).val();
                if ($(this).hasClass('SelectedRow')) {
                    $(this).removeClass('SelectedRow');
                    RemoveLPOid(LPOID, false);
                    $('#lblEditID').text('');
                    $("#ddlLPOs").val('0');
                    $('#txtLPODate').val('');
                    $('#txtLPOAMT').val('');
                    $('#txtLPOAMT_GST').val('');
                    $('#HFPrfInvID').val('');
                    $('#txtAmtPrcnt').val('');
                    $('#txtCrntDate').val('');
                    $('#txtAmt').val('');
                    $('#lblEditID').val('');
                    $('#txtremarks').val('');
                    $('#btnADD1').show();
                    //$('#btnCancel1').hide();
                    $('#btnUpdate1').hide();
                    $('[id$=HFSelectedLPOID]').val('');
                    $("#ddlLPOs0").removeAttr("disabled");
                }
                else {
                    $('#tblLPOs1 tbody tr').removeClass("SelectedRow");
                    $(this).addClass('SelectedRow');
                    var tableData = $(this).children("td").map(function () {
                        return $(this).text();
                    }).get();
                    //var PrfInvID = $('#HFPRFMID' + Number(SelectedRowIndex + 1)).val();
                    //alert("LPOID : " + LPOID + ", Selected Row Index" + SelectedRowIndex);
                    //alert("LPOID : " + LPOID + ", RowIndex : " + SelectedRowIndex + " , " + $.trim(tableData[0]) + " , " + $.trim(tableData[1]) + " , " + $.trim(tableData[2]));
                    $('[id$=HFSelectedLPOID]').val(LPOID);
                    var select = document.getElementById("ddlLPOs0");
                    var option = document.createElement('option');
                    option.text = $.trim(tableData[1]);
                    option.value = LPOID;
                    if ($("#ddlLPOs0 option[value=" + LPOID.trim() + "]").length == 0) {
                        select.add(option, 1);
                        $("#ddlLPOs0").attr("disabled", "disabled");
                    }

                    var select1 = document.getElementById("ddlLPOs");
                    var option1 = document.createElement('option');
                    option1.text = $.trim(tableData[1]);
                    option1.value = LPOID;
                    if ($("#ddlLPOs option[value=" + LPOID.trim() + "]").length == 0) {
                        select1.add(option1, 1);
                    }

                    $('#lblEditID').text($.trim(tableData[0]));
                    $("#ddlLPOs").val(LPOID.trim());
                    $('#txtLPODate').val($.trim(tableData[2]));
                    $('#txtLPOAMT').val($.trim(tableData[3]));
                    $('#txtLPOAMT_GST').val($.trim(tableData[4]));
                    $('#txtPrfInvNo1').val($.trim(tableData[5]));
                    $('#txtAmtPrcnt').val($.trim(tableData[6])); //$('#txtAmtPrcnt').val('');
                    $('#txtCrntDate').val($.trim(tableData[7]));
                    $('#txtAmt').val($.trim(tableData[8])); // $('#txtAmt').val('');
                    $('#txtremarks').val($.trim(tableData[9]));
                    GetProformaINV1(false);
                    $('#btnADD1').hide();
                    $('#btnCancel1').show();
                    $('#btnUpdate1').show();
                }
            });

            function CancelEdit1() {
                var SelectedLPOID = $("#ddlLPOs").val();
                var EditID = $('[id$=HFSelectedLPOID]').val();
                if ($('#lblEditID').text() != '') {
                    if (EditID == SelectedLPOID)
                        RemoveLPOid(SelectedLPOID, false);
                    else
                        RemoveLPOid(EditID, false);
                    $('#lblEditID').text('');
                }
                else
                    $('#lblEditID').text('');
                $("#ddlLPOs").val('0');
                $('#txtLPODate').val('');
                $('#txtLPOAMT').val('');
                $('#txtLPOAMT_GST').val('');
                $('#txtPrfInvNo1').val('');
                $('#HFPrfInvID').val('');
                $('#txtAmtPrcnt').val('');
                $('#txtCrntDate').val('');
                $('#txtAmt').val('');
                $('#txtremarks').val('');
                $('#lblEditID1').val('');
                $('#tblLPOs1 tbody tr').removeClass("SelectedRow");
                $('#btnADD1').show();
                //$('#btnCancel1').hide();
                $('#btnUpdate1').hide();
                $("#ddlLPOs0").removeAttr("disabled");
                GetHistory('', '', false, false);
            }

            function DeleteTwoRow(RowID) {
                if (confirm("Are you sure you want to Delete?")) {
                    $("#ddlLPOs0").removeAttr("disabled");
                    var LPOId = $('#HFFLPOID' + RowID).val();
                    var LPOno = $('#tddPO' + RowID).text();
                    var HFPO_LPOIDs = $('[id$=HFPO_LPOIDs]').val().split(',');
                    if ($.inArray(LPOId, HFPO_LPOIDs) != -1) {
                        HFPO_LPOIDs.splice($.inArray(LPOId, HFPO_LPOIDs), 1);
                        $('[id$=HFPO_LPOIDs]').val(HFPO_LPOIDs);
                        var INVs = $('[id$=HFINV_LPOIDs]').val();

                        var result = BillPaymentApproval.DeleteTwoRow(RowID, INVs, HFPO_LPOIDs);
                        $('#tblLPOs1').html(result.value);

                        GetHistory(INVs, HFPO_LPOIDs, true, false);
                        AddItemDDL(LPOId, LPOno);
                        GetBal(false);
                        SetDate_Grid();
                    }
                }
            }

            function GetHistory(INV, PO, IsDelete, IsAdded) {
                var HFINV_LPOIDs = $('[id$=HFINV_LPOIDs]').val();
                var HFPO_LPOIDs = $('[id$=HFPO_LPOIDs]').val();
                if (INV != '' && IsDelete == false)
                    HFINV_LPOIDs = HFINV_LPOIDs + ',' + INV;
                if (PO != '' && IsDelete == false)
                    HFPO_LPOIDs = HFPO_LPOIDs + ',' + PO;
                var result = BillPaymentApproval.GetHistory(HFINV_LPOIDs, HFPO_LPOIDs);
                $('[id$=DivHistory]').html(result.value);
                //                if (!IsDelete)
                //                    GetHistoryCheques(INV, PO, IsDelete, IsAdded);
                //                else
                GetHistoryCheques(HFINV_LPOIDs, HFPO_LPOIDs, true, IsAdded);

                //GetBal(false);
            }

            function GetHistoryCheques(INV, PO, IsDelete, IsAdded) {
                var result = BillPaymentApproval.GetHistoryCheques(INV, PO, IsDelete, IsAdded);
                $('[id$=DivHistoryCheques]').html(result.value);
            }

            function GetAmount() {
                var Prcnt = $("#txtAmtPrcnt").val();
                if (Prcnt != '') {
                    var LPOAMT = $("#txtLPOAMT_GST").val(); //Without GST Amount for calculation
                    var Amount = (parseFloat(Prcnt) * parseFloat(LPOAMT)) / 100;
                    $("#txtAmt").val(Amount.toFixed(2));
                    CheckLPOAmount_PO();
                }
                else
                    $("#txtAmt").val('');
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

        function Myvalidations() {
            var INVsRCount = $('#tblLPOs tbody tr').length;
            var POsRCount = $('#tblLPOs1 tbody tr').length;

            if (($('[id$=ddlSupplier]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier is Required.');
                $('[id$=ddlSupplier]').focus();
                return false;
            }
            else if (($('[id$=txtRefNo]').val()).trim() == '') {
                ErrorMessage('Reference No is Required.');
                $('[id$=txtRefNo]').focus();
                return false;
            }
            else if (INVsRCount == 0 && POsRCount == 0) {
                ErrorMessage('No rows to save in INVOICE / POs tables.');
                return false;
            }
            //            else if (POsRCount == 0) {
            //                ErrorMessage('No rows to save in POs table.');
            //                return false;
            //            }
            //            else if (($('[id$=txtProInfNo]').val()).trim() == '') {
            //                ErrorMessage('Pro.Inv.No. is Required.');
            //                $('[id$=txtProInfNo]').focus();
            //                return false;
            //            }
            //            else if (($('[id$=txtProInfNoDate]').val()).trim() == '') {
            //                ErrorMessage('Pro. Inv. Date is Required.');
            //                $('[id$=txtProInfNoDate]').focus();
            //                return false;
            //            }
            //            else if (($('[id$=txtExportInvNo]').val()).trim() == '') {
            //                ErrorMessage('Export Inv.No. is Required.');
            //                $('[id$=txtExportInvNo]').focus();
            //                return false;
            //            }
            //            else if (($('[id$=txtExportInvNoDate]').val()).trim() == '') {
            //                ErrorMessage('Export Inv. Date is Required.');
            //                $('[id$=txtExportInvNoDate]').focus();
            //                return false;
            //            }
            //            else if (($('[id$=txtFormCorHNo]').val()).trim() == '') {
            //                ErrorMessage('Form &#39C&#39 or &#39H&#39 No. is Required.');
            //                $('[id$=txtFormCorHNo]').focus();
            //                return false;
            //            }
            //            else if (($('[id$=txtFormCorHNoDate]').val()).trim() == '') {
            //                ErrorMessage('Form &#39C&#39 or &#39H&#39 Date is Required.');
            //                $('[id$=txtFormCorHNoDate]').focus();
            //                return false;
            //            }

        }

        function MyRejectvalidations() {

            if (($('[id$=txtRejectReasons]').val()).trim() == '') {
                ErrorMessage('Reject Reason(s) is Required.');
                $('[id$=txtRejectReasons]').focus();
                return false;
            }
            else
                return true;
        }


        $(document).ready(function () {
            $('[id$=txtAmount]').on('keyup', function () {
                GetBal(true);
            });
        });

        function GetBal(IsKeyPress) {
            var chequePayAmt = 0; // ($('[id$=txtAmount]').length == 0 || $('[id$=txtAmount]').val().trim() == '') ? "0" : $('[id$=txtAmount]').val().trim();

            var ChequeAmt = 0; //$('#HFChequeAmount').val() == undefined ? "0" : $('#HFChequeAmount').val();
            var INVPayAmt = $('#HFINVAmt0').val() == undefined ? "0" : $('#HFINVAmt0').val();
            var POPayAmt = $('#hfPOamt').val() == undefined ? "0" : $('#hfPOamt').val();
            var INVLPOAmt = $('#HFLPOamt0').val() == undefined ? "0" : $('#HFLPOamt0').val();
            var POLPOAmt = $('#HFPOLPOamt').val() == undefined ? "0" : $('#HFPOLPOamt').val();

            var TotalPayAmt = 0;
            if (parseFloat(chequePayAmt) > 0)
                TotalPayAmt = (parseFloat(chequePayAmt) + parseFloat(ChequeAmt));
            else
                TotalPayAmt = (parseFloat(ChequeAmt) + parseFloat(INVPayAmt) + parseFloat(POPayAmt));
            var TotalLPOsAmt = (parseFloat(INVLPOAmt) + parseFloat(POLPOAmt));
            TotalPayAmt = parseFloat(TotalPayAmt) + parseFloat($('#HFAllLPOHistory').val() == undefined ? "0" : $('#HFAllLPOHistory').val());
            var balAmt = parseFloat(TotalLPOsAmt) - parseFloat(TotalPayAmt);
            if (!IsKeyPress)
                $('[id$=txtAmount]').val((parseFloat(INVPayAmt) + parseFloat(POPayAmt)).toFixed(2));

            $('[id$=lblBalance]').text(balAmt.toFixed(2));

            //            if (TotalPayAmt <= TotalLPOsAmt)
            //                $('[id$=lblBalance]').text(balAmt.toFixed(2));
            //            else if (IsKeyPress) {
            //                $('[id$=txtAmount]').val('');
            //                $('[id$=lblBalance]').text('');
            //                ErrorMessage('Amount cannot be greater than sum of Total LPOs amount : ' + TotalLPOsAmt);
            //            }

        }

        $('[id$=txtAmount]').focus(function () {
            GetBal(false);
            var INVPayAmt = $('#HFINVAmt0').val() == undefined ? "0" : $('#HFINVAmt0').val();
            var POPayAmt = $('#hfPOamt').val() == undefined ? "0" : $('#hfPOamt').val();
            var Tot = parseFloat(INVPayAmt) + parseFloat(POPayAmt);
            $('[id$=txtAmount]').val(Tot.toFixed(2));
        });

        function RemoveLPOid(LPOid, IsINV) {
            if (!IsINV)
                $('#ddlLPOs0> option[value=' + LPOid + ']').remove();
            else
                $('#ddlLPOs> option[value=' + LPOid + ']').remove();
        }

        function AddItemDDL(LPOid, LPOno) {
            var select = document.getElementById("ddlLPOs0");
            var option = document.createElement('option');
            option.text = LPOno;
            option.value = LPOid;
            if ($("#ddlLPOs0 option[value=" + LPOid.trim() + "]").length == 0) {
                select.add(option, 1);
            }

            var select1 = document.getElementById("ddlLPOs");
            var option1 = document.createElement('option');
            option1.text = LPOno;
            option1.value = LPOid;
            if ($("#ddlLPOs option[value=" + LPOid.trim() + "]").length == 0) {
                select1.add(option1, 1);
            }
        }

        function CheckChequeNo() {
            var chequNo = $('[id$=txtChequeNo]').val();
            var result = BillPaymentApproval.CheckChequeNo(chequNo);
            if (result.value == "True") {
                $('[id$=txtChequeNo]').val('');
                $('[id$=txtChequeNo]').focus();
                ErrorMessage('This Cheque No. is in use.');
            }
        }

    </script>
</asp:Content>
