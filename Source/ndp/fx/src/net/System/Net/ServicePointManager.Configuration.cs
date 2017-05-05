//------------------------------------------------------------------------------
// <copyright file="ServicePointManager.Configuration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net
{
    using Diagnostics;
    using System.Security.Authentication;
    using System.Threading;

    public partial class ServicePointManager
    {
        private static object s_configurationLoadedLock = new object();
        private static volatile bool s_configurationLoaded = false;

        private const string RegistryGlobalStrongCryptoName = "SchUseStrongCrypto";
        private const string RegistryGlobalReusePortName = "HWRPortReuseOnSocketBind";
        private const string RegistryGlobalSendAuxRecordName = "SchSendAuxRecord";
        private const string RegistryLocalSendAuxRecordName = "System.Net.ServicePointManager.SchSendAuxRecord";
        private const string RegistryGlobalSystemDefaultTlsVersionsName = "SystemDefaultTlsVersions";
        private const string RegistryLocalSystemDefaultTlsVersionsName = "System.Net.ServicePointManager.SystemDefaultTlsVersions";
        private const string RegistryLocalSecureProtocolName = "System.Net.ServicePointManager.SecurityProtocol";

        private static void LoadConfiguration()
        {
            s_reusePort = TryInitialize(LoadReusePortConfiguration, false);

            // Ordering of the initialization statements below is important.
            s_disableStrongCrypto = TryInitialize(LoadDisableStrongCryptoConfiguration, true);
            s_disableSendAuxRecord = TryInitialize(LoadDisableSendAuxRecordConfiguration, false);
            s_disableSystemDefaultTlsVersions = TryInitialize(LoadDisableSystemDefaultTlsVersionsConfiguration, true);

            s_defaultSslProtocols = TryInitialize(LoadSecureProtocolConfiguration, SslProtocols.Ssl3 | SslProtocols.Tls);
            s_SecurityProtocolType = (SecurityProtocolType)s_defaultSslProtocols;
        }

        private static bool LoadDisableStrongCryptoConfiguration(bool disable)
        {
            int schUseStrongCryptoKeyValue = 0;

            if (LocalAppContextSwitches.DontEnableSchUseStrongCrypto)
            {
                // .Net 4.5.2 and below will disable SchStrongCrypto unless the registry key is specifically set to 1.
                schUseStrongCryptoKeyValue = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalStrongCryptoName, 0);
                disable = schUseStrongCryptoKeyValue != 1;
            }
            else
            {
                // .Net 4.6 and above will enable SchStrongCrypto unless the registry key is specifically set to 0.
                schUseStrongCryptoKeyValue = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalStrongCryptoName, 1);
                disable = schUseStrongCryptoKeyValue == 0;
            }

            return disable;
        }

        private static bool LoadDisableSendAuxRecordConfiguration(bool disable)
        {
            int schSendAuxRecordKeyValue;
            schSendAuxRecordKeyValue = RegistryConfiguration.AppConfigReadInt(RegistryLocalSendAuxRecordName, 1);
            if (schSendAuxRecordKeyValue == 0)
            {
                return true;
            }

            schSendAuxRecordKeyValue = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalSendAuxRecordName, 1);
            if (schSendAuxRecordKeyValue == 0)
            {
                return true;
            }

            return disable;
        }

        private static bool LoadDisableSystemDefaultTlsVersionsConfiguration(bool disable)
        {
            if (LocalAppContextSwitches.DontEnableSystemDefaultTlsVersions)
            {
                // .Net 4.6.2 and below will disable SystemDefaultTls unless the registry key is specifically set to 1.
                int globalOverride = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalSystemDefaultTlsVersionsName, 0);
                disable = globalOverride != 1;
            }
            else
            {
                // .Net 4.6.3 and above will enable SystemDefaultTls unless the registry key is specifically set to 0.
                int globalOverride = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalSystemDefaultTlsVersionsName, 1);
                disable = globalOverride == 0;
            }

            if (!disable)
            {
                int appLocalOverride = RegistryConfiguration.AppConfigReadInt(RegistryLocalSystemDefaultTlsVersionsName, 1);
                disable = appLocalOverride != 1;
            }

            return disable;
        }

        private static SslProtocols LoadSecureProtocolConfiguration(SslProtocols defaultValue)
        {
            if (!s_disableSystemDefaultTlsVersions)
            {
                defaultValue = SslProtocols.None;
            }
            else if (!s_disableStrongCrypto)
            {
                defaultValue = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            }
            else
            {
                defaultValue = SslProtocols.Tls | SslProtocols.Ssl3;
            }

            if (!s_disableStrongCrypto || !s_disableSystemDefaultTlsVersions)
            {
                string appSetting = RegistryConfiguration.AppConfigReadString(RegistryLocalSecureProtocolName, null);

                SecurityProtocolType value;
                if (Enum.TryParse(appSetting, out value))
                {
                    ValidateSecurityProtocol(value);
                    defaultValue = (SslProtocols)value;
                }
            }

            return defaultValue;
        }

        private static bool LoadReusePortConfiguration(bool reusePortInternal)
        {
            int reusePortKeyValue = 0;
            reusePortKeyValue = RegistryConfiguration.GlobalConfigReadInt(RegistryGlobalReusePortName, 0);

            if (reusePortKeyValue == 1)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.Web, typeof(ServicePointManager), SR.GetString(SR.net_log_set_socketoption_reuseport_default_on));
                }

                reusePortInternal = true;
            }

            return reusePortInternal;
        }

        private static void EnsureConfigurationLoaded()
        {
            if (s_configurationLoaded)
            {
                return;
            }

            lock (s_configurationLoadedLock)
            {
                if (s_configurationLoaded)
                {
                    return;
                }

                LoadConfiguration();
                s_configurationLoaded = true;
            }
        }

        private static T TryInitialize<T>(Func<T, T> loadConfiguration, T fallbackDefault)
        {
            T ret;

            try
            {
                ret = loadConfiguration(fallbackDefault);
            }
            catch (Exception e)
            {
                if (NclUtilities.IsFatal(e))
                {
                    throw;
                }

                Debug.Fail("ServicePointManager.TryInitialize failed with exception", e.ToString());
                ret = fallbackDefault;
            }

            return ret;
        }
    }
}
