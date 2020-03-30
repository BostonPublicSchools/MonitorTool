using System;
using System.Threading;
using FluentAssertions;

namespace Wolfpack.Core.Testing.Bdd
{
    public abstract class BddTestDomain : IDisposable
    {
        protected Exception _expectedException;

        public abstract void Dispose();

        public void SafeExecute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                _expectedException = ex;
            }
        }

        public void SimulateBackgroundEvent(Action action, int interval, Action asyncAction)
        {
            var bgThread = new Thread((() =>
            {
                Thread.Sleep(interval);
                asyncAction.Invoke();
            }));
            bgThread.Start();
            action.Invoke();
        }

        public void ThrewNoException()
        {
            _expectedException.Should().BeNull();
        }

        public void ShouldThrow_Exception(Type expected)
        {
            _expectedException.Should().NotBeNull();
            _expectedException.Should().Match<Exception>(ee => ee.GetType() == expected);
        }

        public void _ShouldBeInTheExceptionMesssage(string content)
        {
            _expectedException.Message.Should().Contain(content);
        }
    }
}