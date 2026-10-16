
<%@ Page Title="Leaderboard" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Leaderboard.aspx.cs"
    Inherits="CyberArenaWebForms5.Leaderboard" %>

<asp:Content ID="LeaderboardTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Leaderboard | CyberArena
</asp:Content>

<asp:Content ID="LeaderboardHeader" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title">
        <h1>Leaderboard</h1>
        <span class="pill blue">Season 01</span>
    </div>
    <p class="page-sub">Track the top performers competing in CyberArena.</p>
</asp:Content>

<asp:Content ID="LeaderboardContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon blue"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Registered Players</div>
                <div class="stat-value">128</div>
                <div class="stat-note">Across all competitions</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M5 21V4"/><path d="M5 4h11l-1.5 4L16 12H5"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Challenges Solved</div>
                <div class="stat-value">864</div>
                <div class="stat-note">Combined player solves</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon purple"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="9" r="6"/><path d="m8.5 14-1.5 7 5-3 5 3-1.5-7"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Your Rank</div>
                <div class="stat-value">#24</div>
                <div class="stat-note">Sample participant position</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon yellow"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 21h8M12 17v4"/><path d="M7 4h10v5a5 5 0 0 1-10 0V4Z"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Your Score</div>
                <div class="stat-value">450</div>
                <div class="stat-note">Total points earned</div>
            </div>
        </div>
    </div>

    <div class="panel">
        <div class="panel-header" style="padding-bottom:12px;">
            <h2>Global Rankings</h2>

            <asp:DropDownList ID="ddlRankingType"
                runat="server"
                AutoPostBack="true"
                CssClass="select-input"
                OnSelectedIndexChanged="ddlRankingType_SelectedIndexChanged">
                <asp:ListItem Value="individual">Individual</asp:ListItem>
                <asp:ListItem Value="team">Teams</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div style="overflow-x:auto;">
            <asp:GridView ID="gvLeaderboard"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="data-table"
                GridLines="None"
                Width="100%"
                EmptyDataText="No leaderboard entries available.">

                <Columns>
                    <asp:TemplateField HeaderText="Rank">
                        <ItemTemplate>
                            <span class='rank-badge <%# Convert.ToInt32(Eval("Rank")) <= 3 ? "r" + Eval("Rank") : "" %>'><%# Eval("Rank") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Player">
                        <ItemTemplate>
                            <strong><%#: Eval("Username") %></strong>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Team">
                        <ItemTemplate>
                            <span class="pill grey"><%#: Eval("Team") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Solved"
                        HeaderText="Solved" />

                    <asp:TemplateField HeaderText="Points">
                        <ItemTemplate>
                            <strong><%# Eval("Points", "{0:N0}") %></strong>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="panel-body" style="padding-top:12px;">
            <asp:Label ID="lblLeaderboardMessage"
                runat="server"
                EnableViewState="false"
                CssClass="stat-note" />
        </div>
    </div>

</asp:Content>
