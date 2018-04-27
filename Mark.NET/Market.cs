using System.Collections.Generic;

namespace Mark.NET
{
    public class Market
    {
        public Dictionary<int, OrderBook> OrderBooks { get; }

        public Market()
        {
            OrderBooks = new Dictionary<int, OrderBook>();
        }

        public bool AddInstrument(int instrId)
        {
            if (!OrderBooks.ContainsKey(instrId))
            {
                OrderBooks.Add(instrId, new OrderBook(instrId));
                return true;
            }

            return false;
        }

        public OrderBook GetOrderBook(int instrId)
        {
            return OrderBooks.TryGetValue(instrId, out var orderBook) ? orderBook : null;
        }
    }
}
