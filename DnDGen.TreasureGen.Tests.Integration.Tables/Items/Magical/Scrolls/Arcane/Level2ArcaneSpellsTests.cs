using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level2ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.LevelXSPELLTYPESpells, 2, "Arcane"); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase("Animal Messenger", 1)]
        [TestCase("Animal Trance", 2)]
        [TestCase("Arcane Lock", 3)]
        [TestCase("Calm Emotions", 14)]
        [TestCase("Continual Flame", 20)]
        [TestCase("Cure Moderate Wounds", 21)]
        [TestCase("Darkness", 22)]
        [TestCase("Daze Monster", 26)]
        [TestCase("Delay Poison", 27)]
        [TestCase("Enthrall", 35)]
        [TestCase("Fog Cloud", 40)]
        [TestCase("Ghoul Touch", 44)]
        [TestCase("Gust of Wind", 47)]
        [TestCase("Leomund's Trap", 56)]
        [TestCase("Locate Object", 59)]
        [TestCase("Magic Mouth", 60)]
        [TestCase("Minor Image", 63)]
        [TestCase("Misdirection", 66)]
        [TestCase("Obscure Object", 67)]
        [TestCase("Rope Trick", 79)]
        [TestCase("Scare", 80)]
        [TestCase("Shatter", 86)]
        [TestCase("Silence", 87)]
        [TestCase("Sound Burst", 88)]
        [TestCase("Spectral Hand", 89)]
        [TestCase("Tasha's Hideous Laughter", 96)]
        [TestCase("Touch of Idiocy", 97)]
        [TestCase("Whispering Wind", 100)]
        public override void AssertPercentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }

        [TestCase("Bear's Endurance", 4, 6)]
        [TestCase("Blindness/Deafness", 7, 8)]
        [TestCase("Blur", 9, 10)]
        [TestCase("Bull's Strength", 11, 13)]
        [TestCase("Cat's Grace", 15, 17)]
        [TestCase("Command Undead", 18, 19)]
        [TestCase("Darkvision", 23, 25)]
        [TestCase("Detect Thoughts", 28, 29)]
        [TestCase("Disguise Self", 30, 31)]
        [TestCase("Eagle's Splendor", 32, 34)]
        [TestCase("False Life", 36, 37)]
        [TestCase("Flaming Sphere", 38, 39)]
        [TestCase("Fox's Cunning", 41, 43)]
        [TestCase("Glitterdust", 45, 46)]
        [TestCase("Hypnotic Pattern", 48, 49)]
        [TestCase("Invisibility", 50, 52)]
        [TestCase("Knock", 53, 55)]
        [TestCase("Levitate", 57, 58)]
        [TestCase("Melf's Acid Arrow", 61, 62)]
        [TestCase("Mirror Image", 64, 65)]
        [TestCase("Owl's Wisdom", 68, 70)]
        [TestCase("Protection from Arrows", 71, 73)]
        [TestCase("Pyrotechnics", 74, 75)]
        [TestCase("Resist Energy", 76, 78)]
        [TestCase("Scorching Ray", 81, 82)]
        [TestCase("See Invisibility", 83, 85)]
        [TestCase("Spider Climb", 90, 91)]
        [TestCase("Summon Monster II", 92, 93)]
        [TestCase("Summon Swarm", 94, 95)]
        [TestCase("Web", 98, 99)]
        public override void AssertPercentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}