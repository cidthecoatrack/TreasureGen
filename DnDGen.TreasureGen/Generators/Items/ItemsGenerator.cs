﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Generators.Items
{
    internal class ItemsGenerator : IItemsGenerator
    {
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly IRangeDataSelector rangeDataSelector;
        private readonly ICollectionSelector collectionSelector;

        public ItemsGenerator(
            ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
            JustInTimeFactory justInTimeFactory,
            ITreasurePercentileSelector percentileSelector,
            IRangeDataSelector rangeDataSelector,
            ICollectionSelector collectionSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.justInTimeFactory = justInTimeFactory;
            this.percentileSelector = percentileSelector;
            this.rangeDataSelector = rangeDataSelector;
            this.collectionSelector = collectionSelector;
        }

        public IEnumerable<Item> GenerateRandomAtLevel(int level)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, level);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);
            var items = new List<Item>();

            while (result.Amount-- > 0)
            {
                var item = GenerateRandomAtPower(result.Type);
                items.Add(item);
            }

            var epicItems = GetEpicItems(level);
            items.AddRange(epicItems);

            return items;
        }

        private IEnumerable<Item> GetEpicItems(int level)
        {
            var epicItems = new List<Item>();
            var majorItemQuantity = rangeDataSelector.SelectFrom(TableNameConstants.Collections.Set.EpicItems, level.ToString()).Minimum;

            while (majorItemQuantity-- > 0)
            {
                var epicItem = GenerateRandomAtPower(PowerConstants.Major);
                epicItems.Add(epicItem);
            }

            return epicItems;
        }

        public async Task<IEnumerable<Item>> GenerateRandomAtLevelAsync(int level)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, level);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);
            var tasks = new List<Task<Item>>();

            while (result.Amount-- > 0)
            {
                var task = Task.Run(() => GenerateRandomAtPower(result.Type));
                tasks.Add(task);
            }

            var epicItemTasks = GetEpicItemTasks(level);
            tasks.AddRange(epicItemTasks);

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result);
        }

        private IEnumerable<Task<Item>> GetEpicItemTasks(int level)
        {
            var epicItemTasks = new List<Task<Item>>();
            var majorItemQuantity = rangeDataSelector.SelectFrom(TableNameConstants.Collections.Set.EpicItems, level.ToString()).Minimum;

            while (majorItemQuantity-- > 0)
            {
                var task = Task.Run(() => GenerateRandomAtPower(PowerConstants.Major));
                epicItemTasks.Add(task);
            }

            return epicItemTasks;
        }

        private Item GenerateRandomAtPower(string power)
        {
            if (power == PowerConstants.Mundane)
                return GenerateRandomMundaneItem();

            return GenerateRandomMagicalItemAtPower(power);
        }

        private Item GenerateRandomMundaneItem()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Mundane);
            var itemType = percentileSelector.SelectFrom(tableName);

            return GenerateMundaneItem(itemType);
        }

        private Item GenerateRandomMagicalItemAtPower(string power)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, power);
            var itemType = percentileSelector.SelectFrom(tableName);

            return GenerateMagicalItemAtPower(power, itemType);
        }

        public Item GenerateAtLevel(int level, string itemType, string itemName, params string[] traits)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, level);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);
            var powers = collectionSelector.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, itemName);
            var power = PowerHelper.AdjustPower(result.Type, powers);

            if (power == PowerConstants.Mundane)
                return GenerateMundaneItem(itemType, itemName, traits);

            return GenerateMagicalItemAtPower(power, itemType, itemName, traits);
        }

        public async Task<Item> GenerateAtLevelAsync(int level, string itemType, string itemName, params string[] traits) =>
            await Task.Run(() => GenerateAtLevel(level, itemType, itemName, traits));

        private Item GenerateMundaneItem(string itemType, string itemName = null, params string[] traits)
        {
            var generator = justInTimeFactory.Build<MundaneItemGenerator>(itemType);

            if (string.IsNullOrEmpty(itemName))
                return generator.GenerateRandom();

            return generator.Generate(itemName, traits);
        }

        private Item GenerateMagicalItemAtPower(string power, string itemType, string itemName = null, params string[] traits)
        {
            var magicalItemGenerator = justInTimeFactory.Build<MagicalItemGenerator>(itemType);

            if (string.IsNullOrEmpty(itemName))
                return magicalItemGenerator.GenerateRandom(power);

            return magicalItemGenerator.Generate(power, itemName, traits);
        }
    }
}