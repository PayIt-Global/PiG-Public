using System;

namespace CryptAplyApp.Application.Models
{
    public class KeyRotationSchedulerOptions
    {
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(15);
        public TimeSpan RotationWarningThreshold { get; set; } = TimeSpan.FromDays(7);
        public bool EnableAutomaticRotation { get; set; } = true;
        public int MaxConcurrentRotations { get; set; } = 1;
        public TimeSpan MinimumRotationInterval { get; set; } = TimeSpan.FromDays(30);
        public int RetryAttempts { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
    }
}
