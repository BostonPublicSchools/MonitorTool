using System;
using NUnit.Framework;
using Wolfpack.Core;
using Wolfpack.Core.Testing.Bdd;
using System.Linq;

namespace Wolfpack.Tests.System
{
    public class TypeDiscoveryDomain : BddTestDomain
    {
        private Type _typeToDiscover;
        private Type[] _typesFound;

        public TypeDiscoveryDomain()
        {
            Container.Initialise();
        }

        public override void Dispose()
        {            
        }

        public void ThisType_ToDiscover(Type typeToDiscover)
        {
            _typeToDiscover = typeToDiscover;
        }

        public void TheTypeDiscoveryIsExecuted()
        {
            TypeDiscovery.Discover(_typeToDiscover, out _typesFound);
        }

        public void This_TypeShouldBeDiscovered(Type expectedType)
        {
            Assert.That(_typesFound.Contains(expectedType), Is.True);
        }
    }
}