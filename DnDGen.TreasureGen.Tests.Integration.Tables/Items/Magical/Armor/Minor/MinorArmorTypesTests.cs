using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorArmorTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, PowerConstants.Minor); }
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

        [TestCase(AttributeConstants.Shield, 1, 60)]
        [TestCase(ItemTypeConstants.Armor, 61, 80)]
        [TestCase(AttributeConstants.Shield, 81, 85)]
        [TestCase(ItemTypeConstants.Armor, 86, 89)]
        [TestCase(AttributeConstants.Shield, 90, 96)]
        [TestCase(ItemTypeConstants.Armor, 97, 100)]
        public override void AssertPercentile(string type, int lower, int upper)
        {
            base.AssertPercentile(type, lower, upper);
        }
    }
}