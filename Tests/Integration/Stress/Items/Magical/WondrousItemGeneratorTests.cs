﻿using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class WondrousItemGeneratorTests : StressTests
    {
        [Inject, Named(ItemTypeConstants.WondrousItem)]
        public IMagicalItemGenerator WondrousItemGenerator { get; set; }

        private IEnumerable<String> materials;

        [SetUp]
        public void Setup()
        {
            materials = TraitConstants.GetSpecialMaterials();
        }

        [Test]
        public void StressedWondrousItemGenerator()
        {
            StressGenerator();
        }

        protected override void MakeAssertions()
        {
            var power = GetNewPower(false);
            var item = WondrousItemGenerator.GenerateAtPower(power);

            if (item.ItemType == ItemTypeConstants.SpecificCursedItem)
                return;

            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.Traits, Is.Not.Null);
            Assert.That(item.Attributes, Is.Not.Null);
            Assert.That(item.Quantity, Is.EqualTo(1));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Contents, Is.Not.Null);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Magic.Bonus, Is.AtLeast(0));
            Assert.That(item.Magic.Charges, Is.AtLeast(0));
            Assert.That(item.Magic.SpecialAbilities, Is.Empty);

            if (item.Attributes.Contains(AttributeConstants.Charged))
                Assert.That(item.Magic.Charges, Is.GreaterThan(0));

            var itemMaterials = item.Traits.Intersect(materials);
            Assert.That(itemMaterials, Is.Empty);
        }

        [Test]
        public void IntelligenceHappens()
        {
            var item = new Item();

            while (Stopwatch.Elapsed.Seconds < TimeLimitInSeconds && item.Magic.Intelligence.Ego == 0)
            {
                var power = GetNewPower(false);
                item = WondrousItemGenerator.GenerateAtPower(power);
            }

            Assert.That(item.Magic.Intelligence.Ego, Is.GreaterThan(0));
            Assert.Pass("Milliseconds: {0}", Stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void CursesHappen()
        {
            var item = new Item();

            while (Stopwatch.Elapsed.Seconds < TimeLimitInSeconds && (String.IsNullOrEmpty(item.Magic.Curse) || item.ItemType == ItemTypeConstants.SpecificCursedItem))
            {
                var power = GetNewPower(false);
                item = WondrousItemGenerator.GenerateAtPower(power);
            }

            Assert.That(item.ItemType, Is.Not.EqualTo(ItemTypeConstants.SpecificCursedItem));
            Assert.That(item.Magic.Curse, Is.Not.Empty);
            Assert.Pass("Milliseconds: {0}", Stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void SpecificCursesHappen()
        {
            var item = new Item();

            while (Stopwatch.Elapsed.Seconds < TimeLimitInSeconds && item.ItemType != ItemTypeConstants.SpecificCursedItem)
            {
                var power = GetNewPower(false);
                item = WondrousItemGenerator.GenerateAtPower(power);
            }

            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.SpecificCursedItem));
            Assert.Pass("Milliseconds: {0}", Stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TraitsHappen()
        {
            var item = new Item();

            do
            {
                var power = GetNewPower(false);
                item = WondrousItemGenerator.GenerateAtPower(power);
            } while (Stopwatch.Elapsed.Seconds < TimeLimitInSeconds && !item.Traits.Any());

            Assert.That(item.Traits, Is.Not.Empty);
            Assert.Pass("Milliseconds: {0}", Stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void NoDecorationsHappen()
        {
            var item = new Item();

            do
            {
                var power = GetNewPower(false);
                item = WondrousItemGenerator.GenerateAtPower(power);
            } while (Stopwatch.Elapsed.Seconds < TimeLimitInSeconds && item.Traits.Any() && item.Magic.Curse.Any() && item.Magic.Intelligence.Ego > 0);

            Assert.That(item.Traits, Is.Empty);
            Assert.That(item.Magic.Curse, Is.Empty);
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.Pass("Milliseconds: {0}", Stopwatch.ElapsedMilliseconds);
        }
    }
}