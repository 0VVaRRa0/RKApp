using Newtonsoft.Json;
using Payment.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Payment
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "http://localhost:8000/api";
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            wrongLogin.Visible = false;
            string login = loginTB.Text.Trim();

            if (string.IsNullOrEmpty(login))
            { wrongLogin.Visible = true; return; }

            var response = httpClient
                .GetAsync($"{ApiUrl}/clients?enteredLogin={login}")
                .GetAwaiter()
                .GetResult();

            if (!response.IsSuccessStatusCode) return;

            var json = response.Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            var userList = JsonConvert.DeserializeObject<List<ClientDto>>(json);
            if (userList.Count == 0) return;
            var user = userList[0];
            Session["user"] = user;

            if (Session["user"] is ClientDto)
                Response.Redirect("MyInvoices", true);
            else return;
        }
    }
}