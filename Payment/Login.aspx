<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Payment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Р&К Оплата</h1>
            <asp:TextBox ID="LoginTB"  Text="VVaRRa" runat="server" />
            <asp:Button ID="LoginBtn" Text="Войти" runat="server" OnClick="LoginBtn_Click" />
            <asp:Label ID="LoginWarn" Visible="false" runat="server" ForeColor="Red">Неправильный логин!</asp:Label>
        </div>
    </form>
</body>
</html>
