using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class SpecialAbilitiesGenerator : ISpecialAbilitiesGenerator
    {
        private const int MaxBonus = 10;

        private readonly ICollectionSelector collectionsSelector;
        private readonly ICollectionDataSelector<DamageDataSelection> damageDataSelector;
        private readonly ICollectionDataSelector<SpecialAbilityDataSelection> specialAbilityDataSelector;
        private readonly ITreasurePercentileSelector percentileSelector;

        public SpecialAbilitiesGenerator(
            ICollectionSelector collectionsSelector,
            ITreasurePercentileSelector percentileSelector,
            ICollectionDataSelector<SpecialAbilityDataSelection> specialAbilityDataSelector,
            ICollectionDataSelector<DamageDataSelection> damageDataSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.percentileSelector = percentileSelector;
            this.specialAbilityDataSelector = specialAbilityDataSelector;
            this.damageDataSelector = damageDataSelector;
        }

        public IEnumerable<SpecialAbility> GenerateFor(Item targetItem, string power, int quantity)
        {
            if (targetItem.Magic.Bonus <= 0 || quantity <= 0)
                return [];

            var tableNames = GetTableNames(targetItem, power);
            var bonusSum = targetItem.Magic.Bonus;
            var availableAbilities = GetAvailableAbilities(targetItem, tableNames, bonusSum);
            var abilities = new List<SpecialAbility>();

            while (quantity > 0 && availableAbilities.Count > 0)
            {
                if (CanHaveAllAvailableAbilities(quantity, bonusSum, availableAbilities))
                {
                    var strongestAbilities = GetStrongestAvailableAbilities(availableAbilities);
                    var duplicates = abilities.Where(a => strongestAbilities.Any(sA => sA.BaseName == a.BaseName));
                    abilities = abilities.Except(duplicates).ToList();

                    abilities.AddRange(strongestAbilities);
                    availableAbilities.Clear();
                    continue;
                }

                var ability = GenerateAbilityFrom(availableAbilities, tableNames);
                if (ability.Name == "BonusSpecialAbility")
                {
                    quantity++;
                    continue;
                }

                if (abilities.Any(a => a.BaseName == ability.BaseName))
                {
                    var previousAbility = abilities.First(a => a.BaseName == ability.BaseName);
                    bonusSum -= previousAbility.BonusEquivalent;
                    abilities.Remove(previousAbility);
                }

                quantity--;
                bonusSum += ability.BonusEquivalent;
                abilities.Add(ability);
                availableAbilities.Remove(ability);

                var weakerAbilities = availableAbilities.Where(a => a.BaseName == ability.BaseName && a.Power < ability.Power);
                availableAbilities = availableAbilities.Except(weakerAbilities).ToList();

                var tooStrongAbilities = availableAbilities.Where(a => a.BonusEquivalent + bonusSum > 10);
                availableAbilities = availableAbilities.Except(tooStrongAbilities).ToList();
            }

            return abilities;
        }

        private IEnumerable<string> GetTableNames(Item targetItem, string power)
        {
            var tableNames = new List<string>();

            if (targetItem.Attributes.Contains(AttributeConstants.Melee))
            {
                var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Melee);
                tableNames.Add(tableName);
            }

            if (targetItem.Attributes.Contains(AttributeConstants.Ranged))
            {
                var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Ranged);
                tableNames.Add(tableName);
            }

            if (targetItem.Attributes.Contains(AttributeConstants.Shield))
            {
                var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, AttributeConstants.Shield);
                tableNames.Add(tableName);
            }
            else if (targetItem.ItemType == ItemTypeConstants.Armor)
            {
                var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, power, ItemTypeConstants.Armor);
                tableNames.Add(tableName);
            }

            return tableNames;
        }

        private List<SpecialAbility> GetAvailableAbilities(Item targetItem, IEnumerable<string> tableNames, int bonus)
        {
            var availableAbilities = new List<SpecialAbility>();
            var weapon = targetItem as Weapon;

            foreach (var tableName in tableNames)
            {
                var abilityNames = percentileSelector.SelectAllFrom(Config.Name, tableName);

                foreach (var abilityName in abilityNames)
                {
                    if (abilityName == "BonusSpecialAbility")
                        continue;

                    var ability = GetSpecialAbility(abilityName, weapon.CriticalMultiplier);
                    if (ability.RequirementsMet(targetItem) && bonus + ability.BonusEquivalent <= 10)
                        availableAbilities.Add(ability);
                }
            }

            return availableAbilities;
        }

        private SpecialAbility GetSpecialAbility(string abilityName, string criticalMultiplier)
        {
            var ability = new SpecialAbility();
            var abilitySelection = specialAbilityDataSelector.SelectFrom(abilityName);

            ability.Name = abilityName;
            ability.BaseName = abilitySelection.BaseName;
            ability.AttributeRequirements = collectionsSelector.SelectFrom(
                Config.Name,
                TableNameConstants.Collections.Set.SpecialAbilityAttributeRequirements,
                ability.BaseName);
            ability.BonusEquivalent = abilitySelection.BonusEquivalent;
            ability.Power = abilitySelection.Power;

            var damagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.AbilityDamages, abilityName).ToArray();
            ability.Damages.AddRange(damagesData.Select(Damage.From));

            var critDamagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.AbilityDamages, abilityName + criticalMultiplier).ToArray();
            ability.CriticalDamages.AddRange(critDamagesData.Select(Damage.From));

            return ability;
        }

        private bool CanHaveAllAvailableAbilities(int quantity, int bonusSum, IEnumerable<SpecialAbility> availableAbilities)
        {
            return quantity >= availableAbilities.Count() && availableAbilities.Sum(a => a.BonusEquivalent) + bonusSum <= MaxBonus;
        }

        private IEnumerable<SpecialAbility> GetStrongestAvailableAbilities(IEnumerable<SpecialAbility> availableAbilities)
        {
            var strongestAbilities = new List<SpecialAbility>();

            foreach (var ability in availableAbilities)
            {
                //INFO: This means it is a custom special ability
                if (!specialAbilityDataSelector.IsSpecialAbility(ability.Name))
                {
                    strongestAbilities.Add(ability);
                    continue;
                }

                var alreadyAdded = strongestAbilities.Any(a => a.BaseName == ability.BaseName && a.Power == ability.Power && a.Name == ability.Name);
                if (alreadyAdded)
                    continue;

                var max = availableAbilities.Where(a => a.BaseName == ability.BaseName).Max(a => a.Power);

                if (ability.Power == max)
                    strongestAbilities.Add(ability);
            }

            return strongestAbilities;
        }

        private SpecialAbility GenerateAbilityFrom(IEnumerable<SpecialAbility> availableAbilities, IEnumerable<string> tableNames)
        {
            var abilityName = string.Empty;

            do
            {
                var tableName = collectionsSelector.SelectRandomFrom(tableNames);

                abilityName = percentileSelector.SelectFrom(Config.Name, tableName);

                if (abilityName == "BonusSpecialAbility")
                    return new SpecialAbility { Name = abilityName };
            } while (!availableAbilities.Any(a => a.Name == abilityName));

            return availableAbilities.First(a => a.Name == abilityName);
        }

        public IEnumerable<SpecialAbility> GenerateFor(IEnumerable<SpecialAbility> abilityPrototypes, string criticalMultiplier)
        {
            var abilities = new List<SpecialAbility>();

            foreach (var abilityPrototype in abilityPrototypes)
            {
                if (specialAbilityDataSelector.IsSpecialAbility(abilityPrototype.Name))
                {
                    var ability = GetSpecialAbility(abilityPrototype.Name, criticalMultiplier);
                    abilities.Add(ability);
                }
                else
                {
                    abilities.Add(abilityPrototype);
                }
            }

            var strongestAbilities = GetStrongestAvailableAbilities(abilities);
            return strongestAbilities;
        }
    }
}