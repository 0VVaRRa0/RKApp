using System.Net.Http.Json;
using System.Threading.Tasks;
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
        List<string> queryParams = new();

        if (issueDateDTP.Checked)
            queryParams.Add($"issueDate={issueDateDTP.Value.Date:yyyy-MM-dd}");

        if (paymentDateDTP.Checked)
            queryParams.Add($"paymentDate={paymentDateDTP.Value.Date:yyyy-MM-dd}");

        string serviceName = serviceTB.Text.Trim();
        if (!string.IsNullOrEmpty(serviceName))
            queryParams.Add($"serviceName={Uri.EscapeDataString(serviceName)}");

        string clientLogin = clientTB.Text.Trim();
        if (!string.IsNullOrEmpty(clientLogin))
            queryParams.Add($"clientLogin={Uri.EscapeDataString(clientLogin)}");

        var status = statusCB.SelectedItem as string;
        if (!string.IsNullOrEmpty(status))
            queryParams.Add($"status={Uri.EscapeDataString(status)}");

        string urlParams = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

        HttpClient httpClient = new();
        var response = httpClient.GetAsync(
            "http://localhost:8000/api/invoices" + urlParams).GetAwaiter().GetResult();

        var invoices = response.Content.ReadFromJsonAsync<List<InvoiceDto>>().GetAwaiter().GetResult();

        _invoicesDG.DataSource = null;
        _invoicesDG.DataSource = invoices;

        DialogResult = DialogResult.OK;
        Close();
    }

}
