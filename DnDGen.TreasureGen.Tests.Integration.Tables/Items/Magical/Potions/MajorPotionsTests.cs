using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Potions
{
    [TestFixture]
    public class MajorPotionsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Potion); }
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

        [TestCase(PotionConstants.Blur, 0, 1, 2)]
        [TestCase(PotionConstants.CureModerateWounds, 0, 3, 7)]
        [TestCase(PotionConstants.Darkvision, 0, 8, 9)]
        [TestCase(PotionConstants.ShieldOfFaith, 4, 17, 18)]
        [TestCase(PotionConstants.ResistENERGY_20, 0, 19, 20)]
        [TestCase(PotionConstants.CureSeriousWounds, 0, 21, 28)]
        [TestCase(PotionConstants.Displacement, 0, 30, 32)]
        [TestCase(PotionConstants.Fly, 0, 34, 38)]
        [TestCase(PotionConstants.Haste, 0, 40, 41)]
        [TestCase(PotionConstants.Heroism, 0, 42, 44)]
        [TestCase(PotionConstants.KeenEdge, 0, 45, 46)]
        [TestCase(PotionConstants.NeutralizePoison, 0, 48, 50)]
        [TestCase(PotionConstants.Nondetection, 0, 51, 52)]
        [TestCase(PotionConstants.ProtectionFromENERGY, 0, 53, 54)]
        [TestCase(PotionConstants.Barkskin, 4, 62, 63)]
        [TestCase(PotionConstants.ResistENERGY_30, 0, 66, 68)]
        [TestCase(PotionConstants.MagicFang_Greater, 2, 70, 73)]
        [TestCase(PotionConstants.MagicWeapon_Greater, 2, 74, 77)]
        [TestCase(PotionConstants.MagicVestment, 2, 78, 81)]
        [TestCase(PotionConstants.MagicFang_Greater, 3, 83, 85)]
        [TestCase(PotionConstants.MagicWeapon_Greater, 3, 86, 88)]
        [TestCase(PotionConstants.MagicVestment, 3, 89, 91)]
        [TestCase(PotionConstants.MagicFang_Greater, 4, 92, 93)]
        [TestCase(PotionConstants.MagicWeapon_Greater, 4, 94, 95)]
        [TestCase(PotionConstants.MagicVestment, 4, 96, 97)]
        public override void AssertTypeAndAmountPercentile(string name, int amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(name, amount, lower, upper);
        }

        [TestCase(PotionConstants.Invisibility_Potion, 0, 10)]
        [TestCase(PotionConstants.Invisibility_Oil, 0, 11)]
        [TestCase(PotionConstants.Restoration_Lesser, 0, 12)]
        [TestCase(PotionConstants.RemoveParalysis, 0, 13)]
        [TestCase(PotionConstants.ShieldOfFaith, 3, 14)]
        [TestCase(PotionConstants.UndetectableAlignment, 0, 15)]
        [TestCase(PotionConstants.Barkskin, 3, 16)]
        [TestCase(PotionConstants.Daylight, 0, 29)]
        [TestCase(PotionConstants.FlameArrow, 0, 33)]
        [TestCase(PotionConstants.GaseousForm, 0, 39)]
        [TestCase(PotionConstants.MagicCircleAgainstPARTIALALIGNMENT, 0, 47)]
        [TestCase(PotionConstants.Rage, 0, 55)]
        [TestCase(PotionConstants.RemoveBlindnessDeafness, 0, 56)]
        [TestCase(PotionConstants.RemoveCurse, 0, 57)]
        [TestCase(PotionConstants.RemoveDisease, 0, 58)]
        [TestCase(PotionConstants.Tongues, 0, 59)]
        [TestCase(PotionConstants.WaterBreathing, 0, 60)]
        [TestCase(PotionConstants.WaterWalk, 0, 61)]
        [TestCase(PotionConstants.ShieldOfFaith, 5, 64)]
        [TestCase(PotionConstants.GoodHope, 0, 65)]
        [TestCase(PotionConstants.Barkskin, 5, 69)]
        [TestCase(PotionConstants.ProtectionFromArrows_15, 0, 82)]
        [TestCase(PotionConstants.MagicFang_Greater, 5, 98)]
        [TestCase(PotionConstants.MagicWeapon_Greater, 5, 99)]
        [TestCase(PotionConstants.MagicVestment, 5, 100)]
        public override void AssertTypeAndAmountPercentile(string name, int amount, int roll)
        {
            base.AssertTypeAndAmountPercentile(name, amount, roll);
        }
    }
}