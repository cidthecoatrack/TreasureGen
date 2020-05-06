﻿using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalItemGeneratorMundaneProxyTests
    {
        private MagicalItemGenerator mundaneProxy;
        private Mock<MagicalItemGenerator> mockInnerGenerator;
        private Item innerItem;

        [SetUp]
        public void Setup()
        {
            mockInnerGenerator = new Mock<MagicalItemGenerator>();
            mundaneProxy = new MagicalItemGeneratorMundaneProxy(mockInnerGenerator.Object);
            innerItem = new Item();

            mockInnerGenerator.Setup(g => g.GenerateRandom("power")).Returns(innerItem);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var item = mundaneProxy.GenerateRandom("power");
            Assert.That(item, Is.EqualTo(innerItem));
        }

        [Test]
        public void ThrowArgumentExceptionIfPowerIsMundane()
        {
            Assert.That(() => mundaneProxy.GenerateRandom(PowerConstants.Mundane), Throws.ArgumentException);
        }

        [Test]
        public void PassCustomItemThrough()
        {
            var template = new Item();
            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(innerItem);

            var item = mundaneProxy.Generate(template, true);
            Assert.That(item, Is.EqualTo(innerItem));
        }

        [Test]
        public void GenerateFromName()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name")).Returns(innerItem);

            var item = mundaneProxy.Generate("power", "item name");
            Assert.That(item, Is.EqualTo(innerItem));
        }

        [Test]
        public void ThrowArgumentExceptionIfPowerFromNameIsMundane()
        {
            Assert.That(() => mundaneProxy.Generate(PowerConstants.Mundane, "item name"), Throws.ArgumentException);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsItemOfPower_PassesThrough(bool innerIsOfPower)
        {
            mockInnerGenerator.Setup(g => g.IsItemOfPower("item name", "power")).Returns(innerIsOfPower);
            var isOfPower = mundaneProxy.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.EqualTo(innerIsOfPower));
        }

        [Test]
        public void IsItemOfPower_ReturnFalse_IfPowerIsMundane()
        {
            mockInnerGenerator.Setup(g => g.IsItemOfPower("item name", PowerConstants.Mundane)).Returns(true);
            var isOfPower = mundaneProxy.IsItemOfPower("item name", PowerConstants.Mundane);
            Assert.That(isOfPower, Is.False);
        }
    }
}