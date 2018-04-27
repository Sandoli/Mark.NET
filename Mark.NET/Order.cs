using Mark.Common;

namespace Mark.NET
{
    public class Order : Core.Order, IOrder
    {
        #region Constructors
        public Order(int instrumentId, int qty, decimal price, Side side) : base(qty, price, side)
        {
            InstrumentId = instrumentId;
        }
        #endregion

        public int InstrumentId { get; private set; }
    }
}
