namespace Cutter.Domain.Data
{
    public struct CutGroupResult
    {
        public long ShapeId { get; set; }
        public int Quantity { get; set; }
        public int StockLength { get; set; }
        public CutItemResult[] Items { get; set; }
    }
}