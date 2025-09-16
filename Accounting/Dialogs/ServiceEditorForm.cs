using System.Net.Http.Json;
using Accounting.Dtos;
using Accounting.Validation;

namespace Accounting.Dialogs;

class ServiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private const string ApiUrl = "http://localhost:8000/api";
    private readonly ErrorProvider _errProvider = new();
    private ServiceValidation validator = new();
    private Size windowSize = new(683, 384);
    private readonly ServiceDto _service = null!;
    TextBox textBox = null!;
    public ServiceEditorForm(ServiceDto? service = null)
    {
        Text = "Услуга";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _service = service ?? new ServiceDto();
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

        if (_service.Id != 0)
        {
            Button deleteButton = new();
            deleteButton.Text = "Удалить";
            deleteButton.Click += DeleteService;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            saveButton.Click += EditService;
        }
        else
        {
            saveButton.Text = "Добавить";
            saveButton.Click += AddService;
        }

        TableLayoutPanel serviceTable = new();
        serviceTable.Dock = DockStyle.Fill;
        serviceTable.ColumnCount = 2;
        serviceTable.RowCount = 2;
        serviceTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        Label label = new();
        label.Text = "Название услуги:";
        label.AutoSize = true;
        label.Anchor = AnchorStyles.None;

        textBox = new();
        textBox.Text = _service.Name;
        textBox.Width = 400;
        textBox.Anchor = AnchorStyles.Left;

        serviceTable.Controls.Add(label, 0, 0);
        serviceTable.Controls.Add(textBox, 1, 0);
        table.Controls.Add(buttonsPanel, 0, 0);
        table.Controls.Add(serviceTable, 0, 1);
        Controls.Add(table);
    }

    private async void AddService(object? sender, EventArgs e)
    {
        if (!validator.Validate(textBox))
            return;

        _service.Name = textBox.Text;
        var response = await httpClient.PostAsJsonAsync($"{ApiUrl}/services", _service);
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Услуга \"{_service.Name}\" добавлена", "Успех✅");
            CloseWindowOk();
        }
    }

    private async void EditService(object? sender, EventArgs e)
    {
        if (!validator.Validate(textBox))
            return;

        _service.Name = textBox.Text;
        var response = await httpClient.PutAsJsonAsync($"{ApiUrl}/services/{_service.Id}", _service);
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Услуга \"{_service.Name}\" изменена", "Успех✅");
            CloseWindowOk();
        }
    }

    private async void DeleteService(object? sender, EventArgs e)
    {
        var response = await httpClient.DeleteAsync($"{ApiUrl}/services/{_service.Id}");
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Услуга \"{_service.Name}\" удалена", "Успех✅");
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
