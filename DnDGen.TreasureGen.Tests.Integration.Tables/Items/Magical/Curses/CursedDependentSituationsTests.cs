using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class CursedDependentSituationsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.CursedDependentSituations; }
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

        [TestCase(CurseConstants.Dependent.BelowFreezing, 1, 3)]
        [TestCase(CurseConstants.Dependent.AboveFreezing, 4, 5)]
        [TestCase(CurseConstants.Dependent.Day, 6, 10)]
        [TestCase(CurseConstants.Dependent.Night, 11, 15)]
        [TestCase(CurseConstants.Dependent.InSunlight, 16, 20)]
        [TestCase(CurseConstants.Dependent.OutSunlight, 21, 25)]
        [TestCase(CurseConstants.Dependent.Underwater, 26, 34)]
        [TestCase(CurseConstants.Dependent.OutOfWater, 35, 37)]
        [TestCase(CurseConstants.Dependent.Underground, 38, 45)]
        [TestCase(CurseConstants.Dependent.Aboveground, 46, 55)]
        [TestCase(CurseConstants.Dependent.CloseToDESIGNATEDFOE, 56, 64)]
        [TestCase(CurseConstants.Dependent.CloseToArcane, 65, 72)]
        [TestCase(CurseConstants.Dependent.CloseToDivine, 73, 80)]
        [TestCase(CurseConstants.Dependent.HandsOfNonspellcaster, 81, 85)]
        [TestCase(CurseConstants.Dependent.HandsOfSpellcaster, 86, 90)]
        [TestCase(CurseConstants.Dependent.HandsOfPARTIALALIGNMENT, 91, 93)]
        [TestCase(CurseConstants.Dependent.HandsOfFULLALIGNMENT, 94, 95)]
        [TestCase(CurseConstants.Dependent.NonholyDays, 97, 99)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(CurseConstants.Dependent.HandsOfGENDER, 96)]
        [TestCase(CurseConstants.Dependent.MilesFromSite, 100)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}