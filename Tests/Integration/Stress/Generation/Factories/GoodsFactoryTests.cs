﻿using EquipmentGen.Core.Generation.Factories.Interfaces;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Generation.Factories
{
    [TestFixture]
    public class GoodsFactoryTests : StressTest
    {
        [Inject]
        public IGoodsFactory GoodsFactory { get; set; }

        [SetUp]
        public void Setup()
        {
            StartTest();
        }

        [TearDown]
        public void TearDown()
        {
            StopTest();
        }

        [Test]
        public void GoodsFactoryReturnsGoods()
        {
            while (TestShouldKeepRunning())
            {
                var level = GetNewLevel();
                var goods = GoodsFactory.CreateAtLevel(level);

                Assert.That(goods, Is.Not.Null);

                foreach (var good in goods)
                {
                    Assert.That(good.Description, Is.Not.Empty);
                    Assert.That(good.ValueInGold, Is.GreaterThanOrEqualTo(0));
                }
            }

            AssertIterations();
        }
    }
}