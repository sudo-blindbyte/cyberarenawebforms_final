using System;
using System.Web.UI;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class DataSeeding : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshTableStatus();
            }
        }

        protected void btnPopulateData_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = DatabaseHelper.PopulateSeedData();

                if (success)
                {
                    pnlInfo.Attributes["class"] = "message success";
                    pnlInfo.InnerText = "✅ Seed data populated successfully! Refresh Server Explorer to see the data.";
                    pnlInfo.Style["display"] = "block";
                }
                else
                {
                    pnlInfo.Attributes["class"] = "message error";
                    pnlInfo.InnerText = "❌ An error occurred while populating seed data. Check the Debug Output for details.";
                    pnlInfo.Style["display"] = "block";
                }

                RefreshTableStatus();
            }
            catch (Exception ex)
            {
                pnlInfo.Attributes["class"] = "message error";
                pnlInfo.InnerText = "❌ Error: " + Server.HtmlEncode(ex.Message);
                pnlInfo.Style["display"] = "block";
            }
        }

        protected void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            RefreshTableStatus();
            pnlInfo.Attributes["class"] = "message";
            pnlInfo.InnerText = "✓ Status refreshed";
            pnlInfo.Style["display"] = "block";
        }

        private void RefreshTableStatus()
        {
            try
            {
                // AppUser
                int appUserCount = GetTableRowCount("AppUser");
                lblAppUserCount.Text = appUserCount.ToString();
                lblAppUserStatus.Text = appUserCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";

                // Challenge
                int challengeCount = GetTableRowCount("Challenge");
                lblChallengeCount.Text = challengeCount.ToString();
                lblChallengeStatus.Text = challengeCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";

                // Submission
                int submissionCount = GetTableRowCount("Submission");
                lblSubmissionCount.Text = submissionCount.ToString();
                lblSubmissionStatus.Text = submissionCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";

                // AuditLog
                int auditLogCount = GetTableRowCount("AuditLog");
                lblAuditLogCount.Text = auditLogCount.ToString();
                lblAuditLogStatus.Text = auditLogCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";

                // Team
                int teamCount = GetTableRowCount("Team");
                lblTeamCount.Text = teamCount.ToString();
                lblTeamStatus.Text = teamCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";

                // TeamMember
                int teamMemberCount = GetTableRowCount("TeamMember");
                lblTeamMemberCount.Text = teamMemberCount.ToString();
                lblTeamMemberStatus.Text = teamMemberCount > 0 ? "<span style='color: green;'>✓ Has Data</span>" : "<span style='color: red;'>✗ Empty</span>";
            }
            catch (Exception ex)
            {
                pnlInfo.Attributes["class"] = "message error";
                pnlInfo.InnerText = "❌ Error refreshing status: " + Server.HtmlEncode(ex.Message);
                pnlInfo.Style["display"] = "block";
            }
        }

        private int GetTableRowCount(string tableName)
        {
            try
            {
                object result = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM dbo.{tableName}");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return -1; // Error state
            }
        }
    }
}
