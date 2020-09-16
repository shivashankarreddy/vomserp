<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Sevottam.aspx.cs" Inherits="VOMS_ERP.Logistics.Sevottam" EnableEventValidation="false"  %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <div id="divMessage">
                </div>
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Sevottam" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <%--<td style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>--%>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;" class="bcTdnormal">
                            <div style="border: 0px solid #9CB5CB; float: left; background: #ECEFF5; padding: 5px;
                                width: 98%; margin: 5px; height: 99%;">
                                <div class="row" style="text-align: center; width: 80%;">
                                    <asp:Panel ID="pnlActions" runat="server" Width="100%">
                                        <div style="text-align: left; width: 15%;">
                                            <span>
                                                <asp:RadioButton runat="server" ID="rbgNew" GroupName="sev" Text="New" AutoPostBack="true"
                                                    onclick="javascript:void(0)" OnCheckedChanged="rbgNew_CheckedChanged" />
                                            </span>
                                        </div>
                                        <div style="text-align: left; width: 15%;">
                                            <span>
                                                <asp:RadioButton runat="server" ID="rbgCancel" GroupName="sev" Text="Cancel" onclick="javascript:void(0)"
                                                    AutoPostBack="true" OnCheckedChanged="rbgCancel_CheckedChanged" />
                                            </span>
                                        </div>
                                        <div style="text-align: left; width: 15%;">
                                            <span>
                                                <asp:RadioButton runat="server" ID="rbgPOE" GroupName="sev" AutoPostBack="true" Text="POE/Un-Utilized"
                                                    onclick="javascript:void(0)" OnCheckedChanged="rbgUnUtilize_CheckedChanged" />
                                            </span>
                                        </div>
                                        <%--<div style="text-align: left; width: 15%;">
                                            <span>
                                                <asp:RadioButton runat="server" ID="rbgUnUtilize" GroupName="sev" Text="Un-Utilized"
                                                    onclick="javascript:void(0)" AutoPostBack="True" OnCheckedChanged="rbgUnUtilize_CheckedChanged" />
                                            </span>
                                        </div>--%>
                                    </asp:Panel>
                                </div>
                                <div class="row" style="text-align: left; width: -1%;">
                                    <div style="text-align: left; width: 36%;">
                                        <span id="Span9" class="bcLabelright">Sevottam Reference No.
                                            <asp:Label ID="lblStatusStar" runat="server" Text="" Visible="false" Font-Bold="True"
                                                ForeColor="Red"></asp:Label>
                                            :</span>
                                    </div>
                                    <div style="text-align: left; width: 15%;">
                                        <span>
                                            <asp:TextBox runat="server" ID="txtReferenceNo" onchange="GetSevRefNo()" CssClass="bcAsptextbox"
                                                Enabled="false"></asp:TextBox>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <span><asp:Label ID="lblError" runat="server" Text="" Visible="false"/></span>
                            <div id="dvSevottam" runat="server" visible="false">
                                <asp:GridView runat="server" ID="gvSevottam" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                    EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                    PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                    AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" 
                                    ShowFooter="True" FooterStyle-CssClass="bcGridViewHeaderStyle" OnRowDataBound="gvSevottam_RowDataBound">
                                    <PagerStyle HorizontalAlign="Center" CssClass="bcGridViewPagerStyle"></PagerStyle>
                                    <HeaderStyle CssClass="bcGridViewHeaderStyle"></HeaderStyle>
                                    <AlternatingRowStyle CssClass="bcGridViewAlternatingRowStyle"></AlternatingRowStyle>
                                    <Columns>
                                        <%--<asp:TemplateField HeaderText="S.No." HeaderStyle-Width="10px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkCTID" runat="server" OnCheckedChanged="chkCTID_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CT-1 Draft Number">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblCTNo" Text='<%#Eval("CT1DraftRefNo") %>'></asp:Label>
                                                <asp:HiddenField ID="HFCT1ID" runat="server" Value='<%#Eval("CT1ID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Supplier">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblSupplier" Text='<%#Eval("SupplierNm") %>'></asp:Label>
                                                <asp:HiddenField ID="HFEditCheck" runat="server" Value='<%#Eval("Check") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="FPO">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblFPO" Text='<%#Eval("FPOs") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="LPO">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblLPO" Text='<%#Eval("LPOs") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <b><asp:Label runat="server" ID="lblTotAmount" Text="Total Amount : "></asp:Label></b>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CT-1 Value">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblCTValue" Text='<%#Eval("CT1BondValue") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotalVal" runat="server" Font-Bold="true" Text="0.00"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <%--<EmptyDataRowStyle CssClass="bcGridViewEmptyDataRowStyle"></EmptyDataRowStyle>--%>
                                    <RowStyle VerticalAlign="Bottom" CssClass="bcGridViewRowStyle"></RowStyle>
                                </asp:GridView>
                            </div>
                            <div id="dvSevottamPOE" runat="server" visible="false">
                                <asp:GridView runat="server" ID="gvSevottamPOE" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                                    EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                                    PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                    AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%" 
                                    ShowFooter="True" FooterStyle-CssClass="bcGridViewHeaderStyle" OnRowDataBound="gvSevottamPOE_RowDataBound">
                                    <PagerStyle HorizontalAlign="Center" CssClass="bcGridViewPagerStyle"></PagerStyle>
                                    <HeaderStyle CssClass="bcGridViewHeaderStyle"></HeaderStyle>
                                    <AlternatingRowStyle CssClass="bcGridViewAlternatingRowStyle"></AlternatingRowStyle>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkCTID" runat="server" OnCheckedChanged="chkCTID_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CT-1 Number">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblCTNo" Text='<%#Eval("CT1ReferenceNo") %>'></asp:Label>
                                                <asp:HiddenField ID="HFCT1ID" runat="server" Value='<%#Eval("CT1ID") %>' />
                                                <asp:HiddenField ID="HFCTTrackingID" runat="server" Value='<%#Eval("CTpID") %>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <b>Total Amounts : </b>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblctdate" Text='<%#Eval("CTDate") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CT-1 Value">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblCTValue" Text='<%#Eval("CT1BondValue") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <%--<asp:Label ID="lblCTTotalVal" runat="server" Font-Bold="true" Text="0.00"></asp:Label>--%>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ARE-1 Number">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblareNo" Text='<%#Eval("ARE1No") %>'></asp:Label>
                                                <asp:HiddenField ID="HFARE1ID" runat="server" Value='<%#Eval("ARE1ID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblaredate" Text='<%#Eval("AREDate") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ARE-1 Value">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblareval" Text='<%#Eval("ARE1Value") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblARETotalVal" runat="server" Font-Bold="true" Text="0.00"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Un-Utilized">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblUnUtlzd" Text='<%#Eval("UnUtilized") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblUnUtlsdTotalVal" runat="server" Font-Bold="true" Text="0.00"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Supplier">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblSupplier" Text='<%#Eval("SupplierNm") %>'></asp:Label>
                                                <asp:HiddenField ID="HFSUPLRID" runat="server" Value='<%#Eval("SupplierID") %>' />
                                                <asp:HiddenField ID="HFEditCheck" runat="server" Value='<%#Eval("Check") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ECC No.">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblEccno" Text='<%#Eval("ECCNo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sevottam ID" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblSvtmID" Text='<%#Eval("SvtID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <%--<EmptyDataRowStyle CssClass="bcGridViewEmptyDataRowStyle"></EmptyDataRowStyle>--%>
                                    <RowStyle VerticalAlign="Bottom" CssClass="bcGridViewRowStyle"></RowStyle>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <asp:GridView runat="server" ID="gvExportWorkSheet" AutoGenerateColumns="false" RowStyle-CssClass="bcGridViewRowStyle"
                            EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle" PagerStyle-CssClass="bcGridViewPagerStyle"
                            PagerStyle-HorizontalAlign="Center" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" Width="100%"
                            DataKeyNames="CT1ID" Caption="WORK SHEET FOR  ADMISSION OF PROOF OF EXPORTS" Visible="false"
                            OnRowDataBound="gvExportWorkSheet_RowDataBound" OnPreRender="gvExportWorkSheet_PreRender">
                            <Columns>
                                <asp:TemplateField HeaderText="S.No.">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CT1 No/ DATE">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCt1Nmbr" runat="server" Text='<%# Eval("CT1Number") %>'></asp:Label><br />
                                        <asp:Label ID="lblCt1Date" runat="server" Text='<%# Eval("CT1Date") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount of Duty" ItemStyle-HorizontalAlign="right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCt1Value" runat="server" Text='<%# Eval("CT1Value") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ARE1 No/ Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblARE1Nmbr" runat="server" Text='<%# Eval("ARE1Number") %>'></asp:Label><br />
                                        <asp:Label ID="lblARe1Date" runat="server" Text='<%# Eval("ARE1Date") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Duty involved" ItemStyle-HorizontalAlign="right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblARE1Value" runat="server" Text='<%# Eval("ARE1Value") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Un-utilised Amount" ItemStyle-HorizontalAlign="right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUnUsedValue" runat="server" Text='<%# Eval("UnUsedAmt") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Triplicate Copy Recd" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTCRValue" runat="server" Text='<%# Eval("ARE1Forms") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="S.B No/Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSBNmbr" runat="server" Text='<%# Eval("ShpngBilNmbr") %>'></asp:Label><br />
                                        <asp:Label ID="lblSBDate" runat="server" Text='<%# Eval("ShpngBilDate") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="AWB/BL No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAWBL" runat="server" Text='<%# Eval("AWBNumber") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date of Export">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDtExpt" runat="server" Text='<%# Eval("DateofShpmnt") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>

                    <tr>
                        <td class="bcTdNewTable">
                            <div id="DivAttachments" runat="server" style="width: 100%" visible="false">
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
                                                                <ajax:AsyncFileUpload ID="AsyncFileUpload12" runat="server" OnClientUploadError="uploadError"
                                                                    OnClientUploadComplete="uploadComplete" OnClientUploadStarted="uploadStarted"
                                                                    UploaderStyle="Modern" CompleteBackColor="LightGreen" UploadingBackColor="Yellow"
                                                                    ThrobberID="ThrobberImg" OnUploadedComplete="FileUploadComplete" CssClass="FileUploadClass" />
                                                                <asp:Image runat="server" ID="ThrobberImg" ImageUrl="~/images/uploadingImage.gif"
                                                                    AlternateText="loading" />
                                                            </td>
                                                            <td valign="top">
                                                                <div id="divListBox1" runat="server" width="221px">
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
                        <td class="bcTdnormal">
                            <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                <table>
                                    <tr>
                                        <td align="right" style="width: 39%">
                                            <span id="Span2" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
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
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSend" Text="Generate" OnClick="btnSend_Click" />
                                            </div>                                            
                                        </td>
                                        <td class="bcTdButton">
                                            <div id="dvGnrtWrksht" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnExportWorkSht" Text="WorkSheet" 
                                                    onclick="btnExportWorkSht_Click" Visible="false"> </asp:LinkButton>
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
        function uploadComplete() {
            var result = Sevottam.AddItemListBox();
            var getDivLEItems = GetClientID("divListBox1").attr("id");
            $('#' + getDivLEItems).html(result.value);
            var listid = GetClientID("lbItems").attr("id");
            $('#' + listid)[0].selectedIndex = '0';
            $get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
            /* Clear Content */
            var AsyncFileUpload = $("#<%=AsyncFileUpload12.ClientID%>")[0];
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

        $('[id$=lnkdelete]').click(function () {
            if ($('[id$=lbItems]').val() != "") {
                if (confirm("Are you sure you want to delete selected Attachment...?")) {
                    var result = Sevottam.DeleteItemListBox($('#lbItems').val());
                    var getDivFEItems = GetClientID("divListBox1").attr("id");
                    $('#' + getDivFEItems).html(result.value);
                    var listid = GetClientID("lbItems").attr("id");
                    $('#' + listid)[0].selectedIndex = '0';
                }
            }
            else
                ErrorMessage('Select an attachment to delete...?');
        });

        function GetSevRefNo() {
            var refNo = $('[id$=txtReferenceNo]').val();
            var result = Sevottam.GetSevRefNo(refNo);
            if (result.value == false) {
                $('[id$=txtReferenceNo]').val('');
                $('[id$=txtReferenceNo]').focus();
                ErrorMessage('This reference No. is already in Use');
            }
        }
    </script>

    <script type="text/javascript">

        function Myvalidations() {
            var CTOnes = 0;
            if ($('[id$=gvSevottam]')[0] == undefined && $('[id$=gvSevottamPOE]')[0] == undefined)
                CTOnes = 0;
            else if ($('[id$=gvSevottamPOE]')[0] != undefined)
                CTOnes = $('[id$=gvSevottamPOE]')[0].rows.length;
            else if ($('[id$=gvSevottam]')[0] != undefined)
                CTOnes = $('[id$=gvSevottam]')[0].rows.length;

            //var CTOnes1 = ($('[id$=gvSevottamPOE]')[0] == undefined ? $('[id$=gvSevottam]')[0].rows.length : $('[id$=gvSevottamPOE]')[0].rows.length);
            if ($('[id$=txtReferenceNo]').is(':enabled') && $('[id$=txtReferenceNo]').val().trim() == '') {
                $('[id$=txtReferenceNo]').focus();
                ErrorMessage('Reference No. is Required.');
                return false;
            }
            else if (CTOnes == 0 ) {
                ErrorMessage('No CT-Ones to Save.');
                return false;
            }
            else if (CTOnes > 0) {
                var select = 0;
                for (var i = 2; i < CTOnes; i++) {
                    var chkbx = "ctl"; if (i <= 9) { chkbx = chkbx + '0' + i; } else { chkbx = chkbx + i; }
                    var chkCTID = GetClientID(chkbx + "_chkCTID").attr("id");

                    if ($('#' + chkCTID)[0].checked) {
                        select = select + 1;
                        //return true;
                    }
                }
                if (select == 0) {
                    ErrorMessage('Select atleast One CT-1');
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
            else {
                return true;
            }
        }        
    </script>

</asp:Content>
