<%@ Page Async="true" Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EmoSearch._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Table runat="server">
        <asp:TableRow>
            <asp:TableHeaderCell>Happiness</asp:TableHeaderCell>
            <asp:TableHeaderCell>Anger</asp:TableHeaderCell>
            <asp:TableHeaderCell>Surprise</asp:TableHeaderCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell><asp:Image Height="300" Width="300" id="ha" runat="server"/></asp:TableCell>
            <asp:TableCell><asp:Image Height="300" Width="300" id="an" runat="server"/></asp:TableCell>
            <asp:TableCell><asp:Image Height="300" Width="300" id="su" runat="server"/></asp:TableCell>
        </asp:TableRow>
    </asp:Table>

</asp:Content>
