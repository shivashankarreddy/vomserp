<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FETracker.aspx.cs" Inherits="VOMS_ERP.Enquiries.FETracker" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 100%; vert1ical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Enquiry Tracker"
                                            CssClass="bcTdTitleLabel"></asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td style="text-align: right;" colspan="6">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
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
                                        <span id="lblCustName" class="bcLabel">Mail Forwarded Enquiry Nos:<font color="red"
                                            size="2"><b>*</b></font>:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFENo" runat="server" TextMode="MultiLine" onblur="FE_ChangedEvent()"
                                            onchange="FE_ChangedEvent()" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                        <asp:HiddenField ID="Txt_MFFESearchNo" runat="server" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">Mail Forwarded Enquiry Date:
                                            <br />
                                            (DD-MM-YYYY) </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFEDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabelright">Customer Name </span>
                                    </td>
                                    <td class="bcTdnormal">                                        
                                        <asp:DropDownList ID="DDL_Customer" runat="server" CssClass="bcAsptextbox">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                   <td></td>
                                   <td> </td>
                                    <td align="center" colspan="1" valign="middle" class="bcTdButton">
                                        <div id="Div1" class="bcButtonDiv" style="width:10;">
                                            <asp:LinkButton runat="server" ID="Btn_Submit" Text="Generate Report" OnClick="Btn_Submit_Click"
                                                OnClientClick="javascript:return Validations()" />
                                        </div>
                                    </td>                                    
                                    <td></td>
                                    <td></td>
                                    <td colspan="0">
                                        <div runat="server" id="dvexport">
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../images/EXCEL.png"
                                                class="item_top_icons" title="Export Excel" OnClick="BtnExcelExpt_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <%--<div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvTracker1" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                        border="0">
                        <thead>
                            <tr>
                                <th width="10%">
                                    Enquiry No.
                                </th>
                                <th width="10%">
                                    FE Received Date.
                                </th>
                                <th width="10%">
                                    Present Status
                                </th>
                                <th width="18%">
                                    Days to Create FE
                                </th>
                                <th width="10%">
                                    Days to Create LE
                                </th>
                                <th width="05%">
                                    Days to Receive LQ
                                </th>
                                <th width="05%">
                                    No  of LQ Received
                                </th>
                                <th width="20%">
                                    Days to Create FQ
                                </th>
                                <th width="20%">
                                    Total Days to reach FQ
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>                        
                    </table>
                </div>--%>
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
        .dataTables_scroll
        {
            overflow-x: auto;
            overflow-y: visible;
        }
    </style>
    <%--<script type="text/javascript">--%>
       <%-- $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });
        var oTable = null;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            oTable = $("[id$=gvTracker1]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_", //of _TOTAL_ records
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
                "sAjaxSource": "FETracker.aspx/GetData",
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
                "sScrollXInner": "110%",
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
                                    $("#gvTracker1").show();
                                }
                    });
                }
            });

            $("#gvTracker1").dataTable().columnFilter(
                {
                    "aoColumns": [
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

            /* Init the table */
            oTable = $("#gvTracker1").dataTable();
        });


        
    </script>--%>
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
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=Txt_MFFEDate]').datepicker({
                dateFormat: 'mm-dd-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });

        });

        function FE_ChangedEvent() {
            var data = $('[id$=Txt_MFFENo]').val();
            $('[id$=Txt_MFFESearchNo]').val('');
            if (data != '') {
                var Cdata = data.toLowerCase().split(',');
                var i = Cdata.length;
                while (i--) {
                    if (Cdata.indexOf(Cdata[i]) != i) {
                        Cdata.splice(i, 1);
                    }
                }
                $('[id$=Txt_MFFENo]').val(Cdata.join())
                for (var i = 0; i < Cdata.length; i++) {
                    Adata = Cdata[i]; //.split('-');
                    var MFSN = $('[id$=Txt_MFFESearchNo]').val();
                    if (MFSN != '') {
                        MFSN = MFSN + ',';
                    }
                    if (Adata != null && Adata != '' && Adata.length > 1) {

                        $('[id$=Txt_MFFESearchNo]').val(MFSN + '%' + Cdata[i]);
                    }
                    else {
                        $('[id$=Txt_MFFESearchNo]').val(MFSN + '%' + Cdata[i]);
                    }
                }
            }
        }
        function FPO_ChangedEvent() {
            var data = $('[id$=Txt_MFFPONO]').val();
            $('[id$=Txt_MFFPOSearchFPO]').val('');
            if (data != '') {
                var Cdata = data.toLowerCase().split(',');
                var i = Cdata.length;
                while (i--) {
                    if (Cdata.indexOf(Cdata[i]) != i) {
                        Cdata.splice(i, 1);
                    }
                }
                $('[id$=Txt_MFFPONO]').val(Cdata.join())
                for (var i = 0; i < Cdata.length; i++) {
                    Adata = Cdata[i].split('-');
                    var MFSN = $('[id$=Txt_MFFPOSearchFPO]').val();
                    if (MFSN != '') {
                        MFSN = MFSN + ',';
                    }
                    if (Adata != null && Adata != '' && Adata.length > 1) {

                        $('[id$=Txt_MFFPOSearchFPO]').val(MFSN + '%' + Adata[1]);
                    }
                    else {
                        $('[id$=Txt_MFFPOSearchFPO]').val(MFSN + '%' + Adata[0]);
                    }
                }
            }
        }
        function Validations() {
            if (($('[id$=Txt_MFFENo]').val()).trim() != '') {
                if (($('[id$=Txt_MFFEDate]').val()).trim() == '') {
                    ErrorMessage('Mail Forward FE Date is Required.');
                    $('[id$=Txt_MFFEDate]').focus();
                    return false;
                }
            }
        }
    </script>
</asp:Content>
