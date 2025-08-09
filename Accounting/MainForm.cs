using System.Net.Http.Json;
using Accounting.Dialogs;
using Accounting.Dtos;

namespace Accounting;

public class MainForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "https://81951d3b8c90.ngrok-free.app";
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
        Shown += RefreshPage;
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

    private TabPage CreateTabPage(string title)
    {
        TabPage page = new(title);
        page.Controls.Add(CreateTable(title));
        return page;
    }

    private TableLayoutPanel CreateTable(string title)
    {
        TableLayoutPanel table = new();
        table.ColumnCount = 1;
        table.RowCount = 2;
        table.Dock = DockStyle.Fill;
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        table.Controls.Add(CreateButtonsPanel(), 0, 0);
        table.Controls.Add(CreateDataGrid(title), 0, 1);
        return table;
    }

    private FlowLayoutPanel CreateButtonsPanel()
    {
        FlowLayoutPanel panel = new();
        panel.Dock = DockStyle.Fill;
        panel.Controls.Add(CreateButton("Обновить", RefreshPage));
        panel.Controls.Add(CreateButton("Добавить", CreateObject));
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

        if (title == servicesStr)
        {
            servicesDataGrid = dataGrid;
            servicesDataGrid.CellDoubleClick += EditService;
        }
        else if (title == clientsStr)
        {
            clientsDataGrid = dataGrid;
            clientsDataGrid.CellDoubleClick += EditClient;
        }
        else if (title == invoicesStr)
        {
            invoicesDataGrid = dataGrid;
            invoicesDataGrid.CellDoubleClick += EditInvoice;
        };
        return dataGrid;
    }

    private void RefreshPage(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab!.Text == servicesStr) GetData();
        else if (tabControl.SelectedTab!.Text == clientsStr) GetData();
        else if (tabControl.SelectedTab!.Text == invoicesStr) GetData();
    }

    private void CreateObject(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab!.Text == servicesStr) CreateService(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == clientsStr) CreateClient(null, EventArgs.Empty);
        else if (tabControl.SelectedTab!.Text == invoicesStr) CreateInvoice(null, EventArgs.Empty);
    }

    private async void GetData()
    {
        string tab = tabControl.SelectedTab!.Text;

        if (tab == servicesStr)
            await SendRequest<ServiceDto>("api/services", servicesDataGrid);
        else if (tab == clientsStr)
            await SendRequest<ClientDto>("api/clients", clientsDataGrid);
        else if (tab == invoicesStr)
            await SendRequest<InvoiceDto>("api/invoices", invoicesDataGrid);
    }

    private async Task SendRequest<T>(string apiEndpoint, DataGridView grid)
    {
        try
        {
            var response = await httpClient.GetAsync($"{apiUrl}/{apiEndpoint}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
            grid.DataSource = data;
            grid.Columns["Id"]!.Width = 50;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
        }
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
        InvoiceEditorForm form = new(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }

    private void EditInvoice(object? sender, EventArgs e)
    {
        var row = invoicesDataGrid.SelectedRows[0];
        var invoice = row.DataBoundItem as InvoiceDto;
        InvoiceEditorForm form = new(invoice);
        if (form.ShowDialog() == DialogResult.OK)
        {
            RefreshPage(null, EventArgs.Empty);
        }
    }
}
