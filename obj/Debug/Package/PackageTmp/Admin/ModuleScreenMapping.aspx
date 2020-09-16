<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ModuleScreenMapping.aspx.cs" Inherits="VOMS_ERP.Admin.ModuleScreenMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="MainTable" align="center">
        <tr>
            <td class="bcTdNewTable">
                <table style="width: 98%; vertical-align: top;" align="center">
                    <tr class="bcTRTitleRow">
                        <td class="bcTdTitleLeft" align="left" colspan="4">
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text="Module Access Master"
                                            CssClass="bcTdTitleLabel"></asp:Label><div id="divMyMessage" runat="server" align="center"
                                                class="formError1" />
                                    </td>
                                    <td colspan="2" style="text-align: right;">
                                        <span id="Span6" class="bcLabelright">All <font color="red" size="4"><b>*</b></font>fields
                                            are Mandatory</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Company Name <font color="red" size="2"><b>*</b></font>:
                        </td>
                        <td style="padding-top: 8px;">
                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            Module Name <font color="red" size="2"><b>*</b></font>:
                        </td>
                        <td style="padding-top: 8px;">
                            <asp:DropDownList runat="server" ID="ddlModuleName" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlModuleName_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Screen Name <font color="red" size="2"><b>*</b></font>:
                        </td>
                        <td style="padding-top: 8px;">
                            <asp:DropDownList runat="server" ID="ddlScreen" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlScreen_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td class="bcTdButton">
                            <div id="Div1" style="width: 25px;" class="bcButtonDiv">
                                <asp:LinkButton runat="server" ID="btnSave" Width="1px" Text="Save" OnClick="btnSave_Click" />
                            </div>
                        </td>
                        <td style="width: 25px;" align="center" valign="middle">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <script src="../JScript/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="../JScript/jquery-ui-1.9.1.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function OnTreeClick(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
            if (isChkBoxClick) {
                if (src.checked == true) {
                    var nodeText = getNextSibling(src).innerText || getNextSibling(src).innerHTML;
                    var nodeValue = GetNodeValue(getNextSibling(src));
                    document.getElementById("label").innerHTML += nodeText + ",";
                }
                else {
                    var nodeText = getNextSibling(src).innerText || getNextSibling(src).innerHTML;
                    var nodeValue = GetNodeValue(getNextSibling(src));
                    var val = document.getElementById("label").innerHTML;
                    document.getElementById("label").innerHTML = val.replace(nodeText + ",", "");
                }

                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType == 1) {
                    if (nxtSibling.tagName.toLowerCase() == "div") {
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                CheckUncheckParents(src, src.checked);
            }
        }

        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }

        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;
            if (parentNodeTable) {
                var checkUncheckSwitch;
                if (check) {
                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                    if (isAllSiblingsChecked)
                        checkUncheckSwitch = true;
                    else
                        return;
                }
                else {
                    checkUncheckSwitch = false;
                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }

        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }

        function getNextSibling(element) {
            var n = element;
            do n = n.nextSibling;
            while (n && n.nodeType != 1);
            return n;
        }

        function GetNodeValue(node) {
            var nodeValue = "";
            var nodePath = node.href.substring(node.href.indexOf(",") + 2, node.href.length - 2);
            var nodeValues = nodePath.split("\\");
            if (nodeValues.length > 1)
                nodeValue = nodeValues[nodeValues.length - 1];
            else
                nodeValue = nodeValues[0].substr(1);
            return nodeValue;
        } 

    </script>
    <script type="text/javascript">
        function Myvalidations() {
            if (($('[id$=ddlCompany]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Company Name is Required.');
                $('[id$=ddlCompany]').focus();
                return false;
            }
            else if (($('[id$=ddlScreen]').val()).trim() == '00000000-0000-0000-0000-000000000000') {
                ErrorMessage('Screen Name is Required.');
                $('[id$=ddlScreen]').focus();
                return false;
            }
            else if (($('[id$=txtSequenceNumber]').val()).trim() == '') {
                ErrorMessage('Sequence Number is Required.');
                $('[id$=txtSequenceNumber]').focus();
                return false;
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
</asp:Content>
