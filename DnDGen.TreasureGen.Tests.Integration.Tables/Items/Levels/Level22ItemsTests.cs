using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level22ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, 22); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Medium, AmountConstants.Range2d4, 1, 24)] //2-8
        [TestCase(PowerConstants.Major, AmountConstants.Range1d4, 25, 77)] //1-4
        [TestCase(PowerConstants.Major, AmountConstants.Range1d3Plus2, 78, 100)] //3-5
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