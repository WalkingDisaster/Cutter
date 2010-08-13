namespace Cutter.Domain.Model
{
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