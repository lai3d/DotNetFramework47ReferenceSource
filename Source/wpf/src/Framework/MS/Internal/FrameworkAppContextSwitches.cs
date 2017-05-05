//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// File: FrameworkAppContextSwitches.cs
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

    internal static class FrameworkAppContextSwitches
    {
        internal const string DoNotApplyLayoutRoundingToMarginsAndBorderThicknessSwitchName = "Switch.MS.Internal.DoNotApplyLayoutRoundingToMarginsAndBorderThickness";
        private static int _doNotApplyLayoutRoundingToMarginsAndBorderThickness;
        public static bool DoNotApplyLayoutRoundingToMarginsAndBorderThickness
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(DoNotApplyLayoutRoundingToMarginsAndBorderThicknessSwitchName, ref _doNotApplyLayoutRoundingToMarginsAndBorderThickness);
            }
        }

        internal const string GridStarDefinitionsCanExceedAvailableSpaceSwitchName = "Switch.System.Windows.Controls.Grid.StarDefinitionsCanExceedAvailableSpace";
        private static int _gridStarDefinitionsCanExceedAvailableSpace;
        public static bool GridStarDefinitionsCanExceedAvailableSpace
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(GridStarDefinitionsCanExceedAvailableSpaceSwitchName, ref _gridStarDefinitionsCanExceedAvailableSpace);
            }
        }
    }

    #pragma warning restore 436
}
