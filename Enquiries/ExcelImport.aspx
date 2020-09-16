<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ExcelImport.aspx.cs" Inherits="VOMS_ERP.Enquiries.ExcelImport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <center>
        <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple" AutoPostBack="true"
            OnSelectedIndexChanged="ListBox1_SelectedIndexChanged" Rows="10">
            <asp:ListItem Text="One" Value="One" />
            <asp:ListItem Text="Two" Value="Two" />
            <asp:ListItem Text="Three" Value="Three" />
            <asp:ListItem Text="Four" Value="Four" />
            <asp:ListItem Text="Five" Value="Five" />
            <asp:ListItem Text="Six" Value="Six" />
            <asp:ListItem Text="Seven" Value="Seven" />
            <asp:ListItem Text="Eight" Value="Eight" />
            <asp:ListItem Text="Nine" Value="Nine" />
            <asp:ListItem Text="Ten" Value="Ten" />
        </asp:ListBox>
        <br />
        <asp:Button ID="btnShow" runat="server" Text="Show" onclick="btnShow_Click" />
        <br />
        <asp:Label ID="lblDisplay" runat="server" Text="-"></asp:Label>
        <br />
        <br />
        <br />
        <asp:FileUpload ID="FileUpload1" runat="server" />
    </center>
</asp:Content>
