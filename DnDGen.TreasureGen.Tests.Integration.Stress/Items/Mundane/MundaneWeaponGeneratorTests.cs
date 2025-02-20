using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items.Mundane
{
    [TestFixture]
    public class MundaneWeaponGeneratorTests : MundaneItemGeneratorStressTests
    {
        [SetUp]
        public void Setup()
        {
            mundaneItemGenerator = GetNewInstanceOf<MundaneItemGenerator>(ItemTypeConstants.Weapon);
        }

        [Test]
        public void StressWeapon()
        {
            stressor.Stress(GenerateAndAssertItem);
        }

        protected override void MakeSpecificAssertionsAgainst(Item item)
        {
            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), item.Name);
            Assert.That(item.Quantity, Is.Positive, item.Name);
            Assert.That(item.IsMagical, Is.False, item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Simple)
                .Or.Contains(AttributeConstants.Martial)
                .Or.Contains(AttributeConstants.Exotic), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Light)
                .Or.Contains(AttributeConstants.OneHanded)
                .Or.Contains(AttributeConstants.TwoHanded)
                .Or.Contains(AttributeConstants.Ranged), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Melee)
                .Or.Contains(AttributeConstants.Ranged), item.Name);

            var sizes = TraitConstants.Sizes.GetAll();
            Assert.That(item.Traits.Intersect(sizes), Is.Empty);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.Not.Empty);
            Assert.That(sizes, Contains.Item(weapon.Size), item.Name);
        }

        protected override IEnumerable<string> GetItemNames()
        {
            return WeaponConstants.GetAllWeapons(false, false);
        }

        [Test]
        public void BUG_AmmunitionWithQuantityGreaterThan1Happens()
        {
            var ammunition = stressor.GenerateOrFail(GenerateItem, w => w.ItemType == ItemTypeConstants.Weapon && w.Attributes.Contains(AttributeConstants.Ammunition) && w.Quantity > 1);
            AssertItem(ammunition);
            Assert.That(ammunition.Attributes, Contains.Item(AttributeConstants.Ammunition), ammunition.Name);
            Assert.That(ammunition.Quantity, Is.InRange(2, 50), ammunition.Name);
        }

        [Test]
        public void BUG_ThrownRangedWeaponWithQuantityGreaterThan1Happens()
        {
            var thrownWeapon = stressor.GenerateOrFail(GenerateItem, w => w.ItemType == ItemTypeConstants.Weapon && w.Attributes.Contains(AttributeConstants.Thrown) && !w.Attributes.Contains(AttributeConstants.Melee) && w.Quantity > 1);
            AssertItem(thrownWeapon);
            Assert.That(thrownWeapon.Attributes, Contains.Item(AttributeConstants.Thrown)
                .And.Contains(AttributeConstants.Ranged)
                .And.Not.Contains(AttributeConstants.Melee), thrownWeapon.Name);

            var topRange = thrownWeapon.NameMatches(WeaponConstants.Shuriken) ? 50 : 20;
            Assert.That(thrownWeapon.Quantity, Is.InRange(2, topRange), thrownWeapon.Name);
        }

        [Test]
        public void StressCustomWeapon()
        {
            stressor.Stress(GenerateAndAssertCustomItem);
        }

        [Test]
        public void StressMundaneWeaponFromName()
        {
            stressor.Stress(GenerateAndAssertItemFromName);
        }

        [Test]
        public void BUG_ShurikenWithQuantityGreaterThan20Happens()
        {
            var shuriken = stressor.GenerateOrFail(GenerateShuriken, w => w.NameMatches(WeaponConstants.Shuriken) && w.Quantity > 20);
            AssertItem(shuriken);
            Assert.That(shuriken.NameMatches(WeaponConstants.Shuriken), Is.True);
            Assert.That(shuriken.Attributes, Contains.Item(AttributeConstants.Thrown)
                .And.Contains(AttributeConstants.Ranged)
                .And.Contains(AttributeConstants.Ammunition)
                .And.Not.Contains(AttributeConstants.Melee), shuriken.Name);
            Assert.That(shuriken.Quantity, Is.InRange(21, 50), shuriken.Name);
        }

        private Item GenerateShuriken()
        {
            var shuriken = mundaneItemGenerator.Generate(WeaponConstants.Shuriken);

            return shuriken;
        }

        //INFO: This would include weapons that are mostly melee, but can be thrown, such as daggers
        [Test]
        public void BUG_ThrownMeleeWeaponDoesNotGetQuantityGreaterThan1()
        {
            stressor.Stress(GenerateAndAssertThrownMeleeWeapon);
        }

        private void GenerateAndAssertThrownMeleeWeapon()
        {
            var thrownWeapon = GenerateItem(w => w.ItemType == ItemTypeConstants.Weapon && w.Attributes.Contains(AttributeConstants.Thrown) && w.Attributes.Contains(AttributeConstants.Melee));

            AssertItem(thrownWeapon);
            Assert.That(thrownWeapon.Attributes, Contains.Item(AttributeConstants.Thrown)
                .And.Contains(AttributeConstants.Ranged)
                .And.Contains(AttributeConstants.Melee), thrownWeapon.Name);
            Assert.That(thrownWeapon.Quantity, Is.EqualTo(1), thrownWeapon.Name);
        }
    }
}