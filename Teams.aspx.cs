
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Teams : CyberArenaWebForms5.Models.UserPage
    {
        private class TeamInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Initials { get; set; }
            public string Description { get; set; }
            public int MemberCount { get; set; }
            public int Points { get; set; }
            public string Status { get; set; }

            public int Progress
            {
                get { return Math.Min(100, MemberCount * 20); }
            }
        }

        private List<TeamInfo> GetTeamsFromDatabase()
        {
            try
            {
                // Get all teams from database
                List<Team> dbTeams = TeamStore.All();

                var teamsList = new List<TeamInfo>();
                foreach (var team in dbTeams)
                {
                    teamsList.Add(new TeamInfo
                    {
                        Id = team.Id,
                        Name = team.Name,
                        Initials = GetInitials(team.Name),
                        Description = team.Description,
                        MemberCount = team.CurrentMembers,
                        Points = team.TotalPoints,
                        Status = team.Status
                    });
                }

                return teamsList;
            }
            catch (Exception ex)
            {
                lblPageMessage.Text = "Error loading teams: " + ex.Message;
                return new List<TeamInfo>();
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            string[] words = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(words.Select(w => w[0])).ToUpper();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTeams();
            }
        }

        private void BindTeams()
        {
            List<TeamInfo> teams = GetTeamsFromDatabase();

            if (ddlTeamFilter.SelectedValue == "open")
            {
                teams = teams.Where(t => t.Status == "Open").ToList();
            }
            else if (ddlTeamFilter.SelectedValue == "full")
            {
                teams = teams.Where(t => t.Status == "Full").ToList();
            }

            rptTeams.DataSource = teams;
            rptTeams.DataBind();
        }

        protected void ddlTeamFilter_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            BindTeams();
        }

        protected void btnShowCreate_Click(object sender, EventArgs e)
        {
            pnlCreateTeam.Visible = true;
            lblCreateMessage.Text = "";
            txtTeamName.Text = "";
            txtTeamDescription.Text = "";
        }

        protected void btnCancelCreate_Click(object sender, EventArgs e)
        {
            pnlCreateTeam.Visible = false;
            lblCreateMessage.Text = "";
        }

        protected void btnCreateTeam_Click(object sender, EventArgs e)
        {
            string name = txtTeamName.Text.Trim();
            string description = txtTeamDescription.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(name))
            {
                lblCreateMessage.Text = "Please enter a team name.";
                return;
            }

            if (name.Length < 3)
            {
                lblCreateMessage.Text = "Team name must contain at least 3 characters.";
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                lblCreateMessage.Text = "Please enter a team description.";
                return;
            }

            try
            {
                // Check if team name already exists
                Team existing = TeamStore.FindByName(name);
                if (existing != null)
                {
                    lblCreateMessage.Text = "Team name already taken. Please choose another name.";
                    return;
                }

                // Create team in database
                Team newTeam = TeamStore.Create(name, description, 5);

                if (newTeam != null)
                {
                    // Add current user as team leader
                    if (CurrentUser != null)
                    {
                        TeamStore.AddMember(newTeam.Id, CurrentUser.Id);

                        // Log the action
                        AuditLog.Write(
                            CurrentUser.Username,
                            "Created team",
                            newTeam.Name,
                            Context);
                    }

                    lblCreateMessage.Text = "✅ Team created successfully! You are now the team leader.";
                    pnlCreateTeam.Visible = false;

                    // Refresh teams list
                    BindTeams();
                }
                else
                {
                    lblCreateMessage.Text = "Failed to create team. Please try again.";
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation
                {
                    lblCreateMessage.Text = "Team name already exists. Please choose a different name.";
                }
                else
                {
                    lblCreateMessage.Text = "Database error: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                lblCreateMessage.Text = "Error creating team: " + ex.Message;
            }
        }

        protected void rptTeams_ItemCommand(
            object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "JoinTeam")
                return;

            int teamId;

            if (!int.TryParse(
                Convert.ToString(e.CommandArgument), out teamId))
            {
                lblPageMessage.Text = "Invalid team selection.";
                return;
            }

            try
            {
                // Get team from database
                Team team = TeamStore.FindById(teamId);

                if (team == null)
                {
                    lblPageMessage.Text = "Team not found.";
                    return;
                }

                if (team.Status == "Full")
                {
                    lblPageMessage.Text = "❌ This team is already full.";
                    return;
                }

                if (CurrentUser == null)
                {
                    lblPageMessage.Text = "You must be logged in to join a team.";
                    return;
                }

                // Check if already in a team
                if (CurrentUser.TeamId.HasValue && CurrentUser.TeamId.Value > 0)
                {
                    lblPageMessage.Text = "❌ You are already in a team. Leave your current team first.";
                    return;
                }

                // Add user to team
                bool success = TeamStore.AddMember(teamId, CurrentUser.Id);

                if (success)
                {
                    lblPageMessage.Text = "✅ You successfully joined " + Server.HtmlEncode(team.Name) + "!";

                    // Log the action
                    AuditLog.Write(
                        CurrentUser.Username,
                        "Joined team",
                        team.Name,
                        Context);

                    // Refresh teams list
                    BindTeams();
                }
                else
                {
                    lblPageMessage.Text = "❌ Could not join team. You may already be a member or the team is full.";
                }
            }
            catch (Exception ex)
            {
                lblPageMessage.Text = "Error joining team: " + ex.Message;
            }
        }
    }
}
