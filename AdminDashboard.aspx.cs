using System;
using System.Linq;
using System.Web;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminDashboard : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var users = UserStore.All();
            var challenges = Challenge.AllAdmin();
            var submissions = SubmissionStore.All();
            var incidents = IncidentStore.All();

            lblUsers.Text = users.Count.ToString("N0");
            lblActiveUsers.Text = users.Count(u => u.Status == "Active").ToString();
            lblChallenges.Text = challenges.Count(c => c.IsPublished).ToString();
            lblDrafts.Text = challenges.Count(c => !c.IsPublished).ToString();
            lblSubmissions.Text = submissions.Count.ToString();
            lblRejected.Text = submissions.Count(s => s.Status == "Rejected").ToString();
            lblIncidents.Text = incidents.Count(i => i.Status != "Resolved").ToString();
            lblCritical.Text = incidents.Count(i => i.Severity == "Critical" && i.Status != "Resolved").ToString();

            litActivityChart.Text = ChartBuilder.AreaChart(
                new[] { "Oct 4", "Oct 5", "Oct 6", "Oct 7", "Oct 8", "Oct 9", "Oct 10", "Oct 11" },
                new[] { 120, 190, 260, 310, 310, 330, 470, 430 }, 500, "subs", "adminFill");

            rptRecent.DataSource = submissions.Take(4).ToList();
            rptRecent.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            AuditLog.Write(CurrentUser.Username, "Exported audit log", "CSV download", Context);

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Disposition", "attachment; filename=\"audit-log.csv\"");
            Response.Write(AuditLog.ToCsv());
            Response.End();
        }
    }
}
