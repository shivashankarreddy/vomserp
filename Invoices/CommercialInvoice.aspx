<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CommercialInvoice.aspx.cs" Inherits="VOMS_ERP.Invoices.CommercialInvoice" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Commercial Invoice"
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
                        <td class="bcTdnormal">
                            <span id="lblRctdSctg" class="bcLabel">Proforma Invoice No. <font color="red" size="2">
                                <b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:DropDownList runat="server" ID="ddlRefno" AutoPostBack="true" CssClass="bcAspdropdown"
                                OnSelectedIndexChanged="ddlRefno_SelectedIndexChanged">
                                <asp:ListItem Text="Select Ref No." Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="bcTdnormal" style="display: block">
                            <span class="bcLabel">Vessel/Flight <font color="red" size="2"><b>*</b></font> :</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtVessel" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                        <td class="bcTdnormal">
                            <span id="Span11" class="bcLabel">AWB/BL.No.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtBlno" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal">
                            <span id="Span12" class="bcLabel">SB.NO.<font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtSbno" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                        <td>
                            <span id="Span7" class="bcLabel">&nbsp;Commercial Invoice No. <font color="red" size="2">
                                <b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtSpmntInvcNo" onkeypress="return isSomeSplChar(event)"
                                ValidationGroup="D" CssClass="bcAsptextbox" onchange="CheckCommercialInvNo()"></asp:TextBox>
                        </td>
                        <td>
                            <span id="Span8" class="bcLabel">Invoice Date <font color="red" size="2"><b>*</b></font>:</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtSpmntInvcDt" ValidationGroup="D" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                       <td class="bcTdnormal">
                            <span id="Span1" class="bcLabel">Notify :</span>
                        </td>
                        <td class="bcTdnormal">
                            <asp:TextBox runat="server" ID="txtNotify" CssClass="bcAsptextbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="bcTdnormal" colspan="2">
                            <div id="DivComments" runat="server" style="width: 100%;" visible="false">
                                <table width="100%">
                                    <tr>
                                        <td align="right" width="44%">
                                            <span id="Span10" class="bcLabel">Comments<font color="red" size="2"><b>*</b></font>:</span>
                                        </td>
                                        <td>
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
                            <asp:GridView runat="server" ID="gvCmrclInvce" AutoGenerateColumns="false" Width="100%"
                                EmptyDataText="No Records To Display...!" OnRowDataBound="gvCmrclInvce_RowDataBound"
                                OnPreRender="gvCmrclInvce_PreRender">
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No.">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="FPONmbr" DataField="FPONmbr" SortExpression="FPONmbr" />
                                    <asp:TemplateField Visible="false">
                                        <HeaderTemplate>
                                            <asp:CheckBox runat="server" ID="chkbhdr" Checked="true" Visible="false" onClick="javascript:chkAllCheckbox(this);" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" Checked="true" ID="chkbitm" Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IDs" Visible="false">
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item Desc">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsItemDtlsID" runat="server" Text='<%# Eval("StockItemsId") %>'
                                                Visible="false"></asp:Label>
                                            <asp:Label ID="lblItemID" runat="server" Text='<%# Eval("ItemId") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblFrnPoID" runat="server" Text='<%# Eval("ItmForeignPOId") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblSerialNo" runat="server" Visible="false"></asp:Label>
                                            <asp:HiddenField ID="hfFESNo" runat="server" Value='<%# Eval("Sno") %>' Visible="false" />
                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Eval("Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="HS-Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHSCode" runat="server" Text='<%# Eval("HSCode") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Part No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPartno" runat="server" Text='<%# Eval("PartNumber") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Specifications" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSpec" runat="server" Text='<%# Eval("Specifications") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Make">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMk" runat="server" Text='<%# Eval("Make") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("DspchQty") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Units" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUnits" runat="server" Text='<%# Eval("UnitNm") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate($)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRate" runat="server" Text='<%# Eval("Rate") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount($)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("Amount")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="6" align="right">
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
                                        <asp:LinkButton runat="server" ID="btnclear" OnClientClick="Javascript:clearAll()"
                                            Text="Clear" OnClick="btnclear_Click" />
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script type="">
        $(document).ready(function () {
            $('[id$=gvCmrclInvce]').dataTable(
            {
                "aLengthMenu": [[-1], ["All"]],
                "iDisplayLength": -1,
                "aaSorting": [],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            }).rowGrouping({ iGroupingColumnIndex: 0,
                iGroupingOrderByColumnIndex: 1,
                //sGroupBy: "letter",
                bHideGroupingColumn: true
            });
        });
    </script>
    <script type="text/javascript">
        var dateToday = new Date();
        $('[id$=txtSpmntInvcDt]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true,
            maxDate: dateToday
        });

        function CheckCommercialInvNo() {
            try {
                var CommercialInvoiceNo = $('[id$=txtSpmntInvcNo]').val();
                var result = CommercialInvoice.CheckCommercialInvNo(CommercialInvoiceNo);
                if (result.value == false) {
                    ErrorMessage('Commercial Invoice Number Exists.');
                    $('[id$=txtSpmntInvcNo]').val('').focus();
                    //$('[id$=txtSpmntInvcNo]').focus();
                }
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }

        function Myvalidations() {
            try {
                if (($('[id$=ddlRefno]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                    ErrorMessage('Proforma Invoice Number is Required.');
                    $('[id$=ddlRefno]').focus();
                    return false;
                }
                else if (($('[id$=txtVessel]').val()).trim() == '') {
                    ErrorMessage('Vessel/Flight is Required.');
                    $('[id$=txtVessel]').focus();

                    return false;
                }
                else if (($('[id$=txtBlno]').val()).trim() == '') {
                    ErrorMessage('BL.No is Required.');
                    $('[id$=txtBlno]').focus();

                    return false;
                }
                else if (($('[id$=txtSbno]').val()).trim() == '') {
                    ErrorMessage('SB.No is Required.');
                    $('[id$=txtSbno]').focus();

                    return false;
                }
                else if (($('[id$=txtSpmntInvcNo]').val()).trim() == '') {
                    ErrorMessage('Commercial Invoice Number is Required.');
                    $('[id$=txtSpmntInvcNo]').focus();
                    return false;
                }
                else if (($('[id$=txtSpmntInvcDt]').val()).trim() == '') {
                    ErrorMessage('Commercial Invoice Date is Required.');
                    $('[id$=txtSpmntInvcDt]').focus();
                    return false;
                }
                if ($('[id$=DivComments]').css("visibility") == "visible") {
                    if (($('[id$=txtComments]').val()).trim() == '') {
                        ErrorMessage('Comment is Required.');
                        $('[id$=txtComments]').focus();
                        return false;
                    }
                }
                return true;
            }
            catch (Error) {
                ErrorMessage(Error.message);
                return false;
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
