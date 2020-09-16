<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="Tracker.aspx.cs" Inherits="VOMS_ERP.Tracker" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="overflow:auto">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="lblTitle" runat="server" Text="Tracker" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>
                                            fields are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 5px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span3" class="bcLabelright">Mail Forwarded FE Nos:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFENo" runat="server" TextMode="MultiLine" onblur="FE_ChangedEvent()"
                                            onchange="FE_ChangedEvent()" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">Mail Forwarded FE Date:
                                            <br />
                                            (DD-MM-YYYY) </span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFEDate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span2" class="bcLabelright">Search FE No:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFESearchNo" runat="server" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal">
                                        <span id="Span5" class="bcLabelright">Mail Forwarded FPO Nos:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFPONO" runat="server" TextMode="MultiLine" onblur="FPO_ChangedEvent()"
                                            onchange="FPO_ChangedEvent()" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span6" class="bcLabelright">Mail Forwarded FPO Date:<br />
                                            (DD-MM-YYYY)</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFPODate" runat="server" CssClass="bcAsptextbox"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span id="Span7" class="bcLabelright">Search FPO No:</span>
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox ID="Txt_MFFPOSearchFPO" runat="server" TextMode="MultiLine" CssClass="bcAsptextboxmulti"></asp:TextBox>
                                    </td>
                                </tr>
                                <td colspan="8" align="right">
                                    <center>
                                        <table border="0" cellpadding="0" width="30%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="Btn_Submit" Text="Generate Report" OnClick="Btn_Submit_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <%-- <asp:LinkButton runat="server" ID="Btn_Clear" OnClientClick="Javascript:clearAll()"
                                                                    Text="Clear" OnClick="Btn_Clear_Click" />--%>
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </center>
                                </td>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td style="text-align: right;">
                                        <asp:ImageButton ID="IMG_Btn_Export" runat="server" ImageUrl="../images/EXCEL.png"
                                            title="Export Excel" OnClick="IMG_Btn_Export_Click"></asp:ImageButton>
                                    </td>
                                </tr>
                                <tr>
                                    <div style="overflow: auto;">
                                        <td colspan="6">
                                            <table id="Tracker_Grid" class="widthFull fontsize10 displayNone" cellpadding="0"
                                                cellspacing="0" border="0" width="100%">
                                                <thead>
                                                    <tr>
                                                        <th id="FEID" runat="server" visible="false">
                                                        </th>
                                                        <th>
                                                            Enquiry No
                                                        </th>
                                                        <th>
                                                            FE Recieved Date
                                                        </th>
                                                        <th>
                                                            FE Created Date
                                                        </th>
                                                        <th>
                                                            FE No
                                                        </th>
                                                        <th>
                                                            FQ No
                                                        </th>
                                                        <th>
                                                            FQ Created Date
                                                        </th>
                                                        <th>
                                                            FPO Recieved Mail
                                                        </th>
                                                        <th>
                                                            FPO Created in System
                                                        </th>
                                                        <th>
                                                            FPO Recieved Date
                                                        </th>
                                                        <th>
                                                            FPO Created Date
                                                        </th>
                                                        <th>
                                                            LPO NO
                                                        </th>
                                                        <th>
                                                            LPO Created Date
                                                        </th>
                                                        <th>
                                                            No Of Days to Create FE
                                                        </th>
                                                        <th>
                                                            No of Days to Float FE to FQ
                                                        </th>
                                                        <th>
                                                            No of Days FQ to FPO
                                                        </th>
                                                        <th>
                                                            No of Days FPO to LPO
                                                        </th>
                                                        <th>
                                                            Status
                                                        </th>
                                                        <th>
                                                            Created By
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                </tbody>
                                            </table>
                                        </td>
                                    </div>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </div>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <link href="../css/style.css" rel="stylesheet" type="text/css" />
    <link href="../css/nprogress.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/nprogress.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery.expander.js" type="text/javascript"></script>
    <script type="text/javascript">
        var oTable;
        $(document).ready(function () {

            /*          Main Functionality       */
            oTable = $('#Tracker_Grid').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "Tracker.aspx/GetData",
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
                                    $("#Tracker_Grid").show();
                                }
                    });
                },
                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%", //true,
                //"sScrollXInner": "110%",
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
                    "sSearch_1": "",
                    "bRegex": false,
                    "bSmart": true
                }
            });
            oTable = $('#Tracker_Grid').dataTable();


            $('[id$=Txt_MFFEDate]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
            });
        });
        $('[id$=Txt_MFFPODate]').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
            , showAnim: 'blind'
            , showButtonPanel: true
        });
        function fnExcelReport() {
            var tab_text = "<table><tr>";
            var textRange; var j = 0;
            tab = document.getElementById('ExportReport'); // id of table
            for (j = 0; j < tab.rows.length; j++) {
                tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
                //tab_text=tab_text+"</tr>";
            }
            tab_text = tab_text + "</table>";
            tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, ""); //remove if u want links in your table
            tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
            tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");
            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
            {
                txtArea1.document.open("txt/html", "replace");
                txtArea1.document.write(tab_text);
                txtArea1.document.close();
                txtArea1.focus();
                sa = txtArea1.document.execCommand("SaveAs", true, "Say Thanks to Sumit.xls");
            }
            else                 //other browser not tested on IE 11
                sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text)); //'data:application/vnd.ms-excel,' + encodeURIComponent(tab_text)


            return (sa);
        }

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
                    Adata = Cdata[i].split('-');
                    var MFSN = $('[id$=Txt_MFFESearchNo]').val();
                    if (MFSN != '') {
                        MFSN = MFSN + ',';
                    }
                    if (Adata != null && Adata != '' && Adata.length > 1) {

                        $('[id$=Txt_MFFESearchNo]').val(MFSN + '%' + Adata[1]);
                    }
                    else {
                        $('[id$=Txt_MFFESearchNo]').val(MFSN + '%' + Adata[0]);
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

        function SearchData1() {
            var value1 = $('[id$=Txt_MFFENo]').val();
            oTable.fnFilter(value1, 1);
        }
        function SearchData2() {
            var value1 = $('[id$=Txt_MFFEDate]').val();
            oTable.fnFilter(value1, 2);
        }
        function SearchData3() {
            var value1 = $('[id$=Txt_MFFESearchNo]').val();
            oTable.fnFilter(value1, 3);
        }
        function SearchData4() {
            var value1 = $('[id$=Txt_MFFPONO]').val();
            oTable.fnFilter(value1, 4);
        }
        function SearchData5() {
            var value1 = $('[id$=Txt_MFFPODate]').val();
            oTable.fnFilter(value1, 5);
        }
        function SearchData6() {
            var value1 = $('[id$=Txt_MFFPOSearchFPO]').val();
            oTable.fnFilter(value1, 6);
        }
        
    </script>
</asp:Content>
