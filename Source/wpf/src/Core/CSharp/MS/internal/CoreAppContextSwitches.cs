//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// 
// File: CoreAppContextSwitches.cs
//---------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace MS.Internal
{
    // WPF's builds are seeing new warnings as as result of using LocalAppContext in PresentationFramework, PresentationCore and WindowsBase. 
    // These binaries have internalsVisibleTo attribute set between them - which results in the warning. 
    // We don't have a way of suppressing this warning effectively until the shared copies of LocalAppContext and 
    // AppContextDefaultValues have pragmas added to suppress warning 436
#pragma warning disable 436
    internal static class CoreAppContextSwitches
    {
        #region DoNotScaleForDpiChanges

        internal const string DoNotScaleForDpiChangesSwitchName = "Switch.System.Windows.DoNotScaleForDpiChanges";
        private static int _doNotScaleForDpiChanges;
        public static bool DoNotScaleForDpiChanges
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(DoNotScaleForDpiChangesSwitchName, ref _doNotScaleForDpiChanges);
            }
        }

        #endregion

        #region DisableStylusAndTouchSupport

        // DDVSO:221075
        // Switch to disable WPF Stylus and Touch Support
        internal const string DisableStylusAndTouchSupportSwitchName = "Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport";
        private static int _disableStylusAndTouchSupport;
        public static bool DisableStylusAndTouchSupport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(DisableStylusAndTouchSupportSwitchName, ref _disableStylusAndTouchSupport);
            }
        }

        #endregion

        #region EnablePointerSupport

        // DDVSO:197685
        // Switch to enable WPF support for the WM_POINTER based stylus/touch stack
        internal const string EnablePointerSupportSwitchName = "Switch.System.Windows.Input.Stylus.EnablePointerSupport";
        private static int _enablePointerSupport;
        public static bool EnablePointerSupport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(EnablePointerSupportSwitchName, ref _enablePointerSupport);
            }
        }

        #endregion

        #region OverrideExceptionWithNullReferenceException

        // DDVSO:223209
        // Switch to enable the correct exception being thrown in ImageSourceConverter.ConvertFrom instead of NullReferenceException
        internal const string OverrideExceptionWithNullReferenceExceptionName = "Switch.System.Windows.Media.ImageSourceConverter.OverrideExceptionWithNullReferenceException";
        private static int _overrideExceptionWithNullReferenceException;
        public static bool OverrideExceptionWithNullReferenceException
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(OverrideExceptionWithNullReferenceExceptionName, ref _overrideExceptionWithNullReferenceException);
            }
        }

        #endregion
    }
#pragma warning restore 436
}
