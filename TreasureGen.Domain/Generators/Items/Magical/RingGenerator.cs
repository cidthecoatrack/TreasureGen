﻿using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Selectors.Attributes;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class RingGenerator : MagicalItemGenerator
    {
        private IPercentileSelector percentileSelector;
        private ICollectionsSelector collectionsSelector;
        private ISpellGenerator spellGenerator;
        private IChargesGenerator chargesGenerator;
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private Generator generator;

        public RingGenerator(
            IPercentileSelector percentileSelector,
            ICollectionsSelector collectionsSelector,
            ISpellGenerator spellGenerator,
            IChargesGenerator chargesGenerator,
            ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
            Generator generator)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.spellGenerator = spellGenerator;
            this.chargesGenerator = chargesGenerator;
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.generator = generator;
        }

        public Item GenerateAtPower(string power)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);

            var ring = BuildRing(result.Type, power);
            ring.Magic.Bonus = result.Amount;

            return ring;
        }

        private Item BuildRing(string name, string power)
        {
            var ring = new Item();
            ring.Name = name;
            ring.BaseNames = new[] { name };
            ring.IsMagical = true;
            ring.ItemType = ItemTypeConstants.Ring;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ring.ItemType);
            ring.Attributes = collectionsSelector.SelectFrom(tableName, name);

            if (ring.Attributes.Contains(AttributeConstants.Charged))
                ring.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Ring, name);

            if (ring.Name == RingConstants.Counterspells)
            {
                var level = spellGenerator.GenerateLevel(power);
                if (level <= 6)
                {
                    var type = spellGenerator.GenerateType();
                    var spell = spellGenerator.Generate(type, level);
                    ring.Contents.Add(spell);
                }
            }
            else if (ring.Name == RingConstants.SpellStoring_Minor)
            {
                var spells = GenerateSpells(power, 3);
                ring.Contents.AddRange(spells);
            }
            else if (ring.Name == RingConstants.SpellStoring_Major)
            {
                var spells = GenerateSpells(power, 10);
                ring.Contents.AddRange(spells);
            }
            else if (ring.Name == RingConstants.SpellStoring)
            {
                var spells = GenerateSpells(power, 5);
                ring.Contents.AddRange(spells);
            }

            return ring;
        }

        private IEnumerable<string> GenerateSpells(string power, int levelCap)
        {
            var level = spellGenerator.GenerateLevel(power);
            var levelSum = level;
            var spells = new List<string>();

            while (levelSum <= levelCap)
            {
                var type = spellGenerator.GenerateType();
                var spell = spellGenerator.Generate(type, level);
                var formattedSpell = string.Format("{0} ({1}, {2})", spell, type, level);
                spells.Add(formattedSpell);

                level = spellGenerator.GenerateLevel(power);
                levelSum += level;
            }

            return spells;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var ring = template.Clone();
            ring.BaseNames = new[] { ring.Name };
            ring.ItemType = ItemTypeConstants.Ring;
            ring.Quantity = 1;
            ring.IsMagical = true;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            ring.Attributes = collectionsSelector.SelectFrom(tableName, ring.Name);

            return ring.SmartClone();
        }

        public Item GenerateFromSubset(string power, IEnumerable<string> subset)
        {
            var ring = generator.Generate(
                () => GenerateAtPower(power),
                r => subset.Any(n => r.NameMatches(n)),
                () => CreateDefaultRing(power, subset),
                $"Ring from [{string.Join(", ", subset)}]");

            return ring;
        }

        private Item CreateDefaultRing(string power, IEnumerable<string> subset)
        {
            var name = collectionsSelector.SelectRandomFrom(subset);
            var item = BuildRing(name, power);
            var result = GetResult(power, item.Name);

            item.Magic.Bonus = result.Amount;

            return item;
        }

        private TypeAndAmountPercentileResult GetResult(string power, string name)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            var results = typeAndAmountPercentileSelector.SelectAllFrom(tableName);
            var result = results.FirstOrDefault(r => r.Type == name);

            if (result != null)
                return result;

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Minor, ItemTypeConstants.Ring);
            var minorResults = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Ring);
            var mediumResults = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Ring);
            var majorResults = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            results = minorResults.Union(mediumResults).Union(majorResults);
            result = results.FirstOrDefault(r => r.Type == name);

            //INFO: This means the ring name replaces some fillable field such as ALIGNMENT, so we will assume a bonus of 0
            if (result == null)
                return new TypeAndAmountPercentileResult();

            return result;
        }
    }
}