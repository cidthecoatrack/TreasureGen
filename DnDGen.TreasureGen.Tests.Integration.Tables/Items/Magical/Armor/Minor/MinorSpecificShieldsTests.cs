using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorSpecificShieldsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, PowerConstants.Minor, AttributeConstants.Shield); }
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

        [TestCase(ArmorConstants.Buckler, 0, 1, 30)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 0, 31, 80)]
        [TestCase(ArmorConstants.HeavySteelShield, 0, 81, 95)]
        [TestCase(ArmorConstants.CastersShield, 1, 96, 100)]
        public override void AssertTypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}