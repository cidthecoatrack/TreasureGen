using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IsIntelligentTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.IsIntelligent; }
        }

        //TODO: Convert to TypeAndAmount, where amount is the int threshold to be intelligent. High should make intelligent

        [TestCase(false, 2, 100)]
        public override void BooleanPercentile(Boolean isTrue, int lower, int upper)
        {
            base.BooleanPercentile(isTrue, lower, upper);
        }

        [TestCase(true, 1)]
        public override void BooleanPercentile(Boolean isTrue, int roll)
        {
            base.BooleanPercentile(isTrue, roll);
        }
    }
}