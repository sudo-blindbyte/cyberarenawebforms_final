using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class CreateChallenge : AdminPage
    {
        private static readonly string[] Difficulties = { "Easy", "Medium", "Hard" };
        private static readonly string[] AllowedExtensions = { ".zip", ".txt", ".pdf", ".png", ".jpg", ".jpeg", ".pcap", ".bin", ".py", ".md" };
        private const int MaxUploadBytes = 4 * 1024 * 1024;

        private int EditId
        {
            get { return ViewState["EditId"] is int ? (int)ViewState["EditId"] : 0; }
            set { ViewState["EditId"] = value; }
        }

        private string AttachmentName
        {
            get { return ViewState["Attachment"] as string ?? string.Empty; }
            set { ViewState["Attachment"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            foreach (string c in Challenge.Categories()) ddlCategory.Items.Add(c);

            int id;
            if (int.TryParse(Request.QueryString["id"], out id))
            {
                Challenge c = Challenge.GetByIdAny(id);
                if (c == null)
                {
                    Response.Redirect("~/AdminChallenges.aspx", true);
                    return;
                }
                EditId = id;
                lblHeading.Text = "Edit Challenge";
                btnSave.Text = "Save changes";

                txtTitle.Text = c.Title;
                txtSubtitle.Text = c.Subtitle;
                ddlCategory.SelectedValue = c.Category;
                ddlDifficulty.SelectedValue = c.Difficulty;
                txtPoints.Text = c.Points.ToString();
                txtDescription.Text = c.Description;
                txtHint.Text = c.Hint;
                txtFlag.Text = c.Flag;
                AttachmentName = c.Attachment ?? string.Empty;
                chkPublish.Checked = c.IsPublished;
            }

            ShowStep(0);
        }

        // ---------- navigation ----------
        protected void btnNext_Click(object sender, EventArgs e)
        {
            int step = mvSteps.ActiveViewIndex;
            string error = ValidateStep(step);
            if (error != null) { ShowMessage(error, "error"); RefreshSide(); return; }
            ShowStep(step + 1);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            ShowStep(Math.Max(0, mvSteps.ActiveViewIndex - 1));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AdminChallenges.aspx", true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string error = ValidateStep(0) ?? ValidateStep(1);
            if (error != null) { ShowMessage(error, "error"); RefreshSide(); return; }

            var challenge = new Challenge
            {
                Id = EditId,
                Title = txtTitle.Text.Trim(),
                Subtitle = txtSubtitle.Text.Trim(),
                Category = ddlCategory.SelectedValue,
                Difficulty = ddlDifficulty.SelectedValue,
                Points = int.Parse(txtPoints.Text.Trim()),
                Description = txtDescription.Text.Trim(),
                Hint = txtHint.Text.Trim(),
                Attachment = AttachmentName,
                Flag = txtFlag.Text.Trim(),
                IsPublished = chkPublish.Checked
            };

            if (EditId > 0)
            {
                Challenge.Update(challenge);
                AuditLog.Write(CurrentUser.Username, "Edited challenge", challenge.Title, Context);
            }
            else
            {
                Challenge.Add(challenge);
                AuditLog.Write(CurrentUser.Username, challenge.IsPublished ? "Published challenge" : "Created draft challenge", challenge.Title, Context);
            }

            Response.Redirect("~/AdminChallenges.aspx?saved=1", true);
        }

        // ---------- validation ----------
        private string ValidateStep(int step)
        {
            if (step == 0)
            {
                string title = txtTitle.Text.Trim();
                int points;
                if (title.Length < 3) return "Please enter a title (at least 3 characters).";
                if (!Challenge.Categories().Contains(ddlCategory.SelectedValue)) return "Please choose a valid category.";
                if (Array.IndexOf(Difficulties, ddlDifficulty.SelectedValue) < 0) return "Please choose a valid difficulty.";
                if (!int.TryParse(txtPoints.Text.Trim(), out points) || points < 10 || points > 1000) return "Points must be a whole number between 10 and 1000.";
                return null;
            }

            if (step == 1)
            {
                if (txtDescription.Text.Trim().Length < 10) return "Please write a description (at least 10 characters).";
                if (txtDescription.Text.Length > 2000) return "The description is longer than 2000 characters.";
                if (txtFlag.Text.Trim().Length < 4) return "Please enter the flag players must find.";
                return SaveUpload();
            }
            return null;
        }

        /// <summary>Validates and stores the optional attachment. Returns an error text or null.</summary>
        private string SaveUpload()
        {
            if (!fuAttachment.HasFile) return null;

            if (fuAttachment.PostedFile.ContentLength > MaxUploadBytes) return "The attachment is larger than 4 MB.";

            string original = Path.GetFileName(fuAttachment.FileName);
            string extension = Path.GetExtension(original).ToLowerInvariant();
            if (Array.IndexOf(AllowedExtensions, extension) < 0) return "That file type is not allowed.";

            string baseName = Regex.Replace(Path.GetFileNameWithoutExtension(original), "[^A-Za-z0-9_-]", "_");
            if (baseName.Length == 0) baseName = "file";
            if (baseName.Length > 40) baseName = baseName.Substring(0, 40);

            string safeName = baseName + extension;
            string folder = Server.MapPath("~/Uploads");
            Directory.CreateDirectory(folder);
            if (File.Exists(Path.Combine(folder, safeName)))
            {
                safeName = baseName + "_" + Guid.NewGuid().ToString("N").Substring(0, 6) + extension;
            }

            fuAttachment.SaveAs(Path.Combine(folder, safeName));
            AttachmentName = safeName;
            return null;
        }

        // ---------- rendering ----------
        private void ShowStep(int step)
        {
            mvSteps.ActiveViewIndex = step;
            btnBack.Visible = step > 0;
            btnNext.Visible = step < 2;
            btnSave.Visible = step == 2;
            RefreshSide();

            if (step == 2) litReview.Text = BuildReview();
        }

        private void RefreshSide()
        {
            int step = mvSteps.ActiveViewIndex;
            litStepper.Text = BuildStepper(step);
            lblAttachment.Text = Server.HtmlEncode(AttachmentName.Length == 0 ? "none" : AttachmentName);
            litPreview.Text = BuildPreview();
        }

        private static string BuildStepper(int active)
        {
            string[] names = { "Basic info", "Content & flag", "Review" };
            var sb = new StringBuilder("<div class=\"stepper\">");
            for (int i = 0; i < names.Length; i++)
            {
                string css = i < active ? "done" : (i == active ? "on" : "");
                sb.Append("<div class=\"step " + css + "\"><span class=\"num\">" + (i < active ? "✓" : (i + 1).ToString()) + "</span><span>" + names[i] + "</span></div>");
                if (i < names.Length - 1) sb.Append("<div class=\"step-line\"></div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        private string BuildPreview()
        {
            string title = txtTitle.Text.Trim();
            if (title.Length == 0) title = "Untitled challenge";
            string points = txtPoints.Text.Trim();
            int p;
            if (!int.TryParse(points, out p)) p = 0;

            var temp = new Challenge { Category = ddlCategory.SelectedValue ?? "Web Security", Difficulty = ddlDifficulty.SelectedValue };

            return "<div class=\"preview-thumb\"><div class=\"ch-icon " + temp.IconColorCss + "\">" + temp.IconSvg + "</div>" +
                   "<span class=\"preview-label\">" + Server.HtmlEncode(temp.Category) + "</span></div>" +
                   "<div class=\"preview-title\">" + Server.HtmlEncode(title) + "</div>" +
                   "<div class=\"preview-meta\"><span class=\"pill " + temp.DifficultyCss + "\">" + Server.HtmlEncode(temp.Difficulty) + "</span> " +
                   "<strong>" + p + " pts</strong></div>";
        }

        private string BuildReview()
        {
            string flag = txtFlag.Text.Trim();
            string maskedFlag = flag.Length <= 4 ? "••••" : flag.Substring(0, 4) + new string('•', flag.Length - 4);

            var sb = new StringBuilder("<div class=\"review-list\">");
            AddRow(sb, "Title", txtTitle.Text.Trim());
            AddRow(sb, "Summary", txtSubtitle.Text.Trim());
            AddRow(sb, "Category", ddlCategory.SelectedValue);
            AddRow(sb, "Difficulty", ddlDifficulty.SelectedValue);
            AddRow(sb, "Points", txtPoints.Text.Trim());
            AddRow(sb, "Attachment", AttachmentName.Length == 0 ? "none" : AttachmentName);
            AddRow(sb, "Flag", maskedFlag);
            sb.Append("</div>");
            return sb.ToString();
        }

        private void AddRow(StringBuilder sb, string label, string value)
        {
            sb.Append("<div class=\"review-row\"><span>" + label + "</span><strong>" + Server.HtmlEncode(value.Length == 0 ? "—" : value) + "</strong></div>");
        }

        private void ShowMessage(string text, string kind)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "message " + kind;
        }
    }
}
