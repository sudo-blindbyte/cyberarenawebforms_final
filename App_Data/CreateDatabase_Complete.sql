-- ============================================================================
-- CyberArena - Complete Database Schema and Seed Data
-- SQL Server / LocalDB
-- ============================================================================
-- This script creates ALL tables with indexes and loads complete seed data
-- for immediate use in Server Explorer. Run this to initialize the database.
-- ============================================================================

-- Drop existing tables (CAUTION: This deletes all data!)
-- Uncomment if you want to recreate from scratch
/*
DROP TABLE IF EXISTS dbo.AuditLog;
DROP TABLE IF EXISTS dbo.Submission;
DROP TABLE IF EXISTS dbo.TeamMember;
DROP TABLE IF EXISTS dbo.Challenge;
DROP TABLE IF EXISTS dbo.AppUser;
DROP TABLE IF EXISTS dbo.Team;
*/

-- ============================================================================
-- TABLE 1: Team - Team management and standings
-- ============================================================================
CREATE TABLE dbo.Team (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(100) NOT NULL UNIQUE,
	Description NVARCHAR(MAX),
	MaxMembers INT DEFAULT 5,
	CurrentMembers INT DEFAULT 0,
	TotalPoints INT DEFAULT 0,
	Status NVARCHAR(50) DEFAULT 'Open',  -- Open or Full
	CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Team_Name ON dbo.Team(Name);
CREATE INDEX IX_Team_Status ON dbo.Team(Status);

-- ============================================================================
-- TABLE 2: AppUser - User accounts with points and rankings
-- ============================================================================
CREATE TABLE dbo.AppUser (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Username NVARCHAR(50) NOT NULL UNIQUE,
	FullName NVARCHAR(255),
	Email NVARCHAR(255) NOT NULL UNIQUE,
	Role NVARCHAR(50) NOT NULL DEFAULT 'Player',  -- Admin, Moderator, Player
	Status NVARCHAR(50) NOT NULL DEFAULT 'Active',  -- Active, Suspended
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
);

CREATE INDEX IX_AppUser_Username ON dbo.AppUser(Username);
CREATE INDEX IX_AppUser_Email ON dbo.AppUser(Email);
CREATE INDEX IX_AppUser_Status ON dbo.AppUser(Status);
CREATE INDEX IX_AppUser_Points ON dbo.AppUser(Points) DESC;
CREATE INDEX IX_AppUser_Rank ON dbo.AppUser(Rank);
CREATE INDEX IX_AppUser_TeamId ON dbo.AppUser(TeamId);

-- ============================================================================
-- TABLE 3: Challenge - CTF challenges
-- ============================================================================
CREATE TABLE dbo.Challenge (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Title NVARCHAR(255) NOT NULL,
	Subtitle NVARCHAR(255),
	Category NVARCHAR(100) NOT NULL,
	Difficulty NVARCHAR(50) NOT NULL,  -- Easy, Medium, Hard
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
);

CREATE INDEX IX_Challenge_Category ON dbo.Challenge(Category);
CREATE INDEX IX_Challenge_Difficulty ON dbo.Challenge(Difficulty);
CREATE INDEX IX_Challenge_IsPublished ON dbo.Challenge(IsPublished);

-- ============================================================================
-- TABLE 4: Submission - Challenge attempt tracking
-- ============================================================================
CREATE TABLE dbo.Submission (
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL,
	ChallengeId INT NOT NULL,
	Flag NVARCHAR(MAX) NOT NULL,
	IsCorrect BIT NOT NULL DEFAULT 0,
	SubmittedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
	FOREIGN KEY (UserId) REFERENCES dbo.AppUser(Id) ON DELETE CASCADE,
	FOREIGN KEY (ChallengeId) REFERENCES dbo.Challenge(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Submission_UserId ON dbo.Submission(UserId);
CREATE INDEX IX_Submission_ChallengeId ON dbo.Submission(ChallengeId);
CREATE INDEX IX_Submission_IsCorrect ON dbo.Submission(IsCorrect);

-- ============================================================================
-- TABLE 5: TeamMember - Team membership association
-- ============================================================================
CREATE TABLE dbo.TeamMember (
	Id INT PRIMARY KEY IDENTITY(1,1),
	TeamId INT NOT NULL,
	UserId INT NOT NULL,
	JoinedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
	FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id) ON DELETE CASCADE,
	FOREIGN KEY (UserId) REFERENCES dbo.AppUser(Id) ON DELETE CASCADE,
	UNIQUE (TeamId, UserId)
);

CREATE INDEX IX_TeamMember_TeamId ON dbo.TeamMember(TeamId);
CREATE INDEX IX_TeamMember_UserId ON dbo.TeamMember(UserId);

-- ============================================================================
-- TABLE 6: AuditLog - Activity logging and audit trail
-- ============================================================================
CREATE TABLE dbo.AuditLog (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Time DATETIME NOT NULL DEFAULT GETUTCDATE(),
	Actor NVARCHAR(255) NOT NULL,
	Action NVARCHAR(255) NOT NULL,
	Target NVARCHAR(MAX),
	Ip NVARCHAR(45),
	CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE INDEX IX_AuditLog_Time ON dbo.AuditLog(Time);
CREATE INDEX IX_AuditLog_Actor ON dbo.AuditLog(Actor);

-- ============================================================================
-- SEED DATA - Teams (6 teams)
-- ============================================================================
INSERT INTO dbo.Team (Name, Description, MaxMembers, CurrentMembers, TotalPoints, Status)
VALUES 
	('RootAccess', 'Security researchers focused on web exploitation and forensics.', 5, 4, 3300, 'Open'),
	('ZeroDay', 'A collaborative team solving challenging security puzzles.', 5, 3, 2725, 'Open'),
	('BinaryOps', 'Reverse engineering, binary analysis and low-level security.', 5, 5, 2175, 'Full'),
	('NullCrew', 'Learning cybersecurity through hands-on CTF challenges.', 5, 2, 1420, 'Open'),
	('PacketStorm', 'Network analysis, traffic investigation and protocol research.', 5, 5, 1200, 'Full'),
	('CipherLab', 'Cryptography enthusiasts working through complex ciphers.', 5, 1, 650, 'Open');

-- ============================================================================
-- SEED DATA - Users (10 users with rankings and team assignments)
-- ============================================================================
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
	('Participant', 'Participant User', 'participant@cyberarena.local', 'Player', 'Active', NULL, 450, 8, 8, 'salt10', 'hash10');

-- ============================================================================
-- SEED DATA - Team Members (7 team memberships)
-- ============================================================================
INSERT INTO dbo.TeamMember (TeamId, UserId)
VALUES 
	(1, 3),  -- NullPointer in RootAccess
	(1, 5),  -- PacketGhost in RootAccess
	(2, 4),  -- ByteHunter in ZeroDay
	(2, 7),  -- TraceRoute in ZeroDay
	(3, 6),  -- CipherX in BinaryOps
	(3, 8),  -- CyberNova in BinaryOps
	(4, 9);  -- HexHunter in NullCrew

-- ============================================================================
-- SEED DATA - Challenges (12 challenges - 5 Easy, 5 Medium, 2 Hard)
-- ============================================================================
INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Solves, Points, SuccessRate, Description, Flag, IsPublished)
VALUES 
	-- EASY (5 challenges)
	('Hidden Message', 'Inspect a suspicious file', 'Digital Forensics', 'Easy', 1420, 50, 78.4, 
	 'Inspect a suspicious file to discover information hidden inside it. Metadata and file structure may reveal more than the visible content.', 
	 'CTF{hidden_in_metadata}', 1),

	('Caesar''s Secret', 'Classic substitution cipher', 'Cryptography', 'Easy', 1310, 75, 72.1, 
	 'Decode a message encrypted using a classic substitution technique and recover the flag.', 
	 'CTF{rome_was_not_built_in_a_day}', 1),

	('SQL Injection Basics', 'Break a search form', 'Web Security', 'Easy', 1105, 100, 66.0, 
	 'A product search form builds its query from raw input. Extract data that should not be visible.', 
	 'CTF{union_select_wins}', 1),

	('Hidden in Plain Sight', 'Stego warm-up', 'Steganography', 'Easy', 980, 75, 61.7, 
	 'A harmless-looking image carries a secret. Find it.', 
	 'CTF{appended_data}', 1),

	('Basic Network Scan', 'Enumerate open services', 'Networking', 'Easy', 763, 50, 59.3, 
	 'Use standard tools to discover services running on a target system.', 
	 'CTF{nmap_is_your_friend}', 1),

	-- MEDIUM (5 challenges)
	('Broken Login', 'Vulnerable practice login', 'Web Security', 'Medium', 842, 100, 41.6, 
	 'Investigate a deliberately vulnerable practice login application and find a way to access the admin area.', 
	 'CTF{login_bypassed}', 1),

	('Trace the Domain', 'Investigate a fictional domain', 'OSINT', 'Medium', 655, 125, 38.2, 
	 'Use public information to investigate a fictional domain and identify who is behind it.', 
	 'CTF{whois_who_is_it}', 1),

	('Packet Detective', 'Analyse a network capture', 'Networking', 'Medium', 590, 100, 35.9, 
	 'Examine a sample network capture and identify useful evidence hidden in the traffic.', 
	 'CTF{follow_the_stream}', 1),

	('Reverse Me', 'Find the key in the binary', 'Reverse Engineering', 'Medium', 428, 100, 38.5, 
	 'Analyze a binary to recover the hidden key inside the Linux executable. The flag is checked against a derived value, not a static string.', 
	 'CTF{r3v3rs3_m3_pl3as3}', 1),

	('API Abuse', 'Misused endpoints', 'Web Security', 'Medium', 421, 125, 37.8, 
	 'An API has endpoints that expose more data than intended. Exploit the misconfiguration.', 
	 'CTF{api_says_oops}', 1),

	-- HARD (2 challenges)
	('Pixel Clue', 'Concealed message in an image', 'Steganography', 'Hard', 312, 150, 21.4, 
	 'Explore a training image to locate a concealed message. The picture looks ordinary, but the pixels disagree.', 
	 'CTF{lsb_never_lies}', 1),

	('Buffer Overflow 101', 'Classic memory corruption', 'Binary Exploitation', 'Hard', 285, 200, 18.5, 
	 'Overflow a buffer to overwrite return addresses and execute arbitrary code.', 
	 'CTF{segmentation_fault}', 1);

-- ============================================================================
-- SEED DATA - Audit Log (6 activity entries)
-- ============================================================================
INSERT INTO dbo.AuditLog (Actor, Action, Target, Ip)
VALUES 
	('system', 'Platform started', 'CyberArena v5', '127.0.0.1'),
	('admin', 'Published challenge', 'Hidden Message', '10.0.0.4'),
	('admin', 'Published challenge', 'Caesar''s Secret', '10.0.0.4'),
	('moderator', 'Updated leaderboard', 'Season 01', '10.0.0.9'),
	('system', 'Teams created', '6 teams initialized', '127.0.0.1'),
	('system', 'Database initialized', 'All tables and data loaded', '127.0.0.1');

-- ============================================================================
-- VERIFICATION QUERIES - Use these to verify data was inserted correctly
-- ============================================================================

-- Check Teams
SELECT 'Teams' as [Table], COUNT(*) as [Count] FROM dbo.Team;

-- Check Users
SELECT 'Users' as [Table], COUNT(*) as [Count] FROM dbo.AppUser;

-- Check Challenges
SELECT 'Challenges' as [Table], COUNT(*) as [Count] FROM dbo.Challenge;

-- Check Submissions
SELECT 'Submissions' as [Table], COUNT(*) as [Count] FROM dbo.Submission;

-- Check Team Members
SELECT 'Team Members' as [Table], COUNT(*) as [Count] FROM dbo.TeamMember;

-- Check Audit Logs
SELECT 'Audit Logs' as [Table], COUNT(*) as [Count] FROM dbo.AuditLog;

-- Summary: All 6 tables with data counts
SELECT 'TEAM' as [Table], COUNT(*) as [Rows] FROM dbo.Team
UNION ALL
SELECT 'APPUSER', COUNT(*) FROM dbo.AppUser
UNION ALL
SELECT 'CHALLENGE', COUNT(*) FROM dbo.Challenge
UNION ALL
SELECT 'SUBMISSION', COUNT(*) FROM dbo.Submission
UNION ALL
SELECT 'TEAMMEMBER', COUNT(*) FROM dbo.TeamMember
UNION ALL
SELECT 'AUDITLOG', COUNT(*) FROM dbo.AuditLog;

-- ============================================================================
-- END OF SCRIPT
-- ============================================================================
