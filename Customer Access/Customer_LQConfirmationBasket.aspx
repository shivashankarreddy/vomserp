<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true" CodeBehind="Customer_LQConfirmationBasket.aspx.cs" Inherits="VOMS_ERP.Customer_Access.Customer_LQConfirmationBasket" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Customer Local Quotation Confirmation Basket"
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="2">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" style="background-color: #F5F4F4; border: solid 1px #ccc" align="center">
                    <tr>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Name of Customer <font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlcustmr" class="bcAspdropdown" OnSelectedIndexChanged="ddlcustmr_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td class="bcTdnormal">
                            <span class="bcLabel">Purchase Enquiry Number(s)<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:ListBox runat="server" ID="lbFenquiry" CssClass="bcAspMultiSelectListBox" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlfenqy_SelectedIndexChanged" SelectionMode="Multiple">
                            </asp:ListBox>
                            <%--<asp:DropDownList runat="server" ID="ddlfenqy" class="bcAspdropdown" Visible="false" OnSelectedIndexChanged="ddlfenqy_SelectedIndexChanged"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="Select Enquiry Number"></asp:ListItem>
                                        </asp:DropDownList>--%>
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
            <td class="bcTdnormal" colspan="6">
                <asp:GridView runat="server" ID="gvConformBskt" RowStyle-CssClass="bcGridViewRowStyle"
                    AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                    PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                    CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                    Width="100%" OnPreRender="gvConformBskt_PreRender">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No.">
                            <ItemTemplate>
                                <%# Container.DataItemIndex +1 %>
                                <asp:Label runat="server" ID="lblSno" Visible="false" Text='<% #Eval("SNo") %>'></asp:Label>
                                <asp:Label runat="server" ID="lblFenqID" Text='<% #Eval("ForeignEnquireId") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item Description">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblItemDescription" Text='<% #Eval("ItemDescription") %>'></asp:Label>
                                <asp:Label runat="server" ID="lblItemID" Visible="false" Text='<% #Eval("ItemId") %>'></asp:Label>
                                <asp:Label runat="server" ID="lblItemDtlsID" Visible="false" Text='<% #Eval("ItemDetailsId") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Part Number">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPartNumber" Text='<% #Eval("PartNumber") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Specifications">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSpecificaiton" Text='<% #Eval("Specifications") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Make">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMake" Text='<% #Eval("Make") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantity">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQty" Text='<% #Eval("Quantity") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblUnitID" Text='<% #Eval("UNumsId") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRate" Text='<% #Eval("Rate1") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rate">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQPrice" Text='<% #Eval("Rate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Amount">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAmount" Text='<% #Eval("Amont") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:TemplateField HeaderText="Remarks">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblRemarks" Text='<% #Eval("") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="6" class="bcTdNewTable">
                &nbsp;<asp:HiddenField runat="server" ID="hdfempty" />
            </td>
        </tr>
        <tr>
            <td colspan="6" align="right" class="bcTdNewTable">
                <center>
                    <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                        <tbody>
                            <tr valign="middle">
                                <td align="center" valign="middle" class="bcTdButton">
                                    <div id="Div1" class="bcButtonDiv">
                                        <asp:LinkButton runat="server" ID="btnSave" Text="Confirm With LPO" OnClientClick="javascript:Myvalidations()"
                                            OnClick="btnSave_Click" />
                                    </div>
                                </td>
                                <td align="center" valign="middle" class="bcTdButton">
                                    <div id="Div5" class="bcButtonDiv">
                                        <asp:LinkButton runat="server" ID="btnFqSave" Visible="false" Text="Confirm with FQ"
                                            OnClientClick="javascript:Myvalidations()" OnClick="btnFqSave_Click" />
                                    </div>
                                </td>
                                <td align="center" valign="middle" class="bcTdButton">
                                    <div id="Div4" class="bcButtonDiv">
                                        <asp:LinkButton runat="server" ID="btnLpoSave" Visible="false" Text="Confirm with FPO"
                                            OnClientClick="javascript:Myvalidations();" OnClick="btnFpoSave_Click" />
                                    </div>
                                </td>
                                <td align="center" valign="middle" class="bcTdButton">
                                    <div id="Div2" class="bcButtonDiv">
                                        <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
                                    </div>
                                </td>
                                <td align="center" valign="middle" class="bcTdButton">
                                    <div id="Div3" class="bcButtonDiv">
                                        <%--<asp:Button runat="server" ID="btnExit" OnClientClick="Javascript:redirect()" Text="Exit"/>--%>
                                        <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </center>
            </td>
        </tr>
        <tr>
            <td colspan="6" class="bcTdNewTable">
                &nbsp;
            </td>
        </tr>
    </table>
    <ajax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" CancelControlID="btnCancel"
        TargetControlID="hdfempty" OnOkScript="Validations()" OnCancelScript="CancelSelection()"
        PopupControlID="Panel1" PopupDragHandleControlID="PopupHeader" Drag="true" BackgroundCssClass="ModalPopupBG">
    </ajax:ModalPopupExtender>
    <asp:Panel ID="Panel1" Style="display: none; top: 120px;" runat="server">
        <div class="Popup">
            <div class="PopupHeader" id="PopupHeader">
                Conversion Rates</div>
            <div class="PopupBody">
                <p>
                    All * Fields are Mandatory</p>
                <table>
                    <tr>
                        <td>
                            <span id="Span10" class="bcLabel">Conversion Rate(Rs) <font color="red" size="2"><b>
                                *</b></font> :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="10" Width="50%" CssClass="bcAsptextbox" ID="txtConversionRt"
                                onblur="extractNumber(this,2,false);" onkeyup="extractNumber(this,2,false);"
                                onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="Span1" class="bcLabel">Fob + Margin(%) <font color="red" size="2"><b>*</b></font>
                                :</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" MaxLength="10" Width="50%" CssClass="bcAsptextbox" ID="txtMargin"
                                onblur="extractNumber(this,2,false);" onkeyup="extractNumber(this,2,false);"
                                onkeypress="return blockNonNumbers(this, event, true, false);"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="Span2" class="bcLabel">Round off :</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlRateChange" CssClass="bcAspdropdown">
                                <asp:ListItem Text="--Select--" Value="0" />
                                <asp:ListItem Text="1 Decimals" Value="1" />
                                <asp:ListItem Text="2 Decimals" Value="2" />
                                <asp:ListItem Text="0 Decimals" Value="3" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="Controls">
                <%--<input id="btnOkay" type="button" onclick="return Validations()" runat="server"  value="Done" />--%>
                <asp:Button runat="server" ID="btnOkey" Text="Done" OnClientClick="return Validations()"
                    OnClick="btnOkey_Click" />
                <input id="btnCancel" type="button" value="Cancel" onclick="$find('ModalPopupExtender1').hide(); return false;" />
            </div>
        </div>
    </asp:Panel>
    <style type="text/css">
        .ModalPopupBG
        {
            background-color: #666699;
            filter: alpha(opacity=50);
            opacity: 0.7;
        }
        .Popup
        {
            min-width: 50%;
            max-width: 99%;
            min-height: 45%;
            max-height: 85%;
            background: white;
            padding: 5px 5px 5px 5px;
            border: solid 1px blue;
            margin: 10%;
        }
        .PopupHeader
        {
            font-size: 2em;
            font-family: Arial;
        }
        .PopupBody
        {
            font-size: 1em;
            font-family: Times New Roman;
        }
    </style>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvConformBskt]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "bJQueryUI": true,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "bDestroy": true,
                "aaSorting": [],
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
        });       
    </script>
    <script type="text/javascript">
        function Myvalidations() {
            if (($('[id$=ddlcustmr]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Customer is Required.');
                $('[id$=ddlcustmr]').focus();
                return false;
            }
            else if ($('[id$=lbFenquiry]').val() == null) {
                ErrorMessage('Enquiry is Required.');
                $('[id$=lbFenquiry]').focus();
                return false;
            }
            else if ($('[id$=gvConformBskt]').val() == null) {
                ErrorMessage('No Items to Confirm.');
                $('[id$=gvConformBskt]').focus();
                //$('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }

        function Validations() {
            try {
                if (($('[id$=txtConversionRt]').val()).trim() == '0' || ($('[id$=txtConversionRt]').val()).trim() == '') {
                    ErrorMessage('Conversion Rate is Required.');
                    $('[id$=ddlPmtPop]').focus();
                    return false;
                }
                else if (($('[id$=txtMargin]').val()).trim() == '0' || ($('[id$=txtMargin]').val()).trim() == '') {
                    ErrorMessage('Fob + Margin(%) is Required.');
                    $('[id$=txtMargin]').focus();
                    return false;
                }
                else {
                    return true;
                }
            } catch (Error) {
                alert(Error.Message);
            }
        }

    </script>
</asp:Content>
