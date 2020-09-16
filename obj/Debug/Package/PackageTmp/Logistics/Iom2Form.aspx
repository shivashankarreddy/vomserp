<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Iom2Form.aspx.cs" Inherits="VOMS_ERP.Logistics.Iom2Form"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="IOM(Inter Office Memo) Template for Logistics"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span15" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span13" style="float: left !important" class="bcLabelright">IS Bivac/Cotecna</span>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CHkBivac" runat="server" TabIndex="2" onchange="ShowBivac();" AutoPostBack="True"
                                            OnCheckedChanged="CHkBivac_CheckedChanged" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" style="float: left !important" class="bcLabelright">Bivac Shipment
                                            Planning No.</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlBivacShpmntPlnngNo" CssClass="bcAspdropdown"
                                            Enabled="false" AutoPostBack="True" TabIndex="4" OnSelectedIndexChanged="ddlRefno_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="lblRctdSctg" class="bcLabel">Shipment Planning No. <font color="red" size="2">
                                            <b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlRefno" AutoPostBack="true" CssClass="bcAspdropdown"
                                            OnSelectedIndexChanged="ddlRefno_SelectedIndexChanged">
                                            <asp:ListItem Text="Select Ref No." Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRcd" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Ref. No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRefno" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabel">To Address <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtTAdr" onblur="validateEmail(this);" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabel">From Address <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFAdr" onblur="validateEmail(this);" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Password <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPsd" TextMode="Password" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabel">Proforma Invoice No. <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtPrfmatInvc" onchange="GetProformaInvNo()" CssClass="bcAsptextbox"></asp:TextBox>
                                        <asp:HiddenField ID="hdfPrfmInvcID" runat="server" />
                                        <asp:HiddenField ID="hdfPrfmInvcDt" runat="server" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Packing List No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPkngLst" CssClass="bcAsptextbox"></asp:TextBox>
                                        <asp:HiddenField ID="hdfPkngLstID" runat="server" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Subject <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSbjt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span34" class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>
                                            :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" Enabled="false"
                                            CssClass="bcAspMultiSelectListBox"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Mode Of Shipment :</span>
                                    </td>
                                    <td align="left" colspan="3">
                                        <asp:DropDownList runat="server" ID="ddlByAir_Sea" CssClass="bcAspdropdown">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                <td class="bcTdnormal">
                                        <span id="Span16" class="bcLabel">Status <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                <td align="left" colspan="3">
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="bcAspdropdown" 
                                            onselectedindexchanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="Select Status" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                            <asp:ListItem Text="Created" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Accepted" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Rejected" Value="4"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabel">Vessel/Flight <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtVessel" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">BL.No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtBlno" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span12" class="bcLabel">SB.NO.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSbno" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="45%">
                                                        <span id="Span10" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="Left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Width="184px" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DvShpmntInvc" runat="server" style="width: 100%; padding-left: 0px;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="left" width="42%">
                                                        <span id="Span7" class="bcLabel">Shipment Invoice No. <font color="red" size="2"><b>
                                                            *</b></font>:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="txtSpmntInvcNo" onkeypress="return isSomeSplChar(event)"
                                                            ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox> <%--onchange="javascript:return CheckSHPINVNo();"--%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DvShpmntInvcDt" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right">
                                                        <span id="Span8" class="bcLabel">Shipment Invoice Date <font color="red" size="2"><b>
                                                            *</b></font>:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="txtSpmntInvcDt" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Message Body <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left" colspan="5">
                                        <asp:TextBox runat="server" ID="txtBdy" TextMode="MultiLine" Columns="5" Rows="50"
                                            Width="96%" Height="100px" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
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
                                                            <td colspan="5">
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
                                                            <td width="5%">
                                                            </td>
                                                            <td valign="top">
                                                                <div id="divOpen_attachments" runat="server">
                                                                </div>
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
                        <td align="center" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Send" OnClick="btnSend_Click" />
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function ShowBivac() {
            if ($('[id$=CHkBivac]')[0].checked) {
                $('[id$=ddlBivacShpmntPlnngNo]').removeAttr('disabled', 'disabled');
                $('[id$=ddlRefno]').attr('disabled', 'disabled');
            }
            else {
                $('[id$=ddlBivacShpmntPlnngNo]').attr('disabled', 'disabled');
                $('[id$=ddlRefno]').removeAttr('disabled', 'disabled');
                //ClrInputs();
            }
        }


        //        function ClrInputs() {
        //            try {
        //                $('[id$=ddlBivacShpmntPlnngNo]').val('0');
        //                $('[id$=ddlCstmr]').val('0');
        //                $('[id$=lbfpos]').empty();
        //                $('[id$=txtPrfmInvce]').val('');
        //                $('[id$=txtPIDate]').val('');
        //                $('[id$=txtOtrRfs]').val('');
        //                $('[id$=ddlNtfy]').val('0');
        //                $('[id$=ddlPlcOrgGds]').val('0');
        //                $('[id$=ddlPlcFnlDstn]').val('0');
        //                $('[id$=ddlPrtLdng]').val('0');
        //                $('[id$=ddlPrtDscrg]').val('0');
        //                $('[id$=ddlPlcDlry]').val('0');
        //                $('[id$=txtPCrBy]').val('');
        //                $('[id$=txtPlcRcptPCr]').val('');
        //                $('[id$=txtVslFlt]').val('');
        //                $('[id$=txtTrmDlryPmnt]').val('');
        //                $('[id$=txtComments]').val('');
        //            }
        //            catch (Error) {
        //                ErrorMessage(Error.message);
        //            }
        //        }

        function GetProformaInvNo() {
            var refNo = $('[id$=txtpmIn]').val();
            var result = Iom2Form.GetProformaInvNo(refNo);
            if (result.value == false) {
                $('[id$=txtpmIn]').val('');
                $('[id$=txtpmIn]').focus();
                ErrorMessage('This Proforma Invoice No. is already in Use');
            }
        }

        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
            var dateToday = new Date();
            $('[id$=txtRcd]').datepicker({
                dateFormat: 'dd-mm-yy',
                maxDate: dateToday,
                minDate: dateToday
            });
            $('[id$=txtpIdt]').datepicker({
                maxDate: dateToday,
                minDate: -31,
                dateFormat: 'dd-mm-yy'
            });
            $('[id$=txtSpmntInvcDt]').datepicker({
                dateFormat: 'dd-mm-yy'
                //                maxDate: dateToday,
                //                minDate: dateToday
            });

            function ErrorMsging() {
                alert('A');
                ErrorMessage('Check List doesnt have P.Invoice or Packing List.');
            }
        });

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function CheckSHPINVNo() {
            var enqNo = $('[id$=txtSpmntInvcNo]').val();
            var result = Iom2Form.CheckSHPINVNo(enqNo);
            if (result.value == false) {
                $("#<%=txtSpmntInvcNo.ClientID%>")[0].value = '';
                ErrorMessage('Shippment Invoice No. Number Exists.');
                $("#<%=txtSpmntInvcNo.ClientID%>")[0].focus();
                return false;
            }
            else
                return true;
        }

        function isSomeSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) &&
            (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
        // E-Mail Validation
        function validateEmail(emailField) {
            var reg = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            if (emailField.value == '') {
                return true;
            }
            else if (reg.test(emailField.value) == false) {
                emailField.value = '';
                emailField.focus();
                alert('Invalid Email-ID');
                return false;
            }
            return true;
        }

        function uploadComplete() {
            var result = Iom2Form.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivCEDItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                $get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
            }
            /* Clear Content */
            var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
            var txts = AsyncFileUpload.getElementsByTagName("input");
            for (var i = 0; i < txts.length; i++) {
                txts[i].value = "";
                txts[i].style.backgroundColor = "transparent";
            }
            attachmnts_ReLoad();
        }
        function uploadError() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
        }
        function uploadStarted() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != "") {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = Iom2Form.DeleteItemListBox($('#lbItems').val());
                    var getDivCEDItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivCEDItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            //attachmnts_ReLoad();
            else
                ErrorMessage('Select an attachment to delete...?');
        });


        $('#lnkAdd').click(function () {
            //if ($('#lbItems').val() != "") {
            var result = Iom2Form.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivCEDItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
            attachmnts_ReLoad();
        });

        function attachmnts_ReLoad() {
            var result = Iom2Form.attachmnts_ReLoad();
            var getDivCEDItems = GetClientID("divOpen_attachments").attr("id");
            $('#' + getDivCEDItems).html(result.value);
        }

        function Myvalidations() {
          //ddlRefno
            if (($('[id$=ddlRefno]').val()).trim() == '00000000-0000-0000-0000-000000000000' && $('[id$=CHkBivac]')[0].checked == false) {
                ErrorMessage('Shipment Planning Number is Required.');
                $('[id$=ddlRefno]').focus();
                return false;
            }
            else if (($('[id$=ddlBivacShpmntPlnngNo]').val()).trim() == '00000000-0000-0000-0000-000000000000' && $('[id$=CHkBivac]')[0].checked == true) {
                ErrorMessage('Bivac Shipment Planning Number is Required.');
                $('[id$=ddlBivacShpmntPlnngNo]').focus();
                return false;
            }
             else if (($('[id$=txtRcd]').val()).trim() == '') {
                ErrorMessage('Date is Required.');
                $('[id$=txtRcd]').focus();
                return false;
            } 
            else if (($('[id$=txtTAdr]').val()).trim() == '') {
                ErrorMessage('To Address is Required.');
                $('[id$=txtTAdr]').focus();
                return false;
            }
            else if (($('[id$=txtFAdr]').val()).trim() == '') {
                ErrorMessage('From Address is Required.');
                $('[id$=txtFAdr]').focus();
                return false;
            }
            if (($('[id$=txtPsd]').val()).trim() == '') {
                ErrorMessage('Password is Required.');
                $('[id$=txtPsd]').focus();
                return false;
            }
            else if (($('[id$=txtPrfmatInvc]').val()).trim() == '') {
                ErrorMessage('Proforma Invoice Number is Required.');
                $('[id$=txtPrfmatInvc]').focus();
                return false;
            }
            else if (($('[id$=txtPkngLst]').val()).trim() == '' && $('[id$=CHkBivac]')[0].checked == false) {
                ErrorMessage('Packing List Number is Required.');
                $('[id$=txtPkngLst]').focus();
                return false;
            }
            else if (($('[id$=txtSbjt]').val()).trim() == '') {
                ErrorMessage('Subject is Required.');
                $('[id$=txtSbjt]').focus();
                return false;
            }
            else if (($('[id$=ddlStatus]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Status is Required.');
                $('[id$=ddlStatus]').focus();
                return false;
            }
            else if ($('[id$=lbfpos]').val() == null) {
                ErrorMessage('Frn PO Number is Required.');
                $('[id$=lbfpos]').focus();
                return false;
            }
            else if ($('[id$=lblpos]').val() == null && $('[id$=CHkBivac]')[0].checked == false) {
                ErrorMessage('Lcl PO Number is Required.');
                $('[id$=lblpos]').focus();
                return false;
            }
            if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            if ($('[id$=DvShpmntInvc]').css("visibility") == "visible") {
                if (!($('[id$=txtSpmntInvcNo]').is(':disabled'))) {
                    if (($('[id$=txtSpmntInvcNo]').val()).trim() == '') {
                        ErrorMessage('Shipment Invoice Number is Required.');
                        $('[id$=txtSpmntInvcNo]').focus();
                        return false;
                    }
                }
            }
            if ($('[id$=DvShpmntInvcDt]').css("visibility") == "visible") {
                if (($('[id$=txtSpmntInvcDt]').val()).trim() == '') {
                    ErrorMessage('Shipment Invoice Date is Required.');
                    $('[id$=txtSpmntInvcDt]').focus();
                    return false;
                }
            }
            //            else if (($('[id$=txtVessel]').val()).trim() == '') {
            //                ErrorMessage('Vessel/Flight is Required.');
            //                $('[id$=txtVessel]').focus();

            //                return false;
            //            } else if (($('[id$=txtBlno]').val()).trim() == '') {
            //                ErrorMessage('BL.No is Required.');
            //                $('[id$=txtBlno]').focus();

            //                return false;
            //            } else if (($('[id$=txtSbno]').val()).trim() == '') {
            //                ErrorMessage('SB.No is Required.');
            //                $('[id$=txtSbno]').focus();

            //                return false;
            //            }
            else if (($('[id$=txtBdy]').val()).trim() == '') {
                ErrorMessage('Message Body is Required.');
                $('[id$=txtBdy]').focus();

                return false;
            }
            else {
                return true;
            }
        }        
    </script>
</asp:Content>
