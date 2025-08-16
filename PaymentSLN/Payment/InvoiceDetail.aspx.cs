using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Net.Http;
using System.Text;

namespace Payment
{
    public partial class InvoiceDetail : System.Web.UI.Page
    {
        public InvoiceDto invoice;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["client"] == null) Response.Redirect("Login");
                invoice = Session["selectedInvoice"] as InvoiceDto;
                if (invoice == null) Response.Redirect("Invoices");
                if (invoice.Status == "NOT PAID")
                {
                    ReceiptNumber.ReadOnly = false;
                    PayBtn.Visible = true;
                }
                else ReceiptNumber.Text = invoice.ReceiptNumber;
            }
        }

        protected void PayBtn_Click(object sender, EventArgs e)
        {
            EmptyRN.Visible = false;

            invoice = Session["selectedInvoice"] as InvoiceDto;

            if (invoice == null) return;
            if (string.IsNullOrEmpty(ReceiptNumber.Text))
            { EmptyRN.Visible = true; return; }

            invoice.Status = "PAID";
            invoice.PaymentDate = DateTime.Today;
            invoice.ReceiptNumber = ReceiptNumber.Text;

            var jsonInvoice = JsonConvert.SerializeObject(invoice);
            var content = new StringContent(jsonInvoice, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var response = httpClient
                .PutAsync($"http://localhost:8000/api/invoices/{invoice.Id}", content)
                .GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode) Response.Redirect("Invoices");
        }
    }
}