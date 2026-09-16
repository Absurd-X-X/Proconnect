using Application.Common.Dtos.Analytics;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IAnalyticsEventRepository
    {
        Task LogAsync(AnalyticsEvent evt);
        Task<bool> ExistsRecentAsync(AnalyticsEventType eventType, Guid subjectId, Guid actorUserId, DateTime since);
        Task<int> GetCountAsync(AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end);
        Task<List<DateCountDto>> GetTrendAsync(AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end);
        Task<Dictionary<ReferrerSource, int>> GetReferrerBreakdownAsync(Guid subjectId, AnalyticsEventType eventType, DateTime start, DateTime end);

        Task<int> GetCountForSubjectsAsync(AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end);
        Task<List<DateCountDto>> GetTrendForSubjectsAsync(AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end);
        Task<Dictionary<ReferrerSource, int>> GetReferrerBreakdownForSubjectsAsync(AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end);
        Task<Dictionary<Guid, int>> GetCountsGroupedBySubjectAsync(AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end);

        Task<List<ViewerContextDto>> GetViewerContextsAsync(AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end);

        public class DateCountDto
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }
    }
}