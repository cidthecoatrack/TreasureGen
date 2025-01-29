using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumArmorTypesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERArmorTypes(PowerConstants.Medium);

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

        [TestCase(AttributeConstants.Shield, 1, 5)]
        [TestCase(ItemTypeConstants.Armor, 6, 10)]
        [TestCase(AttributeConstants.Shield, 11, 20)]
        [TestCase(ItemTypeConstants.Armor, 21, 30)]
        [TestCase(AttributeConstants.Shield, 31, 40)]
        [TestCase(ItemTypeConstants.Armor, 41, 50)]
        [TestCase(AttributeConstants.Shield, 51, 55)]
        [TestCase(ItemTypeConstants.Armor, 56, 60)]
        [TestCase(AttributeConstants.Shield, 61, 81)]
        [TestCase(ItemTypeConstants.Armor, 82, 100)]
        public void MediumArmorTypesPercentile(string type, int lower, int upper)
        {
            base.AssertPercentile(type, lower, upper);
        }
    }
}