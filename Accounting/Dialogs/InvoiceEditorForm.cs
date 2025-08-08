using System.Net.Http.Json;
using Accounting.Dtos;

namespace Accounting.Dialogs;

class InvoiceEditorForm : Form
{
    private readonly HttpClient httpClient = new();
    private readonly string apiUrl = "https://63b1473a2aa1.ngrok-free.app";
    private Size windowSize = new(683, 384);
    private readonly InvoiceDto _invoice = null!;
    // TextBox invoiceLoginTB = null!;
    // TextBox invoiceFullNameTB = null!;
    // TextBox invoiceEmailTB = null!;
    // TextBox invoicePhoneNumberTB = null!;
    public InvoiceEditorForm(InvoiceDto? invoice = null)
    {
        Text = "Счёт";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
        _invoice = invoice ?? new InvoiceDto();
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
            // deleteButton.Click += SendRequest;
            buttonsPanel.Controls.Add(deleteButton);

            saveButton.Text = "Сохранить";
            // saveButton.Click += SendRequest;
        }
        else
        {
            saveButton.Text = "Добавить";
            // saveButton.Click += SendRequest;
        }

        TableLayoutPanel invoiceTable = new();
        invoiceTable.Dock = DockStyle.Fill;
        invoiceTable.ColumnCount = 2;
        invoiceTable.RowCount = 6;
        invoiceTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        // invoiceLoginTB = CreateTextBox(_invoice.Login);
        // invoiceFullNameTB = CreateTextBox(_invoice.FullName);
        // invoiceEmailTB = CreateTextBox(_invoice.Email);
        // invoicePhoneNumberTB = CreateTextBox(_invoice.PhoneNumber);

        invoiceTable.Controls.Add(CreateLabel("ID Услуги:"), 0, 0);
        invoiceTable.Controls.Add(CreateLabel("ID Клиента:"), 0, 1);
        invoiceTable.Controls.Add(CreateLabel("Сумма:"), 0, 2);
        invoiceTable.Controls.Add(CreateLabel("Дата выставления:"), 0, 3);
        invoiceTable.Controls.Add(CreateLabel("Оплатить до:"), 0, 4);

        // invoiceTable.Controls.Add(invoiceLoginTB, 1, 0);
        // invoiceTable.Controls.Add(invoiceFullNameTB, 1, 1);
        // invoiceTable.Controls.Add(invoiceEmailTB, 1, 2);
        // invoiceTable.Controls.Add(invoicePhoneNumberTB, 1, 3);

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

    // private async void SendRequest(object? sender, EventArgs e)
    // {
    //     if (sender is not Button clickedButton) return;
    //     if (!CheckFields() && clickedButton.Text != "Удалить") return;

    //     string messageBoxText = "";
    //     HttpResponseMessage? response = null;

    //     if (clickedButton.Text == "Добавить")
    //     {
    //         messageBoxText = "Счёт добавлен!";
    //         response = await httpClient.PostAsJsonAsync($"{apiUrl}/api/invoices", _invoice);
    //     }
    //     else if (clickedButton.Text == "Сохранить")
    //     {
    //         messageBoxText = "Счёт изменён!";
    //         response = await httpClient.PutAsJsonAsync($"{apiUrl}/api/invoices/{_invoice.Id}", _invoice);
    //     }
    //     else if (clickedButton.Text == "Удалить")
    //     {
    //         messageBoxText = "Счёт удалён!";
    //         response = await httpClient.DeleteAsync($"{apiUrl}/api/invoices/{_invoice.Id}");
    //     }

    //     if (response != null && response.IsSuccessStatusCode)
    //     {
    //         MessageBox.Show(messageBoxText, "Успех✅");
    //         DialogResult = DialogResult.OK;
    //         Close();
    //     }
    //     else if (response is null) MessageBox.Show("Неизвестная кнопка", "Ошибка⚠️");
    //     else if (!response.IsSuccessStatusCode) MessageBox.Show("Не удалось отправить запрос🔌", "Ошибка⚠️");
    // }

    // private bool CheckFields()
    // {
    //     if (
    //         invoiceLoginTB.Text == ""
    //         || invoiceFullNameTB.Text == ""
    //         || invoiceEmailTB.Text == ""
    //         || invoicePhoneNumberTB.Text == ""
    //     )
    //     {
    //         MessageBox.Show("Заполните все поля!", "Ошибка⚠️");
    //         return false;
    //     }
    //     else
    //     {
    //         _invoice.Login = invoiceLoginTB.Text;
    //         _invoice.FullName = invoiceFullNameTB.Text;
    //         _invoice.Email = invoiceEmailTB.Text;
    //         _invoice.PhoneNumber = invoicePhoneNumberTB.Text;
    //         return true;
    //     }
    // }
}
