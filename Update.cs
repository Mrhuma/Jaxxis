using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Squirrel;

namespace Jaxxis
{
    class Update
    {
        static string newBuildLocation = "C:\\Jaxxis\\Releases";

        public void StartTimer()
        {
            var updateTimer = new Timer(_ => Callback(), null, 0, 3600000);
        }

        private void Callback()
        {
            Task.Run(() => CheckForUpdate()).GetAwaiter();
        }

        //Check for an updated build from the newBuildLocation path.
        private static async void CheckForUpdate()
        {
            try
            {
                string versionNum = "";
                string exePath = "";
                bool needsRestart = false;
                using (var mgr = new UpdateManager(newBuildLocation))
                {
                    UpdateInfo updateInfo = await Task.Run(() => mgr.CheckForUpdate().Result);

                    if (updateInfo.ReleasesToApply.Any())
                    {
                        versionNum = updateInfo.ReleasesToApply.First().Version.ToString();
                        exePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), $"app-{versionNum}\\Jaxxis.exe");
                        await mgr.UpdateApp();
                        mgr.CreateShortcutsForExecutable(exePath, ShortcutLocation.Desktop, false);
                        needsRestart = true;
                    }
                    else
                    {
                        versionNum = updateInfo.CurrentlyInstalledVersion.Version.ToString();
                    }
                }

                if (needsRestart)
                {
                    UpdateManager.RestartApp();
                }
            }

            catch (Exception ex)
            {
                await Global.LogError(ex);
            }
        }
    }
}