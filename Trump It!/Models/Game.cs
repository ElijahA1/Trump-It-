using System.Collections.ObjectModel;

namespace Card_Game
{
    public class Game
    {
        public Player Player = new();
        public Player Dealer = new();
        public Card? TrumpCard { get; set; }

        private List<Card> deckOfCards = new List<Card>();
        private int[] cardValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private string[] cardSuits = { "heart", "diamond", "club", "spade"};

        // -- Game flow --
        public void AddCardsToDeck()
        {
            deckOfCards.Clear();
            deckOfCards.AddRange(
                cardSuits.SelectMany(suit => 
                cardValues.Select(value => 
                new Card(value, suit)))
            );
        }
        public void ShuffleCardsInDeck()
        {
            Random rng = new Random();

            for (int i = deckOfCards.Count - 1; i >= 0; i--)
            {
                int randomIndex = rng.Next(i + 1);
                (deckOfCards[i], deckOfCards[randomIndex]) = (deckOfCards[randomIndex], deckOfCards[i]);
            }
        }
        public void DealCardsForRound(int rounds)
        {
            Player.ResetValues();
            Dealer.ResetValues();

            while (rounds > 0)
            {
                Player.CardsInHand.Add(deckOfCards[0]);
                deckOfCards.RemoveAt(0);

                Dealer.CardsInHand.Add(deckOfCards[0]);
                deckOfCards.RemoveAt(0);
                rounds--;
            }
            TrumpCard = deckOfCards[0];
            deckOfCards.RemoveAt(0);
        }
        public void SetDealersBid(int rounds) 
        {
            // Sum of dealer's and player's bid must avoid matching total winnable tricks(hands)
            Dealer.CurrentBid = Player.CurrentBid switch
            {
                0 => rounds - 1,
                int bid when bid == rounds => 1,
                _ => rounds - Player.CurrentBid - 1
            };
        }
        public void DealerPlaysCard()
        {
            // Dealer must play a card of the same suit as players cardInPlay if the able
            Card sameSuitCard = Dealer.CardsInHand.FirstOrDefault(dealerCard => dealerCard.Suit == Player.CardInPlay.Suit);
            Dealer.CardInPlay = sameSuitCard ?? Dealer.CardsInHand[0];
            Dealer.CardsInHand.Remove(Dealer.CardInPlay);
        }
        public bool PlayerWonHand()
        {
            if (TrumpCard == null || Player.CardInPlay == null || Dealer.CardInPlay == null)
                return false;
            bool playerHasHigherCard = Player.CardInPlay.Value > Dealer.CardInPlay.Value;
            bool sameSuit = Player.CardInPlay.Suit == Dealer.CardInPlay.Suit;
            bool playerHasTrump = Player.CardInPlay.Suit == TrumpCard.Suit;
            bool dealerHasTrump = Dealer.CardInPlay.Suit == TrumpCard.Suit;

            if (sameSuit)
            {
                if (playerHasHigherCard)
                {
                    AwardPoints(Player);
                    return true;
                }
                else
                {
                    AwardPoints(Dealer);
                    return false;
                }
            }
            else if (playerHasTrump)
            {
                AwardPoints(Player);
                return true;
            }
            else if (dealerHasTrump)
            {
                AwardPoints(Dealer);
                return false;
            }

            AwardPoints(Player); // Default win
            return true;
        }
        private void AwardPoints(Player player) 
        {
            player.TricksWon += 1;
            player.TotalPoints += 1;
        }
        public void AddBonusPoints()
        {
            if (Player.CurrentBid == Player.TricksWon)
            {
                Player.TotalPoints += 5;
            }
            if (Dealer.CurrentBid == Dealer.TricksWon)
            {
                Dealer.TotalPoints += 5;
            }
        }
    }
}
