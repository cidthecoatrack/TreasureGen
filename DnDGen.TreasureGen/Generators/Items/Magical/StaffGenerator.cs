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
    internal class StaffGenerator : MagicalItemGenerator
    {
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public StaffGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
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
            var rodPowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Staff);
            var adjustedPower = PowerHelper.AdjustPower(power, rodPowers);

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, adjustedPower, ItemTypeConstants.Staff);
            var selection = typeAndAmountPercentileSelector.SelectFrom(tablename);

            return GenerateStaff(selection.Type, selection.Amount);
        }

        private Item GenerateStaff(string name, int bonus, params string[] traits)
        {
            var staff = new Item();
            staff.Name = name;
            staff.Magic.Bonus = bonus;
            staff.Traits = new HashSet<string>(traits);

            staff = BuildStaff(staff);
            staff.Magic.Charges = chargesGenerator.GenerateFor(staff.ItemType, name);

            return staff;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var staffName = GetStaffName(itemName);
            var powers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.PowerGroups, staffName);
            var adjustedPower = PowerHelper.AdjustPower(power, powers);

            var tablename = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, adjustedPower, ItemTypeConstants.Staff);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(tablename);
            var matches = selections.Where(s => s.Type == staffName).ToList();

            var selection = collectionsSelector.SelectRandomFrom(matches);
            return GenerateStaff(selection.Type, selection.Amount, traits);
        }

        private string GetStaffName(string itemName)
        {
            var staffs = StaffConstants.GetAllStaffs();
            if (staffs.Contains(itemName))
                return itemName;

            var staffFromBaseName = collectionsSelector.FindCollectionOf(Config.Name, TableNameConstants.Collections.Set.ItemGroups, itemName, staffs.ToArray());

            return staffFromBaseName;
        }

        private Item BuildStaff(Item staff)
        {
            staff.ItemType = ItemTypeConstants.Staff;
            staff.Attributes = new[] { AttributeConstants.Charged, AttributeConstants.OneTimeUse };
            staff.Quantity = 1;
            staff.IsMagical = true;
            staff.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, staff.Name);

            staff = GetWeapon(staff);

            return staff;
        }

        private Item GetWeapon(Item staff)
        {
            var weapons = WeaponConstants.GetAllMelee(false, false);
            if (!weapons.Intersect(staff.BaseNames).Any())
                return staff;

            var weaponName = weapons.Intersect(staff.BaseNames).First();

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, staff.Traits.ToArray()) as Weapon;

            staff.Attributes = staff.Attributes.Union(mundaneWeapon.Attributes).Except(new[] { AttributeConstants.OneTimeUse });
            staff.CloneInto(mundaneWeapon);

            if (mundaneWeapon.IsMagical)
                mundaneWeapon.Traits.Add(TraitConstants.Masterwork);

            if (mundaneWeapon.IsDoubleWeapon)
            {
                mundaneWeapon.SecondaryHasAbilities = true;
                mundaneWeapon.SecondaryMagicBonus = staff.Magic.Bonus;
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
            var staff = template.Clone();

            staff.Magic.Intelligence = template.Magic.Intelligence.Clone();
            staff.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(template.Magic.SpecialAbilities);

            staff = BuildStaff(staff);

            return staff.SmartClone();
        }
    }
}