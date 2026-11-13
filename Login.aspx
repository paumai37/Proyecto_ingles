<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ProyectoEnglish.WebForm1" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Log in | Healthcare Centre</title>
  <link rel="stylesheet" href="css/login.css">
</head>
<body>

  <main class="login-box">
    <img src="images/logo.jpg" alt="Healthcare Centre logo" width="70">
    <h1>Healthcare Centre</h1>
    <h2>Log in</h2>

    <form id="form1" runat="server">
      <label >Username<br />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        </label>
      

      <label >Password<br />
        <asp:TextBox ID="TextBox2" runat="server" TextMode="Password"></asp:TextBox>
        </label>
      

      <asp:Button ID="Button1" runat="server" Text="Log in" OnClick="Button1_Click" />
      <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>
    </form>

    <a href="index.aspx" class="back">&larr; Back to home</a>
  </main>

</body>
</html>