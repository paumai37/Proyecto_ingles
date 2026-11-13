<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientPage.aspx.cs" Inherits="ProyectoEnglish.WebForm3" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Patient | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <header class="topbar">
    <h1>Healthcare Centre – Patient</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser" runat="server"></strong></span>
      <a href="login.aspx">Log out</a>
    </div>
  </header>

  <main class="container">

    <section class="card">
      <h2>Personal information</h2>
      <span class="readonly-note">Read-only</span>

      <dl class="info">
        <div>
          <dt>Name</dt>
          <dd id="patName" runat="server"></dd>
        </div>
        <div>
          <dt>ID</dt>
          <dd id="patId" runat="server"></dd>
        </div>
        <div>
          <dt>Date of birth</dt>
          <dd id="patDob" runat="server"></dd>
        </div>
        <div>
          <dt>Address</dt>
          <dd id="patAddress" runat="server"></dd>
        </div>
        <div>
          <dt>Mobile</dt>
          <dd id="patMobile" runat="server"></dd>
        </div>
        <div>
          <dt>My doctor</dt>
          <dd id="patDoctor" runat="server"></dd>
        </div>
      </dl>
    </section>

    <form id="formCitas" runat="server">

    <section class="card">
      <h2>Book an appointment</h2>
      <p>Choose a doctor, a date and a time.</p>

      <div class="form-grid">
        <div>
          <label for="apDoctor">Doctor</label>
          <asp:DropDownList ID="apDoctor" runat="server"></asp:DropDownList>
        </div>
        <div>
          <label for="apDate">Date</label>
          <asp:TextBox ID="apDate" runat="server" TextMode="Date"></asp:TextBox>
        </div>
        <div>
          <label for="apTime">Time</label>
          <asp:TextBox ID="apTime" runat="server" TextMode="Time" step="1800" min="09:00" max="19:30"></asp:TextBox>
        </div>
        <div class="full">
          <label for="apReason">Reason</label>
          <asp:TextBox ID="apReason" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnBookAppointment" runat="server" Text="Book appointment" CssClass="btn-create" OnClick="btnBookAppointment_Click" />
    </section>

    <section class="card">
      <h2>My appointments</h2>
      <p>Your upcoming and past appointments.</p>

      <div class="table-wrap">
        <table id="rptAppointmentsTable">
          <thead>
            <tr>
              <th>Date</th>
              <th>Time</th>
              <th>Doctor</th>
              <th>Reason</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <asp:Repeater ID="rptAppointments" runat="server" OnItemCommand="rptAppointments_ItemCommand">
              <ItemTemplate>
                <tr>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentDate"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentTime"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("DoctorName"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Reason"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Status"))) %></td>
                  <td class="actions">
                    <asp:Button runat="server" Text="Cancel" CssClass="btn-delete" CommandName="CancelAppt"
                      CommandArgument='<%# Eval("AppointmentID") %>'
                      Visible='<%# Convert.ToString(Eval("Status")) == "Scheduled" %>'
                      OnClientClick="return confirm('Cancel this appointment?');" />
                  </td>
                </tr>
              </ItemTemplate>
            </asp:Repeater>
          </tbody>
        </table>
      </div>
    </section>

  </form>

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
            <asp:Repeater ID="rptRecords" runat="server">
            <ItemTemplate>
            <tr>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentDate"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Diagnosis"))) %></td>
                <td><%# Server.HtmlEncode(Convert.ToString(Eval("Treatment"))) %></td>
            </tr>
            </ItemTemplate>
            </asp:Repeater>
        </tbody>
        </table>
      </div>
    </section>

  </main>

</body>
</html>
