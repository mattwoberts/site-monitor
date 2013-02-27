using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Raven.Client.Embedded;

namespace WebFrontEnd
{
    public class Global : System.Web.HttpApplication
    {
        // TODO: Make nice...
        public static EmbeddableDocumentStore _store;

        protected void Application_Start(object sender, EventArgs e)
        {
            _store = new EmbeddableDocumentStore
            {
                DataDirectory = "~/AppData/Database",
                UseEmbeddedHttpServer = true,
            };
            _store.Configuration.Port = 8081;

            _store.Initialize();

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}