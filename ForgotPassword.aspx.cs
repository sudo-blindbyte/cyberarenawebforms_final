using System;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class ForgotPassword : PublicPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if we have a reset token in the query string
                string token = Request.QueryString["token"];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    // Verify the token is valid (stored in session during password reset request)
                    if (Session["ResetToken"] != null && Session["ResetToken"].ToString() == token)
                    {
                        pnlRequest.Visible = false;
                        pnlReset.Visible = true;
                        lblMessage.Text = "Enter your new password below.";
                        lblMessage.CssClass = "message info";
                    }
                    else
                    {
                        ShowMessage("Invalid or expired reset link. Please try again.", "error");
                        pnlRequest.Visible = true;
                        pnlReset.Visible = false;
                    }
                }
            }
        }

        protected void btnResetRequest_Click(object sender, EventArgs e)
        {
            string identifier = txtIdentifier.Text.Trim();

            if (string.IsNullOrWhiteSpace(identifier))
            {
                ShowMessage("Please enter your email or username.", "error");
                return;
            }

            // Find user by email or username
            AppUser user = UserStore.FindByEmailOrUsername(identifier);

            if (user == null)
            {
                // For security, don't reveal if user exists
                ShowMessage("If an account exists with this email or username, a reset link will be sent shortly.", "success");
                return;
            }

            // Generate a simple reset token (in production, use a proper token with expiry)
            string resetToken = Guid.NewGuid().ToString("N");

            // Store in session (in production, store in database with expiry time)
            Session["ResetToken"] = resetToken;
            Session["ResetUserId"] = user.Id;

            // Show the reset form
            pnlRequest.Visible = false;
            pnlReset.Visible = true;
            txtIdentifier.Text = "";

            ShowMessage($"Ready to reset password for {Server.HtmlEncode(user.Username)}. Enter your new password.", "info");
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            // Verify we have a valid reset session
            if (Session["ResetToken"] == null || Session["ResetUserId"] == null)
            {
                ShowMessage("Session expired. Please start over.", "error");
                Response.Redirect("~/ForgotPassword.aspx", false);
                return;
            }

            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowMessage("Please enter a new password.", "error");
                return;
            }

            if (newPassword.Length < 6)
            {
                ShowMessage("Password must be at least 6 characters long.", "error");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowMessage("Passwords do not match.", "error");
                return;
            }

            try
            {
                int userId = (int)Session["ResetUserId"];
                AppUser user = UserStore.FindById(userId);

                if (user == null)
                {
                    ShowMessage("User not found.", "error");
                    return;
                }

                // Update password using UserStore
                bool success = UserStore.UpdatePassword(userId, newPassword);

                if (success)
                {
                    // Clear session
                    Session.Remove("ResetToken");
                    Session.Remove("ResetUserId");

                    // Log the password reset
                    AuditLog.Write(user.Username, "Password reset", "User reset password via forgot password link", Context);

                    // Redirect to login with success message
                    Response.Redirect("~/Login.aspx?reset=1", true);
                }
                else
                {
                    ShowMessage("Failed to reset password. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("An error occurred: " + ex.Message, "error");
            }
        }

        private void ShowMessage(string text, string kind)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "message " + kind;
        }
    }
}
