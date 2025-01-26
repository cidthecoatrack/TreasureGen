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
            Bind<IRangeDataSelector>().To<RangeDataSelector>();
            Bind<ITreasurePercentileSelector>().To<PercentileSelectorStringReplacementDecorator>();
            Bind<IReplacementSelector>().To<ReplacementSelector>();

            Kernel.BindDataSelection<ArmorDataSelection>();
            Kernel.BindDataSelection<WeaponDataSelection>();
            Kernel.BindDataSelection<DamageDataSelection>();
            Kernel.BindDataSelection<SpecialAbilityDataSelection>();
            Kernel.BindDataSelection<IntelligenceDataSelection>();
        }
    }
}