<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="NewFPO_Customer.aspx.cs" Inherits="VOMS_ERP.Customer_Access.NewFPO_Customer" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Foreign Purchase Order"
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
                                            OnCheckedChanged="CHkShow_CheckedChanged" /><%--onclick='DisplayRefFPO()'--%>
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
                                        <span class="bcLabel">Purchase Indent Number(s)<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlfenq" CssClass="bcAspdropdown" Visible="false"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="Select F-Enquiry"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:ListBox runat="server" ID="Lstfenqy" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            OnSelectedIndexChanged="ddlfenq_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign Enquiry Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtfenqDt" CssClass="bcAsptextbox"></asp:TextBox>
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
                                        <span class="bcLabel">Project/Department Name:</span>
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
                                        <asp:TextBox runat="server" MaxLength="150" ID="txtFpoNo" onchange="CheckFPOOrderNo();"
                                            onkeypress='return isSpace(event)' CssClass="bcAsptextbox"></asp:TextBox>
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
                                        <div style="overflow: auto; width: 100%; max-height:300px;" id="divFPOItems" runat="server">
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
                                                                        style="font-family: Verdana; font-size: 12px;"> Weeks</font></span><%--onfocus="this.select()"--%>
                                                            </td>
                                                            <td align="right">
                                                                <span class="bcLabelright">Shipment Mode<font color="red" size="2"><b>*</b></font>:</span>
                                                            </td>
                                                            <td align="left" colspan="2">
                                                                <asp:RadioButtonList ID="rbtnshpmnt" runat="server" RepeatDirection="Horizontal"
                                                                    ForeColor="#000000" Font-Size="11px" font-family="Arial">
                                                                    <%--<asp:ListItem Text="By Air" Value="0"></asp:ListItem>
                                                                    <asp:ListItem Text="By Sea" Value="304" Selected="True"></asp:ListItem>
                                                                    <asp:ListItem Text="By Air/Sea" Value="305"></asp:ListItem>--%>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <span class="bcLabelright">Bivac/Cotecna :</span>
                                                            </td>
                                                            <td align="left" colspan="2">
                                                                <asp:CheckBox runat="server" ID="Chkbivac" CssClass="bcCheckBoxList" onclick='chcked("Chkbivac", "Chkcotecna")'
                                                                    ForeColor="#000000" Font-Size="11px" font-family="Arial" />Bivac
                                                                <asp:CheckBox runat="server" ID="Chkcotecna" CssClass="bcCheckBoxList" onclick='chcked("Chkcotecna", "Chkbivac")'
                                                                    ForeColor="#000000" Font-Size="11px" font-family="Arial" />Cotecna
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
                                                        <%--<asp:GridView runat="server" ID="gvpayments" RowStyle-CssClass="bcGridViewRowStyle"
                                                EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                                PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle">
                                            </asp:GridView>--%>
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
                                                        onclick="fnOpen()" class="bcAlink">Additional Terms & Conditions </a></span>
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
            var result = NewFPO_Customer.AddItemListBox();
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
                    var result = NewFPO_Customer.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    //SuccessMessage('Selected Attachment Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                    SuccessMessage('File Deleted Successfully.');
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });


        $('#lnkAdd').click(function () {
            var result = NewFPO_Customer.AddItemListBox();
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
                var result = NewFPO_Customer.PaymentDeleteItem(id);
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
                var result = NewFPO_Customer.PaymentAddItem(RNo, PercAmt, Desc);
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
                var result = NewFPO_Customer.DeleteItem(id);
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
                var result = NewFPO_Customer.RegretItem(id, fenqid);
                if (result.value == true) {

                    $('[id$=divMyMessage]').append('<span class="Success">Item Regretted Successfully.');


                    var result1 = NewFPO_Customer.BindGridView(0);
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
                var result = NewFPO_Customer.AddNewRow(id, sv, PrtNo, spec, Make, Qty, Rate, Units, Rmrks, isnew);
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
                window.location = "NewFPO_Customer.aspx";
            }
            var result = NewFPO_Customer.ChkBoxRptdFPOMode($('[id$=chkbIRO]')[0].checked);
        }
        function FillItemDRP(id) {
            var ddlCat = GetClientID("ddl" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = NewFPO_Customer.FillItemDRP(id, sv);
            var getDivFEItems = GetClientID("divFPOItems").attr("id");
            $('#' + getDivFEItems).html(result.value);
            Expnder();
        }

        function FillUnitDRP(id) {
            var ddlCat = GetClientID("ddlU" + (parseInt(id))).attr("id");
            var sv = $('#' + ddlCat).val();
            var result = NewFPO_Customer.FillUnitDRP(id, sv);
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
                if (($('[id$=ddlRefFPO]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
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
            if ($('#ctl00_ContentPlaceHolder1_Lstfenqy :selected').length == 0) {
                ErrorMessage('Foreign Enquiry number is Required.');
                $('[id$=Lstfenqy]').focus();
                return false;
            }
            else if ($('[id$=txtfenqDt]').val() == '') {
                ErrorMessage('Foreign Enquiry Date is Required.');
                $('[id$=txtfenqDt]').focus();
                return false;
            }
            else if ($('[id$=txtsubject]').val() == '') {
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
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            if (rowCount > 0) {
                for (var i = 1; i <= rowCount; i++) {
                    var qty = parseFloat($('#txtQuantity' + '' + i).val(), null);
                    var Hid = $('#HItmID' + '' + i).val(); //HItmID
                    var isChecked = $('#ckhChaild' + i).is(':checked');
                    if (!isChecked) {
                        var notSelected = notSelected + ',' + Hid;
                        count = count + 1;
                        $('[id$=HselectedItems]').val(notSelected);
                    }
                    if (qty == '' && qty <= 0) {
                        var message = '';
                        if (qty == '')
                            message = 'Quantity is Required';
                        else if (qty <= 0)
                            message = 'Quantity Cannot Be Zero';
                        ErrorMessage('' + message + ' of SNo : ' + i + '.');
                        return false;
                        break;
                    }
                }
                if (count == rowCount) {
                    ErrorMessage('Select Atleast 1 Item.');
                    return false;
                }
            }
            else if (rowCount <= 0) {
                ErrorMessage('No Records to Save.');
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
            else if ($('[id$=txtDlvry]').val() == '' || $('[id$=txtDlvry]').val() == '0') {
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
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();
            window.showModalDialog("../Enquiries/AddItems.aspx", "", sFeatures);
            BindGridView(rowIndex);
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
        function fnOpen(id, rowIndex) {
            var sFeatures = fnSetValues();
            window.showModalDialog("../Masters/TermsNConditions.aspx?TAr=General", "508", sFeatures); //../Enquiries/AddItems.aspx
            //FillItemGrid(id, rowIndex);
        }
        
    </script>
    <script type="text/javascript">
        function CheckFPOOrderNo() {
            var enqNo = $('[id$=txtFpoNo]').val().trim();
            var result = NewFPO_Customer.CheckFPOOrderNo(enqNo);
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
            var result = NewFPO_Customer.CheckItem(IsChecked, ID);
            $('#tblItems').html('');
            $('#tblItems').html(result.value);
            Expnder();
        }
        
    </script>


</asp:Content>
