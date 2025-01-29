using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumArmorsTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Armor);

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

        [TestCase("1", 1, 10)]
        [TestCase("2", 11, 20)]
        [TestCase("3", 31, 50)]
        [TestCase("4", 51, 57)]
        [TestCase(ItemTypeConstants.Armor, 58, 63)]
        [TestCase("SpecialAbility", 64, 100)]
        public void MediumArmorsPercentile(string value, int lower, int upper)
        {
            AssertPercentile(value, lower, upper);
        }
    }
}