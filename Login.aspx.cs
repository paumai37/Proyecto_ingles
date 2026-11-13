using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data;
using System.Data.SQLite;


namespace ProyectoEnglish
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateUsers.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            inicioSesion();
        }

        void inicioSesion()
        {
            string cadena = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

            long userId = 0;
            string perfil = null;
            string username = TextBox1.Text.Trim();

            using (var con = new SQLiteConnection(cadena))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(
                    "SELECT UserID, Profile FROM login WHERE Username = @user AND Password = @pass", con))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", TextBox2.Text);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            userId = rd.GetInt64(0);
                            perfil = rd.GetString(1);
                        }
                    }
                }

                if (perfil == null)
                {
                    lblMessage.Text = "Incorrect username or password.";
                    return;
                }

                var roles = new List<string> { perfil };   // rol principal

                if (perfil != "Patient")                   // Admin o Doctor: ¿también es paciente?
                {
                    using (var cmd = new SQLiteCommand(
                        "SELECT 1 FROM patients WHERE UserID = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        if (cmd.ExecuteScalar() != null) roles.Add("Patient");
                    }
                }

                Session["UserID"] = userId;
                Session["Username"] = username;
                Session["Roles"] = roles;

                if (roles.Count == 1)
                {
                    Session["Role"] = roles[0];

                    switch (roles[0])
                    {
                        case "Admin": Response.Redirect("AdminPage.aspx"); break;
                        case "Doctor": Response.Redirect("DoctorPage.aspx"); break;
                        case "Patient": Response.Redirect("PatientPage.aspx"); break;
                        default:
                            lblMessage.Text = "Unknown profile.";
                            break;
                    }
                }
                else
                {
                    Response.Redirect("ChooseRole.aspx");
                }
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}