using System.Text;

namespace Mark.NET
{
    public class OrderBook : Core.OrderBook
    {
        public int InstrumentId { get; }

        internal OrderBook(int instrumentId)
        {
            InstrumentId = instrumentId;
        }

        public bool Create(Order order)
        {
            return order.InstrumentId == InstrumentId && base.Create(order);
        }

        public bool Modify(Order order, int? quantity = null, decimal? price = null)
        {
            return order.InstrumentId == InstrumentId && Update(order.OrderId, quantity, price);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----------------------------");
            sb.AppendLine($"   Instrument[{InstrumentId}]");
            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}