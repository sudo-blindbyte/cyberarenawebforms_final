using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace CyberArenaWebForms5.Models
{
    public static class AuthHelper
    {
        private const string SessionKey = "CyberArena.UserId";

        public static AppUser GetCurrentUser(HttpContext context)
        {
            if (context == null || context.Session == null) return null;

            object cached = context.Items["CyberArena.User"];
            if (cached != null) return (AppUser)cached;

            object id = context.Session[SessionKey];
            if (!(id is int)) return null;

            AppUser user = UserStore.FindById((int)id);
            // A suspended or deleted account is signed out immediately.
            if (user == null || user.Status != "Active")
            {
                context.Session.Remove(SessionKey);
                return null;
            }
            context.Items["CyberArena.User"] = user;
            return user;
        }

        public static void SignIn(HttpContext context, AppUser user)
        {
            context.Session.Clear();
            context.Session[SessionKey] = user.Id;
            AuditLog.Write(user.Username, "Signed in", "Session started", context);
        }

        public static void SignOut(HttpContext context)
        {
            AppUser user = GetCurrentUser(context);
            if (user != null) AuditLog.Write(user.Username, "Signed out", "Session ended", context);
            context.Session.Clear();
            context.Session.Abandon();
        }

        public static string HomeFor(AppUser user)
        {
            if (user == null) return "~/Login.aspx";
            if (user.IsAdmin) return "~/AdminDashboard.aspx";
            if (user.IsModerator) return "~/AdminSubmissions.aspx";
            return "~/Dashboard.aspx";
        }
    }

    /// <summary>Base class for every page that needs a signed-in user.</summary>
    public abstract class SecurePage : Page
    {
        public AppUser CurrentUser { get; private set; }

        /// <summary>Return true when the signed-in user may open this page.</summary>
        protected abstract bool IsAllowed(AppUser user);

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            // Never let the browser cache protected pages (back button after sign-out).
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            CurrentUser = AuthHelper.GetCurrentUser(Context);
            if (CurrentUser == null)
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }
            if (!IsAllowed(CurrentUser))
            {
                Response.Redirect(AuthHelper.HomeFor(CurrentUser), true);
                return;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            // Ties the view state to this session (protects against cross-site request forgery).
            ViewStateUserKey = Session.SessionID;
            base.OnInit(e);
        }
    }

    /// <summary>Participant pages: any signed-in account.</summary>
    public abstract class UserPage : SecurePage
    {
        protected override bool IsAllowed(AppUser user) { return true; }
    }

    /// <summary>Admin-only pages.</summary>
    public abstract class AdminPage : SecurePage
    {
        protected override bool IsAllowed(AppUser user) { return user.IsAdmin; }
    }

    /// <summary>Pages that admins and moderators may open.</summary>
    public abstract class StaffPage : SecurePage
    {
        protected override bool IsAllowed(AppUser user) { return user.IsAdmin || user.IsModerator; }
    }

    /// <summary>Login / register pages: signed-in users are sent to their own home page.</summary>
    public abstract class PublicPage : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            AppUser user = AuthHelper.GetCurrentUser(Context);
            if (user != null)
            {
                Response.Redirect(AuthHelper.HomeFor(user), true);
            }
        }
    }
}
