using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Tables
{
    [TestFixture]
    public class TableNameConstantsTests
    {
        [TestCase(TableNameConstants.Collections.Formattable.GOODTYPEDescriptions, "{0}Descriptions")]
        [TestCase(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, "{0}Attributes")]
        [TestCase(TableNameConstants.Collections.Formattable.ITEMTYPESpecialAbilities, "{0}SpecialAbilities")]
        [TestCase(TableNameConstants.Collections.Formattable.POWERITEMTYPE, "{0}{1}")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, "Specific{0}Attributes")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, "Specific{0}SpecialAbilities")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, "Specific{0}Traits")]
        [TestCase(TableNameConstants.Collections.Set.ArmorData, "ArmorData")]
        [TestCase(TableNameConstants.Collections.Set.ChargeLimits, "ChargeLimits")]
        [TestCase(TableNameConstants.Collections.Set.IntelligenceCommunication, "IntelligenceCommunication")]
        [TestCase(TableNameConstants.Collections.Set.IntelligenceData, "IntelligenceData")]
        [TestCase(TableNameConstants.Collections.Set.ItemAlignmentRequirements, "ItemAlignmentRequirements")]
        [TestCase(TableNameConstants.Collections.Set.ItemGroups, "ItemGroups")]
        [TestCase(TableNameConstants.Collections.Set.PowerGroups, "PowerGroups")]
        [TestCase(TableNameConstants.Collections.Set.ReplacementStrings, "ReplacementStrings")]
        [TestCase(TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements, "SpecialAbilityAttributeRequirements")]
        [TestCase(TableNameConstants.Collections.Set.SpecialAbilityData, "SpecialAbilityData")]
        [TestCase(TableNameConstants.Collections.Set.SpecialMaterials, "SpecialMaterials")]
        [TestCase(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "SpecificCursedItemItemTypes")]
        [TestCase(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "SpecificCursedItemAttributes")]
        [TestCase(TableNameConstants.Collections.Set.WeaponDamages, "WeaponDamages")]
        [TestCase(TableNameConstants.Collections.Set.WeaponData, "WeaponData")]
        [TestCase(TableNameConstants.Collections.Set.WondrousItemContents, "WondrousItemContents")]
        [TestCase(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "{0}Types")]
        [TestCase(TableNameConstants.Percentiles.Formattable.GOODTYPEValues, "{0}Values")]
        [TestCase(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Intelligence{0}Powers")]
        [TestCase(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, "Is{0}Intelligent")]
        [TestCase(TableNameConstants.Percentiles.Formattable.LevelXCoins, "Level{0}Coins")]
        [TestCase(TableNameConstants.Percentiles.Formattable.LevelXGoods, "Level{0}Goods")]
        [TestCase(TableNameConstants.Percentiles.Formattable.LevelXItems, "Level{0}Items")]
        [TestCase(TableNameConstants.Percentiles.Formattable.LevelXSPELLTYPESpells, "Level{0}{1}Spells")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, "{0}ArmorTypes")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERItems, "{0}Items")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "{0}{1}s")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, "{0}{1}SpecialAbilities")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "{0}Specific{1}s")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERSpellLevels, "{0}SpellLevels")]
        [TestCase(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "{0}Weapons")]
        [TestCase(TableNameConstants.Percentiles.Set.AlchemicalItems, "AlchemicalItems")]
        [TestCase(TableNameConstants.Percentiles.Set.MundaneGearSizes, "MundaneGearSizes")]
        [TestCase(TableNameConstants.Percentiles.Set.BalorOrPitFiend, "BalorOrPitFiend")]
        [TestCase(TableNameConstants.Percentiles.Set.CastersShieldContainsSpell, "CastersShieldContainsSpell")]
        [TestCase(TableNameConstants.Percentiles.Set.CastersShieldSpellTypes, "CastersShieldSpellTypes")]
        [TestCase(TableNameConstants.Percentiles.Set.CursedDependentSituations, "CursedDependentSituations")]
        [TestCase(TableNameConstants.Percentiles.Set.CurseDrawbacks, "CurseDrawbacks")]
        [TestCase(TableNameConstants.Percentiles.Set.Curses, "Curses")]
        [TestCase(TableNameConstants.Percentiles.Set.DarkwoodShields, "DarkwoodShields")]
        [TestCase(TableNameConstants.Percentiles.Set.Gender, "Gender")]
        [TestCase(TableNameConstants.Percentiles.Set.HasSpecialMaterial, "HasSpecialMaterial")]
        [TestCase(TableNameConstants.Percentiles.Set.HornOfValhallaTypes, "HornOfValhallaTypes")]
        [TestCase(TableNameConstants.Percentiles.Set.IntelligenceAlignments, "IntelligenceAlignments")]
        [TestCase(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers, "IntelligenceDedicatedPowers")]
        [TestCase(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes, "IntelligenceSpecialPurposes")]
        [TestCase(TableNameConstants.Percentiles.Set.IntelligenceStrongStats, "IntelligenceStrongStats")]
        [TestCase(TableNameConstants.Percentiles.Set.IronFlaskContents, "IronFlaskContents")]
        [TestCase(TableNameConstants.Percentiles.Set.IsDeckOfIllusionsFullyCharged, "IsDeckOfIllusionsFullyCharged")]
        [TestCase(TableNameConstants.Percentiles.Set.IsItemCursed, "IsItemCursed")]
        [TestCase(TableNameConstants.Percentiles.Set.IsMasterwork, "IsMasterwork")]
        [TestCase(TableNameConstants.Percentiles.Set.Languages, "Languages")]
        [TestCase(TableNameConstants.Percentiles.Set.MagicalWeaponTypes, "MagicalWeaponTypes")]
        [TestCase(TableNameConstants.Percentiles.Set.MeleeWeaponTraits, "MeleeWeaponTraits")]
        [TestCase(TableNameConstants.Percentiles.Set.MundaneArmors, "MundaneArmors")]
        [TestCase(TableNameConstants.Percentiles.Set.MundaneShields, "MundaneShields")]
        [TestCase(TableNameConstants.Percentiles.Set.MundaneWeaponTypes, "MundaneWeaponTypes")]
        [TestCase(TableNameConstants.Percentiles.Set.PersonalityTraits, "PersonalityTraits")]
        [TestCase(TableNameConstants.Percentiles.Set.Planes, "Planes")]
        [TestCase(TableNameConstants.Percentiles.Set.RangedWeaponTraits, "RangedWeaponTraits")]
        [TestCase(TableNameConstants.Percentiles.Set.RobeOfTheArchmagiColors, "RobeOfTheArchmagiColors")]
        [TestCase(TableNameConstants.Percentiles.Set.RobeOfUsefulItemsExtraItems, "RobeOfUsefulItemsExtraItems")]
        [TestCase(TableNameConstants.Percentiles.Set.RodOfAbsorptionContainsSpellLevels, "RodOfAbsorptionContainsSpellLevels")]
        [TestCase(TableNameConstants.Percentiles.Set.SpecificCursedItems, "SpecificCursedItems")]
        [TestCase(TableNameConstants.Percentiles.Set.SpellStoringContainsSpell, "SpellStoringContainsSpell")]
        [TestCase(TableNameConstants.Percentiles.Set.SpellTypes, "SpellTypes")]
        [TestCase(TableNameConstants.Percentiles.Set.Tools, "Tools")]
        public void TableNameConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void PercentileItemTypeTraits()
        {
            Assert.That(TableNameConstants.Percentiles.Formattable.ITEMTYPETraits, Is.EqualTo("{0}Traits"));
        }

        [Test]
        public void CollectionItemTypeTraits()
        {
            Assert.That(TableNameConstants.Collections.Formattable.ITEMTYPETraits, Is.EqualTo("{0}Traits"));
        }
    }
}