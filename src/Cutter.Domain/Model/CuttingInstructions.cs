using System;
using System.Collections.Generic;
using System.Linq;

namespace Cutter.Domain.Model
{
    public class CuttingInstructions
    {
        private readonly IEnumerable<RequiredItem> _availableItems;
        private readonly decimal _cost;
        private readonly List<CutItem> _items;
        private readonly int _stockLength;

        private int _quantity;

        public CuttingInstructions(int stockLength, decimal cost, IEnumerable<RequiredItem> availableItems)
        {
            _stockLength = stockLength;
            _cost = cost;
            _availableItems = availableItems;
            _items = new List<CutItem>();
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
            get { return Drop*_cost; }
        }

        public int Drop
        {
            get { return StockLength - Items.Sum(i => i.Quantity*i.Length); }
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
        }

        private IEnumerable<CutItem> Optimize(IEnumerable<CutItem> cutItems, int maxLength)
        {
            var best = cutItems;
            foreach (var item in GetRemainingItems(cutItems, maxLength))
            {
                if (cutItems.Sum(i => i.Quantity*i.Length) + item.Length > StockLength)
                    continue;
                var items = new List<CutItem>(cutItems.Select(i => i.Clone()));
                var existing = items.SingleOrDefault(i => i.Length == item.Length);
                if (existing == null)
                    items.Add(new CutItem {Quantity = 1, Length = item.Length});
                else
                    existing.Quantity++;
                var results = Optimize(items, item.Length);
                if (best == null)
                {
                    best = results;
                    continue;
                }
                var drop = StockLength - results.Sum(i => i.Quantity * i.Length);
                var bestDrop = StockLength - best.Sum(i => i.Quantity*i.Length);
                if (drop >= 0 && drop < bestDrop)
                    best = results;
            }
            TryToRepeat(best);
            return best;
        }

        private void TryToRepeat(IEnumerable<CutItem> cuts)
        {
            var leastNumberOfRepeat = 0;

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