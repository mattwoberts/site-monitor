using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using SiteChecker.Domain;
using Raven.Client;

namespace WebFrontEnd
{
    public class SiteModule : NancyModule
    {
        private IDocumentSession _documentSession;

        public SiteModule(IDocumentSession documentStore)
        {
            _documentSession = documentStore;

            dynamic someModel = null;

            Get["/sites"] = parameters =>
                {
                    return View["Mockup.html", someModel];
                };

            Post["/site"] = parameters =>
                {
                    var siteUrl = Request.Form.url;

                    // Add the new site to the database
                    var site = new Site
                        {
                            CreationTime = DateTime.Now,
                            CurrentStatus = SiteStatusEnum.Up,
                            Url = siteUrl
                        };
                    _documentSession.Store(site);
                    _documentSession.SaveChanges();

                    return HttpStatusCode.OK;
                };


        }
    }
}