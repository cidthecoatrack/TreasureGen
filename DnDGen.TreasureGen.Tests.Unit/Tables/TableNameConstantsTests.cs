using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Tables
{
    [TestFixture]
    public class TableNameConstantsTests
    {
        [TestCase(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, "{0}Attributes")]
        [TestCase(TableNameConstants.Collections.Formattable.ITEMTYPESpecialAbilities, "{0}SpecialAbilities")]
        [TestCase(TableNameConstants.Collections.Formattable.POWERITEMTYPE, "{0}{1}")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, "Specific{0}Attributes")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, "Specific{0}SpecialAbilities")]
        [TestCase(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, "Specific{0}Traits")]
        [TestCase(TableNameConstants.Collections.ArmorData, "ArmorData")]
        [TestCase(TableNameConstants.Collections.ChargeLimits, "ChargeLimits")]
        [TestCase(TableNameConstants.Collections.ExtraItems, "ExtraItems")]
        [TestCase(TableNameConstants.Collections.IntelligenceCommunication, "IntelligenceCommunication")]
        [TestCase(TableNameConstants.Collections.IntelligenceData, "IntelligenceData")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceLesserPowers, "IntelligenceLesserPowers")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceGreaterPowers, "IntelligenceGreaterPowers")]
        [TestCase(TableNameConstants.Collections.ItemAlignmentRequirements, "ItemAlignmentRequirements")]
        [TestCase(TableNameConstants.Collections.ItemGroups, "ItemGroups")]
        [TestCase(TableNameConstants.Collections.PowerGroups, "PowerGroups")]
        [TestCase(TableNameConstants.Collections.ReplacementStrings, "ReplacementStrings")]
        [TestCase(TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "SpecialAbilityAttributeRequirements")]
        [TestCase(TableNameConstants.Collections.SpecialAbilityData, "SpecialAbilityData")]
        [TestCase(TableNameConstants.Collections.SpecialMaterials, "SpecialMaterials")]
        [TestCase(TableNameConstants.Collections.SpecificCursedItemItemTypes, "SpecificCursedItemItemTypes")]
        [TestCase(TableNameConstants.Collections.SpecificCursedItemAttributes, "SpecificCursedItemAttributes")]
        [TestCase(TableNameConstants.Collections.WeaponDamages, "WeaponDamages")]
        [TestCase(TableNameConstants.Collections.WeaponData, "WeaponData")]
        [TestCase(TableNameConstants.Collections.WondrousItemContents, "WondrousItemContents")]
        [TestCase(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "{0}Types")]
        [TestCase(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, "Is{0}Intelligent")]
        [TestCase(TableNameConstants.Percentiles.Formattable.LevelXSPELLTYPESpells, "Level{0}{1}Spells")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, "{0}ArmorTypes")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, "{0}{1}SpecialAbilities")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "{0}Specific{1}s")]
        [TestCase(TableNameConstants.Percentiles.Formattable.POWERSpellLevels, "{0}SpellLevels")]
        [TestCase(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, "{0}Weapons")]
        [TestCase(TableNameConstants.Percentiles.AlchemicalItems, "AlchemicalItems")]
        [TestCase(TableNameConstants.Percentiles.MundaneGearSizes, "MundaneGearSizes")]
        [TestCase(TableNameConstants.Percentiles.BalorOrPitFiend, "BalorOrPitFiend")]
        [TestCase(TableNameConstants.Percentiles.CastersShieldContainsSpell, "CastersShieldContainsSpell")]
        [TestCase(TableNameConstants.Percentiles.CastersShieldSpellTypes, "CastersShieldSpellTypes")]
        [TestCase(TableNameConstants.Percentiles.CursedDependentSituations, "CursedDependentSituations")]
        [TestCase(TableNameConstants.Percentiles.CurseDrawbacks, "CurseDrawbacks")]
        [TestCase(TableNameConstants.Percentiles.Curses, "Curses")]
        [TestCase(TableNameConstants.Percentiles.DarkwoodShields, "DarkwoodShields")]
        [TestCase(TableNameConstants.Percentiles.Gender, "Gender")]
        [TestCase(TableNameConstants.Percentiles.HasSpecialMaterial, "HasSpecialMaterial")]
        [TestCase(TableNameConstants.Percentiles.HornOfValhallaTypes, "HornOfValhallaTypes")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceAlignments, "IntelligenceAlignments")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceDedicatedPowers, "IntelligenceDedicatedPowers")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceSpecialPurposes, "IntelligenceSpecialPurposes")]
        [TestCase(TableNameConstants.Percentiles.IntelligenceStrongStats, "IntelligenceStrongStats")]
        [TestCase(TableNameConstants.Percentiles.IronFlaskContents, "IronFlaskContents")]
        [TestCase(TableNameConstants.Percentiles.IsDeckOfIllusionsFullyCharged, "IsDeckOfIllusionsFullyCharged")]
        [TestCase(TableNameConstants.Percentiles.IsItemCursed, "IsItemCursed")]
        [TestCase(TableNameConstants.Percentiles.IsMasterwork, "IsMasterwork")]
        [TestCase(TableNameConstants.Percentiles.Languages, "Languages")]
        [TestCase(TableNameConstants.Percentiles.MagicalWeaponTypes, "MagicalWeaponTypes")]
        [TestCase(TableNameConstants.Percentiles.MeleeWeaponTraits, "MeleeWeaponTraits")]
        [TestCase(TableNameConstants.Percentiles.MundaneArmors, "MundaneArmors")]
        [TestCase(TableNameConstants.Percentiles.MundaneShields, "MundaneShields")]
        [TestCase(TableNameConstants.Percentiles.MundaneWeaponTypes, "MundaneWeaponTypes")]
        [TestCase(TableNameConstants.Percentiles.PersonalityTraits, "PersonalityTraits")]
        [TestCase(TableNameConstants.Percentiles.Planes, "Planes")]
        [TestCase(TableNameConstants.Percentiles.RangedWeaponTraits, "RangedWeaponTraits")]
        [TestCase(TableNameConstants.Percentiles.RobeOfTheArchmagiColors, "RobeOfTheArchmagiColors")]
        [TestCase(TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems, "RobeOfUsefulItemsExtraItems")]
        [TestCase(TableNameConstants.Percentiles.RodOfAbsorptionContainsSpellLevels, "RodOfAbsorptionContainsSpellLevels")]
        [TestCase(TableNameConstants.Percentiles.SpecificCursedItems, "SpecificCursedItems")]
        [TestCase(TableNameConstants.Percentiles.SpellStoringContainsSpell, "SpellStoringContainsSpell")]
        [TestCase(TableNameConstants.Percentiles.SpellTypes, "SpellTypes")]
        [TestCase(TableNameConstants.Percentiles.Tools, "Tools")]
        public void TableNameConstant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void LevelXCoins()
        {
            var tablename = TableNameConstants.Percentiles.LevelXCoins(9266);
            Assert.That(tablename, Is.EqualTo("Level9266Coins"));
        }

        [Test]
        public void LevelXGoods()
        {
            var tablename = TableNameConstants.Percentiles.LevelXGoods(9266);
            Assert.That(tablename, Is.EqualTo("Level9266Goods"));
        }

        [Test]
        public void LevelXItems()
        {
            var tablename = TableNameConstants.Percentiles.LevelXItems(9266);
            Assert.That(tablename, Is.EqualTo("Level9266Items"));
        }

        [Test]
        public void GoodTypeValues()
        {
            var tablename = TableNameConstants.Percentiles.GOODTYPEValues("myGoodType");
            Assert.That(tablename, Is.EqualTo("myGoodTypeValues"));
        }

        [Test]
        public void GoodTypeDescriptions()
        {
            var tablename = TableNameConstants.Collections.GOODTYPEDescriptions("myGoodType");
            Assert.That(tablename, Is.EqualTo("myGoodTypeDescriptions"));
        }

        [Test]
        public void PowerItems()
        {
            var tablename = TableNameConstants.Percentiles.POWERItems("myPower");
            Assert.That(tablename, Is.EqualTo("myPowerItems"));
        }

        [Test]
        public void PowerItemTypes()
        {
            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs("myPower", "myItemType");
            Assert.That(tablename, Is.EqualTo("myPowermyItemTypes"));
        }

        [Test]
        public void ItemTypeTraits()
        {
            Assert.That(TableNameConstants.Percentiles.ITEMTYPETraits("myItemType"), Is.EqualTo("myItemTypeTraits"));
        }
    }
}