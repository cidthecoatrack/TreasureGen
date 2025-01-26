using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Levels
{
    [TestFixture]
    public class Level11GoodsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXGoods, 11); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(EmptyContent, 1, 24)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(GoodsConstants.Gem, AmountConstants.Range1d10, 25, 74)]
        [TestCase(GoodsConstants.Art, AmountConstants.Range1d6, 75, 100)]
        public override void AssertTypeAndAmountPercentile(string type, string amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}