<%@ Page Title="My Submissions" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Submissions.aspx.cs"
    Inherits="CyberArenaWebForms5.Submissions" %>

<asp:Content ID="SubTitle" ContentPlaceHolderID="TitleContent" runat="server">
    My Submissions | CyberArena
</asp:Content>

<asp:Content ID="SubHeader" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title"><h1>My Submissions</h1></div>
    <p class="page-sub">Track your challenge attempts and results.</p>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon blue"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 6h12M8 12h12M8 18h12"/><circle cx="4" cy="6" r=".8"/><circle cx="4" cy="12" r=".8"/><circle cx="4" cy="18" r=".8"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Total Submissions</div>
                <div class="stat-value">6</div>
                <div class="stat-note">All attempts</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="m5 12 5 5 9-10"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Accepted</div>
                <div class="stat-value">3</div>
                <div class="stat-note">Correct flags</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon red"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M6 6l12 12M18 6 6 18"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Incorrect</div>
                <div class="stat-value">3</div>
                <div class="stat-note">Wrong flags</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon yellow"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M8 21h8M12 17v4"/><path d="M7 4h10v5a5 5 0 0 1-10 0V4Z"/></svg></div>
            <div class="stat-body">
                <div class="stat-label">Points Earned</div>
                <div class="stat-value">250</div>
                <div class="stat-note">From accepted flags</div>
            </div>
        </div>
    </div>

    <div class="panel">
        <div class="panel-header">
            <h2>Submission History</h2>

            <asp:DropDownList
                ID="ddlStatus"
                runat="server"
                AutoPostBack="true"
                CssClass="select-input"
                OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">

                <asp:ListItem Text="All Submissions" Value="All" />
                <asp:ListItem Text="Accepted" Value="Accepted" />
                <asp:ListItem Text="Incorrect" Value="Incorrect" />

            </asp:DropDownList>
        </div>

        <div class="panel-body" style="padding:12px 0 4px;">

            <asp:Label
                ID="lblSubmissionMessage"
                runat="server"
                CssClass="profile-message"
                Style="padding:0 20px;" />

            <asp:GridView
                ID="gvSubmissions"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="data-table"
                GridLines="None"
                EmptyDataText="No submissions found.">

                <Columns>
                    <asp:TemplateField HeaderText="Challenge">
                        <ItemTemplate><strong><%#: Eval("Challenge") %></strong></ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="Category"
                        HeaderText="Category" />

                    <asp:BoundField
                        DataField="SubmittedAt"
                        HeaderText="Submitted At" />

                    <asp:TemplateField HeaderText="Result">
                        <ItemTemplate>
                            <span class='pill <%# (string)Eval("Status") == "Accepted" ? "green" : "red" %>'><span class="dot"></span><%#: Eval("Status") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Points">
                        <ItemTemplate><strong><%# Eval("Points") %></strong></ItemTemplate>
                    </asp:TemplateField>
                </Columns>

            </asp:GridView>

        </div>
    </div>

</asp:Content>
