using System;
using System.IO;
using NUnit.Framework;
using Wolfpack.Core.Artifacts;
using Wolfpack.Core.Artifacts.Formats;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.Artifacts
{
    public class FileSystemArtifactManagerDomain : BddTestDomain
    {
        public const string RootFolder = @"TestData\Artifacts";
        public const string PluginName = "ArtifactManagerTestPlugin";

        private readonly IArtifactManager _sut;
        private readonly PluginDescriptor _pluginDescriptor;

        private ArtifactDescriptor _artifactDescriptor;
        private object _data;

        public FileSystemArtifactManagerDomain()
        {
            _sut = new FileSystemArtifactManager(RootFolder, new IArtifactFormatter[] { new TabSeparatedFormatter(), new JsonFormatter() });
            _pluginDescriptor = new PluginDescriptor
                                    {
                                        Name = PluginName,
                                        TypeId = new Guid("D3A6F2A6-63C6-43FF-95F5-010BADAB57A2")
                                    };
        }

        public override void Dispose()
        {
            
        }

        public void TheDataSource(object data)
        {
            _data = data;
        }

        public void TheManagerIsInitialised()
        {
            _sut.Initialise();
        }

        public void TheArtifactIsSaved()
        {
            _artifactDescriptor = _sut.Save(_pluginDescriptor, ArtifactContentTypes.CSV, _data);
        }

        public void TheExpectedOutputFileShouldBeCreated()
        {
            var filepath = Path.Combine(RootFolder, _pluginDescriptor.Name);
            filepath = Path.Combine(filepath, Path.ChangeExtension(_artifactDescriptor.Id.ToString(), "dat"));
            Assert.That(File.Exists(filepath), Is.True);
        }
    }
}