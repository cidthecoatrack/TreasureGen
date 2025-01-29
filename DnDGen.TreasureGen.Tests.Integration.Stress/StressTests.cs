using DnDGen.Stress;
using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System;
using System.Reflection;

namespace DnDGen.TreasureGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        public Random Random { get; set; }

        [SetUp]
        public void StressSetup()
        {
            Random = GetNewInstanceOf<Random>();
        }

        protected Stressor stressor;

        [OneTimeSetUp]
        public void StressOneTimeSetup()
        {
            var options = new StressorOptions();
            options.RunningAssembly = Assembly.GetExecutingAssembly();

            //INFO: Non-stress operations take about 5 minutes, or 9% of the total runtime
            options.TimeLimitPercentage = .91;

#if STRESS
            options.IsFullStress = true;
#else
            options.IsFullStress = false;
#endif

            stressor = new Stressor(options);
        }

        protected int GetNewLevel()
        {
            return Random.Next(LevelLimits.Minimum, LevelLimits.Maximum_Epic + 1);
        }

        protected string GetNewPower(bool allowMinor = true)
        {
            var limit = allowMinor ? 3 : 2;

            switch (Random.Next(limit))
            {
                case 0: return PowerConstants.Major;
                case 1: return PowerConstants.Medium;
                default: return PowerConstants.Minor;
            }
        }
    }
}