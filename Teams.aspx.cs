
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        private List<TeamInfo> GetSampleTeams()
        {
            return new List<TeamInfo>
            {
                new TeamInfo {
                    Id = 1, Name = "RootAccess", Initials = "RA",
                    Description = "Security researchers focused on web exploitation and forensics.",
                    MemberCount = 4, Points = 3300, Status = "Open"
                },
                new TeamInfo {
                    Id = 2, Name = "ZeroDay", Initials = "ZD",
                    Description = "A collaborative team solving challenging security puzzles.",
                    MemberCount = 3, Points = 2725, Status = "Open"
                },
                new TeamInfo {
                    Id = 3, Name = "BinaryOps", Initials = "BO",
                    Description = "Reverse engineering, binary analysis and low-level security.",
                    MemberCount = 5, Points = 2175, Status = "Full"
                },
                new TeamInfo {
                    Id = 4, Name = "NullCrew", Initials = "NC",
                    Description = "Learning cybersecurity through hands-on CTF challenges.",
                    MemberCount = 2, Points = 1420, Status = "Open"
                },
                new TeamInfo {
                    Id = 5, Name = "PacketStorm", Initials = "PS",
                    Description = "Network analysis, traffic investigation and protocol research.",
                    MemberCount = 5, Points = 1200, Status = "Full"
                },
                new TeamInfo {
                    Id = 6, Name = "CipherLab", Initials = "CL",
                    Description = "Cryptography enthusiasts working through complex ciphers.",
                    MemberCount = 1, Points = 650, Status = "Open"
                }
            };
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
            IEnumerable<TeamInfo> teams = GetSampleTeams();

            if (ddlTeamFilter.SelectedValue == "open")
            {
                teams = teams.Where(t => t.Status == "Open");
            }
            else if (ddlTeamFilter.SelectedValue == "full")
            {
                teams = teams.Where(t => t.Status == "Full");
            }

            rptTeams.DataSource = teams.ToList();
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

            if (string.IsNullOrWhiteSpace(name))
            {
                lblCreateMessage.Text = "Please enter a team name.";
                return;
            }

            if (name.Length < 3)
            {
                lblCreateMessage.Text =
                    "Team name must contain at least 3 characters.";
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                lblCreateMessage.Text =
                    "Please enter a team description.";
                return;
            }

            // Temporary demonstration only.
            // Save the team to SQL Server in a later step.
            lblCreateMessage.Text =
                "Validation successful. Database saving will be implemented later.";
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

            TeamInfo team = GetSampleTeams()
                .FirstOrDefault(t => t.Id == teamId);

            if (team == null)
            {
                lblPageMessage.Text = "Team not found.";
                return;
            }

            if (team.Status == "Full")
            {
                lblPageMessage.Text = "This team is already full.";
                return;
            }

            // Do not add membership yet: this is sample data.
            lblPageMessage.Text =
                "You selected " + Server.HtmlEncode(team.Name) +
                ". Actual membership will be connected to the database later.";
        }
    }
}
