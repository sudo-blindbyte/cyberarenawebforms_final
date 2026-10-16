using System;
using System.Text.RegularExpressions;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Profile : UserPage
    {
        private static readonly Regex UsernameRule = new Regex("^[A-Za-z0-9_]{3,20}$");
        private static readonly Regex EmailRule = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            txtFullName.Text = CurrentUser.FullName;
            txtUsername.Text = CurrentUser.Username;
            txtEmail.Text = CurrentUser.Email;
            txtInstitution.Text = "CyberArena Community";
            txtBio.Text = "Cybersecurity enthusiast interested in CTFs, OSINT and digital forensics.";

            lblDisplayName.Text = Server.HtmlEncode(CurrentUser.FullName);
            lblUsername.Text = Server.HtmlEncode(CurrentUser.Username);
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (fullName.Length == 0 || username.Length == 0 || email.Length == 0)
            {
                lblProfileMessage.Text = "Please complete your name, username and email.";
                return;
            }
            if (!UsernameRule.IsMatch(username))
            {
                lblProfileMessage.Text = "Username must be 3–20 characters: letters, numbers or underscore.";
                return;
            }
            if (!EmailRule.IsMatch(email))
            {
                lblProfileMessage.Text = "Please enter a valid email address.";
                return;
            }

            AppUser other = UserStore.FindByIdentifier(username);
            if (other != null && other.Id != CurrentUser.Id)
            {
                lblProfileMessage.Text = "That username is already taken.";
                return;
            }
            other = UserStore.FindByIdentifier(email);
            if (other != null && other.Id != CurrentUser.Id)
            {
                lblProfileMessage.Text = "That email is already registered.";
                return;
            }

            UserStore.UpdateProfile(CurrentUser.Id, fullName, username, email);
            AuditLog.Write(username, "Updated profile", "Profile details changed", Context);

            lblDisplayName.Text = Server.HtmlEncode(fullName);
            lblUsername.Text = Server.HtmlEncode(username);
            lblProfileMessage.Text = "Profile updated.";
        }
    }
}
