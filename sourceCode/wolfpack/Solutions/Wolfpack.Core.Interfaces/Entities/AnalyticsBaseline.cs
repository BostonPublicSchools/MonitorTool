using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public static class AnalyticsBaseline
    {
        public static readonly DateTime BucketBaselineDate = new DateTime(2011, 01, 01);
        public static int MinuteBucket { get { return (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalMinutes;}}
        public static int HourBucket { get { return (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalHours;}}
        public static int DayBucket { get { return (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalDays;}}
    }
}