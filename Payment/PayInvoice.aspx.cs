using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;

namespace Payment
{
    public partial class PayInvoice : System.Web.UI.Page
    {
        private const string ApiUrl = "http://localhost:8000/api";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["invoice"] == null) Response.Redirect("MyInvoices");
        }

        protected void sendBtn_Click(object sender, EventArgs e)
        {
            receiptNumberTB.BorderColor = Color.Black;
            if (string.IsNullOrEmpty(receiptNumberTB.Text.Trim()))
            { receiptNumberTB.BorderColor = Color.Red; return; }

            var invoice = Session["invoice"] as InvoiceDto;
            if (invoice == null) return;

            invoice.PaymentDate = DateTime.Today;
            invoice.Status = true;
            invoice.ReceiptNumber = receiptNumberTB.Text;

            SendRequest(invoice);
        }

        protected void SendRequest(InvoiceDto invoice)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(invoice);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = httpClient
                .PutAsync($"{ApiUrl}/invoices/{invoice.Id}", content)
                .GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var err = response.Content
                    .ReadAsStringAsync()
                    .GetAwaiter().GetResult();

                Response.Write($"Error: {response.StatusCode}<br/>");
                Response.Write($"Server response: \"{err}\"");
                return;
            }
            Response.Redirect("InvoiceDetail", true);
        }

        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session["user"] = null;
            Response.Redirect("Login", true);
        }

        protected void myInvBtn_Click(object sender, EventArgs e)
        {
            Session["invoice"] = null;
            Response.Redirect("MyInvoices", true);
        }
    }
}