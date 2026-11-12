using System;
using System.Collections.Generic;
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
            }
        }
        protected void btnCreateDoctor_Click(object sender, EventArgs e) { }
        protected void btnUpdateDoctor_Click(object sender, EventArgs e) { }
        protected void btnCreatePatient_Click(object sender, EventArgs e) { }
        protected void btnUpdatePatient_Click(object sender, EventArgs e) { }
    }





}