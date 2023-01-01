using System.Diagnostics;

namespace DailyProject_221204
{
    /// <summary>
    /// DailyProject_221204の汎用デバッグ関数群
    /// </summary>
    public static class DPDebug
    {
        public static void WriteLine(string text)
        {
            Debug.WriteLine($"[DP]{text}");
        }
    }
}
