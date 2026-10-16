using System;
using System.IO;
using System.Web.UI;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class ChallengeDetails : CyberArenaWebForms5.Models.UserPage
    {
        private Challenge _challenge;

        protected void Page_Load(object sender, EventArgs e)
        {
            int id;
            _challenge = int.TryParse(Request.QueryString["id"], out id) ? Challenge.GetById(id) : null;

            if (_challenge == null)
            {
                lblTitle.Text = "Challenge not found";
                pnlDetail.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            if (IsPostBack) return;

            lblTitle.Text = Server.HtmlEncode(_challenge.Title);
            lblCategoryPill.Text = Server.HtmlEncode(_challenge.Category);
            lblDifficultyPill.Text = Server.HtmlEncode(_challenge.Difficulty);
            lblDifficultyPill.CssClass = "pill " + _challenge.DifficultyCss;
            lblStatusPill.Text = Server.HtmlEncode(_challenge.Status);
            lblStatusPill.CssClass = "pill " + _challenge.StatusCss;

            lblDescription.Text = Server.HtmlEncode(_challenge.Description);
            lblAttachment.Text = Server.HtmlEncode(_challenge.Attachment);
            lblHint.Text = Server.HtmlEncode(_challenge.Hint);
            lblPoints.Text = _challenge.Points.ToString();
            lblSolves.Text = _challenge.Solves.ToString("N0");
            lblRate.Text = _challenge.SuccessRateText;

            if (_challenge.Status == "Solved")
            {
                ShowResult("You have already solved this challenge.", "info");
            }
        }

        protected void btnHint_Click(object sender, EventArgs e)
        {
            pnlHint.Visible = true;
            btnHint.Visible = false;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_challenge == null) return;

            string submitted = txtFlag.Text.Trim();
            if (submitted.Length == 0)
            {
                ShowResult("Please enter a flag before submitting.", "error");
                return;
            }

            // Sample check only: results are not stored until the database is added.
            if (string.Equals(submitted, _challenge.Flag, StringComparison.Ordinal))
            {
                ShowResult("Correct! +" + _challenge.Points + " points (sample result, not saved yet).", "success");
                txtFlag.Text = string.Empty;
            }
            else
            {
                ShowResult("Incorrect flag. Try again.", "error");
            }
        }

        protected void lnkDownload_Click(object sender, EventArgs e)
        {
            if (_challenge == null) return;

            // The file name comes from the challenge record, never from user input.
            string path = Server.MapPath("~/Uploads/" + _challenge.Attachment);
            if (File.Exists(path))
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + _challenge.Attachment + "\"");
                Response.TransmitFile(path);
                Response.End();
            }
            else
            {
                ShowResult("Sample attachment: place \"" + _challenge.Attachment + "\" in the Uploads folder to enable download.", "info");
            }
        }

        private void ShowResult(string text, string kind)
        {
            lblResult.Text = Server.HtmlEncode(text);
            lblResult.CssClass = "message " + kind;
        }
    }
}
