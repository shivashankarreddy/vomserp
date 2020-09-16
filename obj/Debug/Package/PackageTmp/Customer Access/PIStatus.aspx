<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="PIStatus.aspx.cs" Inherits="VOMS_ERP.Customer_Access.PIStatus" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center" id="headerTable">
        <tr>
            <td class="bcTdNewTable">
                <table width="100%">
                    <tr class="bcTRTitleRow">
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Purchase Enquiry Status"
                                CssClass="bcTdTitleLabel"></asp:Label>
                            <div id="divMyMessage" runat="server" align="center" class="formError1" />
                        </td>
                    </tr>
                    <td colspan="6">
                        <table runat="server" id="Report_Export" style="width: 100%; visibility: hidden;">
                        </table>
                    </td>
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="../images/EXCEL.png"
                                title="Export Excel" OnClick="btnExcelExpt_Click"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <table id="tblOscarNominees" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th width="10%">
                                Enquiry Number
                            </th>
                            <th width="5%">
                                Date
                            </th>
                            <%--<th width="5%">
                                Recv Date
                            </th>--%>
                            <th width="28%">
                                Subject
                            </th>
                            <th width="10%">
                                Department
                            </th>
                            <th width="5%">
                                customer
                            </th>
                            <th width="15%">
                                Contact Person
                            </th>
                            <th width="28%">
                                Status
                            </th>
                            <th width="20%">
                                Amd Report
                            </th>
                            <th width="3%">
                                M
                            </th>
                            <th width="05%">
                                Amd E
                            </th>
                            <th width="3%">
                                E
                            </th>
                            <th width="3%">
                                D
                            </th>
                            <th width="3%">
                                Export
                            </th>
                        </tr>
                    </thead>
                    <tbody class="tbody">
                    </tbody>
                    <tfoot>
                        <tr>
                            <th style="text-align: right" colspan="5">
                            </th>
                            <th colspan="4" align="left">
                            </th>
                            <th colspan="4" align="right">
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Enquiry Number
                            </th>
                            <th>
                                Date
                            </th>
                            <%--<th>
                                Recv Date
                            </th>--%>
                            <th>
                                Subject
                            </th>
                            <th>
                                Department
                            </th>
                            <th>
                                customer
                            </th>
                            <th>
                                Contact Person
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
                <asp:HiddenField ID="HFFromDate" runat="server" Value="" />
                <asp:HiddenField ID="HFToDate" runat="server" Value="" />
                <asp:HiddenField ID="HFFENo" runat="server" Value="" />
                <%--<asp:HiddenField ID="HFRcvdFromDt" runat="server" Value="" />
                <asp:HiddenField ID="HFRcvdToDt" runat="server" Value="" />--%>
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFDept" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFCnctPrsn" runat="server" Value="" />
                <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                <asp:HiddenField ID="HFFilename" runat="server" Value="" />
            </td>
        </tr>
        <tr>
            <td>
                <iframe id="txtArea1" style="display: none"></iframe>
            </td>
        </tr>
        <tr>
            <td>
                <div id="divExcel" style="max-height: 250px; min-height: 100px; display: none" runat="server">
                </div>
            </td>
        </tr>
    </table>
    <%-- <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />--%>
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
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <%--<script src="../JScript/Scripts/jquery.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/jquery.dataTables.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/dataTables.jqueryui.min.js" type="text/javascript"></script>--%>
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
        // alert();
        var Filname = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
        $('[id$=HFFilename]').val(Filname);
        var oTable;
        $(document).ready(function () {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);

            /*          Main Functionality       */
            oTable = $('#tblOscarNominees').dataTable({
                "aLengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
                "iDisplayLength": 100,
                "aaSorting": [[0, "asc"]],
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "bServerSide": true,
                "bDestroy": true,
                //"ajax": "PI_Webservice.asmx/GetFItems",

                "bJQueryUI": true,
                "sAjaxSource": "PI_Webservice.asmx/GetFItems",
                "fnServerData": function (sSource, aoData, fnCallback) {
                    $.ajax({
                        "dataType": 'json',
                        "contentType": "application/json; charset=utf-8",
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success": function (msg) {
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
                    //"sSearch":location.pathname.substring(location.pathname.lastIndexOf("/") + 1),
                    "bRegex": false,
                    "bSmart": true
                }
            });

            $("#tblOscarNominees").dataTable().columnFilter(
                        {
                            "aoColumns": [{ "type": "text" },
                                            { "type": "date-range" },
                                            { "type": "text" },
                                            { "type": "text" },
                                            { "type": "text" },
                                            { "type": "text" },
                                            { "type": "text" },
                                              null, null, null]
                        });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFFENo]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFFromDate]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFToDate]').val(Valuee);
                }
                //                else if (InDex == 3) {
                //                    $('[id$=HFRcvdFromDt]').val(Valuee);
                //                }
                //                else if (InDex == 4) {
                //                    $('[id$=HFRcvdToDt]').val(Valuee);
                //                }
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
                    $('[id$=HFCnctPrsn]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFStatus]').val(Valuee);

                }

            });

            /* Init the table */
            oTable = $('#tblOscarNominees').dataTable();
        });
    </script>
    <script type="text/jscript">
        //        $(document).ready(function () {

        //            $('#tblOscarNominees tfoot th').each(function () {
        //                var title = $(this).text();
        //                $(this).html('<input type="text" placeholder="Search ' + title + '" />');
        //            });

        //            $('#tblOscarNominees').DataTable({
        //                processing: true,
        //                serverSide: true,
        //                info: true,
        //                "lengthMenu": [[100, 250, 500, 1000, -1], [100, 250, 500, 1000, "All"]],
        //                "iDisplayLength": 100,
        //                "aaSorting": [[0, "asc"]],
        //                "bAutoWidth": false,
        //                "pagingType": "full_numbers",
        //                "deferRender": true,
        //                "language": {
        //                    "lengthMenu": "Display _MENU_ records per page",
        //                    "zeroRecords": "Nothing found - sorry",
        //                    "info": "Showing page _PAGE_ of _PAGES_",
        //                    "infoEmpty": "No records available",
        //                    "infoFiltered": "(filtered from _MAX_ total records)"
        //                },
        //                //"ajax": "PI_Webservice.asmx/GetFItems",

        //                "ajax": {
        //                    "url": 'PI_Webservice.asmx/GetFItems',
        //                    "type": "POST"
        //                    //"url": '../aa.txt',                    
        //                    //,"contentType": "application/json; charset=utf-8"
        //                    //                    ,data: function (data) {
        //                    //                        delete data.columns;
        //                    //                    }
        //                }
        //                                , columns: [
        //                                            { "data": "EnquireNumberS" },
        //                                            { "data": "EnquiryDateS" },
        //                                            { "data": "ReceivedDateS" },
        //                                            { "data": "Subject" },
        //                                            { "data": "DepartmentId" },
        //                                            { "data": "CusmorId" },
        //                                            { "data": "ContactPerson" },
        //                                            { "data": "StatusTypeId" },
        //                                            { "data": "AMENDEDIT" },
        //                                            { "data": "MAIL" },
        //                                            { "data": "AMENDEDIT" },
        //                                            { "data": "EDIT" },
        //                                            { "data": "Delt" },
        //                                            { "data": "Export" }
        //                                         ]
        //            });
        //        });    
    </script>
    <script type="text/jscript">
        function AlertMessage(Msg) {
            ErrorMessage('Completed Enquiry cannot ' + Msg.replace('_',' ') + '.');
        }
        function mailsDetails(valddd) {
            try {
                window.location.replace("../Masters/EmailSend.aspx?PeID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }

        function Ammendement(valddd, CreatedBy, IsCust) {
            try {
                var result = PIStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Customer Access/PurchaseEnquiry.aspx?ID=" + valddd.parentNode.parentNode.id + "&IsAm=True");
                }
                else {
                    ErrorMessage(fres);
                }

            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                var result = PIStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                var fres = result.value;
                if (fres == 'Success') {
                    window.location.replace("../Customer Access/PurchaseEnquiry.Aspx?ID=" + valddd.parentNode.parentNode.id);
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
                    var result = PIStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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

        function Regre(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Regret?")) {
                    var result = PIStatus.IsRegret(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.contains('Success')) {
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

        $('.Exp').live('click', function (e) {
            //e.preventDefault();
            // Confirm();
            //openDialog();

            var returnVal = "";
            var Id = this.parentNode.parentNode.id;
            $('[id$=ExportHF]').val(Id);
            var res = PIStatus.ReportExp(Id);
            $('[id$=divExcel]').html(res.value);
            fnExcelReport();

            function openDialog() {
                document.execCommand("SaveAs", true, "Test.xlsx");
                //alert('aa');
            }


            //function fnOpen(id, rowIndex) {
            //returnVal = window.showModalDialog("../Enquiries/Export.aspx?Feid="+ Id , "Export",
            //"dialogHeight:100px; dialogWidth:280px; dialogLeft:450; dialogright:350; dialogTop:400; ");

            //$find('ModalPopupExtender1').show();
            //            var r = confirm("Do You Want To Export The Report");
            //            if (r == true) {
            //                x = PIStatus.ReportExp_Click(Id);
            //            } else {
            //                x = "You pressed Cancel!";
            //            }
            //$('[id$=ReprotExp]').show();
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

    </script>
    <script type="text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            document.execCommand('SaveAs', null, "test1.xls")
            if (confirm("Do you want to save data?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
    </script>
</asp:Content>
