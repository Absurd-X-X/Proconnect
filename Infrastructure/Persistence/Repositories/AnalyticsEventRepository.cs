using Application.Common.Dtos.Analytics;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AnalyticsEventRepository(ProConnectDbContext context) : IAnalyticsEventRepository
    {
        public async Task LogAsync(AnalyticsEvent evt)
        {
            await context.AnalyticsEvents.AddAsync(evt);
        }

        public async Task<bool> ExistsRecentAsync(AnalyticsEventType eventType, Guid subjectId, Guid actorUserId, DateTime since)
        {
            return await context.AnalyticsEvents
                .AsNoTracking()
                .AnyAsync(e => e.EventType == eventType
                    && e.SubjectId == subjectId
                    && e.ActorUserId == actorUserId
                    && e.DateCreated >= since);
        }

        public async Task<int> GetCountAsync(AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end)
        {
            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && e.SubjectId == subjectId
                    && e.DateCreated >= start && e.DateCreated <= end)
                .CountAsync();
        }

        public async Task<List<IAnalyticsEventRepository.DateCountDto>> GetTrendAsync(
            AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end)
        {
            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && e.SubjectId == subjectId
                    && e.DateCreated >= start && e.DateCreated <= end)
                .GroupBy(e => e.DateCreated.Date)
                .Select(g => new IAnalyticsEventRepository.DateCountDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<ViewerContextDto>> GetViewerContextsAsync(
    AnalyticsEventType eventType, Guid subjectId, DateTime start, DateTime end)
        {
            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && e.SubjectId == subjectId
                    && e.DateCreated >= start && e.DateCreated <= end)
                .Select(e => new ViewerContextDto
                {
                    ActorUserId = e.ActorUserId,
                    HasProfessionalProfile = e.ActorUser != null && e.ActorUser.ProfessionalProfile != null,
                    HasRecruiterProfile = e.ActorUser != null && e.ActorUser.RecruiterProfile != null,
                    Industry = e.ActorUser != null && e.ActorUser.RecruiterProfile != null && e.ActorUser.RecruiterProfile.Company != null
                        ? e.ActorUser.RecruiterProfile.Company.Industry
                        : null,
                    CompanySize = e.ActorUser != null && e.ActorUser.RecruiterProfile != null && e.ActorUser.RecruiterProfile.Company != null
                        ? e.ActorUser.RecruiterProfile.Company.CompanySize
                        : null
                })
                .ToListAsync();
        }

        public async Task<Dictionary<ReferrerSource, int>> GetReferrerBreakdownAsync(
            Guid subjectId, AnalyticsEventType eventType, DateTime start, DateTime end)
        {
            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && e.SubjectId == subjectId
                    && e.ReferrerSource != null
                    && e.DateCreated >= start && e.DateCreated <= end)
                .GroupBy(e => e.ReferrerSource!.Value)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Source, x => x.Count);
        }

        public async Task<int> GetCountForSubjectsAsync(AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end)
        {
            if (subjectIds.Count == 0) return 0;

            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && subjectIds.Contains(e.SubjectId)
                    && e.DateCreated >= start && e.DateCreated <= end)
                .CountAsync();
        }

        public async Task<List<IAnalyticsEventRepository.DateCountDto>> GetTrendForSubjectsAsync(
            AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end)
        {
            if (subjectIds.Count == 0) return new List<IAnalyticsEventRepository.DateCountDto>();

            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && subjectIds.Contains(e.SubjectId)
                    && e.DateCreated >= start && e.DateCreated <= end)
                .GroupBy(e => e.DateCreated.Date)
                .Select(g => new IAnalyticsEventRepository.DateCountDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<Dictionary<ReferrerSource, int>> GetReferrerBreakdownForSubjectsAsync(
            AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end)
        {
            if (subjectIds.Count == 0) return new Dictionary<ReferrerSource, int>();

            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && subjectIds.Contains(e.SubjectId)
                    && e.ReferrerSource != null
                    && e.DateCreated >= start && e.DateCreated <= end)
                .GroupBy(e => e.ReferrerSource!.Value)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Source, x => x.Count);
        }

        public async Task<Dictionary<Guid, int>> GetCountsGroupedBySubjectAsync(
            AnalyticsEventType eventType, List<Guid> subjectIds, DateTime start, DateTime end)
        {
            if (subjectIds.Count == 0) return new Dictionary<Guid, int>();

            return await context.AnalyticsEvents
                .AsNoTracking()
                .Where(e => e.EventType == eventType && subjectIds.Contains(e.SubjectId)
                    && e.DateCreated >= start && e.DateCreated <= end)
                .GroupBy(e => e.SubjectId)
                .Select(g => new { SubjectId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SubjectId, x => x.Count);
        }
    }
}