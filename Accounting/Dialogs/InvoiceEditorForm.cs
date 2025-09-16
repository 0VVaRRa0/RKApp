using System.ComponentModel;
using System.Net.Http.Json;
using Accounting.Dtos;
using Accounting.Validation;

namespace Accounting.Dialogs;

class InvoiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private const string ApiUrl = "http://localhost:8000/api";
    private ErrorProvider _errorProvider = new();
    InvoiceValidation validator = new();
    private Size windowSize = new(683, 384);
    private readonly InvoiceDto _invoice = null!;
    private readonly BindingList<ServiceDto> _services = null!;
    private readonly BindingList<ClientDto> _clients = null!;
    private NumericUpDown invoiceAmount = null!;
    private DateTimePicker invoiceIssueDate = null!;
    private DateTimePicker invoiceDueDate = null!;
    private ComboBox servicesComboBox = null!;
    private ComboBox clientsComboBox = null!;
    private Label invoicePaymentDate = null!;
    private Label invoiceStatus = null!;
    public InvoiceEditorForm(BindingList<ServiceDto> services, BindingList<ClientDto> clients, InvoiceDto? invoice = null)
    {
        Text = "Счёт";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _invoice = invoice ?? new InvoiceDto { Status = false };
        _services = services;
        _clients = clients;
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
            deleteButton.Click += DeleteInvoice;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            saveButton.Click += EditInvoice;
        }
        else
        {
            saveButton.Text = "Добавить";
            saveButton.Click += AddInvoice;
        }

        TableLayoutPanel invoiceTable = new();
        invoiceTable.Dock = DockStyle.Fill;
        invoiceTable.ColumnCount = 2;
        invoiceTable.RowCount = 8;
        invoiceTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        servicesComboBox = new();
        servicesComboBox.DisplayMember = "Name";
        servicesComboBox.ValueMember = "Id";
        servicesComboBox.DataSource = _services;
        servicesComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        servicesComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        servicesComboBox.Width = 400;
        if (_invoice.Service != null)
            Shown += (s, e) => { servicesComboBox.SelectedValue = _invoice.Service.Id; };
        else Shown += (s, e) => { servicesComboBox.SelectedIndex = -1; };

        clientsComboBox = new();
        clientsComboBox.DisplayMember = "Login";
        clientsComboBox.ValueMember = "Id";
        clientsComboBox.DataSource = _clients;
        clientsComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        clientsComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        clientsComboBox.Width = 400;
        if (_invoice.Client != null)
            Shown += (s, e) => { clientsComboBox.SelectedValue = _invoice.Client.Id; };
        else Shown += (s, e) => { clientsComboBox.SelectedIndex = -1; };

        invoiceAmount = new();
        invoiceAmount.Minimum = 0;
        invoiceAmount.Maximum = decimal.MaxValue;
        invoiceAmount.DecimalPlaces = 2;
        invoiceAmount.Value = _invoice.Amount;

        invoiceIssueDate = new();
        invoiceIssueDate.Format = DateTimePickerFormat.Short;
        invoiceIssueDate.Value = DateTime.Today;
        invoiceIssueDate.MinDate = DateTime.Today;
        if (_invoice.Id != 0)
        {
            invoiceIssueDate.MinDate = _invoice.IssueDate;
            invoiceIssueDate.Value = _invoice.IssueDate;
        }
        invoiceIssueDate.ValueChanged += ChangeMinDueDate;

        invoiceDueDate = new();
        invoiceDueDate.Format = DateTimePickerFormat.Short;
        invoiceDueDate.Value = DateTime.Today;
        invoiceDueDate.MinDate = DateTime.Today;
        if (_invoice.Id != 0)
        {
            invoiceDueDate.MinDate = _invoice.IssueDate;
            invoiceDueDate.Value = _invoice.DueDate;
        }

        invoicePaymentDate = new();
        if (_invoice.PaymentDate is null) invoicePaymentDate.Text = "--.--.----";
        else invoicePaymentDate.Text = _invoice.PaymentDate.ToString();

        invoiceStatus = new();
        if (!_invoice.Status) invoiceStatus.Text = "Не оплачено";
        else invoiceStatus.Text = "Оплачено";

        invoiceTable.Controls.Add(servicesComboBox, 1, 0);
        invoiceTable.Controls.Add(clientsComboBox, 1, 1);
        invoiceTable.Controls.Add(invoiceAmount, 1, 2);
        invoiceTable.Controls.Add(invoiceIssueDate, 1, 3);
        invoiceTable.Controls.Add(invoiceDueDate, 1, 4);
        invoiceTable.Controls.Add(invoicePaymentDate, 1, 5);
        invoiceTable.Controls.Add(invoiceStatus, 1, 6);

        invoiceTable.Controls.Add(CreateLabel("Услуга:"), 0, 0);
        invoiceTable.Controls.Add(CreateLabel("Клиент:"), 0, 1);
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

    private void ChangeMinDueDate(object? sender, EventArgs e)
    {
        invoiceDueDate.MinDate = invoiceIssueDate.Value;
    }
    
    private async void AddInvoice(object? sender, EventArgs e)
    {
        if (!validator.Validate(servicesComboBox, clientsComboBox, invoiceAmount))
            return;

        _invoice.ServiceId = (int)servicesComboBox.SelectedValue!;
        _invoice.ClientId = (int)clientsComboBox.SelectedValue!;
        _invoice.Amount = invoiceAmount.Value;
        _invoice.IssueDate = invoiceIssueDate.Value.Date;
        _invoice.DueDate = invoiceDueDate.Value.Date;
        var response = await httpClient.PostAsJsonAsync($"{ApiUrl}/invoices", _invoice);
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Счёт добавлен", "Успех✅");
            CloseWindowOk();
        }
    }

    private async void EditInvoice(object? sender, EventArgs e)
    {
        if (!validator.Validate(servicesComboBox, clientsComboBox, invoiceAmount))
            return;

        _invoice.ServiceId = (int)servicesComboBox.SelectedValue!;
        _invoice.ClientId = (int)clientsComboBox.SelectedValue!;
        _invoice.Amount = invoiceAmount.Value;
        _invoice.IssueDate = invoiceIssueDate.Value.Date;
        _invoice.DueDate = invoiceDueDate.Value.Date;

        var response = await httpClient.PutAsJsonAsync($"{ApiUrl}/invoices/{_invoice.Id}", _invoice);
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Счёт изменён", "Успех✅");
            CloseWindowOk();
        }
    }

    private async void DeleteInvoice(object? sender, EventArgs e)
    {
        var response = await httpClient.DeleteAsync($"{ApiUrl}/invoices/{_invoice.Id}");
        if (!await CheckResponse(response)) return;
        else
        {
            MessageBox.Show($"Счёт удалён", "Успех✅");
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
