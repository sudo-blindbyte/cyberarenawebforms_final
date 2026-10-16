using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminChallenges : AdminPage
    {
        private const int PageSize = 6;

        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] is int ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            foreach (string c in Challenge.Categories()) ddlCategory.Items.Add(new ListItem(c, c));

            if (Request.QueryString["saved"] == "1") ShowMessage("Challenge saved.", "success");

            CurrentPage = 1;
            BindChallenges();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindChallenges();
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            CurrentPage = Math.Max(1, CurrentPage - 1);
            BindChallenges();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            CurrentPage = CurrentPage + 1;
            BindChallenges();
        }

        protected void rptChallenges_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            Challenge c = Challenge.GetByIdAny(id);
            if (c == null) return;

            if (e.CommandName == "publish")
            {
                bool publish = !c.IsPublished;
                Challenge.SetPublished(id, publish);
                AuditLog.Write(CurrentUser.Username, publish ? "Published challenge" : "Unpublished challenge", c.Title, Context);
                ShowMessage("\"" + c.Title + "\" is now " + (publish ? "published" : "a draft") + ".", "success");
            }
            else if (e.CommandName == "remove")
            {
                string title = c.Title;
                Challenge.Delete(id);
                AuditLog.Write(CurrentUser.Username, "Deleted challenge", title, Context);
                ShowMessage("\"" + title + "\" deleted.", "success");
            }
            BindChallenges();
        }

        private void BindChallenges()
        {
            List<Challenge> all = Challenge.AllAdmin();

            lblTotal.Text = all.Count.ToString();
            lblPublished.Text = all.Count(c => c.IsPublished).ToString();
            lblDrafts.Text = all.Count(c => !c.IsPublished).ToString();
            lblAvg.Text = all.Count == 0 ? "0" : ((int)Math.Round(all.Average(c => c.Points))).ToString();

            string search = txtSearch.Text.Trim();
            IEnumerable<Challenge> query = all;
            if (search.Length > 0)
            {
                query = query.Where(c =>
                    c.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Category.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (ddlCategory.SelectedValue != "All") query = query.Where(c => c.Category == ddlCategory.SelectedValue);
            if (ddlState.SelectedValue == "Published") query = query.Where(c => c.IsPublished);
            if (ddlState.SelectedValue == "Draft") query = query.Where(c => !c.IsPublished);

            List<Challenge> results = query.ToList();
            int total = results.Count;
            int pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            if (CurrentPage > pageCount) CurrentPage = pageCount;
            if (CurrentPage < 1) CurrentPage = 1;

            List<Challenge> rows = results.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            rptChallenges.DataSource = rows;
            rptChallenges.DataBind();

            int first = total == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
            lblShowing.Text = total == 0 ? "No challenges found" : "Showing " + first + "–" + (first + rows.Count - 1) + " of " + total + " challenges";
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
