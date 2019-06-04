using System;
using System.Collections.Generic;
using System.Linq;
using Mark.Common;
using Xunit;

namespace Mark.NET.Test
{
    public class UnitTest1
    {
        [Fact]
        public void CreateMarketWithSingleInstrument()
        {
            // Arrange
            var market = new Market(new List<int>{ 10 });
            
            // Act
            
            
            // Assert
            Assert.Single(market.OrderBooks);
            Assert.Equal(10, market.OrderBooks.FirstOrDefault().Key);
        }

        [Fact]
        public void CreateMarketWithMultipleInstruments()
        {
            // Arrange
            var market = new Market(new List<int>{ 10, 11 });
            
            // Act
            
            
            // Assert
            Assert.Equal(2, market.OrderBooks.Count);
            Assert.Equal(10, market.OrderBooks.First().Key);
            Assert.Equal(11, market.OrderBooks.Last().Key);
        }

        [Fact]
        public void CreateSingleOrder()
        {
            // Arrange
            var market = new Market(new List<int>{ 10, 11 });
            
            // Act
            var order = new Order(10, 10, 10, Side.Ask);            
            OrderBook ob = market.GetOrderBook(10);
            ob.Create(order);
            
            // Assert
            Assert.Empty(ob.BidOrders);
            Assert.Single(ob.AskOrders);
        }
        
        [Fact]
        public void CreateTwoOppositeOrdersNoMatch()
        {
            // Arrange
            var market = new Market(new List<int>{ 10, 11 });
            
            // Act
            var orderAsk = new Order(10, 10, 10, Side.Ask);            
            var orderBid = new Order(10, 10,  9, Side.Bid);            
            OrderBook ob = market.GetOrderBook(10);
            ob.Create(orderAsk);
            ob.Create(orderBid);
            
            // Assert
            Assert.Single(ob.BidOrders);
            Assert.Single(ob.AskOrders);
        }

        [Fact]
        public void CreateTwoMatchingOrders()
        {
            // Arrange
            var market = new Market(new List<int>{ 10, 11 });
            
            // Act
            var orderAsk = new Order(10, 10, 10, Side.Ask);            
            var orderBid = new Order(10, 10, 10, Side.Bid);            
            OrderBook ob = market.GetOrderBook(10);
            ob.Create(orderAsk);
            ob.Create(orderBid);
            
            // Assert
            Assert.Empty(ob.BidOrders);
            Assert.Empty(ob.AskOrders);
        }
    }
}