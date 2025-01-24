using DnDGen.Infrastructure.Helpers;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class AbilityDamagesTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.Set.AbilityDamages;

        private Dictionary<string, List<string>> abilityDamages;

        [SetUp]
        public void Setup()
        {
            abilityDamages = GetAbilityDamages();
        }

        [Test]
        public void AllKeysArePresent()
        {
            var specialAbilities = SpecialAbilityConstants.GetAllAbilities(true);
            var expectedKeys = specialAbilities
                .Union(specialAbilities.Select(a => a + "x2"))
                .Union(specialAbilities.Select(a => a + "x3"))
                .Union(specialAbilities.Select(a => a + "x4"));
            var actualKeys = GetKeys();

            AssertCollection(actualKeys, specialAbilities);
        }

        [TestCaseSource(nameof(Abilities))]
        public void SpecialAbilityDamages(string specialAbility, params string[] damagesData)
        {
            Assert.That(abilityDamages, Contains.Key(specialAbility));
            base.AssertCollection(specialAbility, [.. abilityDamages[specialAbility]]);

            var criticalMultipliers = new[] { "x2", "x3", "x4" };

            foreach (var criticalMultiplier in criticalMultipliers)
            {
                var key = specialAbility + criticalMultiplier;
                Assert.That(abilityDamages, Contains.Key(key));
                base.AssertCollection(key, [.. abilityDamages[key]]);
            }
        }

        [TestCase(SpecialAbilityConstants.Aberrationbane,
            "2d6##Against Aberrations",
            "2d6##Against Aberrations",
            "2d6##Against Aberrations",
            "2d6##Against Aberrations")]
        [TestCase(SpecialAbilityConstants.AcidResistance)]
        [TestCase(SpecialAbilityConstants.AirOutsiderbane,
            "2d6##Against Air Outsiders",
            "2d6##Against Air Outsiders",
            "2d6##Against Air Outsiders",
            "2d6##Against Air Outsiders")]
        [TestCase(SpecialAbilityConstants.Anarchic,
            "2d6##Against Lawful alignment",
            "2d6##Against Lawful alignment",
            "2d6##Against Lawful alignment",
            "2d6##Against Lawful alignment")]
        [TestCase(SpecialAbilityConstants.Animalbane,
            "2d6##Against Animals",
            "2d6##Against Animals",
            "2d6##Against Animals",
            "2d6##Against Animals")]
        [TestCase(SpecialAbilityConstants.Animated)]
        [TestCase(SpecialAbilityConstants.AquaticHumanoidbane,
            "2d6##Against Aquatic Humanoids",
            "2d6##Against Aquatic Humanoids",
            "2d6##Against Aquatic Humanoids",
            "2d6##Against Aquatic Humanoids")]
        [TestCase(SpecialAbilityConstants.ArrowCatching)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection)]
        [TestCase(SpecialAbilityConstants.Axiomatic,
            "2d6##Against Chaotic alignment",
            "2d6##Against Chaotic alignment",
            "2d6##Against Chaotic alignment",
            "2d6##Against Chaotic alignment")]
        [TestCase(SpecialAbilityConstants.Bane,
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE")]
        [TestCase(SpecialAbilityConstants.Bashing)]
        [TestCase(SpecialAbilityConstants.Blinding)]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy)]
        [TestCase(SpecialAbilityConstants.ChaoticOutsiderbane,
            "2d6##Against Chaotic Outsiders",
            "2d6##Against Chaotic Outsiders",
            "2d6##Against Chaotic Outsiders",
            "2d6##Against Chaotic Outsiders")]
        [TestCase(SpecialAbilityConstants.ColdResistance)]
        [TestCase(SpecialAbilityConstants.Constructbane,
            "2d6##Against Constructs",
            "2d6##Against Constructs",
            "2d6##Against Constructs",
            "2d6##Against Constructs")]
        [TestCase(SpecialAbilityConstants.Dancing)]
        [TestCase(SpecialAbilityConstants.Defending)]
        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane,
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE",
            "2d6##Against DESIGNATEDFOE")]
        [TestCase(SpecialAbilityConstants.Disruption)]
        [TestCase(SpecialAbilityConstants.Distance)]
        [TestCase(SpecialAbilityConstants.Dragonbane,
            "2d6##Against Dragons",
            "2d6##Against Dragons",
            "2d6##Against Dragons",
            "2d6##Against Dragons")]
        [TestCase(SpecialAbilityConstants.Dwarfbane,
            "2d6##Against Dwarfs",
            "2d6##Against Dwarfs",
            "2d6##Against Dwarfs",
            "2d6##Against Dwarfs")]
        [TestCase(SpecialAbilityConstants.EarthOutsiderbane,
            "2d6##Against Earth Outsiders",
            "2d6##Against Earth Outsiders",
            "2d6##Against Earth Outsiders",
            "2d6##Against Earth Outsiders")]
        [TestCase(SpecialAbilityConstants.ElectricityResistance)]
        [TestCase(SpecialAbilityConstants.Elementalbane,
            "2d6##Against Elementals",
            "2d6##Against Elementals",
            "2d6##Against Elementals",
            "2d6##Against Elementals")]
        [TestCase(SpecialAbilityConstants.Elfbane,
            "2d6##Against Elfs",
            "2d6##Against Elfs",
            "2d6##Against Elfs",
            "2d6##Against Elfs")]
        [TestCase(SpecialAbilityConstants.Etherealness)]
        [TestCase(SpecialAbilityConstants.EvilOutsiderbane,
            "2d6##Against Evil Outsiders",
            "2d6##Against Evil Outsiders",
            "2d6##Against Evil Outsiders",
            "2d6##Against Evil Outsiders")]
        [TestCase(SpecialAbilityConstants.Feybane,
            "2d6##Against Feys",
            "2d6##Against Feys",
            "2d6##Against Feys",
            "2d6##Against Feys")]
        [TestCase(SpecialAbilityConstants.FireOutsiderbane,
            "2d6##Against Fire Outsiders",
            "2d6##Against Fire Outsiders",
            "2d6##Against Fire Outsiders",
            "2d6##Against Fire Outsiders")]
        [TestCase(SpecialAbilityConstants.FireResistance)]
        [TestCase(SpecialAbilityConstants.Flaming,
            "1d6#Fire#",
            "1d6#Fire#",
            "1d6#Fire#",
            "1d6#Fire#")]
        [TestCase(SpecialAbilityConstants.FlamingBurst,
            "1d6#Fire#",
            "1d6#Fire#,1d10#Fire#",
            "1d6#Fire#,2d10#Fire#",
            "1d6#Fire#,3d10#Fire#")]
        [TestCase(SpecialAbilityConstants.Fortification)]
        [TestCase(SpecialAbilityConstants.Frost,
            "1d6#Cold#",
            "1d6#Cold#",
            "1d6#Cold#",
            "1d6#Cold#")]
        [TestCase(SpecialAbilityConstants.GhostTouch)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon)]
        [TestCase(SpecialAbilityConstants.Giantbane,
            "2d6##Against Giants",
            "2d6##Against Giants",
            "2d6##Against Giants",
            "2d6##Against Giants")]
        [TestCase(SpecialAbilityConstants.Glamered)]
        [TestCase(SpecialAbilityConstants.Gnollbane, "2d6##Against Gnolls", "2d6##Against Gnolls", "2d6##Against Gnolls", "2d6##Against Gnolls")]
        [TestCase(SpecialAbilityConstants.Gnomebane, "2d6##Against Gnomes", "2d6##Against Gnomes", "2d6##Against Gnomes", "2d6##Against Gnomes")]
        [TestCase(SpecialAbilityConstants.Goblinoidbane, "2d6##Against Goblinoids", "2d6##Against Goblinoids", "2d6##Against Goblinoids", "2d6##Against Goblinoids")]
        [TestCase(SpecialAbilityConstants.GoodOutsiderbane, "2d6##Against Good Outsiders", "2d6##Against Good Outsiders", "2d6##Against Good Outsiders", "2d6##Against Good Outsiders")]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance)]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance)]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance)]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance)]
        [TestCase(SpecialAbilityConstants.GreaterShadow)]
        [TestCase(SpecialAbilityConstants.GreaterSilentMoves)]
        [TestCase(SpecialAbilityConstants.GreaterSlick)]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance)]
        [TestCase(SpecialAbilityConstants.Halflingbane, "2d6##Against Halflings", "2d6##Against Halflings", "2d6##Against Halflings", "2d6##Against Halflings")]
        [TestCase(SpecialAbilityConstants.HeavyFortification)]
        [TestCase(SpecialAbilityConstants.Holy, "2d6##Against Evil alignment", "2d6##Against Evil alignment", "2d6##Against Evil alignment", "2d6##Against Evil alignment")]
        [TestCase(SpecialAbilityConstants.Humanbane, "2d6##Against Humans", "2d6##Against Humans", "2d6##Against Humans", "2d6##Against Humans")]
        [TestCase(SpecialAbilityConstants.IcyBurst, "1d6#Cold#", "1d6#Cold#,1d10#Cold#", "1d6#Cold#,2d10#Cold#", "1d6#Cold#,3d10#Cold#")]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves)]
        [TestCase(SpecialAbilityConstants.ImprovedSlick)]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance)]
        [TestCase(SpecialAbilityConstants.Invulnerability)]
        [TestCase(SpecialAbilityConstants.Keen)]
        [TestCase(SpecialAbilityConstants.KiFocus)]
        [TestCase(SpecialAbilityConstants.LawfulOutsiderbane, "2d6##Against Lawful Outsiders", "2d6##Against Lawful Outsiders", "2d6##Against Lawful Outsiders", "2d6##Against Lawful Outsiders")]
        [TestCase(SpecialAbilityConstants.LightFortification)]
        [TestCase(SpecialAbilityConstants.MagicalBeastbane, "2d6##Against Magical Beasts", "2d6##Against Magical Beasts", "2d6##Against Magical Beasts", "2d6##Against Magical Beasts")]
        [TestCase(SpecialAbilityConstants.Merciful, "1d6##", "1d6##", "1d6##", "1d6##")]
        [TestCase(SpecialAbilityConstants.MightyCleaving)]
        [TestCase(SpecialAbilityConstants.ModerateFortification)]
        [TestCase(SpecialAbilityConstants.MonstrousHumanoidbane, "2d6##Against Monstrous Humanoids", "2d6##Against Monstrous Humanoids", "2d6##Against Monstrous Humanoids", "2d6##Against Monstrous Humanoids")]
        [TestCase(SpecialAbilityConstants.Oozebane, "2d6##Against Oozes", "2d6##Against Oozes", "2d6##Against Oozes", "2d6##Against Oozes")]
        [TestCase(SpecialAbilityConstants.Orcbane, "2d6##Against Orcs", "2d6##Against Orcs", "2d6##Against Orcs", "2d6##Against Orcs")]
        [TestCase(SpecialAbilityConstants.Plantbane, "2d6##Against Plants", "2d6##Against Plants", "2d6##Against Plants", "2d6##Against Plants")]
        [TestCase(SpecialAbilityConstants.Reflecting)]
        [TestCase(SpecialAbilityConstants.ReptilianHumanoidbane, "2d6##Against Reptilian Humanoids", "2d6##Against Reptilian Humanoids", "2d6##Against Reptilian Humanoids", "2d6##Against Reptilian Humanoids")]
        [TestCase(SpecialAbilityConstants.Returning)]
        [TestCase(SpecialAbilityConstants.Seeking)]
        [TestCase(SpecialAbilityConstants.Shadow)]
        [TestCase(SpecialAbilityConstants.Shapeshifterbane, "2d6##Against Shapeshifters", "2d6##Against Shapeshifters", "2d6##Against Shapeshifters", "2d6##Against Shapeshifters")]
        [TestCase(SpecialAbilityConstants.Shock, "1d6#Electricity#", "1d6#Electricity#", "1d6#Electricity#", "1d6#Electricity#")]
        [TestCase(SpecialAbilityConstants.ShockingBurst, "1d6#Electricity#", "1d6#Electricity#,1d10#Electricity#", "1d6#Electricity#,2d10#Electricity#", "1d6#Electricity#,3d10#Electricity#")]
        [TestCase(SpecialAbilityConstants.SilentMoves)]
        [TestCase(SpecialAbilityConstants.Slick)]
        [TestCase(SpecialAbilityConstants.SonicResistance)]
        [TestCase(SpecialAbilityConstants.Speed)]
        [TestCase(SpecialAbilityConstants.SpellResistance)]
        [TestCase(SpecialAbilityConstants.SpellResistance13)]
        [TestCase(SpecialAbilityConstants.SpellResistance15)]
        [TestCase(SpecialAbilityConstants.SpellResistance17)]
        [TestCase(SpecialAbilityConstants.SpellResistance19)]
        [TestCase(SpecialAbilityConstants.SpellStoring)]
        [TestCase(SpecialAbilityConstants.Throwing)]
        [TestCase(SpecialAbilityConstants.Thundering, "", "1d8#Sonic#", "2d8#Sonic#", "3d8#Sonic#")]
        [TestCase(SpecialAbilityConstants.Undeadbane, "2d6##Against Undead", "2d6##Against Undead", "2d6##Against Undead", "2d6##Against Undead")]
        [TestCase(SpecialAbilityConstants.UndeadControlling)]
        [TestCase(SpecialAbilityConstants.Unholy, "2d6##Against Good alignment", "2d6##Against Good alignment", "2d6##Against Good alignment", "2d6##Against Good alignment")]
        [TestCase(SpecialAbilityConstants.Verminbane, "2d6##Against Vermin", "2d6##Against Vermin", "2d6##Against Vermin", "2d6##Against Vermin")]
        [TestCase(SpecialAbilityConstants.Vicious, "2d6##,1d6##To the wielder", "2d6##,1d6##To the wielder", "2d6##,1d6##To the wielder", "2d6##,1d6##To the wielder")]
        [TestCase(SpecialAbilityConstants.Vorpal)]
        [TestCase(SpecialAbilityConstants.WaterOutsiderbane, "2d6##Against Water Outsiders", "2d6##Against Water Outsiders", "2d6##Against Water Outsiders", "2d6##Against Water Outsiders")]
        private Dictionary<string, List<string>> GetAbilityDamages()
        {
            var damages = new Dictionary<string, List<string>>
            {
                [SpecialAbilityConstants.Wild] = [],
                [SpecialAbilityConstants.Wild + "x2"] = [],
                [SpecialAbilityConstants.Wild + "x3"] = [],
                [SpecialAbilityConstants.Wild + "x4"] = [],
                [SpecialAbilityConstants.Wounding] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Constitution" })],
                [SpecialAbilityConstants.Wounding + "x2"] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Constitution" })],
                [SpecialAbilityConstants.Wounding + "x3"] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Constitution" })],
                [SpecialAbilityConstants.Wounding + "x4"] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Constitution" })],
            };

            return damages;
        }

        public static IEnumerable Abilities => SpecialAbilityConstants.GetAllAbilities(true).Select(a => new TestCaseData(a));
    }
}
