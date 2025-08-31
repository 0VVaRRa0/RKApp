using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Accounting.Dialogs;
using Accounting.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace Accounting;

public class MainForm : Form
{
    private const string ApiUrl = "http://localhost:8000/api";
    private const string ServicesStr = "Услуги";
    private const string ClientsStr = "Клиенты";
    private const string InvoicesStr = "Счета";
    private readonly HttpClient httpClient = new();
    private HubConnection _connection = null!;
    private Size windowSize = new(1366, 768);
    TabControl tabControl = null!;
    private DataGridView servicesDataGrid = null!;
    private BindingList<ServiceDto> servicesList = [];
    private DataGridView clientsDataGrid = null!;
    private BindingList<ClientDto> clientsList = [];
    private DataGridView invoicesDataGrid = null!;
    private BindingList<InvoiceDto> invoicesList = [];

    public MainForm()
    {
        Text = "Р&К Бухгалтерия";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        InitializeTabs();
        LoadAllData();
        InitializeSignalR();
    }

    private void InitializeSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{ApiUrl}/hub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On("ServicesUpdated", async () =>
        {
            await LoadData("/services", servicesList);
        });

        _connection.On("ClientsUpdated", async () =>
        {
            await LoadData("/clients", clientsList);
        });

        _connection.On("InvoicesUpdated", async () =>
        {
            await LoadData("/invoices", invoicesList);
        });

        ConnectToHub();
    }

    private async void ConnectToHub()
    {
        try
        {
            await _connection.StartAsync();
            Debug.WriteLine("Connected to SignalR Hub");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error connecting to Hub: " + ex.Message);
        }
    }

    public void InitializeTabs()
    {
        tabControl = new TabControl();
        tabControl.Dock = DockStyle.Fill;

        tabControl.Controls.Add(CreateTabPage(ServicesStr));
        tabControl.Controls.Add(CreateTabPage(ClientsStr));
        tabControl.Controls.Add(CreateTabPage(InvoicesStr));

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
        if (tabTitle == InvoicesStr) panel.Controls.Add(CreateButton("Фильтр", Filter));
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
        dataGrid.AllowUserToAddRows = false;

        if (title == ServicesStr)
        {
            servicesDataGrid = dataGrid;
            servicesDataGrid.CellDoubleClick += EditService;
            servicesDataGrid.Columns.Add(CreateDataGridColumns("Name", "Название услуги"));
            servicesDataGrid.DataSource = servicesList;
        }
        else if (title == ClientsStr)
        {
            clientsDataGrid = dataGrid;
            clientsDataGrid.CellDoubleClick += EditClient;
            clientsDataGrid.Columns.Add(CreateDataGridColumns("Login", "Логин"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("FullName", "Имя"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("Email", "Email"));
            clientsDataGrid.Columns.Add(CreateDataGridColumns("Phone", "Номер телефона"));
            clientsDataGrid.DataSource = clientsList;
        }
        else if (title == InvoicesStr)
        {
            invoicesDataGrid = dataGrid;
            invoicesDataGrid.CellDoubleClick += EditInvoice;
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ServiceName", "Название услуги"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ClientLogin", "Логин клиента"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("Amount", "Сумма"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("IssueDate", "Дата выставления"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("DueDate", "Оплатить до"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("Status", "Статус оплаты"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("PaymentDate", "Дата оплаты"));
            invoicesDataGrid.Columns.Add(CreateDataGridColumns("ReceiptNumber", "Номер квитанции"));
            invoicesDataGrid.DataSource = invoicesList;
            invoicesDataGrid.CellFormatting += invoicesDataGrid_CellFormatting!;

        }
        ;
        return dataGrid;
    }

    private DataGridViewColumn CreateDataGridColumns(string name, string text)
    {
        DataGridViewTextBoxColumn Column = new();
        Column.Name = name;
        Column.DataPropertyName = name;
        Column.HeaderText = text;
        return Column;
    }

    private async void RefreshPage(object? sender, EventArgs e)
    {
        string tabName = tabControl.SelectedTab!.Text;
        switch (tabName)
        {
            case ServicesStr: await LoadData("/services", servicesList); break;
            case ClientsStr: await LoadData("/clients", clientsList); break;
            case InvoicesStr: await LoadData("/invoices", invoicesList); break;
        }
    }

    private async Task LoadData<T>(string endpoint, BindingList<T> list)
    {
        var response = await httpClient.GetAsync(ApiUrl + endpoint);
        if (!response.IsSuccessStatusCode)
        {
            var e = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Ошибка при получении данных: {response.StatusCode}\n{e}", "Ошибка");
            return;
        }
        var objects = await response.Content.ReadFromJsonAsync<List<T>>();
        if (objects == null) { MessageBox.Show("Не удалось преобразовать данные", "Ошибка"); return; }
        if (InvokeRequired)
        {
            Invoke(new Action(() =>
            {
                list.Clear();
                foreach (var obj in objects)
                    list.Add(obj);
            }));
        }
        else
        {
            list.Clear();
            foreach (var obj in objects)
                list.Add(obj);
        }
    }

    private async void LoadAllData()
    {
        await LoadData("/services", servicesList);
        await LoadData("/clients", clientsList);
        await LoadData("/invoices", invoicesList);
    }

    private void CreateObject(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab!.Text == ServicesStr) CreateService(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == ClientsStr) CreateClient(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == InvoicesStr) CreateInvoice(null, EventArgs.Empty);
    }

    private async void CreateService(object? sender, EventArgs e)
    {
        ServiceEditorForm form = new(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadData("/services", servicesList);
        }
    }

    private async void EditService(object? sender, EventArgs e)
    {
        var row = servicesDataGrid.SelectedRows[0];
        var service = row.DataBoundItem as ServiceDto;
        ServiceEditorForm form = new(service);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadData("/services", servicesList);
        }
    }

    private async void CreateClient(object? sender, EventArgs e)
    {
        ClientEditorForm form = new(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadData("/clients", clientsList);
        }
    }

    private async void EditClient(object? sender, EventArgs e)
    {
        var row = clientsDataGrid.SelectedRows[0];
        var client = row.DataBoundItem as ClientDto;
        ClientEditorForm form = new(client);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadData("/clients", clientsList);
        }
    }

    private void CreateInvoice(object? sender, EventArgs e)
    {
        InvoiceEditorForm form = new(servicesList, clientsList, null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void EditInvoice(object? sender, EventArgs e)
    {
        var row = invoicesDataGrid.SelectedRows[0];
        var invoice = row.DataBoundItem as InvoiceDto;
        InvoiceEditorForm form = new(servicesList, clientsList, invoice);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void Filter(object? sender, EventArgs e)
    {
        var form = new FilterForm(invoicesList);
        if (form.ShowDialog() == DialogResult.OK) return;
    }

    private void invoicesDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        var grid = (DataGridView)sender;
        var row = grid.Rows[e.RowIndex];
        var dueDate = row.Cells["DueDate"].Value as DateTime?;
        var paymentDate = row.Cells["PaymentDate"].Value as DateTime?;
        var status = row.Cells["Status"].Value as bool?;

        if (dueDate < DateTime.Today && status != true)
            row.DefaultCellStyle.BackColor = Color.IndianRed;
        else if (paymentDate.HasValue && paymentDate > dueDate && status == true)
            row.DefaultCellStyle.BackColor = Color.Yellow;
        else if (paymentDate.HasValue && paymentDate < dueDate && status == true)
            row.DefaultCellStyle.BackColor = Color.LightGreen;

        if (grid.Columns[e.ColumnIndex].Name == "Status" && e.Value is bool isPaid)
        {
            e.Value = isPaid ? "Оплачено" : "Не оплачено";
            e.FormattingApplied = true;
        }
    }
}

using System.Net.Http.Json;
using Accounting.Dialogs;
using Accounting.Dtos;

namespace Accounting;

public class MainForm : Form
{
    private SignalRService _signalRService= null!;
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
        InitializeSignalR();
        InitializeTabs();
        Shown += GetAllData;
    }

    private void InitializeSignalR()
    {
        _signalRService = new SignalRService(
            $"{apiUrl}/notificationsHub",
            onRefreshServices : () => Invoke(async () => await GetData(servicesStr)),
            onRefreshClients : () => Invoke(async () => await GetData(clientsStr)),
            onRefreshInvoices : () => Invoke(async () => await GetData(invoicesStr))
        );
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await _signalRService.StartAsync();
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

    private async void RefreshPage(object? sender, EventArgs e)
    {
        string tabName = tabControl.SelectedTab!.Text;
        if (tabName == servicesStr || tabName == clientsStr || tabName == invoicesStr)
            await GetData(tabName);
    }

    private async Task GetData(string tabName)
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
