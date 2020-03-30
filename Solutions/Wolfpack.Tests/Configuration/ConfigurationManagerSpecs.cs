using System.Collections.Generic;
using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Configuration.FileSystem;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Testing.Bdd;
using System.Linq;

namespace Wolfpack.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerSpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("")
                .InOrderTo("")
                .AsA("")
                .IWant("")
                .Tag("");
        }

        [Test]
        public void LoadCatalogue()
        {
            using (var domain = new ConfigurationManagerDomain())
            {
                Feature.WithScenario("")
                    .Given(domain.TheFileSystemScheduleRepositoryIsUsed, @"TestData\Config\Schedules")
                        .And(domain.TheConfigurationManagerIsInitialised)
                    .When(domain.TheConfigurationCatalogueForTag_IsLoaded, "Test")
                    .Then(domain.TheCatalogShouldContain_Item, 1)
                    .ExecuteWithReport();
            }
        }
    }

    public class ConfigurationManagerDomain : BddTestDomain
    {
        private readonly IList<IConfigurationRepository> _repositories;
        private IConfigurationManager _manager;
        private ConfigurationCatalogue _catalogue;

        public ConfigurationManagerDomain()
        {
            _repositories = new List<IConfigurationRepository>();
        }

        public override void Dispose()
        {            
        }

        public void TheFileSystemScheduleRepositoryIsUsed(string baseFolder)
        {
            _repositories.Add(new FileSystemScheduleConfigurationRepository(baseFolder));
        }

        public void TheConfigurationManagerIsInitialised()
        {
            _manager = new DefaultConfigurationManager(_repositories, new AgentConfiguration());
            _manager.Initialise();
        }

        public void TheConfigurationCatalogueForTag_IsLoaded(string tag)
        {
            _catalogue = _manager.GetCatalogue(tag);
        }

        public void TheCatalogShouldContain_Item(int expected)
        {
            Assert.That(_catalogue.Items.Count(), Is.EqualTo(expected));
        }
    }
}