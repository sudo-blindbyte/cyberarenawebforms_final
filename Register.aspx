<%@ Page Title="Create account" Language="C#"
    MasterPageFile="~/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="CyberArenaWebForms5.Register" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Create account | CyberArena
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="auth-title">Create your account</h1>
    <p class="auth-sub">Join CyberArena and start solving challenges today.</p>

    <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" />

    <asp:Panel ID="pnlRegister" runat="server" DefaultButton="btnRegister">

        <div class="field-group">
            <label for="<%= txtFullName.ClientID %>">Full name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="team-input" MaxLength="60" placeholder="Jane Doe" autocomplete="name" />
        </div>

        <div class="auth-two">
            <div class="field-group">
                <label for="<%= txtUsername.ClientID %>">Username</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="team-input" MaxLength="20" placeholder="jane_doe" autocomplete="username" />
            </div>
            <div class="field-group">
                <label for="<%= txtEmail.ClientID %>">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="team-input" MaxLength="100" placeholder="jane@example.com" autocomplete="email" />
            </div>
        </div>

        <div class="auth-two">
            <div class="field-group">
                <label for="<%= txtPassword.ClientID %>">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="team-input" TextMode="Password" MaxLength="100" placeholder="At least 8 characters" autocomplete="new-password" />
            </div>
            <div class="field-group">
                <label for="<%= txtConfirm.ClientID %>">Confirm password</label>
                <asp:TextBox ID="txtConfirm" runat="server" CssClass="team-input" TextMode="Password" MaxLength="100" placeholder="Repeat password" autocomplete="new-password" />
            </div>
        </div>

        <p class="auth-hint">Use at least 8 characters with an uppercase letter, a lowercase letter and a number.</p>

        <asp:Button ID="btnRegister" runat="server" Text="Create account"
            CssClass="button-primary auth-submit" OnClick="btnRegister_Click" />
    </asp:Panel>

    <p class="auth-switch">Already have an account? <a href="Login.aspx">Sign in</a></p>

</asp:Content>
