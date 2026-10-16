using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminUsers : AdminPage
    {
        private const int PageSize = 6;
        private static readonly string[] Roles = { "Player", "Moderator", "Admin" };
        private static readonly Regex UsernameRule = new Regex("^[A-Za-z0-9_]{3,20}$");
        private static readonly Regex EmailRule = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] is int ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            CurrentPage = 1;
            BindUsers();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindUsers();
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            CurrentPage = Math.Max(1, CurrentPage - 1);
            BindUsers();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            CurrentPage = CurrentPage + 1;
            BindUsers();
        }

        // ---------- add user ----------
        protected void btnShowAdd_Click(object sender, EventArgs e)
        {
            pnlAdd.Visible = true;
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            pnlAdd.Visible = false;
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            string name = txtNewName.Text.Trim();
            string username = txtNewUsername.Text.Trim();
            string email = txtNewEmail.Text.Trim();
            string password = txtNewPassword.Text;
            string role = ddlNewRole.SelectedValue;

            pnlAdd.Visible = true;

            string error = null;
            if (name.Length < 2 || username.Length == 0 || email.Length == 0 || password.Length == 0) error = "Please complete every field.";
            else if (!UsernameRule.IsMatch(username)) error = "Username must be 3–20 characters: letters, numbers or underscore.";
            else if (!EmailRule.IsMatch(email)) error = "Please enter a valid email address.";
            else if (password.Length < 8 || !Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[a-z]") || !Regex.IsMatch(password, "[0-9]")) error = "Password needs 8+ characters with upper-case, lower-case and a number.";
            else if (Array.IndexOf(Roles, role) < 0) error = "Invalid role.";
            else if (UserStore.IsUsernameTaken(username)) error = "That username is already taken.";
            else if (UserStore.IsEmailTaken(email)) error = "That email is already registered.";

            if (error != null)
            {
                ShowMessage(error, "error");
                BindUsers();
                return;
            }

            UserStore.Create(username, name, email, password, role);
            AuditLog.Write(CurrentUser.Username, "Created user", username + " (" + role + ")", Context);

            pnlAdd.Visible = false;
            txtNewName.Text = txtNewUsername.Text = txtNewEmail.Text = string.Empty;
            ShowMessage("User \"" + username + "\" created.", "success");
            BindUsers();
        }

        // ---------- row actions ----------
        protected void rptUsers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            AppUser user = (AppUser)e.Item.DataItem;
            DropDownList ddl = (DropDownList)e.Item.FindControl("ddlRowRole");
            ddl.SelectedValue = user.Role;

            // Admins cannot change or remove their own account here.
            bool isSelf = user.Id == CurrentUser.Id;
            ddl.Enabled = !isSelf;
            e.Item.FindControl("btnToggle").Visible = !isSelf;
            e.Item.FindControl("btnDelete").Visible = !isSelf;
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            AppUser target = UserStore.FindById(id);
            if (target == null || target.Id == CurrentUser.Id) return;

            if (e.CommandName == "toggle")
            {
                string newStatus = target.Status == "Active" ? "Suspended" : "Active";
                if (UserStore.SetStatus(id, newStatus))
                {
                    AuditLog.Write(CurrentUser.Username, newStatus == "Active" ? "Activated user" : "Suspended user", target.Username, Context);
                    ShowMessage(target.Username + " is now " + newStatus.ToLowerInvariant() + ".", "success");
                }
                else ShowMessage("At least one active administrator must remain.", "error");
            }
            else if (e.CommandName == "remove")
            {
                string name = target.Username;
                if (UserStore.Delete(id))
                {
                    AuditLog.Write(CurrentUser.Username, "Deleted user", name, Context);
                    ShowMessage("User \"" + name + "\" deleted.", "success");
                }
                else ShowMessage("At least one administrator must remain.", "error");
            }
            BindUsers();
        }

        protected void ddlRowRole_Changed(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            HiddenField hf = (HiddenField)item.FindControl("hfUserId");

            int id;
            string role = ddl.SelectedValue;
            if (!int.TryParse(hf.Value, out id) || Array.IndexOf(Roles, role) < 0) return;

            AppUser target = UserStore.FindById(id);
            if (target == null || target.Id == CurrentUser.Id) return;

            if (UserStore.SetRole(id, role))
            {
                AuditLog.Write(CurrentUser.Username, "Changed role", target.Username + " → " + role, Context);
                ShowMessage(target.Username + " is now " + role + ".", "success");
            }
            else ShowMessage("At least one administrator must remain.", "error");

            BindUsers();
        }

        // ---------- binding ----------
        private void BindUsers()
        {
            List<AppUser> all = UserStore.All();

            lblTotal.Text = all.Count.ToString("N0");
            lblActive.Text = all.Count(u => u.Status == "Active").ToString("N0");
            lblSuspended.Text = all.Count(u => u.Status == "Suspended").ToString("N0");
            lblStaff.Text = all.Count(u => u.Role != "Player").ToString("N0");

            string search = txtSearch.Text.Trim();
            IEnumerable<AppUser> query = all;
            if (search.Length > 0)
            {
                query = query.Where(u =>
                    u.FullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.Username.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.Email.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (ddlRole.SelectedValue != "All") query = query.Where(u => u.Role == ddlRole.SelectedValue);
            if (ddlStatus.SelectedValue != "All") query = query.Where(u => u.Status == ddlStatus.SelectedValue);

            List<AppUser> results = query.ToList();
            int total = results.Count;
            int pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            if (CurrentPage > pageCount) CurrentPage = pageCount;
            if (CurrentPage < 1) CurrentPage = 1;

            List<AppUser> rows = results.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            rptUsers.DataSource = rows;
            rptUsers.DataBind();

            int first = total == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
            lblShowing.Text = total == 0 ? "No users found" : "Showing " + first + "–" + (first + rows.Count - 1) + " of " + total + " users";
            lblPageInfo.Text = "Page " + CurrentPage + " of " + pageCount;
            lnkPrev.Enabled = CurrentPage > 1;
            lnkNext.Enabled = CurrentPage < pageCount;
        }

        private void ShowMessage(string text, string kind)
        {
            lblPageMessage.Text = Server.HtmlEncode(text);
            lblPageMessage.CssClass = "message " + kind;
        }
    }
}
