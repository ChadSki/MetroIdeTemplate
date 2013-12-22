﻿using System;
using Assembly.Helpers.Net;
using Assembly.Metro.Dialogs;

namespace Assembly.Helpers
{
    public class Updater
    {
        public static void BeginUpdateProcess()
        {
            // Grab JSON Update package from the server
            UpdateInfo info = Updates.GetUpdateInfo();

            // If the request failed, tell the user to gtfo
            if (info == null || !info.Successful)
            {
                App.AssemblyStorage.AssemblySettings.HomeWindow.Dispatcher.Invoke(new Action(
                    () =>
                        MetroMessageBox.Show("Update Check Failed",
                            "Assembly is unable to check for updates at this time. Sorry :(")));
                return;
            }

            App.AssemblyStorage.AssemblySettings.HomeWindow.Dispatcher.Invoke(
                new Action(() => MetroUpdateDialog.Show(info, UpdateAvailable(info))));
        }

        public static bool UpdateAvailable(UpdateInfo info)
        {
            if (info == null || !info.Successful)
                return false;

            // Just convert the version strings to ints and compare
            long serverVersion = VersionStringToInt(info.LatestVersion);
            long localVersion = VersionStringToInt(VariousFunctions.GetApplicationVersion());

            return (serverVersion > localVersion);
        }

        private static Int64 VersionStringToInt(string version)
        {
            version = version.Replace(".", "");
            Int64 versionInt;

            return Int64.TryParse(version, out versionInt) ? versionInt : 0;
        }
    }
}