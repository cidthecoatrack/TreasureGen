using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls
{
    [TestFixture]
    public class MajorSpellLevelsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERSpellLevels, PowerConstants.Major); }
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

        [TestCase("4", 1, 5)]
        [TestCase("5", 6, 50)]
        [TestCase("6", 51, 70)]
        [TestCase("7", 71, 85)]
        [TestCase("8", 86, 95)]
        [TestCase("9", 96, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}