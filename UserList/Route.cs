using System;
using System.Net;
using System.Text.RegularExpressions;

namespace UserList
{
    public class Route
    {
        public string HttpMethod { get; set; }
        public Regex URLMatch { get; set; }
        public Func<HttpListenerRequest, HttpListenerResponse, HttpListenerResponse> Handler { get; set; }
    }
}
