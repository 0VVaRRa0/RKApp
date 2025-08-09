using System.Net.Http.Json;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class InvoiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "https://81951d3b8c90.ngrok-free.app";
    private Size windowSize = new(683, 384);
    private readonly InvoiceDto _invoice = null!;
    private NumericUpDown invoiceServiceID = null!;
    private NumericUpDown invoiceClientID = null!;
    private NumericUpDown invoiceAmount = null!;
    private DateTimePicker invoiceIssueDate = null!;
    private DateTimePicker invoiceDueDate = null!;
    private Label invoicePaymentDate = null!;
    private Label invoiceStatus = null!;
    public InvoiceEditorForm(InvoiceDto? invoice = null)
    {
        Text = "Счёт";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _invoice = invoice ?? new InvoiceDto { Status = "NOT PAID" };
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

        if (_invoice.Id != 0)
        {
            Button deleteButton = new();
            deleteButton.Text = "Удалить";
            deleteButton.Click += SendRequest;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            // saveButton.Click += SendRequest;
        }
        else
        {
            saveButton.Text = "Добавить";
            saveButton.Click += SendRequest;
        }

        TableLayoutPanel invoiceTable = new();
        invoiceTable.Dock = DockStyle.Fill;
        invoiceTable.ColumnCount = 2;
        invoiceTable.RowCount = 8;
        invoiceTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        invoiceServiceID = new();
        invoiceServiceID.Minimum = 0;
        invoiceServiceID.Increment = 1;
        if (_invoice.Id != 0) invoiceServiceID.Value = _invoice.ServiceId;

        invoiceClientID = new();
        invoiceClientID.Minimum = 0;
        invoiceClientID.Increment = 1;
        if (_invoice.Id != 0) invoiceClientID.Value = _invoice.ClientId;

        invoiceAmount = new();
        invoiceAmount.Minimum = 0;
        invoiceAmount.Maximum = decimal.MaxValue;
        invoiceAmount.DecimalPlaces = 2;

        invoiceIssueDate = new();
        invoiceIssueDate.Format = DateTimePickerFormat.Short;
        invoiceIssueDate.Value = DateTime.Today;
        invoiceIssueDate.MinDate = DateTime.Today;
        if (_invoice.Id != 0)
        {
            invoiceIssueDate.MinDate = _invoice.IssueDate.ToDateTime(TimeOnly.MaxValue);
            invoiceIssueDate.Value = _invoice.IssueDate.ToDateTime(TimeOnly.MaxValue);
        }

        invoiceDueDate = new();
        invoiceDueDate.Format = DateTimePickerFormat.Short;
        invoiceDueDate.Value = DateTime.Today;
        invoiceDueDate.MinDate = DateTime.Today;
        if (_invoice.Id != 0)
        {
            invoiceDueDate.MinDate = _invoice.IssueDate.ToDateTime(TimeOnly.MaxValue);
            invoiceDueDate.Value = _invoice.DueDate.ToDateTime(TimeOnly.MaxValue);
        }

        invoicePaymentDate = new();
        if (_invoice.PaymentDate is null) invoicePaymentDate.Text = "--.--.----";
        else invoicePaymentDate.Text = _invoice.PaymentDate.ToString();

        invoiceStatus = new();
        if (_invoice.Status == "NOT PAID") invoiceStatus.Text = "NOT PAID";
        else invoiceStatus.Text = "PAID";

        invoiceTable.Controls.Add(invoiceServiceID, 1, 0);
        invoiceTable.Controls.Add(invoiceClientID, 1, 1);
        invoiceTable.Controls.Add(invoiceAmount, 1, 2);
        invoiceTable.Controls.Add(invoiceIssueDate, 1, 3);
        invoiceTable.Controls.Add(invoiceDueDate, 1, 4);
        invoiceTable.Controls.Add(invoicePaymentDate, 1, 5);
        invoiceTable.Controls.Add(invoiceStatus, 1, 6);

        invoiceTable.Controls.Add(CreateLabel("ID Услуги:"), 0, 0);
        invoiceTable.Controls.Add(CreateLabel("ID Клиента:"), 0, 1);
        invoiceTable.Controls.Add(CreateLabel("Сумма:"), 0, 2);
        invoiceTable.Controls.Add(CreateLabel("Дата выставления:"), 0, 3);
        invoiceTable.Controls.Add(CreateLabel("Оплатить до:"), 0, 4);
        invoiceTable.Controls.Add(CreateLabel("Дата оплаты:"), 0, 5);
        invoiceTable.Controls.Add(CreateLabel("Статус:"), 0, 6);

        table.Controls.Add(buttonsPanel, 0, 0);
        table.Controls.Add(invoiceTable, 0, 1);
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
            messageBoxText = "Счёт добавлен!";
            response = await httpClient.PostAsJsonAsync($"{apiUrl}/api/invoices", _invoice);
        }
        else if (clickedButton.Text == "Сохранить")
        {
            messageBoxText = "Счёт изменён!";
            response = await httpClient.PutAsJsonAsync($"{apiUrl}/api/invoices/{_invoice.Id}", _invoice);
        }
        else if (clickedButton.Text == "Удалить")
        {
            messageBoxText = "Счёт удалён!";
            response = await httpClient.DeleteAsync($"{apiUrl}/api/invoices/{_invoice.Id}");
        }

        if (response != null && response.IsSuccessStatusCode)
        {
            MessageBox.Show(messageBoxText, "Успех✅");
            DialogResult = DialogResult.OK;
            Close();
        }
        else if (response is null) MessageBox.Show("Неизвестная кнопка", "Ошибка⚠️");
        else if (!response.IsSuccessStatusCode) MessageBox.Show("Не удалось отправить запрос🔌", "Ошибка⚠️");
    }

    private bool CheckFields()
    {
        if (
            invoiceServiceID.Value == 0
            || invoiceClientID.Value == 0
            || invoiceAmount.Value == 0
        )
        {
            MessageBox.Show("Заполните все поля!", "Ошибка⚠️");
            return false;
        }
        else
        {
            _invoice.ServiceId = Convert.ToInt32(invoiceServiceID.Value);
            _invoice.ClientId = Convert.ToInt32(invoiceClientID.Value);
            _invoice.Amount = invoiceAmount.Value;
            _invoice.IssueDate = DateOnly.FromDateTime(invoiceIssueDate.Value);
            _invoice.DueDate = DateOnly.FromDateTime(invoiceDueDate.Value);
            return true;
        }
    }
}
