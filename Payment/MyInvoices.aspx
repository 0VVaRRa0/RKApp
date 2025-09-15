<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyInvoices.aspx.cs" Inherits="Payment.MyInvoices" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Р&К Оплата</title>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@7.0.0/dist/browser/signalr.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Table runat="server" BorderStyle="Solid" Height="100%" Width="60%" HorizontalAlign="Center">

                <asp:TableRow runat="server">
                    <asp:TableCell HorizontalAlign="Left" runat="server">

                        <asp:Button runat="server" ID="logoutBtn" Text="Выйти" OnClick="logoutBtn_Click"/>
                        <asp:Button runat="server" ID="filterBtn" Text="Фильтр" OnClick="filterBtn_Click" />

                        <asp:Label runat="server" Text="Услуга" />
                        <asp:TextBox runat="server" ID="serviceTB" />

                        <asp:Label runat="server" Text="Дата выставления" />
                        <asp:TextBox runat="server" ID="issueDate" TextMode="Date" />

                        <asp:Label runat="server" Text="Дата оплаты" />
                        <asp:TextBox runat="server" ID="paymentDate" TextMode="Date" />

                        <asp:Label runat="server" Text="Статус" />
                        <asp:DropDownList ID="status" runat="server">
                            <asp:ListItem Selected="True" Value="0" Text=" " />
                            <asp:ListItem Text="Оплачено" Value="1" />
                            <asp:ListItem Text="Не оплачено" Value="2" />
                        </asp:DropDownList>

                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Center" BorderStyle="Solid">
                        <h1>Р&К Оплата</h1>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Center" BorderStyle="Solid">
                        <asp:GridView
                            runat="server"
                            ID="invoicesGV"
                            AutoGenerateColumns="False"
                            Width="100%"
                            AutoGenerateSelectButton="True"
                            OnSelectedIndexChanged="invoicesGV_SelectedIndexChanged" DataKeyNames="Id">
                            <Columns>
                                <asp:BoundField DataField="ServiceName" HeaderText="Услуга" />
                                <asp:BoundField DataField="Amount" HeaderText="Сумма" />
                                <asp:TemplateField HeaderText="Статус" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%# (bool)Eval("Status") ? "Оплачено" : "Не оплачено" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:TableCell>
                </asp:TableRow>

            </asp:Table>
        </div>

        <script>
            document.addEventListener("DOMContentLoaded", function () {
                const connection = new signalR.HubConnectionBuilder()
                    .withUrl("http://localhost:8000/api/hub")
                    .withAutomaticReconnect()
                    .build();

                connection.on("InvoicesUpdated", function () {
                    console.log("Invoices was updated");
                    __doPostBack('refreshInvoices', '');
                });

                connection.start()
                    .then(function () {
                        console.log("Подключение к SignalR Hub установлено");
                    })
                    .catch(function (err) {
                        console.error(err.toString());
                    });
            });
        </script>

    </form>
</body>
</html>
