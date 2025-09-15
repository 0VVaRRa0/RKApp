using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Payment
{
    public partial class MyInvoices : System.Web.UI.Page
    {
        private HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "http://localhost:8000/api";
        private ClientDto user;
        private List<InvoiceDto> invoices;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] == null) Response.Redirect("Login");

            user = Session["user"] as ClientDto;

            invoices = GetInvoices();

            if (invoices != null)
            {
                invoicesGV.DataSource = invoices;
                invoicesGV.DataBind();
            }
            else Console.WriteLine("Не удалось получить данные");

            if (IsPostBack)
            {
                string eventTarget = Request["__EVENTTARGET"];
                if (eventTarget == "refreshInvoices")
                {
                    GetInvoices();
                }
            }
        }

        protected List<InvoiceDto> GetInvoices()
        {
            var response = httpClient
                .GetAsync($"{ApiUrl}/invoices?clientId={user.Id}")
                .GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var objects = JsonConvert.DeserializeObject<List<InvoiceDto>>(json);
            return objects;
        }

        protected void invoicesGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = (int)invoicesGV.DataKeys[invoicesGV.SelectedIndex].Value;
            var invoice = invoices.First(inv => inv.Id == id);
            Session["invoice"] = invoice;
            Response.Redirect("InvoiceDetail", true);
        }

        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session["user"] = null;
            Response.Redirect("Login", true);
        }

        protected void filterBtn_Click(object sender, EventArgs e)
        {
            var queryParams = new List<string>
                { $"clientId={user.Id}" };

            if (!string.IsNullOrEmpty(issueDate.Text))
                queryParams.Add($"issueDate={issueDate.Text}");

            if (!string.IsNullOrEmpty(paymentDate.Text))
                queryParams.Add($"paymentDate={paymentDate.Text}");

            if (status.SelectedValue == "1")
                queryParams.Add($"status=true");
            else if (status.SelectedValue == "2")
                queryParams.Add($"status=false");

            if (!string.IsNullOrEmpty(serviceTB.Text.Trim()))
                queryParams.Add($"serviceName={serviceTB.Text.Trim()}");

            string urlParams = "?" + string.Join("&", queryParams);

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"{ApiUrl}/invoices/{urlParams}").GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
                Response.Write(response.StatusCode.ToString());

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json);

            invoicesGV.DataSource = invoices;
            invoicesGV.DataBind();
        }
    }
}