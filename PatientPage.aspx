<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SecuredPage.aspx.cs" Inherits="ProyectoEnglish.WebForm3" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Patient | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <!-- ========== BARRA SUPERIOR ========== -->
  <header class="topbar">
    <h1>Healthcare Centre – Patient</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser"></strong></span>
      <a href="login.html">Log out</a>
    </div>
  </header>

  <main class="container">

    <!-- ========== DATOS PERSONALES ========== -->
    <section class="card">
      <h2>Personal information</h2>
      <span class="readonly-note">Read-only</span>

      <!-- Los valores se cargan desde la base de datos -->
      <dl class="info">
        <div>
          <dt>Name</dt>
          <dd id="patName"></dd>
        </div>
        <div>
          <dt>ID</dt>
          <dd id="patId"></dd>
        </div>
        <div>
          <dt>Date of birth</dt>
          <dd id="patDob"></dd>
        </div>
        <div>
          <dt>Address</dt>
          <dd id="patAddress"></dd>
        </div>
        <div>
          <dt>Mobile</dt>
          <dd id="patMobile"></dd>
        </div>
        <div>
          <dt>My doctor</dt>
          <dd id="patDoctor"></dd>
        </div>
      </dl>
    </section>

    <!-- ========== HISTORIAL MÉDICO ========== -->
    <section class="card">
      <h2>My medical records</h2>
      <span class="readonly-note">Read-only</span>

      <div class="table-wrap">
        <table id="recordsTable">
          <thead>
            <tr>
              <th>Date</th>
              <th>Diagnosis</th>
              <th>Treatment</th>
            </tr>
          </thead>
          <tbody>
            <!-- Las filas se cargan desde la base de datos.
                 Cada fila: Date | Diagnosis | Treatment -->
          </tbody>
        </table>
      </div>
    </section>

  </main>

</body>
</html>