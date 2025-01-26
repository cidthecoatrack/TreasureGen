using System;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    public abstract class BooleanPercentileTests : PercentileTests
    {
        public virtual void BooleanPercentile(Boolean isTrue, int lower, int upper)
        {
            var content = Convert.ToString(isTrue);
            base.AssertPercentile(content, lower, upper);
        }

        public virtual void BooleanPercentile(Boolean isTrue, int roll)
        {
            var content = Convert.ToString(isTrue);
            base.AssertPercentile(content, roll);
        }
    }
}