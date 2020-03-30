using System.Linq;
using NUnit.Framework;
using Wolfpack.Agent;

namespace Wolfpack.Tests.Agent
{
    [TestFixture]
    public class WhenTheInstallArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/install"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheTopshelfArgsShouldContainTheExpectedItems()
        {
            var tsArgs = CmdLine.Expanded;
            var expectedArg = "install";
            Assert.That(tsArgs.Contains(expectedArg), Is.True, "{0} expanded arg missing", expectedArg);
        }

        [Test]
        public void ThenTheTotalNumberOfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class WhenTheUninstallArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/uninstall"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheTopshelfArgsShouldContainTheExpectedItems()
        {
            var tsArgs = CmdLine.Expanded;
            var expectedArg = "uninstall";
            Assert.That(tsArgs.Contains(expectedArg), Is.True, "{0} expanded arg missing", expectedArg);
        }

        [Test]
        public void ThenTheTotalNumberOfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class WhenNoCustomArgsAreSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new string[0]);
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }
    }
    
    [TestFixture]
    public class WhenTheProfileCustomArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/profile:testing"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ThenTheNumberOfCustomArgsSuppliedShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheProfileShouldBeCorrect()
        {
            string profile;
            var found = CmdLine.Value(CmdLine.SwitchNames.Profile, out profile);

            Assert.That(found, Is.True);
            Assert.That(profile, Is.EqualTo("testing"));
        }
    }


    [TestFixture]
    public class WhenTheUpdateCustomArgIsSuppliedWithNoPackageSpecified
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/update"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ThenTheNumberOfCustomArgsSuppliedShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenThePackageShouldBeEmpty()
        {
            string actual;
            var found = CmdLine.Value(CmdLine.SwitchNames.Update, out actual);

            Assert.That(found, Is.True);
            Assert.That(actual, Is.Empty);
        }
    }


    [TestFixture]
    public class WhenTheUpdateCustomArgIsSuppliedWithPackageSpecified
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/update:package"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ThenTheNumberOfCustomArgsSuppliedShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenThePackageShouldBeCorrect()
        {
            const string expected = "package";

            string actual;
            var found = CmdLine.Value(CmdLine.SwitchNames.Update, out actual);

            Assert.That(found, Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}