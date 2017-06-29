using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UserList
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer(new String[] { "http://localhost:" }, 8090);

            server.routes.AddRange(new List<Route>
            {
                new Route()
                {
                    HttpMethod = Helper.HTTPMethod.GET,
                    URLMatch = new Regex(@"^/user/[a-zA-Z]+/age/\d+$"),
                    Handler = (request,response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var url = Helper.SplitURL(request.Url.AbsolutePath);
                        var username = url[2];
                        var age = int.Parse(url[4]);
                        var user = DataStore.Users.FirstOrDefault(n => n.Name == username && n.Age == age);

                        if(user == null)
                        {
                            response.StatusCode = (int) Helper.HttpStatus.NOTFOUND;
                        }

                        return response;
                    }
                },
                new Route()
                {
                    HttpMethod = Helper.HTTPMethod.GET,
                    URLMatch = new Regex(@"^/user$"),
                    Handler = (request,response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var users = DataStore.Users.Select(u => u.ToString());
                        var output = string.Join(", ",users);
                        return response;
                    }
                }
            });
            server.Start();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            server.Stop();
        }
    }
}
