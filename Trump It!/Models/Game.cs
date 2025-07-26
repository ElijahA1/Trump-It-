using SkiaSharp.Extended.UI.Controls;

namespace Class_Practice
{
    public class Game
    {
        public Card? trumpCard { get; set; }

        Player player = new Player();
        Dealer dealer = new Dealer();

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
            player.Hand = new List<Card>();
            dealer.Hand = new List<Card>();
            player.Tricks = 0;
            dealer.Tricks = 0;

            // Add cards to hand lists, pop cards from deck stack
            for (int i = 0; i < rounds; i++)
            {
                player.Hand.Add(deck.Pop());
                dealer.Hand.Add(deck.Pop());
            }
            trumpCard = deck.Pop();
        }

        // Return dealer and player lists to Game Content page
        public List<Card> getPlayerCards() 
        { return player.Hand; }
        public List<Card> getDealerCards()
        { return dealer.Hand; }
        public Card getTrumpCard() 
        { return trumpCard; }

        public void countBids(int round) // Player enter bit and dealer 
        {
            Console.Write($"\nEnter a bid \"0\" through \"{round}\": ");
            int bid = Convert.ToInt32(Console.ReadLine());


            player.Bid = bid;
            if (bid == 0)
            {
                dealer.Bid = round - 1;
            }
            else if (bid == round)
            {
                dealer.Bid = 1;
            }
            else
            {
                dealer.Bid = round - bid - 1;
            }
        }
        public void playersTurn()
        {
            player.CardInPlay = new Card(0, "Null");

            Console.Write("\nEnter your card value: ");
            player.CardInPlay.Value = Convert.ToInt32(Console.ReadLine());
            Console.Write("\nEnter your card suit: ");
            player.CardInPlay.Suit = Console.ReadLine();

            Console.WriteLine($"\nYou played the {player.CardInPlay.Value} of {player.CardInPlay.Suit}");

            removeCardFromList(player.CardInPlay, player.Hand);
        }
        public void dealersTurn()
        {
            List<Card> sameSuitCards = new List<Card>();
            // Add same suit cards to a new list
            foreach (Card card in dealer.Hand)
            {
                if (card.Suit == player.CardInPlay.Suit)
                {
                    sameSuitCards.Add(card);
                }
            }

            if (sameSuitCards.Count > 0)
            {
                // Select the first same suit card from list
                dealer.CardInPlay = sameSuitCards[0];
            }
            else
            {
                dealer.CardInPlay = dealer.Hand[0];
            }
            // Remove cardInPlay from dealers hand
            removeCardFromList(dealer.CardInPlay, dealer.Hand);

            Console.WriteLine($"\nThe dealer played {dealer.CardInPlay.Value} of {dealer.CardInPlay.Suit}");
        }
        public void handWinner()
        {
            bool bothSameValue = player.CardInPlay.Value == dealer.CardInPlay.Value;
            bool bothSameSuit = player.CardInPlay.Suit == dealer.CardInPlay.Suit;
            bool playerHasTrump = player.CardInPlay.Suit == trumpCard.Suit;
            bool dealerHasTrump = dealer.CardInPlay.Suit == trumpCard.Suit;

            if (bothSameSuit)
            {
                if (bothSameValue)
                {
                    player.Tricks += 1;
                    player.Points += 1;
                    Console.WriteLine($"\nPlayer won this hand. Player has {player.Tricks} tricks.");
                }
                else
                {
                    dealer.Tricks += 1;
                    dealer.Points += 1;
                }
                Console.WriteLine($"\nDealer won this hand. Dealer has {dealer.Tricks} tricks.");
            }
            else if (playerHasTrump && !dealerHasTrump)
            {
                player.Tricks += 1;
                player.Points += 1;
                Console.WriteLine($"\nPlayer won this hand. Player has {player.Tricks} tricks.");
            }
            else if (dealerHasTrump && !playerHasTrump)
            {
                dealer.Tricks += 1;
                dealer.Points += 1;
                Console.WriteLine($"\nDealer won this hand. Dealer has {dealer.Tricks} tricks.");
            }
            else
            {
                dealer.Tricks += 1;
                dealer.Points += 1;
                Console.WriteLine($"\nDealer won this hand. Dealer has {dealer.Tricks} tricks.");
            }
        }
        public void addBonusPoints()
        {
            if (player.Bid == player.Tricks)
            {
                player.Points += 5;
                Console.WriteLine("\nPlayer met the bid");
            }
            if (dealer.Bid == dealer.Tricks)
            {
                dealer.Points += 5;
                Console.WriteLine("\nDealer met the bid");
            }
            Console.WriteLine($"\nPlayer has {player.Points} points");
            Console.WriteLine($"\nDealer has {dealer.Points} points");
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
