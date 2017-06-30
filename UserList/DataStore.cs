using System.Collections.Generic;

namespace UserList
{
    public class DataStore
    {
        public List<User> Users = new List<User>()
        {
            new User()
            {
                Name = "Majid",
                Age = 21
            },
            new User()
            {
                Name = "Alex",
                Age = 22
            },
            new User()
            {
                Name = "Ashley",
                Age = 19
            },
            new User()
            {
                Name = "Matthew",
                Age = 30
            },
            new User()
            {
                Name = "Waqar",
                Age = 36
            },
            new User()
            {
                Name = "Richard",
                Age = 32
            }
        };
    }
}
