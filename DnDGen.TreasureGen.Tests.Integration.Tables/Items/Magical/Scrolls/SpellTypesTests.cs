using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls
{
    [TestFixture]
    public class SpellTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.SpellTypes; }
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

        [TestCase("Arcane", 1, 70)]
        [TestCase("Divine", 71, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}