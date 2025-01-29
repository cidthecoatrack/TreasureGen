using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class StaffGenerator : MagicalItemGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public StaffGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            IChargesGenerator chargesGenerator,
            ICollectionSelector collectionsSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            JustInTimeFactory justInTimeFactory)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.chargesGenerator = chargesGenerator;
            this.collectionsSelector = collectionsSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Item GenerateRandom(string power)
        {
            var rodPowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff);
            var adjustedPower = PowerHelper.AdjustPower(power, rodPowers);

            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Staff);
            var selection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, tablename);

            return GenerateStaff(selection.Type, selection.Amount);
        }

        private Item GenerateStaff(string name, int bonus, params string[] traits)
        {
            var staff = new Item
            {
                Name = name
            };
            staff.Magic.Bonus = bonus;
            staff.Traits = new HashSet<string>(traits);

            staff = BuildStaff(staff);
            staff.Magic.Charges = chargesGenerator.GenerateFor(staff.ItemType, name);

            return staff;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var staffName = GetStaffName(itemName);
            var powers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, staffName);
            var adjustedPower = PowerHelper.AdjustPower(power, powers);

            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Staff);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tablename);
            var matches = selections.Where(s => s.Type == staffName).ToList();

            var selection = collectionsSelector.SelectRandomFrom(matches);
            return GenerateStaff(selection.Type, selection.Amount, traits);
        }

        private string GetStaffName(string itemName)
        {
            var staffs = StaffConstants.GetAllStaffs();
            if (staffs.Contains(itemName))
                return itemName;

            var staffFromBaseName = collectionsSelector.FindCollectionOf(Config.Name, TableNameConstants.Collections.ItemGroups, itemName, staffs.ToArray());

            return staffFromBaseName;
        }

        private Item BuildStaff(Item staff)
        {
            staff.ItemType = ItemTypeConstants.Staff;
            staff.Attributes = [AttributeConstants.Charged, AttributeConstants.OneTimeUse];
            staff.Quantity = 1;
            staff.IsMagical = true;
            staff.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, staff.Name);

            staff = GetWeapon(staff);

            return staff;
        }

        private Item GetWeapon(Item staff)
        {
            var weapons = WeaponConstants.GetAllMelee(false, false);
            if (!weapons.Intersect(staff.BaseNames).Any())
                return staff;

            var weaponName = weapons.Intersect(staff.BaseNames).First();

            var weaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = weaponGenerator.Generate(weaponName, [.. staff.Traits]) as Weapon;

            staff.Attributes = staff.Attributes.Union(weapon.Attributes).Except([AttributeConstants.OneTimeUse]);
            staff.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            if (weapon.IsDoubleWeapon)
            {
                weapon.SecondaryHasAbilities = true;
                weapon.SecondaryMagicBonus = staff.Magic.Bonus;
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
            var staff = template.Clone();

            staff.Magic.Intelligence = template.Magic.Intelligence.Clone();
            staff.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(template.Magic.SpecialAbilities);

            staff = BuildStaff(staff);

            return staff.SmartClone();
        }
    }
}