namespace Cutter.Domain.Model
{
    public class StockItem : Item
    {
        private readonly decimal _cost;

        public StockItem(int quantity, int length, decimal cost)
            : base(quantity, length)
        {
            _cost = cost;
        }

        public decimal Cost
        {
            get { return _cost; }
        }
    }
}