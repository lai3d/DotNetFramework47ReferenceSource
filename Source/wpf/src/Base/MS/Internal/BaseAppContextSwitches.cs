//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// 
// File: BaseAppContextSwitches.cs
//---------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace MS.Internal
{

    // There are cases where we have multiple assemblies that are going to import this file and 
    // if they are going to also have InternalsVisibleTo between them, there will be a compiler warning
    // that the type is found both in the source and in a referenced assembly. The compiler will prefer 
    // the version of the type defined in the source
    //
    // In order to disable the warning for this type we are disabling this warning for this entire file.
    #pragma warning disable 436

    /// <summary>
    /// Appcompat switches used by WindowsBase. See comments at the start of each switch. 
    /// Also see AppContextDefaultValues which initializes default values for each of 
    /// these switches.
    /// </summary>
    internal static class BaseAppContextSwitches
    {
        /// <summary>
        /// Starting .NET 4.6, ExecutionContext tracks Thread.CurrentCulture and Thread.CurrentUICulture, which would be restored 
        /// to their respective previous values after a call to ExecutionContext.Run. This behavior is undesirable within the 
        /// Dispatcher - various dispatcher operations can run user code that can in turn set Thread.CurrentCulture or 
        /// Thread.CurrentUICulture, and we do not want those values to be overwritten with their respective previous values. 
        /// To work around the new ExecutionContext behavior, we introduce CulturePreservingExecutionContext for use within 
        /// Dispatcher and DispatcherOperation. WPF in .NET 4.6 & 4.6.1 shipped with buggy behavior - each DispatcherOperation 
        /// ends with all modificaitons to culture infos being reverted.Though unlikely, if some applications targeting 4.6 or 
        /// above might have taken a dependence on this bug, we provide this compatiblity switch that can be enabled by the application.
        /// </summary>
        #region UseCulturePreservingDispatcherOperations

        internal const string SwitchDoNotUseCulturePreservingDispatcherOperations = "Switch.MS.Internal.DoNotUseCulturePreservingDispatcherOperations";
        private static int _doNotUseCulturePreservingDispatcherOperations;

        public static bool DoNotUseCulturePreservingDispatcherOperations
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(SwitchDoNotUseCulturePreservingDispatcherOperations, ref _doNotUseCulturePreservingDispatcherOperations);
            }
        }

        #endregion
    }

    #pragma warning restore 436
}
