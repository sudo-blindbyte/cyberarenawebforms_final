-- ============================================================================
-- CyberArena - Complete Database Schema and Seed Data
-- SQL Server / LocalDB
-- ============================================================================

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
	Status NVARCHAR(50) DEFAULT 'Open',
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
);

CREATE INDEX IX_AppUser_Username ON dbo.AppUser(Username);
CREATE INDEX IX_AppUser_Email ON dbo.AppUser(Email);
CREATE INDEX IX_AppUser_Status ON dbo.AppUser(Status);
CREATE INDEX IX_AppUser_Points ON dbo.AppUser(Points DESC);
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
-- SEED DATA - Users (10 users)
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
	(1, 3),
	(1, 5),
	(2, 4),
	(2, 7),
	(3, 6),
	(3, 8),
	(4, 9);

-- ============================================================================
-- SEED DATA - Challenges (12 challenges)
-- ============================================================================
INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Solves, Points, SuccessRate, Description, Flag, IsPublished)
VALUES 
	('Hidden Message', 'Inspect a suspicious file', 'Digital Forensics', 'Easy', 1420, 50, 78.4, 'Inspect a suspicious file to discover information hidden inside it.', 'CTF{hidden_in_metadata}', 1),
	('Caesar Secret', 'Classic substitution cipher', 'Cryptography', 'Easy', 1310, 75, 72.1, 'Decode a message encrypted using a classic substitution technique.', 'CTF{rome_was_not_built_in_a_day}', 1),
	('SQL Injection Basics', 'Break a search form', 'Web Security', 'Easy', 1105, 100, 66.0, 'A product search form builds its query from raw input.', 'CTF{union_select_wins}', 1),
	('Hidden in Plain Sight', 'Stego warm-up', 'Steganography', 'Easy', 980, 75, 61.7, 'A harmless-looking image carries a secret.', 'CTF{appended_data}', 1),
	('Basic Network Scan', 'Enumerate open services', 'Networking', 'Easy', 763, 50, 59.3, 'Use standard tools to discover services running on a target system.', 'CTF{nmap_is_your_friend}', 1),
	('Broken Login', 'Vulnerable practice login', 'Web Security', 'Medium', 842, 100, 41.6, 'Investigate a deliberately vulnerable practice login application.', 'CTF{login_bypassed}', 1),
	('Trace the Domain', 'Investigate a fictional domain', 'OSINT', 'Medium', 655, 125, 38.2, 'Use public information to investigate a fictional domain.', 'CTF{whois_who_is_it}', 1),
	('Packet Detective', 'Analyse a network capture', 'Networking', 'Medium', 590, 100, 35.9, 'Examine a sample network capture and identify useful evidence.', 'CTF{follow_the_stream}', 1),
	('Reverse Me', 'Find the key in the binary', 'Reverse Engineering', 'Medium', 428, 100, 38.5, 'Analyze a binary to recover the hidden key.', 'CTF{assembly_is_beautiful}', 1),
	('Forgotten Password', 'Recover admin credentials', 'Forensics', 'Medium', 365, 125, 36.9, 'Discover how to reset admin credentials from evidence in the system.', 'CTF{recovery_complete}', 1),
	('Zero Escape', 'Exploit a kernel vuln', 'Exploitation', 'Hard', 128, 200, 18.3, 'A custom kernel module has critical vulnerabilities. Escalate privileges.', 'CTF{kernel_is_king}', 1),
	('The Final Boss', 'Multi-stage challenge', 'Misc', 'Hard', 87, 250, 12.5, 'A challenge combining multiple techniques and concepts from the competition.', 'CTF{champion_crowned}', 1);

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================
SELECT 'Teams' as TableName, COUNT(*) as RecordCount FROM dbo.Team
UNION ALL
SELECT 'Users', COUNT(*) FROM dbo.AppUser
UNION ALL
SELECT 'Challenges', COUNT(*) FROM dbo.Challenge
UNION ALL
SELECT 'TeamMembers', COUNT(*) FROM dbo.TeamMember
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM dbo.AuditLog;
