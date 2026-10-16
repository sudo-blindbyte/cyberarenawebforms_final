<%@ Page Title="Admin Overview" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="CyberArenaWebForms5.AdminDashboard" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Admin Overview | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>Admin Overview</h1></div>
    <p class="page-sub">Platform health, activity and moderation at a glance.</p>
</asp:Content>

<asp:Content ID="ExtrasBlock" ContentPlaceHolderID="HeaderExtras" runat="server">
    <asp:Button ID="btnExport" runat="server" Text="Export logs" CssClass="button-secondary"
        CausesValidation="false" OnClick="btnExport_Click" />
    <a href="CreateChallenge.aspx" class="button-primary">+ New Challenge</a>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon blue"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Total Users</div>
                <div class="stat-value"><asp:Label ID="lblUsers" runat="server" /></div>
                <div class="stat-note positive"><asp:Label ID="lblActiveUsers" runat="server" /> active</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M5 21V4"/><path d="M5 4h11l-1.5 4L16 12H5"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Live Challenges</div>
                <div class="stat-value"><asp:Label ID="lblChallenges" runat="server" /></div>
                <div class="stat-note positive"><asp:Label ID="lblDrafts" runat="server" /> drafts</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon yellow"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 6h12M8 12h12M8 18h12"/><circle cx="4" cy="6" r=".8"/><circle cx="4" cy="12" r=".8"/><circle cx="4" cy="18" r=".8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Submissions</div>
                <div class="stat-value"><asp:Label ID="lblSubmissions" runat="server" /></div>
                <div class="stat-note" style="color:var(--orange);"><asp:Label ID="lblRejected" runat="server" /> rejected</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon purple"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M12 3 2 20h20L12 3Z"/><path d="M12 10v5M12 18v.01"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Open Incidents</div>
                <div class="stat-value"><asp:Label ID="lblIncidents" runat="server" /></div>
                <div class="stat-note" style="color:var(--purple);"><asp:Label ID="lblCritical" runat="server" /> critical</div>
            </div>
        </div>
    </div>

    <div class="admin-grid">

        <div class="dash-col">
            <section class="panel">
                <div class="panel-header">
                    <div>
                        <h2>Platform Activity</h2>
                        <div class="panel-sub">Submissions per day, last 8 days</div>
                    </div>
                    <span class="pill blue">Live</span>
                </div>
                <div class="chart-wrap">
                    <asp:Literal ID="litActivityChart" runat="server" />
                </div>
            </section>

            <section class="panel">
                <div class="panel-header">
                    <h2>Recent Submissions</h2>
                    <a href="AdminSubmissions.aspx">View all</a>
                </div>
                <div class="panel-body">
                    <asp:Repeater ID="rptRecent" runat="server">
                        <ItemTemplate>
                            <a class="list-row row-link" href='SubmissionDetails.aspx?id=<%# Eval("Id") %>'>
                                <div class="meta">
                                    <strong><%#: Eval("User") %> · <%#: Eval("Challenge") %></strong>
                                    <span><%#: Eval("Id") %> · <%#: Eval("SubmittedText") %></span>
                                </div>
                                <span class="pill <%# Eval("StatusCss") %>"><%#: Eval("Status") %></span>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </div>

        <div class="dash-col">
            <section class="panel">
                <div class="panel-header"><h2>Quick Actions</h2></div>
                <div class="panel-body">
                    <a class="quick-action" href="CreateChallenge.aspx"><span>Create a challenge </span><b></b></a>
                    <a class="quick-action" href="AdminUsers.aspx"><span>Manage users &amp; roles</span><b></b></a>
                    <a class="quick-action" href="AdminIncidents.aspx"><span>Review incident reports</span><b></b></a>
                </div>
            </section>

            <section class="panel">
                <div class="panel-header"><h2>System Health</h2></div>
                <div class="panel-body">
                    <div class="health-row"><div class="health-top"><span>Web server</span><strong class="positive">Operational</strong></div><div class="team-progress"><div style="width:42%"></div></div></div>
                    <div class="health-row"><div class="health-top"><span>Scoring engine</span><strong class="positive">Operational</strong></div><div class="team-progress"><div style="width:28%"></div></div></div>
                    <div class="health-row"><div class="health-top"><span>File storage</span><strong style="color:var(--orange);">64% used</strong></div><div class="team-progress"><div style="width:64%; background:#f59e0b;"></div></div></div>
                </div>
            </section>
        </div>

    </div>

</asp:Content>
