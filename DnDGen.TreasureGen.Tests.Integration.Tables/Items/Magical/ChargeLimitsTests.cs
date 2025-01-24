using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class ChargeLimitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.Set.ChargeLimits; }
        }

        [TestCase(WondrousItemConstants.BraceletOfFriends, 1, 4)]
        [TestCase(WondrousItemConstants.BroochOfShielding, 1, 101)]
        [TestCase(WondrousItemConstants.ChimeOfOpening, 1, 10)]
        [TestCase(WondrousItemConstants.DeckOfIllusions, 14, 33)]
        [TestCase(WondrousItemConstants.GemOfBrightness, 1, 50)]
        [TestCase(WondrousItemConstants.KeoghtomsOintment, 1, 5)]
        [TestCase(WondrousItemConstants.ScarabOfProtection, 1, 12)]
        [TestCase(RingConstants.Ram, 1, 50)]
        [TestCase(WeaponConstants.LuckBlade0, 0, 0)]
        [TestCase(WeaponConstants.LuckBlade1, 1, 1)]
        [TestCase(WeaponConstants.LuckBlade2, 2, 2)]
        [TestCase(WeaponConstants.LuckBlade3, 3, 3)]
        [TestCase(WeaponConstants.LuckBlade, 0, 3)]
        [TestCase(WondrousItemConstants.DeckOfIllusions_Full, 34, 34)]
        [TestCase(RingConstants.ThreeWishes, 1, 3)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_I, 1, 3)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_II, 1, 5)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_III, 1, 7)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_IV, 1, 9)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_V, 1, 7)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VI, 1, 9)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VII, 1, 9)]
        [TestCase(WondrousItemConstants.RobeOfBones, 1, 12)]
        [TestCase(RodConstants.Absorption, 1, 50)]
        [TestCase(RodConstants.Absorption_Full, 50, 50)]
        [TestCase(RodConstants.Rulership, 1, 500)]
        public void ChargeLimits(string name, int min, int max)
        {
            var limits = new[] { min.ToString(), max.ToString() };
            base.AssertOrderedCollections(name, limits);
        }
    }
}