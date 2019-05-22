namespace Client.Models
{
    public class User
    {

        public User(string userName, string password)
        {
            Name = userName;
            Password = password;
        }

        public string Name { get; set; }
        public string Password { get; set; }

    }
}
