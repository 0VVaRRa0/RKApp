<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Payment.Login" %>

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
                    <asp:TableCell runat="server" BorderStyle="Solid" HorizontalAlign="Center">
                        <h1>Р&К Оплата</h1>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Center">
                        <p>Введите логин</p>
                        <asp:TextBox ID="loginTB" runat="server"></asp:TextBox>
                        <asp:Button ID="loginBtn" runat="server" Text="Войти" OnClick="loginBtn_Click" />
                        <asp:Label ID="wrongLogin" runat="server" ForeColor="Red" Text="Неверный логин!" Visible="False" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </form>
</body>
</html>
