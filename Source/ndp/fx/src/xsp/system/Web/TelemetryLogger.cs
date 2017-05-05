//------------------------------------------------------------------------------
// <copyright file="TelemetryLogger.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web {
    using System.Diagnostics.Tracing;
    using System.Web.Util;

    [EventData(Name="TargetFrameworkSet")]
    struct TargetFrameworkTelemetryData {
        public string AppID { get; set; }
        public string TargetFramework { get; set; }
    }

    [EventData(Name="HandlerMapped")]
    struct HttpHandlerTelemetryData {
        public string AppID { get; set; }
        public string HttpHandlerType { get; set; }
    }

    static class TelemetryLogger {
        // eventsource provider name
        private static readonly string WebFormsProviderName = "Microsoft.DOTNET.ASPNET.WebForms";

        // event name
        private static readonly string HttpHandlerEventName = "HandlerMapped";
        private static readonly string TargetFrameworkEventName = "TargetFrameworkSet";

        // telemetry eventsource
        private static EventSource s_TelemetryLogger = new TelemetryEventSource(WebFormsProviderName);
        private static readonly string s_AppID = GetAppID();

        public static void LogHttpHandler(Type httpHandlerType) {
            if (httpHandlerType == null) {
                return;
            }

            try {
                s_TelemetryLogger.Write(
                        HttpHandlerEventName,
                        TelemetryEventSource.MeasuresOptions(),
                        new HttpHandlerTelemetryData() {
                            AppID = s_AppID,
                            HttpHandlerType = GetHashCode(httpHandlerType.AssemblyQualifiedName)
                        }
                    );
            }
            catch { }
        }

        public static void LogTargetFramework(Version targetFrameworkVersion) {
            if (targetFrameworkVersion == null) {
                return;
            }

            try {
                s_TelemetryLogger.Write(
                    TargetFrameworkEventName,
                    TelemetryEventSource.MeasuresOptions(),
                    new TargetFrameworkTelemetryData() {
                        AppID = s_AppID,
                        TargetFramework = targetFrameworkVersion.ToString()
                    }
                );
            } 
            catch { }
        }

        private static string GetAppID() {
            return HttpRuntime.AppDomainAppId == null ? string.Empty : GetHashCode(HttpRuntime.AppDomainAppId);
        }

        private static string GetHashCode(string str) {
            return StringUtil.GetStringHashCode(str).ToString("X");
        }
    }
}
