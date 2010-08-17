using System.Diagnostics;

namespace Cutter.Domain.Data
{
    [DebuggerDisplay("{Quantity} x {Length} units")]
    public class CutItemResult
    {
        public int Quantity { get; set; }
        public int Length { get; set; }
    }
}