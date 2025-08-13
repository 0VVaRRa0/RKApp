using Accounting.Dtos;

namespace Accounting.Dialogs;

public class FilterForm : Form
{
    private DateTimePicker issueDateDTP = null!;
    private DateTimePicker paymentDateDTP = null!;
    private TextBox serviceTB = null!;
    private TextBox clientTB = null!;
    private ComboBox statusCB = null!;
    private Size windowSize = new(650, 250);
    DataGridView _invoicesDG = null!;
    DataGridView _clientsDG = null!;
    DataGridView _servicesDG = null!;
    public FilterForm(DataGridView invoices, DataGridView services, DataGridView clients)
    {
        Text = "Фильтр";
        Size = windowSize;
        InitializeComponents();
        _invoicesDG = invoices;
        _servicesDG = services;
        _clientsDG = clients;
    }

    private void InitializeComponents()
    {
        TableLayoutPanel mainTable = new();
        mainTable.ColumnCount = 1;
        mainTable.RowCount = 2;
        mainTable.Dock = DockStyle.Fill;
        mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
        mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        mainTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        Button filterButton = new();
        filterButton.Text = "Фильтровать";
        filterButton.Width = 100;
        filterButton.Anchor = AnchorStyles.None;
        filterButton.Click += Filter;

        mainTable.Controls.Add(CreateFieldsTable());
        mainTable.Controls.Add(filterButton);
        Controls.Add(mainTable);
    }

    private TableLayoutPanel CreateFieldsTable()
    {
        TableLayoutPanel table = new();
        table.ColumnCount = 2;
        table.RowCount = 6;
        table.Dock = DockStyle.Fill;
        table.AutoSize = true;
        table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        issueDateDTP = CreatePicker();

        paymentDateDTP = CreatePicker();

        serviceTB = new();
        serviceTB.Dock = DockStyle.Fill;

        clientTB = new();
        clientTB.Dock = DockStyle.Fill;

        statusCB = new();
        statusCB.DropDownStyle = ComboBoxStyle.DropDownList;
        statusCB.Items.AddRange(["", "PAID", "NOT PAID"]);
        statusCB.SelectedIndex = 0;
        statusCB.Dock = DockStyle.Fill;

        table.Controls.Add(issueDateDTP, 1, 0);
        table.Controls.Add(paymentDateDTP, 1, 1);
        table.Controls.Add(serviceTB, 1, 2);
        table.Controls.Add(clientTB, 1, 3);
        table.Controls.Add(statusCB, 1, 4);

        table.Controls.Add(CreateFieldLabel("Дата выставления:"), 0, 0);
        table.Controls.Add(CreateFieldLabel("Дата оплаты:"), 0, 1);
        table.Controls.Add(CreateFieldLabel("Услуга:"), 0, 2);
        table.Controls.Add(CreateFieldLabel("Клиент:"), 0, 3);
        table.Controls.Add(CreateFieldLabel("Статус:"), 0, 4);
        return table;
    }

    private Label CreateFieldLabel(string labelName)
    {
        Label label = new();
        label.Text = labelName;
        label.AutoSize = true;
        label.Anchor = AnchorStyles.Right;
        return label;
    }

    private DateTimePicker CreatePicker()
    {
        DateTimePicker dateTimePicker = new();
        dateTimePicker.Dock = DockStyle.Fill;
        dateTimePicker.Format = DateTimePickerFormat.Short;
        dateTimePicker.ShowCheckBox = true;
        dateTimePicker.Checked = false;
        return dateTimePicker;
    }

    private void Filter(object? sender, EventArgs e)
    {
        var currentInvoices = _invoicesDG.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.DataBoundItem != null)
            .Select(r => (InvoiceDto)r.DataBoundItem!)
            .ToList();

        if (issueDateDTP.Checked)
        {
            var issueDate = issueDateDTP.Value.Date;
            currentInvoices = currentInvoices
                .Where(inv => inv.IssueDate.Date == issueDate)
                .ToList();
        }

        if (paymentDateDTP.Checked)
        {
            var paymentDate = paymentDateDTP.Value.Date;
            currentInvoices = currentInvoices
                .Where(inv => inv.PaymentDate.HasValue && inv.PaymentDate.Value.Date == paymentDate)
                .ToList();
        }

        string serviceFilter = serviceTB.Text.Trim();
        List<int> matchedServiceIds = new List<int>();
        if (!string.IsNullOrEmpty(serviceFilter))
        {
            matchedServiceIds = _servicesDG.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.DataBoundItem != null)
                .Select(r => (ServiceDto)r.DataBoundItem!)
                .Where(c => c.Name.Contains(serviceFilter, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToList();
        }
        if (matchedServiceIds.Any())
        {
            currentInvoices = currentInvoices
                .Where(inv => matchedServiceIds.Contains(inv.ClientId))
                .ToList();
        }

        string clientFilter = clientTB.Text.Trim();
        List<int> matchedClientIds = new List<int>();
        if (!string.IsNullOrEmpty(clientFilter))
        {
            matchedClientIds = _clientsDG.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.DataBoundItem != null)
                .Select(r => (ClientDto)r.DataBoundItem!)
                .Where(c =>
                c.FullName.Contains(clientFilter, StringComparison.OrdinalIgnoreCase)
                || c.Login.Contains(clientFilter, StringComparison.OrdinalIgnoreCase)
                )
                .Select(c => c.Id)
                .ToList();
        }
        if (matchedClientIds.Any())
        {
            currentInvoices = currentInvoices
                .Where(inv => matchedClientIds.Contains(inv.ClientId))
                .ToList();
        }

        var statusFilter = statusCB.SelectedItem as string;
        if (!string.IsNullOrEmpty(statusFilter))
        {
            currentInvoices = currentInvoices
                .Where(inv => inv.Status == statusFilter)
                .ToList();
        }

        _invoicesDG.DataSource = null;
        _invoicesDG.DataSource = currentInvoices;
        DialogResult = DialogResult.OK;
        Close();
    }
}
