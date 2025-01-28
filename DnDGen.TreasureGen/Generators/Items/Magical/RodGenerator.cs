using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class RodGenerator : MagicalItemGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public RodGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            ICollectionSelector collectionsSelector,
            IChargesGenerator chargesGenerator,
            ITreasurePercentileSelector percentileSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            JustInTimeFactory justInTimeFactory)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.chargesGenerator = chargesGenerator;
            this.percentileSelector = percentileSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Item GenerateRandom(string power)
        {
            var rodPowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod);
            var adjustedPower = PowerHelper.AdjustPower(power, rodPowers);
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Rod));

            return GenerateRod(result.Type, result.Amount);
        }

        private Item GenerateRod(string name, int bonus, params string[] traits)
        {
            var rod = new Item
            {
                ItemType = ItemTypeConstants.Rod,
                Name = name,
                BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name),
                IsMagical = true
            };
            rod.Magic.Bonus = bonus;
            rod.Traits = new HashSet<string>(traits);
            rod.Attributes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod), name);

            if (rod.Attributes.Contains(AttributeConstants.Charged))
                rod.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Rod, name);

            if (name == RodConstants.Absorption)
            {
                var containsSpellLevels = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.RodOfAbsorptionContainsSpellLevels);
                if (containsSpellLevels)
                {
                    var maxCharges = chargesGenerator.GenerateFor(ItemTypeConstants.Rod, RodConstants.Absorption_Full);
                    var containedSpellLevels = (maxCharges - rod.Magic.Charges) / 2;
                    rod.Contents.Add($"{containedSpellLevels} spell levels");
                }
            }

            rod = GetWeapon(rod);

            return rod;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var rodName = GetRodName(itemName);

            var powers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, rodName);
            var adjustedPower = PowerHelper.AdjustPower(power, powers);

            var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Rod));
            var matches = results.Where(r => r.Type == rodName).ToList();

            var match = collectionsSelector.SelectRandomFrom(matches);
            return GenerateRod(match.Type, match.Amount, traits);
        }

        private Item GetWeapon(Item rod)
        {
            var weapons = WeaponConstants.GetAllMelee(false, false);
            var weaponBaseNames = weapons.Intersect(rod.BaseNames);
            if (!weaponBaseNames.Any())
                return rod;

            var weaponName = weaponBaseNames.First();

            var weaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = weaponGenerator.Generate(weaponName, [.. rod.Traits]) as Weapon;

            rod.Attributes = rod.Attributes.Union(weapon.Attributes);
            rod.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            if (weapon.IsDoubleWeapon)
            {
                weapon.SecondaryHasAbilities = true;
                weapon.SecondaryMagicBonus = rod.Magic.Bonus;
            }

            weapon = ApplySpecialAbilities(weapon);
            return weapon;
        }

        private Weapon ApplySpecialAbilities(Weapon weapon)
        {
            foreach (var specialAbility in weapon.Magic.SpecialAbilities)
            {
                var weaponDamages = GetWeaponDamages(specialAbility.Damages, weapon.Damages[0].Type);
                weapon.Damages.AddRange(weaponDamages);

                var weaponCritDamages = GetWeaponDamages(specialAbility.CriticalDamages, weapon.CriticalDamages[0].Type);
                weapon.CriticalDamages.AddRange(weaponCritDamages);

                if (specialAbility.Name == SpecialAbilityConstants.Keen)
                {
                    weapon.ThreatRange *= 2;
                }
            }

            if (weapon.IsDoubleWeapon && weapon.SecondaryHasAbilities)
            {
                var secondaryAbilities = specialAbilitiesGenerator.GenerateFor(weapon.Magic.SpecialAbilities, weapon.SecondaryCriticalMultiplier);

                foreach (var specialAbility in secondaryAbilities)
                {
                    var weaponDamages = GetWeaponDamages(specialAbility.Damages, weapon.SecondaryDamages[0].Type);
                    weapon.SecondaryDamages.AddRange(weaponDamages);

                    var weaponCritDamages = GetWeaponDamages(specialAbility.CriticalDamages, weapon.SecondaryCriticalDamages[0].Type);
                    weapon.SecondaryCriticalDamages.AddRange(weaponCritDamages);
                }
            }

            return weapon;
        }

        private IEnumerable<Damage> GetWeaponDamages(IEnumerable<Damage> abilityDamages, string weaponDamageType)
        {
            if (!abilityDamages.Any())
                return [];

            var damages = abilityDamages.Select(d => d.Clone()).ToArray();
            foreach (var damage in damages)
            {
                if (string.IsNullOrEmpty(damage.Type))
                {
                    damage.Type = weaponDamageType;
                }
            }

            return damages;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var rod = template.Clone();
            rod.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, rod.Name);
            rod.IsMagical = true;
            rod.Quantity = 1;
            rod.ItemType = ItemTypeConstants.Rod;

            var results = GetAllResults();
            var result = results.First(r => rod.NameMatches(r.Type));
            rod.Magic.Bonus = result.Amount;
            rod.Attributes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod), rod.Name);

            var weapons = WeaponConstants.GetAllMelee(false, false);
            var weaponBaseNames = weapons.Intersect(rod.BaseNames);
            if (weaponBaseNames.Any())
            {
                var weaponName = weaponBaseNames.First();
                var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
                var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, [.. rod.Traits]) as Weapon;

                rod.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(rod.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier);
            }

            foreach (var trait in template.Traits)
            {
                rod.Traits.Add(trait);
            }

            rod = GetWeapon(rod);

            return rod.SmartClone();
        }

        private IEnumerable<TypeAndAmountDataSelection> GetAllResults()
        {
            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            var mediumResults = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tablename);

            tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Rod);
            var majorResults = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tablename);

            return mediumResults.Union(majorResults);
        }

        private string GetRodName(string itemName)
        {
            var rods = RodConstants.GetAllRods();
            if (rods.Contains(itemName))
                return itemName;

            var rodFromBaseName = collectionsSelector.FindCollectionOf(Config.Name, TableNameConstants.Collections.ItemGroups, itemName, rods.ToArray());

            return rodFromBaseName;
        }
    }
}