<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="RqstCEDtls.aspx.cs" Inherits="VOMS_ERP.Logistics.RqstCEDtls"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Request for Proforma Invoice"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
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
                                        <span class="bcLabel">Customer Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlcustmr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="Select Customer"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblRctdSctg" class="bcLabel">Supplier Category :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsuplrctgry" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlsuplrctgry_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select Category"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsuplr" AutoPostBack="true" OnSelectedIndexChanged="ddlsuplr_SelectedIndexChanged"
                                            CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Supplier"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            OnSelectedIndexChanged="lbfpos_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                        </asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Ref. No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRefno" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="40%">
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
                                                                    ThrobberID="ThrobberImg" OnUploadedComplete="FileUploadComplete" CssClass="FileUploadClass"/>
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
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Request" OnClick="btnSend_Click" />
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
        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });


        function uploadComplete() {
            var result = RqstCEDtls.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivCEDItems).html(result.value);
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
            //alert($('#lbItems').val());
            if ($('#lbItems').val() != null) //undefined
            {
                var IsDelete = confirm("Are you sure you want to delete selected Attachment...?")
                if (IsDelete) {
                    var result = RqstCEDtls.DeleteItemListBox($('#lbItems').val());
                    var getDivCEDItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivCEDItems).html(result.value);
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
            Expnder();
        });


        $('#lnkAdd').click(function() {
            //if ($('#lbItems').val() != "") {
            var result = RqstCEDtls.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivCEDItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });
        function Myvalidations() {
            if (($('[id$=ddlcustmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Customer Name is Required.</span>');
                $('[id$=ddlcustmr]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlsuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Supplier Name is Required.</span>');
                $('[id$=ddlsuplr]').focus();
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
