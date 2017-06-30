using System.Collections.Generic;
using System.Net;

namespace UserList
{
    public class RouteEngine
    {
        public List<Route> Routes { get; set; }
        public Route NotDefinedRoute { get; set; }
        public RouteEngine()
        {
            Routes = new List<Route>();
            NotDefinedRoute = new Route();
        }

        public void ConstructResponse(HttpListenerResponse response, string responseBody)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseBody);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // closing the output stream.
            output.Close();
        }
    }
}
