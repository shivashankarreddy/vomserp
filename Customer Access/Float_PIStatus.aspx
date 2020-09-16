<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerMaster.master" AutoEventWireup="true"
    CodeBehind="Float_PIStatus.aspx.cs" Inherits="VOMS_ERP.Customer_Access.Float_PIStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table width="100%">
                    <tr class="bcTRTitleRow">
                        <td>
                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Floated Enquiry Status"
                                CssClass="bcTdTitleLabel"></asp:Label>
                            <div id="divMyMessage" runat="server" align="center" class="formError1" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
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
                            <th width="5%">
                                Recv Date
                            </th>
                            <th width="20%">
                                Subject
                            </th>
                            <th width="10%">
                                Department
                            </th>
                            <th width="10%">
                                Vendor/Supplier
                            </th>
                            <th width="5%">
                                Customer
                            </th>
                            <th width="15%">
                                Contact Person
                            </th>
                            <th width="28%">
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
                            <th colspan="2" align="left">
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
                                Vendor
                            </th>
                            <th>
                                Customer
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
                <asp:HiddenField ID="HFRcvdFromDt" runat="server" Value="" />
                <asp:HiddenField ID="HFRcvdToDt" runat="server" Value="" />
                <asp:HiddenField ID="HFSubject" runat="server" Value="" />
                <asp:HiddenField ID="HFDept" runat="server" Value="" />
                <asp:HiddenField ID="HFVen" runat="server" Value="" />
                <asp:HiddenField ID="HFCust" runat="server" Value="" />
                <asp:HiddenField ID="HFCnctPrsn" runat="server" Value="" />
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
                "sAjaxSource": "PI_Webservice.asmx/GetFFEItems",
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
                                    { "type": "date-range" },
                                    { "type": "date-range" },
                                    { "type": "text" },
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
                    $('[id$=HFVen]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFCust]').val(Valuee);
                }
                else if (InDex == 9) {
                    $('[id$=HFCnctPrsn]').val(Valuee);
                }
                else if (InDex == 10) {
                    $('[id$=HFStatus]').val(Valuee);

                }
            });

            /* Init the table */
            oTable = $('#tblOscarNominees').dataTable();
        });

        function mailsDetails(valddd) {
            try {
                window.location.replace("../Masters/EmailSend.aspx?PIFFeID=" + valddd.parentNode.parentNode.id);
            } catch (e) {
                alert(e.Message);
            }
        }
        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                // var result = FEStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                //var fres = result.value;
                //if (fres == 'Success') {
                window.location.replace("../Customer Access/Float_PI.Aspx?ID=" + valddd.parentNode.parentNode.id);
            }

            catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = Float_PIStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
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
            var returnVal = "";
            var Id = this.parentNode.parentNode.id;
            $('[id$=ExportHF]').val(Id);
            var res = FEStatus.ReportExp(Id);
            //function fnOpen(id, rowIndex) {
            //returnVal = window.showModalDialog("../Enquiries/Export.aspx?Feid="+ Id , "Export",
            //"dialogHeight:100px; dialogWidth:280px; dialogLeft:450; dialogright:350; dialogTop:400; ");

            //$find('ModalPopupExtender1').show();
            //            var r = confirm("Do You Want To Export The Report");
            //            if (r == true) {
            //                x = FEStatus.ReportExp_Click(Id);
            //            } else {
            //                x = "You pressed Cancel!";
            //            }
            //$('[id$=ReprotExp]').show();
        });

    </script>
</asp:Content>
