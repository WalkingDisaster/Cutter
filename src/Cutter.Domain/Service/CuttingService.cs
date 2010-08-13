using System;
using System.Collections.Generic;
using System.Linq;
using Cutter.Domain.Data;
using Cutter.Domain.Model;

namespace Cutter.Domain.Service
{
    public class CuttingService : ICuttingService
    {
        public CutResults Optimize(CutParameters parameters)
        {
            var stockItems = GetStockShapes(parameters.StockItems);
            var requiredItems = GetRequiredItems(parameters.RequiredItems);

            var results = new List<CuttingInstructions>();
            foreach (var item in requiredItems)
            {
                var stock = stockItems[item.ShapeId];
                var instructions = item.OptimizeFromStock(stock);
                results.AddRange(instructions);
            }
            throw new NotImplementedException();
        }

        private static Dictionary<Guid, StockManager> GetStockShapes(IEnumerable<StockItemParameter> availableStock)
        {
            var shapes = GetItemsByShape(availableStock, i => i.ShapeId);
            return shapes.ToDictionary(p => p.Key,
                                       e => new StockManager(e.Key,
                                                             from i in e.Value
                                                             orderby i.Length descending
                                                             select new StockItem(i.Quantity, i.Length, i.CostPerUnit)));
        }

        private static IEnumerable<RequiredItemManager> GetRequiredItems(IEnumerable<RequiredItemParameter> items)
        {
            return from shape in GetItemsByShape(items, i => i.ShapeId)
                   select new RequiredItemManager(shape.Key, from i in shape.Value
                                                             orderby i.Length descending
                                                             select new RequiredItem(i.Quantity, i.Length));
        }

        private static IDictionary<Guid, IEnumerable<T>> GetItemsByShape<T>(IEnumerable<T> items, Func<T, Guid> key)
        {
            var shapes = new Dictionary<Guid, List<T>>();
            foreach (var item in items)
            {
                if (!shapes.ContainsKey(key(item)))
                    shapes.Add(key(item), new List<T>());
                shapes[key(item)].Add(item);
            }
            return shapes.ToDictionary(k => k.Key, e => e.Value.AsEnumerable());
        }
    }
}