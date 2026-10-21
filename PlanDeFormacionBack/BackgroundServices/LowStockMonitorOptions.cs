namespace PlanDeFormacionBack.BackgroundServices
{
    public class LowStockMonitorOptions
    {
        public const string SectionName = "LowStockMonitor";

        public int IntervalSeconds { get; set; } = 30;
        public int Threshold { get; set; } = 5;
    }
}
