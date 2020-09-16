<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DespatchInstructions.aspx.cs" Inherits="VOMS_ERP.Logistics.DespatchInstructions"
     %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Dispatch Instructions"
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
                                        <asp:DropDownList runat="server" ID="ddlCstmr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlCstmr_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="--Select--" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Supplier Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlSuplr" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlSuplr_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="--Select--" Value="00000000-0000-0000-0000-000000000000"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">Ref. No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRefno" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Foreign PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:ListBox runat="server" ID="lbfpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            AutoPostBack="true" OnSelectedIndexChanged="lbfpos_SelectedIndexChanged"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:ListBox runat="server" ID="lblpos" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox"
                                            AutoPostBack="true" OnSelectedIndexChanged="lblpos_SelectedIndexChanged"></asp:ListBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">CT-1 Ref No.:</span>
                                    </td>
                                    <td align="left">
                                        <%--    <asp:DropDownList runat="server" ID="ddlCtoRn" CssClass="bcAspdropdown" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCtoRn_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Select CT-1 Ref No."></asp:ListItem>
                                        </asp:DropDownList>--%>
                                        <asp:ListBox runat="server" ID="lbCtoRn" SelectionMode="Multiple" CssClass="bcAspMultiSelectListBox">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">No. of ARE-1 Form Sets :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAREOFS" onkeypress="return isNumberKey(event)"
                                            MaxLength="2" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Subject <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSbjt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <span id="Span8" class="bcLabel">Shipping Address <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="Left">
                                        <asp:TextBox runat="server" ID="txtSpngAdr" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Contact Person <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtCnctPrsn" onkeypress="return isAlphabet(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="lblRctdSctg" class="bcLabel">Contact No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCnctNo" onkeypress="return isNumberKey(event)"
                                            MaxLength="15" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Alternate Contact Person :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtAltCnctPrsn" onkeypress="return isAlphabet(event)"
                                            CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Alternate Contact No. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtAltCnctNo" onkeypress="return isNumberKey(event)"
                                            MaxLength="15" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="48%">
                                                        <span id="Span10" class="bcLabel">Comments <font color="red" size="2"><b>*</b></font>:</span>
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
                            &nbsp;&nbsp;&nbsp;LPO Items
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td>
                                        <div style="width: 100%; min-height:150px; max-height: 350px; overflow: scroll">
                                            <asp:GridView runat="server" ID="gvLPOItems" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                                AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                                PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                                EmptyDataText="No Records are Exists" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                                AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Item Description" DataField="ItemDescription" />
                                                    <asp:BoundField HeaderText="PartNumber" DataField="PartNumber" />
                                                    <asp:BoundField HeaderText="Specifications" DataField="Specifications" />
                                                    <asp:BoundField HeaderText="Make" DataField="Make" />
                                                    <asp:BoundField HeaderText="Quantity" DataField="Quantity" />
                                                    <asp:BoundField HeaderText="Units" DataField="UnitName" />
                                                    <asp:BoundField HeaderText="Rate" DataField="Rate" />
                                                    <asp:BoundField HeaderText="Amount" DataField="Amount" />
                                                </Columns>
                                            </asp:GridView>
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });
        function uploadComplete() {
            var result = DespatchInstructions.AddItemListBox();
            var getDivDIItems = GetClientID("divListBox").attr("id");
            $('#' + getDivDIItems).html(result.value);
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
                if (confirm("Are you sure you want to delete the item")) {
                    var result = DespatchInstructions.DeleteItemListBox($('#lbItems').val());
                    var getDivDIItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivDIItems).html(result.value);
                    SuccessMessage('File Deleted Successfully.');
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });


        $('#lnkAdd').click(function () {
            //if ($('#lbItems').val() != "") {
            var result = DespatchInstructions.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivDIItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            // }
        });

        function noSplChar(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 40 && charCode != 41 && charCode != 44 && charCode != 45 && charCode != 46 && charCode != 47 && charCode != 58 && (charCode < 48 || charCode > 57) && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function isAlphabet(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122))
                return false;
            return true;
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) //charCode != 46
                return false;
            return true;
        }
        function Myvalidations() {

            if (($('[id$=ddlCstmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Customer is Required.');
                $('[id$=ddlCstmr]').focus();
                return false;
            }
            else if (($('[id$=ddlSuplr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Supplier is Required.');
                $('[id$=ddlSuplr]').focus();
                return false;
            }
            else if ($('[id$=lbfpos]').val() == null) {
                ErrorMessage('Foreign PO(s) is Required.');
                $('[id$=lbfpos]').focus();
                return false;
            }
            else if ($('[id$=lblpos]').val() == null) {
                ErrorMessage('Local PO(s) is Required.');
                $('[id$=lblpos]').focus();
                return false;
            }
            //            else if (($('[id$=txtAREOFS]').val()).trim() == '') {
            //                ErrorMessage('Number of ARE-1 Form Sets is Required. ');
            //                $('[id$=txtAREOFS]').focus();
            //                return false;
            //            }
            else if (($('[id$=txtSbjt]').val()).trim() == '') {//txtAREOFS
                ErrorMessage('Subject is Required.');
                $('[id$=txtSbjt]').focus();
                return false;
            }
            else if (($('[id$=txtSpngAdr]').val()).trim() == '') {
                ErrorMessage('Shipping Address is Required. ');
                $('[id$=txtSpngAdr]').focus();
                return false;
            }
            else if (($('[id$=txtCnctPrsn]').val()).trim() == '') {
                ErrorMessage('Contact Person is Required.');
                $('[id$=txtCnctPrsn]').focus();
                return false;
            }
            else if (($('[id$=txtCnctNo]').val()).trim() == '') {
                ErrorMessage('Contact Number is Required.');
                $('[id$=txtCnctNo]').focus();
                return false;
            }
            else if (($('[id$=txtCnctNo]').val()).trim() == ($('[id$=txtAltCnctNo]').val()).trim()) {
                ErrorMessage('Contact Number and Alternate Contact Number can not be same.');
                $('[id$=txtAltCnctNo]').focus();
                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            else {
                return true;
            }
        }  
    </script>
</asp:Content>
