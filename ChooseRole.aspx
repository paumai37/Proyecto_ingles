<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChooseRole.aspx.cs" Inherits="ProyectoEnglish.ChooseRole" %>


<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Choose role | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <form id="form1" runat="server">

  <!-- ========== BARRA SUPERIOR ========== -->
  <header class="topbar">
    <h1>Healthcare Centre</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser" runat="server"></strong></span>
      <a href="Login.aspx">Log out</a>
    </div>
  </header>

  <main class="container">

    <section class="card" id="chooseRole">
      <h2>Choose how to continue</h2>
      <p>Your account has more than one profile. Select the one you want to use for this session.</p>

      <asp:Repeater ID="rptRoles" runat="server" OnItemCommand="rptRoles_ItemCommand">
        <ItemTemplate>
          <asp:Button runat="server"
                      Text='<%# "Continue as " + Container.DataItem %>'
                      CommandName="Select"
                      CommandArgument='<%# Container.DataItem %>' />
        </ItemTemplate>
      </asp:Repeater>
    </section>

  </main>

  </form>

</body>
</html>