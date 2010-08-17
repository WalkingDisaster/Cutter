using System;
using System.Collections.Generic;
using System.Linq;

namespace Cutter.Domain.Model
{
    public class RequiredItemManager
    {
        private readonly List<RequiredItem> _items;
        private readonly long _shapeId;

        public RequiredItemManager(long shapeId, IEnumerable<RequiredItem> items)
        {
            _shapeId = shapeId;
            _items = new List<RequiredItem>(items);
        }

        public IEnumerable<RequiredItem> Items
        {
            get { return _items; }
        }

        public long ShapeId
        {
            get { return _shapeId; }
        }

        private bool HasRemainingPieces
        {
            get
            {
                return Items.Any(i => i.Quantity > 0);
            }
        }

        public IEnumerable<CuttingInstructions> OptimizeFromStock(StockManager stock)
        {
            var results = new List<CuttingInstructions>();
            CuttingInstructions instructions;
            do
            {
                instructions = GetBestInstructionsForRemainingPieces(stock);
                if (instructions != null)
                {
                    results.Add(instructions);
                }
            } while (instructions != null);
            return results;
        }

        private CuttingInstructions GetBestInstructionsForRemainingPieces(StockManager stock)
        {
            if (!HasRemainingPieces)
                return null;
            CuttingInstructions best = null;
            foreach (var stockItem in stock.Items)
            {
                var current = new CuttingInstructions(stock.ShapeId,
                                                      stockItem.Length,
                                                      stockItem.Cost,
                                                      stockItem.Kerf,
                                                      Items.Where(i => i.Quantity > 0)
                                                          .Select(i => i.Clone()));
                current.Optimize();
                if (best == null)
                {
                    best = current;
                }
                else
                {
                    if (current.Waste < best.Waste)
                    {
                        best = current;
                    }
                    else if (current.Waste == best.Waste)
                    {
                        var currentNumberOfCuts = current.Items.Sum(i => i.Quantity);
                        var bestNumberOfCuts = best.Items.Sum(i => i.Quantity);
                        if (currentNumberOfCuts < bestNumberOfCuts)
                            best = current;
                        else if (currentNumberOfCuts == bestNumberOfCuts && current.StockLength > best.StockLength)
                            best = current;
                    }
                }
            }
            if (best != null)
                DeductItems(stock, best);
            return best;
        }

        private void DeductItems(StockManager stock, CuttingInstructions cuttingInstructions)
        {
            stock.DeductFromStock(cuttingInstructions.StockLength,
                                  cuttingInstructions.Cost,
                                  cuttingInstructions.Quantity);
            foreach (var cutItem in cuttingInstructions.Items)
            {
                var item = Items.SingleOrDefault(i => i.Length == cutItem.Length);
                if (item == null)
                    throw new InvalidOperationException("No item of that length " + cutItem.Length + " exists.");
                item.ReduceQuantityBy(cutItem.Quantity * cuttingInstructions.Quantity);
            }
        }
    }
}