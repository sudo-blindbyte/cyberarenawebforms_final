using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CyberArenaWebForms5.Models
{
    /// <summary>
    /// Data access layer for team operations. Uses SQL Server database.
    /// </summary>
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxMembers { get; set; }
        public int CurrentMembers { get; set; }
        public int TotalPoints { get; set; }
        public string Status { get; set; }  // Open, Full
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Team member association class
    /// </summary>
    public class TeamMember
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    /// <summary>
    /// Data access for team operations
    /// </summary>
    public static class TeamStore
    {
        // ---------- queries ----------
        public static List<Team> All()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status, CreatedAt " +
                "FROM dbo.Team ORDER BY TotalPoints DESC");

            var teams = new List<Team>();
            foreach (DataRow row in dt.Rows)
            {
                teams.Add(MapRowToTeam(row));
            }
            return teams;
        }

        public static Team FindById(int id)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status, CreatedAt " +
                "FROM dbo.Team WHERE Id = " + id);

            if (dt.Rows.Count == 0) return null;
            return MapRowToTeam(dt.Rows[0]);
        }

        public static Team FindByName(string name)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status, CreatedAt " +
                "FROM dbo.Team WHERE Name = @name",
                new SqlParameter("@name", name));

            if (dt.Rows.Count == 0) return null;
            return MapRowToTeam(dt.Rows[0]);
        }

        public static List<Team> GetOpenTeams()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status, CreatedAt " +
                "FROM dbo.Team WHERE Status = 'Open' ORDER BY TotalPoints DESC");

            var teams = new List<Team>();
            foreach (DataRow row in dt.Rows)
            {
                teams.Add(MapRowToTeam(row));
            }
            return teams;
        }

        public static List<AppUser> GetTeamMembers(int teamId)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT u.Id, u.Username, u.FullName, u.Email, u.Role, u.Status, u.Team, u.TeamId, " +
                "u.Points, u.Solved, u.Rank, u.Joined, u.LastActive, u.Salt, u.Hash " +
                "FROM dbo.AppUser u " +
                "INNER JOIN dbo.TeamMember tm ON u.Id = tm.UserId " +
                "WHERE tm.TeamId = @teamId ORDER BY u.Points DESC",
                new SqlParameter("@teamId", teamId));

            var users = new List<AppUser>();
            foreach (DataRow row in dt.Rows)
            {
                users.Add(UserStore.MapRowToUserPublic(row));
            }
            return users;
        }

        private static Team MapRowToTeam(DataRow row)
        {
            return new Team
            {
                Id = (int)row["Id"],
                Name = row["Name"].ToString(),
                Description = row["Description"]?.ToString(),
                MaxMembers = (int)row["MaxMembers"],
                CurrentMembers = (int)row["CurrentMembers"],
                TotalPoints = (int)row["TotalPoints"],
                Status = row["Status"].ToString(),
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }

        // ---------- management ----------
        public static Team Create(string name, string description, int maxMembers = 5)
        {
            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "INSERT INTO dbo.Team (Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status) " +
                    "VALUES (@name, @description, @maxMembers, 0, 0, 'Open'); " +
                    "SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@name", name),
                    new SqlParameter("@description", description ?? (object)DBNull.Value),
                    new SqlParameter("@maxMembers", maxMembers));

                if (result == null || result == DBNull.Value)
                {
                    throw new Exception("Failed to create team: no ID returned");
                }

                int teamId = Convert.ToInt32(result);
                return FindById(teamId);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error creating team: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating team: {ex.Message}", ex);
            }
        }

        public static bool Update(int id, string name, string description, int maxMembers)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Team SET Name = @name, Description = @description, MaxMembers = @maxMembers " +
                "WHERE Id = @id",
                new SqlParameter("@name", name),
                new SqlParameter("@description", description ?? (object)DBNull.Value),
                new SqlParameter("@maxMembers", maxMembers),
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool AddMember(int teamId, int userId)
        {
            // Check if user already in team
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT COUNT(*) FROM dbo.TeamMember WHERE TeamId = @teamId AND UserId = @userId",
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@userId", userId));

            int count = (int)dt.Rows[0][0];
            if (count > 0) return false; // Already a member

            // Check if team is full
            Team team = FindById(teamId);
            if (team.CurrentMembers >= team.MaxMembers) return false;

            // Add member
            DatabaseHelper.ExecuteCommand(
                "INSERT INTO dbo.TeamMember (TeamId, UserId) VALUES (@teamId, @userId)",
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@userId", userId));

            // Update user's team
            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.AppUser SET TeamId = @teamId WHERE Id = @userId",
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@userId", userId));

            // Update team member count
            int memberCount = (int)DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.TeamMember WHERE TeamId = @teamId",
                new SqlParameter("@teamId", teamId));

            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Team SET CurrentMembers = @count, Status = CASE WHEN @count >= MaxMembers THEN 'Full' ELSE 'Open' END WHERE Id = @id",
                new SqlParameter("@count", memberCount),
                new SqlParameter("@id", teamId));

            return true;
        }

        public static bool RemoveMember(int teamId, int userId)
        {
            DatabaseHelper.ExecuteCommand(
                "DELETE FROM dbo.TeamMember WHERE TeamId = @teamId AND UserId = @userId",
                new SqlParameter("@teamId", teamId),
                new SqlParameter("@userId", userId));

            // Update user's team
            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.AppUser SET TeamId = NULL WHERE Id = @userId",
                new SqlParameter("@userId", userId));

            // Update team member count
            int memberCount = (int)DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.TeamMember WHERE TeamId = @teamId",
                new SqlParameter("@teamId", teamId));

            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Team SET CurrentMembers = @count, Status = 'Open' WHERE Id = @id",
                new SqlParameter("@count", memberCount),
                new SqlParameter("@id", teamId));

            return true;
        }

        public static bool Delete(int id)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "DELETE FROM dbo.Team WHERE Id = @id",
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool UpdateTotalPoints(int teamId)
        {
            // Calculate total points from all members
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT COALESCE(SUM(Points), 0) FROM dbo.AppUser WHERE TeamId = @teamId",
                new SqlParameter("@teamId", teamId));

            int totalPoints = (int)result;

            // Update team
            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Team SET TotalPoints = @totalPoints WHERE Id = @teamId",
                new SqlParameter("@totalPoints", totalPoints),
                new SqlParameter("@teamId", teamId));

            return true;
        }
    }
}
