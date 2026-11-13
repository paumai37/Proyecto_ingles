using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoEnglish
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        // Contraseña inicial de las cuentas nuevas (se guarda siempre hasheada)
        private const string PasswordInicial = "123456";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                    loggedUser.InnerText = Session["Username"].ToString();
                ListDoctors();
                ListPatients();
                LoadDoctorsDropdown();
            }
        }








        // ===================== DOCTORES =====================







        protected void btnCreateDoctor_Click(object sender, EventArgs e)
        {
            string nombre = docName.Text.Trim();
            if (nombre == "") { Alerta("El nombre es obligatorio."); return; }

            // Regex: DNI/NIE y teléfono
            if (!Validador.EsDocumento(docId.Text.Trim())) { Alerta("DNI/NIE no válido (ej: 12345678Z)."); return; }
            if (!Validador.EsTelefono(docMobile.Text)) { Alerta("Teléfono no válido (9-15 dígitos, prefijo + opcional)."); return; }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();

                    using (var tx = con.BeginTransaction())
                    {
                        long userId;

                        // 1) ¿Ya existe en login (con cualquier rol)?
                        using (var cmd = new SQLiteCommand(
                            "SELECT UserID FROM login WHERE Username = @nombre;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            object r = cmd.ExecuteScalar();
                            userId = (r == null) ? 0 : Convert.ToInt64(r);
                        }

                        if (userId != 0)
                        {
                            // 2a) Existe: comprobar que no sea ya doctor
                            using (var cmd = new SQLiteCommand(
                                "SELECT 1 FROM doctors WHERE UserID = @id;", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", userId);
                                if (cmd.ExecuteScalar() != null)
                                {
                                    Alerta("Ese usuario ya es doctor.");
                                    return; // la transacción se revierte al salir
                                }
                            }
                            using (var cmd = new SQLiteCommand("UPDATE login SET Profile = 'Doctor' WHERE UserID = @id AND Profile = 'Patient';", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // 2b) No existe: crear el login como Doctor (contraseña hasheada)
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO login (Username, Password, Profile) VALUES (@nombre, @pass, 'Doctor');", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@nombre", nombre);
                                cmd.Parameters.AddWithValue("@pass", PasswordHasher.Hash(PasswordInicial));
                                cmd.ExecuteNonQuery();
                            }
                            userId = con.LastInsertRowId;
                        }

                        // 3) Insertar en doctors con el UserID correcto
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO doctors (UserID, Name, NationalID, Mobile) " +
                            "VALUES (@userId, @nombre, @nationalID, @mobile);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            cmd.Parameters.AddWithValue("@nationalID", docId.Text.Trim().ToUpper());
                            cmd.Parameters.AddWithValue("@mobile", docMobile.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                Alerta("Doctor creado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            docName.Text = docId.Text = docMobile.Text = "";
            ListDoctors();
            LoadDoctorsDropdown();
        }










        private void ListDoctors()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new System.Data.DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter("SELECT UserID, Name, NationalID, Mobile FROM doctors ORDER BY Name", con))
                da.Fill(dt);

            rptDoctors.DataSource = dt;
            rptDoctors.DataBind();
        }










        protected void rptDoctors_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete") return;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            string userId = (string)e.CommandArgument;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var tx = con.BeginTransaction())
                    {
                        using (var cmd = new SQLiteCommand(
                            "DELETE FROM doctors WHERE UserID = @id; " +
                            "DELETE FROM doctor_patient WHERE DoctorID = @id; " +
                            "UPDATE patients SET FamilyDoctorID = NULL WHERE FamilyDoctorID = @id; " +
                            // si también es paciente, conserva el login como Patient
                            "UPDATE login SET Profile = 'Patient' WHERE UserID = @id AND Profile = 'Doctor' " +
                            "AND EXISTS (SELECT 1 FROM patients WHERE UserID = @id); " +
                            // solo borra el login si era únicamente doctor
                            "DELETE FROM login WHERE UserID = @id AND Profile = 'Doctor';", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }
                Alerta("Doctor eliminado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("No se pudo eliminar: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }

            ListDoctors();
            ListPatients();
            LoadDoctorsDropdown();
        }









        protected void Clear_Click(object sender, EventArgs e)
        {
            docName.Text = "";
            docId.Text = "";
            docMobile.Text = "";
        }







        // ===================== PACIENTES =====================







        protected void btnCreatePatient_Click(object sender, EventArgs e)
        {
            string nombre = patName.Text.Trim();

            if (nombre == "" || patDoctor.SelectedValue == "")
            {
                Alerta("Nombre y doctor son obligatorios.");
                return;
            }

            // Regex: DNI/NIE y teléfono
            if (!Validador.EsDocumento(patId.Text.Trim())) { Alerta("DNI/NIE no válido (ej: 12345678Z)."); return; }
            if (!Validador.EsTelefono(patMobile.Text)) { Alerta("Teléfono no válido (9-15 dígitos, prefijo + opcional)."); return; }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();

                    using (var tx = con.BeginTransaction())
                    {
                        long userId;

                        // 1) ¿Ya existe en login (con cualquier rol)?
                        using (var cmd = new SQLiteCommand(
                            "SELECT UserID FROM login WHERE Username = @nombre;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            object r = cmd.ExecuteScalar();
                            userId = (r == null) ? 0 : Convert.ToInt64(r);
                        }

                        if (userId != 0)
                        {
                            // 2a) Existe: comprobar que no sea ya paciente
                            using (var cmd = new SQLiteCommand(
                                "SELECT 1 FROM patients WHERE UserID = @id;", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", userId);
                                if (cmd.ExecuteScalar() != null)
                                {
                                    Alerta("Ese usuario ya es paciente.");
                                    return; // al salir del using, la transacción se revierte
                                }
                            }
                        }
                        else
                        {
                            // 2b) No existe: crear el login como Patient (contraseña hasheada)
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO login (Username, Password, Profile) VALUES (@nombre, @pass, 'Patient');", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@nombre", nombre);
                                cmd.Parameters.AddWithValue("@pass", PasswordHasher.Hash(PasswordInicial));
                                cmd.ExecuteNonQuery();
                            }
                            userId = con.LastInsertRowId;
                        }

                        // 3) Insertar en patients con el UserID correcto
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO patients (UserID, Name, NationalID, Mobile, Address, DOB, FamilyDoctorID) " +
                            "VALUES (@userId, @nombre, @nationalID, @mobile, @address, @dob, @familyDoctorID);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            cmd.Parameters.AddWithValue("@nationalID", patId.Text.Trim().ToUpper());
                            cmd.Parameters.AddWithValue("@mobile", patMobile.Text.Trim());
                            cmd.Parameters.AddWithValue("@address", patAddress.Text.Trim());
                            cmd.Parameters.AddWithValue("@dob", patDob.Text);
                            cmd.Parameters.AddWithValue("@familyDoctorID", patDoctor.SelectedValue);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                Alerta("Paciente creado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            patName.Text = patId.Text = patDob.Text = patAddress.Text = patMobile.Text = "";
            ListPatients();
        }









        private void ListPatients()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new System.Data.DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter(
                @"SELECT p.UserID, p.Name, p.NationalID, p.DOB, p.Address, p.Mobile,
                         IFNULL(d.Name, '') AS DoctorName
                  FROM patients p
                  LEFT JOIN doctors d ON d.UserID = p.FamilyDoctorID
                  ORDER BY p.Name", con))
                da.Fill(dt);

            rptPatients.DataSource = dt;
            rptPatients.DataBind();
        }










        protected void rptPatients_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                OpenEditPatient(long.Parse((string)e.CommandArgument));
                return;
            }

            if (e.CommandName != "Delete") return;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            string userId = (string)e.CommandArgument;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var tx = con.BeginTransaction())
                    {
                        using (var cmd = new SQLiteCommand(
                            "DELETE FROM patients WHERE UserID = @id; " +
                            "DELETE FROM doctor_patient WHERE PatientID = @id; " +
                            // solo borra el login si era únicamente paciente
                            "DELETE FROM login WHERE UserID = @id AND Profile = 'Patient';", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }
                Alerta("Paciente eliminado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("No se pudo eliminar: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }

            ListPatients();
        }








        // ===================== AYUDAS =====================








        private void LoadDoctorsDropdown()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new System.Data.DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter(
                "SELECT UserID, Name || ' (' || IFNULL(NationalID, '') || ')' AS Display FROM doctors ORDER BY Name", con))
                da.Fill(dt);

            patDoctor.DataSource = dt;
            patDoctor.DataTextField = "Display";
            patDoctor.DataValueField = "UserID";
            patDoctor.DataBind();

            patDoctor.Items.Insert(0, new ListItem("-- Select doctor --", ""));
        }





        private void Alerta(string texto)
        {
            ClientScript.RegisterStartupScript(GetType(), "msg",
                "alert('" + texto.Replace("'", "\\'") + "');", true);
        }









        void OpenEditPatient(long userId)
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            using (var con = new SQLiteConnection(cadena))
            {
                con.Open();

                // Doctores para el desplegable y la lista de asignados
                var dt = new DataTable();
                using (var da = new SQLiteDataAdapter("SELECT DoctorID, UserID, Name FROM doctors ORDER BY Name;", con))
                    da.Fill(dt);

                editFamilyDoctor.DataSource = dt;
                editFamilyDoctor.DataValueField = "UserID";   // igual que patDoctor y ListPatients
                editFamilyDoctor.DataTextField = "Name";
                editFamilyDoctor.DataBind();
                editFamilyDoctor.Items.Insert(0, new ListItem("(none)", ""));

                cblDoctors.DataSource = dt;
                cblDoctors.DataValueField = "UserID";
                cblDoctors.DataTextField = "Name";
                cblDoctors.DataBind();

                // Datos del paciente
                using (var cmd = new SQLiteCommand(
                    "SELECT Name, NationalID, DOB, Mobile, Address, FamilyDoctorID FROM patients WHERE UserID = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) { Alerta("Paciente no encontrado."); return; }

                        editName.Text = Convert.ToString(rd["Name"]);
                        editId.Text = Convert.ToString(rd["NationalID"]);
                        editDob.Text = Convert.ToString(rd["DOB"]);
                        editMobile.Text = Convert.ToString(rd["Mobile"]);
                        editAddress.Text = Convert.ToString(rd["Address"]);

                        editFamilyDoctor.ClearSelection();
                        var fd = editFamilyDoctor.Items.FindByValue(Convert.ToString(rd["FamilyDoctorID"]));
                        if (fd != null) fd.Selected = true;
                    }
                }

                // Doctores ya asignados
                using (var cmd = new SQLiteCommand(
                    "SELECT DoctorID FROM doctor_patient WHERE PatientID = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read())
                        {
                            var it = cblDoctors.Items.FindByValue(Convert.ToString(rd[0]));
                            if (it != null) it.Selected = true;
                        }
                }
            }

            hfEditUserID.Value = userId.ToString();
            pnlEdit.CssClass = "modal open";
        }













        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            long userId;
            if (!long.TryParse(hfEditUserID.Value, out userId)) return;

            if (editName.Text.Trim() == "")
            {
                Alerta("El nombre es obligatorio.");   // el modal sigue abierto
                return;
            }

            // Regex: DNI/NIE y teléfono
            if (!Validador.EsDocumento(editId.Text.Trim())) { Alerta("DNI/NIE no válido (ej: 12345678Z)."); return; }
            if (!Validador.EsTelefono(editMobile.Text)) { Alerta("Teléfono no válido (9-15 dígitos, prefijo + opcional)."); return; }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var tx = con.BeginTransaction())
                    {
                        using (var cmd = new SQLiteCommand(
                            "UPDATE patients SET Name = @n, NationalID = @nid, DOB = @dob, Mobile = @m, " +
                            "Address = @a, FamilyDoctorID = @fd WHERE UserID = @id;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", editName.Text.Trim());
                            cmd.Parameters.AddWithValue("@nid", editId.Text.Trim().ToUpper());
                            cmd.Parameters.AddWithValue("@dob", editDob.Text);
                            cmd.Parameters.AddWithValue("@m", editMobile.Text.Trim());
                            cmd.Parameters.AddWithValue("@a", editAddress.Text.Trim());
                            cmd.Parameters.AddWithValue("@fd",
                                editFamilyDoctor.SelectedValue == "" ? (object)DBNull.Value : editFamilyDoctor.SelectedValue);
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        // Reemplazar las asignaciones
                        using (var cmd = new SQLiteCommand(
                            "DELETE FROM doctor_patient WHERE PatientID = @id;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (ListItem it in cblDoctors.Items)
                        {
                            if (!it.Selected) continue;
                            using (var cmd = new SQLiteCommand(
                                "INSERT INTO doctor_patient (DoctorID, PatientID) VALUES (@d, @p);", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@d", it.Value);
                                cmd.Parameters.AddWithValue("@p", userId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            pnlEdit.CssClass = "modal";
            Alerta("Paciente actualizado.");
            ListPatients();
        }









        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEdit.CssClass = "modal";
        }
    }
}