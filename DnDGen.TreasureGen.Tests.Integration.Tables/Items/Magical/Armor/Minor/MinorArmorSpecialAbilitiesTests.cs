using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorArmorSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, PowerConstants.Minor, ItemTypeConstants.Armor); }
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

        [TestCase(SpecialAbilityConstants.Glamered, 1, 25)]
        [TestCase(SpecialAbilityConstants.LightFortification, 26, 32)]
        [TestCase(SpecialAbilityConstants.Slick, 33, 52)]
        [TestCase(SpecialAbilityConstants.Shadow, 53, 72)]
        [TestCase(SpecialAbilityConstants.SilentMoves, 73, 92)]
        [TestCase(SpecialAbilityConstants.SpellResistance13, 93, 96)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(SpecialAbilityConstants.ImprovedSlick, 97)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow, 98)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves, 99)]
        [TestCase("BonusSpecialAbility", 100)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}