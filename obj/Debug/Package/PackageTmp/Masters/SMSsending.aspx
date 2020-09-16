<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeBehind="SMSsending.aspx.cs" Inherits="VOMS_ERP.Masters.SMSsending"  %>

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
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Contact" CssClass="bcTdTitleLabel"></asp:Label>
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
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Label13" class="bcLabelright">Supplier Name:</span>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:DropDownList runat="server" ID="ddlsuplr" CssClass="bcAspdropdown">
                                            <asp:ListItem Value="0" Text="Select Supplier"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="VEDAA" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="BHEL"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Span1" class="bcLabelright">Message:</span>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:TextBox runat="server" ID="txtMsg" CssClass="bcAsptextboxmulti" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;" class="bcTdnormal">
                                        <span id="Span2" class="bcLabelright">Send Private Numbers:</span>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td class="bcTdnormal">
                                        <asp:CheckBox runat="server" ID="chkbPvtNmbr" Text=" " onclick='CHeck("chkbPvtNmbr", "dvPvtNmbr")' />
                                        <div id="dvPvtNmbr" style="display: none;">
                                            <asp:TextBox runat="server" ID="txtMblNumbr" CssClass="bcAsptextbox" MaxLength="12"
                                                onkeypress="return isNumberKey(event)"></asp:TextBox>
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
            <td colspan="3">
                <div>
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:GridView runat="server" ID="gvSuplrs" AutoGenerateColumns="False" Width="100%"
                                    RowStyle-CssClass="bcGridViewRowStyle" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                    PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                    CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                    OnPreRender="gvSuplrs_PreRender" OnRowDataBound="gvSuplrs_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Supplier Contact Name" DataField="Department Name" HeaderStyle-Width="300px" />
                                        <asp:TemplateField HeaderText="Mobile Number">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblID" Text='<%#Eval("Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Edit.jpeg" CommandName="Modify"
                                            Text="Modify" ShowHeader="true" HeaderStyle-Width="20px" />
                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/Delete.png" CommandName="Delete"
                                            Text="Delete" ShowHeader="true" HeaderStyle-Width="20px" />
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
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
                                    <asp:LinkButton runat="server" ID="btnSend" Text="Send" OnClientClick="javascript:validations()" />
                                </div>
                            </td>
                            <td align="center" valign="middle" class="bcTdButton">
                                <div id="Div2" class="bcButtonDiv">
                                    <asp:LinkButton runat="server" ID="btnClear" OnClientClick="Javascript:clearAll()"
                                        Text="Clear" />
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
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>
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
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //without passing class names.
            $("[id$=gvSuplrs]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",

                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search criteria",
                    "sLengthMenu": "Display _MENU_ records per page&nbsp;&nbsp;",
                    "sInfo": "Displaying _START_ to _END_ of _TOTAL_ records",
                    "sInfoEmpty": "Showing 0 to 0 of 0 records",
                    "sInfoFiltered": "(filtered from _MAX_ total records)",
                    "sSearch": "Search :"
                },

                //Scrolling--------------
                "sScrollY": "250px",
                "sScrollX": "100%",
                "sScrollXInner": "100%",
                "bScrollCollapse": true
            });
        });
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
                return false;
            return true;
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
    <script language="javascript" type="text/javascript">
        function CHeck(ckid, dvid) {
            var ChkBox = document.getElementById("ctl00_ContentPlaceHolder1_" + ckid);
            if (ChkBox.checked == true) {
                document.getElementById(dvid).style.display = 'block';
                document.getElementById("ctl00_ContentPlaceHolder1_gvSuplrs").style.display = 'none';
            }
            else {
                document.getElementById(dvid).style.display = 'none';
                document.getElementById("ctl00_ContentPlaceHolder1_gvSuplrs").style.display = 'block';
            }
        }
    </script>
</asp:Content>
