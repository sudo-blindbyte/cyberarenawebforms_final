using System;
using System.IO;
using System.Linq;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AppUser user = AuthHelper.GetCurrentUser(Context);
            if (user == null) return;

            litInitials.Text = Server.HtmlEncode(user.Initials);
            litUserName.Text = Server.HtmlEncode(user.FullName);
            litRole.Text = Server.HtmlEncode(user.Role);

            // Moderators only see the pages they are allowed to open.
            phAdminOnlyTop.Visible = user.IsAdmin;
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut(Context);
            Response.Redirect("~/Login.aspx", true);
        }

        protected string Active(params string[] pages)
        {
            string current = Path.GetFileName(Request.Path);
            return pages.Any(p => string.Equals(p, current, StringComparison.OrdinalIgnoreCase))
                ? "active"
                : string.Empty;
        }
    }
}
