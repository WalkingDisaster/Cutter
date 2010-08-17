namespace Cutter.Domain.Model
{
    public class StockItem : Item
    {
        private readonly decimal _cost;
        private readonly int _kerf;

        public StockItem(int quantity, int length, decimal cost, int kerf)
            : base(quantity, length)
        {
            _cost = cost;
            _kerf = kerf;
        }

        public int Kerf
        {
            get { return _kerf; }
        }

        public decimal Cost
        {
            get { return _cost; }
        }
    }
}