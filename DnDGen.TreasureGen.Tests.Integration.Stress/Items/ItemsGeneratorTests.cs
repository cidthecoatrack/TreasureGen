using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items
{
    [TestFixture]
    public class ItemsGeneratorTests : StressTests
    {
        private IItemsGenerator itemsGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemsGenerator = GetNewInstanceOf<IItemsGenerator>();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void StressRandomItems()
        {
            stressor.Stress(GenerateAndAssertRandomItems);
        }

        private void GenerateAndAssertRandomItems()
        {
            var level = GetNewLevel();
            var items = itemsGenerator.GenerateRandomAtLevel(level);

            Assert.That(items, Is.Not.Null);

            if (level > 20)
                Assert.That(items, Is.Not.Empty, $"Level {level}");

            foreach (var item in items)
                itemVerifier.AssertItem(item);
        }

        [Test]
        public async Task StressRandomItemsAsync()
        {
            await stressor.StressAsync(GenerateAndAssertRandomItemsAsync);
        }

        private async Task GenerateAndAssertRandomItemsAsync()
        {
            var level = GetNewLevel();
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(level);

            Assert.That(items, Is.Not.Null);

            if (level > 20)
                Assert.That(items, Is.Not.Empty, $"Level {level}");

            foreach (var item in items)
                itemVerifier.AssertItem(item);
        }

        [TestCase(ItemTypeConstants.AlchemicalItem)]
        [TestCase(ItemTypeConstants.Armor)]
        [TestCase(ItemTypeConstants.Potion)]
        [TestCase(ItemTypeConstants.Ring)]
        [TestCase(ItemTypeConstants.Rod)]
        [TestCase(ItemTypeConstants.Scroll)]
        [TestCase(ItemTypeConstants.Staff)]
        [TestCase(ItemTypeConstants.Tool)]
        [TestCase(ItemTypeConstants.Wand)]
        [TestCase(ItemTypeConstants.Weapon)]
        [TestCase(ItemTypeConstants.WondrousItem)]
        public void StressNamedItemAtLevel(string itemType)
        {
            stressor.Stress(() => GenerateAndAssertNamedItemAtLevelItems(itemType));
        }

        private void GenerateAndAssertNamedItemAtLevelItems(string itemType)
        {
            var level = GetNewLevel();
            var itemName = GetRandomItemName(itemType);

            var item = itemsGenerator.GenerateAtLevel(level, itemType, itemName);
            itemVerifier.AssertItem(item);
        }

        private string GetRandomItemName(string itemType)
        {
            var itemNames = Enumerable.Empty<string>();

            switch (itemType)
            {
                case ItemTypeConstants.AlchemicalItem:
                    itemNames = AlchemicalItemConstants.GetAllAlchemicalItems(); break;
                case ItemTypeConstants.Armor:
                    itemNames = ArmorConstants.GetAllArmorsAndShields(true); break;
                case ItemTypeConstants.Potion:
                    itemNames = PotionConstants.GetAllPotions(true); break;
                case ItemTypeConstants.Ring:
                    itemNames = RingConstants.GetAllRings(); break;
                case ItemTypeConstants.Rod:
                    itemNames = RodConstants.GetAllRods(); break;
                case ItemTypeConstants.Scroll:
                    itemNames = new[] { $"Scroll {Guid.NewGuid()}" }; break;
                case ItemTypeConstants.Staff:
                    itemNames = StaffConstants.GetAllStaffs(); break;
                case ItemTypeConstants.Tool:
                    itemNames = ToolConstants.GetAllTools(); break;
                case ItemTypeConstants.Wand:
                    itemNames = new[] { $"Wand {Guid.NewGuid()}" }; break;
                case ItemTypeConstants.Weapon:
                    itemNames = WeaponConstants.GetAllWeapons(true, true); break;
                case ItemTypeConstants.WondrousItem:
                    itemNames = WondrousItemConstants.GetAllWondrousItems(); break;
            }

            var itemName = collectionSelector.SelectRandomFrom(itemNames);
            return itemName;
        }

        [TestCase(ItemTypeConstants.AlchemicalItem)]
        [TestCase(ItemTypeConstants.Armor)]
        [TestCase(ItemTypeConstants.Potion)]
        [TestCase(ItemTypeConstants.Ring)]
        [TestCase(ItemTypeConstants.Rod)]
        [TestCase(ItemTypeConstants.Scroll)]
        [TestCase(ItemTypeConstants.Staff)]
        [TestCase(ItemTypeConstants.Tool)]
        [TestCase(ItemTypeConstants.Wand)]
        [TestCase(ItemTypeConstants.Weapon)]
        [TestCase(ItemTypeConstants.WondrousItem)]
        public async Task StressNamedItemAtLevelAsync(string itemType)
        {
            await stressor.StressAsync(async () => await GenerateAndAssertNamedItemAtLevelAsync(itemType));
        }

        private async Task GenerateAndAssertNamedItemAtLevelAsync(string itemType)
        {
            var level = GetNewLevel();
            var itemName = GetRandomItemName(itemType);

            var item = await itemsGenerator.GenerateAtLevelAsync(level, itemType, itemName);
            itemVerifier.AssertItem(item);
        }
    }
}