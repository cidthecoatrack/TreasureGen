﻿using System;
using D20Dice;
using EquipmentGen.Core.Generation.Factories;
using EquipmentGen.Core.Generation.Factories.Interfaces;
using EquipmentGen.Core.Generation.Providers.Interfaces;
using EquipmentGen.Core.Generation.Providers.Objects;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generation.Factories
{
    [TestFixture]
    public class CoinFactoryTests
    {
        private Mock<ITypeAndAmountPercentileResultProvider> mockTypeAndAmountPercentileResultProvider;
        private Mock<IDice> mockDice;
        private ICoinFactory factory;

        private TypeAndAmountPercentileResult result;

        [SetUp]
        public void Setup()
        {
            result = new TypeAndAmountPercentileResult();
            result.Type = "coin type";

            mockTypeAndAmountPercentileResultProvider = new Mock<ITypeAndAmountPercentileResultProvider>();
            mockTypeAndAmountPercentileResultProvider.Setup(p => p.GetTypeAndAmountPercentileResult(It.IsAny<String>()))
                .Returns(result);

            mockDice = new Mock<IDice>();
            factory = new CoinFactory(mockTypeAndAmountPercentileResultProvider.Object, mockDice.Object);
        }

        [Test]
        public void CoinFactoryReturnsCoinFromCoinPercentileResultProvider()
        {
            factory.CreateAtLevel(1);
            mockTypeAndAmountPercentileResultProvider.Verify(p => p.GetTypeAndAmountPercentileResult("Level1Coins"), Times.Once);
        }

        [Test]
        public void CoinIsEmptyIfPercentileResultIsEmpty()
        {
            result.Type = String.Empty;

            var coin = factory.CreateAtLevel(1);
            Assert.That(coin.Currency, Is.EqualTo(String.Empty));
            Assert.That(coin.Quantity, Is.EqualTo(0));
        }

        [Test]
        public void ParsesCurrencyOutOfPercentileResult()
        {
            var coin = factory.CreateAtLevel(1);
            Assert.That(coin.Currency, Is.EqualTo(result.Type));
        }

        [Test]
        public void ParsesRollOutOfPercentileResults()
        {
            result.RollToDetermineAmount = "1d2*100";
            mockDice.Setup(d => d.Roll(result.RollToDetermineAmount)).Returns(9266);

            var coin = factory.CreateAtLevel(1);
            Assert.That(coin.Quantity, Is.EqualTo(9266));
        }
    }
}