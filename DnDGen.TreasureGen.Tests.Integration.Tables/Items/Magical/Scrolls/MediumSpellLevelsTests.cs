using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls
{
    [TestFixture]
    public class MediumSpellLevelsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERSpellLevels, PowerConstants.Medium); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase("2", 1, 5)]
        [TestCase("3", 6, 65)]
        [TestCase("4", 66, 95)]
        [TestCase("5", 96, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}