using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}