namespace WorkerServiceNet6.Config;

public interface IScheduleConfig<T>
{
	string CronExpression { get; set; }
	bool IncludeSeconds { get; set; }
	TimeZoneInfo TimeZoneInfo { get; set; }
}
