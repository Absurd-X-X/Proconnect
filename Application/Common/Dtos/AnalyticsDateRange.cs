// Application/Common/Analytics/AnalyticsDateRange.cs
namespace Application.Common.Dtos
{
    public enum DateRangePreset { Last7Days, Last30Days, Last90Days, LastYear, Custom }

    public class AnalyticsDateRange
    {
        public DateRangePreset Preset { get; set; } = DateRangePreset.Last30Days;
        public DateTime? CustomStart { get; set; }
        public DateTime? CustomEnd { get; set; }

        public (DateTime Start, DateTime End) Resolve()
        {
            var now = DateTime.UtcNow;
            return Preset switch
            {
                DateRangePreset.Last7Days => (now.AddDays(-7), now),
                DateRangePreset.Last30Days => (now.AddDays(-30), now),
                DateRangePreset.Last90Days => (now.AddDays(-90), now),
                DateRangePreset.LastYear => (now.AddYears(-1), now),
                DateRangePreset.Custom => (CustomStart ?? now.AddDays(-30), CustomEnd ?? now),
                _ => (now.AddDays(-30), now)
            };
        }

        public (DateTime Start, DateTime End) ResolvePreviousPeriod()
        {
            var (start, end) = Resolve();
            var span = end - start;
            return (start - span, start);
        }
    }
}