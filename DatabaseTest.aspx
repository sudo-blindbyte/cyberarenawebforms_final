<%@ Page Title="Database Connection Test" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DatabaseTest.aspx.cs" Inherits="CyberArenaWebForms5.DatabaseTest" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Database Connection Test | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title">
        <h1>Database Connection Test</h1>
        <p class="page-sub">Verify database connectivity and configuration</p>
    </div>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width: 900px; margin: 0 auto;">

        <!-- Connection Configuration -->
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Connection Configuration</h2>
            </div>
            <div class="panel-body">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600; width: 200px;">Connection Name:</td>
                        <td style="padding: 12px;"><code>CyberArenaDb</code></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Data Source:</td>
                        <td style="padding: 12px;"><code>(LocalDb)\MSSQLLocalDB</code></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Database:</td>
                        <td style="padding: 12px;"><code>CyberArena</code></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Authentication:</td>
                        <td style="padding: 12px;"><code>Integrated Security (Windows Auth)</code></td>
                    </tr>
                    <tr>
                        <td style="padding: 12px; font-weight: 600;">Provider:</td>
                        <td style="padding: 12px;"><code>System.Data.SqlClient</code></td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Connection Test Results -->
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Connection Test</h2>
            </div>
            <div class="panel-body">
                <asp:Label ID="lblConnectionStatus" runat="server" style="font-size: 16px; font-weight: 600;"></asp:Label>
                <br /><br />
                <asp:Label ID="lblConnectionDetails" runat="server" style="color: var(--muted);"></asp:Label>
            </div>
        </div>

        <!-- Database Status -->
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Database Status</h2>
            </div>
            <div class="panel-body">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Database Size:</td>
                        <td style="padding: 12px;"><asp:Label ID="lblDatabaseSize" runat="server" Text="--" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Total Tables:</td>
                        <td style="padding: 12px;"><asp:Label ID="lblTableCount" runat="server" Text="--" /></td>
                    </tr>
                    <tr>
                        <td style="padding: 12px; font-weight: 600;">Last Connection:</td>
                        <td style="padding: 12px;"><asp:Label ID="lblLastConnection" runat="server" Text="--" /></td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Database Tables Summary -->
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Database Tables Overview</h2>
            </div>
            <div class="panel-body">
                <table style="width: 100%; border-collapse: collapse;">
                    <tr style="background-color: var(--bg-secondary); border-bottom: 2px solid var(--border);">
                        <td style="padding: 12px; font-weight: 600;">Table Name</td>
                        <td style="padding: 12px; font-weight: 600;">Rows</td>
                        <td style="padding: 12px; font-weight: 600;">Status</td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">dbo.AppUser</td>
                        <td style="padding: 12px;"><asp:Label ID="lblAppUserRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblAppUserCheck" runat="server" Text="--" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">dbo.Team</td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamCheck" runat="server" Text="--" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">dbo.Challenge</td>
                        <td style="padding: 12px;"><asp:Label ID="lblChallengeRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblChallengeCheck" runat="server" Text="--" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">dbo.Submission</td>
                        <td style="padding: 12px;"><asp:Label ID="lblSubmissionRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblSubmissionCheck" runat="server" Text="--" /></td>
                    </tr>
                    <tr style="border-bottom: 1px solid var(--border);">
                        <td style="padding: 12px;">dbo.TeamMember</td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamMemberRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblTeamMemberCheck" runat="server" Text="--" /></td>
                    </tr>
                    <tr>
                        <td style="padding: 12px;">dbo.AuditLog</td>
                        <td style="padding: 12px;"><asp:Label ID="lblAuditLogRows" runat="server" Text="--" /></td>
                        <td style="padding: 12px;"><asp:Label ID="lblAuditLogCheck" runat="server" Text="--" /></td>
                    </tr>
                </table>
            </div>
        </div>

        <!-- Actions -->
        <div class="panel" style="margin-bottom: 20px;">
            <div class="panel-header">
                <h2>Actions</h2>
            </div>
            <div class="panel-body">
                <div style="display: flex; gap: 10px; flex-wrap: wrap;">
                    <asp:Button ID="btnTestConnection" runat="server" CssClass="button-primary" 
                        Text="Test Connection" OnClick="btnTestConnection_Click" />
                    <asp:Button ID="btnRefreshStatus" runat="server" CssClass="button-secondary" 
                        Text="Refresh Status" OnClick="btnRefreshStatus_Click" />
                    <asp:LinkButton ID="btnViewSeeding" runat="server" CssClass="button-secondary" 
                        PostBackUrl="~/DataSeeding.aspx" CausesValidation="false" Text="Go to Data Seeding" />
                </div>
            </div>
        </div>

        <!-- Troubleshooting -->
        <div class="panel">
            <div class="panel-header">
                <h2>Troubleshooting</h2>
            </div>
            <div class="panel-body" style="line-height: 1.6;">
                <p><strong>If connection test fails:</strong></p>
                <ul style="margin-left: 20px; margin-top: 10px;">
                    <li>Ensure <strong>SQL Server LocalDB</strong> is installed on your machine</li>
                    <li>Verify the <strong>CyberArena</strong> database exists (check in Server Explorer)</li>
                    <li>Check that Web.config has the correct connection string</li>
                    <li>Make sure your Windows user has permissions to access LocalDB</li>
                </ul>

                <p style="margin-top: 15px;"><strong>If tables are missing:</strong></p>
                <ul style="margin-left: 20px; margin-top: 10px;">
                    <li>Click <strong>Refresh Status</strong> to reload table information</li>
                    <li>Visit the <strong>Data Seeding</strong> page to populate sample data</li>
                    <li>Check the Application logs for initialization errors</li>
                </ul>

                <p style="margin-top: 15px;"><strong>To view data in Server Explorer:</strong></p>
                <ul style="margin-left: 20px; margin-top: 10px;">
                    <li>Open <strong>View→ Server Explorer</strong> in Visual Studio</li>
                    <li>Right-click the CyberArena connection → <strong>Refresh</strong></li>
                    <li>Expand Tables and right-click table → <strong>Show Table Data</strong></li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>
