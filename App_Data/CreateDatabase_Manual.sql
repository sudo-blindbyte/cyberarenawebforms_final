-- ============================================================================
-- CyberArena Database Schema - Manual Creation Script
-- For use with SQL Server Management Studio or SQL Server Object Explorer
-- ============================================================================
-- 
-- HOW TO USE THIS SCRIPT:
-- 1. Open SQL Server Management Studio or Object Explorer in Visual Studio
-- 2. Connect to: (LocalDb)\MSSQLLocalDB
-- 3. Create a new query
-- 4. Copy and paste this entire script
-- 5. Press F5 to execute
--
-- ============================================================================

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CyberArena')
BEGIN
	CREATE DATABASE CyberArena;
END
GO

-- Use the CyberArena database
USE CyberArena;
GO

-- ============================================================================
-- Drop existing tables (if re-running this script)
-- ============================================================================
IF OBJECT_ID('dbo.AuditLog', 'U') IS NOT NULL DROP TABLE dbo.AuditLog;
IF OBJECT_ID('dbo.Submission', 'U') IS NOT NULL DROP TABLE dbo.Submission;
IF OBJECT_ID('dbo.Challenge', 'U') IS NOT NULL DROP TABLE dbo.Challenge;
IF OBJECT_ID('dbo.AppUser', 'U') IS NOT NULL DROP TABLE dbo.AppUser;

-- ============================================================================
-- Table 1: AppUser - Stores user account information
-- ============================================================================
CREATE TABLE dbo.AppUser (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Username NVARCHAR(50) NOT NULL UNIQUE,
	FullName NVARCHAR(255),
	Email NVARCHAR(255) NOT NULL UNIQUE,
	Role NVARCHAR(50) NOT NULL DEFAULT 'Player',
	Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
	Team NVARCHAR(255),
	Points INT NOT NULL DEFAULT 0,
	Solved INT NOT NULL DEFAULT 0,
	Joined DATETIME NOT NULL DEFAULT GETUTCDATE(),
	LastActive DATETIME NOT NULL DEFAULT GETUTCDATE(),
	Salt NVARCHAR(MAX) NOT NULL,
	Hash NVARCHAR(MAX) NOT NULL,
	CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- Create indexes for AppUser
CREATE INDEX IX_AppUser_Username ON dbo.AppUser(Username);
CREATE INDEX IX_AppUser_Email ON dbo.AppUser(Email);
CREATE INDEX IX_AppUser_Status ON dbo.AppUser(Status);

PRINT 'Table AppUser created successfully!';

-- ============================================================================
-- Table 2: Challenge - Stores challenge definitions
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

-- Create indexes for Challenge
CREATE INDEX IX_Challenge_Category ON dbo.Challenge(Category);
CREATE INDEX IX_Challenge_Difficulty ON dbo.Challenge(Difficulty);
CREATE INDEX IX_Challenge_IsPublished ON dbo.Challenge(IsPublished);

PRINT 'Table Challenge created successfully!';

-- ============================================================================
-- Table 3: Submission - Stores challenge submission attempts
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

-- Create indexes for Submission
CREATE INDEX IX_Submission_UserId ON dbo.Submission(UserId);
CREATE INDEX IX_Submission_ChallengeId ON dbo.Submission(ChallengeId);
CREATE INDEX IX_Submission_IsCorrect ON dbo.Submission(IsCorrect);

PRINT 'Table Submission created successfully!';

-- ============================================================================
-- Table 4: AuditLog - Stores activity tracking
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

-- Create indexes for AuditLog
CREATE INDEX IX_AuditLog_Time ON dbo.AuditLog(Time);
CREATE INDEX IX_AuditLog_Actor ON dbo.AuditLog(Actor);

PRINT 'Table AuditLog created successfully!';

-- ============================================================================
-- Insert Sample Data
-- ============================================================================

-- Insert sample users
INSERT INTO dbo.AppUser (Username, FullName, Email, Role, Status, Team, Points, Solved, Salt, Hash)
VALUES 
	('admin', 'Administrator', 'admin@cyberarena.local', 'Admin', 'Active', NULL, 0, 0, 'salt1', 'hash1'),
	('moderator', 'Moderator User', 'mod@cyberarena.local', 'Moderator', 'Active', NULL, 0, 0, 'salt2', 'hash2'),
	('player1', 'Player One', 'player1@cyberarena.local', 'Player', 'Active', 'Team Alpha', 150, 5, 'salt3', 'hash3');

PRINT '3 sample users inserted!';

-- Insert sample challenges
INSERT INTO dbo.Challenge (Title, Subtitle, Category, Difficulty, Solves, Points, SuccessRate, Description, Flag, IsPublished)
VALUES 
	('Reverse Me', 'Find the hidden flag in the binary', 'Reverse Engineering', 'Medium', 42, 250, 85.5, 
	 'A simple binary file with an embedded flag. Use a decompiler or debugger to find it.', 'FLAG{reverse_me}', 1),
	('SQL Injection 101', 'Extract the secret from the database', 'Web Security', 'Easy', 128, 100, 95.0, 
	 'A vulnerable login form. Try to bypass authentication using SQL injection.', 'FLAG{sql_injection}', 1),
	('Crypto Challenge', 'Decrypt the message', 'Cryptography', 'Hard', 12, 500, 30.0, 
	 'A message encrypted with a custom cipher. Can you crack it?', 'FLAG{crypto_challenge}', 1);

PRINT '3 sample challenges inserted!';

-- Insert sample audit log entries
INSERT INTO dbo.AuditLog (Actor, Action, Target, Ip)
VALUES 
	('system', 'Platform started', 'CyberArena v5', '127.0.0.1'),
	('admin', 'Published challenge', 'Reverse Me', '10.0.0.4'),
	('moderator', 'Resolved incident', 'IR-2038', '10.0.0.9');

PRINT '3 audit log entries inserted!';

-- ============================================================================
-- Verification Query
-- ============================================================================
PRINT '';
PRINT '============= DATABASE SETUP COMPLETE =============';
PRINT '';
PRINT 'Tables Created:';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo';
PRINT '';
PRINT 'Users:';
SELECT Id, Username, Role, Status FROM dbo.AppUser;
PRINT '';
PRINT 'Challenges:';
SELECT Id, Title, Category, Difficulty, Points FROM dbo.Challenge;
PRINT '';
PRINT '====================================================';
