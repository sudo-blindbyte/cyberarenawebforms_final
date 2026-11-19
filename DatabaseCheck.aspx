<%@ Page Title="Database Connection Check" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DatabaseCheck.aspx.cs" Inherits="CyberArenaWebForms5.DatabaseCheck" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Database Check
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderExtras" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 20px; font-family: monospace;">
        <h2>Database Connection Status</h2>
        <asp:Label ID="lblStatus" runat="server" Style="white-space: pre-wrap; font-size: 14px;"></asp:Label>
    </div>
</asp:Content>
