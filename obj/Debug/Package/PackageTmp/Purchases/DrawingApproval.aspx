<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DrawingApproval.aspx.cs" Inherits="VOMS_ERP.Purchases.DrawingApproval" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Drawing Approval" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span11" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
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
                                        <asp:HiddenField ID="hfDrawingRefNo" runat="server" Value="" />
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
                                        <asp:DropDownList runat="server" ID="ddlFPO" CssClass="bcAspdropdown" OnSelectedIndexChanged="ddlFPO_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Text="Select FPO" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabel">Local PO(s) <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlLPO" CssClass="bcAspdropdown" AutoPostBack="True"
                                            OnSelectedIndexChanged="ddlLPO_SelectedIndexChanged">
                                            <asp:ListItem Text="Select LPO" Value="-1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabel">LPO Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtLPODate" CssClass="bcAsptextbox" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <%--<td class="bcTdnormal">
                                        <span class="bcLabel">Drawing Ref. No. <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRefNo" CssClass="bcAsptextbox" onchange="GetRefNo();"></asp:TextBox>
                                    </td>--%>
                                    <td class="bcTdnormal">
                                        <span id="Span4" class="bcLabel">Received Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRecvDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabel">Approval Req. Dt. :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtApprovalReqDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabel">Cust. Response Date <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtCustReponseDt" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabel">Supp. Intimation Date <font color="red" size="2"><b>
                                            *</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSuppIntimationDt" MaxLength="12" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <%--<td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Response Status <font color="red" size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="bcAspdropdown">
                                            <asp:ListItem Text="Select Status" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Rejected" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>--%>
                                    <td class="bcTdnormal">
                                        <span id="Span9" class="bcLabel">Self Approval Date :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSelfApprovalDT" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabel">Subject :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtSubject" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span8" class="bcLabel">Remarks :</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtRemarks" ValidationGroup="D" CssClass="bcAsptextboxmulti"
                                            TextMode="MultiLine" Rows="4"></asp:TextBox>
                                    </td>
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
                                <%--<tr>
                                    <td colspan="6">
                                        <asp:GridView ID="GvDrawingApprovals" runat="server" Width="100%" AutoGenerateColumns="false"
                                            ShowFooter="true" OnRowDataBound="GvDrawingApprovals_RowDataBound" 
                                            OnRowDeleting="GvDrawingApprovals_RowDeleting" 
                                            onrowcommand="GvDrawingApprovals_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No.">
                                                    <ItemTemplate>
                                                       <asp:Label ID="lblSno" runat="server" Text='<%# Container.DataItemIndex+1 %>' ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Select FPO">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlFPO" runat="server" AutoPostBack="True" 
                                                        OnSelectedIndexChanged="ddlFPO_OnSelectedIndexChanged">
                                                            <asp:ListItem Text="Select FPO" Value="0"></asp:ListItem>                                                            
                                                        </asp:DropDownList>
                                                        <asp:HiddenField ID="hfFPO" runat="server" Value='<% #Eval("FPOID") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:DropDownList ID="ddlAddFPO" runat="server" AutoPostBack="True" 
                                                        OnSelectedIndexChanged="ddlAddFPO_OnSelectedIndexChanged">
                                                            <asp:ListItem Text="Select FPO" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Select LPO">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlLPO" runat="server"
                                                         OnSelectedIndexChanged="ddlLPO_OnSelectedIndexChanged">
                                                            <asp:ListItem Text="Select LPO" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:HiddenField ID="hfLPO" runat="server" Value='<% #Eval("LPOID") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:DropDownList ID="ddlAddLPO" runat="server" 
                                                        OnSelectedIndexChanged="ddlAddLPO_OnSelectedIndexChanged">
                                                            <asp:ListItem Text="Select LPO" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Drawing Ref.No.">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDrawingRefNo" runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtAddDrawingRefNo" runat="server"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Received Date">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtReceivedDT" runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtAddReceivedDT" runat="server"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Approval ReqDT">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtApprovalReqDT" runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtAddApprovalReqDT" runat="server"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cust. Res. DT">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCustResDT" runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtAddCustResDT" runat="server"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Supp. Int. DT">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtSuppIntDT" runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtAddSuppIntDT" runat="server"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Res. Status">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlResStatus" runat="server">
                                                            <asp:ListItem Text="Select Status" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Rejected" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:DropDownList ID="ddlAddResStatus" runat="server">
                                                            <asp:ListItem Text="Select Status" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Rejected" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <FooterTemplate>
                                                        <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="~/images/btnAdd.png" CommandName="Add" />
                                                    </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/btnDelete.png"
                                                            CommandName="Delete" CommandArgument='<%# Container.DataItemIndex %>'/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                                
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>--%>
                                <tr>
                                    <td colspan="6">
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <div style="overflow: auto; width: 100%; min-height: 100px; max-height: 190px;" id="divDrawings"
                                            runat="server">
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
            ClearUploadControle($("#<%=AsyncFileUpload1.ClientID%>"));
        });
        $(document).ready(function () {
            Expnder();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }
     
        function ExpandTXT(obj) {
            $('#' + obj).animate({ "height": "75px" }, "slow");
            $('#' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#' + obj).animate({ "height": "20px" }, "slow");
            $('#' + obj).slideDown("slow");
        }

        function GetRefNo(RowNo) {
            var FPOID = $('[id$=ddlFPO]').val();
            var LPOID = $('[id$=ddlLPO]').val();
            var refNo = ($('[id$=txtDrawingRefNo' + RowNo + ']').val()).trim();
            if (refNo.trim() != '') {
                var flag = 0;
                var RowCount = $('#DrawingAppTbl tbody tr').length;
                for (var i = 0; i < RowCount; i++) {
                    var otherRefNo = ($('[id$=txtDrawingRefNo' + (parseInt(i) + 1) + ']').val()).trim();
                    if (otherRefNo == refNo && i != parseInt(RowNo) - 1) {
                        flag = 1;
                    }
                }
                if (flag == 0) {
                    var result = DrawingApproval.GetRefNo(refNo.trim());
                    if (result.value == true) {
                        $('[id$=txtDrawingRefNo' + RowNo + ']').val('');
                        $('[id$=txtDrawingRefNo' + RowNo + ']').focus();
                        ErrorMessage('Drawing Ref. No is already in Use');
                    }
                    else {
                        FillItemGrid(RowNo);
                    }
                }
                else {
                    $('[id$=txtDrawingRefNo' + RowNo + ']').val('');
                    $('[id$=txtDrawingRefNo' + RowNo + ']').focus();
                    ErrorMessage('Drawing Ref. No is already in Use ');
                }
            }
            else {
                $('[id$=txtDrawingRefNo' + RowNo + ']').val('');
                $('[id$=txtDrawingRefNo' + RowNo + ']').focus();
                ErrorMessage('Drawing Ref. No is cannot be empty.');
            }
        }

        function AddNewRow(RowNo) {
            var DrRfNo = $('[id$=txtDrawingRefNo' + RowNo + ']').val();
            var Description = $('[id$=txtDescription' + RowNo + ']').val();
            var Comments = $('[id$=txtComments' + RowNo + ']').val();
            var RStatus = $('[id$=ddlStatus' + RowNo + ']').val();

            if (DrRfNo.trim() == '') {
                $('[id$=txtDrawingRefNo' + RowNo + ']').focus();
                $('[id$=txtDrawingRefNo' + RowNo + ']').val('');
                ErrorMessage('Drawing Ref. Number cannot be Empty.');
            }
            else if (Description == '') {
                $('[id$=txtDescription' + RowNo + ']').focus();
                $('[id$=txtDescription' + RowNo + ']').val('');
                ErrorMessage('Description cannot be empty.');
            }
            else if (Comments == '') {
                $('[id$=txtComments' + RowNo + ']').focus();
                $('[id$=txtComments' + RowNo + ']').val('');
                ErrorMessage('Comments cannot be empty.');
            }
            else if (RStatus == '0') {
                $('[id$=ddlStatus' + RowNo + ']').focus();
                ErrorMessage('Please select Status.');
            }
            else {
                //                $('#txtSpecification' + '' + obj).animate({ "height": "75px" }, "slow");
                //                $('#txtSpecification' + '' + obj).slideDown("slow");

                var result = DrawingApproval.AddNewRow();
                var getDivDrawing = GetClientID("divDrawings").attr("id");
                $('#' + getDivDrawing).html(result.value);
            }
        }

        function FillItemGrid(RowNo) {
            var DrRfNo = $('[id$=txtDrawingRefNo' + RowNo + ']').val();
            var Description = $('[id$=txtDescription' + RowNo + ']').val();
            var Comments = $('[id$=txtComments' + RowNo + ']').val();
            var RStatus = $('[id$=ddlStatus' + RowNo + ']').val();

            var result = DrawingApproval.SaveChanges(RowNo, DrRfNo, Description, Comments, RStatus);
            var getDivDrawing = GetClientID("divDrawings").attr("id");
            $('#' + getDivDrawing).html(result.value);
        }

        function doConfirm(RowNo) {
            if (confirm("Are you sure you want to Delete this Record...?")) {
                var result = DrawingApproval.DeleteRecord(RowNo);
                var getDivDrawing = GetClientID("divDrawings").attr("id");
                $('#' + getDivDrawing).html(result.value);
            }
            else {
                return false;
            }
        }

        function uploadComplete() {
            var result = DrawingApproval.AddItemListBox();
            var getDivCEDItems = GetClientID("divListBox").attr("id");
            $('#' + getDivCEDItems).html(result.value);
            if (result.value == "") {
                ErrorMessage("File Size is more than 25MB, Resize and Try Again");
            }
            else {
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
                //$get("<%=lblstatus.ClientID%>").innerHTML = "File uploaded <b>SuccessFully</b>, If U need Upload New File.";
                SuccessMessage('File uploaded SuccessFully.');
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
            if ($('#lbItems').val() != null) {
                if (confirm("Are you sure you want to delete the item")) {
                    var result = DrawingApproval.DeleteItemListBox($('#lbItems').val());
                    var getDivCEDItems = GetClientID("divListBox").attr("id");
                    $('#' + getDivCEDItems).html(result.value);
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
            if ($('#lbItems').val() != null) {
                var result = DrawingApproval.AddItemListBox();
                var getDivCEDItems = GetClientID("divListBox").attr("id");
                $('#' + getDivCEDItems).html(result.value);
                var listid = GetClientID("lbItems").attr("id");
                $('#' + listid)[0].selectedIndex = '0';
            }
            
        });

        $(function () {
            var dateToday = new Date();
            $('[id$=txtRecvDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

            $('[id$=txtApprovalReqDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

            $('[id$=txtCustReponseDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

            $('[id$=txtSuppIntimationDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

            $('[id$=txtSelfApprovalDT]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });





        function Myvalidations() {
            var RowCount = $('#DrawingAppTbl tbody tr').length;
            if (($('[id$=ddlcustmr]').val()).trim() == '0') {
                $('[id$=ddlcustmr]').focus();
                ErrorMessage('Customer Name is Required.');
                return false;
            }
            else if (($('[id$=ddlsuplrctgry]').val()).trim() == '0') {
                $('[id$=ddlsuplrctgry]').focus();
                ErrorMessage('Supplier Catagory is Required.');
                return false;
            }
            else if (($('[id$=ddlsuplr]').val()).trim() == '0') {
                $('[id$=ddlsuplr]').focus();
                ErrorMessage('Supplier Name is Required.');
                return false;
            }
            else if ($('[id$=ddlFPO]').val() == '0') {
                $('[id$=ddlFPO]').focus();
                ErrorMessage('Foreign PO(s) is Required.');
                return false;
            }
            else if ($('[id$=ddlLPO]').val() == '0') {
                $('[id$=ddlLPO]').focus();
                ErrorMessage('Local PO(s) is Required.');
                return false;
            }
            else if ($('[id$=txtLPODate]').val() == '') {
                $('[id$=txtLPODate]').focus();
                ErrorMessage('Local PO(s) Date Required.');
                return false;
            }
            else if ($('[id$=txtRefNo]').val() == '') {
                $('[id$=txtRefNo]').focus();
                ErrorMessage('Drawing Reference No. is Required.');
                return false;
            }
            else if ($('[id$=txtRecvDt]').val() == '') {
                $('[id$=txtRecvDt]').focus();
                ErrorMessage('Received Date Required.');
                return false;
            }
            else if ($('[id$=txtCustReponseDt]').val() == '') {
                $('[id$=txtCustReponseDt]').focus();
                ErrorMessage('Customer Response Date Required.');
                return false;
            }
            else if ($('[id$=txtSuppIntimationDt]').val() == '') {
                $('[id$=txtSuppIntimationDt]').focus();
                ErrorMessage('Supplier Intimation Date Required.');
                return false;
            }
            else if ($('[id$=txtRecvDt]').val() == '') {
                $('[id$=txtRecvDt]').focus();
                ErrorMessage('Received Date Required.');
                return false;
            }
            else if ($('[id$=ddlStatus]').val() == '0') {
                $('[id$=ddlStatus]').focus();
                ErrorMessage('Status is Required.');
                return false;
            }
            if (RowCount > 0) {
                var flag = 0;
                for (var i = 0; i < RowCount; i++) {
                    var sno = parseInt(i) + 1;
                    var DrRfNo = $('[id$=txtDrawingRefNo' + sno + ']').val();
                    var Description = $('[id$=txtDescription' + sno + ']').val();
                    var Comments = $('[id$=txtComments' + sno + ']').val();
                    var RStatus = $('[id$=ddlStatus' + sno + ']').val();

                    if (DrRfNo.trim() == '') {
                        $('[id$=txtDrawingRefNo' + sno + ']').focus();
                        $('[id$=txtDrawingRefNo' + sno + ']').val('');
                        ErrorMessage('Drawing Ref. Number cannot be Empty.');
                        flag = 1;
                        return false;
                    }
                    else if (Description == '') {
                        $('[id$=txtDescription' + sno + ']').focus();
                        $('[id$=txtDescription' + sno + ']').val('');
                        ErrorMessage('Description cannot be empty.');
                        flag = 1;
                        return false;
                    }
                    else if (Comments == '') {
                        $('[id$=txtComments' + sno + ']').focus();
                        $('[id$=txtComments' + sno + ']').val('');
                        ErrorMessage('Comments cannot be empty.');
                        flag = 1;
                        return false;
                    }
                    else if (RStatus == '0') {
                        $('[id$=ddlStatus' + sno + ']').focus();
                        ErrorMessage('Please select Status.');
                        flag = 1;
                        return false;
                    }
                }
            }
            else {
                ErrorMessage('No Drawing Approvals to Save.');
                return false;
            }
            if ($('[id$=DivComments]').css("visibility") == "visible") {
                if (($('[id$=txtComments]').val()).trim() == '') {
                    $('[id$=txtComments]').focus();
                    ErrorMessage('Comment is Required.');
                    return false;
                }
            }
            else {
                return true;
            }
        }

    </script>
</asp:Content>
