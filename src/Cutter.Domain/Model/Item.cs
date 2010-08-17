using System;
using System.Diagnostics;

namespace Cutter.Domain.Model
{
    [DebuggerDisplay("{Quantity} x {Length} units")]
    public abstract class Item
    {
        private readonly int _length;
        private int _quantity;

        protected Item(int quantity, int length)
        {
            _length = length;
            _quantity = quantity;
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public int Length
        {
            get { return _length; }
        }

        public void ReduceQuantityBy(int quantity)
        {
            if (Quantity < quantity)
                throw new ArgumentException("You cannot deduct more quantity than the item's quantity", "quantity");
            _quantity -= quantity;
        }
    }
}