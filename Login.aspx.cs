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
            string rol = null;

            using (SQLiteConnection conn = new SQLiteConnection(cadena))
            {
                string sql = "SELECT Profile FROM login WHERE Username = @user AND Password = @pass";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", TextBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", TextBox2.Text);

                    conn.Open();
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        rol = resultado.ToString();
                    }
                }
            }

            if (rol == null)
            {
                lblMessage.Text = "Incorrect username or password.";
                return;
            }

            Session["Role"] = rol;

            if (rol == "Admin")
            {
                Response.Redirect("AdminPage.aspx");
            }
            else if (rol == "Doctor")
            {
                Response.Redirect("DoctorPage.aspx");
            }
            else if (rol == "Patient")
            {
                Response.Redirect("PatientPage.aspx");
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