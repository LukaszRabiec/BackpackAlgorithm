namespace BackpackAlgorithm
{
    public class Item
    {
        public int Value { get; set; }

        public int Weight { get; set; }

        public double Rate => (double)Value / Weight;

        public Item(int value, int weight)
        {
            Value = value;
            Weight = weight;
        }
    }
}
