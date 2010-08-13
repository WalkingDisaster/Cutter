using System;

namespace Cutter.Domain.Model
{
    public class RequiredItem : Item
    {
        public RequiredItem(int quantity, int length) : base(quantity, length)
        {
        }

        public RequiredItem Clone()
        {
            return new RequiredItem(Quantity, Length);
        }
    }
}