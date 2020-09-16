<%@ Page  Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ModuleAccessMaster.aspx.cs" Inherits="VOMS_ERP.Admin.ModuleAccessMaster" %>

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
                        <td style="width: 25px;">
                            Company Name <font color="red" size="2"><b>*</b></font>:
                        </td>
                        <td style="width: 25px;" style="padding-top: 8px;">
                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="bcAspdropdown" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <div style="min-height: 10%; max-height: 15%; overflow: auto;">
                                <asp:TreeView ID="ModuleAcess" runat="server" ShowCheckBoxes="All" OnClick="OnTreeClick(event)">
                                </asp:TreeView>
                            </div>
                            <div id="label" style="display: none;">
                            </div>
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td class="bcTdButton">
                            <div id="Div1" style="width: 25px;" class="bcButtonDiv">
                                <asp:LinkButton runat="server" ID="btnSave" Width="1px" Text="Grant Access" OnClick="btnSave_Click" />
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

                //check if nxt sibling is not null & is an element node

                if (nxtSibling && nxtSibling.nodeType == 1) {

                    //if node has children   

                    if (nxtSibling.tagName.toLowerCase() == "div") {

                        //check or uncheck children at all levels

                        CheckUncheckChildren(parentTable.nextSibling, src.checked);

                    }

                }

                //check or uncheck parents at all levels

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

                //checkbox checked

                if (check) {

                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);

                    if (isAllSiblingsChecked)

                        checkUncheckSwitch = true;

                    else

                        return; //do not need to check parent if any(one or more) child not checked

                }

                else //checkbox unchecked
                {

                    checkUncheckSwitch = false;

                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");

                if (inpElemsInParentTable.length > 0) {

                    var parentNodeChkBox = inpElemsInParentTable[0];

                    parentNodeChkBox.checked = checkUncheckSwitch;

                    //do the same recursively

                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);

                }

            }

        }



        function AreAllSiblingsChecked(chkBox) {

            var parentDiv = GetParentByTagName("div", chkBox);

            var childCount = parentDiv.childNodes.length;

            for (var i = 0; i < childCount; i++) {

                if (parentDiv.childNodes[i].nodeType == 1) {

                    //check if the child node is an element node

                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {

                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];

                        //if any of sibling nodes are not checked, return false

                        if (!prevChkBox.checked) {

                            return false;

                        }

                    }

                }

            }

            return true;

        }

        //utility function to get the container of an element by tagname

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

        //returns NodeValue

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

            if (($('[id$=ddlCompany]').val()).trim() == '0') {
                ErrorMessage('Company Name is Required.');
                $('[id$=ddlCompany]').focus();
                return false;
            }
            //            else if (($('[id$=ModuleAcess]').val()).trim() == '') {
            //                ErrorMessage('Business Name is Required.');
            //                $('[id$=txtbsnm]').focus();
            //                return false;
            //            }
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
</asp:Content>
