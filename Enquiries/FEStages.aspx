<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FEStages.aspx.cs" Inherits="VOMS_ERP.Enquiries.FEStages" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Foreign Enquiry Stages"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="right"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%;">
                                <tr style="display: none">
                                    <td align="right">
                                        <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                            class="item_top_icons" Style="float: right; vertical-align: middle; margin-right: 5px;"
                                            title="Export Excel" CausesValidation="False"></asp:ImageButton>
                                        <%--OnClick="btnExcelExpt_Click"--%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table style="width: 90%;">
                                            <tr>
                                                <td>
                                                    <span style="font-weight: bold;">Customer Name: &nbsp;&nbsp;&nbsp;</span><asp:Label
                                                        runat="server" ID="lblEnquiryNo">...</asp:Label>
                                                    <%--<input type="button" onclick="OpenPopup(2,this)" value="try" />
                                                        <a href="javascript:void(0)" id="OpenDialog" onclick="OpenPopup(2,this)">KC/8/PUR/F3/SMS/084</a>--%>
                                                </td>
                                                <%--                                                <td style="text-align:left;">
                                                    
                                                </td>--%>
                                                <td>
                                                    <span style="font-weight: bold;">Subject:&nbsp;&nbsp;&nbsp;</span><asp:Label runat="server"
                                                        ID="lblEnquirySubject">...</asp:Label>
                                                </td>
                                                <%--<td style="text-align:left;">
                                                    
                                                </td>--%>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div runat="server" id="divtable">
                                        </div>
                                        <%--<table  id="tblFeStaging" cellpadding="0" cellspacing="0" border="0" class="display">
                                            <thead>
                                                <tr>
                                                    <th>
                                                        Enquiry Stages
                                                    </th>
                                                    <th>
                                                        Reference
                                                    </th>
                                                    <th>
                                                        Date Initation
                                                    </th>
                                                    <th>
                                                        Target Date
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody class="tbody">
                                            </tbody>
                                            <tfoot>                                               
                                                <tr>
                                                    <th>
                                                        Enquiry Stages
                                                    </th>
                                                    <th>
                                                        Reference
                                                    </th>
                                                    <th>
                                                        Date Initation
                                                    </th>
                                                    <th>
                                                        Target Date
                                                    </th>
                                                </tr>
                                            </tfoot>
                                        </table>--%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--<tr>
            <td align="right" class="">
                <div id="Div1" class="bcButtonDiv bcTdButton" style="width:100px;">
                    <asp:HyperLink runat="server" ID="btnBack_TOP" Text="Back" NavigateUrl="~/Enquiries/FEStatusNew.aspx" />
                </div>
            </td>
        </tr>--%>
        <tr>
            <td>
                <div id="dialog" title="Dialog Title">
                    <div id="dialogcontent" />
                </div>
            </td>
        </tr>
    </table>
    <div name="loader" id="loader" class="loader loader-curtain" data-curtain-text="Loading..."
        data-colorful data-shadow data-blink>
    </div>
    <%--<div class="loader loader-default" data-text="Loading..." data-blink></div>--%>
    <%--<script src="https://code.jquery.com/jquery-3.3.1.js" type="text/javascript"></script>--%>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <%--<script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>--%>
    <link href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" rel="stylesheet"
        type="text/css" />
    <link href="https://cdn.datatables.net/1.10.20/css/dataTables.jqueryui.min.css" rel="stylesheet"
        type="text/css" />
    <link href="https://cdn.datatables.net/buttons/1.6.0/css/buttons.dataTables.min.css"
        rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/css-loader.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-curtain.css" rel="stylesheet" type="text/css" />
    <%--<link href="../css/loading.css" rel="stylesheet" type="text/css" />--%>
    <%--    <link href="../css/loaderCSS/preloader.css" rel="stylesheet" type="text/css" />--%>
    <%--<link href="../css/loaderCSS/loader-ball.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-smartphone.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-bar-ping-pong.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-bar.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-border.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-bouncing.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-clock.css" rel="stylesheet" type="text/css" />
    
    <link href="../css/loaderCSS/loader-default.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-double.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-music.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-pokeball.css" rel="stylesheet" type="text/css" />--%>
    <%--<script src="../JScript/jquery.loading.min.js" type="text/javascript"></script>--%>
    <script src="../JScript/jquery.preloader.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/1.10.20/js/dataTables.jqueryui.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/buttons/1.6.0/js/dataTables.buttons.min.js"
        type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"
        type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"
        type="text/javascript"></script>
    <script src="https://cdn.datatables.net/buttons/1.6.0/js/buttons.html5.min.js" type="text/javascript"></script>

    <style>
        div.dt-buttons
        {
            position: relative;
            float: right !important;
        }
        .btncolor
        {
            color: #2680ba !important;
            font-weight: bold;
            margin-top: 3px;
        }
        .tooltip
        {
            position: relative;
            display: inline-block;
            border-bottom: 1px dotted black;
        }
        .tooltip .tooltiptext
        {
            visibility: hidden;
            width: auto;
            text-align: center;
            border: 1px solid #9bc800;
            padding: 10px;
            white-space: nowrap;
            border-radius: 6px;
            color: #000000;
            cursor: default;
            background-color: #f0ffb9; /* Position the tooltip */
            position: absolute;
            z-index: 2147483647;
        }
        .tooltip:hover .tooltiptext
        {
            visibility: visible;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {

            //            $('#tblFeStaging tbody').append("<tr><td>Foreign Enquiry</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Local Enquiry</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Local Quotation</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Foreign Quotation</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Foreign Purchase Order</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Local Purchase Order</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Dispatch Instructions</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Goods Dispatch Note</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Goods Receipt Note</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Shipment Planning Details</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Shipment Proforma Invoice</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Packing List</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Shipping Bill Details</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>AirWay Bill</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Bill of Lading</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>Mate Receipt</td><td>2</td><td>3</td><td>4</td></tr>")
            //                                    .append("<tr><td>E-BRC Details</td><td>2</td><td>3</td><td>4</td></tr>");


            var buttonExcel = {
                exportOptions: {
                    format: {
                        body: function (data, row, column, node) {
                            // Strip $ from salary column to make it numeric
                            //return column === 3 ?data.replace(/[12:00:00 AM]/g, '') :data;
                            var fdata = "";
                            if (column === 3)
                                fdata = node.innerText; //.replace("12:00:00 AM","");
                            else
                                fdata = node.innerText;
                            return fdata;
                        }
                    }
                }
            };

            var custName = $("#ctl00_ContentPlaceHolder1_lblEnquiryNo").text();
            var Subject = $("#ctl00_ContentPlaceHolder1_lblEnquirySubject").text();

            $('#tblFeStaging').DataTable({
                "ordering": false,
                //  "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
                "pageLength": 25,
                //dom: 'Blrtip',
                dom: '<"topbuttons"B>frt<"bottombuttons">',
                initComplete: function () {
                    $(".bottombuttons").append($(".topbuttons").clone(true));
                },
                buttons: [
                        $.extend(true, {}, buttonExcel, {
                            extend: 'excelHtml5',
                            title: "Foreign Enquiry Stages",
                            messageTop: 'Customer Name: ' + custName + ',           Subject: ' + Subject,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $('row:gt(2) c', sheet).attr('s', '55');  //<-- wrapped text
                                //$('row c[r*="B"]', sheet).attr('s', '25');// Borders                                
                                //$('row c[r^="A1"]', sheet).attr('s', '31'); // Bold First Column in First Row
                                //$('row:not(:eq(1)) c[r^=A]', sheet).attr('s', '2'); // Bold First Column                   
                                $('row:gt(1) c[r^=A]', sheet).attr('s', '2'); // Bold First Column
                                $('row:eq(2) c', sheet).attr('s', '32');
                            }
                        }),
                        {
                            extend: 'pdfHtml5',
                            title: "Foreign Enquiry Stages",
                            //orientation: 'landscape',
                            //orientation: 'portrait',
                            pageSize: 'A4',
                            //messageTop:'This is a sample message on top of the table.',
                            exportOptions: {
                                //columns: 'th:not(:last-child)',
                                //search: 'applied',
                                //order: 'applied'
                                stripNewlines: false
                            },
                            customize: function (doc) {
                                var rdoc = doc;
                                var rcout = doc.content[doc.content.length - 1].table.body.length - 1;
                                doc.content.splice(0, 1);
                                //Create a date string that we use in the footer. Format is dd-mm-yyyy
                                var now = new Date();
                                var jsDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + '  and Time:' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();

                                //var logo = getBase64FromImageUrl('http://localhost:62857/Admin/ShowImg.ashx?id=6cedfcec-d633-45d5-b12c-8290a285dc76');

                                doc.pageMargins = [30, 70, 30, 30]; // LEFT, TOP,  RIGHT, BOTTOM,
                                doc.defaultStyle.fontSize = 8;
                                doc.styles.tableHeader.fontSize = 9;
                                for (var i = 0; i < rcout; i++) {
                                    //doc.content[doc.content.length - 1].table.body[(i + 1)][0].style = { bold: true }
                                    var obj = doc.content[doc.content.length - 1].table.body[i + 1];
                                    doc.content[doc.content.length - 1].table.body[(i + 1)][0] = { text: obj[0].text, style: [obj[0].style], bold: true };
                                }

                                doc['header'] = (function (page, pages) {
                                    return {
                                        table: {
                                            widths: ['100%'],
                                            headerRows: 0,
                                            body: [
                                    [{ text: 'Foreign Enquiry Stages', alignment: 'center', fontSize: 14, bold: true, margin: [0, 10, 0, 0]}],
                                    [
                                        {
                                            text:
                                                [
                                                    { text: 'Customer Name: ', bold: true }, custName + '\n',
                                                    { text: '      Subject: ', bold: true }, Subject,
                                                ]
                                        }
                                    ]
                                ]
                                        },
                                        layout: 'noBorders',
                                        margin: 10
                                    }
                                });
                                doc['footer'] = (function (page, pages) {
                                    return {
                                        columns: [
                                                    {
                                                        alignment: 'left',
                                                        text: ['Created Date: ', { text: jsDate.toString()}]
                                                    },
                                                    {
                                                        alignment: 'center',
                                                        text: 'Total ' + rcout.toString() + ' rows'
                                                    },
                                                    {
                                                        alignment: 'right',
                                                        text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString()}]
                                                    }
                                        ],
                                        margin: 10
                                    }
                                });

                                var objLayout = {};
                                objLayout['hLineWidth'] = function (i) { return .8; };
                                objLayout['vLineWidth'] = function (i) { return .5; };
                                objLayout['hLineColor'] = function (i) { return '#aaa'; };
                                objLayout['vLineColor'] = function (i) { return '#aaa'; };
                                objLayout['paddingLeft'] = function (i) { return 5; };
                                objLayout['paddingRight'] = function (i) { return 35; };
                                doc.content[doc.content.length - 1].layout = objLayout;

                            }
                        },
                        {
                            name: 'mybutton',
                            text: "Back",
                            className: 'btncolor',
                            //background:'green',
                            action: function (e, dt, node, config) {
                                window.location.href = "FEStatusNew.aspx";
                            }
                        },
                    ]
            });
        });
    </script>
    <script type="text/javascript">
        var windowWidth = $(window).width();
        var windowHeight = $(window).height();
        $(document).ready(function () {
            $("OpenDialog").click(function () {
                $("#dialog").dialog({ modal: true, height: 590, width: 1005 });
            });

            OpenPopup = function (seq, sss) {
                $("#dialog").attr('title', "");

                $('#loader').addClass('is-active');
                //                var loader = document.getElementById("loader");
                //                loader.classList.add("is-active");

                //                $("body").loading({
                //                    stoppable: true,
                //                    message: "Body loading, another message...",
                //                    theme: "dark"
                //                });

                var title = $(sss)[0].parentNode.parentNode.firstChild.innerHTML;
                $("#dialog").attr('title', title);
                var txt = sss.text.trim();
                var ret = FEStages.GetValueByID(txt, seq);

                $(".loader").removeClass('is-active');
                //                $('body').on('loading.stop', function (event, loadingObj) {
                //                    // do something whenever the loading state of #my-element is turned off
                //                });

                $("#dialogcontent").html('');
                $("#dialogcontent").html(ret.value);
                $("#dialog").dialog({
                    modal: true,
                    title: title,
                    //height: windowHeight - 150, 
                    height: "auto",
                    width: windowWidth - 100,
                    open: function (event, ui) {
                        $('.ui-widget-overlay').css({ opacity: '.9' });
                    }
                });
            }
        });
    </script>
</asp:Content>
