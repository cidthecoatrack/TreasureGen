using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class IntelligenceGeneratorTests
    {
        private IIntelligenceGenerator intelligenceGenerator;
        private Mock<Dice> mockDice;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IIntelligenceDataSelector> mockIntelligenceDataSelector;
        private List<string> attributes;
        private IntelligenceDataSelection intelligenceSelection;
        private Item item;
        private string itemType;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockIntelligenceDataSelector = new Mock<IIntelligenceDataSelector>();
            intelligenceSelection = new IntelligenceDataSelection();
            attributes = new List<string>();
            item = new Item();
            itemType = "item type";

            var fillerValues = new[] { "0" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, It.IsAny<string>(), It.IsAny<string>())).Returns(fillerValues);
            mockDice.Setup(d => d.Roll(1).d(4).AsSum<int>()).Returns(4);
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("10");
            mockIntelligenceDataSelector.Setup(s => s.SelectFrom(It.IsAny<string>())).Returns(intelligenceSelection);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments)).Returns(string.Empty);

            intelligenceGenerator = new IntelligenceGenerator(mockDice.Object, mockPercentileSelector.Object, mockCollectionsSelector.Object, mockIntelligenceDataSelector.Object);
        }

        [Test]
        public void DetermineIntelligentFromBooleanSelector()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void DetermineNotIntelligentFromBooleanSelector()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(false);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void DetermineMeleeIntelligence()
        {
            attributes.Add(AttributeConstants.Melee);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Melee);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void DetermineRangedIntelligence()
        {
            attributes.Add(AttributeConstants.Ranged);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Ranged);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void DetermineRangedAndMeleeIntelligence()
        {
            attributes.Add(AttributeConstants.Melee);
            attributes.Add(AttributeConstants.Ranged);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Melee);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void AmmunitionIsNotIntelligent()
        {
            attributes.Add(AttributeConstants.Ammunition);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void OneTimeUseItemsAreNotIntelligent()
        {
            attributes.Add(AttributeConstants.OneTimeUse);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void NonMagicalItemsAreNotIntelligent()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, false);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void AmmunitionCannotBeIntelligent()
        {
            attributes.Add(AttributeConstants.Ammunition);

            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, true);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void OneTimeUseItemsCannotBeIntelligent()
        {
            attributes.Add(AttributeConstants.OneTimeUse);

            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, true);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void NonMagicalItemsCannotBeIntelligent()
        {
            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, false);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void ReturnIntelligence()
        {
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence, Is.Not.Null);
        }

        [Test]
        public void Roll1MeansCharismaIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(10));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll2MeansIntelligenceIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(10));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll3MeansWisdomIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(10));
        }

        [Test]
        public void GetCommunicationFromAttributesSelector()
        {
            var attributes = new[] { "talky" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("9266");
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "9266")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Communication, Is.EqualTo(attributes));
        }

        [Test]
        public void GetLanguagesIfSpeech()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("10");
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Contains.Item("Common"));
        }

        [Test]
        public void GetNumberOfBonusLanguagesEqualToIntelligenceModifier()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("14");
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.Languages)).Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Has.Count.EqualTo(3)
                .And.Contains("Common")
                .And.Contains("english")
                .And.Contains("german"));
        }

        [Test]
        public void DoNotHaveDuplicateLanguages()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("14");
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.Languages))
                .Returns("english").Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Has.Count.EqualTo(3)
                .And.Contains("Common")
                .And.Contains("english")
                .And.Contains("german"));
        }

        [Test]
        public void GetSensesFromAttributesSelector()
        {
            intelligenceSelection.Senses = "sensy";
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Senses, Is.EqualTo(intelligenceSelection.Senses));
        }

        [Test]
        public void GetLesserPowersFromAttributesSelector()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateLesserPowers()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
        }

        [Test]
        public void GetGreaterPowersFromAttributesSelector()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateGreaterPowers()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
        }

        [Test]
        public void ZeroGreaterPowerMeans0PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 0;

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(5)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void OneGreaterPowerMeans25PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 1;

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void OneGreaterPowerMeans75PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 1;

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(false);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(3)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(true);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(3)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(false);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(4)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void ThreeGreaterPowerMeans75PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 3;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(2)).Returns(true);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(4)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void ThreeGreaterPowerMeans25PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 3;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(2)).Returns(false);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(5)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2")
                .And.Contains("greater power 3"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void GetAlignmentFromPercentileSelector()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments)).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Chaotic)]
        public void NonAxiomaticAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Lawful)]
        [TestCase("True")]
        public void AxiomaticAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase(AlignmentConstants.Lawful)]
        public void NonAnarchicAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Chaotic)]
        [TestCase("True")]
        public void AnarchicAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase(AlignmentConstants.Evil)]
        public void NonHolyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Good)]
        public void HolyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [TestCase(AlignmentConstants.Good)]
        public void NonUnholyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Evil)]
        public void UnholyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [Test]
        public void ItemWithSpecificAlignmentHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific alignment";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns(alignment);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment));
        }

        [Test]
        public void ItemWithNoSpecificAlignmentHasAnyAlignment()
        {
            item.Name = "item name";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, "Items")).Returns(new[] { "other item name" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { "specific" });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentBeginningHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentEndingHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "ending";
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment ending");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment ending"));
        }

        [Test]
        public void ItemWithPartOfAlignmentAsTraitUsesThatAsAlignmentRequirement()
        {
            item.Traits.Add(AlignmentConstants.Good);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment Evil").Returns("alignment Good");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment Good"));
        }

        [Test]
        public void ItemWithPartOfAlignmentAsPartOfTraitUsesThatAsAlignmentRequirement()
        {
            var trait = string.Format("trait ({0})", AlignmentConstants.Good);
            item.Traits.Add(trait);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment Evil").Returns("alignment Good");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment Good"));
        }

        [Test]
        public void ItemWithAlignmentAsTraitUsesThatAsAlignmentRequirement()
        {
            item.Traits.Add(AlignmentConstants.ChaoticNeutral);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void ItemWithAlignmentAsPartOfTraitUsesThatAsAlignmentRequirement()
        {
            var trait = string.Format("trait ({0})", AlignmentConstants.ChaoticNeutral);
            item.Traits.Add(trait);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void OnlyAlignmentsEndingInNeutralMatchNeutralRequirement()
        {
            item.Traits.Add(AlignmentConstants.Neutral);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(AlignmentConstants.NeutralEvil).Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void TrueNeutralSatisfiesAllRequirements()
        {
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = SpecialAbilityConstants.Unholy },
                new SpecialAbility { Name = SpecialAbilityConstants.Holy },
                new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic },
                new SpecialAbility { Name = SpecialAbilityConstants.Anarchic }
            };

            item.Traits.Add(AlignmentConstants.ChaoticEvil);
            item.Traits.Add($"trait ({AlignmentConstants.Good})");

            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { "specific" });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(AlignmentConstants.ChaoticEvil)
                .Returns(AlignmentConstants.ChaoticGood)
                .Returns(AlignmentConstants.ChaoticNeutral)
                .Returns(AlignmentConstants.LawfulEvil)
                .Returns(AlignmentConstants.LawfulGood)
                .Returns(AlignmentConstants.LawfulNeutral)
                .Returns(AlignmentConstants.NeutralEvil)
                .Returns(AlignmentConstants.NeutralGood)
                .Returns(AlignmentConstants.TrueNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.TrueNeutral));
        }

        [Test]
        public void EgoIncludesMagicBonus()
        {
            item.Magic.Bonus = 9266;

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(9266));
        }

        [Test]
        public void EgoIncludesSpecialAbilityBonuses()
        {
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { BonusEquivalent = 9200 },
                new SpecialAbility { BonusEquivalent = 66 }
            };

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(9266));
        }

        [Test]
        public void EgoIncludesLesserPowers()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(2));
        }

        [Test]
        public void EgoIncludesGreaterPowers()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power 1").Returns("greater power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesDedicatedPower()
        {
            intelligenceSelection.GreaterPowersCount = 1;
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power");
            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesTelepathy()
        {
            var attributes = new[] { "Telepathy" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReading()
        {
            var attributes = new[] { "Read" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReadMagic()
        {
            var attributes = new[] { "Read magic" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [TestCase(10, 0)]
        [TestCase(11, 0)]
        [TestCase(12, 2)]
        [TestCase(13, 2)]
        [TestCase(14, 4)]
        [TestCase(15, 4)]
        [TestCase(16, 6)]
        [TestCase(17, 6)]
        [TestCase(18, 8)]
        [TestCase(19, 8)]
        [TestCase(20, 10)]
        public void EgoIncludesStatBonuses(int strongStat, int egoBonus)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns(strongStat.ToString());
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(egoBonus));
        }

        [Test]
        public void EgoSumsAllFactors()
        {
            var communication = new[] { "Read", "Read magic", "Telepathy" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("19");
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.IntelligenceCommunication, "19")).Returns(communication);
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power 1").Returns("greater power 2");
            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var ability = new SpecialAbility();
            ability.BonusEquivalent = 92;
            item.Magic.SpecialAbilities = new[] { ability };
            item.Magic.Bonus = 66;

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(177));
        }

        [Test]
        public void IntelligenceHasPersonality()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.PersonalityTraits)).Returns("personality");
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Personality, Is.EqualTo("personality"));
        }
    }
}