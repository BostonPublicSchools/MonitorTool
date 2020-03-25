using System;
using System.Data;
using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.Artifacts
{
    [TestFixture]
    public class FileSystemArtifactManagerSpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Ensuing the FileSystemArtifactManager behaves correctly")
                .InOrderTo("Add artifact data to the file system")
                .AsA("producer or consumer of artifact data")
                .IWant("the file system artifact manager to handle this for me")
                .Tag("ArtifactManager");
        }

        [Test]
        public void ShouldWriteTabSeparatedFile()
        {
            using (var domain = new FileSystemArtifactManagerDomain())
            {
                var tblData = new DataTable();
                tblData.Columns.Add("Column1", typeof (int));
                tblData.Columns.Add("Column2", typeof (long));
                tblData.Columns.Add("Column3", typeof (double));
                tblData.Columns.Add("Column4", typeof (string));
                tblData.Columns.Add("Column5", typeof (DateTime));

                tblData.Rows.Add(1, 1, 1d, "row1", new DateTime(2012, 11, 1));
                tblData.Rows.Add(2, 2, 1d, "row2", new DateTime(2012, 11, 2));
                tblData.Rows.Add(3, 3, 1d, "row3", new DateTime(2012, 11, 3));
                tblData.Rows.Add(4, 4, 1d, "row4", new DateTime(2012, 11, 4));

                Feature.WithScenario("saving a DataTable to csv format")
                    .Given(domain.TheDataSource, tblData)
                        .And(domain.TheManagerIsInitialised)
                    .When(domain.TheArtifactIsSaved)
                    .Then(domain.TheExpectedOutputFileShouldBeCreated)
                    .ExecuteWithReport();
            }
        }
    }
}