<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true" CodeBehind="Purchase_Indent.aspx.cs" Inherits="VOMS_ERP.Customer_Access.Purchase_Indent" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Purchase Indent"
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
                            <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabelright">If Repeated FPO :</span>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CHkShow" runat="server" TabIndex="2" AutoPostBack="true" OnCheckedChanged="CHkShow_CheckedChanged" /><%--onchange="ShowPreviousFE();"--%>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabelright">Ref. FPO No.:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlRefForeignEnqNo" CssClass="bcAspdropdown"
                                            Enabled="false" AutoPostBack="True" TabIndex="4" OnSelectedIndexChanged="ddlRefForeignEnqNo_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblCustName" class="bcLabelright">Name of Customer<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlcustmr" CssClass="bcAspdropdown" onchange="ClrInputs();">
                                        </asp:DropDownList>
                                        <asp:Label runat="server" ID="lblCustomerNm" class="bcLabel" Text="" Visible="false"></asp:Label>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblDept" class="bcLabelright">Project/Department Name<font color="red"
                                            size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddldept" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Department"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Foundry"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblSubject" class="bcLabelright">Subject<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsubject" 
                                            ValidationGroup="D" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Sporadic"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Regular"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Urgent"></asp:ListItem>
                                            </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblEnquiry" class="bcLabelright">Purchase Indent Number<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtenqno" ValidationGroup="D" CssClass="bcAsptextbox"
                                            onkeypress="return isSomeSplChar(event)" MaxLength="350" onchange="javascript:return CheckEnquiryNo();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblEnquirydt" class="bcLabelright">Purchase Indent Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtenqdt" ValidationGroup="D" onchange="changedateEnqDT();"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="display:none">
                                        <span id="lblRecvDt" class="bcLabelright">
                                            <asp:Label ID="lblRcvdDt" class="bcLabel" runat="server" Text="Received Date"></asp:Label><font
                                                color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="display:none">
                                        <asp:TextBox runat="server" ID="txtrecvdt"  onchange="changedate();" ValidationGroup="D"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display:none">
                                        <span id="lblDuedt" class="bcLabelright">
                                            <asp:Label ID="lblDTdue" class="bcLabel" runat="server" Text="Due Date"></asp:Label><font
                                                color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="display:none">
                                        <asp:TextBox runat="server" ID="txtduedt" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblIns" class="bcLabelright">Purpose:</span>
                                    </td>
                                    <td class="bcTdnormal" colspan="3">
                                        <asp:TextBox runat="server" ID="txtimpinst" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine" Style="width: 570px;"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="bcTdnormal">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="49%">
                                                        <span id="Span1" class="bcLabelright">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        &nbsp;&nbsp;&nbsp;Add Items
                                    </td>
                                </tr>
                                <asp:Panel ID="PnlImp" runat="server" Width="98%">
                                    <tr>
                                        <td>
                                            Upload Items From Excel:
                                        </td>
                                        <td colspan="2">
                                            <asp:FileUpload ID="FileUpload1" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnBulkUpload" runat="server" Text="Upload" OnClick="btnBulkUpload_Click" />
                                        </td>
                                        <%--<td colspan="1">
                                            <asp:Button ID="btnShow" runat="server" Text="Upload" OnClick="btnShow_Click" />
                                        </td>--%>
                                    </tr>
                                </asp:Panel>
                                <tr>
                                    <td colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td>
                                                    <%--style="overflow: auto; width: 100%; min-height: 100px; max-height: 190px;"--%>
                                                    <div id="divFEItems" runat="server">
                                                    </div>
                                                    <asp:HiddenField ID="HFSelectedVal" Value="" runat="server" />
                                                    <asp:HiddenField ID="HFIsSubItem" Value="" runat="server" />
                                                    <asp:HiddenField ID="hfSubItemID" Value="" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
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
                                                            <asp:HiddenField ID="hfLoginUser" Value="true" runat="server" />
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
                    <tr visible="false">
                        <td colspan="6" class="bcTdNewTable">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
  <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/nprogress.css" rel="stylesheet" type="text/css" />
   <script src="../JScript/nprogress.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_scrollBody
        {
            height: 150px !important;
            overflow-y: scroll !important;
            overflow: -moz-scrollbars-vertical !important;
            overflow: scroll !important;
        }
    </style>
    <script type="text/javascript">
        //        var buf = document.forms[0]["__VIEWSTATE"].value;
        //        alert("View state is " + buf.length + " bytes");

        $(document).ready(function () {
            Expnder();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });

        function ExpandTXT(CtrlID) {
            $('#' + CtrlID).animate({ "height": "75px" }, "slow");
            $('#' + CtrlID).slideDown("slow");
        }

        function ReSizeTXT(CtrlID) {
            $('#' + CtrlID).animate({ "height": "20px" }, "slow");
            $('#' + CtrlID).slideDown("slow");
        }

        function ShowPreviousFE() {
            if ($('[id$=CHkShow]')[0].checked) {
                $('[id$=ddlRefForeignEnqNo]').removeAttr('disabled', 'disabled');
            }
            else {
                $('[id$=ddlRefForeignEnqNo]').attr('disabled', 'disabled');
                $('[id$=ddlcustmr]').removeAttr('disabled', 'disabled');
                ClrInputs();
            }
        }

        var dateToday = new Date();
        var dateBack = new Date(dateToday.setDate(dateToday.getDate() - 365));
        var MaxDate = new Date(dateToday.setDate(dateToday.getDate() + 365));
        $('[id$=txtenqdt]').datepicker({
            minDate: dateBack,
            maxDate: dateToday,
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
        });

        function changedateEnqDT() {
            var strdateEnqDT = $('[id$=txtenqdt]').val();
            var strdateEnqDT1 = strdateEnqDT.split('-');
            strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
            strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
            $('[id$=txtrecvdt]').datepicker('option', {
                minDate: new Date(strdateEnqDT),
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });

            $('[id$=txtduedt]').val('');
            $('[id$=txtrecvdt]').val('');
        }

        var strdateEnqDT = $('[id$=txtenqdt]').val();
        var strdateEnqDT1 = strdateEnqDT.split('-');
        strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
        strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
        $('[id$=txtrecvdt]').datepicker({
            minDate: strdateEnqDT,
            maxDate: dateToday,
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        });

        function changedate() {
            var strdate = $('[id$=txtrecvdt]').val();
            var strdate1 = strdate.split('-');
            strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
            strdate = new Date(strdate.replace(/-/g, "/"));
            var endDate = "", noOfDaysToAdd = 5, count = 0;
            while (count < noOfDaysToAdd) {
                endDate = new Date(strdate.setDate(strdate.getDate() + 1));
                if (endDate.getDay() != 0) {
                    count++;
                }
            }
            var month = strdate.getMonth() + 1;
            $('[id$=txtduedt]').val(("0" + strdate.getDate()).slice(-2) + '-' + ("0" + month).slice(-2) + '-' + strdate.getFullYear());
            $('[id$=txtimpinst]').focus();
        }

        function ClrInputs() {
            try {
                $('[id$=ddldept]').val('0');
                $('[id$=ddlsubject]').val('0');
                $('[id$=txtenqno]').val('');
                $('[id$=txtrecvdt]').val('');
                $('[id$=txtenqdt]').val('');
                $('[id$=txtduedt]').val('');
                $('[id$=txtimpinst]').val('');
                $('[id$=txtComments]').val('');
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        function isNumberKey(el, evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57)) {
                return false;
            }

            if (charCode == 46 && el.value.indexOf(".") !== -1) {
                return false;
            }
            return true;
        }

        function Myvalidations() {
            try {

                if ($('[id$=CHkShow]')[0].checked == true) {
                    if (($('[id$=ddlRefForeignEnqNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Reference Foreign PO Number is Required.');
                        $('[id$=ddlRefForeignEnqNo]').focus();
                        return false;
                    }
                }

                if ($('[id$=lblCustomerNm]').css("visibility") != "visible") {
                    if (($('[id$=ddlcustmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                        ErrorMessage('Customer is Required.');
                        $('[id$=ddlcustmr]').focus();
                        return false;
                    }
                }
                if (($('[id$=ddldept]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Department is Required.');
                    $('[id$=ddldept]').focus();
                    return false;
                }
                else if (($('[id$=ddlsubject]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Subject is Required.');
                    $('[id$=ddlsubject]').focus();
                    return false;
                }
                else if (($('[id$=txtenqno]').val()).trim() == '') {
                    ErrorMessage('Enqiry Number is Required.');
                    $('[id$=txtenqno]').focus();
                    return false;
                }
                else if (($('[id$=txtenqdt]').val()).trim() == '') {
                    ErrorMessage('Enquiry Date is Required.');
                    $('[id$=txtenqdt]').focus();
                    return false;
                }
                if (($('[id$=hfLoginUser]').val()).trim() == "true") {
                    if (($('[id$=txtrecvdt]').val()).trim() == '') {
                        ErrorMessage('Enquiry Received Date is Required.');
                        $('[id$=txtrecvdt]').focus();
                        return false;
                    }
                    else if (($('[id$=txtduedt]').val()).trim() == '') {
                        ErrorMessage('Enquiry Due Date is Required.');
                        $('[id$=txtduedt]').focus();
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

                var rowCount = $('#tblItems tbody tr').length;
                if (rowCount == 1) {
                    for (var i = 1; i <= rowCount; i++) {
                        var units = $('#ddlUnits' + '' + i).val();
                        var qty = $('#txtQty' + '' + i).val();
                        var ItDesc = $('#ddl' + '' + i).val();
                        if (ItDesc != 0) {
                            if ($('#ddlUnits' + '' + i).val() == '0' || $('#txtQty' + '' + i).val() == '0' || $('#txtQty' + '' + i).val() == '') {
                                //alert('sample : ' + $('#ddlUnits' + '' + i).val());
                                var message = '';
                                if (qty == '')
                                    message = 'Quantity is Required';
                                else if (qty == '0')
                                    message = 'Quantity Cannot Be Zero';
                                else if (units == 0)
                                    message = 'Units is Required'; ErrorMessage(message + ' of SNo : ' + i + '.');
                                return false;
                                break;
                            }
                        }
                        else {
                            ErrorMessage('Please Select atleast 1 Item.');
                            return false;
                            break;
                        }
                    }
                }
                else if (rowCount > 1) {
                    for (var i = 1; i <= rowCount; i++) {
                        var units = $('#ddlUnits' + '' + i).val();
                        var qty = $('#txtQty' + '' + i).val();
                        var ItDesc = $('#ddl' + '' + i).val();
                        if (ItDesc != 0) {
                            if (units == 0 || qty == 0 || qty == '') {
                                var message = '';
                                if (qty == '') {
                                    message = 'Quantity is Required';
                                    $('#txtQty' + '' + i).focus();
                                }
                                else if (qty == '0') {
                                    message = 'Quantity Cannot Be Zero';
                                    $('#txtQty' + '' + i).focus();
                                }
                                else if (units == 0) {
                                    message = 'Units is Required';
                                    $('#ddlUnits' + '' + i).focus();
                                }
                                ErrorMessage(message + ' of SNo : ' + i + '.');
                                return false;
                                break;
                            }
                        }
                    }
                }
                else if (rowCount == 0) {
                    ErrorMessage('No Rows to save.');
                    return false;
                }
                else {
                    return true;
                }
            }
            catch (Error) {
                ErrorMessage(Error.message);
            }
        }

        var oTable;
        $(document).ready(function () {
            //            jQuery('body').show();
            //            NProgress.start();
            //            setTimeout(function () { jQuery('body').fadeTo('slow', 0.5).fadeTo('slow', 50.0); jQuery('body').css('cursor', 'wait'); }, 3000);
            //            setTimeout(function () { NProgress.done(); jQuery('.fade').removeClass('out'); jQuery('body').css('cursor', 'auto'); }, 3000);
            DesignGrid();
        });

        function DesignGrid() {
            $("[id$=tblItems]").dataTable({
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false, //to disable search Box 
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false,

                //Scrolling--------------
                "sScrollY": "700px",
                "sScrollX": "100%",
                "sScrollXInner": "100%"
                //------------
            });
            /* Init the table */
            oTable = $('#tblItems').dataTable();
        }

        function fnGetSelected(oTableLocal) {
            return oTableLocal.$('tr.row_selected');
        }

        function UpdateSelectedItem() {
            var RowID = 0; // (Number($('#tblItems tbody tr').length) + 1);
            var ItemID = $('#Item' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            if (Qty != '') {
                Qty = parseFloat(Qty);
            }
            var UnitID = $('#ddlUnits' + RowID).val();

            //string value, int ItemID, string PartNo, string Spec, string Make, string Qty, int UnitID
            if (ItemID != '00000000-0000-0000-0000-000000000000' && (Qty != 0 || Qty != '') && UnitID != 0) {
                var selVal = $('[id$=HFSelectedVal]').val();
                var result = Purchase_Indent.UpdateSelectedItem(selVal, ItemID, PartNo, Spec, Make, Qty, UnitID);
                var getDivFEItems = GetClientID("divFEItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                DesignGrid();
            }
            else {
                if (ItemID == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('ItemDescription is Required.');
                    $('#ddl' + RowID).focus();
                }
                else if (Qty == '') {
                    ErrorMessage('Quantity cannot be empty.');
                    $('#txtQuantity' + RowID).focus();
                }
                else if (Qty == 0) {
                    ErrorMessage('Quantity cannot be zero.');
                    $('#txtQuantity' + RowID).focus();
                    $('#txtQuantity' + RowID).focus('');
                }
                else if (UnitID == 0) {
                    ErrorMessage('Unit is Required.');
                    $('#ddlUnits' + RowID).focus();
                }
            }
        }

        $('[src*=expand]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            var ItemID = $("[id$=hfItemID" + TRid + "]").val();
            var NewTbl = Purchase_Indent.Expand_FEs(ItemID, TRid);
            $(this).closest("tr").after(NewTbl.value);
            $(this).attr("src", "../images/collapse.png");
            $("[id$=btnExpand" + TRid + "]").prop('title', 'Collapse');
        });

        $('[src*=collapse]').live('click', function (e) {
            e.preventDefault();
            var TRid = $(this).attr('id');
            $(this).attr("src", "../images/expand.png");
            $(".DEL" + TRid).remove();
            $("[id$=btnExpand" + TRid + "]").prop('title', 'Expand');
        });


        function fnOpen_AddSubItems(TRID, RowID, ClickRowID) {
            var returnVall = window.showModalDialog("../Customer Access/Additem_PI.aspx", "Add Item",
            "dialogHeight:680px; dialogWidth:1080px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVall != null) {
                var rtnVal = returnVall.split(',');
                if (rtnVal[1].trim() == "") {
                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                }
                //                else {
                //                    $('#hfSubItemID' + ClickRowID).val(rtnVal[0]);
                //                    GetItemDesc_Spec(rtnVal[0], ClickRowID);
                //                }
            }
            else
                returnVall = "";
            return returnVall;
        }


        function GetItemDesc_Spec(ItmID, id) {
            var rslt = Purchase_Indent.GetItemDesc_Spec(ItmID, id);
            var aray = rslt.value.split('^~^,');
            $("[id$=lblItemDesc" + id + "]").text(aray[0]);
            $("[id$=lblPartNo" + id + "]").text(aray[1]);
            $("[id$=txtDesc-Spec" + id + "]").val(aray[2]);
        }


        $(".fnOpen").live('click', function () {
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var res1 = fnOpen_AddSubItems(aray[0], aray[1], ClickRID);
            if (res1 != "")
                Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, false);
        });

        $(".addrow").live('click', function () {
            var RowIndex = $(this).closest('td').parent()[0].sectionRowIndex;
            var ClickRID = $(this).closest('tr').attr('id');
            var aray = ClickRID.split('a');
            Add_Sub_Itms1(aray[0], aray[1], ClickRID, RowIndex, true);
        });


        function savechanges1(TrID, SNo, x) {
            var ClickRID = TrID + 'a' + SNo;
            var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
            Add_Sub_Itms1(TrID, SNo, ClickRID, RowIndex, false);
        }


        function Add_Sub_Itms1(TrID, RowID, ClickRID, RowIndex, IsAdd) {
            var ParentItemID = $('#hfItemID' + TrID).val();
            var SubRowID = $('#lblSubSNo' + ClickRID).text();
            var ItemID = $('#hfSubItemID' + ClickRID).val();
            var ItmDesc = $('#lblItemDesc' + ClickRID).text();
            var Spec = $("[id$=txtDesc-Spec" + ClickRID + "]").val();
            var PNo = $("[id$=lblPartNo" + ClickRID + "]").text();
            var Make = $("[id$=txtMake" + ClickRID + "]").val();
            var Qty = $('#txtQuantity' + ClickRID).val();
            var UnitID = $('#ddlUnits' + ClickRID).val();
            if (Qty == "") {
                Qty = "0";
            }
            if (IsAdd != true)
                IsAdd = false;
            if (IsAdd == true) {
                if (ItemID.trim() == "" || ItemID.trim() == 0) {
                    ErrorMessage('Select Item to add new row.');
                    return false;
                }
                else if (ItmDesc.trim() == "") {
                    ErrorMessage('Item Description is required.');
                    return false;
                }

                else if (Qty.trim() == "" || Qty.trim() == 0) {
                    ErrorMessage('Item Quantity cannot be empty or zero.');
                    return false;
                }
                else if (UnitID.trim() == "" || UnitID.trim() == 0) {
                    ErrorMessage('Item Unit is required.');
                    return false;
                }
                else {
                    var res;
                    res = Purchase_Indent.Add_Sub_Itms(RowID, TrID, SubRowID, ParentItemID, ItemID, ItmDesc, Spec, PNo, Make, Qty, UnitID, IsAdd);

                    $(".DEL" + TrID).remove();
                    $('#tblItems > tbody > tr').eq((RowIndex - RowID)).after(res.value);
                }
            }
            else {
                var res;
                res = Purchase_Indent.Add_Sub_Itms(RowID, TrID, SubRowID, ParentItemID, ItemID, ItmDesc, Spec, PNo, Make, Qty, UnitID, IsAdd);

                $(".DEL" + TrID).remove();
                $('#tblItems > tbody > tr').eq((RowIndex - RowID)).after(res.value);
            }
        }


        function Delete_SubItem(TrID, SNo, x) {
            if (confirm("Are you sure you want to Delete?")) {
                var ClickRID = $(x).closest('tr').attr('id');
                var RowIndex = $(x).closest('td').parent()[0].rowIndex - 1;
                var ParentItemID = $('#hfItemID' + TrID).val();
                var ItemID = $('#hfSubItemID' + ClickRID).val();
                var res = Purchase_Indent.Delete_SubItem(ParentItemID, TrID, SNo, ItemID);
                $(".DEL" + TrID).remove();
                $('#tblItems > tbody > tr').eq((RowIndex - SNo)).after(res.value);
            }
        }

        function isSomeSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) &&
            (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false; //charCode != 32 &&
            return true;
        }
        function isSomeSplCharSpace(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) &&
            (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function uploadComplete() {
            var result = Purchase_Indent.AddItemListBox();
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

        function SelectDDLunits(ID) {
            var DDLText = "No(s)";
            var DDLVal = "" + $("#ddlUnits0 option:contains('" + DDLText + "')").attr('value') + "";
            $('#ddlUnits' + ID).val(DDLVal);
        }

        function FillSpec_ItemDesc(ID) {
            var ItemID = $('#ddl' + ID).val();
            var result = Purchase_Indent.FillSpec_ItemDesc(ItemID);
            var arr = result.value.split('&@&');
            $('#Item' + ID).val(arr[0]);
            $('#ddl' + ID).val(arr[1]);
            $('#txtPartNo' + ID).val(arr[2]);
            $('#txtSpec' + ID).val(arr[3]);
            $('[id$=HFIsSubItem]').val(arr[4]);
            //$('#ddlUnits' + ID).val('103');
            SelectDDLunits(ID);
        }

        function AddItemRow(RowID) {
            //var ItemID = $('#ddl' + RowID).val();
            var ItemID = $('#Item' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            var UnitID = $('#ddlUnits' + RowID).val();
            var IsSubitem = $('[id$=HFIsSubItem]').val(); //$('#HFIsSubItem').val();



            if (ItemID != 0 && (parseFloat(Qty) != 0 && Qty != '') && UnitID != 0) {
                //int RowID, int ItemID, string PartNo, string Spec, string Make, string Qty, int UnitID
                var result = Purchase_Indent.FillItemGrid(RowID, ItemID, PartNo, Spec, Make, Qty, UnitID, IsSubitem);
                var getDivFEItems = GetClientID("divFEItems").attr("id");
                $('#tblItems tbody').html('');
                $('#tblItems tbody').append(result.value);
                //oTable = $('#tblItems').dataTable();

                var Count = (Number($('#tblItems tbody tr').length));
                $('#btnDel' + Count).focus();
                RowID = 0;
                $('#ddl' + RowID + ' > option[value=' + ItemID + ']').remove();

                $('#ddl' + RowID).val('');
                $('#txtPartNo' + RowID).val('');
                $('textarea#txtSpec' + RowID).val('');
                $('#txtMake' + RowID).val('');
                $('#txtQuantity' + RowID).val('');
                //$('#ddlUnits' + RowID).val('0');
                SelectDDLunits(RowID);
                //DesignGrid();
            }
            else {
                if (ItemID == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('ItemDescription is Required.');
                    $('#ddl' + RowID).focus();
                }
                else if (Qty == '') {
                    ErrorMessage('Quantity cannot be empty.');
                    $('#txtQuantity' + RowID).focus();
                }
                else if (parseFloat(Qty) == 0) {
                    ErrorMessage('Quantity cannot be zero.');
                    $('#txtQuantity' + RowID).focus();
                    $('#txtQuantity' + RowID).focus('');
                }
                else if (UnitID == 0) {
                    ErrorMessage('Unit is Required.');
                    $('#ddlUnits' + RowID).focus();
                }
            }
        }

        var returnVal = "";
        var flag = 0;
        function fnOpen(id) {
            //function fnOpen(id, rowIndex) {

            returnVal = window.showModalDialog("../Customer Access/Additem_PI.aspx", "Add Item",
            "dialogHeight:610px; dialogWidth:1080px; dialogLeft:150; dialogright:150; dialogTop:150; ");
            if (returnVal != null) {
                var rtnVal = returnVal.split(',');
                if (rtnVal[1].trim() == "") {
                    //$('#ddl' + id).val(rtnVal[0]);
                    $('#ddl' + id).val(rtnVal[0].toLowerCase())
                    FillSpec_ItemDesc(id);
                }
                else {
                    var result = Purchase_Indent.NewItemAdded();
                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    $('#ddl' + id).val(rtnVal[0]);
                    FillSpec_ItemDesc(id);
                    DesignGrid();
                }
                returnVal = "";
            }
            else
                returnVal = "";
        }

        function FillItemGrid(obj1, obj2) {

            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
            //            var ddlCat = GetClientID("ddlCategory" + (parseInt(obj2) + 1)).attr("id");
            //            var obj4 = $('#' + ddlCat).val();
            var ddlUnit = GetClientID("ddlUnits" + (parseInt(obj2) + 1)).attr("id");
            var obj5 = $('#' + ddlUnit).val();
            var txtspec = GetClientID("txtSpecification" + (parseInt(obj2) + 1)).attr("id");
            var spec = $('#' + txtspec).val();
            var txtmake = GetClientID("txtMake" + (parseInt(obj2) + 1)).attr("id");
            var make = $('#' + txtmake).val();
            var txtqty = GetClientID("txtQty" + (parseInt(obj2) + 1)).attr("id");
            var qty = $('#' + txtqty).val();
            var lblPNo = GetClientID("lblPartNo" + (parseInt(obj2) + 1)).attr("id");
            var PNo = $('#' + lblPNo).text();
            if (obj3 == '') {
                obj3 = 0;
            }

            if (obj3 == '' && flag == '') {
                ErrorMessage('ItemDescription is Required.');
            }
            else {
                if ((qty != '' && qty != '.')) {//|| obj4 != '0'                
                    var result = Purchase_Indent.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);

                    if ($('#' + txtqty).val() == '' || $('#' + txtqty).val() == '0') {
                        $('#' + txtqty).focus();
                        ErrorMessage('Quantity is required.');
                    }
                    else {
                        $('#' + ddlUnit).focus();
                        //ErrorMessage('Unit is required.');
                    }
                }
                else if (qty == '' && obj3 == 0) {
                    var result = Purchase_Indent.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else {
                    if (qty == '' || qty == '.') {
                        var txtqty = GetClientID("txtQty" + (parseInt(obj2) + 1)).attr("id");
                        $('#' + txtqty).focus();
                        ErrorMessage('Quantity Cannot Be Empty.');
                    }
                }
                Expnder();
            }
        }
        function DeleteItem(obj1) {
            var result = Purchase_Indent.DeleteItem(obj1);
            var getDivFEItems = GetClientID("divFEItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
            DesignGrid();
        }

        function AddItemColumn(obj1, obj2) {
            //            var ddl = GetClientID("ddl" + (parseInt(obj2) + 1)).attr("id");
            //            var obj3 = $('#' + ddl).val();
            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
            var ddl1 = GetClientID("ddl" + (parseInt(obj2) + 2)).attr("id");
            var obj4 = $('#' + ddl1).val();
            if (obj3 != '') {
                if (obj3 != '' && obj3 > 0) {
                    var result = Purchase_Indent.AddItemColumn(obj2 + 1, obj1, obj3);
                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else if (obj3 != 'undefined' && obj3 == 0) {
                    alert('Please Select an Item'); $('#' + ddl1).focus();
                }
                //                else if (obj4 != 'undefined') {
                //                    var result = NewEnquiry.AddItemColumn(obj2 + 1, obj1, obj3);
                //                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                //                    $('#' + getDivFEItems).html(result.value);
                //                }
                $('[id$=ddl' + (obj1 + 1) + ']').focus();
            }
            else { alert('Please Select an Item'); $('#' + ddl).focus(); }
            Expnder();
        }

        function CancelEdit() {
            $('#btnaddItem').show();
            $('#btnEditItem').hide();
            $('#btnCancel').hide();

            var RowID = 0;
            $('#lblEditID' + RowID).text('');
            $('#ddl' + RowID).val(0);
            $('#txtPartNo' + RowID).val('');
            $('#txtSpec' + RowID).val('');
            $('#txtMake' + RowID).val('');
            $('#txtQuantity' + RowID).val('');
            //$('#ddlUnits' + RowID).val(0);
            SelectDDLunits(RowID);
        }

        function EditSelectedItem(RowID) {
            var SelID = Number(RowID) + 1;
            var ItemID = $('#hfItemID' + SelID).val();
            var result = Purchase_Indent.EditItemRow(SelID, ItemID);
            var arr = result.value.split('&@&');

            RowID = 0;
            var select = document.getElementById("ddl" + RowID);
            var option = document.createElement('option');
            option.text = arr[1];
            option.value = ItemID;
            var HfVal = SelID + "," + ItemID;
            $('[id$=HFSelectedVal]').val(HfVal);
            //alert($("#ddl" + RowID + " option[value=" + ItemID.trim() + "]").length);
            //var exists = 0 != $('#ddl"' + RowID + '" option[value=' + ItemID + ']').length;            
            //if ($("#ddl" + RowID + "value=" + option.text + "]").length == 0) {
            //select.add(option, 0);
            $('#ddl' + RowID).val(arr[1]);
            // }
            $('#btnaddItem').hide();
            $('#btnEditItem').show();
            $('#btnCancel').show();

            $('#lblEditID' + RowID).text(SelID);
            $('#Item' + RowID).val(ItemID.trim());
            $('#txtPartNo' + RowID).val(arr[2].trim());
            $('#txtSpec' + RowID).val(arr[3]);
            $('#txtMake' + RowID).val(arr[4]);
            $('#txtQuantity' + RowID).val(arr[5].trim());
            $('#ddlUnits' + RowID).val(arr[6].trim());
        }

        function doConfirm(id) {
            //            var ddl = GetClientID("ddl" + (parseInt(id) + 1)).attr("id");
            //            var ItmId = $('#' + ddl).val();
            var ItmId = $('#hfItemID' + (parseInt(id) + 1)).val();
            var Cst = Purchase_Indent.CheckStat(ItmId, id);
            if (Cst.value == true) {
                if (confirm("Are you sure you want to Continue?")) {
                    var result = Purchase_Indent.DeleteItem(ItmId);
                    var getDivFEItems = GetClientID("divFEItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else {
                    return false;
                }
            }
            else {
                ErrorMessage('This Item is Used By another Transection.');
            }
            Expnder();
            DesignGrid();
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = Purchase_Indent.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';

                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });

        $('#lnkAdd').click(function () {
            var result = Purchase_Indent.AddItemListBox();
            var getDivFEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
        });

        function CheckEnquiryNo() {
            var enqNo = $('[id$=txtenqno]').val();
            var result = Purchase_Indent.CheckEnquiryNo(enqNo);
            if (result.value == false) {
                $("#<%=txtenqno.ClientID%>")[0].value = '';
                ErrorMessage('Enquiry Number Exists.');
                $("#<%=txtenqno.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function ShowPopup(id, rowIndex) {

            var options = {
                title: "Add User Account",
                width: 950,
                height: 400,
                url: "/../Home.aspx"
            };
            SP.UI.ModalDialog.showModalDialog(options);
            return false;
        }
        function fnRandom(iModifier) {
            return parseInt(Math.random() * iModifier);
        }               
    </script>
</asp:Content>
