using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Conventions;
using Raven.Client.Embedded;
using Raven.Client.Document;
using Raven.Client;

namespace WebFrontEnd
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));

        }

  
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // Configure the store
            var store = new EmbeddableDocumentStore
            {
                DataDirectory = "~/AppData/Database",
                UseEmbeddedHttpServer = true,
            };
            store.Configuration.Port = 8081;
            store.Initialize();

            container.Register(store, "DocStore");
        }

        protected override void ConfigureRequestContainer(Nancy.TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            var docStore = container.Resolve<EmbeddableDocumentStore>("DocStore");
            var documentSession = docStore.OpenSession();
            container.Register<IDocumentSession>(documentSession);
        }
    }
}