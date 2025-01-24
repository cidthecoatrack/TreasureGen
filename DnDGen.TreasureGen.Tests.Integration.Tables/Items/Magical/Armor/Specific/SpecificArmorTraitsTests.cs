using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificArmorTraitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor); }
        }

        [TestCase(ArmorConstants.BandedMailOfLuck)]
        [TestCase(ArmorConstants.BreastplateOfCommand)]
        [TestCase(ArmorConstants.CelestialArmor)]
        [TestCase(ArmorConstants.DemonArmor)]
        [TestCase(ArmorConstants.DwarvenPlate, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(ArmorConstants.ElvenChain, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.FullPlateOfSpeed, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep)]
        [TestCase(ArmorConstants.RhinoHide)]
        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        [TestCase(ArmorConstants.ChainShirt, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.FullPlate, TraitConstants.SpecialMaterials.Dragonhide)]
        [TestCase(ArmorConstants.Breastplate, TraitConstants.SpecialMaterials.Adamantine)]
        public void SpecificArmorTraits(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}