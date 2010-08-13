namespace Cutter.Domain.Data
{
    public struct CutParameters
    {
        public StockItemParameter[] StockItems { get; set; }
        public RequiredItemParameter[] RequiredItems { get; set; }
    }
}