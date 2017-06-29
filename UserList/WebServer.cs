using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace UserList
{
    public class WebServer
    {
        public HttpListener listener;
        public List<UriBuilder> uriPrefixes { get; set; }
        public int port { get; set; }
        public List<Route> routes { get; set; }

        public WebServer(string[] urlPrefixes, int port)
        {
            this.port = port;
            this.routes = new List<Route>();
            this.uriPrefixes = new List<UriBuilder>();
            this.uriPrefixes = ToUriBuilder(urlPrefixes);
            listener = new HttpListener();
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
            foreach (UriBuilder prefix in uriPrefixes)
            {
                listener.Prefixes.Add(prefix.Uri.OriginalString);
            }
            // Start the web server
            listener.Start();
            Console.WriteLine("Listening on PORT: " + port + " URI: " + String.Join(",", uriPrefixes));
            HandleIncomingRequest();
        }
        public void HandleIncomingRequest()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((context) =>
                    {
                        var HttpContext = (HttpListenerContext)context;

                        var request = HttpContext.Request;
                        var response = HttpContext.Response;

                        var route = routes.FirstOrDefault(r => r.HttpMethod == request.HttpMethod &&
                            r.URLMatch.Match(request.Url.AbsolutePath).Success);

                        var generatedResponse = route.Handler(request, response);

                        response.OutputStream.Close();

                    }, listener.GetContext());
                }
            });
        }

        public bool IsPrefixesValid()
        {
            // URI prefixes are required, since the server need to listen to a uri
            if (uriPrefixes.Any(u => string.IsNullOrEmpty(u.Uri.AbsolutePath)))
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
                eachUrl.Port = port;

                urls.Add(eachUrl);
            }

            return urls;
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }
    }
}
