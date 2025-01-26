using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class CurseDrawbacksTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.CurseDrawbacks; }
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

        [TestCase(CurseConstants.Drawbacks.HairGrows, 1, 4)]
        [TestCase(CurseConstants.Drawbacks.HEIGHTChanges, 5, 9)]
        [TestCase(CurseConstants.Drawbacks.Cooler, 10, 13)]
        [TestCase(CurseConstants.Drawbacks.Warmer, 14, 17)]
        [TestCase(CurseConstants.Drawbacks.HairColorChanges, 18, 21)]
        [TestCase(CurseConstants.Drawbacks.SkinColorChanges, 22, 25)]
        [TestCase(CurseConstants.Drawbacks.IdentifyingMark, 26, 29)]
        [TestCase(CurseConstants.Drawbacks.GenderChanges, 30, 32)]
        [TestCase(CurseConstants.Drawbacks.RaceChanges, 33, 34)]
        [TestCase(CurseConstants.Drawbacks.EmitsSound, 36, 39)]
        [TestCase(CurseConstants.Drawbacks.Possessive, 41, 45)]
        [TestCase(CurseConstants.Drawbacks.Paranoid, 46, 49)]
        [TestCase(CurseConstants.Drawbacks.AlignmentChanges, 50, 51)]
        [TestCase(CurseConstants.Drawbacks.AttackNearestCreature, 52, 54)]
        [TestCase(CurseConstants.Drawbacks.Stunned, 55, 57)]
        [TestCase(CurseConstants.Drawbacks.BlurryVision, 58, 60)]
        [TestCase(CurseConstants.Drawbacks.NegativeLevel_1, 61, 64)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Intelligence, 66, 70)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Wisdom, 71, 75)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Charisma, 76, 80)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Constitution, 81, 85)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Strength, 86, 90)]
        [TestCase(CurseConstants.Drawbacks.StatDamage_Dexterity, 91, 95)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(CurseConstants.Drawbacks.Disease, 35)]
        [TestCase(CurseConstants.Drawbacks.Ridiculous, 40)]
        [TestCase(CurseConstants.Drawbacks.NegativeLevel_2, 65)]
        [TestCase(CurseConstants.Drawbacks.Polymorphed, 96)]
        [TestCase(CurseConstants.Drawbacks.NoSpells_Arcane, 97)]
        [TestCase(CurseConstants.Drawbacks.NoSpells_Divine, 98)]
        [TestCase(CurseConstants.Drawbacks.NoSpells_Any, 99)]
        [TestCase(CurseConstants.Drawbacks.Harm, 100)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}