﻿using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Mark.Common;
using Mark.NET;

namespace Mark.Server
{
//    public class Server
//    {
//        public Market Market { get; }
//        public Server()
//        {
//            Market = new Market();
//        }
//        IOrder CreateNewOrder(int instrumentId, int qty, decimal price, Side side)
//        {
//            return new Order(instrumentId, qty, price, side);
//        }
//
//        IOrder CreateModifyOrder(string orderId, int qty, decimal price)
//        {
//            var order = Market.GetOrder(orderId);
//            if (order == null) throw new Exception("Order not found");
//            return new Order(order.InstrumentId, qty, price, order.Side);
//        }
//
//        IOrder CreateCancelOrder(string orderId)
//        {
//            var order = Market.GetOrder(orderId);
//            if (order == null) throw new Exception("Order not found");
//            return order;
//        }
//
//        void Send(IOrder order)
//        {
//            Market.GetOrderBook(order.InstrumentId);
//        }
//
//        public event OnOrderAckEvent  ();
//    }
}