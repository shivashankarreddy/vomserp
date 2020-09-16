<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="InspectionPlan.aspx.cs" Inherits="VOMS_ERP.Purchases.InspectionPlan" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Inspection Plan" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span12" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <span id="Span6" class="bcLabel">Plan Req. No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlReqNo" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlReqNo_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select Request No"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Reference No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtReferenceNo" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Inspection Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInspectionDate" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtCstmr" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSuplr" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Stage of Inspection <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Stage" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Stage 1" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Stage 2" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Stage 3" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Stage 4" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Stage 5" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="Final Stage" Value="6"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" colspan="3">
                                        <asp:ListBox runat="server" ID="lblpos" Style="width: 580px;" SelectionMode="Multiple"
                                            CssClass="bcAspMultiSelectListBox"></asp:ListBox>
                                    </td>
                                </tr>
                                <%--New row begin--%>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Place of Inspection <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtInsplace" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Contact Person <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtPerson" MaxLength="12" onkeypress="return isSomeSplChar(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Contact Number :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtContactNo" onkeypress="return isNumberKey(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Third Party Inspector :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlThirdParty" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Third Party Inspector"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span11" class="bcLabel">Self Inspector :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlSelf" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Self Inspector" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="43%">
                                                        <span id="Span10" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="Left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <%--New Row end--%>
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
                        <td align="center" colspan="6">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Save" OnClick="btnSend_Click" />
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
     <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            Expnder();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }


        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
            var dateToday = new Date();
            $('[id$=txtInspectionDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

        });

        function isSomeSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) &&
            (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false; //charCode != 32 &&
            return true;
        }

        function uploadComplete() {
            var result = InspectionPlan.AddItemListBox();
            var getDivIPItems = GetClientID("divListBox").attr("id");
            $('#' + getDivIPItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
                var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
                var txts = AsyncFileUpload.getElementsByTagName("input");
                for (var i = 0; i < txts.length; i++) {
                    txts[i].value = "";
                    txts[i].style.backgroundColor = "transparent";
                }
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                //$get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
                SuccessMessage('File Uploaded Successfully.');
                /* Clear Content */
                var AsyncFileUpload = $("#<%=AsyncFileUpload1.ClientID%>")[0];
                var txts = AsyncFileUpload.getElementsByTagName("input");
                for (var i = 0; i < txts.length; i++) {
                    txts[i].value = "";
                    txts[i].style.backgroundColor = "transparent";
                }
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
            if ($('#lbItems').val() != null) {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = InspectionPlan.DeleteItemListBox($('#lbItems').val());
                    var getDivIPItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivIPItems).html(result.value);
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
            //if ($('#lbItems').val() != "") {
            var result = InspectionPlan.AddItemListBox();
            var getDivIPItems = GetClientID("divListBox").attr("id");
            $('#' + getDivIPItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });
        function Myvalidations() {

            if (($('[id$=ddlReqNo]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Plan Request Number is Required.</span>');
                $('[id$=ddlReqNo]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtInspectionDate]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Inspection Date is Required.</span>');
                $('[id$=txtInspectionDate]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtCstmr]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer Name is Required.</span>');
                $('[id$=txtCstmr]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtSuplr]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Supplier Name is Required.</span>');
                $('[id$=txtSuplr]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlStatus]').val()).trim() == '0') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Stage of Inspection is Required.</span>');
                $('[id$=ddlStatus]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=lbfpos]').val() == null) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Foreign PO(s) is Required.</span>');
                $('[id$=lbfpos]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=lblpos]').val() == null) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Local PO(s) is Required.</span>');
                $('[id$=lblpos]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtInsplace]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Place of Inspection is Required.</span>');
                $('[id$=txtInsplace]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtPerson]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Contact Person is Required.</span>');
                $('[id$=txtPerson]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ((($('[id$=ddlThirdParty]').val()).trim() == '0') && (($('[id$=ddlSelf]').val()).trim() == '0')) {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Inspector is Required.</span>');
                $('[id$=ddlThirdParty]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    $("#<%=divMyMessage.ClientID %> span").remove();
                    $('[id$=divMyMessage]').append('<span class="Error">Comment is Required.</span>');
                    $('[id$=txtComments]').focus();
                    $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                    return false;
                }
            }
            else {
                return true;
            }
        }


        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //charCode != 46 &&
                return false;
            return true;
        }
        
        
        
    </script>
</asp:Content>
