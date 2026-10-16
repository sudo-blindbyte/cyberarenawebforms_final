
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace CyberArenaWebForms5
{
    public partial class Submissions : CyberArenaWebForms5.Models.UserPage
    {
        public class SubmissionInfo
        {
            public string Challenge { get; set; }
            public string Category { get; set; }
            public string SubmittedAt { get; set; }
            public string Status { get; set; }
            public int Points { get; set; }
        }

        private List<SubmissionInfo> GetSampleSubmissions()
        {
            return new List<SubmissionInfo>
            {
                new SubmissionInfo
                {
                    Challenge = "SQL Injection Basics",
                    Category = "Web Security",
                    SubmittedAt = "11 Oct 2026, 10:30 AM",
                    Status = "Accepted",
                    Points = 100
                },
                new SubmissionInfo
                {
                    Challenge = "Caesar Cipher",
                    Category = "Cryptography",
                    SubmittedAt = "11 Oct 2026, 10:15 AM",
                    Status = "Incorrect",
                    Points = 0
                },
                new SubmissionInfo
                {
                    Challenge = "Hidden in Plain Sight",
                    Category = "Steganography",
                    SubmittedAt = "10 Oct 2026, 04:20 PM",
                    Status = "Accepted",
                    Points = 75
                },
                new SubmissionInfo
                {
                    Challenge = "Basic Network Scan",
                    Category = "Networking",
                    SubmittedAt = "10 Oct 2026, 03:40 PM",
                    Status = "Incorrect",
                    Points = 0
                },
                new SubmissionInfo
                {
                    Challenge = "OSINT Investigation",
                    Category = "OSINT",
                    SubmittedAt = "09 Oct 2026, 11:00 AM",
                    Status = "Accepted",
                    Points = 75
                },
                new SubmissionInfo
                {
                    Challenge = "File Signature Analysis",
                    Category = "Digital Forensics",
                    SubmittedAt = "09 Oct 2026, 09:30 AM",
                    Status = "Incorrect",
                    Points = 0
                }
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSubmissions();
            }
        }

        protected void ddlStatus_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            BindSubmissions();
        }

        private void BindSubmissions()
        {
            List<SubmissionInfo> submissions = GetSampleSubmissions();

            string selectedStatus = ddlStatus.SelectedValue;

            if (selectedStatus != "All")
            {
                submissions = submissions
                    .Where(s => s.Status == selectedStatus)
                    .ToList();
            }

            gvSubmissions.DataSource = submissions;
            gvSubmissions.DataBind();

            lblSubmissionMessage.Text =
                submissions.Count + " submission(s) displayed.";
        }
    }
}
