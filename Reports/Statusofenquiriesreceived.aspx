<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" 
    CodeBehind="Statusofenquiriesreceived.aspx.cs" Inherits="VOMS_ERP.Reports.Statusofenquiriesreceived" %>
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
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table style="width: 45%;">
                                            <tr>
                                                <td>
                                                    <span style="font-weight: bold;">From Date</span>                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtFromDate" Text="" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate(this.id);" Width="80px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <span style="font-weight: bold;">To Date</span>                                                    
                                                </td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtToDate" Text="" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate(this.id);" Width="80px"></asp:TextBox>
                                                </td>
                                                <td>
                                                <%--style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; color: #FFFFFF; text-shadow: 0 1px #287B9D;" --%>
                                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"                                                     
                                                    onclick="btnSubmit_Click"/>
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
                                        <div runat="server" id="divtable" style="overflow:hidden !important;" class="hidescrolling">
                                        </div>
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
                <div id="dialog" title="Dialog Title">
                    <div id="dialogcontent" />
                </div>
            </td>
        </tr>
    </table>
    <div name="loader" id="loader" class="loader loader-curtain" data-curtain-text="Loading..."
        data-colorful data-shadow data-blink>
    </div>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <link href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="https://cdn.datatables.net/1.10.20/css/dataTables.jqueryui.min.css" rel="stylesheet" type="text/css" />
    <link href="https://cdn.datatables.net/buttons/1.6.0/css/buttons.dataTables.min.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/css-loader.css" rel="stylesheet" type="text/css" />
    <link href="../css/loaderCSS/loader-curtain.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery.preloader.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/1.10.20/js/dataTables.jqueryui.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/buttons/1.6.0/js/dataTables.buttons.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/buttons/1.6.0/js/buttons.html5.min.js" type="text/javascript"></script>

    <style>
        .MainTable 
        {
            margin-left: 24px;
        }        
        #tblFeStaging_wrapper 
        {
            overflow:inherit !important;    
        }
        div.hidescrolling
        {            
            overflow: hidden !important;
            -ms-overflow-style: none; /* for Internet Explorer, Edge */
             scrollbar-width: none; /* for Firefox */
        }
        ::-webkit-scrollbar { 
            display: none !important; /* for Chrome, Safari, and Opera */
        }
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
            var swidth = $(window).width();
            $(".content, .hidescrolling").css('width', swidth - 30 + 'px !important');
            var FromDate = $('[id$=txtFromDate]').val();
            var ToDate = $('[id$=txtToDate]').val();

            var dateToday = new Date();
            $('[id$=txtFromDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday,
                onSelect: function (date) {
                    var strdateEnqDT = $('[id$=txtFromDate]').val();
                    var strdateEnqDT1 = strdateEnqDT.split('-');
                    strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
                    strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
                    $('[id$=txtToDate]').datepicker('option', {
                        minDate: new Date(strdateEnqDT),
                        maxDate: dateToday,
                        dateFormat: 'dd-mm-yy',
                        changeMonth: true,
                        changeYear: true
                    });
                }
            });
            $('[id$=txtToDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday,
                onSelect: function (date) {
                    var strdateEnqDT = $('[id$=txtToDate]').val();
                    var strdateEnqDT1 = strdateEnqDT.split('-');
                    strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
                    strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
                    $('[id$=txtFromDate]').datepicker('option', {
                        //minDate: dateToday,
                        maxDate: new Date(strdateEnqDT),
                        dateFormat: 'dd-mm-yy',
                        changeMonth: true,
                        changeYear: true
                    });
                }
            });
//            function changedate(FrmDt) {
//                try {
//                    var strdate = $('[id$=' + FrmDt + ']').val();
//                    var strdate1 = strdate.split('-');
//                    strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
//                    strdate = new Date(strdate.replace(/-/g, "/"));
//                    $('[id$=txtToDt]').datepicker('option', {
//                        minDate: new Date(strdate),
//                        dateFormat: 'dd-mm-yy',
//                        changeMonth: true,
//                        changeYear: true
//                    });
//                }
//                catch (Error) {
//                    ErrorMessage(Error.message);
//                }
//            }

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


            $('#tblFeStaging').DataTable({
                "ordering": false,
                //  "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
                "pageLength": 25,
                "pagingType": "full_numbers",
                "paging": true,
                dom: 'Blrtip',
                autoWidth: true,
                "scrollY": 300,
                "scrollX": true,
                //dom: '<"topbuttons"B>frtp<"bottombuttons">',
                initComplete: function () {
                    $(".bottombuttons").append($(".topbuttons").clone(true));
                },
                buttons: [
                        $.extend(true, {}, buttonExcel, {
                            extend: 'excelHtml5',
                            title: "Status of Enquiries Received From"  + FromDate + "           ToDate " + ToDate,
                            //messageTop: 'From: ' + FromDate + ',           ToDate: ' + ToDate,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                 /* Not Working */
                                 // var sSh = xlsx.xl['styles.xml'];
                                 // var lastXfIndex = $('cellXfs xf', sSh).length - 1;
                                 // var greyBoldCentered = lastXfIndex + 2;
                                 // $('row:eq(0) c', sheet).attr('s', greyBoldCentered);  //grey background bold and centered, as added above

                                $('row:gt(1) c', sheet).attr('s', '55');  //<-- wrapped text
                                //$('row c[r*="B"]', sheet).attr('s', '25');// Borders                                
                                //$('row c[r^="A1"]', sheet).attr('s', '31'); // Bold First Column in First Row
                                //$('row:not(:eq(1)) c[r^=A]', sheet).attr('s', '2'); // Bold First Column                   
                                $('row:gt(1) c[r^=A]', sheet).attr('s', '2'); // Bold First Column
                                $('row:eq(1) c', sheet).attr('s', '32'); // Background Gray
                                //$('row:eq(0) c', sheet).attr('s', '2'); // Bold First Row
                            }
                        }),
                        {
                            extend: 'pdfHtml5',
                            title: "Status of Enquiries Received From  " + FromDate + "           ToDate " + ToDate,
                            orientation: 'landscape',
                            //orientation: 'portrait',
                            pageSize: 'A2',
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
                                //doc.defaultStyle['td:nth-child(2)'] = { width: '500px', 'max-width': '500px' }
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
                                    [{ text: 'Status of Enquiries Received', alignment: 'center', fontSize: 14, bold: true, margin: [0, 10, 0, 0]}],
                                    [
                                        {
                                            text:
                                                [
                                                    { text: 'From Date: ', bold: true, margin: [0, 10, 0, 0] }, FromDate + '\n',
                                                    { text: '      To Date: ', bold: true, margin: [0, 10, 0, 0] }, ToDate,
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
                //                        {
                //                            name: 'mybutton',
                //                            text: "Back",
                //                            className: 'btncolor',
                //                            //background:'green',
                //                            action: function (e, dt, node, config) {
                //                                //window.location.href = "FEStatusNew.aspx";
                //                                if ($("#ctl00_username").html().indexOf("PRASAD") != -1)
                //                                    window.location.href = "FeStatusOverView.aspx";
                //                                else
                //                                    window.location.href = "FEStatusNew.aspx";
                //                            }
                //                        },
                    ]
            });
        });
    </script>
    <script type="text/javascript">
//        var windowWidth = $(window).width();
//        var windowHeight = $(window).height();
//        $(document).ready(function () {
//            $("OpenDialog").click(function () {
//                $("#dialog").dialog({ modal: true, height: 590, width: 1005 });
//            });

//            OpenPopup = function (seq, sss) {
//                $("#dialog").attr('title', "");

//                $('#loader').addClass('is-active');
//                //                var loader = document.getElementById("loader");
//                //                loader.classList.add("is-active");

//                //                $("body").loading({
//                //                    stoppable: true,
//                //                    message: "Body loading, another message...",
//                //                    theme: "dark"
//                //                });

//                var title = $(sss)[0].parentNode.parentNode.firstChild.innerHTML;
//                $("#dialog").attr('title', title);
//                var txt = sss.text.trim();
//                var ret = FEStages.GetValueByID(txt, seq);

//                $(".loader").removeClass('is-active');
//                //                $('body').on('loading.stop', function (event, loadingObj) {
//                //                    // do something whenever the loading state of #my-element is turned off
//                //                });

//                $("#dialogcontent").html('');
//                $("#dialogcontent").html(ret.value);
//                $("#dialog").dialog({
//                    modal: true,
//                    title: title,
//                    //height: windowHeight - 150, 
//                    height: "auto",
//                    width: windowWidth - 100,
//                    open: function (event, ui) {
//                        $('.ui-widget-overlay').css({ opacity: '.9' });
//                    }
//                });
//            }
//        });
    </script>
</asp:Content>
