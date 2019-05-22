using System.ComponentModel.DataAnnotations;

namespace TalkBack.DAL
{
    public class UserDb
    {
        public UserDb()
        {

        }


        public UserDb(string username, string password)
        {
            Name = username;
            Password = password;
        }


        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
