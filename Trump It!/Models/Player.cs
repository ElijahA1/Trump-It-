using System.Collections.ObjectModel;

namespace Card_Game
{
    public class Player
    {
         public List<Card>? CardsInHand { get; set; }
         public Card? CardInPlay { get; set; }
         public int CurrentBid { get; set; }
         public int TricksWon { get; set; }
         public int TotalPoints { get; set; }

        
         public void ResetValues()
         {
             CardsInHand = new List<Card> { };
             CardInPlay = null;
             CurrentBid = 0;
             TricksWon = 0;
         }
    }
}
