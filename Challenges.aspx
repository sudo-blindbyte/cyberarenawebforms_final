<%@ Page Title="Challenges" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Challenges.aspx.cs"
    Inherits="CyberArenaWebForms5.Challenges" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Challenges | CyberArena
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>All Challenges</h1></div>
    <p class="page-sub">Solve challenges, earn points and climb the leaderboard.</p>
</asp:Content>

<asp:Content ID="ExtrasBlock" ContentPlaceHolderID="HeaderExtras" runat="server">
    <div class="mini-stats">
        <span>Solved <strong class="g"><asp:Label ID="lblSolvedMini" runat="server" /></strong></span>
        <span>Points available <strong><asp:Label ID="lblPointsMini" runat="server" /></strong></span>
    </div>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlToolbar" runat="server" DefaultButton="btnSearch" CssClass="toolbar">

        <div class="search-box">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/></svg>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                placeholder="Search challenges, tags or categories..." />
        </div>

        <div class="spacer"></div>

        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="select-input"
            AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
            <asp:ListItem Value="All">All categories</asp:ListItem>
        </asp:DropDownList>

        <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="select-input"
            AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
            <asp:ListItem Value="All">All difficulty</asp:ListItem>
            <asp:ListItem>Easy</asp:ListItem>
            <asp:ListItem>Medium</asp:ListItem>
            <asp:ListItem>Hard</asp:ListItem>
        </asp:DropDownList>

        <asp:DropDownList ID="ddlSort" runat="server" CssClass="select-input"
            AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
            <asp:ListItem Value="default">Sort by</asp:ListItem>
            <asp:ListItem Value="points_desc">Points: high to low</asp:ListItem>
            <asp:ListItem Value="points_asc">Points: low to high</asp:ListItem>
            <asp:ListItem Value="solves">Most solved</asp:ListItem>
            <asp:ListItem Value="title">Title A–Z</asp:ListItem>
        </asp:DropDownList>

        <asp:Button ID="btnSearch" runat="server" Text="Search"
            CssClass="button-primary" OnClick="Filter_Changed" />
    </asp:Panel>

    <asp:Panel ID="pnlList" runat="server">

        <div class="list-head">
            <span>Challenge</span>
            <span class="hide-sm">Category</span>
            <span class="hide-sm">Difficulty</span>
            <span>Status</span>
            <span class="hide-sm">Solves</span>
            <span style="text-align:right;">Points</span>
            <span></span>
        </div>

        <asp:Repeater ID="rptChallenges" runat="server">
            <ItemTemplate>
                <a class="ch-row" href='ChallengeDetails.aspx?id=<%# Eval("Id") %>'>
                    <div class="ch-title">
                        <div class="ch-icon <%# Eval("IconColorCss") %>"><%# Eval("IconSvg") %></div>
                        <div>
                            <strong><%#: Eval("Title") %></strong>
                            <span><%#: Eval("Subtitle") %></span>
                        </div>
                    </div>
                    <div class="ch-cell hide-sm"><%#: Eval("Category") %></div>
                    <div class="hide-sm"><span class="pill <%# Eval("DifficultyCss") %>"><%#: Eval("Difficulty") %></span></div>
                    <div><span class="pill <%# Eval("StatusCss") %>"><span class="dot"></span><%#: Eval("Status") %></span></div>
                    <div class="ch-cell hide-sm"><%#: Eval("SolvesText") %></div>
                    <div class="ch-points"><%#: Eval("PointsText") %></div>
                    <div class="ch-chevron">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 6 6 6-6 6"/></svg>
                    </div>
                </a>
            </ItemTemplate>
        </asp:Repeater>

        <div class="pager-bar">
            <asp:Label ID="lblShowing" runat="server" />

            <div class="pager">
                <asp:LinkButton ID="lnkPrev" runat="server" OnClick="lnkPrev_Click"
                    CausesValidation="false">Previous</asp:LinkButton>

                <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Page"
                            CommandArgument='<%# Eval("Number") %>'
                            CssClass='<%# Eval("Css") %>'
                            CausesValidation="false"><%# Eval("Number") %></asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click"
                    CausesValidation="false">Next</asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="card empty">
        No matching challenges found. Try changing your search or filters.
    </asp:Panel>

</asp:Content>
