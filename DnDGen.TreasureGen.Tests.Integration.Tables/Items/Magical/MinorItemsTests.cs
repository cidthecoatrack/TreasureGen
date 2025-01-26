using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class MinorItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Minor); }
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

        [TestCase(ItemTypeConstants.Armor, 1, 4)]
        [TestCase(ItemTypeConstants.Weapon, 5, 9)]
        [TestCase(ItemTypeConstants.Potion, 10, 44)]
        [TestCase(ItemTypeConstants.Ring, 45, 46)]
        [TestCase(ItemTypeConstants.Scroll, 47, 81)]
        [TestCase(ItemTypeConstants.Wand, 82, 91)]
        [TestCase(ItemTypeConstants.WondrousItem, 92, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}