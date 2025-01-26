using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorArmorsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Minor, ItemTypeConstants.Armor); }
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

        [TestCase("1", 1, 80)]
        [TestCase("2", 81, 87)]
        [TestCase(ItemTypeConstants.Armor, 88, 91)]
        [TestCase("SpecialAbility", 92, 100)]
        public override void AssertPercentile(string value, int lower, int upper)
        {
            base.AssertPercentile(value, lower, upper);
        }
    }
}