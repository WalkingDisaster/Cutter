using System;

namespace Cutter.Domain.Data
{
    public struct CutGroupResult
    {
        public Guid ShapeId { get; set; }
        public int Quantity { get; set; }
        public int StockLength { get; set; }
        public CutItemResult[] Items { get; set; }
    }
}