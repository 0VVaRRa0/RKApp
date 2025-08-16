using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Net.Http;

namespace Payment
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTB.Text))
            {
                LoginWarn.Visible = true;
                return;
            }

            LoginWarn.Visible = false;

            var httpClient = new HttpClient();
            var response = httpClient
                .GetAsync(
                $"http://localhost:8000/api/clients?clientLogin={LoginTB.Text}"
                )
                .GetAwaiter().GetResult();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                LoginWarn.Visible = true;
                return;
            }

            var jsonClient = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var client = JsonConvert.DeserializeObject<ClientDto>(jsonClient);

            Session["client"] = client;

            Response.Redirect("Invoices");
        }

    }
}
