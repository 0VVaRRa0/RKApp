using System.Net.Http.Json;
using Accounting.Dtos;

namespace Accounting;

public class MainForm : Form
{
    private HttpClient httpClient = new();
    private string apiUrl = "https://4dbc9d24d6f6.ngrok-free.app";
    private Size windowSize = new(1366, 768);
    TabControl tabControl = null!;
    private string servicesStr = "Услуги";
    private string clientsStr = "Клиенты";
    private string invoicesStr = "Счета";
    private DataGridView servicesDataGrid = null!;
    private DataGridView clientsDataGrid = null!;
    private DataGridView invoicesDataGrid = null!;

    public MainForm()
    {
        Text = "Р&К Бухгалтерия";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        InitializeTabs();
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
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
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
        dataGrid.Dock = DockStyle.Fill;
        dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        if (title == servicesStr) servicesDataGrid = dataGrid;
        else if (title == clientsStr) clientsDataGrid = dataGrid;
        else if (title == invoicesStr) invoicesDataGrid = dataGrid;
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
        if (tabControl.SelectedTab!.Text == servicesStr) MessageBox.Show("Добавить услугу");
        else if (tabControl.SelectedTab!.Text == clientsStr) MessageBox.Show("Добавить клиента");
        else if (tabControl.SelectedTab!.Text == invoicesStr) MessageBox.Show("Добавить счёт");
    }

    private async void GetData()
    {
        if (tabControl.SelectedTab!.Text == servicesStr)
        {
            var response = await httpClient.GetAsync($"{apiUrl}/api/services");
            var data = await response.Content.ReadFromJsonAsync<List<ServiceDto>>();
            servicesDataGrid.DataSource = data;
            servicesDataGrid.Columns["Id"]!.Width = 50;
        }
        else if (tabControl.SelectedTab!.Text == clientsStr)
        {
            var response = await httpClient.GetAsync($"{apiUrl}/api/clients");
            var data = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
            clientsDataGrid.DataSource = data;
            clientsDataGrid.Columns["Id"]!.Width = 50;
        }
        else if (tabControl.SelectedTab!.Text == invoicesStr)
        {
            var response = await httpClient.GetAsync($"{apiUrl}/api/invoices");
            var data = await response.Content.ReadFromJsonAsync<List<InvoiceDto>>();
            invoicesDataGrid.DataSource = data;
            invoicesDataGrid.Columns["Id"]!.Width = 50;
        }
    }
}
