using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Testing.Drivers
{
    public class AutomationScheduler : IHealthCheckSchedulerPlugin
    {
        public bool StartCalled { get; set; }
        public bool StopCalled { get; set; }
        public bool InitialiseCalled { get; set; }
        public IHealthCheckPlugin Check { get; set; }
        public Status Status { get; set; }
        public bool Enabled { get; set; }

        public AutomationScheduler(IHealthCheckPlugin check, bool enabled = true)
        {
            Check = check;
            Enabled = enabled;
            Status = Status.For(Identity.Name)
                .StateIsSuccess();
        }

        public void Initialise()
        {
            InitialiseCalled = true;
        }

        public PluginDescriptor Identity
        {
            get { return Check.Identity; }
        }

        public void Start()
        {
            StartCalled = true;
        }

        public void Stop()
        {
            StopCalled = true;
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }
    }
}