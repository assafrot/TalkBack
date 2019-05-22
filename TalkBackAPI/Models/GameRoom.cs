using TalkBackAPI.BL;

namespace TalkBackAPI.Models
{
    public class GameRoom
    {
        public GameManager GameManager { get; set; }
        public string WhiteUser { get; set; }//connectionid
        public string BlackUser { get; set; }

        public GameRoom(string user1, string user2)
        {
            BlackUser = user1;
            WhiteUser = user2;
            GameManager = new GameManager(user1, user2);
        }
    }
}