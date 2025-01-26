using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceStrongStatTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.IntelligenceStrongStats; }
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

        [TestCase("12", 1, 34)]
        [TestCase("13", 35, 59)]
        [TestCase("14", 60, 79)]
        [TestCase("15", 80, 91)]
        [TestCase("16", 92, 97)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("17", 98)]
        [TestCase("18", 99)]
        [TestCase("19", 100)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}