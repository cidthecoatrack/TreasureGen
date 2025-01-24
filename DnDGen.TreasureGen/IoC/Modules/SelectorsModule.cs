using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using Ninject.Modules;

namespace DnDGen.TreasureGen.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IIntelligenceDataSelector>().To<IntelligenceDataSelector>();
            Bind<IRangeDataSelector>().To<RangeDataSelector>();
            Bind<ITreasurePercentileSelector>().To<PercentileSelectorStringReplacementDecorator>();
            Bind<IReplacementSelector>().To<ReplacementSelector>();
            Bind<ITypeAndAmountPercentileSelector>().To<TypeAndAmountPercentileSelector>();
            Bind<ISpecialAbilityDataSelector>().To<SpecialAbilityDataSelector>();

            Kernel.BindDataSelection<ArmorDataSelection>();
            Kernel.BindDataSelection<WeaponDataSelection>();
            Kernel.BindDataSelection<DamageDataSelection>();
        }
    }
}