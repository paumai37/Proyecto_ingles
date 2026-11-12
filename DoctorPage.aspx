<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Doctor | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <!-- ========== BARRA SUPERIOR ========== -->
  <header class="topbar">
    <h1>Healthcare Centre – Doctor</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser"></strong></span>
      <a href="login.html">Log out</a>
    </div>
  </header>

  <main class="container">

    <!-- ========== SELECCIÓN DE PACIENTE ========== -->
    <section class="card">
      <h2>My patients</h2>
      <p>Select one of your patients to manage their medical records.</p>

      <form action="#" method="get">
        <div class="form-grid">
          <div>
            <label for="patient">Patient</label>
            <select id="patient" name="patient">
              <option value="">-- Select a patient --</option>
              <!-- Solo los pacientes asignados a este doctor, desde la base de datos -->
            </select>
          </div>
        </div>
        <button type="submit">View records</button>
      </form>
    </section>

    <!-- ========== NUEVO / EDITAR HISTORIAL ========== -->
    <section class="card">
      <h2>Add / edit medical record</h2>
      <p>Patient: <strong id="selectedPatient"></strong></p>

      <form action="#" method="post">
        <div class="form-grid">
          <div>
            <label for="date">Date of the appointment</label>
            <input type="date" id="date" name="date" required>
          </div>
          <div class="full">
            <label for="diagnosis">Diagnosis</label>
            <textarea id="diagnosis" name="diagnosis" required></textarea>
          </div>
          <div class="full">
            <label for="treatment">Treatment</label>
            <textarea id="treatment" name="treatment" required></textarea>
          </div>
        </div>
        <button type="submit" class="btn-create">Create record</button>
        <button type="submit" class="btn-edit">Update record</button>
        <button type="reset" class="btn-clear">Clear</button>
      </form>
    </section>

    <!-- ========== LISTA DE HISTORIALES ========== -->
    <section class="card">
      <h2>Medical records</h2>
      <p>Records of the selected patient.</p>

      <div class="table-wrap">
        <table id="recordsTable">
          <thead>
            <tr>
              <th>Date</th>
              <th>Diagnosis</th>
              <th>Treatment</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <!-- Las filas se cargan desde la base de datos.
                 Cada fila: Date | Diagnosis | Treatment | botones Edit y Delete -->
          </tbody>
        </table>
      </div>
    </section>

  </main>

</body>
</html>