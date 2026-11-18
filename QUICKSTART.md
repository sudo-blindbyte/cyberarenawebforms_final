# 🚀 QUICK START - RUN NOW!

## 📋 Pre-Flight Checklist (2 minutes)

### Database Setup
- [ ] SQL Server or LocalDB installed
- [ ] Visual Studio has Server Explorer access
- If not: Open Server Explorer (Ctrl+Alt+S) → Right-click Data Connections → Create New SQL Server Database
  - Database Name: `CyberArena`
  - Server: `(LocalDb)\MSSQLLocalDB`

### Solution Build
- [ ] Open CyberArenaWebForms5.sln
- [ ] Press **Ctrl+Shift+B** to build
- [ ] Error List shows **0 errors** ✅

---

## ▶️ RUN APPLICATION

### Step 1: Start Application
```
Press F5 (or Debug → Start Debugging)
```

### Step 2: Wait for Database Initialization
- First run takes 5-10 seconds
- Tables, indexes, and demo data are created automatically
- Browser opens automatically to application

### Step 3: Login
**Option A - Administrator:**
```
Username: admin
Password: Admin@2024
Navigate to: /AdminDashboard.aspx
```

**Option B - Player:**
```
Username: player1
Password: Player@123
Navigate to: /Dashboard.aspx
```

---

## 🎯 WHAT YOU'LL SEE

### First Time Loading
1. IIS Express starts
2. Database initializes (Creates: AppUser, Challenge, Team tables)
3. Demo accounts inserted
4. Application loads

### Successful Login
✅ Dashboard loads with:
- User statistics (Points, Solved, Rank)
- Challenge list showing 12 sample challenges
- Team rankings
- Profile link

### Test Registration
1. Click "Create an Account"
2. Fill form: Full name, username, email, password
3. Click "Create account"
4. Redirects to login
5. Login with new credentials

---

## 📱 PAGES TO TEST

### Public Pages (No Login Required)
- [ ] http://localhost:****/Default.aspx (Home)
- [ ] http://localhost:****/Register.aspx (Sign up)
- [ ] http://localhost:****/Login.aspx (Sign in)

### User Pages (Login Required as Player)
- [ ] Dashboard.aspx (Overview)
- [ ] Challenges.aspx (Challenge list)
- [ ] ChallengeDetails.aspx (Challenge detail)
- [ ] Profile.aspx (Edit profile)
- [ ] Leaderboard.aspx (Rankings)
- [ ] Submissions.aspx (My submissions)

### Admin Pages (Login Required as Admin)
- [ ] AdminDashboard.aspx (Admin overview)
- [ ] AdminUsers.aspx (Manage users)
- [ ] AdminChallenges.aspx (Manage challenges)
- [ ] CreateChallenge.aspx (Create new challenge)
- [ ] AdminSubmissions.aspx (Review submissions)
- [ ] AdminIncidents.aspx (Audit log)

---

## ✅ VALIDATION TESTS

### Test 1: Required Field Validation
1. Go to Register.aspx
2. Leave "Full name" empty
3. Click "Create account"
4. **Result:** Red error message appears: "Full name is required."

### Test 2: Format Validation
1. Go to Register.aspx
2. Enter Username: "ab" (too short)
3. Click "Create account"
4. **Result:** Error: "Username must be 3–20 characters: letters, numbers or underscore."

### Test 3: Password Validation
1. Go to Register.aspx
2. Enter Password: "weak" (no uppercase, no number)
3. Click "Create account"
4. **Result:** Error: "Password needs 8+ characters with an uppercase letter, a lowercase letter and a number."

### Test 4: Duplicate Check
1. Go to Register.aspx
2. Enter Username: "admin" (already exists)
3. Move to next field
4. Click "Create account"
5. **Result:** Error: "That username is already taken."

### Test 5: Profile Update
1. Login as admin
2. Go to Profile.aspx
3. Change Full Name
4. Click "Save Changes"
5. **Result:** Profile updated message displays

---

## 🔐 DEMO ACCOUNTS (All Ready to Use)

| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | Admin@2024 | Admin | admin@cyberarena.local |
| moderator | Mod@12345 | Moderator | mod@cyberarena.local |
| player1 | Player@123 | Player | player1@cyberarena.local |
| player2 | Player@123 | Player | player2@cyberarena.local |
| NullPointer | Player@123 | Player | nullpointer@cyberarena.local |
| ByteHunter | Player@123 | Player | bytehunter@cyberarena.local |

---

## 🐛 IF SOMETHING GOES WRONG

### Error: "Database 'CyberArena' not found"
```
Solution: Create the database first
1. Open Server Explorer (Ctrl+Alt+S)
2. Right-click Data Connections
3. Select "Create New SQL Server Database"
4. Name: CyberArena, Server: (LocalDb)\MSSQLLocalDB
5. Click OK
```

### Error: "Connection string not found"
```
Solution: Check Web.config has connectionStrings section
Look for: <add name="CyberArenaDb" connectionString="..." />
```

### Error: "Build failed with 5+ errors"
```
Solution: Make sure all fixes were applied
1. Check Register.aspx line 47-48 (cvUsername, cvEmail fixed)
2. Check Profile.aspx line 105, 125 (cvUsername, cvEmail fixed)
3. Check Models/AppUser.cs has FindByUsername, FindByEmail methods
4. Check Web.config has appSettings section with ValidationSettings
5. Rebuild (Ctrl+Shift+B)
```

### Login page loads but button does nothing
```
Solution: Check validator errors
1. Go to Register.aspx instead
2. Fill form completely
3. Check for red error messages
4. Ensure all validators pass before login
```

---

## 📊 DATABASE AUTO-CREATION

On first run, these are automatically created:

**Tables:**
- AppUser (10 demo accounts)
- Challenge (12 sample challenges)
- Team (6 demo teams)
- TeamMember (team memberships)
- Submission (challenge submissions)
- AuditLog (action history)

**Indexes:** 15 performance indexes on key columns

**Data:** All demo accounts, teams, and challenges fully populated

---

## 🎓 HOW THE VALIDATION WORKS

### Example: Register Form Validation Sequence

**Step 1:** User enters data and clicks "Create account"
```
Form Data:
  Full Name: "Jane Doe"
  Username: "jane_doe"
  Email: "jane@example.com"
  Password: "SecurePass123"
  Confirm: "SecurePass123"
```

**Step 2:** ASP.NET Validators Check
```
✅ Full Name field not empty (RequiredFieldValidator)
✅ Full Name is 2-60 characters (RegularExpressionValidator)
✅ Username field not empty (RequiredFieldValidator)
✅ Username matches pattern ^[A-Za-z0-9_]{3,20}$ (RegularExpressionValidator)
✅ Email field not empty (RequiredFieldValidator)
✅ Email matches ^[^@\s]+@[^@\s]+\.[^@\s]+$ (RegularExpressionValidator)
✅ Password field not empty (RequiredFieldValidator)
✅ Password matches ^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$ (RegularExpressionValidator)
✅ Confirm Password matches Password (CompareValidator)
```

**Step 3:** If All Pass → CustomValidator Callbacks
```
Database Check 1: Is "jane_doe" already taken?
  Query: SELECT COUNT(*) FROM dbo.AppUser WHERE LOWER(Username) = LOWER('jane_doe')
  Result: 0 (not taken) ✅

Database Check 2: Is "jane@example.com" already registered?
  Query: SELECT COUNT(*) FROM dbo.AppUser WHERE LOWER(Email) = LOWER('jane@example.com')
  Result: 0 (not taken) ✅
```

**Step 4:** If All CustomValidators Pass → Process Registration
```
1. Generate salt (16 random bytes)
2. Hash password with PBKDF2-SHA256 (20,000 iterations)
3. INSERT INTO dbo.AppUser (...)
4. Get new UserId from SCOPE_IDENTITY()
5. Log to AuditLog
6. Redirect to Login.aspx with success message
```

---

## ✨ FEATURES YOU CAN TEST

- **Registration** - Create account with validation
- **Login** - Sign in with rate limiting after 5 failures
- **Profile** - Update personal information
- **Challenges** - View CTF challenges and details
- **Submissions** - Submit flags for challenges
- **Leaderboard** - View global rankings
- **Teams** - Join teams and collaborate
- **Admin Panel** - Manage users and challenges
- **Audit Logging** - View all actions taken

---

## 📞 NEED HELP?

### Build Errors
→ See &quot;ERROR_FIX_SUMMARY.md&quot;

### Database Issues
→ See &quot;DEPLOYMENT_GUIDE.md&quot; → Step 2

### Validation Issues
→ See &quot;VALIDATION_QUICK_START.md&quot;

### Detailed Analysis
→ See &quot;VALIDATION_AUDIT_COMPLETE.md&quot;

---

## 🎯 SUCCESS CHECKLIST

When running successfully, you should see:

- [x] No compilation errors (Ctrl+Shift+B)
- [x] IIS Express starts (F5)
- [x] Database creates automatically
- [x] Login page loads
- [x] Admin login works
- [x] Dashboard displays
- [x] Create account works
- [x] Validators show red errors
- [x] Admin can create challenges
- [x] Leaderboard displays rankings

---

## 🚀 YOU'RE READY!

```
1. Ctrl+Shift+B      (Build)
↓
2. F5               (Run)
↓
3. Wait 5 seconds   (Database initializes)
↓
4. Login with:
   Username: admin
   Password: Admin@2024
↓
5. Explore!
```

**Enjoy your CyberArena CTF platform!** 🎉

---

**Last Updated:** 2024  
**Status:** ✅ READY TO RUN
