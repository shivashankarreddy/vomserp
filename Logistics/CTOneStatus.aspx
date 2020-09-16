<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="CTOneStatus.aspx.cs" Inherits="VOMS_ERP.Logistics.CTOneStatus"
     %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="CT1 Status" CssClass="bcTdTitleLabel"></asp:Label><div
                                            id="divMyMessage" runat="server" align="center" class="formError1" style="margin-right: 0%;" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <table width="100%">
                    <tr>
                        <td align="right">
                            <div runat="server" id="dvexport">
                                <asp:ImageButton ID="btnExcelExpt" runat="server" ImageUrl="../images/EXCEL.png"
                                    class="item_top_icons" title="Export Excel" OnClick="btnExcelExpt_Click" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                                <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                                    border="0">
                                    <thead>
                                        <tr>
                                            <th width="08%">
                                                CT1 Draft.No.
                                            </th>
                                            <th width="08%">
                                                CT1 Ref.No.
                                            </th>
                                            <th width="10%">
                                                Reference Date
                                            </th>
                                            <th width="12%">
                                                IOM Number
                                            </th>
                                            <th width="12%">
                                                FPO Number(s)
                                            </th>
                                            <th width="12">
                                                LPO Number(s)
                                            </th>
                                            <th width="10">
                                                Supplier Name
                                            </th>
                                            <th width="5">
                                                Status
                                            </th>
                                            <th width="04%">
                                                Re-Apply
                                            </th>
                                            <th width="04%">
                                                Edit
                                            </th>
                                            <th width="03%">
                                                Delete
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th style="text-align: right" colspan="5">
                                            </th>
                                            <th colspan="3" align="left">
                                            </th>
                                            <th colspan="3" align="right">
                                            </th>
                                        </tr>
                                        <tr>
                                            <th>
                                                CT1 Draft.No.
                                            </th>
                                            <th>
                                                CT1 Ref.No.
                                            </th>
                                            <th>
                                                Reference Date
                                            </th>
                                            <th>
                                                IOM Number
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
                                                Status
                                            </th>
                                            <th>
                                            </th>
                                            <th>
                                            </th>
                                            <th>
                                            </th>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HiddenField ID="HFCT1DN" runat="server" Value="" />
                            <asp:HiddenField ID="HFCT1RN" runat="server" Value="" />
                            <asp:HiddenField ID="HFRefFrmDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFrefToDt" runat="server" Value="" />
                            <asp:HiddenField ID="HFIOMno" runat="server" Value="" />
                            <asp:HiddenField ID="HFFPONo" runat="server" Value="" />
                            <asp:HiddenField ID="HFLPONo" runat="server" Value="" />
                            <asp:HiddenField ID="HFSupplier" runat="server" Value="" />
                            <asp:HiddenField ID="HFStatus" runat="server" Value="" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
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

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 84 + "px");
        });

        var oTable = null;
        $(document).ready(function () {
            myfunction();
        });

        function myfunction() {
            $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
            $.datepicker.setDefaults($.datepicker.regional['']);
            oTable = $("#gvItmMstr").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                //"aLengthMenu": [[5000, -1], [5000, "All"]],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "CT1WebService1.asmx/GetCT1Items",
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
                                    $("#gvItmMstr").show();
                                }
                    });
                }
            });
            $("#gvItmMstr").dataTable().columnFilter(
                {
                    "aoColumns": [
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "date-range" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" },
                                    { "type": "text" }, null, null, null
                                   ]
                });

            $("tfoot input").change(function (i) {
                var InDex = $("tfoot input").index(this);
                var Valuee = this.value;

                if (InDex == 0) {
                    $('[id$=HFCT1DN]').val(Valuee);
                }
                else if (InDex == 1) {
                    $('[id$=HFCT1RN]').val(Valuee);
                }
                else if (InDex == 2) {
                    $('[id$=HFRefFrmDt]').val(Valuee);
                }
                else if (InDex == 3) {
                    $('[id$=HFrefToDt]').val(Valuee);
                }
                else if (InDex == 4) {
                    $('[id$=HFIOMno]').val(Valuee);
                }
                else if (InDex == 5) {
                    $('[id$=HFFPONo]').val(Valuee);
                }
                else if (InDex == 6) {
                    $('[id$=HFLPONo]').val(Valuee);
                }
                else if (InDex == 7) {
                    $('[id$=HFSupplier]').val(Valuee);
                }
                else if (InDex == 8) {
                    $('[id$=HFStatus]').val(Valuee);
                }
            });
            /* Init the table */
            oTable = $("#gvItmMstr").dataTable();
        }

        function Reapply(valddd, IomId, PinvId) {
            try {
                window.showModalDialog("../Logistics/CTOneReApplie.aspx?ReApp=yes&PinvID=" + PinvId + "&IomID=" + IomId + "&CT1="
                        + valddd.parentNode.parentNode.id, "Re-Applie", "dialogHeight:300px; dialogWidth:380px; dialogLeft:300; " +
                        "dialogright:150; dialogTop:250; toolbar=no;scrollbars=no;location=no;resizable =no");
                oTable = $("#gvItmMstr").dataTable();
                myfunction();
            }
            catch (e) {
                alert(e.Message);
            }
        }

        function Delet(valddd, CreatedBy, IsCust) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = CTOneStatus.DeleteItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    if (fres.contains('Success::')) {
                        oTable.fnDraw();
                        SuccessMessage(fres.replace('Success::', ''));
                    }
                    else if (fres.contains('Error::')) {
                        ErrorMessage(fres.replace('Error::', ''));
                    }
                    else {
                        ErrorMessage(fres);
                    }
                }
            } catch (e) {
                alert(e.Message);
            }
        }

        function EditDetails(valddd, IomId, PinvId, PinvReqID, CreatedBy, IsCust, Status,ContactType) {
            try {
                if ((Status == "ReApplied") || (Status == "Terminate")) {
                    ErrorMessage("Unable to Edit")
                }
//                else if (ContactType != "Supplier") {
//                    ErrorMessage("You do not have Permissions to Edit.")
//                }
                else {
                    var result = CTOneStatus.EditItemDetails(valddd.parentNode.parentNode.id, CreatedBy, IsCust);
                    var fres = result.value;
                    var Ct1ID = valddd.parentNode.parentNode.id;
                    var Ct1IdRplce = Ct1ID.replace(/'/g, '');
                    if (fres == 'Success') {
                        window.location.replace("../Logistics/CTOneGeneration.aspx?PinvID=" + PinvId + "&IomID=" + IomId + "&CT1=" + Ct1IdRplce + "&PrfINVID=" + PinvReqID);
                    }

                    else {
                        ErrorMessage(fres);
                    }
                }
            }
            catch (e) {
                alert(e.Message);
            }
        }

        function ReApplicable(PInvID, IOmID, CT1) {
            try {
                window.location.replace("../Logistics/CTOneGeneration.aspx?ReApp=yes&PinvID=" + PInvID + "&IomID=" + IOmID + "&CT1=" + CT1 + "&PrfINVID=0");
            } catch (e) {
                ErrorMessage(e.Message);
            }
        }
        

        $(window).load(function () {
            $("#clickExcel").click(function () {
                var gvItmMstr = $('#gvItmMstr').html();
                window.open('data:application/vnd.ms-excel,' + $('#gvItmMstr').html());
            });
        });



        $(document).ready(function () {
            $('[id$=ImgCancel]').click(function (e) {
                var PnvID = $(this).closest('tr').find($('input[id$=HFPnvID]')).val();
                var CT1ID = $(this).closest('tr').find($('input[id$=HFCT1ID]')).val();
                $('input[id$=HFPnvIDNew]').val(PnvID);
                $('input[id$=HFCT1IDNew]').val(CT1ID);
                e.preventDefault();
            });
        });
        function SetStatus(action) {
            var PnvID = $('input[id$=HFPnvIDNew]').val();
            var CT1ID = $('input[id$=HFCT1IDNew]').val();
            var result = CTOneStatus.PerformActions(action, PnvID, CT1ID);
            if (result.value == true) {
                location.reload();
            }
            else {
                ErrorMessage('Unable to Update...!');
            }
        }
    </script>
    <script type="text/javascript">
        function getValue(obj) {
            var val = $(obj).children("span").get(0).innerHTML;
        }

        jQuery(function ($) {
            var OSX = {
                container: null,
                init: function () {
                    $("input.osx, a.osx, image.osx").click(function (e) {
                        e.preventDefault();
                        $("#osx-modal-content").modal({
                            overlayId: 'osx-overlay',
                            containerId: 'osx-container',
                            closeHTML: null,
                            minHeight: 80,
                            opacity: 65,
                            position: ['0', ],
                            overlayClose: true,
                            onOpen: OSX.open,
                            onClose: OSX.close
                        });
                    });
                },
                open: function (d) {
                    var self = this;
                    self.container = d.container[0];
                    d.overlay.fadeIn('slow', function () {
                        $("#osx-modal-content", self.container).show();
                        var title = $("#osx-modal-title", self.container);
                        title.show();
                        d.container.slideDown('slow', function () {
                            setTimeout(function () {
                                var h = $("#osx-modal-data", self.container).height()
							+ title.height()
							+ 20; // padding
                                d.container.animate(
							{ height: h },
							200,
							function () {
							    $("div.close", self.container).show();
							    $("#osx-modal-data", self.container).show();
							}
						);
                            }, 300);
                        });
                    })
                },
                show: function (ReApply) {
                    alert('Z');
                    //$('[id$=btnCancel]').click(function(e) {
                    $('#ctl00_ContentPlaceHolder1_btnCancel').click(function (e) {
                        e.preventDefault();

                        SetStatus('Cancel');

                    });
                    //$('[id$=btnReApplicable]').click(function(e) {
                    $('#ctl00_ContentPlaceHolder1_btnReApplicable').click(function (e) {
                        e.preventDefault();

                        SetStatus('ReApplicable');

                    });
                    //$('[id$=btnTerminate]').click(function(e) {
                    $('#ctl00_ContentPlaceHolder1_btnTerminate').click(function (e) {
                        e.preventDefault();

                        SetStatus('Terminate');
                    });
                },
                SetStatus: function (action) {
                    var PnvID = $('input[id$=HFPnvIDNew]').val();
                    var CT1ID = $('input[id$=HFCT1IDNew]').val();
                    var result = CTOneStatus.PerformActions(action, PnvID, CT1ID);
                    if (result.value == true) {
                        location.reload();
                    }
                    else {
                        ErrorMessage('Unable to Update...!');
                    }
                },
                close: function (d) {
                    var self = this; // this = SimpleModal object
                    d.container.animate(
				{ top: "-" + (d.container.height() + 20) },
				500,
				function () {
				    self.close(); // or $.modal.close();
				    alert('A');
				}
			);
                }
            };

            OSX.init();

        });
    </script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtFrmDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtToDt]').datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });

        function OpenDialod() {

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
    </script>
</asp:Content>
