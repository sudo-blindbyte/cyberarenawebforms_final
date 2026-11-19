<%@ Page Title="Forgot Password" Language="C#"
    MasterPageFile="~/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="ForgotPassword.aspx.cs"
    Inherits="CyberArenaWebForms5.ForgotPassword" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Forgot Password | CyberArena
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="auth-title">Reset your password</h1>
    <p class="auth-sub">Enter your email or username and we'll help you reset your password.</p>

    <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" />

    <asp:Panel ID="pnlRequest" runat="server">
        <div class="field-group">
            <label for="<%= txtIdentifier.ClientID %>">Email or Username</label>
            <asp:TextBox ID="txtIdentifier" runat="server" CssClass="team-input"
                MaxLength="100" placeholder="your@email.com or username" autocomplete="username" />
        </div>

        <asp:Button ID="btnResetRequest" runat="server" Text="Send Reset Link"
            CssClass="button-primary auth-submit" OnClick="btnResetRequest_Click" />
    </asp:Panel>

    <asp:Panel ID="pnlReset" runat="server" Visible="false">
        <div class="field-group">
            <label for="<%= txtNewPassword.ClientID %>">New Password</label>
            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="team-input"
                TextMode="Password" MaxLength="100" placeholder="New password" autocomplete="new-password" />
        </div>

        <div class="field-group">
            <label for="<%= txtConfirmPassword.ClientID %>">Confirm Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="team-input"
                TextMode="Password" MaxLength="100" placeholder="Confirm password" autocomplete="new-password" />
        </div>

        <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password"
            CssClass="button-primary auth-submit" OnClick="btnResetPassword_Click" />
    </asp:Panel>

    <p class="auth-switch"><a href="Login.aspx">Back to sign in</a></p>

</asp:Content>
