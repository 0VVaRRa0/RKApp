<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayInvoice.aspx.cs" Inherits="Payment.PayInvoice" %>

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
                        <asp:Table runat="server" BorderStyle="Solid" Height="100%" Width="50%" HorizontalAlign="Center" BorderWidth="1px">

                            <asp:TableRow runat="server">
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px" HorizontalAlign="Center">
                                    <asp:Label runat="server" Text="Введите номер квитанции" />
                                </asp:TableCell>
                                <asp:TableCell BorderStyle="Solid" BorderWidth="1px">
                                    <asp:TextBox runat="server" ID="receiptNumberTB" Width="97%" />
                                </asp:TableCell>
                            </asp:TableRow>

                        </asp:Table>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Center">
                        <asp:Button runat="server" Width="50%" Text="Отправить" Visible="True" ID="sendBtn" OnClick="sendBtn_Click" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </form>
</body>
</html>
