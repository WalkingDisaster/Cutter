using System.Diagnostics;

namespace Cutter.Domain.Model
{
    [DebuggerDisplay("{Quantity} x {Length} units")]
    public class CutItem
    {
        public int Quantity { get; set; }
        public int Length { get; set; }

        public CutItem Clone()
        {
            return new CutItem { Quantity = Quantity, Length = Length };
        }
    }
}