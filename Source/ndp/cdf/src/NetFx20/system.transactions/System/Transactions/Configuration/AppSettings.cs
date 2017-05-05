//-----------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace System.Transactions.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    internal static class AppSettings
    {
        private static volatile bool settingsInitalized = false;
        private static object appSettingsLock = new object();

        // Bug 954268 Extend Default TransactionException to include the Distributed Transaction ID in error message
        // When the appsetting "Transactions:IncludeDistributedTransactionIdInExceptionMessage" is "true", we display the distributed transaction ID in TransactionException message if the distributed transaction ID is availble.
        private static bool includeDistributedTxIdInExceptionMessage;

        // Bug 954268 Extend Default TransactionException to include the Distributed Transaction ID in error message
        // When the appsetting "Transactions:IncludeDistributedTransactionIdInExceptionMessage" is "true", we display the distributed transaction ID in TransactionException message if the distributed transaction ID is availble.
        internal static bool IncludeDistributedTxIdInExceptionMessage
        {
            get
            {
                EnsureSettingsLoaded();
                return includeDistributedTxIdInExceptionMessage;
            }
        }

        private static void EnsureSettingsLoaded()
        {
            if (!settingsInitalized)
            {
                lock (appSettingsLock)
                {
                    if (!settingsInitalized)
                    {
                        NameValueCollection settings = null;
                        try
                        {
                            settings = ConfigurationManager.AppSettings;
                        }
                        catch (ConfigurationErrorsException)
                        {
                        }
                        finally
                        {
                            if (settings == null || !bool.TryParse(settings["Transactions:IncludeDistributedTransactionIdInExceptionMessage"], out includeDistributedTxIdInExceptionMessage))
                            {
                                includeDistributedTxIdInExceptionMessage = false;
                            }
 
                            settingsInitalized = true;
                        }
                    }
               }
            }
        }
    }
}
