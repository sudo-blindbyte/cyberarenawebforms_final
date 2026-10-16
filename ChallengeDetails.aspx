<%@ Page Title="Challenge" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ChallengeDetails.aspx.cs"
    Inherits="CyberArenaWebForms5.ChallengeDetails" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Challenge | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <a href="Challenges.aspx" class="back-link">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 6-6 6 6 6"/></svg>
        Back to Challenges
    </a>
    <div class="page-title"><h1><asp:Label ID="lblTitle" runat="server" /></h1></div>
    <div style="display:flex; gap:8px; margin-top:10px; flex-wrap:wrap;">
        <asp:Label ID="lblCategoryPill" runat="server" CssClass="pill blue" />
        <asp:Label ID="lblDifficultyPill" runat="server" CssClass="pill" />
        <asp:Label ID="lblStatusPill" runat="server" CssClass="pill" />
    </div>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="card empty">
        We couldn't find that challenge. <a href="Challenges.aspx" class="link-sm">Back to all challenges →</a>
    </asp:Panel>

    <asp:Panel ID="pnlDetail" runat="server" CssClass="detail-grid">

        <div>
            <div class="detail-section">
                <h3>Description</h3>
                <p><asp:Label ID="lblDescription" runat="server" /></p>
            </div>

            <div class="detail-section">
                <h3>Attachments</h3>
                <div class="attachment">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="m21 11-8.5 8.5a5 5 0 0 1-7-7L14 4a3.5 3.5 0 0 1 5 5l-8.5 8.5a2 2 0 0 1-3-3L15 7"/></svg>
                    <span class="name"><asp:Label ID="lblAttachment" runat="server" /></span>
                    <asp:LinkButton ID="lnkDownload" runat="server" OnClick="lnkDownload_Click"
                        CausesValidation="false">Download</asp:LinkButton>
                </div>
            </div>

            <div class="detail-section">
                <h3>Hint</h3>
                <asp:Panel ID="pnlHint" runat="server" Visible="false">
                    <p><asp:Label ID="lblHint" runat="server" /></p>
                </asp:Panel>
                <asp:Button ID="btnHint" runat="server" Text="Reveal hint (−10 pts)"
                    CssClass="button-secondary" CausesValidation="false" OnClick="btnHint_Click" />
            </div>

            <div class="detail-section">
                <h3>Submit Flag</h3>
                <asp:Label ID="lblResult" runat="server" EnableViewState="false" CssClass="message" />
                <asp:Panel ID="pnlFlag" runat="server" DefaultButton="btnSubmit" CssClass="flag-row">
                    <asp:TextBox ID="txtFlag" runat="server" CssClass="team-input"
                        MaxLength="120" placeholder="CTF{...}" autocomplete="off" />
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                        CssClass="button-primary" OnClick="btnSubmit_Click" />
                </asp:Panel>
            </div>
        </div>

        <aside class="card info-card">
            <h2>Challenge Info</h2>
            <div class="row"><span>Points</span><strong><asp:Label ID="lblPoints" runat="server" /></strong></div>
            <div class="row"><span>Solves</span><strong><asp:Label ID="lblSolves" runat="server" /></strong></div>
            <div class="row"><span>Success Rate</span><strong><asp:Label ID="lblRate" runat="server" /></strong></div>
        </aside>

    </asp:Panel>

</asp:Content>
