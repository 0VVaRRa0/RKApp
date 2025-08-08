using System.Net.Http.Json;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class ServiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "https://63b1473a2aa1.ngrok-free.app";
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

    private async void SendRequest(object? sender, EventArgs e)
    {
        if (sender is not Button clickedButton) return;
        string messageBoxText = "";
        HttpResponseMessage? response = null;
        if (clickedButton.Text == "Добавить")
        {
            if (textBox.Text == "") { MessageBox.Show("Заполните все поля!", "Ошибка⚠️"); return; }
            _service.Name = textBox.Text;
            messageBoxText = "Услуга добавлена!";
            response = await httpClient.PostAsJsonAsync($"{apiUrl}/api/services", _service);
        }
        else if (clickedButton.Text == "Сохранить")
        {
            if (textBox.Text == "") { MessageBox.Show("Заполните все поля!", "Ошибка⚠️"); return; }
            _service.Name = textBox.Text;
            messageBoxText = "Услуга изменена!";
            response = await httpClient.PutAsJsonAsync($"{apiUrl}/api/services/{_service.Id}", _service);
        }
        else if (clickedButton.Text == "Удалить")
        {
            messageBoxText = "Услуга удалена!";
            response = await httpClient.DeleteAsync($"{apiUrl}/api/services/{_service.Id}");
        }
        if (response!.IsSuccessStatusCode) MessageBox.Show(messageBoxText, "Успех✅");
        DialogResult = DialogResult.OK;
        Close();
    }
}
