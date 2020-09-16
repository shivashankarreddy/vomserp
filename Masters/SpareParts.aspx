<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="SpareParts.aspx.cs" Inherits="VOMS_ERP.Masters.SpareParts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Spare Parts" CssClass="bcTdTitleLabel"> </asp:Label>
                                        <div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="3">
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td colspan="3" class="bcTdNewTable">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">SpareParts Code<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtSparepartsCode" TextMode="SingleLine" CssClass="bcAsptextbox"
                                                        onchange="SearchSparePartsCode()" MaxLength="3" onkeyup="extractNumber(this,0,true)"
                                                        onkeypress="return blockNonNumbers(this, event, true, true)"></asp:TextBox>
                                                    <asp:HiddenField ID="hfEditID" runat="server" Value="" />
                                                </td>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabel">Description<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal">
                                                    <asp:TextBox runat="server" ID="txtSparePartsDesc" TextMode="SingleLine" CssClass="bcAsptextbox"
                                                        MaxLength="1000"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <%--<tr>
                                                <td colspan="6" class="bcTdTitleLabel">
                                                    <h3>
                                                        Details</h3>
                                                </td>
                                            </tr>--%>
                                            <tr>
                                                <td class="bcTdTitleLabel">
                                                    <asp:FileUpload ID="FileUpload1" runat="server" Visible="false" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnBulkUpload" runat="server" Text="Upload" OnClick="btnBulkUpload_Click"
                                                        Visible="false" />
                                                </td>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="999">
                                                    <div id="divSpareParts" runat="server" style="min-height: 150px; max-height: 500px;
                                                        display: none">
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
                        <td colspan="3" class="bcTdNewTable">
                            <div id="div5" runat="server" style="min-height: 10px; max-height: 10px;">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                <tbody>
                                    <tr align="center" valign="middle">
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div1" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div4" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnUpdate" Text="Update" OnClick="btnUpdate_Click" />
                                            </div>
                                        </td>
                                        <td align="center" valign="middle" class="bcTdButton">
                                            <div id="Div2" class="bcButtonDiv">
                                                <asp:LinkButton runat="server" ID="btnClear" Text="Clear" OnClick="btnClear_Click" />
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
            <td colspan="3" class="bcTdNewTable">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvSpareParts" class="widthFull fontsize10 displayNone" cellpadding="0"
                        cellspacing="0" border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="03%">
                                    Code
                                </th>
                                <th width="20%">
                                    SpareParts Description
                                </th>
                                <%--<th width="10%">
                                    Cat Code
                                </th>
                                <th width="20%">
                                    Cat Desc
                                </th>--%>
                                <th width="02%">
                                    E
                                </th>
                                <th width="02%">
                                    D
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/JScript.js" type="text/javascript"></script>
    <script src="../JScript/validate2.js" type="text/javascript"></script>
    <style type="text/css">
        .dataTables_filter
        {
            visibility: visible !important;
        }
    </style>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });

        function CheckAll() {
            var IsChecked = $("#ChkHead").is(':checked');
            var result = SpareParts.CheckAll(IsChecked);
            var getDivSpareParts = GetClientID("divSpareParts").attr("id");
            $('#' + getDivSpareParts).html(result.value);
        }

        function Check(Cntrl, SubCatID) {
            var IsChecked = $("#" + Cntrl.id).is(':checked');
            var result = SpareParts.Check(SubCatID, IsChecked);
            var getDivSpareParts = GetClientID("divSpareParts").attr("id");
            $('#' + getDivSpareParts).html(result.value);
        }

        function Myvalidations() {
            //            var RCount = $("#tblItems >tbody >tr").length;
            //            if ($('[id$=txtSparepartsCode]').val() == "") {                
            //                $('[id$=txtSparepartsCode]').focus();
            //                ErrorMessage('Spare parts Code is required.');
            //                return false;
            //            }
            var val = $('[id$=txtSparepartsCode]').val();
            if (val == "000") {
                ErrorMessage('Spare parts Code cannot be "000".');
                $('[id$=txtSparepartsCode]').val('').focus();
                return false;
            }
            else if (val.length < 3) {
                ErrorMessage('Spare parts Code length should be 3 digits.');
                $('[id$=txtSparepartsCode]').val('').focus();
                return false;
            }
            else if ($("[id$=txtSparePartsDesc]").val().trim() == "") {
                alert("Spare parts Description is required.");
                $("[id$=txtSparePartsDesc]").focus();
                ErrorMessage("Spare parts Description is required.");
                return false;
            }
        }

        function SearchSparePartsCode() {
            var Code = $('[ID$=txtSparepartsCode]').val();
            var EditID = $('[ID$=hfEditID]').val();
            if ($('[id$=txtSparepartsCode]').val() == "000") {
                ErrorMessage('Spare parts Code cannot be "000".');
                $('[id$=txtSparepartsCode]').val('').focus();
                return false;
            }
            else {
                var result = SpareParts.SearchSparePartsCode(Code, EditID);
                if (result.value == false) {
                    ErrorMessage("SpareParts Code Exists");
                    $('[id$=txtSparepartsCode]').val('');
                    return false
                }
            }
        }
    </script>
    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            oTable = $("[id$=gvSpareParts]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
                },
                "aLengthMenu": [[100, 200, 500, -1], [100, 200, 500, 'ALL']],
                "iDisplayLength": 100,
                "bSortClasses": false,
                "bStateSave": false,
                "bPaginate": true,
                "bAutoWidth": true,
                "bProcessing": true,
                "bServerSide": true,
                "bDestroy": true,
                "sAjaxSource": "ItemCategory.asmx/GetSpareParts",
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bDeferRender": true,
                "sScrollY": "250px",
                "sScrollX": "100%",
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
								    $("#gvItmCategory").show();
								}
                    });
                }
            });
        });
        /* Init the table */
        oTable = $("#gvSpareParts").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

        //        function SearchItmCode() {
        //            var value1 = $('[id$=txtitemCatCode]').val();
        //            var result = ItemCategoryMaster.CheckCode(value1);
        //            if (result.value == false) {
        //                ErrorMessage("Item Category Code Exists");
        //                $('[id$=txtitemCatCode]').val('');
        //                return false
        //            }
        //        }

        function DeleteDetails(ID, CreatedBy) {
            if ($("[id$=hfEditID]").val().toLowerCase() != ID.toLowerCase()) {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = SpareParts.Delete(ID, CreatedBy);
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
            }
            else
                ErrorMessage('You cannot Delete, when the same row is Editing');
        }

        function EditDetails(valddd, ID) {

            //            $("[id$=txtSparepartsCode]").val('');
            //            $("[id$=txtSparepartsDesc]").val('');

            document.location = "SpareParts.aspx?ID=" + ID;

            //            $("[id$=btnSave]").hide();
            //            $("[id$=btnUpdate]").show();
            //            document.getElementById('<%= btnUpdate.ClientID %>').innerHTML = 'Update';
        }
    </script>
</asp:Content>
