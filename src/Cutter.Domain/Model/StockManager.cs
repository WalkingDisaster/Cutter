using System;
using System.Collections.Generic;
using System.Linq;

namespace Cutter.Domain.Model
{
    public class StockManager
    {
        private readonly IEnumerable<StockItem> _items;
        private readonly long _shapeId;

        public StockManager(long shapeId, IEnumerable<StockItem> items)
        {
            _shapeId = shapeId;
            _items = items;
        }

        public IEnumerable<StockItem> Items
        {
            get { return _items; }
        }

        public long ShapeId
        {
            get { return _shapeId; }
        }

        public void DeductFromStock(int length, decimal cost, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Length == length && i.Cost == cost);
            if (item == null)
                throw new ArgumentException(string.Format("Cannot find a stock item with ID {0}, length {1}, and cost {2}", ShapeId, length, cost));
            if (item.Quantity < quantity)
                throw new ArgumentException(string.Format("The stock item with ID {0}, length {1}, and cost {2} does not have enough stock to satisfy demand.", ShapeId, length, cost));
            item.ReduceQuantityBy(quantity);
        }
    }
}