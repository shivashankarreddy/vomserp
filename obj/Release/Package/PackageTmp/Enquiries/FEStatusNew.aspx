<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FEStatusNew.aspx.cs" Inherits="VOMS_ERP.Enquiries.FEStatusNew"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Enquiry Status"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="right"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%;">
                                <tr>
                                    <td align="right">
                                        <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                            class="item_top_icons" Style="float: right; vertical-align: middle; margin-right: 5px;"
                                            title="Export Excel" CausesValidation="False" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table id="tblFeStatNew" cellpadding="0" cellspacing="0" border="0" class="display">
                                            <thead>
                                                <tr>
                                                    <th width="5%">
                                                        Date
                                                    </th>
                                                    <th width="10%">
                                                        FE Number
                                                    </th>
                                                    <th width="10%">
                                                        FPO Number
                                                    </th>
                                                    <th width="5%">
                                                        Recv Date
                                                    </th>
                                                    <th width="28%">
                                                        Subject
                                                    </th>
                                                    <th width="15%">
                                                        Status
                                                    </th>
                                                    <th width="5%">
                                                        Department
                                                    </th>
                                                    <th width="15%">
                                                        customer
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody class="tbody">
                                            </tbody>
                                            <tfoot>
                                                <tr>
                                                    <th style="text-align: right" colspan="4">
                                                    </th>
                                                    <th colspan="2" align="left">
                                                    </th>
                                                    <th colspan="2" align="right">
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        Date
                                                    </th>
                                                    <th>
                                                        FE Number
                                                    </th>
                                                    <th>
                                                        FPO Number
                                                    </th>
                                                    <th>
                                                        Recv Date
                                                    </th>
                                                    <th>
                                                        Subject
                                                    </th>
                                                    <th>
                                                        Status
                                                    </th>
                                                    <th>
                                                        Department
                                                    </th>
                                                    <th>
                                                        customer
                                                    </th>
                                                </tr>
                                            </tfoot>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                                        <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                                        <asp:HiddenField ID="HFFENo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRcvdFromDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFRcvdToDt" runat="server" Value="" />
                                        <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                                        <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                                        <asp:HiddenField ID="HFDept" runat="server" Value="" />
                                        <asp:HiddenField ID="HFCust" runat="server" Value="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="background-color: White; float: left; padding-top: 10px; width: 100%;
                                            border: Solid 1px black; min-height: 100px; max-height: 400px; overflow: auto;">
                                            <table align="center" cellspacing="1px">
                                                <tr>
                                                    <td>
                                                        <span>Foreign Enquiry </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Foreign Enquiry</span>
                                                            <asp:TextBox ID="txtLPK" BackColor="LightPink" Width="20px" Height="10px" runat="server"
                                                                ReadOnly="true" Style="margin: 0px 0px 0px -84px;" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Local Enquiry </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Local Enquiry</span>
                                                            <asp:TextBox ID="txtORG" BackColor="Orange" Width="20px" Height="10px" title="" runat="server"
                                                                ReadOnly="true" Style="margin: 0px 0px 0px -84px;" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Local Quotation </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Local Quotation</span>
                                                            <asp:TextBox ID="txtKHAKI" BackColor="Khaki" Width="20px" Height="10px" title=""
                                                                runat="server" ReadOnly="true" Style="margin: 0px 0px 0px -84px;" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Foreign Quotation </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%; right: 157px;">Foreign Quotation</span>
                                                            <asp:TextBox ID="txtGRY" BackColor="Gray" Width="20px" Height="10px" title="" runat="server"
                                                                ReadOnly="true" Style="margin: 0px 0px 0px -84px;" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span>Foreign Purchase Order </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Foreign Purchase Order</span>
                                                            <asp:TextBox ID="txtLBU" BackColor="LightBlue" Width="20px" Height="10px" title=""
                                                                runat="server" ReadOnly="true" Style="margin: 0px 0px 0px -84px;" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Local Purchase Order </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Local Purchase Order</span>
                                                            <asp:TextBox ID="txtLGN" BackColor="LawnGreen" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Request for P.Invoice (CEE) </span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Request for P.Invoice (CEE)</span>
                                                            <asp:TextBox ID="txtDISQ" BackColor="Bisque" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>IOM Template</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%; right: 157px;">IOM Template</span>
                                                            <asp:TextBox ID="txtCDTBL" BackColor="CadetBlue" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span>CT-1 Generation Form</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">CT-1 Generation Form</span>
                                                            <asp:TextBox ID="txtDRKH" BackColor="DarkKhaki" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Goods Delivery Note (GDN)</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Goods Delivery Note (GDN)</span>
                                                            <asp:TextBox ID="txtDRKCYN" BackColor="DarkCyan" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Goods Receipt Note (GRN)</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Goods Receipt Note (GRN)</span>
                                                            <asp:TextBox ID="txtGLD" BackColor="Gold" Width="20px" title="" Height="10px" runat="server"
                                                                Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Shipment Planning</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%; right: 157px;">Shipment Planning</span>
                                                            <asp:TextBox ID="txtIVRY" BackColor="Ivory" title="" Width="20px" Height="10px" runat="server"
                                                                Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span>Shipment Invoice</span>
                                                    </td>
                                                    <td width="11%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%">Shipment Invoice</span>
                                                            <asp:TextBox ID="txtLTSTLBL" BackColor="LightSteelBlue" title="" Width="20px" Height="10px"
                                                                Style="margin: 0px 0px 0px -84px;" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>Shipping Bill Details</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%; margin: -44px 0px 0px 0px; right: -47px;">Shipping
                                                            Bill Details</span>
                                                            <asp:TextBox ID="txtOrchd" BackColor="Orchid" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>CT-1 Task Details</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%;">CT-1 Task Details</span>
                                                            <asp:TextBox ID="txtSienna" BackColor="Sienna" Width="20px" title="" Height="10px"
                                                                runat="server" Style="margin: 0px 0px 0px -84px;" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                    <td>
                                                        <span>POE/UnUtilized Amount Remit</span>
                                                    </td>
                                                    <td width="10%">
                                                        <a class="has-tooltip" style="cursor: default" href="javascript:void(0)"><span class="tooltip"
                                                            style="float: left; width: 115%; margin: -44px 0px 0px 0px; right: 118px;">POE/UnUtilized
                                                            Amount Remit</span>
                                                            <asp:TextBox ID="txtTurquoise" BackColor="Turquoise" title="" Width="20px" Height="10px"
                                                                Style="margin: 0px 0px 0px -84px;" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
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
        a[title].tooltip
        {
            overflow: hidden;
            width: 45px;
            height: 90px;
        }
        
        .tooltip
        {
            overflow: hidden;
            display: inline-block;
            position: relative;
        }
        
        .has-tooltip
        {
            color: #555;
            font-size: 16px;
            display: block; /* margin: 150px 75px 10px 75px;*/
            padding: 5px 5px;
            position: relative;
            text-align: center;
            width: 100px;
            -webkit-transform: translateZ(0); /* webkit flicker fix */
            -webkit-font-smoothing: antialiased; /* webkit text rendering fix */
        }
        
        .has-tooltip .tooltip
        {
            background: #008000;
            top: 10%;
            color: #fff;
            display: block;
            right: -10px;
            margin: 0px -40px 0px 0px;
            border-radius: 5px;
            opacity: 0; /* padding: 5px;*/
            position: absolute;
            visibility: hidden;
            width: 70%;
            -webkit-transform: translateY(10px);
            -moz-transform: translateY(10px);
            -ms-transform: translateY(10px);
            -o-transform: translateY(10px);
            transform: translateY(10px);
            -webkit-transition: all .25s ease-out;
            -moz-transition: all .25s ease-out;
            -ms-transition: all .25s ease-out;
            -o-transition: all .25s ease-out;
            transition: all .25s ease-out;
            -webkit-box-shadow: 2px 2px 6px rgba(0, 0, 0, 0.28);
            -moz-box-shadow: 2px 2px 6px rgba(0, 0, 0, 0.28);
            -ms-box-shadow: 2px 2px 6px rgba(0, 0, 0, 0.28);
            -o-box-shadow: 2px 2px 6px rgba(0, 0, 0, 0.28);
            box-shadow: 2px 2px 6px rgba(0, 0, 0, 0.28);
        }
        
        .has-tooltip .tooltip:before
        {
            bottom: -20px;
            content: " ";
            display: block;
            height: 20px;
            left: 0;
            position: absolute;
            width: 70%;
        }
        
        .has-tooltip .tooltip:after
        {
            overflow: hidden; /*border-left: solid transparent 10px;
            border-right: solid transparent 10px;*/
            white-space: nowrap;
            border-top: solid #008000 10px;
            bottom: -10px;
            content: " ";
            height: 0;
            left: 50%;
            margin-left: -13px;
            position: absolute;
            width: 0;
        }
        
        .has-tooltip:hover .tooltip
        {
            opacity: 1;
            visibility: visible;
            -webkit-transform: translateY(0px);
            -moz-transform: translateY(0px);
            -ms-transform: translateY(0px);
            -o-transform: translateY(0px);
            transform: translateY(0px);
        }
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
            oTable = $('#tblFeStatNew').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [],
                "bSort": false,
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "WebService1.asmx/GetFeNew",
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
                                    $("#tblFeStatNew").show();
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
                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex) {

                    switch (aData[5]) {
                        case "Foreign Enquiry Received": $(nRow).addClass('FE_LightPink'); break;
                        case "Local Enquiry Sent": $(nRow).addClass('LE_Orange'); break;
                        case "Local Quotation Received": $(nRow).addClass('LQ_Khaki'); break;
                        case "Foreign Quotation Submitted": $(nRow).addClass('FQ_Gray'); break;
                        case "FPO Received": $(nRow).addClass('FPO_LightBlue'); break;
                        case "LPO's Issued": $(nRow).addClass('LPO_LawnGreene'); break;
                        case "Cenral Excise Excemption Detials Requested": $(nRow).addClass('RqstPINV_Bisque'); break;
                        case "IOM Template": $(nRow).addClass('IOMTemplate_CadetBlue'); break;
                        case "CT-1 Generation": $(nRow).addClass('CTOne_DarkKhaki'); break;
                        case "Goods Delivered by Vendor": $(nRow).addClass('GDN_DarkCyan'); break;
                        case "Goods Received": $(nRow).addClass('GRN_Gold'); break;
                        case "Shipment Planning Prepared": $(nRow).addClass('CheckList_Ivory'); break;
                        case "Shipment Invoice Number Updated": $(nRow).addClass('ShipmentInv_LightSteelBlue'); break;
                        case "Shipping Bill Prepared": $(nRow).addClass('ShpngBillDtils_Orchid'); break;
                        case "CT-1 Task Details Prepared": $(nRow).addClass('CTOneTaskdtls_Sienna'); break;
                        case "POE/UnUtilized Amount Remitted": $(nRow).addClass('POEUnutilisedAMT_Turquoise'); break;
                        default: ;
                    }
                }
            });

            $("#tblFeStatNew").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                      null, null, null
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
                    $('[id$=HFFENo]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFRcvdFromDt]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFRcvdToDt]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFSubject]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFStatus]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFDept]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFCust]').val(Valuee);
                }
            });

            /* Init the table */
            oTable = $('#tblFeStatNew').dataTable();
        });
    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            $('[id$=txtfromdt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
            $('[id$=txttodt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        });
    
    </script>
</asp:Content>
