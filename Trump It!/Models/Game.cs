using SkiaSharp.Extended.UI.Controls;

namespace Class_Practice
{
    public class Game
    {
        public Card trumpCard { get; set; }

        private Stack<Card> deck = new Stack<Card>();

        private int[] arrayValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        private string[] arraySuits = { "heart", "diamond", "club", "spade", "star" };

        // Methods to run a game
        public void pushCardsToDeck()
        {
            foreach (string suit in arraySuits)
            {
                foreach (int value in arrayValues)
                {
                    deck.Push(new Card(value, suit));
                }
            }
        }
        public void shuffleDeck()
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
        public void dealCards(int rounds)
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
            trumpCard = deck.Pop();
        }
        public void dealerBid(int playerBid, int rounds) // Player enter bit and dealer 
        {
            if (playerBid == 0)
            {
                Dealer.Bid = rounds - 1;
            }
            else if (playerBid == rounds)
            {
                Dealer.Bid = 1;
            }
            else
            {
                Dealer.Bid = rounds - playerBid - 1;
            }
        }
        public void playersTurn(Card card)
        {
            Player.CardInPlay = card;
            removeCardFromList(Player.CardInPlay, Player.Hand);
        }
        public void dealersTurn()
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
            removeCardFromList(Dealer.CardInPlay, Dealer.Hand);
        }
        public bool handWinner()
        {
            bool playerWins;
            bool playerHasHighCard = Player.CardInPlay.Value > Dealer.CardInPlay.Value;
            bool bothSameSuit = Player.CardInPlay.Suit == Dealer.CardInPlay.Suit;
            bool playerHasTrump = Player.CardInPlay.Suit == trumpCard.Suit;
            bool dealerHasTrump = Dealer.CardInPlay.Suit == trumpCard.Suit;

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
            else if (playerHasTrump && !dealerHasTrump)
            {
                Player.Tricks += 1;
                Player.Points += 1;
                playerWins = true;
            }
            else if (dealerHasTrump && !playerHasTrump)
            {
                Dealer.Tricks += 1;
                Dealer.Points += 1;
                playerWins = false;
            }
            else
            {
                Dealer.Tricks += 1;
                Dealer.Points += 1;
                playerWins = false;
            }
            return playerWins;
        }
        public void addBonusPoints()
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
        public void removeCardFromList(Card usedCard, List<Card> listOfCards)
        {
            var match = listOfCards.Find(card =>
                card.Value == usedCard.Value && card.Suit == usedCard.Suit);

            if (match != null)
                listOfCards.Remove(match);
        }
    }
}
