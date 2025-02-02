using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class ArmorDataSelectorTests
    {
        private IArmorDataSelector armorDataSelector;
        private Mock<ICollectionSelector> mockInnerSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionSelector>();
            armorDataSelector = new ArmorDataSelector(mockInnerSelector.Object);
        }

        [Test]
        public void GetArmorData()
        {
            var data = new string[3];
            data[DataIndexConstants.Armor.ArmorBonus] = "9266";
            data[DataIndexConstants.Armor.ArmorCheckPenalty] = "-90210";
            data[DataIndexConstants.Armor.MaxDexterityBonus] = "42";

            mockInnerSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(data);

            var selection = armorDataSelector.Select("armor");
            Assert.That(selection.ArmorBonus, Is.EqualTo(9266));
            Assert.That(selection.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(selection.MaxDexterityBonus, Is.EqualTo(42));
        }
    }
}
