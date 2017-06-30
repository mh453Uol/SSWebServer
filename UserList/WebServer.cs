using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace UserList
{
    public class WebServer
    {
        public HttpListener Listener;
        public List<UriBuilder> UriPrefixes { get; set; }
        public int Port { get; set; }
        public RouteEngine RoutesManager { get; set; }

        public WebServer(string[] urlPrefixes, int port)
        {
            this.Port = port;
            this.UriPrefixes = new List<UriBuilder>();
            this.UriPrefixes = ToUriBuilder(urlPrefixes);
            Listener = new HttpListener();
            RoutesManager = new RouteEngine();
        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            if (!IsPrefixesValid())
            {
                throw new ArgumentException("No Prefixes");
            }

            // Add the prefixes for server to listen to.
            foreach (UriBuilder prefix in UriPrefixes)
            {
                Listener.Prefixes.Add(prefix.Uri.OriginalString);
            }
            // Start the web server
            Listener.Start();
            Console.WriteLine("Listening on PORT: " + Port + " URI: " + String.Join(",", UriPrefixes));
            HandleIncomingRequest();
        }
        public void HandleIncomingRequest()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (Listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((context) =>
                    {
                        var HttpContext = (HttpListenerContext)context;

                        var request = HttpContext.Request;
                        var response = HttpContext.Response;

                        var route = RoutesManager.Routes.FirstOrDefault(r => r.HttpMethod == request.HttpMethod &&
                            r.URLMatch.Match(request.Url.AbsolutePath).Success);

                        if (route == null)
                        {
                            //Route not defined display a error message
                            RoutesManager.NotDefinedRoute.Handler(request, response);
                        }
                        else
                        {
                            route.Handler(request, response);
                        }

                    }, Listener.GetContext());
                }
            });
        }

        public bool IsPrefixesValid()
        {
            // URI prefixes are required, since the server need to listen to a uri
            if (UriPrefixes.Any(u => string.IsNullOrEmpty(u.Uri.AbsolutePath)))
            {
                return false;
            }
            return true;
        }

        public List<UriBuilder> ToUriBuilder(string[] urlPrefixes)
        {
            var urls = new List<UriBuilder>();

            foreach (var url in urlPrefixes)
            {
                var eachUrl = new UriBuilder(url);
                eachUrl.Port = Port;

                urls.Add(eachUrl);
            }

            return urls;
        }

        public void Stop()
        {
            Listener.Stop();
            Listener.Close();
        }
    }
}
