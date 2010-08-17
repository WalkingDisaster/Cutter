namespace Cutter.Domain.Data
{
    public struct StockItemParameter
    {
        public long ShapeId { get; set; }
        public decimal CostPerUnit { get; set; }
        public int Quantity { get; set; }
        public int Length { get; set; }
        public int Kerf { get; set; }
    }
}