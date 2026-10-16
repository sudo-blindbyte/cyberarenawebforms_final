<%@ Page Title="Incident Reports" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="AdminIncidents.aspx.cs"
    Inherits="CyberArenaWebForms5.AdminIncidents" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Incident Reports | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>Incident Reports &amp; Audit Logs</h1></div>
    <p class="page-sub">Track reported incidents and review every important action on the platform.</p>
</asp:Content>

<asp:Content ID="ExtrasBlock" ContentPlaceHolderID="HeaderExtras" runat="server">
    <asp:Button ID="btnExport" runat="server" Text="Export logs" CssClass="button-primary"
        CausesValidation="false" OnClick="btnExport_Click" />
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Total Reports</div><div class="stat-value"><asp:Label ID="lblTotal" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Open</div><div class="stat-value" style="color:var(--orange);"><asp:Label ID="lblOpen" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Critical</div><div class="stat-value" style="color:var(--red);"><asp:Label ID="lblCritical" runat="server" /></div></div></div>
        <div class="stat-card"><div class="stat-body"><div class="stat-label">Resolved</div><div class="stat-value" style="color:var(--green);"><asp:Label ID="lblResolved" runat="server" /></div></div></div>
    </div>

    <asp:Label ID="lblPageMessage" runat="server" CssClass="message" EnableViewState="false" />

    <section class="panel">
        <asp:Panel ID="pnlToolbar" runat="server" DefaultButton="btnSearch" CssClass="toolbar panel-toolbar">
            <div class="search-box">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/></svg>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search reports by ID, reporter or subject..." />
            </div>
            <div class="spacer"></div>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="select-input" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                <asp:ListItem Value="All">All status</asp:ListItem>
                <asp:ListItem>Open</asp:ListItem>
                <asp:ListItem>Investigating</asp:ListItem>
                <asp:ListItem>Resolved</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button-primary" OnClick="Filter_Changed" />
        </asp:Panel>

        <div class="table-scroll">
            <table class="data-table">
                <thead>
                    <tr><th>Report</th><th>Date</th><th>Reporter</th><th>Subject</th><th>Severity</th><th>Status</th><th>Actions</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptIncidents" runat="server" OnItemCommand="rptIncidents_ItemCommand" OnItemDataBound="rptIncidents_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td><span class="act-link"><%#: Eval("Id") %></span></td>
                                <td class="muted-sm"><%#: Eval("DateText") %></td>
                                <td><strong><%#: Eval("Reporter") %></strong></td>
                                <td><%#: Eval("Subject") %><br /><span class="muted-sm"><%#: Eval("Details") %></span></td>
                                <td><span class="pill <%# Eval("SeverityCss") %>"><%#: Eval("Severity") %></span></td>
                                <td><span class="pill <%# Eval("StatusCss") %>"><span class="dot"></span><%#: Eval("Status") %></span></td>
                                <td>
                                    <div class="row-actions">
                                        <asp:LinkButton ID="btnInvestigate" runat="server" CssClass="act-link" CommandName="Investigating" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Investigate</asp:LinkButton>
                                        <asp:LinkButton ID="btnResolve" runat="server" CssClass="act-link ok" CommandName="Resolved" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Resolve</asp:LinkButton>
                                        <asp:LinkButton ID="btnReopen" runat="server" CssClass="act-link" CommandName="Open" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Reopen</asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty">No reports match your filters.</asp:Panel>
        </div>
    </section>

    <section class="panel">
        <div class="panel-header"><h2>Audit Trail</h2><span class="muted-sm">Latest 12 events</span></div>
        <div class="table-scroll">
            <table class="data-table">
                <thead><tr><th>Time (UTC)</th><th>Actor</th><th>Action</th><th>Target</th><th>IP</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptAudit" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="muted-sm"><%#: Eval("TimeText") %></td>
                                <td><strong><%#: Eval("Actor") %></strong></td>
                                <td><%#: Eval("Action") %></td>
                                <td><%#: Eval("Target") %></td>
                                <td class="muted-sm"><%#: Eval("Ip") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </section>

</asp:Content>
