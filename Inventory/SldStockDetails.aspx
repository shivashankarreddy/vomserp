<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="SldStockDetails.aspx.cs" Inherits="VOMS_ERP.Inventory.SldStockDetails"
     %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Sold Stock Details"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Supplier Name :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtSuplrNm" class="autosuggest" MaxLength="150" CssClass="bcAsptextbox"
                                        onkeyup="this.value=this.value.replace(/[^a-zA-Z /.&(),* ]/g,'');"></asp:TextBox>
                                        <asp:HiddenField ID="hfSuplrId" runat="server" Value="0" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">From Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFrmDt" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">To Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtToDt" CssClass="bcAsptextbox" MaxLength="12"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="6">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div4" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div5" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="LinkButton1" Text="Clear" OnClick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div6" class="bcButtonDiv">
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
                    <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvSoldStock" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                            AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            EmptyDataText="No Records are Exists" CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle"
                                            AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle" OnPreRender="gvSoldStock_PreRender"
                                            OnRowCommand="gvSoldStock_RowCommand" OnRowDataBound="gvSoldStock_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No.">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblgdnID" Text='<% #Eval("StockItemsId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:HyperLinkField HeaderText="Dispatch Inst No." DataTextField="DspchNmbr" DataNavigateUrlFields="ID"
                                                    DataNavigateUrlFormatString="GdnDetails.aspx?ID={0}" SortExpression="ID"></asp:HyperLinkField>--%>
                                                <asp:BoundField HeaderText="Description" DataField="ItemDesc" />
                                                <asp:BoundField HeaderText="Part Number" DataField="PartNumber" />
                                                <asp:BoundField HeaderText="Specifications" DataField="Specifications" />
                                                <asp:BoundField HeaderText="Make" DataField="Make" />
                                                <asp:BoundField HeaderText="Quantity" DataField="Quantity" />
                                                <asp:BoundField HeaderText="UoM" DataField="UserNm" />
                                                 <asp:BoundField HeaderText="Rate" DataField="Rate" />
                                                <asp:BoundField HeaderText="Amount" DataField="Amt" /> 
                                                <asp:BoundField HeaderText="LPO Number" DataField="LPONmbr" />
                                                <asp:BoundField HeaderText="User Name" DataField="UserNm" />
                                            </Columns>
                                        </asp:GridView>
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

    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>

    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(window).load(function() {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtFrmDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtToDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

        $(document).ready(function() {
            $("#<%=txtSuplrNm.ClientID %>").autocomplete({
                source: function(request, response) {
                    $.ajax({
                        url: '<%=ResolveUrl("~/Masters/AutoComplete.asmx/GetSupplierNm") %>',
                        data: "{ 'prefix': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function(data) {
                            response($.map(data.d, function(item) {
                                return {
                                    label: item.split('-')[0]
                                    , val: item.split('-')[1]
                                }
                            }))
                        }
                    });
                }
                , select: function(e, i) {
                    $('[id$=hfSuplrId]').val(i.item.val);
                }
                , minLength: 1
            });
        });


        /// not using
        function Myvalidations() {
            var res = $('[id$=txtSuplrNm]').val();
            var res1 = $('[id$=txtFrmDt]').val();
            if (res.trim() == '' && res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Supplier Name Or Dates are Required.</span>');
                $('[id$=txtSuplrNm]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
    </script>

    <script type="text/javascript">
        $("[id$=gvSoldStock]").dataTable({
            "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            "iDisplayLength": 10,
            "aaSorting": [[0, "asc"]],
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
        });

        function SearchStatus(valu) {
            var value1 = valu.toString();
            oTable.fnFilter(value1, 5);
        }
        
    </script>

</asp:Content>
