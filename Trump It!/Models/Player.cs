using System.Collections.ObjectModel;

namespace Card_Game
{
    public class Player
    {
        public static List<Card>? Hand { get; set; }
        public static int Bid { get; set; }
        public static Card? CardInPlay { get; set; }
        public static int Tricks { get; set; }
        public static int Points { get; set; }
    }
}
