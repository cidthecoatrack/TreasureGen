﻿using System;
using System.Linq;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using Ninject;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class MagicalArmorGeneratorTests : MagicalItemGeneratorStressTests
    {
        [Inject, Named(ItemTypeConstants.Armor)]
        public IMagicalItemGenerator MagicalArmorGenerator { get; set; }

        [TestCase("Magical Armor generator")]
        public override void Stress(String thingToStress)
        {
            Stress();
        }

        protected override String itemType
        {
            get { return ItemTypeConstants.Armor; }
        }

        protected override void MakeAssertionsAgainst(Item armor)
        {
            Assert.That(armor.Name, Is.Not.Empty);
            Assert.That(armor.Traits, Is.Not.Null);
            Assert.That(armor.Attributes, Is.Not.Empty);
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Contents, Is.Not.Null);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Magic.Charges, Is.EqualTo(0));
            Assert.That(armor.Magic.SpecialAbilities, Is.Not.Null);
        }

        protected override Item GenerateItem()
        {
            var power = GetNewPower();
            return MagicalArmorGenerator.GenerateAtPower(power);
        }

        [Test]
        public void MagicalArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && armor.IsMagical == false);

            Assert.That(armor.IsMagical, Is.True);
            Assert.That(armor.Magic.Bonus, Is.Positive);
            AssertIterations();
        }

        [Test]
        public void MundaneArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && armor.IsMagical);

            Assert.That(armor.IsMagical, Is.False);
            Assert.That(armor.Magic.Bonus, Is.EqualTo(0));
            AssertIterations();
        }

        [Test]
        public void SpecialAbilitiesHappen()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && !armor.Magic.SpecialAbilities.Any());

            Assert.That(armor.Magic.SpecialAbilities, Is.Not.Empty);
            AssertIterations();
        }

        [Test]
        public void SpecialAbilitiesDoNotHappen()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && armor.Magic.SpecialAbilities.Any());

            Assert.That(armor.Magic.SpecialAbilities, Is.Empty);
            AssertIterations();
        }

        [Test]
        public void ContentsHappen()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && !armor.Contents.Any());

            Assert.That(armor.Contents, Is.Not.Empty);
            AssertIterations();
        }

        [Test]
        public void ContentsDoNotHappen()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && armor.Contents.Any());

            Assert.That(armor.Contents, Is.Empty);
            AssertIterations();
        }

        [Test]
        public void SpecificArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && !armor.Attributes.Contains(AttributeConstants.Specific));

            Assert.That(armor.Attributes, Contains.Item(AttributeConstants.Specific));
            AssertIterations();
        }

        [Test]
        public void UndecoratedSpecificArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && (!armor.Attributes.Contains(AttributeConstants.Specific) || armor.Magic.Intelligence.Ego > 0 || !String.IsNullOrEmpty(armor.Magic.Curse)));

            Assert.That(armor.Attributes, Contains.Item(AttributeConstants.Specific));
            Assert.That(armor.Magic.Curse, Is.Empty);
            Assert.That(armor.Magic.Intelligence.Ego, Is.EqualTo(0));
            AssertIterations();
        }

        [Test]
        public void IntelligentSpecificArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && (!armor.Attributes.Contains(AttributeConstants.Specific) || armor.Magic.Intelligence.Ego == 0));

            Assert.That(armor.Attributes, Contains.Item(AttributeConstants.Specific));
            Assert.That(armor.Magic.Intelligence.Ego, Is.Positive);
            AssertIterations();
        }

        [Test]
        public void CursedSpecificArmorHappens()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && (!armor.Attributes.Contains(AttributeConstants.Specific) || String.IsNullOrEmpty(armor.Magic.Curse)));

            Assert.That(armor.Attributes, Contains.Item(AttributeConstants.Specific));
            Assert.That(armor.Magic.Curse, Is.Not.Empty);
            AssertIterations();
        }

        [Test]
        public void SpecificArmorDoesNotHappen()
        {
            Item armor;

            do armor = GenerateItem();
            while (TestShouldKeepRunning() && armor.Attributes.Contains(AttributeConstants.Specific));

            Assert.That(armor.Attributes, Is.Not.Contains(AttributeConstants.Specific));
            AssertIterations();
        }

        [Test]
        public override void IntelligenceHappens()
        {
            base.IntelligenceHappens();
        }

        [Test]
        public override void TraitsHappen()
        {
            base.TraitsHappen();
        }

        [Test]
        public override void SpecialMaterialsHappen()
        {
            base.SpecialMaterialsHappen();
        }

        [Test]
        public override void CursesHappen()
        {
            AssertCursesHappen();
        }

        [Test]
        public override void SpecificCursesHappen()
        {
            AssertSpecificCursesHappen();
        }

        [Test]
        public override void NoDecorationsHappen()
        {
            AssertNoDecorationsHappen();
        }

        [Test]
        public override void SpecificCursedItemsAreIntelligent()
        {
            AssertSpecificCursedItemsAreIntelligent();
        }

        [Test]
        public override void SpecificCursedItemsHaveTraits()
        {
            AssertSpecificCursedItemsHaveTraits();
        }

        [Test]
        public override void SpecificCursedItemsHaveSpecialMaterials()
        {
            AssertSpecificCursedItemsHaveSpecialMaterials();
        }

        [Test]
        public override void SpecificCursedItemsAreNotDecorated()
        {
            AssertSpecificCursedItemsAreNotDecorated();
        }
    }
}