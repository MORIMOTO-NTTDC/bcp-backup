using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace CommonLibrary
{
    public static class Configuration
    {

        public static void WriteToRegistry(string key, string value)
        {
            using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(Constants.RegistryKeyPath))
            {
                if (registryKey != null)
                {
                    registryKey.SetValue(key, value);
#if DEBUG
                    Console.WriteLine($"レジストリに {key} = {value} を書き込みました。");
#endif
                }
                else
                {
#if DEBUG
                    Console.WriteLine("レジストリキーを開けませんでした。");
#endif
                }
            }
        }

        public static string ReadFromRegistry(string key)
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Constants.RegistryKeyPath))
            {
                if (registryKey != null)
                {
                    return registryKey.GetValue(key) as string;
                }
                else
                {
#if DEBUG
                    Console.WriteLine("レジストリキーが見つかりませんでした。");
#endif
                    return null;
                }
            }
        }

        public static void DeleteFromRegistry(string key)
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Constants.RegistryKeyPath, true))
            {
                if (registryKey != null)
                {
                    registryKey.DeleteValue(key);
#if DEBUG
                    Console.WriteLine($"レジストリから {key} を削除しました。");
#endif
                }
                else
                {
#if DEBUG
                    Console.WriteLine("レジストリキーが見つかりませんでした。");
#endif
                }
            }
        }

        public static void DeleteAllRegistry()
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Constants.RegistryKeySoftware + @"\", true))
            {
                if (registryKey != null)
                {
                    registryKey.DeleteSubKeyTree(Constants.RegistryKeyBcpsoft);
#if DEBUG
                    Console.WriteLine($"レジストリから bcpSoft を削除しました。");
#endif
                }
                else
                {
#if DEBUG
                    Console.WriteLine("レジストリキーが見つかりませんでした。");
#endif
                }
            }
        }

        public static bool ExistsInRegistry(string key)
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Constants.RegistryKeyPath))
            {
                return registryKey?.GetValue(key) != null;
            }
        }

        /// <summary>
        ///     管理者権限で実行されているか確認します。
        /// </summary>
        /// <returns>確認結果</returns>
        public static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

    }
}
