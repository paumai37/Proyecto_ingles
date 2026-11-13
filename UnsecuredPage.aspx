<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnsecuredPage.aspx.cs" Inherits="ProyectoEnglish.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Anyone can access this page"></asp:Label>
        </div>
        <asp:Label ID="Label2" runat="server" Text="Are you authenticated?"></asp:Label>
        <asp:Label ID="Label3" runat="server" Text="Bool"></asp:Label>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Secured User" />
        <br />
        <asp:Button ID="Button2" runat="server" Text="Admin" Width="119px" />
    </form>
</body>
</html>
