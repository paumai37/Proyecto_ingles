<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="ProyectoEnglish.WebForm4" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Administrator | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <form id="form1" runat="server">

  <!-- ========== BARRA SUPERIOR ========== -->
  <header class="topbar">
    <h1>Healthcare Centre – Administrator</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser" runat="server"></strong></span>
      <a href="Login.aspx">Log out</a>
    </div>
  </header>

  <main class="container">

    <!-- ========== DOCTORES ========== -->
    <section class="card" id="doctors">
      <h2>Doctors</h2>
      <p>Create, view, update and delete doctors.</p>

      <div class="form-grid">
        <div>
          <label for="docName">Name</label>
          <asp:TextBox ID="docName" runat="server"></asp:TextBox>
        </div>
        <div>
          <label for="docId">ID</label>
          <asp:TextBox ID="docId" runat="server"></asp:TextBox>
        </div>
        <div>
          <label for="docMobile">Mobile</label>
          <asp:TextBox ID="docMobile" runat="server" TextMode="Phone"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnCreateDoctor" runat="server" Text="Create doctor" CssClass="btn-create" OnClick="btnCreateDoctor_Click" />
      <asp:Button ID="btnUpdateDoctor" runat="server" Text="Update doctor" CssClass="btn-edit" OnClick="btnUpdateDoctor_Click" />
      <input type="reset" class="btn-clear" value="Clear" />

      <h3>Doctor list</h3>
      <div class="table-wrap">
        <table id="doctorsTable">
          <thead>
            <tr>
              <th>Name</th>
              <th>ID</th>
              <th>Mobile</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <!-- Las filas se cargan desde la base de datos.
                 Cada fila: Name | ID | Mobile | botones Edit y Delete -->
          </tbody>
        </table>
      </div>
    </section>

    <!-- ========== PACIENTES ========== -->
    <section class="card" id="patients">
      <h2>Patients</h2>
      <p>Create, view, update and delete patients and assign each one to a doctor.</p>

      <div class="form-grid">
        <div>
          <label for="patName">Name</label>
          <asp:TextBox ID="patName" runat="server"></asp:TextBox>
        </div>
        <div>
          <label for="patId">ID</label>
          <asp:TextBox ID="patId" runat="server"></asp:TextBox>
        </div>
        <div>
          <label for="patDob">Date of birth</label>
          <asp:TextBox ID="patDob" runat="server" TextMode="Date"></asp:TextBox>
        </div>
        <div>
          <label for="patMobile">Mobile</label>
          <asp:TextBox ID="patMobile" runat="server" TextMode="Phone"></asp:TextBox>
        </div>
        <div>
          <label for="patDoctor">Doctor</label>
          <asp:DropDownList ID="patDoctor" runat="server"></asp:DropDownList>
        </div>
        <div class="full">
          <label for="patAddress">Address</label>
          <asp:TextBox ID="patAddress" runat="server"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnCreatePatient" runat="server" Text="Create patient" CssClass="btn-create" OnClick="btnCreatePatient_Click" />
      <asp:Button ID="btnUpdatePatient" runat="server" Text="Update patient" CssClass="btn-edit" OnClick="btnUpdatePatient_Click" />
      <input type="reset" class="btn-clear" value="Clear" />

      <h3>Patient list</h3>
      <div class="table-wrap">
        <table id="patientsTable">
          <thead>
            <tr>
              <th>Name</th>
              <th>ID</th>
              <th>DOB</th>
              <th>Address</th>
              <th>Mobile</th>
              <th>Doctor</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <!-- Las filas se cargan desde la base de datos.
                 Cada fila: Name | ID | DOB | Address | Mobile | Doctor | botones Edit y Delete -->
          </tbody>
        </table>
      </div>
    </section>

  </main>

  </form>

</body>
</html>
