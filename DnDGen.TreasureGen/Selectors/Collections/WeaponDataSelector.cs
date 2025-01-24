using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System.Linq;

namespace DnDGen.TreasureGen.Selectors.Collections
{
    internal class WeaponDataSelector : IWeaponDataSelector
    {
        private readonly ICollectionDataSelector<DamageDataSelection> damageDataSelector;
        private readonly ICollectionDataSelector<WeaponDataSelection> weaponDataSelector;

        public WeaponDataSelector(ICollectionDataSelector<DamageDataSelection> damageDataSelector, ICollectionDataSelector<WeaponDataSelection> weaponDataSelector)
        {
            this.damageDataSelector = damageDataSelector;
            this.weaponDataSelector = weaponDataSelector;
        }

        public WeaponDataSelection Select(string name, string size)
        {
            var weaponData = weaponDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.WeaponData, name).Single();
            var damagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.WeaponDamages, name + size).ToArray();
            var critDamagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.WeaponCriticalDamages, name + size).ToArray();

            weaponData.Damages.AddRange(damagesData);
            weaponData.CriticalDamages.AddRange(critDamagesData);

            return weaponData;
        }
    }
}
