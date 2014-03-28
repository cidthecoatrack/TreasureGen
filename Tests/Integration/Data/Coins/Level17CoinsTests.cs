﻿using System;
using EquipmentGen.Common.Coins;
using EquipmentGen.Tests.Integration.Tables.TestAttributes;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Coins
{
    [TestFixture, PercentileTable("Level17Coins")]
    public class Level17CoinTests : PercentileTests
    {
        [Test]
        public void Level17EmptyPercentile()
        {
            AssertPercentile(String.Empty, 1, 3);
        }

        [Test]
        public void Level17GoldPercentile()
        {
            var result = String.Format("{0},3d4*1000", CoinConstants.Gold);
            AssertPercentile(result, 4, 68);
        }

        [Test]
        public void Level17PlatinumPercentile()
        {
            var result = String.Format("{0},2d10*100", CoinConstants.Platinum);
            AssertPercentile(result, 69, 100);
        }
    }
}