<%@ Page Title="Submission Details" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="SubmissionDetails.aspx.cs"
    Inherits="CyberArenaWebForms5.SubmissionDetails" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Submission Details | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <a href="AdminSubmissions.aspx" class="back-link">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 6-6 6 6 6"/></svg>
        Back to submissions
    </a>
    <div class="page-title">
        <h1>Submission Details</h1>
        <span class="pill blue"><asp:Label ID="lblSubId" runat="server" /></span>
    </div>
    <p class="page-sub">Review the submitted flag, the account behind it and the server trace.</p>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="card empty">
        Submission not found. <a href="AdminSubmissions.aspx" class="link-sm">Back to submissions →</a>
    </asp:Panel>

    <asp:Panel ID="pnlDetail" runat="server" CssClass="create-grid wide-right">

        <section class="panel form-card">
            <div class="panel-header"><h2>Submission Overview</h2></div>
            <div class="panel-body">

                <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" />

                <div class="detail-fields">
                    <div class="df"><span>User</span><strong><asp:Label ID="lblUser" runat="server" /></strong></div>
                    <div class="df"><span>Result</span><asp:Label ID="lblStatus" runat="server" CssClass="pill" /></div>
                    <div class="df"><span>Challenge</span><strong><asp:Label ID="lblChallenge" runat="server" /></strong></div>
                    <div class="df"><span>Points</span><strong><asp:Label ID="lblPoints" runat="server" /></strong></div>
                    <div class="df"><span>Submitted</span><strong><asp:Label ID="lblSubmitted" runat="server" /></strong></div>
                    <div class="df"><span>Attempt</span><strong><asp:Label ID="lblAttempt" runat="server" /></strong></div>
                </div>

                <div class="field-group" style="margin-top:18px;">
                    <label>Submitted flag (masked)</label>
                    <div class="code-box"><asp:Label ID="lblFlag" runat="server" /></div>
                </div>

                <div class="field-group" style="margin-top:16px;">
                    <label>Admin notes</label>
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="team-input" TextMode="MultiLine" Rows="3" MaxLength="500"
                        placeholder="Add an internal note about this submission..." />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnRevoke" runat="server" Text="Revoke points" CssClass="button-danger"
                        CausesValidation="false" OnClick="btnRevoke_Click"
                        OnClientClick="return confirm('Revoke the points for this submission?');" />
                    <span class="spacer"></span>
                    <asp:Button ID="btnSaveNotes" runat="server" Text="Save note" CssClass="button-secondary" CausesValidation="false" OnClick="btnSaveNotes_Click" />
                    <asp:Button ID="btnVerify" runat="server" Text="Mark verified" CssClass="button-primary" CausesValidation="false" OnClick="btnVerify_Click" />
                </div>
            </div>
        </section>

        <section class="terminal">
            <div class="terminal-head">
                <span>Server trace</span>
                <span class="terminal-sub"><asp:Label ID="lblIp" runat="server" /></span>
            </div>
            <div class="terminal-body">
                <asp:Literal ID="litTrace" runat="server" />
            </div>
        </section>

    </asp:Panel>

</asp:Content>
