using System;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Login : PublicPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["registered"] == "1")
            {
                ShowMessage("Account created. Please sign in.", "success");
            }
            if (!IsPostBack && Request.QueryString["reset"] == "1")
            {
                ShowMessage("Password reset successfully. Please sign in with your new password.", "success");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string identifier = txtIdentifier.Text.Trim();
            string password = txtPassword.Text;

            if (identifier.Length == 0 || password.Length == 0)
            {
                ShowMessage("Please enter your username or email and your password.", "error");
                return;
            }

            AppUser user;
            LoginResult result = UserStore.Authenticate(identifier, password, out user);

            switch (result)
            {
                case LoginResult.Success:
                    AuthHelper.SignIn(Context, user);
                    Response.Redirect(AuthHelper.HomeFor(user), true);
                    return;

                case LoginResult.Suspended:
                    AuditLog.Write(identifier, "Blocked sign-in", "Account suspended", Context);
                    ShowMessage("This account has been suspended. Please contact an administrator.", "error");
                    break;

                case LoginResult.LockedOut:
                    AuditLog.Write(identifier, "Blocked sign-in", "Too many failed attempts", Context);
                    ShowMessage("Too many failed attempts. Please wait a few minutes and try again.", "error");
                    break;

                default:
                    AuditLog.Write(identifier, "Failed sign-in", "Invalid credentials", Context);
                    ShowMessage("Invalid username/email or password.", "error");
                    break;
            }
        }

        protected void lnkDemoAdmin_Click(object sender, EventArgs e)
        {
            FillDemo("admin@cyberarena.com", "Admin@123");
        }

        protected void lnkDemoMod_Click(object sender, EventArgs e)
        {
            FillDemo("mod@cyberarena.com", "Mod@12345");
        }

        protected void lnkDemoPlayer_Click(object sender, EventArgs e)
        {
            FillDemo("player@cyberarena.com", "User@123");
        }

        private void FillDemo(string email, string password)
        {
            txtIdentifier.Text = email;
            txtPassword.Attributes["value"] = password;
            ShowMessage("Demo credentials filled in. Click Sign in.", "info");
        }

        private void ShowMessage(string text, string kind)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = "message " + kind;
        }
    }
}
