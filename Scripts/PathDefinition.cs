using System;
using System.IO;

namespace DailyProject_221204
{
    static class PathDefinition
    {
        public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string DataDirecotryPath = Path.Combine(RootPath, @"Data");

        public static readonly string TaskDirectoryPath = Path.Combine(DataDirecotryPath, @"Tasks");
        public static readonly string SchedulesDirectoryPath = Path.Combine(DataDirecotryPath, @"Schedules");
        
        public static readonly string ReflectionsDirectoryPath = Path.Combine(DataDirecotryPath, @"Reflections");
        public static readonly string DayReflectionsDirectoryPath = Path.Combine(ReflectionsDirectoryPath, @"Day");
        public static readonly string WeekReflectionsDirectoryPath = Path.Combine(ReflectionsDirectoryPath, @"Week");

        public static readonly string DailyMemoDirectoryPath = Path.Combine(DataDirecotryPath, @"DailyMemo");

        public static readonly string DateFormat = "yyyyMMdd";
    }
}
