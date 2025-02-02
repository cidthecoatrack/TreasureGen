using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class ItemsGeneratorTests
    {
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private Mock<MundaneItemGenerator> mockMundaneItemGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<MagicalItemGenerator> mockMagicalItemGenerator;
        private IItemsGenerator itemsGenerator;
        private TypeAndAmountSelection selection;
        private Mock<ICollectionSelector> mockCollectionSelector;

        [SetUp]
        public void Setup()
        {
            selection = new TypeAndAmountSelection();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockMagicalItemGenerator = new Mock<MagicalItemGenerator>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockMundaneItemGenerator = new Mock<MundaneItemGenerator>();
            mockCollectionSelector = new Mock<ICollectionSelector>();

            itemsGenerator = new ItemsGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockJustInTimeFactory.Object,
                mockPercentileSelector.Object,
                mockCollectionSelector.Object);

            selection.Type = "power";
            selection.Amount = 42;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns(selection);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(ItemTypeConstants.WondrousItem);

            var dummyMagicalMock = new Mock<MagicalItemGenerator>();
            dummyMagicalMock.Setup(m => m.GenerateRandom(It.IsAny<string>())).Returns(() => new Item { Name = "magical item" });
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(It.IsAny<string>())).Returns(dummyMagicalMock.Object);

            var dummyMundaneMock = new Mock<MundaneItemGenerator>();
            dummyMundaneMock.Setup(m => m.GenerateRandom()).Returns(() => new Item { Name = "mundane item" });
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(dummyMundaneMock.Object);
        }

        [Test]
        public void GenerateRandomAtLevel_ThrowsException_LevelTooLow()
        {
            Assert.That(() => itemsGenerator.GenerateRandomAtLevel(LevelLimits.Minimum - 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 0 is not a valid level for treasure generation"));
        }

        [Test]
        public void GenerateRandomAtLevel_ThrowsException_LevelTooHigh()
        {
            Assert.That(() => itemsGenerator.GenerateRandomAtLevel(LevelLimits.Maximum + 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 101 is not a valid level for treasure generation"));
        }

        [TestCase(LevelLimits.Minimum)]
        [TestCase(LevelLimits.Minimum + 1)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(42)]
        [TestCase(LevelLimits.Maximum - 1)]
        [TestCase(LevelLimits.Maximum)]
        public void GenerateRandomAtLevel_ItemsAreGenerated(int level)
        {
            var items = itemsGenerator.GenerateRandomAtLevel(level);
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public void GenerateRandomAtLevel_GetItemTypeFromSelector()
        {
            itemsGenerator.GenerateRandomAtLevel(42);
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems, 42);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(expectedTableName), Times.Once);
        }

        [Test]
        public void GenerateRandomAtLevel_GetAmountFromRoll()
        {
            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Count(), Is.EqualTo(42));
        }

        [Test]
        public void GenerateRandomAtLevel_ReturnItems()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 2;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.SetupSequence(f => f.GenerateRandom()).Returns(firstItem).Returns(secondItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateRandomAtLevel_IfSelectorReturnsEmptyResult_ItemsGeneratorReturnsEmptyEnumerable()
        {
            selection.Type = string.Empty;
            selection.Amount = 0;

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void GenerateRandomAtLevel_GetMundaneItems()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 1;

            var mundaneItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems, selection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("mundane item type");
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("mundane item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.GenerateRandom()).Returns(mundaneItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(mundaneItem));
        }

        [Test]
        public void GenerateRandomAtLevel_GetMagicalItems()
        {
            selection.Type = "power";
            selection.Amount = 1;

            var magicalItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems, selection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(selection.Type)).Returns(magicalItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ItemsAreGenerated()
        {
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetItemTypeFromSelector()
        {
            await itemsGenerator.GenerateRandomAtLevelAsync(96);
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems, 96);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(expectedTableName), Times.Once);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetAmountFromRoll()
        {
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Count(), Is.EqualTo(42));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ReturnItems()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 2;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .SetupSequence(f => f.GenerateRandom())
                .Returns(firstItem)
                .Returns(secondItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_IfSelectorReturnsEmptyResult_ItemsGeneratorReturnsEmptyEnumerable()
        {
            selection.Type = string.Empty;
            selection.Amount = 0;

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetMundaneItems()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 1;

            var mundaneItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems, selection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("mundane item type");
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("mundane item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.GenerateRandom()).Returns(mundaneItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Single(), Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetMagicalItems()
        {
            selection.Type = "power";
            selection.Amount = 1;

            var magicalItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems, selection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(selection.Type)).Returns(magicalItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Single(), Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMundaneItem()
        {
            selection.Type = PowerConstants.Mundane;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMundaneItem_WhenNoPowerSpecified()
        {
            selection.Type = string.Empty;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetSpecificItem_WhenMundaneSpecified()
        {
            selection.Type = PowerConstants.Mundane;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "specific item"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(f => f.Generate("power", "specific item")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "specific item");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetSpecificItem_WhenNoneSpecified()
        {
            selection.Type = string.Empty;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "specific item"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(f => f.Generate("power", "specific item")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "specific item");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenNoPowerSpecified()
        {
            selection.Type = string.Empty;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMundaneItem_WhenPowerIsMagical_ButItemTypeIsMundane()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenPowerIsMundane_ButItemTypeIsMagical()
        {
            selection.Type = PowerConstants.Mundane;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenPowerIsMagical_ButItemTypeIsHigherMagical()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("more power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMundaneItem_WithTraits()
        {
            selection.Type = PowerConstants.Mundane;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name", "trait 1", "trait 2")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMagicalItem()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMagicalItem_WithTraits()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_ScrollWithCustomName()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Scroll))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Scroll)).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, ItemTypeConstants.Scroll, "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WandWithCustomName()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Wand))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Wand)).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, ItemTypeConstants.Wand, "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetPowerFromSelector()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            itemsGenerator.GenerateAtLevel(96, "item type", "item name");
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems, 96);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(expectedTableName), Times.Once);
        }

        [Test]
        public void GenerateAtLevel_Named_ReturnItem()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 2;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var generatedItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MundaneItemGenerator>("item type"))
                .Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .Setup(f => f.Generate("item name"))
                .Returns(generatedItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(generatedItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetPowerFromSelector()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            await itemsGenerator.GenerateAtLevelAsync(96, "item type", "item name");
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems, 96);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(expectedTableName), Times.Once);
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_ReturnItem()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 2;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var generatedItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MundaneItemGenerator>("item type"))
                .Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .Setup(f => f.Generate("item name"))
                .Returns(generatedItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(generatedItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetMundaneItem()
        {
            selection.Type = PowerConstants.Mundane;
            selection.Amount = 1;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var mundaneItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.Generate("item name")).Returns(mundaneItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMundaneItem_WithNoPower()
        {
            selection.Type = string.Empty;
            selection.Amount = 1;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var mundaneItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.Generate("item name")).Returns(mundaneItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WithNoPower()
        {
            selection.Type = string.Empty;
            selection.Amount = 1;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMundaneItem_WhenPowerIsMagical_ButItemTypeIsMundane()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WhenPowerIsMundane_ButItemTypeIsMagical()
        {
            selection.Type = PowerConstants.Mundane;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WhenPowerIsMagical_ButItemTypeIsHigherMagical()
        {
            selection.Type = "power";

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("more power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetMagicalItem()
        {
            selection.Type = "power";
            selection.Amount = 1;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MagicalItemGenerator>("item type"))
                .Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator
                .Setup(g => g.Generate(selection.Type, "item name"))
                .Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }
    }
}