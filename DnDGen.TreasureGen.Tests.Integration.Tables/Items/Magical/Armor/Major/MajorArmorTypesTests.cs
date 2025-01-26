using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorArmorTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, PowerConstants.Major); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(AttributeConstants.Shield, 1, 8)]
        [TestCase(ItemTypeConstants.Armor, 9, 16)]
        [TestCase(AttributeConstants.Shield, 17, 27)]
        [TestCase(ItemTypeConstants.Armor, 28, 38)]
        [TestCase(AttributeConstants.Shield, 39, 49)]
        [TestCase(ItemTypeConstants.Armor, 50, 60)]
        [TestCase(AttributeConstants.Shield, 61, 63)]
        [TestCase(ItemTypeConstants.Armor, 64, 66)]
        [TestCase(AttributeConstants.Shield, 67, 85)]
        [TestCase(ItemTypeConstants.Armor, 86, 100)]
        public override void AssertPercentile(string type, int lower, int upper)
        {
            base.AssertPercentile(type, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}