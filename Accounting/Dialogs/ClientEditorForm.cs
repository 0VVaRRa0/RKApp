using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class ClientEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "http://localhost:8000";
    private Size windowSize = new(683, 384);
    private readonly ClientDto _client = null!;
    TextBox clientLoginTB = null!;
    TextBox clientFullNameTB = null!;
    TextBox clientEmailTB = null!;
    TextBox clientPhoneNumberTB = null!;
    public ClientEditorForm(ClientDto? client = null)
    {
        Text = "Клиент";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _client = client ?? new ClientDto();
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        TableLayoutPanel table = new();
        table.Dock = DockStyle.Fill;
        table.ColumnCount = 1;
        table.RowCount = 2;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        FlowLayoutPanel buttonsPanel = new();
        buttonsPanel.Dock = DockStyle.Fill;
        buttonsPanel.FlowDirection = FlowDirection.LeftToRight;

        Button saveButton = new();
        buttonsPanel.Controls.Add(saveButton);

        if (_client.Id != 0)
        {
            Button deleteButton = new();
            deleteButton.Text = "Удалить";
            deleteButton.Click += SendRequest;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            saveButton.Click += SendRequest;
        }
        else
        {
            saveButton.Text = "Добавить";
            saveButton.Click += SendRequest;
        }

        TableLayoutPanel clientTable = new();
        clientTable.Dock = DockStyle.Fill;
        clientTable.ColumnCount = 2;
        clientTable.RowCount = 5;
        clientTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        clientLoginTB = CreateTextBox(_client.Login);
        clientFullNameTB = CreateTextBox(_client.FullName);
        clientEmailTB = CreateTextBox(_client.Email);
        clientPhoneNumberTB = CreateTextBox(_client.PhoneNumber);

        clientTable.Controls.Add(CreateLabel("Логин:"), 0, 0);
        clientTable.Controls.Add(CreateLabel("Имя:"), 0, 1);
        clientTable.Controls.Add(CreateLabel("Email:"), 0, 2);
        clientTable.Controls.Add(CreateLabel("Номер телефона:"), 0, 3);

        clientTable.Controls.Add(clientLoginTB, 1, 0);
        clientTable.Controls.Add(clientFullNameTB, 1, 1);
        clientTable.Controls.Add(clientEmailTB, 1, 2);
        clientTable.Controls.Add(clientPhoneNumberTB, 1, 3);

        table.Controls.Add(buttonsPanel, 0, 0);
        table.Controls.Add(clientTable, 0, 1);
        Controls.Add(table);
    }

    private Label CreateLabel(string text)
    {
        Label label = new();
        label.Text = text;
        label.AutoSize = true;
        label.Anchor = AnchorStyles.Right;
        return label;
    }

    private TextBox CreateTextBox(string text)
    {
        TextBox textBox = new();
        textBox.Text = text;
        textBox.Width = 400;
        textBox.Anchor = AnchorStyles.Left;
        return textBox;
    }

    private async void SendRequest(object? sender, EventArgs e)
    {
        if (sender is not Button clickedButton) return;
        if (!CheckFields() && clickedButton.Text != "Удалить") return;

        string messageBoxText = "";
        HttpResponseMessage? response = null;

        if (clickedButton.Text == "Добавить")
        {
            messageBoxText = "Клиент добавлен!";
            response = await httpClient.PostAsJsonAsync($"{apiUrl}/api/clients", _client);
        }
        else if (clickedButton.Text == "Сохранить")
        {
            messageBoxText = "Клиент изменён!";
            response = await httpClient.PutAsJsonAsync($"{apiUrl}/api/clients/{_client.Id}", _client);
        }
        else if (clickedButton.Text == "Удалить")
        {
            messageBoxText = "Клиент удалён!";
            response = await httpClient.DeleteAsync($"{apiUrl}/api/clients/{_client.Id}");
        }

        if (response != null && response.IsSuccessStatusCode)
        {
            MessageBox.Show(messageBoxText, "Успех✅");
            DialogResult = DialogResult.OK;
            Close();
        }

        else if (response is null) MessageBox.Show("Неизвестная кнопка", "Ошибка⚠️");

        else if (!response.IsSuccessStatusCode)
        {
            var error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            MessageBox.Show($"Ошибка: {error}", "Ошибка⚠️");
        }
    }

    private bool CheckFields()
    {
        var login = clientLoginTB.Text;
        var fullName = clientFullNameTB.Text;
        var email = clientEmailTB.Text;
        var phoneNumber = clientPhoneNumberTB.Text;
        if (
            string.IsNullOrEmpty(login)
            || string.IsNullOrEmpty(fullName)
            || string.IsNullOrEmpty(email)
            || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            || string.IsNullOrEmpty(phoneNumber)
            || !Regex.IsMatch(phoneNumber, @"^\+\d{8,15}$")
        )
        {
            MessageBox.Show("Заполните все поля правильно!", "Ошибка⚠️");
            return false;
        }
        else
        {
            _client.Login = clientLoginTB.Text;
            _client.FullName = clientFullNameTB.Text;
            _client.Email = clientEmailTB.Text;
            _client.PhoneNumber = clientPhoneNumberTB.Text;
            return true;
        }
    }
}
