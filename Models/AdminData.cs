using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CyberArenaWebForms5.Models
{
    // ======================= Audit log =======================
    public class AuditEntry
    {
        public DateTime Time { get; set; }
        public string Actor { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public string Ip { get; set; }
        public string TimeText { get { return Time.ToString("dd MMM, HH:mm:ss"); } }
    }

    public static class AuditLog
    {
        private static readonly object Sync = new object();
        private static readonly List<AuditEntry> Entries = new List<AuditEntry>();

        static AuditLog()
        {
            Add("system", "Platform started", "CyberArena v5", "127.0.0.1", 180);
            Add("admin", "Published challenge", "Reverse Me", "10.0.0.4", 95);
            Add("moderator", "Resolved incident", "IR-2038", "10.0.0.9", 40);
        }

        private static void Add(string actor, string action, string target, string ip, int minutesAgo)
        {
            Entries.Add(new AuditEntry { Time = DateTime.UtcNow.AddMinutes(-minutesAgo), Actor = actor, Action = action, Target = target, Ip = ip });
        }

        public static void Write(string actor, string action, string target, HttpContext context)
        {
            string ip = "unknown";
            if (context != null && context.Request != null) ip = context.Request.UserHostAddress ?? "unknown";
            lock (Sync)
            {
                Entries.Add(new AuditEntry { Time = DateTime.UtcNow, Actor = actor, Action = action, Target = target, Ip = ip });
                if (Entries.Count > 500) Entries.RemoveAt(0);
            }
        }

        public static List<AuditEntry> Recent(int count)
        {
            lock (Sync) { return Entries.OrderByDescending(e => e.Time).Take(count).ToList(); }
        }

        public static string ToCsv()
        {
            var sb = new StringBuilder("Time (UTC),Actor,Action,Target,IP\r\n");
            List<AuditEntry> all;
            lock (Sync) { all = Entries.OrderBy(e => e.Time).ToList(); }
            foreach (var e in all)
            {
                sb.Append(Csv(e.Time.ToString("yyyy-MM-dd HH:mm:ss"))).Append(',')
                  .Append(Csv(e.Actor)).Append(',')
                  .Append(Csv(e.Action)).Append(',')
                  .Append(Csv(e.Target)).Append(',')
                  .Append(Csv(e.Ip)).Append("\r\n");
            }
            return sb.ToString();
        }

        private static string Csv(string value)
        {
            value = value ?? string.Empty;
            // Neutralise spreadsheet formula injection.
            if (value.Length > 0 && "=+-@".IndexOf(value[0]) >= 0) value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }

    // ======================= Incidents =======================
    public class Incident
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Reporter { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public string Severity { get; set; }   // Low | Medium | High | Critical
        public string Status { get; set; }     // Open | Investigating | Resolved

        public string DateText { get { return Date.ToString("dd MMM yyyy"); } }
        public string SeverityCss
        {
            get
            {
                switch (Severity)
                {
                    case "Critical": return "red";
                    case "High": return "orange";
                    case "Medium": return "purple";
                    default: return "grey";
                }
            }
        }
        public string StatusCss { get { return Status == "Resolved" ? "green" : (Status == "Investigating" ? "orange" : "red"); } }
    }

    public static class IncidentStore
    {
        private static readonly object Sync = new object();
        private static readonly List<Incident> Items = new List<Incident>
        {
            new Incident { Id = "IR-2041", Date = DateTime.UtcNow.AddHours(-3),  Reporter = "PacketGhost", Subject = "Suspicious flag sharing", Details = "Identical wrong flags submitted from two accounts within 10 seconds.", Severity = "High", Status = "Open" },
            new Incident { Id = "IR-2040", Date = DateTime.UtcNow.AddHours(-9),  Reporter = "system",      Subject = "Brute-force attempts", Details = "180 failed submissions on Broken Login from a single IP.", Severity = "Critical", Status = "Investigating" },
            new Incident { Id = "IR-2039", Date = DateTime.UtcNow.AddDays(-1),   Reporter = "HexHunter",   Subject = "Challenge file broken", Details = "reverse_me.zip fails to extract on Windows.", Severity = "Low", Status = "Open" },
            new Incident { Id = "IR-2038", Date = DateTime.UtcNow.AddDays(-2),   Reporter = "CipherQueen", Subject = "Rule violation", Details = "Public write-up posted during the live event.", Severity = "Medium", Status = "Resolved" },
            new Incident { Id = "IR-2037", Date = DateTime.UtcNow.AddDays(-3),   Reporter = "system",      Subject = "Unusual login location", Details = "Admin sign-in from a new network.", Severity = "Medium", Status = "Resolved" }
        };

        public static List<Incident> All()
        {
            lock (Sync) { return Items.OrderByDescending(i => i.Date).ToList(); }
        }

        public static bool SetStatus(string id, string status)
        {
            lock (Sync)
            {
                var item = Items.FirstOrDefault(i => i.Id == id);
                if (item == null) return false;
                item.Status = status;
                return true;
            }
        }
    }

    // ======================= Submissions =======================
    public class SubmissionRecord
    {
        public string Id { get; set; }
        public string User { get; set; }
        public int ChallengeId { get; set; }
        public string Challenge { get; set; }
        public string Status { get; set; }      // Accepted | Rejected | Verified | Revoked
        public DateTime SubmittedAt { get; set; }
        public int Points { get; set; }
        public int Attempt { get; set; }
        public string SubmittedFlag { get; set; }
        public string Ip { get; set; }
        public string Notes { get; set; }

        public string SubmittedText { get { return SubmittedAt.ToString("dd MMM yyyy, HH:mm"); } }
        public string StatusCss
        {
            get
            {
                switch (Status)
                {
                    case "Accepted": return "green";
                    case "Verified": return "blue";
                    case "Revoked": return "red";
                    default: return "orange";
                }
            }
        }
        public string PointsText { get { return Points > 0 ? "+" + Points : "0"; } }

        /// <summary>Shows only the start of the flag so admins never see full secrets in lists.</summary>
        public string MaskedFlag
        {
            get
            {
                if (string.IsNullOrEmpty(SubmittedFlag)) return "(empty)";
                int keep = Math.Min(8, SubmittedFlag.Length);
                return SubmittedFlag.Substring(0, keep) + new string('•', Math.Max(4, SubmittedFlag.Length - keep));
            }
        }
    }

    public static class SubmissionStore
    {
        private static readonly object Sync = new object();
        private static readonly List<SubmissionRecord> Items = new List<SubmissionRecord>
        {
            new SubmissionRecord { Id = "SUB-1021", User = "NullPointer -", ChallengeId = 7, Challenge = " Reverse Me", Status = "Accepted", SubmittedAt = DateTime.UtcNow.AddMinutes(-18), Points = 100, Attempt = 2, SubmittedFlag = "CTF{r3v3rs3_m3_pl3as3}", Ip = "203.0.113.24", Notes = "" },
            new SubmissionRecord { Id = "SUB-1020", User = "BotNet99", ChallengeId = 3, Challenge = "Broken Login", Status = "Rejected", SubmittedAt = DateTime.UtcNow.AddMinutes(-41), Points = 0, Attempt = 87, SubmittedFlag = "CTF{guess_87}", Ip = "198.51.100.77", Notes = "Automated guessing pattern." },
            new SubmissionRecord { Id = "SUB-1019", User = "CipherQueen", ChallengeId = 2, Challenge = "Caesar's Secret", Status = "Verified", SubmittedAt = DateTime.UtcNow.AddHours(-2), Points = 75, Attempt = 1, SubmittedFlag = "CTF{rome_was_not_built_in_a_day}", Ip = "192.0.2.41", Notes = "Checked manually." },
            new SubmissionRecord { Id = "SUB-1018", User = "PacketGhost", ChallengeId = 5, Challenge = "Packet Detective", Status = "Accepted", SubmittedAt = DateTime.UtcNow.AddHours(-5), Points = 100, Attempt = 3, SubmittedFlag = "CTF{follow_the_stream}", Ip = "203.0.113.90", Notes = "" },
            new SubmissionRecord { Id = "SUB-1017", User = "HexHunter", ChallengeId = 6, Challenge = "Pixel Clue", Status = "Rejected", SubmittedAt = DateTime.UtcNow.AddHours(-7), Points = 0, Attempt = 4, SubmittedFlag = "CTF{not_the_flag}", Ip = "192.0.2.12", Notes = "" },
            new SubmissionRecord { Id = "SUB-1016", User = "player", ChallengeId = 1, Challenge = "Hidden Message", Status = "Accepted", SubmittedAt = DateTime.UtcNow.AddDays(-1), Points = 50, Attempt = 1, SubmittedFlag = "CTF{hidden_in_metadata}", Ip = "192.0.2.200", Notes = "" },
            new SubmissionRecord { Id = "SUB-1015", User = "NullPointer", ChallengeId = 12, Challenge = "Vault Cracker", Status = "Revoked", SubmittedAt = DateTime.UtcNow.AddDays(-1).AddHours(-3), Points = 0, Attempt = 1, SubmittedFlag = "CTF{small_primes_are_weak}", Ip = "203.0.113.24", Notes = "Points revoked: flag leaked in public channel." }
        };

        public static List<SubmissionRecord> All()
        {
            lock (Sync) { return Items.OrderByDescending(s => s.SubmittedAt).ToList(); }
        }

        public static SubmissionRecord Find(string id)
        {
            lock (Sync) { return Items.FirstOrDefault(s => s.Id == id); }
        }

        public static bool Update(string id, string status, string notes)
        {
            lock (Sync)
            {
                var item = Items.FirstOrDefault(s => s.Id == id);
                if (item == null) return false;
                item.Status = status;
                item.Notes = notes;
                if (status == "Revoked") item.Points = 0;
                return true;
            }
        }

        public static bool SaveNotes(string id, string notes)
        {
            lock (Sync)
            {
                var item = Items.FirstOrDefault(s => s.Id == id);
                if (item == null) return false;
                item.Notes = notes;
                return true;
            }
        }
    }
}
