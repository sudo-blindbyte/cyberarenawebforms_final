using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class Challenges : CyberArenaWebForms5.Models.UserPage
    {
        private const int PageSize = 6;

        public class PageItem
        {
            public int Number { get; set; }
            public string Css { get; set; }
        }

        private int CurrentPage
        {
            get { return ViewState["CurrentPage"] is int ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            foreach (string category in Challenge.Categories())
            {
                ddlCategory.Items.Add(new ListItem(category, category));
            }

            var all = Challenge.All();
            lblSolvedMini.Text = all.Count(c => c.Status == "Solved") + " / " + all.Count;
            lblPointsMini.Text = all.Sum(c => c.Points).ToString("N0");

            CurrentPage = 1;
            BindChallenges();
        }

        // Used by the search button and all three dropdowns.
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

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int page;
            if (e.CommandName == "Page" && int.TryParse(Convert.ToString(e.CommandArgument), out page))
            {
                CurrentPage = page;
                BindChallenges();
            }
        }

        private void BindChallenges()
        {
            string search = txtSearch.Text.Trim();
            string category = ddlCategory.SelectedValue;
            string difficulty = ddlDifficulty.SelectedValue;

            IEnumerable<Challenge> query = Challenge.All();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Category.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Subtitle.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (category != "All") query = query.Where(c => c.Category == category);
            if (difficulty != "All") query = query.Where(c => c.Difficulty == difficulty);

            switch (ddlSort.SelectedValue)
            {
                case "points_desc": query = query.OrderByDescending(c => c.Points); break;
                case "points_asc": query = query.OrderBy(c => c.Points); break;
                case "solves": query = query.OrderByDescending(c => c.Solves); break;
                case "title": query = query.OrderBy(c => c.Title); break;
            }

            List<Challenge> results = query.ToList();
            int total = results.Count;

            pnlList.Visible = total > 0;
            pnlEmpty.Visible = total == 0;
            if (total == 0) return;

            int pageCount = (int)Math.Ceiling(total / (double)PageSize);
            if (CurrentPage > pageCount) CurrentPage = pageCount;
            if (CurrentPage < 1) CurrentPage = 1;

            var pageRows = results.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            rptChallenges.DataSource = pageRows;
            rptChallenges.DataBind();

            int first = (CurrentPage - 1) * PageSize + 1;
            lblShowing.Text = "Showing " + first + "–" + (first + pageRows.Count - 1) + " of " + total + " challenges";

            var pages = new List<PageItem>();
            for (int i = 1; i <= pageCount; i++)
            {
                pages.Add(new PageItem { Number = i, Css = i == CurrentPage ? "current" : "" });
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();

            lnkPrev.Enabled = CurrentPage > 1;
            lnkNext.Enabled = CurrentPage < pageCount;
        }
    }
}
