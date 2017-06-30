using System;
using System.Net;
using System.Text.RegularExpressions;

namespace UserList
{
    public class Route
    {
        public string HttpMethod { get; set; }
        public Regex URLMatch { get; set; }

        // Handler method which gets called when a route is found.
        public Func<HttpListenerRequest, HttpListenerResponse, HttpListenerResponse> Handler { get; set; }
    }
}
