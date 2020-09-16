<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="FEnquiryRpt.aspx.cs" Inherits="VOMS_ERP.Reports.FEnquiryRpt"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"
    defaultbutton="btnSearch" defaultfocus="txtCstNm">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" style="font-size:15.5px;font-weight:bold" Text="Foreign Enquiry Reports"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
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
                                    <td class="bcTdnormal" style="display: block">
                                        <span class="bcLabelright">Customer Name :</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtCstNm" class="autosuggest" MaxLength="150" CssClass="bcAsptextbox"></asp:TextBox>
                                        <asp:HiddenField ID="hfCustomerId" runat="server" Value="0" />
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">From Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtFrmDt" CssClass="bcAsptextbox" MaxLength="12"
                                            onchange="changedate();"></asp:TextBox>
                                    </td>
                                    <td class="bcTdnormal">
                                        <span class="bcLabelright">To Date:</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox runat="server" ID="txtToDt" CssClass="bcAsptextbox" MaxLength="12"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="6">
                                        <table border="0" cellpadding="0" width="15%" style="vertical-align: middle;">
                                            <tbody>
                                                <tr align="center" valign="middle">
                                                    <td align="center" valign="middle" class="bcTdButton">
                                                        <div id="Div1" class="bcButtonDiv">
                                                            <asp:LinkButton runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
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
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" style="background-color: #F5F4F4; padding: 15px; border: solid 1px #ccc">
                                <tr>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Local Enquiry Sent')" onmouseover="this.style.cursor = 'pointer';"
                                        onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of FE's at LE-Stage :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblLes" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Foreign Quotation Submitted')"
                                        onmouseover="this.style.cursor = 'pointer';" onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Quotation Submited :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblfqs" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('FPO Received')" onmouseover="this.style.cursor = 'pointer';"
                                        onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Received FPO :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblfps" Text="0"></asp:Label></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('LPO Issued')" onmouseover="this.style.cursor = 'pointer';"
                                        onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Sent LPO :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lbllps" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Proforma Invoice Prepared')"
                                        onmouseover="this.style.cursor = 'pointer';" onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Completed P-Invoice :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblpis" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Foreign Enquiry Cancelled')"
                                        onmouseover="this.style.cursor = 'pointer';" onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Cancelled :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblcls" Text="0"></asp:Label></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Foreign Enquiry Received')"
                                        onmouseover="this.style.cursor = 'pointer';" onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Not Processed Yet :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblnps" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('Foreign PO Cancelled')"
                                        onmouseover="this.style.cursor = 'pointer';" onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Cancelled FPO :</span> <span>
                                            <asp:Label runat="server" CssClass="bcLabelright" ID="lblfpcs" Text="0"></asp:Label></span>
                                    </td>
                                    <td class="bcTdnormal" colspan="2" onclick="SearchStatus('')" onmouseover="this.style.cursor = 'pointer';"
                                        onmouseout="this.style.cursor = 'auto';">
                                        <span class="bcLabel">Number of Foreign Enquires Received :</span> <span>
                                            <asp:Label CssClass="bcLabelright" runat="server" ID="lbltfes" Text="0"></asp:Label></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                   <%-- <tr>
                        <td colspan="6" class="bcTdNewTable">
                            &nbsp;
                        </td>
                    </tr>--%>
                    <tr>
                        <td colspan="6">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:GridView runat="server" ID="gvFERpt" Width="100%" RowStyle-CssClass="bcGridViewRowStyle"
                                            AutoGenerateColumns="false" EmptyDataRowStyle-CssClass="bcGridViewEmptyDataRowStyle"
                                            PagerStyle-CssClass="bcGridViewPagerStyle" PagerStyle-HorizontalAlign="Center"
                                            CssClass="bcGridViewMain" HeaderStyle-CssClass="bcGridViewHeaderStyle" AlternatingRowStyle-CssClass="bcGridViewAlternatingRowStyle"
                                            OnPreRender="gvFERpt_PreRender">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No." ItemStyle-Width="10px">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Enquiry Number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("EnquireNumber") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Rcvd Date" DataField="ReceivedDate" HeaderStyle-Width="50px" />
                                                <asp:BoundField HeaderText="Customer Name" DataField="CustmrNm" HeaderStyle-Width="170px" />
                                                <asp:BoundField HeaderText="Created By" DataField="UserName" HeaderStyle-Width="170px" />
                                                <asp:BoundField HeaderText="Status" DataField="Status" HeaderStyle-Width="170px" />
                                                <%--<asp:BoundField HeaderText="Message" DataField="Body" HeaderStyle-Width="500px" />
                                                <asp:BoundField HeaderText="Sent Date" DataField="SentDate" HeaderStyle-Width="100px" />--%>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
    <link href="../JScript/media/css/TableTools_JUI.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media/css/TableTools.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media_ColVis/css/ColVis.css" rel="stylesheet" type="text/css" />
    <link href="../JScript/media_ColVis/css/ColVisAlt.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>

    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>

    <script src="../JScript/media/js/jquery.dataTables.min.js" type="text/javascript"></script>

    <script src="../JScript/jquery.sticky.js" type="text/javascript"></script>

    <script src="../JScript/media/js/TableTools.js" type="text/javascript"></script>

    <script src="../JScript/media/js/ZeroClipboard.js" type="text/javascript"></script>

    <script src="../JScript/media_ColVis/js/ColVis.js" type="text/javascript"></script>

    

    <script type="text/javascript">
        $(window).load(function() {
            $('[id$=divMyMessage]').sticky({ topSpacing: 0 });
        });


        $(document).ready(function () {
            var dateToday = new Date();
            $('[id$=txtFrmDt]').datepicker({
                dateFormat: 'mm-dd-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
            $('[id$=txtToDt]').datepicker({
                dateFormat: 'mm-dd-yy',
                changeMonth: true,
                changeYear: true,
                maxDate: dateToday
            });
        });
   

//    <%--To Disable previous dates based on first date picker selection--%>


        function changedate() {
            var strdateEnqDT = $('[id$=txtFrmDt]').val();
            var strdateEnqDT1 = strdateEnqDT.split('-');
            strdateEnqDT = (strdateEnqDT1[1] + '-' + strdateEnqDT1[0] + '-' + strdateEnqDT1[2]);
            strdateEnqDT = new Date(strdateEnqDT.replace(/-/g, "/"));
            $('[id$=txtToDt]').datepicker('option', {
                minDate: new Date(strdateEnqDT),
                maxDate: dateToday,
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true
            });
        } 
    </script>

    <script type="text/javascript">
        $(document).ready(function() {
            $("#<%=txtCstNm.ClientID %>").autocomplete({
                source: function(request, response) {
                    $.ajax({
                        url: '<%=ResolveUrl("~/Masters/AutoComplete.asmx/GetCustomersNm") %>',
                        data: "{ 'prefix': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function(data) {
                            response($.map(data.d, function(item) {
                                return {
                                    label: item.split('-')[0]
                                    , val: item.split('-')[1]
                                }
                            }))
                        }
                       
                    });
                }
                , select: function(e, i) {
                    $('[id$=hfCustomerId]').val(i.item.val);
                    //$("#<%=hfCustomerId.ClientID %>").val(i.item.val);
                }
                , minLength: 1
            });
        }); 
    </script>

    <script type="text/javascript">
        function Myvalidations() {
            var res = $('[id$=txtSubEml]').val();
            var res1 = $('[id$=txtFrmDt]').val();
            if (res.trim() == '' && res1.trim() == '') {
                $("#<%=divMyMessage.ClientID %> span").remove();
                $('[id$=divMyMessage]').append('<span class="Error">Subject/Mail-ID Or Dates are Required.</span>');
                $('[id$=txtSubEml]').focus();
                $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
                return false;
            }
            else {
                return true;
            }
        }
    </script>

    <script type="text/javascript">
        $(document).ready(function() {
                    TableTools.DEFAULTS.aButtons = ["copy", "csv", "xls", "pdf", "print",
                                                   { "sExtends": "collection", "sButtonText": "Save", 
                                                     "aButtons": [ "csv", "xls",
                                                            {
                                                            "sExtends": "pdf",
                                                            "sPdfMessage": "Your custom message would go here."
                                                            },
                                                            "print"
                                                        ]
                                                    }]

            $("[id$=gvFERpt]").dataTable({
                "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                "iDisplayLength": 10,
                "aaSorting": [[0, "asc"]],
                "bJQueryUI": true,
                "bAutoWidth": false,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                //Print, Export.........
                "sDom": 'T<"clear"><"H"lfr>t<"F"ip>',

                
                "sDom": 'TC<"clear">lfrtip',
                
                "oColVis":
                {
                    "sDom": 'C<"clear">lfrtip',
                    //"activate": "mouseover",
                    "bJQueryUI": true,
                    "bRestore": true,
                    "aiExclude": [0],
                    "buttonText": "<b>&nbsp;Change Columns&nbsp;&nbsp;</b>" //Show / hide columns
                },
                // -- End Of ColVis


                "oLanguage": {
                    "sZeroRecords": "There are no Records that match your search critera",
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
            oTable = $("[id$=gvFERpt]").dataTable();
        });

        function SearchStatus(valu) {
            var value1 = valu.toString();
            oTable.fnFilter(value1, 5);
        }
        
    </script>

</asp:Content>
