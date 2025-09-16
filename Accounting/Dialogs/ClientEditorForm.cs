using System.Net.Http.Json;
using Accounting.Validation;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class ClientEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private const string ApiUrl = "http://localhost:8000/api";
    private ClientValidation validator = new();
    private Size windowSize = new(683, 384);
    private readonly ClientDto _client = null!;
    TextBox clientLoginTB = null!;
    TextBox clientFullNameTB = null!;
    TextBox clientEmailTB = null!;
    TextBox clientPhoneTB = null!;
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
            deleteButton.Click += DeleteClient;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            saveButton.Click += EditClient;
        }
        else
        {
            saveButton.Text = "Добавить";
            saveButton.Click += AddClient;
        }

        TableLayoutPanel clientTable = new();
        clientTable.Dock = DockStyle.Fill;
        clientTable.ColumnCount = 2;
        clientTable.RowCount = 5;
        clientTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        clientLoginTB = CreateTextBox(_client.Login);
        clientFullNameTB = CreateTextBox(_client.FullName);
        clientEmailTB = CreateTextBox(_client.Email);
        clientPhoneTB = CreateTextBox(_client.Phone);

        clientTable.Controls.Add(CreateLabel("Логин:"), 0, 0);
        clientTable.Controls.Add(CreateLabel("Имя:"), 0, 1);
        clientTable.Controls.Add(CreateLabel("Email:"), 0, 2);
        clientTable.Controls.Add(CreateLabel("Номер телефона:"), 0, 3);

        clientTable.Controls.Add(clientLoginTB, 1, 0);
        clientTable.Controls.Add(clientFullNameTB, 1, 1);
        clientTable.Controls.Add(clientEmailTB, 1, 2);
        clientTable.Controls.Add(clientPhoneTB, 1, 3);

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

    private async void AddClient(object? sender, EventArgs e)
    {
        if (!validator.Validate(clientLoginTB, clientFullNameTB, clientEmailTB, clientPhoneTB))
            return;

        _client.Login = clientLoginTB.Text;
        _client.FullName = clientFullNameTB.Text;
        _client.Email = clientEmailTB.Text;
        _client.Phone = clientPhoneTB.Text;
        var response = await httpClient.PostAsJsonAsync($"{ApiUrl}/clients", _client);
        if (!await CheckResponse(response)) return;
        MessageBox.Show($"Клиент \"{_client.Login}\" добавлен", "Успех✅");
        CloseWindowOk();
    }

    private async void EditClient(object? sender, EventArgs e)
    {
        if (!validator.Validate(clientLoginTB, clientFullNameTB, clientEmailTB, clientPhoneTB))
            return;

        _client.Login = clientLoginTB.Text;
        _client.FullName = clientFullNameTB.Text;
        _client.Email = clientEmailTB.Text;
        _client.Phone = clientPhoneTB.Text;
        var response = await httpClient.PutAsJsonAsync($"{ApiUrl}/clients/{_client.Id}", _client);
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Клиент \"{_client.Login}\" изменён", "Успех✅");
            CloseWindowOk();
        }
    }

    private async void DeleteClient(object? sender, EventArgs e)
    {
        var response = await httpClient.DeleteAsync($"{ApiUrl}/clients/{_client.Id}");
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Клиент \"{_client.Login}\" удалён", "Успех✅");
            CloseWindowOk();
        }
    }

    private async Task<bool> CheckResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<ValidationErrors>();
            var errorsToDisplay = string.Join("\n", err!.Errors.Values.SelectMany(x => x).ToList());
            MessageBox.Show($"Ошибка: {response.StatusCode}\n{errorsToDisplay}", "Ошибка");
            return false;
        }
        return true;
    }

    private void CloseWindowOk()
    {
        DialogResult = DialogResult.OK;
        Close();
    }
}
