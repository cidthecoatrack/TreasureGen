using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class CastersShieldSpellTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.CastersShieldSpellTypes; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("Divine", 1, 80)]
        [TestCase("Arcane", 81, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}