using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CyberArenaWebForms5.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }        // Player | Moderator | Admin
        public string Status { get; set; }      // Active | Suspended
        public string Team { get; set; }
        public int? TeamId { get; set; }
        public int Points { get; set; }
        public int Solved { get; set; }
        public DateTime Joined { get; set; }
        public DateTime LastActive { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }

        public bool IsAdmin { get { return Role == "Admin"; } }
        public bool IsModerator { get { return Role == "Moderator"; } }

        public string Initials
        {
            get
            {
                string source = string.IsNullOrWhiteSpace(FullName) ? Username : FullName;
                var parts = source.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2) return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
                return source.Substring(0, Math.Min(2, source.Length)).ToUpperInvariant();
            }
        }

        public string RoleCss { get { return Role == "Admin" ? "purple" : (Role == "Moderator" ? "orange" : "blue"); } }
        public string StatusCss { get { return Status == "Active" ? "green" : "red"; } }
        public string AvatarCss
        {
            get
            {
                string[] c = { "bg-blue", "bg-green", "bg-orange", "bg-purple", "bg-red", "bg-navy" };
                return c[Id % c.Length];
            }
        }
        public string PointsText { get { return Points.ToString("N0"); } }
        public string LastActiveText
        {
            get
            {
                TimeSpan d = DateTime.UtcNow - LastActive;
                if (d.TotalMinutes < 2) return "Just now";
                if (d.TotalHours < 1) return (int)d.TotalMinutes + " min ago";
                if (d.TotalDays < 1) return (int)d.TotalHours + " h ago";
                return (int)d.TotalDays + " d ago";
            }
        }
    }

    public enum LoginResult { Success, InvalidCredentials, Suspended, LockedOut }

    /// <summary>
    /// Data access layer for user operations. Uses SQL Server database.
    /// Passwords are stored as salted PBKDF2 hashes.
    /// </summary>
    public static class UserStore
    {
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, FailInfo> Failures = new Dictionary<string, FailInfo>();

        private class FailInfo { public int Count; public DateTime First; }

        private const int MaxFailures = 5;
        private static readonly TimeSpan LockWindow = TimeSpan.FromMinutes(5);

        // ---------- hashing ----------
        private static string NewSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string HashPassword(string password, string salt)
        {
            byte[] saltBytes;
            try
            {
                saltBytes = Convert.FromBase64String(salt ?? string.Empty);
            }
            catch (FormatException)
            {
                // A corrupt database record must not take down the login request.
                return null;
            }

            if (saltBytes.Length != 16) return null;

            using (var kdf = new Rfc2898DeriveBytes(password, saltBytes, 20000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(kdf.GetBytes(32));
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            byte[] x = Encoding.UTF8.GetBytes(a), y = Encoding.UTF8.GetBytes(b);
            int diff = x.Length ^ y.Length;
            for (int i = 0; i < x.Length && i < y.Length; i++) diff |= x[i] ^ y[i];
            return diff == 0;
        }

        // ---------- queries ----------
        public static List<AppUser> All()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Username, FullName, Email, Role, Status, Team, TeamId, Points, Solved, Joined, LastActive, Salt, Hash " +
                "FROM dbo.AppUser ORDER BY Id");

            var users = new List<AppUser>();
            foreach (DataRow row in dt.Rows)
            {
                users.Add(MapRowToUser(row));
            }
            return users;
        }

        public static AppUser FindById(int id)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Username, FullName, Email, Role, Status, Team, TeamId, Points, Solved, Joined, LastActive, Salt, Hash " +
                "FROM dbo.AppUser WHERE Id = " + id);

            if (dt.Rows.Count == 0) return null;
            return MapRowToUser(dt.Rows[0]);
        }

        public static AppUser FindByIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier)) return null;

            var param = new SqlParameter("@identifier", identifier);
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT Id, Username, FullName, Email, Role, Status, Team, TeamId, Points, Solved, Joined, LastActive, Salt, Hash " +
                "FROM dbo.AppUser WHERE Username = @identifier OR Email = @identifier", param);

            if (dt.Rows.Count == 0) return null;
            return MapRowToUser(dt.Rows[0]);
        }

        private static AppUser MapRowToUser(DataRow row)
        {
            return new AppUser
            {
                Id = (int)row["Id"],
                Username = row["Username"].ToString(),
                FullName = row["FullName"]?.ToString(),
                Email = row["Email"].ToString(),
                Role = row["Role"].ToString(),
                Status = row["Status"].ToString(),
                Team = row["Team"]?.ToString(),
                TeamId = row["TeamId"] == DBNull.Value ? (int?)null : (int)row["TeamId"],
                Points = (int)row["Points"],
                Solved = (int)row["Solved"],
                Joined = (DateTime)row["Joined"],
                LastActive = (DateTime)row["LastActive"],
                Salt = row["Salt"].ToString(),
                Hash = row["Hash"].ToString()
            };
        }

        // Public version for use by other classes
        public static AppUser MapRowToUserPublic(DataRow row)
        {
            return MapRowToUser(row);
        }

        // ---------- authentication ----------
        public static LoginResult Authenticate(string identifier, string password, out AppUser user)
        {
            user = null;
            string key = (identifier ?? string.Empty).Trim().ToLowerInvariant();

            lock (Sync)
            {
                FailInfo info;
                if (Failures.TryGetValue(key, out info))
                {
                    if (DateTime.UtcNow - info.First > LockWindow) Failures.Remove(key);
                    else if (info.Count >= MaxFailures) return LoginResult.LockedOut;
                }

                AppUser found = FindByIdentifier(identifier);
                bool ok = false;
                if (found != null && !string.IsNullOrEmpty(password))
                {
                    string computedHash = HashPassword(password, found.Salt);
                    ok = computedHash != null && SlowEquals(computedHash, found.Hash);
                }
                else
                {
                    // Keep timing similar when the account does not exist
                    HashPassword(password ?? string.Empty, "AAAAAAAAAAAAAAAAAAAAAA==");
                }

                if (!ok)
                {
                    if (!Failures.TryGetValue(key, out info))
                    {
                        info = new FailInfo { Count = 0, First = DateTime.UtcNow };
                        Failures[key] = info;
                    }
                    info.Count++;
                    return LoginResult.InvalidCredentials;
                }

                Failures.Remove(key);
                if (found.Status != "Active") return LoginResult.Suspended;

                // Update LastActive
                DatabaseHelper.ExecuteCommand(
                    "UPDATE dbo.AppUser SET LastActive = GETUTCDATE() WHERE Id = @id",
                    new SqlParameter("@id", found.Id));

                found.LastActive = DateTime.UtcNow;
                user = found;
                return LoginResult.Success;
            }
        }

        // ---------- registration / management ----------
        public static bool IsUsernameTaken(string username)
        {
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.AppUser WHERE LOWER(Username) = LOWER(@username)",
                new SqlParameter("@username", username));
            return (int)result > 0;
        }

        public static bool IsEmailTaken(string email)
        {
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.AppUser WHERE LOWER(Email) = LOWER(@email)",
                new SqlParameter("@email", email));
            return (int)result > 0;
        }

        /// <summary>Creates an account. Self-registration always passes role "Player".</summary>
        public static AppUser Create(string username, string fullName, string email, string password, string role)
        {
            string salt = NewSalt();
            string hash = HashPassword(password, salt);

            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "INSERT INTO dbo.AppUser (Username, FullName, Email, Role, Status, Team, Points, Solved, Joined, LastActive, Salt, Hash) " +
                    "VALUES (@username, @fullName, @email, @role, 'Active', 'None', 0, 0, GETUTCDATE(), GETUTCDATE(), @salt, @hash); " +
                    "SELECT SCOPE_IDENTITY();",
                    new SqlParameter("@username", username),
                    new SqlParameter("@fullName", fullName ?? (object)DBNull.Value),
                    new SqlParameter("@email", email),
                    new SqlParameter("@role", role),
                    new SqlParameter("@salt", salt),
                    new SqlParameter("@hash", hash));

                // SCOPE_IDENTITY() returns a Decimal, need to convert properly
                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("Failed to retrieve new user ID.");
                }

                int userId = Convert.ToInt32(result);
                AppUser newUser = FindById(userId);

                if (newUser == null)
                {
                    throw new InvalidOperationException("User was created but could not be retrieved from database.");
                }

                return newUser;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation
                {
                    throw new InvalidOperationException("Username or email already exists.", ex);
                }
                throw new InvalidOperationException("Database error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create user account: " + ex.Message, ex);
            }
        }

        public static int AdminCount()
        {
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.AppUser WHERE Role = 'Admin' AND Status = 'Active'");
            return (int)result;
        }

        public static bool SetRole(int id, string role)
        {
            AppUser u = FindById(id);
            if (u == null) return false;
            if (u.Role == "Admin" && role != "Admin" && AdminCount() <= 1) return false; // Keep one admin

            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.AppUser SET Role = @role WHERE Id = @id",
                new SqlParameter("@role", role),
                new SqlParameter("@id", id));
            return true;
        }

        public static bool SetStatus(int id, string status)
        {
            AppUser u = FindById(id);
            if (u == null) return false;
            if (u.Role == "Admin" && status != "Active" && AdminCount() <= 1) return false;

            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.AppUser SET Status = @status WHERE Id = @id",
                new SqlParameter("@status", status),
                new SqlParameter("@id", id));
            return true;
        }

        public static bool Delete(int id)
        {
            AppUser u = FindById(id);
            if (u == null) return false;
            if (u.Role == "Admin" && AdminCount() <= 1) return false;

            DatabaseHelper.ExecuteCommand(
                "DELETE FROM dbo.AppUser WHERE Id = @id",
                new SqlParameter("@id", id));
            return true;
        }

        public static void UpdateProfile(int id, string fullName, string username, string email)
        {
            DatabaseHelper.ExecuteCommand(
                "UPDATE dbo.AppUser SET FullName = @fullName, Username = @username, Email = @email WHERE Id = @id",
                new SqlParameter("@fullName", fullName ?? (object)DBNull.Value),
                new SqlParameter("@username", username),
                new SqlParameter("@email", email),
                new SqlParameter("@id", id));
        }
    }
}
