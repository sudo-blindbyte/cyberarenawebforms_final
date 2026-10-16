using System;
using System.IO;
using System.Linq;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AppUser user = AuthHelper.GetCurrentUser(Context);
            if (user == null) return;

            litInitials.Text = Server.HtmlEncode(user.Initials);
            litUserName.Text = Server.HtmlEncode(user.FullName);
            litRole.Text = Server.HtmlEncode(user.Role);

            if (user.IsAdmin || user.IsModerator)
            {
                lnkAdminPanel.Visible = true;
                lnkAdminPanel.NavigateUrl = user.IsAdmin ? "~/AdminDashboard.aspx" : "~/AdminSubmissions.aspx";
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut(Context);
            Response.Redirect("~/Login.aspx", true);
        }

        /// <summary>
        /// Returns "active" when the current page matches one of the given
        /// file names. Used to highlight the sidebar navigation item.
        /// </summary>
        protected string Active(params string[] pages)
        {
            string current = Path.GetFileName(Request.Path);
            return pages.Any(p => string.Equals(p, current, StringComparison.OrdinalIgnoreCase))
                ? "active"
                : string.Empty;
        }
    }
}
