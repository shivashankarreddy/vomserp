<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ItemPreviousHistory.aspx.cs" Inherits="VOMS_ERP.Reports.ItemPreviousHistory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <asp:HiddenField runat="server" ID="hdfdItmMstrID" />
    <asp:HiddenField runat="server" ID="hdSubCatgryItmMstrID" />
    <asp:HiddenField runat="server" ID="hdSubSubCatryItmMrID" />
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Item Previous History"
                                            CssClass="bcTdTitleLabel"> </asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdNewTable">
                                        <asp:UpdatePanel ID="UpdtPnlItems" runat="server">
                                            <ContentTemplate>
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Description<font color="red" size="2"><b>*</b></font>: </span>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:TextBox runat="server" ID="txtItmDscrip" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                                                onkeyup="SearchDesc()"></asp:TextBox>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Part Number: </span>
                                                        </td>
                                                        <td class="bcTdnormal">
                                                            <asp:TextBox runat="server" ID="txtItmPrtNmbr" MaxLength="150" CssClass="bcAsptextbox"
                                                                onkeyup="SearchPartNo()"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="bcTdnormal">
                                                            <span class="bcLabel">Specifications : </span>
                                                        </td>
                                                        <td class="bcTdnormal" colspan="4">
                                                            <asp:TextBox runat="server" ID="txtspec" TextMode="MultiLine" CssClass="bcAsptextboxmulti"
                                                                onkeyup="SearchSpec()" Width="595px" Columns="80" Rows="2" onblur="javascript:return CheckItemsMaster();"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" 
                                                                onclick="btnClear_Click" />
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div3" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" class="bcAlink" onclick="javascript:Exit()">
                                                                Exit </a>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
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
            </td>
        </tr>
        <tr>
            <td>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvItmMstr" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                        border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="03%">
                                    Item Code
                                </th>
                                <th width="20%">
                                    Item Description
                                </th>
                                <th width="10%">
                                    Part Number
                                </th>
                                <th width="20%">
                                    Specification
                                </th>
                                <th width="05%">
                                    Contains Sub-Items
                                </th>
                                <th width="02%">
                                    Export
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery.dataTables.columnFilter.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Scripts/js/FixedHeader.js" type="text/javascript"></script>
    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
   <%--<style type="text/css">
		.dataTables_filter
		{
			visibility: visible !important;
		}
	</style>--%>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            //$("[id$=btnUpdate]").hide();
            oTable = $("[id$=gvItmMstr]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "DIENSH"
                },

                "aLengthMenu": [[100, 200, 500, 1000, -1], [100, 200, 500, 1000, 'ALL']],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "../Enquiries/WebService1.asmx/GetIMItems_PreviousRates",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
                "bScrollCollapse": true,

                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "iItemDesc", "value": $('[id$=txtItmDscrip]').val() });
                    aoData.push({ "name": "iPartNo", "value": $('[id$=txtItmPrtNmbr]').val() });
                    aoData.push({ "name": "iSpec", "value": $('[id$=txtspec]').val() });
                },

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
        });
        /* Init the table */
        oTable = $("#gvItmMstr").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });



        function SearchDesc() {
            var value1 = $('[id$=txtItmDscrip]').val();
            oTable.fnFilter(value1, 1);
        }

        function fn_codeclear() {
            $('[id$=txtItmCode]').val('');
        }


        function SearchPartNo() {
            var value2 = $('[id$=txtItmPrtNmbr]').val();
            oTable.fnFilter("^" + value2 + "$", 2, true);
        }
        function SearchSpec() {
            var value3 = $('[id$=txtspec]').val();
            oTable.fnFilter("^" + value3 + "$", 3, true);
        }

        $('.Exp').live('click', function (e) {

            var returnVal = "";
            var Id = this.parentNode.parentNode.id;
            var res = ItemPreviousHistory.ReportExp(Id);
            if (res.value == "") {
                ErrorMessage("No Previous History Found for the Item.");
            }
            else {
                $('[id$=divExcel]').html(res.value);
                fnExcelReport();
            }
        });

        function fnExcelReport() {
            var tab_text = "<table><tr>";
            var textRange; var j = 0;
            tab = document.getElementById('ExportReport'); // id of table
            for (j = 0; j < tab.rows.length; j++) {
                tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
                //tab_text=tab_text+"</tr>";
            }
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");
            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
            {
                txtArea1.document.open("txt/html", "replace");
                txtArea1.document.write(tab_text);
                txtArea1.document.close();
                txtArea1.focus();
                sa = txtArea1.document.execCommand("SaveAs", true, 'ITEMHISTORY.xls');
            }
            else                 //other browser not tested on IE 11
            {
                //sa = txtArea1.document.execCommand("SaveAs", true, 'ITEMHISTORY.xls');
                sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text)); //'data:application/vnd.ms-excel,' + encodeURIComponent(tab_text)
            }

            return (sa);
        }
        
    </script>
</asp:Content>
