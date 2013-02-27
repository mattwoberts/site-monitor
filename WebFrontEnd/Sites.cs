using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using SiteChecker.Domain;

namespace WebFrontEnd
{
    public class Sites : NancyModule
    {
        public Sites()
        {
            dynamic someModel = null;
            
            Get["/sites"] = parameters =>
                {
                    return View["Mockup.html", someModel];
                };

            Post["/site"] = parameters =>
                {
                    var siteUrl = Request.Form.url;

                    // Add the new site to the database
                    using (var session = Global._store.OpenSession())
                    {
                        var site = new Site
                            {
                                CreationTime = DateTime.Now,
                                CurrentStatus = SiteStatusEnum.Up,
                                Url = siteUrl
                            };
                        session.Store(site);
                        session.SaveChanges();
                    }

                    return HttpStatusCode.OK;
                };


        }
    }
}