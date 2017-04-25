﻿using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Selectors.Collections;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Domain.Generators.Items.Mundane
{
    internal class MundaneWeaponGenerator : MundaneItemGenerator
    {
        private readonly IPercentileSelector percentileSelector;
        private readonly ICollectionsSelector collectionsSelector;
        private readonly IBooleanPercentileSelector booleanPercentileSelector;
        private readonly Dice dice;
        private readonly Generator generator;
        private readonly IWeaponDataSelector weaponDataSelector;

        public MundaneWeaponGenerator(
            IPercentileSelector percentileSelector,
            ICollectionsSelector collectionsSelector,
            IBooleanPercentileSelector booleanPercentileSelector,
            Dice dice,
            Generator generator,
            IWeaponDataSelector weaponDataSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.booleanPercentileSelector = booleanPercentileSelector;
            this.generator = generator;
            this.dice = dice;
            this.weaponDataSelector = weaponDataSelector;
        }

        public Item Generate()
        {
            var type = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.MundaneWeapons);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.WEAPONTYPEWeapons, type);
            var weaponName = percentileSelector.SelectFrom(tableName);

            var weapon = new Weapon();
            weapon.ItemType = ItemTypeConstants.Weapon;
            weapon.Name = weaponName;
            weapon.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, weapon.Name);
            weapon.Size = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes);

            if (weapon.Name.Contains("Composite"))
            {
                weapon.Name = GetCompositeBowName(weaponName);
                var compositeStrengthBonus = GetCompositeBowBonus(weaponName);

                if (!string.IsNullOrEmpty(compositeStrengthBonus))
                    weapon.Traits.Add(compositeStrengthBonus);
            }

            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Weapon);
            weapon.Attributes = collectionsSelector.SelectFrom(tableName, weapon.Name);

            //INFO: This must be done after Attributes are set
            weapon.Quantity = GetQuantity(weapon);

            var isMasterwork = booleanPercentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork);
            if (isMasterwork)
                weapon.Traits.Add(TraitConstants.Masterwork);

            var weaponSelection = weaponDataSelector.Select(weapon.Name);
            weapon.CriticalMultiplier = weaponSelection.CriticalMultiplier;
            weapon.Damage = weaponSelection.DamageBySize[weapon.Size];
            weapon.DamageType = weaponSelection.DamageType;
            weapon.ThreatRange = weaponSelection.ThreatRange;

            return weapon;
        }

        private int GetQuantity(Weapon weapon)
        {
            if (weapon.Quantity > 1)
                return weapon.Quantity;

            if (weapon.Attributes.Contains(AttributeConstants.Ammunition))
            {
                var roll = dice.Roll().Percentile().AsSum();
                return Math.Max(1, roll / 2);
            }

            if (weapon.Attributes.Contains(AttributeConstants.Thrown) && !weapon.Attributes.Contains(AttributeConstants.Melee))
                return dice.Roll().d20().AsSum();

            return 1;
        }

        private string GetCompositeBowBonus(string weaponName)
        {
            var compositeBonusStartIndex = weaponName.IndexOf('+');
            if (compositeBonusStartIndex == -1)
                return string.Empty;

            var compositeBonus = weaponName.Substring(compositeBonusStartIndex, 2);
            return $"{compositeBonus} Strength bonus";
        }

        private string GetCompositeBowName(string weaponName)
        {
            switch (weaponName)
            {
                case WeaponConstants.CompositeLongbow:
                case WeaponConstants.CompositePlus0Longbow:
                case WeaponConstants.CompositePlus1Longbow:
                case WeaponConstants.CompositePlus2Longbow:
                case WeaponConstants.CompositePlus3Longbow:
                case WeaponConstants.CompositePlus4Longbow: return WeaponConstants.CompositeLongbow;
                case WeaponConstants.CompositeShortbow:
                case WeaponConstants.CompositePlus0Shortbow:
                case WeaponConstants.CompositePlus1Shortbow:
                case WeaponConstants.CompositePlus2Shortbow: return WeaponConstants.CompositeShortbow;
                default: throw new ArgumentException($"Composite bow {weaponName} does not map to a known bow");
            }
        }

        public Item GenerateFrom(Item template, bool allowRandomDecoration = false)
        {
            var weapon = new Weapon();
            template.MundaneClone(weapon);
            weapon.ItemType = ItemTypeConstants.Weapon;

            if (weapon.Name.Contains("Composite"))
            {
                var fullName = weapon.Name;
                weapon.Name = GetCompositeBowName(fullName);
                var compositeStrengthBonus = GetCompositeBowBonus(fullName);

                if (!string.IsNullOrEmpty(compositeStrengthBonus))
                    weapon.Traits.Add(compositeStrengthBonus);
            }

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Weapon);
            weapon.Attributes = collectionsSelector.SelectFrom(tableName, weapon.Name);

            weapon.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, weapon.Name);

            var sizes = percentileSelector.SelectAllFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes);

            if (weapon.Traits.Intersect(sizes).Any())
            {
                weapon.Size = weapon.Traits.Intersect(sizes).First();
                weapon.Traits.Remove(weapon.Size);
            }
            else if (string.IsNullOrEmpty(weapon.Size))
            {
                weapon.Size = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.MundaneGearSizes);
            }

            var weaponSelection = weaponDataSelector.Select(weapon.Name);
            weapon.CriticalMultiplier = weaponSelection.CriticalMultiplier;
            weapon.Damage = weaponSelection.DamageBySize[weapon.Size];
            weapon.DamageType = weaponSelection.DamageType;
            weapon.ThreatRange = weaponSelection.ThreatRange;

            if (allowRandomDecoration)
            {
                var isMasterwork = booleanPercentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IsMasterwork);
                if (isMasterwork)
                    weapon.Traits.Add(TraitConstants.Masterwork);
            }

            if (weapon.Quantity == 0)
                weapon.Quantity = GetQuantity(weapon);

            return weapon;
        }

        public Item GenerateFrom(IEnumerable<string> subset)
        {
            if (!subset.Any())
                throw new ArgumentException("Cannot generate from an empty collection subset");

            var weapon = generator.Generate(
                Generate,
                w => subset.Any(n => w.NameMatches(n)),
                () => GenerateDefaultFrom(subset),
                $"Mundane weapon from [{string.Join(", ", subset)}]");

            return weapon;
        }

        private Item GenerateDefaultFrom(IEnumerable<string> subset)
        {
            var template = new Item();
            template.Name = collectionsSelector.SelectRandomFrom(subset);
            template.Quantity = 0;

            var defaultWeapon = GenerateFrom(template);
            return defaultWeapon;
        }
    }
}