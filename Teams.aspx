
<%@ Page Title="Teams" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Teams.aspx.cs"
    Inherits="CyberArenaWebForms5.Teams" %>

<asp:Content ID="TeamsTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Teams | CyberArena
</asp:Content>

<asp:Content ID="TeamsHeader" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>Teams</h1></div>
    <p class="page-sub">Collaborate with other players and solve cybersecurity challenges together.</p>
</asp:Content>

<asp:Content ID="TeamsExtras" ContentPlaceHolderID="HeaderExtras" runat="server">
    <asp:Button ID="btnShowCreate"
        runat="server"
        Text="+ Create Team"
        CssClass="button-primary"
        CausesValidation="false"
        OnClick="btnShowCreate_Click" />
</asp:Content>

<asp:Content ID="TeamsContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon blue"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Available Teams</div>
                <div class="stat-value">06</div>
                <div class="stat-note">Sample teams</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Your Team</div>
                <div class="stat-value" style="font-size:18px;">RootAccess</div>
                <div class="stat-note">Example membership</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon purple"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="8" r="3.2"/><path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"/><circle cx="17.5" cy="9" r="2.5"/><path d="M17 14.2c2.5.2 4.5 2.2 4.5 4.8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Team Members</div>
                <div class="stat-value">04</div>
                <div class="stat-note">Example team size</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon yellow"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 21h8M12 17v4"/><path d="M7 4h10v5a5 5 0 0 1-10 0V4Z"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Team Points</div>
                <div class="stat-value">3,300</div>
                <div class="stat-note">Sample score</div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlCreateTeam" runat="server"
        CssClass="panel team-form-panel" Visible="false">

        <div class="panel-header">
            <h2>Create a New Team</h2>
        </div>

        <div class="panel-body">
            <div class="team-form-grid">

                <div class="field-group">
                    <label for="<%= txtTeamName.ClientID %>">Team Name</label>
                    <asp:TextBox ID="txtTeamName" runat="server"
                        CssClass="team-input"
                        MaxLength="40"
                        placeholder="Enter a unique team name" />
                </div>

                <div class="field-group">
                    <label for="<%= txtTeamDescription.ClientID %>">Description</label>
                    <asp:TextBox ID="txtTeamDescription" runat="server"
                        CssClass="team-input"
                        MaxLength="200"
                        placeholder="What is your team about?" />
                </div>

            </div>

            <asp:Label ID="lblCreateMessage" runat="server"
                CssClass="team-message" EnableViewState="false" />

            <div class="team-form-actions">
                <asp:Button ID="btnCreateTeam" runat="server"
                    Text="Create Team"
                    CssClass="button-primary"
                    OnClick="btnCreateTeam_Click" />

                <asp:Button ID="btnCancelCreate" runat="server"
                    Text="Cancel"
                    CssClass="button-secondary"
                    CausesValidation="false"
                    OnClick="btnCancelCreate_Click" />
            </div>
        </div>
    </asp:Panel>

    <asp:Label ID="lblPageMessage" runat="server"
        CssClass="team-message" EnableViewState="false" />

    <div class="panel">
        <div class="panel-header" style="padding-bottom:4px;">
            <h2>Discover Teams</h2>

            <asp:DropDownList ID="ddlTeamFilter" runat="server"
                CssClass="select-input"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlTeamFilter_SelectedIndexChanged">
                <asp:ListItem Value="all">All Teams</asp:ListItem>
                <asp:ListItem Value="open">Open Teams</asp:ListItem>
                <asp:ListItem Value="full">Full Teams</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="panel-body">
            <div class="team-grid">

                <asp:Repeater ID="rptTeams" runat="server"
                    OnItemCommand="rptTeams_ItemCommand">

                    <ItemTemplate>
                        <div class="team-card">

                            <div class="team-card-top">
                                <div class="team-emblem">
                                    <%#: Eval("Initials") %>
                                </div>
                                <span class='pill <%# (string)Eval("Status") == "Full" ? "grey" : "green" %>'><%#: Eval("Status") %></span>
                            </div>

                            <h3><%#: Eval("Name") %></h3>
                            <p class="team-description">
                                <%#: Eval("Description") %>
                            </p>

                            <div class="team-details">
                                <span>Members</span>
                                <strong><%#: Eval("MemberCount") %> / 5</strong>
                            </div>

                            <div class="team-progress">
                                <div style='width:<%# Eval("Progress") %>%'></div>
                            </div>

                            <div class="team-details">
                                <span>Team Points</span>
                                <strong><%#: Eval("Points") %></strong>
                            </div>

                            <asp:Button ID="btnJoin" runat="server"
                                Text='<%# (string)Eval("Status") == "Full" ? "Team Full" : "Request to Join" %>'
                                CommandName="JoinTeam"
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass="button-primary team-action"
                                Enabled='<%# (string)Eval("Status") != "Full" %>'
                                CausesValidation="false" />

                        </div>
                    </ItemTemplate>

                </asp:Repeater>

            </div>
        </div>
    </div>

</asp:Content>
