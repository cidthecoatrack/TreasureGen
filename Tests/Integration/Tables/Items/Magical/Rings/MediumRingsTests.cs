﻿using System;
using EquipmentGen.Common.Items;
using EquipmentGen.Tables.Interfaces;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Items.Magical.Rings
{
    [TestFixture]
    public class MediumRingsTests : TypeAndAmountPercentileTests
    {
        protected override String tableName
        {
            get { return String.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Ring); }
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase("Counterspells", 0, 1, 5)]
        [TestCase("Mind shielding", 0, 6, 8)]
        [TestCase("Protection", 2, 9, 18)]
        [TestCase("Force shield", 0, 19, 23)]
        [TestCase("Ram", 0, 24, 28)]
        [TestCase("Improved climbing", 0, 29, 34)]
        [TestCase("Improved jumping", 0, 35, 40)]
        [TestCase("Improved swimming", 0, 41, 46)]
        [TestCase("Animal friendship", 0, 47, 51)]
        [TestCase("Minor ENERGY resistance", 0, 52, 56)]
        [TestCase("Chameleon power", 0, 57, 61)]
        [TestCase("Water walking", 0, 62, 66)]
        [TestCase("Protection", 3, 67, 71)]
        [TestCase("Minor spell storing", 0, 72, 76)]
        [TestCase("Invisibility", 0, 77, 81)]
        [TestCase("Wizardry (I)", 0, 82, 85)]
        [TestCase("Evasion", 0, 86, 90)]
        [TestCase("X-ray vision", 0, 91, 93)]
        [TestCase("Blinking", 0, 94, 97)]
        [TestCase("Major ENERGY resistance", 0, 98, 100)]
        public override void TypeAndAmountPercentile(String type, Int32 amount, Int32 lower, Int32 upper)
        {
            base.TypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}