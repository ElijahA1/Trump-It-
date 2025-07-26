namespace Class_Practice
{
    public class Card
    {
        public int Value { get; set; }
        public string Suit { get; set; }
        public string ImagePath { get; set; }

        public Card(int value, string suit)
        {
            Value = value;
            Suit = suit;

            string valueName = valueNames[value];
            string suitName = suit;
            ImagePath = $"{valueName}_{suitName}.png"; // Or include folder if needed
        }

        private static readonly Dictionary<int, string> valueNames = new()
        {
            { 1, "two" },
            { 2, "three" },
            { 3, "four" },
            { 4, "five" },
            { 5, "six" },
            { 6, "seven" },
            { 7, "eight" },
            { 8, "nine" },
            { 9, "ten" },
            { 10, "jack" },
            { 11, "queen" },
            { 12, "king" },
            { 13, "ace" }
        };
    }
}