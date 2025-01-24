using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class RodGenerator : MagicalItemGenerator
    {
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public RodGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
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
            var rodPowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Rod);
            var adjustedPower = PowerHelper.AdjustPower(power, rodPowers);

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, adjustedPower, ItemTypeConstants.Rod);
            var result = typeAndAmountPercentileSelector.SelectFrom(tablename);

            return GenerateRod(result.Type, result.Amount);
        }

        private Item GenerateRod(string name, int bonus, params string[] traits)
        {
            var rod = new Item();
            rod.ItemType = ItemTypeConstants.Rod;
            rod.Name = name;
            rod.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, name);
            rod.IsMagical = true;
            rod.Magic.Bonus = bonus;
            rod.Traits = new HashSet<string>(traits);

            var tablename = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            rod.Attributes = collectionsSelector.SelectFrom(Config.Name, tablename, name);

            if (rod.Attributes.Contains(AttributeConstants.Charged))
                rod.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Rod, name);

            if (name == RodConstants.Absorption)
            {
                var containsSpellLevels = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.Set.RodOfAbsorptionContainsSpellLevels);
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

            var powers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.PowerGroups, rodName);
            var adjustedPower = PowerHelper.AdjustPower(power, powers);

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, adjustedPower, ItemTypeConstants.Rod);
            var results = typeAndAmountPercentileSelector.SelectAllFrom(tablename);
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

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, rod.Traits.ToArray()) as Weapon;

            rod.Attributes = rod.Attributes.Union(mundaneWeapon.Attributes);
            rod.CloneInto(mundaneWeapon);

            if (mundaneWeapon.IsMagical)
                mundaneWeapon.Traits.Add(TraitConstants.Masterwork);

            if (mundaneWeapon.IsDoubleWeapon)
            {
                mundaneWeapon.SecondaryHasAbilities = true;
                mundaneWeapon.SecondaryMagicBonus = rod.Magic.Bonus;
            }

            foreach (var specialAbility in mundaneWeapon.Magic.SpecialAbilities)
            {
                if (specialAbility.Damages.Any())
                {
                    var damages = specialAbility.Damages.Select(d => d.Clone()).ToArray();
                    var damageType = mundaneWeapon.Damages[0].Type;

                    foreach (var damage in damages)
                    {
                        if (string.IsNullOrEmpty(damage.Type))
                        {
                            damage.Type = damageType;
                        }
                    }

                    mundaneWeapon.Damages.AddRange(damages);

                    if (mundaneWeapon.SecondaryHasAbilities)
                    {
                        var secondaryDamages = specialAbility.Damages.Select(d => d.Clone()).ToArray();
                        var secondaryDamageType = mundaneWeapon.SecondaryDamages[0].Type;

                        foreach (var damage in secondaryDamages)
                        {
                            if (string.IsNullOrEmpty(damage.Type))
                            {
                                damage.Type = secondaryDamageType;
                            }
                        }

                        mundaneWeapon.SecondaryDamages.AddRange(secondaryDamages);
                    }
                }

                if (specialAbility.CriticalDamages.Any())
                {
                    var damageType = mundaneWeapon.CriticalDamages[0].Type;
                    foreach (var damage in specialAbility.CriticalDamages[mundaneWeapon.CriticalMultiplier])
                    {
                        if (string.IsNullOrEmpty(damage.Type))
                        {
                            damage.Type = damageType;
                        }
                    }

                    mundaneWeapon.CriticalDamages.AddRange(specialAbility.CriticalDamages[mundaneWeapon.CriticalMultiplier]);

                    if (mundaneWeapon.SecondaryHasAbilities)
                    {
                        foreach (var damage in specialAbility.CriticalDamages[mundaneWeapon.SecondaryCriticalMultiplier])
                        {
                            if (string.IsNullOrEmpty(damage.Type))
                            {
                                damage.Type = damageType;
                            }
                        }

                        mundaneWeapon.SecondaryCriticalDamages.AddRange(specialAbility.CriticalDamages[mundaneWeapon.SecondaryCriticalMultiplier]);
                    }
                }

                if (specialAbility.Name == SpecialAbilityConstants.Keen)
                {
                    mundaneWeapon.ThreatRange *= 2;
                }
            }

            return mundaneWeapon;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var rod = template.Clone();
            rod.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, rod.Name);
            rod.IsMagical = true;
            rod.Quantity = 1;
            rod.ItemType = ItemTypeConstants.Rod;

            var results = GetAllResults();
            var result = results.First(r => rod.NameMatches(r.Type));
            rod.Magic.Bonus = result.Amount;

            var tablename = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Rod);
            rod.Attributes = collectionsSelector.SelectFrom(Config.Name, tablename, rod.Name);

            rod.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(rod.Magic.SpecialAbilities);

            foreach (var trait in template.Traits)
            {
                rod.Traits.Add(trait);
            }

            rod = GetWeapon(rod);

            return rod.SmartClone();
        }

        private IEnumerable<TypeAndAmountSelection> GetAllResults()
        {
            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Rod);
            var mediumResults = typeAndAmountPercentileSelector.SelectAllFrom(tablename);

            tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Rod);
            var majorResults = typeAndAmountPercentileSelector.SelectAllFrom(tablename);

            return mediumResults.Union(majorResults);
        }

        private string GetRodName(string itemName)
        {
            var rods = RodConstants.GetAllRods();
            if (rods.Contains(itemName))
                return itemName;

            var rodFromBaseName = collectionsSelector.FindCollectionOf(Config.Name, TableNameConstants.Collections.Set.ItemGroups, itemName, rods.ToArray());

            return rodFromBaseName;
        }
    }
}