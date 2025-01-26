using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level10ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 10); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d6, 1, 40)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1d4, 41, 88)]
        [TestCase(PowerConstants.Medium, AmountConstants.Range1, 89, 99)]
        public override void AssertTypeAndAmountPercentile(string type, string amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }

        [TestCase(PowerConstants.Major, AmountConstants.Range1, 100)]
        public override void AssertTypeAndAmountPercentile(string type, string amount, int roll)
        {
            base.AssertTypeAndAmountPercentile(type, amount, roll);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}