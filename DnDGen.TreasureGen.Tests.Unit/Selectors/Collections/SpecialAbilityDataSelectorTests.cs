using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class SpecialAbilityDataSelectorTests
    {
        private ISpecialAbilityDataSelector selector;
        private Mock<ICollectionSelector> mockInnerSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionSelector>();
            selector = new SpecialAbilityDataSelector(mockInnerSelector.Object);
        }

        [Test]
        public void ReturnSpecialAbilityAttributesResult()
        {
            var attributes = new[] { "42", "base name", "9266" };
            mockInnerSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.Set.SpecialAbilityData, "name")).Returns(attributes);

            var result = selector.SelectFrom("name");
            Assert.That(result.BaseName, Is.EqualTo("base name"));
            Assert.That(result.BonusEquivalent, Is.EqualTo(42));
            Assert.That(result.Power, Is.EqualTo(9266));
        }

        [Test]
        public void ThrowErrorIfFewerThan3Attributes()
        {
            var attributes = new[] { "base name", "42" };
            mockInnerSelector.Setup(s => s.SelectFrom(Config.Name, It.IsAny<string>(), It.IsAny<string>())).Returns(attributes);
            Assert.That(() => selector.SelectFrom("name"), Throws.Exception.With.Message.EqualTo("Data is not formatted for special abilities"));
        }

        [Test]
        public void IsSpecialAbility()
        {
            mockInnerSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.Set.SpecialAbilityData, "name")).Returns(true);
            var isSpecialAbility = selector.IsSpecialAbility("name");
            Assert.That(isSpecialAbility, Is.True);
        }

        [Test]
        public void IsNotSpecialAbility()
        {
            mockInnerSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.Set.SpecialAbilityData, "name")).Returns(false);
            var isSpecialAbility = selector.IsSpecialAbility("name");
            Assert.That(isSpecialAbility, Is.False);
        }
    }
}