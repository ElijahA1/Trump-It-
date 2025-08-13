using System.Collections.ObjectModel;

namespace Card_Game
{
    public class GameLogic
    {
        public Card TrumpCard { get; set; }

        private Stack<Card> deck = new Stack<Card>();

        private int[] arrayValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private string[] arraySuits = { "heart", "diamond", "club", "spade", "star" };

        // -- Game flow --
        public void PushCardsToDeck()
        {
            foreach (string suit in arraySuits)
            {
                foreach (int value in arrayValues)
                {
                    deck.Push(new Card(value, suit));
                }
            }
        }
        public void ShuffleDeck()
        {
            List<Card> deckToList = deck.ToList();
            Random randomIndex = new Random();

            for (int i = deckToList.Count - 1; i >= 0; i--)
            {
                int randomI = randomIndex.Next(i + 1);
                (deckToList[i], deckToList[randomI]) = (deckToList[randomI], deckToList[i]);

            }
            deck = new Stack<Card>(deckToList);
        }
        public void DealCards(int rounds)
        {
            Player.Hand = new List<Card>();
            Dealer.Hand = new List<Card>();
            Player.Tricks = 0;
            Dealer.Tricks = 0;

            // Add cards to hand lists, pop cards from deck stack
            for (int i = 0; i < rounds; i++)
            {
                Player.Hand.Add(deck.Pop());
                Dealer.Hand.Add(deck.Pop());
            }
            TrumpCard = deck.Pop();
        }
        public void DealerBid(int rounds) // Player enter bit and dealer 
        {
            if (Player.Bid == 0)
            {
                Dealer.Bid = rounds - 1;
            }
            else if (Player.Bid == rounds)
            {
                Dealer.Bid = 1;
            }
            else
            {
                Dealer.Bid = rounds - Player.Bid - 1;
            }
        }
        public void PlayerTurn(Card card)
        {
            Player.CardInPlay = card;
            RemoveCardFromList(Player.CardInPlay, Player.Hand);
        }
        public void DealersTurn()
        {
            List<Card> sameSuitCards = new List<Card>();
            // Add same suit cards to a new list
            foreach (Card card in Dealer.Hand)
            {
                if (card.Suit == Player.CardInPlay.Suit)
                {
                    sameSuitCards.Add(card);
                }
            }

            if (sameSuitCards.Count > 0)
            {
                // Select the first same suit card from list
                Dealer.CardInPlay = sameSuitCards[0];
            }
            else
            {
                Dealer.CardInPlay = Dealer.Hand[0];
            }
            // Remove cardInPlay from dealers hand
            RemoveCardFromList(Dealer.CardInPlay, Dealer.Hand);
        }
        public bool PlayerWon()
        {
            bool playerWins;
            bool playerHasHighCard = Player.CardInPlay.Value > Dealer.CardInPlay.Value;
            bool bothSameSuit = Player.CardInPlay.Suit == Dealer.CardInPlay.Suit;
            bool playerHasTrump = Player.CardInPlay.Suit == TrumpCard.Suit;
            bool dealerHasTrump = Dealer.CardInPlay.Suit == TrumpCard.Suit;

            if (bothSameSuit)
            {
                if (playerHasHighCard)
                {
                    Player.Tricks += 1;
                    Player.Points += 1;
                    playerWins = true;
                }
                else
                {
                    Dealer.Tricks += 1;
                    Dealer.Points += 1;
                    playerWins = false;
                }
            }
            else if (playerHasTrump)
            {
                Player.Tricks += 1;
                Player.Points += 1;
                playerWins = true;
            }
            else if (dealerHasTrump)
            {
                Dealer.Tricks += 1;
                Dealer.Points += 1;
                playerWins = false;
            }
            else
            {
                Player.Tricks += 1;
                Player.Points += 1;
                playerWins = true;
            }
            return playerWins;
        }
        public void AddBonusPoints()
        {
            if (Player.Bid == Player.Tricks)
            {
                Player.Points += 5;
            }
            if (Dealer.Bid == Dealer.Tricks)
            {
                Dealer.Points += 5;
            }
        }
        public void RemoveCardFromList(Card usedCard, List<Card> listOfCards)
        {
            var match = listOfCards.Find(card =>
                card.Value == usedCard.Value && card.Suit == usedCard.Suit);

            if (match != null)
                listOfCards.Remove(match);
        }
    }
}
