<%@ Page Title="Challenge Repository" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="AdminChallenges.aspx.cs"
    Inherits="CyberArenaWebForms5.AdminChallenges" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Challenge Repository | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>Challenge Repository</h1></div>
    <p class="page-sub">Create, edit, publish and retire challenges.</p>
</asp:Content>

<asp:Content ID="ExtrasBlock" ContentPlaceHolderID="HeaderExtras" runat="server">
    <a href="CreateChallenge.aspx" class="button-primary">+ New Challenge</a>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Total Challenges</div><div class="stat-value"><asp:Label ID="lblTotal" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Published</div><div class="stat-value" style="color:var(--green);"><asp:Label ID="lblPublished" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Drafts</div><div class="stat-value" style="color:var(--primary);"><asp:Label ID="lblDrafts" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Avg. Points</div><div class="stat-value" style="color:var(--orange);"><asp:Label ID="lblAvg" runat="server" /></div></div></div>
    </div>

    <asp:Label ID="lblPageMessage" runat="server" CssClass="message" EnableViewState="false" />

    <section class="panel">
        <asp:Panel ID="pnlToolbar" runat="server" DefaultButton="btnSearch" CssClass="toolbar panel-toolbar">
            <div class="search-box">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/></svg>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search challenges..." />
            </div>
            <div class="spacer"></div>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All categories</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="ddlState" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All states</asp:ListItem>
                <asp:ListItem Value="Published">Published</asp:ListItem>
                <asp:ListItem Value="Draft">Draft</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button-primary" OnClick="Filter_Changed" />
        </asp:Panel>

        <div class="table-scroll">
            <table class="data-table">
                <thead>
                    <tr><th>Title</th><th>Category</th><th>Difficulty</th><th>Points</th><th>Solves</th><th>State</th><th>Actions</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptChallenges" runat="server" OnItemCommand="rptChallenges_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><strong><%#: Eval("Title") %></strong><br /><span class="muted-sm"><%#: Eval("Subtitle") %></span></td>
                                <td><%#: Eval("Category") %></td>
                                <td><span class="pill <%# Eval("DifficultyCss") %>"><%#: Eval("Difficulty") %></span></td>
                                <td><strong><%# Eval("Points") %></strong></td>
                                <td><%# Eval("Solves") %></td>
                                <td><span class="pill <%# Eval("StateCss") %>"><%#: Eval("StateText") %></span></td>
                                <td>
                                    <div class="row-actions">
                                        <a class="act-link" href='CreateChallenge.aspx?id=<%# Eval("Id") %>'>Edit</a>
                                        <asp:LinkButton ID="btnPublish" runat="server" CssClass="act-link"
                                            CommandName="publish" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"><%# (bool)Eval("IsPublished") ? "Unpublish" : "Publish" %></asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="act-link danger"
                                            CommandName="remove" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                            OnClientClick="return confirm('Delete this challenge permanently?');">Delete</asp:LinkButton>
                                    </div>
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
