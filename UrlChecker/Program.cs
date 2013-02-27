using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Embedded;
using SiteChecker.Domain;

namespace UrlChecker
{
    internal class Program
    {
        private static EmbeddableDocumentStore _store;

        private static void Main(string[] args)
        {
            // Try to fire up an embedded raven
            _store = new EmbeddableDocumentStore
                {
                    DataDirectory = "~/AppData/Database",
                    UseEmbeddedHttpServer = true,
                };
            _store.Configuration.Port = 8081;
    
            _store.Initialize();

            Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var sites = LoadSitesToCheck();

                        foreach (var site in sites)
                        {
                            CheckPage(site.Url);
                        }
                        
                        Console.WriteLine("Waiting for next pings");
                        Thread.Sleep(1000 * 60);
                    }
                });

            Console.WriteLine("ok");
            Console.ReadLine();

        }

        private static List<Site> LoadSitesToCheck()
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                return session.Query<Site>().Take(200).ToList();
            }
        }

        private static async void CheckPage(string url)
        {
            var currentStatus = SiteStatusEnum.Up;
            HttpWebRequest request = WebRequest.CreateHttp(url);
            try
            {
                var responseAsync = (HttpWebResponse) await request.GetResponseAsync();
                Console.WriteLine("Done geting the response, status code was {0}", responseAsync.StatusCode);
                if (responseAsync.StatusCode != HttpStatusCode.OK)
                {
                    currentStatus = SiteStatusEnum.Down;
                }
            }
            catch (WebException e)
            {
                Console.WriteLine("Exception trying to reach " + url);
                currentStatus = SiteStatusEnum.Down;
            }

            UpdateResults(url, currentStatus);
        }

        private static void UpdateResults(string url, SiteStatusEnum currentStatus)
        {
            using (IDocumentSession session = _store.OpenSession())
            {
                // Find or add the site
                Site site = session.Query<Site>().FirstOrDefault(s => s.Url == url);
                if (site == null)
                {
                    site = new Site
                        {
                            CreationTime = DateTime.Now,
                            CurrentStatus = SiteStatusEnum.Up,
                            Url = url,
                        };
                    session.Store(site);
                }

                site.LastCheckedTime = DateTime.Now;

                if (currentStatus == SiteStatusEnum.Down)
                {
                    // If not already known to be down, add a new issue
                    if (site.CurrentStatus != SiteStatusEnum.Down)
                    {
                        site.SiteDownIssues.Add(new SiteDownIssue {TimeDown = DateTime.Now});
                    }
                }
                else
                {
                    // If not already up, log the fact that the site is now back up
                    if (site.CurrentStatus != SiteStatusEnum.Up)
                    {
                        site.SiteDownIssues.Last().TimeUp = DateTime.Now;
                    }
                }

                site.CurrentStatus = currentStatus;
                session.SaveChanges();
            }
        }
    }
}