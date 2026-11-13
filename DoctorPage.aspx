<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoctorPage.aspx.cs" Inherits="ProyectoEnglish.WebForm5" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Doctor | Healthcare Centre</title>
  <link rel="stylesheet" href="css/panel.css">
</head>
<body>

  <form id="form1" runat="server">

  <header class="topbar">
    <h1>Healthcare Centre – Doctor</h1>
    <div>
      <span>Logged in as: <strong id="loggedUser" runat="server"></strong></span>
      <a href="Login.aspx">Log out</a>
    </div>
  </header>

  <main class="container">

    <section class="card">
      <h2>My patients</h2>
      <p>Select one of your patients to manage their medical records.</p>

      <div class="form-grid">
        <div>
          <label for="selPatient">Patient</label>
          <asp:DropDownList ID="selPatient" runat="server"></asp:DropDownList>
        </div>
      </div>
      <asp:Button ID="btnViewRecords" runat="server" Text="View records" OnClick="btnViewRecords_Click" />
    </section>

    <section class="card">
      <h2>Add / edit medical record</h2>
      <p>Patient: <strong id="selectedPatient" runat="server"></strong></p>

      <asp:HiddenField ID="hfRecordID" runat="server" />

      <div class="form-grid">
        <div>
          <label for="recDate">Date of the appointment</label>
          <asp:TextBox ID="recDate" runat="server" TextMode="Date"></asp:TextBox>
        </div>
        <div class="full">
          <label for="recDiagnosis">Diagnosis</label>
          <asp:TextBox ID="recDiagnosis" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>
        <div class="full">
          <label for="recTreatment">Treatment</label>
          <asp:TextBox ID="recTreatment" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnCreateRecord" runat="server" Text="Create record" CssClass="btn-create" OnClick="btnCreateRecord_Click" />
      <asp:Button ID="btnUpdateRecord" runat="server" Text="Update record" CssClass="btn-edit" OnClick="btnUpdateRecord_Click" />
      <asp:Button ID="btnClearRecord" runat="server" Text="Clear" CssClass="btn-clear" OnClick="btnClearRecord_Click" />
    </section>

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
            <asp:Repeater ID="rptRecords" runat="server" OnItemCommand="rptRecords_ItemCommand">
              <ItemTemplate>
                <tr>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentDate"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Diagnosis"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Treatment"))) %></td>
                  <td class="actions">
                    <asp:Button runat="server" Text="Edit" CssClass="btn-edit" CommandName="Edit" CommandArgument='<%# Eval("RecordID") %>' />
                    <asp:Button runat="server" Text="Delete" CssClass="btn-delete" CommandName="Delete" CommandArgument='<%# Eval("RecordID") %>'
                      OnClientClick="return confirm('Delete this record?');" />
                  </td>
                </tr>
              </ItemTemplate>
            </asp:Repeater>
          </tbody>
        </table>
      </div>
    </section>

    <section class="card">
      <h2>Schedule an appointment</h2>
      <p>Book an appointment for one of your patients.</p>

      <div class="form-grid">
        <div>
          <label for="dapPatient">Patient</label>
          <asp:DropDownList ID="dapPatient" runat="server"></asp:DropDownList>
        </div>
        <div>
          <label for="dapDate">Date</label>
          <asp:TextBox ID="dapDate" runat="server" TextMode="Date"></asp:TextBox>
        </div>
        <div>
          <label for="dapTime">Time</label>
          <asp:TextBox ID="dapTime" runat="server" TextMode="Time" step="1800" min="09:00" max="19:30"></asp:TextBox>
        </div>
        <div class="full">
          <label for="dapReason">Reason</label>
          <asp:TextBox ID="dapReason" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnScheduleAppointment" runat="server" Text="Schedule appointment" CssClass="btn-create" OnClick="btnScheduleAppointment_Click" />
    </section>

    <section class="card">
      <h2>My agenda</h2>
      <p>Appointments with your patients.</p>

      <div class="table-wrap">
        <table id="rptAgendaTable">
          <thead>
            <tr>
              <th>Date</th>
              <th>Time</th>
              <th>Patient</th>
              <th>Reason</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <asp:Repeater ID="rptAgenda" runat="server" OnItemCommand="rptAgenda_ItemCommand">
              <ItemTemplate>
                <tr>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentDate"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("AppointmentTime"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("PatientName"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Reason"))) %></td>
                  <td><%# Server.HtmlEncode(Convert.ToString(Eval("Status"))) %></td>
                  <td class="actions">
                    <asp:Button runat="server" Text="Done" CssClass="btn-create" CommandName="CompleteAppt"
                      CommandArgument='<%# Eval("AppointmentID") %>'
                      Visible='<%# Convert.ToString(Eval("Status")) == "Scheduled" %>' />
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

    <section class="card">
      <h2>Change password</h2>
      <p>Use at least 6 characters, with letters and numbers.</p>

      <div class="form-grid">
        <div>
          <label for="curPass">Current password</label>
          <asp:TextBox ID="curPass" runat="server" TextMode="Password"></asp:TextBox>
        </div>
        <div>
          <label for="newPass">New password</label>
          <asp:TextBox ID="newPass" runat="server" TextMode="Password"></asp:TextBox>
        </div>
        <div>
          <label for="confirmPass">Confirm new password</label>
          <asp:TextBox ID="confirmPass" runat="server" TextMode="Password"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnChangePassword" runat="server" Text="Change password" CssClass="btn-edit" OnClick="btnChangePassword_Click" />
    </section>

  </main>

  </form>

</body>
</html>
