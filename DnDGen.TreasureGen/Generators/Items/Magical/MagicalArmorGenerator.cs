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
    internal class MagicalArmorGenerator : MagicalItemGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly ISpecificGearGenerator specificGearGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        private const string SpecialAbility = "SpecialAbility";

        public MagicalArmorGenerator(
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            ISpecificGearGenerator specificGearGenerator,
            JustInTimeFactory justInTimeFactory)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.specificGearGenerator = specificGearGenerator;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Item GenerateRandom(string power)
        {
            var nameResult = GenerateRandomName(power);
            return GenerateArmor(power, nameResult.Name, nameResult.ArmorType, false);
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var armorType = GetArmorType(itemName);
            var isSpecific = specificGearGenerator.IsSpecific(armorType, itemName);

            return GenerateArmor(power, itemName, armorType, isSpecific, traits);
        }

        private string GetArmorType(string itemName)
        {
            if (specificGearGenerator.IsSpecific(AttributeConstants.Shield, itemName))
            {
                return AttributeConstants.Shield;
            }

            if (specificGearGenerator.IsSpecific(ItemTypeConstants.Armor, itemName))
            {
                return ItemTypeConstants.Armor;
            }

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            var attributes = collectionsSelector.SelectFrom(Config.Name, tableName, itemName);

            if (attributes.Contains(AttributeConstants.Shield))
            {
                return AttributeConstants.Shield;
            }

            return ItemTypeConstants.Armor;
        }

        private Item GenerateArmor(string power, string itemName, string armorType, bool isSpecific, params string[] traits)
        {
            var prototype = GeneratePrototype(power, itemName, armorType, isSpecific, traits);
            var armor = GenerateFromPrototype(prototype);

            if (!specificGearGenerator.IsSpecific(armor))
            {
                var abilityCount = armor.Magic.SpecialAbilities.Count();
                armor.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(armor, power, abilityCount);
            }

            return armor;
        }

        private (string Name, string ArmorType) GenerateRandomName(string power)
        {
            var armorTypeTableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, power, ItemTypeConstants.Armor);
            var armorType = percentileSelector.SelectFrom(Config.Name, armorTypeTableName);

            var nameTableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, armorType);
            var name = percentileSelector.SelectFrom(Config.Name, nameTableName);

            return (name, armorType);
        }

        private Armor GeneratePrototype(string power, string itemName, string armorType, bool isSpecific, params string[] traits)
        {
            var prototype = new Armor();

            if (isSpecific)
            {
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, armorType, itemName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            var canBeSpecific = specificGearGenerator.CanBeSpecific(power, armorType, itemName);
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            var bonus = string.Empty;
            var abilityCount = 0;

            do bonus = percentileSelector.SelectFrom(Config.Name, tableName);
            while (!canBeSpecific && bonus == ItemTypeConstants.Armor);

            while (bonus == SpecialAbility)
            {
                abilityCount++;

                do bonus = percentileSelector.SelectFrom(Config.Name, tableName);
                while (!canBeSpecific && bonus == ItemTypeConstants.Armor);
            }

            prototype.Traits = new HashSet<string>(traits);

            if (bonus == ItemTypeConstants.Armor && canBeSpecific)
            {
                var specificName = specificGearGenerator.GenerateNameFrom(power, armorType, itemName);
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, armorType, specificName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            prototype.Name = itemName;
            prototype.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, itemName);
            prototype.Magic.Bonus = Convert.ToInt32(bonus);
            prototype.Magic.SpecialAbilities = Enumerable.Repeat(new SpecialAbility(), abilityCount);

            return prototype;
        }

        private Armor GenerateFromPrototype(Armor prototype)
        {
            if (specificGearGenerator.IsSpecific(prototype))
            {
                var specificArmor = specificGearGenerator.GenerateFrom(prototype);
                specificArmor.Quantity = 1;

                return specificArmor as Armor;
            }

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.Generate(prototype);

            armor.Magic.Bonus = prototype.Magic.Bonus;
            armor.Magic.Charges = prototype.Magic.Charges;
            armor.Magic.Curse = prototype.Magic.Curse;
            armor.Magic.Intelligence = prototype.Magic.Intelligence;
            armor.Magic.SpecialAbilities = prototype.Magic.SpecialAbilities;

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            return armor as Armor;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var armorTemplate = new Armor();
            template.CloneInto(armorTemplate);
            armorTemplate.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            armorTemplate.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, armorTemplate.Name);

            var armor = GenerateFromPrototype(armorTemplate);

            armor.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(template.Magic.SpecialAbilities);

            return armor;
        }
    }
}