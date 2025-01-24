using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificShieldAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, AttributeConstants.Shield); }
        }

        [TestCase(ArmorConstants.AbsorbingShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.Buckler,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.HeavyWoodenShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.HeavySteelShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.CastersShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.SpinedShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.LionsShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.WingedShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        public void SpecificShieldAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}