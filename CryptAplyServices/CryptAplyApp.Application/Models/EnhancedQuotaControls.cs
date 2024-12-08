using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public enum QuotaPriority
    {
        Background = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public class EnhancedQuotaControls : UsageQuotas
    {
        // Adaptive Quota Settings
        public bool EnableAdaptiveQuotas { get; set; } = true;
        public int MinimumQuota { get; set; } = 10;
        public int MaximumQuota { get; set; } = 1000;
        public double QuotaAdjustmentFactor { get; set; } = 0.1;

        // Burst Settings
        public bool AllowBursting { get; set; } = true;
        public int BurstCapacity { get; set; } = 100;
        public TimeSpan BurstDuration { get; set; } = TimeSpan.FromMinutes(5);

        // Cost-based Quota Settings
        public bool EnforceBudgetLimits { get; set; } = true;
        public decimal DailyBudget { get; set; } = 1000.0M;
        public Dictionary<string, decimal> OperationCosts { get; set; } = new Dictionary<string, decimal>();

        // Priority-based Throttling
        public bool EnablePriorityBasedThrottling { get; set; } = true;
        public Dictionary<string, QuotaPriority> OperationPriorities { get; set; } = new Dictionary<string, QuotaPriority>();

        public EnhancedQuotaControls()
        {
            // Initialize default operation costs
            OperationCosts = new Dictionary<string, decimal>
            {
                { "CreateKey", 10.0M },
                { "DeleteKey", 5.0M },
                { "Encrypt", 1.0M },
                { "Decrypt", 1.0M },
                { "Sign", 2.0M },
                { "Verify", 0.5M }
            };

            // Initialize default operation priorities
            OperationPriorities = new Dictionary<string, QuotaPriority>
            {
                { "CreateKey", QuotaPriority.Low },
                { "DeleteKey", QuotaPriority.High },
                { "Encrypt", QuotaPriority.Medium },
                { "Decrypt", QuotaPriority.High },
                { "Sign", QuotaPriority.Medium },
                { "Verify", QuotaPriority.Medium }
            };
        }
    }
}
