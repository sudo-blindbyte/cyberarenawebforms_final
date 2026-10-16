
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace CyberArenaWebForms5
{
    public partial class Leaderboard : CyberArenaWebForms5.Models.UserPage
    {
        public class RankingEntry
        {
            public int Rank { get; set; }
            public string Username { get; set; }
            public string Team { get; set; }
            public int Solved { get; set; }
            public int Points { get; set; }
        }

        private List<RankingEntry> GetIndividualRankings()
        {
            return new List<RankingEntry>
            {
                new RankingEntry { Username = "NullPointer", Team = "RootAccess", Solved = 24, Points = 1850 },
                new RankingEntry { Username = "ByteHunter", Team = "ZeroDay", Solved = 21, Points = 1625 },
                new RankingEntry { Username = "PacketGhost", Team = "RootAccess", Solved = 19, Points = 1450 },
                new RankingEntry { Username = "CipherX", Team = "BinaryOps", Solved = 17, Points = 1275 },
                new RankingEntry { Username = "TraceRoute", Team = "ZeroDay", Solved = 15, Points = 1100 },
                new RankingEntry { Username = "CyberNova", Team = "BinaryOps", Solved = 13, Points = 900 },
                new RankingEntry { Username = "HexHunter", Team = "NullCrew", Solved = 11, Points = 750 },
                new RankingEntry { Username = "Participant", Team = "Your Team", Solved = 8, Points = 450 }
            };
        }

        private List<RankingEntry> GetTeamRankings()
        {
            return new List<RankingEntry>
            {
                new RankingEntry { Username = "RootAccess", Team = "RootAccess", Solved = 43, Points = 3300 },
                new RankingEntry { Username = "ZeroDay", Team = "ZeroDay", Solved = 36, Points = 2725 },
                new RankingEntry { Username = "BinaryOps", Team = "BinaryOps", Solved = 30, Points = 2175 },
                new RankingEntry { Username = "NullCrew", Team = "NullCrew", Solved = 22, Points = 1420 },
                new RankingEntry { Username = "Your Team", Team = "Your Team", Solved = 8, Points = 450 }
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindLeaderboard();
            }
        }

        protected void ddlRankingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindLeaderboard();
        }

        private void BindLeaderboard()
        {
            List<RankingEntry> rankings;

            if (ddlRankingType.SelectedValue == "team")
            {
                rankings = GetTeamRankings();
            }
            else
            {
                rankings = GetIndividualRankings();
            }

            rankings = rankings
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.Solved)
                .ToList();

            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].Rank = i + 1;
            }

            gvLeaderboard.DataSource = rankings;
            gvLeaderboard.DataBind();

            lblLeaderboardMessage.Text =
                "Showing " + rankings.Count +
                " sample ranking entries. Database integration will come later.";
        }
    }
}
