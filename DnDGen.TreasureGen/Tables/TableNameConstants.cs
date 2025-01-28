using System;

namespace DnDGen.TreasureGen.Tables
{
    internal static class TableNameConstants
    {
        public static class Percentiles
        {
            [Obsolete("Don't do formattable table names, find a different way")]
            public static class Formattable
            {
                public const string ARMORTYPETypes = "{0}Types";
                public const string IsITEMTYPEIntelligent = "Is{0}Intelligent";
                public const string LevelXSPELLTYPESpells = "Level{0}{1}Spells";
                public const string POWERArmorTypes = "{0}ArmorTypes";
                public const string POWERATTRIBUTESpecialAbilities = "{0}{1}SpecialAbilities";
                public const string POWERSpecificITEMTYPEs = "{0}Specific{1}s";
                public const string POWERSpellLevels = "{0}SpellLevels";
            }

            public const string AlchemicalItems = "AlchemicalItems";
            public const string BalorOrPitFiend = "BalorOrPitFiend";
            public const string CastersShieldContainsSpell = "CastersShieldContainsSpell";
            public const string CastersShieldSpellTypes = "CastersShieldSpellTypes";
            public const string CurseDrawbacks = "CurseDrawbacks";
            public const string CursedDependentSituations = "CursedDependentSituations";
            public const string Curses = "Curses";
            public const string DarkwoodShields = "DarkwoodShields";
            public const string Gender = "Gender";
            public const string HasSpecialMaterial = "HasSpecialMaterial";
            public const string HornOfValhallaTypes = "HornOfValhallaTypes";
            public const string IntelligenceAlignments = "IntelligenceAlignments";
            public const string IntelligenceDedicatedPowers = "IntelligenceDedicatedPowers";
            public const string IntelligenceLesserPowers = "IntelligenceLesserPowers";
            public const string IntelligenceGreaterPowers = "IntelligenceGreaterPowers";
            public const string IntelligenceSpecialPurposes = "IntelligenceSpecialPurposes";
            public const string IntelligenceStrongStats = "IntelligenceStrongStats";
            public const string IronFlaskContents = "IronFlaskContents";
            public const string IsDeckOfIllusionsFullyCharged = "IsDeckOfIllusionsFullyCharged";
            public const string IsItemCursed = "IsItemCursed";
            public const string IsMasterwork = "IsMasterwork";
            public const string Languages = "Languages";
            public const string MagicalWeaponTypes = "MagicalWeaponTypes";
            public const string MeleeWeaponTraits = "MeleeWeaponTraits";
            public const string MundaneArmors = "MundaneArmors";
            public const string MundaneGearSizes = "MundaneGearSizes";
            public const string MundaneShields = "MundaneShields";
            public const string MundaneWeaponTypes = "MundaneWeaponTypes";
            public const string PersonalityTraits = "PersonalityTraits";
            public const string Planes = "Planes";
            public const string RangedWeaponTraits = "RangedWeaponTraits";
            public const string RobeOfTheArchmagiColors = "RobeOfTheArchmagiColors";
            public const string RobeOfUsefulItemsExtraItems = "RobeOfUsefulItemsExtraItems";
            public const string RodOfAbsorptionContainsSpellLevels = "RodOfAbsorptionContainsSpellLevels";
            public const string SpecificCursedItems = "SpecificCursedItems";
            public const string SpellStoringContainsSpell = "SpellStoringContainsSpell";
            public const string SpellTypes = "SpellTypes";
            public const string Tools = "Tools";

            public static string LevelXCoins(int level) => $"Level{level}Coins";
            public static string LevelXGoods(int level) => $"Level{level}Goods";
            public static string LevelXItems(int level) => $"Level{level}Items";
            public static string GOODTYPEValues(string goodType) => $"{goodType}Values";
            public static string POWERItems(string power) => $"{power}Items";
            public static string POWERITEMTYPEs(string power, string itemType) => $"{power}{itemType}s";
            public static string ITEMTYPETraits(string itemType) => $"{itemType}Traits";
            public static string WEAPONTYPEWeapons(string weaponType) => $"{weaponType}Weapons";
        }

        public static class Collections
        {
            [Obsolete("Don't do formattable table names, find a different way")]
            public static class Formattable
            {
                public const string ITEMTYPESpecialAbilities = "{0}SpecialAbilities";
                public const string POWERITEMTYPE = "{0}{1}";
                public const string SpecificITEMTYPEAttributes = "Specific{0}Attributes";
                public const string SpecificITEMTYPESpecialAbilities = "Specific{0}SpecialAbilities";
                public const string SpecificITEMTYPETraits = "Specific{0}Traits";
            }

            public const string ArmorData = "ArmorData";
            public const string ChargeLimits = "ChargeLimits";
            public const string ExtraItems = "ExtraItems";
            public const string IntelligenceData = "IntelligenceData";
            public const string IntelligenceCommunication = "IntelligenceCommunication";
            public const string IsIntelligent = "IsIntelligent";
            public const string ItemAlignmentRequirements = "ItemAlignmentRequirements";
            public const string ItemGroups = "ItemGroups";
            public const string PowerGroups = "PowerGroups";
            public const string ReplacementStrings = "ReplacementStrings";
            public const string SpecialAbilityData = "SpecialAbilityData";
            public const string SpecialAbilityAttributeRequirements = "SpecialAbilityAttributeRequirements";
            public const string SpecialMaterials = "SpecialMaterials";
            public const string SpecificCursedItemItemTypes = "SpecificCursedItemItemTypes";
            public const string SpecificCursedItemAttributes = "SpecificCursedItemAttributes";
            public const string AbilityDamages = "AbilityDamages";
            public const string WeaponDamages = "WeaponDamages";
            public const string WeaponCriticalDamages = "WeaponCriticalDamages";
            public const string WeaponData = "WeaponData";
            public const string WondrousItemContents = "WondrousItemContents";

            public static string GOODTYPEDescriptions(string goodType) => $"{goodType}Descriptions";
            public static string ITEMTYPEAttributes(string itemType) => $"{itemType}Attributes";
        }
    }
}