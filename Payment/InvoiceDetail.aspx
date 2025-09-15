<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceDetail.aspx.cs" Inherits="Payment.InvoiceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Table runat="server" Height="100%" Width="60%" BorderStyle="Solid" HorizontalAlign="Center">

                <asp:TableRow runat="server">
                    <asp:TableCell BorderStyle="NotSet" HorizontalAlign="Left" runat="server">
                        <asp:Button runat="server" ID="myInvBtn" Text="Мои счета" OnClick="myInvBtn_Click" />
                        <asp:Button runat="server" ID="logoutBtn" Text="Выйти" OnClick="logoutBtn_Click" />
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell BorderStyle="Solid" HorizontalAlign="Center" runat="server">
                        <h1>Р&К Оплата</h1>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Table runat="server" BorderStyle="Solid" Height="100%" Width="50%" HorizontalAlign="Center">

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Услуга" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="serviceName" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Сумма" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="amount" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Дата выставления" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="issueDate" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Оплатить до" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="dueDate" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Дата оплаты" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="paymentDate" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Статус оплаты" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="status" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label runat="server" Text="Номер квитанции" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:Label ID="receiptNumber" runat="server" />
                                </asp:TableCell>     
                            </asp:TableRow>

                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Center">
                        <asp:Button runat="server" Width="50%" Text="Оплатить счёт" Visible="True" ID="payBtn" OnClick="payBtn_Click"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </form>
</body>
</html>
