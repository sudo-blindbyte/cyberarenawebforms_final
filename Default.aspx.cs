using System;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    /// <summary>Entry point: sends visitors to the login page or to their own home page.</summary>
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AppUser user = AuthHelper.GetCurrentUser(Context);
            Response.Redirect(user == null ? "~/Login.aspx" : AuthHelper.HomeFor(user), true);
        }
    }
}
