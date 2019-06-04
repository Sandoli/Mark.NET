using System.Collections.Generic;

namespace Mark.Common
{
    public interface IOrder
    {
        int InstrumentId { get; }
        string OrderId { get; }
        int Qty { get; }
        int RemainingQty { get; }
        decimal Price { get; }
        Side Side { get; }
        int Priority { get;  }
        //IEnumerable<IExec> Execs { get; }
    }
}