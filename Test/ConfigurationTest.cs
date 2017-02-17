using System.Linq;
using NUnit.Framework;
using Paccia;

namespace Test
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void AllConfigurationKeysHaveDefaultHardcodedValue() =>
            Assert.That
                (
                    new HardcodedConfigurationDefaults().Values
                        .Keys
                        .OrderBy(k => k)
                        .SequenceEqual(EnumerableExtensions.GetEnumerableMembers<ConfigurationKey>().OrderBy(k => k)),
                    Is.True
                );
    }
}