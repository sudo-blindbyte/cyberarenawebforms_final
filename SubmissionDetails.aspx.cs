using System;
using System.Text;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class SubmissionDetails : StaffPage
    {
        private string SubmissionId
        {
            get { return (Request.QueryString["id"] ?? string.Empty).Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmissionRecord rec = SubmissionStore.Find(SubmissionId);
            if (rec == null)
            {
                pnlDetail.Visible = false;
                pnlNotFound.Visible = true;
                lblSubId.Text = "unknown";
                return;
            }

            if (!IsPostBack) BindRecord(rec, true);
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            SubmissionRecord rec = SubmissionStore.Find(SubmissionId);
            if (rec == null) return;

            if (rec.Status == "Revoked") { ShowMessage("A revoked submission cannot be verified.", "error"); BindRecord(rec, false); return; }

            SubmissionStore.Update(rec.Id, "Verified", CleanNotes());
            AuditLog.Write(CurrentUser.Username, "Verified submission", rec.Id, Context);
            ShowMessage("Submission marked as verified.", "success");
            BindRecord(SubmissionStore.Find(rec.Id), false);
        }

        protected void btnRevoke_Click(object sender, EventArgs e)
        {
            SubmissionRecord rec = SubmissionStore.Find(SubmissionId);
            if (rec == null) return;

            SubmissionStore.Update(rec.Id, "Revoked", CleanNotes());
            AuditLog.Write(CurrentUser.Username, "Revoked submission points", rec.Id, Context);
            ShowMessage("Points revoked for this submission.", "success");
            BindRecord(SubmissionStore.Find(rec.Id), false);
        }

        protected void btnSaveNotes_Click(object sender, EventArgs e)
        {
            SubmissionRecord rec = SubmissionStore.Find(SubmissionId);
            if (rec == null) return;

            SubmissionStore.SaveNotes(rec.Id, CleanNotes());
            AuditLog.Write(CurrentUser.Username, "Edited submission note", rec.Id, Context);
            ShowMessage("Note saved.", "success");
            BindRecord(SubmissionStore.Find(rec.Id), false);
        }

        private string CleanNotes()
        {
            string notes = txtNotes.Text.Trim();
            return notes.Length > 500 ? notes.Substring(0, 500) : notes;
        }

        private void BindRecord(SubmissionRecord rec, bool loadNotes)
        {
            lblSubId.Text = Server.HtmlEncode(rec.Id);
            lblUser.Text = Server.HtmlEncode(rec.User);
            lblChallenge.Text = Server.HtmlEncode(rec.Challenge);
            lblPoints.Text = Server.HtmlEncode(rec.PointsText);
            lblSubmitted.Text = Server.HtmlEncode(rec.SubmittedText);
            lblAttempt.Text = "#" + rec.Attempt;
            lblStatus.Text = Server.HtmlEncode(rec.Status);
            lblStatus.CssClass = "pill " + rec.StatusCss;
            lblFlag.Text = Server.HtmlEncode(rec.MaskedFlag);
            lblIp.Text = Server.HtmlEncode(rec.Ip);
            if (loadNotes) txtNotes.Text = rec.Notes;

            btnRevoke.Enabled = rec.Status != "Revoked";
            btnVerify.Enabled = rec.Status != "Revoked" && rec.Status != "Verified";

            litTrace.Text = BuildTrace(rec);
        }

        private static string Line(string css, string text)
        {
            return "<div class=\"tl " + css + "\">" + System.Web.HttpUtility.HtmlEncode(text) + "</div>";
        }

        private static string BuildTrace(SubmissionRecord rec)
        {
            var sb = new StringBuilder();
            string t = rec.SubmittedAt.ToString("HH:mm:ss");
            sb.Append(Line("info", "[" + t + "] POST /api/submit  user=" + rec.User + "  ip=" + rec.Ip));
            sb.Append(Line("dim", "[" + t + "] session ok · rate limit ok · attempt #" + rec.Attempt));
            sb.Append(Line("dim", "[" + t + "] challenge=\"" + rec.Challenge + "\" flag=" + rec.MaskedFlag));

            if (rec.Attempt > 20)
                sb.Append(Line("warn", "[" + t + "] WARN high attempt count — possible automated guessing"));

            if (rec.Status == "Rejected")
                sb.Append(Line("err", "[" + t + "] flag mismatch · 0 points awarded"));
            else if (rec.Status == "Revoked")
                sb.Append(Line("err", "[" + t + "] points revoked by moderation"));
            else
                sb.Append(Line("ok", "[" + t + "] flag match · " + rec.PointsText + " points awarded"));

            if (rec.Status == "Verified")
                sb.Append(Line("ok", "[" + t + "] manually verified by staff"));

            sb.Append("<div class=\"tl cursor\">&nbsp;</div>");
            return sb.ToString();
        }

        private void ShowMessage(string text, string kind)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "message " + kind;
        }
    }
}
