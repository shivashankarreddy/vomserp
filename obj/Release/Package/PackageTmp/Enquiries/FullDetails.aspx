<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FullDetails.aspx.cs" Inherits="VOMS_ERP.Enquiries.FullDetails"  %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- <ajax:ToolkitScriptManager ID="Scriptmanager1" runat="server" />--%>
    <table style="width: 98%; vertical-align: top;" align="center">
        <tr>
            <td align="right" class="">
                <div id="Div1" class="bcButtonDiv bcTdButton" style="width:100px;">
                    <asp:HyperLink runat="server" ID="btnBack_TOP" Text="Back" NavigateUrl="~/Enquiries/FEStatusNew.aspx" />
                </div>
            </td>
        </tr>
        <tr class="bcTRTitleRow">
            <td class="bcTdTitleLeft">
                <asp:HiddenField ID="HFAccordions" runat="server" Value="0" />
                <div style="width: 100%">
                    <ajax:Accordion ID="UserAccordion" runat="server" SelectedIndex="0" HeaderCssClass="accordionHeader"
                        HeaderSelectedCssClass="accordionHeaderSelected" ContentCssClass="accordionContent"
                        FadeTransitions="true" SuppressHeaderPostbacks="true" TransitionDuration="250"
                        FramesPerSecond="40" RequireOpenedPane="false">
                        <Panes>
                            <ajax:AccordionPane ID="AccordionPane1" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Foreign Enquiry</a>
                                    <asp:Label ID="lblFENo" runat="server" Text="" ForeColor="black" CssClass="singlepageLabel"></asp:Label>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel1" runat="server">
                                        <div id="divFEAll" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane2" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Local Enquiry</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel2" runat="server">
                                        <div id="divFenqAll" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane3" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Local Quotation</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel3" runat="server">
                                        <div id="LQData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane4" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Foreign Quotation</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel4" runat="server">
                                        <div id="divFQuotation" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane5" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Foreign Purchase Order</a>
                                    <asp:Label ID="lblFPONos" runat="server" Text="" ForeColor="black" CssClass="singlepageLabel"></asp:Label></Header>
                                <Content>
                                    <asp:Panel ID="Panel5" runat="server">
                                        <div id="FPData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane6" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Local Purchase Order</a>
                                    <asp:Label ID="lblLPONos" runat="server" Text="" ForeColor="black" CssClass="singlepageLabel"></asp:Label>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel6" runat="server">
                                        <div id="LPOData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane7" runat="server" Visible="false">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Centarl Excise</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel7" runat="server">
                                        <div id="CExciseData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane8" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Inspection</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel8" runat="server">
                                        <div id="InsptnData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane9" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Drawings</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel9" runat="server">
                                        <div id="Drawings" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane12" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Proforma Invoice Request</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel12" runat="server">
                                        <div id="DivPINVRequest" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane13" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">IOM (Inter Office Memo) Template</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel13" runat="server">
                                        <div id="DivIOMtemplate" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane10" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">CT-1 Details</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel11" runat="server">
                                        <div id="CT1Data" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane11" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Dispatch Instructions</a></Header>
                                <Content>
                                    <asp:Panel ID="Panel10" runat="server">
                                        <div id="DpchInstnData" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane14" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Goods Receipt Note </a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel14" runat="server">
                                        <div id="DivGRN" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane15" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Shipment Planning Details </a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel15" runat="server">
                                        <div id="DivCheckListDetails" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane16" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Shipment Proforma Invoice </a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel16" runat="server">
                                        <div id="DivShipmentProforma" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane17" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Packing List </a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel17" runat="server">
                                        <div id="DivPackingList" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane22" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Shipping Bill Details</a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel22" runat="server">
                                        <div id="DivShpngBillDetails" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane18" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">AirWay Bill </a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel18" runat="server">
                                        <div id="DivAirWayBill" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane19" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Bill of Lading</a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel19" runat="server">
                                        <div id="DivBillOfLading" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane20" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">Mate Receipt</a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel20" runat="server">
                                        <div id="DivMateReceipt" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                            <ajax:AccordionPane ID="AccordionPane21" runat="server">
                                <Header>
                                    <a href="javascript:void(0)" class="href">E-BRC Details</a>
                                </Header>
                                <Content>
                                    <asp:Panel ID="Panel21" runat="server">
                                        <div id="DivEBRCDetails" runat="server" style="min-height: 200px; max-height: 400px">
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </ajax:AccordionPane>
                        </Panes>
                    </ajax:Accordion>
                    <asp:HiddenField ID="HFItemsTable" runat="server" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="right" class="">
                <div id="Div2" class="bcButtonDiv bcTdButton" style="width:100px;">
                    <asp:HyperLink runat="server" ID="btnBack_Bottom" Text="Back"  NavigateUrl="~/Enquiries/FEStatusNew.aspx" />
                </div>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.rowGrouping.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script src="../JScript/tooltip/jquery.powertip.js" type="text/javascript"></script>
    <script type="text/javascript">
        var Itemtablescount = $('[id$=HFItemsTable]').val();
        $(document).ready(function () {
            for (var j = 1; j <= Itemtablescount; j++) {
                $('[id$=tblPrfINVItems' + j + ']').dataTable({
                    "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    "iDisplayLength": 10,
                    "aaSorting": [[1, "asc"]],
                    "bJQueryUI": true,
                    "bAutoWidth": true,
                    "bProcessing": false,
                    "bPaginate": false,
                    "bFilter": false,
                    "bInfo": false,
                    "sPaginationType": "full_numbers",
                    "oLanguage": {
                        "sZeroRecords": "There are no Records that match your search criteria",
                        "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                        "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                        "sInfoEmpty": "Showing 0 to 0 of 0 records",
                        "sInfoFiltered": "(filtered from _MAX_ total records)",
                        "sSearch": "Search :"
                    },
                    "sScrollY": "150px",
                    "sScrollX": "100%",
                    "sScrollXInner": "100%",
                    "bScrollCollapse": true
                }).rowGrouping({ iGroupingColumnIndex: 0,
                    bHideGroupingColumn: true
                });
            }
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('div.expanderR').expander();
        });
        function Expnder() {
            $('div.expanderR').expander();
        }
        $(function () {
            $('#mousefollow-examples div').powerTip({ followMouse: true });
        });

        $(function () {
            var count = $('[id$=HFAccordions]').val();
            for (var i = 0; i <= count; i++) {
                $("#accordion" + i + "").accordion({
                    active: false,
                    autoHeight: false,
                    navigation: true,
                    collapsible: true
                });
            }
        });

        function ExpandTXT(obj) {
            $('#' + obj).animate({ "height": "75px" }, "slow");
            $('#' + obj).slideDown("slow");
        }

        function ReSizeTXT(obj) {
            $('#' + obj).animate({ "height": "20px" }, "slow");
            $('#' + obj).slideDown("slow");
        }
    </script>
</asp:Content>
