using System;
using System.Data;
using System.Data.SqlClient;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class DatabaseDiagnostics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllData();
            }
        }

        private void LoadAllData()
        {
            try
            {
                // Load table summary
                LoadTableSummary();

                // Load all table data
                LoadAppUsers();
                LoadChallenges();
                LoadTeams();
                LoadTeamMembers();

                lblTestResults.Text = "✅ All data loaded successfully!";
                lblTestResults.CssClass = "status success";
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }

        private void LoadTableSummary()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();

                string query = @"
                    SELECT 'Team' as TableName, COUNT(*) as RecordCount FROM Team
                    UNION ALL
                    SELECT 'AppUser', COUNT(*) FROM AppUser
                    UNION ALL
                    SELECT 'Challenge', COUNT(*) FROM Challenge
                    UNION ALL
                    SELECT 'Submission', COUNT(*) FROM Submission
                    UNION ALL
                    SELECT 'TeamMember', COUNT(*) FROM TeamMember
                    UNION ALL
                    SELECT 'AuditLog', COUNT(*) FROM AuditLog
                    ORDER BY TableName";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
                gvTableSummary.DataSource = dt;
                gvTableSummary.DataBind();
            }
        }

        private void LoadAppUsers()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();

                string query = "SELECT Id, Username, FullName, Email, Role, Status, Points, Solved, Rank FROM AppUser ORDER BY Rank, Points DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
                gvAppUsers.DataSource = dt;
                gvAppUsers.DataBind();
            }
        }

        private void LoadChallenges()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();

                string query = "SELECT Id, Title, Category, Difficulty, Points, Solves, IsPublished FROM Challenge ORDER BY Difficulty, Id";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
                gvChallenges.DataSource = dt;
                gvChallenges.DataBind();
            }
        }

        private void LoadTeams()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();

                string query = "SELECT Id, Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status FROM Team ORDER BY Id";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
                gvTeams.DataSource = dt;
                gvTeams.DataBind();
            }
        }

        private void LoadTeamMembers()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                DataTable dt = new DataTable();

                string query = @"
                    SELECT 
                        tm.Id, 
                        t.Name as TeamName, 
                        au.Username, 
                        tm.JoinedAt 
                    FROM TeamMember tm
                    JOIN Team t ON tm.TeamId = t.Id
                    JOIN AppUser au ON tm.UserId = au.Id
                    ORDER BY tm.TeamId, au.Username";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
                gvTeamMembers.DataSource = dt;
                gvTeamMembers.DataBind();
            }
        }

        protected void TestCreate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Create a test user (CREATE operation)
                    string testUsername = "TestUser_" + DateTime.Now.Ticks;
                    string salt = Guid.NewGuid().ToString();
                    string hash = Guid.NewGuid().ToString();

                    string insertQuery = @"
                        INSERT INTO AppUser 
                        (Username, FullName, Email, Role, Status, Points, Solved, Salt, Hash) 
                        VALUES 
                        (@Username, @FullName, @Email, @Role, @Status, @Points, @Solved, @Salt, @Hash)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Username", testUsername);
                    cmd.Parameters.AddWithValue("@FullName", "Test User");
                    cmd.Parameters.AddWithValue("@Email", testUsername + "@test.local");
                    cmd.Parameters.AddWithValue("@Role", "Player");
                    cmd.Parameters.AddWithValue("@Status", "Active");
                    cmd.Parameters.AddWithValue("@Points", 0);
                    cmd.Parameters.AddWithValue("@Solved", 0);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@Hash", hash);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        lblTestResults.Text = $"✅ CREATE Test Passed! New user '{testUsername}' created successfully.";
                        lblTestResults.CssClass = "status success";
                        LoadAppUsers();
                    }
                    else
                    {
                        lblTestResults.Text = "❌ CREATE Test Failed!";
                        lblTestResults.CssClass = "status error";
                    }
                }
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ CREATE Test Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }

        protected void TestRead_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAllData();
                lblTestResults.Text = "✅ READ Test Passed! All data refreshed successfully.";
                lblTestResults.CssClass = "status success";
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ READ Test Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }

        protected void TestUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Update a test user's points (UPDATE operation)
                    string updateQuery = @"
                        UPDATE AppUser 
                        SET Points = Points + 100, Solved = Solved + 1 
                        WHERE Username LIKE 'TestUser_%'
                        AND Points < 1000";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        lblTestResults.Text = $"✅ UPDATE Test Passed! Updated {result} test user record(s).";
                        lblTestResults.CssClass = "status success";
                        LoadAppUsers();
                    }
                    else
                    {
                        lblTestResults.Text = "⚠️ UPDATE Test: No test users found to update.";
                        lblTestResults.CssClass = "status info";
                    }
                }
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ UPDATE Test Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }

        protected void TestDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Delete test users (DELETE operation)
                    string deleteQuery = "DELETE FROM AppUser WHERE Username LIKE 'TestUser_%'";

                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        lblTestResults.Text = $"✅ DELETE Test Passed! Deleted {result} test user record(s).";
                        lblTestResults.CssClass = "status success";
                        LoadAppUsers();
                    }
                    else
                    {
                        lblTestResults.Text = "⚠️ DELETE Test: No test users found to delete.";
                        lblTestResults.CssClass = "status info";
                    }
                }
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ DELETE Test Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }

        protected void ResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Delete test users
                    string deleteQuery = "DELETE FROM AppUser WHERE Username LIKE 'TestUser_%'";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                    int deletedCount = deleteCmd.ExecuteNonQuery();

                    lblTestResults.Text = $"✅ Reset Complete! Removed {deletedCount} test records. Data reset to original seed data.";
                    lblTestResults.CssClass = "status success";
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                lblTestResults.Text = $"❌ Reset Error: {ex.Message}";
                lblTestResults.CssClass = "status error";
            }
        }
    }
}
