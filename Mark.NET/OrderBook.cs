using System.Text;

namespace Mark.NET
{
    public class OrderBook : Core.OrderBook
    {
        public int InstrumendId { get; }

        internal OrderBook(int instrumendId)
        {
            InstrumendId = instrumendId;
        }
        
        public bool Create(Order order)
        {
            return order.InstrumentId == InstrumendId && base.Create(order);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----------------------------");
            sb.AppendLine($"   Instrument[{InstrumendId}]");
            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
