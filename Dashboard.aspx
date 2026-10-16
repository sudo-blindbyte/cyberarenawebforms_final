<%@ Page Title="Dashboard" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="CyberArenaWebForms5.Dashboard" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title">
        <h1>Dashboard</h1>
        <span class="pill red live"><span class="dot"></span>Live event - Season 01 ends in 2d 14h</span>
    </div>
    <p class="page-sub">Welcome back, <asp:Label ID="lblUserName" runat="server" Text="Participant" />.</p>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Stat cards -->
    <div class="stats-grid">

        <div class="stat-card">
            <div class="stat-icon yellow">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 21h8M12 17v4"/><path d="M7 4h10v5a5 5 0 0 1-10 0V4Z"/><path d="M17 5h3v2a3 3 0 0 1-3 3M7 5H4v2a3 3 0 0 0 3 3"/></svg>
            </div>
            <div class="stat-body">
                <div class="stat-label">Total Points</div>
                <div class="stat-value"><asp:Label ID="lblPoints" runat="server" /></div>
            </div>
            <span class="pill green stat-pill">+100 this week</span>
        </div>

        <div class="stat-card">
            <div class="stat-icon blue">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M5 21V4"/><path d="M5 4h11l-1.5 4L16 12H5"/></svg>
            </div>
            <div class="stat-body">
                <div class="stat-label">Challenges Solved</div>
                <div class="stat-value"><asp:Label ID="lblSolved" runat="server" /><small>/ <asp:Label ID="lblTotalChallenges" runat="server" /></small></div>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icon purple">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="9" r="6"/><path d="m8.5 14 -1.5 7 5-3 5 3-1.5-7"/></svg>
            </div>
            <div class="stat-body">
                <div class="stat-label">Current Rank</div>
                <div class="stat-value"><asp:Label ID="lblRank" runat="server" /></div>
            </div>
            <span class="pill purple stat-pill">Top 20%</span>
        </div>

        <div class="stat-card">
            <div class="stat-icon cyan">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg>
            </div>
            <div class="stat-body">
                <div class="stat-label">Team</div>
                <div class="stat-value" style="font-size:18px;"><asp:Label ID="lblTeamName" runat="server" /></div>
                <div class="stat-note">4 members</div>
            </div>
            <span class="pill cyan stat-pill">Rank #3</span>
        </div>

    </div>

    <div class="dash-grid">

        <!-- Progress overview -->
        <section class="panel">
            <div class="panel-header">
                <h2>Progress Overview</h2>
                <div class="segmented"><span>7D</span><span class="on">30D</span><span>All</span></div>
            </div>

            <div class="chart-wrap">
                <asp:Literal ID="litProgressChart" runat="server" />
            </div>

            <div class="chart-foot">
                <div>
                    <strong>Trending Up: Keep it up!</strong>
                    <span>You're solving faster than last week.</span>
                </div>
                <div class="gain">+18%</div>
            </div>
        </section>

        <!-- Category donut -->
        <section class="panel">
            <div class="panel-header"><h2>Challenges by Category</h2></div>
            <div class="panel-body">
                <div class="donut-wrap">
                    <asp:Literal ID="litDonut" runat="server" />
                    <div class="donut-center">
                        <span class="donut-total"><asp:Label ID="lblDonutTotal" runat="server" /></span>
                        <small>solved</small>
                    </div>
                </div>

                <ul class="legend">
                    <asp:Repeater ID="rptCategories" runat="server">
                        <ItemTemplate>
                            <li>
                                <span class="swatch" style='background:<%# Eval("Color") %>'></span>
                                <span class="name"><%#: Eval("Name") %></span>
                                <span class="count"><%# Eval("Solved") %></span>
                                <span class="total">/ <%# Eval("Total") %></span>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </section>

        <!-- Right column -->
        <div class="dash-col">

            <section class="panel">
                <div class="panel-header">
                    <h2>Team: <asp:Label ID="lblTeamHeading" runat="server" /></h2>
                    <a href="Teams.aspx">View teams </a>
                </div>
                <div class="panel-body">
                    <div class="team-summary">
                        <span>Team total <strong>3,300 pts</strong></span>
                        <span class="positive">+120 today</span>
                    </div>

                    <asp:Repeater ID="rptMembers" runat="server">
                        <ItemTemplate>
                            <div class="list-row">
                                <div class="avatar-sm <%# Eval("Color") %>"><%#: Eval("Initials") %></div>
                                <div class="meta">
                                    <strong><%#: Eval("Name") %></strong>
                                    <span><%#: Eval("Role") %></span>
                                </div>
                                <div class="val"><%# Eval("Points", "{0:N0}") %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>

            <section class="panel">
                <div class="panel-header">
                    <h2>Recent Activity</h2>
                    <a href="Submissions.aspx">View all</a>
                </div>
                <div class="panel-body">
                    <asp:Repeater ID="rptActivity" runat="server">
                        <ItemTemplate>
                            <div class="activity-card">
                                <div class="ic <%# Eval("Css") %>">
                                    <%# (bool)Eval("Success")
                                        ? "<svg viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.4' stroke-linecap='round' stroke-linejoin='round'><path d='m5 12 5 5 9-10'/></svg>"
                                        : "<svg viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2.4' stroke-linecap='round'><path d='M6 6l12 12M18 6 6 18'/></svg>" %>
                                </div>
                                <div class="meta">
                                    <strong><%#: Eval("Title") %></strong>
                                    <span><%#: Eval("When") %></span>
                                </div>
                                <span class="pill <%# Eval("Css") %>"><%#: Eval("Result") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>

        </div>
    </div>

</asp:Content>
