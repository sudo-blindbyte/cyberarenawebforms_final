using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class DatabaseTest : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnTestConnection_Click(null, null);
            }
        }

        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                // Get connection string from config
                string connStr = ConfigurationManager.ConnectionStrings["CyberArenaDb"]?.ConnectionString;

                if (string.IsNullOrEmpty(connStr))
                {
                    ShowConnectionError("Connection string 'CyberArenaDb' not found in Web.config");
                    return;
                }

                // Try to open connection
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Connection successful
                    lblConnectionStatus.Text = "✅ Connection Successful";
                    lblConnectionStatus.Style["color"] = "green";
                    lblConnectionStatus.Style["font-weight"] = "bold";

                    lblConnectionDetails.Text = $"Successfully connected to database at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    lblLastConnection.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Get database info
                    GetDatabaseInfo(conn);

                    // Load table information
                    LoadTableInfo(conn);

                    conn.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                ShowConnectionError($"SQL Server Error: {sqlEx.Message} (Error Code: {sqlEx.Number})");
            }
            catch (Exception ex)
            {
                ShowConnectionError($"Connection Error: {ex.Message}");
            }
        }

        protected void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            btnTestConnection_Click(null, null);
        }

        private void ShowConnectionError(string errorMessage)
        {
            lblConnectionStatus.Text = "❌ Connection Failed";
            lblConnectionStatus.Style["color"] = "red";
            lblConnectionStatus.Style["font-weight"] = "bold";

            lblConnectionDetails.Text = errorMessage;
            lblConnectionDetails.Style["color"] = "red";

            lblDatabaseSize.Text = "N/A - Connection Failed";
            lblTableCount.Text = "N/A - Connection Failed";
        }

        private void GetDatabaseInfo(SqlConnection conn)
        {
            try
            {
                // Get database size
                string sizeQuery = @"
                    SELECT 
                        CAST(SUM(size) * 8 / 1024.0 AS DECIMAL(10,2)) as SizeMB
                    FROM sys.database_files";

                using (SqlCommand cmd = new SqlCommand(sizeQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lblDatabaseSize.Text = $"{result} MB";
                    }
                    else
                    {
                        lblDatabaseSize.Text = "< 1 MB";
                    }
                }

                // Get table count
                string tableCountQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE'";

                using (SqlCommand cmd = new SqlCommand(tableCountQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    lblTableCount.Text = result != null ? result.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                lblDatabaseSize.Text = "Error retrieving info";
                lblTableCount.Text = "Error retrieving info";
            }
        }

        private void LoadTableInfo(SqlConnection conn)
        {
            var tables = new[]
            {
                new { Name = "AppUser", Label = lblAppUserRows, CheckLabel = lblAppUserCheck },
                new { Name = "Team", Label = lblTeamRows, CheckLabel = lblTeamCheck },
                new { Name = "Challenge", Label = lblChallengeRows, CheckLabel = lblChallengeCheck },
                new { Name = "Submission", Label = lblSubmissionRows, CheckLabel = lblSubmissionCheck },
                new { Name = "TeamMember", Label = lblTeamMemberRows, CheckLabel = lblTeamMemberCheck },
                new { Name = "AuditLog", Label = lblAuditLogRows, CheckLabel = lblAuditLogCheck }
            };

            foreach (var table in tables)
            {
                try
                {
                    // Check if table exists
                    string existsQuery = $@"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = '{table.Name}' AND TABLE_TYPE = 'BASE TABLE'";

                    using (SqlCommand cmd = new SqlCommand(existsQuery, conn))
                    {
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists == 0)
                        {
                            table.Label.Text = "0";
                            table.CheckLabel.Text = "<span style='color: red;'>✗ Missing</span>";
                            table.CheckLabel.EnableViewState = false;
                            continue;
                        }
                    }

                    // Get row count
                    string rowCountQuery = $"SELECT COUNT(*) FROM dbo.{table.Name}";
                    using (SqlCommand cmd = new SqlCommand(rowCountQuery, conn))
                    {
                        int rowCount = (int)cmd.ExecuteScalar();
                        table.Label.Text = rowCount.ToString();

                        if (rowCount > 0)
                        {
                            table.CheckLabel.Text = "<span style='color: green;'>✓ OK</span>";
                        }
                        else
                        {
                            table.CheckLabel.Text = "<span style='color: orange;'>⚠ Empty</span>";
                        }
                        table.CheckLabel.EnableViewState = false;
                    }
                }
                catch (Exception ex)
                {
                    table.Label.Text = "Error";
                    table.CheckLabel.Text = "<span style='color: red;'>✗ Error</span>";
                    table.CheckLabel.EnableViewState = false;
                }
            }
        }
    }
}
