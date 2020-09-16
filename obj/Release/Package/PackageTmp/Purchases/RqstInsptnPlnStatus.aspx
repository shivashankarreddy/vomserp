<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="RqstInsptnPlnStatus.aspx.cs" Inherits="VOMS_ERP.Purchases.RqstInsptnPlnStatus"
    EnableEventValidation="false"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table width="100%">
                    <tr class="bcTRTitleRow">
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Request for Inspection Plan Status"
                                CssClass="bcTdTitleLabel"></asp:Label>
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
                            <th width="15%">
                                Customer Name
                            </th>
                            <th width="15%">
                                FPO Number(s)
                            </th>
                            <th width="15%">
                                LPO Number(s)
                            </th>
                            <th width="15%">
                                Supplier Name
                            </th>
                            <th width="11%">
                                Request Number
                            </th>
                            <th width="5%">
                                Request Date
                            </th>
                            <th width="15%">
                                Status
                            </th>                           
                            <th width="3%">
                                M
                            </th>
                            <th width="3%">
                                E
                            </th>
                            <th width="3%">
                                D
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
                                Customer Name
                            </th>
                            <th>
                                FPO Number(s)
                            </th>
                            <th>
                                LPO Number(s)
                            </th>
                            <th>
                                Supplier Name
                            </th>
                            <th>
                                Request Number
                            </th>
                            <th>
                                Request Date
                            </th>                            
                            <th>
                                Status
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            <th>
                                &nbsp;
                            </th>
                        </tr>
                    </tfoot>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFFPO" runat="server" Value="" />
                <asp:HiddenField ID="HFLPO" runat="server" Value="" />
                <asp:HiddenField ID="HFSupp" runat="server" Value="" />
                <asp:HiddenField ID="HFReqNo" runat="server" Value="" />                
                <asp:HiddenField ID="HFReqFDate" runat="server" Value="" />
                <asp:HiddenField ID="HFReqTDate" runat="server" Value="" />                
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
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
                "sAjaxSource": "Purchases_WebService.asmx/GetRqstInspnPlan",
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
            });

            $("#tblOscarNominees").dataTable().columnFilter(
                {
                    "aoColumns": [{ "type": "text" },
                                  { "type": "text" },
                                  { "type": "text" },
                                  { "type": "text" },
                                  { "type": "text" },
                                  { "type": "date-range" },
                                  { "type": "text" },
                                      null, null, null]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFPO]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFLPO]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFSupp]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFReqNo]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFReqFDate]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFReqTDate]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });
            
            /* Init the table */
            oTable = $('#tblOscarNominees').dataTable();
        });

        function mailsDetails(valddd) {
            try {
                window.location.replace("../Masters/EmailSend.aspx?IPID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }
        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = RqstInsptnPlnStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Purchases/InspectionPlanRequest.Aspx?ID=" + valddd.parentNode.parentNode.id);
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = RqstInsptnPlnStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                        //ErrorMessage('Cannot Delete this Record, LE already created so delete LE/ Error while Deleting ' + valddd.parentNode.parentNode.id + '.');
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }

        

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