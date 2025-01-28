using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
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
    internal class MagicalWeaponGenerator : MagicalItemGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly ISpecificGearGenerator specificGearGenerator;
        private readonly ISpellGenerator spellGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        private const string SpecialAbility = "SpecialAbility";

        public MagicalWeaponGenerator(
            ICollectionSelector collectionsSelector,
            ITreasurePercentileSelector percentileSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            ISpecificGearGenerator specificGearGenerator,
            ISpellGenerator spellGenerator,
            JustInTimeFactory justInTimeFactory)
        {
            this.collectionsSelector = collectionsSelector;
            this.percentileSelector = percentileSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.specificGearGenerator = specificGearGenerator;
            this.spellGenerator = spellGenerator;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Item GenerateRandom(string power)
        {
            var name = GenerateRandomName();
            return GenerateWeapon(power, name, false);
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var isSpecific = specificGearGenerator.IsSpecific(ItemTypeConstants.Weapon, itemName);
            return GenerateWeapon(power, itemName, isSpecific, traits);
        }

        private Item GenerateWeapon(string power, string name, bool isSpecific, params string[] traits)
        {
            var prototype = GeneratePrototype(power, name, isSpecific, traits);
            var weapon = GenerateFromPrototype(prototype, true);

            if (!specificGearGenerator.IsSpecific(prototype))
            {
                weapon = GenerateRandomSpecialAbilities(weapon, power);
            }

            return weapon;
        }

        private string GenerateRandomName()
        {
            var type = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MagicalWeaponTypes);
            var name = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.WEAPONTYPEWeapons(type));

            return name;
        }

        private Weapon GeneratePrototype(string power, string itemName, bool isSpecific, params string[] traits)
        {
            var prototype = new Weapon();

            if (isSpecific)
            {
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, itemName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            var canBeSpecific = specificGearGenerator.CanBeSpecific(power, ItemTypeConstants.Weapon, itemName);
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            var bonus = string.Empty;
            var specialAbilitiesCount = 0;

            do bonus = percentileSelector.SelectFrom(Config.Name, tableName);
            while (!canBeSpecific && bonus == ItemTypeConstants.Weapon);

            while (bonus == SpecialAbility)
            {
                specialAbilitiesCount++;

                do bonus = percentileSelector.SelectFrom(Config.Name, tableName);
                while (!canBeSpecific && bonus == ItemTypeConstants.Weapon);
            }

            prototype.Traits = new HashSet<string>(traits);

            if (bonus == ItemTypeConstants.Weapon && canBeSpecific)
            {
                var specificName = specificGearGenerator.GenerateNameFrom(power, ItemTypeConstants.Weapon, itemName);
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, specificName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            prototype.Name = itemName;
            prototype.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, itemName);
            prototype.Quantity = 0;
            prototype.Magic.Bonus = Convert.ToInt32(bonus);
            prototype.Magic.SpecialAbilities = Enumerable.Repeat(new SpecialAbility(), specialAbilitiesCount);
            prototype.ItemType = ItemTypeConstants.Weapon;

            return prototype;
        }

        private Weapon GenerateFromPrototype(Item prototype, bool allowDecoration)
        {
            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);

            if (specificGearGenerator.IsSpecific(prototype))
            {
                var specificWeapon = specificGearGenerator.GenerateFrom(prototype);

                //INFO: We need to set the quantity on the weapon. However, ammunition or thrown weapons might have quantities greater than 1
                //The quantity logic is contained within the MundaneWeaponGenerator, to which we do not have direct access
                //So, we will generate a mundane item from the base names of the specific weapon, and use that quantity
                var weaponForQuantity = mundaneWeaponGenerator.Generate(specificWeapon.BaseNames.First());
                specificWeapon.Quantity = weaponForQuantity.Quantity;

                return specificWeapon as Weapon;
            }

            var weapon = mundaneWeaponGenerator.Generate(prototype, allowDecoration);

            weapon.Magic.Bonus = prototype.Magic.Bonus;
            weapon.Magic.Charges = prototype.Magic.Charges;
            weapon.Magic.Curse = prototype.Magic.Curse;
            weapon.Magic.Intelligence = prototype.Magic.Intelligence;
            weapon.Magic.SpecialAbilities = prototype.Magic.SpecialAbilities;

            if (weapon.Attributes.Contains(AttributeConstants.Ammunition))
                weapon.Magic.Intelligence = new Intelligence();

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            var magicWeapon = weapon as Weapon;
            if (!magicWeapon.IsDoubleWeapon)
                return magicWeapon;

            var sameEnhancement = percentileSelector.SelectFrom(.5);
            if (!sameEnhancement)
            {
                magicWeapon.SecondaryMagicBonus = weapon.Magic.Bonus - 1;
                magicWeapon.SecondaryHasAbilities = false;

                return magicWeapon;
            }

            magicWeapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            magicWeapon.SecondaryHasAbilities = true;

            return magicWeapon;
        }

        private Weapon GenerateRandomSpecialAbilities(Weapon weapon, string power)
        {
            weapon.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(weapon, power, weapon.Magic.SpecialAbilities.Count());
            weapon = ApplySpecialAbilities(weapon);

            return weapon;
        }

        private Weapon ApplySpecialAbilities(Weapon weapon)
        {
            if (weapon.Magic.SpecialAbilities.Any(a => a.Name == SpecialAbilityConstants.SpellStoring))
            {
                var shouldStoreSpell = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell);

                if (shouldStoreSpell)
                {
                    var spellType = spellGenerator.GenerateType();
                    var level = spellGenerator.GenerateLevel(PowerConstants.Minor);
                    var spell = spellGenerator.Generate(spellType, level);

                    weapon.Contents.Add(spell);
                }
            }

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
            template.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, template.Name);

            var weapon = new Weapon();

            if (template is Weapon)
                weapon = template.Clone() as Weapon;
            else
                template.CloneInto(weapon);

            weapon = GenerateFromPrototype(weapon, allowRandomDecoration);

            if (weapon.Attributes.Contains(AttributeConstants.Specific))
            {
                //Double rule and special abilities were already applied within the specific gear generator
                return weapon;
            }

            if (weapon.IsDoubleWeapon && weapon.IsMagical)
            {
                weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
                weapon.SecondaryHasAbilities = true;
            }

            weapon.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(template.Magic.SpecialAbilities, weapon.CriticalMultiplier);
            weapon = ApplySpecialAbilities(weapon);

            return weapon;
        }
    }
}