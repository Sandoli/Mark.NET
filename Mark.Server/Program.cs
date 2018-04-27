using Mark.NET;
using Mark.Common;
using System;

namespace Mark.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            const int instrId = 10;
            var market = new Market();
            market.AddInstrument(instrId);
            var ob = market.GetOrderBook(instrId);
            //ob.Create(new Order(10, 10.0m, Side.Bid));
            //ob.Create(new Order(10, 11.0m, Side.Bid));
            //ob.Create(new Order(10, 11.0m, Side.Bid));
            //ob.Create(new Order(10, 12.0m, Side.Bid));
            //ob.Create(new Order(10, 13.0m, Side.Bid));
            //ob.Create(new Order(10, 14.0m, Side.Bid));
            ob.Create(new Order(instrId, 10, 15.0m, Side.Bid));
            //ob.Create(new Order(10, 10.0m, Side.Ask));
            //ob.Create(new Order(10, 11.0m, Side.Ask));
            //ob.Create(new Order(10, 11.0m, Side.Ask));
            //ob.Create(new Order(10, 12.0m, Side.Ask));
            //ob.Create(new Order(10, 13.0m, Side.Ask));
            //ob.Create(new Order(10, 14.0m, Side.Ask));
            ob.Create(new Order(instrId, 10, 15.5m, Side.Ask));
            Console.WriteLine(ob);
            Console.ReadLine();

            ob.Create(new Order(instrId, 10, 12.0m, Side.Bid));
            Console.WriteLine(ob);
            Console.ReadLine();

            ob.Create(new Order(instrId, 2, 15.5m, Side.Bid));
            Console.WriteLine(ob);
            Console.ReadLine();
        }
    }
}
