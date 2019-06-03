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

        protected IEnumerable<Order> BidOrders => _bids.SelectMany(p => p.Value);
        protected IEnumerable<Order> AskOrders => _asks.SelectMany(p => p.Value);

        protected IEnumerable<Limit> BidLimits
        {
            get
            {
                foreach (var pair in _bids.Reverse().Take(10))
                {
                    var bidOrders = pair.Value;
                    if (bidOrders != null && bidOrders.Count != 0)
                    {
                        var firstOrder = bidOrders.First();
                        yield return new Limit(bidOrders.Sum(p => p.RemainingQty), firstOrder.Price, firstOrder.Side, bidOrders.Count);
                    }
                }
            }
        }

        protected IEnumerable<Limit> AskLimits
        {
            get
            {
                foreach (var pair in _asks.Take(10))
                {
                    var askOrders = pair.Value;
                    if (askOrders != null && askOrders.Count != 0)
                    {
                        var firstOrder = askOrders.First();
                        yield return new Limit(askOrders.Sum(p => p.RemainingQty), firstOrder.Price, firstOrder.Side, askOrders.Count);
                    }
                }
            }
        }


        protected OrderBook()
        {
            _bids = new SortedDictionary<decimal, List<Order>>();
            _asks = new SortedDictionary<decimal, List<Order>>();

            _orderByIdList = new Dictionary<string, Order>();
        }

        protected bool Create(Order order)
        {
            CheckMatch(ref order);
            if (order.Filled) return true;

            if (_orderByIdList.ContainsKey(order.OrderId))
            {
                return false;
            }

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
            
        }

        private void CheckMatch(ref Order other)
        {
            if (other.Side == Side.Ask)
            {
                foreach (var pair in _bids.Reverse())
                {
                    if (pair.Key >= other.Price)
                    {
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
            else if (other.Side == Side.Bid)
            {
                foreach (var pair in _asks)
                {
                    if (pair.Key <= other.Price)
                    {
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
        }

        protected bool Update(string orderId, int? quantity = null, decimal? price = null)
        {
            if (quantity == null && price == null) return false;
            
            if (quantity != null && quantity <= 0) return false;

            if (!_orderByIdList.TryGetValue(orderId, out var order)) return false;
            
            // Price different ?
            var priceChanged = price != null && order.Price != price;
            var quantityChanged = quantity != null && order.Qty != quantity;
                
            if (priceChanged)
            {
                if (order.Side == Side.Ask)
                {
                    RemoveOrderFromSide(order, _asks);
                    order.UpdatePrice(price.Value);
                    InsertOrderOnSide(order, _asks);
                }
                else if (order.Side == Side.Bid)
                {
                    RemoveOrderFromSide(order, _bids);
                    order.UpdatePrice(price.Value);
                    InsertOrderOnSide(order, _bids);
                }
                
                return true;
            }

            if (quantityChanged)
            {
                order.UpdateQty(quantity.Value);
                return true;
            }

            return false;
        }

        protected bool Cancel(Order order)
        {
            if (_orderByIdList.Remove(order.OrderId))
            {
                if (order.Side == Side.Bid)
                    return RemoveOrderFromSide(order, _bids);
                if (order.Side == Side.Ask)
                    return RemoveOrderFromSide(order, _asks);
            }
            return false;
        }

        private static bool RemoveOrderFromSide(Order order, IReadOnlyDictionary<decimal, List<Order>> side)
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
