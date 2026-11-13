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
      <asp:Button ID="Clear" runat="server" Text="Clear" CssClass="btn-clear" OnClick="Clear_Click" />
      

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
            <asp:Repeater ID="rptDoctors" runat="server" OnItemCommand="rptDoctors_ItemCommand">
            <ItemTemplate>
            <tr>
                <td><%# Server.HtmlEncode(Eval("Name").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("NationalID").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("Mobile").ToString()) %></td>
                <td>
                    <asp:Button runat="server" Text="Delete" CssClass="btn-delete" CommandName="Delete" CommandArgument='<%# Eval("UserID") %>'
                    OnClientClick="return confirm('Delete this doctor?');" />
                    
                </td>
            </tr>
            </ItemTemplate>
            </asp:Repeater>
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
          <label for="patDoctor">Family Doctor</label>
          <asp:DropDownList ID="patDoctor" runat="server"></asp:DropDownList>
        </div>
        <div class="full">
          <label for="patAddress">Address</label>
          <asp:TextBox ID="patAddress" runat="server"></asp:TextBox>
        </div>
      </div>
      <asp:Button ID="btnCreatePatient" runat="server" Text="Create patient" CssClass="btn-create" OnClick="btnCreatePatient_Click" />
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
            <asp:Repeater ID="rptPatients" runat="server" OnItemCommand="rptPatients_ItemCommand">
            <ItemTemplate>
            <tr>
                <td><%# Server.HtmlEncode(Eval("Name").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("NationalID").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("DOB").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("Address").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("Mobile").ToString()) %></td>
                <td><%# Server.HtmlEncode(Eval("DoctorName").ToString()) %></td>
                <td>
                    <asp:Button runat="server" Text="Delete" CssClass="btn-delete" CommandName="Delete" CommandArgument='<%# Eval("UserID") %>'
                    OnClientClick="return confirm('Delete this patient?');" />
                    <asp:Button runat="server" Text="Edit" CssClass="btn-edit" CommandName="Edit" CommandArgument='<%# Eval("UserID") %>' />
                </td>
            </tr>
            </ItemTemplate>
            </asp:Repeater>
        </tbody>
        </table>
      </div>
    </section>

  </main>
      <asp:Panel ID="pnlEdit" runat="server" CssClass="modal">
  <div class="modal-box">
    <h2>Edit patient</h2>
    <asp:HiddenField ID="hfEditUserID" runat="server" />

    <div class="form-grid">
      <div>
        <label for="editName">Name</label>
        <asp:TextBox ID="editName" runat="server"></asp:TextBox>
      </div>
      <div>
        <label for="editId">ID</label>
        <asp:TextBox ID="editId" runat="server"></asp:TextBox>
      </div>
      <div>
        <label for="editDob">Date of birth</label>
        <asp:TextBox ID="editDob" runat="server" TextMode="Date"></asp:TextBox>
      </div>
      <div>
        <label for="editMobile">Mobile</label>
        <asp:TextBox ID="editMobile" runat="server" TextMode="Phone"></asp:TextBox>
      </div>
      <div>
        <label for="editFamilyDoctor">Family Doctor</label>
        <asp:DropDownList ID="editFamilyDoctor" runat="server"></asp:DropDownList>
      </div>
      <div class="full">
        <label for="editAddress">Address</label>
        <asp:TextBox ID="editAddress" runat="server"></asp:TextBox>
      </div>
    </div>

    <h3>Assigned doctors</h3>
    <asp:CheckBoxList ID="cblDoctors" runat="server" CssClass="check-list" RepeatLayout="Flow"></asp:CheckBoxList>

    <div class="modal-actions">
      <asp:Button ID="btnSaveEdit" runat="server" Text="Save" CssClass="btn-create" OnClick="btnSaveEdit_Click" />
      <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-clear" OnClick="btnCancelEdit_Click" CausesValidation="false" />
    </div>
  </div>
</asp:Panel>

  </form>

</body>
</html>
