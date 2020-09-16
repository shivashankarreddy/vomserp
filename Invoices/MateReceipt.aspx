<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="MateReceipt.aspx.cs" Inherits="VOMS_ERP.Invoices.MateReceipt" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Mate Receipt Page "
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
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Proforma Invoice No.<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlPrfmaInvcNo" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required" AutoPostBack="true" OnSelectedIndexChanged="ddlPrfmaInvcNo_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">S/B No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlSBNo" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Date<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Port of Loading<font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlportloading" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Port of Discharge <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList runat="server" ID="ddlportdischarge" TabIndex="2" CssClass="bcAspdropdown"
                                            required="required">
                                            <asp:ListItem Text="--Select--" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Mate Receipt No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtMateReceiptNo" onchange="GetMR()" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Total Packages <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtTotalPackages" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,2,false);"
                                            onkeyup="extractNumber(this,2,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">EM No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtEMNo" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Gross Weight <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtGrossWt" runat="server" CssClass="bcAsptextbox" onblur="extractNumber(this,3,false);"
                                            onkeyup="extractNumber(this,3,false);" onkeypress="return blockNonNumbers(this, event, true, false);"
                                            MaxLength="15"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Liner Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtLinerName" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Vessel Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtVesselName" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">Forwarder Name <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtForwarderName" runat="server" onkeypress="return isSomeSplChrs(event);"
                                            CssClass="bcAsptextbox" MaxLength="150"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <span class="bcLabel">REMARKS :</span>
                                    </td>
                                    <td class="bcTdnormal" style="width: 185px">
                                        <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                            MaxLength="500"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal" colspan="2">
                                        <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" width="47%">
                                                        <span id="Span6" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox runat="server" ID="txtComments" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span14" class="bcLabel">&nbsp; </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr style="background-color: Gray; font-style: normal; color: White;">
                                    <td colspan="6">
                                        Container Details :
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <asp:GridView runat="server" ID="gv_CntrDtls" AutoGenerateColumns="false" Width="100%"
                                            EmptyDataText="No Records To Display...!" OnRowDataBound="gv_CntrDtls_RowDataBound"
                                            OnPreRender="gv_CntrDtls_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No.">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                        <asp:Label ID="lblSerialNo" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Container Number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("CntrNmbr") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Container Type">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblHSCode" runat="server" Text='<%# Eval("CntrTypeDesc") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Container Size">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPartno" runat="server" Text='<%# Eval("CntrSize") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Container Seal No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPartno" runat="server" Text='<%# Eval("CntrSealNmbr") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Container Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("CntrDate") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" class="bcTdNewTable">
                                        <div style="width: 100%">
                                            <ajax:Accordion ID="Accordion3" runat="server" SelectedIndex="1" HeaderCssClass="accordionHeader"
                                                HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                                                FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                                                FramesPerSecond="40" RequireOpenedPane="false">
                                                <Panes>
                                                    <ajax:AccordionPane ID="AccordionPane4" runat="server">
                                                        <Header>
                                                            <a href="#" class="href">Attachments</a> &nbsp;<asp:Image runat="server" ID="imgAtchmt"
                                                                AlternateText="Attachments are Vailable" ImageUrl="~/images/Attach.gif" Visible="false" />
                                                        </Header>
                                                        <Content>
                                                            <asp:Panel ID="Panel4" runat="server" Width="98%">
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
                                    <td colspan="5" align="right">
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
                                                                <asp:LinkButton runat="server" ID="btnclear" Text="Clear" OnClick="btnClear_Click" />
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
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });


        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday

            });
        });

        function isSomeSplChrs(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 32 && charCode != 8 && charCode != 45 && charCode != 47 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122) && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function uploadComplete() {
            var result = MateReceipt.AddItemListBox();
            var getDivGDNItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGDNItems).html(result.value);
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
        }
        function uploadError() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload Failed.";
        }
        function uploadStarted() {
            $get("<%=lblstatus.ClientID%>").innerHTML = "File upload started.";
        }

        $('#lnkdelete').click(function () {
            if ($('#lbItems').val() != "") {
                var result = MateReceipt.DeleteItemListBox($('#lbItems').val());
                var getDivGDNItems = GetClientID("divListBox").attr("id");
                $('#' + getDivGDNItems).html(result.value);
                SuccessMessage('File Deleted Successfully.');
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
               
            }
        });
        $('#lnkAdd').click(function () {
            var result = PrfmaInvoice.AddItemListBox();
            var getDivGDNItems = GetClientID("divListBox").attr("id");
            $('#' + getDivGDNItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
        });


        function Myvalidations() {
            if ($('[id$=ddlPrfmaInvcNo]').val() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Select Proforma Invoice Number');
                $('[id$=ddlPrfmaInvcNo]').focus();
                return false;
            }
            else if (($('[id$=ddlSBNo]').val()) == '0') {
                ErrorMessage('SB No. is required.');
                $('[id$=ddlSBNo]').focus();
                return false;
            }
            else if (($('[id$=txtDate]').val()).trim() == '') {
                ErrorMessage('Mate Date is required.');
                $('[id$=txtDate]').focus();
                return false;
            }
            else if (($('[id$=ddlportloading]').val()) == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Port of Loading is required.');
                $('[id$=ddlportloading]').focus();
                return false;
            }
            else if (($('[id$=ddlportdischarge]').val()) == '0') {
                ErrorMessage('Port of Discharge is required.');
                $('[id$=ddlportdischarge]').focus();
                return false;
            }
            else if (($('[id$=txtMateReceiptNo]').val()).trim() == '') {
                ErrorMessage('Mate Receipt No. is required.');
                $('[id$=txtMateReceiptNo]').focus();
                return false;
            }

            else if (($('[id$=txtTotalPackages]').val()).trim() == '') {
                ErrorMessage('Total Packages is required.');
                $('[id$=txtTotalPackages]').focus();
                return false;
            }
            else if (($('[id$=txtEMNo]').val()).trim() == '') {
                ErrorMessage('EM No. is required.');
                $('[id$=txtEMNo]').focus();
                return false;
            }
            else if (($('[id$=txtGrossWt]').val()).trim() == '') {
                ErrorMessage('Gross weight is required.');
                $('[id$=txtGrossWt]').focus();
                return false;
            }
            else if (($('[id$=txtLinerName]').val()).trim() == '') {
                ErrorMessage('Liner Name is required.');
                $('[id$=txtLinerName]').focus();
                return false;
            }
            else if (($('[id$=txtVesselName]').val()).trim() == '') {
                ErrorMessage('Vessel Name is required.');
                $('[id$=txtVesselName]').focus();
                return false;
            }
            else if (($('[id$=txtForwarderName]').val()).trim() == '') {
                ErrorMessage('Forwarder Name is required.');
                $('[id$=txtForwarderName]').focus();
                return false;
            }
            else if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    ErrorMessage('Comment is Required.');
                    $('[id$=txtComments]').focus();
                    return false;
                }
            }
            else
                return true;
        }

        function GetMR() {
            var enqNo = $('[id$=txtMateReceiptNo]').val();
            var result = MateReceipt.GetMR(enqNo);
            if (result.value == false) {
                $('[id$=txtMateReceiptNo]').val("");
                ErrorMessage('Matereceipt Number Exists.');
                $('[id$=txtMateReceiptNo]').focus();
            }
        }

    </script>
</asp:Content>
