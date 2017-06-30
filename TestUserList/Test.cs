using System.Net.Http;
using System.Threading.Tasks;
using UserList;

namespace TestUserList
{
    public class Test
    {
        public WebServer WebServer { get; set; }
        public HttpClient Client { get; set; }
        public string Url { get; set; }

        public Test(string url)
        {
            this.Url = url;
        }

        public async Task<HttpResponseMessage> GetUser(string name)
        {
            var url = string.Format("{0}/user/{1}", Url, name);
            var response = await Client.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> AddUser(string name, int age)
        {
            var url = string.Format("{0}/user/{1}/age/{2}", Url, name, age.ToString());
            var response = await Client.PostAsync(url, null);

            return response;
        }
    }
}
