﻿using System;
using EquipmentGen.Common.Items;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Common.Items
{
    [TestFixture]
    public class SpecialAbilityConstantsTests
    {
        [TestCase(SpecialAbilityConstants.Glamered, "Glamered")]
        [TestCase(SpecialAbilityConstants.LightFortification, "Light fortification")]
        [TestCase(SpecialAbilityConstants.Slick, "Slick")]
        [TestCase(SpecialAbilityConstants.Shadow, "Shadow")]
        [TestCase(SpecialAbilityConstants.SilentMoves, "Silent moves")]
        [TestCase(SpecialAbilityConstants.SpellResistance13, "Spell resistance (13)")]
        [TestCase(SpecialAbilityConstants.ImprovedSlick, "Improved slick")]
        [TestCase(SpecialAbilityConstants.ImprovedShadow, "Improved shadow")]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves, "Improved silent moves")]
        [TestCase(SpecialAbilityConstants.AcidResistance, "Acid resistance")]
        [TestCase(SpecialAbilityConstants.ColdResistance, "Cold resistance")]
        [TestCase(SpecialAbilityConstants.ElectricityResistance, "Electricity resistance")]
        [TestCase(SpecialAbilityConstants.FireResistance, "Fire resistance")]
        [TestCase(SpecialAbilityConstants.SonicResistance, "Sonic resistance")]
        [TestCase(SpecialAbilityConstants.GhostTouch, "Ghost touch")]
        [TestCase(SpecialAbilityConstants.Invulnerability, "Invulnerability")]
        [TestCase(SpecialAbilityConstants.ModerateFortification, "Moderate fortification")]
        [TestCase(SpecialAbilityConstants.Fortification, "Fortification")]
        [TestCase(SpecialAbilityConstants.SpellResistance, "Spell resistance")]
        [TestCase(SpecialAbilityConstants.SpellResistance15, "Spell resistance (15)")]
        [TestCase(SpecialAbilityConstants.Wild, "Wild")]
        [TestCase(SpecialAbilityConstants.GreaterSlick, "Greater slick")]
        [TestCase(SpecialAbilityConstants.GreaterShadow, "Greater shadow")]
        [TestCase(SpecialAbilityConstants.GreaterSilentMoves, "Greater silent moves")]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance, "Improved acid resistance")]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance, "Improved cold resistance")]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance, "Improved electricity resistance")]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance, "Improved fire resistance")]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance, "Improved sonic resistance")]
        [TestCase(SpecialAbilityConstants.SpellResistance17, "Spell resistance (17)")]
        [TestCase(SpecialAbilityConstants.Etherealness, "Etherealness")]
        [TestCase(SpecialAbilityConstants.UndeadControlling, "Undead controlling")]
        [TestCase(SpecialAbilityConstants.HeavyFortification, "Heavy fortification")]
        [TestCase(SpecialAbilityConstants.SpellResistance19, "Spell resistance (19)")]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance, "Greater acid resistance")]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance, "Greater cold resistance")]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance, "Greater electricity resistance")]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance, "Greater fire resistance")]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance, "Greater sonic resistance")]
        [TestCase(SpecialAbilityConstants.ArrowCatching, "Arrow catching")]
        [TestCase(SpecialAbilityConstants.Bashing, "Bashing")]
        [TestCase(SpecialAbilityConstants.Blinding, "Blinding")]
        [TestCase(SpecialAbilityConstants.ArrowDeflection, "Arrow deflection")]
        [TestCase(SpecialAbilityConstants.Animated, "Animated")]
        [TestCase(SpecialAbilityConstants.Reflecting, "Reflecting")]
        [TestCase(SpecialAbilityConstants.Bane, "Bane")]
        [TestCase(SpecialAbilityConstants.Distance, "Distance")]
        [TestCase(SpecialAbilityConstants.Flaming, "Flaming")]
        [TestCase(SpecialAbilityConstants.Frost, "Frost")]
        [TestCase(SpecialAbilityConstants.Merciful, "Merciful")]
        [TestCase(SpecialAbilityConstants.Returning, "Returning")]
        [TestCase(SpecialAbilityConstants.Shock, "Shock")]
        [TestCase(SpecialAbilityConstants.Seeking, "Seeking")]
        [TestCase(SpecialAbilityConstants.Thundering, "Thundering")]
        [TestCase(SpecialAbilityConstants.Anarchic, "Anarchic")]
        [TestCase(SpecialAbilityConstants.Axiomatic, "Axiomatic")]
        [TestCase(SpecialAbilityConstants.Disruption, "Disruption")]
        [TestCase(SpecialAbilityConstants.FlamingBurst, "Flaming burst")]
        [TestCase(SpecialAbilityConstants.IcyBurst, "Icy burst")]
        [TestCase(SpecialAbilityConstants.Holy, "Holy")]
        [TestCase(SpecialAbilityConstants.ShockingBurst, "Shocking burst")]
        [TestCase(SpecialAbilityConstants.Unholy, "Unholy")]
        [TestCase(SpecialAbilityConstants.Wounding, "Wounding")]
        [TestCase(SpecialAbilityConstants.Speed, "Speed")]
        [TestCase(SpecialAbilityConstants.Dancing, "Dancing")]
        [TestCase(SpecialAbilityConstants.Vorpal, "Vorpal")]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy, "Brilliant energy")]
        [TestCase(SpecialAbilityConstants.Defending, "Defending")]
        [TestCase(SpecialAbilityConstants.Keen, "Keen")]
        [TestCase(SpecialAbilityConstants.KiFocus, "Ki focus")]
        [TestCase(SpecialAbilityConstants.Throwing, "Throwing")]
        [TestCase(SpecialAbilityConstants.MightyCleaving, "Mighty cleaving")]
        [TestCase(SpecialAbilityConstants.SpellStoring, "Spell storing")]
        [TestCase(SpecialAbilityConstants.Vicious, "Vicious")]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, "Ghost touch (weapon)")]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor, "Ghost touch (armor)")]
        public void Constant(String constant, String value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}