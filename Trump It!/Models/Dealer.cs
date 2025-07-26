namespace Class_Practice
{
    public class Dealer
    {
        public List<Card>? Hand { get; set; }
        public int Bid { get; set; }
        public Card? CardInPlay { get; set; }
        public int Tricks { get; set; }
        public int Points { get; set; }
    }
}
