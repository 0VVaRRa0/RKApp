using System.Net.Http.Json;
using Accounting.Dialogs;
using Accounting.Dtos;

namespace Accounting;

public class MainForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "http://localhost:8000";
    private Size windowSize = new(1366, 768);
    TabControl tabControl = null!;
    private readonly string servicesStr = "Услуги";
    private readonly string clientsStr = "Клиенты";
    private readonly string invoicesStr = "Счета";
    private DataGridView servicesDataGrid = null!;
    private DataGridView clientsDataGrid = null!;
    private DataGridView invoicesDataGrid = null!;
    public MainForm()
    {
        Text = "Р&К Бухгалтерия";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        InitializeTabs();
        Shown += GetAllData;
    }

    public void InitializeTabs()
    {
        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;

        tabControl.Controls.Add(CreateTabPage(servicesStr));
        tabControl.Controls.Add(CreateTabPage(clientsStr));
        tabControl.Controls.Add(CreateTabPage(invoicesStr));

        Controls.Add(tabControl);
    }

    private TabPage CreateTabPage(string tabTitle)
    {
        TabPage page = new(tabTitle);
        page.Controls.Add(CreateTable(tabTitle));
        return page;
    }

    private TableLayoutPanel CreateTable(string tabTitle)
    {
        TableLayoutPanel table = new();
        table.ColumnCount = 1;
        table.RowCount = 2;
        table.Dock = DockStyle.Fill;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        table.Controls.Add(CreateButtonsPanel(tabTitle), 0, 0);
        table.Controls.Add(CreateDataGrid(tabTitle), 0, 1);
        return table;
    }

    private FlowLayoutPanel CreateButtonsPanel(string tabTitle)
    {
        FlowLayoutPanel panel = new();
        panel.Dock = DockStyle.Fill;
        panel.Controls.Add(CreateButton("Обновить", RefreshPage));
        panel.Controls.Add(CreateButton("Добавить", CreateObject));
        if (tabTitle == invoicesStr) panel.Controls.Add(CreateButton("Фильтр", Filter));
        return panel;
    }

    private Button CreateButton(string buttonText, EventHandler handler)
    {
        Button button = new();
        button.Text = buttonText;
        button.Click += handler;
        return button;
    }

    private DataGridView CreateDataGrid(string title)
    {
        DataGridView dataGrid = new();
        dataGrid.ReadOnly = true;
        dataGrid.Dock = DockStyle.Fill;
        dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGrid.AutoGenerateColumns = false;

        if (title == servicesStr)
        {
            servicesDataGrid = dataGrid;
            servicesDataGrid.CellDoubleClick += EditService;
            servicesDataGrid.Columns.Add(CreateDataGridColumns("Name", "Название услуги"));
        }
        else if (title == clientsStr)
        {
            clientsDataGrid = dataGrid;
            clientsDataGrid.CellDoubleClick += EditClient;
            clientsDataGrid.Columns.Add(CreateDataGridColumns("Login", "Логин"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("FullName", "Имя"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("Email", "Email"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("PhoneNumber", "Номер телефона"));
        }
        else if (title == invoicesStr)
        {
            invoicesDataGrid = dataGrid;
            invoicesDataGrid.CellDoubleClick += EditInvoice;
            invoicesDataGrid.CellFormatting += CheckInvoicesDates;
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ServiceName", "Название услуги"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ClientLogin", "Логин клиента"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("Amount", "Сумма"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("IssueDate", "Дата выставления"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("DueDate", "Оплатить до"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("PaymentDate", "Дата оплаты"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ReceiptNumber", "Номер квитанции"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("Status", "Статус оплаты"));
        }
        ;
        return dataGrid;
    }

    private DataGridViewColumn CreateDataGridColumns(string name, string text)
    {
        DataGridViewTextBoxColumn Column = new();
        Column.DataPropertyName = name;
        Column.HeaderText = text;
        return Column;
    }

    private void RefreshPage(object? sender, EventArgs e)
    {
        string tabName = tabControl.SelectedTab!.Text;
        if (tabName == servicesStr || tabName == clientsStr || tabName == invoicesStr)
            GetData(tabName);
    }

    private async void GetData(string tabName)
    {
        if (tabName == servicesStr)
            await SendRequest<ServiceDto>("api/services", servicesDataGrid);
        else if (tabName == clientsStr)
            await SendRequest<ClientDto>("api/clients", clientsDataGrid);
        else if (tabName == invoicesStr)
            await SendRequest<InvoiceDto>("api/invoices", invoicesDataGrid);
    }

    private async void GetAllData(object? sender, EventArgs e)
    {
        await SendRequest<ServiceDto>("api/services", servicesDataGrid);
        await SendRequest<ClientDto>("api/clients", clientsDataGrid);
        await SendRequest<InvoiceDto>("api/invoices", invoicesDataGrid);
    }

    private async Task SendRequest<T>(string apiEndpoint, DataGridView grid)
    {
        try
        {
            var response = await httpClient.GetAsync($"{apiUrl}/{apiEndpoint}");
            response.EnsureSuccessStatusCode();
            grid.DataSource = await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
        }
    }

    private void CreateObject(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab!.Text == servicesStr) CreateService(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == clientsStr) CreateClient(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == invoicesStr) CreateInvoice(null, EventArgs.Empty);
    }

    private void CreateService(object? sender, EventArgs e)
    {
        ServiceEditorForm form = new(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void EditService(object? sender, EventArgs e)
    {
        var row = servicesDataGrid.SelectedRows[0];
        var service = row.DataBoundItem as ServiceDto;
        ServiceEditorForm form = new(service);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void CreateClient(object? sender, EventArgs e)
    {
        ClientEditorForm form = new(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void EditClient(object? sender, EventArgs e)
    {
        var row = clientsDataGrid.SelectedRows[0];
        var client = row.DataBoundItem as ClientDto;
        ClientEditorForm form = new(client);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void CreateInvoice(object? sender, EventArgs e)
    {
        InvoiceEditorForm form = new(servicesDataGrid, clientsDataGrid, null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void EditInvoice(object? sender, EventArgs e)
    {
        var row = invoicesDataGrid.SelectedRows[0];
        var invoice = row.DataBoundItem as InvoiceDto;
        InvoiceEditorForm form = new(servicesDataGrid, clientsDataGrid, invoice);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void Filter(object? sender, EventArgs e)
    {
        var form = new FilterForm(invoicesDataGrid);
        if (form.ShowDialog() == DialogResult.OK) return;
    }

    private void CheckInvoicesDates(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (invoicesDataGrid.Rows[e.RowIndex].DataBoundItem is InvoiceDto invoice)
        {
            bool isOverdue = false;

            if (invoice.PaymentDate.HasValue)
            {
                isOverdue = invoice.PaymentDate.Value > invoice.DueDate;
            }
            else
            {
                isOverdue = DateTime.Today > invoice.DueDate;
            }

            if (isOverdue)
            {
                invoicesDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
            }
            else
            {
                invoicesDataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }
    }
}
