using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class AlertHistory
    {
        public DateTime Received { get; set; }
        public bool? Result { get; set; }
        public double? ResultCount { get; set; }
        public int FailuresSinceLastSuccess { get; set; }
    }
}