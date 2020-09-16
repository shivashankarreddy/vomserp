<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="LpoAmendments.aspx.cs" Inherits="VOMS_ERP.Purchases.LpoAmendments" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="Local Purchase Order Amendments Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                            <tr>
                                <td align="right">
                                    <asp:ImageButton ID="btnExcelExpt" OnClick="btnExcelExpt_Click" runat="server" ImageUrl="../images/EXCEL.png"
                                        title="Export Excel"></asp:ImageButton>
                                </td>
                            </tr>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="98%">
                </table>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvLpoStatus" class="widthFull fontsize10 displayNone" cellpadding="0"
                        cellspacing="0" border="0">
                        <thead>
                            <tr>
                                <th width="10%">
                                    Date
                                </th>
                                <th width="08%">
                                    LPO Number
                                </th>
                                <th width="15%">
                                    Ref. FPO No.
                                </th>
                                <th width="08%">
                                    Subject
                                </th>
                                <th width="08%">
                                    Status
                                </th>
                                <th width="08%">
                                    Supplier
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                        <tfoot>
                            <tr>
                                <th style="text-align: right" colspan="3">
                                </th>
                                <th colspan="2" align="left">
                                </th>
                                <th colspan="1" align="right">
                                </th>
                            </tr>
                            <tr>
                                <th>
                                    Date
                                </th>
                                <th>
                                    LPO Number
                                </th>
                                <th>
                                    Ref. FPO No.
                                </th>
                                <th>
                                    Subject
                                </th>
                                <th>
                                    Status
                                </th>
                                <th>
                                    Supplier
                                </th>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFLPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                <asp:HiddenField ID="HFSupplier" runat="server" Value="" />
            </td>
        </tr>
    </table>
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery.ui.theme.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/themes/overcast/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="../JScript/Scripts/css/jquery.dataTables_themeroller.css" rel="stylesheet"
        type="text/css" />
    <script src="../JScript/Scripts/js/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>
    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
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

            oTable = $("[id$=gvLpoStatus]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "iDisplayLength": 100,
                "aaSorting": [],
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "Purchases_WebService.asmx/GetLPOAmendmensItems",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                //"sScrollXInner": "100%",
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
                                    $("#gvLpoStatus").show();
                                }
                    });
                }
            });
            $("#gvLpoStatus").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                      null, null, null, null, null, null
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFLPONo]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFStatus]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFSupplier]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvLpoStatus").dataTable();
        });


        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

    </script>
    <script type="text/javascript">
        //$(document).ready(function() {
        //without passing class names.
        $("[id$=gvLpoItms]").dataTable({
            "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            "iDisplayLength": 10,
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

            //Scrolling--------------
            "sScrollY": "250px",
            "sScrollX": "100%",
            "sScrollXInner": "100%",
            "bScrollCollapse": true
        });
        //});       
    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtfromdt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txttodt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
    </script>
</asp:Content>
