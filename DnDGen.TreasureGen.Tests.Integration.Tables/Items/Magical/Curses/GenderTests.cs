using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class GenderTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.Set.Gender; }
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

        [TestCase("Male", 1, 50)]
        [TestCase("Female", 51, 100)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}