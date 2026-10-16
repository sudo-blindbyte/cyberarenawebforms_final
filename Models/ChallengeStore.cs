using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CyberArenaWebForms5.Models
{
    /// <summary>
    /// Data access layer for challenge operations. Uses SQL Server database.
    /// </summary>
    public static class ChallengeStore
    {
        // ---------- queries ----------
        public static List<Challenge> All(bool onlyPublished = false)
        {
            string query = "SELECT Id, Title, Subtitle, Category, Difficulty, Status, Solves, Points, SuccessRate, " +
                           "Description, Hint, Attachment, Flag, IsPublished FROM dbo.Challenge";

            if (onlyPublished)
                query += " WHERE IsPublished = 1";

            query += " ORDER BY Id";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            var challenges = new List<Challenge>();

            foreach (DataRow row in dt.Rows)
            {
                challenges.Add(MapRowToChallenge(row));
            }

            return challenges;
        }

        public static Challenge FindById(int id)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Title, Subtitle, Category, Difficulty, Status, Solves, Points, SuccessRate, " +
                "Description, Hint, Attachment, Flag, IsPublished FROM dbo.Challenge WHERE Id = " + id);

            if (dt.Rows.Count == 0) return null;
            return MapRowToChallenge(dt.Rows[0]);
        }

        public static List<Challenge> GetByCategory(string category, bool onlyPublished = false)
        {
            string query = "SELECT Id, Title, Subtitle, Category, Difficulty, Status, Solves, Points, SuccessRate, " +
                           "Description, Hint, Attachment, Flag, IsPublished FROM dbo.Challenge WHERE Category = @category";

            if (onlyPublished)
                query += " AND IsPublished = 1";

            query += " ORDER BY Difficulty, Points DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@category", category));
            var challenges = new List<Challenge>();

            foreach (DataRow row in dt.Rows)
            {
                challenges.Add(MapRowToChallenge(row));
            }

            return challenges;
        }

        public static List<Challenge> GetByDifficulty(string difficulty, bool onlyPublished = false)
        {
            string query = "SELECT Id, Title, Subtitle, Category, Difficulty, Status, Solves, Points, SuccessRate, " +
                           "Description, Hint, Attachment, Flag, IsPublished FROM dbo.Challenge WHERE Difficulty = @difficulty";

            if (onlyPublished)
                query += " AND IsPublished = 1";

            query += " ORDER BY Points DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@difficulty", difficulty));
            var challenges = new List<Challenge>();

            foreach (DataRow row in dt.Rows)
            {
                challenges.Add(MapRowToChallenge(row));
            }

            return challenges;
        }

        public static List<string> GetCategories()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT DISTINCT Category FROM dbo.Challenge WHERE IsPublished = 1 ORDER BY Category");

            var categories = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                categories.Add(row["Category"].ToString());
            }

            return categories;
        }

        private static Challenge MapRowToChallenge(DataRow row)
        {
            return new Challenge
            {
                Id = (int)row["Id"],
                Title = row["Title"].ToString(),
                Subtitle = row["Subtitle"]?.ToString(),
                Category = row["Category"].ToString(),
                Difficulty = row["Difficulty"].ToString(),
                Status = row["Status"]?.ToString() ?? "Unsolved",
                Solves = (int)row["Solves"],
                Points = (int)row["Points"],
                SuccessRate = (double)row["SuccessRate"],
                Description = row["Description"]?.ToString(),
                Hint = row["Hint"]?.ToString(),
                Attachment = row["Attachment"]?.ToString(),
                Flag = row["Flag"].ToString(),
                IsPublished = (bool)row["IsPublished"]
            };
        }

        // ---------- management ----------
        public static Challenge Create(string title, string subtitle, string category, string difficulty,
            int points, string description, string hint, string attachment, string flag, bool isPublished = false)
        {
            int challengeId = (int)DatabaseHelper.ExecuteScalar(
                "INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Points, Description, Hint, Attachment, Flag, IsPublished, Solves, SuccessRate, Status) " +
                "VALUES (@title, @subtitle, @category, @difficulty, @points, @description, @hint, @attachment, @flag, @isPublished, 0, 0.0, 'Draft'); " +
                "SELECT SCOPE_IDENTITY();",
                new SqlParameter("@title", title),
                new SqlParameter("@subtitle", subtitle ?? (object)DBNull.Value),
                new SqlParameter("@category", category),
                new SqlParameter("@difficulty", difficulty),
                new SqlParameter("@points", points),
                new SqlParameter("@description", description ?? (object)DBNull.Value),
                new SqlParameter("@hint", hint ?? (object)DBNull.Value),
                new SqlParameter("@attachment", attachment ?? (object)DBNull.Value),
                new SqlParameter("@flag", flag),
                new SqlParameter("@isPublished", isPublished));

            return FindById((int)challengeId);
        }

        public static bool Update(int id, string title, string subtitle, string category, string difficulty,
            int points, string description, string hint, string attachment, string flag, bool isPublished)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Challenge SET Title = @title, Subtitle = @subtitle, Category = @category, " +
                "Difficulty = @difficulty, Points = @points, Description = @description, Hint = @hint, " +
                "Attachment = @attachment, Flag = @flag, IsPublished = @isPublished, UpdatedAt = GETUTCDATE() " +
                "WHERE Id = @id",
                new SqlParameter("@title", title),
                new SqlParameter("@subtitle", subtitle ?? (object)DBNull.Value),
                new SqlParameter("@category", category),
                new SqlParameter("@difficulty", difficulty),
                new SqlParameter("@points", points),
                new SqlParameter("@description", description ?? (object)DBNull.Value),
                new SqlParameter("@hint", hint ?? (object)DBNull.Value),
                new SqlParameter("@attachment", attachment ?? (object)DBNull.Value),
                new SqlParameter("@flag", flag),
                new SqlParameter("@isPublished", isPublished),
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool Publish(int id)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Challenge SET IsPublished = 1, Status = 'Published', UpdatedAt = GETUTCDATE() WHERE Id = @id",
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool Delete(int id)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "DELETE FROM dbo.Challenge WHERE Id = @id",
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool IncrementSolves(int id)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Challenge SET Solves = Solves + 1 WHERE Id = @id",
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }

        public static bool UpdateSuccessRate(int id, double rate)
        {
            int rowsAffected = DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.Challenge SET SuccessRate = @rate WHERE Id = @id",
                new SqlParameter("@rate", rate),
                new SqlParameter("@id", id));

            return rowsAffected > 0;
        }
    }
}
