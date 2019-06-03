using Mark.NET;
using Mark.Common;
using System;
using System.Collections.Generic;

namespace Mark.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //market.CreateOrderBook(instrId);
            const int instrId = 10;
            var market = new Market(new List<int>(instrId));
            var ob = market.GetOrderBook(instrId);
            //ob.Create(new Order(10, 10.0m, Side.Bid));
            //ob.Create(new Order(10, 11.0m, Side.Bid));
            //ob.Create(new Order(10, 11.0m, Side.Bid));
            //ob.Create(new Order(10, 12.0m, Side.Bid));
            //ob.Create(new Order(10, 13.0m, Side.Bid));
            //ob.Create(new Order(10, 14.0m, Side.Bid));
            var order = new Order(instrId, 10, 15.0m, Side.Bid);
            ob.Create(order);
            //ob.Create(new Order(10, 10.0m, Side.Ask));
            //ob.Create(new Order(10, 11.0m, Side.Ask));
            //ob.Create(new Order(10, 11.0m, Side.Ask));
            //ob.Create(new Order(10, 12.0m, Side.Ask));
            //ob.Create(new Order(10, 13.0m, Side.Ask));
            //ob.Create(new Order(10, 14.0m, Side.Ask));
            ob.Create(new Order(instrId, 10, 15.5m, Side.Ask));
            Console.WriteLine(ob);
            Console.ReadKey();

            ob.Create(new Order(instrId, 10, 12.0m, Side.Bid));
            Console.WriteLine(ob);
            Console.ReadKey();

            ob.Create(new Order(instrId, 2, 15.5m, Side.Bid));
            Console.WriteLine(ob);
            Console.ReadKey();

            ob.Modify(order, 20);
            Console.WriteLine(ob);
            Console.ReadKey();
            
            ob.Modify(order, null, 11);
            Console.WriteLine(ob);
            Console.ReadKey();
        }
    }
}
