using System;
using System.IO;

namespace DailyProject_221204
{
    static class PathDefinition
    {
        public static string RootPath = AppDomain.CurrentDomain.BaseDirectory;

        public static string TaskJsonDirectoryPath = Path.Combine(RootPath, @"Data\Tasks");
        public static string SchedulesJsonDirectoryPath = Path.Combine(RootPath, @"Data\Schedules");
        public static string ReflectionsJsonDirectoryPath = Path.Combine(RootPath, @"Data\Reflections");
        public static string DayReflectionsJsonDirectoryPath = Path.Combine(ReflectionsJsonDirectoryPath, @"Day");
        public static string WeekReflectionsJsonDirectoryPath = Path.Combine(ReflectionsJsonDirectoryPath, @"Week");
    }
}
