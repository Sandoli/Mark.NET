using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mark.Common;

namespace Mark.Core
{
    public class OrderBook //: IOrderBook
    {
        private readonly SortedDictionary<decimal, List<Order>> _bids;
        private readonly SortedDictionary<decimal, List<Order>> _asks;

        private readonly Dictionary<string, Order> _orderByIdList;

        public IEnumerable<Order> BidOrders => _bids.Reverse().SelectMany(p => p.Value);
        public IEnumerable<Order> AskOrders => _asks.SelectMany(p => p.Value);

        protected IEnumerable<Limit> BidLimits =>
            _bids.Reverse()
                .Take(10)
                .Select(pair => pair.Value)
                .Where(bidOrders => bidOrders != null && bidOrders.Any())
                .Select(bidOrders => (bidOrders, first : bidOrders.First()))
                .Select(t => new Limit(t.bidOrders.Sum(p => p.RemainingQty), t.first.Price,
                    t.first.Side, t.bidOrders.Count));

        protected IEnumerable<Limit> AskLimits =>
            _asks.Take(10)
                .Select(pair => pair.Value)
                .Where(askOrders => askOrders != null && askOrders.Any())
                .Select(askOrders => (askOrders, first : askOrders.First()))
                .Select(t => new Limit(t.askOrders.Sum(p => p.RemainingQty), t.first.Price,
                    t.first.Side, t.askOrders.Count));


        protected OrderBook()
        {
            _bids = new SortedDictionary<decimal, List<Order>>();
            _asks = new SortedDictionary<decimal, List<Order>>();

            _orderByIdList = new Dictionary<string, Order>();
        }

        protected bool Create(Order order)
        {
            if (!string.IsNullOrEmpty(order.OrderId) && _orderByIdList.ContainsKey(order.OrderId))
            {
                return false;
            }

            order.CreateOrderId();
            CheckMatch(order);
            if (order.Filled) return true;
            
            _orderByIdList.Add(order.OrderId, order);

            if (order.Side == Side.Bid)
            {
                InsertOrderOnSide(order, _bids);
            }
            else if (order.Side == Side.Ask)
            {
                InsertOrderOnSide(order, _asks);
            }

            return true;
        }

        private static void InsertOrderOnSide(Order order, IDictionary<decimal, List<Order>> side)
        {
            List<Order> orderList;
            if (!side.ContainsKey(order.Price))
            {
                orderList = new List<Order>();
                side.Add(order.Price, orderList);
            }
            else
            {
                orderList = side[order.Price];
            }

            orderList.Add(order);
            order.Priority = orderList.Count;

        }

        private void CheckMatch(Order other)
        {
            if (other.Side == Side.Ask)
            {
                foreach (var pair in _bids.Reverse())
                {
                    if (pair.Key < other.Price) continue;
                    var orderList = pair.Value;
                    for (int i = orderList.Count - 1; i >= 0; i--)
                    {
                        var order = orderList[i];
                        var execQty = Math.Min(order.Qty, other.Qty);
                        var exec = new Exec(execQty, other.Price);

                        other.AddExec(exec);
                        order.AddExec(exec);

                        if (order.Filled) orderList.RemoveAt(i);
                        if (other.Filled) return;
                    }

                }
            }
            else if (other.Side == Side.Bid)
            {
                foreach (var pair in _asks)
                {
                    if (pair.Key > other.Price) continue;
                    var orderList = pair.Value;
                    for (int i = orderList.Count - 1; i >= 0; i--)
                    {
                        var order = orderList[i];
                        var execQty = Math.Min(order.Qty, other.Qty);
                        var exec = new Exec(execQty, other.Price);

                        other.AddExec(exec);
                        order.AddExec(exec);

                        if (order.Filled) orderList.RemoveAt(i);
                        if (other.Filled) return;
                    }

                }
            }
        }

        protected bool Update(string orderId, int? quantity = null, decimal? price = null)
        {
            if (quantity == null && price == null) return false;
            
            if (quantity != null && quantity <= 0) return false;

            if (!_orderByIdList.TryGetValue(orderId, out var order)) return false;
            
            // Price different ?
            var priceChanged = price != null && order.Price != price;
            var quantityChanged = quantity != null && order.Qty != quantity;

            var side = GetSide(order);
                
            if (priceChanged)
            {
                RemoveOrderFromSide(order, side);
                order.UpdatePrice(price.Value);
                InsertOrderOnSide(order, side);
                
                return true;
            }

            if (quantityChanged)
            {
                UpdateOrderOnSide(order, quantity.Value, side);
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method and RemoveOrderFromSide and InsertOrderOnSide will be the ones that will build the feed updates
        /// </summary>
        /// <param name="order"></param>
        /// <param name="quantity"></param>
        /// <param name="side"></param>
        private static void UpdateOrderOnSide(Order order, int quantity, IDictionary<decimal, List<Order>> side)
        {
            // Recompute priority if quantity > order.Quantity
            order.UpdateQty(quantity);
        }

        protected bool Cancel(Order order)
        {
            if (_orderByIdList.Remove(order.OrderId))
            {
                return RemoveOrderFromSide(order, GetSide(order));
            }
            return false;
        }

        private IDictionary<decimal, List<Order>> GetSide(Order order)
        {
            switch (order.Side)
            {
                case Side.Bid: return _bids;
                case Side.Ask: return _asks;
                default: throw new IndexOutOfRangeException(nameof(order.Side));
            }
        }

        private static bool RemoveOrderFromSide(Order order, IDictionary<decimal, List<Order>> side)
        {
            if (!side.ContainsKey(order.Price))
            {
                return false;
            }

            var orderList = side[order.Price];
            for (int i = orderList.Count - 1; i >= 0; i--)
            {
                if (orderList[i].OrderId == order.OrderId)
                {
                    orderList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            using (var bidIt = BidLimits.GetEnumerator())
            {
                using (var askIt = AskLimits.GetEnumerator())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("-----------------------------");
                    sb.AppendLine("Bid       OrderBook       Ask");
                    sb.AppendLine("-----------------------------");
                    var hasCurrentBid = bidIt.MoveNext();
                    var hasCurrentAsk = askIt.MoveNext();
                    while (hasCurrentBid || hasCurrentAsk)
                    {
                        if (hasCurrentBid && hasCurrentAsk)
                        {
                            var currBid = bidIt.Current;
                            var currAsk = askIt.Current;
                            sb.AppendLine($"{currBid} | {currAsk}");
                            hasCurrentBid = bidIt.MoveNext();
                            hasCurrentAsk = askIt.MoveNext();
                        }
                        else if (hasCurrentBid)
                        {
                            var currBid = bidIt.Current;
                            sb.AppendLine($"{currBid} |");
                            hasCurrentBid = bidIt.MoveNext();
                        }
                        else
                        {
                            var currAsk = askIt.Current;
                            sb.AppendLine($"       |    {currAsk}");
                            hasCurrentAsk = askIt.MoveNext();
                        }
                        sb.AppendLine("-----------------------------");

                    }

                    return sb.ToString();
                }
            }
        }
    }
}
