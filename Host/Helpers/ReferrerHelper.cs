using Domain.Enums;

namespace Host.Helpers
{
    public static class ReferrerHelper
    {
        public static ReferrerSource Parse(string? refParam)
        {
            return refParam?.ToLowerInvariant() switch
            {
                "search" => ReferrerSource.Search,
                "network" => ReferrerSource.Network,
                "external" => ReferrerSource.ExternalLink,
                _ => ReferrerSource.Direct
            };
        }
    }
}