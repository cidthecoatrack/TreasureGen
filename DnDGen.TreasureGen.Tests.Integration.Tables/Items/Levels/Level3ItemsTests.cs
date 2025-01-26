using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level3ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 3); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(EmptyContent, 1, 49)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d3, 50, 79)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1, 80, 100)]
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