using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class DatabaseCheck : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder html = new StringBuilder();

            try
            {
                // Test connection
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    html.AppendLine("✅ Database connection: SUCCESS");
                    html.AppendLine($"   Server: {conn.DataSource}");
                    html.AppendLine($"   Database: {conn.Database}");
                    html.AppendLine();

                    // Get all tables
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TABLE_NAME, 
                               (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as ColumnCount,
                               (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES t2 WHERE t2.TABLE_SCHEMA = 'dbo' AND t2.TABLE_NAME = t.TABLE_NAME) as IsRealTable
                        FROM INFORMATION_SCHEMA.TABLES t 
                        WHERE TABLE_SCHEMA = 'dbo'
                        ORDER BY TABLE_NAME", conn))
                    {
                        html.AppendLine("📊 TABLES IN DATABASE:");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tableName = reader["TABLE_NAME"].ToString();
                                html.AppendLine($"   • {tableName}");
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📋 TABLE RECORD COUNTS:");

                    // Check each table
                    string[] tables = { "AppUser", "Team", "Challenge", "Submission", "TeamMember", "AuditLog" };

                    foreach (string tableName in tables)
                    {
                        using (SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) as cnt FROM dbo.[{tableName}]", conn))
                        {
                            try
                            {
                                int count = (int)cmd.ExecuteScalar();
                                html.AppendLine($"   {tableName}: {count} records");
                            }
                            catch (Exception ex)
                            {
                                html.AppendLine($"   {tableName}: ❌ ERROR - {ex.Message}");
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📌 APPUSER DETAILED VIEW:");
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 10 
                               Id, Username, FullName, Email, Role, Status, TeamId, Points
                        FROM dbo.AppUser 
                        ORDER BY Id", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                html.AppendLine("   No users found!");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    html.AppendLine($"   ID: {reader["Id"]} | User: {reader["Username"]} | Name: {reader["FullName"]} | Role: {reader["Role"]} | Status: {reader["Status"]} | TeamId: {(reader["TeamId"] == DBNull.Value ? "NULL" : reader["TeamId"])} | Points: {reader["Points"]}");
                                }
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📌 TEAM DETAILED VIEW:");
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 10 
                               Id, Name, Description, CurrentMembers, Status, TotalPoints, CreatedAt
                        FROM dbo.Team 
                        ORDER BY Id", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                html.AppendLine("   No teams found!");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    html.AppendLine($"   ID: {reader["Id"]} | Name: {reader["Name"]} | Members: {reader["CurrentMembers"]} | Status: {reader["Status"]} | Points: {reader["TotalPoints"]}");
                                }
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📌 CHALLENGE DETAILED VIEW:");
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 10 
                               Id, Title, Category, Difficulty, Points
                        FROM dbo.Challenge 
                        ORDER BY Id", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                html.AppendLine("   No challenges found!");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    html.AppendLine($"   ID: {reader["Id"]} | Title: {reader["Title"]} | Category: {reader["Category"]} | Difficulty: {reader["Difficulty"]} | Points: {reader["Points"]}");
                                }
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📌 SUBMISSION DETAILED VIEW:");
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 10 
                               Id, UserId, ChallengeId, Status, SubmittedAt
                        FROM dbo.Submission 
                        ORDER BY Id", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                html.AppendLine("   No submissions found!");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    html.AppendLine($"   ID: {reader["Id"]} | UserId: {reader["UserId"]} | ChallengeId: {reader["ChallengeId"]} | Status: {reader["Status"]} | Submitted: {reader["SubmittedAt"]}");
                                }
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("📌 AUDITLOG DETAILED VIEW:");
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 10 
                               Id, Username, Action, Details, Timestamp
                        FROM dbo.AuditLog 
                        ORDER BY Id DESC", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                html.AppendLine("   No audit logs found!");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    html.AppendLine($"   LOG: {reader["Username"]} | Action: {reader["Action"]} | Details: {reader["Details"]} | Time: {reader["Timestamp"]}");
                                }
                            }
                        }
                    }

                    html.AppendLine();
                    html.AppendLine("✅ DATABASE CHECK COMPLETE");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"❌ ERROR: {ex.Message}");
                html.AppendLine($"Stack Trace: {ex.StackTrace}");
            }

            lblStatus.Text = html.ToString();
        }
    }
}
