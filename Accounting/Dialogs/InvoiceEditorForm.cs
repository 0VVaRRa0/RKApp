using System.Net.Http.Json;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class InvoiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "http://localhost:8000";
    private Size windowSize = new(683, 384);
    private readonly InvoiceDto _invoice = null!;
    private readonly DataGridView _servicesDataGrid = null!;
    private readonly DataGridView _clientsDataGrid = null!;
    private NumericUpDown invoiceAmount = null!;
    private DateTimePicker invoiceIssueDate = null!;
    private DateTimePicker invoiceDueDate = null!;
    private ComboBox servicesComboBox = null!;
    private ComboBox clientsComboBox = null!;
    private Label invoicePaymentDate = null!;
    private Label invoiceStatus = null!;
    public InvoiceEditorForm(DataGridView servicesDataGrid, DataGridView clientsDataGrid, InvoiceDto? invoice = null)
    {
        Text = "Счёт";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _invoice = invoice ?? new InvoiceDto { Status = "NOT PAID" };
        _servicesDataGrid = servicesDataGrid;
        _clientsDataGrid = clientsDataGrid;
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
            saveButton.Click += SendRequest;
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

        var serviceDtos = _servicesDataGrid.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.DataBoundItem != null)
            .Select(r => (ServiceDto)r.DataBoundItem!)
            .ToList();

        var detailsServiceDtos = serviceDtos
            .Select(s => new ServiceDetailDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToList();

        servicesComboBox = new();
        servicesComboBox.DisplayMember = "DisplayName";
        servicesComboBox.ValueMember = "Id";
        servicesComboBox.DataSource = detailsServiceDtos;
        servicesComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        servicesComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        servicesComboBox.Width = 400;
        Shown += (s, e) => { servicesComboBox.SelectedValue = _invoice.ServiceId; };
        
        var clientDtos = _clientsDataGrid.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.DataBoundItem != null)
            .Select(r => (ClientDto)r.DataBoundItem!)
            .ToList();

        var detailsClientDtos = clientDtos
            .Select(s => new ClientDetailDto
            {
                Id = s.Id,
                Login = s.Login,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber
            })
            .ToList();

        clientsComboBox = new();
        clientsComboBox.DisplayMember = "DisplayName";
        clientsComboBox.ValueMember = "Id";
        clientsComboBox.DataSource = detailsClientDtos;
        clientsComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        clientsComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        clientsComboBox.Width = 400;
        Shown += (s, e) => { clientsComboBox.SelectedValue = _invoice.ClientId; };

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
        if (_invoice.Status == "NOT PAID") invoiceStatus.Text = "NOT PAID";
        else invoiceStatus.Text = "PAID";

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
        else if (!response.IsSuccessStatusCode)
            MessageBox.Show(
                $"Не удалось отправить запрос: {await response.Content.ReadAsStringAsync()}", "Ошибка⚠️"
            );
    }

    private bool CheckFields()
    {
        var selectedServiceId = servicesComboBox.SelectedValue;
        var selectedClientId = clientsComboBox.SelectedValue;
        if (
            invoiceAmount.Value == 0
            || selectedServiceId == null
            || selectedClientId == null
        )
        {
            MessageBox.Show("Заполните все поля!", "Ошибка⚠️");
            return false;
        }
        else
        {
            _invoice.ServiceId = (int)selectedServiceId;
            _invoice.ClientId = (int)selectedClientId;
            _invoice.Amount = invoiceAmount.Value;
            _invoice.IssueDate = invoiceIssueDate.Value;
            _invoice.DueDate = invoiceDueDate.Value;
            return true;
        }
    }
}
