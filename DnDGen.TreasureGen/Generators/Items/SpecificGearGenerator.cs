﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items
{
    internal class SpecificGearGenerator : ISpecificGearGenerator
    {
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpellGenerator spellGenerator;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly IReplacementSelector replacementSelector;

        public SpecificGearGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
            ICollectionSelector collectionsSelector,
            IChargesGenerator chargesGenerator,
            ITreasurePercentileSelector percentileSelector,
            ISpellGenerator spellGenerator,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            JustInTimeFactory justInTimeFactory,
            IReplacementSelector replacementSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.chargesGenerator = chargesGenerator;
            this.percentileSelector = percentileSelector;
            this.spellGenerator = spellGenerator;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.justInTimeFactory = justInTimeFactory;
            this.replacementSelector = replacementSelector;
        }

        private Item SetPrototypeAttributes(Item prototype, string specificGearType)
        {
            var gear = prototype.Clone();

            if (gear.Name == WeaponConstants.JavelinOfLightning)
            {
                gear.IsMagical = true;
            }
            else if (gear.Name == ArmorConstants.CastersShield)
            {
                var hasSpell = percentileSelector.SelectFrom<bool>(TableNameConstants.Percentiles.Set.CastersShieldContainsSpell);

                if (hasSpell)
                {
                    var spellType = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.CastersShieldSpellTypes);
                    var spellLevel = spellGenerator.GenerateLevel(PowerConstants.Medium);
                    var spell = spellGenerator.Generate(spellType, spellLevel);
                    var formattedSpell = string.Format("{0} ({1}, {2})", spell, spellType, spellLevel);
                    gear.Contents.Add(formattedSpell);
                }
            }

            var templateName = gear.Name;
            gear.Name = replacementSelector.SelectSingle(templateName);
            gear.Magic.SpecialAbilities = GetSpecialAbilities(specificGearType, templateName);

            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, specificGearType);
            gear.Attributes = collectionsSelector.SelectFrom(tableName, templateName);

            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, specificGearType);
            var traits = collectionsSelector.SelectFrom(tableName, templateName);

            foreach (var trait in traits)
                gear.Traits.Add(trait);

            if (gear.Attributes.Contains(AttributeConstants.Charged))
                gear.Magic.Charges = chargesGenerator.GenerateFor(specificGearType, templateName);

            if (gear.Name == WeaponConstants.SlayingArrow || gear.Name == WeaponConstants.GreaterSlayingArrow)
            {
                var designatedFoe = collectionsSelector.SelectRandomFrom(TableNameConstants.Collections.Set.ReplacementStrings, ReplacementStringConstants.DesignatedFoe);
                var trait = string.Format("Designated Foe: {0}", designatedFoe);
                gear.Traits.Add(trait);
            }

            if (gear.IsMagical)
                gear.Traits.Add(TraitConstants.Masterwork);

            if (gear.ItemType == ItemTypeConstants.Armor)
                return GetArmor(gear);

            if (gear.ItemType == ItemTypeConstants.Weapon)
                return GetWeapon(gear);

            if (gear.Quantity == 0)
                gear.Quantity = 1;

            return gear;
        }

        private Armor GetArmor(Item gear)
        {
            var name = gear.BaseNames.First();

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.Generate(name, gear.Traits.ToArray()) as Armor;

            gear.CloneInto(armor);

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            armor.Traits.Remove(armor.Size);

            return armor;
        }

        private Weapon GetWeapon(Item gear)
        {
            var name = gear.BaseNames.First();

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = mundaneWeaponGenerator.Generate(name, gear.Traits.ToArray()) as Weapon;

            gear.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            if (weapon.Attributes.Contains(AttributeConstants.Ammunition) || weapon.Attributes.Contains(AttributeConstants.OneTimeUse))
                weapon.Magic.Intelligence = new Intelligence();

            weapon.Traits.Remove(weapon.Size);

            return weapon;
        }

        private IEnumerable<SpecialAbility> GetSpecialAbilities(string specificGearType, string name)
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, specificGearType);
            var abilityNames = collectionsSelector.SelectFrom(tableName, name);
            var abilityPrototypes = abilityNames.Select(n => new SpecialAbility { Name = n });
            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes);

            return abilities;
        }

        public Item GenerateFrom(Item template)
        {
            var gear = template.Clone();

            var specificGearType = GetSpecificGearType(gear.Name);
            gear.ItemType = GetItemType(specificGearType);
            gear.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, gear.Name);

            gear = SetPrototypeAttributes(gear, specificGearType);

            return gear;
        }

        private string GetItemType(string specificGearType)
        {
            if (specificGearType == ItemTypeConstants.Weapon)
                return ItemTypeConstants.Weapon;

            return ItemTypeConstants.Armor;
        }

        private string GetSpecificGearType(string name)
        {
            var gearType = collectionsSelector.FindCollectionOf(TableNameConstants.Collections.Set.ItemGroups, name, AttributeConstants.Shield, ItemTypeConstants.Armor, ItemTypeConstants.Weapon);
            return gearType;
        }

        public bool IsSpecific(Item template)
        {
            return NameMatchesSpecific(template.Name);
        }

        public Item GeneratePrototypeFrom(string power, string specificGearType, string name, params string[] traits)
        {
            var possiblePowers = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, name);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, adjustedPower, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);
            selections = selections.Where(s => NameMatchesWithReplacements(name, s.Type));

            var selection = collectionsSelector.SelectRandomFrom(selections);

            var gear = new Item();
            gear.Name = selection.Type;
            gear.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type);
            gear.ItemType = GetItemType(specificGearType);
            gear.Magic.Bonus = selection.Amount;
            gear.Quantity = 0;
            gear.Traits = new HashSet<string>(traits);

            return gear;
        }

        private bool NameMatchesSpecific(string name)
        {
            var weapons = GetGear(ItemTypeConstants.Weapon);
            var armors = GetGear(ItemTypeConstants.Armor);
            var shields = GetGear(AttributeConstants.Shield);

            var specificGear = weapons.Union(armors).Union(shields);

            return specificGear.Contains(name);
        }

        private bool NameMatchesWithReplacements(string source, string target)
        {
            var sourceReplacements = replacementSelector.SelectAll(source, true);
            var targetReplacements = replacementSelector.SelectAll(target, true);

            return source == target
                || sourceReplacements.Any(s => s == target)
                || targetReplacements.Any(t => t == source);
        }

        public string GenerateRandomNameFrom(string power, string specificGearType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var selection = typeAndAmountPercentileSelector.SelectFrom(tableName);

            return selection.Type;
        }

        public string GenerateNameFrom(string power, string specificGearType, string baseType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            var names = new List<string>();

            foreach (var selection in selections)
            {
                var baseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type);
                if (baseNames.Contains(baseType))
                    names.Add(selection.Type);
            }

            if (!names.Any())
            {
                throw new ArgumentException($"No {power} specific {specificGearType} has base type {baseType}");
            }

            var name = collectionsSelector.SelectRandomFrom(names);

            return name;
        }

        public bool IsSpecific(string specificGearType, string itemName)
        {
            var gearTypeCollection = GetGear(specificGearType);

            var isSpecific = NameMatchesSpecific(itemName);
            isSpecific &= gearTypeCollection.Any(i => NameMatchesWithReplacements(itemName, i));

            return isSpecific;
        }

        private IEnumerable<string> GetGear(string gearType)
        {
            switch (gearType)
            {
                case ItemTypeConstants.Weapon:
                    {
                        var general = WeaponConstants.GetAllWeapons(false, true);
                        var all = WeaponConstants.GetAllWeapons(true, true);

                        return all.Except(general);
                    }
                case ItemTypeConstants.Armor:
                    {
                        var general = ArmorConstants.GetAllArmors(false);
                        var all = ArmorConstants.GetAllArmors(true);

                        return all.Except(general);
                    }
                case AttributeConstants.Shield:
                    {
                        var general = ArmorConstants.GetAllShields(false);
                        var all = ArmorConstants.GetAllShields(true);

                        return all.Except(general);
                    }
                default: throw new ArgumentException($"{gearType} is not a valid specific gear type");
            }
        }

        public bool CanBeSpecific(string power, string specificGearType, string itemName)
        {
            if (IsSpecific(specificGearType, itemName))
                return true;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var powerSelections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            foreach (var selection in powerSelections)
            {
                var baseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type);
                if (baseNames.Contains(itemName))
                    return true;
            }

            return false;
        }
    }
}