<%@ Page Title="Create Challenge" Language="C#"
    MasterPageFile="~/Admin.Master"
    AutoEventWireup="true"
    CodeBehind="CreateChallenge.aspx.cs"
    Inherits="CyberArenaWebForms5.CreateChallenge" %>

<asp:Content ID="TitleBlock" ContentPlaceHolderID="TitleContent" runat="server">
    Create Challenge | CyberArena Admin
</asp:Content>

<asp:Content ID="HeaderBlock" ContentPlaceHolderID="HeaderContent" runat="server">
    <a href="AdminChallenges.aspx" class="back-link">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 6-6 6 6 6"/></svg>
        Back to repository
    </a>
    <div class="page-title"><h1><asp:Label ID="lblHeading" runat="server" Text="Create New Challenge" /></h1></div>
    <p class="page-sub">Fill in the details below. You can save it as a draft or publish it straight away.</p>
</asp:Content>

<asp:Content ID="MainBlock" ContentPlaceHolderID="MainContent" runat="server">

    <div class="create-grid">

        <section class="panel form-card">
            <div class="panel-body">

                <asp:Literal ID="litStepper" runat="server" />

                <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false" />

                <asp:MultiView ID="mvSteps" runat="server" ActiveViewIndex="0">

                    <asp:View ID="vStep1" runat="server">
                        <div class="field-group">
                            <label>Challenge title</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="team-input" MaxLength="80" placeholder="e.g. Reverse Me" />
                        </div>
                        <div class="field-group" style="margin-top:14px;">
                            <label>Short summary</label>
                            <asp:TextBox ID="txtSubtitle" runat="server" CssClass="team-input" MaxLength="80" placeholder="One line shown in the challenge list" />
                        </div>
                        <div class="auth-two" style="margin-top:14px;">
                            <div class="field-group">
                                <label>Category</label>
                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="select-input" Width="100%" />
                            </div>
                            <div class="field-group">
                                <label>Difficulty</label>
                                <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="select-input" Width="100%">
                                    <asp:ListItem>Easy</asp:ListItem>
                                    <asp:ListItem Selected="True">Medium</asp:ListItem>
                                    <asp:ListItem>Hard</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="auth-two" style="margin-top:14px;">
                            <div class="field-group">
                                <label>Points (10 to 1000)</label>
                                <asp:TextBox ID="txtPoints" runat="server" CssClass="team-input" MaxLength="4" Text="100" />
                            </div>
                        </div>
                    </asp:View>

                    <asp:View ID="vStep2" runat="server">
                        <div class="field-group">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="team-input" TextMode="MultiLine" Rows="6" MaxLength="2000"
                                placeholder="Explain the scenario and what the player has to do..." />
                        </div>
                        <div class="field-group" style="margin-top:14px;">
                            <label>Hint (optional)</label>
                            <asp:TextBox ID="txtHint" runat="server" CssClass="team-input" MaxLength="300" placeholder="Costs the player 10 points to reveal" />
                        </div>
                        <div class="field-group" style="margin-top:14px;">
                            <label>Attachment (optional  zip, txt, pdf, png, jpg, pcap, bin, py, md  max 4 MB)</label>
                            <asp:FileUpload ID="fuAttachment" runat="server" CssClass="team-input file-input" />
                            <div class="muted-sm" style="margin-top:6px;">Current file: <asp:Label ID="lblAttachment" runat="server" Text="none" /></div>
                        </div>
                        <div class="field-group" style="margin-top:14px;">
                            <label>Flag</label>
                            <asp:TextBox ID="txtFlag" runat="server" CssClass="team-input" MaxLength="120" placeholder="CTF{...}" autocomplete="off" />
                        </div>
                    </asp:View>

                    <asp:View ID="vStep3" runat="server">
                        <asp:Literal ID="litReview" runat="server" />
                        <label class="check-row">
                            <asp:CheckBox ID="chkPublish" runat="server" Checked="true" />
                            <span>Publish immediately (visible to all players)</span>
                        </label>
                    </asp:View>

                </asp:MultiView>

                <div class="form-actions">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button-secondary" CausesValidation="false" OnClick="btnCancel_Click" />
                    <span class="spacer"></span>
                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button-secondary" CausesValidation="false" OnClick="btnBack_Click" Visible="false" />
                    <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="button-primary" OnClick="btnNext_Click" />
                    <asp:Button ID="btnSave" runat="server" Text="Create Challenge" CssClass="button-primary" OnClick="btnSave_Click" Visible="false" />
                </div>
            </div>
        </section>

        <div class="dash-col">
            <section class="panel">
                <div class="panel-header"><h2>Live Preview</h2></div>
                <div class="panel-body">
                    <asp:Literal ID="litPreview" runat="server" />
                </div>
            </section>

            <section class="panel">
                <div class="panel-header"><h2>Guidelines</h2></div>
                <div class="panel-body guide-list">
                    <div><strong>Clear objective</strong><span>Tell players exactly what they need to find.</span></div>
                    <div><strong>Unique flags</strong><span>Use the CTF{...} format and never reuse a flag.</span></div>
                    <div><strong>Test first</strong><span>Solve it yourself before publishing.</span></div>
                </div>
            </section>
        </div>

    </div>

</asp:Content>
