using System;
using System.Text.RegularExpressions;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Register : PublicPage
    {
        private static readonly Regex UsernameRule = new Regex("^[A-Za-z0-9_]{3,20}$");
        private static readonly Regex EmailRule = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            string error = Validate(fullName, username, email, password, confirm);
            if (error != null)
            {
                ShowMessage(error, "error");
                return;
            }

            // Self-registration can only ever create a Player account.
            AppUser user = UserStore.Create(username, fullName, email, password, "Player");
            AuditLog.Write(user.Username, "Registered account", "New player account", Context);

            Response.Redirect("~/Login.aspx?registered=1", true);
        }

        private static string Validate(string fullName, string username, string email, string password, string confirm)
        {
            if (fullName.Length < 2 || username.Length == 0 || email.Length == 0 || password.Length == 0)
                return "Please complete every field.";
            if (!UsernameRule.IsMatch(username))
                return "Username must be 3–20 characters: letters, numbers or underscore.";
            if (!EmailRule.IsMatch(email))
                return "Please enter a valid email address.";
            if (password.Length < 8 || !Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[a-z]") || !Regex.IsMatch(password, "[0-9]"))
                return "Password needs 8+ characters with an uppercase letter, a lowercase letter and a number.";
            if (password != confirm)
                return "The two passwords do not match.";
            if (UserStore.IsUsernameTaken(username))
                return "That username is already taken.";
            if (UserStore.IsEmailTaken(email))
                return "That email is already registered.";
            return null;
        }

        private void ShowMessage(string text, string kind)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "message " + kind;
        }
    }
}
