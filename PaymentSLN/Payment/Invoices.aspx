<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Invoices.aspx.cs" Inherits="Payment.Invoices" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Р&К Оплата</h1>
            <p><strong>Оплатить до: <input type="date" id="DueDateFilter" runat="server" /></strong></p>
            <p><strong>Дата оплаты: <input type="date" id="PaymentDateFilter" runat="server" /></strong></p>
            <p><strong>Услуга: <asp:TextBox ID="ServiceNameFilter" runat="server" /></strong></p>
            <p>
                <strong>Статус: </strong>
                <asp:DropDownList ID="StatusFilter" runat="server">
                    <asp:ListItem Text="Выберите статус" Value="" />
                    <asp:ListItem Text="Оплачено" Value="PAID" />
                    <asp:ListItem Text="Не оплачено" Value="NOT PAID" />
                </asp:DropDownList>
            </p>
            <p>
                <asp:Button ID="FilterBtn" Text="Фильтровать" runat="server" OnClick="FilterBtn_Click" />
                <asp:Button ID="ClrFilterBtn" Text="Сбросить фильтр" runat="server" OnClick="ClrFilterBtn_Click" />
            </p>
            <asp:Label ID="EmptyInvoices" runat="server">В данный момент у вас нет счетов</asp:Label>
            <asp:GridView ID="InvoicesGV" runat="server" DataKeyNames="Id"
                    AutoGenerateColumns="False" AutoGenerateSelectButton="True"
                    OnSelectedIndexChanged="InvoicesGV_SelectedIndexChanged"
                    OnRowDataBound="InvoicesGV_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ServiceName" HeaderText="Услуга" />
                    <asp:BoundField DataField="Amount" HeaderText="Сумма" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="DueDate" HeaderText="Оплатить до" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="PaymentDate" HeaderText="Дата оплаты" DataFormatString="{0:dd.MM.yyyy}" />
                    <asp:BoundField DataField="Status" HeaderText="Статус" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
