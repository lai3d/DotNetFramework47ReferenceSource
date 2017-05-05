//------------------------------------------------------------------------------
// <copyright file="DpiHelper.Microsoft.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Windows.Forms
{
    using System.Configuration;

    /// <summary>
    /// Part of DpiHelper class, with methods specific to Microsoft assembly
    /// Also handles configuration switches that control states of various Microsoft features
    /// </summary>
    internal static partial class DpiHelper
    {
        private static bool isDpiHelperForWinformsInitialized = false;
        private static bool enableToolStripHighDpiImprovements = false;
        private static bool enableDpiChangedMessageHandling = false;
        private static bool enableCheckedListBoxHighDpiImprovements = false;
        private static bool enableThreadExceptionDialogHighDpiImprovements = false;
        private static bool enableDataGridViewControlHighDpiImprovements = false;
        private static bool enableSinglePassScalingOfDpiForms = false;
        private static bool enableAnchorLayoutHighDpiImprovements = false;
        private static bool enableMonthCalendarHighDpiImprovements = false;

        internal static void InitializeDpiHelperForWinforms()
        {
            // initialize shared fields
            Initialize();

            if (isDpiHelperForWinformsInitialized)
            {
                return;
            }
            try
            {
                // Redstone 2 or greater, where all APIs required by this feature are available
                if ((Environment.OSVersion.Platform == System.PlatformID.Win32NT) &&
                        (Environment.OSVersion.Version.CompareTo(ConfigurationOptions.RS2Version) >= 0) &&
                        (IsExpectedConfigValue(ConfigurationStringConstants.DisableDpiChangedMessageHandlingKeyName, false)) &&
                        (IsDpiAwarenessValueSet()) &&
                        // The dynamic scaling features are implemented only in comclt32 v6, no point to
                        // activate it otherwise.
                        (Application.ComCtlSupportsVisualStyles))
                {
                    // user had not opted out from dynamic scaling level changes but the primary screen DPI might be 96
                    enableDpiChangedMessageHandling = true;
                }

                // IsScalingRequired returns true if the current resolution is not 96DPI on the primary monitor.
                // However PerMonitor DPI aware applicaitons need dynamic scaling initialized properly even if the
                // the current DPI is 96 because they handle DPI change.
                if ((DpiHelper.IsScalingRequired || enableDpiChangedMessageHandling) && IsDpiAwarenessValueSet())
                {
                    if (IsExpectedConfigValue(ConfigurationStringConstants.CheckedListBoxDisableHighDpiImprovementsKeyName, false))
                    {
                        enableCheckedListBoxHighDpiImprovements = true;
                    }

                    if (IsExpectedConfigValue(ConfigurationStringConstants.ToolStripDisableHighDpiImprovementsKeyName, false))
                    {
                        enableToolStripHighDpiImprovements = true;
                    }

                    if (IsExpectedConfigValue(ConfigurationStringConstants.FormDisableSinglePassScalingOfDpiFormsKeyName, false))
                    {
                        enableSinglePassScalingOfDpiForms = true;
                    }

                    if (IsExpectedConfigValue(ConfigurationStringConstants.DataGridViewControlDisableHighDpiImprovements, false))
                    {
                        enableDataGridViewControlHighDpiImprovements = true;
                    }

                    if (IsExpectedConfigValue(ConfigurationStringConstants.AnchorLayoutDisableHighDpiImprovementsKeyName, false))
                    {
                        enableAnchorLayoutHighDpiImprovements = true;
                    }

                    if (IsExpectedConfigValue(ConfigurationStringConstants.MonthCalendarDisableHighDpiImprovementsKeyName, false))
                    {
                        enableMonthCalendarHighDpiImprovements = true;
                    }

                    // no opt-out switch at the moment
                    enableThreadExceptionDialogHighDpiImprovements = true;
                }
            }
            catch
            {
            }

            isDpiHelperForWinformsInitialized = true;
        }

        /// <summary>
        /// Checks if configuration setting is set to expected value
        /// </summary>
        /// <param name="configurationSettingName">Configuration setting name</param>
        /// <param name="expectedValue">Expected value</param>
        /// <returns>true/false</returns>
        internal static bool IsExpectedConfigValue(string configurationSettingName, bool expectedValue)
        {
            string value = ConfigurationOptions.GetConfigSettingValue(configurationSettingName);
            bool valueAsBool;
            if (!bool.TryParse(value, out valueAsBool))
            {
                // neither 'true' nor 'false' - implies 'false'
                valueAsBool = false;
            }

            return valueAsBool == expectedValue;
        }

        /// <summary>
        /// Sets DPI awareness for the process by reading configuration value from app.config file
        /// </summary>
        /// <returns>true/false - If process DPI awareness is successfully set, returns true. Otherwise false.</returns>
        public static bool SetWinformsApplicationDpiAwareness()
        {
            NativeMethods.PROCESS_DPI_AWARENESS dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNINITIALIZED;
            Version currentOSVersion = Environment.OSVersion.Version;
            if (!IsDpiAwarenessValueSet() || Environment.OSVersion.Platform != System.PlatformID.Win32NT)
            {
                return false;
            }

            string dpiAwareness = (dpiAwarenessValue ?? string.Empty).ToLowerInvariant();
            if (currentOSVersion.CompareTo(ConfigurationOptions.RS2Version) >= 0)
            {
                int rs2AndAboveDpiFlag;
                switch (dpiAwareness)
                {
                    case "true":
                    case "system":
                        rs2AndAboveDpiFlag = NativeMethods.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
                        break;
                    case "true/pm":
                    case "permonitor":
                        rs2AndAboveDpiFlag = NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE;
                        break;
                    case "permonitorv2":
                        rs2AndAboveDpiFlag = NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2;
                        break;
                    case "false":
                    default:
                        rs2AndAboveDpiFlag = NativeMethods.DPI_AWARENESS_CONTEXT_UNAWARE;
                        break;
                }
                if (!SafeNativeMethods.SetProcessDpiAwarenessContext(rs2AndAboveDpiFlag))
                {
                    return false;
                }
            }
            // For operating systems windows 8.1 to Windows 10 redstone 1 version.
            else if (currentOSVersion.CompareTo(new Version(6, 3, 0, 0)) >= 0 && currentOSVersion.CompareTo(ConfigurationOptions.RS2Version) < 0)
            {
                switch (dpiAwareness)
                {
                    case "false":
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                        break;
                    case "true":
                    case "system":
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE;
                        break;
                    case "true/pm":
                    case "permonitor":
                    case "permonitorv2":
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;
                        break;
                    default:
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNINITIALIZED;
                        break;
                }

                if (SafeNativeMethods.SetProcessDpiAwareness(dpiFlag) != NativeMethods.S_OK)
                {
                    return false;
                }

            }
            // For operating systems windows 7 to windows 8
            else if (currentOSVersion.CompareTo(new Version(6, 1, 0, 0)) >= 0 && currentOSVersion.CompareTo(new Version(6, 3, 0, 0)) < 0)
            {
                switch (dpiAwareness)
                {
                    case "false":
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                        break;
                    case "true":
                    case "system":
                    case "true/pm":
                    case "permonitor":
                    case "permonitorv2":
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE;
                        break;
                    default:
                        dpiFlag = NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNINITIALIZED;
                        break;
                }

                if (dpiFlag == NativeMethods.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE)
                {
                    if (!SafeNativeMethods.SetProcessDPIAware())
                    {
                        return false;
                    }
                }
            }
            // For windows vista and below, this is not a supported feature.
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a new Padding with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalPadding">Padding in logical units</param>
        /// <returns>Padding in device units</returns>
        public static Padding LogicalToDeviceUnits(Padding logicalPadding, int deviceDpi = 0)
        {
            return new Padding(LogicalToDeviceUnits(logicalPadding.Left, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Top, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Right, deviceDpi),
                               LogicalToDeviceUnits(logicalPadding.Bottom, deviceDpi));
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable ToolStrip HighDPI fixes.
        /// </summary>
        internal static bool EnableToolStripHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableToolStripHighDpiImprovements;
            }
        }
       
        /// <summary>
        /// Returns a boolean to specify if we should enable processing of WM_DPICHANGED and related messages
        /// </summary>
        internal static bool EnableDpiChangedMessageHandling
        {
            get 
            {
                InitializeDpiHelperForWinforms();
                if (enableDpiChangedMessageHandling)
                {
                    // We can't cache this value because different top level windows can have different DPI awareness context
                    // for mixed mode applications.
                    int dpiAwareness = SafeNativeMethods.GetThreadDpiAwarenessContext();
                    return SafeNativeMethods.AreDpiAwarenessContextsEqual(dpiAwareness, NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable all high Dpi improvements.
        /// </summary>
        internal static bool EnableCheckedListBoxHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableCheckedListBoxHighDpiImprovements;
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable single-pass scaling of controls.
        /// </summary>
        internal static bool EnableSinglePassScalingOfDpiForms
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableSinglePassScalingOfDpiForms;
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable ThreadExceptionDialog HighDPI fixes.
        /// </summary>
        internal static bool EnableThreadExceptionDialogHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableThreadExceptionDialogHighDpiImprovements;
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable DataGridViewControl High Dpi improvements
        /// </summary>
        internal static bool EnableDataGridViewControlHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableDataGridViewControlHighDpiImprovements;
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable Control Anchor HighDPI fixes
        /// </summary>
        internal static bool EnableAnchorLayoutHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableAnchorLayoutHighDpiImprovements;
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable MonthCalendar HighDPI fixes
        /// </summary>
        internal static bool EnableMonthCalendarHighDpiImprovements
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return enableMonthCalendarHighDpiImprovements;
            }
        }
    }
}
