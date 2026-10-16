using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Dashboard : CyberArenaWebForms5.Models.UserPage
    {
        // ----- sample view models -----
        public class CategoryStat
        {
            public string Name { get; set; }
            public int Solved { get; set; }
            public int Total { get; set; }
            public string Color { get; set; }
        }

        public class MemberInfo
        {
            public string Name { get; set; }
            public string Initials { get; set; }
            public string Role { get; set; }
            public int Points { get; set; }
            public string Color { get; set; }
        }

        public class ActivityInfo
        {
            public string Title { get; set; }
            public string When { get; set; }
            public string Result { get; set; }
            public bool Success { get; set; }
            public string Css { get { return Success ? "green" : "red"; } }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Dashboard data will be loaded from SQL Server later.
            // These values are currently sample UI data.
            int totalPoints = 450;
            int solved = 12;
            int total = 40;

            lblUserName.Text = Server.HtmlEncode(CurrentUser.FullName);
            lblPoints.Text = totalPoints.ToString("N0");
            lblSolved.Text = solved.ToString();
            lblTotalChallenges.Text = total.ToString();
            lblRank.Text = "#24";
            lblTeamName.Text = "RootAccess";
            lblTeamHeading.Text = "RootAccess";

            var categories = new List<CategoryStat>
            {
                new CategoryStat { Name = "Web Security",         Solved = 3, Total = 9, Color = "#3b82f6" },
                new CategoryStat { Name = "Cryptography",         Solved = 2, Total = 7, Color = "#10b981" },
                new CategoryStat { Name = "Reverse Engineering",  Solved = 1, Total = 6, Color = "#f59e0b" },
                new CategoryStat { Name = "Digital Forensics",    Solved = 3, Total = 8, Color = "#8b5cf6" },
                new CategoryStat { Name = "OSINT",                Solved = 1, Total = 4, Color = "#ec4899" },
                new CategoryStat { Name = "Steganography",        Solved = 2, Total = 6, Color = "#06b6d4" }
            };
            rptCategories.DataSource = categories;
            rptCategories.DataBind();
            lblDonutTotal.Text = categories.Sum(c => c.Solved).ToString();
            litDonut.Text = BuildDonutSvg(categories);

            rptMembers.DataSource = new List<MemberInfo>
            {
                new MemberInfo { Name = "NullPointer", Initials = "NP", Role = "Team Captain", Points = 1850, Color = "bg-navy" },
                new MemberInfo { Name = "PacketGhost", Initials = "PG", Role = "Network analyst", Points = 1450, Color = "bg-green" },
                new MemberInfo { Name = "HexHunter",   Initials = "HH", Role = "Reverse engineer", Points = 750, Color = "bg-orange" },
                new MemberInfo { Name = "Participant", Initials = "P",  Role = "You", Points = 450, Color = "bg-purple" }
            };
            rptMembers.DataBind();

            rptActivity.DataSource = new List<ActivityInfo>
            {
                new ActivityInfo { Title = "Solved Hidden Message",  When = "Today, 10:30 AM", Result = "+50",    Success = true },
                new ActivityInfo { Title = "Solved Caesar's Secret", When = "Yesterday",       Result = "+75",    Success = true },
                new ActivityInfo { Title = "Attempted Broken Login", When = "Yesterday",       Result = "Failed", Success = false }
            };
            rptActivity.DataBind();

            litProgressChart.Text = ChartBuilder.AreaChart(
                new[] { "Oct 4", "Oct 5", "Oct 6", "Oct 7", "Oct 8", "Oct 9", "Oct 10", "Oct 11" },
                new[] { 0, 60, 140, 180, 260, 300, 380, totalPoints }, 500, "pts", "areaFill");
        }

        private static string F(double v)
        {
            return v.ToString("0.##", CultureInfo.InvariantCulture);
        }

        /// <summary>Server-rendered donut chart using stroke-dasharray segments.</summary>
        private static string BuildDonutSvg(List<CategoryStat> categories)
        {
            const double r = 74, stroke = 18, gap = 4;
            double circ = 2 * Math.PI * r;
            double sum = categories.Sum(c => c.Solved);
            double offset = 0;

            var sb = new StringBuilder();
            sb.Append("<svg viewBox=\"0 0 190 190\" xmlns=\"http://www.w3.org/2000/svg\">");
            sb.Append("<circle cx=\"95\" cy=\"95\" r=\"" + F(r) + "\" fill=\"none\" stroke=\"#f1f5f9\" stroke-width=\"" + F(stroke) + "\"/>");
            foreach (var c in categories)
            {
                double len = circ * c.Solved / sum;
                double dash = Math.Max(len - gap, 1);
                sb.Append("<circle cx=\"95\" cy=\"95\" r=\"" + F(r) + "\" fill=\"none\" stroke=\"" + c.Color + "\" stroke-width=\"" + F(stroke) +
                          "\" stroke-dasharray=\"" + F(dash) + " " + F(circ - dash) + "\" stroke-dashoffset=\"" + F(-offset) + "\"/>");
                offset += len;
            }
            sb.Append("</svg>");
            return sb.ToString();
        }
    }
}
