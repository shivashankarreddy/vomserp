<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="ProductSearch.aspx.cs" Inherits="VOMS_ERP.Purchases.ProductSearch"
     %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Product Search" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="right" class="formError1" />
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
                <table width="100%">
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel" onclick="btnExcelExpt_Click" ></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                    border="0">
                    <thead>
                        <tr>
                            <th width="10%">
                                SupplierNm
                            </th>
                            <th width="15%">
                                PONo
                            </th>
                            <th width="10%">
                                PODate
                            </th>
                            <th width="28%">
                                Description
                            </th>
                            <th width="5%">
                                PartNumber
                            </th>
                            <th width="05%">
                                Make
                            </th>
                            <th width="10%">
                                Discount
                            </th>
                            <th width="03%">
                                Rate
                            </th>
                            <th width="03%">
                                CreatedBy
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="4" align="left">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                SupplierNm
                            </th>
                            <th>
                                PONo
                            </th>
                            <th>
                                PODate
                            </th>
                            <th>
                                Description
                            </th>
                            <th>
                                PartNumber
                            </th>
                            <th>
                                Make
                            </th>
                            <th>
                                Discount
                            </th>
                            <th>
                                Rate
                            </th>
                            <th>
                                CreatedBy
                            </th>
                        </tr>
                    </tfoot>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFSuplr" runat="server" Value="" />
                <asp:HiddenField ID="HFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFFrmDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFDesc" runat="server" Value="" />
                <asp:HiddenField ID="HFPartNo" runat="server" Value="" />
                <asp:HiddenField ID="HFmake" runat="server" Value="" />
                <asp:HiddenField ID="HFDisc" runat="server" Value="" />
                <asp:HiddenField ID="HFRate" runat="server" Value="" />
                <asp:HiddenField ID="HFCreatedBy" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <style type="text/css">
        /*.dataTables_filter
        {
            visibility: visible !important;
        }*/
    </style>
    <style type="text/css">
        .ui-datepicker-calendar tr, .ui-datepicker-calendar td, .ui-datepicker-calendar td a, .ui-datepicker-calendar th
        {
            font-size: inherit;
        }
        div.ui-datepicker
        {
            font-size: 12px;
        }
        .ui-datepicker-title span
        {
            font-size: 12px;
        }
        
        .my-style-class input[type=text]
        {
            color: green;
        }
    </style>
    <script type="text/javascript">

        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[10, 200, 500], [10, 200, 500]],
                "iDisplayLength": 10,
                "aaSorting": [],
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "Purchases_WebService.asmx/GetProduct",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                //"bDeferRender": true,

                "fnInitComplete": function (oSettings, json) {
                    for (var i = 0, iLen = oSettings.aoData.length; i < iLen; i++) {
                        oSettings.aoData[i].nTr.className += " " + oSettings.aoData[i]._aData[0];
                    }
                },


                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
//                "sScrollXInner": "100%",
                "bScrollCollapse": true,

                "fnServerData": function (sSource, aoData, fnCallback) {
                    $.ajax({
                        "dataType": 'json',
                        "contentType": "application/json; charset=utf-8",
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success":
                                function (msg) {
                                    var json = jQuery.parseJSON(msg.d);
                                    fnCallback(json);
                                    $("#gvItmMstr").show();
                                }
                    });
                }
            });

            $("#gvItmMstr").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" }
                                 ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFSuplr]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFPONo]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFFrmDate]').val(Valuee);
                }

                else if (InDex == 3) {
                    $('[id$=HFToDt]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFDesc]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFPartNo]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFmake]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFDisc]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFRate]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFCreatedBy]').val(Valuee);
                }

            });

            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        });
   
    </script>
</asp:Content>
