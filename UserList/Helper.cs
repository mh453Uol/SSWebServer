namespace UserList
{
    public static class Helper
    {
        public static class HTTPMethod
        {
            public const string GET = "GET";
            public const string POST = "POST";
            public const string DELETE = "DELETE";
            public const string PUT = "PUT";
        }

        public enum HttpStatus
        {
            OK = 200,
            BADREQUEST = 400,
            NOTFOUND = 405
        }

        public static string[] SplitURL(string url)
        {
            return url.Split('/');
        }
    }


}
