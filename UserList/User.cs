using System;

namespace UserList
{
    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0} - Age: {1}", Name, Age);
        }
    }
}
