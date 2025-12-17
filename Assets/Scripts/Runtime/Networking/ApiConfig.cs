using System;
using UnityEngine;

namespace CrossingSimulator.Networking
{
    public static class ApiConfig
    {
        public const string HostEnvironmentVariable = "CROSSING_API_HOST";
        public const string DefaultHost = "http://localhost:4000";

        static string cachedHost;

        public static string Host => cachedHost ??= ResolveHost();

        static string ResolveHost()
        {
            var rawHost = Environment.GetEnvironmentVariable(HostEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(rawHost))
            {
                Debug.LogWarning($"Environment variable '{HostEnvironmentVariable}' is not set. Falling back to '{DefaultHost}'.");
                rawHost = DefaultHost;
            }

            return rawHost.Trim().TrimEnd('/');
        }

        public static void RefreshHostCache()
        {
            cachedHost = null;
        }

        public static string BuildUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Host;

            return $"{Host}{(path.StartsWith("/") ? path : "/" + path)}";
        }
    }
}
