using System;
using CyberArenaWebForms5.Models;

namespace CyberArenaWebForms5
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Initialize the database on application startup
            DatabaseHelper.InitializeDatabase();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends.
        }
    }
}
