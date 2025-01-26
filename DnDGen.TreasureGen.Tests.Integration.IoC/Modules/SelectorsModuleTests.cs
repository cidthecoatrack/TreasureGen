using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class SelectorsModuleTests : IoCTests
    {
        [Test]
        public void TypeAndAmountPercentileSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ITypeAndAmountPercentileSelector>();
        }

        [Test]
        public void TreasurePercentileSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ITreasurePercentileSelector>();
        }

        [Test]
        public void TreasurePercentileSelectorHasReplacementDecorator()
        {
            var selector = GetNewInstanceOf<ITreasurePercentileSelector>();
            Assert.That(selector, Is.InstanceOf<PercentileSelectorStringReplacementDecorator>());
        }

        [Test]
        public void CollectionData_SpecialAbilityDataSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<SpecialAbilityDataSelection>>();
        }

        [Test]
        public void IntelligenceDataSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<IIntelligenceDataSelector>();
        }

        [Test]
        public void RangeDataSelectorNotConstructedAsSingleton()
        {
            AssertNotSingleton<IRangeDataSelector>();
        }

        [Test]
        public void CollectionData_ArmorDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<ArmorDataSelection>>();
        }

        [Test]
        public void CollectionData_WeaponDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<WeaponDataSelection>>();
        }

        [Test]
        public void WeaponDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<IWeaponDataSelector>();
        }

        [Test]
        public void DamageDataSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<ICollectionDataSelector<DamageDataSelection>>();
        }

        [Test]
        public void ReplacementSelectorIsNotConstructedAsSingleton()
        {
            AssertNotSingleton<IReplacementSelector>();
        }
    }
}