using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoEnglish
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        // Pacientes de este doctor: los que lo tienen como doctor de cabecera
        // o los que tiene asignados en doctor_patient (parámetro @doc)
        const string MisPacientes =
            "SELECT p.UserID FROM patients p WHERE p.FamilyDoctorID = @doc " +
            "OR p.UserID IN (SELECT PatientID FROM doctor_patient WHERE DoctorID = @doc)";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Doctor")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                    loggedUser.InnerText = Session["Username"].ToString();
                LoadPatientsDropdowns();
                ListAgenda();
            }
        }



        // ===================== HISTORIALES =====================



        protected void btnViewRecords_Click(object sender, EventArgs e)
        {
            if (selPatient.SelectedValue == "")
            {
                Alerta("Selecciona un paciente.");
                return;
            }

            selectedPatient.InnerText = selPatient.SelectedItem.Text;
            btnClearRecord_Click(sender, e);
            ListRecords();
        }



        protected void btnCreateRecord_Click(object sender, EventArgs e)
        {
            if (!EsMiPaciente(selPatient.SelectedValue))
            {
                Alerta("Selecciona un paciente.");
                return;
            }

            if (!DatosHistorialValidos()) return;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO records (PatientID, AppointmentDate, Diagnosis, Treatment) " +
                        "VALUES (@p, @date, @diag, @treat);", con))
                    {
                        cmd.Parameters.AddWithValue("@p", selPatient.SelectedValue);
                        cmd.Parameters.AddWithValue("@date", recDate.Text);
                        cmd.Parameters.AddWithValue("@diag", recDiagnosis.Text.Trim());
                        cmd.Parameters.AddWithValue("@treat", recTreatment.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }
                Alerta("Historial creado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            btnClearRecord_Click(sender, e);
            ListRecords();
        }



        protected void btnUpdateRecord_Click(object sender, EventArgs e)
        {
            if (hfRecordID.Value == "")
            {
                Alerta("Pulsa Edit en un historial de la tabla para modificarlo.");
                return;
            }

            if (!DatosHistorialValidos()) return;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE records SET AppointmentDate = @date, Diagnosis = @diag, Treatment = @treat " +
                        "WHERE RecordID = @id AND PatientID IN (" + MisPacientes + ");", con))
                    {
                        cmd.Parameters.AddWithValue("@date", recDate.Text);
                        cmd.Parameters.AddWithValue("@diag", recDiagnosis.Text.Trim());
                        cmd.Parameters.AddWithValue("@treat", recTreatment.Text.Trim());
                        cmd.Parameters.AddWithValue("@id", hfRecordID.Value);
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                Alerta("Historial actualizado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            btnClearRecord_Click(sender, e);
            ListRecords();
        }



        protected void btnClearRecord_Click(object sender, EventArgs e)
        {
            hfRecordID.Value = "";
            recDate.Text = "";
            recDiagnosis.Text = "";
            recTreatment.Text = "";
        }



        private void ListRecords()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter(
                "SELECT RecordID, AppointmentDate, Diagnosis, Treatment FROM records " +
                "WHERE PatientID = @p AND PatientID IN (" + MisPacientes + ") " +
                "ORDER BY AppointmentDate DESC", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@p", selPatient.SelectedValue);
                da.SelectCommand.Parameters.AddWithValue("@doc", Session["UserID"]);
                da.Fill(dt);
            }

            rptRecords.DataSource = dt;
            rptRecords.DataBind();
        }



        protected void rptRecords_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            string recordId = (string)e.CommandArgument;

            if (e.CommandName == "Edit")
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT AppointmentDate, Diagnosis, Treatment FROM records " +
                        "WHERE RecordID = @id AND PatientID IN (" + MisPacientes + ");", con))
                    {
                        cmd.Parameters.AddWithValue("@id", recordId);
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (!rd.Read()) { Alerta("Historial no encontrado."); return; }

                            recDate.Text = Convert.ToString(rd["AppointmentDate"]);
                            recDiagnosis.Text = Convert.ToString(rd["Diagnosis"]);
                            recTreatment.Text = Convert.ToString(rd["Treatment"]);
                        }
                    }
                }
                hfRecordID.Value = recordId;
                return;
            }

            if (e.CommandName != "Delete") return;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "DELETE FROM records WHERE RecordID = @id AND PatientID IN (" + MisPacientes + ");", con))
                    {
                        cmd.Parameters.AddWithValue("@id", recordId);
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                Alerta("Historial eliminado.");
            }
            catch (SQLiteException ex)
            {
                Alerta("No se pudo eliminar: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }

            if (hfRecordID.Value == recordId) btnClearRecord_Click(source, e);
            ListRecords();
        }



        // ===================== CITAS =====================



        protected void btnScheduleAppointment_Click(object sender, EventArgs e)
        {
            if (!EsMiPaciente(dapPatient.SelectedValue))
            {
                Alerta("Selecciona un paciente.");
                return;
            }

            DateTime fecha;
            if (!DateTime.TryParse(dapDate.Text, out fecha) || fecha.Date < DateTime.Today)
            {
                Alerta("La fecha es obligatoria y no puede ser anterior a hoy.");
                return;
            }

            // Horario de 09:00 a 19:30, en tramos de 30 minutos
            TimeSpan hora;
            if (!TimeSpan.TryParse(dapTime.Text, out hora) ||
                hora.TotalMinutes < 540 || hora.TotalMinutes > 1170 || hora.TotalMinutes % 30 != 0)
            {
                Alerta("La hora debe ser entre 09:00 y 19:30, en tramos de 30 minutos.");
                return;
            }

            string fechaTxt = fecha.ToString("yyyy-MM-dd");
            string horaTxt = hora.ToString(@"hh\:mm");
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();

                    // ¿El doctor ya tiene una cita a esa hora?
                    using (var cmd = new SQLiteCommand(
                        "SELECT 1 FROM appointments WHERE DoctorID = @doc AND AppointmentDate = @date " +
                        "AND AppointmentTime = @time AND Status = 'Scheduled';", con))
                    {
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        cmd.Parameters.AddWithValue("@date", fechaTxt);
                        cmd.Parameters.AddWithValue("@time", horaTxt);
                        if (cmd.ExecuteScalar() != null)
                        {
                            Alerta("Ya tienes una cita a esa hora.");
                            return;
                        }
                    }

                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, Status) " +
                        "VALUES (@p, @doc, @date, @time, @reason, 'Scheduled');", con))
                    {
                        cmd.Parameters.AddWithValue("@p", dapPatient.SelectedValue);
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        cmd.Parameters.AddWithValue("@date", fechaTxt);
                        cmd.Parameters.AddWithValue("@time", horaTxt);
                        cmd.Parameters.AddWithValue("@reason", dapReason.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }
                Alerta("Cita agendada.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            dapPatient.ClearSelection();
            dapDate.Text = dapTime.Text = dapReason.Text = "";
            ListAgenda();
        }



        private void ListAgenda()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter(
                @"SELECT a.AppointmentID, a.AppointmentDate, a.AppointmentTime, a.Reason, a.Status,
                         IFNULL(p.Name, '') AS PatientName
                  FROM appointments a
                  LEFT JOIN patients p ON p.UserID = a.PatientID
                  WHERE a.DoctorID = @doc
                  ORDER BY a.AppointmentDate, a.AppointmentTime", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@doc", Session["UserID"]);
                da.Fill(dt);
            }

            rptAgenda.DataSource = dt;
            rptAgenda.DataBind();
        }



        protected void rptAgenda_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string nuevoEstado;
            if (e.CommandName == "CompleteAppt") nuevoEstado = "Completed";
            else if (e.CommandName == "CancelAppt") nuevoEstado = "Cancelled";
            else return;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE appointments SET Status = @status " +
                        "WHERE AppointmentID = @id AND DoctorID = @doc AND Status = 'Scheduled';", con))
                    {
                        cmd.Parameters.AddWithValue("@status", nuevoEstado);
                        cmd.Parameters.AddWithValue("@id", (string)e.CommandArgument);
                        cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }

            ListAgenda();
        }



        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (curPass.Text == "" || newPass.Text == "" || confirmPass.Text == "")
            {
                Alerta("Rellena todos los campos.");
                return;
            }

            if (newPass.Text != confirmPass.Text)
            {
                Alerta("Las contraseñas nuevas no coinciden.");
                return;
            }

            if (!Regex.IsMatch(newPass.Text, @"^(?=.*[A-Za-z])(?=.*\d).{6,}$"))
            {
                Alerta("La nueva contraseña debe tener al menos 6 caracteres, con letras y números.");
                return;
            }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(
                        "UPDATE login SET Password = @new WHERE UserID = @me AND Password = @old;", con))
                    {
                        cmd.Parameters.AddWithValue("@new", newPass.Text);
                        cmd.Parameters.AddWithValue("@old", curPass.Text);
                        cmd.Parameters.AddWithValue("@me", Session["UserID"]);

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            Alerta("La contraseña actual no es correcta.");
                            return;
                        }
                    }
                }
                Alerta("Contraseña cambiada.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }
        }



        // ===================== AYUDAS =====================



        private void LoadPatientsDropdowns()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
            var dt = new DataTable();

            using (var con = new SQLiteConnection(cadena))
            using (var da = new SQLiteDataAdapter(
                "SELECT UserID, Name FROM patients WHERE UserID IN (" + MisPacientes + ") ORDER BY Name", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@doc", Session["UserID"]);
                da.Fill(dt);
            }

            foreach (var lista in new[] { selPatient, dapPatient })
            {
                lista.DataSource = dt;
                lista.DataTextField = "Name";
                lista.DataValueField = "UserID";
                lista.DataBind();
                lista.Items.Insert(0, new ListItem("-- Select a patient --", ""));
            }
        }



        // Comprueba en el servidor que el paciente pertenece a este doctor
        private bool EsMiPaciente(string patientId)
        {
            if (patientId == "") return false;

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            using (var con = new SQLiteConnection(cadena))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT 1 FROM patients WHERE UserID = @p AND UserID IN (" + MisPacientes + ");", con))
                {
                    cmd.Parameters.AddWithValue("@p", patientId);
                    cmd.Parameters.AddWithValue("@doc", Session["UserID"]);
                    return cmd.ExecuteScalar() != null;
                }
            }
        }



        private bool DatosHistorialValidos()
        {
            if (recDate.Text == "" || recDiagnosis.Text.Trim() == "" || recTreatment.Text.Trim() == "")
            {
                Alerta("Fecha, diagnóstico y tratamiento son obligatorios.");
                return false;
            }
            return true;
        }



        private void Alerta(string texto)
        {
            ClientScript.RegisterStartupScript(GetType(), "msg",
                "alert('" + texto.Replace("'", "\\'") + "');", true);
        }
    }
}