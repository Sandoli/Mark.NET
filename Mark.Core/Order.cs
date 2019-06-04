using System.Collections.Generic;
using Mark.Common;

namespace Mark.Core
{
    public class Order
    {
        private readonly List<Exec> _execs;

        public int Qty { get; private set; }
        public int RemainingQty { get; private set; }
        public decimal Price { get; private set; }
        public Side Side { get; }
        public string OrderId { get; private set; }
        public IEnumerable<Exec> Execs => _execs;

        #region Flags
        public bool Filled => RemainingQty == 0;
        public int Priority { get; internal set; }

        #endregion
        
        public Order(int qty, decimal price, Side side)
        {
            Qty = qty;
            RemainingQty = qty;
            Price = price;
            Side = side;
            OrderId = string.Empty;
            _execs = new List<Exec>();
        }

        internal void CreateOrderId(string orderId = null)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                orderId = IdGenerator.CreateNewOrderId();
            }
            OrderId = orderId;
        }

        public void UpdateQty(int qty)
        {
            if (Filled) return;
            var deltaQty = qty - Qty;
            Qty = qty;
            RemainingQty += deltaQty;
        }

        public bool UpdatePrice(decimal price)
        {
            bool canUpdate = !Filled && RemainingQty == Qty;
            if (canUpdate)
            {
                Price = price;
            }
            return canUpdate;
        }

        internal void AddExec(Exec exec)
        {
            _execs.Add(exec);
            RemainingQty -= exec.Qty;
        }

        public override string ToString()
        {
            return Side == Side.Bid ? $"{RemainingQty,3} {Price,6:0.00}" : $"{Price,-6:0.00} {RemainingQty,-3}";
        }
    }
}
