using DnDGen.Infrastructure.Mappers.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables
{
    [TestFixture]
    public abstract class CollectionsTests : TableTests
    {
        protected CollectionMapper CollectionMapper;
        protected Dictionary<string, IEnumerable<string>> table;

        [SetUp]
        public void CollectionsSetup()
        {
            CollectionMapper = GetNewInstanceOf<CollectionMapper>();
            table = CollectionMapper.Map(Name, tableName);
        }

        protected IEnumerable<string> GetKeys()
        {
            return table.Keys;
        }

        protected IEnumerable<string> GetCollection(string name)
        {
            return table[name];
        }

        public virtual void AssertCollection(string name, params string[] items)
        {
            Assert.That(table.Keys, Contains.Item(name), tableName);
            AssertCollection(table[name], items);
        }

        protected void AssertCollection(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            Assert.That(actual, Is.EquivalentTo(expected));

            foreach (var item in expected)
            {
                var actualCount = actual.Count(a => a == item);
                var expectedCount = expected.Count(a => a == item);
                Assert.That(actualCount, Is.EqualTo(expectedCount), item);
            }
        }

        public virtual void AssertOrderedCollections(string name, params string[] items)
        {
            Assert.That(table.Keys, Contains.Item(name), tableName);
            AssertOrderedCollection(table[name], items);
        }

        protected void AssertOrderedCollection(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            AssertCollection(actual, expected);

            if (actual.Count() <= 10 && expected.Count() <= 10)
            {
                Assert.That(actual, Is.EqualTo(expected));
                return;
            }

            var expectedArray = expected.ToArray();
            var actualArray = actual.ToArray();

            for (var i = 0; i < expectedArray.Length; i++)
            {
                Assert.That(actualArray[i], Is.EqualTo(expectedArray[i]), $"Index {i}");
            }
        }
    }
}