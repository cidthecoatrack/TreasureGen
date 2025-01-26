using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorArmorsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Armor); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("3", 1, 16)]
        [TestCase("4", 17, 38)]
        [TestCase("5", 39, 57)]
        [TestCase(ItemTypeConstants.Armor, 58, 63)]
        [TestCase("SpecialAbility", 64, 100)]
        public override void AssertPercentile(string value, int lower, int upper)
        {
            base.AssertPercentile(value, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}