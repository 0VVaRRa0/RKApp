<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceDetail.aspx.cs" Inherits="Payment.InvoiceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p><strong>Счет №:</strong> <%= invoice.Id %></p>
            <p><strong>Услуга:</strong> <%= invoice.ServiceName %></p>
            <p><strong>Сумма:</strong> <%= invoice.Amount.ToString("C") %></p>
            <p><strong>Дата выставления:</strong> <%= invoice.IssueDate.ToString("dd.MM.yyyy") %></p>
            <p><strong>Оплатить до:</strong> <%= invoice.DueDate.ToString("dd.MM.yyyy") %></p>
            <p><strong>Дата оплаты:</strong> <%= invoice.PaymentDate?.ToString("dd.MM.yyyy") ?? "—" %></p>
            <p>
                <strong>Номер квитанции:</strong>
                <asp:TextBox ID="ReceiptNumber" ReadOnly="true" runat="server" />
                <asp:Label ID="EmptyRN" Visible="false" runat="server">Введите номер квитанции!</asp:Label>
            </p>
            <p><strong>Статус:</strong> <%= invoice.Status %></p>
            <p><asp:Button ID="PayBtn" Text="Оплатить" Visible="false" runat="server" OnClick="PayBtn_Click" /></p>
        </div>
    </form>
</body>
</html>
