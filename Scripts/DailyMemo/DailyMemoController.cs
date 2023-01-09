using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DailyProject_221204
{
    public class DailyMemoController
    {
        public DailyMemoController(DPDomain dpDomain, Window mainWindow)
        {
            mainWindow.Subscribe(dpDomain.GlobalHotKey.SubscribeGlobalHotKey(_openText, Key.E, ModifierKeys.Alt));
        }

        void _openText()
        {
            var todayDateTimeText = DateTime.Today.ToString(PathDefinition.DateFormat);
            var todayMemoFilePath = Path.Combine(PathDefinition.DailyMemoDirectoryPath, $"{todayDateTimeText}.md");

            var todayMemofileInfo = new FileInfo(todayMemoFilePath);
            var todayMemoDirectory = todayMemofileInfo.Directory;

            if (todayMemoDirectory != null)
            {
                if (todayMemoDirectory.Exists == false)
                {
                    todayMemoDirectory.Create();
                }

                if (todayMemofileInfo.Exists == false)
                {
                    var file = todayMemofileInfo.Create();
                    file.Dispose();
                }
            }

            var processStartInfo = new ProcessStartInfo("cmd", $"/c code -n {todayMemoFilePath}");
            var process = Process.Start(processStartInfo);
        }
    }
}
