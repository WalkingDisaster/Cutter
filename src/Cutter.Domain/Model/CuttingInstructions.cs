using System;
using System.Collections.Generic;
using System.Linq;

namespace Cutter.Domain.Model
{
    public class CuttingInstructions
    {
        private readonly IEnumerable<RequiredItem> _availableItems;
        private readonly decimal _cost;
        private readonly int _kerf;
        private readonly List<CutItem> _items;
        private readonly long _shapeId;
        private readonly int _stockLength;

        private int _quantity;

        public CuttingInstructions(long shapeId, int stockLength, decimal cost, int kerf, IEnumerable<RequiredItem> availableItems)
        {
            _shapeId = shapeId;
            _stockLength = stockLength;
            _cost = cost;
            _kerf = kerf;
            _availableItems = availableItems;
            _items = new List<CutItem>();
        }

        public long ShapeId
        {
            get { return _shapeId; }
        }

        public int StockLength
        {
            get { return _stockLength; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public decimal Cost
        {
            get { return _cost; }
        }

        public decimal Waste
        {
            get { return Drop * _cost; }
        }

        public int Drop
        {
            get
            {
                var amountCutWithKerf = Items.Sum(i => i.Quantity * i.Length) + (_kerf * (_items.Sum(i => i.Quantity) - 1));
                return StockLength - amountCutWithKerf;
            }
        }

        public decimal TotalWaste
        {
            get { return Waste * Quantity; }
        }

        public decimal TotalDrop
        {
            get { return Drop * Quantity; }
        }

        public IEnumerable<CutItem> Items
        {
            get { return _items; }
        }

        public void Optimize()
        {
            var optimized = Optimize(new List<CutItem>(), int.MaxValue);
            if (optimized == null)
                return;
            _items.Clear();
            _items.AddRange(optimized);
            TryToRepeat();
        }

        private IEnumerable<CutItem> Optimize(IEnumerable<CutItem> cutItems, int maxLength)
        {
            var best = cutItems;
            foreach (var item in GetRemainingItems(cutItems, maxLength))
            {
                if (cutItems.Sum(i => i.Quantity * (i.Length + _kerf)) + item.Length > StockLength)
                    continue;
                var items = new List<CutItem>(cutItems.Select(i => i.Clone()));
                var existing = items.SingleOrDefault(i => i.Length == item.Length);
                if (existing == null)
                    items.Add(new CutItem { Quantity = 1, Length = item.Length });
                else
                    existing.Quantity++;
                var results = Optimize(items, item.Length);
                if (best == null)
                {
                    best = results;
                    continue;
                }
                var drop = StockLength - results.Sum(i => i.Quantity * i.Length) - (_kerf * (results.Count() - 1));
                var bestDrop = StockLength - best.Sum(i => i.Quantity * i.Length) - (_kerf * best.Count() - 1);
                if (drop >= 0 && drop < bestDrop)
                    best = results;
            }
            return best;
        }

        private void TryToRepeat()
        {
            var leastNumberOfRepeat = int.MaxValue;
            foreach (var item in Items)
            {
                var count = _availableItems.Where(i => i.Length == item.Length).Sum(i => i.Quantity) / item.Quantity;
                if (count <= 1)
                {
                    _quantity = 1;
                    return;
                }
                if (count < leastNumberOfRepeat)
                    leastNumberOfRepeat = count;
            }
            if (leastNumberOfRepeat < int.MaxValue)
                _quantity = leastNumberOfRepeat;
        }

        public IEnumerable<RequiredItem> GetRemainingItems(IEnumerable<CutItem> cutItems, int maxLength)
        {
            foreach (var item in _availableItems.Where(i => i.Length <= maxLength))
            {
                var usedItem = cutItems.SingleOrDefault(i => i.Length == item.Length);
                if (usedItem == null)
                {
                    yield return new RequiredItem(item.Quantity, item.Length);
                    continue;
                }
                if (item.Quantity <= usedItem.Quantity)
                    continue;
                yield return new RequiredItem(item.Quantity - usedItem.Quantity, item.Length);
            }
        }
    }
}