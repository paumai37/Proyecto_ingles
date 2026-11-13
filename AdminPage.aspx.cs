using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (docName.Text.Trim() == "") { Alerta("El nombre es obligatorio."); return; }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                using (var cmd = new SQLiteCommand("INSERT INTO login (Username, Password, Profile) VALUES (@nombre, '123456', 'Doctor');", con))
                {
                    cmd.Parameters.AddWithValue("@nombre", docName.Text.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                using (var con = new SQLiteConnection(cadena))
                using (var cmd = new SQLiteCommand("INSERT INTO doctors (UserID, Name, NationalID, Mobile) VALUES ((SELECT MAX(UserID) FROM login), @nombre, @nationalID, @mobile);", con))
                {
                    cmd.Parameters.AddWithValue("@nombre", docName.Text.Trim());
                    cmd.Parameters.AddWithValue("@nationalID", docId.Text.Trim());
                    cmd.Parameters.AddWithValue("@mobile", docMobile.Text.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
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
                        using (var cmd = new SQLiteCommand("DELETE FROM doctors WHERE UserID = @id", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SQLiteCommand("DELETE FROM login WHERE UserID = @id", con, tx))
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
            if (patName.Text.Trim() == "" || patDoctor.SelectedValue == "")
            {
                Alerta("Nombre y doctor son obligatorios.");
                return;
            }

            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            try
            {
                using (var con = new SQLiteConnection(cadena))
                using (var cmd = new SQLiteCommand("INSERT INTO login (Username, Password, Profile) VALUES (@nombre, '123456', 'Patient');", con))
                {
                    cmd.Parameters.AddWithValue("@nombre", patName.Text.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                using (var con = new SQLiteConnection(cadena))
                using (var cmd = new SQLiteCommand("INSERT INTO patients (UserID, Name, NationalID, Mobile, Address, DOB, FamilyDoctorID) VALUES ((SELECT MAX(UserID) FROM login), @nombre, @nationalID, @mobile, @address, @dob, @familyDoctorID);", con))
                {
                    cmd.Parameters.AddWithValue("@nombre", patName.Text.Trim());
                    cmd.Parameters.AddWithValue("@nationalID", patId.Text.Trim());
                    cmd.Parameters.AddWithValue("@mobile", patMobile.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", patAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@dob", patDob.Text);
                    cmd.Parameters.AddWithValue("@familyDoctorID", patDoctor.SelectedValue);
                    con.Open();
                    cmd.ExecuteNonQuery();
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
                        using (var cmd = new SQLiteCommand("DELETE FROM patients WHERE UserID = @id", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SQLiteCommand("DELETE FROM login WHERE UserID = @id", con, tx))
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
    }
}