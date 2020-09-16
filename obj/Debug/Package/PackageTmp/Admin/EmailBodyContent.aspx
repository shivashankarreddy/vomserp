<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="EmailBodyContent.aspx.cs" Inherits="VOMS_ERP.Admin.EmailBodyContent" %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Compose E-Mail Body"
                                            CssClass="bcTdTitleLabel">
                                        </asp:Label><div id="divMyMessage" runat="server" align="center" class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span4" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td colspan="2" class="bcTdNewTable" style="float: left; !important">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabelright">Company Name<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal" colspan="2">
                                                    <asp:DropDownList runat="server" ID="ddlCompany" AutoPostBack ="true" CssClass="bcAspdropdown" 
                                                        onselectedindexchanged="ddlCompany_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabelright">Screen Name<font color="red" size="2"><b>*</b></font>:
                                                    </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal" colspan="2">
                                                    <asp:DropDownList runat="server" ID="ddlPage" CssClass="bcAspdropdown">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabelright">Subject<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal" colspan="2">
                                                    <asp:TextBox runat="server" ID="txtSbjct" TextMode="MultiLine" Width="81%" CssClass="bcAsptextboxmulti">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="bcTdnormal">
                                                    <span class="bcLabelright">Message<font color="red" size="2"><b>*</b></font>: </span>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td class="bcTdnormal" colspan="2">
                                                    <asp:TextBox runat="server" ID="txtMsgBdy" TextMode="MultiLine" Width="81%" Height="250px"
                                                        CssClass="bcAsptextboxmulti">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
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
                                                        <div id="Div2" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnClear" Text="Clear" 
                                                                onclick="btnClear_Click" />
                                                            <%--OnClick="btnClear_Click"--%>
                                                        </div>
                                                    </td>
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div3" class="bcButtonDiv">
                                                            <a href="../Masters/Home.aspx" title="Exit" class="bcAlink">Exit </a>
                                                            <%--onclick="javascript:Exit()"--%>
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
                </table>
                <div style="display: none;">
                    <%-- <rsweb:reportviewer id="ReportViewer1" runat="server" font-names="Verdana" font-size="8pt"
                        height="400px" width="65%">
                    </rsweb:reportviewer>--%>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="aligntable" id="aligntbl" style="margin-left: 10px !important;">
                    <table id="gvEmailBody" class="widthFull fontsize10 displayNone" cellpadding="0" cellspacing="0"
                        border="0" width="100%">
                        <thead>
                            <tr>
                                <th id="ID" runat="server" visible="false">
                                </th>
                                <th width="03%">
                                    S.No
                                </th>
                                <th width="08%">
                                    Company
                                </th>
                                <th width="08%">
                                    Page
                                </th>
                                <th width="05%">
                                    Subject
                                </th>
                                <th width="15%">
                                    Message
                                </th>
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
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(window).load(function () {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });
    </script>
    <script type="text/javascript">

        function Myvalidations() {


            if (($('[id$=ddlCompany]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Company Name is Required.</span>');
                $('[id$=ddlCompany]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=ddlPage]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Page Name is Required.</span>');
                $('[id$=ddlPage]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtSbjct]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Subject is Required.</span>');
                $('[id$=txtSbjct]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else if (($('[id$=txtMsgBdy]').val()).trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Message is Required.</span>');
                $('[id$=txtMsgBdy]').focus();
                $('[id$=divMyMessage]').fadeTo(1000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
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
    </script>

    <script type="text/javascript">

        var oTable;
        $(document).ready(function () {
            $("[id$=btnUpdate]").hide();
            oTable = $("[id$=gvEmailBody]").dataTable({
                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records&nbsp;&nbsp;&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "",
                    "sSearch": "Search : "
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
                "sAjaxSource": "CVMappings.asmx/GetEmailBodyContent",
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
                                    $("#gvEmailBody").show();
                                }
                    });
                }
            });
        });
        /* Init the table */
        oTable = $("#gvEmailBody").dataTable();

        $(document).ready(function () {
            $(".aligntable").width($(window).width() - 60 + "px");
        });

        function Delet(valddd) {
            try {
                if (confirm("Are you sure you want to Delete?")) {
                    var result = EmailBodyContent.DeleteItemDetails(valddd.parentNode.parentNode.id);
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

        function EditDetails(valddd, CreatedBy, IsCust) {
            try {
                window.location.replace("../Admin/EmailBodyContent.aspx?ID=" + valddd.parentNode.parentNode.id);

            } catch (e) {
                alert(e.Message);
            }
        }
    </script>

</asp:Content>
