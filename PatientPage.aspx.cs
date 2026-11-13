using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Web.UI.WebControls;

namespace ProyectoEnglish
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        string Cadena
        {
            get { return ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString; }
        }

        long UserId
        {
            get { return (long)Session["UserID"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Solo entra quien eligió el rol Patient
            if (Session["Role"] == null || Session["Role"].ToString() != "Patient" || !(Session["UserID"] is long))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDoctors();
                LoadAppointments();
                LoadRecords();
                loggedUser.InnerText = Convert.ToString(Session["Username"]);
                LoadPersonalInfo();
                    
            }
        }

        // ===================== CARGA DE DATOS =====================

        void LoadDoctors()
        {
            var dt = new DataTable();

            using (var con = new SQLiteConnection(Cadena))
            using (var da = new SQLiteDataAdapter(
                "SELECT UserID, Name FROM doctors WHERE UserID <> @me ORDER BY Name", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@me", UserId);
                da.Fill(dt);
            }

            apDoctor.DataSource = dt;
            apDoctor.DataValueField = "UserID";
            apDoctor.DataTextField = "Name";
            apDoctor.DataBind();
            apDoctor.Items.Insert(0, new ListItem("-- Select doctor --", ""));
        }

        void LoadRecords()
        {
            var dt = new DataTable();

            using (var con = new SQLiteConnection(Cadena))
            using (var da = new SQLiteDataAdapter(
                @"SELECT r.AppointmentDate, r.Diagnosis, r.Treatment FROM records r JOIN patients p ON p.PatientID = r.PatientID WHERE p.UserID = @me
                ORDER BY r.AppointmentDate DESC", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@me", UserId);
                da.Fill(dt);
            }

            rptRecords.DataSource = dt;
            rptRecords.DataBind();
        }

        void LoadAppointments()
        {
            var dt = new DataTable();

            using (var con = new SQLiteConnection(Cadena))
            using (var da = new SQLiteDataAdapter(
                @"SELECT a.AppointmentID, a.AppointmentDate, a.AppointmentTime,
                         IFNULL(d.Name, '') AS DoctorName, a.Reason, a.Status
                  FROM appointments a
                  LEFT JOIN doctors d ON d.UserID = a.DoctorID
                  WHERE a.PatientID = @me
                  ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@me", UserId);
                da.Fill(dt);
            }

            rptAppointments.DataSource = dt;
            rptAppointments.DataBind();
        }

        void LoadPersonalInfo()
        {
            using (var con = new SQLiteConnection(Cadena))
            using (var cmd = new SQLiteCommand(
                @"SELECT p.Name, p.NationalID, p.DOB, p.Address, p.Mobile, IFNULL(d.Name, '') AS DoctorName
          FROM patients p
          LEFT JOIN doctors d ON d.UserID = p.FamilyDoctorID
          WHERE p.UserID = @me", con))
            {
                cmd.Parameters.AddWithValue("@me", UserId);
                con.Open();

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return;

                    patName.InnerText = Convert.ToString(rd["Name"]);
                    patId.InnerText = Convert.ToString(rd["NationalID"]);
                    patAddress.InnerText = Convert.ToString(rd["Address"]);
                    patMobile.InnerText = Convert.ToString(rd["Mobile"]);
                    patDoctor.InnerText = Convert.ToString(rd["DoctorName"]);

                    // La fecha se guarda como yyyy-MM-dd; se muestra como dd/MM/yyyy
                    string dobTxt = Convert.ToString(rd["DOB"]);
                    DateTime dob;
                    patDob.InnerText = DateTime.TryParseExact(dobTxt, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out dob)
                                       ? dob.ToString("dd/MM/yyyy")
                                       : dobTxt;
                }
            }
        }

        // ===================== RESERVAR CITA =====================

        protected void btnBookAppointment_Click(object sender, EventArgs e)
        {
            if (apDoctor.SelectedValue == "" || apDate.Text == "" || apTime.Text == "")
            {
                Alerta("Elige doctor, fecha y hora.");
                return;
            }

            DateTime fecha, hora;
            if (!DateTime.TryParseExact(apDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out fecha) ||
                !DateTime.TryParseExact(apTime.Text, new[] { "HH:mm", "HH:mm:ss" }, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out hora))
            {
                Alerta("Fecha u hora no válidas.");
                return;
            }

            TimeSpan h = hora.TimeOfDay;

            if (h < new TimeSpan(9, 0, 0) || h > new TimeSpan(19, 30, 0))
            {
                Alerta("El horario de citas es de 09:00 a 19:30.");
                return;
            }

            if (fecha.Date + h <= DateTime.Now)
            {
                Alerta("La cita debe ser en el futuro.");
                return;
            }

            long doctorId = long.Parse(apDoctor.SelectedValue);
            string fechaTxt = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string horaTxt = hora.ToString("HH:mm", CultureInfo.InvariantCulture);

            try
            {
                using (var con = new SQLiteConnection(Cadena))
                {
                    con.Open();

                    // ¿El doctor ya tiene una cita activa a esa hora?
                    using (var cmd = new SQLiteCommand(
                        "SELECT 1 FROM appointments WHERE DoctorID = @d AND AppointmentDate = @f " +
                        "AND AppointmentTime = @h AND Status <> 'Cancelled'", con))
                    {
                        cmd.Parameters.AddWithValue("@d", doctorId);
                        cmd.Parameters.AddWithValue("@f", fechaTxt);
                        cmd.Parameters.AddWithValue("@h", horaTxt);

                        if (cmd.ExecuteScalar() != null)
                        {
                            Alerta("Ese hueco ya está ocupado. Elige otra hora.");
                            return;
                        }
                    }

                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason) " +
                        "VALUES (@p, @d, @f, @h, @r)", con))
                    {
                        cmd.Parameters.AddWithValue("@p", UserId);
                        cmd.Parameters.AddWithValue("@d", doctorId);
                        cmd.Parameters.AddWithValue("@f", fechaTxt);
                        cmd.Parameters.AddWithValue("@h", horaTxt);
                        cmd.Parameters.AddWithValue("@r", apReason.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }

                Alerta("Cita reservada.");
            }
            catch (SQLiteException ex)
            {
                Alerta("Error: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                return;
            }

            apDoctor.SelectedIndex = 0;
            apDate.Text = apTime.Text = apReason.Text = "";
            LoadAppointments();
        }

        // ===================== CANCELAR CITA =====================

        protected void rptAppointments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "CancelAppt") return;

            try
            {
                using (var con = new SQLiteConnection(Cadena))
                // Solo cancela citas propias y que sigan activas
                using (var cmd = new SQLiteCommand(
                    "UPDATE appointments SET Status = 'Cancelled' " +
                    "WHERE AppointmentID = @id AND PatientID = @me AND Status = 'Scheduled'", con))
                {
                    cmd.Parameters.AddWithValue("@id", long.Parse((string)e.CommandArgument));
                    cmd.Parameters.AddWithValue("@me", UserId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Alerta("Cita cancelada.");
            }
            catch (SQLiteException ex)
            {
                Alerta("No se pudo cancelar: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }

            LoadAppointments();
        }

        // ===================== AYUDAS =====================

        private void Alerta(string texto)
        {
            ClientScript.RegisterStartupScript(GetType(), "msg",
                "alert('" + texto.Replace("'", "\\'") + "');", true);
        }
    }
}