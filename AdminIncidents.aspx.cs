using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public partial class AdminIncidents : StaffPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindAll();
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindAll();
        }

        protected void rptIncidents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            Incident incident = (Incident)e.Item.DataItem;
            e.Item.FindControl("btnInvestigate").Visible = incident.Status == "Open";
            e.Item.FindControl("btnResolve").Visible = incident.Status != "Resolved";
            e.Item.FindControl("btnReopen").Visible = incident.Status == "Resolved";
        }

        protected void rptIncidents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = Convert.ToString(e.CommandArgument);
            string status = e.CommandName;
            if (status != "Open" && status != "Investigating" && status != "Resolved") return;

            if (IncidentStore.SetStatus(id, status))
            {
                AuditLog.Write(CurrentUser.Username, "Incident " + status.ToLowerInvariant(), id, Context);
                lblPageMessage.Text = Server.HtmlEncode("Incident " + id + " marked as " + status.ToLowerInvariant() + ".");
                lblPageMessage.CssClass = "message success";
            }
            BindAll();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            AuditLog.Write(CurrentUser.Username, "Exported audit log", "CSV download", Context);

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Disposition", "attachment; filename=\"audit-log.csv\"");
            Response.Write(AuditLog.ToCsv());
            Response.End();
        }

        private void BindAll()
        {
            List<Incident> all = IncidentStore.All();

            lblTotal.Text = all.Count.ToString();
            lblOpen.Text = all.Count(i => i.Status != "Resolved").ToString();
            lblCritical.Text = all.Count(i => i.Severity == "Critical" && i.Status != "Resolved").ToString();
            lblResolved.Text = all.Count(i => i.Status == "Resolved").ToString();

            string search = txtSearch.Text.Trim();
            IEnumerable<Incident> query = all;
            if (search.Length > 0)
            {
                query = query.Where(i =>
                    i.Id.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    i.Reporter.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    i.Subject.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (ddlStatus.SelectedValue != "All") query = query.Where(i => i.Status == ddlStatus.SelectedValue);

            List<Incident> rows = query.ToList();
            rptIncidents.DataSource = rows;
            rptIncidents.DataBind();
            pnlEmpty.Visible = rows.Count == 0;

            rptAudit.DataSource = AuditLog.Recent(12);
            rptAudit.DataBind();
        }
    }
}
