﻿using System;
using EquipmentGen.Common.Items;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Common.Items
{
    [TestFixture]
    public class IntelligenceAlignmentConstantsTests
    {
        [TestCase(IntelligenceAlignmentConstants.Chaotic, "Chaotic")]
        [TestCase(IntelligenceAlignmentConstants.Evil, "Evil")]
        [TestCase(IntelligenceAlignmentConstants.Good, "Good")]
        [TestCase(IntelligenceAlignmentConstants.Lawful, "Lawful")]
        [TestCase(IntelligenceAlignmentConstants.Neutral, "Neutral")]
        public void Constant(String constant, String value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void LawfulGoodConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.LawfulGood, Is.EqualTo("Lawful Good"));
        }

        [Test]
        public void LawfulNeutralConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.LawfulNeutral, Is.EqualTo("Lawful Neutral"));
        }

        [Test]
        public void LawfulEvilConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.LawfulEvil, Is.EqualTo("Lawful Evil"));
        }

        [Test]
        public void NeutralGoodConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.NeutralGood, Is.EqualTo("Neutral Good"));
        }

        [Test]
        public void TrueNeutralConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.TrueNeutral, Is.EqualTo("True Neutral"));
        }

        [Test]
        public void NeutralEvilConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.NeutralEvil, Is.EqualTo("Neutral Evil"));
        }

        [Test]
        public void ChaoticGoodConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.ChaoticGood, Is.EqualTo("Chaotic Good"));
        }

        [Test]
        public void ChaoticNeutralConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.ChaoticNeutral, Is.EqualTo("Chaotic Neutral"));
        }

        [Test]
        public void ChaoticEvilConstant()
        {
            Assert.That(IntelligenceAlignmentConstants.ChaoticEvil, Is.EqualTo("Chaotic Evil"));
        }
    }
}