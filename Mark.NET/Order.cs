using Mark.Common;

namespace Mark.NET
{
    public class Order : IOrder
    {
        public Core.Order CoreOrder { get; }

        public Order(int instrumentId, int qty, decimal price, Side side)
        {
            CoreOrder = new Core.Order(qty, price, side);
            InstrumentId = instrumentId;
        }

        public int InstrumentId { get; }
        public string OrderId => CoreOrder.OrderId;

        public int Qty => CoreOrder.Qty;

        public int RemainingQty => CoreOrder.RemainingQty;

        public decimal Price => CoreOrder.Price;

        public Side Side => CoreOrder.Side;
        public int Priority => CoreOrder.Priority;
    }
}
