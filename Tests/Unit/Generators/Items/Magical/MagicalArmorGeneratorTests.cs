﻿using System;
using System.Collections.Generic;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Generators.Items.Magical;
using EquipmentGen.Selectors.Interfaces;
using EquipmentGen.Selectors.Interfaces.Objects;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class MagicalArmorGeneratorTests
    {
        private IMagicalItemGenerator magicalArmorGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IAttributesSelector> mockAttributesSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<IMagicalItemTraitsGenerator> mockMagicItemTraitsGenerator;
        private Mock<ISpecificGearGenerator> mockSpecificGearGenerator;
        private TypeAndAmountPercentileResult result;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockAttributesSelector = new Mock<IAttributesSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockMagicItemTraitsGenerator = new Mock<IMagicalItemTraitsGenerator>();
            mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            magicalArmorGenerator = new MagicalArmorGenerator(mockTypeAndAmountPercentileSelector.Object, mockPercentileSelector.Object, mockAttributesSelector.Object, mockSpecialAbilitiesGenerator.Object, mockSpecificGearGenerator.Object);

            result = new TypeAndAmountPercentileResult();
            result.Type = "armor type";
            result.Amount = 9266;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom("powerArmors")).Returns(result);
        }

        [Test]
        public void GetArmor()
        {
            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GetBonusFromSelector()
        {
            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetNameFromPercentileSelector()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(result.Type + "Types")).Returns("armor name");

            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor.Name, Is.EqualTo("armor name"));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(result.Type + "Types")).Returns("armor name");

            var attributes = new[] { "type 1", "type 2" };
            mockAttributesSelector.Setup(p => p.SelectFrom("ArmorAttributes", "armor name")).Returns(attributes);

            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetSpecificItemsFromGenerator()
        {
            var specificResult = new TypeAndAmountPercentileResult();
            specificResult.Type = "SpecificArmor";
            specificResult.Amount = 0;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom("powerArmors")).Returns(specificResult);

            var specificArmor = new Item();
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom("power", specificResult.Type)).Returns(specificArmor);

            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor, Is.EqualTo(specificArmor));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator()
        {
            var abilityResult = new TypeAndAmountPercentileResult();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 90210;
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom("powerArmors"))
                .Returns(abilityResult).Returns(result);

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(ItemTypeConstants.Armor, It.IsAny<IEnumerable<String>>(), "power", It.IsAny<Int32>(), 90210))
                .Returns(abilities);

            var armor = magicalArmorGenerator.GenerateAtPower("power");
            Assert.That(armor.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }
    }
}