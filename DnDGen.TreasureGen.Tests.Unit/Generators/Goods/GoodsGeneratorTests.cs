using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Goods;
using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Goods
{
    [TestFixture]
    public class GoodsGeneratorTests
    {
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private IGoodsGenerator generator;

        private TypeAndAmountSelection selection;
        private TypeAndAmountSelection valueSelection;
        private List<string> descriptions;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            generator = new GoodsGenerator(mockTypeAndAmountPercentileSelector.Object, mockCollectionsSelector.Object);
            selection = new TypeAndAmountSelection();
            valueSelection = new TypeAndAmountSelection();

            selection.Type = "type";
            selection.Amount = 2;
            valueSelection.Type = "first value";
            valueSelection.Amount = 9266;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns(selection);

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues, selection.Type);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(valueSelection);

            descriptions = new List<string> { "description 1", "description 2" };
            tableName = TableNameConstants.Collections.GOODTYPEDescriptions, selection.Type);

            var count = 0;
            object testLock = new object();
            mockCollectionsSelector
                .Setup(p => p.SelectRandomFrom(Config.Name, tableName, It.IsAny<string>()))
                .Returns(() =>
                {
                    lock (testLock)
                    {
                        return descriptions[count++ % descriptions.Count];
                    }
                });
        }

        [Test]
        public void GenerateAtLevel_ThrowsException_LevelTooLow()
        {
            Assert.That(() => generator.GenerateAtLevel(LevelLimits.Minimum - 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 0 is not a valid level for treasure generation"));
        }

        [Test]
        public void GenerateAtLevel_ThrowsException_LevelTooHigh()
        {
            Assert.That(() => generator.GenerateAtLevel(LevelLimits.Maximum + 1),
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
        public void GenerateAtLevel_GoodsAreGenerated(int level)
        {
            var goods = generator.GenerateAtLevel(level);
            Assert.That(goods, Is.Not.Null);
        }

        [Test]
        public void GenerateAtLevel_GetTypeAndAmountFromSelector()
        {
            var tableName = TableNameConstants.Percentiles.LevelXGoods, 1);
            generator.GenerateAtLevel(1);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(tableName), Times.Once);
        }

        [Test]
        public void GenerateAtLevel_EmptyGoodsIfNoGoodType()
        {
            selection.Type = string.Empty;
            var goods = generator.GenerateAtLevel(1);
            Assert.That(goods, Is.Empty);
        }

        [Test]
        public void GenerateAtLevel_ReturnsNumberOfGoods()
        {
            selection.Amount = 9266;
            var goods = generator.GenerateAtLevel(1);
            Assert.That(goods.Count(), Is.EqualTo(9266));
        }

        [Test]
        public void GenerateAtLevel_ValueDeterminedByValueResult()
        {
            var secondValueResult = new TypeAndAmountSelection();
            secondValueResult.Type = "second value";
            secondValueResult.Amount = 90210;

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues, selection.Type);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName)).Returns(valueSelection).Returns(secondValueResult);

            var goods = generator.GenerateAtLevel(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.ValueInGold, Is.EqualTo(9266));
            Assert.That(secondGood.ValueInGold, Is.EqualTo(90210));
        }

        [Test]
        public void GenerateAtLevel_DescriptionDeterminedByValueResult()
        {
            var goods = generator.GenerateAtLevel(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.Description, Is.EqualTo("description 1"));
            Assert.That(secondGood.Description, Is.EqualTo("description 2"));
        }

        [Test]
        public async Task GenerateAtLevelAsync_GoodsAreGenerated()
        {
            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods, Is.Not.Null);
        }

        [Test]
        public async Task GenerateAtLevelAsync_GetTypeAndAmountFromSelector()
        {
            var tableName = TableNameConstants.Percentiles.LevelXGoods, 1);
            await generator.GenerateAtLevelAsync(1);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(tableName), Times.Once);
        }

        [Test]
        public async Task GenerateAtLevelAsync_EmptyGoodsIfNoGoodType()
        {
            selection.Type = string.Empty;
            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods, Is.Empty);
        }

        [Test]
        public async Task GenerateAtLevelAsync_ReturnsNumberOfGoods()
        {
            selection.Amount = 9266;
            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods.Count(), Is.EqualTo(9266));
        }

        [Test]
        public async Task GenerateAtLevelAsync_ValueDeterminedByValueResult()
        {
            var secondValueResult = new TypeAndAmountSelection();
            secondValueResult.Type = "second value";
            secondValueResult.Amount = 90210;

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues, selection.Type);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName)).Returns(valueSelection).Returns(secondValueResult);

            var goods = await generator.GenerateAtLevelAsync(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.ValueInGold, Is.EqualTo(9266).Or.EqualTo(90210));
            Assert.That(secondGood.ValueInGold, Is.EqualTo(9266).Or.EqualTo(90210));
            Assert.That(firstGood.ValueInGold, Is.Not.EqualTo(secondGood.ValueInGold));
        }

        [Test]
        public async Task GenerateAtLevelAsync_DescriptionDeterminedByValueResult()
        {
            var goods = await generator.GenerateAtLevelAsync(1);
            var descriptions = goods.Select(g => g.Description);

            Assert.That(descriptions, Contains.Item("description 1")
                .And.Contains("description 2"));
        }
    }
}