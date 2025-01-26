using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor
{
    [TestFixture]
    public class ArmorTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, ItemTypeConstants.Armor); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(ArmorConstants.PaddedArmor, 1)]
        [TestCase(ArmorConstants.LeatherArmor, 2)]
        [TestCase(ArmorConstants.ScaleMail, 43)]
        [TestCase(ArmorConstants.Chainmail, 44)]
        [TestCase(ArmorConstants.SplintMail, 58)]
        [TestCase(ArmorConstants.BandedMail, 59)]
        [TestCase(ArmorConstants.HalfPlate, 60)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }

        [TestCase(ArmorConstants.StuddedLeatherArmor, 3, 17)]
        [TestCase(ArmorConstants.ChainShirt, 18, 32)]
        [TestCase(ArmorConstants.HideArmor, 33, 42)]
        [TestCase(ArmorConstants.Breastplate, 45, 57)]
        [TestCase(ArmorConstants.FullPlate, 61, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}