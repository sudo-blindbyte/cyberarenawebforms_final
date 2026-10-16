using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberArenaWebForms5.Models
{
    /// <summary>
    /// Sample challenge data used by the UI. Replace the static list with
    /// SQL Server queries when the database layer is added.
    /// </summary>
    public class Challenge
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }   // Easy | Medium | Hard
        public string Status { get; set; }       // Solved | In Progress | Unsolved
        public int Solves { get; set; }
        public int Points { get; set; }
        public double SuccessRate { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Attachment { get; set; }
        public string Flag { get; set; }
        public bool IsPublished { get; set; } = true;
        public string StateText { get { return IsPublished ? "Published" : "Draft"; } }
        public string StateCss { get { return IsPublished ? "blue" : "grey"; } }

        // ---------- presentation helpers (used by data binding) ----------

        public string PointsText { get { return Points + " pts"; } }
        public string SolvesText { get { return Solves.ToString("N0") + " solves"; } }
        public string SuccessRateText { get { return SuccessRate.ToString("0.0") + "%"; } }

        public string DifficultyCss
        {
            get
            {
                switch (Difficulty)
                {
                    case "Easy": return "green";
                    case "Medium": return "orange";
                    default: return "red";
                }
            }
        }

        public string StatusCss
        {
            get
            {
                switch (Status)
                {
                    case "Solved": return "green";
                    case "In Progress": return "orange";
                    default: return "grey";
                }
            }
        }

        public string IconColorCss { get { return UiHelper.CategoryColor(Category); } }
        public string IconSvg { get { return UiHelper.CategoryIcon(Category); } }

        // ---------- sample repository ----------

        private static readonly List<Challenge> _all = new List<Challenge>
        {
            new Challenge { Id = 1, Title = "Hidden Message", Subtitle = "Inspect a suspicious file", Category = "Digital Forensics", Difficulty = "Easy", Status = "Solved", Solves = 1420, Points = 50, SuccessRate = 78.4,
                Description = "Inspect a suspicious file to discover information hidden inside it. Metadata and file structure may reveal more than the visible content.",
                Hint = "Look beyond what the file shows by default. Check its metadata and strings.", Attachment = "suspicious_file.zip", Flag = "CTF{hidden_in_metadata}" },
            new Challenge { Id = 2, Title = "Caesar's Secret", Subtitle = "Classic substitution cipher", Category = "Cryptography", Difficulty = "Easy", Status = "Solved", Solves = 1310, Points = 75, SuccessRate = 72.1,
                Description = "Decode a message encrypted using a classic substitution technique and recover the flag.",
                Hint = "There are only 25 possible shifts to try.", Attachment = "cipher.txt", Flag = "CTF{rome_was_not_built_in_a_day}" },
            new Challenge { Id = 3, Title = "Broken Login", Subtitle = "Vulnerable practice login", Category = "Web Security", Difficulty = "Medium", Status = "In Progress", Solves = 842, Points = 100, SuccessRate = 41.6,
                Description = "Investigate a deliberately vulnerable practice login application and find a way to access the admin area.",
                Hint = "Think about how user input is placed inside the login query.", Attachment = "login_source.zip", Flag = "CTF{login_bypassed}" },
            new Challenge { Id = 4, Title = "Trace the Domain", Subtitle = "Investigate a fictional domain", Category = "OSINT", Difficulty = "Medium", Status = "Unsolved", Solves = 655, Points = 125, SuccessRate = 38.2,
                Description = "Use public information to investigate a fictional domain and identify who is behind it.",
                Hint = "DNS records and registration history are public.", Attachment = "brief.pdf", Flag = "CTF{whois_who_is_it}" },
            new Challenge { Id = 5, Title = "Packet Detective", Subtitle = "Analyse a network capture", Category = "Networking", Difficulty = "Medium", Status = "Unsolved", Solves = 590, Points = 100, SuccessRate = 35.9,
                Description = "Examine a sample network capture and identify useful evidence hidden in the traffic.",
                Hint = "Follow the TCP streams that carry plain text.", Attachment = "capture.pcap", Flag = "CTF{follow_the_stream}" },
            new Challenge { Id = 6, Title = "Pixel Clue", Subtitle = "Concealed message in an image", Category = "Steganography", Difficulty = "Hard", Status = "Unsolved", Solves = 312, Points = 150, SuccessRate = 21.4,
                Description = "Explore a training image to locate a concealed message. The picture looks ordinary, but the pixels disagree.",
                Hint = "Least significant bits are a classic hiding place.", Attachment = "pixel_clue.png", Flag = "CTF{lsb_never_lies}" },
            new Challenge { Id = 7, Title = "Reverse Me", Subtitle = "Find the key in the binary", Category = "Reverse Engineering", Difficulty = "Medium", Status = "In Progress", Solves = 428, Points = 100, SuccessRate = 38.5,
                Description = "Analyze a binary to recover the hidden key inside the Linux executable. The flag is checked against a derived value, not a static string.",
                Hint = "This program checks your input against a string in the binary.", Attachment = "reverse_me.zip", Flag = "CTF{r3v3rs3_m3_pl3as3}" },
            new Challenge { Id = 8, Title = "SQL Injection Basics", Subtitle = "Break a search form", Category = "Web Security", Difficulty = "Easy", Status = "Solved", Solves = 1105, Points = 100, SuccessRate = 66.0,
                Description = "A product search form builds its query from raw input. Extract data that should not be visible.",
                Hint = "A single quote is a good first probe.", Attachment = "shop_app.zip", Flag = "CTF{union_select_wins}" },
            new Challenge { Id = 9, Title = "Hidden in Plain Sight", Subtitle = "Stego warm-up", Category = "Steganography", Difficulty = "Easy", Status = "Solved", Solves = 980, Points = 75, SuccessRate = 61.7,
                Description = "A harmless-looking image carries a secret. Find it.",
                Hint = "Check the end of the file.", Attachment = "plain_sight.jpg", Flag = "CTF{appended_data}" },
            new Challenge { Id = 10, Title = "Basic Network Scan", Subtitle = "Enumerate open services", Category = "Networking", Difficulty = "Easy", Status = "Unsolved", Solves = 870, Points = 50, SuccessRate = 69.3,
                Description = "Review the scan output and work out which service leaks the flag.",
                Hint = "Banners often reveal more than intended.", Attachment = "scan_output.txt", Flag = "CTF{banner_grabbed}" },
            new Challenge { Id = 11, Title = "File Signature Analysis", Subtitle = "Identify mislabelled files", Category = "Digital Forensics", Difficulty = "Medium", Status = "Unsolved", Solves = 540, Points = 100, SuccessRate = 44.8,
                Description = "The file extension lies. Identify the real file type and extract its content.",
                Hint = "Magic bytes live at the start of a file.", Attachment = "mystery.bin", Flag = "CTF{magic_bytes}" },
            new Challenge { Id = 12, Title = "Vault Cracker", Subtitle = "Break a weak key exchange", Category = "Cryptography", Difficulty = "Hard", Status = "Unsolved", Solves = 187, Points = 200, SuccessRate = 14.9,
                Description = "A custom key-exchange protocol reuses a small prime. Recover the shared secret.",
                Hint = "Small primes make discrete logs feasible.", Attachment = "vault.py", Flag = "CTF{small_primes_are_weak}" }
        };

        private static readonly object Sync = new object();

        /// <summary>Published challenges (what players see).</summary>
        public static List<Challenge> All()
        {
            lock (Sync) { return _all.Where(c => c.IsPublished).ToList(); }
        }

        /// <summary>Every challenge including drafts (admin only).</summary>
        public static List<Challenge> AllAdmin()
        {
            lock (Sync) { return _all.OrderBy(c => c.Id).ToList(); }
        }

        public static Challenge GetById(int id)
        {
            lock (Sync) { return _all.FirstOrDefault(c => c.Id == id && c.IsPublished); }
        }

        public static Challenge GetByIdAny(int id)
        {
            lock (Sync) { return _all.FirstOrDefault(c => c.Id == id); }
        }

        public static List<string> Categories()
        {
            return new List<string> { "Cryptography", "Digital Forensics", "Networking", "OSINT", "Reverse Engineering", "Steganography", "Web Security" };
        }

        public static Challenge Add(Challenge challenge)
        {
            lock (Sync)
            {
                challenge.Id = _all.Count == 0 ? 1 : _all.Max(c => c.Id) + 1;
                challenge.Status = "Unsolved";
                if (challenge.SuccessRate <= 0) challenge.SuccessRate = 0;
                _all.Add(challenge);
                return challenge;
            }
        }

        public static bool Update(Challenge updated)
        {
            lock (Sync)
            {
                var c = _all.FirstOrDefault(x => x.Id == updated.Id);
                if (c == null) return false;
                c.Title = updated.Title; c.Subtitle = updated.Subtitle; c.Category = updated.Category;
                c.Difficulty = updated.Difficulty; c.Points = updated.Points; c.Description = updated.Description;
                c.Hint = updated.Hint; c.Attachment = updated.Attachment; c.Flag = updated.Flag;
                c.IsPublished = updated.IsPublished;
                return true;
            }
        }

        public static bool SetPublished(int id, bool published)
        {
            lock (Sync)
            {
                var c = _all.FirstOrDefault(x => x.Id == id);
                if (c == null) return false;
                c.IsPublished = published;
                return true;
            }
        }

        public static bool Delete(int id)
        {
            lock (Sync)
            {
                var c = _all.FirstOrDefault(x => x.Id == id);
                if (c == null) return false;
                _all.Remove(c);
                return true;
            }
        }
    }

    /// <summary>Small helpers that return icon markup / colour classes.</summary>
    public static class UiHelper
    {
        private const string Open = "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.8\" stroke-linecap=\"round\" stroke-linejoin=\"round\">";
        private const string Close = "</svg>";

        public static string CategoryColor(string category)
        {
            switch (category)
            {
                case "Web Security": return "blue";
                case "Cryptography": return "green";
                case "Reverse Engineering": return "orange";
                case "Digital Forensics": return "purple";
                case "Networking": return "red";
                case "OSINT": return "pink";
                case "Steganography": return "cyan";
                default: return "blue";
            }
        }

        public static string CategoryIcon(string category)
        {
            switch (category)
            {
                case "Web Security":
                    return Open + "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M3 12h18M12 3c3 3.2 3 14.8 0 18M12 3c-3 3.2-3 14.8 0 18\"/>" + Close;
                case "Cryptography":
                    return Open + "<rect x=\"5\" y=\"11\" width=\"14\" height=\"9\" rx=\"2\"/><path d=\"M8 11V8a4 4 0 0 1 8 0v3\"/>" + Close;
                case "Reverse Engineering":
                    return Open + "<path d=\"m8 8-4 4 4 4M16 8l4 4-4 4M14 5l-4 14\"/>" + Close;
                case "Digital Forensics":
                    return Open + "<circle cx=\"11\" cy=\"11\" r=\"6\"/><path d=\"m20 20-4.2-4.2\"/>" + Close;
                case "Networking":
                    return Open + "<circle cx=\"12\" cy=\"5\" r=\"2\"/><circle cx=\"5\" cy=\"19\" r=\"2\"/><circle cx=\"19\" cy=\"19\" r=\"2\"/><path d=\"M12 7v4M12 11l-6 6M12 11l6 6\"/>" + Close;
                case "OSINT":
                    return Open + "<path d=\"M12 21s7-6.2 7-11.5A7 7 0 0 0 5 9.5C5 14.8 12 21 12 21Z\"/><circle cx=\"12\" cy=\"9.5\" r=\"2.5\"/>" + Close;
                case "Steganography":
                    return Open + "<rect x=\"3\" y=\"4\" width=\"18\" height=\"16\" rx=\"2\"/><circle cx=\"9\" cy=\"10\" r=\"1.6\"/><path d=\"m21 16-5-5-8 9\"/>" + Close;
                default:
                    return Open + "<circle cx=\"12\" cy=\"12\" r=\"8\"/>" + Close;
            }
        }
    }
}
