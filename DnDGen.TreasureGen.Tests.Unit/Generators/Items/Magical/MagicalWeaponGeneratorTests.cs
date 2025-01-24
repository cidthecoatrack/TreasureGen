using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalWeaponGeneratorTests
    {
        private MagicalItemGenerator magicalWeaponGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<ISpecificGearGenerator> mockSpecificGearGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private string power;
        private ItemVerifier itemVerifier;
        private Weapon mundaneWeapon;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            magicalWeaponGenerator = new MagicalWeaponGenerator(
                mockCollectionsSelector.Object,
                mockPercentileSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockSpecificGearGenerator.Object,
                mockSpellGenerator.Object,
                mockJustInTimeFactory.Object);

            itemVerifier = new ItemVerifier();

            power = "power";
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.MagicalWeaponTypes)).Returns("weapon type");
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "weapon type");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            mundaneWeapon = new Weapon();
            mundaneWeapon.Name = "weapon name";
            mundaneWeapon.Quantity = 600;
            mundaneWeapon.Ammunition = "ammo";
            mundaneWeapon.CriticalMultiplier = "crit";
            mundaneWeapon.Damages.Add(new Damage { Roll = "hurty mchurtface", Type = "spiritual" });
            mundaneWeapon.CriticalDamages.Add(new Damage { Roll = "hurty mcSUPERhurtface", Type = "spiritual" });
            mundaneWeapon.Size = "enormous";
            mundaneWeapon.ThreatRange = 96;
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.IsAny<string>())).Returns(new Weapon());
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.IsAny<Item>(), It.IsAny<bool>())).Returns(new Weapon());
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("weapon name")), true)).Returns(mundaneWeapon);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("9266");
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(new[] { "9266", "90210", "42", "SpecialAbility", ItemTypeConstants.Weapon });
        }

        [Test]
        public void GenerateWeapon()
        {
            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GenerateWeapon_DoubleWeapon_OtherHeadHasSameEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.EqualTo("other crit"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(9266));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GenerateWeapon_DoubleWeapon_OtherHeadHasLesserEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(false);

            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.EqualTo("other crit"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(9265));
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GetSpecificWeaponFromGenerator()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = Guid.NewGuid().ToString();
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "weapon name")).Returns(specificWeapon.Name);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, specificWeapon.Name)).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void BUG_GetSpecificWeaponFromGenerator_HasQuantity()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = Guid.NewGuid().ToString();
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "weapon name")).Returns(specificWeapon.Name);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, specificWeapon.Name)).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name 1";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages_EmptyDamageType()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = string.Empty, Condition = "my other condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(mundaneWeapon.Damages[0].Type));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo(mundaneWeapon.Damages[0].Type));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_CriticalDamages()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_CriticalDamages_EmptyDamageType()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = string.Empty });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = string.Empty });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = string.Empty });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = string.Empty });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(ability.CriticalDamages["crit"][0].Type, Is.Not.Empty.And.EqualTo(mundaneWeapon.CriticalDamages[0].Type));
            Assert.That(ability.CriticalDamages["crit"][1].Type, Is.Not.Empty.And.EqualTo(mundaneWeapon.CriticalDamages[0].Type));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_DamagesAndCriticalDamages()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_DamagesAndCriticalDamages_EmptyDamageTypes()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = string.Empty });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = string.Empty });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(mundaneWeapon.Damages[0].Type));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(ability.CriticalDamages["crit"][0].Type, Is.Not.Empty.And.EqualTo(mundaneWeapon.CriticalDamages[0].Type));
            Assert.That(ability.CriticalDamages["crit"][1].Type, Is.Not.Empty.And.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages_DoubleWeapon_SameEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.SecondaryDamages));
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.SecondaryCriticalDamages));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages_DoubleWeapon_SameEnhancement_EmptyDamageTypes()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(mundaneWeapon.Damages[0].Type));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.SecondaryDamages));
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo(mundaneWeapon.SecondaryDamages[0].Type));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1)
                .And.SupersetOf(mundaneWeapon.SecondaryCriticalDamages));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_CriticalDamages_DoubleWeapon_SameEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["other crit"]));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_DamagesAndCriticalDamages_DoubleWeapon_SameEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();
            ability.CriticalDamages["wrong crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "MORE", Type = "solid", Condition = "my crit condition 2" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "a ton", Type = "gas", Condition = "my other crit condition 2" });
            ability.CriticalDamages["wrong crit"].Add(new Damage { Roll = "wrong", Type = "plasma", Condition = "my wrong condition" });
            ability.CriticalDamages["wrong crit"].Add(new Damage { Roll = "nope", Type = "ether", Condition = "my wrong condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.SecondaryDamages));
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.SecondaryCriticalDamages));
            Assert.That(weapon.SecondaryCriticalDamages[1].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Type, Is.EqualTo("solid"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Type, Is.EqualTo("gas"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Condition, Is.EqualTo("my other crit condition 2"));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages_DoubleWeapon_LesserEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(false);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_CriticalDamages_DoubleWeapon_LesserEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(false);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_DamagesAndCriticalDamages_DoubleWeapon_LesserEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(false);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages["crit"] = new List<Damage>();
            ability.CriticalDamages["other crit"] = new List<Damage>();

            ability.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma" });
            ability.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma" });
            ability.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether" });

            var abilities = new[] { new SpecialAbility(), ability };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3)
                .And.SupersetOf(ability.CriticalDamages["crit"]));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator_Damages_MultipleAbilities()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var ability1 = new SpecialAbility();
            ability1.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability1.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability1.CriticalDamages["crit"] = new List<Damage>();
            ability1.CriticalDamages["other crit"] = new List<Damage>();

            ability1.CriticalDamages["crit"].Add(new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" });
            ability1.CriticalDamages["crit"].Add(new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" });
            ability1.CriticalDamages["other crit"].Add(new Damage { Roll = "wrong", Type = "plasma", Condition = "my wrong condition" });
            ability1.CriticalDamages["other crit"].Add(new Damage { Roll = "nope", Type = "ether", Condition = "my wrong condition" });

            var ability2 = new SpecialAbility();
            ability2.Damages.Add(new Damage { Roll = "a touch", Type = "here", Condition = "my condition 2" });
            ability2.Damages.Add(new Damage { Roll = "another touch", Type = "there", Condition = "my other condition 2" });

            ability2.CriticalDamages["crit"] = new List<Damage>();
            ability2.CriticalDamages["other crit"] = new List<Damage>();

            ability2.CriticalDamages["crit"].Add(new Damage { Roll = "MORE", Type = "here", Condition = "my crit condition 2" });
            ability2.CriticalDamages["crit"].Add(new Damage { Roll = "a ton", Type = "there", Condition = "my other crit condition 2" });
            ability2.CriticalDamages["other crit"].Add(new Damage { Roll = "also wrong", Type = "here", Condition = "my wrong condition 2" });
            ability2.CriticalDamages["other crit"].Add(new Damage { Roll = "also nope", Type = "there", Condition = "my wrong condition 2" });

            var abilities = new[] { new SpecialAbility(), ability1, ability2 };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Damages, Has.Count.EqualTo(5)
                .And.SupersetOf(mundaneWeapon.Damages));
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.Damages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.Damages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.Damages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.Damages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.Damages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.Damages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(5)
                .And.SupersetOf(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.CriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.CriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.CriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.CriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.CriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.CriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [TestCase(1, "19-20")]
        [TestCase(2, "17-20")]
        [TestCase(3, "15-20")]
        public void GetSpecialAbilitiesFromGenerator_Keen(int originalThreatRange, string keenDescription)
        {
            mundaneWeapon.ThreatRange = originalThreatRange;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            var keen = new SpecialAbility { Name = SpecialAbilityConstants.Keen };
            var abilities = new[] { new SpecialAbility(), keen };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power) as Weapon;
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.ThreatRange, Is.EqualTo(originalThreatRange * 2));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(keenDescription));
        }

        [Test]
        public void SpellStoringWeaponHasSpellIfSelectorSaysSo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.Set.SpellStoringContainsSpell)).Returns(true);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("9266");

            var abilities = new[] { new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring } };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 1)).Returns(abilities);

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon.Contents, Contains.Item("spell"));
        }

        [Test]
        public void SpellStoringWeaponDoesNotHaveSpellIfSelectorSaysSo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.Set.SpellStoringContainsSpell)).Returns(false);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("9266");

            var abilities = new[] { new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring } };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 1)).Returns(abilities);

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon.Contents, Is.Empty);
        }

        [Test]
        public void GenerateCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = new[] { "type 1", "type 2" };

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Description), Is.EqualTo(
                random.Damages.Select(d => d.Description)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Description), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Description)));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(random.ThreatRangeDescription));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GenerateCustomWeapon_WithAbilityDamages()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = new[] { "type 1", "type 2" };

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility
                {
                    Name = specialAbilityNames.Last(),
                    Damages = new List<Damage> { new Damage { Roll = "some more", Type = "physical" } },
                    CriticalDamages = new Dictionary<string, List<Damage>>
                    {
                        { "wrong", new List<Damage>() },
                        { random.CriticalMultiplier, new List<Damage> { new Damage { Roll = "even more", Type = "chemical" } } },
                    }
                }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(item, Is.InstanceOf<Weapon>());

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Description), Is.EqualTo(
                random.Damages.Select(d => d.Description)
                .Union(abilities.SelectMany(a => a.Damages).Select(d => d.Description))));
            Assert.That(weapon.CriticalDamages.Select(d => d.Description), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Description)
                .Union(abilities.Where(a => a.CriticalDamages.Any()).SelectMany(a => a.CriticalDamages[random.CriticalMultiplier]).Select(d => d.Description))));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(random.ThreatRangeDescription));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GenerateCustomWeapon_DoubleWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = new[] { "type 1", AttributeConstants.DoubleWeapon, "type 2" };
            random.SecondaryDamages.Add(new Damage { Roll = "a touch", Type = "secondary" });
            random.SecondaryCriticalDamages.Add(new Damage { Roll = "a lot", Type = "secondary" });
            random.SecondaryCriticalMultiplier = "sevenfold";

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Description), Is.EqualTo(
                random.Damages.Select(d => d.Description)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Description), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Description)));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(random.ThreatRangeDescription));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.Positive.And.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.SecondaryDamages.Select(d => d.Description), Is.Not.Empty.And.EqualTo(
                random.SecondaryDamages.Select(d => d.Description)));
            Assert.That(weapon.SecondaryCriticalDamages.Select(d => d.Description), Is.Not.Empty.And.EqualTo(
                random.SecondaryCriticalDamages.Select(d => d.Description)));
        }

        [Test]
        public void GenerateCustomWeapon_DoubleWeapon_WithAbilityDamages()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = new[] { "type 1", AttributeConstants.DoubleWeapon, "type 2" };
            random.SecondaryDamages.Add(new Damage { Roll = "a touch", Type = "secondary" });
            random.SecondaryCriticalDamages.Add(new Damage { Roll = "a lot", Type = "secondary" });
            random.SecondaryCriticalMultiplier = "sevenfold";

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility
                {
                    Name = specialAbilityNames.Last(),
                    Damages = new List<Damage> { new Damage { Roll = "some more", Type = "physical" } },
                    CriticalDamages = new Dictionary<string, List<Damage>>
                    {
                        { "wrong", new List<Damage>() },
                        { random.CriticalMultiplier, new List<Damage> { new Damage { Roll = "even more", Type = "chemical" } } },
                        { random.SecondaryCriticalMultiplier, new List<Damage> { new Damage { Roll = "a lot more", Type = "chemical" } } },
                    }
                }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Description), Is.EqualTo(
                random.Damages.Select(d => d.Description)
                .Union(abilities.SelectMany(a => a.Damages).Select(d => d.Description))));
            Assert.That(weapon.CriticalDamages.Select(d => d.Description), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Description)
                .Union(abilities.Where(a => a.CriticalDamages.Any()).SelectMany(a => a.CriticalDamages[random.CriticalMultiplier]).Select(d => d.Description))));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(random.ThreatRangeDescription));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.Positive.And.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
            Assert.That(weapon.SecondaryDamages.Select(d => d.Description), Is.Not.Empty.And.EqualTo(
                random.SecondaryDamages.Select(d => d.Description)
                .Union(abilities.SelectMany(a => a.Damages).Select(d => d.Description))));
            Assert.That(weapon.SecondaryCriticalDamages.Select(d => d.Description), Is.Not.Empty.And.EqualTo(
                random.SecondaryCriticalDamages.Select(d => d.Description)
                .Union(abilities.Where(a => a.CriticalDamages.Any()).SelectMany(a => a.CriticalDamages[random.SecondaryCriticalMultiplier]).Select(d => d.Description))));
        }

        [Test]
        public void GenerateRandomCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var templateMundaneWeapon = new Weapon();
            templateMundaneWeapon.Name = name;
            templateMundaneWeapon.Quantity = 1337;
            templateMundaneWeapon.Attributes = new[] { "type 1", "type 2" };
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), true)).Returns(templateMundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon, Is.EqualTo(templateMundaneWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1337));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(weapon.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomAmmunition()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var templateMundaneWeapon = new Weapon();
            templateMundaneWeapon.Name = name;
            templateMundaneWeapon.Quantity = 1337;
            templateMundaneWeapon.Attributes = new[] { "type 1", "type 2", AttributeConstants.Ammunition };
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template);
            Assert.That(weapon, Is.EqualTo(templateMundaneWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1337));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(weapon.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(weapon.Magic.Intelligence, Is.Not.EqualTo(template.Magic.Intelligence));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Magic.SpecialAbilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Attributes = new[] { "type 1", AttributeConstants.Specific, "type 2" };

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificWeapon.Magic.SpecialAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void GenerateSpecificCustomWeapon_WithSetAbilities()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Attributes = new[] { "type 1", AttributeConstants.Specific, "type 2" };
            var specificAbilities = specificWeapon.Magic.SpecialAbilities.ToArray();

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            var wrongAbilities = template.Magic.SpecialAbilities.Union(specificWeapon.Magic.SpecialAbilities).ToArray();
            mockSpecialAbilitiesGenerator.Setup(g => g.GenerateFor(template.Magic.SpecialAbilities)).Returns(wrongAbilities);

            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificAbilities)
                .And.Not.EquivalentTo(wrongAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void GenerateSpecificCustomWeapon_WithSetAbilities_WhenGeneratedNameIsNotSpecific()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate("mundane name");
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Attributes = new[] { "type 1", AttributeConstants.Specific, "type 2" };
            var specificAbilities = specificWeapon.Magic.SpecialAbilities.ToArray();

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "mundane name"))).Returns(false);

            var wrongAbilities = template.Magic.SpecialAbilities.Union(specificWeapon.Magic.SpecialAbilities).ToArray();
            mockSpecialAbilitiesGenerator.Setup(g => g.GenerateFor(template.Magic.SpecialAbilities)).Returns(wrongAbilities);

            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificAbilities)
                .And.Not.EquivalentTo(wrongAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void BUG_GenerateSpecificCustomWeapon_WithQuantity()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Magic.SpecialAbilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Attributes = new[] { "type 1", AttributeConstants.Specific, "type 2" };
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name 1";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificWeapon.Magic.SpecialAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GenerateFromName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(Config.Name, tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.Generate(power, "weapon name");
            Assert.That(weapon.Name, Is.EqualTo("weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.MagicalWeaponTypes))
                .Returns("weapon type");

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(Config.Name, tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("weapon name")), true))
                .Returns((Item t, bool d) => new Weapon
                {
                    Name = t.Name,
                    Traits = t.Traits,
                    BaseNames = new[] { "mundane base name" },
                });

            var weapon = magicalWeaponGenerator.Generate(power, "weapon name", "trait 1", "trait 2");
            Assert.That(weapon.Name, Is.EqualTo("weapon name"));
            Assert.That(weapon.BaseNames, Contains.Item("mundane base name"));
            Assert.That(weapon.BaseNames.Count(), Is.EqualTo(1));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificFromName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void GenerateSpecificFromName_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator
                .Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name", "trait 1", "trait 2"))
                .Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name", "trait 1", "trait 2");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void BUG_GenerateSpecificFromName_WithQuantity()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GenerateSpecificFromBaseName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void GenerateSpecificWithSpecialAbilitiesFromName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns("9266");

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.MagicalWeaponTypes)).Returns("wrong weapon type");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "wrong weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("wrong weapon name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "other weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("other weapon name");

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            specificWeapon.ItemType = ItemTypeConstants.Weapon;

            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificWithSpecialAbilitiesFromBaseName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.MagicalWeaponTypes)).Returns("wrong weapon type");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "wrong weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("wrong weapon name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "other weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("other weapon name");

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            specificWeapon.ItemType = ItemTypeConstants.Weapon;

            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void BUG_GenerateSpecificFromBaseName_WithQuantity()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns("SpecialAbility").Returns("SpecialAbility").Returns(ItemTypeConstants.Weapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }
    }
}