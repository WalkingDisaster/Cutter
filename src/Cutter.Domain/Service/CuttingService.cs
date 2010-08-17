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
            return new CutResults
                       {
                           Groups = results.Select(r => new CutGroupResult
                                                            {
                                                                Items = r.Items
                                                                    .Select(i => new CutItemResult
                                                                                     {
                                                                                         Quantity = i.Quantity,
                                                                                         Length = i.Length
                                                                                     }).ToArray(),
                                                                Quantity = r.Quantity,
                                                                StockLength = r.StockLength,
                                                                ShapeId = r.ShapeId
                                                            }).ToArray(),
                           UncutItems = CalculateItemsNotCut(parameters.RequiredItems, results)
                       };
        }

        private static UncutItemResult[] CalculateItemsNotCut(IEnumerable<RequiredItemParameter> requiredItems, IEnumerable<CuttingInstructions> cutItems)
        {
            var allCutItems = cutItems
                .SelectMany(i => i.Items.Select(j => new RequiredItemParameter
                                                         {
                                                             ShapeId = i.ShapeId,
                                                             Quantity = -(j.Quantity * i.Quantity),
                                                             Length = j.Length
                                                         }));
            var unionedItems = requiredItems
                .Union(allCutItems)
                .GroupBy(i => new {i.ShapeId, i.Length})
                .Select(i => new UncutItemResult
                                 {
                                     ShapeId = i.Key.ShapeId,
                                     Length = i.Key.Length,
                                     Quantity = i.Sum(x => x.Quantity)
                                 });
            return unionedItems.Where(i => i.Quantity != 0).ToArray();
        }

        private static Dictionary<long, StockManager> GetStockShapes(IEnumerable<StockItemParameter> availableStock)
        {
            var shapes = GetItemsByShape(availableStock, i => i.ShapeId);
            return shapes.ToDictionary(p => p.Key,
                                       e => new StockManager(e.Key,
                                                             from i in e.Value
                                                             orderby i.Length descending
                                                             select new StockItem(i.Quantity, i.Length, i.CostPerUnit, i.Kerf)));
        }

        private static IEnumerable<RequiredItemManager> GetRequiredItems(IEnumerable<RequiredItemParameter> items)
        {
            return from shape in GetItemsByShape(items, i => i.ShapeId)
                   select new RequiredItemManager(shape.Key, from i in shape.Value
                                                             orderby i.Length descending
                                                             select new RequiredItem(i.Quantity, i.Length));
        }

        private static IDictionary<long, IEnumerable<T>> GetItemsByShape<T>(IEnumerable<T> items, Func<T, long> key)
        {
            var shapes = new Dictionary<long, List<T>>();
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