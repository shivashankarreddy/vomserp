<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="NewFPOrderVerbal.aspx.cs" Inherits="VOMS_ERP.Purchases.NewFPOrderVerbal" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="HselectedItems" runat="server" Value="" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Foreign Purchase Order Verbal"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc"
                                align="center">
                                <tr>
                                    <td>
                                        <span class="bcLabel">If Repeated Order : </span>
                                    </td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="chkbIRO" CssClass="bcCheckBoxList" AutoPostBack="true"
                                            OnCheckedChanged="CHkShow_CheckedChanged" />
                                        <asp:HiddenField ID="HF2" runat="server" />
                                        <asp:HiddenField ID="HFID" runat="server" />
                                    </td>
                                    <td colspan="4">
                                        <span id="spnRfpolbl" runat="server" class="bcLabel" style="display: none;">Ref. FPO
                                            Number<font color="red" size="2"><b>*</b></font>:</span> <span id="spnRfpoddl" class="bcLabel"
                                                runat="server" style="display: none;">
                                                <asp:DropDownList runat="server" ID="ddlRefFPO" AutoPostBack="true" OnSelectedIndexChanged="ddlRefFPO_SelectedIndexChanged"
                                                    CssClass="bcAspdropdown">
                                                    <asp:ListItem Text="Select Ref. FPO" Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Customer<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlcustomer" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustomer_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="00000000-0000-0000-0000-000000000000" Text="Select Customer"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label runat="server" ID="lblCustomerNm" class="bcLabel" Text="" Visible="false"></asp:Label>
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                    <td class="bcTdnormal">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Subject<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtsubject" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">FPO Raised by<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlRsdby" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Departmet"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Project/Department Name<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddldept" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Departmet"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PurchaseOrder No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        
                                        <asp:TextBox runat="server" MaxLength="150" ID="txtFpoNo" onkeypress='return isSpace(event)' onchange="CheckFPOOrderNo();"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">FPO Date<font color="red" size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtReceivedDate" onchange="ChangeDueDate()" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td>
                                        <span class="bcLabel">FPO Entry Date<font color="red" size="2"><b>*</b></font></span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtFpoDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">FPO Due Date<font color="red" size="2"><b>*</b></font>: </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtFpoDuedt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Important Instructions: </span>
                                    </td>
                                    <td class="bcTdnormal" colspan="3">
                                        <asp:TextBox runat="server" ID="txtimpinst" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            Style="width: 550px;"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                <tdclass="bcTdnormal" colspan="6">
                                <asp:Panel ID="PnlImp" runat="server" Width="98%">
                                    <tr>
                                        <td>
                                           <span class="bcLabel"> Upload Items From Excel:</span>
                                        </td>
                                        <td colspan="2">
                                           <span class="bcLabel"> <asp:FileUpload ID="FileUpload1" runat="server"/></span>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnBulkUpload" runat="server" Text="Upload" OnClick="btnBulkUpload_Click" />
                                        </td>
                                        <%--<td colspan="1">
                                            <asp:Button ID="btnShow" runat="server" Text="Upload" OnClick="btnShow_Click" />
                                        </td>--%>
                                    </tr>
                                </asp:Panel>
                                </td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="bcTdnormal">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="41%">
                                                        <span id="Span1" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="center">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" colspan="6">
                                        <%--style="overflow: auto; width: 100%; min-height: 200px; max-height: 300px;"--%>
                                        <div id="divFPOItems" class="aligntable" runat="server"> <%--style="margin-left: 10px !important;">--%>
                                        </div>
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        Terms & Conditions
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdNewTable" colspan="6">
                                        <table style="width: 100%; overflow: auto;">
                                            <tr>
                                                <td colspan="8">
                                                    <table style="width: 100%; overflow: auto;">
                                                        <tr>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Price Basis<font color="red" size="2"><b>*</b></font>: </span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:DropDownList runat="server" ID="ddlPrcBsis" CssClass="bcAspdropdown">
                                                                    <asp:ListItem Text="Select Price Basis" Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                                &nbsp;&nbsp;
                                                                <asp:TextBox runat="server" CssClass="bcAsptextboxRight" onkeypress='return SpclChars(event);'
                                                                    ID="txtPriceBasis"></asp:TextBox>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <span class="bcLabel">Delivery Period<font color="red" size="2"><b>*</b></font>:</span>
                                                            </td>
                                                            <td class="bcTdnormal">
                                                                <asp:TextBox runat="server" ID="txtDlvry" CssClass="bcAsptextbox" onkeyup="extractNumber(this, 0, false);"
                                                                    onblur="extractNumber(this, 0, false); chkDlvrPeriod()" Width="23px" MaxLength="2"
                                                                    onMouseUp="return false" onchange="ChangeDueDate()"></asp:TextBox><span class="bcLabel"><font
                                                                        style="font-family: Verdana; font-size: 12px;"> Weeks</font></span>
                                                            </td>
                                                            <td align="right">
                                                                <span class="bcLabelright">Shipment Mode<font color="red" size="2"><b>*</b></font>:</span>
                                                            </td>
                                                            <td align="left" colspan="2">
                                                                <asp:RadioButtonList ID="rbtnshpmnt" runat="server" RepeatDirection="Horizontal"
                                                                    ForeColor="#000000" Font-Size="11px" font-family="Arial">
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr style="background-color: Gray; font-style: normal; color: White;">
                                                <td>
                                                    Payment Terms :
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="8" class="bcTdNewTable">
                                                    <center>
                                                        <div style="overflow: auto; width: 35%;" id="divPaymentTerms" runat="server">
                                                        </div>
                                                    </center>
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
                                                <td colspan="8" class="bcTdNewTable1" align="right">
                                                    <span><a href="javascript:void(0)" id="lbtnATConditions" title="Add Addtional Terms & Conditions"
                                                        onclick="fnOpen_TC()" class="bcAlink">Additional Terms & Conditions </a></span>
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
                                                            <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                                                Text="Clear" OnClick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <asp:HiddenField ID="HFSelectedVal" runat="server" />
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
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });

        function uploadComplete() {
            var result = NewFPOrderVerbal.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivLEItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
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
            ErrorMessage('File upload Failed.');
        }
        function uploadStarted() {
            SuccessMessage('File Uploaded Started.');
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != null) {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = NewFPOrderVerbal.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    //SuccessMessage('Selected Attachment Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';

                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });


        $('#lnkAdd').click(function () {
            var result = NewFPOrderVerbal.AddItemListBox();
            var getDivFEItems = GetClientID("divListBox").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('div.expanderR').expander();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }

        function chcked(CheckedID, UncheckedID) {
            if ($('[id$=' + CheckedID + ']')[0].checked) {
                $('[id$=' + UncheckedID + ']').attr("checked", false);
            }
        }

        //        function chcked1() {
        //            if ($('[id$=Chkcotecna]')[0].checked) {
        //                $('[id$=Chkbivac]').attr("disabled", true);
        //            }
        ////            if ($('[id$=Chkcotecna]')[0].checked == false) {
        ////                $('[id$=Chkbivac]').attr("disabled", false);
        ////            }
        //        }
    </script>
    <script type="text/javascript">
        var dateToday = new Date();
        $('[id$=txtReceivedDate]').datepicker({
            maxDate: dateToday,
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
            , showAnim: 'blind'
            //, showButtonPanel: true
        });

        function chkDlvrPeriod() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == 0) {
                ErrorMessage('Delivery Period should not be Zero');
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtFpoDuedt]').val('');
            }
        }
        function ChangeDueDate() {
            var Currentweeks = $('[id$=txtDlvry]').val();
            if (Currentweeks == "") {
                $('[id$=txtDlvry]').focus(0, 0);
                $('[id$=txtFpoDuedt]').val('');
            }
            else {
                var strdate = $('[id$=txtReceivedDate]').val();
                var strdate1 = strdate.split('-');
                strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
                strdate = new Date(strdate.replace(/-/g, "/"));
                strdate.setDate(strdate.getDate() + (Currentweeks * 7));
                var month = strdate.getMonth() + 1;
                $('[id$=txtFpoDuedt]').val(("0" + strdate.getDate()).slice(-2) + '-' + ("0" + month).slice(-2) + '-' + strdate.getFullYear());
            }
        }

        function ExpandTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "75px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#txtSpecifications' + '' + obj).animate({ "height": "22px" }, "slow");
            $('#txtSpecifications' + '' + obj).slideDown("slow");
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        
    </script>
    <script type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function doConfirmPayment(id) {
            if (confirm("Are you sure you want to Delete Payment?")) {
                var result = NewFPOrderVerbal.PaymentDeleteItem(id);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
            }
            else {
                return false;
            }
        }

        function getPaymentValues(RNo) {
            var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(RNo))).attr("id");
            var PercAmt = $('#' + txtPercAmt).val();
            var txtDesc = GetClientID("txtDesc" + (parseInt(RNo))).attr("id");
            var Desc = $('#' + txtDesc).val();
            if (PercAmt != '0' && PercAmt != '' && Desc != '') {
                var result = NewFPOrderVerbal.PaymentAddItem(RNo, PercAmt, Desc);
                var getdivPaymentTerms = GetClientID("divPaymentTerms").attr("id");
                $('#' + getdivPaymentTerms).html(result.value);
                if ($('[id$=HfMessage]').val() != '') {

                    ErrorMessage('' + $('[id$=HfMessage]').val() + '');

                    $('[id$=' + txtPercAmt + ']').focus();
                }
                else
                    $('[id$=' + txtDesc + ']').focus();
            }
            else {

                if (PercAmt == '') {
                    ErrorMessage('Payment Percentage is Required.');
                }
                else if (PercAmt == '0') {
                    ErrorMessage('Payment Percentage cannot be Zero.');
                }
                else if (PercAmt > 100) {
                    ErrorMessage('Percentage Cannot Exceed 100');
                    $('#' + txtPercAmt).val('0');
                    $('[id$=' + txtPercAmt + ']').focus();
                }

                $('#' + txtPercAmt).focus();
            }
            $('#' + txtDesc).focus();
        }
    </script>
    <script type="text/javascript">

        function doConfirm(id) {
            if (confirm("Are you sure you want to Delete?")) {
                var result = NewFPOrderVerbal.DeleteItem(id);
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                Expnder();
            }
            else {
                return false;
            }
        }

        function doConfirmRegret(id) {
            if (confirm("Are you sure you want to Regret?")) {
                var fenqid = $('[id$=ddlfenq] :selected').val();
                var result = NewFPOrderVerbal.RegretItem(id, fenqid);
                if (result.value == true) {

                    $('[id$=divMyMessage]').append('<span class="Success">Item Regretted Successfully.');


                    var result1 = NewFPOrderVerbal.BindGridView(0);
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
                    $('#' + getDivFEItems).html(result1.value);
                    Expnder();
                }
                else {

                    ErrorMessage('Error while Regretting.');

                }
            }
            else {
                return false;
            }
        }

        function AlertError() {
            alert('You donot have permissions to Regret, Contact your Admin.');
        }

        function AddItemColumn(id, isnew) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var ddlUnits = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var Units = $('#' + ddlUnits).val();
            var txtSpec = GetClientID("txtSpecifications" + (parseInt(id))).attr("id");
            var spec = $('#' + txtSpec).val();
            var txtMake = GetClientID("txtMake" + (parseInt(id))).attr("id");
            var Make = $('#' + txtMake).val();
            var txtQty = GetClientID("txtQuantity" + (parseInt(id))).attr("id");
            var Qty = $('#' + txtQty).val();
            var txtrt = GetClientID("txtRate" + (parseInt(id))).attr("id");
            var Rate = $('#' + txtrt).val();
            var txtPrtNo = GetClientID("txtPrtNo" + (parseInt(id))).attr("id");
            var PrtNo = $('#' + txtPrtNo).val();
            var txtRmrks = GetClientID("txtRemarks" + (parseInt(id))).attr("id");
            var Rmrks = $('#' + txtRmrks).val();

            if (sv == undefined && PrtNo == '' && spec == '' && Make == '' && Qty == '' && Rate == 0) {

                ErrorMessage('Fill all the Details.');

                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
            }
            else if (sv == undefined && PrtNo == undefined && spec == '' && Make == '' && Qty == '' && Rate == 0) {

                ErrorMessage('Fill all the Details..');

                if (ddlUnits != undefined) {
                    $("#" + ddlUnits + " option[value='0']").attr("selected", "selected");
                }
            }
            else if (sv != undefined && sv == 0) {

                ErrorMessage('Select Item Description.');

            }
            else {
                if (sv == undefined)
                    sv = '';
                if (PrtNo == undefined)
                    PrtNo = '';
                if (Units == undefined)
                    Units = '';
                if (Rate == undefined || Rate == '0')
                    Rate = 0;
                //var test = e.tagName; 
                var result = NewFPOrderVerbal.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Rate, Units, Rmrks, isnew);
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
            Expnder();
        }

        function DisplayRefFPO() {
            if ($('[id$=chkbIRO]')[0].checked) {
                $('[id$=spnRfpolbl]')[0].style.display = "block";
                $('[id$=spnRfpoddl]')[0].style.display = "block";
            }
            else {
                $('[id$=spnRfpolbl]')[0].style.display = "none";
                $('[id$=spnRfpoddl]')[0].style.display = "none";
                window.location = "NewFPOrderVerbal.aspx";
            }
            var result = NewFPOrderVerbal.ChkBoxRptdFPOMode($('[id$=chkbIRO]')[0].checked);
        }
        function FillItemDRP(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = NewFPOrderVerbal.FillItemDRP(id, sv);
            var getDivFEItems = GetClientID("divFPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function FillUnitDRP(id) {
            var ddlCat = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = NewFPOrderVerbal.FillUnitDRP(id, sv);
            var getDivFEItems = GetClientID("divFPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function Myvalidations() {
            var PaymentRCount = $('#tblPaymentTerms tbody tr').length;
            var TotalAmt = 0;
            var rowCount = $('#tblItems tbody tr').length;
            var count = 0;


            if ($('[id$=chkbIRO]')[0].checked == true) {
                if (($('[id$=ddlRefFPO]').val()).trim() == '0') {
                    ErrorMessage('Reference Foreign PO Number is Required.');
                    $('[id$=ddlRefFPO]').focus();
                    return false;
                }
            }
            if ($('[id$=lblCustomerNm]').css("visibility") != "visible") {
                if (($('[id$=ddlcustomer]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Customer Name is Required.');
                    $('[id$=ddlcustomer]').focus();
                    return false;
                }
            }
            if ($('[id$=txtsubject]').val() == '') {
                ErrorMessage('Subject is Required.');
                $('[id$=txtsubject]').focus();
                return false;
            }
            else if ($('[id$=ddlRsdby]').val() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('FPO raised By is Required.');
                $('[id$=ddlRsdby]').focus();
                return false;
            }
            else if (($('[id$=ddldept]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Department Name is Required.');
                $('[id$=ddldept]').focus();
                return false;
            }
            else if (($('[id$=txtFpoNo]').val()).trim() == '') {
                ErrorMessage('Foreign PurchaseOrder No is Required.');
                $('[id$=txtFpoNo]').focus();
                return false;
            }
            else if (($('[id$=txtFpoDt]').val()).trim() == '') {
                ErrorMessage('FPO Date is Required.');
                $('[id$=txtFpoDt]').focus();
                return false;
            }
            else if (($('[id$=txtReceivedDate]').val()).trim() == '') {
                ErrorMessage('FPO Received date is Required.');
                $('[id$=txtReceivedDate]').focus();
                return false;
            }
            else if (($('[id$=txtFpoDuedt]').val()).trim() == '') {
                ErrorMessage('FPO Due Date is Required.');
                $('[id$=txtFpoDuedt]').focus();
                return false;
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

            if ($('[id$=ddlPrcBsis]').val() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Price Basis is Required.');
                $('[id$=ddlPrcBsis]').focus();
                return false;
            }
            else if (($('[id$=txtPriceBasis]').val()).trim() == '') {
                ErrorMessage('Price Basis Location is Required.');
                $('[id$=txtPriceBasis]').focus();
                return false;
            }
            else if ($('[id$=txtDlvry]').val() == '' || $('[id$=txtDlvry]').val() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Delivery Period is Required.');
                $('[id$=txtDlvry]').focus();
                return false;
            }
            if (PaymentRCount > 0) {
                for (var k = 1; k <= PaymentRCount; k++) {
                    var txtPercAmt = GetClientID("txtPercAmt" + (parseInt(k))).attr("id");
                    var PercAmt = $('#' + txtPercAmt).val();
                    var txtDesc = GetClientID("txtDesc" + (parseInt(k))).attr("id");
                    var Desc = $('#' + txtDesc).val();
                    TotalAmt = parseFloat(TotalAmt) + parseFloat(PercAmt);
                    if (PercAmt == '' || PercAmt == '0' || Desc == '') {
                        //alert('sample : ' + $('#ddlUnits' + '' + i).val());
                        var message = '';
                        if (PercAmt == '')
                            message = 'Payment Is Required';
                        else if (PercAmt == '0')
                            message = 'Payment Cannot be Zero';
                        else if (Desc == '')
                            message = 'Description Is Required';
                        ErrorMessage('' + message + ' of SNo : ' + k + '.');
                        //$('[id$=txtPercAmt' + k - 1 + ']').focus();
                        return false;
                        break;
                    }
                }
                if (PaymentRCount == 0 || TotalAmt < 100) {
                    ErrorMessage('Total Payment percentage should be 100.');
                    return false;
                }
            }
            else if (PaymentRCount <= 0) {
                ErrorMessage('Payment Terms is Required.');
                return false;
            }
            else {
                return true;
            }
        }
        var gett = "";
        function GetValueFromChild(myVal) {
            gett = myVal;
            window.focus();
            var returnVal = myVal;
            if (returnVal != null) {
                var rtnVal = returnVal.split(',');
                if (rtnVal[1].trim() == "") {
                    //                    $('#ddl' + id).val(rtnVal[0]);
                    //                    FillSpec_ItemDesc(id);
                }
                else {
                    var result = NewFPOrderVerbal.NewItemAdded();
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    //                    $('#ddl' + id).val(rtnVal[0]);
                    //                    FillSpec_ItemDesc(id);
                    DesignGrid();
                }
                returnVal = "";
            }
            else
                returnVal = "";
            var result = NewFPOrderVerbal.FillSpec_ItemDesc(rtnVal[0]);
            var arr = result.value.split('&@&');
            $('#txtPartNo0').val(arr[0]);
            $('#txtSpec0').val(arr[1]);
            $('#Txt_IM').val(arr[2])
            SelectDDLunits(0);
            Txt_AutoComplete();
            var item = { label: $('#Txt_IM').val(), value: $('#Txt_IM').val(), val: rtnVal[0] };
            try {
                $('#Txt_IM').data('ui-autocomplete').selectedItem = item;
                //                $('#Txt_IM').data('ui-autocomplete').selectedItem.val = rtnVal[0];
                //                $('#Txt_IM').data('ui-autocomplete').selectedItem.value = arr[3];
                //                $('#Txt_IM').val(arr[1]);
            }
            catch (Error) {
            }
        }
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();

            window.open("../Enquiries/AddItems.aspx", "", sFeatures);

        }

        function SpclChars(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58
            && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function fnSetValues() {
            var iHeight = 500;
            var iWidth = 1000;
            var sFeatures = "dialogHeight: " + iHeight + "px; dialogWidth: " + iWidth + "px;";
            return sFeatures;
        }
        function fnOpen_TC() {
            var sFeatures = fnSetValues();
            var LQuoteID = $('[id$=HF2]').val();
            var ED = $('[id$=HFID]').val();
            var w = 1000;
            var h = 550;
            var x = (screen.width / 2) - (w / 2);
            var y = (screen.height / 2) - (h / 2);
            //            if (ED == '00000000-0000-0000-0000-000000000000') {
            //                window.open("../Masters/TermsNConditions.aspx?TAr=General&FQQuteID=" + LQuoteID + "&VFPO=true", "508", "width=1000,height=500, top=" + y + ", left=" + x + ""); //../Enquiries/AddItems.aspx
            //            }
            //            else {
            window.open("../Masters/TermsNConditions.aspx?TAr=General&FQQuteID=" + LQuoteID + "&VFPO=false", "508", "width=1000,height=500, top=" + y + ", left=" + x + ""); //../Enquiries/AddItems.aspx
            //            }
        }
        
    </script>
    <script type="text/javascript">
        function CheckFPOOrderNo() {
            var enqNo = $('[id$=txtFpoNo]').val().trim();
            var result = NewFPOrderVerbal.CheckFPOOrderNo(enqNo);
            if (result.value == false) {
                //alert('Mail-ID Exists');
                $("#<%=txtFpoNo.ClientID%>")[0].value = '';
                ErrorMessage('FPO Order Number Exists.');
                $("#<%=txtFpoNo.ClientID%>")[0].focus();
                return false;
            }
        }

        function CheckItem(ID) {
            var IsChecked = $('#ckhChaild' + ID).is(':checked');
            var result = NewFPOrderVerbal.CheckItem(IsChecked, ID);
            $('#tblItems').html('');
            $('#tblItems').html(result.value);
            Expnder();
        }
        
    </script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {
            DesignGrid();
        });

        $(document).ready(function () {
            Txt_AutoComplete();
            $(".aligntable").width($(window).width() - 84 + "px");
        });

        function DesignGrid() {
            $("[id$=tblItems]").dataTable({
                "bScrollCollapse": true,
                "bPaginate": false,
                "bSort": false,
                "bFilter": false,
                "bInfo": false,
                "bJQueryUI": true,
                "bAutoWidth": false,
                "sScrollY": "250px",
                "sScrollX": "100%"
                //"sScrollXInner": "100%"

            });
            oTable = $('#tblItems').dataTable();
        }

        function UpdateSelectedItem() {
            var RowID = 0;
            var ITIMID = '00000000-0000-0000-0000-000000000000';
            if ($('#Txt_IM').data('ui-autocomplete').selectedItem != null) {
                ITIMID = $('#Txt_IM').data('ui-autocomplete').selectedItem.val;
            }
            var ItemID = ITIMID;
            //var ItemID = $('#ddl' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            if (Qty != '') {
                Qty = parseFloat(Qty);
            }
            var UnitID = $('#ddlUnits' + RowID).val();
            var HSCode = $('#txtHSCode' + RowID).val();
            var Rate = $('#txtRate' + RowID).val();
            if (Rate == '') {
                Rate = 0;
                Rate = parseFloat(Rate);
            }
            var Remarks = $('#txtRemarks' + RowID).val();

            if (ItemID != '00000000-0000-0000-0000-000000000000' && (Qty != 0 || Qty != '') && UnitID != 0 && Rate != 0) {
                var selVal = $('[id$=HFSelectedVal]').val();
                var result = NewFPOrderVerbal.UpdateSelectedItem(selVal, ItemID, PartNo, Spec, Make, Qty, UnitID, HSCode, Rate, Remarks);
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
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
                    $('#txtQuantity' + RowID).val('');
                }
                else if (UnitID == 0) {
                    ErrorMessage('Unit is Required.');
                    $('#ddlUnits' + RowID).focus();
                }
                else if (Rate == '' || parseFloat(Rate) == 0) {
                    ErrorMessage('Amount cannot be Empty/zero.');
                    $('#txtRate' + RowID).focus();
                    $('#txtRate' + RowID).val('');
                }
            }            
            Txt_AutoComplete();
        }

        function SelectDDLunits(ID) {
            var DDLText = "No(s)";
            var DDLVal = "" + $("#ddlUnits0 option:contains('" + DDLText + "')").attr('value') + "";
            $('#ddlUnits' + ID).val(DDLVal);
        }

        function FillSpec_ItemDesc(ID) {
            var ItemID = $('#ddl' + ID).val();
            var result = NewFPOrderVerbal.FillSpec_ItemDesc(ItemID);
            var arr = result.value.split('&@&');
            $('#txtPartNo' + ID).val(arr[0]);
            $('#txtSpec' + ID).val(arr[1]);
            SelectDDLunits(ID);
        }

        function AddItemRow(RowID) {
        var ITIMID = '00000000-0000-0000-0000-000000000000';
            if ($('#Txt_IM').data('ui-autocomplete').selectedItem != null) {                
                ITIMID = $('#Txt_IM').data('ui-autocomplete').selectedItem.val;
            }
            var ItemID = ITIMID;  //$('#ddl' + RowID).val();
            var PartNo = $('#txtPartNo' + RowID).val();
            var Spec = $('#txtSpec' + RowID).val();
            var Make = $('#txtMake' + RowID).val();
            var Qty = $('#txtQuantity' + RowID).val();
            var UnitID = $('#ddlUnits' + RowID).val();
            var HSCode = $('#txtHSCode' + RowID).val();
            var Rate = $('#txtRate' + RowID).val();
            var Remarks = $('#txtRemarks' + RowID).val();
            if (Rate == '') {
                Rate = 0;
            }


            if (ItemID != '00000000-0000-0000-0000-000000000000' && (parseFloat(Qty) != 0 && Qty != '') && UnitID != '00000000-0000-0000-0000-000000000000' && (parseFloat(Rate) != 0 && Rate != '')) {
                var result = NewFPOrderVerbal.FillItemGrid(RowID, ItemID, PartNo, Spec, Make, Qty, UnitID, HSCode,Rate, Remarks);
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
                // $('#tblItems tbody').html('');
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
                //$('#tblItems tbody').append(result.value);

                var Count = (Number($('#tblItems tbody tr').length));
                $('#btnDel' + Count).focus();
                RowID = 0;
                $('#ddl' + RowID + ' > option[value=' + ItemID + ']').remove();
                $('#ddl' + RowID).val('0');
                $('#txtPartNo' + RowID).val('');
                $('textarea#txtSpec' + RowID).val('');
                $('#txtMake' + RowID).val('');
                $('#txtQuantity' + RowID).val('');
                $('#txtHSCode' + RowID).val('');
                $('#txtRate' + RowID).val('');
                $('#txtRemarks' + RowID).val('');
                SelectDDLunits(RowID);
                DesignGrid();                
                Txt_AutoComplete();
                ClearAT();
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
                else if (Rate == '' || parseFloat(Rate) == 0) {
                    ErrorMessage('Amount cannot be Empty/zero.');
                    $('#txtRate' + RowID).focus();
                    $('#txtRate' + RowID).val('');
                }
            }
        }

        var returnVal = "";
        var flag = 0;
        function fnOpen(id) {
        var sFeatures = fnSetValues();
       var w = 1000;
            var h = 550;
            var x = (screen.width / 2) - (w / 2);
            var y = (screen.height / 2) - (h / 2);
            //function fnOpen(id, rowIndex) {

            returnVal = window.open("../Enquiries/AddItems.aspx", "Add Item",
            "width=1000,height=500, top=" + y + ", left=" + x + "");
            
        }


        function FillItemGrid(obj1, obj2) {

            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
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
                if ((qty != '' && qty != '.')) {
                    var result = NewFPOrderVerbal.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);

                    if ($('#' + txtqty).val() == '' || $('#' + txtqty).val() == '0') {
                        $('#' + txtqty).focus();
                        ErrorMessage('Quantity is required.');
                    }
                    else {
                        $('#' + ddlUnit).focus();
                    }
                }
                else if (qty == '' && obj3 == 0) {
                    var result = NewFPOrderVerbal.FillItemGrid(obj2, obj1, obj3, 0, 274, obj5, spec, make, qty, PNo, returnVal);
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
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

        function doConfirmm(id) {
            var ItmId = $('#hfItemID' + (parseInt(id) + 1)).val();
            //var Cst = NewFPOrderVerbal.CheckStat(ItmId, id);
            //if (Cst.value == true) {
            if (confirm("Are you sure you want to Continue?")) {
                var result = NewFPOrderVerbal.DeleteItems(ItmId);
                var getDivFEItems = GetClientID("divFPOItems").attr("id");
                $('#' + getDivFEItems).html(result.value);
            }
            else {
                ErrorMessage('This Item is Used By another Transection.');
            }
            // Expnder();
            DesignGrid();
             Txt_AutoComplete();
        }

        function DeleteItem(obj1) {
            var result = NewFPOrderVerbal.DeleteItem(obj1);
            var getDivFEItems = GetClientID("divFPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            Expnder();
            DesignGrid();
        }

        function AddItemColumn(obj1, obj2) {
            var obj3 = $('#hfItemID' + (parseInt(obj2) + 1)).val();
            var ddl1 = GetClientID("ddl" + (parseInt(obj2) + 2)).attr("id");
            var obj4 = $('#' + ddl1).val();
            if (obj3 != '') {
                if (obj3 != '' && obj3 > 0) {
                    var result = NewFPOrderVerbal.AddItemColumn(obj2 + 1, obj1, obj3);
                    var getDivFEItems = GetClientID("divFPOItems").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                }
                else if (obj3 != 'undefined' && obj3 == 0) {
                    alert('Please Select an Item'); $('#' + ddl1).focus();
                }
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
            $('#txtRate' + RowID).val('');
            $('#txtHSCode' + RowID).val('');
            $('#txtRemarks' + RowID).val('');
            SelectDDLunits(RowID);
            ClearAT();
        }

        function EditSelectedItem(RowID) {
            var SelID = Number(RowID) + 1;
            var ItemID = $('#hfItemID' + SelID).val();
            var result = NewFPOrderVerbal.EditItemRow(SelID, ItemID);
            var arr = result.value.split('&@&');

            RowID = 0;
             Txt_AutoComplete();
            
            //var item = [{ item: { label: arr[1], value: arr[1], val: ItemID} }];
            var item = { label:  arr[1], value: arr[1], val:ItemID};
            try {
                $('#Txt_IM').data('ui-autocomplete').selectedItem = item;
//                $('#Txt_IM').data('ui-autocomplete').selectedItem.val = ItemID;
//                $('#Txt_IM').data('ui-autocomplete').selectedItem.value = arr[1];
                $('#Txt_IM').val(arr[1]);
            }
            catch (Error) {
            }
//            var select = document.getElementById("ddl" + RowID);
//            var option = document.createElement('option');
//            option.text = arr[1];
//            option.value = ItemID;
            var HfVal = SelID + "," + ItemID;
            $('[id$=HFSelectedVal]').val(HfVal);
//            if ($("#ddl" + RowID + " option[value=" + ItemID.trim() + "]").length == 0) {
//                select.add(option, 0);
//            }
            $('#btnaddItem').hide();
            $('#btnEditItem').show();
            $('#btnCancel').show();

            $('#lblEditID' + RowID).text(SelID);
            $('#ddl' + RowID).val(ItemID.trim());
            $('#txtPartNo' + RowID).val(arr[2].trim());
            $('#txtSpec' + RowID).val(arr[3]);
            $('#txtMake' + RowID).val(arr[4]);
            $('#txtQuantity' + RowID).val(arr[5].trim());
            $('#ddlUnits' + RowID).val(arr[6].trim());
            //$('#ddlUnits' + RowID).val(arr[6].trim());
            $('#txtHSCode' + RowID).val(arr[7].trim());
            $('#txtRate' + RowID).val(arr[8].trim());
            $('#txtRemarks' + RowID).val(arr[9].trim());
        }

       

       
          function Txt_AutoComplete() {
            $('#Txt_IM').autocomplete({
                autoFocus: true,
                autoSelect: true,
                minLength: 1,
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "NewFPOrderVerbal.aspx/GetAutoCompleteData",
                        data: "{'Txt_Text':'" + document.getElementById('Txt_IM').value + "'}",
                        dataType: "json",
                        success: function (data) {
                            //                        response(data.d);
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.split('VAL--VAL--VAL')[0],
                                    val: item.split('VAL--VAL--VAL')[1]
                                }
                            }))

                        },
                        error: function (result) {
                            ErrorMessage("No Records Found !!!");
                        }
                    });
                },
                select: function (event, ui) {
                    var ID = ui.item.val;
                    var result = NewFPOrderVerbal.FillSpec_ItemDesc(ID);
                    var arr = result.value.split('&@&');
                    $('#txtPartNo0').val(arr[0]);
                    $('#txtSpec0').val(arr[1]);
                    $('[id$=HFIsSubItem]').val(arr[2]);
                    SelectDDLunits(0);
                },
                change: function (event, ui) {
                    if (!ui.item) {
                        $('#Txt_IM').val('');
                        var ID = '00000000-0000-0000-0000-000000000000';
                        var result = NewFPOrderVerbal.FillSpec_ItemDesc(ID);
                        var arr = result.value.split('&@&');
                        $('#txtPartNo0').val(arr[0]);
                        $('#txtSpec0').val(arr[1]);
                        $('[id$=HFIsSubItem]').val(arr[2]);
                        SelectDDLunits(0);
                    }
                    else if (ui.item) {
                    }
                },
                close: function (event, ui) {
                    if (ui.item)
                        ErrorMessage("No Records Found !!!");
                },
//                search: function (event, ui) { 
//                }
//                response: function (event, ui) {
//                    ui.content.push
//                ({
//                    label: 'Add',
//                    value: 'Add'
//                });
//                },
//            focus: function( event, ui ) {
//            $('#Txt_IM').val( ui.item.label );
//            return false;
//            },
            });
        }
        function ClearAT() {
            $('#Txt_IM').val('');          
            $('#Txt_IM').data('ui-autocomplete').selectedItem = null;
        }

    </script>
</asp:Content>
