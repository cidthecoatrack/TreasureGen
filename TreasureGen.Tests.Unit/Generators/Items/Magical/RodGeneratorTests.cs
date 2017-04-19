﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Generators.Items;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class RodGeneratorTests
    {
        private MagicalItemGenerator rodGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionsSelector> mockCollectionsSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<IBooleanPercentileSelector> mockBooleanPercentileSelector;
        private TypeAndAmountPercentileResult result;
        private string power;
        private ItemVerifier itemVerifier;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Generator generator;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionsSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            result = new TypeAndAmountPercentileResult();
            mockBooleanPercentileSelector = new Mock<IBooleanPercentileSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            generator = new ConfigurableIterativeGenerator(5);
            rodGenerator = new RodGenerator(mockTypeAndAmountPercentileSelector.Object, mockCollectionsSelector.Object,
                mockChargesGenerator.Object, mockBooleanPercentileSelector.Object, mockSpecialAbilitiesGenerator.Object, generator);
            itemVerifier = new ItemVerifier();

            result.Type = "rod of ability";
            result.Amount = 9266;
            power = "power";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(result);
        }

        [Test]
        public void GenerateRod()
        {
            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(result.Type));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(result.Amount));
        }

        [Test]
        public void GetBaseNames()
        {
            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, result.Type)).Returns(baseNames);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void MinorPowerThrowsError()
        {
            Assert.That(() => rodGenerator.GenerateAtPower(PowerConstants.Minor), Throws.ArgumentException.With.Message.EqualTo("Cannot generate minor rods"));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, result.Type)).Returns(attributes);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetChargesIfCharged()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, result.Type)).Returns(90210);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(90210));
        }

        [Test]
        public void DoNotGetChargesIfNotCharged()
        {
            var attributes = new[] { "new attribute" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, result.Type)).Returns(90210);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void RodOfAbsorptionContainsLevelsIfSelectorSaysSo()
        {
            result.Type = RodConstants.Absorption;
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, result.Type)).Returns(42);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, RodConstants.FullAbsorption)).Returns(50);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.RodOfAbsorptionContainsSpellLevels)).Returns(true);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(42));
            Assert.That(rod.Contents, Contains.Item("4 spell levels"));
            Assert.That(rod.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void RodOfAbsorptionDoesNotContainLevelsIfSelectorSaysSo()
        {
            result.Type = RodConstants.Absorption;
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, result.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, result.Type)).Returns(42);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, RodConstants.FullAbsorption)).Returns(50);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.RodOfAbsorptionContainsSpellLevels)).Returns(false);

            var rod = rodGenerator.GenerateAtPower(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(42));
            Assert.That(rod.Contents, Is.Empty);
        }

        [Test]
        public void GenerateCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 9266, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 0, Type = name },
            });

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
        }

        [Test]
        public void GenerateCustomRodWithBonus()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 9266, Type = name },
            });

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateCustomRodThatCanBeAWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Club };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 9266, Type = name },
            });

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateRandomCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 9266, Type = name },
            });

            var rod = rodGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateOneTimeUseCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.OneTimeUse };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 0, Type = name },
            });

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
        }

        [Test]
        public void GenerateFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.SetupSequence(s => s.SelectFrom(tableName))
                .Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 })
                .Returns(new TypeAndAmountPercentileResult { Type = "rod", Amount = 90210 })
                .Returns(new TypeAndAmountPercentileResult { Type = "other rod", Amount = 42 });

            var subset = new[] { "rod", "other rod" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateAsWeaponFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.SetupSequence(s => s.SelectFrom(tableName))
                .Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 })
                .Returns(new TypeAndAmountPercentileResult { Type = "rod", Amount = 90210 })
                .Returns(new TypeAndAmountPercentileResult { Type = "other rod", Amount = 42 });

            var subset = new[] { "base name", "other rod" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateDefaultAsMiedumFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 });

            var subset = new[] { "other rod", "rod" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns((IEnumerable<string> ss) => ss.Last());

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 666, Type = "wrong rod" },
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 0, Type = "rod" },
            });

            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateDefaultAsMajorFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 });

            var subset = new[] { "other rod", "rod" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns((IEnumerable<string> ss) => ss.Last());

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 666, Type = "wrong rod" },
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 0, Type = "rod" },
            });

            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test, Ignore("Since defaults take a subset and make it a name, matching to a base name won't work")]
        public void GenerateDefaultAsWeaponFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 });

            var subset = new[] { "other rod", "base name" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns((IEnumerable<string> ss) => ss.Last());

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 666, Type = "wrong rod" },
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 0, Type = "rod" },
            });

            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateDefaultWithBonusFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(new TypeAndAmountPercentileResult { Type = "wrong rod", Amount = 9266 });

            var subset = new[] { "other rod", "rod" };

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, "rod")).Returns(attributes);

            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns((IEnumerable<string> ss) => ss.Last());

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountPercentileResult { Amount = 666, Type = "wrong rod" },
                new TypeAndAmountPercentileResult { Amount = 90210, Type = "other rod" },
                new TypeAndAmountPercentileResult { Amount = 9266, Type = "rod" },
            });


            var rod = rodGenerator.GenerateFromSubset(power, subset);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void MinorPowerFromSubsetThrowsError()
        {
            var subset = new[] { "rod", "other rod" };
            Assert.That(() => rodGenerator.GenerateFromSubset(PowerConstants.Minor, subset), Throws.ArgumentException.With.Message.EqualTo("Cannot generate minor rods"));
        }
    }
}