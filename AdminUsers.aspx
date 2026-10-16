<%@ Page Title="Users" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="AdminUsers.aspx.cs"
    Inherits="CyberArenaWebForms5.AdminUsers" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    User Directory | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>User Directory &amp; Roles</h1></div>
    <p class="page-sub">Manage accounts, roles and access across the platform.</p>
</asp:Content>

<asp:Content ID="ExtrasBlock" ContentPlaceHolderID="HeaderExtras" runat="server">
    <asp:Button ID="btnShowAdd" runat="server" Text="+ Add User" CssClass="button-primary"
        CausesValidation="false" OnClick="btnShowAdd_Click" />
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Total Users</div><div class="stat-value"><asp:Label ID="lblTotal" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Active</div><div class="stat-value" style="color:var(--green);"><asp:Label ID="lblActive" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Suspended</div><div class="stat-value" style="color:var(--red);"><asp:Label ID="lblSuspended" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Staff (Admin &amp; Moderator)</div><div class="stat-value" style="color:var(--primary);"><asp:Label ID="lblStaff" runat="server" /></div></div></div>
    </div>

    <asp:Label ID="lblPageMessage" runat="server" CssClass="message" EnableViewState="false" />

    <asp:Panel ID="pnlAdd" runat="server" Visible="false" CssClass="panel">
        <div class="panel-header"><h2>Add a user</h2></div>
        <div class="panel-body">
            <div class="team-form-grid">
                <div class="field-group"><label>Full name</label><asp:TextBox ID="txtNewName" runat="server" CssClass="team-input" MaxLength="60" /></div>
                <div class="field-group"><label>Username</label><asp:TextBox ID="txtNewUsername" runat="server" CssClass="team-input" MaxLength="20" /></div>
                <div class="field-group"><label>Email</label><asp:TextBox ID="txtNewEmail" runat="server" CssClass="team-input" MaxLength="100" /></div>
                <div class="field-group"><label>Temporary password</label><asp:TextBox ID="txtNewPassword" runat="server" CssClass="team-input" TextMode="Password" MaxLength="100" /></div>
                <div class="field-group"><label>Role</label>
                    <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="select-input" Width="100%">
                        <asp:ListItem>Player</asp:ListItem>
                        <asp:ListItem>Moderator</asp:ListItem>
                        <asp:ListItem>Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="team-form-actions">
                <asp:Button ID="btnCreateUser" runat="server" Text="Create user" CssClass="button-primary" OnClick="btnCreateUser_Click" />
                <asp:Button ID="btnCancelAdd" runat="server" Text="Cancel" CssClass="button-secondary" CausesValidation="false" OnClick="btnCancelAdd_Click" />
            </div>
        </div>
    </asp:Panel>

    <section class="panel">
        <asp:Panel ID="pnlToolbar" runat="server" DefaultButton="btnSearch" CssClass="toolbar panel-toolbar">
            <div class="search-box">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/></svg>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search name, username or email..." />
            </div>
            <div class="spacer"></div>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All roles</asp:ListItem>
                <asp:ListItem>Player</asp:ListItem>
                <asp:ListItem>Moderator</asp:ListItem>
                <asp:ListItem>Admin</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All status</asp:ListItem>
                <asp:ListItem>Active</asp:ListItem>
                <asp:ListItem>Suspended</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button-primary" OnClick="Filter_Changed" />
        </asp:Panel>

        <div class="table-scroll">
            <table class="data-table">
                <thead>
                    <tr><th>User</th><th>Email</th><th>Role</th><th>Status</th><th>Points</th><th>Last active</th><th>Actions</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand" OnItemDataBound="rptUsers_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="user-cell">
                                        <div class="avatar-sm <%# Eval("AvatarCss") %>"><%#: Eval("Initials") %></div>
                                        <div><strong><%#: Eval("FullName") %></strong><br /><span class="muted-sm">@<%#: Eval("Username") %></span></div>
                                    </div>
                                </td>
                                <td><%#: Eval("Email") %></td>
                                <td><span class="pill <%# Eval("RoleCss") %>"><%#: Eval("Role") %></span></td>
                                <td><span class="pill <%# Eval("StatusCss") %>"><span class="dot"></span><%#: Eval("Status") %></span></td>
                                <td><strong><%#: Eval("PointsText") %></strong></td>
                                <td class="muted-sm"><%#: Eval("LastActiveText") %></td>
                                <td>
                                    <div class="row-actions">
                                        <asp:DropDownList ID="ddlRowRole" runat="server" CssClass="select-input select-sm"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlRowRole_Changed"
                                            ToolTip="Change role">
                                            <asp:ListItem>Player</asp:ListItem>
                                            <asp:ListItem>Moderator</asp:ListItem>
                                            <asp:ListItem>Admin</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:LinkButton ID="btnToggle" runat="server" CssClass="act-link"
                                            CommandName="toggle" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"><%# (string)Eval("Status") == "Active" ? "Suspend" : "Activate" %></asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="act-link danger"
                                            CommandName="remove" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                            OnClientClick="return confirm('Delete this user permanently?');">Delete</asp:LinkButton>
                                    </div>
                                    <asp:HiddenField ID="hfUserId" runat="server" Value='<%# Eval("Id") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

        <div class="pager-bar panel-pager">
            <asp:Label ID="lblShowing" runat="server" />
            <div class="pager">
                <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click" CausesValidation="false">Previous</asp:LinkButton>
                <asp:Label ID="lblPageInfo" runat="server" CssClass="page" />
                <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click" CausesValidation="false">Next</asp:LinkButton>
            </div>
        </div>
    </section>

</asp:Content>
