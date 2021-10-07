﻿// <auto-generated />
// This .CS file is automatically generated. If you modify its contents, your changes will be overwritten.
// Modify the respective T4 templates if changes are required.

// <auto-generated />
// ----------- ----------- ----------- ----------- -----------
// The source code below is included via a T4 template.
//  * The template calling MUST specify the value of the <c>LogNamespaceName</c> meta-variable.
//  * The template calling MAY specify the value of the <c>LogClassAccessibilityLevel</c> meta-variable,
//    however its default value "public" is the appropriate choice in most cases
//    (other values would prevent composability by other assemblies).
// ----------- ----------- ----------- ----------- -----------

using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Datadog.Logging.Emission;

namespace Demo.Slimple.NetCore31
{
    /// <summary>
    /// Lightweight Log stub for Logging-SDK-agnostic logging.
    /// Users of this library can use this class as a leighweight redirect to whatever log technology is used for output.
    /// An absolute minimum of dependencies is required: 3 small static classes that are included as source code (no library dependency).
    /// The only namespaces importand by those 3 static classes are (see also <c>Datadog.Logging.Emission.props</c>):
    ///  - System
    ///  - System.Collections.Generic
    ///  - System.Diagnostics (only to get Process.GetCurrentProcess().Id)
    ///  - System.Runtime.CompilerServices (on;y for [MethodImpl(MethodImplOptions.AggressiveInlining)])
    ///  - System.Text
    /// 
    /// This allows to avoid creating complex logging abstractions (or taking dependencies on ILogger or other logging libraries).
    /// This class is re-generated in each project wants to use it using T4. The only thing that T4 does is using a user-specified namespace.
    /// Projects that wish to avoid using T4, can copy this file and hard-code the namespace (beware for source-forking).
    /// 
    /// <para>EMITTING LOGS.</para>
    /// <para>
    /// For example:
    /// 
    /// Library "Datadog.AutoInstrumentation.Profiler.Managed.dll" gets a copy of this file with the adjusted namespace:
    /// 
    /// <code>
    ///   namespace Datadog.AutoInstrumentation.Profiler
    ///   {
    ///       public static class Log
    ///       {
    ///       . . .
    ///       }
    ///   }
    /// </code>
    /// 
    /// Library "Datadog.AutoInstrumentation.Tracer.Managed.dll" also gets a copy of this file with the adjusted namespace:
    /// 
    /// <code>
    ///   namespace Datadog.AutoInstrumentation.Tracer.Managed
    ///   {
    ///       public static class Log
    ///       {
    ///       . . .
    ///       }
    ///   }
    /// </code>  
    /// 
    /// Each library can now make Log statements, for example:
    /// 
    /// <code>
    ///   Log.Info("DataExporter", "Data transport started", "size", _size, "otherAttribute", _otherAttribute);
    /// </code>  
    /// </para>
    /// 
    /// <para>COMPOSING AND PERSISTING LOGS.</para>
    /// <para>
    /// To continue the above example, assume that the entrypoint of the application is another library "Datadog.AutoInstrumentation.TracerAndProfilerLoader.dll".
    /// It uses the the two above libraries and it wants to direct the logs to some particular logging destnation (sink).
    /// For that, the TracerAndProfilerLoader takes a dependencty on a few additional source files.
    /// Those are also small, do not have any non-framework dependencies and run on Net Fx and Core Fx (see also <c>Datadog.Logging.Composition.props</c>).
    /// It creates a trivial adaper and configures the indirection.
    /// If short, the redirection happens as shown below.
    /// A fully rubust example is in the <c>Datadog.Logging.Demo</c> project, and log sinks are included for
    ///  - Console
    ///  - Files (with optional rotation)
    ///  - COmposing multiple sinks together
    /// 
    /// <code>
    ///   namespace Datadog.AutoInstrumentation.TracerAndProfilerLoader
    ///   {
    ///       using ComposerLogAdapter = Datadog.AutoInstrumentation.ProductComposer.LogAdapter;
    ///       using ProfilerLog = Datadog.AutoInstrumentation.Profiler.Log;
    ///       using TracerLog = Datadog.AutoInstrumentation.Tracer.Managed.Log;
    ///       
    ///       internal static class LogComposer
    ///       {
    ///           public const bool IsDebugLoggingEnabled = true;
    ///           
    ///           static LogComposer()
    ///           {
    ///               ProfilerLog.Log.Configure.Error((component, msg, ex, data) => LogError("Profiler", component, msg, ex, data));
    ///               ProfilerLog.Log.Configure.Info((component, msg, data) => LogInfo("Profiler", component, msg, data));
    ///               ProfilerLog.Log.Configure.Debug((component, msg, data) => LogDebug("Profiler", component, msg, data));
    ///               ProfilerLog.Log.Configure.DebugLoggingEnabled(IsDebugLoggingEnabled);
    ///               
    ///               TracerLog.Log.Configure.Error((component, msg, ex, data) => LogError("Tracer", component, msg, ex, data));
    ///               TracerLog.Log.Configure.Info((component, msg, data) => LogInfo("Tracer", component, msg, data));
    ///               TracerLog.Log.Configure.Debug((component, msg, data) => LogDebug("Tracer", component, msg, data));
    ///               TracerLog.Log.Configure.DebugLoggingEnabled(IsDebugLoggingEnabled);
    ///           }
    ///   
    ///           private static void LogError(string logGroupMoniker, string logComponentMoniker, string message, Exception error, IEnumerable<object> dataNamesAndValues)
    ///           {
    ///               // Prepare a log line in any appropriate way. For example:
    ///               string logLine = DefaultFormat.ConstructLogLine(DefaultFormat.LogLevelMoniker_Error,
    ///                                                               logGroupMoniker,
    ///                                                               logComponentMoniker,
    ///                                                               useUtcTimestamp: false,
    ///                                                               DefaultFormat.ConstructErrorMessage(message, error),
    ///                                                               dataNamesAndValues)
    ///                                             .ToString());
    ///               // Persist logLine to file...
    ///           }
    ///
    ///           private static void LogInfo(string logGroupMoniker, string logComponentMoniker, string message, IEnumerable<object> dataNamesAndValues)
    ///           {
    ///               // Prepare a log line (e.g. like above) and persist it to file...
    ///           }
    ///
    ///           private static void LogDebug(string logGroupMoniker, string logComponentMoniker, string message, IEnumerable<object> dataNamesAndValues)
    ///           {
    ///               if (IsDebugLoggingEnabled)
    ///               {
    ///                   // Prepare a log line (e.g. like above) and persist it to file...
    ///               }
    ///           }
    ///       }
    ///   }
    /// </code>
    /// </para>
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Use statements like <c>Log.Configure.EventHandlers.Info(YourHandler)</c> to redirect logging to your destination.
        /// </summary>
        public static class Configure
        {
            /// <summary>
            /// Sets whether Debug log events should be processed or ignored.
            /// </summary>
            public static void DebugLoggingEnabled(bool isDebugLoggingEnabled)
            {
                s_config_IsDebugLoggingEnabled = isDebugLoggingEnabled;
            }

            /// <summary>
            /// The params of the handler Actions are:
            /// <code>
            ///     void Handler(string logSourceNamePart1,
            ///                  string logSourceNamePart2,
            ///                  int logSourceCallLineNumber,
            ///                  string logSourceCallMemberName,
            ///                  string logSourceCallFileName,
            ///                  string logSourceAssemblyName,
            ///                  string message, 
            ///                  Exception exception,                      //  <-- Only for Error handlers
            ///                  IEnumerable<object> dataNamesAndValues);
            /// </code>
            /// </summary>
            public static class EventHandlers
            {
                /// <summary>
                /// Sets the handler delegate for processing Error log events.
                /// If <c>null</c> is specified, then Error log events will be ignored.
                /// </summary>
                public static void Error(Action<string, string, int, string, string, string, string, Exception, IEnumerable<object>> logErrorEventHandler)
                {
                    s_AutomaticAssemblyName_ForError_LastTimestamp = 0;
                    s_config_EventHandlers_Error = logErrorEventHandler;
                }

                /// <summary>
                /// Sets the handler delegate for processing Info log events.
                /// If <c>null</c> is specified, then Error log events will be ignored.
                /// </summary>
                public static void Info(Action<string, string, int, string, string, string, string, IEnumerable<object>> logInfoEventHandler)
                {
                    s_AutomaticAssemblyName_ForInfo_LastTimestamp = 0;
                    s_config_EventHandlers_Info = logInfoEventHandler;
                }

                /// <summary>
                /// Sets the handler delegate for processing Debug log events.
                /// If <c>null</c> is specified, then Error log events will be ignored.
                /// </summary>
                public static void Debug(Action<string, string, int, string, string, string, string, IEnumerable<object>> logDebugEventHandler)
                {
                    s_AutomaticAssemblyName_ForDebug_LastTimestamp = 0; 
                    s_config_EventHandlers_Debug = logDebugEventHandler;
                }
            }  // class Log.Configure.EventHandlers

            public static class AutomaticAssemblyName
            {
                public static class ForError
                {
                    /// <summary>
                    /// Gets or sets whether the assembly name (incl. version) of the <c>LogSourceInfo</c> used when emitting the log line will be automatically
                    /// included when emitting Error log events (callers can always speciy the assembly name manually).
                    /// </summary>
                    public static bool Include
                    {
                        get { return s_config_AutomaticAssemblyName_ForError_Include; }
                        set { s_AutomaticAssemblyName_ForError_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForError_Include = value; }
                    }

                    /// <summary>
                    /// If the assembly name is included automatically for Error log events, this setting controls how frequently.
                    /// <c>0</c> means always include, positive values mean include once per the specified number of seconds
                    /// (negative values are treated like <c>0</c>).
                    /// </summary>
                    public static int PeriodSecs
                    {
                        get { return s_config_AutomaticAssemblyName_ForError_PeriodMillisecs / 1000; }
                        set { s_AutomaticAssemblyName_ForError_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForError_PeriodMillisecs = (value >= 0) ? value * 1000 : 0; }
                    }
                }  // class Log.Configure.AutomaticAssemblyName.ForError

                public static class ForInfo
                {
                    /// <summary>
                    /// Gets or sets whether the assembly name (incl. version) of the <c>LogSourceInfo</c> used when emitting the log line will be automatically
                    /// included when emitting Info log events (callers can always speciy the assembly name manually).
                    /// </summary>
                    public static bool Include
                    {
                        get { return s_config_AutomaticAssemblyName_ForInfo_Include; }
                        set { s_AutomaticAssemblyName_ForInfo_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForInfo_Include = value; }
                    }

                    /// <summary>
                    /// If the assembly name is included automatically for Info log events, this setting controls how frequently.
                    /// <c>0</c> means always include, positive values mean include once per the specified number of seconds
                    /// (negative values are treated like <c>0</c>).
                    /// </summary>
                    public static int PeriodSecs
                    {
                        get { return s_config_AutomaticAssemblyName_ForInfo_PeriodMillisecs / 1000; }
                        set { s_AutomaticAssemblyName_ForInfo_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForInfo_PeriodMillisecs = (value >= 0) ? value * 1000 : 0; }
                    }
                }  // class Log.Configure.AutomaticAssemblyName.ForInfo

                public static class ForDebug
                {
                    /// <summary>
                    /// Gets or sets whether the assembly name (incl. version) of the <c>LogSourceInfo</c> used when emitting the log line will be automatically
                    /// included when emitting Debug log events (callers can always speciy the assembly name manually).
                    /// </summary>
                    public static bool Include
                    {
                        get { return s_config_AutomaticAssemblyName_ForDebug_Include; }
                        set { s_AutomaticAssemblyName_ForDebug_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForDebug_Include = value; }
                    }

                    /// <summary>
                    /// If the assembly name is included automatically for Debug log events, this setting controls how frequently.
                    /// <c>0</c> means always include, positive values mean include once per the specified number of seconds
                    /// (negative values are treated like <c>0</c>).
                    /// </summary>
                    public static int PeriodSecs
                    {
                        get { return s_config_AutomaticAssemblyName_ForDebug_PeriodMillisecs / 1000; }
                        set { s_AutomaticAssemblyName_ForDebug_LastTimestamp = 0;
                              s_config_AutomaticAssemblyName_ForDebug_PeriodMillisecs = (value >= 0) ? value * 1000 : 0; }
                    }
                }  // class Log.Configure.AutomaticAssemblyName.ForDebug
            }  // class Log.Configure.AutomaticAssemblyName
        }  // class Log.Configure

        // The doc-comment on inner class EventHandlers should have an explanation of the Action parameters for the event handlers.
        private static Action<string, string, int, string, string, string, string, Exception, IEnumerable<object>> s_config_EventHandlers_Error = SimpleConsoleSink.Error;
        private static Action<string, string, int, string, string, string, string, IEnumerable<object>> s_config_EventHandlers_Info = SimpleConsoleSink.Info;
        private static Action<string, string, int, string, string, string, string, IEnumerable<object>> s_config_EventHandlers_Debug = SimpleConsoleSink.Debug;

        private static bool s_config_IsDebugLoggingEnabled = true;

        private static bool s_config_AutomaticAssemblyName_ForError_Include = true;
        private static bool s_config_AutomaticAssemblyName_ForInfo_Include = true;
        private static bool s_config_AutomaticAssemblyName_ForDebug_Include = true;

        private static int s_config_AutomaticAssemblyName_ForError_PeriodMillisecs = 0;
        private static int s_config_AutomaticAssemblyName_ForInfo_PeriodMillisecs = 60000;
        private static int s_config_AutomaticAssemblyName_ForDebug_PeriodMillisecs = 0;

        private static int s_AutomaticAssemblyName_ForError_LastTimestamp = 0;
        private static int s_AutomaticAssemblyName_ForInfo_LastTimestamp = 0;
        private static int s_AutomaticAssemblyName_ForDebug_LastTimestamp = 0;

        internal static LogSourceInfo WithCallInfo(string logSourceName,
                                                   [CallerLineNumber] int callLineNumber = 0,
                                                   [CallerMemberName] string callMemberName = null)
        {
            return new LogSourceInfo(logSourceNamePart1: null,
                                     logSourceNamePart2: logSourceName,
                                     callLineNumber,
                                     callMemberName,
                                     callFileName: null,
                                     assemblyName: null);
        }

        internal static LogSourceInfo WithCallInfo(LogSourceInfo logSourceInfo,
                                                   [CallerLineNumber] int callLineNumber = 0,
                                                   [CallerMemberName] string callMemberName = null)
        {
            return logSourceInfo.WithCallInfo(callLineNumber, callMemberName);
        }

        /// <summary>
        /// Gets whether debug log messages should be processed or ignored.
        /// Consider wrapping debug message invocations into IF statements that check for this
        /// value in order to avoid unnecessarily constructing debug message strings.
        /// </summary>
        public static bool IsDebugLoggingEnabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return s_config_IsDebugLoggingEnabled; }
        }

                /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(string logSourceName, string message, params object[] dataNamesAndValues)
        {
            Error(new LogSourceInfo(logSourceName), message, exception: null, dataNamesAndValues);
        }

        /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(LogSourceInfo logSourceInfo, string message, params object[] dataNamesAndValues)
        {
            Error(logSourceInfo, message, exception: null, dataNamesAndValues);
        }

        /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(string logSourceName, Exception exception, params object[] dataNamesAndValues)
        {
            Error(new LogSourceInfo(logSourceName), message: null, exception, dataNamesAndValues);
        }

        /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(LogSourceInfo logSourceInfo, Exception exception, params object[] dataNamesAndValues)
        {
            Error(logSourceInfo, message: null, exception, dataNamesAndValues);
        }

        /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(string logSourceName, string message, Exception exception, params object[] dataNamesAndValues)
        {
            Error(new LogSourceInfo(logSourceName), message, exception, dataNamesAndValues);
        }

        /// <summary> Logs an error. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(LogSourceInfo logSourceInfo, string message, Exception exception, params object[] dataNamesAndValues)
        {
            Action<string, string, int, string, string, string, string, Exception, IEnumerable<object>> logEventHandler = s_config_EventHandlers_Error;
            if (logEventHandler != null)
            {
                // If AutomaticAssemblyName is enabled AND AssemblyName is not already specified, look into including the assembly name now.
                if (s_config_AutomaticAssemblyName_ForError_Include && logSourceInfo.AssemblyName == null)
                {
                    // If AutomaticAssemblyName inclusion period has passed, add the assembly name (xxx_PeriodMillisecs < 1 indicates "always include").
                    // There is a benign race on R/W of xxx_LastTimestamp: We may log all the assembly names a few extra times or get the period off by a few millisecs.
                    if (s_config_AutomaticAssemblyName_ForError_PeriodMillisecs <= 0
                            || GetEnvironmentTicksSince(s_AutomaticAssemblyName_ForError_LastTimestamp, out s_AutomaticAssemblyName_ForError_LastTimestamp)
                                    >= s_config_AutomaticAssemblyName_ForError_PeriodMillisecs)
                    {
                        logSourceInfo = logSourceInfo.WithAssemblyName();
                    }
                }

                logEventHandler(logSourceInfo.LogSourceNamePart1,
                                logSourceInfo.LogSourceNamePart2,
                                logSourceInfo.CallLineNumber,
                                logSourceInfo.CallMemberName,
                                logSourceInfo.CallFileName,
                                logSourceInfo.AssemblyName,
                                message,
                                exception,
                                GetDataNamesAndValuesEnum(dataNamesAndValues));
            }
        }

        /// <summary>
        /// This method logs and rethrows the exception. It is typed to return the exception, to enable writing concise code like:
        /// <code>
        ///   try
        ///   {
        ///       // ...
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///       throw Log.ErrorRethrow("...", ex);
        ///   }
        /// </code>
        /// The throwing actually happens inside of the <c>ErrorRethrow(..)<.c> method. However, this syntaxt allows the compiler to know that 
        /// an exception will occur at that line. This prevents incorrect code analysis warnings/end errors such as 'missing return value',
        /// 'missing initialization' and similar.
        /// </summary>
        /// <returns>Either <c>null</c> if the specified <c>exception</c> is <c>null</c>, or nothing at all,
        /// because the specified <c>exception</c> is rethrown.</returns>
        internal static Exception ErrorRethrow(string logSourceName, Exception exception, params object[] dataNamesAndValues)
        {
            return ErrorRethrow(new LogSourceInfo(logSourceName), message: null, exception, dataNamesAndValues);
        }

        /// <summary>
        /// This method logs and rethrows the exception. It is typed to return the exception, to enable writing concise code like:
        /// <code>
        ///   try
        ///   {
        ///       // ...
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///       throw Log.ErrorRethrow("...", ex);
        ///   }
        /// </code>
        /// The throwing actually happens inside of the <c>ErrorRethrow(..)<.c> method. However, this syntaxt allows the compiler to know that 
        /// an exception will occur at that line. This prevents incorrect code analysis warnings/end errors such as 'missing return value',
        /// 'missing initialization' and similar.
        /// </summary>
        /// <returns>Either <c>null</c> if the specified <c>exception</c> is <c>null</c>, or nothing at all,
        /// because the specified <c>exception</c> is rethrown.</returns>
        internal static Exception ErrorRethrow(LogSourceInfo logSourceInfo, Exception exception, params object[] dataNamesAndValues)
        {
            return ErrorRethrow(logSourceInfo, message: null, exception, dataNamesAndValues);
        }

        /// <summary>
        /// This method logs and rethrows the exception. It is typed to return the exception, to enable writing concise code like:
        /// <code>
        ///   try
        ///   {
        ///       // ...
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///       throw Log.ErrorRethrow("...", ex);
        ///   }
        /// </code>
        /// The throwing actually happens inside of the <c>ErrorRethrow(..)<.c> method. However, this syntaxt allows the compiler to know that 
        /// an exception will occur at that line. This prevents incorrect code analysis warnings/end errors such as 'missing return value',
        /// 'missing initialization' and similar.
        /// </summary>
        /// <returns>Either <c>null</c> if the specified <c>exception</c> is <c>null</c>, or nothing at all,
        /// because the specified <c>exception</c> is rethrown.</returns>
        internal static Exception ErrorRethrow(string logSourceName, string message, Exception exception, params object[] dataNamesAndValues)
        {
            return ErrorRethrow(new LogSourceInfo(logSourceName), message, exception, dataNamesAndValues);
        }

        /// <summary>
        /// This method logs and rethrows the exception. It is typed to return the exception, to enable writing concise code like:
        /// <code>
        ///   try
        ///   {
        ///       // ...
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///       throw Log.ErrorRethrow("...", ex);
        ///   }
        /// </code>
        /// The throwing actually happens inside of the <c>ErrorRethrow(..)<.c> method. However, this syntaxt allows the compiler to know that 
        /// an exception will occur at that line. This prevents incorrect code analysis warnings/end errors such as 'missing return value',
        /// 'missing initialization' and similar.
        /// </summary>
        /// <returns>Either <c>null</c> if the specified <c>exception</c> is <c>null</c>, or nothing at all,
        /// because the specified <c>exception</c> is rethrown.</returns>
        internal static Exception ErrorRethrow(LogSourceInfo logSourceInfo, string message, Exception exception, params object[] dataNamesAndValues)
        {
            Error(logSourceInfo, message, exception, dataNamesAndValues);

            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return exception;
        }

        /// <summary> Logs an important info message. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Info(string logSourceName, string message, params object[] dataNamesAndValues)
        {
            Info(new LogSourceInfo(logSourceName), message, dataNamesAndValues);
        }

        /// <summary> Logs an important info message. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Info(LogSourceInfo logSourceInfo, string message, params object[] dataNamesAndValues)
        {
            Action<string, string, int, string, string, string, string, IEnumerable<object>> logEventHandler = s_config_EventHandlers_Info;
            if (logEventHandler != null)
            {
                // If AutomaticAssemblyName is enabled AND AssemblyName is not already specified, look into including the assembly name now.
                if (s_config_AutomaticAssemblyName_ForInfo_Include && logSourceInfo.AssemblyName == null)
                {
                    // If AutomaticAssemblyName inclusion period has passed, add the assembly name (xxx_PeriodMillisecs < 1 indicates "always include").
                    // There is a benign race on R/W of xxx_LastTimestamp: We may log all the assembly names a few extra times or get the period off by a few millisecs.
                    if (s_config_AutomaticAssemblyName_ForInfo_PeriodMillisecs <= 0
                            || GetEnvironmentTicksSince(s_AutomaticAssemblyName_ForInfo_LastTimestamp, out s_AutomaticAssemblyName_ForInfo_LastTimestamp)
                                    >= s_config_AutomaticAssemblyName_ForInfo_PeriodMillisecs)
                    {
                        logSourceInfo = logSourceInfo.WithAssemblyName();
                    }
                }

                logEventHandler(logSourceInfo.LogSourceNamePart1,
                                logSourceInfo.LogSourceNamePart2,
                                logSourceInfo.CallLineNumber,
                                logSourceInfo.CallMemberName,
                                logSourceInfo.CallFileName,
                                logSourceInfo.AssemblyName,
                                message,
                                GetDataNamesAndValuesEnum(dataNamesAndValues));
            }
        }

        /// <summary> Logs a non-critical debug message. </summary>
        /// <remarks> Mainly used for for debugging during prototyping.
        /// These messages can likely be dropped in production during noirmal operations.
        /// Debug logging may be activated to collect additional informaton to resolve production issues. </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Debug(string logSourceName, string message, params object[] dataNamesAndValues)
        {
            if (s_config_IsDebugLoggingEnabled)
            { 
                Debug(new LogSourceInfo(logSourceName), message, dataNamesAndValues);
            }
        }

        /// <summary> Logs a non-critical debug message. </summary>
        /// <remarks> Mainly used for for debugging during prototyping.
        /// These messages can likely be dropped in production during noirmal operations.
        /// Debug logging may be activated to collect additional informaton to resolve production issues. </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Debug(LogSourceInfo logSourceInfo, string message, params object[] dataNamesAndValues)
        {
            if (s_config_IsDebugLoggingEnabled)
            {
                Action<string, string, int, string, string, string, string, IEnumerable<object>> logEventHandler = s_config_EventHandlers_Debug;
                if (logEventHandler != null)
                {
                    // If AutomaticAssemblyName is enabled AND AssemblyName is not already specified, look into including the assembly name now.
                    if (s_config_AutomaticAssemblyName_ForDebug_Include && logSourceInfo.AssemblyName == null)
                    {
                        // If AutomaticAssemblyName inclusion period has passed, add the assembly name (xxx_PeriodMillisecs < 1 indicates "always include").
                        // There is a benign race on R/W of xxx_LastTimestamp: We may log all the assembly names a few extra times or get the period off by a few millisecs.
                        if (s_config_AutomaticAssemblyName_ForDebug_PeriodMillisecs <= 0
                                    || GetEnvironmentTicksSince(s_AutomaticAssemblyName_ForDebug_LastTimestamp, out s_AutomaticAssemblyName_ForDebug_LastTimestamp)
                                            >= s_config_AutomaticAssemblyName_ForDebug_PeriodMillisecs)
                        {
                            logSourceInfo = logSourceInfo.WithAssemblyName();
                        }
                    }

                    logEventHandler(logSourceInfo.LogSourceNamePart1,
                                    logSourceInfo.LogSourceNamePart2,
                                    logSourceInfo.CallLineNumber,
                                    logSourceInfo.CallMemberName,
                                    logSourceInfo.CallFileName,
                                    logSourceInfo.AssemblyName,
                                    message,
                                    GetDataNamesAndValuesEnum(dataNamesAndValues));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<object> GetDataNamesAndValuesEnum(object[] dataNamesAndValues)
        {
            if (dataNamesAndValues != null
                    && dataNamesAndValues.Length == 1
                    && dataNamesAndValues[0] != null
                    && dataNamesAndValues[0] is IEnumerable<object> enumVal )
            {
                return enumVal;
            }
            else
            {
                return dataNamesAndValues;
            }
        }

        /// <summary>
        /// Computes the delta in the same units as given by <c>Environment.TickCount</c> (i.e. milliseconds) since the specified
        /// previous result of such invocation. Recall that <c>Environment.TickCount</c> cycles approx every 49.8 days.
        ///   ==> https://docs.microsoft.com/en-us/dotnet/api/system.environment.tickcount?examples
        /// <para>THIS METHOD ONLY PRODUCES CORRECT RESULTS IF THE ACTUAL ELAPSED DELTA IS LESS THAN HALF OF THE CYCLE PERIOD,
        /// I.E. LESS THAN APPROX 24.9 DAYS!</para>
        /// Note that the method used here is correct for deltas in the valid range.
        ///   ==> https://stackoverflow.com/questions/243351/environment-tickcount-vs-datetime-now/1078089#1078089
        /// </summary>
        private static int GetEnvironmentTicksSince(int previousEnvironmentTicks, out int currentEnvironmentTicks)
        {
            unchecked
            {
                currentEnvironmentTicks = Environment.TickCount;
                int delta = currentEnvironmentTicks - previousEnvironmentTicks;
                return delta;
            }
        }
    }  // class Log
}  // namespace
