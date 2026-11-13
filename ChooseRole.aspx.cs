using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace ProyectoEnglish
{
    public partial class ChooseRole : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var roles = Session["Roles"] as List<string>;

            if (roles == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            loggedUser.InnerText = Convert.ToString(Session["Username"]);

            if (!IsPostBack)
            {
                rptRoles.DataSource = roles;
                rptRoles.DataBind();
            }
        }

        protected void rptRoles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var roles = Session["Roles"] as List<string>;
            string elegido = Convert.ToString(e.CommandArgument);
            if (roles == null || !roles.Contains(elegido)) return;

            Session["Role"] = elegido;

            switch (elegido)
            {
                case "Admin": Response.Redirect("AdminPage.aspx"); break;
                case "Doctor": Response.Redirect("DoctorPage.aspx"); break;
                case "Patient": Response.Redirect("PatientPage.aspx"); break;
            }
        }
    }
}