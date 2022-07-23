namespace WorkerServiceNet6.Config;

public class ScheduleConfig<T> : IScheduleConfig<T>
{
	public string CronExpression { get; set; }
	public bool IncludeSeconds { get; set; }
	public TimeZoneInfo TimeZoneInfo { get; set; }
}