<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="StatusFeRecevd.aspx.cs" Inherits="VOMS_ERP.Reports.StatusFeRecevd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table width="100%">
                    <tr class="bcTRTitleRow">
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Style="font-size: 15.5px;
                                font-weight: bold" Text="Status of Foreign Enquiry Regreted" CssClass="bcTdTitleLabel"></asp:Label>
                            <asp:HiddenField ID="HF_HeadText" runat="server" />
                            <div id="divMyMessage" runat="server" align="center" class="formError1" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <table id="tblOscarNominees" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th width="10%">
                                FE Number
                            </th>
                            <th width="10%">
                                FE Date
                            </th>
                            <th width="10%">
                                Recv Date
                            </th>
                            <th width="20%">
                                Subject
                            </th>
                            <th width="10%">
                                Department
                            </th>
                            <th width="10%">
                                From
                            </th>
                            <th width="15%">
                                Status
                            </th>
                            <th>
                                IsRegret
                            </th>
                            <th>
                                Remarks
                            </th>
                            <th>
                                Comments
                            </th>
                            <th>
                                Created By
                            </th>
                        </tr>
                    </thead>
                    <tbody class="tbody">
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="5" align="left">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                FE Number
                            </th>
                            <th>
                                FE Date
                            </th>
                            <th>
                                Recv Date
                            </th>
                            <th>
                                Subject
                            </th>
                            <th>
                                Department
                            </th>
                            <th>
                                From
                            </th>
                            <th>
                                Status
                            </th>
                            <th>
                            </th>
                            <th>
                            </th>
                            <th>
                                Comments
                            </th>
                            <th>
                                Created By
                            </th>
                        </tr>
                    </tfoot>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="HFFENo" runat="server" Value="" />
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFRcvdFromDt" runat="server" Value="" />
                <asp:HiddenField ID="HFRcvdToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFDept" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                <asp:HiddenField ID="HFRegret" runat="server" Value="" />
                <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
                <asp:HiddenField ID="HFCreatedBy" runat="server" Value="" />
                <asp:HiddenField ID="HFComments" runat="server" Value="" />
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
    <script src="../JScript/jquery.jeditable.js" type="text/javascript"></script>
    <script src="../JScript/jquery.dataTables.editable.js" type="text/javascript"></script>
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
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#tblOscarNominees').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ReportService.asmx/GetStatusFERecv",
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
                                    $("#tblOscarNominees").show();
                                }
                    });
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true,

                //--- Dynamic Language---------
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search all columns:"
                },

                "oSearch": {
                    "sSearch": "",
                    "bRegex": false,
                    "bSmart": true
                }
            }).makeEditable({
                sUpdateURL: "StatusFeRecevdHandler.ashx",
                "aoColumns": [null, null, null, null, null, null, null,
                            {
                                type: 'select',
                                data: "{'Select':'Select','Regret':'Regret','UnRegret':'UnRegret'}",
                                indicator: 'Saving...',
                                loadtext: 'loading...',
                                tooltip: 'DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE',
                                onblur: 'submit'
                            },
                            {
                                type: 'textarea',
                                indicator: 'Saving...',
                                loadtext: 'loading...',
                                tooltip: 'DOUBLE CLICK ON THE RECORD TO EDIT AND CLICK ENTER TO SAVE',
                                onblur: 'submit'
                            }, null]
            });

            $("#tblOscarNominees").dataTable().columnFilter(
                {
                    //sPlaceHolder: "foot:before",
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "date-range", width: "50px" },
                                    { "type": "date-range", width: "50px" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    null, null,
                                    { "type": "text" },
                                    { "type": "text" }
                                    ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFFENo ]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate ]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFRcvdFromDt]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFRcvdToDt]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFSubject]').val(Valuee);
                }

                else if (InDex == 6) {
                    $('[id$=HFDept]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFStatus]').val(Valuee);
                }
                else if (InDex == 12) {
                    $('[id$=HFRegret]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFRemarks]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFComments]').val(Valuee);
                }
                else if (InDex == 11) {
                    $('[id$=HFCreatedBy]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#tblOscarNominees').dataTable();
        });
    </script>
    <script type="text/jscript">
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
