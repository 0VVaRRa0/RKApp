using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.UI.WebControls;

namespace Payment
{
    public partial class Invoices : System.Web.UI.Page
    {
        private List<InvoiceDto> invoices;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var client = Session["client"] as ClientDto;
                if (client == null) Response.Redirect("Login");

                var httpClient = new HttpClient();
                var response = httpClient
                    .GetAsync($"http://localhost:8000/api/invoices?clientId={client.Id}")
                    .GetAwaiter().GetResult();
                var jsonInvoices = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(jsonInvoices);

                if (invoices.Count == 0) return;

                EmptyInvoices.Visible = false;
                InvoicesGV.DataSource = invoices;
                InvoicesGV.DataBind();

                ViewState["Invoices"] = invoices;
            }
            else
            {
                invoices = ViewState["Invoices"] as List<InvoiceDto>;
            }
        }

        protected void InvoicesGV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var invoice = e.Row.DataItem as InvoiceDto;
                if (invoice != null)
                {
                    if (!invoice.PaymentDate.HasValue && invoice.DueDate.Date < DateTime.Today)
                    {
                        e.Row.BackColor = System.Drawing.Color.LightCoral;
                    }
                }
            }
        }

        protected void InvoicesGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = InvoicesGV.SelectedIndex;
            int invoiceId = (int)InvoicesGV.DataKeys[index].Value;

            var invoice = invoices.FirstOrDefault(inv => inv.Id == invoiceId);
            if (invoice != null)
            {
                Session["selectedInvoice"] = invoice;
                Response.Redirect("InvoiceDetail");
            }
        }

        protected void FilterBtn_Click(object sender, EventArgs e)
        {
            var invoices = ViewState["Invoices"] as List<InvoiceDto>;

            if (DateTime.TryParse(DueDateFilter.Value, out var dueDate))
            {
                if (dueDate == null) return;
                invoices = invoices.Where(
                    inv => inv.DueDate.Date == dueDate.Date).ToList();
            }

            if (DateTime.TryParse(PaymentDateFilter.Value, out var paymentDate) && paymentDate != null)
            {
                invoices = invoices.Where(
                    inv => inv.PaymentDate.HasValue && inv.PaymentDate?.Date == paymentDate.Date).ToList();
            }

            if (!string.IsNullOrEmpty(ServiceNameFilter.Text))
                invoices = invoices.Where(
                    inv => inv.ServiceName.ToLower().Contains(ServiceNameFilter.Text)).ToList();

            if (StatusFilter.Text != "")
            {
                if (StatusFilter.Text == "PAID")
                    invoices = invoices.Where(inv => inv.Status == "PAID").ToList();
                else if (StatusFilter.Text == "NOT PAID")
                    invoices = invoices.Where(inv => inv.Status == "NOT PAID").ToList();
            }

            InvoicesGV.DataSource = invoices;
            InvoicesGV.DataBind();
        }

        protected void ClrFilterBtn_Click(object sender, EventArgs e)
        {
            DueDateFilter.Value = "";
            PaymentDateFilter.Value = "";
            ServiceNameFilter.Text = "";
            StatusFilter.Text = "";
            Response.Redirect("Invoices");
        }
    }
}