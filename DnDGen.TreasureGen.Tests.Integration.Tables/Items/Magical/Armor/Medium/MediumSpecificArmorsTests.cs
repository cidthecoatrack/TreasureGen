using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumSpecificArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Armor); }
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

        [TestCase(ArmorConstants.ChainShirt, 0, 1, 25)]
        [TestCase(ArmorConstants.FullPlate, 0, 26, 45)]
        [TestCase(ArmorConstants.ElvenChain, 0, 46, 57)]
        [TestCase(ArmorConstants.RhinoHide, 2, 58, 67)]
        [TestCase(ArmorConstants.Breastplate, 0, 68, 82)]
        [TestCase(ArmorConstants.DwarvenPlate, 0, 83, 97)]
        [TestCase(ArmorConstants.BandedMailOfLuck, 3, 98, 100)]
        public override void AssertTypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}