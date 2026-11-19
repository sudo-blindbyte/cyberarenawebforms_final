<%@ Page Title="Data Seeding Utility" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataSeeding.aspx.cs" Inherits="CyberArenaWebForms5.DataSeeding" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Data Seeding Utility | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title">
        <h1>Database Seeding Utility</h1>
        <p class="page-sub">View database status and populate seed data</p>
    </div>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width: 800px; margin: 0 auto;">
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Database Status</h2>
            </div>
            <div class="panel-body">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Table</td>
                        <td style="padding: 12px; font-weight: 600;">Row Count</td>
                        <td style="padding: 12px; font-weight: 600;">Status</td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">AppUser</td>
                        <td style="padding: 12px;"><asp:Label ID="lblAppUserCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblAppUserStatus" runat="server" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">Challenge</td>
                        <td style="padding: 12px;"><asp:Label ID="lblChallengeCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblChallengeStatus" runat="server" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">Submission</td>
                        <td style="padding: 12px;"><asp:Label ID="lblSubmissionCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblSubmissionStatus" runat="server" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">AuditLog</td>
                        <td style="padding: 12px;"><asp:Label ID="lblAuditLogCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblAuditLogStatus" runat="server" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">Team</td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamStatus" runat="server" /></td>
                    </tr>
                    <tr>
                        <td style="padding: 12px;">TeamMember</td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamMemberCount" runat="server" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamMemberStatus" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Actions</h2>
            </div>
            <div class="panel-body">
                <asp:Panel ID="pnlInfo" runat="server" class="message" style="margin-bottom: 15px; display: none;"></asp:Panel>

                <div style="display: flex; gap: 10px;">
                    <asp:Button ID="btnPopulateData" runat="server" CssClass="button-primary" 
                        Text="Populate Seed Data" OnClick="btnPopulateData_Click" />
                    <asp:Button ID="btnRefreshStatus" runat="server" CssClass="button-secondary" 
                        Text="Refresh Status" OnClick="btnRefreshStatus_Click" />
                </div>
            </div>
        </div>

        <div class="panel">
            <div class="panel-header">
                <h2>Instructions</h2>
            </div>
            <div class="panel-body" style="line-height: 1.6;">
                <p><strong>How to View Data in Server Explorer:</strong></p>
                <ol style="margin-left: 20px;">
                    <li>Click "Populate Seed Data" button to insert test data into the database</li>
                    <li>In Visual Studio, open <strong>View → Server Explorer</strong></li>
                    <li>Right-click on the connection to CyberArena database</li>
                    <li>Select <strong>"Refresh"</strong> to reload the connection</li>
                    <li>Expand the Tables folder and right-click on any table</li>
                    <li>Select <strong>"Show Table Data"</strong> to view the rows</li>
                </ol>
                <p style="margin-top: 15px; color: var(--muted); font-size: 12px;">
                    ℹ️ The seed data will only be inserted once. Subsequent clicks will not create duplicates.
                </p>
            </div>
        </div>
    </div>
</asp:Content>
