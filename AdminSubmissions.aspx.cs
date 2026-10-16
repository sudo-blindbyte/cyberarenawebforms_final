using System;
using System.Collections.Generic;
using System.Linq;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminSubmissions : StaffPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindSubmissions();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindSubmissions();
        }

        private void BindSubmissions()
        {
            List<SubmissionRecord> all = SubmissionStore.All();

            lblTotal.Text = all.Count.ToString();
            lblAccepted.Text = all.Count(s => s.Status == "Accepted" || s.Status == "Verified").ToString();
            lblRejected.Text = all.Count(s => s.Status == "Rejected").ToString();
            lblRevoked.Text = all.Count(s => s.Status == "Revoked").ToString();

            string search = txtSearch.Text.Trim();
            IEnumerable<SubmissionRecord> query = all;
            if (search.Length > 0)
            {
                query = query.Where(s =>
                    s.Id.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.User.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.Challenge.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (ddlStatus.SelectedValue != "All") query = query.Where(s => s.Status == ddlStatus.SelectedValue);

            List<SubmissionRecord> rows = query.ToList();
            rptSubmissions.DataSource = rows;
            rptSubmissions.DataBind();
            pnlEmpty.Visible = rows.Count == 0;
        }
    }
}
