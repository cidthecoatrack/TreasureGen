using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class MediumItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Medium); }
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

        [TestCase(ItemTypeConstants.Armor, 1, 10)]
        [TestCase(ItemTypeConstants.Weapon, 11, 20)]
        [TestCase(ItemTypeConstants.Potion, 21, 30)]
        [TestCase(ItemTypeConstants.Ring, 31, 40)]
        [TestCase(ItemTypeConstants.Rod, 41, 50)]
        [TestCase(ItemTypeConstants.Scroll, 51, 65)]
        [TestCase(ItemTypeConstants.Staff, 66, 68)]
        [TestCase(ItemTypeConstants.Wand, 69, 83)]
        [TestCase(ItemTypeConstants.WondrousItem, 84, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}