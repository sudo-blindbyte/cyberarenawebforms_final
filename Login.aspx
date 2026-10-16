<%@ Page Title="Sign in" Language="C#"
    MasterPageFile="~/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="CyberArenaWebForms5.Login" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Sign in | CyberArena
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="auth-title">Welcome back</h1>
    <p class="auth-sub">Sign in to continue to your CyberArena account.</p>

    <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" />

    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnLogin">

        <div class="field-group">
            <label for="<%= txtIdentifier.ClientID %>">Username or email</label>
            <asp:TextBox ID="txtIdentifier" runat="server" CssClass="team-input"
                MaxLength="100" placeholder="you@example.com" autocomplete="username" />
        </div>

        <div class="field-group" style="margin-top:14px;">
            <label for="<%= txtPassword.ClientID %>">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="team-input"
                TextMode="Password" MaxLength="100" placeholder="########" autocomplete="current-password" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Sign in"
            CssClass="button-primary auth-submit" OnClick="btnLogin_Click" />
    </asp:Panel>

    <p class="auth-switch">New to CyberArena? <a href="Register.aspx">Create an account</a></p>


</asp:Content>
