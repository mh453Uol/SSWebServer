using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserList;

namespace TestUserList
{
    class Program
    {
        public static void Main(string[] args)
        {
            DataStore data = new DataStore();

            WebServer server = SetUpServer(data);
            server.Start();

            User user = data.Users[0];

            string baseUrl = "http://localhost:8090";

            HttpClient client = new HttpClient();

            var testOne = new Test(baseUrl);
            var testTwo = new Test(baseUrl);

            testOne.Client = client;
            testTwo.Client = client;

            Task task = MainAsync(testOne, testTwo, user);
            task.Wait();
        }

        static async Task MainAsync(Test t1, Test t2, User user)
        {
            var t1UserRequest = t1.GetUser(user.Name);

            var t2UserRequest = t2.GetUser(user.Name);

            var t1UserResponse = await t1UserRequest;

            var t2UserResponse = await t2UserRequest;

            Log(t1UserResponse);

            Log(t2UserResponse);
        }

        public static WebServer SetUpServer(DataStore data)
        {
            WebServer server = new WebServer(new String[] { "http://localhost:" }, 8090);

            server.RoutesManager.Routes.AddRange(new List<Route>
            {
                new Route()
                {
                    HttpMethod = Helper.HTTPMethod.GET,
                    URLMatch = new Regex(@"^/user/[a-zA-Z]+$",RegexOptions.IgnoreCase),
                    Handler = (request, response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var url = Helper.SplitURL(request.Url.AbsolutePath);
                        var username = url[2];
                        var user = data.Users.FirstOrDefault(n => n.Name == username);

                        if (user == null)
                        {
                            response.StatusCode = (int)Helper.HttpStatus.NOTFOUND;
                            server.RoutesManager.ConstructResponse(response,
                                string.Format("Could not find user {0}", username));
                            return response;
                        }
                        server.RoutesManager.ConstructResponse(response,
                            string.Format("{0}", user.Age));

                        return response;
                    }
                },
                new Route() {
                    HttpMethod = Helper.HTTPMethod.GET,
                    URLMatch = new Regex(@"^/user$",RegexOptions.IgnoreCase),
                    Handler = (request, response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var users = data.Users.Select(u => u.ToString());
                        var output = string.Join(", ", users);

                        server.RoutesManager.ConstructResponse(response,
                            output);

                        return response;
                    }
                },
                new Route() {
                    HttpMethod = Helper.HTTPMethod.POST,
                    URLMatch = new Regex(@"^/user/[a-zA-Z]+/age/\d+$",RegexOptions.IgnoreCase),
                    Handler = (request, response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var url = Helper.SplitURL(request.Url.AbsolutePath);
                        var username = url[2];
                        var age = int.Parse(url[4]);
                        var user = data.Users.FirstOrDefault(n => n.Name == username &&
                        n.Age == age);

                        if (user == null)
                        {
                            data.Users.Add(new User()
                            {
                                Name = username,
                                Age = age
                            });

                            response.StatusCode = (int)Helper.HttpStatus.OK;
                            server.RoutesManager.ConstructResponse(response,
                                string.Format("Successful added user {0} age {1}", username, age));
                            return response;
                        }

                        server.RoutesManager.ConstructResponse(response,
                            string.Format("Error user {0} age {1} already exist", username, age));

                        return response;
                    }
                },
                new Route()
                {

                    HttpMethod = Helper.HTTPMethod.PUT,
                    URLMatch = new Regex(@"^/user/[a-zA-Z]+/age/\d+$",RegexOptions.IgnoreCase),
                    Handler = (request, response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var url = Helper.SplitURL(request.Url.AbsolutePath);
                        var username = url[2];
                        var age = int.Parse(url[4]);
                        var user = data.Users.FirstOrDefault(n => n.Name == username);

                        if (user != null)
                        {
                            data.Users.FirstOrDefault(n => n.Name == username).Age = age;

                            response.StatusCode = (int)Helper.HttpStatus.OK;
                            server.RoutesManager.ConstructResponse(response,
                                string.Format("Successful update user {0} with age {1}", username, age));
                            return response;
                        }

                        server.RoutesManager.ConstructResponse(response,
                            string.Format("Error user {0} age {1} does not exist", username, age));

                        return response;
                    }
                },
                new Route() {
                    HttpMethod = Helper.HTTPMethod.DELETE,
                    URLMatch = new Regex(@"^/user/[a-zA-Z]+$",RegexOptions.IgnoreCase),
                    Handler = (request, response) =>
                    {
                        Console.WriteLine("Handling " + request.RawUrl);
                        var url = Helper.SplitURL(request.Url.AbsolutePath);
                        var username = url[2];
                        var user = data.Users.FirstOrDefault(n => n.Name == username);

                        if (user == null)
                        {
                            response.StatusCode = (int)Helper.HttpStatus.NOTFOUND;
                            server.RoutesManager.ConstructResponse(response,
                                string.Format("Could not find user {0}", username));
                            return response;
                        }
                        data.Users.Remove(user);
                        server.RoutesManager.ConstructResponse(response,
                            string.Format("Removed user {0}", username));

                        return response;
                    }
                }
            });

            server.RoutesManager.NotDefinedRoute = new Route()
            {
                HttpMethod = Helper.HTTPMethod.GET,
                URLMatch = new Regex(@"", RegexOptions.IgnoreCase),
                Handler = (request, response) =>
                {
                    Console.WriteLine("Handling " + request.RawUrl);
                    response.StatusCode = (int)Helper.HttpStatus.NOTFOUND;
                    server.RoutesManager.ConstructResponse(response,
                        string.Format(@"Sorry the url is not regestered in the route manager.
                        Observe the route manager to see active routes. Thanks"));
                    return response;
                }
            };

            return server;
        }
        public static void Log(HttpResponseMessage response)
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine(String.Format("Request {0} -> Response : {1} -> Success: {2}",
                response.RequestMessage.RequestUri, response.Content.ReadAsStringAsync().Result, response.IsSuccessStatusCode));
        }
    }
}
