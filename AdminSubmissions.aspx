<%@ Page Title="Submissions" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="AdminSubmissions.aspx.cs"
    Inherits="CyberArenaWebForms5.AdminSubmissions" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Submissions | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>Submissions</h1></div>
    <p class="page-sub">Review flag submissions, verify results and investigate suspicious activity.</p>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Total Submissions</div><div class="stat-value"><asp:Label ID="lblTotal" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Accepted / Verified</div><div class="stat-value" style="color:var(--green);"><asp:Label ID="lblAccepted" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Rejected</div><div class="stat-value" style="color:var(--orange);"><asp:Label ID="lblRejected" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Revoked</div><div class="stat-value" style="color:var(--red);"><asp:Label ID="lblRevoked" runat="server" /></div></div></div>
    </div>

    <section class="panel">
        <asp:Panel ID="pnlToolbar" runat="server" DefaultButton="btnSearch" CssClass="toolbar panel-toolbar">
            <div class="search-box">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/></svg>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by user, challenge or ID..." />
            </div>
            <div class="spacer"></div>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All results</asp:ListItem>
                <asp:ListItem>Accepted</asp:ListItem>
                <asp:ListItem>Verified</asp:ListItem>
                <asp:ListItem>Rejected</asp:ListItem>
                <asp:ListItem>Revoked</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button-primary" OnClick="Filter_Changed" />
        </asp:Panel>

        <div class="table-scroll">
            <table class="data-table">
                <thead>
                    <tr><th>ID</th><th>User</th><th>Challenge</th><th>Submitted</th><th>Attempt</th><th>Result</th><th>Points</th><th></th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptSubmissions" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><a class="act-link" href='SubmissionDetails.aspx?id=<%# Eval("Id") %>'><%#: Eval("Id") %></a></td>
                                <td><strong><%#: Eval("User") %></strong></td>
                                <td><%#: Eval("Challenge") %></td>
                                <td class="muted-sm"><%#: Eval("SubmittedText") %></td>
                                <td>#<%# Eval("Attempt") %></td>
                                <td><span class="pill <%# Eval("StatusCss") %>"><span class="dot"></span><%#: Eval("Status") %></span></td>
                                <td><strong><%#: Eval("PointsText") %></strong></td>
                                <td><a class="act-link" href='SubmissionDetails.aspx?id=<%# Eval("Id") %>'>Review →</a></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty">No submissions match your filters.</asp:Panel>
        </div>
    </section>

</asp:Content>
