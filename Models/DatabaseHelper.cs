using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace CyberArenaWebForms5.Models
{
    /// <summary>
    /// DatabaseHelper class manages database connections and initialization.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["CyberArenaDb"]?.ConnectionString;
        private static bool _initialized = false;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// Gets a new database connection.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("Connection string 'CyberArenaDb' not found in Web.config");

            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Initializes the database by creating tables if they don't exist.
        /// This is called once on application startup.
        /// </summary>
        public static void InitializeDatabase()
        {
            if (_initialized) return;

            lock (_syncLock)
            {
                if (_initialized) return;

                try
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        conn.Open();

                        // Check if AppUser table exists
                        string checkTableQuery = @"
                            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                            WHERE TABLE_NAME = 'AppUser'";

                        using (SqlCommand cmd = new SqlCommand(checkTableQuery, conn))
                        {
                            int count = (int)cmd.ExecuteScalar();
                            if (count == 0)
                            {
                                // Tables don't exist, create them
                                CreateTables(conn);
                            }

                            // Older seed scripts stored values such as "salt1" and
                            // "hash1". Repair those development accounts so the
                            // demo login uses the same PBKDF2 format as registration.
                            RepairDemoAccounts(conn);
                        }
                    }

                    _initialized = true;
                }
                catch (SqlException ex)
                {
                    // If database doesn't exist, provide helpful message
                    if (ex.Number == -2146893056 || ex.Message.Contains("Cannot open database"))
                    {
                        throw new InvalidOperationException(
                            "Database 'CyberArena' not found. Please follow these steps:\n\n" +
                            "1. Open Visual Studio\n" +
                            "2. Go to View → Server Explorer\n" +
                            "3. Right-click 'Data Connections' → 'Create New SQL Server Database'\n" +
                            "4. Server Name: (LocalDb)\\MSSQLLocalDB\n" +
                            "5. New Database Name: CyberArena\n" +
                            "6. Click OK\n\n" +
                            "Then run the application again.", ex);
                    }
                    throw new InvalidOperationException("Failed to initialize database.", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to initialize database.", ex);
                }
            }
        }

        private static void RepairDemoAccounts(SqlConnection conn)
        {
            RepairDemoAccount(conn, "admin", "admin@cyberarena.com", "Admin@123", "Admin");
            RepairDemoAccount(conn, "moderator", "mod@cyberarena.com", "Mod@12345", "Moderator");
        }

        private static void RepairDemoAccount(SqlConnection conn, string username, string email, string password, string role)
        {
            const string selectSql = "SELECT Salt FROM dbo.AppUser WHERE Username = @username";
            string salt = null;

            using (var select = new SqlCommand(selectSql, conn))
            {
                select.Parameters.AddWithValue("@username", username);
                object value = select.ExecuteScalar();
                if (value != null && value != DBNull.Value) salt = value.ToString();
            }

            bool validSalt = false;
            if (!string.IsNullOrWhiteSpace(salt))
            {
                try
                {
                    validSalt = Convert.FromBase64String(salt).Length == 16;
                }
                catch (FormatException) { }
            }

            if (validSalt) return;

            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(saltBytes);
            string newSalt = Convert.ToBase64String(saltBytes);
            byte[] hashBytes;
            using (var kdf = new Rfc2898DeriveBytes(password, saltBytes, 20000, HashAlgorithmName.SHA256))
            {
                hashBytes = kdf.GetBytes(32);
            }

            const string updateSql = @"
                UPDATE dbo.AppUser
                SET Email = @email, Role = @role, Status = 'Active', Salt = @salt, Hash = @hash
                WHERE Username = @username";

            using (var update = new SqlCommand(updateSql, conn))
            {
                update.Parameters.AddWithValue("@email", email);
                update.Parameters.AddWithValue("@role", role);
                update.Parameters.AddWithValue("@salt", newSalt);
                update.Parameters.AddWithValue("@hash", Convert.ToBase64String(hashBytes));
                update.Parameters.AddWithValue("@username", username);
                update.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates all database tables.
        /// </summary>
        private static void CreateTables(SqlConnection conn)
        {
            string[] sqlStatements = new string[]
            {
                // Create Team table first (required for foreign keys)
                @"CREATE TABLE dbo.Team (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL UNIQUE,
                    Description NVARCHAR(MAX),
                    MaxMembers INT DEFAULT 5,
                    CurrentMembers INT DEFAULT 0,
                    TotalPoints INT DEFAULT 0,
                    Status NVARCHAR(50) DEFAULT 'Open',
                    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                );",

                @"CREATE INDEX IX_Team_Name ON dbo.Team(Name);",
                @"CREATE INDEX IX_Team_Status ON dbo.Team(Status);",

                // Create AppUser table with TeamId reference
                @"CREATE TABLE dbo.AppUser (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Username NVARCHAR(50) NOT NULL UNIQUE,
                    FullName NVARCHAR(255),
                    Email NVARCHAR(255) NOT NULL UNIQUE,
                    Role NVARCHAR(50) NOT NULL DEFAULT 'Player',
                    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
                    Team NVARCHAR(255),
                    TeamId INT,
                    Points INT NOT NULL DEFAULT 0,
                    Solved INT NOT NULL DEFAULT 0,
                    Rank INT,
                    Joined DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    LastActive DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    Salt NVARCHAR(MAX) NOT NULL,
                    Hash NVARCHAR(MAX) NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id) ON DELETE SET NULL
                );",

                @"CREATE INDEX IX_AppUser_Username ON dbo.AppUser(Username);",
                @"CREATE INDEX IX_AppUser_Email ON dbo.AppUser(Email);",
                @"CREATE INDEX IX_AppUser_Status ON dbo.AppUser(Status);",
                @"CREATE INDEX IX_AppUser_Points ON dbo.AppUser(Points) DESC;",
                @"CREATE INDEX IX_AppUser_Rank ON dbo.AppUser(Rank);",
                @"CREATE INDEX IX_AppUser_TeamId ON dbo.AppUser(TeamId);",

                // Create Challenge table
                @"CREATE TABLE dbo.Challenge (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Title NVARCHAR(255) NOT NULL,
                    Subtitle NVARCHAR(255),
                    Category NVARCHAR(100) NOT NULL,
                    Difficulty NVARCHAR(50) NOT NULL,
                    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
                    Solves INT NOT NULL DEFAULT 0,
                    Points INT NOT NULL DEFAULT 0,
                    SuccessRate FLOAT NOT NULL DEFAULT 0.0,
                    Description NVARCHAR(MAX),
                    Hint NVARCHAR(MAX),
                    Attachment NVARCHAR(255),
                    Flag NVARCHAR(MAX) NOT NULL,
                    IsPublished BIT NOT NULL DEFAULT 0,
                    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                );",

                @"CREATE INDEX IX_Challenge_Category ON dbo.Challenge(Category);",
                @"CREATE INDEX IX_Challenge_Difficulty ON dbo.Challenge(Difficulty);",
                @"CREATE INDEX IX_Challenge_IsPublished ON dbo.Challenge(IsPublished);",

                // Create Submission table
                @"CREATE TABLE dbo.Submission (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    UserId INT NOT NULL,
                    ChallengeId INT NOT NULL,
                    Flag NVARCHAR(MAX) NOT NULL,
                    IsCorrect BIT NOT NULL DEFAULT 0,
                    SubmittedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    FOREIGN KEY (UserId) REFERENCES dbo.AppUser(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ChallengeId) REFERENCES dbo.Challenge(Id) ON DELETE CASCADE
                );",

                @"CREATE INDEX IX_Submission_UserId ON dbo.Submission(UserId);",
                @"CREATE INDEX IX_Submission_ChallengeId ON dbo.Submission(ChallengeId);",
                @"CREATE INDEX IX_Submission_IsCorrect ON dbo.Submission(IsCorrect);",

                // Create TeamMember table
                @"CREATE TABLE dbo.TeamMember (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    TeamId INT NOT NULL,
                    UserId INT NOT NULL,
                    JoinedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id) ON DELETE CASCADE,
                    FOREIGN KEY (UserId) REFERENCES dbo.AppUser(Id) ON DELETE CASCADE,
                    UNIQUE (TeamId, UserId)
                );",

                @"CREATE INDEX IX_TeamMember_TeamId ON dbo.TeamMember(TeamId);",
                @"CREATE INDEX IX_TeamMember_UserId ON dbo.TeamMember(UserId);",

                // Create AuditLog table
                @"CREATE TABLE dbo.AuditLog (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Time DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    Actor NVARCHAR(255) NOT NULL,
                    Action NVARCHAR(255) NOT NULL,
                    Target NVARCHAR(MAX),
                    Ip NVARCHAR(45),
                    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                );",

                @"CREATE INDEX IX_AuditLog_Time ON dbo.AuditLog(Time);",
                @"CREATE INDEX IX_AuditLog_Actor ON dbo.AuditLog(Actor);",

                // Insert Teams
                @"INSERT INTO dbo.Team (Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status)
                VALUES 
                    ('RootAccess', 'Security researchers focused on web exploitation and forensics.', 5, 4, 3300, 'Open'),
                    ('ZeroDay', 'A collaborative team solving challenging security puzzles.', 5, 3, 2725, 'Open'),
                    ('BinaryOps', 'Reverse engineering, binary analysis and low-level security.', 5, 5, 2175, 'Full'),
                    ('NullCrew', 'Learning cybersecurity through hands-on CTF challenges.', 5, 2, 1420, 'Open'),
                    ('PacketStorm', 'Network analysis, traffic investigation and protocol research.', 5, 5, 1200, 'Full'),
                    ('CipherLab', 'Cryptography enthusiasts working through complex ciphers.', 5, 1, 650, 'Open');",

                // Insert Users with Team Assignments
                @"INSERT INTO dbo.AppUser (Username, FullName, Email, Role, Status, TeamId, Points, Solved, Rank, Salt, Hash)
                VALUES 
                    ('admin', 'Administrator', 'admin@cyberarena.local', 'Admin', 'Active', NULL, 0, 0, NULL, 'salt1', 'hash1'),
                    ('moderator', 'Moderator User', 'mod@cyberarena.local', 'Moderator', 'Active', NULL, 0, 0, NULL, 'salt2', 'hash2'),
                    ('NullPointer', 'Null Pointer', 'nullpointer@cyberarena.local', 'Player', 'Active', 1, 1850, 24, 1, 'salt3', 'hash3'),
                    ('ByteHunter', 'Byte Hunter', 'bytehunter@cyberarena.local', 'Player', 'Active', 2, 1625, 21, 2, 'salt4', 'hash4'),
                    ('PacketGhost', 'Packet Ghost', 'packetghost@cyberarena.local', 'Player', 'Active', 1, 1450, 19, 3, 'salt5', 'hash5'),
                    ('CipherX', 'Cipher X', 'cipherx@cyberarena.local', 'Player', 'Active', 3, 1275, 17, 4, 'salt6', 'hash6'),
                    ('TraceRoute', 'Trace Route', 'traceroute@cyberarena.local', 'Player', 'Active', 2, 1100, 15, 5, 'salt7', 'hash7'),
                    ('CyberNova', 'Cyber Nova', 'cybernova@cyberarena.local', 'Player', 'Active', 3, 900, 13, 6, 'salt8', 'hash8'),
                    ('HexHunter', 'Hex Hunter', 'hexhunter@cyberarena.local', 'Player', 'Active', 4, 750, 11, 7, 'salt9', 'hash9'),
                    ('Participant', 'Participant User', 'participant@cyberarena.local', 'Player', 'Active', NULL, 450, 8, 8, 'salt10', 'hash10');",

                // Insert Team Members
                @"INSERT INTO dbo.TeamMember (TeamId, UserId)
                VALUES 
                    (1, 3), (1, 5),
                    (2, 4), (2, 7),
                    (3, 6), (3, 8),
                    (4, 9);",

                // Insert Challenges - 12 total
                @"INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Solves, Points, SuccessRate, Description, Flag, IsPublished)
                VALUES 
                    ('Hidden Message', 'Inspect a suspicious file', 'Digital Forensics', 'Easy', 1420, 50, 78.4, 
                     'Inspect a suspicious file to discover information hidden inside it.', 'CTF{hidden_in_metadata}', 1),
                    ('Caesar''s Secret', 'Classic substitution cipher', 'Cryptography', 'Easy', 1310, 75, 72.1, 
                     'Decode a message encrypted using a classic substitution technique.', 'CTF{rome_was_not_built_in_a_day}', 1),
                    ('SQL Injection Basics', 'Break a search form', 'Web Security', 'Easy', 1105, 100, 66.0, 
                     'A product search form builds its query from raw input.', 'CTF{union_select_wins}', 1),
                    ('Hidden in Plain Sight', 'Stego warm-up', 'Steganography', 'Easy', 980, 75, 61.7, 
                     'A harmless-looking image carries a secret.', 'CTF{appended_data}', 1),
                    ('Basic Network Scan', 'Enumerate open services', 'Networking', 'Easy', 763, 50, 59.3, 
                     'Use standard tools to discover services running on a target system.', 'CTF{nmap_is_your_friend}', 1),
                    ('Broken Login', 'Vulnerable practice login', 'Web Security', 'Medium', 842, 100, 41.6, 
                     'Investigate a deliberately vulnerable practice login application.', 'CTF{login_bypassed}', 1),
                    ('Trace the Domain', 'Investigate a fictional domain', 'OSINT', 'Medium', 655, 125, 38.2, 
                     'Use public information to investigate a fictional domain.', 'CTF{whois_who_is_it}', 1),
                    ('Packet Detective', 'Analyse a network capture', 'Networking', 'Medium', 590, 100, 35.9, 
                     'Examine a sample network capture and identify evidence hidden in traffic.', 'CTF{follow_the_stream}', 1),
                    ('Reverse Me', 'Find the key in the binary', 'Reverse Engineering', 'Medium', 428, 100, 38.5, 
                     'Analyze a binary to recover the hidden key inside the executable.', 'CTF{r3v3rs3_m3_pl3as3}', 1),
                    ('API Abuse', 'Misused endpoints', 'Web Security', 'Medium', 421, 125, 37.8, 
                     'An API has endpoints that expose more data than intended.', 'CTF{api_says_oops}', 1),
                    ('Pixel Clue', 'Concealed message in an image', 'Steganography', 'Hard', 312, 150, 21.4, 
                     'Explore a training image to locate a concealed message.', 'CTF{lsb_never_lies}', 1),
                    ('Buffer Overflow 101', 'Classic memory corruption', 'Binary Exploitation', 'Hard', 285, 200, 18.5, 
                     'Overflow a buffer to overwrite return addresses.', 'CTF{segmentation_fault}', 1);",

                // Insert Audit Log
                @"INSERT INTO dbo.AuditLog (Actor, Action, Target, Ip)
                VALUES 
                    ('system', 'Platform started', 'CyberArena v5', '127.0.0.1'),
                    ('admin', 'Published challenge', 'Hidden Message', '10.0.0.4'),
                    ('admin', 'Published challenge', 'Caesar''s Secret', '10.0.0.4'),
                    ('moderator', 'Updated leaderboard', 'Season 01', '10.0.0.9'),
                    ('system', 'Teams created', '6 teams initialized', '127.0.0.1'),
                    ('system', 'Database initialized', 'All tables and data loaded', '127.0.0.1');"
            };

            foreach (string sql in sqlStatements)
            {
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Executes a SQL query and returns the result as a DataTable.
        /// </summary>
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Executes a SQL command with parameters.
        /// </summary>
        public static int ExecuteCommand(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a SQL query and returns the first column of the first row.
        /// </summary>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Populates seed data into empty tables. Call this manually if data is missing.
        /// </summary>
        public static bool PopulateSeedData()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();

                    // Check if we already have teams
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Team", conn))
                    {
                        int teamCount = (int)cmd.ExecuteScalar();
                        if (teamCount > 0)
                        {
                            // Data already exists, avoid duplicates
                            return true;
                        }
                    }

                    // Insert Teams
                    string teamsInsert = @"
                        INSERT INTO dbo.Team (Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status)
                        VALUES 
                            ('RootAccess', 'Security researchers focused on web exploitation and forensics.', 5, 4, 3300, 'Open'),
                            ('ZeroDay', 'A collaborative team solving challenging security puzzles.', 5, 3, 2725, 'Open'),
                            ('BinaryOps', 'Reverse engineering, binary analysis and low-level security.', 5, 5, 2175, 'Full'),
                            ('NullCrew', 'Learning cybersecurity through hands-on CTF challenges.', 5, 2, 1420, 'Open'),
                            ('PacketStorm', 'Network analysis, traffic investigation and protocol research.', 5, 5, 1200, 'Full'),
                            ('CipherLab', 'Cryptography enthusiasts working through complex ciphers.', 5, 1, 650, 'Open');";

                    using (SqlCommand cmd = new SqlCommand(teamsInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Users - Note: These use demo hashes that will be updated by RepairDemoAccounts
                    string usersInsert = @"
                        INSERT INTO dbo.AppUser (Username, FullName, Email, Role, Status, TeamId, Points, Solved, Rank, Salt, Hash)
                        VALUES 
                            ('admin', 'Administrator', 'admin@cyberarena.local', 'Admin', 'Active', NULL, 0, 0, NULL, 'salt1', 'hash1'),
                            ('moderator', 'Moderator User', 'mod@cyberarena.local', 'Moderator', 'Active', NULL, 0, 0, NULL, 'salt2', 'hash2'),
                            ('NullPointer', 'Null Pointer', 'nullpointer@cyberarena.local', 'Player', 'Active', 1, 1850, 24, 1, 'salt3', 'hash3'),
                            ('ByteHunter', 'Byte Hunter', 'bytehunter@cyberarena.local', 'Player', 'Active', 2, 1625, 21, 2, 'salt4', 'hash4'),
                            ('PacketGhost', 'Packet Ghost', 'packetghost@cyberarena.local', 'Player', 'Active', 1, 1450, 19, 3, 'salt5', 'hash5'),
                            ('CipherX', 'Cipher X', 'cipherx@cyberarena.local', 'Player', 'Active', 3, 1275, 17, 4, 'salt6', 'hash6'),
                            ('TraceRoute', 'Trace Route', 'traceroute@cyberarena.local', 'Player', 'Active', 2, 1100, 15, 5, 'salt7', 'hash7'),
                            ('CyberNova', 'Cyber Nova', 'cybernova@cyberarena.local', 'Player', 'Active', 3, 900, 13, 6, 'salt8', 'hash8'),
                            ('HexHunter', 'Hex Hunter', 'hexhunter@cyberarena.local', 'Player', 'Active', 4, 750, 11, 7, 'salt9', 'hash9'),
                            ('Participant', 'Participant User', 'participant@cyberarena.local', 'Player', 'Active', NULL, 450, 8, 8, 'salt10', 'hash10');";

                    using (SqlCommand cmd = new SqlCommand(usersInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Team Members
                    string teamMembersInsert = @"
                        INSERT INTO dbo.TeamMember (TeamId, UserId)
                        VALUES 
                            (1, 3), (1, 5),
                            (2, 4), (2, 7),
                            (3, 6), (3, 8),
                            (4, 9);";

                    using (SqlCommand cmd = new SqlCommand(teamMembersInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Challenges
                    string challengesInsert = @"
                        INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Solves, Points, SuccessRate, Description, Flag, IsPublished)
                        VALUES 
                            ('Hidden Message', 'Inspect a suspicious file', 'Digital Forensics', 'Easy', 1420, 50, 78.4, 'Inspect a suspicious file to discover information hidden inside it.', 'CTF{hidden_in_metadata}', 1),
                            ('Caesar''s Secret', 'Classic substitution cipher', 'Cryptography', 'Easy', 1310, 75, 72.1, 'Decode a message encrypted using a classic substitution technique.', 'CTF{rome_was_not_built_in_a_day}', 1),
                            ('SQL Injection Basics', 'Break a search form', 'Web Security', 'Easy', 1105, 100, 66.0, 'A product search form builds its query from raw input.', 'CTF{union_select_wins}', 1),
                            ('Hidden in Plain Sight', 'Stego warm-up', 'Steganography', 'Easy', 980, 75, 61.7, 'A harmless-looking image carries a secret.', 'CTF{appended_data}', 1),
                            ('Basic Network Scan', 'Enumerate open services', 'Networking', 'Easy', 763, 50, 59.3, 'Use standard tools to discover services running on a target system.', 'CTF{nmap_is_your_friend}', 1),
                            ('Broken Login', 'Vulnerable practice login', 'Web Security', 'Medium', 842, 100, 41.6, 'Investigate a deliberately vulnerable practice login application.', 'CTF{login_bypassed}', 1),
                            ('Trace the Domain', 'Investigate a fictional domain', 'OSINT', 'Medium', 655, 125, 38.2, 'Use public information to investigate a fictional domain.', 'CTF{whois_who_is_it}', 1),
                            ('Packet Detective', 'Analyse a network capture', 'Networking', 'Medium', 590, 100, 35.9, 'Examine a sample network capture and identify evidence hidden in traffic.', 'CTF{follow_the_stream}', 1),
                            ('Reverse Me', 'Find the key in the binary', 'Reverse Engineering', 'Medium', 428, 100, 38.5, 'Analyze a binary to recover the hidden key inside the executable.', 'CTF{r3v3rs3_m3_pl3as3}', 1),
                            ('API Abuse', 'Misused endpoints', 'Web Security', 'Medium', 421, 125, 37.8, 'An API has endpoints that reveal sensitive data or allow unauthorized actions.', 'CTF{api_broken_badly}', 1),
                            ('Forensics Deep Dive', 'Recover deleted data', 'Digital Forensics', 'Hard', 215, 200, 24.3, 'Recover and analyze deleted or hidden data from a disk image.', 'CTF{undeleted_secrets}', 1),
                            ('Crypto Challenge', 'Advanced encryption puzzle', 'Cryptography', 'Hard', 142, 250, 18.7, 'Break an advanced encryption scheme using cryptanalysis techniques.', 'CTF{crypto_masterclass}', 1);";

                    using (SqlCommand cmd = new SqlCommand(challengesInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Submissions
                    string submissionsInsert = @"
                        INSERT INTO dbo.Submission (UserId, ChallengeId, Flag, IsCorrect, SubmittedAt)
                        VALUES 
                            (3, 1, 'CTF{hidden_in_metadata}', 1, DATEADD(day, -5, GETUTCDATE())),
                            (3, 2, 'CTF{wrong_guess}', 0, DATEADD(day, -5, GETUTCDATE())),
                            (3, 2, 'CTF{rome_was_not_built_in_a_day}', 1, DATEADD(day, -4, GETUTCDATE())),
                            (4, 1, 'CTF{hidden_in_metadata}', 1, DATEADD(day, -3, GETUTCDATE())),
                            (5, 3, 'CTF{union_select_wins}', 1, DATEADD(day, -2, GETUTCDATE())),
                            (6, 4, 'CTF{appended_data}', 1, DATEADD(day, -1, GETUTCDATE())),
                            (7, 5, 'CTF{nmap_is_your_friend}', 1, DATEADD(hour, -6, GETUTCDATE())),
                            (8, 6, 'CTF{wrong_flag}', 0, DATEADD(hour, -5, GETUTCDATE())),
                            (8, 6, 'CTF{login_bypassed}', 1, DATEADD(hour, -4, GETUTCDATE())),
                            (9, 7, 'CTF{incorrect}', 0, DATEADD(hour, -3, GETUTCDATE()));";

                    using (SqlCommand cmd = new SqlCommand(submissionsInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Audit Log entries
                    string auditInsert = @"
                        INSERT INTO dbo.AuditLog (Time, Actor, Action, Target, Ip)
                        VALUES 
                            (DATEADD(day, -7, GETUTCDATE()), 'admin', 'Login', 'User admin logged in', '192.168.1.100'),
                            (DATEADD(day, -6, GETUTCDATE()), 'NullPointer', 'Challenge Solved', 'Hidden Message', '192.168.1.101'),
                            (DATEADD(day, -5, GETUTCDATE()), 'ByteHunter', 'Challenge Solved', 'Caesar''s Secret', '192.168.1.102'),
                            (DATEADD(day, -4, GETUTCDATE()), 'moderator', 'Challenge Created', 'New Challenge Added', '192.168.1.103'),
                            (DATEADD(day, -3, GETUTCDATE()), 'PacketGhost', 'Challenge Solved', 'SQL Injection Basics', '192.168.1.104'),
                            (DATEADD(day, -2, GETUTCDATE()), 'CipherX', 'Team Created', 'Team: BinaryOps', '192.168.1.105'),
                            (DATEADD(day, -1, GETUTCDATE()), 'TraceRoute', 'Challenge Attempted', 'Packet Detective', '192.168.1.106'),
                            (DATEADD(hour, -6, GETUTCDATE()), 'CyberNova', 'Challenge Solved', 'Hidden in Plain Sight', '192.168.1.107'),
                            (DATEADD(hour, -3, GETUTCDATE()), 'HexHunter', 'Login', 'User HexHunter logged in', '192.168.1.108'),
                            (DATEADD(hour, -1, GETUTCDATE()), 'Participant', 'Registration', 'New user registered', '192.168.1.109');";

                    using (SqlCommand cmd = new SqlCommand(auditInsert, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating seed data: {ex.Message}");
                return false;
            }
        }
    }
}
