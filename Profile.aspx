
<%@ Page Title="My Profile" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="CyberArenaWebForms5.Profile" %>

<asp:Content ID="ProfileTitle" ContentPlaceHolderID="TitleContent" runat="server">
    My Profile | CyberArena
</asp:Content>

<asp:Content ID="ProfileHeader" ContentPlaceHolderID="HeaderContent" runat="server">
    <div class="page-title">
        <h1>My Profile</h1>
        <span class="pill blue">Participant Account</span>
    </div>
    <p class="page-sub">Manage your participant information and track your achievements.</p>
</asp:Content>

<asp:Content ID="ProfileContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="profile-layout">

        <section class="panel profile-card">
            <div class="profile-cover"></div>

            <div class="profile-identity">
                <div class="profile-avatar">U</div>
                <h2><asp:Label ID="lblDisplayName" runat="server"
                    Text="Cyber Participant" /></h2>
                <p class="profile-handle">
                    @<asp:Label ID="lblUsername" runat="server"
                        Text="participant01" />
                </p>
                <span class="pill blue">CTF Participant</span>
            </div>

            <div class="profile-divider"></div>

            <div class="profile-stat-row">
                <div>
                    <strong>450</strong>
                    <span>Total Points</span>
                </div>
                <div>
                    <strong>12</strong>
                    <span>Challenges Solved</span>
                </div>
                <div>
                    <strong>#24</strong>
                    <span>Current Rank</span>
                </div>
            </div>
        </section>

        <section class="panel">
            <div class="panel-header">
                <h2>Personal Information</h2>
            </div>

            <div class="panel-body">
                <div class="profile-form-grid">

                    <div class="field-group">
                        <label for="txtFullName">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server"
                            CssClass="team-input"
                            MaxLength="80"
                            placeholder="Enter your full name" />
                    </div>

                    <div class="field-group">
                        <label for="txtUsername">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="team-input"
                            MaxLength="30"
                            placeholder="Choose a username" />
                    </div>

                    <div class="field-group">
                        <label for="txtEmail">Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server"
                            CssClass="team-input"
                            TextMode="Email"
                            MaxLength="254"
                            placeholder="you@example.com" />
                    </div>

                    <div class="field-group">
                        <label for="txtInstitution">Institution</label>
                        <asp:TextBox ID="txtInstitution" runat="server"
                            CssClass="team-input"
                            MaxLength="100"
                            placeholder="College or university" />
                    </div>

                    <div class="field-group profile-full-width">
                        <label for="txtBio">About Me</label>
                        <asp:TextBox ID="txtBio" runat="server"
                            CssClass="team-input"
                            TextMode="MultiLine"
                            Rows="4"
                            MaxLength="500"
                            placeholder="Tell the community about your interests..." />
                    </div>

                </div>

                <asp:Label ID="lblProfileMessage" runat="server"
                    EnableViewState="false"
                    CssClass="profile-message" />

                <div class="profile-actions">
                    <asp:Button ID="btnSaveProfile" runat="server"
                        Text="Save Changes"
                        CssClass="button-primary"
                        OnClick="btnSaveProfile_Click" />
                </div>
            </div>
        </section>

    </div>

</asp:Content>
